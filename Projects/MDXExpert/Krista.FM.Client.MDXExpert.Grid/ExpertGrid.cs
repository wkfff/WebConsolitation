using System;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Exporter;
using Krista.FM.Client.MDXExpert.Grid.Painters;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Microsoft.AnalysisServices.AdomdClient;
using XmlHelper=Krista.FM.Common.Xml.XmlHelper;
using Krista.FM.Client.MDXExpert.CommonClass;

namespace Krista.FM.Client.MDXExpert.Grid
{
    public partial class ExpertGrid : UserControl, IGridUserInterface
    {
        #region Поля
        private RowAxis _rows;
        private ColumnAxis _columns;
        private ProfessionalPainter _painter;
        private RowCaptions _rowsCaptions;
        private ColumnCaptions _columnsCaptions;
        private FilterCaptions _filtersCaptions;
        private MeasureCaptionsSections _measuresCaptionsSections;
        private MeasuresData _measuresData;
        private MeasuresStub _measuresStub;

        private Graphics _graphics;
        //вспомогательный класс, отвечает за рисование сплитеров, и изменение с помощью их, размеров заголовков
        private GridSplitter _splitter;
        //вспомогательный класс, отвечает за рисование рамки выделения ячеек
        private GridSelectionFrame _selectionFrame;
        //вспомогательный класс, в котором будет информация о текущих выделенных областях и объектах грида, 
        //а так же методы для их поиска
        private LocationHelper _locationHelper;
        //комментарии к ячейкам, отображает и скрывает их
        private CellComments _cellsComments;
        private SelectedCells _selectedCells;
        //вспомогательный класс, здесь храняться характеристики скролов, смещение по осям, и т.д.
        private ScrollBarState _vScrollBarState;
        private ScrollBarState _hScrollBarState;
        private ScrollBarState _filtersScrollBarState;
        //экспортер 
        private GridExporter _exporter;

        //монитор, отчечающий за возможность передвегаться по гриду с помощью стрелок
        private bool isAllowMoveByGrid = true;
        //признак нажатия на стрелку(любую) скролл бара
        private bool arrowClick;
        //включен ли постраничный режим работы
        private bool _isPaddingModeEnabled;
        //области грида, которые будут отрисованы
        private GridArea[] drawAreas = AreaSet.All;
        
        //Данные
        private CellSet _cls;
        private Data.PivotData _pivotData = null;

        //количество пикселей, на которое происходит смещение при скролировании колесиком мыши
        private int _mouseWheelScrollPixel = 50;
        //значение, но которое изменится коеф. увеличения таблицы при вращении колесика мыши
        private float _mouseWheelGridScale = 0.1f;

        //признак, что форма была только что активирована
        private bool _isActivated;
        //высота зазора между областями таблицы
        private int _separatorHeight = 3;
        //расположение таблицы
        private Point _gridLocation = new Point(2, 0);
        //снимок изображения грида
        private Image _gridShot;
        //контекстное меню
        private GridContextMenu contextMenu;

        //поля для обработки двойного щелчка
        private Timer doubleClickTimer = new Timer();
        private bool isFirstClick = true;
        private int milliseconds = 0;

        private Point selectedBoundsLocation = new Point(0, 0);
        //правила условной раскраски
        private ColorRuleCollection _colorRules;
        
        
        //масштаб таблицы
        private GridScale _gridScale;
        private bool canDrawGrid = true;

        #endregion

        public ExpertGrid(Data.PivotData pivotData)
        {
            this._gridScale = new GridScale(this);

            this.canDrawGrid = false;
            this.InitializeComponent();
            this.canDrawGrid = true;

            //создаем заголовки
            this.FilterCaptions = new FilterCaptions(this);
            this.ColumnCaptions = new ColumnCaptions(this);
            this.RowCaptions = new RowCaptions(this);
            this.MeasureCaptionsSections = new MeasureCaptionsSections(this);
            this.MeasuresData = new MeasuresData(this);
            //создаем оси
            this.Row = new RowAxis(this);
            this.Column = new ColumnAxis(this);
            //заглушка пустого места, если нет строк
            this.MeasuresStub = new MeasuresStub(this);

            this.SelectedCells = new SelectedCells(this);
            this.CellComments = new CellComments(this);
            this.LocationHelper = new LocationHelper(this);
            this.Splitter = new GridSplitter(this);
            this.SelectionFrame = new GridSelectionFrame(this);
            this.Painter = new ProfessionalPainter();
            this.VScrollBarState = new ScrollBarState(this, this.vScrollBar, null, Direction.BottomUp, 0);
            this.HScrollBarState = new ScrollBarState(this, this.hScrollBar, null, Direction.RightToLeft, 0);
            this.FiltersScrollBarState = new ScrollBarState(this, this.filtersScrollBar, null, Direction.RightToLeft, 0);

            //Экспортер грида
            this.Exporter = new GridExporter(this);

            this._pivotData = pivotData;
            //Установим здесь обработчик на изменение полей в PropertyGrid
            this.PivotData.AppearanceChanged += new PivotDataAppChangeEventHandler(PivotData_AppearanceChanged);

            //создаем контекстное меню
            this.contextMenu = new GridContextMenu(this);
            //таймер двойного щелчка мышью
            this.doubleClickTimer.Interval = 100;
            this.doubleClickTimer.Tick += new EventHandler(doubleClickTimer_Tick);

            this.ColorRules = new ColorRuleCollection(this);
        }


        /// <summary>
        /// Очистка всех коллекций грида
        /// </summary>
        public void ClearAll()
        {
            //перед очисткой у заголовков сохранаяются значения высоты и ширины, что бы после инициализации
            //применить эти настройки, так же сохраняются значения смещений по горизонтали(HScrollBarState) и 
            //вертикале(VScrollBarState)(в процентах)

            this.SelectedCells.Clear();
            this.FilterCaptions.Clear();
            this.ColumnCaptions.Clear();
            this.RowCaptions.Clear();

            this.Row.Clear();
            this.Column.Clear();

            this.MeasureCaptionsSections.Clear();
            this.MeasuresData.Clear();

            this.VScrollBarState.Clear();
            this.HScrollBarState.Clear();
            this.FiltersScrollBarState.Clear();

            this.CLS = null;

            this.LocationHelper.Clear();
        }

