using System;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Ячейа заголовка
    /// </summary>
    public class CaptionCell: GridCell
    {
        private CaptionsList _captionList;
        private bool _isDummy = false;
        private string _hierarchyUN;
        private string _uniqueName;
        private int _tupleIndex = -1;
        private int _topCountTupleIndex = -1;
        private int _bottomCountTupleIndex = -1;
        //формат значения у мер, сейчас сделанно не красиво, т.к. другим заголовкам это поле нафиг не сдалось
        //надо пересмотреть объектную модель
        private ValueFormat _measureValueFormat;
        //опциональная ячейка, сейчас служит для отображения дополнительной ифнормации к заголовку фильтра
        private OptionalTextCell _optionalCell;
        //опциональная кнопка, используется у заголовков измерений для вызова формы структуры измерения.
        private DropButtonType _dropButton;
        //конпка сортировки, вызывает событие об изменении сортировки
        private SortButtonType _sortButton;
        //тип сортировки 
        private SortType _sortType;
        //признак того что это заголовок меры отклонения от среднего
        private bool isAverageDev = false;


        //признак того что это заголовок меры отклонения от среднего
        public bool IsAverageDev
        {
            get { return this.isAverageDev; }
        }

        //Конструктор для заголовков строк/столбцов
        public CaptionCell(CaptionsList captions, string text, string uniqueName, string hierarchyUN, 
            SortType sortType)
            : this(captions, text, uniqueName, hierarchyUN, null, true, false, true, sortType, false)
        {
        }

        //Конструктор для заголовков мер
        public CaptionCell(CaptionsList captions, string text, string uniqueName, ValueFormat valueFormat,
            bool withDropButton, SortType sortType, bool isAverageDev)
            : this(captions, text, uniqueName, string.Empty, valueFormat, withDropButton, false, true, sortType, isAverageDev)
        {
        }

        //Конструкотор для заголовков фильтров
        public CaptionCell(CaptionsList captions, string text, string uniqueName)
            : this(captions, text, uniqueName, uniqueName, null, true, true, false, SortType.None, false)
        {
        }

        /// <summary>
        /// Конструктор ячейки заголовка
        /// </summary>
        /// <param name="captions">Родительская коллекция</param>
        /// <param name="text">Отображаемый текст</param>
        /// <param name="withDropButton">Признак того, что ячейка создается с кнопкой</param>
        public CaptionCell(CaptionsList captions, string text, string uniqueName, string hierarchyUN,
            ValueFormat valueFormat, bool withDropButton, bool withOptionalCell, bool withSortButton, 
            SortType sortType, bool isAverageDev)
            : base(captions.Grid, GridObject.CaptionCell)
        {
            this.Captions = captions;
            this.UniqueName = uniqueName;
            base.Size = new System.Drawing.Size(120, 17);
            base.Text = text;
            this.HierarchyUN = hierarchyUN;
            this.MeasureValueFormat = valueFormat == null ? new ValueFormat() : valueFormat;
            base.Style = this.Captions.Style;
            this.SortType = sortType;
            this.isAverageDev = isAverageDev;

            if (withDropButton)
                this.DropButton = new DropButtonType(captions.Grid, this);
            if (withOptionalCell)
                this.OptionalCell = new OptionalTextCell(captions.Grid, this);
            if (withSortButton)
                this.SortButton = new SortButtonType(captions.Grid, this);

            if (this.Grid.SelectedCells != null)
            {
                if (this.Grid.SelectedCells.HashCodes.Contains(this.GetHashCode()))
                {
                    this.State = ControlState.Selected;
                    if (!this.Grid.SelectedCells.CurrentCells.Contains(this))
                    {
                        this.Grid.SelectedCells.CurrentCells.Add(this);
                    }
                }
                else
                {
                    this.State = ControlState.Normal;
                }
            }

        }

        /// <summary>
        /// Обработчик на нажатие ячейки
        /// </summary>
        /// <param name="mousePosition">Точное место клика</param>
        public override void OnClick(System.Drawing.Point mousePosition)
        {
            if (this.DropButton != null)
            {
                if (this.DropButton.Contain(mousePosition))
                {
                    Cursor.Clip = Rectangle.Empty;
                    this.DropButton.OnClick(mousePosition);
                    return;
                }
            }
            if (this.SortButton != null)
            {
                if (this.SortButton.Contain(mousePosition))
                {
                    Cursor.Clip = Rectangle.Empty;
                    this.SortButton.OnClick(mousePosition);
                    return;
                }
            }
        }

        /// <summary>
        /// Если нажат дочерний элемент контрола вернет true
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool IsClickChildElement(Point point)
        {
            if (this.DropButton != null)
            {
                if (this.DropButton.Contain(point))
                {
                    return true;
                }
            }

            if (this.SortButton != null)
            {
                if (this.SortButton.Contain(point))
                {
                    return true;
                }
            }
            return false;
        }

        public override string GetComment()
        {
            string comment = base.GetComment();
            switch (this.Captions.Type)
            {
                case CaptionType.Columns:
                case CaptionType.Rows:
                    {
                        //Есля заголовок принадлежит строкам или столбцам, добавим к комментарию имя измерения
                        Microsoft.AnalysisServices.AdomdClient.Hierarchy h = this.Grid.PivotData.GetHierarchyByUniqueName(this.HierarchyUN);
                        if (h != null)
                            comment = "Измерение: " + h.Caption + "\nУровень: " + comment;
                        break;
                    }
                case CaptionType.Filters:
                    {
                        FieldSet dimension = this.Grid.PivotData.FilterAxis.FieldSets.GetFieldSetByName(this.HierarchyUN);
                        if (dimension != null)
                        {
                            comment = dimension.GetMembersComment(((FilterCaptions)this.Captions).TipDisplayMaxMemberCount);
                        }
                        break;
                    }
            }
            return comment;
        }


        /// <summary>
        /// Получить хеш код ячейки
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            switch (this.Captions.Type)
            {
                case CaptionType.Measures:
                    return GetMeasureAllUniqueName().GetHashCode();
                case CaptionType.Columns:
                case CaptionType.Rows:
                case CaptionType.Filters:
                    return this.UniqueName.GetHashCode();
            }

            return -1;
        }

        /// <summary>
        /// Получает свой и всех своих родителей UN 
        /// </summary>
        /// <returns></returns>
        public string GetMeasureAllUniqueName()
        {
            return this.GetMeasureAllUniqueName(string.Empty);
        }

        /// <summary>
        /// Получает свой и всех своих родителей UN 
        /// </summary>
        /// <param name="splitter">разделитель между UN</param>
        /// <returns></returns>
        public string GetMeasureAllUniqueName(string splitter)
        {
            string result = this.UniqueName;
            DimensionCell dimCell = ((MeasuresCaptionsSection) this.Captions).ColumnCell;

            string un = dimCell != null ? dimCell.GetAllUniqueName(splitter) : String.Empty;
            result += (result == string.Empty) ? un : splitter + un;

            return result;
        }



        /// <summary>
        /// Получить границы ячейки с учетом смещения, параметром отвечает за включать/невключать 
        /// в размеры заголовка размеры опциональной ячейки
        /// </summary>
        /// <returns>Rectangle (Границы ячейки)</returns>
        public Rectangle GetOffsetBounds(bool includeOptionalCell)
        {
            Rectangle result = base.Bounds;
            if (includeOptionalCell)
                result.Height = this.Size.Height;
            
            switch (this.Captions.Type)
            {
                case CaptionType.Filters:
                    {
                        result.X -= base.Grid.FiltersScrollBarState.Offset;
                        break;
                    }
                case CaptionType.Measures:
                    {
                        result.X -= this.Grid.HScrollBarState.Offset;
                        break;
                    }
            }
            return result;
        }

        /// <summary>
        /// Получить границы ячейки с учетом смещения
        /// </summary>
        /// <returns>Rectangle (Границы ячейки)</returns>
        public override Rectangle GetOffsetBounds()
        {
            return this.GetOffsetBounds(true);
        }

        /// <summary>
        /// Видимая часть кнопки
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            return Rectangle.Intersect(this.Captions.GetVisibleBounds(), this.GetOffsetBounds());
        }

        /// <summary>
        /// Получить видимую область родителя
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetParentVisibleBounds()
        {
            return this.Captions.Type == CaptionType.Measures ?
                ((MeasuresCaptionsSection)this.Captions).Sections.GetVisibleBounds() :
                this.Captions.GetVisibleBounds();
        }

        /// <summary>
        /// Отрисовка ячейки
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="painter">Painter</param>
        public override void OnPaint(System.Drawing.Graphics graphics, Painter painter)
        {
            //получаем координаты контрола без учета высоты опциональной кнопки
            base.OffsetBounds = this.GetOffsetBounds(false);

            ControlState captionState = this.State;
            
            if (this.DropButton != null)
            {
                this.DropButton.State = ControlState.Normal;
                switch (base.State)
                {
                    case ControlState.Hot:
                    case ControlState.Selected:
                        {
                            if (this.DropButton.GetHitTest(this.Grid.CurrentMousePosition, true))
                            {
                                captionState = ControlState.Normal;
                                this.DropButton.State = base.State;
                            }
                            break;
                        }
                    case ControlState.Normal:
                        {
                            break;
                        }
                }
            }

            Rectangle textBounds = base.OffsetBounds;
            if (this.SortButton != null)
            {
                int sortButtonWidth = this.SortButton.Width + 5;
                textBounds.Location = new Point(textBounds.Location.X + sortButtonWidth, textBounds.Location.Y);
                textBounds.Width -= sortButtonWidth;
            }
            if (this.DropButton != null)
                //если у ячейки имеется кнопка, то место для текста размещеного в ней становиться меньше
                textBounds.Width -= this.DropButton.Width;

            string captionText = base.Text;
            //Если ячейка является заголовком показателя и формат отображения едениц измерения равен - 
            //отображать в заголовке, то к имене прикручиваем еденицы измерения
            if ((this.MeasureValueFormat != null) &&
                (this.MeasureValueFormat.UnitDisplayType == UnitDisplayType.DisplayAtCaption))
            {
                string unit = this.Grid.PivotData.TotalAxis.GetFormatUnit(this.MeasureValueFormat.FormatType);
                captionText += (unit != string.Empty ? ", " + unit : string.Empty);
            }

            painter.DrawCaption(graphics, base.OffsetBounds, base.Style, captionState, captionText, textBounds);
            if (this.DropButton != null)
                this.DropButton.OnPaint(graphics, painter);
            if (this.SortButton != null)
                this.SortButton.OnPaint(graphics, painter);
            if (this.OptionalCell != null)
            {
                this.OptionalCell.State = captionState;
                this.OptionalCell.OnPaint(graphics, painter);
            }

            //А сейчас для кооректного распознования положения нашей ячейки, получаем размеры с учетом
            //опциональной ячейки (если она есть)
            base.OffsetBounds = this.GetOffsetBounds(true);
        }

        public override bool GetHitTest(Point point)
        {
            if (this.OptionalCell == null)
                return base.GetHitTest(point);
            else
            {
                Rectangle bounds = new Rectangle(this.Location, this.Size);
                return bounds.Contains(point);
            }
        }

        /// <summary>
        /// Сортировать, меняет тип сортировки на следующий
        /// </summary>
        /// <param name="sortType"></param>
        public void Sort()
        {
            if (this.Grid.PivotData.IsDeferDataUpdating)
                return;

            //переключаем тип сортировки на следующий
            int sortTypeIndex = (int)this.SortType + 1;
            //если индекс больше чем количество элементов, начинаем сначала
            //Т.к. сейчас в сортировке можно ломать иерархию, что к таблице неприменимо 
            //будем искуствено ограничивать типы возможных сортировок System.Enum.GetNames(typeof(SortType)).Length - 1)
            if (sortTypeIndex > 2)
                sortTypeIndex = 0;
            this.Sort((SortType)sortTypeIndex);
        }

        /// <summary>
        /// Сортировать
        /// </summary>
        /// <param name="sortType">тип сортировки</param>
        public void Sort(SortType sortType)
        {
            string uniqueName = string.Empty;
            string sortedTupleUN = string.Empty;
            switch (this.Captions.Type)
            {
                case CaptionType.Columns:
                case CaptionType.Rows:
                    {
                        uniqueName = this.UniqueName;// HierarchyUN;
                        break;
                    }
                case CaptionType.Measures:
                    {
                        uniqueName = this.UniqueName;
                        MeasuresCaptionsSection measuresSection = (MeasuresCaptionsSection)this.Captions;
                        if (measuresSection.ColumnCell != null)
                            sortedTupleUN = measuresSection.ColumnCell.TupleUN;
                        break;
                    }
            }

            base.Grid.OnSortClick(uniqueName, sortType, sortedTupleUN);
        }

        /// <summary>
        /// Очищаем все ссылки у ячейки
        /// </summary>
        public new void Clear()
        {
            this.Captions = null;
            this.MeasureValueFormat = null;
            if (this.DropButton != null)
                this.DropButton.Clear();
            if (this.OptionalCell != null)
                this.OptionalCell.Clear();
            //if (this.So)
            this.DropButton = null;
            base.Clear();
        }

        /// <summary>
        /// Родительская коллекция заголовков
        /// </summary>
        public CaptionsList Captions
        {
            get { return _captionList; }
            set { _captionList = value; }
        }

        /// <summary>
        /// Кнопка у ячейки 
        /// </summary>
        public DropButtonType DropButton
        {
            get { return _dropButton; }
            set { _dropButton = value; }
        }
        /// <summary>
        /// Кнопка сортировки
        /// </summary>
        public SortButtonType SortButton
        {
            get { return _sortButton; }
            set { _sortButton = value; }
        }

        /// <summary>
        /// Uniqe Name иерархии
        /// </summary>
        public string HierarchyUN
        {
            get { return _hierarchyUN; }
            set { _hierarchyUN = value; }
        }

        /// <summary>
        /// Уникальное имя заголовка (для заголовков измерений - это имя уровня, 
        /// для показателей - это имя самого показателя)
        /// </summary>
        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        /// <summary>
        /// Фиктовность заголовка, заголовок фиктивен в случае когда он есть в PivotData, но в селсете
        /// его нет, т.к. запрос был по не пустым данным
        /// </summary>
        public bool IsDummy
        {
            get { return _isDummy; }
            set { _isDummy = value; }
        }

        /// <summary>
        /// Относится ли заголовок к вычислимому элементу
        /// </summary>
        public bool IsCalculate
        {
            get { return CheckCalculateMember(); }
        }

        /// <summary>
        /// Индекс в кортеже, которому соответсвует данный caption
        /// </summary>
        public int TupleIndex
        {
            get { return _tupleIndex; }
            set { _tupleIndex = value; }
        }

        /// <summary>
        /// Индекс ячейки в кортеже, в которой содержится признак что ячейка нах-ся в k-первых
        /// </summary>
        public int TopCountTupleIndex
        {
            get { return _topCountTupleIndex; }
            set { _topCountTupleIndex = value; }
        }

        /// <summary>
        /// Индекс ячейки в кортеже, в которой содержится признак что ячейка нах-ся в k-последних
        /// </summary>
        public int BottomCountTupleIndex
        {
            get { return _bottomCountTupleIndex; }
            set { _bottomCountTupleIndex = value; }
        }

        /// <summary>
        /// Формат значения для мер
        /// </summary>
        public ValueFormat MeasureValueFormat
        {
            get { return _measureValueFormat; }
            set { _measureValueFormat = value; }
        }

        /// <summary>
        /// Gпциональная ячейка, сейчас служит для отображения дополнительной ифнормации 
        /// к заголовку фильтра
        /// </summary>
        public OptionalTextCell OptionalCell
        {
            get { return _optionalCell; }
            set { _optionalCell = value; }
        }

        /// <summary>
        /// Оригинальная ширина
        /// </summary>
        public int OriginalWidth
        {
            get { return base.OriginalSize.Width; }
        }


        /// <summary>
        /// Переопределяем выставление ширины, если есть опциональная ячейка, то выставляем ширину и ей
        /// </summary>
        public override int Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                if (this.OptionalCell != null)
                    this.OptionalCell.Width = base.OriginalSize.Width;
            }
        }



        /// <summary>
        /// Переопределяем высоту заголовка, считаться она будет как своя высота, плюс высота 
        /// опциональной ячейки
        /// </summary>
        public override int Height
        {
            get
            {
                return base.Height + (this.OptionalCell != null ? this.OptionalCell.Height : 0);
            }
            set
            {
                base.Height = value - (this.OptionalCell != null ? this.OptionalCell.OriginalSize.Height : 0);
            }
        }


        public int OriginalHeight
        {
            get
            {
                return base.OriginalSize.Height + (this.OptionalCell != null ? this.OptionalCell.OriginalSize.Height : 0);
            }
        }


        /// <summary>
        /// Тип сортировки
        /// </summary>
        public SortType SortType
        {
            get { return _sortType; }
            set { _sortType = value; }
        }

        public override string GetValue()
        {
            string captionText = base.Text;
            //Если ячейка является заголовком показателя и формат отображения едениц измерения равен - 
            //отображать в заголовке, то к имене прикручиваем еденицы измерения
            if ((this.MeasureValueFormat != null) &&
                (this.MeasureValueFormat.UnitDisplayType == UnitDisplayType.DisplayAtCaption))
            {
                string unit = this.Grid.PivotData.TotalAxis.GetFormatUnit(this.MeasureValueFormat.FormatType);
                captionText += (unit != string.Empty ? ", " + unit : string.Empty);
            }
            return captionText;
        }

        private bool CheckCalculateMember()
        {
            return ((this.Grid.PivotData.TotalAxis.GetCustomTotalByName(this.UniqueName) != null)||(this.IsAverageDev));
        }


        #region Дополнительные классы
        /// <summary>
        /// Кнопка ячейки заголовка
        /// </summary>
        public class DropButtonType : GridCell
        {
            private CaptionCell _captionCell;

            public CaptionCell CaptionCell
            {
                get { return this._captionCell; }
                set { this._captionCell = value; }
            }

            public DropButtonType(ExpertGrid grid, CaptionCell captionCell)
                : base(grid, GridObject.DropButton)
            {
                this._captionCell = captionCell;
                base.Size = new Size(15, 15);
                base.Location = new Point(captionCell.Width - 20, 1);
            }

            internal bool Contain(Point mousePosition)
            {
                return base.GetHitTest(mousePosition, true);
            }

            public override int GetHashCode()
            {
                return -1;
            }

            public override void OnClick(Point mousePosition)
            {
                base.Grid.OnDropButtonClick(this.CaptionCell.HierarchyUN);
            }

            public override void OnPaint(Graphics graphics, Painter painter)
            {
                if (this._captionCell != null)
                {
                    base.OffsetBounds = GetOffsetBounds();
                    painter.DrawDropButton(graphics, base.OffsetBounds, base.State, false, this.CaptionCell.Style, this.Grid.GridScale);
                }
            }

            public new void Clear()
            {
                this.CaptionCell = null;
                base.Clear();
            }

            public override Rectangle GetOffsetBounds()
            {
                Rectangle bounds = this._captionCell.OffsetBounds;
                bounds.Y += base.Location.Y;
                bounds.X = bounds.Right - this.Width - 2;
                bounds.Size = new Size(this.Width, this.Height); 
                return bounds;
            }

            public override Rectangle GetVisibleBounds()
            {
                return Rectangle.Intersect(this.CaptionCell.GetVisibleBounds(), this.GetOffsetBounds());
            }

            /// <summary>
            /// Получить видимую область родителя
            /// </summary>
            /// <returns></returns>
            public override Rectangle GetParentVisibleBounds()
            {
                return this.CaptionCell.GetVisibleBounds();
            }

            public override bool IsClickChildElement(Point point)
            {
                return false;
            }
        }

        /// <summary>
        /// Опциональная ячейка заголовка
        /// </summary>
        public class OptionalTextCell : GridCell
        {
            private CaptionCell _captionCell;

            public CaptionCell CaptionCell
            {
                get { return this._captionCell; }
                set { this._captionCell = value; }
            }

            public OptionalTextCell(ExpertGrid grid, CaptionCell captionCell)
                : base(grid, GridObject.OptionalCell)
            {
                this._captionCell = captionCell;
                this.Style = (this.CaptionCell.Captions as FilterCaptions).ValueCellStyle;
                base.Size = new Size(captionCell.OriginalSize.Width, this.Style.OriginalTextHeight);
                base.Location = new Point(captionCell.Location.X,
                    captionCell.Bounds.Bottom - this.Style.TextHeight);
            }

            internal bool Contain(Point mousePosition)
            {
                return base.GetHitTest(mousePosition, true);
            }

            public override void OnClick(Point mousePosition)
            {
            }

            public override void OnPaint(Graphics graphics, Painter painter)
            {
                if (this.CaptionCell != null)
                {
                    base.OffsetBounds = this.GetOffsetBounds();
                    painter.DrawMeasureCell(graphics, base.OffsetBounds, this.State, this.Style, base.Text);
                }
            }

            public new void Clear()
            {
                this.CaptionCell = null;
                base.Clear();
            }

            public override Rectangle GetOffsetBounds()
            {
                Point location = new Point(this.CaptionCell.OffsetBounds.X, this.CaptionCell.Bounds.Bottom);
                return new Rectangle(location, this.Size);
            }

            public override Rectangle GetVisibleBounds()
            {
                return this.GetOffsetBounds();
            }

            /// <summary> 
            /// Получить видимую область родителя
            /// </summary>
            /// <returns></returns>
            public override Rectangle GetParentVisibleBounds()
            {
                return this.CaptionCell.GetVisibleBounds();
            }

            public override bool IsClickChildElement(Point point)
            {
                return false;
            }
        }

        /// <summary>
        /// Кнопка сортировки
        /// </summary>
        public class SortButtonType : GridCell
        {
            private CaptionCell _captionCell;

            public CaptionCell CaptionCell
            {
                get { return this._captionCell; }
                set { this._captionCell = value; }
            }

            public SortButtonType(ExpertGrid grid, CaptionCell captionCell)
                : base(grid, GridObject.SortButton)
            {
                this._captionCell = captionCell;
                base.Size = new Size(8, 8);
                base.Location = new Point(4, 4);
            }

            internal bool Contain(Point mousePosition)
            {
                return base.GetHitTest(mousePosition, true);
            }

            public override void OnClick(Point mousePosition)
            {
                this.CaptionCell.Sort();
            }

            public override void OnPaint(Graphics graphics, Painter painter)
            {
                if (this._captionCell != null)
                {
                    base.OffsetBounds = GetOffsetBounds();
                    painter.DrawSortButton(graphics, base.OffsetBounds, this.CaptionCell.SortType);
                }
            }

            public override Rectangle GetVisibleBounds()
            {
                return Rectangle.Intersect(this.CaptionCell.GetVisibleBounds(), this.GetOffsetBounds());
            }

            public override Rectangle GetOffsetBounds()
            {
                Rectangle bounds = this._captionCell.OffsetBounds;
                bounds.Y += base.Location.Y;
                bounds.X += base.Location.X;
                bounds.Size = new Size(this.Width, this.Height);
                return bounds;
            }

            public override Rectangle GetParentVisibleBounds()
            {
                return this.CaptionCell.GetVisibleBounds();
            }

            public new void Clear()
            {
                this.CaptionCell = null;
                base.Clear();
            }

            public override bool IsClickChildElement(Point point)
            {
                return false;
            }
        }

        #endregion
    }
}
