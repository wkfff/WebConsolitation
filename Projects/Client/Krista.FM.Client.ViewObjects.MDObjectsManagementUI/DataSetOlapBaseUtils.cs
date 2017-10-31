using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    /// <summary>
    /// ������������ ��� ��������� �������� (� �������� �� ������� ������) �� ��������,
    /// ������������ ��������� ����������� ����.
    /// ������� ����������� ���� ���������, ���������� � ������������ ������.
    /// � �������� ������ ������� ��������� � ������� �� ����������.
    /// </summary>
    //public class DataSetOlapBaseUtils
    //{
    //    //������� � ��������� ����������� ����.
    //    private DataSet dataSetOlapBase;
    //    //������������� ����������� ����.
    //    private string databaseId;

    //    //��������� ������� ����������� ����. ���������� ��� �������� ������ �� ������.
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
    //    /// ���������� ������ �������� ����.
    //    /// </summary>
    //    /// <param name="cubeId">������������� ����</param>
    //    /// <returns>������ �������� ����.</returns>
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
    //    /// ���������� ������ ��������� ����.
    //    /// </summary>
    //    /// <param name="cubeId">������������� ����.</param>
    //    /// <returns>������ ��������� ����.</returns>
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
    //    /// ���������� ������ �����, ������������ ������ ���������.
    //    /// </summary>
    //    /// <param name="dimensionId">������������� ���������.</param>
    //    /// <returns>������ �����.</returns>
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
    //            //���� �������� - ������ ���, �� ����������� ��� � ���.
    //            if ((OlapObjectType)measureGroupDataRow["ObjectType"] == OlapObjectType.measureGroup)
    //            {
    //                cubeDataRow = measureGroupDataRow.GetParentRow("Parent_Child");
    //            }
    //            //���� �������� ��� - �� � ������ ������ ������ �� ����.
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
    //            //���� �������� - ������ ���, �� ����������� ��� � ���.
    //            if ((OlapObjectType)measureGroupDataRow["ObjectType"] == OlapObjectType.measureGroup)
    //            {
    //                cubeDataRow = olapObjects.Rows.Find(measureGroupDataRow["ParentId"]);
    //            }
    //            //���� �������� ��� - �� � ������ ������ ������ �� ����.
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

    //    //�������� ��������� �� ������.
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