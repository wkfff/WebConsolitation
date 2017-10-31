using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

using Krista.FM.Common.Xml;

namespace Krista.FM.Client.DiagramEditor.Visitors
{
    /// <summary>
    /// Посетитель формирующий XML представление элементов
    /// </summary>
    internal class Save2XMLVisitor : IVisitor
    {
        /// <summary>
        /// XML-узел в котором будут сохранены все посещаемые элементы
        /// </summary>
        private XmlNode rootNode;

        public Save2XMLVisitor(XmlNode xmlNode)
        {
            rootNode = xmlNode;
        }

        /// <summary>
        /// XML-узел в котором будут сохранены все посещаемые элементы
        /// </summary>
        public XmlNode RootNode
        {
            get { return rootNode; }
        }

        #region IVisitor Members

        public void Visit(DiagramEntity shape)
        {
            if (shape is UMLEntityBase)
            {
                SaveEntity(shape);
            }
            else if (shape is UMLAssociation)
            {
                SaveLink(shape);
            }
            else if (shape is UMLAssociationStereotype)
            {
                SaveStereotypeLabel(shape);
            }
            else if (shape is UMLLabel)
            {
                SaveLabel(shape);
            }
            else if (shape is UMLAnchorEntityToNote)
            {
                SaveAnchor(shape);
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Сохранение класса
        /// </summary>
        private void SaveEntity(DiagramEntity shape)
        {
            XmlNode node = rootNode.OwnerDocument.CreateElement("EntitySymbol");
            XmlHelper.AddChildNode(node, "ID", shape.ID.ToString());
            XmlHelper.AddChildNode(node, "Rect", String.Format("{0},{1},{2},{3}", shape.EntityRectangle.X, shape.EntityRectangle.Y, shape.EntityRectangle.Width, shape.EntityRectangle.Height));
            XmlHelper.AddChildNode(node, "LineColor", Convert.ToString(shape.LineColor.ToArgb()));
            XmlHelper.AddChildNode(node, "LineWidth", shape.LineWidth.ToString());
            XmlHelper.AddChildNode(node, "FillColor", Convert.ToString(shape.FillColor.ToArgb()));
            XmlHelper.AddChildNode(node, "IsShadow", ((UMLEntityBase)shape).IsShadow.ToString());
            XmlHelper.AddChildNode(node, "IsStereotype", ((UMLEntityBase)shape).StereotypeVisible.ToString());
            XmlHelper.AddChildNode(node, "ShadowColor", Convert.ToString(((UMLEntityBase)shape).ShadowColor.ToArgb()));
            XmlHelper.AddChildNode(node, "FontName", String.Format("{0};{1};{2}", shape.Font.Name, shape.Font.Size, shape.Font.Style.ToString()));
            XmlHelper.AddChildNode(node, "SuppressAttributes", ((UMLEntityBase)shape).IsSuppressAttribute.ToString());
            XmlHelper.AddChildNode(node, "FontColor", Convert.ToString(shape.TextColor.ToArgb()));
            XmlHelper.AddChildNode(node, "SQLExpression", Convert.ToString(((UMLEntityBase)shape).SqlExpression));
            XmlHelper.AddChildNode(node, "Object", shape.Key);
            rootNode.AppendChild(node);
        }
               
        private void SaveGeneralLabel(DiagramEntity shape, XmlNode node)
        {
            XmlHelper.AddChildNode(node, "ID", shape.ID.ToString());
            XmlHelper.AddChildNode(node, "Rect", String.Format("{0},{1},{2},{3}", shape.EntityRectangle.X, shape.EntityRectangle.Y, shape.EntityRectangle.Width, shape.EntityRectangle.Height));
            if (!((UMLLabel)shape).IsFormatted)
            {
                XmlHelper.AddChildNode(node, "LineColor", Convert.ToString(shape.LineColor.ToArgb()));
                XmlHelper.AddChildNode(node, "FillColor", Convert.ToString(shape.FillColor.ToArgb()));
            }

            XmlHelper.AddChildNode(node, "FontName", String.Format("{0};{1};{2}", shape.Font.Name, shape.Font.Size, shape.Font.Style.ToString()));
            XmlNode xmlNode = XmlHelper.AddChildNode(node, "Text", String.Empty);
            XmlHelper.AppendCDataSection(xmlNode, shape.Text);
            XmlHelper.AddChildNode(node, "FontColor", Convert.ToString(shape.TextColor.ToArgb()));
            XmlHelper.AddChildNode(node, "IsFormatted", ((UMLLabel)shape).IsFormatted.ToString());
        }

        /// <summary>
        /// Сохранение label
        /// </summary>
        private void SaveLabel(DiagramEntity shape)
        {
            XmlNode node = rootNode.OwnerDocument.CreateElement("TextSymbol");
            SaveGeneralLabel(shape, node);

            rootNode.AppendChild(node);
        }

        private void SaveStereotypeLabel(DiagramEntity shape)
        {
            XmlNode node = rootNode.OwnerDocument.CreateElement("TextSymbol");
            SaveGeneralLabel(shape, node);
            XmlHelper.AddChildNode(node, "AssociateKey", ((UMLAssociationStereotype)shape).Association.Key);
            XmlHelper.AddChildNode(node, "Visible", shape.Visible.ToString());
            XmlHelper.AddChildNode(node, "AssociateID", ((UMLAssociationStereotype)shape).Association.ID.ToString());

            rootNode.AppendChild(node);
        }

        /// <summary>
        /// Сохранение пакета
        /// </summary>
        private void SavePackage(DiagramEntity shape)
        {
            XmlNode node = rootNode.OwnerDocument.CreateElement("PackageSymbol");
            XmlHelper.AddChildNode(node, "ID", shape.ID.ToString());
            XmlHelper.AddChildNode(node, "Rect", String.Format("{0},{1},{2},{3}", shape.EntityRectangle.X, shape.EntityRectangle.Y, shape.EntityRectangle.Width, shape.EntityRectangle.Height));
            XmlHelper.AddChildNode(node, "LineColor", Convert.ToString(shape.LineColor.ToArgb()));
            XmlHelper.AddChildNode(node, "LineWidth", shape.LineWidth.ToString());
            XmlHelper.AddChildNode(node, "FillColor", Convert.ToString(shape.FillColor.ToArgb()));
            XmlHelper.AddChildNode(node, "FontName", String.Format("{0};{1};{2}", shape.Font.Name, shape.Font.Size, shape.Font.Style.ToString()));

            // XmlHelper.AddChildNode(node, "FontColor", Convert.ToString(shape.TextColor.ToArgb()));
            XmlHelper.AddChildNode(node, "Object", shape.Key);
            rootNode.AppendChild(node);
        }

        private string ListOfPointsToString(List<Point> points)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Point p in points)
            {
                if (sb.Length != 0)
                {
                    sb.Append(';');
                }

                sb.AppendFormat("{0},{1}", p.X, p.Y);
            }

            return sb.ToString();
        }

