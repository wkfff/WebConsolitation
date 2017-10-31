using System.Collections.Generic;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.ServerLibrary
{
    public interface ITemplateReport
    {
        int Id
        {
            get;
        }

        string Key
        {
            get;
        }

        string Caption
        {
            get;
        }

        string Description
        { 
            get;
        }

        TemplateDocumentTypes DocumentType
        {
            get;
        }

        string DocumentName
        { 
            get;
        }
    }

    public interface ITreeNode<T>
    {
        void AddChild(ITreeNode<T> childNode);

        ITreeNode<T> AddChild(string key, T dataValue);

        ITreeNode<T> GetChildNode(string key);

        ITreeNode<T> Parent
        { 
            get;
        }

        ITree<T> Tree
        { 
            get;
        }

        string Key
        { 
            get;
        }

        T DataValue
        {
            get; set;
        }

        Dictionary<string, ITreeNode<T>> Nodes
        { 
            get;
        }

        void RemoveChild(string key);
    }

    public interface ITree<T>
    {
        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, ITreeNode<T>> Nodes
        { 
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataValue"></param>
        /// <returns></returns>
        ITreeNode<T> AddNode(string key, T dataValue);

        void AddNode(ITreeNode<T> node);

        void RemoveNode(string key);
    }

    public interface IReportsTreeService
    {
        ITree<ITemplateReport> GetReportsTree(string topNodeCode);

        ITree<ITemplateReport> GetSystemReportsTree();

        ITree<ITemplateReport> GetReportsTree(TemplateTypes templateType);
    }
}
