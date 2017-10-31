namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    partial class FormProcessOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnAddToAccumulator = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRunProcess = new System.Windows.Forms.Button();
            this.pnlObjects = new System.Windows.Forms.Panel();
            this.gridObjects = new System.Windows.Forms.DataGridView();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemProcessFull = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProcessData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProcessIncremental = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemAddPartitions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddDimensions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.columnObjectType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnObjectId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnParentId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStateName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnLastProcessed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnRefBatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnProcessType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons.SuspendLayout();
            this.pnlObjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridObjects)).BeginInit();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnAddToAccumulator);
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Controls.Add(this.btnRunProcess);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 499);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(732, 31);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnAddToAccumulator
            // 
            this.btnAddToAccumulator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddToAccumulator.Location = new System.Drawing.Point(306, 5);
            this.btnAddToAccumulator.Name = "btnAddToAccumulator";
            this.btnAddToAccumulator.Size = new System.Drawing.Size(137, 23);
            this.btnAddToAccumulator.TabIndex = 4;
            this.btnAddToAccumulator.Text = "Добавить в накопитель";
            this.btnAddToAccumulator.UseVisualStyleBackColor = true;
            //this.btnAddToAccumulator.Click += new System.EventHandler(this.btnAddToAccumulator_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(592, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(137, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Отмена";
            this.btnClose.UseVisualStyleBackColor = true;
            //this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRunProcess
            // 
            this.btnRunProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunProcess.Location = new System.Drawing.Point(449, 5);
            this.btnRunProcess.Name = "btnRunProcess";
            this.btnRunProcess.Size = new System.Drawing.Size(137, 23);
            this.btnRunProcess.TabIndex = 0;
            this.btnRunProcess.Text = "Расчитать немедленно";
            this.btnRunProcess.UseVisualStyleBackColor = true;
            //this.btnRunProcess.Click += new System.EventHandler(this.btnRunProcess_Click);
            // 
            // pnlObjects
            // 
            this.pnlObjects.Controls.Add(this.gridObjects);
            this.pnlObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlObjects.Location = new System.Drawing.Point(0, 0);
            this.pnlObjects.Name = "pnlObjects";
            this.pnlObjects.Size = new System.Drawing.Size(732, 499);
            this.pnlObjects.TabIndex = 3;
            // 
            // gridObjects
            // 
            this.gridObjects.AllowUserToAddRows = false;
            this.gridObjects.AllowUserToResizeRows = false;
            this.gridObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridObjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnObjectType,
            this.columnObjectId,
            this.columnName,
            this.columnParentId,
            this.columnState,
            this.columnStateName,
            this.columnLastProcessed,
            this.columnRefBatch,
            this.columnProcessType});
            this.gridObjects.ContextMenuStrip = this.contextMenu;
            this.gridObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridObjects.Location = new System.Drawing.Point(0, 0);
            this.gridObjects.Name = "gridObjects";
            this.gridObjects.RowHeadersVisible = false;
            this.gridObjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridObjects.Size = new System.Drawing.Size(732, 499);
            this.gridObjects.TabIndex = 0;
            //this.gridObjects.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridObjects_RowEnter);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemProcessFull,
            this.menuItemProcessData,
            this.menuItemProcessIncremental,
            this.toolStripMenuItem1,
            this.menuItemAddPartitions,
            this.menuItemAddDimensions,
            this.toolStripSeparator1,
            this.menuItemDelete});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(267, 148);
            // 
            // menuItemProcessFull
            // 
            this.menuItemProcessFull.Name = "menuItemProcessFull";
            this.menuItemProcessFull.Size = new System.Drawing.Size(266, 22);
            this.menuItemProcessFull.Text = "Обновить структуру и данные";
            //this.menuItemProcessFull.Click += new System.EventHandler(this.menuItemProcessFull_Click);
            // 
            // menuItemProcessData
            // 
            this.menuItemProcessData.Name = "menuItemProcessData";
            this.menuItemProcessData.Size = new System.Drawing.Size(266, 22);
            this.menuItemProcessData.Text = "Обновить данные";
            //this.menuItemProcessData.Click += new System.EventHandler(this.menuItemProcessData_Click);
            // 
            // menuItemProcessIncremental
            // 
            this.menuItemProcessIncremental.Name = "menuItemProcessIncremental";
            this.menuItemProcessIncremental.Size = new System.Drawing.Size(266, 22);
            this.menuItemProcessIncremental.Text = "Добавить данные";
            //this.menuItemProcessIncremental.Click += new System.EventHandler(this.menuItemProcessIncremental_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(263, 6);
            // 
            // menuItemAddPartitions
            // 
            this.menuItemAddPartitions.Name = "menuItemAddPartitions";
            this.menuItemAddPartitions.Size = new System.Drawing.Size(266, 22);
            this.menuItemAddPartitions.Text = "Добавить другие разделы куба";
            //this.menuItemAddPartitions.Click += new System.EventHandler(this.menuItemAddPartitions_Click);
            // 
            // menuItemAddDimensions
            // 
            this.menuItemAddDimensions.Name = "menuItemAddDimensions";
            this.menuItemAddDimensions.Size = new System.Drawing.Size(266, 22);
            this.menuItemAddDimensions.Text = "Добавить используемые измерения";
            //this.menuItemAddDimensions.Click += new System.EventHandler(this.menuItemAddDimensions_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(263, 6);
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Name = "menuItemDelete";
            this.menuItemDelete.Size = new System.Drawing.Size(266, 22);
            this.menuItemDelete.Text = "Удалить выделенные объекты";
            //this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // columnObjectType
            // 
            this.columnObjectType.DataPropertyName = "ObjectType";
            this.columnObjectType.HeaderText = "Тип объекта";
            this.columnObjectType.Name = "columnObjectType";
            this.columnObjectType.ReadOnly = true;
            this.columnObjectType.Visible = false;
            // 
            // columnObjectId
            // 
            this.columnObjectId.DataPropertyName = "ObjectId";
            this.columnObjectId.HeaderText = "Идентификатор объекта";
            this.columnObjectId.Name = "columnObjectId";
            this.columnObjectId.ReadOnly = true;
            this.columnObjectId.Visible = false;
            this.columnObjectId.Width = 200;
            // 
            // columnName
            // 
            this.columnName.DataPropertyName = "ObjectName";
            this.columnName.HeaderText = "Имя";
            this.columnName.MinimumWidth = 50;
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            this.columnName.Width = 300;
            // 
            // columnParentId
            // 
            this.columnParentId.DataPropertyName = "ParentId";
            this.columnParentId.HeaderText = "Идентификатор родителя";
            this.columnParentId.MinimumWidth = 50;
            this.columnParentId.Name = "columnParentId";
            this.columnParentId.ReadOnly = true;
            this.columnParentId.Width = 300;
            // 
            // columnState
            // 
            this.columnState.DataPropertyName = "State";
            this.columnState.HeaderText = "Состояние";
            this.columnState.Name = "columnState";
            this.columnState.ReadOnly = true;
            this.columnState.Visible = false;
            // 
            // columnStateName
            // 
            this.columnStateName.DataPropertyName = "StateName";
            this.columnStateName.HeaderText = "Имя состояния";
            this.columnStateName.Name = "columnStateName";
            this.columnStateName.ReadOnly = true;
            this.columnStateName.Visible = false;
            // 
            // columnLastProcessed
            // 
            this.columnLastProcessed.DataPropertyName = "LastProcessed";
            this.columnLastProcessed.HeaderText = "Время расчета";
            this.columnLastProcessed.Name = "columnLastProcessed";
            this.columnLastProcessed.ReadOnly = true;
            this.columnLastProcessed.Visible = false;
            // 
            // columnRefBatch
            // 
            this.columnRefBatch.DataPropertyName = "RefBatch";
            this.columnRefBatch.HeaderText = "Пакет";
            this.columnRefBatch.Name = "columnRefBatch";
            this.columnRefBatch.ReadOnly = true;
            this.columnRefBatch.Visible = false;
            // 
            // columnProcessType
            // 
            this.columnProcessType.DataPropertyName = "ProcessType";
            this.columnProcessType.HeaderText = "Тип расчета";
            this.columnProcessType.Name = "columnProcessType";
            this.columnProcessType.ReadOnly = true;
            // 
            // FormProcessOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 530);
            this.Controls.Add(this.pnlObjects);
            this.Controls.Add(this.pnlButtons);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(740, 370);
            this.Name = "FormProcessOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выберите тип расчета";
            this.pnlButtons.ResumeLayout(false);
            this.pnlObjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridObjects)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel pnlObjects;
        private System.Windows.Forms.DataGridView gridObjects;
        private System.Windows.Forms.Button btnRunProcess;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem menuItemProcessFull;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItemProcessData;
        private System.Windows.Forms.ToolStripMenuItem menuItemProcessIncremental;
        private System.Windows.Forms.Button btnAddToAccumulator;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddPartitions;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddDimensions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnObjectType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnObjectId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnParentId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnState;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStateName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLastProcessed;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnRefBatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnProcessType;
    }
}