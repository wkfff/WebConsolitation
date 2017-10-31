using System.IO;
using System.Web.Mvc;

namespace Krista.FM.RIA.Core.TemplateViewEngine
{
    public class TemplateViewResult : ViewResult
    {
        private readonly string viewName;

        public TemplateViewResult(string viewName)
        {
            this.viewName = viewName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var filePath = context.HttpContext.Server.MapPath(viewName);
            if (File.Exists(filePath))
            {
                using (var reader = File.OpenText(filePath))
                {
                    View = new TemplateView(reader.ReadToEnd());
                }
            }
            else
            {
                var vpp = new AssemblyResourceVirtualPathProvider();
                var vf = vpp.GetFile(viewName);
                using (var stream = vf.Open())
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);
                    stream.Close();

                    var html = System.Text.Encoding.UTF8.GetString(buffer);
                    View = new TemplateView(html);
                }
            }

            if (View != null)
            {
                TextWriter writer = context.HttpContext.Response.Output;
                ViewContext viewContext = new ViewContext(context, View, ViewData, TempData, writer);
                View.Render(viewContext, writer);
            }
        }
    }
}