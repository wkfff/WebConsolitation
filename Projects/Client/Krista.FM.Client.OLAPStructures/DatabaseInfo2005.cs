using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.IO;
using Microsoft.AnalysisServices;

namespace Krista.FM.Client.OLAPStructures
{
	public class DatabaseInfo2005 : DatabaseInfoBase
	{
		//private DatabaseHeader databaseHeader;		

		//protected void Init(Package _package)
		//{
		//    package = _package;
		//    ReadDatabase();
		//}

		//public DatabaseInfo2005(string _packageFileName) : base()
		//{
		//    Init(new Package(_packageFileName));
		//}

		//public DatabaseInfo2005(Package _package) : base()
		//{
		//    Init(_package);			
		//}

		public DatabaseInfo2005(XPathNavigator _databaseNavigator, DatabaseControlBlock _controlBlock)
			: base(_databaseNavigator, _controlBlock)
		{
			ReadDatabase();
		}

		public DatabaseInfo2005(Database DB, DatabaseControlBlock _controlBlock)
			: base(DB, _controlBlock)
		{ }

		protected override void ReadIdentificationInfo()
		{
			if (navigator != null)
				idInfo = IdentificationInfo.ReadFromXML(navigator);
		}

		protected override void ReadDescription()
		{
			description = navigator.GetAttribute("description", "");
		}

		protected override void ReadDataSource()
		{
			dataSource = new DataSourceInfo();
			dataSource.ConnectionString = navigator.GetAttribute("connectionstring", "");
		}

		protected override void ReadDatabase()
		{
			base.ReadDatabase();
		}

		protected override void ReadDimensions()
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> header in ControlBlock.Dimensions.ItemsByName)
			{
				try
				{					
					if ((header.Value as DimensionHeader).DimensionInfo != null)
					{
						dimensions.Add((header.Value as DimensionHeader).DimensionInfo);
					}
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine(e.Message);
				}				
			}			
		}

		protected override void ReadCubes()
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> header in ControlBlock.Cubes.ItemsByName)
			{
				try
				{
					if ((header.Value as CubeHeader).CubeInfo != null)
					{
						cubes.Add((header.Value as CubeHeader).CubeInfo);
					}
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine(e.Message);
				}
				
			}			
		}

        protected override void ReadAnnotations()
        {
            return;
        }
	}
}