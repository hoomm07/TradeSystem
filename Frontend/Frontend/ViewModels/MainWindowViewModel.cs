using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Views;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using static Frontend.TickerModels;

namespace Frontend
{
    public partial class MainWindowViewModel : ObservableObject
    {
        #region Member
        private readonly HubConnection _hubconnection;

        private string _colorConnected = "LightGreen";
        private string _colorDisconnected = "Red";

        private static TimeSpan[] DefaultRetryDelaysCustom = 
            Enumerable.Repeat(TimeSpan.FromSeconds(1), 30).ToArray();
        #endregion

        #region ObservableProperty
        [ObservableProperty]
        private ObservableCollection<Ticker> _tickers = new();

        [ObservableProperty]
        private TickerModels.Ticker _selectedTicker = new();

        [ObservableProperty]
        private string _connectionText;

        [ObservableProperty]
        private bool _boolConnection = false;

        [ObservableProperty]
        private string _connectionColor;
        #endregion

        public MainWindowViewModel() 
        {
            _hubconnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5031/Ticker")
                .WithAutomaticReconnect(DefaultRetryDelaysCustom)
                .Build();

            _hubconnection.On("ReceiveTickerLists", (Action<List<Ticker>>)((receivedTickers) =>
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Tickers.Clear();
                    foreach (var ticker in receivedTickers)
                        Tickers.Add(ticker);
                }));
            }));

            _hubconnection.Reconnecting += _ =>
            {
                NotifyReconnecting();
                return Task.CompletedTask;
            };

            _hubconnection.Reconnected += _ =>
            {
                NotifyReconnected();
                return Task.CompletedTask;
            };

            _hubconnection.Closed += _ =>
            {
                NotifyClosed();
                return Task.CompletedTask;
            };
        }

        #region Method
        internal async Task LoadAsync()
        {
            int i = 0;
            while (i < 30)
            {
                try
                {
                    await _hubconnection.StartAsync();
                    await _hubconnection.InvokeAsync("GetTickerLists");
                    ConnectionText = "Select a ticker.";
                    ConnectionColor = _colorConnected;
                    BoolConnection = true;
                    break;
                }
                catch
                {
                    if (i == 29)
                    {
                        ConnectionText = "Fail to Connect.\nNo Server detected.";
                        ConnectionColor = _colorDisconnected;
                        break;
                    }
                    else
                    {
                        ConnectionText = "No Connection. \nTrying to Connect.";
                        ConnectionColor = _colorDisconnected;
                        i++;
                        await Task.Delay(1000);
                    }
                }
            }
        }

        private void NotifyReconnecting()
        {
            BoolConnection = false;
            ConnectionText = "Disconnected.\nTrying to Reconnect.";
            ConnectionColor = _colorDisconnected;
        }

        private void NotifyReconnected()
        {
            BoolConnection = true;
            ConnectionColor = _colorConnected;
            ConnectionText = "Select a Ticker.";
        }

        private void NotifyClosed()
        {
            BoolConnection = false;
            ConnectionText = "Fail to Reconnect.\nNo Server Detected.";
            ConnectionColor = _colorDisconnected;
        }
        #endregion

        #region RelayCommand
        [RelayCommand]
        private void SelectClick()
        {
            if (_selectedTicker != null)
            {
                TickerWindow tickerwindow = new TickerWindow(_selectedTicker);
                tickerwindow.Show();
            }
        }
        #endregion
    }
}
