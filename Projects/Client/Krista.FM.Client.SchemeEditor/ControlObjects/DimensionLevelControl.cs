using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    internal class DimensionLevelControl : MinorObjectControl<SmoDimensionLevelDesign>
    {
        public DimensionLevelControl(SmoDimensionLevelDesign controlObject, CustomTreeNodeControl parent)
            : base(controlObject.Name, controlObject.Name, controlObject, parent, 0)
        {
        }

        /// <summary>
        /// ’ранимое наименование узла задаваемое при создании
        /// </summary>
        public override string Caption
        {
            get { return TypedControlObject.Name; }
        }

        public bool CanDelete()
        {
            // ћожно удал€ть, если тип Regular, и их больше одного.
            return ((((IDimensionLevel)ControlObject).LevelType == LevelTypes.Regular && CanDeleteRegularLevel())) && IsEditable();
        }

        /// <summary>
        /// ѕровер€ет можно ли удал€ть уровень Regular.
        /// </summary>
        /// <returns>true, если уровней Regular больше одного, false в противном случае.</returns>
        private bool CanDeleteRegularLevel()
        {
            int regularLevelCount = 0;
            foreach (IDimensionLevel level in ((IDimensionLevel)ControlObject).Parent.Levels.Values)
            {
                if (level.LevelType == LevelTypes.Regular)
                {
                    regularLevelCount++;
                }
            }
            return regularLevelCount > 1;
        }

        [MenuAction("”далить", Images.Remove, CheckMenuItemEnabling = "CanDelete")]
        public void Delete()
        {
            HierarchyControl levelList = (HierarchyControl)Parent;

            SmoDictionaryBaseDesign<string, IDimensionLevel> smoCollection = (SmoDictionaryBaseDesign<string, IDimensionLevel>)levelList.ControlObject;
            IDictionaryBase<string, IDimensionLevel> collection = (IDictionaryBase<string, IDimensionLevel>)smoCollection.ServerControl;

            if (MessageBox.Show(String.Format("¬ы действительно хотите удалить уровень иерархии \r {0} ?", this.Caption), "”даление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                collection.Remove(((IDimensionLevel)this.ControlObject).ObjectKey);
                OnChange(this, new EventArgs());
                this.Remove();
            }
        }
    }
}
