using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;
using Infragistics.Win.UltraWinTree;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AttributeListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IDataAttribute>, IDataAttribute>
    {
        public AttributeListControl(IDataAttributeCollection controlObject, CustomTreeNodeControl parent)
            : base("IDataAttributeCollection", "Атрибуты",
            new SMODataAttributeCollectionDesign(controlObject), parent, (int)Images.Attributes)
        {
        }


        public override CustomTreeNodeControl Create(IDataAttribute item)
        {
            return new AttributeControl(item, this);
        }

        [MenuAction("Создать атрибут", Images.CreateAttribute, CheckMenuItemEnabling = "IsEditable")]
        public override void AddNew()
        {
            base.AddNew();
        }

        [MenuAction("Создать атрибут-документ", Images.CreateAttribute, CheckMenuItemEnabling = "IsEditable")]
        public void AddNewDocumentAttribute()
        {
            SmoDictionaryBaseDesign<string, IDataAttribute> smoCollection = (SmoDictionaryBaseDesign<string, IDataAttribute>)ControlObject;
            IModifiableCollection<string, IDataAttribute> collection = (IModifiableCollection<string, IDataAttribute>)smoCollection.ServerControl;
            IDataAttribute newObj = ((IDataAttributeCollection)collection).CreateItem(AttributeClass.Document);
            CustomTreeNodeControl node = Create(newObj);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            SelectNewObject(node);
        }

		public void AddRefAttribute(AttributeControl refNode)
		{
			SmoDictionaryBaseDesign<string, IDataAttribute> smoCollection = (SmoDictionaryBaseDesign<string, IDataAttribute>)ControlObject;
            IModifiableCollection<string, IDataAttribute> collection = (IModifiableCollection<string, IDataAttribute>)smoCollection.ServerControl;
			IDataAttribute newObj = ((IDataAttributeCollection)collection).CreateItem(AttributeClass.RefAttribute);
			IDocumentEntityAttribute newObjD = newObj as IDocumentEntityAttribute;
			if (newObjD != null)
			{
				IDataAttribute a = refNode.ControlObject as IDataAttribute;
				SmoAttributeDesign smoAttribute = (SmoAttributeDesign)refNode.ControlObject;
                newObjD.SetSourceAttribute(smoAttribute.ServerControl);
			}
			CustomTreeNodeControl node = Create(newObj);
			Nodes.Add(node);
			OnChange(this, new EventArgs());

			SelectNewObject(node);
		}

        /// <summary>
        /// Добавляет дочерние узлы для коллекции атрибутов
        /// </summary>
        protected override void ExpandNode()
        {
            try
            {
                StartExpand();

                foreach (KeyValuePair<string, IDataAttribute> kvp in (SmoDictionaryBaseDesign<string, IDataAttribute>)ControlObject)
                {
                    AttributeControl control = (AttributeControl)Create(kvp.Value);

                    if (String.IsNullOrEmpty(kvp.Value.GroupParentAttribute))
                        Nodes.Add(control);
                    else
                    {
                        UltraTreeNode findNode = FindParentNode(Nodes, kvp.Value.GroupParentAttribute);

                        if (findNode == null)
                            Nodes.Add(Create(kvp.Value));
                        else
                            findNode.Nodes.Add(Create(kvp.Value));
                    }
                }
            }
            finally
            {
                EndExpand();
            }
        }

        /// <summary>
        /// Поиск родительского атрибута
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private UltraTreeNode FindParentNode(TreeNodesCollection nodes, string key)
        {
            UltraTreeNode node = null;

            foreach (UltraTreeNode ultraTreeNode in nodes)
            {
                if (((SmoAttributeDesign)((AttributeControl)ultraTreeNode).ControlObject).ObjectKey == key)
                    return ultraTreeNode;

                if (ultraTreeNode.Nodes.Count != 0)
                {
                    node = FindParentNode(ultraTreeNode.Nodes, key);

                    if (node != null)
                        return node;
                }
            }

            return node;
        }


        public override void OnDragOver(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            //Get the node the mouse is over.
            Point point = Control.PointToClient(new Point(e.X, e.Y));
            CustomTreeNodeControl targetNode = Control.GetNodeFromPoint(point) as CustomTreeNodeControl;

            if (targetNode !=null
                && targetNode.Parent is ClassControl
                && !SelectedNodesContainsUnavailableItems((SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection)))
                )
            {
                if (((ClassControl)targetNode.Parent).ControlObject is IEntity)
                {
                    IEntity ent = (IEntity)((ClassControl)targetNode.Parent).ControlObject;
                    if (ent.ClassType == ClassTypes.DocumentEntity)
                    {
                        dropHightLightDrawFilter.SetDropHighlightNode(targetNode, point);
                        e.Effect = DragDropEffects.Move;
                        return;
                    }
                }
            }

            //Запрет
            base.OnDragOver(e, dropHightLightDrawFilter);

        }

        internal static bool SelectedNodesContainsUnavailableItems(SelectedNodesCollection selectedNodes)
        {
            //SelectedNodesCollection selectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));
            UltraTreeNode sourceNode;
            int i;
            for (i = 0; i <= (selectedNodes.Count - 1); i++)
            {
                sourceNode = selectedNodes[i];
                if (sourceNode is AttributeControl)
                {
                    if ( ((SmoAttributeDesign)((AttributeControl)sourceNode).Tag).Class == Krista.FM.ServerLibrary.DataAttributeClassTypes.System
                        || ((SmoAttributeDesign)((AttributeControl)sourceNode).Tag).Class == Krista.FM.ServerLibrary.DataAttributeClassTypes.Fixed
                        )
                    {
                        return true;
                    }
                }
            }

            return false;
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
                            foreach (UltraTreeNode item in selectedNodes)
                            {
                                this.AddRefAttribute((AttributeControl) item);
                            }
                            break;
                        }

                    case DropLinePositionEnum.BelowNode: //Drop Below the node
                        {
                            for (i = 0; i <= (selectedNodes.Count - 1); i++)
                            {
                                sourceNode = selectedNodes[i];

                                ((AttributeControl)sourceNode).RepositionAttribute(
                                    ((SmoAttributeDesign)((AttributeControl)targetNode).ControlObject).Position, NodePosition.Next);

                                ((SmoAttributeDesign)((AttributeControl)sourceNode).ControlObject).GroupParentAttribute =
                                    ((SmoAttributeDesign)((AttributeControl)targetNode).ControlObject).GroupParentAttribute;

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

                                ((AttributeControl)sourceNode).RepositionAttribute(
                                    ((SmoAttributeDesign)((AttributeControl)targetNode).ControlObject).Position, NodePosition.Previous);

                                ((SmoAttributeDesign)((AttributeControl)sourceNode).ControlObject).GroupParentAttribute =
                                    ((SmoAttributeDesign)((AttributeControl)targetNode).ControlObject).GroupParentAttribute;
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
            e.StatesAllowed = DropLinePositionEnum.OnNode;
        }


    }
}
