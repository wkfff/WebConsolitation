using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
using CC = Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	/// <summary>
	/// Класс пользовательского интерфейса "Варианты расчета"
	/// </summary>
	public class ValuationUI : ForecastUI
	{
		private ContextMenuStrip cmsIdicPlan = new ContextMenuStrip();
		private ToolStripMenuItem miCheck = new ToolStripMenuItem();
		private ToolStripMenuItem miUnCheck = new ToolStripMenuItem();
		private MasterRows masterRows = new MasterRows();

		public ValuationUI(IForecastService service) : base(service){}

		public override string Key
		{
			get
			{
				return SchemeObjectsKeys.f_S_Valuation_Key;
			}
		}

        // содержит все визуализаторы, которые при проверке загорятся
        private ForecastMessagesVisualizator messagesVisualizator;
        internal ForecastMessagesVisualizator MessagesVisualizator
        {
            get { return messagesVisualizator; }
            set { messagesVisualizator = value; }
        }

        internal MasterRows mRows
        {
            get { return masterRows; }
        }
		
		public override void Initialize()
		{
			base.Initialize();
			
			callClassName = "Valuation";

			SetPermissions();

			UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["Forecast"];
			ButtonTool tool = CommandService.AttachToolbarTool(new CalcValuationCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["calc"];
			tool.SharedProps.Visible = canCalculate;

			tool = CommandService.AttachToolbarTool(new CalcValuationWithValidCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["calc_val"];
			tool.SharedProps.Visible = canCalculate;

			tool = CommandService.AttachToolbarTool(new IdicPlanCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["calc_val"]; /////////////// need for new icon
			tool.SharedProps.Visible = canCalculate;
			
			((BaseClsView)ViewCtrl).ugeCls.OnAfterRowInsert += new AfterRowInsert(afterRowInsert);
			((BaseClsView)ViewCtrl).ugeCls.OnGridInitializeLayout += new GridInitializeLayout(ugeCls_OnGridValuationInitializeLayout);
			((BaseClsView)ViewCtrl).ugeCls.OnInitializeRow += new InitializeRow(OnInitializeRow);
			DetailGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ValuationUI_DetailGridInitializeLayout);
			((BaseClsView)ViewCtrl).ugeCls.OnSaveChanges += new SaveChanges(ugeCls_OnSaveChanges);

			InitContextMenu();
		}

		public override bool CheckVisibility(String key)
		{
			if (key == SchemeObjectsKeys.t_S_Static_Key || key == SchemeObjectsKeys.t_S_UnReg_Key) 
				return false;
			return true;
		}

		private void InitContextMenu()
		{
			cmsIdicPlan.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miCheck, miUnCheck });
			cmsIdicPlan.Name = "cmsIdicPlan";
			cmsIdicPlan.Size = new System.Drawing.Size(206, 48);

			miCheck.Name = "miCheck";
			miCheck.Size = new System.Drawing.Size(205, 22);
			miCheck.Text = "Пометить для прогноза";

			miUnCheck.Name = "miUnCheck";
			miUnCheck.Size = new System.Drawing.Size(205, 22);
			miUnCheck.Text = "Снять отметку";

			cmsIdicPlan.Opening +=new System.ComponentModel.CancelEventHandler(cmsIdicPlan_Opening);
			miCheck.Click += new EventHandler(miCheck_Click);
			miUnCheck.Click += new EventHandler(miUnCheck_Click);
		}

		//private Dictionary<Int32, String> adjForPlan = new Dictionary<Int32, String>();
		//private Dictionary<Int32, String> indForPlan = new Dictionary<Int32, String>();

		private void miCheck_Click(object sender, EventArgs e)
		{
			UltraGridCell activeCell = GetActiveDetailGridEx().ugData.ActiveCell;
			
			if (activeCell == null) return;

			Int32 masterID = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["ID"].Value);
			
			MasterRow mr = masterRows[masterID];
			DetailCells dc = mr[utcDetails.ActiveTab.Key];

			if (!dc.Contains(activeCell.Row.Index))
			{
				dc.Add(activeCell.Row.Index, activeCell.Column.Key);
				activeCell.Appearance.BackColor = Color.DarkSalmon;
			}

			/*
			if (utcDetails.ActiveTab.Key == SchemeObjectsKeys.t_S_Adjusters_Key)
			{
				if (!AdjForPlan.ContainsKey(activeCell.Row.Index))
				{
					AdjForPlan.Add(activeCell.Row.Index, activeCell.Column.Key);
					activeCell.Appearance.BackColor = Color.DarkSalmon;
				}
			}
			if (utcDetails.ActiveTab.Key == SchemeObjectsKeys.t_S_Indicators_Key)
			{
				if (!IndForPlan.ContainsKey(activeCell.Row.Index))
				{
					IndForPlan.Add(activeCell.Row.Index, activeCell.Column.Key);
					activeCell.Appearance.BackColor = Color.DarkSalmon;
				}
			}*/
		}

		private void miUnCheck_Click(object sender, EventArgs e)
		{
			UltraGridCell activeCell = GetActiveDetailGridEx().ugData.ActiveCell;

			if (activeCell == null) return;

			Int32 masterID = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["ID"].Value);
			MasterRow mr = masterRows[masterID];
			DetailCells dc = mr[utcDetails.ActiveTab.Key];

			if (dc.Contains(activeCell.Row.Index) &&
					(dc[activeCell.Row.Index] == activeCell.Column.Key))
			{
				dc.Remove(activeCell.Row.Index);
				activeCell.Appearance.ResetBackColor();
			}

			/*

			if (utcDetails.ActiveTab.Key == SchemeObjectsKeys.t_S_Adjusters_Key)
				if (AdjForPlan.ContainsKey(activeCell.Row.Index) &&
					(AdjForPlan[activeCell.Row.Index] == activeCell.Column.Key))
				{
					AdjForPlan.Remove(activeCell.Row.Index);
					activeCell.Appearance.ResetBackColor();
				}

			if (utcDetails.ActiveTab.Key == SchemeObjectsKeys.t_S_Indicators_Key)
				if (IndForPlan.ContainsKey(activeCell.Row.Index) &&
					(IndForPlan[activeCell.Row.Index] == activeCell.Column.Key))
				{
					IndForPlan.Remove(activeCell.Row.Index);
					activeCell.Appearance.ResetBackColor();
				}*/
			
		}

		private void ugData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
		{
			UltraGridCell activeCell = GetActiveDetailGridEx().ugData.ActiveCell;
			Int32 masterID = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["ID"].Value);
			MasterRow mr = masterRows[masterID];
			DetailCells dc = mr[utcDetails.ActiveTab.Key];

			if (activeCell != null)
			{
				switch (activeCell.Column.Key)
				{
					case "VALUEESTIMATE":
					case "VALUEY1":
					case "VALUEY2":
					case "VALUEY3":
					case "VALUEY4":
					case "VALUEY5":
						if (dc.Contains(e.Cell.Row.Index))
						{
							if (dc[e.Cell.Row.Index] == e.Cell.Column.Key)
								miUnCheck_Click(this, (EventArgs)e);
						}
						else
						{
							miCheck_Click(this, (EventArgs)e);
						}
						/*if (utcDetails.ActiveTab.Key == SchemeObjectsKeys.t_S_Indicators_Key)
						{
							if (IndForPlan.ContainsKey(e.Cell.Row.Index))
							{
								if (IndForPlan[e.Cell.Row.Index] == e.Cell.Column.Key)
									miUnCheck_Click(this, (EventArgs)e);
							}
							else
							{
								miCheck_Click(this, (EventArgs)e);
							}
						}
						if (utcDetails.ActiveTab.Key == SchemeObjectsKeys.t_S_Adjusters_Key)
						{
							if (AdjForPlan.ContainsKey(e.Cell.Row.Index))
							{
								if (AdjForPlan[e.Cell.Row.Index] == e.Cell.Column.Key)
									miUnCheck_Click(this, (EventArgs)e);
							}
							else
							{
								miCheck_Click(this, (EventArgs)e);
							}
						}*/
						break;
					default:
						break;
				}
			}
		}
		
		private void cmsIdicPlan_Opening(object sender, CancelEventArgs e)
		{
			UltraGridCell activeCell = GetActiveDetailGridEx().ugData.ActiveCell;

			Int32 masterID = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["ID"].Value);
			MasterRow mr = masterRows[masterID];
			DetailCells dc = mr[utcDetails.ActiveTab.Key];
			
			if (activeCell != null)
			{
				switch (activeCell.Column.Key)
				{
					case "VALUEESTIMATE":
					case "VALUEY1":
					case "VALUEY2":
					case "VALUEY3":
					case "VALUEY4":
					case "VALUEY5":
						if (dc.Contains(activeCell.Row.Index))
						{
							miCheck.Enabled = false;
							if (dc[activeCell.Row.Index] == activeCell.Column.Key)
								miUnCheck.Enabled = true;
							else
								miUnCheck.Enabled = false;
						}
						else
						{
							miCheck.Enabled = true;
							miUnCheck.Enabled = false;
						}
						break;
					default:
						e.Cancel = true;
						break;
				}
			}
			else e.Cancel = true;
		}
	
		private void afterRowInsert(object sender, UltraGridRow row)
		{
			ScenarioModal sm = new ScenarioModal();
			Int32 ID = sm.ShowScenarioModal();
			if (ID != -1)
			{
				row.Cells["PARENT"].Value = ID;

				String query = String.Format("select REFYEAR from {0} where id={1}", Service.Data.FullDBName, ID);
				IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB;
				Int32 refYear;
				try
				{
					Object o = db.ExecQuery(query, QueryResultTypes.Scalar);
					refYear = Convert.ToInt32(o);
				}
				finally
				{
					db.Dispose();
				}
				row.Cells["REFYEAR"].Value = refYear;
				row.Cells["READYTOCALC"].Value = 2;
				row.Cells["USERID"].Value = ClientAuthentication.UserID;
			}
			else
				row.CancelUpdate();
		}
		

		/// <summary>
		/// Фильтр выделяет только варианты расчета (Parent is NOT NULL)
		/// </summary>
		/// <param name="parentID"></param>
		/// <param name="filterStr"></param>
		/// <returns></returns>
		protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
		{
			dataQuery = GetDataSourcesFilter() + " and (Parent is NOT NULL)";
			filterStr = dataQuery;
			return ((IEntity)ActiveDataObj).GetDataUpdater(dataQuery, null, null);
		}

		protected override IDataUpdater GetDetailUpdater(IEntity activeDetailObject, object masterValue)
		{
			Int32 mv = Convert.ToInt32(masterValue);
			switch (activeDetailObject.ObjectKey) 
			{
				case SchemeObjectsKeys.t_S_Adjusters_Key:
					/// Если MasterValue (mv) = -1, то активный вариант расчета еще не существует vo.ugeCls.ugData.ActiveRow = null
					/// Следовательно возвращаем базовый GetDataUpdater
					if (mv == -1) return base.GetDetailUpdater(activeDetailObject, masterValue);
					/// если в MasterValue уже активный сценарий, то вызываенм переопределенный GetDataUpdater
					return Service.GetValuationAdjustersUpdater(String.Format("(basescenario = {0}) and (refscenario = {1})", Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["Parent"].Value), mv));
				case SchemeObjectsKeys.t_S_Indicators_Key:
					/// Если MasterValue (mv) = -1, то активный вариант расчета еще не существует vo.ugeCls.ugData.ActiveRow = null
					/// Следовательно возвращаем базовый GetDataUpdater
					if (mv == -1) return base.GetDetailUpdater(activeDetailObject, masterValue);
					/// если в MasterValue уже активный сценарий, то вызываенм переопределенный GetDataUpdater
					return Service.GetValuationIndicatorsUpdater(String.Format("(basescenario = {0}) and (refscenario = {1})", Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["Parent"].Value), mv));
				default: 
					return base.GetDetailUpdater(activeDetailObject, masterValue);
			}
			
		}


		/// <summary>
		/// Инициализация грида мастера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ugeCls_OnGridValuationInitializeLayout(object sender, InitializeLayoutEventArgs e)
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

			if (!e.Layout.Bands[0].Columns.Exists("Parent_Lookup"))
			{
				UltraGridColumn usrCol = e.Layout.Bands[0].Columns.Add("Parent_Lookup", "Родительский сценарий");
				usrCol.CellActivation = Activation.ActivateOnly;
			}

			e.Layout.Bands[0].Columns["REFYEAR"].Header.Caption = "Базовый год";

			e.Layout.Bands[0].Columns["NAME"].Header.VisiblePosition = 1;
			e.Layout.Bands[0].Columns["UserID_Lookup"].Header.VisiblePosition = 2;
			e.Layout.Bands[0].Columns["Parent_Lookup"].Header.VisiblePosition = 3;
			
			if (canAssignParam && canCalculate)
			{
				UltraGridHelper.SetLikelyEditButtonColumnsStyle(((UltraGrid)sender).DisplayLayout.Bands[0].Columns["UserID_Lookup"], 0);
			}

			((UltraGrid)sender).DisplayLayout.Bands[0].Columns["REFYEAR"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
			UpdateUsersDict();
		}

		/// <summary>
		/// Заполняет значения лукапных полей
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

				if (e.Row.Cells.Exists("Parent_Lookup") && e.Row.Cells["Parent"].Value != DBNull.Value)
				{
					Int32 id = Convert.ToInt32(e.Row.Cells["Parent"].Value);
					e.Row.Cells["Parent_Lookup"].Value = String.Format("{0} ({1})",
						Service.GetParentScenarioName(id), id);
				}
			}
		}

		protected override void InitializeDetailRow(object sender, InitializeRowEventArgs e)
		{
			base.InitializeDetailRow(sender, e);
			if (e.Row.IsDataRow)
			{
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
						case SchemeObjectsKeys.t_S_UnReg_Key:
							if ((Convert.ToByte(e.Row.Cells["MASK"].Value) & 0x20) == 0)
							{
								e.Row.Cells["VALUEESTIMATE"].Activation = Activation.Disabled;
								e.Row.Cells["VALUEESTIMATE"].Appearance.BackColor = Color.LightGray;
							}
							break;
					}
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
				CheckBounds("V_EST_B", e.Row);
				CheckBounds("V_Y1_B", e.Row);
				CheckBounds("V_Y2_B", e.Row);
				CheckBounds("V_Y3_B", e.Row);
				CheckBounds("V_Y4_B", e.Row);
				CheckBounds("V_Y5_B", e.Row);
			}

		}

		/// <summary>
		/// Задает маску ввода для вновь созданных колонок
		/// </summary>
		/// <param name="clmn"></param>
		private void SetMask(UltraGridColumn clmn)
		{
			clmn.MaskInput = "-nnnnnnnnnnnnnnn.nnnn";
			clmn.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
			clmn.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
			clmn.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
			
			clmn.Editor = new Infragistics.Win.EditorWithMask();
			clmn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
			clmn.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
		}
		
		/// <summary>
		/// Инициализация грида деталей	
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ValuationUI_DetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			e.Layout.Bands[0].Columns["ID"].Hidden = true;

			if (e.Layout.Bands[0].Columns.Exists("BASESCENARIO")) 
				e.Layout.Bands[0].Columns["BASESCENARIO"].Hidden = true;
			if (e.Layout.Bands[0].Columns.Exists("REFPARAMS_Lookup"))
			{
				e.Layout.Bands[0].Columns["REFPARAMS_Lookup"].Header.VisiblePosition = 1;
				e.Layout.Bands[0].Columns["REFPARAMS_Lookup"].CellActivation = Activation.ActivateOnly;
			}

			if (e.Layout.Bands[0].Columns.Exists("VALUEESTIMATE")) 
				e.Layout.Bands[0].Columns["VALUEESTIMATE"].CellActivation = Activation.ActivateOnly;
			if (e.Layout.Bands[0].Columns.Exists("VALUEY1")) 
				e.Layout.Bands[0].Columns["VALUEY1"].CellActivation = Activation.ActivateOnly;
			if (e.Layout.Bands[0].Columns.Exists("VALUEY2")) 
				e.Layout.Bands[0].Columns["VALUEY2"].CellActivation = Activation.ActivateOnly;
			if (e.Layout.Bands[0].Columns.Exists("VALUEY3")) 
				e.Layout.Bands[0].Columns["VALUEY3"].CellActivation = Activation.ActivateOnly;
			if (e.Layout.Bands[0].Columns.Exists("VALUEY4")) 
				e.Layout.Bands[0].Columns["VALUEY4"].CellActivation = Activation.ActivateOnly;
			if (e.Layout.Bands[0].Columns.Exists("VALUEY5")) 
				e.Layout.Bands[0].Columns["VALUEY5"].CellActivation = Activation.ActivateOnly;
			
			e.Layout.Bands[0].Columns["REFPARAMS"].CellActivation = Activation.ActivateOnly;


			if (e.Layout.Bands[0].Columns.Exists("VALUEBASE")) 
				e.Layout.Bands[0].Columns["VALUEBASE"].Hidden = true;
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
					//e.Layout.Bands[0].Columns["VALUEBASE"].Header.VisiblePosition = 2;
					e.Layout.Bands[0].Columns["VALUEESTIMATE"].Header.Caption = String.Format("Оценка {0}", (BaseYear + 1));
					e.Layout.Bands[0].Columns["VALUEY1"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 2));
					e.Layout.Bands[0].Columns["VALUEY2"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 3));
					e.Layout.Bands[0].Columns["VALUEY3"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 4));
					e.Layout.Bands[0].Columns["VALUEY4"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 5));
					e.Layout.Bands[0].Columns["VALUEY5"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 6));
				}
			}

			if (e.Layout.Bands[0].Columns.Exists("USERID")) 
				e.Layout.Bands[0].Columns["USERID"].Hidden = true;
			GetActiveDetailGridEx().OnInitializeRow += new InitializeRow(OnInitializeRow);

			/*if (!e.Layout.Bands[0].Columns.Exists("UserID_Lookup"))
			{
				UltraGridColumn usrCol = e.Layout.Bands[0].Columns.Add("UserID_Lookup", "Ответственное лицо");
				usrCol.CellActivation = Activation.ActivateOnly;
			}*/
			//UltraGridHelper.SetLikelyEditButtonColumnsStyle(((UltraGrid)sender).DisplayLayout.Bands[0].Columns["UserID_Lookup"], 0);
			if (e.Layout.Bands[0].Columns.Exists("REFSCENARIO"))
			{
				e.Layout.Bands[0].Columns["REFSCENARIO"].Hidden = true;
			}
			if (e.Layout.Bands[0].Columns.Exists("V_EST_B"))
			{
				e.Layout.Bands[0].Columns["V_EST_B"].Header.Caption = e.Layout.Bands[0].Columns["VALUEESTIMATE"].Header.Caption + " (вариант)";
				e.Layout.Bands[0].Columns["V_EST_B"].Header.VisiblePosition = 2;
				e.Layout.Bands[0].Columns["VALUEESTIMATE"].Header.Caption += " (сценарий)";
				e.Layout.Bands[0].Columns["VALUEESTIMATE"].Header.VisiblePosition = 3;
				SetMask(e.Layout.Bands[0].Columns["V_EST_B"]);
			}
			if (e.Layout.Bands[0].Columns.Exists("V_Y1_B"))
			{
				e.Layout.Bands[0].Columns["V_Y1_B"].Header.Caption = e.Layout.Bands[0].Columns["VALUEY1"].Header.Caption + "  (вариант)";
				e.Layout.Bands[0].Columns["V_Y1_B"].Header.VisiblePosition = 4;
				e.Layout.Bands[0].Columns["VALUEY1"].Header.Caption += "  (сценарий)";
				e.Layout.Bands[0].Columns["VALUEY1"].Header.VisiblePosition = 5;
				SetMask(e.Layout.Bands[0].Columns["V_Y1_B"]);
			}
			if (e.Layout.Bands[0].Columns.Exists("V_Y2_B"))
			{
				e.Layout.Bands[0].Columns["V_Y2_B"].Header.Caption = e.Layout.Bands[0].Columns["VALUEY2"].Header.Caption + "  (вариант)";
				e.Layout.Bands[0].Columns["V_Y2_B"].Header.VisiblePosition = 6;
				e.Layout.Bands[0].Columns["VALUEY2"].Header.Caption += "  (сценарий)";
				e.Layout.Bands[0].Columns["VALUEY2"].Header.VisiblePosition = 7;
				SetMask(e.Layout.Bands[0].Columns["V_Y2_B"]);
			}
			if (e.Layout.Bands[0].Columns.Exists("V_Y3_B"))
			{
				e.Layout.Bands[0].Columns["V_Y3_B"].Header.Caption = e.Layout.Bands[0].Columns["VALUEY3"].Header.Caption + "  (вариант)";
				e.Layout.Bands[0].Columns["V_Y3_B"].Header.VisiblePosition = 8;
				e.Layout.Bands[0].Columns["VALUEY3"].Header.Caption += "  (сценарий)";
				e.Layout.Bands[0].Columns["VALUEY3"].Header.VisiblePosition = 9;
				SetMask(e.Layout.Bands[0].Columns["V_Y3_B"]);
			}
			if (e.Layout.Bands[0].Columns.Exists("V_Y4_B"))
			{
				e.Layout.Bands[0].Columns["V_Y4_B"].Header.Caption = e.Layout.Bands[0].Columns["VALUEY4"].Header.Caption + "  (вариант)";
				e.Layout.Bands[0].Columns["V_Y4_B"].Header.VisiblePosition = 10;
				e.Layout.Bands[0].Columns["VALUEY4"].Header.Caption += "  (сценарий)";
				e.Layout.Bands[0].Columns["VALUEY4"].Header.VisiblePosition = 11;
				SetMask(e.Layout.Bands[0].Columns["V_Y4_B"]);
			}
			if (e.Layout.Bands[0].Columns.Exists("V_Y5_B"))
			{
				e.Layout.Bands[0].Columns["V_Y5_B"].Header.Caption = e.Layout.Bands[0].Columns["VALUEY5"].Header.Caption + "  (вариант)";
				e.Layout.Bands[0].Columns["V_Y5_B"].Header.VisiblePosition = 12;
				e.Layout.Bands[0].Columns["VALUEY5"].Header.Caption += "  (сценарий)";
				e.Layout.Bands[0].Columns["VALUEY5"].Header.VisiblePosition = 13;
				SetMask(e.Layout.Bands[0].Columns["V_Y5_B"]);
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

			if ((vo.ugeCls.ugData.ActiveRow != null) && (vo.ugeCls.ugData.ActiveRow.Cells["ID"].Value != DBNull.Value))
			{
				Int32 masterID = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["ID"].Value);

				if (!masterRows.ExistRow(masterID))
				{
					MasterRow tmp_mr = masterRows.Add(masterID);
					tmp_mr.Add(SchemeObjectsKeys.t_S_Adjusters_Key);
					tmp_mr.Add(SchemeObjectsKeys.t_S_Indicators_Key);
				}

				UltraGrid activeGrid = GetActiveDetailGridEx().ugData;
				
				MasterRow mr = masterRows[masterID];

				foreach (KeyValuePair<Int32, String> pair in mr[utcDetails.ActiveTab.Key])
					activeGrid.Rows[pair.Key].Cells[pair.Value].Appearance.BackColor = Color.DarkSalmon;
			}
			//SwitchFilter();
			//SwitchGroup();
		}

		public override void ugeDetail_OnCreateGrid(UltraGridEx activeGridEx)
		{
			UltraGrid activeGrid = activeGridEx.ugData;
			activeGrid.ContextMenuStrip = cmsIdicPlan;
			activeGrid.DoubleClickCell += new DoubleClickCellEventHandler(ugData_DoubleClickCell);
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
				/*///Если базовый то выходим
				if ((ScenarioStatus)Convert.ToInt32(e.Cell.Row.Cells["READYTOCALC"].Value) == ScenarioStatus.BaseScenario) return;
				*/
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
				}

				clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();
				clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["Code_Remasked"].FilterConditions.Add(FilterComparisionOperator.Match, filtrStr);
				clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["Code_Remasked"].FilterConditions.Add(FilterComparisionOperator.DoesNotEndWith, "00");
				clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["ID"].SortIndicator = SortIndicator.Ascending;

				if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
				{
					int clsID = modalClsForm.AttachedCls.GetSelectedID();
					// если ничего не выбрали - считаем что функция завершилась неудачно
					if (clsID == -10)
						return;

					String columnName = e.Cell.Column.Key;
					columnName = CC.UltraGridEx.GetSourceColumnName(columnName);

					e.Cell.Row.Cells[columnName].Value = clsID;
				}
			}
			else
				base.ugeDetail_OnClickCellButton(sender, e);
		}

		public override void UpdateToolbar()
		{
			base.UpdateToolbar();

			UltraToolbar toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).utmMain.Toolbars["AdjustersDetail"];
			StateButtonTool tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["filter"];
			tool2 = CommandService.AttachToolbarCheckedTool(new GroupByGroup(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["group"];

			toolbar = GetDetailGridEx(SchemeObjectsKeys.t_S_Indicators_Key).utmMain.Toolbars["IndicatorsDetail"];
			tool2 = CommandService.AttachToolbarCheckedTool(new FilterOutOfRange(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["filter"];
			tool2 = CommandService.AttachToolbarCheckedTool(new GroupByGroup(), toolbar, String.Empty);
			tool2.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["group"];

			vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
			vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
			vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
			//vo.ugeCls.utmMain.Tools["deleteSelectedRows"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["menuLoad"].SharedProps.Visible = false;
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

			//SwitchFilter();
			//SwitchGroup();
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

		protected override void SetPermissionsToDetail()
		{
			//base.SetPermissionsToDetail();

		}


		/// <summary>
		/// Используется при сохранении изменений для поиска добавленых записей 
		/// в таблицу. Обновляется в методе OnBeforeSaveChanges(). Сбрасывается
		/// в методе ugeCls_OnSaveChanges(object sender)
		/// </summary>
		private Dictionary<Int32,Int32> addedID = new Dictionary<Int32,Int32>();

		protected override void BeforeSaveData()
		{
            
		}

        protected override void OnGetChangedAfterUpdate(DataSet dsObjData, DataSet changedRecords)
        {
            DataTable dt = dsObjData.Tables[0];
            
            List<DataRow> list = new List<DataRow>();

            Boolean isNull = false;

            foreach (DataRow row in dt.Rows)
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

            if (isNull)
            {
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
                        Int32 rowID = Convert.ToInt32(row["ID"]);

                        Int32 fromID = Convert.ToInt32(row["PARENT"]);
                        addedID.Add(rowID, fromID);
                    }
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)
                    {
                        Int32 rowID = Convert.ToInt32(dr["ID"]);

                        Int32 fromID = Convert.ToInt32(dr["PARENT"]);
                        addedID.Add(rowID, fromID);
                    }
                }
            }

            /*var i = 0;

            foreach (DataRow dr in dsObjData.Tables[0].Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Int32 rowID;
                    if (dr["ID"] == DBNull.Value)
                    {
                        object id = changedRecords.Tables[0].Rows[i]["ID"];
                        rowID = Convert.ToInt32(id);
                        i++;
                    }
                    else
                    {
                        rowID = Convert.ToInt32(dr["ID"]);

                    }
                    Int32 fromID = Convert.ToInt32(dr["PARENT"]);
                    addedID.Add(rowID, fromID);
                }
            }*/
        }

		bool ugeCls_OnSaveChanges(object sender)
		{
            Boolean wasAdded = false;
            var a = ((UltraGridEx)sender).DataSource;
			foreach (KeyValuePair<Int32, Int32> pair in addedID)
			{
				try
				{
					Service.CopyScenarioDetails(pair.Value, pair.Key, null);
					wasAdded = true;
				}
				catch (Exception e)
				{
					addedID.Remove(pair.Key);
					throw new Exception(e.Message, e);
				}
			}
			addedID.Clear();
            if (wasAdded)
            {
                Refresh();
            }
			return true;
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

						if (dr["V_EST_B"] != DBNull.Value)
							dr["V_EST_B"] = Convert.ToDecimal(dr["V_EST_B"]) * 100;
						if (dr["V_Y1_B"] != DBNull.Value)
							dr["V_Y1_B"] = Convert.ToDecimal(dr["V_Y1_B"]) * 100;
						if (dr["V_Y2_B"] != DBNull.Value)
							dr["V_Y2_B"] = Convert.ToDecimal(dr["V_Y2_B"]) * 100;
						if (dr["V_Y3_B"] != DBNull.Value)
							dr["V_Y3_B"] = Convert.ToDecimal(dr["V_Y3_B"]) * 100;
						if (dr["V_Y4_B"] != DBNull.Value)
							dr["V_Y4_B"] = Convert.ToDecimal(dr["V_Y4_B"]) * 100;
						if (dr["V_Y5_B"] != DBNull.Value)
							dr["V_Y5_B"] = Convert.ToDecimal(dr["V_Y5_B"]) * 100;
						
						if (dr["MINBOUND"] != DBNull.Value)
							dr["MINBOUND"] = Convert.ToDecimal(dr["MINBOUND"]) * 100;
						if (dr["MAXBOUND"] != DBNull.Value)
							dr["MAXBOUND"] = Convert.ToDecimal(dr["MAXBOUND"]) * 100;
						dr.AcceptChanges();
					}
				}
				dt.EndLoadData();
			}
		}

		protected override void BeforeDetailSaveData(ref DataSet detail)
		{
			/*DataSet ds = detail.GetChanges();
			if (ds == null)
				return;*/
			foreach (DataTable dt in detail.Tables)
			{
				if (!dt.Columns.Contains("DESIGNATION"))
					continue;
				dt.BeginLoadData();
				foreach (DataRow dr in dt.Rows)
				{
					if ((String.Compare(dr["DESIGNATION"].ToString(), "%")) == 0)
					{
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

						if (dr["V_EST_B"] != DBNull.Value)
							dr["V_EST_B"] = Convert.ToDecimal(dr["V_EST_B"]) / 100;
						if (dr["V_Y1_B"] != DBNull.Value)
							dr["V_Y1_B"] = Convert.ToDecimal(dr["V_Y1_B"]) / 100;
						if (dr["V_Y2_B"] != DBNull.Value)
							dr["V_Y2_B"] = Convert.ToDecimal(dr["V_Y2_B"]) / 100;
						if (dr["V_Y3_B"] != DBNull.Value)
							dr["V_Y3_B"] = Convert.ToDecimal(dr["V_Y3_B"]) / 100;
						if (dr["V_Y4_B"] != DBNull.Value)
							dr["V_Y4_B"] = Convert.ToDecimal(dr["V_Y4_B"]) / 100;
						if (dr["V_Y5_B"] != DBNull.Value)
							dr["V_Y5_B"] = Convert.ToDecimal(dr["V_Y5_B"]) / 100;
						
						if (dr["MINBOUND"] != DBNull.Value)
							dr["MINBOUND"] = Convert.ToDecimal(dr["MINBOUND"]) / 100;
						if (dr["MAXBOUND"] != DBNull.Value)
							dr["MAXBOUND"] = Convert.ToDecimal(dr["MAXBOUND"]) / 100;
					}
				}
				dt.EndLoadData();
			}
		}

	}

	/// <summary>
	/// Хранит словари с ключами (номер строки детали) и значениями (название столбца)
	/// Методы поиска, удаления и добавления записей в словари
	/// </summary>
	internal class DetailCells: IEnumerable<KeyValuePair<Int32,String>>
	{
		private String detailID;
		private Dictionary<Int32, String> cells = new Dictionary<Int32, String>();

		public String DetailID
		{
			get { return detailID; }
		}

		public Int32 Count
		{
			get { return cells.Count; }
		}

		public DetailCells(String detailID)
		{
			this.detailID = detailID;
		}

		public String this[Int32 detRowID]
		{
			get 
			{
				return cells[detRowID];
			}
		}

		public void Add(Int32 rowID, String colName)
		{
			cells.Add(rowID, colName);
		}

		public Boolean Remove(Int32 rowID)
		{
			return cells.Remove(rowID);
		}

		public Boolean Contains(Int32 rowID)
		{
			return cells.ContainsKey(rowID);
		}
		
		IEnumerator<KeyValuePair<int, string>> IEnumerable<KeyValuePair<int, string>>.GetEnumerator()
		{
			return cells.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<Int32, String>>) this).GetEnumerator();
		}
	}

	/// <summary>
	/// Описывает запись мастер-грида. Идентификатор строки в мастере.
	/// Методы добавления и поиска списков строк детеалей по ключу
	/// </summary>
	internal class MasterRow : IEnumerable<DetailCells>
	{
		private Int32 rowID;
		private List<DetailCells> detailCells = new List<DetailCells>();

		public MasterRow(Int32 id)
		{
			rowID = id;
		}

		public Boolean Add(String detailID)
		{
			Int32 count = Count;
			detailCells.Add(new DetailCells(detailID));
			return Count > count;
		}
		
		public Int32 Count
		{
			get { return detailCells.Count; }
		}

		public DetailCells this[String id]
		{
			get
			{
				return FindRow(id);
			}
		}

		public Boolean ExistRow(String id)
		{
			foreach (DetailCells item in detailCells)
			{
				if (item.DetailID == id)
					return true;
			}
			return false;
		}

		public DetailCells FindRow(String id)
		{
			foreach (DetailCells item in detailCells)
			{
				if (item.DetailID == id)
					return item;
			}
			return null;
		}

		#region IEnumerator
		private IEnumerable<DetailCells> BottomToTop
		{
			get
			{
				for (Int32 i = 0; i < detailCells.Count; i++)
					yield return detailCells[i];
			}
		}

		public int RowID
		{
			get { return rowID; }
		}

		IEnumerator<DetailCells> IEnumerable<DetailCells>.GetEnumerator()
		{
			return BottomToTop.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<DetailCells>)this).GetEnumerator();
		}
		#endregion
	}

	/// <summary>
	/// Класс хранит в себе список строк мастера. Методы для добавления,
	/// удаления и поиска записей по ID (номер строки в гриде)
	/// </summary>
	internal class MasterRows : IEnumerable<MasterRow>
	{
		private List<MasterRow> items = new List<MasterRow>();

		public Int32 Count
		{
			get { return items.Count; }
		}

		public MasterRow this[Int32 id]
		{
			get 
			{ 
				return FindRow(id); 
			}
		}
		
		public Boolean ExistRow(Int32 rowID)
		{
			foreach (MasterRow row in items)
			{
				if (row.RowID == rowID) 
					return true;
			}
			return false;
		}

		public MasterRow FindRow(Int32 rowID)
		{
			foreach (MasterRow row in items)
			{
				if (row.RowID == rowID)
					return row;
			}
			return null;
		}

		public MasterRow Add(Int32 rowID)
		{
			MasterRow mr = new MasterRow(rowID);
			items.Add(mr);
			return mr;
		}

		#region IEnumerator
		private IEnumerable<MasterRow> BottomToTop
		{
			get
			{
				for (Int32 i = 0; i < items.Count; i++)
					yield return items[i];
			}
		}

		IEnumerator<MasterRow> IEnumerable<MasterRow>.GetEnumerator()
		{
			return BottomToTop.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<MasterRow>)this).GetEnumerator();
		}
		#endregion
	}

}