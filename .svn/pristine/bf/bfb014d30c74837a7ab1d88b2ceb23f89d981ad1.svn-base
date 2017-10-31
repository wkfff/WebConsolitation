using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService
{
    /// <summary>
    /// Представляет кэш показателей.
    /// </summary>
    public class MarksCasche : Dictionary<string, Mark>, IDisposable
    {
        private string variantIF;
        private string variantIncome;
        private string variantIncomeYear;
        private string variantOutcome;
        private int variantBorrowID;
        private AdomdConnection adomdConnection;
        private Dictionary<string, string> marksPlacement = new Dictionary<string, string>();

        public MarksCasche(string variantIF, string variantIncome, string variantIncomeYear, string variantOutcome, int variantBorrowID, string[] classifiersNames)
        {
            this.variantIF = variantIF;
            this.variantIncome = variantIncome;
            this.variantOutcome = variantOutcome;
            this.variantBorrowID = variantBorrowID;
            this.variantIncomeYear = variantIncomeYear;
            adomdConnection = new AdomdConnection(string.Format("Provider=MSOLAP;Data Source={0};Initial Catalog={1};MDX Unique Name Style=2",
                FinSourcePlanningFace.Instance.Scheme.SchemeMDStore.ServerName, FinSourcePlanningFace.Instance.Scheme.SchemeMDStore.CatalogName));
            AdomdConnection.Open();
            FillMarksDictionary(classifiersNames);
        }

        private void FillMarksDictionary(string[] classifiersNames)
        {
            IDatabase db = FinSourcePlanningFace.Instance.Scheme.SchemeDWH.DB;
            try
            {
                for (int i = 0; i < classifiersNames.Length; i++)
                {
                    // делаем выборку по имени
                    string queryText = string.Format("select SYMBOL from {0}", classifiersNames[i]);
                    DataTable dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable);
                    foreach(DataRow row in dt.Rows)
                    {
                        marksPlacement.Add(row[0].ToString(), classifiersNames[i]);
                    }
                }
            }
            finally
            {
                db.Dispose();
            }
        }

        public AdomdConnection AdomdConnection
        {
            get { return adomdConnection; }
        }

        public string VariantIF
        {
            get { return variantIF; }
        }

        public string VariantIncome
        {
            get { return variantIncome; }
        }

        public string VariantIncomeYear
        {
            get { return variantIncomeYear; }
        }

        public string VariantOutcome
        {
            get { return variantOutcome; }
        }

        public int VariantBorrowID
        {
            get { return variantBorrowID; }
        }

        /// <summary>
        /// Возвращает показатель со значениями.
        /// </summary>
        /// <param name="markName">Имя показателя.</param>
        /// <returns>Показатель.</returns>
        public Mark GetMark(string markName)
        {
            Mark mark;
            //Если нет в кэше
            if (!ContainsKey(markName))
            {
                mark = new Mark(markName);
                try
                {   // Пытаемся проинициализировать.
                    string markClassifierName;
                    marksPlacement.TryGetValue(markName, out markClassifierName);
                    mark.Initialize(this, markClassifierName);
                }
                catch(Exception e)
                {
                    mark.ExceptionWhileInitialize = e;
                    throw;    
                }
                finally
                {
                    // добавляем в любом случае.
                    Add(markName, mark);
                }
            }
            TryGetValue(markName, out mark);
            // Если извлекли показатель с исключением, то отсылаем его.
            if (mark.ExceptionWhileInitialize != null)
                throw mark.ExceptionWhileInitialize;
            return mark;
        }

        public void Dispose()
        {
            AdomdConnection.Dispose();
        }
    }
}