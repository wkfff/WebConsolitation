using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.OLAPAdmin.CAExceptions;
using Krista.FM.Client.OLAPAdmin.Exceptions;
using Krista.FM.Client.OLAPStructures;
using Microsoft.AnalysisServices;
using Infragistics.Win.UltraWinTree;

namespace Krista.FM.Client.OLAPAdmin
{
    public partial class ApplayPatchForm : Form
    {
        /// <summary>
        /// сервер
        /// </summary>
        private Microsoft.AnalysisServices.Server server = null;

        private BackgroundWorker backgroundWorker = null;

        private ArrayList array = new ArrayList();

        private UltraTreeNode prevNode = null;

        /// <summary>
        /// Пакет
        /// </summary>
        private string filescriptpath = string.Empty;
        /// <summary>
        /// Патч
        /// </summary>
        private XmlDocument scriptDoc = new XmlDocument();

        private PatchManager patchManager = null;

        /// <summary>
        /// Текст ошибки
        /// </summary>
        private string errorMessage = String.Empty;

        /// <summary>
        /// Имя базы
        /// </summary>
        private string databaseName;

        public ApplayPatchForm()
        {
            InitializeComponent();
        }

        public ApplayPatchForm(Microsoft.AnalysisServices.Server server)
            : this()
        {
            this.server = server;

            InitControls();

            InitializeBackgroundWorker();
        }

        private void InitControls()
        {
            databasesBox.SelectedValueChanged += new EventHandler(databasesBox_SelectedValueChanged);
           
            if (server.Databases.Count == 0)
                throw new Exception("Для выбранного сервена нет ни одной многомерной базы!");

            foreach (Database database in server.Databases)
            {
                databasesBox.Items.Add(database.Name);
            }

            databasesBox.Text = databasesBox.Items[0].ToString();
            databaseName = databasesBox.Text;
        }
        
