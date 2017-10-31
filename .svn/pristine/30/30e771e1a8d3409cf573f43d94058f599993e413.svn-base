using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Services.ExportGMU
{
    public class ExportGmuService
    {
        private readonly IAuthService auth;
        private readonly ILinqRepository<F_F_ParameterDoc> headersRepository;
        private readonly IProgressManager progressManager;
        private readonly IScheme scheme;

        public ExportGmuService(IScheme scheme)
        {
            this.scheme = scheme;
            progressManager = Resolver.Get<IProgressManager>();
            headersRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();
            auth = Resolver.Get<IAuthService>();
        }

        public List<confirmation> ExportToGmu(string name, string pass, F_F_ParameterDoc header)
        {
            var confirmations = new List<confirmation>();
            try
            {
                SendDocument(name, pass, header, confirmations);
            }
            catch (Exception e)
            {
                Trace.TraceError("ExportToGmu: Ошибка экспорта документа {0}: {1}", header.ID, e.ExpandException());
                confirmations.Add(new confirmation
                                      {
                                          header = new headerType
                                                       {
                                                           id = Guid.NewGuid().ToString(),
                                                           createDateTime = DateTime.Now
                                                       },
                                          body = new packetResultType
                                                    {
                                                        result = "failure",
                                                        violation = new List<violationType>
                                                                        {
                                                                            new violationType
                                                                                {
                                                                                    code = "EXPORTDOC",
                                                                                    level = "error",
                                                                                    name = "Ошибка во время формирования и отправки документа: {0}".FormatWith(header.ID),
                                                                                    description = e.ExpandException()
                                                                                }
                                                                        }
                                                    }
                                      });
            }

            return confirmations;
        }

        public AjaxFormResult ExportGmu(string name, string pass, int docId)
        {
            try
            {
                F_F_ParameterDoc header = headersRepository.Load(docId);

                // проверяем доступность эеспорта
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["ExportGMU"]))
                {
                    throw new NotImplementedException("Экспорт на сайт ГМУ в данный момент не доступен. Попробуйте позже.");
                }
                
                var confirmations = new List<confirmation>();
                SendDocument(name, pass, header, confirmations);

                if (confirmations.All(x => x.body.result.Equals("success")))
                {
                    var positionResult = new StringBuilder();
                    confirmations.SelectMany(confirmation => confirmation.body.positionResult).Each(
                        x => positionResult.AppendLine(
                            string.Format(
                                "refPositionId={0};result={1};",
                                x.refPositionId,
                                x.result)));

                    var paramMsg = string.Format(
                                                "Пакет {0} - отправлен и обработан",
                                                confirmations.Select(confirm => confirm.body.refId).Aggregate((s, s1) => s1 + ";" + s));
                    
                    return new AjaxFormResult
                               {
                                   Success = true,
                                   ExtraParams =
                                       {
                                           new Parameter("msg", paramMsg),
                                           new Parameter("responseText", positionResult.ToString()),
                                           new Parameter("note", confirmations.Last().header.createDateTime.ToString(CultureInfo.InvariantCulture))
                                       }
                               };
                }

                var violation = new StringBuilder();
                confirmations.SelectMany(confirmation => confirmation.body.violation).Each(
                    x => violation.AppendLine(
                        string.Format(
                            "code={0};level={1};name={2};description{3}",
                            x.code,
                            x.level,
                            x.name,
                            x.description)));

                var paramMsgError = string.Format(
                                                "Пакет {0} - отправлен. Выявлены ошибки в процессе обработки пакета на сайте ГМУ",
                                                confirmations
                                                    .Where(confirm => !confirm.body.result.Equals("success"))
                                                    .Select(confirm => confirm.body.refId)
                                                    .Aggregate((s, s1) => s1 + ";" + s));

                return new AjaxFormResult
                           {
                               Success = false,
                               ExtraParams =
                                   {
                                       new Parameter("msg", paramMsgError),
                                       new Parameter("responseText", violation.ToString())
                                   }
                           };
            }
            catch (Exception e)
            {
                return new AjaxFormResult
                           {
                               Success = false,
                               ExtraParams =
                                   {
                                       new Parameter("msg", "Ошибка во время отправки пакета на сайт ГМУ."),
                                       new Parameter("serverError", "true"),
                                       new Parameter("responseText", e.ExpandException())
                                   }
                           };
            }
        }

        private static byte[] GetAttachmentContent(
                                                   string documentName,
                                                   byte[] documentContent,
                                                   string confirmationContent,
                                                   string exceptionContent)
        {
            using (var zipStream = new MemoryStream())
            {
                using (Package package = Package.Open(zipStream, FileMode.CreateNew))
                {
                    Uri partUriSendDocument = PackUriHelper.CreatePartUri(
                        new Uri(string.Format("{0}.xml", documentName), UriKind.Relative));
                    PackagePart packagePartSendDocument = package.CreatePart(
                        partUriSendDocument,
                        MediaTypeNames.Text.Xml,
                        CompressionOption.Maximum);
                    using (var streamWriter = new BinaryWriter(packagePartSendDocument.GetStream()))
                    {
                        streamWriter.Write(documentContent);
                    }

                    Uri partUriConfirmation = PackUriHelper.CreatePartUri(
                        new Uri(string.Format("confirmation_{0}.xml", documentName), UriKind.Relative));
                    PackagePart packagePartConfirmation = package.CreatePart(
                        partUriConfirmation,
                        MediaTypeNames.Text.Xml,
                        CompressionOption.Maximum);
                    using (var streamWriter = new StreamWriter(packagePartConfirmation.GetStream()))
                    {
                        streamWriter.Write(confirmationContent);
                    }

                    if (exceptionContent.IsNotNullOrEmpty())
                    {
                        Uri partUriException = PackUriHelper.CreatePartUri(new Uri("exception.txt", UriKind.Relative));
                        PackagePart packagePartException = package.CreatePart(
                            partUriException,
                            MediaTypeNames.Text.Plain,
                            CompressionOption.Maximum);
                        using (var streamWriter = new StreamWriter(packagePartException.GetStream()))
                        {
                            streamWriter.Write(exceptionContent);
                        }
                    }
                }

                return zipStream.ToArray();
            }
        }

        private void SendDocument(string name, string pass, F_F_ParameterDoc header, List<confirmation> confirmations)
        {
            switch (header.RefPartDoc.ID)
            {
                case FX_FX_PartDoc.PassportDocTypeID:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "institutionInfo",
                            ExportPassportService.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.StateTaskDocTypeID:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "stateTask",
                            header.RefYearForm.ID < 2016 ? ExportStateTaskService.Serialize(auth, header) : ExportStateTask2016Service.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.PfhdDocTypeID:
                    if (header.RefYearForm.ID < 2017)
                    {
                        confirmations.Add(
                            SendDocument(
                                name,
                                pass,
                                "financialActivityPlan",
                                ExportFinancialActivityPlanService.Serialize(auth, header),
                                header.RefUchr.Name));
                    }
                    else
                    {
                        confirmations.Add(
                            SendDocument(
                                name,
                                pass,
                                "financialActivityPlan2017",
                                ExportFinancialActivityPlan2017Service.Serialize(header),
                                header.RefUchr.Name));
                    }

                    if (confirmations.All(x => x.body.result.Equals("success")))
                    {
                        try
                        {
                            var actionGrantService = ExportActionGrantService.Serialize(header);
                            confirmations.Add(
                                SendDocument(
                                    name,
                                    pass,
                                    "actionGrant",
                                    actionGrantService,
                                    header.RefUchr.Name));
                        }
                        catch (InvalidDataException)
                        {
                            // данное исключение возникает если отсутствуют данные по actionGrant, просто не формируем
                        }
                    }
                    
                    break;
                case FX_FX_PartDoc.SmetaDocTypeID:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "budgetaryCircumstances",
                            ExportBudgetaryCircumstancesService.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.ResultsOfActivityDocTypeID:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "activityResult",
                            ExportActivityResultService.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.InfAboutControlActionsDocTypeID:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "inspectionActivity",
                            ExportInspectionActivityService.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "annualBalanceF0503121",
                            ExportAnnualBalanceF0503121Service.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "annualBalanceF0503127",
                            ExportAnnualBalanceF0503127Service.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "annualBalanceF0503130",
                            ExportAnnualBalanceF0503130Service.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "annualBalanceF0503137",
                            ExportAnnualBalanceF0503137Service.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "annualBalanceF0503721",
                            ExportAnnualBalanceF0503721Service.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "annualBalanceF0503730",
                            ExportAnnualBalanceF0503730Service.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                        confirmations.Add(
                            SendDocument(
                                name,
                                pass,
                                "annualBalanceF0503737",
                                ExportAnnualBalanceF0503737Service.Serialize(auth, header),
                                header.RefUchr.Name));
                    break;
                case FX_FX_PartDoc.DiverseInfo:
                    confirmations.Add(
                        SendDocument(
                            name,
                            pass,
                            "diverseInfo",
                            ExportDiverseInfoService.Serialize(auth, header),
                            header.RefUchr.Name));
                    break;
            }

            progressManager.SetCompleted(1);
        }

        private confirmation SendDocument(string name, string pass, string fileName, byte[] content, string targetName)
        {
            progressManager.SetCompleted("Отправка документа " + fileName);
            const string Boundary = "_N - xfvnZWpeN9_oiHLhkYN0XpVlWtm1guiKS0a";
            var request =
                (HttpWebRequest)WebRequest.Create("https://bus.gov.ru/gmu-integration-web/services/upload");
            request.ContentType = string.Format("multipart/form-data; boundary={0}", Boundary);
            request.Method = "POST";
            request.KeepAlive = true;

            request.Proxy = ConfigurationManager.AppSettings
                .With(collection => collection.Get("E86n.Proxy"))
                .With(s => Regex.Match(s, @"http://(\w+):(\w+)@([a-zA-Z_0-9.]+):(\d{4})"))
                .With(
                    match =>
                    new WebProxy(match.Groups[3].Value, Convert.ToInt32(match.Groups[4].Value))
                        {
                            Credentials = new NetworkCredential(match.Groups[1].Value, match.Groups[2].Value)
                        });

            // вроде как отменяем необходимость добавлять сертификат казначейства
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            string confirmationContent = string.Empty;
            string exceptionContent = string.Empty;

            try
            {
                using (var requestStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(requestStream))
                    {
                        streamWriter.WriteLine("--{0}", Boundary);

                        streamWriter.WriteLine("Content-Disposition: form-data; name=\"login\"");
                        streamWriter.WriteLine();
                        streamWriter.WriteLine(name);
                        streamWriter.WriteLine("--{0}", Boundary);

                        streamWriter.WriteLine("Content-Disposition: form-data; name=\"password\"");
                        streamWriter.WriteLine();
                        streamWriter.WriteLine(pass);
                        streamWriter.WriteLine("--{0}", Boundary);

                        streamWriter.WriteLine(
                            "Content-Disposition: form-data; name=\"document\";filename=\"{0}_{1}_{2}\"",
                            fileName,
                            DateTime.Now.ToString("yyyymmddhhmmss"),
                            "001");
                        streamWriter.WriteLine("Content-Type: text/xml");
                        streamWriter.WriteLine();
                        streamWriter.WriteLine(Encoding.UTF8.GetChars(content));

                        streamWriter.WriteLine("--{0}--", Boundary);

                        streamWriter.Flush();
                        request.ContentLength = streamWriter.BaseStream.Length;
                    }

                    using (var streamWriter = new BinaryWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(requestStream.ToArray());
                    }
                }

                using (
                    StreamReader responseReader = request.GetResponse()
                        .GetResponseStream()
                        .With(stream => new StreamReader(stream)))
                {
                    confirmationContent = responseReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                exceptionContent = e.ExpandException();
                throw;
            }
            finally
            {
                SaveSendState(fileName, content, confirmationContent, targetName, exceptionContent);
            }

            return confirmation.Parse(confirmationContent);
        }

        private void SaveSendState(
                                    string documentName,
                                    byte[] documentContent,
                                    string confirmationContent,
                                    string targetName,
                                    string exceptionContent)
        {
            if (confirmationContent.IsNotNullOrEmpty() &&
                confirmation.Parse(confirmationContent).body.result.Equals("success"))
            {
                return;
            }

            progressManager.SetCompleted("Ошибки при отправлении. Сохранение статуса посылки");
            using (new ServerContext())
            {
                scheme.MessageManager.SendMessage(
                    new MessageWrapper
                        {
                            Subject = string.Format(
                                "EXPORT: {0} {1} {2}",
                                "FAIL",
                                documentName,
                                targetName),
                            DateTimeOfCreation = DateTime.Now,
                            DateTimeOfActual = DateTime.Now.AddDays(1),
                            MessageStatus = MessageStatus.New,
                            MessageImportance = MessageImportance.Importance,
                            MessageType = MessageType.AdministratorMessage,
                            RefGroupRecipient = 1,
                            RefMessageAttachment =
                                new MessageAttachmentWrapper
                                    {
                                        Document = GetAttachmentContent(
                                            documentName,
                                            documentContent,
                                            confirmationContent,
                                            exceptionContent),
                                        DocumentFileName = string.Format("{0}.xml.zip", documentName),
                                        DocumentName = documentName
                                    }
                        });
            }
        }
    }
}
