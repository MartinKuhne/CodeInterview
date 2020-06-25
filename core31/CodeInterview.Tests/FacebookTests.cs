using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeInterview.Tests
{
    [TestClass]
    public class FacebookTests
    {
        [TestMethod]
        public void TestLargeSum()
        {
            var result = Facebook.LargeSum(new[] {"1", "100"});
            Assert.AreEqual(100, result);

            result = Facebook.LargeSum(new[] {"2", "100 100"});
            Assert.AreEqual(200, result);
        }

        [TestMethod]
        public void DesignerPdf()
        {
            var result = Facebook.DesignerPdf(new[] {"1 3 1 3 1 4 1 3 2 5 5 5 5 5 5 5 5 5 5 5 5 5 5 5 5 5", "abc"});
            Assert.AreEqual(9, result);
        }
    }
}
