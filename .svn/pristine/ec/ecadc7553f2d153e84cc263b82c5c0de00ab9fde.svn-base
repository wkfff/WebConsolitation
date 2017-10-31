using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Ext.Net;

[assembly: WebResource("Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel.js.ExcelLikeSelectionModel.js", "text/javascript")]
[assembly: WebResource("Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel.js.FixDateField.js", "text/javascript")]

namespace Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel
{
    /// <summary>
    /// Специализированная модель для редактирования данных грида как в Excel.
    /// </summary>
    public class ExcelLikeSelectionModel : CellSelectionModel
    {
        [Category("0. About")]
        [Description("")]
        public override string InstanceOf
        {
            get
            {
                return XType;
            }
        }

        public override string XType
        {
            get { return "Ext.ux.ExcelLikeSelectionModel"; }
        }

        protected override List<ResourceItem> Resources
        {
            get
            {
                List<ResourceItem> baseList = base.Resources;
                baseList.Capacity += 2;
                baseList.Add(new ClientScriptItem(
                        typeof(ExcelLikeSelectionModel),
                        "Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel.js.ExcelLikeSelectionModel.js",
                        "ux/extensions/extnet/TreeGrid.js"));
                baseList.Add(new ClientScriptItem(
                        typeof(ExcelLikeSelectionModel),
                        "Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel.js.FixDateField.js",
                        "ux/extensions/extnet/FixDateField.js"));
                return baseList;
            }
        }
    }
}

