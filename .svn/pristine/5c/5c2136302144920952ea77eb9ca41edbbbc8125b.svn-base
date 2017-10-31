using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class IndicatorsView
    {
        /// <summary>
        /// Признак, можно ли редактировать показатели
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Идентификатор заявки от налогоплательщика
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Является ли данная заявка копие другой заявки
        /// </summary>
        public bool IsCopy { get; set; }

        /// <summary>
        /// Идентификатор Store для показателей
        /// </summary>
        public string IndicatorsStoreId { get; set; }

        public int RequestYear { get; set; }

        public List<Component> Build(ViewPage page)
        {
            var indicatorsGrid = new IndicatorsGridControl(ApplicationId, RequestYear)
                                     {
                                         IndicatorsStoreId = IndicatorsStoreId, 
                                         IsCopy = IsCopy, 
                                         Editable = Editable
                                     };

            var tabIndicators = new Panel
            {
                ID = "IndicatorsTab",
                Title = @"Показатели",
                Border = false,
                Height = 100,
                LabelWidth = 1,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Items = { new BorderLayout { Center = { Items = { indicatorsGrid.Build(page) } } } },
                Closable = false
            };

            return new List<Component> { tabIndicators };
        }
    }
}
