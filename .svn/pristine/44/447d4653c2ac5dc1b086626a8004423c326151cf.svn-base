using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core
{
    public class AjaxStoreExtraResult : AjaxStoreResult
    {
        private object extraParams;

        public AjaxStoreExtraResult(object data, int totalCount, object extraParams)
            : base(data, totalCount)
        {
            this.extraParams = extraParams;
        }

        public object ExtraParams
        {
            get { return this.extraParams; }
            set { this.extraParams = value; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (this.ResponseFormat == StoreResponseFormat.Load)
            {
                ExtraStoreResponseData storeResponse = new ExtraStoreResponseData();
                storeResponse.Data = JSON.Serialize(this.Data, new List<JsonConverter> { new DataTableConverter(), new DataRowConverter() });
                storeResponse.Total = this.Total;
                storeResponse.Success = this.SaveResponse.Success;
                storeResponse.Message = this.SaveResponse.Message;
                storeResponse.ExtraParams = JSON.Serialize(this.ExtraParams, new List<JsonConverter>());
                storeResponse.Return();
                return;
            }

            if (this.ResponseFormat == StoreResponseFormat.Save)
            {
                ExtraStoreResponseData storeResponse = new ExtraStoreResponseData();
                storeResponse.Data = JSON.Serialize(this.Data, new List<JsonConverter> { new DataTableConverter(), new DataRowConverter() });
                storeResponse.Total = this.Total;
                storeResponse.Success = this.SaveResponse.Success;
                storeResponse.Message = this.SaveResponse.Message;
                storeResponse.ExtraParams = JSON.Serialize(this.ExtraParams, new List<JsonConverter>());
                storeResponse.Return();
                return;
            }

            base.ExecuteResult(context);
        }
    }
}