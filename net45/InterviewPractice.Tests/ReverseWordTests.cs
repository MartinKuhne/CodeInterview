using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class ReverseWordTests
    {
        [TestMethod]
        public void ReverseWordTest()
        {
            Assert.AreEqual("","".ReverseWords());
            Assert.AreEqual("S", "S".ReverseWords());
            Assert.AreEqual("A B", "B A ".ReverseWords());
            Assert.AreEqual("A B", " B A".ReverseWords());
            Assert.AreEqual("A B", " B A ".ReverseWords());
            Assert.AreEqual("Beer Free And Sex", "Sex And Free Beer".ReverseWords());
        }

        public void ReverseWordModernTest()
        {
            Assert.AreEqual("", "".ReverseWordsModern());
            Assert.AreEqual("S", "S".ReverseWordsModern());
            Assert.AreEqual("A B", "B A ".ReverseWordsModern());
            Assert.AreEqual("A B", " B A".ReverseWordsModern());
            Assert.AreEqual("A B", " B A ".ReverseWordsModern());
            Assert.AreEqual("Beer Free And Sex", "Sex And Free Beer".ReverseWordsModern());
        }
    }
}
