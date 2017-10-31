using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Data;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Server.Common;

namespace Krista.FM.Server.Providers.Planing
{
    /// <summary>
    /// "Провайдер" для получения многомерных данных схемы
    /// </summary>
    internal partial class PlaningProvider : DisposableObject
    {
        #region Приватные поля

        //Контекст сообщения об ошибке. Строка, которая поясняет при каких действиях
        //возникло ислючение. Похоже на стек вызовов, только астрактно-человеческой формы.
        //Используется при формировании сообщения об ошибках
        private string exceptionContext = "";

        // Схема к которой подключен "провайдер"
        private IScheme scheme;
        // имя сервера
        private string serverName;
        // имя базы данных 
        private string databaseName;
        // Счетчик использований DSO
        private int usageDSO = 0;
        // Счетчик использований ADOMD
        private int usageADOMD = 0;
        // Счетчик использований ADODB
        private int usageADODB = 0;
        // DSO сервер
        private DSO.ServerClass dsoServer;
        // многомерная база данных, получаемая через DSO
        private DSO.Database dsoDatabase;
        // AMO сервер
        private Microsoft.AnalysisServices.Server amoServer;
        // многомерная база данных, получаемая через AMO
        private Microsoft.AnalysisServices.Database amoDatabase;
        // многомерная база данных, получаемая через ADOMD
        private ADOMD.Catalog adomdCatalog;
        // подключение, используемое при выполнении MDX запросов
        private ADODB.Connection adodbConnection;
        // дата обновления метаданных
        private string metadataDate;
        // провайдер подключения (MSOLAP.2 или MSOLAP.3)
        private string OlapProvider;
        // объект для синхронизации обновления метаданных
        private object mdSyncObj = new object();
        // полное имя UDL-файла со строкой подключения
        private string connectionString;
        // Идентификатор провайдера для случая работы с несколькими базами
        private string providerId;
        // Признак реального (не из кэша) обновления метаданных
        private bool metadataUpdated = false;


        #endregion Приватные поля

        #region Базовые методы

        /// <summary>
        /// Конструктор объекта
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="connectionString"></param>
        /// <param name="providerId"></param>
        public PlaningProvider(IScheme scheme, string connectionString, string providerId)
        {
            this.scheme = scheme;
            this.providerId = providerId;
            this.connectionString = connectionString;

            OlapConnectionString ocs = new OlapConnectionString();
            ocs.ReadConnectionString(connectionString);
            serverName = ocs.DataSource;
            databaseName = ocs.InitialCatalog;

            //проверка подключения
            InitConnections(true, true, true);
            DestroyConnections(true, true, true);
            Trace.TraceInformation("Инициализация провайдера планирования {0} ({1}.{2}, {3}, {4})",
                providerId, serverName, databaseName, Authentication.UserName, SessionContext.SessionId);
        }

        /// <summary>
        /// метод реально выполняющий очистку.
        /// Его вызывают методы Finalize, Dispose и Close
        /// </summary>
        /// <param name="disposing">
        /// true - явное уничтожение/закрытие объекта; 
        /// false - неявное уничтожение при сборке мусора
        /// </param>
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine(String.Format("~{0}({1})", GetType().FullName, disposing));
            Trace.TraceInformation(string.Format("Уничтожение провайдера планирования {0} {1}",
                ProviderId, Authentication.UserName));
            // Синхронизируем потоки для запрета одновременного вызова Dispose/Close
            lock (this)
            {
                if (disposing)
                {
                    // Здесь еще можно обращаться к полям, ссылающимся 
                    // на другие объекты - это безопасно для кода, так как для
                    // этих объектов метод Finalize еще не вызван
                }
            }
        }

        /// <summary>
        /// получить строку подключения к базе
        /// </summary>
        /// <returns> строка подключения к базе </returns>
        private string GetConnectionString()
        {
            return "File Name=" + connectionString;//"File Name=" + scheme.BaseDirectory + "\\MAS.udl";
        }

        /// <summary>
        /// Подключение со счетчиками
        /// Метод через который все остальные должны вызвать инициализицию подключений
        /// </summary>
        /// <param name="needDSO"></param>
        /// <param name="needADOMD"></param>
        /// <param name="needADODB"></param>
        private void InitConnections(bool needDSO, bool needADOMD, bool needADODB)
        {
            _InitConnections
                (
                    (needDSO & (usageDSO == 0)),
                    (needADOMD & (usageADOMD == 0)),
                    (needADODB & (usageADODB == 0))
                );

            if (needDSO) usageDSO++;
            if (needADOMD) usageADOMD++;
            if (needADODB) usageADODB++;
        }

        /// <summary>
        /// Закрытие подключений с учетом счетчиков
        /// </summary>
        /// <param name="needDSO"></param>
        /// <param name="needADOMD"></param>
        /// <param name="needADODB"></param>
        private void DestroyConnections(bool needDSO, bool needADOMD, bool needADODB)
        {
            if (needDSO) usageDSO--;
            if (needADOMD) usageADOMD--;
            if (needADODB) usageADODB--;

            _DestroyConnections
                            (
                                (needDSO & (usageDSO == 0)),
                                (needADOMD & (usageADOMD == 0)),
                                (needADODB & (usageADODB == 0))
                            );

        }


