using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Data
{
    /// <summary>
    /// Класс предназначен для хранения заголовков элементов
    /// измерения, хранит хэш код уникального имени элемента и его заголовок
    /// </summary>
    public class MemberCaptionDictionary : Dictionary<int, string>
    {
        //из-за невозможности определить в постраничной таблице, момент когда изменилась ее структура,
        //единственный выход хранить ограниченое количество состояний. Здесь это количество и задается.
        const int countSavedItems = 5000;

        public MemberCaptionDictionary()
            : base()
        {
        }
        /// <summary>
        /// Добавляет заголовок элемента в коллекцию
        /// </summary>
        public void Add(string uniqueName, string caption)
        {
            if (!String.IsNullOrEmpty(uniqueName) && !String.IsNullOrEmpty(caption))
            {
                int key = uniqueName.GetHashCode();
                if (!this.ContainsKey(key))
                    this.Add(key, caption);
            }
        }

        /// <summary>
        /// Удаляет заголовок элемента из коллекции
        /// </summary>
        public void Remove(string uniqueName)
        {
            if (!String.IsNullOrEmpty(uniqueName))
            {
                int key = uniqueName.GetHashCode();
                if (this.ContainsKey(key))
                    this.Remove(key);
            }
        }

        /// <summary>
        /// Устанавливает заголовок элемента в коллекции
        /// </summary>
        public void SetCaption(string uniqueName, string caption)
        {
            if (!String.IsNullOrEmpty(uniqueName))
            {
                this.Remove(uniqueName);
                this.Add(uniqueName, caption);
            }
        }

        /// <summary>
        /// Содержится ли в коллекции указаный мембер
        /// </summary>
        /// <returns></returns>
        public bool ContainsMember(string uniqueName)
        {
            if (!String.IsNullOrEmpty(uniqueName))
                return this.ContainsKey(uniqueName.GetHashCode());
            else
                return false;
        }

        /// <summary>
        /// Получить заголовок элемента
        /// </summary>
        /// <returns></returns>
        public string GetCaption(string uniqueName, string defaultValue)
        {
            string result = defaultValue;
            if (!String.IsNullOrEmpty(uniqueName))
            {
                if (!this.TryGetValue(uniqueName.GetHashCode(), out result))
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
                    string value = XmlHelper.GetStringAttrValue(item, "value", String.Empty);
                    this.Add(key, value);
                }
            }
        }

        public void Save(XmlNode node)
        {
            if (node == null)
                return;

            int itemIndex = -1;
            //индекс начиная с которого сохраняем элементы словаря
            int startCopyIndex = 0;
            //если количество ключей больше максимально хранимого, будем сохранять  
            //элементы начиная с startCopyIndex индекса
            bool isCheckSavedItemsCount = this.Keys.Count > countSavedItems;
            if (isCheckSavedItemsCount)
                startCopyIndex = this.Keys.Count - countSavedItems;

            foreach (int key in this.Keys)
            {
                itemIndex++;
                if (isCheckSavedItemsCount && (itemIndex < startCopyIndex))
                    continue;

                string value = this[key];
                XmlHelper.AddChildNode(node, "item", new string[] { "key", key.ToString() },
                        new string[] { "value", value.ToString() });
            }
        }
    }

}