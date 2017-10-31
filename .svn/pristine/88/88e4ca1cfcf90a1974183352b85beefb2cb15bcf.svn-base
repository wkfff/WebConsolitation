using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Schema;
using bus.gov.ru;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using HttpMultipartParser;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers;
using Krista.FM.RIA.Extensions.E86N.Services.Pump;
using Xml.Schema.Linq;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public class IntegrationAsGmuController : Controller
    {
        [HttpPost]
        public ActionResult Upload()
        {
            var type = HttpContext.Request.Headers["Content-Type"];
            var boundary = type.Substring(type.IndexOf('=') + 1);
            var data = HttpContext.Request.InputStream;
            var parser = new MultipartFormDataParser(data, boundary, Encoding.UTF8);
            
            var login = parser.Parameters["login"].Data;
            var password = parser.Parameters["password"].Data;

            if (!new AccountMembershipService().ValidateUser(login, password))
            {
                return ConfirmationResult(
                    new confirmation
                        {
                            header = Header(),
                            body = new packetResultType
                                       {
                                           result = "failure",
                                           violation =
                                               new List<violationType>
                                                   {
                                                       new violationType
                                                           {
                                                               code = "USRE",
                                                               name = "Некорректные данные пользователя",
                                                               level = "error",
                                                           },
                                                   },
                                       },
                        });
            }

            new FormsAuthenticationService().SignIn(login, false);

            var extractedDocument = ExtractDocument(parser.Files.First(part => part.Name.Equals("document")));
            IEnumerable<violationType> violations;

            if (!((XTypedElement)extractedDocument).Validate(Resolver.Get<XmlSchemaSet>(), out violations))
            {
                return ConfirmationResult(
                    new confirmation
                        {
                            header = Header(),
                            body = new packetResultType
                                       {
                                           result = "failure",
                                           violation = violations.ToList(),
                                       },
                        });
            }

            using (new ServerContext())
            {
                var headerRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();
                headerRepository.DbContext.BeginTransaction();
                headerRepository.Save(BudgetaryCircumstances2Smeta.Pump((XTypedElement)extractedDocument));
                headerRepository.DbContext.CommitChanges();
                headerRepository.DbContext.CommitTransaction();
            }

            return ConfirmationResult(new confirmation
                                          {
                                              header = Header(),
                                              body = new packetResultType
                                                         {
                                                             result = "success",
                                                         }
                                          });
        }

        private static headerType Header()
        {
            return new headerType
                       {
                           createDateTime = DateTime.Now,
                           id = Guid.NewGuid().ToString(),
                       };
        }

        private static ActionResult ConfirmationResult(confirmation confirmation)
        {
            using (var confirmationStream = new MemoryStream())
            {
                using (var xmlWriter = new XmlTextWriter(confirmationStream, Encoding.UTF8))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.Indentation = 2;

                    confirmation.Save(xmlWriter);
                }

                return new FileContentResult(confirmationStream.ToArray(), MediaTypeNames.Text.Xml);
            }
        }

        private static IXMetaData ExtractDocument(FilePart documentPart)
        {
            if (documentPart.FileName.StartsWith("institutionInfo"))
            {
                return institutionInfo.Load(new StreamReader(documentPart.Data));
            }
            else if (documentPart.FileName.StartsWith("budgetaryCircumstances"))
            {
                return budgetaryCircumstances.Load(new StreamReader(documentPart.Data));
            }

            throw new NotImplementedException();
        }
    }
}
