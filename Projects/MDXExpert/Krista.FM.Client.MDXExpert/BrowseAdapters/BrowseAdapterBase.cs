using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;

namespace Krista.FM.Client.MDXExpert
{
    public abstract class PivotObjectBrowseAdapterBase: FilterablePropertyBase
    {
        private string _header;
        private Data.PivotObject _pivotObject;
        private CustomReportElement _reportElement;

        public PivotObjectBrowseAdapterBase(Data.PivotObject pivotObject, string dropListHeader, 
            CustomReportElement reportElement)
        {
            this._pivotObject = pivotObject;
            this._header = dropListHeader;
            this._reportElement = reportElement;
        }

        /// <summary>
        /// Объект, для которого создается адаптер
        /// </summary>
        [Browsable(false)]
        public Data.PivotObject PivotObject
        {
            get 
            { 
                return _pivotObject; 
            }
        }

        [Browsable(false)]
        public PivotDataType PivotDataType
        {
            get
            {
                return this.PivotObject.ParentPivotData.Type;
            }
        }

        /// <summary>
        /// Заголовок для отображения в выпадающем дереве
        /// </summary>
        [Browsable(false)]
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        
        /// <summary>
        /// Пользовательский интерфейс грида
        /// </summary>
        [Browsable(false)]
        public IGridUserInterface GridInterface
        {
            get { return (this.ReportElement != null) ? this.ReportElement.GridUserInterface : null; }
        }

        /// <summary>
        /// Элемент отчета
        /// </summary>
        [Browsable(false)]
        public CustomReportElement ReportElement
        {
            get { return _reportElement; }
        }

        /// <summary>
        /// Тип элемента отчета
        /// </summary>
        [Browsable(false)]
        public ReportElementType ElementType
        {
            get { return this.ReportElement.ElementType; }
        }

        /// <summary>
        /// Отчет строится по пользовательскому MDX запросу
        /// </summary>
        [Browsable(false)]
        public bool IsCustomMDX
        {
            get { return this.ReportElement.PivotData.IsCustomMDX; }
        }
    }
}
