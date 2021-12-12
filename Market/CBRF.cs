// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PSVClassLibrary
{
    // получить валютные курсы с сайта ЦБ РФ
    public static class CBRF
    {

        public static decimal GetValuteCurs(string ValuteCode)
        {
            string url = "http://www.cbr.ru/scripts/XML_daily.asp";
            decimal result = 0;

            XmlReader reader = XmlReader.Create(url);
            var valutes = XElement.Load(reader).Descendants("Valute");

            foreach (var item in (from v in valutes where v.Element("CharCode").Value == ValuteCode select v.Element("Value").Value))
            {
                decimal.TryParse(item, out result);
            }

            return result;
        }

    }
}
