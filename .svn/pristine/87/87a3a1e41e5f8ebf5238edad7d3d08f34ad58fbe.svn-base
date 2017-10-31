using System;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor.Tools
{
    /// <summary>
    /// Управление зумом
    /// </summary>
    public partial class ZoomControl : UserControl
    {
        #region Fields
        
        /// <summary>
        /// Масштаб текущей диаграммы
        /// </summary>
        private int scaleFactor;

        /// <summary>
        /// Таймер,запускается при нажатии ну кнопки +/-
        /// </summary>
        private Timer timer = new Timer();

        /// <summary>
        /// Нажата кнопка
        /// </summary>
        private bool downbuttonDown = false;

        private bool upbuttonDown = false;

        #endregion

        #region Constructor

        public ZoomControl()
        {
            InitializeComponent();

            this.comboZoom.SelectionChanged += new EventHandler(SelectionChanged);
            this.comboZoom.KeyDown += new KeyEventHandler(ComboZoom_KeyDown);

            timer.Enabled = false;
            timer.Interval = 100;
            timer.Tick += new EventHandler(Timer_Tick);

            DisplayScale();
        }

        #endregion

        public delegate void ScaleEventHandler(object sender, ScaleEventArgs args);

        /// <summary>
        /// Событие для изменения маштаба
        /// </summary>
        public event ScaleEventHandler ScaleChangeEvent;
        
        public int ScaleFactor
        {
            get
            {
                return scaleFactor;
            }

            set
            {
                scaleFactor = value;

                DisplayScale();
                CallEventHandler();
            }
        }

        /// <summary>
        /// Завершение операции
        /// </summary>
        public void FinalizeOperation()
        {
            downbuttonDown = false;
            upbuttonDown = false;
            timer.Enabled = false;
        }

        /// <summary>
        /// Старт операции увеличения масштаба
        /// </summary>
        public void StartUpOperation()
        {
            timer.Enabled = true;
            upbuttonDown = true;
        }

        /// <summary>
        /// Старт операции уменьшения масштаба
        /// </summary>
        public void StartDownOperation()
        {
            timer.Enabled = true;
            downbuttonDown = true;
        }

        public void ScaleChange(int index)
        {
            try
            {
                ValidateText(string.Format("{0}", scaleFactor + index));
            }
            catch
            {
                // ..глушим исключение
                FinalizeOperation();
            }
        }

        /// <summary>
        /// Обновляет строку масштаба
        /// </summary>
        private void DisplayScale()
        {
            this.comboZoom.Text = String.Format("{0}%", scaleFactor);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (downbuttonDown)
                {
                    this.ScaleChange(-10);
                }
                else if (upbuttonDown)
                {
                    this.ScaleChange(10);
                }
            }
            catch
            {
                // Перехватываем исключения.. и глушим
                FinalizeOperation();
            }
        }

        /// <summary>
        /// Проверяем наличие слушателей
        /// </summary>
        private void OnChangeScale(ScaleEventArgs args)
        {
            if (ScaleChangeEvent != null)
            {
                // Уведомляем всех абонентов
                ScaleChangeEvent(this, args);
            }
        }

        /// <summary>
        /// Нажимаем ВВОД
        /// </summary>
        private void ComboZoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidateText(comboZoom.Text);
            }
        }

        /// <summary>
        /// Ракция на событие
        /// </summary>
        private void CallEventHandler()
        {
            ScaleEventArgs args = new ScaleEventArgs(scaleFactor);

            // Вот и всё!
            OnChangeScale(args);
        }

        /// <summary>
        /// Проверка вводимых символов
        /// </summary>
        private void ValidateText(string p)
        {
            if (String.IsNullOrEmpty(p))
            {
                return;
            }

            int z = 0;

            if (p[p.Length - 1] == '%')
            {
                p = p.Substring(0, p.Length - 1);
            }

            try
            {
                z = int.Parse(p);
            }
            catch (FormatException e)
            {
                DisplayScale();

                // генерируем искл, но не перехватываем
                throw new Exception("Неккоректный формат масштаба " + e);
            }

            if (z < 10 || z > 400)
            {
                DisplayScale();

                // генерируем искл, но не перехватываем
                throw new Exception("Число должно быть между 10 и 400");
            }

            scaleFactor = int.Parse(p);

            DisplayScale();
            CallEventHandler();
        }

        /// <summary>
        /// Изменение строки комбобокса
        /// </summary>
        private void SelectionChanged(object sender, EventArgs e)
        {
            switch (comboZoom.Text)
            {
                case "200%":
                    scaleFactor = 200;
                    break;
                case "150%":
                    scaleFactor = 150;
                    break;
                case "100%":
                    scaleFactor = 100;
                    break;
                case "75%":
                    scaleFactor = 75;
                    break;
                case "50%":
                    scaleFactor = 50;
                    break;
                case "25%":
                    scaleFactor = 25;
                    break;
                case "Вся диаграмма":
                    // Для расчета оптимального масштаба будем передавать -1 в качестве аргумента
                    ScaleEventArgs args = new ScaleEventArgs(-1);
                    OnChangeScale(args);

                    // вычисляем размер
                    return;
            }

            CallEventHandler();
        }
    }
}
