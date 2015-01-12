using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InterviewPractice
{
    /// <summary>
    /// Every node on the right subtree has to be larger than the current node
    /// Every node on the left subtree has to be smaller than the current node
    /// Identical values are not allowed
    /// cf. http://en.wikipedia.org/wiki/Binary_search_tree
    /// </summary>
    /// <remarks>
    /// IComparable<T>.CompareTo(T other) is
    /// Less than zero- This object is less than the other parameter.
    /// Zero - This object is equal to other
    /// Greater than zero - This object is greater than other.
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class BinarySearchTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        internal class Node<T> where T: IComparable<T>
        {
            internal T Value { get; set; }
            internal Node<T>[] Edges;

            internal Node(T value)
            {
                this.Value = value;
                this.Edges = new Node<T>[] {null, null};
            }
        }

        private Node<T> Root;

        public void Add(T value)
        {
            // special case empty list
            if (Root == null)
            {
                Root = new Node<T>(value);
                return;
            }

            var iter = Root;

            while (true)
            {
                var compareResult = iter.Value.CompareTo(value);
                if (compareResult == 0)
                {
                    throw new NotImplementedException("Duplicates not allowed");
                }

                // Edges[0] for left, Edges[1] for right
                var edgeIndex = compareResult > 0 ? 0 : 1;

                // if the ednge below is is empty, we'll just add ourselves there
                if (iter.Edges[edgeIndex] == null)
                {
                    iter.Edges[edgeIndex] = new Node<T>(value);
                    return;
                }

                // special case left side and we are smaller than the current, but
                // larger than the child. Here, we need on insert ourselves in between
                if (edgeIndex == 0 && iter.Edges[0].Value.CompareTo(value) < 0)
                {
                    var newNode = new Node<T>(value);
                    newNode.Edges[0] = iter.Edges[0];
                    iter.Edges[0] = newNode;
                    return;
                }

                // same case as previous, but right and larger/smaller
                if (edgeIndex == 1 && iter.Edges[1].Value.CompareTo(value) > 0)
                {
                    var newNode = new Node<T>(value);
                    newNode.Edges[1] = iter.Edges[1];
                    iter.Edges[1] = newNode;
                    return;
                }

                // if we are NOT in the two cases above, continue down the tree
                iter = iter.Edges[edgeIndex];
            }
        }

        public override string ToString()
        {
            var strings = this.Select(i => i.ToString()).ToList();
            var result = string.Join(",", strings);
            return result;
        }

        public bool Contains(T value)
        {
            if (Root == null)
            {
                return false;
            }

            var iter = Root;
            while (true)
            {
                var compareResult = iter.Value.CompareTo(value);
                if (compareResult == 0)
                {
                    return true;
                }

                var edgeIndex = compareResult > 0 ? 0 : 1;

                if (iter.Edges[edgeIndex] == null)
                {
                    return false;
                }

                iter = iter.Edges[edgeIndex];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (Root == null)
            {
                yield break;
            }

            var toEnumerate = new Stack<Node<T>>();
            toEnumerate.Push(Root);
            while (toEnumerate.Count > 0)
            {
                var iter = toEnumerate.Pop();
                yield return (iter.Value);
                foreach (var edge in iter.Edges.Where(edge => edge != null))
                {
                    toEnumerate.Push(edge);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}