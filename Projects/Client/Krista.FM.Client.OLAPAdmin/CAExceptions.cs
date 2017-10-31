using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.OLAPAdmin.CAExceptions
{
	public class CAException : Exception
	{
		private CAException parent;
		private List<Exception> children;

		private CAException()
			: base()
		{}

		public CAException(string message)
			: base(message)
		{
			children = new List<Exception>();
		}		

		private CAException(string message, Exception innerException)
			: base(message, innerException)
		{}
		
		public CAException Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public List<Exception> Children
		{
			get { return children; }
		}

		public void AddChild(Exception childException)
		{
			if (childException != null)
			{
				children.Add(childException);
				if (childException is CAException) { ((CAException)childException).Parent = this; }
			}
		}
	}

	public class CannotAddLinkedMeasureGroupDimensionException : CAException
	{
		public CannotAddLinkedMeasureGroupDimensionException(string message)
			: base(message)
		{ }
	}

	public class CannotAddLinkedMeasureGroupDimensionsException : CAException
	{
		public CannotAddLinkedMeasureGroupDimensionsException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateDatabaseException : CAException
	{
		public CannotCreateDatabaseException(string databaseName)
			: base(databaseName)
		{ }
	}

	public class CannotCreateDimensionException : CAException
	{
		public CannotCreateDimensionException(string dimensionName):base(dimensionName)
		{}		
	}

	public class CannotCreateDimensionAttributeException : CAException
	{
		public CannotCreateDimensionAttributeException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateDimensionAttributesException : CAException
	{
		public CannotCreateDimensionAttributesException(string message)
			: base(message)
		{ }		
	}

	public class CannotCreateDimensionsException : CAException
	{
		public CannotCreateDimensionsException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateDataSourceViewException : CAException
	{
		public CannotCreateDataSourceViewException(string datasourceViewName)
			: base(datasourceViewName)
		{ }		
	}

	public class CannotCreateCreateDataItem : CAException
	{
		public CannotCreateCreateDataItem(string message)
			: base(message)
		{ }		
	}

	public class CannotCreateCubeException : CAException
	{
		public CannotCreateCubeException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateCubeDimensionException : CAException
	{
		public CannotCreateCubeDimensionException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateCubeDimensionsException : CAException
	{
		public CannotCreateCubeDimensionsException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateCubeMeasureGroupException : CAException
	{
		public CannotCreateCubeMeasureGroupException(string message)
			: base(message)
		{ }		
	}

	public class CannotCreateCubeMeasureGroupsException : CAException
	{
		public CannotCreateCubeMeasureGroupsException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateHierarchyException : CAException
	{
		public CannotCreateHierarchyException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateHierarchyLevelException : CAException
	{
		public CannotCreateHierarchyLevelException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateLinkedMeasureGroupException : CAException
	{
		public CannotCreateLinkedMeasureGroupException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateLinkedMeasureGroupsException : CAException
	{
		public CannotCreateLinkedMeasureGroupsException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateMeasureGroupDimensionException : CAException
	{
		public CannotCreateMeasureGroupDimensionException(string dimensionName)
			: base(dimensionName)
		{ }
	}

	public class CannotCreateMeasureGroupDimensionsException : CAException
	{
		public CannotCreateMeasureGroupDimensionsException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateMeasureGroupMeasureException : CAException
	{
		public CannotCreateMeasureGroupMeasureException(
			string measureName, string mgName) : base(String.Format(
			"Не могу создать меру \"{0}\" в группе мер \"{1}\"", measureName, mgName))
		{ }
	}

	//public class CannotCreateMeasureGroupMeasuresException : CAException
	//{
	//    public CannotCreateMeasureGroupMeasuresException(string message)
	//        : base(message)
	//    { }		
	//}

	public class CannotCreateParentChildHierarchyException : CAException
	{
		public CannotCreateParentChildHierarchyException(string message)
			: base(message)
		{ }		
	}

	public class CannotCreateRelationException : CAException
	{
		public CannotCreateRelationException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateRelationsException : CAException
	{
		public CannotCreateRelationsException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateRollUpColumnException : CAException
	{
		public CannotCreateRollUpColumnException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateVirtualCubeException : CAException
	{
		public CannotCreateVirtualCubeException(string message)
			: base(message)
		{ }
	}

	public class CannotFindAttributeInDimensionException : CAException
	{
		public CannotFindAttributeInDimensionException(string dimensionName,
			string attributeName)
			: base(String.Format("В измерении \"{0}\" не найден аттрибут \"{1}\"",
						dimensionName, attributeName))
		{ }
	}

	public class CannotFindCubeDimensionException : CAException
	{
		public CannotFindCubeDimensionException(string cubeName, string cubeDimensionName)
			: base(String.Format("В кубе \"{0}\" не найдено измерение \"{1}\"",
						cubeName, cubeDimensionName))
		{ }

		public CannotFindCubeDimensionException(string message)
			: base(message)
		{ }		
	}

	public class CannotFindMeasureGroupDimensionException : CAException
	{
		public CannotFindMeasureGroupDimensionException(string mgName, string mgDimensionName)
			: base(String.Format("В группе мер \"{0}\" не найдено измерение \"{1}\"",
			mgName, mgDimensionName))
		{ }

		public CannotFindMeasureGroupDimensionException(string message)
			: base(message)
		{ }		
	}

	public class CubeMeasureGroupsListIsEmptyException : CAException
	{
		public CubeMeasureGroupsListIsEmptyException()
			: base("Список групп мер для куба пуст")
		{ }
	}

	public class CubeHasNoDimensionsException : CAException
	{
		public CubeHasNoDimensionsException()
			: base("Куб не содержит ни одного измерения")
		{ }
	}

	public class CubeHasNoMeasureGroupsException : CAException
    {
        public CubeHasNoMeasureGroupsException()
            : base("Куб не содержит ни одной группы мер")
        { }
    }

	public class CubeDimensionsListIsEmptyException : CAException
	{
		public CubeDimensionsListIsEmptyException()
			: base("Список измерений пуст")
		{ }
	}

	public class CannotSetAttributeNameColumnException : CAException
	{
		public CannotSetAttributeNameColumnException(string message)
			: base(message)
		{ }
	}

	public class DatabaseInfoIsNullException : CAException
	{
		public DatabaseInfoIsNullException()
			: base("Описание многомерной базы отсутствует")
		{ }
	}

	public class DimensionHasNoHierarchiesException : CAException
	{
		public DimensionHasNoHierarchiesException()
			: base("В измерении не задано ни одной иерархии ")
		{ }
	}

	public class MeasureGroupDimensionsListIsEmptyException : CAException
	{
		public MeasureGroupDimensionsListIsEmptyException()
			: base("Список измерений пуст")
		{ }
	}

	public class MeasureGroupHasNoDimensionsException : CAException
	{
		public MeasureGroupHasNoDimensionsException(string message)
			: base(message)
		{ }
	}

	public class TableNotExistException : CAException
	{
		public TableNotExistException(string tableName)
			: base(string.Format("Таблица \"{0}\" не найдена.", tableName))
		{ }
	}
}
