using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AnalysisServices;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Xml;
using Krista.FM.Client.OLAPStructures;
using Krista.FM.Client.SQLServerUtils.OLAPExceptions;
using Krista.FM.Client.TreeLogger;
using Krista.FM.Client.SQLServerUtils.Properties;


namespace Krista.FM.Client.SQLServerUtils
{
	public class SQLServerUtils
	{
		private static DatabaseInfoBase databaseInfo = null;

		private static Database DB = null;
		private static DataSourceView DSView = null;

        private static Dictionary<string, string> calculationsToUpdate = new Dictionary<string, string>();

		private static Log log = null;

		private static LogLongOperation opPumpTables = null;
		private static LogLongOperation opCreateRelations = null;
		private static LogLongOperation opRestoreDimensions = null;
		private static LogLongOperation opRestoreCubes = null;
		private static LogLongOperation opConnectToDataWarehouse = null;
        private static LogLongOperation opUpdateCalculations = null;
		private static LogLongOperation globalOp = null;

		private static OleDbConnection connection = null;

		private static void ProcessException(OLAPException parentException, Exception childException)
		{
			if (parentException != null)
				parentException.AddChild(childException);
			else
				throw childException;
		}

		private static void CreateMgDimensionProcessException(OLAPException parentException,
			Exception e, string dimensionName)
		{
			CannotCreateMeasureGroupDimensionException currentException =
					new CannotCreateMeasureGroupDimensionException(String.Format(
					"Не могу добавить измерение \"{0}\"", dimensionName));
			ProcessException(currentException, e);
			ProcessException(parentException, currentException);
		}

		private static DegenerateMeasureGroupDimension CreateDegenerateMeasureGroupDimension
			(MeasureGroupDimensionInfo mgDimensionInfo, CubeDimension cubeDimension,
			OLAPException parentException)
		{
			DegenerateMeasureGroupDimension degMgDimension = null;
			try
			{
				degMgDimension = new DegenerateMeasureGroupDimension(cubeDimension.ID);
				MeasureGroupAttribute mgAttr = degMgDimension.Attributes.Add(
					cubeDimension.Dimension.KeyAttribute.ID);
				mgAttr.Type = MeasureGroupAttributeType.Granularity;
				for (int i = 0; i < cubeDimension.Dimension.KeyAttribute.KeyColumns.Count; i++)
					mgAttr.KeyColumns.Add(cubeDimension.Dimension.KeyAttribute.KeyColumns[i].Clone());
				//mgAttr.KeyColumns.Add(CreateDataItem(new ColumnInfo(
				//    mgDimensionInfo.Dimension.SafeTableName, "ID"), null));
				return degMgDimension;
			}
			catch (Exception e)
			{
				CreateMgDimensionProcessException(parentException, e,
					mgDimensionInfo.CubeDimIdentification.Name);
				return null;
			}
		}

		private static RegularMeasureGroupDimension CreateRegularMeasureGroupDimension
			(MeasureGroupDimensionInfo mgDimensionInfo, CubeDimension cubeDimension,
			OLAPException parentException)
		{
			RegularMeasureGroupDimension regMgDimension = null;
			try
			{
				regMgDimension = new RegularMeasureGroupDimension(cubeDimension.ID);
				MeasureGroupAttribute mgAttr = regMgDimension.Attributes.Add(
					cubeDimension.Dimension.KeyAttribute.ID);
				mgAttr.Type = MeasureGroupAttributeType.Granularity;
				for (int i = 0; i < mgDimensionInfo.ReferenceToThis.Count; i++)
					mgAttr.KeyColumns.Add(CreateDataItem(mgDimensionInfo.ReferenceToThis[i], null));
				return regMgDimension;
			}
			catch (Exception e)
			{
				CreateMgDimensionProcessException(parentException, e,
					mgDimensionInfo.CubeDimIdentification.Name);
				return null;
			}
		}

		private static void MaterializeIntermediateDimension(ReferenceMeasureGroupDimension dim)
		{
		}

		private static ReferenceMeasureGroupDimension CreateReferenceMeasureGroupDimension
			(MeasureGroupDimensionInfo mgDimensionInfo, CubeDimension cubeDimension,
			MeasureGroup measureGroup, OLAPException parentException)
		{
			ReferenceMeasureGroupDimension refMgDimension = null;
			try
			{
				MeasureGroupDimension intermediateDim = measureGroup.Dimensions.
					Find(mgDimensionInfo.IntermediateDimIdentification.ID);
				if (intermediateDim == null)
					throw new CannotFindMeasureGroupDimensionException(
						measureGroup.Name,
						mgDimensionInfo.IntermediateDimIdentification.Name);

				DimensionAttribute dimAttr = intermediateDim.Dimension.Attributes.
					Find(mgDimensionInfo.IntermediateGranularityAttrID);
				if (dimAttr == null)
					throw new CannotFindAttributeInDimensionException(
						intermediateDim.Dimension.Name,
						mgDimensionInfo.IntermediateGranularityAttrID);

				refMgDimension = new ReferenceMeasureGroupDimension();
				refMgDimension.CubeDimensionID = cubeDimension.ID;
				refMgDimension.IntermediateCubeDimensionID = intermediateDim.CubeDimensionID;
				refMgDimension.IntermediateGranularityAttributeID = dimAttr.ID;
				MeasureGroupAttribute mgAttr = refMgDimension.Attributes.
					Add(cubeDimension.Dimension.KeyAttribute.ID);
				mgAttr.Type = MeasureGroupAttributeType.Granularity;
				for (int i = 0; i < mgDimensionInfo.ReferenceToThis.Count; i++)
					mgAttr.KeyColumns.Add(CreateDataItem(mgDimensionInfo.ReferenceToThis[i], null));
				refMgDimension.Materialization = ReferenceDimensionMaterialization.Regular;
				return refMgDimension;
			}
			catch (Exception e)
			{
				CreateMgDimensionProcessException(parentException, e,
					mgDimensionInfo.CubeDimIdentification.Name);
				return null;
			}
		}

		private static void CreateMeasureGroupMeasures(MeasureGroup measureGroup,
			List<MeasureInfo> measures, OLAPException parentException)
		{
			for (int i = 0; i < measures.Count; i++)
			{
				try
				{
					DataItem measureSourceItem = null;
					measureSourceItem = CreateDataItem(measures[i].SourceColumn, null);
					Measure measure = measureGroup.Measures.Add(measures[i].Name, measures[i].ID);
                    measure.AggregateFunction = !(DB.Parent.Edition == ServerEdition.Enterprise ||
                                                 DB.Parent.Edition == ServerEdition.Enterprise64 ||
                                                 DB.Parent.Edition == ServerEdition.Developer ||
                                                 DB.Parent.Edition == ServerEdition.Developer64 ||
                                                 DB.Parent.Edition == ServerEdition.Evaluation)
				                                    ? AggregationFunction.Sum
				                                    : measures[i].AggregateFunction;
					measure.FormatString = measures[i].FormatString;
					measure.Visible = measures[i].Visible;
					measure.Source = measureSourceItem;
					if (measure.Source.DataType == OleDbType.WChar)
					{
						//TODO: Упомянуть в сообщении о сделанной задаче, что строковые меры в кубах недопустимы ("Регион_Параметры МО 131ФЗ")						
						measure.DataType = MeasureDataType.Double;
						measure.AggregateFunction = AggregationFunction.Count;
						measure.FormatString = "";
					}
				}
				catch (Exception e)
				{
					CantCreateMeasureException currentException = new CantCreateMeasureException(
						measures[i].Name, measureGroup.Name);
					ProcessException(currentException, e);
					ProcessException(parentException, currentException);
				}
			}
		}

		private static int CompareDimensionsByType(
			MeasureGroupDimensionInfo leftDim, MeasureGroupDimensionInfo rightDim)
		{
			if (leftDim == rightDim)
			{
				return 0;
			}
			if (leftDim.DimensionType == MeasureGroupDimensionType.ReferenceDimension)
			{
				if (rightDim.DimensionType == MeasureGroupDimensionType.DegenerateDimension ||
					rightDim.DimensionType == MeasureGroupDimensionType.RegularDimension)
				{
					return 1;
				}
				else				
				{
					//if (leftDim.IntermediateDimIdentification.ID.Equals(
					//rightDim.CubeDimIdentification.ID, StringComparison.OrdinalIgnoreCase))
					//{
					//    return 1;
					//}
					//else
					//{
					//    return -1;
					//}
					if (rightDim.IntermediateDimIdentification.ID.Equals(
					leftDim.CubeDimIdentification.ID, StringComparison.OrdinalIgnoreCase))
					{
						return -1;
					}
					else
					{
						return 1;
					}
				}

			}
			else
			{
				if (rightDim.DimensionType == MeasureGroupDimensionType.DegenerateDimension ||
					rightDim.DimensionType == MeasureGroupDimensionType.RegularDimension)
				{
					return 0;
				}
				else
				{
					return -1;
				}
			}
		}

