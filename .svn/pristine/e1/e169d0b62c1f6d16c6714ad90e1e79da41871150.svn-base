using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.UltraChart.Resources.Editor;
using System.Windows.Forms;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolTip;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using Infragistics.Win;

namespace Krista.FM.Client.MDXExpert
{
    public partial class ChartTypeCtrl : ChartEditorControlBase
    {
        // Fields
        private static readonly string[] _Categories;
        private static readonly Hashtable _ChartIconInfoTable;
        private static readonly TileSkin _ChartImagesSkin;
        private UltraTabControl _MainTabControl;
        private static readonly ChartType[] _OrderedChartTypes;
        private UltraToolTipManager _TooltipManager;
        private static readonly string CATEGORY_ALL;
        private static readonly string CATEGORY_BAR;
        private static readonly string CATEGORY_COMPOSITE;
        private static readonly string CATEGORY_LINE;
        private static readonly string CATEGORY_OTHER;
        private static readonly string CATEGORY_XY;
        internal const int CONTROL_MAXHEIGHT = 290;
        internal static readonly int CONTROL_WIDTH = 210;
        private ChartType ctype;
        private const int HEIGHT_OF_TAB_AREA = 0x1c;
        internal const int HOVER_IMAGE_OFFSET = 0x38;
        internal const int ICON_HEIGHT = 0x30;
        private const int ICON_WIDTH = 0x30;
        private const int INTERICON_SPACING = 4;
        private const int PADDING_TOPLEFT = 2;
        private const int PICTUREBOX_HEIGHT = 0x30;
        private const int PICTUREBOX_WIDTH = 0x30;
        internal const int SELECTED_IMAGE_OFFSET = 0x70;
        private const int SKINIMAGE_COLS = 12;
        private const int SKINIMAGE_ROWS = 14;

        // Methods
        static ChartTypeCtrl()
        {
            ChartType[] typeArray = new ChartType[0x34];
            typeArray[1] = ChartType.StackColumnChart;
            typeArray[2] = ChartType.ColumnChart3D;
            typeArray[3] = ChartType.Stack3DColumnChart;
            typeArray[4] = ChartType.LineChart;
            typeArray[5] = ChartType.LineChart3D;
            typeArray[6] = ChartType.AreaChart;
            typeArray[7] = ChartType.AreaChart3D;
            typeArray[8] = ChartType.BarChart;
            typeArray[9] = ChartType.StackBarChart;
            typeArray[10] = ChartType.BarChart3D;
            typeArray[11] = ChartType.Stack3DBarChart;
            typeArray[12] = ChartType.SplineChart;
            typeArray[13] = ChartType.BubbleChart;
            typeArray[14] = ChartType.ScatterChart;
            typeArray[15] = ChartType.RadarChart;
            typeArray[0x10] = ChartType.PieChart3D;
            typeArray[0x11] = ChartType.PieChart;
            typeArray[0x12] = ChartType.DoughnutChart;
            typeArray[0x13] = ChartType.DoughnutChart3D;
            typeArray[20] = ChartType.HeatMapChart3D;
            typeArray[0x15] = ChartType.ColumnLineChart;
            typeArray[0x16] = ChartType.SplineAreaChart;
            typeArray[0x17] = ChartType.CandleChart;
            typeArray[0x18] = ChartType.HeatMapChart;
            typeArray[0x19] = ChartType.ScatterLineChart;
            typeArray[0x1a] = ChartType.StepLineChart;
            typeArray[0x1b] = ChartType.StepAreaChart;
            typeArray[0x1c] = ChartType.GanttChart;
            typeArray[0x1d] = ChartType.PolarChart;
            typeArray[30] = ChartType.ParetoChart;
            typeArray[0x1f] = ChartType.StackAreaChart;
            typeArray[0x20] = ChartType.StackSplineAreaChart;
            typeArray[0x21] = ChartType.StackLineChart;
            typeArray[0x22] = ChartType.StackSplineChart;
            typeArray[0x23] = ChartType.BoxChart;
            typeArray[0x24] = ChartType.CylinderColumnChart3D;
            typeArray[0x25] = ChartType.CylinderStackColumnChart3D;
            typeArray[0x26] = ChartType.CylinderBarChart3D;
            typeArray[0x27] = ChartType.CylinderStackBarChart3D;
            typeArray[40] = ChartType.ProbabilityChart;
            typeArray[0x29] = ChartType.PointChart3D;
            typeArray[0x2a] = ChartType.BubbleChart3D;
            typeArray[0x2b] = ChartType.FunnelChart;
            typeArray[0x2c] = ChartType.FunnelChart3D;
            typeArray[0x2d] = ChartType.PyramidChart;
            typeArray[0x2e] = ChartType.PyramidChart3D;
            typeArray[0x2f] = ChartType.ConeChart3D;
            typeArray[0x30] = ChartType.SplineChart3D;
            typeArray[0x31] = ChartType.SplineAreaChart3D;
            typeArray[50] = ChartType.HistogramChart;
            typeArray[0x33] = ChartType.TreeMapChart;
            //typeArray[0x34] = ChartType.Composite;
            _OrderedChartTypes = typeArray;
            CATEGORY_ALL = "Все";
            CATEGORY_COMPOSITE = "Композитная";
            CATEGORY_BAR = "Гистограмма";
            CATEGORY_LINE = "График";
            CATEGORY_XY = "XY";
            CATEGORY_OTHER = "Другие";
            _Categories = new string[] { CATEGORY_BAR, CATEGORY_LINE, CATEGORY_XY, CATEGORY_OTHER };
            _ChartImagesSkin = new TileSkin((Bitmap)Image.FromStream(typeof(Infragistics.UltraChart.Design.ChartTypeCtrl).Assembly.GetManifestResourceStream("Infragistics.UltraChart.Design.chartComposite.png")), new Size(0x30, 0x30), new Size(12, 14), new Size(4, 4), new Size(2, 2));
            _ChartIconInfoTable = new Hashtable();
            InitializeChartIconInfoTable();
            CONTROL_WIDTH += SystemInformation.VerticalScrollBarWidth;
        }

