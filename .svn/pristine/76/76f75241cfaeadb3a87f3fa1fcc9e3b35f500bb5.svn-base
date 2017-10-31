using System;
using System.Linq;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using NHibernate.Impl;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class VariantCopyService
    {
        private readonly RegionsAccordanceService regionsAccordanceService;
        private readonly NHibernateLinqRepository<D_S_RegionMatchOperation> repositoryRegionsMatch;
        private NHibernateLinqRepository<DataSources> repositoryDatasources;

        public VariantCopyService(RegionsAccordanceService regionsAccordanceService)
        {
            this.regionsAccordanceService = regionsAccordanceService;
            this.repositoryRegionsMatch = new NHibernateLinqRepository<D_S_RegionMatchOperation>();
            this.repositoryDatasources = new NHibernateLinqRepository<DataSources>();
        }

        /// <summary>
        /// Создает новый вариант и копирует в него все данные из указанного варианта.
        /// При этом увеличивает Отчетную_дату на 1 месяц
        /// В случае перехода на новый год - создает ссылки на новые Районы.Анализ нового года, создает копии "Должности для отчётов" с новыми районами если их нет
        /// </summary>
        /// <param name="variantId">Исходный вариант.</param>
        /// <param name="sourceId"></param>
        /// <param name="lockCopiedData">true - кипия данных будет заблокированна для редактирования (будет установлено значение поля IsBlocked в true).</param>
        public void Copy(int variantId, int sourceId, bool lockCopiedData)
        {
            try
            {
                var repository = new NHibernateRepository<D_Variant_Schuldbuch>();
                var variant = repository.Get(variantId);
                if (variant == null)
                {
                    throw new ApplicationException("Исходный вариант ID={0} не найден".FormatWith(variantId));
                }
                
                var newVariant = variant.Copy();

                if (variant.VariantCompleted == 0)
                {
                    variant.CurrentVariant = false;
                    repository.Save(variant);
                }
                else
                {
                    if (variant.CurrentVariant)
                        newVariant.CurrentVariant = false;
                }

                // Добавляем месяц у отчётной даты варианта
                newVariant.ReportDate = variant.ReportDate.AddMonths(1);
                
                // Отчетная дата января - это за декабрь. Копируем его - значит переходим на новый год
                if(variant.ReportDate.Month == 1)
                {
                    newVariant.ActualYear = variant.ActualYear + 1;
                    sourceId = GetSourceIdAnalysis(newVariant.ActualYear);
                }

                newVariant.VariantCompleted = 0;
                newVariant.Name = "Копия: {0}".FormatWith(variant.Name);
                newVariant.VariantComment = 
                    "Копия варианта: ID: {0} Код: {1} Наименование: \"{2}\" Текущий год: {3} Отчетная дата: {4}"
                    .FormatWith(variant.ID, variant.Code, variant.Name, variant.ActualYear, variant.ReportDate);
                repository.Save(newVariant);

                CopyEntityData(newVariant, new Specification<F_S_SchBLimit>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_SchBCapital>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_SchBCreditincome>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_SchBCreditincomePos>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_SchBGuarantissued>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_SchBGuarantissuedPos>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_ServiceDebt>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_StructureDebt>(variant.ID), lockCopiedData);
                CopyEntityData(newVariant, new Specification<F_S_ContrDebt>(variant.ID), lockCopiedData);

                if (newVariant.ActualYear > variant.ActualYear)
                {
                    MergeTitleRecordRegionToNewYear(variant.ActualYear, newVariant.ActualYear);
                }

                InitializeProtocol(newVariant, sourceId);
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка копирования варианта: {0}", e.ExpandException());
                throw new ApplicationException("Ошибка копирования варианта: {0}".FormatWith(e.Message), e);
            }
        }

        /// <summary>
        /// Копирует все данные варианта.
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="sourceId"></param>
        /// <param name="lockCopiedData">true - кипия данных будет заблокированна для редактирования (будет установлено значение поля IsBlocked в true).</param>
        public void CopyAllData(int variantId, int sourceId, bool lockCopiedData)
        {
            CopyVariantData(variantId, sourceId, -1, false, lockCopiedData);
        }

        /// <summary>
        /// Копирует все данные указанного варианта заданного субъекта (района). 
        /// Не копирует данные подчиненных районов.
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="sourceId"></param>
        /// <param name="regionId">Если указано значение -1, то будут копироваться все данные варианта.</param>
        /// <param name="lockCopiedData">true - кипия данных будет заблокированна для редактирования (будет установлено значение поля IsBlocked в true).</param>
        public void CopySubjectData(int variantId, int sourceId, int regionId, bool lockCopiedData)
        {
            CopyVariantData(variantId, sourceId, regionId, false, lockCopiedData);
        }

        /// <summary>
        /// Копирует все данные указанного варианта заданного района и подчиненных ему районов (поселений).
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="sourceId"></param>
        /// <param name="regionId">Если указано значение -1, то будут копироваться все данные варианта.</param>
        /// <param name="copyChildsRegions">true - копировать подчиненные районы второго уровня.</param>
        /// <param name="lockCopiedData">true - кипия данных будет заблокированна для редактирования (будет установлено значение поля IsBlocked в true).</param>
        public void CopyVariantData(int variantId, int sourceId, int regionId, bool copyChildsRegions, bool lockCopiedData)
        {
            var variant = new NHibernateRepository<D_Variant_Schuldbuch>().Get(variantId);
            
            if (variant == null)
                throw new ApplicationException("Целевой вариант ID={0} не найден".FormatWith(variantId));

            var sourceVariant = GetPrevDebtBookVariant(variant.ReportDate);

            if (sourceVariant == null)
                throw new ApplicationException("Вариант, с которого можно копировать данные, не найден");

            int sourceRegionId = regionId;
            if (sourceVariant.ActualYear != variant.ActualYear)
                sourceRegionId = regionsAccordanceService.GetRegionsByYear(
                    sourceVariant.ActualYear, regionId, variant.ActualYear)[0];

            CopyEntityData(variant, new Specification<F_S_SchBLimit>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_SchBCapital>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_SchBCreditincome>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_SchBCreditincomePos>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_SchBGuarantissued>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_SchBGuarantissuedPos>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_ServiceDebt>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_StructureDebt>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);
            CopyEntityData(variant, new Specification<F_S_ContrDebt>(sourceVariant.ID, sourceRegionId, copyChildsRegions), lockCopiedData);

            InitializeProtocol(variant, sourceId);
        }

        /// <summary>
        /// Возвращает предыдущей вариант с наибольшей отчетной датой,
        /// которая меньше отчетной даты текущего варианта.
        /// </summary>
        /// <param name="currentVariantReportDate">Отчетная датя текущего варианта.</param>
        private static D_Variant_Schuldbuch GetPrevDebtBookVariant(DateTime currentVariantReportDate)
        {
            // Правильнее делать так:
            //return new NHibernateLinqRepository<D_Variant_Schuldbuch>().FindAll()
            //    .OrderByDescending(x => x.ReportDate)
            //    .FirstOrDefault(x => x.ReportDate < currentVariantReportDate);
            // но пока NHibernate.Linq 3.0 не поддерживает такое выражение придется
            // делать следующим образом:
            return new NHibernateLinqRepository<D_Variant_Schuldbuch>().FindAll()
                .Where(x => x.ReportDate < currentVariantReportDate).ToList()
                .OrderByDescending(x => x.ReportDate)
                .FirstOrDefault();
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toVariant"></param>
        /// <param name="specification"></param>
        /// <param name="lockCopiedData">true - копия данных будет заблокированна для редактирования (будет установлено значение поля IsBlocked в true).</param>
        private void CopyEntityData<T>(D_Variant_Schuldbuch toVariant, Specification<T> specification, bool lockCopiedData) 
            where T : DebtorBookFactBase, new()
        {
            var repository = new NHibernateLinqRepository<T>();
            var queryableChilds = new NHibernateLinqRepository<T>().FindAll();
            var queryable = repository.FindAll(specification);
            var newSourceId = GetSourceIdDebtBook(toVariant.ActualYear);
            foreach (var entity in queryable.Where(x => x.ParentID == null))
            {
                // Т.к. в настоящее время копирование варианта == создание варианта, то данная проверка лишняя
                ////// Удаление ранее скопированной записи
                ////var rootRecords = repository.FindAll().Where(f => f.SourceKey == entity.ID && f.RefVariant == toVariant);
                ////foreach (var record in rootRecords)
                ////{
                ////    repository.Delete(record);
                ////}

                var newEntity = entity.Copy();
                newEntity.RefVariant = toVariant;
                newEntity.IsBlocked = lockCopiedData;
                newEntity.SourceKey = entity.ID;
                if (newEntity.RefVariant.ActualYear != entity.RefVariant.ActualYear)
                {
                    newEntity.RefRegion = GetNextMatchRegion(entity.RefRegion);
                    newEntity.SourceID = newSourceId;
                }

                repository.Save(newEntity);

                var current = entity;
                foreach (var child in queryableChilds.Where(x => x.ParentID == current.ID))
                {
                    // Т.к. в настоящее время копирование варианта == создание варианта, то данная проверка лишняя
                    //// Удаление ранее скопированной записи
                    ////var childRecords = repository.FindAll().Where(f => f.SourceKey == current.ID && f.RefVariant == toVariant);
                    ////foreach (var record in childRecords)
                    ////{
                    ////    repository.Delete(record);
                    ////}

                    var newChild = child.Copy();
                    newChild.RefVariant = toVariant;
                    newChild.IsBlocked = lockCopiedData;
                    newChild.ParentID = newEntity.ID;
                    newChild.SourceKey = child.ID;
                    if (newChild.RefVariant.ActualYear != child.RefVariant.ActualYear)
                    {
                        newChild.RefRegion = GetNextMatchRegion(child.RefRegion);
                        newChild.SourceID = newSourceId;
                    }

                    repository.Save(newChild);
                }
            }
        }

        public void InitializeProtocol(D_Variant_Schuldbuch variant, int sourceId)
        {
            var repository = new NHibernateLinqRepository<T_S_ProtocolTransfer>();
            var statusSchbRepository = new NHibernateLinqRepository<FX_S_StatusSchb>();

            var minusStatement = "minus";
            if (((SessionFactoryImpl) NHibernateSession.Current.SessionFactory)
                .ConnectionProvider.Driver is NHibernate.Driver.SqlClientDriver)
            {
                minusStatement = "except";
            }

            // NHibernate.Linq пока не поддержикает директиву minus, 
            // так что временно используем нативный SQL
            var query = NHibernateSession.Current.CreateSQLQuery(@"
(select *
  from D_REGIONS_ANALYSIS 
 where SourceID = :sourceId
   and RowType = 0
   and (RefTerr = 4 or RefTerr = 7))
{0}
(select r.*
  from T_S_PROTOCOLTRANSFER t, D_REGIONS_ANALYSIS r
 where t.RefRegion = r.ID
   and t.RefVariant = :variant
   and r.SourceID = :sourceId
   and r.RowType = 0
   and (r.RefTerr = 4 or r.RefTerr = 7))
".FormatWith(minusStatement));
            query.SetInt32("sourceId", sourceId);
            query.SetInt32("variant", variant.ID);

            foreach (D_Regions_Analysis region in query.AddEntity(typeof(D_Regions_Analysis)).List())
            {
                var protocol = new T_S_ProtocolTransfer();
                protocol.RefRegion = region;
                protocol.RefVariant = variant;
                protocol.RefStatusSchb = statusSchbRepository.FindOne(1);
                repository.Save(protocol);
            }
            repository.DbContext.CommitChanges();
        }

        /// <summary>
        /// Копирует записи в "Должности для отчетов" изменяя ссылку RefTerritory на соответствующую запись в разрезе источника нового года
        /// Дубликаты не создаем
        /// </summary>
        private void MergeTitleRecordRegionToNewYear(int prevYear, int newYear)
        {
            if (prevYear == newYear)
            {
                return;
            }
            
            if (prevYear + 1 != newYear)
            {
                throw new Exception("Реализовано только для перехода на один год");
            }

            // Источники для территорий по годам
            var oldSource = GetSourceIdAnalysis(prevYear);
            var newSource = GetSourceIdAnalysis(newYear);

            var repositoryTitleRecodr = new NHibernateLinqRepository<D_S_TitleReport>();

            var candidates = (from s in repositoryTitleRecodr.FindAll()
                             where s.RefRegion.SourceID == oldSource
                            select s).ToList();
            var existingList = (from s in repositoryTitleRecodr.FindAll()
                               where s.RefRegion.SourceID == newSource
                              select s).ToList();

            foreach (var row in candidates)
            {
                var newRow = row.Copy();

                newRow.RefRegion = GetNextMatchRegion(row.RefRegion); 
                if (!existingList.Exists(f => f.RefRegion == newRow.RefRegion && f.TitleManager == newRow.TitleManager))
                {
                    repositoryTitleRecodr.Save(newRow);
                }
            }
        }

        /// <summary>
        /// Получение района по таблице соответствий, соответсвующего данному району.
        /// </summary>
        private D_Regions_Analysis GetNextMatchRegion(D_Regions_Analysis sourceRegion)
        {
            var result = (from t in this.repositoryRegionsMatch.FindAll()
                         where t.RefRegionOld == sourceRegion
                         select t).ToList();
            
            if (result.Count > 1)
            {
                throw new Exception("У района найдено несколько соответствий в новом источнике. Id={0}, Name={1}".FormatWith(sourceRegion.ID, sourceRegion.Name));
            }
            
            if (result.Count == 0)
            {
                throw new Exception("У района нет соответствия в новом источнике. Id={0}, Name={1}".FormatWith(sourceRegion.ID, sourceRegion.Name));
            }

            var accordance = result.First();
            
            if (accordance.RefRegionNew == null)
            {
                throw new Exception("У района отсутствует сопоставление. Район = {0}, SourceId = {1}".FormatWith(accordance.RefRegionOld.Name, accordance.SourceID));
            }

            return accordance.RefRegionNew;
        }

        private int GetSourceIdDebtBook(int year)
        {
            const string supplier = "ФО";
            const int dataCode = 30;
            
            var sources = (from s in this.repositoryDatasources.FindAll()
                           where s.SupplierCode == supplier
                                 && s.DataCode == dataCode
                                 && s.Year == year.ToString()
                                 && s.Deleted == "0"
                           select s).ToList();
            if (sources.Count > 1)
            {
                throw new Exception("Найдено несколько источников для года {0} и кода {1}".FormatWith(year, dataCode));
            }

            if (sources.Count == 0)
            {
                throw new Exception("Не найден источник для года {0} и кода {1}".FormatWith(year, dataCode));
            }

            return sources.First().ID;
        }

        /// <summary>
        /// Источник данных "ФО_Анализ данных" соответствующий текущему году варианта.
        /// </summary>
        private int GetSourceIdAnalysis(int year)
        {
            const string supplier = "ФО";
            const int dataCode = 6;

            var sources = (from s in this.repositoryDatasources.FindAll()
                           where s.SupplierCode == supplier
                                 && s.DataCode == dataCode
                                 && s.Year == year.ToString()
                                 && s.Deleted == "0"
                           select s).ToList();
            if (sources.Count > 1)
            {
                throw new Exception("Найдено несколько источников для года {0} и кода {1}".FormatWith(year, dataCode));
            }

            if (sources.Count == 0)
            {
                throw new Exception("Не найден источник для года {0} и кода {1}".FormatWith(year));
            }

            return sources.First().ID;
        }

        private class Specification<T> : ILinqSpecification<T> where T : DebtorBookFactBase
        {
            private readonly int sourceVariantId;
            private readonly int regionId;
            private readonly bool copyChildsRegions;

            /// <summary>
            /// Все данные варианта.
            /// </summary>
            /// <param name="sourceVariantId"></param>
            public Specification(int sourceVariantId)
            {
                this.sourceVariantId = sourceVariantId;
                regionId = -1;
            }

            /// <summary>
            /// Все данные указанного варианта заданного района и подчиненных ему районов (поселений).
            /// </summary>
            /// <param name="sourceVariantId">Исходный вариант.</param>
            /// <param name="regionId">Если указано значение -1, то будут копироваться все данные варианта.</param>
            /// <param name="copyChildsRegions">true - копировать подчиненные районы второго уровня.</param>
            public Specification(int sourceVariantId, int regionId, bool copyChildsRegions)
            {
                this.sourceVariantId = sourceVariantId;
                this.regionId = regionId;
                this.copyChildsRegions = copyChildsRegions;
            }

            public IQueryable<T> SatisfyingElementsFrom(IQueryable<T> candidates)
            {
                if (regionId != -1 && copyChildsRegions)
                {
                    var regions = new NHibernateLinqRepository<D_Regions_Analysis>().FindAll();

                    // Все данные указанного варианта заданного района и подчиненных ему районов (поселений)
                    return from f in candidates
                           where f.RefVariant.ID == sourceVariantId && (f.RefRegion.ID == regionId ||
                                regions.Any(regLevel2 => f.RefRegion.ID == regLevel2.ID &&
                                    regions.Any(regLevel1 =>
                                        regLevel1.ID == regLevel2.ParentID && regLevel1.ParentID == regionId)))
                           select f;
                }

                if (regionId != -1)
                {
                    // Все данные указанного варианта заданного субъекта
                    return from c in candidates
                           where c.RefVariant.ID == sourceVariantId && c.RefRegion.ID == regionId
                           select c;
                }

                // Все данные указанного варианта
                return from c in candidates
                       where c.RefVariant.ID == sourceVariantId
                       select c;
            }
        }
    }
}
