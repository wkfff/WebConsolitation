using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
	public abstract class GridHelpBase
	{
		protected Page page;
		public GridCommon Grid { protected set; get; }
		public bool ParamOneGrid { set; get; }

		protected GridHelpBase()
		{
			ParamOneGrid = false;
		}

		public abstract void Init(Page mainControl, string queryName);

		public virtual HtmlGenericControl GetItem()
		{
			var item = new HtmlGenericControl("div");
			item.Controls.Add(Grid.Grid);
			return item;
		}
	}

	public abstract class GridCommon
	{
		public UltraGridBrick Grid { set; get; }
		public UltraGridBand Band { set; get; }
		protected QueryWorker Query { set; get; }

		public string LabelCommon { set; get; }
		public string LabelViewed { set; get; }
		public string LabelDeny { set; get; }
		public int HeaderHeight { set; get; }
		public int DeleteColumns { set; get; }

		public abstract void SetDataStyle();
		public abstract void SetDataRules();
		public abstract void SetDataHeader();


		protected GridCommon(UltraGridBrick grid)
		{
			Grid = grid;
			Grid.Grid.InitializeLayout += InitializeLayout;

			LabelCommon = String.Empty;
			LabelViewed = String.Empty;
			LabelDeny = String.Empty;
			HeaderHeight = 0;
			DeleteColumns = 0;
		}
		
		public virtual void SetStyle()
		{
			Grid.EnableViewState = false;
		}

		public void SetFullAutoSizes()
		{
			Grid.AutoSizeStyle = GridAutoSizeStyle.Auto;
		}

		public void SetRowLimitHeight()
		{
			SetFullAutoSizes();
			SetHeight(SportHelper.defaultGridHeight);
			Grid.AutoHeightRowLimit = SportHelper.defRowsCount;
		}
		
		public void SetMaxWidth(bool safe)
		{
			int width = CRHelper.GetScreenWidth;
			width = width < SportHelper.minPageWidth ? SportHelper.minPageWidth : width;
			width = width > CustomReportConst.minScreenWidth ? CustomReportConst.minScreenWidth : width;
			
			width = width - (safe ? SportHelper.scrollWidth : 0);
			Grid.Width = Convert.ToInt32(width);
		}
		
		public void SetHeight(double height)
		{
			// минус высота шапки
			height = height - HeaderHeight;

			string browser = HttpContext.Current.Request.Browser.Browser;
			if (browser.Equals("Firefox"))
			{
				height = height * 1.15;
			}
			Grid.Height = Convert.ToInt32(height);
		}

		public virtual void SetData(string queryName)
		{
			Query = new QueryWorker(queryName);
			SetData();
		}

		public virtual void SetData()
		{
			if (Query == null)
			{
				throw new NullReferenceException("Query is null");
			}

			DataTable dtGrid = Query.GetDataTable();
			if (dtGrid.Rows.Count > 0)
			{
				//dtGrid.DeleteColumns(DeleteColumns);
				Grid.DataTable = dtGrid;
			}
		}

        public virtual void SetDataQuery(DataTable table)
        {
            if (table.Rows.Count > 0)
            {
                Grid.DataTable = table;   
            }
        }
		
		private void InitializeLayout(object sender, LayoutEventArgs e)
		{
			if (e.Layout.Bands[0].Columns.Count == 0)
			{
				return;
			}

			Band = e.Layout.Bands[0];
            SetDataStyle();
            SetDataHeader();
			SetDataRules();
		}

	}
}
