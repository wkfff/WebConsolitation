#region using

//  Managed Runtime namespace           [Microsoft.SqlServer.ManagedDTS.dll]
//  Pipeline Primary Interop Assembly   [Microsoft.SqlServer.DTSPipelineWrap.dll]
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ExecutePackageTaskLib;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Utils.DTSGenerator.TreeObjects;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;
using Application = Microsoft.SqlServer.Dts.Runtime.Application;
using DTSEventFilterKind = Microsoft.SqlServer.Dts.Runtime.DTSEventFilterKind;
using DTSExecResult = Microsoft.SqlServer.Dts.Runtime.DTSExecResult;
using DTSLoggingMode = Microsoft.SqlServer.Dts.Runtime.DTSLoggingMode;
using DTSPackageType = Microsoft.SqlServer.Dts.Runtime.DTSPackageType;
using ForEachEnumeratorHost = Microsoft.SqlServer.Dts.Runtime.Wrapper.ForEachEnumeratorHost;
using Package = Microsoft.SqlServer.Dts.Runtime.Package;
using PrecedenceConstraint = Microsoft.SqlServer.Dts.Runtime.PrecedenceConstraint;
using Sequence = Microsoft.SqlServer.Dts.Runtime.Sequence;
using TaskHost = Microsoft.SqlServer.Dts.Runtime.TaskHost;
using wrap = Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Threading;

#endregion

namespace Krista.FM.Utils.DTSGenerator
{
    public enum ProviderType
    {
        Oracle,
        MSSQL
    }

    /// <summary>
    /// Класс отвечающий за перенос данных
    /// </summary>
    public class IntegrationScheme
    {
        #region Fields

        /// <summary>
        /// Схема-источник для переноса данных
        /// </summary>
        private readonly IScheme schemeSource = null;
        /// <summary>
        /// Схема-преемник для переноса данных
        /// </summary>
        private readonly IScheme schemeDestination = null;
        /// <summary>
        /// Заранее получаем интерфейс, потом какие-то проблемы с контекстом
        /// </summary>
        private readonly IDatabase destinationDatabase;
        /// <summary>
        /// Родительская форма
        /// </summary>
        private readonly CreateDTSXPForm form;
        /// <summary>
        /// лог создания переноса
        /// </summary>
        const string LOGFILENAME = @"\RuntimeLog.log";
        /// <summary>
        /// IDTSEvents90
        /// </summary>
        private readonly PackageEvents packageEvents;            // class that implements the Package events interface IDTSEvents90
        /// <summary>
        /// IDTSComponentEvents90
        /// </summary>
        private readonly ComponentEvents pipelineEvents;         // class that implements the component events interface IDTSComponentEvents90
        /// <summary>
        /// Тип провайдера преемника. Нужен для построение запросов к базе 
        /// </summary>
        private ProviderType destinationProvider;

        /// <summary>
        /// Коллекция пакетов для переноса
        /// </summary>
        private List<DtsxPackage> packageList = new List<DtsxPackage>();

        private LogicalCallContextData _context;

        // корневой пакет
        Package package;

        #endregion

        #region Constructor
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="schemeSource"></param>
        /// <param name="schemeDestination"></param>
        /// <param name="packageNode"></param>
        /// <param name="form"></param>
        /// <param name="database"></param>
        public IntegrationScheme(LogicalCallContextData context, IScheme schemeSource, IScheme schemeDestination, string packageName, CreateDTSXPForm form, IDatabase database)
        {
            this.schemeSource = schemeSource;
            this.schemeDestination = schemeDestination;
            this.form = form;
            destinationDatabase = database;

            _context = context;

            string currentDirectory = Directory.GetCurrentDirectory();

            // Delete the log file, destination, and package files.
            File.Delete(currentDirectory + LOGFILENAME);

            package = GetInnerPackage(packageName);

            // Events.
            packageEvents = new PackageEvents();
            pipelineEvents = new ComponentEvents();
        }
        #endregion

        #region  AnalisysScheme

        /// <summary>
        /// Анализ схемы (общий перенос)
        /// </summary>
        public void AnalisysAllScheme(SSISMajorTreeBase<SSISPackageObject> packageNode)
        {
            AnalisysRootPackage(packageNode, package, form.UltraTree.Nodes[0]);
            CreateBatFile();
        }

        /// <summary>
        /// Анализ схемы (по заранее сформированному набору объектов)
        /// </summary>
        /// <param name="package"></param>
        /// <param name="packageObject"></param>
        public void AnalisysScheme(SSISPackageObject packageObject)
        {
            AnalisysPackage(package, form.UltraTree.Nodes[0], packageObject.SsisEntities);
            CreateBatFile();
        }

        /// <summary>
        /// Формирование батника по переносу
        /// </summary>
        private void CreateBatFile()
        {
            string batString = string.Empty;

            FileStream bat = File.Create("Run.bat");

            StreamWriter writer = new StreamWriter(bat, Encoding.GetEncoding(1251));

            batString += BatWriter.HeaderBat();
            batString += BatWriter.SetVariable(schemeSource.SchemeDWH.OriginalConnectionString, schemeDestination.SchemeDWH.OriginalConnectionString);

            // c пакета _Общие классификаторы есть ссылки на сопоставимые расположенные ниже 
            // в _Общие... Впринцепе, конфликтных ситуаций на схеме нет. Пока не стали ничего
            // планнировать с инициализацией. В переносе _Общие классификаторы переносим
            // после других общих пакетов, на кот есть ссылки

            ReplacePackage();

            foreach (DtsxPackage package in packageList)
            {
                batString += package.ToString();
            }

            batString += BatWriter.EndBat();

            writer.Write(batString);

            writer.Dispose();
        }

        private void ReplacePackage()
        {
            DtsxPackage replacePackage = null;
            DtsxPackage afterPackage = null;

            foreach (DtsxPackage package in packageList)
            {
                if (package.Name == "_Общие классификаторы")
                    replacePackage = package;
                if (package.Name == "_Общие классификаторы_Планирование")
                    afterPackage = package;
            }

            if (replacePackage != null && afterPackage != null)
            {
                packageList.Remove(replacePackage);
                int index = packageList.IndexOf(afterPackage);
                packageList.Insert(index + 1, replacePackage);
            }
        }

