using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PSVClassLibrary
{
    /// <summary>Методы работы с массивами</summary>
    public class ArrayClass
    {
        /// <summary>Вычесть из массива a[] значения массива b[]</summary>
        /// <typeparam name="T">Тип элемента массива</typeparam>
        /// <param name="a">Массив a[] - из него вычистается массив b[]</param>
        /// <param name="b">Массив b[] - он вычитаетсяиз массива a[]</param>
        /// <returns>Массив значений - результат вычитания</returns>
        public static T[] ArrayDiff<T>(T[] a, T[] b)
        {

            // Мой вариант через List<T>
            /*
            List<T> res = new List<T>(a);
            foreach (var val_b in b)
            {
                res.RemoveAll(x => (x.Equals(val_b)));
            }

            return res.ToArray();
            */

            // Мой вариант через LINQ - но в этом случае удалаютя дубликаты из массива a[]!!!
            //return a.Except(b).ToArray();


            // Best Practicies через LINQ - дубликаты в массиве a[] сохраняются
            return a.Where(n => !b.Contains(n)).ToArray();

        }


    }
}
