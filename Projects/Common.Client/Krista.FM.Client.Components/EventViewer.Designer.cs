namespace Krista.FM.Client.Components
{
    partial class EventViewer
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
            this.pnEventsViewer = new System.Windows.Forms.Panel();
            this.ugEvents = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.pgProps = new System.Windows.Forms.PropertyGrid();
            this.cbActive = new System.Windows.Forms.CheckBox();
            this.pnEventsViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugEvents)).BeginInit();
            this.SuspendLayout();
            // 
            // pnEventsViewer
            // 
            this.pnEventsViewer.Controls.Add(this.ugEvents);
            this.pnEventsViewer.Controls.Add(this.pgProps);
            this.pnEventsViewer.Controls.Add(this.cbActive);
            this.pnEventsViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnEventsViewer.Location = new System.Drawing.Point(0, 0);
            this.pnEventsViewer.Name = "pnEventsViewer";
            this.pnEventsViewer.Size = new System.Drawing.Size(674, 213);
            this.pnEventsViewer.TabIndex = 5;
            // 
            // ugEvents
            // 
            this.ugEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.ugEvents.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugEvents.DisplayLayout.AddNewBox.Prompt = "Добавить...";
            this.ugEvents.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;
            this.ugEvents.DisplayLayout.Override.RowSizingArea = Infragistics.Win.UltraWinGrid.RowSizingArea.EntireRow;
            this.ugEvents.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugEvents.Location = new System.Drawing.Point(0, 21);
            this.ugEvents.Name = "ugEvents";
            this.ugEvents.Size = new System.Drawing.Size(400, 189);
            this.ugEvents.SyncWithCurrencyManager = false;
            this.ugEvents.TabIndex = 7;
            this.ugEvents.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.ugEvents_AfterSelectChange);
            this.ugEvents.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugEvents_InitializeLayout);
            // 
            // pgProps
            // 
            this.pgProps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgProps.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pgProps.HelpVisible = false;
            this.pgProps.Location = new System.Drawing.Point(403, 21);
            this.pgProps.Margin = new System.Windows.Forms.Padding(0);
            this.pgProps.Name = "pgProps";
            this.pgProps.Size = new System.Drawing.Size(265, 189);
            this.pgProps.TabIndex = 6;
            this.pgProps.ToolbarVisible = false;
            // 
            // cbActive
            // 
            this.cbActive.AutoSize = true;
            this.cbActive.Checked = true;
            this.cbActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbActive.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbActive.Location = new System.Drawing.Point(3, 3);
            this.cbActive.Name = "cbActive";
            this.cbActive.Size = new System.Drawing.Size(149, 17);
            this.cbActive.TabIndex = 5;
            this.cbActive.Text = "Перехватывать события";
            this.cbActive.UseVisualStyleBackColor = true;
            // 
            // EventViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 213);
            this.Controls.Add(this.pnEventsViewer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EventViewer";
            this.ShowIcon = false;
            this.Text = "Просмотр событий";
            this.pnEventsViewer.ResumeLayout(false);
            this.pnEventsViewer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugEvents)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnEventsViewer;
        public Infragistics.Win.UltraWinGrid.UltraGrid ugEvents;
        private System.Windows.Forms.PropertyGrid pgProps;
        private System.Windows.Forms.CheckBox cbActive;
    }
}