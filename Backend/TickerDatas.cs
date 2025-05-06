using Backend.Models;

namespace Backend
{
    public static class TickerDatas
    {
        #region Member
        //Fake Ticker data Generate.
        public static List<Ticker> Tickers { get; }

        //Always have it sorted by using SortedDictionary.
        //Queue for FIFO.
        public static Dictionary<string, SortedDictionary<int, Queue<OrderBookAsks>>> TickerAsks { get; }
        public static Dictionary<string, SortedDictionary<int, Queue<OrderBookBids>>> TickerBids { get; }
        public static List<TradeHistory> TradeHistory { get; }
        #endregion
        static TickerDatas()
        {
            Tickers = new List<Ticker>
            {
                new() { Name = "AAPL" },
                new() { Name = "006800" },
                new() { Name = "MSFT" },
                new() { Name = "TSLA" },
                new() { Name = "AMZN" },
                new() { Name = "GOOGL" },
                new() { Name = "META" },
                new() { Name = "NVDA" },
                new() { Name = "005930" },
                new() { Name = "005380" }
            };

            TickerAsks = new Dictionary<string, SortedDictionary<int, Queue<OrderBookAsks>>>();
            TickerBids = new Dictionary<string, SortedDictionary<int, Queue<OrderBookBids>>>();

            TradeHistory = new List<TradeHistory>();

            foreach (Ticker ticker in Tickers)
            {
                AddTickerData(ticker.Name);
            }
        }

        #region Random Data Generate method
        private static void AddTickerData(string tickerName)
        {
            Random random = new Random();
            int randomPrice = random.Next(40, 120);

            var orderBookAsks = new SortedDictionary<int, Queue<OrderBookAsks>>();
            var orderBookBids = new SortedDictionary<int, Queue<OrderBookBids>>();

            for (int i = 0; i < 10; i++)
            {
                var ask = new OrderBookAsks
                {
                    Layer = TickerLayer.Ask,
                    Price = randomPrice - i,
                    Quantity = random.Next(1, 50)
                };

                orderBookAsks[ask.Price] = new Queue<OrderBookAsks>();

                orderBookAsks[ask.Price].Enqueue(ask);
            }

            for (int i = 10; i < 20; i++)
            {
                var bid = new OrderBookBids
                {
                    Layer= TickerLayer.Bid,
                    Price = randomPrice - i,
                    Quantity= random.Next(1, 50)
                };

                orderBookBids[bid.Price] = new Queue<OrderBookBids>();
                orderBookBids[bid.Price].Enqueue(bid);
            }

            TickerAsks.Add(tickerName, orderBookAsks);
            TickerBids.Add(tickerName, orderBookBids);

            int j = random.Next(1, 3);
            for (int i = 0; i < j; i++)
            {
                var tradeHistory = new TradeHistory
                {
                    Time = DateTime.Now,
                    Name = tickerName,
                    Side = (TradeSide)random.Next(0,2),
                    Price = randomPrice - 9 + random.Next(0,3),
                    Quantity = random.Next(1, 30)
                };

                TradeHistory.Add(tradeHistory);
                Thread.Sleep(random.Next(1000));
            }
        }
        #endregion
    }

}