		private static bool IntermediateDimensionFound(
			List<MeasureGroupDimensionInfo> listToSearch, string Id)
		{
			for (int i = 0; i < listToSearch.Count; i++)
			{
				if (listToSearch[i].CubeDimIdentification.ID.Equals(Id, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Сортируем, чтобы сначала были
		/// 1) измерения, ссылающиеся на таблицу фактов
		/// 2) измерения, ссылающиеся на 1
		/// 3) измерения, ссылающиеся на 2
		/// </summary>		
		private static List<MeasureGroupDimensionInfo> SortDimensions(
			List<MeasureGroupDimensionInfo> listToSort)
		{
			List<MeasureGroupDimensionInfo> sortedList =
				new List<MeasureGroupDimensionInfo>(listToSort.Count);
			int nonReferenceCounter = 0;
			for (int i = 0; i < listToSort.Count; i++)
			{
				if (listToSort[i].DimensionType != MeasureGroupDimensionType.ReferenceDimension)
				{
					sortedList.Add(listToSort[i]);
					nonReferenceCounter++;
				}
			}			
			while (listToSort.Count > nonReferenceCounter)
			{
				int j = 0;
				while (j < listToSort.Count)
				{
					if (listToSort[j].DimensionType == MeasureGroupDimensionType.ReferenceDimension)
					{
						if (IntermediateDimensionFound(
							sortedList, listToSort[j].IntermediateDimIdentification.ID))
						{
							sortedList.Add(listToSort[j]);
							listToSort.Remove(listToSort[j]);
						}
						else
							j++;
					}
					else
						j++;
				}				
			}
			return sortedList;
		}

		private static void CreateMeasureGroupDimensions(MeasureGroup measureGroup,
			Cube cube, CubeInfoBase cubeInfo, List<MeasureGroupDimensionInfo> mgDimensionsInfo,
			OLAPException parentException)
		{
			//TODO Разобраться, почему не работает сортировка...
			//Измерения с типом Reference должны создаваться последними - сортируем список...
			//mgDimensionsInfo.Sort(CompareDimensionsByType);
			
			mgDimensionsInfo = SortDimensions(mgDimensionsInfo);
			for (int i = 0; i < mgDimensionsInfo.Count; i++)
			{
				if (!cubeInfo.ExcludedDimensions.Contains(
					mgDimensionsInfo[i].CubeDimIdentification.ID))
				{
					try
					{
						CubeDimension cubeDimension = cube.Dimensions.Find(
							mgDimensionsInfo[i].CubeDimIdentification.ID);
						if (cubeDimension == null)
							throw new CannotFindCubeDimensionException(
								cube.Name, mgDimensionsInfo[i].CubeDimIdentification.Name);

						MeasureGroupDimension MGDimension = null;
						switch (mgDimensionsInfo[i].DimensionType)
						{
							case MeasureGroupDimensionType.RegularDimension:
								MGDimension = CreateRegularMeasureGroupDimension(
									mgDimensionsInfo[i], cubeDimension, null);
								break;
							case MeasureGroupDimensionType.ReferenceDimension:
								MGDimension = CreateReferenceMeasureGroupDimension(
									mgDimensionsInfo[i], cubeDimension, measureGroup, null);
								break;
							case MeasureGroupDimensionType.DegenerateDimension:
								MGDimension = CreateDegenerateMeasureGroupDimension(
									mgDimensionsInfo[i], cubeDimension, null);
								break;
							default:
								break;
						}
						if (MGDimension != null)
							measureGroup.Dimensions.Add(MGDimension);
					}
					catch (Exception e)
					{
						ProcessException(parentException, e);
					}
				}
			}
		}

		private static void CreateMultiplePartitions(MeasureGroup measureGroup,
			List<PartitionInfo> partitions)
		{
			for (int i = 0; i < partitions.Count; i++)
			{
                Partition partition = measureGroup.Partitions.Add(
                    partitions[i].identificationInfo.Name, partitions[i].identificationInfo.ID);
				partition.AggregationPrefix = partitions[i].AggregationPrefix;
				partition.Description = partitions[i].Description;				
				partition.ProcessingMode = partitions[i].ProcessingMode;
				partition.ProcessingPriority = partitions[i].ProcessingPriority;
				partition.Slice = partitions[i].Slice;
				partition.StorageLocation = partitions[i].StorageLocation;
				partition.StorageMode = partitions[i].StorageMode;
				if (partitions[i].Source.TableType == TableType.table)				
				{
					partition.Source = new TableBinding(
						DSView.ID, Resources.defaultSchemaName, partitions[i].Source.Name);
				}
				else
					partition.Source = new QueryBinding(DSView.ID, partitions[i].Source.QueryDefinition);

                CreateAnnotations(partition.Annotations, partitions[i].Annotations);
			}
		}

		private static void CreateSinglePartition(MeasureGroup measureGroup,
			List<PartitionInfo> partitions)
		{
			//Partition partition = measureGroup.Partitions.Add(measureGroup.Name);
            Partition partition = measureGroup.Partitions.Add(
                    measureGroup.Name, partitions[0].identificationInfo.ID);
			partition.AggregationPrefix = measureGroup.Name + "_" + measureGroup.Name;
			partition.Description = measureGroup.Description;
			partition.StorageMode = StorageMode.Molap;
			partition.ProcessingMode = ProcessingMode.Regular;
			
			partition.Source = new TableBinding(
				DSView.ID, Resources.defaultSchemaName, partitions[0].Source.Name);

            CreateAnnotations(partition.Annotations, partitions[0].Annotations);
		}

		private static void CreateMeasureGroupPartitions(MeasureGroup measureGroup,
			List<PartitionInfo> partitions)
		{
			//CreateSinglePartition(measureGroup, partitions);
			if (DB.Parent.Edition == ServerEdition.Enterprise ||
				DB.Parent.Edition == ServerEdition.Enterprise64 ||
				DB.Parent.Edition == ServerEdition.Developer ||
				DB.Parent.Edition == ServerEdition.Developer64 ||
                DB.Parent.Edition == ServerEdition.Evaluation)
			{
				CreateMultiplePartitions(measureGroup, partitions);
			}
			else
			{
				CreateSinglePartition(measureGroup, partitions);
			}
		}

		private static Annotation CreateAnnotation(AnnotationInfo annInfo)
		{
			Annotation annotation = new Annotation(annInfo.Name, annInfo.Value);
			annotation.Visibility = annInfo.Visibility;
			return annotation;
		}

		private static void CreateAnnotations(
			AnnotationCollection annotations, Dictionary<string, AnnotationInfo> annotationsInfo)
		{
			foreach(AnnotationInfo info in annotationsInfo.Values)
                annotations.Add(CreateAnnotation(info));
		}

		private static void CreateCubeMeasureGroup(Cube cube, CubeInfoBase cubeInfo,
			MeasureGroupInfo measureGroupInfo, OLAPException parentException)
		{
			MeasureGroup measureGroup = null;
			CannotCreateCubeMeasureGroupException currentException =
				new CannotCreateCubeMeasureGroupException(
					String.Format("Ошибки при добавлении группы мер \"{0}\" в куб \"{1}\"",
					measureGroupInfo.Name, cubeInfo.Name));
			try
			{
				//Обязательно сначала добавляем группу мер в куб,
				//иначе возникнут ошибки при добавлении измерений!!!
				measureGroup = cube.MeasureGroups.Add(measureGroupInfo.Name,
					measureGroupInfo.ID);

				measureGroup.AggregationPrefix = measureGroupInfo.AggregationPrefix;
				measureGroup.DataAggregation = measureGroupInfo.DataAggregation;
				measureGroup.Description = measureGroupInfo.Description;
				measureGroup.IgnoreUnrelatedDimensions = measureGroupInfo.IgnoreUnrelatedDimensions;
				measureGroup.ProcessingMode = measureGroupInfo.ProcessingMode;
				measureGroup.ProcessingPriority = measureGroupInfo.ProcessingPriority;
				measureGroup.StorageLocation = measureGroupInfo.StorageLocation;
				measureGroup.StorageMode = measureGroupInfo.StorageMode;
				measureGroup.Type = measureGroupInfo.Type;

				CreateMeasureGroupMeasures(
					measureGroup, measureGroupInfo.Measures, currentException);
				
				CreateMeasureGroupDimensions(
					measureGroup, cube, cubeInfo, measureGroupInfo.MGDimensions, currentException);				

				if (measureGroup.Dimensions.Count <= 0)
					throw new MeasureGroupHasNoDimensionsException(String.Format(
						"Группа мер \"{0}\"не содержит ни одного измерения", measureGroup.Name));

				CreateMeasureGroupPartitions(measureGroup, measureGroupInfo.Partitions);
				CreateAnnotations(measureGroup.Annotations, measureGroupInfo.Annotations);

				if (currentException.Children.Count > 0)
					ProcessException(parentException, currentException);
			}
			catch (Exception e)
			{
				if (measureGroup != null)
					cube.MeasureGroups.Remove(measureGroup);
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
			}
		}

		private static void CreateCubeCalculation(Cube cube, CalculationInfo calculationInfo)
		{
			CreateCubeCalculation(cube, calculationInfo.Expression);
		}

		private static void CreateCubeCalculation(Cube cube, string expression)
		{
			cube.DefaultMdxScript.Commands.Add(new Command(expression));
		}

		public static void CreateRelations(List<RelationInfo> relations,
			OLAPException parentException)
		{
            if (relations.Count > 0)
            {
                opCreateRelations = globalOp.AddLongOperation(Resources.msgCreateRelations,
                    TreeLogger.OperationStatus.Waiting, 10, relations.Count);
                StartOperation(opCreateRelations);
                try
                {
                    RelationComparer relationComparer = new RelationComparer();
                    relations.Sort(relationComparer);
                    for (int i = 0; i < relations.Count; i++)
                        AddRelation(relations[i], parentException);
                    DSView.Update();
                    EndOperation(opCreateRelations);
                }
                catch (Exception e)
                {
                    EndOperationWithErrors(opCreateRelations, e);
                }
            }
		}

		private static void CreateCubeDimension(Cube cube,
			CubeDimensionInfoBase cubeDimensionInfo, OLAPException parentException)
		{
			try
			{
				if (DB.Dimensions.Find(cubeDimensionInfo.DimensionID) != null)
				{
					CubeDimension cubeDimension = cube.Dimensions.Add(cubeDimensionInfo.DimensionID,
						cubeDimensionInfo.Name, cubeDimensionInfo.ID);
					cubeDimension.AllMemberAggregationUsage =
						cubeDimensionInfo.AllMemberAggregationUsage;
					for (int i = 0; i < cubeDimensionInfo.Attributes.Count; i++)
					{
						CubeAttribute cubeAttribute = cubeDimension.Attributes.Find(
							cubeDimensionInfo.Attributes[i].AttributeID);
						if (cubeAttribute != null)
						{
							cubeAttribute.AggregationUsage =
								cubeDimensionInfo.Attributes[i].AggregationUsage;
							cubeAttribute.AttributeHierarchyEnabled =
								cubeDimensionInfo.Attributes[i].AttributeHierarchyEnabled;
							cubeAttribute.AttributeHierarchyOptimizedState =
								cubeDimensionInfo.Attributes[i].AttributeHierarchyOptimizedState;
							cubeAttribute.AttributeHierarchyVisible =
								cubeDimensionInfo.Attributes[i].AttributeHierarchyVisible;
						}
					}
					//А вот нельзя изменять Description - 
					//вылезает исключение при вызове метода cube.Update(UpdateOptions.ExpandFull).
					//cubeDimension.Description = cubeDimensionInfo.Description;
					for (int i = 0; i < cubeDimensionInfo.Hierarchies.Count; i++)
					{
						CubeHierarchy cubeHierarchy = cubeDimension.Hierarchies.Find(
							cubeDimensionInfo.Hierarchies[i].HierarchyID.ToString());
						if (cubeHierarchy != null)
						{
							cubeHierarchy.Enabled = cubeDimensionInfo.Hierarchies[i].Enabled;
							cubeHierarchy.OptimizedState =
								cubeDimensionInfo.Hierarchies[i].OptimizedState;
							cubeHierarchy.Visible = cubeDimensionInfo.Hierarchies[i].Visible;
						}
					}
					cubeDimension.HierarchyUniqueNameStyle =
						cubeDimensionInfo.HierarchyUniqueNameStyle;
					cubeDimension.MemberUniqueNameStyle = cubeDimensionInfo.MemberUniqueNameStyle;
					cubeDimension.Visible = cubeDimensionInfo.Visible;
				}
			}
			catch (Exception e)
			{
				CannotCreateCubeDimensionException currentException =
					new CannotCreateCubeDimensionException(String.Format(
					"Ошибка при добавлении измерения \"{0}\" в куб \"{1}\"",
					cubeDimensionInfo.Name, cube.Name));
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
			}
		}

		private static void CreateCubeDimensions(Cube cube, CubeInfoBase cubeInfo,
			OLAPException parentException)
		{
			for (int i = 0; i < cubeInfo.Dimensions.Count; i++)
				if (!cubeInfo.ExcludedDimensions.Contains(
					cubeInfo.Dimensions[i].IdentificationInfo.ID))
				{
					CreateCubeDimension(cube, cubeInfo.Dimensions[i], parentException);
				}
		}

		private static void CreateCubeScript(ref Cube cube, CubeInfoBase cubeInfo)
		{
			cube.MdxScripts.Add(new MdxScript("Default", "Default"));
		}

		private static void CreateCubeMeasureGroups(ref Cube cube, CubeInfoBase cubeInfo,
			bool nonLinkedOnly, OLAPException parentException)
		{
			if (nonLinkedOnly)
			{
				for (int i = 0; i < cubeInfo.MeasureGroups.Count; i++)
					CreateCubeMeasureGroup(
						cube, cubeInfo, cubeInfo.MeasureGroups[i], parentException);
			}
			else
			{
				for (int i = 0; i < cubeInfo.MeasureGroups.Count; i++)
					if (cube.MeasureGroups.Find(cubeInfo.MeasureGroups[i].ID) == null)
					{
						CreateCubeMeasureGroup(cube, cubeInfo, cubeInfo.MeasureGroups[i],
							parentException);
					}
				for (int i = 0; i < cubeInfo.LinkedMeasureGroups.Count; i++)
					CreateLinkedMeasureGroup(ref cube, cubeInfo.LinkedMeasureGroups[i],
						parentException);
			}

		}

		private static void CreateCubeCalculations(ref Cube cube, CubeInfoBase cubeInfo)
		{
			CreateCubeScript(ref cube, cubeInfo);
			if (cubeInfo.Calculations != null)
				for (int i = 0; i < cubeInfo.Calculations.Count; i++)
					CreateCubeCalculation(cube, cubeInfo.Calculations[i]);
		}

		//private static void CreateVersionAnnotation(
		//    AnnotationCollection annotations, Version versionInfo)
		//{
		//    AddAnnotation(annotations,
		//        "version", OLAPUtils.VersionToNode(versionInfo), AnnotationVisibility.None);
		//}

		private static void CreateControlBlockAnnotation(
			AnnotationCollection annotations, ControlBlock controlBlock)
		{
			AddAnnotation(annotations,
				"controlblock", controlBlock.ToXMLNode(), AnnotationVisibility.None);
		}

		private static void AddAnnotation(AnnotationCollection annotations,
			string name, XmlNode node, AnnotationVisibility visibility)
		{
			Annotation annotation = new Annotation(name, node);
			annotation.Visibility = visibility;
			annotations.Add(annotation);
		}

		private static void ProcessCubeException(
			OLAPException currentException, Exception e, LogLongOperation logLongOp)
		{
			ProcessException(currentException, e);
			EndOperationWithErrors(logLongOp, currentException);
		}

		public static void TryCreateCube(CubeInfoBase cubeInfo, bool immediatelyUpdate, bool firstPass)
		{
			if (cubeInfo.BlockRestoreReason != null)
			{
				AddOperation(opRestoreCubes, string.Format("Не могу добавить куб \"{0}\" - {1}",
					cubeInfo.Name, cubeInfo.BlockRestoreReason.Message),
					OperationStatus.FinishedWithErrors);
			}
			else
			{
				CreateCube(cubeInfo, immediatelyUpdate, firstPass);
			}
		}

		public static void CreateCube(CubeInfoBase cubeInfo, bool immediatelyUpdate, bool firstPass)
		{
			LogLongOperation logLongOp = AddLongOperation(opRestoreCubes,
				Resources.msgAppendCube, cubeInfo.Name);

			CannotCreateCubeException currentException = new CannotCreateCubeException(
				"Ошибки при добавлении куба");
			try
			{
			Cube cube = DB.Cubes.Find(cubeInfo.ID);

				if (firstPass || cubeInfo.LinkedMeasureGroups.Count > 0)
				{
					if (cubeInfo.Dimensions.Count == 0)
						throw new CubeDimensionsListIsEmptyException();
					if (cubeInfo.MeasureGroups.Count == 0 && cubeInfo.LinkedMeasureGroups.Count == 0)
						throw new CubeMeasureGroupsListIsEmptyException();

					if (cube != null)
					{
						ProcessImpactObjects(cube);
						cube = null;
					}
					if (cube == null) { cube = DB.Cubes.Add(cubeInfo.Name, cubeInfo.ID); }

					cube.Description = cubeInfo.Description;
					cube.Source = new DataSourceViewBinding(DSView.Name);
					cube.StorageMode = StorageMode.Molap;

					CreateCubeDimensions(cube, cubeInfo, currentException);
					if (cube.Dimensions.Count == 0)
						throw new CubeHasNoDimensionsException();

					if (cubeInfo.MeasureGroups.Count > 0)
						CreateCubeMeasureGroups(ref cube, cubeInfo, true, currentException);
					else
						CreateCubeMeasureGroups(ref cube, cubeInfo, false, currentException);					

					if (cube.MeasureGroups.Count == 0 && cubeInfo.LinkedMeasureGroups.Count == 0)
						throw new CubeHasNoMeasureGroupsException();

					CreateCubeCalculations(ref cube, cubeInfo);
					CreateAnnotations(cube.Annotations, cubeInfo.Annotations);					
					CreateControlBlockAnnotation(cube.Annotations, cubeInfo.ControlBlock);
				}
				else
				{
					CreateCubeMeasureGroups(ref cube, cubeInfo, false, currentException);
					if (cube.MeasureGroups.Count == 0)
						throw new CubeHasNoMeasureGroupsException();
				}

			    ValidationErrorCollection errorCollection = new ValidationErrorCollection();

                if (!cube.Validate(errorCollection, true))
                {
                    foreach (ValidationError error in errorCollection)
                    {
                        AddOperation(logLongOp,
                                     String.Format("При валидации куба обнаружена ошибка: {0}, код ошибки: {1}",
                                                   error.FullErrorText, error.ErrorCode),
                                     OperationStatus.FinishedWithWarnings);
                    }
                }

			    if (immediatelyUpdate)
			    {
			        cube.Update(UpdateOptions.ExpandFull, cubeInfo.UpdateMode);
			    }

				if (currentException.Children.Count > 0)
					EndOperationWithWarnings(logLongOp, currentException);
				else
					EndOperation(logLongOp);
			}
			catch (Exception e)
			{
				ProcessCubeException(currentException, e, logLongOp);
			}
		}

		private static MeasureInfo GetMeasure(MeasureGroupInfo MGInfo, string measureID)
		{
			for (int i = 0; i < MGInfo.Measures.Count; i++)
			{
				//if (MGInfo.Measures[i].SourceColumn.ColumnName.Equals(
				//    measureName, StringComparison.OrdinalIgnoreCase))
				if (MGInfo.Measures[i].ID.Equals(
					measureID, StringComparison.OrdinalIgnoreCase))
				{
					return MGInfo.Measures[i];
				}
			}
			return null;
		}

		private static void FilterLinkedMeasureGroupMeasures(
			ref MeasureGroup MG, MeasureGroupInfo MGInfo)
		{
			int i = 0;
			while (i < MG.Measures.Count)
			{
				MeasureInfo measureInfo = GetMeasure(MGInfo, MG.Measures[i].ID);
				if (measureInfo != null)
				{
					//При линковании группы мер названия мер копируются из исходного куба.
					//Однако в прилинкованной группе мер они могут быть переименованы пользователем
					//и их имена перестанут совпадать с именами мер в исходном кубе.
					//Поэтому ищем меру по ID и переименовываем ее так, как назвал пользователь.
					MG.Measures[i].Name = measureInfo.Name;
					i += 1;
				}
				else
					MG.Measures.RemoveAt(i, true);
			}
		}

		private static void AddLinkedMeasureGroupDimensions(
			MeasureGroupDimensionCollection sourceDimensions,
			ref MeasureGroup measureGroup, Cube cube, OLAPException parentException)
		{
			MeasureGroupDimension mgDimension = null;

			for (int i = 0; i < sourceDimensions.Count; i++)
			{
				try
				{
					CubeDimension cubeDimension = cube.Dimensions.Find(
						sourceDimensions[i].CubeDimension.ID);
					if (cubeDimension != null)
					{
						mgDimension = sourceDimensions[i].Clone();
						measureGroup.Dimensions.Add(mgDimension);
					}
					//Не все исходные измерения входят в куб - генерировать исключение излишне.
					//else
					//    throw new CannotAddLinkedMeasureGroupDimensionException(
					//        String.Format("Не найдено измерение \"{0}\" в кубе \"{1}\"",
					//        sourceDimensions[i].CubeDimension.Name, cube.Name));
				}
				catch (Exception e)
				{
					ProcessException(parentException, e);
				}
			}
		}

		private static void CreateLinkedMeasureGroup(
			ref Cube cube, MeasureGroupInfo MGInfo, OLAPException parentException)
		{
			Cube sourceCube = null;
			MeasureGroup sourceMG = null;
			MeasureGroup linkedMG = null;
			CannotCreateLinkedMeasureGroupException currentException =
				new CannotCreateLinkedMeasureGroupException(String.Format(
					"Ошибки при добавлении связанной группы мер \"{0}\"",
					MGInfo.Name));
			try
			{
				sourceCube = DB.Cubes[MGInfo.Source.CubeID];
				sourceMG = sourceCube.MeasureGroups[MGInfo.Source.MeasureGroupID];
				//linkedMG = cube.LinkMeasureGroup(sourceMG, MGInfo.Source.DataSourceID, sourceMG.Name);
				linkedMG = cube.LinkMeasureGroup(sourceMG, MGInfo.Source.DataSourceID, MGInfo.ID);
				linkedMG.Name = MGInfo.Name;
				linkedMG.Source = new MeasureGroupBinding(
					MGInfo.Source.DataSourceID, sourceCube.ID, sourceMG.ID);

				FilterLinkedMeasureGroupMeasures(ref linkedMG, MGInfo);
				AddLinkedMeasureGroupDimensions(sourceMG.Dimensions, ref linkedMG, cube,
					currentException);

				if (currentException.Children.Count > 0)
					ProcessException(parentException, currentException);
			}
			catch (Exception e)
			{
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
			}
		}

		//public static Cube CreateVirtualCube(CubeInfoBase cubeInfo)
		//{
		//    Cube cube = null;
		//    string errors = String.Empty;
		//    LogLongOperation logLongOp = AddLongOperation(opRestoreCubes,
		//        CAStrings.msgAppendCube, cubeInfo.Name);
		//    CannotCreateVirtualCubeException currentException =
		//        new CannotCreateVirtualCubeException("Ошибки при создании виртуального куба");
		//    try
		//    {
		//        if (cubeInfo.LinkedMeasureGroups == null ||
		//            cubeInfo.LinkedMeasureGroups.Count == 0)
		//            throw new CubeMeasureGroupsListIsEmptyException();

		//        cube = DB.Cubes.Add(cubeInfo.Name, cubeInfo.StringID);
		//        cube.Source = new DataSourceViewBinding(DSView.Name);
		//        cube.StorageMode = StorageMode.Molap;

		//        CreateCubeDimensions(ref cube, cubeInfo, currentException);
		//        CreateCubeCalculations(ref cube, cubeInfo);
		//        CreateLinkedMeasureGroups(ref cube, cubeInfo.LinkedMeasureGroups,
		//            currentException);

		//        if (cube.MeasureGroups.Count == 0)
		//            throw new CubeHasNoMeasureGroupsException();

		//        cube.Update(UpdateOptions.ExpandFull);

		//        if (currentException.Children.Count > 0)
		//            EndOperationWithWarnings(logLongOp, currentException);
		//        else
		//            EndOperation(logLongOp);
		//    }
		//    catch (Exception e)
		//    {
		//        ProcessException(currentException, e);
		//        EndOperationWithErrors(logLongOp, currentException);
		//    }
		//    return cube;
		//}

		private static void CreateCubes()
		{
            opRestoreCubes = globalOp.AddLongOperation(Resources.msgRestoreCubes,
                TreeLogger.OperationStatus.Waiting, 0, databaseInfo.Cubes.Count);
            StartOperation(opRestoreCubes);
			try
			{
				for (int i = 0; i < databaseInfo.Cubes.Count; i++)
				{
					if (databaseInfo.Cubes[i].MeasureGroups.Count > 0)
						TryCreateCube(databaseInfo.Cubes[i], true, true);
				}
				for (int i = 0; i < databaseInfo.VirtualCubes.Count; i++)
                    if (databaseInfo.VirtualCubes[i].LinkedMeasureGroups.Count > 0)
                        TryCreateCube(databaseInfo.VirtualCubes[i], true, false);
			}
			finally { EndOperation(opRestoreCubes); }
		}

		static bool CheckTableExist(string tableName, OLAPException parentException)
		{
			try
			{
				int tableIndex = DSView.Schema.Tables.IndexOf(tableName);
				if (tableIndex < 0) { throw new TableNotExistException(tableName); }
				return tableIndex >= 0;
			}
			catch (Exception e)
			{
				ProcessException(parentException, e);
				return false;
			}
		}

		static DataItem CreateDataItem(ColumnInfo columnInfo, OLAPException parentException)
		{
			DataItem dataItem = null;
			try
			{
				CheckTableExist(columnInfo.TableName, null);
				DataTable dataTable = DSView.Schema.Tables[columnInfo.TableName];
				DataColumn dataColumn = dataTable.Columns[columnInfo.ColumnName];
				if (dataColumn != null)
				{
                    dataItem = new DataItem(columnInfo.TableName, GetColumnName(columnInfo),
						OleDbTypeConverter.GetRestrictedOleDbType(dataColumn.DataType));

					dataItem.DataSize = dataColumn.MaxLength;
					//if (dataColumn.MaxLength > 0)
					//{
					//    dataItem.DataSize = dataColumn.MaxLength;
					//}					
				}
				else
					throw new CannotCreateCreateDataItemException(
						String.Format("В таблице \"{0}\" не найдено поле \"{1}\"",
						columnInfo.TableName, columnInfo.ColumnName));
				return dataItem;
			}
			catch (Exception e)
			{
				CannotCreateCreateDataItemException currentException = new CannotCreateCreateDataItemException(
					String.Format("Не могу создать DataItem для [{0}].[{1}]",
					columnInfo.TableName, columnInfo.ColumnName));
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
				return dataItem;
			}
		}

	    private static string GetColumnName(ColumnInfo column)
	    {
            if (String.IsNullOrEmpty(column.Table.ExpressionName))
                return column.ColumnName.ToUpperInvariant();

	        return column.Table.ExpressionName;
	    }

	    static DataItem AddRollUpColumn(string tableName, RollUpInfo rollUpInfo,
			OLAPException parentException)
		{
			DataColumn rollUpColumn = null;
			try
			{
				CheckTableExist(tableName, null);
				DataTable dataTable = DSView.Schema.Tables[tableName];
				string safeComputedColumnName = rollUpInfo.SafeComputedColumnName;
				string safeExpression = rollUpInfo.SafeExpression;
				if (dataTable.Columns.IndexOf(safeComputedColumnName) < 0)
				{
					rollUpColumn = dataTable.Columns.Add(safeComputedColumnName,
						Type.GetType("System.String"), safeExpression);
					rollUpColumn.ExtendedProperties.Add("DbColumnName", safeComputedColumnName);
					rollUpColumn.ExtendedProperties.Add("ComputedColumnExpression", safeExpression);
					rollUpColumn.ExtendedProperties.Add("IsLogical", "True");
					DSView.Update();
				}
				if (rollUpColumn != null)
					return new DataItem(tableName, rollUpColumn.ColumnName,
						OleDbTypeConverter.GetRestrictedOleDbType(rollUpColumn.DataType));
				else
					return null;
			}
			catch (Exception e)
			{
				CannotCreateRollUpColumnException currentException =
					new CannotCreateRollUpColumnException(
					String.Format("Не могу создать CustomRollUp \"{0}\"", rollUpInfo.Name));
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
				return null;
			}
		}

		static void AddRelation(RelationInfo relInfo, OLAPException parentException)
		{
			LogOperation logOp = AddOperation(opCreateRelations, Resources.msgAppendRelation,
				relInfo.Name);
			try
			{
				string pkTableName = relInfo.PrimaryKey.TableName;
				string fkTableName = relInfo.ForeignKey.TableName;
				CheckTableExist(pkTableName, null);
				CheckTableExist(fkTableName, null);
				string relationName = relInfo.Name;
				if (!DSView.Schema.Relations.Contains(relationName))
				{
					DataColumn fkColumn =
						DSView.Schema.Tables[fkTableName].Columns[relInfo.ForeignKey.ColumnName];
					DataColumn pkColumn =
						DSView.Schema.Tables[pkTableName].Columns[relInfo.PrimaryKey.ColumnName];
					if (pkColumn != null && fkColumn != null)
						DSView.Schema.Relations.Add(relationName, pkColumn, fkColumn, false);
					//DSView.Schema.Relations.Add(relationName, pkColumn, fkColumn, true);
				}
				//Если не обновлять DSWiew после каждого добавленного отношения, то получаем выигрыш в скорости в разы,
				//но при этом теряем возможность увидеть какие отношения не удалось установить.
				//--- 
				//DSView.Update();
				//---

				EndOperation(logOp, OperationStatus.FinishedOK);
			}
			catch (Exception e)
			{
				EndOperationWithErrors(logOp, e);
				CannotCreateRelationException currentException = new CannotCreateRelationException(
					String.Format("Не могу создать отношение [{0}].[{1}] к [{2}].[{3}]",
					relInfo.ForeignKey.TableName, relInfo.ForeignKey.ColumnName,
					relInfo.PrimaryKey.TableName, relInfo.PrimaryKey.ColumnName));
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
			}
		}

		static void SetAttributeNameColumn(Dimension dimension, DimensionAttribute attr,
			ColumnInfo columnInfo, OLAPException parentException)
		{
			try
			{
				attr.NameColumn = CreateDataItem(columnInfo, null);
				if (attr.NameColumn != null)
					attr.NameColumn.DataType = OleDbType.WChar;
			}
			catch (Exception e)
			{
				CannotSetAttributeNameColumnException currentException =
					new CannotSetAttributeNameColumnException(String.Format(
					"Не могу установить поле имени [{0}][{1}] для аттрибута \"{2}\"",
					columnInfo.TableName, columnInfo.ColumnName, attr.Name));
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
			}
		}

		static DimensionAttribute AddAttribute(Dimension dim, AttributeInfo attrInfo,
			OLAPException parentException)
		{
			DimensionAttribute attr = dim.Attributes.Find(attrInfo.SafeName);
			if (attr == null)
			{
				try
				{
					//TODO: Поддержка аттрибутов у которых в качестве ключевого поля идет
					//несколько столбцов (см. измерение "КД.Сопоставимый")
					attr = new DimensionAttribute(attrInfo.SafeName);
					for (int i = 0; i < attrInfo.KeyColumns.Count; i++)
					{
						DataItem dataItem = CreateDataItem(attrInfo.KeyColumns[i], null);
						attr.KeyColumns.Add(dataItem);
					}
					SetAttributeNameColumn(dim, attr, attrInfo.NameColumn, null);
					attr.AttributeHierarchyDisplayFolder = attrInfo.AttributeHierarchyDisplayFolder;
					attr.AttributeHierarchyEnabled = attrInfo.AttributeHierarchyEnabled;
					attr.AttributeHierarchyOrdered = attrInfo.AttributeHierarchyOrdered;
					attr.AttributeHierarchyVisible = attrInfo.AttributeHierarchyVisible;
					attr.Description = attrInfo.Description;
					attr.DefaultMember = attrInfo.DefaultMember;
					attr.IsAggregatable = attrInfo.IsAggregatable;
					attr.MemberNamesUnique = attrInfo.MemberNamesUnique;
					attr.MembersWithData = attrInfo.MembersWithData;
					attr.MembersWithDataCaption = attrInfo.MembersWithDataCaption;
					attr.NamingTemplate = attrInfo.NamingTemplate;
					attr.OrderBy = attrInfo.OrderBy;
					attr.OrderByAttributeID = attrInfo.OrderByAttributeID;
					attr.RootMemberIf = attrInfo.RootMemberIf;
					attr.Type = attrInfo.Type;
					attr.Usage = attrInfo.Usage;

					dim.Attributes.Add(attr);
				}
				catch (Exception e)
				{
					CannotCreateDimensionAttributeException currentException =
						new CannotCreateDimensionAttributeException(
						String.Format("Не могу добавить в измерение \"{0}\" аттрибут \"{1}\"",
						dim.Name, attrInfo.SafeName));
					ProcessException(currentException, e);
					ProcessException(parentException, currentException);
				}
			}
			return attr;
		}

		static void CreateParentChildHierarchy(Dimension dim, DimensionAttribute idAttr,
			DimensionAttribute attr, AttributeInfo attrInfo, OLAPException parentException)
		{
			try
			{
				attr.AttributeHierarchyVisible = true;

				//------------------------------------------------------------
				//видна только нужная иерархия, но исчезают MemberProperties
				idAttr.AttributeHierarchyVisible = false;

				//видна ненужная иерархия, зато есть MemberProperties
				//idAttr.AttributeHierarchyVisible = true;
			}
			catch (Exception e)
			{
				ProcessException(parentException, e);
			}
		}

		//Создаем аттрибуты измерения (кроме ID), а заодно и иерархии Parent-Child
		private static void CreateDimensionAttributes(ref Dimension dimension,
			DimensionAttribute idAttr, List<AttributeInfo> attributes,
			OLAPException parentException)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				try
				{
					DimensionAttribute attr = AddAttribute(dimension, attributes[i], null);
					if (attr.Usage == AttributeUsage.Parent)
						CreateParentChildHierarchy(dimension, idAttr, attr, attributes[i], null);
					if (attributes[i].CustomRollUpInfo != null)
						attr.CustomRollupColumn = AddRollUpColumn(
							attributes[i].KeyColumns[0].TableName,
							attributes[i].CustomRollUpInfo, null);

				}
				catch (Exception e)
				{
					ProcessException(parentException, e);
				}
			}
		}

		private static void CreateAttributeRelations(ref Dimension dimension,
			List<AttributeInfo> attributes, OLAPException parentException)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				if (attributes[i].AttrRelationShips.Count > 0)
				{
					try
					{
						DimensionAttribute parentAttr = dimension.Attributes.FindByName(
							attributes[i].SafeName);
						if (parentAttr != null)
						{
							for (int j = 0; j < attributes[i].AttrRelationShips.Count; j++)
							{
								DimensionAttribute childAttr = dimension.Attributes.Find(
									attributes[i].AttrRelationShips[j].AttributeID);
								if (childAttr != null)
								{
								    AttributeRelationship attributeRelationship = new AttributeRelationship(childAttr.ID);
								    attributeRelationship.Name = !String.IsNullOrEmpty(attributes[i].AttrRelationShips[j].Name)
								                                     ? attributes[i].AttrRelationShips[j].Name
								                                     : childAttr.ID;
                                    parentAttr.AttributeRelationships.Add(attributeRelationship);
								}
							}
						}
					}
					catch (Exception e)
					{
						ProcessException(parentException, e);
					}
				}
			}
		}

