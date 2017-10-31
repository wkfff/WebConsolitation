using System;
using System.Collections.Generic;
using System.Xml.XPath;
using Microsoft.AnalysisServices;
using Krista.FM.Client.OLAPResources;

namespace Krista.FM.Client.OLAPStructures
{
	class CubeInfo2000 : CubeInfoBase
	{
		const string xPathSourceTable = "./property[@name='SourceTable']";
		const string xPathSubClass = "@SubClassType";
		const string xPathStatement = "./property[@name='Statement']";
        private const string xPathJoin = "./property[@name='JoinClause']";

		protected DatabaseInfo2000 databaseInfo;
		protected List<RelationInfo> relations = new List<RelationInfo>();

		private DatabaseInfo2000 databaseInfo2000
		{
			get { return (DatabaseInfo2000)databaseInfo; }
		}

		public CubeInfo2000(DatabaseInfo2000 _databaseInfo, XPathNavigator _cubeNavigator)
			: base(_cubeNavigator)
		{
			databaseInfo = _databaseInfo;
			ReadIdentificationInfo();
			ReadCube();
		}

		protected bool IsVirtual()
		{
			DSOEnums.SubClassType SubClass = 
                (DSOEnums.SubClassType)navigator.SelectSingleNode(xPathSubClass).ValueAsInt;
			return SubClass == DSOEnums.SubClassType.sbclsVirtual;
		}

		protected override void ReadCube()
		{
		    try
		    {
                ReadDescription();
                ReadCalculations();

                #region Следующие три процедуры обязательно в заданном порядке.
                ReadTableRelations();	//Сначала читаем отношения.
                ReadCubeDimensions();	//После отношений измерения.
                ReadMeasureGroups();	//И в последнюю очередь группы мер.
                #endregion

                GetMeasureGroupTables();
                if (measureGroups.Count > 0) { description = measureGroups[0].Description; }
                OLAPUtils2000.ReadCustomProperties(navigator, Annotations);

		        CreateDsvNameAnnotation();
		    }
		    catch (Exception e)
		    {
		        throw new Exception(String.Format("При обработке куба {0} возникла ошибка {1}", this.Name, e));
		    }
			
		}

        /// <summary>
        /// Создает в аннотации имя объекта в DataSourceView и QueryDefinituion, если он есть
        /// </summary>
        private void CreateDsvNameAnnotation()
        {
            string fromClause = string.Empty;
            if (navigator.SelectSingleNode(xPathSourceTable) != null)
                fromClause = navigator.SelectSingleNode(xPathSourceTable).Value;

            if (!String.IsNullOrEmpty(fromClause))
            {
                fromClause =
                    fromClause.Replace("\"DV\".", string.Empty).Replace("DV.", string.Empty).Replace("\"", string.Empty).Trim();
                AnnotationInfo dsvNameAnnotation = new AnnotationInfo("dsvNameAnnotation", fromClause);
                annotations.Add("DsvSourceName", dsvNameAnnotation);
            }

            string joinClause = string.Empty;
            if (navigator.SelectSingleNode(xPathJoin) != null)
                joinClause = navigator.SelectSingleNode(xPathJoin).Value;

            if (!String.IsNullOrEmpty(joinClause))
            {
                AnnotationInfo joinAnnotation = new AnnotationInfo("joinAnnotation", joinClause);
                annotations.Add("JoinClause", joinAnnotation);
            }
        }

		private MeasureGroupInfo GetLinkedMeasureGroup(string cubeName)
		{
			for (int i = 0; i < linkedMeasureGroups.Count; i++)
				if (linkedMeasureGroups[i].Name.Equals(cubeName, StringComparison.OrdinalIgnoreCase))
				{
					return linkedMeasureGroups[i];
				}
			return null;
		}

		protected void ReadLinkedMeasureGroups()
		{
			XPathNodeIterator measuresNodes = navigator.Select(".//CubeMeasures//CubeMeasure");
			MeasureGroupInfo mgInfo;
			MeasureInfo measureInfo = null;
			while (measuresNodes.MoveNext())
			{
				measureInfo = CreateMeasureInfo(measuresNodes.Current);
				mgInfo = GetLinkedMeasureGroup(measureInfo.SourceColumn.TableName);
				if (mgInfo == null)
				{
					mgInfo = new MeasureGroupInfo(measureInfo.SourceColumn.TableName, Guid.NewGuid());
					mgInfo.IgnoreUnrelatedDimensions = true;
					linkedMeasureGroups.Add(mgInfo);
				}
				mgInfo.Measures.Add(measureInfo);
			}
		}

