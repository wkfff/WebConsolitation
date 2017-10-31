using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;

namespace Krista.FM.Client.Reports
{
    class EGRIPDataObject : EGRULDataObject
    {
        protected override string GetMainTableKey()
        {
            return ObjectKey.Length > 0 ? ObjectKey : b_IP_EGRIP.InternalKey;
        }
    }
}
