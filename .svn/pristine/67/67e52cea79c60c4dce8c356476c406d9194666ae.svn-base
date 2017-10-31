using System;
using System.Text;
using Ext.Net;
using Ext.Net.Utilities;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// StoreResponseData с дополнительными параметрами ответа.
    /// </summary>
    public class ExtraStoreResponseData : StoreResponseData
    {
        private string extraParams;
        
        /// <summary>
        /// Дополнительные параметры ответа.
        /// </summary>
        public string ExtraParams
        {
            get { return this.extraParams; }
            set { this.extraParams = value; }
        }

        public bool Success { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Data) && (this.Confirmation == null || this.Confirmation.Count == 0))
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            if (this.Data.IsNotEmpty())
            {
                sb.AppendFormat("data:{0}, totalCount: {1}, success: {2}", this.Data, this.Total, this.Success ? "true" : "false");
                if (this.Message.IsNotEmpty())
                {
                    sb.Append(", message: '").Append(this.Message).Append('\'');
                }

                sb.Append(", extraParams: ").Append(this.ExtraParams).Append(",");
            }

            string returnConfirmation = String.Empty;
            if (this.Confirmation != null && this.Confirmation.Count > 0)
            {
                returnConfirmation = this.Confirmation.ToJson();
            }

            if (returnConfirmation.IsNotEmpty())
            {
                sb.AppendFormat("confirm:{0}", returnConfirmation);
            }
            else
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("}");
            return sb.ToString();
        }
    }
}