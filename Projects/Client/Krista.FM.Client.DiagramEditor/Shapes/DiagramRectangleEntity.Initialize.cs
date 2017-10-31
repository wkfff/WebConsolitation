using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый для всех объектов диаграммы, имеющих форму прямоугольника
    /// </summary>
    public partial class DiagramRectangleEntity : DiagramEntity
    {
        #region ToolStripMenuItems

        /// <summary>
        /// Цвет заливки
        /// </summary>
        private ToolStripMenuItem colorEntity = new ToolStripMenuItem();

        /// <summary>
        /// Шрифт объекта
        /// </summary>
        private ToolStripMenuItem entityFont = new ToolStripMenuItem();

        /// <summary>
        /// Авторазмер объекта
        /// </summary>
        private ToolStripMenuItem autoSize = new ToolStripMenuItem();

        /// <summary>
        /// Отображение тени
        /// </summary>
        private ToolStripMenuItem shadowVisible = new ToolStripMenuItem();

        /// <summary>
        /// Тень объекта
        /// </summary>
        private ToolStripMenuItem colorShadow = new ToolStripMenuItem();

        #endregion ToolStripMenuItems

        #region Commands

        /// <summary>
        /// Команда "Авторазмер"
        /// </summary>
        private Command cmdAutoSize;

        /// <summary>
        /// Команда управления цветом заливки
        /// </summary>
        private Command cmdFillColorManager;

        /// <summary>
        /// Команда управления шрифтом
        /// </summary>
        private Command cmdFontManager;

        /// <summary>
        /// Команда управления цветом тени
        /// </summary>
        private Command cmdColorShadowManager;

        /// <summary>
        /// Тень объекта
        /// </summary>
        private CommandWithPrm cmdShadow;

        protected ToolStripMenuItem ShadowVisible
        {
            get { return shadowVisible; }
            set { shadowVisible = value; }
        }

        protected ToolStripMenuItem ColorShadow
        {
            get { return colorShadow; }
            set { colorShadow = value; }
        }

        protected ToolStripMenuItem ColorEntity
        {
            get { return colorEntity; }
            set { colorEntity = value; }
        }

        #endregion Commands

        #region Команды инициализации

        /// <summary>
        /// Инициализация умолчаний
        /// </summary>
        public override void InitializeDefault()
        {
            base.InitializeDefault();

            ShadowColor = ConstShadowColor;
        }

        /// <summary>
        /// Контекстное меню, общее для всех прямоугольников
        /// </summary>
        public override void ContextMenuInitialize()
        {
            base.ContextMenuInitialize();

            // ColorEntity
            this.colorEntity.Name = "ColorEntity";
            this.colorEntity.Size = new System.Drawing.Size(192, 22);
            this.colorEntity.Text = "&Цвет фона...";
            this.colorEntity.ShowShortcutKeys = true;
            this.colorEntity.Tag = cmdFillColorManager;
            this.colorEntity.Image = cmdFillColorManager.Image;

            // EntityFont
            this.entityFont.Name = "EntityFont";
            this.entityFont.Size = new System.Drawing.Size(163, 22);
            this.entityFont.Text = "&Шрифт...";
            this.entityFont.ShowShortcutKeys = true;
            this.entityFont.Tag = cmdFontManager;
            this.entityFont.Image = cmdFontManager.Image;

            // AutoSize
            this.autoSize.Name = "AutoSize";
            this.autoSize.Size = new Size(192, 22);
            this.autoSize.Text = "Автора&змер";
            this.autoSize.ShowShortcutKeys = true;
            this.autoSize.Tag = cmdAutoSize;
            this.autoSize.Image = cmdAutoSize.Image;

            // ShadowColor
            this.colorShadow.Name = "ShadowColor";
            this.colorShadow.Size = new Size(192, 22);
            this.colorShadow.Text = "Цвет тени...";
            this.colorShadow.Enabled = isShadow;
            this.colorShadow.Tag = cmdColorShadowManager;
            this.colorShadow.Image = cmdColorShadowManager.Image;

            // ShadowVisible
            this.shadowVisible.Name = "ShadowVisible";
            this.shadowVisible.Size = new Size(192, 22);
            this.shadowVisible.Text = "Тень";
            /*if (!isShadow)
                this.ShadowVisible.Checked = false;
            else
                this.ShadowVisible.Checked = true;*/
            this.shadowVisible.Tag = cmdShadow;

            this.TsmiFormat.DropDownItems.AddRange(new ToolStripItem[] { entityFont, colorEntity, colorShadow, shadowVisible });
            this.ContextMenu.Items.AddRange(new ToolStripItem[] { autoSize });
        }

        protected override void InitializeCommand()
        {
            base.InitializeCommand();

            this.cmdAutoSize = new Commands.CommandAutoSize(Diagram);
            this.cmdFillColorManager = new Commands.CommandFillColorManager(Diagram);
            this.cmdFontManager = new Commands.CommandFontManager(Diagram);
            this.cmdColorShadowManager = new Commands.CommandShadowManager(Diagram);
            this.cmdShadow = new Commands.CommandShadowVisibleManager(Diagram);
        }

        #endregion 
    }
}
