using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public abstract class DebtBookViewService : ViewService
    {
        private int tabType;

        protected DebtBookViewService(IDebtBookExtension extension, int tabType) 
            : base(extension)
        {
            this.tabType = tabType;
        }

        protected override StringBuilder GetInitNewRecordScript()
        {
            var sb = base.GetInitNewRecordScript();
            sb.AppendLine("var ex = Extension.View.getWorkbench().extensions.DebtBook;");
            sb.AppendLine("record.data.REFVARIANT = ex.selectedVariantId;");
            sb.AppendLine("record.data.SOURCEID = ex.currentSourceId;");

            if ((Extension.UserRegionType == UserRegionType.Subject && tabType != 0) ||
                (Extension.UserRegionType == UserRegionType.Region && tabType != 2) ||
                (Extension.UserRegionType == UserRegionType.Town && tabType != 2) ||
                (Extension.UserRegionType == UserRegionType.Settlement))
            {
                sb.AppendLine("record.data.REFREGION = '';");
                sb.AppendLine("record.data.LP_REFREGION = '';");
            }
            else
            {
                sb.AppendLine("record.data.REFREGION = ex.userRegionId;");
                sb.AppendLine("record.data.LP_REFREGION = ex.userRegionName;");
            }

            sb.AppendLine("record.data.FROMFINSOURCE = 0;");
            return sb;
        }
    }
}
