using System;
using System.Data;
using System.Drawing;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_004
{
	public class MapSubject : MapBase
	{
		private int bgMarkColor;


		public MapSubject(MapControl map, Default report, int bgMarkColor)
			: base(map, report)
		{
			folderName = "../../../maps/СКК/Субъекты/";
			this.bgMarkColor = bgMarkColor;
		}

		public override void SetStyle()
		{
			Map.Width = CRHelper.GetChartWidth(SKKHelper.default1ItemsWidth);
			Map.Height = CRHelper.GetChartHeight(SKKHelper.defaultGridHeight);

			Map.Meridians.Visible = false;
			Map.Parallels.Visible = false;
			Map.ZoomPanel.Visible = true;
			Map.NavigationPanel.Visible = true;
			Map.Viewport.EnablePanning = true;
			Map.SelectionBorderColor = Color.Transparent;
			Map.SelectionMarkerColor = Color.Transparent;
			Map.RenderType = RenderType.ImageTag;
			Map.RenderType = RenderType.InteractiveImage;

			Map.Legends.Clear();

			Map.ShapeFields.Clear();
			Map.ShapeFields.Add("TITLE");
			Map.ShapeFields["TITLE"].Type = typeof(string);
			Map.ShapeFields["TITLE"].UniqueIdentifier = false;
			Map.ShapeFields.Add("MARK");
			Map.ShapeFields["MARK"].Type = typeof(double);
			Map.ShapeFields["MARK"].UniqueIdentifier = false;

			Map.Shapes.Clear();
			Map.Layers.Clear();
			AddMapLayer(report.selectedSubject.Value);
		}

		protected override void ProcessingShape(Shape shape)
		{
			int shapeID = Int32.Parse(shape["ID"].ToString());
			if(shapeID < 1000)
			{
				shape.Category = "Subjects";
				shape.Color = Color.FromArgb(bgMarkColor);
			}
			else if (shapeID > 1000)
			{
				shape.Category = "Points";
				shape.Color = Color.OrangeRed;
				shape.BorderColor = Color.Black;
			}
		}

		public override void FillMapData()
		{
			DataTable mapDt = new Query("skk_004_map_subject").GetDataTable();

			foreach (DataRow row in mapDt.Rows)
			{
				if (row[0] != DBNull.Value && row[0].ToString() != String.Empty &&
				    row[1] != DBNull.Value && row[1].ToString() != String.Empty)
				{
					string shapeTitle = row[0].ToString().Replace("\"", "''");
					double value = Double.Parse(row[1].ToString());

					Shape shape = FindMapShape(shapeTitle);
					if (shape != null)
					{
						shape["MARK"] = value;
						shape.ToolTip = SKKHelper.IsMozilla() 
								?
							String.Format( "#TITLE{0}{1}: #MARK{{{2}}}", ", ", CRHelper.ToLowerFirstSymbol(report.mark2unit[report.comboMarkIndex]), report.markFormat)
								:
							String.Format( "#TITLE{0}{1}: #MARK{{{2}}}", "\n", report.mark2unit[report.comboMarkIndex], report.markFormat);
					}

				}
			}
		}

	}
}