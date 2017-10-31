using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SQLServerUtils.OLAPExceptions
{
	public class OLAPException : Exception
	{
		private OLAPException parent;
		private List<Exception> children;

		private OLAPException()
			: base()
		{ }

		public OLAPException(string message)
			: base(message)
		{
			children = new List<Exception>();
		}

		private OLAPException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		public OLAPException Parent
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
				if (childException is OLAPException) { ((OLAPException)childException).Parent = this; }
			}
		}
	}		

	public class CannotCreateDatabaseException : OLAPException
	{
		public CannotCreateDatabaseException(string databaseName)
			: base(databaseName)
		{ }
	}

	public class CannotCreateDimensionException : OLAPException
	{
		public CannotCreateDimensionException(string dimensionName)
			: base(dimensionName)
		{ }
	}

	public class CannotCreateDimensionAttributeException : OLAPException
	{
		public CannotCreateDimensionAttributeException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateDimensionAttributesException : OLAPException
	{
		public CannotCreateDimensionAttributesException(string message)
			: base(message)
		{ }
	}	

	public class CannotCreateDataSourceViewException : OLAPException
	{
		public CannotCreateDataSourceViewException(string datasourceViewName)
			: base(datasourceViewName)
		{ }
	}

	public class CannotCreateCreateDataItemException : OLAPException
	{
		public CannotCreateCreateDataItemException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateCubeException : OLAPException
	{
		public CannotCreateCubeException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateCubeDimensionException : OLAPException
	{
		public CannotCreateCubeDimensionException(string message)
			: base(message)
		{ }
	}	

	public class CannotCreateCubeMeasureGroupException : OLAPException
	{
		public CannotCreateCubeMeasureGroupException(string message)
			: base(message)
		{ }
	}	

	public class CannotCreateHierarchyException : OLAPException
	{
		public CannotCreateHierarchyException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateHierarchyLevelException : OLAPException
	{
		public CannotCreateHierarchyLevelException(string message)
			: base(message)
		{ }
	}

	public class CannotCreateLinkedMeasureGroupException : OLAPException
	{
		public CannotCreateLinkedMeasureGroupException(string message)
			: base(message)
		{ }
	}	

	public class CannotCreateMeasureGroupDimensionException : OLAPException
	{
		public CannotCreateMeasureGroupDimensionException(string dimensionName)
			: base(dimensionName)
		{ }
	}	

	public class CantCreateMeasureException : OLAPException
	{
		public CantCreateMeasureException(
			string measureName, string mgName)
			: base(String.Format(
			"Не могу создать меру \"{0}\" в группе мер \"{1}\"", measureName, mgName))
		{ }
	}	

	public class CannotCreateRelationException : OLAPException
	{
		public CannotCreateRelationException(string message)
			: base(message)
		{ }
	}	

	public class CannotCreateRollUpColumnException : OLAPException
	{
		public CannotCreateRollUpColumnException(string message)
			: base(message)
		{ }
	}	

	public class CannotFindAttributeInDimensionException : OLAPException
	{
		public CannotFindAttributeInDimensionException(string dimensionName,
			string attributeName)
			: base(String.Format("В измерении \"{0}\" не найден аттрибут \"{1}\"",
						dimensionName, attributeName))
		{ }
	}

	public class CannotFindCubeDimensionException : OLAPException
	{
		public CannotFindCubeDimensionException(string cubeName, string cubeDimensionName)
			: base(String.Format("В кубе \"{0}\" не найдено измерение \"{1}\"",
						cubeName, cubeDimensionName))
		{ }

		public CannotFindCubeDimensionException(string message)
			: base(message)
		{ }
	}

	public class CannotFindMeasureGroupDimensionException : OLAPException
	{
		public CannotFindMeasureGroupDimensionException(string mgName, string mgDimensionName)
			: base(String.Format("В группе мер \"{0}\" не найдено измерение \"{1}\"",
			mgName, mgDimensionName))
		{ }

		public CannotFindMeasureGroupDimensionException(string message)
			: base(message)
		{ }
	}

	public class CubeMeasureGroupsListIsEmptyException : OLAPException
	{
		public CubeMeasureGroupsListIsEmptyException()
			: base("Список групп мер для куба пуст")
		{ }
	}

	public class CubeHasNoDimensionsException : OLAPException
	{
		public CubeHasNoDimensionsException()
			: base("Куб не содержит ни одного измерения")
		{ }
	}

	public class CubeHasNoMeasureGroupsException : OLAPException
	{
		public CubeHasNoMeasureGroupsException()
			: base("Куб не содержит ни одной группы мер")
		{ }
	}

	public class CubeDimensionsListIsEmptyException : OLAPException
	{
		public CubeDimensionsListIsEmptyException()
			: base("Список измерений пуст")
		{ }
	}

	public class CannotSetAttributeNameColumnException : OLAPException
	{
		public CannotSetAttributeNameColumnException(string message)
			: base(message)
		{ }
	}

	public class DatabaseInfoIsNullException : OLAPException
	{
		public DatabaseInfoIsNullException()
			: base("Описание многомерной базы отсутствует")
		{ }
	}

	public class DimensionHasNoHierarchiesException : OLAPException
	{
		public DimensionHasNoHierarchiesException()
			: base("В измерении не задано ни одной иерархии ")
		{ }
	}	

	public class MeasureGroupHasNoDimensionsException : OLAPException
	{
		public MeasureGroupHasNoDimensionsException(string message)
			: base(message)
		{ }
	}

	public class TableNotExistException : OLAPException
	{
		public TableNotExistException(string tableName)
			: base(string.Format("Таблица \"{0}\" не найдена.", tableName))
		{ }
	}
}