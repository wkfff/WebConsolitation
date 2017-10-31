using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

using ADODB;
using Microsoft.AnalysisServices.AdomdClient;

using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;


namespace Krista.FM.Server.Dashboards.Core.DataProviders
{    
    /// <summary>
    /// Поставщик данных для элементов.
    /// Абстрагирует для страниц:
    ///     - Работу с подключением. Инициализиуреся строкой подключения
    ///     - Работу с текстами запросами. Достает их из условленного места. 
    ///     - Работу по выполнению запроса
    ///     - Преобразование результатов запроса к преемлемому для визуализатора виду.
    /// </summary>
    public partial class DataProvider
    {        
        #region Свойства
        
        private string connectionKey;
        /// <summary>
        /// Ключ подключения
        /// </summary>
        public string ConnectionKey
        {
            get { return connectionKey; }
            set { connectionKey = value; }
        }

        private string connectionString = String.Empty;

        //объект подключения
        private AdomdConnection connection;
        

        //строка подключения                
        private string ConnectionString
        {
            get 
            {
                if (String.IsNullOrEmpty(connectionString))
                {
                    return ConfigurationManager.ConnectionStrings[ConnectionKey].ConnectionString;
                }
                else
                {
                    return connectionString;
                }
            }
        }        
        
        //Сообщение о последней ошибке
        private string lastError;

        public string LastError
        {
            get { return lastError; }            
        }
        
        //Рабочий каталог класса.
        //Там должны лежать необходимые ресурсы, которые ищутся по именным умолчаниям.
        //Путь храниться в абсолютной нотации
        private string workDir;
        public string WorkDir
        {
            get { return workDir; }
            set { workDir = value; }
        }
        
        #endregion
                
        public DataProvider()
        {            
            connection = null;
            workDir = string.Empty;
            lastError = string.Empty;
        }

        public DataProvider(string connectionString)
        {
            this.connectionString = connectionString;
            connection = null;
            workDir = string.Empty;
            lastError = string.Empty;
        }

        #region Интерфейсные методы плучения данных
        
        /// <summary>
        /// Получение селл-сета по запросу в режиме as is
        /// </summary>
        /// <param name="queryText">Текст запроса в ресурсах</param>
        public CellSet GetCellset(string queryText)
        {
            CellSet cls = null;
//              string cacheKey = GetCacheKey(queryText);
              // Если ключ есть, то поищем в кэше
//              if (!String.IsNullOrEmpty(cacheKey) && HttpContext.Current.Cache[cacheKey] != null)
//              {
//                  cls = (CellSet)HttpContext.Current.Cache[cacheKey];
//              } 
              // Иначе получаем из базы.
//              else
            if (CheckConnect())
            {
                lastError = string.Empty;
                string qText = queryText;
                AdomdCommand cmd = null;
                if (!string.IsNullOrEmpty(qText))
                {
                    try
                    {
                        CRHelper.SaveToQueryLog(qText);
                        cmd = new AdomdCommand(qText, connection);
                        cls = cmd.ExecuteCellSet();
//                        if (!String.IsNullOrEmpty(cacheKey))
//                        {
//                            HttpContext.Current.Cache.Insert(cacheKey, cls);
//                        }
                    }
                    catch (Exception e)
                    {
                        string errDescr = string.Format("Ошибка выполнения запроса. /n {0} /n {1}", e.Message, qText);
                        CRHelper.SaveToErrorLog(errDescr);
                    }
                    finally
                    {
                        if (cmd != null)
                            cmd.Dispose();
                    }
                }
            }
            return cls;
        }
        
        /// <summary>
        /// Получение таблицы для диаграммы
        /// </summary>
        public void GetDataTableForChart(string queryText, string seriesFieldName, DataTable dt)
        {
            if (dt == null) return;
            dt.Clear();
            CellSet cls = GetCellset(queryText);
            if (cls != null)
            {
                PopulateDataTableForChart(cls, dt, seriesFieldName);
            }
        }

		/// <summary>
		/// Получает таблицу по имени запроса и провайдеру данных
		/// </summary>
		public static DataTable GetDataTableForChart(string queryID, DataProvider provider)
		{
			DataTable table = new DataTable();
			string query = GetQueryText(queryID);
			provider.GetDataTableForChart(query, "title", table);
			return table;
		}

        /// <summary>
        /// Получение таблицы данных для сводной таблицы
        /// </summary>
        public void GetDataTableForPivotTable(string QueryText, DataTable dt)
        {
            if (dt == null) return;
            dt.Clear();

            if (CheckConnect())
            {
                lastError = string.Empty;
                //CellSet cls = GetCellset(QueryID);
                //if (cls != null)
                //{
                //    return TransformForPivotTable(cls);                                       
                //}

                Recordset rs = new Recordset();
                OleDbDataAdapter odda = new OleDbDataAdapter();
                Connection adodbConn = new Connection();

                CRHelper.SaveToQueryLog(QueryText);
                adodbConn.Open(ConnectionString, "", "", -1);
                rs.Open(QueryText, adodbConn, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly, 0);
                
                odda.Fill(dt, rs);                      
                odda.Dispose();
                adodbConn.Close();                          
            }                                    
        }
        
