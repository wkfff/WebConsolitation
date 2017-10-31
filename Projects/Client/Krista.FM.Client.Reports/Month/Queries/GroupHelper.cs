using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports.Month.Queries
{
    public class GroupHelper
    {
        private string key;
        public string Prefix { get; set; }
        public IEntity Entity { get; set; }

        public string EntityKey
        {
            get { return key; }
            set
            {
                key = value; Entity = ConvertorSchemeLink.GetEntity(value);
            }
        }

        public string FullPrefix
        {
            get { return String.Format("{0}.", Prefix); }
        }

        public GroupHelper()
        {
            key = String.Empty;
            Prefix = String.Empty;
        }
    }
}
