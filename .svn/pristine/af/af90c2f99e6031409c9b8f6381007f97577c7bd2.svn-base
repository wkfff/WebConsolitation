using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Общие свойства мер
    /// </summary>
    public class TotalAxisBrowseAdapter : PivotObjectBrowseAdapterBase
    {
        public TotalAxisBrowseAdapter(TotalAxis totalAxis, CustomReportElement reportElement)
            : base(totalAxis, totalAxis.Caption, reportElement)
        {
        }

        #region свойства

        private TotalAxis CurrentPivotObject
        {
            get { return (TotalAxis)base.PivotObject; }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок оси")]
        public string Caption
        {
            get 
            {
                return this.CurrentPivotObject.Caption; 
            }
        }

        #endregion

    }
}
