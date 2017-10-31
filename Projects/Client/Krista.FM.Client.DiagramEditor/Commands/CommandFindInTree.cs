using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда поиска в дереве объектов
    /// </summary>
    public class CommandFindInTree : Command
    {
        private DiagramEntity entity;

        public CommandFindInTree(DiagramEntity entity)
        {
            this.entity = entity;

            this.Image = entity.Diagram.Site.ImageList[Images.imgFindInTree];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            entity.Diagram.Site.SchemeEditor.SelectObject(entity.Key, false);
        }
    }
}
