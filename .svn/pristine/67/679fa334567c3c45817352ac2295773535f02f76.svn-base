using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;
using XmlHelper=Krista.FM.Common.Xml.XmlHelper;

namespace Krista.FM.Client.MDXExpert
{

    // Содержит информацию о докабельном окне
    [Serializable]
    public class CustomDockableControlPane : CustomDockablePaneBase
    {
        private XmlNode _reportElement;
        private bool _pinned;

        /// <summary>
        /// Узел для сохранения всех коллекций и свойств элемента отчета
        /// </summary>
        public XmlNode ReportElementNode
        {
            get { return _reportElement; }
            set { _reportElement = value; }
        }

        /// <summary>
        /// Пришпилино ли окно
        /// </summary>
        public bool Pinned
        {
            get { return _pinned; }
            set { _pinned = value; }
        }

        /// <summary>
        /// Достает из ReportElementNode тип элемента отчета
        /// </summary>
        public ReportElementType ElementType
        {
            get 
            {
                ReportElementType result = ReportElementType.eTable;
                string elementType = XmlHelper.GetStringAttrValue(this.ReportElementNode,
                    Common.Consts.reportElemetType, string.Empty);
                if (elementType != string.Empty)
                    result = (ReportElementType)Enum.Parse(typeof(ReportElementType), elementType);
                return result;
            }
        }

        public CustomDockableControlPane()
        {

        }

        /// <summary>
        /// Создание докабельного окна
        /// </summary>
        /// <param name="cubeDefName">имя куба</param>
        /// <param name="paneId">id окна</param>
        /// <param name="floatParentId">id плавающего родителя</param>
        /// <param name="dockParentId">id докабельного родителя</param>
        /// <param name="state">состояние окна</param>
        /// <param name="size">размер окна</param>
        /// <param name="pinned">пришпилено ли окно</param>
        /// <param name="caption">заголовок окна</param>
        /// <param name="minimized">свернуто ли окно</param>
        public CustomDockableControlPane(int paneId, int floatParentId, int dockParentId, DockedState state, 
            Size size, bool pinned, bool minimized, XmlNode reportElement)
        {
            this.paneId = paneId;
            this.floatParentId = floatParentId;
            this.dockParentId = dockParentId;
            this.state = state;
            this.size = size;
            this.minimized = minimized;
            this.Pinned = pinned;
            this.ReportElementNode = reportElement;
        }
    }
}
