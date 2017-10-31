namespace Krista.FM.Client.iMonitoringWM.ApplicationSettings
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu;

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
            this.mainMenu = new System.Windows.Forms.MainMenu();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tapConnection = new System.Windows.Forms.TabPage();
            this.cbOffLineOperation = new System.Windows.Forms.CheckBox();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.lbHost = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lbPassword = new System.Windows.Forms.Label();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.lbLogin = new System.Windows.Forms.Label();
            this.connectionUnderLine = new System.Windows.Forms.Panel();
            this.lbConnectionCaption = new System.Windows.Forms.Label();
            this.tabCache = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.rbCacheInCard = new System.Windows.Forms.RadioButton();
            this.rbCacheInPhone = new System.Windows.Forms.RadioButton();
            this.btClearCache = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tapConnection.SuspendLayout();
            this.tabCache.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tapConnection);
            this.tabControl.Controls.Add(this.tabCache);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(240, 268);
            this.tabControl.TabIndex = 0;
            // 
            // tapConnection
            // 
            this.tapConnection.Controls.Add(this.cbOffLineOperation);
            this.tapConnection.Controls.Add(this.tbHost);
            this.tapConnection.Controls.Add(this.lbHost);
            this.tapConnection.Controls.Add(this.tbPassword);
            this.tapConnection.Controls.Add(this.lbPassword);
            this.tapConnection.Controls.Add(this.tbLogin);
            this.tapConnection.Controls.Add(this.lbLogin);
            this.tapConnection.Controls.Add(this.connectionUnderLine);
            this.tapConnection.Controls.Add(this.lbConnectionCaption);
            this.tapConnection.Location = new System.Drawing.Point(0, 0);
            this.tapConnection.Name = "tapConnection";
            this.tapConnection.Size = new System.Drawing.Size(240, 245);
            this.tapConnection.Text = "Подключение";
            // 
            // cbOffLineOperation
            // 
            this.cbOffLineOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOffLineOperation.Location = new System.Drawing.Point(13, 118);
            this.cbOffLineOperation.Name = "cbOffLineOperation";
            this.cbOffLineOperation.Size = new System.Drawing.Size(218, 20);
            this.cbOffLineOperation.TabIndex = 23;
            this.cbOffLineOperation.Text = "автономная работа";
            // 
            // tbHost
            // 
            this.tbHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHost.Location = new System.Drawing.Point(76, 91);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(152, 21);
            this.tbHost.TabIndex = 17;
            // 
            // lbHost
            // 
            this.lbHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbHost.Location = new System.Drawing.Point(15, 91);
            this.lbHost.Name = "lbHost";
            this.lbHost.Size = new System.Drawing.Size(61, 20);
            this.lbHost.Text = "Источник";
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPassword.Location = new System.Drawing.Point(76, 64);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(152, 21);
            this.tbPassword.TabIndex = 16;
            this.tbPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPassword_KeyDown);
            // 
            // lbPassword
            // 
            this.lbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPassword.Location = new System.Drawing.Point(15, 64);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(61, 20);
            this.lbPassword.Text = "Пароль";
            // 
            // tbLogin
            // 
            this.tbLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLogin.Location = new System.Drawing.Point(76, 37);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(152, 21);
            this.tbLogin.TabIndex = 15;
            // 
            // lbLogin
            // 
            this.lbLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLogin.Location = new System.Drawing.Point(15, 37);
            this.lbLogin.Name = "lbLogin";
            this.lbLogin.Size = new System.Drawing.Size(61, 20);
            this.lbLogin.Text = "Логин";
            // 
            // connectionUnderLine
            // 
            this.connectionUnderLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionUnderLine.BackColor = System.Drawing.Color.Black;
            this.connectionUnderLine.Location = new System.Drawing.Point(0, 21);
            this.connectionUnderLine.Name = "connectionUnderLine";
            this.connectionUnderLine.Size = new System.Drawing.Size(240, 1);
            // 
            // lbConnectionCaption
            // 
            this.lbConnectionCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbConnectionCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lbConnectionCaption.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.lbConnectionCaption.Location = new System.Drawing.Point(7, 2);
            this.lbConnectionCaption.Name = "lbConnectionCaption";
            this.lbConnectionCaption.Size = new System.Drawing.Size(226, 20);
            this.lbConnectionCaption.Text = "  Подключение";
            // 
            // tabCache
            // 
            this.tabCache.Controls.Add(this.label1);
            this.tabCache.Controls.Add(this.rbCacheInCard);
            this.tabCache.Controls.Add(this.rbCacheInPhone);
            this.tabCache.Controls.Add(this.btClearCache);
            this.tabCache.Controls.Add(this.panel2);
            this.tabCache.Controls.Add(this.label5);
            this.tabCache.Controls.Add(this.button1);
            this.tabCache.Location = new System.Drawing.Point(0, 0);
            this.tabCache.Name = "tabCache";
            this.tabCache.Size = new System.Drawing.Size(240, 245);
            this.tabCache.Text = "Кэш";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(22, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 20);
            this.label1.Text = " Размещение";
            // 
            // rbCacheInCard
            // 
            this.rbCacheInCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rbCacheInCard.Checked = true;
            this.rbCacheInCard.Location = new System.Drawing.Point(24, 80);
            this.rbCacheInCard.Name = "rbCacheInCard";
            this.rbCacheInCard.Size = new System.Drawing.Size(130, 20);
            this.rbCacheInCard.TabIndex = 10;
            this.rbCacheInCard.Text = "На карте памяти";
            this.rbCacheInCard.CheckedChanged += new System.EventHandler(this.rbCacheInCard_CheckedChanged);
            // 
            // rbCacheInPhone
            // 
            this.rbCacheInPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rbCacheInPhone.Location = new System.Drawing.Point(24, 54);
            this.rbCacheInPhone.Name = "rbCacheInPhone";
            this.rbCacheInPhone.Size = new System.Drawing.Size(115, 20);
            this.rbCacheInPhone.TabIndex = 9;
            this.rbCacheInPhone.TabStop = false;
            this.rbCacheInPhone.Text = "В устройстве";
            this.rbCacheInPhone.CheckedChanged += new System.EventHandler(this.rbCacheInPhone_CheckedChanged);
            // 
            // btClearCache
            // 
            this.btClearCache.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btClearCache.Location = new System.Drawing.Point(11, 115);
            this.btClearCache.Name = "btClearCache";
            this.btClearCache.Size = new System.Drawing.Size(208, 20);
            this.btClearCache.TabIndex = 6;
            this.btClearCache.Text = "Очистить";
            this.btClearCache.Click += new System.EventHandler(this.btClearCache_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(0, 21);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 1);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.label5.Location = new System.Drawing.Point(7, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(226, 20);
            this.label5.Text = "  Кэш";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(11, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(156, 70);
            this.button1.TabIndex = 13;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.tabControl);
            this.Menu = this.mainMenu;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Настройки";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.SettingsForm_Closing);
            this.tabControl.ResumeLayout(false);
            this.tapConnection.ResumeLayout(false);
            this.tabCache.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tapConnection;
        private System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.Label lbHost;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.Label lbLogin;
        private System.Windows.Forms.Panel connectionUnderLine;
        private System.Windows.Forms.Label lbConnectionCaption;
        private System.Windows.Forms.TabPage tabCache;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbOffLineOperation;
        private System.Windows.Forms.Button btClearCache;
        private System.Windows.Forms.RadioButton rbCacheInCard;
        private System.Windows.Forms.RadioButton rbCacheInPhone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;




    }
}

