using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;

using bus.gov.ru;
using bus.gov.ru.Imports;
using bus.gov.ru.types.Item1;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

using Root = bus.gov.ru.external.Item1.XRoot;

namespace Bus.Gov.Ru.Imports
{
    public class CommonPump
    {
        public readonly ILinqRepository<F_F_ParameterDoc> HeaderRepository;

        public readonly ILinqRepository<D_Doc_TypeDoc> DocumentTypesCache;

        public readonly ILinqRepository<FX_Org_SostD> StateDocCache;

        public readonly ILinqRepository<FX_FX_PartDoc> TypeDocCache;

        public readonly ILinqRepository<FX_Fin_YearForm> YearFormCache;

        public readonly Dictionary<string, D_Org_Structure> OrgStructuresByRegnumCache;

        private static CommonPump commonPump;

        private CommonPump()
        {
            HeaderRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();

            TypeDocCache = Resolver.Get<ILinqRepository<FX_FX_PartDoc>>();
            StateDocCache = Resolver.Get<ILinqRepository<FX_Org_SostD>>();
            YearFormCache = Resolver.Get<ILinqRepository<FX_Fin_YearForm>>();

            DocumentTypesCache = Resolver.Get<ILinqRepository<D_Doc_TypeDoc>>();

            OrgStructuresByRegnumCache = GetOGSDictionary();
        }

        public static CommonPump GetCommonPump
        {
            get
            {
                return commonPump ?? (commonPump = new CommonPump());
            }
        }

        public IDataPumpProtocol DataPumpProtocol { get; set; }

        /// <summary>
        ///  получение действующих(801) ОГСов(регнамов)
        /// </summary>
        /// <returns> справочник действующих ОГС ключ - regNum, значение - D_Org_Structure</returns>
        public static Dictionary<string, D_Org_Structure> GetOGSDictionary()
        {
            try
            {
                return Resolver.Get<ILinqRepository<D_Org_Structure>>().FindAll().ToList()
                    .Join(
                    Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll().Where(x => x.Stats.Equals(D_Org_NsiOGS.Included)).ToList(),
                    structure => new { inn = structure.INN, kpp = structure.KPP },
                    ogs => new { ogs.inn, ogs.kpp },
                    (structure, ogs) => new { ogs.regNum, structure })
                .ToDictionary(arg => arg.regNum, arg => arg.structure);
            }
            catch (Exception e)
            {
                throw new Exception(string.Concat("Дубли в организациях или ОГС по ИНН и КПП.", e.Message));
            }
        }

        public static int PumpFile(StreamReader file, IDataPumpProtocol dataPumpProtocolProvider, string fileName = "")
        {
            GetCommonPump.DataPumpProtocol = dataPumpProtocolProvider;
            
            try
            {
                var pumpData = Root.Load(file);

                if (pumpData.financialActivityPlan != null)
                {
                    new FinancialActivityPlanPump().PumpFile(pumpData.financialActivityPlan.body.position);    
                    return -1;
                }

                if (pumpData.budgetaryCircumstances != null)
                {
                    new BudgetaryCircumstancesPump().PumpFile(pumpData.budgetaryCircumstances.body.position);
                    return -1;
                }

                if (pumpData.actionGrant != null)
                {
                    new ActionGrantPump().PumpFile(pumpData.actionGrant.body.position);
                    return -1;
                }

                if (pumpData.stateTask != null)
                {
                    return new StateTaskPump().PumpFile(pumpData.stateTask.body.position);
                }

                if (pumpData.stateTask640r != null)
                {
                    return new StateTask2016Pump().PumpFile(pumpData.stateTask640r.body.position);
                }

                if (pumpData.nsiOkei != null)
                {
                    return new NsiOkeiPump().PumpFile(pumpData.nsiOkei.body);
                }
            }
            catch (Exception e)
            {
                GetCommonPump.DataPumpProtocol
                    .WriteProtocolEvent(DataPumpEventKind.dpeError, "Закачка файла {0} завершена с ошибками: {1}".FormatWith(fileName, e.ExpandException()));
            }

            return -1;
        }

