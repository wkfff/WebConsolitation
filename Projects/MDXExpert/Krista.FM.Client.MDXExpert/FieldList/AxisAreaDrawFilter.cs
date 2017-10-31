using System.Drawing;
using System.Globalization;
using Infragistics.Win.UltraWinTree;

namespace Krista.FM.Client.MDXExpert.FieldList
{
    //Перечисление возможных положений выброски 
    [System.Flags]
    public enum DropLinePositionEnum
    {
        None = 0,
        AboveNode = 1,
        BelowNode = 2,
        All = AboveNode | BelowNode
    }

    public class AxisAreaDrawFilterClass : Infragistics.Win.IUIElementDrawFilter
    {

        public event System.EventHandler Invalidate;


        public event QueryStateAllowedForNodeEventHandler QueryStateAllowedForNode;
        public delegate void QueryStateAllowedForNodeEventHandler(object sender, QueryStateAllowedForNodeEventArgs e);

        public class QueryStateAllowedForNodeEventArgs : System.EventArgs
        {
            private UltraTreeNode node;
            private DropLinePositionEnum dropLinePosition;
            private DropLinePositionEnum statesAllowed;

            public UltraTreeNode Node
            {
                get { return node;}
                set { node = value; }
            }

            public DropLinePositionEnum DropLinePosition
            {
                get { return dropLinePosition; }
                set { dropLinePosition = value; }
            }

            public DropLinePositionEnum StatesAllowed
            {
                get { return statesAllowed; }
                set { statesAllowed = value; }
            }
        }


        private string _description;
        private AxisArea _area;

        public AxisArea Area
        {
            get { return this._area; }
            set { this._area = value; }
        }

        public string Description
        {
            get { return this.Area.Description; }
        }

        public AxisAreaDrawFilterClass()
        {
            InitProperties();
        }

        //Инициализация свойств по умолчанию
        private void InitProperties()
        {
            mvarDropHighLightNode = null;
            mvarDropLinePosition = DropLinePositionEnum.None;
            mvarDropLineColor = System.Drawing.SystemColors.ControlText;
            mvarEdgeSensitivity = 0;
            mvarDropLineWidth = 2;
        }


        //Очистка
        public void Dispose()
        {
            mvarDropHighLightNode = null;
        }

        //Ссылка на узел, над которым находится курсор
        private UltraTreeNode mvarDropHighLightNode;
        public UltraTreeNode DropHightLightNode
        {
            get
            {
                return mvarDropHighLightNode;
            }
            set
            {
                //Если узел установлен в то же самое значение, то просто выходим
                if (mvarDropHighLightNode.Equals(value))
                {
                    return;
                }
                mvarDropHighLightNode = value;

                PositionChanged();
            }
        }

        private DropLinePositionEnum mvarDropLinePosition;
        public DropLinePositionEnum DropLinePosition
        {
            get
            {
                return mvarDropLinePosition;
            }
            set
            {
                //Если позиция осталась прежней, просто выходим
                if (mvarDropLinePosition == value)
                {
                    return;
                }
                mvarDropLinePosition = value;

                PositionChanged();
            }
        }

        //Толщина линии
        private int mvarDropLineWidth;
        public int DropLineWidth
        {
            get
            {
                return mvarDropLineWidth;
            }
            set
            {
                mvarDropLineWidth = value;
            }
        }


        //Цвет линии
        private System.Drawing.Color mvarDropLineColor;
        public System.Drawing.Color DropLineColor
        {
            get
            {
                return mvarDropLineColor;
            }
            set
            {
                mvarDropLineColor = value;
            }
        }
        
        //высота края узла. Если мышка нах-ся над верхним краем узла, то будем бросать перетаскиваемый узел выше текущего, иначе - ниже  
        private int mvarEdgeSensitivity;
        public int EdgeSensitivity
        {
            get
            {
                return mvarEdgeSensitivity;
            }
            set
            {
                mvarEdgeSensitivity = value;
            }
        }

       
        //Когда изменилось место, в которое будем бросать узел, нужно перерисовать контролы(т.е. деревья) 
        private void PositionChanged()
        {
            if (this.Invalidate == null)
            {
                return;
            }

            System.EventArgs e = System.EventArgs.Empty;

            this.Invalidate(this, e);
        }
        



        /// <summary>
        /// Очищаем дерево(убираем линию)
        /// </summary>
        public void ClearDropHighlight()
        {
            SetDropHighlightNode(null, DropLinePositionEnum.None);
        }

