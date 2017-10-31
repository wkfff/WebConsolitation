using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Globalization;
using System.Runtime.InteropServices;


namespace Krista.FM.Client.MDXExpert
{
    public class CustomCollectionEditor : UITypeEditor
    {
        // Fields
        private Type collectionItemType;
        private ITypeDescriptorContext currentContext;
        private bool ignoreChangedEvents;
        private bool ignoreChangingEvents;
        private Type[] newItemTypes;
        private Type type;
        private static MainForm mainForm;

        // Methods
        public CustomCollectionEditor(Type type)
        {
            this.type = type;
        }

        public virtual void CancelChanges()
        {
        }

        public virtual bool CanRemoveInstance(object value)
        {
            IComponent component = value as IComponent;
            if (component != null)
            {
                InheritanceAttribute attribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
                if ((attribute != null) && (attribute.InheritanceLevel != InheritanceLevel.NotInherited))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool CanSelectMultipleInstances()
        {
            return true;
        }

        protected virtual CustomCollectionForm CreateCollectionForm()
        {
            return new CustomCollectionEditorCollectionForm(this);
        }

        protected virtual Type CreateCollectionItemType()
        {
            PropertyInfo[] properties = TypeDescriptor.GetReflectionType(this.CollectionType).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name.Equals("Item") || properties[i].Name.Equals("Items"))
                {
                    return properties[i].PropertyType;
                }
            }
            return typeof(object);
        }

        public virtual object CreateInstance(Type itemType)
        {
            return CreateInstance(itemType, (IDesignerHost)this.GetService(typeof(IDesignerHost)), null);
        }

        internal static object CreateInstance(Type itemType, IDesignerHost host, string name)
        {
            object obj2 = null;
            if (typeof(IComponent).IsAssignableFrom(itemType) && (host != null))
            {
                obj2 = host.CreateComponent(itemType, name);
                if (host != null)
                {
                    IComponentInitializer designer = host.GetDesigner((IComponent)obj2) as IComponentInitializer;
                    if (designer != null)
                    {
                        designer.InitializeNewComponent(null);
                    }
                }
            }
            if (obj2 == null)
            {
                obj2 = TypeDescriptor.CreateInstance(host, itemType, null, null);
            }
            return obj2;
        }

        protected virtual Type[] CreateNewItemTypes()
        {
            return new Type[] { this.CollectionItemType };
        }

