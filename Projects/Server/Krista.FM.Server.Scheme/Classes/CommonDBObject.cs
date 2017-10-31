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
    /// Базовый объект для объектов отношений и ассоциаций.
    /// </summary>
    internal abstract class CommonDBObject : CommonObject, ICommonDBObject
    {
        #region Поля
        
        /// <summary>
        /// ID объекта в базе данных 
        /// </summary>
        private int _ID;

        /// <summary>
        /// Текущее состояние структуры объекта
        /// </summary>
        protected DBObjectStateTypes dbObjectState = DBObjectStateTypes.Unknown;

        /// <summary>
        /// Описание разработчика объекта
        /// </summary>
        private string developerDescription = String.Empty;

        /// <summary>
        /// Если true, то объект в данный момент меняет свое состояние, 
        /// т.е. происходит изменений структуры объекта 
        /// и какие-либо действия с объектом запрещены.
        /// </summary>
        private bool inUpdating = false;

        /// <summary>
        /// Движок для работы со структурой базы данных.
        /// </summary>
        protected ScriptingEngineAbstraction _scriptingEngine;

        #endregion Поля

        #region Конструктор

        /// <summary>
        /// Инициализация объекта
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
        /// Базовый объект для объектов отношений и ассоциаций.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic">Семантика</param>
        /// <param name="name">Наименование</param>
        /// <param name="state"></param>
        /// <param name="scriptingEngine"></param>
        public CommonDBObject(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state, ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, name, state)
        {
        	this.semantic = semantic;
            _scriptingEngine = scriptingEngine;
        }

        #endregion Конструктор

        #region Редактирование
/*
        /// <summary>
        /// Перводит объект в состояние редактирования.
        /// </summary>
        /// <remarks>
        /// Создается копия объекта доступная только текущему пользователю, с которой и 
        /// производятся все дальнейшие изменения инициированные текущим пользователем.
        /// </remarks>
        /// <returns>Копия объекта</returns>
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
        /// Завершение редактирования с применением всех изменений
        /// </summary>
        public virtual void EndEdit()
        {
            Unlock();
        }

        /// <summary>
        /// Завершение редактирования с применением всех изменений
        /// </summary>
        /// <param name="comments">Комментарии к сделанным изменениям</param>
        public virtual void EndEdit(string comments)
        {
            Unlock();
        }

        /// <summary>
        /// Отмена редактирования объекта
        /// </summary>
        public virtual void CancelEdit()
        {
            Unlock();
        }
        #endregion Редактирование

        #region ICommonDBObject Members

        /// <summary>
        /// Коллекция с SQL метаданными (имена констрейнов, тригеров) объекта 
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetSQLMetadataDictionary()
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Возвращает SQL-определение объекта.
        /// </summary>
        /// <returns>SQL-определение объекта.</returns>
        public virtual List<string> GetSQLDefinitionScript()
        {
            return new List<string>();
        }

        #endregion

        #region Инициализация, создание и обновление

        /// <summary>
        /// Виртуальный метод инициализации объекта, должет быть переопределен в потомках
        /// </summary>
        /// <returns>Xml-документ содержащий конфигурацию объекта</returns>
        internal virtual XmlDocument Initialize()
        {
            if (state == ServerSideObjectStates.New && ObjectKey == Guid.Empty.ToString())
            {
                ObjectKey = Guid.NewGuid().ToString();
            }

            return Validator.LoadDocument(Configuration);
        }

        /// <summary>
        /// Метод вызывается после инициализации всех объектов схемы.
        /// </summary>
        internal virtual XmlDocument PostInitialize()
        {
            return Validator.LoadDocument(Configuration);
        }

        /// <summary>
        /// Инициализация описания разработчика
        /// </summary>
        /// <param name="doc">Документ с XML настройкой</param>
        /// <param name="tagElementName">наименование тега с настройками объекта</param>
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
        /// Создание метаданных в базе данных и кубах
        /// </summary>
        internal virtual void Create(ModificationContext context)
        {
        }

        internal virtual void Drop(ModificationContext context)
        {
        }

        /// <summary>
        /// Применение изменений. Приводит текущий объект к виду объекта toObject
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toObject">Объект к виду которого будет приведен текущий объект</param>
        public virtual void Update(ModificationContext context, IModifiable toObject)
        {
            //Trace.WriteLine(String.Format("Обновление объекта {0}", FullName));
            CommonDBObject toCommonDBObject = (CommonDBObject)toObject;

            // У объекта сменился родительский пакет
            if (Parent != null)
            {
                if (Parent.ID != toCommonDBObject.Parent.ID)
                {
                    //Trace.WriteLine(String.Format("У объекта \"{0}\" сменился родительский пакет c \"{1}\" на \"{2}\"", FullName, parent.FullName, toObject.Parent.FullName));
                    //Debugger.Break();
                }
            }

            // Семантика
            if (Semantic != toCommonDBObject.Semantic)
            {
                throw new Exception(String.Format("Английское имя у объекта \"{0}\" не может быть изменено.", FullName));
            }

            // Английское имя
            if (Name != toCommonDBObject.Name)
            {
                throw new Exception(String.Format("Английское имя у объекта \"{0}\" не может быть изменено.", FullName));
            }
            
            // Русское наименование
            if (Caption != toCommonDBObject.Caption)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилось русское наименование c \"{1}\" на \"{2}\"", FullName, Caption, toCommonDBObject.Caption));
                Caption = toCommonDBObject.Caption;
            }

            // Описание
            if (Description != toCommonDBObject.Description)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилось описание c \"{1}\" на \"{2}\"", FullName, Description, toCommonDBObject.Description));
                Description = toCommonDBObject.Description;
            }
        }

        /// <summary>
        /// Сохранение XML-конфигурации в базе данных
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
        /// Сохранение XML-конфигурации для генерации справки
        /// </summary>
        internal virtual void Save2XmlDocumentation(XmlNode node)
        {
            Save2Xml(node);
        }

        #endregion Инициализация, создание и обновление

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected CommonDBObject Instance
        {
            [DebuggerStepThrough]
            get { return (CommonDBObject)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
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
        /// Формирует список отличий (операций изменения) текущего редактируемого объекта от состояния объекта с базе данных
        /// </summary>
        /// <returns>список отличий (операций изменения)</returns>
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
        /// Формирует список отличий (операций изменения) текущего объекта от toObject
        /// </summary>
        /// <param name="toObject">Объект с которым будет производиться сравнение</param>
        /// <returns>список отличий (операций изменения)</returns>
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

        #region Свойства

        /// <summary>
        /// Родительский объект
        /// </summary>
        public Package Parent
        {
            [DebuggerStepThrough]
            get { return Owner is Package ? (Package)Owner : null; }
        }

        /// <summary>
        /// Родительский пакет
        /// </summary>
        // TODO: Vtopku 
        public IPackage ParentPackage
        {
            get { return Parent; }
        }

        /// <summary>
        /// Идентификатор объекта в базе данных
        /// </summary>
        public int ID
        {
            [DebuggerStepThrough]
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// Текущее состояние структуры объекта
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
        /// Описание разработчика объекта
        /// </summary>
        public string DeveloperDescription
        {
            [DebuggerStepThrough]
            get { return Instance.developerDescription; }
            set { SetInstance.developerDescription = value; }
        }

        /// <summary>
        /// Если tru, то объект в данный момент меняет свое состояние, 
        /// т.е. происходит изменений структуры объекта 
        /// и какие-либо дейсвия с объектом запрещены.
        /// </summary>
        public bool InUpdating
        {
            [DebuggerStepThrough]
            get { return inUpdating; }
            set { inUpdating = value; }
        }

        /// <summary>
        /// Устанавливает английское наименование
        /// </summary>
        /// <param name="oldValue">Старое наименование</param>
        /// <param name="value">Новое наименование</param>
        protected virtual void SetFullName(string oldValue, string value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void CheckFullName()
        {
        }

        /// <summary>
        /// Уникальный ключ объекта
        /// </summary>
        public string Key
        {
            get { return String.Format("{0}{1}:{2}", Owner != null && Owner is ICommonDBObject ? ((ICommonDBObject)Owner).Key + "::" : String.Empty, this.GetType().Name, GetKey(ObjectKey, FullName)); }
        }

		// Имя семантической принадлежности
		private string semantic;
		
		/// <summary>
        /// Имя семантической принадлежности
        /// </summary>
        public virtual string Semantic
        {
            [DebuggerStepThrough]
			get { return Instance.semantic; }
            set
            {
				Scheme.ScriptingEngine.ScriptingEngineHelper.CheckDBName(value);

                if (State != ServerSideObjectStates.New && !Authentication.IsSystemRole())
                    throw new Exception("Семантику можно изменять только у вновь созданных объектов.");

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
		/// Имя семантической принадлежности
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
        /// Английское уникальное имя объекта
        /// </summary>
        public override string Name
        {
            [DebuggerStepThrough]
            get { return GetterMustUseClone() ? ((CommonObject)CloneObject).Name : base.Name; }
            set
            {
                string oldName = Name;

                if (State != ServerSideObjectStates.New && !Authentication.IsSystemRole())
                    throw new Exception("Наименование можно изменять только у вновь созданных объектов.");

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
        /// Русское наименование объекта выводимое в интерфейсе
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
		/// Текстовое описание объекта выводимое в интерфейсе
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
        /// Скриптовой движок
        /// </summary>
        internal ScriptingEngineAbstraction ScriptingEngine
        {
            get { return _scriptingEngine; }
        }

        #endregion Свойства

        #region Прочее

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public abstract bool CurrentUserCanViewThisObject();

        #endregion Прочее
    }
}
