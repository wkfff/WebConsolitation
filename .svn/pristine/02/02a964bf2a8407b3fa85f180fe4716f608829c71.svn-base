using System;
using System.Data;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Configuration;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	public abstract class EntityNavigationListUI : BaseViewObj
	{
        public EntityNavigationListUI(string key)
            : base(key)
        {
        }

		public override void Initialize()
		{
			base.Initialize();

			Grid.OnGridInitializeLayout += grid_OnGridInitializeLayout;
			Grid.OnRefreshData += grid_OnRefreshData;
			Grid.OnClickCellButton += grid_OnClickCellButton;
			Grid.OnGetGridColumnsState += Grid_OnGetGridColumnsState;
            Grid.OnInitializeRow += Grid_OnInitializeRow;

			ReloadData();
		}

        protected virtual void SortData()
        {
			if (Grid.ugData.DisplayLayout.Bands[0].Columns.Exists("Semantic"))
				Grid.ugData.DisplayLayout.Bands[0].Columns["Semantic"].SortIndicator = SortIndicator.Ascending;
        }

        protected virtual void Grid_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            
        }

		protected UltraGridEx Grid
		{
			get { return ((EntityNavigationListView) fViewCtrl).Grid; }
		}

		protected override void SetViewCtrl()
		{
			fViewCtrl = new EntityNavigationListView();

			ViewSettings viewSettings = new ViewSettings(fViewCtrl, Key);
            viewSettings.Settings.Add(new UltraGridExSettingsPartial(Key, ((EntityNavigationListView)fViewCtrl).Grid.ugData));
		}

		public override System.Drawing.Icon Icon
		{
			get { return System.Drawing.Icon.FromHandle(Properties.Resources.bridgeCls.GetHicon()); }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (fViewCtrl != null)
					fViewCtrl.Dispose();
			}
			base.Dispose(disposing);
		}

		protected virtual GridColumnsStates Grid_OnGetGridColumnsState(object sender)
		{
			GridColumnsStates states = new GridColumnsStates();

			GridColumnState state = new GridColumnState();
			state.ColumnName = "Semantic";
			state.ColumnCaption = "Категория";
			state.ColumnWidth = 150;
			states.Add(state.ColumnName, state);

			state = new GridColumnState();
			state.ColumnName = "Caption";
			state.ColumnCaption = "Наименование";
			state.ColumnWidth = 200;
			states.Add(state.ColumnName, state);

			state = new GridColumnState();
			state.ColumnName = "Description";
			state.ColumnCaption = "Описание";
			state.ColumnWidth = 700;
			states.Add(state.ColumnName, state);

			state = new GridColumnState();
			state.ColumnName = "ClassType";
			state.ColumnCaption = "";
			state.ColumnWidth = 100;
			state.IsHiden = true;
			states.Add(state.ColumnName, state);

			state = new GridColumnState();
			state.ColumnName = "FullName";
			state.ColumnCaption = "";
			state.ColumnWidth = 100;
			state.IsHiden = true;
			states.Add(state.ColumnName, state);

			state = new GridColumnState();
			state.ColumnName = "ObjectKey";
			state.ColumnCaption = "Уникальный ключ объекта";
			state.ColumnWidth = 200;
			state.IsHiden = true;
			states.Add(state.ColumnName, state);

			return states;
		}

		private static void grid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			//throw new NotImplementedException();
		}

		protected virtual DataTable GetNavigationDataTable()
		{
			return WorkplaceSingleton.Workplace.ActiveScheme.Classifiers.GetDataTable();
		}

		protected virtual string GetDataFilter()
		{
			return "1 = 1";
		}

		/// <summary>
		/// Заполнение грида навигации
		/// </summary>
		public override void ReloadData()
		{
			WorkplaceSingleton.Workplace.OperationObj.Text = "Построение списка объектов";
			WorkplaceSingleton.Workplace.OperationObj.StartOperation();
			try
			{
				DataTable filteredObjectsTable = null;
				DataTableHelper.CopyDataTable(
					GetNavigationDataTable(),
					ref filteredObjectsTable, String.Empty,
					GetDataFilter());
				Grid.DataSource = filteredObjectsTable;

				SortData();

				((EntityNavigationListView)fViewCtrl).AddGoToButton();

				fViewCtrl.LoadPersistence();
				((EntityNavigationListView)fViewCtrl).Grid.SetVisibleColumnsButtons();
			}
			finally
			{
				WorkplaceSingleton.Workplace.OperationObj.StopOperation();
			}
		}

		private bool grid_OnRefreshData(object sender)
		{
			fViewCtrl.SavePersistence();
			
			ReloadData();
		
			return true;
		}

		protected abstract BaseViewObj CreateBaseClsUI(string key);

		private void grid_OnClickCellButton(object sender, CellEventArgs e)
		{
			UltraGridRow activeRow = e.Cell.Row;
			if (activeRow == null)
			{
				return;
			}

			if (activeRow.Cells != null)
			{
				string key = GetKeyFromRow(activeRow); 
				ActiveteViewObjectUI(key);
			}
		}

		protected virtual string GetKeyFromRow(UltraGridRow activeRow)
		{
			return Convert.ToString(activeRow.Cells["ObjectKey"].Value);
		}

		internal void ActiveteViewObjectUI(string key)
		{
			IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(key);
			if (vc != null)
			{
				vc.WorkplaceWindow.SelectWindow();
			}
			else
			{
				BaseViewObj viewObject = CreateBaseClsUI(key);
				viewObject.Workplace = WorkplaceSingleton.Workplace;
				viewObject.Initialize();

				WorkplaceSingleton.Workplace.ShowView(viewObject);

				viewObject.InitializeData();
			}
		}
	}
}