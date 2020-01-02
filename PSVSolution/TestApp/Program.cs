using System;
using PSVClassLibrary;


namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var r = new RandomGUID();
            var rr = new RandomGUID();

            
            var i = r.Next();
            Console.WriteLine(i);
            Console.WriteLine(r.FormatString(i.ToString(),12));

            var d = rr.NextDouble();
            Console.WriteLine(d);
            Console.WriteLine(r.FormatString(d.ToString(), 12));

            Console.WriteLine(r.ToString(8));
            Console.WriteLine(rr.ToString(8));
            Console.ReadLine();

            double[] numbers = {};
            Console.WriteLine(MathClass.CalcAvg(numbers));

            Console.WriteLine(MathClass.Factorial(7));
            Console.WriteLine(MathClass.Factorial(-7));
            MathClass.CalcTriangleAngles(AB: 0, BC: 3, CA: 4, out double A, out double B, out double C);
            Console.WriteLine($"{A}, {B}, {C}");


            Console.WriteLine(MathClass.CalcTriangleSquare(AB: 3, BC: 3, CA: 40));
            Console.WriteLine(RomanNumeric.RomanToArabic("CXCIX"));
            Console.WriteLine(RomanNumeric.ArabicToRoman(199));
            Console.WriteLine(MathClass.CalcTriangleSquare(TriangleBase: 4, TriangleHeght: 2.236));

            Console.WriteLine(MathClass.CalcTriangleSquare(AB: 3, CA: 4, Alpha: 48.1896851, isInDegree: true));
            MSOffice.OpenExcelWorkbook(@"D:\OneDrive\Documents\Семейная информация.xlsm");

            Console.WriteLine(StringClass.ReplaceSystemNames("asasfsaff %USERNAME%, fasdfsdfdsf %TERMINAL%.%USERNAME%"));
       
            
            

        }
    }
}
