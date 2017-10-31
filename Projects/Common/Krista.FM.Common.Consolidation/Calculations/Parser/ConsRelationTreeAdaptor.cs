using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Krista.FM.Common.Consolidation.Calculations.Parser
{
    public class ConsRelationTreeAdaptor : CommonTreeAdaptor
    {
        public override object Create(IToken payload)
        {
            return new ConsRelationTree(payload);
        }

        public override object Nil()
        {
            return new ConsRelationTree(null);
        }

        public override object DupNode(object treeNode)
        {
            if (treeNode == null)
            {
                return null;
            }

            return Create(((ConsRelationTree)treeNode).Token);
        }

        public override object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
        {
            return new ConsRelationTreeErrorNode(input, start, stop, e);
        }
    }
}
