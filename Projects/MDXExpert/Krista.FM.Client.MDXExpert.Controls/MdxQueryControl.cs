using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;
using Infragistics.Win.UltraWinToolbars;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class MdxQueryControl : UserControl
    {
        bool isMayHook;
        ButtonTool extendedModeButton;
        ButtonTool formatedButton;

        private PivotData _currentPivotData;
        private bool _autoSaveQuery;
        private ControlDisplayMode _displayMode;

        public event EventHandler QueryTextChanged
        {
            add { this.utCustomMDX.TextChanged += value; }
            remove { this.utCustomMDX.TextChanged -= value; }
        }

        /// <summary>
        /// ����� ���� � ������ �������� �������
        /// </summary>
        public PivotData CurrentPivotData
        {
            get { return _currentPivotData; }
            set { _currentPivotData = value; }
        }

        /// <summary>
        /// ����� ����������� ��������
        /// </summary>
        public ControlDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set 
            {
                switch (value)
                {
                    case ControlDisplayMode.Extended:
                        {
                            this.extendedModeButton.SharedProps.Visible = false;
                            break;
                        }
                    case ControlDisplayMode.Simple:
                        {
                            this.extendedModeButton.SharedProps.Visible = true;
                            break;
                        }
                }
                _displayMode = value; 
            }
        }

        /// <summary>
        /// ��������� � ����� ���� ��������� � ������� �����
        /// </summary>
        public bool AutoSaveQuery
        {
            get { return _autoSaveQuery; }
            set { _autoSaveQuery = value; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        public string Query
        {
            get { return this.utCustomMDX.Text; }
        }

        public MdxQueryControl()
        {
            InitializeComponent();
            this.InitializeToolBarButton();
            this.AddHandlerToolBarButton();
            this.AutoSaveQuery = true;
            this.DisplayMode = ControlDisplayMode.Simple;
        }

        private void InitializeToolBarButton()
        {
            this.extendedModeButton = (ButtonTool)this.ultraToolbarsManager.Tools["ExtendedModeButton"];
            this.formatedButton = (ButtonTool)this.ultraToolbarsManager.Tools["FormatButton"];
        }

        private void AddHandlerToolBarButton()
        {
            this.extendedModeButton.ToolClick += new ToolClickEventHandler(extendedModeButton_ToolClick);
            this.formatedButton.ToolClick += new ToolClickEventHandler(formatedButton_ToolClick);
        }

        /// <summary>
        /// ����� ���������� ���������
        /// </summary>
        public void ResetEditor()
        {
            this.isMayHook = true;
            this.utCustomMDX.Text = string.Empty;
            this.isMayHook = false;
        }

        /// <summary>
        /// ������������� ���������
        /// </summary>
        /// <param name="reportElement"></param>
        public void RefreshEditor(string sourceQuery)
        {
            this.RefreshHandlers();
            this.isMayHook = true;
            this.utCustomMDX.Text = this.FormatMDXQuery(sourceQuery);
            this.isMayHook = false;
        }

        /// <summary>
        /// ������������� ��������� 
        /// </summary>
        /// <param name="reportElement"></param>
        public void RefreshEditor(PivotData pivotData)
        {
            this.CurrentPivotData = pivotData;
            string sourceQuery = string.Empty;

            if (this.CurrentPivotData != null)
            {
                this.utCustomMDX.ReadOnly = !this.CurrentPivotData.IsCustomMDX;
                sourceQuery = this.CurrentPivotData.MDXQuery;
            }

            this.RefreshEditor(sourceQuery);
        }

        /// <summary>
        /// ���������� � ����� ������ �����������
        /// </summary>
        private void RefreshHandlers()
        {
            if (this.CurrentPivotData != null)
            {
                this.CurrentPivotData.DataChanged -= PivotData_DataChanged;
                this.CurrentPivotData.DataChanged += new Krista.FM.Client.MDXExpert.Data.PivotDataEventHandler(PivotData_DataChanged);
            }
        }

        /// <summary>
        /// ����������� MDX ������
        /// </summary>
        /// <param name="sourceQuery">�������� ������</param>
        /// <returns></returns>
        private string FormatMDXQuery(string sourceQuery)
        {
            string result = sourceQuery;
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    MDXParser.CubeInfo cb = new MDXParser.CubeInfo();
                    MDXParser.Source source = new MDXParser.Source();
                    MDXParser.MDXParser parser = new MDXParser.MDXParser(sourceQuery, source, cb);
                    if (parser != null)
                    {
                        MDXParser.FormatOptions fo = new MDXParser.FormatOptions();
                        fo.Output = MDXParser.OutputFormat.Text;
                        parser.Parse();
                        result = parser.FormatMDX(fo);
                    }
                    //� ���� � ������� ���� ������, ��� ������� �������� �������, �������� ����������� 
                    //����������������� 0, ��� ������ ��� � ���... ���� ���������� ����� ����, �����������
                    //���� ������� �������� ���
                    if (result.Contains("\0"))
                        result = sourceQuery;
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// ��� ��������� ������, ��������� MDX ������
        /// </summary>
        void PivotData_DataChanged()
        {
            this.utCustomMDX.Text = this.FormatMDXQuery(this.CurrentPivotData.MDXQuery);
        }

        /// <summary>
        /// ���������� ����� ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void utCustomMDX_TextChanged(object sender, EventArgs e)
        {
            if (!this.AutoSaveQuery || this.isMayHook || (this.CurrentPivotData == null))
                return;

            this.CurrentPivotData.MDXQuery = this.utCustomMDX.Text;
        }

        void extendedModeButton_ToolClick(object sender, ToolClickEventArgs e)
        {
            MdxQueryEditor mdxQueryEditor = new MdxQueryEditor();
            mdxQueryEditor.RefreshEditor(this.CurrentPivotData);
            mdxQueryEditor.Show();
        }

        void formatedButton_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.utCustomMDX.Text = this.FormatMDXQuery(this.utCustomMDX.Text);
        }
    }

    /// <summary>
    /// ����� ����������� ��������
    /// </summary>
    public enum ControlDisplayMode
    {
        /// <summary>
        /// �������
        /// </summary>
        Simple,
        /// <summary>
        /// ����������
        /// </summary>
        Extended
    }
}