        private void SubscribeToSessionTrace()
        {
            server.SessionTrace.OnEvent +=new TraceEventHandler(SessionTrace_OnEvent);
            server.SessionTrace.Stopped +=new TraceStoppedEventHandler(SessionTrace_Stopped);
            server.SessionTrace.Start();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (server != null)
            {
                UnsubscribeSessionTrace();

                progressBar1.Value = 100;
            }

            buttonApplayScript.Enabled = false;

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(String.Format("Патч применен с ошибкой: {0}", errorMessage), "Ошибка при применении",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Патч применен успешно!", "Complited", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        /// <summary>
        /// Отписываемся от подписки на события сервера
        /// </summary>
        private void UnsubscribeSessionTrace()
        {
            server.SessionTrace.Stop();
        }

        static void SessionTrace_Stopped(ITrace sender, TraceStoppedEventArgs e)
        {
            if (e.StopCause == TraceStopCause.StoppedByException)
                throw e.Exception;
        }

        void SessionTrace_OnEvent(object sender, TraceEventArgs e)
        {
            try
            {
                if (e.EventClass != TraceEventClass.Notification)
                    IncrementReport();

                string text = (e.TextData != null && e.TextData.Length > 1000) ? e.TextData.Substring(0, 1000) : e.TextData;

                switch (e.EventClass)
                {
                    case TraceEventClass.CommandBegin:
                        AddNode(String.Format("{0}:{1}", e.ObjectName, text), AddNodeType.NodeBegin, Color.Black);
                        break;
                    case TraceEventClass.CommandEnd:
                        AddNode(String.Format("{0}:{1}", e.ObjectName, text), AddNodeType.NodeEnd, Color.Black);
                        break;
                    case TraceEventClass.ExecuteMdxScriptBegin:
                        AddNode(text, AddNodeType.NodeBegin, Color.DarkViolet);
                        break;
                    case TraceEventClass.ExecuteMdxScriptEnd:
                        AddNode(text, AddNodeType.NodeEnd, Color.DarkViolet);
                        break;
                    case TraceEventClass.ExecuteMdxScriptCurrent:
                        AddNode(text, AddNodeType.NodeInsert, Color.DarkViolet);
                        break;
                    case TraceEventClass.ProgressReportError:
                        AddNode(text, AddNodeType.NodeInsert, Color.Red);
                        errorMessage = e.TextData;
                        break;
                    case TraceEventClass.Error:
                        AddNode(text, AddNodeType.NodeInsert, Color.Red);
                        errorMessage = e.TextData;
                        break;
                }
            }
            catch
            {
            }
        }

        private delegate void AddNodeDelegate(string text, AddNodeType nodetype, Color color);

        private void AddNode(string s, AddNodeType begin, Color color)
        {
            if (ultraTreeExLog.InvokeRequired)
            {
                AddNodeDelegate d = new AddNodeDelegate(AddNode);
                ultraTreeExLog.Invoke(d, new object[] { s, begin, color });
            }
            else
            {
                UltraTreeNode node;
                switch (begin)
                {
                    case AddNodeType.NodeBegin:
                        node = new UltraTreeNode(s);
                        node.Override.NodeAppearance.ForeColor = color;
                        if (prevNode != null)
                        {
                            prevNode.Nodes.Add(node);
                            prevNode.ExpandAll();
                        }
                        else
                            ultraTreeExLog.Nodes.Add(node);

                        prevNode = node;
                        array.Add(node);
                        break;

                    case AddNodeType.NodeEnd:
                        node = (UltraTreeNode)array[array.Count - 1];
                        node.Text = s;
                        prevNode = node.Parent;
                        node.Override.NodeAppearance.ForeColor = color;
                        array.Remove(node);
                        break;

                    case AddNodeType.NodeInsert:
                        node = new UltraTreeNode(s);
                        node.Override.NodeAppearance.ForeColor = color;
                        if (prevNode != null)
                        {
                            prevNode.Nodes.Add(node);
                            prevNode.ExpandAll();
                        }
                        else
                            ultraTreeExLog.Nodes.Add(node);

                        //array.Add(node);

                        break;

                    default:
                        node = null;
                        break;
                }
                //if (node != null) node.ForeColor = black;
            }
        }

        private delegate void IncrementReportDelegate();

        private void IncrementReport()
        {
            if(progressBar1.InvokeRequired)
            {
                IncrementReportDelegate d = new IncrementReportDelegate(IncrementReport);
                progressBar1.Invoke(d);
            }
            else
            {
                progressBar1.Increment(10);
            }
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Operation operation = new Operation();
            try
            {
                operation.StartOperation();
                string s = e.Argument.ToString();

                operation.Text = "Обновление Data Source View";

                // Обновление Data Source View
                UpdateDataSourceView();
                UpdateRelations();

                operation.Text = "Применение патча";
                XmlaResultCollection resultCollection = server.Execute(s);
               
                Thread.Sleep(5000);

                e.Result = true;
            }
            catch (Exception ex)
            {
                ultraTreeExLog.Nodes.Add(ex.Message);
                errorMessage = ex.Message;

                e.Result = false;
            }
            finally
            {
                operation.StopOperation();
                operation.ReleaseThread();
            }
        }

        #region Update Data Source View

        /// <summary>
        /// Обновление источника данных
        /// </summary>
        /// <exception cref="AbandonedMutexException"></exception>
        private void UpdateDataSourceView()
        {
            try
            {
                if (server.Databases.FindByName(databaseName).DataSourceViews.Count == 0)
                    return;

                DataSourceView dsv = server.Databases.FindByName(databaseName).DataSourceViews[0];

                // Получаем строку подключения к базе
                string connectionString = GetConectionString(server.Name, databaseName, dsv.DataSource.ConnectionString);
                
                using (OleDbConnection cn = new OleDbConnection(connectionString)/*@"Provider=OraOLEDB.Oracle.1;Data Source=dv;Persist Security Info=True;Password=dv;User ID=dv"*/)
                {
                    cn.Open();

                    foreach (BaseAMOObject baseAmoObject in patchManager.Dictionary.Values)
                    {
                        if (baseAmoObject is AMOCube || baseAmoObject is AMODimension)
                        {
                            if (baseAmoObject.Method == "Delete")
                                continue;

                            if (String.IsNullOrEmpty(baseAmoObject.DsvName))
                            {
                                ultraTreeExLog.Nodes.Add(string.Format("{0}: Для объекта {1} не указано DSVName",
                                                                        baseAmoObject.GetType(), baseAmoObject.Name));
                                continue;
                            }
                            string[] tableNames = baseAmoObject.DsvName.Split(',');

                            List<string> tabList = new List<string>();
                            tabList.AddRange(tableNames);

                            foreach (string s in tableNames)
                            {
                                string[] parts = Regex.Split(s, "__");
                                foreach (string part in
                                    parts.Where(part => Regex.IsMatch(part.Trim(), "\\w{1,2}_{1}")).Where(part => !tabList.Contains(part.Trim())))
                                {
                                    tabList.Add(part.Trim());
                                }
                            }

                            foreach (var table in tabList)
                            {
                                string tableName = table.Trim();
                                if (dsv.Schema.Tables.Contains(tableName))
                                {
                                    UpdateTables(dsv, cn, tableName);
                                    continue;
                                }

                                AddTables(tableName, dsv, cn, baseAmoObject);
                            }
                        }
                    }
                }
            }
            catch (OleDbException e)
            {
                throw new UpdateDataSourceViewException(e.Message);
            }
            catch (InvalidOperationException e)
            {
                throw new UpdateDataSourceViewException(e.Message);
            }
        }

        private static string GetConectionString(string serverName, string databaseName, string connectionString)
        {
            connectionString = String.Format("{0};Password=dv", connectionString);

            // Если есть специально настроеное подключение, используем его
            string directoryName = Path.Combine(
                        Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                                     serverName),
                        databaseName);
            if (!Directory.Exists(directoryName))
            {
                // если нет, то берет из строки подключени DataSourceView с паролем dv
                return connectionString;
            }

            string[] files = Directory.GetFiles(directoryName, "*.udl");

            if (files.Count() > 0)
            {
                return ReadConnectionStringFromFile(files[0]);
            }

            return connectionString;
        }

        private static string ReadConnectionStringFromFile(string file)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "[oledb]")
                            continue;
                        if (line == "; Everything after this line is an OLE DB initstring")
                            continue;