		protected override void ReadIdentificationInfo()
		{            
            idInfo = new IdentificationInfo(
                navigator.GetAttribute("name", ""), OLAPUtils2000.ReadID(navigator, "ID", Guid.NewGuid()));
		}

		protected override void ReadDescription()
		{
			description = navigator.GetAttribute("Description", "");
		}

		//Ищет общее измерение, которого еще нет в данном кубе.
		private DimensionInfoBase GetSharedDimensionByTableName(string dimensionSafeTableName)
		{
			DimensionInfoBase sharedDimension = null;
			for (int i = 0; i < databaseInfo.Dimensions.Count; i++)
			{
				if (databaseInfo.Dimensions[i].TableName.Equals(
					dimensionSafeTableName, StringComparison.OrdinalIgnoreCase))
				{
					for (int j = 0; j < this.dimensions.Count; j++)
					{
						if (databaseInfo.Dimensions[i].Name.Equals(
							this.dimensions[j].Name, StringComparison.OrdinalIgnoreCase))
							sharedDimension = databaseInfo.Dimensions[i];
					}
					if (sharedDimension != null) { break; }
				}
			}
			return sharedDimension;
		}

		private bool ProcessCustomDimension(string cubeTableName,
			MeasureGroupDimensionInfo mgDimensionInfo, DimensionInfoBase sharedDimensionInfo)
		{
            string name = mgDimensionInfo.CubeDimIdentification.Name;

            // Вариант
            if (name.Equals("Источники данных", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Вариант__ГодОтч", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Вариант__МесОтч", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Вариант__Земельный кадастр", StringComparison.OrdinalIgnoreCase))
            {
                mgDimensionInfo.ReferenceToThis.Add(new ColumnInfo(cubeTableName, "SOURCEID"));
                mgDimensionInfo.DimensionType = MeasureGroupDimensionType.RegularDimension;
                return true;
            }

            /*// Период
            if (name.Equals("Период__ГодУнив", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Период__МесяцУнив", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Период__ДеньУнив", StringComparison.OrdinalIgnoreCase))
            {
                mgDimensionInfo.ReferenceToThis.Add(new ColumnInfo(cubeTableName, "REFYEARMONTHDAY"));
                mgDimensionInfo.DimensionType = MeasureGroupDimensionType.RegularDimension;
                return true;
            }*/
            
			return false;
		}

		private void SetMeasureGroupDimensionFields(MeasureGroupDimensionInfo mgDimensionInfo)
		{
			DimensionInfoBase sharedDimensionInfo = databaseInfo.
				GetSharedDimensionByID(mgDimensionInfo.CubeDimIdentification.ID);
			string cubeTableName = this.measureGroups[0].Partitions[0].Source.Name;
			if (!ProcessCustomDimension(cubeTableName, mgDimensionInfo, sharedDimensionInfo))
			{				
				for (int i = 0; i < relations.Count; i++)
				{
					if (relations[i].PrimaryKey.TableName == sharedDimensionInfo.TableName)
					{
						mgDimensionInfo.ReferenceToThis.Add(relations[i].ForeignKey);
						if (relations[i].ForeignKey.TableName.Equals(cubeTableName,
							StringComparison.OrdinalIgnoreCase))
						{
							mgDimensionInfo.DimensionType = MeasureGroupDimensionType.RegularDimension;
						}
						else
						{
							mgDimensionInfo.DimensionType = MeasureGroupDimensionType.ReferenceDimension;
							mgDimensionInfo.IntermediateGranularityAttrID =
								mgDimensionInfo.ReferenceToThis[0].ColumnName;
							DimensionInfoBase dim = GetSharedDimensionByTableName(
								relations[i].ForeignKey.TableName);

                            if (dim == null)
                                throw new Exception(String.Format("Измерение, построенное на таблице {0} используется в кубе {1}, но не найдено в списке общих измерений", relations[i].ForeignKey.TableName, this.Name));

							mgDimensionInfo.IntermediateDimIdentification =
								new IdentificationInfo(dim.Name, dim.ID);
						}
						break;
					}
				}

				if (mgDimensionInfo.ReferenceToThis.Count == 0)
					mgDimensionInfo.DimensionType = MeasureGroupDimensionType.DegenerateDimension;
			}
		}		

		protected override void ReadCubeDimensions()
		{
			List<DimensionInfoBase> sharedDims = new List<DimensionInfoBase>();
			XPathNodeIterator dimensionsNodes = navigator.Select(
				".//CubeDimensions//CubeDimension");
			while (dimensionsNodes.MoveNext())
				sharedDims.Add(new DimensionInfo2000(
					databaseInfo, dimensionsNodes.Current, this));
			
			//Т.к. измерения должны считаться снизу вверх, а считали мы их сверху вниз,
			//то записываем мы их в цикле downto, т.е. в обратном порядке.
			int startIndex = 1;
			for (int i = sharedDims.Count - 1; i >= 0; i--)
			{
				DimensionInfo2000 sharedDim = (DimensionInfo2000)sharedDims[i];
				if (sharedDim.Calculations.Count > 0)
				{					
					MDXScripts[0].Calculations.InsertRange(
						startIndex, sharedDim.Calculations);
					startIndex = startIndex + sharedDim.Calculations.Count;
				}
			}

			for (int i = 0; i < sharedDims.Count; i++)
			{
				DimensionInfo2000 sharedDim =
					(DimensionInfo2000)databaseInfo.GetSharedDimensionByName(sharedDims[i].Name);				
				if (sharedDim.OlapView != null)
				{
					OLAPUtils2000.ReplaceRelationsTableName(relations, sharedDim.OlapView);
				}				
				dimensions.Add(new CubeDimensionInfo2000(this, sharedDim));
			}
		}

		private MeasureInfo CreateMeasureInfo(XPathNavigator measureNode)
		{
			MeasureInfo measureInfo = new MeasureInfo(
				measureNode.GetAttribute("name", ""), Guid.NewGuid());
			measureInfo.Description = measureNode.GetAttribute("Description", "");
			measureInfo.Visible = measureNode.SelectSingleNode("@IsVisible").ValueAsBoolean;
			measureInfo.FormatString = measureNode.SelectSingleNode("@FormatString").Value;
			measureInfo.SourceColumn = new ColumnInfo(
				measureNode.SelectSingleNode(".//property[@name='SourceColumn']").Value);

		    XPathNavigator aggrFunctionNode =
		        measureNode.SelectSingleNode(".//CustomProperties//Property[@name = 'AggregateFunction']");
		    measureInfo.AggregateFunction = aggrFunctionNode != null
		                                        ? (AggregationFunction) Enum.Parse(
		                                            typeof (AggregationFunction),
		                                            aggrFunctionNode.Value, true)
		                                        : AggregationFunction.Sum;

		    return measureInfo;
		}

		protected void ReadPartitions()
		{
		    try
		    {
                XPathNodeIterator partitionsNodes = navigator.Select(".//Partitions//Partition");
                while (partitionsNodes.MoveNext())
                {
                    string name = partitionsNodes.Current.GetAttribute("name", "");
                    if (!(partitionsNodes.Count > 1 &&
                        name.Equals(this.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        PartitionInfo partitionInfo = new PartitionInfo();
                        partitionInfo.identificationInfo =
                            new IdentificationInfo(name, OLAPUtils2000.ReadID(partitionsNodes.Current, "ID", name));
                        partitionInfo.AggregationPrefix = this.Name + "_" + partitionInfo.identificationInfo.Name;
                        partitionInfo.ProcessingMode = ProcessingMode.Regular;
                        partitionInfo.ProcessingPriority = 0;
                        partitionInfo.Slice = "";

                        string safeTableName = partitionsNodes.Current.
                            SelectSingleNode("./property[@name = 'SourceTable']").Value.
                            Replace("\"", "").Replace("DV.", "");
                        string filterCondition = partitionsNodes.Current.
                            SelectSingleNode("./property[@name = 'SourceTableFilter']").Value.
                            Replace("\"", "").Replace("DV.", "");

                        TableType tableType = TableType.table;
                        if (!string.IsNullOrEmpty(filterCondition))
                        {
                            tableType = TableType.view;
                        }
                        partitionInfo.Source =
                            new TableInfo(safeTableName, string.Empty, TableInfo.GetQueryDefinition(safeTableName, filterCondition), tableType);
                        partitionInfo.StorageMode = StorageMode.Molap;

                        OLAPUtils2000.ReadCustomProperties(navigator.SelectSingleNode(String.Format(".//Partitions//Partition[@name = '{0}']", name)), partitionInfo.Annotations);

                        measureGroups[0].Partitions.Add(partitionInfo);
                    }
                }
		    }
		    catch (Exception e)
		    {
		        throw new Exception(String.Format("При оьработке партиции возникла ошибка: {0}", e));
		    }
		}

		protected override void ReadMeasureGroups()
		{
			if (IsVirtual())
			{
				ReadLinkedMeasureGroups();
				return;
			}
			measureGroups.Add(new MeasureGroupInfo(this.Name, OLAPUtils2000.ReadID(navigator, "MeasureGroupID", Guid.NewGuid())));
			measureGroups[0].Description = Description;
			XPathNodeIterator measuresNodes = navigator.Select(".//CubeMeasures//CubeMeasure");
			while (measuresNodes.MoveNext())
			{
				MeasureInfo measureInfo = CreateMeasureInfo(measuresNodes.Current);
				measureGroups[0].Measures.Add(measureInfo);
			}
			//Сначала читаем разделы, чтобы получить имя таблицы фактов
			//Имя таблицы фактов используется затем при чтении измерений
			ReadPartitions();
			//Для чтения измерений мы должны знать имя таблицы фактов
			MeasureGroupDimensionInfo mgDimensionInfo;
			for (int i = 0; i < this.dimensions.Count; i++)
			{
				mgDimensionInfo = new MeasureGroupDimensionInfo(this.dimensions[i]);
				SetMeasureGroupDimensionFields(mgDimensionInfo);
				measureGroups[0].MGDimensions.Add(mgDimensionInfo);
			}
			OLAPUtils2000.ReadCustomProperties(navigator, measureGroups[0].Annotations);
			versions = new Versions();
			OLAPUtils2000.CreateVersionAnnotations(versions, this.annotations);
		}

		protected void ReadTableRelations()
		{
			relations = OLAPUtils2000.ReadRelations(navigator);
		}

		internal List<RelationInfo> Relations
		{
			get { return relations; }
		}

		protected void ReadStandartCalculations()
		{
			CalculationInfo calculationInfo;
			XPathNodeIterator calculationNodes = navigator.Select(
				".//CubeCommands//CubeCommand");
			while (calculationNodes.MoveNext())
			{
				string name = calculationNodes.Current.GetAttribute("name", "");
				string expression = calculationNodes.Current.SelectSingleNode(xPathStatement).Value;
				string description = calculationNodes.Current.GetAttribute("Description", "");

				expression = expression.Replace("AS '", "AS ");
				if (expression.IndexOf("', SOLVE_ORDER") > 0)
					expression = expression.Replace("', SOLVE_ORDER", ", SOLVE_ORDER");
				else
					if (expression.IndexOf("', DESCRIPTION") > 0)
						expression = expression.Replace("', DESCRIPTION", ", DESCRIPTION");
					else
						if (expression.IndexOf("', CONDITION") > 0)
							expression = expression.Replace("', CONDITION", ", CONDITION");
						else
							if (expression.IndexOf("', FORMAT_STRING") > 0)
								expression = expression.Replace("', FORMAT_STRING", ", FORMAT_STRING");
							else
								if (expression.IndexOf("', VISIBLE") > 0)
									expression = expression.Replace("', VISIBLE", ", VISIBLE");
								else
									if (expression.IndexOf("', CALCULATION_PASS_NUMBER") > 0)
										expression = expression.Replace("', CALCULATION_PASS_NUMBER", ", CALCULATION_PASS_NUMBER");
									else
										if (expression.IndexOf("', NON_EMPTY_BEHAVIOR") > 0)
											expression = expression.Replace("', NON_EMPTY_BEHAVIOR", ", NON_EMPTY_BEHAVIOR");
										else
											expression = expression.Remove(expression.Length - 1);

				//Корректируем выражение для вычисляемых элементов измерения.
				//Будем надеятся, что это единственное измерение с вычисляемыми элементами.
				expression = expression.Replace(
					"CURRENTCUBE.[Значения экономич показателей]",
					"CURRENTCUBE.[Значения экономич показателей].[Значения экономич показателей]");

				expression = expression.Replace("VISIBLE = '0'", "VISIBLE = 0");
				expression = expression.Replace("VISIBLE = '1'", "VISIBLE = 1");

				expression = expression.Replace("'Standard'", "\"Standard\"");
				expression = expression.Replace("'Currency'", "\"Currency\"");
				expression = expression.Replace("'Percent'", "\"Percent\"");

				//expression = expression.Replace("'", "\"");
				calculationInfo = new CalculationInfo(name, expression + ";", description);
				mdxScripts[0].Calculations.Add(calculationInfo);
			}
		}		

		protected override void ReadCalculations()
		{
			mdxScripts.Add(new MDXScriptInfo());
			CalculationInfo calculationInfo = new CalculationInfo("CALCULATE", "CALCULATE;", "");
			mdxScripts[0].Calculations.Add(calculationInfo);
			ReadStandartCalculations();					
		}
	}
}