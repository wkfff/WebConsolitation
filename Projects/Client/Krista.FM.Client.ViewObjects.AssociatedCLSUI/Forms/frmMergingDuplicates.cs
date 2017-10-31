using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using HierarchyType=Krista.FM.ServerLibrary.HierarchyType;
using Resources=Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmMergingDuplicates : Form
    {
        public frmMergingDuplicates()
        {
            InitializeComponent();
        }
      
        private DataTable dtDuplicates;
        private Dictionary<string, string> fieldNames = new Dictionary<string, string>(); 
        private int mainDuplicateID;
        private BaseClsUI clsUI;

        /// <summary>
        /// ���������� ����� ������ ����������.
        /// </summary>
        /// <param name="mainID">ID �������� ������.</param>
        /// <param name="clsUI">������ ��������� ��������������.</param>
        public static void ChooseDuplicatesFormShow(int mainID, BaseClsUI clsUI)
        {
            frmMergingDuplicates frmChooseDuplicates = new frmMergingDuplicates();
            frmChooseDuplicates.mainDuplicateID = mainID;
            frmChooseDuplicates.clsUI = clsUI;
            frmChooseDuplicates.MakeFieldsList();
            frmChooseDuplicates.MainRecordGridSetup();
            frmChooseDuplicates.Text = string.Format(
               "{0}: ����������� ����������.", clsUI.ActiveDataObj.FullCaption);
            frmChooseDuplicates.SetMainRecord();
            // ����������� ���� ����������.
            frmChooseDuplicates.MergeDuplicatesGridSetup();
            // ������ ������� ��� ������ ����������.
            frmChooseDuplicates.dtDuplicates = frmChooseDuplicates.MakeDuplicateTable();
            frmChooseDuplicates.ugeMergingDuplicates.DataSource = frmChooseDuplicates.dtDuplicates;
            if (((IClassifier)clsUI.ActiveDataObj).IsDivided)
            {
                frmChooseDuplicates.GroupByColumn();
            }
            frmChooseDuplicates.Show((Form)frmChooseDuplicates.clsUI.Workplace);
            frmChooseDuplicates.SetDuplicatesCount(0);
        }

        private void GroupByColumn()
        {
            ugeMergingDuplicates.ugData.DisplayLayout.Bands[0].SortedColumns.Add("SourceName", false, true);
            ugeMergingDuplicates.ugData.DisplayLayout.Rows.ExpandAll(false);
        }

        private void SetDuplicatesCount(int count)
        {
            lbDuplicatesCount.Text = string.Format("����� ���������� ���������� = {0}", count);
        }

        private DataTable MakeDuplicateTable()
        {
            DataTable dt = new DataTable();
            DataColumn col = new DataColumn("DuplicateID");
            dt.Columns.Add(col);
            foreach (KeyValuePair<string, string> field in fieldNames)
            {
                col = new DataColumn(field.Key);
                dt.Columns.Add(col);
            }
            if (((IClassifier)clsUI.ActiveDataObj).IsDivided)
            {
                col = new DataColumn("SourceName");
                dt.Columns.Add(col);

                col = new DataColumn("SourceID");
                dt.Columns.Add(col);
            }
            return dt;
        }

        private void MakeFieldsList()
        {
            foreach (IDataAttribute attribute in clsUI.ActiveDataObj.Attributes.Values)
            {
                if (attribute.LookupType != LookupAttributeTypes.None)
                {
                    fieldNames.Add(attribute.Name, attribute.Caption);
                }
            }
        }

        private void SetMainRecord()
        {
            DataTable dtMainRecord = MakeDuplicateTable();
            UltraGridRow row = clsUI.UltraGridExComponent.ugData.Selected.Rows[0];
            int duplicateID = Convert.ToInt32(row.Cells["ID"].Value);
            addRecord(duplicateID, row, dtMainRecord);
            ugeMainRecord.DataSource = dtMainRecord;
        }

        #region ��������� ������

        /// <summary>
        /// ����������� ��������� ����� �������� ������.
        /// </summary>
        private void MainRecordGridSetup()
        {
            ugeMainRecord.IsReadOnly = true;
            ugeMainRecord.ugData.BeforeCellActivate += new CancelableCellEventHandler(ugData_BeforeCellActivate);
            ugeMainRecord.ugData.BeforeSortChange += new BeforeSortChangeEventHandler(ugData_BeforeSortChange);
            ugeMainRecord.ugData.BeforeRowFilterDropDown += new BeforeRowFilterDropDownEventHandler(ugData_BeforeRowFilterDropDown);
            ugeMainRecord.utmMain.Visible = false;
            ugeMainRecord.ugData.DisplayLayout.GroupByBox.Hidden = true;
            InfragisticComponentsCustomize.CustomizeUltraGridParams(ugeMainRecord.ugData);
            ugeMainRecord.OnGridInitializeLayout += new GridInitializeLayout(ugeMainRecord_OnGridInitializeLayout);
        }

        void ugData_BeforeRowFilterDropDown(object sender, BeforeRowFilterDropDownEventArgs e)
        {
            e.Cancel = true;
        }

        void ugData_BeforeSortChange(object sender, BeforeSortChangeEventArgs e)
        {
            e.Cancel = true;
        }

        void ugData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// ����������� ��������� ����� ����������.
        /// </summary>
        private void MergeDuplicatesGridSetup()
        {
            ugeMergingDuplicates.IsReadOnly = true;
            ugeMergingDuplicates.AllowClearTable = true;
            ugeMergingDuplicates.AllowDeleteRows = false;
            ugeMergingDuplicates.AllowImportFromXML = false;
            ugeMergingDuplicates.ExportImportToolbarVisible = false;
            ugeMergingDuplicates.SaveMenuVisible = false;
            ugeMergingDuplicates.LoadMenuVisible = false;
            ugeMergingDuplicates.AllowAddNewRecords = false;
            ugeMergingDuplicates._utmMain.Tools["Refresh"].SharedProps.Visible = false;
            ugeMergingDuplicates._utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;
            ugeMergingDuplicates._utmMain.Tools["SaveChange"].SharedProps.Visible = false;
            ugeMergingDuplicates._utmMain.Tools["CancelChange"].SharedProps.Visible = false;
            ugeMergingDuplicates._utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
            ugeMergingDuplicates._utmMain.Tools["ColumnsVisible"].SharedProps.Visible = false;
            ugeMergingDuplicates._utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = true;
            ugeMergingDuplicates._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;

            AddDuplicateTools();

            ugeMergingDuplicates.OnGridInitializeLayout += new GridInitializeLayout(ugeMergingDuplicates_OnGridInitializeLayout);
            ugeMergingDuplicates.ToolClick += new ToolBarToolsClick(ugeMergingDuplicates_ToolClick);

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ugeMergingDuplicates.ugData);
        }

        /// <summary>
        /// ���������� ������ �� ������.
        /// </summary>
        private void AddDuplicateTools()
        {
            UltraToolbar tb = ugeMergingDuplicates.utmMain.Toolbars["utbColumns"];

            if (!ugeMergingDuplicates.utmMain.Tools.Exists("btnAddDuplicate"))
            {
                ButtonTool btnAddDuplicate = new ButtonTool("btnAddDuplicate");
                btnAddDuplicate.SharedProps.ToolTipText = "�������� ��������";
                btnAddDuplicate.SharedProps.AppearancesSmall.Appearance.Image =
                   Resources.AddDuplicate;
                ugeMergingDuplicates.utmMain.Tools.Add(btnAddDuplicate);
                tb.Tools.AddTool("btnAddDuplicate");
            }

            if (!ugeMergingDuplicates.utmMain.Tools.Exists("btnRemoveDuplicate"))
            {
                ButtonTool btnRemoveDuplicate = new ButtonTool("btnRemoveDuplicate");
                btnRemoveDuplicate.SharedProps.ToolTipText = "������� ��������";
                btnRemoveDuplicate.SharedProps.AppearancesSmall.Appearance.Image =
                    Resources.RemoveDuplicate;
                ugeMergingDuplicates.utmMain.Tools.Add(btnRemoveDuplicate);
                tb.Tools.AddTool("btnRemoveDuplicate");
            }
        }

        void ugeMergingDuplicates_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["DuplicateID"];
            clmn.Header.VisiblePosition = 0;
            clmn.Header.Caption = "ID ���������";
            clmn.Width = 100;

            InitializeOtherColumns(band);
        }

        void ugeMainRecord_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["DuplicateID"];
            clmn.Header.VisiblePosition = 0;
            clmn.Header.Caption = "ID �������� ������";
            clmn.Width = 120;

            InitializeOtherColumns(band);
        }

        private void InitializeOtherColumns(UltraGridBand band)
        {
            UltraGridColumn clmn;
            if (((IClassifier)clsUI.ActiveDataObj).IsDivided)
            {
                clmn = band.Columns["SourceID"];
                clmn.Header.VisiblePosition = 1;
                clmn.Header.Caption = "ID ��������� ������";
                clmn.Width = 120;

                clmn = band.Columns["SourceName"];
                clmn.Header.VisiblePosition = 2;
                clmn.Header.Caption = "������������ ��������� ������";
                clmn.Width = 150;
            }

            int position = 2;
            foreach (KeyValuePair<string, string> field in fieldNames)
            {
                clmn = band.Columns[field.Key];
                clmn.Header.VisiblePosition = ++position;
                clmn.Header.Caption = field.Value;
                switch(field.Key)
                {
                    case "Code":
                        clmn.Width = 70;
                        break;
                    case "Name":
                        clmn.Width = 250;
                        break;
                    default:
                        clmn.Width = 150;
                        break;
                }
            }
        }

        #endregion

        #region ���������� � ��������

        void ugeMergingDuplicates_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool == null) return;
            ButtonTool btn = null;
            if (e.Tool is ButtonTool)
                btn = (ButtonTool)e.Tool;

            if (btn != null)
                switch (btn.Key)
                {
                    case "btnAddDuplicate":
                        {
                            AddDuplicate();
                            break;
                        }
                    case "btnRemoveDuplicate":
                        {
                            RemoveDuplicate();
                            break;
                        }
                }
        }

        /// <summary>
        /// ������� �������� �� ������� ����������.
        /// </summary>
        private void RemoveDuplicate()
        {
            // ������� ������ �����.
            List<DataRow> forRemoveDuplicates = new List<DataRow>();
            foreach (UltraGridRow ugRow in ugeMergingDuplicates.ugData.Selected.Rows)
            {
                foreach (DataRow dtRow in dtDuplicates.Rows)
                {
                    if (ugRow.IsGroupByRow)
                    {
                        continue;
                    }
                    if (Convert.ToInt32(dtRow["DuplicateID"]) ==
                            Convert.ToInt32(ugRow.Cells["DuplicateID"].Value))
                    {
                        // ���������� ���������.
                        forRemoveDuplicates.Add(dtRow);
                        continue;
                    }
                }
            }
            foreach (DataRow dtRow in forRemoveDuplicates)
            {
                dtDuplicates.Rows.Remove(dtRow);
            }
            SetDuplicatesCount(dtDuplicates.Rows.Count);
        }

        /// <summary>
        /// ��������� �������� � ������� ����������,
        /// ���� ������ � ����� ID ��� ��� � ��� �� �������� ������.
        /// </summary>
        private void AddDuplicate()
        {
            // �� ���������� ������� � �������� �����
            foreach (UltraGridRow row in clsUI.UltraGridExComponent.ugData.Selected.Rows)
            {
                int duplicateID = Convert.ToInt32(row.Cells["ID"].Value);
                if (MayAdd(duplicateID, row))
                {
                    // ��������� �������� � �������
                    addRecord(duplicateID, row, dtDuplicates);
                }
            }
            if (((IClassifier)clsUI.ActiveDataObj).IsDivided)
            {
                GroupByColumn();
            }
            SetDuplicatesCount(dtDuplicates.Rows.Count);
        }

        private void addRecord(int duplicateID, UltraGridRow row, DataTable dt)
        {
            DataRow drRow = dt.NewRow();
            dt.Rows.Add(drRow);
            drRow["DuplicateID"] = duplicateID;
            foreach (KeyValuePair<string, string> field in fieldNames)
            {
                drRow[field.Key] = row.Cells[field.Key].Value;
            }
            if (((IClassifier) clsUI.ActiveDataObj).IsDivided)
            {
                drRow["SourceID"] = clsUI.CurrentDataSourceID;
                drRow["SourceName"] =
                    clsUI.Workplace.ActiveScheme.DataSourceManager.GetDataSourceName(
                        clsUI.CurrentDataSourceID);
            }
        }

        private bool MayAdd(int duplicateID, UltraGridRow row)
        {
            // ���� ��������� �������� ������
            if (duplicateID == mainDuplicateID)
                return false;
            // ���� ��������� ��������������� �������
            if (((clsUI.ActiveDataObj is IVariantDataClassifier) &&
                      (Convert.ToBoolean(row.Cells["VariantCompleted"].Value))))
                return false;
                
            // ���� ������� �� ����������
            if ((((IClassifier)clsUI.ActiveDataObj).IsDivided))
            {
                DataTable dtSource = DataSourcesHelper.GetDataSourcesInfo(clsUI.CurrentDataSourceID, clsUI.Workplace.ActiveScheme);
                // � ������� �������� ������������
                if ((dtSource.Rows.Count > 0) && (Convert.ToBoolean(dtSource.Rows[0]["Locked"])))
                {
                    return false;
                }
            }
            // ������������� ������� ����������.
            foreach (DataRow duplicateRow in dtDuplicates.Rows)
            {
                // ���� � ���������� ��� ���� ����� ID
                if (Convert.ToInt32(duplicateRow["DuplicateID"]) == duplicateID)
                {
                    return false;
                }
            }
            return true;
        }

        # endregion

        #region �������

        private Mutex mutex = new Mutex();

        /// <summary>
        /// ���������� ������� ����������.
        /// </summary>
        private void MergeDuplicates()
        {
            mutex.WaitOne();
            List<int> duplicatesID = new List<int>();
            // ��������� ������ ����������.
            foreach (DataRow row in dtDuplicates.Rows)
            {
                duplicatesID.Add(Convert.ToInt32(row["DuplicateID"]));
            }
            MergeDuplicatesInformer informer = new MergeDuplicatesInformer();
            MergeDuplicatesListener listener = new MergeDuplicatesListener(informer.SendMergeMessage);
            informer.OnSendMergeMessage += new MergeDuplicatesListener(informer_OnSendMergeMessage);
            try
            {
                clsUI.ActiveDataObj.MergingDuplicates(mainDuplicateID, duplicatesID, listener);
            }
            // ���������� ����� ������������ �����.
            catch(Exception ex)
            {
                FormException.ShowErrorForm(ex, ErrorFormButtons.WithoutTerminate);
            }
            SetButtonsEnable();
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// ���������� �� ��������� �������� � �������� �������.
        /// </summary>
        /// <param name="message"></param>
        void informer_OnSendMergeMessage(string message)
        {
            SetInformText(message);
        }

        private delegate void SetInformTextCallback(string text);
        private delegate void MoveProgressbarCallback();
        private delegate void SetButtonsEnableCallback();

        private void SetButtonsEnable()
        {
            if (InvokeRequired)
            {
                SetButtonsEnableCallback d = new SetButtonsEnableCallback(SetButtonsEnable);
                Invoke(d);
            }
            else
            {
                btnOK.Enabled = true;
                btnSave.Enabled = true;
            }
        }

        /// <summary>
        /// ���������� ��������� � ������ ������.
        /// </summary>
        /// <param name="message">���������.</param>
        private void SetInformText(string message)
        {
            if (tbResults.InvokeRequired)
            {
                SetInformTextCallback d = new SetInformTextCallback(SetInformText);
                Invoke(d, new object[] {message});
            }
            else
            {
                tbResults.AppendText(message);
                if (!message.Contains("�������� ��������� ID ="))
                {
                    MoveProgressbar();
                }
            }
        }

        /// <summary>
        /// ������� �����������.
        /// </summary>
        private void MoveProgressbar()
        {
            if (pbMergingProgress.InvokeRequired)
            {
                MoveProgressbarCallback d = new MoveProgressbarCallback(MoveProgressbar);
                Invoke(d);
            }
            else
            {
                if (pbMergingProgress.Value < pbMergingProgress.Maximum)
                    pbMergingProgress.Value++;
            }
        }

        private void btnCancelMerging_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnStartMerging_Click(object sender, EventArgs e)
        {
            if (dtDuplicates.Rows.Count == 0)
            {
                return;
            }
            clsUI.Workplace.OperationObj.Text = "������� ����������";
            clsUI.Workplace.OperationObj.StartOperation();
            try
            {
                // ���������� ������ �������.
                utcMerging.SelectedTab = utcMerging.Tabs[1];

                // ������� �������� ������������.
                // ����� ���������� ��������� ��������.
                int progressBarMaximum = clsUI.ActiveDataObj.Associated.Values.Count;
                // ���� ������������� �������������
                if (((IClassifier)clsUI.ActiveDataObj).Levels.HierarchyType == HierarchyType.ParentChild)
                {
                    // ������� � ������� ����������� ������.
                    progressBarMaximum++;
                }
                // � ������ ��������� �������������� ��� ���������.
                progressBarMaximum *= dtDuplicates.Rows.Count;
                // ��� ������� ��� �������� ���������� ����������.
                progressBarMaximum += dtDuplicates.Rows.Count;
                pbMergingProgress.Maximum = progressBarMaximum;
                // ���������� �������.
                Thread merg = new Thread(new ThreadStart(MergeDuplicates));
                merg.Start();
            }
            finally
            {
                clsUI.Workplace.OperationObj.StopOperation();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            clsUI.Refresh();
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveReport();
        }

        /// <summary>
        /// ��������� ����� �� ����������� � ����.
        /// </summary>
        private void SaveReport()
        {
            saveFileDialog.FileName = string.Format("{0}_����������� ���������", clsUI.ActiveDataObj.FullCaption);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream Report = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter ReportText = new StreamWriter(Report);
                ReportText.Write(tbResults.Text);
                ReportText.Close();
            }
        }
        #endregion
    }
}