using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using Antlr4.StringTemplate;

namespace Krista.FM.RIA.Core.TemplateViewEngine
{
    /// <summary>
    /// The ViewEngine for StringTemplate Views
    /// </summary>
    public class TemplateViewEngine : IViewEngine
    {
        public TemplateViewEngine()
        {
            Group = new TemplateGroup('$', '$');
        }

        /// <summary>
        /// The default group that will hold the cached templates
        /// </summary>
        public static TemplateGroup Group { get; private set; }

        /// <summary>
        /// Locates a view.
        /// </summary>
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return LoadView(controllerContext, partialViewName);
        }

        /// <summary>
        /// Locates a view
        /// </summary>
        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return LoadView(controllerContext, viewName);
        }

        /// <summary>
        /// Not used. String templates are cached by the static group object.
        /// </summary>
        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }

        /// <summary>
        /// Loads a view instance from the Group object
        /// </summary>
        /// <param name="controllerContext">The calling controller</param>
        /// <param name="viewName">The name of the view</param>
        /// <returns>a ViewEngineResult</returns>
        private ViewEngineResult LoadView(ControllerContext controllerContext, string viewName)
        {
            // load template from loader
            Template template;
            var match = Regex.Match(viewName, "^~/?(.*)");
            if (match.Success)
            {
                template = Group.GetInstanceOf(match.Groups[1].Value);
            }
            else
            {
                var controllerName = controllerContext.Controller.GetType().Name.Replace("Controller", String.Empty);
                template = Group.GetInstanceOf(string.Format("{0}/{1}", controllerName, viewName));
            }

            if (template == null)
            {
                template = new Template(String.Empty);
            }

            return new ViewEngineResult(new TemplateView(template), this);
        }
    }
}