using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CreateNewBridgeAssociation : CommandNewAssosiationBase
    {
        public CreateNewBridgeAssociation(AbstractDiagram diagram)
            : base(diagram)
        {
        }

        protected override IEntityAssociation Create(DiagramEntity en)
        {
            return Diagram.Site.NewAssociationCommand.Execute((IEntity)Diagram.Site.SelectedShape.CommonObject, (IEntity)en.CommonObject, AssociationClassTypes.Bridge);
        }
    }
}
