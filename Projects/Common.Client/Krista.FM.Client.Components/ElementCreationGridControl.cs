using System;
using System.Windows.Forms;
using Infragistics.Win;

namespace Krista.FM.Client.Components
{
    public class ElementCreationGridControl : UserControl, IUIElementCreationFilter
    {
        public Infragistics.Win.UltraWinGrid.UltraGrid ugData;
    
        public delegate void CreateUIElement(UIElement parent);

        public ElementCreationGridControl()
        {
            InitializeComponent();
        }

        CreateUIElement _createUIElement = null;
        public event CreateUIElement OnCreateUIElement
        {
            add { _createUIElement += value; }
            remove { _createUIElement -= value; }
        }

        public void AfterCreateChildElements(UIElement parent)
        {
            if (_createUIElement != null)
                _createUIElement(parent);
            // called for a UIElement (parent) after it creates its child elements.
        }
        public bool BeforeCreateChildElements(UIElement parent)
        {
            // called for a UIElement (parent) before it creates its child elements.
            return false;
        }

        private void GridControl_Load(object sender, EventArgs e)
        {
            ugData.CreationFilter = this;
        }

        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            this.ugData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this.ugData)).BeginInit();
            this.SuspendLayout();
            // 
            // ugData
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugData.DisplayLayout.Appearance = appearance1;
            this.ugData.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugData.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugData.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.ugData.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance3.BackColor2 = System.Drawing.SystemColors.Control;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugData.DisplayLayout.GroupByBox.PromptAppearance = appearance3;
            this.ugData.DisplayLayout.MaxColScrollRegions = 1;
            this.ugData.DisplayLayout.MaxRowScrollRegions = 1;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugData.DisplayLayout.Override.ActiveCellAppearance = appearance9;
            appearance5.BackColor = System.Drawing.SystemColors.Highlight;
            appearance5.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ugData.DisplayLayout.Override.ActiveRowAppearance = appearance5;
            this.ugData.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ugData.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.Override.CardAreaAppearance = appearance12;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugData.DisplayLayout.Override.CellAppearance = appearance8;
            this.ugData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ugData.DisplayLayout.Override.CellPadding = 0;
            appearance6.BackColor = System.Drawing.SystemColors.Control;
            appearance6.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance6.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.BorderColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.Override.GroupByRowAppearance = appearance6;
            appearance7.TextHAlignAsString = "Left";
            this.ugData.DisplayLayout.Override.HeaderAppearance = appearance7;
            this.ugData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ugData.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.ugData.DisplayLayout.Override.RowAppearance = appearance10;
            this.ugData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ugData.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.ugData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ugData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ugData.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugData.Location = new System.Drawing.Point(0, 0);
            this.ugData.Name = "ugData";
            this.ugData.Size = new System.Drawing.Size(650, 411);
            this.ugData.TabIndex = 0;
            this.ugData.Text = "ultraGrid1";
            // 
            // ElementCreationGridControl
            // 
            this.Controls.Add(this.ugData);
            this.Name = "ElementCreationGridControl";
            this.Size = new System.Drawing.Size(650, 411);
            this.Load += new System.EventHandler(this.GridControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ugData)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
