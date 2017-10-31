using System;
using System.IO;
using System.Linq;
using bus.gov.ru.types.Item1;
using Bus.Gov.Ru.Imports;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace bus.gov.ru.Imports
{
    public class StateTask2016Pump
    {
        private const string ErrorPump = "Ошибка при закачке '{0}'";
        private const string ErrorServicePump = "Ошибка при закачке услуги '{0}'";
        private const string ErrorServiceDetailPump = "Услуга '{0}': Не удалось закачать '{1}'";

        private const int PartDoc = FX_FX_PartDoc.StateTaskDocTypeID;
        private const int State = FX_Org_SostD.CreatedStateID;
        private const bool PlanThreeYear = true;

        private readonly CommonPump commonPump;

        private readonly ILinqRepository<F_F_GosZadanie2016> gosZadanie;
        private readonly ILinqRepository<D_Org_Structure> institutions;
        private readonly ILinqRepository<F_F_ServiceConsumersCategory> consumersCategory; 
        private readonly ILinqRepository<F_F_GZYslPotr2016> consumers; 
        private readonly ILinqRepository<F_F_InfoProcedure2016> infoProcedure; 
        private readonly ILinqRepository<F_F_NPACena2016> npaCena; 
        private readonly ILinqRepository<F_F_PNRZnach2016> pnrZnach; 
        private readonly ILinqRepository<F_F_AveragePrice> averagePriceRepo; 
        private readonly ILinqRepository<F_F_NPARenderOrder2016> npaRenderOrder; 
        private readonly ILinqRepository<D_Services_Service> serviceRepo; 
        private readonly ILinqRepository<F_F_ServiceIndicators> indicatorsRepo; 
        private readonly ILinqRepository<F_F_ServiceInstitutionsInfo> instInfoRepo; 

        private readonly ILinqRepository<F_F_Reports> reportsRepo;
        private readonly ILinqRepository<F_F_RequestAccount2016> reqAccRepo;
        private readonly ILinqRepository<F_F_OtherInfo> otherInfoRepo;
        private readonly ILinqRepository<F_F_OrderControl2016> orderControlRepo;
        private readonly ILinqRepository<F_F_BaseTermination2016> baseTerminationRepo;

        private int year;

        public StateTask2016Pump()
        {
            commonPump = CommonPump.GetCommonPump;

            gosZadanie = Resolver.Get<ILinqRepository<F_F_GosZadanie2016>>();
            institutions = Resolver.Get<ILinqRepository<D_Org_Structure>>();
            consumersCategory = Resolver.Get<ILinqRepository<F_F_ServiceConsumersCategory>>();
            consumers = Resolver.Get<ILinqRepository<F_F_GZYslPotr2016>>();
            infoProcedure = Resolver.Get<ILinqRepository<F_F_InfoProcedure2016>>();
            npaCena = Resolver.Get<ILinqRepository<F_F_NPACena2016>>();
            pnrZnach = Resolver.Get<ILinqRepository<F_F_PNRZnach2016>>();
            averagePriceRepo = Resolver.Get<ILinqRepository<F_F_AveragePrice>>();
            npaRenderOrder = Resolver.Get<ILinqRepository<F_F_NPARenderOrder2016>>();
            serviceRepo = Resolver.Get<ILinqRepository<D_Services_Service>>();
            indicatorsRepo = Resolver.Get<ILinqRepository<F_F_ServiceIndicators>>();
            instInfoRepo = Resolver.Get<ILinqRepository<F_F_ServiceInstitutionsInfo>>();

            reportsRepo = Resolver.Get<ILinqRepository<F_F_Reports>>();
            reqAccRepo = Resolver.Get<ILinqRepository<F_F_RequestAccount2016>>();
            otherInfoRepo = Resolver.Get<ILinqRepository<F_F_OtherInfo>>();
            orderControlRepo = Resolver.Get<ILinqRepository<F_F_OrderControl2016>>();
            baseTerminationRepo = Resolver.Get<ILinqRepository<F_F_BaseTermination2016>>();
        }

        public int PumpFile(stateTask640rType pumpData)
        {
            var targetStructure = pumpData.initiator ?? pumpData.placer;
            D_Org_Structure structure;

            if (targetStructure.inn.HasValue)
            {
                structure = targetStructure.kpp.HasValue
                    ? institutions.FindAll().SingleOrDefault(
                        x => !x.CloseDate.HasValue && x.INN.Equals(targetStructure.inn.ToString())
                             && x.KPP.Equals(targetStructure.kpp.ToString()))
                    : institutions.FindAll().SingleOrDefault(x => !x.CloseDate.HasValue && x.INN.Equals(targetStructure.inn.ToString()));

                if (structure == null)
                {
                    var info = "Наименование: {0} ИНН: {1} КПП: {2}"
                        .FormatWith(
                            targetStructure.fullName.Return(
                                s => string.Format("({0})", s),
                                string.Empty),
                            targetStructure.inn,
                            targetStructure.kpp);

                    commonPump.DataPumpProtocol.WriteProtocolEvent(
                        DataPumpEventKind.dpeError, 
                        "Учреждение не найдено: " + info);
                }
            }
            else
            {
                if (!commonPump.CheckInstTarget(targetStructure))
                {
                    var info = "Учреждение: {0} regnum: {1} не найдено, требуется обновление nsiOgs"
                        .FormatWith(
                            targetStructure.fullName,
                            targetStructure.regNum);

                    commonPump.DataPumpProtocol.WriteProtocolEvent(
                        DataPumpEventKind.dpeError,
                        "Учреждение не найдено: " + info);
                }

                structure = commonPump.OrgStructuresByRegnumCache[targetStructure.regNum];
            }

            if (structure == null)
            {
                return -1;
            }

            year = Convert.ToInt32(pumpData.nextFinancialYear);

            commonPump.CheckDocs(structure.ID, PartDoc, year, new[] { FX_Org_SostD.ExportedStateID });

            commonPump.HeaderRepository.DbContext.BeginTransaction();

            var header = new F_F_ParameterDoc
                {
                    PlanThreeYear = PlanThreeYear,
                    RefPartDoc = commonPump.TypeDocCache.FindOne(PartDoc),
                    RefSost = commonPump.StateDocCache.FindOne(State),
                    RefUchr = structure,
                    RefYearForm = commonPump.YearFormCache.FindOne(year),
                    OpeningDate = DateTime.Now
                };

            commonPump.HeaderRepository.Save(header);
            
            PumpAll(pumpData, header);

            // todo переделать закачки таким образом чтобы каждая деталь качалась отдельной транзакцией, чтобы ошибка возникшая при закачке детали не влияла на весь процесс закачки!
            commonPump.HeaderRepository.DbContext.CommitChanges();
            commonPump.HeaderRepository.DbContext.CommitTransaction();

            return header.ID;
        }

        private void PumpAll(stateTask640rType stateTask640R, F_F_ParameterDoc header)
        {
            foreach (var report in stateTask640R.reports)
            {
                PumpReport(report, header);
            }

            var countPumpedServices = 0;
            foreach (var serviceLocalType in stateTask640R.service)
            {
                countPumpedServices += PumpServices(serviceLocalType, header);
            }

            if (countPumpedServices == 0)
            {
                throw new InvalidDataException("Не удалось импортировать ни одной услуги");
            }

            foreach (var early in stateTask640R.earlyTermination)
            {
                PumpEarlyTermination(early, header);
            }

            foreach (var supervision in stateTask640R.supervisionProcedure)
            {
                PumpSupervisionProcedure(supervision, header);
            }

            foreach (var otherInfo in stateTask640R.otherInfo)
            {
                PumpOtherInfo(otherInfo, header);
            }
            
            if (stateTask640R.reportRequirements != null)
            {
                PumpReportRequirements(stateTask640R.reportRequirements, header);
            }
        }

        private void PumpReportRequirements(stateTask640rType.reportRequirementsLocalType reportRequirements, F_F_ParameterDoc header)
        {
            try
            {
                for (var i = 0; i < reportRequirements.deliveryTerm.Count; i++)
                {
                    reqAccRepo.Save(
                        new F_F_RequestAccount2016
                            {
                                RefFactGZ = header,
                                PeriodicityTerm = reportRequirements.periodicityTerm,
                                DeliveryTerm = reportRequirements.deliveryTerm[i],
                                OtherIndicators = reportRequirements.otherIndicators[i],
                                OtherRequest = reportRequirements.otherRequirement[i]
                            });
                }
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorPump.FormatWith("Требования к отчетности: " + e.Message));
            }
        }

        private void PumpReport(stateTask640rType.reportsLocalType report, F_F_ParameterDoc header)
        {
            try
            {
                reportsRepo.Save(
                    new F_F_Reports
                        {
                            RefFactGZ = header,
                            ReportGuid = report.reportGUID.TypedValue,
                            NameReport = report.periodInfo,
                            DateReport = report.date,
                            HeadName = report.head.name,
                            HeadPosition = report.head.position
                        });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorPump.FormatWith("Отчеты о выполнении" + e.Message));
            }
        }

        private void PumpOtherInfo(string otherInfo, F_F_ParameterDoc header)
        {
            try
            {
                otherInfoRepo.Save(
                    new F_F_OtherInfo
                        {
                            RefFactGZ = header,
                            OtherInfo = otherInfo
                        });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                     DataPumpEventKind.dpeWarning, 
                     ErrorPump.FormatWith("Иная информация" + e.Message));
            }
        }

        private void PumpSupervisionProcedure(stateTask640rType.supervisionProcedureLocalType supervision, F_F_ParameterDoc header)
        {
            try
            {
                orderControlRepo.Save(
                    new F_F_OrderControl2016
                        {
                            RefFactGZ = header,
                            Form = supervision.form,
                            Rate = supervision.rate,
                            Supervisor = supervision.supervisor.fullName
                        });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorPump.FormatWith("Порядок контроля за исполнением" + e.Message));
            }
        }

        private void PumpEarlyTermination(string early, F_F_ParameterDoc header)
        {
            try
            {
                baseTerminationRepo.Save(
                    new F_F_BaseTermination2016
                        {
                            EarlyTerminat = early,
                            RefFactGZ = header
                        });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorPump.FormatWith("Основания для досрочного прекращения" + e.Message));
            }
        }

        private int PumpServices(stateTask640rType.serviceLocalType serviceLocalType, F_F_ParameterDoc header)
        {
            var countPumpedServices = 0;
            foreach (var indexesLocalType in serviceLocalType.indexes)
            {
                if (PumpGZ(serviceLocalType, header, indexesLocalType))
                {
                    countPumpedServices++;
                }
            }

            return countPumpedServices;
        }

        private bool PumpGZ(stateTask640rType.serviceLocalType serviceLocalType, F_F_ParameterDoc header, stateTask640rType.serviceLocalType.indexesLocalType indexesLocalType)
        {
            try
            {
                var service = GetService(indexesLocalType.regNum);
                if (service == null)
                {
                    throw new InvalidDataException("Данная услуга отсутствует в Перечне." +
                                                   "<br>По данному вопросу необходимо обратиться к учредителю");
                }

                if (service.BusinessStatus == D_Services_Service.Excluded)
                {
                    throw new InvalidDataException("Данная услуга является закрытой и недоступной для выбора." +
                                                   "<br>По данному вопросу необходимо обратиться к учредителю");
                }

                if (!instInfoRepo.FindAll().Any(x => x.RefService.ID == service.ID && x.RefStructure.INN == header.RefUchr.INN))
                {
                    throw new InvalidDataException("Учреждение не является поставщиком для данной услуги/работы." +
                                                   "<br>По данному вопросу необходимо обратиться к учредителю");
                }
                
                var gz = new F_F_GosZadanie2016
                    {
                        IsOtherSources = service.IsEditable,
                        RefParameter = header,
                        RefService = service
                    };

                if (year < 2017)
                {
                    gz.AveragePrice = serviceLocalType.averagePrice;
                }
                else
                {
                    gz.AveragePrice = service.RefPay.Code.Equals("1") ? serviceLocalType.averagePrice : null;
                }

                gosZadanie.Save(gz);

                PumpServiceDetails(serviceLocalType, gz);
                return true;
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorServicePump.FormatWith(indexesLocalType.regNum + ": " + e.Message));
                return false;
            }
        }

        private void PumpServiceDetails(stateTask640rType.serviceLocalType serviceLocalType, F_F_GosZadanie2016 gz)
        {
            foreach (var qualityIndex in serviceLocalType.qualityIndex.Where(x => x.index.regNum == gz.RefService.Regrnumber))
            {
                PumpQInd(qualityIndex, gz);
            }

            foreach (var volumeIndex in serviceLocalType.volumeIndex.Where(x => x.index.regNum == gz.RefService.Regrnumber))
            {
                PumpVInd(volumeIndex, gz);
            }

            foreach (var category in serviceLocalType.category)
            {
                PumpConsumersCategory(category, gz);
            }

            foreach (var infoProc in serviceLocalType.informingProcedure)
            {
                PumpInformingProcedures(infoProc, gz);
            }

            foreach (var priceEnact in serviceLocalType.priceEnactment)
            {
                PumpPrices(priceEnact, gz);
            }

            foreach (var value in serviceLocalType.renderEnactment)
            {
                PumpRenderOrders(value, gz);
            }
        }

        private void PumpPrices(stateTask640rType.serviceLocalType.priceEnactmentLocalType priceEnact, F_F_GosZadanie2016 gz)
        {
            try
            {
                npaCena.Save(
                    new F_F_NPACena2016
                        {
                            RefFactGZ = gz,
                            DataNPAGZ = CommonUtils.IsValidSqlDate(priceEnact.date) ? priceEnact.date : (DateTime?)null,
                            Name = priceEnact.name,
                            NumNPA = priceEnact.number,
                            OrgUtvDoc = priceEnact.author.fullName,
                            VidNPAGZ = priceEnact.type
                        });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorServiceDetailPump.FormatWith(gz.RefService.Regrnumber, "НПА, устанавливающие цены:" + e.Message));
            }
        }

        private void PumpInformingProcedures(stateTask640rType.serviceLocalType.informingProcedureLocalType infoProc, F_F_GosZadanie2016 gz)
        {
            try
            {
                infoProcedure.Save(
                    new F_F_InfoProcedure2016
                        {
                            Content = infoProc.content,
                            Method = infoProc.method,
                            Rate = infoProc.rate,
                            RefFactGZ = gz
                        });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning,
                    ErrorServiceDetailPump.FormatWith(gz.RefService.Regrnumber, "Порядок информирования потребителей: " + e.Message));
            }
        }

        private void PumpConsumersCategory(refNsiCustomerCategoryType category, F_F_GosZadanie2016 gz)
        {
            try
            {
                consumers.Save(
                    new F_F_GZYslPotr2016
                        {
                            RefConsumersCategory = GetCategory(gz.RefService.ID, category.name),
                            RefFactGZ = gz
                        });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorServiceDetailPump.FormatWith(gz.RefService.Regrnumber, "Категории потребителей: " + e.Message));
            }
        }
        
        private void PumpQInd(serviceIndex640rType index, F_F_GosZadanie2016 gz)
        {
            try
            {
                var pnr = new F_F_PNRZnach2016
                    {
                        ComingYear = index.valueYear.nextYear,
                        CurrentYear = index.valueYear.currentYear,
                        ReportingYear = index.valueYear.reportYear,
                        SecondPlanYear = index.valueYear.planLastYear,
                        FirstPlanYear = index.valueYear.planFirstYear,
                        Deviation = index.deviation,
                        RefIndicators = GetIndicator(gz.RefService.ID, index.index.name),
                        RefFactGZ = gz
                    };

                if (index.valueActual != null && index.valueActual.Count != 0)
                {
                    pnr.ActualValue = index.valueActual[0].actualValue;
                    pnr.Reject = index.valueActual[0].reject;
                    pnr.Protklp = index.valueActual[0].rejectReason;

                    if (index.valueActual[0].reportGUID == null)
                    {
                        throw new InvalidDataException("Нет ссылки на отчет");
                    }

                    pnr.RefReport = GetReport(gz.RefParameter.ID, index.valueActual[0].reportGUID.TypedValue);
                }

                pnrZnach.Save(pnr);
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorServiceDetailPump.FormatWith(gz.RefService.Regrnumber, "Показатели качества: " + e.Message));
            }
        }

        private void PumpVInd(stateTask640rType.serviceLocalType.volumeIndexLocalType index, F_F_GosZadanie2016 gz)
        {
            try
            {
                var pnr = new F_F_PNRZnach2016
                    {
                        ComingYear = index.valueYear.nextYear,
                        CurrentYear = index.valueYear.currentYear,
                        ReportingYear = index.valueYear.reportYear,
                        SecondPlanYear = index.valueYear.planLastYear,
                        FirstPlanYear = index.valueYear.planFirstYear,
                        Deviation = index.deviation,
                        RefIndicators = GetIndicator(gz.RefService.ID, index.index.name),
                        RefFactGZ = gz
                    };

                if (index.valueActual != null && index.valueActual.Count != 0)
                {
                    pnr.ActualValue = index.valueActual[0].actualValue;
                    pnr.Reject = index.valueActual[0].reject;
                    pnr.Protklp = index.valueActual[0].rejectReason;

                    if (index.valueActual[0].reportGUID == null)
                    {
                        throw new InvalidDataException("Нет ссылки на отчет");
                    }

                    pnr.RefReport = GetReport(gz.RefParameter.ID, index.valueActual[0].reportGUID.TypedValue);
                }

                pnrZnach.Save(pnr);

                if (gz.RefService.RefType.Code.Equals(FX_FX_ServiceType.CodeOfService) &&
                    gz.RefService.RefPay.Code.Equals(FX_FX_ServicePayType2.CodeOfPayable))
                {
                    PumpVIndAveragePrice(index.averagePrice, pnr);
                }
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorServiceDetailPump.FormatWith(gz.RefService.Regrnumber, "Показатели объема: " + e.Message));
            }
        }

        private void PumpVIndAveragePrice(stateTask640rType.serviceLocalType.volumeIndexLocalType.averagePriceLocalType averagePrice, F_F_PNRZnach2016 pnr)
        {
            try
            {
                if (year < 2017)
                {
                    averagePriceRepo.Save(
                    new F_F_AveragePrice
                    {
                        CurrentYearDec = averagePrice.currentYear,
                        NextYearDec = averagePrice.nextYear,
                        PlanFirstYearDec = averagePrice.planFirstYear,
                        PlanLastYearDec = averagePrice.planLastYear,
                        ReportYearDec = averagePrice.reportYear,
                        RefVolumeIndex = pnr
                    });
                }
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    "Услуга: '{0}': В показателях объема не удалось закачать среднегодой размер платы: {1}".FormatWith(pnr.RefFactGZ.RefService.Regrnumber, e.Message));
            }
        }

        private void PumpRenderOrders(stateTask640rType.serviceLocalType.renderEnactmentLocalType renderEnactment, F_F_GosZadanie2016 gz)
        {
            try
            {
                npaRenderOrder.Save(
                    new F_F_NPARenderOrder2016
                        {
                            RefFactGZ = gz,
                            DateNpa = CommonUtils.IsValidSqlDate(renderEnactment.date) ? renderEnactment.date : (DateTime?)null,
                            NumberNpa = renderEnactment.number,
                            RenderEnact = renderEnactment.name,
                            TypeNpa = renderEnactment.type,
                            Author = renderEnactment.author.fullName
                    });
            }
            catch (Exception e)
            {
                commonPump.DataPumpProtocol.WriteProtocolEvent(
                    DataPumpEventKind.dpeWarning, 
                    ErrorServiceDetailPump.FormatWith(gz.RefService.Regrnumber, "НПА, регулирующий порядок оказания услуги: " + e.Message));
            }
        }

        private D_Services_Service GetService(string regNum)
        {
            if (year < 2017)
            {
                return serviceRepo.FindAll().FirstOrDefault(x => x.Regrnumber == regNum);
            }

            return serviceRepo.FindAll().FirstOrDefault(x => x.BusinessStatus.Equals(D_Services_Service.Included)
                                                            && x.FromPlaning.HasValue && x.FromPlaning.Value
                                                            && (!x.EffectiveBefore.HasValue || (x.EffectiveBefore.HasValue && x.EffectiveBefore.Value.Year >= year))
                                                            && x.Regrnumber == regNum);
        }

        private F_F_ServiceConsumersCategory GetCategory(int serviceId, string name)
        {
            return consumersCategory.FindAll().First(
                x => x.RefService.ID == serviceId && x.Name == name);
        }

        private F_F_ServiceIndicators GetIndicator(int serviceId, string name)
        {
            return indicatorsRepo.FindAll().First(x => x.RefService.ID == serviceId && x.Name == name);
        }

        private F_F_Reports GetReport(int docId, string reportGuid)
        {
            return reportsRepo.FindAll().First(x => x.RefFactGZ.ID == docId && x.ReportGuid == reportGuid);
        }
    }
}
