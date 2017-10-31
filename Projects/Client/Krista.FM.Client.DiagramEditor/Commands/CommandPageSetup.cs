using System.Drawing;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandPageSetup : DiagramEditorCommand
    {
        public CommandPageSetup(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgPageSetupDialod];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            PageSetupDialog psd = new PageSetupDialog();

            // в диалоге настроек параметров единица измерения - мм, но в PageSettings похоже
            // что хранится в сотых долях дюйма, и свойство EnableMetric = true переводит обратно в мм, но 
            // с потерей точности :(((
            psd.EnableMetric = true;

            psd.PageSettings = Diagram.Site.DefaultSettings.DafaultPageSettings;

            if (psd.ShowDialog() == DialogResult.OK)
            {
                Diagram.Site.DefaultSettings.DafaultPageSettings = psd.PageSettings;
            }
        }
    }    
}
