using Microsoft.AnalysisServices;


namespace Krista.FM.Server.OLAP.Processor
{
    public static class ProcessorUtils
    {
        public static int CompareProcessTypes(ProcessType processType1, ProcessType processType2)
        {
            switch (processType1)
            {
                default:
                case ProcessType.ProcessAdd:
                case ProcessType.ProcessUpdate:
                    switch (processType2)
                    {
                        default:
                        case ProcessType.ProcessAdd:
                        case ProcessType.ProcessUpdate:
                            return 0;
                        case ProcessType.ProcessData:
                            return -1;
                        case ProcessType.ProcessFull:
                            return -1;
                    }
                case ProcessType.ProcessData:
                    switch (processType2)
                    {
                        default:
                        case ProcessType.ProcessAdd:
                        case ProcessType.ProcessUpdate:
                            return 1;
                        case ProcessType.ProcessData:
                            return 0;
                        case ProcessType.ProcessFull:
                            return -1;
                    }
                case ProcessType.ProcessFull:
                    switch (processType2)
                    {
                        default:
                        case ProcessType.ProcessAdd:
                        case ProcessType.ProcessUpdate:
                            return 1;
                        case ProcessType.ProcessData:
                            return 1;
                        case ProcessType.ProcessFull:
                            return 0;
                    }
            }
        }

        public static AnalysisState ConvertToAS2005State(DSO.OlapStateTypes stateType)
        {
            switch (stateType)
            {
                case DSO.OlapStateTypes.olapStateCurrent:
                    return AnalysisState.Processed;
                case DSO.OlapStateTypes.olapStateMemberPropertiesChanged:
                case DSO.OlapStateTypes.olapStateNeverProcessed:
                case DSO.OlapStateTypes.olapStateSourceMappingChanged:
                case DSO.OlapStateTypes.olapStateStructureChanged:
                default:
                    return AnalysisState.Unprocessed;
            }
        }

        public static DSO.ProcessTypes ConvertToAS2000ProcessType(ProcessType processType)
        {
            switch (processType)
            {
                case ProcessType.ProcessAdd:
                    return DSO.ProcessTypes.processReaggregate;                    
                case ProcessType.ProcessClear:
                    return DSO.ProcessTypes.processBuildStructure;                    
                case ProcessType.ProcessClearStructureOnly:
                    return DSO.ProcessTypes.processBuildStructure;
                case ProcessType.ProcessData:
                    return DSO.ProcessTypes.processRefreshData;                    
                case ProcessType.ProcessDefault:
                    return DSO.ProcessTypes.processDefault;                    
                case ProcessType.ProcessFull:
                    return DSO.ProcessTypes.processFull;                    
                case ProcessType.ProcessIndexes:
                    return DSO.ProcessTypes.processRefreshDataAndIndex;                    
                case ProcessType.ProcessScriptCache:
                    return DSO.ProcessTypes.processDefault;                    
                case ProcessType.ProcessStructure:
                    return DSO.ProcessTypes.processBuildStructure;                    
                case ProcessType.ProcessUpdate:
                    return DSO.ProcessTypes.processRefreshData;                    
                default:
                    return DSO.ProcessTypes.processDefault;                    
            }
        }
    }    
}