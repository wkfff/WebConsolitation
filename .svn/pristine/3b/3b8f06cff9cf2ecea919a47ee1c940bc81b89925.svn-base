using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    internal class ExpenceValuationView : BaseView
    {
        internal Krista.FM.Client.Components.UltraGridEx ugeCalculationParams;
        internal Krista.FM.Client.Components.UltraGridEx ugeContracts;
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager utmActions;
        private System.ComponentModel.IContainer components;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.SplitContainer split;
    
        internal ExpenceValuationView()
        {
            InitializeComponent();
			//InfragisticComponentsCustomize.CustomizeUltraGridParams(this.ugeIndicators._ugData);
		}

        public override string Text
        {
            get { return "Оценка расходов на обслуживание долга"; }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("TransferCredits");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("FillCreditsData");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveData");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("TransfertCredits");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool1 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("CalculationResults");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("FillCreditsData");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("TransfertCredits");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveData");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool2 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("CalculationResults");
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            this.split = new System.Windows.Forms.SplitContainer();
            this.ugeCalculationParams = new Krista.FM.Client.Components.UltraGridEx();
            this.ugeContracts = new Krista.FM.Client.Components.UltraGridEx();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utmActions = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utmActions)).BeginInit();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(0, 27);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.ugeCalculationParams);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.ugeContracts);
            this.split.Size = new System.Drawing.Size(806, 509);
            this.split.SplitterDistance = 182;
            this.split.TabIndex = 0;
            // 
            // ugeCalculationParams
            // 
            this.ugeCalculationParams.AllowAddNewRecords = true;
            this.ugeCalculationParams.AllowClearTable = true;
            this.ugeCalculationParams.Caption = "";
            this.ugeCalculationParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeCalculationParams.InDebugMode = false;
            this.ugeCalculationParams.LoadMenuVisible = false;
            this.ugeCalculationParams.Location = new System.Drawing.Point(0, 0);
            this.ugeCalculationParams.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeCalculationParams.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeCalculationParams.Name = "ugeCalculationParams";
            this.ugeCalculationParams.SaveLoadFileName = "";
            this.ugeCalculationParams.SaveMenuVisible = false;
            this.ugeCalculationParams.ServerFilterEnabled = false;
            this.ugeCalculationParams.SingleBandLevelName = "Добавить запись...";
            this.ugeCalculationParams.Size = new System.Drawing.Size(806, 182);
            this.ugeCalculationParams.sortColumnName = "";
            this.ugeCalculationParams.StateRowEnable = false;
            this.ugeCalculationParams.TabIndex = 2;
            // 
            // ugeContracts
            // 
            this.ugeContracts.AllowAddNewRecords = true;
            this.ugeContracts.AllowClearTable = true;
            this.ugeContracts.Caption = "";
            this.ugeContracts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeContracts.InDebugMode = false;
            this.ugeContracts.LoadMenuVisible = false;
            this.ugeContracts.Location = new System.Drawing.Point(0, 0);
            this.ugeContracts.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeContracts.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeContracts.Name = "ugeContracts";
            this.ugeContracts.SaveLoadFileName = "";
            this.ugeContracts.SaveMenuVisible = false;
            this.ugeContracts.ServerFilterEnabled = false;
            this.ugeContracts.SingleBandLevelName = "Добавить запись...";
            this.ugeContracts.Size = new System.Drawing.Size(806, 323);
            this.ugeContracts.sortColumnName = "";
            this.ugeContracts.StateRowEnable = false;
            this.ugeContracts.TabIndex = 3;
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
            this._BaseView_Toolbars_Dock_Area_Left.ToolbarsManager = this.utmActions;
            // 
            // utmActions
            // 
            this.utmActions.DesignerFlags = 1;
            this.utmActions.DockWithinContainer = this;
            this.utmActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.utmActions.LockToolbars = true;
            this.utmActions.RightAlignedMenus = Infragistics.Win.DefaultableBoolean.False;
            this.utmActions.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.utmActions.ShowFullMenusDelay = 500;
            this.utmActions.ShowQuickCustomizeButton = false;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            comboBoxTool1.InstanceProps.Width = 250;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool5,
            buttonTool2,
            comboBoxTool1});
            ultraToolbar1.Text = "CalculationToolBar";
            this.utmActions.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            buttonTool3.SharedPropsInternal.Caption = "Получить данные из кредитов";
            buttonTool4.SharedPropsInternal.Caption = "Перенос в договора";
            buttonTool4.SharedPropsInternal.Enabled = false;
            buttonTool6.SharedPropsInternal.Caption = "Сохранить данные расчета";
            buttonTool6.SharedPropsInternal.Enabled = false;
            comboBoxTool2.SharedPropsInternal.Caption = "Результаты расчетов";
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList1.MaxDropDownItems = 20;
            comboBoxTool2.ValueList = valueList1;
            buttonTool7.SharedPropsInternal.Caption = "Отчет \"Расходы на обслуживание госдолга\"";
            this.utmActions.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4,
            buttonTool6,
            comboBoxTool2,
            buttonTool7});
            // 
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(806, 27);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 509);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.utmActions;
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(806, 27);
            this._BaseView_Toolbars_Dock_Area_Top.ToolbarsManager = this.utmActions;
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 536);
            this._BaseView_Toolbars_Dock_Area_Bottom.Name = "_BaseView_Toolbars_Dock_Area_Bottom";
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(806, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.utmActions;
            // 
            // ExpenceValuationView
            // 
            this.Controls.Add(this.split);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.Name = "ExpenceValuationView";
            this.Size = new System.Drawing.Size(806, 536);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            this.split.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utmActions)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
