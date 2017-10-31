using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    internal abstract class DimensionLevel : MinorObject, IDimensionLevel, IComparable<IDimensionLevel>
    {
        // наименование уровня
        private string name;
        // описание
        private string description;
        // имя поля содержащее ключ
        private IDataAttribute memberKey;
        // имя поля содержащее имя
        private IDataAttribute memberName;
        // тип уровня
        private LevelTypes levelType;


        public DimensionLevel(string key, ServerSideObject owner)
            : base(key, owner, owner.State)
        {
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected DimensionLevel Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (DimensionLevel)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected DimensionLevel SetInstance
        {
            get
            {
                if (!(Authentication.IsSystemRole()) && this.name == "Данные всех источников")
                    throw new Exception("Нельзя редактировать уровень \"Данные всех источников\"");
                return SetterMustUseClone() ? (DimensionLevel)CloneObject : this;
            }
        }

        public override IServerSideObject Lock()
        {
            Classifier cloneClassifier = (Classifier)Owner.Lock();
            return (ServerSideObject)cloneClassifier.Levels[Name];
        }

        #endregion ServerSideObject

        /// <summary>
        /// Классификатор в котором находится уровень
        /// </summary>
        public IClassifier Parent
        {
            get { return (IClassifier)Owner; }
        }

        /// <summary>
        /// Устанавливает ключ в родительской коллекции
        /// </summary>
        /// <param name="oldValue">Старое наименование</param>
        /// <param name="value">Новое наименование</param>
        protected void SetKeyName(string oldValue, string value)
        {
            Classifier cls = (Classifier)Instance.Parent;

            KeyValuePair<string, IDimensionLevel>[] levelsCollection = new KeyValuePair<string, IDimensionLevel>[cls.Levels.Count];
            cls.Levels.CopyTo(levelsCollection, 0);
            cls.Levels.Clear();
            try
            {
                foreach (KeyValuePair<string, IDimensionLevel> level in levelsCollection)
                {
                    cls.Levels.Add(level.Value.Name, level.Value);
                }
            }
            catch (Exception e)
            {
                //levelsCollection.CopyTo(cls.Levels, 0);
                throw new Exception(e.Message, e);
            }
        }

        public string Name
        {
            get { return Instance.name; }
            set 
            {
				ScriptingEngine.ScriptingEngineHelper.CheckOlapName(value, ".,;'`:/\\*|?\"&%$!-+=[]{}".ToCharArray());

                string oldValue = Name;
                SetInstance.name = value;
                if (!String.IsNullOrEmpty(oldValue))
                {
                    SetKeyName(oldValue, Name);
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

        public string Description
        {
            get { return Instance.description; }
            set { SetInstance.description = value; }
        }

        public IDataAttribute MemberKey
        {
            get { return Instance.memberKey; }
            set { SetInstance.memberKey = value; }
        }

        public IDataAttribute MemberName
        {
            get { return Instance.memberName; }
            set { SetInstance.memberName = value; }
        }

        public LevelTypes LevelType
        {
            get { return Instance.levelType; }
            set { SetInstance.levelType = value; }
        }

        public abstract string LevelNamingTemplate
        {
            get;
            set;
        }

        public abstract IDataAttribute ParentKey
        {
            get;
            set;
        }

        #region IComparable<IDimensionLevel> Members

        public virtual int CompareTo(IDimensionLevel other)
        {
            if (levelType != other.LevelType)
                return 1;
            if (levelType == LevelTypes.All)
                return 0;
            if (memberKey.Name != other.MemberKey.Name)
                return 1;
            if (memberName.Name != other.MemberName.Name)
                return 1;
            if (description != other.Description)
                return 1;
            if (ObjectKey != other.ObjectKey)
                return 1;
            return 0;
        }

        #endregion
    }

    internal class ParentChildLevel : DimensionLevel
    {
        // имя поля содержащее родительский ключ
        private IDataAttribute parentKey;
        // определяет названия уровней
        private string levelNamingTemplate = String.Empty;


        public ParentChildLevel(string key, ServerSideObject owner)
            : base(key, owner)
        {
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new ParentChildLevel Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (ParentChildLevel)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new ParentChildLevel SetInstance
        {
            get { return SetterMustUseClone() ? (ParentChildLevel)CloneObject : this; }
        }

        #endregion ServerSideObject

        public override string LevelNamingTemplate
        {
            get { return Instance.levelNamingTemplate; }
            set { SetInstance.levelNamingTemplate = value; }
        }

        public override IDataAttribute ParentKey
        {
            get { return Instance.parentKey; }
            set { SetInstance.parentKey = value; }
        }

        public override int CompareTo(IDimensionLevel other)
        {
            if (ParentKey.Name != other.ParentKey.Name)
                return 1;
            if (LevelNamingTemplate != other.LevelNamingTemplate)
                return 1;
            return base.CompareTo(other);
        }
    }

    internal class RegularLevel : DimensionLevel
    {
        public RegularLevel(string key, ServerSideObject owner)
            : base(key, owner)
        {
        }

        public override string LevelNamingTemplate
        {
            get { throw new Exception("Объект данного типа не поддерживает эту функцию."); }
            set { throw new Exception("Объект данного типа не поддерживает эту функцию."); }
        }

        public override IDataAttribute ParentKey
        {
            get { throw new Exception("Объект данного типа не поддерживает эту функцию."); }
            set { throw new Exception("Объект данного типа не поддерживает эту функцию."); }
        }
    }
}
