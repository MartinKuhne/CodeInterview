using System.CodeDom;
using System.Collections;
using System.Security.Policy;

namespace InterviewPractice
{
    /// <summary>
    /// A palindrome is a sequence of characters which reads the same backward or forward
    /// </summary>
    /// <remarks>
    /// Implemented as extension method 
    /// </remarks>
    public static class Palindrome
    {
        /// <summary>
        /// Is the given string a palindrome? Always a good warm-up question.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks>
        /// It's somewhat doubtful a single character is a palindrome, 
        /// I am not sure what the official ruling is here :) for the purpose
        /// of this code, it IS a palindrome.
        /// </remarks>
        public static bool IsPalindrome(this string s)
        {
            var ar = s.ToCharArray();
            if (ar.Length == 0)
            {
                return false;
            }
            if (ar.Length == 1)
            {
                return true;
            }

            uint startIndex = 0;
            uint endIndex = (uint) ar.Length - 1;
            while (startIndex < endIndex)
            {
                if (ar[startIndex] != ar[endIndex])
                {
                    return false;
                }
                startIndex ++;
                endIndex--;
            }
            return true;
        }

        /// <summary>
        /// very naive version
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ContainsPalindrome(this string s)
        {
            var ar = s.ToCharArray();
            uint length = (uint) ar.Length;
            if (length == 0)
            {
                return false;
            }
            if (length == 1)
            {
                return true;
            }

            for (uint iterOuter = 0; iterOuter < length; iterOuter++)
            {
                var candidate = ar[iterOuter];
                for (uint iterInner = iterOuter + 1; iterInner < length; iterInner++)
                {
                    if (ar[iterInner] != candidate)
                    {
                        continue;
                    }
                    uint startIndex = iterOuter;
                    uint endIndex = iterInner;
                    while (startIndex < endIndex)
                    {
                        if (ar[startIndex] != ar[endIndex])
                        {
                            break;
                        }
                        startIndex++;
                        endIndex--;
                    }
                    if (startIndex >= endIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Slightly less naive version
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks>
        /// I changed my mind about AA being a palindrome. 
        /// </remarks>
        public static bool ContainsPalindrome2(this string s)
        {
            var ar = s.ToCharArray();
            var length = ar.Length;
            if (length < 3)
            {
                return false;
            }

            // this algorithm scans from the center of the palindrome rather than the outer edge
            for (var iterOuter = 0; iterOuter < length - 2; iterOuter++)
            {
                // we test for "AA" and "A*A". rightOffset is 1 for the AA case and 2 for the A*A case.
                for (var rightOffset = 1; rightOffset <= 2; rightOffset++)
                {
                    var left = iterOuter;
                    var right = iterOuter + rightOffset;
                    while (left >= 0 && right < length && ar[left] == ar[right])
                    {
                        left--;
                        right++;
                    }
                    if (right - left > 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}