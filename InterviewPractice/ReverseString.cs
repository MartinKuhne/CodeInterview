using System.Security;

namespace InterviewPractice
{
    public static class ReverseStringHelper
    {
        public static string ReverseMe(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            var ar = input.ToCharArray();
            var left = 0;
            var right = input.Length - 1;
            while (left < right)
            {
                var temp = ar[left];
                ar[left] = ar[right];
                ar[right] = temp;
                left++;
                right--;
            }
            return new string(ar);
        }
    }
}