using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers.FO47Dkz
{
    public class FO47DkzPumper : Pumper
    {
        private readonly IScheme scheme;
        private readonly IRepository<FX_BdgtLevels_SKIF> bdgtRepository;
        private readonly IRepository<FX_Date_YearDayUNV> dateRepository;
        private readonly ILinqRepository<DataSources> dataSourceRepository;
        private readonly ILinqRepository<D_KVSR_Analysis> kvsrRepository;
        private readonly ILinqRepository<D_FKR_Analysis> fkrRepository;
        private readonly ILinqRepository<D_Arrears_Debt> debtRepository;
        private readonly ILinqRepository<D_Arrears_Marks> markRepository;
        private readonly ILinqRepository<FX_Arrears_LiabilityType> liabilityRepository;
        private readonly ILinqRepository<FX_MeansType_SKIF> meansRepository;
        private readonly IReportSectionDataService dataService;
        private readonly ILinqRepository<F_Arrears_Liability> factRepository;

        public FO47DkzPumper(
            IScheme scheme,
            IRepository<FX_BdgtLevels_SKIF> bdgtRepository,
            IRepository<FX_Date_YearDayUNV> dateRepository,
            ILinqRepository<DataSources> dataSourceRepository,
            ILinqRepository<D_KVSR_Analysis> kvsrRepository,
            ILinqRepository<D_FKR_Analysis> fkrRepository,
            ILinqRepository<D_Arrears_Debt> debtRepository,
            ILinqRepository<D_Arrears_Marks> markRepository,
            ILinqRepository<FX_Arrears_LiabilityType> liabilityRepository,
            ILinqRepository<FX_MeansType_SKIF> meansRepository,
            IReportSectionDataService dataService,
            ILinqRepository<F_Arrears_Liability> factRepository)
        {
            this.scheme = scheme;
            this.bdgtRepository = bdgtRepository;
            this.dateRepository = dateRepository;
            this.kvsrRepository = kvsrRepository;
            this.fkrRepository = fkrRepository;
            this.debtRepository = debtRepository;
            this.markRepository = markRepository;
            this.liabilityRepository = liabilityRepository;
            this.meansRepository = meansRepository;
            this.dataService = dataService;
            this.factRepository = factRepository;
            this.dataSourceRepository = dataSourceRepository;
        }

        public override void Pump(D_CD_Report report, PamperActionsEnum actions)
        {
            var debtMap = new Dictionary<string, int>
            {
                { "AYdkz", 1 },
                { "KSdkz", 2 },
                { "Mdkz", 3 },
                { "BYchdkz", 5 },
                { "AYchdkz", 6 },
                { "KYchdkz", 7 },
                { "CPdkz", 8 },
                { "PRdkz", 9 }
            };

            var columns = new Dictionary<string, KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>>();

            var task = report.RefTask;
            var date = task.BeginDate;
            var year = date.Year;
            var dataSourceFo06 = dataSourceRepository.FindAll()
                .FirstOrDefault(x => x.SupplierCode == "ФО" && x.DataCode == 6 && x.Year == Convert.ToString(year));
            CheckCondition(dataSourceFo06 != null, "Не найден источник данных \"ФО - Анализ данных - {0}\".", year);

            var meansLookup = meansRepository.FindAll().Where(x => x.ID == 1 || x.ID == 2).ToList();
            var means1 = meansLookup.First(x => x.ID == 1);
            var means2 = meansLookup.First(x => x.ID == 2);
            var liabilityLookup = liabilityRepository.FindAll().ToList();
            
            columns.Add("DZBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 0), means1));
            columns.Add("DZoffBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 0), means2));
            columns.Add("KZBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 2), means1));
            columns.Add("KZoffBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 2), means2));
            columns.Add("OutDZBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 1), means1));
            columns.Add("OutDZoffBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 1), means2));
            columns.Add("OutKZBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 3), means1));
            columns.Add("OutKZoffBud", new KeyValuePair<FX_Arrears_LiabilityType, FX_MeansType_SKIF>(liabilityLookup.First(x => x.ID == 3), means2));

            // --- Получить/создать источник ФО_0047 --------------
            var ds = scheme.DataSourceManager.DataSources.CreateElement();
            ds.SupplierCode = "ФО";
            ds.DataCode = "47";
            ds.DataName = "Задолженность Дз Кз";
            ds.ParametersType = ParamKindTypes.Year;
            ds.Year = year;
            int dataSourceFo47Id = ds.FindInDatabase() ?? scheme.DataSourceManager.DataSources.Add(ds);

            // --- Константы --------------------------------------
            // Бюджет субъекта
            var refBdgtLevels = bdgtRepository.Get(3);

            // --- Параметры из отчета ----------------------------
            // Дата
            var dateId = (date.Year * 10000) + 9990 + date.Quarter();
            var refQuarter = dateRepository.Get(dateId);
            CheckCondition(refQuarter != null, "Не найдена дата {0} в классификаторе периодов.", dateId);

            // Администратор.Анализ
            var subjectName = task.RefSubject.Name;
            var refKvsr = kvsrRepository.FindAll()
                .FirstOrDefault(x => x.SourceID == dataSourceFo06.ID && x.Name == subjectName);
            CheckCondition(refQuarter != null, "Не найдена строка \"{0}\" в классификаторе \"Администратор.Анализ\" по источнику {1} года.", subjectName, dataSourceFo06.Year);

            // РзПр.Анализ (FKR)
            var fkrCode = Convert.ToInt32(report.RefForm.InternalName.Substring(8, 2)) * 100;
            var refFkr = fkrRepository.FindAll()
                .FirstOrDefault(x => x.SourceID == dataSourceFo06.ID && x.Code == fkrCode);
            CheckCondition(refQuarter != null, "Не найдена строка с кодом \"{0}\" в классификаторе \"РзПр.Анализ\" по источнику {1} года.", fkrCode, dataSourceFo06.Year);

            // Кешируем показатели
            var marks = markRepository.FindAll().ToList();
            CheckCondition(marks.Count > 0, "Классификатор \"Задолженость.Показатели\" пустой.", fkrCode, dataSourceFo06.Year);

            // --- Разделы ----------------------------------------
            foreach (var section in report.Sections)
            {
                if (section.RefFormSection.InternalName == "PZdkz")
                {
                    continue;
                }

                // Направления финансирования (RefDebt)
                int debtCode = debtMap[section.RefFormSection.InternalName];
                var refDebt = debtRepository.FindAll()
                    .FirstOrDefault(x => x.CodeNumber == debtCode);
                CheckCondition(refDebt != null, "Не найдена строка с кодом \"{0}\" в классификаторе \"Задолженость.Направления финансирования\".", debtCode);

                // Существуюие факты
                var facts = factRepository.FindAll()
                    .Where(x => x.SourceID == dataSourceFo47Id 
                        && x.RefBdgt == refBdgtLevels 
                        && x.RefLiabilityPeriod == refQuarter 
                        && x.RefLiabilityKVSR == refKvsr 
                        && x.RefFKR == refFkr 
                        && x.RefDebt == refDebt).ToList();

                if ((actions & PamperActionsEnum.Clear) == PamperActionsEnum.Clear)
                {
                    // Удаляем существующие факты
                    foreach (var fact in facts)
                    {
                        factRepository.Delete(fact);
                    }

                    facts.Clear();
                }

                if ((actions & PamperActionsEnum.Pump) == PamperActionsEnum.Pump)
                {
                    // Показатели (RefMarks)
                    var rows = dataService.GetAll(report, section.RefFormSection.Code);
                    foreach (var row in rows)
                    {
                        var codeRow = Convert.ToString(row.Get("CodeRow"));
                        if (String.IsNullOrEmpty(codeRow))
                        {
                            continue;
                        }

                        var refMark = marks.FirstOrDefault(x => x.CodeRow == codeRow);
                        CheckCondition(refMark != null, "Не найдена строка с кодом \"{0}\" в классификаторе \"Задолженость.Показатели\".", codeRow);

                        foreach (var column in columns)
                        {
                            var value = row.Get(column.Key);
                            if (value == null)
                            {
                                continue;
                            }

                            var decimalValue = Convert.ToDecimal(value);
                            if (decimalValue == 0)
                            {
                                continue;
                            }

                            // Вид задолжености (RefLiability)
                            var refLiab = column.Value.Key;

                            // Тип средств (RefMeans)
                            var refMeans = column.Value.Value;

                            // Пробуем найти существующий факт
                            var existFacts = facts.Where(x =>
                                                         x.RefLiabilityMarks == refMark &&
                                                         x.RefLiabilityType == refLiab &&
                                                         x.RefMeans == refMeans).ToList();
                            if (existFacts.Count() > 1)
                            {
                                // TODO: не кидать исключение, а записать данные и сказать про дубликаты пользователю
                                // а еще лучше слить дубликаты
                                throw new Exception("Дубликаты.");
                            }

                            if (existFacts.Count() == 1)
                            {
                                // Обновляем существующий факт
                                existFacts[0].Fact = decimalValue;
                                factRepository.Save(existFacts[0]);
                            }
                            else
                            {
                                // Создаем новый факт
                                var fact = new F_Arrears_Liability
                                {
                                    SourceKey = Convert.ToInt32(row.Get("ID")),
                                    SourceID = dataSourceFo47Id,
                                    TaskID = task.ID,
                                    RefBdgt = refBdgtLevels,
                                    RefLiabilityPeriod = refQuarter,
                                    RefLiabilityKVSR = refKvsr,
                                    RefFKR = refFkr,
                                    RefDebt = refDebt,
                                    RefLiabilityMarks = refMark,
                                    RefLiabilityType = refLiab,
                                    RefMeans = refMeans,
                                    Fact = decimalValue
                                };

                                factRepository.Save(fact);
                            }
                        }
                    }
                }
            }

            factRepository.DbContext.CommitChanges();
        }
    }
}