        /// <summary>
        /// Инициализируем оси и данные по CellSet, а заголовки по PivotData
        /// </summary>
        /// <param name="cls"></param>
        public void InitializeGrid(CellSet cls)
        {
            try
            {
                //Point selCellLocation = this.SelectedCells.GetCurrentCellLocation();
                this.ClearAll();

                this.CLS = cls;

                this.FilterCaptions.Initialize(this.PivotData);
                this.ColumnCaptions.Initialize(this.PivotData);
                this.RowCaptions.Initialize(this.PivotData);
                //инициализируем оси
                this.Row.InitializeMembers(cls);
                this.Column.InitializeMembers(cls);
                //инициализируем заголовки мер и область данных
                this.MeasureCaptionsSections.Initialize(this.Column.GetLeafList(), this.PivotData);
                this.MeasuresData.Initialize(this.Row.GetLeafList(), this.MeasureCaptionsSections.GetMeasuresCaptions(), cls);

                //после инициализации пересчитываем координаты
                this.RecalculateGrid();

                this.SelectedCells.Refresh();
            }
            catch (Exception e)
            {
                this.RecalculateGrid();
                FormException.ShowErrorForm(e);
            }
        }

        private void PivotData_AppearanceChanged(bool isNeedRecalculateGrid)
        {
            this.SynchronizePivotData(this.PivotData);
            
            if (isNeedRecalculateGrid)
                this.RecalculateGrid();
            else
                this.DrawGrid(AreaSet.All);
        }

        /// <summary>
        /// Синхронизация данных таблицы с PivotData
        /// </summary>
        /// <param name="pivotData"></param>
        private void SynchronizePivotData(Data.PivotData pivotData)
        {
            this.RowCaptions.SynchronizePivotData(pivotData);
            this.ColumnCaptions.SynchronizePivotData(pivotData);
            this.FilterCaptions.SynchronizePivotData(pivotData);
            this.MeasureCaptionsSections.SynchronizePivotData(pivotData);
        }

        /// <summary>
        /// Расчет всех абсолютных координат всех элементов грида, после этого рисуем грид
        /// </summary>
        public void RecalculateGrid()
        {
            this.GridGraphics = this.GetGridGraphics();
            Point tempPoint = this.GridLocation;

            //вычисляем начальну координату фильтров
            this.FilterCaptions.RecalculateCoordinates(tempPoint);

            //вычисляем начальную координату заголовков столбцов
            tempPoint.X = this.FilterCaptions.Location.X;
            tempPoint.Y = this.FilterCaptions.Height + this.FilterCaptions.Location.Y;
            //если есть заголовки фильтров, отступаем немного вниз, что бы была видна граница
            if (!this.FilterCaptions.IsEmpty && this.FilterCaptions.Visible)
                tempPoint.Y += this.SeparatorHeight;

            this.ColumnCaptions.RecalculateCoordinates(tempPoint);
            //ширина заголовков столбцов должна быть равна ширине заголовков строк
            if (!this.RowCaptions.IsEmpty)
                this.ColumnCaptions.Width = this.RowCaptions.OriginalWidth;

            //вычисляем начальную координату заголовков строк
            tempPoint.X = this.FilterCaptions.Location.X;
            tempPoint.Y = this.ColumnCaptions.Height + this.ColumnCaptions.Location.Y;
            //если есть заголовки столбцов, отступаем немного вниз, что бы была видна граница
            if (!this.ColumnCaptions.IsEmpty)
                tempPoint.Y += this.SeparatorHeight;
            this.RowCaptions.RecalculateCoordinates(tempPoint);

            //вычисляем начальную координату строк
            tempPoint.X = this.RowCaptions.Location.X;
            tempPoint.Y = this.RowCaptions.Height + this.RowCaptions.Location.Y;
            this.Row.RecalculateCoordinates(tempPoint);

            //вычисляем начальную координату заголовков показателей
            tempPoint.X = Math.Max(this.ColumnCaptions.Bounds.Right, this.RowCaptions.Bounds.Right);
            tempPoint.Y = this.ColumnCaptions.Bounds.Bottom;
            //если есть заголовки столбцов, отступаем немного вниз, что бы была видна граница
            if (!this.ColumnCaptions.IsEmpty)
                tempPoint.Y += this.SeparatorHeight;
            this.MeasureCaptionsSections.RecalculateCoordinates(tempPoint);
            //высота заголовков показателей должна быть равна высоте заголовков строк
            if (!this.RowCaptions.IsEmpty)
                this.MeasureCaptionsSections.Height = this.RowCaptions.OriginalHeight;

            //вычисляем начальную координату данных показателей
            tempPoint.X = this.MeasureCaptionsSections.Location.X;
            tempPoint.Y = this.MeasureCaptionsSections.Location.Y + this.GridScale.GetScaledValue(this.MeasureCaptionsSections.Height);
            this.MeasuresData.RecalculateCoordinates(tempPoint);

            //вычисляем начальную координату столбцов
            tempPoint.X = Math.Max(this.ColumnCaptions.Bounds.Right, this.RowCaptions.Bounds.Right);
            tempPoint.Y = this.ColumnCaptions.Location.Y;
            this.Column.RecalculateCoordinates(tempPoint);

            this.SetScrollPosition();

            //нарисуем, что расчитали
            this.DrawGrid(AreaSet.All);
            this.OnRecalculateGrid();
        }

