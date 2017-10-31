using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый для всех объектов диаграммы, имеющих форму прямоугольника
    /// </summary>
    public partial class DiagramRectangleEntity : DiagramEntity
    {
        #region Const

        /// <summary>
        /// Отступ для textBox
        /// </summary>
        public const int IndentTextBox = 4;

        /// <summary>
        /// Цвет тени объекта
        /// </summary>
        public readonly Color ConstShadowColor = Color.LightGray;

        #endregion

        #region Fields

        #region Indents

        /// <summary>
        /// Верхний отступ
        /// </summary>
        private int indentTop = 2;

        /// <summary>
        /// Нижний отступ
        /// </summary>
        private int indentBottom = 2;

        /// <summary>
        /// Левый отступ
        /// </summary>
        private int indendLeft = 2;

        /// <summary>
        /// Правый отступ
        /// </summary>
        private int indentRight = 2;

        /// <summary>
        /// Отступ между блоками
        /// </summary>
        private int indentBetween = 0;

         #endregion Indents

        #region Для сохранения в XML

        /// <summary>
        /// the backcolor of the shapes
        /// </summary>
        private Color fillColor = Color.LightYellow;

        #endregion Для сохранения в XML

        /// <summary>
        /// Минимальная ширина
        /// </summary>
        private int minWidht = 0;

        /// <summary>
        /// Минимальная высота
        /// </summary>
        private int minHeight = 0;

        /// <summary>
        /// the brush corresponding to the backcolor
        /// </summary>
        private Brush shapeBrush;

        /// <summary>
        /// TextBox для редактирования заголовка
        /// </summary>
        private TextBox headerText = new TextBox();

        /// <summary>
        /// Нумерация атрибутов
        /// </summary>
        private int number = -1;

        /// <summary>
        /// Отображение тени
        /// </summary>
        private bool isShadow = false;

        /// <summary>
        /// Форматирование текста
        /// </summary>
        private StringFormat format = new StringFormat(StringFormatFlags.LineLimit);        

        #endregion

        #region Constructor

        public DiagramRectangleEntity(string key, Guid id, int x, int y)
            : this(key, id, null, x, y)
        {
        }

        public DiagramRectangleEntity(string key, Guid id, AbstractDiagram diagram, int x, int y)
            : this(key, id, diagram, x, y, Color.AliceBlue)
        {
        }

        public DiagramRectangleEntity(string key, Guid id, AbstractDiagram diagram, int x, int y, Color color)
            : base(key, id, diagram, color)
        {
            this.fillColor = color;

            // прямоугольник с минимальными размерами
            EntityRectangle = new Rectangle(x, y, 100, 70);
            SetBrush();

            headerText.TextChanged += new EventHandler(HeaderText_TextChanged);

            InitializeDefault();

            // Форматирование
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Count selected points
        /// </summary>
        [Browsable(false)]
        public override int HandleCount
        {
            get
            {
                return 8;
            }
        }

        [Category("Отступы")]
        [DisplayName("Отступ между блоками")]
        [Description("Отступ между блоками")]
        public int IndentBetween
        {
            get
            {
                return indentBetween;
            }

            set
            {
                if (value >= 0)
                {
                    indentBetween = value;
                    SetMinSize(Diagram.Site.CreateGraphics(), false);
                    Diagram.Site.Invalidate();
                }
            }
        }

        [Category("Отступы")]
        [DisplayName("Правый отступ")]
        [Description("Правый отступ")]
        public int IndentRight
        {
            get
            {
                return indentRight;
            }

            set
            {
                if (value >= 0)
                {
                    indentRight = value;
                    SetMinSize(Diagram.Site.CreateGraphics(), false);
                    Diagram.Site.Invalidate();
                }
            }
        }

        [Category("Отступы")]
        [DisplayName("Левый отступ")]
        [Description("Левый отступ")]
        public int IndendLeft
        {
            get
            {
                return indendLeft;
            }

            set
            {
                if (value >= 0)
                {
                    indendLeft = value;
                    SetMinSize(Diagram.Site.CreateGraphics(), false);
                    Diagram.Site.Invalidate();
                }
            }
        }

        [Category("Отступы")]
        [DisplayName("Нижний отступ")]
        [Description("Нижний отступ")]
        public int IndentBottom
        {
            get
            {
                return indentBottom;
            }

            set
            {
                if (value >= 0)
                {
                    indentBottom = value;
                    SetMinSize(Diagram.Site.CreateGraphics(), false);
                    Diagram.Site.Invalidate();
                }
            }
        }

        [Category("Отступы")]
        [DisplayName("Верхний отступ")]
        [Description("Верхний отступ")]
        public int IndentTop
        {
            get
            {
                return indentTop;
            }

            set
            {
                if (value >= 0)
                {
                    indentTop = value;
                    SetMinSize(Diagram.Site.CreateGraphics(), false);
                    Diagram.Site.Invalidate();
                }
            }
        }

        /// <summary>
        /// Видимость тени
        /// </summary>
        public bool IsShadow
        {
            get
            {
                return isShadow;
            }

            set
            {
                if (isShadow != value)
                {
                    isShadow = value;
                    this.colorShadow.Enabled = isShadow;
                    this.shadowVisible.Checked = isShadow;

                    Diagram.IsChanged = true;
                    Diagram.Site.Invalidate();
                }
            }
        }

        /// <summary>
        /// Свойство для установки минимальной ширины 
        /// </summary>
        [Browsable(false)]
        public int MinWidht
        {
            get
            {
                return minWidht;
            }

            set
            {
                minWidht = value;
            }
        }

        /// <summary>
        /// Свойство для установки минимальной высоты
        /// </summary>
        [Browsable(false)]
        public int MinHeight
        {
            get
            {
                return minHeight;
            }

            set
            {
                minHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the shape
        /// </summary>
        [Browsable(true), Description("The width of the shape"), Category("Layout")]
        public int Width
        {
            get
            {
                return EntityRectangle.Width;
            }

            set
            {
                Resize(value, Height);

                // site.DrawTree();
                Diagram.Site.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the height of the shape
        /// </summary>
        [Browsable(true), Description("The height of the shape"), Category("Layout")]
        public int Height
        {
            get
            {
                return this.EntityRectangle.Height;
            }

            set
            {
                Resize(this.Width, value);
                Diagram.Site.Invalidate();
            }
        }

        /// <summary>
        /// the x-coordinate of the upper-left corner
        /// </summary>
        [Browsable(false), Description("The x-coordinate of the upper-left corner"), Category("Layout")]
        public int X
        {
            get
            {
                return EntityRectangle.X;
            }

            set
            {
                Point p = new Point(value - EntityRectangle.X, EntityRectangle.Y);
                this.Move(p);
                Diagram.Site.Invalidate();
            }
        }

        /// <summary>
        /// the y-coordinate of the upper-left corner
        /// </summary>
        [Browsable(false), Description("The y-coordinate of the upper-left corner"), Category("Layout")]
        public int Y
        {
            get
            {
                return EntityRectangle.Y;
            }

            set
            {
                Point p = new Point(EntityRectangle.X, value - EntityRectangle.Y);
                this.Move(p);
                Diagram.Site.Invalidate();
            }
        }

        /// <summary>
        /// The backcolor of the shape
        /// </summary>
        public override Color FillColor
        {
            get
            {
                return fillColor;
            }

            set
            {
                if (fillColor != value)
                {
                    fillColor = value;
                    SetBrush();
                    Invalidate();

                    Diagram.IsChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of the shape;
        /// </summary>
        [Browsable(false)]
        public Point Location
        {
            get
            {
                return new Point(this.EntityRectangle.X, this.EntityRectangle.Y);
            }

            set
            {
                // we use the move method but it requires the delta value, not an absolute position!
                Point p = new Point(value.X - EntityRectangle.X, value.Y - EntityRectangle.Y);

                // if you'd use this it would indeed move the shape but not the connector s of the shape
                // this.rectangle.X = value.X; this.rectangle.Y = value.Y; Invalidate();
                this.Move(p);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the left coordiante of the rectangle
        /// </summary>
        [Browsable(false)]
        public int Left
        {
            get { return this.EntityRectangle.Left; }
        }

        /// <summary>
        /// Gets the right coordiante of the rectangle
        /// </summary>
        [Browsable(false)]
        public int Right
        {
            get { return this.EntityRectangle.Right; }
        }

        /// <summary>
        /// Get the bottom coordinate of the shape
        /// </summary>
        [Browsable(false)]
        public int Bottom
        {
            get { return this.EntityRectangle.Bottom; }
        }

        /// <summary>
        /// Gets the top coordinate of the rectangle
        /// </summary>
        [Browsable(false)]
        public int Top
        {
            get { return this.EntityRectangle.Top; }
        }

        protected TextBox HeaderText
        {
            get { return headerText; }
            set { headerText = value; }
        }

        protected StringFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        protected int Number
        {
            get { return number; }
            set { number = value; }
        }

        #endregion

        #region Methods       
 
        public static Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1)
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static Rectangle GetNormalizedRectangle(Point p1, Point p2)
        {
            return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static Rectangle GetNormalizedRectangle(Rectangle r)
        {
            return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        /// <summary>
        /// Устанавливаем авторазмер прямоугольника
        /// </summary>
        public virtual void AutoSizeRec()
        {
        }

        public override void Draw(Graphics g, Size scrollOffset)
        {
            SolidBrush shawowBrush = new SolidBrush(ShadowColor);
            try
            {
                // тени
                if (isShadow)
                {
                    Rectangle shadowBot = new Rectangle(EntityRectangle.Left + 4, EntityRectangle.Bottom + 1, EntityRectangle.Width, 4);
                    shadowBot.Offset(scrollOffset.Width, scrollOffset.Height);
                    Rectangle shadowRight = new Rectangle(EntityRectangle.Right + 1, EntityRectangle.Top + 4, 4, EntityRectangle.Height);
                    shadowRight.Offset(scrollOffset.Width, scrollOffset.Height);
                    
                    g.FillRectangle(shawowBrush, shadowBot);
                    g.FillRectangle(shawowBrush, shadowRight);
                }
            }
            finally
            {
                shawowBrush.Dispose();
            }
        }

        /// <summary>
        /// Пересечение с прямоугольником выделения
        /// </summary>
        public override bool IntersectWith(Rectangle rectangle)
        {
            return ScaleTransform.TransformRectangle(EntityRectangle, Diagram.Site.ZoomFactor).IntersectsWith(rectangle);
        }

        /// <summary>
        /// Resizes the shape's rectangle in function of the containing text
        /// </summary>
        public void Fit()
        {
            Graphics g = Diagram.Site.CreateGraphics();
            try
            {
                Rectangle rectangle = EntityRectangle;
                Size s = Size.Round(g.MeasureString(Text, Font));
                rectangle.Height = s.Height + 8;
                EntityRectangle = rectangle;
                Invalidate();
            }
            finally
            {
                g.Dispose();
            }
        }
        
        /// <summary>
        /// Override the abstract Hit method
        /// </summary>
        public override bool Hit(System.Drawing.Point p)
        {
            p = ScaleTransform.TransformPoint(p, Diagram.Site.ZoomFactor);
            Rectangle rectangleB = new Rectangle(
                EntityRectangle.Left - 2, 
                EntityRectangle.Top - 2,
                EntityRectangle.Width + 4,
                EntityRectangle.Height + 4);

            return rectangleB.Contains(p);
        }

        /// <summary>
        /// Проверка попадания
        /// </summary>
        public override int HitTest(Point p)
        {
            if (this.IsSelected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (ScaleTransform.TransformRectangle(GetHandleRectangle(i), Diagram.Site.ZoomFactor).Contains(p))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Overrides the abstract Invalidate method
        /// </summary>
        public override void Invalidate()
        {
            Diagram.Site.Invalidate(EntityRectangle);
        }

        /// <summary>
        /// Moves the shape with the given shift
        /// </summary>
        /// <param name="p"> represent a shift-vector, not the absolute position!</param>
        public override void Move(Point p)
        {
            Rectangle rectangle = EntityRectangle;
            Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

            rectangle.X += (int)(p.X / ((float)Diagram.Site.ZoomFactor / 100));
            rectangle.Y += (int)(p.Y / ((float)Diagram.Site.ZoomFactor / 100));

            // не выходит за пределы окна
            rectangle.X = Math.Max(scrollOffset.Width, rectangle.X);
            rectangle.Y = Math.Max(scrollOffset.Height, rectangle.Y);

            EntityRectangle = rectangle;

            Diagram.Site.Invalidate();
        }

        /// <summary>
        /// Resizes the shape 
        /// </summary>
        public override void Resize(int width, int height)
        {
            Rectangle rectangle = EntityRectangle;
            rectangle.Height = height;
            rectangle.Width = width;
            EntityRectangle = rectangle;
            this.Diagram.Site.Invalidate();
        }

        /// <summary>
        /// Создаём TextBox
        /// </summary>
        public virtual void CreateHeaderTextBox(Point point)
        {
            using (System.Drawing.Font font = ScaleTransform.TransformFont(this.Font, Diagram.Site.ZoomFactor))
            {
                // смещение
                Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

                // стиль границы
                headerText.BorderStyle = BorderStyle.None;

                // цвет текста
                headerText.ForeColor = TextColor;

                // Шрифт
                headerText.Font = font;

                // многостроковый режим
                headerText.Multiline = true;

                // родительский объект
                headerText.Parent = Diagram.Site;
            }
        }

        /// <summary>
        /// Прячем редактирование заголовка
        /// </summary>
        public void DisposeHeaderTextBox()
        {
            headerText.Visible = false;
            headerText.Enabled = false;
        }

        /// <summary>
        /// Set cursor
        /// </summary>
        /// <returns>Cursor view</returns>
        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return Cursors.SizeNWSE;
                case 2:
                    return Cursors.SizeNS;
                case 3:
                    return Cursors.SizeNESW;
                case 4:
                    return Cursors.SizeWE;
                case 5:
                    return Cursors.SizeNWSE;
                case 6:
                    return Cursors.SizeNS;
                case 7:
                    return Cursors.SizeNESW;
                case 8:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Default;
            }
        }
                
        /// <summary>
        /// Move handle to point(resize)
        /// </summary>
        public override void MoveHandleTo(Point point, int handleNumber)
        {
            point = ScaleTransform.TransformPoint(point, Diagram.Site.ZoomFactor);

            int left = EntityRectangle.Left;
            int top = EntityRectangle.Top;
            int right = EntityRectangle.Right;
            int bottom = EntityRectangle.Bottom;

            switch (handleNumber)
            {
                case 1:
                    left = point.X;
                    top = point.Y;
                    if (right - left < minWidht)
                    {
                        left = right - minWidht;
                    }

                    if (bottom - top < minHeight)
                    {
                        top = bottom - minHeight;
                    }

                    break;
                case 2:
                    top = point.Y;
                    if (bottom - top < minHeight)
                    {
                        top = bottom - minHeight;
                    }

                    break;
                case 3:
                    right = point.X;
                    top = point.Y;
                    if (bottom - top < minHeight)
                    {
                        top = bottom - minHeight;
                    }

                    break;
                case 4:
                    right = point.X;
                    break;
                case 5:
                    right = point.X;
                    bottom = point.Y;
                    break;
                case 6:
                    bottom = point.Y;
                    break;
                case 7:
                    left = point.X;
                    bottom = point.Y;
                    if (right - left < minWidht)
                    {
                        left = right - minWidht;
                    }

                    break;
                case 8:
                    left = point.X;
                    if (right - left < minWidht)
                    {
                        left = right - minWidht;
                    }

                    break;
            }

            SetRectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Координаты точки выделения
        /// </summary>
        /// <param name="handleNumber">Номер точки</param>
        /// <returns>Непосредственно координаты</returns>
        public override Point GetHandle(int handleNumber)
        {
            int x, y, coordinateXCenter, coordinateYCenter;

            coordinateXCenter = EntityRectangle.X + (EntityRectangle.Width / 2);
            coordinateYCenter = EntityRectangle.Y + (EntityRectangle.Height / 2);
            x = EntityRectangle.X;
            y = EntityRectangle.Y;

            switch (handleNumber)
            {
                case 1:
                    x = EntityRectangle.X;
                    y = EntityRectangle.Y;
                    break;
                case 2:
                    x = coordinateXCenter;
                    y = EntityRectangle.Y;
                    break;
                case 3:
                    x = EntityRectangle.Right;
                    y = EntityRectangle.Y;
                    break;
                case 4:
                    x = EntityRectangle.Right;
                    y = coordinateYCenter;
                    break;
                case 5:
                    x = EntityRectangle.Right;
                    y = EntityRectangle.Bottom;
                    break;
                case 6:
                    x = coordinateXCenter;
                    y = EntityRectangle.Bottom;
                    break;
                case 7:
                    x = EntityRectangle.X;
                    y = EntityRectangle.Bottom;
                    break;
                case 8:
                    x = EntityRectangle.X;
                    y = coordinateYCenter;
                    break;
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Контроль за размерами прямоугольника
        /// </summary>
        public override void Normalize()
        {
            Graphics g = Diagram.Site.CreateGraphics();
            try
            {
                SetMinSize(g, false);
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// Метод отрисовки точки выделения
        /// </summary>
        public override void DrawTracker(Graphics g, Size scrollOffset)
        {
            if (!IsSelected)
            {
                return;
            }

            for (int i = 1; i <= HandleCount; i++)
            {
                Rectangle r = GetHandleRectangle(i);
                r.Offset(scrollOffset.Width, scrollOffset.Height);
                g.FillRectangle(SelectDotBrush, r);
            }
        }

        /// <summary>
        /// return rectangle
        /// </summary>
        public override Rectangle GetHandleRectangle(int handleNumber)
        {
            Point point = GetHandle(handleNumber);

            return new Rectangle(point.X - 3, point.Y - 3, 6, 6);
        }

        /// <summary>
        /// Двойной щелчок на объекте типа прямоугольник
        /// </summary>
        public override void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            base.OnMouseDoubleClick(sender, e);

            this.CreateHeaderTextBox(Diagram.Site.LastClickPoint);
        }

        #endregion

        #region Helper func
        
        public Rectangle GetNormalize(int x1, int y1, int x2, int y2)
        {
            return new Rectangle(x1, y1, x2, y2);
        }

        public Rectangle GetNormalize(Rectangle r)
        {
            return GetNormalize(r.X, r.Y, r.Width, r.Height);
        }

        internal override void CutRect(Size minLocation)
        {
            this.EntityRectangle.Offset(-minLocation.Width + 15, -minLocation.Height + 15);
        }

        /// <summary>
        /// Возвращает максимальное слово в строке
        /// </summary>
        /// <returns> Максимальное слово в строке</returns>
        protected string MaxWord()
        {
            string s = String.Empty;
            string testStr = String.Empty;

            foreach (char ch in Text)
            {
                if (ch != ' ')
                {
                    testStr += ch;
                }
                else
                {
                    if (s.Length < testStr.Length)
                    {
                        s = testStr;
                    }

                    testStr = String.Empty;
                }
            }

            // последнее слово
            if (s.Length < testStr.Length)
            {
                s = testStr;
            }

            return s;
        }

        /// <summary>
        /// Определяет размеры текста и устанавливает минимальные размеры ректангла
        /// </summary>
        protected virtual void SetMinSize(Graphics g, bool auto)
        {
        }

        /// <summary>
        /// Set my rectangle
        /// </summary>
        protected void SetRectangle(int x, int y, int widht, int height)
        {
            Rectangle rectangle = EntityRectangle;
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = widht;
            rectangle.Height = height;
            EntityRectangle = rectangle;
        }

        /// <summary>
        /// Sets the brush corresponding to the backcolor
        /// </summary>
        protected void SetBrush()
        {
            shapeBrush = IsSelected ? new SolidBrush(Color.YellowGreen) : new SolidBrush(fillColor);
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие при редактировании заголовка
        /// </summary>
        private void HeaderText_TextChanged(object sender, EventArgs e)
        {
            // заголовок
            if (number == -1)
            {
                this.Text = headerText.Text;
            }
            else
            {
                int i = 0;
                foreach (UMLAttribute a in ((UMLEntityBase)this).Attributes)
                {
                    if (a.IsAttrVisible)
                    {
                        if (i == number)
                        {
                            a.Name = headerText.Text;
                            return;
                        }

                        i++;
                    }
                }
            }
        }
        #endregion Events
    }
}
