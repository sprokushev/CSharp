using System;
using System.Numerics;

namespace PSVClassLibrary
{
    public class PSVMath
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



    }
}
