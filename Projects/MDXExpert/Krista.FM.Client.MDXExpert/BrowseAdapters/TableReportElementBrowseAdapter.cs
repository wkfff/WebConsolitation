using System.ComponentModel;
using System.Drawing;
using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using System;

namespace Krista.FM.Client.MDXExpert
{
    class TableReportElementBrowseAdapter : CustomReportElementBrowseAdapter
    {
        #region Поля
        
        private IGridUserInterface gridUserInterface;
        private CellCommentsBrowseClass cellCommentsBrowse;
        private TableReportElement reportElement;

        #endregion

        public TableReportElementBrowseAdapter(DockableControlPane dcPane)
            : base(dcPane)
        {
            reportElement = (TableReportElement)dcPane.Control;
            gridUserInterface = reportElement.GridUserInterface;

            cellCommentsBrowse = new CellCommentsBrowseClass(gridUserInterface);
        }

        /// <summary>
        /// Если таблица строиться по пользовательскому MDX запросу
        /// </summary>
        [Browsable(false)]
        public bool IsCustomMdxQuery
        {
            get { return this.reportElement.PivotData.IsCustomMDX; }
        }

        #region Свойства

        [Category("Управление данными")]
        [Description("Данные в таблицу загружаются по мере необходимости, при раскрытии уровней")]
        [DisplayName("Динамическая загрузка данных")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsCustomMdxQuery", "False")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool DynamicLoadData
        {
            get { return this.reportElement.PivotData.DynamicLoadData; }
            set { this.reportElement.PivotData.DynamicLoadData = value; }
        }

        /*
        //тестовое свойство для проверки поведения приложения при окончании сессии
        [Category("Управление данными")]
        [DisplayName("Сессия")]
        [Browsable(true)]
        public int SessionID
        {
            get
            {
                Random r = new Random();
                return r.Next();
            }
            set { this.reportElement.PivotData.Cube.ParentConnection.Close(); }
        }
        */

        [Category("Вид")]
        [Description("Высота разделителя между областями таблицы")]
        [DisplayName("Высота разделителя")]
        [Browsable(true)]
        public string SeparatorHeight
        {
            get
            {
                return gridUserInterface.OriginalSeparatorHeight.ToString();
            }
            set
            {
                byte bValue;
                if (byte.TryParse(value, out bValue))
                    gridUserInterface.OriginalSeparatorHeight = bValue;
                else
                    FormException.ShowErrorForm(new Exception("MDXExpert-PropertyGrid-SeparatorHeight"));
            }
        }

        //appearance(вид)

        //комментарий к ячейкам
        [Category("Вид")]
        [Description("Комментарий к ячейкам")]
        [DisplayName("Комментарий к ячейкам")]
        [Browsable(true)]
        public CellCommentsBrowseClass CellCommentsBrowse
        {
            get { return cellCommentsBrowse; }
            set { cellCommentsBrowse = value; }
        }

        //цвет выделенной ячейки
        [Category("Вид")]
        [Description("Цвет выделенной ячейки")]
        [DisplayName("Цвет выделенной ячейки")]
        [Browsable(true)]
        public Color HighLightColor
        {
            get { return gridUserInterface.HighLightColor; }
            set { gridUserInterface.HighLightColor = value; }
        }

        [Category("Вид")]
        [Description("Масштаб таблицы в процентах (значения от 30 до 300)")]
        [DisplayName("Масштаб")]
        [DefaultValue(100)]
        [Browsable(true)]
        public int Scale
        {
            get { return gridUserInterface.GridPercentScale; }
            set { gridUserInterface.GridPercentScale = value; }
        }

        #endregion
    }


    #region Дополнительные классы для отображения в PropertyGrid

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CellCommentsBrowseClass
    {
        private IGridUserInterface gridUserInterface;

        public CellCommentsBrowseClass(IGridUserInterface gridUserInterface)
        {
            this.gridUserInterface = gridUserInterface;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region Свойства
        /// <summary>
        /// Задержка перед показом
        /// </summary>
        [Category("Комментарий к ячейкам")]
        [Description("Задержка перед показом комментария (мс)")]
        [DisplayName("Задержка перед показом(мс)")]
        [Browsable(true)]
        public string CommentDisplayDelay
        {
            get { return gridUserInterface.CommentDisplayDelay.ToString(); }
            set
            {
                int i;
                if (int.TryParse(value, out i) && (i >= 0) && (i <= 10000))
                {
                    gridUserInterface.CommentDisplayDelay = i;
                }
                else
                {
                    FormException.ShowErrorForm(new Exception("MDXExpert-PropertyGrid-CommentDisplayDellay."),
                        ErrorFormButtons.WithoutTerminate);
                }
            }
        }

        /// <summary>
        /// Видимость
        /// </summary>
        [Category("Комментарий к ячейкам")]
        [Description("Показывать комментарии")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool IsDisplayComments
        {
            get { return gridUserInterface.IsDisplayComments; }
            set { gridUserInterface.IsDisplayComments = value; }
        }

        /// <summary>
        /// Максимальная ширина
        /// </summary>
        [Category("Комментарий к ячейкам")]
        [Description("Максимальная ширина комментария")]
        [DisplayName("Максимальная ширина")]
        [Browsable(true)]
        public string CommentMaxWidth
        {
            get { return gridUserInterface.CommentMaxWidth.ToString(); }
            set
            {
                int i;
                if (int.TryParse(value, out i) && (i >= 0) && (i <= 10000))
                {
                    gridUserInterface.CommentMaxWidth = i;
                }
                else
                {
                    FormException.ShowErrorForm(new Exception("MDXExpert-PropertyGrid-CommentMaxWidth."),
                        ErrorFormButtons.WithoutTerminate);
                }
            }
        }

        /// <summary>
        /// Отображать комментарий к ячейке, до ее смены
        /// </summary>
        [Category("Комментарий к ячейкам")]
        [Description("Комментарий к ячейке будет отображаться до тех пор, пока курсор находится в ее области.")]
        [DisplayName("Отображать до смены ячейки")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool DisplayCommentUntilControlChange
        {
            get { return gridUserInterface.DisplayCommentUntilControlChange; }
            set { gridUserInterface.DisplayCommentUntilControlChange = value; }
        }
        #endregion
    }
    #endregion
}
