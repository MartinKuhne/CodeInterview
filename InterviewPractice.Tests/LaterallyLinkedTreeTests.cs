using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class LaterallyLinkedTreeTests
    {
        [TestMethod]
        public void LaterallyLinkedTreeTest()
        {
            var n1 = new LaterallyLinkedNode<int>(1);
            var n2 = new LaterallyLinkedNode<int>(2);
            var n3 = new LaterallyLinkedNode<int>(3);
            n1.Children.Add(n2);
            n1.Children.Add(n3);

            LaterallyLinkedNode<int>.Linkify(n1);
            Assert.IsNull(n1.Right);
            Assert.AreEqual(n2.Right, n3);
            Assert.IsNull(n3.Right);
        }
        public void LaterallyLinkedTreeTestConstantStorage()
        {
            var n1 = new LaterallyLinkedNode<int>(1);
            var n2 = new LaterallyLinkedNode<int>(2);
            var n3 = new LaterallyLinkedNode<int>(3);
            var n4 = new LaterallyLinkedNode<int>(4);
            var n5 = new LaterallyLinkedNode<int>(5);
            var n6 = new LaterallyLinkedNode<int>(6);
            var n7 = new LaterallyLinkedNode<int>(7);
            n1.Children.Add(n2);
            n1.Children.Add(n3);
            n1.Children.Add(n4);
            n2.Children.Add(n5);
            n2.Children.Add(n6);
            n4.Children.Add(n7);

            LaterallyLinkedNode<int>.LinkifyConstantStorage(n1);
            Assert.IsNull(n1.Right);
            Assert.AreEqual(n2.Right, n3);
            Assert.AreEqual(n3.Right, n4);
            Assert.IsNull(n4.Right);
            Assert.AreEqual(n5.Right, n6);
            Assert.AreEqual(n6.Right, n7);
            Assert.IsNull(n7.Right);
        }
    }
}
