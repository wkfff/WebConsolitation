using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class FixedRowsModificationItem : ModificationItem
    {
        /// <summary>
        /// Запрос модифицирующий базу данных
        /// </summary>
        private string sqlModificationQuery = String.Empty;

        /// <summary>
        /// Конфигурация фиксированных значений
        /// </summary>
        private string xmlConfigiration = String.Empty;

        public FixedRowsModificationItem(string name, object fromObject, object toObject)
            : base(ModificationTypes.Modify, name, fromObject, toObject, null)
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

        /// <summary>
        /// Конфигурация фиксированных значений
        /// </summary>
        public string XmlConfigiration
        {
            get { return xmlConfigiration; }
            set { xmlConfigiration = value; }
        }

        protected override void OnBeforeChildApplay(ModificationContext context)
        {
            base.OnBeforeChildApplay(context);

            if (!String.IsNullOrEmpty(SqlModificationQuery))
            {
                Trace.WriteLine(SqlModificationQuery);
                Database db = SchemeClass.Instance.UpdateDatabase;
                db.ExecQuery(SqlModificationQuery, QueryResultTypes.NonQuery);
            }
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            base.OnAfterChildApplay(context);

            if (!String.IsNullOrEmpty(XmlConfigiration))
            {
                ((Classifier)FromObject).SetFixedRowsXmlConfigiration(XmlConfigiration);
                ((Classifier)FromObject).SaveConfigurationToDatabase();
            }
        }
    }
}
