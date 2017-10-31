using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    /// <summary>
    /// Ссылка на ассоциацию из классификатора
    /// </summary>
    public class ReferenceAssociationControl : AssociationControl
    {
        public ReferenceAssociationControl(SmoAssociationDesign controlObject, CustomTreeNodeControl parent)
            : base(controlObject, parent)
        {
        }

        [MenuAction("Перейти к объекту", Images.GoToObject)]
        public void GoToObject()
        {
            SchemeEditor.SelectObject(((IAssociation)ControlObject).Key, false);
        }
    }
}
