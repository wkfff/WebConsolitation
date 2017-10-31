using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AdministrationUI;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.ForecastUI.Commands;
using Krista.FM.Client.ViewObjects.ForecastUI.Validations;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Forecast;
using Krista.FM.ServerLibrary.Validations;
using CC = Krista.FM.Client.Components;



namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	/// <summary>
	/// Класс пользовательского интерфейса "Сценарии"
	/// </summary>
	public class ScenarioUI: ForecastUI
	{
		//private IForecastService service;		
		
		public ScenarioUI(IForecastService service) : base(service) 
		{ 
		}

		public override string Key
		{
			get
			{
				return SchemeObjectsKeys.f_S_Scenario_Key;
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			callClassName = "Scenario";

			SetPermissions();
			
			UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["Forecast"];
			ButtonTool tool = CommandService.AttachToolbarTool(new CalcScenarioCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["calc"];
			tool.SharedProps.Visible = canCalculate;

			tool = CommandService.AttachToolbarTool(new CalcScenarioWithValidCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["calc_val"];
			tool.SharedProps.Visible = canCalculate;

			//tool = CommandService.AttachToolbarTool(new CopyBaseScenarioParam(), toolbar);
			tool = CommandService.AttachToolbarTool(new SetReadyToCalc(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["m_ready"];
			tool.SharedProps.Visible = canCalculate;

		
			/*toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).utmMain.Toolbars["ForecastDetail"];
			StateButtonTool tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);

			toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_Indicators_Key).utmMain.Toolbars["ForecastDetail"];
			tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);*/
			
			//Добавление обработчиков при создании гридов. 
			((BaseClsView)ViewCtrl).ugeCls.OnGridInitializeLayout += new GridInitializeLayout(ugeCls_OnGridScenarioInitializeLayout);
			DetailGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ScenarioUI_DetailGridInitializeLayout);
			((BaseClsView)ViewCtrl).ugeCls.OnSaveChanges += new SaveChanges(ugeCls_OnSaveChanges);
			((BaseClsView)ViewCtrl).ugeCls.OnInitializeRow += new InitializeRow(OnInitializeRow);
			
			((BaseClsView)ViewCtrl).ugeCls.OnAfterRowInsert += new AfterRowInsert(afterRowInsert);
			
		}

		// содержит все визуализаторы, которые при проверке загорятся
		private ForecastMessagesVisualizator messagesVisualizator;
		public ForecastMessagesVisualizator MessagesVisualizator
		{
			get { return messagesVisualizator; }
			set { messagesVisualizator = value; }
		}

        public IValidatorMessageHolder CalcedValidation { get; set; }

		/// <summary>
		/// Заполняет значения лукапных полей
		/// Вызывается для каждой строчки как мастер грида так и грида деталей
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInitializeRow(object sender, InitializeRowEventArgs e)
		{
			if (e.Row.IsDataRow)
			{
				if (e.Row.Cells.Exists("UserID_Lookup") && (e.Row.Cells["UserID"].Value != DBNull.Value))
				{
					if (UserDict.ContainsKey(Convert.ToInt32(e.Row.Cells["UserID"].Value)))
					{
						e.Row.Cells["UserID_Lookup"].Value = UserDict[Convert.ToInt32(e.Row.Cells["UserID"].Value)];
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
                            e.Row.Cells["READYTOCALC_Lookup"].Value = String.Format("Не рассчитан"); //,Service.GetPercentOfComplete(id)
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
		/// Фильтр выделяет только сценарии (Parent is NULL)
		/// </summary>
		/// <param name="parentID"></param>
		/// <param name="filterStr"></param>
		/// <returns></returns>
		protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
		{
			dataQuery = GetDataSourcesFilter() + " and (Parent is NULL)";
			filterStr = dataQuery;
			return ((IEntity)ActiveDataObj).GetDataUpdater(dataQuery, null, null);
		}

		protected override IDataUpdater GetDetailUpdater(IEntity activeDetailObject, object masterValue)
		{
			//return base.GetDetailUpdater(activeDetailObject, masterValue);
			if (Convert.ToInt64(masterValue) >= 0)
				return Service.GetScenarioDetailsUpdater(activeDetailObject.Key, String.Format("(RefScenario = {0})",masterValue));
			return activeDetailObject.GetDataUpdater("1 = 2", null, null);
		}

		/// <summary>
		/// Обработчик события нажатия на кнопке в мастер гриде
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void ugeCls_OnClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{
			if (e.Cell.Column.Key == "UserID_Lookup")
			{
				///Если базовый то выходим
				if ((ScenarioStatus)Convert.ToInt32(e.Cell.Row.Cells["READYTOCALC"].Value) == ScenarioStatus.BaseScenario) return;

				UsersModalForm userForm = new UsersModalForm(Workplace);
				Int32 userID = -1;
				String userName = String.Empty;
				if (userForm.ShowModal(NavigationNodeKind.ndAllUsers, ref userID, ref userName))
				{
					if (userID > 0)
					{
						e.Cell.Row.Cells["USERID"].Value = userID;
						e.Cell.Row.Cells["UserID_Lookup"].Value = userName;
					}
				}
			}
			else 
				base.ugeCls_OnClickCellButton(sender, e);
		}

		/// <summary>
		/// Обработчик нажатиия на кнопке в гриде деталей
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void ugeDetail_OnClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{
			if (e.Cell.Column.Key == "UserID_Lookup")
			{
				UsersModalForm userForm = new UsersModalForm(Workplace);
				Int32 userID = -1;
				String userName = String.Empty;
				if (userForm.ShowModal(NavigationNodeKind.ndAllUsers, ref userID, ref userName))
				{
					if (userID > 0)
					{
						e.Cell.Row.Cells["USERID"].Value = userID;
						e.Cell.Row.Cells["UserID_Lookup"].Value = userName;
					}
				}
			}
			else
				if (e.Cell.Column.Key == "REFPARAMS_Lookup")
				{
					IClassifier cls = Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_Parametrs_Key];
					// создаем объект просмотра классификаторов нужного типа
					DataClsUI clsUI = new DataClsUI(cls);
					clsUI.Workplace = Workplace;
					clsUI.RestoreDataSet = false;
					clsUI.Initialize();
					clsUI.InitModalCls(-1);

					// создаем форму
					frmModalTemplate modalClsForm = new frmModalTemplate();
					modalClsForm.AttachCls(clsUI, false);

					// скрываем кнопку ОК 
					modalClsForm.HideBtnOk(true);
					ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);

					// ...загружаем данные
					clsUI.RefreshAttachedData();

					String filtrStr = String.Empty;
					switch (utcDetails.ActiveTab.Key)
					{
						case SchemeObjectsKeys.t_S_Adjusters_Key:
							filtrStr = "2\\d\\d\\d\\d\\d";
							break;
						case SchemeObjectsKeys.t_S_Indicators_Key:
							filtrStr = "1\\d\\d\\d\\d\\d";
							break;
						case SchemeObjectsKeys.t_S_Static_Key:
							filtrStr = "3\\d\\d\\d\\d\\d";
							break;
						case SchemeObjectsKeys.t_S_UnReg_Key:
							filtrStr = "5\\d\\d\\d\\d\\d";
							break;
					}

					clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();
					clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["Code_Remasked"].FilterConditions.Add(FilterComparisionOperator.Match, filtrStr);
					clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["Code_Remasked"].FilterConditions.Add(FilterComparisionOperator.DoesNotEndWith, "00");
					clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["ID"].SortIndicator = SortIndicator.Ascending;
					
					if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
					{
						//Сюда мы никогда не попадем... так как кнопка ОК непоказывается :)
						int clsID = modalClsForm.AttachedCls.GetSelectedID();
						// если ничего не выбрали - считаем что функция завершилась неудачно
						if (clsID == -10)
							return;

						String columnName = e.Cell.Column.Key;
						columnName = CC.UltraGridEx.GetSourceColumnName(columnName);

						e.Cell.Row.Cells[columnName].Value = clsID;
					}
					clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();
				}
				else
					base.ugeDetail_OnClickCellButton(sender, e);
		}

		
		/// <summary>
		/// Инициализация грида деталей	
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScenarioUI_DetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			e.Layout.Bands[0].Columns["ID"].Hidden = true;
			if (e.Layout.Bands[0].Columns.Exists("REFPARAMS_Lookup"))
			{
				e.Layout.Bands[0].Columns["REFPARAMS_Lookup"].Header.VisiblePosition = 1;
				e.Layout.Bands[0].Columns["REFPARAMS_Lookup"].CellActivation = Activation.ActivateOnly;
			}
			switch (GetActiveDetailTab().Key)
			{
				case SchemeObjectsKeys.t_S_Adjusters_Key:
				case SchemeObjectsKeys.t_S_Indicators_Key:
				case SchemeObjectsKeys.t_S_UnReg_Key:
					e.Layout.Bands[0].Columns["VALUEBASE"].Hidden = true;
					break;
				case SchemeObjectsKeys.t_S_Static_Key:
					e.Layout.Bands[0].Columns["VALUEY1"].Hidden = true;
					e.Layout.Bands[0].Columns["VALUEY2"].Hidden = true;
					e.Layout.Bands[0].Columns["VALUEY3"].Hidden = true;
					e.Layout.Bands[0].Columns["VALUEY4"].Hidden = true;
					e.Layout.Bands[0].Columns["VALUEY5"].Hidden = true;
					break;
			}

			if (e.Layout.Bands[0].Columns.Exists("GroupName"))
			{
				e.Layout.Bands[0].Columns["GroupName"].Hidden = true;
				e.Layout.Bands[0].Columns["GroupName"].CellActivation = Activation.ActivateOnly;
				e.Layout.Bands[0].Columns["GROUPNAME"].Header.Caption = "Группа";
			}

			if (vo.ugeCls.ugData.ActiveRow != null)
			{
				if (vo.ugeCls.ugData.ActiveRow.Cells["RefYear"].Value != DBNull.Value)
				{
					Int32 BaseYear = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["RefYear"].Value);
					e.Layout.Bands[0].Columns["VALUEBASE"].Header.Caption = String.Format("Отчет {0}", (BaseYear));
					e.Layout.Bands[0].Columns["VALUEESTIMATE"].Header.Caption = String.Format("Оценка {0}", (BaseYear + 1));
					e.Layout.Bands[0].Columns["VALUEY1"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 2));
					e.Layout.Bands[0].Columns["VALUEY2"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 3));
					e.Layout.Bands[0].Columns["VALUEY3"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 4));
					e.Layout.Bands[0].Columns["VALUEY4"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 5));
					e.Layout.Bands[0].Columns["VALUEY5"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 6));
				}
			}

			e.Layout.Bands[0].Columns["USERID"].Hidden = true;
			e.Layout.Bands[0].Columns["REFPARAMS"].CellActivation = Activation.ActivateOnly;
			GetActiveDetailGridEx().OnInitializeRow += new InitializeRow(OnInitializeRow);
			GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).OnAfterRowActivate +=new AfterRowActivate(ScenarioUI_OnAfterRowActivate);
			GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).OnCellChange += new CellChange(ScenarioUI_OnCellChange);


			if (!e.Layout.Bands[0].Columns.Exists("UserID_Lookup"))
			{
				UltraGridColumn usrCol = e.Layout.Bands[0].Columns.Add("UserID_Lookup", "Ответственное лицо");
				usrCol.CellActivation = Activation.ActivateOnly;
			}

			if (e.Layout.Bands[0].Columns.Exists("DESIGNATION"))
			{
				e.Layout.Bands[0].Columns["DESIGNATION"].Header.Caption = "Единица измерения";
				e.Layout.Bands[0].Columns["DESIGNATION"].CellActivation = Activation.ActivateOnly;
			}

			if (e.Layout.Bands[0].Columns.Exists("MASK"))
			{
				e.Layout.Bands[0].Columns["MASK"].Hidden = true;
			}
			
			if (canAssignParam)
				UltraGridHelper.SetLikelyEditButtonColumnsStyle(((UltraGrid)sender).DisplayLayout.Bands[0].Columns["UserID_Lookup"], 0);
						
			//SwitchFilter();
			//SwitchGroup();
		}

		private void ScenarioUI_OnCellChange(object sender, CellEventArgs e)
		{
			if (e.Cell.Column.Key.Contains("VALUE"))
			{
				GetActiveDetailGridEx().ugData.ActiveRow.Cells["INDEXDEF"].Value = DBNull.Value;
			}
		}

		private void ScenarioUI_OnAfterRowActivate(object sender, EventArgs e)
		{
			ComboButtonTool cbt = CommandService.GetComboButtonByKey(GetActiveDetailGridEx().utmMain, "btnFillAdj");
			UltraGridRow row = GetActiveDetailGridEx().ugData.ActiveRow;
			if ((cbt != null) && (row != null) && (row.Cells != null) && (row.Band.Columns.Exists("INDEXDEF")))
			{
				if (row.Cells["INDEXDEF"].Value != DBNull.Value)
				{
					Double d = Convert.ToDouble(row.Cells["INDEXDEF"].Value);
					cbt.Comboboxtool.Value = String.Format("{0} %", d * 100);
				}
				else
					cbt.Comboboxtool.Value = "";
			}
		}


		/// <summary>
		/// Обработчик инициализации строчек грида деталей
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void InitializeDetailRow(object sender, InitializeRowEventArgs e)
		{
			base.InitializeDetailRow(sender, e);
			if (e.Row.IsDataRow)
			{
				//Скрытие ячеек не требующих ввод данных
				if (e.Row.Cells.Exists("MASK") && (e.Row.Cells["MASK"].Value != DBNull.Value))
				{
					switch (utcDetails.ActiveTab.Key)
					{
						case SchemeObjectsKeys.t_S_Adjusters_Key:
							if ((Convert.ToByte(e.Row.Cells["MASK"].Value) & 0x20) == 0)
							{
								e.Row.Cells["VALUEESTIMATE"].Activation = Activation.Disabled;
								e.Row.Cells["VALUEESTIMATE"].Appearance.BackColor = Color.LightGray;
							}
							break;
						case SchemeObjectsKeys.t_S_Static_Key:
							if ((Convert.ToByte(e.Row.Cells["MASK"].Value) & 0x02) == 0)
							{
								e.Row.Cells["VALUEBASE"].Activation = Activation.Disabled;
								e.Row.Cells["VALUEBASE"].Appearance.BackColor = Color.LightGray;
							}
							if ((Convert.ToByte(e.Row.Cells["MASK"].Value) & 0x01) == 0)
							{
								e.Row.Cells["VALUEESTIMATE"].Activation = Activation.Disabled;
								e.Row.Cells["VALUEESTIMATE"].Appearance.BackColor = Color.LightGray;
							}
							break;
						case SchemeObjectsKeys.t_S_UnReg_Key:
							if ((Convert.ToByte(e.Row.Cells["MASK"].Value) & 0x20) == 0)
							{
								e.Row.Cells["VALUEESTIMATE"].Activation = Activation.Disabled;
								e.Row.Cells["VALUEESTIMATE"].Appearance.BackColor = Color.LightGray;
							}
							break;
					}
				}

				if (utcDetails.ActiveTab.Key == SchemeObjectsKeys.t_S_Indicators_Key)
				{
					e.Row.Cells["VALUEESTIMATE"].Activation = Activation.ActivateOnly;
					e.Row.Cells["VALUEESTIMATE"].Appearance.ForeColor = Color.DarkGray;
					e.Row.Cells["VALUEY1"].Activation = Activation.ActivateOnly;
					e.Row.Cells["VALUEY1"].Appearance.ForeColor = Color.DarkGray;
					e.Row.Cells["VALUEY2"].Activation = Activation.ActivateOnly;
					e.Row.Cells["VALUEY2"].Appearance.ForeColor = Color.DarkGray;
					e.Row.Cells["VALUEY3"].Activation = Activation.ActivateOnly;
					e.Row.Cells["VALUEY3"].Appearance.ForeColor = Color.DarkGray;
					e.Row.Cells["VALUEY4"].Activation = Activation.ActivateOnly;
					e.Row.Cells["VALUEY4"].Appearance.ForeColor = Color.DarkGray;
					e.Row.Cells["VALUEY5"].Activation = Activation.ActivateOnly;
					e.Row.Cells["VALUEY5"].Appearance.ForeColor = Color.DarkGray;
				}
				

				//Скрытие ячеек недоступных текущему пользователю для редактирования
				if (!canEdit)
				{
					Int32 userID = ClientAuthentication.UserID;
					if (Convert.ToInt32(e.Row.Cells["UserID"].Value) != userID)
					{
						e.Row.Activation = Activation.Disabled;
						e.Row.Appearance.ForeColor = Color.DarkGray;
					}
				}
				
				//Проверка границ
				CheckBounds("VALUEESTIMATE", e.Row);
				CheckBounds("VALUEY1", e.Row);
				CheckBounds("VALUEY2", e.Row);
				CheckBounds("VALUEY3", e.Row);
				CheckBounds("VALUEY4", e.Row);
				CheckBounds("VALUEY5", e.Row);
			}
			
		}

		/// <summary>
		/// Инициализация грида мастера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ugeCls_OnGridScenarioInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			e.Layout.Bands[0].Columns["ID"].Hidden = true;
			e.Layout.Bands[0].Columns["PERIOD"].Hidden = true;
			e.Layout.Bands[0].Columns["USERID"].Hidden = true;
			e.Layout.Bands[0].Columns["READYTOCALC"].Hidden = true;
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
			e.Layout.Bands[0].Columns["PERCOFCOMPLETE"].Header.VisiblePosition = 3;
			///e.Layout.Bands[0].Columns["READYTOCALC_Lookup"].
			e.Layout.Bands[0].Columns["UserID_Lookup"].Header.VisiblePosition = 4;
			e.Layout.Bands[0].Columns["REFYEAR"].Header.VisiblePosition = 5;
			
			if (canAssignParam && canCalculate)
				UltraGridHelper.SetLikelyEditButtonColumnsStyle(((UltraGrid)sender).DisplayLayout.Bands[0].Columns["UserID_Lookup"], 0);
			
			UpdateUsersDict();
		}
		
		/// <summary>
		/// Обработчик вызываемый при формировании тулбара
		/// </summary>
		public override void UpdateToolbar()
		{
			base.UpdateToolbar();

			UltraToolbar utbDForecast = new UltraToolbar("UnRegDetail");
			utbDForecast.DockedColumn = 3;
			utbDForecast.DockedRow = 0;
			utbDForecast.Text = "UnRegDetail";
			utbDForecast.Visible = true;
			GetDetailGridEx(SchemeObjectsKeys.t_S_UnReg_Key).utmMain.Toolbars.AddRange(new UltraToolbar[] { utbDForecast });

			utbDForecast = new UltraToolbar("StaticDetail");
			utbDForecast.DockedColumn = 3;
			utbDForecast.DockedRow = 0;
			utbDForecast.Text = "StaticDetail";
			utbDForecast.Visible = true;
			GetDetailGridEx(SchemeObjectsKeys.t_S_Static_Key).utmMain.Toolbars.AddRange(new UltraToolbar[] { utbDForecast });

			UltraToolbar toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).utmMain.Toolbars["AdjustersDetail"];
			StateButtonTool tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["filter"];
			tool2 = CommandService.AttachToolbarCheckedTool(new GroupByGroup(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["group"];

			utbDForecast = new UltraToolbar("AdjustersDetail_AutoFill");
			utbDForecast.DockedColumn = 3;
			utbDForecast.DockedRow = 0;
			utbDForecast.Text = "AdjustersDetail_AutoFill";
			utbDForecast.Visible = true;
			GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).utmMain.Toolbars.AddRange(new UltraToolbar[] { utbDForecast });
						
			ComboButtonTool tool = CommandService.AttachToolbarComboButtonTool(new FillAdj(), utbDForecast);
			tool.Comboboxtool.ValueList.ValueListItems.Add("-5%");
			tool.Comboboxtool.ValueList.ValueListItems.Add("-3%");
			tool.Comboboxtool.ValueList.ValueListItems.Add("-2%");
			tool.Comboboxtool.ValueList.ValueListItems.Add("-1%");
			tool.Comboboxtool.ValueList.ValueListItems.Add(" 0%");
			tool.Comboboxtool.ValueList.ValueListItems.Add("+1%");
			tool.Comboboxtool.ValueList.ValueListItems.Add("+2%");
			tool.Comboboxtool.ValueList.ValueListItems.Add("+3%");
			tool.Comboboxtool.ValueList.ValueListItems.Add("+5%");
			tool.Comboboxtool.DropDownStyle = DropDownStyle.DropDown;
			tool.Comboboxtool.MaxLength = 7;
			tool.Comboboxtool.SharedProps.Width = 70;
			tool.Comboboxtool.ToolValueChanged += new ToolEventHandler(ComboButtontool_ToolValueChanged);
			tool.Buttontool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["autofill"];

			toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_Indicators_Key).utmMain.Toolbars["IndicatorsDetail"];
			tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["filter"];
			tool2 = CommandService.AttachToolbarCheckedTool(new GroupByGroup(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["group"];

			toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_UnReg_Key).utmMain.Toolbars["UnRegDetail"];
			/*tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images[1];*/
			tool2 = CommandService.AttachToolbarCheckedTool(new GroupByGroup(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["group"];

			toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_Static_Key).utmMain.Toolbars["StaticDetail"];
			/*tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images[1];*/
			tool2 = CommandService.AttachToolbarCheckedTool(new GroupByGroup(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["group"];

			vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
			vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
			vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
			//vo.ugeCls.utmMain.Tools["deleteSelectedRows"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			//vo.ugeCls.utmMain.Tools["menuLoad"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["PasteRow"].SharedProps.Visible = false;
			((Infragistics.Win.UltraWinToolbars.StateButtonTool)vo.ugeCls.utmMain.Tools["ShowGroupBy"]).Checked = false;
			
			//Adjuster Toolbar
			UltraGridEx adjGrid = GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key);
			adjGrid.AllowAddNewRecords = false;
			ConfDetailsToolbar(adjGrid.utmMain);
						
			//Indicators Toolbar
			UltraGridEx indGrid = GetDetailGridEx(SchemeObjectsKeys.t_S_Indicators_Key);
			indGrid.AllowAddNewRecords = false;
			ConfDetailsToolbar(indGrid.utmMain);

			//UnRegulated Toolbar
			UltraGridEx unregGrid = GetDetailGridEx(SchemeObjectsKeys.t_S_UnReg_Key);
			unregGrid.AllowAddNewRecords = false;
			ConfDetailsToolbar(unregGrid.utmMain);

			//Static Toolbar
			UltraGridEx statGrid = GetDetailGridEx(SchemeObjectsKeys.t_S_Static_Key);
			statGrid.AllowAddNewRecords = false;
			ConfDetailsToolbar(statGrid.utmMain);
		}

		private void ComboButtontool_ToolValueChanged(object sender, ToolEventArgs e)
		{
			ComboButtonTool cbt = CommandService.GetComboButtonByKey(GetActiveDetailGridEx().utmMain, "btnFillAdj");//(ComboButtonTool)e.Tool.Tag;
			ComboBoxTool comboboxtool = cbt.Comboboxtool;
			ButtonTool buttontool = cbt.Buttontool;


			if ((comboboxtool.Value == null) || (comboboxtool.Value.ToString() == ""))
				buttontool.SharedProps.Enabled = false;

			else
			{
				String s = comboboxtool.Value.ToString().TrimEnd('%');
				try
				{
					Double d = Convert.ToDouble(s) / 100;
					buttontool.SharedProps.Enabled = true;
				}
				catch
				{
					buttontool.SharedProps.Enabled = false;
				}
			}
		}
				

		protected override void SetPermissionsToDetail()
		{
			//base.SetPermissionsToDetail();
			
		}

		/// <summary>
		/// Обработчик вызываемый при формировании тулбара деталей
		/// </summary>
		/// <param name="detailToolbar"></param>
		void ConfDetailsToolbar(UltraToolbarsManager detailToolbar)
		{
			detailToolbar.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			detailToolbar.Tools["deleteSelectedRows"].SharedProps.Visible = false;
			detailToolbar.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			detailToolbar.Tools["menuLoad"].SharedProps.Visible = false;
			detailToolbar.Tools["menuSave"].SharedProps.Visible = false;
			detailToolbar.Tools["ShowHierarchy"].SharedProps.Visible = false;
			detailToolbar.Tools["PasteRow"].SharedProps.Visible = false;
			((Infragistics.Win.UltraWinToolbars.StateButtonTool)detailToolbar.Tools["ShowGroupBy"]).Checked = false;
			((Infragistics.Win.UltraWinToolbars.StateButtonTool)detailToolbar.Tools["btnVisibleAddButtons"]).Checked = false;
		}
		
		/// <summary>
		/// Используется при сохранении изменений для поиска добавленых записей 
		/// в таблицу. Обновляется в методе OnBeforeSaveChanges(). Сбрасывается
		/// в методе ugeCls_OnSaveChanges(object sender)
		/// </summary>
		private List<Int32> addedID = new List<Int32>();

		protected override void BeforeSaveData()
		{
			if (MessagesVisualizator != null)
				MessagesVisualizator.Hide();
		}

        protected override void OnGetChangedAfterUpdate(DataSet dsObjData, DataSet changedRecords)
        {
            /*var list = (from f in dsObjData.Tables[0].AsEnumerable()
                       where (f["ID"] == DBNull.Value)
                       select f).ToList();*/

            List<DataRow> list = new List<DataRow>();

            Boolean isNull = false;

            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    if (row["id"] == DBNull.Value)
                    {
                        isNull = true;
                    }

                    list.Add(row);
                }
            }

            if (isNull) ////(list.Count() == 0)
            {
                ////List<DataRow> addedRow = new List<DataRow>();
                Dictionary<int, DataRow> editedRow = new Dictionary<int, DataRow>();

                foreach (DataRow row in list) ////dsObjData.Tables[0].Rows)
                {
                    if (row.RowState == DataRowState.Modified)
                    {
                        editedRow.Add(Convert.ToInt32(row["ID"]), row);
                    }
                }

                foreach (DataRow row in changedRecords.Tables[0].Rows)
                {
                    int id = Convert.ToInt32(row["ID"]);

                    if (!editedRow.ContainsKey(id))
                    {
                        //addedRow.Add(row);
                        addedID.Add(id);
                    }
                }
            }
            else
            {
                foreach (DataRow dr in list) ////dsObjData.Tables[0].Rows)
                {
                    if (dr.RowState == DataRowState.Added)
                    {
                        Int32 rowID;

                        var o = dr["ID"];
                        if (o != DBNull.Value)
                        {
                            rowID = Convert.ToInt32(o);
                            addedID.Add(rowID);
                        }
                    }
                }
            }
        }

		bool ugeCls_OnSaveChanges(object sender)
		{
			Boolean wasAdded = false;
			foreach (Int32 rowID in addedID)
			{
				try
				{
					Service.CopyScenarioDetails(rowID, null);
					wasAdded = true;
				}
				catch (Exception e)
				{
					addedID.Remove(rowID);
					throw new Exception(e.Message, e);
				}
			}
			addedID.Clear();
			if (wasAdded) RefreshDetail();
			return true;
		}


		private void afterRowInsert(object sender, UltraGridRow row)
		{
			row.Cells["USERID"].Value = ClientAuthentication.UserID;
		}

		protected override void AfterDetailSaveData()
		{
			if (addedID.Count == 0)
			{
                var activeRow = GetActiveDataRow();
                if (activeRow != null)
                {
                    var o = activeRow["ID"];
                    if (o != DBNull.Value)
                    {
                        Int32 id = Convert.ToInt32(o);
                        Service.GetPercentOfComplete(id);
                    }
                }
				//Refresh();
			}
		}

		protected override void AfterLoadDetailData(ref DataSet detail)
		{
			foreach (DataTable dt in detail.Tables)
			{
				if (!dt.Columns.Contains("DESIGNATION"))
					continue; 
				dt.BeginLoadData();
				foreach (DataRow dr in dt.Rows)
				{
					if ((String.Compare(dr["DESIGNATION"].ToString(), "%")) == 0)
					{
						if (dr["VALUEBASE"] != DBNull.Value)
							dr["VALUEBASE"] = Convert.ToDecimal(dr["VALUEBASE"]) * 100;
						if (dr["VALUEESTIMATE"] != DBNull.Value)
							dr["VALUEESTIMATE"] = Convert.ToDecimal(dr["VALUEESTIMATE"]) * 100;
						if (dr["VALUEY1"] != DBNull.Value)
							dr["VALUEY1"] = Convert.ToDecimal(dr["VALUEY1"]) * 100;
						if (dr["VALUEY2"] != DBNull.Value)
							dr["VALUEY2"] = Convert.ToDecimal(dr["VALUEY2"]) * 100;
						if (dr["VALUEY3"] != DBNull.Value)
							dr["VALUEY3"] = Convert.ToDecimal(dr["VALUEY3"]) * 100;
						if (dr["VALUEY4"] != DBNull.Value)
							dr["VALUEY4"] = Convert.ToDecimal(dr["VALUEY4"]) * 100;
						if (dr["VALUEY5"] != DBNull.Value)
							dr["VALUEY5"] = Convert.ToDecimal(dr["VALUEY5"]) * 100;
						if (dt.Columns.Contains("MINBOUND"))
						{
							if (dr["MINBOUND"] != DBNull.Value)
								dr["MINBOUND"] = Convert.ToDecimal(dr["MINBOUND"]) * 100;
						}
						if (dt.Columns.Contains("MAXBOUND"))
						{
							if (dr["MAXBOUND"] != DBNull.Value)
								dr["MAXBOUND"] = Convert.ToDecimal(dr["MAXBOUND"]) * 100;
						}
						dr.AcceptChanges();
					}
				}
				dt.EndLoadData();
			}

			RefreshVisualizator();
		}

		private void RefreshVisualizator()
		{			
            IValidatorMessageHolder vmh2 = ScenarioValidation.Validate(CalcedValidation);
            if (MessagesVisualizator != null)
            {
                MessagesVisualizator.Hide();
            }

			//Refresh();

			MessagesVisualizator = new ForecastMessagesVisualizator(vmh2);
		}

		protected override void BeforeDetailSaveData(ref DataSet detail)
		{
			foreach (DataTable dt in detail.Tables)
			{
				if (!dt.Columns.Contains("DESIGNATION"))
					continue;
				dt.BeginLoadData();
				foreach (DataRow dr in dt.Rows)
				{
					if ((String.Compare(dr["DESIGNATION"].ToString(), "%")) == 0)
					{						
						if (dr["VALUEBASE"] != DBNull.Value)
							dr["VALUEBASE"] = Convert.ToDecimal(dr["VALUEBASE"]) / 100;
						if (dr["VALUEESTIMATE"] != DBNull.Value)
							dr["VALUEESTIMATE"] = Convert.ToDecimal(dr["VALUEESTIMATE"]) / 100;
						if (dr["VALUEY1"] != DBNull.Value)
							dr["VALUEY1"] = Convert.ToDecimal(dr["VALUEY1"]) / 100;
						if (dr["VALUEY2"] != DBNull.Value)
							dr["VALUEY2"] = Convert.ToDecimal(dr["VALUEY2"]) / 100;
						if (dr["VALUEY3"] != DBNull.Value)
							dr["VALUEY3"] = Convert.ToDecimal(dr["VALUEY3"]) / 100;
						if (dr["VALUEY4"] != DBNull.Value)
							dr["VALUEY4"] = Convert.ToDecimal(dr["VALUEY4"]) / 100;
						if (dr["VALUEY5"] != DBNull.Value)
							dr["VALUEY5"] = Convert.ToDecimal(dr["VALUEY5"]) / 100;
						if (dt.Columns.Contains("MINBOUND"))
						{
							if (dr["MINBOUND"] != DBNull.Value)
								dr["MINBOUND"] = Convert.ToDecimal(dr["MINBOUND"]) / 100;
						}
						if (dt.Columns.Contains("MAXBOUND"))
						{
							if (dr["MAXBOUND"] != DBNull.Value)
								dr["MAXBOUND"] = Convert.ToDecimal(dr["MAXBOUND"]) / 100;
						}
					}
				}
				dt.EndLoadData();
			}
		}
		
	}
	
}
