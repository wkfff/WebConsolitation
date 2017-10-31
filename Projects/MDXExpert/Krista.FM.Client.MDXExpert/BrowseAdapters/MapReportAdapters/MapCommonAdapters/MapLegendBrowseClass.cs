using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapLegendBrowseClass : MapPanelBrowseAdapterBase
    {
        #region Поля

        private Legend legend;
        private MapLegendTitleAppearance titleAppearance;
        private LegendHeaderAppearance legendHeaderAppearance;
        private ItemColumnAppearance itemColumnAppearance;
        private MapSizeBrowse mapSizeBrowse;
        private MapFormatBrowseClass legendFormat;
        private RuleBase rule;

        private bool showFormat;

        #endregion

        #region Свойства

        [Description("Авторазмер легенды")]
        [DisplayName("Авторазмер")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool AutoSize
        {
            get { return legend.AutoSize; }
            set
            {
                if (value && ((this.legend.Dock == PanelDockStyle.Top) || (this.legend.Dock == PanelDockStyle.Bottom)))
                {
                    this.legend.TableStyle = LegendTableStyle.Wide;
                }
                else
                {
                    this.legend.TableStyle = LegendTableStyle.Tall;
                }

                legend.AutoSize = value;

            }
        }

        [Description("Размер легенды")]
        [DisplayName("Размер")]
        [DynamicPropertyFilter("AutoSize", "False")]
        [Browsable(true)]
        public MapSizeBrowse Size
        {
            get { return mapSizeBrowse; }
            set { mapSizeBrowse = value; }
        }

        [Description("Единицы размера")]
        [DisplayName("Единицы размера")]
        [TypeConverter(typeof(CoordinateUnitConverter))]
        [Browsable(true)]
        public CoordinateUnit SizeUnit
        {
            get { return legend.SizeUnit; }
            set { legend.SizeUnit = value; }
        }
        
        [Description("Минимальный размер шрифта при автоподгонке")]
        [DisplayName("Минимальный размер шрифта")]
        [Browsable(true)]
        public int AutoFitMinFontSize
        {
            get { return legend.AutoFitMinFontSize; }
            set { legend.AutoFitMinFontSize = value; }
        }

        [Description("Автоподгонка текста")]
        [DisplayName("Автоподгонка текста")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool AutoFitText
        {
            get { return legend.AutoFitText; }
            set { legend.AutoFitText = value; }
        }
        
        [Description("Шрифт легенды")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return legend.Font; }
            set { legend.Font = value; }
        }
        
        [Description("Чередующиеся строки")]
        [DisplayName("Чередующиеся строки")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool InterlacedRows
        {
            get { return legend.InterlacedRows; }
            set { legend.InterlacedRows = value; }
        }
        
        [Description("Цвет чередующихся строк")]
        [DisplayName("Цвет чередующихся строк")]
        [Browsable(true)]
        public Color InterlacedRowsColor
        {
            get { return legend.InterlacedRowsColor; }
            set { legend.InterlacedRowsColor = value; }
        }
        
/*        [Description("Выделить легенду")]
        [DisplayName("Выделить")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Selected
        {
            get { return legend.Selected; }
            set { legend.Selected = value; }
        }*/
        
        [Description("Цвет текста")]
        [DisplayName("Цвет текста")]
        [Browsable(true)]
        public Color TextColor
        {
            get { return legend.TextColor; }
            set { legend.TextColor = value; }
        }

        [Description("Граница переноса текста")]
        [DisplayName("Граница переноса текста")]
        [Browsable(true)]
        public int TextWrapThreshold
        {
            get { return legend.TextWrapThreshold; }
            set { legend.TextWrapThreshold = value; }
        }

        [Description("Заголовок")]
        [DisplayName("Заголовок")]
        [Browsable(true)]
        public MapLegendTitleAppearance TitleAppearance
        {
            get { return this.titleAppearance; }
            set { this.titleAppearance = value; }
        }

        [Description("Подзаголовок")]
        [DisplayName("Подзаголовок")]
        [Browsable(true)]
        public LegendHeaderAppearance LegendHeaderAppearance
        {
            get { return this.legendHeaderAppearance; }
            set { this.legendHeaderAppearance = value; }
        }

        [Description("Колонки")]
        [DisplayName("Колонки")]
        [DynamicPropertyFilter("IsObjectNumberLegend", "True")]
        [Browsable(true)]
        public ItemColumnAppearance ItemColumnAppearance
        {
            get { return this.itemColumnAppearance; }
            set { this.itemColumnAppearance = value; }
        }


        /*
        [Description("Расположение легенды")]
        [DisplayName("Расположение")]
        [TypeConverter(typeof(PanelDockStyleConverter))]
        [DynamicPropertyFilter("IsObjectNumberLegend", "False")]
        [Browsable(true)]
        public PanelDockStyle Dock
        {
            get { return this.legend.Dock; }
            set { this.legend.Dock = value; }
        }

        [Description("Выравнивание легенды")]
        [DisplayName("Выравнивание")]
        [TypeConverter(typeof(DockAlignmentConverter))]
        [DynamicPropertyFilter("IsObjectNumberLegend", "False")]
        [Browsable(true)]
        public DockAlignment DockAlignment
        {
            get { return this.legend.DockAlignment; }
            set { this.legend.DockAlignment = value; }
        }
        */

        /*Сейчас если false - вылезает av при изменении размера карты
        [Description("Расположить в области просмотра")]
        [DisplayName("Расположить в области просмотра")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool DockedInsideViewport
        {
            get { return this.legend.DockedInsideViewport; }
            set { this.legend.DockedInsideViewport = value; }
        }
        */

        [Browsable(false)]
        public bool ShowFormat
        {
            get { return this.showFormat; }
        }

        [Browsable(false)]
        public bool IsObjectNumberLegend
        {
            get { return Consts.objectList == this.legend.Name; }
        }


        [Description("Формат значений")]
        [DisplayName("Формат значений")]
        [DynamicPropertyFilter("ShowFormat", "True")]
        [Browsable(true)]
        public MapFormatBrowseClass LegendFormat
        {
            get
            {
                return legendFormat;
            }
            set
            {
                legendFormat = value;
            }
        }

        [Description("Расположение")]
        [DisplayName("Расположение")]
        [TypeConverter(typeof(PanelDockStyleConverter))]
        [DynamicPropertyFilter("IsObjectNumberLegend", "True")]
        [Browsable(true)]
        public PanelDockStyle NumberLegendAlignment
        {
            get { return this.legend.Dock; }
            set { SetObjectNumberLegendAligment(value); }
        }

        #endregion

        public MapLegendBrowseClass(Legend legend) : base(legend)
        {
            this.legend = legend;
            this.titleAppearance = new MapLegendTitleAppearance(legend);
            this.legendHeaderAppearance = new LegendHeaderAppearance(legend);
            this.itemColumnAppearance = new ItemColumnAppearance(legend);
            this.mapSizeBrowse = new MapSizeBrowse(legend.Size);

            this.rule = GetBindingRule();
            this.legendFormat = new MapFormatBrowseClass(GetLegendText(), "", true);
            this.legendFormat.UnitDisplayType = GetUnitDisplayType();
            this.legendFormat.FormatChanged += new ValueFormatEventHandler(legendFormat_FormatChanged);
            this.legendFormat.FormatStringChanged += new ValueFormatEventHandler(legendFormat_FormatStringChanged);

            this.showFormat = this.rule != null;
        }

        private void legendFormat_FormatChanged()
        {
            if (this.legend.Name == "Заливка")
            {
                SetCustomShapeRuleLegendFormat(this.LegendFormat.FormatString);
            }
            SetLegendText(this.LegendFormat.ApplyFormatToText(GetLegendText(), this.LegendFormat.FormatString));

            RefreshHeader(this.legend.CellColumns[1].HeaderText);
            this.legendFormat.SetUnitDisplayType(GetUnitDisplayType());
            
        }

        private void legendFormat_FormatStringChanged()
        {
            this.LegendFormat.FormatString = GetLegendText();
        }

        private void RefreshHeader(string header)
        {
            //очищаем заголовок от единиц измерения
            int fieldNameLength = GetFieldName().Length;
            if (fieldNameLength < header.Length)
            {
                header = header.Remove(fieldNameLength);
            }

            this.legend.CellColumns[1].HeaderText = header;

            if (this.LegendFormat.UnitDisplayType != UnitDisplayType.DisplayAtCaption)
            {
                return;
            }
            //получаем единицы измерения
            string unit = this.LegendFormat.GetUnits().Replace(@"\", "");
            
            if (unit != "")
            {
                this.legend.CellColumns[1].HeaderText = String.Format("{0}, {1}", header, unit);
            }
        }

        //получение, связанного с этой легендой, правила 
        private RuleBase GetBindingRule()
        {
            MapCore map = (MapCore)this.legend.ParentElement;
            if (map == null)
                return null;

            foreach (SymbolRule symbRule in map.SymbolRules)
            {
                if (symbRule.ShowInLegend == this.legend.Name)
                {
                    return symbRule;
                }
            }

            foreach (ShapeRule shapeRule in map.ShapeRules)
            {
                if (shapeRule.ShowInLegend == this.legend.Name)
                {
                    return shapeRule;
                }
            }

            if ((this.legend.Name == "Заливка") && (map.ShapeRules.Count > 0))
            {
                return map.ShapeRules[0];
            }

            return null;
        }

        private string GetLegendText()
        {
            if (rule is SymbolRule)
            {
                return ((SymbolRule) this.rule).LegendText;
            }
            else
                if (rule is ShapeRule)
                {
                    return ((ShapeRule)this.rule).LegendText;
                }
            return "";
        }

        private void SetLegendText(string text)
        {
            if (rule is SymbolRule)
            {
                ((SymbolRule) this.rule).LegendText = text;
            }
            else
                if (rule is ShapeRule)
                {
                    ((ShapeRule) this.rule).LegendText = text;
                }
        }

        /// <summary>
        /// установка формата для альтернативной легенды для заливки
        /// </summary>
        /// <param name="mask"></param>
        private void SetCustomShapeRuleLegendFormat(string mask)
        {
            ShapeRule shRule = (ShapeRule)this.rule;
            legend.Items.Clear();

            foreach (CustomColor customColor in shRule.CustomColors)
            {
                ArrayList aElements = customColor.GetAffectedElements();
                foreach (Shape sh in aElements)
                {
                    string objName = (string)sh["NAME"];

                    double value = 0;
                    if (sh[shRule.ShapeField] != null)
                        value = (double)sh[shRule.ShapeField];

                    string objValue = value.ToString(mask);

                    if (String.IsNullOrEmpty(objName))
                    {
                        continue;
                    }
                    LegendItem legItem = legend.Items.Add(sh.Name);

                    legItem.Cells.Add(LegendCellType.Symbol, objName, ContentAlignment.MiddleCenter);
                    legItem.Cells.Add(LegendCellType.Text, objName, ContentAlignment.MiddleLeft);
                    legItem.Cells.Add(LegendCellType.Text, objValue, ContentAlignment.MiddleRight);
                    legItem.Color = customColor.Color;

                }
            }
        }



        //получение имени показателя
        private string GetFieldName()
        {
            if (rule is SymbolRule)
            {
                return ((SymbolRule)this.rule).SymbolField;
            }
            else
                if (rule is ShapeRule)
                {
                    return ((ShapeRule)this.rule).ShapeField;
                }
            return "";
        }

        private UnitDisplayType GetUnitDisplayType()
        {
            if (!this.legendFormat.FormatCanUnits(this.legendFormat.FormatType))
            {
                return UnitDisplayType.DisplayAtValue;
            }

            if (GetLegendText().Contains(@"\р\."))
            {
                return UnitDisplayType.DisplayAtValue;
            }
            else
            {
                if (this.legend.CellColumns[1].HeaderText.Contains("р."))
                {
                    return UnitDisplayType.DisplayAtCaption;
                }
                else
                {
                    return UnitDisplayType.None;
                }
            }
        }


        /// <summary>
        /// Установка положения легенды со списком объектов
        /// </summary>
        /// <param name="dock"></param>
        private void SetObjectNumberLegendAligment(PanelDockStyle dock)
        {
            this.legend.Dock = dock;
            bool autoSize = this.legend.AutoSize;
            switch(dock)
            {

                case PanelDockStyle.Top:
                case PanelDockStyle.Bottom:
                    this.legend.TableStyle = LegendTableStyle.Wide;
                    this.legend.AutoSize = true;
                    this.legend.LegendStyle = LegendStyle.Table;
                    this.legend.DockAlignment = DockAlignment.Center;
                    this.legend.DockedInsideViewport = false;
                    Application.DoEvents();
                    this.legend.AutoSize = autoSize;
                    this.legend.TableStyle = autoSize ? LegendTableStyle.Wide : LegendTableStyle.Tall;
                    break;
                case PanelDockStyle.Left:
                case PanelDockStyle.Right:
                    this.legend.AutoSize = true;
                    this.legend.LegendStyle = LegendStyle.Column;
                    this.legend.DockAlignment = DockAlignment.Near;
                    this.legend.DockedInsideViewport = false;
                    this.legend.TableStyle = LegendTableStyle.Tall;
                    Application.DoEvents();
                    this.legend.AutoSize = autoSize;
                    break;
                case PanelDockStyle.None:
                    this.legend.LegendStyle = LegendStyle.Table;
                    this.legend.TableStyle = LegendTableStyle.Tall;
                    this.legend.DockedInsideViewport = true;

                    break;
                    
            }
        }


        public override string ToString()
        {
            return "";
        }


    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapLegendTitleAppearance : FilterablePropertyBase
    {
        #region Поля

        private Legend legend;

        #endregion

        #region Свойства

        [Description("Текст заголовка")]
        [DisplayName("Текст")]
        [Browsable(true)]
        public string Title
        {
            get { return legend.Title; }
            set
            {
                int titleWrapThreshold = this.TitleWrapThreshold;
                legend.Title = value;
                this.TitleWrapThreshold = titleWrapThreshold;
            }
        }

        [Description("Граница переноса заголовка")]
        [DisplayName("Граница переноса")]
        [Browsable(true)]
        public int TitleWrapThreshold
        {
            get { return GetTitleWrapThreshold(); }
            set { SetTitleWrapThreshold(value); }
        }

        [Description("Выравнивание заголовка")]
        [DisplayName("Выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [Browsable(true)]
        public StringAlignment Alignment
        {
            get { return legend.TitleAlignment; }
            set { legend.TitleAlignment = value; }
        }

        [Description("Цвет заголовка")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return legend.TitleColor; }
            set { legend.TitleColor = value; }
        }

        [Description("Цвет фона заголовка")]
        [DisplayName("Цвет фона")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return legend.TitleBackColor; }
            set { legend.TitleBackColor = value; }
        }

        [Description("Шрифт заголовка")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return legend.TitleFont; }
            set { legend.TitleFont = value; }
        }

        [Description("Разделитель заголовка")]
        [DisplayName("Разделитель")]
        [TypeConverter(typeof(LegendSeparatorTypeConverter))]
        [Browsable(true)]
        public LegendSeparatorType Separator
        {
            get { return legend.TitleSeparator; }
            set { legend.TitleSeparator = value; }
        }

        [Description("Цвет разделителя заголовка")]
        [DisplayName("Цвет разделителя")]
        [Browsable(true)]
        public Color SeparatorColor
        {
            get { return legend.TitleSeparatorColor; }
            set { legend.TitleSeparatorColor = value; }
        }

        #endregion


        public MapLegendTitleAppearance(Legend legend)
        {
            this.legend = legend;
        }

        private int GetTitleWrapThreshold()
        {
            return legend.Title.IndexOf("\n");
        }

        private void SetTitleWrapThreshold(int value)
        {
            legend.Title = legend.Title.Replace("\n", " ");

            int wrapThreshold = 0;

            while (((value + wrapThreshold) < legend.Title.Length) && ((value + wrapThreshold) > 0))
            {

                wrapThreshold = legend.Title.IndexOf(' ', value + wrapThreshold);

                if (wrapThreshold == -1)
                {
                    return;
                }

                legend.Title = String.Format(legend.Title.Substring(0, wrapThreshold) + "{0}" +
                                             legend.Title.Substring(wrapThreshold + 1), "\n");
            }
        }


        public override string ToString()
        {
            return "";
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LegendHeaderAppearance : FilterablePropertyBase
    {
        #region Поля

        private Legend legend;
        private LegendCellColumn column = null;

        #endregion

        #region Свойства

        [Browsable(false)]
        public bool IsObjectNumberLegend
        {
            get { return Consts.objectList == this.legend.Name; }
        }

        [Description("Разделитель подзаголовка")]
        [DisplayName("Разделитель")]
        [TypeConverter(typeof(LegendSeparatorTypeConverter))]
        [Browsable(true)]
        public LegendSeparatorType Separator
        {
            get { return legend.HeaderSeparator; }
            set { legend.HeaderSeparator = value; }
        }

        [Description("Цвет разделителя подзаголовка")]
        [DisplayName("Цвет разделителя")]
        [Browsable(true)]
        public Color SeparatorColor
        {
            get { return legend.HeaderSeparatorColor; }
            set { legend.HeaderSeparatorColor = value; }
        }

        [Description("Цвет фона подзаголовка")]
        [DisplayName("Цвет фона")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return GetBackColor(); }
            set { SetBackColor(value); }
        }

        [Description("Шрифт подзаголовка")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return GetHeaderFont(); }
            set { SetHeaderFont(value); }
        }

        [Description("Цвет подзаголовка")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return GetHeaderColor(); }
            set { SetHeaderColor(value); }
        }

        [Description("Выравнивание подзаголовка")]
        [DisplayName("Выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [DynamicPropertyFilter("IsObjectNumberLegend", "False")]
        [Browsable(true)]
        public StringAlignment HeaderTextAlignment
        {
            get { return column.HeaderTextAlignment; }
            set { column.HeaderTextAlignment = value; }
        }


        [Description("Граница переноса подзаголовка")]
        [DisplayName("Граница переноса")]
        [DynamicPropertyFilter("IsObjectNumberLegend", "False")]
        [Browsable(true)]
        public int HeaderWrapThreshold
        {
            get { return GetHeaderWrapThreshold(); }
            set { SetHeaderWrapThreshold(value); }
        }

        [Description("Текст подзаголовка")]
        [DisplayName("Текст")]
        [DynamicPropertyFilter("IsObjectNumberLegend", "False")]
        [Browsable(true)]
        public string HeaderText
        {
            get { return column.HeaderText; }
            set
            {
                int headerWrapThreshold = this.HeaderWrapThreshold;
                column.HeaderText = value;
                this.HeaderWrapThreshold = headerWrapThreshold; 
            }
        }



        #endregion



        public LegendHeaderAppearance(Legend legend)
        {
            this.legend = legend;
            if (legend.CellColumns.Count > 1)
            {
                this.column = legend.CellColumns[1];
            }
        }

        private void SetBackColor(Color color)
        {
            foreach (LegendCellColumn column in this.legend.CellColumns)
            {
                column.HeaderBackColor = color;
            }
        }

        private Color GetBackColor()
        {
            if (this.legend.CellColumns.Count > 0)
            {
                return this.legend.CellColumns[0].HeaderBackColor;
            }

            return Color.White;
        }

        private void SetHeaderColor(Color color)
        {
            foreach (LegendCellColumn column in this.legend.CellColumns)
            {
                column.HeaderColor = color;
            }
        }

        private Color GetHeaderColor()
        {
            if (this.legend.CellColumns.Count > 0)
            {
                return this.legend.CellColumns[0].HeaderColor;
            }

            return Color.White;
        }


        private void SetHeaderFont(Font font)
        {
            foreach (LegendCellColumn column in this.legend.CellColumns)
            {
                column.HeaderFont = font;
            }
        }

        private Font GetHeaderFont()
        {
            if (this.legend.CellColumns.Count > 0)
            {
                return this.legend.CellColumns[0].HeaderFont;
            }

            return null;
        }


        private int GetHeaderWrapThreshold()
        {
            return legend.CellColumns[1].HeaderText.IndexOf("\n");
        }

        private void SetHeaderWrapThreshold(int value)
        {
            legend.CellColumns[1].HeaderText = legend.CellColumns[1].HeaderText.Replace("\n", " ");

            int wrapThreshold = 0;

            while (((value + wrapThreshold) < legend.CellColumns[1].HeaderText.Length) && ((value + wrapThreshold) > 0))
            {

                wrapThreshold = legend.CellColumns[1].HeaderText.IndexOf(' ', value + wrapThreshold);

                if (wrapThreshold == -1)
                {
                    return;
                }

                legend.CellColumns[1].HeaderText = String.Format(legend.CellColumns[1].HeaderText.Substring(0, wrapThreshold) + "{0}" +
                                             legend.CellColumns[1].HeaderText.Substring(wrapThreshold + 1), "\n");
            }
        }



        public override string ToString()
        {
            return "";
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapSizeBrowse
    {
        #region Поля

        private MapSize mapSize;

        #endregion

        #region Свойства

        [Description("Высота")]
        [DisplayName("Высота")]
        [Browsable(true)]
        public float Height
        {
            get { return mapSize.Height; }
            set { mapSize.Height = value; }
        }

        [Description("Ширина")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public float Width
        {
            get { return mapSize.Width; }
            set { mapSize.Width = value; }
        }

        #endregion

        public MapSizeBrowse(MapSize mapSize)
        {
            this.mapSize = mapSize;
        }

        public override string ToString()
        {
            return "";
        }

    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ItemColumnAppearance : FilterablePropertyBase
    {
        #region Поля

        private Legend legend;

        #endregion

        #region Свойства

        [Browsable(false)]
        public bool IsObjectNumberLegend
        {
            get { return Consts.objectList == this.legend.Name; }
        }

        [Description("Разделитель колонок")]
        [DisplayName("Разделитель")]
        [TypeConverter(typeof(LegendSeparatorTypeConverter))]
        [Browsable(true)]
        public LegendSeparatorType Separator
        {
            get { return legend.ItemColumnSeparator; }
            set { legend.ItemColumnSeparator = value; }
        }

        [Description("Цвет разделителя колонок")]
        [DisplayName("Цвет разделителя")]
        [Browsable(true)]
        public Color SeparatorColor
        {
            get { return legend.ItemColumnSeparatorColor; }
            set { legend.ItemColumnSeparatorColor = value; }
        }

        [Description("Расстояние между колонками")]
        [DisplayName("Расстояние между колонками")]
        [Browsable(true)]
        public int ColumnSpacing
        {
            get { return legend.ItemColumnSpacing; }
            set { legend.ItemColumnSpacing = value; }
        }


        #endregion

        public ItemColumnAppearance(Legend legend)
        {
            this.legend = legend;
        }

        public override string ToString()
        {
            return "";
        }

    }


}
