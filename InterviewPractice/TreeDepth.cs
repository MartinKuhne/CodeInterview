using System.Collections.Generic;

namespace InterviewPractice
{
    /// <summary>
    /// You have binary tree root, node target and distance x. Find all nodes which are located on x distance from target. 
    /// Consider nodes not just "under" target but "above" and "near" it.
    /// ** half hour solution ** does solve it for under and above, but not near
    /// </summary>
    public class TreeDepth
    {
        public TreeDepth[] Edges;
        // first value is child, second value is parent
        public Dictionary<TreeDepth, TreeDepth> Parents = new Dictionary<TreeDepth, TreeDepth>();

        public ICollection<TreeDepth> AboveBelow(TreeDepth root, TreeDepth target, int levels)
        {
            var result = new List<TreeDepth>();
            AboveBelowHelper(result, root, target, levels, -1);
            for (var iter = target; levels > 0 && Parents.ContainsKey(iter); iter = Parents[iter])
            {
                levels--;
                result.Add(iter);
            }
        }

        public void AboveBelowHelper(List<TreeDepth> output, TreeDepth root, TreeDepth target, int desiredLevels, int levelsRemaining)
        {
            // visit the current node
            if (levelsRemaining == -1)
            {
                foreach (var child in root.Edges)
                {
                    Parents.Add(child, root);
                }

                if (root == target)
                {
                    levelsRemaining = desiredLevels;
                }
            }

            if (levelsRemaining >= 0)
            {
                output.Add(root);
                if (levelsRemaining > 0)
                {
                    foreach (var child in root.Edges)
                    {
                        AboveBelowHelper(output, child, target, desiredLevels, levelsRemaining - 1);
                    }
                }
            }
        }

    }
}