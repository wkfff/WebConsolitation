// ******************************************************************
// ������ �������� ������� ����������� ������ ��� �������� � ���������,
// ������������ ��� ������������� ��������� �������.
// ����� �������������� ������� ������������� �������� � ��� ��������� ���������� 
// ���� ������������ ���� (�� ��������) � �������� ��������� internal
// ******************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region ������� ����������� ������

    /// <summary>
    /// ����� ������� ����� ��� ����� �������� ��������� �������.
    /// ����� ���, ����� ����������� �� XML, ����������� ������� � 
    /// ����������� �������� ������������ ���������
    /// </summary>
    public abstract class ProcessObject
    {
        internal PumpProgram parentProgram;

        /// <summary>
        /// �������� ���� XML ��� ���� <���>
        /// </summary>
        internal string nameTagName = XmlConsts.nameAttr;

        /// <summary>
        /// ��� �������
        /// </summary>
        internal string name;

        protected ProcessObject()
        {
        }

        /// <summary>
        /// ������������� �����������
        /// </summary>
        /// <param name="nameTagName">�������� ���� XML ��� ���� <���></param>
        public ProcessObject(PumpProgram parentProgram, string nameTagName)
        {
            this.parentProgram = parentProgram;
            if (!String.IsNullOrEmpty(nameTagName))
                this.nameTagName = nameTagName;
        }

        /// <summary>
        /// ��������� ������ �� XML
        /// </summary>
        /// <param name="node">����</param>
        internal virtual void LoadFromXml(XmlNode node)
        {
            this.name = XmlHelper.GetStringAttrValue(node, nameTagName, String.Empty);
        }

        /// <summary>
        /// ��������� ������������ ��������� �������
        /// </summary>
        /// <param name="pumpModule">������������ ������ �������, ������������ ��� ������� � ����� ������, ����� � �.�.</param>
        /// <returns></returns>
        internal virtual string Validate()
        {
            return String.Empty;
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        internal virtual void Clear()
        {
        }
    }

    /// <summary>
    /// ������ � "���������"
    /// </summary>
    public abstract class ProcessObjectWithSynonym : ProcessObject
    {
        /// <summary>
        /// "�������", ������������ � �������� ����� � ������������ ����������
        /// </summary>
        internal string synonym;

        public ProcessObjectWithSynonym(PumpProgram parentProgram, string nameTagName) : base (parentProgram, nameTagName)
        {
        }

        /// <summary>
        /// ��������� ������ �� XML
        /// </summary>
        /// <param name="node">����</param>
        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.synonym = XmlHelper.GetStringAttrValue(node, XmlConsts.synonymAttr, String.Empty);
        }
    }
    #endregion

    #region ������� ����������� ��������� � ������
    /// <summary>
    /// ������ �������� (������ ��� ���������)
    /// </summary>
    public abstract class ObjectsGroup : IEnumerable
    {
        /// <summary>
        /// �������� ���� XML ��� ������� ������ ��������
        /// </summary>
        protected string elemTagName;
        
        /// <summary>
        /// ��� ��������� (��� ���������������� ��������)
        /// </summary>
        protected Type elemType;

        /// <summary>
        /// ������� ����������� ��� ����������
        /// </summary>
        protected ObjectsGroup()
        {
        }

        internal PumpProgram parentProgram;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="elemTagName">�������� ���� XML ��� ������� ������ ��������</param>
        /// <param name="elemType">��� ��������� ������</param>
        protected ObjectsGroup(PumpProgram parentProgram, string elemTagName, Type elemType)
        {
            this.parentProgram = parentProgram;
            // �������� ������ ���� ��������� ProcessObject
            if (!elemType.IsSubclassOf(typeof(ProcessObject)))
                throw new Exception(String.Format("����� '{0}' �� �������� ����������� '{1}'",
                    elemType.FullName, typeof(ProcessObject).FullName));
            this.elemTagName = elemTagName;
            this.elemType = elemType;
        }

        /// <summary>
        /// �������� ���������� ������ ��������
        /// </summary>
        /// <returns>����� �������</returns>
        private ProcessObject CreateNewElem()
        {
            return (ProcessObject)Activator.CreateInstance(elemType, this.parentProgram);
        }

        /// <summary>
        /// ���������� ������ �������� (��� ����������� �������������)
        /// </summary>
        /// <param name="elem">����� �������</param>
        protected abstract void AddNewElemInternal(ProcessObject elem);

        /// <summary>
        /// ���������� ������ ��������
        /// </summary>
        /// <param name="elem">����� �������</param>
        public void AddNew(ProcessObject elem)
        {
            AddNewElemInternal(elem);
        }

        /// <summary>
        /// ��������� ������ ��������� �� XML
        /// </summary>
        /// <param name="parentNode">���� XML</param>
        public virtual void LoadFromXml(XmlNode parentNode)
        {
            // �������� ��� �������� ���� � �������� ��������� ����
            XmlNodeList childs = parentNode.SelectNodes(elemTagName);
            // ��������������� ��������� �� �� ���������� ���������
            foreach (XmlNode node in childs)
            {
                ProcessObject obj = CreateNewElem();
                obj.LoadFromXml(node);
                AddNewElemInternal(obj);
            }
        }

        /// <summary>
        /// �������� ���������� (��� ������������� � ������ foreach � �.�.)
        /// ������ ���� ���������� � ��������
        /// </summary>
        /// <returns>����������</returns>
        public abstract IEnumerator GetEnumerator();

        /// <summary>
        /// ��������� ������������ ��������� ������ � ���� �� ���������
        /// </summary>
        /// <param name="pumpModule">������������ ������ ������� (��� ������ ����� �������, ������� � ����� � �.�.)</param>
        /// <returns>������ � ���������� �� ������, String.Empty ���� ������ ���</returns>
        public virtual string Validate()
        {
            // ����������� ����������
            IEnumerator en = this.GetEnumerator();
            // ������� ���������� �������� �� �������
            StringBuilder sb = new StringBuilder();
            // ��������������� ���������� ��� �������� ��������
            en.Reset();
            while (en.MoveNext())
            {
                ProcessObject po = (ProcessObject)en.Current;
                // .. � ������������ �� ���������
                string currErr = po.Validate();
                if (!String.IsNullOrEmpty(currErr))
                {
                    sb.AppendLine(currErr);
                    sb.AppendLine(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// ������� ��������.
        /// ������ ���� �������������� � ��������
        /// </summary>
        public abstract void Clear();

        public abstract int Count { get; }
    }

    /// <summary>
    /// ��������� �������� <���������>, <ProcessObject>
    /// </summary>
    public abstract class ObjectsCollection : ObjectsGroup
    {
        /// <summary>
        /// ���������� ���������
        /// </summary>
        private Dictionary<string, ProcessObjectWithSynonym> dictionary = new Dictionary<string, ProcessObjectWithSynonym>();

        protected ObjectsCollection(PumpProgram parentProgram, string elemTagName, Type elemType)
            : base(parentProgram, elemTagName, elemType)
        {
        }

        protected override void AddNewElemInternal(ProcessObject elem)
        {
            ProcessObjectWithSynonym newElem = (ProcessObjectWithSynonym)elem;
            dictionary.Add(newElem.synonym, newElem);
        }

        public override IEnumerator GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        public override void Clear()
        {
            foreach (ProcessObject po in dictionary.Values)
            {
                po.Clear();
            }
            dictionary.Clear();
        }

        /// <summary>
        /// �������� ������ �� ��������. ����������� ����������
        /// </summary>
        /// <param name="synonym">�������</param>
        /// <returns>������</returns>
        public ProcessObject this[string synonym]
        {
            get
            {
                return dictionary[synonym];
            }
        }

        /// <summary>
        /// �������� ������ �� �����. ������������ �����, �������� ����� ������� - ����� �������
        /// </summary>
        /// <param name="name">��� �������</param>
        /// <returns>������</returns>
        public ProcessObject ObjectByName(string name)
        {
            foreach (ProcessObject po in this)
            {
                if (String.Compare(name, po.name, true) == 0)
                    return po;
            }
            return null;
        }

        public override int Count
        {
            get { return dictionary.Count; }
        }
    }

    /// <summary>
    /// ������ ��������
    /// </summary>
    public abstract class ObjectsList : ObjectsGroup
    {
        /// <summary>
        /// ���������� ������
        /// </summary>
        private List<ProcessObject> list = new List<ProcessObject>();

        protected ObjectsList(PumpProgram parentProgram, string elemTagName, Type elemType)
            : base(parentProgram, elemTagName, elemType)
        {
        }

        protected override void AddNewElemInternal(ProcessObject elem)
        {
            list.Add(elem);
        }

        public override IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public override void Clear()
        {
            foreach (ProcessObject po in list)
            {
                po.Clear();
            }
            list.Clear();
        }

        public ProcessObject this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public override int Count
        {
            get {return list.Count; }
        }

    }
    #endregion

}