using System.Data;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.FO_0002_0002_Settlement.Default.reports.FO_0002_0002_Settlement
{
	/// <summary>
	/// Базовая обертка над гридом
	/// </summary>
	public abstract class GridHelperBase
	{

		public UltraGridBrick Grid { protected set; get; }
		public DataTable Data { protected set; get; }
		public UltraGridBand Band { protected set; get; }

		protected int DeleteColumns { set; get; }
		protected int HiddenColumns { set; get; }

		
		protected GridHelperBase()
		{
			DeleteColumns = 0;
			HiddenColumns = 0;
		}


		public virtual void Init(UltraGridBrick grid, string queryName, DataProvider provider)
		{
			Grid = grid;
			Grid.Grid.InitializeLayout += InitializeLayout;
			Grid.Grid.InitializeRow += InitializeRow;
			
			SetStyle();
			SetData(queryName, provider);
		}

		protected abstract void InitializeRow(object sender, RowEventArgs e);

		protected virtual void SetStyle()
		{
			Grid.EnableViewState = false;
			Grid.RedNegativeColoring = false;
		}
		
		protected virtual void SetData(string queryName, DataProvider provider)
		{
			Data = DataProvider.GetDataTableForChart(queryName, provider);
			if (Data.Rows.Count > 0)
			{
				Data.DeleteColumns(DeleteColumns);
				Grid.DataTable = Data;
			}
		}
		
		protected virtual void InitializeLayout(object sender, LayoutEventArgs e)
		{
			if (e.Layout.Bands[0].Columns.Count == 0)
			{
				return;
			}

			Band = e.Layout.Bands[0];
			SetDataStyle();
			SetDataRules();
			SetDataHeader();
		}

		protected abstract void SetDataStyle();
		protected abstract void SetDataRules();
		protected abstract void SetDataHeader();



	}
}