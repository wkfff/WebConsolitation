using System;
using System.Data;

using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class DataRowModificationItem : ModificationItem
    {
		internal delegate void ModificationItemAfterApplay(ModificationItem sender, ModificationContext context);
		
		internal event ModificationItemAfterApplay AfterApplay;

        /// <summary>
        /// Запрос модифицирующий базу данных
        /// </summary>
        private string sqlModificationQuery = String.Empty;
        
        /// <summary>
        /// Параметры запроса.
        /// </summary>
        private IDbDataParameter[] parameters;

        /// <summary>
        /// Конфигурация фиксированных значений
        /// </summary>
        private string xmlConfigiration = String.Empty;

        public DataRowModificationItem(ModificationTypes type, string name, object fromObject, object toObject)
            : base(type, name, fromObject, toObject, null)
        {
        }

        /// <summary>
        /// Запрос модифицирующий базу данных
        /// </summary>
        internal string SqlModificationQuery
        {
            get { return sqlModificationQuery; }
            set { sqlModificationQuery = value; }
        }

        public override int ImageIndex
        {
            get
            {
                return 57;
            }
        }

        /// <summary>
        /// Параметры запроса.
        /// </summary>
        internal IDbDataParameter[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// Конфигурация фиксированных значений
        /// </summary>
        public string XmlConfigiration
        {
            get { return xmlConfigiration; }
            set { xmlConfigiration = value; }
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            if (!String.IsNullOrEmpty(SqlModificationQuery))
            {
                Trace.WriteLine(SqlModificationQuery);
                context.Database.ExecQuery(SqlModificationQuery, QueryResultTypes.NonQuery, Parameters);
            }
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            base.OnAfterChildApplay(context);

            if (!String.IsNullOrEmpty(XmlConfigiration))
            {
                ((Classifier)FromObject).SetFixedRowsXmlConfigiration(XmlConfigiration);
                ((Classifier)FromObject).SaveConfigurationToDatabase(context);
            }

			if (AfterApplay != null)
				AfterApplay(this, context);
        }
    }
}
