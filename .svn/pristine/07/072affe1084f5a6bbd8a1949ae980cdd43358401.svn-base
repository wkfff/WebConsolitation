using System;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;


namespace Krista.FM.Client.ViewObjects.BaseViewObject
{
	/// <summary>
	/// Родительский класс для объектов просмотра (интерфейсов пользователя)
	/// </summary>
	public abstract class BaseViewObj : BaseViewContentPane
	{
        /// <summary>
        /// Уникальный ключ объекта просмотра.
        /// </summary>
	    private string key;

		// для отложенной загрузки - объект инициализируется при первом обращении к нему пользователя
		private bool _Initialized = false;

        /// <summary>
        /// Название модуля, отображается на вкладке воркплэйса
        /// </summary>
        public string Caption;

        // интерфейс Workplace, инициализируется при загрузке
		public IWorkplace Workplace;

		// Порядковый номер интерфейса в пределах сборки (временный вариант)
		public int Index = 1000;

		// картинка для воркплэйса 16х16
		public virtual System.Drawing.Image TypeImage16
		{
			get { return null; }
		}

		// картинка для воркплэйса 32х32
		public virtual System.Drawing.Image TypeImage24
		{
			get { return null; }
		}

		// Объект просмотра
		protected BaseView fViewCtrl;
		// В этом методе потомки должны создать экземпяр элемента просмотра
		protected abstract void SetViewCtrl();
		/// <summary>
		/// Объект просмотра, помещается в левую часть воркплэйса
		/// </summary>
		public BaseView ViewCtrl
		{
			get
			{
				return fViewCtrl;
			}
		}

        public override Control Control
        {
            get { return fViewCtrl; }
        }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		public BaseViewObj(string key)
		{
		    this.key = key;
		}

		// свойство, индицируюещее загруженность объекта
		public bool Initialized
		{
			get { return _Initialized; }
		}
		
        /// <summary>
		/// Вызывается при активации объекта пользователем. Все свойства уже доступны
		/// </summary>
		public virtual void Initialize()
		{
            // .. и объекта просмотра
            SetViewCtrl();

			_Initialized = true;
			fViewCtrl.Customize();
		}

		/// <summary>
		/// Выполняется инициализация (наполнение) интерфейса данными из базы данных.
		/// </summary>
		public virtual void InitializeData()
		{
		}

		/// <summary>
		/// Вызывается при выгрузке объекта (например при закрытии Workplace)
		/// </summary>
		public virtual void InternalFinalize()
		{
			_Initialized = false;
			fViewCtrl.Dispose();
		}

		/// <summary>
		/// Метод для перезагрузки (обновления) данных
		/// </summary>
		public virtual void ReloadData()
		{
		}

		/// <summary>
		/// Метод для сохранения данных
		/// </summary>
		public virtual void SaveChanges()
		{
		}

		/// <summary>
		/// Метод для отмены сделанных пользователем изменений
		/// </summary>
		public virtual void CancelChanges()
		{
		}

		/// <summary>
		/// Метод, сигнализирующий об активации/деактивации интерфейса
		/// </summary>
		/// <param name="Activated"></param>
		public virtual void Activate(bool Activated) 
		{
		}
    
        /// <summary>
        /// Свойство возвращает true, если объект поддерживает offline режимм работы без подключения к схеме
        /// </summary>
        public virtual bool SupportOfflineMode
        {
            get
            {
                return false;
            }
        }

        protected bool _canDeactivate = true;
        /// <summary>
        /// метод, проверяющий, можно ли перейти с текущего интерфейса на другой
        /// </summary>
        /// <returns></returns>
        public virtual bool CanDeactivate
        {
            get
            {
                return _canDeactivate;
            }
            set
            {
                _canDeactivate = value;
            }
        }

        /// <summary>
        /// Можно ли выгрузить текущий интерфейс
        /// </summary>
        public virtual bool CanUnload
        {
            get
            {
                return true;
            }
        }

	    /// <summary>
	    /// Название объекта, отображается на вкладке воркплэйса
	    /// </summary>
	    public virtual string FullCaption
	    {
	        get
	        {
	            return Caption;
	        }
	    }

	    /// <summary>
	    /// Уникальный ключ объекта просмотра (не должен использоваться как ObjectKey объектов схемы).
	    /// </summary>
	    public override string Key
	    {
	        get { return key; }
	    }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (fViewCtrl != null) 
					fViewCtrl.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
