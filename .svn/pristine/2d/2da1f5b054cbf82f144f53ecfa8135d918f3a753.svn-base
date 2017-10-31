using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.Common.Forms;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    internal class ModificationItemProcessControl : CustomTreeNodeControl<IModificationItem>
    {
        public ModificationItemProcessControl(string key, string name, IModificationItem controlObject, CustomTreeNodeControl parent)
            : base(key, name, controlObject, parent, controlObject.ImageIndex)
        {
        }

        /// <summary>
        /// Обновляет только иконки узла
        /// </summary>
        protected override void RefreshImages()
        {
            LeftImages.Clear();
            Images stateImageIndex = Images.SuffixLock;
            switch (TypedControlObject.State)
            {
                case ModificationStates.NotApplied: stateImageIndex = Images.SuffixCheck1; break;
                case ModificationStates.Applied: stateImageIndex = Images.SuffixCheck2; break;
                case ModificationStates.AppliedPartially: stateImageIndex = Images.SuffixCheck21; break;
                case ModificationStates.AppliedWithErrors: stateImageIndex = Images.SuffixDelete; break;
            }
            LeftImages.Add(SchemeEditor.ImagesList[stateImageIndex]);
            LeftImages.Add(SchemeEditor.ImagesList[(Images)imageIndex]);
        }

        public bool ShowExceptionEnabled()
        {
            return ( TypedControlObject.State == ModificationStates.AppliedWithErrors);
        }

        [MenuAction("Текст ошибки", Images.File, CheckMenuItemEnabling = "ShowExceptionEnabled")]
        public void ShowException()
        {
            FormException.ShowErrorForm(TypedControlObject.Exception);
        }

        public override void Refresh()
        {
            RefreshImages();
        }
    }
}
