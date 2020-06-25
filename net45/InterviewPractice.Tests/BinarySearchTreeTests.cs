using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class BinarySearchTreeTests
    {
        [TestMethod]
        public void WhenICreateASimpleTreeItHasTheCorrectOrder()
        {
            var t = new BinarySearchTree<int> {1, 2, 3};
            Assert.AreEqual(t.ToString(), "1,2,3");
        }

        [TestMethod]
        public void WhenICreateASimpleTreeItContainsAllValues()
        {
            var t = new BinarySearchTree<int> { 1, 2, 3 };
            Assert.IsTrue(t.Contains(1));
            Assert.IsTrue(t.Contains(2));
            Assert.IsTrue(t.Contains(3));
            Assert.IsFalse(t.Contains(4));
        }

        [TestMethod]
        public void WhenICreateASimpleTreeItEnumeratesInTheCorrectOrder()
        {
            var t = new BinarySearchTree<int> { 1, 2, 3 };
            var values = t.ToArray();
            Assert.AreEqual(values[0], 1);
            Assert.AreEqual(values[1], 2);
            Assert.AreEqual(values[2], 3);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void WhenIAddADuplicateValueItThrowsAnException()
        {
            var t = new BinarySearchTree<int>();
            t.Add(1);
            t.Add(1);
        }
    }
}
