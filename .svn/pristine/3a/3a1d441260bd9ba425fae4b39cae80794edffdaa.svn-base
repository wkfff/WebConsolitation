using System;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers
{
    public abstract class Pumper
    {
        public abstract void Pump(D_CD_Report report, PamperActionsEnum actions);

        protected void CheckCondition(bool cond, string format, params object[] args)
        {
            if (!cond)
            {
                throw new PumperPreconditionException(String.Format(format, args));
            }
        }
    }
}
