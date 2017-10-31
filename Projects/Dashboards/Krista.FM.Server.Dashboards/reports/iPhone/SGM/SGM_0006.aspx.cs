using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0006 : CustomReportPage
    {
        protected DataTable dtFullData = new DataTable();
        protected DataTable dtRF = new DataTable();
        protected DataTable dtChart = new DataTable();

        protected string months;
        protected int curYear;
        protected string year1; 
        protected string year2;
        protected string mapName = string.Empty;
        protected string foName, rfName;
        protected string groupName;
        protected string shortSubjectName;

        // Число строк в графике на оригинальном виде
        protected int chartRowCount = 8;
        protected int chartFontSize = 11;

        protected SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMRegionNamer regionNamer = new SGMRegionNamer();
        protected readonly SGMSupport supportClass = new SGMSupport();

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;
            dataObject.InitObject();
            dataRotator.FillDeseasesList(null, 0);
            regionNamer.FillFMtoSGMRegions();

            if (!Page.IsPostBack)
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, true);
            }

            curYear = dataRotator.GetLastYear();
            year1 = Convert.ToString(curYear - 0);
            year2 = Convert.ToString(curYear - 1);
            groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            months = dataRotator.GetMonthParamIphone();
            mapName = regionNamer.GetSGMName(RegionsNamingHelper.FullName(UserParams.ShortStateArea.Value));
            shortSubjectName = RegionsNamingHelper.ShortName(UserParams.ShortStateArea.Value);
            foName = dataObject.GetFOName(dataObject.GetMapCode(mapName), false);
            rfName = dataRotator.mapList[0];

            // Читаем данные из БД
            FillData();
            // Создаем структуру для графика
            FillChartData();
            // Заполняем данными прочие компоненты
            FillComponentData();
        }

        protected virtual void FillData()
        {
            // Структура данных по болезням
            // |dies|abs2009|rel2009|abs2008|rel2008|absFO2009|relFO2009|absRF2009|relRF2009|rel2009/rel2008|Текст роста \ снижения

            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.mainColumnRange = dataRotator.GetDeseaseCodes(0);
            // Заболевание по субъекту текущий год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, months, mapName, groupName, string.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "1");
            // Заболевание по субъекту прошлый год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2, months, mapName, groupName, string.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "3");
            // ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, months, foName, groupName, string.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "5");
            // РФ
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, months, rfName, groupName, string.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "7");
            // Рост
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                "2", "8");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                "1", "3", "2", "4");

            dtFullData = dataObject.FillData(9);

            // Структура данных по территориям
            // |mapName|abs2009|rel2009|rank2009|

            dataObject.InitObject();
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            // абс
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year1, months, string.Empty, groupName, dataRotator.GetDeseaseCodes(0));
            // отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "1");
            // ранг
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "2");
            // абс
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year2, months, string.Empty, groupName, dataRotator.GetDeseaseCodes(0));
            // отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "4");
            // ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");

            dtRF = dataObject.FillData();
        }

        protected virtual void FillComponentData()
        {
            double val1 = 0;
            double val2 = 1;
            int rankRF = 0;
            int maxRankRF = 0;

            // Ищем данные по субъекту
            var drFind = supportClass.FindDataRow(dtRF, mapName, dtRF.Columns[0].ColumnName);
            if (supportClass.CheckValue(drFind))
            {
                val1 = Convert.ToDouble(drFind[2]);
                val2 = Convert.ToDouble(drFind[5]);
                rankRF = Convert.ToInt32(drFind[3]);
                maxRankRF = dtRF.Rows.Count - dataRotator.mapList.Count;
            }
            
            var percent = 100 * val1 / val2 - 100;

            var drsFO = dtRF.Select(
                string.Format("{0} = '{1}'", dtRF.Columns[6].ColumnName, supportClass.GetFOShortName(foName)),
                string.Format("{0} desc", dtRF.Columns[2].ColumnName));
            
            int maxRankFO = drsFO.Length;
            int rankFO = 0;
            for (int i = 0; i < drsFO.Length; i++)
            {
                rankFO++;
                if (drsFO[i][0].ToString() == mapName) break;
            }

            ImageDifference.ImageUrl = percent < 0 ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedUpBB.png";
            LabelPercent.Text = String.Format("{0:N2}%", percent);

            LabelSubject.Text = String.Format("Общая инф. заболеваемость в {0}", shortSubjectName);
            LabelCurValCaption.Text = String.Format("{1} {0}", year1, supportClass.GetMonthLabelShort(months));
            LabelMeasure.Text = "на 100 тыс.";
            LabelCurVal.Text = String.Format("{0:N2}", val1);

            LabelPrevValCaption.Text = String.Format("{1} {0}", year2, supportClass.GetMonthLabelShort(months));
            LabelPrevVal.Text = String.Format("{0:N2}", val2);

            LabelRankFOCaption.Text = String.Format("ранг в {0}", supportClass.GetFOShortName(foName));
            LabelRankRFCaption.Text = String.Format("ранг в {0}", supportClass.GetFOShortName(rfName));

            SetRankImage(ImageRankFO, rankFO, maxRankFO);
            SetRankImage(ImageRankRF, rankRF, maxRankRF);

            LabelRankRF.Text = String.Format("{0}", rankRF);
            LabelRankFO.Text = String.Format("{0}", rankFO);

            DataRow[] drsActual = dtFullData.Select(String.Format("{0} > 1", dtFullData.Columns[9].ColumnName));
            int actualCount = drsActual.Length;

            LabelActualCaption.Text = String.Format("Актуальные инфекции в {0},", shortSubjectName);
            LabelActualCaption.ForeColor = Color.FromArgb(0xE64140);
            LabelActualCaptionDetail.Text = String.Format("превышающие уровень РФ: {0}", actualCount);
            LabelActualCaptionDetail.ForeColor = Color.FromArgb(0xE64140);

            LabelActualCaption.Font.Bold = true;
            LabelActualCaption.Font.Size = FontUnit.Parse("17px");
            LabelActualCaptionDetail.Font.Bold = true;
            LabelActualCaptionDetail.Font.Size = FontUnit.Parse("17px");
        }

        protected virtual void SetRankImage(System.Web.UI.WebControls.Image image, int rank, int maxRank)
        {
            if (rank > 1 && rank != maxRank)
            {
                image.Visible = false;
            }
            else
            {
                image.ImageUrl = rank == 1 ? "~/images/starGray.png" : "~/images/starYellow.png";
            }
        }


        protected virtual void AddChartText(Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e,
            Box box, int padLeft, int width, string value, StringAlignment align)
        {
            int fontSize = chartFontSize;
            if (value.Length > 18) fontSize--;
            var text = new Text
                           {
                               PE = {Fill = Color.FromArgb(0xd1d1d1)},
                               labelStyle = {Font = new Font("Arial", fontSize, FontStyle.Bold)},
                               bounds = new Rectangle(box.rect.X + padLeft, box.rect.Y, width, box.rect.Height)
                           };

            text.SetTextString(value);
            text.labelStyle.Orientation = TextOrientation.Horizontal;
            text.labelStyle.VerticalAlign = StringAlignment.Center;
            text.labelStyle.HorizontalAlign = align;
            e.SceneGraph.Add(text);
        }

        protected virtual string GetCellValue(object obj)
        {
            double value = Math.Round(Convert.ToDouble(obj), 2);
            if (value > 999.99) value = Math.Round(value, 1);
            if (value > 9999.99) value = Math.Round(value, 0);
            return Convert.ToString(value);
        }

        protected virtual void FillChartData()
        {
            DataColumn dataColumn = dtChart.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Subject";

            for (int i = 0; i < chartRowCount; i++)
            {
                var drAdd = dtChart.Rows.Add();
                drAdd[0] = dtFullData.Rows[chartRowCount - 1 - i][9];
            }

            if (shortSubjectName.Length > 6) LabelTableSubject.Font.Size = 11;

            LabelTableRF.Text = "РФ";
            LabelTableSubject.Text = shortSubjectName;
            LabelTableDeseaseName.Text = "Заболевание";
            Label100K.Text = "(данные по заболеваемости на 100 тыс. человек)";
            Label100K.Font.Size = 10;
            Label100K.Width = Unit.Empty;
            Label100K.ForeColor = Color.DimGray;
            LabelTableRF.ForeColor = Color.DarkGray;
            LabelTableSubject.ForeColor = Color.DarkGray;
            LabelTableDeseaseName.ForeColor = Color.DarkGray;

            LabelTableRFValue1.Text = GetCellValue(dtFullData.Rows[0][8]);
            LabelTableRFValue2.Text = GetCellValue(dtFullData.Rows[1][8]);
            LabelTableRFValue3.Text = GetCellValue(dtFullData.Rows[2][8]);
            LabelTableRFValue4.Text = GetCellValue(dtFullData.Rows[3][8]);
            LabelTableRFValue5.Text = GetCellValue(dtFullData.Rows[4][8]);
            LabelTableRFValue6.Text = GetCellValue(dtFullData.Rows[5][8]);
            LabelTableRFValue7.Text = GetCellValue(dtFullData.Rows[6][8]);
            LabelTableRFValue8.Text = GetCellValue(dtFullData.Rows[7][8]);

            LabelTableSubjectValue1.Text = GetCellValue(dtFullData.Rows[0][2]);
            LabelTableSubjectValue2.Text = GetCellValue(dtFullData.Rows[1][2]);
            LabelTableSubjectValue3.Text = GetCellValue(dtFullData.Rows[2][2]);
            LabelTableSubjectValue4.Text = GetCellValue(dtFullData.Rows[3][2]);
            LabelTableSubjectValue5.Text = GetCellValue(dtFullData.Rows[4][2]);
            LabelTableSubjectValue6.Text = GetCellValue(dtFullData.Rows[5][2]);
            LabelTableSubjectValue7.Text = GetCellValue(dtFullData.Rows[6][2]);
            LabelTableSubjectValue8.Text = GetCellValue(dtFullData.Rows[7][2]);

            chart.DataSource = dtChart;
            chart.DataBind();

            chart.Height = 204;
            chart.Width = 182;

            chart.BarChart.SeriesSpacing = 0;

            chart.Axis.X.Visible = false;
            chart.Axis.Z.Visible = false;
            chart.Axis.Y.Visible = false;

            chart.TitleLeft.Visible = false;
            chart.TitleTop.Visible = false;
            chart.TitleBottom.Visible = false;
            chart.TitleRight.Visible = false;

            chart.Legend.Visible = false;
            chart.Data.ZeroAligned = true;

            chart.Axis.X.MajorGridLines.Thickness = 0;
            chart.Axis.X.MinorGridLines.Thickness = 0;

            chart.Axis.Y.MajorGridLines.Thickness = 0;
            chart.Axis.Y.MinorGridLines.Thickness = 0;

            chart.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }

        protected void chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int rfSize = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                var primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    var box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.rect = new Rectangle(box.rect.X - 1, box.rect.Y - 1,
                            box.rect.Width + 2, box.rect.Height + 1);

                        box.PE.FillGradientStyle = GradientStyle.BackwardDiagonal;
                        box.PE.ElementType = PaintElementType.Gradient;
                        box.PE.Fill = Color.Maroon;
                        box.PE.FillStopColor = Color.Red;
                        box.PE.Stroke = Color.Maroon;
                        box.PE.StrokeWidth = 0;


                        int appendY = 0;
                        if (box.Row == 0) appendY = 1;
                        // 
                        var box1 = new Box(new Rectangle(
                            box.rect.X + rfSize,
                            box.rect.Y,
                            box.rect.Width - rfSize,
                            box.rect.Height + appendY * 2))
                                       {
                                           PE =
                                               {
                                                   ElementType = PaintElementType.Gradient,
                                                   FillGradientStyle = GradientStyle.BackwardDiagonal,
                                                   Fill = Color.Maroon,
                                                   FillStopColor = Color.Red
                                               }
                                       };

                        box.PE.Stroke = Color.Red;
                        box1.PE.StrokeWidth = 0;
                        box1.Row = box.Roundness;
                        box1.Column = 0;
                        box1.Value = 1;
                        box1.Layer = e.ChartCore.GetChartLayer();
                        box1.Chart = chart.ChartType;
                        e.SceneGraph.Add(box1);

                        //
                        if (rfSize == 0)
                        {
                            double diffPercent = Convert.ToDouble(dtFullData.Rows[chartRowCount - 1 - box.Row][9]);
                            if (diffPercent > 0)
                            {
                                rfSize = Convert.ToInt32(Convert.ToDouble(box.rect.Width) / diffPercent);
                            }
                        }

                        box1 = new Box(new Rectangle(
                            box.rect.X - 1,
                            box.rect.Y,
                            rfSize,
                            box.rect.Height + appendY * 2))
                                   {
                                       PE =
                                           {
                                               ElementType = PaintElementType.Gradient,
                                               FillGradientStyle = GradientStyle.BackwardDiagonal,
                                               Fill = Color.FromArgb(0x003300),
                                               FillStopColor = Color.FromArgb(0x339900)
                                           }
                                   };

                        box.PE.Stroke = Color.FromArgb(0x003300);
                        box1.PE.StrokeWidth = 0;
                        box1.Row = box.Roundness;
                        box1.Column = 0;
                        box1.Value = 1;
                        box1.Layer = e.ChartCore.GetChartLayer();
                        box1.Chart = chart.ChartType;
                        e.SceneGraph.Add(box1);

                        const int width1 = 175;

                        string deseaseName = dtFullData.Rows[chartRowCount - 1 - box.Row][0].ToString().Trim();
                        AddChartText(e, box, 0, width1, deseaseName, StringAlignment.Near);

                        if (box.Row == 0) 
                        {
                            var line = new Line
                                           {
                                               lineStyle = {DrawStyle = LineDrawStyle.Solid},
                                               PE =
                                                   {
                                                       Stroke = Color.FromArgb(0x003300),
                                                       StrokeWidth = 1
                                                   },
                                               p1 = new Point(box.rect.X - 1, box.rect.Bottom + 2),
                                               p2 = new Point(box.rect.X + box1.rect.Width - 2, box.rect.Bottom + 2)
                                           };

                            e.SceneGraph.Add(line);
                        }
                    }
                }
            }
        }
    }
}
