using System;
using System.Drawing;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	public class AssociationNavigationListUI : EntityNavigationListUI
	{
        public AssociationNavigationListUI()
            : base(typeof(AssociationNavigationListUI).FullName)
        {
            Caption = "Сопоставление классификаторов";
        }

		public override Icon Icon
		{
			get { return Icon.FromHandle(Properties.Resources.cls_Associate_16.GetHicon()); }
		}

		protected override BaseViewObj CreateBaseClsUI(string key)
		{
			return new Association.AssociationUI(key);
		}

		protected override System.Data.DataTable GetNavigationDataTable()
		{
			return Workplace.ActiveScheme.Associations.GetDataTable();
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
			state.ColumnName = "AssociatedRecordsPercent";
			state.ColumnCaption = "% сопоставленных данных";
			state.ColumnWidth = 100;
			//state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.FormattedText;
			//state.Mask = "";
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "FullName";
			state.ColumnCaption = String.Empty;
			state.IsHiden = true;
			state.ColumnWidth = 125;
			states.Add(state.ColumnName, state);

			state = new Components.GridColumnState();
			state.ColumnName = "ObjectKey";
			state.ColumnCaption = "Ключ объекта";
			state.IsHiden = true;
			state.ColumnWidth = 125;
			states.Add(state.ColumnName, state);

			return states;
		}

        protected override void Grid_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("GoToObject"))
                e.Row.Cells["GoToObject"].ToolTipText = "Перейти на сопоставление классификаторов";
        }
	}
}
