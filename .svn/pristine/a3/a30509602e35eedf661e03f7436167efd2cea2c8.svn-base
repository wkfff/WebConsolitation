using System.Drawing;
using System.Drawing.Imaging;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandPrintPreview : DiagramEditorCommand
    {
        private Tools.PrintForm printForm;

        public CommandPrintPreview(AbstractDiagram diagram)
            : base(diagram)
        {
            printForm = new Krista.FM.Client.DiagramEditor.Tools.PrintForm(Diagram.Site);

            this.Image = Diagram.Site.ImageList[Images.imgPrintPreview];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        #region Command Members

        /// <summary>
        /// Провоцируем печать
        /// </summary>
        public override void Execute()
        {
            printForm.Instance();
            InitializeMetafile();
            printForm.Ppd.ShowDialog();
        }

        public void InitializeMetafile()
        {
            printForm.IsOnePage = false;

            Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

            Metafile mf = Diagram.Site.CmdHelper.GetMetafile(scrollOffset, (float)Diagram.Site.ZoomFactor / 100);
            printForm.Print(mf);
        }

        #endregion
    }
}