        public virtual void DestroyInstance(object instance)
        {
            IComponent component = instance as IComponent;
            if (component != null)
            {
                IDesignerHost service = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                if (service != null)
                {
                    service.DestroyComponent(component);
                }
                else
                {
                    component.Dispose();
                }
            }
            else
            {
                IDisposable disposable = instance as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc == null)
                {
                    return value;
                }
                if (MainForm != null)
                {
                    MainForm.Saved = false;
                }

                this.currentContext = context;
                CustomCollectionForm form = this.CreateCollectionForm();
                ITypeDescriptorContext currentContext = this.currentContext;
                form.EditValue = value;
                this.ignoreChangingEvents = false;
                this.ignoreChangedEvents = false;
                DesignerTransaction transaction = null;
                bool flag = true;
                IComponentChangeService service2 = null;
                IDesignerHost service = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                try
                {
                    try
                    {
                        if (service != null)
                        {
                            transaction = service.CreateTransaction(); /// (SR.GetString("CollectionEditorUndoBatchDesc", new object[] { this.CollectionItemType.Name }));
                        }
                    }
                    catch (CheckoutException exception)
                    {
                        if (exception != CheckoutException.Canceled)
                        {
                            throw exception;
                        }
                        return value;
                    }
                    service2 = (service != null) ? ((IComponentChangeService)service.GetService(typeof(IComponentChangeService))) : null;
                    if (service2 != null)
                    {
                        service2.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
                        service2.ComponentChanging += new ComponentChangingEventHandler(this.OnComponentChanging);
                    }
                    if (form.ShowEditorDialog(edSvc) == DialogResult.OK)
                    {
                        value = form.EditValue;
                        return value;
                    }
                    flag = false;
                }
                finally
                {
                    form.EditValue = null;
                    this.currentContext = currentContext;
                    if (transaction != null)
                    {
                        if (flag)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Cancel();
                        }
                    }
                    if (service2 != null)
                    {
                        service2.ComponentChanged -= new ComponentChangedEventHandler(this.OnComponentChanged);
                        service2.ComponentChanging -= new ComponentChangingEventHandler(this.OnComponentChanging);
                    }
                    form.Dispose();
                }
            }
            return value;
        }

        public virtual string GetDisplayText(object value)
        {
            string str;
            if (value == null)
            {
                return string.Empty;
            }
            PropertyDescriptor defaultProperty = TypeDescriptor.GetProperties(value)["Name"];
            if ((defaultProperty != null) && (defaultProperty.PropertyType == typeof(string)))
            {
                str = (string)defaultProperty.GetValue(value);
                if ((str != null) && (str.Length > 0))
                {
                    return str;
                }
            }
            defaultProperty = TypeDescriptor.GetDefaultProperty(this.CollectionType);
            if ((defaultProperty != null) && (defaultProperty.PropertyType == typeof(string)))
            {
                str = (string)defaultProperty.GetValue(value);
                if ((str != null) && (str.Length > 0))
                {
                    return str;
                }
            }
            str = TypeDescriptor.GetConverter(value).ConvertToString(value);
            if ((str != null) && (str.Length != 0))
            {
                return str;
            }
            return value.GetType().Name;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public virtual object[] GetItems(object editValue)
        {
            if ((editValue == null) || !(editValue is ICollection))
            {
                return new object[0];
            }
            ArrayList list = new ArrayList();
            ICollection is2 = (ICollection)editValue;
            foreach (object obj2 in is2)
            {
                list.Add(obj2);
            }
            object[] array = new object[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        public virtual IList GetObjectsFromInstance(object instance)
        {
            ArrayList list = new ArrayList();
            list.Add(instance);
            return list;
        }

        public object GetService(Type serviceType)
        {
            if (this.Context != null)
            {
                return this.Context.GetService(serviceType);
            }
            return null;
        }

        public bool IsAnyObjectInheritedReadOnly(object[] items)
        {
            IInheritanceService service = null;
            bool flag = false;
            foreach (object obj2 in items)
            {
                IComponent component = obj2 as IComponent;
                if ((component != null) && (component.Site == null))
                {
                    if (!flag)
                    {
                        flag = true;
                        if (this.Context != null)
                        {
                            service = (IInheritanceService)this.Context.GetService(typeof(IInheritanceService));
                        }
                    }
                    if ((service != null) && service.GetInheritanceAttribute(component).Equals(InheritanceAttribute.InheritedReadOnly))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (!this.ignoreChangedEvents && (sender != this.Context.Instance))
            {
                this.ignoreChangedEvents = true;
                this.Context.OnComponentChanged();
            }
        }

        private void OnComponentChanging(object sender, ComponentChangingEventArgs e)
        {
            if (!this.ignoreChangingEvents && (sender != this.Context.Instance))
            {
                this.ignoreChangingEvents = true;
                this.Context.OnComponentChanging();
            }
        }

        internal virtual void OnItemRemoving(object item)
        {
        }

        public virtual object SetItems(object editValue, object[] value)
        {
            if (editValue != null)
            {
                int length = this.GetItems(editValue).Length;
                int num2 = value.Length;
                if (!(editValue is IList))
                {
                    return editValue;
                }
                IList list = (IList)editValue;
                list.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    list.Add(value[i]);
                }
            }
            return editValue;
        }

        public virtual void ShowHelp()
        {
            IHelpService service = this.GetService(typeof(IHelpService)) as IHelpService;
            if (service != null)
            {
                service.ShowHelpFromKeyword(this.HelpTopic);
            }
        }

        // Properties
        public Type CollectionItemType
        {
            get
            {
                if (this.collectionItemType == null)
                {
                    this.collectionItemType = this.CreateCollectionItemType();
                }
                return this.collectionItemType;
            }
        }

        public Type CollectionType
        {
            get
            {
                return this.type;
            }
        }

        public ITypeDescriptorContext Context
        {
            get
            {
                return this.currentContext;
            }
        }

        protected virtual string HelpTopic
        {
            get
            {
                return "net.ComponentModel.CollectionEditor";
            }
        }

        public Type[] NewItemTypes
        {
            get
            {
                if (this.newItemTypes == null)
                {
                    this.newItemTypes = this.CreateNewItemTypes();
                }
                return this.newItemTypes;
            }
        }

        public static MainForm MainForm
        {
            get { return mainForm; }
            set { mainForm = value; }
        }

        // Nested Types





        internal class FilterListBox : ListBox
        {
            // Fields
            private PropertyGrid grid;
            private Message lastKeyDown;

            // Methods
            public void RefreshItem(int index)
            {
                base.RefreshItem(index);
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            private static extern IntPtr SetFocus(HandleRef hWnd);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            private static extern IntPtr GetFocus();

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
 

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case 0x100:
                        this.lastKeyDown = m;
                        if ((((int)m.WParam) == 0xe5) && (this.PropertyGrid != null))
                        {
                            this.PropertyGrid.Focus();
                            SetFocus(new HandleRef(this.PropertyGrid, this.PropertyGrid.Handle));
                            Application.DoEvents();
                            if (this.PropertyGrid.Focused || this.PropertyGrid.ContainsFocus)
                            {
                                SendMessage(GetFocus(), 0x100, this.lastKeyDown.WParam, this.lastKeyDown.LParam);
                            }
                        }
                        break;

                    case 0x102:
                        {
                            if (((Control.ModifierKeys & (Keys.Alt | Keys.Control)) != Keys.None) || (this.PropertyGrid == null))
                            {
                                break;
                            }
                            this.PropertyGrid.Focus();
                            SetFocus(new HandleRef(this.PropertyGrid, this.PropertyGrid.Handle));
                            Application.DoEvents();
                            if (!this.PropertyGrid.Focused && !this.PropertyGrid.ContainsFocus)
                            {
                                break;
                            }
                            IntPtr focus = GetFocus();
                            SendMessage(focus, 0x100, this.lastKeyDown.WParam, this.lastKeyDown.LParam);
                            SendMessage(focus, 0x102, m.WParam, m.LParam);
                            return;
                        }
                }
                base.WndProc(ref m);
            }

            // Properties
            private PropertyGrid PropertyGrid
            {
                get
                {
                    if (this.grid == null)
                    {
                        foreach (Control control in base.Parent.Controls)
                        {
                            if (control is PropertyGrid)
                            {
                                this.grid = (PropertyGrid)control;
                                break;
                            }
                        }
                    }
                    return this.grid;
                }
            }
        }

        internal class PropertyGridSite : ISite, IServiceProvider
        {
            // Fields
            private IComponent comp;
            private bool inGetService;
            private IServiceProvider sp;

            // Methods
            public PropertyGridSite(IServiceProvider sp, IComponent comp)
            {
                this.sp = sp;
                this.comp = comp;
            }

            public object GetService(Type t)
            {
                if (!this.inGetService && (this.sp != null))
                {
                    try
                    {
                        this.inGetService = true;
                        return this.sp.GetService(t);
                    }
                    finally
                    {
                        this.inGetService = false;
                    }
                }
                return null;
            }

            // Properties
            public IComponent Component
            {
                get
                {
                    return this.comp;
                }
            }

            public IContainer Container
            {
                get
                {
                    return null;
                }
            }

            public bool DesignMode
            {
                get
                {
                    return false;
                }
            }

            public string Name
            {
                get
                {
                    return null;
                }
                set
                {
                }
            }
        }

        internal class SplitButton : Button
        {
            // Fields
            private PushButtonState _state;
            private Rectangle dropDownRectangle = new Rectangle();
            private const int pushButtonWidth = 14;
            private bool showSplit;

            // Methods
            private void ContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
            {
                ContextMenuStrip strip = sender as ContextMenuStrip;
                if (strip != null)
                {
                    strip.Closed -= new ToolStripDropDownClosedEventHandler(this.ContextMenuStrip_Closed);
                }
                this.SetButtonDrawState();
            }

            public override Size GetPreferredSize(Size proposedSize)
            {
                Size preferredSize = base.GetPreferredSize(proposedSize);
                if ((this.showSplit && !string.IsNullOrEmpty(this.Text)) && ((TextRenderer.MeasureText(this.Text, this.Font).Width + 14) > preferredSize.Width))
                {
                    return (preferredSize + new Size(14, 0));
                }
                return preferredSize;
            }

            protected override bool IsInputKey(Keys keyData)
            {
                return ((keyData.Equals(Keys.Down) && this.showSplit) || base.IsInputKey(keyData));
            }

            protected override void OnGotFocus(EventArgs e)
            {
                if (!this.showSplit)
                {
                    base.OnGotFocus(e);
                }
                else if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
                {
                    this.State = PushButtonState.Default;
                }
            }

            protected override void OnKeyDown(KeyEventArgs kevent)
            {
                if (kevent.KeyCode.Equals(Keys.Down) && this.showSplit)
                {
                    this.ShowContextMenuStrip();
                }
            }

            protected override void OnLostFocus(EventArgs e)
            {
                if (!this.showSplit)
                {
                    base.OnLostFocus(e);
                }
                else if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
                {
                    this.State = PushButtonState.Normal;
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (!this.showSplit)
                {
                    base.OnMouseDown(e);
                }
                else if (this.dropDownRectangle.Contains(e.Location))
                {
                    this.ShowContextMenuStrip();
                }
                else
                {
                    this.State = PushButtonState.Pressed;
                }
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                if (!this.showSplit)
                {
                    base.OnMouseEnter(e);
                }
                else if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
                {
                    this.State = PushButtonState.Hot;
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                if (!this.showSplit)
                {
                    base.OnMouseLeave(e);
                }
                else if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
                {
                    if (this.Focused)
                    {
                        this.State = PushButtonState.Default;
                    }
                    else
                    {
                        this.State = PushButtonState.Normal;
                    }
                }
            }

            protected override void OnMouseUp(MouseEventArgs mevent)
            {
                if (!this.showSplit)
                {
                    base.OnMouseUp(mevent);
                }
                else if ((this.ContextMenuStrip == null) || !this.ContextMenuStrip.Visible)
                {
                    this.SetButtonDrawState();
                    if (base.Bounds.Contains(base.Parent.PointToClient(Cursor.Position)) && !this.dropDownRectangle.Contains(mevent.Location))
                    {
                        this.OnClick(new EventArgs());
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs pevent)
            {
                base.OnPaint(pevent);
                if (this.showSplit)
                {
                    Graphics g = pevent.Graphics;
                    Rectangle bounds = new Rectangle(0, 0, base.Width, base.Height);
                    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    ButtonRenderer.DrawButton(g, bounds, this.State);
                    this.dropDownRectangle = new Rectangle((bounds.Right - 14) - 1, 4, 14, bounds.Height - 8);
                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        this.dropDownRectangle.X = bounds.Left + 1;
                        g.DrawLine(SystemPens.ButtonHighlight, bounds.Left + 14, 4, bounds.Left + 14, bounds.Bottom - 4);
                        g.DrawLine(SystemPens.ButtonHighlight, (bounds.Left + 14) + 1, 4, (bounds.Left + 14) + 1, bounds.Bottom - 4);
                        bounds.Offset(14, 0);
                        bounds.Width -= 14;
                    }
                    else
                    {
                        g.DrawLine(SystemPens.ButtonHighlight, bounds.Right - 14, 4, bounds.Right - 14, bounds.Bottom - 4);
                        g.DrawLine(SystemPens.ButtonHighlight, (bounds.Right - 14) - 1, 4, (bounds.Right - 14) - 1, bounds.Bottom - 4);
                        bounds.Width -= 14;
                    }
                    this.PaintArrow(g, this.dropDownRectangle);
                    if (!base.UseMnemonic)
                    {
                        flags |= TextFormatFlags.NoPrefix;
                    }
                    else if (!this.ShowKeyboardCues)
                    {
                        flags |= TextFormatFlags.HidePrefix;
                    }
                    if (!string.IsNullOrEmpty(this.Text))
                    {
                        TextRenderer.DrawText(g, this.Text, this.Font, bounds, SystemColors.ControlText, flags);
                    }
                    if (this.Focused)
                    {
                        bounds.Inflate(-4, -4);
                    }
                }
            }

            private void PaintArrow(Graphics g, Rectangle dropDownRect)
            {
                Point point = new Point(Convert.ToInt32((int)(dropDownRect.Left + (dropDownRect.Width / 2))), Convert.ToInt32((int)(dropDownRect.Top + (dropDownRect.Height / 2))));
                point.X += dropDownRect.Width % 2;
                Point[] points = new Point[] { new Point(point.X - 2, point.Y - 1), new Point(point.X + 3, point.Y - 1), new Point(point.X, point.Y + 2) };
                g.FillPolygon(SystemBrushes.ControlText, points);
            }

            private void SetButtonDrawState()
            {
                if (base.Bounds.Contains(base.Parent.PointToClient(Cursor.Position)))
                {
                    this.State = PushButtonState.Hot;
                }
                else if (this.Focused)
                {
                    this.State = PushButtonState.Default;
                }
                else
                {
                    this.State = PushButtonState.Normal;
                }
            }

            private void ShowContextMenuStrip()
            {
                this.State = PushButtonState.Pressed;
                if (this.ContextMenuStrip != null)
                {
                    this.ContextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(this.ContextMenuStrip_Closed);
                    this.ContextMenuStrip.Show(this, 0, base.Height);
                }
            }

            // Properties
            public bool ShowSplit
            {
                set
                {
                    if (value != this.showSplit)
                    {
                        this.showSplit = value;
                        base.Invalidate();
                    }
                }
            }

            private PushButtonState State
            {
                get
                {
                    return this._state;
                }
                set
                {
                    if (!this._state.Equals(value))
                    {
                        this._state = value;
                        base.Invalidate();
                    }
                }
            }
        }
    }


}
