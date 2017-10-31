using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor.Shapes.Factory;
using Krista.FM.Common.Xml;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Абстрактная диаграмма
    /// </summary>
    public abstract class AbstractDiagram : IDisposable
    {
        #region Константы

        public const string SaveContextMenuItemKey = "/ContextMenu/Сохранить";
        public const string UndoContextMenuItemKey = "/ContextMenu/Отменить";
        public const string RedoContextMenuItemKey = "/ContextMenu/Вернуть";
        public const string SaveMetafileContextMenuItemKey = "/ContextMenu/Сохранить как метафайл";

        #endregion Константы

        #region Поля

        /// <summary>
        /// Редактор диаграм
        /// </summary>
        private DiargamEditor site;

        /// <summary>
        /// Коллекция элементов
        /// </summary>
        private List<DiagramEntity> entities = new List<DiagramEntity>();

        /// <summary>
        /// Информация о версии
        /// </summary>
        private VersionInfo version;

        /// <summary>
        /// Документ на стороне сервера в котором хранится диаграмма
        /// </summary>
        private IDocument document;

        /// <summary>
        /// Была ли изменена диаграмма
        /// </summary>
        private bool isChanged;

        /// <summary>
        /// Коллекция доступных команд для диаграммы.
        /// </summary>
        private Dictionary<string, BaseCommand> diagramСommands;

        #endregion Поля

        #region Конструктор

        public AbstractDiagram(DiargamEditor site, IDocument document)
            : this(site)
        {
            this.document = document;

            Load();
            isChanged = false;
        }

        public AbstractDiagram(DiargamEditor site)
        {
            this.site = site;
            version = new VersionInfo();
            diagramСommands = new Dictionary<string, BaseCommand>();

            InitializeCommands();
        }

        ~AbstractDiagram()
        {
            Dispose(false);
        }

        #endregion Конструктор

        #region Свойства

        /// <summary>
        /// Коллекция элементов
        /// </summary>
        public List<DiagramEntity> Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        /// <summary>
        /// Информация о версии
        /// </summary>
        public VersionInfo Version
        {
            get { return version; }
        }

        /// <summary>
        /// Документ на стороне сервера в котором хранится диаграмма
        /// </summary>
        public IDocument Document
        {
            get { return document; }
            set { document = value; }
        }

        /// <summary>
        /// Была ли изменена диаграмма
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return isChanged;
            }

            set
            {
                isChanged = value;
                site.IsChanged = value;
            }
        }

        /// <summary>
        /// Коллекция доступных команд для диаграммы.
        /// </summary>
        public Dictionary<string, BaseCommand> DiagramСommands
        {
            get { return diagramСommands; }
            set { diagramСommands = value; }
        }

        public DiagramEditor.DiargamEditor Site
        {
            get { return site; }
            set { site = value; }
        }

        #endregion Свойства

        #region Методы

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Инициализация команд
        /// </summary>
        public virtual void InitializeCommands()
        {
            Command saveCommand = new Commands.SaveDiagramCommand(this);
            Command saveMetafileCommand = new Commands.CommandSaveMetafile(this);
            Command refreshCommand = new Commands.CommandRefresh(this);
            Command undoCommand = new Commands.CommandUndo(this);
            Command redoCommand = new Commands.CommandRedo(this);
            Command printPreviewCommand = new Commands.CommandPrintPreview(this);

            // Команды контекстного меню диаграммы
            diagramСommands.Add("/ContextMenu/Добавить комментарий", new Commands.CommandNewComment(this));
            diagramСommands.Add("/ContextMenu/-", null);
            diagramСommands.Add("/ContextMenu/Отменить", undoCommand);
            diagramСommands.Add("/ContextMenu/Вернуть", redoCommand);
            diagramСommands.Add("/ContextMenu/--", null);
            diagramСommands.Add("/ContextMenu/Сохранить", saveCommand);
            diagramСommands.Add("/ContextMenu/Сохранить как метафайл", saveMetafileCommand);
            diagramСommands.Add("/ContextMenu/---", null);
            diagramСommands.Add("/ContextMenu/Предварительный просмотр", printPreviewCommand);
            diagramСommands.Add("/ContextMenu/Обновить", refreshCommand);

            diagramСommands.Add("/ToolBar/Сохранить", saveCommand);
            diagramСommands.Add("/ToolBar/Сохранить как метафайл", saveMetafileCommand);
            diagramСommands.Add("/ToolBar/Обновить", refreshCommand);
            diagramСommands.Add("/ToolBar/Предварительный просмотр", printPreviewCommand);
            diagramСommands.Add("/ToolBar/Печать на одной странице", new Commands.CommandPrintOnePage(this));
            diagramСommands.Add("/ToolBar/Параметры страницы", new Commands.CommandPageSetup(this));
            diagramСommands.Add("/ToolBar/Отменить", undoCommand);
            diagramСommands.Add("/ToolBar/Вернуть", redoCommand);
        }
        
        /// <summary>
        /// Прямой обход элементов
        /// </summary>
        public void Visiting(IVisitor visitor)
        {
            foreach (DiagramEntity item in entities)
            {
                visitor.Visit(item);
            }
        }

        /// <summary>
        /// Прямой обход элементов
        /// </summary>
        public void DirectVisiting(Type type, IVisitor visitor)
        {
            foreach (DiagramEntity item in entities)
            {
                if (item.GetType().FullName == type.FullName)
                {
                    visitor.Visit(item);
                }
            }
        }

        #region Загрузка диаграммы
        
        /// <summary>
        /// Загрузка диаграммы из документа
        /// </summary>
        public void Load()
        {
            try
            {
                if (document == null)
                {
                    return;
                }

                if (!String.IsNullOrEmpty(document.Configuration))
                {
                    // InitializeCommands();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(document.Configuration);

                    // XmlDocument doc = Validator.LoadDocument(document.Configuration);
                    XmlNode xmlDiagramOptions = doc.SelectSingleNode("/DatabaseConfiguration/Document/Diagram/DiagramOptions");
                    if (xmlDiagramOptions != null)
                    {
                        foreach (XmlNode xmlNode in xmlDiagramOptions.ChildNodes)
                        {
                            LoadOptions(xmlNode);
                        }
                    }

                    XmlNode xmlSymbols = doc.SelectSingleNode("/DatabaseConfiguration/Document/Diagram/Symbols");
                    foreach (XmlNode xmlNode in xmlSymbols.ChildNodes)
                    {
                        LoadSymbol(xmlNode);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка открытия диаграммы", e);
            }
        }

        #endregion Загрузка диаграммы

        /// <summary>
        /// Сохраняет диаграмму в документ на сервере
        /// </summary>
        public void Save()
        {
            string configuration = ToXml();
            Validator.LoadDocument(configuration);
            document.Configuration = configuration;
        }

        internal DiagramEntity FindDiagramEntityByFullName(string fullName)
        {
            foreach (DiagramEntity item in entities)
            {
                string[] partsItem = item.Key.Split(
                    new string[]
                        {
                            "::"
                        },
                    StringSplitOptions.None);
                string[] partsFind = fullName.Split(new string[] { "::" }, StringSplitOptions.None);
                if (partsItem[partsItem.Length - 1] == partsFind[partsFind.Length - 1])
                {
                    return item;
                }
            }

            return null;
        }

        internal DiagramEntity FindDiagramEntityByID(Guid id)
        {
            foreach (DiagramEntity item in entities)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Проверяет может ли объект находится на диаграмме.
        /// </summary>
        internal abstract bool IsAllowedEntity(object obj);

        private static Rectangle ParseRectangle(string s)
        {
            string[] p = s.Split(',');
            return new Rectangle(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]));
        }

        private static List<Point> ParsePoints(string s)
        {
            List<Point> array = new List<Point>();
            string[] points = s.Split(';');
            foreach (string item in points)
            {
                string[] p = item.Split(',');
                array.Add(new Point(int.Parse(p[0]), int.Parse(p[1])));
            }

            return array;
        }

        private static Font ParseFont(string s)
        {
            string[] p = s.Split(';');

            FontStyle fontStylefStyle = ParseFontStyle(p[2]);

            return new Font(p[0], float.Parse(p[1]), fontStylefStyle);
        }

        private static FontStyle ParseFontStyle(string fontStyle)
        {
            switch (fontStyle.ToUpper())
            {
                case "BOLD":
                    return FontStyle.Bold;
                case "ITALIC":
                    return FontStyle.Italic;
                case "REGULAR":
                    return FontStyle.Regular;
                case "BOLD, STRIKEOUT":
                    return FontStyle.Bold | FontStyle.Strikeout;
                case "ITALIC, STRIKEOUT":
                    return FontStyle.Italic | FontStyle.Strikeout;
                case "STRIKEOUT":
                    return FontStyle.Regular | FontStyle.Strikeout;
                case "BOLD, UNDERLINE":
                    return FontStyle.Bold | FontStyle.Underline;
                case "ITALIC, UNDERLINE":
                    return FontStyle.Italic | FontStyle.Underline;
                case "UNDERLINE":
                    return FontStyle.Regular | FontStyle.Underline;
                case "BOLD, ITALIC, UNDERLINE":
                    return FontStyle.Bold | FontStyle.Italic | FontStyle.Underline;
                case "BOLD, ITALIC, STRIKEOUT":
                    return FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout;
                default:
                    throw new Exception("Несущесвующий стиль");
            }
        }

        private static Color ParseColor(string s)
        {
            return Color.FromArgb(int.Parse(s));
        }

        private void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    Entities.Clear();
                }
            }
        }

        /// <summary>
        /// Сохраняет диаграмму в XML
        /// </summary>
        /// <returns>XML-описание диаграммы</returns>
        private string ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.InnerXml = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><DatabaseConfiguration/>";

            XmlNode xmlDocument = doc.CreateElement("Document");
            XmlHelper.SetAttribute(xmlDocument, "type", "Diagram");
            XmlHelper.SetAttribute(xmlDocument, "name", document.Name);
            XmlHelper.SetAttribute(xmlDocument, "description", document.Description);
            doc.DocumentElement.AppendChild(xmlDocument);

            XmlNode xmlDiagram = doc.CreateElement("Diagram");
            xmlDocument.AppendChild(xmlDiagram);

            XmlNode xmlDiagramOptions = doc.CreateElement("DiagramOptions");
            XmlHelper.AddChildNode(xmlDiagramOptions, "Zoom", String.Format("{0}", site.ZoomFactor));
            xmlDiagram.AppendChild(xmlDiagramOptions);

            XmlNode xmlSymbols = doc.CreateElement("Symbols");

            Visitors.Save2XMLVisitor save2XMLVisitor = new Visitors.Save2XMLVisitor(xmlSymbols);
            DirectVisiting(typeof(UMLEntityBase), save2XMLVisitor);
            DirectVisiting(typeof(UMLAssociation), save2XMLVisitor);
            DirectVisiting(typeof(UMLAssociationStereotype), save2XMLVisitor);
            DirectVisiting(typeof(UMLLabel), save2XMLVisitor);
            DirectVisiting(typeof(UMLAnchorEntityToNote), save2XMLVisitor);
            xmlDiagram.AppendChild(save2XMLVisitor.RootNode);

            return doc.InnerXml;
        }

        private void LoadLinkSymbol(XmlNode xmlNode)
        {
            string key = xmlNode.SelectSingleNode("Object").InnerText;
            IEntityAssociation association = site.SchemeEditor.Scheme.GetObjectByKey(key) as IEntityAssociation;
            if (association != null)
            {
                IEntity roleAEntity = site.SchemeEditor.Scheme.RootPackage.FindEntityByName(association.RoleData.ObjectKey);
                IEntity roleBEntity = site.SchemeEditor.Scheme.RootPackage.FindEntityByName(association.RoleBridge.ObjectKey);
                if (roleAEntity != null && roleBEntity != null)
                {
                    List<Point> points = ParsePoints(xmlNode.SelectSingleNode("ListOfPoints").InnerText);

                    // List<Point> points = new List<Point>();
                    Guid sourceSymbolID = Guid.Empty;
                    Guid dectinationSymbolID = Guid.Empty;

                    if (xmlNode.SelectSingleNode("SourceSymbol") != null)
                    {
                        sourceSymbolID = new Guid(xmlNode.SelectSingleNode("SourceSymbol").InnerText);
                    }

                    if (xmlNode.SelectSingleNode("DestinationSymbol") != null)
                    {
                        dectinationSymbolID = new Guid(xmlNode.SelectSingleNode("DestinationSymbol").InnerText);
                    }

                    UMLAssociation entity;

                    if (sourceSymbolID.Equals(Guid.Empty) || dectinationSymbolID.Equals(Guid.Empty))
                    {
                        entity = UMLAssociationFactory.Create(
                            association.AssociationClassType,
                            association.Key,
                            Guid.NewGuid(),
                            this,
                            FindDiagramEntityByFullName(roleAEntity.Key),
                            FindDiagramEntityByFullName(roleBEntity.Key),
                            points);
                    }
                    else
                    {
                        entity = UMLAssociationFactory.Create(
                            association.AssociationClassType,
                            association.Key,
                            Guid.NewGuid(),
                            this,
                            FindDiagramEntityByID(sourceSymbolID),
                            FindDiagramEntityByID(dectinationSymbolID),
                            points);
                    }

                    if (xmlNode.SelectSingleNode("ID") != null)
                    {
                        entity.ID = new Guid(xmlNode.SelectSingleNode("ID").InnerText);
                    }

                    entity.EntityRectangle = ParseRectangle(xmlNode.SelectSingleNode("Rect").InnerText);
                    entity.PenWithCap.Color = ParseColor(xmlNode.SelectSingleNode("LineColor").InnerText);
                    entity.Pen.Color = ParseColor(xmlNode.SelectSingleNode("LineColor").InnerText);
                    entity.LineWidth = int.Parse(xmlNode.SelectSingleNode("LineWidth").InnerText);

                    entities.Add(entity);
                }
            }

            // TODO Сохранять в XML SourceSymbol и DectinationSymbol и если ассоциация по ключу не найдена, то создавать "отсутствующую" ассоциацию
        }

        private void LoadEntitySymbol(XmlNode xmlNode)
        {
            string key = xmlNode.SelectSingleNode("Object").InnerText;
            UMLEntityBase entity = new UMLEntityBase(key, Guid.NewGuid(), this, 0, 0);

            if (xmlNode.SelectSingleNode("ID") != null)
            {
                entity.ID = new Guid(xmlNode.SelectSingleNode("ID").InnerText);
            }

            entity.EntityRectangle = ParseRectangle(xmlNode.SelectSingleNode("Rect").InnerText);
            entity.LineColor = ParseColor(xmlNode.SelectSingleNode("LineColor").InnerText);

            if (xmlNode.SelectSingleNode("LineWidth") != null)
            {
                entity.LineWidth = int.Parse(xmlNode.SelectSingleNode("LineWidth").InnerText);
            }

            entity.FillColor = ParseColor(xmlNode.SelectSingleNode("FillColor").InnerText);

            if (xmlNode.SelectSingleNode("IsShadow") != null)
            {
                entity.IsShadow = bool.Parse(xmlNode.SelectSingleNode("IsShadow").InnerText);
            }

            if (xmlNode.SelectSingleNode("IsStereotype") != null)
            {
                entity.StereotypeVisible = bool.Parse(xmlNode.SelectSingleNode("IsStereotype").InnerText);
            }

            if (xmlNode.SelectSingleNode("ShadowColor") != null)
            {
                entity.ShadowColor = ParseColor(xmlNode.SelectSingleNode("ShadowColor").InnerText);
            }

            entity.Font = ParseFont(xmlNode.SelectSingleNode("FontName").InnerText);

            if (xmlNode.SelectSingleNode("SuppressAttributes") != null)
            {
                entity.IsSuppressAttribute = bool.Parse(xmlNode.SelectSingleNode("SuppressAttributes").InnerText);
            }

            if (xmlNode.SelectSingleNode("FontColor") != null)
            {
                entity.TextColor = ParseColor(xmlNode.SelectSingleNode("FontColor").InnerText);
            }

            if (xmlNode.SelectSingleNode("SQLExpression") != null)
            {
                entity.SqlExpression = bool.Parse(xmlNode.SelectSingleNode("SQLExpression").InnerText);
            }

            if (Document != null && entity.CommonObject != null)
            {
                if (Document.ParentPackage != (entity.CommonObject as IEntity).ParentPackage)
                {
                    entity.Export = String.Format("( from_{0})", (entity.CommonObject as IEntity).ParentPackage.Name);
                }
            }

            entities.Add(entity);
        }

        private void LoadLabelSymbol(XmlNode xmlNode)
        {
            UMLLabel label = null;

            // ассоциация, к которой привязан label
            UMLAssociation umlAssociation = null;

            if (xmlNode.SelectSingleNode("AssociateID") != null)
            {
                umlAssociation = FindDiagramEntityByID(new Guid(xmlNode.SelectSingleNode("AssociateID").InnerText)) as UMLAssociation;
            }
            else if (xmlNode.SelectSingleNode("AssociateKey") != null)
            {
                umlAssociation = FindDiagramEntityByFullName(xmlNode.SelectSingleNode("AssociateKey").InnerText) as UMLAssociation;
            }

            if (umlAssociation != null)
            {
                // label привязан к ассоциации
                label = umlAssociation.GetUMLAssociationStereotype();

                // Отсутствует у ассоциации одна из ролей
                if (label == null)
                {
                    return;
                }
            }
            else
            {
                label = new UMLLabel(Guid.NewGuid(), this, 0, 0);
            }

            if (xmlNode.SelectSingleNode("ID") != null)
            {
                label.ID = new Guid(xmlNode.SelectSingleNode("ID").InnerText);
            }

            label.EntityRectangle = ParseRectangle(xmlNode.SelectSingleNode("Rect").InnerText);
            if (xmlNode.SelectSingleNode("LineColor") != null)
            {
                label.LineColor = ParseColor(xmlNode.SelectSingleNode("LineColor").InnerText);
            }

            if (xmlNode.SelectSingleNode("FillColor") != null)
            {
                label.FillColor = ParseColor(xmlNode.SelectSingleNode("FillColor").InnerText);
            }

            label.Font = ParseFont(xmlNode.SelectSingleNode("FontName").InnerText);
            label.Text = xmlNode.SelectSingleNode("Text").InnerText;
            if (xmlNode.SelectSingleNode("FontColor") != null)
            {
                label.TextColor = ParseColor(xmlNode.SelectSingleNode("FontColor").InnerText);
            }

            if (xmlNode.SelectSingleNode("IsFormatted") != null)
            {
                label.IsFormatted = bool.Parse(xmlNode.SelectSingleNode("IsFormatted").InnerText);
            }

            if (xmlNode.SelectSingleNode("Visible") != null)
            {
                label.Visible = bool.Parse(xmlNode.SelectSingleNode("Visible").InnerText);
            }

            entities.Add(label);

            if (umlAssociation != null)
            {
                umlAssociation.StereotypeEntity = (UMLAssociationStereotype)label;
            }
        }

        private void LoadAnchorSymbol(XmlNode xmlNode)
        {
            DiagramEntity roleA = this.FindDiagramEntityByID(new Guid(xmlNode.SelectSingleNode("ParentID").InnerText));
            DiagramEntity roleB = this.FindDiagramEntityByID(new Guid(xmlNode.SelectSingleNode("ChildID").InnerText));
            if (roleA != null && roleB != null)
            {
                List<Point> points = ParsePoints(xmlNode.SelectSingleNode("ListOfPoints").InnerText);

                UMLAnchorEntityToNote anchor = new UMLAnchorEntityToNote(Guid.NewGuid(), this, roleA, roleB, points);

                anchor.ID = new Guid(xmlNode.SelectSingleNode("ID").InnerText);

                anchor.EntityRectangle = ParseRectangle(xmlNode.SelectSingleNode("Rect").InnerText);
                anchor.Pen.Color = ParseColor(xmlNode.SelectSingleNode("LineColor").InnerText);
                anchor.LineWidth = int.Parse(xmlNode.SelectSingleNode("LineWidth").InnerText);

                entities.Add(anchor);
            }
        }

        private void LoadPackageSymbol(XmlNode xmlNode)
        {
            string key = xmlNode.SelectSingleNode("Object").InnerText;
            UMLPackage package = new UMLPackage(key, Guid.NewGuid(), this, 0, 0);
            if (xmlNode.SelectSingleNode("ID") != null)
            {
                package.ID = new Guid(xmlNode.SelectSingleNode("ID").InnerText);
            }

            package.EntityRectangle = ParseRectangle(xmlNode.SelectSingleNode("Rect").InnerText);
            package.LineColor = ParseColor(xmlNode.SelectSingleNode("LineColor").InnerText);
            package.LineWidth = int.Parse(xmlNode.SelectSingleNode("LineWidth").InnerText);
            package.FillColor = ParseColor(xmlNode.SelectSingleNode("FillColor").InnerText);
            package.Font = ParseFont(xmlNode.SelectSingleNode("FontName").InnerText);

            // package.TextColor = ParseColor(xmlNode.SelectSingleNode("FontColor").InnerText);
            entities.Add(package);
        }

        private void LoadSymbol(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "EntitySymbol": 
                    LoadEntitySymbol(xmlNode); 
                    break;
                case "LinkSymbol": 
                    LoadLinkSymbol(xmlNode); 
                    break;
                case "TextSymbol": 
                    LoadLabelSymbol(xmlNode);
                    break;
                case "AnchorSymbol": 
                    LoadAnchorSymbol(xmlNode); 
                    break;
                case "PackageSymbol":
                    LoadPackageSymbol(xmlNode); 
                    break;
            }
        }

        /// <summary>
        /// Загрузка опций диаграммы
        /// </summary>
        private void LoadOptions(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "Zoom":
                    site.ZoomFactor = int.Parse(xmlNode.InnerText);
                    break;
            }
        }

        #endregion Методы

        #region Struct
        
        /// <summary>
        /// Настройки отображения
        /// </summary>
        private struct DisplayPreferences
        {
        }

        #endregion
    }
}
