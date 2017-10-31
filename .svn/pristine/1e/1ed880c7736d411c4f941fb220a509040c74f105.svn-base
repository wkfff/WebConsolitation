using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.PlaningServiceProxy
{
    /// <summary>
    /// Класс-обертка для доступа к методам веб-сервиса через COM
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("40E73CAC-C602-4B7D-ABB3-8B4E7DBFD3DF")]
    public class PlaningServiceProxy
    {
        // сериализатор для объектов DataTable
        private static XmlSerializer dataTableSerializer;

        // объект-синхронизатор для потокобезопасного доступа к сериализатору
        private static object syncObj = new object();

        /// <summary>
        /// Cериализатор для объектов DataTable
        /// </summary>
        private static XmlSerializer DataTableSerializer
        {
            get
            {
                // синглетон с двойной проверкой
                if (dataTableSerializer == null)
                {
                    lock (syncObj)
                    {
                        if (dataTableSerializer == null)
                        {
                            dataTableSerializer = new XmlSerializer(typeof(DataTable));
                        }
                    }
                }
                return dataTableSerializer;
            }
        }

        // обертка для доступа к веб-сервису через SOAP 
        private PlaningServiceWrapper serviceWrapper = new PlaningServiceWrapper();

        /// <summary>
        /// Проверка доступности сервиса
        /// </summary>
        private void CheckService()
        {
            if (serviceWrapper == null)
            {
                throw new Exception("Не установлено соединение с сервером");
            }
        }

        /// <summary>
        /// Получить список всех объектов схемы (перечисленных через запятую)
        /// </summary>
        /// <returns>Строка со списко объектов</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        public string GetSchemeObjectsNames()
        {
            CheckService();
            return serviceWrapper.GetSchemeObjectsNames();
        }

        /// <summary>
        /// Получить данные объекта схемы в виде DataTable XML
        /// </summary>
        /// <param name="objectName">Имя объекта</param>
        /// <param name="filter">Ограничение на выборку</param>
        /// <returns>Строка с данными объекта</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        public string GetObjectDataTable(string objectName, string filter)
        {
            CheckService();
            return serviceWrapper.GetObjectData(objectName, filter);
        }

        /// <summary>
        /// Получить данные объекта схемы в виде ADODB.Recordset
        /// </summary>
        /// <param name="objectName">Имя объекта</param>
        /// <param name="filter">ограничение на выборку</param>
        /// <returns>ADODB.Recordset (маршалируется как IUnknown)</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        [return: MarshalAs(UnmanagedType.IUnknown)]
        public object GetObjectRecordSet(string objectName, string filter)
        {
            string data = GetObjectDataTable(objectName, filter);
            object rs = null;
            using (StringReader sr = new StringReader(data))
            {
                DataTable dt = (DataTable)DataTableSerializer.Deserialize(sr);
                rs = DataTableConverter.ConvertToRecordset(ref dt);
            }
            return rs;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        public string Connect(int authType, string login, string pwd)
        {
            CheckService();
            string err = string.Empty;
            serviceWrapper.Login = login;
            serviceWrapper.Password = pwd;
            serviceWrapper.AuthType = (AuthenticationType)authType;
            return serviceWrapper.Connect();
        }
    }
}
