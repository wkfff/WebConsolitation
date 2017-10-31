using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.ObjectTree
{
    public class ObjectTree<T> : DisposableObject, ITree<T>
    {
        public Dictionary<string, ITreeNode<T>> Nodes
        {
            get;
            private set;
        }

        public ObjectTree()
        {
            Nodes = new Dictionary<string, ITreeNode<T>>();
        }

        public ITreeNode<T> AddNode(string key, T dataValue)
        {
            ObjectNode<T> node = new ObjectNode<T>(key, dataValue);
            node.Key = key;
            node.Tree = this;
            Nodes.Add(key, node);
            return node;
        }

        public void AddNode(ITreeNode<T> node)
        {
            ((ObjectNode<T>)node).Tree = this;
            Nodes.Add(node.Key, node);
        }

        public void RemoveNode(string key)
        {
            Nodes[key].Parent.Nodes.Remove(key);
            Nodes.Remove(key);
        }
    }
}
