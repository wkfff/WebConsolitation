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

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Класс реализующий ассоциацию между двумя реляционными таблицами
    /// </summary>
    internal abstract class EntityAssociation : CommonDBObject, IEntityAssociation
    {
        // Имя XML элемента, заполняется в потомках
        protected string tagElementName;

        /// <summary>
        /// Поле роли А
        /// </summary>
        private EntityDataAttribute roleDataAttribute;

        /// <summary>
        /// Укороченное наименование аттрибута-ссылки используется для сокращений в базе данных
        /// </summary>
        private string shortName = String.Empty;

        /// <summary>
        /// Класс ассоциации
        /// </summary>
        private AssociationClassTypes associationClassType;

        /// <summary>
        /// Роль содержащая ссылку (внешний ключ)
        /// </summary>
        private Entity roleData;

        /// <summary>
        /// Роль содержащая первичный ключ
        /// </summary>
        private Entity roleBridge;

        /// <summary>
        /// Реакция ассоциации на удаление родительского ключа
        /// </summary>
        internal OnDeleteAction onDeleteAction = OnDeleteAction.None;

        /// <summary>
        /// Инициализация экземпляра.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner">Родительский объект.</param>
        /// <param name="semantic">Семантика.</param>
        /// <param name="name">Наименование.</param>
        /// <param name="associationClassType">Подкласс объекта.</param>
        /// <param name="state">Состояние.</param>
        /// <param name="scriptingEngine">Скриптовой движок.</param>
        public EntityAssociation(string key, ServerSideObject owner, string semantic, string name, AssociationClassTypes associationClassType, ServerSideObjectStates state, ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, semantic, name, state, scriptingEngine)
        {
            this.associationClassType = associationClassType;
            roleDataAttribute = new EntityAssociationAttribute(key, String.Empty, this, state);
        }

        #region Инициализация

        /// <summary>
        /// Инициализации объекта по XML описанию
        /// </summary>
        internal override XmlDocument Initialize()
        {
            //Trace.TraceEvent(TraceEventType.Verbose, "Инициализация объекта {0}", this.FullName);

            XmlDocument doc = base.Initialize();

            XmlNode xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@shortName", tagElementName));
            if (xmlNode != null)
                shortName = xmlNode.Value;

            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@caption", tagElementName));
            if (xmlNode != null)
                this.Caption = xmlNode.Value;

            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/RoleBridge/@default", tagElementName));
            if (xmlNode != null)
                RoleDataAttribute.DefaultValue = Convert.ToInt32(xmlNode.Value);

            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/RoleBridge/@multiplicity", tagElementName));
            if (xmlNode != null)
                MandatoryRoleData = !(xmlNode.Value == "0..*" || xmlNode.Value == "0..1");

            string attributeMask = String.Empty;
            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@mask", tagElementName));
            if (xmlNode != null)
                attributeMask = xmlNode.Value;

            bool attributeVisible = true;
            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@visible", tagElementName));
            if (xmlNode != null)
                attributeVisible = StrUtils.StringToBool(xmlNode.Value);

            bool attributeIsReadOnly = false;
            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@isReadOnly", tagElementName));
            if (xmlNode != null)
                attributeIsReadOnly = StrUtils.StringToBool(xmlNode.Value);

            string lookupObjectName = String.Empty;
            string lookupAttribute = String.Empty;
            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@lookupAttribute", tagElementName));
            if (xmlNode != null)
            {
                lookupAttribute = xmlNode.Value;
                lookupObjectName = RoleB.ObjectKey;
            }

            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@position", tagElementName));
            if (xmlNode != null)
                ((DataAttribute)RoleDataAttribute).PositionSet = Convert.ToInt16(xmlNode.Value);

            xmlNode = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@groupTags", tagElementName));
            if (xmlNode != null)
                RoleDataAttribute.GroupTags = xmlNode.Value;

            LinkObject(attributeMask, lookupAttribute, lookupObjectName, attributeVisible, attributeIsReadOnly);

            return doc;
        }

        /// <summary>
        /// Установка взаимных ссылок.
        /// </summary>
        /// <param name="attributeMask">Маска для атрибута роли A используемого в качестве ссылка.</param>
        /// <param name="lookupAttribute">Наименование атрибута роли B который будет использоваться в качестве lookup-а.</param>
        /// <param name="lookupObjectName">Полное наименование объекта роли B.</param>
        /// <param name="attributeVisible">Видимость атрибута.</param>
        internal void LinkObject(string attributeMask, string lookupAttribute, string lookupObjectName, bool attributeVisible, bool attributeIsReadOnly)
        {
            if (!this.RoleA.Associations.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                this.RoleA.Associations.Add(GetKey(this.ObjectKey, this.FullName), this);
            else
                throw new Exception(String.Format("Объект уже содержит ассоциацию {0}.", this.FullName));

            // В режиме редактирования структуры пользователем не обновляем коллекцию родительского объекта
            // коллекция будет изменена при применении изменений
            if (this.Parent.Name == this.RoleB.Parent.Name || Authentication.IsSystemRole())
            {
                if (!this.RoleB.Associated.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                    this.RoleB.Associated.Add(GetKey(this.ObjectKey, this.FullName), this);
                else
                    throw new Exception(String.Format("Объект уже содержит ассоциацию {0}.", this.FullName));
            }

            roleDataAttribute.Name = FullDBName;
            roleDataAttribute.Caption = this.Caption;
            roleDataAttribute.Type = DataAttributeTypes.dtInteger;
            roleDataAttribute.Size = 10;
            roleDataAttribute.Class = DataAttributeClassTypes.Reference;
            //roleDataAttribute.IsNullable = !mandatoryBridge;
            //roleDataAttribute.DefaultValue = defaultBridgeID;
            roleDataAttribute.Mask = attributeMask;
            roleDataAttribute.LookupAttributeName = lookupAttribute;
            roleDataAttribute.LookupObjectName = lookupObjectName;
            roleDataAttribute.Visible = attributeVisible;
            roleDataAttribute.IsReadOnly = attributeIsReadOnly;

            // Ссылки на сопоставимые классыфикаторы являются системными полями
            if (AssociationClassType == AssociationClassTypes.Bridge ||
                AssociationClassType == AssociationClassTypes.MasterDetail ||
                AssociationClassType == AssociationClassTypes.BridgeBridge)
            {
                roleDataAttribute.Kind = DataAttributeKindTypes.Sys;
            }
            else
            {
                roleDataAttribute.Kind = DataAttributeKindTypes.Regular;
            }

            RoleA.Attributes.Add(roleDataAttribute);
            return;
        }

        #endregion Инициализация

        #region Сохранение

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            //
            // Основные свойства
            //

            XmlHelper.SetAttribute(node, "name", FullDBName);

            if (ShortName != FullDBName)
                XmlHelper.SetAttribute(node, "shortName", ShortName);

            if (!String.IsNullOrEmpty(Caption))
                XmlHelper.SetAttribute(node, "caption", Caption);

            if (!String.IsNullOrEmpty(RoleDataAttribute.Mask))
                XmlHelper.SetAttribute(node, "mask", RoleDataAttribute.Mask);

            if (!String.IsNullOrEmpty(RoleDataAttribute.LookupAttributeName))
                XmlHelper.SetAttribute(node, "lookupAttribute", RoleDataAttribute.LookupAttributeName);

            if (!RoleDataAttribute.Visible)
                XmlHelper.SetAttribute(node, "visible", "false");
            if (IsReadOnly)
                XmlHelper.SetAttribute(node, "isReadOnly", "true");
            if (RoleDataAttribute.Position != 0)
                XmlHelper.SetAttribute(node, "position", Convert.ToString(RoleDataAttribute.Position));
            if (!String.IsNullOrEmpty(RoleDataAttribute.GroupTags))
                XmlHelper.SetAttribute(node, "groupTags", RoleDataAttribute.GroupTags);

            //
            // Роль A
            //

            XmlNode xmlRole = node.OwnerDocument.CreateNode(XmlNodeType.Element, "RoleData", null);

            XmlHelper.SetAttribute(xmlRole, "objectKey", GetKey(RoleA.ObjectKey, RoleA.FullName));
            XmlHelper.SetAttribute(xmlRole, "class", RoleA.TablePrefix);
            XmlHelper.SetAttribute(xmlRole, "semantic", RoleA.Semantic);
            XmlHelper.SetAttribute(xmlRole, "name", RoleA.Name);

            node.AppendChild(xmlRole);

            //
            // Роль B
            //

            xmlRole = node.OwnerDocument.CreateNode(XmlNodeType.Element, "RoleBridge", null);

            XmlHelper.SetAttribute(xmlRole, "objectKey", GetKey(RoleB.ObjectKey, RoleB.FullName));
            XmlHelper.SetAttribute(xmlRole, "class", RoleB.TablePrefix);
            XmlHelper.SetAttribute(xmlRole, "semantic", RoleB.Semantic);
            XmlHelper.SetAttribute(xmlRole, "name", RoleB.Name);

            // по умолчания mandatoryBridge = false
            if (MandatoryRoleData)
                XmlHelper.SetAttribute(xmlRole, "multiplicity", "1..*");
            else
                XmlHelper.SetAttribute(xmlRole, "multiplicity", "0..*");

            if (!String.IsNullOrEmpty(Convert.ToString(RoleDataAttribute.DefaultValue)))
                XmlHelper.SetAttribute(xmlRole, "default", Convert.ToString(RoleDataAttribute.DefaultValue));

            node.AppendChild(xmlRole);
        }

        internal override void Save2XmlDocumentation(XmlNode node)
        {
            Save2Xml(node);

            // Специфичные для документации свойства документа
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

        #endregion Сохранение

        /// <summary>
        /// Устанавливает английское наименование
        /// </summary>
        /// <param name="oldValue">Старое наименование</param>
        /// <param name="value">Новое наименование</param>
        protected override void SetFullName(string oldValue, string value)
        {
            if (State == ServerSideObjectStates.New)
            {
                Package package = (Package)Owner;

                EntityAssociation newObject = Instance;
                EntityAssociation oldObject = package.Associations[oldValue] as EntityAssociation;
                try
                {
                    package.Associations.Remove(oldValue);
                    package.Associations.Add(newObject.FullName, newObject);
                }
                catch (Exception e)
                {
                    package.Associations.Add(oldValue, oldObject);
                    throw new Exception(e.Message, e);
                }
            }
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new EntityAssociation Instance
        {
            [DebuggerStepThrough]
            get { return (EntityAssociation)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new EntityAssociation SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (EntityAssociation)CloneObject;
                else
                    return this;
            }
        }

        public override IServerSideObject Lock()
        {
            Package clonePackage = (Package)Owner.Lock();
            return (ServerSideObject)clonePackage.Associations[this.ObjectKey];
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра. 
        /// </summary>
        /// <returns>Новый объект, являющийся копией текущего экземпляра.</returns>
        public override Object Clone()
        {
            EntityAssociation clon = (EntityAssociation)base.Clone();
            clon.roleDataAttribute = (EntityAssociationAttribute)roleDataAttribute.Clone();
            return clon;
        }

        #endregion ServerSideObject

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            UpdateMajorObjectModificationItem root = (UpdateMajorObjectModificationItem)base.GetChanges(toObject);

            EntityAssociation toAssociation = (EntityAssociation)toObject;

            //
            // Основные свойства
            //

            if (this.FullDBName != toAssociation.FullDBName)
                throw new Exception("Свойство name недоступно для изменения.");

            if (this.ShortName != toAssociation.ShortName)
            {
                ModificationItem item = new PropertyModificationItem("ShortName", this.ShortName, toAssociation.ShortName, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Caption != toAssociation.Caption)
            {
                ModificationItem item = new PropertyModificationItem("Caption", this.Caption, toAssociation.Caption, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Visible != toAssociation.Visible)
            {
                ModificationItem item = new PropertyModificationItem("Visible", this.Visible, toAssociation.Visible, root);
                root.Items.Add(item.Key, item);
            }

            if (this.AttributeMask != toAssociation.AttributeMask)
            {
                ModificationItem item = new PropertyModificationItem("AttributeMask", this.AttributeMask, toAssociation.AttributeMask, root);
                root.Items.Add(item.Key, item);
            }

            if (this.LookupAttribute != toAssociation.LookupAttribute)
            {
                ModificationItem item = new PropertyModificationItem("LookupAttribute", this.LookupAttribute, toAssociation.LookupAttribute, root);
                root.Items.Add(item.Key, item);
            }

            if (this.RoleDataAttribute.Position != toAssociation.RoleDataAttribute.Position)
            {
                ModificationItem item = new PropertyModificationItem("Position", this.RoleDataAttribute.Position, toAssociation.RoleDataAttribute.Position, root);
                root.Items.Add(item.Key, item);
            }

            if (this.RoleDataAttribute.GroupTags != toAssociation.RoleDataAttribute.GroupTags)
            {
                ModificationItem item = new PropertyModificationItem("GroupTags", this.RoleDataAttribute.GroupTags, toAssociation.RoleDataAttribute.GroupTags, root);
                root.Items.Add(item.Key, item);
            }

            //
            // Роль A
            //

            if (this.RoleA.FullName != toAssociation.RoleA.FullName)
            {
                ModificationItem item = new InapplicableModificationItem("Свойство RoleData недоступно для изменения", this.RoleA, toAssociation.RoleA, root);
                root.Items.Add(item.Key, item);
            }

            //
            // Роль B
            //

            if (this.RoleB.FullName != toAssociation.RoleB.FullName)
            {
                ModificationItem item = new InapplicableModificationItem("Свойство RoleBridge недоступно для изменения", this.RoleB, toAssociation.RoleB, root);
                root.Items.Add(item.Key, item);
            }

            if (this.MandatoryRoleData != toAssociation.MandatoryRoleData)
            {
                ModificationItem item = new PropertyModificationItem("MandatoryRoleData", this.MandatoryRoleData, toAssociation.MandatoryRoleData, root);
                root.Items.Add(item.Key, item);
            }

            if (this.DefaultRoleDataID != toAssociation.DefaultRoleDataID)
            {
                ModificationItem item = new PropertyModificationItem("DefaultRoleDataID", this.DefaultRoleDataID, toAssociation.DefaultRoleDataID, root);
                root.Items.Add(item.Key, item);
            }

            if (IsReadOnly != toAssociation.IsReadOnly)
            {
                ModificationItem item = new PropertyModificationItem("IsReadOnly", IsReadOnly,
                                                                     toAssociation.IsReadOnly, root);
                root.Items.Add(item.Key, item);
            }

            return root;
        }

        #region Обновление

        /// <summary>
        /// Для создаваемой ассоциации находит сущьность для роли А в объектной модели
        /// </summary>
        /// <returns></returns>
        private Entity FindRoleDataEntity()
        {
            Entity roleDataEntity;

            IEntity entity = EntityCollection<IEntity>.GetByKeyName(Parent.Classes, this.RoleData.ObjectKey, this.RoleData.FullName);
            if (entity != null)
            {
                roleDataEntity = (Entity)entity;
            }
            else
            {
                roleDataEntity =
                    (Entity)
                    ((Package)SchemeClass.Instance.RootPackage).FindEntityByKeyName(
                        this.RoleData.ObjectKey, this.RoleData.FullName);
            }

            if (roleDataEntity == null)
                throw new Exception(String.Format("Не найден объект RoleData={0}", this.RoleData.FullName));

            return roleDataEntity;
        }

        /// <summary>
        /// Для создаваемой ассоциации находит сущьность для роли В в объектной модели
        /// </summary>
        /// <returns></returns>
        private Entity FindRoleBridgeEntity()
        {
            Entity roleBridgeEntity;

            IEntity entity = EntityCollection<IEntity>.GetByKeyName(Parent.Classes, this.RoleBridge.ObjectKey, this.RoleBridge.FullName);
            if (entity != null)
            {
                roleBridgeEntity = (Entity)entity;
            }
            else
            {
                roleBridgeEntity =
                    (Entity)((Package)SchemeClass.Instance.RootPackage).FindEntityByKeyName(
                        this.RoleBridge.ObjectKey, this.RoleBridge.FullName);
            }

            if (roleBridgeEntity == null)
                throw new Exception(String.Format("Не найден объект RoleBridge={0}", this.RoleBridge.FullName));

            return roleBridgeEntity;
        }

        private void InsertMetaData(ModificationContext context, Entity roleDataEntity, Entity roleBridgeEntity)
        {
            Database db = context.Database;

            int? PackageID = Parent.ID;
            if (PackageID == null)
                throw new Exception(String.Format("У объекта {0} указан несуществующий пакет.", FullName));

            // Проверка на уникальность
            long count = Convert.ToInt64(db.ExecQuery("select Count(*) from MetaLinks where Semantic = ? and Name = ?",
                QueryResultTypes.Scalar,
                db.CreateParameter("Semantic", this.Semantic),
                db.CreateParameter("Name", this.Name)));
            if (count > 0)
                throw new Exception("Ассоциация " + this.FullName + " уже определена.");

            // получаем значение генератора
            if (this.ID <= 0)
                this.ID = db.GetGenerator("g_MetaLinks");

            db.ExecQuery("insert into MetaLinks (ID, ObjectKey, Semantic, Name, Class, RefParent, RefChild, Configuration, RefPackages) values (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                QueryResultTypes.NonQuery,
                db.CreateParameter("ID", this.ID),
                db.CreateParameter("ObjectKey", this.ObjectKey),
                db.CreateParameter("Semantic", this.Semantic),
                db.CreateParameter("Name", this.Name),
                db.CreateParameter("Class", (int)this.AssociationClassType),
                db.CreateParameter("RefParent", roleDataEntity.ID),
                db.CreateParameter("RefChild", roleBridgeEntity.ID),
                db.CreateParameter("Config", this.ConfigurationXml, DbType.AnsiString),
                db.CreateParameter("RefPackages", PackageID));
        }

        /// <summary>
        /// Записывает в каталог метаданных информацию о реляционной таблице
        /// </summary>
        protected void RemoveMetaData(ModificationContext context)
        {
            Database db = context.Database;

            int? PackageID = Parent.ID;
            if (PackageID == null)
                throw new Exception(String.Format("У объекта {0} указан несуществующий пакет.", FullName));

            db.ExecQuery("delete from MetaLinks where ID = ?", QueryResultTypes.NonQuery,
                db.CreateParameter("ID", this.ID));
        }

        /// <summary>
        /// Создание метаданных в базе данных и кубах
        /// </summary>
        internal override void Create(ModificationContext context)
        {
            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.TraceInformation("Создание {0} {1}", this, DateTime.Now);

                base.Create(context);

                Entity roleDataEntity = FindRoleDataEntity();
                Entity roleBridgeEntity = FindRoleBridgeEntity();

                // Добавляем ассоциацию в объектную модель
                {
                    if (DataAttributeCollection.GetAttributeByKeyName(roleDataEntity.Attributes, this.RoleDataAttribute.ObjectKey, this.RoleDataAttribute.Name) == null)
                        roleDataEntity.Attributes.Add(this.RoleDataAttribute);

                    if (!roleDataEntity.Associations.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                        roleDataEntity.Associations.Add(GetKey(this.ObjectKey, this.FullName), this);

                    if (!roleBridgeEntity.Associated.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                        roleBridgeEntity.Associated.Add(GetKey(this.ObjectKey, this.FullName), this);

                    this.RoleA = roleDataEntity;

                    this.RoleB = roleBridgeEntity;

                    if (!Parent.Associations.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                        Parent.Associations.Add(GetKey(this.ObjectKey, this.FullName), this);
                }

                try
                {
                    SchemeClass.Instance.DDLDatabase.RunScript(DropScript(), true);
                }
                catch (Exception e)
                {
                    Trace.TraceWarning("При выполнении скрипта удаления произошла ошибка: {0}", e.Message);
                }

                try
                {
                    // добавляем фиксированныезаписи по источникам
                    if (RoleA is DataSourceDividedClass && RoleB is DataClassifier)
                    {
                        ((DataSourceDividedClass)RoleB).FillDataSourceFixedRows(
                            context.Database,
                            ((DataSourceDividedClass)RoleA).GetDataSourcesID(context.Database));
                    }

                    // Создаем структуры в базе
                    SchemeClass.Instance.DDLDatabase.RunScript(CreateScript());
                    this.Unlock();
                    this.DbObjectState = DBObjectStateTypes.InDatabase;
                    this.roleDataAttribute.State = ServerSideObjectStates.Consistent;

                    InsertMetaData(context, roleDataEntity, roleBridgeEntity);
                }
                catch (Exception e)
                {
                    { // Откатываем изменения
                        if (Parent.Associations.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                            Parent.Associations.Remove(GetKey(this.ObjectKey, this.FullName));
                        if (roleBridgeEntity.Associated.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                            roleBridgeEntity.Associated.Remove(GetKey(this.ObjectKey, this.FullName));
                        if (roleDataEntity.Associations.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                            roleDataEntity.Associations.Remove(GetKey(this.ObjectKey, this.FullName));
                        if (roleDataEntity.Attributes.ContainsKey(this.RoleDataAttribute.Name))
                            roleDataEntity.Attributes.Remove(this.RoleDataAttribute.Name);
                        this.roleData = null;
                        this.roleBridge = null;
                    }
                    Trace.WriteLine(e.Message);
                    throw new Exception(e.Message, e);
                }
            }
            finally
            {
                InUpdating = updatingState;
            }
        }

        /// <summary>
        /// Удаление объекта из базы и метаданных
        /// </summary>
        internal override void Drop(ModificationContext context)
        {
            if (this.DbObjectState != DBObjectStateTypes.InDatabase)
                return;

            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.WriteLine(String.Format("Удаление объекта {0}", this));

                try
                {
                    SchemeClass.Instance.DDLDatabase.RunScript(DropScript(), false);

                    RemoveMetaData(context);

                    this.RoleA.Attributes.Remove(GetKey(this.ObjectKey, this.FullDBName));
                    this.RoleA.Associations.Remove(GetKey(this.ObjectKey, this.FullName));
                    this.RoleB.Associated.Remove(GetKey(this.ObjectKey, this.FullName));
                    this.Parent.Associations.Remove(GetKey(this.ObjectKey, this.FullName));

                    this.DbObjectState = DBObjectStateTypes.Unknown;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    throw new Exception(e.Message, e);
                }

                base.Drop(context);
            }
            finally
            {
                InUpdating = updatingState;
            }
        }

        /// <summary>
        /// Сохранение XML-конфигурации в базе данных
        /// </summary>
        internal override void SaveConfigurationToDatabase(ModificationContext context)
        {
            IDatabase db = context.Database;

            string confXmlString = ConfigurationXml;
            confXmlString = confXmlString.Replace("<DatabaseConfiguration xmlns=\"xmluml\">", "<DatabaseConfiguration>");

            // вставляем запись в каталог метаданных
            db.ExecQuery(
                "update MetaLinks set Configuration = ?, ObjectKey = ? where ID = ?",
                QueryResultTypes.NonQuery,
                db.CreateParameter("Config", confXmlString, DbType.AnsiString),
                db.CreateParameter("ObjectKey", ObjectKey, DbType.AnsiString),
                db.CreateParameter("ID", this.ID));

            this.Configuration = confXmlString;
        }

        /// <summary>
        /// Применение изменений. Приводит текущий объект к виду объекта toObject
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toObject">Объект к виду которого будет приведен текущий объект</param>
        public override void Update(ModificationContext context, IModifiable toObject)
        {
            base.Update(context, toObject);

            EntityAssociation toAssociation = (EntityAssociation)toObject;

            if (this.Caption != toAssociation.Caption)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилось русское наименование ссылки c \"{1}\" на \"{2}\"",
                    FullName, Caption, toAssociation.Caption));
                Caption = toAssociation.Caption;
            }

            if (this.AttributeMask != toAssociation.AttributeMask)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилась маска ссылки c \"{1}\" на \"{2}\"",
                    FullName, AttributeMask, toAssociation.AttributeMask));
                AttributeMask = toAssociation.AttributeMask;
            }

            bool needUpdateAttribute = false;

            if (this.MandatoryRoleData != toAssociation.MandatoryRoleData)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилось обязательность заполнения ссылки c \"{1}\" на \"{2}\"",
                    FullName, MandatoryRoleData, toAssociation.MandatoryRoleData));
                needUpdateAttribute = true;
            }

            if (this.DefaultRoleDataID != toAssociation.DefaultRoleDataID)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилось значение по умолчанию c \"{1}\" на \"{2}\"",
                    FullName, DefaultRoleDataID, toAssociation.DefaultRoleDataID));
                needUpdateAttribute = true;
            }

            if (needUpdateAttribute)
            {
                // добавляем фиксированныезаписи по источникам
                if (RoleA is DataSourceDividedClass && RoleB is DataSourceDividedClass)
                {
                    ((DataSourceDividedClass)RoleB).FillDataSourceFixedRows(
                        context.Database,
                        ((DataSourceDividedClass)RoleA).GetDataSourcesID(context.Database));
                }

                this.RoleA.UpdateAttribute(context, (EntityAssociationAttribute)toAssociation.RoleDataAttribute);
            }
        }

        #endregion Обновление

        #region Работа с DDL

        /// <summary>
        /// Возвращает скрипт для создания объекта в базе данных
        /// </summary>
        public string[] CreateScript()
        {
            return _scriptingEngine.CreateScript(this).ToArray();
        }

        /// <summary>
        /// Возвращает скрипт для удаления объекта из базы данных
        /// </summary>
        public string[] DropScript()
        {
            return _scriptingEngine.DropScript(this).ToArray();
        }

        /// <summary>
        /// Возвращает SQL-определение объекта.
        /// </summary>
        /// <returns>SQL-определение объекта.</returns>
        public override List<string> GetSQLDefinitionScript()
        {
            List<string> script = base.GetSQLDefinitionScript();
            script.AddRange(_scriptingEngine.CreateScript(this));
            return script;
        }

        #endregion Работа со DDL

        #region Права

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            return true;
        }

        #endregion Права

        #region Свойства

        [DebuggerStepThrough]
        public override string ToString()
        {
            return String.Format("{0} : {1}", FullName, base.ToString());
        }

        public override string FullName
        {
            [DebuggerStepThrough]
            get { return String.Format("a.{0}.{1}", base.Semantic, base.Name); }
        }

        /// <summary>
        /// Возвращает имя ссылки в таблице(вьюшке) БД
        /// </summary>
        public override string FullDBName
        {
            [DebuggerStepThrough]
            get { return Name.Split('.')[1]; }
        }

        /// <summary>
        /// Русское наименование ассоциации
        /// </summary>
        public override string Caption
        {
            [DebuggerStepThrough]
            get { return RoleDataAttribute.Caption; }
            [DebuggerStepThrough]
            set { RoleDataAttribute.Caption = value; }
        }

        /// <summary>
        /// Полное русское наименование объекта, состоящее из семантического имени и названия объекта разделенных точкой
        /// </summary>
        public string FullCaption
        {
            //[DebuggerStepThrough]
            get
            {
                string RoleAName = this.RoleA.FullCaption;
                string RoleBName = this.RoleB.FullCaption;
                return RoleAName + " -> " + RoleBName;
            }
        }

        /// <summary>
        /// Укороченное наименование аттрибута-ссылки используется для сокращений в базе данных
        /// </summary>
        public string ShortName
        {
            get { return shortName == String.Empty ? FullDBName : shortName; }
        }

        /// <summary>
        /// Класс ассоциации
        /// </summary>
        public AssociationClassTypes AssociationClassType
        {
            [DebuggerStepThrough]
            get { return associationClassType; }
            [DebuggerStepThrough]
            set { associationClassType = value; }
        }

        /// <summary>
        /// Обязательность роли справочника
        /// </summary>
        public bool MandatoryRoleData
        {
            [DebuggerStepThrough]
            get { return !RoleDataAttribute.IsNullable; }
            [DebuggerStepThrough]
            set { RoleDataAttribute.IsNullable = !value; }
        }

        /// <summary>
        /// Значение по умолчанию для внешнего ключа.
        /// Если null, то значения по умолчанию нет.
        /// </summary>
        public int? DefaultRoleDataID
        {
            [DebuggerStepThrough]
            get { return String.IsNullOrEmpty(Convert.ToString(RoleDataAttribute.DefaultValue)) ? (int?)null : Convert.ToInt32(RoleDataAttribute.DefaultValue); }
            set { RoleDataAttribute.DefaultValue = value; }
        }

        /// <summary>
        /// Объект который является ролью А в ассоциации
        /// </summary>
        internal Entity RoleA
        {
            [DebuggerStepThrough]
            get { return roleData; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                roleData = value;
            }
        }

        /// <summary>
        /// Объект который является ролью А в ассоциации
        /// </summary>
        internal Entity RoleB
        {
            [DebuggerStepThrough]
            get { return roleBridge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                roleBridge = value;
            }
        }

        /// <summary>
        /// Объект который является ролью А в ассоциации
        /// </summary>
        public IEntity RoleData
        {
            [DebuggerStepThrough]
            get { return roleData; }
        }

        /// <summary>
        /// Объект который является ролью B в ассоциации
        /// </summary>
        public IEntity RoleBridge
        {
            [DebuggerStepThrough]
            get { return roleBridge; }
        }

        /// <summary>
        /// Определяет видимость атрибута-ссылки.
        /// </summary>
        public bool Visible
        {
            [DebuggerStepThrough]
            get { return RoleDataAttribute.Visible; }
            set { RoleDataAttribute.Visible = value; }
        }

        /// <summary>
        /// Маска для атрибута роли B используемого в качестве lookup-а
        /// </summary>
        public string AttributeMask
        {
            [DebuggerStepThrough]
            get { return RoleDataAttribute.Mask; }
            set { RoleDataAttribute.Mask = value; }
        }

        /// <summary>
        /// Позиция атрибута
        /// </summary>
        public int Position
        {
            [DebuggerStepThrough]
            get { return RoleDataAttribute.Position; }
            set { RoleDataAttribute.Position = value; }
        }

        /// <summary>
        /// Тэги для группировки атрибутов
        /// </summary>
        public string GroupTags
        {
            [DebuggerStepThrough]
            get { return RoleDataAttribute.GroupTags; }
            set { RoleDataAttribute.GroupTags = value; }
        }

        /// <summary>
        /// Значение только для чтения для атрибута-ссылки
        /// </summary>
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get { return RoleDataAttribute.IsReadOnly; }
            set { RoleDataAttribute.IsReadOnly = value; }
        }

        /// <summary>
        /// Наименование атрибута роли B который будет использоваться в качестве lookup-а
        /// </summary>
        public string LookupAttribute
        {
            [DebuggerStepThrough]
            get { return RoleDataAttribute.LookupAttributeName; }
            set { ((DataAttribute)RoleDataAttribute).LookupAttributeName = value; }
        }

        /// <summary>
        /// Поле роли А
        /// </summary>
        public IDataAttribute RoleDataAttribute
        {
            [DebuggerStepThrough]
            get { return Instance.roleDataAttribute; }
        }

        #endregion Свойства
    }
}