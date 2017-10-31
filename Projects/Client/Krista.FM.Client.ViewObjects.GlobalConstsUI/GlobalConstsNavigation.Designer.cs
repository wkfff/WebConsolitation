namespace Krista.FM.Client.ViewObjects.GlobalConstsUI
{
    partial class GlobalConstsNavigation
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.uebNavi = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            ((System.ComponentModel.ISupportInitialize)(this.uebNavi)).BeginInit();
            this.SuspendLayout();
            // 
            // uebNavi
            // 
            this.uebNavi.AcceptsFocus = Infragistics.Win.DefaultableBoolean.True;
            this.uebNavi.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uebNavi.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarItem1.Key = "976E81D7-AEA2-4e66-A583-9121B77D1324";
            appearance1.Image = global::Krista.FM.Client.ViewObjects.GlobalConstsUI.Properties.Resources.configConsts;
            ultraExplorerBarItem1.Settings.AppearancesSmall.Appearance = appearance1;
            ultraExplorerBarItem1.Text = "Конфигурационные";
            ultraExplorerBarItem2.Key = "1F0EB52E-5A59-4a3c-BD58-0D5F02D9A235";
            ultraExplorerBarItem2.Text = "Настраиваемые";
            ultraExplorerBarItem3.Key = "7B0506E4-5A8C-4c05-BC36-5A4FF0A3B471";
            appearance2.Image = global::Krista.FM.Client.ViewObjects.GlobalConstsUI.Properties.Resources.userAccounts;
            ultraExplorerBarItem3.Settings.AppearancesSmall.Appearance = appearance2;
            ultraExplorerBarItem3.Text = "Пользовательские";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1,
            ultraExplorerBarItem2,
            ultraExplorerBarItem3});
            ultraExplorerBarGroup1.Text = "New Group";
            this.uebNavi.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
            this.uebNavi.GroupSettings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.GroupSettings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.GroupSettings.BorderStyleItemArea = Infragistics.Win.UIElementBorderStyle.None;
            this.uebNavi.GroupSettings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uebNavi.ItemSettings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.uebNavi.ItemSettings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.uebNavi.Location = new System.Drawing.Point(0, 0);
            this.uebNavi.Name = "uebNavi";
            this.uebNavi.ShowDefaultContextMenu = false;
            this.uebNavi.Size = new System.Drawing.Size(216, 472);
            this.uebNavi.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.uebNavi.TabIndex = 1;
            this.uebNavi.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // GlobalConstsNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uebNavi);
            this.Name = "GlobalConstsNavigation";
            ((System.ComponentModel.ISupportInitialize)(this.uebNavi)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar uebNavi;
    }
}