        /// <summary>
        /// инициализация подключений (без счетчиков)
        /// </summary>
        private void _InitConnections(bool needDSO, bool needADOMD, bool needADODB)
        {
            try
            {
                if (needADODB)
                {
                    adodbConnection = new ADODB.Connection();
                    // Этот флаг выставит провайдер, избавим его от проверок
                    adodbConnection.IsolationLevel = ADODB.IsolationLevelEnum.adXactReadUncommitted;
                    // Режим подключения - только чтения (для Аналайзиса больше и не нужно)
                    adodbConnection.Mode = ADODB.ConnectModeEnum.adModeRead;
                    adodbConnection.CommandTimeout = 60 * 50;
                    adodbConnection.Open(GetConnectionString(), "", "", -1);
                    OlapProvider = adodbConnection.Provider.ToString();
                }
                if (needDSO)
                {
                    if (OlapProvider == "MSOLAP.2")
                    {
                        dsoServer = new DSO.ServerClass();
                        dsoServer.Connect(serverName);
                        if (dsoServer.MDStores.Find(databaseName))
                            dsoDatabase = (DSO.Database)dsoServer.MDStores.Item(databaseName);
                        else
                            throw new Exception("Не найдена база данных: " + databaseName);
                    }
                    else
                    {
                        amoServer = new Microsoft.AnalysisServices.Server();
                        amoServer.Connect(serverName);
                        amoDatabase = (Microsoft.AnalysisServices.Database)amoServer.Databases.Find(databaseName);
                        if (amoDatabase == null)
                            throw new Exception("Не найдена база данных: " + databaseName);
                    }
                }
                if (needADOMD)
                {
                    adomdCatalog = new ADOMD.CatalogClass();
                    adomdCatalog.let_ActiveConnection(GetConnectionString());
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Ошибка инициализации провайдера планирования: " + exp.Message);
            }
        }


        /// <summary>
        /// закрытие подключений (без счетчиков)
        /// </summary>
        private void _DestroyConnections(bool needDSO, bool needADOMD, bool needADODB)
        {
            if (needDSO)
            {
                if (OlapProvider == "MSOLAP.2")
                {
                    dsoDatabase = null;
                    if (dsoServer != null)
                        if (dsoServer.State == DSO.ServerStates.stateConnected)
                            dsoServer.CloseServer();
                    dsoServer = null;
                }
                else
                {
                    amoDatabase = null;
                    if (amoServer != null)
                        if (amoServer.Connected)
                            amoServer.Disconnect();
                    amoServer = null;
                }
            }
            if (needADOMD & (adomdCatalog != null))
            {
                adomdCatalog.ActiveConnection = null;
                adomdCatalog = null;
            }
            if (needADODB & (adodbConnection != null))
            {
                adodbConnection.Close();
                adodbConnection = null;
            }
        }

        /// <summary>
        /// инициализация XML документа
        /// </summary>
        /// <param name="functionName"> имя функции </param>
        private XmlDocument InitXMLDocument(string functionName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (functionName != string.Empty)
            {
                XmlNode xmlNode = xmlDocument.CreateElement("function_result");
                xmlDocument.AppendChild(xmlNode);
                XmlHelper.SetAttribute(xmlNode, "function_name", functionName);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Оформляет сообщение об ошибке в стандартный тег Exception
        /// </summary>
        /// <param name="exceptionMessage">Исходный текст ошибки</param>
        /// <returns>Строка - офомленное сообщение об ошике</returns>
        private string EmbraceException(string exceptionMessage)
        {
            AddToExceptionContext(exceptionMessage);
            return String.Format("<Exception><![CDATA[{0}]]></Exception>", exceptionContext);
        }

        /// <summary>
        /// Добавляет строку к контексту исключения.
        /// </summary>
        /// <param name="part">Часть контекста, которую нужно добавить</param>
        private void AddToExceptionContext(string part)
        {
            if (exceptionContext != "")
            {
                exceptionContext += "; ";
            }
            exceptionContext += part;
        }

        #endregion Базовые методы

        #region Методы обработки данных

        #region Методы получения метаданных

        /// <summary>
        /// получение свойства дсо куба
        /// </summary>
        /// <param name="cubeName"> имя куба </param>
        /// <param name="propertyName"> имя свойства </param>
        /// <returns> значение свойства </returns>
        private string GetCubeCustomProperty(string cubeName, string propertyName)
        {
            try
            {
                if (OlapProvider == "MSOLAP.2")
                {
                    DSO.Cube dsoCube = (DSO.Cube)dsoDatabase.Cubes.Item(cubeName);
                    return dsoCube.CustomProperties[propertyName].Value.ToString();
                }
                else
                {
                    Microsoft.AnalysisServices.Cube amoCube = (Microsoft.AnalysisServices.Cube)amoDatabase.Cubes.GetByName(cubeName);
                    Microsoft.AnalysisServices.MeasureGroup amoMeasureGroup = (Microsoft.AnalysisServices.MeasureGroup)amoCube.MeasureGroups.GetByName(cubeName);
                    return amoMeasureGroup.Annotations[propertyName].Value.InnerText;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// получение данных кубов
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="filterName">Имя конкретного куба, если передается пустая строка,
        /// обрабатываются все кубы</param>
        private void GetCubes(XmlNode xmlRootNode, string filterName)
        {
            ADODB.Recordset cubeRecordset = null;
            try
            {
                object[] attrs;
                if (filterName == "")
                    attrs = new object[3] { databaseName, null, null };
                else
                    attrs = new object[3] { databaseName, null, filterName };

                cubeRecordset = adodbConnection.OpenSchema(ADODB.SchemaEnum.adSchemaCubes, attrs, Missing.Value);
                while (!cubeRecordset.EOF)
                {
                    string cubeName = cubeRecordset.Fields["CUBE_NAME"].Value.ToString();
                    Trace.TraceInformation("{0} Обновление метаданных куба {1}", DateTime.Now, cubeName);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    string fullName = GetCubeCustomProperty(cubeName, "FullName");
                    string subClass = GetCubeCustomProperty(cubeName, "SubClass");
                    XmlNode cubeElement = XmlHelper.AddChildNode(xmlRootNode, "Cube",
                        new string[] { "name", cubeName },
                        new string[] { "full_name", fullName },
                        new string[] { "subClass", subClass },
                        new string[] { "providerId", providerId });
                    // получаем меры куба
                    XmlNode measureElement = XmlHelper.AddChildNode(cubeElement, "Measures");
                    if (GetMeasures(measureElement, cubeName))
                    {
                        // получаем измерения куба
                        XmlNode cubeDimensionElement = XmlHelper.AddChildNode(cubeElement, "Dimensions");
                        GetCubeDimensions(cubeDimensionElement, cubeName);
                    }
                    else
                    {
                        //Удаляем весь узел
                        cubeElement.ParentNode.RemoveChild(cubeElement);
                    }
                    sw.Stop();
                    Trace.Indent();
                    Trace.TraceInformation(" - Обновление метаданных куба {0} выполнено за {1} мс",
                        cubeName, sw.ElapsedMilliseconds);
                    Trace.Unindent();
                    cubeRecordset.MoveNext();
                }
            }
            finally
            {
                if (cubeRecordset != null)
                    cubeRecordset.Close();
                cubeRecordset = null;
            }
        }

        /// <summary>
        /// получение формата меры
        /// </summary>
        /// <param name="cubeName"> имя куба </param>
        /// <param name="measureName"> имя меры </param>
        /// <returns> формат меры </returns>
        private string GetMeasureFormat(string cubeName, string measureName)
        {
            string measureFormat = string.Empty;
            if (OlapProvider == "MSOLAP.2")
            {
                DSO.Cube dsoCube = (DSO.Cube)dsoDatabase.Cubes.Item(cubeName);
                if (dsoCube.Measures.Find(measureName))
                {
                    // ищем в хранимых мерах
                    DSO.Measure dsoMeasure = (DSO.Measure)dsoCube.Measures.Item(measureName);
                    try
                    {
                        measureFormat = dsoMeasure.FormatString.ToString();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    if (dsoCube.Commands.Find(measureName))
                    {
                        // ищем в вычислимых мерах
                        DSO.CubeCommand dsoCommand = (DSO.CubeCommand)dsoCube.Commands.Item(measureName);
                        int pos = dsoCommand.Statement.LastIndexOf("FORMAT_STRING", StringComparison.Ordinal);
                        if (pos != -1)
                        {
                            measureFormat = dsoCommand.Statement.ToString();
                            measureFormat = measureFormat.Substring(pos);
                            pos = measureFormat.IndexOf("'", StringComparison.Ordinal);
                            measureFormat = measureFormat.Substring(pos + 1);
                            pos = measureFormat.IndexOf("'", StringComparison.Ordinal);
                            measureFormat = measureFormat.Substring(0, pos);
                        }
                    }
                }
            }
            else
            {
                Microsoft.AnalysisServices.Cube amoCube = (Microsoft.AnalysisServices.Cube)amoDatabase.Cubes.GetByName(cubeName);
                Microsoft.AnalysisServices.MeasureGroup amoMeasureGroup = (Microsoft.AnalysisServices.MeasureGroup)amoCube.MeasureGroups.GetByName(cubeName);
                try
                {
                    Microsoft.AnalysisServices.Measure amoMeasure = (Microsoft.AnalysisServices.Measure)amoMeasureGroup.Measures.GetByName(measureName);
                    measureFormat = amoMeasure.FormatString.ToString();
                }
                catch
                {
                    // мера вычислимая 
                    foreach (Microsoft.AnalysisServices.Command amoCommand in amoCube.DefaultMdxScript.Commands)
                    {
                        if (amoCommand.Text.IndexOf(measureName, StringComparison.Ordinal) == -1)
                            continue;
                        string sign = string.Format("CREATE MEMBER CURRENTCUBE.Measures.[{0}]", measureName);
                        string[] sections = amoCommand.Text.Split(';');
                        foreach (string section in sections)
                        {
                            if (section.IndexOf(sign) == -1)
                                continue;
                            if (section.IndexOf("FORMAT_STRING", StringComparison.Ordinal) == -1)
                                continue;
                            string formatSection = section.Substring(section.IndexOf('"') + 1);
                            measureFormat = formatSection.Substring(0, formatSection.IndexOf('"'));
                        }
                    }
                }
            }
            return measureFormat;
        }

        /// <summary>
        /// получение данных мер куба
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="cubeName"> имя куба </param>
        private bool GetMeasures(XmlNode xmlRootNode, string cubeName)
        {
            ADODB.Recordset measureRecordset = null;
            try
            {
                object[] attrs = new object[4] { databaseName, null, cubeName, null };
                try
                {
                    measureRecordset = adodbConnection.OpenSchema(ADODB.SchemaEnum.adSchemaMeasures, attrs, Missing.Value);
                }
                catch
                {
                    AddToExceptionContext(String.Format("Получение мер куба '{0}'", cubeName));
                    return false;
                }


                while (!measureRecordset.EOF)
                {
                    string measureName = measureRecordset.Fields["MEASURE_NAME"].Value.ToString();
                    string measureType = measureRecordset.Fields["MEASURE_AGGREGATOR"].Value.ToString();
                    // по умолчанию формат меры - общий
                    string measureFormat = "Standart";
                    try
                    {
                        measureFormat = GetMeasureFormat(cubeName, measureName);
                    }
                    catch
                    {
                        AddToExceptionContext(String.Format("Получение формата меры '{0}' куба '{1}'", measureName, cubeName));
                        measureFormat = string.Empty;
                    }
                    XmlNode measureElement = XmlHelper.AddChildNode(xmlRootNode, "Measure",
                        new string[] { "name", measureName },
                        new string[] { "type", measureType },
                        new string[] { "format", measureFormat });
                    measureRecordset.MoveNext();
                }
            }
            finally
            {
                if (measureRecordset != null)
                    measureRecordset.Close();
                measureRecordset = null;

            }
            return true;
        }

        /// <summary>
        /// получение данных измерений куба 
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="cubeName"> имя куба </param>
        private void GetCubeDimensions(XmlNode xmlRootNode, string cubeName)
        {
            ADODB.Recordset dimensionRecordset = null;
            try
            {
                object[] attrs = new object[4] { databaseName, null, cubeName, null };
                try
                {
                    dimensionRecordset = adodbConnection.OpenSchema(ADODB.SchemaEnum.adSchemaDimensions, attrs, Missing.Value);
                }
                catch (Exception e)
                {
                    AddToExceptionContext(string.Format("Получение измерений куба '{0}'", cubeName));
                    throw new Exception(e.Message, e);
                }
                while (!dimensionRecordset.EOF)
                {
                    string dimensionName = dimensionRecordset.Fields["DIMENSION_NAME"].Value.ToString();
                    // исключаем меры
                    if (dimensionName == "Measures")
                    {
                        dimensionRecordset.MoveNext();
                        continue;
                    }
                    XmlNode dimensionElement = XmlHelper.AddChildNode(xmlRootNode, "Dimension",
                        new string[] { "name", dimensionName });
                    // получаем иерархии измерения
                    GetHierarchies(dimensionElement, cubeName, dimensionName);
                    dimensionRecordset.MoveNext();
                }
            }
            finally
            {
                if (dimensionRecordset != null)
                    dimensionRecordset.Close();
                dimensionRecordset = null;
            }
        }

        /// <summary>
        /// получение узла - общая иерархия
        /// </summary>
        /// <param name="Document"> общий документ с данными </param>
        /// <param name="dimensionName"> имя общего измерения </param>
        /// <param name="hierarchyName"> имя общей иерархии </param>
        /// <param name="isNew"> новый элемент, или уже был добавлен ранее </param>
        /// <returns> узел общей иерархии (если в документе не было - создается) </returns>
        private XmlNode GetSharedHierarchyElement(XmlDocument Document, string dimensionName, string hierarchyName, ref bool isNew)
        {
            isNew = false;
            XmlNode sharedDimensionsElement = Document.SelectSingleNode("function_result/SharedDimensions");
            XmlNode sharedDimensionElement = sharedDimensionsElement.SelectSingleNode("Dimension[@name='" + dimensionName + "']");
            if (sharedDimensionElement == null)
            {
                sharedDimensionElement = XmlHelper.AddChildNode(sharedDimensionsElement, "Dimension",
                    new string[] { "name", dimensionName },
                    new string[] { "providerId", providerId });
            }
            XmlNode sharedHierarchyElement = sharedDimensionElement.SelectSingleNode("Hierarchy[@name='" + hierarchyName + "']");
            if (sharedHierarchyElement == null)
            {
                isNew = true;
                sharedHierarchyElement = XmlHelper.AddChildNode(sharedDimensionElement, "Hierarchy",
                    new string[] { "name", hierarchyName },
                    new string[] { "providerId", providerId });
            }
            return sharedHierarchyElement;
        }

        /// <summary>
        /// получение данных иерархий измерения
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="cubeName"> имя куба </param>
        /// <param name="dimensionName"> имя измерения </param>
        private void GetHierarchies(XmlNode xmlRootNode, string cubeName, string dimensionName)
        {
            ADODB.Recordset hierarchyRecordset = null;
            try
            {
                object[] attrs = new object[5] { databaseName, null, cubeName, dimensionName, null };

                try
                {
                    hierarchyRecordset = adodbConnection.OpenSchema(ADODB.SchemaEnum.adSchemaHierarchies, attrs, Missing.Value);
                }
                catch (Exception e)
                {
                    AddToExceptionContext(string.Format("Получение иерархий семантики '{0}'", dimensionName));
                    throw new Exception(e.Message, e);
                }

                while (!hierarchyRecordset.EOF)
                {
                    string hierarchyName = hierarchyRecordset.Fields["HIERARCHY_NAME"].Value.ToString();
                    string isShared = hierarchyRecordset.Fields["DIMENSION_IS_SHARED"].Value.ToString();
                    string allMember = hierarchyRecordset.Fields["ALL_MEMBER"].Value.ToString();
                    XmlNode hierarchyElement = XmlHelper.AddChildNode(xmlRootNode, "Hierarchy",
                        new string[] { "name", hierarchyName },
                        new string[] { "IsShared", isShared });
                    // если иерархия - общая, данные ее заносим в ветку общих измерений, иначе в ветку куба 
                    if (isShared == "True")
                    {
                        bool isNew = false;
                        XmlNode sharedHierarchyElement = GetSharedHierarchyElement(xmlRootNode.OwnerDocument, dimensionName, hierarchyName, ref isNew);
                        if (isNew)
                        {
                            XmlHelper.SetAttribute(sharedHierarchyElement, "all_member", allMember);
                            AddHierarchyData(sharedHierarchyElement, cubeName, dimensionName, hierarchyName);
                        }
                        XmlHelper.SetAttribute(sharedHierarchyElement, "processing_date", GetLastProcessingDate(dimensionName, hierarchyName));
                    }
                    else
                    {
                        XmlHelper.SetAttribute(hierarchyElement, "all_member", allMember);
                        AddHierarchyData(hierarchyElement, cubeName, dimensionName, hierarchyName);
                    }
                    hierarchyRecordset.MoveNext();
                }
            }
            finally
            {
                if (hierarchyRecordset != null)
                    hierarchyRecordset.Close();
                hierarchyRecordset = null;
            }
        }

        /// <summary>
        /// Извлечь из полного имени измерения семантику
        /// </summary>
        /// <param name="dimensionFullName">Полное имя измерения "семантика"."иерархия"</param>
        /// <returns>Имя измерения (семантика)</returns>
        private string ExtractDimensionName(string dimensionFullName)
        {
            string result;
            if (dimensionFullName.Contains("."))
                result = dimensionFullName.Split('.')[0];
            else
                result = dimensionFullName;
            return result;
        }

        /// <summary>
        /// Извлечь из полного имени измерения имя иерархии
        /// </summary>
        /// <param name="dimensionFullName">Полное имя измерения "семантика"."иерархия"</param>
        /// <returns>Имя иерархии</returns>
        private string ExtractHierarchyName(string dimensionFullName)
        {
            string result;
            if (dimensionFullName.Contains("."))
                result = dimensionFullName.Split('.')[1];
            else
                result = "";
            return result;
        }

        /// <summary>
        /// получение имя сущности (fullName) иерархии
        /// </summary>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        /// <returns> имя сущности </returns>
        private string GetHierarchyFullName(string dimensionName, string hierarchyName)
        {
            try
            {
                if (OlapProvider == "MSOLAP.2")
                {
                    DSO.Dimension dsoDimension = GetDSODimension(dimensionName, hierarchyName);
                    return dsoDimension.CustomProperties["FullName"].Value.ToString();
                }
                else
                {
                    Microsoft.AnalysisServices.Dimension amoDimension = GetAMODimension(dimensionName);
                    return amoDimension.Annotations["FullName"].Value.InnerText;
                }
            }
            catch
            {
                return "null";
            }
        }

        /// <summary>
        /// получить DSO измерение 
        /// </summary>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        /// <returns> DSO измерение </returns>
        private DSO.Dimension GetDSODimension(string dimensionName, string hierarchyName)
        {
            string dsoDimendionName = string.Empty;
            if (hierarchyName == string.Empty)
            {
                dsoDimendionName = dimensionName;
            }
            else
            {
                dsoDimendionName = dimensionName + "." + hierarchyName;
            }
            DSO.Dimension dsoDimension = null;
            if (!(dsoDatabase.Dimensions.Find(dsoDimendionName)))
            {
                return null;
            }
            dsoDimension = (DSO.Dimension)dsoDatabase.Dimensions.Item(dsoDimendionName);
            if (!(dsoDimension.IsShared))
            {
                return null;
            }
            return dsoDimension;
        }

        /// <summary>
        /// получить AMO измерение
        /// </summary>
        /// <param name="dimensionName"> имя измерения </param>
        /// <returns> AMO измерение </returns>
        private Microsoft.AnalysisServices.Dimension GetAMODimension(string dimensionName)
        {
            return amoDatabase.Dimensions.GetByName(dimensionName);
        }

        /// <summary>
        /// получить AMO иерархию
        /// </summary>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        /// <returns> AMO иерархия </returns>
        private Microsoft.AnalysisServices.Hierarchy GetAMOHierarchy(string dimensionName, string hierarchyName)
        {
            Microsoft.AnalysisServices.Dimension amoDimension = GetAMODimension(dimensionName);
            if (amoDimension == null)
                return null;
            return amoDimension.Hierarchies.GetByName(hierarchyName);
        }

        /// <summary>
        /// получение данных иерархии
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="cubeName"> имя куба </param>
        /// <param name="dimensionName">имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        private void AddHierarchyData(XmlNode xmlRootNode, string cubeName, string dimensionName, string hierarchyName)
        {
            string fullName = GetHierarchyFullName(dimensionName, hierarchyName);
            XmlHelper.SetAttribute(xmlRootNode, "full_name", fullName);
            XmlHelper.SetAttribute(xmlRootNode, "processing_date", GetLastProcessingDate(dimensionName, hierarchyName));
            XmlNode levelsElement = XmlHelper.AddChildNode(xmlRootNode, "Levels");
            GetLevels(levelsElement, cubeName, dimensionName, hierarchyName);
            XmlNode propertiesElement = XmlHelper.AddChildNode(xmlRootNode, "Properties");
            IDataAttributeCollection attributes = null;
            if (scheme.Classifiers != null)
                if (scheme.Classifiers.ContainsKey(fullName))
                    attributes = scheme.Classifiers[fullName].Attributes;
            GetMemberProperties(propertiesElement, dimensionName, hierarchyName, attributes);
        }

        /// <summary>
        /// получение уровней иерархии
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="cubeName"> имя куба </param>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        private void GetLevels(XmlNode xmlRootNode, string cubeName, string dimensionName, string hierarchyName)
        {
            ADODB.Recordset levelRecordset = null;
            try
            {
                if (OlapProvider == "MSOLAP.2")
                {
                    if (hierarchyName == string.Empty)
                        hierarchyName = dimensionName;
                    else
                        hierarchyName = dimensionName + "." + hierarchyName;
                }
                else
                {
                    if (hierarchyName == string.Empty)
                        hierarchyName = dimensionName;
                    hierarchyName = "[" + dimensionName + "].[" + hierarchyName + "]";
                }
                object[] attrs = new object[6] { databaseName, null, cubeName, dimensionName, hierarchyName, null };


                try
                {
                    levelRecordset = adodbConnection.OpenSchema(ADODB.SchemaEnum.adSchemaLevels, attrs, Missing.Value);
                }
                catch (Exception e)
                {
                    AddToExceptionContext(string.Format("Получение уровней измерения '{0}.{1}'", dimensionName, hierarchyName));
                    throw new Exception(e.Message, e);
                }
                while (!levelRecordset.EOF)
                {
                    string levelName = levelRecordset.Fields["LEVEL_NAME"].Value.ToString();
                    XmlHelper.AddChildNode(xmlRootNode, "Level", new string[2] { "name", levelName });
                    levelRecordset.MoveNext();
                }
            }
            finally
            {
                if (levelRecordset != null)
                    levelRecordset.Close();
                levelRecordset = null;
            }
        }

        /// <summary>
        /// добавление элемента - свойство иерархии
        /// </summary>
        /// <param name="propertiesList"> список уже добавленных свойств </param>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="propertyName"> имя свойства </param>
        /// <param name="attributes"> набор атрибутов сущности </param>
        private void AddMemberProperty(ref List<string> propertiesList, XmlNode xmlRootNode, string propertyName, IDataAttributeCollection attributes)
        {
            if (propertiesList.IndexOf(propertyName) != -1)
                return;
            propertiesList.Add(propertyName);
            string mask = "null";
            if (attributes != null)
            {
                foreach (IDataAttribute attribute in attributes.Values)
                    if (attribute.Caption == propertyName)
                    {
                        mask = attribute.Mask;
                        break;
                    }
            }
            XmlNode propertyElement = XmlHelper.AddChildNode(xmlRootNode, "Property",
                new string[] { "name", propertyName }, new string[] { "mask", mask });
        }

        /// <summary>
        /// получение свойств иерархии
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        /// <param name="attributes"> набор атрибутов сущности </param>
        private void GetMemberProperties(XmlNode xmlRootNode, string dimensionName, string hierarchyName, IDataAttributeCollection attributes)
        {
            List<string> propertiesList = new List<string>();
            if (OlapProvider == "MSOLAP.2")
            {
                // получаем свойства всех уровней иерархии
                DSO.Dimension dsoDimension = GetDSODimension(dimensionName, hierarchyName);
                if (dsoDimension == null)
                    return;
                foreach (DSO.Level dsoLevel in dsoDimension.Levels)
                    foreach (DSO.MemberProperty dsoMemberProperty in dsoLevel.MemberProperties)
                        AddMemberProperty(ref propertiesList, xmlRootNode, dsoMemberProperty.Name, attributes);
            }
            else
            {
                Microsoft.AnalysisServices.Dimension amoDimension = GetAMODimension(dimensionName);
                if (amoDimension == null)
                    return;
                foreach (DimensionAttribute attribute in amoDimension.Attributes)
                {
                    if (attribute.Type.ToString() != "Person")
                        continue;
                    AddMemberProperty(ref propertiesList, xmlRootNode, attribute.Name.ToString(), attributes);
                }
            }
        }

        #endregion Методы получения метаданных

        #region Методы получения элементов измерений

        /// <summary>
        /// получение свойства элемента измерения
        /// </summary>
        /// <param name="memberRecordset"> рекордсет со свойствами мембера </param>
        /// <param name="propertyName"> имя свойства </param>
        /// <returns> значение свойства </returns>
        private string GetMemberProperty(ADODB.Recordset memberRecordset, string propertyName, ADOMD.Member adoMember)
        {
            string value = "null";
            try
            {
                foreach (ADODB.Field field in memberRecordset.Fields)
                {
                    if (field.Name == propertyName)
                        value = field.Value.ToString();
                }
            }
            catch
            {
            }
            if (value == "null")
                try
                {
                    //value = adoMember.Properties[propertyName].Value.ToString();
                }
                catch
                {
                }
            return value;
        }

        /// <summary>
        ///  получение данных дочерних элементов элемента
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="adoMember"> элемент </param>
        /// <param name="levelList"> список нужных уровней </param>
        /// <param name="id"> порядковый номер элемента </param>
        /// <param name="memberPropertiesList"> список свойств измерения, которые нужно получить </param>
        private void GetMemberMembers(XmlNode xmlRootNode, ADOMD.Member adoMember, List<string> levelList,
            List<string> memberPropertiesList, string cubeName, string dimensionName)
        {
            string[] memberLevelName = adoMember.LevelName.Split('[');
            string levelName = memberLevelName[memberLevelName.Length - 1];
            levelName = levelName.Substring(0, levelName.Length - 1);
            // если уровень не входит в список заданных, то пропускаем элемент
            // если входит, то заносим элемент в документ
            XmlNode memberElement;
            if (levelList.IndexOf(levelName) != -1)
            {
                ADODB.Recordset memberRecordset = null;
                try
                {
                    object[] attrs = new object[11] { databaseName, null, cubeName, dimensionName, null, null, null, null, adoMember.UniqueName, null, null };
                    try
                    {
                        memberRecordset = adodbConnection.OpenSchema(ADODB.SchemaEnum.adSchemaMembers, attrs, Missing.Value);
                    }
                    catch (Exception e)
                    {
                        AddToExceptionContext(string.Format("Выборка элементов измерения '{0}'", dimensionName));
                        throw new Exception(e.Message, e);
                    }
                    // если Member - DataMember, то берем заголовок
                    string memberName = string.Empty;
                    if (GetMemberProperty(memberRecordset, "IS_DATAMEMBER", adoMember) == "False")
                        memberName = adoMember.Name;
                    else
                        memberName = adoMember.Caption;
                    // убираем перенос строки 
                    memberName = memberName.Replace("\n", " ");
                    memberName = memberName.Replace("\r", " ");
                    memberElement = XmlHelper.AddChildNode(xmlRootNode, "Member",
                        new string[] { "name", memberName },
                        new string[] { "unique_name", adoMember.UniqueName },
                        new string[] { "pk_id", GetMemberProperty(memberRecordset, "PKID", adoMember) },
                        new string[] { "local_id", GetMemberProperty(memberRecordset, "MEMBER_ORDINAL", adoMember) });
                    foreach (string memberPropertyName in memberPropertiesList)
                    {
                        if (memberPropertyName == string.Empty)
                            continue;
                        string propertyName = memberPropertyName.Replace("Char20", " ");
                        propertyName = propertyName.Replace("Char40", "(");
                        propertyName = propertyName.Replace("Char41", ")");
                        string propertyValue = GetMemberProperty(memberRecordset, propertyName, adoMember);
                        XmlHelper.SetAttribute(memberElement, memberPropertyName, propertyValue);
                    }
                }
                finally
                {
                    if (memberRecordset != null)
                        memberRecordset.Close();
                    memberRecordset = null;
                }
            }
            else
            {
                memberElement = xmlRootNode;
            }
            // проходим по всем дочерним элементам элемента и выполняем функцию над ними
            if (adoMember.ChildCount != 0)
                foreach (ADOMD.Member adoChildMember in adoMember.Children)
                    GetMemberMembers(memberElement, adoChildMember, levelList, memberPropertiesList, cubeName, dimensionName);
        }

        /// <summary>
        /// получение данных элементов уровня 
        /// </summary>
        /// <param name="xmlRootNode"> корневой узел </param>
        /// <param name="adoLevel"> уровень </param>
        /// <param name="levelList"> список нужных уровней </param>
        /// <param name="memberPropertiesList"> список свойств измерения, которые нужно получить </param>
        private void GetLevelMembers(XmlNode xmlRootNode, ADOMD.Level adoLevel, List<string> levelList,
            List<string> memberPropertiesList, string cubeName, string dimensionName)
        {
            if (adoLevel.Members.Count == 0)
                return;
            // получаем данные по всем дочерним элементам элементов уровня
            foreach (ADOMD.Member adoMember in adoLevel.Members)
            {
                GetMemberMembers(xmlRootNode, adoMember, levelList, memberPropertiesList, cubeName, dimensionName);
            }
        }

        /// <summary>
        /// проверка корректности задания имени куба
        /// </summary>
        /// <param name="cubeName"> имя куба </param>
        /// <param name="adoCube"> получаемый куб </param>
        /// <returns> true - корректно, false - некорректно </returns>
        private bool CheckCubeName(string cubeName, ref ADOMD.CubeDef adoCube)
        {
            try
            {
                adoCube = adomdCatalog.CubeDefs[cubeName];
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// проверка корректности задания имени измерения куба 
        /// </summary>
        /// <param name="adoCube"> куб </param>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="adoDimension"> получаемое измерение </param>
        /// <returns> true - корректно, false - некорректно </returns>
        private bool CheckCubeDimensionName(ADOMD.CubeDef adoCube, string dimensionName,
                                            ref ADOMD.Dimension adoDimension)
        {
            try
            {
                adoDimension = adoCube.Dimensions[dimensionName];
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// проверка корректности задания имени общего измерения  
        /// </summary>
        /// <param name="dimensionName"> имя общего измерения </param>
        /// <returns> true - корректно, false - некорректно </returns>
        private bool CheckSharedDimensionName(string dimensionName)
        {
            if (OlapProvider == "MSOLAP.2")
            {
                foreach (DSO.Dimension dsoDimension in dsoDatabase.Dimensions)
                {
                    if (!dsoDimension.IsShared)
                        continue;
                    string[] sharedDimensionNames = dsoDimension.Name.Split('.');
                    string sharedDimensionName = sharedDimensionNames[0];
                    if (sharedDimensionName == dimensionName)
                        return true;
                }
                return false;
            }
            else
                return (amoDatabase.Dimensions.GetByName(dimensionName) != null);
        }

        /// <summary>
        /// получение куба, который содержит измерение с заданным именем
        /// </summary>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="adoDimension"> получаемое измерение куба </param>
        /// <returns> true - корректно, false - некорректно </returns>
        private bool GetFirstCubeWithDimension(string dimensionName, ref ADOMD.Dimension adoDimension, string hierarchyName, ref string cubeName)
        {
            foreach (ADOMD.CubeDef forADOCube in adomdCatalog.CubeDefs)
            {
                try
                {
                    adoDimension = forADOCube.Dimensions[dimensionName];
                    if (OlapProvider == "MSOLAP.3")
                        if (hierarchyName == "")
                            hierarchyName = dimensionName;
                    ADOMD.Hierarchy adoHierarchy = adoDimension.Hierarchies[hierarchyName];
                    cubeName = forADOCube.Name;
                    return true;
                }
                catch
                {
                }
            }
            // просмотрели все кубы, измерение не присутствует ни в одном из них
            return false;
        }

        /// <summary>
        /// проверка корректности задания имени иерархии измерения
        /// </summary>
        /// <param name="adoDimension"> измерение </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        /// <param name="adoHierarchy"> получаемая иерархия </param>
        /// <returns> true - корректно, false - некорректно </returns>
        private bool CheckHierarchyName(ADOMD.Dimension adoDimension, string hierarchyName, ref ADOMD.Hierarchy adoHierarchy)
        {
            try
            {
                if (hierarchyName == string.Empty)
                    adoHierarchy = adoDimension.Hierarchies[0];
                else
                    adoHierarchy = adoDimension.Hierarchies[hierarchyName];
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// проверка корректности задания имен уровней иерархии 
        /// </summary>
        /// <param name="adoHierarchy"> иерархия </param>
        /// <param name="levelListNotSorted"> неотсортированный список уровней </param>
        /// <param name="levelList"> отсортированный список уровней </param>
        /// <param name="adoLevel"> получаемый уровень</param>
        /// <param name="incorrectLevel"> некорректное имя уровня</param>
        /// <returns> true - корректно, false - некорректно </returns>
        private bool CheckLevelName(ADOMD.Hierarchy adoHierarchy, List<string> levelListNotSorted,
                                    ref List<string> levelList, ref ADOMD.Level adoLevel, ref string incorrectLevel)
        {
            // если уровень не задан, то берем первый уровень измерения
            // и добавляем в список нужных уровней все уровни
            if (levelListNotSorted[0] == string.Empty)
            {
                foreach (ADOMD.Level forADOLevel in adoHierarchy.Levels)
                {
                    levelList.Add(forADOLevel.Name);
                }
                adoLevel = adoHierarchy.Levels[0];
                return true;
            }
            else
            {
                // проверяем корректность каждого из переданных в списке уровней 
                try
                {
                    foreach (string listLevelName in levelListNotSorted)
                    {
                        incorrectLevel = listLevelName;
                        adoLevel = adoHierarchy.Levels[listLevelName];
                    }
                }
                catch
                {
                    return false;
                }
                // идем по всем уровням и заносим в список уровень, если он входит в требуемые
                foreach (ADOMD.Level forADOLevel in adoHierarchy.Levels)
                {
                    // проверяем, входит ли уровень в заданные
                    if (levelListNotSorted.IndexOf(forADOLevel.Name) != -1)
                        levelList.Add(forADOLevel.Name);
                }
                // берем самый верхний из переданных уровней
                adoLevel = adoHierarchy.Levels[levelList[0]];
                return true;
            }
        }

        /// <summary>
        /// получение времени последней обработки (процессинга) измерения
        /// </summary>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        /// <returns></returns>
        private string GetLastProcessingDate(string dimensionName, string hierarchyName)
        {
            try
            {
                string lastProcessingDate = string.Empty;
                if (OlapProvider == "MSOLAP.2")
                {
                    DSO.Dimension dsoDimension = GetDSODimension(dimensionName, hierarchyName);
                    lastProcessingDate = dsoDimension.LastProcessed.ToString();
                }
                else
                {
                    Microsoft.AnalysisServices.Dimension amoDimension = GetAMODimension(dimensionName);
                    lastProcessingDate = amoDimension.LastProcessed.ToString();
                }
                // формат даты должен быть xx.xx.xx xx:xx:xx
                lastProcessingDate = lastProcessingDate.Replace("/", ".");
                if (lastProcessingDate[lastProcessingDate.Length - 1] == 'M')
                {
                    bool IsPm = (lastProcessingDate.IndexOf("PM", StringComparison.Ordinal) != 0);
                    lastProcessingDate = lastProcessingDate.Remove(lastProcessingDate.Length - 3);
                    if (IsPm)
                    {
                        string tempString = lastProcessingDate.Substring(0, lastProcessingDate.IndexOf(' ') + 1);
                        string tempString1 = lastProcessingDate.Substring(lastProcessingDate.IndexOf(' ') + 1);
                        int hour = Convert.ToInt32(tempString1[0].ToString()) + 12;
                        tempString1 = tempString1.Remove(0, 1);
                        lastProcessingDate = tempString + hour.ToString() + tempString1;
                    }
                }
                return lastProcessingDate;
            }
            catch
            {
                return "null";
            }
        }

        #endregion Методы получения элементов измерений

        #region Методы выполнения запроса

        /// <summary>
        /// выделить алиас поля
        /// </summary>
        /// <param name="rowSetName"></param>
        /// <returns> алиас поля </returns>
        private string GetFieldAlias(string rowSetName)
        {
            // возвращаем последнюю часть наименования, без кавычек
            int lastDotInd = rowSetName.LastIndexOf('.');
            return rowSetName.Substring(lastDotInd + 2, rowSetName.Length - lastDotInd - 3);
        }

        /// <summary>
        /// записать атрибут
        /// </summary>
        /// <param name="writer"> райтер </param>
        /// <param name="attributeName"> имя атрибута </param>
        /// <param name="attributeValue"> значение атрибута </param>
        private void SetXMLAttribute(XmlWriter writer, string attributeName, object attributeValue)
        {
            string attrValStr = attributeValue.ToString();
            if (attrValStr.Length == 0)
                return;
            writer.WriteStartAttribute(attributeName);
            writer.WriteString(attrValStr);
            writer.WriteEndAttribute();
        }

        private bool MeasuresPresent(string rowSetName)
        {
            return rowSetName.IndexOf("Measures", StringComparison.Ordinal) != -1;
        }

        /// <summary>
        /// является ли поле - данными 
        /// </summary>
        /// <param name="fieldAlias"></param>
        /// <returns></returns>
        private bool TotalPresent(string fieldAlias)
        {
            return (fieldAlias.IndexOf("T", StringComparison.Ordinal) != -1) ||
                   (fieldAlias.IndexOf("S", StringComparison.Ordinal) != -1);
        }

        #endregion Методы выполнения запроса

        #endregion Методы обработки данных

        #region Реализация интерфейса IPlaningProvider

        /// <summary>
        /// получение даты обновления метаданных
        /// </summary>
        /// <returns> дата обновления метаданных </returns>
        public string GetMetadataDate()
        {
            return metadataDate;
        }

        /// <summary>
        /// Получение метаданных базы (по умолчанию берутся из кэша)
        /// </summary>
        /// <returns> xml текст результирующего документа, содержащего метаданные  </returns>
        public string GetMetaData()
        {
            Trace.TraceInformation("{0} Получение метаданных из кэша. Пользователь {1}, сессия {2}, провайдер {3}",
                    DateTime.Now, Authentication.UserName, SessionContext.SessionId, providerId);

            string metadata = LoadMetadata();

            if (metadata != string.Empty)
            {
                Trace.Indent();
                Trace.TraceInformation("{0} Метаданные из кэша получены.", DateTime.Now);
                Trace.Unindent();
                metadataUpdated = false;
                return metadata;
            }
            Trace.Indent();
            Trace.TraceInformation("{0} Не удалось получить метаданные из кэша. Запрашивается обновление.", DateTime.Now);
            Trace.Unindent();

            lock (mdSyncObj)
            {// полное обновление метаданных. имеет смысл выставить блокировку, чтобы не мешали.
                Trace.TraceInformation("{0} Обновление метаданных. Пользователь {1}, сессия {2}, провайдер {3}",
                DateTime.Now, Authentication.UserName, SessionContext.SessionId, providerId);
                XmlDocument metadataDocument = InitXMLDocument("GetMetaData");
                InitConnections(true, false, true);
                try
                {
                    UpdateMetadataDate(metadataDocument);
                    XmlHelper.AddChildNode(metadataDocument.DocumentElement, "SharedDimensions");
                    XmlNode cubeElement = XmlHelper.AddChildNode(metadataDocument.DocumentElement, "Cubes");
                    GetCubes(cubeElement, "");
                    SaveMetadata(metadataDocument);
                    DeleteOldDimensions(metadataDocument);
                    Trace.Indent();
                    Trace.TraceInformation("{0} Метаданные обновлены. Пользователь {1}, сессия {2}",
                        DateTime.Now, Authentication.UserName, SessionContext.SessionId);
                    Trace.Unindent();
                    metadataUpdated = true;
                    return metadataDocument.InnerXml;
                }
                catch (Exception exp)
                {
                    Trace.TraceWarning("{0} Ошибка обновления метаданных. Пользователь {1}, сессия {2}",
                        DateTime.Now, Authentication.UserName, SessionContext.SessionId);
                    return EmbraceException(exp.Message);
                }
                finally
                {
                    XmlHelper.ClearDomDocument(ref metadataDocument);
                    DestroyConnections(true, false, true);
                    GC.Collect();
                    exceptionContext = "";
                }
            }
        }


        /// <summary>
        /// Обновляет дату (метку) метаданных по текущему времени
        /// </summary>
        /// <param name="metadataDocument">Загруженный DOM с метаданными</param>
        private void UpdateMetadataDate(XmlDocument metadataDocument)
        {
            metadataDate = DateTime.Now.ToString();
            XmlNode xmlNode = metadataDocument.SelectSingleNode("function_result");
            XmlHelper.SetAttribute(xmlNode, "date", metadataDate);
        }

        /// <summary>
        /// Получение элементов измерения
        /// </summary>
        /// <param name="cubeName"> имя куба (может быть незаполнено - измерение общее)</param>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии (может быть незаполнено - иерархия по умолчанию)</param>
        /// <param name="levelNames"> список имен уровней (может быть незаполнен - все уровни измерения)</param>
        /// <param name="memberPropertiesNames"> список свойств измерения, которые нужно получить </param>
        /// <returns> xml текст результирующего документа, содержащего список элементов (мемберов) </returns>
        public string GetMembers(string cubeName, string dimensionName, string hierarchyName,
                                 string levelNames, string memberPropertiesNames)
        {
            exceptionContext = string.Format("Получение списка элементов измерения '{0}.{1}'", dimensionName, hierarchyName);
            Trace.TraceInformation("{0} Пользователь {1}. Сессия {2}. Получение измерения '{3}.{4}, , провайдер {5}'", DateTime.Now,
                Authentication.UserName, SessionContext.SessionId, dimensionName, hierarchyName, providerId);

            string dimensionData = LoadDimension(dimensionName, hierarchyName);
            if (dimensionData != string.Empty)
            {
                Trace.Indent();
                Trace.TraceInformation(@"{0} Пользователь {1}. Сессия {2}. Измерение '{3}.{4}' получено из кэша",
                    DateTime.Now, Authentication.UserName, SessionContext.SessionId, dimensionName, hierarchyName);
                Trace.Unindent();
                return dimensionData;
            }

            XmlDocument resultXmlDocument = InitXMLDocument("GetMemberList");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitConnections(true, true, true);
            try
            {
                ADOMD.Dimension adoDimension = null;
                if (cubeName == string.Empty)
                {
                    // если куб не задан, то проверяем в общих измерениях
                    if (!(CheckSharedDimensionName(dimensionName)))
                        throw new Exception("Общее измерение '" + dimensionName + "' не найдено.");
                    // находим первый куб с заданным измерением
                    if (!(GetFirstCubeWithDimension(dimensionName, ref adoDimension, hierarchyName, ref cubeName)))
                        throw new Exception(String.Format("Общее измерение '{0}.{1}' не присутствует ни в одном из кубов.",
                            dimensionName, hierarchyName));
                }
                else
                {
                    ADOMD.CubeDef adoCube = null;
                    if (!(CheckCubeName(cubeName, ref adoCube)))
                        throw new Exception("Куб '" + cubeName + "' не найден.");
                    if (!(CheckCubeDimensionName(adoCube, dimensionName, ref adoDimension)))
                        throw new Exception("Измерение '" + dimensionName + "' не найдено.");
                }
                ADOMD.Hierarchy adoHierarchy = null;
                if (!(CheckHierarchyName(adoDimension, hierarchyName, ref adoHierarchy)))
                    throw new Exception("Иерархия '" + hierarchyName + "' не найдена.");
                // получаем список нужных уровней
                List<string> levelList = new List<string>();
                List<string> levelListNotSorted = new List<string>();
                levelListNotSorted.AddRange(levelNames.Split(new string[] { "$$$" }, StringSplitOptions.None));

                // проверяем на корректное задание имен уровней
                // также получаем самый верхний уровень из заданных 
                // (начиная с него будем делать обход дерева элементов)
                ADOMD.Level adoLevel = null;
                string incorrectLevel = string.Empty;
                if (!(CheckLevelName(adoHierarchy, levelListNotSorted, ref levelList,
                                     ref adoLevel, ref incorrectLevel)))
                    throw new Exception("Уровень '" + incorrectLevel + "' не найден.");
                string lastProcessingDate = GetLastProcessingDate(dimensionName, hierarchyName);
                XmlHelper.AddChildNode(resultXmlDocument.DocumentElement, "Dimension",
                    new string[] { "name", dimensionName },
                    new string[] { "processing_date", lastProcessingDate });
                XmlHelper.AddChildNode(resultXmlDocument.DocumentElement, "Hierarchy", new string[] { "name", hierarchyName });
                XmlNode levelsElement = XmlHelper.AddChildNode(resultXmlDocument.DocumentElement, "Levels");
                foreach (string levelName in levelList)
                {
                    XmlNode levelElement = XmlHelper.AddChildNode(levelsElement, "Level", new string[] { "name", levelName });
                }
                List<string> memberPropertiesList = new List<string>();
                memberPropertiesNames = memberPropertiesNames.Replace(" ", "Char20");
                memberPropertiesNames = memberPropertiesNames.Replace("(", "Char40");
                memberPropertiesNames = memberPropertiesNames.Replace(")", "Char41");
                memberPropertiesList.AddRange(memberPropertiesNames.Split(','));
                XmlNode membersElement = XmlHelper.AddChildNode(resultXmlDocument.DocumentElement, "Members");
                // идем по дереву элементов и выводим элементы нужных уровней в документ
                GetLevelMembers(membersElement, adoLevel, levelList, memberPropertiesList, cubeName, dimensionName);
                SaveDimension(resultXmlDocument);
                sw.Stop();
                Trace.Indent();
                Trace.TraceInformation("{0} Пользователь {1}. Сессия {2}. Измерение '{3}.{4}' получено за {5} мс",
                    DateTime.Now, Authentication.UserName, SessionContext.SessionId, dimensionName, hierarchyName, sw.ElapsedMilliseconds);
                Trace.Unindent();
                return resultXmlDocument.InnerXml;
            }
            catch (Exception exp)
            {
                return EmbraceException(exp.Message);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref resultXmlDocument);
                DestroyConnections(true, true, true);
                if (sw.IsRunning)
                    sw.Stop();
                GC.Collect();
                exceptionContext = "";
            }


        }

        // вспомогательная структура для ускоренного доступа к данным рекордсета
        internal sealed class AdoFieldInfo
        {
            // Указатель на поле
            public ADODB.Field Field;
            // Название которое должно пойти в результирующий XML
            public string FieldAlias;
            public AdoFieldInfo(ADODB.Field field, string fieldAlias)
            {
                Field = field;
                FieldAlias = fieldAlias;
            }
        }

        /// <summary>
        /// Выполнение MDX-запроса через Recordset
        /// Разрешая асинхронное выполнение повышаем риск нехватки памяти.
        /// </summary>
        /// <param name="queryText"> текст запроса </param>
        /// <returns> результат запроса </returns>
        public string GetRecordsetData(string queryText)
        {
            exceptionContext = "Получение ADODB.Recordst";
            // создаем MemoryStream с начальным размером в 5 килобайт
            MemoryStream stream = new MemoryStream(1024 * 5);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.GetEncoding(1251);
            settings.CheckCharacters = false;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(stream, settings);
            ADODB.Recordset mdxRecordset = new ADODB.Recordset();
            // задаем размер кол-во записей на странице, 100 в самый раз
            mdxRecordset.PageSize = 100;
            InitConnections(false, false, true);
            try
            {
                Trace.TraceInformation("{0} MDX-запрос. Пользователь {1}, сессия {2}", DateTime.Now,
                    Authentication.UserName, SessionContext.SessionId);
                DateTime startTime = DateTime.Now;
                queryText = queryText.Replace("\n", "\r\n");
                mdxRecordset.Open(queryText, adodbConnection,
                    ADODB.CursorTypeEnum.adOpenStatic,      // статический
                    ADODB.LockTypeEnum.adLockReadOnly,      // только для чтения
                    (int)ADODB.CommandTypeEnum.adCmdText);  // текстовый запрос
                TimeSpan time = DateTime.Now - startTime;
                Trace.Indent();
                Trace.TraceInformation("- Пользователь {0}, сессия {1}. запрос успешно выполнен за {2}",
                    Authentication.UserName, SessionContext.SessionId, time.ToString());
                Trace.Unindent();
                // пишем заголовок
                writer.WriteStartElement("function_result");
                SetXMLAttribute(writer, "function_name", "ExecMDXQuery");
                writer.WriteStartElement("data");
                // если в рекордсете есть данные - формируем XML
                if (!(mdxRecordset.EOF && mdxRecordset.BOF))
                {
                    mdxRecordset.MoveFirst();
                    bool TotalPresence = false;
                    // создаем список описателей полей для оптимизированного доступа
                    List<AdoFieldInfo> fieldsInfo = new List<AdoFieldInfo>();
                    // проходим по всем полям рекордсета ..
                    for (int fieldInd = 0; fieldInd < mdxRecordset.Fields.Count; fieldInd++)
                    {
                        ADODB.Field adoField = mdxRecordset.Fields[fieldInd];
                        string rowSetName = adoField.Name;
                        if (!MeasuresPresent(rowSetName))
                            continue;
                        // если да - данные по этому полю всегда пойдут в XML
                        // нужно запомнить его параметры
                        string fieldAlias = GetFieldAlias(rowSetName);
                        // создаем объект для оптимизированного доступа который будет содержать
                        // указатель на поле и название для тэга.
                        // Помещаем его в список
                        fieldsInfo.Add(new AdoFieldInfo(adoField, fieldAlias));
                        TotalPresence = TotalPresence || TotalPresent(fieldAlias);
                    }
                    // если данные в рекдсете есть - начинаем формирование секции с данными
                    if (TotalPresence)
                    {
                        while (!mdxRecordset.EOF)
                        {
                            writer.WriteStartElement("row");
                            // Используем созданный выше список описателей полей 
                            // берем данные только из полей определенных там
                            foreach (AdoFieldInfo adoFieldInfo in fieldsInfo)
                            {
                                // получаем упакованное значение, приведение к строке пока не производим
                                object value = adoFieldInfo.Field.Value;
                                // если значение пустое - писать его в XML не нужно
                                if ((DBNull.Value != value) && (null != value))
                                    SetXMLAttribute(writer, adoFieldInfo.FieldAlias, value);
                            }
                            writer.WriteEndElement();
                            mdxRecordset.MoveNext();
                        }
                    }
                    // очищаем список описателей полей
                    fieldsInfo.Clear();
                }
                // завершаем формирование XML
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
                stream.Position = 0;
                time = DateTime.Now - startTime;
                Trace.TraceInformation("- Пользователь {0}, сессия {1}. Ответ сформирован за {2}",
                    Authentication.UserName, SessionContext.SessionId, time.ToString());
                //преобразуем содержимое XmlWriter в строку
                return Encoding.GetEncoding(1251).GetString(stream.ToArray());
            }
            catch (OutOfMemoryException exp)
            {
                return EmbraceException(exp.Message +
                    Environment.NewLine +
                    MemoryStatus.GetAvaillableMemoryReport() +
                    Diagnostics.KristaDiagnostics.ExpandException(exp));
            }
            catch (Exception exp)
            {
                return EmbraceException(exp.Message +
                    Diagnostics.KristaDiagnostics.ExpandException(exp));
            }
            finally
            {
                if (mdxRecordset.State == 1)
                    mdxRecordset.Close();
                DestroyConnections(false, false, true);
                stream.Close();
                exceptionContext = "";
            }

        }

        #region Заглушка для обхождения проблемы OutOfMemory на очень больших cellset'ах
        private void ClearMem()
        {
            GC.GetTotalMemory(true);
        }

        private void ClearAdoNetObjects(ref XmlReader rdr, ref AdomdCommand cmd, ref AdomdConnection cn)
        {
            if (rdr != null)
            {
                rdr.Close();
                rdr = null;
            }
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
            if ((cn != null) && (cn.State == ConnectionState.Open))
            {
                cn.Close();
                cn = null;
            }
        }




        #endregion

        /// <summary>
        /// Выполнение MDX-запроса через Cellset
        /// </summary>
        /// <param name="queryText"> текст запроса </param>
        /// <returns> имя временного файла с данными запроса </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetCellsetData(string queryText)
        {
            // ******************************************************************************
            // Этот метод вызывается для получения очень больших выборок данных (сотни мегабайт).
            // Во избежании запроса больших кусков памяти будем использовать сохранение во вспомогательный файл.
            // Чтение из файла и формирование строки ответа поручим ASP.NET и IIS'у :)
            // ******************************************************************************

            exceptionContext = "Получение ADOMD.Cellset";
            // освобождаем объекты которые могли остаться от предыдущих вызовов
            ClearMem();
            // форимируем строку подключения
            string connectionString = String.Format("Provider=MSOLAP;Data Source={0};Initial Catalog={1};MDX Unique Name Style=2", serverName, databaseName);
            // заранее декларируем объекты (для гарантированоого особождения)
            AdomdConnection cn = null;
            AdomdCommand cmd = null;
            XmlReader rdr = null;
            try
            {
                // формируем имя для временного файла
                string tempFileName = GetTempFileNameForCellsetData();
                // создаем подключение
                cn = new AdomdConnection(connectionString);
                cn.Open();
                DateTime startTime = DateTime.Now;
                Trace.TraceInformation("{0} MDX-запрос. Пользователь {1}, сессия {2}", DateTime.Now,
                    Authentication.UserName, SessionContext.SessionId);
                // ..создаем команду
                cmd = cn.CreateCommand();
                cmd.CommandText = queryText;
                cmd.CommandType = CommandType.Text;
                // ..получаем интерфейс для чтения данных запроса в виде XML
                rdr = cmd.ExecuteXmlReader();
                // создаем XML-документ
                XmlDocument resultXmlDocument = new XmlDocument();
                // загружаем данные в него
                resultXmlDocument.Load(rdr);
                TimeSpan time = DateTime.Now - startTime;
                Trace.Indent();
                Trace.TraceInformation("- Пользователь {0}, сессия {1}. запрос успешно выполнен за {2}",
                    Authentication.UserName, SessionContext.SessionId, time.ToString());
                Trace.Unindent();


                // ************
                // TODO: Правильнее было бы писать сразу на диск (или прямо в поток Response)
                // Нужно разобраться с чтением потока XML
                // ************

                // Освобождаем объекты ADOMD.NET, более они нам не нужны
                ClearAdoNetObjects(ref rdr, ref cmd, ref cn);
                ClearMem();


                // создаем объект для записи XML на диск
                XmlWriterSettings st = new XmlWriterSettings();
                // ************
                // PlanningProvider почему-то понимает только данные в кодировке UTF8
                // TODO: Замена кодировки на win1251 позволит сократить потребление памяти
                // и время передачи данных на клиента примерно на треть
                // ************
                st.Encoding = Encoding.UTF8;
                // декларация xml писать не нужно
                st.OmitXmlDeclaration = true;
                // форматировать - нужно (во избежание запроса больших кусков памяти 
                // читаться этот XML будет построчно)
                // TODO: В рамках дальнейшей оптимизации можно писать XML без форматирования и читать блоками
                st.Indent = true;
                // закрывать файловый объект при закрытии врайтера
                st.CloseOutput = true;
                // не проверять символы на допустимость
                st.CheckCharacters = false;
                // создаем врайтер с вышеперечисленными настройками
                XmlWriter writer = XmlWriter.Create(tempFileName, st);
                // сохраняем данные документа в кэш
                resultXmlDocument.Save(writer);
                // освобождаем врайтер
                writer.Close();
                writer = null;
                // освобождаем документ
                string result = resultXmlDocument.InnerXml;
                XmlHelper.ClearDomDocument(ref resultXmlDocument);
                ClearMem();
                time = DateTime.Now - startTime;
                Trace.TraceInformation("- Пользователь {0}, сессия {1}. Ответ сформирован за {2}",
                    Authentication.UserName, SessionContext.SessionId, time.ToString());

                return result;
            }
            catch (Exception exp)
            {
                // при возникновении исключения не глушим его, а передаем дальше
                throw new Exception(exp.Message);
                //return EmbraceException(exp.Message);
            }
            finally
            {
                // в любом случае пытаемся освободить объекты ADOMD.NET и память
                ClearAdoNetObjects(ref rdr, ref cmd, ref cn);
                ClearMem();
                exceptionContext = "";
            }
        }

        /// <summary>
        /// Обновляет дату процессинга у измерения в метаданных, если этот узел там есть
        /// </summary>
        /// <param name="metadataDOM">Документ метаданных</param>
        /// <param name="dimName">Имя измерения</param>
        /// <param name="hierName">Имя иерархии</param>
        private void UpdateDimensionMetadataNode(XmlDocument metadataDOM,
            string dimName, string hierName)
        {
            string xPath = "function_result/SharedDimensions/Dimension[@name='" + dimName +
               "']/Hierarchy[@name='" + hierName + "']";
            XmlNode hierachyElement = metadataDOM.SelectSingleNode(xPath);
            if (hierachyElement != null)
                XmlHelper.SetAttribute(hierachyElement, "processing_date",
                    GetLastProcessingDate(dimName, hierName));
        }


        /// <summary>
        /// Обновляет данные об одном измерении в серверном кэше
        /// </summary>
        /// <param name="metadataDOM">Загруженный DOM с метаданными</param>
        /// <param name="dimFullName">Имя измерения</param>
        private void UpdateDimensionMetadata(XmlDocument metadataDOM, string dimFullName)
        {
            string dimName = ExtractDimensionName(dimFullName);
            string hierName = ExtractHierarchyName(dimFullName);

            //обновляем дату процессинга
            UpdateDimensionMetadataNode(metadataDOM, dimName, hierName);

            //удаляем данные из кэша
            DeleteDimensionByName(dimName, hierName);
        }


        /// <summary>
        /// Обновляет данные хеша для указанных измерение.
        /// </summary>
        /// <remarks>Вызываестся диспетчером расчета многомерных объектов после их обработки.</remarks>
        /// <param name="names">Список имен измерений.</param>
        public void RefreshDimension(string[] names)
        {
            XmlDocument metadataDOM = LoadMetadataDOM();
            if (metadataDOM == null)
                return; //если метаданных нет, тогда ничего и не делаем
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitConnections(true, false, true);
            try
            {
                //для каждого переданного имени измерения вызываем обновление
                lock (mdSyncObj)
                {
                    foreach (string dimName in names)
                        UpdateDimensionMetadata(metadataDOM, dimName);

                    UpdateMetadataDate(metadataDOM);
                    SaveMetadata(metadataDOM);
                }
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref metadataDOM);
                DestroyConnections(true, false, true);
                GC.Collect();
            }
            Trace.TraceInformation("{0} Обновление измерений {1}. Сессия {2}. Завершено за {3} мс",
                DateTime.Now, String.Join(", ", names), SessionContext.SessionId, sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Обновляет данные об одном кубе в серверном кэше.
        /// В отличие от прежнего варианта, отображает структурные изменения
        /// </summary>
        /// <param name="metadataDOM">Загруженный DOM с метаданными</param>
        /// <param name="cubeName">Имя куба</param>
        private void UpdateCubeMetadata(XmlDocument metadataDOM, string cubeName)
        {
            XmlNode rootNode = metadataDOM.SelectSingleNode("function_result/Cubes");
            XmlNode cubeNode = metadataDOM.SelectSingleNode("function_result/Cubes/Cube[@name='" + cubeName + "']");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (cubeNode != null)
            {
                rootNode.RemoveChild(cubeNode);
                cubeNode = null;
            }

            GetCubes(rootNode, cubeName);
            sw.Stop();
            Trace.TraceInformation("{0} Обновление куба {1} завершено за {2} мс, сессия {3}",
                DateTime.Now, cubeName, sw.ElapsedMilliseconds, SessionContext.SessionId);

        }


        /// <summary>
        /// Обновляет данные кэша для указанных кубов.
        /// </summary>
        /// <remarks>Вызываестся диспетчером расчета многомерных объектов после их обработки.</remarks>
        /// <param name="names">Список имен кубов.</param>
        public void RefreshCube(string[] names)
        {
            XmlDocument metadataDOM = LoadMetadataDOM();
            if (metadataDOM == null)
                return; //если метаданных нет, тогда ничего и не делаем

            InitConnections(true, false, true);
            try
            {
                //для каждого переданного имени куба вызываем обновление
                lock (mdSyncObj)
                {
                    foreach (string cubeName in names)
                        UpdateCubeMetadata(metadataDOM, cubeName);

                    UpdateMetadataDate(metadataDOM);
                    SaveMetadata(metadataDOM);
                    DeleteOldDimensions(metadataDOM);
                }
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref metadataDOM);
                DestroyConnections(true, false, true);
                GC.Collect();
            }
        }

        #endregion

        /// <summary>
        /// Имя сервера
        /// </summary>
        internal string DataSource
        {
            get { return serverName; }
        }

        /// <summary>
        /// Имя базы
        /// </summary>
        internal string Catalog
        {
            get { return databaseName; }
        }

        /// <summary>
        /// Идентификатор провайдера (для работы с несколькими базами)
        /// </summary>
        internal string ProviderId
        {
            get { return providerId; }
        }

        internal bool MetadataUpdated
        {
            get { return metadataUpdated; }
        }
    }
}
