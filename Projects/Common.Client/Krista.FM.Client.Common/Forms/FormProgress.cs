using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using Infragistics.Win.Misc;

namespace Krista.FM.Client.Common.Forms
{
	/// <summary>
	/// Форма прогресса. Для внешних модулей недоступна.
	/// </summary>
	internal class frmProgress : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Button btnCancel;
        private UltraLabel laInfo;
		private PictureBox pbAnimation;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar pbProgress;
        private UltraLabel laCaption;
		private System.ComponentModel.Container components = null;
		#region Конструктор/Деструктор
		/// <summary>
		/// Коструктор класса
		/// </summary>
		public frmProgress()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Деструктор класса. Очистка ресурсов.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.btnCancel = new System.Windows.Forms.Button();
            this.laInfo = new UltraLabel();
            this.pbAnimation = new System.Windows.Forms.PictureBox();
            this.pbProgress = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.laCaption = new UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnCancel.Location = new System.Drawing.Point(400, 83);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // laInfo
            // 
            this.laInfo.ForeColor = System.Drawing.Color.MediumBlue;
            this.laInfo.Location = new System.Drawing.Point(105, 26);
            this.laInfo.Name = "laInfo";
            this.laInfo.Size = new System.Drawing.Size(371, 48);
            this.laInfo.TabIndex = 3;
            this.laInfo.Text = "laInfo";
            this.laInfo.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
            this.laInfo.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle; 
            // 
            // pbAnimation
            // 
            this.pbAnimation.Image = global::Krista.FM.Client.Common.Properties.Resources.Шестеренки;
            this.pbAnimation.InitialImage = null;
            this.pbAnimation.Location = new System.Drawing.Point(8, 8);
            this.pbAnimation.Name = "pbAnimation";
            this.pbAnimation.Size = new System.Drawing.Size(87, 66);
            this.pbAnimation.TabIndex = 6;
            this.pbAnimation.TabStop = false;
            // 
            // pbProgress
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BorderColor = System.Drawing.Color.MediumBlue;
            appearance1.ForeColor = System.Drawing.Color.MediumBlue;
            this.pbProgress.Appearance = appearance1;
            this.pbProgress.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance2.BackColor = System.Drawing.Color.MediumBlue;
            this.pbProgress.FillAppearance = appearance2;
            this.pbProgress.FlatMode = /*Infragistics.Win.DefaultableBoolean.True;/*/ true;
            this.pbProgress.Location = new System.Drawing.Point(8, 83);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(384, 23);
            this.pbProgress.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Segmented;
            this.pbProgress.TabIndex = 7;
            this.pbProgress.Text = "[Formatted]";
            // 
            // laCaption
            // 
            this.laCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.laCaption.ForeColor = System.Drawing.Color.MediumBlue;
            this.laCaption.Location = new System.Drawing.Point(104, 8);
            this.laCaption.Name = "laCaption";
            this.laCaption.Size = new System.Drawing.Size(371, 16);
            this.laCaption.TabIndex = 8;
            this.laCaption.Text = "_";
            this.laCaption.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
            this.laCaption.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            // 
            // frmProgress
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(488, 119);
            this.ControlBox = false;
            this.Controls.Add(this.laCaption);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.pbAnimation);
            this.Controls.Add(this.laInfo);
            this.Controls.Add(this.btnCancel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmProgress";
            this.Opacity = 0.8;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmProgress_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimation)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
		#region Тривиальные методы получения/установки свойств элементов формы
		/// <summary>
		/// Обработчик закрытия формы. Отклоняет все запросы на закрытие.
		/// </summary>
		/// <param name="sender">Поставщик события</param>
		/// <param name="e">Параметры события</param>
		private void frmProgress_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// закрывать эту форму нельзя
			e.Cancel = true;	
		}

		// максимальное значение прогресса (минимальное всегда 0)
		public int GetMaxProgress()
		{
			return pbProgress.Maximum;
		}

		// Установка максимального значения прогресса
		public void SetMaxProgress(int Val)
		{
			pbProgress.Maximum = Val;
		}

		// Получение текущей позиции прогресса
		public int GetPosition()
		{
			return pbProgress.Value;
		}

		// Установка текущей позиции прогресса
		public void SetPosition(int Val)
		{
			pbProgress.Value = Val;
			/*if (pbProgress.Maximum != 0)
			{
				laStep.Text = string.Format("({0:d} из {1:d})", new object[] {Val, pbProgress.Maximum});		
				laPercent.Text = string.Format("{0:d}%", new object[] {Val * 100 / pbProgress.Maximum});
			}*/
		}

		// Отображение формы на экране
		public void StartProgress(bool Cancelable)
		{
			btnCancel.Enabled = Cancelable;
			Canceled = false;
			this.Show();
			this.TopMost = true;
			this.TopMost = false;
		}

		// Сокрытие прогресса
		public void StopProgress()
		{
            pbProgress.Value = 0;
            this.Hide();
		}

		// Возвращает заголовок формы
		public string GetCaption()
		{
			//return this.Text;
            return laCaption.Text;
		}

		// Устанавливает заголовок формы
		public void SetCaption(string Val)
		{
			//this.Text = Val;
            laCaption.Text = Val;
		}

		// Получение текста прогресса
		public string GetText()
		{
			return laInfo.Text;
		}

		// Установка текста прогресса
		public void SetText(string Val)
		{
			laInfo.Text = Val;
		}

		#endregion
		// Выход из нити
		public void ReleaseThread()
		{
			Application.ExitThread();
		}

		// Внешний обработчик события нажатия на кнопку "Отмена"
		public event VoidDelegate OnCancel;
		// Признак отмены процесса пользователем
		public bool Canceled = false;
		// Внутренний обработчик события нажатия на кнопку "Отмена"
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Canceled = true;
			StopProgress();
			if (OnCancel != null) OnCancel();
		}

	}
	
	/// <summary>
	/// Объект "Прогресс"
	/// </summary>
	public class Progress
	{
		// Экземпляр формы прогресса. Работает в отдельной нити.
		private frmProgress ProgressForm;
		// Нить в которой работает форма прогресса
		private Thread thrd;
		#region Делегаты для безопасного вызова методов формы в многопоточном режиме.
		// Назначение понятно из названий...
		private GetIntDelegate GetMaxProgressDelegate;
		private SetIntDelegate SetMaxProgressDelegate;
		private GetIntDelegate GetPositionDelegate;
		private SetIntDelegate SetPositionDelegate;
		private SetBoolDelegate StartProgressDelegate;
		private VoidDelegate StopProgressDelegate;
		private GetStringDelegate GetCaptionDelegate;
		private SetStringDelegate SetCaptionDelegate;
		private GetStringDelegate GetTextDelegate;
		private SetStringDelegate SetTextDelegate;
		private VoidDelegate ReleaseDelegate;
		#endregion
		// Конструктор класса
		public Progress()
		{
			// создаем новую нить в которой будет функционировать форма
			thrd = new Thread(new ThreadStart(this.ThreadEntryPoint));
            thrd.Priority = ThreadPriority.Highest;
			// запускаем ее
			thrd.Start();
			// в текущей версии .NET Framework нельзя по-простому создать форму невидимой.
			// Поэтому:
			// 1) ждем пока нить загрузится
			while (thrd.ThreadState != System.Threading.ThreadState.Running) Thread.Sleep(50);
			// 2) ждем пока создастся форма
			while (ProgressForm == null)Thread.Sleep(50);
			// 3) ждем пока проинициализируется instance формы
			while ((!ProgressForm.IsHandleCreated))	Thread.Sleep(10);
			// 4) инициализируем делегаты для вызова методов формы в многопоточном режиме
			GetMaxProgressDelegate = new GetIntDelegate(ProgressForm.GetMaxProgress);
			SetMaxProgressDelegate = new SetIntDelegate(ProgressForm.SetMaxProgress);
			GetPositionDelegate = new GetIntDelegate(ProgressForm.GetPosition);
			SetPositionDelegate = new SetIntDelegate(ProgressForm.SetPosition);
			StartProgressDelegate = new SetBoolDelegate(ProgressForm.StartProgress);
			StopProgressDelegate = new VoidDelegate(ProgressForm.StopProgress);
			GetCaptionDelegate = new GetStringDelegate(ProgressForm.GetCaption);
			SetCaptionDelegate = new SetStringDelegate(ProgressForm.SetCaption);
			GetTextDelegate = new GetStringDelegate(ProgressForm.GetText);
			SetTextDelegate = new SetStringDelegate(ProgressForm.SetText);
			ReleaseDelegate = new VoidDelegate(ProgressForm.ReleaseThread);
			// 5) прячем форму
			StopProgress();
		}
		// Процедура проверки готовности формы
		private void CheckThread()
		{
			// если нить была завершена - генерируем исключение
			if (thrd == null) throw(new Exception("Progress thread has been released"));
		}
		// Главная процедура нити
		private void ThreadEntryPoint()
		{
			// создаем экземпляр формы прогресса
			ProgressForm = new frmProgress();
			// запускаем для него цикл обработки сообщений
			Application.Run(ProgressForm);						
		}
		#region Общие методы объекта "Прогресс"
		/// <summary>
		/// Запуск прогреса
		/// </summary>
 		public void StartProgress()
		{
			CheckThread();
			ProgressForm.Invoke(StartProgressDelegate, new object[] {false});
            Application.DoEvents();
		}
		/// <summary>
		/// Запуск прогресса
		/// </summary>
		/// <param name="OnCancel">Обработчик события нажатия на кнопку "Отмена"</param>
 		public void StartProgress(VoidDelegate OnCancel)
		{
			CheckThread();
			ProgressForm.Invoke(StartProgressDelegate, new object[] {true});
			ProgressForm.OnCancel += OnCancel;
		}
		/// <summary>
		/// Остновка прогресса, сокрытие формы
		/// </summary>
		public void StopProgress()
		{
			CheckThread();
			ProgressForm.Invoke(StopProgressDelegate);
		}
		/// <summary>
		/// Максимальное значение прогресса (минимальное всегда 0)
		/// </summary>
		public int MaxProgress
		{
			get
			{
				CheckThread();
				return (int)ProgressForm.Invoke(GetMaxProgressDelegate);
			}
			set
			{
				CheckThread();
				ProgressForm.Invoke(SetMaxProgressDelegate, new object[] {value});
			}
		}
		/// <summary>
		/// Текущая позиция прогресса
		/// </summary>
		public int Position
		{
			get
			{
				CheckThread();
				return (int)ProgressForm.Invoke(GetPositionDelegate);
			}
			set 
			{
				CheckThread();
				ProgressForm.Invoke(SetPositionDelegate, new object[] {value});
			}
		}
		/// <summary>
		/// Заголовок формы
		/// </summary>
		public string Caption
		{
			get
			{
				CheckThread();
				return (string)ProgressForm.Invoke(GetCaptionDelegate);
			}
			set 
			{
				CheckThread();
				ProgressForm.Invoke(SetCaptionDelegate, new object[] {value});
			}
		}
		/// <summary>
		/// Надпись на прогрессе
		/// </summary>
		public string Text
		{
			get
			{
				CheckThread();
				return (string)ProgressForm.Invoke(GetTextDelegate);
			}
			set 
			{
				CheckThread();
				ProgressForm.Invoke(SetTextDelegate, new object[] {value});
			}
		}
		/// <summary>
		/// Признак окончания процесса по желанию пользователя (нажатие на кнопку "Отмена")
		/// </summary>
		public bool Canceled
		{
			get
			{
				CheckThread();
				return ProgressForm.Canceled;
			}
		}
		/// <summary>
		/// Завершение нити в которой размещен цикл обработки сообщений формы прогресса
		/// Без вызова этого метода приложение НЕ ЗАВЕРШИТЬСЯ !!!
		/// </summary>
		public void ReleaseThread()
		{
			CheckThread();
			ProgressForm.Invoke(ReleaseDelegate);
			thrd = null;
		}
		#endregion
	}
}
