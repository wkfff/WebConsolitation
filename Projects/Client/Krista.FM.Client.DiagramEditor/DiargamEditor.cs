using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;

using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor.Commands;
using Krista.FM.Client.DiagramEditor.Tools;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Редактор диаграмм
    /// </summary>
    public partial class DiargamEditor : ScrollableControl
    {
        #region Const

        private const string ContextMenuKey = "ContextMenu";
        private const char SubMenuPrefixKey = '@';
        private const string ToolBarKey = "ToolBar";

        #endregion
        
        #region Fields

        private readonly char[] MenuPathSeparator = new char[] { '/' };

        /// <summary>
        /// Редактор схемы
        /// </summary>
        private ISchemeEditor schemeEditor;

        /// <summary>
        /// Диаграмма классов
        /// </summary>
        private AbstractDiagram diagram;

        /// <summary>
        /// just a reference point for the OnMouseDown event
        /// </summary>
        private Point refp = new Point(0, 0);

        /// <summary>
        /// the entities ciollection currently selected
        /// </summary>
        private List<DiagramEntity> selectedEntities = new List<DiagramEntity>();

        /// <summary>
        /// Last DiagramEntity selected
        /// </summary>
        private DiagramEntity selectedShape;               

        /// <summary>
        /// Instance ToolPointer's class
        /// </summary>
        private ToolPointer toolPointer = new ToolPointer();

        /// <summary>
        /// Координаты последней кликнутой точки
        /// </summary>
        private Point lastClickPoint;

        /// <summary>
        /// Прямоугольник для перемещения
        /// </summary>
        private Rectangle rectangle;

        /// <summary>
        /// Выделение угловой точки
        /// </summary>
        private bool isPointSelected;

        /// <summary>
        /// Признак отпускания нажатия клавиши мыши
        /// </summary>
        private bool mouseDown;

        /// <summary>
        /// признак перемещения манипулятора
        /// </summary>
        private bool mouseMoving;

        /// <summary>
        /// Association button checked
        /// </summary>
        private bool createAssociation = false;

        /// <summary>
        /// Момент начала отрисовки стрелки 
        /// </summary>
        private bool beginDrawAssociate = false;

        /// <summary>
        /// rubber rectangle
        /// </summary>
        private Rectangle selectRect = new Rectangle();

        /// <summary>
        /// Прямоугольник для запоминания начальной позиции перемещения
        /// </summary>
        private Rectangle startRect;

        /// <summary>
        /// Для ассоциации
        /// </summary>
        private List<Point> points = new List<Point>();
        
        /// <summary>
        /// Структура, определяющая прямоугольник выделения
        /// </summary>
        private RECT rc;

        private IntPtr intPtrhWnd;
        private IntPtr intPtrhDC;

        /// <summary>
        /// Размеры прямоугольника до их изменения
        /// </summary>
        private Size sizeBeforeChange;

        #region Commands

        /// <summary>
        /// Команда для создания новой ассоциации в схеме
        /// </summary>
        private INewAssociationCommand newAssociationCommand;

        /// <summary>
        /// Команда для создания нового класса в схеме
        /// </summary>
        private INewEntityCommand newEntityCommand;

        /// <summary>
        /// Команда для создания нового пакета в схеме
        /// </summary>
        private INewPackageCommand newPackageCommand;

        /// <summary>
        /// Команда на операцию dragdrop
        /// </summary>
        private Command cmdDragDrop;
       
        /// <summary>
        /// Команда предварительного просмотра
        /// </summary>
        private Command cmdPrintPreview;

        /// <summary>
        /// Команда печати на одной странице
        /// </summary>
        private Command cmdPrintOnePage;

        /// <summary>
        /// Команда обработки с клавиатуры
        /// </summary>
        private CommandKeyPress cmdKey;

        /// <summary>
        /// Вспомогательные команды
        /// </summary>
        private HelperCommand cmdHelper;

        #endregion Commands

        /// <summary>
        /// Параметр масштабирования
        /// </summary>
        private int zoom = 100;

        private ZoomControl scaleControl;

        /// <summary>
        /// Внешний тулбар для диаграммы.
        /// </summary>
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager toolbarsManager;

        /// <summary>
        /// Менеджер команд undo/redo
        /// </summary>
        private UndoRedo undoredoManager;

        /// <summary>
        /// Коллекция иконок дизайнера
        /// </summary>
        private Dictionary<Images, Bitmap> imageList;

        /// <summary>
        /// Настройки страницы для печати
        /// </summary>
        private DefaultSettings defaultSettings = new DefaultSettings();
        
        /// <summary>
        /// Свойство для определения рисовать ассоциацию Сопоставления или нет 
        /// </summary>
        private bool createBridgeAssociation;

        /// <summary>
        /// Свойство для определения рисовать ассоциацию Сопоставления или нет 
        /// </summary>
        private bool createBridge2BridgeAssociation;

        /// <summary>
        /// Свойство для определения рисовать ассоциацию Мастер - деталь или нет 
        /// </summary>
        private bool createMasterDetailAssociation;

        #endregion

        #region Constructor

        public DiargamEditor()
        {
            InitializeComponent();

            InitializeImageList();

            // InitializeCommands();

            // прямоугольник выделения
            rc = new RECT();

            // момент начала создания ассоциации
            BeginDrawAssociate = false;

            // double-buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            // allow scrolling
            this.AutoScroll = true;
            this.HScroll = true;
            this.VScroll = true;

            this.AllowDrop = true;
            this.DragOver += new DragEventHandler(DiargamEditor_DragOver);
            this.DragDrop += new DragEventHandler(DiargamEditor_DragDrop);

            this.Scroll += new ScrollEventHandler(DiargamEditor_Scroll);
            this.defaultSettings.PageSettingsChange += new EventHandler(DefaultPageSettings_PageSettingsChange);

            undoredoManager = new UndoRedo();
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Событие при изменении диаграммы
        /// </summary>
        public event EventHandler ChangeDiagram;

        #endregion Events

        #region Properties 

        public HelperCommand CmdHelper
        {
            get { return cmdHelper; }
            set { cmdHelper = value; }
        }

        public Command CmdRedo
        {
            get
            {
                return (Command)Diagram.DiagramСommands[AbstractDiagram.RedoContextMenuItemKey];
            }
        }

        public Command CmdUndo
        {
            get
            {
                return (Command)Diagram.DiagramСommands[AbstractDiagram.UndoContextMenuItemKey];
            }
        }

        public Command CmdDragDrop
        {
            get { return cmdDragDrop; }
        }

        public DefaultSettings DefaultSettings
        {
            get { return defaultSettings; }
            set { defaultSettings = value; }
        }

        public DiagramEntity SelectedShape
        {
            get { return selectedShape; }
            set { selectedShape = value; }
        }

        public List<Point> Points
        {
            get { return points; }
            set { points = value; }
        }

        /// <summary>
        /// Редактор схемы
        /// </summary>
        public ISchemeEditor SchemeEditor
        {
            get { return schemeEditor; }
            set { schemeEditor = value; }
        }

        public Command CmdSave
        {
            get
            {
                return (Command)Diagram.DiagramСommands[AbstractDiagram.SaveContextMenuItemKey];
            }
        }

        public Dictionary<Images, Bitmap> ImageList
        {
            get { return imageList; }
        }

        public UndoRedo UndoredoManager
        {
            get 
            {
                if (undoredoManager.Site == null)
                {
                    undoredoManager.Site = this;
                }

                return undoredoManager; 
            }

            set
            {
                undoredoManager = value;
            }
        }

        public bool BeginDrawAssociate
        {
            get { return beginDrawAssociate; }
            set { beginDrawAssociate = value; }
        }

        /// <summary>
        /// Масштаб диаграммы
        /// </summary>
        public int ZoomFactor
        {
            get { return zoom; }
            set { zoom = value; }
        }

        /// <summary>
        /// Координаты последней точки
        /// </summary>
        public Point LastClickPoint
        {
            get { return lastClickPoint; }
            set { lastClickPoint = value; }
        }

        /// <summary>
        /// Свойство для определения рисовать ассоциацию или нет
        /// </summary>
        public bool CreateAssociation
        {
            get
            {
                return createAssociation;
            }

            set
            {
                if (newAssociationCommand == null && diagram != null)
                {
                    throw new Exception("Инструмент создания ассоциации не установлен.");
                }

                createAssociation = value;
            }
        }

        public bool CreateBridgeAssociation
        {
            get
            {
                return createBridgeAssociation;
            }

            set
            {
                if (newAssociationCommand == null && diagram != null)
                {
                    throw new Exception("Инструмент создания ассоциации Мастер-деталь не установлен.");
                }

                createBridgeAssociation = value;
            }
        }

        public bool CreateBridge2BridgeAssociation
        {
            get
            {
                return createBridge2BridgeAssociation;
            }

            set
            {
                if (newAssociationCommand == null && diagram != null)
                {
                    throw new Exception("Инструмент создания ассоциации Сопоставления версии сопоставимого не установлен.");
                }

                createBridge2BridgeAssociation = value;
            }
        }

        public bool CreateMasterDetailAssociation
        {
            get
            {
                return createMasterDetailAssociation;
            }

            set 
            {
                if (newAssociationCommand == null && diagram != null)
                {
                    throw new Exception("Инструмент создания ассоциации Мастер-деталь не установлен.");
                }

                createMasterDetailAssociation = value;
            }
        }

        /// <summary>
        /// Команда для создания новой ассоциации в схеме
        /// </summary>
        public INewAssociationCommand NewAssociationCommand
        {
            get { return newAssociationCommand; }
            set { newAssociationCommand = value; }
        }
       
        /// <summary>
        /// Команда для создания нового класса в схеме
        /// </summary>
        public INewEntityCommand NewEntityCommand
        {
            get { return newEntityCommand; }
            set { newEntityCommand = value; }
        }

        /// <summary>
        /// Команда для создания нового пакета в схеме
        /// </summary>
        public INewPackageCommand NewPackageCommand
        {
            get { return newPackageCommand; }
            set { newPackageCommand = value; }
        }

        public new ZoomControl ScaleControl
        {
            get { return scaleControl; }
        }

        /// <summary>
        /// Внешний тулбар для диаграммы.
        /// </summary>
        public Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ToolbarsManager
        {
            get { return toolbarsManager; }
            set { toolbarsManager = value; }
        }

        /// <summary>
        /// Диаграмма классов.
        /// </summary>
        public AbstractDiagram Diagram
        {
            get
            {
                return diagram;
            }

            set
            {
                if (value != null)
                {
                    diagram = value;
                    InitializeCommands();
                }
            }
        }

        /// <summary>
        /// Была ли изменена диаграмма
        /// </summary>
        internal bool IsChanged
        {
            get
            {
                return Diagram.IsChanged;
            }

            set
            {
                if (diagram != null)
                {
                    if (Parent.Parent is Form)
                    {
                        string name = ((Form)Parent.Parent).Text;
                        if (name[name.Length - 1] != '*')
                        {
                            ((Form)Parent.Parent).Text = name + "*";
                        }
                    }

                    Change();
                }
            }
        }

        /// <summary>
        /// Коллекция выделенных элементов
        /// </summary>
        internal List<DiagramEntity> SelectedEntities
        {
            get { return selectedEntities; }
            set { selectedEntities = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Обработчик событий
        /// </summary>
        public void OnChangeDiagram(EventArgs args)
        {
            if (ChangeDiagram != null)
            {
                ChangeDiagram(this, args);
            }

            CmdSave.Enabled = true;
        }

        #endregion Events

        #region Methods

        #region Работа с диаграммой

        /// <summary>
        /// Сохранить диаграмму
        /// </summary>
        public void SaveDiagram()
        {
            diagram.Save();

            // очищаем стеки Undo/Redo
            undoredoManager.Flush();
            IsChanged = false;
            Diagram.IsChanged = false;

            if (Parent.Parent is Form)
            {
                string name = ((Form)Parent.Parent).Text;
                if (name[name.Length - 1] == '*')
                {
                    ((Form)Parent.Parent).Text = name.Substring(0, name.Length - 1);
                }
            }
        }

        public void DrawTracker(Graphics g, Size scrollOffset)
        {
            foreach (DiagramEntity entity in selectedEntities)
            {
                entity.DrawTracker(g, scrollOffset);
            }
        }

        #endregion Рвбота с диаграммой      
        
        /// <summary>
        /// Handles the mouse-down event
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.CanFocus)
            {
                this.Focus();
            }

            Size scrollOffset = new Size(this.AutoScrollPosition);

            Point point = lastClickPoint = new Point(e.X - scrollOffset.Width, e.Y - scrollOffset.Height);

            // нажата левая клавиша
            if (e.Button == MouseButtons.Left)
            {
                if (cmdHelper.SelectedCollection(point).Count == 0)
                {
                    cmdHelper.UnSelectAll();
                }
                else if ((selectedEntities.Count < 2 && Control.ModifierKeys != Keys.Control) || (Control.ModifierKeys != Keys.Control && cmdHelper.SelectedCollection(point).Count == 1 && selectedEntities.Count > 1 && !selectedEntities.Contains(cmdHelper.SelectedCollection(point)[0])))
                {
                    cmdHelper.UnSelectAll();

                    selectedEntities.Add(cmdHelper.SelectedCollection(point)[0]);
                                        
                    selectedEntities[0].IsSelected = true;

                    // GetPropertiesObjetct(selectedEntities[0].Key);
                }
                else if (Control.ModifierKeys == Keys.Control)
                {
                    if (selectedEntities.Contains(cmdHelper.SelectedCollection(point)[0]))
                    {
                        selectedEntities.Remove(cmdHelper.SelectedCollection(point)[0]);
                        cmdHelper.SelectedCollection(point)[0].IsSelected = false;
                    }
                    else
                    {
                        selectedEntities.Add(cmdHelper.SelectedCollection(point)[0]);
                    }

                    foreach (DiagramEntity entity in selectedEntities)
                    {
                        entity.IsSelected = true;
                    }
                }

                if (selectedEntities.Count == 1)
                {
                    if (CreateAssociation || CreateBridgeAssociation || CreateMasterDetailAssociation || CreateBridge2BridgeAssociation)
                    {
                        if (selectedShape == null)
                        {
                            selectedShape = selectedEntities[0];
                            points.Add(selectedShape.Coordinate());

                            lastClickPoint = selectedShape.Coordinate();
                        }

                        beginDrawAssociate = true;
                        mouseDown = true;
                    }
                    
                    if (selectedEntities[0].HitTest(point) != -1)
                    {
                        sizeBeforeChange = new Size(selectedEntities[0].EntityRectangle.Width, selectedEntities[0].EntityRectangle.Height);
                        isPointSelected = true;
                        toolPointer.OnMouseDown(this, e, scrollOffset, selectedEntities[0]);
                    }
                }
            }
            else
            {
                // нажата правая клавиша грызуна
                if (cmdHelper.SelectedCollection(point).Count == 0)
                {
                    cmdHelper.UnSelectAll();
                    contextMenuDiagramEditor.Show(this, new Point(e.X, e.Y));
                }
                else if (cmdHelper.SelectedCollection(point).Count == 1)
                {
                    if (selectedEntities.Count < 2)
                    {
                        cmdHelper.UnSelectAll();

                        selectedEntities.Add(cmdHelper.SelectedCollection(point)[0]);
                        selectedEntities[0].IsSelected = true;

                        selectedEntities[0].ContextMenuInitialize();
                        selectedEntities[0].ContextMenuShow(new Point(e.X, e.Y));
                    }
                    else
                    {
                        // Отображаем контекстное меню объекта, на кот щелкнули
                        cmdHelper.SelectedCollection(point)[0].ContextMenuInitialize();
                        cmdHelper.SelectedCollection(point)[0].ContextMenuShow(new Point(e.X, e.Y));
                    }
                }
            }

            rectangle = cmdHelper.MovingRec(selectedEntities, scrollOffset);
            rectangle.Offset(scrollOffset.Width, scrollOffset.Height);

            startRect = rectangle;

            // useful for all kind of things
            refp = point; 
        }                              

        /// <summary>
        /// Handles the mouse-move event
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            intPtrhWnd = GetHWnd(this);
            intPtrhDC = GetDC(intPtrhWnd);

            try
            {
                Size scrollOffset = new Size(this.AutoScrollPosition);

                Point point = new Point(e.X - scrollOffset.Width, e.Y - scrollOffset.Height);

                // ассоциация
                if ((CreateAssociation || CreateBridgeAssociation || CreateMasterDetailAssociation || CreateBridge2BridgeAssociation) && selectedShape != null)
                {
                    Point lastInCollection = new Point();
                    if (points.Count != 0)
                    {
                        lastInCollection = points[points.Count - 1];
                    }

                    lastInCollection = ScaleTransform.SimpleTransformPoint(lastInCollection, ZoomFactor);

                    point = cmdHelper.ValidatePoint(point);

                    lastInCollection.Offset(scrollOffset.Width, scrollOffset.Height);
                    point.Offset(scrollOffset.Width, scrollOffset.Height);

                    if (!beginDrawAssociate)
                    {
                        ControlPaint.DrawReversibleLine(PointToScreen(lastInCollection), PointToScreen(lastClickPoint), SystemColors.ControlLight);
                    }

                    lastClickPoint = point;
                    ControlPaint.DrawReversibleLine(PointToScreen(lastInCollection), PointToScreen(lastClickPoint), SystemColors.ControlLight);
                    beginDrawAssociate = false;

                    return;
                }

                // растягивание
                if (selectedEntities.Count > 0 && isPointSelected)
                {
                    DrawFocusRect(intPtrhDC, ref rc);

                    toolPointer.OnMouseMove(this, e, scrollOffset, selectedEntities[0]);

                    if (selectedEntities[0] is UMLAssociationBase)
                    {
                        return;
                    }

                    Rectangle r = ScaleTransform.TransformRectangle(selectedEntities[0].EntityRectangle, ZoomFactor);

                    rc.Left = r.Left + scrollOffset.Width;
                    rc.Right = rc.Left + r.Width;
                    rc.Top = r.Top + scrollOffset.Height;
                    rc.Bottom = rc.Top + r.Height;

                    DrawFocusRect(intPtrhDC, ref rc);

                    return;
                }

                if (e.Button == MouseButtons.None)
                {
                    if (selectedEntities.Count == 1 && selectedEntities[0].HitTest(point) != -1)
                    {
                        toolPointer.OnMouseMove(this, e, scrollOffset, selectedEntities[0]);
                        return;
                    }
                    else if (!CreateAssociation && !CreateBridgeAssociation && !CreateMasterDetailAssociation && !CreateBridge2BridgeAssociation)
                    {
                        this.Cursor = DefaultCursor;
                    }
                }

                // выделяемая область
                if (selectedEntities.Count == 0 && e.Button == MouseButtons.Left)
                {
                    if (mouseDown)
                    {
                        DrawFocusRect(intPtrhDC, ref rc);

                        selectRect = DiagramRectangleEntity.GetNormalizedRectangle(lastClickPoint, point);
                        selectRect.Offset(scrollOffset.Width, scrollOffset.Height);

                        rc.Left = selectRect.Left;
                        rc.Right = selectRect.Right;
                        rc.Top = selectRect.Top;
                        rc.Bottom = selectRect.Bottom;

                        DrawFocusRect(intPtrhDC, ref rc);
                    }

                    mouseDown = true;
                }

                // moving
                if (selectedEntities.Count > 0 && selectedEntities[0].HitTest(point) == -1 && e.Button == MouseButtons.Left)
                {
                    if (mouseMoving)
                    {
                        DrawFocusRect(intPtrhDC, ref rc);

                        rectangle.X += point.X - refp.X;
                        rectangle.Y += point.Y - refp.Y;
                        refp = point;

                        rc.Left = rectangle.Left;
                        rc.Right = rectangle.Right;
                        rc.Top = rectangle.Top;
                        rc.Bottom = rectangle.Bottom;

                        DrawFocusRect(intPtrhDC, ref rc);
                    }

                    mouseMoving = true;
                }
            }
            finally
            {
                ReleaseDC(intPtrhWnd, intPtrhDC);
            }
        }

        /// <summary>
        /// Handles the mouse-up event
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Cursor = DefaultCursor;

            if (e.Button == MouseButtons.Right)
            {
                cmdHelper.FinalizeCreateAssociation();
            }

            Size scrollOffset = new Size(this.AutoScrollPosition);

            Point point = new Point(e.X - scrollOffset.Width, e.Y - scrollOffset.Height);

            // перемещение
            if (selectedEntities.Count > 0 && !CreateAssociation && mouseMoving)
            {
                Point pointBeforeChange = selectedEntities[0].EntityRectangle.Location;
                foreach (DiagramEntity entity in selectedEntities)
                {
                    if (rectangle.Left > scrollOffset.Width && rectangle.Top > scrollOffset.Height)
                    {
                        entity.Move(new Point(point.X - lastClickPoint.X, point.Y - lastClickPoint.Y));
                    }
                    else if (rectangle.Left < scrollOffset.Width && rectangle.Top < scrollOffset.Height)
                    {
                        int x = lastClickPoint.X - startRect.Left;
                        int y = lastClickPoint.Y - startRect.Top;
                        entity.Move(new Point(x - lastClickPoint.X, y - lastClickPoint.Y));
                    }
                    else if (rectangle.Left < scrollOffset.Width)
                    {
                        int x = lastClickPoint.X - startRect.Left;
                        entity.Move(new Point(x - lastClickPoint.X, point.Y - lastClickPoint.Y));
                    }
                    else if (rectangle.Top < scrollOffset.Height)
                    {
                        int y = lastClickPoint.Y - startRect.Top;
                        entity.Move(new Point(point.X - lastClickPoint.X, y - lastClickPoint.Y));
                    }

                    if (entity is DiagramRectangleEntity)
                    {
                        (entity as DiagramRectangleEntity).DisposeHeaderTextBox();
                    }
                }

                // добавляем с стек
                UndoredoManager.Do(new CommandChangeLocation(selectedEntities, new Size(selectedEntities[0].EntityRectangle.Location.X - pointBeforeChange.X, selectedEntities[0].EntityRectangle.Location.Y - pointBeforeChange.Y)));

                IsChanged = true;
                mouseMoving = false;

                this.Invalidate();

                rc.Bottom = rc.Left = rc.Right = rc.Top = 0;
            }

            // выделение
            if (mouseDown)
            {
                selectedEntities = cmdHelper.SelectFromRect(selectRect, scrollOffset);

                mouseDown = false;

                selectRect.X = selectRect.Y = selectRect.Width = selectRect.Height = 0;
                rc.Bottom = rc.Left = rc.Right = rc.Top = 0;
            }

            // изменение размеров
            if (selectedEntities.Count > 0 && isPointSelected)
            {
                toolPointer.OnMouseUp(this, e, scrollOffset, selectedEntities[0]);

                undoredoManager.Do(new CommandChangeSize(selectedEntities[0], new Size(selectedEntities[0].EntityRectangle.Width - sizeBeforeChange.Width, selectedEntities[0].EntityRectangle.Height - sizeBeforeChange.Height)));
                isPointSelected = false;
                IsChanged = true;
            }

            // ассоциация
            if (CreateAssociation || CreateBridgeAssociation || CreateMasterDetailAssociation || CreateBridge2BridgeAssociation)
            {
                if (cmdHelper.SelectedCollection(point).Count == 1)
                {
                    CommandNewAssosiationBase cmdNewAssociate;

                    if (CreateMasterDetailAssociation)
                    {
                        cmdNewAssociate = new CreateNewMDAssociation(diagram);
                    }
                    else if (CreateBridgeAssociation)
                    {
                        cmdNewAssociate = new CreateNewBridgeAssociation(diagram);
                    }
                    else if (CreateBridge2BridgeAssociation)
                    {
                        cmdNewAssociate = new CreateNewBridge2BridgeAssociation(diagram);
                    }
                    else
                    {
                        cmdNewAssociate = new CreateNewAssociation(diagram);
                    }

                    cmdNewAssociate.Point = point;
                    cmdNewAssociate.Execute();

                    this.Cursor = DefaultCursor;
                }
                else
                {
                    point = cmdHelper.CheckPoint(point);
                    points.Add(ScaleTransform.TransformPoint(point, ZoomFactor));
                    lastClickPoint = point;
                    beginDrawAssociate = true;
                }
            }

            Invalidate(false);
        }
       
        /// <summary>
        /// Paints the background
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            this.BackColor = Color.White;
            Size scrollOffset = new Size(this.AutoScrollPosition);
            base.OnPaintBackground(e);

            Graphics g = e.Graphics;

            g.DrawString(
                "DiargamEditor Control [version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "]",
                Font,
                Brushes.SlateGray,
                new Point(20 + scrollOffset.Width, 10 + scrollOffset.Height));
        }

        /// <summary>
        /// Отрисовка диаграммы
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Size scrollOffset = ScaleTransform.TransformSize(new Size(this.AutoScrollPosition), ZoomFactor);

            Graphics g = e.Graphics;

            // use the best quality, with a performance penalty
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            g.SetClip(e.ClipRectangle);
             
            // zoom
            g.ScaleTransform((float)ZoomFactor / 100, (float)ZoomFactor / 100);

            if (diagram != null)
            {
                foreach (DiagramEntity entity in Diagram.Entities)
                {
                    entity.BeforeDraw();
                    entity.Draw(g, scrollOffset);
                    entity.AfterDraw();
                }

                if (CreateAssociation || CreateBridgeAssociation || CreateMasterDetailAssociation)
                {
                    cmdHelper.DrawAssociate(g, scrollOffset);
                }

                DrawTracker(g, scrollOffset);
            }

            Size diagramSize = cmdHelper.GetDiagramSize(ZoomFactor);

            this.AutoScrollMinSize = new Size(diagramSize.Width + 15, diagramSize.Height + 15);
        }

        #region HelperFunc

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool DrawFocusRect(IntPtr hDC, ref RECT lprc);

        [DllImport("user32.dll")]
        private static extern IntPtr SetCapture(IntPtr intPtrhWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetCapture();

        private static IntPtr GetHWnd(Control ctrl)
        {
            IntPtr intPtrhOldWnd = GetCapture();
            IntPtr intPtrhWnd = GetCapture();
            SetCapture(intPtrhOldWnd);
            return intPtrhWnd;
        }

        /// <summary>
        /// Возвращает контекст устройства, подготовленный к выводу только в границах
        /// клиентской части окна
        /// </summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>
        /// Освобождает ресурсы, связанные с манипулятором
        /// контекста устройства, полученным при вызове GetWindowDC, GetDC или GetDCEx
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        #endregion

        /// <summary>
        /// Изменение диаграммы
        /// </summary>
        private void Change()
        {
            EventArgs args = new EventArgs();
            OnChangeDiagram(args);
        }

        private void DefaultPageSettings_PageSettingsChange(object sender, EventArgs e)
        {
            ((CommandPrintPreview)cmdPrintPreview).InitializeMetafile();
            ((CommandPrintOnePage)cmdPrintOnePage).InitializeMetafile();
        }

        /// <summary>
        /// Double click
        /// </summary>
        private void DiargamEditor_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (cmdHelper.SelectedCollection(lastClickPoint).Count == 0)
            {
                cmdHelper.UnSelectAll();
            }

            if (cmdHelper.SelectedCollection(lastClickPoint).Count == 1)
            {
                cmdHelper.SelectedCollection(lastClickPoint)[0].OnMouseDoubleClick(sender, e);
            }
        }

        /// <summary>
        /// Перерисовка при скроллинге
        /// </summary>
        private void DiargamEditor_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Перетаскивание с дерева
        /// </summary>
        private void DiargamEditor_DragDrop(object sender, DragEventArgs e)
        {
            if (cmdDragDrop is CommandDragDrop)
            {
                ((CommandDragDrop)cmdDragDrop).Execute(e);
            }
        }

        private void DiargamEditor_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            Size scrollOffset = new Size(AutoScrollPosition);

            Point clientPoint = PointToClient(new Point(e.X, e.Y));
            clientPoint.Offset(-scrollOffset.Width, -scrollOffset.Height);
            clientPoint = ScaleTransform.TransformPoint(clientPoint, ZoomFactor);

            List<DiagramEntity> mouseOverEntityList = cmdHelper.SelectedCollection(clientPoint);
            if (mouseOverEntityList.Count != 0)
            {
                foreach (DiagramEntity mouseOverEntity in mouseOverEntityList)
                {
                    mouseOverEntity.OnDragOver(this, e);
                    return;
                }
            }

            SelectedNodesCollection selectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));
            foreach (UltraTreeNode node in selectedNodes)
            {
                if (!diagram.IsAllowedEntity(node.Tag))
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
            }

            e.Effect = DragDropEffects.All;
        }

        private ToolStripItem InitializeContextMenuCommand(string key, BaseCommand cmd)
        {
            ToolStripItem mi;

            if (cmd != null)
            {
                mi = new ToolStripMenuItem();
                mi.Name = key;
                mi.Text = key.Split(MenuPathSeparator)[key.Split(MenuPathSeparator).GetLength(0) - 1];
                mi.Tag = cmd;
                mi.Image = cmd.Image;
                mi.Enabled = cmd.Enabled;
                mi.Click += new EventHandler(ContextMenuItem_Click);
                cmd.OnChangeEnabled += new EventHandler(Command_OnChangeEnabled);
            }
            else
            {
                mi = new ToolStripSeparator();
                mi.Name = key;
            }

            return mi;
        }

        private ToolStripItem InitializeContextSubMenuCommand(ToolStripItemCollection toolStripItemCollection, int nestedLevel, string key, BaseCommand cmd)
        {
            ToolStripItem mi;

            string[] path = key.Split(MenuPathSeparator);

            ToolStripMenuItem smi;

            if (toolStripItemCollection.ContainsKey(path[nestedLevel]))
            {
                smi = (ToolStripMenuItem)toolStripItemCollection[path[nestedLevel]];
            }
            else
            {
                smi = new ToolStripMenuItem(path[nestedLevel].Substring(1, path[nestedLevel].Length - 1));
                smi.Name = path[nestedLevel];
            }

            if (path[nestedLevel + 1][0] == SubMenuPrefixKey)
            {
                mi = InitializeContextSubMenuCommand(smi.DropDownItems, nestedLevel + 1, key, cmd);
            }
            else
            {
                mi = InitializeContextMenuCommand(key, cmd);
            }

            smi.DropDownItems.Add(mi);
            return smi;
        }

        private void Command_OnChangeEnabled(object sender, EventArgs e)
        {
            BaseCommand cmd = (BaseCommand)sender;

            // TODO: Сделать рекурсивный обход вложенных меню
            foreach (ToolStripItem item in contextMenuDiagramEditor.Items)
            {
                if (item.Tag == cmd)
                {
                    item.Enabled = cmd.Enabled;
                    return;
                }
            }
        }

        private void ContextMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = sender as ToolStripItem;
            if (tsi != null)
            {
                ICommand command = tsi.Tag as ICommand;
                if (command != null)
                {
                    command.Execute();
                }
            }
        }

        private void InitializeContextMenu()
        {
            contextMenuDiagramEditor.Items.Clear();

            foreach (KeyValuePair<string, BaseCommand> item in diagram.DiagramСommands)
            {
                string[] path = item.Key.Split(MenuPathSeparator);

                if (path[1] != ContextMenuKey)
                {
                    continue;
                }

                ToolStripItem mi;
                if (path[2][0] == SubMenuPrefixKey)
                {
                    mi = InitializeContextSubMenuCommand(contextMenuDiagramEditor.Items, 2, item.Key, item.Value);
                }
                else
                {
                    mi = InitializeContextMenuCommand(item.Key, item.Value);
                }

                contextMenuDiagramEditor.Items.Add(mi);
            }
        }

        private void InitializeToolbar()
        {
            foreach (KeyValuePair<string, BaseCommand> item in diagram.DiagramСommands)
            {
                string[] path = item.Key.Split(MenuPathSeparator);

                if (path[1] != ToolBarKey)
                {
                    continue;
                }

                string toolKey = path[2];

                ToolBase buttonTool = new ButtonTool(toolKey);
                buttonTool.SharedProps.Caption = toolKey;
                buttonTool.SharedProps.Enabled = item.Value.Enabled;
                buttonTool.SharedProps.AppearancesSmall.Appearance.Image = item.Value.Image;
                buttonTool.SharedProps.Tag = item.Value;
                buttonTool.ToolClick += new ToolClickEventHandler(ButtonTool_ToolClick);
                item.Value.OnChangeEnabled += new EventHandler(ToolbarCommand_OnChangeEnabled);

                toolbarsManager.Tools.Add(buttonTool);
                toolbarsManager.Toolbars["ToolBar"].Tools.AddTool(buttonTool.Key);
            }

            scaleControl = new ZoomControl();
            scaleControl.Location = new System.Drawing.Point(0, 0);
            scaleControl.Name = "scaleControl";
            scaleControl.ScaleFactor = ZoomFactor;
            scaleControl.Size = new System.Drawing.Size(94, 23);
            scaleControl.TabIndex = 0;
            scaleControl.ScaleChangeEvent += new ZoomControl.ScaleEventHandler(ScaleControl_ScaleChangeEvent);

            ControlContainerTool controlContainerTool = new ControlContainerTool("ControlContainerScale");
            controlContainerTool.Control = scaleControl;
            controlContainerTool.SharedProps.Caption = "ControlContainerScale";
            controlContainerTool.SharedProps.Width = 96;
            toolbarsManager.Tools.Add(controlContainerTool);
            toolbarsManager.Toolbars["ToolBar"].Tools.AddTool(controlContainerTool.Key);
        }

        private void ToolbarCommand_OnChangeEnabled(object sender, EventArgs e)
        {
            BaseCommand cmd = (BaseCommand)sender;

            foreach (ToolBase item in toolbarsManager.Tools)
            {
                if (item.SharedProps.Tag == cmd)
                {
                    item.SharedProps.Enabled = cmd.Enabled;
                }
            }
        }

        private void ScaleControl_ScaleChangeEvent(object sender, ScaleEventArgs args)
        {
            cmdHelper.ScaleUpdate(args.ScaleFactor);
        }

        private void ButtonTool_ToolClick(object sender, ToolClickEventArgs e)
        {
            ICommand command = e.Tool.SharedProps.Tag as ICommand;
            if (command != null)
            {
                command.Execute();
            }
        }

        /// <summary>
        /// Инициализация команд
        /// </summary>
        private void InitializeCommands()
        {
            if (diagram != null)
            {
                InitializeContextMenu();

                if (toolbarsManager != null)
                {
                    InitializeToolbar();
                }
            }

            this.cmdKey = new CommandKeyPress(diagram);
            this.cmdHelper = new HelperCommand(diagram);

            // команда печати на одной странице
            cmdPrintOnePage = new CommandPrintOnePage(diagram);

            this.cmdDragDrop = new CommandDragDrop(diagram);
        }
       
        #endregion
        
        #region Structure

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion
    }
}
