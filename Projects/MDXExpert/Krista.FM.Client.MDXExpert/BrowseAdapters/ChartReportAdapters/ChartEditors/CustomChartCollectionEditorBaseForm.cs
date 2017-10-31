using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Infragistics.Shared;
using Infragistics.UltraChart.Design;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinToolbars;
using Appearance = Infragistics.Win.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomChartCollectionEditorBaseForm : Form
    {
        // Fields
        private IChartCollection _collection;
        private IChartCollection _oldCollection;
        private PropertyDescriptor _Property;
        private object _PropertyReset;
        protected UltraButton addButton;
        protected PopupMenuTool addTypeMenu;
        protected UltraButton cancelButton;
        protected UltraButton downButton;
        protected RoundBorderListBox listBox; 
        protected UltraButton okButton;
        protected UltraLabel propertiesLabel;
        protected UltraPropPagePropertyGrid propertyGrid;
        protected PopupMenuTool propertyGridContextMenu;
        protected UltraButton removeButton;
        protected UltraToolbarsManager ultraToolbarsManager1;
        protected UltraButton upButton;

        // Methods
        public CustomChartCollectionEditorBaseForm()
            : this(null, null)
        {
        }

        public CustomChartCollectionEditorBaseForm(IChartCollection collection, PropertyDescriptor property)
        {
            this.ultraToolbarsManager1 = new UltraToolbarsManager();
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
            this.ultraToolbarsManager1.ToolClick += new ToolClickEventHandler(this.ToolClick);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if ((this.ItemTypes != null) && (this.ItemTypes.Length > 1))
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
            this.RefreshChart();
        }

        protected virtual void AddItem(Type type)
        {
            object obj2;
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(IChartComponent) });
            if (constructor != null)
            {
                obj2 = constructor.Invoke(new object[] { this.Collection.ChartComponent });
            }
            else
            {
                obj2 = type.GetConstructor(new Type[0]).Invoke(null);
            }
            this.Collection.Add(obj2);
            
            this.listBox.SelectedIndex = this.listBox.Items.Add(obj2);
        }

        private void ChartCollectionEditorBaseForm_Load(object sender, EventArgs e)
        {
            this.okButton.Text = "ОК";
            this.cancelButton.Text = "Отмена";
            this.removeButton.Text = "Удалить";
            this.addButton.Text = "Добавить";
            base.AcceptButton = this.okButton;
            base.CancelButton = this.cancelButton;
            if (this.Collection != null)
            {
                if (((this.Collection.ChartComponent != null) && (this.Collection.ChartComponent.Site != null)) && this.Collection.ChartComponent.Site.DesignMode)
                {
                    this.propertyGrid.Site = this.Collection.ChartComponent.Site;
                }
                this.OldCollection = this.Collection.Clone() as IChartCollection;
                for (int i = 0; i < this.Collection.Count; i++)
                {
                    this.listBox.Items.Add(this.Collection[i]);
                }
                Attribute[] array = new Attribute[this.propertyGrid.BrowsableAttributes.Count + 3];
                this.propertyGrid.BrowsableAttributes.CopyTo(array, 0);
                array[array.Length - 3] = new BrowsableForOffsetModeAttribute(LocationOffsetMode.Manual | LocationOffsetMode.Automatic);
                array[array.Length - 2] = new BrowsableForLocationTypeAttribute(LocationType.Percentage | LocationType.Pixels | LocationType.DataValues | LocationType.RowColumn);
                array[array.Length - 1] = new BrowsableForPaintElementTypeAttribute(PaintElementType.Texture | PaintElementType.CustomBrush | PaintElementType.Gradient | PaintElementType.Hatch | PaintElementType.Image | PaintElementType.SolidFill);
                this.propertyGrid.BrowsableAttributes = new AttributeCollection(array);
                IProvideChartComponent collection = this.Collection;
                if ((collection != null) && (collection.ChartComponent != null))
                {
                    string str = collection.ChartComponent.GetType().ToString();
                    if (str.IndexOf("UltraWebChart") >= 0)
                    {
                        if (this.propertyGrid.BrowsableAttributes != null)
                        {
                            array = new Attribute[this.propertyGrid.BrowsableAttributes.Count + 1];
                            this.propertyGrid.BrowsableAttributes.CopyTo(array, 0);
                            array[array.Length - 1] = new WebBrowsableAttribute(true);
                            this.propertyGrid.BrowsableAttributes = new AttributeCollection(array);
                        }
                    }
                    else if ((str.IndexOf("UltraWinChart") >= 0) && (this.propertyGrid.BrowsableAttributes != null))
                    {
                        array = new Attribute[this.propertyGrid.BrowsableAttributes.Count + 1];
                        this.propertyGrid.BrowsableAttributes.CopyTo(array, 0);
                        array[array.Length - 1] = new WinBrowsableAttribute(true);
                        this.propertyGrid.BrowsableAttributes = new AttributeCollection(array);
                    }
                }
            }
            if (((this.ItemTypes != null) && (this.ItemTypes.Length > 1)) && (this.TypeNames.Length == this.ItemTypes.Length))
            {
                for (int j = 0; j < this.ItemTypes.Length; j++)
                {
                    ButtonTool tool = new ButtonTool(this.TypeNames[j])
                    {
                        //SharedProps = { Caption = "Добавить элемент - " + this.TypeNames[j] }
                        SharedProps = { Caption = this.TypeNames[j] }
                    };
                    this.ultraToolbarsManager1.Tools.Add(tool);
                    this.addTypeMenu.Tools.AddTool(tool.Key);
                }
            }
            this.listBox.SelectionMode = SelectionMode.One;
            this.OnLoaded();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.OnCancel();
        }

        protected override void Dispose(bool disposing)
        {
            this.propertyGrid.Site = null;
        }

        private object GetParentOfCurrentProperty(PropertyGrid grid, GridItem current)
        {
            if ((current.Parent != null) && (current.Parent.PropertyDescriptor != null))
            {
                object parentOfCurrentProperty = this.GetParentOfCurrentProperty(grid, current.Parent);
                if (parentOfCurrentProperty != null)
                {
                    PropertyInfo property = parentOfCurrentProperty.GetType().GetProperty(current.PropertyDescriptor.Name);
                    if (property != null)
                    {
                        return property.GetValue(parentOfCurrentProperty, null);
                    }
                }
            }
            return grid.SelectedObject;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(ChartCollectionEditorBaseForm));
            Appearance appearance = new Appearance();
            Appearance appearance2 = new Appearance();
            this.propertyGrid = new UltraPropPagePropertyGrid();
            this.listBox = new RoundBorderListBox();
            this.upButton = new UltraButton();
            this.downButton = new UltraButton();
            this.propertiesLabel = new UltraLabel();
            this.okButton = new UltraButton();
            this.cancelButton = new UltraButton();
            this.removeButton = new UltraButton();
            this.addButton = new UltraButton();
            base.SuspendLayout();
            manager.ApplyResources(this.propertyGrid, "propertyGrid");
            this.propertyGrid.CommandsActiveLinkColor = SystemColors.ActiveCaption;
            this.propertyGrid.CommandsDisabledLinkColor = SystemColors.ControlDark;
            this.propertyGrid.CommandsLinkColor = SystemColors.ActiveCaption;
            this.propertyGrid.LineColor = SystemColors.ScrollBar;
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.SelectedObjectsChanged += new EventHandler(this.propertyGrid_SelectedObjectsChanged);
            this.propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            manager.ApplyResources(this.listBox, "listBox");
            this.listBox.BackColor = SystemColors.Window;
            this.listBox.Name = "listBox";
            this.listBox.SelectedIndex = -1;
            this.listBox.SelectedItem = null;
            this.listBox.SelectionMode = SelectionMode.One;
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            appearance.Image = manager.GetObject("appearance1.Image");
            appearance.ImageHAlign = HAlign.Center;
            appearance.ImageVAlign = VAlign.Middle;
            this.upButton.Appearance = appearance;
            manager.ApplyResources(this.upButton, "upButton");
            this.upButton.Name = "upButton";
            this.upButton.Click += new EventHandler(this.upDownButtons_Click);
            appearance2.Image = manager.GetObject("appearance2.Image");
            this.downButton.Appearance = appearance2;
            manager.ApplyResources(this.downButton, "downButton");
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
            manager.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.Name = "removeButton";
            this.removeButton.Click += new EventHandler(this.removeButton_Click);
            manager.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new EventHandler(this.addButton_Click);
            manager.ApplyResources(this, "$this");
            this.Size = new Size(630, 390);
            base.Controls.Add(this.addButton);
            base.Controls.Add(this.propertyGrid);
            base.Controls.Add(this.listBox);
            base.Controls.Add(this.removeButton);
            base.Controls.Add(this.okButton);
            base.Controls.Add(this.cancelButton);
            base.Controls.Add(this.upButton);
            base.Controls.Add(this.downButton);
            base.Name = "ChartCollectionEditorBaseForm";
            this.ShowInTaskbar = false;

            base.Load += new EventHandler(this.ChartCollectionEditorBaseForm_Load);
            base.ResumeLayout(false);
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.propertyGrid.SelectedObject = this.listBox.SelectedItem;
        }

        private void menuItems_Click(object sender, EventArgs e)
        {
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.RefreshChart();
            MainForm.Instance.Saved = false;
            base.Close();
        }

        protected virtual void OnCancel()
        {
            this.Collection.Clear();
            foreach (object obj2 in this.OldCollection)
            {
                this.Collection.Add(obj2);
            }
            this.RefreshChart();
        }

        protected virtual void OnLoaded()
        {
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Value is LocationType)
            {
                this.UpdatePropertyGridBrowsableAttributes((LocationType)e.ChangedItem.Value);
            }
            else if (e.ChangedItem.Value is PaintElementType)
            {
                this.UpdatePropertyGridBrowsableAttributes((PaintElementType)e.ChangedItem.Value);
            }
            else if (e.ChangedItem.Value is LocationOffsetMode)
            {
                this.UpdatePropertyGridBrowsableAttributes((LocationOffsetMode)e.ChangedItem.Value);
            }
            this.RefreshListBox();
            this.RefreshChart();
        }

        private void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (this.propertyGrid.SelectedObject is Annotation)
            {
                Annotation selectedObject = (Annotation)this.propertyGrid.SelectedObject;
                if (selectedObject.Location != null)
                {
                    LocationType locationType = selectedObject.Location.Type;
                    if (selectedObject is OffsetableAnnotation)
                    {
                        locationType |= ((OffsetableAnnotation)selectedObject).Offset.Type;
                    }
                    this.UpdatePropertyGridBrowsableAttributes(locationType);
                }
                IProvidePaintElement element = selectedObject as IProvidePaintElement;
                if ((element != null) && (element.PE != null))
                {
                    this.UpdatePropertyGridBrowsableAttributes(element.PE.ElementType);
                }
                OffsetableAnnotation annotation2 = selectedObject as OffsetableAnnotation;
                if (annotation2 != null)
                {
                    this.UpdatePropertyGridBrowsableAttributes(annotation2.OffsetMode);
                }
            }
            else if (this.propertyGrid.SelectedObject is PaintElement)
            {
                PaintElement element2 = (PaintElement)this.propertyGrid.SelectedObject;
                this.UpdatePropertyGridBrowsableAttributes(element2.ElementType);
            }
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

        protected virtual void RefreshChart()
        {
            IComponent component = null;
            if ((this.propertyGrid.Site != null) && this.propertyGrid.Site.DesignMode)
            {
                component = this.propertyGrid.Site.Component;
            }
            if (this.Collection.ChartComponent != null)
            {
                this.Collection.ChartComponent.Invalidate(CacheLevel.LayerLevelCache);
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
        }

        protected virtual void RefreshListBox()
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
            if (selectedIndex >= 0)
            {
                this.Collection.Remove(this.listBox.SelectedItem);
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
            this.RefreshChart();
        }

        private void ToolClick(object sender, ToolClickEventArgs e)
        {
            this.propertyGridContextMenu.BeforeToolDropdown += new BeforeToolDropdownEventHandler(this.propertyGridContextMenu_Popup);
            switch (e.Tool.Key)
            {
                case "contextMenuReset":
                    {
                        object parentOfCurrentProperty = this.GetParentOfCurrentProperty(this.propertyGrid, this.propertyGrid.SelectedGridItem);
                        if (this.PropertyReset != null)
                        {
                            if (this.PropertyReset is MethodInfo)
                            {
                                ((MethodInfo)this.PropertyReset).Invoke(parentOfCurrentProperty, null);
                            }
                            else
                            {
                                this.propertyGrid.SelectedGridItem.PropertyDescriptor.SetValue(parentOfCurrentProperty, this.PropertyReset);
                            }
                            this.propertyGrid.Refresh();
                            this.RefreshChart();
                        }
                        return;
                    }
                case "contextMenuDescription":
                    this.propertyGrid.HelpVisible = ((StateButtonTool)this.ultraToolbarsManager1.Tools["contextMenuDescription"]).Checked;
                    return;
            }
            if (this.addTypeMenu.Tools.IndexOf(e.Tool.Key) != -1)
            {
                this.AddItem(this.ItemTypes[this.addTypeMenu.Tools.IndexOf(e.Tool.Key)]);
                this.listBox.SelectedIndex = this.listBox.Items.Count - 1;
            }
        }

        private void UpdatePropertyGridBrowsableAttributes(LocationOffsetMode offsetMode)
        {
            Attribute[] array = new Attribute[this.propertyGrid.BrowsableAttributes.Count];
            this.propertyGrid.BrowsableAttributes.CopyTo(array, 0);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] is BrowsableForOffsetModeAttribute)
                {
                    array[i] = new BrowsableForOffsetModeAttribute(offsetMode);
                }
            }
            this.propertyGrid.BrowsableAttributes = new AttributeCollection(array);
        }

        private void UpdatePropertyGridBrowsableAttributes(LocationType locationType)
        {
            Attribute[] array = new Attribute[this.propertyGrid.BrowsableAttributes.Count];
            this.propertyGrid.BrowsableAttributes.CopyTo(array, 0);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] is BrowsableForLocationTypeAttribute)
                {
                    array[i] = new BrowsableForLocationTypeAttribute(locationType);
                }
            }
            this.propertyGrid.BrowsableAttributes = new AttributeCollection(array);
        }

        private void UpdatePropertyGridBrowsableAttributes(PaintElementType paintElementType)
        {
            Attribute[] array = new Attribute[this.propertyGrid.BrowsableAttributes.Count];
            this.propertyGrid.BrowsableAttributes.CopyTo(array, 0);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] is BrowsableForPaintElementTypeAttribute)
                {
                    array[i] = new BrowsableForPaintElementTypeAttribute(paintElementType);
                }
            }
            this.propertyGrid.BrowsableAttributes = new AttributeCollection(array);
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
                this.listBox.Items.RemoveAt(selectedIndex);
                object obj2 = this.Collection[selectedIndex];
                this.Collection.RemoveAt(selectedIndex);
                this.Collection.Insert(selectedIndex + num, obj2);
                this.listBox.Items.Insert(selectedIndex + num, obj2);
                this.listBox.SelectedIndex = selectedIndex + num;
            }
        }

        // Properties
        protected IChartCollection Collection
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

        protected virtual Type[] ItemTypes
        {
            get
            {
                return new Type[] { typeof(object) };
            }
        }

        protected virtual IChartCollection OldCollection
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

        protected virtual string[] TypeNames
        {
            get
            {
                return new string[] { "Item" };
            }
        }
    }




}
