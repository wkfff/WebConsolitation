using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.SchemeEditor.ControlObjects;
using Krista.FM.Client.SMO;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor
{
    /// <summary>
    /// Дерево пакетов и объектов.
    /// </summary>
    public class ObjectsTreeView : CustomTreeView
    {
        /// <summary>
        /// Create a new instance of the DrawFilter class to 
        /// Handle drawing the DropHighlight/DropLines
        /// </summary>
        private ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter;

        public ObjectsTreeView()
        {
            dropHightLightDrawFilter = new ObjectsTreeViewDropHightLightDrawFilter();
            dropHightLightDrawFilter.Invalidate += new EventHandler(dropHightLightDrawFilter_Invalidate);
            dropHightLightDrawFilter.QueryStateAllowedForNode += new ObjectsTreeViewDropHightLightDrawFilter.QueryStateAllowedForNodeEventHandler(dropHightLightDrawFilter_QueryStateAllowedForNode);
            Override.SelectionType = Infragistics.Win.UltraWinTree.SelectType.ExtendedAutoDrag;
            DrawFilter = dropHightLightDrawFilter;
            DragOver += new DragEventHandler(CustomTreeView_DragOver);
            DragDrop += new DragEventHandler(CustomTreeView_DragDrop);
            QueryContinueDrag += new QueryContinueDragEventHandler(CustomTreeView_QueryContinueDrag);
            DragLeave += new EventHandler(CustomTreeView_DragLeave);
        }

        /// <summary>
        /// Fires when the user drags outside the control. 
        /// </summary>
        private void CustomTreeView_DragLeave(object sender, EventArgs e)
        {
            //When the mouse goes outside the control, clear the 
            //drophighlight. 
            //Since the DropHighlight is cleared when the 
            //mouse is not over a node, anyway, 
            //this is probably not needed
            //But, just in case the user goes from a node directly
            //off the control...
            dropHightLightDrawFilter.ClearDropHighlight();
        }

        /// <summary>
        /// Test to see if we want to continue dragging
        /// </summary>
        private void CustomTreeView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //Did the user press escape? 
            if (e.EscapePressed)
            {
                //User pressed escape
                //Cancel the Drag
                e.Action = DragAction.Cancel;
                //Clear the Drop highlight, since we are no longer
                //dragging
                dropHightLightDrawFilter.ClearDropHighlight();
            }
        }

        /// <summary>
        /// The DragDrop event. Here we respond to a Drop on the tree
        /// </summary>
        private void CustomTreeView_DragDrop(object sender, DragEventArgs e)
        {
            CustomTreeNodeControl targetNodeControl = dropHightLightDrawFilter.DropHightLightNode as CustomTreeNodeControl;
            if (targetNodeControl != null)
            {
                targetNodeControl.OnDragDrop(e, dropHightLightDrawFilter);
            }
            else
            {
                e.Effect = DragDropEffects.None;
                dropHightLightDrawFilter.ClearDropHighlight();
            }

            return;

            ////refactored
            //try
            //{
            //    //A dummy node variable used for various things
            //    UltraTreeNode aNode;
            //    //The SelectedNodes which will be dropped
            //    SelectedNodesCollection SelectedNodes;
            //    //The Node to Drop On
            //    UltraTreeNode DropNode;
            //    //An integer used for loops
            //    int i;

            //    //Set the DropNode
            //    DropNode = dropHightLightDrawFilter.DropHightLightNode;

            //    //Get the Data and put it into a SelectedNodes collection,
            //    //then clone it and work with the clone
            //    //These are the nodes that are being dragged and dropped
            //    SelectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));

            //    switch (dropHightLightDrawFilter.DropLinePosition)
            //    {
            //        case DropLinePositionEnum.OnNode: //Drop ON the node
            //            {
            //                if (DropNode is AttributeListControl)
            //                {
            //                    foreach (UltraTreeNode item in SelectedNodes)
            //                    {
            //                        ((AttributeListControl)DropNode).AddRefAttribute((AttributeControl)item);
            //                    }
            //                    break;
            //                }
            //                if (DropNode is AttributeControl)
            //                {
            //                    for (i = 0; i <= (SelectedNodes.Count - 1); i++)
            //                    {
            //                        aNode = SelectedNodes[i];

            //                        if (aNode.Key == DropNode.Key)
            //                            break;

            //                        if (DropNode.Nodes.Count == 0)
            //                            ((AttributeControl)aNode).RepositionAttribute(((SmoAttributeDesign)((AttributeControl)DropNode).ControlObject).Position, NodePosition.Next);
            //                        else
            //                            ((AttributeControl)aNode).RepositionAttribute(
            //                                ((SmoAttributeDesign)((AttributeControl)DropNode.Nodes[DropNode.Nodes.Count - 1]).ControlObject).Position, NodePosition.Next);

            //                        ((SmoAttributeDesign)((AttributeControl)aNode).ControlObject).GroupParentAttribute =
            //                            ((SmoAttributeDesign)((AttributeControl)DropNode).ControlObject).ObjectKey.ToString();

            //                        aNode.Reposition(DropNode.Nodes);

            //                        aNode.Selected = true;
            //                    }
            //                    break;
            //                }
            //                break;
            //            }
            //        case DropLinePositionEnum.BelowNode: //Drop Below the node
            //            {
            //                for (i = 0; i <= (SelectedNodes.Count - 1); i++)
            //                {
            //                    aNode = SelectedNodes[i];

            //                    ((AttributeControl)aNode).RepositionAttribute(
            //                        ((SmoAttributeDesign)((AttributeControl)DropNode).ControlObject).Position, NodePosition.Next);

            //                    ((SmoAttributeDesign)((AttributeControl)aNode).ControlObject).GroupParentAttribute =
            //                        ((SmoAttributeDesign)((AttributeControl)DropNode).ControlObject).GroupParentAttribute;

            //                    aNode.Reposition(DropNode, NodePosition.Next);

            //                    //Change the DropNode to the node that was
            //                    //just repositioned so that the next 
            //                    //added node goes below it. 
            //                    DropNode = aNode;
            //                }
            //                break;
            //            }
            //        case DropLinePositionEnum.AboveNode: //New Index should be the same as the Drop
            //            {
            //                for (i = 0; i <= (SelectedNodes.Count - 1); i++)
            //                {
            //                    aNode = SelectedNodes[i];

            //                    ((AttributeControl)aNode).RepositionAttribute(
            //                        ((SmoAttributeDesign)((AttributeControl)DropNode).ControlObject).Position, NodePosition.Previous);

            //                    ((SmoAttributeDesign)((AttributeControl)aNode).ControlObject).GroupParentAttribute =
            //                        ((SmoAttributeDesign)((AttributeControl)DropNode).ControlObject).GroupParentAttribute;

            //                    aNode.Reposition(DropNode, NodePosition.Previous);
            //                }
            //                break;
            //            }
            //    }
            //}
            //catch (InvalidOperationException exception)
            //{
            //    MessageBox.Show(exception.Message, "Пакет не взят на редактирование", MessageBoxButtons.OK,
            //                    MessageBoxIcon.Information);
            //}
            //finally
            //{
            //    dropHightLightDrawFilter.ClearDropHighlight();
            //}
        }



        private void CustomTreeView_DragOver(object sender, DragEventArgs e)
        {
            CustomTreeNodeControl targetNodeControl = GetNodeFromPoint(PointToClient(new Point(e.X, e.Y))) as CustomTreeNodeControl;
            if (targetNodeControl != null)
            {
                targetNodeControl.OnDragOver(e, dropHightLightDrawFilter);
            }
            else
            {
                e.Effect = DragDropEffects.None;
                dropHightLightDrawFilter.ClearDropHighlight();
            }

            return;

            ////refactored
            //string[] formats = e.Data.GetFormats();
            //if (formats.GetLength(0) != 0)
            //{
            //    SelectedNodesCollection selectedNodes = (SelectedNodesCollection)e.Data.GetData(formats[0]);
            //    if (selectedNodes[0].GetType().Name != typeof(AttributeControl).Name
            //        && selectedNodes[0].GetType().Name != typeof(AttributePresentationControl).Name
            //        && selectedNodes[0].GetType().Name != typeof(AttributeReferencePresentationControl).Name)
            //    {
            //        //The Mouse is not over a node
            //        //Do not allow dropping here
            //        e.Effect = DragDropEffects.None;
            //        //Erase any DropHighlight
            //        dropHightLightDrawFilter.ClearDropHighlight();
            //        //Exit stage left
            //        return;
            //    }
            //}
            

            ////A dummy node variable used to hold nodes for various 
            ////things
            //CustomTreeNodeControl aNode;
            ////The Point that the mouse cursor is on, in Tree coords. 
            ////This event passes X and Y in form coords. 
            //Point PointInTree;

            ////Get the position of the mouse in the tree, as opposed
            ////to form coords
            //PointInTree = PointToClient(new Point(e.X, e.Y));

            ////Get the node the mouse is over.
            //aNode = GetNodeFromPoint(PointInTree) as CustomTreeNodeControl;

            ////Make sure the mouse is over a node
            //if (aNode == null)
            //{
            //    //The Mouse is not over a node
            //    //Do not allow dropping here
            //    e.Effect = DragDropEffects.None;
            //    //Erase any DropHighlight
            //    dropHightLightDrawFilter.ClearDropHighlight();
            //    //Exit stage left
            //    return;
            //}
           

            ////	Don't let continent nodes be dropped onto other continent nodes
            //if (aNode is PackageListControl)
            //{
            //    if (PointInTree.Y > (aNode.Bounds.Top + 2) &&
            //         PointInTree.Y < (aNode.Bounds.Bottom - 2))
            //    {
            //        e.Effect = DragDropEffects.None;
            //        dropHightLightDrawFilter.ClearDropHighlight();
            //        return;
            //    }
            //}

            //if (aNode is AttributeListControl && aNode.Parent is ClassControl)
            //{
            //    if (((ClassControl)aNode.Parent).ControlObject is IEntity)
            //    {
            //        IEntity ent = (IEntity)((ClassControl)aNode.Parent).ControlObject;
            //        if (ent.ClassType == ClassTypes.DocumentEntity)
            //        {
            //            //If we//ve reached this point, it//s okay to drop on this node
            //            //Tell the DrawFilter where we are by calling SetDropHighlightNode
            //            dropHightLightDrawFilter.SetDropHighlightNode(aNode, PointInTree);
            //            //Allow Dropping here. 
            //            e.Effect = DragDropEffects.Move;
            //            //Exit stage left
            //            return;
            //        }
            //    }
            //}


            //if (aNode is AttributeControl && ((IDataAttribute)aNode.ControlObject).Class != DataAttributeClassTypes.System)
            //{
            //    e.Effect = DragDropEffects.Move;
            //    dropHightLightDrawFilter.SetDropHighlightNode(aNode, PointInTree);
            //    return;
            //}


            ////Mouse is over a node whose parent is selected
            ////Do not allow the drop
            //e.Effect = DragDropEffects.None;
            ////Clear the DropHighlight
            //dropHightLightDrawFilter.ClearDropHighlight();
        }

        private void dropHightLightDrawFilter_Invalidate(object sender, EventArgs e)
        {
            Invalidate();
        }

        //This event is fired by the DrawFilter to let us determine
        //what kinds of drops we want to allow on any particular node
        private void dropHightLightDrawFilter_QueryStateAllowedForNode(object sender, ObjectsTreeViewDropHightLightDrawFilter.QueryStateAllowedForNodeEventArgs e)
        {
            CustomTreeNodeControl targetNodeControl = e.Node as CustomTreeNodeControl;
            if (targetNodeControl != null)
            {
                targetNodeControl.QueryStateAllowedForNode(e, dropHightLightDrawFilter);
            }
            else
            {
                e.StatesAllowed = DropLinePositionEnum.None;
            }

            return;

            ////refactored
            //if (e.Node is AttributeListControl)
            //{
            //    e.StatesAllowed = DropLinePositionEnum.OnNode;
            //    return;
            //}
            //else if (e.Node is AttributeControl)
            //{
            //    //This is not a continent
            //    //Allow users to drop above or below this node - but not on
            //    //it, because countries don//t have child countries
            //    e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode |
            //        /* OnNode сделать только для Табличных документов */ DropLinePositionEnum.OnNode;

            //    //Since we can only drop above or below this node, 
            //    //we don//t want a middle section. So we set the 
            //    //sensitivity to half the height of the node
            //    //This means the DrawFilter will respond to the top half
            //    //bottom half of the node, but not the middle. 
            //    dropHightLightDrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 3;
            //}
        }
    }
}