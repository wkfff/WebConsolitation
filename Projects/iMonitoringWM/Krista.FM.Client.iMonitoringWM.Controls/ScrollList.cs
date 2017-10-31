using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class ScrollList : Control
    {
        #region События
        public delegate void SelectedItemHandler(object sender, ScrollListItem item);

        private event SelectedItemHandler _selectedItem;

        /// <summary>
        /// Выбрали элемент из списка
        /// </summary>
        public event SelectedItemHandler SelectedItem
        {
            add { _selectedItem += value; }
            remove { _selectedItem -= value; }
        }
        #endregion

        #region Поля
        private Pen separatePen;
        private Bitmap _bmDoubleBuffer;
        private int itemsHeight;
        private ScrollListItem draggingItem;

        //засечка времени нажатие мышки
        private int startClickTime;
        //смещение от начала нажатия до его окончания
        private int movedOffset;
        //стартовая позиция нажатия мышки, относительно экрана
        private int mouseClickPosition;
        //стартовая позиция нажатия мышки, абсолютная с учетом смещения
        private int offsetMouseClickPosition;
        //направление движения мышкой
        private DirectionMode mouseDirection;

        private StateControl prepareState;
        private int _vOffset;
        private StateControl _currentState;
        private ImageList _imageList;
        private List<ScrollListItem> _items;
        #endregion

        #region Свойства
        public StateControl CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }

        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }

        public int VOffset
        {
            get { return _vOffset; }
            set { _vOffset = value; }
        }

        public List<ScrollListItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        private Bitmap DoubleBufferImage
        {
            get
            {
                if (_bmDoubleBuffer == null)
                    _bmDoubleBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                return _bmDoubleBuffer;
            }
            set
            {
                if (_bmDoubleBuffer != null)
                    _bmDoubleBuffer.Dispose();
                _bmDoubleBuffer = value;
            }
        }

        /// <summary>
        /// Перещет координат требуется, если элементво больше нуля, а их высота до 
        /// сих пор равна нулю
        /// </summary>
        private bool IsNeedRecalculate
        {
            get { return (this.Items.Count > 0) && (this.itemsHeight == 0); }
        }
        #endregion

        public ScrollList()
        {
            InitializeComponent();
            this.Items = new List<ScrollListItem>();
            this.ImageList = new ImageList();
            this.VOffset = 0;
            this.separatePen = new Pen(Color.LightGray);
            this.CurrentState = StateControl.None;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (this.IsNeedRecalculate)
                this.RecalculatedItemsLocation();

            using (Graphics g = Graphics.FromImage(this.DoubleBufferImage))
            {
                g.Clear(this.BackColor);

                foreach (ScrollListItem item in this.Items)
                {
                    //если это перетаскиваемый элемент, отрисуем его позже
                    if (item.Visible && (this.draggingItem != item))
                        this.PaintItem(g, item);
                }
                this.PaintItem(g, this.draggingItem);
            }
            pe.Graphics.DrawImage(_bmDoubleBuffer, 0, 0);
            base.OnPaint(pe);
        }

        private void PaintItem(Graphics g, ScrollListItem item)
        {
            if (item != null)
            {
                int itemTop = item.Top;
                item.Top = itemTop + this.VOffset;
                //будем отрисоывать, только если элемент находится в зоне видимости
                if (this.ClientRectangle.IntersectsWith(item.Bounds))
                {
                    item.Paint(g);
                    //рисуем разделитель элементов скролла
                    g.DrawLine(this.separatePen, 0, item.Top - 1, this.Width, item.Top - 1);
                    g.DrawLine(this.separatePen, 0, item.Bottom + 1, this.Width, item.Bottom + 1);
                }
                item.Top = itemTop;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.DoubleBufferImage = null;
            foreach (ScrollListItem item in this.Items)
            {
                item.ResetBounds();
                item.Width = this.Width;
            }
        }

        private Point GetPointWithOffset(Point pointWitoutOffset)
        {
            return new Point(pointWitoutOffset.X, pointWitoutOffset.Y + Math.Abs(this.VOffset));
        }

        /// <summary>
        /// Меняем порядок элементов
        /// </summary>
        /// <param name="pointWithOffset"></param>
        private void DragItem(Point pointWithOffset)
        {
            if (this.draggingItem != null)
            {
                ScrollListItem selectedItem = this.GetItemByPoint(pointWithOffset);

                if ((selectedItem != null) && (selectedItem != this.draggingItem))
                {
                    int selectedItemIndex = this.Items.IndexOf(selectedItem);
                    this.Items.Remove(this.draggingItem);
                    this.Items.Insert(selectedItemIndex, this.draggingItem);
                    this.RecalculatedItemsLocation();
                }

                //половина высоты
                int halfHeight = this.draggingItem.Height / 2;
                this.draggingItem.Top = pointWithOffset.Y - halfHeight;

                //требуется ли скролирование вниз
                bool isNeedScrollToDown = (this.draggingItem.Bottom < this.itemsHeight) 
                    && (this.draggingItem.Bottom + halfHeight > (this.Height - this.VOffset));
                //требуется ли скролирование вверх
                bool isNeedScrollToUpper = !isNeedScrollToDown && 
                    (this.draggingItem.Top - halfHeight + this.VOffset < 0) && (this.VOffset < 0);

                if (isNeedScrollToDown || isNeedScrollToUpper)
                {
                    this.VOffset += isNeedScrollToDown ? 
                        this.draggingItem.Height / -6 : this.draggingItem.Height / 6;
                    this.Invalidate();
                    Application.DoEvents();
                    if (this.CurrentState == StateControl.DraggingItem)
                        this.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 100, 0,
                            this.PointToClient(new Point(0, Control.MousePosition.Y)).Y, 0));
                }
                else
                    this.Invalidate();
            }
        }

        /// <summary>
        /// Обнаруживаем перетаскиваемый элемент, если такой вообще есть
        /// </summary>
        /// <param name="e"></param>
        private void DetectDragItem(MouseEventArgs e)
        {
            Point pointWithOffset = this.GetPointWithOffset(new Point(e.X, e.Y));
            ScrollListItem selectedItem = this.GetItemByPoint(pointWithOffset);
            if (selectedItem != null)
            {
                bool isDragItem = selectedItem.IsBelongEditBox(pointWithOffset);
                if (isDragItem)
                {
                    this.CurrentState = StateControl.DraggingItem;
                    this.draggingItem = selectedItem;
                    this.DragItem(pointWithOffset);
                }
            }
        }

        private void ScrollingItems(MouseEventArgs e)
        {
            this.movedOffset = this.mouseClickPosition - e.Y;
            DirectionMode prepareDirectionMouse = this.mouseDirection;

            if (this.movedOffset == 0)
                this.mouseDirection = DirectionMode.None;
            else
                this.mouseDirection = this.mouseClickPosition < e.Y ? DirectionMode.ToDown : DirectionMode.ToUp;
            //Если направление движения поменялось, сбросим таймер
            if ((prepareDirectionMouse != DirectionMode.None) && (prepareDirectionMouse != this.mouseDirection))
            {
                this.startClickTime = Environment.TickCount;
                this.mouseClickPosition = e.Y;
            }
            this.movedOffset = Math.Abs(this.movedOffset);

            this.VOffset = this.offsetMouseClickPosition + e.Y;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.prepareState = this.CurrentState;
            this.CurrentState = StateControl.None;

            this.mouseClickPosition = e.Y;
            this.offsetMouseClickPosition = this.VOffset - e.Y;
            this.startClickTime = Environment.TickCount;
            this.mouseDirection = DirectionMode.None;

            this.DetectDragItem(e);

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            switch (this.CurrentState)
            {
                case StateControl.DraggingItem:
                    {
                        this.DragItem(this.GetPointWithOffset(new Point(e.X, e.Y)));
                        break;
                    }
                case StateControl.Scrolling:
                    {
                        this.ScrollingItems(e);
                        break;
                    }
                default:
                    {
                        //смещение мышки относительно нажатия
                        int clickOffset = Math.Abs(Math.Abs(this.VOffset - this.offsetMouseClickPosition) - e.Y);

                        Point pointWithOffset = this.GetPointWithOffset(new Point(e.X, e.Y));
                        ScrollListItem selectedItem = this.GetItemByPoint(pointWithOffset);
                        int halfItemHeight = 10;
                        if (selectedItem != null)
                            halfItemHeight = (int)((float)selectedItem.Height / 3f);
                        //если смещение не значительное и мы не прервали инерционное скролирование
                        //значит пользователь хочет нажать кнопку
                        if ((clickOffset <= halfItemHeight) && (this.prepareState != StateControl.InertialScrolling))
                        {
                            this.CurrentState = StateControl.SelectingItem;
                        }
                        //иначе пользователь листает список
                        else
                        {
                            this.ScrollingItems(e);
                            this.CurrentState = StateControl.Scrolling;
                        }
                        break;
                    }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            switch (this.CurrentState)
            {
                case StateControl.DraggingItem:
                    {
                        this.EndDraggingItem();
                        this.CurrentState = StateControl.None;
                        break;
                    }
                case StateControl.SelectingItem:
                    {
                        this.SelectItem(e);
                        this.CurrentState = StateControl.None;
                        break;
                    }
                case StateControl.Scrolling:
                    {
                        //начнем иннерционное скролирование
                        Thread t = new Thread(new ThreadStart(this.InertialScroll));
                        t.Priority = ThreadPriority.Highest;
                        t.Start();
                        break;
                    }
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Инерционное пролистывание списка 
        /// </summary>
        private void InertialScroll()
        {
            //лимит выхода за пределы границ
            int offBounds = Utils.ScreenSize == ScreenSizeMode.s240x320 ? 40 : 80;

            this.CurrentState = StateControl.InertialScrolling;
            try
            {
                //время затраченое на движение
                float t = Environment.TickCount - this.startClickTime;
                //если слишком большое, то скролировать не будем, т.к. вероятней всего пользователь просто
                //смотрит на список, и не хочит его листать
                if (t > 600)
                    return;
                //растояние
                float s = this.movedOffset;
                //скорость
                float v = s / t;
                //ускорение
                float a = v / t;
                //растояние которое должно быть пройдено по иннерции
                float gs = v * v * 50 / (2f * 0.025f);

                //физиология пальца такова, что вниз им двигать удобнее и быстрее, поэтому при движении вверх 
                //стоит коэффицент 2
                int goal = this.VOffset + ((int)gs * (this.mouseDirection == DirectionMode.ToUp ? -2 : 1));

                //высота контрола
                int height = 0;
                this.Invoke(new EventHandler(delegate { height = this.Height; }));

                //проверим границы контрола
                if (Math.Abs(goal) > this.itemsHeight)
                    goal = height - this.itemsHeight;
                if (goal > 0)
                    goal = 0;

                bool isScroll = (this.mouseDirection == DirectionMode.ToUp) ? goal < this.VOffset : goal > this.VOffset;
                while (isScroll && (this.CurrentState == StateControl.InertialScrolling))
                {
                    int stepHeight = (goal - this.VOffset) / 5;
                    if (stepHeight == 0)
                        break;

                    this.VOffset += stepHeight;

                    if ((Math.Abs(this.VOffset) + height - offBounds) > this.itemsHeight)
                    {
                        this.VOffset = height - offBounds - this.itemsHeight;
                        this.Invoke(new EventHandler(delegate { this.Invalidate(); }));
                        break;
                    }
                    if (this.VOffset - 100 > 0)
                    {
                        this.VOffset = 100;
                        this.Invoke(new EventHandler(delegate { this.Invalidate(); }));
                        break;
                    }

                    this.Invoke(new EventHandler(delegate { this.Invalidate(); }));

                    isScroll = (this.mouseDirection == DirectionMode.ToUp) ? goal < this.VOffset : goal > this.VOffset;
                }
            }
            finally
            {
                if (this.CurrentState == StateControl.InertialScrolling)
                    this.CurrentState = StateControl.None;

                Thread th = new Thread(new ThreadStart(this.CheckBounds));
                th.Start();
            }
        }

        private void SelectItem(MouseEventArgs mouseArgs)
        {
            Point pointWithOffset = this.GetPointWithOffset(new Point(mouseArgs.X, mouseArgs.Y));
            ScrollListItem selectedItem = this.GetItemByPoint(pointWithOffset);
            if (selectedItem != null)
            {
                this.HighlightSelectedItem(selectedItem);
                selectedItem.IsChanged = !selectedItem.IsChanged;
                this.OnSelectedItem(selectedItem);
            }
        }

        /// <summary>
        /// Закончили перетаскивание элемента, приберемся...
        /// </summary>
        private void EndDraggingItem()
        {
            this.draggingItem = null;
            this.RecalculatedItemsLocation();
            this.Invalidate();
        }

        /// <summary>
        /// Если при перемещении вышли за пределы контрола, доводим до крайнего 
        /// допустимого положения
        /// </summary>
        public void CheckBounds()
        {
            int height = 0;
            this.Invoke(new EventHandler(delegate { height = this.Height; }));
            int scrollStep = height / 15;

            //пока не дошли до верхней граници, или не начато новое движение, будем выравнивать
            while (((this.VOffset > 0) || ((this.itemsHeight < height) && this.VOffset < 0))
                && (this.CurrentState == StateControl.None))
            {
                this.VOffset -= scrollStep;
                if (this.VOffset < 0)
                    this.VOffset = 0;
                this.Invoke(new EventHandler(delegate { this.Invalidate(); }));
            }

            //пока не дошли до нижней граници, или не начато новое движение, будем выравнивать
            while (((this.itemsHeight + this.VOffset) < height)
                && (this.itemsHeight > height)
                && (this.CurrentState == StateControl.None))
            {
                this.VOffset += scrollStep;
                if ((this.itemsHeight + this.VOffset) > height)
                    this.VOffset = height - this.itemsHeight;
                this.Invoke(new EventHandler(delegate { this.Invalidate(); }));
            }
        }

        /// <summary>
        /// Получаем элемент списка по координате
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private ScrollListItem GetItemByPoint(Point point)
        {
            foreach (ScrollListItem item in this.Items)
            {
                if (item.Visible && (this.draggingItem != item) && item.Bounds.Contains(point))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Подсвечиваем выделеный элемент списка
        /// </summary>
        /// <param name="selectedItem"></param>
        private void HighlightSelectedItem(ScrollListItem selectedItem)
        {
            if (selectedItem != null)
            {
                selectedItem.BackColor = Color.Blue;
                this.Invalidate();
                Application.DoEvents();
                Thread.Sleep(300);
                selectedItem.BackColor = Color.Black;
                this.Invalidate();
            }
        }

        public void RecalculatedItemsLocation()
        {
            this.itemsHeight = 0;
            foreach (ScrollListItem item in this.Items)
            {
                if (item.Visible)
                {
                    item.Location = new Point(0, this.itemsHeight);
                    this.itemsHeight += item.Height + 2;
                }
            }
            this.CheckBounds();
        }

        public void AddItem(ScrollListItem item)
        {
            //Инициализируем картинку отобрающуюся в элементе
            item.ImageList = this.ImageList;
            item.IsChanged = item.IsChanged;

            this.Items.Add(item);
        }

        public void RemoveItem(ScrollListItem item)
        {
            this.Items.Remove(item);
        }

        public void Clear()
        {
            this.Items.Clear();
            this.RecalculatedItemsLocation();
        }

        protected virtual void OnSelectedItem(ScrollListItem item)
        {
            if (this._selectedItem != null)
                this._selectedItem(this, item);
        }

        /// <summary>
        /// Направление движения курсора
        /// </summary>
        private enum DirectionMode
        {
            ToUp,
            ToDown,
            None
        }
    }

    /// <summary>
    /// Состояние контрола
    /// </summary>
    public enum StateControl
    {
        /// <summary>
        /// Состояние покоя
        /// </summary>
        None,
        /// <summary>
        /// Скролируем
        /// </summary>
        Scrolling,
        /// <summary>
        /// Скролирование по иннерции
        /// </summary>
        InertialScrolling,
        /// <summary>
        /// Перемещаем элемент
        /// </summary>
        DraggingItem,
        /// <summary>
        /// Нажали на элемент
        /// </summary>
        SelectingItem
    }

}
