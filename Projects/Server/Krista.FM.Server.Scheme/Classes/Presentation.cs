using System;
using System.Collections.Generic;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Scheme.Modifications;
using System.Diagnostics;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Представление объекта сервера
    /// </summary>
    internal class Presentation : MinorObject, IPresentation
    {
        #region Fields

        /// <summary>
        /// Имя представления
        /// </summary>
        private string name;

        /// <summary>
        /// Коллекция атрибутов представления
        /// </summary>
        private DataAttributeCollection attributes;

        /// <summary>
        /// Наименование уровней иерархии
        /// </summary>
        private string levelNamingTemplate;

        /// <summary>
        /// Xml-конфигурация представления
        /// </summary>
        private readonly string configuration;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Конструктор для создания представления из мастера создания представлений
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="levelNamingTemplate"></param>
        /// <param name="owner"></param>
        /// <param name="state"></param>
        public Presentation(string key, string name, List<IDataAttribute> _attributes,
            string levelNamingTemplate, ServerSideObject owner, ServerSideObjectStates state)
            : this(key, owner, state)
        {
            this.name = name;

            this.attributes = new DataAttributeCollection((Entity)owner, state);
            this.attributes.Owner = this;

            foreach (DataAttribute attr in _attributes)
            {
                InitializeAttribute(attr);
                if (attributes.ContainsKey(attr.ObjectKey))
                    attributes[attr.ObjectKey].Visible = attr.Visible;
            }

            this.levelNamingTemplate = levelNamingTemplate;

            configuration = ConfigurationXml;
        }

        /// <summary>
        /// Конструктор для представления объекта сервера
        /// </summary>
        /// <param name="key">Идентификатор представления</param>
        /// <param name="owner">Объект сервера, содержащий данное представление</param>
        /// <param name="state">Состояние серверного объекта</param>
        public Presentation(string key, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, owner, state)
        {
            attributes = new DataAttributeCollection((Entity)owner, state);
        }

        /// <summary>
        /// Конструктор для представления объекта сервера
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="xmlPresentation"></param>
        /// <param name="state"></param>
        public Presentation(string key, ServerSideObject owner, string name, XmlNode xmlPresentation, ServerSideObjectStates state)
            : this(key, owner, state)
        {
            this.name = name;

            this.configuration = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><DatabaseConfiguration>" + xmlPresentation.InnerXml + "</DatabaseConfiguration>";

            Initialize(xmlPresentation);
        }

        #endregion Constructor

        #region IPresentation Members

        /// <summary>
        /// Коллекция атрибутов у представления
        /// </summary>
        public IDataAttributeCollection Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Коллекция сгруппированных атрибутов
        /// </summary>
        public IDataAttributeCollection GroupedAttributes
        {
            get
            {
                return new GroupedAttributeCollection((Entity)Owner, state, Attributes);
            }
        }

        /// <summary>
        /// Наименование уровней иерархии представления
        /// </summary>
        public string LevelNamingTemplate
        {
            get { return levelNamingTemplate; }
            set { levelNamingTemplate = value; }
        }

        public override string ObjectOldKeyName
        {
            get { return Name; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Xml-конфигурация представления
        /// </summary>
        public string Configuration
        {
            get { return configuration; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Инициализация из xml-описания
        /// </summary>
        /// <param name="xmlPresentation"></param>
        internal void Initialize(XmlNode xmlPresentation)
        {
            // инициализация атрибутов
            InitializeAttributes(xmlPresentation);

            // инициализация имен уровней иерархии
            InitializeLevels(xmlPresentation);
        }

        /// <summary>
        /// Инициализация уровней иерархиии по xml-описанию
        /// </summary>
        private void InitializeLevels(XmlNode xmlLevel)
        {
            XmlNode presentationHierarchyNode = xmlLevel.SelectSingleNode("PresentationHierarchy");
            if (presentationHierarchyNode != null)
                levelNamingTemplate = presentationHierarchyNode.Attributes["levelNamingTemplate"].InnerText;
        }

        /// <summary>
        /// Инициализация атрибутов представления по xml-описанию,
        /// находми атрибут в списке атрибутов объекта
        /// </summary>
        /// <param name="xmlPresentation"></param>
        private void InitializeAttributes(XmlNode xmlPresentation)
        {
            try
            {
                attributes.Owner = this;

                foreach (DataAttribute attribute in ((IEntity)Owner).Attributes.Values)
                    InitializeAttribute(attribute);

                // настройка отличных от оригинальных свойств атрибутов
                XmlNodeList xmlAttributes =
                    xmlPresentation.SelectNodes(String.Format("PresentationAttributes/PresentationAttribute"));
                foreach (XmlNode xmlAttribute in xmlAttributes)
                {
                    if (this.attributes.ContainsKey(xmlAttribute.Attributes["objectKey"].Value))
                    {
                        //
                        // размерность
                        //
                        if (xmlAttribute.Attributes["size"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].Size =
                                Convert.ToInt32(xmlAttribute.Attributes["size"].Value);
                        }
                        //
                        // точность
                        //
                        if (xmlAttribute.Attributes["scale"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].Scale =
                                Convert.ToInt32(xmlAttribute.Attributes["scale"].Value);
                        }
                        //
                        // видимость атрибута
                        //
                        if (xmlAttribute.Attributes["visible"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].Visible =
                                Convert.ToBoolean(xmlAttribute.Attributes["visible"].Value);
                        }
                        //
                        // маска отображения
                        //
                        if (xmlAttribute.Attributes["mask"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].Mask =
                                xmlAttribute.Attributes["mask"].Value;
                        }
                        //
                        // маска расщепления
                        //
                        if (xmlAttribute.Attributes["divide"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].Divide =
                                xmlAttribute.Attributes["divide"].Value;
                        }
                        //
                        // заголовок
                        //
                        if (xmlAttribute.Attributes["caption"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].Caption =
                                xmlAttribute.Attributes["caption"].Value;
                        }
                        //
                        // lookup
                        //
                        if (xmlAttribute.Attributes["lookupType"] != null)
                            attributes[xmlAttribute.Attributes["objectKey"].Value].LookupType = (LookupAttributeTypes)Convert.ToInt32(xmlAttribute.Attributes["lookupType"].Value);
                        else attributes[xmlAttribute.Attributes["objectKey"].Value].LookupType = LookupAttributeTypes.None;

                        //
                        // позиция
                        //
                        if (xmlAttribute.Attributes["position"] != null)
                        {
                            ((DataAttribute)attributes[xmlAttribute.Attributes["objectKey"].Value]).PositionSet =
                                Convert.ToInt32(xmlAttribute.Attributes["position"].Value);
                        }
                        //
                        // теги для группировки
                        //
                        if (xmlAttribute.Attributes["groupTags"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].GroupTags =
                                Convert.ToString(xmlAttribute.Attributes["groupTags"].Value);
                        }

                        //
                        // только для чтения
                        //
                        if (xmlAttribute.Attributes["isReadOnly"] != null)
                        {
                            attributes[xmlAttribute.Attributes["objectKey"].Value].IsReadOnly =
                                Convert.ToBoolean(xmlAttribute.Attributes["isReadOnly"].Value);
                        }

                        if (xmlAttribute.Attributes["nullable"] != null)
                            attributes[xmlAttribute.Attributes["objectKey"].Value].IsNullable = StrUtils.StringToBool(xmlAttribute.Attributes["nullable"].Value);
                        else
                            attributes[xmlAttribute.Attributes["objectKey"].Value].IsNullable = false;
                    }
                }
                // настройка отличных от оригинальных свойств атрибутов ссылок
                XmlNodeList xmlAttributesReference =
                    xmlPresentation.SelectNodes(
                        String.Format("PresentationAttributesReference/PresentationAttributeReference"));
                if (xmlAttributesReference != null)
                    foreach (XmlNode xmlAttributeReference in xmlAttributesReference)
                    {
                        if (this.attributes.ContainsKey(xmlAttributeReference.Attributes["objectKey"].Value))
                        {

                            //
                            // видимость атрибута
                            //
                            if (xmlAttributeReference.Attributes["visible"] != null)
                            {
                                attributes[xmlAttributeReference.Attributes["objectKey"].Value].Visible =
                                    Convert.ToBoolean(xmlAttributeReference.Attributes["visible"].Value);
                            }
                            //
                            // заголовок
                            //
                            if (xmlAttributeReference.Attributes["caption"] != null)
                            {
                                attributes[xmlAttributeReference.Attributes["objectKey"].Value].Caption =
                                    xmlAttributeReference.Attributes["caption"].Value;
                            }
                            //
                            // позиция
                            //
                            if (xmlAttributeReference.Attributes["position"] != null)
                            {
                                ((DataAttribute)attributes[xmlAttributeReference.Attributes["objectKey"].Value]).PositionSet =
                                    Convert.ToInt32(xmlAttributeReference.Attributes["position"].Value);
                            }
                            //
                            // теги для группировки
                            //
                            if (xmlAttributeReference.Attributes["groupTags"] != null)
                            {
                                attributes[xmlAttributeReference.Attributes["objectKey"].Value].GroupTags =
                                    Convert.ToString(xmlAttributeReference.Attributes["groupTags"].Value);
                            }
                            //
                            // lookup
                            //
                            if (xmlAttributeReference.Attributes["lookupType"] != null)
                                attributes[xmlAttributeReference.Attributes["objectKey"].Value].LookupType = (LookupAttributeTypes)Convert.ToInt32(xmlAttributeReference.Attributes["lookupType"].Value);
                            else attributes[xmlAttributeReference.Attributes["objectKey"].Value].LookupType = LookupAttributeTypes.None;

                            //
                            // теги для группировки
                            //
                            if (xmlAttributeReference.Attributes["isReadOnly"] != null)
                            {
                                attributes[xmlAttributeReference.Attributes["objectKey"].Value].IsReadOnly =
                                    Convert.ToBoolean(xmlAttributeReference.Attributes["isReadOnly"].Value);
                            }

                            if (xmlAttributeReference.Attributes["nullable"] != null)
                                attributes[xmlAttributeReference.Attributes["objectKey"].Value].IsNullable = StrUtils.StringToBool(xmlAttributeReference.Attributes["nullable"].Value);
                            else
                                attributes[xmlAttributeReference.Attributes["objectKey"].Value].IsNullable = false;
                        }
                    }
            }
            catch (InvalidCastException e)
            {
                throw new InitializeXmlException(e.Message, e);
            }
            catch (ArgumentException e)
            {
                throw new InitializeXmlException(e.Message, e);
            }
            catch (OverflowException e)
            {
                throw new InitializeXmlException(e.Message, e);
            }
            catch (FormatException e)
            {
                throw new InitializeXmlException(e.Message, e);
            }
        }

        private void InitializeAttribute(DataAttribute attribute)
        {
            DataAttribute newAttribute;
            if (attribute.Class != DataAttributeClassTypes.Fixed && attribute.Class != DataAttributeClassTypes.System)
            {
                newAttribute = attribute.CloneStatic();
                newAttribute.Owner = this;
                newAttribute.GroupParentAttribute = "test";
                // создаем копии пользовательских и ссылочных атрибутов
            }
            else
            {
                newAttribute = attribute;
                newAttribute.Owner = this;
            }

            if (!attributes.ContainsKey(newAttribute.ObjectKey))
                attributes.Add(newAttribute);
        }

        /// <summary>
        /// Создание представления по XML-описанию
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="xmlPresentation"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Presentation CreatePresentation(ServerSideObject owner, XmlNode xmlPresentation, ServerSideObjectStates state)
        {
            try
            {
                string key = new Guid(xmlPresentation.Attributes["objectKey"].Value).ToString();

                string name = xmlPresentation.Attributes["name"].Value;

                return new Presentation(key, owner, name, xmlPresentation, state);
            }
            catch (FormatException ex)
            {
                throw new InitializeXmlException(ex.Message, ex);
            }
            catch (OverflowException ex)
            {
                throw new InitializeXmlException(ex.Message, ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new InitializeXmlException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Создание представления из мастера
        /// </summary>
        /// <param name="name"></param>
        /// <param name="caption"></param>
        /// <param name="attributeCollection"></param>
        /// <param name="levelNamingTemplate"></param>
        /// <returns></returns>
        public static Presentation CreatePresentation(string key, string name, string caption, List<IDataAttribute> attributeCollection, string levelNamingTemplate, ServerSideObject owner, ServerSideObjectStates state)
        {
            return new Presentation(key, name, attributeCollection, levelNamingTemplate, owner, state);
        }

        /// <summary>
        /// XML конфигурация объекта
        /// </summary>
        public string ConfigurationXml
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.InnerXml = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><DatabaseConfiguration/>";

                XmlNode element = doc.CreateNode(XmlNodeType.Element, "Presentation", null);
                doc.DocumentElement.AppendChild(element);

                Save2Xml(element);
                return doc.InnerXml;
            }
        }

        /// <summary>
        /// Сохранение представления в XML
        /// </summary>
        /// <param name="node"></param>
        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            XmlHelper.SetAttribute(node, "name", Name);

            // Сохранение информации об атрибутах

            XmlNode attributesNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "PresentationAttributes", null);
            foreach (EntityDataAttribute attr in Attributes.Values)
            {
                if (attr.Class == DataAttributeClassTypes.Typed)
                {
                    XmlNode attributeNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "PresentationAttribute", null);

                    XmlHelper.SetAttribute(attributeNode, "objectKey", attr.ObjectKey);
                    XmlHelper.SetAttribute(attributeNode, "size", attr.Size.ToString());
                    XmlHelper.SetAttribute(attributeNode, "scale", attr.Scale.ToString());
                    XmlHelper.SetAttribute(attributeNode, "visible", Convert.ToString(attr.Visible));
                    XmlHelper.SetAttribute(attributeNode, "mask", attr.Mask);
                    XmlHelper.SetAttribute(attributeNode, "divide", attr.Divide);
                    XmlHelper.SetAttribute(attributeNode, "caption", attr.Caption);
                    XmlHelper.SetAttribute(attributeNode, "position", attr.Position.ToString());
                    XmlHelper.SetAttribute(attributeNode, "lookupType", Convert.ToString((int)attr.LookupType));
                    if (!String.IsNullOrEmpty(attr.GroupTags))
                        XmlHelper.SetAttribute(attributeNode, "groupTags", attr.GroupTags);
                    XmlHelper.SetAttribute(attributeNode, "isReadOnly", Convert.ToString(attr.IsReadOnly));
                    if(attr.IsNullable)
                        XmlHelper.SetAttribute(attributeNode, "nullable", "true");

                    attributesNode.AppendChild(attributeNode);
                }
            }
            node.AppendChild(attributesNode);

            // сохранение информации о ссылках

            XmlNode attributesNodeReference = node.OwnerDocument.CreateNode(XmlNodeType.Element, "PresentationAttributesReference", null);
            foreach (EntityDataAttribute attr in Attributes.Values)
            {
                if (attr.Class == DataAttributeClassTypes.Reference)
                {
                    XmlNode attributeNodeReferenence = node.OwnerDocument.CreateNode(XmlNodeType.Element, "PresentationAttributeReference", null);

                    XmlHelper.SetAttribute(attributeNodeReferenence, "objectKey", attr.ObjectKey);
                    XmlHelper.SetAttribute(attributeNodeReferenence, "visible", Convert.ToString(attr.Visible));
                    XmlHelper.SetAttribute(attributeNodeReferenence, "caption", attr.Caption);
                    XmlHelper.SetAttribute(attributeNodeReferenence, "position", attr.Position.ToString());
                    XmlHelper.SetAttribute(attributeNodeReferenence, "lookupType", Convert.ToString((int)attr.LookupType));
                    if (!String.IsNullOrEmpty(attr.GroupTags))
                        XmlHelper.SetAttribute(attributeNodeReferenence, "groupTags", attr.GroupTags);
                    XmlHelper.SetAttribute(attributeNodeReferenence, "isReadOnly", Convert.ToString(attr.IsReadOnly));
                    if (attr.IsNullable)
                        XmlHelper.SetAttribute(attributeNodeReferenence, "nullable", "true");

                    attributesNodeReference.AppendChild(attributeNodeReferenence);
                }
            }
            node.AppendChild(attributesNodeReference);

            // Сохранение информации об иерархии

            if (!String.IsNullOrEmpty(LevelNamingTemplate))
            {
                XmlNode hierarhyNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "PresentationHierarchy", null);
                XmlHelper.SetAttribute(hierarhyNode, "levelNamingTemplate", LevelNamingTemplate);

                node.AppendChild(hierarhyNode);
            }

        }

        /// <summary>
        /// Блокирвка представления
        /// </summary>
        /// <returns></returns>
        public override IServerSideObject Lock()
        {
            Entity cloneEntity = (Entity)Owner.Lock();
            return (ServerSideObject)cloneEntity.Presentations[GetKey(this.ObjectKey, this.Name)];
        }

        /// <summary>
        /// Снятие блокировки у представления
        /// </summary>
        public override void Unlock()
        {
            base.Unlock();
            this.Attributes.Unlock();
        }

        /// <summary>
        /// Поиск отличий
        /// </summary>
        /// <param name="toObject"></param>
        /// <returns></returns>
        public override IModificationItem GetChanges(IModifiable toObject)
        {
            PresentationModificationItem root = new PresentationModificationItem(String.Format("Представление:{0} ", this.Name), this, toObject);

            Presentation presentation = (Presentation)toObject;

            if (this.Name != presentation.Name)
            {
                ModificationItem item = new PropertyModificationItem("Name", this.Name, presentation.Name, root);
                root.Items.Add(item.Key, item);
            }


            if (this.LevelNamingTemplate != presentation.LevelNamingTemplate)
            {
                ModificationItem item = new PropertyModificationItem("LevelNamingTemplate", this.LevelNamingTemplate, presentation.LevelNamingTemplate, root);
                root.Items.Add(item.Key, item);
            }

            //
            // Attributes
            //
            ModificationItem dataAttributeModificationItem =
                ((DataAttributeCollection)Attributes).GetChanges((DataAttributeCollection)presentation.Attributes);

            List<IModificationItem> removeModification = new List<IModificationItem>();

            foreach (KeyValuePair<string, IModificationItem> modificationItem in dataAttributeModificationItem.Items)
            {
                if (modificationItem.Value.Type == ModificationTypes.Create ||
                    modificationItem.Value.Type == ModificationTypes.Remove)
                    removeModification.Add(modificationItem.Value);
            }

            foreach (IModificationItem modificationItem in removeModification)
            {
                if (dataAttributeModificationItem.Items.ContainsKey(modificationItem.Key))
                    dataAttributeModificationItem.Items.Remove(modificationItem.Key);
            }

            if (dataAttributeModificationItem.Items.Count > 0)
            {
                dataAttributeModificationItem.Parent = root;
                root.Items.Add(dataAttributeModificationItem.Key, dataAttributeModificationItem);
            }

            return root;
        }

        /// <summary>
        /// Обновление представления и сохранение изменений в базе данных
        /// </summary>
        /// <param name="entity"> Отношение, в котором находится представление</param>
        /// <param name="toPresentation"> Представление, которому должно соответствовать изменяемое представление</param>
        internal virtual void Update(Entity entity, Presentation toPresentation)
        {
            // наименование
            if (Name != toPresentation.Name)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилось наименование c \"{1}\" на \"{2}\"",
                    Name, Name, toPresentation.Name));
                Name = toPresentation.Name;
            }

            // имена уровней иерархии
            if (LevelNamingTemplate != toPresentation.LevelNamingTemplate)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменились имена уровней иерархии c \"{1}\" на \"{2}\"",
                    Name, LevelNamingTemplate, toPresentation.LevelNamingTemplate));
                LevelNamingTemplate = toPresentation.LevelNamingTemplate;
            }
        }

        /// <summary>
        /// Обновление атрибута представления
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toDataAttribute"></param>
        internal void UpdateAttribute(ModificationContext context, EntityDataAttribute toDataAttribute)
        {
            EntityDataAttribute attribute = (EntityDataAttribute)DataAttributeCollection.GetAttributeByKeyName(Attributes, toDataAttribute.ObjectKey, toDataAttribute.Name);

            attribute.Update(((Entity)this.Owner), toDataAttribute);

            ((Entity)this.Owner).SaveConfigurationToDatabase(context);

            Trace.WriteLine(String.Format("Атрибут представления \"{0}\" успешно изменен.", attribute));
        }

        /// <summary>
        /// Создание клона представления
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            Presentation clone = (Presentation)base.Clone();
            clone.attributes = (DataAttributeCollection)attributes.Clone();

            return clone;
        }

        /// <summary>
        /// Состояние представления
        /// </summary>
        public override ServerSideObjectStates State
        {
            [DebuggerStepThrough]
            get
            {
                return base.State;
            }
            set
            {
                base.State = value;
                ((DataAttributeCollection)this.Attributes).State = value;
            }
        }

        #endregion Methods

        #region IPresentation Members

        /// <summary>
        /// Имя представления
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }

        #endregion
    }
}