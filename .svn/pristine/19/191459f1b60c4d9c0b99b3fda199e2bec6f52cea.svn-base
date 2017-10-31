using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.ForecastUI.Commands;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Forecast;
using CC = Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	public partial class Form2pUI : BaseForecastUI
	{
		private IForm2pService service;

		#region necessary
		public Form2pUI(IForm2pService service)
			: base(service.Data)
		{
			this.service = service;
			clsClassType = ClassTypes.clsFactData;

			InfragisticsRusification.LocalizeAll();
		}

		public IForm2pService Service
		{
			get { return service; }
		}

		public override string Key
		{
			get
			{
				return SchemeObjectsKeys.f_S_Form2p_Key;
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			SetPermissions();

			UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["Forecast"];
			ButtonTool tool = CommandService.AttachToolbarTool(new FillParamCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["paste_from"];
			tool.SharedProps.Visible = canCreateNew;

			tool = CommandService.AttachToolbarTool(new FillFromScenCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["calc"];
			tool.SharedProps.Visible = canCalculate;

			tool = CommandService.AttachToolbarTool(new SaveToFormCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["save2minec"];

			/*tool = CommandService.AttachToolbarTool(new PumpDataCommand(), toolbar);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["pump"];

			tool = CommandService.AttachToolbarTool(new AltForecastCommand(), toolbar);*/

			//Добавление обработчиков при создании гридов. 
			((BaseClsView)ViewCtrl).ugeCls.OnAfterRowInsert += new AfterRowInsert(afterRowInsert);
			((BaseClsView)ViewCtrl).ugeCls.OnGridInitializeLayout += new GridInitializeLayout(ugeCls_OnGridForm2pInitializeLayout);
			DetailGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(Form2pUI_DetailGridInitializeLayout);
		}

		/// <summary>
		/// Фильтр 
		/// </summary>
		/// <param name="parentID"></param>
		/// <param name="filterStr"></param>
		/// <returns></returns>
		protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
		{
			dataQuery = GetDataSourcesFilter();// +" and (Parent is NULL)";
			filterStr = dataQuery;
			return ((IEntity)ActiveDataObj).GetDataUpdater(dataQuery, null, null);
		}
		#endregion

		protected override IDataUpdater GetDetailUpdater(IEntity activeDetailObject, object masterValue)
		{
			Int32 mv = Convert.ToInt32(masterValue);
			// Если MasterValue (mv) = -1, то активный вариант формы2п еще не существует vo.ugeCls.ugData.ActiveRow = null
			/// Следовательно возвращаем базовый GetDataUpdater
			if (mv == -1) return base.GetDetailUpdater(activeDetailObject, masterValue);
			/// если в MasterValue уже активный вариант, то вызываенм переопределенный GetDataUpdater
			return Service.GetForm2pDetailUpdater(String.Format("(refvarf2p = {0}) and (yearof = {1})", mv, Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["REFYEAR"].Value)));
		}

		/// <summary>
		/// Обработчик вызываемый при формировании тулбара
		/// </summary>
		public override void UpdateToolbar()
		{
			//base.UpdateToolbar();
						
			//Подключение тулбара

			UltraToolbar utbDForecast = new UltraToolbar("Form2pDetail");
			utbDForecast.DockedColumn = 3;
			utbDForecast.DockedRow = 0;
			utbDForecast.Text = "Form2pDetail";
			utbDForecast.Visible = true;
			GetDetailGridEx(SchemeObjectsKeys.t_s_Form2p_Key).utmMain.Toolbars.AddRange(new UltraToolbar[] { utbDForecast });

			vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
			vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
			vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
			//vo.ugeCls.utmMain.Tools["deleteSelectedRows"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			//vo.ugeCls.utmMain.Tools["menuLoad"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["PasteRow"].SharedProps.Visible = false;
			((Infragistics.Win.UltraWinToolbars.StateButtonTool)vo.ugeCls.utmMain.Tools["ShowGroupBy"]).Checked = false;

			UltraToolbar toolbar = GetDetailGridEx(SchemeObjectsKeys.t_s_Form2p_Key).utmMain.Toolbars["Form2pDetail"];
			StateButtonTool tool = CommandService.AttachToolbarCheckedTool(new GroupByGroup(), toolbar, String.Empty);
			tool.SharedProps.AppearancesSmall.Appearance.Image = ilButtons.Images["group"];

			UltraGridEx formGrid = GetDetailGridEx(SchemeObjectsKeys.t_s_Form2p_Key);
			formGrid.AllowAddNewRecords = false;
			ConfDetailsToolbar(formGrid.utmMain);
		}

		/// <summary>
		/// Инициализация грида мастера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ugeCls_OnGridForm2pInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			e.Layout.Bands[0].Columns["REFYEAR"].Header.Caption = "Оценочный год";
		}
		
		private void afterRowInsert(object sender, UltraGridRow row)
		{
			row.Cells["Name"].Value = "Форма 2П";
		}

		/// <summary>
		/// Обработчик события нажатия на кнопке в мастер гриде
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void ugeCls_OnClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{
			base.ugeCls_OnClickCellButton(sender, e);
			makeForm2pName(e.Cell.Row);
		}

		/// <summary>
		/// Обработчик нажатиия на кнопке в гриде деталей
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void ugeDetail_OnClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{
			if (e.Cell.Column.Key == "REFPARAMETRS_Lookup")
			{
				IClassifier cls = Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_Form2p_Key];
				// создаем объект просмотра классификаторов нужного типа
				DataClsUI clsUI = new DataClsUI(cls);
				clsUI.Workplace = Workplace;
				clsUI.RestoreDataSet = false;
				clsUI.Initialize();
				clsUI.InitModalCls(-1);
				DataRow dr = GetActiveDataRow();
				clsUI.CurrentDataSourceID = Convert.ToInt32(dr["SOURCEID"]);

				// создаем форму
				frmModalTemplate modalClsForm = new frmModalTemplate();
				modalClsForm.AttachCls(clsUI);

				// скрываем кнопку ОК 
				modalClsForm.HideBtnOk(true);
				ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);

				// ...загружаем данные
				clsUI.RefreshAttachedData();

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
			}
			else
				base.ugeDetail_OnClickCellButton(sender, e);
		}
	
		/// <summary>
		/// Инициализация грида деталей	
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form2pUI_DetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			if (vo.ugeCls.ugData.ActiveRow != null)
			{
				if (vo.ugeCls.ugData.ActiveRow.Cells["REFYEAR"].Value != DBNull.Value)
				{
					Int32 BaseYear = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["REFYEAR"].Value);
					e.Layout.Bands[0].Columns["R1"].Header.Caption = String.Format("Отчет {0}", (BaseYear - 2));
					e.Layout.Bands[0].Columns["R2"].Header.Caption = String.Format("Отчет {0}", (BaseYear - 1));
					e.Layout.Bands[0].Columns["EST"].Header.Caption = String.Format("Оценка {0}", (BaseYear));
					e.Layout.Bands[0].Columns["Y1"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 1));
					e.Layout.Bands[0].Columns["Y2"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 2));
					e.Layout.Bands[0].Columns["Y3"].Header.Caption = String.Format("Прогноз {0}", (BaseYear + 3));
					
				}
				
				e.Layout.Bands[0].Columns["REFPARAMETRS_Lookup"].Header.VisiblePosition = 1;
				if (e.Layout.Bands[0].Columns.Exists("R1"))
					e.Layout.Bands[0].Columns["R1"].Header.VisiblePosition = 2;
				if (e.Layout.Bands[0].Columns.Exists("R2"))
					e.Layout.Bands[0].Columns["R2"].Header.VisiblePosition = 3;
				if (e.Layout.Bands[0].Columns.Exists("EST"))
					e.Layout.Bands[0].Columns["EST"].Header.VisiblePosition = 4;
				if (e.Layout.Bands[0].Columns.Exists("Y1"))
					e.Layout.Bands[0].Columns["Y1"].Header.VisiblePosition = 5;
				if (e.Layout.Bands[0].Columns.Exists("Y2"))
					e.Layout.Bands[0].Columns["Y2"].Header.VisiblePosition = 6;
				if (e.Layout.Bands[0].Columns.Exists("Y3"))
					e.Layout.Bands[0].Columns["Y3"].Header.VisiblePosition = 7;

				e.Layout.Bands[0].Columns["ID"].Hidden = true;
				e.Layout.Bands[0].Columns["YEAROF"].Hidden = true;
				if (e.Layout.Bands[0].Columns.Exists("UNITS"))
				{
					e.Layout.Bands[0].Columns["UNITS"].Header.Caption = "Единица иземерения";
					e.Layout.Bands[0].Columns["UNITS"].CellActivation = Activation.ActivateOnly;
				}
				if (e.Layout.Bands[0].Columns.Exists("SIGNAT"))
					e.Layout.Bands[0].Columns["SIGNAT"].Hidden = true;

				if (e.Layout.Bands[0].Columns.Exists("GROUPNAME"))
				{
					e.Layout.Bands[0].Columns["GROUPNAME"].Hidden = true;
					e.Layout.Bands[0].Columns["GROUPNAME"].Header.Caption = "Группа";
				}
				
				e.Layout.Bands[0].Columns["REFPARAMETRS_Lookup"].CellActivation = Activation.ActivateOnly;
				e.Layout.Bands[0].Columns["REFPARAMETRS"].CellActivation = Activation.ActivateOnly;
			}
			e.Layout.Bands[0].Columns["REFPARAMETRS"].SortIndicator = SortIndicator.Ascending;
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
				if (e.Row.Cells.Exists("SIGNAT") && (e.Row.Cells["SIGNAT"].Value != DBNull.Value))
				{
					String signat = e.Row.Cells["SIGNAT"].Value.ToString();
					if (signat.Length == 5)
						e.Row.Appearance.BackColor = Color.LightGray;
					if (signat.Contains("_"))
					{
						String[] parts = signat.Split('_');
						for (Int32 i = 1; i < parts.Length; i++)
						{
							if (parts[i].StartsWith("I"))
							{
								Int32 k = Convert.ToInt32(parts[i].Substring(1));
								if ((k & 0x01) != 0x00)
									e.Row.Cells["R2"].Appearance.BackColor = Color.YellowGreen;
								if ((k & 0x02) != 0x00)
									e.Row.Cells["R1"].Appearance.BackColor = Color.YellowGreen;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Метод генерирует имя для формы2п на основе года, территории и варианта
		/// </summary>
		/// <param name="row"></param>
		private static void makeForm2pName(UltraGridRow row)
		{
			String year = row.Cells["REFYEAR"].Value.ToString();
			
			String var = String.Empty;
			String[] s = row.Cells["REFFORECAST_Lookup"].Value.ToString().Split(' ');
			if (s.Length > 0)
			{
				var = String.Join(" ", s, 0, s.Length-1);
			}
			
			String terr = String.Empty;
			s = row.Cells["REFTERRITORY_Lookup"].Value.ToString().Split(' ');			
			if (s.Length > 0)
			{
				terr = String.Join(" ", s, 0, s.Length-1);
			}

			row.Cells["Name"].Value = String.Format("Форма 2П {0}, Вариант: {1}, Регион: {2}", year, var, terr);
		}

		public Boolean checkFillingStrictParam()
		{
			foreach (DataRow dr in dsDetail.Tables[0].Rows)
			{
				String signat = dr["SIGNAT"].ToString();
				if (signat.Contains("_"))
				{
					String[] parts = signat.Split('_');
					for (Int32 i = 1; i < parts.Length; i++)
					{
						if (parts[i].StartsWith("I"))
						{
							Int32 k = Convert.ToInt32(parts[i].Substring(1));
							if ((k & 0x01) != 0x00)
								if (dr["R2"] == DBNull.Value)
									return false;

							if ((k & 0x02) != 0x00)
								if (dr["R1"] == DBNull.Value)
									return false;
						}
					}
				}
				
			}
			return true;
		}

		// <summary>
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

	}
}
