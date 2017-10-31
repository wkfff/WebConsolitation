using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Services.ViewServices
{
    public class HiddenColumnsViewService : IViewService
    {
        private readonly List<ActionDescriptor> actions;
        //private readonly IAuthService auth;
        private readonly IEntity Entity;

        private readonly string filter;

        public HiddenColumnsViewService(IEntity Entity, string filter)
        {
            actions = new List<ActionDescriptor>();
            //this.auth = auth;
            this.Entity = Entity;

            this.filter = filter;
        }

        public List<ActionDescriptor> Actions
        {
            get { return actions; }
        }

        public virtual string GetDataFilter()
        {
            return filter;
        }

        public virtual string GetClientScript()
        {
            return "{{\n{0}\n}};".FormatWith(GetHiddenColumnsScript());
        }

        protected virtual string GetHiddenColumnsScript()
        {
            try
            {
                const string fullDbName = "d_OKOPF_OKOPF";
                if (Entity.FullDBName == fullDbName)
                {
                    return @"
                        gridView{0}.colModel.getColumnById('Code').hidden = true;
                      ".FormatWith(fullDbName);
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
