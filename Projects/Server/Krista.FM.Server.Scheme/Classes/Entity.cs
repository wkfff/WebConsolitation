using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

using Krista.FM.ServerLibrary;
using Krista.FM.Common.Exceptions;
using System.Xml.XPath;
using Krista.FM.Server.Scheme.Classes.PresentationLayer;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// ����� ����������� ����������� �������
    /// </summary>
    internal abstract class Entity : CommonDBObject, IEntity
    {
        // ����� �������
        private readonly ClassTypes classType;
        // �������� �������
        private SubClassTypes subClassType;
        // �������� �������� �������, ����������� ��� ���������� �����
        private string shortCaption = String.Empty;
        // ������������ ��� ������� � ������������ �������
        private string macroSet = String.Empty;

        // ��������� ���������
        protected DataAttributeCollection dataAttributeCollection;

        //��������� ���������� ������
        private UniqueKeyCollection _uniqueKeys;
        

        // ��� XML ��������, ����������� � ��������
        protected string tagElementName;

        private string shortName = String.Empty;

        /// <summary>
        /// ���������� �� ������������ ���������
        /// </summary>
        private AssociationCollection associations;

        /// <summary>
        /// ���������� �� �������� ���������
        /// </summary>
        private AssociatedCollection associated;

        /// <summary>
        /// ��������� �������������
        /// </summary>
        private PresentationCollection presentations;

        /// <summary>
        /// ��������� ������������� ����� �������������� �������� ��� ������� ���������,
        /// ��� � ����������-������, �� ������������� ���������� � ����� PostInitialize.
        /// ��� ���� ����� ��������� ����� ������� ������� (�������� ��������� xmlConfig), 
        /// �������� ������� ������������� ������������� �������������.
        /// </summary>
        private bool needInitializePresentations;

        /// <summary>
        /// ������������� ����������.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner">������������ ������.</param>
        /// <param name="semantic">���������.</param>
        /// <param name="name">������������.</param>
        /// <param name="classType">�����.</param>
        /// <param name="subClassType">��������.</param>
        /// <param name="state">���������.</param>
        /// <param name="scriptingEngine">���������� ������.</param>
        public Entity(string key, ServerSideObject owner, string semantic, string name, ClassTypes classType, SubClassTypes subClassType, ServerSideObjectStates state, ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, semantic, name, state, scriptingEngine)
        {
            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            try
            {
                SessionContext.SetSystemContext();

                this.classType = classType;
                this.subClassType = subClassType;

                dataAttributeCollection = new DataAttributeCollection(this, state);
                _uniqueKeys = new UniqueKeyCollection(this, state);

                if (EntityDataAttribute.SystemDummy == null)
                {
                    EntityDataAttribute.CreateAttribute("", "", this, AttributeClass.Regular, state);
                }
                presentations = new PresentationCollection(this, state, string.Empty);

                // ��������� ��������� ��������
                Attributes.Add(EntityDataAttribute.SystemID);

                associations = new AssociationCollection(this, state);
                associated = new AssociatedCollection(this, state);

            }
            finally
            {
                LogicalCallContextData.SetContext(callerContext);
            }
        }

        /// <summary>
        /// ������������� ������� ������� 
        /// </summary>
        /// <param name="doc">�������� � XML ����������</param>
        /// <param name="atagElementName">������������ ���� � ����������� �������</param>
        protected virtual void InitializeProperties(XmlDocument doc, string atagElementName)
        {
            this.Caption = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@caption", atagElementName)).Value;

            XmlNode xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@description", atagElementName));
            if (xmlNode != null)
                this.Description = xmlNode.Value;

            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@shortCaption", atagElementName));
            if (xmlNode != null)
                this.ShortCaption = xmlNode.Value;

            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@shortName", atagElementName));
            if (xmlNode != null)
                this.ShortName = xmlNode.Value;

            xmlNode = doc.SelectSingleNode("MacroSet");
            if (xmlNode != null)
                this.MacroSet = xmlNode.Value;
        }

        /// <summary>
        /// �������������� ��������� ��������� ������� �� ���������� �� XML �������� 
        /// </summary>
        /// <param name="doc">�������� � XML ����������</param>
        /// <param name="atagElementName">������������ ���� � ����������� �������</param>
        protected virtual void InitializeAttributes(XmlDocument doc, string atagElementName)
        {
            XmlNodeList xmlAttributes = doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/Attributes/Attribute", atagElementName));
            foreach (XmlNode xmlAttribute in xmlAttributes)
            {
                EntityDataAttribute attr = EntityDataAttribute.CreateAttribute(this, xmlAttribute, this.State);
                Attributes.Add(attr);
            }
        }

        /// <summary>
        /// �������������� ��������� ������������� �� ���������� �� ���� ������
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tagElementName"></param>
        internal void InitializePresentation(XmlDocument doc)
        {
            // ������������� ������������� �� ���������
            XmlNode xmlPresentations = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/Presentations", tagElementName));
            if (xmlPresentations != null)
                this.Presentations.DefaultPresentation = xmlPresentations.Attributes["default"].InnerText;

            XmlNodeList xmlPresentationList = doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/Presentations/Presentation", tagElementName));
            foreach (XmlNode xmlPresentation in xmlPresentationList)
            {
                try
                {
                    Presentation presentation = Presentation.CreatePresentation(this, xmlPresentation, State);
                    Presentations.Add(presentation.ObjectKey, presentation);
                }
                // ��������� ���������� ��� ������������� �� xml
                catch (InitializeXmlException ex)
                {
                    Trace.TraceError(String.Format("��� ������������� ������������� � ������� {0} �������� ����������: {1}", this.Name, ex.Message));
                    throw new ServerException(String.Format("��� ������������� ������������� � ������� {0} �������� ����������: {1}", this.Name), ex);
                }
            }
        }

        /// <summary>
        /// ����� ������������� �������
        /// </summary>
        internal override XmlDocument Initialize()
        {
            //Trace.TraceEvent(TraceEventType.Verbose, "������������� ������� {0}", this.FullName);
            XmlDocument doc = base.Initialize();

            InitializeProperties(doc, tagElementName);
            InitializeAttributes(doc, tagElementName);
            //TODO: ��������� � InitializeUniqueKeys
            _uniqueKeys.Initialize(doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/UniqueKeyList", tagElementName)));

            if (doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/Presentations", tagElementName)) != null)
                needInitializePresentations = true;

            /*SetDefaultLookupTypeForAttributes();*/
            InitializeDeveloperDescription(doc, tagElementName);
            
            
            return doc;
        }

        internal override void Save2Xml(XmlNode node)
        {
            BaseSave2Xml(node);

            //
            // ��������
            //

            XmlNode attributesNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Attributes", null);

            foreach (DataAttribute attribute in Attributes.Values)
            {
                if (attribute.Class == DataAttributeClassTypes.Typed)
                {
                    XmlNode attributeNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Attribute", null);
                    attribute.Save2Xml(attributeNode);
                    attributesNode.AppendChild(attributeNode);
                }
            }

            node.AppendChild(attributesNode);

            //
            // ���������� �������������
            //
            XmlNode presentationsNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Presentations", null);
            XmlHelper.SetAttribute(presentationsNode, "default", Presentations.DefaultPresentation);

            foreach (Presentation presentation in Presentations.Values)
            {
                XmlNode presentationNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Presentation", null);
                presentation.Save2Xml(presentationNode);
                presentationsNode.AppendChild(presentationNode);
            }
            if (presentationsNode.ChildNodes.Count != 0)
                node.AppendChild(presentationsNode);

            //
            // �������� "MacroSet"
            //

            if (!String.IsNullOrEmpty(MacroSet))
            {
                XmlNode macroSetNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "MacroSet", null);
                XmlCDataSection cdata = node.OwnerDocument.CreateCDataSection(MacroSet);
                macroSetNode.AppendChild(cdata);
                node.AppendChild(macroSetNode);
            }
        }

        /// <summary>
        /// ���������� ������ ��� ����������������
        /// </summary>
        /// <param name="node"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        internal override void Save2XmlDocumentation(XmlNode node)
        {
            try
            {
                BaseSave2Xml(node);

                //
                // ��������
                //

                XmlNode attributesNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Attributes", null);

                foreach (DataAttribute attribute in Attributes.Values)
                {
                    if (attribute.Class == DataAttributeClassTypes.Typed)
                    {
                        XmlNode attributeNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Attribute", null);
                        attribute.Save2XmlDocumentation(attributeNode);
                        attributesNode.AppendChild(attributeNode);
                    }
                }

                node.AppendChild(attributesNode);

                //
                // �������� "MacroSet"
                //

                if (!String.IsNullOrEmpty(MacroSet))
                {
                    XmlNode macroSetNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "MacroSet", null);
                    XmlCDataSection cdata = node.OwnerDocument.CreateCDataSection(MacroSet);
                    macroSetNode.AppendChild(cdata);
                    node.AppendChild(macroSetNode);
                }

                // ���������� �������� ID
                attributesNode = node.SelectSingleNode("//Attributes");
                if (attributesNode != null)
                {
                    // ��������� ��������� ��������
                    foreach (EntityDataAttribute atttribute in Attributes.Values)
                    {
                        if (atttribute.Class != DataAttributeClassTypes.Typed && !(atttribute is DocumentEntityAttribute))
                        {
                            XmlNode attributeNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Attribute", null);
                            atttribute.Save2XmlDocumentation(attributeNode);

                            if (atttribute.Name == "ID")
                            {
                                //��������� ����� ������� ID �������
                                XmlNode firstAttribute = (attributesNode.SelectSingleNode("//Attribute") != null)
                                                             ? attributesNode.SelectSingleNode("//Attribute")
                                                             : attributesNode.SelectSingleNode("//RefAttribute");

                                if (firstAttribute != null)
                                {
                                    attributesNode.InsertBefore(attributeNode, firstAttribute);
                                    continue;
                                }
                            }

                            attributesNode.AppendChild(attributeNode);
                        }
                    }
                }

                // �������������� ��� ������������ �������� ������
                // ������� ���������
                XmlHelper.SetAttribute(node, "semanticCaption", SemanticCaption);
                // ������ ��� � ��
                XmlHelper.SetAttribute(node, "fullDBName", FullDBName);
                // �������
                XmlHelper.SetAttribute(node, "tablePrefix", TablePrefix);
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XPathException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        /// <summary>
        /// ��� ������������ ������������� ��������� ����������
        /// </summary>
        /// <param name="node"></param>
        internal void SaveAssociationDo�umentation(XmlNode node)
        {
            try
            {
                // ����������
                if (Associations.Count != 0)
                {
                    XmlNode assosiationsNode = node.OwnerDocument.CreateElement("AssociationsCls");
                    foreach (EntityAssociation a in associations.Values)
                    {
                        XmlNode assosiation = assosiationsNode.OwnerDocument.CreateElement("AssociationCls");
                        XmlHelper.SetAttribute(assosiation, "name", a.FullDBName);
                        XmlHelper.SetAttribute(assosiation, "fullCaption", a.FullCaption);
                        XmlHelper.SetAttribute(assosiation, "caption", a.Caption);
                        XmlHelper.SetAttribute(assosiation, "associationType", ((int)a.AssociationClassType).ToString());

                        if (a.RoleB != null)
                        {
                            XmlElement RoleB = node.OwnerDocument.CreateElement("RoleBCls");
                            XmlHelper.SetAttribute(RoleB, "objectKey", a.RoleB.ObjectKey);
                            XmlHelper.SetAttribute(RoleB, "fullCaptionRoleB", a.RoleB.FullCaption);
                            XmlHelper.SetAttribute(RoleB, "typeClsRoleB", ((int)a.RoleB.ClassType).ToString());

                            assosiation.AppendChild(RoleB);
                        }

                        assosiationsNode.AppendChild(assosiation);
                    }
                    node.AppendChild(assosiationsNode);
                }

                // ������������ �
                // ����������
                if (Associated.Count != 0)
                {
                    XmlNode assosiatedNode = node.OwnerDocument.CreateElement("AssociatedCls");
                    foreach (EntityAssociation a in associated.Values)
                    {
                        XmlNode assosiation = assosiatedNode.OwnerDocument.CreateElement("AssociateCls");
                        XmlHelper.SetAttribute(assosiation, "name", a.FullDBName);
                        XmlHelper.SetAttribute(assosiation, "fullCaption", a.FullCaption);
                        XmlHelper.SetAttribute(assosiation, "caption", a.Caption);
                        XmlHelper.SetAttribute(assosiation, "associationType", ((int)a.AssociationClassType).ToString());
                        assosiatedNode.AppendChild(assosiation);

                        if (a.RoleA != null)
                        {
                            XmlElement RoleA = node.OwnerDocument.CreateElement("RoleACls");
                            XmlHelper.SetAttribute(RoleA, "objectKey", a.RoleA.ObjectKey);
                            XmlHelper.SetAttribute(RoleA, "fullCaptionRoleA", a.RoleA.FullCaption);
                            XmlHelper.SetAttribute(RoleA, "typeClsRoleA", ((int)a.RoleA.ClassType).ToString());
                            XmlHelper.SetAttribute(RoleA, "fullNameRoleA", a.RoleA.FullDBName);

                            assosiation.AppendChild(RoleA);
                        }
                    }

                    node.AppendChild(assosiatedNode);
                }
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XPathException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        private void BaseSave2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            //
            // �������� ��������
            //

            XmlHelper.SetAttribute(node, "semantic", Semantic);
            XmlHelper.SetAttribute(node, "name", Name);
            if (ShortName != Name)
                XmlHelper.SetAttribute(node, "shortName", ShortName);

            XmlHelper.SetAttribute(node, "caption", Caption);
            if (!String.IsNullOrEmpty(ShortCaption))
                XmlHelper.SetAttribute(node, "shortCaption", ShortCaption);

            if (!String.IsNullOrEmpty(Description))
                XmlHelper.SetAttribute(node, "description", Description);
        }

        public override string ConfigurationXml
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.InnerXml = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><DatabaseConfiguration/>";

                XmlNode element = doc.CreateElement(tagElementName);
                doc.DocumentElement.AppendChild(element);

                Save2Xml(element);
                return doc.InnerXml;
            }
        }

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            UpdateMajorObjectModificationItem root = (UpdateMajorObjectModificationItem)base.GetChanges(toObject);

            Entity toEntity = (Entity)toObject;

            //
            // �������� ��������
            //

            if (this.Semantic != toEntity.Semantic)
                throw new Exception(string.Format("� �������� {0} �������� Semantic ���������� ��� ���������.", this.FullName));

            if (this.Name != toEntity.Name)
                throw new Exception(string.Format("� �������� {0} �������� Name ���������� ��� ���������.", this.FullName));

            // ��������
            if (SubClassType != toEntity.SubClassType)
            {
                if ((SubClassType == SubClassTypes.Input || SubClassType == SubClassTypes.Pump) && toEntity.SubClassType == SubClassTypes.PumpInput)
                {
                    ModificationItem item = new PropertyModificationItem("SubClassType", this.SubClassType, toEntity.SubClassType, root);
                    root.Items.Add(item.Key, item);
                }
                else
                    throw new Exception(String.Format("�������� ������� \"{0}\" �� ����� ���� ������ � {1} �� {2}.", FullName, SubClassType, toEntity.SubClassType));
            }

            if (this.ShortName != toEntity.ShortName)
            {
                ModificationItem item = new PropertyModificationItem("ShortName", this.ShortName, toEntity.ShortName, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Caption != toEntity.Caption)
            {
                ModificationItem item = new PropertyModificationItem("Caption", this.Caption, toEntity.Caption, root);
                root.Items.Add(item.Key, item);
            }

            if (this.ShortCaption != toEntity.ShortCaption)
            {
                ModificationItem item = new PropertyModificationItem("ShortCaption", this.ShortCaption, toEntity.ShortCaption, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Description != toEntity.Description)
            {
                ModificationItem item = new PropertyModificationItem("Description", this.Description, toEntity.Description, root);
                root.Items.Add(item.Key, item);
            }


            if (Instance.MacroSet != toEntity.MacroSet)
            {
                ModificationItem item = new PropertyModificationItem("MacroSet", this.MacroSet, toEntity.MacroSet, root);
                root.Items.Add(item.Key, item);
            }

            //
            // ��������
            //

            ModificationItem dataAttributeModificationItem =
                ((DataAttributeCollection)Attributes).GetChanges((DataAttributeCollection)toEntity.Attributes);

            if (dataAttributeModificationItem.Items.Count > 0)
            {
                dataAttributeModificationItem.Parent = root;
                root.Items.Add(dataAttributeModificationItem.Key, dataAttributeModificationItem);
            }

            //
            // �������������
            //

            ModificationItem presentationsModificationItem = presentations.GetChanges((PresentationCollection)toEntity.Presentations);

            if (presentationsModificationItem.Items.Count > 0)
            {
                presentationsModificationItem.Parent = root;
                root.Items.Add(presentationsModificationItem.Key, presentationsModificationItem);
            }

            //
            // ���������� �����
            //
            ModificationItem uniqueKeysModificationItem = (ModificationItem)UniqueKeys.GetChanges(toEntity.UniqueKeys);
            if (uniqueKeysModificationItem.Items.Count > 0)
            {
                uniqueKeysModificationItem.Parent = root;
                root.Items.Add(uniqueKeysModificationItem.Key, uniqueKeysModificationItem);
            }

            return root;
        }

        /// <summary>
        /// ���������� ID ������ �� ��� �����
        /// </summary>
        /// <param name="db"></param>
        /// <param name="name">������������ ������</param>
        /// <returns>ID ������</returns>
        public static int? GetPackageIDByName(IDatabase db, string name)
        {
            object result = db.ExecQuery("select ID from MetaPackages where Name = ?", QueryResultTypes.Scalar, db.CreateParameter("Name", name));
            if (result == null)
            {
                int PackageID = db.GetGenerator("g_MetaPackages");
                db.ExecQuery("insert into MetaPackages (ID, Name, BuiltIn, Configuration) values (?, ?, ?, ?)", QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", PackageID),
                    db.CreateParameter("Name", name),
                    db.CreateParameter("BuiltIn", 0),
                    db.CreateParameter("Configuration", "", DbType.AnsiString));
                return PackageID;
            }
            else
                return Convert.ToInt32(result);
        }

        /// <summary>
        /// ���������� � ������� ���������� ���������� � ����������� �������
        /// </summary>
        protected void InsertMetaData(ModificationContext context)
        {
            Database db = context.Database;

            int? PackageID = Parent.ID;
            if (PackageID == null)
                throw new Exception(String.Format("� ������� {0} ������ �������������� �����.", FullName));

            // �������� �������� ����������
            int result;
            if (this.ID <= 0)
                result = db.GetGenerator("g_MetaObjects");
            else
                result = this.ID;

            // ��������� ������ � ������� ����������
            db.ExecQuery(
                "insert into MetaObjects (ID, ObjectKey, Name, Semantic, Class, SubClass, Configuration, RefPackages) values (?, ?, ?, ?, ?, ?, ?, ?)",
                QueryResultTypes.NonQuery,
                db.CreateParameter("ID", result),
                db.CreateParameter("ObjectKey", this.ObjectKey),
                db.CreateParameter("Name", this.Name),
                db.CreateParameter("Semantic", this.Semantic),
                db.CreateParameter("Class", (int)this.ClassType),
                db.CreateParameter("SubClass", (int)this.SubClassType),
                db.CreateParameter("Config", this.ConfigurationXml, DbType.AnsiString),
                db.CreateParameter("RefPackages", PackageID));

            this.ID = result;
        }

        /// <summary>
        /// ������� �� �������� ���������� ���������� � ����������� �������
        /// </summary>
        protected void RemoveMetaData(ModificationContext context)
        {
            Database db = context.Database;

            db.ExecQuery("delete from MetaObjects where ID = ?", QueryResultTypes.NonQuery,
                db.CreateParameter("ID", this.ID));
        }

        /// <summary>
        /// �������� ���������� � ���� ������ � �����
        /// </summary>
        internal override void Create(ModificationContext context)
        {
            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.TraceInformation("�������� {0} {1}", this, DateTime.Now);

                base.Create(context);

                try
                {
                    // ���� � ���� ������ ���� ����������� �������, �� �������� �� �������
                    SchemeClass.Instance.DDLDatabase.RunScript(DropScript(), true);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                }

                try
                {
                    SchemeClass.Instance.DDLDatabase.RunScript(CreateScript());
                    this.Unlock();
                    this.DbObjectState = DBObjectStateTypes.InDatabase;

                    InsertMetaData(context);

                    if (!Parent.Classes.ContainsKey(this.ObjectKey))
                        Parent.Classes.Add(this.ObjectKey, this);
                }
                catch (Exception e)
                {
                    if (Parent.Classes.ContainsKey(this.ObjectKey))
                        Parent.Classes.Remove(this.ObjectKey);

                    Trace.TraceError(e.Message);
                    throw new Exception(e.Message, e);
                }
            }
            finally
            {
                InUpdating = updatingState;
            }
        }

        /// <summary>
        /// �������� ������� �� ���� � ����������
        /// </summary>
        internal override void Drop(ModificationContext context)
        {
            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.WriteLine(String.Format("�������� ������� {0}", this));

                #region - �������� ���������� ��������� � �������� -
                Trace.Indent();
                try
                {
                    // ������ ����������, ������� ���������� �������
                    List<EntityAssociation> remove = new List<EntityAssociation>(Associated.Count);

                    // �������� �� ��������� ���������� � ��� �����
                    foreach (EntityAssociation item in Associated.Values)
                        remove.Add(item);

                    foreach (EntityAssociation item in remove)
                        item.Drop(context);

                    remove.Clear();

                    // �������� �� ��������� ���������� � ��� �����
                    foreach (EntityAssociation item in Associations.Values)
                        remove.Add(item);

                    foreach (EntityAssociation item in remove)
                        item.Drop(context);
                }
                finally
                {
                    Trace.Unindent();
                }
                #endregion - �������� ���������� ��������� � �������� -

                #region - �������� ������� -
                try
                {
                    SchemeClass.Instance.DDLDatabase.RunScript(DropScript(), false);

                    RemoveMetaData(context);

                    if (Parent.Classes.ContainsKey(this.ObjectKey))
                        Parent.Classes.Remove(this.ObjectKey);

                    this.DbObjectState = DBObjectStateTypes.Unknown;
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    throw new Exception(e.Message, e);
                }

                base.Drop(context);
                #endregion - �������� ������� -
            }
            finally
            {
                InUpdating = updatingState;
            }
        }

        /// <summary>
        /// ���������� ���������. �������� ������� ������ � ���� ������� toObject
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toObject">������ � ���� �������� ����� �������� ������� ������</param>
        public override void Update(ModificationContext context, IModifiable toObject)
        {
            base.Update(context, toObject);

            Entity toEntity = (Entity)toObject;

            // ����� 
            if (this.ClassType != toEntity.ClassType)
            {
                throw new Exception(String.Format("����� ������� \"{0}\" �� ����� ���� ������.", FullName));
            }

            // ��������
            if (SubClassType != toEntity.SubClassType)
            {
                if (SubClassType == SubClassTypes.Input && toEntity.SubClassType == SubClassTypes.PumpInput)
                {
                    try
                    {
                        List<string> script = new List<string>();

                        if (Attributes.ContainsKey(DataAttribute.TaskIDColumnName))
                        {
                            script.AddRange(((EntityDataAttribute)DataAttribute.SystemTaskIDDefault).ModifyScript(this, true, false, true));
                            SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
                            script.Clear();
                        }

                        Attributes.Add(DataAttribute.SystemPumpIDDefault);
                        script.AddRange(((EntityDataAttribute)DataAttribute.SystemPumpIDDefault).AddScript(this, true, true));
                        SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
                        script.Clear();

                        Attributes.Add(DataAttribute.SystemSourceKey);
                        script.AddRange(((EntityDataAttribute)DataAttribute.SystemSourceKey).AddScript(this, true, true));
                        SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
                        script.Clear();

                        context.Database.ExecQuery(
                            "update MetaObjects set SubClass = ? where ID = ?",
                            QueryResultTypes.NonQuery,
                            context.Database.CreateParameter("SubClass", (int)toEntity.SubClassType),
                            context.Database.CreateParameter("ID", this.ID));

                        SubClassType = toEntity.SubClassType;
                    }
                    catch (Exception e)
                    {
                        // ���������� ���������
                        if (Attributes.ContainsKey(DataAttribute.PumpIDColumnName))
                            Attributes.Remove(DataAttribute.PumpIDColumnName);
                        if (Attributes.ContainsKey(DataAttribute.SourceKeyColumnName))
                            Attributes.Remove(DataAttribute.SourceKeyColumnName);

                        Trace.TraceError("��� ������� �������� ������ ������� �� PumpInput ��������� ������: ", e.Message);
                        throw new Exception(e.Message, e);
                    }
                }
                else if (SubClassType == SubClassTypes.Pump && toEntity.SubClassType == SubClassTypes.PumpInput)
                {
                    try
                    {
                        List<string> script = new List<string>();

                        script.AddRange(((EntityDataAttribute)DataAttribute.SystemPumpIDDefault).ModifyScript(this, true, false, true));
                        SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
                        script.Clear();

                        if (ClassType == ClassTypes.clsFactData)
                        {
                            Attributes.Add(DataAttribute.SystemTaskIDDefault);
                            script.AddRange(((EntityDataAttribute)DataAttribute.SystemTaskIDDefault).AddScript(this, true, true));
                            SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
                            script.Clear();
                        }

                        context.Database.ExecQuery(
                            "update MetaObjects set SubClass = ? where ID = ?",
                            QueryResultTypes.NonQuery,
                            context.Database.CreateParameter("SubClass", (int)toEntity.SubClassType),
                            context.Database.CreateParameter("ID", this.ID));

                        SubClassType = toEntity.SubClassType;
                    }
                    catch (Exception e)
                    {
                        // ���������� ���������
                        if (Attributes.ContainsKey(DataAttribute.TaskIDColumnName))
                            Attributes.Remove(DataAttribute.TaskIDColumnName);

                        Trace.TraceError("��� ������� �������� ������ ������� �� PumpInput ��������� ������: ", e.Message);
                        throw new Exception(e.Message, e);
                    }
                }
                else
                    throw new Exception(String.Format("�������� ������� \"{0}\" �� ����� ���� ������.", FullName));
            }

            // ����������� ���������� ������������
            if (ShortName != toEntity.ShortName)
            {
                Trace.WriteLine(String.Format("� ������� \"{0}\" ���������� ����������� ���������� ������������ c \"{1}\" �� \"{2}\"",
                    FullName, ShortName, toEntity.ShortName));
                ShortName = toEntity.ShortName;
            }

            // ����������� ������� ������������
            if (ShortCaption != toEntity.ShortCaption)
            {
                Trace.WriteLine(String.Format("� ������� \"{0}\" ���������� ����������� ������� ������������ c \"{1}\" �� \"{2}\"",
                    FullName, ShortCaption, toEntity.ShortCaption));
                ShortCaption = toEntity.ShortCaption;
            }
        }

        /// <summary>
        /// ���������� XML-������������ � ���� ������
        /// </summary>
        internal override void SaveConfigurationToDatabase(ModificationContext context)
        {
            IDatabase db = context.Database;

            string confXmlString = ConfigurationXml;

            // TODO: ��������� XML ����� ����������� � ��

            // ��������� ������ � �������� ����������
            db.ExecQuery(
                "update MetaObjects set Configuration = ?, ObjectKey = ? where ID = ?",
                QueryResultTypes.NonQuery,
                db.CreateParameter("Config", confXmlString, DbType.AnsiString),
                db.CreateParameter("ObjectKey", ObjectKey, DbType.AnsiString),
                db.CreateParameter("ID", this.ID));

            this.Configuration = confXmlString;
        }

        /// <summary>
        /// ������������� ���������� ������������
        /// </summary>
        /// <param name="oldValue">������ ������������</param>
        /// <param name="value">����� ������������</param>
        protected override void SetFullName(string oldValue, string value)
        {
            if (State == ServerSideObjectStates.New)
            {
                Package package = (Package)Instance.Owner;

                Entity newObject = Instance;
                Entity oldObject = package.Classes[oldValue] as Entity;
                try
                {
                    package.Classes.Remove(oldValue);
                    package.Classes.Add(newObject.FullName, newObject);
                }
                catch (Exception e)
                {
                    package.Classes.Add(oldValue, oldObject);
                    throw new Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// �������� �� ������������ � �������� �����
        /// </summary>
        protected override void CheckFullName()
        {
            if (SchemeClass.Instance.RootPackage.FindEntityByName(FullName) != null)
                throw new Exception(String.Format("������ � ������ {0} ��� ������������ � �����.", FullName));
        }

        #region ������ � DDL

        /// <summary>
        /// ���������� ������������ ���������� (FullDBName ���������� �� 28 ��������)
        /// </summary>
        public virtual string GeneratorName
        {
            get
            {
                if (SessionContext.IsDeveloper &&
                    (ClassType == ClassTypes.clsBridgeClassifier || ClassType == ClassTypes.clsDataClassifier))
                    return ((ClassifierEntityScriptingEngine)_scriptingEngine).DeveloperGeneratorName(FullDBName);
                else
                    return ((EntityScriptingEngine)_scriptingEngine).GeneratorName(FullDBName);
            }
        }

        /// <summary>
        /// ���������� ������������ ���������� ��� ������ "������������" (FullDBName ���������� �� 28 ��������)
        /// </summary>
        public virtual string DeveloperGeneratorName
        {
            get
            {
                if (ClassType == ClassTypes.clsBridgeClassifier || ClassType == ClassTypes.clsDataClassifier)
                    return ((ClassifierEntityScriptingEngine)_scriptingEngine).DeveloperGeneratorName(FullDBName);
                else
                    return ((EntityScriptingEngine)_scriptingEngine).GeneratorName(FullDBName);
            }
        }

        /// <summary>
        /// �������� �������������� ������������� ��� ���������� �����
        /// </summary>
        /// <returns>DDL-����������� �������������</returns>
        internal string[] CreateViews()
        {
            return CreateViews(EntityDataAttribute.SystemDummy);
        }

        /// <summary>
        /// �������� �������������� ������������� ��� ���������� �����
        /// </summary>
        /// <param name="withoutAttribute">�������, ������� �� ����� ����������� ��� �������� ��������� ��������</param>
        /// <returns>DDL-����������� �������������</returns>
        protected virtual string[] CreateViews(DataAttribute withoutAttribute)
        {
            return new string[0];
        }

        internal string TriggerNamePart
        {
            get { return EntityScriptingEngine.TriggerNamePart(FullDBName, FullDBShortName); }
        }

        internal string AuditTriggerName
        {
            get { return String.Format("t_{0}_aa", TriggerNamePart); }
        }

        internal void EnableAllTriggers(Database db)
        {
            db.RunScript(_scriptingEngine.EnableAllTriggersForObject(FullDBName).ToArray());
        }

        internal void DisableAllTriggers(Database db)
        {
            db.RunScript(_scriptingEngine.DisableAllTriggers(FullDBName).ToArray());
        }

        /// <summary>
        /// ���������� ����������� �������� (���������, ������������� � �.�.)
        /// ��������� � ������� ��������
        /// </summary>
        /// <returns>DDL-����������� ��������</returns>
        internal string[] GetDependentScripts()
        {
            return GetDependentScripts(EntityDataAttribute.SystemDummy);
        }

        /// <summary>
        /// ���������� ����������� �������� (���������, ������������� � �.�.)
        /// ��������� � ������� ��������
        /// </summary>
        /// <param name="withoutAttribute">�������, ������� �� ����� ����������� ��� �������� ��������� ��������</param>
        /// <returns>DDL-����������� ��������</returns>
        internal string[] GetDependentScripts(DataAttribute withoutAttribute)
        {
            return _scriptingEngine.CreateDependentScripts(this, withoutAttribute).ToArray();
        }

        /// <summary>
        /// ���������� ������ �������� ��� ��������� ���������
        /// </summary>
        /// <param name="entity">�������� ���������</param>
        /// <param name="withoutAttribute"></param>
        /// <returns>������</returns>
        internal virtual List<string> GetCustomTriggerScriptForChildEntity(Entity entity, DataAttribute withoutAttribute)
        {
            return new List<string>();
        }

        internal string[] CreateScript()
        {
            return _scriptingEngine.CreateScript(this).ToArray();
        }

        internal string[] DropScript()
        {
            return _scriptingEngine.DropScript(this).ToArray();
        }

        /// <summary>
        /// ���������� ��������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toDataAttribute"></param>
        internal void UpdateAttribute(ModificationContext context, EntityDataAttribute toDataAttribute)
        {
            EntityDataAttribute attribute = (EntityDataAttribute)DataAttributeCollection.GetAttributeByKeyName(Attributes, toDataAttribute.ObjectKey, toDataAttribute.Name);

            attribute.Update(this, toDataAttribute);

            SaveConfigurationToDatabase(context);

            Trace.WriteLine(String.Format("������� \"{0}\" ������� �������.", attribute));
        }

        /// <summary>
        /// ���������� �������������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toPresentattion"></param>
        internal void UpdatePresentation(ModificationContext context, Presentation toPresentation)
        {
            Presentation presentation = (Presentation)this.presentations[toPresentation.ObjectKey];
            if (presentation != null)
            {
                presentation.Update(this, toPresentation);
                SaveConfigurationToDatabase(context);

                Trace.WriteLine(String.Format("������������� \"{0}\" ������� ��������.", presentation));
            }
        }


        /// <summary>
        /// ��������� ������� � �������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attribute"></param>
        internal void AddAttribute(ModificationContext context, EntityDataAttribute attribute)
        {
            if (!Attributes.ContainsKey(GetKey(attribute.ObjectKey, attribute.Name)))
            {
                try
                {
                    attribute.Owner = this;
                    // ��������� �������� � ���������
                    Attributes.Add(attribute);

                    try
                    {
                        // �������� �� ������ ������ ����
                        SchemeClass.Instance.DDLDatabase.RunScript(attribute.DropScript(this).ToArray(), true);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceWarning("��� ���������� ������� �������� ��������� ������: {0}", e.Message);
                    }

                    // ��������� �������� � ����
                    SchemeClass.Instance.DDLDatabase.RunScript(attribute.AddScript(this, true, true).ToArray());

                    // ��������� ����������
                    SaveConfigurationToDatabase(context);

                    attribute.State = ServerSideObjectStates.Consistent;

                    Trace.WriteLine(String.Format("������� \"{0}\" ������� ��������.", attribute));
                }
                catch (Exception e)
                {
                    if (Attributes.ContainsKey(attribute.Name))
                        Attributes.Remove(attribute.Name);
                    throw new Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// �������� �������� �� �������
        /// </summary>
        internal void RemoveAttribute(ModificationContext context, string attributeName)
        {
            if (Attributes.ContainsKey(attributeName))
            {
                EntityDataAttribute attribute = (EntityDataAttribute)DataAttributeCollection.GetAttributeByKeyName(Attributes, attributeName, attributeName);

                // ������� �������� �� ����
                SchemeClass.Instance.DDLDatabase.RunScript(attribute.DropScript(this).ToArray());

                // ������� �������� �� ���������
                Attributes.Remove(GetKey(attribute.ObjectKey, attribute.Name));

                // ��������� ����������
                SaveConfigurationToDatabase(context);

                Trace.WriteLine(String.Format("������� \"{0}\" ������� ������.", attribute));
            }
        }

        /// <summary>
        /// ���������� ������������� � ���������� � ����������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="presentation"></param>
        internal void AddPresentation(ModificationContext context, Presentation presentation)
        {
            if (!Presentations.ContainsKey(presentation.ObjectKey))
            {
                this.Presentations.Add(presentation.ObjectKey, presentation);
                presentation.State = ServerSideObjectStates.Consistent;

                SaveConfigurationToDatabase(context);

                Trace.WriteLine(String.Format("������������� \"{0}\" ������� ���������", presentation));
            }
        }

        /// <summary>
        /// �������� ������������� � ���������� � ����������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="presentationKey"></param>
        internal void RemovePresentation(ModificationContext context, string presentationKey)
        {
            if (Presentations.ContainsKey(presentationKey))
            {
                Presentation presentation = PresentationCollection.GetPresentationByKey(Presentations, presentationKey);

                if (presentation != null)
                {
                    Presentations.Remove(presentationKey);
                    // �������� �� ������� ������ ���������������
                    SchemeClass.Instance.DataVersionsManager.DataVersions.RemovePresentation(presentationKey);

                    SaveConfigurationToDatabase(context);

                    Trace.WriteLine(String.Format("������������� \"{0}\" ������� �������", presentation));
                }
            }
        }

        #endregion ������ �� DDL

        #region ������ � �������

        /// <summary>
        /// ���������� ���������� ������� �� ���������� ���������.
        /// </summary>
        /// <param name="sourceID">ID ��������� ������.  ���� ����� -1, �� ���������� �� ���� ����������.</param>
        /// <returns>���������� ������� �� ���������</returns>
        public virtual int RecordsCount(int sourceID)
        {
            IDatabase db = null;
            try
            {
                IDbDataParameter[] parameters = null;
                string filterQuery = String.Empty;
                if (sourceID > -1)
                {
                    filterQuery = string.Format(" where SourceID = {0}", sourceID);
                }
                string selectQuery = "select count(*) from " + FullDBName;
                db = SchemeClass.Instance.SchemeDWH.DB;
                return Convert.ToInt32(db.ExecQuery(selectQuery + filterQuery, QueryResultTypes.Scalar, parameters));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// ���������� ��������� �������� ����������
        /// </summary>
        public virtual int GetGeneratorNextValue
        {
            get
            {
                IDatabase db = null;
                try
                {
                    db = SchemeClass.Instance.SchemeDWH.DB;
                    return db.GetGenerator(GeneratorName);
                }
                finally
                {
                    if (db != null)
                        db.Dispose();
                }
            }
        }

        public virtual IDataUpdater GetDataUpdater()
        {
            return GetDataUpdater(null, null);
        }

        public virtual IDataUpdater GetDataUpdater(IDatabase db)
        {
            return GetDataUpdater(db, null, null);
        }

        public virtual IDataUpdater GetDataUpdater(string selectFilter, int? maxRecordCountInSelect, params IDbDataParameter[] selectFilterParameters)
        {
            IDatabase db = null;
            try
            {
                db = SchemeClass.Instance.SchemeDWH.DB;
                return GetDataUpdater(db, selectFilter, maxRecordCountInSelect, selectFilterParameters);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        public virtual IDataUpdater GetDataUpdater(IDatabase db, string selectFilter, int? maxRecordCountInSelect, params IDbDataParameter[] selectFilterParameters)
        {
            try
            {
                DataUpdater du = (DataUpdater)db.GetDataUpdater(this.FullDBName, this.Attributes, selectFilter, maxRecordCountInSelect, selectFilterParameters);
                du.OnAfterUpdate += new AfterUpdateEventDelegate(OnAfterUpdate);
                return du;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        protected virtual void OnAfterUpdate()
        {
        }

        /// <summary>
        /// ������� ������ �������
        /// </summary>
        /// <param name="db">������ ������� � ������ ��� ������ �������� ���������� ��������</param>
        /// <param name="whereClause">
        /// ��������� "where Param1 = ? and Param2 = ? or ...", 
        /// ���� ������� ���, �� ����� ���������� ������ ������: String.Empty</param>
        /// <param name="parameters">�������� ���������� ������������� � whereClause</param>
        /// <returns>���������� ��������� �������</returns>
        public int DeleteData(Database db, string whereClause, params object[] parameters)
        {
            IDbDataParameter[] dbParameters = null;
            if (parameters != null)
            {
                dbParameters = new IDbDataParameter[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    dbParameters[i] = db.CreateParameter(String.Format("Param{0}", i), parameters[i]);
            }

            int result = Convert.ToInt32(db.ExecQuery(
                String.Format("delete from {0} {1}", FullDBName, whereClause),
                QueryResultTypes.NonQuery, dbParameters));

            OnAfterUpdate();
            
            return result;
        }

        /// <summary>
        /// ������� ������ ������� � ����������� ������
        /// </summary>
        /// <param name="whereClause">
        /// ��������� "where Param1 = ? and Param2 = ? or ...", 
        /// ���� ������� ���, �� ����� ���������� ������ ������: String.Empty</param>
        /// <param name="parameters">�������� ���������� ������������� � whereClause</param>
        /// <param name="disableTriggerAudit">true - ���������� ������
        /// false - �������� ������ �������� ������ � ����������� ���������</param>
        /// <returns></returns>
        public int DeleteData(string whereClause, bool disableTriggerAudit, params object[] parameters)
        {
            if (!disableTriggerAudit)
                throw new Exception("������� �������������� � ��������� ������ �������� ������ �� �����������");

            bool isOracle = (SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess);

            int result = 0;

            Database db = (Database)SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                bool existTrigger_aa = _scriptingEngine.ExistsObject(String.Format("t_{0}_aa", FullDBName), ObjectTypes.Trigger);

                if (existTrigger_aa && isOracle)
                    db.ExecQuery(String.Format("alter trigger t_{0}_aa disable", FullDBName),
                                 QueryResultTypes.NonQuery);

                bool existTrigger_sourceid_lc = _scriptingEngine.ExistsObject(String.Format("t_{0}_sourceid_lc", FullDBName), ObjectTypes.Trigger);

                if (existTrigger_sourceid_lc && isOracle)
                    db.ExecQuery(String.Format("alter trigger t_{0}_sourceid_lc disable", FullDBName),
                                 QueryResultTypes.NonQuery);

                result = DeleteData(db, whereClause, parameters);

                if (existTrigger_aa && isOracle)
                    db.ExecQuery(String.Format("alter trigger t_{0}_aa enable", FullDBName),
                                 QueryResultTypes.NonQuery);

                if (existTrigger_sourceid_lc && isOracle)
                    db.ExecQuery(String.Format("alter trigger t_{0}_sourceid_lc enable", FullDBName),
                                 QueryResultTypes.NonQuery);
            }
            finally
            {
                db.Dispose();
            }

            return result;
        }

        /// <summary>
        /// ������� ������ �������
        /// </summary>
        /// <param name="whereClause">
        /// ��������� "where Param1 = ? and Param2 = ? or ...", 
        /// ���� ������� ���, �� ����� ���������� ������ ������: String.Empty</param>
        /// <param name="parameters">�������� ���������� ������������� � whereClause</param>
        /// <returns>���������� ��������� �������</returns>
        public virtual int DeleteData(string whereClause, params object[] parameters)
        {
            Database db = (Database)SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                return DeleteData(db, whereClause, parameters);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// ��������� �������������� ��������� ������ � �������.
        /// </summary>
        public virtual bool ProcessObjectData()
        {
            return false;
        }

        #endregion ������ � �������

        #region ServerSideObject

        /// <summary>
        /// ���������� ��������� ������� � ������� ������ �������� ������� ������������
        /// </summary>
        /// <returns>��������� �������</returns>
        protected new Entity Instance
        {
            [DebuggerStepThrough]
            get { return (Entity)GetInstance(); }
        }

        /// <summary>
        /// ���������� ��������� ������� � ������� ������ �������� ������� ������������, ��� ��������� �������� �������
        /// </summary>
        /// <returns>��������� �������</returns>
        protected new Entity SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (Entity)CloneObject;
                else
                    return this;
            }
        }

        public override IServerSideObject Lock()
        {
            Package clonePackage = (Package)Owner.Lock();
            return (ServerSideObject)clonePackage.Classes[this.ObjectKey];
        }

        /// <summary>
        /// ������� ����� ������, ���������� ������ �������� ����������. 
        /// </summary>
        /// <returns>����� ������, ���������� ������ �������� ����������.</returns>
        public override Object Clone()
        {
            Entity clone = (Entity)base.Clone();
            clone.dataAttributeCollection = (DataAttributeCollection)dataAttributeCollection.Clone();
            clone.associations = (AssociationCollection)associations.Clone(false);
            clone.associated = (AssociatedCollection)associated.Clone(false);
            clone.presentations = (PresentationCollection)presentations.Clone(false);
            clone._uniqueKeys = (UniqueKeyCollection)_uniqueKeys.Clone();
            return clone;
        }

        /// <summary>
        /// ������� ���������� � �������
        /// </summary>
        public override void Unlock()
        {
            associations.Unlock();
            associated.Unlock();
            dataAttributeCollection.Unlock();
            presentations.Unlock();
            _uniqueKeys.Unlock();
            base.Unlock();
        }

        /// <summary>
        /// ��������� ���������� ������� �� ������� ��� �������������
        /// </summary>
        public override ServerSideObjectStates State
        {
            [DebuggerStepThrough]
            get { return base.State; }
            set
            {
                //ServerSideObjectStates oldState = base.State;
                if (base.State != value)
                {
                    base.State = value;
                    dataAttributeCollection.State = value;
                    _uniqueKeys.State = value;
                    // �������� ��������� ��������, ������� �� �������� ����������
                    // �������� ������� ������!
                    //if (oldState == ServerSideObjectStates.New)
                    //{
                    //    associated.State = value;
                    //    associations.State = value;
                    //}
                }
            }
        }

        #endregion ServerSideObject

        /// <summary>
        /// ������������ ��������� ��������� ������� (UP or virtual(abstract))
        /// </summary>
        public IDataAttributeCollection Attributes
        {
            [DebuggerStepThrough]
            get
            {
                if (Presentations.Count != 0)
                {
                    LogicalCallContextData context = LogicalCallContextData.GetContext();
                    if (context[String.Format("{0}.Presentation", this.FullDBName)] != null)
                    {
                        string presentationKey = Convert.ToString(context[String.Format("{0}.Presentation", this.FullDBName)]);
                        if (Presentations.ContainsKey(presentationKey))
                            return Presentations[presentationKey].Attributes;
                        else if (Presentations.ContainsKey(Presentations.DefaultPresentation))
                            return Presentations[Presentations.DefaultPresentation].Attributes;

                    }
                }
                return dataAttributeCollection;
            }
        }

        /// <summary>
        /// ��������� ��������������� ���������
        /// </summary>
        public IDataAttributeCollection GroupedAttributes
        {
            get
            {
                return new GroupedAttributeCollection(this, state, Attributes);
            }
        }

        public IPresentationCollection Presentations
        {
            [DebuggerStepThrough]
            get { return presentations; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        public ClassTypes ClassType
        {
            [DebuggerStepThrough]
            get { return this.classType; }
        }

        /// <summary>
        /// ������������� �������� �������, ������� ��� ���������
        /// </summary>
        /// <param name="value">��������������� ��������</param>
        protected virtual void ChangeSubClassType(SubClassTypes value)
        {
            if (Instance.subClassType != value)
                throw new Exception("�������� SubClassType �������� ������ ��� ������.");
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        public SubClassTypes SubClassType
        {
            [DebuggerStepThrough]
            get { return Instance.subClassType; }
            set
            {
                if (Authentication.IsSystemRole())
                {
                    subClassType = value;
                }
                else if (Instance.subClassType != value)
                {
                    if (State != ServerSideObjectStates.New && Instance.subClassType != value
                        && (!(SetInstance.subClassType == SubClassTypes.Input || SetInstance.subClassType == SubClassTypes.Pump) || value != SubClassTypes.PumpInput))
                        throw new Exception("�������� SubClassType ����� ������ ������ � ����� ��������� ��������.");

                    ChangeSubClassType(value);

                    SetInstance.subClassType = value;
                }
            }
        }

        /// <summary>
        /// �������� ���������� ������������ ������������ ��� ���������� ������� ���� � ���� ������
        /// </summary>
        public string ShortName
        {
            [DebuggerStepThrough]
            get { return shortName == String.Empty ? Name : shortName; }
            set { shortName = value; }
        }

        public virtual string TablePrefix
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// ������ ��� �������.
        /// </summary>
        public override string FullName
        {
            [DebuggerStepThrough]
            get { return Semantic + "." + base.Name; }
        }

        public string FullShortName
        {
            [DebuggerStepThrough]
            get { return TablePrefix + "." + Semantic + "." + ShortName; }
        }

        public string FullDBShortName
        {
            [DebuggerStepThrough]
            get { return FullShortName.Replace('.', '_'); }
        }

        /// <summary>
        /// ������ ������� ������������ �������, ��������� �� �������������� ����� � �������� ������� ����������� ������
        /// </summary>
        public virtual string FullCaption
        {
            [DebuggerStepThrough]
            get
            {
                return SemanticCaption + "." + Caption;
            }
        }

        /// <summary>
        /// ��������� ��� OLAP-�������
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        protected virtual string GetOlapCaption()
        {
            if (this.ShortCaption != String.Empty)
                return SemanticCaption + "." + ShortCaption;
            else
                return SemanticCaption + "." + Caption;
        }

        /// <summary>
        /// ���������� ��� ���������������� OLAP-�������
        /// </summary>
        public virtual string OlapName
        {
            [DebuggerStepThrough]
            get { return GetOlapCaption(); }
        }

        /// <summary>
        /// ������� ������������ ������� ��������� � ����������
        /// </summary>
        public override string Caption
        {
            get { return base.Caption; }
            set
            {
                Scheme.ScriptingEngine.ScriptingEngineHelper.CheckOlapName(value);
                base.Caption = value;
            }
        }

        /// <summary>
        /// �������� �������� �������, ����������� ��� ���������� �����
        /// </summary>
        public string ShortCaption
        {
            [DebuggerStepThrough]
            get { return Instance.shortCaption; }
            set { SetInstance.shortCaption = value; }
        }

        /// <summary>
        /// ������������ ��� ������� � ������������ �������
        /// </summary>
        public string MacroSet
        {
            [DebuggerStepThrough]
            get { return Instance.macroSet; }
            set { SetInstance.macroSet = value; }
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return String.Format("{0} : {1}", FullName, base.ToString());
        }

        /// <summary>
        /// ��������� ���������� ������
        /// </summary>
        public IUniqueKeyCollection UniqueKeys
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return Instance._uniqueKeys; }
        }

        /// <summary>
        /// ���������� ������� ������������ �������� ���������� ������ � ������� ���� ��������
        /// </summary>
        public virtual bool UniqueKeyAvailable
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        /// ���������� �� ������������ ���������
        /// </summary>
        public IEntityAssociationCollection Associations
        {
            [DebuggerStepThrough]
            get { return associations; }
        }

        /// <summary>
        /// ���������� �� �������� ���������
        /// </summary>
        public IEntityAssociationCollection Associated
        {
            [DebuggerStepThrough]
            get { return associated; }
        }

        /// <summary>
        /// ����������� ����������� ������ (UP)
        /// </summary>
        public virtual void Process()
        {
            Process(ProcessTypes.Default);
        }

        /// <summary>
        /// ����������� ����������� ������ (UP)
        /// </summary>
        /// <param name="sourceID">ID ��������� �������� �������� ���������� ���������</param>
        public void Process(int sourceID)
        {
            Process(ProcessTypes.Default);
        }

        /// <summary>
        /// ����������� ����������� ������ (UP)
        /// </summary>
        /// <param name="processTypes">��� ������� �������</param>
        public virtual void Process(ProcessTypes processTypes)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> GetSQLMetadataDictionary()
        {
            Dictionary<string, string> sqlMetadata = base.GetSQLMetadataDictionary();

            sqlMetadata.Add(SQLMetadataConstants.AuditTrigger, AuditTriggerName);

            foreach (EntityAssociation entityAssociation in Associations.Values)
            {
                if (entityAssociation.RoleB is IVariantDataClassifier)
                {
                    if (!sqlMetadata.ContainsKey(SQLMetadataConstants.VariantLockTrigger))
                        sqlMetadata.Add(SQLMetadataConstants.VariantLockTrigger, VariantDataClassifierScriptingEngine.GetVariantLockTriggerName(FullDBName, ShortName, entityAssociation.FullDBName));
                }
            }

            return sqlMetadata;
        }

        /// <summary>
        /// ���������� SQL-����������� �������.
        /// </summary>
        /// <returns>SQL-����������� �������.</returns>
        public override List<string> GetSQLDefinitionScript()
        {
            List<string> script = base.GetSQLDefinitionScript();
            script.AddRange(_scriptingEngine.CreateScript(this));
            return script;
        }

        #region ����� ��������� ������

        /// <summary>
        /// �� �������� � ��������� ������ ���� ��������� ������
        /// </summary>
        /// <param name="rID">������������� ������ � �������</param>
        /// <param name="recursive">������ �� ��������� ��� ���������.</param>
        /// <returns>DataSet � ���������� ������ � ����������� ��������� ������ (���� ����)</returns>
        public DataSet GetDependedData(int rID, bool recursive)
        {
            DataTable dt = CreateResultDT();
            IDatabase db = null;
            DataSet dsDepended = new DataSet();
            dsDepended.Tables.Add(dt);
            try
            {
                db = SchemeClass.Instance.SchemeDWH.DB;
                // ���� �� ����������
                if (!recursive)
                {
                    // ����
                    GetDirectDepended(db, dt, rID);
                }
                else
                {
                    // ����� ������� ��������
                    DataColumn colResultRowID = new DataColumn("ResultRowID", Type.GetType("System.Int32"));
                    DataColumn colParentID = new DataColumn("ParentID", Type.GetType("System.Int32"));
                    DataColumn colDepth = new DataColumn("Depth", Type.GetType("System.Int32"));
                    dt.Columns.Add(colResultRowID);
                    dt.Columns.Add(colParentID);
                    dt.Columns.Add(colDepth);
                    dsDepended.Relations.Add(new DataRelation("ParentChild", colResultRowID, colParentID, false));
                    string queryText = string.Format("select ID from {0} where {1} = ?", this.FullDBName, DataAttribute.IDColumnName);
                    DataTable dtDepended = (DataTable)(db.ExecQuery(queryText, QueryResultTypes.DataTable, db.CreateParameter("rID", rID)));
                    // ����
                    GetDependedDataRecursive(dtDepended.Rows, this, ref dt, db, 0, 0);
                }
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dsDepended;
        }

        // ���� ������ ���������.
        private void GetDirectDepended(IDatabase db, DataTable dt, int rID)
        {
            foreach (EntityAssociation item in associated.Values)
            {
                Entity entity = (Entity)item.RoleData;

                string fullDBName = entity.FullDBName;
                string refCls = item.RoleDataAttribute.Name;

                // ������� ��������� ������.
                string queryText = string.Format("select count (*) from {0} where {1} = ?", fullDBName, refCls);
                int count = Convert.ToInt32(db.ExecQuery(queryText, QueryResultTypes.Scalar, db.CreateParameter("rID", rID)));

                // ��������� ������.
                DataRow row = GetDependedDataRow(dt, entity, count, GetAssociationType(item));
                dt.Rows.Add(row);
            }
        }


        /// <summary>
        /// ���������� ���� ��������� ������.
        /// </summary>
        /// <param name="rID">ID �������, ��� ������� ������ ���������.</param>
        /// <param name="parentEntity">��������, ��� ������� ������ ���������.</param>
        /// <param name="dt">������� � ������������.</param>
        /// <param name="db">������ ������� � ��.</param>
        /// <param name="parentID">ID ������������ ������ � ������� �����������.</param>
        /// <param name="depth">������� ��������.</param>
        private void GetDependedDataRecursive(DataRowCollection rID, Entity parentEntity, ref DataTable dt, IDatabase db, int parentID, int depth)
        {
            // ���������� ����������.
            foreach (EntityAssociation item in parentEntity.associated.Values)
            {
                Entity entity = (Entity)item.RoleData;
                string fullDBName = entity.FullDBName;
                string refCls = item.RoleDataAttribute.Name;
                // � ��� ���������� ���������� ���������.
                foreach (DataRow rowID in rID)
                {
                    // �� ������� ID ������� ���������
                    string queryText = string.Format("select ID from {0} where {1} = ?", fullDBName, refCls);
                    DataTable dtDepended = (DataTable)(db.ExecQuery(queryText, QueryResultTypes.DataTable, db.CreateParameter("rID", Convert.ToInt32(rowID["ID"]))));
                    int count = dtDepended.Rows.Count;

                    // ��������� ������.
                    DataRow row = GetDependedDataRow(dt, entity, count, GetAssociationType(item));

                    // ��������� ��� ID � ������ �� ��������.
                    // ���������� ������� � ����� � ��� ID.
                    row["ResultRowID"] = dt.Rows.Count + 1;
                    row["ParentID"] = parentID;
                    row["Depth"] = depth;
                    dt.Rows.Add(row);
                    //  �������� ����� ��������� ��� ���������.
                    GetDependedDataRecursive(dtDepended.Rows, entity, ref dt, db, dt.Rows.Count, depth + 1);
                }
            }
        }

        /// <summary>
        /// ��������� ������ ��� ���������� � ������� �����������.
        /// </summary>
        /// <param name="dt">�������.</param>
        /// <param name="obj">������, ���������� � ������� ���������.</param>
        /// <returns>������ ��� ����������.</returns>
        private static DataRow GetDependedDataRow(DataTable dt, Entity obj, int count, string associationType)
        {
            DataRow row = dt.NewRow();
            row["ObjectType"] = obj.GetObjectType();
            row["Name"] = obj.FullName;
            row["FullDBName"] = obj.FullDBName;
            row["FullCaption"] = obj.FullCaption;
            row["Count"] = count;
            row["AssociationType"] = associationType;
            return row;
        }

        /// <summary>
        /// ���������� �������� ���� �������.
        /// </summary>
        public string GetObjectType()
        {
            return EnumHelper.GetEnumItemDescription(typeof(ClassTypes), classType);
        }

        /// <summary>
        /// ��������� ��� ����������.
        /// </summary>
        /// <param name="item">����������.</param>
        /// <returns>���.</returns>
        private static string GetAssociationType(EntityAssociation item)
        {
            string associationType = string.Empty;
            if (item is MasterDetailAssociation)
                associationType = "���������� ������-������";
            else
                if (item is BridgeAssociation)
                    associationType = "���������� ������������� ����� ��������������� ������ � ������������";
                else if (item is FactAssociation)
                    associationType = "���������� ����� ������� � ���������������� ������";
            return associationType;
        }

        /// <summary>
        /// ������� Datatable ��� �����������.
        /// </summary>
        /// <returns></returns>
        private static DataTable CreateResultDT()
        {
            DataTable dt = new DataTable("DependedData");
            DataColumn colFullCaption = new DataColumn("FullCaption", Type.GetType("System.String"));
            DataColumn colFullDBName = new DataColumn("FullDBName", Type.GetType("System.String"));
            DataColumn colObjectType = new DataColumn("ObjectType", Type.GetType("System.String"));
            DataColumn colName = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn colAssociationType = new DataColumn("AssociationType", Type.GetType("System.String"));
            DataColumn colCount = new DataColumn("Count", Type.GetType("System.Int32"));

            dt.Columns.Add(colObjectType);
            dt.Columns.Add(colFullCaption);
            dt.Columns.Add(colFullDBName);
            dt.Columns.Add(colName);
            dt.Columns.Add(colAssociationType);
            dt.Columns.Add(colCount);
            return dt;
        }

        #endregion

        #region ������� ����������

        /// <summary>
        /// ���������� ������� ����������.
        /// </summary>
        /// <param name="mainRecordID">ID �������� ������.</param>
        /// <param name="duplicatesID">������ ID ����������.</param>
        public void MergingDuplicates(int mainRecordID, List<int> duplicatesID, MergeDuplicatesListener listener)
        {
            Database db = null;
            int allMovedRecordsCount = 0;
            int duplicatesCount = 0;
            string moveParentChildMessage = string.Empty;
            Entity associatedEntity = null;
            try
            {
                db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                db.BeginTransaction();
                // �������� �� ������ ����������
                foreach (int duplicateID in duplicatesID)
                {
                    // ���� ������������� ������� �� ����������
                    // ����� ���������� � ��� ID ���������
                    string sourceIdMessage = string.Empty;
                    if (this is DataSourceDividedClass && ((DataSourceDividedClass)this).IsDivided)
                    {
                        int sourceID;
                        sourceID = Convert.ToInt32(db.ExecQuery(
                            string.Format("select {0} from {1} where {2} = ?",
                            DataAttribute.SourceIDColumnName, FullDBName, DataAttribute.IDColumnName),
                            QueryResultTypes.Scalar,
                            db.CreateParameter("duplicateID", duplicateID)));
                        sourceIdMessage = string.Format("; �������� ������ ID = {0}", sourceID);
                    }

                    // ��������, ��� ������ �������� � ����������.
                    listener(string.Format("�������� ��������� ID = {0}{1}{2}",
                        duplicateID, sourceIdMessage, Environment.NewLine));

                    // ���� ������������� �������������
                    if (((IClassifier)this).Levels.HierarchyType == HierarchyType.ParentChild)
                    {
                        // ������������ ������ Parent-Child.
                        int movedRecordsCount = Convert.ToInt32(db.ExecQuery(
                            string.Format("update {0} set {1} = ? where {1} = ?",
                            this.FullDBName, // ��� ��������� ���������
                            DataAttribute.ParentIDColumnName // ��� �������� ����� 
                            ),
                            QueryResultTypes.NonQuery,
                            db.CreateParameter("mainRecordID", mainRecordID),
                            db.CreateParameter("duplicateID", duplicateID)));

                        listener(string.Format("    ����������� ������ � ����� �������: ���������� ������ {0}{1}",
                            movedRecordsCount, Environment.NewLine));
                        moveParentChildMessage =
                            string.Format("� ����� ������� ���������� �������: {0}", movedRecordsCount);
                    }

                    // ���� ������ �� ������ � �������� ����������.
                    foreach (EntityAssociation item in associated.Values)
                    {
                        associatedEntity = (Entity)item.RoleData;
                        // ������������ ������ � ������� ����������.
                        int movedRecordsCount;
                        movedRecordsCount = Convert.ToInt32(db.ExecQuery(
                            string.Format("update {0} set {1} = ? where {1} = ?",
                            associatedEntity.FullDBName, // ��� ��������� ���������
                            item.RoleDataAttribute.Name // ��� �������� ����� 
                            ),
                            QueryResultTypes.NonQuery,
                            db.CreateParameter("mainRecordID", mainRecordID),
                            db.CreateParameter("duplicateID", duplicateID)));

                        // �������� �������, ��� � ������� � ����������� N �������.
                        listener(string.Format("    ������ {0}: ���������� ������ {1}{2}",
                            associatedEntity.FullCaption, movedRecordsCount, Environment.NewLine));

                        allMovedRecordsCount += movedRecordsCount;
                    }
                    // ������� ��������.
                    db.ExecQuery(string.Format("delete from {0} where ID = ?", this.FullDBName),
                                 QueryResultTypes.NonQuery,
                                 db.CreateParameter("duplicateID", duplicateID));
                    // ��������, ��� �������� ������.
                    listener(string.Format("    �������� ID = {0} ������.{1}{1}", duplicateID, Environment.NewLine));
                    duplicatesCount++;
                }
                listener(String.Format("���������� �������� ���������� '{0}', ID �������� ������ = {1}:{2}" +
                "   ���������� {3} ������� � {4} ����������� ��������, ������� {5} ����������.{2}{6}",
                this.FullCaption, mainRecordID, Environment.NewLine,
                allMovedRecordsCount, associated.Values.Count, duplicatesCount, moveParentChildMessage));
                db.Commit();
            }
            catch (Exception ex)
            {
                string exMessage = string.Empty;
                // ���� ���������� �� ��������������� �������
                if (ex.Message.Contains("ORA-20101"))
                {
                    exMessage = string.Format(": � ������� {0} ���������� ������ �� ��������������� �������",
                        associatedEntity.FullCaption);
                }
                // ���� �� ��������������� ��������
                if (ex.Message.Contains("ORA-20102"))
                {
                    exMessage = string.Format(": � ������� {0} ���������� ������ �� ��������������� ��������",
                        associatedEntity.FullCaption);
                }
                listener(string.Format("�� ����� ����������� ���������� ��������� ������{0}", exMessage));
                db.Rollback();
                // ���� ��� ���-�� �����������, �� ������ ������.
                if (exMessage == string.Empty)
                {
                    throw ex;
                }
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        #endregion

        #region �����������
        /*
        private void SetDefaultLookupTypeForAttributes()
        {
            if (Attributes.ContainsKey("Code") || Attributes.ContainsKey("CodeStr"))
            {
                if (Attributes.ContainsKey("Code"))
                    Attributes["Code"].LookupType = LookupAttributeTypes.Primary;
                if (Attributes.ContainsKey("CodeStr"))
                    Attributes["CodeStr"].LookupType = LookupAttributeTypes.Primary;
                if (Attributes.ContainsKey("Name"))
                    Attributes["Name"].LookupType = LookupAttributeTypes.Secondary;
            }
            else
            {
                if (Attributes.ContainsKey("Name"))
                    Attributes["Name"].LookupType = LookupAttributeTypes.Primary;
            }
        }
*/
        /*
                private void ReCreateDependentScripts()
                {
                    Database db = null;
                    try
                    {
                        db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                        db.RunScript(((EntityScriptingEngine)_scriptingEngine).CreateDependentScripts(this, DataAttribute.SystemDummy).ToArray(), false);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("������ ��� �������� ��������� �������� {0}", e.Message);
                    }
                    finally
                    {
                        db.Dispose();
                    }
                }
        */

        /*
                private void ReCreateSequences()
                {
                    if (!(ClassType == ClassTypes.clsBridgeClassifier || ClassType == ClassTypes.clsDataClassifier))
                        return;

                    Database db = null;
                    try
                    {
                        db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                        object result = db.ExecQuery(String.Format(
                                "select max({0}) from {1} where {0} < 1000000000",
                                DataAttribute.IDColumnName, this.FullDBName),
                            QueryResultTypes.Scalar);

                        if (!(result is DBNull))
                        {
                            int seed = (int)result;
                            if (seed > 0)
                                db.RunScript(((EntityScriptingEngine)_scriptingEngine).ReCreateSequences(this, seed + 1).ToArray(), false);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("������ ��� ������������ ������������������ ��� {0}: {1}", this.FullName, e.Message);
                    }
                    finally
                    {
                        db.Dispose();
                    }
                }
        */

        /// <summary>
        /// ����� ���������� ����� ������������� ���� �������� �����
        /// </summary>
        internal override XmlDocument PostInitialize()
        {
            if (needInitializePresentations)
                InitializePresentation(base.PostInitialize());

            if (ID <= 0)
                return null;

            #region �������� ��������� �����������
            if (ID > 0 && ClassType != ClassTypes.clsFixedClassifier)
            {
                Database db = null;
                try
                {
                    db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                    if (!_scriptingEngine.ExistsObject(AuditTriggerName, ObjectTypes.Trigger))
                    {
                        db.RunScript( ((EntityScriptingEngine)_scriptingEngine).GetAuditTriggersScript(this).ToArray(), false);
                    }

                    // db.RunScript(((EntityScriptingEngine)_scriptingEngine).CreateDependentScripts(this, DataAttribute.SystemDummy).ToArray(), false);
                }
                catch (Exception e)
                {
                    Trace.TraceError("������ ��� �������� �������� ������ {0}: {1}", AuditTriggerName, e.Message);
                }
                finally
                {
                    if (db != null) db.Dispose();
                }
            }

            #endregion �������� ��������� �����������

            //ReCreateDependentScripts();
            //ReCreateSequences();
            return null;
        }

        /// <summary>
        /// ����� ������������� �������������, ����� ������������� ���� �������� �������
        /// </summary>
        internal void PostInitializePresentation()
        {
            InitializePresentation(base.PostInitialize());
        }

        #endregion �����������

    }
}