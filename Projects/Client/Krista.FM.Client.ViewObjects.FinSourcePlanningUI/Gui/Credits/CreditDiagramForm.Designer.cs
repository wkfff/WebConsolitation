namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits
{
    partial class CreditDiagramForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.UltraChart.Resources.Appearance.PaintElement paintElement1 = new Infragistics.UltraChart.Resources.Appearance.PaintElement();
            Infragistics.UltraChart.Resources.Appearance.TimeAxisAppearance timeAxisAppearance1 = new Infragistics.UltraChart.Resources.Appearance.TimeAxisAppearance();
            Infragistics.UltraChart.Resources.Appearance.ColumnChartAppearance columnChartAppearance1 = new Infragistics.UltraChart.Resources.Appearance.ColumnChartAppearance();
            Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.uchDiagram = new Infragistics.Win.UltraWinChart.UltraChart();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uchDiagram)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ultraTabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button3);
            this.splitContainer1.Panel2.Controls.Add(this.button2);
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Size = new System.Drawing.Size(701, 509);
            this.splitContainer1.SplitterDistance = 457;
            this.splitContainer1.TabIndex = 0;
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl1.Size = new System.Drawing.Size(701, 457);
            this.ultraTabControl1.TabIndex = 2;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Данные";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Диаграмма";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.ultraTabControl1.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.VisualStudio2005;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(697, 434);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.ultraGrid1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(2, 21);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(697, 434);
            // 
            // ultraGrid1
            // 
            appearance4.BackColor = System.Drawing.SystemColors.Window;
            appearance4.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ultraGrid1.DisplayLayout.Appearance = appearance4;
            this.ultraGrid1.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraGrid1.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance1.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance1.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGrid1.DisplayLayout.GroupByBox.Appearance = appearance1;
            appearance2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGrid1.DisplayLayout.GroupByBox.BandLabelAppearance = appearance2;
            this.ultraGrid1.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance3.BackColor2 = System.Drawing.SystemColors.Control;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGrid1.DisplayLayout.GroupByBox.PromptAppearance = appearance3;
            this.ultraGrid1.DisplayLayout.MaxColScrollRegions = 1;
            this.ultraGrid1.DisplayLayout.MaxRowScrollRegions = 1;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraGrid1.DisplayLayout.Override.ActiveCellAppearance = appearance12;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ultraGrid1.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.ultraGrid1.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ultraGrid1.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGrid1.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance5.BorderColor = System.Drawing.Color.Silver;
            appearance5.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ultraGrid1.DisplayLayout.Override.CellAppearance = appearance5;
            this.ultraGrid1.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ultraGrid1.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGrid1.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance11.TextHAlignAsString = "Left";
            this.ultraGrid1.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.ultraGrid1.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ultraGrid1.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.ultraGrid1.DisplayLayout.Override.RowAppearance = appearance10;
            this.ultraGrid1.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance8.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ultraGrid1.DisplayLayout.Override.TemplateAddRowAppearance = appearance8;
            this.ultraGrid1.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGrid1.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ultraGrid1.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ultraGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGrid1.Location = new System.Drawing.Point(0, 0);
            this.ultraGrid1.Name = "ultraGrid1";
            this.ultraGrid1.Size = new System.Drawing.Size(697, 434);
            this.ultraGrid1.TabIndex = 0;
            this.ultraGrid1.Text = "ultraGrid1";
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.uchDiagram);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(697, 432);
            // 
            //			'UltraChart' properties's serialization: Since 'ChartType' changes the way axes look,
            //			'ChartType' must be persisted ahead of any Axes change made in design time.
            //		
            this.uchDiagram.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.StackColumnChart;
            // 
            // uchDiagram
            // 
            this.uchDiagram.Axis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            paintElement1.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.None;
            paintElement1.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            this.uchDiagram.Axis.PE = paintElement1;
            this.uchDiagram.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.X.Labels.FontColor = System.Drawing.Color.DimGray;
            this.uchDiagram.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.X.Labels.ItemFormatString = "test";
            this.uchDiagram.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Custom;
            this.uchDiagram.Axis.X.Labels.OrientationAngle = 20;
            this.uchDiagram.Axis.X.Labels.ReverseText = true;
            this.uchDiagram.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.uchDiagram.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.X.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Custom;
            this.uchDiagram.Axis.X.Labels.SeriesLabels.OrientationAngle = 45;
            this.uchDiagram.Axis.X.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.X.LineThickness = 1;
            this.uchDiagram.Axis.X.MajorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.uchDiagram.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.X.MajorGridLines.Visible = true;
            this.uchDiagram.Axis.X.Margin.Far.Value = 5.3658536585365857;
            this.uchDiagram.Axis.X.MinorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.uchDiagram.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.X.MinorGridLines.Visible = false;
            this.uchDiagram.Axis.X.TickmarkIntervalType = Infragistics.UltraChart.Shared.Styles.AxisIntervalType.Months;
            this.uchDiagram.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            timeAxisAppearance1.TimeAxisStyle = Infragistics.UltraChart.Shared.Styles.RulerGenre.Discrete;
            this.uchDiagram.Axis.X.TimeAxisStyle = timeAxisAppearance1;
            this.uchDiagram.Axis.X.Visible = true;
            this.uchDiagram.Axis.X2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.X2.Labels.FontColor = System.Drawing.Color.Gray;
            this.uchDiagram.Axis.X2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.uchDiagram.Axis.X2.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.uchDiagram.Axis.X2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.X2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.uchDiagram.Axis.X2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.X2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.uchDiagram.Axis.X2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.uchDiagram.Axis.X2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.X2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.uchDiagram.Axis.X2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.X2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.X2.Labels.Visible = false;
            this.uchDiagram.Axis.X2.LineThickness = 1;
            this.uchDiagram.Axis.X2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.X2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.uchDiagram.Axis.X2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.X2.MajorGridLines.Visible = true;
            this.uchDiagram.Axis.X2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.X2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.uchDiagram.Axis.X2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.X2.MinorGridLines.Visible = false;
            this.uchDiagram.Axis.X2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.uchDiagram.Axis.X2.Visible = false;
            this.uchDiagram.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
            this.uchDiagram.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.uchDiagram.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.uchDiagram.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Y.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.uchDiagram.Axis.Y.Labels.SeriesLabels.FormatString = "";
            this.uchDiagram.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.uchDiagram.Axis.Y.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Y.LineThickness = 1;
            this.uchDiagram.Axis.Y.MajorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.uchDiagram.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Y.MajorGridLines.Visible = true;
            this.uchDiagram.Axis.Y.Margin.Far.Value = 10.909090909090908;
            this.uchDiagram.Axis.Y.MinorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.uchDiagram.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Y.MinorGridLines.Visible = false;
            this.uchDiagram.Axis.Y.TickmarkInterval = 200;
            this.uchDiagram.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.uchDiagram.Axis.Y.Visible = true;
            this.uchDiagram.Axis.Y2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Y2.Labels.FontColor = System.Drawing.Color.Gray;
            this.uchDiagram.Axis.Y2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.uchDiagram.Axis.Y2.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.uchDiagram.Axis.Y2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Y2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Y2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Y2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.uchDiagram.Axis.Y2.Labels.SeriesLabels.FormatString = "";
            this.uchDiagram.Axis.Y2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.uchDiagram.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Y2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Y2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Y2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Y2.Labels.Visible = false;
            this.uchDiagram.Axis.Y2.LineThickness = 1;
            this.uchDiagram.Axis.Y2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Y2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.uchDiagram.Axis.Y2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Y2.MajorGridLines.Visible = true;
            this.uchDiagram.Axis.Y2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Y2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.uchDiagram.Axis.Y2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Y2.MinorGridLines.Visible = false;
            this.uchDiagram.Axis.Y2.TickmarkInterval = 200;
            this.uchDiagram.Axis.Y2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.uchDiagram.Axis.Y2.Visible = false;
            this.uchDiagram.Axis.Z.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Z.Labels.FontColor = System.Drawing.Color.DimGray;
            this.uchDiagram.Axis.Z.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.uchDiagram.Axis.Z.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.uchDiagram.Axis.Z.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Z.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Z.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Z.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.uchDiagram.Axis.Z.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.uchDiagram.Axis.Z.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Z.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Z.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Z.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Z.Labels.Visible = false;
            this.uchDiagram.Axis.Z.LineThickness = 1;
            this.uchDiagram.Axis.Z.MajorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Z.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.uchDiagram.Axis.Z.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Z.MajorGridLines.Visible = true;
            this.uchDiagram.Axis.Z.MinorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Z.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.uchDiagram.Axis.Z.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Z.MinorGridLines.Visible = false;
            this.uchDiagram.Axis.Z.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.uchDiagram.Axis.Z.Visible = false;
            this.uchDiagram.Axis.Z2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Z2.Labels.FontColor = System.Drawing.Color.Gray;
            this.uchDiagram.Axis.Z2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.uchDiagram.Axis.Z2.Labels.ItemFormatString = "";
            this.uchDiagram.Axis.Z2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Z2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Z2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.uchDiagram.Axis.Z2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.uchDiagram.Axis.Z2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.uchDiagram.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.uchDiagram.Axis.Z2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.uchDiagram.Axis.Z2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Z2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.uchDiagram.Axis.Z2.Labels.Visible = false;
            this.uchDiagram.Axis.Z2.LineThickness = 1;
            this.uchDiagram.Axis.Z2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Z2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.uchDiagram.Axis.Z2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Z2.MajorGridLines.Visible = true;
            this.uchDiagram.Axis.Z2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.uchDiagram.Axis.Z2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.uchDiagram.Axis.Z2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.uchDiagram.Axis.Z2.MinorGridLines.Visible = false;
            this.uchDiagram.Axis.Z2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.uchDiagram.Axis.Z2.Visible = false;
            this.uchDiagram.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.uchDiagram.ColorModel.AlphaLevel = ((byte)(150));
            this.uchDiagram.ColorModel.ColorBegin = System.Drawing.Color.DodgerBlue;
            this.uchDiagram.ColorModel.ColorEnd = System.Drawing.Color.DarkRed;
            this.uchDiagram.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomRandom;
            this.uchDiagram.ColorModel.Scaling = Infragistics.UltraChart.Shared.Styles.ColorScaling.Increasing;
            columnChartAppearance1.ColumnSpacing = 5;
            this.uchDiagram.ColumnChart = columnChartAppearance1;
            this.uchDiagram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uchDiagram.Effects.Effects.Add(gradientEffect1);
            this.uchDiagram.Location = new System.Drawing.Point(0, 0);
            this.uchDiagram.Name = "uchDiagram";
            this.uchDiagram.Size = new System.Drawing.Size(697, 432);
            this.uchDiagram.TabIndex = 0;
            this.uchDiagram.Tooltips.HighlightFillColor = System.Drawing.Color.DimGray;
            this.uchDiagram.Tooltips.HighlightOutlineColor = System.Drawing.Color.DarkGray;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(533, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(614, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(415, 13);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "Сохранить отчет";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button1_Click);
            // 
            // CreditDiagramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 509);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "CreditDiagramForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Отчет по кредитной линии";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uchDiagram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinChart.UltraChart uchDiagram;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;

    }
}