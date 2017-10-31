using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
    /// <summary>
    /// ��� ����������� ����������
    /// </summary>
    internal class DataKind : ServerSideObject, IDataKind, ICloneable
    {
        private DataSupplier dataSupplier;
        // ��� ����������� ����������
        private string code;
        // ������������ ����������� ����������
        private string name;
        // �������� ����������� ����������
        private string description;
        // ����� ��������� ����������
        private TakeMethodTypes takeMethod;
        // ��� ����������
        private ParamKindTypes paramKind;

        private DataKindCollection parent;

        /// <summary>
        /// ��� ����������� ����������
        /// </summary>
        internal DataKind(DataSupplier dataSupplier)
            : base(null)
        {
            this.dataSupplier = dataSupplier;
        }

        /// <summary>
        /// ��� ����������� ����������
        /// </summary>
        /// <param name="xmlDataKind">XML-���� � ����������� ��������</param>
        /// <param name="parent">������������ ������-��������� � ������� ��������� ������ ������</param>
        internal DataKind(XmlNode xmlDataKind, DataKindCollection parent, DataSupplier dataSupplier)
            : base(null)
        {
            this.dataSupplier = dataSupplier;
            this.parent = parent;
            this.code = xmlDataKind.Attributes["code"].Value;
            this.name = xmlDataKind.Attributes["name"].Value;
            if (xmlDataKind.Attributes["description"] != null) 
                this.description = xmlDataKind.Attributes["description"].Value;
            switch (xmlDataKind.Attributes["takeMethod"].Value)
            {
                case "������":
                    takeMethod = TakeMethodTypes.Import;
                    break;
                case "����":
                    takeMethod = TakeMethodTypes.Receipt;
                    break;
                case "����":
                    takeMethod = TakeMethodTypes.Input;
                    break;
                default:
                    throw new Exception("�������� ����� ��������� ���������� " + xmlDataKind.Attributes["takeMethod"].Value);
            }

            paramKind = DataSourceUtils.String2ParamKind(xmlDataKind.Attributes["paramKind"].Value);
        }

        /// <summary>
        /// ���������� ���� ����������� ����������
        /// </summary>
        internal void Update()
        {
            // ������� ��������� ������� ���� � ���������.
            if(parent != null)
                // �������� ���� � ������ ������� ���� � ���������.
                if(parent.ContainsKey(Code))
                    parent[Code] = this;
        }

        internal DataKindCollection Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                    throw new InvalidOperationException("������ �������� ��� ������, ������� ��������� � ���������");
                parent = value;
            }
        }

        #region ���������� IDataKind

        /// <summary>
        /// ��������� ������.
        /// </summary>
        public IDataSupplier Supplier
        {
            get { return dataSupplier; }
        }

        /// <summary>
        /// ��� ����������� ����������
        /// </summary>
        public string Code
        {
            get { return code; }
            set 
            {
                if (parent != null)
                    throw new InvalidOperationException("������ �������� ��� ����������� ������, ������� ��������� � ���������");
                code = value; 
            }
        }

        /// <summary>
        /// �������� ����������� ����������
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; Update(); }
        }
        /// <summary>
        /// ������������ ����������� ����������
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; Update(); }
        }

        /// <summary>
        /// ����� ��������� ����������
        /// </summary>
        public TakeMethodTypes TakeMethod
        {
            get { return takeMethod; }
            set
            {
                takeMethod = value; Update();
            }
        }

        /// <summary>
        /// ��� ����������
        /// </summary>
        public ParamKindTypes ParamKind
        {
            get { return paramKind; }
            set 
            {
                paramKind = value; Update();
            }
        }
       #endregion ���������� IDataKind

        #region ICloneable Members

        public override object Clone()
        {
            throw new Exception("The method DataKind.Clone() is not implemented.");
        }

        #endregion
    }
}
