using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;
using Krista.FM.Client.Design;


namespace Krista.FM.Client.SchemeEditor.Commands
{
    /// <summary>
    /// ��������� �������� ������ ������
    /// </summary>
    class NewEntityCommand : INewEntityCommand
    {
        /// <summary>
        /// �������� ����
        /// </summary>
        ISchemeEditor schemeEditor;

        /// <summary>
        /// ��������� ������� �������� ������ ������
        /// </summary>
        /// <param name="schemeEditor"></param>
        public NewEntityCommand(ISchemeEditor schemeEditor)
        {
            this.schemeEditor = schemeEditor;
        }

        #region ���������� ���������� INewEntityCommand

        /// <summary>
        /// ������� ����� �����
        /// </summary>
        /// <returns></returns>
        public IEntity Execute(ClassTypes classTypes)
        {
            DiargamEditor.DiargamEditorForm deForm = ((SchemeEditor)schemeEditor).ActiveDiargamEditorForm as DiargamEditor.DiargamEditorForm;
            if (deForm != null)
            {
                IPackage package = deForm.DiargamEditor.Diagram.Document.ParentPackage;
                switch (classTypes)
                {
                    case ClassTypes.clsBridgeClassifier:
                        return package.Classes.CreateItem(ClassTypes.clsBridgeClassifier, SubClassTypes.Regular);

                    case ClassTypes.clsDataClassifier:
                        return package.Classes.CreateItem(ClassTypes.clsDataClassifier, SubClassTypes.Input);

                    case ClassTypes.clsFactData:
                        return package.Classes.CreateItem(ClassTypes.clsFactData, SubClassTypes.Input);

                    case ClassTypes.clsFixedClassifier:
                        return package.Classes.CreateItem(ClassTypes.clsFixedClassifier, SubClassTypes.Regular);

                    case ClassTypes.Table:
                        return package.Classes.CreateItem(ClassTypes.Table, SubClassTypes.Regular);
					
					case ClassTypes.DocumentEntity:
						return package.Classes.CreateItem(ClassTypes.DocumentEntity, SubClassTypes.Regular);

                    default:
                        throw new Exception("������������ ��� ������.");
                }
            }
            return null;
        }

        #endregion

    
        #region ICommand Members

        void  ICommand.Execute()
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        #endregion
	}
}
