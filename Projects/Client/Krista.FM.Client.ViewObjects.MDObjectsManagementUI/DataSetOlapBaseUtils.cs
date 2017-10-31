using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    /// <summary>
    /// Предназначен для различных операций (в основном по выборке данных) из датасета,
    /// описывающего структуру многомерной базы.
    /// Датасет многомерной базы локальный, передается в конструкторе класса.
    /// В процессе работы никаких обращений к серверу не происходит.
    /// </summary>
    //public class DataSetOlapBaseUtils
    //{
    //    //Датасет с объектами многомерной базы.
    //    private DataSet dataSetOlapBase;
    //    //Идентификатор многомерной базы.
    //    private string databaseId;

    //    //Интрефейс оболчки многомерной базы. Необъходим для передачи данных на сервер.
    //    private IOlapDBWrapper olapDBWrapper;

    //    public static string TableName_OlapObjects = "OlapObjects";        
    //    public static string TableName_CubeDimensionLinks = "CubeDimensionLinks";

    //    public static string RelationName_CubesPartitions = "Parent_Child";
    //    public static string RelationName_CubesCubeDimensionLinks = "Cubes_CubeDimensionLinks";
    //    public static string RelationName_DimensionsCubeDimensionLinks = "Dimensions_CubeDimensionLinks";

    //    public static string FieldName_Id = "ObjectId";
        
    //    private DataTable dataTableOlapObjects;

    //    private DataSetOlapBaseUtils()
    //    { }

    //    public DataSetOlapBaseUtils(DataSet dataSetOlapBase, IOlapDBWrapper olapDBWrapper)
    //    {   
    //        Refresh(dataSetOlapBase, olapDBWrapper);
    //    }

    //    public void Refresh(DataSet dataSetOlapBase, IOlapDBWrapper olapDBWrapper)
    //    {
    //        this.olapDBWrapper = olapDBWrapper;
    //        this.dataSetOlapBase = dataSetOlapBase;
    //        this.databaseId = this.olapDBWrapper.DatabaseId;
    //        dataTableOlapObjects = this.dataSetOlapBase.Tables[TableName_OlapObjects];
    //    }        

    //    private DataRow GetRow(OlapObjectType objectType, string objectId)
    //    {
    //        return dataTableOlapObjects.Rows.Find(new object[] { objectType, objectId, databaseId });
    //    }

    //    private DataRow GetCubeRow(string cubeId)
    //    {
    //        return GetRow(OlapObjectType.cube, cubeId);
    //    }

    //    private DataRow GetDimensionRow(string dimensionId)
    //    {
    //        return GetRow(OlapObjectType.dimension, dimensionId);
    //    }

    //    /// <summary>
    //    /// Возвращает список разделов куба.
    //    /// </summary>
    //    /// <param name="cubeId">Идентификатор куба</param>
    //    /// <returns>Список разделов куба.</returns>
    //    public List<DataRow> GetCubePartitions(string cubeId)
    //    {
    //        List<DataRow> partitionList = new List<DataRow>();
    //        DataRow cubeRow = GetCubeRow(cubeId);            
    //        if (null != cubeRow)
    //        {
    //            partitionList.AddRange(cubeRow.GetChildRows(RelationName_CubesPartitions));
    //        }
    //        return partitionList;
    //    }

    //    /// <summary>
    //    /// Возвращает список измерений куба.
    //    /// </summary>
    //    /// <param name="cubeId">Идентификатор куба.</param>
    //    /// <returns>Список измерений куба.</returns>
    //    public List<DataRow> GetCubeDimensions(string cubeId)
    //    {
    //        List<DataRow> dimensionList = new List<DataRow>();
    //        DataRow cubeRow = GetCubeRow(cubeId);
    //        if (null != cubeRow)
    //        {
    //            DataRow[] links = cubeRow.GetChildRows(RelationName_CubesCubeDimensionLinks);
    //            for (int i = 0; i < links.Length; i++)
    //            {                    
    //                DataRow dimension = GetDimensionRow((string)links[i]["RefDimension"]);
    //                if (null != dimension)
    //                {
    //                    dimensionList.Add(dimension);
    //                }                    
    //            }                
    //        }
    //        return dimensionList;
    //    }

    //    /// <summary>
    //    /// Возвращает список кубов, использующих данное измерение.
    //    /// </summary>
    //    /// <param name="dimensionId">Идентификатор измерения.</param>
    //    /// <returns>Список кубов.</returns>
    //    public List<DataRow> GetDimensionCubes(string dimensionId)
    //    {
    //        List<DataRow> cubes = new List<DataRow>();
    //        DataRow dimensionRow = GetDimensionRow(dimensionId);
    //        if (null != dimensionRow)
    //        {
    //            DataRow[] links = dimensionRow.GetChildRows(RelationName_DimensionsCubeDimensionLinks);
    //            for (int i = 0; i < links.Length; i++)
    //            {
    //                DataRow cube = GetCubeRow((string)links[i]["RefCube"]);
    //                if (null != cube)
    //                {
    //                    cubes.Add(cube);
    //                }
    //            }
    //        }
    //        return cubes;
    //    }
       
    //    public string GetCubeName(string partitionId)
    //    {
    //        DataRow partitionRow = dataTableOlapObjects.Rows.Find(partitionId);
    //        if ((OlapObjectType)partitionRow["ObjectType"] == OlapObjectType.partition)
    //        {
    //            DataRow cubeDataRow = null;
    //            DataRow measureGroupDataRow = partitionRow.GetParentRow("Parent_Child");
    //            //Если родитель - группа мер, то вытаскиваем еще и куб.
    //            if ((OlapObjectType)measureGroupDataRow["ObjectType"] == OlapObjectType.measureGroup)
    //            {
    //                cubeDataRow = measureGroupDataRow.GetParentRow("Parent_Child");
    //            }
    //            //Если родитель куб - то и делать больше ничего не надо.
    //            {
    //                cubeDataRow = measureGroupDataRow;
    //            }
    //            return (string)cubeDataRow["ObjectName"];
    //        }
    //        return string.Empty;
    //    }

    //    public static string GetCubeName(DataTable olapObjects, string partitionId)
    //    {
    //        DataRow partitionRow = olapObjects.Rows.Find(partitionId);
    //        if ((OlapObjectType)partitionRow["ObjectType"] == OlapObjectType.partition)
    //        {
    //            DataRow cubeDataRow = null;
    //            DataRow measureGroupDataRow = olapObjects.Rows.Find(partitionRow["ParentId"]);
    //            //Если родитель - группа мер, то вытаскиваем еще и куб.
    //            if ((OlapObjectType)measureGroupDataRow["ObjectType"] == OlapObjectType.measureGroup)
    //            {
    //                cubeDataRow = olapObjects.Rows.Find(measureGroupDataRow["ParentId"]);
    //            }
    //            //Если родитель куб - то и делать больше ничего не надо.
    //            {
    //                cubeDataRow = measureGroupDataRow;
    //            }
    //            return (string)cubeDataRow["ObjectName"];
    //        }
    //        return string.Empty;
    //    }

    //    internal DataSet DataSetOlapBase
    //    {
    //        get { return this.dataSetOlapBase; }
    //    }

    //    internal DataTable DataTableOlapObjects
    //    {
    //        get { return dataTableOlapObjects; }
    //    }

    //    internal string DatabaseId
    //    {
    //        get { return this.databaseId; }
    //    }

    //    //Посылает изменения на сервер.
    //    internal void UpdateValues()
    //    {
    //        DataSet changes = dataSetOlapBase.GetChanges();
    //        if (null != changes && null != changes.Tables[TableName_OlapObjects] &&
    //            changes.Tables[TableName_OlapObjects].Rows.Count > 0)
    //        {
    //            this.olapDBWrapper.UpdateValues(changes);
    //            dataSetOlapBase.AcceptChanges();
    //        }            
    //    }

    //    internal IOlapDBWrapper OlapDBWrapper
    //    {
    //        get { return olapDBWrapper; }
    //    }
    //}
}