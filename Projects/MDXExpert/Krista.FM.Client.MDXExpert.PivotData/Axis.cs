using System.Collections.Generic;
using System.Xml;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Data
{
    /// <summary>
    /// Ось структуры(абстрактная)
    /// </summary>
    public abstract class Axis : PivotObject
    {
        protected AxisType axisType;
        private string caption;
        private FieldSetCollection fieldSets;

        /// <summary>
        /// Вроде эта функция программе не нужна.
        /// Перекрыто, что бы в проперти гриде не маячило ненужная надпись.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Empty;
        }

        public AxisType AxisType
        {
            get 
            { 
                return axisType; 
            }
        }

        public string Caption
        {
            get 
            { 
                return caption; 
            }
            set 
            { 
                caption = value;
            }
        }

        public FieldSetCollection FieldSets
        {
            get { return fieldSets; }
            set { fieldSets = value; }
        }

        public virtual bool IsEmpty
        {
            get { return this.FieldSets.Count == 0; }
        }

        public abstract void Clear();

        //public abstract bool ObjectIsPresent(string objectName);

        public abstract PivotObject GetPivotObject(string objectName);

        public abstract XmlNode SaveSettingsXml(XmlNode parentNode);

        public abstract void SetXml(XmlNode node);

        /// <summary>
        /// Добавление списка юникнеймов элементов в список узлов
        /// </summary>
        /// <param name="root">родительский узел</param>
        /// <param name="nameList">список узлов</param>
        protected void AddNameListToNodes(XmlNode root, List<string> nameList)
        {
            XmlNode nameNode;
            foreach (string uniqueName in nameList)
            {
                nameNode = XmlHelper.AddChildNode(root, "uname", uniqueName, null);
            }
        }

        /// <summary>
        /// Получение списка юникнеймов из узлов
        /// </summary>
        /// <param name="root">родительский узел для узлов, в кот. записаны юникнеймы</param>
        /// <returns>список юникнеймов</returns>
        protected List<string> GetNameListFromNodes(XmlNode root)
        {
            List<string> result = new List<string>();

            if (root != null)
            {
                foreach (XmlNode node in root.SelectNodes("uname"))
                {
                    result.Add(node.InnerText);
                }
            }

            return result;
        }

    }

}
