using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Channels;

namespace InterviewPractice
{
    public class GraphNode<T>
    {
        public T Value { get; set; }
        public HashSet<GraphNode<T>> Edges;

        public GraphNode(T value)
        {
            this.Value = value;
            this.Edges = new HashSet<GraphNode<T>>();
        }
    }

    public class Graph<T> : IEnumerable<GraphNode<T>>
    {
        private readonly HashSet<GraphNode<T>> _nodes = new HashSet<GraphNode<T>>();

        public void Add(GraphNode<T> parent, GraphNode<T> newNode)
        {
            if (parent.Edges.Contains(newNode))
            {
                throw new ApplicationException("Edge already present");
            }
            
            if (!_nodes.Contains(newNode))
            {
                _nodes.Add(newNode);
            }
            parent.Edges.Add(newNode);
        }

        public IEnumerable<GraphNode<T>> DepthFirstSearch()
        {
            // We need to enumerate all the nodes, because just following links from an
            // arbitrary starting point will not find all the nodes if the graph is not  
            // connected enough. Using this method we likely will enumerate nodes twice.
            // Keep a dictionary of visited nodes.
            // the key is the node being examined, the value is it's parent
            var parents = new HashSet<GraphNode<T>>();

            // use Queue in lieu of recursion
            var toVisit = new Queue<GraphNode<T>>();

            foreach (var current in _nodes)
            {
                if (parents.Contains(current))
                {
                    continue;
                }
                toVisit.Enqueue(current);
                while (toVisit.Any())
                {
                    var currentInner = toVisit.Dequeue();
                    yield return currentInner;
                    foreach (var currentInnerInner in currentInner.Edges)
                    {
                        toVisit.Enqueue(currentInnerInner);
                    }
                }
            }
        }

        public void Remove(GraphNode<T> nodeToRemove)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<GraphNode<T>> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}