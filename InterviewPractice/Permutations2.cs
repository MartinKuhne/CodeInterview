using System.Collections;
using System.Collections.Generic;

namespace InterviewPractice
{
    /// <summary>
    /// Provide all permutations of a string
    /// </summary>
    public class Permutations2 : IEnumerable<string>
    {
        private readonly char[] _input;

        public Permutations2(string input)
        {
            _input = input.ToCharArray();
        }

        public IEnumerator<string> GetEnumerator()
        {
            var len = (uint) _input.Length;
            if (len == 0)
            {
                yield break;
            }

            // Factorial is covered in another interview question!
            var resultCount = len.Factorial();

            // There are len! results
            for (var iter = 0; iter < resultCount - 1; iter++)
            {
                // find out where we are in the "permutation tree"
                // do the permutations, and return the result
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}