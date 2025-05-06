using Backend.Models;
using System.Threading.Tasks;

namespace Backend.Service
{
    public class TickerService
    {
        public List<OrderBook> GetOrderBook(string tickerName)
        {
            List<OrderBook> orderBook = new List<OrderBook>();

            if (TickerDatas.TickerAsks.TryGetValue(tickerName, out var asks))
            {
                foreach (var ask in asks.Reverse().Take(10))
                {
                    foreach (var askValue in ask.Value)
                    {
                        orderBook.Add(new OrderBook
                        {
                            Layer = askValue.Layer,
                            Price = askValue.Price,
                            Quantity = askValue.Quantity
                        });
                    }
                }
            }

            if (TickerDatas.TickerBids.TryGetValue(tickerName, out var bids))
            {
                foreach (var bid in bids.Reverse().Take(10))
                {
                    foreach (var bidValue in bid.Value)
                    {
                        orderBook.Add(new OrderBook
                        {
                            Layer = bidValue.Layer,
                            Price = bidValue.Price,
                            Quantity = bidValue.Quantity
                        });
                    }
                }
            }

            return orderBook;
        }

        public List<TradeHistory> GetTradeHistory()
        {
            List<TradeHistory> tradeHistoryDesc = new List<TradeHistory>();
            tradeHistoryDesc = TickerDatas.TradeHistory.OrderByDescending(x => x.Time).ToList();
            return tradeHistoryDesc;
        }

        public void SellPrice(string sellPrice, string sellQuantity, string tickerName)
        {
            try
            {
                int price = Convert.ToInt32(sellPrice);
                int quantity = Convert.ToInt32(sellQuantity);

                if (TickerDatas.TickerBids.TryGetValue(tickerName, out var bids))
                {

                }
            }
            catch
            {

            }
        }
    }
}
