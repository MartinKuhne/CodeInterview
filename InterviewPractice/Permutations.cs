using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InterviewPractice
{
    /// <summary>
    /// Provide all permutations of a string
    /// </summary>
    public class Permutations : IEnumerable<string>
    {
        private class PermContext
        {
            public string Value { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
        }
        
        private readonly string _input;

        public Permutations(string input)
        {
            _input = input;
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (string.IsNullOrWhiteSpace(_input))
            {
                yield break;
            }

            var workItems = new Stack<PermContext>();
            workItems.Push(new PermContext {Value = _input, StartIndex = 0, EndIndex = _input.Length -1});

            while (workItems.Any())
            {
                var context = workItems.Pop();
                var ar = context.Value.ToCharArray();
                for (var index = context.StartIndex; index <= context.EndIndex; index++)
                {
                    var swap = ar[context.StartIndex];
                    ar[context.StartIndex] = ar[index];
                    ar[index] = swap;
                    if ((context.EndIndex - context.StartIndex) <= 1)
                    {
                        string result = new string(ar);
                        yield return result;
                    }
                    else
                    {
                        workItems.Push(new PermContext { Value = new string(ar), StartIndex = context.StartIndex + 1, EndIndex = context.EndIndex });
                    }

                    swap = ar[context.StartIndex];
                    ar[context.StartIndex] = ar[index];
                    ar[index] = swap;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}