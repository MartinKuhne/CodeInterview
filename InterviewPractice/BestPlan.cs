using System;
using System.Collections.Generic;
using System.Linq;

namespace InterviewPractice
{
    /// <summary>
    /// From a list of available plans and desired features, 
    /// select the least expensive combination of plans satisfying the list of features
    /// </summary>
    public class BestPlan
    {
        // --- begin code provided ---

        public class Feature
        {
            public string Name { get; set; }
        }

        public class Plan
        {
            public string Name { get; set; }
            public double Cost { get; set; }
            public Feature[] Features { get; set; }
        }

        // this is the list of plans available instantiated as per the above
        private List<Plan> allPlans = new List<Plan>();

        // --- end code provided ---

        public void Add(Plan plan)
        {
            allPlans.Add(plan);
        }

        readonly Dictionary<string, List<string>> MapPlanFeature = new Dictionary<string, List<string>>();

        /// <summary>
        /// Find the least expensive combination of plans
        /// </summary>
        /// <param name="selectedFeatures">
        /// (quoted from requirements) this is the list of features the user wants 
        // -> find combinations of 1-N plans that fulfill those features 
        // -> select the cheapest combination(s) 
        /// </param>
        /// <remarks>
        /// The depth part of the recursion (higher values of columnIndex) will re-compute the same subset
        /// of permutations. Dynamnic programming (caching of solutions on subset of the data) is applicable.
        /// </remarks>
        /// <returns>
        /// IEnumerable<Plan> or null if no plan available
        /// </returns>
        public IEnumerable<Plan> LeastExpensive(Feature[] selectedFeatures)
        {
            BuildMapFeaturePlan();
            // convert array to HashSet because we're going to do a lot of lookups
            var desiredFeatures = new HashSet<string>(selectedFeatures.Select(f => f.Name).Distinct());

            // build a list of plans which contain at least one of the desired features
            var planOptions = allPlans.Where(p => p.Features.Select(f => f.Name).Intersect(desiredFeatures).Any()).ToArray();
            var planFeatureOptions = planOptions.SelectMany(p => p.Features.Select(f => f.Name));
            
            // there may be no plans available
            if (desiredFeatures.Except(planFeatureOptions).Any())
            {
                return null;
            }

            // build a matrix of plans and features
            var features = desiredFeatures.ToArray();
            var plans = planOptions.ToArray();
            var planFeatureMatrix = new int[planOptions.Length, desiredFeatures.Count];
            for (var iter = 0; iter < planOptions.Length; iter++)
            {
                for (var iterInner = 0; iterInner < desiredFeatures.Count; iterInner++)
                {
                    if (MapPlanFeature[plans[iter].Name].Contains(features[iterInner]))
                    {
                        planFeatureMatrix[iter, iterInner] = 1;
                    }
                }
            }

            // now, we can iterate over the successful permutations using the points in the matrix
            // starting best result is all plans
            var bestResult = plans;
            var planCombinations = new int[desiredFeatures.Count];
            Backtrack(ref plans, ref planFeatureMatrix, ref planCombinations, 0, ref bestResult);
            return bestResult;
        }

        /// <summary>
        /// This is the recursion that goes over all permutations of plans which meet
        /// the criteria. Imagine a matrix where the Plans are rows and the Features are columns.
        /// In the matrix, there is a 1 whereever the plan fulfills the feature.
        /// Now, imagine scanning each column with your fingers, stopping whereever there is a 1
        /// indicating you have found one of the plans that satisfies your criteria.
        /// </summary>
        /// <remarks>
        /// As a backtracking algorithm, we are using minimal and linear resources (the only variable is the columIndex)
        /// </remarks>
        /// <param name="plans"></param>
        /// <param name="permutations"></param>
        /// <param name="planCombination"></param>
        /// <param name="columnIndex"></param>
        /// <param name="bestResult"></param>
        public void Backtrack(ref Plan[] plans, ref int[,] permutations, ref int[] planCombination, int columnIndex, ref Plan[] bestResult)
        {
            if (columnIndex == permutations.GetLength(1))
            {
                // at the tail of the recursion, identify the plans which belong to this permutation
                // and cost it. If the cost is lower then the previous solution, remember it
                var selectedPlans = new HashSet<Plan>();
                for (var iter = 0; iter < planCombination.Length; iter++)
                {
                    var plan = plans[planCombination[iter]];
                    if (!selectedPlans.Contains(plan))
                    {
                        selectedPlans.Add(plan);
                    }
                }
                var cost = selectedPlans.Sum(p => p.Cost);
                if (cost < bestResult.Sum(p => p.Cost))
                {
                    bestResult = selectedPlans.ToArray();
                    Console.WriteLine("{0} {1}", string.Join(",", selectedPlans.Select(p => p.Name)), cost);
                }
                return;
            }

            // go over all the permutations by setting the current level value in turn to all possible values 
            // then calling ourselves to do the permutations for the next level
            for (var iter = 0; iter < permutations.GetLength(0); iter++)
            {
                if (permutations[iter, columnIndex] == 1)
                {
                    planCombination[columnIndex] = iter;
                    Backtrack(ref plans, ref permutations, ref planCombination, columnIndex + 1, ref bestResult);
                }
            }
        }

        /// <summary>
        /// Build a mapping from feature to plan, and plan to features for speedier lookups
        /// </summary>
        private void BuildMapFeaturePlan()
        {
            MapPlanFeature.Clear();
            foreach (var plan in allPlans)
            {
                MapPlanFeature.Add(plan.Name, new List<string>(plan.Features.Select(f => f.Name)));
            }
        }

        /// <summary>
        /// Eliminate plans which are clearly a subset or same cost equivalent of another plan
        /// </summary>
        /// <param name="features"></param>
        /// <param name="plans"></param>
        private void RemoveSubset(IEnumerable<string> features, ICollection<Plan> plans)
        {
            // plan did not leave idea stage
        }
    }
}