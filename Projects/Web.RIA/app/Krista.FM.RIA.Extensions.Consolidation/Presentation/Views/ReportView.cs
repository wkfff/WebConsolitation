using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ReportView : View
    {
        private readonly ILinqRepository<D_CD_Report> reportRepository;
        private readonly FormLayoutMarkupService formLayoutMarkupService;
        private readonly List<string> stores;
        private D_CD_Report report;

        public ReportView(ILinqRepository<D_CD_Report> reportRepository, FormLayoutMarkupService formLayoutMarkupService)
        {
            stores = new List<string>();
            this.reportRepository = reportRepository;
            this.formLayoutMarkupService = formLayoutMarkupService;
        }

        public int TaskId
        {
            get
            {
                if (Params.ContainsKey("taskId"))
                {
                    return Convert.ToInt32(Params["taskId"]);
                }

                throw new InvalidOperationException("В запросе не задан обязательный параметр taskId.");
            }
        }

        public override List<Component> Build(ViewPage page)
        {
            report = reportRepository.FindAll().First(x => x.RefTask.ID == TaskId);

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportMain",
                    HideBorders = true,
                    Items = 
                    {
                        new BorderLayout
                        {
                            Center = { Items = { CreateReportControl(page) } }
                        }
                    }
                }
            };
        }

        private static Component CreateErrorWindow(List<string> errors, string title)
        {
            return new ErrorWindow { Title = title, Text = String.Join("<br/>", errors.ToArray()) } 
                .Build(null)[0];
        }

        private Component CreateReportControl(ViewPage page)
        {
            var reportPanel = GetReportSheets(page);

            return new Panel
            {
                Border = false,
                Layout = "fit",
                TopBar = { CreateToolbar(page) },
                Items = 
                { 
                    new TabPanel 
                    { 
                        Border = false,
                        Items = { reportPanel }
                    } 
                }
            };
        }

        private Toolbar CreateToolbar(ViewPage page)
        {
            var toolbar = new Toolbar();

            // Кнопка сохранить
            StringBuilder refreshFunc = new StringBuilder();
            refreshFunc.Append("function refreshReport() {");
            refreshFunc.Append("viewportMain.getEl().mask();");
            foreach (var storeId in stores)
            {
                refreshFunc.Append("store").Append(storeId).Append(".reload();");
            }

            refreshFunc.Append("viewportMain.getEl().unmask();");
            refreshFunc.Append("};");

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("RefreshData", refreshFunc.ToString());

            toolbar.AddIconButton("btnRefresh", Icon.PageRefresh, "Обновить")
                .Listeners.Click.Fn = "refreshReport";

            // Кнопка сохранить
            StringBuilder saveFunc = new StringBuilder();
            saveFunc.Append("function save() {");
            saveFunc.Append("viewportMain.getEl().mask();");
            foreach (var storeId in stores)
            {
                saveFunc.Append("store").Append(storeId).Append(".save();");
            }

            saveFunc.Append("viewportMain.getEl().unmask();");
            saveFunc.Append("};");

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("SaveData", saveFunc.ToString());

            toolbar.AddIconButton("btnSave", Icon.PageSave, "Сохранить изменения")
                .Listeners.Click.Fn = "save";

            // Кнопка вычисления соотношений
            StringBuilder calcAssignFunc = new StringBuilder();
            calcAssignFunc.Append("function calcAssign() {");
            calcAssignFunc.Append("viewportMain.getEl().mask();");
            foreach (var storeId in stores)
            {
                calcAssignFunc.Append("store").Append(storeId).Append(".save();");
            }

            calcAssignFunc.Append("viewportMain.getEl().unmask();");
            calcAssignFunc.Append("};");

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("CalcAssign", calcAssignFunc.ToString());

            var calcBtn = toolbar.AddIconButton("btnCalcAssign", Icon.Calculator, "Вычислить соотношения");
            calcBtn.DirectEvents.Click.Before = "save();";
            calcBtn.DirectEvents.Click.Success = "refreshReport();";
            calcBtn.DirectEvents.Click.Url = "/ConsRelations/CalcReport";
            calcBtn.DirectEvents.Click.CleanRequest = true;
            calcBtn.DirectEvents.Click.FormID = "Form1";
            calcBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("reportId", Convert.ToString(report.ID), ParameterMode.Raw));
            calcBtn.DirectEvents.Click.EventMask.Msg = "Выполняется расчет...";
            calcBtn.DirectEvents.Click.EventMask.ShowMask = true;
            calcBtn.DirectEvents.Click.Timeout = 3 * 60 * 1000;

            var reportBtn = toolbar.AddIconButton("btnReport", Icon.Report, "Отчет");
            reportBtn.DirectEvents.Click.Url = "/ConsReport/Report";
            reportBtn.DirectEvents.Click.CleanRequest = true;
            reportBtn.DirectEvents.Click.IsUpload = true;
            reportBtn.DirectEvents.Click.FormID = "Form1";
            reportBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("reportId", Convert.ToString(report.ID), ParameterMode.Raw));

            return toolbar;
        }

        private IEnumerable<Component> GetReportSheets(ViewPage page)
        {
            List<Component> tabs = new List<Component>();

            Form markupMappings;
            try
            {
                markupMappings = report.RefForm.GetFormMarkupMappings();
            }
            catch (LayoutException e)
            {
                return new ErrorWindow { Title = "Ошибка", Text = e.Message } .Build(page);
            }

#if DEBUG
            var validator = new FormLayoutMarkupValidator();
            var errors = validator.Validate(report.RefForm, markupMappings);
            
            var mappingValidator = new FormMappingValidator();
            errors.AddRange(mappingValidator.Validate(report.RefForm));
            
            if (errors.Count > 0)
            {
                var window = CreateErrorWindow(errors, "Ошибки валидации формы");

                page.Controls.Add(window);
                return new List<Component>();
            }
#endif 

            foreach (var sheet in markupMappings.Sheets)
            {
                var panel = new Panel { Title = sheet.Name, AutoScroll = true, Border = false };

                try
                {
                    // Реквизиты формы
                    if (sheet.HeaderRequisites != null && sheet.HeaderRequisites.Requisites.Count > 0)
                    {
                        var grid = CreateReportRequisites(sheet, RequisiteKinds.Header, report);

                        panel.Items.Add(grid.Build(page));
                        stores.Add(grid.Id);
                    }

                    // Разделы формы
                    if (sheet.Sections.Count > 0)
                    {
                        foreach (var section in sheet.Sections)
                        {
                            // Реквизиты раздела заголовочные
                            if (section.HeaderRequisites != null && section.HeaderRequisites.Requisites.Count > 0)
                            {
                                var headerReqGrid = CreateReportSectionRequisites(sheet, RequisiteKinds.Header, report, section.Code);

                                panel.Items.Add(headerReqGrid.Build(page));
                                stores.Add(headerReqGrid.Id);
                            }

                            // Таблица раздела
                            var grid = CreateSectionTable(sheet, section, report);
                            panel.Items.Add(grid.Build(page));
                            stores.Add(grid.Id);

                            // Реквизиты раздела заключительныеы
                            if (section.FooterRequisites != null && section.FooterRequisites.Requisites.Count > 0)
                            {
                                var footerReqGrid = CreateReportSectionRequisites(sheet, RequisiteKinds.Footer, report, section.Code);

                                panel.Items.Add(footerReqGrid.Build(page));
                                stores.Add(footerReqGrid.Id);
                            }
                        }
                    }

                    // Реквизиты формы
                    if (sheet.FooterRequisites != null && sheet.FooterRequisites.Requisites.Count > 0)
                    {
                        var grid = CreateReportRequisites(sheet, RequisiteKinds.Footer, report);

                        panel.Items.Add(grid.Build(page));
                        stores.Add(grid.Id);
                    }
                }
                catch (LayoutException e)
                {
                    panel.Items.Add(new ErrorWindow { Title = "Ошибка", Text = e.Message } .Build(page));
                }

                tabs.Add(panel);
            }

            return tabs;
        }

        private ExcelReportRequisitesControl CreateReportRequisites(Sheet sheet, RequisiteKinds requisiteKinds, D_CD_Report report)
        {
            var markup = formLayoutMarkupService.GetFormRequisite(report.RefForm, sheet, requisiteKinds);
            var idPrefix = requisiteKinds == RequisiteKinds.Header ? "HR" : "FR";
            return new ExcelReportRequisitesControl
            {
                Id = idPrefix + report.RefForm.InternalName,
                Title = "Реквизиты " + (requisiteKinds == RequisiteKinds.Header ? "заголовочной" : "заключительной") + " части отчета",
                ReportId = report.ID,
                RequisiteKind = requisiteKinds,
                RequisiteClass = RequisiteClass.Report,
                Requisites = report.RefForm.Requisites.Where(x => x.IsHeader == (requisiteKinds == RequisiteKinds.Header)).ToList(),
                LayoutMarkup = markup
            };
        }

        private ExcelReportRequisitesControl CreateReportSectionRequisites(Sheet sheet, RequisiteKinds requisiteKinds, D_CD_Report report, string sectionCode)
        {
            var markup = formLayoutMarkupService.GetSectionRequisite(report.RefForm, sheet, sectionCode, requisiteKinds);
            var idPrefix = requisiteKinds == RequisiteKinds.Header ? "HR" : "FR";
            var reportSection = report.RefForm.Parts.First(x => x.InternalName == sectionCode);
            return new ExcelReportRequisitesControl
            {
                Id = idPrefix + report.RefForm.InternalName + reportSection.InternalName,
                Title = "Реквизиты " + (requisiteKinds == RequisiteKinds.Header ? "заголовочной" : "заключительной") + " части раздела",
                ReportId = report.ID,
                SectionCode = reportSection.Code,
                RequisiteKind = requisiteKinds,
                RequisiteClass = RequisiteClass.Section,
                Requisites = reportSection.Requisites.Where(x => x.IsHeader == (requisiteKinds == RequisiteKinds.Header)).ToList(),
                LayoutMarkup = markup
            };
        }

        private ExcelReportTableControl CreateSectionTable(Sheet sheet, Section section, D_CD_Report report)
        {
            var markup = formLayoutMarkupService.GetSectionTable(report.RefForm, sheet, section.Code);
            D_Form_Part formSection = report.RefForm.Parts.First(x => x.InternalName == section.Code);
            return new ExcelReportTableControl
            {
                Id = formSection.InternalName,
                Title = sheet.Code,
                ReportId = report.ID,
                Section = formSection,
                LayoutMarkup = markup
            };
        }
    }
}
