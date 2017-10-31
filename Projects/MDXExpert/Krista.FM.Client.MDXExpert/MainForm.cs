using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Connection;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Exporter;
using Krista.FM.Common.RegistryUtils;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.IO;
using System.Xml;
using System.Drawing;

using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.Printing;
using About=Krista.FM.Client.MDXExpert.Controls.About;
using ConnectionString=Krista.FM.Common.ConnectionString;
using Krista.FM.Client.MDXExpert.CommonClass;
using Krista.FM.Common;
using Axis = Krista.FM.Client.MDXExpert.Data.Axis;

namespace Krista.FM.Client.MDXExpert
{
    public partial class MainForm : Form
    {
        #region ����

        private Client.Common.Forms.Operation _operation;

        //������ ��������� ������
        private static MainForm _instance;
        //����������� � ����������
        private AdomdConnection _adomdConn;
        //����� ������ ����
        CubeChooser cubeChooser;
        //����� �������� � Excel
        ExcelExportForm _exportForm;
        //����� "� ���������..."
        About _aboutForm;
        //��������������� ����� �� ���������� ������
        ToolbarsManage _toolbarsManager;
        //��������� ������
        ReportExporter _exporter;
        //������� ������� ������
        private CustomReportElement activeElement = null;
        //������� ������������ ������
        private bool _saved;
        private bool _isSilentMode;
        private bool isReportLoading = false;
        //���� � ����� ������
        private string _reportFileName = String.Empty;
        private string _styleSheet;
        private MDXCommand _mdxCommand = new MDXCommand();
        private UndoRedoManager _undoRedoManager;
        private MRUManager _mruManager = new MRUManager();

        private static bool _isHideTotalsByDefault = true;

        // ����������� ����������� ���������
        private bool _isCompositeChart;

        //���� ���������� ���� ����������� � ��
        private DateTime _lastAdomdConnectionDate;

        //�������� ����� �������� �����
        private static Color _boderColor;
        //���� ������� ������
        private static Color _panelColor;
        //���� ������ ������
        private static Color _darkPanelColor;


        #endregion

        //��������� � ��� ��� ��������� ��������� ���������� 
        const int WM_ACTIVATEAPP = 0x1c;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ACTIVATEAPP)
            {
                this.ActivatedAppl();
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// ��� ��������� ����������, ����� ���������� ������������� � ������ ������ �������� 
        /// (���� ��� ����). � ������ ����������� ���������� ����.
        /// </summary>
        private void ActivatedAppl()
        {
            if (this.Operation != null)
            {
                try
                {
                    ShowMemberListIsVisible();

                    if (this.Operation.Visible)
                    {
                        this.Operation.SetTopIsVisible();
                        Application.DoEvents();
                    }
                }
                catch (Exception exc)
                {
                    this.Operation = new Operation();
                }
            }
        }

        private void ShowMemberListIsVisible()
        {
            if (this.ActiveElement != null)
            {
                this.ActiveElement.PivotData.MemberListSetTop();
            }
        }

        public DockPanelControl DockPanelControl
        {
            get { return this.dockPanelControl; }
        }

        public MainForm(string fileName)
        {
            _instance = this;
            this.Visible = false;
            this.Operation = new Operation();
            this.Operation.StartOperation();

            try
            {

                this.Operation.Text = "�������� ���������� ��������...";
                this.InitializeComponent();

                this.Operation.Text = "�������� �������� �� �������...";
                this.LoadRegSettings();

                this.Operation.Text = "�������� ��������� ��������...";
                this.CreateFieldObjects();
                this.CreateApplicationDirectorys();

                this.InitConnection();

                this.Operation.Text = "�������� �������� �����...";
                this.InitializeColorSchema();

                dockPanelControl.StyleSheet = this.StyleSheet;
                dockPanelControl.MainForm = this;
                dockPanelControl.udManager.PaneActivate += new ControlPaneEventHandler(ElementActivateHandler);
                fieldListEditor.CompositeChartEditor.MainForm = this;

                UpdateFrameworkLibraryFactory.InvokeMethod("InitializeNotifyIconForm");

                //��������� �� ������� ��� �������� Infragistics, ��� �������� ��������� � ��������
                InfragisticsCustomization.LocalizeAll();
                this.SetApplicationInformation();
                //���������� � %AppData% ���� ��� ���� ������� �����, � �� ���������� ��
                //������ ��, ���� �� ���������
                //Directory.Delete(Consts.UserAppDataPathForDelete, true);
            }
            finally
            {
                this.Operation.StopOperation();
                this.Visible = true;
                this.Activate();

                if (fileName != "")
                {
                    this.IsSilentMode = true;
                    OpenReport(fileName);
                    this.IsSilentMode = false;
                }
            }
        }

        private void CreateFieldObjects()
        {
            //����������� � ����������
            AdomdConn = new AdomdConnection();
            //����� ������ ����
            cubeChooser = new CubeChooser();
            //��������� ���������� adomd (������ �������� ������ �� ������� �����)
            AdomdExceptionHandler.mainForm = this;
            //�������� �������
            this.UndoRedoManager = new UndoRedoManager(100);

            //��������� ������ ����� ����� ������ ����� ������������� �������� ���������
            this.ToolbarsManager = new ToolbarsManage(this, this.ultraToolbarsManager);
            this.ToolbarsManager.Initialize();

            this.Exporter = new ReportExporter(this);
            this.ExportForm = new ExcelExportForm();
            this.ExportForm.Exporter = this.Exporter;

            this.AboutForm = new About(this);

            this.MRUManager.Initialize((ListTool)this.ultraToolbarsManager.Tools["RecentList"]);

        }

        /// <summary>
        /// ������� ������ ���������� ����������
        /// </summary>
        private void CreateApplicationDirectorys()
        {
            //�������� ���������� ��� ��������
            Directory.CreateDirectory(Consts.UserAppLogPath);
        }

        /// <summary>
        /// �������� ������� ���������� �� �������
        /// </summary>
        private void LoadRegSettings()
        {
            //������ ���� �������� ������������, ������ �� ����� � ����������             
            this.MdxCommand.LogPath = Consts.UserAppQueryLogPath;
            this.MdxCommand.IsKeepLog = true;

            string regValue = string.Empty;
            Utils regUtils = new Utils(GetType(), true);

            regValue = regUtils.GetKeyValue(Consts.isKeepQueryLogRegKey);
            if (regValue != string.Empty)
                this.MdxCommand.IsKeepLog = bool.Parse(regValue);

            //regValue = regUtils.GetKeyValue(Consts.connectionNameKey);
            //if (regValue != string.Empty)
            Consts.connectionName = Connection.Settings.GetConnectionName(); //regValue;

            //� ������ ������� ����������, ����� ������� ���
            this.MdxCommand.DeleteLog();

            regValue = regUtils.GetKeyValue(Consts.isHideTotalsByDefaultKey);
            MainForm.IsHideTotalsByDefault = (regValue != string.Empty) ? bool.Parse(regValue) : false;

            this.MRUManager.LoadMRU();

            //���������� ���������� ������� � ���������
            CommonUtils.AssociationReportExtension(Application.ExecutablePath);
        }

        /// <summary>
        /// ���������� ������� ���������� � ������
        /// </summary>
        private void SaveRegSettins()
        {
            Utils regUtils = new Utils(GetType(), true);
            regUtils.SetKeyValue(Consts.isKeepQueryLogRegKey, this.MdxCommand.IsKeepLog);
            
            regUtils.SetKeyValue(Consts.isHideTotalsByDefaultKey, MainForm.IsHideTotalsByDefault);
            Connection.Settings.SaveConnectionName();

            this.MRUManager.SaveMRU();
            //regUtils.SetKeyValue(Consts.connectionNameKey, Consts.connectionName);
        }

        /// <summary>
        /// ������������ ������� ��� ��������� � ����������� � �������� �������� ������.
        /// �������������� ��������, ������� ������ ���������� �������.
        /// ��� ����� ������������ ����������� ���� - ���� ���������� �������� �������
        /// </summary>
        /// <param name="stylesheetPath">���� � ������� ������</param>
        private void AdjustWindowViewByStylesheet(string stylesheetPath)
        {
            XmlDocument dom = new XmlDocument();
            try
            {
                dom.Load(stylesheetPath);
                XmlNode colorsNode = dom.SelectSingleNode("//expertBaseElementsColors");
                if (colorsNode != null)
                {
                    ColorConverter colorConvertor = new ColorConverter();
                    string strColor;

                    //���� �������
                    strColor = XmlHelper.GetStringAttrValue(colorsNode, "border", "127; 157; 185");
                    _boderColor = (Color)colorConvertor.ConvertFromString(strColor);

                    //���� ������� ������
                    strColor = XmlHelper.GetStringAttrValue(colorsNode, "panel", "193; 216; 240");
                    _panelColor = (Color)colorConvertor.ConvertFromString(strColor);

                    //���� ������ ������
                    strColor = XmlHelper.GetStringAttrValue(colorsNode, "darkpanel", "141; 178; 227");
                    _darkPanelColor = (Color)colorConvertor.ConvertFromString(strColor);

                    //������������� �����
                    panel2.Appearance.BackColor = _panelColor;
                    propertyGrid.BackColor = _panelColor;
                    propertyGrid.CommandsActiveLinkColor = _panelColor;
                    fieldListEditor.AdjustColors(_panelColor, _boderColor, _darkPanelColor);
                }
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref dom);
            }
        }


