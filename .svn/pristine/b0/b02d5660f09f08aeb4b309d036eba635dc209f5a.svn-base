using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.AnalysisServices;
using Krista.FM.Client.OLAPResources;
using Krista.FM.Client.Common.Forms;

namespace Krista.FM.Client.OLAPStructures
{
	public class DatabaseInfo2000 : DatabaseInfoBase
	{
	    private XPathNavigator packageNavigator;

        /// <summary>
        /// Список макросов
        /// </summary>
		private XPathNavigator macrosNavigator;

		private XPathNavigator FMMDNavigator;
		private XPathNavigator deletePartitionsNavigator;
		private XPathNavigator guidsNavigator;

        /// <summary>
        /// Информация о источнике
        /// </summary>
	    private XPathNavigator datasourceNavigator;
           	    
		private static bool LoadFileFromPackage(
			XPathNavigator packageNavigator, out XPathNavigator navigatorToLoad, string xPathPackageName)
		{
		    string FileName = String.Empty;
            try
            {
                FileName = packageNavigator.SelectSingleNode(
                    String.Format(".//package{0}", xPathPackageName)).GetAttribute("file", "");
                XPathDocument fileToLoad = new XPathDocument(FileName);
                navigatorToLoad = fileToLoad.CreateNavigator();
                return true;
            }
            catch (ArgumentNullException e)
            {
                throw new Exception("Не удалось создать навигатор по пути " + FileName + ". " + e);
            }
            catch (XmlException e)
            {
                throw new Exception(String.Format("Ошибка в XML данных: {0}. Текст ошибки: {1}", FileName, e.Message));
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
		}

		private void LoadMacrosFile(XPathNavigator packageNavigator)
		{
			LoadFileFromPackage(packageNavigator, out macrosNavigator, "//macros");
		}

        /// <summary>
        /// Инициализация dasourceNavigator
        /// </summary>
        /// <param name="packageNavigator"></param>
        private void LoadDataSourceFile(XPathNavigator packageNavigator)
        {
            LoadFileFromPackage(packageNavigator, out datasourceNavigator, "//packets/packet[@file = \"FMMD_Datasource.xml\"]");
        }

		private void LoadFMMDFile(XPathNavigator packageNavigator)
		{
			try
			{
				LoadFileFromPackage(packageNavigator, out FMMDNavigator,
					"//packets/packet[@order = 2]");
				XPathDocument fileToLoad = new XPathDocument("FMMD_All.xml.guids");
				guidsNavigator = fileToLoad.CreateNavigator();
			}
			catch (Exception)
			{
				guidsNavigator = null;
			}
		}

        /// <summary>
        /// Обрабатываем CustomProperties
        /// </summary>
        protected override void ReadAnnotations()
        {
            try
            {
                OLAPUtils2000.ReadCustomProperties(DatasourceNavigator, annotations);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("При обработке CustomProperties возникла ошибка: {0}", e));
            }
        }

		private void LoadDeletePartitionsFile(XPathNavigator packageNavigator)
		{
            if (packageNavigator.Select("//packets/packet[@file = FMMD_Delete_Partitions.xml]").Count == 0)
                return;

			LoadFileFromPackage(packageNavigator, out deletePartitionsNavigator,
				"//packets/packet[@file = \"FMMD_Delete_Partitions.xml\"]");
		}

		public DatabaseInfo2000(XPathNavigator databaseNavigator)
			: base(databaseNavigator)
		{
			packageNavigator = databaseNavigator;
			ReadDatabase();
		}

