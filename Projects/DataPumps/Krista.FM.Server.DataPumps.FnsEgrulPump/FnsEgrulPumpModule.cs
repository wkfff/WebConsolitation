using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using bus.gov.ru.fk.EGRUL;

using Bus.Gov.Ru.Imports;

using bus.gov.ru.types.Item1;

using egrul.nalog.ruvo_rugf_2_311_26_04_04_02_custom;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Extensions;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

using Xml.Schema.Linq;

using LinqToXsdTypeManager = bus.gov.ru.fk.EGRUL.LinqToXsdTypeManager;
using АдрРФЕГРЮЛТип = egrul.nalog.ruvo_rugf_2_311_26_04_04_02_custom.АдрРФЕГРЮЛТип;

namespace Krista.FM.Server.DataPumps.FnsEgrulPump
{
    public class FnsEgrulPumpModule : DataPumpModuleBase
    {
        private static readonly string[] AddressDetailDifference = { "10", "11", "12", "13", "14", "15", "16", "17", "19", "20", "21" };
        private static readonly string[] InstitutionDifference = { "01", "02", "04" };

        private string clientKladrCode;
        private egrulDifference egrulDifferenceDocument;
        private StringBuilder errors;
        private SummaryData summaryData;

        private string ClientLocationRegionNumber
        {
            get { return clientKladrCode.Substring(0, 2); }
        }

        /// <summary>
        /// Выполняет действия по инициализации программы закачки
        /// </summary>
        public override void Initialize(IScheme scheme, string programIdentifier, string userParams)
        {
            base.Initialize(scheme, programIdentifier, userParams);

            // инициализируем NHibernate
            UnityStarter.Initialize();

            NHibernateInitializer.Instance().InitializeNHibernateOnce(
                () => NHibernateSession.InitializeNHibernateSession(
                    new ThreadSessionStorage(),
                    scheme.SchemeDWH.ConnectionString,
                    scheme.SchemeDWH.FactoryName,
                    scheme.SchemeDWH.ServerVersion));

            clientKladrCode = scheme.GlobalConstsManager.Consts["KLADR"].Value.ToString();
            WriteToTrace(string.Format("Текущий регион: {0}, КЛАДР {1}", Region, clientKladrCode), TraceMessageKind.Warning);
        }

        /// <summary>
        /// Закачка данных. 
        /// Переопределяется в потомках для выполнения действий по закачке данных.
        /// </summary>
        protected override void DirectPumpData()
        {
            PumpDataSource(RootDir);
        }

        protected override void MarkClsAsInvalidate()
        {
        }

        protected override void DirectCheckData()
        {
        }

        /// <summary>
        /// теперь по назначению используем
        /// </summary>
        protected override void DirectClsHierarchySetting()
        {
        }

        protected override void DirectDeleteData(int pumpID, int sourceID, string constr)
        {
        }

        protected override void InitDBObjects()
        {
            if (State == PumpProcessStates.PumpData)
            {
                // TODO в новых форматах есть egrulDifferenceType надо посмотреть че там и как
                egrulDifferenceDocument = new egrulDifference
                                               {
                                                   header = new headerType
                                                                {
                                                                    createDateTime = DateTime.Now,
                                                                    id = Guid.NewGuid().ToString()
                                                                },
                                                   body = new egrulDifference.bodyLocalType
                                                              {
                                                                  position = new List<egrulDifferenceType>()
                                                              }
                                               };

                errors = new StringBuilder();
            }
        }

        protected void ProcessFilesEgrul(
                                            DirectoryInfo dir,
                                            string searchPattern,
                                            ProcessFileDelegate processFile,
                                            bool emptyDirException,
                                            SearchOption searchOption)
        {
            WriteToTrace(string.Format("Запрос списка файлов по маске {0}", searchPattern), TraceMessageKind.Information);
            FileInfo[] files = dir.GetFiles(searchPattern, searchOption);
            if (files.GetLength(0) == 0 && emptyDirException)
            {
                throw new Exception("В каталоге источника нет данных для закачки");
            }

            string sourcePath = GetShortSourcePathBySourceID(SourceID);
            int totalFiles = files.GetLength(0);
            
            files.OrderBy(x => x.Name).Each(
                (x, i) =>
                    {
                        SetProgress(
                                    totalFiles, 
                                    i + 1,
                                    string.Format("Обработка файла {0}\\{1}...", sourcePath, x.Name),
                                    string.Format("Файл {0} из {1}", i + 1, totalFiles),
                                    true);

                        if (x.Exists && x.Directory != null && !x.Directory.Name.StartsWith("__"))
                        {
                            WriteEventIntoDataPumpProtocol(
                                DataPumpEventKind.dpeStartFilePumping,
                                string.Format("Старт закачки файла {0}.", x.FullName));

                            try
                            {
                                processFile(x);

                                WriteEventIntoDataPumpProtocol(
                                    DataPumpEventKind.dpeSuccessfullFinishFilePump,
                                    string.Format("Закачка файла {0} успешно завершена.", x.FullName));
                            }
                            catch (ThreadAbortException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                WriteEventIntoDataPumpProtocol(
                                    DataPumpEventKind.dpeFinishFilePumpWithError,
                                    string.Format("Закачка файла {0} завершена с ошибками.", x.FullName),
                                    ex);
                                throw;
                            }
                        }
                    });
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            SetProgress(0, 0, "Выполняется распаковка архивов исходных данных", string.Empty, true);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Распаковка архивов исходных данных");
            DirectoryInfo archiveDir = CommonRoutines.GetTempDir();
            CommonRoutines.ExtractArchiveFiles(
                dir.FullName,
                archiveDir.FullName,
                ArchivatorName.Zip,
                FilesExtractingOption.SeparateSubDirs);
            try
            {
                ProcessFilesEgrul(archiveDir, "*.xml", ProcessFile, true, SearchOption.AllDirectories);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(archiveDir);
            }
        }

