using System;
using System.Collections.Generic;
using System.Text;

namespace PSVClassLibrary
{

    public class SysUtils
    {
        // случайное целое число на базе GUID
        public static int RandomInt(int Low = 0, int High = int.MaxValue)
        {
            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
            int rnd = rndNum.Next(Low, High);
            return rnd;
        }

        // случайное число с плавающей точкой от 0 до 1 на базе GUID
        public static double RandomDouble()
        {
            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
            return rndNum.NextDouble();
        }

        // Случайное число на базе GUID в формате строки нужной длины, дополненная слева 0
        public static string RandomStr(int MaxLength)
        {
            string _str = RandomDouble().ToString();
            int _len = (_str.Length - 2 < MaxLength) ? (_str.Length - 2) : MaxLength;
            return _str.Substring(2, _len).PadLeft(MaxLength, '0');
        }
               
    }
}
