using System;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.iPadBricks.iPadBricks
{
    public class iPadBricksHelper
    {
        public static void SetConditionBall(RowEventArgs e, int index, bool directAssesment)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null &&
                e.Row.Cells[index - 1] != null &&
                e.Row.Cells[index - 1].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                double compareValue = Convert.ToDouble(e.Row.Cells[index - 1].Value.ToString());
                string positiveImg = directAssesment ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
                string negativeImg = directAssesment ? "~/images/ballRedBB.png" : "~/images/ballGreenBB.png";
                string img;
                if (value < compareValue)
                {
                    img = negativeImg;

                }
                else
                {
                    img = positiveImg;

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 20px center; padding-left: 2px";
            }
        }

        public static void SetConditionArrow(RowEventArgs e, int index)
        {
            SetConditionArrow(e, index, 1, true);
        }

        public static void SetConditionArrow(RowEventArgs e, int index, int borderValue, bool direct)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;
                if (direct)
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                    }
                    else if (value < borderValue)
                    {
                        img = "~/images/arrowRedDownBB.png";
                    }
                }
                else
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowRedUpBB.png";
                    }
                    else if (value < borderValue)
                    {
                        img = "~/images/arrowGreenDownBB.png";
                    }
                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
            }
        }

        public static void SetConditionCorner(RowEventArgs e, int index)
        {
            SetConditionCorner(e, index, 0);
        }

        public static void SetConditionCorner(RowEventArgs e, int index, int borderValue)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;
                if (value < borderValue)
                {
                    img = "~/images/cornerRed.gif";

                }
                else if (value > borderValue)
                {
                    img = "~/images/cornerGreen.gif";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-left: 2px";
            }
        }

        public static void SetConditionCorner(RowEventArgs e, int index, int borderValue, bool useGrey)
        {
            if (useGrey)
            {
                if (e.Row.Cells[index] != null &&
                    e.Row.Cells[index].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                    string img;
                    if (value < borderValue)
                    {
                        img = "~/images/cornerRed.gif";

                    }
                    else if (value > borderValue)
                    {
                        img = "~/images/cornerGreen.gif";

                    }
                    else
                    {
                        img = "~/images/CornerGray.gif";

                    }
                    e.Row.Cells[index].Style.BackgroundImage = img;
                    e.Row.Cells[index].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: right top; padding-left: 2px";
                }
            }
            else
            {
                SetConditionCorner(e, index, borderValue);
            }
        }

        public static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex, bool direct, string style)
        {
            SetRankImage(e, rankCellIndex, worseRankCelIndex, direct, style, rankCellIndex, "~/images/StarYellow.png", "~/images/StarGray.png");
        }

        public static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex, bool direct, string style, int indicatorCellIndex, string bestRankIcon, string worseRankIcon)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null &&
                e.Row.Cells[worseRankCelIndex] != null &&
                e.Row.Cells[worseRankCelIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = Convert.ToInt32(e.Row.Cells[worseRankCelIndex].Value.ToString());
                string img = String.Empty;
                if (direct)
                {
                    if (value == 1)
                    {
                        img = bestRankIcon;
                    }
                    else if (value == worseRankValue)
                    {
                        img = worseRankIcon;
                    }
                }
                else
                {
                    if (value == 1)
                    {
                        img = worseRankIcon;
                    }
                    else if (value == worseRankValue)
                    {
                        img = bestRankIcon;
                    }
                    e.Row.Cells[rankCellIndex].Value = worseRankValue - value + 1;
                }
                e.Row.Cells[indicatorCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[indicatorCellIndex].Style.CustomRules = style;
            }
        }

        public static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex, bool direct)
        {
            string style = "background-repeat: no-repeat; background-position: 40px center; padding-left: 2px; padding-right: 32px";
            SetRankImage(e, rankCellIndex, worseRankCelIndex, direct, style);
        }

        public static string GetWarpedHint(string hint)
        {
            string name = hint.Replace("\"", "'");
            if (name.Length > 30)
            {
                int k = 11;

                for (int j = 0; j < name.Length; j++)
                {
                    k++;
                    if (k > 30 && name[j] == ' ')
                    {
                        name = name.Insert(j, "<br/>");
                        k = 0;
                    }
                }
            }
            return name;
        }

		public static void SetMinMaxImage(UltraWebGrid grid, int column)
		{
			int valuesCount = 0;
			double[] values = new double[grid.Rows.Count];
			int[] rows = new int[grid.Rows.Count];
			foreach (UltraGridRow row in grid.Rows)
			{
                if (row.Cells[0].Value.ToString().StartsWith("&nbsp;&nbsp;&nbsp;"))
                    continue;
				double value;
				if (row.Cells[column].Value != null && Double.TryParse(row.Cells[column].Value.ToString(), out value))
				{
					values[valuesCount] = value;
					rows[valuesCount] = row.Index;
					++valuesCount;
				}
			}
			if (valuesCount < 2)
				return;
			Array.Resize(ref values, valuesCount);
			Array.Resize(ref rows, valuesCount);
			Array.Sort(values, rows);
			string img = "~/images/min.png";
			for (int i = 0; values[i] == values[0]; ++i)
			{
				grid.Rows[rows[i]].Cells[column].Style.BackgroundImage = img;
				grid.Rows[rows[i]].Cells[column].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
			}
			img = "~/images/max.png";
			for (int i = valuesCount - 1; values[i] == values[valuesCount - 1]; --i)
			{
				grid.Rows[rows[i]].Cells[column].Style.BackgroundImage = img;
				grid.Rows[rows[i]].Cells[column].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
			}
		}

	}
}
