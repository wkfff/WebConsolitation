using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportActivityResultService
    {
        private static readonly PropertyUseViewModel PropertyUseModel = new PropertyUseViewModel();
        private static readonly FinNFinAssetsViewModel FinNFinAssetsModel = new FinNFinAssetsViewModel();

        private static int year;

        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            List<F_Doc_Docum> documents = header.Documents.Where(docum => docum.RefTypeDoc.Code.Equals("D")).ToList();
            List<F_ResultWork_UseProperty> useProperties = header.ActivityResultUse.ToList();
            List<F_ResultWork_FinNFinAssets> finAssetses = header.ActivityResultFinNFin.ToList();
            List<F_ResultWork_CashReceipts> cashReceiptses = header.ActivityResultCashReceipts.ToList();
            List<F_ResultWork_CashPay> cashPays = header.ActivityResultCashPay.ToList();
            year = header.RefYearForm.ID;

            var position =
                new activityResultType
                    {
                        positionId = Guid.NewGuid().ToString(), 
                        changeDate = DateTime.Now, 
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr), 
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null, 
                        reportYear = year, 
                        document = ExportServiceHelper.Documents(documents), 
                        staff = header.ActivityResultStaff.First().With(
                            staff => new activityResultType.staffLocalType
                                {
                                    beginYear = staff.BeginYear, 
                                    endYear = staff.EndYear, 
                                    averageSalary = staff.AvgSalary
                                }), 
                        assetsUse = AssetsUse(useProperties), 
                        result = new activityResultType.resultLocalType
                            {
                                nonfinancialAssetsChange =
                                    new activityResultType.resultLocalType.nonfinancialAssetsChangeLocalType
                                        {
                                            highValuePersonalAssets = ChangeIndexType(
                                                finAssetses, 
                                                () => FinNFinAssetsModel.ParticularlyValuableProperty), 
                                            realAssets = ChangeIndexType(
                                                finAssetses,
                                                () => FinNFinAssetsModel.ImmovableProperty), 
                                            total = ChangeIndexType(
                                                finAssetses,
                                                () => FinNFinAssetsModel.InfAboutCarryingValueTotal)
                                        }, 
                                financialAssetsChange =
                                    new activityResultType.resultLocalType.financialAssetsChangeLocalType
                                        {
                                            debit =
                                                new activityResultType.resultLocalType.financialAssetsChangeLocalType.debitLocalType
                                                    {
                                                        expense = ChangeIndexType(
                                                            finAssetses,
                                                            () => FinNFinAssetsModel.Expenditure), 
                                                        income = ChangeIndexType(
                                                            finAssetses,
                                                            () => FinNFinAssetsModel.Income), 
                                                        total = ChangeIndexType(
                                                            finAssetses,
                                                            () => FinNFinAssetsModel.ChangingArrearsTotal)
                                                    }, 
                                            kredit =
                                                new activityResultType.resultLocalType.financialAssetsChangeLocalType.kreditLocalType
                                                    {
                                                        expired = ChangeIndexType(
                                                            finAssetses,
                                                            () => FinNFinAssetsModel.OverduePayables), 
                                                        total = ChangeIndexType(
                                                            finAssetses, 
                                                            () => FinNFinAssetsModel.IncreaseInAccountsPayableTotal)
                                                    }
                                        }, 
                                service = year < 2016 
                                            ? header.ActivityResultServices.Select(ServiceLocalType).ToList() 
                                            : header.ActivityResultServices2016.Select(ServiceLocalType2016).ToList(), 
                                cashReceipts = cashReceiptses.First().With(
                                    receipts =>
                                    new activityResultType.resultLocalType.cashReceiptsLocalType
                                        {
                                            actionGrant = receipts.ActionGrant, 
                                            budgetaryFunds = receipts.BudgetFunds, 
                                            paidServices = receipts.PaidServices, 
                                            stateTaskGrant = receipts.TaskGrant, 
                                            total = receipts.Total
                                        })
                            }
                            .Do(
                                resultLocalType =>
                                {
                                    // для КУ с 2016 года не заполняем раздел cashPayments
                                    if (!(target.RefTipYc.ID == FX_Org_TipYch.GovernmentID && year > 2015))
                                    {
                                        resultLocalType.cashPayments = cashPays.Select(pay => CashPaymentsLocalType(target, pay)).ToList();
                                    }
                                    
                                    var damagesReparation = FinNFinAssets(finAssetses, () => FinNFinAssetsModel.AmountOfDamageCompensation);
                                    if (damagesReparation.Value.HasValue)
                                    {
                                        resultLocalType.damagesReparation = damagesReparation.Value;
                                    }
                                })
                    };

            // TODO: заплатка для корректного экспорта аналогично http://medved.krista.ru:81/redmine/issues/5716
            position.result.service.Where(type => type.type.Equals("W")).Each((type, i) => type.ordinalNumber = i + 1);
            position.result.service.Where(type => type.type.Equals("S")).Each((type, i) => type.ordinalNumber = i + 1);

            return ExportServiceHelper.Serialize(
                new activityResult
                    {
                        header = ExportServiceHelper.HeaderType(), 
                        body = new activityResult.bodyLocalType { position = position }
                    }
                    
                    .Save);
        }

        private static activityResultType.resultLocalType.serviceLocalType ServiceLocalType2016(F_F_ShowService2016 service)
        {
            var result = new activityResultType.resultLocalType.serviceLocalType
            {
                name = service.RefService.NameName,
                type =
                        service.RefService.RefType.ID
                        == FX_FX_ServiceType.IdOfWork
                            ? "W"
                            : "S"
            }
                .Do(
                    type => service.Reaction
                                .If(s => s.IsNotNullOrEmpty())
                                .Do(s => type.reaction = s));
            if (service.Customers.HasValue)
            {
                result.customers = service.Customers.Value;
            }

            if (service.Complaints.HasValue)
            {
                result.complaints = service.Complaints.Value;
            }

            return result;
        }

        private static activityResultType.resultLocalType.serviceLocalType ServiceLocalType(F_ResultWork_ShowService service)
        {
            var result = new activityResultType.resultLocalType.serviceLocalType
                {
                    name = service.RefVedPch.Name, 
                    type =
                        service.RefVedPch.RefTipY.ID
                        == D_Services_TipY.FX_FX_WORK
                            ? "W"
                            : "S"
                }
                
                .Do(
                    type => service.Reaction
                                .If(s => s.IsNotNullOrEmpty())
                                .Do(s => type.reaction = s));
            if (service.Customers.HasValue)
            {
                result.customers = service.Customers.Value;
            }

            if (service.Complaints.HasValue)
            {
                result.complaints = service.Complaints.Value;
            }

            return result;
        }

        private static activityResultType.resultLocalType.cashPaymentsLocalType CashPaymentsLocalType(
            D_Org_Structure target, F_ResultWork_CashPay pay)
        {
            return
                new activityResultType.resultLocalType.cashPaymentsLocalType
                    {
                        government = target
                            .If(structure => structure.RefTipYc.ID == FX_Org_TipYch.GovernmentID)
                            .With(
                                _ => new activityResultType.resultLocalType.cashPaymentsLocalType.governmentLocalType
                                    {
                                        name = pay.RefKosgy.Name, 
                                        payment = pay.Payment, 
                                        kbk = new refNsiKbkType
                                            {
                                                code = string.Format(
                                                    "{0,3}{1,4}{2,7}{3,3}{4,3}", 
                                                    target.RefOrgGRBS.Code.PadLeft(3, '0'), 
                                                    pay.RefRazdPodr.Code.PadLeft(4, '0'), 
                                                    pay.CelStatya.PadLeft(7, '0'), 
                                                    pay.RefVidRash.Code.PadLeft(3, '0'), 
                                                    pay.RefKosgy.Code.PadLeft(3, '0'))
                                            }
                                    }), 
                        autonomous = target
                            .If(structure => structure.RefTipYc.ID == FX_Org_TipYch.AutonomousID)
                            .With(
                                _ => new institutionCashPaymentType
                                    {
                                        name = pay.RefKosgy.Name, 
                                        payment = pay.Payment
                                    }), 
                        budgetary = target
                            .If(structure => structure.RefTipYc.ID == FX_Org_TipYch.BudgetaryID)
                            .With(
                                _ => new activityResultType.resultLocalType.cashPaymentsLocalType.budgetaryLocalType
                                    {
                                        name = pay.RefKosgy.Name, 
                                        payment = pay.Payment, 
                                        kosgu = new refNsiKosguType { code = pay.RefKosgy.Code }
                                    }).Do(budgetaryLocalType =>
                                            {
                                                if (year > 2015)
                                                {
                                                    budgetaryLocalType.kbk = new refNsiKbkType
                                                    {
                                                        code = string.Format(
                                                                            "{0,3}{1,4}{2,10}{3,3}",
                                                                            target.RefOrgGRBS.Code.PadLeft(3, '0'),
                                                                            pay.RefRazdPodr.Code.PadLeft(4, '0'),
                                                                            pay.CelStatya.PadLeft(10, '0'),
                                                                            pay.RefVidRash.Code.PadLeft(3, '0'))
                                                    };
                                                }
                                            })
                };
        }

        private static changeIndexType ChangeIndexType<T>(
            IEnumerable<F_ResultWork_FinNFinAssets> finAssetses,
            Expression<Func<T>> expr)
        {
            return FinNFinAssets(finAssetses, expr)
                .With(
                    assets => new changeIndexType
                        {
                            type = IndexType(assets)
                        }

                                  .Do(
                                      changeIndexType =>
                                      {
                                          if (assets.Value.HasValue)
                                          {
                                              changeIndexType.quantity = assets.Value;
                                          }
                                      }));
        }

        private static string IndexType(F_ResultWork_FinNFinAssets assets)
        {
            switch (assets.RefTypeIxm.ID)
            {
                case 1:
                    return "I";
                case 2:
                    return "D";
                default:
                    return "U";
            }
        }

        private static F_ResultWork_FinNFinAssets FinNFinAssets<T>(
            IEnumerable<F_ResultWork_FinNFinAssets> finAssetses,
            Expression<Func<T>> expr)
        {
            return
                finAssetses.First(
                    assets =>
                    assets.RefStateValue.ID == FinNFinAssetsModel.DescriptionIdOf(expr));
        }

        private static activityResultType.assetsUseLocalType AssetsUse(List<F_ResultWork_UseProperty> activityOfUse)
        {
            return
                new activityResultType.assetsUseLocalType
                    {
                        realAssetsArea =
                            new activityResultType.assetsUseLocalType.realAssetsAreaLocalType
                                {
                                    assetsFunds = PropertyUse(
                                        activityOfUse, 
                                        () => PropertyUseModel.AmountOfFundsFromDisposalID), 
                                    realAssetsAreaSummary =
                                        new activityResultType.assetsUseLocalType.realAssetsAreaLocalType.realAssetsAreaSummaryLocalType
                                            {
                                                gratisArea = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.DonatedID), 
                                                leaseArea = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.GivenOnRentID), 
                                                total = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.AreaOfRealEstateTotalID)
                                            }
                                }, 
                        bookValueAssets =
                            new activityResultType.assetsUseLocalType.bookValueAssetsLocalType
                                {
                                    personalAssets =
                                        new activityResultType.assetsUseLocalType.bookValueAssetsLocalType.personalAssetsLocalType
                                            {
                                                gratisPersonalAssets = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.MovablePropertyDonatedID), 
                                                leasePersonalAssets = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.MovablePropertyGivenOnRentID), 
                                                total = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.CarryingAmountOfMovablePropertyTotalID)
                                            }, 
                                    realAssets =
                                        new activityResultType.assetsUseLocalType.bookValueAssetsLocalType.realAssetsLocalType
                                            {
                                                gratisRealAssets = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.ImmovablePropertyDonatedID), 
                                                leaseRealAssets = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.ImmovablePropertyGivenOnRentID), 
                                                total = PropertyUse(
                                                    activityOfUse,
                                                    () => PropertyUseModel.BookValueOfRealEstateTotalID)
                                            }
                                }
                    };
        }

        private static changeRangeType PropertyUse<T>(
            IEnumerable<F_ResultWork_UseProperty> activityOfUse,
            Expression<Func<T>> expr)
        {
            return
                activityOfUse
                    .FirstOrDefault(property => property.RefStateValue.ID == PropertyUseModel.DescriptionIdOf(expr))
                    .With(
                        property => new changeRangeType
                            {
                                beginYear = property.BeginYear, 
                                endYear = property.EndYear
                            });
        }
    }
}
