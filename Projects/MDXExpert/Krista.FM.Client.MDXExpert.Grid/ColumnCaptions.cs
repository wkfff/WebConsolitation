using System;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;
using System.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    public class ColumnCaptions : CaptionsList
    {
        private int _captionsWidth = 0;//ширина заголовка
        private int[] _captionsHeights = new int[0];//высота каждого заголовка

        public ColumnCaptions(ExpertGrid grid)
            : base(grid, CaptionType.Columns)
        {
        }

        public override void RecalculateCoordinates(Point startPoint)
        {
            base.Location = startPoint;
            Point locationCaption = startPoint;
            for (int i = 0; i < this.Count; i++)
            {
                CaptionCell caption = this[i];
                caption.Location = locationCaption;

                //минимальная высота ячейки колонок
                int minColumnCellHeigth = this.Grid.Column.GetMemPropHeight(caption.UniqueName)
                    + this.Grid.Column.Style.OriginalTextHeight;

                //минимальная высота заколовка колонки не должна быть меньше минимальной высоты ячейки колонки
                int minColumnCaptionHeight = Math.Max(this.Style.OriginalTextHeight, minColumnCellHeigth);
                if (caption.OriginalSize.Height < minColumnCaptionHeight)
                    caption.Height = minColumnCaptionHeight;

                locationCaption.Y += caption.Height;
            }
        }

        public new void Clear()
        {
            //Сохраним высоту и ширину заголовков
            this.CaptionsHeights = base.GetHeights();
            this.CaptionsWidth = this.OriginalWidth;

            base.Clear();
        }

        private void AfterInitialization()
        {
            base.SetHeights(this.CaptionsHeights);
            this.Width = this.CaptionsWidth;
        }

        /// <summary>
        /// Инициализациия по информации извлеченной из селсета
        /// </summary>
        /// <param name="clsInfo"></param>
        public override void Initialize(CellSetInfo clsInfo)
        {
            if (clsInfo != null)
            {
                if (clsInfo.ColumnsInfo != null)
                {
                    foreach (DimensionInfo dim in clsInfo.ColumnsInfo)
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
                foreach (FieldSet fieldSet in pivotData.ColumnAxis.FieldSets)
                {
                    foreach (PivotField pivotField in fieldSet)
                    {
                        //сначала смотрим есть ли сортировка у оси или у измерения
                        SortType sortType = pivotData.ColumnAxis.SortType != SortType.None ?
                            pivotData.ColumnAxis.SortType : fieldSet.SortType;
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

        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = base.Bounds;
            result.Height = Math.Min(base.Grid.GridBounds.Bottom - result.Location.Y - 1, result.Height + 1);
            result.Width = Math.Min(base.Grid.GridBounds.Right - result.Location.X - 1, result.Width + 1);
            return result;
        }

        /// <summary>
        /// Загружаем размеры элементов коллекции
        /// </summary>
        /// <param name="sizesNode"></param>
        protected override void LoadSizes(XmlNode sizesNode)
        {
            if (sizesNode == null)
                return;
            //У заголовков столбцов ширина одинаковая
            this.Width = XmlHelper.GetIntAttrValue(sizesNode, GridConsts.widths, 0);
            
            //А вот высота может быть разной
            string sHeights = XmlHelper.GetStringAttrValue(sizesNode, GridConsts.heights, "");
            //Парсим строку
            int[] heights = CommonUtils.ArrayFromString(sHeights, CommonUtils.separator);
            //Выставляем заголовкам полученные высоты
            base.SetHeights(heights);
        }

        /// <summary>
        /// Сохраняем размеры элементов коллекции
        /// </summary>
        /// <param name="sizesNode"></param>
        protected override void SaveSizes(XmlNode sizesNode)
        {
            if (sizesNode == null)
                return;
            //Сохраняем ширину заголовков, она у всех одинокова
            XmlHelper.SetAttribute(sizesNode, GridConsts.widths, this.OriginalWidth.ToString());

            //Получаем высоту каждого из заголовка
            int[] heights = base.GetHeights();
            //Конвертируем в строку
            string sHeights = CommonUtils.ArrayToString(heights, CommonUtils.separator);
            //Сохраняем в атрибуте
            XmlHelper.SetAttribute(sizesNode, GridConsts.heights, sHeights);
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
                FieldSet fieldSet = pivotData.ColumnAxis.FieldSets.GetFieldSetByName(captionCell.HierarchyUN);
                if (fieldSet != null)
                {
                    //в измерение ищем уровень
                    PivotField field = fieldSet.GetFieldByName(captionCell.UniqueName);
                    if (field != null)
                        captionCell.Text = field.Caption;
                }
            }
        }

        public int OriginalWidth
        {
            get
            {
                if (this.Count > 0)
                    return this[0].OriginalWidth;
                else
                    return 0;
            }
        }


        public override int Width
        {
            get
            {
                if (this.Count > 0)
                    return this[0].Width;
                else
                    return 0;
            }
            set
            {
                foreach (CaptionCell caption in this)
                {
                    caption.Width = value;
                }
            }
        }


        public int OriginalHeight
        {
            get
            {
                int height = 0;
                foreach (CaptionCell caption in this)
                {
                    height += caption.Height;
                }
                return height;
            }
        }


        public override int Height
        {
            get
            {
                int height = 0;
                foreach (CaptionCell caption in this)
                {
                    height += caption.Height;
                }
                return height;
            }
            set
            {
                int captionsCount = this.Count;
                if (captionsCount > 0)
                {
                    CaptionCell lastCaption = this[captionsCount - 1];
                    int newCaptionHeight = (value + this.Location.Y - lastCaption.Location.Y);
                    lastCaption.Width = newCaptionHeight;
                }
            }
        }

        //Ширина заголовка
        public int CaptionsWidth
        {
            get 
            { 
                return this._captionsWidth; 
            }
            set 
            { 
                this._captionsWidth = value; 
            }
        }

        //Ширина каждого из заголовков
        public int[] CaptionsHeights
        {
            get 
            { 
                return this._captionsHeights; 
            }
            set 
            {
                this._captionsHeights = value; 
            }
        }
    }
}
