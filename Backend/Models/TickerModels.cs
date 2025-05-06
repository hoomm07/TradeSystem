namespace Backend.Models
{
    public class Ticker
    {
        public string Name { get; set; }
    }

    public class OrderBook
    {
        public TickerLayer Layer { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderBookAsks : OrderBook
    {

    }

    public class OrderBookBids : OrderBook
    {

    }

    public class TradeHistory
    {
        public DateTime Time { get; set; }
        public TradeSide Side { get; set; }
        public string Name { get; set;}
        public int Price { get; set;}
        public int Quantity { get; set; }
    }

    public enum TradeSide
    {
        Buy,
        Sell
    }

    public enum TickerLayer
    {
        Ask,
        Bid
    }
}
