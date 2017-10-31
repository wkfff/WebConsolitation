using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using eb.www.roskazna.ru.eb.domain.EPGU_DepRegList.formular;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace bus.gov.ru.Imports
{
    /// <summary>
    /// Закачка Ведомственного перечня услуг из Электронного бюджета
    /// </summary>
    public class Service2016EbPump
    {
        private const string FieldDefinitionError = "Не удалось определить '{0}': {1}";
        private const string FieldPumpError = "Не удалось закачать '{0}': {1}";

        // Репозитории для Услуги
        private readonly ILinqRepository<D_Services_Service> serviceRepository;
        private readonly ILinqRepository<FX_FX_ServiceType> serviceTypeRepository;
        private readonly ILinqRepository<D_Services_ActivityType> activityTypeRepository;
        private readonly ILinqRepository<D_OKTMO_OKTMO> oktmoRepository;
        private readonly ILinqRepository<FX_FX_ServicePayType2> payTypeRepository;
        private readonly ILinqRepository<D_Org_Structure> orgStructureRepository;

        // Репозитории для детализаций Услуги
        private readonly ILinqRepository<F_F_ServiceInstitutionType> institutionTypeRepository;
        private readonly ILinqRepository<F_F_ServiceLegalAct> legalActRepository;
        private readonly ILinqRepository<F_F_ServiceOKPD> okpdRepository;
        private readonly ILinqRepository<F_F_ServiceOKVED> okvedRepository;
        private readonly ILinqRepository<F_F_ServiceConsumersCategory> consumersCategoryRepository;
        private readonly ILinqRepository<F_F_ServiceInstitutionsInfo> institutionsInfoRepository;
        private readonly ILinqRepository<F_F_ServiceIndicators> indicatorsRepository;
        private readonly ILinqRepository<FX_FX_CharacteristicType> charateristicTypeRepository;
        private readonly ILinqRepository<D_Org_OKEI> okeiTypeRepository;
        
        private readonly string region;

        // Кэш для быстрых проверок на дубли услуг
        private List<ServiceCache> serviceCache;

        public Service2016EbPump()
        {
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
            okeiTypeRepository = Resolver.Get<ILinqRepository<D_Org_OKEI>>();
        }

        public Service2016EbPump(string region)
        {
            this.region = region;
        }

        private IDataPumpProtocol DataPumpProtocolProvider { get; set; }

        private string FileName { get; set; }

        public void Pump(StreamReader file, IDataPumpProtocol dataPumpProtocolProvider, string fileName = "")
        {
            try
            {
                DataPumpProtocolProvider = dataPumpProtocolProvider;
                FileName = fileName;

                var depRegList = EPGU_DepRegList.Load(file).EPGU_SvcListDepReg.EPGU_SvcListDepReg_ITEM;

                InitCache();

                foreach (var pumpData in depRegList)
                {
                    ProcessPump(pumpData);
                }
            }
            catch (Exception e)
            {
                DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 0, 0, "Ошибка при закачке файла {0}: {1}".FormatWith(FileName, e.Message));
            }
        }
        
        private void ProcessPump(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            if (!IsNeedToPump(pumpData))
            {
                return;
            }

            try
            {
                serviceRepository.DbContext.BeginTransaction();

                if (!CheckActuality(pumpData))
                {
                    serviceRepository.DbContext.RollbackTransaction();
                    return;
                }

                PumpAll(pumpData);
                
                serviceRepository.DbContext.CommitChanges();
                serviceRepository.DbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                serviceRepository.DbContext.RollbackTransaction();
                DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, 0, 0, "Ошибка при закачке услуги {0}: {1}".FormatWith(pumpData.regrnumber, e.Message));
            }
        }

        private void InitCache()
        {
            serviceCache = serviceRepository.FindAll()
                .Where(x => x.IsEditable == true)
                .Select(
                    p => new ServiceCache
                    {
                        Regrnumber = p.Regrnumber,
                        RefTypeCode = p.RefType.Code,
                        NameName = p.NameName,
                        RefYchrInn = p.RefYchr.INN,
                        SvcCntsName1Val = p.SvcCntsName1Val,
                        SvcCntsName2Val = p.SvcCntsName2Val,
                        SvcCntsName3Val = p.SvcCntsName3Val,
                        SvcTermsName1Val = p.SvcTermsName1Val,
                        SvcTermsName2Val = p.SvcTermsName2Val,
                        RefActivityTypeCode = p.RefActivityType.Code,
                    })
                .ToList();
        }

        private bool IsNeedToPump(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            // Если услуга уже закачана отменяем закачку
            // Поиск осуществляется только по перечню из иных источников
            if (serviceCache.Any(
                p => (p.Regrnumber == pumpData.regrnumber) &&
                     (p.NameName == pumpData.Name_Name) &&
                     (p.RefYchrInn == pumpData.inn) &&
                     (p.SvcCntsName1Val == pumpData.SvcCntsName1Val) &&
                     (p.SvcCntsName2Val == pumpData.SvcCntsName2Val) &&
                     (p.SvcCntsName3Val == pumpData.SvcCntsName3Val) &&
                     (p.SvcTermsName1Val == pumpData.SvcTermsName1Val) &&
                     (p.SvcTermsName2Val == pumpData.SvcTermsName2Val) &&
                     (p.RefTypeCode == pumpData.SvcKind_Code) &&
                     (p.RefActivityTypeCode == pumpData.ActDomnCode)))
            {
                DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeInformation, 0, 0, "Файл {0}: ".FormatWith(FileName) + "Услуга " + pumpData.regrnumber + " уже закачана.");
                return false;
            }
            
            if (!pumpData.inn.StartsWith(GetRegion()))
            {
                return false;
            }

            return true;
        }

        private string GetRegion()
        {
            if (region != null)
            {
                return region;
            }

            switch (ConfigurationManager.AppSettings["ClientLocationOKATOCode"])
            {
                case "78":
                    return "76";
                case "96":
                    return "20";
                default:
                    throw new InvalidDataException("Неизвестный код ОКАТО");
            }
        }

        private bool CheckActuality(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            if (pumpData.isactual == false)
            {
                var actualService = serviceRepository.FindAll().FirstOrDefault(
                    x => x.Regrnumber == pumpData.regrnumber && x.BusinessStatus == D_Services_Service.Included);
                if (actualService != null)
                {
                    actualService.BusinessStatus = D_Services_Service.Excluded;
                    serviceRepository.Save(actualService);
                    InfoProtocolEvent("Услуга стала неактуальной " + actualService.Regrnumber);
                }

                return false;
            }

            var actualCommonService = serviceRepository.FindAll().FirstOrDefault(
                    x => x.Regrnumber.StartsWith(pumpData.regrnumber.Substring(0, 42)) && x.BusinessStatus == D_Services_Service.Included);
            if (actualCommonService != null)
            {
                var oldVersion = Convert.ToInt32(actualCommonService.Regrnumber.Substring(42, 3));
                var newVersion = Convert.ToInt32(pumpData.regrnumber.Substring(42, 3));

                if (oldVersion > newVersion)
                {
                    InfoProtocolEvent("Закачиваемая услуга {0} устарела по сравнению с имеющейся в перечне ({1})"
                        .FormatWith(pumpData.regrnumber, actualCommonService.Regrnumber));
                    return false;
                }

                actualCommonService.BusinessStatus = D_Services_Service.Excluded;
                serviceRepository.Save(actualCommonService);
                InfoProtocolEvent("Услуга стала неактуальной " + actualCommonService.Regrnumber);
            }

            return true;
        }

        private void PumpAll(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            PumpActivityType(pumpData);

            var service = PumpService(pumpData);

            PumpInstitutionsType(pumpData, service);
            PumpLegalAct(pumpData, service);
            PumpOkpd(pumpData, service);
            var countOkved = PumpOkved(pumpData, service);
            var countConsCateg = PumpConsumersCategory(pumpData, service);
            var countInstInfo = PumpInstitutionsInfo(pumpData, service);
            var countVInd = PumpVolumeIndexes(pumpData, service);
            PumpQualityIndexes(pumpData, service);

            CheckForInclude(service, countOkved, countConsCateg, countInstInfo, countVInd);

            AddToCache(service);

            InfoProtocolEvent("Услуга добавлена " + pumpData.regrnumber);
        }

        private void CheckForInclude(D_Services_Service service, int countOkved, int countConsCateg, int countInstInfo, int countVInd)
        {
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
                        RefTypeCode = service.RefType.Code,
                        NameName = service.NameName,
                        RefYchrInn = service.RefYchr.INN,
                        SvcCntsName1Val = service.SvcCntsName1Val,
                        SvcCntsName2Val = service.SvcCntsName2Val,
                        SvcCntsName3Val = service.SvcCntsName3Val,
                        SvcTermsName1Val = service.SvcTermsName1Val,
                        SvcTermsName2Val = service.SvcTermsName2Val,
                        RefActivityTypeCode = service.RefActivityType.Code,
                    });
        }

        private D_Services_Service PumpService(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            var service =
                new D_Services_Service
                    {
                        Regrnumber = pumpData.regrnumber,
                        EffectiveFrom = pumpData.EffectiveFrom,
                        EffectiveBefore = pumpData.EffectiveBefore,
                        InstCode = pumpData.inst_code,
                        NameCode = pumpData.Name_Code.Replace(".", string.Empty),
                        NameName = pumpData.Name_Name,
                        SvcCnts1CodeVal = pumpData.SvcCnts1CodeVal,
                        SvcCnts2CodeVal = pumpData.SvcCnts2CodeVal,
                        SvcCnts3CodeVal = pumpData.SvcCnts3CodeVal,
                        SvcCntsName1Val = pumpData.SvcCntsName1Val,
                        SvcCntsName2Val = pumpData.SvcCntsName2Val,
                        SvcCntsName3Val = pumpData.SvcCntsName3Val,
                        SvcTerms1CodeVal = pumpData.SvcTerms1CodeVal,
                        SvcTerms2CodeVal = pumpData.SvcTerms2CodeVal,
                        SvcTermsName1Val = pumpData.SvcTermsName1Val,
                        SvcTermsName2Val = pumpData.SvcTermsName2Val,
                        GUID = pumpData.GUID,
                        RefType = GetRefType(pumpData),
                        RefActivityType = GetRefActivityType(pumpData),
                        RefOKTMO = GetOktmo(pumpData),
                        RefPay = GetRefPay(pumpData), // ЭБ не соответствует своим же форматам
                        RefYchr = GetRefYchr(pumpData),
                        IsEditable = true,
                        BusinessStatus = D_Services_Service.Excluded,
                };
            serviceRepository.Save(service);
            return service;
        }

        private FX_FX_ServiceType GetRefType(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            try
            {
                return serviceTypeRepository.FindAll().FirstOrDefault(x => x.Code.Equals(pumpData.SvcKind_Code));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Тип услуги", e.Message));
            }
        }

        private D_Services_ActivityType GetRefActivityType(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            try
            {
                return activityTypeRepository.FindAll().FirstOrDefault(x => x.Code.Equals(pumpData.ActDomnCode));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Вид деятельности", e.Message));
            }
        }

        private D_OKTMO_OKTMO GetOktmo(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            try
            {
                D_OKTMO_OKTMO refOKTMO;
                if (pumpData.ppocode != null)
                {
                    refOKTMO = oktmoRepository.FindAll().First(x => x.Code.Equals(pumpData.ppocode));
                }
                else
                {
                    var ppo = orgStructureRepository.FindAll().First(x => x.INN.Equals(pumpData.inn)).RefOrgPPO;
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

        private FX_FX_ServicePayType2 GetRefPay(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            try
            {
                return payTypeRepository.FindAll().FirstOrDefault(x => x.Code.Equals(pumpData.depregistry_paid.Substring(0, 1)));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Платность", e.Message));
            }
        }

        private D_Org_Structure GetRefYchr(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            try
            {
                return orgStructureRepository.FindAll().FirstOrDefault(x => x.INN.Equals(pumpData.inn));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Учредитель", e.Message));
            }
        }

        private int PumpActivityType(tEPGU_SvcListDepReg_ITEM pumpData)
        {
            try
            {
                var count = 0;
                if (activityTypeRepository.FindAll() == null || activityTypeRepository.FindAll().FirstOrDefault(x => x.Code.Equals(pumpData.ActDomnCode)) == null)
                {
                    var activityType =
                        new D_Services_ActivityType
                            {
                                Code = pumpData.ActDomnCode,
                                Name = pumpData.ActDomnName,
                            };
                    activityTypeRepository.Save(activityType);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Вид деятельности", e.Message));
            }
        }

        private int PumpInstitutionsType(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.DEPREGISTRY_IK == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var depregistryIkItem in pumpData.DEPREGISTRY_IK.DEPREGISTRY_IK_ITEM)
                {
                    var institutionType =
                        new F_F_ServiceInstitutionType
                            {
                                Code = depregistryIkItem.Code.Substring(0, 4), // в перечне ЭБ длина кода 7, что не соответсвует форматам 
                                Name = depregistryIkItem.Name,
                                GUID = depregistryIkItem.GID,
                                RefService = service,
                            };
                    institutionTypeRepository.Save(institutionType);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Вид учреждений", e.Message));
            }
        }

        private int PumpLegalAct(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_la == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var tdepregistryLaItem in pumpData.depregistry_la.depregistry_la_ITEM)
                {
                    var legalAct =
                        new F_F_ServiceLegalAct
                            {
                                Kind = tdepregistryLaItem.LglAct_Knd,
                                Name = tdepregistryLaItem.LglAct_Name,
                                LANumber = tdepregistryLaItem.LglAct_LANumber,
                                ApprovedBy = tdepregistryLaItem.LglAct_ApprovedBy,
                                ApprvdAt = tdepregistryLaItem.LglAct_ApprvdAt,
                                EffectiveFrom = tdepregistryLaItem.LglAct_EffectiveFrom,
                                DatetEnd = tdepregistryLaItem.dtend,
                                MJnumber = tdepregistryLaItem.MJnumber,
                                MJregdate = tdepregistryLaItem.MJregdate,
                                RefService = service,
                            };
                    legalActRepository.Save(legalAct);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("НПА", e.Message));
            }
        }

        private int PumpOkpd(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_okpd == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var tdepregistryOkpdItem in pumpData.depregistry_okpd.depregistry_okpd_ITEM)
                {
                    var okpd =
                        new F_F_ServiceOKPD
                            {
                                Code = tdepregistryOkpdItem.RuClsPrEcAcs_Code,
                                Name = tdepregistryOkpdItem.RuClsPrEcAcs_Name,
                                RefService = service,
                            };
                    okpdRepository.Save(okpd);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("ОКПД", e.Message));
            }
        }

        private int PumpOkved(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_okved == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var tdepregistryOkvedItem in pumpData.depregistry_okved.depregistry_okved_ITEM)
                {
                    var okved =
                        new F_F_ServiceOKVED
                            {
                                Code = tdepregistryOkvedItem.RuClsEcActs_Code,
                                Name = tdepregistryOkvedItem.RuClsEcActs_Name,
                                RefService = service,
                            };
                    okvedRepository.Save(okved);
                    count++;
                }

                return count;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("ОКВЭД", e.Message));
            }
        }

        private int PumpConsumersCategory(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_conscat == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var tdepregistryConscatItem in pumpData.depregistry_conscat.depregistry_conscat_ITEM)
                {
                    var consumersCategory =
                        new F_F_ServiceConsumersCategory
                            {
                                Code = tdepregistryConscatItem.CsmCtgy_Code,
                                Name = tdepregistryConscatItem.CsmCtgy_Name,
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

        private int PumpInstitutionsInfo(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depreginsttninfo == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var tdepreginsttninfoItem in pumpData.depreginsttninfo.depreginsttninfo_ITEM)
                {
                    var refStructure = orgStructureRepository.FindAll().FirstOrDefault(x => x.INN.Equals(tdepreginsttninfoItem.inn));
                    if (refStructure == null)
                    {
                        DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeWarning,
                            0,
                            0,
                            "Файл {0}: ".FormatWith(FileName) + "Не удалось найти учреждение" + tdepreginsttninfoItem.inn + " для услуги " + service.Regrnumber);
                        continue;
                    }

                    var institutionsInfo =
                        new F_F_ServiceInstitutionsInfo
                            {
                                Code = tdepreginsttninfoItem.code,
                                OKOPF = tdepreginsttninfoItem.okopf,
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

        private int PumpVolumeIndexes(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_vind == null)
                {
                    return 0;
                }

                var count = 0;
                foreach (var tdepregistryVindItem in pumpData.depregistry_vind.depregistry_vind_ITEM)
                {
                    var okei = okeiTypeRepository.FindAll().FirstOrDefault(x => x.Name.ToString().Equals(tdepregistryVindItem.VolInd_Units));
                    if (okei == null)
                    {
                        DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeWarning,
                            0,
                            0,
                            "Файл {0}: ".FormatWith(FileName) + "Не удалось найти ОКЕИ для услуги {0}".FormatWith(service.Regrnumber));
                        continue;
                    }

                    var ind =
                        new F_F_ServiceIndicators
                            {
                                Name = tdepregistryVindItem.VolInd_Name,
                                Code = tdepregistryVindItem.VolInd_Code,
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

        private void PumpQualityIndexes(tEPGU_SvcListDepReg_ITEM pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_qind == null)
                {
                    return;
                }
                
                foreach (var tdepregistryQindItem in pumpData.depregistry_qind.depregistry_qind_ITEM)
                {
                    var okei = okeiTypeRepository.FindAll().FirstOrDefault(x => x.Code.ToString().Equals(tdepregistryQindItem.QltyInd_UnСode))
                               ?? okeiTypeRepository.FindAll().FirstOrDefault(x => x.Name.ToString().Equals(tdepregistryQindItem.QltyInd_Units));
                    if (okei == null)
                    {
                        DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeWarning,
                            0,
                            0,
                            "Файл {0}: ".FormatWith(FileName) + "Не удалось найти ОКЕИ для услуги {0}".FormatWith(service.Regrnumber));
                        continue;
                    }

                    var ind =
                        new F_F_ServiceIndicators
                            {
                                Name = tdepregistryQindItem.QltyInd_Name,
                                Code = tdepregistryQindItem.QltyInd_Code,
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

        private void InfoProtocolEvent(string message)
        {
            DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                DataPumpEventKind.dpeInformation,
                0,
                0,
                "Файл {0}: {1}".FormatWith(FileName, message));
        }

        #region Nested Classes
        private class ServiceCache
        {
            public string Regrnumber { get; set; }

            public string RefTypeCode { get; set; }

            public string NameName { get; set; }

            public string RefYchrInn { get; set; }

            public string SvcCntsName1Val { get; set; }

            public string SvcCntsName2Val { get; set; }
            
            public string SvcCntsName3Val { get; set; }

            public string SvcTermsName1Val { get; set; }

            public string SvcTermsName2Val { get; set; }

            public string RefActivityTypeCode { get; set; }
        }

        #endregion
    }
}
