using System;
using System.Collections.Generic;
using System.Linq;

namespace InterviewPractice
{
    /// <summary>
    /// In the laterally linked tree, each node has a link to it's neighbor at the same tree depth
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LaterallyLinkedTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        public class Node<T> where T: IComparable<T>
        {
            public T Value { get; set; }
            public List<Node<T>> Edges;
            
            // The distinguishing factor of this class is the lateral pointer
            // (pointer to the next node at the same level)
            public Node<T> Right;

            public Node(T value)
            {
                this.Value = value;
                this.Edges = new List<Node<T>>();
                this.Right = null;
            }
        }

        /// <summary>
        /// This solution is relatively easy to write, but 
        /// </summary>
        /// <param name="Root"></param>
        public static void Linkify(Node<T> Root)
        {
            var neighbors = new Dictionary<int, Node<T>>();
            var toVisit = new Queue<KeyValuePair<int, Node<T>>>();
            toVisit.Enqueue(new KeyValuePair<int, Node<T>>(1, Root));
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
                    toVisit.Enqueue(new KeyValuePair<int, Node<T>>(level + 1, edge));
                }
            }
        }
         
        public IEnumerator<T> GetEnumerator()
        {
 	        throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
 	        throw new NotImplementedException();
        }
    }
}