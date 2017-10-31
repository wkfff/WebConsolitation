using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Infragistics.Shared;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraGauge.Resources.Editor;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinToolbars;


namespace Krista.FM.Client.MDXExpert
{
    public sealed class CustomGaugeCollectionEditorBaseForm : Form
    {
        // Fields
        private GaugeCollectionBase _collection;
        private bool _ForceMultiAdd;
        private Type[] _ItemTypes;
        private GaugeCollectionBase _oldCollection;
        private PropertyDescriptor _Property;
        private object _PropertyReset;
        private string[] _TypeNames;
        private UltraButton addButton;
        private PopupMenuTool addTypeMenu;
        private UltraButton cancelButton;
        private UltraButton downButton;
        private ListBox listBox;
        private PopupMenuTool listBoxContextMenu;
        private UltraButton okButton;
        private Panel panel1;
        private Panel panel2;
        private UltraLabel propertiesLabel;
        private UltraPropPagePropertyGrid propertyGrid;
        private PopupMenuTool propertyGridContextMenu;
        private UltraButton removeButton;
        private Splitter spl1;
        private TrackBar trackBar1;
        private UltraToolbarsManager ultraToolbarsManager1;
        private UltraButton upButton;

        //элемент отчета, для которого вызывается редактор с данной формой
        private GaugeReportElement _gaugeElement;
        // Methods
        public CustomGaugeCollectionEditorBaseForm()
            : this(null, null)
        {
        }

        public CustomGaugeCollectionEditorBaseForm(GaugeCollectionBase collection, PropertyDescriptor property)
        {
            this.ultraToolbarsManager1 = new UltraToolbarsManager();
            this.ultraToolbarsManager1.DockWithinContainer = this;
            Utilities.ForceNativeMessageFilter();
            this.InitializeComponent();
            this.Collection = collection;
            this.Property = property;
            this.propertyGridContextMenu = new PopupMenuTool("propertyGridContextMenu");
            ButtonTool tool = new ButtonTool("contextMenuReset")
            {
                SharedProps = { Caption = "Сбросить" }
            };
            StateButtonTool tool2 = new StateButtonTool("contextMenuDescription")
            {
                Checked = true
            };
            tool2.SharedProps.Caption = "Описание";
            tool2.MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
            this.ultraToolbarsManager1.Tools.AddRange(new ToolBase[] { this.propertyGridContextMenu, tool, tool2 });
            this.propertyGridContextMenu.Tools.AddToolRange(new string[] { "contextMenuReset", "contextMenuDescription" });
            this.propertyGridContextMenu.Tools["contextMenuDescription"].InstanceProps.IsFirstInGroup = true;
            this.ultraToolbarsManager1.SetContextMenuUltra(this.propertyGrid, "propertyGridContextMenu");
            this.addTypeMenu = new PopupMenuTool("addTypeMenu");
            this.ultraToolbarsManager1.Tools.Add(this.addTypeMenu);
            this.listBoxContextMenu = new PopupMenuTool("listBoxContextMenu");
            ButtonTool tool3 = new ButtonTool("cloneItem")
            {
                SharedProps = { Caption = "Копировать элемент" }
            };
            this.ultraToolbarsManager1.Tools.Add(tool3);
            this.ultraToolbarsManager1.Tools.Add(this.listBoxContextMenu);
            this.listBoxContextMenu.Tools.AddTool(tool3.Key);
            this.listBox.MouseDown += new MouseEventHandler(this.listBox_MouseDown);
            this.ultraToolbarsManager1.ToolClick += new ToolClickEventHandler(this.ToolClick);

            this._gaugeElement = MainForm.Instance.ActiveGaugeElement;


        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if ((this.ItemTypes != null) && ((this.ItemTypes.Length > 1) || this.ForceMultiAdd))
            {
                Point screenPoint = base.PointToScreen(this.addButton.Location);
                screenPoint.Y += this.addButton.Height;
                this.addTypeMenu.ShowPopup(screenPoint, this.addButton);
            }
            else if ((this.ItemTypes != null) && (this.ItemTypes.Length > 0))
            {
                this.AddItem(this.ItemTypes[0]);
            }
            else
            {
                this.AddItem(typeof(object));
            }
            this.RefreshListBox();
            this.RefreshGauge();
        }

