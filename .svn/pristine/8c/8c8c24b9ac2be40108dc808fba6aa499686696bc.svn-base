using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CreateNewBridge2BridgeAssociation : CommandNewAssosiationBase
    {
        public CreateNewBridge2BridgeAssociation(AbstractDiagram diagram)
            : base(diagram)
        {
        }

        protected override IEntityAssociation Create(DiagramEntity en)
        {
            return Diagram.Site.NewAssociationCommand.Execute((IEntity)Diagram.Site.SelectedShape.CommonObject, (IEntity)en.CommonObject, AssociationClassTypes.BridgeBridge);
        }
    }
}
