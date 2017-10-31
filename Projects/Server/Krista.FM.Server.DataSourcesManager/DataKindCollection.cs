using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
    /// <summary>
    /// Коллекция видов поступающей информации
    /// </summary>
    class DataKindCollection : DictionaryBase<string, IDataKind>, IDataKindCollection
    {
        // Корневой объект-коллекция в которой находится данный объект
        private DataSupplier parent;
     
        /// <summary>
        /// Коллекция видов поступающей информации
        /// </summary>
        public DataKindCollection(DataSupplier parent)
            : base(parent, ServerSideObjectStates.Consistent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Коллекция видов поступающей информации
        /// </summary>
        /// <param name="xmlDataKinds">XML-узел с настройкани элемента</param>
        /// <param name="parent">Корневой объект-коллекция в которой находится данный объект</param>
        public DataKindCollection(XmlNodeList xmlDataKinds, DataSupplier parent)
            : base(parent, ServerSideObjectStates.Consistent)
        {
            this.parent = parent;

            foreach (XmlNode xmlDataKind in xmlDataKinds)
            {
                DataKind dataKind = new DataKind(xmlDataKind, this, parent);
                list.Add(dataKind.Code, dataKind);
            }
        }

        /// <summary>
        /// Создание вида
        /// </summary>
        /// <returns>Возвращает вид данных</returns>
        public IDataKind New()
        {
            return new DataKind(parent);
        }

        /// <summary>
        /// Добавление вида поступающей информации
        /// </summary>
        /// <param name="key">Имя вида</param>
        /// <param name="value">Атрибуты вида</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Add(string key, IDataKind value)
        {
            try
            {
                if (Krista.FM.Common.ClientAuthentication.UserID != Parent.LockedByUserID)
                    throw new ArgumentException("Для внесения изменений объект необходимо заблокировать");

                // Проверка на наличии вида поступающей информации в коллекции
                if (list.ContainsKey(key))
                    throw new ArgumentException(String.Format("Запись с именем {0} уже существует", key));

                // Проверка заполнения обязательных аттрибутов
                if (String.IsNullOrEmpty(value.Code))
                    throw new Exception("Аттрибут code обязательный для заполнения");
                if (String.IsNullOrEmpty(value.Name))
                    throw new Exception("Аттрибут name обязательный для заполнения");
               
                XmlDocument doc = XmlHelper.Load(Parent.Parent.DataSourceManager.DataSupliersFilePath);
                                
                XmlElement elem = doc.CreateElement("DataKind");
                              
                XmlNode attrCode = doc.CreateNode(XmlNodeType.Attribute, "code", "");
                attrCode.Value = value.Code;
                elem.Attributes.SetNamedItem(attrCode);

                XmlNode attrName = doc.CreateNode(XmlNodeType.Attribute, "name", "");
                attrName.Value = value.Name;
                elem.Attributes.SetNamedItem(attrName);

                // Атрибут Description не обязательный
                if (value.Description != null)
                {
                    XmlNode attrDescription = doc.CreateNode(XmlNodeType.Attribute, "description", "");
                    attrDescription.Value = value.Description;
                    elem.Attributes.SetNamedItem(attrDescription);
                }

                XmlNode attrtype = doc.CreateNode(XmlNodeType.Attribute, "takeMethod", "");
                attrtype.Value = DataSourceUtils.TakeMethod2String(value.TakeMethod);
                elem.Attributes.SetNamedItem(attrtype);

                XmlNode attrparamKind = doc.CreateNode(XmlNodeType.Attribute, "paramKind", "");
                attrparamKind.Value = DataSourceUtils.ParamKind2String(value.ParamKind);
                elem.Attributes.SetNamedItem(attrparamKind);

                doc.SelectSingleNode(String.Format("/DataSourcesConfiguration/Suppliers/Supplier[@name = '{0}']", parent.Name)).AppendChild(elem);

                XmlHelper.Save(doc, Parent.Parent.DataSourceManager.DataSupliersFilePath);

                // Добавляем в коллекцию видов поступающей информации новый вид,
                // только в случае успешного добавления в Xml.
                list.Add(key, value);

                // Новому виду присваивается текущая коллекция
                ((DataKind)value).Parent = this;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при добавлении нового вида поступающей информации {0} : ", e.Message));
            }
        }

        public void Add(IDataKind value)
        {
            Add(value.Code, value);
        }

        /// <summary>
        /// Удаление вида поступающей информации
        /// </summary>
        /// <param name="key">Имя вида</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Remove(string key)
        {
            try
            {
                if (Krista.FM.Common.ClientAuthentication.UserID != Parent.LockedByUserID)
                    throw new ArgumentException("Для внесения изменений объект необходимо заблокировать");

                if (!list.ContainsKey(key))
                    return false;
                    
                XmlDocument doc = XmlHelper.Load(Parent.Parent.DataSourceManager.DataSupliersFilePath);

                XmlNode root = doc.SelectSingleNode(String.Format("/DataSourcesConfiguration/Suppliers/Supplier[@name = '{0}']", parent.Name));
                root.RemoveChild(doc.SelectSingleNode(String.Format("/DataSourcesConfiguration/Suppliers/Supplier[@name = '{1}']/DataKind[@code = '{0}']", key, parent.Name)));

                XmlHelper.Save(doc, Parent.Parent.DataSourceManager.DataSupliersFilePath);

                return list.Remove(key);
            }
            
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при удалении вида поступающей информации {0} : ", e.Message));
            }
        }

        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="key">Имя вида</param>
        /// <returns></returns>
        public override IDataKind this[string key]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                try
                {
                    if (Krista.FM.Common.ClientAuthentication.UserID != Parent.LockedByUserID)
                        throw new ArgumentException("Для внесения изменений объект необходимо заблокировать");

                    XmlDocument doc = XmlHelper.Load(Parent.Parent.DataSourceManager.DataSupliersFilePath);

                    XmlNode xmlDataKind = doc.SelectSingleNode(String.Format("/DataSourcesConfiguration/Suppliers/Supplier[@name = '{1}']/DataKind[@code = '{0}']", key, parent.Name));
                    xmlDataKind.Attributes["code"].Value = value.Code;
                    xmlDataKind.Attributes["name"].Value = value.Name;
                    if (value.Description != null)
                    {
                        //если аттрибута нет..
                        if (xmlDataKind.Attributes["description"] == null)
                        {
                            XmlNode attrDescription = doc.CreateNode(XmlNodeType.Attribute, "description", "");
                            attrDescription.Value = value.Description;
                            xmlDataKind.Attributes.SetNamedItem(attrDescription);  
                        }
                        else
                            xmlDataKind.Attributes["description"].Value = value.Description;
                    }
                    xmlDataKind.Attributes["takeMethod"].Value = DataSourceUtils.TakeMethod2String(value.TakeMethod);
                    xmlDataKind.Attributes["paramKind"].Value = DataSourceUtils.ParamKind2String(value.ParamKind);

                    XmlHelper.Save(doc, Parent.Parent.DataSourceManager.DataSupliersFilePath);

                    list[key] = value;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(String.Format("При изменении элемента с именем {1}  возникла ошибка : {0} ", e.Message, key));
                    throw;
                }
            }
        }

       /// <summary>
        /// Возвращает корневой объект-коллекция, в которой находится данный объект
       /// </summary>
        public DataSupplier Parent
        {
            get { return parent; }
        }

    }
}
