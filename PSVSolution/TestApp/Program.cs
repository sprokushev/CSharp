using System;
using PSVClassLibrary;


namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(RomanNumeric.TryParse("XCIX",out _));
            //Console.WriteLine(MathClass.Factorial(5));
            //Console.WriteLine(MathClass.CalcTriangleSquare(AB: 3, BC: 3, AC: 4));
            //Console.WriteLine(MathClass.CalcTriangleSquare(TriangleBase: 4, TriangleHeght: 2.236));
            //double[] numbers = { 1, 2, 3, 4, 5 };
            //Console.WriteLine(MathClass.CalcAvg(numbers));
            //double A;
            //double B;
            //double C;
            //MathClass.CalcTriangleAngles(AB: 3, BC: 3, CA: 4, out A, out B, out C);
            //Console.WriteLine($"{A}, {B}, {C}");
            //Console.WriteLine(MathClass.CalcTriangleSquare(SideA: 3, SideB: 4, Alpha: 48.1896851, isInDegree: true));
            MSOffice.OpenExcelWorkbook(@"D:\OneDrive\Documents\Семейная информация.xlsm", true, "", null);


        }
    }
}
