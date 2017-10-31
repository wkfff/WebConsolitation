using System;
using System.Linq.Expressions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel
{
    // Описания взяты из FX_FX_StateValue
    public class PropertyUseViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseDescription("Объем средств, полученный в отчетном году от распоряжения в установленном порядке имуществом", 9)]
        public int? AmountOfFundsFromDisposalID { get; set; }

        public decimal? AmountOfFundsFromDisposalBeginYear { get; set; }

        public decimal? AmountOfFundsFromDisposalEndYear { get; set; }

        [DataBaseDescription("Балансовая стоимость недвижимого имущества, ВСЕГО", 10)]
        public int? BookValueOfRealEstateTotalID { get; set; }

        public decimal? BookValueOfRealEstateTotalBeginYear { get; set; }

        public decimal? BookValueOfRealEstateTotalEndYear { get; set; }

        [DataBaseDescription("Недвижимого имущества переданного в аренду", 11)]
        public int? ImmovablePropertyGivenOnRentID { get; set; }

        public decimal? ImmovablePropertyGivenOnRentBeginYear { get; set; }

        public decimal? ImmovablePropertyGivenOnRentEndYear { get; set; }

        [DataBaseDescription("Недвижимого имущества переданного в безвозмездное пользование", 12)]
        public int? ImmovablePropertyDonatedID { get; set; }

        public decimal? ImmovablePropertyDonatedBeginYear { get; set; }

        public decimal? ImmovablePropertyDonatedEndYear { get; set; }

        [DataBaseDescription("Балансовая стоимость движимого имущества, ВСЕГО", 13)]
        public int? CarryingAmountOfMovablePropertyTotalID { get; set; }

        public decimal? CarryingAmountOfMovablePropertyTotalBeginYear { get; set; }

        public decimal? CarryingAmountOfMovablePropertyTotalEndYear { get; set; }

        [DataBaseDescription("Движимого имущества переданного в аренду", 14)]
        public int? MovablePropertyGivenOnRentID { get; set; }

        public decimal? MovablePropertyGivenOnRentBeginYear { get; set; }

        public decimal? MovablePropertyGivenOnRentEndYear { get; set; }

        [DataBaseDescription("Движимого имущества переданное в безвозмездное пользование", 15)]
        public int? MovablePropertyDonatedID { get; set; }

        public decimal? MovablePropertyDonatedBeginYear { get; set; }

        public decimal? MovablePropertyDonatedEndYear { get; set; }

        [DataBaseDescription("Площадь объектов недвижимого имущества, ВСЕГО", 16)]
        public int? AreaOfRealEstateTotalID { get; set; }

        public decimal? AreaOfRealEstateTotalBeginYear { get; set; }

        public decimal? AreaOfRealEstateTotalEndYear { get; set; }

        [DataBaseDescription("Переданного в аренду", 17)]
        public int? GivenOnRentID { get; set; }

        public decimal? GivenOnRentBeginYear { get; set; }

        public decimal? GivenOnRentEndYear { get; set; }

        [DataBaseDescription("Переданного в безвозмездное пользование", 18)]
        public int? DonatedID { get; set; }

        public decimal? DonatedBeginYear { get; set; }

        public decimal? DonatedEndYear { get; set; }

        public int DescriptionIdOf<T>(Expression<Func<T>> expr)
        {
            return UiBuilders.DescriptionIdOf(expr);
        }
    }
}
