using System.ComponentModel;
using System.Drawing.Design;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.CommonClass;
using Krista.FM.Client.MDXExpert.Data;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Свойства меры
    /// </summary>
    public class PivotTotalBrowseAdapter : PivotObjectBrowseAdapterBase
    {

        public PivotTotalBrowseAdapter(PivotTotal pivotTotal, CustomReportElement reportElement)
            : base(pivotTotal, pivotTotal.Caption, reportElement)
        {
        }

        #region свойства

        private PivotTotal CurrentPivotObject
        {
            get { return (PivotTotal)base.PivotObject; }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок меры")]
        public string Caption
        {
            get 
            {
                return this.CurrentPivotObject.Caption; 
            }
            set 
            {
                if (this.CurrentPivotObject.ParentPivotData.IsTotalNameExists(value))
                {
                    MessageBox.Show(
                        string.Format("Мера с именем \"{0}\" уже существует. Задайте другое имя.", value),
                        "MDXExpert 3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                base.Header = value;
                this.CurrentPivotObject.Caption = value;
                if(this.CurrentPivotObject.IsCustomTotal)
                {
                    this.CurrentPivotObject.ParentPivotData.SetSelection(SelectionType.SingleObject, this.UniqueName);
                }
            }
        }


        [Category("Общие")]
        [DisplayName("Уникальное имя")]
        [Description("Уникальное имя меры")]
        public string UniqueName
        {
            get 
            {
                return this.CurrentPivotObject.UniqueName; 
            }
        }

        [Browsable(false)]
        public bool IsShowCustomTotalProperties
        {
            get
            {
                return this.CurrentPivotObject.IsCustomTotal && !this.IsCustomMDX;
            }
        }

        [Browsable(false)]
        public bool ShowSortType
        {
            get
            {
                return (this.ElementType != ReportElementType.eChart) && (this.ElementType != ReportElementType.eMultiGauge) && !this.IsCustomMDX ;
            }
        }


        [Category("Общие")]
        [DisplayName("Формула")]
        [Description("Формула вычисляемой меры")]
        [Editor(typeof(TotalExpressionEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("IsShowCustomTotalProperties", "True")]
        public PivotTotal Expression
        {
            get
            {
                return this.CurrentPivotObject;
            }
        }

        [Category("Общие")]
        [DisplayName("Сортировка")]
        [Description("Вид сортировки меры")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(Data.SortType.None)]
        [DynamicPropertyFilter("ShowSortType", "True")]
        public Data.SortType SortType
        {
            get
            {
                return this.CurrentPivotObject.SortType;
            }
            set
            {
                if ((this.PivotDataType == PivotDataType.Table)
                    && ((value == SortType.BDESC) || (value == SortType.BASC)))
                {
                    DialogResult dialogResult = MessageBox.Show(Common.Consts.sortWarning, "MDXExpert ", System.Windows.Forms.MessageBoxButtons.YesNo, 
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.No)
                        return;
                }
                this.CurrentPivotObject.SortType = value;
            }
        }


        [Category("Общие")]
        [DisplayName("Отображать в отчете")]
        [Description("Отображать в отчете")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool IsVisible
        {
            get
            {
                return this.CurrentPivotObject.IsVisible;
            }
            set
            {
                this.CurrentPivotObject.IsVisible = value;
            }
        }


        #endregion
    }
}
