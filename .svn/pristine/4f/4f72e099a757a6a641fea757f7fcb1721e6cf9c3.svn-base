using System;
using System.Data;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Common.Validations;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary.Validations;


namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations
{
    public class MasterDetailEndDateValidator : AbstractMasterDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable)
        {
            ValidationMessages vmh = new ValidationMessages();

            DateTime masterStartDate = Convert.ToDateTime(masterRow["StartDate"]);
            DateTime masterEndDate;
            if (!(masterRow["RenewalDate"] is DBNull))
                masterEndDate = Convert.ToDateTime(masterRow["RenewalDate"]);
            else
                masterEndDate = Convert.ToDateTime(masterRow["EndDate"]);

            foreach (DataRow row in detailTable.Rows)
            {
                DateTime detailEndDate = Convert.ToDateTime(row["EndDate"]);
                if (masterStartDate > detailEndDate || masterEndDate < detailEndDate)
                {
                    ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(row["ID"]) , "EndDate", Association);
                    msg.Summary = string.Format(ErrorMessage, row["ID"]);
                    msg.HasError = true;
                    vmh.Add(msg);
                }
            }
            return vmh;
        }
    }

    public class MasterDetailStartDateValidator : AbstractMasterDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable)
        {
            ValidationMessages vmh = new ValidationMessages();

            DateTime masterStartDate = Convert.ToDateTime(masterRow["StartDate"]);
            DateTime masterEndDate;
            if (!(masterRow["RenewalDate"] is DBNull))
                masterEndDate = Convert.ToDateTime(masterRow["RenewalDate"]);
            else
                masterEndDate = Convert.ToDateTime(masterRow["EndDate"]);

            foreach (DataRow row in detailTable.Rows)
            {
                DateTime detailStartDate = Convert.ToDateTime(row["StartDate"]);
                if (masterStartDate > detailStartDate || detailStartDate > masterEndDate)
                {
                    ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(row["ID"]), "StartDate", Association);
                    msg.Summary = string.Format(ErrorMessage, row["ID"]);
                    msg.HasError = true;
                    vmh.Add(msg);
                }
            }
            return vmh;
        }
    }

    public class MasterDetailChargeDateValidator : AbstractMasterDetailValidator
    {
        public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable)
        {
            ValidationMessages vmh = new ValidationMessages();

            DateTime masterStartDate = Convert.ToDateTime(masterRow["StartDate"]);
            DateTime masterEndDate;
            if (!(masterRow["RenewalDate"] is DBNull))
                masterEndDate = Convert.ToDateTime(masterRow["RenewalDate"]);
            else
                masterEndDate = Convert.ToDateTime(masterRow["EndDate"]);

            foreach (DataRow row in detailTable.Rows)
            {
                DateTime detailStartDate = Convert.ToDateTime(row["ChargeDate"]);
                if (masterStartDate > detailStartDate || detailStartDate > masterEndDate)
                {
                    ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(row["ID"]), "ChargeDate", Association);
                    msg.Summary = string.Format(ErrorMessage, row["ID"]);
                    msg.HasError = true;
                    vmh.Add(msg);
                }
            }
            return vmh;
        }
    }
}
