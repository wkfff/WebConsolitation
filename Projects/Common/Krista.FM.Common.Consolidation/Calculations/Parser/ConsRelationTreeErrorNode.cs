using Antlr.Runtime;

namespace Krista.FM.Common.Consolidation.Calculations.Parser
{
    public class ConsRelationTreeErrorNode : ConsRelationTree
    {
        public ConsRelationTreeErrorNode(ITokenStream tokenStream, IToken start, IToken stop, RecognitionException e)
            : base(start)
        {
        }
    }
}
