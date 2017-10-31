using System;

using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class HierarchyControl : ModifiableListControl<IDimensionLevelCollection, IDimensionLevel>/*CustomListControl<IDimensionLevelCollection, IDimensionLevel>*/
    {
        public HierarchyControl(IDimensionLevelCollection controlObject, CustomTreeNodeControl parent)
            : base("IDimensionLevelCollection", "��������", 
            new SmoDimensionLevelCollectionDesign(controlObject), parent, (int)Images.Hierarchy)
        {
        }

        public override void OnChange(object sender, EventArgs e)
        {
            base.OnChange(sender, e);
            Refresh();
        }

        public override CustomTreeNodeControl Create(IDimensionLevel item)
        {
            SmoDimensionLevelDesign level;

            IDimensionLevelCollection levels = (IDimensionLevelCollection)ControlObject;
            
            if (item.LevelType == LevelTypes.All)
                level = new SmoDimensionLevelAllDesign(item);
            else if (levels.HierarchyType == HierarchyType.Regular)
                level = new SmoDimensionLevelRegularDesign(item);
            else
                level = new SmoDimensionLevelDesign(item);

            return new DimensionLevelControl(level, this);
        }

        [MenuAction("������� ������� All", CheckMenuItemEnabling = "IsEditable")]
        public void AddNewAllLevel()
        {
            SmoDictionaryBaseDesign<string, IDimensionLevel> smoCollection = (SmoDictionaryBaseDesign<string, IDimensionLevel>)ControlObject;
            IDimensionLevelCollection collection = (IDimensionLevelCollection)smoCollection.ServerControl;
            IDimensionLevel newObj = collection.CreateItem("���", LevelTypes.All);
            Nodes.Add(Create(newObj));
            OnChange(this, new EventArgs());
        }

        public bool AllowCreateLevel()
        {
            IDimensionLevelCollection levels = (IDimensionLevelCollection)ControlObject;
            return levels.HierarchyType == HierarchyType.Regular && IsEditable();
        }

        [MenuAction("�������", CheckMenuItemEnabling = "AllowCreateLevel")]
        public override void AddNew()
        {
            SmoDictionaryBaseDesign<string, IDimensionLevel> smoCollection = (SmoDictionaryBaseDesign<string, IDimensionLevel>)ControlObject;
            IDimensionLevelCollection collection = (IDimensionLevelCollection)smoCollection.ServerControl;

            IDimensionLevel newObj = collection.CreateItem(
                String.Format("������� {0}", collection.Count), 
                LevelTypes.Regular);
            
            Nodes.Add(Create(newObj));

            OnChange(this, new EventArgs());
        }
    }
}
