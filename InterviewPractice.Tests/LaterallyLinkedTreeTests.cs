using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class LaterallyLinkedTreeTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var n1 = new LaterallyLinkedTree<int>.Node<int>(1);
            var n2 = new LaterallyLinkedTree<int>.Node<int>(2);
            var n3 = new LaterallyLinkedTree<int>.Node<int>(3);
            n1.Edges.Add(n2);
            n1.Edges.Add(n3);

            LaterallyLinkedTree<int>.Linkify(n1);
            Assert.AreEqual(n2.Right, n3);
        }
    }
}
