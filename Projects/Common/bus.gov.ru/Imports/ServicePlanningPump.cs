using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

using planning;

namespace bus.gov.ru.Imports
{
    /// <summary>
    /// Закачка Ведомственного перечня услуг из Планирования
    /// </summary>
    public class ServicePlanningPump
    {
        private const string FieldDefinitionError = "Не удалось определить '{0}': {1}";
        private const string FieldPumpError = "Не удалось закачать '{0}': {1}";

        // код региона
        private string region;

        // Репозитории для Услуги
        private ILinqRepository<D_Services_Service> serviceRepository;
        private ILinqRepository<FX_FX_ServiceType> serviceTypeRepository;
        private ILinqRepository<D_Services_ActivityType> activityTypeRepository;
        private ILinqRepository<D_OKTMO_OKTMO> oktmoRepository;
        private ILinqRepository<FX_FX_ServicePayType2> payTypeRepository;
        private ILinqRepository<D_Org_Structure> orgStructureRepository;

        // Репозитории для детализаций Услуги
        private ILinqRepository<F_F_ServiceInstitutionType> institutionTypeRepository;
        private ILinqRepository<F_F_ServiceLegalAct> legalActRepository;
        private ILinqRepository<F_F_ServiceOKPD> okpdRepository;
        private ILinqRepository<F_F_ServiceOKVED> okvedRepository;
        private ILinqRepository<F_F_ServiceConsumersCategory> consumersCategoryRepository;
        private ILinqRepository<F_F_ServiceInstitutionsInfo> institutionsInfoRepository;
        private ILinqRepository<F_F_ServiceIndicators> indicatorsRepository;
        private ILinqRepository<FX_FX_CharacteristicType> charateristicTypeRepository;

        // Справочники
        private ILinqRepository<D_Org_OKEI> commonOkeiTypeRepository;
        private ILinqRepository<D_OKVED_OKVED> commonOkvedRepository;
        
        // Кэш для быстрых проверок на дубли услуг
        private List<ServiceCache> serviceCache;
        
        public ServicePlanningPump()
        {
            InitPump();
        }

        public ServicePlanningPump(string region)
        {
            this.region = region;
            InitPump();
        }

        private IDataPumpProtocol DataPumpProtocol { get; set; }

        private string FileName { get; set; }

        public void InitPump()
        {
            if (region == null)
            {
                region = Resolver.Get<IScheme>().GlobalConstsManager.Consts["KLADR"].Value.ToString().Substring(0, 2);
            }
            
            serviceRepository = Resolver.Get<ILinqRepository<D_Services_Service>>();
            serviceTypeRepository = Resolver.Get<ILinqRepository<FX_FX_ServiceType>>();
            activityTypeRepository = Resolver.Get<ILinqRepository<D_Services_ActivityType>>();
            oktmoRepository = Resolver.Get<ILinqRepository<D_OKTMO_OKTMO>>();
            payTypeRepository = Resolver.Get<ILinqRepository<FX_FX_ServicePayType2>>();
            orgStructureRepository = Resolver.Get<ILinqRepository<D_Org_Structure>>();

            institutionTypeRepository = Resolver.Get<ILinqRepository<F_F_ServiceInstitutionType>>();
            legalActRepository = Resolver.Get<ILinqRepository<F_F_ServiceLegalAct>>();
            okpdRepository = Resolver.Get<ILinqRepository<F_F_ServiceOKPD>>();
            okvedRepository = Resolver.Get<ILinqRepository<F_F_ServiceOKVED>>();
            consumersCategoryRepository = Resolver.Get<ILinqRepository<F_F_ServiceConsumersCategory>>();
            institutionsInfoRepository = Resolver.Get<ILinqRepository<F_F_ServiceInstitutionsInfo>>();
            indicatorsRepository = Resolver.Get<ILinqRepository<F_F_ServiceIndicators>>();
            charateristicTypeRepository = Resolver.Get<ILinqRepository<FX_FX_CharacteristicType>>();

            commonOkeiTypeRepository = Resolver.Get<ILinqRepository<D_Org_OKEI>>();
            commonOkvedRepository = Resolver.Get<ILinqRepository<D_OKVED_OKVED>>();

            serviceCache = serviceRepository.FindAll()
               .Where(x => x.IsEditable == true)
               .Select(
                   p => new ServiceCache
                   {
                       Regrnumber = p.Regrnumber,
                       RefTypeName = p.RefType.Name,
                       NameName = p.NameName,
                       RefYchrInn = p.RefYchr.INN,
                       SvcCntsName1Val = p.SvcCntsName1Val,
                       SvcCntsName2Val = p.SvcCntsName2Val,
                       SvcCntsName3Val = p.SvcCntsName3Val,
                       SvcTermsName1Val = p.SvcTermsName1Val,
                       RefActivityTypeCode = p.RefActivityType.Code,
                       FromPlaning = p.FromPlaning.GetValueOrDefault()
                   })
               .ToList();
        }
        
