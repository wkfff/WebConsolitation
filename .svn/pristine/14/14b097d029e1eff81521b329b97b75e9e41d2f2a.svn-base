using System;
using System.IO;
using System.Linq;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

using www.roskazna.ru.eb.domain.EPGU_SvcListDepReg.formular;

namespace bus.gov.ru.Imports
{
    /// <summary>
    /// Закачка Ведомственного перечня услуг из ГМУ
    /// </summary>
    public class Service2016Pump
    {
        private const string FieldDefinitionError = "Не удалось определить '{0}': {1}";
        private const string FieldPumpError = "Не удалось закачать '{0}': {1}";

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
        private ILinqRepository<D_Org_OKEI> okeiTypeRepository;

        // код региона
        private string region;
        
        public Service2016Pump()
        {
            InitPump();
        }

        public Service2016Pump(string region)
        {
            this.region = region;
            InitPump();
        }

        private IDataPumpProtocol DataPumpProtocolProvider { get; set; }

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
            okeiTypeRepository = Resolver.Get<ILinqRepository<D_Org_OKEI>>();
        }

        public void Pump(StreamReader file, IDataPumpProtocol dataPumpProtocolProvider, string fileName = "")
        {
            try
            {
                DataPumpProtocolProvider = dataPumpProtocolProvider;
                FileName = fileName;
                var pumpData = EPGU_SvcListDepReg.Load(file);
                ProcessPump(pumpData);
            }
            catch (Exception e)
            {
                DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 0, 0, "Ошибка при закачке файла {0}: {1}".FormatWith(FileName, e.Message));
            }
        }

        private static void CheckExistanceIndexes(D_Services_Service service, int countVInd)
        {
            const string Msg = "Нет данных по показателям объема для {0} {1}";

            if (countVInd == 0)
            {
                if (service.RefType.Code.Equals(FX_FX_ServiceType.CodeOfWork))
                {
                    throw new InvalidDataException(Msg.FormatWith("работы", service.Regrnumber));
                }

                if (service.RefType.Code.Equals(FX_FX_ServiceType.CodeOfService))
                {
                    throw new InvalidDataException(Msg.FormatWith("услуги", service.Regrnumber));
                }
            }
        }

        private static string GetNameCode(EPGU_SvcListDepReg pumpData)
        {
            try
            {
                return pumpData.Name_Code.Replace(".", string.Empty);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Неверный формат кода услуги: " + e.Message);
            }
        }

        private void ProcessPump(EPGU_SvcListDepReg pumpData)
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
        
        private bool IsNeedToPump(EPGU_SvcListDepReg pumpData)
        {
            // Если услуга уже закачана отменяем закачку
            // Поиск осуществляется только по перечню ГМУ
            if (serviceRepository.FindAll().FirstOrDefault(x => x.GUID.Equals(pumpData.GUID) && (x.IsEditable == false || !x.IsEditable.HasValue)) != null)
            {
                return false;
            }

            // Если услуга не для нужного района отменяем закачку
            if (!pumpData.inn.StartsWith(region))
            {
                return false;
            }

            return true;
        }
        
        private bool CheckActuality(EPGU_SvcListDepReg pumpData)
        {
            if (pumpData.isactual == false)
            {
                var actualService = serviceRepository.FindAll()
                    .FirstOrDefault(x => (x.IsEditable == false || !x.IsEditable.HasValue)
                                        && x.Regrnumber.Equals(pumpData.regrnumber)
                                        && x.BusinessStatus.Equals(D_Services_Service.Included));
                if (actualService != null)
                {
                    actualService.BusinessStatus = D_Services_Service.Excluded;
                    serviceRepository.Save(actualService);
                    InfoProtocolEvent("Услуга стала неактуальной " + actualService.Regrnumber);
                }

                return false;
            }
            
            var actualCommonService = serviceRepository.FindAll()
                        .FirstOrDefault(x => (x.IsEditable == false || !x.IsEditable.HasValue)
                                            && x.Regrnumber.StartsWith(pumpData.regrnumber.Substring(0, 42)) 
                                            && x.BusinessStatus.Equals(D_Services_Service.Included));
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

        private void PumpAll(EPGU_SvcListDepReg pumpData)
        {
            PumpActivityType(pumpData);

            var service = PumpService(pumpData);

            PumpInstitutionsType(pumpData, service);
            PumpLegalAct(pumpData, service);
            PumpOkpd(pumpData, service);
            PumpOkved(pumpData, service);
            PumpConsumersCategory(pumpData, service);
            PumpInstitutionsInfo(pumpData, service);
            var countVInd = PumpVolumeIndexes(pumpData, service);
            PumpQualityIndexes(pumpData, service);
            
            CheckExistanceIndexes(service, countVInd);

            InfoProtocolEvent("Услуга добавлена " + pumpData.regrnumber);
        }
        
        private D_Services_Service PumpService(EPGU_SvcListDepReg pumpData)
        {
            var service =
                new D_Services_Service
                    {
                        Regrnumber = pumpData.regrnumber,
                        EffectiveFrom = pumpData.EffectiveFrom,
                        EffectiveBefore = pumpData.EffectiveBefore,
                        InstCode = pumpData.inst_code,
                        NameCode = GetNameCode(pumpData),
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
                        RefPay = GetRefPay(pumpData),
                        RefYchr = GetRefYchr(pumpData),
                        BusinessStatus = D_Services_Service.Included,
                        IsEditable = false
                };
            serviceRepository.Save(service);
            return service;
        }
        
        private FX_FX_ServiceType GetRefType(EPGU_SvcListDepReg pumpData)
        {
            try
            {
                return serviceTypeRepository.FindAll().First(x => x.Code.Equals(pumpData.SvcKind_Code));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Тип услуги", e.Message));
            }
        }

        private D_Services_ActivityType GetRefActivityType(EPGU_SvcListDepReg pumpData)
        {
            try
            {
                return activityTypeRepository.FindAll().First(x => x.Code.Equals(pumpData.ActDomnCode));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Вид деятельности", e.Message));
            }
        }

        private D_OKTMO_OKTMO GetOktmo(EPGU_SvcListDepReg pumpData)
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

        private FX_FX_ServicePayType2 GetRefPay(EPGU_SvcListDepReg pumpData)
        {
            try
            {
                // todo: ГМУ не обновляют форматы, приходится извращаться #15221
                return payTypeRepository.FindAll().First(x => x.Code.Equals(pumpData.depregistry_paid.Untyped.Value.Substring(0, 1)));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Платность", e.Message));
            }
        }

        private D_Org_Structure GetRefYchr(EPGU_SvcListDepReg pumpData)
        {
            try
            {
                return orgStructureRepository.FindAll().First(x => x.INN.Equals(pumpData.inn));
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldDefinitionError.FormatWith("Учредитель", e.Message));
            }
        }

        private void PumpActivityType(EPGU_SvcListDepReg pumpData)
        {
            try
            {
                if (activityTypeRepository.FindAll() == null || activityTypeRepository.FindAll().FirstOrDefault(x => x.Code.Equals(pumpData.ActDomnCode)) == null)
                {
                    var activityType =
                        new D_Services_ActivityType
                            {
                                Code = pumpData.ActDomnCode,
                                Name = pumpData.ActDomnName
                            };
                    activityTypeRepository.Save(activityType);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Вид деятельности", e.Message));
            }
        }

        private void PumpInstitutionsType(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.DEPREGISTRY_IK == null)
                {
                    return;
                }

                foreach (var depregistryIkItem in pumpData.DEPREGISTRY_IK.DEPREGISTRY_IK_ITEM)
                {
                    var institutionType =
                        new F_F_ServiceInstitutionType
                            {
                                Code = depregistryIkItem.Code,
                                Name = depregistryIkItem.Name,
                                GUID = depregistryIkItem.GID,
                                RefService = service
                            };
                    institutionTypeRepository.Save(institutionType);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Виды учреждений", e.Message));
            }
        }

        private void PumpLegalAct(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_la == null)
                {
                    return;
                }

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
                                RefService = service
                            };
                    legalActRepository.Save(legalAct);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("НПА", e.Message));
            }
        }

        private void PumpOkpd(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_okpd == null)
                {
                    return;
                }

                foreach (var tdepregistryOkpdItem in pumpData.depregistry_okpd.depregistry_okpd_ITEM)
                {
                    var okpd =
                        new F_F_ServiceOKPD
                            {
                                Code = tdepregistryOkpdItem.RuClsPrEcAcs_Code,
                                Name = tdepregistryOkpdItem.RuClsPrEcAcs_Name,
                                RefService = service
                            };
                    okpdRepository.Save(okpd);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("ОКПД", e.Message));
            }
        }

        private void PumpOkved(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                foreach (var tdepregistryOkvedItem in pumpData.depregistry_okved.depregistry_okved_ITEM)
                {
                    var okved =
                        new F_F_ServiceOKVED
                            {
                                Code = tdepregistryOkvedItem.RuClsEcActs_Code,
                                Name = tdepregistryOkvedItem.RuClsEcActs_Name,
                                RefService = service
                            };
                    okvedRepository.Save(okved);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("ОКВЭД", e.Message));
            }
        }

        private void PumpConsumersCategory(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                foreach (var tdepregistryConscatItem in pumpData.depregistry_conscat.depregistry_conscat_ITEM)
                {
                    var consumersCategory =
                        new F_F_ServiceConsumersCategory
                            {
                                Code = tdepregistryConscatItem.CsmCtgy_Code,
                                Name = tdepregistryConscatItem.CsmCtgy_Name,
                                RefService = service
                            };
                    consumersCategoryRepository.Save(consumersCategory);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Категории потребителей", e.Message));
            }
        }

        private void PumpInstitutionsInfo(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                foreach (var tdepreginsttninfoItem in pumpData.depreginsttninfo.depreginsttninfo_ITEM)
                {
                    var refStructure = orgStructureRepository.FindAll().FirstOrDefault(x => x.INN.Equals(tdepreginsttninfoItem.inn));
                    if (refStructure == null)
                    {
                        throw new InvalidDataException("Не удалось найти учреждение: " + tdepreginsttninfoItem.inn);
                    }

                    var institutionsInfo =
                        new F_F_ServiceInstitutionsInfo
                            {
                                Code = tdepreginsttninfoItem.code,
                                OKOPF = tdepreginsttninfoItem.okopf,
                                RefStructure = refStructure,
                                RefService = service
                            };

                    institutionsInfoRepository.Save(institutionsInfo);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(FieldPumpError.FormatWith("Информация об учреждениях", e.Message));
            }
        }

        private int PumpVolumeIndexes(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                var count = 0;
                foreach (var tdepregistryVindItem in pumpData.depregistry_vind.depregistry_vind_ITEM)
                {
                    var okei = okeiTypeRepository.FindAll().FirstOrDefault(x => x.Name.ToString().Equals(tdepregistryVindItem.VolInd_Units));
                    if (okei == null)
                    {
                        DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeWarning, 0, 0, "Файл {0}: ".FormatWith(FileName) + "Не удалось найти ОКЕИ для услуги {0}".FormatWith(service.Regrnumber));
                        continue;
                    }

                    var ind =
                        new F_F_ServiceIndicators
                            {
                                Name = tdepregistryVindItem.VolInd_Name,
                                Code = tdepregistryVindItem.VolInd_Code,
                                RefType = charateristicTypeRepository.FindOne(FX_FX_CharacteristicType.VolumeIndex), // Сопоставляем по названию XML аттрибута (vInd)
                                RefOKEI = okei,
                                RefService = service
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

        private void PumpQualityIndexes(EPGU_SvcListDepReg pumpData, D_Services_Service service)
        {
            try
            {
                if (pumpData.depregistry_qind == null)
                {
                    return;
                }
                
                foreach (var tdepregistryQindItem in pumpData.depregistry_qind.depregistry_qind_ITEM)
                {
                    var okei = okeiTypeRepository.FindAll().FirstOrDefault(x => x.Name.ToString().Equals(tdepregistryQindItem.QltyInd_Units));
                    if (okei == null)
                    {
                        DataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeWarning, 0, 0, "Файл {0}: ".FormatWith(FileName) + "Не удалось найти ОКЕИ для услуги {0}".FormatWith(service.Regrnumber));
                        continue;
                    }

                    var ind =
                        new F_F_ServiceIndicators
                            {
                                Name = tdepregistryQindItem.QltyInd_Name,
                                Code = tdepregistryQindItem.QltyInd_Code,
                                RefType = charateristicTypeRepository.FindOne(FX_FX_CharacteristicType.QualityIndex), // Сопоставляем по названию XML аттрибута (qInd)
                                RefOKEI = okei,
                                RefService = service
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
    }
}
