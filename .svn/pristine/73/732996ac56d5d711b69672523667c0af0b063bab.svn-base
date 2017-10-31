using System;
using System.Linq;
using System.Web.Mvc;

using Ext.Net.MVC;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.ParameterDocService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public sealed class ParameterDocController : SchemeBoundController
    {
        private readonly IParameterDocService paramDocService;

        private readonly ICommonDataService commonDataService;
        
        public ParameterDocController()
        {
            paramDocService = Resolver.Get<IParameterDocService>();
            commonDataService = Resolver.Get<ICommonDataService>();
        }

        public ActionResult Read(int itemId)
        {
            var data = paramDocService.GetItems<F_F_ParameterDoc>()
                .Where(doc => doc.ID == itemId)
                .Select(item => new ParameterDocViewModel
                {
                    ID = item.ID,
                    PlanThreeYear = item.PlanThreeYear,
                    Note = item.Note,
                    RefPartDocID = item.RefPartDoc.ID,
                    RefSostID = item.RefSost.ID,
                    RefUchrID = item.RefUchr.ID,
                    RefYearFormID = item.RefYearForm.ID,
                    RefUchrID_RefOrgPPOID_Name = item.RefUchr.RefOrgPPO != null ? item.RefUchr.RefOrgPPO.Name : "(ППО не указано)",
                    RefUchrID_RefOrgGRBSID_Name = item.RefUchr.RefOrgGRBS != null ? item.RefUchr.RefOrgGRBS.Name : "(ГРБС не указано)",
                    RefUchrID_Name = item.RefUchr.Name,
                    RefUchrID_RefTypYcID_Name = commonDataService.GetTypeOfInstitution(item).Name,
                    PlanThreeYear_Name = item.PlanThreeYear ? "3 года" : "1 год",
                    RefPartDocID_Name = item.RefPartDoc.Name,
                    RefSostID_Name = item.RefSost.Name,
                    OpeningDate = item.OpeningDate.HasValue ? item.OpeningDate.Value.ToString("dd.MM.yyyy") : string.Empty,
                    CloseDate = item.CloseDate.HasValue ? item.CloseDate.Value.ToString("dd.MM.yyyy") : string.Empty,
                    INN = item.RefUchr.INN
                });
            return new AjaxStoreResult(data, data.Count());
        }
        
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Update(string data)
        {
            try
            {
                var viewModel = JavaScriptDomainConverter<ParameterDocViewModel>.DeserializeSingle(data);

                var item = new F_F_ParameterDoc
                    {
                        ID = viewModel.ID,
                        PlanThreeYear = viewModel.PlanThreeYear,
                        Note = viewModel.Note,
                        RefPartDoc = paramDocService.GetItem<FX_FX_PartDoc>(viewModel.RefPartDocID),
                        RefSost = paramDocService.GetItem<FX_Org_SostD>(viewModel.RefSostID),
                        RefUchr = paramDocService.GetItem<D_Org_Structure>(viewModel.RefUchrID),
                        RefYearForm = paramDocService.GetItem<FX_Fin_YearForm>(viewModel.RefYearFormID),
                        OpeningDate = viewModel.ID < 0 ? DateTime.Now : DateTime.Parse(viewModel.OpeningDate),
                        CloseDate = DateTime.Parse(viewModel.CloseDate)
                    };
                paramDocService.Save(item);
                Resolver.Get<IChangeLogService>().WriteChangeDocDetail(item);
                return new RestResult
                    {
                        Success = true,
                        Message = "Запись создана.",
                        Data = paramDocService.GetItems<F_F_ParameterDoc>()
                            .Where(doc => doc.ID == item.ID)
                            .Select(
                                doc => new ParameterDocViewModel
                                    {
                                        ID = doc.ID,
                                        PlanThreeYear = doc.PlanThreeYear,
                                        Note = doc.Note,
                                        RefPartDocID = doc.RefPartDoc.ID,
                                        RefSostID = doc.RefSost.ID,
                                        RefUchrID = doc.RefUchr.ID,
                                        RefYearFormID = doc.RefYearForm.ID,
                                        RefUchrID_RefOrgPPOID_Name = (doc.RefUchr.RefOrgPPO != null) ? doc.RefUchr.RefOrgPPO.Name : "(ППО не указано)",
                                        RefUchrID_RefOrgGRBSID_Name = (doc.RefUchr.RefOrgGRBS != null) ? doc.RefUchr.RefOrgGRBS.Name : "(ГРБС не указано)",
                                        RefUchrID_Name = doc.RefUchr.Name,
                                        RefUchrID_RefTypYcID_Name = commonDataService.GetTypeOfInstitution(item).Name,
                                        PlanThreeYear_Name = doc.PlanThreeYear ? "3 года" : "1 год",
                                        RefPartDocID_Name = doc.RefPartDoc.Name,
                                        RefSostID_Name = doc.RefSost.Name,
                                        OpeningDate = doc.OpeningDate.HasValue ? doc.OpeningDate.Value.ToString("dd.MM.yyyy") : string.Empty,
                                        CloseDate = doc.CloseDate.HasValue ? doc.CloseDate.Value.ToString("dd.MM.yyyy") : string.Empty,
                                        INN = item.RefUchr.INN
                                })
                    };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e); //// Для сообщения методу-обертке о необходимости отката транзакции при ошибке.
                return new RestResult { Success = false, Message = "RestController::Create: Ошибка создания записи: " + e.Message, Data = null };
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                paramDocService.BeginTransaction();
                paramDocService.Delete(id);
                if (paramDocService.HaveTransaction)
                {
                    paramDocService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена.", Data = null };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                if (paramDocService.HaveTransaction)
                {
                    paramDocService.RollbackTransaction();
                }

                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "RestController::Delete: Ошибка удаления записи: " + e.Message, Data = null };
            }
        }
    }
}
