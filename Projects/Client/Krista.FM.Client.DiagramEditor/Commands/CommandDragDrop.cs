using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.DiagramEditor.Shapes.Factory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandDragDrop : DiagramEditorCommand
    {
        private CommandNewDiagramEntity cmdNew;

        public CommandDragDrop(AbstractDiagram diagram)
            : base(diagram)
        {
            cmdNew = new CommandNewDiagramEntity(diagram);
        }

        public void Execute(DragEventArgs e)
        {
            Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

            Point clientPoint = Diagram.Site.PointToClient(new Point(e.X, e.Y));
            clientPoint.Offset(-scrollOffset.Width, -scrollOffset.Height);
            clientPoint = ScaleTransform.TransformPoint(clientPoint, Diagram.Site.ZoomFactor);

            // Get the Data and put it into a SelectedNodes collection.
            // These are the nodes that are being dragged and dropped
            SelectedNodesCollection selectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));
            
            foreach (UltraTreeNode node in selectedNodes)
            {
                try
                {
                    if (node.Tag is IEntity)
                    {
                        UMLEntityBase umlEntity = cmdNew.AddEntity((IEntity)node.Tag, clientPoint);

                        AssociationRestore((ICommonDBObject)node.Tag, umlEntity);
                        continue;
                    }

                    if (node.Tag is IAssociation)
                    {
                        DragDropAssociation((ICommonDBObject)node.Tag, clientPoint);
                        continue;
                    }

                    if (node.Tag is IDataAttribute)
                    {
                        IDataAttribute attr = ((IEntity)((IDataAttribute)node.Tag).OwnerObject).Attributes[((IDataAttribute)node.Tag).ObjectKey];
                        DragDropDataAttribute(attr, clientPoint);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception(ex.Message, ex);
                }

                clientPoint.X += 50;
                clientPoint.Y += 50;
            }

            Diagram.IsChanged = true;
            Diagram.Site.Invalidate();
        }

        public override void Execute()
        {
        }

        /// <summary>
        /// Восстановление ассоциаций у выносимого класса
        /// </summary>
        /// <param name="commonDBObject"> Серверный объект</param>
        public void AssociationRestore(ICommonDBObject commonDBObject, UMLEntityBase umlEntity)
        {
            foreach (IEntityAssociation association in ((IEntity)commonDBObject).Associated.Values)
            {
                string key = association.Key;
                DiagramEntity roleAEntity = Diagram.FindDiagramEntityByFullName(association.RoleData.Key);
                if (roleAEntity != null)
                {
                    List<Point> points = new List<Point>();
                    points.Add(umlEntity.Coordinate());
                    points.Add(roleAEntity.Coordinate());
                    UMLAssociation umlAssociation = UMLAssociationFactory.Create(
                        association.AssociationClassType,
                        key,
                        Guid.NewGuid(),
                        Diagram,
                        roleAEntity,
                        umlEntity,
                        points);
                    if (umlAssociation.StereotypeEntity == null)
                    {
                        umlAssociation.StereotypeEntity = umlAssociation.GetUMLAssociationStereotype();
                        Diagram.Entities.Add(umlAssociation);

                        if (umlAssociation.StereotypeEntity != null)
                        {
                            Diagram.Entities.Add(umlAssociation.StereotypeEntity);
                        }
                    }
                }
            }

            foreach (IEntityAssociation association in ((IEntity)commonDBObject).Associations.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.BridgeBridge)
                {
                    continue;
                }

                string key = association.Key;
                DiagramEntity roleBEntity = Diagram.FindDiagramEntityByFullName(association.RoleBridge.Key);
                if (roleBEntity != null)
                {
                    List<Point> points = new List<Point>();
                    points.Add(umlEntity.Coordinate());
                    points.Add(roleBEntity.Coordinate());

                    UMLAssociation umlAssociation = UMLAssociationFactory.Create(
                        association.AssociationClassType,
                        key,
                        Guid.NewGuid(),
                        Diagram,
                        umlEntity,
                        roleBEntity,
                        points);
                    if (umlAssociation.StereotypeEntity == null)
                    {
                        umlAssociation.StereotypeEntity = umlAssociation.GetUMLAssociationStereotype();
                        Diagram.Entities.Add(umlAssociation);

                        if (umlAssociation.StereotypeEntity != null)
                        {
                            Diagram.Entities.Add(umlAssociation.StereotypeEntity);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Вытаскиваем ассоциацию
        /// </summary>
        /// <param name="commonDBObject"> Северный объект</param>
        /// <param name="clientPoint"> Положение на диаграмме</param>
        private void DragDropAssociation(ICommonDBObject commonDBObject, Point clientPoint)
        {
            if (Diagram.FindDiagramEntityByFullName(commonDBObject.Key) != null)
            {
                return;
            }

            DiagramEntity entityA = Diagram.FindDiagramEntityByFullName(((IAssociation)commonDBObject).RoleData.Key);
            if (entityA == null)
            {
                entityA = cmdNew.AddEntity(((IAssociation)commonDBObject).RoleData, new Point(clientPoint.X - 50, clientPoint.Y - 50));
            }

            DiagramEntity entityB = Diagram.FindDiagramEntityByFullName(((IAssociation)commonDBObject).RoleBridge.Key);
            if (entityB == null)
            {
                entityB = cmdNew.AddEntity(((IAssociation)commonDBObject).RoleBridge, new Point(clientPoint.X + 50, clientPoint.Y + 50));
            }

            List<Point> points = new List<Point>();
            points.Add(entityA.Coordinate());
            points.Add(entityB.Coordinate());

            UMLAssociation association =
                UMLAssociationFactory.Create(
                ((IAssociation)commonDBObject).AssociationClassType,
                commonDBObject.Key,
                Guid.NewGuid(),
                Diagram,
                entityA,
                entityB,
                points);
            Diagram.Entities.Add(association);
            association.StereotypeEntity = association.GetUMLAssociationStereotype();
            if (association.StereotypeEntity != null)
            {
                Diagram.Entities.Add(association.StereotypeEntity);
            }
        }

        /// <summary>
        /// Помещает атрибут в объект "Табличный документ".
        /// </summary>
        private void DragDropDataAttribute(IDataAttribute dataAttribute, Point clientPoint)
        {
            List<DiagramEntity> dropToEtity = Diagram.Site.CmdHelper.SelectedCollection(clientPoint);
            if (dropToEtity.Count > 0)
            {
                IEntity entity = (IEntity)Diagram.Site.SchemeEditor.Scheme.GetObjectByKey(dropToEtity[0].Key);

                IDataAttribute newObj = null;
                try
                {
                    newObj = entity.Attributes.CreateItem(AttributeClass.RefAttribute);
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("{0}, необходимо взять на редактирование пакет \"{1}\".", e.Message, ((IPackage)entity.OwnerObject).Name), e);
                }

                IDocumentEntityAttribute newObjD = newObj as IDocumentEntityAttribute;
                if (newObjD != null)
                {
                    newObjD.SetSourceAttribute(dataAttribute);
                }

                ((UMLEntityBase)dropToEtity[0]).RefreshAttrCollection();
                dropToEtity[0].Invalidate();
            }
        }
    }
}
