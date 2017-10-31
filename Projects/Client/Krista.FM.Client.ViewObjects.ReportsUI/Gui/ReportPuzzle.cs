using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Reports;
using Krista.FM.Client.Reports.UFK.CommonObjects;
using Krista.FM.Client.Reports.UFK.CommonObjects.Structures;
using Krista.FM.Client.Reports.UFK.DocumentWriter;
using Krista.FM.Client.Reports.UFK.ExportImport;
using Krista.FM.Client.Reports.UFK.GuiComponent;
using Krista.FM.Client.Reports.UFK.ParamFunctions;
using Krista.FM.Client.Reports.UFK.ReportGenerators;
using Krista.FM.Common;
using Krista.FM.Common.FileUtils;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public partial class ReportPuzzle : BaseNavigationCtrl
    {
        // все параметры отчета
        ReportSnapshot report;
        // параметры воркплейса
        public IWin32Window WindowHandle { get; set; }
        public Operation OperationObj { get; set; }
        private IScheme scheme;
        public IScheme Scheme
        {
            get { return scheme; }
            set
            {
                ConvertorSchemeLink.scheme = value;
                scheme = value; 
            }
        }

        private int reportId = -1;
        private string reportPath = String.Empty;

        public ReportPuzzle()
        {
            InitializeComponent();

            miDeleteColumn.Image = Properties.Resources.PuzzleDelColumn;
            miDeleteParam.Image = Properties.Resources.PuzzleDelParam;
            miDeleteTable.Image = Properties.Resources.PuzzleDelTable;
            miAddTable.Image = Properties.Resources.PuzzleAddTable;
            miAddParam.Image = Properties.Resources.PuzzleAddParam;
            miAddColumn.Image = Properties.Resources.PuzzleAddColumn;
        }

        public void InitComboList()
        {
            report = new ReportSnapshot();
            GeneratorReport.OperationObj = OperationObj;
            GeneratorReport.WindowHandle = WindowHandle;

            PropertyGridHelper.ColGrid = pgColumns;
            PropertyGridHelper.PrmGrid = pgParams;
            PropertyGridHelper.GrpGrid = pgGroup;
            
            InitComboGroupCount();
            InitComboTableTypes();
        }

        private static string GetComboTableCaption(int tableNum)
        {
            return String.Format("Таблица {0}", tableNum);
        }

        private DataRow GetTemplateRow(object id)
        {
            var tblTemplates = scheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
            if (tblTemplates != null)
            {
                var rowsTemplates = tblTemplates.Select(String.Format("{0} = {1}", TemplateFields.ID, id));
                if (rowsTemplates.Length > 0)
                {
                    return rowsTemplates[0];
                }
            }

            return null;
        }

        private string GetDocumentFileName(int id)
        {
            var rowTemplate = GetTemplateRow(id);
            if (rowTemplate != null)
            {
                return String.Format("{0}{1}",
                                     rowTemplate[TemplateFields.Name],
                                     Path.GetExtension(Convert.ToString(rowTemplate[TemplateFields.DocumentFileName])));
            }

            return String.Empty;
        }

        public void LoadReportDescription()
        {
            var activeNode = reportTree.tReports.ActiveNode;
            if (activeNode == null) return;
            reportId = Convert.ToInt32(activeNode.Key);
            reportPath = TemplatesDocumentsHelper.GetFullFileName(reportId, GetDocumentFileName(reportId));
            var buffer = scheme.TemplatesService.Repository.GetDocument(reportId);
            if (buffer == null || buffer.Length == 0) return;
            var document = DocumentsHelper.DecompressFile(buffer);
            TemplatesDocumentsHelper.SaveDocument(reportPath, document);

            if (reportPath.Length > 0)
            {
                var reportLoader = new DescriptionLoader(reportPath);
                report = reportLoader.LoadReportStructure();

                if (report.TableList.Count == 0)
                {
                    report = PropertyGridHelper.AddTable(report);
                }

                PropertyGridHelper.RefreshGrid(pgParams, report.ReportParams, CreatePropertyType.cptCreateList);
                PropertyGridHelper.RefreshGrid(pgColumns, report.CurrentTable.prmColumns,
                                               CreatePropertyType.cptCreateList);
                PropertyGridHelper.RefreshGrid(pgGroup, report.CurrentTable.prmGrouping,
                                               CreatePropertyType.cptCreateList);
                PropertyGridHelper.SetGridColumnFilterHandler(pgColumns.Controls);

                excelCtl.LoadWorkbook(reportPath);

                cbCurrentTable.Items.Clear();

                if (report.TableList.Count > 0)
                {
                    var groupCount = report.TableList[0].cntGrouping;
                    cbGroup.SelectedIndex = 0;
                    cbGroup.SelectedIndex = groupCount;

                    for (var i = 0; i < report.TableList.Count; i++)
                    {
                        cbCurrentTable.Items.Add(GetComboTableCaption(i + 1));
                    }

                    cbCurrentTable.SelectedIndex = -1;
                    cbCurrentTable.SelectedIndex = 0;
                }
                else
                {
                    cbGroup.SelectedIndex = 0;
                    cbCurrentTable.SelectedIndex = -1;                    
                }
            }
        }

        public void SaveReportDescription()
        {
            excelCtl.SaveWorkbook();
            excelCtl.QuitExcel();
            ExcelWriter.WriteReportDescription(reportPath, DescriptionSaver.GetXmlDescription(report));
            var buffer = DocumentsHelper.CompressFile(FileHelper.ReadFileData(reportPath));
            scheme.TemplatesService.Repository.SetDocument(buffer, reportId);
        }

        private void InitTableComboValues()
        {
            cbCurrentTable.Items.Add(GetComboTableCaption(report.TableList.Count));
            cbCurrentTable.SelectedIndex = 0;
            cbGroup.SelectedIndex = 0;            
        }

        private void miAddParam_Click(object sender, EventArgs e)
        {
            if (PropertyGridHelper.AddProperty(report, GridPropertyType.propPrm, pgParams))
            {
                InitTableComboValues();
            }
        }

        private void miAddField_Click(object sender, EventArgs e)
        {
            if (PropertyGridHelper.AddProperty(report, GridPropertyType.propCol, pgColumns))
            {
                InitTableComboValues();
            }

            PropertyGridHelper.SetGridColumnFilterHandler(pgColumns.Controls);
        }

        private void miAddTable_Click(object sender, EventArgs e)
        {
            if (report.CurrentTable != null)
            {
                report.CurrentTable.prmColumns.FillParamValues();
            }

            report = PropertyGridHelper.AddTable(report);
            InitTableComboValues();
        }

        private void cbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCurrentTable.SelectedIndex < 0) return;
            report.CurrentTable = report.TableList[cbCurrentTable.SelectedIndex];
            PropertyGridHelper.RefreshGrid(pgColumns, report.CurrentTable.prmColumns, CreatePropertyType.cptCreateList);
            PropertyGridHelper.RefreshGrid(pgGroup, report.CurrentTable.prmGrouping, CreatePropertyType.cptCreateList);
        }

        private void InitComboTableTypes()
        {
            cbTableType.Items.Clear();
            
            foreach (var tableInfo in report.TableTypes)
            {
                cbTableType.Items.Add(tableInfo.caption);
            }

            if (cbTableType.Items.Count > 0)
            {
                cbTableType.SelectedIndex = 0;
            }
        }

        private void InitComboGroupCount()
        {
            cbGroup.Items.Add("Без группировок");

            for (int i = 0; i < ParamCreator.maxGroupingCount; i++)
            {
                cbGroup.Items.Add(String.Format("Число группировок: {0}", i + 1));
            }

            if (cbGroup.Items.Count > 0)
            {
                cbGroup.SelectedIndex = 0;
            }
        }

        private void pg_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PropertyGridHelper.PropertyValueChanged(s, e);
        }

        public void CreateReport()
        {
            GeneratorReport.Generation(report, reportPath);
        }

        private void miDeleteParam_Click(object sender, EventArgs e)
        {
            PropertyGridHelper.DeleteGridEntry(pgParams, report.ReportParams, ParamCreator.ParamName);
        }

        private void miDeleteColumn_Click(object sender, EventArgs e)
        {
            PropertyGridHelper.DeleteGridEntry(pgColumns, report.CurrentTable.prmColumns, ParamCreator.ColumnName);
        }

        private void cbGroup_SelectionChanged(object sender, EventArgs e)
        {
            if (report.CurrentTable == null) return;
            report.CurrentTable.cntGrouping = cbGroup.SelectedIndex;
            report.CurrentTable.prmGrouping[ParamCreator.GroupCountParam] = report.CurrentTable.cntGrouping;
            PropertyGridHelper.Renew(pgGroup);
        }

        private void cbTableType_SelectionChanged(object sender, EventArgs e)
        {
            int selectedIndex = cbCurrentTable.SelectedIndex;
            if (selectedIndex < 0) return;
            report.TableList[selectedIndex].objectKey = report.TableTypes[selectedIndex].objectKey;
            FieldPathfinder.SwitchCurrentTable(report.CurrentTable.objectKey);
        }

        private void tabContainer_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            foreach (ToolStripItem menuItem in menuActions.Items)
            {
                menuItem.Visible = false;
            }

            cbTableType.Visible = true;
            cbCurrentTable.Visible = true;

            if (e.Tab.Index == 0 || e.Tab.Index == 4)
            {
                cbTableType.Visible = false;
                cbCurrentTable.Visible = false;
            }

            if (e.Tab.Index == 1)
            {
                miDeleteParam.Visible = true;
                miAddParam.Visible = true;
            }

            if (e.Tab.Index == 2)
            {
                miDeleteTable.Visible = true;
                miAddTable.Visible = true;
                miDeleteColumn.Visible = true;
                miAddColumn.Visible = true;
            }
        }

        private void pnlProperty_Layout(object sender, LayoutEventArgs e)
        {
            PropertyGridHelper.SetSplitterPosition(pgParams);
            PropertyGridHelper.SetSplitterPosition(pgColumns);
            PropertyGridHelper.SetSplitterPosition(pgGroup);
        }
    }
}
