using System;
using System.Text;
using PSVClassLibrary;


namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            do
            {
                TicTacToeGame game = new TicTacToeGameConsole(3, TicTacToeGame.CellValues.Cross, 3, TicTacToeGame.PlayerTypes.Machine, TicTacToeGame.PlayerTypes.Human);
                game.Start();
                Console.WriteLine("Сыграем еще (y/n)?");
            } while (Console.ReadLine().ToUpper()=="Y");

            Console.ReadLine();

            Console.WriteLine(Test("2 4 7 8 10"));
            Console.WriteLine(Test("1 2 1 1"));
            Console.ReadLine();
            
            var array = ArrayClass.ArrayDiff<int>(new int[] { 1, 2, 2, 2, 3 }, new int[] { 1, 3 });
            foreach (var ii in array)
            {
                Console.WriteLine(ii);
            }

            Console.ReadLine();

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


        public static string AddBinary(int a, int b)
        {
            return Convert.ToString(a + b, 2);
        }


        public static int Test(string numbers)
        {
            string[] str_array = numbers.Split(' ');
            int num=0;
            int odd_num = 0;
            int odd_count = 0;
            int even_num = 0;
            int even_count = 0;



            for (int i=str_array.Length-1; i>=0; i--)
            {
                int.TryParse(str_array[i], out num);
                if (num%2==0)
                {
                    even_num = i+1;
                    even_count++;
                }
                else
                {
                    odd_num = i+1;
                    odd_count++;
                }
            }

            if (even_count > odd_count)
            {
                return odd_num;
             }
            else
            {
                return even_num;
            }
        }
    }
}
