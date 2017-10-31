using System;
using System.Data;
using System.Web;
using System.Xml;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Default : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            //if (NeedInitializeDefautStateArea())
            //{
            //    CustomParams.MakeRegionParams("37", "id");
            //}
            //if (Request.Params["subjectID"] != null)
            //{
            //    CustomParams.MakeRegionParams(Request.Params["subjectID"], "id");
            //}
            if (Request.Params["reportID"] != null)
            {
                string reportId = Request.Params["reportID"];
                string reportFolderName = GetReportFolderName(reportId.Replace("_white", String.Empty).Replace("_White", String.Empty));
                Response.Redirect(string.Format("~/{0}{1}.aspx", reportFolderName, GetReportRedirectId(reportId.Replace("_white", String.Empty).Replace("_White", String.Empty).Replace("indexPage_", String.Empty).Replace("indexpage_", String.Empty))));
            }
            else
            {
                Response.Redirect("~/reports/iPhone/index.aspx");
            }
            
        }

        /// <summary>
        /// Извлекает регион и ставит его.
        /// Если в имени нет, ставит из конфига.
        /// </summary>
        /// <param name="reportName"></param>
        /// <returns></returns>
        private string SetRegionByReportName(string reportName)
        {
			string reportId = TrimRotateSuffix(reportName);
            reportId = reportId.Replace("_Horizontal", String.Empty).Replace("_horizontal", String.Empty);
			string[] reportNameSplitted = reportId.Split('_');
            
            if (RegionNames.IsRegionName(reportNameSplitted[reportNameSplitted.Length - 1]))
            {
				RegionSettingsHelper.Instance.SetWorkingRegion(reportNameSplitted[reportNameSplitted.Length - 1]);
                return reportNameSplitted[reportNameSplitted.Length - 1];
            }
            else 
            {
                int value;
                if ((reportNameSplitted[reportNameSplitted.Length - 1].Length == 2 ||
                    reportNameSplitted[reportNameSplitted.Length - 1].Length == 1 ) &&
                    Int32.TryParse(reportNameSplitted[reportNameSplitted.Length - 1], out value))
                {
					CustomParams.MakeRegionParams(value.ToString(), "id");
                    
                    if (reportId.ToUpper().Contains("CONTROL"))
                    {
                        CustomParams.MakeControlsParams(reportNameSplitted[reportNameSplitted.Length - 2].Split('=')[1], "id");
                        return reportNameSplitted[reportNameSplitted.Length - 2] + "_" + reportNameSplitted[reportNameSplitted.Length - 1];
                    }
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
				else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("FOOD")) && (reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("MO")))
				{
					string[] splitParams = reportNameSplitted[reportNameSplitted.Length - 1].Split(';');
					CustomParams.MakeFoodParams(splitParams[0].Split('=')[1], "id");
					CustomParams.MakeMoParams(splitParams[1].Split('=')[1], "id");
					return reportNameSplitted[reportNameSplitted.Length - 1];
				}
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("MOTYPE")))
                {
                    CustomParams.MakeMotypeParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
				else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("MO")))
                {
                    CustomParams.MakeMoParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("GRBS")))
                {
                    CustomParams.MakeGrbsParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("IT")))
                {
                    CustomParams.MakeItParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
				else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("FOOD")))
				{
					CustomParams.MakeFoodParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
					return reportNameSplitted[reportNameSplitted.Length - 1];
				}
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("PROG")))
                {
                    CustomParams.MakeProgParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("FO")))
                {
                    CustomParams.MakeFoParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("OUTCOMES")))
                {
                    CustomParams.MakeOutcomesParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("OIL")))
                {
                    CustomParams.MakeOilParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("BANK")))
                {
                    CustomParams.MakeBanksParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("BUDGET")))
                {
                    CustomParams.MakeBudgetParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }                
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("SETTLEMENT")))
                {
                    CustomParams.MakeSettlementParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
                else if ((reportNameSplitted[reportNameSplitted.Length - 1].ToUpper().Contains("BUILDERCUSTOMER")))
                {
                    CustomParams.MakeBuildingCustomerParams(reportNameSplitted[reportNameSplitted.Length - 1].Split('=')[1], "id");
                    return reportNameSplitted[reportNameSplitted.Length - 1];
                }
				else
                {
                    //RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Идентификатор базового отчета.
        /// </summary>
        /// <param name="reportName"></param>
        /// <returns></returns>
        private string GetReportRedirectId(string reportName)
        {
            string reportId = reportName;
            string region = SetRegionByReportName(reportName);
            if (!String.IsNullOrEmpty(region))
            {
                reportId = reportId.ToLower().Replace("_" + region.ToLower(), string.Empty);
            }
            return reportId;
        }

        private static string TrimRotateSuffix(string reportName)
        {
            string[] reportNameSplitted = reportName.Split('_');
            if (reportNameSplitted[reportNameSplitted.Length - 1].ToLower() == "v" ||
                reportNameSplitted[reportNameSplitted.Length - 1].ToLower() == "h")
            {
                return reportName.Remove(reportName.Length - 2, 2);
            }
            return reportName;
        }

        private string GetReportFolderName(string reportName)
        {
            string reportId = TrimRotateSuffix(reportName);
            DataRow[] rows = AllowedReportsIPhone.Select(String.Format("Code like '{0}'", reportId));
            if (rows.Length > 0)
            {
                return rows[0]["DocumentFileName"].ToString();
            }
            return string.Empty;
        }

        private bool NeedInitializeDefautStateArea()
        {
            return UserParams.Region.Value == null ||
                   UserParams.StateArea.Value == null ||
                   UserParams.Region.Value == String.Empty ||
                   UserParams.StateArea.Value == String.Empty;
        }
    }
}
