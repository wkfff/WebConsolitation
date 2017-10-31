using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
    /// <summary>
    /// Коллекция поставщиков данных
    /// </summary>
    class DataSupplierCollection : DictionaryBase<string, IDataSupplier>, IDataSupplierCollection
    {
        // Родительский объект
        private DataSourceManager dataSourceManager;
       
        /// <summary>
        /// Конструктор объекта
        /// </summary>
        public DataSupplierCollection(DataSourceManager dataSourceManager)
            : base(null, ServerSideObjectStates.Consistent)
        {
            this.dataSourceManager = dataSourceManager;
            Initialize();
        }

        /// <summary>
        /// Создание поставщика данных
        /// </summary>
        /// <returns> Возвращает поставщика данных</returns>
        public IDataSupplier New()
        {
            return new DataSupplier(this);
        }
              
        /// <summary>
        /// Добавление поставщика
        /// </summary>
        /// <param name="key">Имя</param>
        /// <param name="value">Описание поставщика</param>
        [MethodImpl (MethodImplOptions.Synchronized)]
        public override void Add(string key, IDataSupplier value)
        {
            try
            {
                if (Krista.FM.Common.ClientAuthentication.UserID != this.LockedByUserID)
                    throw new ArgumentException("Для внесения изменений объект необходимо заблокировать");
                
                if (list.ContainsKey(key))
                    throw new ArgumentException(String.Format("Запись с именем {0} уже существует", key));

                // Проверка заполнения обязательных аттрибутов
                if (String.IsNullOrEmpty(value.Name))
                    throw new Exception("Аттрибут name обязательный для заполнения");

                XmlDocument doc = XmlHelper.Load(dataSourceManager.DataSupliersFilePath);

                XmlElement elem = doc.CreateElement("Supplier");
                
                XmlNode attrName = doc.CreateNode(XmlNodeType.Attribute, "name", "");
                attrName.Value = value.Name;
                elem.Attributes.SetNamedItem(attrName);

                if (value.Description != null)
                {
                    XmlNode attrCap = doc.CreateNode(XmlNodeType.Attribute, "description", "");
                    attrCap.Value = value.Description;
                    elem.Attributes.SetNamedItem(attrCap);
                }
                
                doc.SelectSingleNode("/DataSourcesConfiguration/Suppliers").AppendChild(elem);

                XmlHelper.Save(doc, dataSourceManager.DataSupliersFilePath);

                list.Add(key, value);

                ((DataSupplier)value).Parent = this;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при добавлении нового поставщика {0} : ", e.Message));
            }
        }

        /// <summary>
        /// Добавление поставщика
        /// </summary>
        /// <param name="value">Описание поставщика</param>
        public void Add(IDataSupplier value)
        {
            Add(value.Name, value);
        }

        /// <summary>
        /// Удаление поставщика из коллекции
        /// </summary>
        /// <param name="key">Имя поставщика</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Remove(string key)
        {
            try
            {
                if (Krista.FM.Common.ClientAuthentication.UserID != this.LockedByUserID)
                    throw new ArgumentException("Для внесения изменений объект необходимо заблокировать");

                if (!list.ContainsKey(key))
                    return false;

                XmlDocument doc = XmlHelper.Load(dataSourceManager.DataSupliersFilePath);

                XmlNode xmlSupplier = doc.SelectSingleNode("/DataSourcesConfiguration/Suppliers");
                xmlSupplier.RemoveChild(doc.SelectSingleNode(String.Format("/DataSourcesConfiguration/Suppliers/Supplier[@name = '{0}']", key)));

                XmlHelper.Save(doc, dataSourceManager.DataSupliersFilePath);

                return list.Remove(key);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при удалении поставщика {0} : ", e.Message));
            }
        }

        /// <summary>
        /// Индексатор для обращения к поставщику
        /// </summary>
        /// <param name="key">Имя</param>
        /// <returns></returns>
        public override IDataSupplier this[string key]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                try
                {
                    if (Krista.FM.Common.ClientAuthentication.UserID != this.LockedByUserID)
                        throw new ArgumentException("Для внесения изменений объект необходимо заблокировать");

                    XmlDocument doc = XmlHelper.Load(dataSourceManager.DataSupliersFilePath);

                    XmlNode xmlSupplier = doc.SelectSingleNode(String.Format("/DataSourcesConfiguration/Suppliers/Supplier[@name = '{0}']", key));
                    xmlSupplier.Attributes["name"].Value = value.Name;

                    if (value.Description != null)
                        xmlSupplier.Attributes["description"].Value = value.Description;

                    XmlHelper.Save(doc, dataSourceManager.DataSupliersFilePath);

                    list[key] = value;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(String.Format("При изменении элемента с именем {1}  возникла ошибка : {0} ", e.Message, key));
                    throw;
                }
            }
        }
    
        private void Initialize()
        {
            string errMsg;

            // Открываем и разбираем файл настроек пакета
            XmlDocument xmlDoc = Validator.LoadValidated(dataSourceManager.DataSupliersFilePath, 
                "ServerConfiguration.xsd", "xmluml", out errMsg);
            if (xmlDoc == null)
                throw new Exception(String.Format("Ошибка загрузки XML файла с конфигурацией поставшиков данных: {0}{1}Error: {2}",
                    dataSourceManager.DataSupliersFilePath, Environment.NewLine, errMsg));

            XmlNodeList xmlSuppliers =  xmlDoc.SelectNodes("/DataSourcesConfiguration/Suppliers/Supplier");
            foreach (XmlNode xmlSupplier in xmlSuppliers)
            {
                DataSupplier ds = new DataSupplier(xmlSupplier, this);
                list.Add(ds.Name, ds);
            }
        }

        /// <summary>
        /// Возвращает родительский объект
        /// </summary>
        public DataSourceManager DataSourceManager
        {
            get { return dataSourceManager; }
            set { dataSourceManager = value; }
        }

        public override IServerSideObject Lock()
        {
            CheckOut(String.Empty);
            IsEndPoint = true;
            return base.Lock() as DataSupplierCollection;
        }

        public void EndEdit()
        {
            CheckIn("");
            Unlock();
        }

        public void CancelEdit()
        {
            UndoCheckOut();
            Unlock();
        }

        #region Работа с SourceSafe

        private string GetSourceSafeLocalSpec
        {
            get { return "DataSources.xml"; }
        } 

        private void CheckOut(string comments)
        {
            IVSSFacade vssFacade = dataSourceManager.Scheme.VSSFacade;
            if (vssFacade != null)
            {
                try
                {
                    string local = GetSourceSafeLocalSpec;
                    switch (vssFacade.IsCheckedOut(local))
                    {
                        case VSSFileStatus.VSSFILE_CHECKEDOUT:
                            throw new Exception(String.Format("В базе SourceSafe файл \"{0}\" заблокирован другим пользователем.", local));
                        case VSSFileStatus.VSSFILE_NOTCHECKEDOUT:
                            vssFacade.Checkout(local, comments); break;
                    }
                }
                finally
                {
                    vssFacade.Close();
                }
            }
        }

        private void CheckIn(string comments)
        {
            IVSSFacade vssFacade = dataSourceManager.Scheme.VSSFacade;
            if (vssFacade != null)
            {
                try
                {
                    string local = GetSourceSafeLocalSpec;
                    if (!vssFacade.Find(local))
                        vssFacade.Checkin(local, comments);
                    else if (vssFacade.IsCheckedOut(local) == VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
                        vssFacade.Checkin(local, comments);
                }
                finally
                {
                    vssFacade.Close();
                }
            }
        }

        private void UndoCheckOut()
        {
            IVSSFacade vssFacade = dataSourceManager.Scheme.VSSFacade;
            if (vssFacade != null)
            {
                try
                {
                    string local = GetSourceSafeLocalSpec;
                    if (vssFacade.IsCheckedOut(local) == VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
                        vssFacade.UndoCheckout(local);
                }
                finally
                {
                    vssFacade.Close();
                }
            }
        }

        public VSSFileStatus IsCheckedOut
        {
            get
            {
                try
                {
                    IVSSFacade vssFacade =  dataSourceManager.Scheme.VSSFacade;
                    if (vssFacade != null)
                    {
                        try
                        {
                            if (State == ServerSideObjectStates.New)
                                return VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
                            else
                                return vssFacade.IsCheckedOut(GetSourceSafeLocalSpec);
                        }
                        finally
                        {
                            vssFacade.Close();
                        }
                    }
                    else
                        return VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }
        
        #endregion Работа с SourceSafe

    }
}
