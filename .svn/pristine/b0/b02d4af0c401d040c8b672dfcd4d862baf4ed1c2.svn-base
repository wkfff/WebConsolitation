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
    /// Поставщик данных
    /// </summary>
    class DataSupplier : ServerSideObject, IDataSupplier, ICloneable
    {
        // идентификатор поставщика данных. Максимальная длина 11 символов
        private string name;
        // описание поставщика данных
        private string description;
        // коллекция видов поступающей информации
        private DataKindCollection dataKindCollection;
        // Корневой объект-коллекция в которой находится данный объект,
        // признак того, что поставщик ноходится в колекции
        private DataSupplierCollection parent;

        /// <summary>
        /// Поставщик данных
        /// </summary>
        internal DataSupplier(ServerSideObject owner)
            : base(owner)
        {
            dataKindCollection = new DataKindCollection(this);
        }
            
        /// <summary>
        /// Поставщик данных
        /// </summary>
        /// <param name="xmlSupplier">XML-узел с настройками</param>
        /// <param name="parent">родительский объект</param>
        internal DataSupplier(XmlNode xmlSupplier, DataSupplierCollection parent)
            : base(parent)
        {
            this.parent = parent;
            this.name = GetValidatedName(xmlSupplier.Attributes["name"].Value);
            this.description = xmlSupplier.Attributes["description"].Value;

            dataKindCollection = new DataKindCollection(xmlSupplier.SelectNodes("DataKind"), this);
         }

        /// <summary>
        /// Обновлении поставщика данных.
        /// </summary>
       public void Update()
       {
           // Признак проверяет наличие поставщика в коллекции.
           if (parent != null)
               // Изменяем лишь в случае наличия поставщика в коллекции.
                if (parent.ContainsKey(Name))
                    parent[Name] = this;
       }

        /// <summary>
        /// Проверяет на корректность идентификатор поставщика данных.
        /// </summary>
        /// <param name="name">Идентификатор поставщика данных</param>
        /// <returns>Корректный идентификатор поставщика данных</returns>
        /// <exception cref="System.Exception">Генерируется если идентификатор превышает 11 символов</exception>
        private string GetValidatedName(string name)
        {
            if (name.Length > 11)
                throw new Exception("Длина идентификатора поставщика данных не должена превышать 11 символов.");
            
            foreach (char c in name)
            {
                // Если в имени не только цифры
                if (!char.IsDigit(c))
                {
                    // возвращаем имя.
                    return name.ToUpper();
                }
            }
            throw new Exception("Идентификатор поставщика данных не может состоять только из цифр.");
        }

        /// <summary>
        /// Идентификатор поставщика данных. Максимальная длина 11 символов
        /// </summary>
        public string Name
        {
            get { return name; }
            set 
            {
                if (parent != null)
                    throw new InvalidOperationException("Нельзя изменить имя поставщика данных, который находится в коллекции");
                name = GetValidatedName(value); 
            }
        }

        /// <summary>
        /// Описание поставщика данных
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; Update(); }
        }

        /// <summary>
        /// Коллекция видов поступающей информации
        /// </summary>
        public IDataKindCollection DataKinds
        {
            get { return dataKindCollection; }
        }

        /// <summary>
        /// Возвращает корневой объект-коллекция, в которой находится данный объект
        /// </summary>
        public DataSupplierCollection Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                    throw new InvalidOperationException("Нельзя изменить поставщика данных, который находится в коллекции");
                parent = value;
            }
        }

        #region ICloneable Members

        public override object Clone()
        {
            DataSupplier clone = (DataSupplier)base.Clone();
            return clone;
        }

        #endregion
    }
}
