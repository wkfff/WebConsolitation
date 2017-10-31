using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.SchemeEditor.Commands
{
    class SaveDiagramCommand : ICommand
    {
        DiagramEditor.DiargamEditor site;

        public SaveDiagramCommand(DiagramEditor.DiargamEditor site)
        {
            this.site = site;
        }

        #region ICommand Members

        public void Execute()
        {
            if (site != null)
                site.SaveDiagram();
        }

        #endregion
    }
}
