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

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controllers.Propaganda
{
    public class InternetController : SchemeBoundController
    {
        private readonly ILinqRepository<T_MassMedia_List> repMassMedia;
        private readonly ILinqRepository<D_Territory_RF> repTerritory;
        private readonly IRepository<D_MassMedia_Kind> repMassMediaKind;
        private readonly IRepository<D_KD_TargetGroup> repTargetGroup;

        public InternetController(
            ILinqRepository<T_MassMedia_List> repMassMedia,
            ILinqRepository<D_Territory_RF> repTerritory,
            IRepository<D_MassMedia_Kind> repMassMediaKind,
            IRepository<D_KD_TargetGroup> repTargetGroup)
        {
            this.repTerritory = repTerritory;
            this.repMassMedia = repMassMedia;
            this.repMassMediaKind = repMassMediaKind;
            this.repTargetGroup = repTargetGroup;
        }
         
        [HttpGet]
        public ActionResult Read(int territoryId, int beginCode, int endCode)
        {
            var view = from f in repMassMedia.FindAll()
                       where f.Territory.ParentID == territoryId && f.Kind.Code >= beginCode && f.Kind.Code <= endCode
                       select new
                                  {
                                      f.ID,
                                      f.MediaName,
                                      f.OtherName,
                                      TerritoryID = f.Territory.ID,
                                      TerritoryName = f.Territory.Name,
                                      KindID = f.Kind.ID,
                                      f.Kind.KindName,
                                      TargetGroupID = f.TargetGroup.ID,
                                      TargetGroupName = f.TargetGroup.OtherName,
                                      f.Website,
                                      f.RubricName,
                                      f.Placement
                                  };
            return new AjaxStoreResult(view, view.Count());
        }

        [HttpPut]
        [Transaction]
        public RestResult Update(int id, string data)
        {
            try
            {
                var fields = JSON.Deserialize<JsonObject>(data);
                var obj = repMassMedia.FindOne(id);
                obj.MediaName = fields["MediaName"].ToString();
                obj.OtherName = fields["OtherName"].ToString();
                obj.Territory = repTerritory.FindOne(Convert.ToInt32(fields["TerritoryID"]));
                obj.Kind = repMassMediaKind.Get(Convert.ToInt32(fields["KindID"]));
                obj.TargetGroup = repTargetGroup.Get(Convert.ToInt32(fields["TargetGroupID"]));

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
            try
            {
                var script = new StringBuilder();
                var newMassMedia = new T_MassMedia_List();
                newMassMedia.MediaName = parameters["tfMediaName"];
                newMassMedia.OtherName = parameters["tfOtherName"];
                newMassMedia.Website = parameters["tfWebsite"];
                newMassMedia.RubricName = parameters["tfRubricName"];
                newMassMedia.Placement = parameters["tfPlacement"];
                newMassMedia.Territory = repTerritory.FindOne(Convert.ToInt32(parameters["cbTerritoryID_Value"]));
                newMassMedia.Kind = repMassMediaKind.Get(Convert.ToInt32(parameters["cbKindID_Value"]));
                newMassMedia.TargetGroup = repTargetGroup.Get(Convert.ToInt32(parameters["cbTargetGroupID_Value"]));
                repMassMedia.Save(newMassMedia);
                
                script.AppendLine("dsInternet.reload();");

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

        [HttpDelete]
        [Transaction]
        public RestResult Destroy(int id)
        {
            var result = new RestResult();
            try
            {
                var obj = repMassMedia.FindOne(id);
                repMassMedia.Delete(obj);
                result.Success = true;
                result.Message = "Запись удалена! ";
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = String.Format("Ошибка удаления: {0}", e.Message);
                return result;
            }
        }    
    }
}
