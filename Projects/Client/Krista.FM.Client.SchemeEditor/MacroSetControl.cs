using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor
{
    public partial class MacroSetControl : UserControl
    {
        /// <summary>
        /// Выделенный объект
        /// </summary>
        private IServerSideObject selectedObject;

        /// <summary>
        /// Признак, изменен текст или нет
        /// </summary>
        private bool isChanged = false;

        public MacroSetControl()
        {
            InitializeComponent();

            InitializeContextMenu();
        }

        #region Контекстное меню

        private void InitializeContextMenu()
        {
            this.highlightEdit.ContextMenu = new ContextMenu();

            MenuItem undo = new MenuItem();
            undo.Name = "Undo";
            undo.Text = "Отменить";
            undo.Enabled = (highlightEdit.CanUndo) ? true : false;
            undo.Click += new EventHandler(menu_Click);

            MenuItem cut = new MenuItem();
            cut.Name = "Cut";
            cut.Text = "Вырезать";
            cut.Enabled = (String.IsNullOrEmpty(highlightEdit.SelectedText)) ? false : true;
            cut.Click += new EventHandler(menu_Click);

            MenuItem copy = new MenuItem();
            copy.Name = "Copy";
            copy.Text = "Копировать";
            copy.Enabled = (String.IsNullOrEmpty(highlightEdit.SelectedText)) ? false : true;
            copy.Click += new EventHandler(menu_Click);

            MenuItem paste = new MenuItem();
            paste.Name = "Paste";
            paste.Text = "Вставить";
            paste.Click += new EventHandler(menu_Click);

            MenuItem delete = new MenuItem();
            delete.Name = "Delete";
            delete.Text = "Удалить";
            delete.Enabled = (String.IsNullOrEmpty(highlightEdit.SelectedText)) ? false : true;
            delete.Click += new EventHandler(menu_Click);

            MenuItem selectAll = new MenuItem();
            selectAll.Name = "SelectAll";
            selectAll.Text = "Выделить всё";
            selectAll.Enabled = (String.IsNullOrEmpty(highlightEdit.Text)) ? false : true;
            selectAll.Click += new EventHandler(menu_Click);

            this.highlightEdit.ContextMenu.MenuItems.AddRange(new MenuItem[] {undo, new MenuItem("-"), cut, copy, paste, delete, new MenuItem("-"), selectAll});
        }

        void menu_Click(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                switch (item.Name)
                {
                    case "Undo":
                        highlightEdit.Undo();
                        break;
                    case "Cut":
                        highlightEdit.Cut();
                        break;
                    case "Copy":
                        highlightEdit.Copy();
                        break;
                    case "Paste" :
                        highlightEdit.Paste();
                        break;
                    case "Delete":
                        highlightEdit.SelectedText = String.Empty;
                        break;
                    case "SelectAll" :
                        highlightEdit.SelectAll();
                        break;
                    default:
                        throw new Exception(String.Format("Обработчик для {0} не реализован", item.Name));
                }
            }
        }

        private void highlightEdit_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                InitializeContextMenu();

                highlightEdit.ContextMenu.Show(highlightEdit, new Point(e.X, e.Y));
            }
        }

        #endregion Контекстное меню

        /// <summary>
        /// Выделенный объект
        /// </summary>
        public IServerSideObject SelectedObject
        {
            set 
            { 
                selectedObject = value;

                if (selectedObject != null)
                {
                    IServerSideObject tempselectedObject = selectedObject;
                    try
                    {
                        highlightEdit.Rtf = String.Empty;

                        selectedObject = null;
                        if (tempselectedObject is IEntity)
                            highlightEdit.Rtf = ((IEntity) tempselectedObject).MacroSet;
                        isChanged = false;
                    }
                    finally
                    {
                        selectedObject = tempselectedObject;
                    }
                }
            }
        }

        /// <summary>
        /// Теряет фокус
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void highlightEdit_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (selectedObject != null && isChanged)
            {
                if (selectedObject is IEntity)
                    ((IEntity)selectedObject).MacroSet = highlightEdit.Rtf;
                isChanged = false;
            }
        }

        private void highlightEdit_TextChanged(object sender, EventArgs e)
        {
            isChanged = true;
        }        
    }
}
