using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor.Commands;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Редактор диаграмм
    /// </summary>
    public partial class DiargamEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ContextMenuStrip contextMenuDiagramEditor;
        private System.Windows.Forms.ToolStripMenuItem undo;
        private System.Windows.Forms.ToolStripMenuItem redo;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiargamEditor));
            this.contextMenuDiagramEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undo = new System.Windows.Forms.ToolStripMenuItem();
            this.redo = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuDiagramEditor.SuspendLayout();
            this.SuspendLayout();
			// 
            // DiargamEditor
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.MinimumSize = new System.Drawing.Size(20, 20);
            this.Size = new System.Drawing.Size(20, 20);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DiargamEditor_MouseDoubleClick);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.DiargamEditor_Scroll);
            this.contextMenuDiagramEditor.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void InitializeImageList()
        {
            imageList = new Dictionary<Images, Bitmap>();

            this.imageList.Add(Images.imgPrintPreview, Resource.Предварительный_просмотр_16х16_Вариант2);
            this.imageList.Add(Images.imgRefresh, Resource.Обновить_16х16_Вариант2);
            this.imageList.Add(Images.imgSaveAsMetafile, Resource.Сохранить_как_метафайл_16х16_Вариант1T);
            this.imageList.Add(Images.imgSaveDiagram, Resource.Сохранить_16х16_Вариант1);
            this.imageList.Add(Images.imgDeletSymbol, Resource.Удалить__16х16_Вариант1);
            this.imageList.Add(Images.imgDeletFromScheme, Resource.Удалить_16х16_Вариант3);
            this.imageList.Add(Images.imgFindInTree, Resource.Найти_в_папке_пакете__16х16_Вариант3);
            this.imageList.Add(Images.imgOptions, Resource.Опции_16х16_Вариант1);
            this.imageList.Add(Images.imgFormat, Resource.Формат_16х16_Вариант2);
            this.imageList.Add(Images.imgBackGround, Resource.На_задний_фон_16х16_Вариант1);
            this.imageList.Add(Images.imgForeGround, Resource.На_передний_план_16х16_Вариант1);
            this.imageList.Add(Images.imgAutoSize, Resource.Авторазмер_16х16_Вариант1);
            this.imageList.Add(Images.imgClsBridge, Resource.Сопоставимый_16х16_Вариант1);
            this.imageList.Add(Images.imgClsData, Resource.КД_16х16_Вариант1);
            this.imageList.Add(Images.imgClsFact, Resource.Таблица_фактов_16х16_Вариант1);
            this.imageList.Add(Images.imgClsFix, Resource.Фиксированный_16х16_Вариант1t);
            this.imageList.Add(Images.imgNewAssociation, Resource.Добавить_ассоциацию_16х16_Вариант1);
            this.imageList.Add(Images.imgBridgeAssociation, Resource.Добавить_ассоциацию_16х16_Вариант2);
            this.imageList.Add(Images.imgMDAssociation, Resource.Добавить_ассоциациюMD_16х16_Вариант1);
            this.imageList.Add(Images.imgNewComment, Resource.Добавить_комментарий_16х16_Вариант1);
            this.imageList.Add(Images.imgNewEntity, Resource.Добавить_класс_16х16_Вариант1);
            this.imageList.Add(Images.imgNewPackage, Resource.Добавить_пакет_16х16_Вариант2);
            this.imageList.Add(Images.imgStereotype, Resource.Стереотип_16х16_Вариант2);
            this.imageList.Add(Images.imgAttrVisible, Resource.Видимость_атрибутов_16х16_Вариант2);
            this.imageList.Add(Images.imgAttributes, Resource.Опции_Атрибуты_16х16_Вариант1);
            this.imageList.Add(Images.imgShow, Resource.Показать_16х16_Вариант1);
            this.imageList.Add(Images.imgHide, Resource.Скрыть_16х16_Вариант1);
            this.imageList.Add(Images.imgStandartFormatting, Resource.Стандартное_форматирование_16х16_Вариант1t);
            this.imageList.Add(Images.imgFillColor, Resource.Цвет_фона_16х16_Вариант1);
            this.imageList.Add(Images.imgFont, Resource.Шрифт_16х16_Вариант2);
            this.imageList.Add(Images.imgLineWidht, Resource.Толщина_линии_16х16_Вариант1);
            this.imageList.Add(Images.imgColorLine, Resource.Цвет_линии_16х16_Вариант1);
            this.imageList.Add(Images.imgShadow, Resource.Цвет_тени_16х16_Вариант2);
            this.imageList.Add(Images.imgPageSetupDialod, Resource.Параметры_страницы_16х16_Вариант2);
            this.imageList.Add(Images.imgPrint, Resource.Печать_16х16_Вариант1);
            this.imageList.Add(Images.imgPrintOnPage, Resource.Печать_на_одной_странице_16х16_Вариант1);
            this.imageList.Add(Images.imgEditRedo, Resource.Редактировать_выполнить_повторно_16х16_Вариант1);
            this.imageList.Add(Images.imgEditUndo, Resource.Редактировать_отменить_16х16_Вариант1);
            this.imageList.Add(Images.imgNewtable, Resource.Таблица_Вариант1);
            this.ImageList.Add(Images.imgStereotypeDefPos, Resource.Сттереотип_16х16_Вариант2);
        }

        #endregion
    }
}
