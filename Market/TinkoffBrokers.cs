using PSVClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel=Microsoft.Office.Interop.Excel;

namespace Market
{
    public partial class ThisAddIn
    {
        

        // Обновляем котировки на листах с портфелями других брокеров
        private void ribbon_btnRefreshBrokersClicked()
        {
       
            // Портфели брокеров
            Dictionary<string, Dictionary<string, Orderbook>> Brokers = new Dictionary<string, Dictionary<string, Orderbook>>();

            foreach (var broker in ThisAddIn.BrokersName)
            {

                if ((this.Application != null) && (this.Application.ActiveWorkbook != null))
                {

                    Excel.Worksheet BrokerSheet = null;
                    int max_columns = 0;
                    int max_rows = 0;
                    int TickerNameColumn = 0;
                    int LastDateNameColumn = 0;
                    int PriceNameColumn = 0;
                    int NominalNameColumn = 0;

                    // Находим нужный лист
                    BrokerSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, broker, 
                        sh => 
                        {
                            // Находим нужные колонки
                            max_columns = sh.UsedRange.Columns.Count;

                            for (int j = 1; j <= max_columns; j++)
                            {
                                string name = sh.Cells[1, j].Text;

                                if (name.ToLower() == TickerName.ToLower()) TickerNameColumn = j;
                                if (name.ToLower() == LastDateName.ToLower()) LastDateNameColumn = j;
                                if (name.ToLower() == PriceName.ToLower()) PriceNameColumn = j;
                                if (name.ToLower() == NominalName.ToLower()) NominalNameColumn = j;
                            }

                        }
                    );

                    Dictionary<string, Orderbook> BrokerPapers = new Dictionary<string, Orderbook>();

                    if ((BrokerSheet != null) && (TickerNameColumn != 0))
                    {

                        ribbon_btnRefreshValuteCursClicked();

                        BrokerSheet.Activate();
                        max_rows = BrokerSheet.UsedRange.Rows.Count;

                        List<string> Tickers = new List<string>();

                        // заполняем список тикеров
                        for (int _count = 2; _count <= max_rows; _count++)
                        {
                            string ticker = BrokerSheet.Cells[_count, TickerNameColumn].Text;

                            if ((! Tickers.Contains(ticker)) && (! Currencies.IsValuteByTicker(ticker)))
                                Tickers.Add(ticker);
                        }

                        // запрашиваем котировки
                        if (this.Application != null)
                            this.Application.StatusBar = $"Запрашиваем котировки ценных бумаг для портфеля {broker}";
                        
                        LoadBroker(broker, Tickers, BrokerPapers).GetAwaiter().GetResult();
                        
                        Brokers.Add(broker, BrokerPapers);
                    }

                    // заполняем поля в excel
                    if (this.Application != null)
                        this.Application.StatusBar = $"Заполняем котировки ценных бумаг для портфеля {broker} в excel";
                    
                    for (int _count = 2; _count <= max_rows; _count++)
                    {
                        string ticker = BrokerSheet.Cells[_count, TickerNameColumn].Text;

                        if (BrokerPapers.ContainsKey(ticker))
                        {
                            BrokerSheet.Cells[_count, LastDateNameColumn] = DateTime.Today;
                            if (PriceNameColumn != 0)
                                BrokerSheet.Cells[_count, PriceNameColumn] = BrokerPapers[ticker].lastPrice;
                            if ((NominalNameColumn != 0) && (BrokerPapers[ticker].faceValue > 0))
                                BrokerSheet.Cells[_count, NominalNameColumn] = BrokerPapers[ticker].faceValue;
                        }
                    }

                }
            }

            if (this.Application != null)
                this.Application.StatusBar = false;
        }

        static async Task LoadBroker(string BrokerName, List<string> Tickers,  Dictionary<string, Orderbook> BrokerPapers)
        {
            // добавляем котировки валют
            foreach (var item in Currencies.payload.instruments)
            {
                var orderbook = new Orderbook();
                orderbook.faceValue = 0;
                orderbook.lastPrice = 1;
                orderbook.Instrument = new MarketInstrumentPosition();
                orderbook.Instrument.currency = item.currency;
                orderbook.Instrument.lot = 1;
                orderbook.Instrument.ticker = item.ticker;
                BrokerPapers.Add(item.ticker, orderbook);
            }

            // добавляем котировки ценных бумаг
            foreach (var ticker in Tickers)
            { 
                // получаем figi
                var Papers = await GetAsync<MarketInstrumentListResponse>(token, $"https://api-invest.tinkoff.ru/openapi/market/search/by-ticker?ticker={ticker}");

                if ((Papers != null) && (Papers.status == "Ok") && (Papers.payload != null) && (Papers.payload.instruments != null))
                {
                    foreach (var paper in Papers.payload.instruments)
                    {
                        // запрашиваем котировку
                        var Price = await GetAsync<OrderbookResponse>(token, $"https://api-invest.tinkoff.ru/openapi/market/orderbook?figi={paper.figi}&depth=1");
                        if ((Price != null) && (Price.status == "Ok") && (Price.payload != null))
                        {
                            Price.payload.Instrument = paper;
                            BrokerPapers.Add(ticker,Price.payload);
                            break;
                        }
                    }
                }
                else
                {


                }
            }
        }


        public class OrderbookResponse
        {
            public string trackingId { get; set; }
            public Orderbook payload { get; set; }
            public string status { get; set; }
        }

        public class Orderbook
        {
            public string figi { get; set; }
            public int depth { get; set; }
            public List<OrderResponse> bids { get; set; }
            public List<OrderResponse> asks { get; set; }
            public string tradeStatus { get; set; }
            public decimal minPriceIncrement { get; set; }
            public decimal faceValue { get; set; }  // номинал облигации
            public decimal lastPrice { get; set; }  // последняя цена
            public decimal closePrice { get; set; } 
            public decimal limitUp { get; set; }
            public decimal limitDown { get; set; }

            public class OrderResponse
            {
                public decimal price { get; set; }
                public int quantity { get; set; }
            }

            public MarketInstrumentPosition Instrument { get; set; } // Ценная бумага

        }

    }
}






