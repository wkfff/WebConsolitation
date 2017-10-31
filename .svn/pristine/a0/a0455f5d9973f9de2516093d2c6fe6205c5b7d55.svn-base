using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Krista.FM.Client.DiagramEditor.Commands;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый класс для сущности на диаграмме
    /// </summary>
    public partial class UMLEntityBase : DiagramRectangleEntity
    {       
        #region Fields

        /// <summary>
        /// Атрибуты объекта
        /// </summary>
        private List<UMLAttribute> attributes = new List<UMLAttribute>();
        
        /// <summary>
        /// Видимость иконок у атрибутов
        /// </summary>
        private bool isAttrIconsVisible = true;
        
        /// <summary>
        /// Видимость обычных атрибутов
        /// </summary>
        private bool regularAttrVisible = true;
       
        /// <summary>
        /// Видимость служебных атрибутов
        /// </summary>
        private bool servisedAttrVisible;
        
        /// <summary>
        /// Видимость системных атрибутов
        /// </summary>
        private bool sysAttrVisible;
                
        /// <summary>
        /// Строка экспорта
        /// </summary>
        private string export = string.Empty;

        /// <summary>
        /// Использовать SQL-выражение
        /// </summary>
        private bool sqlExpression = true;

        #region Для сохранения в Xml

        /// <summary>
        /// Скрывать или нет атрибуты
        /// </summary>
        private bool isSuppressAttribute = false;

        #endregion Для сохранения в Xml
                        
        #endregion Fields

        #region Constructor

        public UMLEntityBase(string key, Guid id, int x, int y)
            : this(key, id, null, x, y, Color.LightYellow)
        {
        }

        public UMLEntityBase(string key, Guid id, AbstractDiagram diagram, int x, int y)
            : this(key, id, diagram, x, y, Color.LightYellow)
        {
        }

        public UMLEntityBase(string key, Guid id, AbstractDiagram diagram, int x, int y, Color color)
            : base(key, id, diagram, x, y, color)
        {
            sqlExpression = diagram is Krista.FM.Client.DiagramEditor.Diagrams.ClassDiagram;
            Initialization();

            RefreshAttrCollection();
                        
            this.regular.Click += new EventHandler(Attr_Click);
            this.servised.Click += new EventHandler(Attr_Click);
            this.sys.Click += new EventHandler(Attr_Click);

            this.visibleAttributes.DropDownItemClicked += new ToolStripItemClickedEventHandler(ContextMenu_ItemClicked);
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Видимость атрибутов
        /// </summary>
        public bool IsAttrVisible
        {
            get { return isAttrIconsVisible; }
            set { isAttrIconsVisible = value; }
        }

        public bool IsSuppressAttribute
        {
            get { return isSuppressAttribute; }
            set { isSuppressAttribute = value; }
        }
                
        public List<UMLAttribute> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        /// <summary>
        /// Использовать SQL-выражение
        /// </summary>
        public bool SqlExpression
        {
            get
            {
                return sqlExpression;
            }

            set
            {
                sqlExpression = value;
                ChangeNames(value);
            }
        }

        public string Export
        {
            get { return export; }
            set { export = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Операции со стереотипом
        /// </summary>
        public override void DisplayStereotype(bool offon)
        {
            if (this.StereotypeVisible != offon)
            {
                this.StereotypeVisible = offon;

                // Авторазмер - если измениласть видимость стереотипа
                this.AutoSizeRec();
            }
        }

        /// <summary>
        /// Показать SQL-выражение
        /// </summary>
        public override void DisplaySQLExpression(bool visible)
        {
            if (this.SqlExpression != visible)
            {
                this.SqlExpression = visible;
                this.AutoSizeRec();
            }
        }

        public override void OnDragOver(object sender, DragEventArgs e)
        {
            // перетаскивать можно только атрибуты
            foreach (Infragistics.Win.UltraWinTree.UltraTreeNode node in (Infragistics.Win.UltraWinTree.SelectedNodesCollection)e.Data.GetData(typeof(Infragistics.Win.UltraWinTree.SelectedNodesCollection)))
            {
                if (!(node.Tag is IDataAttribute))
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
            }

            // и только на табличные документы
            if (CommonObject is IEntity && ((IEntity)CommonObject).ClassType == ClassTypes.DocumentEntity)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        public override void RemoveEntity()
        {
            List<DiagramEntity> collection = new List<DiagramEntity>();

            Diagram.Entities.Remove(this);

            // если удаляем класс или пакет, удаляем ассоциации
            foreach (DiagramEntity association in Diagram.Entities)
            {
                if (association is UMLAssociationBase)
                {
                    if (((UMLAssociationBase)association).ParentDiagramEntity.ID == this.ID || ((UMLAssociationBase)association).ChildDiagramEntity.ID == this.ID)
                    {
                        collection.Add(association);
                    }
                }
            }

            foreach (DiagramEntity en in collection)
            {
                en.RemoveEntity();
            }

            base.RemoveEntity();
        }

        public override void RestoreEntity()
        {
            ICommonDBObject commonDBObject = Diagram.Site.SchemeEditor.GetObjectByPathName(Diagram.Site.SchemeEditor.GetNameByServerName(this.Key)) as ICommonDBObject;
            if (commonDBObject is IEntity)
            {
                Diagram.Entities.Add(this);

                ((CommandDragDrop)Diagram.Site.CmdDragDrop).AssociationRestore(commonDBObject, this);
            }

            base.RestoreEntity();
        }

        public override void InitializeDefault()
        {
            IEntity entity = this.Diagram.Site.SchemeEditor.Scheme.GetObjectByKey(Key) as IEntity;

            if (entity != null)
            {
                this.FillColor = GetColorByClassType(entity.ClassType);
            }

            this.IsShadow = false;

            base.InitializeDefault();

            this.AutoSizeRec();
        }

        /// <summary>
        /// Минимальные размеры, прикоторых видно всю облась класса
        /// </summary>
        public override void AutoSizeRec()
        {
            Graphics g = Diagram.Site.CreateGraphics();
            try
            {
                int s = 0;
                int lenght = 0;
                if (!isSuppressAttribute)
                {
                    foreach (UMLAttribute a in this.Attributes)
                    {
                        if (a.IsAttrVisible)
                        {
                            s += (int)this.Font.GetHeight() + 5;
                            lenght = Math.Max(lenght, (int)g.MeasureString(a.Name, Font).Width);
                        }
                    }
                }

                SetMinSize(g, true);

                MinWidht = Math.Max(MinWidht, lenght + 20 + IndentRight);
                Rectangle rectangle = EntityRectangle;
                rectangle.Width = MinWidht;

                if (isSuppressAttribute)
                {
                    rectangle.Height = MinHeight;
                }
                else
                {
                    rectangle.Height = GetHeight(MinWidht) + s + 5;
                }

                EntityRectangle = rectangle;

                Diagram.IsChanged = true;
                Diagram.Site.Invalidate();
            }
            finally
            {
                g.Dispose();
            }
        }

        public override void CreateHeaderTextBox(Point point)
        {
            Size scrollOffset = ScaleTransform.TransformSize(
                new Size(Diagram.Site.AutoScrollPosition),
                Diagram.Site.ZoomFactor);

            base.CreateHeaderTextBox(point);

            // разделяем заголовок и атрибуты
            HeaderText.BackColor = FillColor;

            Rectangle rect = new Rectangle();

            // Атрибуты могут быть скрыты
            if (IsSuppressAttribute)
            {
                Number = -1;
            }
            else
            {
                Number = ReturnNumberOfAttribute(point, ref rect);
            }

            if (Number != -1)
            {
                HeaderText.Location = new Point(rect.X + scrollOffset.Width, rect.Y + scrollOffset.Height);
                HeaderText.Width = rect.Width;
                HeaderText.Height = rect.Height;

                // находим только видимый атрибут под номером number
                int i = 0;
                foreach (UMLAttribute attr in this.attributes)
                {
                    if (attr.IsAttrVisible)
                    {
                        if (i == Number)
                        {
                            HeaderText.Text = attr.Name;
                            break;
                        }

                        i++;
                    }
                }
            }
            else
            {
                // Чтоб установить MinHeight
                int size = GetHeight(EntityRectangle.Width);
                HeaderText.Text = Text;

                HeaderText.Location =
                    ScaleTransform.SimpleTransformPoint(
                        new Point(
                            this.X + (IndentTextBox / 2) + scrollOffset.Width,
                            this.Y + (IndentTextBox / 2) + scrollOffset.Height),
                        Diagram.Site.ZoomFactor);
                HeaderText.Width = ScaleTransform.TransformInt(EntityRectangle.Width - IndentTextBox, Diagram.Site.ZoomFactor);
                if (this.IsSuppressAttribute)
                {
                    HeaderText.Height = ScaleTransform.TransformInt(EntityRectangle.Height - 5, Diagram.Site.ZoomFactor);
                    rect =
                        ScaleTransform.TransformRectangle(
                            new Rectangle(EntityRectangle.X, EntityRectangle.Y, EntityRectangle.Width, EntityRectangle.Height), Diagram.Site.ZoomFactor);
                }
                else
                {
                    HeaderText.Height = ScaleTransform.TransformInt(size + IndentTextBox, Diagram.Site.ZoomFactor);
                    rect =
                        ScaleTransform.TransformRectangle(
                            new Rectangle(EntityRectangle.X, EntityRectangle.Y, EntityRectangle.Width, HeaderText.Height), Diagram.Site.ZoomFactor);
                }

                // Должны дважды щёлкнуть по верхнему прямоугольнику
                if (!rect.Contains(point))
                {
                    DisposeHeaderTextBox();
                    return;
                }
            }

            HeaderText.Enabled = true;
            HeaderText.Visible = true;

            if (HeaderText.CanFocus)
            {
                HeaderText.Focus();
            }
        }                                                          

        /// <summary>
        /// Overrides the abstract paint method
        /// </summary>
        /// <param name="g">a graphics object onto which to paint</param>
        public override void Draw(System.Drawing.Graphics g, Size scrollOffset)
        {          
            // задаем параметры карандаша
            Pen.Color = LineColor;
            Pen.Width = LineWidth;

            // Получем области для отрисовки
            Rectangle top = new Rectangle();
            Rectangle bottom = new Rectangle();

            int height = GetHeight(EntityRectangle.Width - (IndendLeft + IndentRight));
            Rectangle rectangle = EntityRectangle;

            if (rectangle.Height < MinHeight)
            {
                rectangle.Height = MinHeight;
            }

            if (isSuppressAttribute)
            {
                top = rectangle;
            }
            else
            {
                top = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, height);
                bottom = new Rectangle(
                    rectangle.X, 
                    rectangle.Y + top.Height, 
                    rectangle.Width, 
                    rectangle.Height - top.Height);
            }
           
            // Скроллигг
            top.Offset(scrollOffset.Width, scrollOffset.Height);
            bottom.Offset(scrollOffset.Width, scrollOffset.Height);

            EntityRectangle = rectangle;

            // Области отрисовки
            DrawHeaderRect(top, g);
            DrawAttrRect(bottom, g);
          
            // В базовом методе рисуем тень
            base.Draw(g, scrollOffset);
        }

        /// <summary>
        /// Обновление коллекции атрибутов
        /// </summary>
        public void RefreshAttrCollection()
        {
            List<UMLAttribute> attr = new List<UMLAttribute>();

            if (Diagram.Site != null)
            {
                IEntity entity = CommonObject as IEntity;

                if (entity != null)
                {
                    foreach (IDataAttribute item in entity.Attributes.Values)
                    {
                        UMLAttribute a = new UMLAttribute(item, this, Diagram);
                        if (CheckAttrVisible(a))
                        {
                            attr.Add(a);
                        }
                    }
                }
            }

            if (!EqualsCollection(attr, attributes))
            {
                attributes = attr;
                AutoSizeRec();
            }
        }

        /// <summary>
        /// обновление имени класса
        /// </summary>
        public override void RefreshKey()
        {
            base.RefreshKey();

            if (CommonObject != null)
            {
                string s = GetServerObjectCaption(CommonObject);
                if (Text != s)
                {
                    Text = s;
                }
            }
        }

        /// <summary>
        /// Определяет цвет класса по его типу
        /// </summary>
        /// <param name="classType">Тип класса</param>
        /// <returns>Цвет класса</returns>
        internal static Color GetColorByClassType(ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.clsDataClassifier:
                    return Color.LightYellow;
                case ClassTypes.clsBridgeClassifier:
                    // Color.PaleGreen;
                    return Color.FromArgb(235, 253, 193); 
                case ClassTypes.clsFixedClassifier:
                    // Color.PaleVioletRed;
                    return Color.FromArgb(252, 204, 188); 
                case ClassTypes.clsFactData:
                    // Color.PaleTurquoise;
                    return Color.FromArgb(206, 241, 253); 
                case ClassTypes.Table:
                    return Color.FromArgb(240, 240, 240);
                case ClassTypes.DocumentEntity:
                    return Color.FromArgb(206, 206, 206);
                default:
                    return Color.FromArgb(235, 253, 193);
            }
        }

        protected override void ChangeNames(bool value)
        {
            base.ChangeNames(value);

            foreach (UMLAttribute attribute in attributes)
            {
                attribute.ChangeName(value);
            }
        }

        /// <summary>
        /// Метрика заголовка
        /// </summary>
        /// <param name="g">Объект типа Graphics</param>
        /// <param name="auto">признак форматирования</param>
        protected override void SetMinSize(Graphics g, bool auto)
        {
            Font exportFont = new Font(Font.Name, Font.Size - 1.0f, Font.Style);
            try
            {
                MinHeight = 0; 
                MinWidht = 0;

                MinWidht = (int)g.MeasureString(MaxWord(), this.Font).Width;

                // с поправкой на стереотип
                if (this.StereotypeVisible)
                {
                    MinWidht = Math.Max(MinWidht, (int)g.MeasureString("<<" + Stereotype + ">>", this.Font).Width);
                }

                if (!String.IsNullOrEmpty(export))
                {
                    MinWidht = Math.Max(MinWidht, (int)g.MeasureString(export, exportFont).Width + 5);
                }

                MinWidht += 5;

                // Получаем мин высоту, в зависимости от ширины
                MinHeight = GetHeight(!auto ? Math.Max(MinWidht, EntityRectangle.Width - (IndendLeft + IndentRight)) : MinWidht);

                // запас - отступ
                MinWidht += IndendLeft + IndentRight;

                // + eсли не скрываем атрибуты
                if (!isSuppressAttribute)
                {
                    MinHeight += 5;
                }

                // проверка ны выход
                if (EntityRectangle.Width < MinWidht)
                {
                    Rectangle rectangle = EntityRectangle;
                    rectangle.Width = MinWidht;
                    EntityRectangle = rectangle;
                }

                if (EntityRectangle.Height < MinHeight)
                {
                    Rectangle rectangle = EntityRectangle;
                    rectangle.Height = MinHeight;
                    EntityRectangle = rectangle;
                }
            }
            finally
            {
                exportFont.Dispose();
            }
        }

        protected override string GetServerObjectCaption(ICommonObject obj)
        {
            if (obj is IDocumentEntity)
            {
                return obj.Caption;
            }
            else
            {
                return sqlExpression
                        ? String.Format("{0}.{1} ({2})", ((IEntity)obj).SemanticCaption, obj.Caption, obj.FullName)
                        : String.Format("{0}.{1}", ((IEntity)obj).SemanticCaption, obj.Caption);
            }
        }

        /// <summary>
        /// Видимость атрибутв
        /// </summary>
        private static bool AttrCheckedUnchecked(ref ToolStripMenuItem item)
        {
            bool isChecked;

            if (item.Checked)
            {
                item.Checked = false;
                isChecked = false;
            }
            else
            {
                item.Checked = true;
                isChecked = true;
            }

            return isChecked;
        }

        private void Initialization()
        {
            IEntity entity = this.Diagram.Site.SchemeEditor.Scheme.GetObjectByKey(Key) as IEntity;

            if (entity != null)
            {
                this.Stereotype = GetStereotypeByClass(entity);
            }
        }

        private string GetStereotypeByClass(IEntity entity)
        {
            switch (entity.ClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    return "Сопоставимый";
                case ClassTypes.clsDataClassifier:
                    return "КлассификаторДанных";
                case ClassTypes.clsFixedClassifier:
                    return "Фиксированный";
                case ClassTypes.clsFactData:
                    return "Факты";
                case ClassTypes.Table:
                    return "Таблица";
                case ClassTypes.DocumentEntity:
                    return "Табличный документ";
                default:
                    return string.Empty;
            }
        }

        private void DrawAttrRect(Rectangle bottom, Graphics g)
        {
            SolidBrush brush = new SolidBrush(FillColor);
            try
            {
                if (isSuppressAttribute)
                {
                    return;
                }

                // поле аттрибутов
                g.FillRectangle(brush, bottom);
                g.DrawRectangle(Pen, bottom);

                // attributes
                int y = bottom.Top + 3;
                foreach (UMLAttribute item in attributes)
                {
                    if (item.IsAttrVisible)
                    {
                        item.DrawAttribute(g, y, bottom, this, IsAttrVisible, item, attributes.IndexOf(item));

                        y += (int)Font.GetHeight() + 5;
                    }
                }
            }
            finally
            {
                brush.Dispose();
            }
        }

        private void DrawHeaderRect(Rectangle top, Graphics g)
        {
            using (SolidBrush fillBrush = new SolidBrush(FillColor),
                textBrush = new SolidBrush(TextColor))
            {
                using (System.Drawing.Font exportFont = new Font(Font.Name, Font.Size - 1.0f, Font.Style))
                {
                    // + к высоте шрифта
                    const int FontHeightPlus = 4;

                    // заголовок
                    g.FillRectangle(fillBrush, top);
                    g.DrawRectangle(Pen, top);

                    top = new Rectangle(
                        top.X + IndendLeft,
                        top.Y + IndentTop,
                        top.Width - (IndentRight + IndendLeft),
                        top.Height - (IndentBottom + IndentTop));

                    Rectangle cut = new Rectangle(top.X, top.Y, top.Width, top.Height);
                    if (StereotypeVisible)
                    {
#if DRAW_INVIS_REGIONS
                g.DrawRectangle(Pens.Red, new Rectangle(top.X, top.Y, top.Width, this.Font.Height + fontHeightPlus));
#endif
                        g.DrawString(
                            "<<" + Stereotype + ">>",
                            Font,
                            textBrush,
                            new Rectangle(top.X, top.Y, top.Width, this.Font.Height + FontHeightPlus),
                            Format);
                        top = new Rectangle(
                            top.X,
                            top.Y + FontHeightPlus,
                            top.Width,
                            top.Height - FontHeightPlus);
                        cut = new Rectangle(
                            cut.X,
                            cut.Y + this.Font.Height + FontHeightPlus + IndentBetween,
                            cut.Width,
                            cut.Height - this.Font.Height - FontHeightPlus - IndentBetween);
                    }

                    if (!String.IsNullOrEmpty(export))
                    {
#if DRAW_INVIS_REGIONS
                g.DrawRectangle(Pens.Green, new Rectangle(top.Left, top.Bottom - this.Font.Height - fontHeightPlus, top.Width, this.Font.Height + fontHeightPlus));
#endif
                        g.DrawString(
                            export,
                            exportFont,
                            textBrush,
                            new Rectangle(top.Left, top.Bottom - this.Font.Height - FontHeightPlus, top.Width, this.Font.Height + FontHeightPlus),
                            Format);
                        top = new Rectangle(
                            top.X,
                            top.Y,
                            top.Width,
                            top.Height - FontHeightPlus);
                        cut = new Rectangle(
                            cut.X,
                            cut.Y,
                            cut.Width,
                            cut.Height - this.Font.Height - FontHeightPlus - IndentBetween);
                    }

#if DRAW_INVIS_REGIONS
            g.DrawRectangle(Pens.Blue, cut);
#endif
                    g.DrawString(Text, Font, textBrush, cut, Format);
                }
            }
        }

        /// <summary>
        /// Видимость атрибута
        /// </summary>
        private bool CheckAttrVisible(UMLAttribute a)
        {
            switch (a.Kind)
            {
                case DataAttributeKindTypes.Regular:
                    return a.IsAttrVisible = this.regularAttrVisible ? true : false;

                case DataAttributeKindTypes.Serviced:
                    return a.IsAttrVisible = this.servisedAttrVisible ? true : false;

                case DataAttributeKindTypes.Sys:
                    return a.IsAttrVisible = this.sysAttrVisible ? true : false;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Получаем минимальную высоту, в зависимости от ширины 
        /// </summary>
        private int GetHeight(int widht)
        {
            Graphics g = Diagram.Site.CreateGraphics();
            try
            {
                int height = 0;

                if (this.StereotypeVisible)
                {
                    height += this.Font.Height + 5 + IndentBetween;
                }

                if (!String.IsNullOrEmpty(export))
                {
                    height += this.Font.Height + IndentBetween;
                }

                height += (int)g.MeasureString(Text, Font, widht, Format).Height + 5;

                // Верхний + нижний отступы
                height += IndentTop + IndentBottom;

                return height;
            }
            finally
            {
                g.Dispose();
            }
        }

        #endregion

        #region Events

        private void Attr_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            if (menuItem != null)
            {
                bool isVis;
                switch (menuItem.Name)
                {
                    case "Regular":
                        isVis = AttrCheckedUnchecked(ref regular);
                        foreach (UMLAttribute attr in this.attributes)
                        {
                            if (attr.Kind == DataAttributeKindTypes.Regular)
                            {
                                attr.IsAttrVisible = isVis;
                            }
                        }

                        this.regularAttrVisible = isVis;

                        break;
                    case "Servised":
                        isVis = AttrCheckedUnchecked(ref servised);
                        foreach (UMLAttribute attr in this.attributes)
                        {
                            if (attr.Kind == DataAttributeKindTypes.Serviced)
                            {
                                attr.IsAttrVisible = isVis;
                            }
                        }

                        this.servisedAttrVisible = isVis;

                        break;
                    case "Sys":
                        isVis = AttrCheckedUnchecked(ref sys);
                        foreach (UMLAttribute attr in this.attributes)
                        {
                            if (attr.Kind == DataAttributeKindTypes.Sys)
                            {
                                attr.IsAttrVisible = isVis;
                            }
                        }

                        this.sysAttrVisible = isVis;

                        break;
                }
            }

            RefreshAttrCollection();
        }

        #endregion Events

        #region Helper

        private bool EqualsCollection(List<UMLAttribute> l1, List<UMLAttribute> l2)
        {
            if (l1.Count != l2.Count)
            {
                return false;
            }

            return !l1.Where((t, i) => !t.Equals(l2[i])).Any();
        }

        /// <summary>
        /// Номер выделяемого атрибута
        /// </summary>
        private int ReturnNumberOfAttribute(Point point, ref Rectangle rect)
        {
            if (Diagram.Site.ZoomFactor != 100)
            {
                return -1;
            }

            // стартовая высота
            int y = this.MinHeight + this.EntityRectangle.Y;

            // стартовая ширина
            int x = this.EntityRectangle.X;

            for (int i = 0; i < this.attributes.Count; i++)
            {
                rect =
                    ScaleTransform.TransformRectangle(
                        new Rectangle(x + 20, y, this.EntityRectangle.Width - 25, (int)Font.GetHeight() + 5),
                        Diagram.Site.ZoomFactor);
                y += (int)Font.GetHeight() + 5;

                if (rect.Contains(point))
                {
                    return i;
                }
            }

            return -1;
        }
        
        #endregion
    }
}
