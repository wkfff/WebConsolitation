using System;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public class VersionParams
    {
        public VersionParams(string calculationComment)
            : this(null, calculationComment)
        {

        }

        public VersionParams(DateTime? calculationDate, string calculationComment)
        {
            CalculationComment = calculationComment;
            CalculationDate = calculationDate;
        }

        public string CalculationComment
        {
            get; set;
        }

        public DateTime? CalculationDate
        {
            get; set;
        }

        public override string ToString()
        {
            if (CalculationDate != null)
                return string.Format("{0} ({1})", CalculationComment, CalculationDate.Value.ToShortDateString());
            return CalculationComment;
        }

        public override bool Equals(object obj)
        {
            var eqObject = obj as VersionParams;
            if (eqObject == null)
                return false;
            return eqObject.CalculationDate == CalculationDate &&
                   string.Compare(eqObject.CalculationComment, CalculationComment, true) == 0;
        }
    }
}
