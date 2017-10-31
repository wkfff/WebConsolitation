using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

using Microsoft.Win32;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class HelperCommand
    {
        private AbstractDiagram diagram;

        public HelperCommand(AbstractDiagram diagram)
        {
            this.diagram = diagram;
        }

        /// <summary>
        /// открытое свойста для получения и модификации коллекции цветов
        /// </summary>
        public int[] FavoriteColors
        {
            get
            {
                // Чтение из реестра
                int[] myColors = new int[16];

                RegistryKey rk = Krista.FM.Common.RegistryUtils.Utils.BuildRegistryKey(Registry.CurrentUser, diagram.Site.GetType().FullName);

                if (rk.ValueCount == 0)
                {
                    return null;
                }

                for (int i = 0; i < 16; i++)
                {
                    myColors[i] = (int)rk.GetValue(i.ToString());
                }

                return myColors;
            }

            set
            {
                // Запись в реестр
                if (value.Length == 0)
                {
                    return;
                }

                RegistryKey rk = Krista.FM.Common.RegistryUtils.Utils.BuildRegistryKey(Registry.CurrentUser, diagram.Site.GetType().FullName);

                for (int i = 0; i < 16; i++)
                {
                    rk.SetValue(i.ToString(), value[i]);
                }
            }
        }

        #region HelperFunc

        public Color ColorDialog()
        {
            ColorDialog myDialog = new ColorDialog();

            // Keeps the user from selecting a custom color.
            myDialog.AllowFullOpen = true;
            myDialog.ShowHelp = true;

            // обращаемся к реестру
            myDialog.CustomColors = diagram.Site.CmdHelper.FavoriteColors;

            // Update the text box color if the user clicks OK 
            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                diagram.Site.CmdHelper.FavoriteColors = myDialog.CustomColors;
                return myDialog.Color;
            }

            // Если нажали отмена
            return Color.Empty;
        }

        /// <summary>
        /// Выделяемая коллекция
        /// </summary>
        public List<DiagramEntity> SelectFromRect(Rectangle rec, Size scrollOffset)
        {
            // Очищаем список выделенных элементов
            UnSelectAll();

            List<DiagramEntity> e = new List<DiagramEntity>();

            if (rec.Width < 0)
            {
                rec.Width = -rec.Width;
                rec.X -= rec.Width;
            }

            if (rec.Height < 0)
            {
                rec.Height = -rec.Height;
                rec.Y -= rec.Height;
            }

            rec.X -= scrollOffset.Width;
            rec.Y -= scrollOffset.Height;
            foreach (DiagramEntity entity in diagram.Entities)
            {
                if (entity.IntersectWith(rec))
                {
                    e.Add(entity);
                    entity.IsSelected = true;
                }
            }

            return e;
        }

        /// <summary>
        /// Выделенная колекция (1 элемент)
        /// </summary>
        public List<DiagramEntity> SelectedCollection(Point point)
        {
            List<DiagramEntity> mouseHitCollection = new List<DiagramEntity>();

            for (int i = diagram.Entities.Count - 1; i > -1; i--)
            {
                if (diagram.Entities[i].Hit(point))
                {
                    mouseHitCollection.Add(diagram.Entities[i]);
                    return mouseHitCollection;
                }
            }

            return mouseHitCollection;
        }

        /// <summary>
        /// Размер диаграммы в зависимости от масштаба
        /// </summary>
        public Size GetDiagramSize(int scaleFactor)
        {
            Size diagramSize = new Size(0, 0);

            if (diagram != null)
            {
                foreach (DiagramEntity entity in diagram.Entities)
                {
                    diagramSize.Width = Math.Max(ScaleTransform.TransformInt(entity.EntityRectangle.Right, scaleFactor), diagramSize.Width);
                    diagramSize.Height = Math.Max(ScaleTransform.TransformInt(entity.EntityRectangle.Bottom, scaleFactor), diagramSize.Height);
                }
            }

            return diagramSize;
        }

        /// <summary>
        /// Завершение режима создания новой ассоциации
        /// </summary>
        public void FinalizeCreateAssociation()
        {
            diagram.Site.CreateAssociation = false;
            diagram.Site.CreateBridgeAssociation = false;
            diagram.Site.CreateBridge2BridgeAssociation = false;
            diagram.Site.CreateMasterDetailAssociation = false;
            diagram.Site.SelectedShape = null;
            diagram.Site.Points.Clear();

            diagram.Site.Invalidate();
        }

        /// <summary>
        /// В момент рисования ассоциации 
        /// </summary>
        public void DrawAssociate(Graphics g, Size scrollOffset)
        {
            if (diagram.Site.Points.Count > 1)
            {
                Point[] lines = new Point[diagram.Site.Points.Count];
                int i = 0;
                foreach (Point point in diagram.Site.Points)
                {
                    point.Offset(scrollOffset.Width, scrollOffset.Height);
                    lines.SetValue(point, i);
                    i++;
                }

                g.DrawLines(Pens.Black, lines);
            }
        }

        /// <summary>
        /// Select all
        /// </summary>
        public void SelectAll()
        {
            foreach (DiagramEntity entity in diagram.Entities)
            {
                entity.IsSelected = true;
                diagram.Site.SelectedEntities.Add(entity);
            }

            diagram.Site.Invalidate();
        }
    
        // Полная отмена выделения
        public void UnSelectAll()
        {
            foreach (DiagramEntity entity in diagram.Entities)
            {
                entity.IsSelected = false;

                // Исчезает контрол textBox
                if (entity is DiagramRectangleEntity)
                {
                    ((DiagramRectangleEntity)entity).DisposeHeaderTextBox();
                }
            }

            diagram.Site.SelectedEntities.Clear();
            diagram.Site.Invalidate();
        }

        /// <summary>
        /// Свойства объекта
        /// </summary>
        public void GetPropertiesObjetct(string p)
        {
            ICommonObject obj = diagram.Site.SchemeEditor.Scheme.GetObjectByKey(p);

            diagram.Site.SchemeEditor.PropertyGrid.SelectedObject = obj;
        }       

        public void ScaleUpdate(int scaleFactor)
        {
            if (scaleFactor == -1)
            {
                diagram.Site.ZoomFactor = 100;
                CutLeftTopArea();
                Size s = GetDiagramSize(diagram.Site.ZoomFactor);
                diagram.Site.ZoomFactor =
                    (int)(Math.Min(
                            4f,
                            Math.Min(diagram.Site.Width / (s.Width + 10f), diagram.Site.Height / (s.Height + 10f))) * 100);

                if (diagram.Site.ScaleControl != null)
                {
                    diagram.Site.ScaleControl.ScaleFactor = diagram.Site.ZoomFactor;
                }
            }
            else
            {
                diagram.Site.ZoomFactor = scaleFactor;
            }

            diagram.Site.Invalidate();
        }

        /// <summary>
        /// Проверка на выход точке за границы диаграммы
        /// </summary>
        public Point CheckPoint(Point point)
        {
            return new Point(Math.Max(point.X, 0), Math.Max(point.Y, 0));
        }

        /// <summary>
        /// Проверка на выход точки за границы видимости
        /// </summary>
        public Point ValidatePoint(Point point)
        {
            Size scrollOffset = new Size(diagram.Site.AutoScrollPosition);

            if (point.X < diagram.Site.ClientRectangle.X - scrollOffset.Width)
            {
                point.X = diagram.Site.ClientRectangle.X - scrollOffset.Width;
            }

            if (point.Y < diagram.Site.ClientRectangle.Y - scrollOffset.Height)
            {
                point.Y = diagram.Site.ClientRectangle.Y - scrollOffset.Height;
            }

            if (point.X > diagram.Site.ClientRectangle.Right - scrollOffset.Width)
            {
                point.X = diagram.Site.ClientRectangle.Right - scrollOffset.Width;
            }

            if (point.Y > diagram.Site.ClientRectangle.Bottom - scrollOffset.Height)
            {
                point.Y = diagram.Site.ClientRectangle.Bottom - scrollOffset.Height;
            }

            return point;
        }
   
        /// <summary>
        /// Определение размеров перетаскиваемого прямоугольника
        /// </summary>
        /// <param name="list">Коллекция активных элементов</param>
        /// <returns>Искомый прямоугольник</returns>
        public Rectangle MovingRec(List<DiagramEntity> list, Size scrollOffset)
        {
            int minX = 10000, minY = 10000, maxX = 0, maxY = 0;
            foreach (DiagramEntity entity in list)
            {
                if (minX > entity.EntityRectangle.X)
                {
                    minX = entity.EntityRectangle.X;
                }

                if (minY > entity.EntityRectangle.Y)
                {
                    minY = entity.EntityRectangle.Y;
                }

                if (maxX < entity.EntityRectangle.X + entity.EntityRectangle.Width)
                {
                    maxX = entity.EntityRectangle.X + entity.EntityRectangle.Width;
                }

                if (maxY < entity.EntityRectangle.Y + entity.EntityRectangle.Height)
                {
                    maxY = entity.EntityRectangle.Y + entity.EntityRectangle.Height;
                }
            }

            return ScaleTransform.TransformRectangle(
                new Rectangle(minX, minY, maxX - minX, maxY - minY),
                diagram.Site.ZoomFactor);
        }

        /// <summary>
        /// Обрезаем лево-верхнюю область
        /// </summary>
        public void CutLeftTopArea()
        {
            Size minLocation = new Size(10000, 10000);

            foreach (DiagramEntity entity in diagram.Entities)
            {
                minLocation.Width = Math.Min(entity.EntityRectangle.X, minLocation.Width);
                minLocation.Height = Math.Min(entity.EntityRectangle.Y, minLocation.Height);
            }

            foreach (DiagramEntity entity in diagram.Entities)
            {
                entity.CutRect(minLocation);
            }
        }

        /// <summary>
        /// Get metafile
        /// </summary>
        /// <returns>Out Metafile</returns>
        public Metafile GetMetafile(Size scrollOffset, float scaleFactor)
        {
            Graphics g = diagram.Site.CreateGraphics();

            // loop over the shapes and draw
            IntPtr ptr = g.GetHdc();

            Metafile mf = new Metafile(new MemoryStream(), ptr);

            g.ReleaseHdc(ptr);
            g.Dispose();

            g = Graphics.FromImage(mf);

            // масштаб
            g.ScaleTransform(scaleFactor, scaleFactor);

            foreach (DiagramEntity entity in diagram.Entities)
            {
                entity.Draw(g, scrollOffset);
            }

            g.Dispose();

            return mf;
        }
        
        #endregion 
    }
}
