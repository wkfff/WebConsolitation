using System;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.ResultsOfActivity
{
    public class ResultsOfActivityService : NewRestService, IResultsOfActivityService
    {
        private readonly FinNFinAssetsViewModel finNFinAssetsModel = new FinNFinAssetsViewModel();

        private readonly PropertyUseViewModel propertyUseModel = new PropertyUseViewModel();
        #region IResultsOfActivityService Members

        public FinNFinAssetsViewModel GetFinNFinAssetsItem(int parentId)
        {
            try
            {
                var data = from p in GetItems<F_ResultWork_FinNFinAssets>()
                           where p.RefParametr.ID == parentId
                           select p;

                var result = new FinNFinAssetsViewModel { ID = 0 };

                if (data.Count() != 0)
                {
                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.AmountOfDamageCompensation)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.AmountOfDamageCompensation));

                        result.AmountOfDamageCompensationID = row.ID;
                        result.AmountOfDamageCompensation = row.Value;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ChangingArrearsTotal)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ChangingArrearsTotal));

                        result.ChangingArrearsTotalID = row.ID;
                        result.ChangingArrearsTotal = row.Value;

                        result.ChangingArrearsRefTypeIxm = row.RefTypeIxm.ID;
                        result.ChangingArrearsRefTypeIxmName = row.RefTypeIxm.Name;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.Income)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.Income));

                        result.IncomeID = row.ID;
                        result.Income = row.Value;
                        result.IncomeRefTypeIxm = row.RefTypeIxm.ID;
                        result.IncomeRefTypeIxmName = row.RefTypeIxm.Name;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.Expenditure)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.Expenditure));

                        result.ExpenditureID = row.ID;
                        result.Expenditure = row.Value;
                        result.ExpenditureRefTypeIxm = row.RefTypeIxm.ID;
                        result.ExpenditureRefTypeIxmName = row.RefTypeIxm.Name;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal));

                        result.InfAboutCarryingValueTotalID = row.ID;
                        result.InfAboutCarryingValueTotal = row.Value;

                        result.InfAboutCarryingValueTotalRefTypeIxm = row.RefTypeIxm.ID;
                        result.InfAboutCarryingValueTotalRefTypeIxmName = row.RefTypeIxm.Name;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ImmovableProperty)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ImmovableProperty));

                        result.ImmovablePropertyID = row.ID;
                        result.ImmovableProperty = row.Value;
                        result.ImmovablePropertyRefTypeIxm = row.RefTypeIxm.ID;
                        result.ImmovablePropertyRefTypeIxmName = row.RefTypeIxm.Name;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ParticularlyValuableProperty)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ParticularlyValuableProperty));

                        result.ParticularlyValuablePropertyID = row.ID;
                        result.ParticularlyValuableProperty = row.Value;
                        result.ParticularlyValuablePropertyRefTypeIxm = row.RefTypeIxm.ID;
                        result.ParticularlyValuablePropertyRefTypeIxmName = row.RefTypeIxm.Name;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal));

                        result.IncreaseInAccountsPayableTotalID = row.ID;
                        result.IncreaseInAccountsPayableTotal = row.Value;

                        result.IncreaseInAccountsPayableTotalRefTypeIxm = row.RefTypeIxm.ID;
                        result.IncreaseInAccountsPayableTotalRefTypeIxmName = row.RefTypeIxm.Name;
                    }

                    if (data.Any(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.OverduePayables)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.OverduePayables));

                        result.OverduePayablesID = row.ID;
                        result.OverduePayables = row.Value;
                        result.OverduePayablesRefTypeIxm = row.RefTypeIxm.ID;
                        result.OverduePayablesRefTypeIxmName = row.RefTypeIxm.Name;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(
                    "Ошибка получения данных по финансовым и нефинансовым активам: "
                    + e.Message, 
                    e);
            }
        }

        public void SaveValRowFinNFinAssets(
            int recId, 
            string valueId, 
            decimal? val, 
            int refStateValue, 
            int? refTypeIxm)
        {
            F_ResultWork_FinNFinAssets record;
            if (string.IsNullOrEmpty(valueId) || valueId == "0")
            {
                record = new F_ResultWork_FinNFinAssets
                    {
                        ID = 0, 
                        TaskID = 0, 
                        SourceID = 0, 
                        RefStateValue = GetItem<FX_FX_StateValue>(refStateValue), 
                        RefParametr = GetItem<F_F_ParameterDoc>(recId)
                    };
            }
            else
            {
                record = GetItem<F_ResultWork_FinNFinAssets>(Convert.ToInt32(valueId));
            }

            if (refTypeIxm.HasValue)
            {
                record.RefTypeIxm = GetItem<FX_Fin_TypeIzmen>(refTypeIxm.Value);
            }

            record.Value = val;
            Save(record);
        }

        /// <summary>
        ///   Получение даных для формы "Использование имущества"
        /// </summary>
        /// <param name="parentId"> ID документа </param>
        /// <returns> возвращается запись </returns>
        public PropertyUseViewModel GetPropertyUseItem(int parentId)
        {
            try
            {
                var data = from p in GetItems<F_ResultWork_UseProperty>()
                           where p.RefParametr.ID == parentId
                           select p;

                var result = new PropertyUseViewModel { ID = 0 };

                if (data.Count() != 0)
                {
                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.AmountOfFundsFromDisposalID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.AmountOfFundsFromDisposalID));

                        result.AmountOfFundsFromDisposalID = row.ID;
                        result.AmountOfFundsFromDisposalBeginYear = row.BeginYear;
                        result.AmountOfFundsFromDisposalEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.BookValueOfRealEstateTotalID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.BookValueOfRealEstateTotalID));

                        result.BookValueOfRealEstateTotalID = row.ID;
                        result.BookValueOfRealEstateTotalBeginYear = row.BeginYear;
                        result.BookValueOfRealEstateTotalEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.ImmovablePropertyGivenOnRentID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.ImmovablePropertyGivenOnRentID));

                        result.ImmovablePropertyGivenOnRentID = row.ID;
                        result.ImmovablePropertyGivenOnRentBeginYear = row.BeginYear;
                        result.ImmovablePropertyGivenOnRentEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.ImmovablePropertyDonatedID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.ImmovablePropertyDonatedID));

                        result.ImmovablePropertyDonatedID = row.ID;
                        result.ImmovablePropertyDonatedBeginYear = row.BeginYear;
                        result.ImmovablePropertyDonatedEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalID));

                        result.CarryingAmountOfMovablePropertyTotalID = row.ID;
                        result.CarryingAmountOfMovablePropertyTotalBeginYear = row.BeginYear;
                        result.CarryingAmountOfMovablePropertyTotalEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.MovablePropertyGivenOnRentID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.MovablePropertyGivenOnRentID));

                        result.MovablePropertyGivenOnRentID = row.ID;
                        result.MovablePropertyGivenOnRentBeginYear = row.BeginYear;
                        result.MovablePropertyGivenOnRentEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.MovablePropertyDonatedID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.MovablePropertyDonatedID));

                        result.MovablePropertyDonatedID = row.ID;
                        result.MovablePropertyDonatedBeginYear = row.BeginYear;
                        result.MovablePropertyDonatedEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.AreaOfRealEstateTotalID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.AreaOfRealEstateTotalID));

                        result.AreaOfRealEstateTotalID = row.ID;
                        result.AreaOfRealEstateTotalBeginYear = row.BeginYear;
                        result.AreaOfRealEstateTotalEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.GivenOnRentID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.GivenOnRentID));

                        result.GivenOnRentID = row.ID;
                        result.GivenOnRentBeginYear = row.BeginYear;
                        result.GivenOnRentEndYear = row.EndYear;
                    }

                    if (data.Any(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.DonatedID)))
                    {
                        var row = data.First(x => x.RefStateValue.ID == propertyUseModel.DescriptionIdOf(() => propertyUseModel.DonatedID));

                        result.DonatedID = row.ID;
                        result.DonatedBeginYear = row.BeginYear;
                        result.DonatedEndYear = row.EndYear;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw new InvalidDataException(
                    "Ошибка получения данных по финансовым и нефинансовым активам: "
                    + e.Message, 
                    e);
            }
        }

        /// <summary>
        ///   Сохранение значения формы "Использование имущества"
        /// </summary>
        /// <param name="recId"> ID документа </param>
        /// <param name="valueId"> ID записи значения </param>
        /// <param name="beginYearVal"> значение на начало года </param>
        /// <param name="endYearVal"> значение на конец года </param>
        /// <param name="refStateValue"> ID типа значения </param>
        public void SaveValRowPropertyUse(int recId, string valueId, decimal beginYearVal, decimal endYearVal, int refStateValue)
        {
            F_ResultWork_UseProperty record;
            if (string.IsNullOrEmpty(valueId) || valueId == "0")
            {
                record = new F_ResultWork_UseProperty
                    {
                        ID = 0, 
                        TaskID = 0, 
                        SourceID = 0, 
                        RefStateValue = GetItem<FX_FX_StateValue>(refStateValue), 
                        RefParametr = GetItem<F_F_ParameterDoc>(recId)
                    };
            }
            else
            {
                record = GetItem<F_ResultWork_UseProperty>(Convert.ToInt32(valueId));
            }

            record.BeginYear = beginYearVal;
            record.EndYear = endYearVal;
            Save(record);
        }

        /// <summary>
        ///   Удаление документа
        /// </summary>
        /// <param name="recId"> ID документа </param>
        public void DeleteDoc(int recId)
        {
            try
            {
                var membersOfStaff = from p in GetItems<F_ResultWork_Staff>()
                                     where p.RefParametr.ID == recId
                                     select p;

                membersOfStaff.Each(x => Delete<F_ResultWork_Staff>(x.ID));

                var servicesWorks = from p in GetItems<F_ResultWork_ShowService>()
                                    where p.RefParametr.ID == recId
                                    select p;

                servicesWorks.Each(x => Delete<F_ResultWork_ShowService>(x.ID));

                var servicesWorks2016 = from p in GetItems<F_F_ShowService2016>()
                                    where p.RefParametr.ID == recId
                                    select p;

                servicesWorks2016.Each(x => Delete<F_F_ShowService2016>(x.ID));

                var cashReceipts = from p in GetItems<F_ResultWork_CashReceipts>()
                                   where p.RefParametr.ID == recId
                                   select p;

                cashReceipts.Each(x => Delete<F_ResultWork_CashReceipts>(x.ID));

                var cashPayments = from p in GetItems<F_ResultWork_CashPay>()
                                   where p.RefParametr.ID == recId
                                   select p;

                cashPayments.Each(x => Delete<F_ResultWork_CashPay>(x.ID));

                var finNFinAssets = from p in GetItems<F_ResultWork_FinNFinAssets>()
                                    where p.RefParametr.ID == recId
                                    select p;

                finNFinAssets.Each(x => Delete<F_ResultWork_FinNFinAssets>(x.ID));

                var propertyUse = from p in GetItems<F_ResultWork_UseProperty>()
                                  where p.RefParametr.ID == recId
                                  select p;

                propertyUse.Each(x => Delete<F_ResultWork_UseProperty>(x.ID));
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления документа \"Результаты деятельности и использование имущества\": " + e.Message, e);
            }
        }

        #endregion
    }
}
