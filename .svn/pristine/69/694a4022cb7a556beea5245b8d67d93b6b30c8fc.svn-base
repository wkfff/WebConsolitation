using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;

namespace Krista.FM.RIA.Core.Tests.Helpers
{
    public sealed class MockHttpContext
    {
        // NOTE: This code is based on the following article:
        // http://righteousindignation.gotdns.org/blog/archive/2004/04/13/149.aspx
        private const string ContextKeyAspSession = "AspSession";
        private const string ThreadDataKeyAppPath = ".appPath";
        private const string ThreadDataKeyAppPathValue = "c:\\inetpub\\wwwroot\\webapp\\";
        private const string ThreadDataKeyAppVPath = ".appVPath";
        private const string ThreadDataKeyAppVPathValue = "/webapp";
        private const string WorkerRequestPage = "default.aspx";

        private readonly HttpContext context;

        public MockHttpContext(bool isSecure)
            : this()
        {
            Thread.GetDomain().SetData(ThreadDataKeyAppPath, ThreadDataKeyAppPathValue);
            Thread.GetDomain().SetData(ThreadDataKeyAppVPath, ThreadDataKeyAppVPathValue);
            SimpleWorkerRequest request = new WorkerRequest(WorkerRequestPage, string.Empty, new StringWriter(), isSecure);
            context = new HttpContext(request);

            HttpSessionStateContainer container = new HttpSessionStateContainer(
                Guid.NewGuid().ToString("N"), 
                new SessionStateItemCollection(), 
                new HttpStaticObjectsCollection(),
                5, 
                true, 
                HttpCookieMode.AutoDetect, 
                SessionStateMode.InProc, 
                false);

            HttpSessionState state = Activator.CreateInstance(
                 typeof(HttpSessionState),
                 BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                 null,
                 new object[] { container }, 
                 CultureInfo.CurrentCulture) as HttpSessionState;
            context.Items[ContextKeyAspSession] = state;
        }

        private MockHttpContext()
        {
        }

        public HttpContext Context
        {
            get { return context; }
        }

        private class WorkerRequest : SimpleWorkerRequest
        {
            private readonly bool isSecure;

            public WorkerRequest(string page, string query, TextWriter output, bool isSecure)
                : base(page, query, output)
            {
                this.isSecure = isSecure;
            }

            public override bool IsSecure()
            {
                return isSecure;
            }
        }
    }
}
