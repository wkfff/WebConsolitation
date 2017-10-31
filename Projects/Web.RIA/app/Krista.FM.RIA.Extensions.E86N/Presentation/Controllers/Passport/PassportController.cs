using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Passport
{
    public class PassportController : RestBaseController<F_Org_Passport>
    {
        private readonly ILinqRepository<D_Org_Category> catYhRepository;

        private readonly ILinqRepository<F_F_ParameterDoc> docRepository;

        private readonly IChangeLogService logService;

        private readonly ILinqRepository<D_OKATO_OKATO> okatoRepository;

        private readonly ILinqRepository<D_OKFS_OKFS> okfsRepository;

        private readonly ILinqRepository<D_OKOPF_OKOPF> okopfRepository;

        private readonly ILinqRepository<D_OKTMO_OKTMO> oktmoRepository;

        private readonly ILinqRepository<F_F_ParameterDoc> parameterDoc;

        private readonly ILinqRepository<D_OrgGen_Raspor> rasporRepository;

        private readonly ILinqRepository<FX_Org_SostD> sostDRepository;

        private readonly ILinqRepository<D_Org_VidOrg> vidOrgRepository;

        public PassportController(
            ILinqRepository<F_Org_Passport> passport, 
            ILinqRepository<D_Org_Category> ogsRepository, 
            ILinqRepository<D_OKATO_OKATO> okatoRepository, 
            ILinqRepository<D_OKOPF_OKOPF> okopfRepository, 
            ILinqRepository<D_OKTMO_OKTMO> oktmoRepository, 
            ILinqRepository<D_OrgGen_Raspor> rasporRepository, 
            ILinqRepository<D_Org_VidOrg> vidorgRepository, 
            ILinqRepository<D_OKFS_OKFS> okfsRepository, 
            ILinqRepository<FX_Org_SostD> sostDRepository, 
            ILinqRepository<F_F_ParameterDoc> docRepository, 
            ILinqRepository<F_F_ParameterDoc> parameterDoc)
        {
            TableRepository = passport;
            this.parameterDoc = parameterDoc;
            catYhRepository = ogsRepository;
            this.okatoRepository = okatoRepository;
            this.okfsRepository = okfsRepository;
            this.okopfRepository = okopfRepository;
            this.oktmoRepository = oktmoRepository;
            this.rasporRepository = rasporRepository;
            vidOrgRepository = vidorgRepository;
            this.docRepository = docRepository;
            this.sostDRepository = sostDRepository;
            logService = Resolver.Get<IChangeLogService>();
        }

        public ActionResult Load(int docId)
        {
            var passport = new List<OgsViewModel>(
                from p in TableRepository.FindAll()
                where p.RefParametr.ID == docId
                select new OgsViewModel
                    {
                        ID = p.ID, 
                        RefOrgPpoName = p.RefParametr.RefUchr.RefOrgPPO.Name, 
                        RefTipYcName = p.RefParametr.RefUchr.RefTipYc.Name, 
                        RefOrgGrbsName = p.RefParametr.RefUchr.RefOrgGRBS.Name, 
                        Name = p.RefParametr.RefUchr.Name, 
                        ShortName = p.RefParametr.RefUchr.ShortName, 
                        Inn = p.RefParametr.RefUchr.INN, 
                        Kpp = p.RefParametr.RefUchr.KPP, 
                        RefRaspor = p.RefRaspor.ID, 
                        RefRasporName = p.RefRaspor.Name, 
                        Ogrn = p.OGRN, 
                        RefCatYh = p.RefCateg.Code, 
                        RefCatYhName = p.RefCateg.Name, 
                        RefVid = p.RefVid.ID, 
                        RefVidName = p.RefVid.Name, 
                        RefOkato = p.RefOKATO.ID, 
                        RefOkatoCode = p.RefOKATO.Code, 
                        RefOkatoName = p.RefOKATO.Name, 
                        RefOkfs = p.RefOKFS.ID, 
                        RefOkfsCode = p.RefOKFS.Code, 
                        RefOkfsName = p.RefOKFS.Name, 
                        RefOktmo = p.RefOKTMO.ID, 
                        RefOktmoCode = p.RefOKTMO.Code, 
                        RefOktmoName = p.RefOKTMO.Name, 
                        RefOkopf = p.RefOKOPF.ID, 
                        RefOkopfCode = p.RefOKOPF.Code5Zn, 
                        RefOkopfName = p.RefOKOPF.Name, 
                        Okpo = p.OKPO, 
                        Adr = p.Adr, 
                        Website = p.Website, 
                        Phone = p.Phone, 
                        Mail = p.Mail, 
                        Fam = p.Fam, 
                        NameRuc = p.NameRuc, 
                        Otch = p.Otch, 
                        Ordinary = p.Ordinary, 
                        Indeks = p.Indeks, 
                        RefSost = p.RefParametr.RefSost.ID, 
                        RefSostName = p.RefParametr.RefSost.Name, 
                        OpeningDate = p.RefParametr.OpeningDate.HasValue ? p.RefParametr.OpeningDate.Value.ToString("dd.MM.yyyy") : string.Empty, 
                        CloseDate = p.RefParametr.CloseDate.HasValue ? p.RefParametr.CloseDate.Value.ToString("dd.MM.yyyy") : string.Empty
                    });

            OgsViewModel data;
            if (passport.Count > 0)
            {
                data = passport.First();
                if (data.RefOkato != null)
                {
                    data.RefOkatoName = data.RefOkatoCode + "; " + data.RefOkatoName + ";";
                }

                if (data.RefOktmo != null)
                {
                    data.RefOktmoName = data.RefOktmoName + "; " + data.RefOktmoCode + ";";
                }

                if (data.RefOkfs != null)
                {
                    data.RefOkfsName = data.RefOkfsName + "; " + data.RefOkfsCode + ";";
                }

                if (data.RefOkopf != null)
                {
                    data.RefOkopfName = data.RefOkopfName + "; " + string.Format("{0:00000}", data.RefOkopfCode) + ";";
                }
            }
            else
            {
                var doc = (from p in docRepository.FindAll()
                           where p.ID == docId
                           select new
                               {
                                   p.ID, 
                                   RefUchrName = p.RefUchr.Name, 
                                   RefUchrShortName = p.RefUchr.ShortName, 
                                   RefUchrKpp = p.RefUchr.KPP, 
                                   RefUchrInn = p.RefUchr.INN, 
                                   RefOrgPpoName = p.RefUchr.RefOrgPPO.Name, 
                                   RegOrfGRBSName = p.RefUchr.RefOrgGRBS.Name, 
                                   RefTipYcName = p.RefUchr.RefTipYc.Name, 
                                   RefSost = p.RefSost.ID, 
                                   RefSostName = p.RefSost.Name
                               }).ToList().First();

                data = new OgsViewModel
                    {
                        ID = -1, 
                        RefOrgPpoName = doc.RefOrgPpoName, 
                        RefTipYcName = doc.RefTipYcName, 
                        RefOrgGrbsName = doc.RegOrfGRBSName, 
                        Name = doc.RefUchrName, 
                        ShortName = doc.RefUchrShortName, 
                        Inn = doc.RefUchrInn, 
                        Kpp = doc.RefUchrKpp, 
                        RefSost = doc.RefSost, 
                        RefSostName = doc.RefSostName
                    };
            }

            return new AjaxStoreResult(data, 1);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public override RestResult Update(int id, string data)
        {
            logService.WriteChangeDocDetail(TableRepository.Load(id).RefParametr);
            return base.Update(id, data);
        }

        [HttpDelete]
        [Transaction]
        public override RestResult Destroy(int id)
        {
            logService.WriteDeleteDocDetail(TableRepository.Load(id).RefParametr);
            return base.Destroy(id);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult Save(FormCollection values, int docId)
        {
            var result = new AjaxFormResult();
            try
            {
                int id = Convert.ToInt32(values["ID"]);
                F_Org_Passport record;
                if (id == -1)
                {
                    record = new F_Org_Passport
                        {
                            ID = 0, 
                            TaskID = 0, 
                            SourceID = 0
                        };
                }
                else
                {
                    record = TableRepository.FindOne(id);
                }

                record.Adr = values["Adr"];
                record.Fam = values["Fam"];
                record.Indeks = values["Indeks"];
                record.NameRuc = values["NameRuc"];
                record.OGRN = values["Ogrn"];
                record.Ordinary = values["Ordinary"];
                record.Otch = values["Otch"];
                record.Phone = values["Phone"];
                record.Website = values["Website"];
                record.Mail = values["Mail"];

                if (values["comboRefCatYh_SelIndex"] != "-1")
                {
                    record.RefCateg = catYhRepository.FindOne(Convert.ToInt32(values["comboRefCatYh_Value"]));
                }

                string val = values["RefOkato"];
                record.RefOKATO = val.Length != 0 ? okatoRepository.FindOne(Convert.ToInt32(val)) : null;

                val = values["RefOkfs"];
                record.RefOKFS = val.Length != 0 ? okfsRepository.FindOne(Convert.ToInt32(val)) : null;

                val = values["RefOkopf"];
                record.RefOKOPF = val.Length != 0 ? okopfRepository.FindOne(Convert.ToInt32(val)) : null;

                record.OKPO = values["Okpo"];

                val = values["RefOktmo"];
                record.RefOKTMO = val.Length != 0 ? oktmoRepository.FindOne(Convert.ToInt32(val)) : null;

                record.RefParametr = docRepository.FindOne(docId);
                record.RefParametr.RefSost = sostDRepository.FindOne(Convert.ToInt32(values["RefSostID"]));

                val = values["RefRaspor"];
                record.RefRaspor = val.Length != 0 ? rasporRepository.FindOne(Convert.ToInt32(val)) : null;

                val = values["RefVid"];
                record.RefVid = val.Length != 0 ? vidOrgRepository.FindOne(Convert.ToInt32(val)) : null;

                if (record.ID == 0)
                {
                    TableRepository.Save(record);
                    result.ExtraParams["newID"] = record.ID.ToString(CultureInfo.InvariantCulture);
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                logService.WriteChangeDocDetail(record.RefParametr);
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        public override void ConfigUpdates()
        {
        }

        [Transaction]
        public AjaxStoreResult InstitutionSorts(int limit, int start, string query)
        {
            var servise = new NewRestService().GetItems<D_Org_VidOrg>().Where(x => x.BusinessStatus.Equals("801"));
            var list = servise.Where(x => x.ParentID != x.ID).Select(x => x.ParentID).Distinct().ToList();
            var data = servise.Where(x => !list.Contains(x.ID)).Where(x => x.Name.Contains(query) || x.Code.Contains(query)).OrderBy(x => x.Code);
            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }

        [HttpPost]
        [Transaction]
        public RestResult CheckIfCanDocumentCopy(int recId)
        {
            if (!Resolver.Get<IVersioningService>().CheckCloseDocs(recId))
            {
                return new RestResult
                    {
                        Success = false, 
                        Message = "Нет закрытых документов"
                    };
            }

            var passports = TableRepository.FindAll()
                .Where(
                    x =>
                    x.RefParametr.ID == recId).ToList();

            if (passports.Count == 0)
            {
                return new RestResult
                    {
                        Success = true, 
                        Message = "Документ пуст"
                    };
            }

            return new RestResult
                {
                    Success = false, 
                    Message = "Документ не пуст"
                };
        }

        [HttpPost]
        [Transaction]
        public RestResult CopyContent(int recId)
        {
            var formData = parameterDoc.FindAll().First(x => x.ID == recId);

            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId).ID;
            try
            {
                var passports = TableRepository.FindAll()
                    .Where(
                        x =>
                        x.RefParametr.ID == idOfLastDoc).ToList();
                if (passports.Count > 0)
                {
                    var passport = new F_Org_Passport
                        {
                            SourceID = passports.First().SourceID, 
                            TaskID = passports.First().TaskID, 
                            OGRN = passports.First().OGRN, 
                            Fam = passports.First().Fam, 
                            NameRuc = passports.First().NameRuc, 
                            Otch = passports.First().Otch, 
                            Ordinary = passports.First().Ordinary, 
                            Adr = passports.First().Adr, 
                            Indeks = passports.First().Indeks, 
                            Website = passports.First().Website, 
                            OKPO = passports.First().OKPO, 
                            Phone = passports.First().Phone, 
                            Mail = passports.First().Mail, 
                            RefOKOPF = passports.First().RefOKOPF, 
                            RefOKATO = passports.First().RefOKATO, 
                            RefCateg = passports.First().RefCateg, 
                            RefOKTMO = passports.First().RefOKTMO, 
                            RefVid = passports.First().RefVid, 
                            RefOKFS = passports.First().RefOKFS, 
                            RefRaspor = passports.First().RefRaspor, 
                            RefParametr = formData
                        };

                    formData.Passports.Add(passport);

                    var founderRepository = Resolver.Get<ILinqRepository<F_F_Founder>>();
                    var founders =
                        founderRepository.FindAll().Where(x => x.RefPassport.RefParametr.ID == idOfLastDoc).ToList();
                    foreach (var founder in founders)
                    {
                        var item = new F_F_Founder
                            {
                                SourceID = founder.SourceID, 
                                TaskID = founder.TaskID, 
                                formative = founder.formative, 
                                stateTask = founder.stateTask, 
                                supervisoryBoard = founder.supervisoryBoard, 
                                RefPassport = passport, 
                                RefYchred = founder.RefYchred,
                                financeSupply = founder.financeSupply,
                                manageProperty = founder.manageProperty
                            };
                        founderRepository.Save(item);
                    }

                    var okvedyRepository = Resolver.Get<ILinqRepository<F_F_OKVEDY>>();
                    var okvedys = okvedyRepository.FindAll().Where(x => x.RefPassport.RefParametr.ID == idOfLastDoc).ToList();
                    foreach (var okvedy in okvedys)
                    {
                        var item = new F_F_OKVEDY
                            {
                                SourceID = okvedy.SourceID, 
                                TaskID = okvedy.TaskID, 
                                Name = okvedy.Name, 
                                RefPrOkved = okvedy.RefPrOkved, 
                                RefOKVED = okvedy.RefOKVED, 
                                RefPassport = passport
                            };
                        okvedyRepository.Save(item);
                    }

                    var filialRepository = Resolver.Get<ILinqRepository<F_F_Filial>>();
                    var filials = filialRepository.FindAll().Where(x => x.RefPassport.RefParametr.ID == idOfLastDoc).ToList();
                    foreach (var filial in filials)
                    {
                        var item = new F_F_Filial
                            {
                                SourceID = filial.SourceID, 
                                TaskID = filial.TaskID, 
                                Code = filial.Code, 
                                Name = filial.Name, 
                                Nameshot = filial.Nameshot, 
                                KPP = filial.KPP, 
                                INN = filial.INN, 
                                RefTipFi = filial.RefTipFi, 
                                RefPassport = passport
                            };
                        filialRepository.Save(item);
                    }

                    TableRepository.Save(passport);
                }

                parameterDoc.Save(formData);
                parameterDoc.DbContext.CommitChanges();

                logService.WriteChangeDocDetail(formData);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Данные скопированы"
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
