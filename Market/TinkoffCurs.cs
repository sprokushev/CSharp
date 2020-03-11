using PSVClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Market
{

    // код для загрузки курсаов валют
    public partial class ThisAddIn
    {

        // курсы валют
        public static CurrencyInstrumentListResponse Currencies;


        // Обновляем курсы валют на панели и на вкладке List
        private void ribbon_btnRefreshValuteCursClicked()
        {
            if (this.Application != null)
                this.Application.StatusBar = "Загружаем курсы валют";

            LoadValuteCurs().GetAwaiter().GetResult();

            if (Ribbon != null)
            {
                // обновляем курс на панели
                Ribbon.lbUSD.Label = $"USD={Currencies.GetValuteCursByTicker("USD")} руб.";
                Ribbon.lbEUR.Label = $"EUR={Currencies.GetValuteCursByTicker("EUR")} руб.";
            }

            if ((Application != null) && (this.Application.ActiveWorkbook != null))
            {

                if (this.Application != null)
                    this.Application.StatusBar = $"Обновляем курсы валют на вкладке {ExcelListName}";

                // Находим лист List
                Excel.Worksheet ExcelListSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, ExcelListName, sh => { });

                if ((ExcelListSheet != null) && (ExcelList_columnValuteCurs != 0) && (ExcelList_columnTicker != 0))
                {
                    // обновляем курс на вкладке List
                    int max_columns = ExcelListSheet.UsedRange.Columns.Count;
                    int max_rows = ExcelListSheet.UsedRange.Rows.Count;

                    foreach (var item in Currencies.payload.instruments)
                    {
                        for (int i = 1; i <= max_rows; i++)
                        {
                            if (ExcelListSheet.Cells[i, ExcelList_columnTicker].Text == item.ticker)
                            {
                                ExcelListSheet.Cells[i, ExcelList_columnValuteCurs] = item.ValuteCurs;
                                break;
                            }
                        }
                    }

                }

                if (this.Application != null)
                    this.Application.StatusBar = false;
            }
        }

        // Получем текущие курсы валют
        static async Task LoadValuteCurs()
        {
            decimal _usd = 0;
            decimal _eur = 0;
            decimal _rub = 0;

            Currencies = await GetAsync<CurrencyInstrumentListResponse>(token, "https://api-invest.tinkoff.ru/openapi/market/currencies");
            if ((Currencies != null) && (Currencies.status == "Ok") && (Currencies.payload != null) && (Currencies.payload.instruments != null))
            {
                foreach (var item in Currencies.payload.instruments)
                {
                    item.ticker = item.ticker.Substring(0, 3);
                    item.currency = item.ticker;
                    item.lot = 1;

                    // заполняем цену
                    var Price = await GetAsync<OrderbookResponse>(token, $"https://api-invest.tinkoff.ru/openapi/market/orderbook?figi={item.figi}&depth=1");
                    if ((Price != null) && (Price.status == "Ok") && (Price.payload != null))
                    {
                        item.ValuteCurs = Price.payload.lastPrice;

                        switch (item.ticker)
                        {
                            case "USD": _usd = item.ValuteCurs; break;
                            case "EUR": _eur = item.ValuteCurs; break;
                            case "RUB": item.ValuteCurs = 1;  _rub = 1; break;
                        }
                    }

                }
            }

            // в крайнем случае - определяем курсы доллара и евро по данным центробанка
            if (Currencies == null) Currencies = new CurrencyInstrumentListResponse();
            if (Currencies.payload == null) Currencies.payload = new CurrencyInstrumentList();
            if (Currencies.payload.instruments == null) Currencies.payload.instruments = new List<CurrencyInstrumentList.Position>();

            if (_usd == 0)
            {
                string valuteCode = "USD";
                _usd = CBRF.GetValuteCurs(valuteCode);
                Currencies.payload.instruments.Add(new CurrencyInstrumentList.Position()
                { ticker = valuteCode, name = "Доллары США", currency = valuteCode, lot = 1, ValuteCurs = _usd });
                Currencies.payload.total++;
                Currencies.status = "Ok";
            }
            if (_eur == 0)
            {
                string valuteCode = "EUR";
                _eur = CBRF.GetValuteCurs(valuteCode);
                Currencies.payload.instruments.Add(new CurrencyInstrumentList.Position()
                { ticker = valuteCode, name = "Евро", currency = valuteCode, lot = 1, ValuteCurs = _eur });
                Currencies.payload.total++;
                Currencies.status = "Ok";
            }
            // обязательно добавляем рубль
            if (_rub == 0)
            {
                string valuteCode = "RUB";
                _rub = 1;
                Currencies.payload.instruments.Add(new CurrencyInstrumentList.Position()
                { ticker = valuteCode, name = "Рубль", currency = valuteCode, lot = 1, ValuteCurs = _rub });
                Currencies.payload.total++;
                Currencies.status = "Ok";
            }

        }



        public class CurrencyInstrumentListResponse
        {
            public string trackingId { get; set; }
            public CurrencyInstrumentList payload { get; set; }
            public string status { get; set; }

            public Boolean IsValuteByTicker(string ticker)
            {
                if ((payload!=null) && (payload.instruments != null) && (ticker != null) && (ticker != ""))
                    foreach (var item in payload.instruments)
                    {
                        if (item.ticker == ticker)
                            return true;
                    }
                return false;
            }


            public decimal GetValuteCursByTicker(string ticker)
            {
                if ((payload != null) && (payload.instruments != null) && (ticker != null) && (ticker != ""))
                    foreach (var item in payload.instruments)
                    {
                        if (item.ticker == ticker)
                            return item.ValuteCurs;
                    }
                return 1;
            }

            public string GetNameByTicker(string ticker)
            {
                if ((payload != null) && (payload.instruments != null) && (ticker != null) && (ticker != ""))
                    foreach (var item in payload.instruments)
                    {
                        if (item.ticker == ticker)
                            return item.name;
                    }
                return "";
            }

        }

        public class CurrencyInstrumentList
        {
            public List<Position> instruments { get; set; }
            public int total { get; set; }

            public class Position
            {
                public string figi { get; set; }
                public string ticker { get; set; }
                public string isin { get; set; }
                public decimal minPriceIncrement { get; set; }
                public int lot { get; set; }
                public string currency { get; set; }
                public string name { get; set; }
                public string type { get; set; }

                public string InstrumentTypeName               // Тип ценной бумаги 
                {
                    get
                    {
                        string res = currency;

                        if (MarketTypes != null)
                        {
                            string value="";
                            if (MarketTypes.ContainsKey(ticker))
                                value = MarketTypes[ticker];
                            if ((value != null) && (value != ""))
                                    res = value;
                        }

                        return res;
                    }
                }

                public decimal ValuteCurs { get; set; } // курс валюты

            }
        }






    }
}
