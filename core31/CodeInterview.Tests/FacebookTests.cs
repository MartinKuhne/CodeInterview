using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeInterview.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var result = Facebook.LargeSum(new[] {"1", "100"});
            Assert.AreEqual(100, result);

            result = Facebook.LargeSum(new[] {"2", "100 100"});
            Assert.AreEqual(200, result);
        }
    }
}
