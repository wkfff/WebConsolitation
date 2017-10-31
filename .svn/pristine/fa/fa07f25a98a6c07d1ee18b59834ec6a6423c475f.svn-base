using Ext.Net.MVC;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetAjaxStoreResultExtension
    {
        public static AjaxStoreResult Error(this AjaxStoreResult result, string message)
        {
            result.SaveResponse.Message = message;
            result.SaveResponse.Success = false;

            return result;
        }
    }
}
