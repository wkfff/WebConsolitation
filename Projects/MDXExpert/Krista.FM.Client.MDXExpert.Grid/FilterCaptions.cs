using System;
using System.Collections.Generic;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;
using System.Xml;
using System.ComponentModel;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Коллекция заголовков фильтров
    /// </summary>
    public class FilterCaptions : CaptionsList
    {
        private int[] _captionsWidths = new int[0];//ширина каждого заголовка
        private int _captionsHeight = 0;//высота заголовка
        private int _tipDisplayMaxMemberCount = 10;
        private DisplayMemberCount _displayMemberCount;
        private bool _isCaptionIncludeParents;
        CellStyle _valueCellStyle;

        public FilterCaptions(ExpertGrid grid)
            : base(grid, CaptionType.Filters)
        {
            this.ValueCellStyle = new CellStyle(this.Grid, Color.White, Color.White, Color.Black, Color.Black);
        }

        /// <summary>
        /// Вычисление абсолютных координат всех заголовков фильтров
        /// </summary>
        /// <param name="startPoint"></param>
        public override void RecalculateCoordinates(Point startPoint)
        {
            base.Location = startPoint;
            Point locationCaption = startPoint;
            this.SetOptionalCellHeight();
            this.CheckMinHeightCell();

            foreach (CaptionCell caption in this)
            {
                caption.Location = locationCaption;
                locationCaption.X += caption.Width;
            }
        }

        /// <summary>
        /// Выставляем высоту опциональным ячейкам
        /// </summary>
        private void SetOptionalCellHeight()
        {
            int optionalCellMaxHeight = 0;

            Graphics graphics = base.Grid.GetGridGraphics();
            //Узнаем максимальную высоту опциональной ячейки
            foreach (CaptionCell caption in this)
            {
                optionalCellMaxHeight = Math.Max(optionalCellMaxHeight,
                    CommonUtils.GetStringHeight(graphics, caption.OptionalCell.Text, 
                    this.ValueCellStyle.OriginalFont, 100000));
            }

            //Выставляем ее
            foreach (CaptionCell caption in this)
            {
                caption.OptionalCell.Height = optionalCellMaxHeight;
            }
        }

        /// <summary>
        /// Следит что бы высота ячейки не была меньше размера шрифта
        /// </summary>
        private void CheckMinHeightCell()
        {
            //высота опциональной ячейки
            int optionalCellHeight = this.IsEmpty ? 0 : this[0].OptionalCell.OriginalSize.Height;
            //если высота ячейки без высоты опциональной ячейки, меньше чем размер шрифта, удленим
            //ее на нужное количество пикселей
            if ((this.OriginalHeight - optionalCellHeight) < this.Style.OriginalTextHeight)
                this.Height = this.Style.OriginalTextHeight + optionalCellHeight;
        }

        public new void Clear()
        {
            //Сохраним высоту и ширину заголовков
            this.CaptionsHeight = this.OriginalHeight;
            this.CaptionsWidths = base.GetWidths();

            base.Clear();
        }

        public override void Load(XmlNode collectionNode, bool isLoadTemplate)
        {
            base.Load(collectionNode, isLoadTemplate);

            if (collectionNode == null)
                return;
            this.ValueCellStyle.Load(collectionNode.SelectSingleNode(GridConsts.optionalCellStyle),
                isLoadTemplate);
            this.TipDisplayMaxMemberCount = XmlHelper.GetIntAttrValue(collectionNode,
                GridConsts.tipDisplayMaxMemberCount, 10);
            this.DisplayMemberCount = (DisplayMemberCount)Enum.Parse(typeof(DisplayMemberCount),
                XmlHelper.GetStringAttrValue(collectionNode, GridConsts.displayMemberCount, "One"));
            this.IsCaptionIncludeParents = XmlHelper.GetBoolAttrValue(collectionNode,
                GridConsts.isCaptionIncludeParents, false);

        }

        public override void Save(XmlNode collectionNode)
        {
            base.Save(collectionNode);

            if (collectionNode == null)
                return;
            this.ValueCellStyle.Save(XmlHelper.AddChildNode(collectionNode, GridConsts.optionalCellStyle));
            XmlHelper.SetAttribute(collectionNode, GridConsts.tipDisplayMaxMemberCount,
                this.TipDisplayMaxMemberCount.ToString());
            XmlHelper.SetAttribute(collectionNode, GridConsts.displayMemberCount, 
                this.DisplayMemberCount.ToString());
            XmlHelper.SetAttribute(collectionNode, GridConsts.isCaptionIncludeParents,
                this.IsCaptionIncludeParents.ToString());
        }

        private void AfterInitialization()
        {
            base.SetWidths(this.CaptionsWidths);
            //Прежде чем установить высоту коллекции, надо выставить высоту опциональных ячеек
            this.SetOptionalCellHeight();
            this.Height = this.CaptionsHeight;
        }

        public CaptionCell FindCaption(Point mousePoint, bool isFindByOffsetBounds)
        {
            if (isFindByOffsetBounds)
                mousePoint = new Point(mousePoint.X + this.Grid.FiltersScrollBarState.Offset, mousePoint.Y);
            return base.FindCaption(mousePoint);
        }

        public List<GridControl> FindCaptions(Rectangle searchBounds, bool isFindByOffsetBounds)
        {
            if (isFindByOffsetBounds)
                searchBounds = new Rectangle(searchBounds.X + this.Grid.FiltersScrollBarState.Offset, searchBounds.Y, searchBounds.Width, searchBounds.Height);
            return base.FindCaptions(searchBounds);
        }


        /// <summary>
        /// Инициализация по инфорамации извлеченной из селсета
        /// </summary>
        /// <param name="clsInfo"></param>
        public override void Initialize(CellSetInfo clsInfo)
        {
            //!!!! Пока не инициализируем
            return;
            this.AfterInitialization();
        }

        /// <summary>
        /// Инициализация по пивот дата
        /// </summary>
        /// <param name="pivotData"></param>
        public override void Initialize(Data.PivotData pivotData)
        {
            if (pivotData != null)
            {
                int displayMemberCount = this.GetDisplayMemberCount(this.DisplayMemberCount);
                foreach (FieldSet fieldSet in pivotData.FilterAxis.FieldSets)
                {
                    CaptionCell caption = new CaptionCell(this, fieldSet.Caption, fieldSet.UniqueName);
                    fieldSet.InitializeMemberNames();
                    caption.OptionalCell.Text = fieldSet.GetMembersDefinition(this.IsCaptionIncludeParents, displayMemberCount);
                    this.Add(caption);
                }
                this.AfterInitialization();
            }
        }

        private int GetDisplayMemberCount(DisplayMemberCount value)
        {
            switch (value)
            {
                case DisplayMemberCount.One: return 1;
                case DisplayMemberCount.Five: return 5;
                case DisplayMemberCount.Ten: return 10;
                case DisplayMemberCount.Twenty: return 20;
                //большее количество просто не поместиться на экран...
                case DisplayMemberCount.All: return 100; 
            }
            return 1;
        }

        /// <summary>
        /// Видимая часть заголовков фильтров
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = base.Bounds;
            result.Height += 1;
            //из ширины вычитаем  ширину filtersScrollBar
            result.Width = this.Grid.GridBounds.Width - 27;
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

            //Ширина
            string sWidths = XmlHelper.GetStringAttrValue(sizesNode, GridConsts.widths, "");
            //Парсим строку
            int[] widths = CommonUtils.ArrayFromString(sWidths, CommonUtils.separator);
            //Выставляем заголовкам полученные высоты
            base.SetWidths(widths);

            //У заголовков фильтров высота одинаковая
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

            int height = this.Visible ? this.OriginalHeight : !this.IsEmpty ? this[0].OriginalHeight : 0;
            //Сохраняем высоту заголовков, она у всех одинокова
            XmlHelper.SetAttribute(sizesNode, GridConsts.heights, height.ToString());
        }

        /// <summary>
        /// Синхронизация данных таблицы с PivotData
        /// </summary>
        /// <param name="pivotData"></param>
        public override void SynchronizePivotData(Data.PivotData pivotData)
        {
            if (pivotData == null)
                return;

            int displayMemberCount = this.GetDisplayMemberCount(this.DisplayMemberCount);
            foreach (CaptionCell captionCell in this)
            {
                //находим измерение
                FieldSet fieldSet = pivotData.FilterAxis.FieldSets.GetFieldSetByName(captionCell.UniqueName);
                if (fieldSet != null)
                {
                    captionCell.Text = fieldSet.Caption;
                    captionCell.OptionalCell.Text = fieldSet.GetMembersDefinition(this.IsCaptionIncludeParents, displayMemberCount);
                }
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
                if (this.Visible)
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
        /// Оригинальная высота
        /// </summary>
        public int OriginalHeight
        {
            get
            {
                if ((this.Count > 0) && this.Visible)
                    return this[0].OriginalSize.Height;
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
                if ((this.Count > 0) && this.Visible)
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

        /// <summary>
        /// Стиль отображения ячейки со значением
        /// </summary>
        public CellStyle ValueCellStyle
        {
            get { return _valueCellStyle; }
            set { _valueCellStyle = value; }
        }

        /// <summary>
        /// Максимально количество элементов отображающееся в подсказке к фильтрам
        /// </summary>
        public int TipDisplayMaxMemberCount
        {
            get { return _tipDisplayMaxMemberCount; }
            set 
            {
                if (value >= 0)
                    _tipDisplayMaxMemberCount = value; 
            }
        }

        /// <summary>
        /// Количество отображаемых элементов в значении фильтра
        /// </summary>
        public DisplayMemberCount DisplayMemberCount
        {
            get { return _displayMemberCount; }
            set { _displayMemberCount = value; }
        }

        /// <summary>
        /// Отображать предков в заголовков элементов фильтра
        /// </summary>
        public bool IsCaptionIncludeParents
        {
            get { return _isCaptionIncludeParents; }
            set { _isCaptionIncludeParents = value; }
        }
    }

    /// <summary>
    /// Количество отображаемых элементов в значении фильтра
    /// </summary>
    public enum DisplayMemberCount
    {
        [Description("1")]
        One,
        [Description("5")]
        Five,
        [Description("10")]
        Ten,
        [Description("20")]
        Twenty,
        [Description("Все")]
        All
    }
}
