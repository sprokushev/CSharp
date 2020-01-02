using System;
using System.Collections.Generic;
using System.Text;

namespace PSVClassLibrary
{

    /// <summary>Случайное число на базе GUID с возможность вывода в формате строки определенной длинны, дополненной слева 0. </summary>
    public class RandomGUID : Random
    {
        /// <summary>  Размер строки по умолчанию, возвращаемый <see cref="ToString()"/></summary>
        public const int DefaultMaxLengthRandomStr = 8;

        /// <summary>  Конструктор класса <see cref="RandomGUID"/>.
        /// По умолчанию значение seed value = int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber)</summary>
        public RandomGUID() : base(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber))
        {
        }

        /// <summary>Случайное число на базе GUID в формате строки длинной по умолчанию <see cref="DefaultMaxLengthRandomStr"/>, дополненной слева 0</summary>
        /// <returns>Строка</returns>        
        public override string ToString() => this.ToString(DefaultMaxLengthRandomStr);

        /// <summary>Случайное число на базе GUID в формате строки длинной MaxLength, дополненной слева 0</summary>
        /// <param name="MaxLength">Длина строки</param>
        /// <returns>Строка</returns>
        public string ToString(int MaxLength)
        {
            string _str = NextDouble().ToString();
            return FormatString(_str, MaxLength);
        }

        /// <summary>Отформатировать вывод методов Next(), NextDouble(), NextByte(), Sample() в соответствии с правилами класса</summary>
        /// <param name="_str">Форматируемая строка</param>
        /// <param name="MaxLength">Длина строки</param>
        /// <returns>Строка</returns>
        /// <example>FormatString(Next().ToString(),12)
        /// <code></code></example>
        public string FormatString(string _str, int MaxLength)
        {
            char[] decimals = { '.', ',' };
            int _dec = _str.IndexOfAny(decimals);
            if (_dec != -1)
            {
                _str = _str.Substring(_dec + 1);
            }

            int _len = (_str.Length < MaxLength) ? _str.Length : MaxLength;
            return _str.Substring(0, _len).PadLeft(MaxLength, '0');
        }
    }
}
