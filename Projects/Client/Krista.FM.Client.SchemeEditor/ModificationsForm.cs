using System;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor
{
    /// <summary>
    /// Форма применения изменений
    /// </summary>
    internal partial class ModificationsForm : Form
    {
        /// <summary>
        /// Инициализация формы
        /// </summary>
        public ModificationsForm()
        {
            InitializeComponent();

            Krista.FM.Client.Common.DefaultFormState.Load(this);

            modificationsTreeControl.ReadOnly = true;

            btnOK.Enabled = false;
        }

        /// <summary>
        /// Обработчик на изменение комментария. Используется для активации кнопри "Применить"
        /// </summary>
        public event EnableEventHandler CommentsChanged;

        /// <summary>
        /// Текст комментария
        /// </summary>
        internal string Comments
        {
            get { return textBoxComments.Text; }
        }

        /// <summary>
        /// Дерево изменений
        /// </summary>
        internal ModificationsTreeControl ModificationsTreeControl
        {
            get { return modificationsTreeControl; }
        }

        /// <summary>
        /// Обработчик на изменения комментария
        /// </summary>
        private void textBoxComments_TextChanged(object sender, EventArgs e)
        {
            if (CommentsChanged != null)
            {
                EnableEventArgs args = new EnableEventArgs(Comments);
                CommentsChanged(this, args);
                btnOK.Enabled = args.Enable;
            }
        }

        private void ModificationsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }
    }

    /// <summary>
    /// Параметры обработчика изменения комментария.
    /// </summary>
    public class EnableEventArgs : EventArgs
    {
        private bool enable = false;
        private string text;

        /// <summary>
        /// Инициализация параметров обработчика изменения комментария.
        /// </summary>
        /// <param name="text">Измененный текст комментария</param>
        public EnableEventArgs(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Определяет доступность кнопки "Применить"
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        /// <summary>
        /// Текст комментария
        /// </summary>
        public string Text
        {
            get { return text; }
        }
    }

    /// <summary>
    /// Прототип обработчика изменения комментария к изменениям
    /// </summary>
    /// <param name="sender">Отправитель события</param>
    /// <param name="e">Параметры события</param>
    public delegate void EnableEventHandler(object sender, EnableEventArgs e);
}