                        if (line.Contains("Provider"))
                        {
                            return line;
                        }
                    }

                    throw new Exception("Неверная строка подключения.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Определение типа объекта в предвставлинии источника данных
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static TableType GetTableType(string tableName)
        {
            if (String.IsNullOrEmpty(tableName))
                throw new Exception("Не указано имя в DSV.");

            return !tableName.StartsWith("OLAP_") ? TableType.table : TableType.view;
        }

        /// <summary>
        /// Добавление нового объекта
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dsv"></param>
        /// <param name="connection"></param>
        /// <param name="tabaseAmoObject"></param>
        static void AddTables(string tableName, DataSourceView dsv, OleDbConnection connection, BaseAMOObject tabaseAmoObject)
        {
            TableType tableType = GetTableType(tabaseAmoObject.DsvName);

            string strSelectText = String.Empty;

            if (!tableName.StartsWith("OLAP_"))
            {
                strSelectText = string.Format("SELECT * FROM DV.{0} WHERE 1 = 0", tableName);
                AddTable(tabaseAmoObject, tableName, strSelectText, connection, dsv, tableType);
                return;
            }

            string[] parts = Regex.Split(tableName, "__");
            foreach (string part in parts)
            {
                if (!dsv.Schema.Tables.Contains(tableName))
                {
                    if (Regex.IsMatch(part.Trim(), "\\w{1,2}_{1}"))
                    {
                        strSelectText = string.Format("SELECT * FROM DV.{0} WHERE 1 = 0", part.Trim());
                        AddTable(tabaseAmoObject, tableName, strSelectText, connection, dsv, tableType);
                        UpdateTables(dsv, connection, tableName);
                    }
                }
                else
                {
                    UpdateTables(dsv, connection, tableName);
                }
            }
        }

        private static void AddTable(BaseAMOObject tabaseAmoObject, string tableName, string strSelectText, OleDbConnection connection, DataSourceView dsv, TableType tableType)
        {
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSelectText, connection))
            {
                DataTable[] dataTables = adapter.FillSchema(dsv.Schema,
                                                            SchemaType.Mapped, tableName);
                using (DataTable dataTable = dataTables[0])
                {
                    SetTableProperties(tableName, tableType, dataTable, tabaseAmoObject.QueryDefinition);

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        dataTable.Columns[i].ColumnName = dataTable.Columns[i].ColumnName.ToUpperInvariant();
                    }
                }
            }

            dsv.Update();
        }

        private static void SetTableProperties(string tableName, TableType tableType, DataTable dataTable, string queryDefinition)
        {
            switch (tableType)
            {
                case TableType.table:
                    dataTable.ExtendedProperties.Add("TableType", "Table");
                    dataTable.ExtendedProperties.Add("DbSchemaName", "DV");
                    dataTable.ExtendedProperties.Add("DbTableName", tableName);
                    dataTable.ExtendedProperties.Add("FriendlyName", tableName);
                    break;
                case TableType.view:
                    dataTable.ExtendedProperties.Add("TableType", "View");
                    dataTable.ExtendedProperties.Add("ViewName", tableName);
                    dataTable.ExtendedProperties.Add("QueryDefinition", queryDefinition);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Обновление существующего
        /// </summary>
        /// <param name="dsv"></param>
        /// <param name="connection"></param>
        static void UpdateTables(DataSourceView dsv, OleDbConnection connection, string tableName)
        {
            string strSelectText = String.Empty;

            if (!tableName.StartsWith("OLAP_"))
            {
                strSelectText = string.Format("SELECT * FROM DV.{0} WHERE 1 = 0", tableName);
                UpdateTable(tableName, strSelectText, connection, dsv);
                return;
            }

            string[] parts = Regex.Split(tableName, "__");
            foreach (string part in parts)
            {
                if (Regex.IsMatch(part.Trim(), "\\w{1,2}_{1}"))
                {
                    strSelectText = string.Format("SELECT * FROM DV.{0} WHERE 1 = 0", part.Trim());
                    UpdateTable(tableName, strSelectText, connection, dsv);
                }
            }
        }

        private static void UpdateTable(string tableName, string strSelectText, OleDbConnection connection, DataSourceView dsv)
        {
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSelectText, connection))
            {
                DataTable dataTable = adapter.FillSchema(dsv.Schema.Tables[tableName], SchemaType.Mapped);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    dataTable.Columns[i].ColumnName = dataTable.Columns[i].ColumnName.ToUpperInvariant();
                }
            }

            dsv.Update();
        }

        private enum TableType
        {
            table,
            view
        }

        #endregion

        #region Update relations

        private void UpdateRelations()
        {
            try
            {
                var joinClauseList = new List<RelationInfo>();

                if (server.Databases.FindByName(databaseName).DataSourceViews.Count == 0)
                    return;

                DataSourceView dsv = server.Databases.FindByName(databaseName).DataSourceViews[0];

                string connectionString = GetConectionString(server.Name, databaseName, dsv.DataSource.ConnectionString);

                using (
                    OleDbConnection cn =
                        new OleDbConnection(connectionString
                            /*@"Provider=OraOLEDB.Oracle.1;Data Source=dv;Persist Security Info=True;Password=dv;User ID=dv"*/)
                    )
                {
                    cn.Open();

                    foreach (var baseAmoObject in patchManager.Dictionary.Values)
                    {
                        if (!String.IsNullOrEmpty(baseAmoObject.JoinClause))
                        {
                            if (!String.IsNullOrEmpty(baseAmoObject.JoinClause))
                                OLAPUtils2000.ReadRelation(baseAmoObject.JoinClause, joinClauseList);
                        }
                    }
                }

                RenameRelations(joinClauseList);

                if (joinClauseList.Count > 0)
                {
                    RelationComparer relationComparer = new RelationComparer();
                    joinClauseList.Sort(relationComparer);
                    for (int i = 0; i < joinClauseList.Count; i++)
                        AddRelation(joinClauseList[i], dsv);
                    dsv.Update();
                }
            }
            catch (Exception e)
            {
                throw new UpdateDataSourceViewException(e.Message);
            }
        }

        private void RenameRelations(List<RelationInfo> relationsInfo)
        {
            foreach (var value in patchManager.Dictionary.Values)
            {
                if (String.IsNullOrEmpty(value.DsvName))
                    continue;

                if (GetTableType(value.DsvName) == TableType.view)
                {
                    TableInfo info = new TableInfo(value.DsvName, value.QueryDefinition);
                    info.TableNameDictionary = SetTableNameDictionary(value.QueryDefinition);
                    OLAPUtils2000.ReplaceRelationsTableName(relationsInfo, info);

                    /*Dictionary<string , string >  names = SetTableNameDictionary(value.QueryDefinition);
                    foreach (var relationInfo in relationsInfo)
                    {
                        CheckRelationInfo(relationInfo, names);
                    }*/
                }
            }
        }

        /// <summary>
        /// Извлекает имена таблиц из SQL запроса и заполняет ими словарь tableNameList.
        /// </summary>
        private static Dictionary<string, string> SetTableNameDictionary(string _queryDefinition)
        {
            Dictionary<string , string > tableNameDictionary = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(_queryDefinition))
            {
                int fromIndex = _queryDefinition.IndexOf("from", StringComparison.OrdinalIgnoreCase);
                int whereIndex = _queryDefinition.IndexOf("where", StringComparison.OrdinalIgnoreCase);
                string names;
                if (fromIndex > -1)
                {
                    fromIndex += 5; //Увеличиваем на длину слова "from" + один символ на пробел.
                    if (whereIndex > -1)
                    {
                        names = _queryDefinition.Substring(fromIndex, whereIndex - fromIndex).Trim();
                    }
                    else
                    {
                        names = _queryDefinition.Substring(fromIndex).Trim();
                    }
                    //Получаем имена таблиц и их алиасы.
                    string[] nameAndAliasArray = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    //Разделяем имена таблиц и алиасов, приводим к верхнему регистру и заносим их в словарь.
                    foreach (string nameAndAlias in nameAndAliasArray)
                    {
                        string[] nameArray = nameAndAlias.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameArray.Length == 1)
                        {
                            //Алиаса нет, только имя.
                            tableNameDictionary.Add(nameArray[0].Trim().ToUpperInvariant(), string.Empty);
                        }
                        else
                            if (nameArray.Length == 2)
                            {
                                //Есть и алиас, и имя.
                                tableNameDictionary.Add(nameArray[0].Trim().ToUpperInvariant(), nameArray[1].Trim().ToUpperInvariant());
                            }
                    }
                }
            }

            return tableNameDictionary;
            ////Заносим имя самой TableInfo в список используемых имен.
            //if (!string.IsNullOrEmpty(_name) && !tableNameDictionary.ContainsKey(_name.ToUpperInvariant()))
            //{
            //    tableNameDictionary.Add(_name, string.Empty);
            //}
        }

        private static void AddRelation(RelationInfo relInfo, DataSourceView dsv)
        {
            try
            {
                string pkTableName = relInfo.PrimaryKey.TableName;
                string fkTableName = relInfo.ForeignKey.TableName;
                CheckTableExist(pkTableName, dsv);
                CheckTableExist(fkTableName, dsv);
                string relationName = relInfo.Name;
                if (!dsv.Schema.Relations.Contains(relationName))
                {
                    DataColumn fkColumn =
                        dsv.Schema.Tables[fkTableName].Columns[relInfo.ForeignKey.ColumnName];
                    DataColumn pkColumn =
                        dsv.Schema.Tables[pkTableName].Columns[relInfo.PrimaryKey.ColumnName];
                    if (pkColumn != null && fkColumn != null)
                    {
                        if (pkColumn.DataType != fkColumn.DataType)
                        {
                            try
                            {
                                fkColumn.DataType = pkColumn.DataType;
                            }
                            catch (ArgumentException e)
                            {
                                throw new Exception(string.Format("Не удалось преобразовать тип атрибута: {0}", e.Message));
                            }
                        }
                        dsv.Schema.Relations.Add(relationName, pkColumn, fkColumn, false);
                    }
                    //DSView.Schema.Relations.Add(relationName, pkColumn, fkColumn, true);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static void CheckTableExist(string pkTableName, DataSourceView dsv)
        {
            try
            {
                int tableIndex = dsv.Schema.Tables.IndexOf(pkTableName);
                if (tableIndex < 0) { throw new TableNotExistException(pkTableName); }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion
        
        enum AddNodeType
        {
            NodeBegin,
            NodeEnd,
            NodeInsert
        }

        private void buttonApplayScript_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            ultraTreeExLog.Nodes.Clear();

            SubscribeToSessionTrace();

            if (!string.IsNullOrEmpty(scriptDoc.InnerXml))
            {
                try
                {
                    backgroundWorker.RunWorkerAsync(scriptDoc.InnerXml);
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("При применении патча возникла ошибка: {0}", ex.Message));
                }
                finally
                {
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.Filter = "xml package files (*.xmla)|*.xmla";
                fd.FilterIndex = 2;
                fd.RestoreDirectory = true;

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    filescriptpath = fd.FileName;
                    CheckFileName(filescriptpath);

                    filepath.Text = filescriptpath;
                }
                InitializePatchManager();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void InitializePatchManager()
        {
            if (!string.IsNullOrEmpty(filescriptpath))
            {
                InitializePatchDocument();

                // Добавляем пространство имен для использования XPath
                XmlNamespaceManager xnsman = new XmlNamespaceManager(scriptDoc.NameTable);
                xnsman.AddNamespace("as", "http://schemas.microsoft.com/analysisservices/2003/engine");

                patchManager = new PatchManager(scriptDoc, xnsman);

                InitializeTreeView();

                CollectTree();
            }
        }

        private void InitializePatchDocument()
        {
            try
            {
                scriptDoc.Load(filescriptpath);

                if (!String.IsNullOrEmpty(scriptDoc.InnerXml))
                    UpdateDatabaseName(databaseName, scriptDoc);

            }
            catch (XmlException e)
            {
                throw new Exception(String.Format("Ошибка при загрузки файла скрипта: {0}", e.Message));
            }
            
            /*try
            {
                // коллекция макросов
                Dictionary <string, string> macrosList = new Dictionary<string, string>();

                FileInfo info = new FileInfo(filescriptpath);

                XmlDocument packageDoc = new XmlDocument();
                packageDoc.Load(filescriptpath);

                XmlNode macrosNode = packageDoc.SelectSingleNode("XMLDSOConverter/package/macros");
                if (macrosNode != null)
                {
                    string macrosFile = macrosNode.Attributes["file"].InnerText;
                    XmlDocument macrosDoc = new XmlDocument();
                    macrosDoc.Load(info.DirectoryName + "//" + macrosFile);

                    XmlNodeList macrosNodeList = macrosDoc.SelectNodes("XMLDSOConverter/macroslist//macros");
                    foreach (XmlNode node in macrosNodeList)
                    {
                        macrosList.Add(node.Attributes["name"].InnerText, node.InnerText);
                    }
                }
                else
                    throw new Exception("В комулятивном патче отсутствуют макросы");

                XmlNode patchNode = packageDoc.SelectSingleNode("XMLDSOConverter/package/patch");
                if (patchNode != null)
                {
                    scriptDoc.Load(info.DirectoryName + "//" + patchNode.Attributes["file"].InnerText);
                    if (macrosList.ContainsKey("DatabaseName"))
                        UpdateDatabaseName(databaseName, scriptDoc);
                    else
                        throw new Exception("Не найден макрос с именем многомерной БД");
                }
                else
                    throw new Exception("В комулятивном патче отсутствует файл патча");

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя многомерной базы</param>
        /// <param name="doc">Скрипт</param>
        private void UpdateDatabaseName(string name, XmlDocument doc)
        {
            XmlNamespaceManager xmlns = new XmlNamespaceManager(doc.NameTable);
            xmlns.AddNamespace("as", "http://schemas.microsoft.com/analysisservices/2003/engine");

            // коллекция объектов для создания
            XmlNodeList createList = doc.SelectNodes("as:Batch//as:Create", xmlns);
            UpdateNode(createList, xmlns, name);

            //коллекция альтеров
            XmlNodeList alterList = doc.SelectNodes("as:Batch//as:Alter", xmlns);
            UpdateNode(alterList, xmlns, name);

            // коллекция объектов для удаления
            XmlNodeList deleteList = doc.SelectNodes("as:Batch//as:Delete", xmlns);
            UpdateNode(deleteList, xmlns, name);
        }

        private void UpdateNode(XmlNodeList list, XmlNamespaceManager xmlns, string name)
        {
            foreach (XmlNode node in list)
            {
                // для alter и delete
                if (node.SelectSingleNode("as:Object/as:DatabaseID", xmlns) != null)
                    node.SelectSingleNode("as:Object/as:DatabaseID", xmlns).InnerText = name;
                // для команды create
                if (node.SelectSingleNode("as:ParentObject/as:DatabaseID", xmlns) != null)
                    node.SelectSingleNode("as:ParentObject/as:DatabaseID", xmlns).InnerText = name;
            }
        }

        private void InitializeTreeView()
        {
            this.TransparencyKey = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));

            treeViewPatchObj.ImageList = new ImageList();
            treeViewPatchObj.ImageList.Images.Add("cube", Krista.FM.Client.OLAPAdmin.Properties.Resources.cube);
            treeViewPatchObj.ImageList.Images.Add("dimension", Krista.FM.Client.OLAPAdmin.Properties.Resources.dimension);
            treeViewPatchObj.ImageList.Images.Add("dsv", Krista.FM.Client.OLAPAdmin.Properties.Resources.dsv);
            treeViewPatchObj.ImageList.Images.Add("DB", Krista.FM.Client.OLAPAdmin.Properties.Resources.DB);
            treeViewPatchObj.ImageList.Images.Add("Inserted", Krista.FM.Client.OLAPAdmin.Properties.Resources.Inserted);
            treeViewPatchObj.ImageList.Images.Add("Delete", Krista.FM.Client.OLAPAdmin.Properties.Resources.Delete);
            treeViewPatchObj.ImageList.Images.Add("Edited", Krista.FM.Client.OLAPAdmin.Properties.Resources.Edited);
        }

        private void CollectTree()
        {
            treeViewPatchObj.Nodes.Clear();

            foreach (BaseAMOObject obj in PatchManager.Analysis().Values)
            {
                if (obj is AMOCube)
                {
                    SMOCube smoCube = new SMOCube((AMOCube)obj);
                    CubeForScript cube = new CubeForScript((AMOCube)obj, smoCube);
                    AddStateImage(obj, cube);
                    cube.LeftImages.Add(treeViewPatchObj.ImageList.Images["cube"]);
                    //cube.SelectedImageKey = "cube";

                    treeViewPatchObj.Nodes.Add(cube);
                }
                else if (obj is AMODimension)
                {
                    SMODimension smoDinension = new SMODimension((AMODimension)obj);
                    DimensionObjectForScript dimension = new DimensionObjectForScript((AMODimension)obj, smoDinension);
                    AddStateImage(obj, dimension);
                    dimension.LeftImages.Add(treeViewPatchObj.ImageList.Images["dimension"]);
                    //dimension.SelectedImageKey = "dimension";

                    treeViewPatchObj.Nodes.Add(dimension);
                }
                else if (obj is AMODSV)
                {
                    SMODSV smoDSV = new SMODSV((AMODSV)obj);
                    DSVObjectForScript dsv = new DSVObjectForScript((AMODSV)obj, smoDSV);
                    AddStateImage(obj, dsv);
                    dsv.LeftImages.Add(treeViewPatchObj.ImageList.Images["dsv"]);
                    //dsv.SelectedImageKey = "dsv";

                    treeViewPatchObj.Nodes.Add(dsv);
                }
                else if (obj is AMODatabase)
                {
                    SMODatabase smoDatabase = new SMODatabase((AMODatabase)obj);
                    DatabaseObjectForScript database = new DatabaseObjectForScript((AMODatabase)obj, smoDatabase);
                    AddStateImage(obj, database);
                    database.LeftImages.Add(treeViewPatchObj.ImageList.Images["DB"]);
                    //database.SelectedImageKey = "DB";

                    treeViewPatchObj.Nodes.Add(database);
                }
            }

            if (treeViewPatchObj.Nodes.Count != 0)
                treeViewPatchObj.Nodes[0].Selected = true;
        }

        private void UpdateTree()
        {
            foreach (UltraTreeNode node in treeViewPatchObj.Nodes)
            {
                if (node is CubeForScript)
                    ((CubeForScript) node).ControlOblect.Database = databaseName;
                if (node is DimensionObjectForScript)
                    ((DimensionObjectForScript)node).ControlOblect.Database = databaseName;
                if (node is DSVObjectForScript)
                    ((DSVObjectForScript)node).ControlOblect.Database = databaseName;
                if (node is DatabaseObjectForScript)
                    ((DatabaseObjectForScript)node).ControlOblect.Database = databaseName;
            }
        }

        private void AddStateImage(IBaseObjForScript obj, UltraTreeNode node)
        {
            switch (obj.Method)
            {
                case "Alter":
                    node.LeftImages.Add(treeViewPatchObj.ImageList.Images["Edited"]);
                    break;;
                case "Delete":
                    node.LeftImages.Add(treeViewPatchObj.ImageList.Images["Delete"]);
                    break;
                case "Create":
                    node.LeftImages.Add(treeViewPatchObj.ImageList.Images["Inserted"]);
                    break;
            }
        }
        
        public PatchManager PatchManager
        {
            get
            {
                if (patchManager != null)
                    return patchManager;
                InitializePatchManager();
                return patchManager;
            }
            set { patchManager = value; }
        }

        private static bool  CheckFileName(string name)
        {
            if (!File.Exists(name) || new FileInfo(name).Extension != ".xmla")
                throw new Exception("Некорректное имя файла скрипта");

            return true;
        }

        private void treeViewPatchObj_AfterSelect(object sender, Infragistics.Win.UltraWinTree.SelectEventArgs e)
        {
            propertyGrid.SelectedObject = ((BaseTreeNode)e.NewSelections[0]).SmoObject;
        }

        private void filepath_TextChanged(object sender, EventArgs e)
        {
            CheckAllowApplay();
        }

        void databasesBox_SelectedValueChanged(object sender, EventArgs e)
        {
            databaseName = databasesBox.SelectedItem.ToString();
            if (!String.IsNullOrEmpty(scriptDoc.InnerXml))
            {
                UpdateDatabaseName(databaseName, scriptDoc);
                UpdateTree();
            }
            CheckAllowApplay();
        }

        /// <summary>
        /// Проверка на возможность применения скрипта
        /// </summary>
        private void CheckAllowApplay()
        {
            if (String.IsNullOrEmpty(databasesBox.Text) || String.IsNullOrEmpty(filepath.Text))
                buttonApplayScript.Enabled = false;
            else
                buttonApplayScript.Enabled = true;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btSaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "xml files (*.xml)|*.xml";
            fileDialog.FilterIndex = 2;
            fileDialog.RestoreDirectory = true;
            fileDialog.FileName = "scriptLog";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Operation op = new Operation();
                try
                {
                    op.Text = "Сохранение логов";
                    op.StartOperation();

                    ultraTreeExLog.SaveAsXml(fileDialog.FileName);
                }
                finally
                {
                    op.StopOperation();
                    op.ReleaseThread();
                }
            }
        }
    }
}