		private string ReplaceDimensionNames(string MDXExpression)
		{
			DimensionInfo2000 dimensionInfo;
			for (int i = 0; i < dimensions.Count; i++)
			{
				dimensionInfo = (DimensionInfo2000)dimensions[i];
				if (dimensionInfo.MDXName != null)
				{
                    if (MDXExpression.Contains(dimensionInfo.MDXName.BracketedName) &&
                        MDXExpression[MDXExpression.IndexOf(dimensionInfo.MDXName.BracketedName) - 1] == '.')
                        continue;

					MDXExpression = MDXExpression.Replace(
						dimensionInfo.MDXName.BracketedName, dimensionInfo.MDXName.NewName);
					MDXExpression = MDXExpression.Replace(
						dimensionInfo.MDXName.BracketedName2, dimensionInfo.MDXName.NewName);
				}
			}
			return MDXExpression;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public XPathNavigator ReadPackage()
		{
			try
			{
				LoadMacrosFile(packageNavigator);
				LoadFMMDFile(packageNavigator);
				LoadDeletePartitionsFile(packageNavigator);
                LoadDataSourceFile(packageNavigator);

                return packageNavigator;
			}
			catch (Exception e)
			{
                FormException.ShowErrorForm(e);
                throw;
			}			
		}

		public XPathNavigator GUIDsNavigator
		{
			get { return guidsNavigator; }
		}

	    /// <summary>
	    /// Информация о источнике
	    /// </summary>
	    public XPathNavigator DatasourceNavigator
	    {
	        get
	        {
                if (datasourceNavigator != null)
                    return datasourceNavigator;

                LoadDataSourceFile(packageNavigator);
	            return datasourceNavigator;
	        }
	    }

	    protected override void ReadDatabase()
		{
            try
            {
                ReadPackage();
                base.ReadDatabase();
            }
            catch (Exception)
            {
            }
		}

		protected override void ReadIdentificationInfo()
		{
		    try
		    {
                string name = macrosNavigator.
                SelectSingleNode("//macroslist/macros[@name = \"DatabaseName\"]").Value;
                idInfo = new IdentificationInfo(name, name);
		    }
		    catch (Exception e)
		    {
                throw new Exception(String.Format("При обработке macroslist возникла ошибка: {0}", e));
		    }
		}

		protected override void ReadDescription()
		{

		}

		protected override void ReadDataSource()
		{
		    try
		    {
                dataSource = new DataSourceInfo();
                dataSource.ConnectionString = macrosNavigator.SelectSingleNode(
                    ".//macroslist/macros[@name = \"ConnectionString\"]").Value;
		    }
		    catch (Exception e)
		    {
                throw new Exception(String.Format("При обработке источника данных возникла ошибка (ошибка в connectionString): {0}", e));
		    }
		}

		private CubeInfo2000 GetCubeByName(string cubeName)
		{
			for (int i = 0; i < cubes.Count; i++)
			{
				if (cubes[i].Name.Equals(cubeName, StringComparison.OrdinalIgnoreCase))
					return (CubeInfo2000)cubes[i];
			}
			return null;
		}

		private MeasureGroupInfo GetMeasureGroupByName(CubeInfo2000 cubeInfo, string measureGroupName)
		{
		    try
		    {
                for (int i = 0; i < cubeInfo.MeasureGroups.Count; i++)
                {
                    if (cubeInfo.MeasureGroups[i].Name.Equals(
                        measureGroupName, StringComparison.OrdinalIgnoreCase))
                    {
                        return cubeInfo.MeasureGroups[i];
                    }
                }
                return null;
		    }
		    catch (Exception e)
		    {
		        throw new Exception(e.ToString());
		    }
			
		}

		private MeasureInfo GetMeasureByName(MeasureGroupInfo MGInfo, string measureName)
		{
			for (int i = 0; i < MGInfo.Measures.Count; i++)
			{
				if (MGInfo.Measures[i].Name.Equals(measureName, StringComparison.OrdinalIgnoreCase))
					return MGInfo.Measures[i];
			}
			return null;
		}

		private void CorrectCubeCalculations()
		{
			for (int i = 0; i < cubes.Count; i++)
			{
				for (int j = 0; j < cubes[i].Calculations.Count; j++)
					cubes[i].Calculations[j].Expression = ReplaceDimensionNames(
						cubes[i].Calculations[j].Expression);
			}
		}

		protected override void ReadCubes()
		{
		    try
		    {
                XPathNodeIterator cubesNodes = FMMDNavigator.Select(
                "//XMLDSOConverter//Databases//Database//Cubes//Cube");
                CubeInfoBase cubeInfo;
                cubes = new List<CubeInfoBase>();
                virtualCubes = new List<CubeInfoBase>();
                while (cubesNodes.MoveNext())
                {
                    cubeInfo = new CubeInfo2000(this, cubesNodes.Current);

                    // Виртуальные кубы
                    if (cubeInfo.LinkedMeasureGroups.Count > 0)
                    {
                        virtualCubes.Add(cubeInfo);
                    }
                    else
                    {
                        cubes.Add(cubeInfo);
                    }
                }
                CubeInfo2000 sourceCube;
                MeasureGroupInfo sourceMG;
                MeasureInfo sourceMeasure;
                for (int i = 0; i < virtualCubes.Count; i++)
                {
                    for (int j = 0; j < virtualCubes[i].LinkedMeasureGroups.Count; j++)
                    {
                        sourceCube = GetCubeByName(virtualCubes[i].LinkedMeasureGroups[j].Name);
                        if (sourceCube != null)
                        {
                            sourceMG = GetMeasureGroupByName(
                                sourceCube, virtualCubes[i].LinkedMeasureGroups[j].Name);
                            virtualCubes[i].LinkedMeasureGroups[j].Source = new MeasureGroupBindingInfo(
                                sourceCube.ID, sourceMG.ID);
                            for (int k = 0; k < virtualCubes[i].LinkedMeasureGroups[j].Measures.Count; k++)
                            {
                                sourceMeasure = GetMeasureByName(sourceMG,
                                                                 virtualCubes[i].LinkedMeasureGroups[j].Measures[k].SourceColumn.ColumnName);
                                virtualCubes[i].LinkedMeasureGroups[j].Measures[k].GUID = sourceMeasure.GUID;
                            }
                        }
                    }
                }
                CorrectCubeCalculations();
		    }
		    catch (Exception e)
		    {
		        throw new Exception(String.Format("При обработке кубов возникла ошибка: {0}", e));
		    }
			
		}

		private void CorrectDimensionCalculations()
		{
			for (int i = 0; i < dimensions.Count; i++)
			{
				for (int j = 0; j < dimensions[i].Attributes.Count; j++)
				{
					dimensions[i].Attributes[j].DefaultMember = ReplaceDimensionNames(
						dimensions[i].Attributes[j].DefaultMember);
					if (dimensions[i].Attributes[j].CustomRollUpInfo != null)
					{
						dimensions[i].Attributes[j].CustomRollUpInfo.Expression = ReplaceDimensionNames(
							dimensions[i].Attributes[j].CustomRollUpInfo.Expression);
					}
				}
			}
		}

		protected override void ReadDimensions()
		{
		    try
		    {
                XPathNodeIterator dimensionsNodes = FMMDNavigator.Select(
                "//XMLDSOConverter//Databases//Database//DatabaseDimensions//DatabaseDimension");
                while (dimensionsNodes.MoveNext())
                    dimensions.Add(new DimensionInfo2000(this, dimensionsNodes.Current, null));
                CorrectDimensionCalculations();
		    }
		    catch (Exception e)
		    {
                throw new Exception(String.Format("При обработке измерений возникла ошибка: {0}", e));
		        throw;
		    }
		}

		//protected override void ReadCubesTableRelations(RelationComparer relationComparer)
		//{
		//    for (int i = 0; i < cubes.Count; i++)
		//    {
		//        dataSourceView.TableRelations.AddRange((cubes[i] as CubeInfo2000).Relations);
		//    }
		//}

	}

