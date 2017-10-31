using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations
{
	public class DetailFactDateValidator : AbstractDetailValidator
	{
		public override IValidatorMessageHolder Validate(DataTable detailTable)
		{
			ValidationMessages vmh = new ValidationMessages();

			foreach (DataRow detailRow in detailTable.Rows)
			{
#warning переделать контроль на какой нибудь другой
                
				DateTime factDate = Convert.ToDateTime(detailRow["FactDate"]);
				DateTime planDate = Convert.ToDateTime(detailRow["PlanDate"]);
				if (factDate > planDate)
				{
					ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(detailRow["ID"]), "FactDate", Association, SecondAssociation);
					msg.Summary = string.Format(ErrorMessage, detailRow["ID"]);
					msg.HasError = true;
					vmh.Add(msg);
				}
			}
			return vmh;
		}
	}

	public class DetailStartEndDateValidator : AbstractDetailValidator
	{
		public override IValidatorMessageHolder Validate(DataTable detailTable)
		{
			ValidationMessages vmh = new ValidationMessages();

			foreach (DataRow detailRow in detailTable.Rows)
			{
				DateTime startDate = Convert.ToDateTime(detailRow["StartDate"]);
				DateTime endDate = Convert.ToDateTime(detailRow["EndDate"]);
				if (startDate > endDate)
				{
					List<string> ColumnsKeys = new List<string>();
					ColumnsKeys.Add("StartDate");
					ColumnsKeys.Add("EndDate");
					ValidationMessage msg = new UltraGridRowCellsValidationMessage(Convert.ToInt32(detailRow["ID"]), ColumnsKeys, Association);
					msg.Summary = ErrorMessage;
					msg.HasError = true;
					vmh.Add(msg);
				}
			}
			return vmh;
		}
	}

	public class DetailCurrencyValidation : AbstractDetailValidator
	{
		public override IValidatorMessageHolder Validate(DataTable detailTable)
		{
			ValidationMessages vmh = new ValidationMessages();

			foreach (DataRow detailRow in detailTable.Rows)
			{
				int refOKV = Convert.ToInt32(detailRow["RefOKV"]);
				// если валюта не рубль, проверим заполненность полей
				if (refOKV != -1)
				{
					if (detailRow.IsNull("ExchangeRate"))
					{
						ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(detailRow["ID"]), "ExchangeRate", Association);
						msg.Summary = string.Format(ErrorMessage, detailRow["ID"]);
						msg.HasError = true;
						vmh.Add(msg);
					}
					if (detailRow.IsNull("CurrencySum"))
					{
						ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(detailRow["ID"]), "CurrencySum", Association);
						msg.Summary = string.Format(ErrorMessage, detailRow["ID"]);
						msg.HasError = true;
						vmh.Add(msg);
					}
				}
				else
				{
					if (!detailRow.IsNull("ExchangeRate"))
					{
						ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(detailRow["ID"]), "ExchangeRate", Association);
						msg.Summary = string.Format(ErrorMessage, detailRow["ID"]);
						msg.HasError = true;
						vmh.Add(msg);
					}
					if (!detailRow.IsNull("CurrencySum"))
					{
						ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(detailRow["ID"]), "CurrencySum", Association);
						msg.Summary = string.Format(ErrorMessage, detailRow["ID"]);
						msg.HasError = true;
						vmh.Add(msg);
					}
				}
			}
			return vmh;
		}
	}

	public class DetailCurrentNumberValidator : AbstractDetailValidator
	{
		public override IValidatorMessageHolder Validate(DataTable detailTable)
		{
			ValidationMessages vmh = new ValidationMessages();

			DataRow[] rows = detailTable.Select(string.Empty, "StartDate asc");
			int current = 1;
			foreach (DataRow row in rows)
			{
				if (Convert.ToInt32(row["Payment"]) != current)
				{
					ValidationMessage msg = new UltraGridCellValidationMessage(Convert.ToInt32(row["ID"]), "Payment", Association);
					msg.Summary = string.Format(ErrorMessage, row["ID"]);
					msg.HasError = true;
					vmh.Add(msg);
				}
				current++;
			}
			return vmh;
		}
	}
}