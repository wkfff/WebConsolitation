using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;

namespace Krista.FM.Server.Dashboards.Components
{
    public enum GridAutoSizeStyle
    {
        None,
        Auto,
        AutoHeight,
        AutoWidth
    }

    public partial class UltraGridBrick : UserControl
    {
		#region Поля и свойства

		private Collection<IndicatorRule> indicatorRuleCollection;

		public Unit Width { set; get; }
		
		public Unit Height { set; get; }

        public DataTable DataTable { set; get; }
		
		public GridHeaderLayout GridHeaderLayout { private set; get; }
		
		public UltraWebGrid Grid
		{
			get { return GridControl; }
		}
		
        public bool BrowserSizeAdapting
        {
			get { return browserSizeAdapting && AutoSizeStyle == GridAutoSizeStyle.None; }
            set { browserSizeAdapting = value; }
        }
		private bool browserSizeAdapting;

		public GridAutoSizeStyle AutoSizeStyle { set; get; }

    	public int AutoHeightRowLimit { set; get; }
    	
        public string GridDefaultFontName
        {
			get { return GridControl.DisplayLayout.RowStyleDefault.Font.Name; }
        }

		public FontUnit GridDefaultFontSize
        {
			get { return GridControl.DisplayLayout.RowStyleDefault.Font.Size; }
        }

		public bool HeaderVisible { set; get; }

    	public bool AllowColumnSorting { set; get; }

    	public bool RedNegativeColoring { set; get; }
        
		#endregion

		#region Аналогичные типы шрифтов

		public Font BoldFont12pt
		{
			get { return new Font(GridDefaultFontName, 12, FontStyle.Bold); }
		}

    	public Font BoldFont11pt
    	{
    		get { return new Font(GridDefaultFontName, 11, FontStyle.Bold); }
    	}

    	public Font BoldFont10pt
    	{
    		get { return new Font(GridDefaultFontName, 10, FontStyle.Bold); }
    	}

    	public Font BoldFont8pt
    	{
    		get { return new Font(GridDefaultFontName, 8, FontStyle.Bold); }
    	}

    	public Font ItalicFont10pt
    	{
    		get { return new Font(GridDefaultFontName, 10, FontStyle.Italic); }
    	}

    	public Font ItalicFont8pt
    	{
    		get { return new Font(GridDefaultFontName, 8, FontStyle.Italic); }
    	}

    	public Font Font8pt
    	{
    		get { return new Font(GridDefaultFontName, 8, FontStyle.Bold); }
    	}

    	#endregion


		public UltraGridBrick()
		{
			DataTable = new DataTable();

			BrowserSizeAdapting = true;
			AutoSizeStyle = GridAutoSizeStyle.None;
			AutoHeightRowLimit = 40;

			HeaderVisible = true;
			AllowColumnSorting = false;
			RedNegativeColoring = true;
		}

        protected void Page_Load(object sender, EventArgs e)
        {
			if (!Visible) { return; }
			
            GridControl.Width = BrowserSizeAdapting ? CRHelper.GetGridWidth(Width.Value) : Width;
            GridControl.Height = BrowserSizeAdapting ? CRHelper.GetGridHeight(Height.Value) : Height;
			
            GridControl.DisplayLayout.NoDataMessage = "Нет данных";

            GridControl.DataBinding += new EventHandler(GridControl_DataBinding);
            GridControl.InitializeLayout += new InitializeLayoutEventHandler(GridControl_SetupLayout);
            GridControl.InitializeRow += new InitializeRowEventHandler(GridControl_RowStyling);

            if (RedNegativeColoring)
            {
                GridControl.InitializeRow += new InitializeRowEventHandler(GridControl_RedNegativeValues);
            }
            
            if (!HeaderVisible)
            {
                GridControl.DataBound += new EventHandler(GridControl_HeaderClearing);
            }

            if (AutoSizeStyle != GridAutoSizeStyle.None)
            {
                GridControl.DataBound += new EventHandler(GridControl_AutoSizing);
            }

            GridControl.Bands.Clear();
			GridHeaderLayout = new GridHeaderLayout(GridControl);
            GridControl.DataBind();
        }

        public void AddIndicatorRule(IndicatorRule rule)
        {
            if (indicatorRuleCollection == null)
            {
                indicatorRuleCollection = new Collection<IndicatorRule>();
            }

            indicatorRuleCollection.Add(rule);
        }

        #region Обработчики грида

        protected void GridControl_DataBinding(object sender, EventArgs e)
        {
			if (DataTable.Rows.Count > 0)
            {
				GridControl.DataSource = DataTable;
            }
        }

        protected void GridControl_AutoSizing(object sender, EventArgs e)
        {
            switch (AutoSizeStyle)
            {
                case GridAutoSizeStyle.AutoWidth:
                    {
                        GridControl.Width = Unit.Empty;
                        break;
                    }
                case GridAutoSizeStyle.AutoHeight:
                    {
                        if (Grid.Rows.Count < AutoHeightRowLimit)
                        {
                            GridControl.Height = Unit.Empty;
                        } 
                        break;
                    }
                case GridAutoSizeStyle.Auto:
                    {
                        GridControl.Width = Unit.Empty;
                        if (Grid.Rows.Count < AutoHeightRowLimit)
                        {
                            GridControl.Height = Unit.Empty;
                        } 
                        break;
                    }
            }
        }

        protected void GridControl_HeaderClearing(object sender, EventArgs e)
        {
            if (GridControl.Bands.Count > 0)
            {
                GridControl.Bands[0].HeaderLayout.Clear();
            }
        }

        protected void GridControl_SetupLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowColumnSorting ? AllowSorting.OnClient : AllowSorting.No;
        }

        protected void GridControl_RowStyling(object sender, RowEventArgs e)
        {
            if (indicatorRuleCollection == null)
            {
                return;
            }

            foreach (IndicatorRule indicatorRule in indicatorRuleCollection)
            {
                indicatorRule.SetRowStyle(e.Row);
            }
        }

        protected void GridControl_RedNegativeValues(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];

                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion
    }
}