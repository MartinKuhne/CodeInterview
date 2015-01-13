using System;

namespace InterviewPractice
{
    public static class FactorialExtension
    {
        /// <summary>
        /// A factorial is the product of all positive integers less than or equal to n
        /// i.e 5! = (1*) 2*3*4*5 = 6*20 = 120
        /// </summary>
        /// <param name="x"></param>
        /// <remarks>
        /// cf. http://en.wikipedia.org/wiki/Factorial
        /// </remarks>
        /// <returns></returns>
        public static uint Factorial(this uint x)
        {
            if (x < 1)
            {
                throw new NotImplementedException();
            }

            uint sum = 1;
            for (uint iter = 2; iter <= x; iter++)
            {
                sum = sum*iter;
            }
            return sum;
        }
    }
}