        public ChartTypeCtrl(ChartUIEditorBase ed)
            : base(ed)
        {
            try
            {
                this.InitTabControl();
            }
            catch (LicenseException)
            {
            }
            this._TooltipManager = new UltraToolTipManager();
            this.TooltipManager.DisplayStyle = ToolTipDisplayStyle.Office2007;
            this.TooltipManager.InitialDelay = 10;
            base.Width = CONTROL_WIDTH;
        }

        private PictureBox AddPictureBoxToTab(ChartIconInfo iconInfo, UltraTab tab, ChartType currentChartType)
        {
            PictureBox control = new PictureBox();
            control.Name = tab.Key.ToLower() + "_" + currentChartType.ToString();
            control.Size = new Size(0x30, 0x30);
            control.SizeMode = PictureBoxSizeMode.StretchImage;
            if (currentChartType == this.ctype)
            {
                control.Image = ChartImagesSkin.Elements[iconInfo.SkinIndex + 0x70].Image;
            }
            else
            {
                control.Image = ChartImagesSkin.Elements[iconInfo.SkinIndex].Image;
            }

            control.MouseDown += new MouseEventHandler(this.pictureBoxes_MouseDown);
            control.MouseEnter += new EventHandler(this.pictureBoxes_MouseEnter);
            control.MouseLeave += new EventHandler(this.pictureBoxes_MouseLeave);
            this.TooltipManager.SetUltraToolTip(control, new UltraToolTipInfo(ChartTypeConverter.GetLocalizedChartType(currentChartType), ToolTipImage.None, null, DefaultableBoolean.True));
            tab.TabPage.Controls.Add(control);
            return control;
        }

        private void done()
        {
            base.Ok();
            this.OnClick(new EventArgs());
        }

        private ChartType GetChartType(string controlName)
        {
            return (ChartType)Enum.Parse(typeof(ChartType), this.GetChartTypeString(controlName));
        }

        private string GetChartTypeString(string controlName)
        {
            if (controlName != null)
            {
                string[] strArray = controlName.Split(new char[] { '_' });
                if ((strArray != null) && (strArray.Length == 2))
                {
                    return strArray[1];
                }
            }
            return "ColumnChart";
        }

