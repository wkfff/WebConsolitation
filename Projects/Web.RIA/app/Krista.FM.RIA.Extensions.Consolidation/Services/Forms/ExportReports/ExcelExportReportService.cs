using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports
{
    public class ExcelExportReportService : IExportReportService
    {
        private readonly IReportSectionDataService sectionDataService;
        private readonly IReportRequisiteDataService reportRequisiteDataService;
        private readonly ISectionRequisiteDataService sectionRequisiteDataService;
        private HSSFWorkbook wb;
        private HSSFSheet sheet;
        private Sheet mappingSheet;
        private Section mappingSection;
        private D_CD_Report reportData;
        private D_Report_Section reportSection;

        public ExcelExportReportService(
            IReportSectionDataService sectionDataService,
            IReportRequisiteDataService reportRequisiteDataService,
            ISectionRequisiteDataService sectionRequisiteDataService)
        {
            this.sectionDataService = sectionDataService;
            this.reportRequisiteDataService = reportRequisiteDataService;
            this.sectionRequisiteDataService = sectionRequisiteDataService;
        }

        public Stream Export(D_CD_Report report)
        {
            var form = report.RefForm;
            var mapping = form.GetFormMarkupMappings();
            var memoryStream = new MemoryStream(form.TemplateFile);
            wb = new HSSFWorkbook(memoryStream, true);
            reportData = report;

            foreach (var currentSheet in mapping.Sheets)
            {
                mappingSheet = currentSheet;
                sheet = wb.GetSheet(mappingSheet.Name);

                if (sheet == null)
                {
                    continue;
                }
                
                FillRequisites(RequisiteKinds.Header);
                FillRequisites(RequisiteKinds.Footer);

                foreach (var currentSection in mappingSheet.Sections)
                {
                    mappingSection = currentSection;
                    reportSection = reportData.Sections
                        .FirstOrDefault(s => s.RefFormSection.InternalName == mappingSection.Code);

                    if (reportSection == null)
                    {
                        continue;
                    }

                    FillSectionRequisites(RequisiteKinds.Header);
                    FillSectionRequisites(RequisiteKinds.Footer);
                    var regionTable = wb.GetRegionArea(mappingSection.Table.RowsRegion);

                    var tblSection = sectionDataService.GetAll(reportData, reportSection.RefFormSection.Code);

                    foreach (var mappingRow in mappingSection.Table.Rows.OrderByDescending(t => Convert.ToInt32(t.Region)))
                    {
                        var metaRow = reportSection.RefFormSection.Rows.FirstOrDefault(x => x.Name == mappingRow.Code);

                        if (metaRow == null)
                        {
                            continue;
                        }

                        var rowsData = tblSection.Where(x => x.MetaRowId == metaRow.ID);
                        var rowCount = rowsData.Count();

                        if (rowCount == 0)
                        {
                            continue;
                        }

                        var rowIndex = regionTable.FirstCell.Row + Convert.ToInt32(mappingRow.Region);

                        for (var i = 1; i < rowCount; i++)
                        {
                            Consolidation.NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                        }

                        foreach (var rowData in rowsData)
                        {
                            foreach (var mappingColumn in mappingSection.Table.Columns)
                            {
                                var regionColumn = wb.GetRegionArea(mappingColumn.Region);
                                Consolidation.NPOIHelper.SetCellValue(
                                    sheet,
                                    rowIndex,
                                    regionColumn.FirstCell.Col,
                                    rowData.Get(mappingColumn.Code));
                            }

                            rowIndex++;
                        }
                    }
                }
            }

            var ms = new MemoryStream();
            wb.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private void CommonFillRequisite(DataTable tblRequisites, Element requisite)
        {
            var region = wb.GetRegionArea(requisite.Region);
            if (tblRequisites.Rows.Count == 0)
            {
                return;
            }

            var columnName = requisite.Code.ToUpper();

            if (!tblRequisites.Columns.Contains(columnName))
            {
                return;
            }

            var requisiteValue = tblRequisites.Rows[0][columnName];

            Consolidation.NPOIHelper.SetCellValue(
                sheet,
                region.FirstCell.Row,
                region.FirstCell.Col,
                requisiteValue);
        }

        private void FillRequisites(RequisiteKinds kind)
        {
            var exists = reportData.RefForm.Requisites.Any(x => x.IsHeader == (kind == RequisiteKinds.Header));

            if (!exists)
            {
                return;
            }

            var tblRequisites = reportRequisiteDataService.Get(reportData, reportData.RefForm.Requisites.Where(x => x.IsHeader == (kind == RequisiteKinds.Header)), kind);
            List<Element> requisites = null;

            switch (kind)
            {
                case RequisiteKinds.Header:
                    if (mappingSheet.HeaderRequisites != null)
                    {
                        requisites = mappingSheet.HeaderRequisites.Requisites;
                    }

                    break;
                default:
                    if (mappingSheet.FooterRequisites != null)
                    {
                        requisites = mappingSheet.FooterRequisites.Requisites;
                    }

                    break;
            }

            if (requisites == null)
            {
                return;
            }

            foreach (var requisite in requisites)
            {
                CommonFillRequisite(tblRequisites, requisite);
            }
        }

        private void FillSectionRequisites(RequisiteKinds kind)
        {
            var exists = reportSection.RefFormSection.Requisites.Any(x => x.IsHeader == (kind == RequisiteKinds.Header));
            if (!exists)
            {
                return;
            }

            var tblRequisites = sectionRequisiteDataService.Get(reportData, reportSection, kind);
            List<Element> requisites = null;

            switch (kind)
            {
                case RequisiteKinds.Header:
                    if (mappingSection.HeaderRequisites != null)
                    {
                        requisites = mappingSection.HeaderRequisites.Requisites;
                    }

                    break;
                default:
                    if (mappingSection.FooterRequisites != null)
                    {
                        requisites = mappingSection.FooterRequisites.Requisites;
                    }

                    break;
            }

            if (requisites == null)
            {
                return;
            }

            foreach (var requisite in requisites)
            {
                CommonFillRequisite(tblRequisites, requisite);
            }
        }
    }
}