		//Создаем иерархии для измерения (все, кроме Parent-Child)
		private static void CreateDimensionHierarchy(ref Dimension dimension,
			HierarchyInfo hierarchyInfo, OLAPException parentException)
		{
			try
			{
				Hierarchy hier = new Hierarchy(hierarchyInfo.Name, hierarchyInfo.ID);				
				hier.AllMemberName = hierarchyInfo.AllMemberName;
				hier.AllowDuplicateNames = hierarchyInfo.AllowDuplicateNames;
				hier.MemberNamesUnique = hierarchyInfo.MemberNamesUnique;
				Level level = null;
				for (int j = 0; j < hierarchyInfo.Levels.Count; j++)
				{
					if (!hierarchyInfo.Levels[j].IsAll())
					{
						DimensionAttribute sourceAttribute = dimension.Attributes.
							Find(hierarchyInfo.Levels[j].SourceAttributeID);
						if (sourceAttribute == null)
							throw new CannotCreateHierarchyLevelException(String.Format(
								"Не могу создать уровень \"{0}\" в иеррахии \"{1}\" - аттрибут \"{2}\" не найден",
								hierarchyInfo.Levels[j].SourceAttributeID,
								hier.Name, hierarchyInfo.Levels[j].SourceAttributeID));

						level = hier.Levels.Add(hierarchyInfo.Levels[j].Name);
						level.SourceAttribute = sourceAttribute;
					}
				}
				dimension.Hierarchies.Add(hier);
			}
			catch (Exception e)
			{
				CannotCreateHierarchyException currentException =
					new CannotCreateHierarchyException(
					String.Format("Не могу добавить иерархию \"{0}\"", hierarchyInfo.Name));
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
			}
		}