        public void Pump(StreamReader file, IDataPumpProtocol dataPumpProtocolProvider, string fileName = "")
        {
            DataPumpProtocol = dataPumpProtocolProvider;
            FileName = fileName;
            DataPumpProtocol.WriteProtocolEvent(
                DataPumpEventKind.dpeInformation,
                "Закачка ведомственного перечня услуг из Планирования; Файл {0}: ".FormatWith(FileName));

            var заголовкиГруппРеестровыхЗаписей = Документы.Load(file).Коллекция.Объект;
            
            foreach (var заголовок in заголовкиГруппРеестровыхЗаписей)
            {
                ProcessPump(заголовок);
            }
        }

        private static string ConvertToActivityTypeCode(string кодВидДеятельности)
        {
            try
            {
                return кодВидДеятельности.Length == 1
                    ? "0" + кодВидДеятельности
                    : кодВидДеятельности;
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Не удалось определить код вида деятельности: " + e.Message);
            }
        }

        private static string GetInstCode(Объект реестроваяЗапись)
        {
            try
            {
                return реестроваяЗапись.Код_РеестрЗапись.Substring(0, 19);
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Код учредителя", e.Message));
            }
        }

        private static string DashesDate(string date)
        {
            try
            {
                return date == null ? null : date.Insert(4, "-").Insert(7, "-");
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Неверный формат даты '{0}': {1}".FormatWith(date, e.Message));
            }
        }

        private void ProcessPump(Объект заголовок)
        {
            try
            {
                if (!IsNeedToPump(заголовок))
                {
                    return;
                }

                foreach (var реестроваяЗапись in заголовок.РеестровыеЗаписи.Объект)
                {
                    try
                    {
                        serviceRepository.DbContext.BeginTransaction();

                        if (IsDublicate(заголовок, реестроваяЗапись) || !CheckActuality(реестроваяЗапись))
                        {
                            serviceRepository.DbContext.RollbackTransaction();
                            continue;
                        }

                        PumpAll(реестроваяЗапись, заголовок);

                        serviceRepository.DbContext.CommitChanges();
                        serviceRepository.DbContext.CommitTransaction();

                        DataPumpProtocol.WriteProtocolEvent(
                          DataPumpEventKind.dpeInformation,
                          "Услуга добавлена: {0}".FormatWith(реестроваяЗапись.Код_РеестрЗапись));
                    }
                    catch (Exception ex)
                    {
                        serviceRepository.DbContext.RollbackTransaction();
                        DataPumpProtocol.WriteProtocolEvent(
                            DataPumpEventKind.dpeError,
                            "Ошибка при закачке услуги: {0}; {1}".FormatWith(реестроваяЗапись.Код_РеестрЗапись, ex.Message));
                    }
                }
            }
            catch (Exception e)
            {
                DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeError,
                    "Закачка завершена с ошибкой: {0}".FormatWith(e.Message));
            }
        }

        private bool IsNeedToPump(Объект заголовок)
        {
            return заголовок.ИНН.StartsWith(region);
        }

