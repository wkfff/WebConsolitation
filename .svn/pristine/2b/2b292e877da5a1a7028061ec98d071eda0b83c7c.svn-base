using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Data;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.StartDocModel;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.ParameterDocService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public sealed class StartDocController : SchemeBoundController
    {
        private readonly IAuthService auth;
        private readonly IChangeLogService logService;
        private readonly IParameterDocService parameterDocService;
        private readonly IStateSystemService stateSystemService;
        private readonly ILinqRepository<D_Org_Structure> tableRepository;
        private readonly StartDocDocumentViewModel model = new StartDocDocumentViewModel();
        private readonly StartDocInstitutionsViewModel modelInst = new StartDocInstitutionsViewModel();

        public StartDocController()
        {
            parameterDocService = Resolver.Get<IParameterDocService>();
            stateSystemService = Resolver.Get<IStateSystemService>();
            auth = Resolver.Get<IAuthService>();
            logService = Resolver.Get<IChangeLogService>();
            tableRepository = new AuthRepositiory<D_Org_Structure>(
                stateSystemService.GetRepository<D_Org_Structure>(),
                auth,
                ppoIdExpr => ppoIdExpr.RefOrgPPO,
                grbsIdExpr => grbsIdExpr.RefOrgGRBS.ID,
                orgIdExpr => orgIdExpr.ID);
        }

        public ActionResult GetNote(int docId)
        {
            var data = stateSystemService.GetItems<F_F_ParameterDoc>()
                .Where(x => x.ID == docId)
                .Select(
                    x => new StartDocDocumentViewModel
                        {
                            ID = x.ID,
                            Note = x.Note
                        });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult SetNote(string data, int docId)
        {
            var dataUpdate = JsonUtils.FromJsonRaw(data);

            var note = JsonUtils.GetFieldOrDefault(dataUpdate, "Note", string.Empty);
            
            if (note != string.Empty)
            {
                stateSystemService.ChangeNotes(docId, note, false);
            }
            
            return new RestResult { Success = true };
        }

        public RestResult GetPpoInside()
        {
            var data = stateSystemService.GetItems<D_Org_PPO>();

            if (auth.IsPpoUser())
            {
                var code = auth.Profile.RefUchr.RefOrgPPO.Code;
                data = data.Where(x => x.Code.StartsWith(code.Substring(0, 5)));
            }

            return new RestResult
                {
                    Success = true,
                    Data = data.Select(x => new { x.ID, x.Code, x.Name })
                };
        }

        public RestResult Read(int masterId, int yf)
        {
            var master = masterId;
            if (master == -1 && !auth.IsAdmin() && !auth.IsGrbsUser() && !auth.IsPpoUser() && !auth.IsSpectator())
            {
                master = auth.Profile.RefUchr.ID;
            }

            var data = stateSystemService.GetItems<F_F_ParameterDoc>()
                .Where(x => x.RefUchr.ID == master)
                .Select(
                    x => new StartDocDocumentViewModel
                        {
                            ID = x.ID,
                            Note = x.Note,
                            PlanThreeYear = x.PlanThreeYear,
                            RefPartDoc = x.RefPartDoc.ID,
                            RefPartDocName = x.RefPartDoc.Name,
                            RefSost = x.RefSost.ID,
                            RefUchr = x.RefUchr.ID,
                            RefYearForm = x.RefYearForm.ID,
                            OpeningDate = x.OpeningDate,
                            CloseDate = x.CloseDate,
                            FormationDate = x.FormationDate,
                            Url = x.RefPartDoc.Url
                    });

            if (yf != -1)
            {
                data = data.Where(f => (f.RefYearForm == yf));
            }

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public RestResult Save(string data, int masterId)
        {
            try
            {
                var flag = false;
                int institution;
                if (masterId == -1 && !auth.IsAdmin() && !auth.IsGrbsUser() && !auth.IsPpoUser() && !auth.IsSpectator())
                {
                    institution = auth.Profile.RefUchr.ID;
                }
                else
                {
                    institution = masterId;
                }

                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ValidateData(dataUpdate, institution);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<F_F_ParameterDoc>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    flag = true;

                    record.OpeningDate = DateTime.Now;

                    var schemStateTransitionsID = stateSystemService
                        .GetSchemStateTransitionsID(record.RefPartDoc.ID);
                    if (schemStateTransitionsID != null)
                    {
                        record.RefSost = stateSystemService.GetState(
                            stateSystemService.GetStartStateID(schemStateTransitionsID.Value));
                    }
                    else
                    {
                        record.RefSost = stateSystemService.GetState((int)StatesType.Сreated);
                    }

                    msg = "Новая запись добавлена";
                }
                else
                {
                    record.RefSost = stateSystemService.GetItem<FX_Org_SostD>(record.RefSost.ID);
                }

                record.RefUchr = stateSystemService.GetItem<D_Org_Structure>(institution);
                record.RefYearForm = stateSystemService.GetItem<FX_Fin_YearForm>(record.RefYearForm.ID);
                record.RefPartDoc = stateSystemService.GetItem<FX_FX_PartDoc>(record.RefPartDoc.ID);

                stateSystemService.Save(record);
                if (flag)
                {
                    logService.WriteChange(record, FX_FX_ChangeLogActionType.AddDocument);
                }
                else
                {
                    logService.WriteChangeDocDetail(record);
                }

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = stateSystemService.GetItems<F_F_ParameterDoc>()
                        .Where(p => p.ID == record.ID)
                        .Select(
                            x => new StartDocDocumentViewModel
                                {
                                    ID = x.ID,
                                    Note = x.Note,
                                    PlanThreeYear = x.PlanThreeYear,
                                    RefPartDoc = x.RefPartDoc.ID,
                                    RefPartDocName = x.RefPartDoc.Name,
                                    RefSost = x.RefSost.ID,
                                    RefUchr = x.RefUchr.ID,
                                    RefYearForm = x.RefYearForm.ID,
                                    OpeningDate = x.OpeningDate,
                                    CloseDate = x.CloseDate,
                                    FormationDate = x.FormationDate,
                                    Url = x.RefPartDoc.Url
                            })
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult Delete(int id)
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

        public ActionResult ReadInstitutions(int limit, int start, int? ppo, int? grbs, [FiltersBinder] FilterConditions filters)
        {
            var data = tableRepository.FindAll()
                .Where(x => x.CloseDate == null)
                .Select(
                    p => new StartDocInstitutionsViewModel
                    {
                        ID = p.ID,
                        Name = p.Name,
                        ShortName = p.ShortName,
                        INN = p.INN,
                        RefTipYcName = p.RefTipYc != null ? p.RefTipYc.Name : string.Empty,
                        RefOrgPPO = p.RefOrgPPO != null ? p.RefOrgPPO.ID : -1,
                        RefOrgGRBS = p.RefOrgGRBS != null ? p.RefOrgGRBS.ID : -1
                    });

            foreach (var filter in filters.Conditions)
            {
                var filterValue = filter.Value;
                if (filter.Name == modelInst.NameOf(() => modelInst.ID))
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

                if (filter.Name == modelInst.NameOf(() => modelInst.Name))
                {
                    data = data.Where(v => v.Name.Contains(filterValue) || v.INN.Contains(filterValue));
                }

                if (filter.Name == modelInst.NameOf(() => modelInst.ShortName))
                {
                    data = data.Where(v => v.ShortName.Contains(filterValue));
                }

                if (filter.Name == modelInst.NameOf(() => modelInst.RefTipYcName))
                {
                    data = data.Where(v => v.RefTipYcName.Contains(filterValue));
                }

                if (filter.Name == modelInst.NameOf(() => modelInst.INN))
                {
                    data = data.Where(v => v.INN.Contains(filterValue));
                }
            }

            if ((ppo != -1) || (grbs != -1))
            {
                if ((ppo != -1) && (grbs == -1))
                {
                    data = data.Where(f => (f.RefOrgPPO == ppo));
                }
                else
                {
                    data = data.Where(f => (f.RefOrgPPO == ppo) && (f.RefOrgGRBS == grbs));
                }
            }

            // кроме женщин и детей
            var cachedFounder = tableRepository.FindAll()
                .Join(
                    stateSystemService.GetItems<D_Org_OrgYchr>(),
                    structure => new
                    {
                        inn = structure.INN,
                        kpp = structure.KPP
                    },
                    ychr => new
                    {
                        ychr.RefNsiOgs.inn,
                        ychr.RefNsiOgs.kpp
                    },
                    (structure, ychr) => structure.ID)
                .ToList();

            var cachedFounderByName = tableRepository.FindAll()
                .Join(
                    stateSystemService.GetItems<D_Org_OrgYchr>(),
                    structure => structure.Name,
                    ychr => ychr.Name,
                    (structure, ychr) => structure.ID)
                .ToList();

            var storeData = data.Where(x => !cachedFounder.Union(cachedFounderByName).ToList().Contains(x.ID)).ToList();

            return new AjaxStoreResult(storeData.Skip(start).Take(limit), storeData.Count);
        }

        private string ValidateData(IDictionary<string, object> record, int parentId)
        {
            const string Msg = "Вы не можете создать документ \"Паспорт учреждения\" <br>";
            const string Msg1 = "Вы не можете изменять {0} <br>";
            const string Msg2 = "Создание нового документа невозможно, так как предыдущий документ не закрыт <br>";

            var message = new StringBuilder(string.Empty);

            if (!auth.IsAdmin() && Convert.ToInt32(record[model.NameOf(() => model.RefPartDoc)]) == FX_FX_PartDoc.PassportDocTypeID)
            {
                message.Append(Msg);
            }

            var id = Convert.ToInt32(record[model.NameOf(() => model.ID)]);

            if (id > 0)
            {
                if (!auth.IsAdmin())
                {
                    var paramdoc = stateSystemService.Load<F_F_ParameterDoc>(id);

                    if (paramdoc.RefPartDoc.ID != Convert.ToInt32(record[model.NameOf(() => model.RefPartDoc)]))
                    {
                        message.Append(Msg1.FormatWith("Тип документа"));
                    }

                    if (paramdoc.RefYearForm.ID != Convert.ToInt32(record[model.NameOf(() => model.RefYearForm)]))
                    {
                        message.Append(Msg1.FormatWith("Год формирования"));
                    }
                }
            }
            else
            {
                if (stateSystemService.GetItems<F_F_ParameterDoc>()
                    .Any(
                        x =>
                        x.RefUchr.ID == parentId && x.RefPartDoc.ID == Convert.ToInt32(record[model.NameOf(() => model.RefPartDoc)]) &&
                        x.RefYearForm.ID == Convert.ToInt32(record[model.NameOf(() => model.RefYearForm)]) &&
                        !x.CloseDate.HasValue))
                {
                    message.Append(Msg2);
                }
            }

            if ((string)record[model.NameOf(() => model.FormationDate)] == string.Empty)
            {
                message.Append("Необходимо заполнить дату утверждения");
            }

            var formationDate = Convert.ToDateTime(record[model.NameOf(() => model.FormationDate)]);
            var structure = stateSystemService.GetItem<D_Org_Structure>(parentId);
            var yearForm = Convert.ToInt32(record[model.NameOf(() => model.RefYearForm)]);
            if (!ValidateYearForm(yearForm, structure, formationDate))
            {
                message.Append("Неверно указан год формирования");
            }

            return message.ToString();
        }

        private bool ValidateYearForm(int yearForm, D_Org_Structure structure, DateTime formationDate)
        {
            if (Math.Abs(yearForm - formationDate.Year) > 1)
            {
                return false;
            }
                
            var historyType = structure.TypeHistories.SingleOrDefault(
                    x => x.DateStart <= formationDate && formationDate <= x.DateEnd);
            if (historyType != null)
            {
                return historyType.DateStart.Year <= yearForm && yearForm <= historyType.DateEnd.Year;
            }
                
            var lastType = structure.TypeHistories.OrderByDescending(x => x.DateEnd).FirstOrDefault();
            if (lastType != null)
            {
                return yearForm >= lastType.DateEnd.Year;
            }
                
            return true;
        }
    }
}
