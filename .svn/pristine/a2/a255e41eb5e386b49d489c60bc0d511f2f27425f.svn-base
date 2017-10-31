using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_004
{
	public class MapRF : MapBase
	{
		private Collection<Shape> toRemove;
		private Dictionary<string, object[]> dbobjects;
		private string fileName;

		private Collection<double> axisBase;
		private double countriesMax;
		private Collection<Color> countriesColors;
		private Collection<double> countriesAxis;
		private double subjectsMax;
		private Collection<Color> subjectsColors;
		private Collection<double> subjectsAxis;
		
	
		public MapRF(MapControl map, Default report) 
			: base(map, report)
		{
			toRemove = new Collection<Shape>();
			dbobjects = new Dictionary<string, object[]>();
			folderName = "../../../maps/СКК/РФ/";
			fileName = "РФ";
			map.Click += map_Click;
			
			countriesMax = 0;
			subjectsMax = 0;

			axisBase = new Collection<double>
			{
				0.0, 0.005, 0.05, 0.25, 0.5, 1.0
			};

			countriesAxis = new Collection<double>();
			countriesColors = new Collection<Color>
			{
				Color.FromArgb(255, Color.FromArgb(0xF9F4DF)),
				Color.FromArgb(255, Color.FromArgb(0xE8CEBB)),
				Color.FromArgb(255, Color.FromArgb(0xD5AA88)),
				Color.FromArgb(255, Color.FromArgb(0xC38E63)),
				Color.FromArgb(255, Color.FromArgb(0xAC703D)),
				Color.FromArgb(255, Color.FromArgb(0x793F0D))                		
			};

			subjectsAxis = new Collection<double>();
			subjectsColors = new Collection<Color>
			{
				Color.FromArgb(255, Color.FromArgb(0xEAF3DE)),
			    Color.FromArgb(255, Color.FromArgb(0xA5DB92)),
			    Color.FromArgb(255, Color.FromArgb(0x54BE46)),
			    Color.FromArgb(255, Color.FromArgb(0x12AD2A)),
			    Color.FromArgb(255, Color.FromArgb(0x289728)),
			    Color.FromArgb(255, Color.FromArgb(0x317023))
			};
			
		}

		public override void SetStyle()
		{
			Map.Width = CRHelper.GetChartWidth(SKKHelper.defaultMapWidth);
			Map.Height = CRHelper.GetChartHeight(SKKHelper.defaultMapHeight);

			Map.Meridians.Visible = false;
			Map.Parallels.Visible = false;
			Map.ZoomPanel.Visible = false;
			Map.NavigationPanel.Visible = false;
			Map.Viewport.EnablePanning = true;
			Map.SelectionBorderColor = Color.Transparent;
			Map.SelectionMarkerColor = Color.Transparent;
			Map.RenderType = RenderType.ImageTag;

			Map.Legends.Clear();

			// добавляем легенду

			Map.Legends.Add(AddLegend("LegendBorders", "по участкам границы", PanelDockStyle.Right, DockAlignment.Near));
			Map.Legends.Add(AddLegend("LegendSubjects", "по субъектам", PanelDockStyle.Right, DockAlignment.Far));

			Map.ShapeFields.Clear();
			Map.ShapeFields.Add("TITLE");
			Map.ShapeFields["TITLE"].Type = typeof(string);
			Map.ShapeFields["TITLE"].UniqueIdentifier = false;
			Map.ShapeFields.Add("MARK");
			Map.ShapeFields["MARK"].Type = typeof(double);
			Map.ShapeFields["MARK"].UniqueIdentifier = false;
			Map.ShapeFields.Add("SHAPE_COLOR");
			Map.ShapeFields["SHAPE_COLOR"].Type = typeof(int);
			Map.ShapeFields["SHAPE_COLOR"].UniqueIdentifier = false;

			Map.Shapes.Clear();
			Map.Layers.Clear();
			AddMapLayer(fileName);
		}

		private Legend AddLegend(string name, string title, PanelDockStyle dock, DockAlignment dockAlignment)
		{
			Legend legend = 
				new Legend(name)
					{
						Visible = true,
						Dock = dock,
						DockAlignment = dockAlignment,
						BackColor = Color.White,
						BackSecondaryColor = Color.Gainsboro,
						BackGradientType = GradientType.DiagonalLeft,
						BackHatchStyle = MapHatchStyle.None,
						BorderColor = Color.Gray,
						BorderWidth = 1,
						BorderStyle = MapDashStyle.Solid,
						BackShadowOffset = 4,
						TextColor = Color.Black,
						Font = new Font("MS Sans Serif", 7, FontStyle.Regular),
						AutoFitText = true,
						Title = String.Format("{0},\n{1}", report.mark2unit[report.comboMarkIndex], title),
						AutoFitMinFontSize = 7
					};
			return legend;
		}

		public override void AddMapLayer(string layerFileName)
		{
			base.AddMapLayer(layerFileName);

			foreach (Shape shape in toRemove)
			{
				Map.Shapes.Remove(shape);
			}
			toRemove.Clear();
		}

		protected override void ProcessingShape(Shape shape)
		{
			int shapeID = Int32.Parse(shape["ID"].ToString());
			if (shapeID == -1)
			{
				// прочее
				shape.Category = ShapeType.other.ToString();
				shape.Color = Color.FromArgb(0xff, Color.FromArgb(0xE2EAEB));
				
			}
			if (shapeID == 0)
			{
				// вода
				shape.Category = ShapeType.water.ToString();
				shape.Color = Color.FromArgb(0xff, Color.FromArgb(0xD5EEF4));
			}
			if (shapeID == 1)
			{
				// соседи
				shape.Category = ShapeType.country.ToString();
			}
			if (shapeID > 1 && shapeID < 100)
			{
				// субъекты
				shape.Category = ShapeType.subject.ToString();
				shape.Color = Color.White;
				string shapeTitle = shape["TITLE"].ToString();
				shapeTitle = shapeTitle.Replace("обл.", "область");
				shapeTitle = shapeTitle.Replace("Р.", "Республика");
				shapeTitle = shapeTitle.Replace("АО", "автономный округ");
				shape["TITLE"] = shapeTitle;
			}
			if (shapeID >= 100 && shapeID < 1000)
			{
				// города-субъекты
				shape.Category = ShapeType.subject.ToString();
				shape.Color = Color.White;
				shape.BorderColor = Color.DarkSlateGray;
				string shapeTitle = shape["TITLE"].ToString();
				shape["TITLE"] = shapeTitle;
			}
			if (shapeID > 1000)
			{
				// пункты пропуска
				toRemove.Add(shape);
			}
		}

		public override void FillMapData()
		{
			// обработка данных из запросов

			DataTable mapDt;
			double zeros;

			mapDt = new Query("skk_004_map_rf_borders").GetDataTable();
			countriesMax = 0;
			if ((mapDt.Rows.Count > 0) && ((countriesMax = Double.Parse(mapDt.Rows[0][2].ToString())) > 0))
			{
				zeros = Math.Pow(10, Math.Floor(Math.Log10(countriesMax)));
				countriesMax = Math.Ceiling(countriesMax / zeros) * zeros;
				
				foreach (DataRow row in mapDt.Rows)
				{
					if (row[0] != DBNull.Value && row[0].ToString() != String.Empty &&
						row[1] != DBNull.Value && row[1].ToString() != String.Empty &&
						report.border2country.ContainsKey(row[0].ToString()))
					{
						string shapeTitle = report.border2country[row[0].ToString()];
						double value = Double.Parse(row[1].ToString());
						dbobjects.Add(shapeTitle, new object[] { ShapeType.country, value });
					}
				}
			}

			mapDt = new Query("skk_004_map_rf_subjects").GetDataTable();
			subjectsMax = 0;
			if ((mapDt.Rows.Count > 0) && ((subjectsMax = Double.Parse(mapDt.Rows[0][2].ToString())) > 0))
			{
				zeros = Math.Pow(10, Math.Floor(Math.Log10(subjectsMax)));
				subjectsMax = Math.Ceiling(subjectsMax / zeros) * zeros;

				foreach (DataRow row in mapDt.Rows)
				{
					if (row[0] != DBNull.Value && row[0].ToString() != String.Empty &&
						row[1] != DBNull.Value && row[1].ToString() != String.Empty)
					{
						string shapeTitle = row[0].ToString();
						double value = Double.Parse(row[1].ToString());
						dbobjects.Add(shapeTitle, new object[] { ShapeType.subject, value });
						report.subjects.Add(shapeTitle);
					}
				}
			}
			
			// настройка шкал

			SetAxis(countriesAxis, countriesMax);
			SetAxis(subjectsAxis, subjectsMax);

			// добавление легенды
			SetLegend(Map.Legends["LegendBorders"], countriesColors, countriesAxis);
			Map.Legends["LegendSubjects"].Items.Add(new LegendItem { Color = Color.White, Text = "нет ПП" });
			SetLegend(Map.Legends["LegendSubjects"], subjectsColors, subjectsAxis);

			// обработка форм

			foreach (Shape shape in Map.Shapes)
			{
				string shapeTitle = shape["TITLE"].ToString();
				if (!dbobjects.ContainsKey(shapeTitle))
					continue;
				
				double value = (double) dbobjects[shapeTitle][1];
				shape["MARK"] = value;
				shape.ToolTip = SKKHelper.IsMozilla() 
						?
					String.Format("#TITLE{0}{1}: #MARK{{{2}}}", ", ", CRHelper.ToLowerFirstSymbol(report.mark2unit[report.comboMarkIndex]), report.markFormat)
						:
					String.Format("#TITLE{0}{1}: #MARK{{{2}}}", "\n", report.mark2unit[report.comboMarkIndex], report.markFormat);
				
				switch ((ShapeType)dbobjects[shapeTitle][0])
				{
					case ShapeType.subject:
					{
						int index = GetIndexColor(subjectsAxis, value);
						shape.Color = subjectsColors[index];
						shape["SHAPE_COLOR"] = subjectsColors[index].ToArgb();
						break;
					}
					case ShapeType.country:
					{
						int index = GetIndexColor(countriesAxis, value);
						shape.Color = countriesColors[index];
						shape["SHAPE_COLOR"] = countriesColors[index].ToArgb();
						break;
					}
				}
			}
		}

		protected void map_Click(object sender, ClickEventArgs e)
		{
			Shape shape;
			e.MapControl.CallbackManager.DisableClientUpdate = true;
			
			// обновить ранее выбранный (если вдруг кликнуто не по субъекту/форме)
			if ((shape = GetSelectedShape()) != null)
			{
				report.selectedSubject.Value = shape["TITLE"].ToString();
			}

			// найти форму, по которой кликнуто
			HitTestResult result = e.MapControl.HitTest(e.X, e.Y);
			if (result == null || result.Object == null || !(result.Object is Shape))
				return;

			shape = (Shape)result.Object;

			// из форм обрабатываем только субъекты
			if (!report.subjects.Contains(shape["TITLE"].ToString()))
				return;

			if (!shape.Selected)
			{
				SelectShape(shape);
				report.selectedSubject.Value = shape["TITLE"].ToString();

				e.MapControl.CallbackManager.ExecuteClientScript("DoSubmit(document.getElementById('RefreshButton'));");
			}

			HttpContext.Current.Session["Process"] = false;
		}

		private int GetFormatDecimals()
		{
			if (report.markFormat.ToUpper().Contains("N"))
			{
				return
					Convert.ToInt32(report.markFormat.Substring(1));
			}
			return -1;
		}

		private void SetAxis(Collection<double> axis, double max)
		{
			axis.Add(0);
			for (int i = 1; i < axisBase.Count; i++)
			{
				// максимум предыдущего уровня
				double value = axisBase[i] * max; 
				
				// минимум текущего чуть-чуть больше максимума предыдущего
				if (GetFormatDecimals() == 0) value = Math.Floor(value) + 1;
				else if (GetFormatDecimals() == 3) value = value + 0.001;

				axis.Add(value);
			}
		}

		public int GetShapeColor(string title)
		{
			Shape shape = FindMapShape(title);
			return (int)shape["SHAPE_COLOR"];
		}

		private void SetLegend(Legend legend, Collection<Color> colors, Collection<double> values)
		{
			legend.Items.Add(new LegendItem { Color = colors[0], Text = "0" });
			for (int i = 1; i < axisBase.Count; i++)
			{
				double minValue = Convert.ToDouble(values[i - 1].ToString(report.markFormat));
				double maxValue = Convert.ToDouble(values[i].ToString(report.markFormat));

				if (i == 1)
				{
					if (GetFormatDecimals() == 0) minValue = minValue + 1;
					else if (GetFormatDecimals() == 3) minValue = minValue + 0.001;
				}

				if (GetFormatDecimals() == 0) maxValue = maxValue - 1;
				else if (GetFormatDecimals() == 3) maxValue = maxValue - 0.001;

				if (minValue > maxValue)
					continue;

				string text = (minValue == maxValue)
					? String.Format("{0:" + report.markFormat + "}", minValue)
					: String.Format("{0:" + report.markFormat + "} - {1:" + report.markFormat + "}", minValue, maxValue);
				
				legend.Items.Add(
					new LegendItem
						{
							Color = colors[i],
							Text = text
						});
			}
		}
		
		private static int GetIndexColor(Collection<double> axis, double value)
		{
			if (value == axis[0])
				return 0;

			int index;
			for (index = 1; index < axis.Count; index++)
			{
				if (value < axis[index])
					break;
			}

			if (index >= axis.Count) 
				index = axis.Count - 1;

			while ((index < axis.Count-1) && (axis[index] == axis[index + 1]))
				index++;
			
			return index;
		}
		
	}

	public enum ShapeType
	{
		water, other, country, subject
	}
}