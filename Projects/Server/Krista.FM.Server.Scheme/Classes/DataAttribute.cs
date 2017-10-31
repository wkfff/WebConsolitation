using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Krista.FM.Common;
using Krista.FM.Common.Exceptions;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Базовый класс для атрибутов данных
    /// </summary>
    [XmlRoot("Attribute")]
    internal abstract class DataAttribute : MinorObject, IDataAttribute, IXmlSerializable
    {
        // Борисов: Объекты этого типа являются очень многочисленными и долгоживущими.
        // Имеет смысл включить для них интернирование строковых свойств. Это позволит исключить
        // дублирование таких часто используемых строк как ID, CODE1 и т.п.
        // ***
        // Введение интернирования позволило на момент внесения изменений (28.08.2006)
        // сократить кол-во объектов типа System.String в управляемой куче домена 
        // c 21 000 до 18 000. На скорости загрузки практически не сказалось.

        #region Статические поля

        private static readonly string[] AttributeTypes = new string[] { "ftInteger", "ftDouble", "ftChar", "ftString", "ftBoolean", "ftDate", "ftDateTime", "ftBLOB" };

        internal const string IDColumnName = "ID";
        internal const string PumpIDColumnName = "PumpID";
        internal const string SourceIDColumnName = "SourceID";
        internal const string TaskIDColumnName = "TaskID";
        internal const string ParentIDColumnName = "ParentID";
        internal const string CubeParentIDColumnName = "CubeParentID";
        internal const string SourceKeyColumnName = "SourceKey";
        internal const string HashUKColumnName = "HashUK";
        internal const string RowTypeColumnName = "RowType";
        internal const string VariantCompletedColumnName = "VariantCompleted";
        internal const string VariantCommentColumnName = "VariantComment";
        internal const string DocumentNameColumnName = "{0}Name";
        internal const string DocumentTypeColumnName = "{0}Type";
        internal const string DocumentFileNameColumnName = "{0}FileName";
        internal const string DocumentNameColumnCaption = "{0}";
        internal const string DocumentTypeColumnCaption = "{0} Тип документа";
        internal const string DocumentFileNameColumnCaption = "{0} Имя файла";
        private static readonly string[] ReservedNames = new string[] { 
            IDColumnName, 
            PumpIDColumnName,
            SourceIDColumnName,
            TaskIDColumnName,
            ParentIDColumnName,
            CubeParentIDColumnName,
            SourceKeyColumnName,
            HashUKColumnName,
            RowTypeColumnName,
            VariantCompletedColumnName,
            VariantCommentColumnName
        };
        protected static DataAttribute systemDummy;
        protected static DataAttribute systemID;
        protected static DataAttribute systemPumpID;
        protected static DataAttribute systemPumpIDDefault;
        protected static DataAttribute systemSourceID;
        protected static DataAttribute systemTaskID;
        protected static DataAttribute systemTaskIDDefault;
        protected static DataAttribute systemParentID;
        protected static DataAttribute systemCubeParentID;
        protected static DataAttribute systemHashUK;
        protected static DataAttribute systemSourceKey;
        protected static DataAttribute systemRowType;
        protected static DataAttribute systemVariantCompleted;
        protected static DataAttribute systemVariantComment;
        protected static DataAttribute fixedSourceID;

        #endregion Статические поля

        #region Поля

        // имя
        protected string name;

        // наименование
        protected string caption;

        // описание атрибута
        protected string description;

        // тип
        protected DataAttributeTypes type = DataAttributeTypes.dtUnknown;

        // размер
        protected int size = 0;

        // точность
        protected int scale = 0;

        // обязательность значения
        protected bool nullable = false;

        // значение атрибута по умолчанию
        protected object defaultValue;

        // маска для ввода значения
        protected string mask = String.Empty;
        protected string divide = String.Empty;
        protected bool visible = false;
        private DataAttributeClassTypes _class;

        // вид атрибута
        private DataAttributeKindTypes kind;

        /// <summary>
        /// Определяет возможность редактирования атрибута пользователем
        /// </summary>
        private bool isReadOnly = false;

        /// <summary>
        /// Определяет является ли атрибут строковым идентификатором или нет.
        /// </summary>
        private bool stringIdentifier = false;

        /// <summary>
        /// Описание разработчика объекта
        /// </summary>
        private string developerDescription = String.Empty;

        private string lookupObjectName = String.Empty;

        /// <summary>
        /// Позиция атрибута в коллекции атрибутов (для контроля последовательности атрибутов) 
        /// </summary>
        protected int position;

        /// <summary>
        /// Родительский атрибут при группировке
        /// </summary>
        private string groupParentAttribute = string.Empty;

        /// <summary>
        /// Тэги группировки атрибута
        /// </summary>
        private string groupTags;

        /// <summary>
        /// Полное наименование объекта в котором находится ключ (ID), 
        /// на который "ссылается" значение хранимое в атрибуте
        /// </summary>
        public string LookupObjectName
        {
            get { return Instance.lookupObjectName; }
            set { SetInstance.lookupObjectName = String.Intern(value ?? String.Empty); }
        }

        private string lookupAttributeName = String.Empty;

        /// <summary>
        /// Наименование аттрибута лукап-объекта значение которого отображается
        /// </summary>
        public string LookupAttributeName
        {
            get { return Instance.lookupAttributeName; }
            set
            {
                SetInstance.lookupAttributeName = String.Intern(value ?? String.Empty);
                if (Owner is Entity)
                    LookupObjectName = ((CommonDBObject)Owner).FullName;
            }
        }

        /// <summary>
        /// Определяет является ли атрибут атрибутом типа Lookup
        /// </summary>
        public bool IsLookup
        {
            get { return !String.IsNullOrEmpty(lookupObjectName) && !String.IsNullOrEmpty(lookupAttributeName); }
        }

        private string configuration;
        public string Configuration
        {
            get { return configuration; }
            set { configuration = String.Intern(value ?? String.Empty); }
        }

        /// <summary>
        /// Определяет возможность редактирования атрибута пользователем
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        }

        /// <summary>
        /// Определяет тип лукап атрибута.
        /// </summary>
        private LookupAttributeTypes lookupType;

        /// <summary>
        /// Определяет тип лукап атрибута.
        /// </summary>
        public LookupAttributeTypes LookupType
        {
            get { return Instance.lookupType; }
            set { SetInstance.lookupType = value; }
        }

        #endregion Поля

        #region Конструкторы

        public DataAttribute(string key, string name, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, owner, state)
        {
            this.name = name;
        }

        /// <summary>
        /// Конструктор создает атрибут по XML описанию
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="xmlAttribute"></param>
        /// <param name="state"></param>
        public DataAttribute(string key, ServerSideObject owner, XmlNode xmlAttribute, ServerSideObjectStates state)
            : base(key, owner, state)
        {
            try
            {
                this.configuration = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><DatabaseConfiguration>" + xmlAttribute.OuterXml + "</DatabaseConfiguration>";

                this.name = xmlAttribute.Attributes["name"].Value;

                if (IsFixedDivideCodeAttribute(this.name))
                    _class = DataAttributeClassTypes.Fixed;
                else
                    _class = DataAttributeClassTypes.Typed;

                this.kind = DataAttributeKindTypes.Regular;
                this.visible = true;

                string typeName = xmlAttribute.Attributes["type"].Value;
                for (int i = 0; i < AttributeTypes.Length; i++)
                    if (AttributeTypes[i] == typeName)
                    {
                        this.type = (DataAttributeTypes)Convert.ToInt32(i);
                        break;
                    }

                if (this.type == DataAttributeTypes.dtUnknown)
                    throw new Exception(String.Format("У атрибута {0} указан неверный тип {1}.", this.name, typeName));

                this.caption = xmlAttribute.Attributes["caption"].Value;

                XmlNode xmlNode;
                xmlNode = xmlAttribute.Attributes["description"];
                if (xmlNode != null) this.description = xmlNode.Value;

                xmlNode = xmlAttribute.Attributes["size"];
                if (xmlNode != null) this.size = Convert.ToInt32(xmlNode.Value);
                else this.size = 0;

                xmlNode = xmlAttribute.Attributes["scale"];
                if (xmlNode != null) this.Scale = Convert.ToInt32(xmlNode.Value);
                else this.scale = 0;

                xmlNode = xmlAttribute.Attributes["nullable"];
                if (xmlNode != null) this.nullable = StrUtils.StringToBool(xmlNode.Value);
                else this.nullable = false;

                xmlNode = xmlAttribute.Attributes["visible"];
                if (xmlNode != null) this.Visible = StrUtils.StringToBool(xmlNode.Value);
                else this.Visible = true;

                xmlNode = xmlAttribute.Attributes["isReadOnly"];
                if (xmlNode != null) this.IsReadOnly = StrUtils.StringToBool(xmlNode.Value);
                else this.IsReadOnly = false;

                xmlNode = xmlAttribute.Attributes["mask"];
                if (xmlNode != null) this.mask = xmlNode.Value;
                else this.mask = String.Empty;

                xmlNode = xmlAttribute.Attributes["stringIdentifier"];
                if (xmlNode != null) this.stringIdentifier = StrUtils.StringToBool(xmlNode.Value);
                else this.stringIdentifier = false;

                xmlNode = xmlAttribute.Attributes["lookupType"];
                if (xmlNode != null) this.lookupType = (LookupAttributeTypes)Convert.ToInt32(xmlNode.Value);
                else this.lookupType = LookupAttributeTypes.None;

                // лукап атрибут
                xmlNode = xmlAttribute.Attributes["lookupObject"];
                if (xmlNode != null)
                {
                    this.lookupObjectName = xmlNode.Value;

                    xmlNode = xmlAttribute.Attributes["lookupAttribute"];
                    if (xmlNode != null)
                        this.lookupAttributeName = xmlNode.Value;
                    else
                        throw new Exception("У объекта указан атрибут lookupObject, но не указан lookupAttribute.");
                    this.type = DataAttributeTypes.dtInteger;
                    this.size = 10;
                    this.scale = 0;
                }
                else
                {
                    xmlNode = xmlAttribute.Attributes["divide"];
                    if (xmlNode != null) this.divide = xmlNode.Value;
                    else this.divide = String.Empty;
                }

                xmlNode = xmlAttribute.Attributes["default"];
                if (xmlNode != null && !String.IsNullOrEmpty(xmlNode.Value))
                {
                    switch (this.Type)
                    {
                        case DataAttributeTypes.dtBoolean:
                            int value = Convert.ToInt32(xmlNode.Value);
                            if ((value != 1) && (value != 0))
                            {
                                throw new Exception("Логическое поле может принимать значения 0 или 1");
                            }
                            this.DefaultValue = value;
                            break;
                        case DataAttributeTypes.dtChar:
                            this.DefaultValue = Convert.ToChar(xmlNode.Value);
                            break;
                        case DataAttributeTypes.dtDate:
                            this.DefaultValue = Convert.ToDateTime(xmlNode.Value);
                            break;
                        case DataAttributeTypes.dtDateTime:
                            this.DefaultValue = Convert.ToDateTime(xmlNode.Value);
                            break;
                        case DataAttributeTypes.dtDouble:
                            this.DefaultValue = Convert.ToDouble(xmlNode.Value);
                            break;
                        case DataAttributeTypes.dtInteger:
                            this.DefaultValue = Convert.ToInt32(xmlNode.Value);
                            break;
                        case DataAttributeTypes.dtString:
                            this.DefaultValue = xmlNode.Value;
                            break;
                    }
                }

                xmlNode = xmlAttribute.Attributes["position"];
                if (xmlNode != null) this.position = Convert.ToInt32(xmlNode.Value);
                else this.position = 0;

                xmlNode = xmlAttribute.Attributes["groupParentAttribute"];
                if (xmlNode != null) this.groupParentAttribute = xmlNode.Value;
                else this.groupParentAttribute = string.Empty;

                xmlNode = xmlAttribute.Attributes["groupTags"];
                if (xmlNode != null) this.groupTags = xmlNode.Value;
                else this.groupTags = string.Empty;

                xmlNode = xmlAttribute.SelectSingleNode("DeveloperDescription");
                if (xmlNode != null)
                {
                    DeveloperDescription = xmlNode.InnerText;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Создание атрибута по XML описанию: " + e.Message, e);
            }
        }

        #endregion Конструкторы

        #region Сериализация

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            XmlHelper.SetAttribute(node, "name", Name);
            XmlHelper.SetAttribute(node, "caption", Caption);
            XmlHelper.SetAttribute(node, "type", AttributeTypes[(int)Type]);
            if (Size != 0)
                XmlHelper.SetAttribute(node, "size", Convert.ToString(Size));
            if (Scale != 0)
                XmlHelper.SetAttribute(node, "scale", Convert.ToString(Scale));
            if (IsNullable)
                XmlHelper.SetAttribute(node, "nullable", "true");
            if (!Visible)
                XmlHelper.SetAttribute(node, "visible", "false");
            if (Convert.ToString(DefaultValue) != String.Empty)
                XmlHelper.SetAttribute(node, "default", Convert.ToString(DefaultValue));
            if (!String.IsNullOrEmpty(Mask))
                XmlHelper.SetAttribute(node, "mask", Mask);

            if (!String.IsNullOrEmpty(lookupObjectName))
            {
                XmlHelper.SetAttribute(node, "lookupObject", lookupObjectName);
                XmlHelper.SetAttribute(node, "lookupAttribute", lookupAttributeName);
            }
            else
            {
                if (!String.IsNullOrEmpty(Divide))
                    XmlHelper.SetAttribute(node, "divide", Divide);
            }

            if (!String.IsNullOrEmpty(Description))
                XmlHelper.SetAttribute(node, "description", Description);

            if (!String.IsNullOrEmpty(DeveloperDescription))
            {
                XmlNode devDescr = XmlHelper.AddChildNode(node, "DeveloperDescription", String.Empty, null);
                XmlHelper.AppendCDataSection(devDescr, DeveloperDescription);
            }

            XmlHelper.SetAttribute(node, "lookupType", Convert.ToString((int)this.LookupType));

            if (StringIdentifier)
            {
                XmlHelper.SetAttribute(node, "stringIdentifier", "true");
            }

            if (isReadOnly)
            {
                XmlHelper.SetAttribute(node, "isReadOnly", "true");
            }

            if (Position != 0)
            {
                XmlHelper.SetAttribute(node, "position", Convert.ToString(Position));
            }

            if (!String.IsNullOrEmpty(GroupParentAttribute))
            {
                XmlHelper.SetAttribute(node, "groupParentAttribute", Convert.ToString(GroupParentAttribute));
            }

            if (!String.IsNullOrEmpty(GroupTags))
            {
                XmlHelper.SetAttribute(node, "groupTags", GroupTags);
            }

        }

        /// <summary>
        /// Сериализация атрибута для документации
        /// </summary>
        /// <param name="node"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        public virtual void Save2XmlDocumentation(XmlNode node)
        {
            try
            {
                Save2Xml(node);

                AppendAttributes(node);
            }
            catch (System.InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }

        }

        /// <summary>
        /// Дополнительные свойства для документации
        /// </summary>
        /// <param name="node"></param>
        public void AppendAttributes(XmlNode node)
        {
            if (node != null)
            {
                // Тип атрибута
                XmlHelper.SetAttribute(node, "kind", ((int)this.Kind).ToString());
                //Класс атрибута
                XmlHelper.SetAttribute(node, "class", ((int)this.Class).ToString());

                // У атрибутов системных объектов отсутствует objectKey
                if (node.Attributes["objectKey"] == null)
                {
                    node.Attributes.Prepend(XmlHelper.SetAttribute(node, "objectKey", Guid.Empty.ToString()));
                }

                string calcPos = this.CalculatePosition(string.Empty);
                if (String.IsNullOrEmpty(calcPos))
                    calcPos = position.ToString();

                //Вычисляемая позиция для справки
                XmlHelper.SetAttribute(node, "positionCalc", calcPos);

                if (this is IDocumentEntityAttribute)
                {
                    IEntity sourceEntity = ((IEntity)SchemeClass.Instance.RootPackage.FindEntityByName(
                                                          ((IDocumentEntityAttribute)this).
                                                              SourceEntityKey));
                    if (sourceEntity != null)
                        node.Attributes["caption"].InnerText = String.Format("{0} ({1})", this.Caption,
                                                                         sourceEntity.FullCaption);
                }
            }
        }

        #endregion Сериализация

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected DataAttribute Instance
        {
            [DebuggerStepThrough]
            get { return (DataAttribute)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected DataAttribute SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (DataAttribute)CloneObject;
                else
                    return this;
            }
        }

        public override IServerSideObject Lock()
        {
            Entity cloneEntity = (Entity)Owner.Lock();
            return (ServerSideObject)cloneEntity.Attributes[GetKey(this.ObjectKey, this.Name)];
        }

        #endregion ServerSideObject

        #region События

        /// <summary>
        /// Вызывается после добавления атрибута в коллекцию.
        /// </summary>
        internal virtual void OnAfterAdd()
        {
        }

        /// <summary>
        /// Вызывается перед удалением атрибута из коллекции.
        /// </summary>
        internal virtual void OnBeforeRemove()
        {
            // Если атрибут пользовательского типа, удаляем ссылки на него.
            if (Class == DataAttributeClassTypes.Typed)
                CascadeRemove();

            //Изменяем уникальные ключи, в которых используется данный атрибут
            CascadeRemoveUniqueKey();
        }

        #endregion События

        #region Системные атрибуты

        public DataAttribute CloneStatic()
        {
            DataAttribute clone = (DataAttribute)MemberwiseClone();
            clone.identifier = GetNewIdentifier();
            return clone;
        }

        /// <summary>
        /// Пустой атрибут, используется для внутренних целей
        /// </summary>
        internal static DataAttribute SystemDummy
        {
            [DebuggerStepThrough]
            get { return systemDummy; }
        }

        /// <summary>
        /// Первичный ключ для всех таблиц
        /// </summary>
        public static DataAttribute SystemID
        {
            [DebuggerStepThrough]
            get { return systemID.CloneStatic(); }
        }

        /// <summary>
        /// Идентификатор закачки данных
        /// </summary>
        public static DataAttribute SystemPumpID
        {
            [DebuggerStepThrough]
            get { return systemPumpID.CloneStatic(); }
        }

        /// <summary>
        /// Идентификатор закачки данных со значением по умолчанию -1
        /// </summary>
        public static DataAttribute SystemPumpIDDefault
        {
            [DebuggerStepThrough]
            get { return systemPumpIDDefault.CloneStatic(); }
        }

        /// <summary>
        /// Идентификатор источника данных
        /// </summary>
        public static DataAttribute SystemSourceID
        {
            [DebuggerStepThrough]
            get { return systemSourceID.CloneStatic(); }
        }

        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public static DataAttribute SystemTaskID
        {
            [DebuggerStepThrough]
            get { return systemTaskID.CloneStatic(); }
        }

        /// <summary>
        /// Идентификатор задачи со значением по умолчанию -1
        /// </summary>
        public static DataAttribute SystemTaskIDDefault
        {
            [DebuggerStepThrough]
            get { return systemTaskIDDefault.CloneStatic(); }
        }

        /// <summary>
        /// Идентификатор источника данных из которого пришла запись при формировании сопоставимого
        /// </summary>
        public static DataAttribute FixedSourceID
        {
            [DebuggerStepThrough]
            get { return fixedSourceID.CloneStatic(); }
        }

        /// <summary>
        /// Родительский ключь
        /// </summary>
        public static DataAttribute SystemParentID
        {
            [DebuggerStepThrough]
            get { return systemParentID.CloneStatic(); }
        }

        /// <summary>
        /// Родительский ключь иерархии в кубах
        /// </summary>
        public static DataAttribute SystemCubeParentID
        {
            [DebuggerStepThrough]
            get { return systemCubeParentID.CloneStatic(); }
        }

        /// <summary>
        /// Поле с хэшем по полям из уникального ключа, помеченного опцией "хэшировать"
        /// </summary>
        public static DataAttribute SystemHashUK
        {
            [DebuggerStepThrough]
            get { return systemHashUK.CloneStatic(); }
        }

        /// <summary>
        /// Идентификарор исходной записи источника данных откуда пришли данные
        /// </summary>
        public static DataAttribute SystemSourceKey
        {
            [DebuggerStepThrough]
            get { return systemSourceKey.CloneStatic(); }
        }

        /// <summary>
        /// Тип записи классификаторов
        /// </summary>
        public static DataAttribute SystemRowType
        {
            [DebuggerStepThrough]
            get { return systemRowType.CloneStatic(); }
        }

        /// <summary>
        /// Поле-флаг "Завершен расчет варианта"
        /// </summary>
        public static DataAttribute SystemVariantCompleted
        {
            [DebuggerStepThrough]
            get { return systemVariantCompleted.CloneStatic(); }
        }

        /// <summary>
        /// Комментарий к варианту
        /// </summary>
        public static DataAttribute SystemVariantComment
        {
            [DebuggerStepThrough]
            get { return systemVariantComment.CloneStatic(); }
        }

        #endregion Системные атрибуты

        #region Редартируемые свойства

        /// <summary>
        /// Устанавливает английское наименование
        /// </summary>
        /// <param name="oldValue">Старое наименование</param>
        /// <param name="value">Новое наименование</param>
        protected virtual void SetFullName(string oldValue, string value)
        {
        }

        private void InitializePredefinedAttributesByName(string attribeteName)
        {
            switch (attribeteName)
            {
                case "Code":
                    Caption = "Код";
                    Type = DataAttributeTypes.dtInteger;
                    Mask = String.Empty;
                    LookupType = LookupAttributeTypes.Primary;
                    break;
                case "CodeStr":
                    Caption = "Код";
                    Type = DataAttributeTypes.dtString;
                    Size = 20;
                    Mask = String.Empty;
                    LookupType = LookupAttributeTypes.Primary;
                    break;
                case "Name":
                    Caption = "Наименование";
                    Type = DataAttributeTypes.dtString;
                    Mask = String.Empty;
                    LookupType = LookupAttributeTypes.Secondary;
                    StringIdentifier = true;
                    break;
                default:
                    StringIdentifier = false;
                    break;
            }
        }

        /// <summary>
        /// Имя атрибута в БД
        /// </summary>
        public virtual string Name
        {
            [DebuggerStepThrough]
            get { return Instance.name; }
            set
            {
                ScriptingEngine.ScriptingEngineHelper.CheckDBName(value);

                if (IsReservedName(value))
                {
                    throw new ServerException(String.Format("Наименование \"{0}\" зарезервировано.", value));
                }

                if (this.Name != value)
                {
                    CascadeRemoveUniqueKey();
                }

                string oldName = ObjectOldKeyName;
                string oldValue = GetKey(ObjectKey, ObjectOldKeyName);
                SetInstance.name = String.Intern(value ?? String.Empty);
                if (!String.IsNullOrEmpty(oldValue))
                {
                    SetFullName(oldValue, Name);

                    if (oldName.StartsWith("NewAttribute"))
                    {
                        InitializePredefinedAttributesByName(value);
                    }
                }
                
            }
        }

        /// <summary>
        /// Уникальное наименование объекта (старый ключ).
        /// Это свойство после выпуска версии 2.4.1 необходимо удалить.
        /// </summary>
        public override string ObjectOldKeyName
        {
            get { return Name; }
        }

        /// <summary>
        /// Наименование атрибута выводимое в интерфейсе
        /// </summary>
        public virtual string Caption
        {
            [DebuggerStepThrough]
            get { return Instance.caption; }
            set
            {
                ScriptingEngine.ScriptingEngineHelper.CheckOlapName(value, ",;'`:/\\*|?\"&%$!-+=()[]{}".ToCharArray());
                SetInstance.caption = String.Intern(value ?? String.Empty);
            }
        }

        /// <summary>
        /// Описание атрибута
        /// </summary>
        public string Description
        {
            [DebuggerStepThrough]
            get { return Instance.description; }
            set { SetInstance.description = String.Intern(value ?? String.Empty); }
        }

        /// <summary>
        /// Тип данных атрибута
        /// </summary>
        public DataAttributeTypes Type
        {
            [DebuggerStepThrough]
            get { return Instance.type; }
            set
            {
                SetInstance.type = value;
                SetTypeDimensions();
                CheckDefaultValue();
            }
        }

        /// <summary>
        /// Размер данных атрибута
        /// </summary>
        public int Size
        {
            [DebuggerStepThrough]
            get { return Instance.size; }
            [DebuggerStepThrough]
            set
            {
                SetInstance.size = value;
                CheckDefaultValue();
            }
        }

        /// <summary>
        /// Точность данных атрибута
        /// </summary>
        public int Scale
        {
            [DebuggerStepThrough]
            get { return Instance.scale; }
            [DebuggerStepThrough]
            set
            {
                SetInstance.scale = value;
                CheckDefaultValue();
            }
        }

        /// <summary>
        /// Определяет фиксированный атрибут или нет
        /// </summary>
        public bool Fixed
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        /// Определяет обязательность значения.
        /// </summary>
        public virtual bool IsNullable
        {
            [DebuggerStepThrough]
            get { return Instance.nullable; }
            [DebuggerStepThrough]
            set { SetInstance.nullable = value; }
        }

        /// <summary>
        /// Значение атрибута по умолчанию
        /// </summary>
        public object DefaultValue
        {
            [DebuggerStepThrough]
            get { return Instance.defaultValue; }
            [DebuggerStepThrough]
            set
            {
                if (!RightDefaultValue(value))
                {
                    throw new Exception("Значение по умолчанию не соответствует типу атрибута.");
                }
                SetInstance.defaultValue = value;
            }
        }

        /// <summary>
        /// Маска для ввода значения
        /// </summary>
        public string Mask
        {
            [DebuggerStepThrough]
            get { return Instance.mask; }
            [DebuggerStepThrough]
            set { SetInstance.mask = String.Intern(value ?? String.Empty); }
        }

        public string Divide
        {
            [DebuggerStepThrough]
            get { return Instance.divide; }
            [DebuggerStepThrough]
            set { SetInstance.divide = String.Intern(value ?? String.Empty); }
        }

        public bool Visible
        {
            [DebuggerStepThrough]
            get { return Instance.visible; }
            [DebuggerStepThrough]
            set { SetInstance.visible = value; }
        }

        /// <summary>
        /// Системный класс
        /// </summary>
        public DataAttributeClassTypes Class
        {
            [DebuggerStepThrough]
            get { return _class; }
            [DebuggerStepThrough]
            set { _class = value; }
        }

        /// <summary>
        /// Вид атрибута (для пользовательского интерфейса)
        /// </summary>
        public DataAttributeKindTypes Kind
        {
            [DebuggerStepThrough]
            get { return kind; }
            [DebuggerStepThrough]
            set { kind = value; }
        }

        /// <summary>
        /// Определяет является ли атрибут строковым идентификатором или нет.
        /// </summary>
        public bool StringIdentifier
        {
            [DebuggerStepThrough]
            get { return Instance.stringIdentifier; }
            [DebuggerStepThrough]
            set { SetInstance.stringIdentifier = value; }
        }

        /// <summary>
        /// Описание разработчика объекта
        /// </summary>
        public string DeveloperDescription
        {
            [DebuggerStepThrough]
            get { return Instance.developerDescription; }
            [DebuggerStepThrough]
            set { SetInstance.developerDescription = String.Intern(value ?? String.Empty); }
        }

        /// <summary>
        /// Позиция атрибута в коллекции атрибутов (для контроля последовательности атрибутов) 
        /// </summary>
        public int Position
        {
            get { return Instance.position; }
            set
            {
                if (Instance.Position != value)
                {
                    RepositionAttribute(value);
                    SetInstance.position = value;
                }
            }
        }

        /// <summary>
        /// Позиция атрибута в коллекции атрибутов (для контроля последовательности атрибутов) 
        /// </summary>
        internal int PositionSet
        {
            set
            {
                SetInstance.position = value;
            }
        }

        /// <summary>
        /// Родительский атрибут при группировке
        /// </summary>
        public string GroupParentAttribute
        {
            get { return Instance.groupParentAttribute; }
            set { SetInstance.groupParentAttribute = value; }
        }

        /// <summary>
        /// Тэги для группировки
        /// </summary>
        public string GroupTags
        {
            get { return Instance.groupTags; }
            set { SetInstance.groupTags = value; }
        }

        #endregion Редартируемые свойства

        #region Свойства

        /// <summary>
        /// SQL-определение атрибута
        /// </summary>
        public virtual string SQLDefinition
        {
            get { return name; }
        }

        /// <summary>
        /// Уникальный ключ атрибута
        /// </summary>
        public string Key
        {
            get
            {
                if (Owner is IEntity)
                {
                    return String.Format("{0}{1}:{2}", ((IEntity)Owner).Key + "::", this.GetType().Name, Name);
                }
                if (Owner is IEntityAssociation)
                {
                    return String.Format("{0}{1}:{2}", ((IEntityAssociation)Owner).Key + "::", this.GetType().Name, Name);
                }
                return String.Format("{0}{1}:{2}", String.Empty, this.GetType().Name, Name);
            }
        }

        /// <summary>
        /// Полное имя атрибута
        /// </summary>
        public string FullName
        {
            get
            {
                if (Owner is IEntity)
                {
                    return String.Format("{0}{1}", ((IEntity)Owner).FullName + ".", Name);
                }
                if (Owner is IEntityAssociation)
                {
                    return String.Format("{0}{1}", ((IEntityAssociation)Owner).FullName + ".", Name);
                }
                return String.Format("{0}{1}", String.Empty, Name);
            }
        }

        #endregion

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            reader.ReadStartElement("DataAttribute");
            Name = reader.ReadElementString("Name");
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteElementString("Name", Name);
        }

        #endregion

        #region Прочее

        internal static Type TypeOfAttribute(DataAttributeTypes dataAttributeTypes)
        {
            switch (dataAttributeTypes)
            {
                case DataAttributeTypes.dtInteger: return typeof(Int32);
                case DataAttributeTypes.dtDouble: return typeof(Double);
                case DataAttributeTypes.dtString: return typeof(String);
                case DataAttributeTypes.dtBoolean: return typeof(Int32);
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime: return typeof(DateTime);
                default: throw new Exception(String.Format("Тип данных {0} не поддерживается.", dataAttributeTypes));
            }
        }

        internal static object GetStandardDefaultValue(IDataAttribute attr)
        {
            return GetStandardDefaultValue(attr, String.Empty);
        }

        internal static object GetStandardDefaultValue(IDataAttribute attr, string stringDefaultValue)
        {
            object value = null;
            switch (attr.Type)
            {
                case DataAttributeTypes.dtInteger:
                case DataAttributeTypes.dtDouble:
                case DataAttributeTypes.dtBoolean:
                    value = 0;
                    break;
                case DataAttributeTypes.dtString:
                    stringDefaultValue = stringDefaultValue.Substring(0, stringDefaultValue.Length > attr.Size ? attr.Size : stringDefaultValue.Length);
                    value = "'" + stringDefaultValue + "'";
                    break;
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    value = DateTime.MinValue;
                    break;
            }
            return value;
        }

        /// <summary>
        /// Устанавливает размерности типа по умолчанию
        /// </summary>
        private void SetTypeDimensions()
        {
            switch (type)
            {
                case DataAttributeTypes.dtInteger:
                    size = 10;
                    scale = 0;
                    break;
                case DataAttributeTypes.dtString:
                    size = 255;
                    scale = 0;
                    break;
                case DataAttributeTypes.dtDouble:
                    size = 17;
                    scale = 2;
                    break;
                case DataAttributeTypes.dtBoolean:
                    size = 1;
                    scale = 0;
                    break;
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    size = 0;
                    scale = 0;
                    break;
                case DataAttributeTypes.dtChar:
                    size = 1;
                    scale = 0;
                    break;
                case DataAttributeTypes.dtBLOB:
                    size = 0;
                    scale = 0;
                    defaultValue = null;
                    visible = false;
                    mask = String.Empty;
                    divide = String.Empty;
                    lookupType = LookupAttributeTypes.None;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Определяет можно ли преобразовать одит тип данных в другой.
        /// </summary>
        /// <param name="from">Исходный тип.</param>
        /// <param name="to">Целевой тип.</param>
        /// <returns>true - преобразование возможно, иначе false.</returns>
        internal static bool IsConvertableTypes(DataAttributeTypes from, DataAttributeTypes to)
        {
            switch (from)
            {
                case DataAttributeTypes.dtBLOB:
                    return to == DataAttributeTypes.dtBLOB;
                case DataAttributeTypes.dtBoolean:
                    return to == DataAttributeTypes.dtBoolean
                        || to == DataAttributeTypes.dtString;
                case DataAttributeTypes.dtChar:
                    return to == DataAttributeTypes.dtChar
                        || to == DataAttributeTypes.dtString;
                case DataAttributeTypes.dtDate:
                    return to == DataAttributeTypes.dtDate
                        || to == DataAttributeTypes.dtDateTime
                        || to == DataAttributeTypes.dtString;
                case DataAttributeTypes.dtDateTime:
                    return to == DataAttributeTypes.dtDateTime
                        || to == DataAttributeTypes.dtString;
                case DataAttributeTypes.dtDouble:
                    return to == DataAttributeTypes.dtDouble
                        || to == DataAttributeTypes.dtString;
                case DataAttributeTypes.dtInteger:
                    return to == DataAttributeTypes.dtInteger
                        || to == DataAttributeTypes.dtDouble
                        || to == DataAttributeTypes.dtString;
                case DataAttributeTypes.dtString:
                    return to == DataAttributeTypes.dtString;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Определяет является ли имя name зарезирвированным именем.
        /// </summary>
        /// <param name="name">Имя атрибута.</param>
        /// <returns>true - если имя зарезервировано; false - иначе.</returns>
        internal static bool IsReservedName(string name)
        {
            if (IsFixedDivideCodeAttribute(name))
                return true;

            string upperName = name.ToUpper();
            foreach (string item in ReservedNames)
            {
                if (item.ToUpper() == upperName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Определяет является ли имя name именем фиксированного атрибута или нет
        /// </summary>
        /// <param name="name">Имя атрибута</param>
        /// <returns>true - если имя фиксированное; false - если имя нефиксированное</returns>
        internal static bool IsFixedDivideCodeAttribute(string name)
        {
            if (name.Length > 4 && name.Substring(0, 4).ToUpper() == "CODE")
            {
                try
                {
                    Convert.ToInt32(name.Substring(4, name.Length - 4));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет соответствие значения по умолчанию типу данных атрибута.
        /// </summary>
        /// <param name="value">Присваиваемое значение.</param>
        /// <returns>true, если системный, или пользовательский и все соответствует, false, в противном случае.</returns>
        private bool RightDefaultValue(object value)
        {
            // Если атрибут пользовательского типа, то проверяем.
            if (value != null && value.ToString() != String.Empty && Class == DataAttributeClassTypes.Typed)
                return (RightValueType(value) && RightValueSize(value) && RightValueScale(value));
            // Иначе, считаем, что все хорошо.
            else return true;
        }

        /// <summary>
        /// Проверяет точность данных атрибута.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool RightValueScale(object value)
        {
            // Если тип атрибута вещественный, и в значении есть точки.
            if (type == DataAttributeTypes.dtDouble && value.ToString().IndexOf('.') >= 0)
            {
                // Разбиваем его на целую и дробную часть.
                string[] valueSplit = value.ToString().Split('.');
                // Если длина дробной части больше точности.
                if (valueSplit[1].Length > scale)
                {
                    // то это неправильно.
                    return false;
                }
            }
            // Иначе все нормально.
            return true;
        }

        /// <summary>
        /// Проверяет размер присваиваемой величины.
        /// </summary>
        /// <param name="value">Присваиваемая величина.</param>
        /// <returns>true, если размер не превышает размер данных атрибута.</returns>
        private bool RightValueSize(object value)
        {
            // Если имеет смысл проверять длину
            if (type == DataAttributeTypes.dtDouble || type == DataAttributeTypes.dtInteger || type == DataAttributeTypes.dtString)
                // Возвращаем результат проверки.
                return value.ToString().Length <= Size;
            // Иначе все нормально.
            return true;
        }

        /// <summary>
        /// Проверяет сответствие типа данных.
        /// </summary>
        /// <param name="value">Значение по умолчанию.</param>
        /// <returns>true, если значение соответствует типу данных.</returns>
        private bool RightValueType(object value)
        {
            // Смотрим тип атрибута и пытаемся привести к нему значение по умолчанию.
            try
            {
                switch (type)
                {
                    case DataAttributeTypes.dtInteger:
                        {
                            Convert.ToInt32(value);
                            break;
                        }
                    case DataAttributeTypes.dtDouble:
                        {
                            Convert.ToDouble(value);
                            break;
                        }
                    case DataAttributeTypes.dtBoolean:
                        {
                            if ((value.ToString() != "1") && (value.ToString() != "0"))
                                throw new Exception();
                            break;
                        }
                    case DataAttributeTypes.dtDate:
                    case DataAttributeTypes.dtDateTime:
                        {
                            Convert.ToDateTime(value);
                            break;
                        }
                    case DataAttributeTypes.dtChar:
                        {
                            Convert.ToChar(value);
                            break;
                        }
                }
            }
            catch
            {
                // Если при приведении возникло исключение, то тип не соответствует.
                return false;
            }
            // Если все нормально, то соответствует.
            return true;
        }

        /// <summary>
        /// Проверяет, соответствует ли значиние по умолчанию изменениям, и, если нет, то очищает его.
        /// </summary>
        private void CheckDefaultValue()
        {
            if (SetInstance.DefaultValue != null && !RightDefaultValue(SetInstance.DefaultValue))
            {
                SetInstance.defaultValue = null;
            }
        }

        #region Каскадное удаление ссылок на атрибут

        /// <summary>
        /// Очищает ссылки на атрибут из объектов схемы.
        /// </summary>
        private void CascadeRemove()
        {
            if (Owner is IClassifier)
                ClearDimensions();

            // Тут будут накапливаться отчет об ошибках.
            string erMessage = string.Empty;

            if (Owner is IEntity)
            {
                // Перебираем ассоциации на родительские отношения.
                foreach (IEntityAssociation item in ((IEntity)Owner).Associations.Values)
                    // Если ассоциация сопоставления
                    if (item is BridgeAssociation)
                    {
                        // Пытаемся удалять правила из ассоциации.
                        try
                        {
                            RemoveAssociationMapping(item);
                            RemoveAssociationRuleMapping(item);
                        }
                        // Если поймали исключение, то добавляем информацию об ошибке.
                        catch (Exception e)
                        {
                            erMessage += AddErMessage(e, item);
                        }
                    }

                // Перебираем ассоциации на дочерние отношения.
                foreach (IEntityAssociation item in ((IEntity)Owner).Associated.Values)
                    // Если ассоциация сопоставления
                    if (item is BridgeAssociation)
                    {
                        // Пытаемся удалять правила из ассоциации.
                        try
                        {
                            RemoveAssociatedMapping(item);
                            RemoveAssociatedRuleMapping(item);
                        }
                        // Если поймали исключение, то добавляем информацию об ошибке.
                        catch (Exception e)
                        {
                            erMessage += AddErMessage(e, item);
                        }
                    }
                // Если при удалении были ошибки, то генерируем новое исключение с отчетом об ошибках.
                if (erMessage != string.Empty)
                    throw new Exception(
                        string.Format("При удалении ссылок на атрибут возникли следующие ошибки:\n {0}", erMessage));
            }
        }

        /// <summary>
        /// Очищает ссылки на атрибут из уникальных ключей этой таблицы (удаляя эти уникальные ключи)
        /// </summary>
        private void CascadeRemoveUniqueKey()
        {
            UniqueKeyCollection uniqueKeyCollection = null;

            if (Owner is IEntity)
            {
                uniqueKeyCollection = (UniqueKeyCollection) ((IEntity)Owner).UniqueKeys;
            }
            else if (Owner is IAssociation)
            {
                uniqueKeyCollection = (UniqueKeyCollection) ((IEntity)((Association)Owner).RoleA).UniqueKeys;
            }
            
            if (uniqueKeyCollection != null)
            {
                uniqueKeyCollection.RemoveFieldFromUniqueKeys(this.Name);
            }
        }


        /// <summary>
        /// Формирует строку для добавления к отчету.
        /// </summary>
        /// <param name="e">Сообщение.</param>
        /// <param name="item">Ассоциация при удалении из которой возникла ошибка.</param>
        /// <returns>Строка для добавления</returns>
        private static string AddErMessage(Exception e, IEntityAssociation item)
        {
            return string.Format("{0}: {1};\n ", item.FullName, e.Message);
        }

        /// <summary>
        /// Удаляет правила формирования, где используется атрибут со стороны сопоставимого классификатора данных.
        /// </summary>
        /// <param name="item">Ассоциация.</param>
        private void RemoveAssociatedMapping(IEntityAssociation item)
        {
            // Список правил, которые будем удалять.
            List<AssociateMapping> forRemove = new List<AssociateMapping>();

            foreach (AssociateMapping mappingRule in ((BridgeAssociation)item).Mappings)
            {
                if (mappingRule.BridgeValue.Attribute.Name == name)
                {
                    forRemove.Add(mappingRule);
                }
            }
            // Удаляем правила по списку.
            RemoveMappingRule(forRemove, item);
        }

        /// <summary>
        /// Удаляет правила формирования, где используется атрибут со стороны классификатора данных.
        /// </summary>
        /// <param name="item">Ассоциация.</param>
        private void RemoveAssociationMapping(IEntityAssociation item)
        {
            // Список правил, которые будем удалять.
            List<AssociateMapping> forRemove = new List<AssociateMapping>();
            foreach (AssociateMapping mappingRule in ((BridgeAssociation)item).Mappings)
            {
                if (mappingRule.DataValue.Attribute.Name == name)
                {
                    forRemove.Add(mappingRule);
                }
            }
            // Удаляем правила по списку.
            RemoveMappingRule(forRemove, item);
        }

        /// <summary>
        /// Удаляет правила формирования из ассоциации.
        /// </summary>
        /// <param name="forRemove">Список правил на удаление.</param>
        /// <param name="item">Ассоциация, из которой удаляем.</param>
        private static void RemoveMappingRule(List<AssociateMapping> forRemove, IEntityAssociation item)
        {
            foreach (AssociateMapping mappingRule in forRemove)
            {
                ((BridgeAssociation)item).Mappings.Remove(mappingRule);
            }
        }

        /// <summary>
        /// Удаляет правила сопоставления, где используется атрибут со стороны сопоставимого классификатора данных.
        /// </summary>
        /// <param name="item">Ассоциация.</param>
        private void RemoveAssociatedRuleMapping(IEntityAssociation item)
        {
            // Список правил, которые будем удалять.
            List<string> forRemove = new List<string>();

            foreach (AssociateRule rule in ((BridgeAssociation)item).AssociateRules.Values)
            {
                foreach (AssociateMapping mappingRule in rule.Mappings)
                {
                    if (mappingRule.BridgeValue.Attribute != null
                        && mappingRule.BridgeValue.Attribute.Name == name)
                    {
                        forRemove.Add(rule.Name);
                    }
                }
            }
            // Удаляем правила по списку.
            RemoveAssociateRule(forRemove, item);
        }

        /// <summary>
        /// Удаляет правила сопоставления, где используется атрибут со стороны классификатора данных.
        /// </summary>
        /// <param name="item">Ассоциация.</param>
        private void RemoveAssociationRuleMapping(IEntityAssociation item)
        {
            // Список правил, которые будем удалять.
            List<string> forRemove = new List<string>();

            foreach (AssociateRule rule in ((BridgeAssociation)item).AssociateRules.Values)
            {
                foreach (AssociateMapping mappingRule in rule.Mappings)
                {
                    if (mappingRule.DataValue.Attribute != null
                        && mappingRule.DataValue.Attribute.Name == name)
                    {
                        forRemove.Add(rule.Name);
                    }
                }
            }
            // Удаляем правила по списку.
            RemoveAssociateRule(forRemove, item);
        }

        /// <summary>
        /// Удаляет правила сопоставления из ассоциации.
        /// </summary>
        /// <param name="forRemove">Список правил на удаление.</param>
        /// <param name="item">Ассоциация из которой будем удалять.</param>
        private static void RemoveAssociateRule(List<string> forRemove, IEntityAssociation item)
        {
            foreach (string ruleName in forRemove)
            {
                ((BridgeAssociation)item).AssociateRules.Remove(ruleName);
            }
        }

        /// <summary>
        /// Удаляет ссылки на атрибут из параметров иерархии.
        /// </summary>
        private void ClearDimensions()
        {
            try
            {
                foreach (IDimensionLevel item in ((IClassifier)Owner).Levels.Values)
                {
                    if (item.LevelType != LevelTypes.All)
                    {
                        if (((IClassifier)Owner).Levels.HierarchyType == HierarchyType.ParentChild)
                        {
                            if (item.MemberKey.Name == name)
                                item.MemberKey = ((IClassifier)Owner).Attributes[IDColumnName];
                            if (item.MemberName.Name == name)
                                item.MemberName = ((IClassifier)Owner).Attributes[IDColumnName];
                            if (item.ParentKey.Name == name)
                                item.ParentKey = ((IClassifier)Owner).Attributes[ParentIDColumnName];
                        }
                        else
                        {
                            if (item.MemberKey.Name == name)
                                item.MemberKey = ((IClassifier)Owner).Attributes[IDColumnName];
                            if (item.MemberName.Name == name)
                                item.MemberName = ((IClassifier)Owner).Attributes[IDColumnName];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("{0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
            }
        }

        #endregion

        /// <summary>
        /// Метод получает вычисленную позицию атрибута в коллекции
        /// </summary>
        /// <returns></returns>
        public string GetCalculationPosition()
        {
            return CalculatePosition(String.Empty);
        }

        /// <summary>
        /// Метод вычисляет позицию атрибута в коллекции
        /// </summary>
        /// <param name="positionCalc"></param>
        /// <returns></returns>
        private string CalculatePosition(string positionCalc)
        {
            IDataAttributeCollection attributeCollection = GetAttributesCollection(Instance.Owner);


            // Вычисляем позицию для атрибутов сущности, для представлений структуры пока не надо
            if (attributeCollection == null)
                return String.Empty;

            if (Instance.Class != DataAttributeClassTypes.Fixed && Instance.Class != DataAttributeClassTypes.System)
            {
                SortedList<int, DataAttribute> list = new SortedList<int, DataAttribute>();

                foreach (KeyValuePair<string, IDataAttribute> pair in attributeCollection)
                {
                    DataAttribute inst = ((DataAttribute)pair.Value).Instance;

                    if (inst.GroupParentAttribute == Instance.GroupParentAttribute)
                        if (inst.Class != DataAttributeClassTypes.Fixed && inst.Class != DataAttributeClassTypes.System
                            && inst.Visible)
                            if (!list.ContainsKey(inst.Position))
                                list.Add(inst.Position, inst);
                }

                int pos = list.IndexOfKey(Instance.Position) + 1;

                if (String.IsNullOrEmpty(positionCalc))
                    positionCalc = pos.ToString();
                else
                {
                    positionCalc = String.Format("{0}.{1}", pos, positionCalc);
                }

                if (!String.IsNullOrEmpty(Instance.GroupParentAttribute))
                {
                    // Родительский атрибут
                    DataAttribute parent =
                        DataAttributeCollection.GetAttributeByKeyName(attributeCollection,
                                                                      Instance.GroupParentAttribute, string.Empty);
                    if (parent != null)
                        positionCalc = parent.CalculatePosition(positionCalc);
                }
            }

            return positionCalc;
        }

        /// <summary>
        /// Получение коллекции атрибутов в зависимости от типа владельца
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private IDataAttributeCollection GetAttributesCollection(ServerSideObject owner)
        {
            if (owner is IEntity)
                return ((IEntity)owner).Attributes;

            if (owner is IPresentation)
                return ((IPresentation)owner).Attributes;

            if (owner is IAssociation)
                return ((IAssociation)owner).RoleData.Attributes;

            return null;
        }

        #region Контроль за последовательностью атрибутов

        /// <summary>
        /// Перемещение атрибута
        /// </summary>
        /// <returns></returns>
        public bool RepositionAttribute(int newPosition)
        {
            if (Instance.position != 0)
            {
                try
                {
                    if (Instance.Position < newPosition)
                    {
                        Decrement(Instance.Position, newPosition + 1);
                    }
                    else
                    {
                        Increment(newPosition - 1, Instance.Position);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        private void Increment(int startPosition, int endPosition)
        {
            IDataAttributeCollection attributeCollection = GetAttributesCollection(Instance.Owner);

            foreach (KeyValuePair<string, IDataAttribute> pair in attributeCollection)
                if (((DataAttribute)pair.Value).Instance.Position > startPosition && ((DataAttribute)pair.Value).Instance.Position < endPosition)
                    if (pair.Key != Instance.ObjectKey && ((DataAttribute)pair.Value).Instance.position != 0)
                        ((DataAttribute)pair.Value).Instance.position++;
        }

        private void Decrement(int startPosition, int endPosition)
        {
            IDataAttributeCollection attributeCollection = GetAttributesCollection(Instance.Owner);

            foreach (KeyValuePair<string, IDataAttribute> pair in attributeCollection)
                if (((DataAttribute)pair.Value).Instance.Position > startPosition && ((DataAttribute)pair.Value).Instance.Position < endPosition)
                    if (pair.Key != Instance.ObjectKey && ((DataAttribute)pair.Value).Instance.position != 0)
                        ((DataAttribute)pair.Value).Instance.position--;
        }

        #endregion Контроль за последовательностью атрибутов

        #endregion Прочее

    }
}