		private static void CreateDimensionHierarchies(ref Dimension dimension,
			List<HierarchyInfo> hierarchies, OLAPException parentException)
		{
			for (int i = 0; i < hierarchies.Count; i++)
			{
				try
				{
					CreateDimensionHierarchy(ref dimension, hierarchies[i], null);
				}
				catch (Exception e)
				{
					ProcessException(parentException, e);
				}
			}
		}

		private static void EndOperationWithErrors(LogOperation logOp, Exception e)
		{
			if (logOp != null)
			{
				AppendLogMessageError(logOp, e.Message);
				logOp.EndOperation(OperationStatus.FinishedWithErrors);
			}
		}


		private static void EndOperationWithErrors(LogOperation logOp, OLAPException e)
		{
			if (logOp != null)
			{
				for (int i = 0; i < e.Children.Count; i++)
					AppendExceptionMessages(logOp, e.Children[i], OperationStatus.FinishedWithErrors);
				logOp.EndOperation(OperationStatus.FinishedWithErrors);
			}
		}

		private static void EndOperationWithWarnings(LogOperation logOp, OLAPException e)
		{
			if (logOp != null)
			{
				for (int i = 0; i < e.Children.Count; i++)
					AppendExceptionMessages(logOp, e.Children[i],
						OperationStatus.FinishedWithWarnings);
				logOp.EndOperation(OperationStatus.FinishedWithWarnings);
			}
		}

