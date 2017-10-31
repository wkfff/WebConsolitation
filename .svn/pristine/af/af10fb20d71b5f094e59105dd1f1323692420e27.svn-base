using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Атрибут класса
    /// </summary>
    public class UMLAttribute
    {
        #region Const

        /// <summary>
        /// Отступ для иконки
        /// </summary>
        public const int IndentForIcon = 20;

        private IDataAttribute controlObject;

        #endregion

        #region Fields

        private StringFormat format = new StringFormat(StringFormatFlags.LineLimit);

        /// <summary>
        /// Идентификатор атрибута
        /// </summary>
        private string name;

        /// <summary>
        /// Тип атрибута
        /// </summary>
        private DataAttributeKindTypes kind;

        /// <summary>
        /// Класс, которому принадлежит атрибут
        /// </summary>
        private UMLEntityBase entity;

        /// <summary>
        /// Видимость атрибута
        /// </summary>
        private bool visible;

        private Bitmap icon;

        #endregion Fields

        #region Constructor
        public UMLAttribute(IDataAttribute controlObject, UMLEntityBase entity, AbstractDiagram diagram)
        {
            this.entity = entity;
            this.controlObject = controlObject;

            ChangeName(entity.SqlExpression);
            
            this.kind = controlObject.Kind;

            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;

            icon = DefineIconByDataAttributeKindTypes(this.Kind);
            icon.MakeTransparent(Color.FromArgb(254, 0, 254));
        }
        #endregion Constructor

        #region Properties

        /// <summary>
        /// Возвращаем идентификатор атрибута
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsAttrVisible
        {
            get { return visible; }
            set { visible = value; }
        }

        /// <summary>
        /// Возвращаем тип атрибута
        /// </summary>
        public DataAttributeKindTypes Kind
        {
            get { return kind; }
        }

        public IDataAttribute ControlObject
        {
            get { return controlObject; }
            set { controlObject = value; }
        }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            // Уже можно свободно приводить типы
            UMLAttribute a = (UMLAttribute)obj;

            if (this.Name != a.Name)
            {
                return false;
            }

            if (this.Kind != a.Kind)
            {
                return false;
            }

            if (this.IsAttrVisible != a.IsAttrVisible)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void ChangeName(bool value)
        {
            name = value ? controlObject.Caption + " (" + controlObject.SQLDefinition + ")" : controlObject.Caption;

            if (controlObject is IDocumentEntityAttribute)
            {
                string fromObjectCaption;
                IDocumentEntityAttribute da = (IDocumentEntityAttribute)controlObject;
                ICommonDBObject cdbo = entity.Diagram.Site.SchemeEditor.Scheme.RootPackage.FindEntityByName(da.SourceEntityKey);
                if (cdbo != null)
                {
                    fromObjectCaption = ((IEntity)cdbo).FullCaption;
                }
                else
                {
                    fromObjectCaption = da.SourceEntityKey;
                }

                name = String.Format("{0} ({1})", da.Caption, fromObjectCaption);
            }
        }

        /// <summary>
        /// Отрисовка атрибута
        /// </summary>
        public void DrawAttribute(Graphics g, int y, Rectangle bottom, DiagramEntity association, bool isVisible, UMLAttribute attr, int numberOfAttribute)
        {
            SolidBrush fillBrush = new SolidBrush(entity.TextColor);
            try
            {
                if (y + association.Font.Height > bottom.Bottom)
                {
                    return;
                }
                else
                {
                    if (isVisible)
                    {
                        g.DrawImage(icon, bottom.Left + 2, y);

                        if (numberOfAttribute != entity.Attributes.Count - 1)
                        {
                            if (y + (2 * association.Font.Height) + 4 > bottom.Bottom)
                            {
                                g.DrawString(
                                    "...",
                                    entity.Font,
                                    fillBrush,
                                    new Rectangle(bottom.Left + IndentForIcon, y, bottom.Width - IndentForIcon, association.Font.Height + 2),
                                    format);
                            }
                            else
                            {
#if DRAW_INVIS_REGIONS
                        g.DrawRectangle(Pens.DarkGoldenrod, new Rectangle(bottom.Left + indentForIcon, y, bottom.Width - indentForIcon - entity.IndentRight, association.Font.Height + 2));
#endif
                                g.DrawString(
                                    attr.Name,
                                    entity.Font,
                                    fillBrush,
                                    new Rectangle(bottom.Left + IndentForIcon, y, bottom.Width - IndentForIcon - entity.IndentRight, association.Font.Height + 2),
                                    format);
                            }
                        }
                        else
                        {
#if DRAW_INVIS_REGIONS
                    g.DrawRectangle(Pens.DarkGoldenrod, new Rectangle(bottom.Left + indentForIcon, y, bottom.Width - indentForIcon - entity.IndentRight, association.Font.Height + 2));
#endif
                            g.DrawString(
                                attr.Name,
                                entity.Font,
                                fillBrush,
                                new Rectangle(bottom.Left + IndentForIcon, y, bottom.Width - IndentForIcon - entity.IndentRight, association.Font.Height + 2),
                                format);
                        }
                    }
                }
            }
            finally
            {
                fillBrush.Dispose();
            }
        }

        /// <summary>
        /// Преобразовываем иконку в битмап
        /// </summary>
        private Bitmap DefineIconByDataAttributeKindTypes(DataAttributeKindTypes kind)
        {
            switch (kind)
            {
                case DataAttributeKindTypes.Regular:
                    return Resource.privateAttribute;
                case DataAttributeKindTypes.Serviced:
                    return Resource.PRIVATEATTRIBUTE2;
                case DataAttributeKindTypes.Sys:
                    return Resource.PRIVATEATTRIBUTESYS;
                default:
                    throw new Exception("Обработчик не реализован.");
            }
        }

        #endregion
    }
}
