using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor.Commands;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый для всех объектов диаграммы
    /// </summary>
    public abstract partial class DiagramEntity
    {
        #region ToolStripMenuItems

        /// <summary>
        /// Удаление объекта
        /// </summary>
        private ToolStripMenuItem deleteToolStripMenuItem = new ToolStripMenuItem();

        /// <summary>
        /// Удаление со схемы
        /// </summary>
        private ToolStripMenuItem deleteFromSchemeToolStripMenuItem = new ToolStripMenuItem();

        /// <summary>
        /// Опции объекта
        /// </summary>
        private ToolStripMenuItem options = new ToolStripMenuItem();

        /// <summary>
        /// Стереотип объекта
        /// </summary>
        private ToolStripMenuItem cmdStereotype = new ToolStripMenuItem();

        /// <summary>
        /// Показывать стереотип
        /// </summary>
        private ToolStripMenuItem cmdStereotypeDisplay = new ToolStripMenuItem();

        /// <summary>
        /// не показывать стереотип
        /// </summary>
        private ToolStripMenuItem cmdStereotypeNotDisplay = new ToolStripMenuItem();

        /// <summary>
        /// Форматирование объекта
        /// </summary>
        private ToolStripMenuItem tsmiFormat = new ToolStripMenuItem();

        /// <summary>
        /// Цвет линий объекта
        /// </summary>
        private ToolStripMenuItem tsmilineColor = new ToolStripMenuItem();

        /// <summary>
        /// Толщина линий
        /// </summary>
        private ToolStripMenuItem tsmiLineWidht = new ToolStripMenuItem();

        /// <summary>
        /// Объект в CustomTree 
        /// </summary>
        private ToolStripMenuItem tsmiSelectInBrowser = new ToolStripMenuItem();

        /// <summary>
        /// Переносим объект на фон 
        /// </summary>
        private ToolStripMenuItem tsmiBackGroundObject = new ToolStripMenuItem();

        /// <summary>
        /// Переносим объект на передний план
        /// </summary>
        private ToolStripMenuItem tsmiForeGroundObject = new ToolStripMenuItem();

        /// <summary>
        /// Свойства объекта
        /// </summary>
        private ToolStripMenuItem properties = new ToolStripMenuItem();

        /// <summary>
        /// Стандартное форматирование
        /// </summary>
        private ToolStripMenuItem standartFormat = new ToolStripMenuItem();

        /// <summary>
        /// Разделители меню
        /// </summary>
        private ToolStripSeparator separator1 = new ToolStripSeparator();
        private ToolStripSeparator separator2 = new ToolStripSeparator();
        private ToolStripSeparator separator3 = new ToolStripSeparator();

        #endregion ToolStripMenuItems

        #region Commands

        /// <summary>
        /// Команда удаления объекта с диаграммы
        /// </summary>
        private Command cmdDeleteSymbol;

        /// <summary>
        /// Команда удаления из схемы
        /// </summary>
        private Command cmdDeletFromScheme;

        /// <summary>
        /// Команда поиска в дереве объектов
        /// </summary>
        private Command cmdFindInTree;

        /// <summary>
        /// На задний план
        /// </summary>
        private Command cmdBackGround;

        /// <summary>
        /// На передний план
        /// </summary>
        private Command cmdForeGround;

        /// <summary>
        /// Команда показать/скрыть стереотип
        /// </summary>
        private Command cmdStereotypeShow;
        private Command cmdSStereotypeHide;

        /// <summary>
        /// Команда стандартного форматирования
        /// </summary>
        private Command cmdStabdartFormatting;

        /// <summary>
        /// Команда редактирования толщины линии
        /// </summary>
        private Command cmdLineWidhtManager;

        /// <summary>
        /// Команда редактирования цвета линии
        /// </summary>
        private Command cmdLineColorManager;

        #endregion Commands

        #region Properties

        protected ToolStripMenuItem LColor
        {
            get { return tsmilineColor; }
        }

        protected ToolStripMenuItem LWidht
        {
            get { return tsmiLineWidht; }
        }

        protected ToolStripMenuItem TsmiFormat
        {
            get { return tsmiFormat; }
        }

        protected ToolStripMenuItem CmStereotypeNotDisplay
        {
            get { return cmdStereotypeNotDisplay; }
        }

        protected ToolStripMenuItem CmStereotypeDisplay
        {
            get { return cmdStereotypeDisplay; }
        }

        protected ToolStripMenuItem CmStereotype
        {
            get { return cmdStereotype; }
        }

        protected ToolStripMenuItem Options
        {
            get { return options; }
        }

        protected ToolStripMenuItem TsmiSelectInBrowser
        {
            get { return tsmiSelectInBrowser; }
        }

        protected ToolStripMenuItem TsmiBackGroundObject
        {
            get { return tsmiBackGroundObject; }
        }

        protected ToolStripMenuItem TsmiForeGroundObject
        {
            get { return tsmiForeGroundObject; }
        }

        protected ToolStripMenuItem Properties
        {
            get { return properties; }
        }

        protected ToolStripMenuItem StandartFormat
        {
            get { return standartFormat; }
        }

        protected ToolStripSeparator Separator1
        {
            get { return separator1; }
        }

        protected ToolStripSeparator Separator2
        {
            get { return separator2; }
        }

        protected ToolStripSeparator Separator3
        {
            get { return separator3; }
        }

        protected ToolStripMenuItem DeleteFromScheme
        {
            get { return deleteFromSchemeToolStripMenuItem; }
        }

        #endregion Properties

        #region Методы отвечающие исключительно за инициализацию
        
        /// <summary>
        /// Инициализация свойст по-умолчанию (переопределяемый метод)
        /// </summary>
        public virtual void InitializeDefault()
        {
            this.LineWidth = ConstLineWidth;
            this.LineColor = ConstLineColor;
            this.Font = ConstFont;
            this.TextColor = ConstFontColor;
            this.Pen = ConstPen;
        }

        /// <summary>
        /// Инициализация контекстного меню
        /// </summary>
        public virtual void ContextMenuInitialize()
        {
            this.deleteFromSchemeToolStripMenuItem.Visible = (Diagram.Site.SelectedEntities.Count > 1) ? false : true;
            this.TsmiSelectInBrowser.Visible = (Diagram.Site.SelectedEntities.Count > 1) ? false : true;
            this.TsmiForeGroundObject.Visible = (Diagram.Site.SelectedEntities.Count > 1) ? false : true;
            this.TsmiBackGroundObject.Visible = (Diagram.Site.SelectedEntities.Count > 1) ? false : true;

            // Delete
            deleteToolStripMenuItem.Name = "Delete";
            deleteToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            deleteToolStripMenuItem.Text = "Удалить объект с диаграммы";
            deleteToolStripMenuItem.Tag = cmdDeleteSymbol;
            deleteToolStripMenuItem.Image = cmdDeleteSymbol.Image;

            // Delete frob scheme
            this.deleteFromSchemeToolStripMenuItem.Name = "DeleteFromScheme";
            this.deleteFromSchemeToolStripMenuItem.Size = new Size(192, 22);
            this.deleteFromSchemeToolStripMenuItem.Text = "Удалить объект из схемы";
            this.deleteFromSchemeToolStripMenuItem.Tag = cmdDeletFromScheme;
            this.deleteFromSchemeToolStripMenuItem.Image = cmdDeletFromScheme.Image;

            // Options
            Options.Name = "Options";
            Options.Size = new System.Drawing.Size(192, 22);
            Options.Text = "О&пции";
            this.Options.ShowShortcutKeys = true;
            this.Options.Image = Diagram.Site.ImageList[Images.imgOptions];
            this.Options.ImageTransparentColor = Color.FromArgb(Command.transparentColor);

            // Стереотип
            this.cmdStereotype.AutoToolTip = true;
            this.cmdStereotype.Name = "Display";
            this.cmdStereotype.Size = new System.Drawing.Size(194, 22);
            this.cmdStereotype.Text = "&Стереотип";
            this.cmdStereotype.ShowShortcutKeys = true;
            this.cmdStereotype.Image = Diagram.Site.ImageList[Images.imgStereotype];
            this.cmdStereotype.ImageTransparentColor = Color.FromArgb(Command.transparentColor);

            // Показать
            this.cmdStereotypeDisplay.AutoToolTip = true;
            this.cmdStereotypeDisplay.Name = "cmStereotypeDisplay";
            this.cmdStereotypeDisplay.Size = new System.Drawing.Size(194, 22);
            this.cmdStereotypeDisplay.Text = "Показать";
            this.cmdStereotypeDisplay.ToolTipText = "Показывает стереотип у всех выделенных объектов";
            this.cmdStereotypeDisplay.Tag = cmdStereotypeShow;
            this.cmdStereotypeDisplay.Image = cmdStereotypeShow.Image;

            // Скрыть
            this.cmdStereotypeNotDisplay.AutoToolTip = true;
            this.cmdStereotypeNotDisplay.Name = "cmStereotypeNotDisplay";
            this.cmdStereotypeNotDisplay.Size = new System.Drawing.Size(194, 22);
            this.cmdStereotypeNotDisplay.Text = "Скрыть";
            this.cmdStereotypeNotDisplay.ToolTipText = "Скрывает стереотип у всех выделенных объектов";
            this.cmdStereotypeNotDisplay.Tag = cmdSStereotypeHide;
            this.cmdStereotypeNotDisplay.Image = cmdSStereotypeHide.Image;

            // Format
            this.TsmiFormat.Name = "Format";
            this.TsmiFormat.Size = new System.Drawing.Size(192, 22);
            this.TsmiFormat.Text = "&Формат";
            this.TsmiFormat.ShowShortcutKeys = true;
            this.TsmiFormat.Image = Diagram.Site.ImageList[Images.imgFormat];
            this.TsmiFormat.ImageTransparentColor = Color.FromArgb(Command.transparentColor);

            // LineColor
            this.LColor.Name = "LineColor";
            this.LColor.Size = new System.Drawing.Size(163, 22);
            this.LColor.Text = "Цвет &линии...";
            this.LColor.ShowShortcutKeys = true;
            this.LColor.Tag = cmdLineColorManager;
            this.LColor.Image = cmdLineColorManager.Image;

            // LineWidht
            this.LWidht.Name = "LineWidth";
            this.LWidht.Size = new System.Drawing.Size(163, 22);
            this.LWidht.Text = "&Толщина линии...";
            this.LWidht.ShowShortcutKeys = true;
            this.LWidht.Tag = cmdLineWidhtManager;
            this.LWidht.Image = cmdLineWidhtManager.Image;

            // SelectInBrowser
            this.TsmiSelectInBrowser.Name = "SelectInBrowser";
            this.TsmiSelectInBrowser.Size = new Size(192, 22);
            this.TsmiSelectInBrowser.Text = "Найти в дереве объектов";
            this.TsmiSelectInBrowser.Tag = cmdFindInTree;
            this.TsmiSelectInBrowser.Image = cmdFindInTree.Image;

            // BackGroundObject
            this.TsmiBackGroundObject.Name = "BackGroundObject";
            this.TsmiBackGroundObject.Size = new Size(192, 22);
            this.TsmiBackGroundObject.Text = "Перенести на задний план";
            this.TsmiBackGroundObject.Tag = cmdBackGround;
            this.TsmiBackGroundObject.Image = cmdBackGround.Image;

            // ForeGroundObject
            this.TsmiForeGroundObject.Name = "ForeGroundObject";
            this.TsmiForeGroundObject.Size = new Size(192, 22);
            this.TsmiForeGroundObject.Text = "Перенести на передний план";
            this.TsmiForeGroundObject.Tag = cmdForeGround;
            this.TsmiForeGroundObject.Image = cmdForeGround.Image;

            // Стандартный настройки форматирования
            this.StandartFormat.Name = "StandartFormat";
            this.StandartFormat.Text = "Стандартное форматирование";
            this.StandartFormat.Size = new System.Drawing.Size(144, 22);
            this.StandartFormat.ToolTipText = "Вернуть стандартное форматирование объекта";
            this.StandartFormat.Tag = cmdStabdartFormatting;
            this.StandartFormat.Image = cmdStabdartFormatting.Image;

            // Properties
            this.Properties.Name = "Properties";
            this.Properties.Size = new System.Drawing.Size(144, 22);
            this.Properties.Text = "Свойства...";
            this.Properties.Visible = false;
#if DRAW_INVIS_REGIONS
            // режим отладки
            this.Properties.Visible = true;
#endif

            this.TsmiFormat.DropDownItems.AddRange(new ToolStripItem[] { StandartFormat, separator1, LColor, LWidht });

            this.contextMenu.Items.AddRange(new ToolStripItem[] { deleteToolStripMenuItem, deleteFromSchemeToolStripMenuItem, separator2, Options, TsmiFormat, separator3, TsmiSelectInBrowser, TsmiBackGroundObject, TsmiForeGroundObject, Properties });

            this.Options.DropDownItems.AddRange(new ToolStripItem[] { cmdStereotype });

            this.cmdStereotype.DropDownItems.AddRange(new ToolStripItem[] { cmdStereotypeDisplay, cmdStereotypeNotDisplay });
        }

        /// <summary>
        /// Инициализация команд
        /// </summary>
        protected virtual void InitializeCommand()
        {
            cmdDeleteSymbol = new CommandDeleteSymbol(diagram);
            cmdDeletFromScheme = new CommandDeletFromScheme(diagram);
            cmdFindInTree = new CommandFindInTree(this);
            cmdBackGround = new CommandBackGroundPositionManager(this);
            cmdForeGround = new CommandForeGroundPositionManager(this);
            cmdStereotypeShow = new CommandStereotypeManager(diagram, true);
            cmdSStereotypeHide = new CommandStereotypeManager(diagram, false);
            cmdStabdartFormatting = new CommandsStandartFormatting(diagram);
            cmdLineWidhtManager = new CommandLineWidthManager(diagram);
            cmdLineColorManager = new CommandLineColorManager(diagram);
        }

        #endregion
    }
}
