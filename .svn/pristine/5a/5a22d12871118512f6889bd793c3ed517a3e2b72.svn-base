using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public class ExportStateTask2016Service
    {
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            var tasks = header.StateTasks2016.ToList();
            var target = header.RefUchr;
            var placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            var year = header.RefYearForm.ID;

            var requirements = header.StateTasksRequestAccounts2016.ToList();

            var stateTask640RContent =
                new stateTask640rType
                {
                    positionId = Guid.NewGuid().ToString(),
                    changeDate = DateTime.Now,
                    placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                    initiator = target.ID != placerProfile.RefUchr.ID
                            ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                            : null,

                    reportYear = year - 2,
                    financialYear = year - 1,
                    nextFinancialYear = year,
                    planFirstYear = year + 1,
                    planLastYear = year + 2,

                    service = tasks.GroupBy(x => x.RefService.NameCode).Select(ServiceLocal640R).ToList(),

                    earlyTermination = header.StateTasksTerminations2016.Select(termination => termination.EarlyTerminat).ToList(),
                    otherInfo = header.StateTasksOtherInfo.Select(oi => oi.OtherInfo).ToList(),
                    reportRequirements = requirements.Count > 0
                            ? new stateTask640rType.reportRequirementsLocalType
                            {
                                periodicityTerm = requirements.Select(account => account.PeriodicityTerm).FirstOrDefault(),
                                deliveryTerm = requirements.Select(account => account.DeliveryTerm).ToList(),
                                otherRequirement = requirements.Select(account => account.OtherRequest).ToList(),
                                otherIndicators = requirements.Select(account => account.OtherIndicators).ToList()
                            }
                            : null,
                    supervisionProcedure = header.StateTasksSupervisionProcedures2016.Select(
                            control => new stateTask640rType.supervisionProcedureLocalType
                            {
                                rate = control.Rate,
                                form = control.Form,
                                supervisor = new refNsiOgsSoftType1 { fullName = control.Supervisor }
                            })
                            .ToList(),
                    reports = header.StateTasksReports.Select(
                            report => new stateTask640rType.reportsLocalType
                            {
                                date = report.DateReport,
                                head = new stateTask640rType.reportsLocalType.headLocalType { name = report.HeadName, position = report.HeadPosition },
                                periodInfo = report.NameReport,
                                reportGUID = new reportGUID { TypedValue = report.ReportGuid }
                            })
                            .ToList(),
                    document = ExportServiceHelper.Documents(header.Documents.ToList())
                };

            AdditionalInformation(stateTask640RContent, header);

            stateTask640RContent.service.Where(type => type.type.Equals("W")).Each((type, i) => type.ordinalNumber = i + 1);
            stateTask640RContent.service.Where(type => type.type.Equals("S")).Each((type, i) => type.ordinalNumber = i + 1);

            return ExportServiceHelper.Serialize(
                new stateTask640r
                {
                    header = ExportServiceHelper.HeaderType(),
                    body = new stateTask640r.bodyLocalType
                    {
                        position = stateTask640RContent
                    }
                }.Save);
        }

        /// <summary>
        /// Дополнительная информация о ГЗ
        /// </summary>
        private static void AdditionalInformation(stateTask640rType position, F_F_ParameterDoc header)
        {
            if (header.StateTasksExtHeader.Any())
            {
                var stateTasksExtHeader = header.StateTasksExtHeader.First();
                if (stateTasksExtHeader.StatementTask.HasValue)
                {
                    position.statementDate = stateTasksExtHeader.StatementTask;
                }
                
                if (stateTasksExtHeader.StateTaskNumber.IsNotNullOrEmpty())
                {
                    position.number = stateTasksExtHeader.StateTaskNumber;
                }

                if (stateTasksExtHeader.ApproverFirstName.IsNotNullOrEmpty())
                {
                    position.approverFirstName = stateTasksExtHeader.ApproverFirstName;
                }

                if (stateTasksExtHeader.ApproverLastName.IsNotNullOrEmpty())
                {
                    position.approverLastName = stateTasksExtHeader.ApproverLastName;
                }

                if (stateTasksExtHeader.ApproverMiddleName.IsNotNullOrEmpty())
                {
                    position.approverMiddleName = stateTasksExtHeader.ApproverMiddleName;
                }

                if (stateTasksExtHeader.ApproverPosition.IsNotNullOrEmpty())
                {
                    position.approverMiddleName = stateTasksExtHeader.ApproverPosition;
                }
            }
        }

        private static stateTask640rType.serviceLocalType ServiceLocal640R(IGrouping<string, F_F_GosZadanie2016> tasks)
        {
            var taskHeader = tasks.First();

            var isWork = taskHeader.RefService.RefType.Code.Equals(FX_FX_ServiceType.CodeOfWork);

            var serviceLocal =
                new stateTask640rType.serviceLocalType
                {
                    code = taskHeader.RefService.NameCode,
                    name = taskHeader.RefService.NameName,
                    type = isWork ? "W" : "S",
                    ordinalNumber = taskHeader.OrdinalNumber,
                    category = tasks
                            .SelectMany(x => x.CustomersCategory)
                            .Select(x => new refNsiCustomerCategoryType { name = x.RefConsumersCategory.Name })
                            .GroupBy(x => x.name)
                            .Select(group => group.First())
                            .ToList()
                };

            if (!isWork)
            {
                if (taskHeader.RefService.RefPay.Code.Equals(FX_FX_ServicePayType2.CodeOfPayable))
                {
                    serviceLocal.payService = true;
                    serviceLocal.averagePrice = taskHeader.AveragePrice;
                }

                serviceLocal.priceEnactment = tasks
                    .Where(task => task.RefService.RefPay.Code.Equals(FX_FX_ServicePayType2.CodeOfPayable))
                    .SelectMany(PriceEnactment640R)
                    .GroupBy(x => new { x.type, x.author.fullName, x.date, x.name, x.number })
                    .Select(group => group.First())
                    .ToList();

                serviceLocal.renderEnactment = tasks
                    .SelectMany(RenderEnactment640R)
                    .GroupBy(x => new { x.name, x.number, x.type })
                    .Select(group => group.First())
                    .ToList();

                serviceLocal.informingProcedure = tasks
                    .SelectMany(InformingProcedureLocalTypes640R)
                    .GroupBy(x => new { x.content, x.method, x.rate })
                    .Select(group => group.First())
                    .ToList();
            }
            
            serviceLocal.qualityIndex = tasks.SelectMany(task => ServiceIndexTypes640R(task, FX_FX_CharacteristicType.QualityIndex)).ToList();
            
            serviceLocal.volumeIndex = tasks.SelectMany(task => VolumeIndexLocalTypes(task, FX_FX_CharacteristicType.VolumeIndex)).ToList();

            serviceLocal.indexes = tasks
                .SelectMany(Indexes)
                .ToList();

            return serviceLocal;
        }

        private static List<stateTask640rType.serviceLocalType.indexesLocalType> Indexes(F_F_GosZadanie2016 task)
        {
            return new List<stateTask640rType.serviceLocalType.indexesLocalType>
                {
                    new stateTask640rType.serviceLocalType.indexesLocalType
                        {
                            regNum = task.RefService.Regrnumber,
                            contentIndex = GetContentIndex(task.RefService),
                            conditionIndex = GetConditionIndex(task.RefService)
                        }
                };
        }

        private static IList<string> GetContentIndex(D_Services_Service service)
        {
            var list = new List<string>();

            if (service.SvcCntsName1Val != null)
            {
                list.Add(service.SvcCntsName1Val);
            }

            if (service.SvcCntsName2Val != null)
            {
                list.Add(service.SvcCntsName2Val);
            }

            if (service.SvcCntsName3Val != null)
            {
                list.Add(service.SvcCntsName3Val);
            }

            return list;
        }

        private static IList<string> GetConditionIndex(D_Services_Service service)
        {
            var list = new List<string>();

            if (service.SvcTermsName1Val != null)
            {
                list.Add(service.SvcTermsName1Val);
            }

            if (service.SvcTermsName2Val != null)
            {
                list.Add(service.SvcTermsName2Val);
            }

            return list;
        }

        private static List<stateTask640rType.serviceLocalType.informingProcedureLocalType> InformingProcedureLocalTypes640R(
            F_F_GosZadanie2016 task)
        {
            return
                task.InformingProcedures
                    .Select(
                        procedure =>
                            new stateTask640rType.serviceLocalType.informingProcedureLocalType
                            {
                                content = procedure.Content,
                                method = procedure.Method,
                                rate = procedure.Rate
                            })
                    .ToList();
        }

        private static List<stateTask640rType.serviceLocalType.priceEnactmentLocalType> PriceEnactment640R(
            F_F_GosZadanie2016 task)
        {
            return
                task.Prices
                    .Select(
                        x =>
                            new stateTask640rType.serviceLocalType.priceEnactmentLocalType
                            {
                                type = x.VidNPAGZ,
                                author = new refNsiOgsSoftType { fullName = x.OrgUtvDoc },
                                date = x.DataNPAGZ.GetValueOrDefault(),
                                name = x.Name,
                                number = x.NumNPA
                            })
                    .ToList();
        }

        private static IEnumerable<stateTask640rType.serviceLocalType.renderEnactmentLocalType> RenderEnactment640R(F_F_GosZadanie2016 task)
        {
            return task.RenderOrders
                .Select(
                    x => new stateTask640rType.serviceLocalType.renderEnactmentLocalType
                        {
                            type = x.TypeNpa,
                            date = x.DateNpa.GetValueOrDefault(),
                            number = x.NumberNpa,
                            name = x.RenderEnact,
                            author = new refNsiOgsSoftType { fullName = x.Author }
                        });
        }

        private static List<serviceIndex640rType> ServiceIndexTypes640R(F_F_GosZadanie2016 task, int indexType)
        {
            return
                task.Indicators.Where(
                    x => x.RefIndicators.RefType.Code.Equals(indexType))
                    .Select(
                        x =>
                        {
                            var result = new serviceIndex640rType
                                {
                                    index = new serviceIndex640rType.indexLocalType
                                        {
                                            regNum = task.RefService.Regrnumber,
                                            name = x.RefIndicators.Name,
                                            unit = RefNsiOkeiType(x)
                                        },
                                    valueYear = ValueYear640R(x),
                                    valueActual = ValueActual640R(x)
                                };

                            if (x.Deviation.IsNotNullOrEmpty())
                            {
                                result.deviation = x.Deviation;
                            }

                            return result;
                        })
                    .ToList();
        }

        private static List<stateTask640rType.serviceLocalType.volumeIndexLocalType> VolumeIndexLocalTypes(F_F_GosZadanie2016 task, int indexType)
        {
            try
            {
                return
                    task.Indicators.Where(
                        x => x.RefIndicators.RefType.Code.Equals(indexType))
                        .Select(
                            x =>
                            {
                                var result = new stateTask640rType.serviceLocalType.volumeIndexLocalType
                                    {
                                        index = new serviceIndex640rType.indexLocalType
                                            {
                                                regNum = task.RefService.Regrnumber,
                                                name = x.RefIndicators.Name,
                                                unit = RefNsiOkeiType(x)
                                            },
                                        valueYear = ValueYear640R(x),
                                        valueActual = ValueActual640R(x)
                                    };

                                if (task.RefService.RefType.Code.Equals(FX_FX_ServiceType.CodeOfService) &&
                                    task.RefService.RefPay.Code.Equals(FX_FX_ServicePayType2.CodeOfPayable))
                                {
                                    result.averagePrice = AveragePrice(x.AveragePrices.FirstOrDefault());
                                }

                                if (x.Deviation.IsNotNullOrEmpty())
                                {
                                    result.deviation = x.Deviation;
                                }

                                return result;
                            })
                        .ToList();
            }
            catch (Exception e)
            {
                throw new InvalidDataContractException("Показателя объема услуги {0}: {1}".FormatWith(task.RefService.Regrnumber, e.Message));
            }
        }

        private static serviceIndex640rType.valueYearLocalType ValueYear640R(F_F_PNRZnach2016 pnrZnach)
        {
            if (pnrZnach.ReportingYear.IsNullOrEmpty() && pnrZnach.CurrentYear.IsNullOrEmpty() &&
                pnrZnach.ComingYear.IsNullOrEmpty() &&
                pnrZnach.FirstPlanYear.IsNullOrEmpty() && pnrZnach.SecondPlanYear.IsNullOrEmpty())
            {
                throw new InvalidDataContractException("Значения показателя на год должны быть заполнены");
            }

            var result = new serviceIndex640rType.valueYearLocalType();
            if (!pnrZnach.ReportingYear.IsNullOrEmpty())
            {
                result.reportYear = pnrZnach.ReportingYear;
            }

            if (!pnrZnach.CurrentYear.IsNullOrEmpty())
            {
                result.currentYear = pnrZnach.CurrentYear;
            }

            if (!pnrZnach.ComingYear.IsNullOrEmpty())
            {
                result.nextYear = pnrZnach.ComingYear;
            }

            if (!pnrZnach.FirstPlanYear.IsNullOrEmpty())
            {
                result.planFirstYear = pnrZnach.FirstPlanYear;
            }

            if (!pnrZnach.SecondPlanYear.IsNullOrEmpty())
            {
                result.planLastYear = pnrZnach.SecondPlanYear;
            }

            return result;
        }

        private static IList<serviceIndex640rType.valueActualLocalType> ValueActual640R(F_F_PNRZnach2016 pnrZnach)
        {
            if (pnrZnach.ActualValue.IsNullOrEmpty() && pnrZnach.Protklp.IsNullOrEmpty())
            {
                return null;
            }

            var result = new serviceIndex640rType.valueActualLocalType();
            if (!pnrZnach.ActualValue.Trim().IsNullOrEmpty())
            {
                result.actualValue = pnrZnach.ActualValue;
                result.reportGUID = new reportGUID { TypedValue = pnrZnach.RefReport.ReportGuid };
            }

            if (pnrZnach.Protklp.IsNotNullOrEmpty())
            {
                result.rejectReason = pnrZnach.Protklp;
                result.reject = pnrZnach.Reject;
            }

            if (pnrZnach.AveragePriceFact.HasValue)
            {
                result.averagePrice = pnrZnach.AveragePriceFact.Value;
            }

            return new List<serviceIndex640rType.valueActualLocalType> { result };
        }

        private static stateTask640rType.serviceLocalType.volumeIndexLocalType.averagePriceLocalType AveragePrice(F_F_AveragePrice averagePrice)
        {
            if (averagePrice == null)
            {
                throw new InvalidDataContractException("Не заполнена среднегодовая цена");
            }

            if (!averagePrice.ReportYearDec.HasValue
                && !averagePrice.CurrentYearDec.HasValue
                && !averagePrice.NextYearDec.HasValue
                && !averagePrice.PlanFirstYearDec.HasValue
                && !averagePrice.PlanLastYearDec.HasValue)
            {
                throw new InvalidDataContractException("Значения среднегодовой цены должны быть заполнены");
            }

            var result = new stateTask640rType.serviceLocalType.volumeIndexLocalType.averagePriceLocalType
                             {
                                 currentYear = averagePrice.CurrentYearDec.GetValueOrDefault(),
                                 nextYear = averagePrice.NextYearDec.GetValueOrDefault(),
                                 planFirstYear = averagePrice.PlanFirstYearDec.GetValueOrDefault(),
                                 planLastYear = averagePrice.PlanLastYearDec.GetValueOrDefault(),
                                 reportYear = averagePrice.ReportYearDec.GetValueOrDefault()
            };
            
            return result;
        }

        private static refNsiOkeiType RefNsiOkeiType(F_F_PNRZnach2016 x)
        {
            return
                new refNsiOkeiType
                {
                    code = x.RefIndicators.RefOKEI.Code.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'),
                    symbol = x.RefIndicators.RefOKEI.Name
                };
        }
    }
}
