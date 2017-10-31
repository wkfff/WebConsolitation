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

    // �������� ���������� � ����������� ����
    [Serializable]
    public class CustomDockableControlPane : CustomDockablePaneBase
    {
        private XmlNode _reportElement;
        private bool _pinned;

        /// <summary>
        /// ���� ��� ���������� ���� ��������� � ������� �������� ������
        /// </summary>
        public XmlNode ReportElementNode
        {
            get { return _reportElement; }
            set { _reportElement = value; }
        }

        /// <summary>
        /// ���������� �� ����
        /// </summary>
        public bool Pinned
        {
            get { return _pinned; }
            set { _pinned = value; }
        }

        /// <summary>
        /// ������� �� ReportElementNode ��� �������� ������
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
        /// �������� ������������ ����
        /// </summary>
        /// <param name="cubeDefName">��� ����</param>
        /// <param name="paneId">id ����</param>
        /// <param name="floatParentId">id ���������� ��������</param>
        /// <param name="dockParentId">id ������������ ��������</param>
        /// <param name="state">��������� ����</param>
        /// <param name="size">������ ����</param>
        /// <param name="pinned">���������� �� ����</param>
        /// <param name="caption">��������� ����</param>
        /// <param name="minimized">�������� �� ����</param>
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
