using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

namespace Krista.FM.Server.Dashboards.Core
{
	public class GadgetControlBase : UserControl, IWebPart
	{
		protected virtual void Page_Load(object sender, EventArgs e)
		{
            if (Request.Params["paramlist"] != null)
            {
                string paramlist = Request.Params["paramlist"];
                CustomParams.InitializeParamsFromList(paramlist);
            }

		    if (Parent is GenericWebPart)
			{
				GenericWebPart wp = (GenericWebPart) Parent;
				wp.AllowClose = false;
				wp.AllowHide = false;
				wp.AllowMinimize = false;
			}
			else
			{
				if (Page.Form.Controls[1].Controls[0].ID == "RerortReference")
				{
					((HtmlAnchor) Page.Form.Controls[1].Controls[0]).HRef = TitleUrl;
				}
				//Page.Title = Title;
			}
		}
                
        // Осталось убрать зависимости по UserParams
        // и можно будет избавиться от этого поля.
		protected CustomReportPage CustumReportPage
		{
			get
			{
			    Control control = this;
                while (control != null)
                {
                    if (control is CustomReportPage)
                    {
                        return (CustomReportPage)control;
                    }

                    control = control.Parent;
                }

				return null;
			}
		}

        protected CustomReportPage GetCustomReportPage(Control control)
        {
            if (control.Parent != null)
            {
                if (control.Parent is CustomReportPage)
                {
                    return ((CustomReportPage)control.Parent);
                }
                else
                {
                    return GetCustomReportPage(control.Parent);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Коллекция пользовательских параметров уровня сесси
        /// </summary>
        private CustomParams userParams;
        public CustomParams UserParams
        {
            get
            {
                if (userParams == null)
                {
                    userParams = new CustomParams();
                }
                return userParams;
            }
        }

		#region IWebPart Members

		public virtual string CatalogIconImageUrl
		{
			get
			{
				return String.Empty;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public virtual string Description
		{
			get
			{
				return String.Empty;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public virtual string Subtitle
		{
			get
			{
				return String.Empty;
			}
		}

		public virtual string Title
		{
			get
			{
				return String.Empty;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public virtual string TitleIconImageUrl
		{
			get
			{
				return String.Empty;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public virtual string TitleUrl
		{
			get
			{
				return String.Empty;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion
	}
}
