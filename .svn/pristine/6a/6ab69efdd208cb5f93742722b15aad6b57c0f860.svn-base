namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public class GroupFunction : ConsRelationExpression
    {
        public GroupFunction(GroupFunctionTypes type, ConsRelationExpression rvalueSelector)
        {
            Type = type;
            RValueSelector = rvalueSelector;
        }

        public GroupFunction()
        {
        }

        public GroupFunctionTypes Type { get; set; }

        public ConsRelationExpression RValueSelector { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
