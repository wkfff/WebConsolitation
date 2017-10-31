using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;
using Krista.FM.Common;
using Krista.FM.Common.Exceptions;


namespace Krista.FM.Client.SchemeEditor
{
    public partial class DeveloperDescriptionControl : UserControl
    {
        /// <summary>
        /// Выделенный объект
        /// </summary>
        private IServerSideObject selectedObject;

        /// <summary>
        /// Признак, изменен текст или нет
        /// </summary>
        private bool isChanged = false;

        public DeveloperDescriptionControl()
        {
            InitializeComponent();

            InitializeContextMenu();

            this.richTextBox.DetectUrls = true;
            this.richTextBox.LinkClicked += new LinkClickedEventHandler(richTextBox_LinkClicked);
        }

        /// <summary>
        /// Переход по гиперссылке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch (ObjectDisposedException ex)
            {
                throw new LinkClickedException(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                throw new LinkClickedException(ex.Message);
            }
            catch (Win32Exception ex)
            {
                throw new LinkClickedException(ex.Message);
            }
        }

        #region Контекстное меню

        private void InitializeContextMenu()
        {
            this.richTextBox.ContextMenu = new ContextMenu();

            MenuItem undo = new MenuItem();
            undo.Name = "Undo";
            undo.Text = "Отменить";
            undo.Enabled = (richTextBox.CanUndo) ? true : false;
            undo.Click += new EventHandler(menu_Click);

            MenuItem cut = new MenuItem();
            cut.Name = "Cut";
            cut.Text = "Вырезать";
            cut.Enabled = (String.IsNullOrEmpty(richTextBox.SelectedText)) ? false : true;
            cut.Click += new EventHandler(menu_Click);

            MenuItem copy = new MenuItem();
            copy.Name = "Copy";
            copy.Text = "Копировать";
            copy.Enabled = (String.IsNullOrEmpty(richTextBox.SelectedText)) ? false : true;
            copy.Click += new EventHandler(menu_Click);

            MenuItem paste = new MenuItem();
            paste.Name = "Paste";
            paste.Text = "Вставить";
            paste.Click += new EventHandler(menu_Click);

            MenuItem delete = new MenuItem();
            delete.Name = "Delete";
            delete.Text = "Удалить";
            delete.Enabled = (String.IsNullOrEmpty(richTextBox.SelectedText)) ? false : true;
            delete.Click += new EventHandler(menu_Click);

            MenuItem selectAll = new MenuItem();
            selectAll.Name = "SelectAll";
            selectAll.Text = "Выделить всё";
            selectAll.Enabled = (String.IsNullOrEmpty(richTextBox.Text)) ? false : true;
            selectAll.Click += new EventHandler(menu_Click);

            this.richTextBox.ContextMenu.MenuItems.AddRange(new MenuItem[] {undo, new MenuItem("-"), cut, copy, paste, delete, new MenuItem("-"), selectAll});
        }

        void menu_Click(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                switch (item.Name)
                {
                    case "Undo":
                        richTextBox.Undo();
                        break;
                    case "Cut":
                        richTextBox.Cut();
                        break;
                    case "Copy":
                        richTextBox.Copy();
                        break;
                    case "Paste" :
                        richTextBox.Paste();
                        break;
                    case "Delete":
                        richTextBox.SelectedText = String.Empty;
                        break;
                    case "SelectAll" :
                        richTextBox.SelectAll();
                        break;
                    default:
                        throw new Exception(String.Format("Обработчик для {0} не реализован", item.Name));
                }
            }
        }

        private void richTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                InitializeContextMenu();

                richTextBox.ContextMenu.Show(richTextBox, new Point(e.X, e.Y));
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
                        richTextBox.Rtf = String.Empty;

                        selectedObject = null;
                        if (tempselectedObject is ICommonDBObject)
                            richTextBox.Rtf = ((ICommonDBObject)tempselectedObject).DeveloperDescription;
                        else if (tempselectedObject is IDataAttribute)
                            richTextBox.Rtf = ((IDataAttribute)tempselectedObject).DeveloperDescription;
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
        private void richTextBox_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (selectedObject != null && isChanged)
            {
                if (selectedObject is ICommonDBObject)
                    ((ICommonDBObject)selectedObject).DeveloperDescription = richTextBox.Rtf;
                else if (selectedObject is IDataAttribute)
                    ((IDataAttribute)selectedObject).DeveloperDescription = richTextBox.Rtf;
                isChanged = false;
            }
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            isChanged = true;
        }        
    }
}
