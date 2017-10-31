using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOMarkDetailsView : View
    {
        private readonly int markId;
        private readonly int categoryId;
        private readonly int periodId;
        private readonly int okeiCode;
        private readonly IFO41Extension extension;
        private readonly string markName;

        public HMAOMarkDetailsView(IFO41Extension extension, string markName, int markId, int categoryId, int periodId, int okeiCode)
        {
            this.extension = extension;
            this.markName = markName;
            this.markId = markId;
            this.categoryId = categoryId;
            this.periodId = periodId;
            this.okeiCode = okeiCode;
        }

        public override List<Component> Build(ViewPage page)
        {
            var grid = new MarkDetails(extension, markName, markId, categoryId, periodId, okeiCode);
            grid.InitAll(page);
            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportMarkDetails",
                    Items = { new BorderLayout { Center = { Items = { grid } } } }
                }
            };
        }
    }
}
