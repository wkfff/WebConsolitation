using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.SMO.FinSourcePlaning
{
    public class SmoGuarantyService : SmoServerSideObject<IGuarantIssuedService>, IGuarantIssuedService
    {
        public SmoGuarantyService(IGuarantIssuedService serverObject)
			: base(serverObject)
        {

        }

        public ServerLibrary.IFactTable Data
        {
            get { return new SmoFactTable(ServerControl.Data.GetSMOObjectData()); }
        }

        public Dictionary<string, ServerLibrary.IEntityAssociation> Details
        {
            get { return ServerControl.Details; }
        }

        public void FillDebtRemainder(int refVariant)
        {
            ServerControl.FillDebtRemainder(refVariant);
        }
    }
}
