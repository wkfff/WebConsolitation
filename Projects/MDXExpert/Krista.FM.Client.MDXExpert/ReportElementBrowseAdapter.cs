using System.ComponentModel;
using System.Drawing;
using Infragistics.Win.UltraWinDock;

namespace Krista.FM.Client.MDXExpert
{
    class ReportElementBrowseAdapter
    {
        #region ����

        private CustomReportElement reportElement;
        private DockableControlPane dockableControlPane;

        #endregion

        #region ��������

        [Category("���� ��������")]
        [DisplayName("���������")]
        [Description("��������� ����")]
        [Browsable(true)]
        public string Name
        {
            get { return dockableControlPane.Text; }
            set { dockableControlPane.Text = value; }
        }

        [Category("���� ��������")]
        [DisplayName("���������")]
        [Description("��������� ����")]
        [Browsable(true)]
        public DockedState DockedState
        {
            get { return dockableControlPane.DockedState; }
        }

        [Category("���� ��������")]
        [DisplayName("�����������")]
        [Description("�������������� �� ����")]
        [Browsable(true)]
        public bool Minimized
        {
            get { return dockableControlPane.Minimized; }
            set { dockableControlPane.Minimized = value; }
        }

        [Category("���� ��������")]
        [DisplayName("��������������")]
        [Description("���������� �� ����")]
        [Browsable(true)]
        public bool Pinned
        {
            get { return dockableControlPane.Pinned; }
            set { dockableControlPane.Pinned = value; }
        }

        [Category("���� ��������")]
        [DisplayName("������")]
        [Description("������ ����")]
        [Browsable(true)]
        public Size Size
        {
            get { return dockableControlPane.Size; }
            set { dockableControlPane.Size = value; }
        }
        
        [Category("������� ������")]
        [DisplayName("���")]
        [Description("���, �� �������� ������� ������ ��� �������� ������")]
        [Browsable(true)]
        public string CubeDef
        {
            get { return reportElement.Cube.Name; }
        }

        [Category("������� ������")]
        [DisplayName("MDX-������")]
        [Description("MDX-������ � ����")]
        [Browsable(true)]
        public string MDXQuery
        {
            get { return reportElement.MDXQuery; }
        }

        [Category("������� ������")]
        [DisplayName("���������")]
        [Description("��������� ������ ��������")]
        [Browsable(true)]
        public string PivotData
        {
            get { return reportElement.PivotData.StrSettings; }
        }

        [Category("������� ������")]
        [DisplayName("���")]
        [Description("��� ��������")]
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
