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
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomCollectionEditorCollectionForm : CustomCollectionForm
    {
        // Fields
        private CustomCollectionEditor.SplitButton addButton;
        private ContextMenuStrip addDownMenu;
        private Button cancelButton;
        private ArrayList createdItems;
        private bool dirty;
        private Button downButton;
        private CustomCollectionEditor editor;
        private CustomCollectionEditor.FilterListBox listbox;
        private static readonly double LOG10 = Math.Log(10.0);
        private Label membersLabel;
        private Button okButton;
        private ArrayList originalItems;
        private const int PAINT_INDENT = 0x1a;
        private const int PAINT_WIDTH = 20;
        private Label propertiesLabel;
        private CustomPropertyGrid propertyBrowser;
        private Button removeButton;
        private ArrayList removedItems;
        private int suspendEnabledCount;
        private const int TEXT_INDENT = 1;
        private Button upButton;
        private EditorButtons editorBtns = (EditorButtons)(EditorButtons.AddRemove | EditorButtons.UpDown);
        private MainForm mainForm;

        public string MembersLabel
        {
            get { return this.membersLabel.Text; }
            set { this.membersLabel.Text = value; }
        }

        public string FormHeader
        {
            get { return this.Text; }
            set { this.Text = value; }
        }


        public EditorButtons Buttons
        {
            get { return this.editorBtns; }
            set 
            {
                SetEditorBtns(value);
            }
        }

        // Methods
        public CustomCollectionEditorCollectionForm(CustomCollectionEditor editor)
            : base(editor)
        {
            this.editor = editor;
            this.InitializeComponent();
            ///this.Text = SR.GetString("CollectionEditorCaption", new object[] { base.CollectionItemType.Name });
            this.HookEvents();
            Type[] newItemTypes = base.NewItemTypes;
            if (newItemTypes.Length > 1)
            {
                EventHandler handler = new EventHandler(this.AddDownMenu_click);
                this.addButton.ShowSplit = true;
                this.addDownMenu = new ContextMenuStrip();
                this.addButton.ContextMenuStrip = this.addDownMenu;
                for (int i = 0; i < newItemTypes.Length; i++)
                {
                    this.addDownMenu.Items.Add(new TypeMenuItem(newItemTypes[i], handler));
                }
            }
            this.AdjustListBoxItemHeight();
            propertyBrowser.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyBrowser_PropertyValueChanged);
        }

        void propertyBrowser_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (this.MainForm != null)
                this.MainForm.Saved = false;
        }


        private void SetEditorBtns(EditorButtons buttons)
        {
            this.editorBtns = buttons;

            if ((buttons & EditorButtons.None) == EditorButtons.None)
            {
                addButton.Visible = false;
                removeButton.Visible = false;
                upButton.Visible = false;
                downButton.Visible = false;
                return;
            }

            bool addRemoveShow = ((buttons & EditorButtons.AddRemove) == EditorButtons.AddRemove);
            bool upDownShow = ((buttons & EditorButtons.UpDown) == EditorButtons.UpDown);

            addButton.Visible = addRemoveShow;
            removeButton.Visible = addRemoveShow;
            upButton.Visible = upDownShow;
            downButton.Visible = upDownShow;

        }

        private void AddButton_click(object sender, EventArgs e)
        {
            this.PerformAdd();
        }

        private void AddDownMenu_click(object sender, EventArgs e)
        {
            if (sender is TypeMenuItem)
            {
                TypeMenuItem item = (TypeMenuItem)sender;
                this.CreateAndAddInstance(item.ItemType);
            }
        }

        private void AddItems(IList instances)
        {
            if (this.createdItems == null)
            {
                this.createdItems = new ArrayList();
            }
            this.listbox.BeginUpdate();
            try
            {
                foreach (object obj2 in instances)
                {
                    if (obj2 != null)
                    {
                        this.dirty = true;
                        this.createdItems.Add(obj2);
                        ListItem item = new ListItem(this.editor, obj2);
                        this.listbox.Items.Add(item);
                    }
                }
            }
            finally
            {
                this.listbox.EndUpdate();
            }
            if (instances.Count == 1)
            {
                this.UpdateItemWidths(this.listbox.Items[this.listbox.Items.Count - 1] as ListItem);
            }
            else
            {
                this.UpdateItemWidths(null);
            }
            this.SuspendEnabledUpdates();
            try
            {
                this.listbox.ClearSelected();
                this.listbox.SelectedIndex = this.listbox.Items.Count - 1;
                object[] objArray = new object[this.listbox.Items.Count];
                for (int i = 0; i < objArray.Length; i++)
                {
                    objArray[i] = ((ListItem)this.listbox.Items[i]).Value;
                }
                base.Items = objArray;
                if ((this.listbox.Items.Count > 0) && (this.listbox.SelectedIndex != (this.listbox.Items.Count - 1)))
                {
                    this.listbox.ClearSelected();
                    this.listbox.SelectedIndex = this.listbox.Items.Count - 1;
                }
            }
            finally
            {
                this.ResumeEnabledUpdates(true);
            }
        }

        private void AdjustListBoxItemHeight()
        {
            this.listbox.ItemHeight = this.Font.Height + (SystemInformation.BorderSize.Width * 2);
        }

        private bool AllowRemoveInstance(object value)
        {
            return (((this.createdItems != null) && this.createdItems.Contains(value)) || base.CanRemoveInstance(value));
        }

        private int CalcItemWidth(Graphics g, ListItem item)
        {
            int count = this.listbox.Items.Count;
            if (count < 2)
            {
                count = 2;
            }
            SizeF ef = g.MeasureString(count.ToString(CultureInfo.CurrentCulture), this.listbox.Font);
            int num2 = ((int)(Math.Log((double)(count - 1)) / LOG10)) + 1;
            int num3 = 4 + (num2 * (this.Font.Height / 2));
            num3 = Math.Max(num3, (int)Math.Ceiling((double)ef.Width)) + (SystemInformation.BorderSize.Width * 4);
            SizeF ef2 = g.MeasureString(this.GetDisplayText(item), this.listbox.Font);
            int num4 = 0;
            if ((item.Editor != null) && item.Editor.GetPaintValueSupported())
            {
                num4 = 0x15;
            }
            return (((((int)Math.Ceiling((double)ef2.Width)) + num3) + num4) + (SystemInformation.BorderSize.Width * 4));
        }

        private void CancelButton_click(object sender, EventArgs e)
        {
            try
            {
                this.editor.CancelChanges();
                if (this.CollectionEditable && this.dirty)
                {
                    this.dirty = false;
                    this.listbox.Items.Clear();
                    if (this.createdItems != null)
                    {
                        object[] objArray = this.createdItems.ToArray();
                        if (((objArray.Length > 0) && (objArray[0] is IComponent)) && (((IComponent)objArray[0]).Site != null))
                        {
                            return;
                        }
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            base.DestroyInstance(objArray[i]);
                        }
                        this.createdItems.Clear();
                    }
                    if (this.removedItems != null)
                    {
                        this.removedItems.Clear();
                    }
                    if ((this.originalItems != null) && (this.originalItems.Count > 0))
                    {
                        object[] objArray2 = new object[this.originalItems.Count];
                        for (int j = 0; j < this.originalItems.Count; j++)
                        {
                            objArray2[j] = this.originalItems[j];
                        }
                        base.Items = objArray2;
                        this.originalItems.Clear();
                    }
                    else
                    {
                        base.Items = new object[0];
                    }
                }
            }
            catch (Exception exception)
            {
                base.DialogResult = DialogResult.None;
                this.DisplayError(exception);
            }
        }

        private void CollectionEditor_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.editor.ShowHelp();
        }

        private void CreateAndAddInstance(Type type)
        {
            try
            {
                object instance = base.CreateInstance(type);
                IList objectsFromInstance = this.editor.GetObjectsFromInstance(instance);
                if (objectsFromInstance != null)
                {
                    this.AddItems(objectsFromInstance);
                }
            }
            catch (Exception exception)
            {
                this.DisplayError(exception);
            }
        }

        private void DownButton_click(object sender, EventArgs e)
        {
            try
            {
                this.SuspendEnabledUpdates();
                this.dirty = true;
                int selectedIndex = this.listbox.SelectedIndex;
                if (selectedIndex != (this.listbox.Items.Count - 1))
                {
                    int topIndex = this.listbox.TopIndex;
                    object obj2 = this.listbox.Items[selectedIndex];
                    this.listbox.Items[selectedIndex] = this.listbox.Items[selectedIndex + 1];
                    this.listbox.Items[selectedIndex + 1] = obj2;
                    if (topIndex < (this.listbox.Items.Count - 1))
                    {
                        this.listbox.TopIndex = topIndex + 1;
                    }
                    this.listbox.ClearSelected();
                    this.listbox.SelectedIndex = selectedIndex + 1;
                    Control control = (Control)sender;
                    if (control.Enabled)
                    {
                        control.Focus();
                    }
                }
            }
            finally
            {
                this.ResumeEnabledUpdates(true);
            }
        }

        private void Form_HelpRequested(object sender, HelpEventArgs e)
        {
            this.editor.ShowHelp();
        }

        private string GetDisplayText(ListItem item)
        {
            if (item != null)
            {
                return item.ToString();
            }
            return string.Empty;
        }

        private void HookEvents()
        {
            this.listbox.KeyDown += new KeyEventHandler(this.Listbox_keyDown);
            this.listbox.DrawItem += new DrawItemEventHandler(this.Listbox_drawItem);
            this.listbox.SelectedIndexChanged += new EventHandler(this.Listbox_selectedIndexChanged);
            this.listbox.HandleCreated += new EventHandler(this.Listbox_handleCreated);
            this.upButton.Click += new EventHandler(this.UpButton_click);
            this.downButton.Click += new EventHandler(this.DownButton_click);
            this.propertyBrowser.PropertyValueChanged += new PropertyValueChangedEventHandler(this.PropertyGrid_propertyValueChanged);
            this.addButton.Click += new EventHandler(this.AddButton_click);
            this.removeButton.Click += new EventHandler(this.RemoveButton_click);
            this.okButton.Click += new EventHandler(this.OKButton_click);
            this.cancelButton.Click += new EventHandler(this.CancelButton_click);
            base.HelpButtonClicked += new CancelEventHandler(this.CollectionEditor_HelpButtonClicked);
            base.HelpRequested += new HelpEventHandler(this.Form_HelpRequested);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(CustomCollectionEditor));
            this.membersLabel = new Label();
            this.listbox = new CustomCollectionEditor.FilterListBox();
            this.upButton = new Button();
            this.downButton = new Button();
            this.propertiesLabel = new Label();
            this.propertyBrowser = new CustomPropertyGrid(base.Context);
            this.addButton = new CustomCollectionEditor.SplitButton();
            this.removeButton = new Button();
            this.okButton = new Button();
            this.cancelButton = new Button();

            base.SuspendLayout();

            this.SuspendLayout();
            // 
            // propertiesLabel
            // 
            this.propertiesLabel.AutoSize = true;
            this.propertiesLabel.AutoEllipsis = true;
            this.propertiesLabel.Location = new System.Drawing.Point(295, 9);
            this.propertiesLabel.Name = "propertiesLabel";
            this.propertiesLabel.Size = new System.Drawing.Size(35, 13);
            this.propertiesLabel.TabIndex = 0;
            this.propertiesLabel.Text = "label1";
            // 
            // membersLabel
            // 
            this.membersLabel.AutoSize = true;
            this.membersLabel.Location = new System.Drawing.Point(12, 9);
            this.membersLabel.Name = "membersLabel";
            this.membersLabel.Size = new System.Drawing.Size(50, 13);
            this.membersLabel.TabIndex = 1;
            this.membersLabel.Text = "Members";
            // 
            // listBox
            // 
            this.listbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listbox.Location = new System.Drawing.Point(12, 29);
            this.listbox.Name = "listBox";
            this.listbox.SelectionMode = this.CanSelectMultipleInstances() ? SelectionMode.MultiExtended : SelectionMode.One;
            this.listbox.DrawMode = DrawMode.OwnerDrawFixed;
            this.listbox.FormattingEnabled = true;
            this.listbox.Size = new System.Drawing.Size(220, 270);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(350, 327);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.DialogResult = DialogResult.OK;
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(431, 327);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // upButton
            // 
            this.upButton.Location = new System.Drawing.Point(240, 29);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(24, 24);
            this.upButton.TabIndex = 5;
            this.upButton.UseVisualStyleBackColor = true;
            // 
            // downButton
            // 
            this.downButton.Location = new System.Drawing.Point(240, 59);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(24, 24);
            this.downButton.TabIndex = 6;
            this.downButton.UseVisualStyleBackColor = true;
            // 
            // AddButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton.Location = new System.Drawing.Point(12, 297);
            this.addButton.Name = "AddButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 7;
            this.addButton.Text = "Добавить";
            this.addButton.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Location = new System.Drawing.Point(100, 297);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 8;
            this.removeButton.Text = "Удалить";
            this.removeButton.UseVisualStyleBackColor = true;
            // 
            // propertyBrowser
            // 
            this.propertyBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyBrowser.Location = new System.Drawing.Point(290, 29);
            this.propertyBrowser.Name = "propertyBrowser";
            this.propertyBrowser.Size = new System.Drawing.Size(220, 291);
            this.propertyBrowser.TabIndex = 9;


            /*

           // manager.ApplyResources(this.membersLabel, "membersLabel", CultureInfo.CurrentUICulture);
            this.membersLabel.Name = "membersLabel";
            this.membersLabel.Text = "Элементы";
            //  manager.ApplyResources(this.listbox, "listbox");
            this.listbox.SelectionMode = this.CanSelectMultipleInstances() ? SelectionMode.MultiExtended : SelectionMode.One;
            this.listbox.DrawMode = DrawMode.OwnerDrawFixed;
            this.listbox.FormattingEnabled = true;
            this.listbox.Name = "listbox";
            this.overArchingTableLayoutPanel.SetRowSpan(this.listbox, 2);
            //  manager.ApplyResources(this.upButton, "upButton");
            this.upButton.Name = "upButton";
            //  manager.ApplyResources(this.downButton, "downButton");
            this.downButton.Name = "downButton";
            // manager.ApplyResources(this.propertiesLabel, "propertiesLabel");
            this.propertiesLabel.AutoEllipsis = true;
            this.propertiesLabel.Name = "propertiesLabel";
            this.propertiesLabel.Text = "Свойства:";
            //  manager.ApplyResources(this.propertyBrowser, "propertyBrowser");
            this.propertyBrowser.CommandsVisibleIfAvailable = false;
            this.propertyBrowser.Name = "propertyBrowser";
            this.overArchingTableLayoutPanel.SetRowSpan(this.propertyBrowser, 3);
            //  manager.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Text = "Добавить";
            // manager.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.Name = "removeButton";
            this.removeButton.Text = "Удалить";
            //  manager.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Text = "OK";
            //  manager.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Text = "Отмена";
            // manager.ApplyResources(this.okCancelTableLayoutPanel, "okCancelTableLayoutPanel");

            this.okCancelTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okCancelTableLayoutPanel.ColumnCount = 2;
            this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okCancelTableLayoutPanel.RowCount = 1;
            this.okCancelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));

            this.overArchingTableLayoutPanel.SetColumnSpan(this.okCancelTableLayoutPanel, 3);
            this.okCancelTableLayoutPanel.Controls.Add(this.okButton, 0, 0);
            this.okCancelTableLayoutPanel.Controls.Add(this.cancelButton, 1, 0);
            this.okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";

            //  manager.ApplyResources(this.overArchingTableLayoutPanel, "overArchingTableLayoutPanel");
            this.overArchingTableLayoutPanel.ColumnCount = 4;
            this.overArchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.overArchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.overArchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.overArchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.overArchingTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overArchingTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.overArchingTableLayoutPanel.Name = "overArchingTableLayoutPanel";
            this.overArchingTableLayoutPanel.RowCount = 5;
            this.overArchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overArchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overArchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.overArchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overArchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overArchingTableLayoutPanel.Size = new System.Drawing.Size(550, 358);
            this.overArchingTableLayoutPanel.Controls.Add(this.downButton, 1, 2);
            this.overArchingTableLayoutPanel.Controls.Add(this.addRemoveTableLayoutPanel, 0, 3);
            this.overArchingTableLayoutPanel.Controls.Add(this.propertiesLabel, 2, 0);
            this.overArchingTableLayoutPanel.Controls.Add(this.membersLabel, 0, 0);
            this.overArchingTableLayoutPanel.Controls.Add(this.listbox, 0, 1);
            this.overArchingTableLayoutPanel.Controls.Add(this.propertyBrowser, 2, 1);
            this.overArchingTableLayoutPanel.Controls.Add(this.okCancelTableLayoutPanel, 0, 4);
            this.overArchingTableLayoutPanel.Controls.Add(this.upButton, 1, 1);
            this.overArchingTableLayoutPanel.Name = "overArchingTableLayoutPanel";


            //  manager.ApplyResources(this.addRemoveTableLayoutPanel, "addRemoveTableLayoutPanel");
            this.addRemoveTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addRemoveTableLayoutPanel.ColumnCount = 3;
            this.addRemoveTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.addRemoveTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.addRemoveTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.addRemoveTableLayoutPanel.RowCount = 1;
            this.addRemoveTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.addRemoveTableLayoutPanel.Controls.Add(this.addButton, 0, 0);
            this.addRemoveTableLayoutPanel.Controls.Add(this.removeButton, 2, 0);
            this.addRemoveTableLayoutPanel.Margin = new Padding(0, 3, 3, 3);
            this.addRemoveTableLayoutPanel.Name = "addRemoveTableLayoutPanel";*/

            base.AcceptButton = this.okButton;
            //   manager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.cancelButton;
          //  base.Controls.Add(this.overArchingTableLayoutPanel);
            base.ClientSize = new System.Drawing.Size(518, 359);
            base.Controls.Add(this.propertyBrowser);
            base.Controls.Add(this.removeButton);
            base.Controls.Add(this.addButton);
            base.Controls.Add(this.downButton);
            base.Controls.Add(this.upButton);
            base.Controls.Add(this.okButton);
            base.Controls.Add(this.cancelButton);
            base.Controls.Add(this.listbox);
            base.Controls.Add(this.membersLabel);
            base.Controls.Add(this.propertiesLabel);

            base.HelpButton = false;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CustomCollectionEditor";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;

            SetStartPosition();
            
            /*
            this.okCancelTableLayoutPanel.ResumeLayout(false);
            this.okCancelTableLayoutPanel.PerformLayout();
            this.overArchingTableLayoutPanel.ResumeLayout(false);
            this.overArchingTableLayoutPanel.PerformLayout();
            this.addRemoveTableLayoutPanel.ResumeLayout(false);
            this.addRemoveTableLayoutPanel.PerformLayout();*/
            this.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void SetStartPosition()
        {
            int newX = Cursor.Position.X - base.Width;
            int newY = Cursor.Position.Y;

            if (newX < 5)
            {
                newX = 5;
            }

            if ((newY + base.Height) > Screen.PrimaryScreen.WorkingArea.Height)
            {
                newY = Screen.PrimaryScreen.WorkingArea.Height - base.Height;
            }

            base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            base.Location = new Point(newX, newY);
 
        }

        private void Listbox_drawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                ListItem item = (ListItem)this.listbox.Items[e.Index];
                Graphics graphics = e.Graphics;
                int count = this.listbox.Items.Count;
                int num2 = (count > 1) ? (count - 1) : count;
                SizeF ef = graphics.MeasureString(num2.ToString(CultureInfo.CurrentCulture), this.listbox.Font);
                int num3 = ((int)(Math.Log((double)num2) / LOG10)) + 1;
                int num4 = 4 + (num3 * (this.Font.Height / 2));
                num4 = Math.Max(num4, (int)Math.Ceiling((double)ef.Width)) + (SystemInformation.BorderSize.Width * 4);
                Rectangle rectangle = new Rectangle(e.Bounds.X, e.Bounds.Y, num4, e.Bounds.Height);
                ControlPaint.DrawButton(graphics, rectangle, ButtonState.Normal);
                rectangle.Inflate(-SystemInformation.BorderSize.Width * 2, -SystemInformation.BorderSize.Height * 2);
                int num5 = num4;
                Color window = SystemColors.Window;
                Color windowText = SystemColors.WindowText;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    window = SystemColors.Highlight;
                    windowText = SystemColors.HighlightText;
                }
                Rectangle rect = new Rectangle(e.Bounds.X + num5, e.Bounds.Y, e.Bounds.Width - num5, e.Bounds.Height);
                graphics.FillRectangle(new SolidBrush(window), rect);
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    ControlPaint.DrawFocusRectangle(graphics, rect);
                }
                num5 += 2;
                if ((item.Editor != null) && item.Editor.GetPaintValueSupported())
                {
                    Rectangle rectangle3 = new Rectangle(e.Bounds.X + num5, e.Bounds.Y + 1, 20, e.Bounds.Height - 3);
                    graphics.DrawRectangle(SystemPens.ControlText, rectangle3.X, rectangle3.Y, rectangle3.Width - 1, rectangle3.Height - 1);
                    rectangle3.Inflate(-1, -1);
                    item.Editor.PaintValue(item.Value, graphics, rectangle3);
                    num5 += 0x1b;
                }
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    graphics.DrawString(e.Index.ToString(CultureInfo.CurrentCulture), this.Font, SystemBrushes.ControlText, new Rectangle(e.Bounds.X, e.Bounds.Y, num4, e.Bounds.Height), format);
                }
                Brush brush = new SolidBrush(windowText);
                string displayText = this.GetDisplayText(item);
                try
                {
                    graphics.DrawString(displayText, this.Font, brush, new Rectangle(e.Bounds.X + num5, e.Bounds.Y, e.Bounds.Width - num5, e.Bounds.Height));
                }
                finally
                {
                    if (brush != null)
                    {
                        brush.Dispose();
                    }
                }
                int num6 = num5 + ((int)graphics.MeasureString(displayText, this.Font).Width);
                if ((num6 > e.Bounds.Width) && (this.listbox.HorizontalExtent < num6))
                {
                    this.listbox.HorizontalExtent = num6;
                }
            }
        }

        private void Listbox_handleCreated(object sender, EventArgs e)
        {
            this.UpdateItemWidths(null);
        }

        private void Listbox_keyDown(object sender, KeyEventArgs kevent)
        {
            switch (kevent.KeyData)
            {
                case Keys.Insert:
                    this.PerformAdd();
                    return;

                case Keys.Delete:
                    this.PerformRemove();
                    return;
            }
        }

        private void Listbox_selectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateEnabled();
        }

        private void OKButton_click(object sender, EventArgs e)
        {
            /*
            try
            {
                if (!this.dirty || !this.CollectionEditable)
                {
                    this.dirty = false;
                    base.DialogResult = DialogResult.Cancel;
                }
                else
                {
                    if (this.dirty)
                    {
                        object[] objArray = new object[this.listbox.Items.Count];
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            objArray[i] = ((ListItem)this.listbox.Items[i]).Value;
                        }
                        base.Items = objArray;
                    }
                    /*
                    if ((this.removedItems != null) && this.dirty)
                    {
                        object[] objArray2 = this.removedItems.ToArray();
                        for (int j = 0; j < objArray2.Length; j++)
                        {
                            base.DestroyInstance(objArray2[j]);
                        }
                        this.removedItems.Clear();
                    }
                    if (this.createdItems != null)
                    {
                        this.createdItems.Clear();
                    }
                    if (this.originalItems != null)
                    {
                        this.originalItems.Clear();
                    }
                     
                    this.listbox.Items.Clear();
                    this.dirty = false;
                }
            }
            catch (Exception exception)
            {
                base.DialogResult = DialogResult.None;
                this.DisplayError(exception);
            }*/
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (!this.dirty)
            {
                foreach (object obj2 in this.originalItems)
                {
                    if (obj2 == e.Component)
                    {
                        this.dirty = true;
                        break;
                    }
                }
            }
        }

        protected override void OnEditValueChanged()
        {
            if (this.originalItems == null)
            {
                this.originalItems = new ArrayList();
            }
            this.originalItems.Clear();
            this.listbox.Items.Clear();
            this.propertyBrowser.Site = new CustomCollectionEditor.PropertyGridSite(base.Context, this.propertyBrowser);
            if (base.EditValue != null)
            {
                this.SuspendEnabledUpdates();
                try
                {
                    object[] items = base.Items;
                    for (int i = 0; i < items.Length; i++)
                    {
                        this.listbox.Items.Add(new ListItem(this.editor, items[i]));
                        this.originalItems.Add(items[i]);
                    }
                    if (this.listbox.Items.Count > 0)
                    {
                        this.listbox.SelectedIndex = 0;
                    }
                }
                finally
                {
                    this.ResumeEnabledUpdates(true);
                }
            }
            else
            {
                this.UpdateEnabled();
            }
            this.AdjustListBoxItemHeight();
            this.UpdateItemWidths(null);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.AdjustListBoxItemHeight();
        }

        private void PerformAdd()
        {
            if (this.CollectionEditable)
            {
                this.CreateAndAddInstance(base.NewItemTypes[0]);
            }
        }

        private void PerformRemove()
        {
            if (!this.CollectionEditable)
            {
                return;
            }

            int selectedIndex = this.listbox.SelectedIndex;
            if (selectedIndex != -1)
            {
                this.SuspendEnabledUpdates();
                try
                {
                    if (this.listbox.SelectedItems.Count > 1)
                    {
                        ArrayList list = new ArrayList(this.listbox.SelectedItems);
                        foreach (ListItem item in list)
                        {
                            this.RemoveInternal(item);
                        }
                    }
                    else
                    {
                        this.RemoveInternal((ListItem)this.listbox.SelectedItem);
                    }
                    if (selectedIndex < this.listbox.Items.Count)
                    {
                        this.listbox.SelectedIndex = selectedIndex;
                    }
                    else if (this.listbox.Items.Count > 0)
                    {
                        this.listbox.SelectedIndex = this.listbox.Items.Count - 1;
                    }
                }
                finally
                {
                    this.ResumeEnabledUpdates(true);
                }
            }
        }

        private void PropertyGrid_propertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            this.dirty = true;
            this.SuspendEnabledUpdates();
            try
            {
                this.listbox.RefreshItem(this.listbox.SelectedIndex);
            }
            finally
            {
                this.ResumeEnabledUpdates(false);
            }
            this.UpdateItemWidths(null);
            this.listbox.Invalidate();
            this.propertiesLabel.Text = "Свойства"; ///SR.GetString("CollectionEditorProperties", new object[] { this.GetDisplayText((ListItem)this.listbox.SelectedItem) });
        }

        private void RemoveButton_click(object sender, EventArgs e)
        {
            this.PerformRemove();
            Control control = (Control)sender;
            if (control.Enabled)
            {
                control.Focus();
            }
        }

        private void RemoveInternal(ListItem item)
        {
            if (item != null)
            {
                this.editor.OnItemRemoving(item.Value);
                this.dirty = true;
                if ((this.createdItems != null) && this.createdItems.Contains(item.Value))
                {
                    base.DestroyInstance(item.Value);
                    this.createdItems.Remove(item.Value);
                    this.listbox.Items.Remove(item);
                }
                else
                {
                    try
                    {
                        if (!base.CanRemoveInstance(item.Value))
                        {
                            throw new Exception("Не удалось удалить элемент"); ///SR.GetString("CollectionEditorCantRemoveItem", new object[] { this.GetDisplayText(item) }));
                        }
                        if (this.removedItems == null)
                        {
                            this.removedItems = new ArrayList();
                        }
                        this.removedItems.Add(item.Value);
                        this.listbox.Items.Remove(item);
                    }
                    catch (Exception exception)
                    {
                        this.DisplayError(exception);
                    }
                }
                this.UpdateItemWidths(null);
            }
        }

        private void ResumeEnabledUpdates(bool updateNow)
        {
            this.suspendEnabledCount--;
            if (updateNow)
            {
                this.UpdateEnabled();
            }
            else
            {
                base.BeginInvoke(new MethodInvoker(this.UpdateEnabled));
            }
        }

        protected internal override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
        {
            IComponentChangeService service = null;
            DialogResult oK = DialogResult.OK;
            try
            {
                service = (IComponentChangeService)this.editor.Context.GetService(typeof(IComponentChangeService));
                if (service != null)
                {
                    service.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
                }
                base.ActiveControl = this.listbox;
                oK = base.ShowEditorDialog(edSvc);
            }
            finally
            {
                if (service != null)
                {
                    service.ComponentChanged -= new ComponentChangedEventHandler(this.OnComponentChanged);
                }
            }
            return oK;
        }

        private void SuspendEnabledUpdates()
        {
            this.suspendEnabledCount++;
        }

        private void UpButton_click(object sender, EventArgs e)
        {
            int selectedIndex = this.listbox.SelectedIndex;
            if (selectedIndex != 0)
            {
                this.dirty = true;
                try
                {
                    this.SuspendEnabledUpdates();
                    int topIndex = this.listbox.TopIndex;
                    object obj2 = this.listbox.Items[selectedIndex];
                    this.listbox.Items[selectedIndex] = this.listbox.Items[selectedIndex - 1];
                    this.listbox.Items[selectedIndex - 1] = obj2;
                    if (topIndex > 0)
                    {
                        this.listbox.TopIndex = topIndex - 1;
                    }
                    this.listbox.ClearSelected();
                    this.listbox.SelectedIndex = selectedIndex - 1;
                    Control control = (Control)sender;
                    if (control.Enabled)
                    {
                        control.Focus();
                    }
                }
                finally
                {
                    this.ResumeEnabledUpdates(true);
                }
            }
        }

        private void UpdateEnabled()
        {
            if (this.suspendEnabledCount <= 0)
            {
                bool flag = (this.listbox.SelectedItem != null) && this.CollectionEditable;
                this.removeButton.Enabled = flag && this.AllowRemoveInstance(((ListItem)this.listbox.SelectedItem).Value);
                this.upButton.Enabled = flag && (this.listbox.Items.Count > 1);
                this.downButton.Enabled = flag && (this.listbox.Items.Count > 1);
                this.propertyBrowser.Enabled = flag;
                this.addButton.Enabled = this.CollectionEditable;
                if (this.listbox.SelectedItem == null)
                {
                    this.propertiesLabel.Text = "Свойства"; /// SR.GetString("CollectionEditorPropertiesNone");
                    this.propertyBrowser.SelectedObject = null;
                }
                else
                {
                    object[] objArray;
                    if (this.IsImmutable)
                    {
                        objArray = new object[] { new SelectionWrapper(base.CollectionType, base.CollectionItemType, this.listbox, this.listbox.SelectedItems) };
                    }
                    else
                    {
                        objArray = new object[this.listbox.SelectedItems.Count];
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            object value = ((ListItem)this.listbox.SelectedItems[i]).Value;
                            if (value is Legend)
                            {
                                value = new MapLegendBrowseClass((Legend)value);
                            }
                            /*if (value is Layer)
                            {
                                value = new MapLayerBrowseClass((Layer)value);
                            }*/
                            if (value is CustomColor)
                            {
                                value = new CustomColorBrowseClass((CustomColor)value);
                            }
                            objArray[i] = value;
                        }
                    }
                    switch (this.listbox.SelectedItems.Count)
                    {
                        case 1:
                        case -1:
                            this.propertiesLabel.Text = "Свойства " + this.GetDisplayText((ListItem)this.listbox.SelectedItem) + ":";  ///SR.GetString("CollectionEditorProperties", new object[] { this.GetDisplayText((ListItem)this.listbox.SelectedItem) });
                            break;

                        default:
                            this.propertiesLabel.Text = "Свойства:"; /// SR.GetString("CollectionEditorPropertiesMultiSelect");
                            break;
                    }
                    if (this.editor.IsAnyObjectInheritedReadOnly(objArray))
                    {
                        this.propertyBrowser.SelectedObjects = null;
                        this.propertyBrowser.Enabled = false;
                        this.removeButton.Enabled = false;
                        this.upButton.Enabled = false;
                        this.downButton.Enabled = false;
                        this.propertiesLabel.Text = "Свойства:"; /// SR.GetString("CollectionEditorInheritedReadOnlySelection");
                    }
                    else
                    {
                        this.propertyBrowser.Enabled = true;
                        this.propertyBrowser.SelectedObjects = objArray;
                    }
                }
            }
        }

        private void UpdateItemWidths(ListItem item)
        {
            if (this.listbox.IsHandleCreated)
            {
                using (Graphics graphics = this.listbox.CreateGraphics())
                {
                    int horizontalExtent = this.listbox.HorizontalExtent;
                    if (item != null)
                    {
                        int num2 = this.CalcItemWidth(graphics, item);
                        if (num2 > horizontalExtent)
                        {
                            this.listbox.HorizontalExtent = num2;
                        }
                    }
                    else
                    {
                        int num3 = 0;
                        foreach (ListItem item2 in this.listbox.Items)
                        {
                            int num4 = this.CalcItemWidth(graphics, item2);
                            if (num4 > num3)
                            {
                                num3 = num4;
                            }
                        }
                        this.listbox.HorizontalExtent = num3;
                    }
                }
            }
        }

        // Properties
        private bool IsImmutable
        {
            get
            {
                if (!TypeDescriptor.GetConverter(base.CollectionItemType).GetCreateInstanceSupported())
                {
                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(base.CollectionItemType))
                    {
                        if (!descriptor.IsReadOnly)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public MainForm MainForm
        {
            get { return mainForm; }
            set { mainForm = value; }
        }

        // Nested Types
        private class ListItem
        {
            // Fields
            private CustomCollectionEditor parentCollectionEditor;
            private object uiTypeEditor;
            private object value;

            // Methods
            public ListItem(CustomCollectionEditor parentCollectionEditor, object value)
            {
                this.value = value;
                this.parentCollectionEditor = parentCollectionEditor;
            }

            public override string ToString()
            {
                return this.parentCollectionEditor.GetDisplayText(this.value);
            }

            // Properties
            public UITypeEditor Editor
            {
                get
                {
                    if (this.uiTypeEditor == null)
                    {
                        this.uiTypeEditor = TypeDescriptor.GetEditor(this.value, typeof(UITypeEditor));
                        if (this.uiTypeEditor == null)
                        {
                            this.uiTypeEditor = this;
                        }
                    }
                    if (this.uiTypeEditor != this)
                    {
                        return (UITypeEditor)this.uiTypeEditor;
                    }
                    return null;
                }
            }

            public object Value
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.uiTypeEditor = null;
                    this.value = value;
                }
            }
        }

        private class SelectionWrapper : PropertyDescriptor, ICustomTypeDescriptor
        {
            // Fields
            private ICollection collection;
            private Type collectionItemType;
            private Type collectionType;
            private Control control;
            private PropertyDescriptorCollection properties;
            private object value;

            // Methods
            public SelectionWrapper(Type collectionType, Type collectionItemType, Control control, ICollection collection)
                : base("Value", new Attribute[] { new CategoryAttribute(collectionItemType.Name) })
            {
                this.collectionType = collectionType;
                this.collectionItemType = collectionItemType;
                this.control = control;
                this.collection = collection;
                this.properties = new PropertyDescriptorCollection(new PropertyDescriptor[] { this });
                this.value = this;
                foreach (CustomCollectionEditorCollectionForm.ListItem item in collection)
                {
                    if (this.value == this)
                    {
                        this.value = item.Value;
                        continue;
                    }
                    object obj2 = item.Value;
                    if (this.value != null)
                    {
                        if (obj2 == null)
                        {
                            this.value = null;
                        }
                        else
                        {
                            if (this.value.Equals(obj2))
                            {
                                continue;
                            }
                            this.value = null;
                        }
                        break;
                    }
                    if (obj2 != null)
                    {
                        this.value = null;
                        break;
                    }
                }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return this.value;
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
                this.value = value;
                foreach (CustomCollectionEditorCollectionForm.ListItem item in this.collection)
                {
                    item.Value = value;
                }
                this.control.Invalidate();
                this.OnValueChanged(component, EventArgs.Empty);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                return TypeDescriptor.GetAttributes(this.collectionItemType);
            }

            string ICustomTypeDescriptor.GetClassName()
            {
                return this.collectionItemType.Name;
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return null;
            }

            TypeConverter ICustomTypeDescriptor.GetConverter()
            {
                return null;
            }

            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
                return null;
            }

            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
                return this;
            }

            object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
                return null;
            }

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return EventDescriptorCollection.Empty;
            }

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                return EventDescriptorCollection.Empty;
            }

            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                return this.properties;
            }

            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                return this.properties;
            }

            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                return this;
            }

            // Properties
            public override Type ComponentType
            {
                get
                {
                    return this.collectionType;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return this.collectionItemType;
                }
            }
        }

        private class TypeMenuItem : ToolStripMenuItem
        {
            // Fields
            private Type itemType;

            // Methods
            public TypeMenuItem(Type itemType, EventHandler handler)
                : base(itemType.Name, null, handler)
            {
                this.itemType = itemType;
            }

            // Properties
            public Type ItemType
            {
                get
                {
                    return this.itemType;
                }
            }
        }

        internal class CustomPropertyGrid : PropertyGrid
        {
            // Methods
            public CustomPropertyGrid(IServiceProvider serviceProvider)
            {
                if (serviceProvider != null)
                {
                    IUIService service = serviceProvider.GetService(typeof(IUIService)) as IUIService;
                    if (service != null)
                    {
                        base.ToolStripRenderer = (ToolStripProfessionalRenderer)service.Styles["VsToolWindowRenderer"];
                    }
                }
            }
        }

        [Flags]
        public enum EditorButtons
        {
            None = 0,
            AddRemove = 1,
            UpDown = 2,
        }

    }

}
