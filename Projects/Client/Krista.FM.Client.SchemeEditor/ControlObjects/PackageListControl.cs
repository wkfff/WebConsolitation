using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class PackageListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IPackage>, IPackage>
    {
        public PackageListControl(IPackageCollection controlObject, CustomTreeNodeControl parent)
            : base("IPackageCollection", "Пакеты", new SmoDictionaryBaseDesign<string, IPackage>(controlObject), parent, (int)Images.Folders)
        {
        }

        public override CustomTreeNodeControl Create(IPackage item)
        {
            return new PackageControl(item, this);
        }

        protected override void ExpandNode()
        {
            StartExpand();

            base.ExpandNode();

            EndExpand();
        }

        [MenuAction("Создать", Images.CreatePackage, CheckMenuItemEnabling = "IsEditable")]
        public override void AddNew()
        {
            base.AddNew();
        }
    }
}
