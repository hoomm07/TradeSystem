using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Windows;
using static Frontend.TickerModels;

namespace Frontend
{
    public partial class TickerWindowViewModel : ObservableObject
    {
        #region Member
        private readonly HubConnection _hubConnection;

        private string _colorConnected = "LightGreen";
        private string _colorDisconnected = "Red";

        private static TimeSpan[] DefaultRetryDelaysCustom =
            Enumerable.Repeat(TimeSpan.FromSeconds(1), 30).ToArray();

        #endregion

        #region ObservableProperty
        [ObservableProperty]
        private string _tickerName;

        [ObservableProperty]
        private string _buyPrice;

        [ObservableProperty] 
        private string _sellPrice;

        [ObservableProperty] 
        private string _buyQuantity;

        [ObservableProperty] 
        private string _sellQuantity;

        [ObservableProperty]
        private List<TickerModels.OrderBook> _orderBook = new();

        [ObservableProperty]
        private List<TickerModels.TradeHistory> _tradeHistory = new();

        [ObservableProperty]
        private string _connectionText;

        [ObservableProperty]
        private string _connectionColor;

        [ObservableProperty]
        private bool _boolConnection;
        #endregion

        public TickerWindowViewModel(TickerModels.Ticker ticker) 
        {
            _tickerName = ticker.Name.ToString();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5031/Ticker")
                .WithAutomaticReconnect(DefaultRetryDelaysCustom)
                .Build();

            _hubConnection.On<List<OrderBook>>
                ("ReceiveSpecificTickerData", receivedOrderBook =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SetDataGridOrderBook(receivedOrderBook);
                    });
                }
            );

            _hubConnection.On<List<TickerModels.TradeHistory>>
                ("ReceiveTradeHistory", receivedTradeHistory =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SetTradeHistory(receivedTradeHistory);
                    });
                }
            );

            _hubConnection.Reconnecting += _ =>
            {
                NotifyReconnecting();
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += _ =>
            {
                NotifyReconnected();
                return Task.CompletedTask;
            };

            _hubConnection.Closed += _ =>
            {
                NotifyClosed();
                return Task.CompletedTask;
            };
        }

        #region method
        internal async void LoadAsync()
        {
            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("GetSpecificTickerData", _tickerName);
            await _hubConnection.InvokeAsync("GetTradeHistory");
            await _hubConnection.InvokeAsync("AddToGroup", _tickerName);
            ConnectionText = "Connected";
            ConnectionColor = _colorConnected;
            BoolConnection = true;
        }

        private void SetDataGridOrderBook(List<OrderBook> orderBook)
        {
            OrderBook.Clear();

            int askLevel = 10;
            int bidLevel = 1;

            foreach (var item in orderBook)
            {
                if (item.Layer == TickerLayer.Ask)
                {
                    item.DisplayName = item.Layer.ToString() + " " + askLevel.ToString();
                    askLevel--;
                }
                else
                {
                    item.DisplayName = item.Layer.ToString() + " " + bidLevel.ToString();
                    bidLevel++;
                }
            }

            OrderBook = orderBook;
        }

        private void SetTradeHistory(List<TickerModels.TradeHistory> tradeHistory)
        {
            TradeHistory = tradeHistory;
        }

        private void NotifyReconnecting()
        {
            BoolConnection = false;
            ConnectionColor = _colorDisconnected;
            ConnectionText = "Reconnecting";
        }

        private void NotifyReconnected()
        {
            BoolConnection = true;
            ConnectionColor = _colorConnected;
            ConnectionText = "Connected";
        }

        private void NotifyClosed()
        {
            BoolConnection = false;
            ConnectionColor = _colorDisconnected;
            ConnectionText = "Connection Failed. Please restart.";
        }

        private bool ValidatePriceQuantity(string price, string quantity)
        {
            bool isValid = false;

            if (int.TryParse(price, out int intPrice) && int.TryParse(quantity, out int intQuantity))
            {
                if (intPrice > 0 && intQuantity > 0)
                {
                    isValid = true;
                }
                else
                {
                    //Error Message of please enter more then 0 
                }
            }

            return isValid;
        }

        #endregion

        #region RelayCommand
        [RelayCommand]
        private async Task SellClick()
        {
            if (ValidatePriceQuantity(SellPrice, SellQuantity))
            {
                await _hubConnection.InvokeAsync("PlaceAsk", TickerName, SellPrice, SellQuantity);
            }
            else
            {

            }
        }

        [RelayCommand]
        private async Task BuyClick()
        {
            if (ValidatePriceQuantity(BuyPrice, BuyQuantity))
            {
                await _hubConnection.InvokeAsync("PlaceBid", TickerName, BuyPrice, BuyQuantity);
            }
            else
            {
                MessageBox.Show(
                    "Please insert valid Price and Quantity.",
                    "Input Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
        #endregion
    }
}
