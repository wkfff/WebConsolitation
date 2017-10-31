using System;
using System.Drawing;
using Krista.FM.Client.DiagramEditor.Shapes.Factory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда создания ассоциации
    /// </summary>
    public class CommandNewAssosiationBase : DiagramEditorCommand
    {
        private Point point;

        public CommandNewAssosiationBase(AbstractDiagram diagram)
            : base(diagram)
        {
        }

        public Point Point
        {
            get { return point; }
            set { point = value; }
        }

        public override void Execute()
        {
            if (Diagram.Site.SelectedShape == null)
            {
                return;
            }

            if (Diagram.Site.SelectedShape == Diagram.Site.CmdHelper.SelectedCollection(point)[0]
                && !Diagram.Site.CreateBridge2BridgeAssociation)
            {
                return;
            }

            if ((Diagram.Site.CmdHelper.SelectedCollection(point)[0] is UMLLabel) || (Diagram.Site.SelectedShape is UMLLabel))
            {
                DiagramEntity en = Diagram.Site.CmdHelper.SelectedCollection(point)[0];
                Diagram.Site.Points.Add(en.Coordinate());
                UMLAnchorEntityToNote a = new UMLAnchorEntityToNote(Guid.NewGuid(), Diagram, Diagram.Site.SelectedShape, en, Diagram.Site.Points);
                Diagram.Entities.Add(a);
                Diagram.IsChanged = true;

                Diagram.Site.CmdHelper.FinalizeCreateAssociation();
            }
            else if (!(Diagram.Site.CmdHelper.SelectedCollection(point)[0] is UMLLabel) & !(Diagram.Site.SelectedShape is UMLLabel))
            {
                try
                {
                    DiagramEntity en = Diagram.Site.CmdHelper.SelectedCollection(point)[0];
                    Diagram.Site.Points.Add(en.Coordinate());

                    // Вызываем общий метод создания ассоциации с схеме
                    IEntityAssociation association = Create(en);

                    UMLAssociation a = UMLAssociationFactory.Create(association.AssociationClassType, association.Key, Guid.NewGuid(), Diagram, Diagram.Site.SelectedShape, en, Diagram.Site.Points);
                    Diagram.Entities.Add(a);

                    a.StereotypeEntity = a.GetUMLAssociationStereotype();
                    if (a.StereotypeEntity != null)
                    {
                        Diagram.Entities.Add(a.StereotypeEntity);
                    }

                    Diagram.IsChanged = true;

                    Diagram.Site.SchemeEditor.RefreshPackage(association.ParentPackage.Key);
                    Diagram.Site.SchemeEditor.SelectObject(association.Key, false);
                }
                finally
                {
                    Diagram.Site.SelectedShape = null;
                    Diagram.Site.CmdHelper.FinalizeCreateAssociation();
                }
            }
        }

        protected virtual IEntityAssociation Create(DiagramEntity en)
        {
            return null;
        }
    }
}
