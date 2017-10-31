using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.Server.OLAP.Processor;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public static class ProcessorUtils
    {
        public static OptionsResult SelectObjectToProcess(DataGridView grid,
            DataGridViewCellMouseEventArgs e,
            DataGridViewColumn columnId, DataGridViewColumn columnCubeId,
            IWin32Window owner, IOlapDBWrapper DBWrapper,
            out List<IProcessableObjectInfo> objectToProcessList)
        {
            if (e.Button == MouseButtons.Right)
            {                
                OlapObjectType olapObjectType = GetObjectType(grid);
                string id = GetCellValue(grid, e.RowIndex, columnId);
                ProcessableMajorObject objectToProcess = null;
                switch (olapObjectType)
                {
                    case OlapObjectType.dimension:
                        objectToProcess = DBWrapper.FindDimension(id);
                        break;
                    case OlapObjectType.cube:
                        objectToProcess = DBWrapper.FindCube(id);
                        break;
                    case OlapObjectType.partition:
                        objectToProcess = DBWrapper.FindPartition(id, GetCellValue(grid, e.RowIndex, columnCubeId));
                        break;
                    default:
                        break;
                }
                
                if (objectToProcess != null)
                {
                    FormProcessOptions frmOptions = new FormProcessOptions();                    
                    return frmOptions.ShowOptions(owner, DBWrapper, 
                        new ProcessableObjectInfo(objectToProcess,
                        ProcessType.ProcessFull) as IProcessableObjectInfo, out objectToProcessList);
                }                
            }
            objectToProcessList = new List<IProcessableObjectInfo>();
            return OptionsResult.cancel;
        }        

        private static string GetCellValue(DataGridView grid, int rowIndex, DataGridViewColumn column)
        {
            if (grid != null && column != null)
            {
                return grid.Rows[rowIndex].Cells[column.Index].Value.ToString();
            }
            return string.Empty;
        }

        private static string GetObjectIdFromRow(object sender, int rowIndex)
        {
            return string.Empty;
        }

        private static OlapObjectType GetObjectType(DataGridView grid)
        {
            if (grid.Name.Contains("Dimension"))
            {
                return OlapObjectType.dimension;
            }
            if (grid.Name.Contains("Cube"))
            {
                return OlapObjectType.cube;
            }            
            return OlapObjectType.partition;
        }
    }    
}