        /// <summary>
        /// Вычисляем положение линии в дереве
        /// </summary>
        /// <param name="node">Узел, над кот. нах-ся курсор</param>
        /// <param name="pointInTreeCoords">координаты курсора в дереве</param>
        public void SetDropHighlightNode(UltraTreeNode node, System.Drawing.Point pointInTreeCoords)
        {
            int distanceFromEdge;

            DropLinePositionEnum newDropLinePosition = DropLinePositionEnum.AboveNode;

            distanceFromEdge = mvarEdgeSensitivity;

            if (distanceFromEdge == 0)
            {
                distanceFromEdge = node.Bounds.Height / 2;
            }

            if (pointInTreeCoords.Y < (node.Bounds.Top + distanceFromEdge))
            {
                newDropLinePosition = DropLinePositionEnum.AboveNode;
            }
            else
            {
                if (pointInTreeCoords.Y > ((node.Bounds.Bottom - distanceFromEdge) - 1))
                {
                    newDropLinePosition = DropLinePositionEnum.BelowNode;
                }
            }

            SetDropHighlightNode(node, newDropLinePosition);
        }

        /// <summary>
        /// Рисуем линию в том месте, куда будем перемещать узел
        /// </summary>
        /// <param name="node">Узел, над кот. нах-ся курсор</param>
        /// <param name="dropLinePosition">позиция линии относительно узла</param>
        private void SetDropHighlightNode(UltraTreeNode node, DropLinePositionEnum dropLinePosition)
        {
            bool isPositionChanged = false;

            try
            {
                if (mvarDropHighLightNode != null && mvarDropHighLightNode.Equals(node) && (mvarDropLinePosition == dropLinePosition))
                {
                    isPositionChanged = false;
                }
                else
                {
                    isPositionChanged = true;
                }
            }
            catch
            {
                if (mvarDropHighLightNode == null)
                {
                    isPositionChanged = !(node == null);
                }
            }

            mvarDropHighLightNode = node;
            mvarDropLinePosition = dropLinePosition;

            if (isPositionChanged)
            {
                PositionChanged();
            }
        }

        //Отлавливаем только 1 фазу:
        //AfterDrawElement: для отрисовки линии
        Infragistics.Win.DrawPhase Infragistics.Win.IUIElementDrawFilter.GetPhasesToFilter(ref Infragistics.Win.UIElementDrawParams drawParams)
        {
            return Infragistics.Win.DrawPhase.AfterDrawElement;
            /*|
                   Infragistics.Win.DrawPhase.AfterDrawForeground |
                   Infragistics.Win.DrawPhase.AfterDrawBackColor |
                   Infragistics.Win.DrawPhase.AfterDrawBorders |
                   Infragistics.Win.DrawPhase.AfterDrawChildElements |
                   Infragistics.Win.DrawPhase.AfterDrawForeground |
                   Infragistics.Win.DrawPhase.AfterDrawImage |
                   Infragistics.Win.DrawPhase.AfterDrawImageBackground |
                   Infragistics.Win.DrawPhase.AfterDrawTheme |
                   Infragistics.Win.DrawPhase.BeforeDrawBackColor |
                   Infragistics.Win.DrawPhase.BeforeDrawBorders |
                   Infragistics.Win.DrawPhase.BeforeDrawChildElements |
                   Infragistics.Win.DrawPhase.BeforeDrawElement |
                   Infragistics.Win.DrawPhase.BeforeDrawFocus |
                   Infragistics.Win.DrawPhase.BeforeDrawForeground |
                   Infragistics.Win.DrawPhase.BeforeDrawImage |
                   Infragistics.Win.DrawPhase.BeforeDrawImageBackground |
                   Infragistics.Win.DrawPhase.BeforeDrawTheme;*/
        }
        
