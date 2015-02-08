using System;
using System.Runtime.InteropServices;

namespace InterviewPractice
{
    /// <summary>
    /// You are given an array of positive integers. Convert it to a sorted array with minimum cost. 
    /// Valid operation are
    /// 1) Decrement -> cost = 1
    /// 2) Delete an element completely from the array -> cost = value of element
    /// </summary>
    /// <example>
    /// 4,3,5,6, -> cost 1 //cost is 1 to make 4->3
    /// 10,3,11,12 -> cost 3 // cost 3 to remove 3
    /// </example>
    public static class AwkwardArraySortHelper
    {
        public static int AwkwardArraySort(this int[] input)
        {
            var cost = 0;
            for (var index = 0; index < input.Length - 2; index++)
            {
                if (input[index] <= input[index + 1]) continue;

                var costSmash = input[index] - input[index + 1];
                if (costSmash < input[index + 1])
                {
                    cost += costSmash;
                }
                else
                {
                    cost += input[index + 1];
                }
            }
            return cost;
        }
    }
}