	public static class OLAPUtils2000
	{
		public static List<RelationInfo> ReadRelations(XPathNavigator objectNavigator)
		{
			List<RelationInfo> joinClauseList = new List<RelationInfo>();
			XPathNavigator joinClauseNavigator = objectNavigator.SelectSingleNode(
				"./property[@name=\"JoinClause\"]");
			if (joinClauseNavigator != null)
			{
			    string safeJoinClause = joinClauseNavigator.Value;
				if (safeJoinClause.Length > 0)
				{
					ReadRelation(safeJoinClause, joinClauseList);
				}
			}
			return joinClauseList;
		}

        public static void ReadRelation(string safeJoinClause, List<RelationInfo> joinClauseList)
	    {
	        RelationInfo relationInfo;
	        safeJoinClause = safeJoinClause.Replace("\"", "");
	        safeJoinClause = safeJoinClause.Replace("DV.", "");

	        string[] relationsDelimiter = { ") AND (", "(", ")" };
	        string[] relationDelimiter = { "=", "." };

	        string[] relations = safeJoinClause.Split(relationsDelimiter,
	                                                  StringSplitOptions.RemoveEmptyEntries);
	        for (int i = 0; i < relations.Length; i++)
	        {
	            string[] relation = relations[i].Split(relationDelimiter,
	                                                   StringSplitOptions.RemoveEmptyEntries);						
	            ColumnInfo table1 = new ColumnInfo(relation[0], relation[1]);
	            ColumnInfo table2 = new ColumnInfo(relation[2], relation[3]);
	            if (table1.ColumnName.Equals("ID", StringComparison.OrdinalIgnoreCase))
	            {
	                relationInfo = new RelationInfo(table2, table1);
	            }
	            else
	            {
	                relationInfo = new RelationInfo(table1, table2);
	            }
	            joinClauseList.Add(relationInfo);
	        }
	    }

