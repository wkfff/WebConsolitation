using System.Collections.Generic;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Services.ViewServices
{
    public class UserPpoFilterViewService : IViewService
    {
        private readonly List<ActionDescriptor> actions;
        private readonly IAuthService auth;

        public UserPpoFilterViewService(IAuthService auth)
        {
            actions = new List<ActionDescriptor>();
            this.auth = auth; 
        }

        public List<ActionDescriptor> Actions
        {
            get { return actions; }
        }

        public virtual string GetDataFilter()
        {
            try
            {
                if (!auth.IsAdmin())
                    return "(RefOrgPPO = {0})".FormatWith(auth.Profile.RefUchr.RefOrgPPO.ID);
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public virtual string GetClientScript()
        {
            return "function StoreBeforeSave(store, params) {{\n{0}\n}};".FormatWith(GetInitNewRecordScript());
        }

        protected virtual string GetInitNewRecordScript()
        {
            try
            {
                return @"
                        var Records = store.getModifiedRecords();
                        for(var i = 0; i < Records.length; i++)
                        {{
                          if (!Records[i].data.REFORGPPO)
                           Records[i].data.REFORGPPO = {0};
                        }}
                      ".FormatWith(auth.Profile.RefUchr.RefOrgPPO.ID);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
