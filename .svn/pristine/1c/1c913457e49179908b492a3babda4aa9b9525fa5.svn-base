using System;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{

	internal class ScenarioValidation
	{
		public static IValidatorMessageHolder Validate(IValidatorMessageHolder vmh)
		{
			ValidationMessages vmh2 = new ValidationMessages();
			ScenarioUI content = (ScenarioUI)WorkplaceSingleton.Workplace.ActiveContent;

			/// Разбор сообщений со стороны сервера
			if (vmh is ValidationMessages)
			{
				IEntityAssociation associationInd = content.Service.Data.Associated[SchemeObjectsKeys.a_Forecast_IndValuesKey];
				foreach (ValidationMessage item in ((ValidationMessages)vmh))
				{
					if (item != null)
					{
						String[] s = item.Message.Split('_');
						Int32 rowID = Convert.ToInt32(s[0]);
						String columnKey = s[1];
						String direction = s[2];

					/*	ScenarioUI content = (ScenarioUI)WorkplaceSingleton.Workplace.ActiveContent;
						IEntityAssociation association = content.Service.Data.Associated[SchemeObjectsKeys.a_Forecast_IndValuesKey];*/
						UltraGridCellValidationMessage mes = new UltraGridCellValidationMessage(rowID, columnKey, SchemeObjectsKeys.f_S_Scenario_Key, associationInd);
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

			///Генерация клиентских сообщений
			IEntityAssociation associationAdj = content.Service.Data.Associated[SchemeObjectsKeys.a_Forecast_AdjValuesKey];
			UltraGrid ug = content.GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).ugData;
			foreach (UltraGridRow row in ug.Rows)
			{
                if (row.Cells != null)
                {
                    if (row.Cells["INDEXDEF"].Value != DBNull.Value)
                    {
                        Int32 id = Convert.ToInt32(row.Cells["ID"].Value);
                        UltraGridRowValidationMessage mes = new UltraGridRowValidationMessage(id, SchemeObjectsKeys.f_S_Scenario_Key, associationAdj);
                        mes.Summary = "Значения этой строки заполнено на основе значения на оценочный год.";
                        vmh2.Add(mes);
                    }
                }
			}

			return vmh2;
		}


	}


}