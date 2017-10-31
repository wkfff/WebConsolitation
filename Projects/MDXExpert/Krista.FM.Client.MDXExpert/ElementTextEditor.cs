using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System;

using Infragistics.Win.FormattedLinkLabel;
using Infragistics.Win;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Common.Converts;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;


namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// �������� �� ������� UltraFormattedTextEditor, �� ������� ���� ���������, 
    /// � ��� ���� �������� �������
    /// </summary>
    public class ElementTextEditor : UltraFormattedTextEditor
    {
        #region �������

        private EventHandler _afterEditText = null;

        /// <summary>
        /// ��������� ����� ���������� �������������� ������
        /// </summary>
        public event EventHandler AfterEditText
        {
            add { _afterEditText += value; }
            remove { _afterEditText -= value; }
        }

        #endregion

        private CustomReportElement _report;
        private Panel _parentPanel;
        private Splitter _splitter;
        bool isVisibleEditor;
        string tempText;

        public ElementTextEditor(CustomReportElement report, Panel parentPanel, Splitter editorSplitter)
            : base()
        {
            this.Report = report;
            this.Splitter = editorSplitter;
            this.ParentPanel = parentPanel;
            this.BorderStyle = UIElementBorderStyle.Solid;
            this.TextRenderingMode = Infragistics.Win.TextRenderingMode.GDI;
            this.GotFocus += new System.EventHandler(ElementTextEditor_GotFocus);
            this.LostFocus += new System.EventHandler(ElementTextEditor_LostFocus);
            //���������� ����, ������ ���������� �� ��������� ������ ���� �������� �� ������, 
            //��� ������������� ����������� ������� � ����� �������������� ������ � ��������
            this.Report.MainForm.UltraToolbarsManager.MouseEnterElement += new UIElementEventHandler(UltraToolbarsManager_MouseEnterElement);
        }

       

        /// <summary>
        /// ��������� ����� ��������� ��������
        /// </summary>
        /// <param name="collectionNode"></param>
        public virtual void Load(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            string sFontData = XmlHelper.GetStringAttrValue(collectionNode, Consts.sfont, string.Empty);
            if (sFontData != string.Empty)
            {
                FontData fontData = UltraFontConvertor.StringToFontData(sFontData);
                UltraFontConvertor.SynchronizeFontData(this.Appearance.FontData, fontData);
            }

            ColorConverter colorConverter = new ColorConverter();
            string sBackColor = XmlHelper.GetStringAttrValue(collectionNode, Consts.backColor, string.Empty);
            if (sBackColor != string.Empty)
            {
                this.BackColor = (Color)colorConverter.ConvertFromString(sBackColor);
            }

            string sForeColor = XmlHelper.GetStringAttrValue(collectionNode, Consts.foreColor, string.Empty);
            if (sForeColor != string.Empty)
            {
                this.Appearance.ForeColor = (Color)colorConverter.ConvertFromString(sForeColor);
            }

            this.Visible = XmlHelper.GetBoolAttrValue(collectionNode, Consts.isDisplay, false);
            this.BorderStyle = (UIElementBorderStyle)XmlHelper.GetIntAttrValue(collectionNode, Consts.borderStyle, 0);
            this.Text = XmlHelper.GetStringAttrValue(collectionNode, Consts.value, string.Empty);
            this.Appearance.TextHAlign = (HAlign)XmlHelper.GetIntAttrValue(collectionNode, Consts.hAligment, 2);
            this.Appearance.TextVAlign = (VAlign)XmlHelper.GetIntAttrValue(collectionNode, Consts.vAligment, 2);
        }

        /// <summary>
        /// ��������� ��������� ��������
        /// </summary>
        /// <param name="captionsNode"></param>
        public virtual void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;
            XmlHelper.SetAttribute(collectionNode, Consts.sfont,
                UltraFontConvertor.FontDataToString(this.Appearance.FontData));

            ColorConverter colorConverter = new ColorConverter();
            XmlHelper.SetAttribute(collectionNode, Consts.backColor,
                colorConverter.ConvertToString(this.BackColor));
            XmlHelper.SetAttribute(collectionNode, Consts.foreColor,
                colorConverter.ConvertToString(this.Appearance.ForeColor));

            XmlHelper.SetAttribute(collectionNode, Consts.isDisplay, this.Visible.ToString());
            XmlHelper.SetAttribute(collectionNode, Consts.borderStyle, ((int)this.BorderStyle).ToString());
            XmlHelper.SetAttribute(collectionNode, Consts.value, this.Text);
            XmlHelper.SetAttribute(collectionNode, Consts.hAligment, ((int)this.Appearance.TextHAlign).ToString());
            XmlHelper.SetAttribute(collectionNode, Consts.vAligment, ((int)this.Appearance.TextVAlign).ToString());
        }

        #region ����������� �������
        /// <summary>
        /// ��� ��������� ������ �������� ������������ ������� � ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ElementTextEditor_GotFocus(object sender, System.EventArgs e)
        {
            this.tempText = this.Text;
        }

        /// <summary>
        /// ����� ������ ������, ���������� �������, ���� ��� ������ ���������� ������� � 
        /// �������������� ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ElementTextEditor_LostFocus(object sender, System.EventArgs e)
        {
            if ((this.tempText != null) && this.tempText != this.Text)
            {
                this.OnAfterEditText();
                this.tempText = this.Text;
            }
        }

        /// <summary>
        /// ���� ������������ ����� �� �����, ������� ��� �������������� ������ ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UltraToolbarsManager_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if ((this.tempText != null) && this.tempText != this.Text)
            {
                this.OnAfterEditText();
                this.tempText = this.Text;
            }
        }

        #endregion

        private void OnAfterEditText()
        {
            if (this._afterEditText != null)
                this._afterEditText(this, new EventArgs());
        }

        /// <summary>
        /// ����� ��������
        /// </summary>
        public new Font Font
        {
            get { return UltraFontConvertor.FontDataToFont(base.Appearance.FontData); }
            set
            {
                FontData fontData = UltraFontConvertor.FontToFontData(value);
                UltraFontConvertor.SynchronizeFontData(this.Appearance.FontData, fontData);
            }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public new bool Visible
        {
            get
            {
                return this.isVisibleEditor;
            }
            set
            {
                //�.�. ��������� �������� ������� �� ��������� ������ �� ������� �� �����, ��������� ��������������
                //��������, � ���������� ������ ������� ������ � �� �������� (� ����������� �� ���������� ���������)
                if (this.ParentPanel != null)
                    this.ParentPanel.Visible = value;
                if (this.Splitter != null)
                    this.Splitter.Visible = value;
                this.isVisibleEditor = value;
            }
        }

        /// <summary>
        /// ������ �� ������� ������������� ��������
        /// </summary>
        public Panel ParentPanel
        {
            get { return _parentPanel; }
            set { _parentPanel = value; }
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        public new Color BackColor
        {
            get { return this.ParentPanel.BackColor; }
            set { this.ParentPanel.BackColor = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public bool IsEmpty
        {
            get { return (this.Text == string.Empty); }
        }

        /// <summary>
        /// �������������� ������������ ������
        /// </summary>
        public HAlign TextHAlign
        {
            get { return this.Appearance.TextHAlign; }
            set { this.Appearance.TextHAlign = value; }
        }

        /// <summary>
        /// ������������ ������������ ������
        /// </summary>
        public VAlign TextVAlign
        {
            get { return this.Appearance.TextVAlign; }
            set { this.Appearance.TextVAlign = value; }
        }

        public CustomReportElement Report
        {
            get { return _report; }
            set { _report = value; }
        }

        /// <summary>
        /// ������� ���������� ���������
        /// </summary>
        public Splitter Splitter
        {
            get { return _splitter; }
            set { _splitter = value; }
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        public CellStyle Style
        {
            get 
            {
                CellStyle style = new CellStyle(null, this.BackColor, this.BackColor, 
                    this.Appearance.ForeColor, this.Appearance.BorderColor);
                style.Font = this.Font;
                //���� �������� ���, �� � ���� � ��� ����������
                if (this.BorderStyle == UIElementBorderStyle.None)
                    style.BorderColor = Color.Transparent;
                style.StringFormat.Trimming = TrimmingConvertor.ToSystemTrimming(this.Appearance.TextTrimming);
                style.StringFormat.Alignment = AlignConvertor.ToStringAlignment(this.Appearance.TextHAlign);
                style.StringFormat.LineAlignment = AlignConvertor.ToStringAlignment(this.Appearance.TextVAlign);
                return style;
            }
            set 
            {
                if (value != null)
                {
                    this.BackColor = value.BackColorStart;
                    this.Appearance.ForeColor = value.ForeColor;
                    //this.Appearance.BorderColor = value.BorderColor;
                    this.Appearance.TextTrimming = TrimmingConvertor.ToInfragisticsTrimming(value.StringFormat.Trimming);
                    this.Appearance.TextHAlign = AlignConvertor.ToHAlign(value.StringFormat.Alignment);
                    this.Appearance.TextVAlign = AlignConvertor.ToVAlign(value.StringFormat.LineAlignment);
                    if (value.BorderColor == Color.Transparent)
                        this.BorderStyle = UIElementBorderStyle.None;
                    else
                        if (this.BorderStyle == UIElementBorderStyle.None)
                            this.BorderStyle = UIElementBorderStyle.Solid;
                    this.Font = value.Font;
                }
            }
        }
    }
}
