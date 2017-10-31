using System.Windows.Forms;
using System.Xml;

namespace Krista.FM.Common.TaskParamEditors
{
    internal enum MemberNodeImage
    {
        RoyalBlue = 0,
        SimpleBlue,
        RoyalGreen,
        SimpleGreen,
        RoyalRed,
        SimpleRed,
        RoyalGrey,
        SimpleGrey,
        White
    }

    internal class MembersTreeNode : TreeNode
    {
        private NodeInfluence influence;
        private NodeInfluence underInfluence;
        private bool inCheckedLevel;
        //private bool 


        internal MembersTreeNode(string text, XmlNode xmlNode)
            : base(text)
        {
            DomNode = xmlNode;
        }


        #region Свойства


        public new bool Checked
        {
            get { return base.Checked; }
            set
            {
                base.Checked = value;
            }
        }


        /// <summary>
        /// Влияние узла на потомков
        /// </summary>
        internal NodeInfluence Influence
        {
            get
            {
                return influence;
            }
            set
            {
                NodeInfluence oldInfluence = influence;
                influence = value;
                //DomElement.SetAttribute(Attr.Influence, ((int)influence).ToString());

                // если мы имели белый шарик и делаем из него другой, а сверху на него действует зеленый или красный, 
                // то необходимо поставить галку
                if (influence == NodeInfluence.Exclude)
                    Checked = false;
                else if ((oldInfluence == NodeInfluence.Exclude) && IsUnderInfluence)
                    Checked = true;

                var notLoading = !MembersTree.IsLoading;
                if (notLoading)
                    MembersTree.IsLoading = true;

                InfluenceChildren(this, influence, oldInfluence);

                if (notLoading)
                    MembersTree.IsLoading = false;
                
                if (!MembersTree.IsLoading)
                {
                    ReflexParent();
                    UpdateImage();
                }
            }
        }

        /// <summary>
        /// Нахождение под влиянием вышестоящего узла
        /// </summary>
        internal NodeInfluence UnderInfluence
        {
            get { return underInfluence; }
        }

        /// <summary>
        /// Действует ли на узел красный или зеленый шарик
        /// </summary>
        internal bool IsUnderInfluence 
        { 
            get { return (UnderInfluence == NodeInfluence.Children) || (UnderInfluence == NodeInfluence.Descendants); }
        }

        /// <summary>
        /// Ссылка на исходный узел xml-документа, 1 вариант
        /// </summary>
        internal XmlElement DomElement { get; set; }

        /// <summary>
        /// Ссылка на исходный узел xml-документа, 2 вариант
        /// </summary>
        internal XmlNode DomNode
        {
            get { return (XmlNode) DomElement; }
            set { DomElement = (XmlElement) value; }
        }

        /// <summary>
        /// Принадлежность узла отмеченному уровню
        /// </summary>
        internal bool InCheckedLevel
        {
            get
            {
                return inCheckedLevel;
            }

            set 
            { 
                inCheckedLevel = value; 
                if (!MembersTree.IsLoading) 
                    UpdateImage();
            }
        }

        internal MembersTree MembersTree
        {
            get { return (MembersTree) Tag; }

            set { Tag = value; }

        }

        #endregion

        /// <summary>
        /// обновляет признак "нахождения под влиянием сверху"
        /// </summary>
        internal void NewParentInfluence()
        {
            NodeInfluence parentInfluence = NodeInfluence.None;
            NodeInfluence parentUnderInfluence = NodeInfluence.None;
            if (Parent != null)
            {
                parentInfluence = ((MembersTreeNode) Parent).Influence;
                parentUnderInfluence = ((MembersTreeNode) Parent).UnderInfluence;
            }

            switch (parentUnderInfluence)
            {
                case NodeInfluence.None:
                    underInfluence = parentInfluence;
                    break;

                case NodeInfluence.Children:
                    underInfluence = parentInfluence;
                    break;

                case NodeInfluence.Descendants:
                    if (parentInfluence == NodeInfluence.Exclude)
                        underInfluence = NodeInfluence.Exclude;
                    else
                        underInfluence = NodeInfluence.Descendants;
                    break;

                case NodeInfluence.Exclude:
                    underInfluence = NodeInfluence.Exclude;
                    break;
            }

            //if (DomElement != null)
              //  DomElement.SetAttribute(Attr.UnderInfluence, ((int)underInfluence).ToString());            

        }


