using Backend.Service;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    public class TickerHubs : Hub
    {
        private readonly TickerService _tickerService = new();

        //for MainWindow to show data of TickerLists.
        public async Task GetTickerLists()
        {
            await Clients.Caller.SendAsync("ReceiveTickerLists", TickerDatas.Tickers);
        }

        //Select Ticker, Ticker data to Caller
        public async Task GetSpecificTickerData(string tickerName)
        {
            var data = _tickerService.GetOrderBook(tickerName);

            if (data is not null)
            {
                await Clients.Caller.SendAsync("ReceiveSpecificTickerData", data);
            }
        }

        //TradeHistory info to Caller
        public async Task GetTradeHistory()
        {
            var data = _tickerService.GetTradeHistory();

            if (data is not null)
            {
                await Clients.Caller.SendAsync("ReceiveTradeHistory", data);
            }
        }

        //Add to Group by each Ticker. When Disconnected, signalR gets rid of ConnectionId Auto.
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task PlaceAsk(string tickerName, string sellPrice, string sellQuantity)
        {
            _tickerService.SellPrice(sellPrice, sellQuantity, tickerName);
        }

        public async Task PlaceBid(string tickerName, string buyPrice, string buyQuantity)
        {
            _tickerService.
        }
    }
}
