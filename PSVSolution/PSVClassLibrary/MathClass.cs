using System;
using System.Numerics;


namespace PSVClassLibrary
{
    public class MathClass
    {

        // Факториал
        public static BigInteger Factorial (int number)
        {
            BigInteger result = 1;
            
            for (int i = number; i > 0; i--)
            {
                result *= i;
            }

            return result;
        }

        //Расчет углов треугольника
        public static void CalcTriangleAngles(double AB, double BC, double CA, out double A, out double B, out double C)
        {
            double cosA = (AB * AB + CA * CA - BC * BC) / (2 * AB * CA);
            double cosB = (AB * AB + BC * BC - CA * CA) / (2 * AB * BC);

            A = Math.Acos(cosA) * 180 / Math.PI;
            B = Math.Acos(cosB) * 180 / Math.PI;
            C = 180.0-(A+B);

        }


        // Расчет площади треугольника разными методами
        public static double CalcTriangleSquare(double AB, double BC, double AC)
        {
            // по формуле Герона
            double P = (AB + BC + AC)/2;
            double tmp = P * (P - AB) * (P - BC) * (P - AC);
            return Math.Sqrt(tmp);
        }

        public static double CalcTriangleSquare(double TriangleBase, double TriangleHeght)
        {
            // через основание и высоту
            return 0.5*TriangleBase*TriangleHeght;
        }

        public static double CalcTriangleSquare(double SideA, double SideB, double Alpha, bool isInDegree)
        {
            // через две стороны и угол
            if (isInDegree)
            {
                // угол в градусах
                return 0.5 * SideA * SideB * Math.Sin(Math.PI * Alpha / 180.0);
            }
            else
            {
                // угол в радианах
                return 0.5 * SideA * SideB * Math.Sin(Alpha);
            }

        }

        // Определение среднего
        public static double CalcAvg(params double[] values)
        {
            double sum = 0;
            if (values.Length == 0) 
                return sum;
            foreach (double val in values)
            {
                sum += val;
            };
            return (sum / values.Length);
        }

    }
}