        private bool IsDublicate(Объект заголовок, Объект реестроваяЗапись)
        {
            if (serviceCache.Any(
                p => (p.Regrnumber == реестроваяЗапись.Код_РеестрЗапись) &&
                     (p.NameName == заголовок.Имя_ГосУслуг) &&
                     (p.RefYchrInn == заголовок.ИНН) &&
                     (p.SvcCntsName1Val == реестроваяЗапись.Содержание1) &&
                     (p.SvcCntsName2Val == реестроваяЗапись.Содержание2) &&
                     (p.SvcCntsName3Val == реестроваяЗапись.Содержание3) &&
                     (p.SvcTermsName1Val == реестроваяЗапись.Условие1) &&
                     (p.RefTypeName == заголовок.ТипУслуг) &&
                     (p.RefActivityTypeCode == ConvertToActivityTypeCode(заголовок.Код_ВидДеятельности))
                     && p.FromPlaning))
            {
                DataPumpProtocol.WriteProtocolEvent(
                            DataPumpEventKind.dpeInformation,
                            "Дублирование услуги. Услуга {0} уже закачана.".FormatWith(реестроваяЗапись.Код_РеестрЗапись));
                return true;
            }

            return false;
        }

        private bool CheckActuality(Объект реестроваяЗапись)
        {
            var actualCommonService = serviceRepository.FindAll().FirstOrDefault(
                    x => x.Regrnumber.StartsWith(реестроваяЗапись.Код_РеестрЗапись.Substring(0, 42)) && x.BusinessStatus == D_Services_Service.Included);
            if (actualCommonService != null)
            {
                var oldVersion = Convert.ToInt32(actualCommonService.Regrnumber.Substring(42, 3));
                var newVersion = Convert.ToInt32(реестроваяЗапись.Код_РеестрЗапись.Substring(42, 3));

                if (oldVersion > newVersion)
                {
                    DataPumpProtocol.WriteProtocolEvent(
                        DataPumpEventKind.dpeInformation,
                        "Услуга устарела. Закачиваемая {0} - в перечне {1}".FormatWith(реестроваяЗапись.Код_РеестрЗапись, actualCommonService.Regrnumber));
                    return false;
                }

                actualCommonService.BusinessStatus = D_Services_Service.Excluded;
                serviceRepository.Save(actualCommonService);

                DataPumpProtocol.WriteProtocolEvent(
                   DataPumpEventKind.dpeInformation,
                   "Услуга {0} стала неактуальной ".FormatWith(actualCommonService.Regrnumber));
            }

            return true;
        }
        
        private void PumpAll(Объект реестроваяЗапись, Объект заголовок)
        {
            PumpActivityType(заголовок);

            var service = PumpService(реестроваяЗапись, заголовок);

            PumpInstitutionsType(заголовок, service);
            var countConsCateg = PumpConsumersCategory(заголовок, service);

            PumpLegalAct(реестроваяЗапись, service);
            PumpOkpd(реестроваяЗапись, service);
            var countOkved = PumpOkved(реестроваяЗапись, service);
            var countInstInfo = PumpInstitutionsInfo(реестроваяЗапись, service);
            var countVInd = PumpVolumeIndexes(реестроваяЗапись, service);
            PumpQualityIndexes(реестроваяЗапись, service);

            CheckForInclude(service, countOkved, countConsCateg, countInstInfo, countVInd);

            AddToCache(service);
        }

        private void CheckForInclude(D_Services_Service service, int countOkved, int countConsCateg, int countInstInfo, int countVInd)
        {
            // Если все обязательные поля заполнены, то проставляем статус "Включена"
            if (countOkved > 0 && countConsCateg > 0 && countInstInfo > 0 && countVInd > 0)
            {
                service.BusinessStatus = D_Services_Service.Included;
                serviceRepository.Save(service);
            }
        }

        private void AddToCache(D_Services_Service service)
        {
            serviceCache.Add(
                new ServiceCache
                    {
                        Regrnumber = service.Regrnumber,
                        RefTypeName = service.RefType.Name,
                        NameName = service.NameName,
                        RefYchrInn = service.RefYchr.INN,
                        SvcCntsName1Val = service.SvcCntsName1Val,
                        SvcCntsName2Val = service.SvcCntsName2Val,
                        SvcCntsName3Val = service.SvcCntsName3Val,
                        SvcTermsName1Val = service.SvcTermsName1Val,
                        RefActivityTypeCode = service.RefActivityType.Code,
                        FromPlaning = service.FromPlaning.GetValueOrDefault()
                    });
        }

