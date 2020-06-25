using System;

namespace CodeInterview
{
    public partial class Facebook
    {
        /*Calculate and print the sum of the elements in an array, keeping in mind that some of those integers may be quite large.
        Function Description

        Complete the aVeryBigSum function in the editor below. It must return the sum of all array elements.

        aVeryBigSum has the following parameter(s):

        ar: an array of integers .
        Input Format

        The first line of the input consists of an integer .
        The next line contains  space-separated integers contained in the array.

        Output Format

        Print the integer sum of the elements in the array.

        Constraints
        */
        public static Int64 LargeSum(string[] input)
        {
            var count = int.Parse(input[0]);
            var items = input[1].Split(new[] {' '});

            if (items.Length != count)
            {
                throw new ArgumentException("mismatch", "items");
            }

            Int64 result = 0;
            foreach (var iter in items)
            {
                var toAdd = int.Parse(iter);
                result += toAdd;
            }
        
            return result;
        }
    }
}
