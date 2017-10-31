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
    // TODO прикрутить этот класс к закачке ProcessBudgetaryCircumstancesPumpFile
    // TODO что бы не дублировать и использовать одно поведение
    
    /// <summary>
    /// Закачка ПФХД
    /// </summary>
    public class ActionGrantPump
    {
        private readonly CommonPump commonPump;

        private readonly ILinqRepository<D_Fin_OtherGant> otherGant;
     
        public ActionGrantPump()
        {
            otherGant = Resolver.Get<ILinqRepository<D_Fin_OtherGant>>();
            commonPump = CommonPump.GetCommonPump;

            State = FX_Org_SostD.CreatedStateID;
            PlanThreeYear = false;
        }

        public int State { get; set; }

        public bool PlanThreeYear { get; set; }

        /// <summary>
        /// Выполняет закачку из файла
        /// </summary>
        /// <param name="pumpData">
        /// The pump Data.
        /// </param>
        public void PumpFile(actionGrantType pumpData)
        {
            string result;

            // Проверка по схеме
            if (!pumpData.Validate(Resolver.Get<XmlSchemaSet>(), out result))
            {
                throw new Exception(result);
            }

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            // Проверка учреждения
            if (!commonPump.CheckInstTarget(targetOrg))
            {
                /*return;*/
                throw new Exception(
                    "Учреждение regnum={0}{1} не найдено, требуется обновление nsiOgs"
                        .FormatWith(
                            targetOrg.regNum,
                            targetOrg.fullName.Return(
                                s => string.Format("({0})", s),
                                string.Empty)));
            }

            var year = Convert.ToInt32(pumpData.financialYear);
            var uchr = commonPump.OrgStructuresByRegnumCache[targetOrg.regNum];

            // Проверка предыдущих документов
            commonPump.CheckDocs(uchr.ID, FX_FX_PartDoc.PfhdDocTypeID, year, new[] { FX_Org_SostD.ExportedStateID, State });

            commonPump.HeaderRepository.DbContext.BeginTransaction();

            var header = commonPump.HeaderRepository.FindAll().SingleOrDefault(
                x => x.RefSost.ID == State
                     && x.RefPartDoc.ID == FX_FX_PartDoc.PfhdDocTypeID
                     && x.RefYearForm.ID == year
                     && x.PlanThreeYear == PlanThreeYear
                     && x.RefUchr.ID == uchr.ID)
                         ?? new F_F_ParameterDoc
                             {
                                 PlanThreeYear = PlanThreeYear,
                                 RefPartDoc = commonPump.TypeDocCache.FindOne(FX_FX_PartDoc.PfhdDocTypeID),
                                 RefSost = commonPump.StateDocCache.FindOne(State),
                                 RefUchr = uchr,
                                 RefYearForm = commonPump.YearFormCache.FindOne(year),
                                 OpeningDate = DateTime.Now
                             };

            pumpData.capitalConstructionFunds.Each(
                x => header.CapitalConstructionFundses.Add(
                    new F_Fin_CapFunds
                        {
                            RefParametr = header,
                            Name = x.name,
                            funds = x.funds
                        }));

            pumpData.realAssetsFunds.Each(
                x => header.RealAssFundses.Add(
                    new F_Fin_realAssFunds
                        {
                            RefParametr = header,
                            Name = x.name,
                            funds = x.funds
                        }));

            pumpData.otherGrantFunds.Each(
                x =>
                {
                    var otherGrant = otherGant.FindAll().SingleOrDefault(o => o.Code.Equals(x.code.Replace(".", string.Empty)));
                    var funds = x.funds;
                    var kosgu = x.kosgu.code;

                    if (otherGrant == null)
                    {
                        commonPump.DataPumpProtocol.WriteProtocolEvent(
                                DataPumpEventKind.dpeWarning, 
                                "Не удалось закачать субсидию. Cубсидия {0} \"{1}\" отсутствует в справочнике. По данному вопросу необходимо обратиться к куратору.".FormatWith(x.code, x.name.Substring(0, 50)));
                    }
                    else if (otherGrant.CloseDate.HasValue && !(otherGrant.OpenDate.Value.Year <= year && year <= otherGrant.CloseDate.Value.Year))
                    {
                        commonPump.DataPumpProtocol.WriteProtocolEvent(
                            DataPumpEventKind.dpeWarning,
                            "Не удалось закачать субсидию. Cубсидия {0} \"{1}\" не относится к актуальным записям. По данному вопросу необходимо обратиться к куратору.".FormatWith(otherGrant.Code, otherGrant.Name.Substring(0, 50)));
                    }
                    else if (!header.OtherGrantFundses.Any(og => og.RefOtherGrant.Code.Equals(otherGrant.Code) && og.KOSGY.Equals(kosgu) && og.funds == funds))
                    {
                        header.OtherGrantFundses.Add(
                            new F_Fin_othGrantFunds
                            {
                                RefParametr = header,
                                RefOtherGrant = otherGrant,
                                funds = funds,
                                KOSGY = kosgu
                            });        
                    }
                });

            commonPump.ProcessDocumentsHeader(
                                                header,
                                                pumpData.document,
                                                type => type.With(x => commonPump.DocumentTypesCache.FindAll().First(doc => doc.Code.Equals("A"))));

            commonPump.HeaderRepository.Save(header);
            commonPump.HeaderRepository.DbContext.CommitChanges();
            commonPump.HeaderRepository.DbContext.CommitTransaction();
        }
    }
}