        /// <summary>
        /// Инициализаций всех скролов в гриде
        /// </summary>
        public void SetScrollPosition()
        {
            if (!this.canDrawGrid)
                return;
            try
            {
                Rectangle tableRectangle = this.GridBounds;

                //скролл фильтров
                this.filtersScrollBar.Location = new Point(tableRectangle.Right - this.filtersScrollBar.Width + 1,
                                                           this.FilterCaptions.Location.Y);
                this.filtersScrollBar.Size = new Size(this.filtersScrollBar.Width, this.FilterCaptions.Height + 1);

                this.filtersScrollBar.Minimum = 0;
                this.filtersScrollBar.Maximum = this.FilterCaptions.Width;
                //Скролл будем отображать если ширина заголовков больше ширины таблиы, а так же если
                //не все заголовки находятся в зоне видимости
                this.filtersScrollBar.Visible = this.FilterCaptions.Visible && !this.FilterCaptions.IsEmpty
                                                &&
                                                ((this.FilterCaptions.Width >
                                                  tableRectangle.Width - this.filtersScrollBar.Width - 1)
                                                 || (this.FiltersScrollBarState.Offset != 0));

                //отключаем инерактивность скроллов, т.к. во время выставления некоторых свойств, срабатывает 
                //обработчик повешеный на изменение значения скролла, нам этого не надо...
                this.VScrollBarState.Changed = true;
                this.HScrollBarState.Changed = true;

                //вертикальный скролл
                this.vScrollBar.Location = new Point(tableRectangle.Right, this.Row.Location.Y);
                this.vScrollBar.Size = new Size(this.vScrollBar.Width, tableRectangle.Bottom - this.vScrollBar.Location.Y);

                this.vScrollBar.Enabled = (this.Row.Bounds.Bottom > tableRectangle.Bottom);
                this.vScrollBar.Minimum = 0;
                this.vScrollBar.Maximum = (this.vScrollBar.Enabled) ? this.Row.Height : 1;
                this.vScrollBar.LargeChange = Math.Max(tableRectangle.Height - this.Row.Location.Y, 1);

                //горизонтальный скролл
                this.hScrollBar.Location = new Point(Math.Max(this.RowCaptions.Bounds.Right, this.MeasureCaptionsSections.Bounds.Left), tableRectangle.Bottom);
                this.hScrollBar.Size = new Size(tableRectangle.Right - this.hScrollBar.Location.X, this.hScrollBar.Size.Height);

                this.hScrollBar.Enabled = (this.Column.Bounds.Right > tableRectangle.Right) || (this.MeasureCaptionsSections.Bounds.Right > tableRectangle.Right);
                this.hScrollBar.Minimum = 0;
                int hMaximum = Math.Max(this.Column.Width, this.MeasureCaptionsSections.Width);
                this.hScrollBar.Maximum = (this.hScrollBar.Enabled) ? hMaximum : 1;
                this.hScrollBar.LargeChange = Math.Max(tableRectangle.Width - Math.Max(this.Column.Location.X, this.MeasureCaptionsSections.Location.X), 1);

                //Если нету ячейки на которой фокусируемся, это значит что только что произошла инициализация
                //таблицы, то включим обработчик присваивания значения, что бы произошла инициализация той
                //самой ячейки
                this.VScrollBarState.Changed = this.VScrollBarState.FocusCell != null ? true : false;
                this.HScrollBarState.Changed = this.HScrollBarState.FocusCell != null ? true : false;
                //при изменини ширины строк, изменяется высота таблицы, при этом, если мы просматривали низ таблицы то 
                //фокус останется на прежнем месте, а таблица сожмется и ее не будет видно, поэтому всегда фокусируем 
                //таблицу на ранее запомненой ячейке
                this.vScrollBar.Value = this.VScrollBarState.GetValue();
                this.VScrollBarState.Offset = this.vScrollBar.Value;

                this.hScrollBar.Value = this.HScrollBarState.GetValue();
                this.HScrollBarState.Offset = this.hScrollBar.Value;
            }
            catch(Exception exc)
            {
                CommonUtils.ProcessException(exc);
            }
            finally
            {
                this.VScrollBarState.Changed = false;
                this.HScrollBarState.Changed = false;
            }
        }

        /// <summary>
        /// Загрузка свойств из щаблона, заточен под шаблон стилей инфрагистика
        /// </summary>
        /// <param name="fileName">Имя файла в котором содержатся cвойства</param>
        public void LoadPropertys(string fileName)
        {
            XmlDocument dom = new XmlDocument();
            try
            {
                dom.Load(fileName);
                XmlNode gridPropertys = dom.SelectSingleNode("styleLibrary//" + GridConsts.gridPropertys);
                this.LoadPropertys(gridPropertys, true);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref dom);
            }
        }

