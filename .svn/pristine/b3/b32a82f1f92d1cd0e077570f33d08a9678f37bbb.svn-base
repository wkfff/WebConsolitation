using System;
using System.Collections.Generic;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.OrgGKH.Params;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views
{
    public class WeeklyCollectionView : CollectionView
    {
        public WeeklyCollectionView(
            IOrgGkhExtension extension, 
            ILinqRepository<F_Org_GKH> planRepository, 
            ILinqRepository<D_Org_RegistrOrg> orgRepository)
            : base(extension, planRepository, orgRepository)
        {
            var today = DateTime.Today;
            var change = (today.DayOfWeek > DayOfWeek.Friday || today.DayOfWeek == DayOfWeek.Sunday) ? -1 : 1;

            while (today.DayOfWeek != DayOfWeek.Friday)
            {
                today = today.AddDays(change);
            }

            // найти ближайшую пятницу (текущая неделя)
            PeriodId = (today.Year * 10000) + (today.Month * 100) + today.Day;
            DetermineState();
            Init();
        }

        public WeeklyCollectionView(
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
            Init();
        }

        protected override void CreateColumns(Ext.Net.GridPanel gp)
        {
            AddColumn(gp, "Code", "№", false).SetWidth(40);
            AddColumn(gp, "MarkName", "Наименование", false).SetWidth(300);
            AddColumn(gp, "YearPlan", "План на год", true).SetWidth(150).Renderer.Fn = @"
            function (v, p, r, rowIndex, colIndex, ds) {{
                p.css = 'gray-cell';
                var f = Ext.util.Format.numberRenderer(',00/i');
                return f(v);
            }}";
            AddColumn(gp, "PerformedOP", "Оплачено фактически, тыс.руб.", true).SetWidth(200)
                .Renderer.Fn = RendererFn.FormatWith("PrPerformedOP");
        }

        private void Init()
        {
            GridId = "WeeklyGrid";
            ReadPeriodUrl = "/Periods/LookupWeekPeriod";
            ReadPlanUrl = "/Monthly/ReadWeek";
            SavePlanUrl = "/Monthly/SaveWeek";
            IsMonth = false;
            FormTitle = @"Еженедельная форма ввода информации о деятельности организаций ЖКХ";

            RendererFn =
                @"function (v, p, r, rowIndex, colIndex, ds) {{
                if (r.data.{0} == 'M' || r.data.{0} == 'X')  {{
                    return 'X';
                }}
                if (r.data.{0} == 'AS' || r.data.{0} == 'AD') {{
                    p.css = 'gray-cell';
                }}
                var f = Ext.util.Format.numberRenderer(',00/i');
                return f(v);
            }}";

            CheckEdit = @"
                if (e.field == 'YearPlan' || 
                    !(e.record.get('Status') == 1) || 
                    (e.record.get('PrPerformedOP') != 'W') || {0}) {{
                    return false;
                }}
                return true;".FormatWith(!User.IsInRole(OrgGKHConsts.GroupMOName) &&
                                         Extension.UserGroup != OrgGKHExtension.GroupOrg
                            ? "true" : "false");

            ColumnNames = new List<string>
                              {
                                  "ID",
                                  "MarkId",
                                  "MarkName",
                                  "PrPerformedOP",
                                  "PerformedOP",
                                  "Status",
                                  "Code",
                                  "YearPlan"
                              };
        }
    }
}