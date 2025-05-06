using Backend.Models;
using System.Threading.Tasks;

namespace Backend.Service
{
    public class TickerService
    {
        public List<OrderBook> GetOrderBook(string tickerName)
        {
            List<OrderBook> orderBook = new List<OrderBook>();
            int totalQuantity = 0;

            if (TickerDatas.TickerAsks.TryGetValue(tickerName, out var asks))
            {
                foreach (var ask in asks.Reverse().Take(10))
                {
                    totalQuantity = ask.Value.Sum(x => x.Quantity);

                    orderBook.Add(new OrderBook
                    {
                        Layer = TickerLayer.Ask,
                        Price = ask.Key,
                        Quantity = totalQuantity
                    });
                }
            }

            if (TickerDatas.TickerBids.TryGetValue(tickerName, out var bids))
            {
                foreach (var bid in bids.Reverse().Take(10))
                {
                    totalQuantity = bid.Value.Sum(x => x.Quantity);

                    orderBook.Add(new OrderBook
                    {
                        Layer = TickerLayer.Bid,
                        Price = bid.Key,
                        Quantity = totalQuantity
                    });
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

        //Palce Ask: Sell Logic
        public void SellStock(string tickerName, string strSellPrice, string strSellQuantity)
        {
            try
            {
                int intSellPrice = Convert.ToInt32(strSellPrice);
                int intSellQuantity = Convert.ToInt32(strSellQuantity);
                TickerDatas.TickerAsks.TryGetValue(tickerName, out var asks);

                if (TickerDatas.TickerBids.TryGetValue(tickerName, out var bids))
                {
                    int highestBidPrice = bids.Keys.Max();

                    //if the Ask price is higher than the current highest Bids Price
                    //place it on the Ask OrderBook
                    if (intSellPrice > highestBidPrice)
                    {
                        if (!asks.ContainsKey(intSellPrice))
                        {
                            TickerDatas.TickerAsks[tickerName].Add(intSellPrice, new Queue<OrderBookAsks>());
                        }
                        
                        TickerDatas.TickerAsks[tickerName][intSellPrice].Enqueue(new OrderBookAsks
                        {
                            Layer = TickerLayer.Ask,
                            Price = intSellPrice,
                            Quantity = intSellQuantity
                        });
                    }
                }
                else
                {

                }
            }
            catch
            {

            }
        }

        //Place Bid: Buy Logic
        public void BuyStock(string tickerName, string buyPrice, string buyQuantity)
        {
            try
            {
                int price = Convert.ToInt32(buyPrice);
                int quantity = Convert.ToInt32(buyQuantity);

                //if the Bid price is lower than the current lowest Ask Price
                //place it on the Bid OrderBook
                if (TickerDatas.TickerAsks.TryGetValue(tickerName, out var asks))
                {
                    int lowestAskPrice = asks.Keys.Min();

                }
            }
            catch
            {

            }
        }
    }
}
