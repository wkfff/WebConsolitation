using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Krista.FM.Client.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый для всех объектов диаграммы
    /// </summary>
    public abstract partial class DiagramEntity
    {
        #region Const
        // Настройки по-умолчанию

        /// <summary>
        /// Толщина линии
        /// </summary>
        public readonly int ConstLineWidth = 1;

        /// <summary>
        /// Цвет линий
        /// </summary>
        public readonly Color ConstLineColor = Color.Black;

        /// <summary>
        /// Шрифт объекта
        /// </summary>
        public readonly Font ConstFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);

        /// <summary>
        /// Цвет шрифта
        /// </summary>
        public readonly Color ConstFontColor = Color.Black;

        /// <summary>
        /// Инструмента для рисования линии
        /// </summary>
        public readonly Pen ConstPen = new Pen(Color.Black);

        #endregion Const

        #region Identifier

        /// <summary>
        /// Счетчик для генерации идентификаторов
        /// </summary>
        private static object globalIdentifier = 0;

        /// <summary>
        /// Идентификатор текущего объекта
        /// </summary>
        private int identifier = -1;

        #endregion

        #region Fields
               
        #region Для сохранения в XML

        /// <summary>
        /// the unique identifier
        /// </summary>
        private string key;

        /// <summary>
        /// Currently rectangle
        /// </summary>
        private Rectangle entityRectangle;

        /// <summary>
        /// Default font for drawing text
        /// </summary>
        private Font font;

        /// <summary>
        /// Цвет текста
        /// </summary>
        private Color textColor;

        /// <summary>
        /// Цвет линий
        /// </summary>
        private Color lineColor;

        /// <summary>
        /// Толщина линий
        /// </summary>
        private int lineWidth;

        #endregion Для сохранения в XML

        #region General

        /// <summary>
        /// Кисть для точек выделения
        /// </summary>
        private SolidBrush selectDotBrush = new SolidBrush(Color.Black);

        /// <summary>
        /// Серверный объект
        /// </summary>
        private ICommonObject commonObject;

        /// <summary>
        /// tells whether the current entity is hovered by the mouse
        /// </summary>
        private bool hovered = false;

        /// <summary>
        /// the control to which the eneity belongs
        /// </summary>
        private AbstractDiagram diagram;

        /// <summary>
        /// tells whether the entity is selected
        /// </summary>
        private bool isSelected = false;

        /// <summary>
        /// the visibility bit
        /// </summary>
        private bool visible = true;

        /// <summary>
        /// the text on the shape
        /// </summary>
        private string text = string.Empty;

        /// <summary>
        /// Стереотип объекта    
        /// </summary>
        private string stereotype = String.Empty;

        /// <summary>
        /// Определяет видимость стереотипа
        /// </summary>
        private bool stereotypeVisible = true;

        /// <summary>
        /// Количество точек выделения у фигуры
        /// </summary>
        private int countOfPoint;

        /// <summary>
        /// Графический путь объекта
        /// </summary>
        private GraphicsPath graphicsPath = new GraphicsPath();

        /// <summary>
        /// Контекстное меню объекта диаграммы
        /// </summary>
        private ContextMenuStrip contextMenu = new ContextMenuStrip();

        #endregion General        

        #endregion

        #region Constructor

        public DiagramEntity(string key, Guid id)
        {
            this.key = key;
            this.ID = id;

            identifier = GetNewIdentifier();
        }

        public DiagramEntity(string key, Guid id, AbstractDiagram diagram)
            : this(key, id)
        {
            this.diagram = diagram;

            // настройки по-умолчанию
            InitializeDefault();

            // Инициализация команд
            InitializeCommand();

            if (!(this is UMLLabel) && !(this is UMLAnchorEntityToNote))
            {
                commonObject = Diagram.Site.SchemeEditor.Scheme.GetObjectByKey(key);
            }

            if (CommonObject != null)
            {
                this.text = GetServerObjectCaption(CommonObject);
            }
            else
            {
                text = key;
            }

            Pen = new Pen(LineColor);

            this.Properties.Click += new EventHandler(Properties_Click);

            this.contextMenu.ItemClicked += new ToolStripItemClickedEventHandler(ContextMenu_ItemClicked);
            this.TsmiFormat.DropDownItemClicked += new ToolStripItemClickedEventHandler(ContextMenu_ItemClicked);
            this.cmdStereotype.DropDownItemClicked += new ToolStripItemClickedEventHandler(ContextMenu_ItemClicked);
            this.Options.DropDownItemClicked += new ToolStripItemClickedEventHandler(ContextMenu_ItemClicked);
        }

        public DiagramEntity(string key, Guid id, AbstractDiagram diagram, Color color)
            : this(key, id, diagram)
        {
        }

        #endregion

        #region Properties
        
        public Command CmdDeletFromScheme
        {
            get { return cmdDeletFromScheme; }
        }

        public Command CmdDeleteSymbol
        {
            get { return cmdDeleteSymbol; }
        }

        [Browsable(false)]
        public Guid ID { get; set; }

        public Pen Pen { get; set; }

        /// <summary>
        /// Цвет тени объекта
        /// </summary>
        public Color ShadowColor { get; set; }

        /// <summary>
        /// Серверный объект
        /// </summary>
        public ICommonObject CommonObject
        {
            get { return commonObject; }
        }

        /// <summary>
        /// Currently rectangle
        /// </summary>
        public virtual Rectangle EntityRectangle
        {
            get
            {
                return entityRectangle;
            }

            set 
            {
                if (entityRectangle != value)
                {
                    entityRectangle = value;
                    Diagram.IsChanged = true;
                }
            }
        }

        /// <summary>
        /// Get stereotype
        /// </summary>
        public string Stereotype
        {
            get { return stereotype; }
            protected set { stereotype = value; }
        }

        /// <summary>
        /// Gets or sets whether the entity is selected
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        /// <summary>
        /// Gets or sets the site of the entity
        /// </summary>
        [Browsable(false)]
        public AbstractDiagram Diagram
        {
            get { return diagram; }
            set { diagram = value; }
        }

        /// <summary>
        /// The globally unique identifier of the entity.
        /// Mostly useful for serialization.
        /// </summary>
        [Browsable(false)]
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Текст заголовка 
        /// </summary>
        public virtual string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                Diagram.Site.Invalidate();
            }
        }

        /// <summary>
        /// The font used for drawing the text (if any)
        /// </summary>
        [Browsable(false)]
        public Font Font
        {
            get
            {
                return font;
            }

            set
            {
                if (font != value)
                {
                    font = value;
                    Diagram.IsChanged = true;
                }
            }
        }

        /// <summary>
        /// Цвет линий
        /// </summary>
        public Color LineColor
        {
            get
            {
                return lineColor;
            }

            set
            {
                if (lineColor != value)
                {
                    lineColor = value;
                    Diagram.IsChanged = true;
                }
            }
        }

        /// <summary>
        /// Толщина линий
        /// </summary>
        public int LineWidth
        {
            get
            {
                return lineWidth;
            }

            set 
            {
                if (lineWidth != value)
                {
                    lineWidth = value;
                    Diagram.IsChanged = true;
                }
            }
        }

        /// <summary>
        /// Цвет текста
        /// </summary>
        public Color TextColor
        {
            get
            {
                return textColor;
            }

            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    Diagram.IsChanged = true;
                }
            }
        }

        /// <summary>
        /// Кисть для точек выделения
        /// </summary>
        public SolidBrush SelectDotBrush
        {
            get { return selectDotBrush; }
            set { selectDotBrush = value; }
        }

        /// <summary>
        /// The backcolor of the shape
        /// </summary>
        public virtual Color FillColor
        {
            get { return Color.White; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets whether the shape is visible
        /// </summary>
        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        /// <summary>
        /// Количество точек выделения
        /// </summary>
        public virtual int HandleCount
        {
            get
            {
                return -1;
            }

            set
            {
                countOfPoint = value;
            }
        }

        /// <summary>
        /// Return graphicsPath object
        /// </summary>
        public GraphicsPath GraphicsPath
        {
            get
            {
                return graphicsPath;
            }

            set
            {
                graphicsPath = value;
            }
        }

        /// <summary>
        /// Определяет видимость стереотипа
        /// </summary>
        public bool StereotypeVisible
        {
            get
            {
                return stereotypeVisible;
            }

            set
            {
                stereotypeVisible = value;
            }
        }

        protected int CountOfPoint
        {
            get { return countOfPoint; }
            set { countOfPoint = value; }
        }

        protected ContextMenuStrip ContextMenu
        {
            get { return contextMenu; }
            set { contextMenu = value; }
        }

        #endregion

        #region Methods
        
        public override string ToString()
        {
            return String.Format("Shape identifier - {0} : {1}", identifier, base.ToString());
        }

        /// <summary>
        /// Метод, вызываниемый перед отрисовкой объекта
        /// </summary>
        public virtual void BeforeDraw()
        {
        }

        /// <summary>
        /// Paints the entity on the control
        /// </summary>
        /// <param name="g">the graphics object to paint on</param>
        public abstract void Draw(Graphics g, Size scrollOffset);

        /// <summary>
        /// Метод, вызываниемый после отрисовкой объекта
        /// </summary>
        public virtual void AfterDraw()
        {
        }

        /// <summary>
        /// Tests whether the shape is hit by the mouse
        /// </summary>
        public abstract bool Hit(Point p);

        /// <summary>
        /// Invalidates the entity
        /// </summary>
        public abstract void Invalidate();

        /// <summary>
        /// Moves the entity on the canvas
        /// </summary>
        /// <param name="p">the shifting vector, not an absolute position!</param>
        public abstract void Move(Point p);

        /// <summary>
        /// Resize currently object
        /// </summary>
        public virtual void Resize(int height, int width)
        {
        }

        /// <summary>
        /// Double click on Diagram Entity
        /// </summary>
        public virtual void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Get curesor for the handle
        /// </summary>
        public virtual Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Default;
        }

        /// <summary>
        /// Coordinate selected point
        /// </summary>
        public virtual Point GetHandle(int handleNumber)
        {
            return new Point(0, 0);
        }

        /// <summary>
        /// попадание в точку выделения
        /// </summary>
        public virtual int HitTest(Point p)
        {
            return -1;
        }

        /// <summary>
        /// Пересечение с прямоугольником выделения
        /// </summary>
        /// <param name="rectangle">Прямоугольник выделения</param>
        /// <returns>true - есть пересечение, false - нет пересечения</returns>
        public virtual bool IntersectWith(Rectangle rectangle)
        {
            return false;
        }

        /// <summary>
        /// Move handle to point(resize)
        /// </summary>
        public virtual void MoveHandleTo(Point point, int handleNumber)
        {
        }

        /// <summary>
        /// контроль за размерами
        /// </summary>
        public virtual void Normalize()
        {
        }

        /// <summary>
        /// Рисование точки выделения
        /// </summary>
        public virtual void DrawTracker(Graphics g, Size scrollOffset)
        {
        }

        /// <summary>
        /// Прямоугольник выделения
        /// </summary>
        public virtual Rectangle GetHandleRectangle(int handleNumber)
        {
            Point point = GetHandle(handleNumber);

            return new Rectangle(point.X - 3, point.Y - 3, 6, 6);
        }

        /// <summary>
        /// Отображение контекстного меню
        /// </summary>
        public virtual void ContextMenuShow(Point p)
        {
            this.contextMenu.Show(Diagram.Site, p);
        }

        /// <summary>
        /// Удаление объекта с диаграммы
        /// </summary>
        public virtual void RemoveEntity()
        {
            SiteRefresh();
        }

        /// <summary>
        /// Восстановление объекта диаграммы
        /// </summary>
        public virtual void RestoreEntity()
        {
            SiteRefresh();
        }

        /// <summary>
        /// Обновление имени объекта
        /// </summary>
        public virtual void RefreshKey()
        {
        }

        /// <summary>
        /// Виртуальный метод, отвечающий за отображение/скрытие стереотипа
        /// </summary>
        public virtual void DisplayStereotype(bool onoff)
        {
        }

        /// <summary>
        /// Виртуальный метод, отвечающий за отображение/скрытие SQL-выражения
        /// </summary>
        public virtual void DisplaySQLExpression(bool visible)
        {
        }

        public virtual void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Координаты стартовой единицы
        /// </summary>
        /// <returns>Координаты центра</returns>
        internal Point Coordinate()
        {
            return new Point((this.EntityRectangle.Width / 2) + this.EntityRectangle.Left, (this.EntityRectangle.Height / 2) + this.EntityRectangle.Top);
        }

        internal virtual void CutRect(Size minLocation)
        {
        }

        #region Identifier

        /// <summary>
        /// Возвращает следующий идентификатор из счетчика
        /// </summary>
        /// <returns>Новый идентификатор</returns>
        [DebuggerStepThrough()]
        protected static int GetNewIdentifier()
        {
            int localIdentifier;
            lock (globalIdentifier)
            {
                globalIdentifier = localIdentifier = Convert.ToInt32(globalIdentifier) + 1;
            }

            return localIdentifier;
        }

        #endregion
        
        /// <summary>
        /// Имена объектов, используемые на диаграмме
        /// </summary>
        protected virtual void ChangeNames(bool value)
        {
            if (CommonObject != null)
            {
                this.text = GetServerObjectCaption(CommonObject);
            }
        }

        protected void SiteRefresh()
        {
            Diagram.IsChanged = true;
            Diagram.Site.Invalidate();
        }

        #region Утилиты

        protected virtual string GetServerObjectCaption(ICommonObject obj)
        {
            if (obj is IEntity)
            {
                return String.Format("{0}.{1} ({2})", ((IEntity)obj).SemanticCaption, obj.Caption, obj.FullName);
            }
            else
            {
                return String.Format("{0} ({1})", obj.Caption, obj.FullName);
            }
        }

        #endregion Утилиты

        #endregion
        
        #region Events

        /// <summary>
        /// Общий обработчик для всех пунктов контекстного меню
        /// </summary>
        protected void ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.contextMenu.Hide();

            Command cmd = e.ClickedItem.Tag as Command;

            if (cmd != null)
            {
                cmd.Execute();
            }

            CommandWithPrm cmdWithPrm = e.ClickedItem.Tag as CommandWithPrm;

            if (cmdWithPrm != null)
            {
                cmdWithPrm.Execute(!((ToolStripMenuItem)e.ClickedItem).Checked);
            }
        }

        /// <summary>
        /// Свойства объекта
        /// </summary>
        private void Properties_Click(object sender, EventArgs e)
        {
            PropertiesForm propertyForm = new PropertiesForm();
            propertyForm.PropertyGrid.SelectedObject = this;

            propertyForm.ShowDialog();
        }
        
        #endregion
    }
}
