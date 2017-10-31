using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Панель на которой распологается грид
    /// </summary>
    public class RichPanel : Panel
    {
        public RichPanel()
            : base()
        {
            //Выставили признаки которое спасают от мерцания при перерисовке грида
            this.SetStyle(ControlStyles.DoubleBuffer
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                //признак что панель может быть выбранна (выставялем для того таблица просто так, не отдавала 
                //фокус другим контролам расположенным на общем контроле ExpertGrid)
                | ControlStyles.Selectable, 
                true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
