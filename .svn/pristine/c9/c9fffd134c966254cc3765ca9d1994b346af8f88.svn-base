using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Globalization;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DisintRules
{
    /// <summary>
    /// Класс с данными по расщеплению 28н
    /// </summary>
    public partial class DisintRules : DisposableObject, IDisintRules
    {
        private IScheme activeScheme;

        /// <summary>
        /// Конструктор
        /// </summary>
        public DisintRules(IScheme scheme)
        {
            activeScheme = scheme;
        }

        #region Реализация интерфейса IDisintRules

        private const string mainKDSelectQuery = "select id, kd, name, year, bybudget, fed_percent, cons_percent, subj_percent, " +
            "consmo_percent, consmr_percent, mr_percent, stad_percent, go_percent, outofbudgetfond_percent " +
            ", smolenskaccount_percent, tumenaccount_percent, comments " +
            "from disintrules_kd order by id asc";
        private static string mainKDInsertQuery = "insert into disintrules_kd (id, kd, name, year, bybudget, fed_percent, cons_percent, subj_percent, consmo_percent, consmr_percent, " +
            "mr_percent, stad_percent, go_percent, outofbudgetfond_percent, smolenskaccount_percent, tumenaccount_percent, comments)" +
            "values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string mainKDUpdateQuery = "UPDATE DISINTRULES_KD SET KD = ?, NAME = ?, YEAR = ?, BYBUDGET = ?, FED_PERCENT = ?, CONS_PERCENT = ?, " +
            "SUBJ_PERCENT = ?, CONSMO_PERCENT = ?, CONSMR_PERCENT = ?, MR_PERCENT = ?, STAD_PERCENT = ?, GO_PERCENT = ?, " +
            "OUTOFBUDGETFOND_PERCENT = ?, SMOLENSKACCOUNT_PERCENT = ?, TUMENACCOUNT_PERCENT = ?, COMMENTS = ? WHERE ID = ?";
        private static string mainKDDeleteQuery = "DELETE FROM DISINTRULES_KD WHERE ID = ?";

        private static string altKDSelectQuery = "SELECT id, kd, name, comments, refdisintrules_kd FROM DISINTRULES_ALTKD order by ID asc";
        private static string altKDInsertQuery = "INSERT INTO DISINTRULES_ALTKD (ID, KD, NAME, COMMENTS, REFDISINTRULES_KD) VALUES (?, ?, ?, ?, ?)";
        private static string altKDUpdateQuery = "UPDATE DISINTRULES_ALTKD SET KD = ?, NAME = ?, COMMENTS = ?, REFDISINTRULES_KD = ? WHERE ID = ?";
        private static string altKDDeleteQuery = "DELETE FROM DISINTRULES_ALTKD WHERE ID = ?";

        private static string rules_EXSelectQuery = "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
            "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " +
            "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
            "FROM DISINTRULES_EX order by ID asc";
        private static string rules_EXInsertQuery = "INSERT INTO DISINTRULES_EX (ID, BASIC, INIT_DATE, REGION, FED_PERCENT, CONS_PERCENT, SUBJ_PERCENT, CONSMO_PERCENT, CONSMR_PERCENT, " +
            "MR_PERCENT, STAD_PERCENT, GO_PERCENT, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, COMMENTS, " +
            "REFDISINTRULES_KD) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string rules_EXUpdateQuery = "UPDATE DISINTRULES_EX SET BASIC = ?, INIT_DATE = ?, REGION = ?, FED_PERCENT = ?, CONS_PERCENT = ?, SUBJ_PERCENT = " +
            "?, CONSMO_PERCENT = ?, CONSMR_PERCENT = ?, MR_PERCENT = ?, STAD_PERCENT = ?, GO_PERCENT = ?, OUTOFBUDGETFOND_PERCENT = " +
            "?, SMOLENSKACCOUNT_PERCENT = ?, TUMENACCOUNT_PERCENT = ?, COMMENTS = ?, REFDISINTRULES_KD = ? WHERE ID = ?";
        private static string rules_EXDeleteQuery = "DELETE FROM DISINTRULES_EX WHERE ID = ?";

        private static string exPeriodSelectQuery = "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
            "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " +
            "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
            "FROM DISINTRULES_EX where basic = 1 order by ID asc";
        private static string exRegionSelectQuery = "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
            "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " +
            "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
            "FROM DISINTRULES_EX where basic = 2 order by ID asc";
        private static string exBothSelectQuery = "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
            "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " +
            "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
            "FROM DISINTRULES_EX where basic = 3 order by ID asc";

        #region Основной КД
        internal static void AppendInsertCommandParamsMainKD(Database db, IDbCommand command)
        {
            IDbDataParameter prm = null;
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("KD", DataAttributeTypes.dtString, 20);
            prm.SourceColumn = "KD";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 4000);
            prm.SourceColumn = "Name";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("YEAR", DataAttributeTypes.dtInteger, 4);
            prm.SourceColumn = "YEAR";
            command.Parameters.Add(prm);
            // название файла
            prm = db.CreateParameter("BYBUDGET", DataAttributeTypes.dtString, 1);
            prm.SourceColumn = "BYBUDGET";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("FED_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "FED_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONS_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONS_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SUBJ_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SUBJ_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("MR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "MR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("STAD_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "STAD_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("GO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "GO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("OUTOFBUDGETFOND_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "OUTOFBUDGETFOND_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SMOLENSKACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SMOLENSKACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("TUMENACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "TUMENACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("COMMENTS", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "COMMENTS";
            command.Parameters.Add(prm);
        }

        internal static void AppendUpdateCommandParamsMainKD(Database db, IDbCommand command)
        {
            IDbDataParameter prm = null;
            // Name
            prm = db.CreateParameter("KD", DataAttributeTypes.dtString, 20);
            prm.SourceColumn = "KD";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 4000);
            prm.SourceColumn = "Name";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("YEAR", DataAttributeTypes.dtInteger, 4);
            prm.SourceColumn = "YEAR";
            command.Parameters.Add(prm);
            // название файла
            prm = db.CreateParameter("BYBUDGET", DataAttributeTypes.dtString, 1);
            prm.SourceColumn = "BYBUDGET";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("FED_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "FED_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONS_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONS_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SUBJ_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SUBJ_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("MR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "MR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("STAD_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "STAD_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("GO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "GO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("OUTOFBUDGETFOND_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "OUTOFBUDGETFOND_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SMOLENSKACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SMOLENSKACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("TUMENACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "TUMENACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("COMMENTS", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "COMMENTS";
            command.Parameters.Add(prm);

            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
        }

        private IDataUpdater InitEditParamsDataAdapterMainKD(Database db)
        {
            IDbDataAdapter adapter = db.GetDataAdapter();
            adapter.SelectCommand = db.Connection.CreateCommand();
            adapter.SelectCommand.CommandText = mainKDSelectQuery;
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();
            AppendInsertCommandParamsMainKD(db, adapter.InsertCommand);
            adapter.InsertCommand.CommandText = db.GetQuery(mainKDInsertQuery, adapter.InsertCommand.Parameters);

            // команда обновления данных
            adapter.UpdateCommand = db.Connection.CreateCommand();
            AppendUpdateCommandParamsMainKD(db, adapter.UpdateCommand);
            adapter.UpdateCommand.CommandText = db.GetQuery(mainKDUpdateQuery, adapter.UpdateCommand.Parameters);

            // команда удаления данных
            adapter.DeleteCommand = db.Connection.CreateCommand();
            IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.DeleteCommand.Parameters.Add(prm);
            adapter.DeleteCommand.CommandText = db.GetQuery(mainKDDeleteQuery, adapter.DeleteCommand.Parameters);

            DataUpdater upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
            return (IDataUpdater)upd;
        }
        #endregion

        #region дополнительный КД

        internal static void AppendInsertCommandParamsAltKD(Database db, IDbCommand command)
        {
            IDbDataParameter prm = null;
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("KD", DataAttributeTypes.dtString, 20);
            prm.SourceColumn = "KD";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 4000);
            prm.SourceColumn = "Name";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("COMMENTS", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "COMMENTS";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REFDISINTRULES_KD", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "REFDISINTRULES_KD";
            command.Parameters.Add(prm);
        }

        internal static void AppendUpdateCommandParamsAltKD(Database db, IDbCommand command)
        {
            IDbDataParameter prm = null;
            // Name
            prm = db.CreateParameter("KD", DataAttributeTypes.dtString, 20);
            prm.SourceColumn = "KD";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 4000);
            prm.SourceColumn = "Name";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("COMMENTS", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "COMMENTS";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REFDISINTRULES_KD", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "REFDISINTRULES_KD";
            command.Parameters.Add(prm);

            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
        }

        private IDataUpdater InitEditParamsDataAdapterAltKD(Database db)
        {
            IDbDataAdapter adapter = db.GetDataAdapter();
            adapter.SelectCommand = db.Connection.CreateCommand();
            adapter.SelectCommand.CommandText = altKDSelectQuery;
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();
            AppendInsertCommandParamsAltKD(db, adapter.InsertCommand);
            adapter.InsertCommand.CommandText = db.GetQuery(altKDInsertQuery, adapter.InsertCommand.Parameters);

            // команда обновления данных
            adapter.UpdateCommand = db.Connection.CreateCommand();
            AppendUpdateCommandParamsAltKD(db, adapter.UpdateCommand);
            adapter.UpdateCommand.CommandText = db.GetQuery(altKDUpdateQuery, adapter.UpdateCommand.Parameters);

            // команда удаления данных
            adapter.DeleteCommand = db.Connection.CreateCommand();
            IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.DeleteCommand.Parameters.Add(prm);
            adapter.DeleteCommand.CommandText = db.GetQuery(altKDDeleteQuery, adapter.DeleteCommand.Parameters);

            DataUpdater upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
            return (IDataUpdater)upd;
        }
        #endregion

        #region исключение из КД
        internal static void AppendInsertCommandParamsExKD(Database db, IDbCommand command)
        {
            IDbDataParameter prm = null;
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("BASIC", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "BASIC";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("INIT_DATE", DataAttributeTypes.dtInteger, 8);
            prm.SourceColumn = "INIT_DATE";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("REGION", DataAttributeTypes.dtString, 20);
            prm.SourceColumn = "REGION";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("FED_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "FED_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONS_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONS_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SUBJ_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SUBJ_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("MR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "MR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("STAD_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "STAD_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("GO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "GO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("OUTOFBUDGETFOND_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "OUTOFBUDGETFOND_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SMOLENSKACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SMOLENSKACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("TUMENACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "TUMENACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("COMMENTS", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "COMMENTS";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REFDISINTRULES_KD", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "REFDISINTRULES_KD";
            command.Parameters.Add(prm);
        }

        internal static void AppendUpdateCommandParamsExKD(Database db, IDbCommand command)
        {
            IDbDataParameter prm = null;
            // Name
            prm = db.CreateParameter("BASIC", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "BASIC";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("INIT_DATE", DataAttributeTypes.dtInteger, 8);
            prm.SourceColumn = "INIT_DATE";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("REGION", DataAttributeTypes.dtString, 20);
            prm.SourceColumn = "REGION";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("FED_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "FED_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONS_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONS_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SUBJ_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SUBJ_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("CONSMR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "CONSMR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("MR_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "MR_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("STAD_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "STAD_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("GO_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "GO_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("OUTOFBUDGETFOND_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "OUTOFBUDGETFOND_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SMOLENSKACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "SMOLENSKACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("TUMENACCOUNT_PERCENT", DataAttributeTypes.dtDouble, 5);
            prm.SourceColumn = "TUMENACCOUNT_PERCENT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("COMMENTS", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "COMMENTS";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REFDISINTRULES_KD", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "REFDISINTRULES_KD";
            command.Parameters.Add(prm);

            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
        }

        private IDataUpdater InitEditParamsDataAdapterExKD(Database db, string selectQuery)
        {
            IDbDataAdapter adapter = db.GetDataAdapter();
            adapter.SelectCommand = db.Connection.CreateCommand();
            adapter.SelectCommand.CommandText = selectQuery;
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();
            AppendInsertCommandParamsExKD(db, adapter.InsertCommand);
            adapter.InsertCommand.CommandText = db.GetQuery(rules_EXInsertQuery, adapter.InsertCommand.Parameters);

            // команда обновления данных
            adapter.UpdateCommand = db.Connection.CreateCommand();
            AppendUpdateCommandParamsExKD(db, adapter.UpdateCommand);
            adapter.UpdateCommand.CommandText = db.GetQuery(rules_EXUpdateQuery, adapter.UpdateCommand.Parameters);

            // команда удаления данных
            adapter.DeleteCommand = db.Connection.CreateCommand();
            IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.DeleteCommand.Parameters.Add(prm);
            adapter.DeleteCommand.CommandText = db.GetQuery(rules_EXDeleteQuery, adapter.DeleteCommand.Parameters);

            DataUpdater upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
            return (IDataUpdater)upd;
        }
        #endregion

        /// <summary>
        /// Таблица правил расщепления
        /// </summary>
        public IDataUpdater GetDisintRules_KD()
        {/*
            IDataUpdater du;
            IDatabase db = activeScheme.SchemeDWH.DB;
            try
            {
                du = db.GetDataUpdater(selectQueryMainKD);
            }
            finally
            {
                db.Dispose();
            }
            return du;
          * */

            using (Database db = (Database)activeScheme.SchemeDWH.DB)
            {
                return InitEditParamsDataAdapterMainKD(db);
            }
        }

        /// <summary>
        /// Таблица альтернативных кодов для расщепления
        /// </summary>
        public IDataUpdater GetDisintRules_ALTKD()
        {
            /*
            IDataUpdater du;
            IDatabase db = activeScheme.SchemeDWH.DB;
            try
            {
                du = db.GetDataUpdater(altKDSelectQuery);
            }
            finally
            {
                db.Dispose();
            }
            return du;
             */
            using (Database db = (Database)activeScheme.SchemeDWH.DB)
            {
                return InitEditParamsDataAdapterAltKD(db);
            }
        }

        /// <summary>
        /// Таблица исключений
        /// </summary>
        public IDataUpdater GetDisintRules_EX()
        {
            /*
            IDataUpdater du;
            IDatabase db = activeScheme.SchemeDWH.DB;
            try
            {
                string QueryStr =
                    "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
                    "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " + 
                    "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
                    "FROM DISINTRULES_EX order by ID asc";
                du = db.GetDataUpdater(QueryStr);
            }
            finally
            {
                db.Dispose();
            }
            return du;
             */
            using (Database db = (Database)activeScheme.SchemeDWH.DB)
            {
                return InitEditParamsDataAdapterExKD(db, rules_EXSelectQuery);
            }
        }

        /// <summary>
        /// Исключения по периоду
        /// </summary>
        public IDataUpdater GetDisintRules_ExPeriod()
        {
            /*
            IDataUpdater du;
            IDatabase db = activeScheme.SchemeDWH.DB;
            try
            {
                string QueryStr =
                    "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
                    "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " + 
                    "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
                    "FROM DISINTRULES_EX where basic = 1 order by ID asc";
                du = db.GetDataUpdater(QueryStr);
            }
            finally
            {
                db.Dispose();
            }
            return du;
             * */
            using (Database db = (Database)activeScheme.SchemeDWH.DB)
            {
                return InitEditParamsDataAdapterExKD(db, exPeriodSelectQuery);
            }
        }

        /// <summary>
        /// Исключения по району
        /// </summary>
        public IDataUpdater GetDisintRules_ExRegion()
        {
            /*
            IDataUpdater du;
            IDatabase db = activeScheme.SchemeDWH.DB;
            try
            {
                string QueryStr =
                    "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
                    "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " + 
                    "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
                    "FROM DISINTRULES_EX where basic = 2 order by ID asc";
                du = db.GetDataUpdater(QueryStr);
            }
            finally
            {
                db.Dispose();
            }
            return du;
             * */
            using (Database db = (Database)activeScheme.SchemeDWH.DB)
            {
                return InitEditParamsDataAdapterExKD(db, exRegionSelectQuery);
            }
        }

        /// <summary>
        /// Исключения района по периоду
        /// </summary>
        public IDataUpdater GetDisintRules_ExBoth()
        {
            /*
            IDataUpdater du;
            IDatabase db = activeScheme.SchemeDWH.DB;
            try
            {
                string QueryStr =
                    "SELECT id, basic, init_date, region, fed_percent, cons_percent, " +
                    "subj_percent, consmo_percent, consmr_percent, mr_percent, stad_percent, " + 
                    "go_percent, OUTOFBUDGETFOND_PERCENT, SMOLENSKACCOUNT_PERCENT, TUMENACCOUNT_PERCENT, comments, refdisintrules_kd " +
                    "FROM DISINTRULES_EX where basic = 3 order by ID asc";
                du = db.GetDataUpdater(QueryStr);
            }
            finally
            {
                db.Dispose();
            }
            return du;
             * */
            using (Database db = (Database)activeScheme.SchemeDWH.DB)
            {
                return InitEditParamsDataAdapterExKD(db, exBothSelectQuery);
            }
        }

        #endregion Реализация интерфейса IDisintRules


        #region общие константы и переменные
        /// <summary>
        /// колонка со значением, которое используется для расщеспления
        /// </summary>
        private const string VALUE_POSTFIX = "_Value";
        // временная колонка, будет удалена
        private const string RESULT_VALUE_POSTFIX = "_ResultValue";
        /// <summary>
        /// колонка со значением, подтянутым из другого норматива
        /// </summary>
        private const string REF_VALUE_POSTFIX = "_RefValue";
        /// <summary>
        /// колонка, используемое в дифференцированых нормативах для хранения собственного значения
        /// </summary>
        private const string SELF_VALUE_POSTFIX = "_Self_Value";
        #endregion


        #region новые нормативы

        #region общие методы

        /// <summary>
        /// получение данных из фиксированного классификатора "уровни бюджета"
        /// </summary>
        /// <returns></returns>
        private DataTable GetBudgetLevels()
        {
            IDataUpdater upd = null;
            try
            {
                IEntity budLevelsObject = GetEntityObjectByName(NormativesKind.Unknown);
                if (budLevelsObject == null) return null;
                upd = budLevelsObject.GetDataUpdater();
                DataTable dt = new DataTable();
                upd.Fill(ref dt);
                dt.Select("ID = 0")[0].Delete();
                dt.AcceptChanges();
                return dt;
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
            }
        }

        private const string f_Norm_BK = "9573725c-76e4-421f-9ec9-ffdc0950e571";
        private const string f_Norm_Region = "5c717c31-e716-4cc3-bab1-450bc8357b3a";
        private const string f_Norm_MR = "d076de1c-9891-40d8-930a-ee43475cc9ed";
        private const string f_Norm_VariedRegion = "98e6d985-a163-47ca-bf3f-a0071cff9ff4";
        private const string f_Norm_VariedMR = "24b11318-466c-454d-ac01-c50e3a6c75a2";
        private const string fx_FX_BudgetLevels = "bd6afa07-4c81-498d-8a50-8d667179af07";
        private const string d_KD_Analysis = "2553274b-4cee-4d20-a9a6-eef173465d8b";
        private const string d_Regions_Analysis = "383f887a-3ebb-4dba-8abb-560b5777436f";

        private static IEntity normBK;
        private static IEntity normRegion;
        private static IEntity normMR;
        private static IEntity normVarRegion;
        private static IEntity normVarMR;
        private static IEntity fxBudgetLevels;

        private IEntity GetEntityObjectByName(NormativesKind normative)
        {
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                    if (normBK == null)
                        normBK = activeScheme.FactTables[f_Norm_BK];
                    return normBK;
                case NormativesKind.NormativesRegionRF:
                    if (normRegion == null)
                        normRegion = activeScheme.FactTables[f_Norm_Region];
                    return normRegion;
                case NormativesKind.NormativesMR:
                    if (normMR == null)
                        normMR = activeScheme.FactTables[f_Norm_MR];
                    return normMR;
                case NormativesKind.VarNormativesRegionRF:
                    if (normVarRegion == null)
                        normVarRegion = activeScheme.FactTables[f_Norm_VariedRegion];
                    return normVarRegion;
                case NormativesKind.VarNormativesMR:
                    if (normVarMR == null)
                        normVarMR = activeScheme.FactTables[f_Norm_VariedMR];
                    return normVarMR;
                default:
                    if (fxBudgetLevels == null)
                        fxBudgetLevels = activeScheme.Classifiers[fx_FX_BudgetLevels];
                    return fxBudgetLevels;
            }
        }

        /// <summary>
        /// добавление записи в общую таблицу нормативов
        /// </summary>
        /// <param name="normatives"></param>
        /// <param name="db"></param>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="refRegions"></param>
        /// <param name="values"></param>
        /// <param name="ids"></param>
        private void AddNewRow(DataTable normatives, IDatabase db, int refKD, int refYearDayUNV, int refRegions, Dictionary<int, object> values, Dictionary<int, int> ids)
        {
            DataRow newRow = normatives.NewRow();
            newRow.BeginEdit();
            newRow["ID"] = normatives.Rows.Count;
            newRow["RefKD"] = refKD;
            if (newRow.Table.Columns.Contains("KDCode"))
                newRow["KDCode"] = GetKDCode(refKD, db);
            newRow["RefYearDayUNV"] = refYearDayUNV;
            if (newRow.Table.Columns.Contains("RefRegions"))
                newRow["RefRegions"] = refRegions;

            // добавляем значения по нормативам
            foreach (KeyValuePair<int, object> value in values)
            {
                newRow[Convert.ToString(value.Key) + VALUE_POSTFIX] = value.Value;
                if (newRow.Table.Columns.Contains(value.Key.ToString()))
                    newRow[Convert.ToString(value.Key)] = ids[value.Key];
            }
            newRow.EndEdit();
            normatives.Rows.Add(newRow);
        }

        /// <summary>
        /// получение кода дохода по ID записи классификатора
        /// </summary>
        /// <param name="refKD"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private object GetKDCode(int refKD, IDatabase db)
        {
            string query = "select CodeStr from d_KD_Analysis where ID = ?";
            return db.ExecQuery(query, QueryResultTypes.Scalar, new DbParameterDescriptor("p0", refKD, DbType.Int32));
        }

        #region получение данных о родительской записи

        public bool GetParentRowParams(int refKD, int refYearDayUNV, NormativesKind rowNormative,
            ref int parentRefKD, ref int parentRefYearDayUNV, ref NormativesKind parentRowNormative)
        {
            IDatabase db = null;
            try
            {
                db = this.activeScheme.SchemeDWH.DB;
                return GetParentRowParams(refKD, refYearDayUNV, rowNormative, ref parentRefKD, ref parentRefYearDayUNV, ref parentRowNormative, db);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        private bool GetParentRowParams(int refKD, int refYearDayUNV, NormativesKind rowNormative,
            ref int parentRefKD, ref int parentRefYearDayUNV, ref NormativesKind parentRowNormative, IDatabase db)
        {
            string query = string.Empty;
            IDbDataParameter[] queryParams = new IDbDataParameter[2];
            queryParams[0] = db.CreateParameter("RefKD", refKD);
            queryParams[1] = db.CreateParameter("RefYearDayUNV", refYearDayUNV);
            DataTable dt = null;
            switch (rowNormative)
            {
                case NormativesKind.NormativesRegionRF:
                    query = "select RefKD, RefYearDayUNV from f_Norm_BK where refKD = ? and RefYearDayUNV = ?";
                    dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
                    parentRowNormative = NormativesKind.NormativesBK;
                    break;
                case NormativesKind.NormativesMR:
                    query = "select RefKD, RefYearDayUNV from f_Norm_Region where refKD = ? and RefYearDayUNV = ?";
                    dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
                    parentRowNormative = NormativesKind.NormativesRegionRF;
                    if (dt.Rows.Count == 0)
                    {
                        RefreshDBParams(ref queryParams, db);
                        query = "select RefKD, RefYearDayUNV from f_Norm_BK where refKD = ? and RefYearDayUNV = ?";
                        dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
                        parentRowNormative = NormativesKind.NormativesBK;
                    }
                    break;
                case NormativesKind.VarNormativesRegionRF:
                case NormativesKind.VarNormativesMR:
                    query = "select RefKD, RefYearDayUNV from F_NORM_MR where refKD = ? and RefYearDayUNV = ?";
                    dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
                    parentRowNormative = NormativesKind.NormativesMR;
                    if (dt.Rows.Count == 0)
                    {
                        RefreshDBParams(ref queryParams, db);
                        query = "select RefKD, RefYearDayUNV from F_NORM_REGION where refKD = ? and RefYearDayUNV = ?";
                        dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
                        parentRowNormative = NormativesKind.NormativesRegionRF;
                        if (dt.Rows.Count == 0)
                        {
                            RefreshDBParams(ref queryParams, db);
                            query = "select RefKD, RefYearDayUNV from f_Norm_BK where refKD = ? and RefYearDayUNV = ?";
                            dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
                            parentRowNormative = NormativesKind.NormativesBK;
                        }
                    }
                    break;
            }
            if (dt.Rows.Count == 0)
                return false;
            parentRefKD = Convert.ToInt32(dt.Rows[0]["RefKD"]);
            parentRefYearDayUNV = Convert.ToInt32(dt.Rows[0]["RefYearDayUNV"]);
            return true;
        }

        #endregion

        #region получение значения родительского норматива

        /// <summary>
        /// получает ID родительской записи
        /// </summary>
        /// <param name="refKD"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private int GetParentRefKD(int refKD, IDatabase db)
        {
            string selectRefParentQuery = "select ParentID from d_KD_Analysis where ID = ?";
            IDbDataParameter param = db.CreateParameter("ID", refKD);
            object result = db.ExecQuery(selectRefParentQuery, QueryResultTypes.Scalar, param);
            if (result != DBNull.Value)
                return Convert.ToInt32(result);
            return -1;
        }

        public decimal GetConsRegionBudget(NormativesKind normative, int refKD, int refYearDayUNV, int refBudLevel)
        {

            IDatabase db = null;
            try
            {
                decimal value = -1;
                db = internalDb == null ? activeScheme.SchemeDWH.DB : internalDb;
                value = GetConsRegionBudget(normative, refKD, refYearDayUNV, refBudLevel, db);
                while (value == -1 && GetParentRefKD(refKD, db) != -1)
                {
                    refKD = GetParentRefKD(refKD, db);
                    value = GetConsRegionBudget(normative, refKD, refYearDayUNV, refBudLevel, db);
                }
                return value;
            }
            finally
            {
                if (db != null && internalDb == null)
                    db.Dispose();
            }
        }

        private decimal GetConsRegionBudget(NormativesKind normative, int refKD, int refYearDayUNV, int refBudLevel, IDatabase db)
        {
            string year = refYearDayUNV.ToString().Substring(0, 4);

            DbParameterDescriptor[] paramDiscriptors = new DbParameterDescriptor[3];
            paramDiscriptors[0] = new DbParameterDescriptor("p0", refKD);
            paramDiscriptors[1] = new DbParameterDescriptor("p1", string.Format("{0}____", year));
            paramDiscriptors[2] = new DbParameterDescriptor("p2", refBudLevel);

            string queryNormBK = "select value from f_Norm_BK where refKD = ? and RefYearDayUNV like ? and RefBudLevel = ?";
            string queryNormRegion = "select value from f_Norm_Region where refKD = ? and RefYearDayUNV like ? and RefBudLevel = ?";
            string queryNormMR = "select value from F_NORM_MR where refKD = ? and RefYearDayUNV like ? and RefBudLevel = ?";

            object queryResult = null;
            switch (normative)
            {
                case NormativesKind.NormativesRegionRF:
                    queryResult = db.ExecQuery(queryNormBK, QueryResultTypes.Scalar, paramDiscriptors);
                    break;
                case NormativesKind.NormativesMR:
                    queryResult = db.ExecQuery(queryNormRegion, QueryResultTypes.Scalar, paramDiscriptors);
                    if (queryResult == null || queryResult is DBNull)
                    {
                        queryResult = db.ExecQuery(queryNormBK, QueryResultTypes.Scalar, paramDiscriptors);
                    }
                    break;
                case NormativesKind.VarNormativesRegionRF:
                case NormativesKind.VarNormativesMR:
                    queryResult = db.ExecQuery(queryNormMR, QueryResultTypes.Scalar, paramDiscriptors);
                    if (queryResult == null || queryResult is DBNull)
                    {
                        queryResult = db.ExecQuery(queryNormRegion, QueryResultTypes.Scalar, paramDiscriptors);
                        if (queryResult == null || queryResult is DBNull)
                        {
                            queryResult = db.ExecQuery(queryNormBK, QueryResultTypes.Scalar, paramDiscriptors);
                        }
                    }
                    break;
            }
            if (queryResult is DBNull || queryResult == null)
                return -1;

            return Convert.ToDecimal(queryResult);
        }

        #endregion

        private void RefreshDBParams(ref IDbDataParameter[] dbParams, IDatabase db)
        {
            for (int i = 0; i <= dbParams.Length - 1; i++)
            {
                dbParams[i] = db.CreateParameter(dbParams[i].ParameterName, dbParams[i].Value);
            }
        }

        private void FillAutofilledColumns(DataTable normatives, NormativesKind normativesKind, IDatabase db)
        {
            normatives.BeginLoadData();
            foreach (DataRow row in normatives.Rows)
            {
                row.BeginEdit();
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                int refRegions = Convert.ToInt32(row["RefRegions"]);
                switch (normativesKind)
                {
                    case NormativesKind.NormativesBK:
                        return;
                    case NormativesKind.NormativesRegionRF:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2);

                        break;
                    case NormativesKind.NormativesMR:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 3);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 14);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 15);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 4);
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 15);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 5);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 6);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 16);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 17);
                        break;
                    case NormativesKind.VarNormativesMR:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 3);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 14);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 15);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 4);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 6);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 16);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 17);
                        break;
                }
                // для всех одинаковые поля...
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 7);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 8);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 9);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 10);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 11);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 12);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 13);
                row.EndEdit();
            }
            normatives.EndLoadData();
        }

        private void GetAutoFilledValues(DataTable normatives, NormativesKind normativesKind, IDatabase db)
        {
            switch (normativesKind)
            {
                case NormativesKind.NormativesRegionRF:
                    foreach (DataRow row in normatives.Rows)
                    {
                        int refKD = Convert.ToInt32(row["RefKD"]);
                        int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 2);
                    }
                    break;
                case NormativesKind.NormativesMR:
                    foreach (DataRow row in normatives.Rows)
                    {
                        int refKD = Convert.ToInt32(row["RefKD"]);
                        int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 4);
                    }
                    break;
                case NormativesKind.VarNormativesMR:
                    foreach (DataRow row in normatives.Rows)
                    {
                        // получаем значения из других нормативов и считаем общие
                        int refKD = Convert.ToInt32(row["RefKD"]);
                        int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 17);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 16);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 6);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 4);
                    }
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    foreach (DataRow row in normatives.Rows)
                    {
                        int refKD = Convert.ToInt32(row["RefKD"]);
                        int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                        // получаем значения из других нормативов и считаем общие
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 17);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 16);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 6);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 5);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 15);
                        FillCellValue(row, normativesKind, refKD, refYearDayUNV, 2);
                    }
                    break;
            }
        }

        /// <summary>
        /// заполнение поля - ссылки на другой норматив в нормативах для интерфейса
        /// </summary>
        /// <param name="row"></param>
        /// <param name="normative"></param>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="refBudLevel"></param>
        private void FillCellValue(DataRow row, NormativesKind normative, int refKD, int refYearDayUNV, int refBudLevel)
        {
            decimal cellValue = GetConsRegionBudget(normative, refKD, refYearDayUNV, refBudLevel);
            string dataValueColumnName = string.Format("{0}{1}", refBudLevel, VALUE_POSTFIX);
            string filledValueColumnName = string.Format("{0}{1}", refBudLevel, REF_VALUE_POSTFIX);
            string resultValueColumnName = string.Format("{0}{1}", refBudLevel, RESULT_VALUE_POSTFIX);

            if (row.Table.Columns.Contains(resultValueColumnName))
            {
                if (cellValue >= 0)
                {
                    row[filledValueColumnName] = cellValue;
                    row[resultValueColumnName] = Convert.ToDecimal(row[dataValueColumnName]) + cellValue;
                }
                else
                {
                    row[filledValueColumnName] = 0;
                    row[resultValueColumnName] = Convert.ToDecimal(row[dataValueColumnName]);
                }
            }
            else
            {
                if (cellValue >= 0)
                    row[dataValueColumnName] = cellValue;
                else
                    row[dataValueColumnName] = 0;
            }
        }

        private IDatabase internalDb;

        /// <summary>
        /// заполнения поля - ссылки на другой норматив в нормативах для расщепления
        /// </summary>
        /// <param name="row"></param>
        /// <param name="normative"></param>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="refBudLevel"></param>
        private void NewFillCellValue(DataRow row, NormativesKind normative, int refKD, int refYearDayUNV, int refBudLevel)
        {
            decimal cellValue = GetConsRegionBudget(normative, refKD, refYearDayUNV, refBudLevel);
            string dataValueColumnName = string.Format("{0}{1}", refBudLevel, VALUE_POSTFIX);
            string filledValueColumnName = string.Format("{0}{1}", refBudLevel, REF_VALUE_POSTFIX);
            string selfValueColumn = string.Format("{0}{1}", refBudLevel, SELF_VALUE_POSTFIX);

            if (row.Table.Columns.Contains(selfValueColumn))
            {
                if (cellValue >= 0)
                {
                    row[filledValueColumnName] = cellValue;
                    row[dataValueColumnName] = Convert.ToDecimal(row[dataValueColumnName]) + cellValue;
                }
                else
                {
                    row[filledValueColumnName] = 0;
                    row[dataValueColumnName] = Convert.ToDecimal(row[dataValueColumnName]);
                }
            }
            else
            {
                if (cellValue >= 0)
                    row[dataValueColumnName] = cellValue;
                else
                    row[dataValueColumnName] = 0;
            }
        }


        #endregion

        #region реализация новых методов интерфейса
        /// <summary>
        /// получение нормативов
        /// </summary>
        /// <param name="normatives"></param>
        /// <returns></returns>
        public DataTable GetNormatives(NormativesKind normatives)
        {
            DataTable dtNormatives = InnerGetNormatives(normatives);
            // умножение значений нормативов на 100
            dtNormatives.BeginLoadData();
            foreach (DataRow row in dtNormatives.Rows)
            {
                foreach (DataColumn column in dtNormatives.Columns)
                {
                    if (column.ColumnName.Contains(VALUE_POSTFIX) ||
                        column.ColumnName.Contains(RESULT_VALUE_POSTFIX) ||
                        column.ColumnName.Contains(REF_VALUE_POSTFIX))
                    {
                        decimal testP = Convert.ToDecimal(row[column]);
                        row[column] = testP * 100;
                    }
                    if (string.Compare(column.ColumnName, "RefYearDayUNV", true) == 0)
                        row[column] = Convert.ToInt32(row[column]) / 10000;
                }
            }
            dtNormatives.EndLoadData();
            dtNormatives.AcceptChanges();
            return dtNormatives;
        }

        public bool ApplyChanges(NormativesKind normatives, DataTable changes)
        {
            if (changes == null)
                return true;

            changes.BeginLoadData();
            foreach (DataRow row in changes.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    foreach (DataColumn column in changes.Columns)
                    {
                        if (column.ColumnName.Contains(VALUE_POSTFIX))
                        {
                            decimal rowValue = Convert.ToDecimal(row[column]);
                            row[column] = Convert.ToDecimal(rowValue / 100);
                        }
                        if (string.Compare(column.ColumnName, "RefYearDayUNV", true) == 0)
                            row[column] = Convert.ToInt32(row[column]) * 10000;
                    }
                }
            }
            changes.EndLoadData();
            return InnerApplyChanges(changes, normatives);
        }

        public int GetNormativeRowsCount(NormativesKind normatives)
        {
            using (IDatabase db = this.activeScheme.SchemeDWH.DB)
            {
                string countQuery = "select count(ID) from {0}";
                switch (normatives)
                {
                    case NormativesKind.NormativesBK:
                        countQuery = string.Format(countQuery, "F_NORM_BK");
                        break;
                    case NormativesKind.NormativesMR:
                        countQuery = string.Format(countQuery, "F_NORM_MR");
                        break;
                    case NormativesKind.NormativesRegionRF:
                        countQuery = string.Format(countQuery, "F_NORM_REGION");
                        break;
                    case NormativesKind.VarNormativesMR:
                        countQuery = string.Format(countQuery, "F_NORM_VARIEDMR");
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        countQuery = string.Format(countQuery, "F_NORM_VARIEDREGION");
                        break;
                }
                int count = Convert.ToInt32(db.ExecQuery(countQuery, QueryResultTypes.Scalar));
                return count;
            }
        }

        public DataTable GetSourcesTable(List<int> sourcesIDs)
        {
            DataTable dtSources = CreateDataSourceTable();
            foreach (int sourceID in sourcesIDs)
            {
                IDataSource dataSource = activeScheme.DataSourceManager.DataSources[sourceID];
                if (dataSource != null)
                    dtSources.Rows.Add(dataSource.ID, dataSource.SupplierCode, dataSource.DataCode, dataSource.DataName,
                        dataSource.ParametersType.ToString(), dataSource.BudgetName, dataSource.Year, dataSource.Month,
                        dataSource.Variant, dataSource.Quarter, dataSource.Territory, activeScheme.DataSourceManager.GetDataSourceName(sourceID));
            }
            return dtSources;
        }

        #region получение нормативов для расщепления

        public DataTable GetNewNormatives(NormativesKind normatives)
        {
            using (IDatabase db = activeScheme.SchemeDWH.DB)
            {
                return InnerGetNormativesForSplit(normatives, db);
            }
        }

        private DataTable InnerGetNormativesForSplit(NormativesKind normatives, IDatabase db)
        {
            // создаем таблицу для хранения нормативов в том виде, в котором они будут отправляться на клиент
            DataTable dtNormatives = CreateNewNormativesTable(normatives);
            // получаем нормативы из таблицы фактов
            DataTable crudeNormatives = GetCrudeNormatives(normatives, db);
            crudeNormatives.BeginLoadData();
            // если нормативов нету, выходим
            if (crudeNormatives == null) return null;
            if (crudeNormatives.Rows.Count == 0) return null;
            // преобразуем нормативы в нормальные
            Dictionary<string, UniqueReferencesSet> references = new Dictionary<string, UniqueReferencesSet>();
            object refKD = crudeNormatives.Rows[0]["RefKD"];
            object refYearDayUNV = crudeNormatives.Rows[0]["RefYearDayUNV"];
            object refRegions = crudeNormatives.Rows[0]["RefRegions"];
            if (refRegions == DBNull.Value)
                refRegions = -1;
            references.Add(string.Format("{0}_{1}_{2}", refKD, refYearDayUNV, refRegions), new UniqueReferencesSet(refKD, refYearDayUNV, refRegions));
            // получим разные наборы ссылок на КД и год
            foreach (DataRow row in crudeNormatives.Select(string.Empty, string.Empty))
            {
                refKD = row["RefKD"];
                refYearDayUNV = row["RefYearDayUNV"];
                refRegions = row["RefRegions"];
                if (refRegions == DBNull.Value)
                    refRegions = -1;
                string key = string.Format("{0}_{1}_{2}", refKD, refYearDayUNV, refRegions);
                if (!references.ContainsKey(key))
                    references.Add(key, new UniqueReferencesSet(refKD, refYearDayUNV, refRegions));
            }

            Dictionary<int, object> values = new Dictionary<int, object>();
            Dictionary<int, int> ids = new Dictionary<int, int>();

            foreach (UniqueReferencesSet reference in references.Values)
            {
                DataRow[] newRow = null;
                if (Convert.ToInt32(reference.RefRegions) != -1)
                    newRow = crudeNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions = {2}",
                        reference.RefKD, reference.RefYearDayUNV, reference.RefRegions));
                else
                    newRow = crudeNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions is null",
                        reference.RefKD, reference.RefYearDayUNV));
                foreach (DataRow row in newRow)
                {
                    if (!values.ContainsKey(Convert.ToInt32(row["RefBudLevel"])))
                    {
                        values.Add(Convert.ToInt32(row["RefBudLevel"]), row["value"]);
                        ids.Add(Convert.ToInt32(row["RefBudLevel"]), Convert.ToInt32(row["ID"]));
                    }
                }
                AddNewRow(dtNormatives, db, Convert.ToInt32(reference.RefKD), Convert.ToInt32(reference.RefYearDayUNV),
                    Convert.ToInt32(reference.RefRegions), values, ids);
                values.Clear();
                ids.Clear();
            }

            crudeNormatives.EndLoadData();

            FillAutofilledColumns(dtNormatives, normatives, db);

            dtNormatives.AcceptChanges();
            return dtNormatives;
        }

        /// <summary>
        /// возвращает нормативы из таблицы фактов
        /// </summary>
        /// <param name="normatives"></param>
        /// <returns></returns>
        private DataTable GetCrudeNormatives(NormativesKind normatives, IDatabase db)
        {
            string query = string.Empty;
            DataTable dtCrudeNormatives = null;
            switch (normatives)
            {
                case NormativesKind.NormativesBK:
                    query = "select id, RefKD, RefYearDayUNV, RefBudLevel, value from f_Norm_BK";
                    dtCrudeNormatives = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                    break;
                case NormativesKind.NormativesRegionRF:
                    query = "select id, RefKD, RefYearDayUNV, RefBudLevel, value from f_Norm_Region";
                    dtCrudeNormatives = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                    break;
                case NormativesKind.NormativesMR:
                    query = "select id, RefKD, RefYearDayUNV, RefBudLevel, value from f_Norm_MR";
                    dtCrudeNormatives = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    query = "select id, RefKD, RefYearDayUNV, RefRegions, RefBudLevel, value from F_NORM_VARIEDREGION";
                    dtCrudeNormatives = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                    break;
                case NormativesKind.VarNormativesMR:
                    query = "select id, RefKD, RefYearDayUNV, RefRegions, RefBudLevel, value from F_NORM_VARIEDMR";
                    dtCrudeNormatives = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                    break;
            }
            if (dtCrudeNormatives != null && !dtCrudeNormatives.Columns.Contains("RefRegions"))
            {
                DataColumn column = dtCrudeNormatives.Columns.Add("RefRegions");
                column.DefaultValue = -1;
            }
            return dtCrudeNormatives;
        }

        /// <summary>
        /// создает общую таблицу для получения нормативов
        /// </summary>
        /// <param name="normatives"></param>
        /// <returns></returns>
        private DataTable CreateNewNormativesTable(NormativesKind normatives)
        {
            DataTable table = new DataTable();
            // поля под хранение ссылок на классификаторы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("RefKD", typeof(int));
            table.Columns.Add("KDCode", typeof(string));
            table.Columns.Add("RefYearDayUNV", typeof(int));
            table.Columns.Add("RefRegions", typeof(int));
            // поля под хранение значений нормативов
            DataColumn column = table.Columns.Add("1", typeof(int));
            column = table.Columns.Add("1" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("2" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("3" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("14" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF)
            {
                column = table.Columns.Add("15" + SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("15" + REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("15" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("4" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF)
            {
                column = table.Columns.Add("5" + SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("5" + REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("5" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;


            if (normatives == NormativesKind.VarNormativesRegionRF || normatives == NormativesKind.VarNormativesMR)
            {
                column = table.Columns.Add("6" + SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("6" + REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("6" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF || normatives == NormativesKind.VarNormativesMR)
            {
                column = table.Columns.Add("16" + SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("16" + REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("16" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF || normatives == NormativesKind.VarNormativesMR)
            {
                column = table.Columns.Add("17" + SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("17" + REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }

            column = table.Columns.Add("17" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("7" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("8" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("9" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("10" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("11" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("12" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("13" + VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;
            return table;
        }

        #endregion


        #endregion

        #region общие методы получения и сохранения нормативов

        private DataTable InnerGetNormatives(NormativesKind normatives)
        {
            using (IDatabase db = activeScheme.SchemeDWH.DB)
            {

                //db.ExecQuery("delete from f_Norm_BK", QueryResultTypes.NonQuery);
                /*
                db.ExecQuery("delete from f_Norm_Region", QueryResultTypes.NonQuery);
                db.ExecQuery("delete from f_Norm_MR", QueryResultTypes.NonQuery);
                db.ExecQuery("delete from F_NORM_VARIEDREGION", QueryResultTypes.NonQuery);
                db.ExecQuery("delete from F_NORM_VARIEDMR", QueryResultTypes.NonQuery);
                */
                // получаем данные из таблицы фактов для нормативов
                DataTable dtCrudeNormatives = GetCrudeNormatives(normatives, db);
                // создаем структуру таблицы
                DataTable dtNormatives = new DataTable();
                dtNormatives.Columns.Add("ID", typeof(int));
                dtNormatives.Columns.Add("RefKD", typeof(int));
                dtNormatives.Columns.Add("RefYearDayUNV", typeof(int));
                DataColumn column = dtNormatives.Columns.Add("RefRegions", typeof(int));
                if (normatives == NormativesKind.NormativesBK || normatives == NormativesKind.NormativesRegionRF || normatives == NormativesKind.NormativesMR)
                    column.DefaultValue = -1;
                AddColumnsToTable(dtNormatives, normatives);
                // тут и так все понятно, нет данных, возвращаем пустую таблицу
                if (dtCrudeNormatives.Rows.Count == 0)
                    return dtNormatives;
                // данные из фиксированного классификатора по уровням бюджета
                DataTable dtBudgetLevesl = GetBudgetLevels();

                // получение списка уникальных наборов ссылок на классификаторы с целью получения строк норматива
                Dictionary<string, UniqueReferencesSet> uniqueReferencesSet = new Dictionary<string, UniqueReferencesSet>();
                object refKD = dtCrudeNormatives.Rows[0]["RefKD"];
                object refYearDayUNV = dtCrudeNormatives.Rows[0]["RefYearDayUNV"];
                object refRegions = dtCrudeNormatives.Rows[0]["RefRegions"];
                if (refRegions == DBNull.Value)
                    refRegions = -1;
                uniqueReferencesSet.Add(string.Format("{0}_{1}_{2}", refKD, refYearDayUNV, refRegions), new UniqueReferencesSet(refKD, refYearDayUNV, refRegions));
                foreach (DataRow row in dtCrudeNormatives.Rows)
                {
                    refKD = row["RefKD"];
                    refYearDayUNV = row["RefYearDayUNV"];
                    refRegions = row["RefRegions"];
                    if (refRegions == DBNull.Value)
                        refRegions = -1;
                    string key = string.Format("{0}_{1}_{2}", refKD, refYearDayUNV, refRegions);
                    if (!uniqueReferencesSet.ContainsKey(key))
                        uniqueReferencesSet.Add(key, new UniqueReferencesSet(refKD, refYearDayUNV, refRegions));
                }

                Dictionary<int, object> values = new Dictionary<int, object>();
                Dictionary<int, int> ids = new Dictionary<int, int>();

                foreach (UniqueReferencesSet referenceSet in uniqueReferencesSet.Values)
                {
                    DataRow[] newRow = null;
                    if (Convert.ToInt32(referenceSet.RefRegions) != -1)
                        newRow = dtCrudeNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions = {2}",
                            referenceSet.RefKD, referenceSet.RefYearDayUNV, referenceSet.RefRegions));
                    else
                        newRow = dtCrudeNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions is null",
                            referenceSet.RefKD, referenceSet.RefYearDayUNV));
                    foreach (DataRow row in newRow)
                    {
                        // убираем возможные дубликаты 
                        if (!values.ContainsKey(Convert.ToInt32(row["RefBudLevel"])))
                        {
                            values.Add(Convert.ToInt32(row["RefBudLevel"]), row["value"]);
                            ids.Add(Convert.ToInt32(row["RefBudLevel"]), Convert.ToInt32(row["ID"]));
                        }
                        else
                        {
                            DataRow delRow = dtCrudeNormatives.Select(string.Format("ID = {0}", row["ID"]))[0];
                            delRow.Delete();
                        }
                    }
                    AddNewRow(dtNormatives, db, Convert.ToInt32(referenceSet.RefKD), Convert.ToInt32(referenceSet.RefYearDayUNV),
                        Convert.ToInt32(referenceSet.RefRegions), values, ids);
                    values.Clear();
                    ids.Clear();
                }
                internalDb = db;
                GetAutoFilledValues(dtNormatives, normatives, db);
                dtNormatives.AcceptChanges();
                if (normatives == NormativesKind.NormativesBK || normatives == NormativesKind.NormativesRegionRF || normatives == NormativesKind.NormativesMR)
                {
                    dtNormatives.Columns.Remove(column);
                }
                internalDb = null;
                return dtNormatives;
            }
        }

        private bool InnerApplyChanges(DataTable changes, NormativesKind normatives)
        {
            bool needRefresh = false;

            using (IDatabase db = this.activeScheme.SchemeDWH.DB)
            {
                DataTable dt = GetBudgetLevels();

                IEntity normativeEnttity = GetEntityObjectByName(normatives);
                if (normativeEnttity == null)
                    return false;

                int refKD = 0;
                int refYearDayUNV = 0;
                int refRegions = -1;
                bool isRegions = changes.Columns.Contains("RefRegions");

                foreach (DataRow row in changes.Rows)
                {
                    // записи общие, выделяем из них отдельные
                    if (row.RowState != DataRowState.Deleted)
                    {
                        // для не удаленных записей получаем общие парамер параметры
                        refKD = Convert.ToInt32(row["RefKD"]);
                        refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                        if (isRegions)
                            refRegions = Convert.ToInt32(row["RefRegions"]);
                    }
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            // добавляем все записи что есть в общей
                            foreach (DataRow budgetLevelRow in dt.Rows)
                            {
                                // находим записи
                                // получим ID новой записи
                                int refBudLevel = Convert.ToInt32(budgetLevelRow["ID"]);
                                if (IsBudgetLevelInNormative(normatives, refBudLevel))
                                {
                                    // сохраняем норматив, который ввели 
                                    object value = row[Convert.ToString(budgetLevelRow["ID"]) + VALUE_POSTFIX];
                                    InsertNewRow(normativeEnttity.FullDBName, refKD, refYearDayUNV, refRegions, refBudLevel, value, db, normatives);
                                }
                            }
                            needRefresh = true;
                            break;
                        case DataRowState.Deleted:
                            // удаляем все записи, что есть в общей
                            foreach (DataRow budgetLevelRow in dt.Rows)
                            {
                                int refBudLevel = Convert.ToInt32(budgetLevelRow["ID"]);
                                if (IsBudgetLevelInNormative(normatives, refBudLevel))
                                {
                                    DataColumn column = row.Table.Columns[Convert.ToString(budgetLevelRow["ID"])];
                                    // если нет id значит нет записи в таблице фактов. Пропускаем 
                                    if (row.IsNull(column, DataRowVersion.Original))
                                        continue;

                                    int id = Convert.ToInt32(row[column, DataRowVersion.Original]);
                                    DeleteRow(normativeEnttity.FullDBName, id, db);
                                }
                            }
                            break;
                        case DataRowState.Modified:
                            // изменяем все записи, что есть в общей
                            foreach (DataRow budgetLevelRow in dt.Rows)
                            {
                                int refBudLevel = Convert.ToInt32(budgetLevelRow["ID"]);
                                if (IsBudgetLevelInNormative(normatives, refBudLevel))
                                {

                                    int id = Convert.ToInt32(row[Convert.ToString(budgetLevelRow["ID"])]);
                                    object value = row[Convert.ToString(budgetLevelRow["ID"]) + VALUE_POSTFIX];
                                    if (refRegions != -1)
                                        UpdateRow(normativeEnttity.FullDBName, id, refKD, refYearDayUNV, refRegions, value, db);
                                    else
                                        UpdateRow(normativeEnttity.FullDBName, id, refKD, refYearDayUNV, value, db);
                                }
                            }
                            break;
                    }
                }
                return needRefresh;
            }
        }

        private bool IsBudgetLevelInNormative(NormativesKind normative, int budgetLevel)
        {
            // не рассматриваем уровни бюджета отличные от тех, по которым есть нормативы
            if (budgetLevel > 17)
                return false;
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                    return true;
                case NormativesKind.NormativesRegionRF:
                case NormativesKind.VarNormativesRegionRF:
                    if (budgetLevel == 3 || budgetLevel == 14 ||
                        budgetLevel == 15 || budgetLevel == 4 || budgetLevel == 5 ||
                        budgetLevel == 6 || budgetLevel == 16 || budgetLevel == 17)
                        return true;
                    else
                        return false;
                case NormativesKind.NormativesMR:
                case NormativesKind.VarNormativesMR:
                    if (budgetLevel == 5 || budgetLevel == 6 ||
                        budgetLevel == 16 || budgetLevel == 17)
                        return true;
                    else
                        return false;
            }
            return false;
        }

        /// <summary>
        /// добавляем колонки в таблицу в таком порядке, в каком они должны быть на клиенте
        /// </summary>
        /// <param name="table"></param>
        /// <param name="normative"></param>
        private void AddColumnsToTable(DataTable table, NormativesKind normative)
        {
            DataColumn column = null;
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                    column = table.Columns.Add(Convert.ToString(1), typeof(int));
                    column = table.Columns.Add(Convert.ToString(1) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(2), typeof(int));
                    column = table.Columns.Add(Convert.ToString(2) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(3), typeof(int));
                    column = table.Columns.Add(Convert.ToString(3) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(14), typeof(int));
                    column = table.Columns.Add(Convert.ToString(14) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(15), typeof(int));
                    column = table.Columns.Add(Convert.ToString(15) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(4), typeof(int));
                    column = table.Columns.Add(Convert.ToString(4) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(5), typeof(int));
                    column = table.Columns.Add(Convert.ToString(5) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(6), typeof(int));
                    column = table.Columns.Add(Convert.ToString(6) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(16), typeof(int));
                    column = table.Columns.Add(Convert.ToString(16) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(17), typeof(int));
                    column = table.Columns.Add(Convert.ToString(17) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(7), typeof(int));
                    column = table.Columns.Add(Convert.ToString(7) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(8), typeof(int));
                    column = table.Columns.Add(Convert.ToString(8) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(9), typeof(int));
                    column = table.Columns.Add(Convert.ToString(9) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(10), typeof(int));
                    column = table.Columns.Add(Convert.ToString(10) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(11), typeof(int));
                    column = table.Columns.Add(Convert.ToString(11) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(12), typeof(int));
                    column = table.Columns.Add(Convert.ToString(12) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(13), typeof(int));
                    column = table.Columns.Add(Convert.ToString(13) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    break;
                case NormativesKind.NormativesRegionRF:
                    column = table.Columns.Add(Convert.ToString(2), typeof(int));
                    column = table.Columns.Add(Convert.ToString(2) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(3), typeof(int));
                    column = table.Columns.Add(Convert.ToString(3) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(14), typeof(int));
                    column = table.Columns.Add(Convert.ToString(14) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(15), typeof(int));
                    column = table.Columns.Add(Convert.ToString(15) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(4), typeof(int));
                    column = table.Columns.Add(Convert.ToString(4) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(5), typeof(int));
                    column = table.Columns.Add(Convert.ToString(5) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(6), typeof(int));
                    column = table.Columns.Add(Convert.ToString(6) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(16), typeof(int));
                    column = table.Columns.Add(Convert.ToString(16) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(17), typeof(int));
                    column = table.Columns.Add(Convert.ToString(17) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    break;
                case NormativesKind.NormativesMR:
                    column = table.Columns.Add(Convert.ToString(4), typeof(int));
                    column = table.Columns.Add(Convert.ToString(4) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(5), typeof(int));
                    column = table.Columns.Add(Convert.ToString(5) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(6), typeof(int));
                    column = table.Columns.Add(Convert.ToString(6) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(16), typeof(int));
                    column = table.Columns.Add(Convert.ToString(16) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(17), typeof(int));
                    column = table.Columns.Add(Convert.ToString(17) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    column = table.Columns.Add(Convert.ToString(2), typeof(int));
                    column = table.Columns.Add(Convert.ToString(2) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(3), typeof(int));
                    column = table.Columns.Add(Convert.ToString(3) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(14), typeof(int));
                    column = table.Columns.Add(Convert.ToString(14) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(15) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(15) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(15), typeof(int));
                    column = table.Columns.Add(Convert.ToString(15) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(4), typeof(int));
                    column = table.Columns.Add(Convert.ToString(4) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(5) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(5) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(5), typeof(int));
                    column = table.Columns.Add(Convert.ToString(5) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(6) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(6) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(6), typeof(int));
                    column = table.Columns.Add(Convert.ToString(6) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(16) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(16) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(16), typeof(int));
                    column = table.Columns.Add(Convert.ToString(16) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(17) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(17) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(17), typeof(int));
                    column = table.Columns.Add(Convert.ToString(17) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    break;
                case NormativesKind.VarNormativesMR:
                    column = table.Columns.Add(Convert.ToString(4), typeof(int));
                    column = table.Columns.Add(Convert.ToString(4) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(5), typeof(int));
                    column = table.Columns.Add(Convert.ToString(5) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(6) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(6) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(6), typeof(int));
                    column = table.Columns.Add(Convert.ToString(6) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(16) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(16) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(16), typeof(int));
                    column = table.Columns.Add(Convert.ToString(16) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;

                    column = table.Columns.Add(Convert.ToString(17) + RESULT_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(17) + REF_VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    column = table.Columns.Add(Convert.ToString(17), typeof(int));
                    column = table.Columns.Add(Convert.ToString(17) + VALUE_POSTFIX, typeof(decimal));
                    column.DefaultValue = 0;
                    break;
            }
        }

        #endregion

        #region работа с записями нормативов

        private void GetKDCodeNameFromId(int id, ref string kd, ref string name, IDatabase db)
        {
            string selectQuery = "select CodeStr, Name from d_KD_Analysis where ID = ?";
            IDbDataParameter param = db.CreateParameter("ID", id);
            DataTable dt = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable, param);
            kd = Convert.ToString(dt.Rows[0]["CodeStr"]);
            name = Convert.ToString(dt.Rows[0]["Name"]);
        }

        private string GetRegionNameFromID(int id, IDatabase db)
        {
            string selectQuery = "select Name from d_Regions_Analysis where ID = ?";
            IDbDataParameter param = db.CreateParameter("ID", id);
            object name = db.ExecQuery(selectQuery, QueryResultTypes.Scalar, param);
            return Convert.ToString(name);
        }

        /// <summary>
        /// обновляем запись
        /// </summary>
        /// <param name="id"></param>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="value"></param>
        private void UpdateRow(string normativeDBName, object id, int refKD, int refYearDayUNV, object value, IDatabase db)
        {
            string updateQuery = string.Format("update {0} set value = ?, refYearDayUNV = ?, refkd = ? where id = ?", normativeDBName);
            IDbDataParameter[] queryParams = new IDbDataParameter[4];
            queryParams[0] = db.CreateParameter("value", value, DbType.Decimal);
            queryParams[1] = db.CreateParameter("refYearDayUNV", refYearDayUNV);
            queryParams[2] = db.CreateParameter("refkd", refKD);
            queryParams[3] = db.CreateParameter("id", id);
            db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, queryParams);
        }

        /// <summary>
        /// обновляем запись со ссылкой на классификатор районов
        /// </summary>
        /// <param name="normativeDBName"></param>
        /// <param name="id"></param>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="refRegions"></param>
        /// <param name="value"></param>
        /// <param name="db"></param>
        private void UpdateRow(string normativeDBName, object id, int refKD, int refYearDayUNV, int refRegions, object value, IDatabase db)
        {
            string updateQuery = string.Format("update {0} set value = ?, refYearDayUNV = ?, refkd = ?, refregions = ? where id = ?", normativeDBName);
            IDbDataParameter[] queryParams = new IDbDataParameter[5];
            queryParams[0] = db.CreateParameter("value", value, DbType.Decimal);
            queryParams[1] = db.CreateParameter("refYearDayUNV", refYearDayUNV);
            queryParams[2] = db.CreateParameter("refkd", refKD);
            queryParams[3] = db.CreateParameter("refRegions", refRegions);
            queryParams[4] = db.CreateParameter("id", id);
            db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, queryParams);
        }

        /// <summary>
        /// удаляем запись
        /// </summary>
        /// <param name="id"></param>
        private void DeleteRow(string normativeDBName, int id, IDatabase db)
        {
            string deleteQuery = string.Format("delete from {0} where id = ?", normativeDBName);
            IDbDataParameter[] queryParams = new IDbDataParameter[1];
            queryParams[0] = db.CreateParameter("id", id);
            db.ExecQuery(deleteQuery, QueryResultTypes.NonQuery, queryParams);
        }

        /// <summary>
        /// источник данных по нормативам
        /// </summary>
        private int normativeSourceID = -1;

        /// <summary>
        /// вставляем новую запись
        /// </summary>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="refBudLevel"></param>
        /// <param name="value"></param>
        private void InsertNewRow(string normativeDBName, int refKD, int refYearDayUNV, int refRegions, int refBudLevel, object value, IDatabase db, NormativesKind normative)
        {
            if (normativeSourceID == -1)
                normativeSourceID = GetNormativesDataSource();

            string insertQuery = string.Empty;
            IDbDataParameter[] queryParams = null;
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                case NormativesKind.NormativesMR:
                case NormativesKind.NormativesRegionRF:
                    insertQuery = string.Format("insert into {0} (sourceid, taskid, value, refYearDayUNV, RefBudLevel, refkd) values (?, ?, ?, ?, ?, ?)", normativeDBName);
                    queryParams = new IDbDataParameter[6];
                    break;
                case NormativesKind.VarNormativesMR:
                case NormativesKind.VarNormativesRegionRF:
                    insertQuery = string.Format("insert into {0} (sourceid, taskid, value, refYearDayUNV, RefBudLevel, refkd, refRegions) values (?, ?, ?, ?, ?, ?, ?)", normativeDBName);
                    queryParams = new IDbDataParameter[7];
                    break;
            }

            queryParams[0] = db.CreateParameter("sourceid", normativeSourceID);
            queryParams[1] = db.CreateParameter("taskid", -1);
            queryParams[2] = db.CreateParameter("value", value);
            queryParams[3] = db.CreateParameter("refYearDayUNV", refYearDayUNV);
            queryParams[4] = db.CreateParameter("RefBudLevel", refBudLevel);
            queryParams[5] = db.CreateParameter("refkd", refKD);
            if (queryParams.Length == 7)
                queryParams[6] = db.CreateParameter("refregions", refRegions);
            db.ExecQuery(insertQuery, QueryResultTypes.NonQuery, queryParams);
        }

        private int GetNormativesDataSource()
        {
            IDataSourceManager sourceManager = this.activeScheme.DataSourceManager;
            IDataSource ds = sourceManager.DataSources.CreateElement();
            ds.SupplierCode = "ФО";
            ds.DataCode = "0023";
            ds.DataName = "Нормативы отчислений";
            ds.ParametersType = ParamKindTypes.WithoutParams;
            if (!sourceManager.DataSources.Contains(ds))
                // если источника нету, то добавляем его
                return sourceManager.DataSources.Add(ds);
            else
                // если есть, то палучаем его ID
                return Convert.ToInt32(sourceManager.DataSources.FindDataSource(ds));
        }

        #endregion

        #region дополнительные действия при импорте - экспотре

        private DataTable CreateDataSourceTable()
        {
            DataTable dtSource = new DataTable();
            dtSource.Columns.Add("id", typeof(Int32));
            dtSource.Columns.Add("suppplierCode", typeof(String));
            dtSource.Columns.Add("dataCode", typeof(String));
            dtSource.Columns.Add("dataName", typeof(String));
            dtSource.Columns.Add("kindsOfParams", typeof(String));
            dtSource.Columns.Add("name", typeof(String));
            dtSource.Columns.Add("year", typeof(Int32));
            dtSource.Columns.Add("month", typeof(Int32));
            dtSource.Columns.Add("variant", typeof(String));
            dtSource.Columns.Add("quarter", typeof(Int32));
            dtSource.Columns.Add("territory", typeof(String));
            dtSource.Columns.Add("dataSourceNameValue", typeof(String));
            return dtSource;
        }

        public Dictionary<int, int> GetNewClassifiersRef(DataTable sources, DataTable classifiersRows,
            string classifierName)
        {
            IDatabase db = null;
            try
            {
                db = activeScheme.SchemeDWH.DB;
                IEntity classifier = activeScheme.Classifiers[classifierName];

                Dictionary<object, int> newSourcesID = new Dictionary<object, int>();
                foreach (DataRow dataSource in sources.Rows)
                {
                    newSourcesID.Add(dataSource["ID"], AddDataSource(dataSource));
                }
                Dictionary<int, int> classifiersNewIDs = new Dictionary<int, int>();
                string codeCoulumnName = string.Empty;
                if (classifier.FullName.Contains("KD"))
                    codeCoulumnName = "CodeStr";
                else
                    codeCoulumnName = "Code";
                foreach (DataRow classifierRow in classifiersRows.Rows)
                {
                    string query = string.Format("select ID from {0} where {1} = ? and Name = ? and SourceID = {2}",
                        classifier.FullDBName, codeCoulumnName, newSourcesID[classifierRow["SourceID"]]);
                    IDbDataParameter[] queryParams = new IDbDataParameter[2];
                    queryParams[0] = db.CreateParameter(codeCoulumnName, classifierRow[codeCoulumnName]);
                    queryParams[1] = db.CreateParameter("Name", classifierRow["Name"]);
                    object rowID = db.ExecQuery(query, QueryResultTypes.Scalar, queryParams);
                    if (!classifiersNewIDs.ContainsKey(Convert.ToInt32(classifierRow["ID"])))
                    {
                        if (rowID == null || rowID == DBNull.Value)
                        {
                            // для разных классификаторов разный набор параметров
                            queryParams = new IDbDataParameter[classifier.Attributes.Count];
                            string[] columnsNames = new string[classifier.Attributes.Count];
                            string[] signs = new string[classifier.Attributes.Count];
                            int i = 0;
                            foreach (IDataAttribute attr in classifier.Attributes.Values)
                            {
                                if (attr.Name == "ID")
                                {
                                    int id = classifier.GetGeneratorNextValue;
                                    queryParams[i] = db.CreateParameter(attr.Name, id);
                                    classifiersNewIDs.Add(Convert.ToInt32(classifierRow["ID"]), id);
                                }
                                else if (attr.Name == "SourceID")
                                {
                                    queryParams[i] = db.CreateParameter(attr.Name, newSourcesID[classifierRow["SourceID"]]);
                                }
                                else if (attr.Name == codeCoulumnName)
                                    queryParams[i] = db.CreateParameter(attr.Name, classifierRow[codeCoulumnName]);
                                else if (attr.Name == "Name")
                                    queryParams[i] = db.CreateParameter(attr.Name, classifierRow["Name"]);
                                else
                                {
                                    if (attr.DefaultValue != null)
                                        queryParams[i] = db.CreateParameter(attr.Name, attr.DefaultValue);
                                    else
                                        queryParams[i] = db.CreateParameter(attr.Name, DBNull.Value);
                                }
                                columnsNames[i] = attr.Name;
                                signs[i] = "?";
                                i++;
                            }
                            query = string.Format("insert into {0} ({1}) values ({2})",
                                classifier.FullDBName, string.Join(", ", columnsNames), string.Join(", ", signs));
                            db.ExecQuery(query, QueryResultTypes.NonQuery, queryParams);
                        }
                        else
                            classifiersNewIDs.Add(Convert.ToInt32(classifierRow["ID"]), Convert.ToInt32(rowID));
                    }
                }
                return classifiersNewIDs;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// проверяет, существует ли источник данных из XML
        /// </summary>
        /// <returns>true, если источник существует</returns>
        private int AddDataSource(DataRow dataSource)
        {
            IDataSourceManager sourceManager = this.activeScheme.DataSourceManager;
            IDataSource ds = sourceManager.DataSources.CreateElement();
            ds.BudgetName = dataSource["Name"].ToString();
            ds.DataCode = dataSource["DataCode"].ToString();
            ds.DataName = dataSource["DataName"].ToString();
            ds.Month = Convert.ToInt32(dataSource["Month"]);
            ds.ParametersType = GetParamsType(dataSource["kindsOfParams"].ToString());
            ds.Quarter = Convert.ToInt32(dataSource["Quarter"]);
            ds.SupplierCode = dataSource["SuppplierCode"].ToString();
            ds.Territory = dataSource["Territory"].ToString();
            ds.Variant = dataSource["Variant"].ToString();
            ds.Year = Convert.ToInt32(dataSource["Year"]);
            if (!sourceManager.DataSources.Contains(ds))
                // если источника нету, то добавляем его
                return sourceManager.DataSources.Add(ds);
            else
                // если есть, то палучаем его ID
                return Convert.ToInt32(sourceManager.DataSources.FindDataSource(ds));
        }

        /// <summary>
        /// получение из строки типа параметров источника
        /// </summary>
        /// <param name="kindsOfParams"></param>
        /// <returns></returns>
        private static ParamKindTypes GetParamsType(string kindsOfParams)
        {
            switch (kindsOfParams)
            {
                case "YearVariant":
                    return ParamKindTypes.YearVariant;
                case "YearTerritory":
                    return ParamKindTypes.YearTerritory;
                case "YearQuarterMonth":
                    return ParamKindTypes.YearQuarterMonth;
                case "YearQuarter":
                    return ParamKindTypes.YearQuarter;
                case "YearMonthVariant":
                    return ParamKindTypes.YearMonthVariant;
                case "YearMonth":
                    return ParamKindTypes.YearMonth;
                case "Year":
                    return ParamKindTypes.Year;
                case "WithoutParams":
                    return ParamKindTypes.WithoutParams;
                case "Budget":
                    return ParamKindTypes.Budget;
                case "Variant":
                    return ParamKindTypes.Variant;
                case "YearVariantMonthTerritory":
                    return ParamKindTypes.YearVariantMonthTerritory;
            }
            return ParamKindTypes.NoDivide;
        }

        #endregion

        #endregion

        #region перенос нормативов из блока "Фонды"

        public void FundDataTransfert(int fundVariant, int fundMarks, string kdCode, string kdName, ref string messages)
        {
            var normativesService = new NormativesService(activeScheme, 0);
            normativesService.NormativeTransfert(kdCode, kdName, fundVariant, fundMarks, ref messages);
        }

        #endregion

    }

    #region внутренние объекты

    internal class UniqueReferencesSet
    {
        private object _refKD;
        private object _refYearDayUNV;
        private object _refRegions;

        internal UniqueReferencesSet(object refKD, object refYearDayUNV, object refRegions)
        {
            _refKD = refKD;
            _refYearDayUNV = refYearDayUNV;
            _refRegions = refRegions;
        }

        public object RefKD
        {
            get { return _refKD; }
            set { _refKD = value; }
        }

        public object RefYearDayUNV
        {
            get { return _refYearDayUNV; }
            set { _refYearDayUNV = value; }
        }

        public object RefRegions
        {
            get { return _refRegions; }
            set { _refRegions = value; }
        }
    }

    #endregion
}