        #endregion

        #region AnalisysPackage

        /// <summary>
        /// Анализ пакета и его подпакетов
        /// </summary>
        /// <param name="parentPackageNode"></param>
        /// <param name="ssisParentPackage"></param>
        /// <param name="rootnode"></param>
        private void AnalisysRootPackage(SSISMajorTreeBase<SSISPackageObject> parentPackageNode, Package ssisParentPackage, UltraTreeNode rootnode)
        {
            if (parentPackageNode.CheckedState == CheckState.Checked)
            {
                UltraTreeNode node = StartHandlePackage(ssisParentPackage, rootnode);

                IPackage iPackage = parentPackageNode.ControlOblect.ControlObject as IPackage;

                if (iPackage != null)
                {
                    Dictionary<string, SSISEntitiesObject> dict = parentPackageNode.ControlOblect.SsisEntities;

                    AnalisysPackage(ssisParentPackage, rootnode, dict);
                }
            }

            if (parentPackageNode.CheckedState == CheckState.Checked || parentPackageNode.CheckedState == CheckState.Indeterminate)
            {
                // Обработка подпакетов дерева
                foreach (UltraTreeNode ultraTreeNode in parentPackageNode.Nodes)
                {
                    if (ultraTreeNode.CheckedState == CheckState.Checked || ultraTreeNode.CheckedState == CheckState.Indeterminate)
                        AnalisysRootPackage((SSISMajorTreeBase<SSISPackageObject>)ultraTreeNode,
                                        GetInnerPackage(ultraTreeNode.Text), rootnode);
                }
            }
        }

        /// <summary>
        /// Обрабока пакета
        /// </summary>
        /// <param name="ssisParentPackage"></param>
        /// <param name="rootnode"></param>
        /// <param name="list"></param>
        /// <param name="node"></param>
        private void AnalisysPackage(Package ssisParentPackage, UltraTreeNode rootnode, Dictionary<string, SSISEntitiesObject> list)
        {
            UltraTreeNode node = StartHandlePackage(ssisParentPackage, rootnode);
            // предыдущий контейнер
            Sequence sequencePrev = null;

            foreach (SSISEntitiesObject ssisEntity in list.Values)
            {
                if (((IEntity)ssisEntity.ControlObject).ClassType != ClassTypes.clsFixedClassifier)
                    sequencePrev = AddingEntityExecuteTask(ssisEntity, ssisParentPackage, sequencePrev, rootnode);
            }

            ValidatePackage(ssisParentPackage, rootnode);

            SavePackage(ssisParentPackage, rootnode);

            packageList.Add(new DtsxPackage(ssisParentPackage.Name + ".dtsx"));
        }


        /// <summary>
        /// Добавление новой задачи по переносу
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="package"></param>
        /// <param name="sequencePrev"></param>
        /// <param name="node"></param>
        private Sequence AddingEntityExecuteTask(SSISEntitiesObject ssisEntity, Package package, DtsContainer sequencePrev, UltraTreeNode node)
        {

            IEntity entity = ssisEntity.ControlObject as IEntity;

            Sequence currentSequence = (Sequence)package.Executables.Add("STOCK:SEQUENCE");

            currentSequence.Name = entity.FullDBName;
            currentSequence.Description = entity.Description;

            StartdataFlowtask(entity.FullDBName, node);

            TaskHost task = AddDataFlowTask(entity.FullDBName, entity.Description, currentSequence, out task);
            // источник
            IDTSComponentMetaData100 oleDBSource = AddOLEDBSource(ssisEntity, task, package);
            // приемник
            bool existRows = AddOLEDBDestination(entity, task, package, oleDBSource);

            if (!existRows) currentSequence.Disable = true;

            ValidationTask(task, node);

            // Команда на очистку таблицы
            //TaskHost deleteData = DeleteData(currentSequence, ssisEntity);

            TaskHost disableTriggerLastTask = DisableTriggers(currentSequence, entity);

            TaskHost enableTriggerLastTask = EnableTriggers(currentSequence, entity);
            TaskHost disableConstraint = null;
            TaskHost enableConstraint = null;
            if (entity.Presentations.Count != 0)
            {
                foreach (KeyValuePair<string, IPresentation> keyValuePair in entity.Presentations)
                {
                    if (keyValuePair.Value.Attributes.ContainsKey("ParentID"))
                    {
                        disableConstraint = DisableConstraint(currentSequence, entity);
                        enableConstraint = EnableConstraint(currentSequence, entity);

                        break;
                    }
                }
            }
            else
            {
                if (entity.Attributes.ContainsKey("ParentID"))
                {
                    disableConstraint = DisableConstraint(currentSequence, entity);
                    enableConstraint = EnableConstraint(currentSequence, entity);
                }
            }

            CreateSequence(currentSequence, new TaskHost[]  { disableTriggerLastTask, disableConstraint, /*deleteData, */task, enableConstraint,
                                                                  enableTriggerLastTask });

            if (sequencePrev != null)
                CreateConstraint(package, sequencePrev, currentSequence);

            DTSExecResult status = currentSequence.Validate(null, null, packageEvents, null);
            Console.WriteLine("\tВалидация контейнера " + currentSequence.Name + ": " + status);

            return currentSequence;
        }

        private void CreateSequence(Sequence currentSequence, TaskHost[] tasksHost)
        {
            TaskHost prevTask = null;

            foreach (TaskHost taskHost in tasksHost)
            {
                if (taskHost != null)
                {
                    if (prevTask != null)
                        CreateConstraint(currentSequence, prevTask, taskHost);

                    prevTask = taskHost;
                }
            }
        }

        private TaskHost DeleteData(Sequence currentSequence, SSISEntitiesObject ssisEntity)
        {
            IEntity entity = ssisEntity.ControlObject as IEntity;

            if (entity != null)
            {
                TaskHost sqlTaskDelete = currentSequence.Executables.Add("STOCK:SQLTask") as TaskHost;

                if (sqlTaskDelete != null) sqlTaskDelete.Name = String.Format("Delete from {0}", entity.FullDBName);

                if (sqlTaskDelete != null)
                {
                    ExecuteSQLTask mytask = sqlTaskDelete.InnerObject as ExecuteSQLTask;
                    if (mytask != null) mytask.Connection = "OLEDBConnectionDest";
                    if (mytask != null)
                    {
                        mytask.SqlStatementSource =
                            String.Format("DELETE FROM {0}", entity.FullDBName);

                        int clausePosition = ssisEntity.SqlExpession.ToUpper().LastIndexOf("WHERE");
                        if (clausePosition != -1)
                        {
                            mytask.SqlStatementSource += String.Format(" {0}", ssisEntity.SqlExpession.Substring(clausePosition));
                        }
                    }
                }

                return sqlTaskDelete;
            }

            return null;
        }

