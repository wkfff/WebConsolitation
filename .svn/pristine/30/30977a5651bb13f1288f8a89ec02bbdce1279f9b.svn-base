using System;
using Infragistics.Win;
using Krista.FM.Client.SMO;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AttributeReferencePresentationControl : AttributeControl
    {
        public AttributeReferencePresentationControl(IDataAttribute controlObject, CustomTreeNodeControl parent)
            : base(SmoAttributePresentationDesign.SmoAttributeFactory(controlObject), parent)
        {
            if ((controlObject.Kind == DataAttributeKindTypes.Regular) && !controlObject.Visible)
                Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
        }

        public AttributeReferencePresentationControl(SmoAttributePresentationDesign smoObject, CustomTreeNodeControl parent)
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
                        if (((SmoAttributePresentationReferenceDesign)this.ControlObject).Position < targetNode)
                        {
                            ((SmoAttributePresentationReferenceDesign)this.ControlObject).Position =
                                targetNode - 1;
                        }
                        else
                        {
                            ((SmoAttributePresentationReferenceDesign)this.ControlObject).Position =
                                targetNode;
                        }
                        break;
                    case Infragistics.Win.UltraWinTree.NodePosition.Next:
                        if (((SmoAttributePresentationReferenceDesign)this.ControlObject).Position < targetNode)
                        {
                            ((SmoAttributePresentationReferenceDesign)this.ControlObject).Position =
                                targetNode;
                        }
                        else
                        {
                            ((SmoAttributePresentationReferenceDesign)this.ControlObject).Position =
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
