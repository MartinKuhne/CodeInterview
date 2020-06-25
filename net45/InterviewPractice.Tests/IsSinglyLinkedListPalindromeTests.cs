using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class IsSinglyLinkedListPalindromeTests
    {
        [TestMethod]
        public void IsSinglyLinkedListPalindromeTest()
        {
            var n1 = new Node { Data = 'A' };
            var n2 = new Node { Data = 'N' };
            var n3 = new Node { Data = 'N' };
            var n4 = new Node { Data = 'A' };
            n1.Next = n2;
            n2.Next = n3;
            n3.Next = n4;
            Assert.IsTrue(Node.IsPalindrome(n1));
            n2.Data = 'R';
            Assert.IsFalse(Node.IsPalindrome(n1));
        }
    }
}
