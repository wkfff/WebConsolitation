using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Principal;

namespace Krista.FM.RIA.Core.Gui
{
    public class View : Control
    {
        public View()
        {
            Params = new Dictionary<string, string>();

            // Установка атрибутов пользователя в HTTPContext сессии, чтобы им можно было пользоваться в наследуемых ViewBuilders
            Resolver.Get<IPrincipalProvider>().SetBasePrincipal(); 
        }

        public string Title { get; set; }

        public Type Type { get; set; }
        
        public string Config { get; set; }
        
        public string Url { get; set; }
        
        public Dictionary<string, string> Params { get; private set; }

        public BasePrincipal User
        {
            get { return (BasePrincipal)System.Web.HttpContext.Current.User; }
        }

        public override List<Component> Build(ViewPage page)
        {
            Panel panel = new Panel
            {
                ID = Id,
                Title = Title
            };
            panel.AutoLoad.Url = Url;
            panel.AutoLoad.Mode = LoadMode.IFrame;

            return new List<Component> { panel };
        }
    }
}