        private D_Services_Service PumpService(Объект реестроваяЗапись, Объект заголовок)
        {
            var service =
                new D_Services_Service
                {
                    InstCode = GetInstCode(реестроваяЗапись),
                    Regrnumber = реестроваяЗапись.Код_РеестрЗапись,
                    SvcCntsName1Val = реестроваяЗапись.Содержание1,
                    SvcCntsName2Val = реестроваяЗапись.Содержание2,
                    SvcCntsName3Val = реестроваяЗапись.Содержание3,
                    SvcTermsName1Val = реестроваяЗапись.Условие1,
                    RefPay = GetRefPay(реестроваяЗапись),

                    GUID = string.Empty,
                    IsEditable = true,
                    BusinessStatus = D_Services_Service.Excluded,

                    // эти поля берем из заголовка
                    EffectiveFrom = GetEffectiveFrom(заголовок),
                    EffectiveBefore = GetEffectiveBefore(заголовок),
                    NameCode = заголовок.Код_ГосУслуг,
                    NameName = заголовок.Имя_ГосУслуг,
                    RefType = GetRefType(заголовок),
                    RefActivityType = GetRefActivityType(заголовок),
                    RefYchr = GetRefYchr(заголовок),
                    RefOKTMO = GetOktmo(заголовок),
                    FromPlaning = true
                };
            serviceRepository.Save(service);
            return service;
        }
        
        private FX_FX_ServicePayType2 GetRefPay(Объект реестроваяЗапись)
        {
            try
            {
                return payTypeRepository.FindAll().FirstOrDefault(x => x.Code.Equals(реестроваяЗапись.Код_Платность)) ??
                       payTypeRepository.FindAll().Single(x => x.Name.Equals(реестроваяЗапись.Платность));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Платность", e.Message));
            }
        }

        private DateTime GetEffectiveFrom(Объект заголовок)
        {
            try
            {
                return заголовок.Срок_С != null ? Convert.ToDateTime(DashesDate(заголовок.Срок_С)) : DateTime.Today;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Срок_С", e.Message));
            }
        }

        private DateTime? GetEffectiveBefore(Объект заголовок)
        {
            try
            {
                return заголовок.Срок_По != null ? Convert.ToDateTime(DashesDate(заголовок.Срок_По)) : (DateTime?)null;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Срок_По", e.Message));
            }
        }

        private FX_FX_ServiceType GetRefType(Объект заголовок)
        {
            try
            {
                return serviceTypeRepository.FindAll().First(x => x.Name.Equals(заголовок.ТипУслуг));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Тип услуги", e.Message));
            }
        }

        private D_Services_ActivityType GetRefActivityType(Объект заголовок)
        {
            try
            {
                return activityTypeRepository.FindAll()
                    .First(x => x.Code.Equals(ConvertToActivityTypeCode(заголовок.Код_ВидДеятельности)));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Вид деятельности", e.Message));
            }
        }

        private D_Org_Structure GetRefYchr(Объект заголовок)
        {
            try
            {
                return orgStructureRepository.FindAll().First(x => x.INN.Equals(заголовок.ИНН));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Учредитель", e.Message));
            }
        }

