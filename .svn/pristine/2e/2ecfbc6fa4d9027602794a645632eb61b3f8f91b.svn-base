using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Common.Validations;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations
{
    /// <summary>
    /// проверка суммы фактической к сумме запланированной по отдельной записи
    /// </summary>
    public class MasterDetailDetailSumValidator : AbstractMasterDetailDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable, DataTable boundDetailTable)
        {
            ValidationMessages vmh = new ValidationMessages();

            foreach (DataRow boundRow in boundDetailTable.Rows)
            {
                DataRow[] rows = detailTable.Select(string.Format("PlanPayment = {0}", boundRow["Payment"]));
                if (rows != null && rows.Length > 0)
                {
                    Decimal detailSum = 0;
                    Decimal boundDetailSum = Convert.ToDecimal(boundRow["Sum"]);
                    List<int> ids = new List<int>();
                    foreach (DataRow detailRow in rows)
                    {
                        detailSum += Convert.ToDecimal(detailRow["Sum"]);
                        ids.Add(Convert.ToInt32(detailRow["ID"]));
                    }
                    if (detailSum < boundDetailSum)
                    {
                        ValidationMessage msg = new UltraGridColumnCellsValidationMessage(ids, "Sum", Association);
                        string[] strRowsID = new string[ids.Count];
                        for(int i = 0; i <= ids.Count - 1; i++)
                        {
                            strRowsID[i] = ids[i].ToString();
                        }
                        msg.Summary = string.Format(ErrorMessage, string.Join(", ", strRowsID));
                        msg.HasError = true;
                        vmh.Add(msg);
                    }
                }
            }
            return vmh;
        }
    }

    /// <summary>
    /// проверка общей суммы фактической к общей сумме запланированной
    /// </summary>
    public class MasterDetailDetailTotalSumValidator : AbstractMasterDetailDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable, DataTable boundDetailTable)
        {
            ValidationMessage vmh = null;
            Decimal detailTotalSum = 0;
            Decimal boundDetailTotalSum = 0;
            foreach (DataRow boundRow in boundDetailTable.Rows)
            {
                boundDetailTotalSum += Convert.ToDecimal(boundRow["Sum"]);
            }
            foreach (DataRow detailRow in detailTable.Rows)
            {
                detailTotalSum += Convert.ToDecimal(detailRow["Sum"]);
            }
            if (detailTable.Rows.Count > 0 && boundDetailTable.Rows.Count > 0 && detailTotalSum != boundDetailTotalSum)
            {
                vmh = new UltraGridColumnValidationMessage("Sum", Association);
                vmh.Summary = ErrorMessage;//"Задолженность по процентам погашена не полностью";
                vmh.HasError = true;
            }
            return vmh;
        }
    }

    /// <summary>
    /// проверка даты на вхождение в диапазон дат из другой детали
    /// </summary>
    public class MasterDetailDetailDateValidator : AbstractMasterDetailDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable, DataTable boundDetailTable)
        {
            ValidationMessages vmh = new ValidationMessages();

            foreach (DataRow boundRow in boundDetailTable.Rows)
            {
                DataRow[] rows = detailTable.Select(string.Format("PlanPayment = {0}", boundRow["Payment"]));
                if (rows != null && rows.Length > 0)
                {
                    DateTime boundStartDate = Convert.ToDateTime(boundRow["StartDate"]);
                    string boundDetailEndDateColumnKey = "EndDate";
                    if (!boundDetailTable.Columns.Contains(boundDetailEndDateColumnKey))
                        boundDetailEndDateColumnKey = "LateDate";
                    DateTime boundEndDate = Convert.ToDateTime(boundRow[boundDetailEndDateColumnKey]);
                    foreach (DataRow detailRow in rows)
                    {
                        DateTime detailFactDate = Convert.ToDateTime(detailRow["FactDate"]);
                        if (detailFactDate < boundStartDate || detailFactDate > boundEndDate)
                        {
                            ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(detailRow["ID"]), "FactDate", Association);
                            msg.Summary = string.Format(ErrorMessage, detailRow["ID"]);
                            msg.HasError = true;
                            vmh.Add(msg);
                        }
                    }
                }
            }
            return vmh;
        }
    }

    /// <summary>
    ///  проверка даты на вхождение в диапазон дат из другой детали
    /// </summary>
    public class MasterDetailDetailFactDateValidator : AbstractMasterDetailDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable, DataTable boundDetailTable)
        {
            ValidationMessage vmh = null;

            if (detailTable.Rows.Count != 0 && boundDetailTable.Rows.Count != 0)
            {
                DateTime boundStartDate = Convert.ToDateTime(boundDetailTable.Rows[0]["StartDate"]);
                DateTime boundEndDate = Convert.ToDateTime(boundDetailTable.Rows[0]["EndDate"]);
                DateTime detailFactDate = Convert.ToDateTime(detailTable.Rows[0]["FactDate"]);
                if (detailFactDate < boundStartDate || detailFactDate > boundEndDate)
                {
                    vmh = new UltraGridCellValidationMessage(Convert.ToInt32(detailTable.Rows[0]["ID"]), "FactDate", Association);
                    vmh.Summary = string.Format(ErrorMessage, detailTable.Rows[0]["ID"]);
                    vmh.HasError = true;
                }
            }
            return vmh;
        }
    }

    public class MasterDetailDetailStartDateValidator : AbstractMasterDetailDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable, DataTable boundDetailTable)
        {
            ValidationMessages vmh = new ValidationMessages();

            foreach (DataRow boundRow in boundDetailTable.Rows)
            {
                DateTime boundPlanDate = Convert.ToDateTime(boundRow["PlanDate"]);
                string selectStr = string.Format("Payment = {0}", boundRow["PlanPayment"]);
                DataRow[] rows = detailTable.Select(selectStr);
                if (rows != null && rows.Length > 0)
                {
                    foreach (DataRow detailRow in rows)
                    {
                        DateTime detailStartDate = Convert.ToDateTime(detailRow["StartDate"]);
                        if (boundPlanDate != detailStartDate)
                        {
                            ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(detailRow["ID"]), "StartDate", Association);
                            msg.Summary = string.Format(ErrorMessage, detailRow["ID"]);
                            msg.HasError = true;
                            vmh.Add(msg);
                        }
                    }
                }
            }
            return vmh;
        }
    }

}
