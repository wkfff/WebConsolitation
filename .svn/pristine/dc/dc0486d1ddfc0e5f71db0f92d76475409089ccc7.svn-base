using System;
using System.Linq;
using System.Xml.Schema;
using bus.gov.ru;
using bus.gov.ru.types.Item1;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Bus.Gov.Ru.Imports
{
    // TODO прикрутить этот класс к закачке 
    // TODO что бы не дублировать и использовать одно поведение
    
    /// <summary>
    /// Закачка Гос Задания
    /// </summary>
    public class StateTaskPump
    {
        private const int PartDoc = FX_FX_PartDoc.StateTaskDocTypeID;

        private const bool PlanThreeYear = true;

        private const int State = FX_Org_SostD.CreatedStateID;

        private readonly CommonPump commonPump;

        private readonly ILinqRepository<D_Services_VedPer> services;

        private readonly ILinqRepository<D_Services_TipY> typeService;

        private readonly ILinqRepository<F_F_PNRysl> indicatorsService;

        private readonly ILinqRepository<D_Services_Indicators> indicators;

        private readonly ILinqRepository<FX_FX_CharacteristicType> typeIndicators;

        private readonly ILinqRepository<D_Org_OKEI> okeiIndicators;

        private readonly ILinqRepository<F_F_VedPerProvider> serviceProviders;

        private readonly ILinqRepository<D_Org_Structure> institutions;

        public StateTaskPump()
        {
            services = Resolver.Get<ILinqRepository<D_Services_VedPer>>();
            typeService = Resolver.Get<ILinqRepository<D_Services_TipY>>();
            indicatorsService = Resolver.Get<ILinqRepository<F_F_PNRysl>>();
            indicators = Resolver.Get<ILinqRepository<D_Services_Indicators>>();
            typeIndicators = Resolver.Get<ILinqRepository<FX_FX_CharacteristicType>>();
            okeiIndicators = Resolver.Get<ILinqRepository<D_Org_OKEI>>();
            serviceProviders = Resolver.Get<ILinqRepository<F_F_VedPerProvider>>();
            institutions = Resolver.Get<ILinqRepository<D_Org_Structure>>();
            
            commonPump = CommonPump.GetCommonPump;
        }

        /// <summary>
        /// Выполняет закачку из файла
        /// </summary>
        /// <param name="pumpData">
        /// The pump Data.
        /// </param>
        public int PumpFile(stateTaskType pumpData)
        {
            string result;

            // Проверка по схеме
            if (!pumpData.Validate(Resolver.Get<XmlSchemaSet>(), out result))
            {
                /*throw new Exception(result);*/
                commonPump.DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeError, "Документ не соответствует формату" + result);
            }

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            D_Org_Structure uchr;

            // Проверка учреждения
            // если ИНН есть то ищем по инн в "наших" учреждениях
            if (targetOrg.inn.HasValue)
            {
                uchr = targetOrg.kpp.HasValue ? institutions.FindAll().SingleOrDefault(x => !x.CloseDate.HasValue && x.INN.Equals(targetOrg.inn.ToString()) 
                                                                                                    && x.KPP.Equals(targetOrg.kpp.ToString()))
                                                : institutions.FindAll().SingleOrDefault(x => !x.CloseDate.HasValue && x.INN.Equals(targetOrg.inn.ToString()));

                if (uchr == null)
                {
                    var info = "Наименование: {0} ИНН: {1} КПП: {2}"
                        .FormatWith(
                            targetOrg.fullName.Return(
                                s => string.Format("({0})", s),
                                string.Empty),
                            targetOrg.inn,
                            targetOrg.kpp);

                    commonPump.DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeError, "Учреждение не найдено:" + info);
                }    
            }
            else
            {
                if (!commonPump.CheckInstTarget(targetOrg))
                {
                    var info = "Учреждение: {0} regnum: {1} не найдено, требуется обновление nsiOgs"
                        .FormatWith(
                                    targetOrg.fullName,
                                    targetOrg.regNum);

                    commonPump.DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeError, "Учреждение не найдено:" + info);
                }    

                uchr = commonPump.OrgStructuresByRegnumCache[targetOrg.regNum];
            }
           
            if (uchr == null)
            {
                return -1;
            }

            var year = Convert.ToInt32(pumpData.nextFinancialYear);
            
            // Проверка предыдущих документов
            commonPump.CheckDocs(uchr.ID, PartDoc, year, new[] { FX_Org_SostD.ExportedStateID });

            commonPump.HeaderRepository.DbContext.BeginTransaction();

            var header = new F_F_ParameterDoc
                            {
                                PlanThreeYear = PlanThreeYear,
                                RefPartDoc = commonPump.TypeDocCache.FindOne(PartDoc),
                                RefSost = commonPump.StateDocCache.FindOne(State),
                                RefUchr = uchr,
                                RefYearForm = commonPump.YearFormCache.FindOne(year),
                                OpeningDate = DateTime.Now
                            };

            pumpData.service.Each(x =>
                                {
                                    var typeServiceId = x.type.Equals("W") ? D_Services_TipY.FX_FX_WORK : D_Services_TipY.FX_FX_SERVICE;

                                    var service = GetSrvice(x.name, typeServiceId, uchr);

                                    if (service != null)
                                    {
                                        var st = new F_F_GosZadanie
                                                {
                                                    RefParametr = header,
                                                    RefVedPch = service,
                                                    RazdelN = x.ordinalNumber
                                                };

                                        try
                                        {
                                            var payment = st.RefVedPch.RefPl.Code;

                                            x.volumeIndex.Each(vi => AddIndicator(vi, FX_FX_CharacteristicType.VolumeIndex, st));

                                            x.qualityIndex.Each(qi => AddIndicator(qi, FX_FX_CharacteristicType.QualityIndex, st));

                                            // делаем только для служб
                                            if (typeServiceId == D_Services_TipY.FX_FX_SERVICE)
                                            {
                                                x.renderEnactment.Each(re => st.RenderOrders.Add(new F_F_NPARenderOrder
                                                {
                                                    RefFactGZ = st,
                                                    RenderEnact = re
                                                }));
                                                if (payment != D_Services_Platnost.Free)
                                                {
                                                    st.CenaEd = x.payment.averagePrice;
                                                    st.Prices.Add(new F_F_NPACena
                                                    {
                                                        RefGZPr = st,
                                                        VidNPAGZ = x.payment.priceEnactment.type,
                                                        OrgUtvDoc = x.payment.priceEnactment.author.fullName,
                                                        DataNPAGZ = x.payment.priceEnactment.date,
                                                        Name = x.payment.priceEnactment.name,
                                                        NumNPA = x.payment.priceEnactment.number
                                                    });

                                                    x.payment.limitPrice.Each(lp => st.Limits.Add(new F_F_LimitPrice
                                                    {
                                                        RefFactGZ = st,
                                                        Name = lp.name,
                                                        Price = lp.price
                                                    }));
                                                }

                                                x.informingProcedure.Each(ip => st.InformingProcedures.Add(new F_F_InfoProcedure
                                                {
                                                    RefFactGZ = st,
                                                    Content = ip.content,
                                                    Method = ip.method,
                                                    Rate = ip.rate
                                                }));
                                            }

                                            x.supervisionProcedure.Each(sp => header.StateTasksSupervisionProcedures.Add(new F_F_OrderControl
                                            {
                                                RefFactGZ = header,
                                                Form = sp.form,
                                                Rate = sp.rate,
                                                Supervisor = sp.supervisor.fullName
                                            }));

                                            if (x.reportRequirements != null)
                                            {
                                                x.reportRequirements.earlyTermination.Each(et => header.StateTasksTerminations.Add(new F_F_BaseTermination
                                                {
                                                    RefFactGZ = header,
                                                    EarlyTerminat = et
                                                }));

                                                x.reportRequirements.deliveryTerm.Each(dt => header.StateTasksRequestAccounts.Add(new F_F_RequestAccount
                                                {
                                                    RefFactGZ = header,
                                                    DeliveryTerm = dt
                                                }));

                                                x.reportRequirements.otherRequirement.Each(or => header.StateTasksRequestAccounts.Add(new F_F_RequestAccount
                                                {
                                                    RefFactGZ = header,
                                                    OtherRequest = or
                                                }));

                                                x.reportRequirements.otherInfo.Each(oi => header.StateTasksRequestAccounts.Add(new F_F_RequestAccount
                                                {
                                                    RefFactGZ = header,
                                                    OtherInfo = oi
                                                }));
                                            }

                                            header.StateTasks.Add(st);
                                        }
                                        catch (Exception e)
                                        {
                                            commonPump.DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeWarning, "Во время закачки возникло предупреждение: " + e.ExpandException());
                                        }
                                    }
                                });

            commonPump.ProcessDocumentsHeader(
                                                header,
                                                pumpData.document,
                                                type => type.With(x => commonPump.DocumentTypesCache.FindAll().First(doc => doc.Code.Equals("A"))));

            if (header.StateTasks.Any())
            {
                commonPump.HeaderRepository.Save(header);
                commonPump.HeaderRepository.DbContext.CommitChanges();
                commonPump.HeaderRepository.DbContext.CommitTransaction();

                return header.ID;
            }

            throw new Exception("Документ пуст и небыл сохранен");
        }

        // поиск и проверка услуги
        private D_Services_VedPer GetSrvice(string name, int typeId, D_Org_Structure institution)
        {
            var providers = serviceProviders.FindAll().Where(x => x.RefProvider.INN.Equals(institution.INN)).Select(x => x.RefService.ID);

            if (!providers.Any())
            {
               /* throw new Exception("Не найдена услуга(работа) \"{0}\" в реестре услуг с поставщиком ИНН: \"{1}\"".FormatWith(name, institution.INN));*/
               commonPump.DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeWarning, "Не найдена услуга(работа) в реестре услуг. Наименование услуги\"{0}\", поставщик ИНН: \"{1}\"".FormatWith(name, institution.INN));
                return null;
            }

            try
            {
                return services.FindAll().Single(x => x.Name.Trim().Equals(name) && x.RefTipY.ID.Equals(typeId) && providers.Contains(x.ID));
            }
            catch (Exception e)
            {
                /*throw new Exception("Не найдена услуга(работа) в реестре услуг: \"{0}\", тип: \"{1}\", поставщик ИНН: \"{2}\"".FormatWith(name, typeService.Load(typeId).Name, institution.INN), e);*/

                commonPump.DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeWarning, "Не найдена услуга(работа) в реестре услуг. Наименование услуги\"{0}\", тип: \"{1}\", поставщик ИНН: \"{2}\" \r\n Exception: {3}".FormatWith(name, typeService.Load(typeId).Name, institution.INN, e.Message));
                return null;
            }
        }

        // поиск и проверка показателя
        private D_Services_Indicators GetIndicator(string name, int okei, int type, int service)
        {
            D_Org_OKEI okeiItem;

            var typeIndicator = typeIndicators.Load(type);

            try
            {
                okeiItem = okeiIndicators.FindAll().Single(x => x.Code.Equals(okei));
            }
            catch (Exception e)
            {
                /*throw new Exception("В справочнике отсутствует единица измерения с кодом: \"{0}\"".FormatWith(okei));*/
                commonPump.DataPumpProtocol.WriteProtocolEvent(DataPumpEventKind.dpeWarning, "Не найдена единица измерения. Код: {0} \r\n Exception: {1}".FormatWith(okei, e.Message));

                return null;
            }

            D_Services_Indicators servicesIndicators;

            try
            {
                servicesIndicators = indicators.FindAll().Single(x => x.Name.Equals(name) && x.RefCharacteristicType.ID.Equals(type) && x.RefOKEI.ID.Equals(okei));
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning,
                    "Не найден показатель качества или обема услуги в справочнике. Наименование: {0}, единица измерения: {1}, тип: {2} \r\n Exception: {3}".FormatWith(name, okeiItem.Name, typeIndicator.Name, e.Message));

                return null;
            }

            if (indicatorsService.FindAll().Any(x => x.RefPerV.ID.Equals(service) && x.RefIndicators.ID.Equals(servicesIndicators.ID)))
            {
                return servicesIndicators;
            }

            commonPump.DataPumpProtocol.WriteProtocolEvent(
                DataPumpEventKind.dpeWarning, 
                "В реестре услуг у услуги не заведен показатель. Наименование: {0}, единица измерения: {1}, тип: {2}".FormatWith(name, okeiItem.Name, typeIndicator.Name));

            return null;
        }

        // добавление показателей
        private void AddIndicator(serviceIndexType indicatorType, int type, F_F_GosZadanie stateTask)
        {
            var indicator = GetIndicator(indicatorType.index.name, Convert.ToInt32(indicatorType.index.unit.code), type, stateTask.RefVedPch.ID);
            if (indicator != null)
            {
                stateTask.Indicators.Add(new F_F_PNRZnach
                {
                    RefFactGZ = stateTask,
                    RefIndicators = indicator,
                    ReportingYear = indicatorType.valueYear.reportYear,
                    CurrentYear = indicatorType.valueYear.currentYear,
                    ComingYear = indicatorType.valueYear.nextYear,
                    FirstPlanYear = indicatorType.valueYear.planFirstYear,
                    SecondPlanYear = indicatorType.valueYear.planLastYear,
                    ActualValue = indicatorType.valueActual != null ? indicatorType.valueActual.actualValue : null,
                    Protklp = indicatorType.valueActual != null ? indicatorType.valueActual.rejectReason : null
                });
            }
        }
    }
}