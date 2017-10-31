using System.ComponentModel;
using System.Drawing;
using Infragistics.Win.UltraWinDock;

namespace Krista.FM.Client.MDXExpert
{
    class ReportElementBrowseAdapter
    {
        #region Поля

        private CustomReportElement reportElement;
        private DockableControlPane dockableControlPane;

        #endregion

        #region Свойства

        [Category("Окно элемента")]
        [DisplayName("Заголовок")]
        [Description("Заголовок окна")]
        [Browsable(true)]
        public string Name
        {
            get { return dockableControlPane.Text; }
            set { dockableControlPane.Text = value; }
        }

        [Category("Окно элемента")]
        [DisplayName("Состояние")]
        [Description("Состояние окна")]
        [Browsable(true)]
        public DockedState DockedState
        {
            get { return dockableControlPane.DockedState; }
        }

        [Category("Окно элемента")]
        [DisplayName("Минимизация")]
        [Description("Минимизировано ли окно")]
        [Browsable(true)]
        public bool Minimized
        {
            get { return dockableControlPane.Minimized; }
            set { dockableControlPane.Minimized = value; }
        }

        [Category("Окно элемента")]
        [DisplayName("Пришпиленность")]
        [Description("Пришпилено ли окно")]
        [Browsable(true)]
        public bool Pinned
        {
            get { return dockableControlPane.Pinned; }
            set { dockableControlPane.Pinned = value; }
        }

        [Category("Окно элемента")]
        [DisplayName("Размер")]
        [Description("Размер окна")]
        [Browsable(true)]
        public Size Size
        {
            get { return dockableControlPane.Size; }
            set { dockableControlPane.Size = value; }
        }
        
        [Category("Элемент отчета")]
        [DisplayName("Куб")]
        [Description("Куб, из которого берутся данные для элемента отчета")]
        [Browsable(true)]
        public string CubeDef
        {
            get { return reportElement.Cube.Name; }
        }

        [Category("Элемент отчета")]
        [DisplayName("MDX-запрос")]
        [Description("MDX-запрос к кубу")]
        [Browsable(true)]
        public string MDXQuery
        {
            get { return reportElement.MDXQuery; }
        }

        [Category("Элемент отчета")]
        [DisplayName("Структура")]
        [Description("Структура данных элемента")]
        [Browsable(true)]
        public string PivotData
        {
            get { return reportElement.PivotData.StrSettings; }
        }

        [Category("Элемент отчета")]
        [DisplayName("Тип")]
        [Description("Тип элемента")]
        [Browsable(true)]
        public ReportElementType ElementType
        {
            get { return reportElement.ElementType; }
        }

        #endregion

        public ReportElementBrowseAdapter(DockableControlPane dcPane)
        {
            reportElement = (CustomReportElement)dcPane.Control;
            dockableControlPane = dcPane;
        }
    }
}
