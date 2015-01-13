using InterviewPractice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class FactorialTests
    {
        [TestMethod]
        public void FactorialTest1()
        {
            uint x = 1;
            Assert.AreEqual((uint) 1, x.Factorial());
            x = 5;
            Assert.AreEqual((uint) 120, x.Factorial());
        }
    }
}
