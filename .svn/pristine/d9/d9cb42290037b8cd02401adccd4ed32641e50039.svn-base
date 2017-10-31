using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controllers
{
    public class EO15AIPCObjectCardController : SchemeBoundController
    {
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly IConstructionService constrRepository;
        private readonly ILinqRepository<D_Territory_RF> regionRepository;
        private readonly ILinqRepository<D_ExcCosts_ListPrg> programRepository;
        private readonly IClientService clientRepository;
        private readonly ILinqRepository<D_ExcCosts_Status> statusRepository;
        private readonly ILinqRepository<D_ExcCosts_CharObj> constrMarksRepository;
        private readonly ILinqRepository<D_ExcCosts_AIPMark> marksRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly ILinqRepository<D_ExcCosts_StatusD> statusDRepository;
        private readonly IRepository<D_ExcCosts_Tasks> tasksRepository;

        public EO15AIPCObjectCardController(
            IEO15ExcCostsAIPExtension extension,
            IConstructionService constrRepository,
            ILinqRepository<D_Territory_RF> regionRepository,
            ILinqRepository<D_ExcCosts_ListPrg> programRepository,
            IClientService clientRepository,
            ILinqRepository<D_ExcCosts_Status> statusRepository,
            ILinqRepository<D_ExcCosts_CharObj> constrMarksRepository,
            ILinqRepository<D_ExcCosts_AIPMark> marksRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository,
            ILinqRepository<D_ExcCosts_StatusD> statusDRepository,
            IRepository<D_ExcCosts_Tasks> tasksRepository)
        {
            this.extension = extension;
            this.constrRepository = constrRepository;
            this.regionRepository = regionRepository;
            this.programRepository = programRepository;
            this.clientRepository = clientRepository;
            this.statusRepository = statusRepository;
            this.constrMarksRepository = constrMarksRepository;
            this.marksRepository = marksRepository;
            this.periodRepository = periodRepository;
            this.statusDRepository = statusDRepository;
            this.tasksRepository = tasksRepository;
        }

        /// <summary>
        /// Сохранение объекта строительства
        /// </summary>
        /// <param name="objId">Идентификатор объекта строительства</param>
        /// <param name="values">Реквизиты организации</param>
        /// <returns>Результат сохранения организации</returns>
        [Transaction]
        public AjaxStoreResult Save(int objId, FormCollection values)
        {
            try
            {
                var constrObject = constrRepository.GetOne(objId);
                if (constrObject == null)
                {
                    var lastOrgByCode = constrRepository.FindAll().OrderBy(x => x.Code).LastOrDefault();
                    var code = (lastOrgByCode == null) ? 1 : lastOrgByCode.Code + 1;
                    constrObject = new D_ExcCosts_CObject
                                       {
                                           Login = "0",
                                           RowType = 0,
                                           Code = code,
                                           RefTasks = tasksRepository.Get(-1) 
                                       };
                }

                var regionId = Int32.Parse(values["cbRegion_Value"]);
                constrObject.RefTerritory = regionRepository.FindOne(regionId);
                if (regionId < 1 || constrObject.RefTerritory == null)
                {
                    return GetErrorResult("МО не определено. Изменения не сохранены.");
                }

                var programId = Int32.Parse(values["cbProgram_Value"]);
                constrObject.RefListPrg = programRepository.FindOne(programId);
                if (programId < 1 || constrObject.RefListPrg == null)
                {
                    return GetErrorResult("Целевая программа не определена. Изменения не сохранены.");
                }

                if (User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient))
                {
                    constrObject.RefClients = extension.Client;
                }
                else
                {
                    if (values.AllKeys.Contains("cbClient_Value"))
                    {
                        var clientId = Int32.Parse(values["cbClient_Value"]);
                        constrObject.RefClients = clientRepository.Get(clientId);
                        if (clientId < 1 || constrObject.RefClients == null)
                        {
                            return GetErrorResult("Заказчик не определен. Изменения не сохранены.");
                        }
                    }
                    else
                    {
                        return GetErrorResult("Заказчик не определен. Изменения не сохранены.");
                    }
                }

                if (values["StartConstr"] == null || values["StartConstr"].IsEmpty())
                {
                    return GetErrorResult("Не задан год начала строительства. Изменения не сохранены.");
                }

                if (values["EndConstr"] == null || values["EndConstr"].IsEmpty())
                {
                    return GetErrorResult("Не задан год завершения строительства. Изменения не сохранены.");
                }

                constrObject.Name = values["Name"];
                constrObject.Permit = values["Permit"];
                constrObject.Act = values["Act"];
                constrObject.Power = values["Power"];
                constrObject.Unit = values["Unit"];
                constrObject.DesignEstimates = values["DesignEstimates"];
                constrObject.NormativeDate = values["NormativeDate"];
                constrObject.PlanDate = values["PlanDate"];

                constrObject.StartConstruction = DateTime.Parse("01.01.{0}".FormatWith(values["StartConstr"]));
                constrObject.EndConstruction = DateTime.Parse("31.12.{0}".FormatWith(values["EndConstr"]));

                constrObject.RefStat = statusRepository.FindOne(Int32.Parse(values["cbStatus_Value"]));
                constrObject.RefStatusD = statusDRepository.FindOne(Convert.ToInt32(values["StatusDId"]));
                constrRepository.Save(constrObject);

                var constrMarks = constrMarksRepository.FindAll().Where(x => x.RefCObject.ID == constrObject.ID).ToList();
                var defaultPeriod = periodRepository.Get(-1);
                SaveObjectMark(constrObject, values, constrMarks, AIPMarks.MarkBuildDirection, "BuildDirection", defaultPeriod);
                SaveObjectMark(constrObject, values, constrMarks, AIPMarks.MarkObjectType, "ObjectType", defaultPeriod);
                SaveObjectMark(constrObject, values, constrMarks, AIPMarks.MarkObjectKind, "ObjectKind", defaultPeriod);
                SaveObjectMark(constrObject, values, constrMarks, AIPMarks.MarkReasonsNonProgram, "ReasonsNonProgram", defaultPeriod);

                var result = new AjaxStoreResult
                                 {
                                     ResponseFormat = StoreResponseFormat.Save, 
                                     Data = values,
                                     SaveResponse =
                                         {
                                             Success = true,
                                             Message = constrObject.ID.ToString()
                                         }
                                 };

                return result;
            }
            catch (Exception e)
            {
                return GetErrorResult(e.Message);
            }
        }

        private void SaveObjectMark(
            D_ExcCosts_CObject constrObject, 
            FormCollection values, 
            List<D_ExcCosts_CharObj> constrMarks,
            int markCode, 
            string fieldName, 
            FX_Date_YearDayUNV defaultPeriod)
        {
            var objectMark = constrMarks.FirstOrDefault(x => x.RefAIPMark.Code == markCode);
            if (objectMark == null)
            {
                var mark = marksRepository.FindAll().FirstOrDefault(x => x.Code == markCode);
                objectMark = new D_ExcCosts_CharObj { RefAIPMark = mark, RefCObject = constrObject, RefYearDayUNV = defaultPeriod };  
            }

            objectMark.Value = values[fieldName];
            constrMarksRepository.Save(objectMark);
        }

        private AjaxStoreResult GetErrorResult(string msg)
        {
            return new AjaxStoreResult
                             {
                                 ResponseFormat = StoreResponseFormat.Save,
                                 SaveResponse =
                                     {
                                         Success = false,
                                         Message = msg
                                     }
                             };
        }
    }
}