        /// <summary>
        /// Загрузка свойств всех элементов листа из Xml
        /// </summary>
        public void LoadPropertys(XmlDocument dom)
        {
            if (dom == null)
                return;
            try
            {
                XmlNode gridPropertys = dom.SelectSingleNode(GridConsts.gridPropertys);
                this.LoadPropertys(gridPropertys, false);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref dom);
            }
        }

        /// <summary>
        /// Загрузка свойств для всех элементов грида из узла Xml
        /// </summary>
        /// <param name="propertysNode">узел со свойствами</param>
        public void LoadPropertys(XmlNode propertysNode, bool isLoadTemplate)
        {
            if (propertysNode == null)
                return;
            this.GridScale.Load(propertysNode.SelectSingleNode(GridConsts.gridScale));

            this.LoadAddinPropertys(propertysNode.SelectSingleNode(GridConsts.addinPropertys));
            this.CellComments.Load(propertysNode.SelectSingleNode(GridConsts.comments), isLoadTemplate);
            this.Painter.Load(propertysNode.SelectSingleNode(GridConsts.painter), isLoadTemplate);

            this.FilterCaptions.Load(propertysNode.SelectSingleNode(GridConsts.filtersCaptions), isLoadTemplate);
            this.ColumnCaptions.Load(propertysNode.SelectSingleNode(GridConsts.columnsCaptions), isLoadTemplate);
            this.RowCaptions.Load(propertysNode.SelectSingleNode(GridConsts.rowsCaptions), isLoadTemplate);

            this.Row.Load(propertysNode.SelectSingleNode(GridConsts.rows), isLoadTemplate);
            this.Column.Load(propertysNode.SelectSingleNode(GridConsts.columns), isLoadTemplate);

            this.MeasureCaptionsSections.Load(propertysNode.SelectSingleNode(GridConsts.measuresCaptions), isLoadTemplate);
            this.MeasuresData.Load(propertysNode.SelectSingleNode(GridConsts.measuresData), isLoadTemplate);

            this.ColorRules.Load(propertysNode.SelectSingleNode(GridConsts.colorRules), isLoadTemplate);

            this.SelectedCells.Load(propertysNode.SelectSingleNode(GridConsts.selectedCells), isLoadTemplate);

            //Если загружаем из шаблона, пересчета не требуется, т.к. следующий шаг будет загрузка свойств 
            //из отчета, вот тогда и посчитаем
            if (isLoadTemplate)
            {
                //Отрисуем таблицу уже с новым стилем
                this.DrawGrid(AreaSet.All);
            }
            else
            {   
                //После загрузки свойств пересчитаем координаты элементов
                this.RecalculateGrid();
            }

           
        }

        /// <summary>
        /// Загрузка дополнительных свойств таблицы
        /// </summary>
        /// <param name="addinPropertys">узел с дополнительными свойствами</param>
        private void LoadAddinPropertys(XmlNode addinPropertys)
        {
            if (addinPropertys == null)
                return;
            this._separatorHeight = XmlHelper.GetIntAttrValue(addinPropertys, GridConsts.separatorHeight, 3);
        }

        /// <summary>
        /// Сохраняет все свойства грида в указанный документ
        /// </summary>
        /// <param name="fileName"></param>
        public void SavePropertys(string fileName)
        {
            XmlDocument dom = this.SavePropertys();
            if (dom == null)
                return;
            dom.Save(fileName);
            XmlHelper.ClearDomDocument(ref dom);
        }

        /// <summary>
        /// Вернет документ с настройками грида
        /// </summary>
        /// <returns></returns>
        public XmlDocument SavePropertys()
        {
            XmlDocument dom = new XmlDocument();
            XmlNode gridPropertys = dom.CreateElement(GridConsts.gridPropertys);
            dom.AppendChild(gridPropertys);
            this.SavePropertys(gridPropertys);
            return dom;
        }

        /// <summary>
        /// Сохранение свойств всех элементов грида в Xml
        /// </summary>
        /// <param name="propertysNode"></param>
        public void SavePropertys(XmlNode propertysNode)
        {
            if (propertysNode == null)
                return;

            this.SaveAddinPropertys(XmlHelper.AddChildNode(propertysNode, GridConsts.addinPropertys));
            this.CellComments.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.comments));
            this.Painter.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.painter));

            this.FilterCaptions.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.filtersCaptions));
            this.ColumnCaptions.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.columnsCaptions));
            this.RowCaptions.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.rowsCaptions));

            this.Row.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.rows));
            this.Column.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.columns));

            this.MeasureCaptionsSections.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.measuresCaptions));
            this.MeasuresData.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.measuresData));

            this.ColorRules.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.colorRules));
            this.GridScale.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.gridScale));

            this.SelectedCells.Save(XmlHelper.AddChildNode(propertysNode, GridConsts.selectedCells));
        }

        /// <summary>
        /// Сохраняет дополнительные свойства таблицы
        /// </summary>
        /// <param name="addinPropertys"></param>
        private void SaveAddinPropertys(XmlNode addinPropertys)
        {
            XmlHelper.SetAttribute(addinPropertys, GridConsts.separatorHeight, 
                this.SeparatorHeight.ToString());
        }

        /// <summary>
        /// метод надо вызывать для перерисовки грида, в нем определяется регион собственно который и будет
        /// перерисован
        /// </summary>
        /// <param name="drawAreas">Области грида, требующие перерисовки</param>
        public void DrawGrid(GridArea[] drawAreas)
        {
            if (!this.canDrawGrid)
                return;
            try
            {
                //будем перерисовывать области перечисленные в drawAreas
                this.drawAreas = drawAreas;

                //вычисляем регион в котором распологается наша таблица
                Size size = this.gridPlace.Size;
                size.Height -= this.GridLocation.Y;
                size.Width -= this.GridLocation.X;
                Rectangle regionBounds = new Rectangle(this.GridLocation, size);

                //предпологаем что требуется перерисовать всю поверхность таблицы
                Region region = new Region(regionBounds);

                //исключаем те области, которые перерисовывать не надо
                bool isExcludeFiltersCaption = !this.FilterCaptions.IsEmpty;
                bool isExcludeColumnsCaption = !this.ColumnCaptions.IsEmpty;
                bool isExcludeRowsCaption = !this.RowCaptions.IsEmpty;
                bool isExcludeMeasuresCaption = !this.MeasureCaptionsSections.IsEmpty;
                bool isExcludeMeasuresData = !this.MeasuresData.IsEmpty;
                bool isExcludeRows = !this.Row.IsEmpty;
                bool isExcludeColumns = !this.Column.IsEmpty;

                foreach (GridArea gridArea in this.drawAreas)
                {
                    switch (gridArea)
                    {
                        case GridArea.FiltersCaption:
                            {
                                isExcludeFiltersCaption = false;
                                break;
                            }
                        case GridArea.ColumnsCaption:
                            {
                                isExcludeColumnsCaption = false;
                                break;
                            }
                        case GridArea.Columns:
                            {
                                isExcludeColumns = false;
                                break;
                            }
                        case GridArea.MeasuresCaption:
                            {
                                isExcludeMeasuresCaption = false;
                                break;
                            }
                        case GridArea.MeasuresData:
                            {
                                isExcludeMeasuresData = false;
                                break;
                            }
                        case GridArea.RowsCaption:
                            {
                                isExcludeRowsCaption = false;
                                break;
                            }
                        case GridArea.Rows:
                            {
                                isExcludeRows = false;
                                break;
                            }
                    }
                }

                //исключаем из поверхности контрола, области которые перерисовывать не надо.
                if (isExcludeFiltersCaption)
                    region.Exclude(this.FilterCaptions.GetVisibleBounds());
                if (isExcludeColumnsCaption)
                    region.Exclude(this.ColumnCaptions.GetVisibleBounds());
                if (isExcludeColumns)
                    region.Exclude(this.Column.GetVisibleBounds());
                if (isExcludeMeasuresCaption)
                    region.Exclude(this.MeasureCaptionsSections.GetVisibleBounds());
                if (isExcludeMeasuresData)
                    region.Exclude(this.MeasuresData.GetVisibleBounds());
                if (isExcludeRowsCaption)
                    region.Exclude(this.RowCaptions.GetVisibleBounds());
                if (isExcludeRows)
                    region.Exclude(this.Row.GetVisibleBounds());

                this.gridPlace.Invalidate(region);

            }
            catch(Exception exc)
            {
                CommonUtils.ProcessException(exc);
            }
        }

        /// <summary>
        /// Метод вызывается из обработчика OnPaint, он все и рисует
        /// </summary>
        private void DoDrawGrid()
        {
            //если только что активировали форму, то перерисовывать надо все
            if (this.IsActivated)
            {
                this.drawAreas = AreaSet.All;
                this.IsActivated = false;
            }

            foreach (GridArea gridArea in this.drawAreas)
            {
                switch (gridArea)
                {
                    case GridArea.FiltersCaption:
                        {
                            if (this.FilterCaptions.Visible)
                                this.FilterCaptions.Draw(this.GridGraphics, this.Painter);
                            break;
                        }
                    case GridArea.ColumnsCaption:
                        {
                            this.ColumnCaptions.Draw(this.GridGraphics, this.Painter);
                            break;
                        }
                    case GridArea.Columns:
                        {
                            this.Column.DrawMembers(this.GridGraphics, this.Painter);
                            break;
                        }
                    case GridArea.MeasuresCaption:
                        {
                            this.MeasureCaptionsSections.Draw(this.GridGraphics, this.Painter);
                            break;
                        }
                    case GridArea.MeasuresData:
                        {
                            this.MeasuresData.Draw(this.GridGraphics, this.Painter);
                            break;
                        }
                    case GridArea.RowsCaption:
                        {
                            this.RowCaptions.Draw(this.GridGraphics, this.Painter);
                            break;
                        }
                    case GridArea.Rows:
                        {
                            this.Row.DrawMembers(this.GridGraphics, this.Painter);
                            break;
                        }
                }
            }

            this.MeasuresStub.Draw(this.GridGraphics, this.Painter);
            this.drawAreas = AreaSet.All;
        }

        /// <summary>
        /// Скролироване по вертикале
        /// </summary>
        internal void ScrollByVertical(int scrollBarValue, bool isArrowClick)
        {
            //скоролируем по высоте ячейки
            if (this.VScrollBarState.Changed)
                return;
            try
            {
                if (this.VScrollBarState.Value == scrollBarValue)
                    return;
                this.VScrollBarState.Changed = true;
                int oldScrollBarValue = this.VScrollBarState.Value;
                Rectangle rowsBounds = this.Row.Bounds;
                //вычисляем координаты ячейки по высоте которой будем скролировать
                Point cellPosition = new Point(rowsBounds.Right - 5, rowsBounds.Top + scrollBarValue);
                DimensionCell cell = this.Row.FindCell(cellPosition, false);
                Direction direction = (this.VScrollBarState.Value < scrollBarValue ? Direction.TopDown : Direction.BottomUp);
                this.VScrollBarState.Value = scrollBarValue;
                int newScrollBarValue = scrollBarValue;

                if (cell != null)
                {
                    if (!isArrowClick)
                    {
                        //если ячейка на которой фокусируемся и направление движения не изменились, то выходим...
                        if (cell.Equals(this.VScrollBarState.FocusCell) &&
                            (this.VScrollBarState.ScrollDirection == direction))
                            return;
                    }

                    //вычисляем новое значение скролл бара
                    if (direction == Direction.TopDown)
                        //сверху вниз
                        newScrollBarValue = cell.Bounds.Bottom - rowsBounds.Top;
                    else
                        //снизу вверх
                        newScrollBarValue = cell.Bounds.Top - rowsBounds.Top;
                }
                if (isArrowClick)
                {
                    this.VScrollBarState.Value = newScrollBarValue;
                    this.BlendByScrolling(newScrollBarValue, oldScrollBarValue, direction);
                }
                this.VScrollBarState.FocusCell = cell;
                this.VScrollBarState.ScrollDirection = direction;
                this.vScrollBar.Value = this.VScrollBarState.GetSafeValue(newScrollBarValue);
                this.VScrollBarState.Offset = Math.Min(this.vScrollBar.Maximum - 1, this.vScrollBar.Value);

                this.DrawGrid(new GridArea[] { GridArea.Rows, GridArea.MeasuresData });
            }
            finally
            {
                this.VScrollBarState.Changed = false;
            }

        }

        /// <summary>
        /// Скролированеие по горизонтале
        /// </summary>
        internal void ScrollByHorizontal(int scrollBarValue, bool isArrowClick)
        {
            //скоролируем по ширене ячейки
            if (this.HScrollBarState.Changed)
                return;
            try
            {
                if (this.HScrollBarState.Value == scrollBarValue)
                    return;
                this.HScrollBarState.Changed = true;
                int oldScrollBarValue = this.HScrollBarState.Value;
                Rectangle bounds = this.MeasureCaptionsSections.IsEmpty ? this.Column.Bounds : this.MeasureCaptionsSections.Bounds;
                Point cellPosition = new Point(bounds.Left + scrollBarValue, bounds.Bottom - 5);
                GridCell cell;
                if (this.MeasureCaptionsSections.IsEmpty)
                    //если нет показателей, скролируем по ширине ячейке принадлежащей колонке
                    cell = this.Column.FindCell(cellPosition, false);
                else
                    cell = this.MeasureCaptionsSections.FindCaption(cellPosition, false);
                Direction direction = (this.HScrollBarState.Value < scrollBarValue ? Direction.LeftToRight : Direction.RightToLeft);
                this.HScrollBarState.Value = scrollBarValue;
                int newScrollBarValue = scrollBarValue;

                if (cell != null)
                {
                    if (!isArrowClick)
                    {
                        //если ячейка на которой фокусируемся и направление движения не изменились, то выходим...
                        if (cell.Equals(this.HScrollBarState.FocusCell) &&
                            (this.HScrollBarState.ScrollDirection == direction))
                            return;
                    }

                    if (direction == Direction.LeftToRight)
                        //слева направо
                        newScrollBarValue = cell.Bounds.Right - bounds.Left;
                    else
                        //справа налево
                        newScrollBarValue = cell.Bounds.Left - bounds.Left;
                }
                if (isArrowClick)
                {
                    this.HScrollBarState.Value = newScrollBarValue;
                    this.BlendByScrolling(newScrollBarValue, oldScrollBarValue, direction);
                }
                this.HScrollBarState.FocusCell = cell;
                this.HScrollBarState.ScrollDirection = direction;
                this.hScrollBar.Value = this.HScrollBarState.GetSafeValue(newScrollBarValue);
                this.HScrollBarState.Offset = Math.Min(this.hScrollBar.Maximum - 1, this.hScrollBar.Value);
                
                //учитывая текущее смещение, перерисовываем ось столбцов, и показатели
                this.DrawGrid(new GridArea[] { GridArea.Columns, GridArea.MeasuresCaption, GridArea.MeasuresData });
            }
            finally
            {
                this.HScrollBarState.Changed = false;
            }
        }

        /// <summary>
        /// Плавный переход при скролировании
        /// </summary>
        /// <param name="currentOffset">Текущее смещение скролла</param>
        /// <param name="oldOffset">Предыдущее смещение скролла</param>
        /// <param name="direction">Направления скролирования</param>
        private void BlendByScrolling(int currentOffset, int oldOffset, Direction direction)
        {
            //приблизительное количество пикселей в кадре
            const int approximatePixelCount = 25;
            //вычисляем количество кадров, требующихся для плавной отрисовки (стоит ограничение, не больше 10)
            int frameCount = Math.Min(Math.Abs(currentOffset - oldOffset) / approximatePixelCount, 15);
            if (frameCount == 0)
                return;

            //точное количество пикселей в одном кадре
            int pixelCount = Math.Abs(currentOffset - oldOffset) / frameCount;
            switch (direction)
            {
                case Direction.TopDown:
                    {
                        for (int i = frameCount - 1; i > 0; i--)
                        {
                            this.VScrollBarState.Offset = this.VScrollBarState.GetSafeValue(currentOffset - (pixelCount * i));
                            this.DrawGrid(new GridArea[] { GridArea.Rows, GridArea.MeasuresData });
                            Application.DoEvents();
                        }
                        this.VScrollBarState.Offset = currentOffset;
                        break;
                    }
                case Direction.BottomUp:
                    {
                        for (int i = frameCount - 1; i > 0; i--)
                        {
                            this.VScrollBarState.Offset = this.VScrollBarState.GetSafeValue(currentOffset + (pixelCount * i));
                            this.DrawGrid(new GridArea[] { GridArea.Rows, GridArea.MeasuresData });
                            Application.DoEvents();
                        }
                        this.VScrollBarState.Offset = currentOffset;
                        break;
                    }
                case Direction.LeftToRight:
                    {
                        for (int i = frameCount - 1; i > 0; i--)
                        {
                            this.HScrollBarState.Offset = this.HScrollBarState.GetSafeValue(currentOffset - (pixelCount * i));
                            this.DrawGrid(new GridArea[] { GridArea.Columns, GridArea.MeasuresCaption, GridArea.MeasuresData });
                            Application.DoEvents();
                        }
                        this.HScrollBarState.Offset = currentOffset;
                        break;
                    }
                case Direction.RightToLeft:
                    {
                        for (int i = frameCount - 1; i > 0; i--)
                        {
                            this.HScrollBarState.Offset = this.HScrollBarState.GetSafeValue(currentOffset + (pixelCount * i));
                            this.DrawGrid(new GridArea[] { GridArea.Columns, GridArea.MeasuresCaption, GridArea.MeasuresData });
                            Application.DoEvents();
                        }
                        this.HScrollBarState.Offset = currentOffset;
                        break;
                    }
            }
        }

        /// <summary>
        /// Скролирование фильтров
        /// </summary>
        internal void ScrollFilters(int scrollBarValue)
        {
            //скоролируем по ширене ячейки
            if (this.FiltersScrollBarState.Changed)
                return;
            try
            {
                if (this.FiltersScrollBarState.Value == scrollBarValue)
                    return;
                this.FiltersScrollBarState.Changed = true;
                Rectangle filtersBounds = this.FilterCaptions.Bounds;
                Point captionPosition = new Point(filtersBounds.Left + scrollBarValue, filtersBounds.Top + 5);
                CaptionCell caption = this.FilterCaptions.FindCaption(captionPosition, false);
                Direction direction = (this.FiltersScrollBarState.Value < scrollBarValue ? Direction.LeftToRight : Direction.RightToLeft);
                int newScrollBarValue = scrollBarValue;

                if (caption != null)
                {
                    if (direction == Direction.LeftToRight)
                        //слева направо
                        newScrollBarValue = caption.Bounds.Right - filtersBounds.Left;
                    else
                        //справа налево
                        newScrollBarValue = caption.Bounds.Left - filtersBounds.Left;
                }
                this.FiltersScrollBarState.Value = newScrollBarValue;
                this.FiltersScrollBarState.FocusCell = caption;
                this.FiltersScrollBarState.ScrollDirection = direction;
                this.filtersScrollBar.Value = newScrollBarValue;
                this.FiltersScrollBarState.Offset = Math.Min(this.filtersScrollBar.Maximum - 1, this.filtersScrollBar.Value);

                this.DrawGrid(new GridArea[] { GridArea.FiltersCaption });
                this.SetScrollPosition();
            }
            finally
            {
                this.FiltersScrollBarState.Changed = false;
            }
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((e.Type == ScrollEventType.SmallDecrement) || (e.Type == ScrollEventType.SmallIncrement))
                this.arrowClick = true;
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((e.Type == ScrollEventType.SmallDecrement) || (e.Type == ScrollEventType.SmallIncrement))
                this.arrowClick = true;
        }

        private void filtersScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((e.Type == ScrollEventType.SmallDecrement) || (e.Type == ScrollEventType.SmallIncrement))
                this.arrowClick = true;
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.ScrollByVertical(this.vScrollBar.Value, this.arrowClick);
            this.arrowClick = false;
            if (this.SelectionFrame.IsDrag)
            {
                //this.GridShot = GetGridShot();
            }

        }

        private void hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.ScrollByHorizontal(this.hScrollBar.Value, this.arrowClick);
            this.arrowClick = false;
        }

        private void filtersScrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.ScrollFilters(this.filtersScrollBar.Value);
            this.arrowClick = false;
        }

        #region Обработчики событий панели на которой лежит грид

        private void gridPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            this.CellComments.CleanComment(true);
            this.SelectedCells.PreviewKeyDown(e);
        }

        private void gridPlace_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                this.GridGraphics = e.Graphics;
                this.GridGraphics.SmoothingMode = SmoothingMode.HighSpeed;
                this.DoDrawGrid();
                this.GridGraphics = null;
            }
            catch (Exception exc)
            {
                FormException.ShowErrorForm(exc);
            }
        }

        private float scaleValue = 1;

        void gridPlace_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.CellComments.CleanComment(true);

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                float amount = e.Delta > 0 ? MouseWheelGridScale : -MouseWheelGridScale;

                
                this.GridScale.Value += amount;
                if (this.GridScale.Value > 3)
                {
                    this.GridScale.Value = 3;
                    return;
                }
                if (this.GridScale.Value < 0.3)
                {
                    this.GridScale.Value = 0.3f;
                    return;
                }
                

                this.RecalculateGrid();
            }
            else
            {
                int amount = e.Delta > 0 ? MouseWheelScrollPixel : -MouseWheelScrollPixel;
                if ((this.vScrollBar.Value - amount) < this.vScrollBar.Minimum)
                    this.vScrollBar.Value = this.vScrollBar.Minimum;
                else if ((this.vScrollBar.Value - amount + this.vScrollBar.LargeChange) > this.vScrollBar.Maximum)
                    this.vScrollBar.Value = Math.Max(this.vScrollBar.Maximum - this.vScrollBar.LargeChange,
                                                     this.vScrollBar.Minimum);
                else
                    this.vScrollBar.Value -= amount;
            }

            if (this.SelectionFrame.IsDrag)
            {
                this.Refresh();
                this.GridShot = GetGridShot();
            }

        }

        private void gridPlace_MouseClick(object sender, MouseEventArgs e)
        {
            if (!this.Splitter.IsDrag)
            {
                this.SelectedCells.MouseClick(e);
            }
        }

        private void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            this.milliseconds += 100;

            if (milliseconds >= SystemInformation.DoubleClickTime)
            {
                this.doubleClickTimer.Stop();
                this.isFirstClick = true;
                this.milliseconds = 0;
            }

        }

        private void gridPlace_Resize(object sender, EventArgs e)
        {
            this.DrawGrid(AreaSet.All);
        }

        private void gridPlace_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.gridPlace.Focused)
            {
                this.gridPlace.Focus();
            }
            this.selectedBoundsLocation = e.Location;
            this.VScrollBarState.BeginOffset = this.VScrollBarState.Offset;

            //Событие MouseDoubleClick не всегда успевает срабатывать, поетому делаем так...
            if (this.isFirstClick)
            {
                this.isFirstClick = false;
                this.doubleClickTimer.Start();
            }
            else
            {
                if (this.milliseconds < SystemInformation.DoubleClickTime)
                {
                    if (this.Splitter.DoubleClick(e.Location))
                        return;
                }
            }

            this.CellComments.CleanComment(true);
            if (this.Splitter.StartDrag(e.Location))
            {
                //получаем снимок таблицы
                this.GridShot = this.GetGridShot();
                //ограничиваем область перемещения курсора, чтобы при изменении границ контролов не вылезал за 
                //пределы грида
                this.ClipCursor();
                //если что-то зацепили то выходим
                return;
            }
            else
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    /*Выделение рамкой начинаем не при нажатии, а когда сдвинули мышку при нажатой кнопке,
                     * поэтому перенес этот код в mouseMove

                    this.SelectionFrame.StartDrag(e.Location);
                    this.SelectedCells.ClearSelection();

                    //получаем снимок таблицы
                    this.GridShot = this.GetGridShot();
                    //ограничиваем область перемещения курсора
                    this.ClipCursor();
                     */
                }
            }

            if ((e.Button == System.Windows.Forms.MouseButtons.Left) || (this.SelectedCells.CurrentCells.Count <= 1))
            {
                this.SelectedCells.MouseDown(e);
            }
        }

        private void gridPlace_MouseMove(object sender, MouseEventArgs e)
        {
            this.CellComments.CleanComment(!this.CellComments.DisplayUntillControlChange);

            if (this.Splitter.IsDrag)
            {
                this.Splitter.Move(e.Location);
            }
            else
            if (this.SelectionFrame.IsDrag)
            {
                this.SelectionFrame.Move(e.Location);
            }
            else
            if (!(this.SelectionFrame.IsDrag)&&(e.Button == System.Windows.Forms.MouseButtons.Left))
            {
                int width = Math.Abs(this.selectedBoundsLocation.X - e.X);
                int height = Math.Abs(this.selectedBoundsLocation.Y - e.Y);
                bool isCanSelection = ((width != 0) && (height != 0));
                if (isCanSelection)
                {
                    this.SelectionFrame.StartDrag(e.Location);
                    this.SelectedCells.ClearSelection();
                    //получаем снимок таблицы
                    this.GridShot = this.GetGridShot();
                    //ограничиваем область перемещения курсора
                    this.ClipCursor();

                }
            }
            else
            {
                //Ищем выделенные области, и объекты грида. Идея такая, что ищем все необходимое один раз, а в
                //других местах испульзуем уже результаты
                this.LocationHelper.Initialize(e.Location, true);
                //Начало отсчета, до показа комментария
                this.CellComments.StartTimer();
                this.Cursor = this.SelectedCells.MouseMove(e);
            }
        }

        private void gridPlace_MouseUp(object sender, MouseEventArgs e)
        {
            //снимаем ограничение на перемещение курсора
            Cursor.Clip = Rectangle.Empty;
            if (this.Splitter.IsDrag)
            {
                this.Splitter.EndDrag();
                //после окончания перетаскиваня объекта, смотрим может опять надо что-то таскать...
                Application.DoEvents();
                //что то перетащили, значит изменили размер, сообщаем об этом
                this.OnGridSizeChanged();
                this.Cursor = this.SelectedCells.MouseMove(e);
                return;
            }
            else
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                int top = (this.selectedBoundsLocation.X <= e.X) ? this.selectedBoundsLocation.X : e.X;
                int left = (this.selectedBoundsLocation.Y <= e.Y) ? this.selectedBoundsLocation.Y : e.Y;
                int width = Math.Abs(this.selectedBoundsLocation.X - e.X);
                int height = Math.Abs(this.selectedBoundsLocation.Y - e.Y);
                bool isMultiSelect = ((width != 0) && (height != 0));
                
                if (isMultiSelect)
                {
                    this.SelectedCells.SelectRegion(new Rectangle(top, left, width, height));
                }
                
                if (this.SelectionFrame.IsDrag)
                    this.SelectionFrame.EndDrag(isMultiSelect);
                

            }

            
            //показываем контекстное меню
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.CellComments.CleanComment(true);
                if (this.SelectedCells.CurrentCells.Count > 1)
                {
                    this.contextMenu.Show(null, this.PointToScreen(e.Location), true);
                }
                else
                {
                    this.SelectedCells.MouseDown(e);
                    this.contextMenu.Show(this.SelectedCells.CurrentCell, this.PointToScreen(e.Location), false);
                }
            }
        }

        private void gridPlace_SizeChanged(object sender, EventArgs e)
        {
            this.SetScrollPosition();
        }
        #endregion

        //ограничить область перемещения курсора
        private void ClipCursor()
        {
            Rectangle cursorClip = this.GridBounds;
            cursorClip.Width -= 1;
            cursorClip.Height -= 1;
            Cursor.Clip = this.gridPlace.RectangleToScreen(cursorClip);
        }

        /// <summary>
        /// Получить снимок таблицы
        /// </summary>
        /// <returns></returns>
        private Image GetGridShot()
        {
            Image result = null;
            using (Graphics graphics = this.GetGridGraphics())
            {
                result = CommonUtils.GetBitMap(this.gridPlace.RectangleToScreen(Rectangle.Truncate(graphics.VisibleClipBounds)));
            }
            return result;
        }

        /// <summary>
        /// Получаем Graphics, данным методом получаем graphics только в случае, когда НЕ требуется полная 
        /// перерисовка грида
        /// </summary>
        /// <returns>Graphics</returns>
        public Graphics GetGridGraphics()
        {
            return Graphics.FromHwnd(this.gridPlace.Handle);
        }

        public void OnRecalculateGrid()
        {
            if (_recalculatedGrid != null)
                _recalculatedGrid(this, new EventArgs());
        }
        
        public void OnGridSizeChanged()
        {
            if (_gridSizeChanged != null)
                _gridSizeChanged(this, new EventArgs());
        }

        public void OnColorRulesChanged()
        {
            if (_colorRulesChanged != null)
                _colorRulesChanged(this, new EventArgs());
        }

        public void OnScaleChanged()
        {
            if (_scaleChanged != null)
                _scaleChanged(this, new EventArgs());
        }

        public void OnSortClick(string uniqueName, SortType sortType)
        {
            this.OnSortClick(uniqueName, sortType, string.Empty);
        }

        public void OnSortClick(string uniqueName, SortType sortType, string sortedTupleUN)
        {
            if (_sortClick != null)
                _sortClick(this, uniqueName, sortType, sortedTupleUN);
        }

        public void OnDropButtonClick(string hierarchyUN)
        {
            if (_dropButtonClick != null)
                _dropButtonClick(this, hierarchyUN);
        }

        public void OnObjectSelected(SelectionType selectionType, string objectUN)
        {
            if (_objectSelected != null)
                _objectSelected(selectionType, objectUN);
        }

        public void OnDrillThrough(string measureUN, string rowCellUN, bool rowCellIsTotal,
            string columnCellUN, bool columnCellIsTotal, string actionName)
        {
            if (_drillThrough != null)
                _drillThrough(measureUN, rowCellUN, rowCellIsTotal, columnCellUN, columnCellIsTotal, actionName);
        }

        public void OnExpandedMember(string dimemsionUN, string levelUN, bool state)
        {
            if (_expandedMember != null)
                _expandedMember(dimemsionUN, levelUN, state);
        }

        # region Свойства

        /// <summary>
        /// Видимая часть грида, с учетом начальной позиции, и завычетом скроллов
        /// </summary>
        public Rectangle GridBounds
        {
            get
            {
                Point point = this.GridLocation;
                Size size = this.gridPlace.Size;
                size.Width -= this.vScrollBar.Width + point.X;
                size.Height -= this.hScrollBar.Height + point.Y;
                return new Rectangle(point, size);
            }
        }

        /// <summary>
        /// Коллекция строк
        /// </summary>
        public RowAxis Row
        {
            get { return _rows;  }
            set { _rows = value; }
        }

        /// <summary>
        /// Коллекция колонок
        /// </summary>
        public ColumnAxis Column
        {
            get { return _columns; }
            set { _columns = value; }
        }

        /// <summary>
        /// Коллекция заголовков строк
        /// </summary>
        public RowCaptions RowCaptions
        {
            get { return _rowsCaptions; }
            set { _rowsCaptions = value;}
        }

        /// <summary>
        /// Коллекция заголовков колонок
        /// </summary>
        public ColumnCaptions ColumnCaptions
        {
            get { return _columnsCaptions; }
            set { _columnsCaptions = value; }
        }

        /// <summary>
        /// Коллекция заголовков фильтров
        /// </summary>
        public FilterCaptions FilterCaptions
        {
            get { return _filtersCaptions; }
            set { _filtersCaptions = value; }
        }

        /// <summary>
        /// Коллекция заголовков показателей
        /// </summary>
        public MeasureCaptionsSections MeasureCaptionsSections
        {
            get { return _measuresCaptionsSections; }
            set { _measuresCaptionsSections = value; }
        }

        /// <summary>
        /// Коллекция данных показателей
        /// </summary>
        public MeasuresData MeasuresData
        {
            get { return _measuresData;}
            set { _measuresData = value; }
        }

        /// <summary>
        /// Заполняет свободное место возникающие в случае присутствия в таблице колонок и показателей, 
        /// и отсутствии строк
        /// </summary>
        public MeasuresStub MeasuresStub
        {
            get { return _measuresStub; }
            set { _measuresStub = value; }
        }

        /// <summary>
        /// Отрисовщик грида
        /// </summary>
        public ProfessionalPainter Painter
        {
            get { return _painter; }
            set { _painter = value; }
        }

        /// <summary>
        /// Графической средство
        /// </summary>
        public Graphics GridGraphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        /// <summary>
        /// Получаем координаты мыши, относительно грида
        /// </summary>
        public Point CurrentMousePosition
        {
            get { return this.gridPlace.PointToClient(Control.MousePosition); }
        }

        /// <summary>
        /// Источник данных, при установке происходит инициализация и пересчет координат всех коллекций
        /// </summary>
        public CellSet CLS
        {
            get { return _cls; }
            set { _cls = value; }
        }

        public Data.PivotData PivotData
        {
            get { return _pivotData; }
        }

        /// <summary>
        /// Количестово пикселей на которое скролируется вертикальный скролл, при прокрутке колесиком мыши
        /// </summary>
        public int MouseWheelScrollPixel
        {
            get { return _mouseWheelScrollPixel; }
            set { _mouseWheelScrollPixel = value; }
        }

        /// <summary>
        /// Состояние горизонтального скролл бара
        /// </summary>
        public ScrollBarState HScrollBarState
        {
            get { return _hScrollBarState; }
            set { _hScrollBarState = value; }
        }

        /// <summary>
        /// Состояние скролл бара фильтров
        /// </summary>
        public ScrollBarState FiltersScrollBarState
        {
            get { return _filtersScrollBarState; }
            set { _filtersScrollBarState = value; }
        }

        /// <summary>
        /// Состояние вертикального скролл бара
        /// </summary>
        public ScrollBarState VScrollBarState
        {
            get { return _vScrollBarState; }
            set { _vScrollBarState = value; }
        }
        
        /// <summary>
        /// Вертикальный сколл бар
        /// </summary>
        public Infragistics.Win.UltraWinScrollBar.UltraScrollBar VScrollBar
        {
            get { return this.vScrollBar; }
        }

        /// <summary>
        /// Горизонтальный скролл бар
        /// </summary>
        public Infragistics.Win.UltraWinScrollBar.UltraScrollBar HScrollBar
        {
            get { return this.hScrollBar; }
        }

        /// <summary>
        /// Снимок таблицы
        /// </summary>
        public Image GridShot
        {
            get { return _gridShot; }
            set { _gridShot = value; }
        }

        /// <summary>
        /// Вспомогательный класс, в котором будет информация о текущих выделенных областях и объектах грида, 
        /// а так же методы для их поиска. Идея такая, что ищем все необходимое один раз, а в
        ///других местах испульзуем уже результаты
        /// </summary>
        internal LocationHelper LocationHelper
        {
            get { return _locationHelper; }
            set { _locationHelper = value; }
        }

        /// <summary>
        /// Комментарии к ячейкам, отображает и скрывает их
        /// </summary>
        internal CellComments CellComments
        {
            get { return _cellsComments; }
            set { _cellsComments = value; }
        }

        public GridSplitter Splitter
        {
            get { return _splitter; }
            set { _splitter = value; }
        }

        public GridSelectionFrame SelectionFrame
        {
            get { return _selectionFrame; }
            set { _selectionFrame = value; }
        }

        public SelectedCells SelectedCells
        {
            get { return _selectedCells; }
            set { _selectedCells = value; }
        }

        /// <summary>
        /// Признак, что форма была только что активирована
        /// </summary>
        public bool IsActivated
        {
            get { return _isActivated; }
            set { _isActivated = value; }
        }

        /// <summary>
        /// Включен ил постраничный режим
        /// </summary>
        public bool IsPaddingModeEnabled
        {
            get { return _isPaddingModeEnabled; }
            set { _isPaddingModeEnabled = value; }
        }

        /// <summary>
        /// Монитор, отчечающий за возможность передвегаться по гриду с помощью стрелок (т.к. clr 
        /// оптимизирует вызов процедур, может возникнуть ситуация, что еще не полностью обработано 
        /// предыдущее нажатие, как начинается новое)
        /// </summary>
        public bool IsAllowMoveByGrid
        {
            get { return isAllowMoveByGrid; }
            set { isAllowMoveByGrid = value; }
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        public GridExporter Exporter
        {
            get { return _exporter; }
            set { _exporter = value; }
        }

        /// <summary>
        /// Панель на которой распологается таблица
        /// </summary>
        public RichPanel GridPlace
        {
            get { return gridPlace; }
        }

        public ColorRuleCollection ColorRules
        {
            get { return _colorRules; }
            set { _colorRules = value; }
        }

        public float MouseWheelGridScale
        {
            get { return _mouseWheelGridScale; }
            set { _mouseWheelGridScale = value; }
        }

        /// <summary>
        /// Масштаб таблицы
        /// </summary>
        public GridScale GridScale
        {
            get { return _gridScale; }
        }

        #endregion
    }
}
