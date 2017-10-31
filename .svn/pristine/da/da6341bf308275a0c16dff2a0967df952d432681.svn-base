using System;
using System.Text;


using Krista.FM.Common;
using Krista.FM.Server.ProcessorLibrary;
using System.Data;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// Предназначен для генерации тех объектов многомерной базы, которых нет в схеме сервера.
    /// Например, измерения типа "Группировка", отличающиеся названием и фильтром на исходной таблице.
    /// Генерация поддерживается только для MS Analysis 2000.
    /// Хранилище должно быть на Oracle.
    /// </summary>    
    public sealed class OlapDatabaseGenerator2000 : DisposableObject, IOlapDatabaseGenerator
    {
        private readonly IScheme scheme;

        private readonly DSO.ServerClass server = new DSO.ServerClass();
        private DSO.Database database;
        private DSO.DataSource dataSource;

        private readonly DataSet dataSet = new DataSet();
        private DataTable dataTableObjGroup;

        private static readonly string connectionStringOracle = "Provider=OraOLEDB.Oracle.1;Password=DV;Persist Security Info=True;User ID=DV;Data Source=Develop";
        //private static readonly string connectionStringMSSQL = @"Provider=SQLNCLI.1; Data Source=MRRSERV\SQLSERVER2005; User ID=UlanUde; Initial Catalog=UlanUde";

        private static readonly string selectSQL_FX_FX_OBJGROUP =
            "select ID, CODE, NAME from FX_FX_OBJGROUP WHERE ID > 0 ORDER BY NAME";

        private static readonly string mainTableName = "\"DV\".\"D_OK_GROUP\"";        
        private static readonly string filter = "DV.D_OK_GROUPSET.REFFX = {0} OR DV.D_OK_GROUPSET.REFFX = -1";
        //private static readonly string filter = "DV.DV_OK_GROUP.OBJGROUP = {0} OR DV.DV_OK_GROUP.OBJGROUP = -1";
        private static readonly string joinClause = "\"DV\".\"D_OK_GROUP\".\"ID\" = \"DV\".\"D_OK_GROUPSET\".\"REFOK\"";

        public OlapDatabaseGenerator2000(IScheme scheme)
        {
            this.scheme = scheme;
        }

        protected override void Dispose(bool disposing)
        {            
            if (disposing)
            {
                dataSet.Dispose();
            }            
            base.Dispose(disposing);
        }

        /// <summary>
        /// Создает многомерную базу данных. Если база с таким именем уже есть - она предваритиельно уничтожается.
        /// </summary>
        /// <param name="databaseName">Имя многомерной базы.</param>
        /// <param name="dataSourceName">То, что будет записано в строке DataSource источника данных.</param>
        private void CreateDatabase(string databaseName, string dataSourceName)
        {
            //Если база с таким именем уже существует - уничтожаем ее.
            if (server.MDStores.Find(databaseName))
            {                
                database = (DSO.Database)server.MDStores.Item(databaseName);
                server.RemoveChild(server.MDStores, (DSO.ICommon)database);
                server.Update();
            }

            //Добавляем новую многомерную базу.
            database = (DSO.Database)server.MDStores.AddNew(databaseName, DSO.SubClassTypes.sbclsRelational);            
            database.Update();

            //Добавляем новый источник данных с именем DV.
            dataSource = (DSO.DataSource)database.DataSources.AddNew("DV", DSO.SubClassTypes.sbclsRelational);
            dataSource.ConnectionString = connectionStringOracle.Replace("Develop", dataSourceName);
            dataSource.Update();
        }        

        /// <summary>
        /// Выбираем данные из базы в таблицу датасета.
        /// </summary>
        /// <param name="warehouseDatabase">Хранилище.</param>
        /// <param name="dataTable">Таблица датасета, которую надо заполнить.</param>
        /// <param name="selectSQL">Запрос, выбирающий данные в таблицу.</param>
        private static void FillDataTable(Database warehouseDatabase, DataTable dataTable, string selectSQL)
        {
            dataTable.Clear();
            DataUpdater dataUpdater = (DataUpdater)warehouseDatabase.GetDataUpdater(selectSQL);
            try
            {
                dataUpdater.Fill(ref dataTable);
            }
            finally
            {   
                dataUpdater.Dispose();
            }
        }

        /// <summary>
        /// Создает и заполняет данными таблицы датасета.
        /// </summary>
        private void FillDataTables()
        {
            Database kristaDB = (Database)scheme.SchemeDWH.DB;
            try
            {
                if (null == dataTableObjGroup)
                {
                    dataTableObjGroup = dataSet.Tables.Add("OBJGROUP");
                }                
                FillDataTable(kristaDB, dataTableObjGroup, selectSQL_FX_FX_OBJGROUP);
            }
            finally
            {
                kristaDB.Dispose();
            }                        
        }

        /// <summary>
        /// Конвертирует наши типы данных в типы данных многомерной базы.
        /// </summary>
        /// <param name="type">Тип, который надо конвертировать.</param>
        /// <returns>Тип данных многомернуй базы.</returns>
        private static short GetOlapDataType(DataAttributeTypes type)
        {
            switch (type)
            {
                case DataAttributeTypes.dtBoolean:
                case DataAttributeTypes.dtInteger:
                    return 131;//Numeric
                case DataAttributeTypes.dtDouble:
                    return 5;
                case DataAttributeTypes.dtChar:
                case DataAttributeTypes.dtString:
                    return 129;
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    return 133;
                default:
                    throw new Exception("Неизвестный тип данных.");
            }
        }

        /// <summary>
        /// Заменяет недопустимые символы в имени на знак подчеркивания ("_") и обрезает имя до 50 символов.
        /// </summary>
        /// <param name="name">Исходное имя, которое надо привести в корректный вид.</param>
        /// <param name="warningString">Строка, в которую пишется сообщение о том, что имя было обрезано.</param>
        /// <returns>Имя длинной не более 50 символов и содержащее только корректные символы.</returns>
        private static string CheckDimensionName(string name, out string warningString)
        {
            int maxNameLenght = 50;
            warningString = string.Empty;
            if (name.Length > maxNameLenght)
            {
                name = name.Substring(0, maxNameLenght);
                warningString = string.Format("имя сокращено до \"{0}\"", name);                
            }
            char[] illegalCharacters = new char[] { ',', ';', ':', '(', ')', '-', '+', '=', '?', '"' };
            for (int i = 0; i < illegalCharacters.Length; i++)
            {
                if (name.IndexOf(illegalCharacters[i]) > -1)
                {
                    name = name.Replace(illegalCharacters[i], '_');
                }                
            }            
            return name;
        }

        /// <summary>
        /// Проходит по записям в таблице FX_FX_OBJGROUP и генерирует для каждой записи измерение.
        /// </summary>
        /// <param name="logStringBuilder">Для каждой записи в этот лог пишутся сообщения об удачной генерации или об ошибке.</param>
        /// <param name="totalCount">Общее число записей в таблице FX_FX_OBJGROUP.</param>
        /// <param name="errorCount">Число измерений, при генерации которых произошла ошибка.</param>
        private void GenerateDimensions(ref StringBuilder logStringBuilder, out int totalCount, out int errorCount)
        {
            string name = string.Empty;

            //string filter = "DV.DV_OK_GROUP.OBJGROUP = {0} OR DV.DV_OK_GROUP.OBJGROUP = -1";            
            totalCount = dataTableObjGroup.Rows.Count;
            errorCount = 0;
            foreach (DataRow dataRow in dataTableObjGroup.Rows)
            {                
                try
                {
                    name = (string)dataRow["Name"];
                    
                    //Удаляем имя блока ("СТАТ_")
                    int splitterIndex = name.IndexOf("_");
                    if (splitterIndex > 0)
                    {
                        name = name.Substring(splitterIndex + 1);
                    }

                    name = "Группировки." + name;
                    string warningString;
                    GenerateDimension(CheckDimensionName(name, out warningString), string.Format(filter, dataRow["ID"]));
                    if (string.IsNullOrEmpty(warningString))                    
                        logStringBuilder.AppendLine(string.Format("{0}: ок.", name));
                    else
                        logStringBuilder.AppendLine(string.Format("{0}: предупреждение - {1}.", name, warningString));
                }
                catch (Exception e)
                {
                    errorCount++;
                    logStringBuilder.AppendLine(string.Format("{0}: ошибка - {1}.", name, e.Message));
                }
            }
        }

        /// <summary>
        /// Генерирует измерения с заданным именем и с заданным фильтром на исходной таблице.
        /// </summary>
        /// <param name="name">Имя измерения.</param>
        /// <param name="sourceTableFilter">Фильтр на исходную таблицу.</param>
        private void GenerateDimension(string name, string sourceTableFilter)
        {
            DSO.Dimension dim;
            dim = (DSO.Dimension)database.Dimensions.AddNew(name, DSO.SubClassTypes.sbclsRegular);
            dim.DataSource = dataSource;
            dim.Description = "Перечень группировок.";
            //dim.FromClause = mainTableName;
            dim.JoinClause = joinClause;
            dim.SourceTableFilter = sourceTableFilter;
            dim.CustomProperties.Add("d.OK.Group", "FullName", VBA.VbVarType.vbString);
            dim.CustomProperties.Add("Группировки", "Caption", VBA.VbVarType.vbString);
            dim.CustomProperties.Add("Перечень группировок.", "Description", VBA.VbVarType.vbString);
            dim.CustomProperties.Add(string.Empty, "Version", VBA.VbVarType.vbString);

            DSO.Level level = (DSO.Level)dim.Levels.AddNew("Группировка", DSO.SubClassTypes.sbclsRegular);
            level.MemberKeyColumn = mainTableName + ".\"ID\"";
            level.MemberNameColumn = mainTableName  + ".\"NAME\"";
            level.EstimatedSize = 1;

            short dataType = GetOlapDataType(DataAttributeTypes.dtInteger);
            short dataSize = 10;
            DSO.MemberProperty memberProperty = (DSO.MemberProperty)level.MemberProperties.AddNew("PKID", DSO.SubClassTypes.sbclsRegular);
            memberProperty.SourceColumn = mainTableName + ".\"ID\"";
            memberProperty.set_ColumnType(ref dataType);            
            memberProperty.set_ColumnSize(ref dataSize);

            memberProperty = (DSO.MemberProperty)level.MemberProperties.AddNew("Код", DSO.SubClassTypes.sbclsRegular);
            memberProperty.SourceColumn = mainTableName + ".\"CODE\"";
            memberProperty.set_ColumnType(ref dataType);
            memberProperty.set_ColumnSize(ref dataSize);

            level.Ordering = DSO.OrderTypes.orderMemberProperty;
            level.OrderingMemberProperty = "Код";
            
            dim.Update();
        }

        #region IOlapDatabaseGenerator Members        
        
        /// <summary>
        /// Создает новую многомерную базу с заданным именем на заданном сервере и
        /// генерирует на этой базе специальные измерения, предназначенные для отдела MRR.
        /// </summary>
        /// <param name="serverName">Имя сервера многомерных баз.</param>
        /// <param name="databaseName">Имя многомерной базы.</param>
        /// <param name="dataSourceName">Имя источника данных.</param>
        /// <param name="logString">Лог, содержащий информацию о результатах генерации каждого измерения.</param>
        /// <param name="totalCount">Общее число измерений, которое необходимо было сгенерировать.</param>
        /// <param name="errorCount">Число измерений, при генерации которых произошла ошибка.</param>
        public void GenerateMRRDimensions(string serverName, string databaseName, string dataSourceName,
            out string logString, out int totalCount, out int errorCount)
        {            
            totalCount = 0;
            errorCount = 0;
            StringBuilder logStringBuilder = new StringBuilder();            
            try
            {
                server.Connect(serverName);
                CreateDatabase(databaseName, dataSourceName);

                FillDataTables();
                GenerateDimensions(ref logStringBuilder, out totalCount, out errorCount);

                logString = logStringBuilder.ToString();
            }
            catch (Exception e)
            {
                logStringBuilder.AppendLine(e.Message);
                logString = logStringBuilder.ToString();                
            }
        }

        #endregion
    }
}
