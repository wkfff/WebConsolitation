using System;
using System.Data;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService
{
    /// <summary>
    /// Преставляет показатели.
    /// </summary>
    public class Mark
    {
        private string name;
        private MarkHandler handler;
        private double[] values;
        private Exception exceptionWhileInitialize;

        public Mark(string markName)
        {
            name = markName;
        }

        public void Initialize(MarksCasche marksCasche, string marksClassifierName)
        {
            DataTable dtMark = GetMarkRow(marksClassifierName);
            // Пытаемся десереализовать обработчик.
            handler = (MarkHandler) XmlDeserealizeHelper.GetHandler(
                (byte[]) dtMark.Rows[0]["Handler"], typeof (MarkHandler));
            values = handler.CalculateMarkValues(marksCasche);
        }

        public string Name
        {
            get { return name; }
        }

        public double[] Values
        {
            get { return values; }
        }

        // Если при инициализации возникало исключение, запомним его.
        public Exception ExceptionWhileInitialize
        {
            get { return exceptionWhileInitialize; }
            set { exceptionWhileInitialize = value; }
        }

        /// <summary>
        /// Получает показатель запросом из соответствующего классификатора.
        /// </summary>
        /// <returns>Таблица с показателями.</returns>
        private DataTable GetMarkRow(string marksClassifierName)
        {
            DataTable dt;
            IDatabase db = null;
            try
            {
                db = FinSourcePlanningFace.Instance.Scheme.SchemeDWH.DB;
                string queryText = string.Format("select Handler from {0} where SYMBOL = ?", marksClassifierName);
                dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable,
                    db.CreateParameter("Symbol", name));
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }
    }
}
