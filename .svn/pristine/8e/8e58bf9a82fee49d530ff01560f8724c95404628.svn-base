using System.Drawing;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.MDXExpert.FieldList;

namespace Krista.FM.Client.MDXExpert
{
    public class MapSeriesDrawFilterClass : Infragistics.Win.IUIElementDrawFilter
    {

        public event System.EventHandler Invalidate;


        public event QueryStateAllowedForNodeEventHandler QueryStateAllowedForNode;
        public delegate void QueryStateAllowedForNodeEventHandler(object sender, QueryStateAllowedForNodeEventArgs e);

        public class QueryStateAllowedForNodeEventArgs : System.EventArgs
        {
            private UltraTreeNode node;

            public UltraTreeNode Node
            {
                get { return node;}
                set { node = value; }
            }
        }

        public MapSeriesDrawFilterClass()
        {
            InitProperties();
        }

        //Инициализация свойств по умолчанию
        private void InitProperties()
        {
            mvarSelectHighLightNode = null;
            mvarSelectNodeColor = System.Drawing.SystemColors.ControlText;
        }


        //Очистка
        public void Dispose()
        {
            mvarSelectHighLightNode = null;
        }

        //Ссылка на выделенный узел
        private UltraTreeNode mvarSelectHighLightNode;
        public UltraTreeNode SelectHightLightNode
        {
            get
            {
                return mvarSelectHighLightNode;
            }
            set
            {
                //Если узел установлен в то же самое значение, то просто выходим
                if ((mvarSelectHighLightNode != null) && (mvarSelectHighLightNode.Equals(value)))
                {
                    return;
                }
                mvarSelectHighLightNode = value;

                PositionChanged();
            }
        }


        //Цвет линии
        private System.Drawing.Color mvarSelectNodeColor;
        public System.Drawing.Color SelectNodeColor
        {
            get
            {
                return mvarSelectNodeColor;
            }
            set
            {
                mvarSelectNodeColor = value;
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
        public void ClearSelectHighlight()
        {
            //SetDropHighlightNode(null, DropLinePositionEnum.None);
        }

        /// <summary>
        /// Вычисляем положение линии в дереве
        /// </summary>
        /// <param name="node">Узел, над кот. нах-ся курсор</param>
        public void SetSelectHighlightNode(UltraTreeNode node)
        {

           // SetSelectHighlightNode(node);
        }

        /// <summary>
        /// Рисуем линию в том месте, куда будем перемещать узел
        /// </summary>
        /// <param name="node">Узел, над кот. нах-ся курсор</param>
        /// <param name="dropLinePosition">позиция линии относительно узла</param>
        private void SetSelectHighlightNode(UltraTreeNode node, DropLinePositionEnum dropLinePosition)
        {
            /*
            bool isPositionChanged = false;

            try
            {
                if (mvarSelectHighLightNode != null && mvarSelectHighLightNode.Equals(node) && (mvarDropLinePosition == dropLinePosition))
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
                if (mvarSelectHighLightNode == null)
                {
                    isPositionChanged = !(node == null);
                }
            }

            mvarSelectHighLightNode = node;
            mvarDropLinePosition = dropLinePosition;

            if (isPositionChanged)
            {
                PositionChanged();
            }
             */
        }

        //Отлавливаем только 1 фазу:
        //AfterDrawElement: для отрисовки линии
        Infragistics.Win.DrawPhase Infragistics.Win.IUIElementDrawFilter.GetPhasesToFilter(ref Infragistics.Win.UIElementDrawParams drawParams)
        {
            return Infragistics.Win.DrawPhase. AfterDrawElement;
        }
        
        //Собственно рисование линии
        bool Infragistics.Win.IUIElementDrawFilter.DrawElement(Infragistics.Win.DrawPhase drawPhase, ref Infragistics.Win.UIElementDrawParams drawParams)
        {
            Infragistics.Win.UIElement aUIElement;
            System.Drawing.Graphics g;

            if (mvarSelectHighLightNode == null)
            {
                return false;
            }

            QueryStateAllowedForNodeEventArgs eArgs = new QueryStateAllowedForNodeEventArgs();

            eArgs.Node = mvarSelectHighLightNode;


            //вызываем событие
          //  this.QueryStateAllowedForNode(this, eArgs);


            aUIElement = drawParams.Element;


            if (drawPhase == Infragistics.Win.DrawPhase.AfterDrawElement)
            {
                if (aUIElement.GetType() == typeof(UltraTreeUIElement))
                {
                    Pen p = new System.Drawing.Pen(mvarSelectNodeColor, 1);

                    g = drawParams.Graphics;

                    NodeSelectableAreaUIElement tElement = (NodeSelectableAreaUIElement)drawParams.Element.GetDescendant(typeof(NodeSelectableAreaUIElement), mvarSelectHighLightNode);

                    if (tElement == null)
                    {
                        return false;
                    }
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle rect = mvarSelectHighLightNode.UIElement.RectInsideBorders;

                    g.DrawRectangle(p, rect.Left + 38, rect.Top, rect.Width - 40, rect.Height - 1);
                    /*
                    g.DrawRectangle(p, 
                                    mvarSelectHighLightNode. Bounds.Left + 39, 
                                    mvarSelectHighLightNode.Bounds.Top, 
                                    mvarSelectHighLightNode.Bounds.Width - 41, 
                                    mvarSelectHighLightNode.Bounds.Height);
                    */
                    /*
                    UltraTree aTree = (UltraTree)tElement.GetContext(typeof(UltraTree));
                    int leftEdge = aTree.DisplayRectangle.Left + 4;
                    int rightEdge = aTree.DisplayRectangle.Right - 4;
                    g.DrawRectangle(p, leftEdge, mvarSelectHighLightNode.Bounds.Top - 1, rightEdge, mvarSelectHighLightNode.Bounds.Height - 2);
                    */
                }
            }
            
            
            return false;
        }
    }
}