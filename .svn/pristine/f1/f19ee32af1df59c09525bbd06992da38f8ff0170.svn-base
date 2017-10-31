using System;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Forecast;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	class SelectScenarioUI : ForecastFactTablesUI
	{
		public SelectScenarioUI(IEntity dataObject) : base(dataObject)
		{
		
		}
		
		/// <summary>
		/// Заполняет значеия лукапных полей
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnInitializeRow(object sender, InitializeRowEventArgs e)
		{
			if (e.Row.IsDataRow)
			{
				if (e.Row.Cells.Exists("UserID_Lookup") && (e.Row.Cells["UserID"].Value != DBNull.Value))
				{
					if (ForecastUI.UserDict.ContainsKey(Convert.ToInt32(e.Row.Cells["UserID"].Value)))
					{
						e.Row.Cells["UserID_Lookup"].Value = ForecastUI.UserDict[Convert.ToInt32(e.Row.Cells["UserID"].Value)];
					}
					else
						e.Row.Cells["UserID_Lookup"].Value = "Пользователь не определен";
				}

				if (e.Row.Cells.Exists("READYTOCALC_Lookup") && e.Row.Cells["READYTOCALC"].Value != DBNull.Value)
				{
					Int32 i = Convert.ToInt32(e.Row.Cells["READYTOCALC"].Value);
					switch ((ScenarioStatus)i)
					{
						case ScenarioStatus.BaseScenario:
							e.Row.Cells["READYTOCALC_Lookup"].Value = "Базовый сценарий";
							break;
						case ScenarioStatus.NonCalculated:
							e.Row.Cells["READYTOCALC_Lookup"].Value = "Не рассчитан";
							break;
						case ScenarioStatus.ReadyToCalc:
							e.Row.Cells["READYTOCALC_Lookup"].Value = "Готов к расчету";
							break;
						case ScenarioStatus.Calculated:
							e.Row.Cells["READYTOCALC_Lookup"].Value = "Расcчитан";
							break;
					}
				}
			}
		}
				
				

		/// <summary>
		/// Инициализация грида мастера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnGridFactTableInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			e.Layout.Bands[0].Columns["ID"].Hidden = true;
			e.Layout.Bands[0].Columns["PERIOD"].Hidden = true;
			e.Layout.Bands[0].Columns["USERID"].Hidden = true;
			e.Layout.Bands[0].Columns["READYTOCALC"].Hidden = true;
			e.Layout.Bands[0].Columns["PERCOFCOMPLETE"].Hidden = true;
			e.Layout.Bands[0].Columns["PARENT"].Hidden = true;
			if (!e.Layout.Bands[0].Columns.Exists("UserID_Lookup"))
			{
				UltraGridColumn usrCol = e.Layout.Bands[0].Columns.Add("UserID_Lookup", "Владелец сценария");
				usrCol.CellActivation = Activation.ActivateOnly;
			}

			if (!e.Layout.Bands[0].Columns.Exists("READYTOCALC_Lookup"))
			{
				UltraGridColumn usrCol = e.Layout.Bands[0].Columns.Add("READYTOCALC_Lookup", "Готовность сценария");
				usrCol.CellActivation = Activation.ActivateOnly;
			}

			e.Layout.Bands[0].Columns["REFYEAR"].Header.Caption = "Базовый год";
			e.Layout.Bands[0].Columns["NAME"].Header.VisiblePosition = 1;
			e.Layout.Bands[0].Columns["READYTOCALC_Lookup"].Header.VisiblePosition = 2;
			e.Layout.Bands[0].Columns["UserID_Lookup"].Header.VisiblePosition = 3;
			//UltraGridHelper.SetLikelyEditButtonColumnsStyle(((UltraGrid)sender).DisplayLayout.Bands[0].Columns["UserID_Lookup"], 0);
			ForecastUI.UpdateUsersDict();
		}
	}

	class ScenarioModal : FactTableModal
	{
		public int ShowScenarioModal()
		{
			return ShowFactTableModal(SchemeObjectsKeys.f_S_Forecast_Key);
		}

		public override void CreateFactTableUI(IFactTable cls)
		{
			clsUI = new SelectScenarioUI(cls);
		}

		public override void AdditionsFilters(BaseClsUI clsUI)
		{
			clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();
			clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["READYTOCALC"].FilterConditions.Add(FilterComparisionOperator.Equals, "2");
			clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["PARENT"].FilterConditions.Add(FilterComparisionOperator.Equals, null);		
		}
	}
}
