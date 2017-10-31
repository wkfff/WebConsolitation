using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.OGSService
{
    public sealed class OGSService : IOGSService
    {
        private readonly ILinqRepository<F_F_OKVEDY> activityRepository;
        private readonly ILinqRepository<F_F_Filial> branchesRepository;
        private readonly ILinqRepository<F_Doc_Docum> documentRepository;
        private readonly ILinqRepository<F_F_Founder> founderepository;
        private readonly INewRestService newRestService;
        private readonly ILinqRepository<D_Fin_nsiBudget> nsiBudgetRepository;
        private readonly ILinqRepository<D_Org_NsiOGS> nsiOgsRepository;
        private readonly ILinqRepository<D_Org_Structure> ogsRepository;
        private readonly ILinqRepository<D_OKATO_OKATO> okatoRepository;
        private readonly ILinqRepository<D_OKOPF_OKOPF> okopfRepository;
        private readonly ILinqRepository<D_OKTMO_OKTMO> oktmoRepository;
        private readonly ILinqRepository<D_OKVED_OKVED> okvedRepository;
        private readonly ILinqRepository<D_Org_OrgYchr> orgYchrRepository;
        private readonly ILinqRepository<F_F_ParameterDoc> parameterDocRepository;
        private readonly ILinqRepository<FX_FX_PartDoc> partDocRepository;
        private readonly ILinqRepository<F_Org_Passport> passportRepository;
        private readonly ILinqRepository<D_Org_PPO> ppoRepository;
        private readonly ILinqRepository<D_Org_PrOKVED> priznakOKVEDRepository;
        private readonly ILinqRepository<FX_Org_SostD> sostRepository;
        private readonly ILinqRepository<FX_Org_TipYch> tipYchRepository;
        private readonly ILinqRepository<D_Org_VidOrg> vidOrgRepository;
        private readonly ILinqRepository<FX_Fin_YearForm> yearFormRepository;

        public OGSService(
            ILinqRepository<D_Org_Structure> ogsRepository, 
            ILinqRepository<F_F_OKVEDY> activityRepository, 
            ILinqRepository<F_F_Filial> branchesRepository, 
            ILinqRepository<FX_Org_SostD> sostRepository, 
            ILinqRepository<FX_Org_TipYch> tipYchRepository, 
            ILinqRepository<D_Org_PPO> ppoRepository, 
            ILinqRepository<F_Doc_Docum> documentRepository, 
            ILinqRepository<D_OKATO_OKATO> okatoRepository, 
            ILinqRepository<D_Org_VidOrg> vidOrgRepository, 
            ILinqRepository<D_OKVED_OKVED> okvedRepository, 
            ILinqRepository<D_OKTMO_OKTMO> oktmoRepository, 
            ILinqRepository<D_OKOPF_OKOPF> okopfRepository, 
            ILinqRepository<D_Org_OrgYchr> orgYchrRepository, 
            ILinqRepository<D_Org_NsiOGS> nsiOgsRepository, 
            ILinqRepository<FX_FX_PartDoc> partDocRepository, 
            ILinqRepository<FX_Fin_YearForm> yearFormRepository, 
            ILinqRepository<F_F_ParameterDoc> parameterDocRepository, 
            ILinqRepository<D_Org_PrOKVED> priznakOKVEDRepository, 
            ILinqRepository<D_Fin_nsiBudget> nsiBudgetRepository, 
            ILinqRepository<F_Org_Passport> passportRepository, 
            ILinqRepository<F_F_Founder> founderepository)
        {
            this.ogsRepository = ogsRepository;
            this.branchesRepository = branchesRepository;
            this.activityRepository = activityRepository;
            this.documentRepository = documentRepository;
            this.sostRepository = sostRepository;
            this.tipYchRepository = tipYchRepository;
            this.ppoRepository = ppoRepository;
            this.passportRepository = passportRepository;
            this.okatoRepository = okatoRepository;
            this.vidOrgRepository = vidOrgRepository;
            this.okvedRepository = okvedRepository;
            this.oktmoRepository = oktmoRepository;
            this.okopfRepository = okopfRepository;
            this.orgYchrRepository = orgYchrRepository;
            this.nsiOgsRepository = nsiOgsRepository;
            this.partDocRepository = partDocRepository;
            this.yearFormRepository = yearFormRepository;
            this.parameterDocRepository = parameterDocRepository;
            this.passportRepository = passportRepository;
            this.priznakOKVEDRepository = priznakOKVEDRepository;
            this.nsiBudgetRepository = nsiBudgetRepository;
            this.founderepository = founderepository;

            newRestService = Resolver.Get<INewRestService>();
        }

        #region IOGSService Members

        /// <summary>
        ///   Добавление учреждения
        /// </summary>
        [Transaction]
        public D_Org_Structure AddOrg(ref D_Org_Structure record)
        {
            record.ID = 0;

            ogsRepository.Save(record);
            return record;
        }

        // public void ImportFile( /*XDocument xmlFile*/ XmlTextReader xmlFile)
        // {
        // try
        // {
        // string idFile = string.Empty;
        // var okvedy = new List<Okved>();
        // var rec = new D_Org_Structure();
        // var recpassport = new F_Org_Passport();
        // xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
        // while (xmlFile.Read())
        // {
        // if (xmlFile.NodeType == XmlNodeType.Element)
        // switch (xmlFile.Name)
        // {
        // case "HEADER":
        // idFile = xmlFile.GetAttribute("IDFILE");
        // break;
        // case "UL":
        // rec.INN = xmlFile.GetAttribute("INN");
        // rec.KPP = xmlFile.GetAttribute("KPP");
        // recpassport.OGRN = xmlFile.GetAttribute("OGRN");
        // break;
        // case "OKVED":
        // Okved okved;
        // //okved.KOD_OKVED = xmlFile.GetAttribute("KOD_OKVED").Replace(".", "");
        // okved.KodOkved = xmlFile.GetAttribute("KOD_OKVED");
        // okved.Main = xmlFile.GetAttribute("MAIN");
        // okved.Name = xmlFile.GetAttribute("NAME");
        // okvedy.Add(okved);
        // break;
        // case "UL_NAME":
        // rec.Name = xmlFile.GetAttribute("NAMEP");
        // rec.ShortName = xmlFile.GetAttribute("NAMES");
        // break;
        // case "OPF":
        // if (xmlFile.GetAttribute("SPR") == "OKOPF")
        // recpassport.RefOKOPF =
        // okopfRepository.FindOne(FindOkopf(xmlFile.GetAttribute("KOD_OPF")));
        // break;
        // case "UCHR":
        // DateTime dtstart = DateTime.MinValue;
        // string ogrn = string.Empty, namep = string.Empty;
        // while (xmlFile.Read())
        // {
        // if ((xmlFile.NodeType == XmlNodeType.Element)
        // && (xmlFile.Name == "RUL"))
        // {
        // if (dtstart < Convert.ToDateTime(xmlFile.GetAttribute("DTSTART")))
        // {
        // dtstart = Convert.ToDateTime(xmlFile.GetAttribute("DTSTART"));
        // ogrn = xmlFile.GetAttribute("OGRN");
        // namep = xmlFile.GetAttribute("NAMEP");
        // }
        // }

        // if ((xmlFile.NodeType == XmlNodeType.EndElement) &&
        // (xmlFile.Name == "UCHR"))
        // break;
        // }

        // //если в учредителях более одной записи с одинаковым именем то будет вываливаться эгзепшен
        // D_Org_OrgYchr orgUchrID =
        // orgYchrRepository.FindAll().SingleOrDefault(p => p.Name == namep);
        // if (orgUchrID == null)
        // {
        // var recOrgUchr = new D_Org_OrgYchr {ID = 0};
        // if (string.IsNullOrEmpty(ogrn))
        // {
        // ogrn = "Нет кода";
        // }
        // recOrgUchr.Code = ogrn;
        // recOrgUchr.Name = namep;
        // if (
        // !(string.IsNullOrEmpty(recOrgUchr.Name) ||
        // string.IsNullOrEmpty(recOrgUchr.Code)))
        // {
        // orgYchrRepository.Save(recOrgUchr);
        // orgUchrID = recOrgUchr;
        // }
        // }
        // else
        // {
        // if (orgUchrID.Name != namep)
        // {
        // orgUchrID.Name = namep;
        // }
        // }

        ////                                recpassport.RefYchred = OrgUchrID;

        // break;
        // case "DOLGNFL":
        // recpassport.Ordinary = xmlFile.GetAttribute("DOLGN");
        // break;
        // case "FL":
        // recpassport.Fam = xmlFile.GetAttribute("FAM_FL");
        // recpassport.NameRuc = xmlFile.GetAttribute("NAME_FL");
        // recpassport.Otch = xmlFile.GetAttribute("OTCH_FL");
        // break;
        // case "UL_ADDRESS":
        // string dom = "",
        // region = "", 
        // raion = "", 
        // gorod = "", 
        // street = "",
        // korp = "",
        // kvart = "";
        // while (xmlFile.Read())
        // {
        // if (xmlFile.NodeType == XmlNodeType.Element)
        // switch (xmlFile.Name)
        // {
        // case "ADDRESS":
        // recpassport.Indeks = xmlFile.GetAttribute("INDEKS");
        // int okatoid = FindOkato(xmlFile.GetAttribute("OKATO"));
        // if (okatoid != -1)
        // recpassport.RefOKATO = okatoRepository.FindOne(okatoid);
        // dom = xmlFile.GetAttribute("DOM");

        // korp = xmlFile.GetAttribute("KORP");
        // if ((korp == "-") || (korp == "0"))
        // korp = "";

        // kvart = xmlFile.GetAttribute("KVART");
        // if ((kvart == "-") || (kvart == "0"))
        // kvart = "";
        // break;
        // case "REGION":
        // region = xmlFile.GetAttribute("NAME") + ",";
        // break;
        // case "RAION":
        // raion = xmlFile.GetAttribute("NAME") + ",";
        // break;
        // case "GOROD":
        // gorod = xmlFile.GetAttribute("NAME") + ",";
        // break;
        // case "NASPUNKT":
        // gorod = xmlFile.GetAttribute("NAME") + ",";
        // break;
        // case "STREET":
        // street = xmlFile.GetAttribute("NAME") + ",";
        // break;
        // case "CONTACT":
        // if (!string.IsNullOrEmpty(xmlFile.GetAttribute("KODGOROD")))
        // recpassport.Phone = xmlFile.GetAttribute("KODGOROD") + "-" +
        // xmlFile.GetAttribute("TELEFON");
        // else recpassport.Phone = xmlFile.GetAttribute("TELEFON");
        // break;
        // }

        // if ((xmlFile.NodeType == XmlNodeType.EndElement) &&
        // (xmlFile.Name == "UL_ADDRESS"))
        // break;
        // }

        // recpassport.Adr = "{0} {1} {2} {3} {4}".FormatWith(region, raion, gorod, street, dom);
        // if (!string.IsNullOrEmpty(korp))
        // recpassport.Adr += "," + korp;
        // if (!string.IsNullOrEmpty(kvart))
        // recpassport.Adr += "," + kvart;
        // break;
        // }

        // if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "UL"))
        // {
        // //Организация может оказаться без значений ИНН/КПП (прецеденты были), такую записывать нельзя.
        // if (string.IsNullOrEmpty(rec.INN) || string.IsNullOrEmpty(rec.KPP))
        // {
        // //пропускаем организацию, так как у нее нет ИНН/КПП
        // }
        // else
        // {
        // if (string.IsNullOrEmpty(rec.Name)) rec.Name = "Не указано наименование";
        // if (string.IsNullOrEmpty(rec.ShortName))
        // rec.ShortName = "Не указано сокращенное наименование";
        // int uchrID = FindOrg(rec.INN, rec.KPP);
        // if (uchrID == -1)
        // {
        // AddOrg(ref rec);
        // }
        // else
        // {
        // rec.ID = uchrID;
        // D_Org_Structure org = ogsRepository.FindOne(uchrID);
        // org.Name = rec.Name;
        // org.ShortName = rec.ShortName;
        // }

        // var data = from p in passportRepository.FindAll()
        // where /*(p.RefParametr.PlanThreeYear == false) &&*/
        // (p.RefParametr.RefUchr.ID == rec.ID) &&
        // (p.RefParametr.RefPartDoc.ID == 1) &&
        // (p.RefParametr.RefYearForm.ID == DateTime.Now.Year)
        // select new
        // {
        // p.ID,
        // Sost = p.RefParametr.RefSost.ID
        // };

        // if (string.IsNullOrEmpty(recpassport.OGRN)) recpassport.OGRN = "Не указан";
        // if (string.IsNullOrEmpty(recpassport.Ordinary)) recpassport.Ordinary = "Не указан";
        // if (string.IsNullOrEmpty(recpassport.Fam)) recpassport.Fam = "Не указан";
        // if (string.IsNullOrEmpty(recpassport.NameRuc)) recpassport.NameRuc = "Не указан";
        // if (string.IsNullOrEmpty(recpassport.Otch)) recpassport.Otch = "Не указан";
        // if (string.IsNullOrEmpty(recpassport.Indeks)) recpassport.Indeks = "Нет";
        // if (string.IsNullOrEmpty(recpassport.Phone)) recpassport.Phone = "Не указан";

        // if (data.Any())
        // {
        ////если паспорт есть
        // //if (data.First().Sost != 8) // если паспорт не экспортирован то обновляем
        // {
        // recpassport.ID = data.First().ID;
        // F_Org_Passport passport = passportRepository.FindOne(recpassport.ID);
        // if (recpassport.RefOKATO != null)
        // passport.RefOKATO = recpassport.RefOKATO;
        // passport.RefOKOPF = recpassport.RefOKOPF;
        // //passport.Phone = recpassport.Phone;
        // passport.Fam = recpassport.Fam;
        // passport.NameRuc = recpassport.NameRuc;
        // passport.Otch = recpassport.Otch;
        // passport.Ordinary = recpassport.Ordinary;
        ////                                    passport.RefYchred = recpassport.RefYchred;
        // passport.OGRN = recpassport.OGRN;
        // passport.Indeks = recpassport.Indeks;
        // passport.Adr = recpassport.Adr;

        // //удаляем старые виды деятельности
        // var activity = from p in activityRepository.FindAll()
        // where (p.RefPassport.ID == recpassport.ID)
        // select new
        // {
        // p.ID,
        // };
        // foreach (var item in activity)
        // {
        // activityRepository.Delete(activityRepository.FindOne(item.ID));
        // }

        // passport.RefParametr
        // .If(doc => doc.RefSost.ID == 8)
        // .Do(doc =>
        // {
        // doc.Note = string.Format("Требуется повторный экспорт." + 
        // " Изменения на основании файла ЕГРЮЛ - 'IDFILE' <{0}>", idFile);
        // doc.RefSost = sostRepository.FindOne(7); //Завершено
        // })
        // .Do(doc => parameterDocRepository.Save(doc));
        // }
        // }
        // else
        // {
        // var doc = new F_F_ParameterDoc
        // {
        // ID = 0,
        // Note = "Данные из ЕГРЮЛ",
        // PlanThreeYear = false,
        // RefUchr = ogsRepository.FindOne(rec.ID),
        // RefPartDoc = partDocRepository.FindOne(1),
        // RefSost = sostRepository.FindOne(2),
        // RefYearForm = yearFormRepository.FindOne(DateTime.Now.Year)
        // };

        // parameterDocRepository.Save(doc);

        // recpassport.ID = 0;
        // recpassport.TaskID = 0;
        // recpassport.SourceID = 0;
        // recpassport.RefParametr = parameterDocRepository.FindOne(doc.ID);

        // passportRepository.Save(recpassport);
        // }

        // // если паспорт не экспортирован или создан новый паспорт то обновляем
        // //if ((data.Count() == 0) || (data.First().Sost != 8))
        // {
        // foreach (Okved item in okvedy)
        // {
        // var recokved = new F_F_OKVEDY
        // {
        // ID = 0,
        // SourceID = 0,
        // TaskID = 0,
        // RefPassport = passportRepository.FindOne(recpassport.ID),
        // Name = item.Name,
        // RefOKVED = okvedRepository.FindOne(FindOkved(item.KodOkved)),
        // RefPrOkved = prOkvedRepository.FindOne(item.Main == "1"
        // ? FindPrOkved(1)
        // : FindPrOkved(2))
        // };
        // activityRepository.Save(recokved);
        // }
        // }

        // okvedy.Clear();
        // recpassport = null;
        // //doc = null;
        // rec = null;
        // rec = new D_Org_Structure();
        // recpassport = new F_Org_Passport();
        // }
        // }
        // }
        // }
        // finally
        // {
        // if (xmlFile != null)
        // xmlFile.Close();
        // }
        // }

        /// <summary>
        ///   Закачка учреждений и паспортов из ЕГРЮЛ
        /// </summary>
        public void ImportFile(XmlTextReader xmlFile)
        {
            try
            {
                string idFile = string.Empty;
                var okvedy = new List<Okved>();
                var founders = new List<D_Org_OrgYchr>();
                var rec = new D_Org_Structure();
                var recpassport = new F_Org_Passport();
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                var difference = new StringBuilder();
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "HEADER":
                                idFile = xmlFile.GetAttribute("IDFILE");
                                break;
                            case "UL":
                                rec.INN = xmlFile.GetAttribute("INN");
                                rec.KPP = xmlFile.GetAttribute("KPP");
                                recpassport.OGRN = xmlFile.GetAttribute("OGRN");
                                break;
                            case "OKVED":
                                Okved okved;

                                // okved.KOD_OKVED = xmlFile.GetAttribute("KOD_OKVED").Replace(".", "");
                                okved.KodOkved = xmlFile.GetAttribute("KOD_OKVED");
                                okved.Main = xmlFile.GetAttribute("MAIN");
                                okved.Name = xmlFile.GetAttribute("NAME");
                                okvedy.Add(okved);
                                break;
                            case "UL_NAME":
                                rec.Name = xmlFile.GetAttribute("NAMEP");
                                rec.ShortName = xmlFile.GetAttribute("NAMES");
                                break;
                            case "OPF":
                                if (xmlFile.GetAttribute("SPR") == "OKOPF")
                                {
                                    recpassport.RefOKOPF =
                                        okopfRepository.FindOne(FindOkopf(xmlFile.GetAttribute("KOD_OPF")));
                                }

                                break;
                            case "RUL":
                                ProcessEgrulFounder(xmlFile, founders);
                                break;
                            case "DOLGNFL":
                                recpassport.Ordinary = xmlFile.GetAttribute("DOLGN");
                                break;
                            case "FL":
                                recpassport.Fam = xmlFile.GetAttribute("FAM_FL");
                                recpassport.NameRuc = xmlFile.GetAttribute("NAME_FL");
                                recpassport.Otch = xmlFile.GetAttribute("OTCH_FL");
                                break;
                            case "UL_ADDRESS":
                                string dom = string.Empty, 
                                       region = string.Empty, 
                                       raion = string.Empty, 
                                       gorod = string.Empty, 
                                       street = string.Empty, 
                                       korp = string.Empty, 
                                       kvart = string.Empty;
                                while (xmlFile.Read())
                                {
                                    if (xmlFile.NodeType == XmlNodeType.Element)
                                    {
                                        switch (xmlFile.Name)
                                        {
                                            case "ADDRESS":
                                                recpassport.Indeks = xmlFile.GetAttribute("INDEKS");
                                                int okatoid = FindOkato(xmlFile.GetAttribute("OKATO"));
                                                if (okatoid != -1)
                                                {
                                                    recpassport.RefOKATO = okatoRepository.FindOne(okatoid);
                                                }

                                                dom = xmlFile.GetAttribute("DOM");

                                                korp = xmlFile.GetAttribute("KORP");
                                                if ((korp == "-") || (korp == "0"))
                                                {
                                                    korp = string.Empty;
                                                }

                                                kvart = xmlFile.GetAttribute("KVART");
                                                if ((kvart == "-") || (kvart == "0"))
                                                {
                                                    kvart = string.Empty;
                                                }

                                                break;
                                            case "REGION":
                                                region = xmlFile.GetAttribute("NAME") + ",";
                                                break;
                                            case "RAION":
                                                raion = xmlFile.GetAttribute("NAME") + ",";
                                                break;
                                            case "GOROD":
                                                gorod = xmlFile.GetAttribute("NAME") + ",";
                                                break;
                                            case "NASPUNKT":
                                                gorod = xmlFile.GetAttribute("NAME") + ",";
                                                break;
                                            case "STREET":
                                                street = xmlFile.GetAttribute("NAME") + ",";
                                                break;
                                            case "CONTACT":
                                                if (!string.IsNullOrEmpty(xmlFile.GetAttribute("KODGOROD")))
                                                {
                                                    recpassport.Phone = xmlFile.GetAttribute("KODGOROD") + "-" +
                                                                        xmlFile.GetAttribute("TELEFON");
                                                }
                                                else
                                                {
                                                    recpassport.Phone = xmlFile.GetAttribute("TELEFON");
                                                }

                                                break;
                                        }
                                    }

                                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "UL_ADDRESS"))
                                    {
                                        break;
                                    }
                                }

                                recpassport.Adr = "{0} {1} {2} {3} {4}".FormatWith(region, raion, gorod, street, dom);
                                if (!string.IsNullOrEmpty(korp))
                                {
                                    recpassport.Adr += "," + korp;
                                }

                                if (!string.IsNullOrEmpty(kvart))
                                {
                                    recpassport.Adr += "," + kvart;
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "UL"))
                    {
                        // Организация может оказаться без значений ИНН/КПП (прецеденты были), такую записывать нельзя.
                        if (string.IsNullOrEmpty(rec.INN) || string.IsNullOrEmpty(rec.KPP))
                        {
                            // пропускаем организацию, так как у нее нет ИНН/КПП
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(rec.Name))
                            {
                                rec.Name = "Не указано наименование";
                            }

                            if (string.IsNullOrEmpty(rec.ShortName))
                            {
                                rec.ShortName = "Не указано сокращенное наименование";
                            }

                            /*int uchrID = FindOrg(rec.INN, rec.KPP);*/
                            if (/*uchrID == -1*/ !newRestService.GetItems<D_Org_Structure>().Any(x => x.INN.Equals(rec.INN) && x.KPP.Equals(rec.KPP)))
                            {
                                AddOrg(ref rec);
                            }
                            else
                            {
                                rec.ID = /*uchrID*/ newRestService.GetItems<D_Org_Structure>().First(x => x.INN.Equals(rec.INN) && x.KPP.Equals(rec.KPP)).ID;
                                var org = /*ogsRepository.FindOne(uchrID)*/ newRestService.GetItem<D_Org_Structure>(rec.ID);
                                rec.If(structure => structure.Name != org.Name)
                                    .Do(structure => AddDifference(difference, org.Name, structure.Name, "Name"))
                                    .Do(structure => org.Name = structure.Name);
                                rec.If(structure => structure.ShortName != org.ShortName)
                                    .Do(structure => AddDifference(difference, org.ShortName, structure.ShortName, "ShortName"))
                                    .Do(structure => org.ShortName = structure.ShortName);
                            }

                            D_Org_Structure rec1 = rec;
                            var data = from p in /*passportRepository.FindAll()*/ newRestService.GetItems<F_Org_Passport>()
                                       where p.RefParametr.RefUchr.ID == rec1.ID &&
                                             p.RefParametr.RefPartDoc.ID == FX_FX_PartDoc.PassportDocTypeID &&
                                             (!p.RefParametr.CloseDate.HasValue)
                                       orderby p.ID descending
                                       select new
                                           {
                                               p.ID, 
                                               Sost = p.RefParametr.RefSost.ID, 
                                               p.RefParametr
                                           };

                            if (string.IsNullOrEmpty(recpassport.OGRN))
                            {
                                recpassport.OGRN = "Не указан";
                            }

                            if (string.IsNullOrEmpty(recpassport.Ordinary))
                            {
                                recpassport.Ordinary = "Не указан";
                            }

                            if (string.IsNullOrEmpty(recpassport.Fam))
                            {
                                recpassport.Fam = "Не указан";
                            }

                            if (string.IsNullOrEmpty(recpassport.NameRuc))
                            {
                                recpassport.NameRuc = "Не указан";
                            }

                            if (string.IsNullOrEmpty(recpassport.Otch))
                            {
                                recpassport.Otch = "Не указан";
                            }

                            if (string.IsNullOrEmpty(recpassport.Indeks))
                            {
                                recpassport.Indeks = "Нет";
                            }

                            if (string.IsNullOrEmpty(recpassport.Phone))
                            {
                                recpassport.Phone = "Не указан";
                            }

                            if (data.Any())
                            {
                                // если паспорт есть
                                if ((data.First().Sost == 7) || (data.First().Sost == 8))
                                {
                                    // если паспорт экспортирован или завершен, то создаем новый паспорт
                                    F_F_ParameterDoc newParameterDoc;
                                    F_Org_Passport newPassport;
                                    List<F_F_Founder> newFounders;
                                    List<F_F_OKVEDY> newActivities;
                                    List<F_F_Filial> newFilials;
                                    CopyPassportContent(
                                        data.First().RefParametr, 
                                        out newParameterDoc, 
                                        out newPassport, 
                                        out newFounders, 
                                        out newActivities, 
                                        out newFilials);

                                    // recpassport.ID = data.First().ID;
                                    var passport = passportRepository.FindOne(data.First().ID);

                                    recpassport.RefOKATO.If(okato => passport.RefOKATO != okato)
                                        .Do(okato => newPassport.RefOKATO = okato);
                                    recpassport.RefOKOPF.If(okopf => passport.RefOKOPF != okopf)
                                        .Do(
                                            okopf =>
                                            AddDifference(difference, passport.RefOKOPF.Return(okopfOkopf => okopfOkopf.Code.ToString(), "Не задано"), okopf.Code.ToString(), "RefOKOPF.Code"))
                                        .Do(okopf => newPassport.RefOKOPF = okopf);
                                    recpassport.Fam.If(s => passport.Fam != s)
                                        .Do(s => AddDifference(difference, passport.Fam, s, "Fam"))
                                        .Do(s => newPassport.Fam = s);
                                    recpassport.NameRuc.If(s => passport.NameRuc != s)
                                        .Do(s => AddDifference(difference, passport.NameRuc, s, "NameRuc"))
                                        .Do(s => newPassport.NameRuc = s);
                                    recpassport.Otch.If(s => passport.Otch != s)
                                        .Do(s => AddDifference(difference, passport.Otch, s, "Otch"))
                                        .Do(s => newPassport.Otch = s);
                                    recpassport.Ordinary.If(s => passport.Ordinary != s)
                                        .Do(s => AddDifference(difference, passport.Ordinary, s, "Ordinary"))
                                        .Do(s => newPassport.Ordinary = s);
                                    recpassport.OGRN.If(s => passport.OGRN != s)
                                        .Do(s => newPassport.OGRN = s);
                                    recpassport.Indeks.If(s => passport.Indeks != s)
                                        .Do(s => newPassport.Indeks = s);
                                    recpassport.Adr.If(s => passport.Adr != s)
                                        .Do(s => newPassport.Adr = s);

                                    CollectActivityDifference(passport, okvedy, difference);
                                    newPassport.Activity.Clear();

                                    EgrulUpdateFounder(passport, newFounders, founders, difference);

                                    if (difference.Length != 0)
                                    {
                                        parameterDocRepository.Save(newParameterDoc);

                                        newPassport.RefParametr = newParameterDoc;
                                        foreach (var founder in newFounders)
                                        {
                                            founder.RefPassport = newPassport;
                                            founderepository.Save(founder);
                                        }

                                        foreach (var filial in newFilials)
                                        {
                                            filial.RefPassport = newPassport;
                                            branchesRepository.Save(filial);
                                        }

                                        passportRepository.Save(newPassport);

                                        recpassport.ID = newPassport.ID;

                                        newPassport.RefParametr
                                            .If(doc => difference.Length > 0)
                                            .Do(
                                                doc =>
                                                {
                                                    doc.RefSost = sostRepository.FindOne(5); // На доработке
                                                    doc.Note = "Данные из ЕГРЮЛ";
                                                    Trace.TraceWarning(
                                                        "Изменения на основании файла ЕГРЮЛ - 'IDFILE' <{0}>\n{1}\n{2}",
                                                        idFile,
                                                        string.Format("{0}(ИНН={1};КПП={2})", doc.RefUchr.Name, doc.RefUchr.INN, doc.RefUchr.KPP),
                                                        difference.ToString());
                                                    difference.Length = 0;
                                                })
                                            .Do(parameterDocRepository.Save);
                                    }
                                    else
                                    {
                                        recpassport.ID = data.First().ID;
                                        passportRepository.FindOne(recpassport.ID).Activity.Clear();
                                        passportRepository.Delete(newPassport);
                                    }

                                    // passport.RefParametr
                                    // .If(doc => doc.RefSost.ID == 8)
                                    // .If(doc => difference.Length > 0)
                                    // .Do(
                                    // doc =>
                                    // {
                                    // doc.Note =
                                    // string.Format(
                                    // "Требуется повторный экспорт. Изменения на основании файла ЕГРЮЛ - 'IDFILE' {0}\n{1}",
                                    // idFile,
                                    // difference.ToString()).With(s => s.Length > 1000 ? s.Substring(0, 1000) : s);
                                    // doc.RefSost = sostRepository.FindOne(7); //Завершено
                                    // Trace.TraceWarning(
                                    // "Изменения на основании файла ЕГРЮЛ - 'IDFILE' <{0}>\n{1}\n{2}",
                                    // idFile,
                                    // string.Format(
                                    // "{0}(ИНН={1};КПП={2})",
                                    // doc.RefUchr.Name,
                                    // doc.RefUchr.INN,
                                    // doc.RefUchr.KPP),
                                    // difference.ToString());
                                    // difference.Length = 0;
                                    // })
                                    // .Do(parameterDocRepository.Save);
                                }
                                else
                                {
                                    var passport = passportRepository.FindOne(data.First().ID);

                                    recpassport.RefOKATO.If(okato => passport.RefOKATO != okato)
                                        .Do(okato => passport.RefOKATO = okato);
                                    recpassport.RefOKOPF.If(okopf => passport.RefOKOPF != okopf)
                                        .Do(
                                            okopf => AddDifference(
                                                difference, passport.RefOKOPF.Return(okopfOkopf => okopfOkopf.Code.ToString(), "Не задано"), okopf.Code.ToString(), "RefOKOPF.Code"))
                                        .Do(okopf => passport.RefOKOPF = okopf);
                                    recpassport.Fam.If(s => passport.Fam != s)
                                        .Do(s => AddDifference(difference, passport.Fam, s, "Fam"))
                                        .Do(s => passport.Fam = s);
                                    recpassport.NameRuc.If(s => passport.NameRuc != s)
                                        .Do(s => AddDifference(difference, passport.NameRuc, s, "NameRuc"))
                                        .Do(s => passport.NameRuc = s);
                                    recpassport.Otch.If(s => passport.Otch != s)
                                        .Do(s => AddDifference(difference, passport.Otch, s, "Otch"))
                                        .Do(s => passport.Otch = s);
                                    recpassport.Ordinary.If(s => passport.Ordinary != s)
                                        .Do(s => AddDifference(difference, passport.Ordinary, s, "Ordinary"))
                                        .Do(s => passport.Ordinary = s);
                                    recpassport.OGRN.If(s => passport.OGRN != s)
                                        .Do(s => passport.OGRN = s);
                                    recpassport.Indeks.If(s => passport.Indeks != s)
                                        .Do(s => passport.Indeks = s);
                                    recpassport.Adr.If(s => passport.Adr != s)
                                        .Do(s => passport.Adr = s);

                                    CollectActivityDifference(passport, okvedy, difference);
                                    passport.Activity.Clear();

                                    var newFounders = new List<F_F_Founder>();
                                    EgrulUpdateFounder(passport, newFounders, founders, difference);

                                    foreach (var founder in newFounders)
                                    {
                                        founder.RefPassport = passport;
                                        founderepository.Save(founder);
                                    }

                                    passport.RefParametr
                                        .If(doc => difference.Length > 0)
                                        .Do(
                                            doc =>
                                            {
                                                doc.Note =
                                                    string.Format(
                                                        "Изменения на основании файла ЕГРЮЛ - 'IDFILE' {0}\n{1}", 
                                                        idFile, 
                                                        difference.ToString()).With(s => s.Length > 1000 ? s.Substring(0, 1000) : s);
                                                doc.RefSost = sostRepository.FindOne(5); // На доработке
                                                Trace.TraceWarning(
                                                    "Изменения на основании файла ЕГРЮЛ - 'IDFILE' <{0}>\n{1}\n{2}",
                                                    idFile,
                                                    string.Format("{0}(ИНН={1};КПП={2})", doc.RefUchr.Name, doc.RefUchr.INN, doc.RefUchr.KPP),
                                                    difference.ToString());
                                                difference.Length = 0;
                                            })
                                        .Do(parameterDocRepository.Save);

                                    recpassport.ID = passport.ID;
                                }
                            }
                            else
                            {
                                var doc = new F_F_ParameterDoc
                                    {
                                        ID = 0, 
                                        Note = "Данные из ЕГРЮЛ", 
                                        PlanThreeYear = false, 
                                        RefUchr = ogsRepository.FindOne(rec.ID), 
                                        RefPartDoc = partDocRepository.FindOne(1), 
                                        RefSost = sostRepository.FindOne(5), 
                                        RefYearForm = yearFormRepository.FindOne(DateTime.Now.Year), 
                                        OpeningDate = DateTime.Now
                                    };

                                parameterDocRepository.Save(doc);

                                recpassport.ID = 0;
                                recpassport.TaskID = 0;
                                recpassport.SourceID = 0;
                                recpassport.RefParametr = parameterDocRepository.FindOne(doc.ID);

                                founders.Each(
                                    ychr => recpassport.Founders.Add(
                                        new F_F_Founder
                                            {
                                                RefPassport = recpassport, 
                                                RefYchred = ychr
                                            }));

                                passportRepository.Save(recpassport);
                            }

                            // если паспорт не экспортирован или создан новый паспорт то обновляем
                            {
                                // if ((data.Count() == 0) || (data.First().Sost != 8))
                                foreach (Okved item in okvedy)
                                {
                                    var recokved = new F_F_OKVEDY();
                                    recokved.ID = 0;
                                    recokved.SourceID = 0;
                                    recokved.TaskID = 0;
                                    recokved.RefPassport = passportRepository.FindOne(recpassport.ID);
                                    recokved.Name = item.Name;
                                    recokved.RefOKVED = okvedRepository.FindOne(FindOkved(item.KodOkved));
                                    if (item.Main == "1")
                                    {
                                        recokved.RefPrOkved = priznakOKVEDRepository.FindOne(FindPrOkved(1));
                                    }
                                    else
                                    {
                                        recokved.RefPrOkved = priznakOKVEDRepository.FindOne(FindPrOkved(2));
                                    }

                                    activityRepository.Save(recokved);
                                }
                            }

                            okvedy.Clear();
                            founders.Clear();
                            recpassport = null;

                            // doc = null;
                            rec = new D_Org_Structure();
                            recpassport = new F_Org_Passport();
                            difference.Length = 0;
                        }
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Закачка ППО
        /// </summary>
        public string ImportFilePPO(XDocument xmlFile)
        {
            var clientCode = ConfigurationManager.AppSettings["ClientLocationOKATOCode"];
            var dns = xmlFile.Root.GetDefaultNamespace();
            var ns = xmlFile.Root.GetNamespaceOfPrefix("ns2");
            var root = xmlFile.Root.Element(ns + "body");

            if (root != null)
            {
                foreach (XElement element in root.Elements(ns + "position"))
                {
                    var code = element.Element(dns + "code").Value;
                    var okato = element.Element(dns + "okato");

                    D_Org_PPO rec;

                    // todo: костыль для чечни исправить срочно
                    if (!okato.Element(dns + "code").Value
                             .StartsWith(clientCode))
                    {
                        continue;
                    }

                    rec = FindPpo(code) == -1 ? new D_Org_PPO { ID = 0 } : ppoRepository.FindOne(FindPpo(code));

                    rec.Code = code;
                    rec.Name = element.Element(dns + "name").Value;

                    if (okato != null)
                    {
                        int okatoid = FindOkato(okato.Element(dns + "code").Value);
                        if (okatoid == -1)
                        {
                            var recOkato = new D_OKATO_OKATO
                                {
                                    ID = 0, 
                                    Code = okato.Element(dns + "code").Value, 
                                    Name = okato.Element(dns + "name").Value
                                };

                            okatoRepository.Save(recOkato);
                            okatoid = recOkato.ID;
                        }

                        rec.RefOKATO = okatoRepository.FindOne(okatoid);
                    }

                    if (FindPpo(code) == -1)
                    {
                        ppoRepository.Save(rec);
                    }

                    //// PPORepository.DbContext.CommitChanges();
                }

                return "Файл успешно импортирован.";
            }

            return "Неверный формат файла.";
        }

        /// <summary>
        ///   Закачка учреждений
        /// </summary>
        public void ImportFileOGSNew(XmlTextReader xmlFile)
        {
            try
            {
                string clientCode = ConfigurationManager.AppSettings["ClientLocationOKATOCode"];
                string state = "start";
                var rec = new D_Org_Structure();
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                string okatoCode = string.Empty;
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "fullName":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.Name = xmlFile.Value;
                                }

                                break;
                            case "shortName":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.ShortName = xmlFile.Value;
                                }

                                break;
                            case "inn":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.INN = xmlFile.Value;
                                }

                                break;
                            case "kpp":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.KPP = xmlFile.Value;
                                    state = "endstart";
                                }

                                break;
                            case "orgType":
                                if (state == "endstart")
                                {
                                    xmlFile.Read();
                                    rec.RefTipYc = tipYchRepository.FindOne(Convert.ToInt32(xmlFile.Value));
                                    state = "end";
                                }

                                break;
                            case "okato":
                                state = "okato";
                                break;
                            case "ppo":
                                state = "ppo";
                                break;
                            case "code":
                                switch (state)
                                {
                                    case "okato":
                                        xmlFile.Read();
                                        okatoCode = xmlFile.Value;
                                        state = "end";
                                        break;
                                    case "ppo":
                                        xmlFile.Read();
                                        int ppoid = FindPpo(xmlFile.Value);
                                        if (ppoid != -1)
                                        {
                                            rec.RefOrgPPO = ppoRepository.FindOne(ppoid);
                                        }

                                        state = "end";
                                        break;
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "ns2:position"))
                    {
                        // Организация может оказаться без значений ИНН/КПП (прецеденты были), такую записывать нельзя.
                        if (!string.IsNullOrEmpty(rec.INN)
                            && !string.IsNullOrEmpty(rec.KPP)
                            && okatoCode.StartsWith(clientCode))
                        {
                            if (string.IsNullOrEmpty(rec.Name))
                            {
                                rec.Name = "Не указано наименование";
                            }

                            if (string.IsNullOrEmpty(rec.ShortName))
                            {
                                rec.ShortName = "Не указано сокращенное наименование";
                            }

                            /*int orgID = FindOrg(rec.INN, rec.KPP);*/
                            if (/*orgID == -1*/ !newRestService.GetItems<D_Org_Structure>().Any(x => x.INN.Equals(rec.INN) && x.KPP.Equals(rec.KPP)))
                            {
                                AddOrg(ref rec);
                            }
                            else
                            {
                                D_Org_Structure recorg = /*ogsRepository.FindOne(orgID)*/
                                    newRestService.GetItems<D_Org_Structure>().First(x => x.INN.Equals(rec.INN) && x.KPP.Equals(rec.KPP));
                                recorg.ShortName = rec.ShortName;
                                recorg.Name = rec.Name;
                                recorg.INN = rec.INN;
                                recorg.KPP = rec.KPP;
                                recorg.RefTipYc = rec.RefTipYc;
                                recorg.RefOrgPPO = rec.RefOrgPPO;
                            }
                        }

                        state = "start";
                        rec = new D_Org_Structure();
                        okatoCode = string.Empty;
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Закачка Вида учреждения
        /// </summary>
        public void ImportInstitutionType(XmlTextReader xmlFile)
        {
            try
            {
                var codesList = new Dictionary<string, string>();
                var rec = new D_Org_VidOrg();
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "code":
                                xmlFile.Read();
                                rec.Code = xmlFile.Value;
                                break;
                            case "businessStatus":
                                xmlFile.Read();
                                rec.BusinessStatus = xmlFile.Value;
                                break;
                            case "name":
                                xmlFile.Read();
                                rec.Name = xmlFile.Value;
                                break;
                            case "parentCode":
                                xmlFile.Read();
                                int parentID = FindInstitutionType(xmlFile.Value);
                                if (parentID != -1)
                                {
                                    rec.ParentID = parentID;
                                }
                                else
                                {
                                    codesList.Add(rec.Code, xmlFile.Value);
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "ns2:position"))
                    {
                        if (string.IsNullOrEmpty(rec.Name))
                        {
                            rec.Name = "Не указано наименование";
                        }

                        D_Org_VidOrg vidOrg = vidOrgRepository.FindAll()
                            .SingleOrDefault(p => p.Code == rec.Code);
                        if (vidOrg == null)
                        {
                            rec.ID = 0;
                            vidOrgRepository.Save(rec);
                        }
                        else
                        {
                            vidOrg.Name = rec.Name;
                            vidOrg.BusinessStatus = rec.BusinessStatus;
                            vidOrgRepository.DbContext.CommitChanges();
                        }

                        rec = new D_Org_VidOrg();
                    }
                }

                foreach (var item in codesList)
                {
                    int parentID = FindInstitutionType(item.Value);
                    if (parentID != -1)
                    {
                        D_Org_VidOrg record = vidOrgRepository.FindOne(FindInstitutionType(item.Key));
                        if (record != null)
                        {
                            record.ParentID = parentID;
                        }
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Закачка ОКВЭД
        /// </summary>
        public void ImportOkved(XmlTextReader xmlFile)
        {
            try
            {
                var codesList = new Dictionary<string, string>();
                var rec = new D_OKVED_OKVED();
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "code":
                                xmlFile.Read();
                                rec.Code = xmlFile.Value;
                                break;
                            case "name":
                                xmlFile.Read();
                                rec.Name = xmlFile.Value;
                                break;
                            case "section":
                                xmlFile.Read();
                                rec.Section = xmlFile.Value;
                                break;
                            case "subsection":
                                xmlFile.Read();
                                rec.SubSection = xmlFile.Value;
                                break;
                            case "parentCode":
                                xmlFile.Read();
                                int parentID = FindOkved(xmlFile.Value);
                                if (parentID != -1)
                                {
                                    rec.ParentID = parentID;
                                }
                                else
                                {
                                    codesList.Add(rec.Code, xmlFile.Value);
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "ns2:position"))
                    {
                        rec.ID = 0;
                        if (rec.Name == null)
                        {
                            rec.Name = "Не указано наименование";
                        }

                        okvedRepository.Save(rec);
                        rec = new D_OKVED_OKVED();
                    }
                }

                foreach (var item in codesList)
                {
                    int parentID = FindOkved(item.Value);
                    if (parentID != -1)
                    {
                        D_OKVED_OKVED record = okvedRepository.FindOne(FindOkved(item.Key));
                        if (record != null)
                        {
                            record.ParentID = parentID;
                        }
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Закачка OKTMO
        /// </summary>
        public void ImportOKTMO(XmlTextReader xmlFile)
        {
            try
            {
                string clientCode = ConfigurationManager.AppSettings["ClientLocationOKATOCode"];
                var codesList = new Dictionary<string, string>();
                var rec = new D_OKTMO_OKTMO();
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "code":
                                xmlFile.Read();
                                rec.Code = xmlFile.Value;
                                break;
                            case "name":
                                xmlFile.Read();
                                rec.Name = xmlFile.Value;
                                break;
                            case "okato":
                                xmlFile.Read();
                                rec.Okato = xmlFile.Value;
                                break;
                            case "parentCode":
                                xmlFile.Read();
                                int parentID = FindOktmo(xmlFile.Value);
                                if (parentID != -1)
                                {
                                    rec.ParentID = parentID;
                                }
                                else
                                {
                                    codesList.Add(rec.Code, xmlFile.Value);
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "ns2:position"))
                    {
                        rec.ID = 0;
                        if (rec.Name == null)
                        {
                            rec.Name = "Не указано наименование";
                        }

                        if (rec.Code.StartsWith(clientCode))
                        {
                            oktmoRepository.Save(rec);
                        }

                        rec = new D_OKTMO_OKTMO();
                    }
                }

                foreach (var item in codesList)
                {
                    int parentID = FindOktmo(item.Value);
                    if (parentID != -1)
                    {
                        D_OKTMO_OKTMO record = oktmoRepository.FindOne(FindOktmo(item.Key));
                        if (record != null)
                        {
                            record.ParentID = parentID;
                        }
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Закачка OKATO
        /// </summary>
        public void ImportOKATO(XmlTextReader xmlFile)
        {
            try
            {
                string clientCode = ConfigurationManager.AppSettings["ClientLocationOKATOCode"];
                var codesList = new Dictionary<string, string>();
                var rec = new D_OKATO_OKATO();
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "code":
                                xmlFile.Read();
                                rec.Code = xmlFile.Value;
                                break;
                            case "name":
                                xmlFile.Read();
                                rec.Name = xmlFile.Value;
                                break;
                            case "parentCode":
                                xmlFile.Read();
                                int parentID = FindOkato(xmlFile.Value);
                                if (parentID != -1)
                                {
                                    rec.ParentID = parentID;
                                }
                                else
                                {
                                    codesList.Add(rec.Code, xmlFile.Value);
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "ns2:position"))
                    {
                        rec.ID = 0;
                        if (rec.Name == null)
                        {
                            rec.Name = "Не указано наименование";
                        }

                        if (rec.Code.Equals("00000000000")
                            || rec.Code.StartsWith(clientCode))
                        {
                            D_OKATO_OKATO item = okatoRepository.FindAll()
                                .SingleOrDefault(p => p.Code.Equals(rec.Code));
                            if (item == null)
                            {
                                okatoRepository.Save(rec);
                            }
                        }

                        rec = new D_OKATO_OKATO();
                    }
                }

                foreach (var item in codesList)
                {
                    int parentID = FindOkato(item.Value);
                    if (parentID != -1)
                    {
                        D_OKATO_OKATO record = okatoRepository.FindOne(FindOkato(item.Key));
                        if (record != null)
                        {
                            record.ParentID = parentID;
                        }
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Закачка NsiOGS
        /// </summary>
        public void ImportNsiOGS(XmlTextReader xmlFile)
        {
            try
            {
                string clientCode = ConfigurationManager.AppSettings["ClientLocationOKATOCode"];
                string state = "start";
                var rec = new D_Org_NsiOGS();
                var orgYchr = new D_Org_OrgYchr();
                string okatoCode = string.Empty;
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "regNum":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.regNum = xmlFile.Value;
                                }

                                if (state == "founder")
                                {
                                    xmlFile.Read();
                                    orgYchr.Code = xmlFile.Value;
                                }

                                break;
                            case "fullName":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.FullName = xmlFile.Value;
                                }

                                if (state == "founder")
                                {
                                    xmlFile.Read();
                                    orgYchr.Name = xmlFile.Value;
                                    state = "end";
                                }

                                break;
                            case "shortName":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.ShortName = xmlFile.Value;
                                }

                                break;
                            case "inn":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.inn = xmlFile.Value;
                                }

                                break;
                            case "kpp":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.kpp = xmlFile.Value;
                                    state = "endstart";
                                }

                                break;
                            case "okato":
                                state = "okato";
                                break;
                            case "founder":
                                state = "founder";
                                break;
                            case "code":
                                if (state == "okato")
                                {
                                    xmlFile.Read();
                                    okatoCode = xmlFile.Value;
                                    state = "end";
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "ns2:position"))
                    {
                        // Организация может оказаться без значений ИНН/КПП (прецеденты были), такую записывать нельзя.
                        if (!string.IsNullOrEmpty(rec.inn)
                            && !string.IsNullOrEmpty(rec.kpp)
                            && okatoCode.StartsWith(clientCode))
                        {
                            if (string.IsNullOrEmpty(rec.FullName))
                            {
                                rec.FullName = "Не указано наименование";
                            }

                            if (string.IsNullOrEmpty(rec.ShortName))
                            {
                                rec.ShortName = "Не указано сокращенное наименование";
                            }

                            int ogsid = FindOgs(rec.inn, rec.kpp);
                            if (ogsid == -1)
                            {
                                rec.ID = 0;
                                nsiOgsRepository.Save(rec);
                            }
                            else
                            {
                                D_Org_NsiOGS recogs = nsiOgsRepository.FindOne(ogsid);
                                recogs.regNum = rec.regNum;
                                recogs.ShortName = rec.ShortName;
                                recogs.FullName = rec.FullName;
                                recogs.inn = rec.inn;
                                recogs.kpp = rec.kpp;
                            }

                            if (!string.IsNullOrEmpty(orgYchr.Code))
                            {
                                int orgYchrID = FindOrgYchr(orgYchr.Code);
                                if (orgYchrID == -1)
                                {
                                    if (string.IsNullOrEmpty(orgYchr.Name))
                                    {
                                        orgYchr.Name =
                                            "Не указано наименование";
                                    }

                                    orgYchr.ID = 0;
                                    orgYchrRepository.Save(orgYchr);
                                }
                            }
                        }

                        state = "start";
                        orgYchr = new D_Org_OrgYchr();
                        rec = new D_Org_NsiOGS();
                        okatoCode = string.Empty;
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Закачка NsiBudget
        /// </summary>
        public void ImportNsiBudget(XmlTextReader xmlFile)
        {
            try
            {
                string clientCode = ConfigurationManager.AppSettings["ClientLocationOKATOCode"];
                string state = "start";
                var rec = new D_Fin_nsiBudget();
                string okatoCode = string.Empty;
                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "name":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.Name = xmlFile.Value;
                                    state = "end";
                                }

                                break;
                            case "okato":
                                state = "okato";
                                break;
                            case "code":
                                if (state == "start")
                                {
                                    xmlFile.Read();
                                    rec.Code = xmlFile.Value;
                                }

                                if (state == "okato")
                                {
                                    xmlFile.Read();
                                    okatoCode = xmlFile.Value;
                                    state = "end";
                                }

                                break;
                        }
                    }

                    if ((xmlFile.NodeType == XmlNodeType.EndElement) && (xmlFile.Name == "ns2:position"))
                    {
                        if (okatoCode.StartsWith(clientCode))
                        {
                            if (string.IsNullOrEmpty(rec.Name))
                            {
                                rec.Name = "Не указано наименование";
                            }

                            if (string.IsNullOrEmpty(rec.Code))
                            {
                                rec.Code = "Не указан код";
                            }

                            int id = FindNsiBudget(rec.Code);
                            if (id == -1)
                            {
                                rec.ID = 0;
                                nsiBudgetRepository.Save(rec);
                            }
                            else
                            {
                                D_Fin_nsiBudget recogs = nsiBudgetRepository.FindOne(id);
                                recogs.Name = rec.Name;
                                recogs.Code = rec.Code;
                            }
                        }

                        state = "start";
                        rec = new D_Fin_nsiBudget();
                        okatoCode = string.Empty;
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        public void CopyPassportContent(
            F_F_ParameterDoc parameterDoc, 
            out F_F_ParameterDoc oldParameterDoc, 
            out F_Org_Passport oldPassport, 
            out List<F_F_Founder> oldFounders, 
            out List<F_F_OKVEDY> oldActivities, 
            out List<F_F_Filial> oldFilials)
        {
            F_F_ParameterDoc oldFormData = parameterDocRepository.FindAll().Where(x => x.ID == parameterDoc.ID).First();

            oldParameterDoc = new F_F_ParameterDoc();
            oldActivities = new List<F_F_OKVEDY>();
            oldFilials = new List<F_F_Filial>();
            oldFounders = new List<F_F_Founder>();

            oldParameterDoc.ID = 0;
            oldParameterDoc.PlanThreeYear = oldFormData.PlanThreeYear;
            oldParameterDoc.RefUchr = oldFormData.RefUchr;
            oldParameterDoc.RefPartDoc = partDocRepository.FindOne(1); // паспорт учреждения
            oldParameterDoc.RefSost = oldFormData.RefSost;
            oldParameterDoc.RefYearForm = yearFormRepository.FindOne(DateTime.Now.Year);

            // oldParameterDoc.RefYearForm = oldFormData.RefYearForm;
            oldParameterDoc.OpeningDate = oldFormData.OpeningDate;

            oldPassport = new F_Org_Passport();

            // var dateOfLastDoc = parameterDocRepository.FindAll()
            // .Where(
            // x =>
            // x.RefUchr.ID == oldFormData.RefUchr.ID && x.RefPartDoc.ID == oldFormData.RefPartDoc.ID &&
            // x.CloseDate.HasValue).Select(x => x.CloseDate).Max();

            // var idOfLastDoc =
            // parameterDocRepository.FindAll().Where(x => x.CloseDate == dateOfLastDoc).Select(x => x.ID).First();
            var passports = passportRepository.FindAll()
                .Where(
                    x =>
                    x.RefParametr.ID == parameterDoc.ID).ToList();
            if (passports.Count > 0)
            {
                oldPassport.SourceID = passports.First().SourceID;
                oldPassport.TaskID = passports.First().TaskID;
                oldPassport.OGRN = passports.First().OGRN;
                oldPassport.Fam = passports.First().Fam;
                oldPassport.NameRuc = passports.First().NameRuc;
                oldPassport.Otch = passports.First().Otch;
                oldPassport.Ordinary = passports.First().Ordinary;
                oldPassport.Adr = passports.First().Adr;
                oldPassport.Indeks = passports.First().Indeks;
                oldPassport.Website = passports.First().Website;
                oldPassport.OKPO = passports.First().OKPO;
                oldPassport.Phone = passports.First().Phone;
                oldPassport.Mail = passports.First().Mail;
                oldPassport.RefOKOPF = passports.First().RefOKOPF;
                oldPassport.RefOKATO = passports.First().RefOKATO;
                oldPassport.RefCateg = passports.First().RefCateg;
                oldPassport.RefOKTMO = passports.First().RefOKTMO;
                oldPassport.RefVid = passports.First().RefVid;
                oldPassport.RefOKFS = passports.First().RefOKFS;
                oldPassport.RefRaspor = passports.First().RefRaspor;

                // oldPassport.RefParametr = formData;

                // var founders =
                // founderepository.FindAll().Where(x => x.RefPassport.RefParametr.ID == parameterDoc.ID).ToList();
                oldFounders = parameterDoc.Passports.First().Founders.Select(
                    fFFounder => new F_F_Founder
                        {
                            SourceID = fFFounder.SourceID, 
                            TaskID = fFFounder.TaskID, 
                            formative = fFFounder.formative, 
                            stateTask = fFFounder.stateTask, 
                            supervisoryBoard = fFFounder.supervisoryBoard,
                            financeSupply = fFFounder.financeSupply,
                            manageProperty = fFFounder.manageProperty,
                            
                            // RefPassport = orgPassport,
                            RefYchred = fFFounder.RefYchred
                        }).ToList();

                // var okvedys =
                // activityRepository.FindAll().Where(x => x.RefPassport.RefParametr.ID == parameterDoc.ID).ToList();
                oldActivities = parameterDoc.Passports.First().Activity.Select(
                    fFOkvedy => new F_F_OKVEDY
                        {
                            SourceID = fFOkvedy.SourceID, 
                            TaskID = fFOkvedy.TaskID, 
                            Name = fFOkvedy.Name, 
                            RefPrOkved = fFOkvedy.RefPrOkved, 
                            RefOKVED = fFOkvedy.RefOKVED, 
                            
                            // RefPassport = orgPassport
                        }).ToList();

                oldFilials = new List<F_F_Filial>();

                // var filials =
                // branchesRepository.FindAll().Where(x => x.RefPassport.RefParametr.ID == parameterDoc.ID).ToList();
                oldFilials = parameterDoc.Passports.First().Branches.Select(
                    fFFilial => new F_F_Filial
                        {
                            SourceID = fFFilial.SourceID, 
                            TaskID = fFFilial.TaskID, 
                            Code = fFFilial.Code, 
                            Name = fFFilial.Name, 
                            Nameshot = fFFilial.Nameshot, 
                            KPP = fFFilial.KPP, 
                            INN = fFFilial.INN, 
                            RefTipFi = fFFilial.RefTipFi, 
                        }).ToList();

                // foreach (var fFFilial in filials)
                // {
                // var filial = new F_F_Filial
                // {
                // SourceID = fFFilial.SourceID,
                // TaskID = fFFilial.TaskID,
                // Code = fFFilial.Code,
                // Name = fFFilial.Name,
                // Nameshot = fFFilial.Nameshot,
                // KPP = fFFilial.KPP,
                // INN = fFFilial.INN,
                // RefTipFi = fFFilial.RefTipFi,
                // //RefPassport = orgPassport
                // };
                // branchesRepository.Save(filial);
                // }

                // passportRepository.Save(orgPassport);
            }

            // Resolver.Get<IDocService>().CopyFiles(oldFormData.ID);

            // formData.CloseDate = DateTime.Now;
            // parameterDocRepository.Save(formData);
            // parameterDocRepository.DbContext.CommitChanges();

            // return orgPassport;
        }

        /// <summary>
        ///   Экспорт учреждения в XML.
        /// </summary>
        /// <param name="recId"> Id учреждения. </param>
        public Stream ExportForOgs(int recId)
        {
            var pass = new List<OgsViewModel>(
                from p in passportRepository.FindAll()
                where p.RefParametr.ID == recId
                select new OgsViewModel
                {
                    ID = p.ID,
                    RefOrgPpo = p.RefParametr.RefUchr.RefOrgPPO.ID,
                    RefOrgPpoCode = p.RefParametr.RefUchr.RefOrgPPO.Code,
                    RefOrgPpoName = p.RefParametr.RefUchr.RefOrgPPO.Name,
                    RefTipYc = p.RefParametr.RefUchr.RefTipYc.ID,
                    RefTipYcName = p.RefParametr.RefUchr.RefTipYc.Name,
                    RefOrgGrbs = p.RefParametr.RefUchr.RefOrgGRBS.ID,
                    RefOrgGrbsName = p.RefParametr.RefUchr.RefOrgGRBS.Name,
                    RefOrgGrbsCode = p.RefParametr.RefUchr.RefOrgGRBS.Code,
                    RefRaspor = p.RefRaspor.ID,
                    RefRasporName = p.RefRaspor.Name,
                    RefRasporCode = p.RefRaspor.Code,
                    Name = p.RefParametr.RefUchr.Name,
                    ShortName = p.RefParametr.RefUchr.ShortName,
                    Inn = p.RefParametr.RefUchr.INN,
                    Kpp = p.RefParametr.RefUchr.KPP,
                    Ogrn = p.OGRN,
                    RefCatYh = p.RefCateg.ID,
                    RefVid = p.RefVid.ID,
                    RefVidCode = p.RefVid.Code,
                    RefVidName = p.RefVid.Name,
                    RefOkato = p.RefOKATO.ID,
                    RefOkatoName = p.RefOKATO.Name,
                    RefOkatoCode = p.RefOKATO.Code,
                    RefOkfs = p.RefOKFS.ID,
                    RefOkfsName = p.RefOKFS.Name,
                    RefOkfsCode = p.RefOKFS.Code,
                    RefOktmo = p.RefOKTMO.ID,
                    RefOktmoCode = p.RefOKTMO.Code,
                    RefOktmoName = p.RefOKTMO.Name,
                    RefOkopf = p.RefOKOPF.ID,
                    RefOkopfName = p.RefOKOPF.Name,
                    RefOkopfCode = p.RefOKOPF.Code5Zn,
                    Okpo = p.OKPO,
                    Adr = p.Adr,
                    Website = p.Website,
                    Phone = p.Phone,
                    Mail = p.Mail,
                    Fam = p.Fam,
                    NameRuc = p.NameRuc,
                    Otch = p.Otch,
                    Ordinary = p.Ordinary
                });

            OgsViewModel ogs;

            if (pass.Count > 0)
            {
                ogs = pass.First();
            }
            else
            {
                throw new KeyNotFoundException("Учреждение не найдено.");
            }

            List<ActivityViewModel> activity = (from p in activityRepository.FindAll()
                                                where p.RefPassport.ID == ogs.ID
                                                select new ActivityViewModel
                                                {
                                                    name = p.Name,
                                                    OKVEDcode = p.RefOKVED.Code,
                                                    OKVEDname = p.RefOKVED.Name,
                                                    OKVEDtype = p.RefPrOkved.Code,
                                                }).ToList();

            List<BranchesViewModel> branches = (from p in branchesRepository.FindAll()
                                                where p.RefPassport.ID == ogs.ID
                                                select new BranchesViewModel
                                                {
                                                    fullName = p.Name,
                                                    shortName = p.Nameshot,
                                                    type = p.RefTipFi.Code.ToString("00"),
                                                }).ToList();
            ////EnactmentViewModel enactment = null;

            List<DocumentViewModel> documents = (from p in documentRepository.FindAll()
                                                 where p.RefParametr.ID == recId
                                                 select new DocumentViewModel
                                                 {
                                                     name = p.Name,
                                                     url = p.Url,
                                                     type = p.RefTypeDoc.Code,
                                                     date = p.DocDate
                                                 }).ToList();

            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlNode rootNode = doc.AppendChild(doc.CreateElement("institutionInfo"));

            XmlNode headerNode = rootNode.AppendChild(doc.CreateElement("header"));
            XmlAttributeCollection attrColl = headerNode.Attributes;

            XmlAttribute newAttr = doc.CreateAttribute("id");
            newAttr.Value = "Глобальный идентификатор информационного пакета";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("createDateTime");
            newAttr.Value = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            XmlNode rootBody = rootNode.AppendChild(doc.CreateElement("body"));

            XmlNode rootPosition = rootBody.AppendChild(doc.CreateElement("position"));
            attrColl = rootPosition.Attributes;

            newAttr = doc.CreateAttribute("positionId");
            newAttr.Value = "Идентификатор позиции в информационном пакете";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("changeDate");
            newAttr.Value = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("versionNumber");
            newAttr.Value = "1";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            XmlNode rootPlacer = rootPosition.AppendChild(doc.CreateElement("placer"));
            attrColl = rootPlacer.Attributes;

            newAttr = doc.CreateAttribute("regNum");
            newAttr.Value = "Реестровый номер организации в ОГС";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("fullName");
            newAttr.Value = "Полное наименование организации";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("inn");
            newAttr.Value = "ИНН";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("kpp");
            newAttr.Value = "КПП";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            /*
            XmlNode rootInitiator = rootPosition.AppendChild(doc.CreateElement("initiator"));
            attrColl = rootInitiator.Attributes;

            newAttr = doc.CreateAttribute("regNum");
            newAttr.Value = "Реестровый номер организации в ОГС";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("fullName");
            newAttr.Value = "Полное наименование организации";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("inn");
            newAttr.Value = "ИНН";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("kpp");
            newAttr.Value = "КПП";
            attrColl.Append(newAttr);
      */
            /*           XmlNode rootMain = rootPosition.AppendChild(doc.CreateElement("main"));
            attrColl = rootMain.Attributes;

            newAttr = doc.CreateAttribute("shortName");
            newAttr.Value = OGS.ShortName;
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("orgType");
            newAttr.Value = OGS.RefTip.ToString("00");
            attrColl.Append(newAttr);

            XmlNode rootRbs = rootMain.AppendChild(doc.CreateElement("rbs"));
            attrColl = rootRbs.Attributes;

            newAttr = doc.CreateAttribute("regNum");
            newAttr.Value = OGS.RefRasporCode;
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("fullName");
            newAttr.Value = OGS.RefRasporName;
            attrColl.Append(newAttr);

            XmlNode rootGrbs = rootMain.AppendChild(doc.CreateElement("grbs"));
            attrColl = rootGrbs.Attributes;

            newAttr = doc.CreateAttribute("regNum");
            newAttr.Value = OGS.RefOrgGRBSCode;
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("fullName");
            newAttr.Value = OGS.RefOrgGRBSName;
            attrColl.Append(newAttr);

            XmlNode rootClassifier = rootMain.AppendChild(doc.CreateElement("classifier"));
            attrColl = rootClassifier.Attributes;

            newAttr = doc.CreateAttribute("ogrn");
            newAttr.Value = OGS.OGRN;
            attrColl.Append(newAttr);

            XmlNode rootOkfs = rootClassifier.AppendChild(doc.CreateElement("okfs"));
            attrColl = rootOkfs.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = OGS.RefOKFSCode;
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = OGS.RefOKFSName;
            attrColl.Append(newAttr);

            XmlNode rootOkopf = rootClassifier.AppendChild(doc.CreateElement("okopf"));
            attrColl = rootOkopf.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = OGS.RefOKOPFCode;
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = OGS.RefOKOPFName;
            attrColl.Append(newAttr);

//            XmlNode rootOkogu = rootClassifier.AppendChild(doc.CreateElement("okogu"));
//            attrColl = rootOkogu.Attributes;

//            newAttr = doc.CreateAttribute("code");
//            newAttr.Value = "Код ОКОГУ";
//            attrColl.Append(newAttr);

//            newAttr = doc.CreateAttribute("name");
//            newAttr.Value = "Наименование по ОКОГУ";
//            attrColl.Append(newAttr);

            XmlNode rootOkpo = rootClassifier.AppendChild(doc.CreateElement("okpo"));
            attrColl = rootOkpo.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = OGS.RefOKPOCode;
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = OGS.RefOKPOName;
            attrColl.Append(newAttr);

            XmlNode rootOkato = rootClassifier.AppendChild(doc.CreateElement("okato"));
            attrColl = rootOkato.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = OGS.RefOKATOCode;
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = OGS.RefOKATOName;
            attrColl.Append(newAttr);

            foreach (OKVEDYViewModel rec in OKVEDY)
            {
                XmlNode rootOkved = rootClassifier.AppendChild(doc.CreateElement("okved"));
                attrColl = rootOkved.Attributes;

                newAttr = doc.CreateAttribute("code");
                newAttr.Value = rec.OkvedCode;
                attrColl.Append(newAttr);

                newAttr = doc.CreateAttribute("name");
                newAttr.Value = rec.OkvedName;
                attrColl.Append(newAttr);

                newAttr = doc.CreateAttribute("type");
                if (rec.PrOkvedCode == "12") 
                  { newAttr.Value = "C"; }
                else
                  { newAttr.Value = "O"; }
                
                attrColl.Append(newAttr);
            }
            XmlNode rootAddress = rootMain.AppendChild(doc.CreateElement("address"));
            attrColl = rootAddress.Attributes;

            newAttr = doc.CreateAttribute("zip");
            newAttr.Value = "Почтовый индекс";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("building");
            newAttr.Value = "Номер дома";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("office");
            newAttr.Value = "Номер офиса (квартиры)";
            attrColl.Append(newAttr);

            XmlNode rootSubject = rootAddress.AppendChild(doc.CreateElement("subject"));
            attrColl = rootSubject.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = "Код КЛАДР";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = "Наименование по КЛАДР";
            attrColl.Append(newAttr);

            XmlNode rootRegion = rootAddress.AppendChild(doc.CreateElement("region"));
            attrColl = rootRegion.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = "Код КЛАДР";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = "Наименование по КЛАДР";
            attrColl.Append(newAttr);

            XmlNode rootCity = rootAddress.AppendChild(doc.CreateElement("city"));
            attrColl = rootCity.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = "Код КЛАДР";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = "Наименование по КЛАДР";
            attrColl.Append(newAttr);

            XmlNode rootLocality = rootAddress.AppendChild(doc.CreateElement("locality"));
            attrColl = rootLocality.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = "Код КЛАДР";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = "Наименование по КЛАДР";
            attrColl.Append(newAttr);

            XmlNode rootStreet = rootAddress.AppendChild(doc.CreateElement("street"));
            attrColl = rootStreet.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = "Код КЛАДР";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = "Наименование по КЛАДР";
            attrColl.Append(newAttr);
*/
            XmlNode rootAdditional = rootPosition.AppendChild(doc.CreateElement("additional"));
            attrColl = rootAdditional.Attributes;

            newAttr = doc.CreateAttribute("phone");
            newAttr.Value = ogs.Phone;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("www");
            newAttr.Value = ogs.Website;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            // newAttr = doc.CreateAttribute("section");
            // newAttr.Value = "Код главы ГРБС (Первые три символа КБК)";
            // attrColl.Append(newAttr);
            newAttr = doc.CreateAttribute("eMail");
            newAttr.Value = ogs.Mail;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            XmlNode rootInstitutionType = rootAdditional.AppendChild(doc.CreateElement("institutionType"));
            attrColl = rootInstitutionType.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = ogs.RefVidCode;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = ogs.RefVidName;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            XmlNode rootPpo = rootAdditional.AppendChild(doc.CreateElement("ppo"));
            attrColl = rootPpo.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = ogs.RefOrgPpoCode;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = ogs.RefOrgPpoName;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            /*         XmlNode rootOkato = rootPpo.AppendChild(doc.CreateElement("okato"));
            attrColl = rootOkato.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = "Код ОКАТО";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = "Наименование по ОКАТО";
            attrColl.Append(newAttr);
   */
            XmlNode rootOktmo = rootPpo.AppendChild(doc.CreateElement("oktmo"));
            attrColl = rootOktmo.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = string.Empty;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = string.Empty;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            XmlNode rootChief = rootAdditional.AppendChild(doc.CreateElement("chief"));
            attrColl = rootChief.Attributes;

            newAttr = doc.CreateAttribute("lastName");
            newAttr.Value = ogs.Fam;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("firstName");
            newAttr.Value = ogs.NameRuc;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("middleName");
            newAttr.Value = ogs.Otch;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("position");
            newAttr.Value = ogs.Ordinary;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);

                /*        XmlNode rootBranchInfo = rootAdditional.AppendChild(doc.CreateElement("branchInfo"));
            attrColl = rootBranchInfo.Attributes;

            newAttr = doc.CreateAttribute("type");
            newAttr.Value = "Тип филиала";
            attrColl.Append(newAttr);

            XmlNode rootHeadOffice = rootBranchInfo.AppendChild(doc.CreateElement("headOffice"));
            attrColl = rootHeadOffice.Attributes;

            newAttr = doc.CreateAttribute("regNum");
            newAttr.Value = "Реестровый номер организации в ОГС";
            attrColl.Append(newAttr);

            newAttr = doc.CreateAttribute("fullName");
            newAttr.Value = "Полное наименование организации";
            attrColl.Append(newAttr);
*/
                foreach (BranchesViewModel rec in branches)
                {
                    XmlNode rootBranch = rootAdditional.AppendChild(doc.CreateElement("branch"));
                    attrColl = rootBranch.Attributes;

                    newAttr = doc.CreateAttribute("regNum");
                    newAttr.Value = "Реестровый номер организации в ОГС";
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }

                    newAttr = doc.CreateAttribute("fullName");
                    newAttr.Value = rec.fullName;
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }

                    newAttr = doc.CreateAttribute("type");
                    newAttr.Value = rec.type;
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }

                    newAttr = doc.CreateAttribute("shortName");
                    newAttr.Value = rec.shortName;
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }
                }
            }

            var rootFounder = rootAdditional.AppendChild(doc.CreateElement("founder"));
            attrColl = rootFounder.Attributes;

            newAttr = doc.CreateAttribute("regNum");
            newAttr.Value = "Реестровый номер организации в ОГС";
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("fullName");
            newAttr.Value = ogs.RefOrgGrbsName;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);

