using System;
using System.Data;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Common.Validations;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations
{
	/// <summary>
	/// Проверяет соответствие суммы в таблице фактов и общей суммы в детали.
	/// </summary>
	public class MasterDetailSumValidator : AbstractMasterDetailValidator
	{
		public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable)
		{
			ValidationMessage msg = null;

			decimal masterSum = Convert.ToDecimal(masterRow["Sum"]);
			decimal detailSum = 0;
			foreach (DataRow row in detailTable.Rows)
			{
				detailSum += Convert.ToDecimal(row["Sum"]);
			}

			if (masterSum < detailSum)
			{
				msg = new UltraGridColumnValidationMessage("Sum", Association);
				msg.Summary = ErrorMessage;
				msg.HasError = true;
			}

			return msg;
		}
	}
}
