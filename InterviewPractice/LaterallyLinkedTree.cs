using System;
using System.Collections.Generic;
using System.Linq;

namespace InterviewPractice
{
    /// <summary>
    /// In the laterally linked k-nary, each node has a link to it's neighbor at the same tree depth
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LaterallyLinkedNode<T> where T: IComparable<T>
    {
        public T Value { get; set; }
        public List<LaterallyLinkedNode<T>> Children;
            
        // The distinguishing factor of this class is the lateral pointer
        // (pointer to the next node at the same level)
        public LaterallyLinkedNode<T> Right;

        public LaterallyLinkedNode(T value)
        {
            this.Value = value;
            this.Children = new List<LaterallyLinkedNode<T>>();
            this.Right = null;
        }

        /// <summary>
        /// This solution is relatively easy to write, but requires around log n (depth of tree) storage
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

                foreach (var edge in node.Children)
                {
                    toVisit.Enqueue(new KeyValuePair<int, LaterallyLinkedNode<T>>(level + 1, edge));
                }
            }
        }

        /// <summary>
        /// Solution in constant storage
        /// </summary>
        /// <remarks>
        /// Let's call this "a recursive approach with constant storage". In reality the recursion itself takes up stack space.
        /// </remarks>
        /// <param name="root"></param>
        /// <seealso cref="http://leetcode.com/2010/03/first-on-site-technical-interview.html"/>
        public static void LinkifyConstantStorage(LaterallyLinkedNode<T> root)
        {
            // linkify my own children
            for (var iter = 0; iter < root.Children.Count - 1; iter++)
            {
                root.Children[iter].Right = root.Children[iter + 1];
            }

            var neighbor = root.Right;
            if (neighbor != null && neighbor.Children.Any())
            {
                root.Children.Last().Right = neighbor.Children.First();
            }

            // call down into the next level
            foreach (var child in root.Children)
            {
                LinkifyConstantStorage(child);
            }
        }
    }
}