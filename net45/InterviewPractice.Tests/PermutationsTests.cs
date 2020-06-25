using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class PermutationsTests
    {
        [TestMethod]
        public void PermutationsPermutations()
        {
            var p = new Permutations(null);
            Assert.AreEqual(0, p.Count(), "null");
            p = new Permutations("");
            Assert.AreEqual(0, p.Count(), "empty");
            p = new Permutations(" ");
            Assert.AreEqual(0, p.Count(), "Whitespace");
            p = new Permutations("A");
            Assert.AreEqual(1, p.Count(), "1!");
            p = new Permutations("AB");
            Assert.AreEqual(2, p.Count(), "2!");
            p = new Permutations("ABC");
            Assert.AreEqual(6, p.Count(), "3!");
        }

        [TestMethod]
        public void PermutationsResult()
        {
            var p = new Permutations("ABC").ToArray();
            var expected = new[] {"ABC", "BAC", "CBA", "ACB", "BCA", "CAB"};
            CollectionAssert.AreEquivalent(expected, p.ToArray());
        }
    }
}