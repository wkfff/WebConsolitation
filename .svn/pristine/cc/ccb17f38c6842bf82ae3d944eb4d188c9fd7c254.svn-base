using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.Design;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.Commands
{
    /// <summary>
    /// Команда открытия документа
    /// </summary>
    public class OpenDocumentCommand : IOpenDocumentCommand
    {
        /// <summary>
        /// Редактор схем
        /// </summary>
        private ISchemeEditor schemeEditor;


        /// <summary>
        /// Команда открытия документа
        /// </summary>
        /// <param name="schemeEditor">Редактор схем</param>
        public OpenDocumentCommand(ISchemeEditor schemeEditor)
        {
            this.schemeEditor = schemeEditor;
        }

        #region IOpenDocumentCommand Members

        public void Execute(IDocument document)
        {
            // TODO schemeEditor.OpenDocument(document);
        }

        #endregion

        #region ICommand Members

        public void Execute()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
