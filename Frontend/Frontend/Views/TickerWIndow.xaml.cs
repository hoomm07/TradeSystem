using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
