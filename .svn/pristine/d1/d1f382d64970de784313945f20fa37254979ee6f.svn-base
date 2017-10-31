using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.Messages.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Messages.Presentation.Controllers
{
    public class MessagesNavController : SchemeBoundController
    {
        private readonly IMessageService messageService;
        private readonly ILinqRepository<MessageAttachment> messageAttachmentRepository;

        public MessagesNavController(
            IMessageService messageService,
            ILinqRepository<MessageAttachment> messageAttachmentRepository)
        {
            this.messageService = messageService;
            this.messageAttachmentRepository = messageAttachmentRepository;
        }

        public AjaxStoreResult ReciveMessages(int limit, int start)
        {
            var page = start == 0 ? 1 : (start / limit) + 1;
            var msgs = messageService.ReciveMessages(User.DbUser.ID, page, limit);

              var list = (from f in msgs
                         select new
                         {
                             f.ID,
                             Sender = f.RefUserSender,
                             Date = f.ReceivedDate,
                             Status = Convert.ToInt32(f.MessageStatus),
                             Importance = Convert.ToInt32(f.MessageImportance),
                             Subject = !String.IsNullOrEmpty(f.Body) ? f.Body : f.Subject,
                             MessageType = GetMessageType(f.MessageType),
                             DateGroup = CalcDateReciveName(f.ReceivedDate),
                             DateCaption = CalcDateReciveCaption(f.ReceivedDate),
                             f.RefMessageAttachment,
                             MessageTypeCaption = GetMessageTypeCaption(f.MessageType),
                             f.TransferLink
                         }).ToList();

              return new AjaxStoreResult(list, messageService.GetMessagesCount(User.DbUser.ID));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public RestResult SendMessage(string msg, string recips, string fileName)
        {
            try
            {
                var messageWrapper = new MessageWrapper
                    {
                        Subject = msg,
                        Body = msg,
                        MessageType = MessageType.AdministratorMessage,
                        MessageImportance = MessageImportance.Importance,
                        RefUserSender = User.DbUser.ID
                    };

                var list = recips.Split(',');

                var groups = new List<int>();
                var users = new List<int>();

                foreach (var s in list)
                {
                    if (s.StartsWith("g"))
                    {
                        groups.Add(Convert.ToInt32(s.Replace("g", String.Empty)));
                    }

                    if (s.StartsWith("u"))
                    {
                        users.Add(Convert.ToInt32(s.Replace("u", String.Empty)));
                    }
                }

                IList<MessageAttachmentWrapper> messageAttachments =
                    messageAttachmentRepository.FindAll().Where(
                        x => x.DocumentName == string.Format("message" + "_{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), fileName)).
                        Select(x => new MessageAttachmentWrapper
                        {
                            DocumentFileName = x.DocumentFileName,
                            Document = x.Document,
                            DocumentName = x.DocumentName
                        }).ToList();
                if (messageAttachments.Count > 0)
                {
                    messageWrapper.RefMessageAttachment = messageAttachments.ElementAt(0);
                }

                messageService.SendMessage(messageWrapper, groups, users);

                return new RestResult { Success = true };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RestResult DeleteMessage(string ids)
        {
            try
            {
                string[] parts = ids.Split(',');

                foreach (var part in parts)
                {
                    messageService.DeleteMessage(Convert.ToInt32(part.Trim()));
                }

                return new RestResult { Success = true };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public RestResult MakeMessagesRead(string ids)
        {
            try
            {
                string[] parts = ids.Split(',');

                foreach (var part in parts)
                {
                    messageService.UpdateMessage(Convert.ToInt32(part.Trim()), (int)MessageStatus.Read);
                }

                return new RestResult { Success = true };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public ActionResult Upload()
        {
            var ajaxFormResult = new AjaxFormResult();

            try
            {
                HttpPostedFileBase attachment = Request.Files[0];
                if (attachment == null)
                {
                    throw new Exception("Ошибка передачи файла на сервер");
                }

                var fileBody = new byte[attachment.ContentLength];
                attachment.InputStream.Read(fileBody, 0, attachment.ContentLength);

                var messageAttachment = new MessageAttachment
                {
                    Document = fileBody,
                    DocumentFileName = attachment.FileName,
                    DocumentName = string.Format("message" + "_{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), Path.GetFileName(attachment.FileName))
                };

                messageService.UploadAttachment(messageAttachment);

                var script = new StringBuilder();
                script.AppendLine("Ext.MessageBox.hide();");

                ajaxFormResult.Success = true;
                ajaxFormResult.ExtraParams["msg"] = "Файл успешно передан на сервер.";
                ajaxFormResult.IsUpload = true;
                ajaxFormResult.Script = script.ToString();

                return ajaxFormResult;
            }
            catch (Exception e)
            {
                ajaxFormResult.Success = false;
                ajaxFormResult.Script = null;
                ajaxFormResult.ExtraParams["msg"] = "Ошибка передачи файла.";
                ajaxFormResult.ExtraParams["responseText"] = e.Message;
                ajaxFormResult.IsUpload = true;
                return ajaxFormResult;
            }
        }

        public ActionResult DownloadAttachment(int attachmentId)
        {
            var attachment = messageAttachmentRepository.FindOne(attachmentId);

            byte[] buff = attachment.Document;

            string mimetype = GetContentMimeType(attachment.DocumentFileName);

            if ((buff != null) && (buff.Length > 0))
            {
                var fcr = new FileContentResult(buff, mimetype)
                    {
                        FileDownloadName = FileHelper.GetDownloadableFileName(attachment.DocumentFileName)
                    };

                return fcr;
            }

            var ar = new AjaxFormResult();
            ar.ExtraParams["msg"] = "Ошибка загрузки файла.";
            ar.ExtraParams["responseText"] = "Файл не был ранее загружен на сервер!";
            ar.Success = false;
            return ar;
        }

        public AjaxStoreResult GetUsersAndGroups()
        {
            DataTable groupsTable = messageService.GroupsTable;
            DataTable usersTable = messageService.UsersTable;

            var groups = (from f in groupsTable.AsEnumerable()
                                     select new
                                     {
                                         Value = "g{0}".FormatWith(Convert.ToString(f["ID"])),
                                         Text = String.Format("Группа {0}", Convert.ToString(f["NAME"])),
                                         Order = 0
                                     }).OrderBy(x => x.Text).ToList();

            var users = (from f in usersTable.AsEnumerable()
                                    select new
                                    {
                                        Value = "u{0}".FormatWith(Convert.ToString(f["ID"])),
                                        Text = Convert.ToString(f["NAME"]),
                                        Order = 1
                                    }).OrderBy(x => x.Text).ToList();

            var mergedList = groups.Union(users).OrderBy(x => x.Order).ToList();

            return new AjaxStoreResult(mergedList, mergedList.Count());
        }

        #region private methods

        private string GetContentMimeType(string fileName)
        {
            switch (Path.GetExtension(fileName))
            {
                case ".doc": return "application/msword";
                case ".dot": return "application/msword";
                case ".xls": return "application/vnd.ms-excel";
                case ".xla": return "application/vnd.ms-excel";
                case ".xlt": return "application/vnd.ms-excel";
                case ".xlc": return "application/vnd.ms-excel";
                case ".xlm": return "application/vnd.ms-excel";
                case ".xlw": return "application/vnd.ms-excel";
                case ".txt": return "text/plain";
                default: return "application/octet-stream";
            }
        }

        private int GetMessageType(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.PumpMessage:
                case MessageType.PumpProcessMessage:
                case MessageType.PumpCheckDataMessage:
                case MessageType.PumpProcessCubesMessage:
                case MessageType.PumpAssociateMessage:
                    {
                        return Convert.ToInt32(MessageType.PumpMessage);
                    }

                default:
                    return Convert.ToInt32(messageType);
            }
        }

        private string GetMessageTypeCaption(MessageType messageType)
        {
            FieldInfo fi = messageType.GetType().GetField(messageType.ToString());
            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (!attributes.Any())
            {
                throw new Exception(String.Format("У типа сообщения {0} не указано описание.", messageType));
            }

            return attributes[0].Description;
        }

        private string CalcDateReciveName(DateTime date)
        {
            DateTime receivedDate = date.Date;

            DateTime today = DateTime.Now;
            int delta = DayOfWeek.Monday - today.DayOfWeek;
            DateTime monday = today.AddDays(delta);

            if (receivedDate.Date == DateTime.Today)
            {
                return "Сегодня";
            }

            if (receivedDate.Date == DateTime.Today.AddDays(-1f))
            {
                return "Вчера";
            }

            if (receivedDate.Date < monday.Date && receivedDate.Date > DateTime.Today.AddDays(-7f))
            {
                return "На прошлой неделе";
            }

            if (receivedDate.Date > DateTime.Today.AddDays(-7f))
            {
                return GetDayOfWeekName(receivedDate);
            }

            return "Неделю назад или ранее";
        }

        private string CalcDateReciveCaption(DateTime date)
        {
            if (date.Date == DateTime.Today)
            {
                return date.ToString("HH:mm");
            }

            if (date.Date > DateTime.Today.AddDays(-7f))
            {
                return String.Format(
                    "{0} {1}",
                    GetDayOfWeekName(date),
                    date.ToString("HH:mm"));
            }

            return date.ToString("dd.MM.yyyy");
        }

        private string GetDayOfWeekName(DateTime groupByDate)
        {
            switch (groupByDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Вс";
                case DayOfWeek.Monday:
                    return "Пн";
                case DayOfWeek.Tuesday:
                    return "Вт";
                case DayOfWeek.Wednesday:
                    return "Ср";
                case DayOfWeek.Thursday:
                    return "Чт";
                case DayOfWeek.Friday:
                    return "Пт";
                case DayOfWeek.Saturday:
                    return "Сб";
            }

            return string.Empty;
        }

        #endregion
    }
}