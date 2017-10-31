using System.Drawing;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Класс для трансформации диаграммы (пока только масштабирование) 
    /// </summary>
    public class ScaleTransform
    {
        public ScaleTransform()
        {
        }

        #region Статические функции преобразования

        /// <summary>
        /// Трансформация точки для изменения размеров, обратная зависимость
        /// </summary>
        public static Point TransformPoint(Point p, int scalefactor)
        {
            if (scalefactor == 0)
            {
                return p;
            }

            return new Point((int)(p.X / ((float)scalefactor / 100)), (int)(p.Y / ((float)scalefactor / 100)));
        }

        public static PointF TransformPointF(PointF p, int scalefactor)
        {
            if (scalefactor == 0)
            {
                return p;
            }

            return new PointF(p.X / ((float)scalefactor / 100), p.Y / ((float)scalefactor / 100));
        }

        /// <summary>
        /// Простое преобразование с учётом масштаба
        /// </summary>
        public static Point SimpleTransformPoint(Point p, int scalefactor)
        {
            return new Point((int)(p.X * ((float)scalefactor / 100)), (int)(p.Y * ((float)scalefactor / 100)));
        }

        /// <summary>
        /// Трансформация прямоугольника
        /// </summary>
        public static Rectangle TransformRectangle(Rectangle rec, int scalefactor)
        {
            return new Rectangle(
                (int)(rec.X * ((float)scalefactor / 100)),
                (int)(rec.Y * ((float)scalefactor / 100)),
                (int)(rec.Width * ((float)scalefactor / 100)),
                (int)(rec.Height * ((float)scalefactor / 100)));
        }

        public static Rectangle ReverseTransformRectangle(Rectangle rec, int scalefactor)
        {
            return new Rectangle(
                (int)(rec.X / ((float)scalefactor / 100)),
                (int)(rec.Y / ((float)scalefactor / 100)),
                (int)(rec.Width / ((float)scalefactor / 100)),
                (int)(rec.Height / ((float)scalefactor / 100)));
        }

        /// <summary>
        /// Преобразование шрифта с учетом масштаба
        /// </summary>
        public static Font TransformFont(Font font, int scalefactor)
        {
            return new Font(font.Name, font.Size * ((float)scalefactor / 100), font.Style);
        }

        /// <summary>
        /// Простое преобразование целого цисла
        /// </summary>
        public static int TransformInt(int i, int scalefactor)
        {
            float f = (float)scalefactor / 100;
            int n = (int)(i * f);
            return n;
        }

        /// <summary>
        /// Обратное преобразование размера
        /// </summary>
        public static Size TransformSize(Size size, int scalefactor)
        {
            return new Size((int)(size.Width / ((float)scalefactor / 100)), (int)(size.Height / ((float)scalefactor / 100)));
        }

        #endregion Статические функции преобразования
    }
}
