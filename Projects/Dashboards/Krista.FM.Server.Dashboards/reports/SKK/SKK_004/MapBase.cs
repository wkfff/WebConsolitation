using System;
using System.Drawing;
using System.IO;
using System.Web;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_004
{
	/// <summary>
	/// Базовый класс-помощник для карты
	/// </summary>
	public abstract class MapBase
	{
		public MapControl Map { set; get; }

		protected Default report;
		protected string folderName;

		protected MapBase(MapControl map, Default report)
		{
			Map = map;
			this.report = report;
		}

		public abstract void SetStyle();
		protected abstract void ProcessingShape(Shape shape);
		public abstract void FillMapData();

		public virtual void AddMapLayer(string layerFileName)
		{
			layerFileName = layerFileName.Replace(".", "_");
			string layerName = HttpContext.Current.Server.MapPath(String.Format("{0}/{1}.shp", folderName, layerFileName));

			if (!File.Exists(layerName))
			{
				return;
			}

			int oldShapesCount = Map.Shapes.Count;

			Map.LoadFromShapeFile(layerName, "NAME", true);
			Map.Layers.Add(layerFileName);

			int i = oldShapesCount;
			while (i < Map.Shapes.Count)
			{
				Shape shape = Map.Shapes[i];
				shape.Layer = layerFileName;
				shape["TITLE"] = shape.Name.ToLower().Contains("shape") ? String.Empty : shape.Name.Replace("\"", "''");
				shape.Name = Guid.NewGuid().ToString();
				shape.Text = "";
				shape.ToolTip = "#TITLE";

				ProcessingShape(shape);
				i++;
			}
		}

		public Shape FindMapShape(string title)
		{
			foreach (Shape shape in Map.Shapes)
			{
				if (shape["TITLE"].ToString().ToLower().Equals(title.ToLower()))
				{
					return shape;
				}
			}
			return null;
		}

		public void SelectShape(string shapeTitle)
		{
			SelectShape(FindMapShape(shapeTitle));
		}

		public void SelectShape(Shape shape)
		{
			if (shape != null)
			{
				Shape prev;
				if ((prev = GetSelectedShape()) != null)
				{
					Map.Shapes.Remove(prev);
				}

				Shape clone = (Shape)shape.Clone();
				clone.Name = Guid.NewGuid().ToString();

				clone.TextVisibility = TextVisibility.Shown;
				clone.Text = SKKHelper.IsMozilla() ? clone.ToolTip.Replace(", ", "\n") : clone.ToolTip;
				clone.TextColor = Color.Black;
				clone.Font = new Font("Arial", 10, FontStyle.Regular);
				
				clone.Color = Color.FromArgb(0xff, Color.OrangeRed);
				clone.Selected = true;
				
				Map.Shapes.Add(clone);
			}
		}

		public Shape GetSelectedShape()
		{
			int i = Map.Shapes.Count - 1;
			while (i >= 0)
			{
				if (Map.Shapes[i].Selected)
				{
					return Map.Shapes[i];
				}
				i--;
			}
			return null;
		}
	}
}