        /// <summary>
        /// Получить таблицу по запросу из хранилища
        /// </summary>
        public DataTable GetWarehouseDataTable(string QueryStr)
        {
            OleDbConnection warehouseConn = new OleDbConnection();
            DataTable resDT = new DataTable();

            try
            {
                try
                {
                    CRHelper.SaveToQueryLog(QueryStr);
                    warehouseConn.ConnectionString = ConfigurationManager.ConnectionStrings["warehouse"].ConnectionString;
                    OleDbDataAdapter adapter = new OleDbDataAdapter(QueryStr, warehouseConn);
                    adapter.Fill(resDT);

                }
                catch (Exception e)
                {
                    string errDescr = string.Format("Ошибка выполнения запроса. /n {0} /n {1}", e.Message, QueryStr);
                    CRHelper.SaveToErrorLog(QueryStr);
                                
                    return null;
                }
            }
            finally
            {
                warehouseConn.Close();
            }            
            
            return resDT;                       
        }
        
        #endregion

        #region Методы работы с текстом запроса (из CustumReportPage)

        

        /// <summary>
        /// Получение текста запроса по его коду.
        /// В случае неприятностей вернет пустую строку
        /// </summary>
        public static string GetQueryText(string QueryID)
        {
            return GetQueryText(QueryID, string.Empty);
        }

        /// <summary>
        /// Получение текста запроса по его коду и пути к хранилищу.
        /// В случае неприятностей кинет исключение.
        /// </summary>
        public static string GetQueryText(string QueryID, string Path)
        { 
            string queryDirPath =
                Path == string.Empty
                ? HttpContext.Current.Server.MapPath(".")
                : Path;
            DirectoryInfo dirInfo = new DirectoryInfo(queryDirPath);
            foreach (FileInfo f in dirInfo.GetFiles(CustomReportConst.QueryFileMasc))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(f.FullName);

#warning Нет проверки на успешность загрузки

                XmlNode xmlNode = xmlDoc.SelectSingleNode(string.Format("//query[@id='{0}']", QueryID));
                if (xmlNode != null)
                {
#warning обработка
                    //lastError = string.Format("Не найден запрос с кодом {0}", QueryID);
                    return Singleton<MdxQueryRenameService>.Instance.Rename(ImplantParams(xmlNode.LastChild.InnerText));
                }
            }
            throw new Exception("Не удалось найти текст запроса " + QueryID);
        }

        /// <summary>
        /// Применить параметры к запросу
        /// </summary>
        /// <param name="queryText">Первоначальный текст запроса</param>
        private static string ImplantParams(string queryText)
        {
            Regex regex = new Regex(@"<%([\s\S]*?)%>");
            MatchCollection paramCollection = regex.Matches(queryText);

            foreach (Match match in paramCollection)
            {
                string paramName = match.Value.TrimStart('<').TrimStart('%').TrimEnd('>').TrimEnd('%');
                queryText = queryText.Replace(match.Value, GetParamMDXSet(paramName));
            }

            return queryText;
        }

        /// <summary>
        /// Получить MDX-множество параметра страницы
        /// </summary>
        /// <param name="hierarchyName">Юник-нэйм измерения</param>
        /// <returns></returns>
        private static string GetParamMDXSet(string hierarchyName)
        {
            //Если не нашли в параметрах, пытаемся взять из переменных сессии
            foreach (string key in HttpContext.Current.Session.Contents.Contents.Keys)
            {
                if (key == hierarchyName) return CustomParam.CustomParamFactory(hierarchyName).Value;
            }

            return "{}";
        }

        #endregion

        private string GetCacheKey(string queryText)
        {
            // Если не одно совпадение, значит непорядок с запросом.
            if (Regex.Match(queryText, "<#.*?#>").Captures.Count != 1)
            {
                return string.Empty;
            }
            string cacheKey = string.Empty;
            // Берем имена кубов
            string cubesString = Regex.Match(queryText, "<#.*?#>").Captures[0].Value;
            // Очищаем от оберток
            cubesString = cubesString.Trim('<');
            cubesString = cubesString.Trim('>');
            cubesString = cubesString.Trim('#');
            // разбиваем на части
            string[] cubes = cubesString.Split(';');
            if (CheckConnect())
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    // отрезаем последние скобки и формируем строку куб: время расчета.
                    cubes[i] = cubes[i].Trim('[');
                    cubes[i] = cubes[i].Trim(']');
                    cacheKey += cubes[i] + " " + GetCubeLastProcessedDate(cubes[i]).ToString();
                }
            }
            // Приписываем сам запрос
            cacheKey += Environment.NewLine;
            cacheKey += queryText;
            return cacheKey;
        }

        public DateTime GetCubeLastProcessedDate(string cubeName)
        {
            if (CheckConnect())
            {
                return connection.Cubes[cubeName].LastProcessed;
            }

            return DateTime.MinValue;
        }
    }
}
