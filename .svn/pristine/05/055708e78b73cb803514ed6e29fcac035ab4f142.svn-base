using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class EstimateResultDataView : View
    {
        /// <summary>
        /// Идентификатор заявки от ОГВ
        /// </summary>
        private readonly int appFromOGVId;

        private readonly int requestYear;

        public EstimateResultDataView(int appFromOGVId, int requestYear)
        {
            this.appFromOGVId = appFromOGVId;
            this.requestYear = requestYear;
            IndicatorsStoreId = "IndicatorsStore";
        }

        /// <summary>
        /// Идентификатор Store для показателей
        /// </summary>
        public string IndicatorsStoreId { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            var indicatorsGrid = new IndicatorsGridControl(appFromOGVId, requestYear)
                {
                    IndicatorsStoreId = IndicatorsStoreId, 
                    IsCopy = false, 
                    Editable = false,
                    ReadUrl = "/FO41Indicators/ReadEstimate?applicationId={0}".FormatWith(appFromOGVId),
                    UpdateUrl = "/FO41ResultIndicators/SaveResultData?appFromOGVId={0}".FormatWith(appFromOGVId),
                    ShowDetailsMark = true
                };

            var tabDetails = new Panel
            {
                ID = "EstimateResultTab",
                Title = @"Итоговые данные",
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Closable = false,
                Items =
                        {
                            new BorderLayout { Center = { Items = { indicatorsGrid.Build(page) } } }
                        }
            };

            return new List<Component> { tabDetails };
        }
    }
}
