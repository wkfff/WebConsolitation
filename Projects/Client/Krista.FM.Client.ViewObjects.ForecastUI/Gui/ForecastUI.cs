using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Forecast;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	public abstract partial class BaseForecastUI : BaseClsUI
	{
		internal ImageList il;
		internal ImageList ilButtons;
		//private IForecastBaseService service;

		public BaseForecastUI(IEntity dataObject)
			: base(dataObject)
        {
            /*this.service = service;
            clsClassType = ClassTypes.clsFactData;
            
            InfragisticsRusification.LocalizeAll();*/
        }

		/*public IForecastBaseService Service
		{
			get { return service; }
		}*/
		
		/// <summary>
		/// Инициализация.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			((BaseClsView)ViewCtrl).utcDataCls.Tabs[1].Visible = false;
			
			//Создание и настройка тулбара Мастера
			UltraToolbar utbForecast = new UltraToolbar("Forecast");
			utbForecast.DockedColumn = 0;
			utbForecast.DockedRow = 1;
			utbForecast.Text = "Forecast";
			utbForecast.Visible = true;
			
			//Подключение тулбара
			((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars.AddRange(new UltraToolbar[] { utbForecast });

			il = new ImageList();
			il.Images.Add("over_down", Resources.ru.over_down);
			il.Images.Add("over_up", Resources.ru.over_up);

			ilButtons = new ImageList();
			ilButtons.TransparentColor = Color.Magenta;
			ilButtons.Images.Add("calc", Resources.ru.calculator);
			ilButtons.Images.Add("filter", Resources.ru.filter);
			ilButtons.Images.Add("group", Resources.ru.group);
			ilButtons.Images.Add("m_ready", Resources.ru.mark_ready);
			ilButtons.Images.Add("paste_from", Resources.ru.paste_from);
			ilButtons.Images.Add("calc_val", Resources.ru.calculator_val);
			ilButtons.Images.Add("autofill", Resources.ru.autofill);
			ilButtons.Images.Add("save2minec", Resources.ru.save2minec);
			ilButtons.Images.Add("pump", Resources.ru.pump);

		}
	

		protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
		{
			dataQuery = GetDataSourcesFilter();
			filterStr = dataQuery;
			return ((IEntity)ActiveDataObj).GetDataUpdater(dataQuery, null, null);
		}

		/// <summary>
		/// Возвращает текущую строку.
		/// </summary>
		/// <returns></returns>
		public DataRow GetActiveDataRow()
		{
			var o = vo.ugeCls._ugData.ActiveRow.Cells["ID"].Value;
            if (o != DBNull.Value)
            {
                int activeRowID = Convert.ToInt32(o);
                DataRow[] rows = ((DataView)((BindingSource)vo.ugeCls.ugData.DataSource).DataSource).ToTable().Select(String.Format("ID = {0}", activeRowID));
                return rows.GetLength(0) > 0 ? rows[0] : null;
            }
            else
            {
                return null;
            }
		}

		/// <summary>
		/// Возвращает загруженные в грид строчки.
		/// </summary>
		public DataRow[] GetDataRows()
		{
			return ((DataView)((BindingSource)vo.ugeCls.ugData.DataSource).DataSource).ToTable().Select();
		}

		/*
		/// <summary>
		/// Возвращает вкладку с деталью по ключу
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <returns></returns>
		public UltraTab GetDetailTab(String key)
		{
			return utcDetails.Tabs[key];
		}

		/// <summary>
		/// Возвращает активню вкладку
		/// </summary>
		/// <returns></returns>
		public UltraTab GetActiveDetailTab()
		{
			return utcDetails.ActiveTab;
		}

		/// <summary>
		/// Возвращает активный грид
		/// </summary>
		/// <returns></returns>
		public UltraGridEx GetActiveDetailGridEx()
		{
			if (GetActiveDetailTab().TabPage.Controls.Count > 0 && GetActiveDetailTab().TabPage.Controls[0] is UltraGridEx)
			{
				return (UltraGridEx)GetActiveDetailTab().TabPage.Controls[0];
			}
			return null;
		}

		/// <summary>
		/// Возвращает грид по ключу
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <returns></returns>
		public UltraGridEx GetDetailGridEx(String key)
		{
			if (utcDetails.Tabs[key].TabPage.Controls.Count > 0 && utcDetails.Tabs[key].TabPage.Controls[0] is UltraGridEx)
			{
				return (UltraGridEx)utcDetails.Tabs[key].TabPage.Controls[0];
			}
			return null;
		}*/

		internal void CustomizePlaningGrid()
		{
			((BaseClsView)ViewCtrl).ugeCls.ServerFilterEnabled = false;
			((BaseClsView)ViewCtrl).ugeCls.IsReadOnly = false;
			((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = true;
			((BaseClsView)ViewCtrl).ugeCls.AllowDeleteRows = true;
			((BaseClsView)ViewCtrl).ugeCls.AllowEditRows = true;
		}

		protected override AllowAddNew CheckAllowAddNew()
		{
			//utcDetails.ActiveTab
			//return AllowAddNew.No;
			return base.CheckAllowAddNew();
		}

		// Определение наличия у объекта источников данных
		public override bool HasDataSources()
		{
			return true;
		}

		protected override void AttachDataSources(IEntity obj)
		{
			base.AttachDataSources(obj);
			//DataSourcesHelper.scheme = this.Workplace.ActiveScheme;
		}

		public override ObjectType GetClsObjectType()
		{
			return ObjectType.FactTable;
		}

		public override object GetNewId()
		{
			try
			{
				return ((IFactTable)ActiveDataObj).GetGeneratorNextValue;
			}
			catch
			{
				return DBNull.Value;
			}
		}

		public override void SetTaskId(ref UltraGridRow row)
		{
			row.Cells["TASKID"].Value = -1;
		}
		
		/// <summary>
		/// Необходим для импорта экспорта в XML.
		/// </summary>
		/// <returns></returns>
		protected override IExportImporter GetExportImporter()
		{
			return Workplace.ActiveScheme.GetXmlExportImportManager().GetExportImporter(ObjectType.FactTable);
		}
		
		public virtual void SwitchGroup()
		{
			if (!GetActiveDetailGridEx().ugData.DisplayLayout.Bands[0].Columns.Exists("GroupName"))
				return;
			Boolean btnChecked = ((Infragistics.Win.UltraWinToolbars.StateButtonTool)GetActiveDetailGridEx().utmMain.Tools["btnGroupByGroup"]).Checked;
			if (btnChecked)
				GetActiveDetailGridEx().ugData.DisplayLayout.Bands[0].SortedColumns.Add("GroupName", false, true);
			else
				if (GetActiveDetailGridEx().ugData.DisplayLayout.Bands[0].SortedColumns.Exists("GroupName"))
					GetActiveDetailGridEx().ugData.DisplayLayout.Bands[0].SortedColumns.Remove("GroupName");
		}
	}

	public partial class ForecastUI: BaseForecastUI
	{
		private IForecastService service;

		private static readonly Dictionary<Int32, String> userDict = new Dictionary<Int32, String>();

		public ForecastUI(IForecastService service): base(service.Data)
		{ 
			this.service = service;
            clsClassType = ClassTypes.clsFactData;

			InfragisticsRusification.LocalizeAll();
        }

		public IForecastService Service
		{
			get { return service; }
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public static Dictionary<int, string> UserDict
		{
			get { return userDict; }
		}
		
		public static void UpdateUsersDict()
		{
			userDict.Clear();

			DataTable usr = ForecastNavigation.Instance.Workplace.ActiveScheme.UsersManager.GetUsers();
			foreach (DataRow row in usr.Rows)
			{
				userDict.Add(Convert.ToInt32(row["ID"]), Convert.ToString(row["NAME"]));
			}
		}

		protected void CheckBounds(String field, UltraGridRow row)
		{
			if (row.Cells.Exists(field) && !row.Cells[field].Hidden && row.Cells.Exists("MINBOUND") && row.Cells.Exists("MAXBOUND"))
			{
				/*if (row.Cells[field].Column.Style != ColumnStyle.Image) 
					return;*/
				if (row.Cells[field].Value == DBNull.Value) 
					return;
				Decimal value = Convert.ToDecimal(row.Cells[field].Value);
				if (Decimal.Compare(value, Convert.ToDecimal(row.Cells["MINBOUND"].Value)) == -1)
					row.Cells[field].Appearance.Image = il.Images["over_down"];
				else
					if (Decimal.Compare(value, Convert.ToDecimal(row.Cells["MAXBOUND"].Value)) == 1)
						row.Cells[field].Appearance.Image = il.Images["over_up"];
					else row.Cells[field].Appearance.Image = null;
			}
		}

		public void SwitchFilter()
		{
			if (!GetActiveDetailGridEx().ugData.DisplayLayout.Bands[0].Columns.Exists("ID"))
				return;
			Boolean btnChecked = ((Infragistics.Win.UltraWinToolbars.StateButtonTool)GetActiveDetailGridEx().utmMain.Tools["btnFilterOutOfRange"]).Checked;
			if (btnChecked)
			{
				GetActiveDetailGridEx().ugData.DisplayLayout.Bands[0].ColumnFilters["ID"].FilterConditions.Add(new CheckBoundsFilterConditions());
			}
			else
				GetActiveDetailGridEx().ugData.DisplayLayout.Bands[0].ColumnFilters["ID"].FilterConditions.Clear();
				
		}

		public override void UpdateToolbar()
		{
			//base.UpdateToolbar();
			//vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
			//vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
			//vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
			//vo.utbToolbarManager.Toolbars["utbSelectVariant"].Visible = true;

			UltraToolbar utbDForecast = new UltraToolbar("IndicatorsDetail");
			utbDForecast.DockedColumn = 3;
			utbDForecast.DockedRow = 0;
			utbDForecast.Text = "IndicatorsDetail";
			utbDForecast.Visible = true;
			GetDetailGridEx(SchemeObjectsKeys.t_S_Indicators_Key).utmMain.Toolbars.AddRange(new UltraToolbar[] { utbDForecast });

			//Подключение тулбара

			utbDForecast = new UltraToolbar("AdjustersDetail");
			utbDForecast.DockedColumn = 3;
			utbDForecast.DockedRow = 0;
			utbDForecast.Text = "AdjustersDetail";
			utbDForecast.Visible = true;
			GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).utmMain.Toolbars.AddRange(new UltraToolbar[] { utbDForecast });
		}
	
	}

	/// <summary>
	/// Используетсы длы ильтрации вышедших за границы параметров
	/// </summary>
	class CheckBoundsFilterConditions : FilterCondition
	{
		private static readonly string[] scenarioColName = new string[] { "VALUEESTIMATE", "VALUEY1", "VALUEY2", "VALUEY3", "VALUEY4", "VALUEY5" };
		private static readonly string[] valuationColName = new string[] { "V_EST_B", "V_Y1_B", "V_Y2_B", "V_Y3_B", "V_Y4_B", "V_Y5_B" };

		public override bool MeetsCriteria(UltraGridRow row)
		{
			if (!row.Cells.Exists("MINBOUND") || !row.Cells.Exists("MAXBOUND"))
				return true;
			Decimal min = Convert.ToDecimal(row.Cells["MINBOUND"].Value);
			Decimal max = Convert.ToDecimal(row.Cells["MAXBOUND"].Value);

			String[] colName;
			if (row.Cells.Exists("V_Y1_B")) 
				colName = valuationColName;
			else
				colName = scenarioColName;

			Boolean pass_check = true;
			foreach (String s in colName)
			{
				if (row.Cells.Exists(s) && (row.Cells[s].Value != DBNull.Value))
				{
					Decimal value = Convert.ToDecimal(row.Cells[s].Value);

					if (Decimal.Compare(value, min) == -1)
						pass_check = false;
					if (Decimal.Compare(value, max) == 1)
						pass_check = false;
				}
			}
						
			return !pass_check;
			//return base.MeetsCriteria(row);
		}
	}
}
