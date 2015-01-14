using System;
using System.Collections.Generic;
using System.Linq;

namespace InterviewPractice
{
    /// <summary>
    /// In the laterally linked tree, each node has a link to it's neighbor at the same tree depth
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LaterallyLinkedNode<T> where T: IComparable<T>
    {
        public T Value { get; set; }
        public List<LaterallyLinkedNode<T>> Edges;
            
        // The distinguishing factor of this class is the lateral pointer
        // (pointer to the next node at the same level)
        public LaterallyLinkedNode<T> Right;

        public LaterallyLinkedNode(T value)
        {
            this.Value = value;
            this.Edges = new List<LaterallyLinkedNode<T>>();
            this.Right = null;
        }

        /// <summary>
        /// This solution is relatively easy to write, but requires around log n (depth of tree) storage
        /// Improvement: This approach does not make use of the Right pointers previously generated.
        /// </summary>
        /// <param name="root"></param>
        public static void Linkify(LaterallyLinkedNode<T> root)
        {
            var neighbors = new Dictionary<int, LaterallyLinkedNode<T>>();
            var toVisit = new Queue<KeyValuePair<int, LaterallyLinkedNode<T>>>();
            toVisit.Enqueue(new KeyValuePair<int, LaterallyLinkedNode<T>>(1, root));
            while (toVisit.Any())
            {
                var current = toVisit.Dequeue();
                var level = current.Key;
                var node = current.Value;

                if (neighbors.ContainsKey(level))
                {
                    var left = neighbors[level];
                    left.Right = node;
                    neighbors.Remove(level);
                }
                neighbors.Add(level, node);

                foreach (var edge in node.Edges)
                {
                    toVisit.Enqueue(new KeyValuePair<int, LaterallyLinkedNode<T>>(level + 1, edge));
                }
            }
        }

        /// <summary>
        /// Solution in constant storage
        /// </summary>
        /// <param name="root"></param>
        /// <seealso cref="http://leetcode.com/2010/03/first-on-site-technical-interview.html"/>
        public static void LinkifyConstantStorage(LaterallyLinkedNode<T> root)
        {
            // I was able to find an O(1) storage solution online, but it involved recursion
            // I didn't dig any further. I think my blocker in understanding the question was 
            // that I didn't consider a recursive function to have constant storage
        }
    }
}