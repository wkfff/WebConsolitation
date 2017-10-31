using System;
using System.Collections.Generic;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.TemplatesUI.Commands;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
    public class TemplatesView : BaseViewObject.BaseView
    {
        internal System.Windows.Forms.ImageList ilTools;
        private System.ComponentModel.IContainer components;
        internal System.Windows.Forms.ImageList ilTemplatesType;
        internal System.Windows.Forms.SplitContainer sc1;
		private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControlDetailData;
		private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
		internal Krista.FM.Client.Components.UltraGridEx ugeTemplates;
    
        public TemplatesView()
        {
            InitializeComponent();

        	ugeTemplates.ugData.DisplayLayout.GroupByBox.Hidden = true;
			ugeTemplates.StateRowEnable = true;
			ugeTemplates.ugData.SyncWithCurrencyManager = true;
			ugeTemplates.ugData.DisplayLayout.Override.AllowColSizing = AllowColSizing.Synchronized;
			ugeTemplates.ugData.AllowDrop = true;

			// Инициализация грида
			ugeTemplates.utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
			ugeTemplates.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			ugeTemplates.utmMain.Tools["menuSave"].SharedProps.Enabled = false;
			ugeTemplates.utmMain.Tools["menuLoad"].SharedProps.Enabled = false;
			ugeTemplates.utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;
			ugeTemplates.utmMain.Tools["excelImport"].SharedProps.Visible = false;
			ugeTemplates.utmMain.Tools["excelExport"].SharedProps.Visible = false;
		}

    	public UltraTabPageControl GroupsTabPage
    	{
    		get { return ultraTabPageControl1; }
    	}

    	public UltraTabPageControl UsersTabPage
    	{
    		get { return ultraTabPageControl2; }
    	}

    	/// <summary>
		/// Добавление различных кнопок на тулбар грида.
		/// </summary>
		internal void InitializeGridToolBarCommands(Dictionary<string, TemplatesCommand> commandList)
		{
			// Инициализация команд
    		UltraToolbar tb;
			if (ugeTemplates._utmMain.Toolbars.Exists("Templates"))
			{
				tb = ugeTemplates._utmMain.Toolbars["Templates"];
			}
			else
			{
				tb = new UltraToolbar("Templates");
				tb.DockedColumn = 1;
				tb.DockedRow = 0;
				ugeTemplates._utmMain.Toolbars.AddRange(new UltraToolbar[] { tb });
			}

			foreach (AbstractCommand command in commandList.Values)
    		{
    			if (tb.Tools.Exists(command.Key)) 
					continue;
    			CommandService.AttachToolbarTool(command, tb);
    			command.StateChanged += OnCommandStateChanged;
    		}
		}

		private void OnCommandStateChanged(object sender, EventArgs e)
		{
			AbstractCommand command = (AbstractCommand) sender;
			ugeTemplates.utmMain.Tools[command.Key].SharedProps.Enabled = command.IsEnabled;
		}

		private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplatesView));
			Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
			Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
			this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
			this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
			this.ilTools = new System.Windows.Forms.ImageList(this.components);
			this.ilTemplatesType = new System.Windows.Forms.ImageList(this.components);
			this.sc1 = new System.Windows.Forms.SplitContainer();
			this.ugeTemplates = new Krista.FM.Client.Components.UltraGridEx();
			this.ultraTabControlDetailData = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
			this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
			this.sc1.Panel1.SuspendLayout();
			this.sc1.Panel2.SuspendLayout();
			this.sc1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ultraTabControlDetailData)).BeginInit();
			this.ultraTabControlDetailData.SuspendLayout();
			this.SuspendLayout();
			// 
			// ultraTabPageControl1
			// 
			this.ultraTabPageControl1.Location = new System.Drawing.Point(2, 24);
			this.ultraTabPageControl1.Name = "ultraTabPageControl1";
			this.ultraTabPageControl1.Size = new System.Drawing.Size(570, 148);
			// 
			// ultraTabPageControl2
			// 
			this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
			this.ultraTabPageControl2.Name = "ultraTabPageControl2";
			this.ultraTabPageControl2.Size = new System.Drawing.Size(570, 189);
			// 
			// ilTools
			// 
			this.ilTools.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTools.ImageStream")));
			this.ilTools.TransparentColor = System.Drawing.Color.Magenta;
			this.ilTools.Images.SetKeyName(0, "newTemplate.bmp");
			this.ilTools.Images.SetKeyName(1, "OpenDoc.bmp");
			this.ilTools.Images.SetKeyName(2, "SaveDoc.bmp");
			this.ilTools.Images.SetKeyName(3, "AddDocument.bmp");
			this.ilTools.Images.SetKeyName(4, "AddTopLevel.bmp");
			this.ilTools.Images.SetKeyName(5, "AddChild.bmp");
			this.ilTools.Images.SetKeyName(6, "newTemplate.bmp");
			this.ilTools.Images.SetKeyName(7, "block.bmp");
			this.ilTools.Images.SetKeyName(8, "unBlock.bmp");
			this.ilTools.Images.SetKeyName(9, "editByCurrent.bmp");
			this.ilTools.Images.SetKeyName(10, "CheckOut.bmp");
			this.ilTools.Images.SetKeyName(11, "CheckIn.bmp");
			this.ilTools.Images.SetKeyName(12, "UndoCheckOut.bmp");
			// 
			// ilTemplatesType
			// 
			this.ilTemplatesType.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTemplatesType.ImageStream")));
			this.ilTemplatesType.TransparentColor = System.Drawing.Color.Magenta;
			this.ilTemplatesType.Images.SetKeyName(0, "folder.bmp");
			this.ilTemplatesType.Images.SetKeyName(1, "Word.bmp");
			this.ilTemplatesType.Images.SetKeyName(2, "ExportExcel1.bmp");
			this.ilTemplatesType.Images.SetKeyName(3, "Textbox.bmp");
			this.ilTemplatesType.Images.SetKeyName(4, "ReportMDXExpert.ico");
			this.ilTemplatesType.Images.SetKeyName(5, "ReportWebStatistic.ico");
			// 
			// sc1
			// 
			this.sc1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sc1.Location = new System.Drawing.Point(0, 0);
			this.sc1.Name = "sc1";
			this.sc1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// sc1.Panel1
			// 
			this.sc1.Panel1.Controls.Add(this.ugeTemplates);
			// 
			// sc1.Panel2
			// 
			this.sc1.Panel2.Controls.Add(this.ultraTabControlDetailData);
			this.sc1.Size = new System.Drawing.Size(574, 428);
			this.sc1.SplitterDistance = 250;
			this.sc1.TabIndex = 1;
			// 
			// ugeTemplates
			// 
			this.ugeTemplates.AllowAddNewRecords = true;
			this.ugeTemplates.AllowClearTable = true;
			this.ugeTemplates.Caption = "";
			this.ugeTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ugeTemplates.InDebugMode = false;
			this.ugeTemplates.LoadMenuVisible = false;
			this.ugeTemplates.Location = new System.Drawing.Point(0, 0);
			this.ugeTemplates.MaxCalendarDate = new System.DateTime(((long)(0)));
			this.ugeTemplates.MinCalendarDate = new System.DateTime(((long)(0)));
			this.ugeTemplates.Name = "ugeTemplates";
			this.ugeTemplates.SaveLoadFileName = "";
			this.ugeTemplates.SaveMenuVisible = false;
			this.ugeTemplates.ServerFilterEnabled = false;
			this.ugeTemplates.SingleBandLevelName = "Добавить запись...";
			this.ugeTemplates.Size = new System.Drawing.Size(574, 250);
			this.ugeTemplates.sortColumnName = "";
			this.ugeTemplates.StateRowEnable = false;
			this.ugeTemplates.TabIndex = 0;
			// 
			// ultraTabControlDetailData
			// 
			this.ultraTabControlDetailData.Controls.Add(this.ultraTabSharedControlsPage1);
			this.ultraTabControlDetailData.Controls.Add(this.ultraTabPageControl1);
			this.ultraTabControlDetailData.Controls.Add(this.ultraTabPageControl2);
			this.ultraTabControlDetailData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ultraTabControlDetailData.Location = new System.Drawing.Point(0, 0);
			this.ultraTabControlDetailData.Name = "ultraTabControlDetailData";
			this.ultraTabControlDetailData.SharedControlsPage = this.ultraTabSharedControlsPage1;
			this.ultraTabControlDetailData.Size = new System.Drawing.Size(574, 174);
			this.ultraTabControlDetailData.TabIndex = 0;
			ultraTab1.Key = "Groups";
			ultraTab1.TabPage = this.ultraTabPageControl1;
			ultraTab1.Text = "Права групп";
			ultraTab2.Key = "Users";
			ultraTab2.TabPage = this.ultraTabPageControl2;
			ultraTab2.Text = "Права пользователей";
			this.ultraTabControlDetailData.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
			// 
			// ultraTabSharedControlsPage1
			// 
			this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
			this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
			this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(570, 148);
			// 
			// TemplatesView
			// 
			this.Controls.Add(this.sc1);
			this.Name = "TemplatesView";
			this.Size = new System.Drawing.Size(574, 428);
			this.sc1.Panel1.ResumeLayout(false);
			this.sc1.Panel2.ResumeLayout(false);
			this.sc1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ultraTabControlDetailData)).EndInit();
			this.ultraTabControlDetailData.ResumeLayout(false);
			this.ResumeLayout(false);

        }
    }
}
