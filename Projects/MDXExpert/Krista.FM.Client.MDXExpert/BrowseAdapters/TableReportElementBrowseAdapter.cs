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
        #region ����
        
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
        /// ���� ������� ��������� �� ����������������� MDX �������
        /// </summary>
        [Browsable(false)]
        public bool IsCustomMdxQuery
        {
            get { return this.reportElement.PivotData.IsCustomMDX; }
        }

        #region ��������

        [Category("���������� �������")]
        [Description("������ � ������� ����������� �� ���� �������������, ��� ��������� �������")]
        [DisplayName("������������ �������� ������")]
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
        //�������� �������� ��� �������� ��������� ���������� ��� ��������� ������
        [Category("���������� �������")]
        [DisplayName("������")]
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

        [Category("���")]
        [Description("������ ����������� ����� ��������� �������")]
        [DisplayName("������ �����������")]
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

        //appearance(���)

        //����������� � �������
        [Category("���")]
        [Description("����������� � �������")]
        [DisplayName("����������� � �������")]
        [Browsable(true)]
        public CellCommentsBrowseClass CellCommentsBrowse
        {
            get { return cellCommentsBrowse; }
            set { cellCommentsBrowse = value; }
        }

        //���� ���������� ������
        [Category("���")]
        [Description("���� ���������� ������")]
        [DisplayName("���� ���������� ������")]
        [Browsable(true)]
        public Color HighLightColor
        {
            get { return gridUserInterface.HighLightColor; }
            set { gridUserInterface.HighLightColor = value; }
        }

        [Category("���")]
        [Description("������� ������� � ��������� (�������� �� 30 �� 300)")]
        [DisplayName("�������")]
        [DefaultValue(100)]
        [Browsable(true)]
        public int Scale
        {
            get { return gridUserInterface.GridPercentScale; }
            set { gridUserInterface.GridPercentScale = value; }
        }

        #endregion
    }


    #region �������������� ������ ��� ����������� � PropertyGrid

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

        #region ��������
        /// <summary>
        /// �������� ����� �������
        /// </summary>
        [Category("����������� � �������")]
        [Description("�������� ����� ������� ����������� (��)")]
        [DisplayName("�������� ����� �������(��)")]
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
        /// ���������
        /// </summary>
        [Category("����������� � �������")]
        [Description("���������� �����������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool IsDisplayComments
        {
            get { return gridUserInterface.IsDisplayComments; }
            set { gridUserInterface.IsDisplayComments = value; }
        }

        /// <summary>
        /// ������������ ������
        /// </summary>
        [Category("����������� � �������")]
        [Description("������������ ������ �����������")]
        [DisplayName("������������ ������")]
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
        /// ���������� ����������� � ������, �� �� �����
        /// </summary>
        [Category("����������� � �������")]
        [Description("����������� � ������ ����� ������������ �� ��� ���, ���� ������ ��������� � �� �������.")]
        [DisplayName("���������� �� ����� ������")]
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
