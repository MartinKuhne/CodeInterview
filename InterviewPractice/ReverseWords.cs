using System;
using System.Linq;
using System.Text;

namespace InterviewPractice
{
    /// <summary>
    /// Reverse words in a string given the string is a character array
    /// </summary>
    public static class ReverseWordsHelper
    {
        /// <summary>
        /// Quickest approach - basically a one-liner. Be careful to explain how this 
        /// works to the interviewer if they are not familiar with C#, and likely
        /// you will be asked to proceed to solution 2 bellow without all the fancy
        /// LINQ stuff
        /// </summary>
        public static string ReverseWordsModern(this string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length == 1)
            {
                return s;
            }

            var words = s.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            var result = string.Join(" ", words.Reverse().ToArray());
            return result;
        }

        /// <summary>
        /// This is the "classic" version (using less runtime function)
        /// </summary>
        /// <remarks>Character arrays aren't very popular in C# so this still takes a string
        /// then converts to char array to satisfy the purpose of the interview question
        /// Does not handle double spaces</remarks>
        public static string ReverseWords(this string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length == 1)
            {
                return s;
            }

            var input = s.Trim().ToCharArray();
            var right = input.Length;
            var index = input.Length - 2;
            var sb = new StringBuilder();
            while(index >= 0)
            {
                if (input[index] == ' ')
                {
                    for (var iterInner = index + 1; iterInner < right; iterInner++)
                    {
                        sb.Append(input[iterInner]);
                    }
                    sb.Append(" ");
                    right = index;
                }
                index--;
            }
            for (var iterInner = 0; iterInner < right; iterInner++)
            {
                sb.Append(input[iterInner]);
            }
            return sb.ToString();
        }
    }
}