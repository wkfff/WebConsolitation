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
    public static class ExportStateTaskService
    {
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            List<F_F_GosZadanie> tasks = header.StateTasks.ToList();
            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            int year = header.RefYearForm.ID;

            var stateTaskContent =
                new stateTaskType
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,

                        // todo add versionNumber 
                        reportYear = year - 2,
                        financialYear = year - 1,
                        nextFinancialYear = year,
                        planFirstYear = year + 1,
                        planLastYear = year + 2,
                        service = tasks.Select(ServiceLocal).ToList(),
                        document = ExportServiceHelper.Documents(header.Documents.ToList())
                    };
            
            // грязный хак http://medved.krista.ru:81/redmine/issues/5716
            stateTaskContent.service.Where(type => type.type.Equals("W")).Each((type, i) => type.ordinalNumber = i + 1);
            stateTaskContent.service.Where(type => type.type.Equals("S")).Each((type, i) => type.ordinalNumber = i + 1);

            return ExportServiceHelper.Serialize(
                new stateTask
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new stateTask.bodyLocalType
                                   {
                                       position = stateTaskContent
                                   }
                    }.Save);
        }

        private static stateTaskType.serviceLocalType ServiceLocal(F_F_GosZadanie task)
        {
            bool isWork = task.RefVedPch.RefTipY.Name.Equals("Работа");
            List<F_F_RequestAccount> requirements = task.RefParametr.StateTasksRequestAccounts.ToList();
            return new stateTaskType.serviceLocalType
                    {
                        name = task.RefVedPch.Name,
                        type = isWork ? "W" : "S",
                        ordinalNumber = task.RazdelN,
                        category =
                            task.RefVedPch.Customers
                            .Select(x => new refNsiCustomerCategoryType { name = x.RefCPotr.Name })
                            .ToList(),
                        volumeIndex = ServiceIndexTypes(task, FX_FX_CharacteristicType.VolumeIndex),
                        qualityIndex = task
                            .Unless(arg => isWork)
                            .With(x => ServiceIndexTypes(x, FX_FX_CharacteristicType.QualityIndex)),

                        renderEnactment = task
                            .Unless(arg => isWork)
                            .With(zadanie => zadanie.RenderOrders.Select(order => order.TypeNpa.IsNullOrEmpty() ?
                                                                                      order.RenderEnact
                                                                                      : "{0} от {1} №{2} \"{3} \""
                                                                                      .FormatWith(
                                                                                          order.TypeNpa,
                                                                                          order.DateNpa.HasValue ? order.DateNpa.Value.ToShortDateString() : string.Empty,
                                                                                          order.NumberNpa,
                                                    order.RenderEnact)).ToList()),

                        payment = task
                            .Unless(x => isWork || x.RefVedPch.RefPl.Name.Equals("Бесплатная"))
                            .With(
                                arg => new stateTaskType.serviceLocalType.paymentLocalType
                                           {
                                               averagePrice = task.CenaEd.GetValueOrDefault(),
                                               priceEnactment = PriceEnactment(task),
                                               limitPrice = LimitPrice(task)
                                           }),
                        informingProcedure = task
                            .Unless(arg => isWork)
                            .With(InformingProcedureLocalTypes),
                        supervisionProcedure = task.RefParametr.StateTasksSupervisionProcedures
                            .Select(
                                control => new stateTaskType.serviceLocalType.supervisionProcedureLocalType
                                               {
                                                   rate = control.Rate,
                                                   form = control.Form,
                                                   supervisor = new refNsiOgsSoftType { fullName = control.Supervisor }
                                               })
                            .ToList(),
                        reportRequirements = task.RefParametr
                            .With(
                                doc => new stateTaskType.serviceLocalType.reportRequirementsLocalType
                                           {
                                               earlyTermination = doc.StateTasksTerminations
                                                   .Select(termination => termination.EarlyTerminat)
                                                   .ToList(),
                                               deliveryTerm = requirements.Select(account => account.DeliveryTerm)
                                                   .ToList(),
                                               otherInfo = requirements.Select(account => account.OtherInfo).ToList(),
                                               otherRequirement = requirements.Select(account => account.OtherRequest)
                                                   .ToList()
                                           })
                    };
        }

        private static List<stateTaskType.serviceLocalType.informingProcedureLocalType> InformingProcedureLocalTypes(
            F_F_GosZadanie task)
        {
            return
                task.InformingProcedures
                    .Select(
                        procedure => new stateTaskType.serviceLocalType.informingProcedureLocalType
                                         {
                                             content = procedure.Content,
                                             method = procedure.Method,
                                             rate = procedure.Rate
                                         })
                    .ToList();
        }

        private static stateTaskType.serviceLocalType.paymentLocalType.priceEnactmentLocalType PriceEnactment(
            F_F_GosZadanie task)
        {
            return
                task.Prices.FirstOrDefault()
                    .With(
                        x => new stateTaskType.serviceLocalType.paymentLocalType.priceEnactmentLocalType
                                 {
                                     type = x.VidNPAGZ,
                                     author = new refNsiOgsSoftType { fullName = x.OrgUtvDoc },
                                     date = x.DataNPAGZ.GetValueOrDefault(),
                                     name = x.Name,
                                     number = x.NumNPA
                                 });
        }

        private static List<stateTaskType.serviceLocalType.paymentLocalType.limitPriceLocalType> LimitPrice(
            F_F_GosZadanie task)
        {
            return
                task.Limits
                    .Select(
                        price => new stateTaskType.serviceLocalType.paymentLocalType.limitPriceLocalType
                                     {
                                         name = price.Name,
                                         price = price.Price
                                     })
                    .ToList();
        }

        private static List<serviceIndexType> ServiceIndexTypes(F_F_GosZadanie task, int indexType)
        {
            return
                task.Indicators.Where(x => x.RefIndicators.RefCharacteristicType.Code.Equals(indexType))
                    .Select(
                        x => new serviceIndexType
                                 {
                                     index = new serviceIndexType.indexLocalType
                                                 {
                                                     name = x.RefIndicators.Name,
                                                     unit = RefNsiOkeiType(x.RefIndicators)
                                                 },
                                     valueYear = ValueYear(x),
                                     valueActual = ValueActual(x)
                                 })
                    .ToList();
        }

        private static serviceIndexType.valueYearLocalType ValueYear(F_F_PNRZnach pnrZnach)
        {
            if (pnrZnach.ReportingYear.IsNullOrEmpty() && pnrZnach.CurrentYear.IsNullOrEmpty() &&
                pnrZnach.ComingYear.IsNullOrEmpty() &&
                pnrZnach.FirstPlanYear.IsNullOrEmpty() && pnrZnach.SecondPlanYear.IsNullOrEmpty())
            {
                throw new InvalidDataContractException("Значения показателя на год должны быть заполнены");
            }

            var result = new serviceIndexType.valueYearLocalType();
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

        private static serviceIndexType.valueActualLocalType ValueActual(F_F_PNRZnach pnrZnach)
        {
            if (pnrZnach.ActualValue.IsNullOrEmpty() && pnrZnach.Protklp.IsNullOrEmpty())
            {
                return null;
            }

            var result = new serviceIndexType.valueActualLocalType();
            if (!pnrZnach.ActualValue.IsNullOrEmpty())
            {
                result.actualValue = pnrZnach.ActualValue;
            }

            if (pnrZnach.Protklp.IsNotNullOrEmpty())
            {
                result.rejectReason = pnrZnach.Protklp;
            }

            return result;
        }

        private static refNsiOkeiType RefNsiOkeiType(D_Services_Indicators x)
        {
            return new refNsiOkeiType
                       {
                           code = x.RefOKEI.Code.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'),
                           symbol = x.RefOKEI.Symbol
                       };
        }
    }
}
