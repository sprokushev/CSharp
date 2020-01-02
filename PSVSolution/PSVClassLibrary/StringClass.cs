using System;
using System.Text;


namespace PSVClassLibrary
{

    /// <summary>Класс для ряботы со строками</summary>
    public class StringClass
    {
        /// <summary>Заменить в строке %USERNAME%, %TERMINAL% на имя пользователя и компьютера. Результирующая строка преобразуется в верхний регистр</summary>
        /// <param name="s">Исходная строка</param>
        /// <returns>Строка</returns>        
        public static string ReplaceSystemNames(string s) 
        {
            StringBuilder _sb = new StringBuilder();

            _sb.Replace("%USERNAME%", Environment.UserName.ToUpper());
            _sb.Replace("%TERMINAL%", Environment.MachineName.ToUpper());

            return _sb.ToString();
        }
    }
}
