using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.Design;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.Commands
{
    /// <summary>
    /// Интерфейс команды создания ассоциации 
    /// </summary>
    public class NewAssociation : INewAssociationCommand
    {
        /// Редактор схем
        /// </summary>
        private ISchemeEditor schemeEditor;

        /// <summary>
        /// Интерфейс команды создания новой ассоциации
        /// </summary>
        /// <param name="schemeEditor">Редактор схем</param>
        public NewAssociation(ISchemeEditor schemeEditor)
        {
            this.schemeEditor = schemeEditor;
        }

        #region INewAssociationCommand Members

		public IEntityAssociation Execute(IEntity roleA, IEntity roleB, AssociationClassTypes type)
        {
            DiargamEditor.DiargamEditorForm deForm = ((SchemeEditor)schemeEditor).ActiveDiargamEditorForm as DiargamEditor.DiargamEditorForm;
            if (deForm != null)
            {
                IPackage package = deForm.DiargamEditor.Diagram.Document.ParentPackage;
                return package.Associations.CreateItem(roleA, roleB, type);
            }
            return null;
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
