using System;
using System.Collections.Generic;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;
using System.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Секция заголовков показателей
    /// </summary>
    public class MeasuresCaptionsSection : CaptionsList
    {
        private MeasureCaptionsSections _sections;
        private MeasuresSectionInfo _measuresInfo;
        private Rectangle _offsetBounds = Rectangle.Empty;
        private DimensionCell _columnCell;
        private bool _isVisible;
        private bool _isDummy;

        public MeasuresCaptionsSection(MeasureCaptionsSections sections, DimensionCell columnCell)
            : base(sections.Grid, CaptionType.Measures)
        {
            this.Sections = sections;
            this.ColumnCell = columnCell;
            base.Style = this.Sections.Style;
        }

        //Заглухи
        public override void Initialize(Data.PivotData pivotData)
        {

        }
        public override void Initialize(CellSetInfo clsInfo)
        {

        }
        protected override void LoadSizes(XmlNode sizesNode)
        {

        }
        protected override void SaveSizes(XmlNode sizesNode)
        {

        }

        /// <summary>
        /// Синхронизация данных таблицы с PivotData
        /// </summary>
        /// <param name="pivotData"></param>
        public override void SynchronizePivotData(Data.PivotData pivotData)
        {
            if (pivotData == null)
                return;
            foreach (CaptionCell captionCell in this)
            {
                PivotTotal field = pivotData.TotalAxis.GetTotalByName(captionCell.UniqueName);
                if (field != null)
                {
                    captionCell.Text = field.Caption;
                }
            }
        }

        /// <summary>
        /// Расчет секции заголовков показателей
        /// </summary>
        /// <param name="startPoint">Стартовая точка</param>
        public override void RecalculateCoordinates(System.Drawing.Point startPoint)
        {
            base.Location = startPoint;
            Point locationCaption = startPoint;
            foreach (CaptionCell caption in this)
            {
                caption.Location = locationCaption;
                locationCaption.X += this.IsVisible ? caption.Width : 0;
            }
        }

        /// <summary>
        /// Отрисовка секции заголовков показателей
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public new void Draw(Graphics graphics, Painter painter)
        {
            Rectangle bounds = this.Bounds;
            this.VisibleBounds = this.GetVisibleBounds(bounds);
            this.OffsetBounds = this.GetOffsetBounds(bounds);
            if (this.IsVisible)
            {
                foreach (CaptionCell caption in this)
                {
                    caption.OnPaint(graphics, painter);
                }
            }
        }

        /// <summary>
        /// Очистка всех ссылок секции
        /// </summary>
        public new void Clear()
        {
            this.Sections = null;
            this.ColumnCell = null;
            base.Clear();
        }

        /// <summary>
        /// Получить относительные координаты
        /// </summary>
        /// <returns></returns>
        public Rectangle GetOffsetBounds()
        {
            Rectangle result = base.Bounds;
            result.X -= this.Grid.HScrollBarState.Offset;
            return result;
        }

        /// <summary>
        /// Получить относительные координаты
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public Rectangle GetOffsetBounds(Rectangle bounds)
        {
            bounds.X -= this.Grid.HScrollBarState.Offset;
            return bounds;
        }

        /// <summary>
        /// Получить видимую область секции
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = base.Bounds;
            result.Height += 1;
            result.X = Math.Max(result.X - this.Grid.HScrollBarState.Offset, this._sections.VisibleBounds.X);
            result.Width = Math.Min(this.Grid.GridBounds.Right - result.X, result.Width + 1);
            return result;
        }

        /// <summary>
        /// Получить видимую область секции
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public Rectangle GetVisibleBounds(Rectangle bounds)
        {
            bounds.Height += 1;
            bounds.X = Math.Max(bounds.X - this.Grid.HScrollBarState.Offset, this._sections.VisibleBounds.X);
            bounds.Width = Math.Min(this.Grid.GridBounds.Right - bounds.X, bounds.Width + 1);
            //bounds.Width = this.Grid.GridBounds.Right - bounds.X;
            return bounds;
        }

        /// <summary>
        /// Оригинальная ширина
        /// </summary>
        public int OriginalWidth
        {
            get
            {
                int width = 0;
                //спорный момент конечно, что при невидимости ширина равна нулю, но это сделано для
                //корректного поиска секции
                if (this.IsVisible)
                {
                    foreach (CaptionCell caption in this)
                    {
                        width += caption.OriginalWidth;
                    }
                }
                return width;
            }
        }

        /// <summary>
        /// Ширина
        /// </summary>
        public override int Width
        {
            get
            {
                int width = 0;
                //спорный момент конечно, что при невидимости ширина равна нулю, но это сделано для
                //корректного поиска секции
                if (this.IsVisible)
                {
                    foreach (CaptionCell caption in this)
                    {
                        width += caption.Width;
                    }
                }
                return width;
            }

            set
            {
                int captionsCount = this.Count;
                if (captionsCount > 0)
                {
                    CaptionCell lastCaption = this[captionsCount - 1];
                    int newCaptionWidth = (value + this.Location.X - lastCaption.Location.X);
                    lastCaption.Width = newCaptionWidth;
                }
            }
        }

        /// <summary>
        /// Установка высоты
        /// </summary>
        /// <param name="height"></param>
        public void SetHeight(int height)
        {
            foreach (CaptionCell caption in this)
            {
                caption.Height = height;
            }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public override int Height
        {
            get
            {
                int result = 0;

                if ((this.IsVisible) && (this.Count > 0))
                    result = this[0].Height;
                return result;
            }
            set
            {
                this.Sections.Height = value;
            }
        }

        /// <summary>
        /// Оригинальная высота (без зума)
        /// </summary>
        public int OriginalHeight
        {
            get
            {
                int result = 0;

                if ((this.IsVisible) && (this.Count > 0))
                    result = this[0].OriginalHeight;
                return result;
            }
        }


        /// <summary>
        /// Ссылка на коллекцию секций заголовков показателей
        /// </summary>
        public MeasureCaptionsSections Sections
        {
            get
            {
                return this._sections;
            }
            set
            {
                this._sections = value;
            }
        }

        /// <summary>
        /// Относительные координаты
        /// </summary>
        public Rectangle OffsetBounds
        {
            get
            {
                return this._offsetBounds;
            }
            set
            {
                this._offsetBounds = value;
            }
        }

        /// <summary>
        /// Ячейка колонки, которой принадлежит секция показателей
        /// </summary>
        public DimensionCell ColumnCell
        {
            get
            {
                return this._columnCell;
            }
            set
            {
                this._columnCell = value;
            }
        }

        /// <summary>
        /// Видимость, если секция фиктивная, значит видимая...
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return (this._isVisible || this.IsDummy);
            }
            set
            {
                this._isVisible = value;
            }
        }

        /// <summary>
        /// Получить видимость
        /// </summary>
        /// <returns>bool</returns>
        public bool GetVisibility()
        {
            return this.ColumnCell == null ? true : this.ColumnCell.GetVisibility();
        }

        /// <summary>
        /// Признак того, что секция является фиктивной и рисуется только для отсутсвия свободного пространсва.
        /// Выставляется в случае, когда слохпнут один из верхних уровней, у которого нет настоящего итога, и мы
        /// используем существующую секцию как заглушку, все данные в этой секции будут пустой строкой...
        /// </summary>
        public bool IsDummy
        {
            get 
            { 
                return this._isDummy; 
            }
            set 
            { 
                this._isDummy = value; 
            }
        }

        public MeasuresSectionInfo MeasuresInfo
        {
            get { return _measuresInfo; }
            set { _measuresInfo = value; }
        }
    }

    /// <summary>
    /// Коллекция секций заголовков показателей
    /// </summary>
    public class MeasureCaptionsSections : GridCollection<MeasuresCaptionsSection>
    {
        private CellStyle _style;
        private int _captionsHeight = 0;//высота заголовков
        private int[] _captionsWidths = new int[0];//ширина каждого заголовка
        private int maxHeight;

        public MeasureCaptionsSections(ExpertGrid grid)
        {
            this.Grid = grid;
            
            this.Style = new CellStyle(this.Grid, GridConsts.gridCaptionsBackColorStart, GridConsts.gridCaptionsBackColorEnd,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);            

            //если текст не помещается в ячейку, обрезаем его и ставим многоточие
            this.Style.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.Style.StringFormat.FormatFlags = StringFormatFlags.LineLimit;
        }

        private void AfterInitialization()
        {
            //Устанавливаем прежние размеры
            this.DetermineWidths(this.CaptionsWidths);
            this.Height = this.CaptionsHeight;
        }

        private MeasuresCaptionsSection AddMeasuresCaptionsSection(DimensionCell dimCell, 
            MeasuresSectionInfo clsSection)
        {
            MeasuresCaptionsSection captionsSection = new MeasuresCaptionsSection(this, dimCell);
            foreach (MeasureInfo clsMeasure in clsSection.MeasuresInfo)
            {
                captionsSection.Add(new CaptionCell(captionsSection, clsMeasure.Caption,
                    clsMeasure.UniqueName, null, false, SortType.None, false));
            }
            this.Add(captionsSection);
            return captionsSection;
        }

        /// <summary>
        /// Получим сортировку показателя
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="dimCell"></param>
        /// <returns></returns>
        private Data.SortType GetMeasureSortType(PivotTotal measure, DimensionCell dimCell)
        {
            Data.SortType result = measure.SortType;
            //если показатель сортируется по кортежу, проверим та ли эта секция
            if ((measure.SortedTupleUN != string.Empty) && (dimCell != null))
            {
                if (measure.SortedTupleUN != dimCell.TupleUN)
                    result = SortType.None;
            }
            return result;
        }

        private MeasuresCaptionsSection AddMeasuresCaptionsSection(DimensionCell dimCell, Data.PivotData pivotData, MeasuresSectionInfo measuresInfo)
        {
            MeasuresCaptionsSection captionsSection = new MeasuresCaptionsSection(this, dimCell);
            foreach (PivotTotal measure in pivotData.TotalAxis.VisibleTotals)
            {
                Data.SortType sortType = this.GetMeasureSortType(measure, dimCell);
                captionsSection.Add(new CaptionCell(captionsSection, measure.Caption,
                    measure.UniqueName, measure.Format, false, sortType, false));

                //Добавляем меру - отклонение, если нужно
                if (pivotData.IsNeedAverageCalculation() && pivotData.AverageSettings.IsAverageDeviationCalculate)
                {
                    string measureUName = PivotData.GetAverageDeviationMeasureName(measure.UniqueName);
                    string measureCaption = PivotData.GetNameFromUniqueName(measureUName);
                    captionsSection.Add(new CaptionCell(captionsSection, measureCaption,
                        measureUName, measure.Format, false, SortType.None, true));
                }
            }

            captionsSection.MeasuresInfo = measuresInfo;
            this.Add(captionsSection);
            return captionsSection;
        }

        /// <summary>
        /// Количество фиктивных заголовков в секции
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private int GetDummyCaptionCount(MeasuresCaptionsSection section)
        {
            int result = 0;
            foreach (CaptionCell caption in section)
            {
                if (caption.IsDummy)
                    result++;
            }
            return result;
        }

        /// <summary>
        /// Проставим индексы кортежей у заголовков
        /// </summary>
        /// <param name="captionsSection"></param>
        private void DetermineTuplesIndexs(MeasuresCaptionsSection captionsSection)
        {
            //количество заголовков в одной секции заголовков показателей
            int allCaptionsCount = captionsSection.Count;
            //количество фиктивных заголовков мер
            int dummyCaptionCount = this.GetDummyCaptionCount(captionsSection);
            //количесчество действиельных заголовков
            int validCaptionCount = allCaptionsCount - dummyCaptionCount;
            //количество фиктивных заголовков на момент итерации
            int prepareDummyCount = 0;

            CaptionCell currentCaption;
            //проходим по каждому заголовку показателя
            for (int c = 0; c < allCaptionsCount; c++)
            {
                currentCaption = captionsSection[c];
                if (currentCaption.IsDummy)
                {
                    currentCaption.TupleIndex = -1;
                    prepareDummyCount++;
                }
                else
                {
                    int tupleIndex = -1;

                    /* Как та все похитрому здесь...
                    //у секции заголовковков позазателей получаем ссылку на ячейку оси колонок
                    DimensionCell columnCell = captionsSection.ColumnCell;
                    if (columnCell != null)
                    {
                        //Мега формула, вычисляем инедкс требующего отображения кортежа
                        tupleIndex = columnCell.TupleIndex - validCaptionCount + c - prepareDummyCount + 1;
                    }
                    else
                    {
                        //вычисляем инедкс требующего отображения кортежа
                        tupleIndex = c - prepareDummyCount;
                    }
                        currentCaption.TupleIndex = tupleIndex;
                    */

                    //Просто берем индексы из MeasuresInfo
                    MeasureInfo mInfo = captionsSection.MeasuresInfo.GetMeasureByUN(currentCaption.UniqueName);

                    currentCaption.TupleIndex = mInfo.TupleIndex;
                    //Если надо, найдем индекс ячейки, в кот. нах-ся признак попадания текущей ячейки в топ
                    if(this.Grid.PivotData.IsNeedTopCountCalculation())
                    {
                        string topCountMeasureName =
                            this.Grid.PivotData.TopCountSettings.GetTopCountMeasureName(currentCaption.UniqueName);
                        mInfo = captionsSection.MeasuresInfo.GetMeasureByUN(topCountMeasureName);
                        currentCaption.TopCountTupleIndex = (mInfo != null) ? mInfo.TupleIndex : -1;
                    }

                    //Если надо, найдем индекс ячейки, в кот. нах-ся признак попадания текущей ячейки в k-последних
                    if (this.Grid.PivotData.IsNeedBottomCountCalculation())
                    {
                        string bottomCountMeasureName =
                            this.Grid.PivotData.BottomCountSettings.GetBottomCountMeasureName(currentCaption.UniqueName);
                        mInfo = captionsSection.MeasuresInfo.GetMeasureByUN(bottomCountMeasureName);
                        currentCaption.BottomCountTupleIndex = (mInfo != null) ? mInfo.TupleIndex : -1;
                    }

                }
            }
        }

        /// <summary>
        /// Инициализация по информации извлеченной из селсета
        /// </summary>
        /// <param name="columnsLeafs">все листья колонок</param>
        /// <param name="clsInfo">селл сет</param>
        public void Initialize(DimensionCell[] columnsLeafs, CellSetInfo clsInfo)
        {
            if ((clsInfo != null) && (clsInfo.MeasuresSectionsInfo.Count > 0))
            {


                if (columnsLeafs.Length > 0)
                {
                    foreach (DimensionCell dimCell in columnsLeafs)
                    {
                        MeasuresSectionInfo clsSection;
                        if (dimCell.IsGrandTotal)
                            clsSection = this.Grid.PivotData.ClsInfo.LastMeasureSectionInfo;
                        else
                            //Ищем секцию показателей в селсете соответствующую данной ячейке
                            clsSection = this.Grid.PivotData.ClsInfo.GetSectionInfoByTupleUN(dimCell.TupleUN);

                        MeasuresCaptionsSection captionsSection = AddMeasuresCaptionsSection(dimCell, clsSection);
                        //Проставим индексы кортежей у заголовков
                        this.DetermineTuplesIndexs(captionsSection);
                    }
                }
                else
                {
                    foreach (MeasuresSectionInfo clsSection in clsInfo.MeasuresSectionsInfo)
                    {
                        MeasuresCaptionsSection captionsSection = AddMeasuresCaptionsSection(null, clsSection);
                        //Проставим индексы кортежей у заголовков
                        this.DetermineTuplesIndexs(captionsSection);
                    }
                }
                this.AfterInitialization();
            }
        }

        /// <summary>
        /// Проверка на фиктивность каждой кнопки в секции
        /// </summary>
        /// <param name="captionsSection"></param>
        /// <param name="clsSection"></param>
        private void DummyCheck(MeasuresCaptionsSection captionsSection, MeasuresSectionInfo clsSection)
        {
            foreach (CaptionCell caption in captionsSection)
            {
                //Считаем заголовок фиктивным в случае если он есть в PivotData, но его нет в реальных
                //выбранных данных
                caption.IsDummy = (clsSection == null) || !clsSection.IsExistMeasure(caption.UniqueName);
            }
        }

        public void Initialize(DimensionCell[] columnsLeafs, PivotData pivotData)
        {
            if ((pivotData != null) && (pivotData.TotalAxis.VisibleTotals.Count > 0))
            {
                if (columnsLeafs.Length > 0)
                {
                    foreach (DimensionCell dimCell in columnsLeafs)
                    {
                        MeasuresSectionInfo clsSection;
                        if (dimCell.IsGrandTotal)
                            clsSection = this.Grid.PivotData.ClsInfo.LastMeasureSectionInfo;
                        else
                            //Ищем секцию показателей в селсете соответствующую данной ячейке
                            clsSection = this.Grid.PivotData.ClsInfo.GetSectionInfoByTupleUN(dimCell.TupleUN);

                        MeasuresCaptionsSection captionsSection = this.AddMeasuresCaptionsSection(dimCell, pivotData, clsSection);
                        //Выставим признаки фиктивности
                        this.DummyCheck(captionsSection, clsSection);
                        //Проставим индексы кортежей у заголовков
                        this.DetermineTuplesIndexs(captionsSection);
                    }
                }
                else
                {
                    MeasuresSectionInfo clsSection = (this.Grid.PivotData.ClsInfo.MeasuresSectionsInfo.Count > 0
                                                          ? this.Grid.PivotData.ClsInfo.MeasuresSectionsInfo[0]
                                                          : null);
                    MeasuresCaptionsSection captionsSection = this.AddMeasuresCaptionsSection(null, pivotData, clsSection);
                    //Выставим признаки фиктивности
                    this.DummyCheck(captionsSection, clsSection);
                    //Проставим индексы кортежей у заголовков
                    this.DetermineTuplesIndexs(captionsSection);
                }
                this.AfterInitialization();
            }
        }

        /// <summary>
        /// По указанным координатам ищем ячейку в коллекции
        /// </summary>
        /// <param name="mousePoint">Координата для поиска</param>
        /// <param name="isFindByOffsetBounds">Признак что поиск ячейки будет осуществлятся по относительным координатам, 
        ///с учетом скролирования.</param>
        /// <returns>CaptionCell (результат)</returns>
        public CaptionCell FindCaption(Point mousePoint, bool isFindByOffsetBounds)
        {
            if (this.IsEmpty)
                return null;

            if (isFindByOffsetBounds)
                mousePoint = new Point(mousePoint.X + this.Grid.HScrollBarState.Offset, mousePoint.Y);

            int sectionIndex  = this.FindIntersectItem(mousePoint);
            if ((sectionIndex >= 0) && (sectionIndex < this.Count))
            {
                MeasuresCaptionsSection section = this[sectionIndex];
                if (section.IsVisible)
                    return section.FindCaption(mousePoint);
            }
            return null;
        }

        /// <summary>
        /// В указанной области ищем ячейки
        /// </summary>
        /// <param name="searchBounds">Координаты для поиска</param>
        /// <param name="isFindByOffsetBounds">Признак что поиск ячейки будет осуществлятся по относительным координатам, 
        ///с учетом скролирования.</param>
        /// <returns></returns>
        public List<GridControl> FindCaptions(Rectangle searchBounds, bool isFindByOffsetBounds)
        {
            List<GridControl> result = new List<GridControl>();
            if (this.IsEmpty)
                return result;

            if (isFindByOffsetBounds)
                searchBounds = new Rectangle(searchBounds.X + this.Grid.HScrollBarState.Offset, searchBounds.Y, searchBounds.Width, searchBounds.Height);

            List<int> sectionIndexes = this.FindIntersectItems(searchBounds);

            foreach (int sectionIndex in sectionIndexes)
            {
                if ((sectionIndex >= 0) && (sectionIndex < this.Count))
                {
                    MeasuresCaptionsSection section = this[sectionIndex];
                    if (section.IsVisible)
                        result.AddRange(section.FindCaptions(searchBounds));
                }
            }
            return result;
        }



        /// <summary>
        /// Расчет абсолютных координат
        /// </summary>
        /// <param name="startPoint">Точка начала</param>
        public override void RecalculateCoordinates(System.Drawing.Point startPoint)
        {
            this.Location = startPoint;
            Point locationSection = startPoint;
            DimensionCell prepareVisibleParent = null;
            DimensionCell curentVisibleParent = null;
            bool sectionVisible;
            this.maxHeight = 0;
            foreach (MeasuresCaptionsSection section in this)
            {
                //получаем видимость секции, без учета ее фиктивности
                sectionVisible = section.GetVisibility();
                //Здесь тема такая: Если секция не видима(значит схлопнут один из верхних уровней) и у 
                //ячейки колонок к которой она принадлежит, есть хоть один видимый парент, у которого нет итога,
                //то считаем эту секцию видимой, но фиктивной, это значит что все данные отображающие в секции 
                //с данными будут пустой строкой
                curentVisibleParent = section.ColumnCell != null ? section.ColumnCell.GetVisibleParent() : null;
                section.IsDummy = !sectionVisible && (curentVisibleParent != null)
                    && !curentVisibleParent.Equals(prepareVisibleParent)
                    && !curentVisibleParent.IsExistTotal;
                prepareVisibleParent = curentVisibleParent;
                section.IsVisible = sectionVisible;

                section.RecalculateCoordinates(locationSection);
                locationSection.X += section.IsVisible ? section.Width : 0;
                if (section.IsVisible)
                    this.maxHeight = Math.Max(this.maxHeight, section.OriginalHeight);
            }
        }


        /// <summary>
        /// Возвращает индекс секции, которой принадлежит передающаяся параметром точка, поиск осуществляется 
        /// по абсолютным координатам(похожий метод есть у коллекции данных, но он ище по относительным коорд.)
        /// </summary>
        /// <param name="point"></param>
        /// <returns>int(индекс ячейки)</returns>
        public int FindIntersectItem(Point point)
        {
            int startIndex = 0;
            int endIndex = this.Count - 1;
            int middle;

            if (startIndex == endIndex)
                return startIndex;
            Rectangle range;
            do
            {
                middle = (int)((endIndex + startIndex) / 2);

                range = Rectangle.Union(this[startIndex].Bounds, this[middle].Bounds);
                if (range.Height == 0)
                    range.Height = point.Y - range.Y + 1;

                if (range.Contains(point))
                    endIndex = middle;
                else
                    startIndex = middle + 1;
            }
            while (startIndex != endIndex);

            return startIndex;
        }

        /// <summary>
        /// Возвращает список индексов секций, которые входят в указанную область searchBounds, поиск осуществляется 
        /// по абсолютным координатам(похожий метод есть у коллекции данных, но он ище по относительным коорд.)
        /// </summary>
        /// <param name="point"></param>
        /// <returns>int(индекс ячейки)</returns>
        public List<int> FindIntersectItems(Rectangle searchBounds)
        {
            List<int> result = new List<int>();
            int startIndex = 0;
            int endIndex = this.Count - 1;
            int middle;

            if (startIndex == endIndex)
            {
                result.Add(startIndex);
                return result;
            }
            Rectangle range;
            for (int i = startIndex; i <= endIndex; i++)
            {
                range = this[i].Bounds;
                if (range.Height == 0)
                    range.Height = searchBounds.Y - range.Y + 1;

                if (range.IntersectsWith(searchBounds))
                {
                    result.Add(i);
                }
            }
            /*
            do
                {
                    middle = (int)((endIndex + startIndex) / 2);

                    range = Rectangle.Union(this[startIndex].Bounds, this[middle].Bounds);
                    if (range.Height == 0)
                        range.Height = point.Y - range.Y + 1;

                    if (range.Contains(point))
                        endIndex = middle;
                    else
                        startIndex = middle + 1;
                }
                while (startIndex != endIndex);
            */
            return result;
        }


        /// <summary>
        /// Вычисляем диапазон секций которые надо рисовать (попадающие в видимую область), далее все рисуем.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public override void Draw(Graphics graphics, Painter painter)
        {
            Region oldClip = graphics.Clip;
            try
            {
                this.VisibleBounds = this.GetVisibleBounds();
                graphics.Clip = new Region(this.VisibleBounds);

                if (this.IsEmpty || (this.VisibleBounds.Width <= 0) || (this.VisibleBounds.Height <= 0))
                    return;

                //вычисляем диапазон индексов, видимых областей данных
                
                //точка начала, области видимости, т.к. метод ищет по абсолютным координатам, приплюсовываем смещение 
                //по горизонтали (тоесть из относительной координаты, делаем абсолютную)
                Point point = new Point(this.VisibleBounds.Location.X + this.Grid.HScrollBarState.Offset, this.VisibleBounds.Location.Y);
                int startIndex = this.FindIntersectItem(point);
                //точка конца, области видимости
                point = new Point(this.VisibleBounds.Right - 1 + this.Grid.HScrollBarState.Offset, this.VisibleBounds.Y);
                int endIndex = this.FindIntersectItem(point);

                if ((startIndex > this.Count - 1) || (endIndex > this.Count - 1))
                    return;
                MeasuresCaptionsSection captionSection;
                for (; startIndex <= endIndex; startIndex++)
                {
                    captionSection = this[startIndex];
                    if (captionSection.IsVisible)
                    {
                        captionSection.Draw(graphics, painter);
                    }
                }
            }
            finally
            {
                graphics.Clip = oldClip;
                graphics = null;
            }
        }

        /// <summary>
        /// Получить видимую область коллекции
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            //т.к. операция по вычислинею видимых границ заголовков показателей, достаточно трудоемка, будем считать, что
            //видимая ширина заголовков всегда равна видимой ширене экрана
            Point point = this.Location;
            Size size = new Size(this.Grid.GridBounds.Right - point.X, this.Grid.GridScale.GetScaledValue(this.Height) + 1);
            return new Rectangle(point, size);
        }

        /// <summary>
        /// Получить массив всех заголовков показателей
        /// </summary>
        /// <returns>CaptionCell[]</returns>
        public CaptionCell[] GetMeasuresCaptions()
        {
            List<CaptionCell> result = new List<CaptionCell>();
            foreach (MeasuresCaptionsSection section in this)
            {
                result.AddRange(section);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Очистка заголовков показателей
        /// </summary>
        public new void Clear()
        {
            //перед очисткой соберем сведения о ширине и высоте заголовков
            this.CaptionsHeight = this.Height;
            this.CaptionsWidths = this.GetWidths();

            foreach (MeasuresCaptionsSection section in this)
            {
                section.Clear();
            }
            base.Clear();
        }

        /// <summary>
        /// Установить ширину для каждого заголовка
        /// </summary>
        /// <param name="widths"></param>
        public void DetermineWidths(int[] widths)
        {
            int index = 0;
            foreach(MeasuresCaptionsSection section in this)
            {
                foreach(CaptionCell caption in section)
                {
                    if (index > widths.Length - 1)
                        return;
                    caption.Width = widths[index];
                    index++;
                }
            }
        }

        /// <summary>
        /// Получить ширину каждого заголовка
        /// </summary>
        /// <returns></returns>
        public int[] GetWidths()
        {
            List<int> result = new List<int>();
            foreach (MeasuresCaptionsSection section in this)
            {
                foreach (CaptionCell caption in section)
                {
                    result.Add(caption.OriginalSize.Width);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Загружаем некие настройки коллекции
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Load(XmlNode collectionNode, bool isLoadTemplate)
        {
            if (collectionNode == null)
                return;
            this.Style.Load(collectionNode.SelectSingleNode(GridConsts.style), isLoadTemplate);
            this.LoadSizes(collectionNode.SelectSingleNode(GridConsts.sizes));
        }

        /// <summary>
        /// Сохраняем настройки коллекции
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;
            this.Style.Save(XmlHelper.AddChildNode(collectionNode, GridConsts.style));
            this.SaveSizes(XmlHelper.AddChildNode(collectionNode, GridConsts.sizes));
        }

        /// <summary>
        /// Загружаем размеры элементов коллекции
        /// </summary>
        /// <param name="sizesNode"></param>
        private void LoadSizes(XmlNode sizesNode)
        {
            if (sizesNode == null)
                return;

            //Ширина
            string sWidths = XmlHelper.GetStringAttrValue(sizesNode, GridConsts.widths, "");
            //Парсим строку
            int[] widths = CommonUtils.ArrayFromString(sWidths, CommonUtils.separator);
            //Выставляем заголовкам полученные высоты
            this.DetermineWidths(widths);

            //У заголовков мер высота одинаковая
            this.Height = XmlHelper.GetIntAttrValue(sizesNode, GridConsts.heights, 0);
        }

        /// <summary>
        /// Сохраняем размеры элементов коллекции
        /// </summary>
        /// <param name="sizesNode"></param>
        private void SaveSizes(XmlNode sizesNode)
        {
            if (sizesNode == null)
                return;

            //Получаем ширину каждого из заголовка
            int[] widths = this.GetWidths();
            //Конвертируем в строку
            string sWidths = CommonUtils.ArrayToString(widths, CommonUtils.separator);
            //Сохраняем в атрибуте
            XmlHelper.SetAttribute(sizesNode, GridConsts.widths, sWidths);

            //Сохраняем высоту заголовков, она у всех одинокова
            XmlHelper.SetAttribute(sizesNode, GridConsts.heights, this.OriginalHeight.ToString());
        }

        /// <summary>
        /// Синхронизация данных таблицы с PivotData
        /// </summary>
        /// <param name="pivotData"></param>
        public void SynchronizePivotData(Data.PivotData pivotData)
        {
            if (pivotData == null)
                return;
            foreach (MeasuresCaptionsSection section in this)
            {
                section.SynchronizePivotData(pivotData);
            }
        }

        /// <summary>
        /// Установить всем секциям с заголовками показателей указанный стиль
        /// </summary>
        /// <param name="style"></param>
        public override void SetStyle(CellStyle style)
        {
            style.Grid = this.Grid;
            this.Style = style;
            foreach (MeasuresCaptionsSection section in this)
            {
                section.SetStyle(style);
            }
        }

        /// <summary>
        /// Ширина
        /// </summary>
        public override int Width
        {
            get
            {
                int width = 0;
                foreach (MeasuresCaptionsSection section in this)
                {
                    width += section.IsVisible ? section.Width : 0;
                }
                return width;
            }

            set
            {
                int sectionCount = this.Count;
                if (sectionCount > 0)
                {
                    MeasuresCaptionsSection lastSection = this[sectionCount - 1];
                    int newSectionWidth = (value + this.Location.X - lastSection.Location.X);
                    lastSection.Width = newSectionWidth;
                }
            }
        }

        /// <summary>
        /// Оригинальная высота
        /// </summary>
        public int OriginalHeight
        {
            get
            {
                if (this.Count > 0)
                    return this.Grid.GridScale.GetNonScaledValue(this.maxHeight);
                else
                    return 0;
            }
        }


        /// <summary>
        /// Высота
        /// </summary>
        public override int Height
        {
            get
            {
                if (this.Count > 0)
                    return this.maxHeight;
                else
                    return 0;
            }
            set
            {
                foreach (MeasuresCaptionsSection section in this)
                {
                    section.SetHeight(value);
                }
                this.maxHeight = value;
            }
        }

        /// <summary>
        /// Эталон стиля для заголовков показателей
        /// </summary>
        public CellStyle Style
        {
            get
            {
                return this._style;
            }
            set
            {
                this._style = value;
            }
        }

        /// <summary>
        /// Высота кнопок
        /// </summary>
        public int CaptionsHeight
        {
            get 
            { 
                return this._captionsHeight; 
            }
            set 
            { 
                this._captionsHeight = value; 
            }
        }

        /// <summary>
        /// Ширина каждого заголовка
        /// </summary>
        public int[] CaptionsWidths
        {
            get 
            { 
                return this._captionsWidths; 
            }
            set 
            { 
                this._captionsWidths = value; 
            }
        }
    }
}