        private void AddItem(object newItem)
        {
            ((IList)this.Collection).Add(newItem);
            this.listBox.SelectedIndex = this.listBox.Items.Add(newItem);
        }

        private void AddItem(Type type)
        {
            object obj2;
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(IGaugeComponent) });
            if (constructor != null)
            {
                obj2 = constructor.Invoke(new object[] { this.Collection.GaugeComponent });
            }
            else
            {
                obj2 = type.GetConstructor(new Type[0]).Invoke(null);
            }
            this.AddItem(obj2);

            GaugeSettings settings = ((this._gaugeElement != null) && (GaugeConsts.gaugeCollectionSettings.ContainsKey(this._gaugeElement.PresetName))) ? GaugeConsts.gaugeCollectionSettings[this._gaugeElement.PresetName] : null;
            double rangeLength = (this._gaugeElement != null) ? GetRangeLength(this._gaugeElement) : 0;

            if (obj2 is LinearGaugeRange)
            {
                LinearGaugeRange range = (LinearGaugeRange)obj2;

                range.BrushElement = new SolidFillBrushElement();
                if (settings != null)
                {
                    range.InnerExtent = settings.InnerExtent;
                    range.OuterExtent = settings.OuterExtent;

                    double startValue = this._gaugeElement.StartValue;
                    foreach (LinearGaugeRange existRange in (LinearGaugeRangeCollection)this.Collection)
                    {
                        existRange.StartValue = startValue;
                        existRange.EndValue = startValue + rangeLength;
                        startValue += rangeLength;
                    }

                    range.StartValue = this._gaugeElement.EndValue - rangeLength;
                    range.EndValue = this._gaugeElement.EndValue;
                }


            }

