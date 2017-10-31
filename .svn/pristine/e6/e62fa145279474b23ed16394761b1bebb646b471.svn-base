using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.Common;
using Krista.FM.Client.Help;
using Krista.FM.Client.HelpGenerator;
using Krista.FM.Common;
using Krista.FM.Client.Common.Forms;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeDesigner
{
    /// <summary>
    /// Главная форма дизайнера
    /// </summary>
    public partial class SchemeDesigner : Form/*, ISchemeDesigner*/
    {
        private const string SettingsFileName = "Krista.FM.Client.SchemeDesigner.usersettings";
        private FormWindowState defaultWindowState = FormWindowState.Normal;
        private Rectangle normalBounds = new Rectangle(0, 0, 640, 480);
        private IScheme scheme;
        /// <summary>
        /// Режим работы с различными версиями клиента и сервера
        /// </summary>
        private bool differentVersionsMode;

        /// <summary>
        /// Текст ошибки при подключение к серверу, при которой продолжаем работу
        /// </summary>
        private string connectionErrorMessage;

        /// <summary>
        /// Инициализация экземпляра формы дизайнера.
        /// </summary>
        public SchemeDesigner(IScheme scheme, bool differentVersionsMode, string connectionErrorMessage)
        {
            this.scheme = scheme;
            this.differentVersionsMode = differentVersionsMode;
            this.connectionErrorMessage = connectionErrorMessage;

            SchemeEditor.SetMainForm(this);

            InitializeComponent();
            //this.Visible = false;

            this.Controls.Add(Krista.FM.Client.SchemeEditor.Services.StatusBarService.Control);

            SchemeEditor.ObjectsTreeView = this.objectsTreeView;
            SchemeEditor.PropertyGrid = this.propertyGrid;
            SchemeEditor.DeveloperDescriptionControl = this.developerDescriptionControl;
            SchemeEditor.SemanticsGridControl = this.semanticsGridControl;
            SchemeEditor.DataSuppliersGridControl = this.dataSuppliersGridControl;
            SchemeEditor.DataKindsGridControl = this.dataKindsGridControl;
            SchemeEditor.ModificationsTreeControl = this.modificationsTreeControl;
            SchemeEditor.TabbedMdiManager = this.ultraTabbedMdiManager;
            SchemeEditor.DockManager = this.ultraDockManager;
            SchemeEditor.ToolbarManager = this.MainToolbarsManager;
            SchemeEditor.SearchTabControl = this.searchTabControl;
            SchemeEditor.MacroSetControl = this.macroSetControl;
            SchemeEditor.SessionGridControl = this.sessionGridControl;

            MainToolbarsManager.Tools["MDHelp_cl"].SharedProps.Enabled =
                (scheme.Server.GetConfigurationParameter("SourceSafeIniFile") ==
                 null)
                    ? false
                    : true;

            Design.Editors.DataSourcesTreeForm form =
                Design.Editors.DataSourcesTreeForm.Instance;
            form.DataSourceControl.DataSourcesManager = scheme.DataSourceManager;

            DefaultFormState.Load(this);
        }

        public bool InitializeSchemeDesigner()
        {
            if (differentVersionsMode && !String.IsNullOrEmpty(connectionErrorMessage))
            {
                if (MessageBox.Show(connectionErrorMessage, "Предупреждение",
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    Close();
                    return false;
                }
            }

            return true;
        }

        private void ActivateFirstOpenedPane()
        {
            foreach (DockableControlPane pane in ultraDockManager.ControlPanes)
            {
                if (!pane.Closed)
                {
                    pane.Activate();
                    break;
                }
            }
        }

        private bool LoadSettings()
        {
            try
            {
                string fileName = string.Format(@"{0}\{1}", Path.GetDirectoryName(Application.ExecutablePath), SettingsFileName);

                if (!System.IO.File.Exists(fileName))
                    return false;

                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    this.MainToolbarsManager.LoadFromBinary(fs);
                    this.ultraDockManager.LoadFromBinary(fs);
                    this.ultraTabbedMdiManager.LoadFromBinary(fs);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void SaveSettings()
        {
            string fileName = string.Format(@"{0}\{1}", Path.GetDirectoryName(Application.ExecutablePath), SettingsFileName);

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                this.MainToolbarsManager.SaveAsBinary(fs, true);
                this.ultraDockManager.SaveAsBinary(fs);
                this.ultraTabbedMdiManager.SaveAsBinary(fs);
            }
        }

        private void SchemeDesigner_Load(object sender, EventArgs e)
        {
            if (scheme == null)
            {
                this.Close();
                return;
            }

            LogicalCallContextData lccd = LogicalCallContextData.GetContext();

            lccd["Supervisor"] = CommandLineUtils.ParameterPresent("Supervisor");
            lccd["IgnoreVersions"] = CommandLineUtils.ParameterPresent("IgnoreVersions");

            try
            {
                SchemeEditor.Operation.Text = "Загрузка...";
                SchemeEditor.Operation.StartOperation();

                SchemeEditor.Scheme = scheme;

               this.LoadSettings();               

                ActivateFirstOpenedPane();

                SchemeEditor.Initialize();

                this.InitializeMenu();

                //Infragistics.Win.AppStyling.StyleManager.Load(@".\Styles\Office 2007 (голубая).isl");

                // Запуск процесса обновления
                UpdateFrameworkLibraryFactory.SetPropertyValue("Scheme", scheme);
                UpdateFrameworkLibraryFactory.InvokeMethod("InitializeNotifyIconForm");

                if (differentVersionsMode && !String.IsNullOrEmpty(connectionErrorMessage))
                {
                    Text += " [Режим обновления клиента]";
                    MessageBox.Show(
                        "SchemeDesigner запущен в аварийном режиме. Пожалуйста, дождитесь, пока будет получена новая версия, и установите ее.",
                        "Предупреждение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            finally
            {
                SchemeEditor.Operation.StopOperation();
                this.Visible = true;
                this.Activate();
            }
        }

        /// <summary>
        /// Инициализация меню в зависимости от возможности сервера работать с VSS
        /// </summary>
        private void InitializeMenu()
        {

            MainToolbarsManager.Tools["MDHelp_cl"].SharedProps.Enabled = (scheme.Server.GetConfigurationParameter("SourceSafeIniFile") ==
                                     null)
                                        ? false
                                        : true;
            MainToolbarsManager.Tools["compareDescription"].SharedProps.Enabled = (scheme.Server.GetConfigurationParameter("SourceSafeIniFile") ==
                                     null)
                                        ? false
                                        : true;
        }

        private void SchemeDesigner_FormClosed(object sender, FormClosedEventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }

        private void SchemeDesigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveSettings();
        }

        private void SchemeDesigner_DragOver(object sender, DragEventArgs e)
        {
            //e.Effect = DragDropEffects.All;

        }

        private void SchemeDesigner_DragDrop(object sender, DragEventArgs e)
        {
            //Get the Data and put it into a SelectedNodes collection,
            //then clone it and work with the clone
            //These are the nodes that are being dragged and dropped
            SelectedNodesCollection SelectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));
        }

        private void SchemeDesigner_KeyPress(object sender, KeyPressEventArgs e)
        {
            SchemeEditor.OnKeyPress(sender, e);
        }

        private static Infragistics.Win.UltraWinToolbars.StateButtonTool GetStateButtonTool(Infragistics.Win.UltraWinToolbars.ToolBase tool)
        {
            return tool as Infragistics.Win.UltraWinToolbars.StateButtonTool;
        }

        private static Infragistics.Win.UltraWinToolbars.ButtonTool GetButtonTool(Infragistics.Win.UltraWinToolbars.ToolBase tool)
        {
            return tool as Infragistics.Win.UltraWinToolbars.ButtonTool;
        }

        private void MainToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            bool enabled;
            switch (e.Tool.Key)
            {
                case "Exit":
                    {
                        this.Close();
                        break;
                    }
                case "objectsTreeView":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["objectsTreeView"].Closed = !enabled;
                        if (enabled)
                        {
                            this.ultraDockManager.ControlPanes["objectsTreeView"].Activate();
                        }
                        break;
                    }
                case "propertyGrid":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["propertyGrid"].Closed = !enabled;
                        if (enabled)
                            this.ultraDockManager.ControlPanes["propertyGrid"].Activate();
                        break;
                    }
                case "developerDescriptionControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["developerDescriptionControl"].Closed = !enabled;
                        if (enabled)
                        {
                            //this.developerDescriptionControl.Text = SchemeEditor.Scheme.SessionManager.Sessions;
                            this.ultraDockManager.ControlPanes["developerDescriptionControl"].Activate();
                        }
                        //else
                        //    this.developerDescriptionControl.Text = String.Empty;
                        break;
                    }
                case "semanticsGridControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["semanticsGridControl"].Closed = !enabled;
                        if (enabled)
                        {
                            semanticsGridControl.Value = SchemeEditor.Scheme.Semantics;
                            this.ultraDockManager.ControlPanes["semanticsGridControl"].Activate();
                        }
                        else
                            semanticsGridControl.Value = null;
                        break;
                    }
                case "dataSuppliersGridControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["dataSuppliersGridControl"].Closed = !enabled;
                        if (enabled)
                        {
                            dataSuppliersGridControl.DataSupplierCollection = SchemeEditor.Scheme.DataSourceManager.DataSuppliers;
                            this.ultraDockManager.ControlPanes["dataSuppliersGridControl"].Activate();
                        }
                        else
                            dataSuppliersGridControl.DataSupplierCollection = null;
                        break;
                    }
                case "dataKindsGridControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["dataKindsGridControl"].Closed = !enabled;
                        if (enabled)
                        {
                            dataKindsGridControl.DataSupplierCollection = SchemeEditor.Scheme.DataSourceManager.DataSuppliers;
                            this.ultraDockManager.ControlPanes["dataKindsGridControl"].Activate();
                        }
                        else
                            dataKindsGridControl.DataSupplierCollection = null;
                        break;
                    }
                case "modificationsTreeControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["modificationsTreeControl"].Closed = !enabled;
                        if (enabled)
                            this.ultraDockManager.ControlPanes["modificationsTreeControl"].Activate();
                        break;
                    }
                case "sessionGridControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["sessionGridControl"].Closed = !enabled;
                        if (enabled)
                        {
                            this.sessionGridControl.Sessions = SchemeEditor.Scheme.SessionManager.Sessions;
                            this.ultraDockManager.ControlPanes["sessionGridControl"].Activate();
                        }
                        else
                            this.sessionGridControl.Sessions = null;
                        break;
                    }
                case "searchTabControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["searchTabControl"].Closed = !enabled;
                        if (enabled)
                        {
                            this.ultraDockManager.ControlPanes["searchTabControl"].Activate();
                        }
                        break;
                    }
                case "macroSetControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["macroSetControl"].Closed = !enabled;
                        if (enabled)
                        {
                            this.ultraDockManager.ControlPanes["macroSetControl"].Activate();
                        }
                        break;
                    }
                case "TabbedMDI":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraTabbedMdiManager.Enabled = enabled;
                        break;
                    }
                case "FullScreen":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.FullScreen = enabled;
                        break;
                    }
                case "About":
                    {
                        Krista.FM.Client.Common.Forms.About.ShowAbout(this.scheme, (IWin32Window)(Control)this);
                        break;
                    }
                case "Back":
                    {
                        Krista.FM.Client.SchemeEditor.Services.NavigationService.NavigationService.Instance.Backward(Krista.FM.Client.SchemeEditor.SchemeEditor.Instance, 1);
                        Krista.FM.Client.SchemeEditor.SchemeEditor.Instance.NaviButtonProcessing();
                        break;
                    }
                case "Forward":
                    {
                        Krista.FM.Client.SchemeEditor.Services.NavigationService.NavigationService.Instance.Forward(Krista.FM.Client.SchemeEditor.SchemeEditor.Instance, 1);
                        Krista.FM.Client.SchemeEditor.SchemeEditor.Instance.NaviButtonProcessing();
                        break;
                    }
                case "searchServiceControl":
                    {
                        enabled = GetStateButtonTool(e.Tool).Checked;
                        this.ultraDockManager.ControlPanes["searchServiceControl"].Closed = !enabled;
                        if (enabled)
                        {
                            this.ultraDockManager.ControlPanes["searchServiceControl"].Activate();
                            this.ultraDockManager.ControlPanes["searchServiceControl"].DockAreaPane.Size = new Size(512, 145);
                        }
                        break;
                    }
                case "UMLHelp":
                    {
                        Parameters.Show(this);
                        break;
                    }
                case "MDHelp_cl":
                    {
                        HelpCubesManager cubesManager = new HelpCubesManager();
                        cubesManager.HelpGenerate();
                        break;
                    }
                case "MDHelp_all":
                    {
                        HelpGenerator.HelpGenerator form = new HelpGenerator.HelpGenerator();
                        form.ShowDialog(this);
                        break;
                    }
                case "compareDescription":
                    {
                        CompareDescription desc = new CompareDescription();
                        break;
                    }
                case "createCutFmmd_all":
                    {
                        CutFMMD_AllForm form = new CutFMMD_AllForm();
                        form.ShowDialog(this);
                        break;
                    }
                case "ConflictsTable ":
                    {
                        FormPackageConflicts conflictForm = new FormPackageConflicts(scheme);
                        conflictForm.ShowDialog(this);
                        break;
                    }
                case "FillVersions":
                    {
                        scheme.DataVersionsManager.FillObjectVersionTable();
                        break;
                    }
                default:
                    {  
                        Client.SchemeEditor.SchemeEditor.Instance.NaviMenuItemClicked(e.Tool);                    
                        Krista.FM.Client.SchemeEditor.SchemeEditor.Instance.NaviButtonProcessing();
                        break;
                    }
            }
        }


        private void ultraDockManager_AfterPaneButtonClick(object sender, Infragistics.Win.UltraWinDock.PaneButtonEventArgs e)
        {
            if (e.Button == PaneButton.Close || e.Button == PaneButton.Menu)
            {
                string menuTool = (e.Pane.Key == "searchServiceControl") ? "Edit" : "View";
                PopupMenuTool popupMenuTool = (PopupMenuTool)MainToolbarsManager.Toolbars["MenuBar"].Tools[menuTool];
                ((StateButtonTool)popupMenuTool.Tools[e.Pane.Key]).Checked = false;
            }
        }

        private void ultraDockManager_PropertyChanged(object sender, Infragistics.Win.PropertyChangedEventArgs e)
        {
            if (((Infragistics.Win.UltraWinDock.PropertyIds)e.ChangeInfo.PropId) == Infragistics.Win.UltraWinDock.PropertyIds.ControlPanes)
            {
                if (((Infragistics.Win.UltraWinDock.PropertyIds)e.ChangeInfo.Trigger.PropId) == Infragistics.Win.UltraWinDock.PropertyIds.DockableControlPane)
                {
                    if (((Infragistics.Win.UltraWinDock.PropertyIds)e.ChangeInfo.Trigger.Trigger.PropId) == Infragistics.Win.UltraWinDock.PropertyIds.Closed)
                    {
                        DockableControlPane pane = ((DockableControlPane)e.ChangeInfo.Trigger.Trigger.Source);
                        string menuTool = (pane.Key == "searchServiceControl") ? "Edit" : "View";
                        PopupMenuTool popupMenuTool = (PopupMenuTool)MainToolbarsManager.Toolbars["MenuBar"].Tools[menuTool];
                        ((StateButtonTool)popupMenuTool.Tools[pane.Key]).Checked = pane.Closed ? false : true;
                    }
                }
            }
        }

        private bool InFullScreen(Form f)
        {
            return f.FormBorderStyle == FormBorderStyle.None && f.WindowState == FormWindowState.Maximized;
        }

        public bool FullScreen
        {
            get
            {
                return InFullScreen(this);
            }
            set
            {
                if (InFullScreen(this) == value)
                    return;
                if (value)
                {
                    defaultWindowState = this.WindowState;
                    // - Hide window to prevet any further animations.
                    // - It fixes .NET Framework bug where the bounds of
                    //   visible window are set incorectly too.
                    this.Visible = false;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    this.normalBounds = this.Bounds;
                    this.Visible = true;
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.Bounds = normalBounds;
                    this.WindowState = defaultWindowState;
                }
                this.Invalidate();
            }
        }

        #region ISchemeDesigner Members

        public Krista.FM.Client.SchemeEditor.SchemeEditor SchemeEditor
        {
            get { return Krista.FM.Client.SchemeEditor.SchemeEditor.Instance; }
        }

        #endregion

    }
}