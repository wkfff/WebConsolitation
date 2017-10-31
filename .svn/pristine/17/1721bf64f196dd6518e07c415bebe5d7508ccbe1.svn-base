using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.ObjectBuilder2;

namespace Krista.FM.RIA.Extensions.Informator.Presentation.Controllers
{
    public class NewsController : SchemeBoundController
    {
        private readonly IRepository<Users> userRepository;
        private readonly ILinqRepository<D_Org_UserProfile> repository;
        private readonly ILinqRepository<D_Org_Structure> tableRepository;
        private readonly ILinqRepository<Memberships> memberships;
        private readonly ILinqRepository<Groups> groups;
        private readonly INewsService newsService;
        private readonly ILinqRepository<Message> messageRepository;

        private readonly ILinqRepository<MessageAttachment> messageAttachmentRepository;

        public NewsController(
                IRepository<Users> userRepository,
                ILinqRepository<MessageAttachment> messageAttachmentRepository,
                ILinqRepository<D_Org_UserProfile> repository,
                ILinqRepository<Memberships> memberships,
                ILinqRepository<Groups> groups,
                INewsService newsService,
                ILinqRepository<Message> messageRepository,
                ILinqRepository<D_Org_Structure> tableRepository)
        {
            this.userRepository = userRepository;
            this.messageAttachmentRepository = messageAttachmentRepository;
            this.repository = repository;
            this.memberships = memberships;
            this.groups = groups;
            this.newsService = newsService;
            this.messageRepository = messageRepository;
            this.tableRepository = tableRepository;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ReadMessage(int messageId)
        {
            return new AjaxStoreResult(newsService.RecieveOneMessage(messageId), 1);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Read([FiltersBinder] FilterConditions filters, string type, int limit = 10000, int start = 0)
        {
            var messages = messageRepository.FindAll().Where(x => x.MessageType == 1 && x.MessageStatus != 3);
            switch (type)
            {
                case "inbox":
                    messages = messages.Where(x => x.RefUserRecipient == User.DbUser.ID);

                    break;
                case "outbox":
                    messages = messages.Where(x => x.RefUserSender == User.DbUser.ID);
                    break;
            }

            filters.Conditions
            .ForEach(filter =>
            {
                var filterValue = filter.Value;
                switch (filter.Name)
                {
                    case "MessageStatus":
                        var values = new List<int>();
                        filter.ValuesList.ForEach(x => values.Add(GetIntStatus(x)));
                        messages = messages.Where(x => values.Contains(x.MessageStatus));
                        break;
                    case "MessageImportance":
                        messages = messages.Where(x => filter.ValuesList.Contains(x.MessageImportance.ToString()));
                        break;
                    case "RefUserRecipient":
                        messages = messages.Where(x => GetOrgName(x.RefUserRecipient).Contains(filterValue));
                        break;
                    case "RefUserSender":
                        messages = messages.Where(x => GetOrgName(x.RefUserSender).Contains(filterValue));
                        break;
                    case "new_message":
                        messages = messages.Where(message => message.MessageStatus == 1);
                        break;
                    case "read_message":
                        messages = messages.Where(message => message.MessageStatus == 2);
                        break;
                    case "exclamation_message":
                        messages = messages.Where(message => message.MessageImportance == 2);
                        break;
                    case "regular_message":
                        messages = messages.Where(message => message.MessageImportance == 3);
                        break;
                }
            });
            var dtomessages = messages.ToList();
            return new AjaxStoreResult(
                dtomessages.Skip(start).Take(limit).Select(message => new MessageView
            {
                ID = message.ID,
                Subject = newsService.GetSubject(message.Subject),
                Body = newsService.GetBody(message.Subject),
                MessageStatus = GetStringMessageStatus((MessageStatus)message.MessageStatus),
                MessageType = (MessageType)message.MessageType,
                MessageImportance = GetStringMessageImportance((MessageImportance)message.MessageImportance),
                RefUserSender = message.RefUserSender.IsNull() ? "Система" : GetOrgName(message.RefUserSender),
                RefUserRecipient = GetOrgName(message.RefUserRecipient),
                RefMessageAttachment = message.RefMessageAttachment != null ? message.RefMessageAttachment.ID : -1,
                MessageAttachment = message.RefMessageAttachment != null,
                ////Document = (message.RefMessageAttachment == null) ? null : message.RefMessageAttachment.Document,
                DocumentFileName = (message.RefMessageAttachment == null) ? null : message.RefMessageAttachment.DocumentFileName,
                ReceivedDate = message.DateTimeOfCreation
            }).ToList(), 
            messages.Count());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendMessage(string subject, string message, string importance, string grops, string singleGrbs, string singlePpos, bool isSendAll, int numberAct, string typeAct, string fileName)
        {
            var result = new AjaxFormResult();

            var regex = new Regex(@"^(https?:)?\/\/.+$");
            subject = regex.Replace(subject, "<a href=" + '"' + "$0" + '"' + ">$0</a>") + "/separator/" + regex.Replace(message, "<a href=" + '"' + "$0" + '"' + ">$0</a>");

            var dateTime = DateTime.Now;
            switch (typeAct)
            {
                case "дней":
                    dateTime += TimeSpan.FromDays(numberAct);
                    break;
                case "часов":
                    dateTime += TimeSpan.FromHours(numberAct);
                    break;
                case "минут":
                    dateTime += TimeSpan.FromMinutes(numberAct);
                    break;
            }

            var ids = new List<int>();

            if (isSendAll)
            {
                ids.AddRange(userRepository.GetAll().Select(x => x.ID));
            }
            else
            {
                if (singleGrbs.IsNotEmpty())
                {
                    var names = singleGrbs.Split(',').Where(x => x != string.Empty).ToList();
                    ids.AddRange(names.Select(Int32.Parse));
                }

                if (singlePpos.IsNotEmpty())
                {
                    var names = singlePpos.Split(',').Where(x => x != string.Empty).ToList();
                    ids.AddRange(names.Where(x => x != string.Empty).Select(Int32.Parse));
                }
            }

            var gps = grops.Split(',').ToList();

            var grbsGroup = groups.FindAll().Single(x => x.Name.Equals("E86N_Provider"));
            var ppoGroup = groups.FindAll().Single(x => x.Name.Equals("E86N_SuperProvider"));

            foreach (var gp in gps)
            {
                switch (gp)
                {
                    case "ГРБС":
                        ids.AddRange(userRepository.GetAll().Where(user => memberships.FindAll().Any(grbs => grbs.RefGroups == grbsGroup && grbs.RefUsers == user)).Select(x => x.ID));
                        break;
                    case "ФО":
                        ids.AddRange(userRepository.GetAll().Where(user => memberships.FindAll().Any(ppo => ppo.RefGroups == ppoGroup && ppo.RefUsers == user)).Select(x => x.ID));
                        break;
                    case "Бюджетные учреждения":
                        var users = repository.FindAll().Where(x => tableRepository.FindAll()
                                                                                   .Where(
                                                                                       ogs =>
                                                                                       ogs.RefTipYc.Name.Equals(
                                                                                           "Бюджетное учреждение")
                                                                                       &&
                                                                                       (ogs.CloseDate == null ||
                                                                                        ogs.CloseDate > DateTime.Now))
                                                                                   .Select(y => y.ID).Contains(x.RefUchr.ID)).Select(y => y.UserLogin)
                                                                                   .ToList();
                        ids.AddRange(userRepository.GetAll().Where(user => users.Contains(user.Name))
                                .Select(x => x.ID));
                        break;
                    case "Автономные учреждения":
                        var users2 = repository.FindAll().Where(x => tableRepository.FindAll()
                                                                                   .Where(
                                                                                       ogs =>
                                                                                       ogs.RefTipYc.Name.Equals(
                                                                                           "Автономное учреждение")
                                                                                       &&
                                                                                       (ogs.CloseDate == null ||
                                                                                        ogs.CloseDate > DateTime.Now))
                                                                                   .Select(y => y.ID).Contains(x.RefUchr.ID)).Select(y => y.UserLogin)
                                                                                   .ToList();
                        ids.AddRange(userRepository.GetAll().Where(user => users2.Contains(user.Name))
                                .Select(x => x.ID));
                        break;
                    case "Казенные учреждения":
                        var users3 = repository.FindAll().Where(x => tableRepository.FindAll()
                                                                                   .Where(
                                                                                       ogs =>
                                                                                       ogs.RefTipYc.Name.Equals(
                                                                                           "Казенное учреждение")
                                                                                       &&
                                                                                       (ogs.CloseDate == null ||
                                                                                        ogs.CloseDate > DateTime.Now))
                                                                                   .Select(y => y.ID).Contains(x.RefUchr.ID)).Select(y => y.UserLogin)
                                                                                   .ToList();
                        ids.AddRange(userRepository.GetAll().Where(user => users3.Contains(user.Name))
                                .Select(x => x.ID));
                        break;
                }
            }

            using (new ServerContext())
            {
                var messageWrapper = new MessageWrapper
                {
                    SendAll = isSendAll,
                    DateTimeOfActual = dateTime,
                    DateTimeOfCreation = DateTime.Now,
                    MessageStatus = MessageStatus.New,
                    MessageType = MessageType.AdministratorMessage,
                    RefUserSender = Scheme.UsersManager.GetCurrentUserID(),
                    Subject = subject,
                };
                if (fileName != string.Empty)
                {
                    var attachment = messageAttachmentRepository.FindAll().First(x => x.DocumentFileName == fileName);
                    messageWrapper.RefMessageAttachment = new MessageAttachmentWrapper
                    {
                        Document = attachment.Document,
                        DocumentFileName = attachment.DocumentFileName,
                        DocumentName = attachment.DocumentName
                    };
                }

                switch (importance)
                {
                    case "Высокая важность":
                        messageWrapper.MessageImportance = MessageImportance.HighImportance;
                        break;
                    case "Важная":
                        messageWrapper.MessageImportance = MessageImportance.Importance;
                        break;
                    case "Обычная":
                        messageWrapper.MessageImportance = MessageImportance.Regular;
                        break;
                    case "Неважная":
                        messageWrapper.MessageImportance = MessageImportance.Unimportant;
                        break;
                }

                if (!isSendAll)
                {
                    foreach (var id in ids.Where(id => id != messageWrapper.RefUserSender))
                    {
                        messageWrapper.RefUserRecipient = id;
                        Scheme.MessageManager.SendMessage(messageWrapper);
                    }
                }
                else
                {
                    Scheme.MessageManager.SendMessage(messageWrapper);
                }
            }

            result.Success = true;
            result.ExtraParams["msg"] = "Сообщение отправлено";
            result.IsUpload = true;
            return result;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteMessage(string id)
        {
            var result = new AjaxFormResult();
            try
            {
                var strIds = id.Split(',').Where(x => x != string.Empty).ToList();

                var ids = strIds.Select(Int32.Parse).ToList();

                var messageManager = Scheme.MessageManager;
                using (new ServerContext())
                {
                    foreach (var strId in ids)
                    {
                        var fileNames =
                            messageRepository.FindAll().Where(x => x.ID == strId).Select(
                                x => x.RefMessageAttachment.DocumentFileName);
                        if (fileNames.Any())
                        {
                            var attachments =
                                messageAttachmentRepository.FindAll().Where(x => x.DocumentFileName == fileNames.First())
                                    .ToList();
                            if (attachments.Count != 0)
                            {
                                if (
                                    messageRepository.FindAll().Count(x => x.RefMessageAttachment.DocumentFileName == fileNames.First() &&
                                                                           x.MessageStatus != 3) == 1)
                                {
                                    var localPath = ConfigurationManager.AppSettings["DocFilesSavePath"] + "\\" + "news" +
                                                    "\\";
                                    if (System.IO.File.Exists(localPath + fileNames.First()))
                                    {
                                        System.IO.File.Delete(localPath + fileNames.First());
                                    }
                                }
                            }
                        }

                        messageManager.DeleteMessage(strId);
                    }
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Новости удалены";
                result.IsUpload = true;
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }

        // todo
        [HttpPost]
        [Core.NHibernate.Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult Upload(string fileName)
        {
            var result = new AjaxFormResult();
            var file = Request.Files[0];
            try
            {
                var localFileName = "news" + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + fileName;

                var content = new byte[file.ContentLength];
                file.InputStream.Read(content, 0, file.ContentLength);

                var messageAttachment = new MessageAttachment
                {
                    DocumentFileName = localFileName,
                    DocumentName = fileName,
                    Document = content
                };

                if (!newsService.UploadAttachment(messageAttachment))
                {
                    throw new Exception();
                }

                result.Success = true;
                result.ExtraParams["msg"] = localFileName;
                result.IsUpload = true;
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка передачи файла";
                result.ExtraParams["responseText"] = "Ошибка передачи файла: " + e.Message;
                result.IsUpload = true;
                return result;
            }
        }

        public ActionResult Download(int attachment)
        {
            var content = messageAttachmentRepository.FindAll().Where(x => x.ID == attachment).Select(x => new { x.Document, x.DocumentFileName }).First();

            return File(content.Document, "application/octet-stream", content.DocumentFileName);
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GetGrbsAdressBook([FiltersBinder] FilterConditions filters)
        {
            var grbsGroup = groups.FindAll().Single(
                x => x.Name.Equals("E86N_Provider"));
            var grbs = userRepository.GetAll().Where(
                user => memberships.FindAll().Any(
                    g => g.RefGroups == grbsGroup && g.RefUsers == user)).Select(
                    x => new { x.ID, x.Name, INN = String.Empty }).ToList();

            grbs = grbs.Join(
                repository.FindAll(),
                user => user.Name,
                rep => rep.UserLogin,
                (user, rep) => new
                {
                    user.ID,
                    rep.RefUchr.Name,
                    rep.RefUchr.INN
                }).ToList();
            foreach (var filter in filters.Conditions)
            {
                var filterValue = filter.Value;
                switch (filter.Name)
                {
                    case "Name":
                        grbs = grbs.Where(x => (x.Name.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) != -1) || (x.INN.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) != -1)).ToList();
                        break;
                }
            }

            return new AjaxStoreResult(grbs, grbs.Count());
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GetPpoAdressBook([FiltersBinder] FilterConditions filters)
        {
            var ppoGroup = groups.FindAll().Single(x => x.Name.Equals("E86N_SuperProvider"));

            var ppos = userRepository.GetAll().Where(user => memberships.FindAll().Any(ppo => ppo.RefGroups == ppoGroup && ppo.RefUsers == user)).Select(x => new { ID = x.ID, Name = x.Name, INN = String.Empty }).ToList();

            ppos = ppos.Join(
                repository.FindAll(),
                user => user.Name,
                rep => rep.UserLogin,
                (user, rep) => new
                {
                    user.ID,
                    rep.RefUchr.Name,
                    rep.RefUchr.INN
                }).ToList();

            foreach (var filter in filters.Conditions)
            {
                var filterValue = filter.Value;
                switch (filter.Name)
                {
                    case "Name":
                        ppos = ppos.Where(x => (x.Name.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) != -1) || (x.INN.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) != -1)).ToList();
                        break;
                }
            }

            return new AjaxStoreResult(ppos, ppos.Count());
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GetImportance()
        {
            IList<string> importances = new List<string> { "Важная", "Обычная" };
            var imps = importances.AsEnumerable().Select(x => new { Name = x }).ToList();
            return new AjaxStoreResult(imps, imps.Count);
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GetTopicality()
        {
            IList<string> topicalitys = new List<string> { "без ограничения", "дней", "часов", "минут" };
            return new AjaxStoreResult(topicalitys, topicalitys.Count);
        }

        public virtual RestResult Update(string data)
        {
            try
            {
                var dataUpdate = JSON.Deserialize<JsonObject>(data);
                Scheme.MessageManager.UpdateMessage(Convert.ToInt32(dataUpdate["ID"]), MessageStatus.Read);

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        private static string GetStringMessageImportance(MessageImportance messageImportance)
        {
            var status = string.Empty;
            switch (messageImportance)
            {
                case MessageImportance.HighImportance:
                    status = "Высокая важность";
                    break;
                case MessageImportance.Importance:
                    status = "Важная";
                    break;
                case MessageImportance.Regular:
                    status = "Обычная";
                    break;
                case MessageImportance.Unimportant:
                    status = "Неважная";
                    break;
            }

            return status;
        }

        private static string GetStringMessageStatus(MessageStatus messageStatus)
        {
            var status = string.Empty;
            switch (messageStatus)
            {
                case MessageStatus.Read:
                    status = "Прочитана";
                    break;
                case MessageStatus.New:
                    status = "Новая";
                    break;
                case MessageStatus.Deleted:
                    status = "Удалена";
                    break;
            }

            return status;
        }

        private static int GetIntStatus(string status)
        {
            switch (status)
            {
                case "Прочитана":
                    return 2;
                case "Новая":
                    return 1;
                case "Удалена":
                    return 3;
            }

            return 2;
        }

        private string GetOrgName(int? refUserId)
        {
            var name = userRepository.Get((int)refUserId).Name;
            try
            {
                return repository.FindAll().Where(x => x.UserLogin == name).Select(x => x.RefUchr.Name).First();
            }
            catch (Exception)
            {
                return name;
            }
        }
    }
}