		private static bool DimensionHasNoHierarchies(Dimension dim)
		{
			if (dim.Hierarchies.Count > 0)
				return false;
			else
			{
				for (int i = 0; i < dim.Attributes.Count; i++)
					if (dim.Attributes[i].Usage == AttributeUsage.Parent)
						return false;
				return true;
			}
		}

		private static void ProcessImpactObjects(Cube cube)
		{
			XmlaWarningCollection warnings = new XmlaWarningCollection();
			ImpactDetailCollection impactResult = new ImpactDetailCollection();
			cube.Drop(DropOptions.IgnoreFailures, warnings, impactResult, true);
			try
			{
				for (int i = 0; i < impactResult.Count; i++)
				{
					if (impactResult[i].Object is Cube)
					{
						Cube virtualCube = impactResult[i].Object as Cube;
						databaseInfo.AddCubeInfo(new CubeInfo2005(virtualCube));
						virtualCube.Drop(DropOptions.IgnoreFailures);
						DB.Cubes.Remove(virtualCube, true);
					}
				}
				databaseInfo.AddCubeInfo(new CubeInfo2005(cube));
				cube.Drop(DropOptions.IgnoreFailures);
				DB.Cubes.Remove(cube, true);
			}
			catch { }
		}

		private static void ProcessImpactObjects(Dimension dim)
		{
			XmlaWarningCollection warnings = new XmlaWarningCollection();
			ImpactDetailCollection impactResult = new ImpactDetailCollection();
			dim.Drop(DropOptions.IgnoreFailures, warnings, impactResult, true);
			try
			{
				for (int i = 0; i < impactResult.Count; i++)
				{
					//if (impactResult[i].Object is Cube)
					//    ProcessImpactObjects(impactResult[i].Object as Cube);
					if (impactResult[i].Object is MeasureGroup)
						ProcessImpactObjects((impactResult[i].Object as MeasureGroup).Parent);
				}
				DB.Dimensions.Remove(dim, true);
				dim.Drop(DropOptions.IgnoreFailures);
			}
			catch { }
		}

