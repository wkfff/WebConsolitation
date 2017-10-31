using System;
using System.Windows.Forms;

using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class ReferenceClassControl : MajorObjectControl<SmoEntityDesign>
    {

        public ReferenceClassControl(SmoEntityDesign controlObject, CustomTreeNodeControl parent)
            : base(controlObject, parent, GetImageIndex(controlObject))
        {
            AddLoadNode();
        }

        private static int GetImageIndex(SmoEntityDesign controlObject)
        {
            int imageIndex = (int)Images.Class;
            switch (controlObject.ClassType)
            {
                case ClassTypes.clsFactData: imageIndex = (int)Images.ClassBlue; break;
                case ClassTypes.clsDataClassifier: imageIndex = (int)Images.ClassYellow; break;
                case ClassTypes.clsBridgeClassifier: imageIndex = (int)Images.ClassGreen; break;
                case ClassTypes.clsFixedClassifier: imageIndex = (int)Images.ClassViolet; break;
            }
            return imageIndex;
        }
    
        public bool CanDelete()
        {
            return ((IPackage)((PackageControl)Parent.Parent).ControlObject).Name != "Системные объекты" && IsEditable();
        }

        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "CanDelete")]
        public void Delete()
        {
            ClassListControl classList = (ClassListControl)Parent;

            SmoDictionaryBaseDesign<string, IEntity> smoCollection = (SmoDictionaryBaseDesign<string, IEntity>)classList.ControlObject;
            IModifiableCollection<string, IEntity> collection = (IModifiableCollection<string, IEntity>)smoCollection.ServerControl;

            //Запрос на удаление
            if (MessageBox.Show(String.Format("Вы действительно хотите удалить класс \r {0} ?", this.Caption), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                collection.Remove(((IEntity)this.ControlObject).ObjectKey);
                OnChange(this, new EventArgs());
                this.Remove();
            }
        }

        public bool CanShowFixedRows()
        {
            return ControlObject is IClassifier;
        }

        [MenuAction("Фиксированные строки", Images.FixedString, CheckMenuItemEnabling = "CanShowFixedRows")]
        public void ShowFixedRows()
        {
            FixedRowsGridForm form = new FixedRowsGridForm();
            form.Classifier = (IClassifier)ControlObject;
            SchemeEditor.ShowDialog(form);
        }

        [MenuAction("Хеш-таблица", Images.FixedString, CheckMenuItemEnabling = "CanShowFixedRows")]
        public void ShowHashTable()
        {
            Krista.FM.Client.SchemeEditor.Test.HashCode.ClassifierHashCode.ShowHashTable((IClassifier)ControlObject);
        }

        [MenuAction("Показать данные", Images.FixedString)]
        public void ShowEntityTable()
        {
            SchemeEditor.OpenObject((ICommonDBObject)ControlObject);
        }

        [MenuAction("Перейти к объекту", Images.GoToObject)]
        public void GoToObject()
        {
            SchemeEditor.SelectObject(((IEntity)ControlObject).Key, false);
        }
    }
}
