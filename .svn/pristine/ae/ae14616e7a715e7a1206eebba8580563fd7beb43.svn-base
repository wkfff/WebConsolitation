using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using bus.gov.ru.types.Item1;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        private List<D_Doc_TypeDoc> _documentTypesCache;
        private List<FX_Org_SostD> _stateDocCache;
        private bool _toPumpNewDocument;
        private List<FX_FX_PartDoc> _typeDocCache;
        private List<FX_Fin_YearForm> _yearFormCache;

        private bool CheckDocumentTarget(refNsiConsRegExtendedStrongType targetOrg)
        {
            if (!_orgStructuresByRegnumCache.ContainsKey(targetOrg.regNum))
            {
                var msg = string.Format(
                    "Учреждение regnum={0}{1} не найдено, требуется обновление nsiOgs", targetOrg.regNum, targetOrg.fullName.Return(s => string.Format("({0})", s), string.Empty));
                
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, msg);
                return false;
            }

            return true;
        }

        private void ProcessDocumentsHeader(F_F_ParameterDoc header, IEnumerable<documentType> pumpDataDocument, Func<documentType, D_Doc_TypeDoc> getTypeDocFunc)
        {
            List<string> documents =
                header.Unless(doc => doc.ID == 0).With(doc => doc.Documents.Select(docum => docum.UrlExternal).ToList());

            const int LengthFDocDocumFieldName = 128;
            pumpDataDocument.Each(
                type => header
                            .Unless(doc => documents.Return(list => list.Contains(type.url), false))
                            .With(
                                doc => new F_Doc_Docum
                                           {
                                               UrlExternal = type.url,
                                               RefParametr = header,
                                           })
                            .Do(
                                docum =>
                                    {
                                        var name = type.name;
                                        docum.Name = name.Substring(0, Math.Min(name.Length, LengthFDocDocumFieldName));
                                        if (name.Length > LengthFDocDocumFieldName)
                                        {
                                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format("Наименование документа '{0}' было обрезано", name));
                                        }
                                    })
                            .Do(
                                docum =>
                                    {
                                        if (type.date > SqlDateTime.MinValue.Value)
                                        {
                                            docum.DocDate = type.date;
                                        }
                                        else
                                        {
                                            var msg = string.Format(
                                                "Дата документа '{1}' пропущена как ошибочная(значение {0} ранее начала Грегорианского календаря {2})",
                                                type.date.ToString(CultureInfo.InvariantCulture),
                                                type.name,
                                                SqlDateTime.MinValue.Value.ToString(CultureInfo.InvariantCulture));
                                            
                                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, msg);
                                        }
                                    })
                            .Do(docum => docum.RefTypeDoc = getTypeDocFunc(type))
                            .Do(header.Documents.Add));
        }

        private void BeforeDocumentsCommonPumpFiles()
        {
            _typeDocCache = Resolver.Get<ILinqRepository<FX_FX_PartDoc>>().FindAll().ToList();
            _stateDocCache = Resolver.Get<ILinqRepository<FX_Org_SostD>>().FindAll().ToList();
            _yearFormCache = Resolver.Get<ILinqRepository<FX_Fin_YearForm>>().FindAll().ToList();

            _documentTypesCache = Resolver.Get<ILinqRepository<D_Doc_TypeDoc>>().FindAll().ToList();

            _orgStructuresByRegnumCache = BuildOrgStructureByRegnumCache();

            // читаем свойства закачек
            _toPumpNewDocument =
                Convert.ToBoolean(
                    GetParamValueByName(
                        PumpRegistryElement.ProgramConfig,
                        "ucbDocument.CreateNewWithExportedStatus",
                        "True"));
        }

        private void PumpDataDocument(DirectoryInfo directoryInfo)
        {
            // Каталог по с документами территории по коду КЛАДР
            var territiryRoot = new DirectoryInfo(_rootDir);
            if (!territiryRoot.Exists)
            {
                throw new PumpDataFailedException(
                    string.Format(
                        "В каталоге {0} не найдено ни одного источника.",
                        territiryRoot.FullName));
            }
                
            PumpDataSource(territiryRoot);
        }

        private void SetDocumentSourceId(F_F_ParameterDoc header)
        {
            DataSource.Do(source => header.SourceID = source.ID);
        }

        private void DeleteDocumentsFromSource(int sourceID)
        {
            var headerRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();
            var docs = headerRepository.FindAll().Where(doc => doc.SourceID == sourceID).ToList();
            var servicesCache = new List<D_Services_VedPer>();

            WriteEventIntoDeleteDataProtocol(
                DeleteDataEventKind.ddeInformation,
                string.Format("Удаление документов {0} источника {1}", _pumpIdentifier, sourceID));
            SetProgress(
                docs.Count,
                -1,
                string.Format("Удаление документов {0} источника {1}", _pumpIdentifier, sourceID),
                string.Empty);

            headerRepository.DbContext.BeginTransaction();
            for (int i = 0, count = docs.Count; i < count; i++)
            {
                SetProgress(-1, i, string.Empty, string.Empty);
                var doc = docs[i];
                switch (_pumpIdentifier)
                {
                    case "stateTask":
                        doc.StateTasks
                            .Select(zadanie => zadanie.RefVedPch)
                            .Where(per => per.Code.Contains(".86n.krista.ru"))
                            .Each(servicesCache.Add);
                        break;
                }

                headerRepository.Delete(doc);
            }

            headerRepository.DbContext.CommitChanges();
            headerRepository.DbContext.CommitTransaction();

            if (servicesCache.Any())
            {
                headerRepository.DbContext.BeginTransaction();
                servicesCache.Each(Resolver.Get<ILinqRepository<D_Services_VedPer>>().Delete);
                headerRepository.DbContext.CommitChanges();
                headerRepository.DbContext.CommitTransaction();
            }
        }
    }
}
