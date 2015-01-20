using System.Collections.Generic;
using System.Linq;

namespace InterviewPractice
{
    /// <summary>
    /// detect if single link list is palindrome 
    /// </summary>
    /// <remarks>
    /// Partial, very brute force solution. Only solves the "ANNA" case, not the "ANA" case</remarks>
    public class Node
    {
        public char Data { get; set; }
        public Node Next { get; set; }

        public static bool IsPalindrome(Node head)
        {
            var iter = head;
            var backtrack = new Stack<char>();
            while (iter != null)
            {
                backtrack.Push(iter.Data);
                if (iter.Next != null && iter.Next.Data == iter.Data)
                {
                    var iterInner = iter.Next;
                    var backtrack2 = new Stack<char>();
                    var compare = backtrack.Pop();
                    backtrack2.Push(compare);
                    while (iterInner != null && iterInner.Data == compare && backtrack.Any())
                    {
                        iterInner = iterInner.Next;
                        compare = backtrack.Pop();
                        backtrack2.Push(compare);
                    }
                    if (iterInner.Next == null && !backtrack.Any())
                    {
                        return true;
                    }

                    foreach (var character in backtrack2)
                    {
                        backtrack.Push(character);
                    }
                }
                iter = iter.Next;
            }
            return false;
        }
    }
}