        public override object GetValue()
        {
            return this.ctype;
        }

        private static void InitializeChartIconInfoTable()
        {
            int num = 0;
            ChartIconInfoTable.Add(ChartType.ColumnChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.LineChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.BarChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.SplineChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.PieChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.ColumnLineChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.ScatterLineChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.PolarChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_XY }));
            ChartIconInfoTable.Add(ChartType.ParetoChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.CylinderStackBarChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.StackSplineChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.PyramidChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.PointChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_XY }));
            ChartIconInfoTable.Add(ChartType.HistogramChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.StackColumnChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.LineChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.StackBarChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.BubbleChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_XY }));
            ChartIconInfoTable.Add(ChartType.PieChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.SplineAreaChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.StepLineChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.StackAreaChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.CylinderColumnChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.BoxChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.ProbabilityChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_XY }));
            ChartIconInfoTable.Add(ChartType.PyramidChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.SplineAreaChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            num++;
            ChartIconInfoTable.Add(ChartType.ColumnChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.AreaChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.BarChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.ScatterChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_XY }));
            ChartIconInfoTable.Add(ChartType.DoughnutChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.CandleChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.StepAreaChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.StackSplineAreaChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.CylinderStackColumnChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));

            //ChartIconInfoTable.Add(ChartType.Composite, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            num++;

            ChartIconInfoTable.Add(ChartType.FunnelChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.ConeChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.SplineChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            num++;
            ChartIconInfoTable.Add(ChartType.Stack3DColumnChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.AreaChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.Stack3DBarChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.RadarChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.HeatMapChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.HeatMapChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.GanttChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.StackLineChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_LINE }));
            ChartIconInfoTable.Add(ChartType.CylinderBarChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_BAR }));
            ChartIconInfoTable.Add(ChartType.DoughnutChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.FunnelChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            ChartIconInfoTable.Add(ChartType.BubbleChart3D, new ChartIconInfo(num++, true, new string[] { CATEGORY_XY }));
            ChartIconInfoTable.Add(ChartType.TreeMapChart, new ChartIconInfo(num++, true, new string[] { CATEGORY_OTHER }));
            num++;
        }

        private void InitTabControl()
        {
            this._MainTabControl = new UltraTabControl();
            base.Controls.Add(this._MainTabControl);
            this._MainTabControl.Dock = DockStyle.Fill;
        }

        private void pictureBoxes_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox box = sender as PictureBox;
            this.ctype = this.GetChartType(box.Name);
            this.done();
        }

        private void pictureBoxes_MouseEnter(object sender, EventArgs e)
        {
            PictureBox box = sender as PictureBox;
            ChartType chartType = this.GetChartType(box.Name);
            int skinIndex = ((ChartIconInfo)ChartIconInfoTable[chartType]).SkinIndex;
            box.Image = ChartImagesSkin.Elements[skinIndex + 0x38].Image;
        }

        private void pictureBoxes_MouseLeave(object sender, EventArgs e)
        {
            PictureBox box = sender as PictureBox;
            ChartType chartType = this.GetChartType(box.Name);
            int skinIndex = ((ChartIconInfo)ChartIconInfoTable[chartType]).SkinIndex;
            if (this.ctype == chartType)
            {
                box.Image = ChartImagesSkin.Elements[skinIndex + 0x70].Image;
            }
            else
            {
                box.Image = ChartImagesSkin.Elements[skinIndex].Image;
            }
        }

        public void ResetTabs()
        {
            if (this._MainTabControl == null)
            {
                this.InitTabControl();
            }
            this._MainTabControl.Tabs.Clear();
            this._MainTabControl.Tabs.Add("All", CATEGORY_ALL).TabPage.AutoScroll = true;
            this._MainTabControl.Tabs.Add("Composite", CATEGORY_COMPOSITE).TabPage.AutoScroll = true;
            Hashtable hashtable = new Hashtable();
            hashtable.Add(this._MainTabControl.Tabs["All"], new Point(0, 0));
            hashtable.Add(this._MainTabControl.Tabs["Composite"], new Point(0, 0));
            foreach (string str in Categories)
            {
                UltraTab key = this._MainTabControl.Tabs.Add(str, str);
                hashtable.Add(key, new Point(0, 0));
            }
            ChartType[] values = (ChartType[])Enum.GetValues(typeof(ChartType));
            foreach (ChartType type in values)
            {
                if (ChartIconInfoTable[type] is ChartIconInfo)
                {
                    ChartIconInfo iconInfo = (ChartIconInfo)ChartIconInfoTable[type];
                    if (iconInfo.Visible)
                    {
                        UltraTab tab = this._MainTabControl.Tabs["All"];
                        Point point = this.AddPictureBoxToTab(iconInfo, tab, type).Location = (Point)hashtable[tab];
                        if ((point.X + 0x60) >= base.Width)
                        {
                            point.X = 0;
                            point.Y += 0x30;
                        }
                        else
                        {
                            point.X += 0x30;
                        }
                        hashtable[tab] = point;
                        base.Height = Math.Max(base.Size.Height, (point.Y + 0x30) + 0x1c);
                        base.Height = Math.Min(290, base.Height);
                        foreach (string str2 in iconInfo.Categories)
                        {
                            UltraTab tab3 = this._MainTabControl.Tabs[str2];
                            point = this.AddPictureBoxToTab(iconInfo, tab3, type).Location = (Point)hashtable[tab3];
                            if ((point.X + 0x60) >= base.Width)
                            {
                                point.X = 0;
                                point.Y += 0x30;
                            }
                            else
                            {
                                point.X += 0x30;
                            }
                            hashtable[tab3] = point;
                        }
                        object[] customAttributes = typeof(ChartType).GetField(type.ToString()).GetCustomAttributes(typeof(CompositeChartBrowsableAttribute), true);
                        if ((customAttributes.Length == 0) || ((customAttributes[0] is CompositeChartBrowsableAttribute) && ((CompositeChartBrowsableAttribute)customAttributes[0]).Browsable))
                        {
                            UltraTab tab4 = this._MainTabControl.Tabs["Composite"];
                            point = this.AddPictureBoxToTab(iconInfo, tab4, type).Location = (Point)hashtable[tab4];
                            if ((point.X + 0x60) >= base.Width)
                            {
                                point.X = 0;
                                point.Y += 0x30;
                            }
                            else
                            {
                                point.X += 0x30;
                            }
                            hashtable[tab4] = point;
                        }
                    }
                }
            }
        }

        public void SetComposite(bool composite)
        {
            if (this._MainTabControl != null)
            {
                for (int i = 0; i < this._MainTabControl.Tabs.Count; i++)
                {
                    UltraTab tab = this._MainTabControl.Tabs[i];
                    if (composite != (tab.Key == "Composite"))
                    {
                        this._MainTabControl.Tabs.RemoveAt(i);
                        i--;
                    }
                }
                this._MainTabControl.SelectedTab = this._MainTabControl.Tabs[0];
            }
        }

        public override void SetValue(object val)
        {
            this.ctype = (ChartType)val;
        }

        // Properties
        private static string[] Categories
        {
            get
            {
                return _Categories;
            }
        }

        internal static Hashtable ChartIconInfoTable
        {
            get
            {
                return _ChartIconInfoTable;
            }
        }

        internal static TileSkin ChartImagesSkin
        {
            get
            {
                return _ChartImagesSkin;
            }
        }

        internal static ChartType[] OrderedChartTypes
        {
            get
            {
                return _OrderedChartTypes;
            }
        }

        private UltraToolTipManager TooltipManager
        {
            get
            {
                return this._TooltipManager;
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        internal struct ChartIconInfo
        {
            public int SkinIndex;
            public bool Visible;
            public string[] Categories;
            public ChartIconInfo(int skinIndex, bool visible, string[] categories)
            {
                this.SkinIndex = skinIndex;
                this.Visible = visible;
                this.Categories = categories;
            }
        }
    }
}
