using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewPractice
{
    public class PowerSetHelper
    {
        public static List<string> PowerSet(string input)
        {
            var result = new List<string>();
            var ar = input.ToCharArray();
            var setSize = ar.Length;

            // note overflow risk
            var resultCount = Convert.ToInt32(Math.Pow(2, setSize));

            result.Add(string.Empty);
            if (setSize == 0)
            {
                return result;
            }

            result.Add(input);
            var sb = new StringBuilder();
            foreach (var iter in Enumerable.Range(1, resultCount - 2))
            {
                var index = 0;
                var bitmask = iter;
                while (bitmask > 0)
                {
                    if ((bitmask & 1) == 1)
                    {
                        sb.Append(ar[index]);
                    }
                    bitmask >>= 1;
                    index++;
                }
                result.Add(sb.ToString());
                sb.Clear();
            }
            return result;
        }
    }
}
