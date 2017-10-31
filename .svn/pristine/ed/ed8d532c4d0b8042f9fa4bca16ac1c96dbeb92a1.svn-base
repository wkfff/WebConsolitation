using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Extensions.E86N.Auth.Data;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Models.DocumentsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Models.InstitutionsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Services.Reports;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

using Microsoft.Practices.ObjectBuilder2;

using NPOI.HSSF.UserModel;

using Region = NPOI.HSSF.Util.Region;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public class ReportsController : SchemeBoundController
    {
        private readonly IReportService service;
        private readonly INewRestService newRestService;
        private readonly ILinqRepository<F_F_ParameterDoc> docRepository;

        public ReportsController(
            IAuthService auth,
            IReportService service,
            ILinqRepository<F_F_ParameterDoc> docRepository)
        {
           this.service = service;
           this.docRepository = new AuthRepositiory<F_F_ParameterDoc>(
                docRepository,
                auth,
                ppoIdExpr => ppoIdExpr.RefUchr.RefOrgPPO,
                grbsIdExpr => grbsIdExpr.RefUchr.RefOrgGRBS.ID,
                orgIdExpr => orgIdExpr.RefUchr.ID);

            newRestService = Resolver.Get<INewRestService>();
        }

        public ActionResult ImportDocumentsGrid([StateBinder] ReportsHelper.ColumnsAndSorts state, [FiltersBinder] FilterConditions filters)
        {
            state.Columns.Add(
                new ReportsHelper.Column
                {
                    ID = "StructureInn",
                    Width = 186
                });
            state.Columns.Add(
                new ReportsHelper.Column
                {
                    ID = "StructureKpp",
                    Width = 186
                });
            state.Columns.Add(
                new ReportsHelper.Column
                {
                    ID = "StructureType",
                    Width = 186
                });
            var validOrgTypes = new[] { 3, 8, 10 };
            var validColumns = new List<string>
                {
                    "StructureID",
                    "StructureName",
                    "StructureGrbs",
                    "StructurePpo",
                    "StructureInn",
                    "StructureKpp",
                    "StructureCloseDate",
                    "Type",
                    "State",
                    "Url",
                    "Note",
                    "Year",
                    "Closed",
                    "StructureType"
                };

            var data = docRepository.FindAll()
                .Where(doc => validOrgTypes.Contains(doc.RefUchr.RefTipYc.ID))
                .Select(
                    x => new DocumentsRegisterViewModel
                    {
                        ID = x.ID,
                        StructureID = x.RefUchr.ID,
                        StructureName = x.RefUchr.Name,
                        StructureShortName = x.RefUchr.ShortName,
                        StructureType = x.RefUchr.RefTipYc.Name,
                        StructureInn = x.RefUchr.INN,
                        StructureKpp = x.RefUchr.KPP,
                        StructureGrbsCode = x.RefUchr.RefOrgGRBS.Code,
                        StructureGrbs = x.RefUchr.RefOrgGRBS.Name,
                        StructurePpo = x.RefUchr.RefOrgPPO.Name,
                        StructureCloseDate = x.RefUchr.CloseDate,
                        Type = x.RefPartDoc.Name,
                        State = x.RefSost.Name,
                        Note = x.Note,
                        Year = x.RefYearForm.ID,
                        Closed = x.CloseDate.HasValue,
                        ClosedOrg = x.RefUchr.CloseDate.HasValue
                    });

            filters.Conditions
                .ForEach(
                    filter =>
                    {
                        switch (filter.Name)
                        {
                            case "ID":
                                switch (filter.Comparison)
                                {
                                    case Comparison.Eq:
                                        data = data.Where(x => x.ID == filter.ValueAsInt);
                                        break;
                                    case Comparison.Gt:
                                        data = data.Where(x => x.ID > filter.ValueAsInt);
                                        break;
                                    case Comparison.Lt:
                                        data = data.Where(x => x.ID < filter.ValueAsInt);
                                        break;
                                }

                                break;
                            case "State":
                                data = data.Where(x => filter.ValuesList.Contains(x.State));
                                break;
                            case "Type":
                                data = data.Where(x => filter.ValuesList.Contains(x.Type));
                                break;
                            case "StructureName":
                                data =
                                    data.Where(
                                        x =>
                                        x.StructureName.Contains(filter.Value) ||
                                        x.StructureShortName.Contains(filter.Value) ||
                                        x.StructureInn.Contains(filter.Value) ||
                                        x.StructureKpp.Contains(filter.Value));
                                break;
                            case "StructurePpo":
                                data = data.Where(x => x.StructurePpo.Contains(filter.Value));
                                break;
                            case "StructureGrbs":
                                data =
                                    data.Where(
                                        x =>
                                        x.StructureGrbs.Contains(filter.Value) ||
                                        x.StructureGrbsCode.Contains(filter.Value));
                                break;
                            case "StructureCloseDate":
                                switch (filter.Comparison)
                                {
                                    case Comparison.Eq:
                                        data = data.Where(x => x.StructureCloseDate == filter.ValueAsDate);
                                        break;
                                    case Comparison.Lt:
                                        data = data.Where(x => x.StructureCloseDate < filter.ValueAsDate);
                                        break;
                                    case Comparison.Gt:
                                        data = data.Where(x => x.StructureCloseDate > filter.ValueAsDate);
                                        break;
                                }

                                break;
                            case "Note":
                                data = data.Where(x => x.Note.Contains(filter.Value));
                                break;
                            case "Year":
                                var list = filter.ValuesList.Select(s => Convert.ToInt32(s)).ToList();
                                data = data.Where(arg => list.Contains(arg.Year));
                                break;
                            case "Closed":
                                data = data.Where(arg => arg.Closed == filter.ValueAsBoolean);
                                break;
                            case "ClosedOrg":
                                data = data.Where(arg => arg.ClosedOrg == filter.ValueAsBoolean);
                                break;
                        }
                    });

            if (data.Count() != 0)
            {
                var listData = data.ToList();
                state.Sort.ForEach(
                    x =>
                    {
                        switch (x.Field)
                        {
                            case "ID":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.ID).ToList()
                                               : listData.OrderByDescending(y => y.ID).ToList();
                                break;
                            case "StructureName":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.StructureName[0])
                                                     .ThenBy(y => y.StructureName).ToList()
                                               : listData.OrderByDescending(y => y.StructureName[0])
                                                     .ThenBy(y => y.StructureName).ToList();
                                break;
                            case "StructureGrbs":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.StructureGrbs).ToList()
                                               : listData.OrderByDescending(y => y.StructureGrbs).ToList();
                                break;
                            case "StructurePpo":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.StructurePpo).ToList()
                                               : listData.OrderByDescending(y => y.StructurePpo).ToList();
                                break;
                            case "Type":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.Type).ToList()
                                               : listData.OrderByDescending(y => y.Type).ToList();
                                break;
                            case "State":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.State).ToList()
                                               : listData.OrderByDescending(y => y.State).ToList();
                                break;
                            case "Note":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.Note).ToList()
                                               : listData.OrderByDescending(y => y.Note).ToList();
                                break;
                            case "StructureCloseDate":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.StructureCloseDate).ToList()
                                               : listData.OrderByDescending(y => y.StructureCloseDate).ToList();
                                break;
                            case "Year":
                                listData = x.Direction.ToLower() == "asc"
                                               ? listData.OrderBy(y => y.Year.ToString(CultureInfo.InvariantCulture)).ToList()
                                               : listData.OrderByDescending(y => y.Year.ToString(CultureInfo.InvariantCulture)).ToList();
                                break;
                        }
                    });

                var workBook = service
                            .InitializeWorkBookWithoutTemplate(
                                new List<string>
                                    {
                                        ReportsHelper.CreateFirstHeader(filters, state.Sort),
                                        ReportsHelper.CreateSecondHeader()
                                    },
                                ReportsHelper.GetRightColumnsNames(state.Columns.Where(x => !x.Hidden && validColumns.Contains(x.ID)).ToList()),
                                listData.ToList());

                var output = new MemoryStream();
                workBook.Write(output);

                return File(output.ToArray(), "application/vnd.ms-excel", "Реестр документов.xls");
            }

            var result = new AjaxFormResult { Success = false };
            result.ExtraParams["msg"] = "Нет данных";
            result.IsUpload = true;
            return result;
        }

        public ActionResult ImportChangeLogGrid([SortBinder] ReportsHelper.ColumnsAndSort state, [FiltersBinder] FilterConditions filters)
        {
            var data = newRestService.GetItems<F_F_ChangeLog>().Select(
                                                x => new ChangeLogModel
                                                {
                                                    ID = x.ID,
                                                    Date = x.Data.Date,
                                                    DocId = x.DocId,
                                                    Login = x.Login,
                                                    OrgINN = x.OrgINN,
                                                    Time = x.Data.ToString("HH:mm"),
                                                    Year = x.Year,
                                                    RefChangeType = x.RefChangeType.ID,
                                                    RefChangeTypeName = x.RefChangeType.Name,
                                                    RefType = x.RefType.ID,
                                                    RefTypeName = x.RefType.Name,
                                                    Note = x.Note
                                                });

            var model = new ChangeLogModel();
            filters.Conditions
                .ForEach(
                    filter =>
                    {
                        if (filter.Name == model.NameOf(() => model.Date))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    data = data.Where(x => x.Date.Date == filter.ValueAsDate);
                                    break;
                                case Comparison.Lt:
                                    data = data.Where(x => x.Date.Date < filter.ValueAsDate);
                                    break;
                                case Comparison.Gt:
                                    data = data.Where(x => x.Date.Date > filter.ValueAsDate);
                                    break;
                            }
                        }

                        if (filter.Name == model.NameOf(() => model.Login))
                        {
                            data = data.Where(x => x.Login.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.OrgINN))
                        {
                            data = data.Where(x => x.OrgINN.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.Note))
                        {
                            data = data.Where(x => x.Note != null && x.Note.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.RefChangeTypeName))
                        {
                            data = data.Where(x => x.RefChangeTypeName.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.RefTypeName))
                        {
                            data = data.Where(x => x.RefTypeName.Contains(filter.Value));
                        }
                        
                        if (filter.Name == model.NameOf(() => model.Year))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    data = data.Where(x => x.Year == filter.ValueAsInt);
                                    break;
                                case Comparison.Lt:
                                    data = data.Where(x => x.Year < filter.ValueAsInt);
                                    break;
                                case Comparison.Gt:
                                    data = data.Where(x => x.Year > filter.ValueAsInt);
                                    break;
                            }
                        }

                        if (filter.Name == model.NameOf(() => model.DocId))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    data = data.Where(x => x.DocId == filter.ValueAsInt);
                                    break;
                                case Comparison.Lt:
                                    data = data.Where(x => x.DocId < filter.ValueAsInt);
                                    break;
                                case Comparison.Gt:
                                    data = data.Where(x => x.DocId > filter.ValueAsInt);
                                    break;
                            }
                        }
                    });

            if (data.Count() != 0 && state.Sort != null)
            {
                var x = state.Sort;

                if (x.Field == model.NameOf(() => model.Date))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.Date)
                               : data.OrderByDescending(y => y.Date);
                }

                if (x.Field == model.NameOf(() => model.DocId))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.DocId)
                               : data.OrderByDescending(y => y.DocId);
                }

                if (x.Field == model.NameOf(() => model.Login))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.Login)
                               : data.OrderByDescending(y => y.Login);
                }

                if (x.Field == model.NameOf(() => model.OrgINN))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.OrgINN)
                               : data.OrderByDescending(y => y.OrgINN);
                }

                if (x.Field == model.NameOf(() => model.RefChangeTypeName))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.RefChangeTypeName)
                               : data.OrderByDescending(y => y.RefChangeTypeName);
                }

                if (x.Field == model.NameOf(() => model.RefTypeName))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.RefTypeName)
                               : data.OrderByDescending(y => y.RefTypeName);
                }

                if (x.Field == model.NameOf(() => model.Time))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.Time)
                               : data.OrderByDescending(y => y.Time);
                }

                if (x.Field == model.NameOf(() => model.Year))
                {
                    data = x.Direction.ToLower() == "asc"
                               ? data.OrderBy(y => y.Year)
                               : data.OrderByDescending(y => y.Year);
                }
            }

            var workBook = new HSSFWorkbook();
            var sheet = workBook.CreateSheet("Действия пользователей");
            var currentRow = 0;
            var headersCellStyle = new ReportsHelper.HSSFCellStyleForHeader(workBook).CellStyle;

            var headers = new List<string>
                {
                    ReportsHelper.CreateFilterHeader(model, filters, state.Sort),
                    ReportsHelper.CreateSecondHeader()
                };
            foreach (var header in headers)
            {
                sheet.CreateRow(currentRow).CreateCell(0);
                sheet.GetRow(currentRow).Height = 500;
                sheet.GetRow(currentRow).GetCell(0).CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                sheet.GetRow(currentRow).GetCell(0).CellStyle = headersCellStyle;
                sheet.GetRow(currentRow).GetCell(0).SetCellValue(header);
                sheet.AddMergedRegion(
                    new Region
                        {
                            RowFrom = currentRow,
                            RowTo = currentRow,
                            ColumnFrom = 0,
                            ColumnTo = 11
                        });
                currentRow++;
            }

            ReportsHelper.BuildModelTable(data, workBook, currentRow);

            var output = new MemoryStream();
            workBook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Действия пользователей.xls");
        }

        public ActionResult ImportInstitutionsGrid([SortBinder] ReportsHelper.ColumnsAndSort state, [FiltersBinder] FilterConditions filters)
        {
            var model = new InstitutionsRegisterModel();
            var data = newRestService.GetItems<D_Org_Structure>()
                .Select(
                    p => new InstitutionsRegisterModel
                        {
                            ID = p.ID,
                            RefOrgPpo = p.RefOrgPPO.ID,
                            RefOrgPpoName = p.RefOrgPPO.Name,
                            RefTipYc = p.RefTipYc.ID,
                            RefTipYcName = p.RefTipYc.Name,
                            RefOrgGrbs = p.RefOrgGRBS.ID,
                            RefOrgGrbsName = p.RefOrgGRBS.Name,
                            Name = p.Name,
                            ShortName = p.ShortName,
                            Inn = p.INN,
                            Kpp = p.KPP,
                            Status = !p.CloseDate.HasValue,
                            OpenDate = p.OpenDate,
                            CloseDate = p.CloseDate
                        });

            foreach (var filter in filters.Conditions)
            {
                if (filter.Name == model.NameOf(() => model.ID))
                {
                    switch (filter.Comparison)
                    {
                        case Comparison.Eq:
                            data = data.Where(v => v.ID == filter.ValueAsInt);
                            break;
                        case Comparison.Gt:
                            data = data.Where(v => v.ID > filter.ValueAsInt);
                            break;
                        case Comparison.Lt:
                            data = data.Where(v => v.ID < filter.ValueAsInt);
                            break;
                    }
                }

                if (filter.Name == model.NameOf(() => model.Status))
                {
                    data = data.Where(v => v.Status == filter.ValueAsBoolean);
                }

                if (filter.Name == model.NameOf(() => model.OpenDate))
                {
                    switch (filter.Comparison)
                    {
                        case Comparison.Eq:
                            data = data.Where(v => v.OpenDate == filter.ValueAsDate);
                            break;
                        case Comparison.Gt:
                            data = data.Where(v => v.OpenDate > filter.ValueAsDate);
                            break;
                        case Comparison.Lt:
                            data = data.Where(v => v.OpenDate < filter.ValueAsDate);
                            break;
                    }
                }

                if (filter.Name == model.NameOf(() => model.CloseDate))
                {
                    switch (filter.Comparison)
                    {
                        case Comparison.Eq:
                            data = data.Where(v => v.CloseDate == filter.ValueAsDate);
                            break;
                        case Comparison.Gt:
                            data = data.Where(v => v.CloseDate > filter.ValueAsDate);
                            break;
                        case Comparison.Lt:
                            data = data.Where(v => v.CloseDate < filter.ValueAsDate);
                            break;
                    }
                }

                var filterValue = filter.Value;
                if (filter.Name == model.NameOf(() => model.RefOrgGrbsName))
                {
                    data = data.Where(v => v.RefOrgGrbsName.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.RefOrgPpoName))
                {
                    data = data.Where(v => v.RefOrgPpoName.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.Name))
                {
                    data = data.Where(v => v.Name.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.ShortName))
                {
                    data = data.Where(v => v.ShortName.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.Inn))
                {
                    data = data.Where(v => v.Inn.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.Kpp))
                {
                    data = data.Where(v => v.Kpp.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.RefTipYcName))
                {
                    data = data.Where(v => v.RefTipYcName.Contains(filterValue));
                }
            }

            if (data.Count() != 0)
            {
                var listData = data.ToList();
                state.Sort.Do(
                    x =>
                    {
                        if (x.Field == model.NameOf(() => model.ID))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.ID).ToList()
                                           : listData.OrderByDescending(y => y.ID).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.OpenDate))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.OpenDate).ToList()
                                           : listData.OrderByDescending(y => y.OpenDate).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.CloseDate))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.CloseDate).ToList()
                                           : listData.OrderByDescending(y => y.CloseDate).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.Name))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.Name).ToList()
                                           : listData.OrderByDescending(y => y.Name).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.ShortName))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.ShortName).ToList()
                                           : listData.OrderByDescending(y => y.ShortName).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.Inn))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.Inn).ToList()
                                           : listData.OrderByDescending(y => y.Inn).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.Kpp))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.Kpp).ToList()
                                           : listData.OrderByDescending(y => y.Kpp).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.RefTipYcName))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.RefTipYcName).ToList()
                                           : listData.OrderByDescending(y => y.RefTipYcName).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.RefOrgGrbsName))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.RefOrgGrbsName).ToList()
                                           : listData.OrderByDescending(y => y.RefOrgGrbsName).ToList();
                        }

                        if (x.Field == model.NameOf(() => model.RefOrgPpoName))
                        {
                            listData = x.Direction.ToLower() == "asc"
                                           ? listData.OrderBy(y => y.RefOrgPpoName).ToList()
                                           : listData.OrderByDescending(y => y.RefOrgPpoName).ToList();
                        }
                    });

                var workBook = new HSSFWorkbook();
                var sheet = workBook.CreateSheet("Реестр учреждений");
                var currentRow = 0;
                var headersCellStyle = new ReportsHelper.HSSFCellStyleForHeader(workBook).CellStyle;
                var columns = state.Columns.Where(t => !t.Hidden).Select(t => t.ID);

                var headers = new List<string>
                {
                    ReportsHelper.CreateFilterHeader(model, filters, state.Sort),
                    ReportsHelper.CreateSecondHeader()
                };
                foreach (var header in headers)
                {
                    sheet.CreateRow(currentRow).CreateCell(0);
                    sheet.GetRow(currentRow).Height = 500;
                    sheet.GetRow(currentRow).GetCell(0).CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                    sheet.GetRow(currentRow).GetCell(0).CellStyle = headersCellStyle;
                    sheet.GetRow(currentRow).GetCell(0).SetCellValue(header);
                    sheet.AddMergedRegion(
                        new Region
                        {
                            RowFrom = currentRow,
                            RowTo = currentRow,
                            ColumnFrom = 0,
                            ColumnTo = columns.Count() - 2,
                        });
                    currentRow++;
                }

                ReportsHelper.BuildModelTable(listData, workBook, currentRow, columns);

                var output = new MemoryStream();
                workBook.Write(output);

                return File(output.ToArray(), "application/vnd.ms-excel", "Реестр учреждений.xls");
            }

            var result = new AjaxFormResult { Success = false };
            result.ExtraParams["msg"] = "Нет данных";
            result.IsUpload = true;
            return result;
        }

        public ActionResult GetDocReport(int typeDoc, string reportDate, bool isPPO, string docYear)
        {
            var partDoc = newRestService.GetItem<FX_FX_PartDoc>(typeDoc);

            DateTime date;
            if (reportDate.IsNotNullOrEmpty())
            {
                if (reportDate.First() == '\"' && reportDate.Last() == '\"')
                {
                    reportDate = reportDate.Substring(1, 19);
                }

                date = DateTime.Parse(reportDate).AddHours(23).AddMinutes(59);
            }
            else
            {
                date = DateTime.Now;
            }

            var workBook = service.GetDocReport(partDoc, date, isPPO, Server.MapPath(@"\Content\ReportTemplate.xls"), docYear);

            var output = new MemoryStream();
            workBook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "{0} на {1}.xls".FormatWith(partDoc.Name, docYear));
        }

        // todo зарефакторить. Сделать один экшен на отчеты с параметром отчета
        public ActionResult GetStateTaskForm(int docId)
        {
            var workBook = service.GetStateTaskForm(docId);

            var output = new MemoryStream();
            workBook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Печатная форма ГЗ {0}.xls".FormatWith(DateTime.Now.ToLongDateString()));
        }

        public ActionResult GetStateTask2016Form(int docId)
        {
            var workBook = service.GetStateTaskForm2016(docId);

            var output = new MemoryStream();
            workBook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Печатная форма ГЗ {0}.xls".FormatWith(DateTime.Now.ToLongDateString()));
        }

        public ActionResult GetAnalReport(string reportDate)
        {
            var date = reportDate.IsNotNullOrEmpty() ? DateTime.Parse(reportDate.Substring(1, 19)).AddHours(23).AddMinutes(59)
                : DateTime.Now;

            var workBook = service.GetAnalReport(date);

            var output = new MemoryStream();
            workBook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "{0} на {1}.xls".FormatWith("Аналитический отчет в разрезе видов деятельности и ППО", date));
        }

        public ActionResult GetNewAnalReport(string reportDate, bool isPPO)
        {
            var date = reportDate.IsNotNullOrEmpty() ? DateTime.Parse(reportDate.Substring(1, 19)).AddHours(23).AddMinutes(59)
                : DateTime.Now;

            var workBook = service.GetNewAnalReport(date, isPPO);

            var output = new MemoryStream();
            workBook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "{0} на {1}.xls".FormatWith("Аналитический отчет в разрезе ППО и типов учреждений", date));
        }

        public ActionResult GetMonitoringPlacementInfoReport(string reportDate, string docYear)
        {
            DateTime date;
            if (reportDate.IsNotNullOrEmpty())
            {
                if (reportDate.First() == '\"' && reportDate.Last() == '\"')
                {
                    reportDate = reportDate.Substring(1, 19);
                }

                date = DateTime.Parse(reportDate).AddHours(23).AddMinutes(59);
            }
            else
            {
                date = DateTime.Now;
            }

            var workBook = service.GetMonitoringPlacementInfoReport(date, docYear);

            var output = new MemoryStream();
            workBook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "{0} на {1}.xls".FormatWith("Мониторинг размещения сведений", docYear));
        }
    }
}