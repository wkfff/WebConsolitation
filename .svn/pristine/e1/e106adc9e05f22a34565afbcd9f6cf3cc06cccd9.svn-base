using System;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;
using System.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Коллекция заголовков строк
    /// </summary>
    public class RowCaptions : CaptionsList
    {
        //ширина каждого заголовка
        private int[] _captionsWidths = new int[0];
        //высота заголовка
        private int _captionsHeight = 0;

        public RowCaptions(ExpertGrid grid)
            : base(grid, CaptionType.Rows)
        {
        }

        /// <summary>
        /// Расчет абсолютных координат
        /// </summary>
        /// <param name="startPoint">Точка начала</param>
        public override void RecalculateCoordinates(Point startPoint)
        {
            base.Location = startPoint;
            Point locationCaption = startPoint;

            //минимальная высота заголовка меры
            int minMeasureCaptionHeight = this.Grid.MeasureCaptionsSections.IsEmpty ? 0 :
                this.Grid.MeasureCaptionsSections.Style.TextHeight;
            //минимальная высота заголовка строки не должна быть меньше высоты заголовка меры
            int minRowCaptionHeight = Math.Max(this.Style.TextHeight, minMeasureCaptionHeight);

            if (this.Height < minRowCaptionHeight)
                this.Height = minRowCaptionHeight;

            foreach (CaptionCell caption in this)
            {
                caption.Location = locationCaption;
                locationCaption.X += caption.Width;
            }
        }

        /// <summary>
        /// Видимая область заголовков
        /// </summary>
        /// <returns>Rectangle</returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = base.Bounds;
            result.Height = Math.Min(base.Grid.GridBounds.Bottom - result.Location.Y - 1, result.Height + 1);
            result.Width = Math.Min(base.Grid.GridBounds.Right - result.Location.X - 1, result.Width + 1);
            return result;
        }

        public new void Clear()
        {
            //Сохраним высоту и ширину заголовков
            this.CaptionsHeight = this.OriginalHeight;
            this.CaptionsWidths = base.GetWidths();

            base.Clear();
        }

        private void AfterInitialization()
        {
            base.SetWidths(this.CaptionsWidths);
            this.Height = this.CaptionsHeight;
        }

        /// <summary>
        /// Инициализация по информации извлеченной из селсета
        /// </summary>
        /// <param name="clsInfo"></param>
        public override void Initialize(CellSetInfo clsInfo)
        {
            if (clsInfo != null)
            {
                if (clsInfo.RowsInfo != null)
                {
                    foreach (DimensionInfo dim in clsInfo.RowsInfo)
                    {
                        foreach (LevelInfo lev in dim.LevelsInfo)
                        {
                            this.Add(new CaptionCell(this, lev.Caption, lev.UniqueName, dim.UniqueName, SortType.None));
                        }
                    }
                }
                this.AfterInitialization();
            }
        }


        /// <summary>
        /// Инициализация по PivotData
        /// </summary>
        /// <param name="pivotData"></param>
        public override void Initialize(Data.PivotData pivotData)
        {
            if (pivotData != null)
            {
                foreach (FieldSet fieldSet in pivotData.RowAxis.FieldSets)
                {
                    foreach (PivotField pivotField in fieldSet)
                    {
                        //сначала смотрим есть ли сортировка у оси или у измерения
                        SortType sortType = pivotData.RowAxis.SortType != SortType.None ?
                            pivotData.RowAxis.SortType : fieldSet.SortType;
                        //если признак сортировки не стоит ни у оси, ни у измерения, берем сортировку 
                        //уровня
                        sortType = sortType != SortType.None ? sortType : pivotField.SortType;

                        this.Add(new CaptionCell(this, pivotField.Caption, pivotField.UniqueName,
                            fieldSet.UniqueName, sortType));
                    }
                }
                this.AfterInitialization();
            }
        }

        /// <summary>
        /// Загружаем размеры элементов коллекции
        /// </summary>
        /// <param name="sizesNode"></param>
        protected override void LoadSizes(XmlNode sizesNode)
        {
            if (sizesNode == null)
                return;

            //Ширина
            string sWidths = XmlHelper.GetStringAttrValue(sizesNode, GridConsts.widths, "");
            //Парсим строку
            int[] widths = CommonUtils.ArrayFromString(sWidths, CommonUtils.separator);
            //Выставляем заголовкам полученные высоты
            base.SetWidths(widths);

            //У заголовков строк высота одинаковая
            this.Height = XmlHelper.GetIntAttrValue(sizesNode, GridConsts.heights, 0);
        }

        /// <summary>
        /// Сохраняем размеры элементов коллекции
        /// </summary>
        /// <param name="sizesNode"></param>
        protected override void SaveSizes(XmlNode sizesNode)
        {
            if (sizesNode == null)
                return;

            //Получаем ширину каждого из заголовка
            int[] widths = base.GetWidths();
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
        public override void SynchronizePivotData(Data.PivotData pivotData)
        {
            if (pivotData == null)
                return;
            foreach (CaptionCell captionCell in this)
            {
                //находим измерение
                FieldSet fieldSet = pivotData.RowAxis.FieldSets.GetFieldSetByName(captionCell.HierarchyUN);
                if (fieldSet != null)
                {
                    //в измерение ищем уровень
                    PivotField field = fieldSet.GetFieldByName(captionCell.UniqueName);
                    if (field != null)
                        captionCell.Text = field.Caption;
                }
            }
        }

        /// <summary>
        /// Граница
        /// </summary>
        public Rectangle OriginalBounds
        {
            get
            {
                Size size = new Size(this.OriginalWidth, this.OriginalHeight);
                return new Rectangle(this.Location, size);
            }
        }

        /// <summary>
        /// Оригинальная ширина
        /// </summary>
        public int OriginalWidth
        {
            get
            {
                int width = 0;
                foreach (CaptionCell caption in this)
                {
                    width += caption.OriginalWidth;
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
                foreach (CaptionCell caption in this)
                {
                    width += caption.Width;
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
        /// Оригинальная высота 
        /// </summary>
        public int OriginalHeight
        {
            get
            {
                if (this.Count > 0)
                    return this[0].OriginalHeight;
                else
                    return 0;
            }
            set
            {
                foreach (CaptionCell caption in this)
                {
                    caption.Height = value;
                }
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
                    return this[0].Height;
                else
                    return 0;
            }
            set
            {        
                foreach (CaptionCell caption in this)
                {
                    caption.Height = value;
                }
            }
        }

        /// <summary>
        /// Ширина каждого из заголовков
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

        /// <summary>
        /// Высота заголовка
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
    }
}
