namespace Krista.FM.Client.DiagramEditor.Tools
{
    /// <summary>
    /// Управление зумом
    /// </summary>
    public partial class ZoomControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Infragistics.Win.UltraWinEditors.UltraComboEditor comboZoom;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance("100%", 8607187);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance("75%", 8609711);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance("50%", 8610311);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance("25%", 8610972);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance("Fit", 8611483);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            this.comboZoom = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.comboZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // ComboZoom
            // 
            this.comboZoom.Appearances.Add(appearance1);
            this.comboZoom.Appearances.Add(appearance2);
            this.comboZoom.Appearances.Add(appearance3);
            this.comboZoom.Appearances.Add(appearance4);
            this.comboZoom.Appearances.Add(appearance5);
            this.comboZoom.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.VisualStudio2005;
            this.comboZoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            valueListItem1.DataValue = "200";
            valueListItem1.DisplayText = "200%";
            valueListItem2.DataValue = "150";
            valueListItem2.DisplayText = "150%";
            valueListItem3.DataValue = "100";
            valueListItem3.DisplayText = "100%";
            valueListItem4.DataValue = "75";
            valueListItem4.DisplayText = "75%";
            valueListItem5.DataValue = "50";
            valueListItem5.DisplayText = "50%";
            valueListItem6.DataValue = "25";
            valueListItem6.DisplayText = "25%";
            valueListItem7.DataValue = "Fit";
            valueListItem7.DisplayText = "Вся диаграмма";
            this.comboZoom.Items.Add(valueListItem1);
            this.comboZoom.Items.Add(valueListItem2);
            this.comboZoom.Items.Add(valueListItem3);
            this.comboZoom.Items.Add(valueListItem4);
            this.comboZoom.Items.Add(valueListItem5);
            this.comboZoom.Items.Add(valueListItem6);
            this.comboZoom.Items.Add(valueListItem7);
            this.comboZoom.Location = new System.Drawing.Point(0, 0);
            this.comboZoom.Margin = new System.Windows.Forms.Padding(1);
            this.comboZoom.Name = "\x441omboZoom";
            this.comboZoom.Size = new System.Drawing.Size(94, 21);
            this.comboZoom.TabIndex = 0;
            // 
            // ZoomControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboZoom);
            this.Name = "ZoomControl";
            this.Size = new System.Drawing.Size(94, 21);
            ((System.ComponentModel.ISupportInitialize)(this.comboZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
