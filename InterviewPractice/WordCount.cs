using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewPractice
{
    public class WordCount
    {
        /// <summary>
        /// One Problem per function
        /// Design for extensibility
        /// 
        /// Helper function to determine if a chacater is whitespace
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool isWhiteSpace(this char input)
        {
            return (input == ' ');
        }

        public static uint WordCount(char[] input)
        {
            var len = input.Length;
            uint count = 0;
            bool scanningWord = false;
            foreach (char iter in input)
            {
                if (isWhiteSpace(iter))
                {
                    if (scanningWord == true)
                    {
                        count++;
                        scanningWord = false;
                    }
                }
                else
                {
                    if (scanningWord == false)
                    {
                        scanningWord = true;
                    }
                }
            }

            // account for encountering the end of the string
            if (scanningWord)
            {
                count++;
            }

            return count;
        }
    }
}
