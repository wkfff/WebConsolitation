using System;
using System.Drawing;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	public class TranslationsNavigationListUI : EntityNavigationListUI
	{
        public TranslationsNavigationListUI()
            : base(typeof(TranslationsNavigationListUI).FullName)
        {
            Caption = "Таблицы перекодировки";
        }

		public override Icon Icon
		{
			get { return Icon.FromHandle(Properties.Resources.cls_TranslationTable_16.GetHicon()); }
		}

		protected override BaseViewObj CreateBaseClsUI(string key)
		{
			return new TranslationsTables.TranslationsTablesUI(key);
		}

		protected override string GetKeyFromRow(UltraGridRow activeRow)
		{
			return Convert.ToString(activeRow.Cells["Name"].Value) + "." + Convert.ToString(activeRow.Cells["ObjectKey"].Value);
		}

		protected override System.Data.DataTable GetNavigationDataTable()
		{
			return Workplace.ActiveScheme.ConversionTables.GetDataTable();
		}

        protected override void SortData()
        {
            Grid._ugData.DisplayLayout.Bands[0].Columns[1].SortIndicator = SortIndicator.Ascending;
        }

		protected override Components.GridColumnsStates Grid_OnGetGridColumnsState(object sender)
		{
			Components.GridColumnsStates states = new Components.GridColumnsStates();

			Components.GridColumnState state = new Components.GridColumnState();
			state.ColumnName = "DataSemantic";
			state.ColumnCaption = "Категория сопоставляемого";
			state.ColumnWidth = 200;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "DataCaption";
			state.ColumnCaption = "Сопоставляемый классификатор";
			state.ColumnWidth = 200;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "BridgeSemantic";
			state.ColumnCaption = "Категория сопоставимого";
			state.ColumnWidth = 200;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "BridgeCaption";
			state.ColumnCaption = "Сопоставимый классификатор";
			state.ColumnWidth = 200;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "RuleName";
			state.ColumnCaption = "Правило перекодировки";
			state.ColumnWidth = 200;
			state.ColumnPosition = 6;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "ObjectKey";
			state.ColumnCaption = "Ключ объекта";
			state.IsHiden = true;
			state.ColumnWidth = 100;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "Name";
			state.ColumnCaption = "";
			state.IsHiden = true;
			state.ColumnWidth = 100;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "ConvTableKey";
			state.ColumnCaption = "";
			state.IsHiden = true;
			states.Add(state.ColumnName, state);

			return states;
		}

        protected override void Grid_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("GoToObject"))
                e.Row.Cells["GoToObject"].ToolTipText = "Перейти на таблицу перекодировки";
        }
	}
}
