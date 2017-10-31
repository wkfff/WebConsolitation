using System;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;
using Krista.Diagnostics;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Extensions.E86N.Auth.Data;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.DocumentsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.ParameterDocService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public sealed class DocumentsController : SchemeBoundController
    {
        private readonly ILinqRepository<F_F_ParameterDoc> documentsRepository;
        private readonly IParameterDocService parameterDocService;
        private readonly IChangeLogService logService;

        public DocumentsController(IAuthService auth, ILinqRepository<F_F_ParameterDoc> documentsRepository)
        {
            this.documentsRepository = new AuthRepositiory<F_F_ParameterDoc>(
                documentsRepository,
                auth,
                ppoIdExpr => ppoIdExpr.RefUchr.RefOrgPPO,
                grbsIdExpr => grbsIdExpr.RefUchr.RefOrgGRBS.ID,
                orgIdExpr => orgIdExpr.RefUchr.ID);

            parameterDocService = Resolver.Get<IParameterDocService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public ActionResult Read([FiltersBinder] FilterConditions filters, int limit = 10000, int start = 0)
        {
            var validOrgTypes = new[]
                {
                    FX_Org_TipYch.BudgetaryID,
                    FX_Org_TipYch.GovernmentID,
                    FX_Org_TipYch.AutonomousID
                };

            var data = documentsRepository.FindAll()
                .Where(doc => validOrgTypes.Contains(doc.RefUchr.RefTipYc.ID))
                .Select(
                    x => new DocumentsRegisterViewModel
                        {
                            ID = x.ID,
                            StructureID = x.RefUchr.ID,
                            StructureName = x.RefUchr.Name,
                            StructureShortName = x.RefUchr.ShortName,
                            StructureInn = x.RefUchr.INN,
                            StructureKpp = x.RefUchr.KPP,
                            StructureGrbsCode = x.RefUchr.RefOrgGRBS.Code,
                            StructureGrbs = x.RefUchr.RefOrgGRBS.Name,
                            StructurePpo = x.RefUchr.RefOrgPPO.Name,
                            StructureCloseDate = x.RefUchr.CloseDate,
                            Type = x.RefPartDoc.Name,
                            State = x.RefSost.Name,
                            Url = string.Concat(x.RefPartDoc.Url, "?docId=", x.ID),
                            Note = x.Note,
                            Year = x.RefYearForm.ID,
                            Closed = x.CloseDate.HasValue,
                            ClosedOrg = x.RefUchr.CloseDate.HasValue && x.RefUchr.CloseDate <= DateTime.Now
                        });

            filters.Conditions
                .Each(
                    filter =>
                    {
                        try
                        {
                            // костыль для проверки значения в фильтре-списке. Если пропускать пустой фильтр то на момент получения данных будет валиться эгзепшен
                            if (filter.FilterType.Equals(FilterType.List))
                            {
                                filter.ValuesList.Count();
                            }

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
                        }
                        catch
                        {
                            // ignored
                        }
                    });
            
            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }

        public ActionResult CloseDocument(int recId)
        {
            try
            {
                Resolver.Get<IVersioningService>().CloseDocument(recId);

                return new RestResult { Success = true };
            }
            catch (Exception)
            {
                return new RestResult { Success = false };
            }
        }

        public ActionResult OpenDocument(int recId)
        {
            try
            {
                Resolver.Get<IVersioningService>().OpenDocument(recId);

                return new RestResult { Success = true };
            }
            catch (Exception)
            {
                return new RestResult { Success = false };
            }
        }

        [HttpDelete]
        public RestResult Destroy(int id)
        {
            var doc = parameterDocService.Load<F_F_ParameterDoc>(id);

            try
            {
                parameterDocService.BeginTransaction();
                
                parameterDocService.Delete(id);
                logService.WriteChange(doc, FX_FX_ChangeLogActionType.DeleteDocument);

                if (parameterDocService.HaveTransaction)
                {
                    parameterDocService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Документ удален" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                logService.WriteChange(doc, FX_FX_ChangeLogActionType.DeleteDocumentAbort);

                if (parameterDocService.HaveTransaction)
                {
                    parameterDocService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}