using System;
using System.Collections.Generic;
using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Shapes
{
    public class UMLAssociationItSelf : UMLAssociation
    {
        public UMLAssociationItSelf(string key, Guid id, AbstractDiagram diagram, DiagramEntity entityShape)
            : base(key, id, diagram, entityShape, entityShape, GetAssociationPoints(entityShape))
        {
        }

        public override void BeforeDraw()
        {
            ListOfPoints = GetAssociationPoints(ParentDiagramEntity);
        }

        private static List<Point> GetAssociationPoints(DiagramEntity entityShape)
        {
            var points = new List<Point>();
            points.AddRange(new[] 
            { 
                entityShape.Coordinate(),
                new Point(entityShape.EntityRectangle.Left + (entityShape.EntityRectangle.Width / 2), entityShape.EntityRectangle.Top - 30),
                new Point(entityShape.EntityRectangle.Right + 30, entityShape.EntityRectangle.Top - 30),
                new Point(entityShape.EntityRectangle.Right + 30, entityShape.EntityRectangle.Top + (entityShape.EntityRectangle.Height / 2)),
                entityShape.Coordinate() 
            });

            return points;
        }
    }
}
