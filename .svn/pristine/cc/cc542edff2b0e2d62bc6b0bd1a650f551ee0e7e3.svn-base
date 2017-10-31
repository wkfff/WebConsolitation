using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class ScrollView : UserControl
    {
        /// <summary>
        /// До этого значения (мс) движение считается резким
        /// </summary>
        const int jerk = 500;

        #region Делегаты
        public delegate void ControlChangedHandler(object sender, int index, Control control);
        #endregion

        #region События
        private event ControlChangedHandler _beforeControlChanged;
        private event ControlChangedHandler _afterControlChanged;
        private event EventHandler _scrollIndicatorClick;

        /// <summary>
        /// Происходит перед выбором контрола
        /// </summary>
        public event ControlChangedHandler BeforeControlChanged
        {
            add { _beforeControlChanged += value; }
            remove { _beforeControlChanged -= value; }
        }

        /// <summary>
        /// Происходит после выбора контрола
        /// </summary>
        public event ControlChangedHandler AfterControlChanged
        {
            add { _afterControlChanged += value; }
            remove { _afterControlChanged -= value; }
        }

        /// <summary>
        /// Происходит при нажатии на область индикации
        /// </summary>
        public event EventHandler ScrollIndicatorClick
        {
            add { _scrollIndicatorClick += value; }
            remove { _scrollIndicatorClick -= value; }
        }
        #endregion

        #region Поля
        private int startMovedTime;
        private Point startPointClick;
        private Point startPanelLocation;
        private int movedOffset;
        private DirectionMode directionMouse;

        private int _changedControlIndex;
        private bool _enabledScroll;
        #endregion

        #region Свойства
        /// <summary>
        /// Количество контролов на ленте
        /// </summary>
        public int Count
        {
            get { return this.RibbonControls.Count; }
        }

        /// <summary>
        /// Пустой
        /// </summary>
        public bool Empty
        {
            get { return this.Count <= 0; }
        }

        /// <summary>
        /// Все контролы ленты
        /// </summary>
        public ControlCollection RibbonControls
        {
            get { return this.ribbon.Controls; }
        }

        /// <summary>
        /// Индекс выбраного контрола
        /// </summary>
        public int ChangedControlIndex
        {
            get 
            { 
                return (this.Count > 0) ? _changedControlIndex : -1; 
            }
            set
            {
                if ((this.Count > 0) && (value >= 0) && (value < this.Count))
                {
                    this.OnBeforeControlChanged(value, this.RibbonControls[value]);

                    this._scrollIndicator.ChangedControlIndex = value;
                    _changedControlIndex = value;
                    this.ribbon.Left = this.RibbonControls[value].Left * -1;
                    this.OnAfterControlChanged();
                }
            }
        }

        /// <summary>
        /// Выбранный контрол
        /// </summary>
        public Control ChangedControl
        {
            get { return this.Count > 0 ? this.RibbonControls[this.ChangedControlIndex] : null; }
            set 
            {
                if ((value != null) && this.RibbonControls.Contains(value))
                    this.ChangedControlIndex = this.RibbonControls.IndexOf(value);
            }
        }

        /// <summary>
        /// Следующий контрол после выбраного
        /// </summary>
        public Control NextControl
        {
            get 
            {
                Control result = null;
                int nextControlIndex = this.ChangedControlIndex + 1;
                if (nextControlIndex  < this.Count)
                    result = this.RibbonControls[nextControlIndex];
                return result;
            }
        }

        /// <summary>
        /// Работает ли скролл
        /// </summary>
        public bool EnabledScroll
        {
            get { return _enabledScroll; }
            set 
            { 
                _enabledScroll = value;;
                this.ScrollIndicator.Visible = value;
            }
        }

        /// <summary>
        /// Индикатор скролирования
        /// </summary>
        public ScrollIndicator ScrollIndicator
        {
            get { return _scrollIndicator; }
            set { _scrollIndicator = value; }
        }

        public Size WorkingArea
        {
            get 
            {
                this.ribbon.Height = this.EnabledScroll ? this.Height - this.ScrollIndicator.Height : this.Height;
                Size size = new Size(this.Width, this.ribbon.Height);
                return size; 
            }
        }
        #endregion

        public ScrollView()
        {
            InitializeComponent();
            this.SetDefaultValue();
        }

        /// <summary>
        /// Выставляем значения поумолчанию
        /// </summary>
        private void SetDefaultValue()
        {
            this.startPointClick = Point.Empty;
            this.startPanelLocation = Point.Empty;
            this.ribbon.Height = this.Height;
            this.EnabledScroll = true;
        }

        /// <summary>
        /// Добавить контрол на ленту
        /// </summary>
        /// <param name="value">контрол</param>
        /// <param name="resizeControls">пересчитать размеры контролов</param>
        public void AddControl(Control value, bool resizeControls)
        {
            if (value == null)
                return;

            this.ribbon.Controls.Add(value);
           
            value.MouseDown += new MouseEventHandler(this.ribbon_MouseDown);
            value.MouseMove += new MouseEventHandler(this.ribbon_MouseMove);
            value.MouseUp += new MouseEventHandler(this.ribbon_MouseUp);

            if (resizeControls)
                this.ResizeControls();
        }

        /// <summary>
        /// Удалить контрол с ленты
        /// </summary>
        /// <param name="value">контрол</param>
        /// <param name="resizeControls">пересчитать размеры контролов</param>
        public void RemoveControl(Control value, bool resizeControls)
        {
            if (value == null)
                return;

            value.MouseDown -= this.ribbon_MouseDown;
            value.MouseMove -= this.ribbon_MouseMove;
            value.MouseUp -= this.ribbon_MouseUp;
            this.ribbon.Controls.Remove(value);

            if (resizeControls)
                this.ResizeControls();
        }

        /// <summary>
        /// Очистить коллекцию контролов
        /// </summary>
        public void Clear()
        {
            try
            {
                this.SuspendLayout();

                for (int i = this.Count - 1; i >= 0; i--)
                {
                    this.RemoveControl(this.RibbonControls[i], (i == 0));
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        public void ChangeNextControl()
        {
            this.ChangedControlIndex++;
        }

        public void ChangePrepareControl()
        {
            this.ChangedControlIndex--;
        }

        /// <summary>
        /// Выровнять позицию ленты по текущему выделенному контролу
        /// </summary>
        /// <param name="item">контрол</param>
        public void AlignByChangedControl()
        {
            this.AlignByControl(this.ChangedControl);
        }

        /// <summary>
        /// Выровнять позицию ленты по контролу
        /// </summary>
        /// <param name="item">контрол</param>
        public void AlignByControl(Control item)
        {
            if ((item != null) && this.RibbonControls.Contains(item))
            {
                this.ribbon.Left = item.Left * -1;
            }
        }

        private void AutomateSecectControl()
        {
            //Когда добавим первый контрол, сразу сфокусируемся на нем
            if (this.Count == 1)
            {
                this.ResizeControls();
                this.ChangedControlIndex = 0;
            }
        }

        /// <summary>
        /// Расчитать позиции всех контролов
        /// </summary>
        public void ResizeControls()
        {
            this.SuspendLayout();
            try
            {
                this._scrollIndicator.ControlCount = this.Count;
                this.ribbon.Width = this.GetControlsWidth();
                this.SetControlsLocation();
            }
            finally
            {
                this.ResumeLayout(false);
            }
        }

        /// <summary>
        /// Ширина всех контролов на ленте
        /// </summary>
        /// <returns></returns>
        private int GetControlsWidth()
        {
            int result = 0;
            for (int i = 0; i < this.Count; i++) 
            {
                Control item = this.RibbonControls[i];
                result += item.Width;
            }
            return result;
        }

        /// <summary>
        /// Установим начальные позиции контролов
        /// </summary>
        private void SetControlsLocation()
        {
            int currentX = 0;
            for (int i = 0; i < this.Count; i++)
            {
                Control item = this.RibbonControls[i];
                item.Location = new Point(currentX, 0);
                currentX += item.Width;
            }
        }

        /// <summary>
        /// Плавное сдвигаем ленту, фокусируя на выбранный контрол
        /// </summary>
        /// <param name="selectedControlIndex">индекс выбраного контрола</param>
        /// <param name="totalMovedTime">время перемещения ленты (от него зависти плавность)</param>
        private void SmoothMotionRibbon(int selectedControlIndex, int totalMovedTime)
        {
            int s = Screen.PrimaryScreen.WorkingArea.Width / 5;
            int goalX = this.RibbonControls[selectedControlIndex].Left * -1;

            while (this.ribbon.Left != goalX)
            {
                if (this.ribbon.Left > goalX)
                {
                    this.ribbon.Left -= s;
                    if (this.ribbon.Left < goalX)
                        this.ribbon.Left = goalX;
                }
                else
                {
                    this.ribbon.Left += s;
                    if (this.ribbon.Left > goalX)
                        this.ribbon.Left = goalX;
                }
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Вычисляем выделенный контрол
        /// </summary>
        /// <param name="mouseArgs">аргументы мышки</param>
        /// <param name="movedOffset">длина перемещения мышки</param>
        /// <returns></returns>
        private int DetermineChangedControlIndex(MouseEventArgs mouseArgs, int movedOffset)
        {
            if (movedOffset > 10)
            {
                switch (this.directionMouse)
                {
                    case DirectionMode.ToLeft:
                        {
                            if (this.ChangedControlIndex + 1 < this.Count)
                                return this.ChangedControlIndex + 1;
                            break;
                        }
                    case DirectionMode.ToRight:
                        {
                            if (this.ChangedControlIndex - 1 >= 0)
                                return this.ChangedControlIndex - 1;
                            break;
                        }
                }
            }
            /*
            else
            {
                //середина экрана
                int halfScreen = Screen.PrimaryScreen.WorkingArea.Width / 2;
                return (halfScreen > mouseArgs.X) ? this.ChangedControlIndex - 1 : this.ChangedControlIndex + 1;
            }
            */
            return this.ChangedControlIndex;
        }

        /* Когда можно было передвигать ленту использовал ниже лежащий алгоритм
        /// <summary>
        /// Вычисляем выделенный контрол
        /// </summary>
        /// <param name="totalMovedTime">время перемещения ленты</param>
        /// <returns></returns>
        private int DetermineChangedControlIndex(int totalMovedTime)
        {
            //если движение резкое, то в зависимости от направления выбираем следующий контрол
            if (totalMovedTime <= jerk)
            {
                switch (this.directionMouse)
                {
                    case DirectionMode.ToLeft: 
                        {
                            if (this.ChangedControlIndex + 1 < this.Count)
                                return this.ChangedControlIndex + 1;
                            break;
                        }
                    case DirectionMode.ToRight:
                        {
                            if (this.ChangedControlIndex - 1 >= 0)
                                return this.ChangedControlIndex - 1;
                            break;
                        }
                }
                //если условия не выполняются, то скролирование не возможно
                //возвращаем текущее значение
                return this.ChangedControlIndex;
            }

            //середина экрана
            int halfScreen = Screen.PrimaryScreen.WorkingArea.Width / 2;
            //точка лежащая по середине экрана
            Point point = new Point(this.ribbon.Left * -1 + halfScreen, this.ribbon.Top);
            //возвращаем хозяина точки
            return OwnPointControlIndex(point);
        }*/

        /// <summary>
        /// Вернет индекс контрола, которому принадлежит указаная точка
        /// </summary>
        /// <returns></returns>
        private int OwnPointControlIndex(Point point)
        {
            for (int i = 0; i < this.Count; i++)
            {
                Control item = this.RibbonControls[i];
                if (item.Bounds.Contains(point))
                    return i;
            }
            return this.ChangedControlIndex;
        }

        /// <summary>
        /// Запомним координаты клика, и ленты
        /// </summary>
        private void StartMove(MouseEventArgs e)
        {
            this.startPointClick = new Point(e.X, e.Y);
            this.startPanelLocation = this.ribbon.Location;
            this.directionMouse = DirectionMode.None;
            this.startMovedTime = Environment.TickCount;

            //На время передвижения, приостановим отрисовку контролов на ленте
            Utils.SetControlsLayout(this.RibbonControls, false, true);
            this.SuspendLayout();
        }

        /// <summary>
        /// Двигаем ленту
        /// </summary>
        private void RibbonMove(MouseEventArgs e)
        {
            //найдем координату оси X у мышки отностилено экрана, Control.MousePosition.X - не работает (ХЗ)
            int mouseX = e.X + (this.ribbon.Left + this.RibbonControls[this.ChangedControlIndex].Left);
            this.movedOffset = this.startPointClick.X - mouseX;// Control.MousePosition.X;

            DirectionMode prepareDirectionMouse = this.directionMouse;
            if (this.movedOffset == 0)
                this.directionMouse = DirectionMode.None;
            else
                this.directionMouse = this.movedOffset > 0 ? DirectionMode.ToLeft : DirectionMode.ToRight;
            //Если направление движения поменялось, сбросим таймер
            if ((prepareDirectionMouse != DirectionMode.None) && (prepareDirectionMouse != this.directionMouse))
                this.startMovedTime = Environment.TickCount;

            this.movedOffset = Math.Abs(this.movedOffset);
            /*
            int ribbonOffset = 0;

            switch (this.directionMouse)
            {
                case DirectionMode.ToLeft:
                    {
                        ribbonOffset = this.startPanelLocation.X - this.movedOffset;
                        this.ribbon.Location = new Point(ribbonOffset, this.ribbon.Location.Y);
                        break;
                    }
                case DirectionMode.ToRight:
                    {
                        ribbonOffset = this.startPanelLocation.X + this.movedOffset;
                        this.ribbon.Location = new Point(ribbonOffset, this.ribbon.Location.Y);
                        break;
                    }
            }*/
        }

        /// <summary>
        /// Закончили перемещение
        /// </summary>
        private void EndMove(MouseEventArgs e)
        {
            //итоговое время перетаскивания (в миллисекундах)
            int totalMovedTime = Environment.TickCount - this.startMovedTime;
            int controlIndex = this.DetermineChangedControlIndex(e, this.movedOffset);
            //плавное доведение контрола до границ экрана
            //this.SmoothMotionRibbon(controlIndex, totalMovedTime);
            this.ChangedControlIndex = controlIndex;

            //Закончили передвижение, возобновим отрисовку контролов на ленте
            Utils.SetControlsLayout(this.RibbonControls, true, true);
            this.ResumeLayout();
        }

        protected virtual void OnBeforeControlChanged(int index, Control control)
        {
            if (this._beforeControlChanged != null)
                this._beforeControlChanged(this, index, control);
        }

        protected virtual void OnAfterControlChanged()
        {
            if (this._afterControlChanged != null)
                this._afterControlChanged(this, this.ChangedControlIndex, this.ChangedControl);
        }

        protected virtual void OnScrollIndicatorClick()
        {
            if (this._scrollIndicatorClick != null)
                this._scrollIndicatorClick(this, new EventArgs());
        }

        #region Обработчики событий
        private void ribbon_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
            if (this.EnabledScroll && (this.RibbonControls.Count > 0))
                this.StartMove(e);
        }

        private void ribbon_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.EnabledScroll && (this.RibbonControls.Count > 0))
                this.RibbonMove(e);
        }

        private void ribbon_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnMouseUp(e);
            if (this.EnabledScroll && (this.RibbonControls.Count > 0))
                this.EndMove(e);
        }

        private void _scrollIndicator_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnScrollIndicatorClick();
        }
        #endregion

        /// <summary>
        /// Направление движения курсора
        /// </summary>
        private enum DirectionMode
        {
            ToLeft,
            ToRight,
            None
        }
    }
}
