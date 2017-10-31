using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ExportGMU;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public class UpLoadGmuController : SchemeBoundController
    {
        public readonly IStateSystemService StateSystemService;

        private readonly IProgressManager progressManager;

        private readonly IVersioningService versioningService;

        public UpLoadGmuController(IProgressManager progressManager, IVersioningService versioningService)
        {
            StateSystemService = Resolver.Get<IStateSystemService>();
            this.progressManager = progressManager;
            this.versioningService = versioningService;
        }

        public AjaxFormResult BatchExport(string name, string pass, int[] docs)
        {
            try
            {
                // проверяем доступность эеспорта
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["ExportGMU"]))
                {
                    throw new NotImplementedException("Экспорт на сайт ГМУ в данный момент не доступен. Попробуйте позже.");
                }

                var exportGmuService = new ExportGmuService(Scheme);
                var log = new StringBuilder();

                var docscounter = 0;
                var docsCount = docs.Length;
                docs.Each(
                    (item, index) =>
                        {
                            progressManager.SetCompleted("Экспорт документа {0} из {1}".FormatWith(index, docsCount), index / docsCount * 100);
                            var doc = StateSystemService.GetItem<F_F_ParameterDoc>(item);
                            if (doc.RefSost.ID.Equals(FX_Org_SostD.FinishedStateID) && !doc.CloseDate.HasValue)
                            {
                                var confirmations = exportGmuService.ExportToGmu(name, pass, doc);
                                ++docscounter;

                                if (confirmations.All(x => x.body.result.Equals("success")))
                                {
                                    log.AppendFormat("Пакет {0} - отправлен и обработан<br>", confirmations.Select(confirm => confirm.body.refId).Aggregate((s, s1) => s1 + ";" + s));

                                    // выполняем переход
                                    StateSystemService.SetState(item, FX_Org_SostD.ExportedStateID);

                                    // меняем примечание документа
                                    StateSystemService.ChangeNotes(item, " Export confirm:{0}".FormatWith(confirmations.Last().header.createDateTime.ToString(CultureInfo.InvariantCulture)));

                                    // закрываем документ
                                    versioningService.CloseDocument(item);
                                }
                                else
                                {
                                    log.AppendFormat(
                                        "Пакет {0}-отправлен.Выявлены ошибки в процессе обработки пакета на сайте ГМУ<br>",
                                        confirmations.Where(confirm => !confirm.body.result.Equals("success")).Select(confirm => confirm.body.refId).Aggregate((s, s1) => s1 + ";" + s));

                                    log.Append("Ошибки: ");
                                    var errors = new StringBuilder();
                                    confirmations.SelectMany(confirmation => confirmation.body.violation).Each(
                                        x =>
                                            {
                                                var msg = "code={0};level={1};name={2};description{3}<br>".FormatWith(x.code, x.level, x.name, x.description);
                                                errors.Append(msg.Replace("<br>", string.Empty));
                                                log.Append(msg);
                                            });

                                    // меняем примечание документа
                                    StateSystemService.ChangeNotes(
                                        item,
                                        "{0}_{1}".FormatWith(confirmations.Last().header.createDateTime.ToString(CultureInfo.InvariantCulture), errors.ToString()),
                                        false);
                                }
                            }
                        });

                return new AjaxFormResult
                           {
                               Success = true,
                               ExtraParams = { new Parameter("msg", "Обработано документов {0}".FormatWith(docscounter)), new Parameter("responseText", log.ToString()) }
                           };
            }
            catch (Exception e)
            {
                Trace.TraceError("BatchExport: Ошибка экспорта документов: {0}", e.ExpandException());
                return new AjaxFormResult
                           {
                               Success = false,
                               ExtraParams = { new Parameter("msg", "Ошибка во время отправки пакета на сайт ГМУ."), new Parameter("responseText", e.ExpandException()) }
                           };
            }
        }

        public AjaxFormResult Index(string name, string pass, int docs)
        {
            try
            {
                var result = new ExportGmuService(Scheme).ExportGmu(name, pass, docs);

                if (result.Success)
                {
                    // выполняем переход
                    StateSystemService.SetState(docs, FX_Org_SostD.ExportedStateID);

                    // меняем примечание документа
                    StateSystemService.ChangeNotes(docs, " Export confirm:{0}".FormatWith(result.ExtraParams["note"]));

                    // закрываем документ
                    versioningService.CloseDocument(docs);

                    return result;
                }

                return result;
            }
            catch (Exception e)
            {
                Trace.TraceError("Index: Ошибка экспорта документа: {0}", e.ExpandException());
                return new AjaxFormResult
                {
                    Success = false,
                    ExtraParams = { new Parameter("msg", "Ошибка во время отправки пакета на сайт ГМУ."), new Parameter("responseText", e.ExpandException()) }
                };
            }
        }
    }
}
