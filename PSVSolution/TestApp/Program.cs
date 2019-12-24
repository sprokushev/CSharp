using System;
using PSVClassLibrary;


namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
           Console.WriteLine(RomanNumeric.TryParse("XCIX",out _));
           Console.WriteLine(PSVClassLibrary.PSVMath.Factorial(5));
        }
    }
}