        //Собственно рисование линии
        bool Infragistics.Win.IUIElementDrawFilter.DrawElement(Infragistics.Win.DrawPhase drawPhase, ref Infragistics.Win.UIElementDrawParams drawParams)
        {
            Infragistics.Win.UIElement aUIElement;
            System.Drawing.Graphics g;


            aUIElement = drawParams.Element;

            switch (drawPhase)
            {
                    
                case Infragistics.Win.DrawPhase.AfterDrawElement:
                    {
                        g = drawParams.Graphics;
                        UltraTree aTree = (UltraTree)aUIElement.GetContext(typeof(UltraTree));

                        if (aTree == null)
                            return false;

                        if (aTree.Nodes.Count == 0)
                        {
                            using (StringFormat format = new StringFormat())
                            {
                                format.Alignment = StringAlignment.Center;
                                format.LineAlignment = StringAlignment.Center;

                                Font f = new Font(aTree.Font, FontStyle.Bold);

                                SolidBrush br = new SolidBrush(Color.FromArgb(160,160,160));
                                g.DrawString(this.Description, f, br,
                                             new Rectangle(0, 0, aTree.Width, aTree.Height),
                                             format);
                            }
                        }


                        if ((mvarDropHighLightNode == null) || (mvarDropLinePosition == DropLinePositionEnum.None))
                        {
                            return false;
                        }

                        QueryStateAllowedForNodeEventArgs eArgs = new QueryStateAllowedForNodeEventArgs();

                        eArgs.Node = mvarDropHighLightNode;
                        eArgs.DropLinePosition = this.mvarDropLinePosition;

                        eArgs.StatesAllowed = DropLinePositionEnum.All;

                        //вызываем событие
                        this.QueryStateAllowedForNode(this, eArgs);

                        if ((eArgs.StatesAllowed & mvarDropLinePosition) != mvarDropLinePosition)
                        {
                            return false;
                        }



                        if (aUIElement.GetType() == typeof (Infragistics.Win.UltraWinTree.UltraTreeUIElement))
                        {
                            System.Drawing.Pen p = new System.Drawing.Pen(mvarDropLineColor, mvarDropLineWidth);

                            g = drawParams.Graphics;

                            Infragistics.Win.UltraWinTree.NodeSelectableAreaUIElement tElement;
                            tElement =
                                (Infragistics.Win.UltraWinTree.NodeSelectableAreaUIElement)
                                drawParams.Element.GetDescendant(
                                    typeof (Infragistics.Win.UltraWinTree.NodeSelectableAreaUIElement),
                                    mvarDropHighLightNode);

                            if (tElement == null)
                            {
                                return false;
                            }

                            //UltraTree aTree = (UltraTree)tElement.GetContext(typeof(UltraTree));

                            int leftEdge = aTree.DisplayRectangle.Left + 4;
                            int rightEdge = aTree.DisplayRectangle.Right - 4;

                            int lineVPosition;

                            if ((mvarDropLinePosition & DropLinePositionEnum.AboveNode) ==
                                DropLinePositionEnum.AboveNode)
                            {
                                lineVPosition = mvarDropHighLightNode.Bounds.Top;
                                g.DrawLine(p, leftEdge, lineVPosition, rightEdge, lineVPosition);
                                p.Width = 1;
                                g.DrawLine(p, leftEdge, lineVPosition - 3, leftEdge, lineVPosition + 2);
                                g.DrawLine(p, leftEdge + 1, lineVPosition - 2, leftEdge + 1, lineVPosition + 1);
                                g.DrawLine(p, rightEdge, lineVPosition - 3, rightEdge, lineVPosition + 2);
                                g.DrawLine(p, rightEdge - 1, lineVPosition - 2, rightEdge - 1, lineVPosition + 1);
                            }
                            if ((mvarDropLinePosition & DropLinePositionEnum.BelowNode) ==
                                DropLinePositionEnum.BelowNode)
                            {
                                UltraTreeNode tmpNode = null;
                                if ((mvarDropHighLightNode.HasVisibleNodes) &&
                                    (DropLinePosition == DropLinePositionEnum.BelowNode))
                                {
                                    if (mvarDropHighLightNode.Nodes.Count > 1)
                                    {
                                        tmpNode = mvarDropHighLightNode.Nodes[0].GetSibling(NodePosition.Last);
                                    }
                                    else
                                    {
                                        tmpNode = mvarDropHighLightNode.Nodes[0];
                                    }
                                }
                                else
                                {
                                    tmpNode = mvarDropHighLightNode;
                                }


                                lineVPosition = tmpNode.Bounds.Bottom;
                                g.DrawLine(p, leftEdge, lineVPosition, rightEdge, lineVPosition);
                                p.Width = 1;
                                g.DrawLine(p, leftEdge, lineVPosition - 3, leftEdge, lineVPosition + 2);
                                g.DrawLine(p, leftEdge + 1, lineVPosition - 2, leftEdge + 1, lineVPosition + 1);
                                g.DrawLine(p, rightEdge, lineVPosition - 3, rightEdge, lineVPosition + 2);
                                g.DrawLine(p, rightEdge - 1, lineVPosition - 2, rightEdge - 1, lineVPosition + 1);
                            }
                        }
                    }
                    break;
            }

            return false;
        }
    }
}