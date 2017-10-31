using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor
{
    class Attribute
    {
        protected string name = String.Empty;

        protected TextFormatFlags flag = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis;

        #region Constructor
        public Attribute(string name)
        {
            this.name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// наименование аттрибута
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
       
        #endregion

        #region Methods
        /// <summary>
        /// Отрисовка атрибута
        /// </summary>
        /// <param name="g"></param>
        /// <param name="point"></param>
        public void Draw(Graphics g, int y, Rectangle bottom, DiagramEntity association, bool isVisible)
        {
            if (y + association.Font.Height > bottom.Bottom)
                return;
            else if (y + 2 * association.Font.Height + 4 > bottom.Bottom)
            {
                if (isVisible)
                    g.DrawIcon(Resource.privateAttribute, bottom.Left + 2, y);

                TextRenderer.DrawText(g, "...", association.Font, new Rectangle(bottom.Left + 17, y, bottom.Width - 17, association.Font.Height + 2), SystemColors.MenuText, flag);
            }
            else
            {
                if (isVisible)
                    g.DrawIcon(Resource.privateAttribute, bottom.Left + 2, y);

                TextRenderer.DrawText(g, this.Name, association.Font, new Rectangle(bottom.Left + 17, y, bottom.Width - 17, association.Font.Height + 2), SystemColors.MenuText, flag);
            }
        }
        #endregion
    }
}
