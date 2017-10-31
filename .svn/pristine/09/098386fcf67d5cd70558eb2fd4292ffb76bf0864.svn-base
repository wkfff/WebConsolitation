using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

using bus.gov.ru;
using bus.gov.ru.external.Item1;
using bus.gov.ru.Imports;

using Bus.Gov.Ru.Imports;

using bus.gov.ru.types.Item1;

using egrul.nalog.ru.Item2;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.OGSService;
using Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

using LINQtoCSV;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    // todo надо бы сделать отдельный сервайс закачек который бы инкапсулировал классы для разных закачек, т.е. каждая закачка должна быть оформлена в отдельный класс и вызываться из одного сервайса!!
    public class ImportsController : SchemeBoundController
    {
        private readonly IProgressManager progressManager;
        private readonly IOGSService service;
        private readonly INewRestService newRestService;
        private readonly IStateSystemService stateSystemService;

        public ImportsController(IOGSService service, IProgressManager progressManager)
        {
            this.service = service;
            this.progressManager = progressManager;

            newRestService = Resolver.Get<INewRestService>();
            stateSystemService = Resolver.Get<IStateSystemService>();
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult ImportNsiKbkBudget(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    IEnumerable<FX_FX_TypeKBK> cacheTypeKbk = newRestService.GetItems<FX_FX_TypeKBK>()
                                                                                .ToList();
                    IEnumerable<D_Fin_nsiBudget> cacheBudget = newRestService
                                                                .GetItems<D_Fin_nsiBudget>().ToList();

                    nsiKbkBudget.Load(new StreamReader(file))
                        .body.position.Each(
                            p =>
                                {
                                    D_Fin_KbkBudget finKbkBudget =
                                       newRestService.GetItems<D_Fin_KbkBudget>()
                                       .SingleOrDefault(x => x.Code.Equals(p.code)) ??
                                        new D_Fin_KbkBudget { Code = p.code };

                                    finKbkBudget.StartDate =
                                        CommonUtils.IsValidSqlDate(p.startDateActive)
                                            ? p.startDateActive
                                            : (DateTime?)null;

                                    finKbkBudget.ChahgeDate =
                                        CommonUtils.IsValidSqlDate(p.changeDate)
                                            ? p.changeDate
                                            : (DateTime?)null;

                                    finKbkBudget.EndDate =
                                        p.endDateActive.HasValue &&
                                        CommonUtils.IsValidSqlDate(p.endDateActive.Value)
                                            ? p.endDateActive
                                            : null;

                                    // todo: вообще это криминал, но такие исходные
                                    finKbkBudget.Name = p.name ?? "Название не указано";
                                    finKbkBudget.BusinessStatus = p.businessStatus;
                                    finKbkBudget.RefBudget =
                                        cacheBudget.SingleOrDefault(x => x.Code.Equals(p.budget.code));
                                    finKbkBudget.RefTypeKBK =
                                        cacheTypeKbk.SingleOrDefault(x => x.ID == Convert.ToInt32(p.type));

                                    newRestService.Save(finKbkBudget);
                                });
                }

                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult ImportNsiSubjectService(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    string ppo = ConfigurationManager.AppSettings["ClientLocationOKATOCode"];
                    IEnumerable<nsiSubjectServiceType> subjectServiceType =
                        nsiSubjectService.Load(new StreamReader(file))
                            .body.position.Where(x => x.ppo.code.StartsWith(ppo)).ToList();
                    IEnumerable<D_Org_PPO> cachePpo = newRestService.GetItems<D_Org_PPO>().ToList();
                    IEnumerable<D_Services_TipY> cacheType = newRestService.GetItems<D_Services_TipY>()
                                                                .ToList();
                    ////IEnumerable<D_Org_NsiOGS> cacheRegnum = _regnumRepository.FindAll().ToList();
                    IEnumerable<CachedRegNum> cacheRegnum = newRestService.GetItems<D_Org_NsiOGS>()
                        .Select(x => new CachedRegNum
                                         {
                                             RegNum = x.regNum,
                                             FullName = x.FullName
                                         })
                        .ToList();
                    IEnumerable<D_Org_OrgYchr> cacheFounder = newRestService.GetItems<D_Org_OrgYchr>()
                                                                                    .ToList();

                    var result = new Dictionary<string, D_Services_VedPer>();

                    subjectServiceType.Each(p => p.service.Each(
                        s =>
                            {
                                D_Services_VedPer dServicesVedPer =
                                    newRestService.GetItems<D_Services_VedPer>()
                                     .SingleOrDefault(x => x.Code.Equals(s.code)) ??
                                    new D_Services_VedPer
                                        {
                                            Code = s.code,
                                            Name = s.name,
                                            RefOrgPPO = cachePpo.Single(x => x.Code.Equals(p.ppo.code)),
                                            RefTipY =
                                                cacheType.Single(
                                                    x => x.Name.Equals(
                                                        NsiServiceTypeType2DServiceTipYName(s),
                                                        StringComparison.OrdinalIgnoreCase)),
                                            RefSferaD =
                                                newRestService.GetItems<D_Services_SferaD>().SingleOrDefault(
                                                    x => x.Name.Equals(s.field.name)) ??
                                                new D_Services_SferaD
                                                    {
                                                        Name = s.field.name,
                                                        Code = s.field.code
                                                    },
                                            NumberService = s.number,
                                            Founder = p.founder.fullName,
                                            RefYchred = cacheRegnum
                                                .SingleOrDefault(x => x.RegNum.Equals(p.founder.regNum))
                                                .With(
                                                    ogs =>
                                                    cacheFounder.SingleOrDefault(x => x.Name.Equals(ogs.FullName))),
                                            AuthorLaw = s.enactment.author.fullName,
                                            TypeLow = s.enactment.type,
                                            NameLow = s.enactment.name,
                                            DateLaw = s.enactment.date,
                                            NumberLaw = s.enactment.number
                                        };

                                dServicesVedPer.DataVkluch =
                                    CommonUtils.IsValidSqlDate(p.startDateActive)
                                        ? p.startDateActive
                                        : (DateTime?)null;
                                dServicesVedPer.DataIskluch =
                                    p.endDateActive.HasValue &&
                                    CommonUtils.IsValidSqlDate(p.endDateActive.Value)
                                        ? p.endDateActive
                                        : null;
                                dServicesVedPer.BusinessStatus = p.businessStatus;

                                if (dServicesVedPer.RefYchred != null && dServicesVedPer.RefYchred.RefNsiOgs != null)
                                {
                                    newRestService.GetItems<D_Org_Structure>().Where(x => x.CloseDate == null || x.CloseDate > DateTime.Now) 
                                        .SingleOrDefault(
                                            x =>
                                            x.INN.Equals(dServicesVedPer.RefYchred.RefNsiOgs.inn) &&
                                            x.KPP.Equals(dServicesVedPer.RefYchred.RefNsiOgs.kpp))
                                        .With(x => dServicesVedPer.RefGRBSs = x.RefOrgGRBS);
                                }

                                newRestService.Save(dServicesVedPer);
                                result.Add(dServicesVedPer.Code, dServicesVedPer);
                            }));

                    subjectServiceType.Each(p => p.service.Each(s => s.category.Each(
                        category => category
                                        .Unless(c => newRestService.GetItems<F_F_PotrYs>() 
                                                         .Any(
                                                             x =>
                                                             x.RefVedPP.Code.Equals(s.code) &&
                                                             x.RefCPotr.Name.Equals(category.name)))
                                        .Do(c => newRestService.Save(
                                            new F_F_PotrYs
                                                {
                                                    RefCPotr =
                                                        newRestService.GetItems<D_Services_CPotr>()
                                                        .SingleOrDefault(
                                                            x => x.Name.Equals(category.name)) ??
                                                        new D_Services_CPotr
                                                            {
                                                                Name = category.name,
                                                                Code = category.code
                                                            },
                                                    RefVedPP = result[s.code]
                                                })))));

                    subjectServiceType.Each(
                        p => p.service
                                 .Where(x => x.parentCode != null)
                                 .Join(
                                     result,
                                     type => type.code,
                                     pair => pair.Key,
                                     (type, pair) =>
                                     new
                                         {
                                             type.parentCode,
                                             service = pair.Value
                                         })
                                 .Each(
                                     a => newRestService
                                              .Save(newRestService.GetItems<D_Services_VedPer>()
                                                        .SingleOrDefault(parent => parent.Code.Equals(a.parentCode))
                                                        .With(x =>
                                                                  {
                                                                      a.service.ParentID = x.ID;
                                                                      return a.service;
                                                                  }))));
                }

                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        /// <summary>
        /// Закачка Ведомственного перечня услуг из ГМУ
        /// </summary>
        [HttpPost]
        public AjaxFormResult ImportService2016(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    var protocol = new ConfirmationDataPumpProtocolProvider();
                    new Service2016Pump().Pump(new StreamReader(file), protocol, fileName);

                    if (protocol.Confirmation.body.result.Equals("failure"))
                    {
                        return GetResult(false, fileName, protocol.ToString());
                    }

                    return GetResult(true, fileName, protocol.ToString());
                }
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.ExpandException());
            }
        }

        /// <summary>
        /// Импорт Ведомственного перечня услуг из Электронного бюджета
        /// </summary>
        [HttpPost]
        public AjaxFormResult ImportService2016FromEb(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    var protocol = new ConfirmationDataPumpProtocolProvider();
                    new Service2016EbPump().Pump(new StreamReader(file), protocol, fileName);
                    
                    var warningMsg = new StringBuilder();
                    foreach (var violationType in protocol.Confirmation.body.violation.Where(x => x.level.Equals("warning")))
                    {
                        warningMsg.Append("{0}<br>{1}<br><br>".FormatWith(violationType.name, violationType.description));
                    }

                    if (protocol.Confirmation.body.result.Equals("failure"))
                    {
                        return GetResult(false, fileName, protocol.ToString());
                    }

                    return GetResult(true, fileName, warningMsg.ToString());
                }
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.ExpandException());
            }
        }

        /// <summary>
        /// Импорт Ведомственного перечня услуг из Планирования
        /// </summary>
        [HttpPost]
        public AjaxFormResult ImportServicePlanning(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    var protocol = new ConfirmationDataPumpProtocolProvider();
                    new ServicePlanningPump().Pump(new StreamReader(file, Encoding.GetEncoding("windows-1251")), protocol, fileName);
                    
                    return GetResult(true, fileName, protocol.ToString());
                }
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.ExpandException());
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult ImportYaroslavlSubjectService(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    IEnumerable<YaroslavlService> services =
                        new CsvContext().Read<YaroslavlService>(
                            new StreamReader(file, Encoding.GetEncoding(CultureInfo.GetCultureInfo("ru-RU").TextInfo.ANSICodePage)),
                            new CsvFileDescription { SeparatorChar = ';', FirstLineHasColumnNames = false, EnforceCsvColumnAttribute = true }).ToList();

                    IEnumerable<D_Org_PPO> cachePpo = newRestService.GetItems<D_Org_PPO>().ToList();
                    IEnumerable<D_Services_TipY> cacheType = newRestService.GetItems<D_Services_TipY>()
                        .ToList();
                    D_Services_SferaD unnamedSferaD =
                        newRestService.GetItems<D_Services_SferaD>()
                        .SingleOrDefault(x => x.Name.Equals("Неуказанная сфера деятельности"));

                    services.Each(s => s.Code = s.Code.PadLeft(20, '0'));
                    services
                        .Where(s => !newRestService.GetItems<D_Services_VedPer>()
                                                    .Any(x => x.Code.Equals(s.Code)))
                        .Each(s =>
                                  {
                                      var dServicesVedPer =
                                          new D_Services_VedPer
                                              {
                                                  Code = s.Code, 
                                                  Name = s.Name, 
                                                  RefOrgPPO =
                                                      cachePpo.SingleOrDefault(
                                                          x => x.Code.Equals(s.Okato.PadRight(11, '0'))), 
                                                  RefTipY =
                                                      cacheType.Single(
                                                          x => x.Name
                                                                   .Equals(s.Type, StringComparison.OrdinalIgnoreCase)), 
                                                  NumberService = s.Number, 
                                                  AuthorLaw = s.EnactmentFounder, 
                                                  TypeLow = s.EnactmentType, 
                                                  NameLow = s.EnactmentName, 
                                                  DateLaw = s.EnactmentDate, 
                                                  NumberLaw = s.EnactmentNumber, 
                                                  //// fakeparam
                                                  BusinessStatus = "801", 
                                                  DataVkluch = s.EnactmentDate, 
                                                  RefSferaD = unnamedSferaD
                                              };
                                      newRestService.Save(dServicesVedPer);
                                      if (s.Category != null)
                                      {
                                          s.Category.Split(';').Each(
                                              category => category
                                                              .Unless(
                                                                  c => newRestService.GetItems<F_F_PotrYs>()
                                                                           .Any(
                                                                               x =>
                                                                               x.RefVedPP.Code.Equals(
                                                                                   dServicesVedPer.Code) &&
                                                                               x.RefCPotr.Name.Equals(category)))
                                                              .Do(
                                                                  c => newRestService.Save(
                                                                      new F_F_PotrYs
                                                                          {
                                                                              RefCPotr =
                                                                                  newRestService
                                                                                      .GetItems<D_Services_CPotr>()
                                                                                      .SingleOrDefault(
                                                                                          x => x.Name.Equals(category)) ??
                                                                                  new D_Services_CPotr
                                                                                      {
                                                                                          Name = category
                                                                                      }, 
                                                                              RefVedPP = dServicesVedPer
                                                                          })));
                                      }
                                  });
                }

                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult ImportEgrulFounder(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    IEnumerable<D_Org_NsiOGS> cacheOgs = newRestService.GetItems<D_Org_NsiOGS>().ToList();

                    EGRUL_UL_DATA.Load(
                        new StreamReader(
                            file,
                            Encoding.GetEncoding(CultureInfo.GetCultureInfo("ru-RU").TextInfo.ANSICodePage), 
                            true))
                        .UL.Each(ul => ul.UCHR.RUL.Each(
                            rul =>
                                {
                                    D_Org_OrgYchr orgYchr =
                                        newRestService.GetItems<D_Org_OrgYchr>().SingleOrDefault(x => x.Name.Equals(rul.NAMEP)) ??
                                        new D_Org_OrgYchr
                                            {
                                                Name = rul.NAMEP
                                            };

                                    // попробуем восстановить связь
                                    orgYchr.RefNsiOgs = cacheOgs.SingleOrDefault(
                                        x => x.inn.Equals(rul.INN) && x.kpp.Equals(rul.KPP));
                                    orgYchr.Code = rul.OGRN;

                                    newRestService.Save(orgYchr);
                                }));
                }

                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportOkato(string fileName)
        {
            try
            {
                Stream file = GetFile();

                var xmlfilereader = new XmlTextReader(file);
                service.ImportOKATO(xmlfilereader);
                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportSubsidies(string fileName)
        {
            try
            {
                Stream file = GetFile();

                var xmlFile = new XmlTextReader(file);
                try
                {
                    var rec = new D_Fin_OtherGant();
                    xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                    while (xmlFile.Read())
                    {
                        if (xmlFile.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlFile.Name)
                            {
                                case "Объект":
                                    if (xmlFile.GetAttribute("ВидСубсидии") ==
                                        "Субсидии на иные цели (Целевые субсидии)")
                                    {
                                        rec.ID = 0;
                                        rec.Code = xmlFile.GetAttribute("Код");
                                        rec.Name = xmlFile.GetAttribute("Name");
                                        D_Fin_OtherGant item =
                                            newRestService.GetItems<D_Fin_OtherGant>().SingleOrDefault(p => p.Code.Equals(rec.Code));
                                        if (item == null)
                                        {
                                            newRestService.Save(rec);
                                        }

                                        rec = new D_Fin_OtherGant();
                                    }

                                    break;
                            }
                        }
                    }
                }
                finally
                {
                    xmlFile.Close();
                }

                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportSubsidiesFromExcel(string fileName)
        {
            try
            {
                progressManager.SetCompleted("Обработка данных...", 0);

                HSSFSheet sheet = LoadExcelDocument(GetFile()).GetSheetAt(0);
                int count = sheet.LastRowNum - sheet.FirstRowNum;
                foreach (HSSFRow row in sheet)
                {
                    progressManager.SetCompleted("Обработка данных...", (double)(row.RowNum + 1) / count);
                    string code = row.GetCell(0).CellType == 0
                                      ? row.GetCell(0).NumericCellValue.ToString(CultureInfo.InvariantCulture)
                                      : row.GetCell(0).StringCellValue;
                    string name = row.GetCell(1).StringCellValue;
                    string ppo = row.GetCell(2).CellType == 0
                                     ? row.GetCell(2).NumericCellValue.ToString(CultureInfo.InvariantCulture)
                                     : row.GetCell(2).StringCellValue;

                    DateTime openDate = row.GetCell(3).DateCellValue;

                    if (!newRestService.GetItems<D_Fin_OtherGant>().Any(x => x.Code.Equals(code) && x.RefOrgPPO.Code.Equals(ppo) && x.Name.Equals(name)))
                    {
                       var otherGant = new D_Fin_OtherGant
                                       {
                                            Code = code,
                                            Name = name,
                                            RefOrgPPO = newRestService.GetItems<D_Org_PPO>()
                                                               .SingleOrDefault(x => x.Code.Equals(ppo))
                                       };

                        if (openDate != DateTime.MaxValue)
                        {
                            otherGant.OpenDate = openDate.Date;
                        }

                        newRestService.Save(otherGant);
                    }
                    else
                    {
                        if (openDate != DateTime.MaxValue)
                        {
                            newRestService.GetItems<D_Fin_OtherGant>().Where(x => x.Code.Equals(code) && x.RefOrgPPO.Code.Equals(ppo) && x.Name.Equals(name))
                                .Each(x =>
                                        {
                                           if (!x.OpenDate.HasValue)
                                           {
                                               x.OpenDate = openDate;
                                               newRestService.Save(x);
                                           }
                                        });
                        }
                    }
                }

                progressManager.SetCompleted("Обработка завершена", 1);
                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }
        
        [HttpPost]
        [Transaction]
        public AjaxFormResult ImportOkfs(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    nsiOkfs.Load(new StreamReader(file)).body.position.Each(
                        p =>
                        {
                            var okei = newRestService.GetItems<D_OKFS_OKFS>()
                                          .SingleOrDefault(x => x.Code == Convert.ToInt32(p.code)) ??
                                      new D_OKFS_OKFS { Code = Convert.ToInt32(p.code) };

                            okei.Name = p.name ?? "Не указано";
                            
                            newRestService.Save(okei);
                        });
                }

                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult ImportOkopf(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    nsiOkopf.Load(new StreamReader(file)).body.position.Each(
                        p =>
                        {
                            var okei = newRestService.GetItems<D_OKOPF_OKOPF>()
                                          .SingleOrDefault(x => x.Code == Convert.ToInt32(p.code)) ??
                                      new D_OKOPF_OKOPF { Code = Convert.ToInt32(p.code) };

                            okei.Name = p.name ?? "Не указано";
                            okei.businessStatus = p.businessStatus ?? "801";

                            newRestService.Save(okei);
                        });
                }

                return GetResult(true, fileName);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        [HttpPost]
        public AjaxFormResult ImportsFromGmuXml(string fileName)
        {
            try
            {
                using (Stream file = GetFile())
                {
                    var docId = CommonPump.PumpFile(new StreamReader(file), new ConfirmationDataPumpProtocolProvider(), fileName);

                    var protocol = CommonPump.GetCommonPump.DataPumpProtocol as ConfirmationDataPumpProtocolProvider;
                    var protocolText = protocol.ToString();

                    if (docId != -1 && protocol.Confirmation.body.result.Equals("success"))
                    {
                        stateSystemService.ChangeNotes(docId, protocolText);
                    }

                    return GetResult(true, fileName, protocolText);
                }
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.ExpandException());
            }
        }

        [Transaction]
        public AjaxFormResult ImportsConsolidation(int docId)
        {
            try
            {
                new PumpWebCons(docId).PumpWebConsData();

                return new AjaxFormResult
                {
                    Success = true,
                    IsUpload = true
                };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                var ms = new AjaxFormResult { IsUpload = true, Success = false };
                ms.ExtraParams["responseText"] = e.Message;
                return ms;
            }
        }

        [Transaction]
        public AjaxFormResult ImportsConsolidationSubjects()
        {
            var result = new AjaxFormResult
            {
                IsUpload = true
            };

            try
            {
                new PumpWebCons().PumpWebConsSubjects();

                result.Success = true;
                result.ExtraParams.Add(new Parameter("msg", "Закачка субъектов выполнена"));

                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);

                result.Success = false;
                result.ExtraParams.Add(new Parameter("msg", "Ошибка при закачке субъектов"));
                result.ExtraParams.Add(new Parameter("responseText", e.ExpandException()));

                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportOKVEDFromExcel(string fileName)
        {
            var warnings = new StringBuilder();

            try
            {
                HSSFSheet sheet = LoadExcelDocument(GetFile()).GetSheetAt(0);
                int count = sheet.LastRowNum - sheet.FirstRowNum;

                var itemList = new List<D_OKVED_OKVED>();
                foreach (HSSFRow row in sheet)
                {
                    progressManager.SetCompleted("Чтение данных из файла...", (double)(row.RowNum + 1) / count);
                    string code = row.GetCell(0).StringCellValue;
                    string name = row.GetCell(1).StringCellValue;
                    DateTime? openDate;
                    try
                    {
                        openDate = row.GetCell(2).DateCellValue;
                    }
                    catch
                    {
                        warnings.AppendLine("Ошибка преобразования даты в строке с кодом {0}".FormatWith(code));
                        continue;
                    }

                    if (code.Trim().IsNotNullOrEmpty())
                    {
                        itemList.Add(
                                    new D_OKVED_OKVED
                                    {
                                        ID = 0,
                                        Code = code,
                                        Name = name,
                                        Section = "n",
                                        OpenDate = openDate
                                    });    
                    }
                }

                itemList.Sort((x, y) => string.Compare(x.Code, y.Code, StringComparison.Ordinal));

                count = 0;
                foreach (D_OKVED_OKVED item in itemList)
                {
                    progressManager.SetCompleted("Обработка данных...", ((double)count++) / itemList.Count);

                    var okved = item;
                    var conflictElements = newRestService.GetItems<D_OKVED_OKVED>().Where(x => x.Code.Equals(okved.Code) && (!x.CloseDate.HasValue || x.CloseDate >= okved.OpenDate));

                    // если нет конфликтующих эелемнтов справочника то вставляем элемент
                    if (!conflictElements.Any())
                    {
                        okved.ParentID = GetParentOkvedId(okved.Code, warnings);

                        newRestService.Save(okved);
                    }
                    else
                    {
                        foreach (var element in conflictElements)
                        {
                            if (!element.CloseDate.HasValue)
                            {
                                if (!element.Name.Equals(okved.Name))
                                {
                                    element.CloseDate = okved.OpenDate.HasValue ? okved.OpenDate.Value.AddDays(-1) : DateTime.Now.Date;

                                    newRestService.Save(element);

                                    okved.ParentID = GetParentOkvedId(okved.Code, warnings);

                                    newRestService.Save(okved);
                                }
                            }
                            else
                            {
                                warnings.AppendLine("Дата закрытия {0} элемента с кодом {1} больше даты открытия {2} импортиремого кода".FormatWith(element.CloseDate, element.Code, item.OpenDate));
                            }
                        }
                    }
                }

                progressManager.SetCompleted("Импорт завершен", 1);
                return GetResult(true, fileName, warnings.ToString());
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return GetResult(false, fileName, e.Message);
            }
        }

        /// <summary>
        /// Получет идентификатор предка
        /// </summary>
        /// <param name="code"> код элемента </param>
        /// <param name="warnings"> Протокол закачки </param>
        /// <returns> идентификатор предка или null если корень</returns>
        private int? GetParentOkvedId(string code, StringBuilder warnings)
        {
            var partsCode = code.Split('.');
            var pertCount = partsCode.Length;

            // если код состоит из одно секции(нет точек-разделителей) то это корневой элемент
            if (pertCount == 1)
            {
                return null;
            }

            // формируем код родителя
            var parentCode = string.Join(".", partsCode.Take(pertCount - 1).ToArray());

            var parentOKVED = newRestService.GetItems<D_OKVED_OKVED>().SingleOrDefault(x => x.Code.Equals(parentCode) && !x.CloseDate.HasValue);

            if (parentOKVED == null)
            {
                warnings.AppendLine("Не найден родительский элемент для импотрируемого элемента с кодом {0}".FormatWith(code));    
            }
            
            return parentOKVED != null ? parentOKVED.ID : (int?)null;
        }

        private string NsiServiceTypeType2DServiceTipYName(nsiServiceType s)
        {
            switch (s.type)
            {
                case "S":
                    return "услуга";
                case "W":
                    return "работа";
                default:
                    throw new NotImplementedException();
            }
        }

        private AjaxFormResult GetResult(bool success, string fileName, string msg = "")
        {
            var message = success ? "Файл '{0}' успешно импортирован.".FormatWith(fileName)
                                : "Ошибка передачи файла '{0}'.".FormatWith(fileName);

            return new AjaxFormResult
                {
                    Success = success,
                    IsUpload = true,
                    ExtraParams =
                        {
                            new Parameter("msg", message),
                            success
                                ? msg.IsNotNullOrEmpty()
                                    ? new Parameter("warning", msg)
                                    : new Parameter("name", fileName)
                                : new Parameter("responseText", msg)
                        }
                };
        }

        private Stream GetFile()
        {
            HttpPostedFileBase file = Request.Files[0];
            if (file == null || file.ContentLength == 0)
            {
                throw new ArgumentNullException("Файл" + " пустой!");
            }

            return file.InputStream;
        }

        private HSSFWorkbook LoadExcelDocument(Stream file)
        {
            try
            {
                return new HSSFWorkbook(file);
            }
            catch (Exception e)
            {
                throw new Exception("Документ не является документом Excel.<br>" + e.Message, e);
            }
        }

        #region Nested type: CachedRegNum

        private class CachedRegNum
        {
            public string RegNum { get; set; }

            public string FullName { get; set; }
        }

        #endregion

        #region Nested type: YaroslavlService

        private class YaroslavlService
        {
            [CsvColumn(Name = "Наименование субъекта РФ", FieldIndex = 1)]
            public string Subject { get; set; }

            [CsvColumn(Name = "Наименование публично-правового образования", FieldIndex = 2)]
            public string Ppo { get; set; }

            [CsvColumn(Name = "Код главы БК", FieldIndex = 3)]
            public string Kbk { get; set; }

            [CsvColumn(Name = "ОКАТО", FieldIndex = 4)]
            public string Okato { get; set; }

            [CsvColumn(Name = "Присвоенный номер услуги", FieldIndex = 5)]
            public string Code { get; set; }

            [CsvColumn(Name = "Вид", FieldIndex = 6)]
            public string EnactmentType { get; set; }

            [CsvColumn(Name = "Наименование органа", FieldIndex = 7)]
            public string EnactmentFounder { get; set; }

            [CsvColumn(Name = "Дата", FieldIndex = 8)]
            public DateTime EnactmentDate { get; set; }

            [CsvColumn(Name = "Номер", FieldIndex = 9)]
            public string EnactmentNumber { get; set; }

            [CsvColumn(Name = "Наименование", FieldIndex = 10)]
            public string EnactmentName { get; set; }

            [CsvColumn(Name = "Вид (услуга/работа)", FieldIndex = 11)]
            public string Type { get; set; }

            [CsvColumn(Name = "№ (согласно ведомственному перечню услуг)", FieldIndex = 12)]
            public string Number { get; set; }

            [CsvColumn(Name = "Наименование государственной услуги (работы)", FieldIndex = 13)]
            public string Name { get; set; }

            [CsvColumn(Name = "Категории потребителей государственной услуги (работы)", FieldIndex = 14)]
            public string Category { get; set; }

            [CsvColumn(Name = "Показатель 1", FieldIndex = 15)]
            public string IndexName1 { get; set; }

            [CsvColumn(FieldIndex = 16)]
            public string IndexUnit1 { get; set; }

            [CsvColumn(Name = "Показатель 2", FieldIndex = 17)]
            public string IndexName2 { get; set; }

            [CsvColumn(FieldIndex = 18)]
            public string IndexUnit2 { get; set; }

            [CsvColumn(Name = "Показатель 3", FieldIndex = 19)]
            public string IndexName3 { get; set; }

            [CsvColumn(FieldIndex = 20)]
            public string IndexUnit3 { get; set; }

            [CsvColumn(Name = "Показатель 4", FieldIndex = 21)]
            public string IndexName4 { get; set; }

            [CsvColumn(FieldIndex = 22)]
            public string IndexUnit4 { get; set; }

            [CsvColumn(Name = "Показатель 5", FieldIndex = 23)]
            public string IndexName5 { get; set; }

            [CsvColumn(FieldIndex = 24)]
            public string IndexUnit5 { get; set; }

            [CsvColumn(Name = "Показатель 6", FieldIndex = 25)]
            public string IndexName6 { get; set; }

            [CsvColumn(FieldIndex = 26)]
            public string IndexUnit6 { get; set; }

            [CsvColumn(Name = "Показатель 7", FieldIndex = 27)]
            public string IndexName7 { get; set; }

            [CsvColumn(FieldIndex = 28)]
            public string IndexUnit7 { get; set; }

            [CsvColumn(Name = "Показатель 8", FieldIndex = 29)]
            public string IndexName8 { get; set; }

            [CsvColumn(FieldIndex = 30)]
            public string IndexUnit8 { get; set; }
        }

        #endregion
    }
}