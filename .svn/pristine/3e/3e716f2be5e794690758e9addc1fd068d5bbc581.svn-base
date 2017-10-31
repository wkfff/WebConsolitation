using System.Collections.Generic;

namespace Krista.FM.RIA.Core.ViewModel
{
    /// <summary>
    /// Модель отображения сущности.
    /// Представляет вкладки основного интерфейса такие как: 
    /// Субъект, Районы, Поселения.
    /// </summary>
    public sealed class DetailViewModel
    {
        /// <summary>
        /// Преднастроенные свойства отображения полей.
        /// </summary>
        public Dictionary<string, ColumnState> ColumnState { get; set; }

        /// <summary>
        /// Количество полей в первом столбце формы.
        /// </summary>
        public int FieldsPerColumn { get; set; }

        public IViewService ViewService { get; set; }

        public int TabRegionType { get; set; }

        public static DetailViewModelBuilder Builder()
        {
            return new DetailViewModelBuilder();
        }
    }
}