            if (obj2 is RadialGaugeRange)
            {
                RadialGaugeRange range = (RadialGaugeRange)obj2;

                range.BrushElement = new SolidFillBrushElement();
                if (settings != null)
                {
                    range.InnerExtentStart = settings.InnerExtent;
                    range.InnerExtentEnd = settings.InnerExtent;
                    range.OuterExtent = settings.OuterExtent;

                    double startValue = this._gaugeElement.StartValue;
                    foreach (RadialGaugeRange existRange in (RadialGaugeRangeCollection)this.Collection)
                    {
                        existRange.StartValue = startValue;
                        existRange.EndValue = startValue + rangeLength;
                        startValue += rangeLength;
                    }

                    range.StartValue = this._gaugeElement.EndValue - rangeLength;
                    range.EndValue = this._gaugeElement.EndValue;
                }
            }
        }

        /// <summary>
        /// Получение длины цветового интервала
        /// </summary>
        /// <param name="gaugeElement"></param>
        /// <returns></returns>
        private double GetRangeLength(GaugeReportElement gaugeElement)
        {
            return (gaugeElement.EndValue - gaugeElement.StartValue)/(double)(this.Collection.Count);
        }


        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.OnCancel();
        }

        protected override void Dispose(bool disposing)
        {
            this.propertyGrid.Site = null;
            if (this.ultraToolbarsManager1 != null)
            {
                this.ultraToolbarsManager1.Dispose();
            }
        }

        private void GaugeCollectionEditorBaseForm_Load(object sender, EventArgs e)
        {
            this.okButton.Text = "OK";
            this.cancelButton.Text = "Отмена";
            this.removeButton.Text = "Удалить";
            this.addButton.Text = "Добавить";
            base.AcceptButton = this.okButton;
            base.CancelButton = this.cancelButton;
            if (this.Collection != null)
            {
                if (((this.Collection.GaugeComponent != null) && (this.Collection.GaugeComponent.Site != null)) && this.Collection.GaugeComponent.Site.DesignMode)
                {
                    this.propertyGrid.Site = this.Collection.GaugeComponent.Site;
                }
                this.OldCollection = this.Collection.Clone();
                IList collection = this.Collection;
                for (int i = 0; i < this.Collection.Count; i++)
                {
                    this.listBox.Items.Add(collection[i]);
                }
            }
            if (((this.ItemTypes != null) && ((this.ItemTypes.Length > 1) || this.ForceMultiAdd)) && (this.TypeNames.Length == this.ItemTypes.Length))
            {
                for (int j = 0; j < this.ItemTypes.Length; j++)
                {
                    ButtonTool tool = new ButtonTool(this.TypeNames[j]);
                    string str = ""; //"SR_AddPrefix";
                    string str2 = ""; //"SR_AddSuffix";
                    tool.SharedProps.Caption = str + this.TypeNames[j] + str2;
                    this.ultraToolbarsManager1.Tools.Add(tool);
                    this.addTypeMenu.Tools.AddTool(tool.Key);
                }
            }
            this.listBox.SelectionMode = SelectionMode.One;
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            this.addButton.Click += new EventHandler(this.addButton_Click);
            this.removeButton.Click += new EventHandler(this.removeButton_Click);
            if (this.listBox.Items.Count != 0)
            {
                this.listBox.SelectedIndex = 0;
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(GaugeCollectionEditorBaseForm));
            Infragistics.Win.Appearance appearance = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.propertyGrid = new UltraPropPagePropertyGrid();
            this.upButton = new UltraButton();
            this.downButton = new UltraButton();
            this.propertiesLabel = new UltraLabel();
            this.okButton = new UltraButton();
            this.cancelButton = new UltraButton();
            this.panel1 = new Panel();
            this.addButton = new UltraButton();
            this.listBox = new ListBox();
            this.removeButton = new UltraButton();
            this.spl1 = new Splitter();
            this.panel2 = new Panel();
            this.trackBar1 = new TrackBar();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.trackBar1.BeginInit();
            base.SuspendLayout();
            manager.ApplyResources(this.propertyGrid, "propertyGrid");
            this.propertyGrid.CommandsActiveLinkColor = SystemColors.ActiveCaption;
            this.propertyGrid.CommandsDisabledLinkColor = SystemColors.ControlDark;
            this.propertyGrid.CommandsLinkColor = SystemColors.ActiveCaption;
            this.propertyGrid.LineColor = SystemColors.ScrollBar;
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            manager.ApplyResources(this.upButton, "upButton");
            appearance.Image = manager.GetObject("appearance1.Image");
            appearance.ImageHAlign = HAlign.Center;
            appearance.ImageVAlign = VAlign.Middle;
            this.upButton.Appearance = appearance;
            this.upButton.Name = "upButton";
            this.upButton.Click += new EventHandler(this.upDownButtons_Click);
            manager.ApplyResources(this.downButton, "downButton");
            appearance2.Image = manager.GetObject("appearance2.Image");
            this.downButton.Appearance = appearance2;
            this.downButton.Name = "downButton";
            this.downButton.Click += new EventHandler(this.upDownButtons_Click);
            manager.ApplyResources(this.propertiesLabel, "propertiesLabel");
            this.propertiesLabel.Name = "propertiesLabel";
            manager.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.Click += new EventHandler(this.okButton_Click);
            manager.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new EventHandler(this.cmdCancel_Click);
            this.panel1.Controls.Add(this.addButton);
            this.panel1.Controls.Add(this.listBox);
            this.panel1.Controls.Add(this.removeButton);
            this.panel1.Controls.Add(this.upButton);
            this.panel1.Controls.Add(this.downButton);
            manager.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            manager.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            manager.ApplyResources(this.listBox, "listBox");
            this.listBox.BackColor = SystemColors.Window;
            this.listBox.Name = "listBox";
            manager.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.Name = "removeButton";
            manager.ApplyResources(this.spl1, "spl1");
            this.spl1.Name = "spl1";
            this.spl1.TabStop = false;
            this.panel2.Controls.Add(this.trackBar1);
            this.panel2.Controls.Add(this.propertyGrid);
            this.panel2.Controls.Add(this.cancelButton);
            this.panel2.Controls.Add(this.okButton);
            manager.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            manager.ApplyResources(this.trackBar1, "trackBar1");
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = 10;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.TickStyle = TickStyle.None;
            this.trackBar1.Value = 100;
            this.trackBar1.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
            manager.ApplyResources(this, "$this");
            base.Controls.Add(this.panel2);
            base.Controls.Add(this.spl1);
            base.Controls.Add(this.panel1);
            base.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            base.Name = "GaugeCollectionEditorBaseForm";
            base.Load += new EventHandler(this.GaugeCollectionEditorBaseForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.trackBar1.EndInit();
            base.ResumeLayout(false);
        }

        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (this.listBox.SelectedIndex != -1))
            {
                Point screenPoint = base.PointToScreen(new Point(e.X, e.Y));
                this.listBoxContextMenu.ShowPopup(screenPoint, this.listBox);
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedItem is GaugeRange)
            {
                this.propertyGrid.SelectedObject = new  GaugeRangeBrowseClass((GaugeRange)this.listBox.SelectedItem, this._gaugeElement);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.RefreshGauge();
            base.Close();
        }

        private void OnCancel()
        {
            this.Collection.Clear();
            IList collection = this.Collection;
            foreach (object obj2 in (IEnumerable)this.OldCollection)
            {
                collection.Add(obj2);
            }
            this.RefreshGauge();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            this.RefreshListBox();
            this.RefreshGauge();
            

        }

        private void propertyGridContextMenu_Popup(object sender, BeforeToolDropdownEventArgs e)
        {
            this.PropertyReset = null;
            PropertyDescriptor propertyDescriptor = this.propertyGrid.SelectedGridItem.PropertyDescriptor;
            if (propertyDescriptor != null)
            {
                if ((propertyDescriptor.Attributes != null) && (propertyDescriptor.Attributes[typeof(DefaultValueAttribute)] != null))
                {
                    DefaultValueAttribute attribute = propertyDescriptor.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
                    if (attribute != null)
                    {
                        this.PropertyReset = attribute.Value;
                    }
                }
                else
                {
                    Type propertyType;
                    if (((this.propertyGrid != null) && (this.propertyGrid.SelectedGridItem != null)) && ((this.propertyGrid.SelectedGridItem.Parent != null) && (this.propertyGrid.SelectedGridItem.Parent.PropertyDescriptor != null)))
                    {
                        propertyType = this.propertyGrid.SelectedGridItem.Parent.PropertyDescriptor.PropertyType;
                    }
                    else
                    {
                        propertyType = this.propertyGrid.SelectedObject.GetType();
                    }
                    MethodInfo method = propertyType.GetMethod("Reset" + propertyDescriptor.Name);
                    if (method != null)
                    {
                        this.PropertyReset = method;
                    }
                }
            }
            this.propertyGridContextMenu.Tools["contextMenuReset"].SharedProps.Enabled = this.PropertyReset != null;
        }

        private void RefreshGauge()
        {
            IComponent component = null;
            if ((this.propertyGrid.Site != null) && this.propertyGrid.Site.DesignMode)
            {
                component = this.propertyGrid.Site.Component;
            }
            if (this.Collection.GaugeComponent != null)
            {
                this.Collection.GaugeComponent.Invalidate();
            }
            if (component != null)
            {
                TypeDescriptor.Refresh(component);
                IComponentChangeService service = (IComponentChangeService)this.propertyGrid.Site.GetService(typeof(IComponentChangeService));
                if ((this.Property != null) && (this.Property.ComponentType == component.GetType()))
                {
                    service.OnComponentChanged(component, this.Property, this.OldCollection, this.Collection);
                }
                else
                {
                    PropertyDescriptor member = TypeDescriptor.GetProperties(component).Find("Visible", false);
                    if (member != null)
                    {
                        bool oldValue = (bool)member.GetValue(component);
                        service.OnComponentChanged(component, member, oldValue, oldValue);
                    }
                }
            }
            this._gaugeElement.InitLegend();
        }

        private void RefreshListBox()
        {
            Application.DoEvents();
            object selectedItem = this.listBox.SelectedItem;
            object[] destination = new object[this.listBox.Items.Count];
            this.listBox.Items.CopyTo(destination, 0);
            this.listBox.Items.Clear();
            this.listBox.Items.AddRange(destination);
            this.listBox.SelectedItem = selectedItem;
        }


        private void removeButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.listBox.SelectedIndex;
            IList collection = this.Collection;
            if (selectedIndex >= 0)
            {
                collection.Remove(this.listBox.SelectedItem);
                this.listBox.Items.Remove(this.listBox.SelectedItem);
                int count = this.listBox.Items.Count;
                if (selectedIndex < count)
                {
                    this.listBox.SelectedIndex = selectedIndex;
                }
                else
                {
                    this.listBox.SelectedIndex = count - 1;
                }
                if (count == 0)
                {
                    this.propertyGrid.SelectedObject = null;
                }
                else
                {
                    this.listBox_SelectedIndexChanged(this, null);
                }
                this.listBox.Refresh();
            }

            if ((this.Collection is GaugeRangeCollection)&&(this._gaugeElement != null))
            {
                double rangeLength = GetRangeLength(this._gaugeElement);
                double startValue = this._gaugeElement.StartValue;

                foreach(GaugeRange range in this.Collection)
                {
                    range.StartValue = startValue;
                    range.EndValue = startValue + rangeLength;
                    startValue += rangeLength;
                }
            }

            this.RefreshListBox();
            this.RefreshGauge();
        }

        internal void SetItemTypes(Type[] itemTypes)
        {
            this._ItemTypes = itemTypes;
        }

        internal void SetTypeNames(string[] typeNames)
        {
            this._TypeNames = typeNames;
        }

        private void ToolClick(object sender, ToolClickEventArgs e)
        {
            this.propertyGridContextMenu.BeforeToolDropdown += new BeforeToolDropdownEventHandler(this.propertyGridContextMenu_Popup);
            switch (e.Tool.Key)
            {
                case "contextMenuReset":
                    this.propertyGrid.ResetSelectedProperty();
                    this.propertyGrid.Refresh();
                    this.RefreshGauge();
                    return;

                case "contextMenuDescription":
                    this.propertyGrid.HelpVisible = ((StateButtonTool)this.ultraToolbarsManager1.Tools["contextMenuDescription"]).Checked;
                    return;

                case "cloneItem":
                    {
                        GaugeCollectionObject selectedItem = this.listBox.SelectedItem as GaugeCollectionObject;
                        if (selectedItem != null)
                        {
                            GaugeCollectionObject newItem = ((ICloneable)selectedItem).Clone() as GaugeCollectionObject;
                            if (newItem == null)
                            {
                                return;
                            }
                            this.AddItem(newItem);
                        }
                        return;
                    }
            }
            if (this.addTypeMenu.Tools.IndexOf(e.Tool.Key) != -1)
            {
                this.AddItem(this.ItemTypes[this.addTypeMenu.Tools.IndexOf(e.Tool.Key)]);
                this.listBox.SelectedIndex = this.listBox.Items.Count - 1;
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            base.Opacity = ((double)this.trackBar1.Value) / 100.0;
        }

        private void upDownButtons_Click(object sender, EventArgs e)
        {
            int num;
            if (sender == this.downButton)
            {
                num = 1;
            }
            else
            {
                num = -1;
            }
            int selectedIndex = this.listBox.SelectedIndex;
            if (((selectedIndex + num) >= 0) && ((selectedIndex + num) < this.listBox.Items.Count))
            {
                IList collection = this.Collection;
                this.listBox.Items.RemoveAt(selectedIndex);
                object obj2 = collection[selectedIndex];
                this.Collection.RemoveAt(selectedIndex);
                collection.Insert(selectedIndex + num, obj2);
                this.listBox.Items.Insert(selectedIndex + num, obj2);
                this.listBox.SelectedIndex = selectedIndex + num;
            }
        }

        // Properties
        private GaugeCollectionBase Collection
        {
            get
            {
                return this._collection;
            }
            set
            {
                this._collection = value;
            }
        }

        internal bool ForceMultiAdd
        {
            get
            {
                return this._ForceMultiAdd;
            }
            set
            {
                this._ForceMultiAdd = value;
            }
        }

        private Type[] ItemTypes
        {
            get
            {
                return this._ItemTypes;
            }
        }

        private GaugeCollectionBase OldCollection
        {
            get
            {
                return this._oldCollection;
            }
            set
            {
                this._oldCollection = value;
            }
        }

        public PropertyDescriptor Property
        {
            get
            {
                return this._Property;
            }
            set
            {
                this._Property = value;
            }
        }

        private object PropertyReset
        {
            get
            {
                return this._PropertyReset;
            }
            set
            {
                this._PropertyReset = value;
            }
        }

        private string[] TypeNames
        {
            get
            {
                return this._TypeNames;
            }
        }
    }


}