        protected override void PumpFinalizing()
        {
            Scheme.MessageManager.SendMessage(
                new MessageWrapper
                {
                    Subject = string.Format("Протокол загрузки ЕГРЮЛ от {0}", DateTime.Now),
                    DateTimeOfCreation = DateTime.Now,
                    DateTimeOfActual = DateTime.Now.AddDays(14),
                    MessageStatus = MessageStatus.New,
                    MessageImportance = MessageImportance.Importance,
                    MessageType = MessageType.AdministratorMessage,
                    RefGroupRecipient = 2,
                    RefMessageAttachment =
                        new MessageAttachmentWrapper
                        {
                            Document =
                                GetAttachmentContent(egrulDifferenceDocument, errors.ToString(), summaryData),
                            DocumentFileName =
                                string.Format("ed_{0}.xml.zip", DateTime.Now.ToString("yyyyMMddhhmmss")),
                            DocumentName =
                                string.Format("приложение к протоколу загрузки ЕГРЮЛ от {0}", DateTime.Now)
                        },
                    Body = GetReport(summaryData)
                });
        }

        private static byte[] GetAttachmentContent(
                                                   egrulDifference egrulDifference,
                                                   string errors,
                                                   SummaryData summaryData)
        {
            using (var zipStream = new MemoryStream())
            {
                using (Package package = Package.Open(zipStream, FileMode.CreateNew))
                {
                    Uri partUriEgrulDefference =
                        PackUriHelper.CreatePartUri(
                            new Uri(
                                string.Format("ed_{0}.xml", DateTime.Now.ToString("yyyyMMddhhmmss")),
                                UriKind.Relative));
                    PackagePart packagePartEgrulDefference = package.CreatePart(
                        partUriEgrulDefference,
                        MediaTypeNames.Text.Xml,
                        CompressionOption.Maximum);
                    using (var xmlWriter = new XmlTextWriter(packagePartEgrulDefference.GetStream(), Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlWriter.Indentation = 2;
                        egrulDifference.Save(xmlWriter);
                    }

                    Uri partUriPumpErrors = PackUriHelper.CreatePartUri(new Uri("pump_error.log", UriKind.Relative));
                    PackagePart packagePartPumpErrors = package.CreatePart(
                        partUriPumpErrors,
                        MediaTypeNames.Text.Plain,
                        CompressionOption.Maximum);
                    using (var streamWriter = new StreamWriter(packagePartPumpErrors.GetStream()))
                    {
                        streamWriter.Write(errors);
                    }

                    Uri partUriPumpReport = PackUriHelper.CreatePartUri(new Uri("report.txt", UriKind.Relative));
                    PackagePart packagePartPumpReport = package.CreatePart(
                        partUriPumpReport,
                        MediaTypeNames.Text.Plain,
                        CompressionOption.Maximum);
                    using (var streamWriter = new StreamWriter(packagePartPumpReport.GetStream()))
                    {
                        streamWriter.WriteLine(GetReport(summaryData));
                    }
                }

                return zipStream.ToArray();
            }
        }

        private static string GetReport(SummaryData summaryData)
        {
            var sb = new StringBuilder()
                .AppendLine(string.Format("Файлы источника успешно обработаны [{0}]", summaryData.Files))
                .AppendLine(string.Format("Всего учреждений отобрано {0}", summaryData.All))
                .AppendLine(string.Format("\t{0} - не найдено учреждений", summaryData.NewStructures))
                .AppendLine(string.Format("\t{0} - добавлено паспортов", summaryData.NewDocuments))
                .AppendLine(string.Format("\t{0} - обновлено", summaryData.Modified))
                .AppendLine(string.Format("\t{0} - ошибки во время обработки", summaryData.Errors))
                .AppendLine(string.Format("\t{0} - без изменений", summaryData.NonModified))
                .AppendLine(string.Format("\nУчреджений сайта интеграции отсутствует в ЕГРЮЛ {0}", string.Empty));

            if (summaryData.Messages.IsNotNullOrEmpty())
            {
                sb.AppendLine(string.Format("\nДополнительные замечания: {0}", summaryData.Messages));
            }

            return sb.ToString();
        }

        private static IList<egrulDifferenceType.differenceLocalType> ShortDifference(EGRUL.СвЮЛLocalType ultype, string type)
        {
            var list = new List<egrulDifferenceType.differenceLocalType>();
            CheckDifferences(list, ultype.СвНаимЮЛ.НаимЮЛПолн, string.Empty, "01", type);
            CheckDifferences(list, ultype.ИНН, string.Empty, "03", type);
            CheckDifferences(list, ultype.КПП, string.Empty, "04", type);
            return list;
        }

        private static IList<egrulDifferenceType.differenceLocalType> FullDifference(EGRUL.СвЮЛLocalType ultype)
        {
            return ShortDifference(ultype, "03");
        }
        
        private static void CheckDifferencesDolgnfl(
                ICollection<egrulDifferenceType.differenceLocalType> differences,
                EGRUL.СвЮЛLocalType ul,
                F_Org_Passport activePassport)
        {
            ul.СведДолжнФЛ
                .OrderByDescending(type => type.ГРНДатаПерв.ДатаЗаписи)
                .FirstOrDefault()
                .Do(
                    type =>
                    {
                        CheckDifferences(differences, type.СвФЛ.Фамилия, activePassport.Fam, "23");
                        CheckDifferences(differences, type.СвФЛ.Имя, activePassport.NameRuc, "24");
                        CheckDifferences(differences, type.СвФЛ.Отчество, activePassport.Otch, "25");
                        CheckDifferences(differences, type.СвДолжн.НаимДолжн, activePassport.Ordinary, "26");
                    });
        }

        private static void CheckDifferencesAddress(
                ICollection<egrulDifferenceType.differenceLocalType> differences,
                АдрРФЕГРЮЛТип address,
                string adr)
        {
            Match originalAddress = GetPartsOfAddress(adr);
            CheckDifferences(differences, address.Регион.НаимРегион, originalAddress.Groups[1].Value.Trim(), "11");
            CheckDifferences(differences, address.Регион.ТипРегион, originalAddress.Groups[2].Value.Trim(), "10");
            
            CheckDifferences(differences, address.Район.With(type => type.НаимРайон), originalAddress.Groups[3].Value.Trim(), "13");
            CheckDifferences(differences, address.Район.With(type => type.ТипРайон), originalAddress.Groups[4].Value.Trim(), "12");

            CheckDifferences(differences, address.Город.With(type => type.ТипГород), originalAddress.Groups[5].Value.Trim(), "14");
            CheckDifferences(differences, address.Город.With(type => type.НаимГород), originalAddress.Groups[6].Value.Trim(), "15");

            CheckDifferences(differences, address.НаселПункт.With(type => type.ТипНаселПункт), originalAddress.Groups[7].Value.Trim(), "16");
            CheckDifferences(differences, address.НаселПункт.With(type => type.НаимНаселПункт), originalAddress.Groups[8].Value.Trim(), "17");

            CheckDifferences(differences, address.Улица.With(type => type.НаимУлица), originalAddress.Groups[9].Value.Trim(), "19");

            CheckDifferences(differences, address.Дом, originalAddress.Groups[10].Value.Trim(), "20");
            CheckDifferences(differences, address.Корпус, originalAddress.Groups[11].Value.Trim(), "21");
        }

        private static void CheckDifferencesActivity(
                ICollection<egrulDifferenceType.differenceLocalType> differences,
                EGRUL.СвЮЛLocalType.СвОКВЭДLocalType okvedTypes,
                IEnumerable<F_F_OKVEDY> activity)
        {
            List<string> okvedLocal = new List<string>();
            if (okvedTypes != null)
            {
                okvedTypes.СвОКВЭДОсн.Do(x => okvedLocal.Add(new OKVEDType
                                                                    {
                                                                        KodOKVED = x.КодОКВЭД,
                                                                        Main = D_Org_PrOKVED.MainID.ToString()
                                                                    }.Untyped.ToString()));

                okvedTypes.СвОКВЭДДоп.Each(x => okvedLocal.Add(new OKVEDType
                {
                    KodOKVED = x.КодОКВЭД,
                    Main = D_Org_PrOKVED.OtherID.ToString()
                }.Untyped.ToString()));
            }

            List<string> activityLocal = activity.Select(
                okvedy => new OKVEDType
                {
                    KodOKVED = okvedy.RefOKVED.Code,
                    Main = okvedy.RefPrOkved.Code.ToString()
                }.Untyped.ToString()).ToList();

            activityLocal.Except(okvedLocal).Each(
                type => differences.Add(
                    new egrulDifferenceType.differenceLocalType
                    {
                        detail = "08",
                        originalData = ((OKVEDType)XElement.Parse(type)).KodOKVED,
                        type = "02"
                    }));

            okvedLocal.Except(activityLocal).Each(
                type => differences.Add(
                    new egrulDifferenceType.differenceLocalType
                    {
                        detail = "08",
                        egrulData = type,
                        type = "03"
                    }));
        }
        
        private static void CheckDifferences(
                ICollection<egrulDifferenceType.differenceLocalType> differences,
                string egrulData,
                string originalData,
                string detail,
                string typeD = "01")
        {
            var caseInsensitiveDetails = new List<string> { "23", "24", "25", "26" };
            if (!string.IsNullOrEmpty(egrulData)
                && ((!caseInsensitiveDetails.Contains(detail) && !originalData.Equals(egrulData))
                    ||
                    (caseInsensitiveDetails.Contains(detail)
                     && !originalData.Equals(egrulData, StringComparison.InvariantCultureIgnoreCase))))
            {
                differences.Add(
                    new egrulDifferenceType.differenceLocalType
                    {
                        detail = detail,
                        type = typeD,
                        egrulData = egrulData
                    }.Do(
                            type =>
                            {
                                if (!string.IsNullOrEmpty(originalData))
                                {
                                    type.originalData = originalData;
                                }
                            }));
            }
        }

        private static string ProcessInstitutionDifferences(
                IEnumerable<egrulDifferenceType.differenceLocalType> differences,
                D_Org_Structure orgStructure)
        {
            var note = new StringBuilder();
            foreach (egrulDifferenceType.differenceLocalType difference in differences)
            {
                switch (difference.detail)
                {
                    case "01":
                        note.AppendLine(
                            string.Format("полное наименование {0}=>{1}", difference.originalData, difference.egrulData));
                        orgStructure.Name = difference.egrulData;
                        break;
                    case "02":
                        note.AppendLine(
                            string.Format(
                                "сокращенное наименование {0}=>{1}", difference.originalData, difference.egrulData));
                        orgStructure.ShortName = difference.egrulData;
                        break;
                    case "04":
                        note.AppendLine(string.Format("КПП {0}=>{1}", difference.originalData, difference.egrulData));
                        orgStructure.KPP = difference.egrulData;
                        break;
                }
            }

            return note.ToString();
        }
        
        private static void ProcessDifferences(
                List<egrulDifferenceType.differenceLocalType> differences,
                D_Org_Structure orgStructure,
                F_F_ParameterDoc header)
        {
            F_Org_Passport passport = header.Passports.First();
            var note = new StringBuilder();

            note.AppendLine(ProcessInstitutionDifferences(differences, orgStructure));

            foreach (egrulDifferenceType.differenceLocalType difference in differences)
            {
                switch (difference.detail)
                {
                    case "05":
                        note.AppendLine(string.Format("ОГРН {0}=>{1}", difference.originalData, difference.egrulData));
                        passport.OGRN = difference.egrulData;
                        break;
                    case "09":
                        note.AppendLine(
                            string.Format("Почтовый индекс {0}=>{1}", difference.originalData, difference.egrulData));
                        passport.Indeks = difference.egrulData;
                        break;
                    case "23":
                        note.AppendLine(
                            string.Format(
                                "Фамилия руководителя {0}=>{1}", difference.originalData, difference.egrulData));
                        passport.Fam = difference.egrulData;
                        break;
                    case "24":
                        note.AppendLine(
                            string.Format("Имя руководителя {0}=>{1}", difference.originalData, difference.egrulData));
                        passport.NameRuc = difference.egrulData;
                        break;
                    case "25":
                        note.AppendLine(
                            string.Format(
                                "Отчество руководителя {0}=>{1}", difference.originalData, difference.egrulData));
                        passport.Otch = difference.egrulData;
                        break;
                    case "26":
                        note.AppendLine(
                            string.Format(
                                "Должность руководителя {0}=>{1}", difference.originalData, difference.egrulData));
                        passport.Ordinary = difference.egrulData;
                        break;
                    case "11":
                        note.AppendLine(
                            string.Format(
                                "Наименование субъекта РФ {0}=>{1}",
                                difference.originalData,
                                difference.egrulData));
                        break;
                    case "15":
                        note.AppendLine(
                            string.Format(
                                "Наименование города {0}=>{1}",
                                difference.originalData,
                                difference.egrulData));
                        break;
                    case "17":
                        note.AppendLine(
                            string.Format(
                                "Наименование населенного пункта {0}=>{1}",
                                difference.originalData,
                                difference.egrulData));
                        break;
                    case "19":
                        note.AppendLine(
                            string.Format(
                                "Наименование улицы {0}=>{1}",
                                difference.originalData,
                                difference.egrulData));
                        break;
                    case "20":
                        note.AppendLine(
                            string.Format(
                                "Номер дома {0}=>{1}",
                                difference.originalData,
                                difference.egrulData));
                        break;
                    case "21":
                        note.AppendLine(
                            string.Format(
                                "Номер офиса (квартиры) {0}=>{1}",
                                difference.originalData,
                                difference.egrulData));
                        break;
                    case "07":
                        if (difference.type == "02")
                        {
                            note.AppendLine(
                                string.Format(
                                    "Наименование учредителя {0}=>{1}", difference.originalData, difference.egrulData));
                            passport.Founders.Remove(
                                passport.Founders
                                    .First(
                                        founder =>
                                        founder.RefYchred.Name.Equals(
                                            difference.originalData)));
                        }
                        else if (difference.type == "03")
                        {
                            var rul = (EGRUL.СвЮЛLocalType.СвУчредитLocalType.УчрЮЛРосLocalType)XElement.Parse(difference.egrulData);
                            note.AppendLine(string.Format("Наименование учредителя {0}=>{1}", string.Empty, rul.НаимИННЮЛ.НаимЮЛПолн));

                            passport.Founders.Add(
                                new F_F_Founder
                                {
                                    RefPassport = passport,
                                    RefYchred = Resolver.Get<ILinqRepository<D_Org_OrgYchr>>().FindAll().ToList()
                                                        .FirstOrDefault(ychr => ychr.Name.Equals(rul.НаимИННЮЛ.НаимЮЛПолн)) ??
                                                new D_Org_OrgYchr
                                                {
                                                    Name = rul.НаимИННЮЛ.НаимЮЛПолн,
                                                    Code = rul.НаимИННЮЛ.ОГРН,
                                                    RefNsiOgs = Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll()
                                                      .FirstOrDefault(x => x.Stats.Equals("801") && x.inn.Equals(rul.НаимИННЮЛ.ИНН))
                                                }
                                });
                            difference.egrulData = rul.НаимИННЮЛ.НаимЮЛПолн;
                        }

                        break;
                    case "08":
                        if (difference.type == "02")
                        {
                            note.AppendLine(
                                string.Format("ОКВЭД {0}=>{1}", difference.originalData, difference.egrulData));
                            passport.Activity.Remove(
                                passport.Activity
                                    .First(okvedy => okvedy.RefOKVED.Code.Equals(difference.originalData)));
                        }
                        else if (difference.type == "03")
                        {
                            var okved = (OKVEDType)XElement.Parse(difference.egrulData);
                            note.AppendLine(string.Format("ОКВЭД {0}=>{1}", string.Empty, okved.KodOKVED));
                            var okvedCls = CommonPump.GetActualOKVED(passport.RefParametr, okved.KodOKVED);
                            passport.Activity.Add(
                                new F_F_OKVEDY
                                {
                                    RefPassport = passport,
                                    Name = okvedCls.Name,
                                    RefOKVED = okvedCls,
                                    RefPrOkved = Resolver.Get<ILinqRepository<D_Org_PrOKVED>>()
                                        .Load(Convert.ToInt32(okved.Main))
                                });

                            difference.egrulData = okved.KodOKVED;
                        }

                        break;
                }
            }

            if (differences.Any(type => AddressDetailDifference.Contains(type.detail)))
            {
                Match originalAddress = GetPartsOfAddress(passport.Adr);

                passport.Adr = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    differences.FirstOrDefault(type => type.detail.Equals("11")).With(type => type.egrulData) ?? originalAddress.Groups[1].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("10")).With(type => type.egrulData) ?? originalAddress.Groups[2].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("13")).With(type => type.egrulData) ?? originalAddress.Groups[3].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("12")).With(type => type.egrulData) ?? originalAddress.Groups[4].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("14")).With(type => type.egrulData) ?? originalAddress.Groups[5].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("15")).With(type => type.egrulData) ?? originalAddress.Groups[6].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("16")).With(type => type.egrulData) ?? originalAddress.Groups[7].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("17")).With(type => type.egrulData) ?? originalAddress.Groups[8].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("19")).With(type => type.egrulData) ?? originalAddress.Groups[9].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("20")).With(type => type.egrulData) ?? originalAddress.Groups[10].Value,
                    differences.FirstOrDefault(type => type.detail.Equals("21")).With(type => type.egrulData) ?? originalAddress.Groups[11].Value);
            }

            header.Note = note.ToString().With(s => s.Substring(0, Math.Min(s.Length, 1024)));
          
            {
                header.RefSost =
                    Resolver.Get<ILinqRepository<FX_Org_SostD>>().Load(FX_Org_SostD.OnEditingStateID);
            }
        }

        private static F_F_ParameterDoc ClonePassport(F_F_ParameterDoc activeHeader)
        {
            var header = new F_F_ParameterDoc
            {
                RefPartDoc = activeHeader.RefPartDoc,
                RefSost = activeHeader.RefSost,
                RefYearForm = Resolver.Get<ILinqRepository<FX_Fin_YearForm>>().Load(DateTime.Now.Year),
                OpeningDate = DateTime.Now,
                PlanThreeYear = false,
                RefUchr = activeHeader.RefUchr
            };
            
            F_Org_Passport activePassport = activeHeader.Passports.First();
            var passport = new F_Org_Passport
            {
                RefParametr = header,
                OGRN = activePassport.OGRN,
                Indeks = activePassport.Indeks,
                Phone = activePassport.Phone,
                Ordinary = activePassport.Ordinary,
                Fam = activePassport.Fam,
                NameRuc = activePassport.NameRuc,
                Otch = activePassport.Otch,
                Adr = activePassport.Adr,
                Website = activePassport.Website,
                Mail = activePassport.Mail,
                OKPO = activePassport.OKPO,
                RefOKOPF = activePassport.RefOKOPF,
                RefOKATO = activePassport.RefOKATO,
                RefCateg = activePassport.RefCateg,
                RefOKFS = activePassport.RefOKFS,
                RefOKTMO = activePassport.RefOKTMO,
                RefRaspor = activePassport.RefRaspor,
                RefVid = activePassport.RefVid
            };

            passport.Activity = activePassport.Activity.Select(
                okvedy => new F_F_OKVEDY
                {
                    RefPassport = passport,
                    Name = okvedy.Name,
                    RefPrOkved = okvedy.RefPrOkved,
                    RefOKVED = okvedy.RefOKVED
                }).ToList();

            passport.Branches = activePassport.Branches.Select(
                filial => new F_F_Filial
                {
                    RefPassport = passport,
                    Name = filial.Name,
                    Nameshot = filial.Nameshot,
                    Code = filial.Code,
                    INN = filial.INN,
                    KPP = filial.KPP,
                    RefTipFi = filial.RefTipFi
                }).ToList();

            passport.Founders = activePassport.Founders.Select(
                founder => new F_F_Founder
                {
                    RefPassport = passport,
                    formative = founder.formative,
                    stateTask = founder.stateTask,
                    supervisoryBoard = founder.supervisoryBoard,
                    RefYchred = founder.RefYchred
                }).ToList();

            header.Passports.Add(passport);
            return header;
        }

        private static F_F_ParameterDoc NewFfParameterDoc(EGRUL.СвЮЛLocalType ul, string idFile)
        {
            var header = new F_F_ParameterDoc
            {
                RefPartDoc = Resolver.Get<ILinqRepository<FX_FX_PartDoc>>()
                        .Load(FX_FX_PartDoc.PassportDocTypeID),
                RefSost = Resolver.Get<ILinqRepository<FX_Org_SostD>>()
                        .Load(FX_Org_SostD.OnEditingStateID),
                RefYearForm =
                        Resolver.Get<ILinqRepository<FX_Fin_YearForm>>().Load(DateTime.Now.Year),
                OpeningDate = DateTime.Now,
                PlanThreeYear = false,
                Note = string.Format("Импорт ЕРГЮЛ  ID={0}", idFile)
            };

            var ordinary = ul.СведДолжнФЛ
                                .OrderByDescending(x => x.СвДолжн.ГРНДата)
                                    .First();

            var passport = new F_Org_Passport
            {
                RefParametr = header,
                OGRN = ul.ОГРН,
                Indeks = ul.СвАдресЮЛ.АдресРФ.Индекс ?? string.Empty,
                Phone = ul.СведДолжнФЛ.FirstOrDefault().With(x => x.СвНомТел.НомТел) ?? string.Empty,
                Ordinary = ordinary.СвДолжн.НаимДолжн,
                Fam = ordinary.СвФЛ.Фамилия,
                NameRuc = ordinary.СвФЛ.Имя,
                Otch = ordinary.СвФЛ.Отчество,
                RefOKOPF = ul.If(opf => opf.СпрОПФ.IsNotNullOrEmpty() && opf.СпрОПФ.Equals("ОКОПФ")).With(
                        opf => Resolver.Get<ILinqRepository<D_OKOPF_OKOPF>>().FindAll()
                                       .SingleOrDefault(
                                               okopf =>
                                               okopf.Code == Convert.ToInt32(opf.КодОПФ))),
                Adr = AddressFormat(ul)
            };

            if (ul.СвОКВЭД != null)
            {
                var okveds = new List<F_F_OKVEDY>();

                ul.СвОКВЭД.СвОКВЭДОсн.Do(x => okveds.Add(new F_F_OKVEDY
                                                            {
                                                                RefPassport = passport,
                                                                Name = CommonPump.GetActualOKVED(passport.RefParametr, x.КодОКВЭД).Name,
                                                                RefOKVED = CommonPump.GetActualOKVED(passport.RefParametr, x.КодОКВЭД),
                                                                RefPrOkved = Resolver.Get<ILinqRepository<D_Org_PrOKVED>>()
                                                                            .Load(D_Org_PrOKVED.MainID)
                                                            }));

                ul.СвОКВЭД.With(x => x.СвОКВЭДДоп.If(o => o.Any())).Each(
                    x =>
                        {
                            okveds.Add(
                                new F_F_OKVEDY
                                    {
                                        RefPassport = passport,
                                        Name = CommonPump.GetActualOKVED(passport.RefParametr, x.КодОКВЭД).Name,
                                        RefOKVED = CommonPump.GetActualOKVED(passport.RefParametr, x.КодОКВЭД),
                                        RefPrOkved = Resolver.Get<ILinqRepository<D_Org_PrOKVED>>().Load(D_Org_PrOKVED.OtherID)
                                    });
                        });

                passport.Activity = okveds;
            }
            
            var uchrUl = ul.СвУчредит.With(uchr => uchr.УчрЮЛРос.If(ulr => ulr.Any()));
            if (uchrUl != null)
            {
                passport.Founders = uchrUl.Select(
                        rul => new F_F_Founder
                        {
                            RefPassport = passport,
                            RefYchred =
                                    Resolver.Get<ILinqRepository<D_Org_OrgYchr>>().FindAll().ToList()
                                            .FirstOrDefault(ychr => ychr.Name.Equals(rul.НаимИННЮЛ.НаимЮЛПолн))
                                    ?? new D_Org_OrgYchr
                                    {
                                        Name = rul.НаимИННЮЛ.НаимЮЛПолн,
                                        Code = rul.НаимИННЮЛ.ОГРН,
                                        RefNsiOgs = Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll()
                                          .FirstOrDefault(x => x.Stats.Equals("801") && x.inn.Equals(rul.НаимИННЮЛ.ИНН))
                                    }
                        })
                        .ToList();    
            }

            header.Passports.Add(passport);
            return header;
        }

        private static Match GetPartsOfAddress(string adr)
        {
            return Regex.Match(adr, @"([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*),([\w\s]*)");
        }

        private static string AddressFormat(EGRUL.СвЮЛLocalType ul)
        {
            return string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                ul.СвАдресЮЛ.АдресРФ.Регион.НаимРегион,
                ul.СвАдресЮЛ.АдресРФ.Регион.ТипРегион,
                ul.СвАдресЮЛ.АдресРФ.Район.With(raion => raion.НаимРайон),
                ul.СвАдресЮЛ.АдресРФ.Район.With(raion => raion.ТипРайон),
                ul.СвАдресЮЛ.АдресРФ.Город.With(gorod => gorod.ТипГород),
                ul.СвАдресЮЛ.АдресРФ.Город.With(gorod => gorod.НаимГород),
                ul.СвАдресЮЛ.АдресРФ.НаселПункт.With(naspunkt => naspunkt.ТипНаселПункт),
                ul.СвАдресЮЛ.АдресРФ.НаселПункт.With(naspunkt => naspunkt.НаимНаселПункт),
                ul.СвАдресЮЛ.АдресРФ.Улица.With(street => street.НаимУлица),
                ul.СвАдресЮЛ.АдресРФ.Дом,
                ul.СвАдресЮЛ.АдресРФ.Корпус);
        }

        private void CheckDifferencesFounder(
                ICollection<egrulDifferenceType.differenceLocalType> differences,
                IEnumerable<EGRUL.СвЮЛLocalType.СвУчредитLocalType.УчрЮЛРосLocalType> rul,
                IEnumerable<string> founders)
        {
            if (rul != null)
            {
                var rulTypes = rul as List<EGRUL.СвЮЛLocalType.СвУчредитLocalType.УчрЮЛРосLocalType> ?? rul.ToList();
                List<string> rulNames = rulTypes.Select(type => type.НаимИННЮЛ.НаимЮЛПолн).ToList();
                List<string> founderNames = founders.ToList();
                founderNames.Except(rulNames).Each(
                    s => differences.Add(
                        new egrulDifferenceType.differenceLocalType
                        {
                            detail = "07",
                            originalData = s,
                            type = "02"
                        }));
                rulNames.Except(founderNames).Each(
                    s => differences.Add(
                        new egrulDifferenceType.differenceLocalType
                        {
                            detail = "07",
                            egrulData = rulTypes.First(type => type.НаимИННЮЛ.НаимЮЛПолн.Equals(s))
                                .Untyped.ToString(),
                            type = "03"
                        }));
            }
            else
            {
                summaryData.SetMessage("В импортируемом файле отсутствуют Учредители");
            }
        }

        private void CollectDifferences(
               ICollection<egrulDifferenceType.differenceLocalType> differences,
               EGRUL.СвЮЛLocalType ul,
               D_Org_Structure orgStructure,
               F_Org_Passport activePassport)
        {
            CheckDifferences(differences, ul.СвНаимЮЛ.НаимЮЛПолн, orgStructure.Name, "01");
            CheckDifferences(differences, ul.СвНаимЮЛ.НаимЮЛСокр, orgStructure.ShortName, "02");
            CheckDifferences(differences, ul.КПП, orgStructure.KPP, "04");
            CheckDifferences(differences, ul.ОГРН, activePassport.OGRN, "05");
            CheckDifferences(differences, ul.СвАдресЮЛ.АдресРФ.Индекс, activePassport.Indeks, "09");

            CheckDifferencesDolgnfl(differences, ul, activePassport);
            CheckDifferencesAddress(differences, ul.СвАдресЮЛ.АдресРФ, activePassport.Adr);

            CheckDifferencesFounder(
                differences,
                ul.СвУчредит.With(uchr => uchr.УчрЮЛРос.If(ulr => ulr.Any())),
                activePassport.Founders.Select(founder => founder.RefYchred.Name));

            CheckDifferencesActivity(differences, ul.СвОКВЭД, activePassport.Activity);
        }

        private void ProcessFile(FileInfo fileInfo)
        {
            EGRUL pumpFile = EGRUL.Load(fileInfo.FullName);
            List<EGRUL.СвЮЛLocalType> pumpDocs = pumpFile.СвЮЛ.Where(type => type.ИНН.IsNotNullOrEmpty() && type.ИНН.StartsWith(ClientLocationRegionNumber)).ToList();
            int pumpDocsCount = pumpDocs.Count;
            summaryData.All += pumpDocsCount;
            summaryData.Files += fileInfo.Name + " ";
            
            SetProgress(1, -1, "Обработка данных", string.Empty);
            int counter = 1;
            foreach (EGRUL.СвЮЛLocalType pumpDoc in pumpDocs)
            {
                try
                {
                    using (new PersistenceContext())
                    {
                        SetProgress(pumpDocsCount, counter, string.Empty, string.Format("{0} из {1}", counter++, pumpDocsCount), true);
                        WriteToTrace("Обработка организации", TraceMessageKind.Information);
                        ProcessUlData(pumpDoc, fileInfo.FullName);
                    }
                }
                catch (Exception e)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeError,
                        string.Format("Ошибки во время обработки учреждения [{0}]", pumpDoc.Untyped),
                        e);
                    AddEdRecord(ShortDifference(pumpDoc, "01"), false);
                    errors.AppendLine(pumpDoc.Untyped.ToString());
                    ++summaryData.Errors;
                }
            }
        }

        private void AddEdRecord(
                IList<egrulDifferenceType.differenceLocalType> differenceLocalTypes,
                bool originalRewrite,
                bool egrulAbsent = false,
                string regNum = null)
        {
            egrulDifferenceDocument.body.position.Add(
                new egrulDifferenceType
                    {
                        positionId = Guid.NewGuid().ToString(),
                        createDate = DateTime.Now,
                        originalRewrite = originalRewrite,
                        egrulAbsent = egrulAbsent,
                        difference = differenceLocalTypes
                    }.Do(type => regNum.Do(s => type.regNum = s)));
        }

        [UnitOfWork]
        private void ProcessUlData(EGRUL.СвЮЛLocalType ul, string idFile)
        {
            var repository = Resolver.Get<ILinqRepository<D_Org_Structure>>();
            repository.DbContext.BeginTransaction();

            D_Org_Structure orgStructure;

            try
            {
                orgStructure = repository.FindAll().SingleOrDefault(structure => structure.INN.Equals(ul.ИНН));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Ошибка поиска учреждения по ИНН. Возможно дублирование учреждений с ИНН: {0}.".FormatWith(ul.ИНН), e);
            }

            if (orgStructure != null && (orgStructure.CloseDate == null || orgStructure.CloseDate > DateTime.Now))
            {
                F_Org_Passport activePassport =
                    orgStructure.Documents
                        .Where(doc => !doc.CloseDate.HasValue)
                        .OrderByDescending(doc => doc.OpeningDate)
                        .SelectMany(doc => doc.Passports)
                        .FirstOrDefault() ??
                    orgStructure.Documents
                        .OrderByDescending(doc => doc.CloseDate)
                        .SelectMany(doc => doc.Passports)
                        .FirstOrDefault();

                if (activePassport != null)
                {
                    UpdatePassportData(ul, orgStructure, activePassport);
                }
                else
                {
                    WriteToTrace("Добавление учреждению нового паспорта", TraceMessageKind.Information);
                    orgStructure.Name = ul.СвНаимЮЛ.НаимЮЛПолн;
                    orgStructure.ShortName = ul.СвНаимЮЛ.НаимЮЛСокр.Return(s => s, orgStructure.ShortName);
                    orgStructure.KPP = ul.КПП;
                    orgStructure.Documents.Add(NewFfParameterDoc(ul, idFile).Do(doc => doc.RefUchr = orgStructure));
                    AddEdRecord(
                        FullDifference(ul),
                        false,
                        false,
                        Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll()
                            .FirstOrDefault(ogs => ogs.inn.Equals(ul.ИНН)).With(ogs => ogs.regNum));

                    ++summaryData.NewDocuments;
                }
            }
            else
            {
                if (orgStructure == null)
                {
                    WriteToTrace("Учреждение {0} не найдено".FormatWith(ul.ИНН), TraceMessageKind.Information);
                    AddEdRecord(ShortDifference(ul, "03"), false);

                    ++summaryData.NewStructures;
                }
                else
                {
                    WriteToTrace("Обработка закрытого учреждения", TraceMessageKind.Information);
                }
            }

            repository.DbContext.CommitChanges();
            repository.DbContext.CommitTransaction();
        }

        private void UpdatePassportData(EGRUL.СвЮЛLocalType ul, D_Org_Structure orgStructure, F_Org_Passport activePassport)
        {
            WriteToTrace("Сбор информации о различающихся данных", TraceMessageKind.Information);
            var differences = new List<egrulDifferenceType.differenceLocalType>();
            CollectDifferences(differences, ul, orgStructure, activePassport);

            // если только изменения по учреждению то с паспортами ничего не делаем
            if (differences.All(type => InstitutionDifference.Contains(type.detail)))
            {
                summaryData.SetMessage(ProcessInstitutionDifferences(differences, orgStructure));
                ++summaryData.Modified;
            }
            else
            {
                if (differences.Any(type => !AddressDetailDifference.Contains(type.detail)))
                {
                    /*Если исходный паспорт закрыт- создаем новый, иначе вносим изменения в него*/
                    if (activePassport.RefParametr.CloseDate.HasValue
                        || activePassport.RefParametr.RefSost.ID == FX_Org_SostD.ExportedStateID)
                    {
                        WriteToTrace("Клонирование документа и применение изменений", TraceMessageKind.Information);
                        
                        F_F_ParameterDoc newActiveHeader = ClonePassport(activePassport.RefParametr);
                        orgStructure.Documents.Add(newActiveHeader);
                        
                        ProcessDifferences(differences, orgStructure, newActiveHeader);
                    }
                    else
                    {
                        ProcessDifferences(differences, orgStructure, activePassport.RefParametr);
                    }

                    AddEdRecord(differences, true);
                    ++summaryData.Modified;
                }
                else
                {
                    ++summaryData.NonModified;
                }  
            }
        }

        #region Nested type: SummaryData

        private struct SummaryData
        {
            public int Errors { get; set; }

            public int NonModified { get; set; }

            /// <summary>
            /// новых документов
            /// </summary>
            public int NewDocuments { get; set; }

            /// <summary>
            /// Новых организаций
            /// </summary>
            public int NewStructures { get; set; }

            public int All { get; set; }

            public int Modified { get; set; }

            public string Files { get; set; }

            public string Messages { get; private set; }

            /// <summary>
            /// Добавление сообщения в протокол закачки
            /// </summary>
            public void SetMessage(string msg)
            {
                Messages += "{0} \n".FormatWith(msg);
            }
        }

        #endregion

        private class OKVEDType : XTypedElement, IXMetaData
        {
            /// <summary>
            /// <para>
            /// Occurrence: required
            /// </para>
            /// </summary>
            public string KodOKVED
            {
                get
                {
                    XAttribute x = Attribute(XName.Get("KodOKVED", string.Empty));
                    return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
                }

                set
                {
                    SetAttribute(XName.Get("KodOKVED", string.Empty), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
                }
            }

            /// <summary>
            /// <para>
            /// Occurrence: required
            /// </para>
            /// </summary>
            public string Main
            {
                get
                {
                    XAttribute x = Attribute(XName.Get("Main", string.Empty));
                    return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NmToken).Datatype);
                }

                set
                {
                    SetAttribute(XName.Get("Main", string.Empty), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NmToken).Datatype);
                }
            }
            
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            XName IXMetaData.SchemaName
            {
                get
                {
                    return XName.Get("OKVED", string.Empty);
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            SchemaOrigin IXMetaData.TypeOrigin
            {
                get
                {
                    return SchemaOrigin.Fragment;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ILinqToXsdTypeManager IXMetaData.TypeManager
            {
                get
                {
                    return LinqToXsdTypeManager.Instance;
                }
            }

            public static explicit operator OKVEDType(XElement xe)
            {
                return XTypedServices.ToXTypedElement<OKVEDType>(xe, LinqToXsdTypeManager.Instance);
            }

            public override XTypedElement Clone()
            {
                return XTypedServices.CloneXTypedElement(this);
            }

            ContentModelEntity IXMetaData.GetContentModel()
            {
                return ContentModelEntity.Default;
            }
        }
    }
}
