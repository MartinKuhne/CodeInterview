using System;
using System.Collections.Generic;
using System.Linq;

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

        /*Input Format

        The first line contains  space-separated integers describing the respective heights of each consecutive lowercase English letter, ascii[a-z].
        The second line contains a single word, consisting of lowercase English alphabetic letters.

        Constraints

        , where  is an English lowercase letter.
        contains no more than  letters.
        Output Format

        Print a single integer denoting the area in  of highlighted rectangle when the given word is selected. Do not print units of measure.*/

        public static int DesignerPdf(string[] input)
        {
            // I am finding the input format hilarious. It totally goes against the grain to combine business logic and input parsing in one function.
            if (input == null || input.Length != 2)
            {
                throw new ArgumentException("bad argument count", "input");
            }

            var sizes = input[0].Split(new[] {' '}).Select(sizeString => int.Parse(sizeString)).ToArray();

            var sizeMap = new Dictionary<char, int>();
            int k = 0;
            foreach(var iter in Enumerable.Range('a', 26)) // I had to look this up, awkward to ask in an interview, who does this really?
            {
                sizeMap.Add((char)iter, sizes[k++]);
            }

            var word = input[1];
            if (string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentException("no content", "word");
            }

            var ar = word.ToCharArray();
            int maxHeight = 0;
            foreach (var character in ar)
            {
                maxHeight = Math.Max(maxHeight, sizeMap[character]);
                if (maxHeight == 7)
                {
                    break;
                }
            }

            return ar.Length * maxHeight;
        }
    }
}