        #region Включение отключение тригеров

        private TaskHost EnableTriggers(Sequence sequence, ICommonDBObject entity)
        {
            TaskHost prevTask = null;

            TaskHost firstTask = null;

            foreach (KeyValuePair<string, string> pair in entity.GetSQLMetadataDictionary())
            {
                if (GetExsitDelegate()(pair.Value, ObjectTypes.Trigger))
                {
                    if (pair.Key != SQLMetadataConstants.ParentIDForeignKeyConstraint)
                    {
                        TaskHost sqlTaskDisable = sequence.Executables.Add("STOCK:SQLTask") as TaskHost;
                        if (sqlTaskDisable != null) sqlTaskDisable.Name = pair.Key + "_Enable";

                        if (sqlTaskDisable != null)
                        {
                            ExecuteSQLTask mytask = sqlTaskDisable.InnerObject as ExecuteSQLTask;
                            if (mytask != null) mytask.Connection = "OLEDBConnectionDest";
                            if (mytask != null)
                            {
                                if (destinationProvider == ProviderType.MSSQL)
                                    mytask.SqlStatementSource =
                                        String.Format("ALTER TABLE {0} ENABLE TRIGGER {1}", entity.FullDBName,
                                                  pair.Value);
                                else
                                    mytask.SqlStatementSource =
                                       String.Format("ALTER TRIGGER {0} ENABLE",
                                                 pair.Value);
                            }
                        }

                        if (sqlTaskDisable != null)
                        {
                            sqlTaskDisable.Validate(null, null, null, null);
                        }
                        if (prevTask != null)
                            CreateConstraint(sequence, prevTask, sqlTaskDisable);

                        prevTask = sqlTaskDisable;

                        if (firstTask == null)
                            firstTask = sqlTaskDisable;
                    }
                }
            }

            return firstTask;
        }


        private TaskHost DisableTriggers(Sequence sequence, ICommonDBObject entity)
        {
            TaskHost prevTask = null;

            foreach (KeyValuePair<string, string> pair in entity.GetSQLMetadataDictionary())
            {
                if (GetExsitDelegate()(pair.Value, ObjectTypes.Trigger))
                {
                    if (pair.Key != SQLMetadataConstants.ParentIDForeignKeyConstraint)
                    {
                        TaskHost sqlTaskDisable = sequence.Executables.Add("STOCK:SQLTask") as TaskHost;
                        if (sqlTaskDisable != null) sqlTaskDisable.Name = pair.Key + "_Disable";

                        if (sqlTaskDisable != null)
                        {
                            ExecuteSQLTask mytask = sqlTaskDisable.InnerObject as ExecuteSQLTask;
                            if (mytask != null) mytask.Connection = "OLEDBConnectionDest";
                            if (mytask != null)
                            {
                                if (destinationProvider == ProviderType.MSSQL)
                                    mytask.SqlStatementSource =
                                        String.Format("ALTER TABLE {0} DISABLE TRIGGER {1}", entity.FullDBName,
                                                  pair.Value);
                                else
                                    mytask.SqlStatementSource =
                                        String.Format("ALTER TRIGGER {0} DISABLE",
                                                  pair.Value);
                            }
                        }

                        if (sqlTaskDisable != null)
                        {
                            sqlTaskDisable.Validate(null, null, null, null);
                        }
                        if (prevTask != null)
                            CreateConstraint(sequence, prevTask, sqlTaskDisable);

                        prevTask = sqlTaskDisable;
                    }
                }
            }

            return prevTask;
        }

        #endregion

        #region Включение - отключение констрейнов

        private delegate bool Existhandler(string objectName, ObjectTypes objectType);

        /// <summary>
        /// Отключение\включение проверки констрейнов, надо определять СУБД назначения, а то запросы на констрейны разные
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="entity"></param>
        private TaskHost DisableConstraint(Sequence seq, ICommonDBObject entity)
        {
            if (seq == null) throw new ArgumentNullException("seq");
            // добавляем отключение\включение проверки констрейнов для классификаторов, у которых
            // иерархия parent-child, но предварительно проверяем, есть ли констрейн, который
            // собираемся отключить

            if (GetExsitDelegate()(ParentIDForeignKeyConstraintName(entity), ObjectTypes.ForeignKeysConstraint))
            {
                TaskHost sqlTaskDisable = seq.Executables.Add("STOCK:SQLTask") as TaskHost;
                if (sqlTaskDisable != null) sqlTaskDisable.Name = entity.FullDBName + "_Disable";

                if (sqlTaskDisable != null)
                {
                    ExecuteSQLTask mytask = sqlTaskDisable.InnerObject as ExecuteSQLTask;
                    if (mytask != null) mytask.Connection = "OLEDBConnectionDest";
                    if (mytask != null)
                    {
                        if (destinationProvider == ProviderType.MSSQL)
                            mytask.SqlStatementSource =
                                String.Format("ALTER TABLE {0} NOCHECK CONSTRAINT {1}", entity.FullDBName,
                                              ParentIDForeignKeyConstraintName(entity));
                        else
                            mytask.SqlStatementSource =
                                String.Format("ALTER TABLE {0} DISABLE CONSTRAINT {1}", entity.FullDBName,
                                              ParentIDForeignKeyConstraintName(entity));
                    }
                    DTSExecResult status = sqlTaskDisable.Validate(null, null, null, null);
                    Console.WriteLine("\t\tВалидация SQL-задачи " + sqlTaskDisable.Name + ": " + status);

                    return sqlTaskDisable;
                }
            }
            return null;
        }

