using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using CLAP;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace PSVClassLibrary
{
    partial class Program
    {
        public const string delim = "\t";
        private static decimal SummaTinkoff = 0;
        private static decimal CountTinkoff = 0;

        static int Main(string[] args) 
        {
            // парсим параметры командной строки
            var argApp = new ArgApp();
            var argParser = new Parser<ArgApp>();
            //argParser.Register.HelpHandler("?,h,help", help => argApp.Help(help));
            argParser.Register.EmptyHandler(() => { });

            int result = 0;

            try
            {
                argParser.Run(args, argApp);

                if (argApp.token != "")
                {
                    // записан новый токен, завершаем работу программы
                    result = -1;
                    goto ExitApp;
                }
                else
                {
                    // token не передан в параметрах, считаем его последнее значение из реестра
                    byte[] bytes = (byte[])Registry.GetValue(argApp.keyName, "Token", null);

                    // Дешифруем токен
                    if (bytes != null) argApp.token = CryptoClass.decrypt_function(bytes);
                    else
                    {
                        throw new ArgumentException($"Токен ранее не регистрировался!");
                    }

                }
            }
            catch (Exception ex)
            {

                string error = ex.Message;
                if (ex.InnerException != null) error = ex.InnerException.Message;

                string logfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".log";
                argApp.WriteErrorToLog(logfile, ex.ToString());

                Console.WriteLine(error);

                //argApp.Help(argParser.GetHelpString());

                result = -1;
                goto ExitApp;
            }

            // запрашиваем информацию из банка
            try
            {
                //DoWorkAsync(argApp).Wait();
                DoWorkAsync(argApp).GetAwaiter().GetResult();

                result = decimal.ToInt32(SummaTinkoff);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                if (ex.InnerException != null) error = ex.InnerException.Message;

                string logfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".log";
                argApp.WriteErrorToLog(logfile, ex.ToString());

                Console.WriteLine(error);

                result = -1;
                goto ExitApp;
            }

            
        ExitApp:
            //if ((argApp != null) && (argApp.ExcelBook != null)) argApp.ExcelBook.Close(argApp.SaveExcel);
            //if ((argApp != null) && (argApp.ExcelApp != null)) argApp.ExcelApp.Quit();
            if (argApp.ExcelApp != null) argApp.ExcelApp.Visible = true;

            return result;
        }


        static async Task DoWorkAsync(ArgApp argApp)
        {

            // получаем текущие курсы валют
            decimal _usd = 0;
            decimal _eur = 0;
            decimal _rub = 0;

            Currencies = await GetAsync<MarketInstrumentListResponse>(argApp.token, "https://api-invest.tinkoff.ru/openapi/market/currencies");
            if ((Currencies != null) && (Currencies.status == "Ok") && (Currencies.payload != null) && (Currencies.payload.instruments != null))
            {
                foreach (var item in Currencies.payload.instruments)
                {
                    // заполняем цену
                    var Price = await GetAsync<OrderbookResponse>(argApp.token, $"https://api-invest.tinkoff.ru/openapi/market/orderbook?figi={item.figi}&depth=1");
                    if ((Price != null) && (Price.status == "Ok") && (Price.payload != null))
                    {
                        item.changePrice = Price.payload.lastPrice - Price.payload.closePrice;
                        item.Price = 1;
                        item.Nominal = Price.payload.faceValue;

                        item.ticker = item.ticker.Substring(0, 3);
                        item.type = item.ticker;
                        item.valuteCurs = Price.payload.lastPrice;

                        switch (item.ticker)
                        {
                            case "USD": _usd = item.valuteCurs; break;
                            case "EUR": _eur = item.valuteCurs; break;
                            case "RUB": _rub = item.valuteCurs; break;
                        }
                    }

                }
            }

            // в крайнем случае - определяем курсы доллара и евро по данным центробанка
            if (Currencies == null) Currencies = new MarketInstrumentListResponse();
            if (Currencies.payload == null) Currencies.payload = new MarketInstrumentList();
            if (Currencies.payload.instruments == null) Currencies.payload.instruments = new List<MarketInstrumentList.Position>();

            if (_usd == 0)
            {
                string valuteCode = "USD";
                _usd = CBRF.GetValuteCurs(valuteCode);
                Currencies.payload.instruments.Add(new MarketInstrumentList.Position() 
                    { ticker = valuteCode, name = "Доллары США", currency = valuteCode, lot = 1, type = valuteCode, Price = 1, valuteCurs = _usd });
                Currencies.payload.total++;
                Currencies.status = "Ok";
            }
            if (_eur == 0)
            {
                string valuteCode = "EUR";
                _eur = CBRF.GetValuteCurs(valuteCode);
                Currencies.payload.instruments.Add(new MarketInstrumentList.Position()
                { ticker = valuteCode, name = "Евро", currency = valuteCode, lot = 1, type = valuteCode, Price = 1, valuteCurs = _eur });
                Currencies.payload.total++;
                Currencies.status = "Ok";
            }
            if (_rub == 0)
            {
                string valuteCode = "RUB";
                _rub = 1;
                Currencies.payload.instruments.Add(new MarketInstrumentList.Position()
                { ticker = valuteCode, name = "Рубль", currency = valuteCode, lot = 1, type = valuteCode, Price = 1, valuteCurs = _rub });
                Currencies.payload.total++;
                Currencies.status = "Ok";
            }

            // заполним в excel список доступных ценных бумаг
            if (argApp.ExcelListSheet != null)
            {
                decimal _count = 0;
                argApp.ExcelListSheet.Cells.ClearContents();

                // заголовок
                _count++;
                argApp.ExcelListSheet.Cells[_count, 1] = "Ticker";
                argApp.ExcelListSheet.Cells[_count, 2] = "Type";
                argApp.ExcelListSheet.Cells[_count, 3] = "Name";
                argApp.ExcelListSheet.Cells[_count, 4] = "Lot";
                argApp.ExcelListSheet.Cells[_count, 5] = "Currency";
                argApp.ExcelListSheet.Cells[_count, 6] = "valuteCurs";

                // валюты
                if ((Currencies != null) && (Currencies.status == "Ok") && (Currencies.payload != null) && (Currencies.payload.instruments != null))
                {
                    foreach (var item in Currencies.payload.instruments)
                    {
                        _count++;
                        argApp.ExcelListSheet.Cells[_count, 1] = item.ticker;
                        argApp.ExcelListSheet.Cells[_count, 2] = item.type;
                        argApp.ExcelListSheet.Cells[_count, 3] = item.name;
                        argApp.ExcelListSheet.Cells[_count, 4] = item.lot;
                        argApp.ExcelListSheet.Cells[_count, 5] = item.currency;
                        argApp.ExcelListSheet.Cells[_count, 6] = item.valuteCurs;
                    }
                }

                // акции
                var Markets = await GetAsync<MarketInstrumentListResponse>(argApp.token, "https://api-invest.tinkoff.ru/openapi/market/stocks");
                if ((Markets != null) && (Markets.status == "Ok") && (Markets.payload != null) && (Markets.payload.instruments != null))
                {
                    foreach (var item in Markets.payload.instruments)
                    {
                        // заполняем цену
                        /* var Price = await GetAsync<OrderbookResponse>(argApp.token, $"https://api-invest.tinkoff.ru/openapi/market/orderbook?figi={item.figi}&depth=1");
                        if ((Price != null) && (Price.status == "Ok") && (Price.payload != null))
                        {
                            item.changePrice = Price.payload.lastPrice-Price.payload.closePrice;
                            item.Price = Price.payload.lastPrice;
                            item.Nominal = Price.payload.faceValue;


                            item.valuteCurs = Currencies.payload.GetByTicker(item.currency).valuteCurs;
                        }
                        */
                        if (item.currency == "USD") item.type = "USD_USA";
                        if (item.currency == "RUB") item.type = "RUB_RUS";
                        if (item.currency == "EUR") item.type = "EUR_EU";

                        var value = argApp.MarketTypes[item.ticker];
                        if ((value != null) && (value != "")) item.type = value;

                        _count++;
                        Console.Write($"\r{_count}");
                        argApp.ExcelListSheet.Cells[_count, 1] = item.ticker;
                        argApp.ExcelListSheet.Cells[_count, 2] = item.type;
                        argApp.ExcelListSheet.Cells[_count, 3] = item.name;
                        argApp.ExcelListSheet.Cells[_count, 4] = item.lot;
                        argApp.ExcelListSheet.Cells[_count, 5] = item.currency;
                        argApp.ExcelListSheet.Cells[_count, 6] = Currencies.payload.valuteCursByTicker(item.currency);
                    }
                }

                // облигации
                var Bonds = await GetAsync<MarketInstrumentListResponse>(argApp.token, "https://api-invest.tinkoff.ru/openapi/market/bonds");
                if ((Bonds != null) && (Bonds.status == "Ok") && (Bonds.payload != null) && (Bonds.payload.instruments != null))
                {
                    foreach (var item in Bonds.payload.instruments)
                    {
                        // заполняем цену
                        /*  var Price = await GetAsync<OrderbookResponse>(argApp.token, $"https://api-invest.tinkoff.ru/openapi/market/orderbook?figi={item.figi}&depth=1");
                          if ((Price != null) && (Price.status == "Ok") && (Price.payload != null))
                          {
                              item.changePrice = Price.payload.lastPrice - Price.payload.closePrice;
                              item.Price = Price.payload.lastPrice;
                              item.Nominal = Price.payload.faceValue;

                              item.valuteCurs = Currencies.payload.GetByTicker(item.currency).valuteCurs;
                          }*/

                        if (item.currency == "USD") item.type = "USD_USA_BOND";
                        if (item.currency == "RUB") item.type = "RUB_RUS_BOND";
                        if (item.currency == "EUR") item.type = "EUR_EU_BOND";

                        if ((argApp.MarketTypes != null) && (argApp.MarketTypes[item.ticker] != "")) item.type = argApp.MarketTypes[item.ticker];

                        _count++;
                        Console.Write($"\r{_count}");
                        argApp.ExcelListSheet.Cells[_count, 1] = item.ticker;
                        argApp.ExcelListSheet.Cells[_count, 2] = item.type;
                        argApp.ExcelListSheet.Cells[_count, 3] = item.name;
                        argApp.ExcelListSheet.Cells[_count, 4] = item.lot;
                        argApp.ExcelListSheet.Cells[_count, 5] = item.currency;
                        argApp.ExcelListSheet.Cells[_count, 6] = Currencies.payload.valuteCursByTicker(item.currency);
                    }
                }

                // фонды
                var Etfs = await GetAsync<MarketInstrumentListResponse>(argApp.token, "https://api-invest.tinkoff.ru/openapi/market/etfs");
                if ((Etfs != null) && (Etfs.status == "Ok") && (Etfs.payload != null) && (Etfs.payload.instruments != null))
                {
                    foreach (var item in Etfs.payload.instruments)
                    {
                        // заполняем цену
                        /*var Price = await GetAsync<OrderbookResponse>(argApp.token, $"https://api-invest.tinkoff.ru/openapi/market/orderbook?figi={item.figi}&depth=1");
                        if ((Price != null) && (Price.status == "Ok") && (Price.payload != null))
                        {
                            item.closePrice = Price.payload.closePrice;
                            item.lastPrice = Price.payload.lastPrice;
                            item.faceValue = Price.payload.faceValue;

                            item.valuteCurs = 1;
                            if (item.currency == "USD") item.valuteCurs = usd;
                            if (item.currency == "EUR") item.valuteCurs = eur;
                            item.rubPrice = decimal.Round(item.lastPrice * item.valuteCurs, 2);
                        }*/

                        if (item.currency == "USD") item.type = "USD_USA_FOND";
                        if (item.currency == "RUB") item.type = "RUB_RUS_FOND";
                        if (item.currency == "EUR") item.type = "EUR_EU_FOND";

                        if ((argApp.MarketTypes != null) && (argApp.MarketTypes[item.ticker] != "")) item.type = argApp.MarketTypes[item.ticker];

                        _count++;
                        Console.Write($"\r{_count}");
                        argApp.ExcelListSheet.Cells[_count, 1] = item.ticker;
                        argApp.ExcelListSheet.Cells[_count, 2] = item.type;
                        argApp.ExcelListSheet.Cells[_count, 3] = item.name;
                        argApp.ExcelListSheet.Cells[_count, 4] = item.lot;
                        argApp.ExcelListSheet.Cells[_count, 5] = item.currency;
                        argApp.ExcelListSheet.Cells[_count, 6] = Currencies.payload.valuteCursByTicker(item.currency);
                    }
                }

                Console.Clear();
            }


            //  брокерские счета
            var BrokerAccounts = await GetAsync<AccountsResponse>(argApp.token, "https://api-invest.tinkoff.ru/openapi/user/accounts");

            Dictionary<string, PortfolioList> PaperAccounts = new Dictionary<string, PortfolioList>();
            Dictionary<string, CurrencyList> CurrencyAccounts = new Dictionary<string, CurrencyList>();

            if ((BrokerAccounts != null) && (BrokerAccounts.status == "Ok") && (BrokerAccounts.payload != null) && (BrokerAccounts.payload.accounts != null))
            {
                foreach (var item in BrokerAccounts.payload.accounts)
                {
                    // портфели счетов
                    var Portfolio = await GetAsync<PortfolioResponse>(argApp.token, $"https://api-invest.tinkoff.ru/openapi/portfolio?brokerAccountId={item.brokerAccountId}");

                    if ((Portfolio != null) && (Portfolio.status == "Ok") && (Portfolio.payload != null))
                    {
                        Portfolio.payload.account = new UserAccount() { brokerAccountId = item.brokerAccountId, brokerAccountType = item.brokerAccountType };
                        PaperAccounts.Add(item.brokerAccountId, Portfolio.payload);
                    }

                    // портфели валют
                    var Currency = await GetAsync<CurrencyResponse>(argApp.token, $"https://api-invest.tinkoff.ru/openapi/portfolio/currencies?brokerAccountId={item.brokerAccountId}");

                    if ((Currency != null) && (Currency.status == "Ok") && (Currency.payload != null))
                    {
                        Currency.payload.account = new UserAccount() { brokerAccountId = item.brokerAccountId, brokerAccountType = item.brokerAccountType };
                        CurrencyAccounts.Add(item.brokerAccountId, Currency.payload);
                    }
                }

            }
            else throw new JsonException("Ошибка при считывании списка брокерских счетов Тинькофф!");

            // перебираю ценные бумаги
            foreach (var account in PaperAccounts)
            {
                // перебираю ценные бумаги
                foreach (var pos in account.Value.positions)
                {
                    if (pos.instrumentType == "Currency") continue;
                    if (pos.averagePositionPrice == null) continue;

                    // выводим результат на экран
                    if (argApp.AppMode == AppModeTypes.List)
                    {
                        Console.WriteLine($"{pos.ticker,-20}{delim}{pos.instrumentType,-10}{delim}{pos.balance,-10}{delim}{pos.rubPrice,-10}{delim}{pos.rubSumma,-10}{delim}RUB{delim}{pos.infoCurs}");
                    }

                    // считаем общую сумму
                    SummaTinkoff += pos.rubSumma;
                    CountTinkoff += pos.balance;

                }
            }

            foreach (var account in CurrencyAccounts)
            {
                // перебираю наличные
                foreach (var pos in account.Value.currencies)
                {
                    if (argApp.AppMode == AppModeTypes.List)
                    {
                        Console.WriteLine($"{pos.currency,-20}{delim}{"Currency",-10}{delim}{pos.Summa,-10}{delim}{pos.valuteCurs,-10}{delim}{pos.rubSumma,-10}{delim}RUB{delim}{pos.infoCurs}");
                    }
                    SummaTinkoff += pos.rubSumma;
                }
            }

            if (argApp.AppMode == AppModeTypes.Sum)
            {
                Console.WriteLine(SummaTinkoff);
            }

        }

    } 
}
