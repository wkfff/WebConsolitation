using System.ComponentModel;
using System.Drawing;
using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Client.Common.Forms;
using System;
using Infragistics.Win;

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

        #region Свойства

        [Category("Вид")]
        [Description("Высота разделителя между областями таблицы")]
        [DisplayName("Высота разделителя")]
        [Browsable(true)]
        public string SeparatorHeight
        {
            get
            {
                return gridUserInterface.SeparatorHeight.ToString();
            }
            set
            {
                byte bValue;
                if (byte.TryParse(value, out bValue))
                    gridUserInterface.SeparatorHeight = bValue;
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
