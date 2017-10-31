namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public class ConsRelationRoot : ConsRelationExpression
    {
        public ConsRelationRoot()
        {
        }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
