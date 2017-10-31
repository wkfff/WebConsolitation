using WatiN.Core;
using WatiN.Core.Constraints;

namespace Krista.FM.RIA.Integration.Tests.Controls
{
    public class Button : Control<Table>
    {
        public override Constraint ElementConstraint
        {
            get
            {
                return Find.ByClass("x-btn") 
                    || Find.ByClass("x-btn x-btn-text-icon") 
                    || Find.ByClass("x-btn  x-btn-text-icon")
                    || Find.ByClass("x-btn x-btn-noicon");
            }
        }

        public void Click()
        {
            Element.Buttons[0].Click();
        }
    }
}