/*
                if (enactment != null)
                {
                    XmlNode rootEnactment = rootAdditional.AppendChild(doc.CreateElement("enactment"));
                    attrColl = rootEnactment.Attributes;

                    newAttr = doc.CreateAttribute("type");
                    newAttr.Value = enactment.type;
                    attrColl.Append(newAttr);

                    newAttr = doc.CreateAttribute("name");
                    newAttr.Value = enactment.name;
                    attrColl.Append(newAttr);

                    newAttr = doc.CreateAttribute("number");
                    newAttr.Value = enactment.number;
                    attrColl.Append(newAttr);

                    newAttr = doc.CreateAttribute("date");
                    newAttr.Value = enactment.date.HasValue ? enactment.date.Value.ToString("yyyy-MM-dd") : string.Empty;
                    attrColl.Append(newAttr);

                    XmlNode rootFounderAuthority = rootEnactment.AppendChild(doc.CreateElement("founderAuthority"));
                    attrColl = rootFounder.Attributes;

                    newAttr = doc.CreateAttribute("regNum");
                    newAttr.Value = "Реестровый номер организации в ОГС";
                    attrColl.Append(newAttr);

                    newAttr = doc.CreateAttribute("fullName");
                    newAttr.Value = ogs.RefOrgGrbsName;
                    attrColl.Append(newAttr);
                }
*/
            }

            rootOktmo = rootAdditional.AppendChild(doc.CreateElement("oktmo"));
            attrColl = rootOktmo.Attributes;

            newAttr = doc.CreateAttribute("code");
            newAttr.Value = ogs.RefOktmoCode;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);
            }

            newAttr = doc.CreateAttribute("name");
            newAttr.Value = ogs.RefOktmoName;
            if (attrColl != null)
            {
                attrColl.Append(newAttr);

                foreach (ActivityViewModel rec in activity)
                {
                    XmlNode rootActivity = rootAdditional.AppendChild(doc.CreateElement("activity"));
                    attrColl = rootActivity.Attributes;

                    newAttr = doc.CreateAttribute("name");
                    newAttr.Value = rec.name;
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }

                    XmlNode rootOkved1 = rootActivity.AppendChild(doc.CreateElement("okved"));
                    attrColl = rootOkved1.Attributes;

                    newAttr = doc.CreateAttribute("code");
                    newAttr.Value = rec.OKVEDcode; // .ToString("##'.'#'.'#'.'#'.'#");
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }

                    newAttr = doc.CreateAttribute("name");
                    newAttr.Value = rec.OKVEDname;
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }

                    newAttr = doc.CreateAttribute("type");
                    newAttr.Value = rec.OKVEDtype == 1 ? "C" : "O";
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }
                }

                foreach (DocumentViewModel rec in documents)
                {
                    XmlNode rootDocument = rootPosition.AppendChild(doc.CreateElement("document"));
                    attrColl = rootDocument.Attributes;

                    newAttr = doc.CreateAttribute("name");
                    newAttr.Value = rec.name;
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);

                        if (rec.date.HasValue)
                        {
                            newAttr = doc.CreateAttribute("date");
                            newAttr.Value = rec.date.Value.ToString("yyyy-MM-dd");
                            attrColl.Append(newAttr);
                        }
                    }

                    newAttr = doc.CreateAttribute("url");
                    newAttr.Value = rec.url;
                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }

                    // newAttr = doc.CreateAttribute("code");
                    // newAttr.Value = "Код документа";
                    // attrColl.Append(newAttr);

                    // newAttr = doc.CreateAttribute("content");
                    // newAttr.Value = "Содержимое документа";
                    // attrColl.Append(newAttr);
                    newAttr = doc.CreateAttribute("type");
                    switch (rec.type)
                    {
                        case "1":
                            newAttr.Value = "F";
                            break;
                        case "2":
                            newAttr.Value = "E";
                            break;
                        case "3":
                            newAttr.Value = "S";
                            break;
                        case "4":
                            newAttr.Value = "L";
                            break;
                        case "5":
                            newAttr.Value = "I";
                            break;
                        case "6":
                            newAttr.Value = "O";
                            break;
                    }

                    if (attrColl != null)
                    {
                        attrColl.Append(newAttr);
                    }
                }
            }

            Stream strm = new MemoryStream();
            doc.Save(strm);
            strm.Position = 0;
            return strm;
        }

        #endregion

        private static StringBuilder AddDifference(StringBuilder difference, string oldValue, string newValue, string description)
        {
            return difference.AppendLine(
                string.Format(
                    "{2}({0}=>{1})",
                    oldValue,
                    newValue,
                    description));
        }

        private static void CollectActivityDifference(F_Org_Passport passport, List<Okved> okvedy, StringBuilder difference)
        {
            var oldActivity = passport.Activity.Select(
                okvedy1 =>
                new
                    {
                        name = okvedy1.Name, 
                        code = okvedy1.RefOKVED.Code, 
                        type = okvedy1.RefPrOkved.ID == 1 ? "1" : "0"
                    }).ToList();
            var newActivity = okvedy.Select(
                okved => new
                    {
                        name = okved.Name, 
                        code = okved.KodOkved, 
                        type = okved.Main
                    }).ToList();
            passport
                .Unless(orgPassport => oldActivity.SequenceEqual(newActivity))
                .Do(
                    orgPassport =>
                    AddDifference(
                        difference,
                        string.Join(";", oldActivity.Select(arg => string.Format("({0};{1};{2})", arg.name, arg.code, arg.type)).ToArray()),
                        string.Join(";", newActivity.Select(arg => string.Format("({0};{1};{2})", arg.name, arg.code, arg.type)).ToArray()),
                        "ACTIVITY"));
        }

        private static void EgrulUpdateFounder(F_Org_Passport passport, List<F_F_Founder> newFounders, IList<D_Org_OrgYchr> founders, StringBuilder difference)
        {
            var founderMarkAsDeleted = passport.Founders.Select(founder => founder.RefYchred).ToList()
                .Except(founders)
                .ToList();
            var founderDeleted = passport.Founders
                .Where(founder => founderMarkAsDeleted.Contains(founder.RefYchred))
                .ToList();
            var founderAdded = founders.ToList()
                .Except(passport.Founders.Select(founder => founder.RefYchred).ToList())
                .ToList();

            founderDeleted.Each(
                founder =>
                {
                    AddDifference(difference, founder.RefYchred.Name, string.Empty, "Учредитель удален");
                    newFounders.RemoveAll(x => x.RefYchred == founder.RefYchred);
                });
            founderAdded.Each(
                founder =>
                {
                    AddDifference(difference, string.Empty, founder.Name, "Учредитель добавлен");
                    newFounders.Add(new F_F_Founder { RefPassport = passport, RefYchred = founder });
                });
        }

        private void ProcessEgrulFounder(XmlTextReader xmlFile, IList<D_Org_OrgYchr> founders)
        {
            string ogrn = xmlFile.GetAttribute("OGRN");
            string namep = xmlFile.GetAttribute("NAMEP");

            // string inn = xmlFile.GetAttribute("INN");
            // string kpp = xmlFile.GetAttribute("KPP");
            (orgYchrRepository.FindAll().SingleOrDefault(p => p.Name.Equals(namep)) ??
             new D_Org_OrgYchr
                 {
                     Name = namep, 
                 })
                .Do(ychr => ychr.Code = ogrn)
                .Do(founders.Add);

            // .Do(
            // ychr =>
            // recpassport.Founders.Add(
            // new F_F_Founder
            // {
            // RefPassport = recpassport,
            // RefYchred = ychr
            // }));

            // .Do(ychr => AddDifference(difference, ychr.Name, namep, "Учредитель"))
        }

        /// <summary>
        ///   Ищет ППО по коду
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindPpo(string code)
        {
            int result = -1;

            var data = from p in ppoRepository.FindAll()
                       where p.Code == code
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет OKATO
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindOkato(string code)
        {
            int result = -1;

            var data = from p in okatoRepository.FindAll()
                       where p.Code == code
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет Вид учреждения
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindInstitutionType(string code)
        {
            int result = -1;

            var data = from p in vidOrgRepository.FindAll()
                       where p.Code == code
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет ОКВЕД
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindOkved(string code)
        {
            int result = -1;

            var data = from p in okvedRepository.FindAll()
                       where p.Code.Equals(code)
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет признак ОКВЕД
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindPrOkved(int code)
        {
            int result = -1;

            var data = from p in priznakOKVEDRepository.FindAll()
                       where p.Code == code
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет OKTMO
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindOktmo(string code)
        {
            int result = -1;

            var data = from p in oktmoRepository.FindAll()
                       where p.Code == code
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет OKOPF
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindOkopf(string code)
        {
            int result = -1;

            var data = from p in okopfRepository.FindAll()
                       where p.Code == Convert.ToInt32(code)
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет OrgYchr
        /// </summary>
        /// <returns> Если не найдено то -1 </returns>
        private int FindOrgYchr(string name)
        {
            int result = -1;

            var data = from p in orgYchrRepository.FindAll()
                       where p.Name == name
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет OGS по ИНН и КПП
        /// </summary>
        /// <returns> Если не найдено то false </returns>
        private int FindOgs(string inn, string kpp)
        {
            int result = -1;

            var data = from p in nsiOgsRepository.FindAll()
                       where (p.inn == inn) && (p.kpp == kpp)
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        /// <summary>
        ///   Ищет NsiBudget по коду
        /// </summary>
        /// <returns> Если не найдено то false </returns>
        private int FindNsiBudget(string code)
        {
            int result = -1;

            var data = from p in nsiBudgetRepository.FindAll()
                       where p.Code == code
                       select new
                           {
                               p.ID, 
                           };
            if (data.Any())
            {
                result = data.First().ID;
            }

            return result;
        }

        #region Nested type: Okved

        public struct Okved
        {
            public string KodOkved;
            public string Main;
            public string Name;
        }
        #endregion
    }
}
