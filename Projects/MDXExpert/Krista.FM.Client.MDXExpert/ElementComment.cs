using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert
{
    public class ElementComment : ElementTextEditor
    {
        private Panel commentAndReportPanel;
        private CommentPlace _place;

        public ElementComment(CustomReportElement report, Panel parentPanel, Splitter editorSplitter)
            : base(report, parentPanel, editorSplitter)
        {
        }

        /// <summary>
        /// ��������� ����� ��������� ����������� �������
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Load(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            base.Load(collectionNode);

            this.Place = (CommentPlace)XmlHelper.GetIntAttrValue(collectionNode, Consts.place, 0);
            this.Proportion = XmlHelper.GetFloatAttrValue(collectionNode, Consts.proportion, 20f);

        }

        /// <summary>
        /// ��������� ��������� ����������� �������
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            base.Save(collectionNode);

            XmlHelper.SetAttribute(collectionNode, Consts.place, ((int)this.Place).ToString());
            XmlHelper.SetAttribute(collectionNode, Consts.proportion, this.Proportion.ToString());
        }

        /// <summary>
        /// ���������� ����������� ������ �� ��������
        /// </summary>
        /// <param name="commAndGridPanel">������ �� ������� ������������� ����������� � ���� �������</param>
        public void SetLinks(Panel commAndReportPanel)
        {
            this.commentAndReportPanel = commAndReportPanel;
        }

        /// <summary>
        /// ��������� ��������� ����� ����������� �� ������ � ��������
        /// </summary>
        public float Proportion
        {
            get 
            {
                float result = 20;
                switch (this.Place)
                {
                    case CommentPlace.Top:
                    case CommentPlace.Bottom:
                        {
                            result = this.ParentPanel.Height * 100 / (float)this.commentAndReportPanel.Height;
                            break;
                        }
                    case CommentPlace.Left:
                    case CommentPlace.Right:
                        {
                            result = this.ParentPanel.Width * 100 / (float)this.commentAndReportPanel.Width;
                            break;
                        }
                }
                return result;
            }
            set 
            {
                switch (this.Place)
                {
                    case CommentPlace.Top:
                    case CommentPlace.Bottom:
                        {
                            this.ParentPanel.Height = (int)(this.commentAndReportPanel.Height / 100f * value);
                            break;
                        }
                    case CommentPlace.Left:
                    case CommentPlace.Right:
                        {
                            this.ParentPanel.Width = (int)(this.commentAndReportPanel.Width / 100f * value);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// ����� ��������� ����������� �������
        /// </summary>
        public CommentPlace Place
        {
            get { return _place; }
            set 
            {
                if (_place != value)
                {
                    //�������� ��������� �����������
                    float proportion = this.Proportion;
                    switch (value)
                    {
                        case CommentPlace.Top:
                            {
                                this.ParentPanel.Dock = DockStyle.Top;
                                this.Splitter.Dock = DockStyle.Top;
                                break;
                            }
                        case CommentPlace.Right:
                            {
                                this.ParentPanel.Dock = DockStyle.Right;
                                this.Splitter.Dock = DockStyle.Right;
                                break;
                            }
                        case CommentPlace.Bottom:
                            {
                                this.ParentPanel.Dock = DockStyle.Bottom;
                                this.Splitter.Dock = DockStyle.Bottom;
                                break;
                            }
                        case CommentPlace.Left:
                            {
                                this.ParentPanel.Dock = DockStyle.Left;
                                this.Splitter.Dock = DockStyle.Left;
                                break;
                            }
                    }
                    _place = value;
                    //�������� ��������� ����������� ������ ���� ��� ����������� �����
                    if (this.Visible)
                        this.Proportion = proportion;
                }
            }
        }
    }

    /// <summary>
    /// ����� ������������ �����������
    /// </summary>
    public enum CommentPlace
    {
        /// <summary>
        /// ������
        /// </summary>
        Top,
        /// <summary>
        /// ������
        /// </summary>
        Right,
        /// <summary>
        /// �����
        /// </summary>
        Bottom,
        /// <summary>
        /// �����
        /// </summary>
        Left
    }
}
