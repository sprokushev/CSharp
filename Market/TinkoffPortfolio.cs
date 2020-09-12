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
        // брокерские счета в портфеле Тинькофф
        public static AccountsResponse TinkoffAccounts;
        public static Dictionary<string, PortfolioList> TinkoffPaperAccounts;
        public static Dictionary<string, CurrencyList> TinkoffCurrencyAccounts;

        // Заполняем лист с портфелем Тинькофф
        private void ribbon_btnTinkoffClicked()
        {

            if ((this.Application != null) && (this.Application.ActiveWorkbook != null))
            {
                // Проверяем наличие нужного листа
                Excel.Worksheet TinkoffSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, ExcelTinkoffName, sh => { }); ;

                if (TinkoffSheet == null)
                {
                    // лист не найден - добавляем
                    TinkoffSheet = (Excel.Worksheet)this.Application.ActiveWorkbook.Sheets.Add();
                    TinkoffSheet.Name = ExcelTinkoffName;
                }

                if (TinkoffSheet != null)
                {

                    if (this.Application != null)
                        this.Application.StatusBar = "Загружаем курсы валют";

                    LoadValuteCurs().GetAwaiter().GetResult();

                    if (this.Application != null)
                        this.Application.StatusBar = "Загружаем список ценных бумаг из портфеля Тинькофф";

                    LoadTinkoff().GetAwaiter().GetResult();

                    if (this.Application != null)
                        this.Application.StatusBar = $"Заполняем портфель Тинькофф  на листе {ExcelTinkoffName}";

                    // заполняем лист
                    int _count = 0;
                    TinkoffSheet.Activate();
                    TinkoffSheet.Cells.ClearContents();

                    // заголовок
                    _count++;
                    TinkoffSheet.Cells[_count, 1] = "Счет";
                    TinkoffSheet.Cells[_count, 2] = "Тикер";
                    TinkoffSheet.Cells[_count, 3] = "Ценная бумага";
                    TinkoffSheet.Cells[_count, 4] = "Тип";
                    TinkoffSheet.Cells[_count, 5] = "Кол-во";
                    TinkoffSheet.Cells[_count, 6] = "Валюта";
                    TinkoffSheet.Cells[_count, 7] = "Цена покупки (средняя)";
                    TinkoffSheet.Cells[_count, 8] = "Стоимость покупки (средняя)";
                    TinkoffSheet.Cells[_count, 9] = "Дата котировки";
                    TinkoffSheet.Cells[_count, 10] = "Котировка в валюте";
                    TinkoffSheet.Cells[_count, 11] = "Изменение котировки";
                    TinkoffSheet.Cells[_count, 12] = "Курс к рублю";
                    TinkoffSheet.Cells[_count, 13] = "Котировка в рублях";
                    TinkoffSheet.Cells[_count, 14] = "Рыночная стоимость в валюте";
                    TinkoffSheet.Cells[_count, 15] = "Рыночная стоимость в рублях";

                    // перебираю наличные
                    foreach (var account in TinkoffCurrencyAccounts)
                    {
                        foreach (var pos in account.Value.currencies)
                        {
                            _count++;
                            TinkoffSheet.Cells[_count, 1] = account.Key;
                            TinkoffSheet.Cells[_count, 2] = pos.ticker;
                            TinkoffSheet.Cells[_count, 3] = pos.name;
                            TinkoffSheet.Cells[_count, 4] = pos.InstrumentTypeName;
                            TinkoffSheet.Cells[_count, 5] = pos.Count;
                            TinkoffSheet.Cells[_count, 6] = pos.currency;
                            TinkoffSheet.Cells[_count, 7] = pos.Price;
                            TinkoffSheet.Cells[_count, 8] = pos.Summa;
                            TinkoffSheet.Cells[_count, 9] = DateTime.Today;
                            TinkoffSheet.Cells[_count, 10] = pos.MarketPrice;
                            TinkoffSheet.Cells[_count, 11] = pos.MarketPrice - pos.Price;
                            TinkoffSheet.Cells[_count, 12] = pos.ValuteCurs;
                            TinkoffSheet.Cells[_count, 13] = pos.RubPrice;
                            TinkoffSheet.Cells[_count, 14] = pos.MarketSumma;
                            TinkoffSheet.Cells[_count, 15] = pos.RubSumma;
                        }
                    }

                    // перебираю ценные бумаги
                    foreach (var account in TinkoffPaperAccounts)
                    {
                        foreach (var pos in account.Value.positions)
                        {
                            if (pos.instrumentType == "Currency") continue;
                            if (pos.averagePositionPrice == null) continue;

                            _count++;
                            TinkoffSheet.Cells[_count, 1] = account.Key;
                            TinkoffSheet.Cells[_count, 2] = pos.ticker;
                            TinkoffSheet.Cells[_count, 3] = pos.name;
                            TinkoffSheet.Cells[_count, 4] = pos.InstrumentTypeName;
                            TinkoffSheet.Cells[_count, 5] = pos.Count;
                            TinkoffSheet.Cells[_count, 6] = pos.currency;
                            TinkoffSheet.Cells[_count, 7] = pos.Price;
                            TinkoffSheet.Cells[_count, 8] = pos.Summa;
                            TinkoffSheet.Cells[_count, 9] = DateTime.Today;
                            TinkoffSheet.Cells[_count, 10] = pos.MarketPrice;
                            TinkoffSheet.Cells[_count, 11] = pos.MarketPrice - pos.Price;
                            TinkoffSheet.Cells[_count, 12] = pos.ValuteCurs;
                            TinkoffSheet.Cells[_count, 13] = pos.RubPrice;
                            TinkoffSheet.Cells[_count, 14] = pos.MarketSumma;
                            TinkoffSheet.Cells[_count, 15] = pos.RubSumma;
                        }
                    }

                    // итоги
                    //TinkoffSheet.Cells[_footer_row, 5].FormulaR1C1 = $"=SUBTOTAL(9,R[{-stocks_rows}]C:R[-1]C)";
                    //TinkoffSheet.Cells[_footer_row, 14].FormulaR1C1 = $"=SUBTOTAL(9,R[{-all_rows}]C:R[-1]C)";

                    // оформляем
                    int max_columns = TinkoffSheet.UsedRange.Columns.Count;
                    int max_rows = TinkoffSheet.UsedRange.Rows.Count;
                    MSExcel.RangeBold(TinkoffSheet.Range[TinkoffSheet.Cells[1, 1], TinkoffSheet.Cells[max_rows, max_columns]], false);
                    MSExcel.RangeBold(TinkoffSheet.Range[TinkoffSheet.Cells[1, 1], TinkoffSheet.Cells[1, max_columns]], true);
                    MSExcel.RangeWrapText(TinkoffSheet.Range[TinkoffSheet.Cells[1, 1], TinkoffSheet.Cells[1, max_columns]]);
                    MSExcel.RangeBorder(TinkoffSheet.Range[TinkoffSheet.Cells[1, 1], TinkoffSheet.Cells[max_rows, max_columns]]);
                    MSExcel.RangeAutoFilter(TinkoffSheet.Cells);
                    MSExcel.RangeVerticalAlignment(TinkoffSheet.Cells, Excel.XlVAlign.xlVAlignCenter);
                }
            }

            if (this.Application != null)
                this.Application.StatusBar = false;

        }

        // Загрузить портфели
        static async Task LoadTinkoff()
        {
            //  брокерские счета
            TinkoffAccounts = await GetAsync<AccountsResponse>(token, "https://api-invest.tinkoff.ru/openapi/user/accounts");

            TinkoffPaperAccounts = new Dictionary<string, PortfolioList>();
            TinkoffCurrencyAccounts = new Dictionary<string, CurrencyList>();

            if ((TinkoffAccounts != null) && (TinkoffAccounts.status == "Ok") && (TinkoffAccounts.payload != null) && (TinkoffAccounts.payload.accounts != null))
            {
                foreach (var item in TinkoffAccounts.payload.accounts)
                {
                    // портфели счетов
                    var Portfolio = await GetAsync<PortfolioResponse>(token, $"https://api-invest.tinkoff.ru/openapi/portfolio?brokerAccountId={item.brokerAccountId}");

                    if ((Portfolio != null) && (Portfolio.status == "Ok") && (Portfolio.payload != null))
                    {
                        Portfolio.payload.account = new UserAccount() { brokerAccountId = item.brokerAccountId, brokerAccountType = item.brokerAccountType };
                        TinkoffPaperAccounts.Add(item.brokerAccountId, Portfolio.payload);
                    }

                    // портфели валют
                    var Currency = await GetAsync<CurrencyResponse>(token, $"https://api-invest.tinkoff.ru/openapi/portfolio/currencies?brokerAccountId={item.brokerAccountId}");

                    if ((Currency != null) && (Currency.status == "Ok") && (Currency.payload != null) && (Currency.payload.currencies != null))
                    {
                        bool _finish = false;

                        do
                        {
                            _finish = true;

                            foreach (var item1 in Currency.payload.currencies)
                            {
                                if (item1.currency != "USD")
                                if (item1.currency != "EUR")
                                if (item1.currency != "RUB")
                                {
                                   Currency.payload.currencies.Remove(item1);
                                   _finish = false;
                                   break;
                                }
                            }
                        } while (!_finish);
                    }

                    if ((Currency != null) && (Currency.status == "Ok") && (Currency.payload != null))
                    {
                        Currency.payload.account = new UserAccount() { brokerAccountId = item.brokerAccountId, brokerAccountType = item.brokerAccountType };
                        TinkoffCurrencyAccounts.Add(item.brokerAccountId, Currency.payload);
                    }
                }

            }

        }

        


        public class AccountsResponse
        {
            public string trackingId { get; set; }
            public UserAccountList payload { get; set; }
            public string status { get; set; }
        }

        public class UserAccountList
        {
            public List<UserAccount> accounts { get; set; }
        }

        public class UserAccount
        {
            public string brokerAccountType { get; set; }
            public string brokerAccountId { get; set; }
        }

        public class PortfolioResponse
        {
            public string trackingId { get; set; }
            public PortfolioList payload { get; set; }
            public string status { get; set; }
        }

        public class PortfolioList
        {
            public List<Position> positions { get; set; }
            public UserAccount account { get; set; }

            public class Position
            {
                public string figi { get; set; }
                public string ticker { get; set; }
                public string isin { get; set; }
                public string instrumentType { get; set; }
                public decimal balance { get; set; }
                public decimal blocked { get; set; }
                public MoneyAmount expectedYield { get; set; }
                public int lots { get; set; }
                public MoneyAmount averagePositionPrice { get; set; }
                public MoneyAmount averagePositionPriceNoNkd { get; set; }
                public string name { get; set; }


                public string currency  // Валюта
                {
                    get
                    {
                        string value = "";
                        if (averagePositionPrice != null)
                            value = averagePositionPrice.currency;
                        else
                            if (expectedYield != null)
                                value = expectedYield.currency;
                            else
                                if (averagePositionPriceNoNkd != null)
                                    value = averagePositionPriceNoNkd.currency;

                        return value;
                    }
                }
                
                public string InstrumentTypeName   // Тип ценной бумаги 
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

                        if (MarketTypes != null)
                        {
                            string value;
                            if (MarketTypes.TryGetValue(ticker, out value) && (value != null) && (value != ""))
                                res = value;
                        }

                        return res;
                    }
                }

                public decimal Count    // Кол-во ценных бумаг
                {
                    get
                    {
                        return balance;
                    }
                }

                public decimal Price    //Цена покупки ценных бумаг (средневзвешенная)
                {
                    get
                    {
                        decimal value = 0;
                        if (Count != 0) value = Summa / Count;
                        return value;
                    }
                }


                public decimal Summa    // Сумма покупки ценных бумаг (средневзвешенная)
                {
                    get
                    {
                        decimal value = 0;
                        if (averagePositionPrice != null)
                            value = balance * averagePositionPrice.value;
                        return value;
                    }
                }

                public decimal MarketPrice  // Рыночная цена ценной бумаги
                {
                    get
                    {
                        decimal value = 0;
                        if (Count != 0) value = MarketSumma / Count;
                        return value;
                    }
                }


                public decimal MarketSumma  // Рыночная стоимость ценной бумаги
                {
                    get
                    {
                        decimal value = Summa;
                        if (expectedYield != null)
                            value = value + expectedYield.value;
                        return value;
                    }
                }


                public decimal ValuteCurs   // Курс валюты
                {
                    get
                    {
                        return Currencies.GetValuteCursByTicker(currency);
                    }
                }


                public decimal RubPrice // Рыночная цена ценной бумаги в рублях
                {
                    get
                    {

                        decimal value = 0;
                        if (Count != 0) value = decimal.Round(RubSumma / Count,2);
                        return value;
                    }
                }

                public decimal RubSumma // Рыночная стоимость ценной бумаги в рублях
                {
                    get
                    {
                        return decimal.Round(MarketSumma * ValuteCurs, 2);
                    }
                }
            }

            public class MoneyAmount
            {
                public string currency { get; set; }
                public decimal value { get; set; }
            }

        }

        public class CurrencyResponse
        {
            public string trackingId { get; set; }
            public CurrencyList payload { get; set; }
            public string status { get; set; }
        }

        public class CurrencyList
        {
            public List<Position> currencies { get; set; }
            public UserAccount account { get; set; }

            public class Position
            {
                public string currency { get; set; }
                public decimal balance { get; set; }
                public decimal blocked { get; set; }


                public string ticker    // Тиккер (для валюты - ее код)
                {
                    get
                    {
                        return currency;
                    }
                }

                public string instrumentType
                {
                    get
                    {
                        return "Currency";
                    }
                }

                public string InstrumentTypeName               // Тип ценной бумаги 
                {
                    get
                    {
                        string res = "";

                        res = currency;

                        if (MarketTypes != null)
                        {
                            string value;
                            if (MarketTypes.TryGetValue(ticker, out value) && (value != null) && (value != ""))
                                res = value;
                        }

                        return res;
                    }
                }

                public string name
                {
                    get
                    {
                        return Currencies.GetNameByTicker(currency);
                    }
                }




                public decimal Count    // Кол-во
                {
                    get
                    {
                        return decimal.Round(balance, 2);
                    }
                }

                public decimal Price    //Цена
                {
                    get
                    {
                        return 1;
                    }
                }


                public decimal Summa    // Сумма
                {
                    get
                    {
                        return decimal.Round(balance, 2);
                    }
                }

                public decimal MarketPrice  // Рыночная цена
                {
                    get
                    {
                        return 1;
                    }
                }

                public decimal MarketSumma  // Рыночная стоимость
                {
                    get
                    {
                        return decimal.Round(balance, 2);
                    }
                }

                public decimal ValuteCurs   // Курс валюты
                {
                    get
                    {
                        return Currencies.GetValuteCursByTicker(currency);
                    }
                }


                public decimal RubPrice // Рыночная цена в рублях
                {
                    get
                    {

                        decimal value = 0;
                        if (Count != 0) value = decimal.Round(RubSumma / Count, 2);
                        return value;
                    }
                }

                public decimal RubSumma // Рыночная стоимость в рублях
                {
                    get
                    {
                        return decimal.Round(MarketSumma * ValuteCurs, 2);
                    }
                }

            }
        }



    }
}
