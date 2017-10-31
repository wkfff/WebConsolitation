using System.Collections.Generic;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public class CalcFunction : ConsRelationExpression
    {
        public CalcFunction(CalcFunctionTypes type, List<ConsRelationExpression> parameters)
        {
            Type = type;
            Parameters = parameters;
        }

        public CalcFunction()
        {
        }

        public CalcFunctionTypes Type { get; set; }

        public List<ConsRelationExpression> Parameters { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
