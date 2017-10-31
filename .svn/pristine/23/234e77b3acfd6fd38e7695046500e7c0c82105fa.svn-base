using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services
{
    public class СreditIssuedService : FinSourceBaseService, IСreditIssuedService
    {
        private static СreditIssuedService instance;

        public static СreditIssuedService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new СreditIssuedService(null);
                }
                return instance;
            }
        }

		private СreditIssuedService(ServerSideObject owner)
			: base(owner)
        {
        }

        public override IFactTable Data
        {
            get
            {
                return FinSourcePlanningFace.Instance.Scheme.FactTables["fb029d1d-e648-46b4-8a1f-bff21ea0fbf5"];
            }
        }
    }
}