        /// <summary>
        /// В зависимости от типа провайдера определяем метод поиска объекта в базе
        /// </summary>
        /// <returns>Метод поиска объекта</returns>
        private Existhandler GetExsitDelegate()
        {
            Existhandler exsitDelegate = null;

            switch (destinationProvider)
            {
                case ProviderType.MSSQL:
                    exsitDelegate = ExistsObjectMS;
                    break;
                case ProviderType.Oracle:
                    exsitDelegate = ExistsObjectOracle;
                    break;
            }
            return exsitDelegate;
        }

        /// <summary>
        /// Включение констрейна
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private TaskHost EnableConstraint(IDTSSequence sequence, ICommonDBObject entity)
        {
            if (GetExsitDelegate()(ParentIDForeignKeyConstraintName(entity), ObjectTypes.ForeignKeysConstraint))
            {
                TaskHost sqlTaskEnanble = sequence.Executables.Add("STOCK:SQLTask") as TaskHost;
                if (sqlTaskEnanble != null) sqlTaskEnanble.Name = entity.FullDBName + "Enable";

                if (sqlTaskEnanble != null)
                {
                    ExecuteSQLTask mytaskEn = sqlTaskEnanble.InnerObject as ExecuteSQLTask;
                    if (mytaskEn != null) mytaskEn.Connection = "OLEDBConnectionDest";
                    if (mytaskEn != null)
                    {
                        if (destinationProvider == ProviderType.MSSQL)
                            mytaskEn.SqlStatementSource =
                                String.Format("ALTER TABLE {0} CHECK CONSTRAINT {1}", entity.FullDBName,
                                              ParentIDForeignKeyConstraintName(entity));
                        else
                            mytaskEn.SqlStatementSource =
                              String.Format("ALTER TABLE {0} DISABLE CONSTRAINT {1}", entity.FullDBName,
                                            ParentIDForeignKeyConstraintName(entity));
                    }

                    DTSExecResult status = sqlTaskEnanble.Validate(null, null, null, null);
                    Console.WriteLine("\t\tВалидация SQL-задачи " + sqlTaskEnanble.Name + ": " + status);
                }

                return sqlTaskEnanble;
            }
            return null;
        }

        #endregion


        /// <summary>
        /// Поиск констрейна, который собираемся отключить
        /// </summary>
        /// <returns></returns>
        private bool FindConstriant(ICommonDBObject entity)
        {
            bool exist = false;
            DataTable table =
                (DataTable)destinationDatabase.ExecQuery(
                               String.Format(
                                   "select ''||constraint_name||''code from user_constraints where constraint_type = 'R' and table_name = '{0}'",
                                   entity.FullDBName.ToUpper()), QueryResultTypes.DataTable);
            foreach (DataRow row in table.Rows)
            {
                if (row["code"].ToString().ToUpper() == ParentIDForeignKeyConstraintName(entity).ToUpper())
                    exist = true;
            }
            return exist;
        }

        #region Проверка на существование объекта в базе

        /// <summary>
        /// Тыпы объектов в базе данны.
        /// </summary>
        internal enum ObjectTypes
        {
            /// <summary>
            /// Индекс.
            /// </summary>
            Index,
            /// <summary>
            /// Хранимая процедура.
            /// </summary>
            Procedure,
            /// <summary>
            /// Последовательность (генератор).
            /// </summary>
            Sequence,
            /// <summary>
            /// Отношение.
            /// </summary>
            Table,
            /// <summary>
            /// Триггер.
            /// </summary>
            Trigger,
            /// <summary>
            /// Представление.
            /// </summary>
            View,
            /// <summary>
            /// Ограничение.
            /// </summary>
            ForeignKeysConstraint
        }

        /// <summary>
        /// Преобразует перечисление в строку
        /// </summary>
        /// <param name="objectType">Тып объекта в базе данны</param>
        /// <returns>Наименование объекта в базе данных</returns>
        private static string ObjectTypes2StringMS(ObjectTypes objectType)
        {
            switch (objectType)
            {
                case ObjectTypes.Index: return "INDEX";
                case ObjectTypes.Procedure: return "P";
                case ObjectTypes.Sequence: return "U";
                case ObjectTypes.Table: return "U";
                case ObjectTypes.Trigger: return "TR";
                case ObjectTypes.View: return "V";
                case ObjectTypes.ForeignKeysConstraint: return "F";
                default: throw new Exception("Указан неизвестный тип объекта.");
            }
        }

        internal bool ExistsObjectMS(string objectName, ObjectTypes objectType)
        {
            string schemaName = objectType == ObjectTypes.Sequence ? "G" : "DV";

            int schemaID = Convert.ToInt32(destinationDatabase.ExecQuery("select schema_id from sys.schemas where Upper(name) = ?",
                                                                         QueryResultTypes.Scalar, destinationDatabase.CreateParameter("Name", schemaName)));

            return 1 == Convert.ToInt32(destinationDatabase.ExecQuery("select count(*) from DVDB_Objects where Schema_id = ? and Upper(name) = ? and type = ?",
                                                                      QueryResultTypes.Scalar,
                                                                      destinationDatabase.CreateParameter("Schema_id", schemaID),
                                                                      destinationDatabase.CreateParameter("ObjectName", objectName.ToUpper()),
                                                                      destinationDatabase.CreateParameter("ObjectType", ObjectTypes2StringMS(objectType))));
        }

        /// <summary>
        /// Преобразует перечисление в строку
        /// </summary>
        /// <param name="objectType">Тып объекта в базе данны</param>
        /// <returns>Наименование объекта в базе данных</returns>
        private static string ObjectTypes2StringOracle(ObjectTypes objectType)
        {
            switch (objectType)
            {
                case ObjectTypes.Index: return "INDEX";
                case ObjectTypes.Procedure: return "PROCEDURE";
                case ObjectTypes.Sequence: return "SEQUENCE";
                case ObjectTypes.Table: return "TABLE";
                case ObjectTypes.Trigger: return "TRIGGER";
                case ObjectTypes.View: return "VIEW";
                case ObjectTypes.ForeignKeysConstraint: return "R";
                default: throw new Exception("Указан неизвестный тип объекта.");
            }
        }

        internal bool ExistsObjectOracle(string objectName, ObjectTypes objectType)
        {
            return 1 == Convert.ToInt32(destinationDatabase.ExecQuery("select count(*) from DVDB_Objects where Name = ? and Type = ?",
                QueryResultTypes.Scalar,
                destinationDatabase.CreateParameter("Name", objectName.ToUpper()),
                destinationDatabase.CreateParameter("Type", ObjectTypes2StringOracle(objectType))));
        }

