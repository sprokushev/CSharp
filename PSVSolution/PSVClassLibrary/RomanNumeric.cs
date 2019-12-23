using System;
using System.Collections.Generic;
using System.Text;

namespace PSVClassLibrary
{
    public class RomanNumeric
    {

        private static Dictionary<char, int> RomanDict = new Dictionary<char, int>()
        {
            {'I', 1 },
            {'V', 5 },
            {'X', 10 },
            {'L', 50 },
            {'C', 100 },
            {'D', 500 },
            {'M', 1000 }
        };

        private static bool CheckRomanDict(string pRomanStr)
        {
            for (int i = 0; i < pRomanStr.Length; i++)
            {
                if (!RomanDict.ContainsKey(pRomanStr[i])) return false;
            }
            return true;
        }


        public static int TryParse(string pRomanStr, out bool pIsRoman)
        {
            string vStr = pRomanStr.ToUpper();

            pIsRoman = CheckRomanDict(vStr);
            if (!pIsRoman) return 0;

            // перебираем сиволы в исходной строке и заменяем римские цифры на арабские
            // собираем в список чисел
            int res = 0;
            for (int i = 0; i < pRomanStr.Length; i++)
            {
                if ((i + 1 < pRomanStr.Length) && (RomanDict[vStr[i]] < RomanDict[vStr[i + 1]]))
                {
                    res -= RomanDict[vStr[i]];
                }
                else
                {
                    res += RomanDict[vStr[i]];
                }
            }

            return res;
        }

    }
}
