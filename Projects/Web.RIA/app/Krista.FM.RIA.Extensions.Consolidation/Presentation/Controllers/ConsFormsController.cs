using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsFormsController : SchemeBoundController
    {
        private readonly ILinqRepository<D_CD_Templates> templatesRepository;

        public ConsFormsController(ILinqRepository<D_CD_Templates> templatesRepository)
        {
            this.templatesRepository = templatesRepository;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Create(string data)
        {
            try
            {
                D_CD_Templates form = JavaScriptDomainConverter<D_CD_Templates>.DeserializeSingle(data);

                form.ID = 0;
                form.Class = "123";
                templatesRepository.Save(form);
                templatesRepository.DbContext.CommitChanges();

                return new RestResult
                {
                    Success = true,
                    Message = "Новая запись добавлена",
                    Data = from f in templatesRepository.FindAll()
                           where f.ID == form.ID
                           select new
                                      {
                                          f.ID,
                                          f.Name,
                                          f.ShortName,
                                          f.NameCD,
                                          f.Code,
                                          f.InternalName,
                                          f.FormVersion
                                      }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult Read()
        {
            try
            {
                return new RestResult
                {
                    Success = true,
                    Data = (from f in templatesRepository.FindAll()
                           select new
                           {
                               f.ID,
                               f.Name,
                               f.ShortName,
                               f.NameCD,
                               f.Code,
                               f.InternalName,
                               f.FormVersion,
                               f.Status
                           }).ToList()
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Update(int id, string data)
        {
            try
            {
                JsonObject fields = JSON.Deserialize<JsonObject>(data);

                var template = templatesRepository.FindOne(id);

                template.Name = fields["Name"].ToString();
                template.ShortName = fields["ShortName"].ToString();
                template.NameCD = fields["NameCD"].ToString();
                template.Code = fields["Code"].ToString();
                template.InternalName = fields["InternalName"].ToString();

                templatesRepository.Save(template);
                templatesRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [Transaction]
        public RestResult Destroy(int id)
        {
            try
            {
                var reglament = templatesRepository.FindOne(id);
                templatesRepository.Delete(reglament);
                templatesRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
