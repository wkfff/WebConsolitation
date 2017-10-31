using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using bus.gov.ru;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;
using Xml.Schema.Linq;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// закачка stateTask
    /// </summary>
    public partial class BusGovRuPumpModule
    {
        private static F_F_PNRZnach FfPnrZnach(F_F_GosZadanie zadanie, serviceIndexType type, int indexType)
        {
            var okei = Convert.ToInt32(type.index.unit.code);
            
            return
                new F_F_PNRZnach
                {
                    RefFactGZ = zadanie,

                    ReportingYear = type.valueYear.reportYear ?? string.Empty,
                    CurrentYear = type.valueYear.currentYear ?? string.Empty,
                    ComingYear = type.valueYear.nextYear ?? string.Empty,
                    FirstPlanYear = type.valueYear.planFirstYear ?? string.Empty,
                    SecondPlanYear = type.valueYear.planLastYear ?? string.Empty,

                    Protklp = type.valueActual.With(localType => localType.rejectReason) ?? string.Empty,
                    ActualValue = type.valueActual.With(localType => localType.actualValue) ?? string.Empty,

                    RefIndicators = Resolver.Get<ILinqRepository<D_Services_Indicators>>().FindAll().FirstOrDefault(x => x.Name.Equals(type.index.name)),
                };
        }

        private bool CheckValidSource(XTypedElement element, string messageCaption = "Документ не соответствует формату")
        {
            string detail;
            if (element.Validate(Resolver.Get<XmlSchemaSet>(), out detail))
            {
                return true;
            }

            string message = string.Format("'{0}', {1}, {2}", messageCaption, detail, element.Untyped);

            WriteToTrace(message, TraceMessageKind.Warning);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message.Substring(0, 4000));

            return false;
        }

        private void ProcessStateTaskPumpFile(FileInfo fileInfo)
        {
            stateTaskType pumpData = stateTask.Load(fileInfo.OpenText()).body.position;

            CheckValidSource(pumpData);

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            if (!CheckDocumentTarget(targetOrg))
            {
                return;
            }

            var headerRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();

            headerRepository.DbContext.BeginTransaction();

            var header =
                new F_F_ParameterDoc
                    {
                        PlanThreeYear = false,
                        RefPartDoc = Resolver.Get<ILinqRepository<FX_FX_PartDoc>>()
                            .Load(FX_FX_PartDoc.StateTaskDocTypeID),
                        RefSost = Resolver.Get<ILinqRepository<FX_Org_SostD>>()
                            .Load(FX_Org_SostD.ExportedStateID),
                        RefUchr = _orgStructuresByRegnumCache[targetOrg.regNum],
                        RefYearForm = Resolver.Get<ILinqRepository<FX_Fin_YearForm>>()
                            .Load(Convert.ToInt32(pumpData.nextFinancialYear)),
                    }
                    .Do(SetDocumentSourceId);
            
            foreach (var servicePump in pumpData.service)
            {
                header.StateTasks.Add(FfGosZadanie(servicePump, header));
                header.StateTasksTerminations.AddRange(
                    servicePump.reportRequirements.With(type => type.earlyTermination.Select(
                        s => new F_F_BaseTermination
                                 {
                                     RefFactGZ = header,
                                     EarlyTerminat = s,
                                 }).ToArray()).Return(terminations => terminations, new F_F_BaseTermination[0]));
                header.StateTasksSupervisionProcedures.AddRange(
                    servicePump.supervisionProcedure
                        .Where(type => CheckValidSource(type))
                        .Select(
                            type => new F_F_OrderControl
                                        {
                                            RefFactGZ = header,
                                            Rate = type.rate,
                                            Form = type.form,
                                            Supervisor = type.supervisor.fullName,
                                        }).ToArray());

                if (servicePump.reportRequirements != null)
                {
                    var requestAccounts = new Dictionary<int, F_F_RequestAccount>();
                    servicePump.reportRequirements.deliveryTerm.Select((s, i) => new { i, deliveryTerm = s }).Each(
                        obj =>
                        {
                                var key = obj.i;
                                if (requestAccounts.ContainsKey(key))
                                {
                                    requestAccounts[key].DeliveryTerm = obj.deliveryTerm;
                                }
                                else
                                {
                                    requestAccounts.Add(
                                        key,
                                        new F_F_RequestAccount { RefFactGZ = header, DeliveryTerm = obj.deliveryTerm, });
                                }
                        });
                    servicePump.reportRequirements.otherInfo.Select((s, i) => new { i, otherInfo = s }).Each(
                        obj =>
                        {
                                var key = obj.i;
                                if (requestAccounts.ContainsKey(key))
                                {
                                    requestAccounts[key].OtherInfo = obj.otherInfo;
                                }
                                else
                                {
                                    requestAccounts.Add(
                                        key,
                                        new F_F_RequestAccount { RefFactGZ = header, OtherInfo = obj.otherInfo, });
                                }
                        });
                    servicePump.reportRequirements.otherRequirement.Select((s, i) => new { i, otherRequirement = s }).Each(
                            obj =>
                            {
                                    var key = obj.i;
                                    if (requestAccounts.ContainsKey(key))
                                    {
                                        requestAccounts[key].OtherRequest = obj.otherRequirement;
                                    }
                                    else
                                    {
                                        requestAccounts.Add(
                                            key,
                                            new F_F_RequestAccount
                                                {
                                                    RefFactGZ = header, OtherRequest = obj.otherRequirement, 
                                                });
                                    }
                            });
                    header.StateTasksRequestAccounts.AddRange(requestAccounts.Values.ToArray());
                }
            }

            header.StateTasksRequestAccounts = header.StateTasksRequestAccounts.Distinct().ToList();
            
            ProcessDocumentsHeader(
                header,
                pumpData.document,
                type => type.With(x => _documentTypesCache.First(doc => doc.Code.Equals("C"))));

            SetDocumentSourceId(header);

            headerRepository.Save(header);
            headerRepository.DbContext.CommitChanges();
            headerRepository.DbContext.CommitTransaction();
        }

        private F_F_GosZadanie FfGosZadanie(stateTaskType.serviceLocalType servicePump, F_F_ParameterDoc header)
        {
            return new F_F_GosZadanie
                       {
                           RefVedPch = RefVedPch(servicePump, header),
                           CenaEd = servicePump.payment.Return(payment => payment.averagePrice, (decimal?)null),
                           RazdelN = servicePump.ordinalNumber,
                           RefParametr = header,
                       }
                .Do(
                    zadanie =>
                        {
                            zadanie.InformingProcedures = servicePump.informingProcedure
                                .Where(type => CheckValidSource(type))
                                .Select(
                                    localType => new F_F_InfoProcedure
                                                     {
                                                         RefFactGZ = zadanie,
                                                         Content = localType.content,
                                                         Method = localType.method,
                                                         Rate = localType.rate,
                                                     })
                                .ToList();
                            zadanie.RenderOrders = servicePump.renderEnactment.Select(
                                localType => new F_F_NPARenderOrder
                                                 {
                                                     RefFactGZ = zadanie,
                                                     RenderEnact = localType,
                                                 })
                                .ToList();
                            zadanie.Prices = servicePump.payment.With(type => type.priceEnactment)
                                .If(type => CheckValidSource(type))
                                .With(
                                    type => new F_F_NPACena
                                                {
                                                    RefGZPr = zadanie,
                                                    Name = type.name,
                                                    VidNPAGZ = type.type,
                                                    OrgUtvDoc = type.author.fullName,
                                                    DataNPAGZ = type.date,
                                                    NumNPA = type.number,
                                                })
                                .With(cena => new List<F_F_NPACena> { cena });
                            zadanie.Limits = servicePump.payment.With(
                                type => type.limitPrice
                                            .Where(localType => CheckValidSource(localType))
                                            .Select(
                                                localType => new F_F_LimitPrice
                                                                 {
                                                                     RefFactGZ = zadanie,
                                                                     Name = localType.name,
                                                                     Price = localType.price,
                                                                 })
                                            .ToList());
                            zadanie.Indicators.AddRange(
                                servicePump.qualityIndex
                                    .Where(type => CheckValidSource(type))
                                    .Where(type => type.index.unit != null)
                                    .Select(type => FfPnrZnach(zadanie, type, FX_FX_CharacteristicType.QualityIndex))
                                    .ToArray());
                            zadanie.Indicators.AddRange(
                                servicePump.volumeIndex
                                    .Where(type => CheckValidSource(type))
                                    .Where(type => type.index.unit != null)
                                    .Select(type => FfPnrZnach(zadanie, type, FX_FX_CharacteristicType.VolumeIndex))
                                    .ToArray());
                        });
        }

        private D_Services_VedPer RefVedPch(stateTaskType.serviceLocalType servicePump, F_F_ParameterDoc header)
        {
            var isWork = servicePump.type.Equals("W");
            var servicesRepository = Resolver.Get<ILinqRepository<D_Services_VedPer>>();
            var service =
                servicePump.code.With(
                    s =>
                    servicesRepository.FindAll().FirstOrDefault(
                        per => per.Code.Equals(s) && per.Name.Equals(servicePump.name))) ??
                new D_Services_VedPer
                    {
                        // полноценной услуги не получится
                        Code = servicePump.code.Return(
                                                s => s,
                                                string.Format("{0}.86n.krista.ru", Guid.NewGuid().ToString().Substring(0, 6))),
                        Name = servicePump.name,
                        RefTipY = Resolver.Get<ILinqRepository<D_Services_TipY>>()
                            .Load(isWork ? D_Services_TipY.FX_FX_WORK : D_Services_TipY.FX_FX_SERVICE),
                        BusinessStatus = "801",
                        RefSferaD = Resolver.Get<ILinqRepository<D_Services_SferaD>>()
                            .Load(D_Services_SferaD.FX_FX_UNDEFINED_ID),
                        RefOrgPPO = header.RefUchr.RefOrgPPO,
                        RefGRBSs = header.RefUchr.RefOrgGRBS,
                    }.Do(
                        per => per.Customers = servicePump.category.Select(
                            type => new F_F_PotrYs
                                        {
                                            RefVedPP = per,
                                            RefCPotr =
                                                Resolver.Get<ILinqRepository<D_Services_CPotr>>().FindAll()
                                                    .FirstOrDefault(potr => potr.Name.Equals(type.name)) ??
                                                new D_Services_CPotr
                                                    {
                                                        Name = type.name,
                                                        Code = type.code ?? string.Empty,
                                                    },
                                        }).ToList());

            service.RefPl = Resolver.Get<ILinqRepository<D_Services_Platnost>>().Load(
                isWork
                    ? D_Services_Platnost.Free
                    : servicePump.payment.Return(type => D_Services_Platnost.Paid, D_Services_Platnost.PartiallyPaid));

            return service;
        }
    }
}
