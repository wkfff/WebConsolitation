using System;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services.Exceptions
{
    public class CannotAcceptReportException : Exception
    {
        public CannotAcceptReportException()
            : base("Доклад не может быть принят, так как не все показатели утверждены УИМ или находятся на редактировании у МО.")
        {
        }
    }
}
