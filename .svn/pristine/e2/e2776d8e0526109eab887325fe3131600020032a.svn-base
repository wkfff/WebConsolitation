using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;

namespace Krista.FM.RIA.Core
{
    public class AjaxFormResult : ActionResult
    {
        public AjaxFormResult() { }

        [ConfigOption]
        public string Script { get; set; }

        private bool success = true;

        [ConfigOption]
        [DefaultValue("")]
        public bool Success
        {
            get { return this.success; }
            set { this.success = value; }
        }

        private List<FieldError> errors;

        [ConfigOption(JsonMode.AlwaysArray)]
        public List<FieldError> Errors
        {
            get
            {
                if (this.errors == null)
                {
                    this.errors = new List<FieldError>();
                }

                return this.errors;
            }
        }

        private ParameterCollection extraParams;
        public ParameterCollection ExtraParams
        {
            get
            {
                if (this.extraParams == null)
                {
                    this.extraParams = new ParameterCollection();
                }

                return this.extraParams;
            }
        }

        [ConfigOption("extraParams", JsonMode.Raw)]
        [DefaultValue("")]
        protected string ExtraParamsProxy
        {
            get
            {
                if (this.ExtraParams.Count > 0)
                {
                    return ExtraParams.ToJson();
                }

                return "";
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            CompressionUtils.GZipAndSend(new ClientConfig().Serialize(this));
        }
    }

    public class FieldError
    {
        public FieldError(string fieldID, string errorMessage)
        {
            if (string.IsNullOrEmpty(fieldID))
            {
                throw new ArgumentNullException("fieldID", "Field ID can not be empty");
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentNullException("errorMessage", "Error message can not be empty");
            }

            this.FieldID = fieldID;
            this.ErrorMessage = errorMessage;
        }

        [ConfigOption("id")]
        [DefaultValue("")]
        public string FieldID { get; set; }

        [ConfigOption("msg")]
        [DefaultValue("")]
        public string ErrorMessage { get; set; }
    }

    public class FieldErrors : Collection<FieldError> { }
}
