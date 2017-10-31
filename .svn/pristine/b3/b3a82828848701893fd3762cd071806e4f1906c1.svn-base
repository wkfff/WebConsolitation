using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Печать диаграммы на одном листе
    /// </summary>
    public class CommandPrintOnePage : DiagramEditorCommand
    {
        private Tools.PrintForm printForm;

        public CommandPrintOnePage(AbstractDiagram diagram)
            : base(diagram)
        {
            printForm = new Krista.FM.Client.DiagramEditor.Tools.PrintForm(Diagram.Site);
            printForm.Command = this;

            this.Image = Diagram.Site.ImageList[Images.imgPrintOnPage];
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

        /// <summary>
        /// Получение метафайда и вывод его на печать
        /// </summary>
        public void InitializeMetafile()
        {
            // здесь печатаем на одной странице
            printForm.IsOnePage = true;

            printForm.Print(Diagram.Site.CmdHelper.GetMetafile(Size.Empty, 1.0f));
        }

        #endregion
    }
}
