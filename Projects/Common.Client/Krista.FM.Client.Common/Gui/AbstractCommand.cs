using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.Common.Gui
{
    /// <summary>
    /// Абстрактная реализация интерфейса <see cref="ICommand"/>.
    /// </summary>
    public abstract class AbstractCommand : ICommand
    {
        private object owner = null;

        public virtual object Owner
        {
            get { return owner; }
            set
            {
                owner = value;
                OnOwnerChanged(EventArgs.Empty);
            }
        }

    	protected string key;

		/// <summary>
		/// Ключ команды.
		/// </summary>
    	public string Key
    	{
			get { return key; }
    	}

		protected string caption;

		/// <summary>
		/// Наименование команды выводимое в интерфейса пользователя.
		/// </summary>
		public string Caption
		{
			get { return caption; }
		}

		protected Shortcut shortcut;

		/// <summary>
		/// Горячая клавиша.
		/// </summary>
		public Shortcut Shortcut
    	{
    		get { return shortcut; }
    	}

		protected string iconKey = String.Empty;

		/// <summary>
		/// Иконка.
		/// </summary>
		public string IconKey
		{
			get { return iconKey; }
		}

		bool isEnabled = true;

		/// <summary>
		/// Доступность команды.
		/// </summary>
		public virtual bool IsEnabled
		{
			get
			{
				return isEnabled;
			}
			set
			{
				isEnabled = value;
				OnStateChanged(new EventArgs());
			}
		}

		/// <summary>
        /// Выполняет команду.
        /// </summary>
        public abstract void Run();


        protected virtual void OnOwnerChanged(EventArgs e)
        {
            if (OwnerChanged != null)
            {
                OwnerChanged(this, e);
            }
        }

        public event EventHandler OwnerChanged;

		protected virtual void OnStateChanged(EventArgs e)
		{
			if (StateChanged != null)
			{
				StateChanged(this, e);
			}
		}

		public event EventHandler StateChanged;
	}
}
