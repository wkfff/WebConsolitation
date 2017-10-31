using System.Collections.Generic;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public class InStatement : ConsRelationExpression
    {
        public InStatement(ConsRelationExpression undependParam, List<ValueExpression> constants)
        {
            UndependParam = undependParam;
            Constants = constants;
        }

        public InStatement()
        {
        }

        public ConsRelationExpression UndependParam { get; set; }

        public List<ValueExpression> Constants { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
