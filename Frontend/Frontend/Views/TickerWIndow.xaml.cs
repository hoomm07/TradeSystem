using System.Windows;


namespace Frontend.Views
{

    public partial class TickerWindow : Window
    {
        TickerWindowViewModel _viewModel;
        public TickerWindow(TickerModels.Ticker tickerName)
        {
            InitializeComponent();
            _viewModel = new TickerWindowViewModel(tickerName);
            this.DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadAsync();
        }
    }
}