        /// <summary>
        /// Возвращает актальный ОКВЕД по датам докумнта и коду ОКВЕД(версионность ОКВЭД)
        /// </summary>
        /// <param name="doc">документ, отсюда берется дата открытия и закрытия документа</param>
        /// <param name="codeOkved">код ОКВЭД</param>
        /// <returns>ОКВЭД из справочника</returns>
        public static D_OKVED_OKVED GetActualOKVED(F_F_ParameterDoc doc, string codeOkved)
        {
            var data = Resolver.Get<ILinqRepository<D_OKVED_OKVED>>().FindAll().Where(x => x.Code.Equals(codeOkved)).ToList();
            return data.FindAll(x => x.IsEntryDates(doc.OpeningDate, doc.CloseDate, "OpenDate", "CloseDate")).OrderByDescending(x => x.ID).FirstOrDefault();
        }

        // Проверка предыдущих документов
        public void CheckDocs(int uchr, int partDoc, int year, IEnumerable<int> state)
        {
            if (HeaderRepository.FindAll().Any(x => x.RefUchr.ID == uchr
                                                && x.RefPartDoc.ID == partDoc
                                                && x.RefYearForm.ID == year
                                                && !state.Contains(x.RefSost.ID)))
            {
                throw new Exception("Импорт невозможен. Предыдущий документ должен быть в состоянии «Экспортировано»");
            }    
        }

        // Проверка учреждения
        public bool CheckInstTarget(refNsiConsRegExtendedStrongType targetOrg)
        {
            if (!OrgStructuresByRegnumCache.ContainsKey(targetOrg.regNum))
            {
                /*WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeError,
                    string.Format(
                        "Учреждение regnum={0}{1} не найдено, требуется обновление nsiOgs",
                        targetOrg.regNum,
                        targetOrg.fullName.Return(
                            s => string.Format("({0})", s),
                            string.Empty)));*/
                return false;
            }

            return true;
        }

        public void ProcessDocumentsHeader(
                                           F_F_ParameterDoc header,
                                           IEnumerable<documentType> pumpDataDocument,
                                           Func<documentType, D_Doc_TypeDoc> getTypeDocFunc)
        {
            List<string> documents = header.Unless(doc => doc.ID == 0)
                                            .With(doc => doc.Documents.Select(docum => docum.UrlExternal).ToList());

            const int LengthFDocDocumFieldName = 128;

            pumpDataDocument.Each(
                type => header.Unless(doc => documents.Return(list => list.Contains(type.url), false))
                              .With(doc => new F_Doc_Docum
                              {
                                  UrlExternal = type.url,
                                  RefParametr = header
                              })
                              .Do(docum =>
                              {
                                  var name = type.name;
                                  docum.Name = name.Substring(0, Math.Min(name.Length, LengthFDocDocumFieldName));
                                  /*if (name.Length > LengthFDocDocumFieldName)
                                  {
                                      WriteEventIntoDataPumpProtocol(
                                          DataPumpEventKind.dpeWarning,
                                          string.Format(
                                              "Наименование документа '{0}' было обрезано",
                                              name));
                                  }*/
                              })
                            .Do(
                                docum =>
                                {
                                    if (type.date > SqlDateTime.MinValue.Value)
                                    {
                                        docum.DocDate = type.date;
                                    }

                                    /*else
                                    {
                                        WriteEventIntoDataPumpProtocol(
                                            DataPumpEventKind.dpeWarning,
                                            string.Format(
                                                "Дата документа '{1}' пропущена как ошибочная(значение {0} ранее начала Грегорианского календаря {2})",
                                                type.date.ToString(CultureInfo.InvariantCulture),
                                                type.name,
                                                SqlDateTime.MinValue.Value.ToString(CultureInfo.InvariantCulture)));
                                    }*/
                                })
                            .Do(docum => docum.RefTypeDoc = getTypeDocFunc(type))
                            .Do(header.Documents.Add));
        }
    }
}
