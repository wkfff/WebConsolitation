using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapPanelBrowseAdapterBase : FilterablePropertyBase
    {
        #region Поля

        private DockablePanel mapPanel;
        private MapPanelAppearanceBrowseAdapter appearance;

        #endregion

        #region Свойства

        [Description("Текст подсказки")]
        [DisplayName("Подсказка")]
        [Browsable(true)]
        public string ToolTip
        {
            get { return mapPanel.ToolTip; }
            set { mapPanel.ToolTip = value; }
        }

        [Description("Показывать панель")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return mapPanel.Visible; }
            set { mapPanel.Visible = value; }
        }

        [Description("Порядок размещения")]
        [DisplayName("Порядок размещения")]
        [Browsable(true)]
        public int ZOrder
        {
            get { return mapPanel.ZOrder; }
            set { mapPanel.ZOrder = value; }
        }

        [Description("Внешний вид панели")]
        [DisplayName("Внешний вид панели")]
        [Browsable(true)]
        public MapPanelAppearanceBrowseAdapter Appearance
        {
            get { return appearance; }
            set { appearance = value; }
        }

        [Browsable(false)]
        public bool ShowAlignment
        {
            get { return !((this is MapLegendBrowseClass)&&(this.mapPanel.Name == Consts.objectList));}
        }

        [Description("Расположение")]
        [DisplayName("Расположение")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("ShowAlignment", "True")]
        [Browsable(true)]
        public MapPanelAlignment Alignment
        {
            get { return GetPanelAlignment(); }
            set { SetPanelAlignment(value); }
        }


        #endregion

        public MapPanelBrowseAdapterBase(DockablePanel mapPanel)
        {
            this.mapPanel = mapPanel;
            this.appearance = new MapPanelAppearanceBrowseAdapter(mapPanel);
        }


        private void SetPanelAlignment(MapPanelAlignment lAlignment)
        {
            switch (lAlignment)
            {
                case MapPanelAlignment.BottomDockCenter:
                    this.mapPanel.Dock = PanelDockStyle.Bottom;
                    this.mapPanel.DockAlignment = DockAlignment.Center;
                    break;
                case MapPanelAlignment.BottomDockLeft:
                    this.mapPanel.Dock = PanelDockStyle.Bottom;
                    this.mapPanel.DockAlignment = DockAlignment.Near;
                    break;
                case MapPanelAlignment.BottomDockRight:
                    this.mapPanel.Dock = PanelDockStyle.Bottom;
                    this.mapPanel.DockAlignment = DockAlignment.Far;
                    break;
                case MapPanelAlignment.LeftDockBottom:
                    this.mapPanel.Dock = PanelDockStyle.Left;
                    this.mapPanel.DockAlignment = DockAlignment.Far;
                    break;
                case MapPanelAlignment.LeftDockCenter:
                    this.mapPanel.Dock = PanelDockStyle.Left;
                    this.mapPanel.DockAlignment = DockAlignment.Center;
                    break;
                case MapPanelAlignment.LeftDockTop:
                    this.mapPanel.Dock = PanelDockStyle.Left;
                    this.mapPanel.DockAlignment = DockAlignment.Near;
                    break;
                case MapPanelAlignment.RightDockBottom:
                    this.mapPanel.Dock = PanelDockStyle.Right;
                    this.mapPanel.DockAlignment = DockAlignment.Far;
                    break;
                case MapPanelAlignment.RightDockCenter:
                    this.mapPanel.Dock = PanelDockStyle.Right;
                    this.mapPanel.DockAlignment = DockAlignment.Center;
                    break;
                case MapPanelAlignment.RightDockTop:
                    this.mapPanel.Dock = PanelDockStyle.Right;
                    this.mapPanel.DockAlignment = DockAlignment.Near;
                    break;
                case MapPanelAlignment.TopDockCenter:
                    this.mapPanel.Dock = PanelDockStyle.Top;
                    this.mapPanel.DockAlignment = DockAlignment.Center;
                    break;
                case MapPanelAlignment.TopDockLeft:
                    this.mapPanel.Dock = PanelDockStyle.Top;
                    this.mapPanel.DockAlignment = DockAlignment.Near;
                    break;
                case MapPanelAlignment.TopDockRight:
                    this.mapPanel.Dock = PanelDockStyle.Top;
                    this.mapPanel.DockAlignment = DockAlignment.Far;
                    break;
            }
        }

        private MapPanelAlignment GetPanelAlignment()
        {
            if (this.mapPanel.DockAlignment == DockAlignment.Center)
            {
                switch (this.mapPanel.Dock)
                {
                    case PanelDockStyle.Left:
                        return MapPanelAlignment.LeftDockCenter;
                    case PanelDockStyle.Right:
                        return MapPanelAlignment.RightDockCenter;
                    case PanelDockStyle.Top:
                        return MapPanelAlignment.TopDockCenter;
                    case PanelDockStyle.Bottom:
                        return MapPanelAlignment.BottomDockCenter;
                }
            }
            if (this.mapPanel.DockAlignment == DockAlignment.Near)
            {
                switch (this.mapPanel.Dock)
                {
                    case PanelDockStyle.Left:
                        return MapPanelAlignment.LeftDockTop;
                    case PanelDockStyle.Right:
                        return MapPanelAlignment.RightDockTop;
                    case PanelDockStyle.Top:
                        return MapPanelAlignment.TopDockLeft;
                    case PanelDockStyle.Bottom:
                        return MapPanelAlignment.BottomDockLeft;
                }
            }
            if (this.mapPanel.DockAlignment == DockAlignment.Far)
            {
                switch (this.mapPanel.Dock)
                {
                    case PanelDockStyle.Left:
                        return MapPanelAlignment.LeftDockBottom;
                    case PanelDockStyle.Right:
                        return MapPanelAlignment.RightDockBottom;
                    case PanelDockStyle.Top:
                        return MapPanelAlignment.TopDockRight;
                    case PanelDockStyle.Bottom:
                        return MapPanelAlignment.BottomDockRight;
                }
            }

            return MapPanelAlignment.LeftDockTop;
        }



        public override string ToString()
        {
            return "";
        }


    }

    public enum MapPanelAlignment
    {
        [Description("По верхнему краю, слева")]
        TopDockLeft,
        [Description("По верхнему краю, в центре")]
        TopDockCenter,
        [Description("По верхнему краю, справа")]
        TopDockRight,
        [Description("По левому краю, сверху")]
        LeftDockTop,
        [Description("По левому краю, в центре")]
        LeftDockCenter,
        [Description("По левому краю, снизу")]
        LeftDockBottom,
        [Description("По правому краю, сверху")]
        RightDockTop,
        [Description("По правому краю, в центре")]
        RightDockCenter,
        [Description("По правому краю, снизу")]
        RightDockBottom,
        [Description("По нижнему краю, слева")]
        BottomDockLeft,
        [Description("По нижнему краю, в центре")]
        BottomDockCenter,
        [Description("По нижнему краю, справа")]
        BottomDockRight

    }

}
