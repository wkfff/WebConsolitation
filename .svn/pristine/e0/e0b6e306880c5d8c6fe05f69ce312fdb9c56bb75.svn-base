using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
    /// <summary>
    /// Вид поступающей информации
    /// </summary>
    internal class DataKind : ServerSideObject, IDataKind, ICloneable
    {
        private DataSupplier dataSupplier;
        // код поступающей информации
        private string code;
        // наименование поступающей информации
        private string name;
        // описание поступающей информации
        private string description;
        // метод получения информации
        private TakeMethodTypes takeMethod;
        // вид параметров
        private ParamKindTypes paramKind;

        private DataKindCollection parent;

        /// <summary>
        /// Вид поступающей информации
        /// </summary>
        internal DataKind(DataSupplier dataSupplier)
            : base(null)
        {
            this.dataSupplier = dataSupplier;
        }

        /// <summary>
        /// Вид поступающей информации
        /// </summary>
        /// <param name="xmlDataKind">XML-узел с настройкани элемента</param>
        /// <param name="parent">Родительский объект-коллекция в которой находится данный объект</param>
        internal DataKind(XmlNode xmlDataKind, DataKindCollection parent, DataSupplier dataSupplier)
            : base(null)
        {
            this.dataSupplier = dataSupplier;
            this.parent = parent;
            this.code = xmlDataKind.Attributes["code"].Value;
            this.name = xmlDataKind.Attributes["name"].Value;
            if (xmlDataKind.Attributes["description"] != null) 
                this.description = xmlDataKind.Attributes["description"].Value;
            switch (xmlDataKind.Attributes["takeMethod"].Value)
            {
                case "ИМПОРТ":
                    takeMethod = TakeMethodTypes.Import;
                    break;
                case "СБОР":
                    takeMethod = TakeMethodTypes.Receipt;
                    break;
                case "ВВОД":
                    takeMethod = TakeMethodTypes.Input;
                    break;
                default:
                    throw new Exception("Неверный метод получения информации " + xmlDataKind.Attributes["takeMethod"].Value);
            }

            paramKind = DataSourceUtils.String2ParamKind(xmlDataKind.Attributes["paramKind"].Value);
        }

        /// <summary>
        /// Обновление вида поступающей информации
        /// </summary>
        internal void Update()
        {
            // Признак проверяет наличие вида в коллекции.
            if(parent != null)
                // Изменяем лишь в случае наличия вида в коллекции.
                if(parent.ContainsKey(Code))
                    parent[Code] = this;
        }

        internal DataKindCollection Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                    throw new InvalidOperationException("Нельзя изменить вид данных, который находится в коллекции");
                parent = value;
            }
        }

        #region Реализация IDataKind

        /// <summary>
        /// Поставщик данных.
        /// </summary>
        public IDataSupplier Supplier
        {
            get { return dataSupplier; }
        }

        /// <summary>
        /// Код поступающей информации
        /// </summary>
        public string Code
        {
            get { return code; }
            set 
            {
                if (parent != null)
                    throw new InvalidOperationException("Нельзя изменить код поступающих данных, который находится в коллекции");
                code = value; 
            }
        }

        /// <summary>
        /// Описание поступающей информации
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; Update(); }
        }
        /// <summary>
        /// Наименование поступающей информации
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; Update(); }
        }

        /// <summary>
        /// Метод получения информации
        /// </summary>
        public TakeMethodTypes TakeMethod
        {
            get { return takeMethod; }
            set
            {
                takeMethod = value; Update();
            }
        }

        /// <summary>
        /// Вид параметров
        /// </summary>
        public ParamKindTypes ParamKind
        {
            get { return paramKind; }
            set 
            {
                paramKind = value; Update();
            }
        }
       #endregion Реализация IDataKind

        #region ICloneable Members

        public override object Clone()
        {
            throw new Exception("The method DataKind.Clone() is not implemented.");
        }

        #endregion
    }
}
