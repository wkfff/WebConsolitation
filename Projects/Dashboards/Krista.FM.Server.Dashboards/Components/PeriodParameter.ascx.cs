using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Krista.FM.Server.Dashboards.core
{
    public partial class PeriodParameter : System.Web.UI.UserControl
    {
        private string headOfMember;
        public string HeadOfMember
        {
            get { return headOfMember; }
            set { headOfMember = value; }            
        }

        public string MemberUniqueName
        {
            get 
            { 
                string result = HeadOfMember;
                
                if (ddYear.SelectedIndex == 0) return result;
                result += string.Format(".[{0}]", ddYear.SelectedValue);

                if (ddHalfYear.SelectedIndex == 0) return result;
                result += string.Format(".[Полугодие {0}]", ddHalfYear.SelectedValue);
                
                if (ddQuarter.SelectedIndex == 0) return result;
                result += string.Format(".[Квартал {0}]", ddQuarter.SelectedValue);
                
                if (ddMonth.SelectedIndex == 0) return result;
                result += string.Format(".[{0}]", ddMonth.SelectedValue);

                if (ddDay.SelectedIndex == 0) return result;
                return result + string.Format(".[{0}]", ddDay.SelectedValue);                                
            }
        }
        
    
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        
        

    }
}