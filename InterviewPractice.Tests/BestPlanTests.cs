using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewPractice.Tests
{
    [TestClass]
    public class BestPlanTests
    {
        [TestMethod]
        public void BestPlanTest1()
        {
            var bp = new BestPlan();
            var feat = new List<BestPlan.Feature> { new BestPlan.Feature { Name = "Rome" }, new BestPlan.Feature { Name = "Paris" }, new BestPlan.Feature { Name = "Munich" }, new BestPlan.Feature { Name = "Venice" } };
            var features = feat.ToDictionary(feature => feature.Name);

            bp.Add(new BestPlan.Plan { Name = "Just Rome", Cost = 10, Features = new [] {features["Rome"]}});
            bp.Add(new BestPlan.Plan { Name = "Just Paris", Cost = 15, Features = new[] { features["Paris"] } });
            bp.Add(new BestPlan.Plan { Name = "Just Munich", Cost = 20, Features = new[] { features["Munich"] } });
            bp.Add(new BestPlan.Plan { Name = "Rome+Paris", Cost = 20, Features = new[] { features["Rome"], features["Paris"] } });
            bp.Add(new BestPlan.Plan { Name = "Rome+Munich", Cost = 25, Features = new[] { features["Rome"], features["Munich"] } });
            bp.Add(new BestPlan.Plan { Name = "Sale!Rome+Venice", Cost = 3, Features = new[] { features["Rome"], features["Venice"] } });

            var route = bp.LeastExpensive(new[] {features["Rome"], features["Munich"]});
            Assert.AreEqual(23, route.Sum(p => p.Cost));
        }
    }
}
