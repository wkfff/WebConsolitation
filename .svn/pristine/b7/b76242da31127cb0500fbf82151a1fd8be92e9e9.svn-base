using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Класс предназначен для хранения признака схлопнутости/расхлопнутости элементов
    /// измерения, хранит хэш код уникального имени элемента и состояние развертываемести
    /// </summary>
    public class MembersExpandDictionary : Dictionary<int, bool>
    {
        //из-за невозможности определить в постраничной таблице, момент когда изменилась ее структура,
        //единственный выход хранить ограниченое количество состояний. Здесь это количество и задается.
        const int countSavedItems = 5000;
 
        public MembersExpandDictionary()
            : base()
        {
        }
        /// <summary>
        /// Добавляет состояниел элемента в коллекцию, запоминаем только схлопнутые
        /// </summary>
        /// <param name="DC"></param>
        public void Add(DimensionCell DC)
        {
            if (DC != null)
            {
                int key = DC.AllParentHashCode;
                if (!this.ContainsKey(key))
                    this.Add(key, DC.Expanded);
            }
        }

        /// <summary>
        /// Удаляет состояние элемента из коллекции
        /// </summary>
        /// <param name="DC"></param>
        public void Remove(DimensionCell DC)
        {
            if (DC != null)
            {
                int key = DC.AllParentHashCode;
                if (this.ContainsKey(key))
                    this.Remove(key);
            }
        }

        /// <summary>
        /// Устанавливает состояние элемента в коллекции
        /// </summary>
        /// <param name="DC"></param>
        public void SetState(DimensionCell DC)
        {
            if (DC != null)
            {
                this.Remove(DC);
                this.Add(DC);
            }
        }

        /// <summary>
        /// Содержится ли в коллекции указаный мембер
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public bool ContainsMember(DimensionCell DC)
        {
            if (DC != null)
                return this.ContainsKey(DC.AllParentHashCode);
            else
                return false;
        }

        /// <summary>
        /// Получить состояние развернутости ячейки
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public bool GetState(DimensionCell DC, bool defaultValue)
        {
            bool result = defaultValue;
            if (DC != null)
            {
                if (!this.TryGetValue(DC.AllParentHashCode, out result))
                    result = defaultValue;
            }
            return result;
        }

        public void Load(XmlNode node)
        {
            if (node == null)
                return;

            this.Clear();
            XmlNodeList itemList = node.SelectNodes("item");
            foreach (XmlNode item in itemList)
            {
                int key = XmlHelper.GetIntAttrValue(item, "key", -1);
                if (key != -1)
                {
                    bool value = XmlHelper.GetBoolAttrValue(item, "value", false);
                    this.Add(key, value);
                }
            }
        }

        public void Save(XmlNode node, SaveMode saveMode)
        {
            if (node == null)
                return;

            int itemIndex = -1;
            //индекс начиная с которого сохраняем элементы словаря
            int startCopyIndex = 0;
            //если количество ключей больше максимально хранимого, будем сохрянять  
            //элементы начиная с startCopyIndex индекса
            bool isCheckSavedItemsCount = this.Keys.Count > countSavedItems;
            if (isCheckSavedItemsCount)
                startCopyIndex = this.Keys.Count - countSavedItems;

            foreach (int key in this.Keys)
            {
                itemIndex++;
                if (isCheckSavedItemsCount && (itemIndex < startCopyIndex))
                    continue;

                bool value = this[key];
                if ((saveMode == SaveMode.All) || ((saveMode == SaveMode.OnlyFalse) && !value) ||
                    ((saveMode == SaveMode.OnlyTrue) && value))
                {
                    XmlHelper.AddChildNode(node, "item", new string[] { "key", key.ToString() },
                        new string[] { "value", value.ToString() });
                }
            }
        }
    }

    public enum SaveMode
    {
        All,
        OnlyTrue,
        OnlyFalse
    }
}