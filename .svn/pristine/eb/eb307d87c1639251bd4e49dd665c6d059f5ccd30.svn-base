using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controllers.Subject
{
    public class TerritoryController : SchemeBoundController
    {
        private readonly ILinqRepository<D_Territory_RF> repTerritory;
        private readonly ILinqRepository<FX_FX_TerritorialPartitionType> repTypeTerritory; 
        private readonly IMinSportExtension extension;
        private readonly ILinqRepository<B_Regions_Bridge> repRegionsBridge;
        private readonly ILinqRepository<B_Regions_BridgePlan> repRegionsBridgePlan;
        private readonly ILinqRepository<B_Territory_RFBridge> repTerritoryRfBridge;

        public TerritoryController(
            ILinqRepository<D_Territory_RF> repTerritory, 
            IMinSportExtension extension,
            ILinqRepository<FX_FX_TerritorialPartitionType> repTypeTerritory,
            ILinqRepository<T_People_Population> repPeoplePopulation,
            ILinqRepository<B_Regions_Bridge> repRegionsBridge,
            ILinqRepository<B_Regions_BridgePlan> repRegionsBridgePlan,
            ILinqRepository<B_Territory_RFBridge> repTerritoryRfBridge)
        {
            this.repTerritory = repTerritory;
            this.extension = extension;
            this.repTypeTerritory = repTypeTerritory;
            this.repRegionsBridge = repRegionsBridge;
            this.repRegionsBridgePlan = repRegionsBridgePlan;
            this.repTerritoryRfBridge = repTerritoryRfBridge;
        }

        public ActionResult Read(int territoryId)
        {
                var view = from f in repTerritory.FindAll()
                           where (f.RefTerritorialPartType.ID == 4 || f.RefTerritorialPartType.ID == 5) && (f.ParentID == territoryId) 
                           orderby f.Name
                           select new
                           {
                                          f.ID,
                                          f.Name,
                                          f.ParentID,
                                          f.ShortName,
                                          TypeID = f.RefTerritorialPartType.ID,
                                          TypeName = f.RefTerritorialPartType.Name
                                      };
                return new AjaxStoreResult(view, view.Count());   
        }

        public ActionResult LoadComboBox()
        {
            if (extension.CurrUser.RefRegion == null)
            {
                var list = (from f in repTerritory.FindAll()
                            where f.RefTerritorialPartType.ID == 3 
                            orderby f.Name
                            select new
                            {
                                           Value = f.ID,
                                           Text = f.Name
                                       }).ToList();
                return new AjaxStoreResult(list, list.Count);
            }
            else
            {
                var list = (from f in repTerritory.FindAll()
                            where f.RefTerritorialPartType.ID == 3 && f.ID == extension.CurrUser.RefRegion 
                            orderby f.Name
                            select new
                            {
                                Value = f.ID,
                                Text = f.Name
                            }).ToList();
                return new AjaxStoreResult(list, list.Count);
            }    
        }

        [HttpDelete]
        [Transaction]
        public RestResult Destroy(int id)
        {
            try
            {
                var terr = repTerritory.FindOne(id);
                repTerritory.Delete(terr);

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPut]
        [Transaction]
        public RestResult Update(int id, string data)
        {
            try
            {
                var fields = JSON.Deserialize<JsonObject>(data);
                var terr = repTerritory.FindOne(id);
                terr.Name = fields["Name"].ToString();
                terr.ShortName = fields["ShortName"].ToString();
                terr.RefTerritorialPartType = repTypeTerritory.FindOne(Convert.ToInt32(fields["TypeID"]));
                terr.ParentID = Convert.ToInt32(fields["ParentID"]);

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult Create(FormCollection parameters)
        {
            var result = new AjaxFormResult();
            var script = new StringBuilder();
            try
            {
                var newTerritory = new D_Territory_RF();
                newTerritory.Name = parameters["tfName"];
                newTerritory.ShortName = parameters["tfShortName"];
                newTerritory.RefTerritorialPartType = repTypeTerritory.FindOne(Convert.ToInt32(parameters["cbTypeTerritory_Value"]));
                newTerritory.RefRegionsBridge = repRegionsBridge.FindOne(-1);
                newTerritory.RefBridgeRegionsPlan = repRegionsBridgePlan.FindOne(-1);
                newTerritory.RefBridge = repTerritoryRfBridge.FindOne(-1);
                newTerritory.OKATO = Convert.ToString(0);
                newTerritory.ParentID = Convert.ToInt32(Convert.ToInt32(parameters["cbSubjects_Value"]));
                repTerritory.Save(newTerritory);

                script.AppendLine("dsTerritory.reload();");
                
                result.Success = true;
                result.ExtraParams["msg"] = "Вставлена новая запись";
                result.Script = script.ToString();
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка сохранения.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }
    }
}
