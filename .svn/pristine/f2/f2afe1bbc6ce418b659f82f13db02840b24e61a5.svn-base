using System;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.DashboardNotepadFin
{
    public partial class DashboardNotepadFin : CustomReportPage
    {
        private const string cookieStateAreaParamName = "StateArea";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        }

        /// <summary>
        /// Глобальный обработчик загрузки страницы
        /// </summary>
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                rzprCombo.Width = 300;
                rzprCombo.MultiSelect = false;
                rzprCombo.ParentSelect = true;
                rzprCombo.FillDictionaryValues(GetKindsDictionary());
                rzprCombo.Title = "Разрезность";
                rzprCombo.SetСheckedState("КОСГУ", true);
            }

            Session["rzprType"] = rzprCombo.SelectedValue == "РЗПР" ? "rzpr" : "kosgu";

            UserParams.BudgetLevelEnum.Value = BudgetLevel.Checked ? 
                                "[Уровни бюджета].[СКИФ].[Все].[Конс.бюджет субъекта]" :
                                "[Уровни бюджета].[СКИФ].[Все].[Бюджет субъекта]";

            UserParams.FODeficitDocumentType.Value = BudgetLevel.Checked ?
                                "Консолидированная отчетность и отчетность внебюджетных территориальных фондов" :
                                "Собственный отчет по бюджету субъекта";
            
            Page.Title = "Блокнот финансиста";

            TitleLabel.Text = "Вологодская область";
            SubTitleLabel.Text = BudgetLevel.Checked ? "Сводная информация об исполнении консолидированного бюджета области" : "Сводная информация об исполнении областного бюджета";

            SetCookies(cookieStateAreaParamName, UserParams.StateArea.Value);
        }

        private Dictionary<string, int> GetKindsDictionary()
        {
            Dictionary<string, int> kinds = new Dictionary<string, int>();
            kinds.Add("РЗПР", 0);
            kinds.Add("КОСГУ", 0);
            return kinds;
        }
    }
}
