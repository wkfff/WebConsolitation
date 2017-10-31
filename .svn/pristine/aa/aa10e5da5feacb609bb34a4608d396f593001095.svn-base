using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using Microsoft.AnalysisServices;

namespace Krista.FM.Client.OLAPStructures
{
	public class DimensionInfo2005 : DimensionInfoBase
	{
		public DimensionInfo2005()
			: base()
		{ }

		public DimensionInfo2005(XPathNavigator _dimensionNavigator)
			: base(_dimensionNavigator)
		{
			ReadDimension();
		}

		public DimensionInfo2005(
			XPathNavigator _dimensionNavigator, DimensionControlBlock _dimensionControlBlock)
			: base(_dimensionNavigator, _dimensionControlBlock)
		{
			ReadDimension();
		}

		public DimensionInfo2005(Dimension dimension)
			: base(dimension)
		{
		}

		public DimensionInfo2005(Dimension dimension, DimensionControlBlock _dimensionControlBlock)
			: base(dimension, _dimensionControlBlock)
		{
		}

		protected override void ReadDescription()
		{
			description = navigator.GetAttribute("description", "");
		}

		protected override void ReadDimensionType()
		{
			dimensionType = (DimensionType)Enum.Parse(typeof(DimensionType),
				navigator.GetAttribute("dimensiontype", ""));
		}

		protected override void ReadIdentificationInfo()
		{
			idInfo = IdentificationInfo.ReadFromXML(navigator);
		}

		protected override void ReadDimension()
		{
			base.ReadDimension();
			GetUsedTables();
		}

		protected override void ReadAllMemberName()
		{
			allMemberName = navigator.GetAttribute("attributeallmembername", "");
		}

		//protected override void ReadKeyAttribute()
		//{
		//    string name = navigator.SelectSingleNode(
		//        ".//keyattribute//attribute//@name").Value;
		//    keyAttribute = AttributeInfo.ReadFromXML(navigator.SelectSingleNode(
		//        ".//keyattribute//attribute"));
		//}

		protected override void ReadAttributes()
		{
			XPathNodeIterator attrNodes = navigator.Select(".//attributes//attribute");
			while (attrNodes.MoveNext())
				attributes.Add(AttributeInfo.ReadFromXML(attrNodes.Current));
		}		

		protected override void ReadAnnotations()
		{
			XPathNodeIterator nodes = navigator.Select(".//annotations//annotation");
			while (nodes.MoveNext())
			{
				annotations.Add(AnnotationInfo.ReadFromXML(nodes.Current).Name, AnnotationInfo.ReadFromXML(nodes.Current));
			}
			versions = OLAPUtils.VersionsFromAnnotations(annotations);
		}

		protected override void ReadHierarchies()
		{
			XPathNodeIterator hierNodes = navigator.Select(".//hierarchies//hierarchy");
			while (hierNodes.MoveNext())
				hierarchies.Add(HierarchyInfo.ReadFromXML(hierNodes.Current));
		}

        protected override void ChangeDateUNV()
        {
        }

		protected override void GetUsedTables()
		{
			base.GetUsedTables();
		}

		protected override void ReadTableRelations()
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				switch (attributes[i].Usage)
				{
					case AttributeUsage.Key:
						break;
					case AttributeUsage.Parent:
						tableRelations.Add(new RelationInfo(
							attributes[i].KeyColumns[0], this.KeyAttribute.KeyColumns[0]));
						break;
					default:
						if (!attributes[i].KeyColumns[0].TableName.Equals(
							attributes[i].NameColumn.TableName, StringComparison.OrdinalIgnoreCase))
						{
							tableRelations.Add(new RelationInfo(attributes[i].KeyColumns[0],
								new ColumnInfo(attributes[i].NameColumn.TableName, "ID")));
						}
						break;
				}
			}
		}
	}

	public class CubeDimensionInfo2005 : CubeDimensionInfoBase
	{
		public CubeDimensionInfo2005(CubeInfoBase _cubeInfo, DimensionInfoBase _dimInfo)
			: base(_cubeInfo, _dimInfo)
		{ }

		public CubeDimensionInfo2005(XPathNavigator _dimensionNavigator, CubeInfoBase _cubeInfo)
			: base(_dimensionNavigator, _cubeInfo)
		{ }

		protected override void ReadAttributes()
		{
			XPathNodeIterator attrNodes = dimensionNavigator.Select(".//attributes//attribute");
			while (attrNodes.MoveNext())
				attributes.Add(CubeAttributeInfo.ReadFromXML(attrNodes.Current));
		}

		protected override void ReadDescription()
		{
			description = dimensionNavigator.GetAttribute("description", "");
		}

		protected override void ReadDimension()
		{
			base.ReadDimension();
		}

		protected override void ReadHierarchies()
		{
			XPathNodeIterator hierNodes = dimensionNavigator.Select(".//hierarchies//hierarchy");
			while (hierNodes.MoveNext())
				hierarchies.Add(CubeHierarchyInfo.ReadFromXML(hierNodes.Current));
		}

		protected override void ReadIdentificationInfo()
		{
			identificationInfo = IdentificationInfo.ReadFromXML(dimensionNavigator);
		}
	}
}