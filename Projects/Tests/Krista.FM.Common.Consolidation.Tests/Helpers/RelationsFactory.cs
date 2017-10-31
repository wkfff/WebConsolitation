using Krista.FM.Domain;

namespace Krista.FM.Common.Consolidation.Tests.Helpers
{
    public static class RelationsFactory
    {
        public static D_Form_Relations Create(string left, string op, string right)
        {
            return new D_Form_Relations { LeftPart = left, RalationType = op, RightPart = right };
        }

        public static D_Form_Relations Create(string left, string op, string right, int activationType)
        {
            return new D_Form_Relations { LeftPart = left, RalationType = op, RightPart = right, ActivationType = activationType };
        }
    }
}
