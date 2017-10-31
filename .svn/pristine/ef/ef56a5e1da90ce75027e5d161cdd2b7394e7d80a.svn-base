using System;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports
{
	public partial class DashboardFederal_NAO : CustomReportPage
	{
	    private const string cookieStateAreaParamName = "StateArea";
        private bool onWall;
        private bool blackStyle;

        private int fontSizeMultiplier = 4;


        #region Параметры запроса

        private CustomParam selectedSubject;

        #endregion

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);

            blackStyle = false;
            if (Session["blackStyle"] != null)
            {
                blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            }

            string regionTheme = RegionSettings.Instance.Id;
            CRHelper.SetPageTheme(this, blackStyle ? regionTheme + "BlackStyle" : regionTheme);
        }

        /// <summary>
		/// Глобальный обработчик загрузки страницы
		/// </summary>
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

            #region Инициализация параметров запроса

            selectedSubject = UserParams.CustomParam("selected_subject", true);

            #endregion

            Session["blackStyle"] = null;

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
            }

            WallLink.Visible = !onWall;
            BlackStyleWallLink.Visible = !onWall;
            regionsCombo.Visible = !onWall;
            RefreshButton1.Visible = !onWall;

            fontSizeMultiplier = onWall ? 4 : 1;

            if (onWall)
            {
                ComprehensiveDiv.Style.Add("width", "5600px");
                ComprehensiveDiv.Style.Add("height", "2100px");
                //ComprehensiveDiv.Style.Add("border", "medium solid #FF0000");
            }

			if (DataProvidersFactory.SecondaryMASDataProvider == null)
			{
				Server.Transfer(CustomReportConst.userErrorPageUrl);
			}

            if (!Page.IsPostBack)
            {
                regionsCombo.Width = 410;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;
                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    selectedSubject.Value = UserParams.StateArea.Value;
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    selectedSubject.Value = RegionSettings.Instance.Name;
                }
                regionsCombo.SetСheckedState(selectedSubject.Value, true);
            }

            selectedSubject.Value = regionsCombo.SelectedValue;
            
            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;

            CustomParams.MakeRegionParams(selectedSubject.Value, "name");

            Page.Title = string.Format("Сравнение субъектов РФ: {0}", UserParams.StateArea.Value);
            imgHerald.ImageUrl = String.Format("../../images/Heralds/{0}.png", HttpContext.Current.Session["CurrentSubjectID"]);

            imgHerald.Height = onWall ? 250 : 65;

            TitleLabel.Text = "Сводная информация об исполнении консолидированного бюджета субъекта РФ&nbsp;";
            SubTitleLabel.Text = UserParams.StateArea.Value;

            TitleLabel.Font.Size = 14 * fontSizeMultiplier; 
            SubTitleLabel.Font.Size = 12 * fontSizeMultiplier;

            if (onWall && Page.Master is IMasterPage)
            {
                ((IMasterPage) Page.Master).SetHeaderVisible(false);
            }

            WallLink.Text = "Для&nbsp;видеостены";
            WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());

            BlackStyleWallLink.Text = "Для&nbsp;видеостены&nbsp;(черный&nbsp;стиль)";
            BlackStyleWallLink.NavigateUrl = String.Format("{0};onWall=true;blackStyle=true", UserParams.GetCurrentReportParamList());

            string labelCssClass = onWall ? "WallGadgetLabelText" : "GadgetLabelText";

            RedArrowHintLabel.CssClass = labelCssClass;
            YellowArrowHintLabel.CssClass = labelCssClass;
            BestStarHintLabel.CssClass = labelCssClass;
            WorseStarHintLabel.CssClass = labelCssClass;
            EqualbilityHintLabel.CssClass = labelCssClass;

            string bestStar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    onWall ? 96 : 27);

            string worseStar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    onWall ? 96 : 27);

            string redGradientBar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                   onWall ? 96 : 27);

            string greenGradientBar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                   onWall ? 96 : 27);

            BestStarHintLabel.Text = String.Format("{0} - лучший ранг&nbsp;&nbsp;&nbsp;{1} - худший ранг", bestStar, worseStar);
            RedArrowHintLabel.Text = String.Format("Большая стрелка циферблата - исполнение по субъекту, %");
            YellowArrowHintLabel.Text = String.Format("Малая стрелка циферблата - среднее значение исполнения по РФ, %");
            EqualbilityHintLabel.Text = String.Format("Шкала равномерности {0} / {1} - равномерность/неравномерность исполнения бюджета", greenGradientBar, redGradientBar);
		}

        protected override void Page_LoadComplete(object sender, EventArgs e)
        {
            base.Page_LoadComplete(sender, e);
            Session["onWall"] = null;
        }

        protected override void Page_Error(object sender, EventArgs e)
        {
            base.Page_Error(sender, e);
            Session["onWall"] = null;
        }
	}
}
