using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService;

namespace Krista.FM.Server.FinSourcePlanning.Services.DDEIndicators
{
    internal class DDEIndicatorsList : Dictionary<string, Mark>
    {
        private Dictionary<string, string> indicatorNames = new Dictionary<string, string>();
        private Dictionary<string, string> indicatorExceptions = new Dictionary<string, string>();
        private List<string> nessesaryIndicators = new List<string>();

        public DDEIndicatorsList()
        {
            // Формируем список нужных нам показателей.
            nessesaryIndicators.Add("Доходыi");
            nessesaryIndicators.Add("Текущие расходыi");
            nessesaryIndicators.Add("ДЕi");
            nessesaryIndicators.Add("СГi");
            nessesaryIndicators.Add("ДДЕi");

            // Заполняем словарь имен.
            indicatorNames.Add(nessesaryIndicators[0], "Доходы бюджета");
            indicatorNames.Add(nessesaryIndicators[1],
                               "Текущие расходы бюджета (без учета расходов на обслуживание и погашение существующих долговых обязательств)");
            indicatorNames.Add(nessesaryIndicators[2], "Долговая емкость бюджета");
            indicatorNames.Add(nessesaryIndicators[3], "Сводный график обслуживания");
            indicatorNames.Add(nessesaryIndicators[4], "Доступная долговая емкость");

        }

        /// <summary>
        /// Формирует структуру таблицы результатов.
        /// </summary>
        /// <returns>Таблица для результатов.</returns>
        private DataTable CreateResultDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn indicatorName = new DataColumn("IndicatorName", Type.GetType("System.String"));
            dt.Columns.Add(indicatorName);

            for (int year = 0; year < IndicatorsService.IndicatorsService.yearsCount; year++)
            {
                string valueColumnName = string.Format("Value{0}", year);
                DataColumn Value = new DataColumn(valueColumnName, Type.GetType("System.String"));
                dt.Columns.Add(Value);
            }
            DataColumn summ = new DataColumn("Summ", typeof (Double));
            dt.Columns.Add(summ);

            DataColumn nonAssessReason = new DataColumn("NonAssessReason", typeof (String));
            dt.Columns.Add(nonAssessReason);

            return dt;
        }

        /// <summary>
        /// Формирует строку для таблицы результатов.
        /// </summary>
        /// <param name="row">Ссылка на формируемую строку.</param>
        /// /// <param name="mark">Добавляемый индикатор.</param>
        private void MakeResultRow(DataRow row, Mark mark, double summ, string indicatorException)
        {
            string indicatorName = string.Empty;
            if (mark != null)
            {
                indicatorNames.TryGetValue(mark.Name, out indicatorName);
                row["IndicatorName"] = indicatorName;

                for (int year = 0; year < DDEIndicatorsService.yearsCount; year++)
                {
                    string valueRowName = string.Format("Value{0}", year);
                    row[valueRowName] = mark.Values[year].ToString();
                }

                row["Summ"] = summ;
            }
            row["NonAssessReason"] = string.IsNullOrEmpty(indicatorException) 
                    ? string.Empty
                    : indicatorException + Environment.NewLine;
        }

        public DataTable ToDataTable()
        {
            DataTable dt = CreateResultDataTable();

            foreach (string key in nessesaryIndicators)
            {
                DataRow row = dt.NewRow();
                string indicatorException = string.Empty;
                double summ = 0;
                if (this[key] != null)
                {
                    for (int i = 0; i < DDEIndicatorsService.yearsCount; i++)
                    {
                        summ += this[key].Values[i];
                    }
                }
                indicatorExceptions.TryGetValue(key, out indicatorException);
                MakeResultRow(row, this[key], summ, indicatorException);
                dt.Rows.Add(row);
            }
            return dt;
        }

        public void CalculateIndicatorsValues(MarksCasche marksCashe)
        {
            // И вычисляем
            foreach (string name in nessesaryIndicators)
            {
                Mark mark = null;
                try
                {
                    // Просто берем их из кэша.
                    Add(name, marksCashe.GetMark(name));
                }
                catch (Exception e)
                {
                    indicatorExceptions.Add(name, IndicatorExceptionHelper.ExceptionReason(e, name));
                    Add(name, null);
                }

            }
        }
    }
}