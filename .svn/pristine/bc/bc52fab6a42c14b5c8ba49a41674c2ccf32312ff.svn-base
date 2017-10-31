namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public class ClassProperty : ConsRelationExpression
    {
        public ClassProperty(string name)
        {
            Name = name;
        }

        public ClassProperty()
        {
        }

        public string Name { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
