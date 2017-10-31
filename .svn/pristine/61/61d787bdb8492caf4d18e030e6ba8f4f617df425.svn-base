using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.Commands
{
    /// <summary>
    /// Интерфейс команды создания нового пакета
    /// </summary>
    class NewPackageCommand : INewPackageCommand
    {
        /// <summary>
        /// Редактор схем
        /// </summary>
        ISchemeEditor schemeEditor;

        /// <summary>
        /// Интерфейс команды создания нового пакета
        /// </summary>
        public NewPackageCommand(ISchemeEditor schemeEditor)
        {
            this.schemeEditor = schemeEditor;
        }

        #region INewPackageCommand Members

        /// <summary>
        /// Создать новый пакет
        /// </summary>
        /// <returns></returns>
        public IPackage Execute()
        {
            DiargamEditor.DiargamEditorForm deForm = ((SchemeEditor)schemeEditor).ActiveDiargamEditorForm as DiargamEditor.DiargamEditorForm;
            if (deForm != null)
            {
                IPackage package = deForm.DiargamEditor.Diagram.Document.ParentPackage;
                return package.Packages.CreateItem();
            }
            return null;
        }

        #endregion

        #region ICommand Members

        void ICommand.Execute()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
