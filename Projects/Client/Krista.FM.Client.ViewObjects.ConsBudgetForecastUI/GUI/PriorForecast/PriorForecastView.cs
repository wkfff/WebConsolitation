using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.PriorForecast
{
    public class PriorForecastView : BaseView
    {
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor BudLevelsVariant;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor ContingentVariant;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.CheckBox cbForecast;
        internal System.Windows.Forms.CheckBox cbEstimate;
        internal Infragistics.Win.UltraWinGrid.UltraGrid PriorForecastGrid;
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager tbManager;
        private System.ComponentModel.IContainer components;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;
        internal Infragistics.Win.UltraWinGrid.UltraCombo TableFormParams;
        internal Infragistics.Win.Misc.UltraButton SplitVariantClear;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;

        public PriorForecastView()
        {
            InitializeComponent();
        }
    
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PriorForecastView));
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("DataOperations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RefreshData");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveChanges");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CancelChanges");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddNewKd");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SplitData");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RefreshData");
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CancelChanges");
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveChanges");
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveToExcel");
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoadDataFromExcel");
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CopyVariant");
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SplitData");
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddNewKd");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.SplitVariantClear = new Infragistics.Win.Misc.UltraButton();
            this.TableFormParams = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.cbForecast = new System.Windows.Forms.CheckBox();
            this.cbEstimate = new System.Windows.Forms.CheckBox();
            this.BudLevelsVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ContingentVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            this.PriorForecastGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.tbManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TableFormParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BudLevelsVariant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContingentVariant)).BeginInit();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PriorForecastGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.SplitVariantClear);
            this.ultraPanel1.ClientArea.Controls.Add(this.TableFormParams);
            this.ultraPanel1.ClientArea.Controls.Add(this.cbForecast);
            this.ultraPanel1.ClientArea.Controls.Add(this.cbEstimate);
            this.ultraPanel1.ClientArea.Controls.Add(this.BudLevelsVariant);
            this.ultraPanel1.ClientArea.Controls.Add(this.ContingentVariant);
            this.ultraPanel1.ClientArea.Controls.Add(this.label3);
            this.ultraPanel1.ClientArea.Controls.Add(this.label2);
            this.ultraPanel1.ClientArea.Controls.Add(this.label1);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 27);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(870, 91);
            this.ultraPanel1.TabIndex = 0;
            // 
            // SplitVariantClear
            // 
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.SplitVariantClear.Appearance = appearance3;
            this.SplitVariantClear.Location = new System.Drawing.Point(595, 57);
            this.SplitVariantClear.Name = "SplitVariantClear";
            this.SplitVariantClear.Size = new System.Drawing.Size(22, 22);
            this.SplitVariantClear.TabIndex = 14;
            this.SplitVariantClear.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // TableFormParams
            // 
            this.TableFormParams.CheckedListSettings.CheckStateMember = "";
            appearance35.BackColor = System.Drawing.SystemColors.Window;
            appearance35.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.TableFormParams.DisplayLayout.Appearance = appearance35;
            this.TableFormParams.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.TableFormParams.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.TableFormParams.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance36.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance36.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance36.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance36.BorderColor = System.Drawing.SystemColors.Window;
            this.TableFormParams.DisplayLayout.GroupByBox.Appearance = appearance36;
            appearance38.ForeColor = System.Drawing.SystemColors.GrayText;
            this.TableFormParams.DisplayLayout.GroupByBox.BandLabelAppearance = appearance38;
            this.TableFormParams.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance37.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance37.BackColor2 = System.Drawing.SystemColors.Control;
            appearance37.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance37.ForeColor = System.Drawing.SystemColors.GrayText;
            this.TableFormParams.DisplayLayout.GroupByBox.PromptAppearance = appearance37;
            this.TableFormParams.DisplayLayout.MaxColScrollRegions = 1;
            this.TableFormParams.DisplayLayout.MaxRowScrollRegions = 1;
            appearance43.BackColor = System.Drawing.SystemColors.Window;
            appearance43.ForeColor = System.Drawing.SystemColors.ControlText;
            this.TableFormParams.DisplayLayout.Override.ActiveCellAppearance = appearance43;
            appearance39.BackColor = System.Drawing.SystemColors.Highlight;
            appearance39.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.TableFormParams.DisplayLayout.Override.ActiveRowAppearance = appearance39;
            this.TableFormParams.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.TableFormParams.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance46.BackColor = System.Drawing.SystemColors.Window;
            this.TableFormParams.DisplayLayout.Override.CardAreaAppearance = appearance46;
            appearance42.BorderColor = System.Drawing.Color.Silver;
            appearance42.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.TableFormParams.DisplayLayout.Override.CellAppearance = appearance42;
            this.TableFormParams.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.TableFormParams.DisplayLayout.Override.CellPadding = 0;
            appearance40.BackColor = System.Drawing.SystemColors.Control;
            appearance40.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance40.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance40.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance40.BorderColor = System.Drawing.SystemColors.Window;
            this.TableFormParams.DisplayLayout.Override.GroupByRowAppearance = appearance40;
            appearance41.TextHAlignAsString = "Left";
            this.TableFormParams.DisplayLayout.Override.HeaderAppearance = appearance41;
            this.TableFormParams.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.TableFormParams.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance44.BackColor = System.Drawing.SystemColors.Window;
            appearance44.BorderColor = System.Drawing.Color.Silver;
            this.TableFormParams.DisplayLayout.Override.RowAppearance = appearance44;
            this.TableFormParams.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance45.BackColor = System.Drawing.SystemColors.ControlLight;
            this.TableFormParams.DisplayLayout.Override.TemplateAddRowAppearance = appearance45;
            this.TableFormParams.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.TableFormParams.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.TableFormParams.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.TableFormParams.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.TableFormParams.Location = new System.Drawing.Point(263, 7);
            this.TableFormParams.Name = "TableFormParams";
            this.TableFormParams.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.TableFormParams.Size = new System.Drawing.Size(326, 20);
            this.TableFormParams.TabIndex = 13;
            this.TableFormParams.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // cbForecast
            // 
            this.cbForecast.AutoSize = true;
            this.cbForecast.Checked = true;
            this.cbForecast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbForecast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbForecast.Location = new System.Drawing.Point(595, 33);
            this.cbForecast.Name = "cbForecast";
            this.cbForecast.Size = new System.Drawing.Size(66, 17);
            this.cbForecast.TabIndex = 12;
            this.cbForecast.Text = "Прогноз";
            this.cbForecast.UseVisualStyleBackColor = true;
            // 
            // cbEstimate
            // 
            this.cbEstimate.AutoSize = true;
            this.cbEstimate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbEstimate.Location = new System.Drawing.Point(595, 11);
            this.cbEstimate.Name = "cbEstimate";
            this.cbEstimate.Size = new System.Drawing.Size(61, 17);
            this.cbEstimate.TabIndex = 11;
            this.cbEstimate.Text = "Оценка";
            this.cbEstimate.UseVisualStyleBackColor = true;
            // 
            // BudLevelsVariant
            // 
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            this.BudLevelsVariant.Appearance = appearance14;
            this.BudLevelsVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance15.FontData.BoldAsString = "True";
            editorButton1.Appearance = appearance15;
            editorButton1.Text = "...";
            this.BudLevelsVariant.ButtonsRight.Add(editorButton1);
            this.BudLevelsVariant.Location = new System.Drawing.Point(263, 60);
            this.BudLevelsVariant.Name = "BudLevelsVariant";
            this.BudLevelsVariant.ReadOnly = true;
            this.BudLevelsVariant.Size = new System.Drawing.Size(326, 19);
            this.BudLevelsVariant.TabIndex = 9;
            this.BudLevelsVariant.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // ContingentVariant
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            this.ContingentVariant.Appearance = appearance1;
            this.ContingentVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance2.FontData.BoldAsString = "True";
            editorButton2.Appearance = appearance2;
            editorButton2.Text = "...";
            this.ContingentVariant.ButtonsRight.Add(editorButton2);
            this.ContingentVariant.Location = new System.Drawing.Point(263, 35);
            this.ContingentVariant.Name = "ContingentVariant";
            this.ContingentVariant.ReadOnly = true;
            this.ContingentVariant.Size = new System.Drawing.Size(326, 19);
            this.ContingentVariant.TabIndex = 9;
            this.ContingentVariant.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(246, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Вариант проекта доходов по уровням бюджета";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Вариант проекта доходов в контингенте";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Формирование таблицы";
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.BackColor = System.Drawing.SystemColors.Control;
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraSplitter1.Location = new System.Drawing.Point(0, 118);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 91;
            this.ultraSplitter1.Size = new System.Drawing.Size(870, 6);
            this.ultraSplitter1.TabIndex = 1;
            // 
            // ultraPanel2
            // 
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.PriorForecastGrid);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel2.Location = new System.Drawing.Point(0, 124);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(870, 412);
            this.ultraPanel2.TabIndex = 2;
            // 
            // PriorForecastGrid
            // 
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.PriorForecastGrid.DisplayLayout.Appearance = appearance13;
            this.PriorForecastGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.PriorForecastGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance16.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance16.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance16.BorderColor = System.Drawing.SystemColors.Window;
            this.PriorForecastGrid.DisplayLayout.GroupByBox.Appearance = appearance16;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this.PriorForecastGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance18;
            this.PriorForecastGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.PriorForecastGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance17.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance17.BackColor2 = System.Drawing.SystemColors.Control;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance17.ForeColor = System.Drawing.SystemColors.GrayText;
            this.PriorForecastGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance17;
            this.PriorForecastGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.PriorForecastGrid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance47.BackColor = System.Drawing.SystemColors.Window;
            appearance47.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PriorForecastGrid.DisplayLayout.Override.ActiveCellAppearance = appearance47;
            appearance19.BackColor = System.Drawing.SystemColors.Highlight;
            appearance19.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.PriorForecastGrid.DisplayLayout.Override.ActiveRowAppearance = appearance19;
            this.PriorForecastGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.PriorForecastGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance48.BackColor = System.Drawing.SystemColors.Window;
            this.PriorForecastGrid.DisplayLayout.Override.CardAreaAppearance = appearance48;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            appearance22.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.PriorForecastGrid.DisplayLayout.Override.CellAppearance = appearance22;
            this.PriorForecastGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.PriorForecastGrid.DisplayLayout.Override.CellPadding = 0;
            appearance20.BackColor = System.Drawing.SystemColors.Control;
            appearance20.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance20.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance20.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance20.BorderColor = System.Drawing.SystemColors.Window;
            this.PriorForecastGrid.DisplayLayout.Override.GroupByRowAppearance = appearance20;
            appearance21.TextHAlignAsString = "Left";
            this.PriorForecastGrid.DisplayLayout.Override.HeaderAppearance = appearance21;
            this.PriorForecastGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.PriorForecastGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance49.BackColor = System.Drawing.SystemColors.Window;
            appearance49.BorderColor = System.Drawing.Color.Silver;
            this.PriorForecastGrid.DisplayLayout.Override.RowAppearance = appearance49;
            this.PriorForecastGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance50.BackColor = System.Drawing.SystemColors.ControlLight;
            this.PriorForecastGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance50;
            this.PriorForecastGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.PriorForecastGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.PriorForecastGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.PriorForecastGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PriorForecastGrid.Location = new System.Drawing.Point(0, 0);
            this.PriorForecastGrid.Name = "PriorForecastGrid";
            this.PriorForecastGrid.Size = new System.Drawing.Size(870, 412);
            this.PriorForecastGrid.TabIndex = 0;
            this.PriorForecastGrid.Text = "ultraGrid1";
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(870, 27);
            this._BaseView_Toolbars_Dock_Area_Top.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 536);
            this._BaseView_Toolbars_Dock_Area_Bottom.Name = "_BaseView_Toolbars_Dock_Area_Bottom";
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(870, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Left
            // 
            this._BaseView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 27);
            this._BaseView_Toolbars_Dock_Area_Left.Name = "_BaseView_Toolbars_Dock_Area_Left";
            this._BaseView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 509);
            this._BaseView_Toolbars_Dock_Area_Left.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(870, 27);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 509);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.tbManager;
            // 
            // tbManager
            // 
            this.tbManager.DesignerFlags = 1;
            this.tbManager.DockWithinContainer = this;
            this.tbManager.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool3,
            buttonTool2,
            buttonTool6,
            buttonTool5,
            buttonTool16});
            ultraToolbar1.Text = "DataOperations";
            this.tbManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            appearance53.Image = ((object)(resources.GetObject("appearance53.Image")));
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance53;
            buttonTool7.SharedPropsInternal.Caption = "Обновить данные";
            appearance54.Image = ((object)(resources.GetObject("appearance54.Image")));
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance54;
            buttonTool8.SharedPropsInternal.Caption = "Отменить изменения";
            appearance55.Image = ((object)(resources.GetObject("appearance55.Image")));
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance55;
            buttonTool9.SharedPropsInternal.Caption = "Сохранить изменения";
            appearance56.Image = ((object)(resources.GetObject("appearance56.Image")));
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance56;
            buttonTool10.SharedPropsInternal.Caption = "Сохранить данные в Excel";
            appearance57.Image = ((object)(resources.GetObject("appearance57.Image")));
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance57;
            buttonTool11.SharedPropsInternal.Caption = "Загрузить данные из Excel";
            appearance58.Image = ((object)(resources.GetObject("appearance58.Image")));
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance58;
            buttonTool12.SharedPropsInternal.Caption = "Создать отчет";
            appearance60.Image = ((object)(resources.GetObject("appearance60.Image")));
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance60;
            buttonTool13.SharedPropsInternal.Caption = "Копирование варианта";
            appearance24.Image = ((object)(resources.GetObject("appearance24.Image")));
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance24;
            buttonTool15.SharedPropsInternal.Caption = "Расщепить данные";
            appearance23.Image = ((object)(resources.GetObject("appearance23.Image")));
            buttonTool4.SharedPropsInternal.AppearancesSmall.Appearance = appearance23;
            buttonTool4.SharedPropsInternal.Caption = "Добавить доходный источник";
            this.tbManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool15,
            buttonTool4});
            // 
            // PriorForecastView
            // 
            this.Controls.Add(this.ultraPanel2);
            this.Controls.Add(this.ultraSplitter1);
            this.Controls.Add(this.ultraPanel1);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.Name = "PriorForecastView";
            this.Size = new System.Drawing.Size(870, 536);
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ClientArea.PerformLayout();
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TableFormParams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BudLevelsVariant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContingentVariant)).EndInit();
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PriorForecastGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
