using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class NaiveSubstringTests
    {
        [TestMethod]
        public void NaiveSubstringTest()
        {
            var input = "ABBAANNA".ToCharArray();
            Assert.IsTrue(NaiveSubstringHelper.ContainsBruteForce(input, "ANNA".ToCharArray()));
            Assert.IsTrue(NaiveSubstringHelper.ContainsBruteForce(input, "BB".ToCharArray()));
            Assert.IsFalse(NaiveSubstringHelper.ContainsBruteForce(input, "BEER".ToCharArray()));
            Assert.IsFalse(NaiveSubstringHelper.ContainsBruteForce(input, "BEERBEERBEER".ToCharArray()));
            Assert.IsFalse(NaiveSubstringHelper.ContainsBruteForce("".ToCharArray(), "".ToCharArray()));
            Assert.IsFalse(NaiveSubstringHelper.ContainsBruteForce(input, "".ToCharArray()));
        }
    }
}
