using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;


namespace PSVClassLibrary
{
	/// <summary>
	/// Класс предназначен для преобразований арабских чисел в римские и обратно
	/// <para>Класс изначально содержит алфавит римских чисел, способных определять арабские числа от 1 до 39999.
	/// Если необходимо расширить диапазон, то можно определить дополнительные обозначения для римских чисел, используя
	/// поле <see cref="BaseRomanNumbers"/> </para>
	/// </summary>
	public static class RomanNumeric
	{
		/// <summary>
		/// Алфавит базовых римских чисел
		/// <para>Алфавит построен в виде словаря. Ключом словаря является арабское число (int), значением - соответствующее ему
		/// римское число (string)</para>
		/// <para>Содержит римское обозначения арабских чисел 1*,4*,5*,9* - где "*"представляет собой 0...N нулей.
		/// При создании содержит в себе обозначение чисел от 1 до 10000 (I...ↂ). Так как в римском числе один символ не может
		/// встречаться более трех раз, то изначально можно преобразовать в римский формат числа от 1 до 39999. 
		/// Если Вы хотите иметь возможность работать с большим количеством римских чисел, то вы должны добавить в список 
		/// дополнительные обозначения начиная с 40000 не пропуская элементы 1*,4*,5*,9*.</para>
		/// </summary>
		public static SortedList<int, string> BaseRomanNumbers { get; set; }

		static RomanNumeric()
		{
			BaseRomanNumbers = new SortedList<int, string>(17);
			BaseRomanNumbers.Add(1, "I");
			BaseRomanNumbers.Add(4, "IV");
			BaseRomanNumbers.Add(5, "V");
			BaseRomanNumbers.Add(9, "IX");
			BaseRomanNumbers.Add(10, "X");
			BaseRomanNumbers.Add(40, "XL");
			BaseRomanNumbers.Add(50, "L");
			BaseRomanNumbers.Add(90, "XC");
			BaseRomanNumbers.Add(100, "C");
			BaseRomanNumbers.Add(400, "CD");
			BaseRomanNumbers.Add(500, "D");
			BaseRomanNumbers.Add(900, "CM");
			BaseRomanNumbers.Add(1000, "M");
			BaseRomanNumbers.Add(4000, "Mↁ");
			BaseRomanNumbers.Add(5000, "ↁ");
			BaseRomanNumbers.Add(9000, "Mↂ");
			BaseRomanNumbers.Add(10000, "ↂ");
		}

		/// <summary>
		/// Рассчитывает максимально возможное римское число для текущего алфавита римских чисел.
		/// </summary>
		public static uint MaxRomanNumber()
		{
			int _lastNumber = BaseRomanNumbers.Keys.Last();
			int _nonzero = int.Parse(_lastNumber.ToString().Replace('0', '\0'));
			int _res = 0;

			switch (_nonzero)
			{
				case 1:
					_res = _lastNumber * 4 - 1;
					break;
				case 4:
				case 9:
					_res = _lastNumber;
					break;
				case 5:
					_res = _lastNumber + _lastNumber / 5 * 3;
					break;
				default:
					break;
			}

			return uint.Parse(_res.ToString().Replace('0', '9')); ;
		}


		/// <summary>
		/// Конвентирует целое число в римское число
		/// </summary>
		/// <param name="ArabicNumber">Арабское число, которое необходимо преобразовать в римскую запись</param>
		/// <exception cref="ArgumentOutOfRangeException">Генерируется когда в качестве параметра передано число равное "0" 
		/// или число большее чем максимальная римское число.</exception>
		/// <returns>Строку, представляющую собой римской число</returns>
		public static string ArabicToRoman(this int ArabicNumber)
		{
			StringBuilder RomanNumber = new StringBuilder();

			//Исключаем знак "-" из арабского числа и делаем его первым символом римского числа
			if (ArabicNumber < 0)
			{
				RomanNumber.Append("-");
				ArabicNumber = -ArabicNumber;
			}

			if (ArabicNumber == 0)
				throw new ArgumentOutOfRangeException("ArabicNumber", ArabicNumber,
					"Недопустимое значение аргумента: римские числа не могут быть равными\"0\"");
			else if (ArabicNumber > MaxRomanNumber())
				throw new ArgumentOutOfRangeException("ArabicNumber", ArabicNumber,
					string.Format("Недопустимое значение аргумента: невозможно задать римское число большее чем {0}",
						MaxRomanNumber()));

			//Раскладываем арабское число на составляющие его римские числа и объединяем их в одну строку
			var neededBaseRomanNubers =
				from к in BaseRomanNumbers.Keys
				where к <= ArabicNumber
				orderby к descending
				select к;

			foreach (int val in neededBaseRomanNubers)
			{
				while ((ArabicNumber / val) >= 1)
				{
					ArabicNumber -= val;
					RomanNumber.Append(BaseRomanNumbers[val]);
				}
			}

			return RomanNumber.ToString();
		}

		/// <summary>
		/// Конвентирует римское число в арабское
		/// </summary>
		/// <param name="RomanNumber">Римское число, которое необходимо преобразовать в тип int</param>
		/// <exception cref="FormatException">Генерируется когда в качестве параметра передано число не являющееся римским</exception>
		/// <returns>Целое число, представляющее собой арабскую запись римского числа</returns>
		public static int RomanToArabic(this string RomanNumber)
		{
			int ArabicNumber = 0;
			sbyte negativ = 1;
			string Roman = RomanNumber.Trim();

			if (Roman[0] == '-')
			{
				negativ = -1;
				Roman = Roman.Substring(1);
			}

			StringBuilder shablonRomanNumber = new StringBuilder();

			foreach (int _key in BaseRomanNumbers.Keys)
			{
				int _index = BaseRomanNumbers.Keys.IndexOf(_key);
				string _quant = "?";
				if (_index == 0 || (_index % 4) == 0)
					_quant = "{0,3}";

				shablonRomanNumber.Insert(0, string.Format("(?<{0}>({1}){2})?", _key.ToString(),
					BaseRomanNumbers[_key], _quant));
			}

			//Игнорировать регистр + соответствие должно начинаться с начала строки
			shablonRomanNumber.Insert(0, "(?i)^");
			//Соответствие должно обнаруживаться в конце строки
			shablonRomanNumber.Append("$");

			//Упрощенная проверка. Не проверяет таких ошибок как IVII
			if (!Regex.IsMatch(Roman, shablonRomanNumber.ToString()))
				throw new FormatException(string.Format("Текст \"{0}\" не является римским числом", RomanNumber));

			Match число = Regex.Match(Roman, shablonRomanNumber.ToString());


			foreach (int val in BaseRomanNumbers.Keys)
			{
				ArabicNumber += число.Groups[val.ToString()].Length / BaseRomanNumbers[val].Length * val;
			}

			return ArabicNumber * negativ;
		}
	}









	class RomanNumeric2
	{

		private static Dictionary<char, int> RomanDict = new Dictionary<char, int>()
		{
			{'I', 1 },
			{'V', 5 },
			{'X', 10 },
			{'L', 50 },
			{'C', 100 },
			{'D', 500 },
			{'M', 1000 },

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