        #endregion


        /// <summary>
        /// Определяем имя констрейна, построенного на поле ParentID
        /// </summary>
        /// <returns></returns>
        private static string ParentIDForeignKeyConstraintName(ICommonDBObject obj)
        {
            Dictionary<string, string> sqlMetadata = obj.GetSQLMetadataDictionary();

            if (sqlMetadata.ContainsKey(SQLMetadataConstants.ParentIDForeignKeyConstraint))
                return sqlMetadata[SQLMetadataConstants.ParentIDForeignKeyConstraint];

            return String.Empty;
        }


        /// <summary>
        /// Добавление задачи на добавление подпакета
        /// </summary>
        /// <param name="packageNode"></param>
        /// <param name="prevTask"></param>
        /// <param name="ssisParentPackage"></param>
        /// <returns></returns>
        /// <param name="rootNode"></param>
        private TaskHost AddingPackageExecuteTask(SSISMajorTreeBase<SSISPackageObject> packageNode, TaskHost prevTask, Package ssisParentPackage, UltraTreeNode rootNode)
        {
            Package innerPackage = GetInnerPackage(packageNode.Text);

            // Add new connection
            ConnectionManager cmflatFile = ssisParentPackage.Connections.Add("FILE");
            cmflatFile.Properties["ConnectionString"].SetValue(cmflatFile, Directory.GetCurrentDirectory() + "\\" + innerPackage.Name + ".dtsx");
            cmflatFile.Properties["Name"].SetValue(cmflatFile, innerPackage.Name + ".dtsx");

            // добавляем задачу
            TaskHost task = ssisParentPackage.Executables.Add("STOCK:ExecutePackageTask") as TaskHost;
            if (task != null)
            {
                task.Name = ((IPackage)packageNode.ControlOblect.ControlObject).Name;
                task.Description = ((IPackage)packageNode.ControlOblect.ControlObject).Description;

                ExecutePackageTask myTask;
                myTask = task.InnerObject as ExecutePackageTask;

                if (myTask != null) myTask.Connection = String.Format("{0}.dtsx", ((IPackage)packageNode.ControlOblect.ControlObject).Name);
            }

            if (prevTask != null)
            {
                CreateConstraint(ssisParentPackage, prevTask, task);
            }

            prevTask = task;

            AnalisysRootPackage(packageNode, innerPackage, rootNode);
            return prevTask;
        }

        private Package GetInnerPackage(string packageName)
        {
            // Add package
            Package innerPackage = CreatePackage(packageName, String.Empty);
            // Добавление подключений
            AddConnectionManagers(innerPackage);
            return innerPackage;
        }

        /// <summary>
        /// Направление выполнения потока
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="prevTask"></param>
        /// <param name="task"></param>
        private static void CreateConstraint(Sequence seq, DtsContainer prevTask, DtsContainer task)
        {
            if (seq == null) throw new ArgumentNullException("seq");
            // соединяем
            // создаем новый объект пути
            PrecedenceConstraint pc =
                seq.PrecedenceConstraints.Add(prevTask, task);
            pc.Name = String.Format("{0}-{1}", prevTask.Name, task.Name);
        }

        /// <summary>
        /// Направление выполнения потока
        /// </summary>
        /// <param name="package"></param>
        /// <param name="prevTask"></param>
        /// <param name="task"></param>
        private static void CreateConstraint(IDTSSequence package, DtsContainer prevTask, DtsContainer task)
        {
            // соединяем
            // создаем новый объект пути
            PrecedenceConstraint pc =
                package.PrecedenceConstraints.Add(prevTask, task);
            pc.Name = String.Format("{0}-{1}", prevTask.Name, task.Name);
        }

        #endregion

        #region Делегаты, события и т.д. и т.п

        private delegate UltraTreeNode HandlePackagedelegate(Package package, UltraTreeNode rootnode);

        private delegate void HandleVoidPackagedelegate(Package package, UltraTreeNode rootnode);

        private delegate void DataFlowTaskdelegate(string name, UltraTreeNode rootnode);

        private delegate void ValidatedataFlowTaskDelegate(TaskHost taskHost, UltraTreeNode rootNode);

