using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOResultCategoryView : View
    {
        /// <summary>
        /// Идентификатор категории
        /// </summary>
        private readonly int categoryId;

        /// <summary>
        /// Идентификатор типа налога
        /// </summary>
        private readonly int taxTypeId;

        /// <summary>
        /// Идентификатор периода
        /// </summary>
        private readonly int periodId;

        public HMAOResultCategoryView(int categoryId, int taxTypeId, int periodId)
         {
            this.categoryId = categoryId;
            this.taxTypeId = taxTypeId;
            this.periodId = periodId;
         }

         /// <summary>
         /// Идентификатор Store для показателей
         /// </summary>
         public string IndicatorsStoreId { get; set; }

         public override List<Component> Build(ViewPage page)
         {
             var indicatorsGrid = new HMAOIndicatorsGridControl(categoryId, taxTypeId, periodId)
             {
                 IndicatorsStoreId = IndicatorsStoreId,
                 Editable = false,
                 IsCopy = false,
                 ShowDetailsMark = true,
                 ShowOKEI = true
             };

             var tabDetails = new Panel
             {
                 ID = "HMAOResultTab{0}".FormatWith(categoryId),
                 Title = @"Общая форма по категории",
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
