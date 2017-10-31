using System;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	internal class ValuationValidation
	{
		public static IValidatorMessageHolder Validate(IValidatorMessageHolder vmh)
		{
			ValidationMessages vmh2 = new ValidationMessages();

			if (vmh is ValidationMessages)
			{
				ValuationUI content = (ValuationUI)WorkplaceSingleton.Workplace.ActiveContent;
				IEntityAssociation association = content.Service.Data.Associated[SchemeObjectsKeys.a_Forecast_IndValuesKey];
				foreach (ValidationMessage item in ((ValidationMessages)vmh))
				{
					if (item != null)
					{
						String[] s = item.Message.Split('_');
						Int32 rowID = Convert.ToInt32(s[0]);
						String columnKey = s[1];
						String direction = s[2];

						switch (columnKey)
						{
							case "VALUEYESTIMATE":
								columnKey = "V_EST_B";
								break;
							case "VALUEY1":
								columnKey = "V_Y1_B";
								break;
							case "VALUEY2":
								columnKey = "V_Y2_B";
								break;
							case "VALUEY3":
								columnKey = "V_Y3_B";
								break;
							case "VALUEY4":
								columnKey = "V_Y4_B";
								break;
							case "VALUEY5":
								columnKey = "V_Y5_B";
								break;
						}
												
						UltraGridCellValidationMessage mes = new UltraGridCellValidationMessage(rowID, columnKey, SchemeObjectsKeys.f_S_Valuation_Key, association);
						mes.HasError = true;

						if (direction == "inc")
							mes.Summary = "Индикатор увеличил значение";
						if (direction == "dec")
							mes.Summary = "Индикатор уменьшил значение";

						mes.Message = item.Message;
						vmh2.Add(mes);
					}
				}
			}

			return vmh2;
		}


		
	}
}