        private D_OKTMO_OKTMO GetOktmo(Объект заголовок)
        {
            try
            {
                D_OKTMO_OKTMO refOKTMO;
                if (заголовок.ХарактеристикаГРБС != null 
                    && заголовок.ХарактеристикаГРБС.Объект != null 
                    && заголовок.ХарактеристикаГРБС.Объект.Вид_ППО != null)
                {
                    refOKTMO = oktmoRepository.FindAll().First(x => x.Code.Equals(заголовок.ХарактеристикаГРБС.Объект.Вид_ППО));
                }
                else
                {
                    var ppo = orgStructureRepository.FindAll().First(x => x.INN.Equals(заголовок.ИНН)).RefOrgPPO;
                    if (ppo.Name.Equals("Ярославская область"))
                    {
                        refOKTMO = oktmoRepository.FindAll().First(x => x.Name.Equals("Муниципальные образования Ярославской области"));
                    }
                    else if (ppo.Name.Equals("Городской округ город Рыбинск"))
                    {
                        refOKTMO = oktmoRepository.FindAll().First(x => x.Name.Equals("город Рыбинск"));
                    }
                    else
                    {
                        refOKTMO = oktmoRepository.FindAll().First(x => x.Name.Equals(ppo.Name));
                    }
                }

                return refOKTMO;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("ОКТМО", e.Message));
            }
        }

        private void PumpActivityType(Объект заголовок)
        {
            try
            {
                if (activityTypeRepository.FindAll() == null || activityTypeRepository.FindAll().FirstOrDefault(x => x.Code.Equals(заголовок.Код_ВидДеятельности)) == null)
                {
                    var activityType =
                        new D_Services_ActivityType
                            {
                                Code = заголовок.Код_ВидДеятельности,
                                Name = заголовок.Имя_ВидДеятельности,
                            };
                    activityTypeRepository.Save(activityType);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Вид деятельности", e.Message));
            }
        }

        private void PumpInstitutionsType(Объект заголовок, D_Services_Service service)
        {
            try
            {
                if (заголовок.ВидыУчреждений == null)
                {
                    return;
                }
                
                foreach (var видУчр in заголовок.ВидыУчреждений.Объект)
                {
                    var institutionType =
                        new F_F_ServiceInstitutionType
                            {
                                // в перечне Планирования длина кода 6, что не соответсвует форматам 
                                Code = видУчр.Код_ВидУчр.Length > 4 ? видУчр.Код_ВидУчр.Substring(0, 4) : видУчр.Код_ВидУчр,
                                Name = видУчр.Имя_ВидУчр,
                                GUID = string.Empty,
                                RefService = service,
                            };
                    institutionTypeRepository.Save(institutionType);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Виды учреждений", e.Message));
            }
        }

        private void PumpLegalAct(Объект реестроваяЗапись, D_Services_Service service)
        {
            try
            {
                if (реестроваяЗапись.НПА == null)
                {
                    return;
                }

                foreach (var нпа in реестроваяЗапись.НПА.Объект)
                {
                    if (нпа.Имя_НПА.IsNullOrEmpty())
                    {
                        throw new InvalidDataException("Отсутствует наименование НПА");
                    }
                    
                    var legalAct =
                        new F_F_ServiceLegalAct
                            {
                                Kind = нпа.Вид_НПА,
                                Name = нпа.Имя_НПА,
                                LANumber = нпа.Номер_НПА,
                                ApprovedBy = нпа.Орган_ПринНПА,
                                EffectiveFrom = нпа.Дата_ПринНПА != null ? Convert.ToDateTime(DashesDate(нпа.Дата_ПринНПА)) : (DateTime?)null,
                                RefService = service,
                            };
                    legalActRepository.Save(legalAct);
                }
            }
            catch (Exception e)
            {
                DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeWarning, FieldPumpError.FormatWith("НПА", e.Message));
            }
        }

        private void PumpOkpd(Объект реестроваяЗапись, D_Services_Service service)
        {
            try
            {
                if (реестроваяЗапись.ОКПД == null)
                {
                    return;
                }

                foreach (var окпд in реестроваяЗапись.ОКПД.Объект)
                {
                    var okpd =
                        new F_F_ServiceOKPD
                            {
                                Code = окпд.Код_ОКПД,
                                Name = окпд.Имя_ОКПД,
                                RefService = service,
                            };
                    okpdRepository.Save(okpd);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("ОКПД", e.Message));
            }
        }

        private int PumpOkved(Объект реестроваяЗапись, D_Services_Service service)
        {
            try
            {
                if (реестроваяЗапись.ОКВЭД == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var оквэд in реестроваяЗапись.ОКВЭД)
                {
                    foreach (var объект in оквэд.Объект)
                    {
                        var commonOkved = commonOkvedRepository.FindAll().FirstOrDefault(
                            x => x.Code.Equals(объект.ОКВЭД1));
                        if (commonOkved == null)
                        {
                            DataPumpProtocol.WriteProtocolEvent(
                                DataPumpEventKind.dpeWarning,
                                "Не удалось найти ОКВЭД. Услуга {0}".FormatWith(service.Regrnumber));
                        }

                        var okved =
                            new F_F_ServiceOKVED
                                {
                                    Code = объект.ОКВЭД1,
                                    Name = commonOkved != null ? commonOkved.Name : string.Empty,
                                    RefService = service,
                                };
                        okvedRepository.Save(okved);

                        // увеличиваем счетчик только для ОКВЭДов с непустым наименованием
                        if (commonOkved != null)
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("ОКВЭД", e.Message));
            }
        }

        private int PumpConsumersCategory(Объект заголовок, D_Services_Service service)
        {
            try
            {
                if (заголовок.КатегорииПотребителей == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var категория in заголовок.КатегорииПотребителей.Объект)
                {
                    var consumersCategory =
                        new F_F_ServiceConsumersCategory
                            {
                                Code = категория.Код_Категория,
                                Name = категория.Имя_Категория,
                                RefService = service,
                            };
                    consumersCategoryRepository.Save(consumersCategory);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Категории потребителей", e.Message));
            }
        }

        private int PumpInstitutionsInfo(Объект реестроваяЗапись, D_Services_Service service)
        {
            try
            {
                if (реестроваяЗапись.Поставщики == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var поставщик in реестроваяЗапись.Поставщики.Объект)
                {
                    var refStructure = orgStructureRepository.FindAll().FirstOrDefault(x => x.INN.Equals(поставщик.ИНН));
                    if (refStructure == null)
                    {
                        DataPumpProtocol.WriteProtocolEvent(
                            DataPumpEventKind.dpeWarning,
                            "Не удалось найти учреждение ИНН: " + поставщик.ИНН + " для услуги: " + service.Regrnumber);
                        continue;
                    }

                    var institutionsInfo =
                        new F_F_ServiceInstitutionsInfo
                            {
                                RefStructure = refStructure,
                                RefService = service,
                            };

                    institutionsInfoRepository.Save(institutionsInfo);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Информация об учреждениях", e.Message));
            }
        }

        private int PumpVolumeIndexes(Объект реестроваяЗапись, D_Services_Service service)
        {
            try
            {
                if (реестроваяЗапись.ПоказателиОбъема == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var показательОбъема in реестроваяЗапись.ПоказателиОбъема.Объект)
                {
                    var okei = commonOkeiTypeRepository.FindAll().FirstOrDefault(x => x.Name.ToString().Equals(показательОбъема.ЕдИзм));
                    if (okei == null)
                    {
                        DataPumpProtocol.WriteProtocolEvent(
                            DataPumpEventKind.dpeWarning,
                            "Не удалось найти ОКЕИ. Услуга {0}".FormatWith(service.Regrnumber));
                        continue;
                    }

                    var ind =
                        new F_F_ServiceIndicators
                            {
                                Name = показательОбъема.Имя_Показатель,
                                Code = показательОбъема.Код_Показатель,
                                RefType = charateristicTypeRepository.FindOne(1), // Сопоставляем по названию XML аттрибута (vInd)
                                RefOKEI = okei,
                                RefService = service,
                            };
                    indicatorsRepository.Save(ind);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Показатели объема", e.Message));
            }
        }

        private void PumpQualityIndexes(Объект реестроваяЗапись, D_Services_Service service)
        {
            try
            {
                if (реестроваяЗапись.ПоказателиКачества == null)
                {
                    return;
                }
                
                foreach (var показательКачества in реестроваяЗапись.ПоказателиКачества.Объект)
                {
                    var okei = commonOkeiTypeRepository.FindAll().FirstOrDefault(x => x.Name.ToString().Equals(показательКачества.ЕдИзм));
                    if (okei == null)
                    {
                        DataPumpProtocol.WriteProtocolEvent(
                            DataPumpEventKind.dpeWarning,
                            "Не удалось найти ОКЕИ. Услуга {0}".FormatWith(service.Regrnumber));
                        continue;
                    }

                    var ind =
                        new F_F_ServiceIndicators
                            {
                                Name = показательКачества.Имя_Показатель,
                                Code = показательКачества.Код_Показатель,
                                RefType = charateristicTypeRepository.FindOne(2), // Сопоставляем по названию XML аттрибута (qInd)
                                RefOKEI = okei,
                                RefService = service,
                            };
                    indicatorsRepository.Save(ind);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Показатели качества", e.Message));
            }
        }

    #region Nested Classes
    private class ServiceCache
        {
            public string Regrnumber { get; set; }

            public string RefTypeName { get; set; }

            public string NameName { get; set; }

            public string RefYchrInn { get; set; }

            public string SvcCntsName1Val { get; set; }

            public string SvcCntsName2Val { get; set; }

            public string SvcCntsName3Val { get; set; }

            public string SvcTermsName1Val { get; set; }

            public string RefActivityTypeCode { get; set; }

            public bool FromPlaning { get; set; }
        }
        #endregion
    }
}
