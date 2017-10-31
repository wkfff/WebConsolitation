using System.Collections.Generic;
using System.Data;
using Krista.FM.Common;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;
using NCalc;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    /// <summary>
    /// Сервис для пересчета платежей договора.
    /// </summary>
    public class ChangesCalcService : IChangesCalcService
    {
        /// <summary>
        /// Пересчет всех платежей договора.
        /// </summary>
        /// <param name="entity">Сущность договора.</param>
        /// <param name="documentId">Id договора.</param>
        /// <param name="formules">Список формул.</param>
        public void Recalc(IEntity entity, int documentId, Dictionary<string, ColumnState> formules)
        {
            using (var du = entity.GetDataUpdater("ParentId = ? or ID = ?", null, new DbParameterDescriptor("parentId", documentId), new DbParameterDescriptor("documentId", documentId)))
            {
                DataTable data = new DataTable();
                du.Fill(ref data);

                if (data.Rows.Count == 0)
                {
                    return;
                }

                // Перебираем все строки в порядке возрастания даты платежа
                DataRow parentRow = data.Select("ID = {0}".FormatWith(documentId))[0];
                DataRow prevRow = null;
                DataRow[] dataRows = data.Select("ParentId = {0}".FormatWith(documentId), "ChargeDate");
                for (var i = 0; i < dataRows.GetLength(0); i++)
                {
                    DataRow row = dataRows[i];
                    RecalcRow(row, prevRow, parentRow, formules);
                    prevRow = row;
                }

                du.Update(ref data);
            }
        }

        private void RecalcRow(DataRow row, DataRow prevRow, DataRow parentRow, Dictionary<string, ColumnState> formules)
        {
            foreach (var formula in formules)
            {
                var eval = new FormulaEvaluter(formula.Value.CalcFormula, formula.Key, row, prevRow, parentRow);
                eval.Calc();
            }
        }

        private class FormulaEvaluter
        {
            private readonly string formula;
            private readonly string resultColumn;
            private readonly DataRow row;
            private readonly DataRow prevRow;
            private readonly DataRow parentRow;

            public FormulaEvaluter(string formula, string resultColumn, DataRow row, DataRow prevRow, DataRow parentRow)
            {
                this.formula = formula;
                this.resultColumn = resultColumn;
                this.row = row;
                this.prevRow = prevRow;
                this.parentRow = parentRow;
            }

            public void Calc()
            {
                Expression exp = new Expression(formula);
                exp.EvaluateParameter += OnEvaluateParameter;
                exp.EvaluateFunction += OnEvaluateFunction;
                row[resultColumn] = exp.Evaluate();
            }

            private void OnEvaluateFunction(string name, FunctionArgs args)
            {
                if (name.ToUpper() == "PREV")
                {
                    if (prevRow != null)
                    {
                        args.Result = prevRow[((NCalc.Domain.Identifier)args.Parameters[0].ParsedExpression).Name];
                    }
                    else
                    {
                        args.Result = parentRow[((NCalc.Domain.Identifier)args.Parameters[1].ParsedExpression).Name];
                    }
                }
                else
                {
                    args.HasResult = false;
                }
            }

            private void OnEvaluateParameter(string name, ParameterArgs args)
            {
                args.Result = row[name];
            }
        }
    }
}
