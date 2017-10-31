using System.Text;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controls
{
    public class HMAOIndicatorsGridControl : IndicatorsGridControl
    {
        private readonly bool showCntOrgsLable;
        private readonly int categoryId;
        private readonly int periodId;
        private readonly string cntOrgsLabelId;

        public HMAOIndicatorsGridControl(int applicationId, int taxTypeId, int periodId, int categoryId) : base(applicationId, periodId / 10000)
        {
            ReadUrl = "/FO41HMAOIndicators/Read?applicationId={0}&taxTypeId={1}".FormatWith(applicationId, taxTypeId);
            UpdateUrl = "/FO41HMAOIndicators/Create";
            UpdateUrl = "/FO41HMAOIndicators/Save?applicationId={0}".FormatWith(applicationId);

            // показываем колонку с кнопками добавлени/удаления комментариев 
            // только для налога на имущество организаций
            ShowAddRemoveDetailsButtons = taxTypeId == 9;
            this.periodId = periodId;
            AfterLoadListener = string.Empty;
            showCntOrgsLable = false;
            InitControl();
        }

        public HMAOIndicatorsGridControl(int categoryId, int taxTypeId, int periodId) : base(-1, periodId / 10000)
        {
            ReadUrl = "/FO41HMAOIndicators/ReadByCategory?categoryId={0}&taxTypeId={1}&periodID={2}"
                .FormatWith(categoryId, taxTypeId, periodId);
            GridId = "{0}{1}".FormatWith(GridId, categoryId);

            // показываем колонку с кнопками добавлени/удаления комментариев 
            // только для налога на имущество организаций
            ShowAddRemoveDetailsButtons = taxTypeId == 9;
            this.categoryId = categoryId;
            this.periodId = periodId;
            showCntOrgsLable = true;
            cntOrgsLabelId = "CntOrgsLabel{0}{1}".FormatWith(categoryId, periodId);
            AfterLoadListener = GetGetCntOrgsHandler();
            InitControl();
        }

        protected void InitControl()
        {
            IsCopy = false;

            AddToolTipAutoCalc = false;

            RendererFnPart = @"function (v, p, r) {{
                    if (r.data.IsFormula || {0} || r.data.RefName == 'Справочно') 
                        p.css = 'gray-cell';
                    var okei = r.data.OKEI;
                    var f;
                    if  (okei == 744)
                        f = Ext.util.Format.numberRenderer('0.000,00/i');
                    else
                    if  (okei == 383 || okei == 384)
                        f = Ext.util.Format.numberRenderer('0.000,0/i');
                    else if (okei == 792 || okei == 796)
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    else
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    return f(v);
                }}";

            BeforeEditHandler = @"
                if (e.field == 'RefName' && 
                    (e.record.get('DetailMark') == null || e.record.get('DetailMark') == 0)) 
                {
                    return false;
                }
                if (e.record.get('RefName') == 'Справочно') {
                    return false;
                }
                return true;";
        }

        protected override void CreateValueColumns(GridPanel gp)
        {
            /*За отчетный (налоговый) период*/
            AddDoubleColumn(gp, "Fact", "{0}".FormatWith(ExportService.GetTextForPeriod(periodId))).Width = 150;
            /*За соответствующий отчетный (налоговый) период предыдущего года*/
            AddDoubleColumn(
                    gp, 
                    "PreviousFact",
                    "{0}".FormatWith(ExportService.GetTextForPeriod(periodId - 10000)))
                .Width = 150;

            GroupValueColumns.ColSpan = 2;

            gp.AddRefreshButton();

            if (showCntOrgsLable)
            {
                gp.Toolbar().Add(new DisplayField { ID = cntOrgsLabelId });
            }
        }

        protected override void AddDetailMarkHandler(GridPanel gp)
        {
            var bookWindowEdit = GetDetailsWindow();
            gp.Listeners.Command.Handler = @"
            if (command == 'ViewMarkDetails') {{
                if (record != null) {{
                    {0}
                    {1}.autoLoad.url = '/FO41HMAOIndicators/ViewDetails?markId=' + record.data.RefMarks + 
                        '&categoryId={3}&periodId={4}';
                    {1}.show();
                }}
            }}".FormatWith(bookWindowEdit.ToScript(), bookWindowEdit.ID, gp.ID, categoryId, periodId);
        }

        private string GetGetCntOrgsHandler()
        {
            return new StringBuilder(@"
            Ext.Ajax.request({
                url: '/FO41HMAORequestsList/GetCntOrgs?categoryId=")
                    .Append(categoryId)
                    .Append("&periodID=' + ")
                    .Append(periodId)
                    .Append(@",
                success: function (response, options) {
                        var fi = response.responseText.indexOf('message:') + 9;
                        var li = response.responseText.lastIndexOf('" + '"' + @"')
                        var msg = response.responseText.substring(li, fi);
                    if (response.responseText.indexOf('success:true') > -1) {")
                    .Append(cntOrgsLabelId)
                    .Append(@".setValue('Количество организаций, по заявкам от которых строится общая форма: ' + msg);
                    }
                    else {
                        Ext.net.Notification.show({
                            iconCls    : 'icon-information', 
                            html       : msg, 
                            title      : 'Анализ и планирование', 
                            hideDelay  : 2500
                        });
                    }
                },
                failure: function (response, options) {
                }
            })").ToString();
        }
    }
}
