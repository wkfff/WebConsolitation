using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators
{
    class IndicatorsList : List<Indicator>
    {
        private const string indicatorsClassifierName = "d_marks_estimates";

        /// <summary>
        /// Формирует структуру таблицы результатов.
        /// </summary>
        /// <returns>Таблица для результатов.</returns>
        private DataTable CreateResultDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn indicatorName = new DataColumn("IndicatorName", Type.GetType("System.String"));
            DataColumn caption = new DataColumn("Caption", Type.GetType("System.String"));
            DataColumn formula = new DataColumn("Formula", Type.GetType("System.String"));
            DataColumn comment = new DataColumn("Comment", Type.GetType("System.String"));
            DataColumn assessinonType = new DataColumn("AssessionType", typeof(AssessionType));
            DataColumn assessCrit = new DataColumn("AssessCrit", Type.GetType("System.String"));
            DataColumn nonAssessReason = new DataColumn("NonAssessReason", typeof(List<string>));

            dt.Columns.Add(indicatorName);
            dt.Columns.Add(caption);
            dt.Columns.Add(formula);
            dt.Columns.Add(comment);
            dt.Columns.Add(assessinonType);
            dt.Columns.Add(assessCrit);
            dt.Columns.Add(nonAssessReason);

            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
            {
                string valueColumnName = string.Format("Value{0}", year);
                string assessColumnName = string.Format("Assess{0}", year);
                string formulaValueColumnName = string.Format("FormulaValue{0}", year);
                DataColumn Value = new DataColumn(valueColumnName, Type.GetType("System.String"));
                DataColumn FormulaValue = new DataColumn(formulaValueColumnName, Type.GetType("System.String"));
                DataColumn Assess = new DataColumn(assessColumnName, Type.GetType("System.Double"));
                dt.Columns.Add(Value);
                dt.Columns.Add(FormulaValue);
                dt.Columns.Add(Assess);
            }
            return dt;
        }

        /// <summary>
        /// Формирует строку для таблицы результатов.
        /// </summary>
        /// <param name="row">Ссылка на формируемую строку.</param>
        /// /// <param name="indicator">Добавляемый индикатор.</param>
        private void MakeResultRow(DataRow row, Indicator indicator)
        {
            row["IndicatorName"] = indicator.Name;
            row["Caption"] = indicator.Caption;
            row["Formula"] = indicator.Formula;
            row["Comment"] = indicator.Comment;
            row["AssessionType"] = indicator.Handler.assessionType;
            row["AssessCrit"] = indicator.AssessCritToString();
            row["NonAssessReason"] = indicator.NonAssessReason;

            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
            {
                string valueRowName = string.Format("Value{0}", year);
                string formulaValueRowName = string.Format("FormulaValue{0}", year);
                string assessRowName = string.Format("Assess{0}", year);
                row[valueRowName] = indicator.Handler.assessionType != AssessionType.NonAssessed ?
                    indicator.ResultData[year].value.ToString() : "не определено";
                row[formulaValueRowName] = indicator.FormulaValues[year];
                row[assessRowName] = indicator.ResultData[year].assession;
            }
        }

        /// <summary>
        /// Получает индикаторы запросом из соответствующего классификатора.
        /// </summary>
        /// <returns>Таблица с индикаторами.</returns>
        private DataTable GetIndicators()
        {
            DataTable dt;
            IDatabase db = null;
            try
            {
                db = FinSourcePlanningFace.Instance.Scheme.SchemeDWH.DB;
                string indicatorsQueryText = string.Format(
                    "select SYMBOL, NAME, FORMULA, MEANING, THRESHOLD1, THRESHOLD2, HANDLER from {0}", indicatorsClassifierName);
                dt = (DataTable) db.ExecQuery(indicatorsQueryText, QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }
        
        public DataTable ToDataTable()
        {
            DataTable dt = CreateResultDataTable();
            foreach (Indicator indicator in this)
            {
                DataRow row = dt.NewRow();
                MakeResultRow(row, indicator);
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// Загружает в список индикаторов данные из БД.
        /// </summary>
        public void LoadIndicatorsData()
        {
            DataTable dtIndicators = GetIndicators();
            foreach (DataRow row in dtIndicators.Rows)
            {
                Add(new Indicator(row));   
            }
        }
    }
}