using System;
using System.Windows.Forms;

using Infragistics.Win.IGControls;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class MenuActionAttribute : Attribute
    {
        private readonly string caption;
        private int imageIndex;
        private string checkMenuItemEnabling;
        private bool showByCtrlKey;

        public MenuActionAttribute(string caption)
        {
            this.caption = caption;
        }

        public MenuActionAttribute(string caption, Images image)
            : this(caption)
        {
            this.imageIndex = (int)image;
        }

        public string Caption
        {
            get { return caption; }
        }

        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }

        public string CheckMenuItemEnabling
        {
            get { return checkMenuItemEnabling; }
            set { checkMenuItemEnabling = value; }
        }

        /// <summary>
        /// Показывать пункт меню, если нажата клавиша Ctrl.
        /// </summary>
        public bool ShowByCtrlKey 
        {
            get { return showByCtrlKey; }
            set { showByCtrlKey = value; }
        }
    }


    public class CustomIGMenuItem : IGMenuItem
    {
        private string checkMenuItemEnabling;

        public CustomIGMenuItem(string text, EventHandler onClick)
            : base(text, onClick)
        {
        }

        public string CheckMenuItemEnabling
        {
            get { return checkMenuItemEnabling; }
            set { checkMenuItemEnabling = value; }
        }
    }


    /// <summary>
    /// Базовый абстрактный класс для всех типов узлов дерева
    /// </summary>
    public abstract class CustomTreeNodeControl : UltraTreeNode
    {
        /// <summary>
        /// Хранимое наименование узла задаваемое при создании
        /// </summary>
        private readonly string caption = String.Empty;

        protected int imageIndex = 0;

        public int ImageIndex
        {
            get { return imageIndex; }
        }
        

        /// <summary>
        /// Базовый абстрактный класс для всех типов узлов дерева
        /// </summary>
        /// <param name="name">Уникальный ключ элемента в дереве</param>
        /// <param name="text">Наименование узла видимое в интерфейсе</param>
        /// <param name="parent">Родительский узел</param>
        public CustomTreeNodeControl(string name, string text, CustomTreeNodeControl parent, int imageIndex)
            : base(name)
        {
            this.caption = text;
            this.imageIndex = imageIndex;
            
            if (parent == null)
                this.Key = name;
            else
                this.Key = String.Format("{0}{1}{2}", parent.Key, CustomTreeView.CustomPathSeparator, name);
        }

        public virtual object ControlObject
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Хранимое наименование узла задаваемое при создании
        /// </summary>
        public virtual string Caption
        {
            get { return caption; }
        }

        /// <summary>
        /// Редактор схем
        /// </summary>
        public SchemeEditor SchemeEditor
        {
            get 
            {
                return SchemeEditor.Instance/*((CustomTreeView)this.Control).SchemeEditor*/;
            }
        }

        /// <summary>
        /// Возвращает наименование узла садаваемое при создании элемента
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCaption()
        {
            return Caption;
        }

        #region Test

        protected override void OnObjectPropChanged(Infragistics.Shared.PropChangeInfo propChange)
        {
            base.OnObjectPropChanged(propChange);
        }

        protected override void OnSubObjectPropChanged(Infragistics.Shared.PropChangeInfo propChangeInfo)
        {
            base.OnSubObjectPropChanged(propChangeInfo);
        }

        #endregion Test

        public virtual void OnBeforeExpand(CancelableNodeEventArgs e)
        {
        }

        public virtual void OnAfterExpand(NodeEventArgs e)
        {
        }

        public virtual void OnBeforeSelect(BeforeSelectEventArgs e)
        {
        }

        public virtual void OnAfterSelect(SelectEventArgs e)
        {
            if (SchemeEditor != null)
                SchemeEditor.SetSlectedObject(this.Key);
        }

        public virtual void OnDoubleClick(NodeEventArgs e)
        {
        }

        public virtual void OnChange(object sender, EventArgs e)
        {
            if (Parent != null)
                ((CustomTreeNodeControl)Parent).OnChange(sender, e);
            RefreshCaption();
        }

        /// <summary>
        /// Доступность объекта для перетаскивания мышой
        /// </summary>
        public virtual bool Draggable
        {
            get { return false; }
        }

        /// <summary>
        /// Событие происходит, когда пользователь помещает указатель мыши на элемент управления при перетаскивании объекта с помощью мыши
        /// </summary>
        public virtual void OnDragOver(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            e.Effect = DragDropEffects.None;
            dropHightLightDrawFilter.ClearDropHighlight();
        }

        /// <summary>
        /// This event is fired by the DrawFilter to let us determine
        /// what kinds of drops we want to allow on any particular node
        /// </summary>
        public virtual void QueryStateAllowedForNode(ObjectsTreeViewDropHightLightDrawFilter.QueryStateAllowedForNodeEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            e.StatesAllowed = DropLinePositionEnum.None;
        }

        /// <summary>
        /// Событие происходит, когда пользователь завершает операцию перетаскивания объекта на элемент управления, высвобождая объект при отпускании кнопки мыши
        /// </summary>
        public virtual void OnDragDrop(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            dropHightLightDrawFilter.ClearDropHighlight();
        }



        /// <summary>
        /// Добовляет дочерние элементы
        /// </summary>
        protected virtual void ExpandNode()
        {
        }

        /// <summary>
        /// Заполняет контекстное меню элементами.
        /// </summary>
        /// <param name="contextMenu">контекстное меню которое необходимо заполнить.</param>
        private void InitializeContextMenu(IGContextMenu contextMenu)
        {
            contextMenu.MenuItems.Clear();
            contextMenu.Popup += contextMenu_Popup;

            Type t = this.GetType();
            string currentDeclaringType = String.Empty;
            bool needSeparator = false;
            foreach (System.Reflection.MethodInfo methodInfo in t.GetMethods())
            {
                if (String.IsNullOrEmpty(currentDeclaringType))
                    currentDeclaringType = methodInfo.DeclaringType.FullName;

                needSeparator = currentDeclaringType != methodInfo.DeclaringType.FullName;

                if (methodInfo.DeclaringType.FullName == "System.Windows.Forms.TreeNode")
                    break;

                foreach (object customAttribute in methodInfo.GetCustomAttributes(typeof(MenuActionAttribute), true))
                {
                    MenuActionAttribute attr = (MenuActionAttribute)customAttribute;
                    if (attr.ShowByCtrlKey && ((System.Windows.Forms.Control.ModifierKeys & Keys.Control) != Keys.Control))
                    {
                        break;
                    }

                    string caption = attr.Caption;
                    if (String.IsNullOrEmpty(caption))
                        caption = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;

                    if (caption == "-")
                    {
                        contextMenu.MenuItems.Add("-");
                    }
                    else
                    {
                        CustomIGMenuItem menuItem = new CustomIGMenuItem(caption, new EventHandler(this.OnContextMenuItemClick));
                        menuItem.Name = methodInfo.Name;
                        menuItem.Image = SchemeEditor.ImagesList[(Images)attr.ImageIndex];
                        menuItem.CheckMenuItemEnabling = attr.CheckMenuItemEnabling;
                        menuItem.Tag = this;

                        contextMenu.MenuItems.Add(menuItem);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Возвращает контекстное меню элемента
        /// </summary>
        /// <returns>Контекстное меню элемента</returns>
        internal IGContextMenu GetContextMenu()
        {
            IGContextMenu contextMenu = new IGContextMenu();
            InitializeContextMenu(contextMenu);
            return contextMenu;
        }

        /// <summary>
        /// Обработчик выпадения контекстного меню. Устанавливает доступность пунктов меню в зависимости от вычисляемых условий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void contextMenu_Popup(object sender, EventArgs e)
        {
            foreach (CustomIGMenuItem item in ((IGContextMenu)(sender)).MenuItems)
            {
                if (item is CustomIGMenuItem)
                {
                    if (!String.IsNullOrEmpty(item.CheckMenuItemEnabling))
                    {
                        try
                        {
                            CustomTreeNodeControl node = (CustomTreeNodeControl)item.Tag;
                            object result = node.GetType().InvokeMember(item.CheckMenuItemEnabling, System.Reflection.BindingFlags.InvokeMethod, null, node, null);
                            item.Enabled = Convert.ToBoolean(result);
                            //item.ToolTipText = ((CustomIGMenuItem)item).CheckMenuItemEnabling;
                        }
                        catch /*(Exception ex)*/
                        {
                            item.Enabled = false;
                            //item.ToolTipText = ex.Message;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие выбора элемента меню
        /// </summary>
        private void OnContextMenuItemClick(object sender, EventArgs e)
        {
            IGMenuItem menuItem = sender as IGMenuItem;

            CustomTreeNodeControl node = menuItem != null ? menuItem.Tag as CustomTreeNodeControl : null;
            if (node != null)
            {
                try
                {
                    node.GetType().InvokeMember(menuItem.Name, System.Reflection.BindingFlags.InvokeMethod, null, node, null);
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp.InnerException);
                }
            }
        }

        /// <summary>
        /// Обновляет только наименование узла
        /// </summary>
        public virtual void RefreshCaption()
        {
            Text = GetCaption();
        }

        /// <summary>
        /// Обновляет только иконки узла
        /// </summary>
        protected virtual void RefreshImages()
        {
        }

        /// <summary>
        /// Обновляет наименования дочерних узлов
        /// </summary>
        public virtual void RefreshCaptionForChildren()
        {
            foreach (CustomTreeNodeControl node in Nodes)
            {
                node.RefreshCaption();
                node.RefreshCaptionForChildren();
            }
        }

        [MenuAction("Обновить")]
        public virtual void Refresh()
        {
            Nodes.Clear();
            ExpandNode();
            //!!((Krista.FM.Client.ViewObjects.AdminConsole.Navigation)this.Control.Parent.Parent.Parent.Parent).Console.ReloadData();
        }
    }


    /// <summary>
    /// Базовый абстрактный класс для определенных типов узлов дерева
    /// </summary>
    public abstract class CustomTreeNodeControl<TIntf> : CustomTreeNodeControl
    {
        /// <summary>
        /// Объект привязанный к данному узлу
        /// </summary>
        private TIntf controlObject;
    
        /// <summary>
        /// Базовый абстрактный класс для определенных типов узлов дерева
        /// </summary>
        /// <param name="name">Уникальный ключ элемента в дереве</param>
        /// <param name="text">Наименование узла видимое в интерфейсе</param>
        /// <param name="controlObject">Объект который нужно привязать к данному узлу</param>
        /// <param name="parent">Родительский узел</param>
        public CustomTreeNodeControl(string name, string text, TIntf controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(name, text, parent, imageIndex)
           // : this(name, text, controlObject, parent, imageIndex, false)
        {
            this.controlObject = controlObject;
         //   this.isLeaf = isLeaf;
            Text = GetCaption();

            RefreshImages();

            if (this.ControlObject is ServerManagedObjectAbstract)
                ((ServerManagedObjectAbstract)this.ControlObject).OnChange += OnChange;
        }

        public static string LoadNodeName
        {
            get { return "Загрузка..."; }
        }

        public static CustomTreeNodeControl LoadNode
        {
            get
            {
                return new SampleControl(LoadNodeName, LoadNodeName, null, null, 0);
            }
        }

        public bool IsLockedByCurrentUser()
        {
            return ((IServerSideObject)ControlObject).LockedByUserID == Krista.FM.Common.ClientAuthentication.UserID;
        }

        public bool IsEditable()
        {
            return ((IServerSideObject)ControlObject).IsClone || IsLockedByCurrentUser() || ((IServerSideObject)ControlObject).State == ServerSideObjectStates.New;
        }

        protected static string AddStateModificators(TIntf obj, string _caption)
        {
            /*IServerSideObject sso = obj as IServerSideObject;
            if (sso != null)
            {
                if (sso.IsClone || sso.LockedByUserID == Krista.FM.Common.ClientAuthentication.UserID)
                    _caption = String.Format("[Edit] {0}", _caption);
                else if (sso.IsLocked)
                    _caption = String.Format("[Locked] {0}", _caption);
            }*/
            return _caption;
        }

        protected void AddLoadNode()
        {
            CustomTreeNodeControl loadNode = LoadNode;
            loadNode.Key = String.Format("{0}{1}{2}", this.Key, CustomTreeView.CustomPathSeparator, loadNode.Key);
            Nodes.Add(loadNode);
        }

        public override void OnBeforeExpand(CancelableNodeEventArgs e)
        {
            base.OnBeforeExpand(e);

            if (Nodes.Exists(String.Format("{0}{1}{2}", Key, CustomTreeView.CustomPathSeparator, LoadNodeName)))
            {
                Nodes.Clear();
                ExpandNode();
            }
        }

/*        protected override string GetCaption()
        {
            if (controlObject is IServerSideObject)
            {
                ServerSideObjectStates state = ((IServerSideObject)controlObject).State;
                if (state != ServerSideObjectStates.Consistent)
                {
                    return String.Format("[{0}] {1}", state.ToString(), Caption);
                }
                else
                    return Caption;
            }
            else
                return Caption;
        }*/

        /// <summary>
        /// Обновляет только наименование узла
        /// </summary>
        public override void RefreshCaption()
        {
            base.RefreshCaption();

            RefreshImages();
        }

        /// <summary>
        /// Обновляет только иконки узла
        /// </summary>
        protected override void RefreshImages()
        {
            LeftImages.Clear();
            IServerSideObject sso = this.controlObject as IServerSideObject;
            Images stateImageIndex = Images.SuffixLock;
            if (sso != null)
            {
                if (sso.IsClone || sso.LockedByUserID == Krista.FM.Common.ClientAuthentication.UserID || (sso.IsClone && sso is System.Collections.IEnumerable || sso.State == ServerSideObjectStates.New))
                    if (sso.State == ServerSideObjectStates.New)
                        stateImageIndex = Images.SuffixPlus;
                    else
                        stateImageIndex = Images.SuffixCheck1;
                else if (sso.IsLocked)
                    stateImageIndex = Images.SuffixUser;
            }
            if (stateImageIndex == Images.SuffixLock)
            {
                if (this.Control != null)
                    this.Control.ImageTransparentColor = System.Drawing.Color.FromArgb(0, 255, 0);
                LeftImages.Add(Krista.FM.Client.SchemeEditor.Properties.Resource.SuffixLock);
            }
            else
                LeftImages.Add(SchemeEditor.ImagesList[stateImageIndex]);
            LeftImages.Add(SchemeEditor.ImagesList[(Images)imageIndex]);
        }

        public override object ControlObject
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return controlObject; }
            set { controlObject = (TIntf)value; }
        }

        public TIntf TypedControlObject
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return controlObject; }
            set { controlObject = value; }
        }
        
        [MenuAction("Обновить", Images.Refresh)]
        public override void Refresh()
        {
            RefreshCaption();
            base.Refresh();
        }

		public override object Tag
		{
			get
			{
				return controlObject;
			}
		}
    }
}
