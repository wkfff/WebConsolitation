using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Data;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Общие свойства осей
    /// </summary>
    public class PivotAxisBrowseAdapter : PivotObjectBrowseAdapterBase
    {

        public PivotAxisBrowseAdapter(PivotAxis pivotAxis, CustomReportElement reportElement)
            : base(pivotAxis, pivotAxis.Caption, reportElement)
        {
        }

        #region свойства

        protected PivotAxis CurrentPivotObject
        {
            get { return (PivotAxis)base.PivotObject; }
        }

        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(false)]
        public AxisType CurrentAxisType
        {
            get { return this.CurrentPivotObject.AxisType; }
        }

        /// <summary>
        /// Вернет True, если ось строк или колонок в элементе построенном не по 
        /// пользовательскому MDX запросу
        /// </summary>
        [Browsable(false)]
        public bool IsRowsOrColumnAxisNotCustomMDX
        {
            get
            {
                return (this.CurrentAxisType == AxisType.atRows || this.CurrentAxisType == AxisType.atColumns)
                    && !this.IsCustomMDX;
            }
        }

        /// <summary>
        /// Если элемент отчета таблица и она построена не по пользовательскому запросу
        /// и ось строк вернет - true
        /// </summary>
        [Browsable(false)]
        public bool IsTableRowAxisNotCustomMDX
        {
            get
            {
                if ((this.ReportElement != null) && (this.ReportElement is TableReportElement)
                    && !this.IsCustomMDX)
                {
                    return this.CurrentPivotObject.AxisType == AxisType.atRows;
                }
                return false;
            }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок оси")]
        public string Caption
        {
            get { return this.CurrentPivotObject.Caption; }
        }

        [Category("Поведение")]
        [DisplayName("Скрывать пустые")]
        [Description("Скрывать элементы, для которых отсутствуют данные показателей")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("IsRowsOrColumnAxisNotCustomMDX", "True")]
        public bool HideEmptyPositions
        {
            get { return this.CurrentPivotObject.HideEmptyPositions; }
            set { this.CurrentPivotObject.HideEmptyPositions = value; }
        }

        [Category("Управление данными")]
        [DisplayName("Количество строк на странице")]
        [Description("Определяет максимальное количество строк, отображаемых на странице в таблице.")]
        [DefaultValue(TablePager.DefaultPageSize)]
        [DynamicPropertyFilter("IsTableRowAxisNotCustomMDX", "True")]
        public int MaxTablePageSize
        {
            get { return ((TableReportElement)this.ReportElement).TablePager.PageSize; }
            set { ((TableReportElement)this.ReportElement).TablePager.PageSize = value; }
        }

        [Category("Управление данными")]
        [DisplayName("Сортировка")]
        [Description("Вид сортировки оси")]
        [DefaultValue(SortType.None)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("IsRowsOrColumnAxisNotCustomMDX", "True")]
        public SortType AxisSortType
        {
            get { return this.CurrentPivotObject.SortType; }
            set 
            {
                /*
                if ((this.PivotDataType == PivotDataType.Table)
                    && ((value == SortType.BDESC) || (value == SortType.BASC)))
                {
                    DialogResult dialogResult = MessageBox.Show(Common.Consts.sortWarning, "MDXExpert ", System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.No)
                        return;
                }*/
                this.CurrentPivotObject.SortType = value; 
            }
        }
       #endregion свойства
    }
}