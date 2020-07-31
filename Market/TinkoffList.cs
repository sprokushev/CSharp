using PSVClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Market
{
    public partial class ThisAddIn
    {

        public static List<Paper> ListPapers;
        public static bool ListPapersChanged = true;

        // Заполняем лист со списком ценных бумаг
        public void ribbon_btnMarketListClicked()
        {
            if ((this.Application != null) && (this.Application.ActiveWorkbook != null))
            {
                // Проверяем наличие нужного листа
                Excel.Worksheet ExcelListSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, ExcelListName, sh => { }); 

                if (ExcelListSheet == null)
                {
                    // лист не найден - добавляем
                    ExcelListSheet = (Excel.Worksheet)this.Application.ActiveWorkbook.Sheets.Add();
                    ExcelListSheet.Name = ExcelListName;
                }

                if (ExcelListSheet != null)
                {
                    // заполняем лист
                    int _count = 0;
                    ExcelListSheet.Activate();
                    ExcelListSheet.Cells.ClearContents();

                    // заголовок
                    _count++;
                    ExcelListSheet.Cells[_count, ExcelList_columnTicker] = "Ticker";
                    ExcelListSheet.Cells[_count, 2] = "Type";
                    ExcelListSheet.Cells[_count, 3] = "Name";
                    ExcelListSheet.Cells[_count, 4] = "Lot";
                    ExcelListSheet.Cells[_count, 5] = "Currency";
                    ExcelListSheet.Cells[_count, ExcelList_columnValuteCurs] = "ValuteCurs";

                    if (this.Application != null)
                        this.Application.StatusBar = "Загружаем курсы валют";

                    LoadValuteCurs().GetAwaiter().GetResult();

                    if (this.Application != null)
                        this.Application.StatusBar = $"Заполняем список валют на листе {ExcelListName}";

                    // валюты
                    if ((Currencies != null) && (Currencies.status == "Ok") && (Currencies.payload != null) && (Currencies.payload.instruments != null))
                    {
                        foreach (var item in Currencies.payload.instruments)
                        {
                            if (item.ticker != "")
                            {
                                _count++;
                                ExcelListSheet.Cells[_count, ExcelList_columnTicker] = item.ticker;
                                ExcelListSheet.Cells[_count, 2] = item.InstrumentTypeName;
                                ExcelListSheet.Cells[_count, 3] = item.name;
                                ExcelListSheet.Cells[_count, 4] = item.lot;
                                ExcelListSheet.Cells[_count, 5] = item.currency;
                                ExcelListSheet.Cells[_count, ExcelList_columnValuteCurs] = item.ValuteCurs;
                            }
                        }
                    }

                    if (this.Application != null)
                        this.Application.StatusBar = "Загружаем список ценных бумаг";

                    LoadMarket().GetAwaiter().GetResult();

                    if (this.Application != null)
                        this.Application.StatusBar = $"Заполняем список ценных бумаг на листе {ExcelListName}";

                    // ценные бумаги
                    if (ListPapers != null)
                    {
                        foreach (var item in ListPapers)
                        {
                            _count++;
                            ExcelListSheet.Cells[_count, ExcelList_columnTicker] = item.ticker;
                            ExcelListSheet.Cells[_count, 2] = item.InstrumentTypeName;
                            ExcelListSheet.Cells[_count, 3] = item.name;
                            ExcelListSheet.Cells[_count, 4] = item.lot;
                            ExcelListSheet.Cells[_count, 5] = item.currency;
                        }
                    }

                    // оформляем
                    int max_columns = ExcelListSheet.UsedRange.Columns.Count;
                    int max_rows = ExcelListSheet.UsedRange.Rows.Count;
                    MSExcel.RangeAutoFit(ExcelListSheet.Cells);
                    MSExcel.RangeAutoFilter(ExcelListSheet.Cells);
                    MSExcel.RangeBold((Excel.Range)ExcelListSheet.Range[ExcelListSheet.Cells[1, 1], ExcelListSheet.Cells[1, max_columns]],true);
                    MSExcel.RangeBorder((Excel.Range)ExcelListSheet.Range[ExcelListSheet.Cells[1, 1], ExcelListSheet.Cells[max_rows, max_columns]]);
                }
            }

            if (this.Application != null)
                this.Application.StatusBar = false;
        }


        // Загрузить список ценных бумаг
        static async Task LoadMarket()
        {
            ListPapers = new List<Paper>();
            ListPapersChanged = true;

            // акции
            var Stocks = await GetAsync<MarketInstrumentListResponse>(token, "https://api-invest.tinkoff.ru/openapi/market/stocks");
            if ((Stocks != null) && (Stocks.status == "Ok") && (Stocks.payload != null) && (Stocks.payload.instruments != null))
            {
                foreach (var item in Stocks.payload.instruments)
                {
                    item.type = "Stocks";
                    ListPapers.Add(new Paper()
                    {
                        ticker = item.ticker,
                        InstrumentTypeName = item.InstrumentTypeName,
                        name = item.name,
                        currency = item.currency,
                        lot = item.lot
                    });
                }
            }

            // облигации
            var Bonds = await GetAsync<MarketInstrumentListResponse>(token, "https://api-invest.tinkoff.ru/openapi/market/bonds");
            if ((Bonds != null) && (Bonds.status == "Ok") && (Bonds.payload != null) && (Bonds.payload.instruments != null))
            {
                foreach (var item in Bonds.payload.instruments)
                {
                    item.type = "Bonds";
                    ListPapers.Add(new Paper()
                    {
                        ticker = item.ticker,
                        InstrumentTypeName = item.InstrumentTypeName,
                        name = item.name,
                        currency = item.currency,
                        lot = item.lot
                    });
                }
            }

            // фонды
            var Etfs = await GetAsync<MarketInstrumentListResponse>(token, "https://api-invest.tinkoff.ru/openapi/market/etfs");
            if ((Etfs != null) && (Etfs.status == "Ok") && (Etfs.payload != null) && (Etfs.payload.instruments != null))
            {
                foreach (var item in Etfs.payload.instruments)
                {
                    item.type = "Etfs";
                    ListPapers.Add(new Paper()
                    {
                        ticker = item.ticker,
                        InstrumentTypeName = item.InstrumentTypeName,
                        name = item.name,
                        currency = item.currency,
                        lot = item.lot
                    });
                }
            }

            ListPapersChanged = false;
        }

        public class MarketInstrumentListResponse
        {
            public string trackingId { get; set; }
            public MarketInstrumentList payload { get; set; }
            public string status { get; set; }
        }

        public class MarketInstrumentList
        {
            public List<MarketInstrumentPosition> instruments { get; set; }
            public int total { get; set; }
        }

        public class MarketInstrumentPosition
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
                    string res = "";

                    switch (currency)
                    {
                        case "USD": res = "USD_USA"; break;
                        case "EUR": res = "EUR_EU"; break;
                        case "RUB": res = "RUB_RUS"; break;
                    }

                    switch (type)
                    {
                        case "Bonds":  res = res + "_BOND"; break;
                        case "Etfs": res = res + "_FOND"; break;
                    }

                    if (MarketTypes != null)
                    {
                        string value;
                        if (MarketTypes.TryGetValue(ticker, out value) && (value != null) && (value != ""))
                            res = value;
                    }

                    return res;
                }
            }
        }

        public class Paper
        {
            public string ticker { get; set; }
            public string InstrumentTypeName { get; set; }
            public string name { get; set; }
            public double lot { get; set; }
            public string currency { get; set; }

            public string DisplayText 
            { 
                get 
                {
                    return $"{ticker.PadRight(20)}  {InstrumentTypeName.PadRight(20)}  {name}";
                } 
            }
        }


        public void FillListPapersFromExcel()
        {
            if (ListPapersChanged)
            {
                // Проверяем наличие нужного листа
                Excel.Worksheet ExcelListSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, ExcelListName, sh => { });
                if (ExcelListSheet != null)
                {
                    int max_rows = ExcelListSheet.UsedRange.Rows.Count;
                    ListPapers = new List<Paper>();

                    for (int _count = 2; _count <= max_rows; _count++)
                    //try
                    {
                        if (ExcelListSheet.Cells[_count, ExcelList_columnTicker].Text != "") {
                            ListPapers.Add(new Paper()
                            {
                                ticker = ExcelListSheet.Cells[_count, ExcelList_columnTicker].Text,
                                InstrumentTypeName = ExcelListSheet.Cells[_count, 2].Text,
                                name = ExcelListSheet.Cells[_count, 3].Text,
                                lot = ExcelListSheet.Cells[_count, 4].Value,
                                currency = ExcelListSheet.Cells[_count, 5].Text
                            });
                        }
                    }
                    //catch { }

                    ListPapersChanged = false;
                }

            }
        }


    }
}
