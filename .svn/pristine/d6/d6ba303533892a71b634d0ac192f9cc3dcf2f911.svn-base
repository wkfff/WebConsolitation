using System;
using System.Collections.Generic;
using System.Linq;

namespace Krista.FM.Extensions
{
    public static class LinqExtensions
    {
        public class Node<T>
        {
            internal Node()
            {
            }

            public int Level;
            public Node<T> Parent;
            public T Item;
            public List<Node<T>> Nodes = new List<Node<T>>();
        }

        public static IEnumerable<Node<T>> ByHierarchy<T>(
            this IEnumerable<T> source,
            Func<T, bool> startWith,
            Func<T, T, bool> connectBy)
        {
            return source.ByHierarchy<T>(startWith, connectBy, null);
        }


        private static IEnumerable<Node<T>> ByHierarchy<T>(
            this IEnumerable<T> source,
            Func<T, bool> startWith,
            Func<T, T, bool> connectBy,
            Node<T> parent)
        {
            int level = (parent == null ? 0 : parent.Level + 1);

            if (source == null)
                throw new ArgumentNullException("source");

            if (startWith == null)
                throw new ArgumentNullException("startWith");

            if (connectBy == null)
                throw new ArgumentNullException("connectBy");

            foreach (T value in from item in source
                                where startWith(item)
                                select item)
            {
                Node<T> newNode = new Node<T> { Level = level, Parent = parent, Item = value };

                if (parent != null)
                {
                    parent.Nodes.Add(newNode);
                }

                yield return newNode;

                foreach (Node<T> subNode in source.ByHierarchy<T>(possibleSub => connectBy(value, possibleSub),
                                                                  connectBy, newNode))
                {
                    yield return subNode;
                }
            }
        }
    }
}