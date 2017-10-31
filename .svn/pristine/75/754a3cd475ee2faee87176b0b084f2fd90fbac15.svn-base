using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services
{
    public class GuarantIncomeService : FinSourceBaseService, IGuarantIncomeService
    {
        private static GuarantIncomeService instance;

        public static GuarantIncomeService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GuarantIncomeService(null);
                }
                return instance;
            }
        }

		private GuarantIncomeService(ServerSideObject owner)
			: base(owner)
        {
        }

        public override IFactTable Data
        {
            get
            {
                return FinSourcePlanningFace.Instance.Scheme.FactTables["8085e515-d224-4725-ada9-855d9d83bb8c"];
            }
        }
    }
}
