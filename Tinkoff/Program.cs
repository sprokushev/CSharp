using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using Xunit;
using System.Linq;
using System.IO;
using CLAP;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace PSVClassLibrary
{
    enum ResultTypes { List, Sum }

    static class CryptoClass
    {
        // Create byte array for additional entropy when using Protect method.
        static byte[] s_aditionalEntropy = { 9, 2, 3, 7, 5 };


        public static byte[] encrypt_function(string secret)
        {
            System.Text.UTF8Encoding Byte_Transform = new System.Text.UTF8Encoding();

            //Just grabbing the bytes since most crypto functions need bytes.
            byte[] bytes = Byte_Transform.GetBytes(secret);

            //Encrypt the data.
            return ProtectedData.Protect(bytes, s_aditionalEntropy, DataProtectionScope.CurrentUser); 
        }


        public static string decrypt_function(byte[] crypted_bytes)
        {
            byte[] bytes = ProtectedData.Unprotect(crypted_bytes, s_aditionalEntropy, DataProtectionScope.CurrentUser);

            System.Text.UTF8Encoding Byte_Transform = new System.Text.UTF8Encoding();

            return Byte_Transform.GetString(bytes);
        }

    }

    class ArgApp
    {
        public string token { get; set; }
        //public Currency currency { get; private set; }
        public ResultTypes result { get; private set; }

        public ArgApp()
        {
            token = "";
            //currency = Tinkoff.Trading.OpenApi.Models.Currency.Rub;
            result = ResultTypes.Sum;
        }

        [Verb(IsDefault = true)]
        public void Help(string help)
        {
            if ((help == null) || (help == "")) return;

            string exe_name = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine($"{exe_name} - программа для считывания информации пользователя на брокерском счете в Тинкофф-инвестиции.{Environment.NewLine}{Environment.NewLine}Возвращаемое программой значение - общая сумма ценных бумаг и наличной валюты на брокерском счете, номинированных в указанной валюте.");

            Console.WriteLine(help);
        }

        public void WriteErrorToLog(string logfile, string errormessage)
        {
            
            File.AppendAllText(logfile, Environment.NewLine+DateTime.Now+ Environment.NewLine+errormessage,System.Text.Encoding.GetEncoding(1251));
        }

        [Global(Description = "текстовый файл с токеном авторизации в первой строке. Токен после считывания шифруется и сохраняется в шифрованном виде, затем программа завершает работу. Достаточно однократно считать токен, при всех последующих запусках зашифрованный токен используется для авторизации пользователя в банковском API", Aliases="t")]
        public void Token(string value)
        {
            string[] data;

            if (File.Exists(value))
            {
                data = File.ReadAllLines(value);
                if ((data == null) || (data.Length==0) || (data[0] == "")) 
                    throw new ArgumentException($"Файл {value} пустой или не содержит токен!");
                token = data[0];

                // Шифруем токен
                byte[] Encrypted_Bytes = CryptoClass.encrypt_function(token);

                // Записывем токен в реестр
                Registry.SetValue(Program.keyName, "Token", Encrypted_Bytes);

                Console.WriteLine($"Токен зашифрован и сохранен для последующих запусков. Файл {value} можно удалить! Запустите повторно программу без ключей t или token."); ;
            }
            else throw new FileNotFoundException($"Файл {value} не существует!");
        }

 /*       [Global(Description = "запросить активы, номинированные в указанной валюте. Возможные значения: rub, usd или eur. По умолчанию = rub", Aliases = "c")]
        public void Currency(string value)
        {
            value = value.ToLower();
            switch (value)
            {
                case "usd":
                    currency = Tinkoff.Trading.OpenApi.Models.Currency.Usd;
                    break;
                case "eur":
                    currency = Tinkoff.Trading.OpenApi.Models.Currency.Eur;
                    break;
                case "rub":
                default:
                    currency = Tinkoff.Trading.OpenApi.Models.Currency.Rub;
                    break;
            }
        }*/

        [Global(Description = "list - вывести на экран список активов со стоимостью в указанной валюте; sum - вывести на экран общую сумму активов, номинированных в указанной валюте. По умолчанию = sum", Aliases = "r")]
        public void Result(string value)
        {
            value = value.ToLower();
            switch (value)
            {
                case "list":
                    result = ResultTypes.List;
                    break;
                case "sum":
                default:
                    result = ResultTypes.Sum;
                    break;
            }
        }
    }


    class Program
    {

        public const string keyName = "HKEY_CURRENT_USER\\Software\\PSV";
        public const string delim = "\t";

        private static decimal Summa = 0;

        static int Main(string[] args) 
        {
            // парсим параметры командной строки
            var argApp = new ArgApp();
            var argParser = new Parser<ArgApp>();
            argParser.Register.HelpHandler("?,h,help", help => argApp.Help(help));
            argParser.Register.EmptyHandler(() => { });

            try
            {
                argParser.Run(args, argApp);

                if (argApp.token != "")
                {
                    // записан новый токен, завершаем работу программы
                    return -1;
                }
                else
                {
                    // token не передан в параметрах, считаем его последнее значение из реестра
                    byte[] bytes = (byte[])Registry.GetValue(keyName, "Token", null);

                    // Дешифруем токен
                    if (bytes != null) argApp.token = CryptoClass.decrypt_function(bytes);
                    else
                    {
                        throw new ArgumentException($"Не указан токен!");
                    }

                }
            }
            catch (Exception ex)
            {
                string logfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".log";

                string error = ex.Message;
                if (ex.InnerException != null) error = ex.InnerException.Message;

                argApp.WriteErrorToLog(logfile, ex.ToString());
                Console.WriteLine(error);

                argApp.Help(argParser.GetHelpString());

                return -1;
            }


            // запрашиваем информацию из банка
            try
            {
                DoWorkAsync(argApp).Wait();
            }
            catch (Exception ex)
            {
                string logfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".log";

                string error = ex.Message;
                if (ex.InnerException != null) error = ex.InnerException.Message;

                argApp.WriteErrorToLog(logfile, ex.ToString());
                Console.WriteLine(error);

                return -1;
            }


            return decimal.ToInt32(Summa);
        }

        static async Task DoWorkAsync(ArgApp argApp)
        {
            var connection = ConnectionFactory.GetConnection(argApp.token);
            var context = connection.Context;
            var portfolio = await context.PortfolioAsync();
            var portfolioCurrency = await context.PortfolioCurrenciesAsync();

            // определяю курсы доллара и евро - пока заглушка, делаю через остатки наличных
            decimal usd = 0;
            decimal eur = 0;

            var pos_usd = from p in portfolio.Positions where (p.InstrumentType==InstrumentType.Currency) select p;
            foreach (var pos in pos_usd)
            {
                if (pos.Ticker == "USD000UTSTOM")
                {
                    usd = (pos.Balance * pos.AveragePositionPrice.Value + pos.ExpectedYield.Value) / pos.Balance;
                    //Console.WriteLine($"USD={usd}");
                }
            }

            // перебираю ценные бумаги
            decimal count = 0;
            decimal price = 0;
            decimal value = 0;
            decimal original_price = 0;
            decimal original_value = 0;
            var pos_papers = from p in portfolio.Positions select p;
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
            var pos_currencies = from p in portfolioCurrency.Currencies where (p.Currency == Currency.Rub) select p;
            foreach (var pos in pos_currencies)
            {
                value = decimal.Round(pos.Balance,2);
                if (argApp.result == ResultTypes.List)
                    Console.WriteLine($"{"Рубли",-20}{delim}{"Currency",-10}{delim}{value,-10}{delim}{1,-10}{delim}{value,-10}{delim}{pos.Currency}");
                Summa += value;
            }
            
            if (argApp.result == ResultTypes.Sum)
                Console.WriteLine(Summa);

        }
    }
}
