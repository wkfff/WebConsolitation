using Krista.FM.Domain;
using Krista.FM.Domain.Services.FinSourceDebtorBook;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public interface IDebtBookExtension
    {
        RegionsAccordanceService RegionsAccordance { get; }

        VariantDescriptor Variant { get; set; }

        /// <summary>
        /// год источника, по которому записан текущий пользователь
        /// </summary>
        int UserYear { get; }

        int CurrentSourceId { get; }

        /// <summary>
        /// Источник данных "ФО_Анализ данных" соответствующий текущему году варианта.
        /// </summary>
        int CurrentAnalysisSourceId { get; }

        /// <summary>
        /// Регион текущего пользователя.
        /// </summary>
        int UserRegionId { get; }

        /// <summary>
        /// Наименование текущего региона пользователя.
        /// </summary>
        string UserRegionName { get; }

        /// <summary>
        /// Тип региона текущего пользователя.
        /// </summary>
        UserRegionType UserRegionType { get; set; }

        /// <summary>
        /// Id субъекта по выбранному источнику данных "ФО_Анализ данных" из классификатора "Районы.Анализ".
        /// </summary>
        int SubjectRegionId { get; }

        /// <summary>
        /// Статус сбора, для текущего региона
        /// </summary>
        T_S_ProtocolTransfer StatusSchb { get; set; }

        /// <summary>
        /// Имя статуса сбора, для текущего региона
        /// </summary>
        string StatusSchbText { get; }

        void SetCurrentVariant(VariantDescriptor variant);

        /// <summary>
        ///  Проверка статуса текущего варианта
        /// </summary>
        bool CurrentVariantBlocked();
    }
}