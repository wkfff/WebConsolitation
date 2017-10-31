using System.Windows.Forms.Design;

namespace Krista.FM.Client.Design
{
    public class LineFrameDesigner : ParentControlDesigner
    {
        public override System.Windows.Forms.Design.SelectionRules SelectionRules
        {
            get
            {
                SelectionRules sel = SelectionRules.LeftSizeable |
                  SelectionRules.RightSizeable |
                  SelectionRules.Moveable |
                  SelectionRules.Visible;

                return sel;
            }
        }
    }
}
