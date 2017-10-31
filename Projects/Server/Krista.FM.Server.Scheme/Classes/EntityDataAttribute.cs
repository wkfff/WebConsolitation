using System;
using System.Collections.Generic;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

using Krista.FM.ServerLibrary;
using Krista.FM.Server.Scheme.Classes.PresentationLayer;


namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
	/// ������� ��������
	/// </summary>
    [Serializable]
    internal class EntityDataAttribute : DataAttribute, IModifiable
    {
        private static AttributeScriptingEngine _scriptingEngine;

        #region �������������

        /// <summary>
        /// ����������� ����
        /// </summary>
        static EntityDataAttribute()
        {
            // ��������� ���� ��� ���� ������
            systemDummy = new EntityDataAttribute(Guid.Empty.ToString(), "Dummy", null, ServerSideObjectStates.Consistent);
            systemDummy.Type = DataAttributeTypes.dtInteger;
            systemDummy.Size = 10;
            systemDummy.Caption = "Dummy";
            systemDummy.Description = "Dummy";
            systemDummy.Class = DataAttributeClassTypes.System;
            systemDummy.Kind = DataAttributeKindTypes.Sys;
            systemDummy.Visible = false;
            systemDummy.IsReadOnly = true;
            systemDummy.Position = -12;
            
            // ��������� ���� ��� ���� ������
            systemID = new EntityDataAttribute(Guid.Empty.ToString(), IDColumnName, null, ServerSideObjectStates.Consistent);
            systemID.Type = DataAttributeTypes.dtInteger;
            systemID.Size = 10;
            systemID.Caption = IDColumnName;
            systemID.Description = "���������� ����������� ��������� ����";
            systemID.Class = DataAttributeClassTypes.System;
            systemID.Kind = DataAttributeKindTypes.Serviced;
            systemID.Visible = true;
            systemID.IsReadOnly = true;
            systemID.Position = -11;

            // ������������� ������� ������
            systemPumpID = new EntityDataAttribute(Guid.Empty.ToString(), PumpIDColumnName, null, ServerSideObjectStates.Consistent);
            systemPumpID.Type = DataAttributeTypes.dtInteger;
            systemPumpID.Size = 10;
            systemPumpID.IsNullable = false;
            systemPumpID.Caption = "�������";
            systemPumpID.Description = "������������� �������";
            systemPumpID.Class = DataAttributeClassTypes.System;
            systemPumpID.Kind = DataAttributeKindTypes.Serviced;
            systemPumpID.IsReadOnly = true;
            systemPumpID.Position = -10;

            systemPumpIDDefault = systemPumpID.CloneStatic() as EntityDataAttribute;
            systemPumpIDDefault.DefaultValue = -1;

            // ������������� ��������� ������
            systemSourceID = new EntityDataAttribute(Guid.Empty.ToString(), SourceIDColumnName, null, ServerSideObjectStates.Consistent);
            systemSourceID.Type = DataAttributeTypes.dtInteger;
            systemSourceID.Size = 10;
            systemSourceID.IsNullable = false;
            systemSourceID.Caption = "��������";
            systemSourceID.Description = "������������� ���������";
            systemSourceID.Class = DataAttributeClassTypes.System;
            systemSourceID.Kind = DataAttributeKindTypes.Serviced;
            systemSourceID.IsReadOnly = true;
            systemSourceID.Position = -9;

            // ������������� ������
            systemTaskID = new EntityDataAttribute(Guid.Empty.ToString(), TaskIDColumnName, null, ServerSideObjectStates.Consistent);
            systemTaskID.Type = DataAttributeTypes.dtInteger;
            systemTaskID.Size = 10;
            systemTaskID.IsNullable = false;
            systemTaskID.Caption = "������";
            systemTaskID.Description = "������������� ������";
            systemTaskID.Class = DataAttributeClassTypes.System;
            systemTaskID.Kind = DataAttributeKindTypes.Serviced;
            systemTaskID.IsReadOnly = true;
            systemTaskID.Position = -8;

            systemTaskIDDefault = systemTaskID.CloneStatic() as EntityDataAttribute;
            systemTaskIDDefault.DefaultValue = -1;

            // ������������� ��������� ������ �� �������� ������ ������ ��� ������������ �������������
            fixedSourceID = new EntityDataAttribute(Guid.Empty.ToString(), SourceIDColumnName, null, ServerSideObjectStates.Consistent);
            fixedSourceID.Type = DataAttributeTypes.dtInteger;
            fixedSourceID.Size = 10;
            fixedSourceID.IsNullable = true;
            fixedSourceID.Caption = "��������";
            fixedSourceID.Description = "������������� ��������� ������ ������ ������";
            fixedSourceID.Class = DataAttributeClassTypes.Fixed;
            fixedSourceID.Kind = DataAttributeKindTypes.Serviced;
            fixedSourceID.IsReadOnly = true;
            fixedSourceID.Position = -7;

            // ������������ �����
            systemParentID = new EntityDataAttribute(Guid.Empty.ToString(), ParentIDColumnName, null, ServerSideObjectStates.Consistent);
            systemParentID.Type = DataAttributeTypes.dtInteger;
            systemParentID.Size = 10;
            systemParentID.IsNullable = true;
            systemParentID.Caption = "��������";
            systemParentID.Description = "������������ ���� ��� ���������� �������� � ����������";
            systemParentID.Class = DataAttributeClassTypes.Fixed;
            systemParentID.Kind = DataAttributeKindTypes.Regular;
            systemParentID.Visible = true;
            systemParentID.Position = -6;

            // ������������ ����� �������� � �����
            systemCubeParentID = new EntityDataAttribute(Guid.Empty.ToString(), CubeParentIDColumnName, null, ServerSideObjectStates.Consistent);
            systemCubeParentID.Type = DataAttributeTypes.dtInteger;
            systemCubeParentID.Size = 10;
            systemCubeParentID.IsNullable = true;
            systemCubeParentID.Caption = "�������� � �����";
            systemCubeParentID.Description = "������������ ���� ��� ���������� �������� � �����";
            systemCubeParentID.Class = DataAttributeClassTypes.Fixed;
            systemCubeParentID.Kind = DataAttributeKindTypes.Sys;
            systemCubeParentID.IsReadOnly = true;
            systemCubeParentID.Position = -5;

            // ������������� �������� ������ ��������� ������ ������ ������ ������
            systemSourceKey = new EntityDataAttribute(Guid.Empty.ToString(), SourceKeyColumnName, null, ServerSideObjectStates.Consistent);
            systemSourceKey.Type = DataAttributeTypes.dtInteger;
            systemSourceKey.Size = 10;
            systemSourceKey.IsNullable = true;
            systemSourceKey.Caption = "ID �������� ������";
            systemSourceKey.Description = "������������� �������� ������";
            systemSourceKey.Class = DataAttributeClassTypes.Fixed;
            systemSourceKey.Kind = DataAttributeKindTypes.Sys;
            systemSourceKey.IsReadOnly = true;
            systemSourceKey.Position = -4;

            // ��� ������ ���������������
            systemRowType = new EntityDataAttribute(Guid.Empty.ToString(), RowTypeColumnName, null, ServerSideObjectStates.Consistent);
            systemRowType.Type = DataAttributeTypes.dtInteger;
            systemRowType.Size = 10;
            systemRowType.DefaultValue = 0;
            systemRowType.Caption = "��� ������";
            systemRowType.Description = "��� ������. 0 - ������� ������ � �������; 1 - ������������� ������ '����������� ������'; 2 - ��������� ������ ��� �������� ������� ����������";
            systemRowType.Class = DataAttributeClassTypes.System;
            systemRowType.Kind = DataAttributeKindTypes.Sys;
            systemRowType.IsReadOnly = true;
            systemRowType.Position = -3;

            // ����-���� "�������� ������ ��������"
            systemVariantCompleted = new EntityDataAttribute(Guid.Empty.ToString(), VariantCompletedColumnName, null, ServerSideObjectStates.Consistent);
            systemVariantCompleted.Type = DataAttributeTypes.dtInteger;
            systemVariantCompleted.Size = 1;
            systemVariantCompleted.DefaultValue = 0;
            systemVariantCompleted.Caption = "�������� ������ ��������";
            systemVariantCompleted.Description = "����-���� \"�������� ������ ��������\". 0 � ������; 1 � ������";
            systemVariantCompleted.Class = DataAttributeClassTypes.System;
            systemVariantCompleted.Kind = DataAttributeKindTypes.Sys;
            systemVariantCompleted.Position = -2;

            // ����������� � ��������
            systemVariantComment = new EntityDataAttribute(Guid.Empty.ToString(), VariantCommentColumnName, null, ServerSideObjectStates.Consistent);
            systemVariantComment.Type = DataAttributeTypes.dtString;
            systemVariantComment.Size = 1000;
            systemVariantComment.Caption = "�����������";
            systemVariantComment.Description = "����������� � ��������";
            systemVariantComment.Class = DataAttributeClassTypes.System;
            systemVariantComment.Kind = DataAttributeKindTypes.Regular;
            systemVariantComment.Visible = true;

            // ���� � ����� �������� �� ����������� �����
            systemHashUK = new EntityDataAttribute(Guid.Empty.ToString(), HashUKColumnName, null, ServerSideObjectStates.Consistent);
            systemHashUK.Type = DataAttributeTypes.dtString;
            systemHashUK.Size = 32;
            systemHashUK.IsNullable = true;
            systemHashUK.Caption = "��� ����������� �����";
            systemHashUK.Description = "����������� ��� md5 �� ����� ����������� �����";
            systemHashUK.Class = DataAttributeClassTypes.System;
            systemHashUK.Kind = DataAttributeKindTypes.Sys;
            systemHashUK.IsReadOnly = true;
            systemHashUK.Position = -1;

        }

        public EntityDataAttribute(string key, string name, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, name, owner, state)
        {
        }

        public EntityDataAttribute(string key, ServerSideObject owner, XmlNode xmlAttribute, ServerSideObjectStates state)
            : base(key, owner, xmlAttribute, state)
        {
        }

        public static EntityDataAttribute CreateAttribute(string key, string name, ServerSideObject owner, AttributeClass attributeClass, ServerSideObjectStates state)
        {
            switch (attributeClass)
            {
                case AttributeClass.Regular: 
                    return new EntityDataAttribute(key, name, owner, state);
                case AttributeClass.Reference:
                    return new EntityAssociationAttribute(key, name, owner, state);
                case AttributeClass.Document:
                    return new DocumentAttribute(key, name, owner, state);
				case AttributeClass.RefAttribute:
					return new PresentationLayer.DocumentEntityAttribute(key, name, owner, state);
				default:
                    throw new ServerException("����������� ����� ��������.");
            }
        }

        public static EntityDataAttribute CreateAttribute(ServerSideObject owner, XmlNode xmlAttribute, ServerSideObjectStates state)
        {
            XmlNode keyNode = xmlAttribute.Attributes["objectKey"];
            string key = keyNode != null ? keyNode.Value : Guid.Empty.ToString();

            if (state == ServerSideObjectStates.New && key == Guid.Empty.ToString())
                key = Guid.NewGuid().ToString();

            XmlNode xmlNode = xmlAttribute.Attributes["isDocumentAttribute"];
            if (xmlNode != null && StrUtils.StringToBool(xmlNode.Value))
                return new DocumentAttribute(key, owner, xmlAttribute, state);
            else
                return new EntityDataAttribute(key, owner, xmlAttribute, state);
        }

        #endregion �������������


        internal static AttributeScriptingEngine ScriptingEngine
        {
            get 
            {
                if (_scriptingEngine == null)
                    _scriptingEngine = SchemeClass.ScriptingEngineFactory.AttributeScriptingEngine;
                return _scriptingEngine;
            }
        }

        #region DDL

        /// <summary>
        /// ���������� �������� �� ��������� � ���� ������.
        /// ���� �������� �� ��������� ���, �� ���������� ������ ������.
        /// </summary>
        internal string GetDefaultValue
        {
            get { return AttributeScriptingEngine.GetDefaultValue(type, defaultValue); }
        }

        /// <summary>
        /// ������������� �������� �� ��������� ��� ��������
        /// </summary>
        /// <param name="entity">������� � ������� ������ �������</param>
        /// <param name="script">������ SQL-��������</param>
        public virtual void UpdateSystemRowsSetDefaultValue(Entity entity, List<string> script)
        {
            if (entity is BridgeClassifier)
            {
				ScriptingEngine.UpdateTableSetDefaultValue(
					this, 
					entity.FullDBName, 
					GetStandardDefaultValue(this, "����������� ������"),
					String.Format("{0} IS NULL and {1} = -1", Name, IDColumnName), 
					script);
            }
        }

        /// <summary>
        /// ������������� �������� �� ��������� ��� ��������
        /// </summary>
        /// <param name="entity">������� � ������� ������ �������</param>
        /// <param name="script">������ SQL-��������</param>
        internal virtual void UpdateTableSetDefaultValue(Entity entity, List<string> script)
        {
        	ScriptingEngine.UpdateTableSetDefaultValue(this, entity.FullDBName, String.Format("{0} IS NULL", Name), script);
        }

        internal virtual List<string> AddScript(Entity entity, bool withNullClause, bool generateDependendScripts)
        {
            return ScriptingEngine.AddScript(this, entity, withNullClause, generateDependendScripts);
        }

        internal string[] AddScript(Entity entity)
        {
            return AddScript(entity, !this.IsNullable, false).ToArray();
        }

		internal virtual string[] ModifyScript(Entity entity, bool withTypeModification, bool withNullClause, bool generateDependendScripts)
        {
            return ScriptingEngine.ModifyScript(this, entity, withTypeModification, withNullClause, generateDependendScripts).ToArray();
        }

        internal virtual List<string> DropScript(Entity entity)
        {
            return ScriptingEngine.DropScript(this, entity);
        }

        private string ColumnScript()
        {
            return ScriptingEngine.ColumnScript(this, true);
        }

        #endregion DDL

        /// <summary>
        /// SQL-����������� ��������
        /// </summary>
        public override string SQLDefinition
        {
            get { return ColumnScript(); }
        }

        public override string ToString()
        {
            return ColumnScript() + " : " + base.ToString();
        }

        #region IModifiable Members

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            string modificationAttributeName = string.Empty;

            if (this is DocumentEntityAttribute)
                modificationAttributeName = String.Format("{0} ({1}:{2})", this.Caption, this.Name, this.ObjectKey);
            else
                modificationAttributeName = String.Format("{0} ({1})", this.Caption, this.Name);

            MinorObjectModificationItem root = new AttributeModificationItem(modificationAttributeName, this, toObject);

            EntityDataAttribute toAttribute = (EntityDataAttribute)toObject;

            IModificationItem keyModificationItem = base.GetChanges(toObject);
            if (keyModificationItem != null)
            {
                root.Items.Add(keyModificationItem.Key, keyModificationItem);
                ((ModificationItem)keyModificationItem).Parent = root;
            }

            if (this.Name != toAttribute.Name)
                throw new Exception(string.Format("� �������� {0} �������� Name ���������� ��� ���������.", Name));

            if (this.Type != toAttribute.Type)
            {
                ModificationItem item = new InapplicableModificationItem("Type",
                    this.Type, toAttribute.Type, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Caption != toAttribute.Caption)
            {
                ModificationItem item = new PropertyModificationItem("Caption", 
                    this.Caption, toAttribute.Caption, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Description != toAttribute.Description)
            {
                ModificationItem item = new PropertyModificationItem("Description", 
                    this.Description, toAttribute.Description, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Size != toAttribute.Size)
            {
                ModificationItem item = new PropertyModificationItem("Size",
                    this.Size, toAttribute.Size, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Scale != toAttribute.Scale)
            {
                ModificationItem item = new PropertyModificationItem("Scale",
                    this.Scale, toAttribute.Scale, root);
                root.Items.Add(item.Key, item);
            }

            if (this.IsNullable != toAttribute.IsNullable)
            {
                ModificationItem item = new PropertyModificationItem("IsNullable",
                    this.IsNullable, toAttribute.IsNullable, root);
                root.Items.Add(item.Key, item);
            }

            if (this.StringIdentifier != toAttribute.StringIdentifier)
            {
                ModificationItem item = new PropertyModificationItem("StringIdentifier",
                    this.StringIdentifier, toAttribute.StringIdentifier, root);
                root.Items.Add(item.Key, item);
            }

            if (Convert.ToString(this.DefaultValue) != Convert.ToString(toAttribute.DefaultValue))
            {
                ModificationItem item = new PropertyModificationItem("DefaultValue",
                    this.DefaultValue, toAttribute.DefaultValue, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Mask != toAttribute.Mask)
            {
                ModificationItem item = new PropertyModificationItem("Mask",
                    this.Mask, toAttribute.Mask, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Divide != toAttribute.Divide)
            {
                ModificationItem item = new PropertyModificationItem("Divide",
                    this.Divide, toAttribute.Divide, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Visible != toAttribute.Visible)
            {
                ModificationItem item = new PropertyModificationItem("Visible",
                    this.Visible, toAttribute.Visible, root);
                root.Items.Add(item.Key, item);
            }

            if (this.LookupType != toAttribute.LookupType)
            {
                ModificationItem item = new PropertyModificationItem("LookupType",
                    this.LookupType, toAttribute.LookupType, root);
                root.Items.Add(item.Key, item);
            }

            if (this.DeveloperDescription != toAttribute.DeveloperDescription)
            {
                ModificationItem item = new PropertyModificationItem("DeveloperDescription",
                    this.DeveloperDescription, toAttribute.DeveloperDescription, root);
                root.Items.Add(item.Key, item);
            }

            if (IsReadOnly != toAttribute.IsReadOnly)
            {
                ModificationItem item = new PropertyModificationItem("IsReadOnly", IsReadOnly, toAttribute.IsReadOnly,
                                                                     root);
                root.Items.Add(item.Key, item);
            }

            if (this.Position != toAttribute.Position)
            {
                ModificationItem item = new PropertyModificationItem("Position",
                    this.Position, toAttribute.Position, root);
                root.Items.Add(item.Key, item);
            }
            
            if (this.GroupTags != toAttribute.GroupTags)
            {
                ModificationItem item = new PropertyModificationItem("GroupTags",
                    this.GroupTags, toAttribute.GroupTags, root);
                root.Items.Add(item.Key, item);
            }

            return root;
        }

        #endregion

        /// <summary>
        /// ������� �������������� ��� ��������.
        /// </summary>
        /// <param name="entity">��������.</param>
        internal virtual List<string> UpdateSetNullable(Entity entity)
        {
            List<string> script = new List<string>();
            script.AddRange(this.ModifyScript(entity, false, true, true));
            return script;
        }

        /// <summary>
        /// ������������� �������������� ��� ��������.
        /// </summary>
        /// <param name="entity">��������.</param>
        internal virtual List<string> UpdateSetNotNull(Entity entity)
        {
            List<string> script = new List<string>();

            if (this.DefaultValue != null)
            {
                script.Add(String.Format("update {0} set {1} = {2} where {1} is null", entity.FullDBName, this.Name, this.GetDefaultValue));
            }
            else
            {
                // ��������� ���� �� � ������� NULL �������� ��� �������.
                // ���� ����, �� ��������, ���� ���, �� �������� �������.
                if (0 < Convert.ToInt32(SchemeClass.Instance.DDLDatabase.ExecQuery(String.Format(
                    "select count(*) from {0} where {1} is null", entity.FullDBName, this.Name),
                    QueryResultTypes.Scalar)))
                {
                    // ����� ����� ��������� �������� �� ��������� � ������������
                    throw new Exception(String.Format("� ������� ������� ������, � ������� ������� \"{0}\" ����� �������� NULL.\n��� ���������� �������� \"{0}\" ���������� ������� �������� �� ��������� ��� ��������� ������ ��������.", this.name));
                }
            }

            script.AddRange(this.ModifyScript(entity, false, true, true));

            return script;
        }

        /// <summary>
        /// ���������� �������� � ���������� ��������� � ���� ������
        /// </summary>
        /// <param name="entity">��������� � ������� ��������� �������</param>
        /// <param name="toDataAttribute">�������, �������� ������ ��������������� ���������� �������</param>
        internal virtual void Update(Entity entity, EntityDataAttribute toDataAttribute)
        {
            if (this.name != toDataAttribute.Name)
                throw new Exception(String.Format("������������ �������� \"{0}\" �� ����� ���� ��������.", entity.FullName));

            if (this.type != toDataAttribute.type)
                throw new Exception(String.Format("��� �������� \"{0}\" �� ����� ���� �������.", entity.FullName));

            // �������� ��������� ��������� �� ����, ��� �� ����� ���������� �����������
            if (this.divide != toDataAttribute.Divide)
            {
                if (String.IsNullOrEmpty(this.divide))
                {
                    // �������� �����������
                    this.divide = toDataAttribute.Divide;
                    if (!(toDataAttribute.Owner is IPresentation))
                    {
                        List<DataAttribute> list = Classifier.InitializeDivideAttributes(entity, this);
                        foreach (DataAttribute item in list)
                        {
                            if (!entity.Attributes.ContainsKey(GetKey(item.ObjectKey, item.Name)))
                                entity.Attributes.Add(item);
                            SchemeClass.Instance.DDLDatabase.RunScript(
                                ((EntityDataAttribute) item).AddScript(entity, false, true).ToArray());
                        }
                    }
                }
                else if (String.IsNullOrEmpty(toDataAttribute.Divide))
                {
                    if (!(toDataAttribute.Owner is IPresentation))
                    {
                        // �������� �����������
                        List<string> forDeleteAttributes = new List<string>();
                        foreach (IDataAttribute item in entity.Attributes.Values)
                        {
                            if (IsFixedDivideCodeAttribute(item.Name))
                            {
                                SchemeClass.Instance.DDLDatabase.RunScript(
                                    ((EntityDataAttribute) item).DropScript(entity).ToArray());
                                forDeleteAttributes.Add(item.Name);
                            }
                        }

                        foreach (string item in forDeleteAttributes)
                        {
                            entity.Attributes.Remove(item);
                        }
                    }

                    this.divide = String.Empty;
                }
                else
                {
                    throw new ServerException(
                        String.Format("����������� �������� \"{0}\" �� ����� ���� �������� (�� �����������!).",
                                      entity.FullName));
                }
            }

            if (this.Size != toDataAttribute.Size || this.Scale != toDataAttribute.Scale
                || this.DefaultValue != toDataAttribute.DefaultValue || this.Position != toDataAttribute.Position)
            {
                List<string> script = new List<string>();
                bool recreateViews = this.Name == "Name" && this.Size <= 255 && toDataAttribute.Size > 255;

                this.Size = toDataAttribute.Size;
                this.Scale = toDataAttribute.Scale;
                this.DefaultValue = toDataAttribute.DefaultValue;
                this.position = toDataAttribute.Position;
                
                script.AddRange(this.ModifyScript(entity, true, false, false));
                if (recreateViews)
                    script.AddRange(entity.CreateViews());

                if (!(toDataAttribute.Owner is IPresentation))
                    SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
            }

            if (this.IsNullable != toDataAttribute.IsNullable)
            {
                List<string> script = new List<string>();
                this.IsNullable = toDataAttribute.IsNullable;
                if (!(Owner is IPresentation))
                {
                    if (!toDataAttribute.IsNullable)
                    {
                        // ������������� ��������������
                        script.AddRange(UpdateSetNotNull(entity));
                    }
                    else
                    {
                        // ������� ��������������
                        script.AddRange(UpdateSetNullable(entity));
                    }
                    SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
                }
            }
        }

        /// <summary>
        /// ������������� ���������� ������������
        /// </summary>
        /// <param name="oldValue">������ ������������</param>
        /// <param name="value">����� ������������</param>
        protected override void SetFullName(string oldValue, string value)
        {
            /*if (State == ServerSideObjectStates.New)
            {
                Entity entity = (Entity)Instance.Owner;

                DataAttribute newObject = Instance;
                DataAttribute oldObject = entity.Attributes[oldValue] as DataAttribute;
                try
                {
                    entity.Attributes.Remove(oldValue);
                    entity.Attributes.Add(newObject.Name, newObject);
                }
                catch (Exception e)
                {
                    entity.Attributes.Add(oldValue, oldObject);
                    throw new Exception(e.Message, e);
                }
            }*/
        }
    }
}
