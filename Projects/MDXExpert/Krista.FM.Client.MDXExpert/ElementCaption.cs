using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;

using Infragistics.Win.FormattedLinkLabel;
using Infragistics.Win;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Client.MDXExpert.Common.Converts;

namespace Krista.FM.Client.MDXExpert
{

    public class ElementCaption : ElementTextEditor
    {
        public ElementCaption(CustomReportElement report, Panel parentPanel, Splitter editorSplitter)
            : base(report, parentPanel, editorSplitter)
        {
        }

        /// <summary>
        /// «агружаем некие настройки заголовка
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Load(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            base.Load(collectionNode);

            this.Height = XmlHelper.GetIntAttrValue(collectionNode, Consts.heights, 25);
        }

        /// <summary>
        /// —охран€ем настройки заголовка
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            base.Save(collectionNode);

            XmlHelper.SetAttribute(collectionNode, Consts.heights, this.Height.ToString());
        }

        /// <summary>
        /// ¬ысота
        /// </summary>
        public new int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                //“.к. высота контрола зависит от высоты панели на которой он лежит, пришлость переопределить
                //свойство, и выставл€ть данный признак панели а не контролу (у заголовка он измен€етс€ автоматом)
                if (this.ParentPanel != null)
                    this.ParentPanel.Height = value;
            }
        }
    }
}
