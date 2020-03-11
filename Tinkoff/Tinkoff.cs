using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PSVClassLibrary
{

    partial class Program
    {
        // курсы валют
        public static MarketInstrumentListResponse Currencies;


        static async Task<T> GetAsync<T>(string Token, string Uri)
        {
            // инициализация HTTP
            var cl = new HttpClient();
            cl.BaseAddress = new Uri(Uri);

            int _TimeoutSec = 90;
            cl.Timeout = new TimeSpan(0, 0, _TimeoutSec);

            string _ContentType = "application/json";
            cl.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));

            var _CredentialBase64 = Token;
            cl.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", _CredentialBase64));


            // GET-запрос
            T result = default(T);

            HttpResponseMessage response = await cl.GetAsync(Uri);

            if (response.IsSuccessStatusCode)
            {
                var jsonAsString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<T>(jsonAsString);
            }
            return result;
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

                public decimal valuteCurs
                {
                    get
                    {
                        if (averagePositionPrice != null) 
                            return Currencies.payload.valuteCursByTicker(averagePositionPrice.currency);
                        else 
                            return 1;
                    }
                }

                public string infoCurs
                {
                    get
                    {
                        string value = "";
                        if ((averagePositionPrice != null) && (averagePositionPrice.currency != "RUB")) 
                            value = $"{Summa} {averagePositionPrice.currency}, {averagePositionPrice.currency}={decimal.Round(valuteCurs, 4)} RUB";
                        return value;
                    }
                }

                public decimal Price
                {
                    get
                    {
                        decimal value = 0;
                        if (balance != 0) value = Summa / balance;
                        return value;
                    }
                }

                public decimal Summa
                {
                    get
                    {
                        decimal value = 0;
                        if (averagePositionPrice != null) 
                            value = balance * averagePositionPrice.value;
                        if (expectedYield != null) 
                            value = value + expectedYield.value;
                        return value;
                    }
                }

                public decimal rubPrice
                {
                    get
                    {
                        decimal value = 0;
                        if ((averagePositionPrice != null) && (averagePositionPrice.currency == "RUB")) 
                            value = Price;
                        else
                            if (balance != 0) 
                            value = decimal.Round(Summa * valuteCurs / balance, 2);
                        else 
                            value = 0;
                        return value;
                    }
                }

                public decimal rubSumma
                {
                    get
                    {
                        decimal value = 0;
                        if ((averagePositionPrice != null) && (averagePositionPrice.currency == "RUB")) 
                            value = Summa;
                        else 
                            value = decimal.Round(Summa * valuteCurs, 2);
                        return value;
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

                public decimal valuteCurs
                {
                    get
                    {
                        return Currencies.payload.valuteCursByTicker(currency);
                    }
                }

                public string infoCurs
                {
                    get
                    {
                        string value = "";
                        if (currency != "RUB")
                            value = $"{Summa} {currency}, {currency}={decimal.Round(valuteCurs, 4)} RUB";
                        return value;
                    }
                }

                public decimal Summa
                {
                    get
                    {
                        return decimal.Round(balance, 2);
                    }
                }

                public decimal rubSumma
                {
                    get
                    {
                        decimal value = 0;
                        if (currency == "RUB")
                            value = Summa;
                        else
                            value = decimal.Round(Summa * valuteCurs, 2);
                        return value;
                    }
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
            public decimal faceValue { get; set; }
            public decimal lastPrice { get; set; }
            public decimal closePrice { get; set; }
            public decimal limitUp { get; set; }
            public decimal limitDown { get; set; }

            public class OrderResponse
            {
                public decimal price { get; set; }
                public int quantity { get; set; }
            }
        }


        public class MarketInstrumentListResponse
        {
            public string trackingId { get; set; }
            public MarketInstrumentList payload { get; set; }
            public string status { get; set; }
        }

        public class MarketInstrumentList
        {
            public List<Position> instruments { get; set; }
            public int total { get; set; }

            public decimal valuteCursByTicker(string ticker)
            {
                if (instruments != null)
                    foreach (var item in instruments)
                    {
                        if (item.ticker == ticker) 
                            return item.valuteCurs;
                    }
                return 1;
            }

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

                public decimal Nominal { get; set; } // номинал облигации
                public decimal changePrice { get; set; } // изменение в течение текущей торговой сессии
                public decimal Price { get; set; } // текущая цена
                public decimal valuteCurs { get; set; } // курс валюты
                public decimal rubPrice { get { return decimal.Round(Price * valuteCurs, 2); } } // цена в рублях
            }
        }

    }


    /*       static async Task DoWorkAsync(ArgApp argApp)
       {
           var connection = ConnectionFactory.GetConnection(argApp.token);
           var context = connection.Context;
           var portfolio = await context.PortfolioAsync();
           var portfolioCurrency = await context.PortfolioCurrenciesAsync();

           // определяю курсы доллара и евро - пока заглушка, делаю через остатки наличных
           decimal usd = 0;
           decimal eur = 0;

           //var pos_usd = from p in portfolio.Positions where (p.InstrumentType==InstrumentType.Currency) select p;
           //foreach (var pos in pos_usd)
           //{
           //    if (pos.Ticker == "USD000UTSTOM")
           //    {
           //        usd = (pos.Balance * pos.AveragePositionPrice.Value + pos.ExpectedYield.Value) / pos.Balance;
           //        //Console.WriteLine($"USD={usd}");
           //    }
           }

           string url = "http://www.cbr.ru/scripts/XML_daily.asp";
           XmlReader reader = XmlReader.Create(url);

           var valutes = XElement.Load(reader).Descendants("Valute");

           foreach (var item in (from v in valutes where v.Element("CharCode").Value == "USD" select v.Element("Value").Value))
           {
              decimal.TryParse(item, out usd);
           }

           foreach (var item in (from v in valutes where v.Element("CharCode").Value == "EUR" select v.Element("Value").Value))
           {
               decimal.TryParse(item, out eur);
           }

           // перебираю ценные бумаги
           decimal count = 0;
           decimal price = 0;
           decimal value = 0;
           decimal original_price = 0;
           decimal original_value = 0;
           var pos_papers = from p in portfolio.Positions where (p.InstrumentType != InstrumentType.Currency) select p;
           foreach (var pos in pos_papers)
           {
               // текущая стоимость всех бумаг в портфеле (в валюте)
               value = pos.Balance*pos.AveragePositionPrice.Value + pos.ExpectedYield.Value;
               original_price = value / pos.Balance;
               original_value = value;
               // переводим валюту в рубли
               string curs_info = "";
               if (pos.AveragePositionPrice.Currency == Currency.Usd) 
                   { value *= usd; curs_info = $", USD={Math.Round(usd,4)} руб."; }
               if (pos.AveragePositionPrice.Currency == Currency.Eur) 
                   { value *= eur; curs_info = $", EUR={Math.Round(eur, 4)} руб."; }
               // пересчитываем 
               value = decimal.Round(value,2);
               price = decimal.Round(value / pos.Balance,2);

               if (argApp.result == ResultTypes.List)
               {
                   Console.Write($"{pos.Ticker,-20}{delim}{pos.InstrumentType,-10}{delim}{pos.Balance,-10}{delim}{price,-10}{delim}{value,-10}{delim}{Currency.Rub}");

                   if (pos.AveragePositionPrice.Currency != Currency.Rub)
                   {
                       Console.Write($"{delim}{original_value} {pos.AveragePositionPrice.Currency}{curs_info}");
                   }
                   else Console.Write($"{delim}");
                   Console.WriteLine();
               }
               Summa += value;
               count += pos.Balance;
           }

           // остатки рублей
           var pos_currencies = from p in portfolioCurrency.Currencies select p;
           foreach (var pos in pos_currencies)
           {
               value = decimal.Round(pos.Balance,2);
               if (argApp.result == ResultTypes.List)
               {
                   switch (pos.Currency)
                   {
                       case Currency.Rub:
                           Console.WriteLine($"{"Наличные",-20}{delim}{"Currency",-10}{delim}{value,-10}{delim}{1,-10}{delim}{value,-10}{delim}{Currency.Rub}");
                           Summa += value;
                           break;
                       case Currency.Usd:
                           Console.WriteLine($"{"Наличные",-20}{delim}{"Currency",-10}{delim}{value,-10}{delim}{usd,-10}{delim}{decimal.Round(usd*value, 2),-10}{delim}{Currency.Rub}{delim}{value} {pos.Currency}, USD={Math.Round(usd, 4)} руб.");
                           Summa += usd*value;
                           break;
                       case Currency.Eur:
                           Console.WriteLine($"{"Наличные",-20}{delim}{"Currency",-10}{delim}{value,-10}{delim}{eur,-10}{delim}{decimal.Round(eur * value, 2),-10}{delim}{Currency.Rub}{delim}{value} {pos.Currency}, EUR={Math.Round(eur, 4)} руб.");
                           Summa += eur * value;
                           break;
                       default:
                           break;
                   }
               }
           }

           if (argApp.result == ResultTypes.Sum)
               Console.WriteLine(Summa);

       } */


}
