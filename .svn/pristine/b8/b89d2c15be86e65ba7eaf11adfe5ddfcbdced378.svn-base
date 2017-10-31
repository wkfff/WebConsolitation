using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning
{
	public abstract class FinSourceBaseService : ServerSideObject, IFinSourceBaseService
    {
		public FinSourceBaseService(ServerSideObject owner)
			: base (owner)
		{
		}

        #region IFinSourceBaseService Members

        public abstract IFactTable Data
        {
            get;
        }

        public Dictionary<string, IEntityAssociation> Details
        {
            get
            {
                Dictionary<string, IEntityAssociation> tables = new Dictionary<string, IEntityAssociation>();

                foreach (IEntityAssociation item in Data.Associated.Values)
                {
                    if (item.AssociationClassType == AssociationClassTypes.MasterDetail)
                    {
                        tables.Add(item.ObjectKey, item);
                    }
                }

                return tables;
            }
        }

        #endregion
    }
}