        /// <summary>
        ///  ������������� �������� ���� ���������
        /// a) ����� �������� ������ � ��������� ������ ���� � Application menu
        /// b) ��������� ������� ����� ���������
        /// </summary>
        private void InitializeColorSchema()
        {
            try
            {
                //��������� ������. � �������� ������ ���� ������ ����, �������� ��� ����� ��� ����������            
                ListTool list = (ListTool)this.ultraToolbarsManager.Tools["ColorSchemeList"];
                string defPath = GetDefaultStyleSheetPath();
                string[] files = Directory.GetFiles(Application.StartupPath + @"\Styles\", "*.isl", SearchOption.TopDirectoryOnly);
                try
                {
                    ToolbarsManager.IsMayHook = true;
                    foreach (string fileName in files)
                    {
                        list.ListToolItems.Add(Path.GetFullPath(fileName),
                            Path.GetFileNameWithoutExtension(fileName),
                            ((Path.GetFullPath(fileName) == defPath) || (defPath == "")));
                    }
                }
                finally
                {
                    ToolbarsManager.IsMayHook = false;
                }

                this.StyleSheet = defPath;
            }
            catch
            {
                #warning ��������� �� �������, ���� �������������� �����
            }
        }

        private string GetDefaultStyleSheetPath()
        {
            //������ ���������� ��������� �� ������� (��� ������ ������� ��� ����� �� ����)
            Utils regUtils = new Utils(GetType(), true);
            string defaultPath = regUtils.GetKeyValue(Consts.styleSheetPathRegKey);
            if (File.Exists(defaultPath))
            {
                return defaultPath;
            }

            //������ ���������  "������� �����" �� �����
            string blueOfficeSchemePath = Application.StartupPath + @"\Styles\Office 2007 (�������).isl";
            if (File.Exists(blueOfficeSchemePath))
            {
                return Path.GetFullPath(blueOfficeSchemePath);
            }

            return string.Empty;
        }

        /// <summary>
        /// �������� ���������� � ��
        /// </summary>
        /// <returns></returns>
        public AdomdConnection GetConnection()
        {
            string connFileName = String.Format("{0}\\{1}", Consts.ConnectionFolderPath, Consts.connectionName);
            if (!File.Exists(connFileName))
                return null;

            AdomdConnection result = new AdomdConnection();
            FM.Common.ConnectionString cs = new ConnectionString();

            //cs.ReadConnectionString(Application.StartupPath + @"\MAS.udl");
            cs.ReadConnectionString(connFileName);

            //������ ����������� 10��, ��������� �������� �� udl
            Consts.tmpProvider = "MSOLAP.4";//cs.Provider;
            Consts.tmpCatalogName = cs.InitialCatalog;
            Consts.tmpServerName = cs.DataSource;
            Consts.tmpConnectTo = cs.ConnectTo;
            Consts.tmpTimeout = cs.Timeout;
            Consts.tmpPassword = cs.Password;
            Consts.tmpUserID = cs.UserID;


            if (Consts.tmpCatalogName == String.Empty)
            {
                List<string> catalogs = CommonUtils.GetCatalogList(Consts.TmpConnStr);
                if (catalogs.Count > 0)
                {
                    Consts.tmpCatalogName = catalogs[0];
                }
            }

            result.Close();
            result.ConnectionString = Consts.TmpConnStr;
            result.Open();
            return result;
        }

        /// <summary>
        /// ���������� ���� ����������� �����������
        /// </summary>
        /// <returns>������� ������</returns>
        public bool InitConnection()
        {
            bool result = false;

            bool operationShowing = this.Operation.Visible;
            if (!operationShowing)
                this.Operation.StartOperation();
            this.Operation.Text = "����������� � �������...";

            try
            {
                this.AdomdConn = this.GetConnection();
                if (this.AdomdConn != null)
                {
                    Krista.FM.Client.MDXExpert.Data.PivotData.AdomdConn = AdomdConn;

                    if (ActiveElement != null)
                    {
                        fieldListEditor.InitEditor(ActiveElement);
                    }

                    this.LastAdomdConnectionDate = DateTime.Now;
                    result = true;
                }
            }
            catch (Exception e)
            {
                if (!this.IsSilentMode)
                {
                    if (e.Message == string.Empty)
                        e = new Exception("����������� ������, ��� ������� �����������.");
                    Common.CommonUtils.ProcessException(e, true);
                }
                result = false;
            }

            this.HostName = Consts.tmpServerName + " " + Consts.tmpCatalogName;

            cubeChooser.ClearCubeList();
            if (result)
            {
                //���� ���������� ����������, ������� ������ �����
                this.Operation.Text = "���������� ������ �����...";
                if (this.AdomdConn.Cubes.Count > 0)
                {
                    cubeChooser.LoadMetadata();
                }
            }
            else
            {
                this.HostName += Consts.disconnection;
            }

            //���� �������� ������ ���������� � ���� ������, �� ����� �� �� � �������
            if (!operationShowing)
                this.Operation.StopOperation();

            return result;
        }

        /// <summary>
        /// ���� ������������ ������� ��� ����� ����� ������.
        /// </summary>
        /// <returns>������ ���� �� ADOMD</returns>
        public CubeDef ChooseCube()
        {
            if (this.AdomdConn.Cubes.Count > 0)
            {
                cubeChooser.LoadMetadata();
            }
            string cubeName = cubeChooser.SelectCube("");

            if (!string.IsNullOrEmpty(cubeName))
            {
                try
                {
                    if (AdomdConn.Cubes.Find(cubeName) == null)
                        this.InitConnection();

                    return AdomdConn.Cubes[cubeName];
                }
                catch (Exception e)
                {

                    Common.CommonUtils.ProcessException(e);
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        private void ElementActivateHandler(object sender, ControlPaneEventArgs e)
        {
            if (!this.isReportLoading)
            {
                this.FindActiveElement(e.Pane, true);
                //���� ������� ��������� ������� ����� ���������� ����������, ��� ���������� �� ���������.
                this.SetActivationTable(e.Pane);
            }
        }

        /// <summary>
        /// ������������� �������, ������� ���������
        /// </summary>
        /// <param name="pane"></param>
        private void SetActivationTable(DockableControlPane pane)
        {
            if (pane != null)
            {
                CustomReportElement pp = ((CustomReportElement)pane.Control);
                if (pp.ElementType == ReportElementType.eTable)
                {
                    ((TableReportElement)pp).ExpertGrid.IsActivated = true;
                }
            }
        }

        private void reportObjectBox_SelectionChanged()
        {
            propertyGrid.SelectedObject = reportObjectBox.SelectedObject;
        }

        /// <summary>
        /// ����� � ��������� ��������� �������� ������
        /// </summary>
        /// <param name="pane">������ �� ������� ���������� ������� ������</param>
        /// <param name="skipIfSamePane">���� �������� ������ � ������ ������� ������ ���� 
        /// �������� ��������� �� �������, ������ �� �����</param>
        private void FindActiveElement(DockableControlPane pane, bool skipIfSamePane)
        {
            if (pane == null)
            {
                this.ActiveElement = null;
                fieldListEditor.ClearEditor();
                reportObjectBox.Clear();
                reportObjectBox_SelectionChanged();
                return;
            }

            if (skipIfSamePane && (pane == dockPanelControl.GetActivePane()))
            {
                return;
            }

            this.SetActivityIndicator(pane);

            CustomReportElement pp = ((CustomReportElement)pane.Control);

            if (pp != null)
            {
                pp.IsActive = true;
                if (pp.Visible)
                {
                    fieldListEditor.InitEditor(pp);
                    // ��������� ��� ����������� ���������
                    //RefreshCompositeCharts();
                }

                reportObjectBox.Init(pane);

                propertyGrid.SelectedObject = reportObjectBox.SelectedObject;
                propertyGrid.Refresh();
            }
            else
            {
                fieldListEditor.ClearEditor();
            }

            this.ActiveElement = pp;
        }

        public void SetActivityIndicator(DockableControlPane pane)
        {
            if ((pane != null) && (imageList1.Images.Count > 0))
                pane.Settings.Appearance.Image = imageList1.Images[0];
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        /// <param name="askSavePath">���� �� ���������� ���� ���������� � ������������</param>
        /// <param name="oldReportPath">���� ������� ��� �������� ��� �������� ���������</param>
        /// <retrurns>���� ��������� ������ - true, ���������� - false</retrurns>
        public bool SaveReport(bool askSavePath, string oldReportPath)
        {
            //���� ��������� ������, ����� ������ �������
            if (dockPanelControl.udManager.ControlPanes.Count == 0)
                return false;

            if ((askSavePath) || (oldReportPath == string.Empty))
            {
                // ����� �������� ��� ������ � ������������, ���� ��������� ��������������� �������
                // ��� ������ ���� �� ��������������� 
                if (oldReportPath == string.Empty)
                    saveReportDialog.FileName = saveReportDialog.InitialDirectory + Consts.newReportName + Consts.reportExt;
                else
                    saveReportDialog.FileName = oldReportPath;
            }
            else
            {
                //���� ������ "���������" �� �������� ����, �� ��� ������ ��������, ��������� � ������� ������
                this.Saved = dockPanelControl.SaveDockSettings(oldReportPath);
                this.MRUManager.Add(oldReportPath);
                return this.Saved;
            }

            if (saveReportDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveReportDialog.FilterIndex == 1)
                {
                    //���� ������� ��������� ��� ������ ��������, ������ ����� ����� ���������� �������
                    saveReportDialog.FileName = Common.CommonUtils.SetExtension(saveReportDialog.FileName,
                        Common.Consts.reportExt);
                }
                //���������� ��� ������ �� ������, ���� ������������ ����� ������� ��� 
                //���������, � �� ��� ��� ��������� ���� �������� ���                    
                this.ReportFileName = saveReportDialog.FileName;
                this.Saved = dockPanelControl.SaveDockSettings(this.ReportFileName);
                this.MRUManager.Add(this.ReportFileName);
                this.SetApplicationInformation();
                return this.Saved;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ������� �����
        /// </summary>
        public void OpenReport(string fileName)
        {
            //����� �������� ������ ������, ������� ������
            if (!this.CloseReport())
                return;

            if (fileName == "")
            {
                // �������� ������ �� �����
                if ((!String.IsNullOrEmpty(openReportDialog.FileName))&&(!File.Exists(openReportDialog.FileName)))
                {
                    openReportDialog.FileName = String.Empty;
                }

                if (openReportDialog.ShowDialog() != DialogResult.OK)
                    return;
            }
            else
            {
                fileName = fileName.TrimEnd(new char[] {'\\'});
                if (!File.Exists(fileName))
                {
                    return;
                }
                openReportDialog.FileName = fileName;
            }

            

            dockPanelControl.ResetDockSettings();

            if (!this.IsExistsConnection)
            {
                this.InitConnection();
            }

            this.Operation.StartOperation();
            this.Operation.Text = "�������� ������...";
            try
            {
                this.isReportLoading = true;
                this.IsSilentMode = true;

                this.ResetCompositeCharts();

                dockPanelControl.LoadDockSettings(openReportDialog.FileName);

                this.RefreshCompositeCharts();

                this.isReportLoading = false;
                this.IsSilentMode = false;

                if (dockPanelControl.udManager.ControlPanes.Count > 0)
                {
                    DockableControlPane cp = dockPanelControl.GetActivePane();
                    if (cp == null)
                    {
                        cp = dockPanelControl.udManager.ControlPanes[0] as DockableControlPane;
                    }
                    CustomReportElement pp = ((CustomReportElement)cp.Control);
                    pp.IsActive = false;

                    this.FindActiveElement(cp, true);
                    //this.ReportFileName = openReportDialog.FileName;
                    //this.SetApplicationInformation();
                }
                this.ReportFileName = openReportDialog.FileName;
                this.SetApplicationInformation();
                this.Saved = true;
                this.MRUManager.Add(this.ReportFileName);

            }
            catch (Exception exc)
            {
                this.MRUManager.Remove(fileName);
                this.Operation.StopOperation();
                Common.CommonUtils.ProcessException(exc);
            }
            this.Operation.StopOperation();

            this.UndoRedoManager.ClearEvents();
            //������� ������� ��������� ������ � �������� ������ ���������
            if (this.ActiveElement != null)
                this.UndoRedoManager.AddEvent(this.ActiveElement, UndoRedoEventType.AppearanceChange);

        }

        public void OpenMRUReport(string fileName)
        {
            OpenReport(fileName);
        }

        /// <summary>
        /// ������� �����
        /// </summary>
        /// <returns>���� ����� ������� ������ - true, ���� ������������ ��������� - false</returns>
        public bool CloseReport()
        {
            //���� ��������� ������, ����� ������ �������
            if (dockPanelControl.udManager.ControlPanes.Count == 0)
                return true;

            if (!this.Saved)
            {
                switch (MessageBox.Show("��������� ��������� � ������?", "MDX Expert",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.OK:
                    case DialogResult.Yes:
                        {
                            if (this.SaveReport(false, this.ReportFileName))
                                break;
                            else
                                return false;
                        }
                    case DialogResult.No:
                        {
                            break;
                        }
                    case DialogResult.Cancel:
                        {
                            return false;
                        }
                }
            }

            dockPanelControl.ResetDockSettings();
            this.ResetCompositeCharts();
            this.FindActiveElement(null, true);
            this.ReportFileName = string.Empty;
            this.SetApplicationInformation();
            this.Saved = true;
            this.UndoRedoManager.ClearEvents();
            return true;
        }

        public CustomReportElement CloneReportElement(CustomReportElement reportElement)
        {
            XmlNode savedElement = reportElement.Save();

            DockableControlPane cp = dockPanelControl.AddDockControlPane(reportElement.PivotData.CubeName,
                reportElement.ElementType);
            this.FindActiveElement(cp, true);
            CustomReportElement newReportElement = (CustomReportElement)cp.Control;
            if (newReportElement != null)
            {
                newReportElement.Load(savedElement, true);

                //���� � ���������� ������� ���� ��������(�������������) � ������� ����������, �� ������� ��
                if (newReportElement is TableReportElement)
                    ((TableReportElement)newReportElement).AnchoredElements.Clear();



                newReportElement.Title = this.dockPanelControl.GetCloneElementName(reportElement.Title);
                // ���������� ���� ������ ���� ������
                newReportElement.UniqueName = Guid.NewGuid().ToString();

                //���� � ���������� ��������� ���� ��������(�������������) � �������, �� ��������� �������� � ������� � � ������������� ���������
                string boundTo = String.Empty;
                if (newReportElement is ChartReportElement)
                {
                    boundTo = ((ChartReportElement)newReportElement).Synchronization.BoundTo;
                }
                else
                    if (newReportElement is MapReportElement)
                    {
                        boundTo = ((MapReportElement)newReportElement).Synchronization.BoundTo;
                    }
                    else
                        if (newReportElement is GaugeReportElement)
                        {
                            boundTo = ((GaugeReportElement)newReportElement).Synchronization.BoundTo;
                        }
                        else
                            if (newReportElement is MultipleGaugeReportElement)
                            {
                                boundTo = ((MultipleGaugeReportElement)newReportElement).Synchronization.BoundTo;
                            }

                if (boundTo != String.Empty)
                {
                    TableReportElement table = this.FindTableReportElement(boundTo);
                    if (table != null)
                    {
                        table.AnchoredElements.Add(newReportElement.UniqueName);
                    }
                }

                fieldListEditor.InitEditor(newReportElement);
            }

            this.Saved = false;
            return newReportElement;
        }

        /// <summary>
        /// ������� ����������� � ���� ������
        /// </summary>
        public void SelectConnection()
        {
            ConnectionForm connForm = new ConnectionForm(Consts.connectionName, false);
            if (connForm.ShowDialog() == DialogResult.OK)
            {
                if (dockPanelControl.udManager.ControlPanes.Count > 0)
                {
                    if (!this.Saved)
                    {
                        if (MessageBox.Show("��������� ��������� � ������?", "MDX Expert",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                            MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            this.SaveReport(false, this.ReportFileName);
                        }
                    }
                }
                Application.DoEvents();

                InitConnection();
                RefreshReportData();
                this._saved = true;
            }
        }

        /// <summary>
        /// �������� ������� �����������
        /// </summary>
        public void RefreshConnection()
        {
            InitConnection();
            RefreshReportData();
        }

        /// <summary>
        /// �������� ������ ��� ���� ��������� ������
        /// </summary>
        public void RefreshReportData()
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    RefreshElementData(element);
                }
            }
        }

        /// <summary>
        /// �������� ������ �������� ������
        /// </summary>
        /// <param name="element"></param>
        public void RefreshElementData(CustomReportElement element)
        {
            element.PivotData.CheckContent();
            element.PivotData.DoForceDataChanged();
        }

        /// <summary>
        /// �������� ������ ���� ��������� ������
        /// </summary>
        /// <returns></returns>
        public List<CustomReportElement> GetReportElementList()
        {
            List<CustomReportElement> result = new List<CustomReportElement>();
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    result.Add(element);
                }
            }
            return result;
        }

        /// <summary>
        /// ������� ����� �������
        /// </summary>
        /// <param name="elementType">��� ��������</param>
        /// <param name="elementSubType">������ ��������</param>
        /// <returns></returns>
        private CustomReportElement AddReportElement(ReportElementType elementType,
            ReportElementSubType elementSubType)
        {
            //���� ���������� �����������, ��� ������� ��� ����������, ����� ���������� 
            //����� ������ ����
            if (this.IsExistsConnection || this.InitConnection())
            {
                this.isReportLoading = true;
                DockableControlPane cp = null;
                try
                {
                    switch (elementSubType)
                    {
                        case ReportElementSubType.Standart:
                            {
                                CubeDef cubeDef = ChooseCube();
                                if (cubeDef != null)
                                {
                                    // ���������� ������ � DockManager
                                    cp = dockPanelControl.AddDockControlPane(cubeDef.Name, elementType);
                                    if (elementType == ReportElementType.eMap)
                                    {
                                        MapReportElement element = (MapReportElement)cp.Control;
                                        element.SelectTemplateName();
                                    }

                                }
                                else
                                    return null;

                                break;
                            }
                        case ReportElementSubType.Composite:
                            {
                                this.IsCompositeChart = true;
                                // ���������� ������ � DockManager
                                cp = dockPanelControl.AddDockControlPane(string.Empty, elementType);

                                CustomReportElement element = (CustomReportElement)cp.Control;
                                element.Title = "����������� ���������";
                                this.IsCompositeChart = false;
                                break;
                            }
                        case ReportElementSubType.CustomMDX:
                            {
                                MdxQueryEditor mdxQueryEditor = new MdxQueryEditor();
                                mdxQueryEditor.RefreshEditor(null);
                                if ((mdxQueryEditor.ShowDialog() == DialogResult.OK)
                                    && (mdxQueryEditor.Query != string.Empty))
                                {
                                    // ���������� ������ � DockManager
                                    cp = dockPanelControl.AddDockControlPane(string.Empty, elementType);
                                    CustomReportElement element = (CustomReportElement)cp.Control;
                                    element.PivotData.IsCustomMDX = true;
                                    element.PivotData.MDXQuery = mdxQueryEditor.Query;
                                    element.Title = "���������������� MDX ������";
                                    element.PivotData.DoDataChanged();
                                }
                                else
                                    return null;
                                break;
                            }
                        case ReportElementSubType.CustomData:
                            {
                                cp = dockPanelControl.AddDockControlPane(string.Empty, elementType);

                                if (elementType == ReportElementType.eMap)
                                {
                                    ((MapReportElement)cp.Control).DataSourceType = DataSourceType.Custom;
                                    ((MapReportElement)cp.Control).Title = "�����";
                                    ((MapReportElement)cp.Control).SelectTemplateName();
                                }
                                else
                                    if (elementType == ReportElementType.eGauge)
                                    {
                                        ((GaugeReportElement)cp.Control).Title = "���������";
                                    }
                                break;
                            }

                    }
                }
                finally
                {
                    this.isReportLoading = false;
                }
                this.FindActiveElement(cp, true);
                this.Saved = false;
                //�.� ������� �����, ����� ����������� ������� ��� � �������� ������� ����������
                this.UndoRedoManager.AddEvent((CustomReportElement)cp.Control,
                    UndoRedoEventType.DataChange, true);
                return (CustomReportElement)cp.Control;
            }
            else
            {
                MessageBox.Show("��� ����������� � ���� ������.", "MDX Expert",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return null;
        }

        /// <summary>
        /// �������� � ������ �������
        /// </summary>
        public void AddTable()
        {
            this.AddTable(ReportElementSubType.Standart);
        }

        /// <summary>
        /// �������� � ������ �������
        /// </summary>
        public void AddTable(ReportElementSubType elementSubType)
        {
            this.AddReportElement(ReportElementType.eTable, elementSubType);
        }

        /// <summary>
        /// �������� � ������ ���������
        /// </summary>
        public void AddChart()
        {
            this.AddChart(ReportElementSubType.Standart);
        }

        /// <summary>
        /// �������� � ������ ���������
        /// </summary>
        public void AddChart(ReportElementSubType elementSubType)
        {
            this.AddReportElement(ReportElementType.eChart, elementSubType);
        }

        /// <summary>
        /// �������� � ������ �����
        /// </summary>
        public void AddMap(ReportElementSubType elementSubType)
        {
            this.AddReportElement(ReportElementType.eMap, elementSubType);
        }

        /// <summary>
        /// �������� � ������ ���������
        /// </summary>
        public void AddGauge(ReportElementSubType elementSubType)
        {
            this.AddReportElement(ReportElementType.eGauge, elementSubType);
        }

        /// <summary>
        /// �������� � ������ ���������
        /// </summary>
        public void AddMultiGauge(ReportElementSubType elementSubType)
        {
            this.AddReportElement(ReportElementType.eMultiGauge, elementSubType);
        }


        public void ShowMdxQueryForm()
        {
            // �������������� MDX �������
            if (ActiveElement != null)
            {
                MDXQueryForm mdxQueryForm = new MDXQueryForm(ActiveElement);
                mdxQueryForm.Show();
            }
        }

        /// <summary>
        /// �������� ������� ������� � �����
        /// </summary>
        public void RefreshUserInterface(CustomReportElement element)
        {
            DockableControlPane pane = ((DockableWindow)element.Parent).Pane;
            this.ToolbarsManager.RefreshTabs(element);
            this.FindActiveElement(pane, false);
        }

        /// <summary>
        /// ��������� ��������� �������� ������
        /// </summary>
        /// <param name="value"></param>
        private void SetActiveElement(CustomReportElement value)
        {
            this.activeElement = value;
            //�������������� ���������� ��������� �������
            this.ToolbarsManager.RefreshTabs(value);
        }

        /// <summary>
        /// ��������� ��������� �������� ������ �� �����
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>�������� ��������</returns>
        public string GetReportElementText(string key)
        {
            foreach (DockableControlPane pane in this.ControlPanes)
            {
                if (((CustomReportElement)pane.Control).UniqueName == key)
                {
                    return pane.Text;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// ������� ������� ������� �� ������
        /// </summary>
        public void RemoveActiveElement()
        {
            if (activeElement != null)
            {
                DockableControlPane pane = dockPanelControl.GetActivePane();
                if ((pane != null) && (pane.Control != null))
                {
                    if (this.DeletingElementRequest(pane.Control))
                    {
                        CustomReportElement element = (CustomReportElement)pane.Control;
                        string elementUN = element.UniqueName;

                        if (pane.Control is TableReportElement)
                        {
                            TableReportElement tableElement = (TableReportElement)element;
                            List<string> anchElems = new List<string>();
                            anchElems.AddRange(tableElement.AnchoredElements);

                            foreach (string key in anchElems)
                            {
                                ChartReportElement chartElement = this.FindChartReportElement(key);
                                if (chartElement != null)
                                {
                                    chartElement.Synchronization.BoundTo = "";
                                }
                                else
                                {
                                    MapReportElement mapElement = this.FindMapReportElement(key);
                                    if (mapElement != null)
                                    {
                                        mapElement.Synchronization.BoundTo = "";
                                    }

                                    else
                                    {
                                        GaugeReportElement gaugeElement = this.FindGaugeReportElement(key);
                                        if (gaugeElement != null)
                                        {
                                            gaugeElement.Synchronization.BoundTo = "";
                                        }
                                        else
                                        {
                                            MultipleGaugeReportElement multiGaugeElement = this.FindMultiGaugeReportElement(key);
                                            if (multiGaugeElement != null)
                                            {
                                                multiGaugeElement.Synchronization.BoundTo = "";
                                            }

                                        }
                                    }
                                }
                            }

                        }
                        else
                            if (pane.Control is ChartReportElement)
                            {
                                ChartReportElement chartElement = (ChartReportElement)element;

                                if (chartElement.IsCompositeChart)
                                {
                                    // ������������ ����� ��������� ��������
                                    RemoveCompositeParentKey(elementUN);
                                    chartElement.ResetCompositeChart();
                                }
                                else
                                {
                                    if (this.IsCompositeChildChart(elementUN))
                                    {
                                        // ������� ������������� �� ��������
                                        string elementText = this.GetReportElementText(chartElement.UniqueName);
                                        string chartTypeText = ChartTypeConverter.GetLocalizedChartType(chartElement.Chart.ChartType);
                                        string msg = string.Format("��������� \"{0} ({1})\" ������������ � �������� ���� � ����������� ���������. " +
                                           "�� ������������ ������ �� �������?",
                                           elementText, chartTypeText);
                                        if (MessageBox.Show(msg, "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            // ������� ��� ����, ���� ������� ��������
                                            RemoveCompositeChildKey(elementUN);
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                }

                                fieldListEditor.CompositeChartEditor.RefreshEditor();
                            }
                        pane.Control.Dispose();
                        dockPanelControl.udManager.ControlPanes.Remove(pane);
                        //������ �� ������� ������� ������� ��������
                        this.UndoRedoManager.RemoveElementEvents(elementUN);

                        if (dockPanelControl.udManager.ControlPanes.Count > 0)
                            this.FindActiveElement(dockPanelControl.udManager.ControlPanes[0], true);
                        else
                            this.FindActiveElement(null, true);
                        this.Saved = false;
                    }
                }
            }
        }

        /// <summary>
        /// ������ �� �������� �������� ������
        /// </summary>
        /// <param name="reportElement"></param>
        /// <returns></returns>
        private bool DeletingElementRequest(Control reportElement)
        {
            if (reportElement == null)
                return false;
            CustomReportElement customElement = (CustomReportElement)reportElement;
            return (MessageBox.Show(string.Format("������� ������� ������ \"{0}\"?",
                customElement.Title), "�������", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes);
        }

        /// <summary>
        /// ������������� ���������� ��������� � ��� � TaskBar-�
        /// </summary>
        private void SetApplicationInformation()
        {
            this.ToolbarsManager.RibbonCaption = this.ApplicationCaption;
            this.Text = this.ApplicationCaption;
        }

        /// <summary>
        /// ��������� �� ��������������� � ����������� ����, ����������� ��� ����� � ������
        /// ���� ���������� ��������� ���� ��������� ����� ���� ���������� �����������.
        /// </summary>
        /// <param name="cubeName"></param>
        /// <returns></returns>
        public bool IsNeedReconnectAdomdConnection(string cubeName)
        {
            if (cubeName == string.Empty || this.AdomdConn == null || this.LastAdomdConnectionDate == null)
                return false;

            this.AdomdConn.RefreshMetadata();
            CubeDef cubeDef = this.AdomdConn.Cubes.Find(cubeName);
            if (cubeDef == null)
                return true;

            return (cubeDef.LastProcessed > this.LastAdomdConnectionDate) ||
                (cubeDef.LastProcessed < cubeDef.LastUpdated);
        }

        /// <summary>
        /// ���������������� � �����
        /// </summary>
        public void ResetAdomdConnection()
        {
            this.Operation.StartOperation();
            this.Operation.Text = "����������� � �������...";
            try
            {
                if (this.AdomdConn != null)
                {
                    this.AdomdConn.Close(false);
                    this.AdomdConn = null;
                }
                this.AdomdConn = new AdomdConnection(Consts.TmpConnStr);

                this.AdomdConn.Open();
                Krista.FM.Client.MDXExpert.Data.PivotData.AdomdConn = this.AdomdConn;
                this.LastAdomdConnectionDate = DateTime.Now;
            }
            finally
            {
                this.Operation.StopOperation();
            }
        }

        #region ����������� �������

        private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            this.propertyGrid.Refresh();
            //�������������� �������� � ������ ������������ �� ���������� � PropertyGrid
            this.ToolbarsManager.RefreshTabs(this.ActiveElement);

            this.Saved = false;
        }

        private void dockPanelControl_PaneDeactivated()
        {
            //������� ���������� (�� RpoertyGrid � �.�.)����� ������ � ������, ���� ���� �������������� 
            //��������� ������, ����� ��� ������� �������� ������
            if (dockPanelControl.udManager.ControlPanes.Count == 1)
            {
                FindActiveElement(null, true);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.CloseReport())
            {
                // ���� ��� ������ ������ ��� ��������� �������� - ����������
                if (this.Operation != null)
                {
                    this.Operation.ReleaseThread();
                    this.Operation = null;

                }
                //��������� ��������� ���������� ���������� � �������
                this.SaveRegSettins();
            }
            else
            {
                //������������ ����� ���������� ��������� 
                e.Cancel = true;
            }
            PivotData.AdomdConn = null;
        }

        private void ultraDockManager_AfterPaneButtonClick(object sender, PaneButtonEventArgs e)
        {
            if (e.Button == PaneButton.Close)
            {
                if (e.Pane == ultraDockManager.ControlPanes[0])
                {
                    ToolbarsManager.structureTool.Checked = false;
                }

                if (e.Pane == ultraDockManager.ControlPanes[1])
                {
                    ToolbarsManager.propertyTool.Checked = false;
                }
            }
        }

        #endregion

        #region ��������

        /// <summary>
        /// ���������� ��������� ������� ������
        /// </summary>
        public static MainForm Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// ��� ��������� ������, �������������� � statusBar-�
        /// </summary>
        public string HostName
        {
            get
            {
                return statusBar.Panels["HostName"].Text.Replace("�������� ������: ", string.Empty);
            }
            set
            {
                UltraStatusPanel hostName = statusBar.Panels["HostName"];

                if (value.Contains(Consts.disconnection))
                    hostName.Appearance.ForeColor = Color.Red;
                else
                    hostName.Appearance.ResetForeColor();
                hostName.Text = "�������� ������: " + value;
                hostName.Visible = true;
            }
        }

        /// <summary>
        /// ����� ���������� �������, �������������� statusBar-�
        /// </summary>
        public string QueryTime
        {
            get { return statusBar.Panels["QueryTime"].Text; }
            set
            {
                UltraStatusPanel queryTime = statusBar.Panels["QueryTime"];
                queryTime.Text = string.Format("{0}: {1}{2}", "����� ���������� �������",
                    value, "c");
                queryTime.Visible = true;
            }
        }

        /// <summary>
        /// ��������������� ����� �� ���������� ������
        /// </summary>
        public ToolbarsManage ToolbarsManager
        {
            get { return _toolbarsManager; }
            set { _toolbarsManager = value; }
        }

        /// <summary>
        /// ����� ������������
        /// </summary>
        public UltraToolbarsManager UltraToolbarsManager
        {
            get { return this.ultraToolbarsManager; }
        }

        /// <summary>
        /// ����������
        /// </summary>
        public Client.Common.Forms.Operation Operation
        {
            get { return _operation; }
            set { _operation = value; }
        }

        /// <summary>
        /// �������� ������� �������� ������
        /// </summary>
        public PropertyGrid PropertyGrid
        {
            get { return this.propertyGrid; }
        }

        /// <summary>
        /// �������� ������� ������
        /// </summary>
        public CustomReportElement ActiveElement
        {
            get { return activeElement; }
            set { this.SetActiveElement(value); }
        }

        /// <summary>
        /// �������� ������� ������ ���������� ��������
        /// </summary>
        public ChartReportElement ActiveChartElement
        {
            get
            {
                if (activeElement is ChartReportElement)
                    return (ChartReportElement)activeElement;
                else
                    return null;
            }
            set { this.SetActiveElement(value); }
        }

        /// <summary>
        /// �������� ������� ������ ���������� �������
        /// </summary>
        public TableReportElement ActiveTableElement
        {
            get
            {
                if (activeElement is TableReportElement)
                    return (TableReportElement)activeElement;
                else
                    return null;
            }
            set { this.SetActiveElement(value); }
        }

        /// <summary>
        /// �������� ������� ������ ���������� �����
        /// </summary>
        public MapReportElement ActiveMapElement
        {
            get
            {
                if (this.activeElement is MapReportElement)
                    return (MapReportElement)this.activeElement;
                else
                    return null;
            }
            set { this.SetActiveElement(value); }
        }

        /// <summary>
        /// �������� ������� ������ ���������� ���������
        /// </summary>
        public GaugeReportElement ActiveGaugeElement
        {
            get
            {
                if (activeElement is GaugeReportElement)
                    return (GaugeReportElement)activeElement;
                else
                    return null;
            }
            set { this.SetActiveElement(value); }
        }


        /// <summary>
        /// ���������� �� �������� ������� ������
        /// </summary>
        public bool IsExistsActiveElement
        {
            get { return (this.ActiveElement != null); }
        }

        /// <summary>
        /// ���������� �� �������� ������� ������ ���������� �������
        /// </summary>
        public bool IsExistsActiveTable
        {
            get { return (this.ActiveTableElement != null); }
        }

        /// <summary>
        /// ���������� �� �������� ������� ������ ���������� �����
        /// </summary>
        public bool IsExistsActiveMap
        {
            get { return (this.ActiveMapElement != null); }
        }

        /// <summary>
        /// ���������� �� �������� ������� ������ ���������� ��������
        /// </summary>
        public bool IsExistsActiveChart
        {
            get { return (this.ActiveChartElement != null); }
        }

        /// <summary>
        /// ��� �������� ������� ���������� (���� isl)
        /// ��������� ������ ���� � �������� �������
        /// </summary>
        public string StyleSheet
        {
            get { return _styleSheet; }
            set
            {
                if (File.Exists(value))
                {
                    //������������� ����
                    _styleSheet = value;

                    //������ ����� ����������
                    Infragistics.Win.AppStyling.StyleManager.Load(value);
                    AdjustWindowViewByStylesheet(value);

                    //��������� ������� ������ ������������ �������� �����������, � ����������� 
                    //�� ��������� �������� �����, ��� ��� ������ �� ��������
                    this.ToolbarsManager.Load(value);

                    //���������� � �������
                    Utils regUtils = new Utils(GetType(), true);
                    regUtils.SetKeyValue(Consts.styleSheetPathRegKey, value);

                    dockPanelControl.StyleSheet = _styleSheet;
                }
                else
                {
#warning ������ ���������, ��� ���� ����� �� ������ (��� ����� �������� �����, ����� ������� ���������)
                }
            }
        }

        /// <summary>
        /// ������� ������������� ������
        /// </summary>
        public bool Saved
        {
            get { return _saved; }
            set
            {
                _saved = value;
                if (!_saved)
                {
                    // ��������� ����������� ���������, ���� ���� �������� ��������
                    if (ActiveChartElement != null)
                    {
                        List<ChartReportElement> chartElements = GetCompositeParentCharts(activeElement.UniqueName);
                        foreach (ChartReportElement element in chartElements)
                        {
                            element.RefreshCompositeChart();
                        }
                    }
                    this.UndoRedoManager.AddEvent(this.ActiveElement, UndoRedoEventType.AppearanceChange);
                }
            }
        }

        /// <summary>
        /// ���� � ����� ������
        /// </summary>
        public string ReportFileName
        {
            get { return _reportFileName; }
            set { _reportFileName = value; }
        }

        /// <summary>
        /// ��������� ������ ��������� ��������
        /// </summary>
        public bool StructureToolVisible
        {
            get { return ultraDockManager.ControlPanes[0].Closed; }
            set
            {
                if (ultraDockManager.ControlPanes[0].Closed != value)
                    ultraDockManager.ControlPanes[0].Closed = value;
            }
        }

        /// <summary>
        /// ��������� ������ ������� ��������
        /// </summary>
        public bool PropertyToolVisible
        {
            get { return ultraDockManager.ControlPanes[1].Closed; }
            set
            {
                if (ultraDockManager.ControlPanes[1].Closed != value)
                    ultraDockManager.ControlPanes[1].Closed = value;
            }
        }

        /// <summary>
        /// ��� ������ � ���������� ������
        /// </summary>
        public DockableControlPanesCollection ControlPanes
        {
            get { return dockPanelControl.udManager.ControlPanes; }
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        internal ReportExporter Exporter
        {
            get { return _exporter; }
            set { _exporter = value; }
        }

        /// <summary>
        /// ����� �������� � Excel
        /// </summary>
        public ExcelExportForm ExportForm
        {
            get { return _exportForm; }
            set { _exportForm = value; }
        }

        /// <summary>
        /// ����� "� ���������..."
        /// </summary>
        public About AboutForm
        {
            get { return _aboutForm; }
            set { _aboutForm = value; }
        }

        public UltraPrintPreviewDialog PrintPreviewDialog
        {
            get { return ultraPrintPreviewDialog; }
        }

        /// <summary>
        /// ��������� ���������� (��� ������ � ��� ����������)
        /// </summary>
        public string ApplicationCaption
        {
            get
            {
                return string.Format("{0} - {1}", CommonUtils.SeparateFileName(this.ReportFileName == string.Empty ? Consts.newReportName :
                    this.ReportFileName, false), Consts.applicationName);
            }
        }

        /// <summary>
        /// ������� �� AdomdCommand, ����� ����� ������ ����������� ��������
        /// </summary>
        public MDXCommand MdxCommand
        {
            get { return _mdxCommand; }
            set { _mdxCommand = value; }
        }

        /// <summary>
        /// �������� ��������� ������������ ������
        /// </summary>
        public MRUManager MRUManager
        {
            get { return _mruManager; }
            set { _mruManager = value; }
        }

        /// <summary>
        /// �������� ������� ������
        /// </summary>
        public UndoRedoManager UndoRedoManager
        {
            get { return _undoRedoManager; }
            set { _undoRedoManager = value; }
        }

        public AdomdConnection AdomdConn
        {
            get { return _adomdConn; }
            set
            {
                if (_adomdConn != null)
                {
                    _adomdConn.Close();
                    _adomdConn = null;
                }
                _adomdConn = value;
            }
        }

        /// <summary>
        /// �� ����, ���� ������� ����� �����, ���������� �� ������ ���������� ������������ 
        /// ������� ���������.
        /// </summary>
        public bool IsSilentMode
        {
            get { return _isSilentMode; }
            set { _isSilentMode = value; }
        }

        /// <summary>
        /// ���������� �� �� ������ ������ ���������� � ��������
        /// </summary>
        public bool IsExistsConnection
        {
            get
            {
                if ((this.AdomdConn == null) || (this.AdomdConn.State != System.Data.ConnectionState.Open))
                    return false;
                try
                {
                    //���� ���� �������� ���������� Adomd ������ �� ����������, � ����� ������ 
                    //�� ���������� ����. ��� ��������� � �������� Database, Adomd �������� 
                    //������� ������ �� ������, ���� ���������� ���, ����� �� ��� � �������.
                    string dataBase = this.AdomdConn.Database;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// �������� �� ���������� ��������� �����������
        /// </summary>
        public bool IsCompositeChart
        {
            get { return _isCompositeChart; }
            set { _isCompositeChart = value; }
        }

        public FieldList.FieldListEditor FieldListEditor
        {
            get { return this.fieldListEditor; }
        }

        public DateTime LastAdomdConnectionDate
        {
            get { return _lastAdomdConnectionDate; }
            set { _lastAdomdConnectionDate = value; }
        }

        public static Color BoderColor
        {
            get { return _boderColor; }
        }

        public static Color PanelColor
        {
            get { return _panelColor; }
        }

        public static Color DarkPanelColor
        {
            get { return _darkPanelColor; }
        }

        /// <summary>
        /// ����������/�������� ����� �� ���������
        /// </summary>
        public static bool IsHideTotalsByDefault
        {
            get { return _isHideTotalsByDefault; }
            set { _isHideTotalsByDefault = value; }
        }

        #endregion

        #region ������ � ����������� ����������

        /// <summary>
        /// ����� ���������� ���� ����������� �������� ������
        /// </summary>
        public void ResetCompositeCharts()
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element is ChartReportElement && ((ChartReportElement)element).IsCompositeChart)
                    {
                        ((ChartReportElement)element).ResetCompositeChart();
                    }
                }
            }
        }

        /// <summary>
        /// ���������� ���������� ���� ����������� �������� ������
        /// </summary>
        public void RefreshCompositeCharts()
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element is ChartReportElement && ((ChartReportElement)element).IsCompositeChart)
                    {
                        ((ChartReportElement)element).RefreshCompositeChart();
                    }
                }
            }
        }

        /// <summary>
        /// ����� �������� �� ����������� �����
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>������� ������</returns>
        public CustomReportElement FindReportElement(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element.UniqueName == key)
                    {
                        return element;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// ����� ��������� �� ����������� �����
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>������� ���������</returns>
        public ChartReportElement FindChartReportElement(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element is ChartReportElement && element.UniqueName == key)
                    {
                        return (ChartReportElement)element;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// ����� �������������� ���������� �� ����������� �����
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>������� ���������</returns>
        public MultipleGaugeReportElement FindMultiGaugeReportElement(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element is MultipleGaugeReportElement && element.UniqueName == key)
                    {
                        return (MultipleGaugeReportElement)element;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// ����� ���������� �� ����������� �����
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>������� ����������</returns>
        public GaugeReportElement FindGaugeReportElement(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element is GaugeReportElement && element.UniqueName == key)
                    {
                        return (GaugeReportElement)element;
                    }
                }
            }

            return null;
        }




        /// <summary>
        /// ����� ����� �� ����������� �����
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>������� �����</returns>
        public MapReportElement FindMapReportElement(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element is MapReportElement && element.UniqueName == key)
                    {
                        return (MapReportElement)element;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// ����� ������� �� ����������� �����
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>�������</returns>
        public TableReportElement FindTableReportElement(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control != null)
                {
                    CustomReportElement element = (CustomReportElement)pane.Control;
                    if (element is TableReportElement && element.UniqueName == key)
                    {
                        return (TableReportElement)element;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// �������� ������ ������ ��������� �������� ��������
        /// </summary>
        /// <param name="key">���� ����������� ���������</param>
        /// <returns>������ ������ ��������</returns>
        public List<string> GetAvialableCompositeCharts(string key)
        {
            List<string> availableItems = new List<string>();

            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control is ChartReportElement)
                {
                    ChartReportElement element = (ChartReportElement)pane.Control;
                    if (CompositeChartUtils.IsCompositeCompatibleType(element.Chart.ChartType))
                    {
                        ChartReportElement parentElement = FindChartReportElement(key);
                        if (parentElement != null)
                        {
                            string childKey = element.UniqueName;
                            if (!parentElement.ContainsChildChart(childKey) && !availableItems.Contains(childKey))
                            {
                                availableItems.Add(childKey);
                            }
                        }
                    }
                }
            }

            return availableItems;
        }

        /// <summary>
        /// �������� ������ ������ ���������-������
        /// </summary>
        /// <returns>������ ������ ��������</returns>
        public List<string> GetAvialableTables()
        {
            List<string> availableItems = new List<string>();

            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control is TableReportElement)
                {
                    TableReportElement element = (TableReportElement)pane.Control;
                    string childKey = element.UniqueName;
                    availableItems.Add(childKey);
                }
            }

            return availableItems;
        }


        /// <summary>
        /// ������������� ������ ����������� �������� ��� �������� �������� ���������
        /// </summary>
        /// <param name="key">���� �������� ���������</param>
        public void RemoveCompositeChildKey(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control is ChartReportElement)
                {
                    ChartReportElement element = (ChartReportElement)pane.Control;
                    if (element.Chart.ChartType == ChartType.Composite)
                    {
                        element.RemoveChildChart(key);
                    }
                }
            }
        }

        /// <summary>
        /// ������������� ������ �������� �������� ��� �������� ����������� ���������
        /// </summary>
        /// <param name="key">���� ����������� ���������</param>
        public void RemoveCompositeParentKey(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control is ChartReportElement)
                {
                    ChartReportElement element = (ChartReportElement)pane.Control;
                    if (element.Chart.ChartType != ChartType.Composite)
                    {
                        element.RemoveParentChart(key);
                    }
                }
            }
        }

        /// <summary>
        /// �������� �� ��������� �������� ��� �����������
        /// </summary>
        /// <param name="key">���� �������� ���������</param>
        /// <returns>True, ���� ��������</returns>
        public bool IsCompositeChildChart(string key)
        {
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control is ChartReportElement)
                {
                    ChartReportElement element = (ChartReportElement)pane.Control;
                    if (element.IsCompositeChart && element.ContainsChildChart(key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// �������� ��������� �������� ���������
        /// </summary>
        /// <param name="key">���� �������� ���������</param>
        /// <returns>������ ���������, ���������� ����������� ���������</returns>
        public List<ChartReportElement> GetCompositeParentCharts(string key)
        {
            List<ChartReportElement> elements = new List<ChartReportElement>();

            int j = 0;
            foreach (DockableControlPane pane in dockPanelControl.udManager.ControlPanes)
            {
                if (pane.Control is ChartReportElement)
                {
                    ChartReportElement element = (ChartReportElement)pane.Control;
                    if (element.IsCompositeChart && element.ContainsChildChart(key))
                    {
                        elements.Add(element);
                    }
                }
            }

            return elements;
        }

        #endregion

        /// <summary>
        /// ��������� ������� ������ ��� ������� �����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //���������� ��������� ��� ��������� �������� ������
            if (e.KeyCode.Equals(Keys.F5))
            {
                CustomReportElement element = this.ActiveElement;
                if (element != null)
                {
                    if (element.PivotData != null)
                    {
                        element.PivotData.DoForceDataChanged();
                    }
                }
                return;
            }

            if (e.Control && e.Shift && e.KeyCode.Equals(Keys.Z))
            {
                this.UndoRedoManager.Redo();
                return;
            }

            if (e.Control && e.KeyCode.Equals(Keys.Z))
            {
                this.UndoRedoManager.Undo();
                return;
            }


        }


    }
}
