﻿using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CreateNewMDAssociation : CommandNewAssosiationBase
    {
        public CreateNewMDAssociation(AbstractDiagram diagram)
            : base(diagram)
        {
        }

        protected override IEntityAssociation Create(DiagramEntity en)
        {
            return Diagram.Site.NewAssociationCommand.Execute((IEntity)Diagram.Site.SelectedShape.CommonObject, (IEntity)en.CommonObject, AssociationClassTypes.MasterDetail);
        }
    }
}
