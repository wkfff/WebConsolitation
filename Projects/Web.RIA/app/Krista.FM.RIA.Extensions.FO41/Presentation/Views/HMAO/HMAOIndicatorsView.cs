using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOIndicatorsView : View
    {
        private readonly IFO41Extension extension;

        /// <summary>
        /// Идентификатор заявки
        /// </summary>
        private readonly int applicationId;

        /// <summary>
        /// Идентификатор типа налога
        /// </summary>
        private readonly int taxTypeId;

        /// <summary>
        /// Идентификатор периода
        /// </summary>
        private readonly int periodId;

        public HMAOIndicatorsView(IFO41Extension extension, int applicationId, int taxTypeId, int periodId)
        {
            this.extension = extension;
            this.applicationId = applicationId;
            this.taxTypeId = taxTypeId;
            this.periodId = periodId;
        }

        /// <summary>
        /// Признак, можно ли редактировать показатели
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Идентификатор заявки от налогоплательщика
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Идентификатор Store для показателей
        /// </summary>
        public string IndicatorsStoreId { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.UserGroup != FO41Extension.GroupOGV && extension.UserGroup != FO41Extension.GroupTaxpayer)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен налогоплательщик или экономический орган.").
                        ToScript());

                return new List<Component>();
            }

            var indicatorsGrid = new HMAOIndicatorsGridControl(applicationId, taxTypeId, periodId, -1)
                                     {
                                         IndicatorsStoreId = IndicatorsStoreId, 
                                         IsCopy = false, 
                                         Editable = Editable,
                                         ShowDetailsMark = false,
                                         ShowOKEI = true
                                     };
            var columnDetails = indicatorsGrid.Columns.Find(x => x.ColumnID.Equals("HasDetail"));
            if (columnDetails != null)
            {
                columnDetails.Hidden = true;    
            }

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
