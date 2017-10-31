using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class ClassListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IEntity>, IEntity>
    {
        public ClassListControl(IEntityCollection<IEntity> controlObject, CustomTreeNodeControl parent)
            : base("IEntityCollection", "Классы", 
            new SmoDictionaryBaseDesign<string, IEntity>(controlObject), parent, 15)
        {
        }

        public override CustomTreeNodeControl Create(IEntity item)
        {
            SmoEntityDesign node;
            ClassTypes ct = item.ClassType;
            SubClassTypes sct = item.SubClassType;
            if (ct == ClassTypes.clsFactData)
                node = new SmoFactTableDesign(item);
            else if (ct == ClassTypes.Table)
                node = new SmoEntityDesign(item);
            else if (sct == SubClassTypes.System)
                node = new SmoEntityDesign(item);
			else if (ct == ClassTypes.DocumentEntity)
                node = new SmoEntityDesign(item);
			else
                node = new SmoClassifierDesign((IClassifier)item);

            return new ClassControl(node, this);
        }

        [MenuAction("Создать фиксированный классификатор", Images.ClassViolet, CheckMenuItemEnabling = "IsEditable")]
        public void AddNewFixedTable()
        {
            SmoDictionaryBaseDesign<string, IEntity> smoCollection = (SmoDictionaryBaseDesign<string, IEntity>)ControlObject;
            IEntityCollection<IEntity> collection = (IEntityCollection<IEntity>)smoCollection.ServerControl;
            IEntity newObj = collection.CreateItem(ClassTypes.clsFixedClassifier, SubClassTypes.Regular);
            CustomTreeNodeControl node = Create(newObj);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            this.SelectNewObject(node);
        }

        [MenuAction("Создать сопоставимый классификатор", Images.ClassGreen, CheckMenuItemEnabling = "IsEditable")]
        public void AddNewBridgeTable()
        {
            SmoDictionaryBaseDesign<string, IEntity> smoCollection = (SmoDictionaryBaseDesign<string, IEntity>)ControlObject;
            IEntityCollection<IEntity> collection = (IEntityCollection<IEntity>)smoCollection.ServerControl;
            IEntity newObj = collection.CreateItem(ClassTypes.clsBridgeClassifier, SubClassTypes.Regular);
            CustomTreeNodeControl node = Create(newObj);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            this.SelectNewObject(node);
        }

        [MenuAction("Создать классификатор данных", Images.ClassYellow, CheckMenuItemEnabling = "IsEditable")]
        public override void AddNew()
        {
            SmoDictionaryBaseDesign<string, IEntity> smoCollection = (SmoDictionaryBaseDesign<string, IEntity>)ControlObject;
            IEntityCollection<IEntity> collection = (IEntityCollection<IEntity>)smoCollection.ServerControl;
            IEntity newObj = collection.CreateItem(ClassTypes.clsDataClassifier, SubClassTypes.Input);
            CustomTreeNodeControl node = Create(newObj);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            this.SelectNewObject(node);
        }

        [MenuAction("Создать таблицу фактов", Images.ClassBlue, CheckMenuItemEnabling = "IsEditable")]
        public void AddNewFactTable()
        {
            SmoDictionaryBaseDesign<string, IEntity> smoCollection = (SmoDictionaryBaseDesign<string, IEntity>)ControlObject;
            IEntityCollection<IEntity> collection = (IEntityCollection<IEntity>)smoCollection.ServerControl;
            IEntity newObj = collection.CreateItem(ClassTypes.clsFactData, SubClassTypes.Input);
            CustomTreeNodeControl node = new ClassControl(new SmoFactTableDesign(newObj), this);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            SelectNewObject(node);
        }

        [MenuAction("Создать таблицу", Images.Class, CheckMenuItemEnabling = "IsEditable")]
        public void AddNewTable()
        {
            SmoDictionaryBaseDesign<string, IEntity> smoCollection = (SmoDictionaryBaseDesign<string, IEntity>)ControlObject;
            IEntityCollection<IEntity> collection = (IEntityCollection<IEntity>)smoCollection.ServerControl;
            IEntity newObj = collection.CreateItem(ClassTypes.Table, SubClassTypes.Regular);
            CustomTreeNodeControl node = new ClassControl(new SmoEntityDesign(newObj), this);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            SelectNewObject(node);
        }

		[MenuAction("Создать табличный документ", Images.Class, CheckMenuItemEnabling = "IsEditable")]
		public void AddNewDocumentEntity()
		{
			SmoDictionaryBaseDesign<string, IEntity> smoCollection = (SmoDictionaryBaseDesign<string, IEntity>)ControlObject;
			IEntityCollection<IEntity> collection = (IEntityCollection<IEntity>)smoCollection.ServerControl;
			IEntity newObj = collection.CreateItem(ClassTypes.DocumentEntity, SubClassTypes.Regular);
			CustomTreeNodeControl node = new ClassControl(new SmoEntityDesign(newObj), this);
			Nodes.Add(node);
			OnChange(this, new EventArgs());

			SelectNewObject(node);
		}

		[Obsolete()]
        protected override void ExpandNode()
        {
            StartExpand();

            base.ExpandNode();

            EndExpand();
        }
    }
}
