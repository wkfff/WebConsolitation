using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons
{
    // todo впилить лог операции и выводить лог а не ошибку при любой ситуации
    public sealed class PumpWebCons
    {
        #region Поля

        // периодичность задач(Годовая бюджетная отчетность)
        /* на тесте */
        /*private const string ReportPeriodPeriodic = "147872b3-77e7-4df5-8b16-f4a3ee66b054";*/
        /* на продакшине */
        private const string ReportPeriodPeriodic = "6de21230-bb0b-46b0-a13f-b90e55fc3dcc";

        /// <summary>
        /// Максимальная длинна поля Наименования. Определяется форматами ГМУ
        /// С консы могут приходить гораздо больше
        /// </summary>
        private const int MaxLenghtName = 300;

        private readonly string baseUrl;

        /*private const string baseUrl = "https://report.adm.yar.ru/application/backdoor-v2?";*/

        /*private const string baseUrl = "https://76.report.krista.ru/application/backdoor-v2?";*/

        /*private const string baseUrl = "http://test.report.krista.ru/application/backdoor-v2?";*/

        /*private const string baseUrl = "http://beta.report.krista.ru/application/backdoor-v2?";*/

        private readonly string login;

        private readonly string pass;
        
        private readonly INewRestService newRestService;

        private readonly F_F_ParameterDoc doc;

        private readonly FormNameFactory formNameFactory = new FormNameFactory();

        private CookieContainer pumpCookies;

        private ProcessFormDataDelegate curProcessFormDataDelegate;

        private int errorCounter;

        #endregion Поля

        public PumpWebCons()
        {
            newRestService = Resolver.Get<INewRestService>();

            switch (ConfigurationManager.AppSettings["ClientLocationOKATOCode"])
            {
                case "78":
                    baseUrl = "https://report.adm.yar.ru/backdoor/backdoor-v2?";
                    login = "web86";
                    pass = "master";
                    break;
                case "96":
                    baseUrl = "https://20.report.krista.ru/backdoor/backdoor-v2?";
                    login = "web86";
                    pass = "master";
                    break;
                default:
                    throw new InvalidDataException("Неизвестный код ОКАТО");
            }
        }

        public PumpWebCons(int docId) : this()
        {
            doc = newRestService.GetItem<F_F_ParameterDoc>(docId);

            // на всякий случай подстрахуемся!
            if (new[] { FX_Org_SostD.ExportedStateID, FX_Org_SostD.FinishedStateID }.Contains(doc.RefSost.ID))
            {
                throw new Exception("Документ находится в состоянии \"{0}\". Нельзя импортировать.".FormatWith(doc.RefSost.Name));
            }
        }

        private delegate void ProcessFormDataDelegate(string data);

        private delegate void ProcessWebConsDataDelegate(XmlDocument doc);

        private CookieContainer PumpCookies
        {
            get
            {
                if (pumpCookies != null)
                {
                    return pumpCookies;
                }

                // храним куки для всех последующих операций
                pumpCookies = new CookieContainer();

                return pumpCookies;
            }
        }

        /// <summary>
        /// Проверка и формирование имени показателя
        /// </summary>
        /// <param name="name">наименование показателя</param>
        /// <returns>обработаное наименование</returns>
        public static string GetIndicatorName(string name)
        {
            if (name.IsNullOrEmpty())
            {
                return "-";
            }

            return name.Length > MaxLenghtName ? name.Substring(0, MaxLenghtName) : name;
        }

        public static void SetHeadAttr(string okpo, F_F_ParameterDoc doc)
        {
            var newRestService = Resolver.Get<INewRestService>();

            if (okpo.IsNotNullOrEmpty())
            {
                var row = doc.ReportHeadAttribute.FirstOrDefault() ??
                                            new F_Report_HeadAttribute
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                RefPeriodic = newRestService.GetItem<FX_FX_Periodic>(FX_FX_Periodic.AnualID),
                                                Datedata = DateTime.Parse("1/1/{0}".FormatWith(doc.RefYearForm.ID + 1))
                                            };

                if (row.founderAuthorityOkpo.IsNullOrEmpty())
                {
                    row.founderAuthorityOkpo = okpo;
                }

                newRestService.Save(row);
            }
        }

        /// <summary>
        /// закачка субъектов
        /// </summary>
        public void PumpWebConsSubjects()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            
            newRestService.GetItems<T_Org_SubjectsCons>().Each(x => newRestService.Delete(x));
            newRestService.CommitChanges();

            PumpSubjects();
        }

        /// <summary>
        /// закачка форм(должны быть закачены сначала субъекты)
        /// </summary>
        public void PumpWebConsData()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            try
            {
                var roles = GetSubjectType(doc.RefUchr.RefTipYc.ID);
                if (roles == null)
                {
                    throw new Exception("Роль учреждения не определена");
                }

                var subjects = newRestService.GetItems<T_Org_SubjectsCons>().Where(x => roles.Contains(x.SubjectRole));

                if (!subjects.Any())
                {
                    throw new Exception("Нет субъектов с ролями {0}".FormatWith(roles));
                }

                // ищем субъект сначала по ИНН, если несколько добавляем КПП, если опять несколько то берем с "короткой" ролью или уже по названию просто ищем
                var institutions = subjects.Where(x => x.Inn != null && x.Inn.Equals(doc.RefUchr.INN)).ToList();

                if (institutions.Count != 1)
                {
                    institutions = institutions.Any()
                                        ? institutions.Where(x => x.Kpp != null && x.Kpp.Equals(doc.RefUchr.KPP)).ToList()
                                        : subjects.Where(x => x.Kpp != null && x.Kpp.Equals(doc.RefUchr.KPP)).ToList();

                    if (institutions.Count != 1)
                    {
                        if (institutions.Any())
                        {
                            institutions = institutions.Where(x => x.SubjectRole.Equals(GetMainRole(doc.RefUchr.RefTipYc.ID))).ToList();
                            if (institutions.Count != 1)
                            {
                                institutions = institutions.Any()
                                            ? institutions.Where(x => x.Name.Equals(doc.RefUchr.Name)).ToList()
                                            : subjects.Where(x => x.Name.Equals(doc.RefUchr.Name)).ToList();
                            }
                        }
                        else
                        {
                            institutions = subjects.Where(x => x.Name.Equals(doc.RefUchr.Name)).ToList();
                        }
                    }
                }

                var subjectId = institutions.SingleOrDefault().With(x => x.Uuid);

                if (subjectId.IsNullOrEmpty())
                {
                    throw new Exception("Субъект \"{0}\" с ИНН: {1}, КПП: {2} не найден".FormatWith(doc.RefUchr.Name, doc.RefUchr.INN, doc.RefUchr.KPP));
                }
                
                ProcessTasks(subjectId);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ExpandException());

                throw;
            }
        }

        #region работа с веб сервисом 

        private HttpWebRequest GetRequest(string verb)
        {
            string urlWithParams = string.Format("{0}{1}", baseUrl, verb);
#if DEBUG
            Trace.TraceInformation(urlWithParams);
#endif
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlWithParams);

            // ставим 10 минут - ПИЗДЕЦ! но чото все тормозит у веб консолидаторов
            httpWebRequest.Timeout = 6000000;

            httpWebRequest.Proxy = ConfigurationManager.AppSettings
                .With(collection => collection.Get("E86n.Proxy"))
                .With(s => Regex.Match(s, @"http://(\w+):(\w+)@([a-zA-Z_0-9.]+):(\d{4})"))
                .With(
                    match =>
                    new WebProxy(match.Groups[3].Value, Convert.ToInt32(match.Groups[4].Value))
                    {
                        Credentials = new NetworkCredential(match.Groups[1].Value, match.Groups[2].Value)
                    });

            return httpWebRequest;
        }

        private void SetAuthorization(string plogin, string password)
        {
            string verb = string.Format("verb=login&username={0}&password={1}", plogin, password);
            var httpWebRequest = GetRequest(verb);

            httpWebRequest.CookieContainer = PumpCookies;

            httpWebRequest.GetResponse();
        }

        private string GetWebDataWithAuth(string verb)
        {
            try
            {
               return GetWebData(verb);
            }
            catch (WebException e)
            {
                if (((HttpWebResponse)e.Response).StatusCode.Equals(HttpStatusCode.PreconditionFailed))
                {
                    SetAuthorization(login, pass);
                }
            }

            return GetWebData(verb);
        }

        private string GetWebData(string verb)
        {
            var httpWebRequest = GetRequest(verb);
            httpWebRequest.CookieContainer = PumpCookies;
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            
            var responseStream = httpWebResponse.GetResponseStream();

            if (responseStream == null)
            {
                throw new NullReferenceException("GetWebData: responseStream is null");
            }

            var sr = new StreamReader(responseStream);
            return sr.ReadToEnd();
        }

        #endregion работа с веб сервисом

        #region закачка субъекта

        /// <summary>
        /// получение роли по типу учреждения
        /// </summary>
        /// <param name="typeInst">тип учреждения</param>
        /// <returns> массив ролей из консолидации</returns>
        private IEnumerable<string> GetSubjectType(int typeInst)
        {
            switch (typeInst)
            {
                case FX_Org_TipYch.AutonomousID:
                    return new[] { "au", "au_subd" };
                case FX_Org_TipYch.BudgetaryID:
                    return new[] { "bu", "bu_subd" };
                case FX_Org_TipYch.GovernmentID:
                    return new[] { "pbs", "pbs_subd" };
                default:
                    return null;
            }
        }

        private string GetMainRole(int typeInst)
        {
            switch (typeInst)
            {
                case FX_Org_TipYch.AutonomousID:
                    return "au";
                case FX_Org_TipYch.BudgetaryID:
                    return "bu";
                case FX_Org_TipYch.GovernmentID:
                    return "pbs";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Закачивает субъекты
        /// </summary>
        private void PumpSubjects()
        {
            string verb = "verb=query&what=subject&role={0}&-follow=person".FormatWith("au,au_subd,bu,bu_subd,pbs,pbs_subd");
            var data = GetWebDataWithAuth(verb);

            if (data.IsNullOrEmpty())
            {
                return;
            }

            var subjects = Pump.PumpWebCons.subjects.Objects.Parse(data);
            /*subjects.Save(@"e:\xml.xml");*/

            subjects.ReportSubject.Where(x => x.person != null).Each(x =>
            {
                var person = subjects.Office.FirstOrDefault(o => o.uuid.Equals(x.person.Officekey.uuid));

                if (person != null)
                {
                    var row = new T_Org_SubjectsCons
                    {
                        Uuid = x.uuid.IsNotNullOrEmpty() ? x.uuid.Trim() : null,
                        SubjectRole = x.subjectRole.ReportSubjectRolekey.code.IsNotNullOrEmpty() ? x.subjectRole.ReportSubjectRolekey.code.Trim() : null,
                        Name = person.fullName.IsNotNullOrEmpty() ? person.fullName.Trim() : null,
                        Inn = person.inn.IsNotNullOrEmpty() ? person.inn.Trim() : null,
                        Kpp = person.kpp.IsNotNullOrEmpty() ? person.kpp.Trim() : null
                    };

                    newRestService.Save(row);
                }
            });
        }

        #endregion закачка субъекта

        #region закачка данных

        private void ProcessWebConsData(string data, ProcessWebConsDataDelegate pwcdDelegate)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(data);
            try
            {
                pwcdDelegate(xmlDoc);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref xmlDoc);
            }
        }

        private void ProcessTasks(string subject)
        {
            const string Msg = "Документ не найден";

            var yearTask = doc.RefYearForm.ID + 1;

            var startDate = string.Format("{0}.{1}.00", yearTask, 1);
            var endDate = string.Format("{0}.{1}.30", yearTask, 1);
            var verb = string.Format("verb=query&what=task&from={0}&till={1}", startDate, endDate);
            var taskData = GetWebDataWithAuth(verb);
            var tasks = Pump.PumpWebCons.tasks.Objects.Parse(taskData).ReportCollectingPeriod
                .Where(x =>
                {
                    var reportPeriodAbstract = x.reportPeriodAbstract.SingleOrDefault();
                    return reportPeriodAbstract != null && reportPeriodAbstract.ReportPeriodPeriodickey.uuid.Contains(ReportPeriodPeriodic);
                })
                   .Select(x => x.uuid).ToList();

            if (!tasks.Any())
            {
                throw new Exception(Msg);
            }

            tasks.Each(
                task =>
                {
                    errorCounter = 0;

                    foreach (var item in formNameFactory.GetFormNames(doc))
                    {
                        SetFormReaderByFormName(item);
                        GetFormMasterData(task, item, subject);
                    }
                    
                    if (doc.RefPartDoc.ID == FX_FX_PartDoc.AnnualBalanceF0503737Type)
                    {
                        // если все из 5ти отчетов не найдены то генерим исключение иначе закачиваем то что есть
                        if (errorCounter == 5)
                        {
                            throw new Exception(Msg);
                        }
                    }
                    else
                    {
                        if (errorCounter != 0)
                        {
                            throw new Exception(Msg);
                        }
                    }
                });
        }

        private void GetFormMasterData(string taskUuid, string formName, string subjectUuid)
        {
            string verb = string.Format("verb=query&what=form&type={0}&task={1}&subject={2}&state=Принят", formName, taskUuid, subjectUuid);
            string formsData = GetWebDataWithAuth(verb);
            if (formsData != string.Empty)
            {
                ProcessWebConsData(formsData, PumpFormMasterRow);
            }
            else
            {
                errorCounter++;
            }
        }

        private void GetFormsDetailData(string formUuid)
        {
            // для 737 формы нужно расширить запрос на данные
            string verb = string.Format(
                doc.RefPartDoc.ID == FX_FX_PartDoc.AnnualBalanceF0503737Type
                    ? "verb=query&what=form&uuid={0}&-follow=reportSections,rows,headerReqHolder"
                    : "verb=query&what=form&uuid={0}&-follow=reportSections,rows",
                formUuid);

            string formData = GetWebDataWithAuth(verb);
            curProcessFormDataDelegate(formData);
        }

        private void PumpFormMasterRow(XmlDocument pdoc)
        {
            XmlNode parentNode = pdoc.SelectSingleNode("Objects");
            if (parentNode != null)
            {
                foreach (XmlNode xn in parentNode.ChildNodes)
                {
                    string formUuid = XmlHelper.GetStringAttrValue(xn, "uuid", string.Empty);
                    if (formUuid == string.Empty)
                    {
                        continue;
                    }

                    GetFormsDetailData(formUuid);
                }
            }
        }
        #endregion закачка данных

        #region Функции ридеры для импорта

        private void SetFormReaderByFormName(string formName)
        {
            switch (formName)
            {
                case F0503121PumpWebCons.F0503121ClassV1:
                    curProcessFormDataDelegate = new F0503121PumpWebCons(newRestService, doc).ProcessFormData0503121V1;
                    break;
                case F0503121PumpWebCons.F0503121ClassV2:
                    curProcessFormDataDelegate = new F0503121PumpWebCons(newRestService, doc).ProcessFormData0503121V2;
                    break;
                case F0503121PumpWebCons.F0503121ClassV3:
                    curProcessFormDataDelegate = new F0503121PumpWebCons(newRestService, doc).ProcessFormData0503121V3;
                    break;
                case F0503127PumpWebCons.F0503127V1:
                    curProcessFormDataDelegate = new F0503127PumpWebCons(newRestService, doc).ProcessFormData0503127V1;
                    break;
                case F0503127PumpWebCons.F0503127V2016:
                    curProcessFormDataDelegate = new F0503127PumpWebCons(newRestService, doc).ProcessFormData0503127V2016;
                    break;
                case F0503130PumpWebCons.F0503130ClassV2:
                    curProcessFormDataDelegate = new F0503130PumpWebCons(newRestService, doc).ProcessFormData0503130V2;
                    break;
                case F0503130PumpWebCons.F0503130ClassV3:
                    curProcessFormDataDelegate = new F0503130PumpWebCons(newRestService, doc).ProcessFormData0503130V3;
                    break;
                case F0503130PumpWebCons.F0503130ClassV4:
                    curProcessFormDataDelegate = new F0503130PumpWebCons(newRestService, doc).ProcessFormData0503130V4;
                    break;
                case F0503130PumpWebCons.F0503130ClassV5:
                    curProcessFormDataDelegate = new F0503130PumpWebCons(newRestService, doc).ProcessFormData0503130V5;
                    break;
                case F0503130PumpWebCons.F0503130ClassV2016:
                    curProcessFormDataDelegate = new F0503130PumpWebCons(newRestService, doc).ProcessFormData0503130V2016;
                    break;
                case F0503721PumpWebCons.F0503721V1:
                    curProcessFormDataDelegate = new F0503721PumpWebCons(newRestService, doc).ProcessFormData0503721V1;
                    break;
                case F0503721PumpWebCons.F0503721V2:
                    curProcessFormDataDelegate = new F0503721PumpWebCons(newRestService, doc).ProcessFormData0503721V2;
                    break;
                case F0503721PumpWebCons.F0503721V3:
                    curProcessFormDataDelegate = new F0503721PumpWebCons(newRestService, doc).ProcessFormData0503721V3;
                    break;
                case F0503730PumpWebCons.F0503730V2:
                    curProcessFormDataDelegate = new F0503730PumpWebCons(newRestService, doc).ProcessFormData0503730V2;
                    break;
                case F0503730PumpWebCons.F0503730V3:
                    curProcessFormDataDelegate = new F0503730PumpWebCons(newRestService, doc).ProcessFormData0503730V3;
                    break;
                case F0503730PumpWebCons.F0503730V4:
                    curProcessFormDataDelegate = new F0503730PumpWebCons(newRestService, doc).ProcessFormData0503730V4;
                    break;
                case F0503730PumpWebCons.F0503730V5:
                    curProcessFormDataDelegate = new F0503730PumpWebCons(newRestService, doc).ProcessFormData0503730V5;
                    break;
                case F0503730PumpWebCons.F0503730V2016:
                    curProcessFormDataDelegate = new F0503730PumpWebCons(newRestService, doc).ProcessFormData0503730V6;
                    break;
                case F05037372PumpWebCons.F05037372V2:
                    curProcessFormDataDelegate = new F05037372PumpWebCons(newRestService, doc).ProcessFormData05037372V2;
                    break;
                case F05037372PumpWebCons.F05037372V3:
                    curProcessFormDataDelegate = new F05037372PumpWebCons(newRestService, doc).ProcessFormData05037372V3;
                    break;
                case F05037372PumpWebCons.F05037372V2016:
                    curProcessFormDataDelegate = new F05037372PumpWebCons(newRestService, doc).ProcessFormData05037372V2016;
                    break;
                case F05037374PumpWebCons.F05037374V2:
                    curProcessFormDataDelegate = new F05037374PumpWebCons(newRestService, doc).ProcessFormData05037374V2;
                    break;
                case F05037374PumpWebCons.F05037374V3:
                    curProcessFormDataDelegate = new F05037374PumpWebCons(newRestService, doc).ProcessFormData05037374V3;
                    break;
                case F05037374PumpWebCons.F05037374V2016:
                    curProcessFormDataDelegate = new F05037374PumpWebCons(newRestService, doc).ProcessFormData05037374V2016;
                    break;
                case F05037375PumpWebCons.F05037375V2:
                    curProcessFormDataDelegate = new F05037375PumpWebCons(newRestService, doc).ProcessFormData05037375V2;
                    break;
                case F05037375PumpWebCons.F05037375V3:
                    curProcessFormDataDelegate = new F05037375PumpWebCons(newRestService, doc).ProcessFormData05037375V3;
                    break;
                case F05037375PumpWebCons.F05037375V2016:
                    curProcessFormDataDelegate = new F05037375PumpWebCons(newRestService, doc).ProcessFormData05037375V2016;
                    break;
                case F05037376PumpWebCons.F05037376V2:
                    curProcessFormDataDelegate = new F05037376PumpWebCons(newRestService, doc).ProcessFormData05037376V2;
                    break;
                case F05037376PumpWebCons.F05037376V3:
                    curProcessFormDataDelegate = new F05037376PumpWebCons(newRestService, doc).ProcessFormData05037376V3;
                    break;
                case F05037376PumpWebCons.F05037376V2016:
                    curProcessFormDataDelegate = new F05037376PumpWebCons(newRestService, doc).ProcessFormData05037376V2016;
                    break;
                case F05037377PumpWebCons.F05037377V2:
                    curProcessFormDataDelegate = new F05037377PumpWebCons(newRestService, doc).ProcessFormData05037377V2;
                    break;
                case F05037377PumpWebCons.F05037377V3:
                    curProcessFormDataDelegate = new F05037377PumpWebCons(newRestService, doc).ProcessFormData05037377V3;
                    break;
                case F05037377PumpWebCons.F05037377V2016:
                    curProcessFormDataDelegate = new F05037377PumpWebCons(newRestService, doc).ProcessFormData05037377V2016;
                    break;
                default:
                    throw new Exception(string.Format("Закачка документа {0} за {1} год не поддерживается.", doc.RefPartDoc.Name, doc.RefYearForm.ID));
            }
        }

        #endregion Функции ридеры для импорта
    }
}
