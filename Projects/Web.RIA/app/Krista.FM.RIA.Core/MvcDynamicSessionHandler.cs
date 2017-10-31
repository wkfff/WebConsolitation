using System;
using System.Web;

namespace Krista.FM.RIA.Core
{
    internal class MvcDynamicSessionHandler : IHttpAsyncHandler 
    {
        private readonly IHttpAsyncHandler originalHandler;

        public MvcDynamicSessionHandler(IHttpAsyncHandler originalHandler) 
        {
            this.originalHandler = originalHandler;
        }

        public bool IsReusable
        {
            get { return originalHandler.IsReusable; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) 
        {
            return originalHandler.BeginProcessRequest(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result) 
        {
            originalHandler.EndProcessRequest(result);
        }

        public void ProcessRequest(HttpContext context) 
        {
            originalHandler.ProcessRequest(context);
        }
    }
}
