using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    /// <summary>
    /// Summary description for PalindromeTests
    /// </summary>
    [TestClass]
    public class PalindromeTests
    {
        [TestMethod]
        public void IsPalindromeTest()
        {
            Assert.IsFalse("".IsPalindrome());
            Assert.IsTrue("A".IsPalindrome());
            Assert.IsTrue("AA".IsPalindrome());
            Assert.IsTrue("ANNA".IsPalindrome());
            Assert.IsTrue("ANA".IsPalindrome());
            Assert.IsFalse("BEER".IsPalindrome());
        }

        [TestMethod]
        public void ContainsPalindromeTest()
        {
            Assert.IsFalse("".ContainsPalindrome());
            Assert.IsTrue("A".ContainsPalindrome());
            Assert.IsTrue("AA".ContainsPalindrome());
            Assert.IsTrue("ANNA".ContainsPalindrome());
            Assert.IsTrue("ANA".ContainsPalindrome());
            Assert.IsFalse("KIND".ContainsPalindrome());
            Assert.IsTrue("ANNAKIND".ContainsPalindrome());
            Assert.IsTrue("KINDANNA".ContainsPalindrome());
            Assert.IsTrue("KINDANNAKIND".ContainsPalindrome());
        }

        [TestMethod]
        public void ContainsPalindrome2Test()
        {
            Assert.IsFalse("".ContainsPalindrome2());
            // as noted in the code, A and AA are no longer palindromes for this version
            Assert.IsFalse("A".ContainsPalindrome2());
            Assert.IsFalse("AA".ContainsPalindrome2());
            Assert.IsTrue("ANNA".ContainsPalindrome2());
            Assert.IsTrue("ANA".ContainsPalindrome2());
            Assert.IsFalse("KIND".ContainsPalindrome2());
            Assert.IsTrue("ANNAKIND".ContainsPalindrome2());
            Assert.IsTrue("KINDANNA".ContainsPalindrome2());
            Assert.IsTrue("KINDANNAKIND".ContainsPalindrome2());
        }
    }
}
