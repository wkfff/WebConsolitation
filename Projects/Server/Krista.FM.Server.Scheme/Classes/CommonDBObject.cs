using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// ������� ������ ��� �������� ��������� � ����������.
    /// </summary>
    internal abstract class CommonDBObject : CommonObject, ICommonDBObject
    {
        #region ����
        
        /// <summary>
        /// ID ������� � ���� ������ 
        /// </summary>
        private int _ID;

        /// <summary>
        /// ������� ��������� ��������� �������
        /// </summary>
        protected DBObjectStateTypes dbObjectState = DBObjectStateTypes.Unknown;

        /// <summary>
        /// �������� ������������ �������
        /// </summary>
        private string developerDescription = String.Empty;

        /// <summary>
        /// ���� true, �� ������ � ������ ������ ������ ���� ���������, 
        /// �.�. ���������� ��������� ��������� ������� 
        /// � �����-���� �������� � �������� ���������.
        /// </summary>
        private bool inUpdating = false;

        /// <summary>
        /// ������ ��� ������ �� ���������� ���� ������.
        /// </summary>
        protected ScriptingEngineAbstraction _scriptingEngine;

        #endregion ����

        #region �����������

        /// <summary>
        /// ������������� �������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        public CommonDBObject(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : this(key, owner, semantic, name, state, SchemeClass.ScriptingEngineFactory.NullScriptingEngine)
        {
        }

        /// <summary>
        /// ������� ������ ��� �������� ��������� � ����������.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic">���������</param>
        /// <param name="name">������������</param>
        /// <param name="state"></param>
        /// <param name="scriptingEngine"></param>
        public CommonDBObject(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state, ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, name, state)
        {
        	this.semantic = semantic;
            _scriptingEngine = scriptingEngine;
        }

        #endregion �����������

        #region ��������������
/*
        /// <summary>
        /// �������� ������ � ��������� ��������������.
        /// </summary>
        /// <remarks>
        /// ��������� ����� ������� ��������� ������ �������� ������������, � ������� � 
        /// ������������ ��� ���������� ��������� �������������� ������� �������������.
        /// </remarks>
        /// <returns>����� �������</returns>
        internal ICommonDBObject BeginEdit()
        {
            if (IsClone)
                return this;
            else
            {
                return Lock() as ICommonDBObject;
            }
        }
*/
        /// <summary>
        /// ���������� �������������� � ����������� ���� ���������
        /// </summary>
        public virtual void EndEdit()
        {
            Unlock();
        }

        /// <summary>
        /// ���������� �������������� � ����������� ���� ���������
        /// </summary>
        /// <param name="comments">����������� � ��������� ����������</param>
        public virtual void EndEdit(string comments)
        {
            Unlock();
        }

        /// <summary>
        /// ������ �������������� �������
        /// </summary>
        public virtual void CancelEdit()
        {
            Unlock();
        }
        #endregion ��������������

        #region ICommonDBObject Members

        /// <summary>
        /// ��������� � SQL ����������� (����� �����������, ��������) ������� 
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetSQLMetadataDictionary()
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// ���������� SQL-����������� �������.
        /// </summary>
        /// <returns>SQL-����������� �������.</returns>
        public virtual List<string> GetSQLDefinitionScript()
        {
            return new List<string>();
        }

        #endregion

        #region �������������, �������� � ����������

        /// <summary>
        /// ����������� ����� ������������� �������, ������ ���� ������������� � ��������
        /// </summary>
        /// <returns>Xml-�������� ���������� ������������ �������</returns>
        internal virtual XmlDocument Initialize()
        {
            if (state == ServerSideObjectStates.New && ObjectKey == Guid.Empty.ToString())
            {
                ObjectKey = Guid.NewGuid().ToString();
            }

            return Validator.LoadDocument(Configuration);
        }

        /// <summary>
        /// ����� ���������� ����� ������������� ���� �������� �����.
        /// </summary>
        internal virtual XmlDocument PostInitialize()
        {
            return Validator.LoadDocument(Configuration);
        }

        /// <summary>
        /// ������������� �������� ������������
        /// </summary>
        /// <param name="doc">�������� � XML ����������</param>
        /// <param name="tagElementName">������������ ���� � ����������� �������</param>
        protected void InitializeDeveloperDescription(XmlDocument doc, string tagElementName)
        {
            XmlNode xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/DeveloperDescription", tagElementName));
            if (xmlNode != null)
            {
                DeveloperDescription = xmlNode.InnerText;
            }
        }

        internal void SetParent(CommonDBObject parent)
        {
            Owner = parent;
        }

        /// <summary>
        /// �������� ���������� � ���� ������ � �����
        /// </summary>
        internal virtual void Create(ModificationContext context)
        {
        }

        internal virtual void Drop(ModificationContext context)
        {
        }

        /// <summary>
        /// ���������� ���������. �������� ������� ������ � ���� ������� toObject
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toObject">������ � ���� �������� ����� �������� ������� ������</param>
        public virtual void Update(ModificationContext context, IModifiable toObject)
        {
            //Trace.WriteLine(String.Format("���������� ������� {0}", FullName));
            CommonDBObject toCommonDBObject = (CommonDBObject)toObject;

            // � ������� �������� ������������ �����
            if (Parent != null)
            {
                if (Parent.ID != toCommonDBObject.Parent.ID)
                {
                    //Trace.WriteLine(String.Format("� ������� \"{0}\" �������� ������������ ����� c \"{1}\" �� \"{2}\"", FullName, parent.FullName, toObject.Parent.FullName));
                    //Debugger.Break();
                }
            }

            // ���������
            if (Semantic != toCommonDBObject.Semantic)
            {
                throw new Exception(String.Format("���������� ��� � ������� \"{0}\" �� ����� ���� ��������.", FullName));
            }

            // ���������� ���
            if (Name != toCommonDBObject.Name)
            {
                throw new Exception(String.Format("���������� ��� � ������� \"{0}\" �� ����� ���� ��������.", FullName));
            }
            
            // ������� ������������
            if (Caption != toCommonDBObject.Caption)
            {
                Trace.WriteLine(String.Format("� ������� \"{0}\" ���������� ������� ������������ c \"{1}\" �� \"{2}\"", FullName, Caption, toCommonDBObject.Caption));
                Caption = toCommonDBObject.Caption;
            }

            // ��������
            if (Description != toCommonDBObject.Description)
            {
                Trace.WriteLine(String.Format("� ������� \"{0}\" ���������� �������� c \"{1}\" �� \"{2}\"", FullName, Description, toCommonDBObject.Description));
                Description = toCommonDBObject.Description;
            }
        }

        /// <summary>
        /// ���������� XML-������������ � ���� ������
        /// </summary>
        internal abstract void SaveConfigurationToDatabase(ModificationContext context);

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            if (!String.IsNullOrEmpty(DeveloperDescription))
            {
                XmlNode devDescr = XmlHelper.AddChildNode(node, "DeveloperDescription", String.Empty, null);
                XmlHelper.AppendCDataSection(devDescr, DeveloperDescription);
            }
        }

        /// <summary>
        /// ���������� XML-������������ ��� ��������� �������
        /// </summary>
        internal virtual void Save2XmlDocumentation(XmlNode node)
        {
            Save2Xml(node);
        }

        #endregion �������������, �������� � ����������

        #region ServerSideObject

        /// <summary>
        /// ���������� ��������� ������� � ������� ������ �������� ������� ������������
        /// </summary>
        /// <returns>��������� �������</returns>
        protected CommonDBObject Instance
        {
            [DebuggerStepThrough]
            get { return (CommonDBObject)GetInstance(); }
        }

        /// <summary>
        /// ���������� ��������� ������� � ������� ������ �������� ������� ������������, ��� ��������� �������� �������
        /// </summary>
        /// <returns>��������� �������</returns>
        protected CommonDBObject SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (CommonDBObject)CloneObject;
                else
                    return this;
            }
        }

        #endregion ServerSideObject

        #region IMajorModifiable Members

        /// <summary>
        /// ��������� ������ ������� (�������� ���������) �������� �������������� ������� �� ��������� ������� � ���� ������
        /// </summary>
        /// <returns>������ ������� (�������� ���������)</returns>
        public virtual IModificationItem GetChanges()
        {
            if (CloneObject != null)
            {
                LogicalCallContextData userContext = LogicalCallContextData.GetContext();
                try
                {
                    SessionContext.SetSystemContext();
                    return GetChanges(CloneObject as IModifiable);
                }
                finally
                {
                    LogicalCallContextData.SetContext(userContext);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// ��������� ������ ������� (�������� ���������) �������� ������� �� toObject
        /// </summary>
        /// <param name="toObject">������ � ������� ����� ������������� ���������</param>
        /// <returns>������ ������� (�������� ���������)</returns>
        public override IModificationItem GetChanges(IModifiable toObject)
        {
            UpdateMajorObjectModificationItem root = new UpdateMajorObjectModificationItem(ModificationTypes.Modify, this.FullName, this, toObject, null);
            
            IModificationItem keyModificationItem = base.GetChanges(toObject);
            if (keyModificationItem != null)
            {
                root.Items.Add(keyModificationItem.Key, keyModificationItem);
                ((ModificationItem) keyModificationItem).Parent = root;
            }

            CommonDBObject toCommonDBObject = (CommonDBObject)toObject;

            if (this.DeveloperDescription != toCommonDBObject.DeveloperDescription)
            {
                ModificationItem item = new PropertyModificationItem("DeveloperDescription", this.DeveloperDescription, toCommonDBObject.DeveloperDescription, root);
                root.Items.Add(item.Key, item);
            }

            return root;
        }

        #endregion

        #region ��������

        /// <summary>
        /// ������������ ������
        /// </summary>
        public Package Parent
        {
            [DebuggerStepThrough]
            get { return Owner is Package ? (Package)Owner : null; }
        }

        /// <summary>
        /// ������������ �����
        /// </summary>
        // TODO: Vtopku 
        public IPackage ParentPackage
        {
            get { return Parent; }
        }

        /// <summary>
        /// ������������� ������� � ���� ������
        /// </summary>
        public int ID
        {
            [DebuggerStepThrough]
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// ������� ��������� ��������� �������
        /// </summary>
        public DBObjectStateTypes DbObjectState
        {
            [DebuggerStepThrough]
            get { return dbObjectState; }
            set 
            { 
                dbObjectState = value;
                switch (dbObjectState)
                {
                    case DBObjectStateTypes.InDatabase: State = ServerSideObjectStates.Consistent; break;
                    case DBObjectStateTypes.New: State = ServerSideObjectStates.New; break;
                    case DBObjectStateTypes.Changed: State = ServerSideObjectStates.Changed; break;
                    case DBObjectStateTypes.ForDelete: State = ServerSideObjectStates.Deleted; break;
                    case DBObjectStateTypes.Unknown: State = ServerSideObjectStates.Deleted; break;
                }
            }
        }

        /// <summary>
        /// �������� ������������ �������
        /// </summary>
        public string DeveloperDescription
        {
            [DebuggerStepThrough]
            get { return Instance.developerDescription; }
            set { SetInstance.developerDescription = value; }
        }

        /// <summary>
        /// ���� tru, �� ������ � ������ ������ ������ ���� ���������, 
        /// �.�. ���������� ��������� ��������� ������� 
        /// � �����-���� ������� � �������� ���������.
        /// </summary>
        public bool InUpdating
        {
            [DebuggerStepThrough]
            get { return inUpdating; }
            set { inUpdating = value; }
        }

        /// <summary>
        /// ������������� ���������� ������������
        /// </summary>
        /// <param name="oldValue">������ ������������</param>
        /// <param name="value">����� ������������</param>
        protected virtual void SetFullName(string oldValue, string value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void CheckFullName()
        {
        }

        /// <summary>
        /// ���������� ���� �������
        /// </summary>
        public string Key
        {
            get { return String.Format("{0}{1}:{2}", Owner != null && Owner is ICommonDBObject ? ((ICommonDBObject)Owner).Key + "::" : String.Empty, this.GetType().Name, GetKey(ObjectKey, FullName)); }
        }

		// ��� ������������� ��������������
		private string semantic;
		
		/// <summary>
        /// ��� ������������� ��������������
        /// </summary>
        public virtual string Semantic
        {
            [DebuggerStepThrough]
			get { return Instance.semantic; }
            set
            {
				Scheme.ScriptingEngine.ScriptingEngineHelper.CheckDBName(value);

                if (State != ServerSideObjectStates.New && !Authentication.IsSystemRole())
                    throw new Exception("��������� ����� �������� ������ � ����� ��������� ��������.");

				string oldValue = FullName;
				SetInstance.semantic = value;
                try
                {
                    //SetFullName(oldValue, FullName);
                }
                catch (Exception e)
                {
					SetInstance.semantic = oldValue;
                    throw new Exception(e.Message, e);
                }
            }
        }

		/// <summary>
		/// ��� ������������� ��������������
		/// </summary>
		public string SemanticCaption
		{
			get
			{
				try
				{
					string semanticCaption;
					if (SchemeClass.Instance.Semantics.TryGetValue(Semantic, out semanticCaption))
						return semanticCaption;
					else
						return Semantic;
				}
				catch
				{
					return Semantic;
				}
			}
		}

		/// <summary>
        /// ���������� ���������� ��� �������
        /// </summary>
        public override string Name
        {
            [DebuggerStepThrough]
            get { return GetterMustUseClone() ? ((CommonObject)CloneObject).Name : base.Name; }
            set
            {
                string oldName = Name;

                if (State != ServerSideObjectStates.New && !Authentication.IsSystemRole())
                    throw new Exception("������������ ����� �������� ������ � ����� ��������� ��������.");

                if (SetterMustUseClone())
                    ((CommonObject)CloneObject).Name = value;
                else
                    base.Name = value;

                try
                {
                    CheckFullName();
                }
                catch (Exception e)
                {
                    if (SetterMustUseClone())
                        ((CommonObject)CloneObject).Name = oldName;
                    else
                        base.Name = oldName;
                    throw new Exception(e.Message, e);
                }
            }
        }        

        /// <summary>
        /// ������� ������������ ������� ��������� � ����������
        /// </summary>
        public override string Caption
        {
            [DebuggerStepThrough]
            get { return GetterMustUseClone() ? ((CommonObject)CloneObject).Caption : base.Caption; }
            set
            {
                if (SetterMustUseClone())
                    ((CommonObject)CloneObject).Caption = value;
                else
                    base.Caption = value;
            }
        }

        /// <summary>
		/// ��������� �������� ������� ��������� � ����������
		/// </summary>
		public override string Description
        {
            [DebuggerStepThrough]
            get { return GetterMustUseClone() ? ((CommonObject)CloneObject).Description : base.Description; }
            set
            {
                if (SetterMustUseClone())
                    ((CommonObject)CloneObject).Description = value;
                else
                    base.Description = value;
            }
        }

        /// <summary>
        /// ���������� ������
        /// </summary>
        internal ScriptingEngineAbstraction ScriptingEngine
        {
            get { return _scriptingEngine; }
        }

        #endregion ��������

        #region ������

        /// <summary>
        /// ��������� ����� �� �������� ������� ��� �������� ������������
        /// </summary>
        /// <returns>true - ���� � ������������ ���� ����� �� ��������</returns>
        public abstract bool CurrentUserCanViewThisObject();

        #endregion ������
    }
}
