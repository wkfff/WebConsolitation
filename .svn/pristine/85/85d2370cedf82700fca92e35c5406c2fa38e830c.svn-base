using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Ext.Net;

namespace Krista.FM.RIA
{
    public partial class _Default : Page
    {
        public void Page_Load(object sender, System.EventArgs e)
        {
            RIA.Trace.TraceVerbose(
                "Клиент: Chrome = {0}, Gecko = {1}, Gecko3 = {2}, " +
                "IE = {3}, IE6 = {4}, IE7 = {5}, IE8 = {6}, " +
                "Opera = {7}, WebKit = {8}, " +
                "Safari = {9}, Safari3 = {10}, Safari4 = {11}, " +
                "Linux = {12}, Mac = {13}, Windows = {14}, UserAgent = {15}",
                RequestManager.IsChrome, RequestManager.IsGecko, RequestManager.IsGecko3,
                RequestManager.IsIE, RequestManager.IsIE6, RequestManager.IsIE7, RequestManager.IsIE8,
                RequestManager.IsOpera, RequestManager.IsWebKit,
                RequestManager.IsSafari, RequestManager.IsSafari3, RequestManager.IsSafari4,
                RequestManager.IsLinux, RequestManager.IsMac, RequestManager.IsWindows,
                Request.UserAgent);

            // Change the current path so that the Routing handler can correctly interpret
            // the request, then restore the original path so that the OutputCache module
            // can correctly process the response (if caching is enabled).

            string originalPath = Request.Path;
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
            HttpContext.Current.RewritePath(originalPath, false);
        }
    }
}
