using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AnalysisServices;

using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Server.OLAP.Processor
{
    public class ProcessableObjectInfo : MarshalByRefObject, IProcessableObjectInfo
    {   
        private ProcessableMajorObject processableObject;
        private ProcessType objectProcessType = ProcessType.ProcessDefault;
        private string databaseId;        
        private Partition partition;


        public ProcessableObjectInfo(ProcessableMajorObject _processableObject, ProcessType _processType)
        {   
            processableObject = _processableObject;
            objectProcessType = _processType;
            partition = processableObject as Partition;
            databaseId = GetDataBaseId(processableObject);
        }        

        private string GetDataBaseId(ProcessableMajorObject processableObject)
        {
            if (processableObject is Cube || processableObject is Dimension)
            {
                return ((Database)processableObject.Parent).ID;
            }
            //Рассчитываемый объект - раздел куба
            return ((Database)processableObject.Parent.Parent.Parent).ID;
        }

        [Browsable(false)]
        public ProcessableMajorObject ProcessableObject
        {
            get { return processableObject; }
            set { processableObject = value; }
        }

        [DisplayName("Тип рассчета")]
        public ProcessType ObjectProcessType
        {
            get { return objectProcessType; }
            set
            {
                if (value == ProcessType.ProcessAdd && processableObject is Dimension)
                {
                    objectProcessType = ProcessType.ProcessUpdate;
                }
                else
                {
                    if (value == ProcessType.ProcessUpdate && processableObject is Partition)
                    {
                        objectProcessType = ProcessType.ProcessAdd;
                    }
                    else
                    {
                        objectProcessType = value;
                    }
                }
            }
        }

        [DisplayName("ID")]
        public string ObjectID
        {
            get { return processableObject.ID; }
        }

        [DisplayName("Имя")]
        public string Name
        {
            get { return processableObject.Name; }
        }        

        /// <summary>
        /// Используется при генерации скрипта для расчета. Должно принимать значения "Dimension" или "Partition".
        /// </summary>
        public OlapObjectType ObjectType
        {
            get
            {
                if (processableObject is Dimension)
                {
                    return OlapObjectType.dimension;
                }
                else
                {
                    return OlapObjectType.partition;
                }
            }
        }

        [Browsable(false)]
        public string DatabaseId
        {
            get { return databaseId; }
        }
        
        [Browsable(false)]
        public string CubeId
        {
            get
            {
                if (partition != null)
                    return partition.ParentCube.ID;
                else                
                    return null;
            }
        }

        [Browsable(false)]
        public string MeasureGroupId
        {
            get
            {
                if (partition != null)
                    return partition.Parent.ID;
                else
                    return null;
            }
        }

        [Browsable(false)]
        public string CubeName
        {
            get
            {
                Partition partition = processableObject as Partition;
                if (partition != null)
                {
                    return partition.ParentCube.Name;
                }
                return null;
            }
        }

        public override string ToString()
        {
            return processableObject.Name;
        }
    }
}
