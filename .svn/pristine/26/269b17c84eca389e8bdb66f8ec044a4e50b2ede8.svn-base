using System;

using Krista.FM.Server.ProcessorLibrary;


namespace Krista.FM.Server.OLAP.Processor
{
    public static class TraceUtils
    {
        public static OlapObjectType GetPartitionParents(string objectPath, out string cubeId, out string measureGroupId)
        {
            cubeId = string.Empty;
            measureGroupId = string.Empty;
            if (!string.IsNullOrEmpty(objectPath))
            {
                string[] idArray = objectPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
                if (idArray.Length == 5)
                {
                    cubeId = idArray[2];
                    measureGroupId = idArray[3];
                }
            }
            if (string.IsNullOrEmpty(measureGroupId))
            {
                return OlapObjectType.Dimension;
            }
            else
            {
                return OlapObjectType.Partition;
            }
        }
    }
}
