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
            var n1 = new LaterallyLinkedNode<int>(1);
            var n2 = new LaterallyLinkedNode<int>(2);
            var n3 = new LaterallyLinkedNode<int>(3);
            n1.Edges.Add(n2);
            n1.Edges.Add(n3);

            LaterallyLinkedNode<int>.Linkify(n1);
            Assert.IsNull(n1.Right);
            Assert.AreEqual(n2.Right, n3);
            Assert.IsNull(n3.Right);
        }
    }
}