	    public static void ReadCustomProperties(XPathNavigator navigator,
			Dictionary<string, AnnotationInfo> annotations)
		{
			XPathNodeIterator propertiesNodes = navigator.Select(".//CustomProperties//Property");
			string propertyName;
			while (propertiesNodes.MoveNext())
			{
				propertyName = propertiesNodes.Current.GetAttribute("name", "");
                if (propertyName.Equals("FullName") || propertyName.Equals("SubClass") ||
                    propertyName.Equals("Версия") || propertyName.Equals("ObjectKey") ||
                    propertyName.Equals("HierarchyID") || propertyName.Equals("Revision") || propertyName.Equals("BatchOperations"))
				{
					AnnotationInfo annotationInfo = new AnnotationInfo(
						propertyName, propertiesNodes.Current.Value);
					annotationInfo.Visibility = AnnotationVisibility.SchemaRowset;
                    if (!annotations.ContainsKey(propertyName))
                        annotations.Add(propertyName, annotationInfo);
				}
			}
		}

		public static void ReplaceRelationsTableName(List<RelationInfo> relations, TableInfo newTable)
		{
			for (int i = 0; i < relations.Count; i++)
			{
				relations[i].PrimaryKey.UpdateTableInfo(newTable);
				relations[i].ForeignKey.UpdateTableInfo(newTable);
			}
		}

		public static void CreateVersionAnnotations(Versions versions, Dictionary<string, AnnotationInfo> annotations)
		{
			versions = new Versions();
			versions.AddVersion(1, 0, 0, 0, "OLAPAdmin", "Конвертация из формата AS2000", DateTime.Now);
            annotations.Add(OLAPResources.Utils.ToXmlNode(new ToXMLDelegate(versions.WriteToXML)).Name,
            new AnnotationInfo(
				OLAPResources.Utils.ToXmlNode(new ToXMLDelegate(versions.WriteToXML)),
				AnnotationVisibility.None));
		}

       
		public static string GetHierarchyName(string dimensionName)
		{
			return dimensionName;
			//int i = dimensionName.IndexOf(OLAPStructures.OLAPUtils.SemanticDelimiter);
			//if (i < 0)
			//{
			//    return dimensionName;
			//}
			//else
			//{
			//    return dimensionName.Substring(i + 1);
			//}
		}

        public static string ReadID(XPathNavigator navigator, string attributeName)
        {
            string id = string.Empty;
            string path = string.Format("./CustomProperties/Property[@name = '{0}']", attributeName);
            if (null != navigator && null != navigator.SelectSingleNode(path))
            {
                id = navigator.SelectSingleNode(path).Value;
            }
            return id;
        }        

        public static string ReadID(XPathNavigator navigator, string attributeName, string defaultId)
        {
            string id = ReadID(navigator, attributeName);
            if (string.IsNullOrEmpty(id))
            {
                id = defaultId;
            }
            return id;
        }

        public static string ReadID(XPathNavigator navigator, string attributeName, Guid defaultId)
        {
            string id = ReadID(navigator, attributeName);
            if (string.IsNullOrEmpty(id))
            {
                id = defaultId.ToString();
            }
            return id;
        }
	}
}