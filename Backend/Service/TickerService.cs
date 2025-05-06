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
                var asksTop10 = asks.Take(10);

                foreach (var ask in asksTop10.Reverse())
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
                    else
                    {
                        foreach (var bid in bids.Reverse())
                        {
                            var bidPrice = bid.Key;
                            var bidQueue = bid.Value;
                            int boughtQuantity = 0;

                            //execute through the bid queue til no more bid exist or Sold all.
                            while (intSellQuantity > 0 && bidQueue.Count >0)
                            {
                                var firstBid = bidQueue.Peek();

                                //if Bid's Quantity is less then Sell Quantity
                                if (firstBid.Quantity <= intSellQuantity)
                                {
                                    intSellQuantity -= firstBid.Quantity;
                                    boughtQuantity += firstBid.Quantity;

                                    bidQueue.Dequeue();
                                }
                                else
                                {
                                    firstBid.Quantity -= intSellQuantity;
                                    boughtQuantity += intSellQuantity;
                                    intSellQuantity = 0;
                                }
                            }

                            if (boughtQuantity > 0)
                            {
                                TickerDatas.TradeHistory.Add(new TradeHistory
                                {
                                    Time = DateTime.Now,
                                    Side = TradeSide.Sell,
                                    Name = tickerName,
                                    Price = bidPrice,
                                    Quantity = boughtQuantity
                                });
                            }

                            if (intSellQuantity == 0)
                                return;
                        }

                        //All bids are sold but I still havs some Sell Quantity left.
                        if (intSellQuantity > 0)
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
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Place Bid: Buy Logic
        public void BuyStock(string tickerName, string strBuyPrice, string strBuyQuantity)
        {
            try
            {
                int intBuyPrice = Convert.ToInt32(strBuyPrice);
                int intBuyQuantity = Convert.ToInt32(strBuyQuantity);
                TickerDatas.TickerBids.TryGetValue(tickerName, out var bids);

                if (TickerDatas.TickerAsks.TryGetValue(tickerName, out var asks))
                {
                    int lowestAskPrice = asks.Keys.Max();

                    if (intBuyPrice < lowestAskPrice)
                    {
                        if (!bids.ContainsKey(intBuyPrice))
                        {
                            TickerDatas.TickerBids[tickerName].Add(intBuyPrice, new Queue<OrderBookBids>());
                        }

                        TickerDatas.TickerBids[tickerName][intBuyPrice].Enqueue(new OrderBookBids
                        {
                            Layer = TickerLayer.Bid,
                            Price = intBuyPrice,
                            Quantity = intBuyQuantity
                        });
                    }
                    else
                    {
                        foreach (var ask in asks)
                        {
                            var askPrice = ask.Key;
                            var askQueue = ask.Value;
                            int soldQuantity = 0;

                            //execute through the ask queue til no more bid exist or Sold all.
                            while (intBuyQuantity > 0 && askQueue.Count > 0)
                            {
                                var firstAsk = askQueue.Peek();

                                //if Ask's Quantity is less then Buy Quantity
                                if (firstAsk.Quantity <= intBuyQuantity)
                                {
                                    intBuyQuantity -= firstAsk.Quantity;
                                    soldQuantity += firstAsk.Quantity;

                                    askQueue.Dequeue();
                                }
                                else
                                {
                                    firstAsk.Quantity -= intBuyQuantity;
                                    soldQuantity += intBuyQuantity;
                                    intBuyQuantity = 0;
                                }
                            }

                            if (soldQuantity > 0)
                            {
                                TickerDatas.TradeHistory.Add(new TradeHistory
                                {
                                    Time = DateTime.Now,
                                    Side = TradeSide.Buy,
                                    Name = tickerName,
                                    Price = askPrice,
                                    Quantity = soldQuantity
                                });
                            }

                            if (intBuyQuantity == 0)
                                return;
                        }

                        //All asks are bought but I still havs some Buy Quantity left.
                        if (intBuyQuantity > 0)
                        {
                            if (!bids.ContainsKey(intBuyPrice))
                            {
                                TickerDatas.TickerBids[tickerName].Add(intBuyPrice, new Queue<OrderBookBids>());
                            }

                            TickerDatas.TickerBids[tickerName][intBuyPrice].Enqueue(new OrderBookBids
                            {
                                Layer = TickerLayer.Bid,
                                Price = intBuyPrice,
                                Quantity = intBuyQuantity
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