        /// <summary>
        /// Обновляет ImageIndex
        /// </summary>
        internal void UpdateImage()
        {
            //узлы с невыделенных уровней и без зависимостей или под белым шариком будут серенькими
            if (!(InCheckedLevel || ((Influence == NodeInfluence.Children) || (Influence == NodeInfluence.Descendants))) || (UnderInfluence == NodeInfluence.Exclude))
            {
                if (Influence == NodeInfluence.Exclude)
                    ImageIndex = (int)MemberNodeImage.White;
                else
                {
                    ImageIndex = GetCheckedCount(true) > 0
                                     ? (int)MemberNodeImage.RoyalGrey
                                     : (int)MemberNodeImage.SimpleGrey;
                }
            }
            else
            {
                if (GetCheckedCount(true) > 0)
                {
                    switch (Influence)
                    {
                        case NodeInfluence.None:
                            ImageIndex = (int) MemberNodeImage.RoyalBlue;
                            break;

                        case NodeInfluence.Children:
                            ImageIndex = (int) MemberNodeImage.RoyalGreen;
                            break;

                        case NodeInfluence.Descendants:
                            ImageIndex = (int) MemberNodeImage.RoyalRed;
                            break;

                        case NodeInfluence.Exclude:
                            ImageIndex = (int) MemberNodeImage.White;
                            break;
                    }
                }
                else
                {
                    switch (Influence)
                    {
                        case NodeInfluence.None:
                            ImageIndex = (int)MemberNodeImage.SimpleBlue;
                            break;

                        case NodeInfluence.Children:
                            ImageIndex = (int)MemberNodeImage.SimpleGreen;
                            break;

                        case NodeInfluence.Descendants:
                            ImageIndex = (int)MemberNodeImage.SimpleRed;
                            break;

                        case NodeInfluence.Exclude:
                            ImageIndex = (int)MemberNodeImage.White;
                            break;
                    }
                }
            }
            SelectedImageIndex = ImageIndex;

            foreach (MembersTreeNode node in Nodes)
                node.UpdateImage();
        }

        /// <summary>
        /// Возвращает число отмеченных потомков
        /// </summary>
        /// <param name="anywhere">False для дочерних, true для всех</param>
        /// <returns></returns>
        internal int GetCheckedCount(bool anywhere)
        {
            int result = 0;
            foreach (MembersTreeNode childNode in Nodes)
            {
                if (childNode.Checked)
                    result++;
                if (anywhere)
                    result += childNode.GetCheckedCount(anywhere);
            }
            return result;
        }

        internal int GetUncheckedCount(bool anywhere)
        {
            int result = 0;
            foreach (MembersTreeNode childNode in Nodes)
            {
                if (!childNode.Checked && (childNode.Influence != NodeInfluence.Exclude) && (childNode.UnderInfluence != NodeInfluence.Exclude))
                    result++;
                if (anywhere)
                    result += childNode.GetUncheckedCount(anywhere);
            }
            return result;
        }


        private void InfluenceChildren(MembersTreeNode node, NodeInfluence newInfluence, NodeInfluence oldInfluence)
        {
            foreach (MembersTreeNode childNode in node.Nodes)
            {
                childNode.NewParentInfluence();
                if ((childNode.Influence == NodeInfluence.Children) || (childNode.Influence == NodeInfluence.Descendants) &&
                    ((childNode.UnderInfluence == NodeInfluence.Descendants) || (childNode.UnderInfluence == NodeInfluence.Exclude)))
                    childNode.Influence = NodeInfluence.None;

                childNode.Checked = (childNode.Checked || (childNode.IsUnderInfluence)) &&
                                    (childNode.Influence != NodeInfluence.Exclude) &&
                                    (childNode.UnderInfluence != NodeInfluence.Exclude);

                if ((oldInfluence == NodeInfluence.Descendants) || (oldInfluence == NodeInfluence.Exclude) || 
                    (newInfluence == NodeInfluence.Descendants) || (newInfluence == NodeInfluence.Exclude))
                    InfluenceChildren(childNode, newInfluence, oldInfluence);

                childNode.UpdateImage();

                if (!(node.MembersTree).IsLoading && (childNode.NextNode == null)) 
                    childNode.ReflexParent();
            }
        }

        private void ReflexParent()
        {
            MembersTreeNode parentNode = (MembersTreeNode) Parent;
            if (parentNode == null)
                return;
            
            // если где-то под зеленым или красным шариком сняли галку, то он становится синим
            if ((parentNode.Influence == NodeInfluence.Children) || (parentNode.Influence == NodeInfluence.Descendants))
                if (parentNode.GetUncheckedCount(parentNode.Influence == NodeInfluence.Descendants) > 0)
                        parentNode.Influence = NodeInfluence.None;
                
            if (!MembersTree.IsLoading)
                parentNode.UpdateImage();

            parentNode.ReflexParent();
        }

        internal void CheckChildren(bool checkValue, bool checkAll)
        {
            foreach (MembersTreeNode childNode in Nodes)
            {
                if (!((childNode.Influence == NodeInfluence.Exclude) && checkValue))
                    childNode.Checked = checkValue;
                if (checkAll)
                    childNode.CheckChildren(checkValue, checkAll);
                if (!MembersTree.IsLoading && (childNode.NextNode == null))
                    childNode.ReflexParent();
            }
        }

        internal void ToggleCheck()
        {
            if (UnderInfluence == NodeInfluence.Exclude)
                return;
            if ((Influence != NodeInfluence.Exclude) && InCheckedLevel)
            {
                Checked = !Checked;
                //XmlHelper.SetAttribute(DomNode, Attr.Checked, Checked.ToString().ToLower());
                if (!MembersTree.IsLoading)
                    ReflexParent();
            }
        }

    }
}