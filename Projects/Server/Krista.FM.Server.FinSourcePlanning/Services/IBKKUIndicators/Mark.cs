using System;
using System.Data;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators
{
    /// <summary>
    /// ����������� ����������.
    /// </summary>
    public class Mark
    {
        /// <summary>
        /// ��������������� ������������� �����������.
        /// </summary>
        private const string marksClassifierName = "d_marks_estimatesdata";

        private string name;
        private MarkHandler handler;
        private double[] values;
        private Exception exceptionWhileInitialize;

        public Mark(string markName)
        {
            name = markName;
        }

        public void Initialize(string variantIF, string variantIncome, string variantOutcome, int variantBorrowID, MarksCasche marksCasche)
        {
            DataTable dtMark = GetMarkRow();
            // �������� ��������������� ����������.
            handler = (MarkHandler) XmlDeserealizeHelper.GetHandler(
                (byte[]) dtMark.Rows[0]["Handler"], typeof (MarkHandler));
            values = handler.CalculateMarkValues(variantIF, variantIncome, variantOutcome, variantBorrowID, marksCasche);
        }

        public string Name
        {
            get { return name; }
        }

        public double[] Values
        {
            get { return values; }
        }

        // ���� ��� ������������� ��������� ����������, �������� ���.
        public Exception ExceptionWhileInitialize
        {
            get { return exceptionWhileInitialize; }
            set { exceptionWhileInitialize = value; }
        }

        /// <summary>
        /// �������� ���������� �������� �� ���������������� ��������������.
        /// </summary>
        /// <returns>������� � ������������.</returns>
        private DataTable GetMarkRow()
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
