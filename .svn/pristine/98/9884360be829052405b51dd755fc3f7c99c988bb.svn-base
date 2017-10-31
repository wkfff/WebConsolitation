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

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controllers.Oiv
{
    public class OivController : SchemeBoundController
    {
        private readonly ILinqRepository<T_Org_Authority> repAuthority;
        private readonly ILinqRepository<D_Territory_RF> repTerritory;
        private readonly IRepository<D_OK_LocalityType> repLocalityType; 

        public OivController(
            ILinqRepository<T_Org_Authority> repAuthority,
            ILinqRepository<D_Territory_RF> repTerritory,
            IRepository<D_OK_LocalityType> repLocalityType)
        {
            this.repAuthority = repAuthority;
            this.repTerritory = repTerritory;
            this.repLocalityType = repLocalityType;
        }
     
        [HttpGet]
        public ActionResult Read(int territoryId)
        {
            var view = from f in repAuthority.FindAll()
                       where f.Territory.ID == territoryId
                       select new
                                  {
                                      f.ID,
                                      f.AuthorityName,
                                      territoryID = f.Territory.ID,
                                      territoryName = f.Territory.Name,
                                      f.LeaderSurname,
                                      f.LeaderFirstName,
                                      f.LeaderPatronymic,
                                      f.LeaderJobPosition,
                                      f.LeaderInformation,
                                      f.OtherName,
                                      f.Telephone,
                                      f.Email,
                                      f.Website,
                                      f.PostalCode,
                                      f.Locality,
                                      LocalityTypeID = f.LocalityType.ID,
                                      f.LocalityType.LocalityTypeName,
                                      f.House,
                                      f.BuildingK,
                                      f.BuildingS
                                  };
            return new AjaxStoreResult(view, view.Count());
        }

        [HttpDelete]
        [Transaction]
        public RestResult Destroy(int id)
        {
            try
            {
                var auth = repAuthority.FindOne(id);
                repAuthority.Delete(auth);
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
                var auth = repAuthority.FindOne(id);
                auth.AuthorityName = fields["AuthorityName"].ToString();
                auth.Territory = repTerritory.FindOne(Convert.ToInt32(fields["territoryID"]));
                auth.OtherName = fields["OtherName"].ToString();
                auth.LeaderSurname = fields["LeaderSurname"].ToString();
                auth.LeaderFirstName = fields["LeaderFirstName"].ToString();
                auth.LeaderPatronymic = fields["LeaderPatronymic"].ToString();
                auth.LeaderJobPosition = fields["LeaderJobPosition"].ToString();
                auth.Telephone = fields["Telephone"].ToString();
                auth.Email = fields["Email"].ToString();
                auth.Website = fields["Website"].ToString();
                auth.PostalCode = fields["PostalCode"].ToString();
                auth.Locality = fields["Locality"].ToString();
                auth.House = fields["House"].ToString();
                auth.BuildingK = fields["BuildingK"].ToString();
                auth.BuildingS = fields["BuildingS"].ToString();
                auth.LocalityType = repLocalityType.Get(Convert.ToInt32(fields["LocalityTypeID"]));

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
                var authority = new T_Org_Authority();

                authority.AuthorityName = parameters["tfAuthorityName"];
                authority.Territory = repTerritory.FindOne(Convert.ToInt32(parameters["territoryID"]));
                authority.OtherName = parameters["tfOtherName"];
                authority.LeaderSurname = parameters["tfLeaderSurname"];
                authority.LeaderFirstName = parameters["tfLeaderFirstName"];
                authority.LeaderPatronymic = parameters["tfLeaderPatronymic"];
                authority.LeaderJobPosition = parameters["tfLeaderJobPosition"];
                authority.Telephone = parameters["tfTelephone"];
                authority.Email = parameters["tfEmail"];
                authority.Website = parameters["tfWebsite"];
                authority.PostalCode = parameters["tfPostalCode"];
                authority.Locality = parameters["tfLocality"];
                authority.House = parameters["tfHouse"];
                authority.BuildingK = parameters["tfBuildingK"];
                authority.BuildingS = parameters["tfBuildingS"];
                authority.LocalityType = repLocalityType.Get(Convert.ToInt32(parameters["cbLocalityType_Value"]));

                repAuthority.Save(authority);

                script.AppendLine("dsOIV.reload();");

                result.Success = true;
                result.ExtraParams["msg"] = "Вставлена новая запись";
                result.Script = script.ToString();
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }      
    }
}
