using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый класс для ассоциаций всех типов
    /// </summary>
    public class UMLAssociationBase : DiagramEntity
    {
        #region Fields

        /// <summary>
        /// Start object
        /// </summary>
        private DiagramEntity parentDiagramEntity;
        
        /// <summary>
        /// ChildDiagramEntity object
        /// </summary>
        private DiagramEntity childDiagramEntity;
        
        /// <summary>
        /// Object GraphicsPath Pen
        /// </summary>
        private Pen grfxPen = new Pen(Color.Black, 8);
                
        /// <summary>
        /// Center association
        /// </summary>
        private Point center;
        
        /// <summary>
        /// Start asociation
        /// </summary>
        private Point startPoint;
        
        /// <summary>
        /// End association
        /// </summary>
        private Point endPoint;

        /// <summary>
        /// Коллекция узлов ассоциации 
        /// </summary>
        private List<Point> selectedPoints = new List<Point>();
                       
        #endregion

        #region Constructor

        public UMLAssociationBase(string key, Guid id, AbstractDiagram diagram, DiagramEntity parentDiagramEntity, DiagramEntity childDiagramEntity, List<Point> selPoints)
            : base(key, id, diagram)
        {
            this.parentDiagramEntity = parentDiagramEntity;
            this.childDiagramEntity = childDiagramEntity;

            foreach (Point p in selPoints)
            {
                selectedPoints.Add(p);
            }

            CountOfPoint = selectedPoints.Count;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Центр ассоциации
        /// </summary>
        public Point Center
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// Количество точек выделения
        /// </summary>
        public override int HandleCount
        {
            get
            {
                return CountOfPoint;
            }

            set
            {
                CountOfPoint = value;
            }
        }

        /// <summary>
        /// Прямоугольник для ассоциации содержит все её точки
        /// </summary>
        public override Rectangle EntityRectangle
        {
            get
            {
                System.Drawing.Rectangle rectangle = new Rectangle();
                foreach (Point point in selectedPoints)
                {
                    rectangle.X = Math.Min(base.EntityRectangle.Left, point.X);
                    rectangle.Y = Math.Min(base.EntityRectangle.Top, point.Y);
                    rectangle.Width = Math.Max(base.EntityRectangle.Width, point.X - base.EntityRectangle.X);
                    rectangle.Height = Math.Max(base.EntityRectangle.Height, point.Y - base.EntityRectangle.Y);
                }

                return rectangle;
            }
        }
        
        /// <summary>
        /// Доступ к коллекции узлов ассоциации
        /// </summary>
        public List<Point> ListOfPoints
        {
            get { return selectedPoints; }
            set { selectedPoints = value; }
        }

        /// <summary>
        /// Начало ассоциации
        /// </summary>
        public DiagramEntity ParentDiagramEntity
        {
            get { return parentDiagramEntity; }
        }

        /// <summary>
        /// Конец ассоциации
        /// </summary>
        public DiagramEntity ChildDiagramEntity
        {
            get { return childDiagramEntity; }
        }

        protected DiagramEntity ParentDiagramEntity1
        {
            get { return parentDiagramEntity; }
            set { parentDiagramEntity = value; }
        }

        protected DiagramEntity ChildDiagramEntity1
        {
            get { return childDiagramEntity; }
            set { childDiagramEntity = value; }
        }

        protected List<Point> SelectedPoints
        {
            get { return selectedPoints; }
            set { selectedPoints = value; }
        }

        #endregion
        
        #region StaticMethods

        public static string GetStereotypeNameByAssociationClassType(AssociationClassTypes associationClassTypes)
        {
            switch (associationClassTypes)
            {
                case AssociationClassTypes.Link:
                    return "Ссылка";

                case AssociationClassTypes.Bridge:
                case AssociationClassTypes.BridgeBridge:
                    return "Сопоставление";

                case AssociationClassTypes.MasterDetail:
                    return "Мастер-деталь";

                default:
                    return String.Empty;
            }
        }
        #endregion

        #region HelperFunc

        /// <summary>
        /// Центр Отрезка
        /// </summary>
        /// <returns> Центр ассоциации</returns>
        public Point GetCenter(Point startP, Point endP)
        {
            Point associateCenter = Point.Empty;

            try
            {
                associateCenter.X = (startP.X + endP.X) / 2;
                associateCenter.Y = (startP.Y + endP.Y) / 2;
            }
            catch (Exception e)
            {
                throw new Exception("Что-то не так" + e.Message);
            }

            return associateCenter;
        }
        #endregion

        #region Metods
        
        public override void RemoveEntity()
        {
            base.RemoveEntity();
        }

        public override void Draw(Graphics g, Size scrollOffset)
        {
            // реальные концы ассоциациии(в цетрах)
            int ax = 0, ay = 0, cx = 0, cy = 0;
            if (parentDiagramEntity != null && childDiagramEntity != null)
            {
                // координаты точек начала и конца
                ax = (parentDiagramEntity.EntityRectangle.Width / 2) + parentDiagramEntity.EntityRectangle.Left;
                ay = (parentDiagramEntity.EntityRectangle.Height / 2) + parentDiagramEntity.EntityRectangle.Top;

                cx = (childDiagramEntity.EntityRectangle.Width / 2) + childDiagramEntity.EntityRectangle.Left;
                cy = (childDiagramEntity.EntityRectangle.Height / 2) + childDiagramEntity.EntityRectangle.Top;
            }
            else
            {
                // У ассоциации отсутствует одна из ролей
                // и мы её не рисуем
                return;
            }

            EntityRectangle = new Rectangle(Math.Min(ax, cx), Math.Min(ay, cy), Math.Abs(ax - cx), Math.Abs(ay - cy));

            // стрелка
            if (selectedPoints.Count == 2)
            {
                if (parentDiagramEntity.EntityRectangle.IntersectsWith(childDiagramEntity.EntityRectangle))
                {
                    endPoint = new Point(cx, cy);
                    startPoint = new Point(ax, ay);
                }
                else
                {
                    endPoint = SecPointCrosssOptimal(new Point(ax, ay), new Point(cx, cy), childDiagramEntity.EntityRectangle);
                    startPoint = SecPointCrosssOptimal(new Point(cx, cy), new Point(ax, ay), parentDiagramEntity.EntityRectangle);
                }
            }
            else
            {
                endPoint =
                    SecPointCrosssOptimal(
                        new Point(selectedPoints[selectedPoints.Count - 2].X, selectedPoints[selectedPoints.Count - 2].Y),
                        new Point(cx, cy),
                        childDiagramEntity.EntityRectangle);
                startPoint = SecPointCrosssOptimal(selectedPoints[1], new Point(ax, ay), parentDiagramEntity.EntityRectangle);
            }

            center = GetCenter(startPoint, endPoint);

            selectedPoints.Remove(selectedPoints[0]);
            selectedPoints.Remove(selectedPoints[selectedPoints.Count - 1]);

            selectedPoints.Insert(0, startPoint);
            selectedPoints.Add(endPoint);

            Point[] po = new Point[selectedPoints.Count];

            int i = 0;
            foreach (Point point in selectedPoints)
            {
                point.Offset(scrollOffset.Width, scrollOffset.Height);
                po.SetValue(point, i);
                i++;
            }

            // толщина основной линии
            Pen.Width = this.LineWidth;

            // сначало будем рисовать линию без наконечника(основная)
            g.DrawLines(Pen, po);

            // изменяем размер
            ResizeGrPath(selectedPoints);
        }
        
        /// <summary>
        /// Пересечение ассоциации с областью выделения
        /// </summary>
        public override bool IntersectWith(Rectangle rectangle)
        {
            Region region = new Region(GraphicsPath);

            return region.IsVisible(ScaleTransform.ReverseTransformRectangle(rectangle, Diagram.Site.ZoomFactor));
        }

        /// <summary>
        /// Вид курсора
        /// </summary>
        public override System.Windows.Forms.Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Cross;
        }

        public override void MoveHandleTo(Point point, int handleNumber)
        {
            Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

            // Есть соседние точки???
            if (handleNumber - 1 != 0 && handleNumber - 1 != selectedPoints.Count - 1)
            {
                point = Diagram.Site.CmdHelper.ValidatePoint(point);

                Point p = Point.Empty;
                if (handleNumber == selectedPoints.Count - 1)
                {
                    p = this.ChildDiagramEntity.Coordinate();
                }
                else
                {
                    p = selectedPoints[handleNumber];
                }

                Point p2 = Point.Empty;
                if (handleNumber - 2 == 0)
                {
                    p2 = this.ParentDiagramEntity.Coordinate();
                }
                else
                {
                    p2 = selectedPoints[handleNumber - 2];
                }

                p2 = ScaleTransform.SimpleTransformPoint(p2, Diagram.Site.ZoomFactor);
                p = ScaleTransform.SimpleTransformPoint(p, Diagram.Site.ZoomFactor);

                p2.Offset(scrollOffset.Width, scrollOffset.Height);
                p.Offset(scrollOffset.Width, scrollOffset.Height);
                point.Offset(scrollOffset.Width, scrollOffset.Height);

                if (!Diagram.Site.BeginDrawAssociate)
                {
                    ControlPaint.DrawReversibleLine(
                        Diagram.Site.PointToScreen(Diagram.Site.LastClickPoint),
                        Diagram.Site.PointToScreen(p2),
                        Color.LightGray);
                    ControlPaint.DrawReversibleLine(
                        Diagram.Site.PointToScreen(Diagram.Site.LastClickPoint),
                        Diagram.Site.PointToScreen(p),
                        Color.LightGray);
                }

                Diagram.Site.LastClickPoint = point;

                ControlPaint.DrawReversibleLine(
                    Diagram.Site.PointToScreen(Diagram.Site.LastClickPoint),
                    Diagram.Site.PointToScreen(p2),
                    Color.LightGray);
                ControlPaint.DrawReversibleLine(
                    Diagram.Site.PointToScreen(Diagram.Site.LastClickPoint),
                    Diagram.Site.PointToScreen(p),
                    Color.LightGray);

                Diagram.Site.BeginDrawAssociate = false;
            }
        }

        public void AfterMouseUp(Point point, int handleNumber)
        {
            point = Diagram.Site.CmdHelper.CheckPoint(point);
            selectedPoints[handleNumber - 1] = ScaleTransform.TransformPoint(point, Diagram.Site.ZoomFactor);
        }
        
        /// <summary>
        /// Двойной щелчок на ассоциации
        /// </summary>
        public override void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

            base.OnMouseDoubleClick(sender, e);

            GraphicsPath g = new GraphicsPath();

            foreach (Point p in selectedPoints)
            {
                if (selectedPoints.IndexOf(p) == selectedPoints.Count - 1)
                {
                    return;
                }

                g.AddLine(p, selectedPoints[selectedPoints.IndexOf(p) + 1]);
                g.Widen(grfxPen);

                if (g.IsVisible(ScaleTransform.TransformPoint(Diagram.Site.LastClickPoint, Diagram.Site.ZoomFactor)))
                {
                    selectedPoints.Insert(selectedPoints.IndexOf(p) + 1, ScaleTransform.TransformPoint(Diagram.Site.LastClickPoint, Diagram.Site.ZoomFactor));

                    // Увеличиваем количество точек выделения
                    CountOfPoint++;
                    return;
                }
            }
        }

        /// <summary>
        /// Перемещение ассоциации
        /// </summary>
        public override void Move(Point p)
        {
            selectedPoints = MoveAllPoint(selectedPoints, p);
        }

        public override void Resize(int height, int width)
        {
            base.Resize(height, width);
        }

        public override void Normalize()
        {
            base.Normalize();

            ValidatePoints(selectedPoints);
        }

        /// <summary>
        /// Точка, где находится прямоугольник выделения
        /// </summary>
        public override Point GetHandle(int handleNumber)
        {
            return new Point(selectedPoints[handleNumber - 1].X, selectedPoints[handleNumber - 1].Y);
        }
        
        /// <summary>
        /// Проверка на попадание
        /// </summary>
        public override bool Hit(System.Drawing.Point p)
        {
            if (GraphicsPath.IsVisible(ScaleTransform.TransformPoint(p, Diagram.Site.ZoomFactor)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Попадание в точку выделения
        /// </summary>
        public override int HitTest(Point p)
        {
            if (this.IsSelected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(ScaleTransform.TransformPoint(p, Diagram.Site.ZoomFactor)))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
       
        /// <summary>
        /// Перерисовка ассоциации
        /// </summary>
        public override void Invalidate()
        {
            this.Diagram.Site.Invalidate();
        }

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

        internal override void CutRect(Size minLocation)
        {
            List<Point> cutpoint = new List<Point>();

            foreach (Point point in selectedPoints)
            {
                Point p = point;
                p.Offset(-minLocation.Width + 15, -minLocation.Height + 15);
                cutpoint.Add(p);
            }

            selectedPoints = cutpoint;
        }

        /// <summary>
        /// Новый агоритм получения точки пересечения отрезка в прямоугольником
        /// </summary>
        protected static Point SecPointCrosssOptimal(Point start, Point end, Rectangle rect)
        {
            if (rect.Contains(start))
            {
                return end;
            }

            Point s1 = start;
            Point s2 = end;

            Point midle = Point.Empty;

            for (int i = 0; i < 50; i++)
            {
                midle = new Point((s1.X + s2.X) / 2, (s1.Y + s2.Y) / 2);

                if (rect.Contains(midle))
                {
                    s2 = midle;
                }
                else
                {
                    s1 = midle;
                }
            }

            return midle;
        }

        /// <summary>
        /// Смещение всех точек
        /// </summary>
        protected List<Point> MoveAllPoint(List<Point> points, Point point)
        {
            List<Point> list = new List<Point>();
            foreach (Point p in points)
            {
                Point checkPoint =
                    Diagram.Site.CmdHelper.CheckPoint(
                        new Point(
                            p.X + ((int)(point.X / ((float)Diagram.Site.ZoomFactor / 100))),
                            p.Y + ((int)(point.Y / ((float)Diagram.Site.ZoomFactor / 100)))));
                list.Add(checkPoint);
            }

            return list;
        }

        /// <summary>
        /// Точка пересечения ассоциации и сущности 
        /// </summary>
        protected Point SetPointCross(Point start, Point end, Rectangle rect, Size scrollOffset)
        {
            if (rect.Contains(start))
            {
                return end;
            }

            Point cross = Point.Empty;

            double angle =
                Math.Acos(Math.Sqrt(((start.X - end.X) * (start.X - end.X))) /
                          Math.Sqrt(((start.X - end.X) * (start.X - end.X)) + ((start.Y - end.Y) * (start.Y - end.Y))));
            if (start.X < end.X && start.Y > end.Y)
            {
                double an =
                    Math.Acos(Math.Sqrt(((start.X - rect.Left) * (start.X - rect.Left))) /
                              Math.Sqrt(((start.X - rect.Left) * (start.X - rect.Left)) +
                                        ((start.Y - rect.Bottom) * (start.Y - rect.Bottom))));
                if (start.X > rect.Left)
                {
                    an = Math.PI - an;
                }

                if (start.Y < rect.Bottom)
                {
                    an = an - 1;
                }

                if (angle <= an)
                {
                    cross.X = end.X - (((end.X - start.X) * rect.Height / 2) / (start.Y - end.Y));
                    cross.Y = Math.Min(rect.Bottom, start.Y);
                }
                else
                {
                    cross.X = Math.Max(rect.Left, start.X);
                    cross.Y = end.Y + (((start.Y - end.Y) * rect.Width / 2) / (end.X - start.X));
                }
            }

            if (start.X > end.X && start.Y > end.Y)
            {
                double an =
                    Math.Acos(Math.Sqrt(((start.X - rect.Right) * (start.X - rect.Right))) /
                              Math.Sqrt(((start.X - rect.Right) * (start.X - rect.Right)) +
                                        ((start.Y - rect.Bottom) * (start.Y - rect.Bottom))));
                if (start.X < rect.Right)
                {
                    an = an + 1;
                }

                if (start.Y < rect.Bottom)
                {
                    an = an - 1;
                }

                if (angle > an)
                {
                    cross.X = Math.Min(rect.Right, start.X);
                    cross.Y = end.Y + (((start.Y - end.Y) * rect.Width / 2) / (start.X - end.X));
                }
                else
                {
                    cross.X = end.X + (((start.X - end.X) * rect.Height / 2) / (start.Y - end.Y));
                    cross.Y = Math.Min(rect.Bottom, start.Y);
                }
            }

            if (start.X < end.X && start.Y < end.Y)
            {
                double an =
                    Math.Acos(Math.Sqrt(((start.X - rect.Left) * (start.X - rect.Left))) /
                              Math.Sqrt(((start.X - rect.Left) * (start.X - rect.Left)) +
                                        ((start.Y - rect.Top) * (start.Y - rect.Top))));

                if (start.X > rect.Left)
                {
                    an = an + 1;
                }

                if (start.Y > rect.Top)
                {
                    an = an - 1;
                }

                if (an >= angle)
                {
                    cross.X = end.X - (((end.X - start.X) * rect.Height / 2) / (end.Y - start.Y));
                    cross.Y = Math.Max(rect.Top, start.Y);
                }
                else
                {
                    cross.X = Math.Max(rect.Left, start.X);
                    cross.Y = end.Y - (((end.Y - start.Y) * rect.Width / 2) / (end.X - start.X));
                }
            }

            if (start.X > end.X && start.Y < end.Y)
            {
                double an =
                    Math.Acos(Math.Sqrt(((start.X - rect.Right) * (start.X - rect.Right))) /
                              Math.Sqrt(((start.X - rect.Right) * (start.X - rect.Right)) +
                                        ((start.Y - rect.Top) * (start.Y - rect.Top))));

                if (start.X < rect.Right)
                {
                    an = an + 1;
                }

                if (start.Y > rect.Top)
                {
                    an = an - 1;
                }

                if (an < angle)
                {
                    cross.X = Math.Min(rect.Right, start.X);
                    cross.Y = end.Y - (((end.Y - start.Y) * rect.Width / 2) / (start.X - end.X));
                }
                else
                {
                    cross.X = end.X + (((start.X - end.X) * rect.Height / 2) / (end.Y - start.Y));
                    cross.Y = Math.Max(rect.Top, start.Y);
                }
            }

            if (start.X == end.X)
            {
                if (start.Y < end.Y)
                {
                    cross.X = start.X;
                    cross.Y = rect.Top;
                }
                else
                {
                    cross.X = start.X;
                    cross.Y = rect.Bottom;
                }
            }

            if (start.Y == end.Y)
            {
                if (start.X < end.X)
                {
                    cross.X = rect.Left;
                    cross.Y = start.Y;
                }
                else
                {
                    cross.X = rect.Right;
                    cross.Y = end.Y;
                }
            }

            if (cross.Y > rect.Bottom)
            {
                cross.Y = rect.Bottom;
                cross.X = rect.Left + (rect.Width / 2);
            }

            if (cross.Y < rect.Top)
            {
                cross.Y = rect.Top;
                cross.X = rect.Left + (rect.Width / 2);
            }

            if (cross.X > rect.Right)
            {
                cross.X = rect.Right;
                cross.Y = rect.Bottom + (rect.Height / 2);
            }

            if (cross.X < rect.Left)
            {
                cross.X = rect.Left;
                cross.Y = rect.Bottom + (rect.Height / 2);
            }

            return cross;
        }

        /// <summary>
        /// Если 3 точки лежат на одной прямой, среднюю удаляем
        /// </summary>
        protected void ValidatePoints(List<Point> selectedPoints)
        {
            foreach (Point p in selectedPoints)
            {
                if (selectedPoints.IndexOf(p) != 0 && selectedPoints.IndexOf(p) != selectedPoints.Count - 1)
                {
                    Point last = selectedPoints[selectedPoints.IndexOf(p) - 1];
                    Point next = selectedPoints[selectedPoints.IndexOf(p) + 1];

                    GraphicsPath gr = new GraphicsPath();
                    gr.AddLine(last, next);
                    gr.Widen(grfxPen);

                    if (gr.IsVisible(p))
                    {
                        selectedPoints.Remove(p);
                        CountOfPoint--;
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Возвращает предпоследнюю точку у линии ассоциации
        /// </summary>
        protected Point GetPrePoint(Point p1, int count)
        {
            if (count < 2)
            {
                return Point.Empty;
            }

            try
            {
                Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);
                if (this.ChildDiagramEntity == null || this.ParentDiagramEntity == null)
                {
                    return Point.Empty;
                }

                Rectangle rect = new Rectangle(
                    this.ChildDiagramEntity.EntityRectangle.X,
                    this.ChildDiagramEntity.EntityRectangle.Y,
                     this.ChildDiagramEntity.EntityRectangle.Width,
                     this.ChildDiagramEntity.EntityRectangle.Height);
                rect.Inflate(new Size(5, 5));
                if (count == 2)
                {
                    return SecPointCrosssOptimal(this.ParentDiagramEntity.Coordinate(), this.ChildDiagramEntity.Coordinate(), rect);
                }
                else
                {
                    return SecPointCrosssOptimal(p1, this.ChildDiagramEntity.Coordinate(), rect);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// Измениние графического пути стрелки
        /// </summary>
        protected void ResizeGrPath(List<Point> point)
        {
            try
            {
                Point[] po = new Point[point.Count];

                point.CopyTo(po);

                GraphicsPath.Reset();
                GraphicsPath.AddLines(po);
                GraphicsPath.Widen(grfxPen);
            }
            catch
            {
            }
        }

        #endregion
    }
}
