using System;
using System.Collections.Generic;
using System.Drawing;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Shapes.Factory
{
    public class UMLAssociationFactory
    {
        public static UMLAssociation Create(AssociationClassTypes associationClassType, string key, Guid id, AbstractDiagram diagram, DiagramEntity startEntity, DiagramEntity endEntity, List<Point> selPoints)
        {
            switch (associationClassType)
            {
                    case AssociationClassTypes.BridgeBridge:
                        return new UMLAssociationItSelf(key, id, diagram, startEntity);
                default:
                    return new UMLAssociation(key, id, diagram, startEntity, endEntity, selPoints);
            }
        }
    }
}
