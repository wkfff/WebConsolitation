// ******************************************************************
// Модуль содержит базовые абстрактные классы для объектов и коллекций,
// используемых для представления программы закачки.
// Ввиду ограниченности области использования объектов и для улучшения читаемости 
// кода используются поля (не свойства) с областью видимости internal
// ******************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region Базовые абстрактные классы

    /// <summary>
    /// Самый базовый класс для любой сущности программы закачки.
    /// Имеет имя, умеет загружаться из XML, освобождать ресурсы и 
    /// осущесвлять проверку корректности состояния
    /// </summary>
    public abstract class ProcessObject
    {
        internal PumpProgram parentProgram;

        /// <summary>
        /// Название тэга XML для узла <имя>
        /// </summary>
        internal string nameTagName = XmlConsts.nameAttr;

        /// <summary>
        /// имя объекта
        /// </summary>
        internal string name;

        protected ProcessObject()
        {
        }

        /// <summary>
        /// Общедоступный конструктор
        /// </summary>
        /// <param name="nameTagName">Название тэга XML для узла <имя></param>
        public ProcessObject(PumpProgram parentProgram, string nameTagName)
        {
            this.parentProgram = parentProgram;
            if (!String.IsNullOrEmpty(nameTagName))
                this.nameTagName = nameTagName;
        }

        /// <summary>
        /// Загрузить объект из XML
        /// </summary>
        /// <param name="node">узел</param>
        internal virtual void LoadFromXml(XmlNode node)
        {
            this.name = XmlHelper.GetStringAttrValue(node, nameTagName, String.Empty);
        }

        /// <summary>
        /// Проверить корректность состояния объекта
        /// </summary>
        /// <param name="pumpModule">родительский модуль закачки, используется для доступа в общим методм, схеме и т.п.</param>
        /// <returns></returns>
        internal virtual string Validate()
        {
            return String.Empty;
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        internal virtual void Clear()
        {
        }
    }

    /// <summary>
    /// Объект с "синонимом"
    /// </summary>
    public abstract class ProcessObjectWithSynonym : ProcessObject
    {
        /// <summary>
        /// "синоним", используется в качестве ключа в родительских коллекциях
        /// </summary>
        internal string synonym;

        public ProcessObjectWithSynonym(PumpProgram parentProgram, string nameTagName) : base (parentProgram, nameTagName)
        {
        }

        /// <summary>
        /// Загрузить объект из XML
        /// </summary>
        /// <param name="node">узел</param>
        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.synonym = XmlHelper.GetStringAttrValue(node, XmlConsts.synonymAttr, String.Empty);
        }
    }
    #endregion

    #region Базовые абстрактные коллекции и списки
    /// <summary>
    /// Группа объектов (список или коллекция)
    /// </summary>
    public abstract class ObjectsGroup : IEnumerable
    {
        /// <summary>
        /// Название тэга XML для выборки одного элемента
        /// </summary>
        protected string elemTagName;
        
        /// <summary>
        /// Тип элементов (для унифицированного создания)
        /// </summary>
        protected Type elemType;

        /// <summary>
        /// Скрытый конструктор без параметров
        /// </summary>
        protected ObjectsGroup()
        {
        }

        internal PumpProgram parentProgram;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="elemTagName">Название тэга XML для выборки одного элемента</param>
        /// <param name="elemType">Тип элементов группы</param>
        protected ObjectsGroup(PumpProgram parentProgram, string elemTagName, Type elemType)
        {
            this.parentProgram = parentProgram;
            // элементы должны быть потомками ProcessObject
            if (!elemType.IsSubclassOf(typeof(ProcessObject)))
                throw new Exception(String.Format("Класс '{0}' не является наследником '{1}'",
                    elemType.FullName, typeof(ProcessObject).FullName));
            this.elemTagName = elemTagName;
            this.elemType = elemType;
        }

        /// <summary>
        /// Создание экземпляра нового элемента
        /// </summary>
        /// <returns>новый элемент</returns>
        private ProcessObject CreateNewElem()
        {
            return (ProcessObject)Activator.CreateInstance(elemType, this.parentProgram);
        }

        /// <summary>
        /// Добавление нового элемента (для внутреннего использования)
        /// </summary>
        /// <param name="elem">Новый элемент</param>
        protected abstract void AddNewElemInternal(ProcessObject elem);

        /// <summary>
        /// Добавление нового элемента
        /// </summary>
        /// <param name="elem">Новый элемент</param>
        public void AddNew(ProcessObject elem)
        {
            AddNewElemInternal(elem);
        }

        /// <summary>
        /// Загрузить группу элементов из XML
        /// </summary>
        /// <param name="parentNode">узел XML</param>
        public virtual void LoadFromXml(XmlNode parentNode)
        {
            // получаем все дочерние узлы с заданным значением тэга
            XmlNodeList childs = parentNode.SelectNodes(elemTagName);
            // последовательно добавляем их во внутреннее хранилище
            foreach (XmlNode node in childs)
            {
                ProcessObject obj = CreateNewElem();
                obj.LoadFromXml(node);
                AddNewElemInternal(obj);
            }
        }

        /// <summary>
        /// Получить енумератор (для использования в циклах foreach и т.п.)
        /// Должен быть реализован в потомках
        /// </summary>
        /// <returns>енумератор</returns>
        public abstract IEnumerator GetEnumerator();

        /// <summary>
        /// Проверить корректность состояния группы и всех ее элементов
        /// </summary>
        /// <param name="pumpModule">родительский модуль закачки (для вызова общим методов, доступа к схеме и т.п.)</param>
        /// <returns>Строка с сообщением об ошиках, String.Empty если ошибок нет</returns>
        public virtual string Validate()
        {
            // запрашиваем енумератор
            IEnumerator en = this.GetEnumerator();
            // создаем накопитель собщений об ошибках
            StringBuilder sb = new StringBuilder();
            // последовательно перебираем все дочерние элементы
            en.Reset();
            while (en.MoveNext())
            {
                ProcessObject po = (ProcessObject)en.Current;
                // .. и верифицируем их состояние
                string currErr = po.Validate();
                if (!String.IsNullOrEmpty(currErr))
                {
                    sb.AppendLine(currErr);
                    sb.AppendLine(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Очистка ресурсов.
        /// Должна быть переопределена в потомках
        /// </summary>
        public abstract void Clear();

        public abstract int Count { get; }
    }

    /// <summary>
    /// Коллекция объектов <синононим>, <ProcessObject>
    /// </summary>
    public abstract class ObjectsCollection : ObjectsGroup
    {
        /// <summary>
        /// Внутренняя коллекция
        /// </summary>
        private Dictionary<string, ProcessObjectWithSynonym> dictionary = new Dictionary<string, ProcessObjectWithSynonym>();

        protected ObjectsCollection(PumpProgram parentProgram, string elemTagName, Type elemType)
            : base(parentProgram, elemTagName, elemType)
        {
        }

        protected override void AddNewElemInternal(ProcessObject elem)
        {
            ProcessObjectWithSynonym newElem = (ProcessObjectWithSynonym)elem;
            dictionary.Add(newElem.synonym, newElem);
        }

        public override IEnumerator GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        public override void Clear()
        {
            foreach (ProcessObject po in dictionary.Values)
            {
                po.Clear();
            }
            dictionary.Clear();
        }

        /// <summary>
        /// Получить объект по синониму. Стандартный индексатор
        /// </summary>
        /// <param name="synonym">Синоним</param>
        /// <returns>Объект</returns>
        public ProcessObject this[string synonym]
        {
            get
            {
                return dictionary[synonym];
            }
        }

        /// <summary>
        /// Получить объект по имени. Используется редко, основной метод доступа - через синоним
        /// </summary>
        /// <param name="name">имя объекта</param>
        /// <returns>Объект</returns>
        public ProcessObject ObjectByName(string name)
        {
            foreach (ProcessObject po in this)
            {
                if (String.Compare(name, po.name, true) == 0)
                    return po;
            }
            return null;
        }

        public override int Count
        {
            get { return dictionary.Count; }
        }
    }

    /// <summary>
    /// Список объектов
    /// </summary>
    public abstract class ObjectsList : ObjectsGroup
    {
        /// <summary>
        /// Внутренний список
        /// </summary>
        private List<ProcessObject> list = new List<ProcessObject>();

        protected ObjectsList(PumpProgram parentProgram, string elemTagName, Type elemType)
            : base(parentProgram, elemTagName, elemType)
        {
        }

        protected override void AddNewElemInternal(ProcessObject elem)
        {
            list.Add(elem);
        }

        public override IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public override void Clear()
        {
            foreach (ProcessObject po in list)
            {
                po.Clear();
            }
            list.Clear();
        }

        public ProcessObject this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public override int Count
        {
            get {return list.Count; }
        }

    }
    #endregion

}