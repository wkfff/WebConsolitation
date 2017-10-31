using System;
using System.Collections.Generic;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.OrgGKH.Params;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views
{
    public class MonthlyCollectionView : CollectionView
    {
        private string rendererPerformedOpFn;

        public MonthlyCollectionView(
            IOrgGkhExtension extension, 
            ILinqRepository<F_Org_GKH> planRepository, 
            ILinqRepository<D_Org_RegistrOrg> orgRepository)
            : base(extension, planRepository, orgRepository)
        {
            var month = DateTime.Today.Month;
            PeriodId = month > 1
                ? (DateTime.Today.Year * 10000) + (month * 100) - 100
                : ((DateTime.Today.Year - 1) * 10000) + 1200;
            DetermineState();
            InitAll();
        }

        public MonthlyCollectionView(
            IOrgGkhExtension extension,
            ILinqRepository<F_Org_GKH> planRepository,
            ILinqRepository<D_Org_RegistrOrg> orgRepository,
            int orgId, 
            string orgName, 
            int periodId, 
            string regionName) 
            : base(extension, planRepository, orgRepository, orgId, orgName, periodId, regionName)
        {
            DetermineState();
            InitAll();
        }

        protected override void CreateColumns(GridPanel gp)
        {
            gp.AutoExpandColumn = "MarkName";

            var groupRow = new HeaderGroupRow();
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 3 });
            groupRow.Columns.Add(new HeaderGroupColumn
                                     {
                                         Header = "Нарастающим итогом с начала года", 
                                         ColSpan = 4, 
                                         Align = Alignment.Center
                                     });
            groupRow.Columns.Add(new HeaderGroupColumn
                                     {
                                         Header = "За отчетный период", 
                                         ColSpan = 4, 
                                         Align = Alignment.Center
                                     });
            gp.View.Add(new GridView { HeaderGroupRows = { groupRow }, ForceFit = true });

            AddColumn(gp, "Code", "№", false).SetWidth(30);
            AddColumn(gp, "MarkName", "Наименование", false);

            AddColumn(gp, "Value", "План на год", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrPlanY");

            AddColumn(gp, "PlanO", "План по начислению, тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrPlanO");
            AddColumn(gp, "Assigned", "Начислено фактически, тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrAssigned");
            AddColumn(gp, "PlanS", "План по сбору (оплате), тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrPlanS");
            AddColumn(gp, "Performed", "Оплачено фактически, тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrPerformed");

            AddColumn(gp, "PlanOOP", "План по начислению, тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrPlanOOP");
            AddColumn(gp, "AssignedOP", "Начислено фактически, тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrAssignedOP");
            AddColumn(gp, "PlanSOP", "План по сбору (оплате), тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = RendererFn.FormatWith("PrPlanSOP");
            AddColumn(gp, "PerformedOP", "Оплачено фактически, тыс.руб.", true)
                .SetWidth(45).Renderer.Fn = rendererPerformedOpFn;
        }

        private void InitAll()
        {
            GridId = "monthlyGrid";
            ReadPeriodUrl = "/Periods/LookupMonthPeriod";
            ReadPlanUrl = "/Monthly/Read";
            SavePlanUrl = "/Monthly/Save";
            IsMonth = true;
            FormTitle = @"Ежемесячная форма ввода информации о деятельности организаций ЖКХ";

            RendererFn =
                @"function (v, p, r, rowIndex, colIndex, ds) {{
                if (r.data.{0} == 'AS' || r.data.{0} == 'AD' || r.data.{0} == 'W') {{
                    p.css = 'gray-cell';
                }}
                if ((r.data.{0} == 'W' && '{0}' != 'PrPlanY') || r.data.{0} == 'X')  {{
                    return 'X';
                }}
                var f = Ext.util.Format.numberRenderer(',00/i');
                return f(v);
            }}";

            rendererPerformedOpFn =
                @"function (v, p, r, rowIndex, colIndex, ds) {
                if (r.data.PrPerformedOP == 'AS' || r.data.PrPerformedOP == 'AD' || r.data.PrPerformedOP == 'W') {
                    p.css = 'gray-cell';
                }
                if (r.data.PrPerformedOP == 'X')  {
                    return 'X';
                }
                var f = Ext.util.Format.numberRenderer(',00/i');
                return f(v);
            }";

            CheckEdit = @"
                if ({0}) {{
                    return false;
                }}
                if (
                    !(e.record.get('Status') == 1) || 
                    (e.field == 'Value' && e.record.get('PrPlanY') != 'M') ||
                    (e.field == 'PlanO' && e.record.get('PrPlanO') != 'M') || 
                    (e.field == 'PlanS' && e.record.get('PrPlanS') != 'M') ||
                    (e.field == 'Assigned' && e.record.get('PrAssigned') != 'M') ||
                    (e.field == 'Performed' && e.record.get('PrPerformed') != 'M') ||

                    (e.field == 'PlanOOP' && e.record.get('PrPlanOOP') != 'M') || 
                    (e.field == 'PlanSOP' && e.record.get('PrPlanSOP') != 'M') ||
                    (e.field == 'AssignedOP' && e.record.get('PrAssignedOP') != 'M') ||
                    (e.field == 'PerformedOP' && e.record.get('PrPerformedOP') != 'M')
                ) {{
                    return false;
                }}
                return true;".FormatWith(
                             !User.IsInRole(OrgGKHConsts.GroupMOName) &&
                             Extension.UserGroup != OrgGKHExtension.GroupOrg ? "true" : "false");

            ColumnNames = new List<string>
                              {
                                  "ID",
                                  "MarkId",
                                  "MarkName",
                                  "PrPlanY",
                                  "Value",

                                  "PrPlanO",
                                  "PlanO",
                                  "PrAssigned",
                                  "Assigned",
                                  "PrPlanS",
                                  "PlanS",
                                  "PrPerformed",
                                  "Performed",

                                  "PrPlanOOP",
                                  "PlanOOP",
                                  "PrAssignedOP",
                                  "AssignedOP",
                                  "PrPlanSOP",
                                  "PlanSOP",
                                  "PrPerformedOP",
                                  "PerformedOP",

                                  "Status",
                                  "Code"
                              };
        }
    }
}