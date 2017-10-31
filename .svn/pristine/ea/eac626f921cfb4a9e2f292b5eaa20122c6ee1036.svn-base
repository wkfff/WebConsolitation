using System;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Common.Validations.Messages
{
    [Serializable]
	public class ValidationMessage : IValidatorMessageHolder
    {
        private string detail;
        private bool hasError;
        private string summary;
		private string message;

        public ValidationMessage()
            : this(String.Empty)
        {
            hasError = false;
        }

        public ValidationMessage(string summary)
            : this(summary, summary)
        {
        }

        public ValidationMessage(string summary, string detail)
        {
            hasError = true;
            Summary = summary;
            Detail = detail;
        }

		public ValidationMessage(string message, string summary, string detail)
		{
			hasError = true;
			Summary = summary;
			Detail = detail;
			Message = message;
		}

        public bool HasError
        {
            get { return hasError; }
            set { hasError = value; }
        }

        public string Detail
        {
            get { return detail; }
            set { detail = value; }
        }

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }

    	/// <summary>
    	/// Используется для обмена сообщениями между сервером и клиентом
    	/// </summary>
		public string Message
    	{
    		get { return message; }
    		set { message = value; }
    	}
    }
}