using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views;
using Krista.FM.RIA.Extensions.FO51PassportMO.Services;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Controllers
{
    public class CheckPassportMOController : SchemeBoundController
    {
        private readonly IFO51Extension extension;
        private readonly IFactsPassportMOService factsService;
        private readonly ILinqRepository<D_Marks_PassportMO> marksRepository; 
        private readonly ILinqRepository<D_Regions_Analysis> regionAnalysRepository; 
        private readonly ILinqRepository<FX_State_PassportMO> stateRepository;
        
        public CheckPassportMOController(
            IFO51Extension extension,
            ILinqRepository<FX_State_PassportMO> stateRepository,
            ILinqRepository<D_Regions_Analysis> regionAnalysRepository,
            ILinqRepository<D_Marks_PassportMO> marksRepository,
            IFactsPassportMOService factsService)
        {
            this.extension = extension;
            this.stateRepository = stateRepository;
            this.regionAnalysRepository = regionAnalysRepository;
            this.factsService = factsService;
            this.marksRepository = marksRepository;
        }

        /// <summary>
        /// Чтение данных по МО
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список данных по МО</returns>
        public AjaxStoreExtraResult Read(int periodId)
        {
            try
            {
                var sourceFacts = extension.DataSourcesFO51
                    .FirstOrDefault(x => x.Year.Equals((periodId / 10000).ToString()));

                var regions = extension.GetRegions(periodId);
                var fictRegion = extension.GetRegionByBridge(periodId, FO51Extension.RegionFictID);
                if (fictRegion != null)
                {
                    regions.Add(fictRegion);
                }

                var data = new List<CheckMoModel>();
               
                var acceptedAll = true;
                
                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        if (region == null)
                        {
                            throw new Exception("Сопоставимый регион не найден");
                        }

                        var stateId = factsService.GetStateId(periodId, region.ID, sourceFacts.ID);
                        var state = stateRepository.FindOne(stateId);
                        acceptedAll = acceptedAll && stateId == FO51Extension.StateAccept;
                        var model = new CheckMoModel
                                        {
                                            MoName = region.Name,
                                            ID = region.ID,
                                            StatusID = state.ID,
                                            StatusName = state.Name
                                        };
                        data.Add(model);
                    }
                }

                data.Add(new CheckMoModel
                             {
                                 MoName = "Всего по ГО", 
                                 ID = FO51Extension.RegionsGO, 
                                 StatusID = 1,  
                                 StatusName = String.Empty
                             });
                data.Add(new CheckMoModel
                             {
                                 MoName = "Всего по МР", 
                                 ID = FO51Extension.RegionsMR, 
                                 StatusID = 1, 
                                 StatusName = String.Empty
                             });
                data.Add(new CheckMoModel
                             {
                                 MoName = "Всего по МР и ГО", 
                                 ID = FO51Extension.RegionsAll, 
                                 StatusID = 1, 
                                 StatusName = String.Empty
                             });

                return new AjaxStoreExtraResult(data, data.Count, acceptedAll)
                {
                    ResponseFormat = StoreResponseFormat.Load,
                    Data = data,
                    Total = data.Count()
                };
            }
            catch (Exception)
            {
                return new AjaxStoreExtraResult(new List<string>(), 0, false)
                {
                    ResponseFormat = StoreResponseFormat.Load,
                    Data = new List<string>(),
                    Total = 0
                };
            }
        }

        public ActionResult ShowRequest(int periodId, int markId, int regionId)
        {
            var view = new EditPassportOGV(
                extension, 
                regionAnalysRepository, 
                marksRepository, 
                extension.GetActualRegion(periodId, regionId), 
                periodId);
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", view);
        }

        private class CheckMoModel
        {
            /// <summary>
            /// Наименование территории
            /// </summary>
            public string MoName { get; set; }

            /// <summary>
            /// Идентификуатор региона
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// Идентификатор статуса
            /// </summary>
            public int StatusID { get; set; }

            /// <summary>
            /// Наименование статуса
            /// </summary>
            public string StatusName { get; set; }
        }
    }
}
