using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor.Commands;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// �������� ��������
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

            this.imageList.Add(Images.imgPrintPreview, Resource.���������������_��������_16�16_�������2);
            this.imageList.Add(Images.imgRefresh, Resource.��������_16�16_�������2);
            this.imageList.Add(Images.imgSaveAsMetafile, Resource.���������_���_��������_16�16_�������1T);
            this.imageList.Add(Images.imgSaveDiagram, Resource.���������_16�16_�������1);
            this.imageList.Add(Images.imgDeletSymbol, Resource.�������__16�16_�������1);
            this.imageList.Add(Images.imgDeletFromScheme, Resource.�������_16�16_�������3);
            this.imageList.Add(Images.imgFindInTree, Resource.�����_�_�����_������__16�16_�������3);
            this.imageList.Add(Images.imgOptions, Resource.�����_16�16_�������1);
            this.imageList.Add(Images.imgFormat, Resource.������_16�16_�������2);
            this.imageList.Add(Images.imgBackGround, Resource.��_������_���_16�16_�������1);
            this.imageList.Add(Images.imgForeGround, Resource.��_��������_����_16�16_�������1);
            this.imageList.Add(Images.imgAutoSize, Resource.����������_16�16_�������1);
            this.imageList.Add(Images.imgClsBridge, Resource.������������_16�16_�������1);
            this.imageList.Add(Images.imgClsData, Resource.��_16�16_�������1);
            this.imageList.Add(Images.imgClsFact, Resource.�������_������_16�16_�������1);
            this.imageList.Add(Images.imgClsFix, Resource.�������������_16�16_�������1t);
            this.imageList.Add(Images.imgNewAssociation, Resource.��������_����������_16�16_�������1);
            this.imageList.Add(Images.imgBridgeAssociation, Resource.��������_����������_16�16_�������2);
            this.imageList.Add(Images.imgMDAssociation, Resource.��������_����������MD_16�16_�������1);
            this.imageList.Add(Images.imgNewComment, Resource.��������_�����������_16�16_�������1);
            this.imageList.Add(Images.imgNewEntity, Resource.��������_�����_16�16_�������1);
            this.imageList.Add(Images.imgNewPackage, Resource.��������_�����_16�16_�������2);
            this.imageList.Add(Images.imgStereotype, Resource.���������_16�16_�������2);
            this.imageList.Add(Images.imgAttrVisible, Resource.���������_���������_16�16_�������2);
            this.imageList.Add(Images.imgAttributes, Resource.�����_��������_16�16_�������1);
            this.imageList.Add(Images.imgShow, Resource.��������_16�16_�������1);
            this.imageList.Add(Images.imgHide, Resource.������_16�16_�������1);
            this.imageList.Add(Images.imgStandartFormatting, Resource.�����������_��������������_16�16_�������1t);
            this.imageList.Add(Images.imgFillColor, Resource.����_����_16�16_�������1);
            this.imageList.Add(Images.imgFont, Resource.�����_16�16_�������2);
            this.imageList.Add(Images.imgLineWidht, Resource.�������_�����_16�16_�������1);
            this.imageList.Add(Images.imgColorLine, Resource.����_�����_16�16_�������1);
            this.imageList.Add(Images.imgShadow, Resource.����_����_16�16_�������2);
            this.imageList.Add(Images.imgPageSetupDialod, Resource.���������_��������_16�16_�������2);
            this.imageList.Add(Images.imgPrint, Resource.������_16�16_�������1);
            this.imageList.Add(Images.imgPrintOnPage, Resource.������_��_�����_��������_16�16_�������1);
            this.imageList.Add(Images.imgEditRedo, Resource.�������������_���������_��������_16�16_�������1);
            this.imageList.Add(Images.imgEditUndo, Resource.�������������_��������_16�16_�������1);
            this.imageList.Add(Images.imgNewtable, Resource.�������_�������1);
            this.ImageList.Add(Images.imgStereotypeDefPos, Resource.����������_16�16_�������2);
        }

        #endregion
    }
}
