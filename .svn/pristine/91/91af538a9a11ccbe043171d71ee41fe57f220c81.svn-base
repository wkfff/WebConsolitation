using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AssociationListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IEntityAssociation>, IEntityAssociation>
    {
        public AssociationListControl(string name, string text, IEntityAssociationCollection controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(name, text, new SmoDictionaryBaseDesign<string, IEntityAssociation>(controlObject), parent, imageIndex)
        {
        }

        public override CustomTreeNodeControl Create(IEntityAssociation item)
        {
            ICommonDBObject currentContext = ((CustomTreeNodeControl)Parent).ControlObject as ICommonDBObject;
            string nameContext = String.Empty;
            if (currentContext is IPackage)
                nameContext = currentContext.Key;
            else if (currentContext is IEntity)
                nameContext = currentContext.ParentPackage.Key;

            SmoAssociationDesign association;
            if (item.ParentPackage.Key != nameContext)
                association = SmoAssociationReadOnlyDesign.CreateInstance(item);
            else
                association = SmoAssociationDesign.CreateInstance(item);

            return new AssociationControl(association, this);
        }

        public override void AddNew()
        {
        }
    }

    public class PackageReferenceAssociationListControl : AssociationListControl
    {
        public PackageReferenceAssociationListControl(IEntityAssociationCollection controlObject, CustomTreeNodeControl parent)
            : base("IEntityAssociationCollection", "Ассоциации", controlObject, parent, (int)Images.Associasions)
        {
        }
    }

    public class  ReferenceAssociationLilsControlBase : AssociationListControl
    {
        public ReferenceAssociationLilsControlBase(string name, string text, IEntityAssociationCollection controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(name, text, controlObject, parent, imageIndex)
        {
        }

        public override CustomTreeNodeControl Create(IEntityAssociation item)
        {
            ICommonDBObject currentContext = ((CustomTreeNodeControl)Parent).ControlObject as ICommonDBObject;
            string nameContext = String.Empty;
            if (currentContext is IPackage)
                nameContext = currentContext.Key;
            else if (currentContext is IEntity)
                nameContext = currentContext.ParentPackage.Key;

            SmoAssociationDesign association;
            if (item.ParentPackage.Key != nameContext)
                association = SmoAssociationReadOnlyDesign.CreateInstance(item);
            else
                association = SmoAssociationDesign.CreateInstance(item);

            return new ReferenceAssociationControl(association, this);
        }
    }

    public class ReferenceAssociationListControl : ReferenceAssociationLilsControlBase
    {
        public ReferenceAssociationListControl(IEntityAssociationCollection controlObject, CustomTreeNodeControl parent)
            : base("IEntityAssociationCollection", "Ассоциации", controlObject, parent, (int)Images.Associasions)
        {
        }
    }

    public class ReferencedByAssociationListControl : ReferenceAssociationLilsControlBase
    {
        public ReferencedByAssociationListControl(IEntityAssociationCollection controlObject, CustomTreeNodeControl parent)
            : base("ReferencedByAssociationCollection", "Используется в", controlObject, parent, (int)Images.Associasions2)
        {
        }
    }

}
