using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOTaxCategoryView : Panel
    {
        /// <summary>
        /// Идентификатор Store для показателей
        /// </summary>
        public static readonly string IndicatorsStoreId = "IndicatorsCommonStore";

        private readonly IFO41Extension extension; 
        
        private readonly int periodId;

        private readonly int taxTypeId;

        private readonly D_Org_CategoryTaxpayer category;

        private readonly string titleTemplate;
        
        public HMAOTaxCategoryView(
            IFO41Extension extension,
            D_Org_CategoryTaxpayer category, 
            int taxTypeId, 
            int periodId)
        {
            this.extension = extension;
            this.category = category;
            this.taxTypeId = taxTypeId;
            this.periodId = periodId;
            titleTemplate = "{0} (общее количество заявок, в том числе в состоянии создана - "
                .FormatWith(category.Name) + "{0})";
        }

        public void InitAll(ViewPage page)
        {
            ID = "panel{0}categoryHMAO".FormatWith(category.ID);
            Items.Add(
                new BorderLayout
                {
                    Center =
                        {
                            Items = { CreateTabPanel(page) }
                        },
                    North =
                    {
                        Items = 
                        { 
                            new CompositeField 
                            { 
                                Items = 
                                {
                                        new Button
                                        {
                                            ID = "exportByCategoryButton{0}".FormatWith(category.ID),
                                            Icon = Icon.PageExcel,
                                            ToolTip = @"Выгрузка в Excel итоговой формы по категории",
                                            Listeners = 
                                            { 
                                                Click = 
                                                { 
                                                    Handler = @"
                                                    Ext.net.DirectMethod.request({{
                                                        url: '/FO41Export/ReportResultCategoryHMAO',
                                                        isUpload: true,
                                                        formProxyArg: '{0}',
                                                        cleanRequest: true,
                                                        params: {{
                                                            categoryId: {1}, periodId: {2}
                                                        }},
                                                        success:function (response, options) {{
                                                        }},
                                                        failure: function (response, options) {{
                                                        }}
                                                    }});".FormatWith("HMAOTaxForm{0}".FormatWith(taxTypeId), category.ID, periodId)
                                                } 
                                            }
                                        },
                                        new DisplayField
                                                    {
                                                        ID = "taxCnt{0}".FormatWith(category.ID),
                                                        Text = titleTemplate.FormatWith(0),
                                                        StyleSpec = "padding-left: 5px; padding-right: 5px; padding-top: 5px; padding-bottom: 5px; font-size: 12px;"
                                                    } 
                                }
                            }
                        }
                    }
                });
        }

        private IEnumerable<Component> CreateTabPanel(ViewPage page)
        {
            var tabs = new TabPanel
            {
                ID = "Category{0}TabPanel".FormatWith(category.ID),
                Border = false,
                ActiveTabIndex = 0,
                EnableTabScroll = true
            };

            var reqListView = new HMAOReqListForOGVView(extension, category.ID, periodId, taxTypeId, titleTemplate)
                                  {
                                      Title = @"Список заявок от налогоплательщиков"
                                  }; 
            reqListView.Build(page);
            tabs.Add(reqListView);

            var indicatorsView = new HMAOResultCategoryView(category.ID, taxTypeId, periodId)
                                     {
                                         IndicatorsStoreId = "{0}{1}".FormatWith(IndicatorsStoreId, category.ID)
                                     };
            tabs.Add(indicatorsView.Build(page));
            
            tabs.Add((new HMAOEstimateView(category.ID, taxTypeId, periodId) { Editable = extension.IsReqLastPeriod(periodId) }).Build(page));

            return new List<Component> { tabs };
        }
    }
}
