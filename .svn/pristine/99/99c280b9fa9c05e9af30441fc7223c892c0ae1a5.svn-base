using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor.Commands;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый класс для сущности на диаграмме
    /// </summary>
    public partial class UMLEntityBase : DiagramRectangleEntity
    {
        #region ToolStripMenuItems

        /// <summary>
        /// Видимость атрибутов
        /// </summary>
        private ToolStripMenuItem visibleAttributes = new ToolStripMenuItem();

        /// <summary>
        /// Показать атрибуты
        /// </summary>
        private ToolStripMenuItem showAttr = new ToolStripMenuItem();

        /// <summary>
        /// Скрыть атрибуты
        /// </summary>
        private ToolStripMenuItem notShowAttr = new ToolStripMenuItem();

        /// <summary>
        /// Видимость конкретных атрибутов
        /// </summary>
        private ToolStripMenuItem allAttributes = new ToolStripMenuItem();

        /// <summary>
        /// Обычные атрибуты
        /// </summary>
        private ToolStripMenuItem regular = new ToolStripMenuItem();

        /// <summary>
        /// Служебные атрибуты
        /// </summary>
        private ToolStripMenuItem servised = new ToolStripMenuItem();

        /// <summary>
        /// Системные атрибуты
        /// </summary>
        private ToolStripMenuItem sys = new ToolStripMenuItem();

        /// <summary>
        /// Показать SQL-выражение
        /// </summary>
        private ToolStripMenuItem sqlExpresiionMenuItem = new ToolStripMenuItem();

        #endregion ToolStripMenuItems

        #region Commands

        /// <summary>
        /// Команды показать\скрыть атрибуты
        /// </summary>
        private Command cmdAttrHide;
        private Command cmdAttrShow;

        /// <summary>
        /// Команда управления SQL-выражением
        /// </summary>
        private CommandWithPrm cmdSQLExpression;

        #endregion Commands

        #region Методы инициализации

        public override void ContextMenuInitialize()
        {
            base.ContextMenuInitialize();

            // SuppressAttribute
            this.visibleAttributes.Name = "VisibleAttributes";
            this.visibleAttributes.Size = new System.Drawing.Size(194, 22);
            this.visibleAttributes.Text = "Видимость атрибутов";
            this.visibleAttributes.Image = Diagram.Site.ImageList[Images.imgAttrVisible];
            this.visibleAttributes.ImageTransparentColor = Color.FromArgb(Command.transparentColor);

            // Attributes
            this.allAttributes.Name = "AllAttributes";
            this.allAttributes.Size = new System.Drawing.Size(194, 22);
            this.allAttributes.Text = "&Атрибуты";
            this.allAttributes.ShowShortcutKeys = true;
            this.allAttributes.Image = Diagram.Site.ImageList[Images.imgAttributes];
            this.allAttributes.ImageTransparentColor = Color.FromArgb(Command.transparentColor);

            // ShowAttributes
            this.showAttr.Name = "ShowAttr";
            this.showAttr.Size = new System.Drawing.Size(194, 22);
            this.showAttr.Text = "Показать";
            this.showAttr.ToolTipText = "Показывает атрибуты у всех выдленных объектов";
            this.showAttr.Tag = cmdAttrShow;
            this.showAttr.Image = cmdAttrShow.Image;

            // ShowAttributes
            this.notShowAttr.Name = "NotShowAttr";
            this.notShowAttr.Size = new System.Drawing.Size(194, 22);
            this.notShowAttr.Text = "Скрыть";
            this.notShowAttr.ToolTipText = "Скрывает атрибуты у всех выдленных объектов";
            this.notShowAttr.Tag = cmdAttrHide;
            this.notShowAttr.Image = cmdAttrHide.Image;

            // Regular
            this.regular.Checked = true;
            this.regular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.regular.Name = "Regular";
            this.regular.Size = new System.Drawing.Size(144, 22);
            this.regular.Text = "О&бычные";
            this.regular.Checked = this.regularAttrVisible;
            this.regular.ShowShortcutKeys = true;

            // Servised
            this.servised.Name = "Servised";
            this.servised.Size = new System.Drawing.Size(144, 22);
            this.servised.Text = "Слу&жебные";
            this.servised.Checked = this.servisedAttrVisible;
            this.servised.ShowShortcutKeys = true;

            // Sys
            this.sys.Name = "Sys";
            this.sys.Size = new System.Drawing.Size(144, 22);
            this.sys.Text = "Сист&емные";
            this.sys.Checked = this.sysAttrVisible;
            this.sys.ShowShortcutKeys = true;

            // SQL-выражение
            this.sqlExpresiionMenuItem.AutoToolTip = true;
            this.sqlExpresiionMenuItem.Name = "SQLExpresiionMenuItem";
            this.sqlExpresiionMenuItem.Size = new System.Drawing.Size(194, 22);
            this.sqlExpresiionMenuItem.Text = "&SQL-выражение";
            this.sqlExpresiionMenuItem.ShowShortcutKeys = true;
            this.sqlExpresiionMenuItem.CheckState = sqlExpression ? CheckState.Checked : CheckState.Unchecked;
            this.sqlExpresiionMenuItem.Tag = cmdSQLExpression;

            this.allAttributes.DropDownItems.AddRange(new ToolStripItem[]
                                                            {
                                                                regular, servised, sys
                                                            });

            this.Options.DropDownItems.AddRange(new ToolStripItem[]
                                                    {
                                                        visibleAttributes, allAttributes
                                                    });

            this.visibleAttributes.DropDownItems.AddRange(new ToolStripItem[]
                                                              {
                                                                  showAttr, notShowAttr
                                                              });

            this.ContextMenu.Items.AddRange(new ToolStripItem[]
                                                {
                                                    sqlExpresiionMenuItem
                                                 });
        }

        protected override void InitializeCommand()
        {
            base.InitializeCommand();

            cmdAttrHide = new CommandAttributesManager(Diagram, false);
            cmdAttrShow = new CommandAttributesManager(Diagram, true);
            cmdSQLExpression = new CommandSQLExpressionManager(Diagram);
        }

        #endregion
    }
}
