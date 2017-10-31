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
    public class PersonCuratorController : SchemeBoundController
    {
        private readonly ILinqRepository<T_Person_Curator> repPersonCurator;
        private readonly ILinqRepository<D_Territory_RF> repTerritory;

        public PersonCuratorController(
            ILinqRepository<D_Territory_RF> repTerritory,
            ILinqRepository<T_Person_Curator> repPersonCurator)
        {
            this.repTerritory = repTerritory;
            this.repPersonCurator = repPersonCurator;
        }

        [HttpGet]
        public ActionResult Read(int territoryId)
        {
            var view = from f in repPersonCurator.FindAll()
                       where f.Territory.ID == territoryId
                       select new
                                  {
                                      f.ID,
                                      f.FirstName,
                                      f.Patronimyc,
                                      f.Surname,
                                      f.JobPosition,
                                      f.Telephone,
                                      f.Email,
                                      territoryID = f.Territory.ID,
                                      territoryName = f.Territory.Name
                                  };
            return new AjaxStoreResult(view, view.Count());
        }

        [HttpPut]
        public RestResult Update(int id, string data)
        {
            try
            {
                JsonObject fields = JSON.Deserialize<JsonObject>(data);
                var pc = repPersonCurator.FindOne(id);
                pc.FirstName = fields["FirstName"].ToString();
                pc.Surname = fields["Surname"].ToString();
                pc.Patronimyc = fields["Patronimyc"].ToString();
                pc.JobPosition = fields["JobPosition"].ToString();
                pc.Telephone = fields["Telephone"].ToString();
                pc.Email = fields["Email"].ToString();
                pc.Territory = repTerritory.FindOne(Convert.ToInt32(fields["territoryID"]));
                repPersonCurator.DbContext.CommitChanges();

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

                var personCurator = new T_Person_Curator();
                personCurator.Surname = parameters["tfSurname"];
                personCurator.FirstName = parameters["tfFirstName"];
                personCurator.Patronimyc = parameters["tfPatronimyc"];
                personCurator.Email = parameters["tfEmail"];
                personCurator.JobPosition = parameters["tfJobPosition"];
                personCurator.Telephone = parameters["tfTelephone"];
                personCurator.Territory = repTerritory.FindOne(Convert.ToInt32(parameters["cbTerritory_Value"]));
                repPersonCurator.Save(personCurator);

                script.AppendLine("dsPersonCurator.reload();");

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

        [HttpDelete]
        [Transaction]
        public RestResult Destroy(int id)
        {
            var result = new RestResult();
            try
            {
                var personCurator = repPersonCurator.FindOne(id);
                repPersonCurator.Delete(personCurator);
                return new RestResult { Success = true, Message = "Запись удалена" };
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
