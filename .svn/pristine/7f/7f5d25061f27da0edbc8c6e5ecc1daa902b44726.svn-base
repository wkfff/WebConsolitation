using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using Microsoft.AnalysisServices;

namespace Krista.FM.Client.OLAPStructures
{
	public class CubeInfo2005 : CubeInfoBase
	{
		public CubeInfo2005()
			: base()
		{ }

		public CubeInfo2005(XPathNavigator _cubeNavigator)
			: base(_cubeNavigator)
		{
			ReadCube();
		}

		public CubeInfo2005(XPathNavigator _cubeNavigator, CubeControlBlock _cubeControlBlock)
			: base(_cubeNavigator, _cubeControlBlock)
		{
			ReadCube();
		}

		public CubeInfo2005(Cube cube)
			: base(cube)
		{ }

		public CubeInfo2005(Cube cube, CubeControlBlock _cubeControlBlock)
			: base(cube, _cubeControlBlock)
		{ }

		protected override void ReadCalculations()
		{
			XPathNodeIterator commandsNodes;
			XPathNodeIterator mdxScriptsNodes = navigator.Select(".//mdxscripts//mdxscript");
			while (mdxScriptsNodes.MoveNext())
			{
				mdxScripts.Add(
					new MDXScriptInfo(IdentificationInfo.ReadFromXML(mdxScriptsNodes.Current)));

				commandsNodes = mdxScriptsNodes.Current.Select(".//commands//command");
				while (commandsNodes.MoveNext())
				{
					mdxScripts[mdxScripts.Count - 1].Calculations.Add(new CalculationInfo(
						string.Empty, commandsNodes.Current.Value, string.Empty));
				}
			}
		}

		protected override void ReadCube()
		{
			ReadIdentificationInfo();
			base.ReadCube();
			ReadAnnotations();
		}

		private bool DimIsExcluded(string id)
		{
			for (int i = 0; i < ExcludedDimensions.Count; i++)
			{
				if (ExcludedDimensions[i].ID.Equals(id, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		protected override void ReadCubeDimensions()
		{
			XPathNodeIterator dimNodes = navigator.Select(".//cubedimensions/cubedimension");
			while (dimNodes.MoveNext())
				dimensions.Add(new CubeDimensionInfo2005(dimNodes.Current, this));
		}

		protected override void ReadDescription()
		{
			description = navigator.GetAttribute("description", "");
		}

		protected override void ReadIdentificationInfo()
		{
			if (navigator != null)
				idInfo = IdentificationInfo.ReadFromXML(navigator);
		}

		protected override void ReadMeasureGroups()
		{
			XPathNodeIterator mgNodes = navigator.Select(".//measuregroups//measuregroup");
			while (mgNodes.MoveNext())
			{
				MeasureGroupInfo MGInfo = MeasureGroupInfo.ReadFromXML(
					mgNodes.Current, ExcludedDimensions);
				if (MGInfo.IsLinked) { linkedMeasureGroups.Add(MGInfo); }
				else { measureGroups.Add(MGInfo); }
			}
		}

		protected virtual void ReadAnnotations()
		{
			XPathNodeIterator nodes = navigator.Select(".//annotations//annotation");
			while (nodes.MoveNext())
			{
				annotations.Add(AnnotationInfo.ReadFromXML(nodes.Current).Name, AnnotationInfo.ReadFromXML(nodes.Current));
			}
			versions = OLAPUtils.VersionsFromAnnotations(annotations);
		}
	}
}