        private void SaveLink(DiagramEntity shape)
        {
            XmlNode node = rootNode.OwnerDocument.CreateElement("LinkSymbol");
            XmlHelper.AddChildNode(node, "ID", shape.ID.ToString());
            if (((UMLAssociation)shape).StereotypeEntity == null)
            {
                ((UMLAssociation)shape).StereotypeEntity = ((UMLAssociation)shape).GetUMLAssociationStereotype();
            }

            SaveGeneralLinkProrties(node, shape);

            XmlHelper.AddChildNode(node, "Object", shape.Key);
            XmlHelper.AddChildNode(node, "SourceSymbol", ((UMLAssociation)shape).ParentDiagramEntity.ID.ToString());
            XmlHelper.AddChildNode(node, "DestinationSymbol", ((UMLAssociation)shape).ChildDiagramEntity.ID.ToString());
            rootNode.AppendChild(node);
        }

        /// <summary>
        /// Сохранение ассоциации между комментарием и классом 
        /// </summary>
        private void SaveAnchor(DiagramEntity shape)
        {
            XmlNode node = rootNode.OwnerDocument.CreateElement("AnchorSymbol");
            XmlHelper.AddChildNode(node, "ID", shape.ID.ToString());

            SaveGeneralLinkProrties(node, shape);

            XmlHelper.AddChildNode(node, "ParentID", ((UMLAnchorEntityToNote)shape).ParentDiagramEntity.ID.ToString());
            XmlHelper.AddChildNode(node, "ChildID", ((UMLAnchorEntityToNote)shape).ChildDiagramEntity.ID.ToString());

            rootNode.AppendChild(node);
        }

        /// <summary>
        /// Общее для ассоциаций
        /// </summary>
        private void SaveGeneralLinkProrties(XmlNode node, DiagramEntity shape)
        {
            XmlHelper.AddChildNode(node, "Rect", String.Format("{0},{1},{2},{3}", shape.EntityRectangle.X, shape.EntityRectangle.Y, shape.EntityRectangle.Width, shape.EntityRectangle.Height));
            XmlHelper.AddChildNode(node, "LineColor", Convert.ToString(shape.Pen.Color.ToArgb()));
            XmlHelper.AddChildNode(node, "LineWidth", shape.LineWidth.ToString());
            XmlHelper.AddChildNode(node, "ListOfPoints", ListOfPointsToString(((UMLAssociationBase)shape).ListOfPoints));
        }

        #endregion Методы
    }
}
