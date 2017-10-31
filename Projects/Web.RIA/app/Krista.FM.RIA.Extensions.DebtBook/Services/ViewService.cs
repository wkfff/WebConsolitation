using System.Collections.Generic;
using System.Text;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ViewModel;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public abstract class ViewService : IViewService
    {
        private readonly IDebtBookExtension extension;
        private readonly List<ActionDescriptor> actions;

        protected ViewService(IDebtBookExtension extension)
        {
            this.extension = extension;
            actions = new List<ActionDescriptor>();
        }

        public List<ActionDescriptor> Actions
        {
            get { return actions; }
        }

        public IDebtBookExtension Extension
        {
            get { return extension; }
        }

        public abstract string GetDataFilter();

        public virtual string GetClientScript()
        {
            var sb = new StringBuilder();

            sb.AppendLine("initNewRecord = function(record){{\n{0}\n}}".FormatWith(GetInitNewRecordScript()));

            return sb.ToString();
        }

        protected virtual StringBuilder GetInitNewRecordScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine("record.data.TASKID = -1;");
            return sb;
        }
    }
}
