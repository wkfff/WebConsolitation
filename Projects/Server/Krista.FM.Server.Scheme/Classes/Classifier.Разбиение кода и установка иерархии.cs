using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{

    /// <summary>
    /// Базовый Ромин класс для классификаторов данных и сопоставимых для бития
    /// </summary>
    partial class Classifier
    {
        /// <summary>
        /// Возвращает значение лукап-атрибута (работает быстрее и рекомендуется для использования)
        /// </summary>
        /// <param name="db">Объект для боступа к базе данных</param>
        /// <param name="rowID">ID записи</param>
        /// <param name="attributeName">Наименование атрибута значение которого нужно получить</param>
        /// <returns>Значение лукап-атрибута</returns>
        public object GetLookupAttribute(IDatabase db, int rowID, string attributeName)
        {

            if (DataAttributeCollection.GetAttributeByKeyName(Attributes, attributeName, attributeName) == null)
                throw new Exception(String.Format("Атрибут {0} не найден.", attributeName));

            return db.ExecQuery(String.Format("select {0} from {1} where ID = ?", attributeName, this.FullDBName),
                QueryResultTypes.Scalar,
                db.CreateParameter("ID", rowID));
        }

        /// <summary>
        /// Возвращает значение лукап-атрибута
        /// </summary>
        /// <param name="rowID">ID записи</param>
        /// <param name="attributeName">Наименование атрибута значение которого нужно получить</param>
        /// <returns>Значение лукап-атрибута</returns>
        public object GetLookupAttribute(int rowID, string attributeName)
        {
            if (DataAttributeCollection.GetAttributeByKeyName(Attributes, attributeName, attributeName) == null)
                throw new Exception(String.Format("Атрибут {0} не найден.", attributeName));

            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                return db.ExecQuery(String.Format("select {0} from {1} where ID = ?", attributeName, this.FullDBName), 
                    QueryResultTypes.Scalar,
                    db.CreateParameter("ID", rowID));
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Разбивает код классификатора на Code1, Code2 и т.д.
		/// Устанавливает древовидную иерархию
        /// </summary>
        /// <param name="SourceID">Фильтр для классификатора по источнику данных</param>
        /// <returns></returns>
        public int DivideAndFormHierarchy(int SourceID, bool setFullHierarchy)
        {
            DataSet ds = null;
            return DivideAndFormHierarchy(SourceID, -1, ref ds, null, setFullHierarchy);
        }

        /// <summary>
        /// Разбивает код классификатора на Code1, Code2 и т.д.
        /// Устанавливает древовидную иерархию
        /// </summary>
        /// <param name="SourceID">Фильтр для классификатора по источнику данных</param>
        /// <param name="clsTable">Таблица с данными классификатора. Если null - данные берутся из базы</param>
        /// <returns></returns>
        public int DivideAndFormHierarchy(int SourceID, ref DataSet clsDataSet)
        {
            //DataTable dt = null;
            return DivideAndFormHierarchy(SourceID, -1, ref clsDataSet, null, true);
        }

        /// <summary>
        /// Разбивает код классификатора на Code1, Code2 и т.д.
        /// Устанавливает древовидную иерархию
        /// </summary>
        /// <param name="SourceID">Фильтр для классификатора по источнику данных</param>
        /// <param name="db">IDatabase</param>
        /// <returns></returns>
        public int DivideAndFormHierarchy(int SourceID, IDatabase db)
        {
            DataSet ds = null;
            return DivideAndFormHierarchy(SourceID, -1, ref ds, null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SourceID">Фильтр для классификатора по источнику данных</param>
        /// <param name="dataPumpTable">ID закачки, для протоколирования</param>
        /// <param name="clsTable">Таблица с данными классификатора. Если null - данные берутся из базы</param>
        /// <returns></returns>
        public int DivideAndFormHierarchy(int SourceID, int dataPumpID, ref DataSet clsDataSet)
        {
            return DivideAndFormHierarchy(SourceID, dataPumpID, ref clsDataSet, null, true);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // для переноса в отдельный объект потребуется
        // 1 Параметр Divide
        // 2 Названия полей, то на которое сслылаются и поле со ссылкой
        // 3 Тип иерархии

        ///////////////////////////////////////////////////////////////////////////////

        private bool CopyDataTable(DataTable sourceTable, ref DataTable destinationTable, string sortOrder, string filter)
        {
            // если исходная таблица не задана - создать копию невозможно
            if (sourceTable == null)
                return false;
            // если не задана результриующая таблица - делаем клон исходной
            if (destinationTable == null)
            {
                destinationTable = sourceTable.Clone();
            }
            try
            {
                DataRow[] rows = null;
                if (!String.IsNullOrEmpty(sortOrder))
                {
                    rows = sourceTable.Select(filter, sortOrder);
                }
                else
                {
                    rows = sourceTable.Select(filter);
                }
                destinationTable.BeginLoadData();
                foreach (DataRow row in rows)
                {
                    destinationTable.Rows.Add(row);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                destinationTable.EndLoadData();
            }
        }


        /// <summary>
        /// Разбивает код классификатора на Code1, Code2 и т.д.
        /// </summary>
        /// <param name="SourceID">Фильтр для классификатора по источнику данных</param>
        /// <returns></returns>
        public int DivideClassifierCode(int SourceID)
        {
            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            try
            {
                string codeClmnName;
                string Divide = GetDivide(out codeClmnName);

                SessionContext.SetSystemContext();

                int RecCount = 0;
                
                // Проверяем, есть ли у классификатора разбиение по маске (наличие не пустого Divide)
               
                // Если разбиения по маске нету, то выходим, иначе разбиваем
                if (Divide == string.Empty) return RecCount;

                string[] splittedDivide = Divide.Split('.');

                StringBuilder strDivide = new StringBuilder();

                // Получаем коллекцию наименований уровней ращепления
                Dictionary<int, DesintegrationLevel> DesintegrationLevels = new Dictionary<int, DesintegrationLevel>();
                for (int i = 0; i <= splittedDivide.Length - 1; i++)
                {
                    DesintegrationLevel dl = new DesintegrationLevel(i + 1, splittedDivide[i]);
                    DesintegrationLevels.Add(i, dl);
                    strDivide.Append(System.Convert.ToString(DesintegrationLevels[i].Divider / 10) + '.');
                }

                // получаем данные из классификатора
                IDataUpdater updater = null;
                DataTable dt = new DataTable();
                updater = GetClsDataUpdater(SourceID, null, codeClmnName, string.Empty, DesintegrationLevels);
                updater.Fill(ref dt);

                DataColumn clmn = new DataColumn("RowValidate", typeof(int));
                if (!dt.Columns.Contains("RowValidate"))
                    dt.Columns.Add(clmn);

                // ращепляем код классификатора
                foreach (DataRow row in dt.Rows)
                {
                    row.BeginEdit();
                    DivideCode(row, strDivide.ToString(), row[codeClmnName].ToString(), DesintegrationLevels);
                    row.EndEdit();
                }
               
                // сохраняем изменения

                DataTable changesTable = dt.GetChanges();
                if (changesTable != null)
                    RecCount = updater.Update(ref changesTable);

               
                return RecCount;
            }
            finally
            {
                LogicalCallContextData.SetContext(callerContext);
            }
        }

        /// <summary>
        /// Метод получения строки расщепления, поскольку расщепление и установку иерархии делаем в системном контексте
        /// строку пытаемся получить находять в текущем контексте вызова
        /// </summary>
        /// <param name="codeClmnName"></param>
        /// <returns></returns>
        private string GetDivide(out string codeClmnName)
        {
            // получаем строку ращепления и код, который будем ращеплять
            // получаем его в текущем контексте вызова, чтоб работать с текущей версией классификатора
            string Divide = string.Empty;
            codeClmnName = string.Empty;

            foreach (IDataAttribute item in this.Attributes.Values)
            {
                DataAttribute attr = (DataAttribute)item;
                Divide = attr.Divide;
                if (Divide != string.Empty)
                {
                    codeClmnName = attr.Name;
                    break;
                }
            }
            return Divide;
        }

        /// <summary>
        /// Разбивает код классификатора на Code1, Code2 и т.д.
		/// Устанавливает древовидную иерархию
        /// </summary>
        /// <param name="SourceID">Фильтр для классификатора по источнику данных</param>
        /// <param name="clsTable">Таблица с данными классификатора. Если null - данные берутся из базы</param>
        /// <param name="db">Если данные берутся из базы, то для доступа к данным использутся этот параметр.
        /// Если он не равен null.</param>
        /// <returns></returns>
        private int DivideAndFormHierarchy(int SourceID, int dataPumpID, ref DataSet clsDataSet, IDatabase db, bool setFullHierarchy)
        {
            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            IClassifiersProtocol protocol = (IClassifiersProtocol)SchemeClass.Instance.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
            try
            {
                string codeClmnName = string.Empty;
                string Divide = GetDivide(out codeClmnName);
                
                SessionContext.SetSystemContext();

                bool formHierarchy = true;
                // Количество обработанных записей
                int RecCount = 0;

                // Если разбиения по маске нету, то выходим, иначе разбиваем и строим иерархию
                if (Divide == string.Empty)
                {
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceWarning, this.OlapName,
                        dataPumpID, SourceID, (int)this.ClassType, "Операция установки иерархии. Маска ращепления кода не найдена");
                    protocol.Dispose();
                    return 0;
                }
                // определяем, нужно ли выполнять установку иерархии по другому алгоритму
                bool isMesOtchAlgoritm = false;

                if (OtherHierarchyMethod(ObjectKey))
                {
                    // измененная установка иерархии действует с 2007 года
                    // или когда год 0 или -1 (параметра года нет)
                    if (GetDataSourceYear(SourceID) > 2006 || GetDataSourceYear(SourceID) <= 0)
                        isMesOtchAlgoritm = true;
                }
                // Поиск колонки для построения иерархии
                string RefParentClmnName = string.Empty;
                string ParentClmnName = string.Empty;
                // Проверяем тип иерархии
                // Получаем названия колонок для построения иерархии
                if (this.Levels.HierarchyType == HierarchyType.Regular)
                {
                    // Это временный ваниант. 
                    // В данном случае не нужно устанавливать иерархию,
                    // т.е. ее просто нет! Но расшепление кода можно и сделать, хотя зачем?
                    // будем просто ращеплять код
                    formHierarchy = false;
                }
                else
                {
                    // Если "деревянная", то ищем название колонки в уровнях иерархии
                    string levelWithTemplateName = string.Empty;
                    foreach (IDimensionLevel item in Levels.Values)
                    {
                        if (item.LevelType != LevelTypes.All)
                        {
                            levelWithTemplateName = item.ObjectKey;
                            break;
                        }
                    }
                    RefParentClmnName = Levels[levelWithTemplateName].ParentKey.Name;
                    ParentClmnName = Levels[levelWithTemplateName].MemberKey.Name;
                }

                // если нету названий полей, по которым строится иерархия, то не будем ее строить, только ращепление
                if (RefParentClmnName == string.Empty || ParentClmnName == string.Empty)
                    formHierarchy = false;

                string[] splittedDivide = Divide.Split('.');

                StringBuilder strDivide = new StringBuilder();

                // Получаем коллекцию наименований уровней ращепления
                Dictionary<int, DesintegrationLevel> DesintegrationLevels = new Dictionary<int, DesintegrationLevel>();
                for (int i = 0; i <= splittedDivide.Length - 1; i++)
                {
                    DesintegrationLevel dl = new DesintegrationLevel(i + 1, splittedDivide[i]);
                    DesintegrationLevels.Add(i, dl);

                    strDivide.Append(System.Convert.ToString(DesintegrationLevels[i].Divider / 10) + '.');
                }

                IDataUpdater updater = null;
                
                DataRow[] rows = new DataRow[0];
                bool callFromThePump = true;
                
                if (clsDataSet == null)
                {
                    clsDataSet = new DataSet();
                    clsDataSet.Tables.Add();
                    callFromThePump = false;
                }

                try
                {
                    if (!callFromThePump)
                    {
                        updater = GetClsDataUpdater(SourceID, db, codeClmnName, RefParentClmnName, DesintegrationLevels);
                        DataTable table = clsDataSet.Tables[0];
                        updater.Fill(ref table);
                        rows = clsDataSet.Tables[0].Select(string.Empty, string.Empty);
                    }
                    else
                    {
                        // копируем данные в порядке возрастания кода
                        //dt = clsTable;
                        string sortOrder = string.Format("{0} Asc", codeClmnName);
                        rows = clsDataSet.Tables[0].Select(string.Empty, sortOrder);
                        //callFromThePump = true;
                    }

                    if (clsDataSet.Tables[0].Rows.Count < 1)
                        return 0;

                    clsDataSet.Tables[0].BeginLoadData();

                    DataColumn clmn = new DataColumn("RowLevel", typeof(int));
                    if (!clsDataSet.Tables[0].Columns.Contains("RowLevel"))
                        clsDataSet.Tables[0].Columns.Add(clmn);

                    clmn = new DataColumn("RowValidate", typeof(int));
                    if (!clsDataSet.Tables[0].Columns.Contains("RowValidate"))
                        clsDataSet.Tables[0].Columns.Add(clmn);

                    // индексы колонок
                    int parentClmnInd = clsDataSet.Tables[0].Columns.IndexOf(ParentClmnName);
                    int refParentClmnInd = clsDataSet.Tables[0].Columns.IndexOf(RefParentClmnName);
                    int codeClmnInd = clsDataSet.Tables[0].Columns.IndexOf(codeClmnName);
                    int rowLevelClmnInd = clsDataSet.Tables[0].Columns.IndexOf("RowLevel");

                    // Бежим по всем строкам таблицы, и разбиваем код по правилу ращепления кода
                    foreach (DataRow row in rows)
                    {
                        row.BeginEdit();
                        string str = row[codeClmnInd].ToString();
                        DivideCode(row, strDivide.ToString(), str, DesintegrationLevels);
                        row.EndEdit();
                    }
                    
                    // почистим иерархию
                    if (formHierarchy && setFullHierarchy)
                        ClearHierarchy(rows, refParentClmnInd);

                    // строим иерархию
                    
                    if (formHierarchy)
                        SetHierarchy(rows, refParentClmnInd, parentClmnInd, rowLevelClmnInd, DesintegrationLevels, setFullHierarchy, isMesOtchAlgoritm);
                    // для некоторых классификаторов введем другой алгоритм установки иерархии

                    clsDataSet.Tables[0].EndLoadData();

                    if (clsDataSet.Tables[0].Columns.Contains("RowLevel"))
                        clsDataSet.Tables[0].Columns.Remove("RowLevel");
                    if (clsDataSet.Tables[0].Columns.Contains("RowValidate"))
                        clsDataSet.Tables[0].Columns.Remove("RowValidate");

                    // Расходы.Сопоставимый и Расходы.Сопоставимый Планирование
                    if (ObjectKey == "0a626485-8481-4058-aa0f-a917df395f3c" 
                        || ObjectKey == "abf83a0a-0545-4a85-88d0-c43f470cb11d")
                        FillB_R_BRIDGE(clsDataSet.Tables[0], DesintegrationLevels);

                    // Записываем изменения
                    if (!callFromThePump)
                    {
                        //protocol.WriteEventIntoPreviewDataProtocol(ClassifiersEventKind.dpeInformation, this.FullName, -1, SourceID, "Сохранение изменений");
                        // получаем все изменения
                        DataTable changesTable = clsDataSet.Tables[0].GetChanges();
                        if (changesTable != null)
                            RecCount = updater.Update(ref changesTable);
                        //protocol.WriteEventIntoPreviewDataProtocol(ClassifiersEventKind.dpeInformation, this.FullName, -1, SourceID, "Изменения успешно сохранены");
                    }
                    else
                    {
                        RecCount = clsDataSet.Tables[0].GetChanges().Rows.Count;
                    }
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation, this.OlapName, dataPumpID,
                        SourceID, (int)this.ClassType, string.Format("Операция установки иерархии. Обработано записей: {0}", RecCount));
                }
                catch (Exception e)
                {
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceFinishHierarchySetWithError, this.OlapName, dataPumpID,
                        SourceID, (int)this.ClassType, string.Format("При установке иерархии произошла ошибка {0}{1}", Environment.NewLine, e.Message));

                    Trace.WriteLine(string.Format("При установке иерархии классификатора '{0}' произошла ошибка", this.FullName));
                    throw new Exception(e.Message, e);
                }
                finally
                {
                    if (!callFromThePump)
                        clsDataSet = null;
                }
                return RecCount;
            }
            finally
            {
                protocol.Dispose();
                LogicalCallContextData.SetContext(callerContext);
            }
        }

        private const string d_KD_MonthRep = "e151c4f4-9fbd-4234-bb55-c6427a995963";
        private const string d_KD_ASBudget = "f9747df9-e29b-4293-83c8-801b37461230";
        private const string d_KD_FOProj = "5221400b-e97f-4693-8d5e-0d1744a28a72";
        private const string d_KD_FOYR = "8ce19831-1c98-468a-ba8c-125f2123a719";
        private const string d_KD_FKMR = "66450b83-d2fa-465c-a35d-009704607c7b";
        private const string d_KD_Etalon = "8c51f8ce-62cd-450f-8d6a-afeb998f3be9";
        private const string d_KD_Analysis = "2553274b-4cee-4d20-a9a6-eef173465d8b";
        private const string d_KD_PlanIncomes = "a6e33772-325a-4932-a0aa-7ce82f0b3921";
        private const string b_KD_Bridge = "5cd4f631-6276-4a9f-b466-980282500b50";
        //
        private const int fifthLevelIndex = 4;
        private const int sixthLevelIndex = 5;

        /// <summary>
        /// определяет классификаторы, для которых возможен измененный метод установки иерархии
        /// </summary>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        private static bool OtherHierarchyMethod(string objectKey)
        {
            if (objectKey == d_KD_MonthRep ||
                objectKey == d_KD_ASBudget ||
                objectKey == d_KD_FOProj ||
                objectKey == d_KD_FOYR ||
                objectKey == d_KD_FKMR ||
                objectKey == d_KD_Etalon ||
                objectKey == d_KD_Analysis ||
                objectKey == b_KD_Bridge ||
                objectKey == d_KD_PlanIncomes)
                return true;
            return false;
        }

        /// <summary>
        /// получение года источника по ID
        /// </summary>
        /// <param name="dataSourceID"></param>
        /// <returns></returns>
        private static int GetDataSourceYear(int dataSourceID)
        {
            if (dataSourceID == -1)
                return -1;
            if (!SchemeClass.Instance.DataSourceManager.DataSources.Contains(dataSourceID))
                return -1;
            return SchemeClass.Instance.DataSourceManager.DataSources[dataSourceID].Year;
        }

        #region Заполнение классификаторов Расходы.Сопоставимый и Расходы.Сопоставимый Планирование

        /// <summary>
        /// Заполнение полей Расходы.Сопоставимый и Расходы.Сопоставимый Планирование
        /// </summary>
        /// <param name="table"></param>
        /// <param name="levels"></param>
        private void FillB_R_BRIDGE(DataTable table, Dictionary<int, DesintegrationLevel> levels)
        {
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    row.BeginEdit();
                    FillCodes(row, levels);
                    // дополнительная обработка поля Код_отчет(RepCode)
                    FillB_R_BRIDGECodeRep(row, levels);
                    row.EndEdit();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("При заполнении строковых полей классификатора '{0}' произошла ошибка: {1}", this.FullName, e));
            }
        }

        /// <summary>
        /// Заполнение поля код_отчет классификатора Расходы.Сопоставимый Планирование
        /// </summary>
        /// <param name="row"></param>
        /// <param name="desintegrationLevels"></param>
        private void FillB_R_BRIDGECodeRep(DataRow row, Dictionary<int, DesintegrationLevel> desintegrationLevels)
        {
            try
            {
                if (row.Table.Columns.Contains("RepCode"))
                    row["RepCode"] = String.Format("{0}{1}{2}{3}{4}{5}",
                                                   AddDivider(desintegrationLevels[1].Divider.ToString(),
                                                              row["CODE2"].ToString()),
                                                   AddDivider(desintegrationLevels[2].Divider.ToString(),
                                                              row["CODE3"].ToString()),
                                                   AddDivider(desintegrationLevels[3].Divider.ToString(),
                                                              row["CODE4"].ToString()),
                                                   AddDivider(desintegrationLevels[4].Divider.ToString(),
                                                              row["CODE5"].ToString()),
                                                   AddDivider(desintegrationLevels[5].Divider.ToString(),
                                                              row["CODE6"].ToString()),
                                                   AddDivider(desintegrationLevels[6].Divider.ToString(),
                                                              row["CODE7"].ToString()));
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("При заполнении поля код_отчет возникло исключение: {0}", e));
            }
        }

        /// <summary>
        /// Заполнение кодов для Расходы.Сопоставимый и Расходы.Сопоставимый планирование
        /// </summary>
        /// <param name="row"></param>
        /// <param name="desintegrationLevels"></param>
        private void FillCodes(DataRow row, Dictionary<int, DesintegrationLevel> desintegrationLevels)
        {
            row["KVSR"] = row["CODE1"];
            row["FKR"] = String.Format("{0}{1}",
                                       AddDivider(desintegrationLevels[1].Divider.ToString(), row["CODE2"].ToString()),
                                       AddDivider(desintegrationLevels[2].Divider.ToString(), row["CODE3"].ToString()));
            row["Section"] = row["CODE2"];
            row["SubSection"] = row["CODE3"];
            row["KCSR"] = String.Format("{0}{1}{2}",
                                       AddDivider(desintegrationLevels[3].Divider.ToString(), row["CODE4"].ToString()),
                                       AddDivider(desintegrationLevels[4].Divider.ToString(), row["CODE5"].ToString()),
                                       AddDivider(desintegrationLevels[5].Divider.ToString(), row["CODE6"].ToString()));
            row["KVR"] = row["CODE7"];
        }

        /// <summary>
        /// Дополнение кода недостоющими нулями
        /// </summary>
        /// <param name="divider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string AddDivider(string divider, string value)
        {
            StringBuilder builder = new StringBuilder(value);
            for (int i = builder.Length; i < divider.Length - 1; i++)
            {
                builder.Insert(0, 0);
            }

            return builder.ToString();
        }

        #endregion


        private void ClearHierarchy(DataRow[] rows, int RefParentColumnInd)
        {
            foreach (DataRow row in rows)
                row[RefParentColumnInd] = DBNull.Value;
        }

        /// <summary>
        /// Возвращает IDataUpdater для получения данных классификатора
        /// </summary>
        /// <param name="SourceID">ИД источника</param>
        /// <param name="db">Объект ДБ</param>
        /// <param name="codeClmnName">Название поля кода</param>
        /// <param name="RefParentClmnName">Название поля ссылки на родительскую запись</param>
        /// <returns>IDataUpdater</returns>
        private IDataUpdater GetClsDataUpdater(int SourceID, IDatabase _db, string codeClmnName, 
            string refParentClmnName, Dictionary<int, DesintegrationLevel> codeLevels)
        {
            IDataUpdater updater = null;
            string constr = string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= codeLevels.Count - 2; i++)
            {
                if (codeLevels[i].UseInHierarchy)
                    sb.Append(string.Format("order by {0}, ", codeLevels[i].LevelName));
            }
            sb.Append(string.Format("order by {0}", codeLevels[codeLevels.Count - 1].LevelName));
            string orderBy = sb.ToString();

            if (SourceID >= 0)
            {
                // если делится по источникам и устанавливаем иерархию по одному источнику
                constr = string.Format(
                    "SourceID = {0} and (RowType = 0) order by {1}",
                    SourceID, codeClmnName);
            }
            else
            {
                // если устанавливаем иерархию по всему классифмкатору
                constr = string.Format("(RowType = 0) order by {0}", codeClmnName);
            }

            // из всех аттрибутов берем только нужные
            DataAttributeCollection filteredAttr = new DataAttributeCollection(this, State);
            // ID (он же парент)
            IDataAttribute attr = Attributes[DataAttribute.IDColumnName];
            filteredAttr.Add(attr);
            // CODE
            attr = DataAttributeCollection.GetAttributeByKeyName(Attributes, codeClmnName, codeClmnName);
            filteredAttr.Add(attr);
            // RefParent
            if (refParentClmnName != string.Empty)
            {
                attr = DataAttributeCollection.GetAttributeByKeyName(Attributes, refParentClmnName, refParentClmnName);
                filteredAttr.Add(attr);
            }
            // коды разбиения
            for (int i = 0; i < codeLevels.Keys.Count; i ++)
            {
                attr = DataAttributeCollection.GetAttributeByKeyName(Attributes, codeLevels[i].LevelName, codeLevels[i].LevelName);
                filteredAttr.Add(attr);
            }
            if (ObjectKey == "0a626485-8481-4058-aa0f-a917df395f3c" || ObjectKey == "abf83a0a-0545-4a85-88d0-c43f470cb11d")
            {
                if (Attributes.ContainsKey("KVSR"))
                    filteredAttr.Add(Attributes["KVSR"]);
                if (Attributes.ContainsKey("FKR"))
                    filteredAttr.Add(Attributes["FKR"]);
                if (Attributes.ContainsKey("Section"))
                    filteredAttr.Add(Attributes["Section"]);
                if (Attributes.ContainsKey("Subsection"))
                    filteredAttr.Add(Attributes["Subsection"]);
                if (Attributes.ContainsKey("KCSR"))
                    filteredAttr.Add(Attributes["KCSR"]);
                if (Attributes.ContainsKey("KVR"))
                    filteredAttr.Add(Attributes["KVR"]);
                if (Attributes.ContainsKey("RepCode"))
                    filteredAttr.Add(Attributes["RepCode"]);
            }
            // а вот теперь получаем апдейтер только для нужных полей
            if (_db == null)
            {
                IDatabase db = null;
                try
                {
                    db = SchemeClass.Instance.SchemeDWH.DB;
                    updater = db.GetDataUpdater(this.FullDBName, filteredAttr, constr, null, null);
                }
                finally
                {
                    if (db != null)
                        db.Dispose();
                }
            }
            else
            {
                updater = _db.GetDataUpdater(this.FullDBName, filteredAttr, constr, null, null);
            }
            return updater;
        }

		/// <summary>
		///  Получение коллекции значений ращепленного кода
		/// </summary>
		/// <param name="dataRow"></param>
		/// <param name="DesintegrationValues"></param>
		/// <param name="DesintegrationLevels"></param>
        private void SetCodeValues(DataRow dataRow, int[] DesintegrationValues, int[] CodeLevelsIndexes)
		{
            for (int i = 0; i <= CodeLevelsIndexes.Length - 1; i++)
			{
                DesintegrationValues[i] = Convert.ToInt32(dataRow[CodeLevelsIndexes[i]]);
			}
		}

		// делегат для получения текущего состояния установки иерархии

		/// <summary>
		///  Ращепляет код записи в зависимости от установок ращепления
		/// </summary>
		/// <param name="row">текущая запись</param>
		/// <param name="Divide">правило ращепления</param>
		/// <param name="Code">код, который ращепляем</param>
		/// <param name="CodeLevels">коллекция наименования уровней ращепления</param>
		private void DivideCode(DataRow row, string Divide, string Code, Dictionary<int, DesintegrationLevel> CodeLevels)
		{
			string tmpCode = Code;
			int k = 0;
			// Нормализуем код, получаемый как строка. Если в начале не хватает нулей, добавим нули
			for (int i = 0; i <= Divide.Length - 1; i++)
				if (Divide[i] != '.') k++;
			tmpCode = tmpCode.PadLeft(k, '0');
			int index = 0;
			int j = 0;
			k = 0;
            try
            {
                StringBuilder codeValue = new StringBuilder();
                // Разбиваем код на части, которые зависят от разбиения (Divide)
                do
                {
                    if (Divide[index] != '.')
                    {
                        codeValue.Append(tmpCode[j]);
                        j++;
                    }
                    else
                    {
                        row[CodeLevels[k].LevelName] = codeValue.ToString();
                        k++;
                        codeValue.Remove(0, codeValue.Length);
                    }
                    index++;
                }
                while (index <= Divide.Length - 1);
                row["RowValidate"] = 1;
            }
            catch
            {
                row["RowValidate"] = 0;
                Trace.WriteLine(string.Format("При ращеплении кода {0} классификатора '{1}' произошла ошибка", Code, this.FullName));
            }
		}

		/// <summary>
		/// Устанавливает иерархию  
		/// </summary>
        private void SetHierarchy(DataRow[] rows, int refParentClmnInd, int parentClmnInd,
            int rowLevelClmnInd, Dictionary<int, DesintegrationLevel> CodeLevels,
            bool setFullHierarchy, bool IsMesOtchAlgoritm)
		{
			// пробегаем по всей таблице. Берем последнюю запись, для нее ищем родителя в предпоследней записи..
			// не нашли, переходим на следующую запись и т.д.
			// добрались до верха и ничего не нашли, значит верхнего уровня

			// найденные запись помещаем в контейнер для хранения последней найденной записи каждого уровня
            if (rows.Length == 0) return;
            // если есть код администратора, то его не будем учитывать при установке иерархии
            int CodeLevelsCount = CodeLevels.Count;
            if (!CodeLevels[0].UseInHierarchy)
                CodeLevelsCount = CodeLevels.Count - 1; 
            int[] CodeLevelsIndexes = new int[CodeLevels.Count];
            int[] CodeUse = new int[CodeLevels.Count];
            for (int i = 0; i <= CodeLevels.Count - 1; i++)
            {
                CodeLevelsIndexes[i] = rows[0].Table.Columns.IndexOf(CodeLevels[i].LevelName);
                CodeUse[i] = 1;
            }

            foreach (DataRow row in rows)
            {
                row[rowLevelClmnInd] = GetRowLevel(row, CodeLevelsIndexes);
            }

            if (CodeLevelsCount != CodeLevels.Count)
                CodeUse[0] = 0;

            int[] CodeValues = new int[CodeLevels.Count];

            for (int i = rows.Length - 1; i >= 0; i--)
            {
                // не ищем родителя у неращепленных записей
                if (Convert.ToInt32(rows[i]["RowValidate"]) == 1)
                    // если устанавливаем полную иерархию
                    if (setFullHierarchy)
                        // для всех записей ищем родителей
                        FindParentRowForCurrentRow(rows[i], rows, i, CodeLevelsIndexes, CodeValues,
                            refParentClmnInd, parentClmnInd, rowLevelClmnInd, CodeUse, IsMesOtchAlgoritm);
                    else
                    {
                        // ищем родителей для записей, у которых нету родителей
                        if (rows[i].IsNull(refParentClmnInd))
                            FindParentRowForCurrentRow(rows[i], rows, i, CodeLevelsIndexes, CodeValues,
                                refParentClmnInd, parentClmnInd, rowLevelClmnInd, CodeUse, IsMesOtchAlgoritm);
                    }
            }
		}

        private Dictionary<int, DataRow> FindRows = new Dictionary<int, DataRow>();

		/// <summary>
		/// Ищет родительскую запись для текущей по всей таблице
		/// </summary>
		/// <param name="row"></param>
        private void FindParentRowForCurrentRow(DataRow row, DataRow[] rows, int rowIndex,
            int[] CodeLevelsIndexes, int[] CodeValues, int refParentClmnInd, int parentClmnInd,
            int rowLevelClmnInd, int[] CodeUse, bool IsMesOtchAlgoritm)
		{
            // ищем для текущей записи запись-родитель
		    DataRow parentRow = null;
            if (IsMesOtchAlgoritm && !CheckDataRowCode(row, CodeLevelsIndexes))
            {

                parentRow = Convert.ToInt32(row[CodeLevelsIndexes[1]]) == 2 ?
                    FindParentMesOtch(row, rows, rowIndex, CodeLevelsIndexes, CodeValues, CodeUse, rowLevelClmnInd) :
                    FindParent(row, rows, rowIndex, CodeLevelsIndexes, CodeValues, CodeUse, rowLevelClmnInd);
            }
            else
                parentRow = FindParent(row, rows, rowIndex, CodeLevelsIndexes, CodeValues, CodeUse, rowLevelClmnInd);
            // если нашли, то проставляем на нее ссылочку
            if (parentRow != null)
            {
                row[refParentClmnInd] = parentRow[parentClmnInd];
            }
		}

        /// <summary>
        /// Новая версия установки иерархии
        /// </summary>
        /// <param name="ChildRow"></param>
        /// <param name="ChildRowIndex"></param>
        /// <returns></returns>
        private DataRow FindParent(DataRow ChildRow, DataRow[] rows, int ChildRowIndex,
            int[] CodeLevelsIndexes, int[] CodeValues, int[] CodeUse, int rowLevelClmnInd)
        {
            // идем по последнему уровню кода, устанавливаем в 0 и ищем такие записи среди всех, которые находятся
            // выше текущей записи. Все найденные преденденты запоминаем и среди них ищем максимально подходящего
            // Если не находим, то переходим на 1 уровень кода выше, и повторяем все действия. Так до тех пор, пока 
            // уровень кода не станет равным 0 или будут найдены претенденты, среди которых выберем запись-родителя.
            SetCodeValues(ChildRow, CodeValues, CodeLevelsIndexes);
            // начальный уровень, по которому ищем запись
            int currentLevel = CodeLevelsIndexes.Length - 1;
            int findRowsCount = 0;
            // уровень записи зависит от того, в каком месте появилась не нулевая часть кода
            int currentRowLevel = Convert.ToInt32(ChildRow[rowLevelClmnInd]);
            while (currentLevel > 0)
            {
                if (CodeValues[currentLevel] != 0)
                {
                    CodeValues[currentLevel] = 0;
                    // ищем по текущему уровню записи, у которых равно 0 (нулю), если не находим, то ищем по следующему уровню 
                    for (int i = ChildRowIndex - 1; i >= 0; i--)
                    {
                        DataRow row = rows[i];
                        if (Convert.ToInt32(row["RowValidate"]) == 1)
                            if (currentRowLevel >= Convert.ToInt32(row[rowLevelClmnInd]))
                                if (CompareRows(row, CodeLevelsIndexes, CodeValues, currentLevel, CodeUse))
                                {
                                    // если нашли, то пишем в коллекцию таких найденых, потом будем искать претендента
                                    FindRows.Add(findRowsCount, row);
                                    findRowsCount++;
                                }
                    }
                }
                currentLevel--;
            }
            // ищем претендента среди найденных записей
            // если таки нашли, то выбираем претендента
            DataRow findParentRow = null;
            if (findRowsCount > 0)
            {
                SetCodeValues(ChildRow, CodeValues, CodeLevelsIndexes);
                findParentRow = FindParentRowFromPretender(FindRows, CodeLevelsIndexes,
                    CodeValues, currentLevel);
                FindRows.Clear();
            }
            return findParentRow;
        }

        /// <summary>
        /// Новая версия установки иерархии
        /// </summary>
        /// <param name="ChildRow"></param>
        /// <param name="ChildRowIndex"></param>
        /// <returns></returns>
        private DataRow FindParentMesOtch(DataRow ChildRow, DataRow[] rows, int ChildRowIndex,
            int[] CodeLevelsIndexes, int[] CodeValues, int[] CodeUse, int rowLevelClmnInd)
        {
            // идем по последнему уровню кода, устанавливаем в 0 и ищем такие записи среди всех, которые находятся
            // выше текущей записи. Все найденные преденденты запоминаем и среди них ищем максимально подходящего
            // Если не находим, то переходим на 1 уровень кода выше, и повторяем все действия. Так до тех пор, пока 
            // уровень кода не станет равным 0 или будут найдены претенденты, среди которых выберем запись-родителя.
            SetCodeValues(ChildRow, CodeValues, CodeLevelsIndexes);
            // начальный уровень, по которому ищем запись
            int currentLevel = CodeLevelsIndexes.Length - 1;
            int findRowsCount = 0;
            // уровень записи зависит от того, в каком месте появилась не нулевая часть кода
            int currentRowLevel = Convert.ToInt32(ChildRow[rowLevelClmnInd]);
            while (currentLevel > 0)
            {
                // пропускаем уровень, который считается вместе с предыдущим
                if (currentLevel != sixthLevelIndex)
                {
                    int currentLevelValue = CodeValues[currentLevel];
                    // для пятого уровня получим значение из пятого и шестого уровней
                    if (currentLevel == fifthLevelIndex)
                        currentLevelValue = CodeValues[fifthLevelIndex] * 10 + CodeValues[sixthLevelIndex];
                    if (currentLevelValue != 0)
                    {
                        // для поиска родителя обнуляем значение текущего уровня
                        CodeValues[currentLevel] = 0;
                        if (currentLevel == fifthLevelIndex)
                            CodeValues[sixthLevelIndex] = 0;
                        // ищем по текущему уровню записи, у которых равно 0 (нулю), если не находим, то ищем по следующему уровню 
                        for (int i = ChildRowIndex - 1; i >= 0; i--)
                        {
                            DataRow row = rows[i];
                            if (Convert.ToInt32(row["RowValidate"]) == 1)
                                if (currentRowLevel >= Convert.ToInt32(row[rowLevelClmnInd]))
                                    if (CompareRowsMesOtch(row, CodeLevelsIndexes, CodeValues, currentLevel, CodeUse))
                                    {
                                        // если нашли, то пишем в коллекцию таких найденых, потом будем искать претендента
                                        FindRows.Add(findRowsCount, row);
                                        findRowsCount++;
                                    }
                        }
                    }
                }
                currentLevel--;
            }
            // ищем претендента среди найденных записей
            // если таки нашли, то выбираем претендента
            DataRow findParentRow = null;
            if (findRowsCount > 0)
            {
                SetCodeValues(ChildRow, CodeValues, CodeLevelsIndexes);
                findParentRow = FindParentRowFromPretenderMesOtch(FindRows, CodeLevelsIndexes,
                    CodeValues, currentLevel);

                FindRows.Clear();
            }
            return findParentRow;
        }

        /// <summary>
        /// проверка на исключение из исключения
        /// </summary>
        /// <param name="row"></param>
        /// <param name="CodeLevelsIndexes"></param>
        /// <returns></returns>
        private bool CheckDataRowCode(DataRow row, int[] CodeLevelsIndexes)
        {
            // для кодов начинающихся на 
            //000.2.02.05
            //000.2.02.09
            //000.2.04.01
            int code2 = Convert.ToInt32(row[CodeLevelsIndexes[1]]);
            if (code2 != 2)
                return false;
            int code3 = Convert.ToInt32(row[CodeLevelsIndexes[2]]);
            int code4 = Convert.ToInt32(row[CodeLevelsIndexes[3]]);
            if (code3 == 2 && code4 == 5)
                return true;
            if (code3 == 2 && code4 == 9)
                return true;
            if (code3 == 4 && code4 == 1)
                return true;
            return false;
            
        }


        #region поиск претендентов на родительскую запись

        /// <summary>
        /// поиск претендентов на звание родительской записи
        /// </summary>
        /// <param name="ParentRows"></param>
        /// <param name="CodeLevels"></param>
        /// <param name="CodeValues"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        private DataRow FindParentRowFromPretender(Dictionary<int, DataRow> ParentRows, int[] CodeLevels, int[] CodeValues, int Level)
        {
            int[] rowsMarks = new int[ParentRows.Count];
            DataRow row = null;
            // бежим по всем найденным записям
            for (int i = 0; i <= ParentRows.Count - 1; i++)
            {
                // считаем число баллов для каждой записи
                for (int j = CodeLevels.Length - 1; j >= Level; j--)
                {
                    if (CodeValues[j] == Convert.ToInt32(ParentRows[i][CodeLevels[j]]))
                        rowsMarks[i]++;
                    else if (Convert.ToInt32(ParentRows[i][CodeLevels[j]]) != 0)
                        rowsMarks[i] = rowsMarks[i] - 1;
                }
            }
            int maxMark = rowsMarks[0];
            int index = 0;
            for (int i = 1; i <= rowsMarks.Length - 1; i++)
            {
                if (maxMark < rowsMarks[i])
                {
                    maxMark = rowsMarks[i];
                    index = i;
                }
            }
            if (maxMark >= 0)
                row = ParentRows[index];
            return row;
        }

        /// <summary>
        /// поиск претендентов на звание родительской записи для классификаторов по месячной отчетности
        /// </summary>
        /// <param name="ParentRows"></param>
        /// <param name="CodeLevels"></param>
        /// <param name="CodeValues"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        private DataRow FindParentRowFromPretenderMesOtch(Dictionary<int, DataRow> ParentRows, int[] CodeLevels, int[] CodeValues, int Level)
        {
            int[] rowsMarks = new int[ParentRows.Count];
            DataRow row = null;
            // бежим по всем найденным записям
            for (int i = 0; i <= ParentRows.Count - 1; i++)
            {
                // считаем число баллов для каждой записи
                for (int j = CodeLevels.Length - 1; j >= Level; j--)
                {
                    // не проверяем 6 уровень разбиения кода
                    if (j == sixthLevelIndex)
                        continue;
                    // пятый уровень смотрим как сумму пятого и шестого
                    if (j == fifthLevelIndex)
                    {
                        if (CodeValues[fifthLevelIndex] * 10 + CodeValues[sixthLevelIndex] ==
                            Convert.ToInt32(ParentRows[i][CodeLevels[fifthLevelIndex]]) * 10 +
                            Convert.ToInt32(ParentRows[i][CodeLevels[sixthLevelIndex]]))
                            rowsMarks[i]++;
                        else if (Convert.ToInt32(ParentRows[i][CodeLevels[j]]) +
                            Convert.ToInt32(ParentRows[i][CodeLevels[j + 1]])
                            != 0)
                            rowsMarks[i] = rowsMarks[i] - 1;
                    }
                    else
                    {
                        if (CodeValues[j] == Convert.ToInt32(ParentRows[i][CodeLevels[j]]))
                            rowsMarks[i]++;
                        else if (Convert.ToInt32(ParentRows[i][CodeLevels[j]]) != 0)
                            rowsMarks[i] = rowsMarks[i] - 1;
                    }
                }
            }
            int maxMark = rowsMarks[0];
            int index = 0;
            for (int i = 1; i <= rowsMarks.Length - 1; i++)
            {
                if (maxMark < rowsMarks[i])
                {
                    maxMark = rowsMarks[i];
                    index = i;
                }
            }
            if (maxMark >= 0)
                row = ParentRows[index];
            return row;
        }

        #endregion

        /// <summary>
        /// получение уровня записи. Первое не нулевое значение
        /// </summary>
        /// <param name="row"></param>
        /// <param name="CodeLevelsIndexes"></param>
        /// <returns></returns>
        private int GetRowLevel(DataRow row, int[] CodeLevelsIndexes)
		{
            if (!row.IsNull("RowLevel"))
                return Convert.ToInt32(row["RowLevel"]);
            for (int i = CodeLevelsIndexes.Length - 1; i >= 0; i--)
			{
                if (!row.IsNull(CodeLevelsIndexes[i]))
                {
                    if (Convert.ToInt32(row[CodeLevelsIndexes[i]]) != 0)
                    {
                        row["RowLevel"] = i;
                        return i;
                    }
                }
			}
            row["RowLevel"] = 0;
			return 0;
        }

        #region сравнение записей на предмет поиска родительской

        /// <summary>
		/// Сравниваем записи по кодам
		/// </summary>
		/// <param name="CodeLevels"></param>
		/// <param name="CodeValues"></param>
		/// <param name="currentLevel"></param>
		/// <returns></returns>
        private bool CompareRows(DataRow row, int[] CodeLevels, int[] CodeValues, int currentLevel, int[] CodeUse)
		{
			bool findRow = true;
			for (int i = 0; i <= currentLevel; i++)
			{
                if (CodeUse[i] == 1)
                    if (!(Convert.ToInt32(row[CodeLevels[i]]) == CodeValues[i]))
                    {
                        findRow = false;
                        break;
                    }
			}
			return findRow;
		}
        
        private bool CompareRowsMesOtch(DataRow row, int[] CodeLevels, int[] CodeValues, int currentLevel, int[] CodeUse)
        {
            // если текущий уровень тот, который не проверяем, то просто не проверяем его
            for (int i = 0; i <= currentLevel; i++)
            {
                if (i == sixthLevelIndex)
                    continue;
                if (CodeUse[i] == 1)
                {
                    int compareValue1 = 0;
                    int compareValue2 = 0;
                    if (i == fifthLevelIndex)
                    {
                        compareValue1 = Convert.ToInt32(row[CodeLevels[fifthLevelIndex]]) * 10 +
                            Convert.ToInt32(row[CodeLevels[sixthLevelIndex]]);
                        compareValue2 = CodeValues[fifthLevelIndex] * 10 +
                            CodeValues[sixthLevelIndex];
                    }
                    else
                    {
                        compareValue1 = Convert.ToInt32(row[CodeLevels[i]]);
                        compareValue2 = CodeValues[i];
                    }
                    if (compareValue1 != compareValue2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }

    /// <summary>
	///  Вспомогательный класс
	///  Содердит наименование уровней ращепления
	/// </summary>
	internal class DesintegrationLevel
	{
		public long Divider;
		public string LevelName;
		public bool UseInHierarchy;

		public DesintegrationLevel(int index, string currentDivide)
		{
			string tmpDivide = string.Empty;
			try
			{
				int tmpDiv = Convert.ToInt32(currentDivide);
				if (tmpDiv >= 0)
				{
					UseInHierarchy = true;
					tmpDivide = currentDivide;
				}
				else
				{
					UseInHierarchy = false;
					tmpDivide = currentDivide.Remove(0, 1);
				}
			}
			catch
			{
				UseInHierarchy = false;
				tmpDivide = currentDivide[currentDivide.Length - 1].ToString();
			}
            LevelName = "Code" + index.ToString();
			Divider = 1;
			int i = Convert.ToInt32(tmpDivide);
			for (int j = 0; j < i; j++)
				Divider = 10 * Divider;
		}
	}
}