        private void ValidatePackage(Package ssisParentPackage, UltraTreeNode rootnode)
        {
            try
            {
                if (form.UltraTree.InvokeRequired)
                {
                    HandleVoidPackagedelegate dlgt = ValidatePackage;
                    form.UltraTree.Invoke(dlgt, ssisParentPackage, rootnode);
                }
                else
                {
                    // Validate the layout of the package.
                    DTSExecResult status = ssisParentPackage.Validate(null, null, packageEvents, null);


                    using (
                        PackageTaskNode node =
                            new PackageTaskNode(
                                (status == DTSExecResult.Failure) ? StatusValidate.Failure : StatusValidate.Sucsess,
                                NodeClass.packageTask))
                    {
                        node.Text = "Валидация пакета" + ssisParentPackage.Name + " : " + status;
                        if (!rootnode.Nodes.Exists(node.Text))
                        {
                            rootnode.Nodes.Add(node);
                            //node.BringIntoView();
                        }

                        rootnode.ExpandAll();
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        private void SavePackage(Package ssisParentPackage, UltraTreeNode rootnode)
        {
            try
            {
                if (form.UltraTree.InvokeRequired)
                {
                    HandleVoidPackagedelegate dlgt = SavePackage;
                    form.UltraTree.Invoke(dlgt, ssisParentPackage, rootnode);
                }
                else
                {
                    // Сохранение пакета
                    Application a = new Application();
                    a.SaveToXml(Directory.GetCurrentDirectory() + "\\" + ssisParentPackage.Name + ".dtsx",
                                ssisParentPackage,
                                packageEvents);

                    using (
                        InformationNode node =
                            new InformationNode(String.Format("Сохранение пакета {0}", ssisParentPackage.Name),
                                                NodeClass.packageTask, NodeType.savepackage))
                    {
                        if (!rootnode.Nodes.Exists(node.Text))
                        {
                            rootnode.Nodes.Add(node);
                            //node.BringIntoView();
                        }
                    }

                    rootnode.ExpandAll();
                }
            }
            catch
            {
            }
        }

        private UltraTreeNode StartHandlePackage(Package package, UltraTreeNode rootnode)
        {
            try
            {
                if (form.UltraTree.InvokeRequired)
                {
                    HandlePackagedelegate dlgt = StartHandlePackage;
                    return (UltraTreeNode)form.UltraTree.Invoke(dlgt, package, rootnode);
                }
                else
                {
                    InformationNode node = new InformationNode(String.Format("Старт обработки пакета {0}", package.Name), NodeClass.packageTask, NodeType.information);
                    if (!rootnode.Nodes.Exists(node.Text))
                    {
                        rootnode.Nodes.Add(node);
                        //node.BringIntoView();

                        rootnode.ExpandAll();
                    }

                    return node;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }

        private void StartdataFlowtask(string name, UltraTreeNode rootnode)
        {
            try
            {
                if (form.UltraTree.InvokeRequired)
                {
                    DataFlowTaskdelegate dlgt = StartdataFlowtask;
                    form.UltraTree.Invoke(dlgt, name, rootnode);
                }
                else
                {
                    InformationNode node = new InformationNode(String.Format("Старт обработки задачи {0} ({1})", name, Guid.NewGuid()), NodeClass.dataflowTask, NodeType.start);
                    if (!rootnode.Nodes.Exists(node.Key))
                    {
                        rootnode.Nodes.Add(node);
                        //node.BringIntoView();

                        rootnode.ExpandAll();
                    }
                }
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
            }
        }

        private void ValidationTask(TaskHost task, UltraTreeNode rootNode)
        {
            try
            {
                if (form.UltraTree.InvokeRequired)
                {
                    ValidatedataFlowTaskDelegate dlgt = ValidationTask;
                    form.UltraTree.Invoke(dlgt, task, rootNode);
                }
                else
                {
                    DTSExecResult status = task.Validate(null, null, packageEvents, null);
                    PackageTaskNode node = new PackageTaskNode((status == DTSExecResult.Failure) ? StatusValidate.Failure : StatusValidate.Sucsess, NodeClass.dataflowTask);
                    node.Text = String.Format("Валидация задачи {0} : {1} ({2})", task.Name, status, Guid.NewGuid());

                    if (!rootNode.Nodes.Exists(node.Key))
                    {
                        rootNode.Nodes.Add(node);
                        //node.BringIntoView();

                        rootNode.ExpandAll();
                    }
                }
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
            }
        }

        #endregion

        #region CreatePackage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        private Package CreatePackage(string Name, string Description)
        {
            Package p = new Package();

            p.PackageType = DTSPackageType.DTSDesigner100;
            p.Name = Name;
            p.Description = Description;
            p.CreatorComputerName = Environment.MachineName;
            p.CreatorName = Environment.UserName;
            p.DesignEvents = packageEvents;

            AddLogging(Directory.GetCurrentDirectory() + LOGFILENAME, p);

            return p;
        }
        #endregion

        #region AddLogging
        /// <summary>
        /// Enable package level logging.
        /// </summary>
        private static void AddLogging(string Path, Package package)
        {
            try
            {
                // Add a file connection manager for the text log provider.
                ConnectionManager cm = package.Connections.Add("FILE");

                cm.ConnectionString = Path;
                cm.Name = "FileLogProviderConnection";

                // Add a LogProvider.
                LogProvider provider = package.LogProviders.Add("DTS.LogProviderTextFile.2");

                provider.ConfigString = cm.Name;
                package.LoggingOptions.SelectedLogProviders.Add(provider);

                package.LoggingOptions.EventFilterKind = DTSEventFilterKind.Inclusion;
                package.LoggingOptions.EventFilter = new String[] { "OnPreExecute", "OnPostExecute", "OnError", "OnWarning", "OnInformation", "OnPreValidate", "OnPostValidate" };
                package.LoggingMode = DTSLoggingMode.Enabled;
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine(nre.StackTrace);
            }
        }
        #endregion

        #region AddConnectionManagers
        /// <summary>
        /// Adds the OLEDB and FlatFile connection managers to the package.
        /// </summary>
        private void AddConnectionManagers(Package package)
        {
            // Подключение к источнику
            SourceConnection(package);

            // Подключение к приемнику
            DestinationConnection(package);
        }

        private static void FlatConnection(Package package)
        {
            // Add the Destination connection manager.
            ConnectionManager cmflatFile = package.Connections.Add("FLATFILE");

            // Set the stock properties.
            //cmflatFile.Properties["ConnectionString"].SetValue(cmflatFile, DestinationDataDirectory + DATAFILENAME);
            cmflatFile.Properties["Format"].SetValue(cmflatFile, "Delimited");
            cmflatFile.Properties["DataRowsToSkip"].SetValue(cmflatFile, 0);
            cmflatFile.Properties["ColumnNamesInFirstDataRow"].SetValue(cmflatFile, false);
            cmflatFile.Properties["Name"].SetValue(cmflatFile, "FlatFileConnection");
            cmflatFile.Properties["RowDelimiter"].SetValue(cmflatFile, "\r\n");
            cmflatFile.Properties["TextQualifier"].SetValue(cmflatFile, "\"");
        }

        /// <summary>
        /// Создание подключения к преемнику
        /// </summary>
        /// <param name="package"> Текущий пакет</param>
        private void DestinationConnection(Package package)
        {
            // Add the OLEDB connection manager.
            ConnectionManager dest = package.Connections.Add("OLEDB");

            // Set stock properties.
            dest.Name = "OLEDBConnectionDest";

            string connectionString = schemeDestination.SchemeDWH.OriginalConnectionString;
            destinationProvider = GetProviderType(connectionString);

            //                      string mssqlConnectionString = 
            //                          @"Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=DV;Data Source=tsvet\tsvetmssql2005;";
            //            string oracleConnectionString2 =
            //                @"Provider=OraOLEDB.Oracle.1;Password=dv;Persist Security Info=True;User ID=dv;Data Source=SSISL2;";
            dest.ConnectionString = connectionString;
        }

        /// <summary>
        /// Создание подкл.чения к источнику
        /// </summary>
        /// <param name="package"> Текущий пакет</param>
        private void SourceConnection(Package package)
        {
            // Add the OLEDB connection manager.
            ConnectionManager source = package.Connections.Add("OLEDB");

            string connectionString = schemeSource.SchemeDWH.OriginalConnectionString;

            // Set stock properties.
            source.Name = "OLEDBConnectionSource";
            //            string mssqlConnectionString = 
            //                @"Provider=SQLNCLI.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=DV;Data Source=tsvet\tsvetmssql2005;";
            //            string oracleConnectionString =
            //                String.Format(@"Provider=OraOLEDB.Oracle.1;Password=dv;Persist Security Info=True;User ID=dv;Data Source={0};", scheme.SchemeDWH.DataBaseName);
            source.ConnectionString = connectionString;
        }

        /// <summary>
        /// Определяем тип провайдера по строке подключения
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static ProviderType GetProviderType(string connectionString)
        {
            string[] parameters = connectionString.Split(';');
            string provider = string.Empty;
            foreach (string prm in parameters)
            {
                string[] keyValue = prm.Split('=');
                switch (keyValue[0])
                {
                    case "Provider":
                        provider = keyValue[1];
                        break;
                }
            }

            if (provider.StartsWith("Ora"))
                return ProviderType.Oracle;
            else
                return ProviderType.MSSQL;
        }

        #endregion

        #region AddDataFlowTask
        /// <summary>
        /// Adds a DataFlow task to the Executables collection of the package.
        /// Retrieves the MainPipe object from the TaskHost and stores it in 
        /// the dataFlow member variable
        /// </summary>
        private TaskHost AddDataFlowTask(string name, string description, IDTSSequence seq, out TaskHost task)
        {
            task = seq.Executables.Add("DTS.Pipeline") as TaskHost;
            if (task != null)
            {
                task.Name = name;
                task.Description = description;

                MainPipe dataFlow;
                dataFlow = task.InnerObject as MainPipe;
                if (dataFlow != null) dataFlow.Events = pipelineEvents as IDTSComponentEvents100;

                return task;
            }

            return null;
        }
        #endregion

        #region AddOLEDBSource
        /// <summary>
        /// Adds the OLEDB Data Source component to the DataFlow task.
        /// Creates an instance of the component.
        /// Sets the runtime connection manager.
        /// Sets two custom properties; the SqlCommand, and ValidateColumnMetaData.
        /// Acquires the connection and Reinitializes the metadata.
        /// </summary>
        private IDTSComponentMetaData100 AddOLEDBSource(SSISEntitiesObject ssisEntity, IDTSObjectHost task, Package package)
        {
            IDTSComponentMetaData100 oledbSource = ((MainPipe)task.InnerObject).ComponentMetaDataCollection.New();

            // Связываем объект с метаданными компонента с источником OLE DB
            oledbSource.ComponentClassID = "DTSAdapter.OLEDBSource";
            oledbSource.Name = "OLEDBSource";
            oledbSource.Description = "Source data in the dataFlow";

            // Создаем экземпляр источника OLE DB
            CManagedComponentWrapper instance = oledbSource.Instantiate();
            // Настраиваем объект с метаданными компонента
            instance.ProvideComponentProperties();

            // Associate the runtime connection manager
            //  The connection manager association will fail if called before ProvideComponentProperties
            oledbSource.RuntimeConnectionCollection[0].ConnectionManagerID
                = package.Connections["OLEDBConnectionSource"].ID;
            oledbSource.RuntimeConnectionCollection[0].ConnectionManager
                = DtsConvert.ToConnectionManager90(package.Connections["OLEDBConnectionSource"]);

            // Sql запрос для сопоставимого классификатора
            instance.SetComponentProperty("AccessMode", 2);

            // фиксированные классификаторы вообще не переносим,
            // а для классификаторов данных надо смотреть, если у них фиксированные строки и их не переносить,
            // иначе возникнет исключение

            string sqlquery = SQLQUERY(ssisEntity);

            instance.SetComponentProperty("SqlCommand", sqlquery);
            instance.SetComponentProperty("AlwaysUseDefaultCodePage", true);

            // Acquire Connections and reinitialize the component
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();

            instance.ReleaseConnections();

            return oledbSource;
        }

        /// <summary>
        /// Формирование SQL-запроса
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string SQLQUERY(SSISEntitiesObject ssisEntity)
        {
            IEntity entity = ssisEntity.ControlObject as IEntity;

            // SQL-запрос на выборку данных
            string sqlqwery = ssisEntity.SqlExpession.Trim(';');

            // дополнительная строка для сопоставимых классиикаторов

            if (entity.ClassType == ClassTypes.clsBridgeClassifier)
            {
                if (sqlqwery.ToUpper().Contains("WHERE"))
                    sqlqwery += " AND ID<>-1";
                else
                {
                    sqlqwery += " WHERE ID<>-1";
                }

            }

            if (entity is IClassifier)
            {
                DataTable fixedRowsTable = null;
                LogicalCallContextData contextData = LogicalCallContextData.GetContext();
                try
                {
                    LogicalCallContextData.SetContext(_context);
                    fixedRowsTable = ((IClassifier)entity).GetFixedRowsTable();
                }
                catch (Exception e)
                {
                    // throw new Exception(e.Message);
                    // Глушим ошибки, возникают при работе с таблицами с BLOB-полями
                }
                finally
                {
                    LogicalCallContextData.SetContext(contextData);
                }

                // Заглушка на фантомную строку с ID=0
                if (fixedRowsTable != null)
                {
                    if (fixedRowsTable.Rows.Count == 1 && fixedRowsTable.Rows[0]["ID"].ToString() == "0")
                        return sqlqwery;

                    foreach (DataRow row in fixedRowsTable.Rows)
                    {
                        if (!sqlqwery.ToUpper().Contains("WHERE"))
                            sqlqwery += String.Format(" WHERE ID<>{0}", row["ID"]);
                        else
                            sqlqwery += String.Format(" AND ID<>{0}", row["ID"]);
                    }
                }
            }

            return sqlqwery;
        }
        #endregion

        #region AddOLEDBSource
        /// <summary>
        /// Adds the OLEDB Data Source component to the DataFlow task.
        /// Creates an instance of the component.
        /// Sets the runtime connection manager.
        /// Sets two custom properties; the SqlCommand, and ValidateColumnMetaData.
        /// Acquires the connection and Reinitializes the metadata.
        /// </summary>
        private bool AddOLEDBDestination(IEntity entity, IDTSObjectHost task, Package package, IDTSComponentMetaData100 source)
        {
            IDTSComponentMetaData100 oledbDestination = ((MainPipe)task.InnerObject).ComponentMetaDataCollection.New();

            // Связываем объект с метаданными компонента с источником OLE DB
            oledbDestination.ComponentClassID = "DTSAdapter.OLEDBDestination";
            oledbDestination.Name = "OLEDBDestination";
            oledbDestination.Description = "Destination data in the dataFlow";

            // Создаем экземпляр источника OLE DB
            CManagedComponentWrapper instance = oledbDestination.Instantiate();
            // Настраиваем объект с метаданными компонента
            instance.ProvideComponentProperties();

            // Associate the runtime connection manager
            //  The connection manager association will fail if called before ProvideComponentProperties
            oledbDestination.RuntimeConnectionCollection[0].ConnectionManagerID
                = package.Connections["OLEDBConnectionDest"].ID;
            oledbDestination.RuntimeConnectionCollection[0].ConnectionManager
                = DtsConvert.ToConnectionManager90(package.Connections["OLEDBConnectionDest"]);

            // set custom component properties

            if (entity.ClassType == ClassTypes.clsFactData &&
                entity.Associated.Count != 0)
            {
                instance.SetComponentProperty("OpenRowset", entity.FullDBName.ToUpper());
                instance.SetComponentProperty("AccessMode", 3);

                instance.SetComponentProperty("FastLoadMaxInsertCommitSize", 1000000);
                instance.SetComponentProperty("FastLoadOptions", "TABLOCK");
                instance.SetComponentProperty("FastLoadKeepIdentity", true);
            }
            else
            {
                instance.SetComponentProperty("AccessMode", 2);
                instance.SetComponentProperty("SqlCommand", String.Format("SELECT * FROM {0}", entity.FullDBName.ToUpper()));
            }

            instance.SetComponentProperty("AlwaysUseDefaultCodePage", true);
            // Map a path between the OLE DB transformation component to the FlatFileDestination
            ((MainPipe)task.InnerObject).PathCollection.New().AttachPathAndPropagateNotifications(
                source.OutputCollection[0], oledbDestination.InputCollection[0]);
            // Acquire Connections and reinitialize the component

            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();

            MapFlatFileDestinationColumns(entity, source, oledbDestination);
            instance.ReleaseConnections();

            return true;
        }


        #endregion

        #region MapFlatFileDestination Columns
        private void MapFlatFileDestinationColumns(IEntity entity, IDTSComponentMetaData100 oleDBSource, IDTSComponentMetaData100 oleDBDestination)
        {
            //            CManagedComponentWrapper wrp = flatfileDestination.Instantiate();
            //
            //            IDTSVirtualInput90 vInput = flatfileDestination.InputCollection[0].GetVirtualInput();
            //            foreach (IDTSVirtualInputColumn90 vColumn in vInput.VirtualInputColumnCollection)
            //                wrp.SetUsageType(flatfileDestination.InputCollection[0].ID, vInput, vColumn.LineageID, DTSUsageType.UT_READONLY);
            //
            //            // For each column in the input collection
            //            // find the corresponding external metadata column.
            //            foreach (IDTSInputColumn90 col in flatfileDestination.InputCollection[0].InputColumnCollection)
            //            {
            //                IDTSExternalMetadataColumn90 exCol = flatfileDestination.InputCollection[0].ExternalMetadataColumnCollection[col.Name];
            //                wrp.MapInputColumn(flatfileDestination.InputCollection[0].ID, col.ID, exCol.ID);
            //            }

            // Такой вариант, когда будем переносить из таблицы в таблицу...а для 
            // текстового файла столбцы придеться создавать вручную

            try
            {
                IDTSOutputColumnCollection100 OleDBSourceOuputColumns =
                    oleDBSource.OutputCollection[0].OutputColumnCollection;
                IDTSExternalMetadataColumnCollection100 externalmetaDataColumns =
                    oleDBDestination.InputCollection[0].ExternalMetadataColumnCollection;

                // to Oracle
                if (schemeDestination.SchemeDWH.FactoryName == "Krista.FM.Providers.MSOracleDataAccess")
                {
                    foreach (IDTSOutputColumn100 ouputColumn in OleDBSourceOuputColumns)
                        if (ouputColumn.DataType == DataType.DT_NUMERIC)
                            ouputColumn.SetDataTypeProperties(
                                DataType.DT_DECIMAL, 0, 0, 0, ouputColumn.CodePage);
                }

                IDTSVirtualInput100 virtualInput = oleDBDestination.InputCollection[0].GetVirtualInput();

                CManagedComponentWrapper wrp = oleDBDestination.Instantiate();

                foreach (IDTSOutputColumn100 ouputColumn in OleDBSourceOuputColumns)
                {
                    // для таблиц фактов столбец ID не переносим (кроме отношения мастер-деталь)
                    if (entity.ClassType == ClassTypes.clsFactData &&
                        ouputColumn.Name.ToUpper() == "ID" &&
                        entity.Associated.Count == 0)
                        continue;

                    IDTSInputColumn100 inputColumn =
                        wrp.SetUsageType(oleDBDestination.InputCollection[0].ID, virtualInput, ouputColumn.LineageID,
                                         DTSUsageType.UT_READONLY);

                    // очень мешает различие в регистре для oracle и mssql
                    IDTSExternalMetadataColumn100 externalMetaDataColumn =
                        FindColumns(ouputColumn.Name, externalmetaDataColumns);
                    if (externalMetaDataColumn != null)
                        wrp.MapInputColumn(oleDBDestination.InputCollection[0].ID, inputColumn.ID,
                                           externalMetaDataColumn.ID);
                }
            }
            catch (COMException ex)
            {
                throw new COMException(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static IDTSExternalMetadataColumn100 FindColumns(string name, IDTSExternalMetadataColumnCollection100 columns)
        {
            foreach (IDTSExternalMetadataColumn100 column in columns)
            {
                if (column.Name.ToUpper() == name.ToUpper())
                    return column;
            }
            return null;
        }

        #endregion
    }
}