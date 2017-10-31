using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда сохранения в метафайл
    /// </summary>
    public class CommandSaveMetafile : DiagramEditorCommand
    {
        /// <summary>
        /// Базовый каталог для сохранения, если он не задан предлагаем диалоговое окно
        /// </summary>
        private string baseDirectory = String.Empty;

        public CommandSaveMetafile(AbstractDiagram diagram)
            : base(diagram)
        {
            Image = Diagram.Site.ImageList[Images.imgSaveAsMetafile];
            Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public string BaseDirectory
        {
            get { return baseDirectory; }
            set { baseDirectory = value; }
        }

        #region Command Members

        public override void Execute()
        {
            try
            {
                Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

                Graphics g = Diagram.Site.CreateGraphics();

                // возникают проблемы при скролинге
                IntPtr ptr = g.GetHdc();
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "EMF File(*.emf)|*.emf";
                string filename = string.Empty;
                if (Diagram.Site.Parent.Parent is Form)
                {
                    filename = Diagram.Site.Parent.Parent.Text;
                    if (filename[filename.Length - 1] == '*')
                    {
                        filename = filename.Substring(0, filename.Length - 1);
                    }

                    sfd.FileName = filename + ".emf";
                }

                char[] illegalCharacters = new[] { ':', '/', '\\', '|', '*', '<', '>', '?', '"' };
                for (int i = 0; i < illegalCharacters.Length; i++)
                {
                    if (filename.IndexOf(illegalCharacters[i]) > -1)
                    {
                        filename = filename.Replace(illegalCharacters[i], '_');
                    }
                }

                Metafile mf;
                if (!String.IsNullOrEmpty(baseDirectory))
                {
                    mf = new Metafile(String.Format(@"{0}\diagrams\{1}.emf", baseDirectory, filename), ptr);
                    if (true)
                    {
                        SavePait(ref scrollOffset, ref g, ptr, mf, false);
                    }
                }
                else
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        mf = new Metafile(sfd.FileName, ptr);
                        if (true)
                        {
                            SavePait(ref scrollOffset, ref g, ptr, mf, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        private void SavePait(ref Size scrollOffset, ref Graphics g, IntPtr ptr, Metafile mf, bool scale)
        {
            g.ReleaseHdc(ptr);
            g.Dispose();

            g = Graphics.FromImage(mf);

            if (scale)
            {
                g.ScaleTransform((float)Diagram.Site.ZoomFactor / 100, (float)Diagram.Site.ZoomFactor / 100);
            }
            else
            {
                g.ScaleTransform(1, 1);
            }

            foreach (DiagramEntity entity in Diagram.Entities)
            {
                entity.Draw(g, scrollOffset);
            }

            g.Dispose();
        }
        
        #endregion
    }
}
