using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using Krista.FM.Client.SchemeEditor.Gui.ViewControls;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    /// <summary>
    /// Элемент управления пакетом
    /// </summary>
    public class PackageControl : MajorObjectControl<SmoPackageDesign>
    {
        /// <summary>
        /// Элемент управления пакетом
        /// </summary>
        /// <param name="controlObject">Управляемый объект</param>
        /// <param name="parent">Родительский узел элемента управления</param>
        public PackageControl(IPackage controlObject, CustomTreeNodeControl parent)
            : base(new SmoPackageDesign(controlObject), parent, (int)Images.Folder)
        {
            AddLoadNode();
        }

        protected override void ExpandNode()
        {
            base.ExpandNode();
            IPackage packege = (IPackage)ControlObject;
            PackageListControl packageListControl = new PackageListControl(packege.Packages, this);
            this.Nodes.Add(packageListControl);
            packageListControl.Expanded = true;
            if (packege.Name != "Корневой пакет")
            {
                this.Nodes.Add(new ClassListControl(packege.Classes, this));
                this.Nodes.Add(new PackageReferenceAssociationListControl(packege.Associations, this));
                this.Nodes.Add(new DocumentListControl(packege.Documents, this));
            }
        }

        public override string Caption
        {
            get 
            {
                if (((IPackage)ControlObject).FullName == "Корневой пакет")
                    return SchemeEditor.Scheme.Name;
                else
                    return ((IPackage)ControlObject).FullName; 
            }
        }

        public override void OnChange(object sender, EventArgs e)
        {
            if (TypedControlObject.IsEndPoint)
            {
                RefreshCaption();
                RefreshCaptionForChildren();
            }
            else
            {
                if (Parent != null)
                    ((CustomTreeNodeControl)Parent).OnChange(sender, e);
                Refresh();
            }
        }

        public bool CanDelete()
        {
            string name = ((IPackage)ControlObject).Name;
            return name != "Системные объекты" && name != "Корневой пакет" &&
                    ((IPackage)ControlObject).ParentPackage.LockedByUserID == Krista.FM.Common.ClientAuthentication.UserID;
        }

        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "CanDelete")]
        public void Delete()
        {
            PackageListControl packageList = (PackageListControl)Parent;

            SmoDictionaryBaseDesign<string, IPackage> smoCollection = (SmoDictionaryBaseDesign<string, IPackage>)packageList.ControlObject;
            IModifiableCollection<string, IPackage> collection = (IModifiableCollection<string, IPackage>)smoCollection.ServerControl;

            if (MessageBox.Show(String.Format("Вы действительно хотите удалить пакет \r {0}?", this.Caption), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                collection.Remove(((IPackage)this.ControlObject).ObjectKey);
                OnChange(this, new EventArgs());
                ((CustomTreeNodeControl)Parent).OnChange(this, new EventArgs());
                this.Remove();
            }
        }

        public bool CanApplayModifications()
        {
            return IsLockedByCurrentUser() && ((ICommonDBObject)ControlObject).IsEndPoint;
        }

        [MenuAction("Показать изменения", Images.ViewChange, CheckMenuItemEnabling = "CanApplayModifications")]
        public void ShowModificationsTree()
        {
            // TODO Показать окно с изменениями
            //SchemeEditor.SetModificationsContext(ControlObject as ICommonDBObject);
        }

        [MenuAction("Применить изменения", Images.ApplayChange, CheckMenuItemEnabling = "CanApplayModifications")]
        public void ApplayModifications()
        {
            // Если в пакете есть ошибки, то предепреждает об этом пользователя
            DataTable dt = ((IPackage)ControlObject).Validate();
            if (dt != null)
            {
                ValidationErrorListForm form = new ValidationErrorListForm(dt);
                DialogResult dr = SchemeEditor.Instance.ShowDialog(form);
                if (dr == DialogResult.OK)
                {
                    return;
                }
            }

            // Применяем изменения
            if (SchemeEditor.ApplayModifications((ICommonDBObject)ControlObject))
                this.Refresh();
        }

        public bool CanEditPackage()
        {
            return !((ICommonDBObject)ControlObject).IsLocked && ((ICommonDBObject)ControlObject).IsEndPoint;
        }

        [MenuAction("Редактировать", Images.Edit, CheckMenuItemEnabling = "CanEditPackage")]
        public void EditPackage()
        {
            ((IServerSideObject)ControlObject).Lock();
            this.Refresh();
        }

        [MenuAction("Проверить пакет", Images.ErrorReport)]
        public void Validate()
        {
            DataTable dt = ((IPackage)ControlObject).Validate();
            if (dt == null)
            {
                Services.MessageService.ShowMessage("Проверка пакета завершена. Ошибок не найдено.", "Проверка пакета");
            }
            else
            {
                // Показываем форму с информацией о проверке
                ValidationErrorListControl velControl = new ValidationErrorListControl(dt);
                Form form = new EmptyForm();
                form.Text = "Список ошибок";
                form.Icon = System.Drawing.Icon.FromHandle(Properties.Resource.ErrorReport.GetHicon());
                form.Controls.Add(velControl);
                velControl.Dock = DockStyle.Fill;
                form.MdiParent = SchemeEditor.Instance.Form;
                form.Show();
            }
        }

        [MenuAction("Сохранить в репозиторий", ShowByCtrlKey = true)]
        public void SaveToDisk()
        {
            ((IPackage)ControlObject).SaveToDisk();
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
