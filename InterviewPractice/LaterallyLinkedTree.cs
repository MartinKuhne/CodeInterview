using System;
using System.Collections.Generic;

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

            public Node(T value)
            {
                this.Value = value;
                this.Edges = new List<Node<T>>();
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