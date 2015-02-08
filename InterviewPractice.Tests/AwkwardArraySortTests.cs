using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class AwkwardArraySortTests
    {
        [TestMethod]
        public void AwkwardArraySortTest()
        {
            Assert.AreEqual(1, new int[] { 4, 3, 5, 6 }.AwkwardArraySort());
            Assert.AreEqual(3, new int[] { 10, 3, 11, 12 }.AwkwardArraySort());
        }
    }
}
