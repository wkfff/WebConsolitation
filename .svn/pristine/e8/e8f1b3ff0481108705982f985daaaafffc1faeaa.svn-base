using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.SMO
{
	public class SmoСreditIncomeService : SmoServerSideObject<IСreditIncomeService>, IСreditIncomeService
	{
		public SmoСreditIncomeService(IСreditIncomeService serverObject)
			: base(serverObject)
        {
        }


		#region IFinSourceBaseService Members

		public Krista.FM.ServerLibrary.IFactTable Data
		{
			get { return new SmoFactTable(ServerControl.Data.GetSMOObjectData()); }
		}

		public Dictionary<string, Krista.FM.ServerLibrary.IEntityAssociation> Details
		{
			get { return ServerControl.Details; }
		}

        public void FillDebtRemainder(int refVariant)
        {
            ServerControl.FillDebtRemainder(refVariant);
        }

		#endregion
	}
}
