using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations
{
	public class MasterDateValidator : AbstractMasterValidator
	{
		public override IValidatorMessageHolder Validate(DataRow masterRow, string masterKey)
		{
			ValidationMessages vmh = new ValidationMessages();

			DateTime masterStartDate = Convert.ToDateTime(masterRow["StartDate"]);
			DateTime masterEndDate = Convert.ToDateTime(masterRow["EndDate"]);
			DateTime? renevalDate = null;
			if (!(masterRow["RenewalDate"] is DBNull))
			{
				renevalDate = Convert.ToDateTime(masterRow["RenewalDate"]);
			}

			if (masterStartDate > masterEndDate)
			{
				List<string> columns = new List<string>();
				columns.Add("StartDate");
				columns.Add("EndDate");
				ValidationMessage msg = new UltraGridRowCellsValidationMessage(Convert.ToInt32(masterRow["ID"]), columns, masterKey);
				msg.Summary = string.Format(ErrorMessage, masterRow["ID"]);
				msg.HasError = true;
				vmh.Add(msg);
			}

			if (renevalDate != null)
			{
				if (masterEndDate > renevalDate)
				{
					List<string> columns = new List<string>();
					columns.Add("EndDate");
					columns.Add("RenewalDate");
					ValidationMessage msg = new UltraGridRowCellsValidationMessage(Convert.ToInt32(masterRow["ID"]), columns, masterKey);
					msg.Summary = string.Format(ErrorMessage, masterRow["ID"]);
					msg.HasError = true;
					vmh.Add(msg);
				}
			}
			return vmh;
		}
	}

	public class MasterCurrencyValidation : AbstractMasterValidator
	{
		public override IValidatorMessageHolder Validate(DataRow masterRow, string masterKey)
		{
			ValidationMessages vmh = new ValidationMessages();

			int refOKV = Convert.ToInt32(masterRow["RefOKV"]);
			// если валюта не рубль, проверим заполненность полей
			if (refOKV != -1)
			{
				if (masterRow.IsNull("ExchangeRate"))
				{
					ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(masterRow["ID"]), "ExchangeRate", masterKey);
					msg.Summary = string.Format(ErrorMessage, masterRow["ID"]);
					msg.HasError = true;
					vmh.Add(msg);
				}
				if (masterRow.IsNull("CurrencySum"))
				{
					ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(masterRow["ID"]), "CurrencySum", masterKey);
					msg.Summary = string.Format(ErrorMessage, masterRow["ID"]);
					msg.HasError = true;
					vmh.Add(msg);
				}

			}
			else
			{
				if (!masterRow.IsNull("ExchangeRate"))
				{
					ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(masterRow["ID"]), "ExchangeRate", masterKey);
					msg.Summary = string.Format(ErrorMessage, masterRow["ID"]);
					msg.HasError = true;
					vmh.Add(msg);
				}
				if (!masterRow.IsNull("CurrencySum"))
				{
					ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(masterRow["ID"]), "CurrencySum", masterKey);
					msg.Summary = string.Format(ErrorMessage, masterRow["ID"]);
					msg.HasError = true;
					vmh.Add(msg);
				}
			}
			return vmh;
		}
	}
}