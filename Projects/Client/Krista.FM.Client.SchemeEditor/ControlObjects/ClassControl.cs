using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.SchemeEditor.Commands;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class ClassControl : MajorObjectControl<SmoEntityDesign>
    {

        public ClassControl(SmoEntityDesign controlObject, CustomTreeNodeControl parent)
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

        protected override void ExpandNode()
        {
            base.ExpandNode();

            IEntity entity = (IEntity)ControlObject;

            AttributeListControl attributeListControl = new AttributeListControl(entity.Attributes, this);
            Nodes.Add(attributeListControl);

            PresentationListControl presentations = new PresentationListControl(entity.Presentations, this);
            if (presentations.Nodes.Count != 0)
            {
                Nodes.Add(presentations);
            }

            attributeListControl.Expanded = true;

            if (entity is SmoClassifierDesign)
            {
                Nodes.Add(new HierarchyControl(((IClassifier)entity).Levels, this));
            }

            Nodes.Add(new ReferenceAssociationListControl(entity.Associations, this));
            Nodes.Add(new ReferencedByAssociationListControl(entity.Associated, this));

            //добавление ветки со списком уникальных ключей
            if ( entity.UniqueKeyAvailable )
            {Nodes.Add(new UniqueKeyListControl(entity.UniqueKeys, this));}
            
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
            if (MessageBox.Show(String.Format("Вы действительно хотите удалить класс \r {0} ?", Caption), @"Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                collection.Remove(((IEntity)ControlObject).ObjectKey);
                OnChange(this, new EventArgs());
                Remove();
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
            Client.SchemeEditor.Test.HashCode.ClassifierHashCode.ShowHashTable((IClassifier)ControlObject);
        }

        [MenuAction("Показать данные", Images.FixedString)]
        public void ShowEntityTable()
        {
            SchemeEditor.OpenObject((ICommonDBObject)ControlObject);
        }

        [MenuAction("Сгенерировать C# код")]
        public void GenerateCode()
        {
            List<IEntity> entities = new List<IEntity>();
            foreach (var selectedNode in Control.SelectedNodes)
            {
                var entity = selectedNode as ClassControl;
                if (entity != null)
                {
                    entities.Add(entity.TypedControlObject);
                }
            } 
            new GenerateCodeCommand().Execute(entities);
        }


        public override bool Draggable
        {
            get
            {
                return true;
            }
        }
    }
}
