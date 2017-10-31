using System;
using Infragistics.Win;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AttributePresentationControl : AttributeControl
    {
        public AttributePresentationControl(IDataAttribute controlObject, CustomTreeNodeControl parent)
            : base(SmoAttributePresentationDesign.SmoAttributeFactory(controlObject), parent)
        {
            if ((controlObject.Kind == DataAttributeKindTypes.Regular) && !controlObject.Visible)
                Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
        }

        public AttributePresentationControl(SmoAttributePresentationDesign smoObject, CustomTreeNodeControl parent)
            : base(smoObject, parent)
        {
        }

        public override bool RepositionAttribute(int targetNode, Infragistics.Win.UltraWinTree.NodePosition pos)
        {
            try
            {
                switch (pos)
                {
                    case Infragistics.Win.UltraWinTree.NodePosition.Previous:
                        if (((SmoAttributePresentationDesign)this.ControlObject).Position < targetNode)
                        {
                            ((SmoAttributePresentationDesign)this.ControlObject).Position =
                                targetNode - 1;
                        }
                        else
                        {
                            ((SmoAttributePresentationDesign)this.ControlObject).Position =
                                targetNode;
                        }
                        break;
                    case Infragistics.Win.UltraWinTree.NodePosition.Next:
                        if (((SmoAttributePresentationDesign)this.ControlObject).Position < targetNode)
                        {
                            ((SmoAttributePresentationDesign)this.ControlObject).Position =
                                targetNode;
                        }
                        else
                        {
                            ((SmoAttributePresentationDesign)this.ControlObject).Position =
                                targetNode + 1;
                        }
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
