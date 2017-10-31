using System;
using System.Collections.Generic;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
	/// ������� ����� ��� ��������������� ������ � ������������
	/// </summary>
    internal abstract partial class Classifier : DataSourceDividedClass, IClassifier
    {
        #region ����

        // ���� ��� �������� ��������
        private DimensionLevelCollection levels;

        #endregion ����

        /// <summary>
        /// ����������� �������� ������ ��������������� ������ � ������������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic"></param>
        /// <param name="name">���������� ������������ �������</param>
        /// <param name="classType">����� �������</param>
        /// <param name="subClassType">�������� �������</param>
        /// <param name="supportDivide"></param>
        /// <param name="state"></param>
        /// <param name="scriptingEngine"></param>
        public Classifier(string key, ServerSideObject owner, string semantic, string name, ClassTypes classType, SubClassTypes subClassType, bool supportDivide, ServerSideObjectStates state, ScriptingEngine.ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, semantic, name, classType, subClassType, supportDivide, state, scriptingEngine)
		{
            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            try
            {
                SessionContext.SetSystemContext();

                // � ���� ��������������� ������ �������������� ���� ������������ ��� ������
                Attributes.Add(DataAttribute.SystemRowType);

                levels = new DimensionLevelCollection(this, state);
            }
            finally
            {
                LogicalCallContextData.SetContext(callerContext);
            }
        }

        #region ServerSideObject

        /// <summary>
        /// ���������� ��������� ������� � ������� ������ �������� ������� ������������
        /// </summary>
        /// <returns>��������� �������</returns>
        protected new Classifier Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (Classifier)GetInstance(); }
        }

        /// <summary>
        /// ���������� ��������� ������� � ������� ������ �������� ������� ������������, ��� ��������� �������� �������
        /// </summary>
        /// <returns>��������� �������</returns>
        protected new Classifier SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (Classifier)CloneObject;
                else
                    return this;
            }
        }

        /// <summary>
        /// ������� ����� ������, ���������� ������ �������� ����������. 
        /// </summary>
        /// <returns>����� ������, ���������� ������ �������� ����������.</returns>
        public override Object Clone()
        {
            Classifier clon = (Classifier)base.Clone();
            clon.levels = (DimensionLevelCollection)levels.Clone();
            return clon;
        }

        /// <summary>
        /// ������� ���������� � �������
        /// </summary>
        public override void Unlock()
        {
            levels.Unlock();
            base.Unlock();
        }

        /// <summary>
        /// ��������� ���������� ������� �� ������� ��� �������������
        /// </summary>
        public override ServerSideObjectStates State
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return base.State; }
            set
            {
                if (base.State != value)
                {
                    base.State = value;
                    levels.State = value;
                }
            }
        }

        #endregion ServerSideObject

        /// <summary>
        /// �������������� ��������� ��������� ������� �� ���������� �� XML �������� 
        /// </summary>
        /// <param name="doc">�������� � XML ����������</param>
        /// <param name="atagElementName">������������ ���� � ����������� �������</param>
        protected override void InitializeAttributes(XmlDocument doc, string atagElementName)
        {
            bool DividePresent = false;
            XmlNodeList xmlAttributes = doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/Attributes/Attribute", atagElementName));
            foreach (XmlNode xmlAttribute in xmlAttributes)
            {
                EntityDataAttribute attr = EntityDataAttribute.CreateAttribute(this, xmlAttribute, this.State);
                Attributes.Add(attr);

                if (attr.Divide != String.Empty)
                {
                    if (DividePresent)
                        throw new Exception("�������� divide �� ����� ���� ������� ������ � ���������� ���������.");

                    List<DataAttribute> list = InitializeDivideAttributes(this, attr);
                    foreach (DataAttribute item in list)
                    {
                        if (DataAttributeCollection.GetAttributeByKeyName(Attributes, item.Name, item.Name) == null)
                            Attributes.Add(item);
                    }
                    DividePresent = true;
                }
            }
        }

        internal static List<DataAttribute> InitializeDivideAttributes(Entity entity, EntityDataAttribute attr)
	    {
            List<DataAttribute> list = new List<DataAttribute>();

	        string[] divide = attr.Divide.Split('.');
	        if (divide.Length > 11)
	            throw new Exception("���������� ��������� � �������� divide �� ������ ��������� 11.");
	        for (int i = 1; i <= divide.Length; i++)
	        {
                DataAttribute divideAttr = EntityDataAttribute.CreateAttribute(Guid.Empty.ToString(), "Code" + i, entity, AttributeClass.Regular, entity.State);
	            divideAttr.Caption = "��� " + i;
	            divideAttr.Class = DataAttributeClassTypes.Fixed;
	            divideAttr.Description = "������� " + i + "-� ������������� ����";
	            divideAttr.Type = DataAttributeTypes.dtInteger;
	            divideAttr.Size = Math.Abs(Convert.ToInt32(divide[i - 1]));
	            divideAttr.IsNullable = true;
	            divideAttr.DefaultValue = 0;
	            divideAttr.Mask = String.Empty.PadLeft(divideAttr.Size, '#');
	            divideAttr.Kind = DataAttributeKindTypes.Serviced;
	            divideAttr.IsReadOnly = true;
                list.Add(divideAttr);
	        }
            return list;
	    }

	    private void InitializeFixedRows(XmlNode xmlNode)
        {
            if ((xmlNode == null) || (String.IsNullOrEmpty(xmlNode.InnerXml)))
                return;

            if (xmlNode.SelectSingleNode("File") != null)
            {
                XmlNode xmlFileNode = xmlNode.SelectSingleNode("File/@privatePath");
                XmlNode xmlValuesNode = GetFixedRowsNodeListFromFile(xmlFileNode.Value);
                xmlFixedRowsData = xmlValuesNode.OuterXml;
            }
            else
                xmlFixedRowsData = xmlNode.InnerXml;
        }

        internal override XmlDocument Initialize()
        {
            XmlDocument doc = base.Initialize();

            levels.InitializeHierarchy(doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/Hierarchy", tagElementName)));

            InitializeFixedRows(doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/Data", tagElementName)));

            if (Levels.HierarchyType == HierarchyType.ParentChild)
            {
                if (!Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                    Attributes.Add(DataAttribute.SystemParentID);
            }

            return doc;
        }

        /// <summary>
        /// ����� ���������� ����� ������������� ���� �������� �����
        /// </summary>
        internal override XmlDocument PostInitialize()
        {
            XmlDocument doc = base.PostInitialize();

            if (ID > 0)
            {
                Database db = null;
                try
                {
                    db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                    List<string> script = new List<string>();
#if false //������������ �������������
                    script = ((ScriptingEngine.Classes.ClassifierEntityScriptingEngine)this._scriptingEngine).CreateDependentScripts(this, DataAttribute.SystemDummy);
#endif
                    if (ClassType == ClassTypes.clsBridgeClassifier || ClassType == ClassTypes.clsDataClassifier)
                    {
						// ���� ��� �������� ���������� ��������, ��� ��������� ����������
                        if (!_scriptingEngine.ExistsObject(ClassifierEntityScriptingEngine.GetCascadeDeleteTriggerName(this), ObjectTypes.Trigger) ||
                                                    SchemeDWH.Instance.DatabaseVersion == "2.4.1.0" && SchemeClass.Instance.NeedUpdateScheme)
                        {
                            // ������� ������� ���������� ��������.
                            script.AddRange(
								((ClassifierEntityScriptingEngine)_scriptingEngine).GetCascadeDeleteTrigerScript(this));
                        }

						// ���������� �������� ��� ���� ParentID. 
						// ��������� � ������ ���� ������ 2.6.0.0 � 2.5.0.10
						if (SchemeClass.Instance.NeedUpdateScheme && (
							SchemeDWH.Instance.DatabaseVersion == "2.6.0.0" ||
							SchemeDWH.Instance.DatabaseVersion == "2.5.0.12"))
						{
							script.AddRange(
								((ClassifierEntityScriptingEngine) _scriptingEngine).CreateIfNotExistsParentIdIndexScript(this));
						}
                    }
                    db.RunScript(script.ToArray(), false);
                }
                catch (Exception e)
                {
                    Trace.TraceError("������ ��� �������� � ���� ������ ��������� �������� ��� {0}: {1}", FullName, e.Message);
                }
                finally
                {
                    if (db != null) 
                        db.Dispose();
                }
            }
            
            return doc;
        }

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            MajorObjectModificationItem root = (MajorObjectModificationItem)base.GetChanges(toObject);

            Classifier toClassifier = (Classifier)toObject;

            ModificationItem hierarchyModificationItem = (ModificationItem)this.Levels.GetChanges(toClassifier.Levels);
            if (hierarchyModificationItem != null)
            {
                hierarchyModificationItem.Parent = root;
                root.Items.Add(hierarchyModificationItem.Key, hierarchyModificationItem);
            }

            ModificationItem miFixedRows = GetChangesFixedRows(toClassifier);
            if (miFixedRows != null)
            {
                miFixedRows.Parent = root;
                root.Items.Add(miFixedRows.Key, miFixedRows);
            }

            return root;
        }

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);
            
            //
            // ��������
            //
            ((DimensionLevelCollection)Levels).HierarchySave2Xml(node);

            //
            // ������������� ��������
            //
            if (!String.IsNullOrEmpty(xmlFixedRowsData))
            {
                XmlNode xmlData = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                xmlData.InnerXml = xmlFixedRowsData;
                node.AppendChild(xmlData);
            }

            //���������� �����
            if ((UniqueKeys != null) && (UniqueKeys.Count > 0))
            {
                XmlNode uniqueKeysNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "UniqueKeyList", null);
                ((UniqueKeyCollection)UniqueKeys).Save2Xml(uniqueKeysNode);
                node.AppendChild(uniqueKeysNode);
            }
        }

        internal override void Save2XmlDocumentation(XmlNode node)
        {
            base.Save2XmlDocumentation(node);

            //
            // ��������
            //
            ((DimensionLevelCollection)Levels).HierarchySave2Xml(node);

            //
            // ������������� ��������
            //
            if (!String.IsNullOrEmpty(xmlFixedRowsData))
            {
                XmlNode xmlData = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                xmlData.InnerXml = xmlFixedRowsData;
                node.AppendChild(xmlData);
            }
        }

        protected override void OnAfterUpdate()
        {
            try
            {
                SchemeClass.Instance.Processor.InvalidateDimension(
                    this,
                    "Krista.FM.Server.Scheme.Classes.Classifier",
                    Krista.FM.Server.ProcessorLibrary.InvalidateReason.ClassifierChanged,
                    OlapName);
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "��� ��������� �������� ������������� ������� ��� ������� \"{0}\" ��������� ������: {1}", 
                    this.FullName, e.Message);
            }
        }

        /// <summary>
        /// ��������� �������
        /// </summary>
        public IDimensionLevelCollection Levels
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return Instance.levels; }
        }

        /// <summary>
        /// ���������� ������ ���������� �� ������� ����������� �������������
        /// </summary>
        /// <returns>Key - ID ��������� ������; Value - �������� ���������</returns>
        public virtual Dictionary<int, string> GetDataSourcesNames()
        {
            if (Attributes.ContainsKey(DataAttribute.SourceIDColumnName))
                return ((DataSourcesManager.DataSourceManager)SchemeClass.Instance.DataSourceManager).GetDataSourcesNames(FullDBName, "ID <> -1 and RowType = 0");
            else
                throw new Exception(String.Format("���������� �������� ������ ���������� � ������� � ������� ��� �������� {0}.", DataAttribute.SourceIDColumnName));
        }

        /// <summary>
        /// ��������� �������� ��� ���������� �� ��� ���������
        /// </summary>
        /// <returns>���������� ������������ �������</returns>
        public virtual int FormCubesHierarchy()
        {
            throw new NotImplementedException("������� FormCubesHierarchy �� �����������.");
        }

        public override Dictionary<string, string> GetSQLMetadataDictionary()
        {
            Dictionary<string, string> sqlMetadata = base.GetSQLMetadataDictionary();

            sqlMetadata.Add(SQLMetadataConstants.ParentIDForeignKeyConstraint, ClassifierEntityScriptingEngine.ParentIDForeignKeyConstraintName(FullDBName));

            return sqlMetadata;
        }

        #region ������ � ���������

        /// <summary>
        /// ������������� ������� ��������
        /// </summary>
        protected virtual void InitializeDefaultRegularHierarchy()
        {
            Levels.Clear();

            Levels.HierarchyType = HierarchyType.Regular;

            RegularLevel allLevel = new RegularLevel(Guid.Empty.ToString(), this);
            allLevel.Name = "���";
            allLevel.LevelType = LevelTypes.All;
            Levels.Add(allLevel.Name, allLevel);

            RegularLevel level = new RegularLevel(Guid.NewGuid().ToString(), this);
            level.Name = this.SemanticCaption;
            level.LevelType = LevelTypes.Regular;
            level.MemberKey = Attributes[DataAttribute.IDColumnName];
            level.MemberName = Attributes[DataAttribute.IDColumnName];
            Levels.Add(level.ObjectKey, level);

            if (Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                Attributes.Remove(DataAttribute.ParentIDColumnName);
        }

        /// <summary>
        /// ������������� ���������� ��������
        /// </summary>
        protected virtual void InitializeDefaultParentChildHierarchy()
        {
            Levels.HierarchyType = HierarchyType.ParentChild;
            Levels.Clear();

            // ������� All
            RegularLevel allLevel = new RegularLevel(Guid.Empty.ToString(), this);
            allLevel.Name = "���";
            allLevel.LevelType = LevelTypes.All;
            Levels.Add(allLevel.Name, allLevel);

            if (!Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                Attributes.Add(DataAttribute.SystemParentID);

            // ���������� �������
            ParentChildLevel level = new ParentChildLevel(Guid.NewGuid().ToString(), this);
            level.Name = this.SemanticCaption;
            level.LevelType = LevelTypes.Regular;
            level.MemberKey = Attributes[DataAttribute.IDColumnName];
            level.MemberName = Attributes[DataAttribute.IDColumnName];
            level.ParentKey = Attributes[DataAttribute.ParentIDColumnName];
            level.LevelNamingTemplate = "������� 1; ������� 2; ������� 3; ������� *";
            Levels.Add(level.ObjectKey, level);
        }

        /// <summary>
        /// ������������� �������� ��������������
        /// </summary>
        internal void InitializeDefaultHierarchy()
        {
            if (Levels.HierarchyType == HierarchyType.Regular)
                InitializeDefaultRegularHierarchy();
            else
                InitializeDefaultParentChildHierarchy();
        }
        
        #endregion ������ � ���������
    }
}
