using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Очистка временных файлов
        /// </summary>
        private void ClearTmpFolder()
        {
            string fPath = Server.MapPath("TemporaryImages");

            //Тут разная фигня может быть с правами, нужно разбираться и очищать умнее
            try
            {
                
                if (Directory.Exists(fPath))
                {
                    foreach (string file_path in Directory.GetFiles(fPath))
	                {
	                    try 
	                    {	        
	                        File.Delete(file_path);
		
	                    }
	                    catch 
	                    {
	                    }
		 
	                }
                                        
                }
                else
                {
                    Directory.CreateDirectory(fPath);
                }                
            }
            catch
            {
            }                       
        }
    
        protected void Application_Start(object sender, EventArgs e)
        {
			ClearTmpFolder();
        }

        protected void Application_End(object sender, EventArgs e)
        {
		}

        protected void Session_Start(object sender, EventArgs e)
        {
		}

        protected void Session_End(object sender, EventArgs e)
        {

        }
    }
}