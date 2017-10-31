using System;
using System.Diagnostics;

using Krista.FM.Server.Common;
using Krista.FM.Server.Users;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
	/// Обший класс предок для остальных классов схемы.
	/// </summary>
    [DebuggerStepThrough]
    internal abstract class CommonObject : KeyIdentifiedObject, ICommonObject
	{
        // Имя объекта
        private string name;
		// Описание объекта
        private string description = String.Empty;
        // Название объекта
        private string caption = String.Empty;
		// XML конфигурация объекта
        private string xmlConfig = String.Empty;

		/// <summary>
		/// Конструкто базового класса
		/// </summary>
		/// <param name="key"></param>
		/// <param name="owner"></param>
		/// <param name="name">Имя объекта</param>
        /// <param name="state"></param>
        public CommonObject(string key, ServerSideObject owner, string name, ServerSideObjectStates state)
            : base(key, owner, state)
		{
            this.name = name;
		}

        /// <summary>
        /// Регистрация объекта в системе учета прав
        /// </summary>
        public virtual void RegisterObject(string objectName, string objectCaption, SysObjectsTypes sysObjectsType)
        {
            try
            {
                ((UsersManager)SchemeClass.Instance.UsersManager).RegisterSystemObject(objectName, objectCaption, sysObjectsType);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Неудалось зарегистрировать объект {0}: {1}", objectName, e));
            }
        }

        /// <summary>
        /// Удаление объекта в системе учета прав
        /// </summary>
        /// <param name="objectName"></param>
        public virtual void UnRegisterObject(string objectName)
        {
            try
            {
                ((UsersManager)SchemeClass.Instance.UsersManager).UnregisterSystemObject(objectName);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Не удалось удалить объект {0}: {1}", objectName, e));
            }
		}

		/// <summary>
		/// XML конфигурация объекта
		/// </summary>
		public virtual string Configuration
		{
            [DebuggerStepThrough]
            get { return xmlConfig; }
            [DebuggerStepThrough]
            set { xmlConfig = value; }
		}

		#region Реализация ICommonObject

        /// <summary>
		/// Английское уникальное имя объекта
		/// </summary>
        public virtual string Name 
		{
            [DebuggerStepThrough]
            get { return name; }
            set
            {
				// Вариант не из лучших, но...
                if (!(this is Document) && !(this is Package) && !(this is EntityAssociation)) 
                    ScriptingEngine.ScriptingEngineHelper.CheckDBName(value);
                name = value;
            }
        }

        /// <summary>
        /// Полное имя объекта.
        /// </summary>
        public virtual string FullName
        {
            [DebuggerStepThrough]
            get { return name; }
        }

        /// <summary>
        /// Уникальное наименование объекта (старый ключ).
        /// Это свойство после выпуска версии 2.4.1 необходимо удалить.
        /// </summary>
        public override string ObjectOldKeyName
        {
            get { return FullName; }
        }

        /// <summary>
        /// Полное имя объекта в базе данных
        /// </summary>
        public virtual string FullDBName
        {
            [DebuggerStepThrough]
            get { return FullName.Replace('.', '_'); }
        }

        /// <summary>
		/// Определяет состояние корректности объекта
		/// </summary>
		public virtual bool IsValid
		{ 
			get	{ return true; }
		}
		
		/// <summary>
		/// Русское наименование объекта выводимое в интерфейсе
		/// </summary>
		public virtual string Caption
		{
            [DebuggerStepThrough]
            get { return caption; }
			set	{ caption = value; }
		}

        /// <summary>
		/// Текстовое описание объекта выводимое в интерфейсе
		/// </summary>
		public virtual string Description
		{
            [DebuggerStepThrough]
            get { return description; }
			set	
            { 
                description = value;
                if (!String.IsNullOrEmpty(description))
                    if (description[description.Length - 1] != '.')
                        description += '.';
            }
		}

        /// <summary>
        /// XML конфигурация объекта
        /// </summary>
        public virtual string ConfigurationXml
        {
            get { return String.Empty; }
        }

        #endregion
    }
}
