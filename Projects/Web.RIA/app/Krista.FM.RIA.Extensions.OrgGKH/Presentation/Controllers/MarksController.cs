using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.OrgGKH.Params;
using Krista.FM.RIA.Extensions.OrgGKH.Presentation.Models;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Controllers
{
    public class MarksController : SchemeBoundController
    {
        private readonly IOrgGkhExtension extension;
        private readonly ILinqRepository<F_Org_GKH> markFactsRepository;
        private readonly IRepository<D_Regions_Analysis> regionsRepository;
        private readonly IRepository<D_Org_RegistrOrg> orgRepository;
        private readonly ILinqRepository<D_Org_MarksGKH> marksRepository;
        private readonly ILinqRepository<FX_Date_YearDayUNV> periodsRepository;
        private readonly FX_Org_StatusD status;

        public MarksController(
            IOrgGkhExtension extension,
            ILinqRepository<F_Org_GKH> markFactsRepository,
            IRepository<D_Regions_Analysis> regionsRepository,
            IRepository<D_Org_RegistrOrg> orgRepository,
            ILinqRepository<D_Org_MarksGKH> marksRepository,
            ILinqRepository<FX_Date_YearDayUNV> periodsRepository,
            ILinqRepository<FX_Org_StatusD> statusDRepository)
        {
            this.extension = extension;
            this.markFactsRepository = markFactsRepository;
            this.regionsRepository = regionsRepository;
            this.orgRepository = orgRepository;
            this.marksRepository = marksRepository;
            this.periodsRepository = periodsRepository;
            status = statusDRepository.FindOne(OrgGKHConsts.StateLocked);
        }

        /// <summary>
        /// Чтение данных
        /// </summary>
        public ActionResult Read(int regionId, int periodId)
        {
            try
            {
                // список территорий - МО, относящееся к МР или ГО, выбранному в параметре «Территория» 
                // (либо ссылка на сам выбранный МР или ГО)
                var regions =
                    regionsRepository.GetAll().Where(x => x.ID == regionId || x.ParentID == regionId)
                        .Select(x => x.ID);

                // выбор из классификатора: «Организации.Реестр организаций (d.Org.RegistrOrg)» тех записей, 
                // в которых в поле «Районы.Анализ» стоит ссылка выбранную территорию
                var orgs = orgRepository.GetAll()
                    .Where(x => regions.Contains(x.RefRegionAn.ID)).OrderBy(x => x.Code).ToList();

                var result = new List<MarksModel>();

                var startPeriod = ((periodId / 10000) * 10000) - 1;

                var i = 1;
                foreach (var org in orgs)
                {
                    var mark50000 = markFactsRepository.FindAll()
                        .OrderByDescending(x => x.RefYearDayUNV.ID).
                        FirstOrDefault(x => 
                            x.RefYearDayUNV.ID > startPeriod && 
                            x.RefYearDayUNV.ID <= periodId && 
                            x.RefRegistrOrg.ID == org.ID && 
                            x.RefMarksGKH.Code == 50000);
                    var mark60000 = markFactsRepository.FindAll()
                        .OrderByDescending(x => x.RefYearDayUNV.ID)
                        .FirstOrDefault(x => 
                            x.RefYearDayUNV.ID > startPeriod && 
                            x.RefYearDayUNV.ID <= periodId && 
                            x.RefRegistrOrg.ID == org.ID && 
                            x.RefMarksGKH.Code == 60000);
                    var mark = new MarksModel
                    {
                        NameOrg = org.NameOrg,
                        OrgId = org.ID,
                        INN = org.INN,
                        RegionName = org.RefRegionAn == null ? string.Empty : org.RefRegionAn.Name,
                        Number = i
                    };
                    i++;

                    if (mark50000 == null)
                    {
                        mark.Mark50000Id = -1;
                        mark.Mark50000Value = null;
                    }
                    else
                    {
                        mark.Mark50000Id = (mark50000.RefYearDayUNV.ID == periodId)
                                               ? mark50000.ID
                                               : -1;

                        mark.Mark50000Value = mark50000.Value;
                    }

                    if (mark60000 == null)
                    {
                        mark.Mark60000Id = -1;
                        mark.Mark60000Value = null;
                    }
                    else
                    {
                        mark.Mark60000Id = (mark60000.RefYearDayUNV.ID == periodId)
                                               ? mark60000.ID
                                               : -1;
                        mark.Mark60000Value = mark60000.Value;
                    }

                    result.Add(mark);
                }

                return new AjaxStoreResult(result, result.Count);
            }
            catch (Exception)
            {
                return new AjaxStoreResult(new List<string>(), 0);
            }
        }

        [Transaction]
        public ActionResult Save(int regionId, int periodId)
        {
            try
            {
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        SaveMarkFact(record, periodId, 50000);
                        SaveMarkFact(record, periodId, 60000);
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Показатели сохранены",
                    Data = dataSet.ToString()
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [Transaction]
        private void SaveMarkFact(IDictionary<string, object> record, int periodId, int code)
        {
            object idMark;
            record.TryGetValue("Mark{0}Id".FormatWith(code), out idMark);
            if (idMark != null)
            {
                var id = Int32.Parse(idMark.ToString());
                F_Org_GKH fact;
                if (id > 0)
                {
                    fact = markFactsRepository.FindOne(id);
                }
                else
                {
                    fact = new F_Org_GKH
                    {
                        RefMarksGKH = marksRepository.FindAll().FirstOrDefault(x => x.Code == code),
                        RefYearDayUNV = periodsRepository.FindOne(periodId),
                        RefRegistrOrg = orgRepository.Get(Convert.ToInt32(record["OrgId"])),
                        RefStatusD = status,
                        SourceID = extension.DataSource.ID
                    };
                }

                var value = record["Mark{0}Value".FormatWith(code)];
                if (value == null || value.Equals(string.Empty))
                {
                    fact.Value = null;
                }
                else
                {
                    fact.Value = Convert.ToDecimal(value);
                }

                markFactsRepository.Save(fact);
            }
        }
    }
}