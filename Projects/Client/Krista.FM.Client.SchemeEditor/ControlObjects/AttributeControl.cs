using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AttributeControl : MinorObjectControl<SmoAttributeDesign>
    {
        public AttributeControl(IDataAttribute controlObject, CustomTreeNodeControl parent)
			: base(controlObject.Class == DataAttributeClassTypes.Typed ? controlObject.ObjectKey : controlObject.Name, 
				String.Format("{0} ({1})", controlObject.Caption, controlObject.Name),
                SmoAttributeDesign.SmoAttributeFactory(controlObject), parent, GetImageIndex(controlObject))
        {
        }

        public AttributeControl(SmoAttributeDesign smoObject, CustomTreeNodeControl parent)
			: base(smoObject.Class == DataAttributeClassTypes.Typed ? smoObject.ObjectKey : smoObject.Name, 
				String.Format("{0} ({1})", smoObject.Caption, smoObject.Name), 
				smoObject, parent, GetImageIndex(smoObject))
        {
        }

        private static int GetImageIndex(IDataAttribute controlObject)
        {
            int imageIndex = (int)Images.Attribute;
            switch (controlObject.Kind)
            {
                case DataAttributeKindTypes.Regular: imageIndex = (int)Images.Attribute; break;
                case DataAttributeKindTypes.Sys: imageIndex = (int)Images.AttributeLock; break;
                case DataAttributeKindTypes.Serviced: imageIndex = (int)Images.AttributeServ; break;
            }
            if (controlObject.Name == "ID")
                return (int)Images.AttributeKey;
            else if (controlObject.Class == DataAttributeClassTypes.Reference)
                return (int)Images.AttributeLink;
            else 
                return imageIndex;
        }

        /// <summary>
        /// Хранимое наименование узла задаваемое при создании
        /// </summary>
        public override string Caption
        {
            get { return String.Format("{0} ({1})", TypedControlObject.Caption, TypedControlObject.Name); }
        }

        public bool CanDelete()
        {
            return ((IDataAttribute)ControlObject).Class == DataAttributeClassTypes.Typed && IsEditable();
        }

        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "CanDelete")]
        public void Delete()
        {
            AttributeListControl attributeList = GetAttributeListControl(Parent);

            SmoDictionaryBaseDesign<string, IDataAttribute> smoCollection = (SmoDictionaryBaseDesign<string, IDataAttribute>)attributeList.ControlObject;
            IModifiableCollection<string, IDataAttribute> collection = (IModifiableCollection<string, IDataAttribute>)smoCollection.ServerControl;

            if (MessageBox.Show(String.Format("Вы действительно хотите удалить атрибут \r {0} ?", Caption), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                collection.Remove(((IDataAttribute)ControlObject).ObjectKey);
                OnChange(this, new EventArgs());
                Remove();
            }
        }

        /// <summary>
        /// Рекурсивный метод получает у атрибута родительскую коллекцию
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private AttributeListControl GetAttributeListControl(Infragistics.Win.UltraWinTree.UltraTreeNode parent)
        {
            if (parent is AttributeListControl)
                return (AttributeListControl)parent;

            return GetAttributeListControl(parent.Parent);
        }

        public virtual bool RepositionAttribute(int targetNode, Infragistics.Win.UltraWinTree.NodePosition pos)
        {
            if (!IsEditable())
                throw new InvalidOperationException(
                    String.Format(
                        "Для внесения изменений сначала необходимо заблокировать пакет.\nВстаньте на пакет и в контекстном меню выберите пункт \"Редактировать\""
                        ));

            try
            {
                switch (pos)
                {
                    case NodePosition.Previous:
                        if (((SmoAttributeDesign)this.ControlObject).Position < targetNode)
                        {
                            ((SmoAttributeDesign)this.ControlObject).Position =
                                targetNode - 1;
                        }
                        else
                        {
                            ((SmoAttributeDesign)this.ControlObject).Position =
                                targetNode;
                        }
                        break;
                    case NodePosition.Next:
                        if (((SmoAttributeDesign)this.ControlObject).Position < targetNode)
                        {
                            ((SmoAttributeDesign)this.ControlObject).Position =
                                targetNode;
                        }
                        else
                        {
                            ((SmoAttributeDesign)this.ControlObject).Position =
                                targetNode + 1;
                        }
                        break;
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public override bool Draggable
        {
            get
            {
                if (((SmoAttributeDesign)this.Tag).Class == Krista.FM.ServerLibrary.DataAttributeClassTypes.Typed
                    || ((SmoAttributeDesign)this.Tag).Class == Krista.FM.ServerLibrary.DataAttributeClassTypes.Reference
                    
                    //Нужны для UniqueKey, в остальных случаях должен быть запрет в onDragOver и onDragDrop
                    || ((SmoAttributeDesign)this.Tag).Class == Krista.FM.ServerLibrary.DataAttributeClassTypes.System
                    || ((SmoAttributeDesign)this.Tag).Class == Krista.FM.ServerLibrary.DataAttributeClassTypes.Fixed
                    )
                {
                    return true;
                }
                return false;
            }
        }


        public override void OnDragOver(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            //Get the node the mouse is over.
            Point point = Control.PointToClient(new Point(e.X, e.Y));
            CustomTreeNodeControl targetNode = Control.GetNodeFromPoint(point) as CustomTreeNodeControl;

            if ( targetNode != null
                 && ((IDataAttribute)targetNode.ControlObject).Class != DataAttributeClassTypes.System
                 && !AttributeListControl.SelectedNodesContainsUnavailableItems((SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection)))
                )
            {
                e.Effect = DragDropEffects.Move;
                dropHightLightDrawFilter.SetDropHighlightNode(targetNode, point );
                return;
            }

            //Запрет
            base.OnDragOver(e, dropHightLightDrawFilter);
            
        }


        public override void OnDragDrop(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {

            SelectedNodesCollection selectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));
            UltraTreeNode targetNode = dropHightLightDrawFilter.DropHightLightNode;
            UltraTreeNode sourceNode;
            int i;

            try
            {
                switch (dropHightLightDrawFilter.DropLinePosition)
                {
                    case DropLinePositionEnum.OnNode: //Drop ON the node
                        {
                            for (i = 0; i <= (selectedNodes.Count - 1); i++)
                            {
                                sourceNode = selectedNodes[i];

                                if (sourceNode.Key != targetNode.Key)
                                {
                                    if (targetNode.Nodes.Count == 0)
                                    {
                                        ((AttributeControl) sourceNode).RepositionAttribute(
                                            ((SmoAttributeDesign) ((AttributeControl) targetNode).ControlObject).
                                                Position,
                                            NodePosition.Next);
                                    }
                                    else
                                    {
                                        ((AttributeControl) sourceNode).RepositionAttribute(
                                            ((SmoAttributeDesign)
                                             ((AttributeControl) targetNode.Nodes[targetNode.Nodes.Count - 1]).
                                                 ControlObject).
                                                Position,
                                            NodePosition.Next);
                                    }

                                    ((SmoAttributeDesign) ((AttributeControl) sourceNode).ControlObject).
                                        GroupParentAttribute =
                                        ((SmoAttributeDesign) ((AttributeControl) targetNode).ControlObject).ObjectKey.
                                            ToString();

                                    sourceNode.Reposition(targetNode.Nodes);

                                    sourceNode.Selected = true;
                                }
                            }
                            break;
                        }
                    case DropLinePositionEnum.BelowNode: //Drop Below the node
                        {
                            for (i = 0; i <= (selectedNodes.Count - 1); i++)
                            {
                                sourceNode = selectedNodes[i];

                                ((AttributeControl) sourceNode).RepositionAttribute(
                                    ((SmoAttributeDesign) ((AttributeControl) targetNode).ControlObject).Position,
                                    NodePosition.Next);

                                ((SmoAttributeDesign) ((AttributeControl) sourceNode).ControlObject).
                                    GroupParentAttribute =
                                    ((SmoAttributeDesign) ((AttributeControl) targetNode).ControlObject).
                                        GroupParentAttribute;

                                sourceNode.Reposition(targetNode, NodePosition.Next);
                                targetNode = sourceNode;
                            }
                            break;
                        }

                    case DropLinePositionEnum.AboveNode: //New Index should be the same as the Drop
                        {
                            for (i = 0; i <= (selectedNodes.Count - 1); i++)
                            {
                                sourceNode = selectedNodes[i];

                                ((AttributeControl) sourceNode).RepositionAttribute(
                                    ((SmoAttributeDesign) ((AttributeControl) targetNode).ControlObject).Position,
                                    NodePosition.Previous);

                                ((SmoAttributeDesign) ((AttributeControl) sourceNode).ControlObject).
                                    GroupParentAttribute =
                                    ((SmoAttributeDesign) ((AttributeControl) targetNode).ControlObject).
                                        GroupParentAttribute;
                                sourceNode.Reposition(targetNode, NodePosition.Previous);
                            }
                            break;
                        }
                }
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message, "Пакет не взят на редактирование", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }

            finally
            {
                dropHightLightDrawFilter.ClearDropHighlight();
            }
        }

        public override void QueryStateAllowedForNode(ObjectsTreeViewDropHightLightDrawFilter.QueryStateAllowedForNodeEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            //This is not a continent
            //Allow users to drop above or below this node - but not on
            //it, because countries don//t have child countries
            e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode |
                /* OnNode сделать только для Табличных документов */ DropLinePositionEnum.OnNode;

            //Since we can only drop above or below this node, 
            //we don//t want a middle section. So we set the 
            //sensitivity to half the height of the node
            //This means the DrawFilter will respond to the top half
            //bottom half of the node, but not the middle. 
            dropHightLightDrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 3;
        }

    }
}
