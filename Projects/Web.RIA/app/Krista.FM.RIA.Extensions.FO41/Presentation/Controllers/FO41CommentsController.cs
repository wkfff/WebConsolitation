using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41CommentsController : SchemeBoundController
    {
        private readonly ILinqRepository<T_Note_ApplicationOGV> commentsOGVRepository;
        private readonly ILinqRepository<T_Note_ApplicationOrg> commentsOrgVRepository;
        private readonly AppPrivilegeService requestsOrgVRepository;
        private readonly AppFromOGVService requestsOGVRepository;

        public FO41CommentsController(
            ILinqRepository<T_Note_ApplicationOGV> commentsOGVRepository, 
            AppFromOGVService requestsOGVRepository,
            AppPrivilegeService requestsOrgVRepository,
            ILinqRepository<T_Note_ApplicationOrg> commentsOrgVRepository)
        {
            this.commentsOGVRepository = commentsOGVRepository;
            this.commentsOrgVRepository = commentsOrgVRepository;
            this.requestsOrgVRepository = requestsOrgVRepository;
            this.requestsOGVRepository = requestsOGVRepository;
        }

        /// <summary>
        /// Получение комментариев от ОГВ для заявки от ОГВ
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от ОГВ</param>
        /// <returns>Комментарии от ОГВ</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult OGVRead(int applicationId)
        {
            var data = from c in commentsOGVRepository.FindAll().Where(c => c.RefApplicOGV.ID == applicationId).ToList()
                       select new
                                  {
                                      c.ID,
                                      c.Executor,
                                      c.Text,
                                      StateName = c.RefStateOGV.Name,
                                      NoteDate = c.NoteDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"))
                                  };
            return new AjaxStoreResult(data);
        }

        /// <summary>
        /// Сохранение комментариев от ОГВ
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от ОГВ</param>
        /// <param name="data">измененные данные</param>
        /// <returns>Обновленные комментарии</returns>
        [AcceptVerbs(HttpVerbs.Put)]
        [Transaction]
        public ActionResult OGVSave(int applicationId, string data)
        {
            try
            {
                var app = requestsOGVRepository.Get(applicationId);
                var dataSet = JSON.Deserialize<JsonObject>(data);
                var record = new T_Note_ApplicationOGV
                                 {
                                     Executor = dataSet["Executor"].ToString(),
                                     Text = dataSet["Text"].ToString(),
                                     NoteDate = DateTime.Parse(dataSet["NoteDate"].ToString()),
                                     RefApplicOGV = app,
                                     RefStateOGV = app.RefStateOGV
                                 };
                var id = Convert.ToInt32(dataSet["ID"]);
                if (id > -1)
                {
                    record.ID = id;
                }

                commentsOGVRepository.Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = "Комментарии обновлены",
                    Data = new
                    {
                        record.ID,
                        record.NoteDate,
                        record.Text,
                        StateName = record.RefStateOGV.Name,
                        record.Executor,
                        RefApplicOGV = record.RefApplicOGV.ID
                    }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
        
        /// <summary>
        /// Удаление комментария от ОГВ
        /// </summary>
        /// <param name="id">Идентификатор комментария</param>
        /// <returns>Обновленные комментарии</returns>
        [Transaction]
        public ActionResult OGVDestroy(int id)
        {
            try
            {
                var comment = commentsOGVRepository.FindOne(id);
                if (comment != null)
                {
                    commentsOGVRepository.Delete(comment);
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Комментарий удален",
                    Data = new { }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Получение комментариев от налогоплательщиков по заявке от налогоплательщиков
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от налогоплательщика</param>
        /// <returns>Комментарии от налогоплательщика</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult OrgRead(int applicationId)
        {
            var data = from c in commentsOrgVRepository.FindAll().Where(c => c.RefApplicOrg.ID == applicationId).ToList()
                       select new
                       {
                           c.ID,
                           c.Executor,
                           c.Text,
                           StateName = c.RefStateOrg.Name,
                           NoteDate = c.NoteDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"))
                       };
            return new AjaxStoreResult(data);
        }

        /// <summary>
        /// Удаление комментария от налогоплательщика
        /// </summary>
        /// <param name="id">Идентификатор комментария</param>
        /// <returns>Обновленные комментарии</returns>
        [Transaction]
        public ActionResult OrgDestroy(int id)
        {
            try
            {
                var comment = commentsOrgVRepository.FindOne(id);
                if (comment != null)
                {
                    commentsOrgVRepository.Delete(comment);
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Комментарий удален",
                    Data = new { }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Сохранение комментариев от налогоплательщиков
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от налогоплательщика</param>
        /// <param name="data">Измененные данные</param>
        /// <returns>Обновленные комментарии</returns>
        [AcceptVerbs(HttpVerbs.Put)]
        [Transaction]
        public ActionResult OrgSave(int applicationId, string data)
        {
            try
            {
                var app = requestsOrgVRepository.Get(applicationId);
                var dataSet = JSON.Deserialize<JsonObject>(data);
                var record = new T_Note_ApplicationOrg
                {
                    Executor = dataSet["Executor"].ToString(),
                    Text = dataSet["Text"].ToString(),
                    NoteDate = DateTime.Parse(dataSet["NoteDate"].ToString()),
                    RefApplicOrg = app,
                    RefStateOrg = app.RefStateOrg
                };
                var id = Convert.ToInt32(dataSet["ID"]);
                if (id > -1)
                {
                    record.ID = id;
                }

                commentsOrgVRepository.Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = "Комментарии обновлены",
                    Data = new
                    {
                        record.ID,
                        record.NoteDate,
                        record.Text,
                        StateName = record.RefStateOrg.Name,
                        record.Executor,
                        RefApplicOrg = record.RefApplicOrg.ID
                    }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