		public static void TryCreateDimension(DimensionInfoBase dimInfo, bool immediatelyUpdate)
		{
			if (dimInfo.BlockRestoreReason != null)
			{
				AddOperation(opRestoreDimensions,
					string.Format("Не могу добавить измерение \"{0}\" - {1}",
					dimInfo.Name, dimInfo.BlockRestoreReason.Message),
					OperationStatus.FinishedWithErrors);
			}
			else
			{
				CreateDimension(dimInfo, immediatelyUpdate);
			}
		}

		public static void CreateDimension(DimensionInfoBase dimInfo, bool immediatelyUpdate)
		{
			LogLongOperation logLongOp = AddLongOperation(opRestoreDimensions,
				Resources.msgAppendDimension, dimInfo.Name);
			CannotCreateDimensionException currentException = new CannotCreateDimensionException(
				"Ошибки при создании измерения");
			try
			{
				string guid = dimInfo.ID;
				Dimension dimension = DB.Dimensions.Find(guid);

				//if (dimension != null)
				//    throw new CannotCreateDimensionException(
				//        String.Format(Resources.msgDimensionAlreadyExist, dimension.Name));

				//dimension = DB.Dimensions.Add(dimInfo.SafeName, guid);

				//___________________
				if (dimension != null)
				{
					ProcessImpactObjects(dimension);
					dimension = null;
				}

				//___________________
				if (dimension == null) { dimension = new Dimension(dimInfo.Name, guid); }
				//DB.Dimensions.Add(dimInfo.SafeName, guid); }

				dimension.Description = dimInfo.Description;
				dimension.Type = dimInfo.DimensionType;
				//dim.UnknownMember = UnknownMemberBehavior.Hidden;
				dimension.AttributeAllMemberName = dimInfo.AllMemberName;
				dimension.Source = new DataSourceViewBinding(DB.DataSources[0].Name);
				dimension.StorageMode = dimInfo.StorageMode;

				DimensionAttribute idAttr = AddAttribute(dimension, dimInfo.KeyAttribute, null);
				CreateDimensionAttributes(ref dimension, idAttr, dimInfo.Attributes,
					currentException);
				CreateAttributeRelations(ref dimension, dimInfo.Attributes, currentException);
				CreateDimensionHierarchies(ref dimension, dimInfo.Hierarchies, currentException);
				if (DimensionHasNoHierarchies(dimension))
					throw new DimensionHasNoHierarchiesException();

				CreateAnnotations(dimension.Annotations, dimInfo.Annotations);
				//CreateVersionAnnotation(dimension.Annotations, dimInfo.VersionInfo);
				CreateControlBlockAnnotation(dimension.Annotations, dimInfo.ControlBlock);

				if (!DB.Dimensions.Contains(dimension.ID)) { DB.Dimensions.Add(dimension); }

                ValidationErrorCollection errorCollection = new ValidationErrorCollection();

                if (!dimension.Validate(errorCollection, true))
                {
                    foreach (ValidationError error in errorCollection)
                    {
                        AddOperation(logLongOp,
                                     String.Format("При валидации измерения обнаружена ошибка: {0}, код ошибки: {1}",
                                                   error.FullErrorText, error.ErrorCode),
                                     OperationStatus.FinishedWithWarnings);
                    }
                }

				if (immediatelyUpdate)
					dimension.Update(UpdateOptions.ExpandFull, dimInfo.UpdateMode);
				dimInfo.Dimension = dimension;

				if (currentException.Children.Count > 0)
					EndOperationWithWarnings(logLongOp, currentException);
				else
					EndOperation(logLongOp);
			}
			catch (Exception e)
			{
				ProcessException(currentException, e);
				EndOperationWithErrors(logLongOp, currentException);
			}
		}

		public static void CreateDimensions(bool immediatelyUpdate)
		{
            opRestoreDimensions = globalOp.AddLongOperation(Resources.msgRestoreDimensions,
                TreeLogger.OperationStatus.Waiting, 0, databaseInfo.Dimensions.Count);
            StartOperation(opRestoreDimensions);
			try
			{
				for (int i = 0; i < databaseInfo.Dimensions.Count; i++)
				{
					DimensionInfoBase dimensionInfo = databaseInfo.Dimensions[i];
					TryCreateDimension(dimensionInfo, immediatelyUpdate);
				}
				if (!immediatelyUpdate) { DB.Update(UpdateOptions.ExpandFull); }
			}
			catch (Exception e)
			{
				EndOperationWithErrors(opRestoreDimensions, e);
			}
			finally { EndOperation(opRestoreDimensions); }
		}

		/// <summary>
		/// Удаляет из списка все отношения, в которых используется данная таблица.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="tableInfo"></param>
        private static void RemoveRelations(List<RelationInfo> items, TableInfo tableInfo)
		{
            if (tableInfo.Name.StartsWith("OLAP_"))
                return;

			int i = 0;
			while (i < items.Count)
			{
				if (items[i].ContainsTable(tableInfo))
				{
					items.Remove(items[i]);
				}
				else { i += 1; }
			}
		}		

        /// <summary>
        /// Помечает элементы из заданного списка как непригодные к востановлению.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="blockReason"></param>
        private static void BlockOlapObjects(IEnumerable<ObjectInfo> items, Exception blockReason)
        {
            foreach (ObjectInfo item in items)
            {
                if (null == item.BlockRestoreReason)
                {
                    item.BlockRestoreReason = blockReason;
                }
            }            
        }        

		public static void AddTableToDataSourceView(
			OleDbDataAdapter adapter, TableInfo tableInfo, DataTable tables, DataTable views)
		{
		    bool isTableExist = false;

			LogOperation logOp = AddOperation(opPumpTables, Resources.msgAppendTable, tableInfo.Name);
			try
			{
			    adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                ConverSourceFilterExpression(tableInfo);

			    adapter.SelectCommand.CommandText = tableInfo.QueryDefinition;

			    DataTable dataTable = new DataTable();
                if (!DSView.Schema.Tables.Contains(tableInfo.Name))
			    {
			        DataTable[] dataTables = adapter.FillSchema(DSView.Schema,
                                                                SchemaType.Mapped, tableInfo.Name);
			        dataTable = dataTables[0];
			    }
			    else
			    {
                    dataTable = DSView.Schema.Tables[tableInfo.Name];
			        isTableExist = true;
			    }

                if (!String.IsNullOrEmpty(tableInfo.Expression))
                    AddComputedColumn(tableInfo, dataTable);

			    if (!isTableExist)
			    {
			        for (int i = 0; i < dataTable.Columns.Count; i++)
			        {
			            dataTable.Columns[i].ColumnName = dataTable.Columns[i].ColumnName.ToUpperInvariant();
			        }

			        //DataColumn IDColumn = dataTable.Columns["ID"];
			        //if (IDColumn != null)
			        //    dataTable.PrimaryKey = new DataColumn[1] { IDColumn };

			        TableType tableType = tableInfo.TableType;
			        //TableType tableType = tableInfo.GetTableType(tables, views);
			        switch (tableType)
			        {
			            case TableType.table:					
			                dataTable.ExtendedProperties.Add("TableType", "Table");
			                dataTable.ExtendedProperties.Add("DbSchemaName", "DV");
			                dataTable.ExtendedProperties.Add("DbTableName", tableInfo.Name);
			                dataTable.ExtendedProperties.Add("FriendlyName", tableInfo.Name);
			                break;
			            case TableType.view:
			                dataTable.ExtendedProperties.Add("TableType", "View");
			                dataTable.ExtendedProperties.Add("ViewName", tableInfo.Name);
			                dataTable.ExtendedProperties.Add("QueryDefinition", tableInfo.QueryDefinition);
			                break;
			            default:
			                break;
			        }
			        dataTable.ExtendedProperties.Add("BaseTableName", tableInfo.Name);
			    }

				EndOperation(logOp, OperationStatus.FinishedOK);
			}
			catch (Exception e)
			{
				EndOperationWithErrors(logOp, e);
				RemoveRelations(databaseInfo.DataSourceView.TableRelations, tableInfo);
				
                //Блокируем восстановление измерений.
                BlockOlapObjects(OLAPUtils.GetDimensionsByTableName(databaseInfo.Dimensions,
					tableInfo.Name), new TableNotExistException(tableInfo.Name));
                
                //Блокируем восстановление кубов.
                BlockOlapObjects(OLAPUtils.GetCubesByTableName(databaseInfo.Cubes,
					tableInfo.Name), new TableNotExistException(tableInfo.Name));
			}
		}

