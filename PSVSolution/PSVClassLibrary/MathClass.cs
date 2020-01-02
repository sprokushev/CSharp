using System;
using System.Numerics;


namespace PSVClassLibrary
{
    /// <summary>Класс математических методов.</summary>
    public class MathClass
    {
        /// <summary>Вычисляет факториал аргумента number, возвращает число типа BigInteger. Если number меньше 0 - возвращается BigInteger.Zero</summary>
        /// <param name="number">Число типа int</param>
        /// <returns>факториал числа number, тип BigInteger</returns>
        public static BigInteger Factorial (int number)
        {

            if (number < 0)
                return BigInteger.Zero;

            BigInteger _result = 1;

            for (int i = number; i > 0; i--)
            {
                _result *= i;
            }

            return _result;
        }

        /// <summary>Вычисляет углы треугольника</summary>
        /// <param name="AB">Сторона AB => 0</param>
        /// <param name="BC">Сторона BC => 0</param>
        /// <param name="CA">Сторона CA => 0</param>
        /// <param name="A">Угол BAC - возвращаемое значение. Если угол нельзя рассчитать - возвращается значение NaN</param>
        /// <param name="B">Угол ABC - возвращаемое значение. Если угол нельзя рассчитать - возвращается значение NaN</param>
        /// <param name="C">Угол BCA - возвращаемое значение. Если угол нельзя рассчитать - возвращается значение NaN</param>
        public static void CalcTriangleAngles(double AB, double BC, double CA, out double A, out double B, out double C)
        {
            if ((AB <= 0) || (BC <= 0) || (CA <= 0))
            {
                A = double.NaN;
                B = double.NaN;
                C = double.NaN;
            }
            else
            {
                double cosA = (AB * AB + CA * CA - BC * BC) / (2 * AB * CA);
                double cosB = (AB * AB + BC * BC - CA * CA) / (2 * AB * BC);

                A = Math.Acos(cosA) * 180 / Math.PI;
                B = Math.Acos(cosB) * 180 / Math.PI;
                C = 180.0 - (A + B);
            }
        }


        /// <summary>Расчет площади треугольника, возвращает число типа double. Если площадь рассчитать невозможно - возвращает NaN.</summary>
        /// <param name="AB">Сторона AB</param>
        /// <param name="BC">Сторона BC</param>
        /// <param name="CA">Сторона CA</param>
        /// <returns>Площадь треугольника типа double</returns>
        public static double CalcTriangleSquare(double AB, double BC, double CA)
        {
            if ((AB <= 0) || (BC <= 0) || (CA <= 0))
                return double.NaN;

            // по формуле Герона
            double P = (AB + BC + CA)/2;
            double tmp = P * (P - AB) * (P - BC) * (P - CA);
            return Math.Sqrt(tmp);
        }

        /// <summary>Расчет площади треугольника, возвращает double</summary>
        /// <param name="TriangleBase">Основание треугольника</param>
        /// <param name="TriangleHeght">Высота треугольника</param>
        /// <returns>Площадь треугольника, тип double</returns>
        public static double CalcTriangleSquare(double TriangleBase, double TriangleHeght)
        {
            if ((TriangleBase <= 0) || (TriangleHeght <= 0))
                return double.NaN;

            // через основание и высоту
            return 0.5 *TriangleBase*TriangleHeght;
        }

        /// <summary>Расчет площади треугольника, возвращает double. Если площадь рассчитать невозможно - возвращает NaN.</summary>
        /// <param name="AB">Сторона AB</param>
        /// <param name="CA">Сторона CA</param>
        /// <param name="Alpha">Угол между сторонами AB и CA</param>
        /// <param name="isInDegree">Если true - угол в градусах, если false - в радианах</param>
        /// <returns>Площадь треугольника, тип double</returns>
        public static double CalcTriangleSquare(double AB, double CA, double Alpha, bool isInDegree)
        {

            if ((AB <= 0) || (CA <= 0) || (Alpha <= 0))
                return double.NaN;

            // через две стороны и угол
            if (isInDegree)
            {
                // угол в градусах
                return 0.5 * AB * CA * Math.Sin(Math.PI * Alpha / 180.0);
            }
            else
            {
                // угол в радианах
                return 0.5 * AB * CA * Math.Sin(Alpha);
            }

        }

        // Определение среднего
        /// <summary>Расчет среднего значения чисел в массиве values, возвращает значение типа double. Если массив values пустой - возвращает NaN.</summary>
        /// <param name="values">Массив чисел для расчета среднего значения</param>
        /// <returns>Среднее значение чисел в массиве values.</returns>
        public static double CalcAvg(params double[] values)
        {
            double sum = 0;
            
            if (values.Length == double.NaN) 
                return sum;

            foreach (double val in values)
            {
                sum += val;
            };
            
            return (sum / values.Length);
        }

    }
}
