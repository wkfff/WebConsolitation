using Krista.FM.Common;
using Krista.FM.Client.Common;

namespace Krista.FM.Client.SchemeDesigner
{
    partial class SchemeDesigner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                SchemeEditor.Close();
                components.Dispose();
                UpdateFrameworkLibraryFactory.InvokeMethod("Dispose");
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane1 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedLeft, new System.Guid("4dffeb30-6dd9-464a-b3c2-a2ed3cddf887"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane1 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("2ea73f1a-25f5-4ae3-887f-619af5af9645"), new System.Guid("00000000-0000-0000-0000-000000000000"), -1, new System.Guid("4dffeb30-6dd9-464a-b3c2-a2ed3cddf887"), -1);
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane2 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("aabe09c0-2546-4ec9-b21e-4f27878e5039"), new System.Guid("bd641374-33a2-4572-903c-e02e11802de2"), -1, new System.Guid("4dffeb30-6dd9-464a-b3c2-a2ed3cddf887"), 0);
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane3 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("325aaa57-2146-485d-9825-8db6d5055f2d"), new System.Guid("71fb77c1-66b4-4919-afc1-4f5e35739905"), -1, new System.Guid("4dffeb30-6dd9-464a-b3c2-a2ed3cddf887"), 1);
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane2 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("71fb77c1-66b4-4919-afc1-4f5e35739905"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane3 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("371155e7-c05f-4125-99f8-e968f6a683b3"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane4 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedBottom, new System.Guid("10307084-87e3-4ae1-9536-3da981739421"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane4 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("5dc1c589-136f-40d5-b1d1-fee4234a6ae3"), new System.Guid("590699b1-4ec4-4768-aaeb-c50c6d0ca478"), -1, new System.Guid("10307084-87e3-4ae1-9536-3da981739421"), 1);
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane5 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("128b066d-f543-426d-81e6-b8646f636197"), new System.Guid("f58bf029-12a6-4808-b1fc-48ccf3d54ead"), -1, new System.Guid("10307084-87e3-4ae1-9536-3da981739421"), 2);
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane6 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("5cc4f339-c29e-49de-9e88-cfc45842a79a"), new System.Guid("371155e7-c05f-4125-99f8-e968f6a683b3"), -1, new System.Guid("10307084-87e3-4ae1-9536-3da981739421"), 3);
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane7 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("a6ad0597-9bd9-4520-836d-20ada0cd01da"), new System.Guid("8d54fb46-c16b-454e-9f82-60f35c1563ac"), -1, new System.Guid("10307084-87e3-4ae1-9536-3da981739421"), 4);
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane5 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("f58bf029-12a6-4808-b1fc-48ccf3d54ead"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane6 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("590699b1-4ec4-4768-aaeb-c50c6d0ca478"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane7 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("8d54fb46-c16b-454e-9f82-60f35c1563ac"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane8 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("6cd4fd9f-a39f-4108-a619-1151c889dd1d"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane9 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedBottom, new System.Guid("af0f037f-f425-438d-b2bf-082548baf367"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane8 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("88dd3992-e686-43e4-9126-cb1f3487fcaa"), new System.Guid("6cd4fd9f-a39f-4108-a619-1151c889dd1d"), -1, new System.Guid("af0f037f-f425-438d-b2bf-082548baf367"), 0);
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane10 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("bd641374-33a2-4572-903c-e02e11802de2"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane11 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedBottom, new System.Guid("de3257dc-c0a6-4a18-ae84-60a25f234f95"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane9 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("130b5d47-f62a-493d-9b61-41eefb2fd2ba"), new System.Guid("00000000-0000-0000-0000-000000000000"), -1, new System.Guid("de3257dc-c0a6-4a18-ae84-60a25f234f95"), -1);
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane12 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("adf52eb9-6e67-46a5-ae20-0fbc59952178"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane10 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("46bd79e3-6706-4282-94c1-2cb7d8b1492b"), new System.Guid("adf52eb9-6e67-46a5-ae20-0fbc59952178"), 0, new System.Guid("51ba1c14-9db7-431b-827b-fbfefda48a7d"), 0);
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane13 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedRight, new System.Guid("51ba1c14-9db7-431b-827b-fbfefda48a7d"));
            Infragistics.Win.UltraWinDock.DockAreaPane dockAreaPane14 = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.Floating, new System.Guid("426d45d5-b5bf-4020-868e-497ca5f9f0f3"));
            Infragistics.Win.UltraWinDock.DockableControlPane dockableControlPane11 = new Infragistics.Win.UltraWinDock.DockableControlPane(new System.Guid("057fa081-41ac-4588-bede-44c3a2fa39ef"), new System.Guid("426d45d5-b5bf-4020-868e-497ca5f9f0f3"), -1, new System.Guid("00000000-0000-0000-0000-000000000000"), -1);
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("MenuBar");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool19 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool20 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Edit");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool21 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("View");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool22 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Tools");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool23 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Window");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool24 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Help");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("NavigationBar");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool25 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Back");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool26 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Forward");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool27 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("View");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool33 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("objectsTreeView", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool34 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("propertyGrid", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool35 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("developerDescriptionControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool36 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("semanticsGridControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool37 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("dataSuppliersGridControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool38 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("modificationsTreeControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool39 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("dataKindsGridControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool40 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionGridControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool41 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("searchTabControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool42 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("macroSetControl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool43 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("FullScreen", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool28 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Tools");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UMLHelp");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool29 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("MDHelp");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool44 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("compareDescription", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("createCutFmmd_all");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ConflictsTable ");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("FillVersions");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool45 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("objectsTreeView", "");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool46 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("propertyGrid", "");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool47 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("semanticsGridControl", "");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool48 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("dataSuppliersGridControl", "");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool49 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("dataKindsGridControl", "");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool30 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Exit");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool31 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Window");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool50 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("TabbedMDI", "");
            Infragistics.Win.UltraWinToolbars.MdiWindowListTool mdiWindowListTool3 = new Infragistics.Win.UltraWinToolbars.MdiWindowListTool("MDIWindowListTool");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool32 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Help");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("About");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("About");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.MdiWindowListTool mdiWindowListTool4 = new Infragistics.Win.UltraWinToolbars.MdiWindowListTool("MDIWindowListTool");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool51 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("TabbedMDI", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool52 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("modificationsTreeControl", "");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool53 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionGridControl", "");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Exit");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool54 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("developerDescriptionControl", "");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool55 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("FullScreen", "");
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool33 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Edit");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool56 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("searchServiceControl", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool34 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Back");
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool35 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Forward");
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool57 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("searchTabControl", "");
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool58 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("searchServiceControl", "");
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool59 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("macroSetControl", "");
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool36 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("MDHelp");
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemeDesigner));
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool60 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("MDHelp_cl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool61 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("MDHelp_all", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool62 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("MDHelp_cl", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool63 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("MDHelp_all", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool64 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("compareDescription", "");
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UMLHelp");
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("createCutFmmd_all");
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ConflictsTable ");
			this.objectsTreeView = new Krista.FM.Client.SchemeEditor.ObjectsTreeView();
            this.developerDescriptionControl = new Krista.FM.Client.SchemeEditor.DeveloperDescriptionControl();
            this.semanticsGridControl = new Krista.FM.Client.Design.Editors.SemanticsGridControl();
            this.modificationsTreeControl = new Krista.FM.Client.SchemeEditor.ModificationsTreeControl();
            this.dataSuppliersGridControl = new Krista.FM.Client.SchemeEditor.DataSuppliersGridControl();
            this.dataKindsGridControl = new Krista.FM.Client.SchemeEditor.DataKindsGridControl();
            this.sessionGridControl = new Krista.FM.Client.Common.SessionGrid();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.searchTabControl = new Krista.FM.Client.SchemeEditor.Services.SearchService.SearchTabControl();
            this.searchServiceControl = new Krista.FM.Client.SchemeEditor.Services.SearchService.SearchServiceControl();
            this.macroSetControl = new Krista.FM.Client.SchemeEditor.MacroSetControl();
            this.ultraTabbedMdiManager = new Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager(this.components);
            this.ultraDockManager = new Infragistics.Win.UltraWinDock.UltraDockManager(this.components);
            this._SchemeDesignerUnpinnedTabAreaLeft = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._SchemeDesignerUnpinnedTabAreaRight = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._SchemeDesignerUnpinnedTabAreaTop = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._SchemeDesignerUnpinnedTabAreaBottom = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._SchemeDesignerAutoHideControl = new Infragistics.Win.UltraWinDock.AutoHideControl();
            this.MainToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._SchemeDesigner_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SchemeDesigner_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SchemeDesigner_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.windowDockingArea2 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.dockableWindow4 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow11 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow10 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow8 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow5 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow2 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow3 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow7 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow12 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow9 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.dockableWindow1 = new Infragistics.Win.UltraWinDock.DockableWindow();
            this.windowDockingArea3 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea9 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.windowDockingArea10 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea12 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea13 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea1 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea5 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea8 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea11 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea6 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea14 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea15 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            this.windowDockingArea17 = new Infragistics.Win.UltraWinDock.WindowDockingArea();
            ((System.ComponentModel.ISupportInitialize)(this.objectsTreeView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabbedMdiManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainToolbarsManager)).BeginInit();
            this.windowDockingArea2.SuspendLayout();
            this.dockableWindow4.SuspendLayout();
            this.dockableWindow11.SuspendLayout();
            this.dockableWindow10.SuspendLayout();
            this.dockableWindow8.SuspendLayout();
            this.dockableWindow5.SuspendLayout();
            this.dockableWindow2.SuspendLayout();
            this.dockableWindow3.SuspendLayout();
            this.dockableWindow7.SuspendLayout();
            this.dockableWindow12.SuspendLayout();
            this.dockableWindow9.SuspendLayout();
            this.dockableWindow1.SuspendLayout();
            this.windowDockingArea10.SuspendLayout();
            this.windowDockingArea8.SuspendLayout();
            this.windowDockingArea6.SuspendLayout();
            this.windowDockingArea14.SuspendLayout();
            this.windowDockingArea17.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectsTreeView
            // 
            this.objectsTreeView.AllowDrop = true;
            this.objectsTreeView.HideSelection = false;
            this.objectsTreeView.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.objectsTreeView.Location = new System.Drawing.Point(0, 20);
            this.objectsTreeView.Name = "objectsTreeView";
            this.objectsTreeView.PathSeparator = "::";
            this.objectsTreeView.Size = new System.Drawing.Size(100, 80);
            this.objectsTreeView.TabIndex = 0;
            // 
            // developerDescriptionControl
            // 
            this.developerDescriptionControl.Location = new System.Drawing.Point(0, 18);
            this.developerDescriptionControl.Name = "developerDescriptionControl";
            this.developerDescriptionControl.Size = new System.Drawing.Size(675, 77);
            this.developerDescriptionControl.TabIndex = 0;
            // 
            // semanticsGridControl
            // 
            this.semanticsGridControl.Location = new System.Drawing.Point(0, 18);
            this.semanticsGridControl.Name = "semanticsGridControl";
            this.semanticsGridControl.Size = new System.Drawing.Size(675, 82);
            this.semanticsGridControl.TabIndex = 22;
            this.semanticsGridControl.Value = null;
            // 
            // modificationsTreeControl
            // 
            this.modificationsTreeControl.Location = new System.Drawing.Point(0, 20);
            this.modificationsTreeControl.Name = "modificationsTreeControl";
            this.modificationsTreeControl.Size = new System.Drawing.Size(675, 80);
            this.modificationsTreeControl.TabIndex = 31;
            // 
            // dataSuppliersGridControl
            // 
            this.dataSuppliersGridControl.DataSupplierCollection = null;
            this.dataSuppliersGridControl.Location = new System.Drawing.Point(0, 18);
            this.dataSuppliersGridControl.Name = "dataSuppliersGridControl";
            this.dataSuppliersGridControl.Size = new System.Drawing.Size(675, 222);
            this.dataSuppliersGridControl.TabIndex = 22;
            // 
            // dataKindsGridControl
            // 
            this.dataKindsGridControl.DataSupplierCollection = null;
            this.dataKindsGridControl.Location = new System.Drawing.Point(0, 18);
            this.dataKindsGridControl.Name = "dataKindsGridControl";
            this.dataKindsGridControl.Size = new System.Drawing.Size(675, 251);
            this.dataKindsGridControl.TabIndex = 23;
            // 
            // sessionGridControl
            // 
            this.sessionGridControl.Location = new System.Drawing.Point(0, 18);
            this.sessionGridControl.Name = "sessionGridControl";
            this.sessionGridControl.Sessions = null;
            this.sessionGridControl.Size = new System.Drawing.Size(675, 231);
            this.sessionGridControl.TabIndex = 31;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(0, 20);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(675, 80);
            this.propertyGrid.TabIndex = 0;
            // 
            // searchTabControl
            // 
            this.searchTabControl.Location = new System.Drawing.Point(0, 18);
            this.searchTabControl.Name = "searchTabControl";
            this.searchTabControl.Size = new System.Drawing.Size(266, 261);
            this.searchTabControl.TabIndex = 43;
            // 
            // searchServiceControl
            // 
            this.searchServiceControl.Location = new System.Drawing.Point(0, 18);
            this.searchServiceControl.MinimumSize = new System.Drawing.Size(515, 130);
            this.searchServiceControl.Name = "searchServiceControl";
            this.searchServiceControl.Size = new System.Drawing.Size(515, 131);
            this.searchServiceControl.TabIndex = 10;
            // 
            // macroSetControl
            // 
            this.macroSetControl.Location = new System.Drawing.Point(0, 18);
            this.macroSetControl.Name = "macroSetControl";
            this.macroSetControl.Size = new System.Drawing.Size(100, 82);
            this.macroSetControl.TabIndex = 37;
            // 
            // ultraTabbedMdiManager
            // 
            this.ultraTabbedMdiManager.ImageTransparentColor = System.Drawing.Color.Lime;
            this.ultraTabbedMdiManager.MdiParent = this;
            this.ultraTabbedMdiManager.TabGroupSettings.ShowTabListButton = Infragistics.Win.DefaultableBoolean.True;
            this.ultraTabbedMdiManager.TabSettings.DisplayFormIcon = Infragistics.Win.DefaultableBoolean.True;
            // 
            // ultraDockManager
            // 
            dockAreaPane1.DockedBefore = new System.Guid("71fb77c1-66b4-4919-afc1-4f5e35739905");
            dockableControlPane1.Closed = true;
            dockableControlPane1.Control = this.objectsTreeView;
            dockableControlPane1.Key = "objectsTreeView";
            dockableControlPane1.OriginalControlBounds = new System.Drawing.Rectangle(52, 15, 121, 97);
            appearance31.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.ObjectTreeView;
            dockableControlPane1.Settings.Appearance = appearance31;
            dockableControlPane1.Size = new System.Drawing.Size(100, 291);
            dockableControlPane1.Text = "Дерево объектов";
            dockableControlPane2.Closed = true;
            dockableControlPane2.Control = this.developerDescriptionControl;
            dockableControlPane2.Key = "developerDescriptionControl";
            dockableControlPane2.OriginalControlBounds = new System.Drawing.Rectangle(358, 101, 150, 150);
            appearance32.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.DeveloperDescription;
            dockableControlPane2.Settings.Appearance = appearance32;
            dockableControlPane2.Size = new System.Drawing.Size(100, 152);
            dockableControlPane2.Text = "Описание разработчика";
            dockableControlPane3.Closed = true;
            dockableControlPane3.Control = this.semanticsGridControl;
            dockableControlPane3.Key = "semanticsGridControl";
            dockableControlPane3.OriginalControlBounds = new System.Drawing.Rectangle(34, 91, 372, 355);
            appearance33.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Semantics;
            dockableControlPane3.Settings.Appearance = appearance33;
            dockableControlPane3.Size = new System.Drawing.Size(100, 100);
            dockableControlPane3.Text = "Список семантик";
            dockAreaPane1.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane1,
            dockableControlPane2,
            dockableControlPane3});
            dockAreaPane1.SelectedTabIndex = 2;
            dockAreaPane1.Size = new System.Drawing.Size(266, 466);
            dockAreaPane2.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane2.DockedBefore = new System.Guid("371155e7-c05f-4125-99f8-e968f6a683b3");
            dockAreaPane2.FloatingLocation = new System.Drawing.Point(48, 340);
            dockAreaPane2.Size = new System.Drawing.Size(180, 262);
            dockAreaPane3.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane3.DockedBefore = new System.Guid("10307084-87e3-4ae1-9536-3da981739421");
            dockAreaPane3.FloatingLocation = new System.Drawing.Point(364, 184);
            dockAreaPane3.Size = new System.Drawing.Size(180, 262);
            dockAreaPane4.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane4.DockedBefore = new System.Guid("f58bf029-12a6-4808-b1fc-48ccf3d54ead");
            dockAreaPane4.FloatingLocation = new System.Drawing.Point(364, 184);
            dockableControlPane4.Closed = true;
            dockableControlPane4.Control = this.modificationsTreeControl;
            dockableControlPane4.Key = "modificationsTreeControl";
            dockableControlPane4.OriginalControlBounds = new System.Drawing.Rectangle(408, 138, 456, 342);
            appearance34.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Refresh;
            dockableControlPane4.Settings.Appearance = appearance34;
            dockableControlPane4.Size = new System.Drawing.Size(100, 100);
            dockableControlPane4.Text = "Обновление схемы";
            dockableControlPane5.Closed = true;
            dockableControlPane5.Control = this.dataSuppliersGridControl;
            dockableControlPane5.Key = "dataSuppliersGridControl";
            dockableControlPane5.OriginalControlBounds = new System.Drawing.Rectangle(10, 110, 489, 328);
            appearance35.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.DataSuppliers;
            dockableControlPane5.Settings.Appearance = appearance35;
            dockableControlPane5.Size = new System.Drawing.Size(100, 100);
            dockableControlPane5.Text = "Поставщики данных";
            dockableControlPane6.Closed = true;
            dockableControlPane6.Control = this.dataKindsGridControl;
            dockableControlPane6.Key = "dataKindsGridControl";
            dockableControlPane6.OriginalControlBounds = new System.Drawing.Rectangle(33, 82, 396, 399);
            appearance36.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.DataKinds;
            dockableControlPane6.Settings.Appearance = appearance36;
            dockableControlPane6.Size = new System.Drawing.Size(100, 100);
            dockableControlPane6.Text = "Виды поступающей информации";
            dockableControlPane7.Closed = true;
            dockableControlPane7.Control = this.sessionGridControl;
            dockableControlPane7.Key = "sessionGridControl";
            dockableControlPane7.OriginalControlBounds = new System.Drawing.Rectangle(333, 79, 489, 328);
            appearance37.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Sessions;
            dockableControlPane7.Settings.Appearance = appearance37;
            dockableControlPane7.Size = new System.Drawing.Size(100, 100);
            dockableControlPane7.Text = "Сессии";
            dockAreaPane4.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane4,
            dockableControlPane5,
            dockableControlPane6,
            dockableControlPane7});
            dockAreaPane4.Size = new System.Drawing.Size(675, 269);
            dockAreaPane5.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane5.DockedBefore = new System.Guid("590699b1-4ec4-4768-aaeb-c50c6d0ca478");
            dockAreaPane5.FloatingLocation = new System.Drawing.Point(219, 318);
            dockAreaPane5.Size = new System.Drawing.Size(180, 262);
            dockAreaPane6.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane6.DockedBefore = new System.Guid("8d54fb46-c16b-454e-9f82-60f35c1563ac");
            dockAreaPane6.FloatingLocation = new System.Drawing.Point(369, 556);
            dockAreaPane6.Size = new System.Drawing.Size(404, 95);
            dockAreaPane7.DockedBefore = new System.Guid("6cd4fd9f-a39f-4108-a619-1151c889dd1d");
            dockAreaPane7.FloatingLocation = new System.Drawing.Point(275, 524);
            dockAreaPane7.Size = new System.Drawing.Size(404, 95);
            dockAreaPane8.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane8.DockedBefore = new System.Guid("af0f037f-f425-438d-b2bf-082548baf367");
            dockAreaPane8.FloatingLocation = new System.Drawing.Point(332, 308);
            dockAreaPane8.Size = new System.Drawing.Size(266, 142);
            dockAreaPane9.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane9.DockedBefore = new System.Guid("bd641374-33a2-4572-903c-e02e11802de2");
            dockAreaPane9.FloatingLocation = new System.Drawing.Point(332, 308);
            dockableControlPane8.Closed = true;
            dockableControlPane8.Control = this.propertyGrid;
            dockableControlPane8.Key = "propertyGrid";
            dockableControlPane8.OriginalControlBounds = new System.Drawing.Rectangle(87, 142, 130, 130);
            appearance38.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Properties;
            dockableControlPane8.Settings.Appearance = appearance38;
            dockableControlPane8.Size = new System.Drawing.Size(100, 100);
            dockableControlPane8.Text = "Свойства объекта";
            dockAreaPane9.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane8});
            dockAreaPane9.Size = new System.Drawing.Size(675, 240);
            dockAreaPane10.ChildPaneStyle = Infragistics.Win.UltraWinDock.ChildPaneStyle.TabGroup;
            dockAreaPane10.DockedBefore = new System.Guid("de3257dc-c0a6-4a18-ae84-60a25f234f95");
            dockAreaPane10.FloatingLocation = new System.Drawing.Point(-68, 459);
            dockAreaPane10.Size = new System.Drawing.Size(404, 269);
            dockAreaPane11.DockedBefore = new System.Guid("adf52eb9-6e67-46a5-ae20-0fbc59952178");
            dockableControlPane9.Closed = true;
            dockableControlPane9.Control = this.searchTabControl;
            dockableControlPane9.Key = "searchTabControl";
            dockableControlPane9.MinimumSize = new System.Drawing.Size(100, 100);
            dockableControlPane9.OriginalControlBounds = new System.Drawing.Rectangle(116, 139, 415, 323);
            appearance39.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.SearchResults;
            dockableControlPane9.Settings.Appearance = appearance39;
            dockableControlPane9.Size = new System.Drawing.Size(100, 100);
            dockableControlPane9.Text = "Результаты поиска";
            dockAreaPane11.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane9});
            dockAreaPane11.Size = new System.Drawing.Size(675, 95);
            dockAreaPane12.DockedBefore = new System.Guid("51ba1c14-9db7-431b-827b-fbfefda48a7d");
            dockAreaPane12.FloatingLocation = new System.Drawing.Point(154, 278);
            dockAreaPane12.MinimumSize = new System.Drawing.Size(515, 130);
            dockableControlPane10.Closed = true;
            dockableControlPane10.Control = this.searchServiceControl;
            dockableControlPane10.Key = "searchServiceControl";
            dockableControlPane10.MinimumSize = new System.Drawing.Size(515, 145);
            dockableControlPane10.OriginalControlBounds = new System.Drawing.Rectangle(68, 203, 538, 150);
            dockableControlPane10.Size = new System.Drawing.Size(100, 100);
            dockableControlPane10.Text = "Параметры поиска";
            dockAreaPane12.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane10});
            dockAreaPane12.Size = new System.Drawing.Size(515, 149);
            dockAreaPane13.DockedBefore = new System.Guid("426d45d5-b5bf-4020-868e-497ca5f9f0f3");
            dockAreaPane13.FloatingLocation = new System.Drawing.Point(142, 233);
            dockAreaPane13.MinimumSize = new System.Drawing.Size(515, 130);
            dockAreaPane13.Size = new System.Drawing.Size(510, 466);
            dockAreaPane14.FloatingLocation = new System.Drawing.Point(472, 303);
            dockableControlPane11.Closed = true;
            dockableControlPane11.Control = this.macroSetControl;
            dockableControlPane11.Key = "macroSetControl";
            dockableControlPane11.OriginalControlBounds = new System.Drawing.Rectangle(260, 224, 150, 150);
            appearance40.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.MacroSet;
            dockableControlPane11.Settings.Appearance = appearance40;
            dockableControlPane11.Size = new System.Drawing.Size(100, 100);
            dockableControlPane11.Text = "Обработчики событий";
            dockAreaPane14.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
            dockableControlPane11});
            dockAreaPane14.Size = new System.Drawing.Size(100, 100);
            this.ultraDockManager.DockAreas.AddRange(new Infragistics.Win.UltraWinDock.DockAreaPane[] {
            dockAreaPane1,
            dockAreaPane2,
            dockAreaPane3,
            dockAreaPane4,
            dockAreaPane5,
            dockAreaPane6,
            dockAreaPane7,
            dockAreaPane8,
            dockAreaPane9,
            dockAreaPane10,
            dockAreaPane11,
            dockAreaPane12,
            dockAreaPane13,
            dockAreaPane14});
            this.ultraDockManager.DragWindowStyle = Infragistics.Win.UltraWinDock.DragWindowStyle.LayeredWindowWithIndicators;
            this.ultraDockManager.HostControl = this;
            this.ultraDockManager.ImageTransparentColor = System.Drawing.Color.Lime;
            this.ultraDockManager.PropertyChanged += new Infragistics.Win.PropertyChangedEventHandler(this.ultraDockManager_PropertyChanged);
            this.ultraDockManager.AfterPaneButtonClick += new Infragistics.Win.UltraWinDock.PaneButtonEventHandler(this.ultraDockManager_AfterPaneButtonClick);
            // 
            // _SchemeDesignerUnpinnedTabAreaLeft
            // 
            this._SchemeDesignerUnpinnedTabAreaLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this._SchemeDesignerUnpinnedTabAreaLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._SchemeDesignerUnpinnedTabAreaLeft.Location = new System.Drawing.Point(0, 71);
            this._SchemeDesignerUnpinnedTabAreaLeft.Name = "_SchemeDesignerUnpinnedTabAreaLeft";
            this._SchemeDesignerUnpinnedTabAreaLeft.Owner = this.ultraDockManager;
            this._SchemeDesignerUnpinnedTabAreaLeft.Size = new System.Drawing.Size(0, 441);
            this._SchemeDesignerUnpinnedTabAreaLeft.TabIndex = 0;
            // 
            // _SchemeDesignerUnpinnedTabAreaRight
            // 
            this._SchemeDesignerUnpinnedTabAreaRight.Dock = System.Windows.Forms.DockStyle.Right;
            this._SchemeDesignerUnpinnedTabAreaRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._SchemeDesignerUnpinnedTabAreaRight.Location = new System.Drawing.Point(675, 71);
            this._SchemeDesignerUnpinnedTabAreaRight.Name = "_SchemeDesignerUnpinnedTabAreaRight";
            this._SchemeDesignerUnpinnedTabAreaRight.Owner = this.ultraDockManager;
            this._SchemeDesignerUnpinnedTabAreaRight.Size = new System.Drawing.Size(0, 441);
            this._SchemeDesignerUnpinnedTabAreaRight.TabIndex = 1;
            // 
            // _SchemeDesignerUnpinnedTabAreaTop
            // 
            this._SchemeDesignerUnpinnedTabAreaTop.Dock = System.Windows.Forms.DockStyle.Top;
            this._SchemeDesignerUnpinnedTabAreaTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._SchemeDesignerUnpinnedTabAreaTop.Location = new System.Drawing.Point(0, 71);
            this._SchemeDesignerUnpinnedTabAreaTop.Name = "_SchemeDesignerUnpinnedTabAreaTop";
            this._SchemeDesignerUnpinnedTabAreaTop.Owner = this.ultraDockManager;
            this._SchemeDesignerUnpinnedTabAreaTop.Size = new System.Drawing.Size(675, 0);
            this._SchemeDesignerUnpinnedTabAreaTop.TabIndex = 2;
            // 
            // _SchemeDesignerUnpinnedTabAreaBottom
            // 
            this._SchemeDesignerUnpinnedTabAreaBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._SchemeDesignerUnpinnedTabAreaBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._SchemeDesignerUnpinnedTabAreaBottom.Location = new System.Drawing.Point(0, 512);
            this._SchemeDesignerUnpinnedTabAreaBottom.Name = "_SchemeDesignerUnpinnedTabAreaBottom";
            this._SchemeDesignerUnpinnedTabAreaBottom.Owner = this.ultraDockManager;
            this._SchemeDesignerUnpinnedTabAreaBottom.Size = new System.Drawing.Size(675, 0);
            this._SchemeDesignerUnpinnedTabAreaBottom.TabIndex = 3;
            // 
            // _SchemeDesignerAutoHideControl
            // 
            this._SchemeDesignerAutoHideControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._SchemeDesignerAutoHideControl.Location = new System.Drawing.Point(0, 0);
            this._SchemeDesignerAutoHideControl.Name = "_SchemeDesignerAutoHideControl";
            this._SchemeDesignerAutoHideControl.Owner = this.ultraDockManager;
            this._SchemeDesignerAutoHideControl.Size = new System.Drawing.Size(0, 0);
            this._SchemeDesignerAutoHideControl.TabIndex = 4;
            // 
            // MainToolbarsManager
            // 
            this.MainToolbarsManager.DesignerFlags = 1;
            this.MainToolbarsManager.DockWithinContainer = this;
            this.MainToolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.MainToolbarsManager.ImageTransparentColor = System.Drawing.Color.Lime;
            this.MainToolbarsManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.IsMainMenuBar = true;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool19,
            popupMenuTool20,
            popupMenuTool21,
            popupMenuTool22,
            popupMenuTool23,
            popupMenuTool24});
            ultraToolbar1.Text = "MenuBar";
            ultraToolbar2.DockedColumn = 0;
            ultraToolbar2.DockedRow = 1;
            ultraToolbar2.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool25,
            popupMenuTool26});
            ultraToolbar2.Text = "NavigationBar";
            this.MainToolbarsManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2});
            popupMenuTool27.SharedProps.Caption = "&Вид";
            popupMenuTool27.SharedProps.MergeOrder = 3;
            stateButtonTool33.Checked = true;
            stateButtonTool34.Checked = true;
            stateButtonTool35.Checked = true;
            popupMenuTool27.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool33,
            stateButtonTool34,
            stateButtonTool35,
            stateButtonTool36,
            stateButtonTool37,
            stateButtonTool38,
            stateButtonTool39,
            stateButtonTool40,
            stateButtonTool41,
            stateButtonTool42,
            stateButtonTool43});
            popupMenuTool28.SharedProps.Caption = "С&ервис";
            popupMenuTool28.SharedProps.MergeOrder = 500;
            popupMenuTool28.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool11,
            popupMenuTool29,
            stateButtonTool44,
            buttonTool12,
            buttonTool13,
            buttonTool1});
            stateButtonTool45.Checked = true;
            appearance1.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.ObjectTreeView;
            stateButtonTool45.SharedProps.AppearancesSmall.Appearance = appearance1;
            stateButtonTool45.SharedProps.Caption = "Дерево объектов";
            stateButtonTool45.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            stateButtonTool46.Checked = true;
            appearance2.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Properties;
            stateButtonTool46.SharedProps.AppearancesSmall.Appearance = appearance2;
            stateButtonTool46.SharedProps.Caption = "Свойства объекта";
            appearance3.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Semantics;
            stateButtonTool47.SharedProps.AppearancesSmall.Appearance = appearance3;
            stateButtonTool47.SharedProps.Caption = "Список семантик";
            appearance4.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.DataSuppliers;
            stateButtonTool48.SharedProps.AppearancesSmall.Appearance = appearance4;
            stateButtonTool48.SharedProps.Caption = "Поставщики данных";
            appearance5.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.DataKinds;
            stateButtonTool49.SharedProps.AppearancesSmall.Appearance = appearance5;
            stateButtonTool49.SharedProps.Caption = "Виды поступающей информации";
            popupMenuTool30.SharedProps.Caption = "&Файл";
            popupMenuTool30.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool14});
            popupMenuTool31.SharedProps.Caption = "&Окно";
            popupMenuTool31.SharedProps.MergeOrder = 998;
            stateButtonTool50.Checked = true;
            stateButtonTool50.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            popupMenuTool31.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool50,
            mdiWindowListTool3});
            popupMenuTool32.SharedProps.Caption = "&Справка";
            popupMenuTool32.SharedProps.MergeOrder = 999;
            popupMenuTool32.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool15});
            appearance6.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.About;
            buttonTool16.SharedProps.AppearancesSmall.Appearance = appearance6;
            buttonTool16.SharedProps.Caption = "&О программе";
            mdiWindowListTool4.SharedProps.Caption = "MDIWindowListTool";
            stateButtonTool51.Checked = true;
            stateButtonTool51.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool51.SharedProps.Caption = "В виде &закладок";
            appearance7.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Refresh;
            stateButtonTool52.SharedProps.AppearancesSmall.Appearance = appearance7;
            stateButtonTool52.SharedProps.Caption = "&Обновление схемы";
            appearance8.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Sessions;
            stateButtonTool53.SharedProps.AppearancesSmall.Appearance = appearance8;
            stateButtonTool53.SharedProps.Caption = "&Сессии";
            appearance9.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Exit;
            buttonTool17.SharedProps.AppearancesSmall.Appearance = appearance9;
            buttonTool17.SharedProps.Caption = "В&ыход";
            buttonTool17.SharedProps.CustomizerCaption = "В&ыход";
            stateButtonTool54.Checked = true;
            appearance10.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.DeveloperDescription;
            stateButtonTool54.SharedProps.AppearancesSmall.Appearance = appearance10;
            stateButtonTool54.SharedProps.Caption = "Описание &разработчика";
            appearance41.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.FullScreen;
            stateButtonTool55.SharedProps.AppearancesSmall.Appearance = appearance41;
            stateButtonTool55.SharedProps.Caption = "Во весь &экран";
            stateButtonTool55.SharedProps.Shortcut = System.Windows.Forms.Shortcut.F11;
            popupMenuTool33.SharedProps.Caption = "Правка";
            popupMenuTool33.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool56});
            popupMenuTool34.DropDownArrowStyle = Infragistics.Win.UltraWinToolbars.DropDownArrowStyle.Segmented;
            appearance42.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Back;
            popupMenuTool34.SharedProps.AppearancesSmall.Appearance = appearance42;
            popupMenuTool34.SharedProps.Caption = "Назад";
            popupMenuTool35.DropDownArrowStyle = Infragistics.Win.UltraWinToolbars.DropDownArrowStyle.Segmented;
            appearance43.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Forward;
            popupMenuTool35.SharedProps.AppearancesSmall.Appearance = appearance43;
            popupMenuTool35.SharedProps.Caption = "Вперед";
            appearance44.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.SearchResults;
            stateButtonTool57.SharedProps.AppearancesSmall.Appearance = appearance44;
            stateButtonTool57.SharedProps.Caption = "Результаты поиска";
            appearance45.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.Search;
            stateButtonTool58.SharedProps.AppearancesSmall.Appearance = appearance45;
            stateButtonTool58.SharedProps.Caption = "Поиск";
            stateButtonTool58.SharedProps.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            appearance46.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.MacroSet;
            stateButtonTool59.SharedProps.AppearancesSmall.Appearance = appearance46;
            stateButtonTool59.SharedProps.Caption = "Обработчики событий";
            appearance47.Image = ((object)(resources.GetObject("appearance47.Image")));
            popupMenuTool36.SharedProps.AppearancesSmall.Appearance = appearance47;
            popupMenuTool36.SharedProps.Caption = "Создание справки по кубам";
            popupMenuTool36.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool60,
            stateButtonTool61});
            stateButtonTool62.SharedProps.Caption = "FMMD_All из VSS";
            stateButtonTool63.SharedProps.Caption = "С выбором FMMD_All";
            appearance48.Image = ((object)(resources.GetObject("appearance48.Image")));
            stateButtonTool64.SharedProps.AppearancesSmall.Appearance = appearance48;
            stateButtonTool64.SharedProps.Caption = "Перенос описаний семантической структуры в FMMD_All";
            appearance49.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.createDBHelp;
            buttonTool18.SharedProps.AppearancesSmall.Appearance = appearance49;
            buttonTool18.SharedProps.Caption = "Создание справки по семантической структуре";
            appearance50.Image = global::Krista.FM.Client.SchemeDesigner.Properties.Resources.CutFMMD_All;
            buttonTool19.SharedProps.AppearancesSmall.Appearance = appearance50;
            buttonTool19.SharedProps.Caption = "Создание урезанной FMMD_All";
            buttonTool20.SharedProps.Caption = "Таблица конфликтов ";
            this.MainToolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool27,
            popupMenuTool28,
            stateButtonTool45,
            stateButtonTool46,
            stateButtonTool47,
            stateButtonTool48,
            stateButtonTool49,
            popupMenuTool30,
            popupMenuTool31,
            popupMenuTool32,
            buttonTool16,
            mdiWindowListTool4,
            stateButtonTool51,
            stateButtonTool52,
            stateButtonTool53,
            buttonTool17,
            stateButtonTool54,
            stateButtonTool55,
            popupMenuTool33,
            popupMenuTool34,
            popupMenuTool35,
            stateButtonTool57,
            stateButtonTool58,
            stateButtonTool59,
            popupMenuTool36,
            stateButtonTool62,
            stateButtonTool63,
            stateButtonTool64,
            buttonTool18,
            buttonTool19,
            buttonTool20});
            this.MainToolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.MainToolbarsManager_ToolClick);
            // 
            // _SchemeDesigner_Toolbars_Dock_Area_Left
            // 
            this._SchemeDesigner_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SchemeDesigner_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._SchemeDesigner_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._SchemeDesigner_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SchemeDesigner_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 71);
            this._SchemeDesigner_Toolbars_Dock_Area_Left.Name = "_SchemeDesigner_Toolbars_Dock_Area_Left";
            this._SchemeDesigner_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 441);
            this._SchemeDesigner_Toolbars_Dock_Area_Left.ToolbarsManager = this.MainToolbarsManager;
            // 
            // _SchemeDesigner_Toolbars_Dock_Area_Right
            // 
            this._SchemeDesigner_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SchemeDesigner_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._SchemeDesigner_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._SchemeDesigner_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SchemeDesigner_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(675, 71);
            this._SchemeDesigner_Toolbars_Dock_Area_Right.Name = "_SchemeDesigner_Toolbars_Dock_Area_Right";
            this._SchemeDesigner_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 441);
            this._SchemeDesigner_Toolbars_Dock_Area_Right.ToolbarsManager = this.MainToolbarsManager;
            // 
            // _SchemeDesigner_Toolbars_Dock_Area_Top
            // 
            this._SchemeDesigner_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SchemeDesigner_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._SchemeDesigner_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._SchemeDesigner_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SchemeDesigner_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._SchemeDesigner_Toolbars_Dock_Area_Top.Name = "_SchemeDesigner_Toolbars_Dock_Area_Top";
            this._SchemeDesigner_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(675, 71);
            this._SchemeDesigner_Toolbars_Dock_Area_Top.ToolbarsManager = this.MainToolbarsManager;
            // 
            // _SchemeDesigner_Toolbars_Dock_Area_Bottom
            // 
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 512);
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.Name = "_SchemeDesigner_Toolbars_Dock_Area_Bottom";
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(675, 0);
            this._SchemeDesigner_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.MainToolbarsManager;
            // 
            // windowDockingArea2
            // 
            this.windowDockingArea2.Controls.Add(this.dockableWindow4);
            this.windowDockingArea2.Controls.Add(this.dockableWindow11);
            this.windowDockingArea2.Controls.Add(this.dockableWindow10);
            this.windowDockingArea2.Dock = System.Windows.Forms.DockStyle.Left;
            this.windowDockingArea2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea2.Location = new System.Drawing.Point(0, 46);
            this.windowDockingArea2.Name = "windowDockingArea2";
            this.windowDockingArea2.Owner = this.ultraDockManager;
            this.windowDockingArea2.Size = new System.Drawing.Size(271, 466);
            this.windowDockingArea2.TabIndex = 0;
            // 
            // dockableWindow4
            // 
            this.dockableWindow4.Controls.Add(this.objectsTreeView);
            this.dockableWindow4.Location = new System.Drawing.Point(-10000, 0);
            this.dockableWindow4.Name = "dockableWindow4";
            this.dockableWindow4.Owner = this.ultraDockManager;
            this.dockableWindow4.Size = new System.Drawing.Size(515, 149);
            this.dockableWindow4.TabIndex = 37;
            // 
            // dockableWindow11
            // 
            this.dockableWindow11.Controls.Add(this.developerDescriptionControl);
            this.dockableWindow11.Location = new System.Drawing.Point(-10000, 0);
            this.dockableWindow11.Name = "dockableWindow11";
            this.dockableWindow11.Owner = this.ultraDockManager;
            this.dockableWindow11.Size = new System.Drawing.Size(515, 142);
            this.dockableWindow11.TabIndex = 38;
            // 
            // dockableWindow10
            // 
            this.dockableWindow10.Controls.Add(this.semanticsGridControl);
            this.dockableWindow10.Location = new System.Drawing.Point(-10000, 5);
            this.dockableWindow10.Name = "dockableWindow10";
            this.dockableWindow10.Owner = this.ultraDockManager;
            this.dockableWindow10.Size = new System.Drawing.Size(675, 249);
            this.dockableWindow10.TabIndex = 39;
            // 
            // dockableWindow8
            // 
            this.dockableWindow8.Controls.Add(this.modificationsTreeControl);
            this.dockableWindow8.Location = new System.Drawing.Point(0, 0);
            this.dockableWindow8.Name = "dockableWindow8";
            this.dockableWindow8.Owner = this.ultraDockManager;
            this.dockableWindow8.Size = new System.Drawing.Size(515, 144);
            this.dockableWindow8.TabIndex = 40;
            // 
            // dockableWindow5
            // 
            this.dockableWindow5.Controls.Add(this.dataSuppliersGridControl);
            this.dockableWindow5.Location = new System.Drawing.Point(-10000, 0);
            this.dockableWindow5.Name = "dockableWindow5";
            this.dockableWindow5.Owner = this.ultraDockManager;
            this.dockableWindow5.Size = new System.Drawing.Size(266, 279);
            this.dockableWindow5.TabIndex = 41;
            // 
            // dockableWindow2
            // 
            this.dockableWindow2.Controls.Add(this.dataKindsGridControl);
            this.dockableWindow2.Location = new System.Drawing.Point(-10000, 0);
            this.dockableWindow2.Name = "dockableWindow2";
            this.dockableWindow2.Owner = this.ultraDockManager;
            this.dockableWindow2.Size = new System.Drawing.Size(100, 100);
            this.dockableWindow2.TabIndex = 42;
            // 
            // dockableWindow3
            // 
            this.dockableWindow3.Controls.Add(this.sessionGridControl);
            this.dockableWindow3.Location = new System.Drawing.Point(0, 0);
            this.dockableWindow3.Name = "dockableWindow3";
            this.dockableWindow3.Owner = this.ultraDockManager;
            this.dockableWindow3.Size = new System.Drawing.Size(515, 146);
            this.dockableWindow3.TabIndex = 43;
            // 
            // dockableWindow7
            // 
            this.dockableWindow7.Controls.Add(this.propertyGrid);
            this.dockableWindow7.Location = new System.Drawing.Point(0, 5);
            this.dockableWindow7.Name = "dockableWindow7";
            this.dockableWindow7.Owner = this.ultraDockManager;
            this.dockableWindow7.Size = new System.Drawing.Size(675, 100);
            this.dockableWindow7.TabIndex = 44;
            // 
            // dockableWindow12
            // 
            this.dockableWindow12.Controls.Add(this.searchTabControl);
            this.dockableWindow12.Location = new System.Drawing.Point(-10000, 0);
            this.dockableWindow12.Name = "dockableWindow12";
            this.dockableWindow12.Owner = this.ultraDockManager;
            this.dockableWindow12.Size = new System.Drawing.Size(515, 142);
            this.dockableWindow12.TabIndex = 45;
            // 
            // dockableWindow9
            // 
            this.dockableWindow9.Controls.Add(this.searchServiceControl);
            this.dockableWindow9.Location = new System.Drawing.Point(-10000, 0);
            this.dockableWindow9.Name = "dockableWindow9";
            this.dockableWindow9.Owner = this.ultraDockManager;
            this.dockableWindow9.Size = new System.Drawing.Size(515, 149);
            this.dockableWindow9.TabIndex = 46;
            // 
            // dockableWindow1
            // 
            this.dockableWindow1.Controls.Add(this.macroSetControl);
            this.dockableWindow1.Location = new System.Drawing.Point(-10000, 0);
            this.dockableWindow1.Name = "dockableWindow1";
            this.dockableWindow1.Owner = this.ultraDockManager;
            this.dockableWindow1.Size = new System.Drawing.Size(534, 161);
            this.dockableWindow1.TabIndex = 47;
            // 
            // windowDockingArea3
            // 
            this.windowDockingArea3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea3.Location = new System.Drawing.Point(0, 24);
            this.windowDockingArea3.Name = "windowDockingArea3";
            this.windowDockingArea3.Owner = this.ultraDockManager;
            this.windowDockingArea3.Size = new System.Drawing.Size(180, 262);
            this.windowDockingArea3.TabIndex = 21;
            // 
            // windowDockingArea9
            // 
            this.windowDockingArea9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea9.Location = new System.Drawing.Point(0, 238);
            this.windowDockingArea9.Name = "windowDockingArea9";
            this.windowDockingArea9.Owner = this.ultraDockManager;
            this.windowDockingArea9.Size = new System.Drawing.Size(180, 262);
            this.windowDockingArea9.TabIndex = 0;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "Дизайнер схем";
            this.notifyIcon.Text = "Дизайнер схем";
            this.notifyIcon.Visible = true;
            // 
            // windowDockingArea10
            // 
            this.windowDockingArea10.Controls.Add(this.dockableWindow8);
            this.windowDockingArea10.Controls.Add(this.dockableWindow5);
            this.windowDockingArea10.Controls.Add(this.dockableWindow2);
            this.windowDockingArea10.Controls.Add(this.dockableWindow3);
            this.windowDockingArea10.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.windowDockingArea10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea10.Location = new System.Drawing.Point(4, 4);
            this.windowDockingArea10.Name = "windowDockingArea10";
            this.windowDockingArea10.Owner = this.ultraDockManager;
            this.windowDockingArea10.Size = new System.Drawing.Size(675, 274);
            this.windowDockingArea10.TabIndex = 0;
            // 
            // windowDockingArea12
            // 
            this.windowDockingArea12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea12.Location = new System.Drawing.Point(0, 407);
            this.windowDockingArea12.Name = "windowDockingArea12";
            this.windowDockingArea12.Owner = this.ultraDockManager;
            this.windowDockingArea12.Size = new System.Drawing.Size(180, 262);
            this.windowDockingArea12.TabIndex = 37;
            // 
            // windowDockingArea13
            // 
            this.windowDockingArea13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea13.Location = new System.Drawing.Point(0, 267);
            this.windowDockingArea13.Name = "windowDockingArea13";
            this.windowDockingArea13.Owner = this.ultraDockManager;
            this.windowDockingArea13.Size = new System.Drawing.Size(404, 95);
            this.windowDockingArea13.TabIndex = 0;
            // 
            // windowDockingArea1
            // 
            this.windowDockingArea1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea1.Location = new System.Drawing.Point(0, 0);
            this.windowDockingArea1.Name = "windowDockingArea1";
            this.windowDockingArea1.Owner = this.ultraDockManager;
            this.windowDockingArea1.Size = new System.Drawing.Size(404, 95);
            this.windowDockingArea1.TabIndex = 24;
            // 
            // windowDockingArea5
            // 
            this.windowDockingArea5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea5.Location = new System.Drawing.Point(0, 162);
            this.windowDockingArea5.Name = "windowDockingArea5";
            this.windowDockingArea5.Owner = this.ultraDockManager;
            this.windowDockingArea5.Size = new System.Drawing.Size(266, 142);
            this.windowDockingArea5.TabIndex = 0;
            // 
            // windowDockingArea8
            // 
            this.windowDockingArea8.Controls.Add(this.dockableWindow7);
            this.windowDockingArea8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.windowDockingArea8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea8.Location = new System.Drawing.Point(4, 4);
            this.windowDockingArea8.Name = "windowDockingArea8";
            this.windowDockingArea8.Owner = this.ultraDockManager;
            this.windowDockingArea8.Size = new System.Drawing.Size(675, 245);
            this.windowDockingArea8.TabIndex = 0;
            // 
            // windowDockingArea11
            // 
            this.windowDockingArea11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea11.Location = new System.Drawing.Point(0, 62);
            this.windowDockingArea11.Name = "windowDockingArea11";
            this.windowDockingArea11.Owner = this.ultraDockManager;
            this.windowDockingArea11.Size = new System.Drawing.Size(404, 269);
            this.windowDockingArea11.TabIndex = 0;
            // 
            // windowDockingArea6
            // 
            this.windowDockingArea6.Controls.Add(this.dockableWindow12);
            this.windowDockingArea6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.windowDockingArea6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea6.Location = new System.Drawing.Point(271, 244);
            this.windowDockingArea6.Name = "windowDockingArea6";
            this.windowDockingArea6.Owner = this.ultraDockManager;
            this.windowDockingArea6.Size = new System.Drawing.Size(675, 100);
            this.windowDockingArea6.TabIndex = 31;
            // 
            // windowDockingArea14
            // 
            this.windowDockingArea14.Controls.Add(this.dockableWindow9);
            this.windowDockingArea14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea14.Location = new System.Drawing.Point(4, 4);
            this.windowDockingArea14.Name = "windowDockingArea14";
            this.windowDockingArea14.Owner = this.ultraDockManager;
            this.windowDockingArea14.Size = new System.Drawing.Size(515, 149);
            this.windowDockingArea14.TabIndex = 44;
            // 
            // windowDockingArea15
            // 
            this.windowDockingArea15.Dock = System.Windows.Forms.DockStyle.Right;
            this.windowDockingArea15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea15.Location = new System.Drawing.Point(4, 4);
            this.windowDockingArea15.Name = "windowDockingArea15";
            this.windowDockingArea15.Owner = this.ultraDockManager;
            this.windowDockingArea15.Size = new System.Drawing.Size(515, 466);
            this.windowDockingArea15.TabIndex = 0;
            // 
            // windowDockingArea17
            // 
            this.windowDockingArea17.Controls.Add(this.dockableWindow1);
            this.windowDockingArea17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowDockingArea17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.windowDockingArea17.Location = new System.Drawing.Point(4, 4);
            this.windowDockingArea17.Name = "windowDockingArea17";
            this.windowDockingArea17.Owner = this.ultraDockManager;
            this.windowDockingArea17.Size = new System.Drawing.Size(100, 100);
            this.windowDockingArea17.TabIndex = 0;
            // 
            // SchemeDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(675, 512);
            this.Controls.Add(this._SchemeDesignerAutoHideControl);
            this.Controls.Add(this.windowDockingArea2);
            this.Controls.Add(this.windowDockingArea10);
            this.Controls.Add(this.windowDockingArea8);
            this.Controls.Add(this.windowDockingArea6);
            this.Controls.Add(this.windowDockingArea15);
            this.Controls.Add(this._SchemeDesignerUnpinnedTabAreaTop);
            this.Controls.Add(this._SchemeDesignerUnpinnedTabAreaBottom);
            this.Controls.Add(this._SchemeDesignerUnpinnedTabAreaLeft);
            this.Controls.Add(this._SchemeDesignerUnpinnedTabAreaRight);
            this.Controls.Add(this._SchemeDesigner_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._SchemeDesigner_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._SchemeDesigner_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._SchemeDesigner_Toolbars_Dock_Area_Bottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Name = "SchemeDesigner";
            this.Text = "Дизайнер схем";
            this.Load += new System.EventHandler(this.SchemeDesigner_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SchemeDesigner_DragDrop);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SchemeDesigner_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SchemeDesigner_KeyPress);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SchemeDesigner_FormClosing);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.SchemeDesigner_DragOver);
            ((System.ComponentModel.ISupportInitialize)(this.objectsTreeView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabbedMdiManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainToolbarsManager)).EndInit();
            this.windowDockingArea2.ResumeLayout(false);
            this.dockableWindow4.ResumeLayout(false);
            this.dockableWindow11.ResumeLayout(false);
            this.dockableWindow10.ResumeLayout(false);
            this.dockableWindow8.ResumeLayout(false);
            this.dockableWindow5.ResumeLayout(false);
            this.dockableWindow2.ResumeLayout(false);
            this.dockableWindow3.ResumeLayout(false);
            this.dockableWindow7.ResumeLayout(false);
            this.dockableWindow12.ResumeLayout(false);
            this.dockableWindow9.ResumeLayout(false);
            this.dockableWindow1.ResumeLayout(false);
            this.windowDockingArea10.ResumeLayout(false);
            this.windowDockingArea8.ResumeLayout(false);
            this.windowDockingArea6.ResumeLayout(false);
            this.windowDockingArea14.ResumeLayout(false);
            this.windowDockingArea17.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager ultraTabbedMdiManager;
        private Infragistics.Win.UltraWinDock.AutoHideControl _SchemeDesignerAutoHideControl;
        private Infragistics.Win.UltraWinDock.UltraDockManager ultraDockManager;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _SchemeDesignerUnpinnedTabAreaTop;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _SchemeDesignerUnpinnedTabAreaBottom;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _SchemeDesignerUnpinnedTabAreaLeft;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _SchemeDesignerUnpinnedTabAreaRight;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SchemeDesigner_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager MainToolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SchemeDesigner_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SchemeDesigner_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SchemeDesigner_Toolbars_Dock_Area_Bottom;

        private Krista.FM.Client.SchemeEditor.ObjectsTreeView objectsTreeView;
        private Krista.FM.Client.SchemeEditor.DeveloperDescriptionControl developerDescriptionControl;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private Krista.FM.Client.Design.Editors.SemanticsGridControl semanticsGridControl;
        private Krista.FM.Client.SchemeEditor.DataSuppliersGridControl dataSuppliersGridControl;
        private Krista.FM.Client.SchemeEditor.DataKindsGridControl dataKindsGridControl;
        private Krista.FM.Client.SchemeEditor.ModificationsTreeControl modificationsTreeControl;
        private Krista.FM.Client.Common.SessionGrid sessionGridControl;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow4;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow5;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea3;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow2;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow3;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea2;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea1;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea5;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea8;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow7;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea9;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow8;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea6;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea11;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea12;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow9;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow10;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea10;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea13;
        private Krista.FM.Client.SchemeEditor.Services.SearchService.SearchTabControl searchTabControl;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea14;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow11;
        private Krista.FM.Client.SchemeEditor.Services.SearchService.SearchServiceControl searchServiceControl;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow12;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea15;
        private Infragistics.Win.UltraWinDock.DockableWindow dockableWindow1;
        private Krista.FM.Client.SchemeEditor.MacroSetControl macroSetControl;
        private Infragistics.Win.UltraWinDock.WindowDockingArea windowDockingArea17;
    }
}