	    private static void ConverSourceFilterExpression(TableInfo tableInfo)
	    {
            // Преобразование подвыражения mod на % для SQL Server
	        Regex regex = new Regex(@"\w{3}\({1}.+\,[^)]+\){1}");
	        if (regex.IsMatch(tableInfo.QueryDefinition))
	        {
	            Match match = regex.Match(tableInfo.QueryDefinition);
	            tableInfo.QueryDefinition = tableInfo.QueryDefinition.Replace(match.ToString(), ConvertExpression(match.ToString()).ToString());
	        }
	    }

	    /// <summary>
        /// Добавляет вычислимое поле в представление источника
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="dataTable"></param>
	    private static void AddComputedColumn(TableInfo tableInfo, DataTable dataTable)
	    {
	        using (DataColumn clmn = new DataColumn(tableInfo.ExpressionName, typeof (int)))
	        {
	            dataTable.Columns.Add();
	            clmn.ExtendedProperties.Add("ComputedColumnExpression",
	                                        ConvertExpression(tableInfo.Expression));
	            dataTable.Columns.Add(clmn);
	        }
	    }

	    /// <summary>
        /// Преобразование выражения под конкретную СУБД
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
	    private static object ConvertExpression(string expression)
	    {
            if (connection.Provider.Contains(Resources.SQLServerUtils_InitTablesAndViews_SQLOLEDB))
            {
                if (expression.ToLower().StartsWith("mod"))
                {
                    string[] parts = expression.Split(new char[] {'(', ',', ')'});
                    return String.Format("{0} % {1}", parts[1], parts[2]);
                }
            }

            return expression;
	    }

	    public static int GetTablesCount()
		{
			OleDbCommand SQLCommand = new OleDbCommand(Resources.sqlGetTablesCount, connection);
			SQLCommand.Prepare();
			int objectsCount = Convert.ToInt32(SQLCommand.ExecuteScalar());
			SQLCommand.CommandText = Resources.sqlGetViewsCount;
			objectsCount = objectsCount + Convert.ToInt32(SQLCommand.ExecuteScalar());
			return objectsCount;
		}

		//public static void AddAllTablesToDataSourceView(string sqlGetNames,
		//    TableType tableType)
		//{
		//    OleDbCommand SQLCommand = new OleDbCommand(sqlGetNames, connection);
		//    SQLCommand.Prepare();
		//    OleDbDataReader dataReader = SQLCommand.ExecuteReader();
		//    OleDbDataAdapter adapter = new OleDbDataAdapter("", connection);
		//    while (dataReader.Read())
		//    {
		//        string tableName = dataReader[0].ToString();
		//        AddTableToDataSourceView(adapter, tableName, tableType, CAStrings.sqlGetOneTable + tableName);
		//    }
		//    dataReader.Close();			
		//}

		//public static void AddAllTablesToDataSourceView(
		//    string collectionName, string nameColumn, TableType tableType)
		//{	
		//    DataTable names = connection.GetSchema(collectionName,
		//        new string[] {null, "DV"});
		//    OleDbDataAdapter adapter = new OleDbDataAdapter("", connection);
		//    int nameIndex = names.Columns.IndexOf(nameColumn);
		//    for (int i = 0; i < names.Rows.Count; i++)
		//    {
		//        string tableName = names.Rows[i].ItemArray.GetValue(nameIndex).ToString();
		//        AddTableToDataSourceView(adapter, tableName, tableType,
		//            CAStrings.sqlGetOneTable + tableName);
		//    }			
		//}

        //private static TableType GetTableType(string tableName, DataTable tables, DataTable views)
        //{
        //    if (views.Select("table_name like '" + tableName + "'").Length > 0)
        //        return TableType.View;
        //    else
        //        return TableType.table;
        //}

		private static bool IsOlapTable(string tableName)
		{
			if (tableName.StartsWith("OLAP", StringComparison.OrdinalIgnoreCase))
				return true;
			else
				return false;
		}

		public static void RemoveRelations(
			DataRelationCollection schemaRelations, DataRelationCollection tableRelations)
		{
			for (int i = 0; i < tableRelations.Count; i++)
			{
				schemaRelations.Remove(tableRelations[i]);
			}
			tableRelations.Clear();
		}

		private static void InitTablesAndViews(out DataTable tables, out DataTable views)
		{
			string[] restrictions = new string[] { null, "DV" };
			tables = connection.GetSchema(OleDbMetaDataCollectionNames.Tables,
				restrictions);

            if (connection.Provider.Contains(Resources.SQLServerUtils_InitTablesAndViews_SQLOLEDB))
                views = new DataTable();
            else
			    views = connection.GetSchema(OleDbMetaDataCollectionNames.Views,
				    restrictions);		
		}

		private static void CheckTable(string tableName)
		{
			int i = DSView.Schema.Tables.IndexOf(tableName);
			if (i >= 0)
			{
				DataTable table = DSView.Schema.Tables[i];
				RemoveRelations(DSView.Schema.Relations, table.ChildRelations);
				RemoveRelations(DSView.Schema.Relations, table.ParentRelations);

				int j = 1;
				while (j < table.Constraints.Count)
				{
					table.Constraints.RemoveAt(j);
				}

				DSView.Schema.Tables.Remove(table);
			}
		}

		public static void CreateDataSourceView(DataSourceViewInfo dataSourceViewInfo,
			OLAPException parentException)
		{
			try
			{
				DataTable tables;
				DataTable views;
				if (databaseInfo.UpdateMode == UpdateMode.CreateOrReplace ||
					databaseInfo.UpdateMode == UpdateMode.Create)
				{
					DSView = DB.DataSourceViews.Add(dataSourceViewInfo.Name);
					DSView.DataSourceID = dataSourceViewInfo.DataSourceName;
					DSView.Schema = new DataSet();
					DSView.Schema.Locale = CultureInfo.CurrentCulture;
					DSView.Schema.EnforceConstraints = false;
					//DSView.Schema.CaseSensitive = true;

                    opPumpTables = globalOp.AddLongOperation(Resources.msgPumpingTables,
                        TreeLogger.OperationStatus.Waiting, 10, databaseInfo.UsedTables.Count);
                    StartOperation(opPumpTables);
					try
					{
						InitTablesAndViews(out tables, out views);
						OleDbDataAdapter adapter = new OleDbDataAdapter(string.Empty, connection);

                        foreach (TableInfo tableInfo in dataSourceViewInfo.UsedTables.Tables)
                        {
                            AddTableToDataSourceView(adapter, tableInfo, tables, views);
                        }                        
					}
					finally
                    { 
                        EndOperation(opPumpTables);
                    }
					if (databaseInfo.UpdateMode == UpdateMode.CreateOrReplace ||
						databaseInfo.UpdateMode == UpdateMode.Create)
						DSView.Update(UpdateOptions.Default, UpdateMode.CreateOrReplace);
					else
						DSView.Update(UpdateOptions.Default, UpdateMode.UpdateOrCreate);

					CreateRelations(dataSourceViewInfo.TableRelations, parentException);
				}
				else DSView = DB.DataSourceViews[dataSourceViewInfo.Name];
			}
			catch (Exception e)
			{
				CannotCreateDataSourceViewException currentException =
					new CannotCreateDataSourceViewException(dataSourceViewInfo.Name);
				ProcessException(currentException, e);
				ProcessException(parentException, currentException);
			}
		}


		public static void CreateDataSource(DataSourceInfo dataSourceInfo)
		{
			DataSource dataSource = null;
			if (DB.DataSources != null)
				dataSource = DB.DataSources.Find(Resources.defaultDataSourceName);
			if (dataSource == null)
			{
				dataSource = DB.DataSources.Add(dataSourceInfo.Name);
				dataSource.ConnectionStringSecurity = ConnectionStringSecurity.Unchanged;
				dataSource.ConnectionString = dataSourceInfo.ConnectionString;
				dataSource.Update();
				//dataSource.Update(UpdateOptions.Default, dataSourceInfo.UpdateMode);			
			}
		}

		private static void DropDatabase(Database DB)
		{
			if (DB != null)
			{
				LogLongOperation opDropDatabase = AddLogOperation(
					Resources.msgDropDatabase + " - \"" + DB.Name + "\"",
					OperationStatus.Running, 0, 1);
				StartOperation(opDropDatabase);
				try
				{
					DB.Drop();
					EndOperation(opDropDatabase);
				}
				catch (Exception e)
				{
					EndOperationWithErrors(opDropDatabase, e);
					throw;
				}
			}
		}

