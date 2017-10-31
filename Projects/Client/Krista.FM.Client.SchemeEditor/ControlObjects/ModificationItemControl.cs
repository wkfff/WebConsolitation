using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class ModificationItemControl : CustomListControl<IModificationItem, IModificationItem>
    {
        public ModificationItemControl(IModificationItem controlObject, CustomTreeNodeControl parent)
            : base(controlObject.Key, controlObject.Key, new SmoModificationItemDesign(controlObject), parent, controlObject.ImageIndex)
        {
        }

        public override void Dispose()
        {
            this.TypedControlObject.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Добовляет дочерние элементы
        /// </summary>
        protected override void ExpandNode()
        {
            foreach (IModificationItem item in ((IModificationItem)ControlObject).Items.Values)
            {
                ModificationItemControl itemControl = new ModificationItemControl(item, this);
                Nodes.Add(itemControl);
                Nodes[itemControl.Key].Text = item.Name; 
            }
        }

        /// <summary>
        /// Обновляет только иконки узла
        /// </summary>
        protected override void RefreshImages()
        {
            LeftImages.Clear();
            Images stateImageIndex = Images.SuffixLock;
            switch (TypedControlObject.Type)
            {
                case ModificationTypes.Create: stateImageIndex = Images.SuffixPlus; break;
                case ModificationTypes.Modify: stateImageIndex = Images.SuffixCheck3; break;
                case ModificationTypes.Remove: stateImageIndex = Images.SuffixMinus; break;
                case ModificationTypes.Inapplicable: stateImageIndex = Images.SuffixLock; break;
            }
            LeftImages.Add(SchemeEditor.ImagesList[stateImageIndex]);
            LeftImages.Add(SchemeEditor.ImagesList[(Images)imageIndex]);
        }

        public bool ExpandEnabled()
        {
            return !Expanded;
        }

        [MenuAction("Развернуть", CheckMenuItemEnabling = "ExpandEnabled")]
        public void ExpandAllChild()
        {
            ExpandAll(Infragistics.Win.UltraWinTree.ExpandAllType.Always);
        }

        public bool ApplayEnabled()
        {
            return !((ModificationTreeView)this.Control).ReadOnly && TypedControlObject.State == ModificationStates.NotApplied;
        }

        [MenuAction("Применить", Images.ApplayChange, CheckMenuItemEnabling = "ApplayEnabled")]
        public void Applay()
        {
            if (MessageBox.Show("Применить изменения?", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    SchemeEditor.Instance.StartModificationsApplayProcess(TypedControlObject, String.Empty);
                }
                finally
                {
                    Refresh();
                }
            }
        }
    }
}
