using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    //***************************************************************************************
    // FMQ00005624 Борисов 30.03.2007
    //
    // Специфическая обработка для 1НМ - добавление и расчет дополнительных уровней бюджета.
    // Cодержит дублирование кода из основного модуля (SummCorrection.cs). 
    // Вообще всю коррекцию сумм нужно переписывать. 
    // В том виде в котором она находитcя сейчас, код совершенно не поддается ни пониманию,
    // ни модификации.
    //
    //***************************************************************************************
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region Для отладки
        private void SaveParent2ChildCls(Dictionary<int, List<int>> parent2ChildCls)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, List<int>> item in parent2ChildCls)
            {
                sb.AppendLine(Convert.ToString(item.Key));
                foreach (int child in item.Value)
                {
                    sb.AppendLine("    " + child);
                }
                File.WriteAllText("C:\\_parent2ChildCls.txt", sb.ToString(), Encoding.GetEncoding(1251));
            }
        }

        private string DataRowToString(DataRow row)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object val in row.ItemArray)
            {
                sb.Append(Convert.ToString(val));
                sb.Append(", ");
            }
            return sb.ToString();
        }

        private void SaveFactCashe(Dictionary<int, Dictionary<int, List<DataRow>>> cache)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, Dictionary<int, List<DataRow>>> item in cache)
            {
                sb.AppendLine(Convert.ToString(item.Key));
                foreach (KeyValuePair<int, List<DataRow>> item2 in item.Value)
                {
                    sb.AppendLine("    " + Convert.ToString(item2.Key));
                    foreach (DataRow row in item2.Value)
                    {
                        sb.AppendLine("        " + DataRowToString(row));
                    }
                }
            }
            File.WriteAllText("C:\\_factCache.txt", sb.ToString(), Encoding.GetEncoding(1251));
        }
        #endregion

        private readonly int[] BudgetLevels_7_DependsIDs = { 8, 9, 10, 11 };
        private readonly int[] BudgetLevels_0_DependsIDs = { 1, 2, 8, 9, 10, 11 };
        private readonly int[] BudgetLevels_3_DependsIDs = { 2, 14 };

        private double GetChildSumForBudgetLevel(List<DataRow> factRows, int budgetLevelID, string sumField)
        {
            double sum = 0;
            foreach (DataRow row in factRows)
            {
                int curBudgetLevel = Convert.ToInt32(row["RefBudgetLevels"]);
                if (curBudgetLevel == budgetLevelID)
                    sum += GetDoubleCellValue(row, sumField, 0);
            }
            return sum;
        }

        private void CorrectSumForBudgetLevel(List<DataRow> factRows, int budgetLevelID, int[] dependIDs, 
            DataTable factTable, bool sumAfterFirstMember, string sumField)
        {
            double sum = 0;
            for (int i = 0; i < dependIDs.Length; i++)
            {
                int dependID = dependIDs[i];
                if (sumAfterFirstMember)
                {
                    sum += GetChildSumForBudgetLevel(factRows, dependID, sumField);
                }
                else
                {
                    if (i == 0)
                        sum = GetChildSumForBudgetLevel(factRows, dependID, sumField);
                    else
                        sum -= GetChildSumForBudgetLevel(factRows, dependID, sumField);
                }
            }
            // если сумма не равна 0 - добавляем уровень
            if (sum != 0)
                AddBudgetLevel(budgetLevelID, factTable, factRows, sumField, sum);
        }

        protected void Calc1NMAdditionalBudgetLevels(IFactTable fct, DataTable clsTable, IClassifier cls,
            string factRefToCls, BlockProcessModifier blockProcessModifier, string[] multiClsCorrFields,
            string regionRefFieldName, string bdgtLevelsRefFieldName, string sumField)
        {
            string factSemantic = fct.FullCaption;
            string clsSemantic = cls.FullCaption;

            WriteToTrace(string.Format(
                "Коррекция сумм {0} по классификатору {1}...", factSemantic, clsSemantic), TraceMessageKind.Information);

            // Запрос данных фактов
            SetProgress(0, 0, string.Format("Запрос данных {0}...", factSemantic), string.Empty);

            if (clsTable.Rows.Count == 0)
            {
                WriteToTrace("Классификатор пуст", TraceMessageKind.Warning);
                return;
            }

            // Запрашиваем данные фактов
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
            DataTable factTable = ds.Tables[0];
            if (factTable.Rows.Count == 0)
            {
                WriteToTrace("Отсутствуют данные фактов.", TraceMessageKind.Warning);
                return;
            }
            // Заполняем кэш корректируемых данных
            // Структура кэша:
            // Ключ - ИД района, Значение - данные фактов по району.
            // Структура данных фактов по району:
            // Ключ - ИД классификатора, Значение - список строк по этому классификатору.
            Dictionary<int, Dictionary<int, List<DataRow>>> factCache =
                new Dictionary<int, Dictionary<int, List<DataRow>>>(50);
            FillCorrectedSumsCache(ds.Tables[0], regionRefFieldName, factRefToCls, factCache);

            try
            {
                SetProgress(0, 0, string.Format(
                    "Расчет дополнительных уровней бюджета {0} по классификатору {1}...", factSemantic, clsSemantic), string.Empty);

                // Идем по датасету классификатора районов
                int i = 0;
                foreach (KeyValuePair<int, Dictionary<int, List<DataRow>>> kvp in factCache)
                {
                    i++;
                    SetProgress(factCache.Count, i,
                        string.Format("Расчет дополнительных уровней бюджета {0} по классификатору {1}...", factSemantic, clsSemantic),
                        string.Format("Район {0} из {1}", i, factCache.Count), true);
                    // идем по КД
                    // добавление фиктивных уровней
                    foreach (KeyValuePair<int, List<DataRow>> parentKD in kvp.Value)
                    {
                        CorrectSumForBudgetLevel(parentKD.Value, 7, BudgetLevels_7_DependsIDs, factTable, true, sumField);
                        CorrectSumForBudgetLevel(parentKD.Value, 0, BudgetLevels_0_DependsIDs, factTable, true, sumField);
                        CorrectSumForBudgetLevel(parentKD.Value, 3, BudgetLevels_3_DependsIDs, factTable, false, sumField);
                    }
                }

                UpdateDataSet(da, ds, fct);
                ClearDataSet(ref ds);

                SetProgress(-1, -1,
                    string.Format("Расчет дополнительных уровней бюджета {0} по классификатору {1} закончена.",
                    factSemantic, clsSemantic), string.Empty, true);
                WriteToTrace(string.Format("Расчет дополнительных уровней бюджета {0} по классификатору {1} закончена.",
                    factSemantic, clsSemantic), TraceMessageKind.Information);
            }
            finally
            {
                if (factCache != null)
                {
                    factCache.Clear();
                    factCache = null;
                }
            }
        }

        private DataRow GetRowWithBudgetLevel(List<DataRow> rows, int budgetLevelID)
        {
            foreach (DataRow row in rows)
            {
                int curLevel = Convert.ToInt32(row["RefBudgetLevels"]);
                if (curLevel == budgetLevelID)
                    return row;
            }
            return null;
        }

        private void CopyRowToRowNoId(DataRow sourceRow, DataRow destRow)
        {
            if (sourceRow == null || destRow == null)
                return;
            for (int i = 0; i < destRow.Table.Columns.Count; i++)
            {
                string columnName = destRow.Table.Columns[i].ColumnName;
                if (columnName.ToUpper() == "ID")
                    continue;
                if (sourceRow.Table.Columns.Contains(columnName))
                    destRow[i] = sourceRow[columnName];
            }
        }

        private void AddBudgetLevel(int budgetLevelId, DataTable factTable, List<DataRow> factRows, string sumField, double sum)
        {
            DataRow budgetLevelRow = GetRowWithBudgetLevel(factRows, budgetLevelId);
            if (budgetLevelRow == null)
            {
                budgetLevelRow = factTable.NewRow();
                // копируем поля первой попавшейся строки (???)
                CopyRowToRowNoId(factRows[0], budgetLevelRow);
                // меняем нужны поля
                //budgetLevelRow["ID"] = DBNull.Value;
                budgetLevelRow["RefBudgetLevels"] = budgetLevelId;
                budgetLevelRow["Earned"] = DBNull.Value;
                budgetLevelRow["EarnedReport"] = DBNull.Value;
                budgetLevelRow["Inpayments"] = DBNull.Value;
                budgetLevelRow["InpaymentsReport"] = DBNull.Value;
                // еще есть непонятное поле SourceKey, оставим его пока как есть
                factTable.Rows.Add(budgetLevelRow);
                factRows.Add(budgetLevelRow);
            }
            budgetLevelRow[sumField] = sum;
        }
    }
}