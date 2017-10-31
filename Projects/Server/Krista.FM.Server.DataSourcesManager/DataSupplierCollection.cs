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
    /// ��������� ����������� ������
    /// </summary>
    class DataSupplierCollection : DictionaryBase<string, IDataSupplier>, IDataSupplierCollection
    {
        // ������������ ������
        private DataSourceManager dataSourceManager;
       
        /// <summary>
        /// ����������� �������
        /// </summary>
        public DataSupplierCollection(DataSourceManager dataSourceManager)
            : base(null, ServerSideObjectStates.Consistent)
        {
            this.dataSourceManager = dataSourceManager;
            Initialize();
        }

        /// <summary>
        /// �������� ���������� ������
        /// </summary>
        /// <returns> ���������� ���������� ������</returns>
        public IDataSupplier New()
        {
            return new DataSupplier(this);
        }
              
        /// <summary>
        /// ���������� ����������
        /// </summary>
        /// <param name="key">���</param>
        /// <param name="value">�������� ����������</param>
        [MethodImpl (MethodImplOptions.Synchronized)]
        public override void Add(string key, IDataSupplier value)
        {
            try
            {
                if (Krista.FM.Common.ClientAuthentication.UserID != this.LockedByUserID)
                    throw new ArgumentException("��� �������� ��������� ������ ���������� �������������");
                
                if (list.ContainsKey(key))
                    throw new ArgumentException(String.Format("������ � ������ {0} ��� ����������", key));

                // �������� ���������� ������������ ����������
                if (String.IsNullOrEmpty(value.Name))
                    throw new Exception("�������� name ������������ ��� ����������");

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
                throw new Exception(String.Format("������ ��� ���������� ������ ���������� {0} : ", e.Message));
            }
        }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        /// <param name="value">�������� ����������</param>
        public void Add(IDataSupplier value)
        {
            Add(value.Name, value);
        }

        /// <summary>
        /// �������� ���������� �� ���������
        /// </summary>
        /// <param name="key">��� ����������</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Remove(string key)
        {
            try
            {
                if (Krista.FM.Common.ClientAuthentication.UserID != this.LockedByUserID)
                    throw new ArgumentException("��� �������� ��������� ������ ���������� �������������");

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
                throw new Exception(String.Format("������ ��� �������� ���������� {0} : ", e.Message));
            }
        }

        /// <summary>
        /// ���������� ��� ��������� � ����������
        /// </summary>
        /// <param name="key">���</param>
        /// <returns></returns>
        public override IDataSupplier this[string key]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                try
                {
                    if (Krista.FM.Common.ClientAuthentication.UserID != this.LockedByUserID)
                        throw new ArgumentException("��� �������� ��������� ������ ���������� �������������");

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
                    Trace.WriteLine(String.Format("��� ��������� �������� � ������ {1}  �������� ������ : {0} ", e.Message, key));
                    throw;
                }
            }
        }
    
        private void Initialize()
        {
            string errMsg;

            // ��������� � ��������� ���� �������� ������
            XmlDocument xmlDoc = Validator.LoadValidated(dataSourceManager.DataSupliersFilePath, 
                "ServerConfiguration.xsd", "xmluml", out errMsg);
            if (xmlDoc == null)
                throw new Exception(String.Format("������ �������� XML ����� � ������������� ����������� ������: {0}{1}Error: {2}",
                    dataSourceManager.DataSupliersFilePath, Environment.NewLine, errMsg));

            XmlNodeList xmlSuppliers =  xmlDoc.SelectNodes("/DataSourcesConfiguration/Suppliers/Supplier");
            foreach (XmlNode xmlSupplier in xmlSuppliers)
            {
                DataSupplier ds = new DataSupplier(xmlSupplier, this);
                list.Add(ds.Name, ds);
            }
        }

        /// <summary>
        /// ���������� ������������ ������
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

        #region ������ � SourceSafe

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
                            throw new Exception(String.Format("� ���� SourceSafe ���� \"{0}\" ������������ ������ �������������.", local));
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
        
        #endregion ������ � SourceSafe

    }
}
