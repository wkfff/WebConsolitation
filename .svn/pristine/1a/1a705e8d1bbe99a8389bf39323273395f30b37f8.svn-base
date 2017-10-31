using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Krista.FM.Update.Framework.Forms
{
    public partial class PatchListForm : Form
    {
        public PatchListForm(IList<IUpdatePatch> patches, bool readOnlyMode, bool isServerMode, bool receivedUpdateMode)
        {
            CreatePatchListControl(receivedUpdateMode);

            InitializeComponent();

            patchListControl.Patches = patches;
            patchListControl.ReadOnlyMode = readOnlyMode;
            patchListControl.IsServerMode = isServerMode;
        }

        private void CreatePatchListControl(bool receivedUpdateMode)
        {
            patchListControl = receivedUpdateMode
                                        ? new Krista.FM.Update.Framework.Controls.ReceivedPatchListControl()
                                        : new Krista.FM.Update.Framework.Controls.PatchListControl();
        }

        /// <summary>
        /// Обработчик нажатия с клавиатуры
        /// </summary>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
                Close();

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
