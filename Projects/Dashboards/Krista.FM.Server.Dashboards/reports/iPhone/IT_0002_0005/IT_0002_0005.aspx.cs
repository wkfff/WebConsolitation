using System;
using System.Web;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0002_0005 : CustomReportPage
    {
        private CustomParam assessType;
        private CustomParam directAssess;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string id = HttpContext.Current.Session["CurrentITID"].ToString();
            assessType = CustomParam.CustomParamFactory("assess_type");
            directAssess = CustomParam.CustomParamFactory("direct_assess");

            switch(id)
            {
                case "4":
                    {
                        assessType.Value = "������� �� ���������� �����, ���.���.";
                        directAssess.Value = "BDESC";
                        break;
                    }
                case "5":
                    {
                        assessType.Value = "������� (������) �� ���������������, ���.���.";
                        directAssess.Value = "BDESC";
                        break;
                    }
                case "6":
                    {
                        assessType.Value = "�������������� ������������ (� ��������)";
                        directAssess.Value = "BDESC";
                        break;
                    }
                case "7":
                    {
                        assessType.Value = "������� �� 100 ������ �������, ���.";
                        directAssess.Value = "BASC";
                        break;
                    }
                case "8":
                    {
                        assessType.Value = "����������� ��� � �������, %";
                        directAssess.Value = "BASC";
                        break;
                    }
            }
            Label1.Text = assessType.Value;
        }
    }
}
