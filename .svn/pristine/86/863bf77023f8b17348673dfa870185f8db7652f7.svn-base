using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.ObjectTree
{
    public class ObjectNode<T> : DisposableObject, ITreeNode<T>
    {
        public Dictionary<string, ITreeNode<T>> Nodes
        {
            get;
            private set;
        }

        /// <summary>
        /// объект, содержащийся в узле
        /// </summary>
        public T DataValue
        {
            get;
            set;
        }

        public string Key
        {
            get;
            internal set;
        }

        public ObjectNode(string key, T dataValue)
        {
            Nodes = new Dictionary<string, ITreeNode<T>>();
            DataValue = dataValue;
            Key = key;
        }

        public void AddChild(ITreeNode<T> childNode)
        {
            Nodes.Add(childNode.Key, childNode);
            ((ObjectNode<T>)childNode).Parent = this;
            Tree.AddNode(childNode);
        }

        public ITreeNode<T> AddChild(string key, T dataValue)
        {
            var childNode = new ObjectNode<T>(key, dataValue);
            Nodes.Add(key, childNode);
            childNode.Parent = this;
            Tree.AddNode(childNode);
            return childNode;
        }

        public ITreeNode<T> GetChildNode(string key)
        {
            return Nodes[key];
        }

        public ITreeNode<T> Parent
        {
            get;
            private set;
        }

        public ITree<T> Tree
        {
            get;
            internal set;
        }

        public void RemoveChild(string key)
        {
            Nodes.Remove(key);
            if (Tree.Nodes.ContainsKey(key))
                Tree.RemoveNode(key);
        }
    }
}
