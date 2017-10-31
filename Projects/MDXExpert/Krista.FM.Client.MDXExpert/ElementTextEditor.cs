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
    /// Оболочка на контрол UltraFormattedTextEditor, но умеющий себя сохранять, 
    /// и еще пару полезных методов
    /// </summary>
    public class ElementTextEditor : UltraFormattedTextEditor
    {
        #region События

        private EventHandler _afterEditText = null;

        /// <summary>
        /// Происходи после завершения редактирования текста
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
            //вынужденая мера, вешаем обработчик на выделение какого либо контрола на рибоне, 
            //для своевременого возбуждения события о конце редактирования текста в контроле
            this.Report.MainForm.UltraToolbarsManager.MouseEnterElement += new UIElementEventHandler(UltraToolbarsManager_MouseEnterElement);
        }

       

        /// <summary>
        /// Загружаем некие настройки контрола
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
        /// Сохраняем настройки контрола
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

        #region Обработчики событий
        /// <summary>
        /// при получении фокуса элементу запоминается надпись в нем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ElementTextEditor_GotFocus(object sender, System.EventArgs e)
        {
            this.tempText = this.Text;
        }

        /// <summary>
        /// После потери фокуса, сравниваем надписи, если они разные возбуждаем событие о 
        /// редактировании текста
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
        /// Если пользователь зашел на рибон, считаем что редактирование текста закончил
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
        /// Шрифт контрола
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
        /// Видимость
        /// </summary>
        public new bool Visible
        {
            get
            {
                return this.isVisibleEditor;
            }
            set
            {
                //Т.к. видимость контрола зависит от видимости панели на которой он лежит, пришлость переопределить
                //свойство, и выставлять данный признак панели а не контролу (у комментария он изменяется автоматом)
                if (this.ParentPanel != null)
                    this.ParentPanel.Visible = value;
                if (this.Splitter != null)
                    this.Splitter.Visible = value;
                this.isVisibleEditor = value;
            }
        }

        /// <summary>
        /// Панель на которой распологается редактор
        /// </summary>
        public Panel ParentPanel
        {
            get { return _parentPanel; }
            set { _parentPanel = value; }
        }

        /// <summary>
        /// Цвет фона
        /// </summary>
        public new Color BackColor
        {
            get { return this.ParentPanel.BackColor; }
            set { this.ParentPanel.BackColor = value; }
        }

        /// <summary>
        /// Пустой
        /// </summary>
        public bool IsEmpty
        {
            get { return (this.Text == string.Empty); }
        }

        /// <summary>
        /// Горизонтальное выравнивание текста
        /// </summary>
        public HAlign TextHAlign
        {
            get { return this.Appearance.TextHAlign; }
            set { this.Appearance.TextHAlign = value; }
        }

        /// <summary>
        /// Вертикальное выравнивание текста
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
        /// Сплитер текстового редактора
        /// </summary>
        public Splitter Splitter
        {
            get { return _splitter; }
            set { _splitter = value; }
        }

        /// <summary>
        /// Транслятор Стиля
        /// </summary>
        public CellStyle Style
        {
            get 
            {
                CellStyle style = new CellStyle(null, this.BackColor, this.BackColor, 
                    this.Appearance.ForeColor, this.Appearance.BorderColor);
                style.Font = this.Font;
                //Если бордюров нет, то и цвет у них прозрачный
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