		public static void CreateOlapDatabase(OLAPException parentException)
		{
			try
			{
				DB = null;
				if (databaseInfo.server.Databases != null)
					DB = databaseInfo.server.Databases.FindByName(databaseInfo.Name);
				if (DB != null && (databaseInfo.UpdateMode == UpdateMode.CreateOrReplace ||
					databaseInfo.UpdateMode == UpdateMode.Create))
				{
					DropDatabase(DB);
					DB = null;
				}
				if (DB == null)
				{
					DB = databaseInfo.server.Databases.Add(databaseInfo.Name);
					DB.DataSourceImpersonationInfo =
						new ImpersonationInfo(ImpersonationMode.ImpersonateServiceAccount);
					DB.Language = (int)Language.Russian;
					DB.Description = databaseInfo.Description;
					CreateAnnotations(DB.Annotations, databaseInfo.Annotations);
					DB.Update(UpdateOptions.Default, databaseInfo.UpdateMode);
				}
				databaseInfo.DB = DB;
			}
			catch (Exception e)
			{
				ProcessException(parentException, e);
				throw;
			}
		}

		private static void InitLog()
		{
			log = databaseInfo.log;
			if (log != null)
			{
				log.ResetLog("Восстановление многомерной базы");
				globalOp = log.GlobalOperation;
				globalOp.StartOperation();
                
                //AddLogOperations(databaseInfo.UsedTables.Count, databaseInfo.Dimensions.Count,
                //    databaseInfo.Cubes.Count,
                //    databaseInfo.DataSourceView.TableRelations.Count);                
			}
		}

		private static void AddLogOperations(int tablesCount, int dimensionsCount, int cubesCount,
			int relationsCount)
		{
			if (globalOp != null)
			{
				opPumpTables = globalOp.AddLongOperation(Resources.msgPumpingTables,
					TreeLogger.OperationStatus.Waiting, 10, tablesCount);
				if (databaseInfo.DataSourceView.TableRelations.Count > 0)
					opCreateRelations = globalOp.AddLongOperation(Resources.msgCreateRelations,
					TreeLogger.OperationStatus.Waiting, 10, relationsCount);
				opRestoreDimensions = globalOp.AddLongOperation(Resources.msgRestoreDimensions,
					TreeLogger.OperationStatus.Waiting, 0, dimensionsCount);
				opRestoreCubes = globalOp.AddLongOperation(Resources.msgRestoreCubes,
					TreeLogger.OperationStatus.Waiting, 0, cubesCount);
			}
		}

		private static LogLongOperation AddLogOperation(string opName, OperationStatus opStatus,
			int showBuffer, int progressMaximum)
		{
			if (globalOp != null)
				return globalOp.AddLongOperation(opName, opStatus, showBuffer, progressMaximum);
			else
				return null;
		}

		private static void StartOperation(LogLongOperation logLongOp)
		{
			if (logLongOp != null) { logLongOp.StartOperation(); }
		}

		private static LogOperation AddOperation(
			LogLongOperation parentOp, string opName, string objectName)
		{
			if (parentOp != null)
			{
				parentOp.ProgressUpdate();
				return parentOp.AddOperation(
					String.Format("{0} '{1}'", opName, objectName), OperationStatus.Running);
			}
			else
				return null;
		}

		private static LogLongOperation AddLongOperation(
			LogLongOperation parentOp, string opName, string objectName)
		{
			if (parentOp != null)
			{
				parentOp.ProgressUpdate();
				return parentOp.AddLongOperation(
					String.Format("{0} '{1}'", opName, objectName), OperationStatus.Running);
			}
			else
				return null;
		}

		private static LogOperation AddOperation(
			LogLongOperation parentOp, string opName, OperationStatus status)
		{
			if (parentOp != null)
			{
				parentOp.ProgressUpdate();
				return parentOp.AddOperation(opName, status);
			}
			else
				return null;
		}

		private static void AppendExceptionMessages(LogOperation parentOp, Exception e,
			OperationStatus opStatus)
		{
			if (parentOp != null)
			{
				LogOperation logOp = parentOp.AddOperation(e.Message, opStatus);
				if (e is OLAPException)
				{
					OLAPException OLAPException = (e as OLAPException);
					for (int i = 0; i < OLAPException.Children.Count; i++)
						AppendExceptionMessages(logOp, OLAPException.Children[i], opStatus);
				}
			}
		}

		private static void AppendLogMessage(LogOperation parentOp, string logMessage)
		{
			if (parentOp != null) { parentOp.AppendLogMessage(logMessage); }
		}

		private static void AppendLogMessageError(LogOperation parentOp, string logMessage)
		{
			if (parentOp != null)
				parentOp.AddOperation(logMessage, OperationStatus.FinishedWithErrors);
		}

		private static void EndOperation(LogLongOperation logLongOp)
		{
			if (logLongOp != null) { logLongOp.EndOperation(); }
		}

		private static void EndOperation(LogOperation logOp, OperationStatus opStatus)
		{
			if (logOp != null) { logOp.EndOperation(opStatus); }
		}

		private static void ConnectToDataWarehouse()
		{
            opConnectToDataWarehouse = AddLogOperation(String.Format(
                Resources.msgConnectToDataWarehouse, databaseInfo.DataSource.Name,
                databaseInfo.DataSource.ConnectionString), OperationStatus.Running, 0, 1);
            StartOperation(opConnectToDataWarehouse);
			try
			{
				connection = new OleDbConnection(databaseInfo.DataSource.ConnectionString);
				connection.Open();
				EndOperation(opConnectToDataWarehouse);
			}
			catch (Exception e)
			{
				EndOperationWithErrors(opConnectToDataWarehouse, e);
				throw;
			}
		}

        /// <summary>
        /// Разбирает строку с именем файла, возвращая имена кубов.
        /// </summary>
        /// <param name="fileName">Имя файла. Должно содержать имена кубов, разделенные запятыми.</param>
        /// <returns>Массив с именами кубов.</returns>
        private static string[] GetCubesNames(string fullFileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullFileName);            
            //Убираем расширение.            
            string fileName = fileInfo.Name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
            //Получаем имена файлов.
            return fileName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }


        /// <summary>
        /// Считывает файлы с вычислениями в словарь, где ключ - имя куба.
        /// Если словарь не пустой - добавляет операцию со статусом ожидание в лог.
        /// </summary>
        private static void InitUpdateCalculationsFiles()
        {
            string updateCalculationsDirectory = System.Environment.CurrentDirectory +
                "\\" + Resources.defaultUpdateCalculationsDirectoryName;
            if (System.IO.Directory.Exists(updateCalculationsDirectory))
            {                
                string[] calculationsFiles = System.IO.Directory.GetFiles(
                    updateCalculationsDirectory, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
                foreach (string calculationsFile in calculationsFiles)
                {
                    string[] cubesNames = GetCubesNames(calculationsFile);
                    string calculationText = System.IO.File.ReadAllText(calculationsFile, Encoding.Default);
                    foreach (string cubeName in cubesNames)
                    {
                        string invariantCubeName = cubeName.Trim();
                        if (!calculationsToUpdate.ContainsKey(invariantCubeName))
                        {
                            calculationsToUpdate.Add(invariantCubeName, calculationText);
                        }
                    }                    
                }
            }
        }

        private static void UpdateCalculations()
        {
            InitUpdateCalculationsFiles();
            if (calculationsToUpdate.Count > 0)
            {
                opUpdateCalculations = AddLogOperation(
                    Resources.msgUpdateCalculations, OperationStatus.Waiting, 0, calculationsToUpdate.Count);

                StartOperation(opUpdateCalculations);
                try
                {
                    LogOperation logOp = null;
                    foreach (KeyValuePair<string, string> calculation in calculationsToUpdate)
                    {
                        try
                        {
                            Cube cube = DB.Cubes.FindByName(calculation.Key);
                            if (null != cube)
                            {
                                logOp = AddOperation(
                                    opUpdateCalculations, Resources.msgUpdateCalculation, calculation.Key);
                                cube.DefaultMdxScript.Commands.Clear();
                                cube.DefaultMdxScript.Commands.Add(new Command(calculation.Value));
                                cube.Update(UpdateOptions.ExpandFull);                                
                                EndOperation(logOp, OperationStatus.FinishedOK);
                            }
                            else
                            {
                                logOp = AddOperation(opUpdateCalculations,
                                    string.Format(Resources.msgCannotUpdateCalculations, calculation.Key) + " - куб не найден.",
                                    OperationStatus.FinishedWithErrors);
                            }
                        }
                        catch (Exception e)
                        {
                            EndOperationWithErrors(logOp, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    EndOperationWithErrors(opUpdateCalculations, e);
                }
                finally { EndOperation(opUpdateCalculations); }
            }
        }

		public static Database CreateFullDatabase(DatabaseInfoBase _databaseInfo)
		{
			//TODO обрабатывать ошибки соединения.
			CannotCreateDatabaseException currentException = new CannotCreateDatabaseException(
				"Ошибки при восстановлении многомерной базы данных");
			try
			{
				if (_databaseInfo == null) throw new DatabaseInfoIsNullException();
				databaseInfo = _databaseInfo;                

				InitLog();
				ConnectToDataWarehouse();
				CreateOlapDatabase(currentException);
				CreateDataSource(databaseInfo.DataSource);
				CreateDataSourceView(databaseInfo.DataSourceView, currentException);

				CreateDimensions(true);
				CreateCubes();

                UpdateCalculations();
				//opRestoreCubes.EndOperation();

				//TODO При завершении глобальной операции - завершать всех ее потомков.
				EndOperation(globalOp);
			}
			catch (Exception e)
			{
				EndOperationWithErrors(globalOp, e);
				databaseInfo.DB = null;
			}
			finally
			{
				if (log != null)
					log.ToggleButtons(true);
			}
			return DB;
		}
	}
}