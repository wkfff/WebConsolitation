using System.Web.Mvc;

using Antlr4.StringTemplate;

namespace Krista.FM.RIA.Core.TemplateViewEngine
{
    /// <summary>
    /// A view that renders a StringTemplate
    /// </summary>
    public class TemplateView : IView
    {
        /// <summary>
        /// The template associated with this view
        /// </summary>
        private readonly Template template;

        public TemplateView(string templateString)
        {
            if (TemplateViewEngine.Group == null)
            {
                new TemplateViewEngine();    
            }

            template = new Template(TemplateViewEngine.Group, templateString);
        }

        public TemplateView(Template template)
        {
            this.template = template;
        }

        /// <summary>
        /// Renders the string template
        /// </summary>
        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            // persist the controller's viewdata in the template's attribute store
            foreach (var item in viewContext.Controller.ViewData)
            {
                template.Add(item.Key, item.Value);
            }

            // persist the context (so the template has access to it)
            template.Add("context", viewContext.HttpContext);

            // render the template to the text writer
            var noIndentWriter = new NoIndentWriter(writer);
            template.Write(noIndentWriter);
        }
    }
}
