using System;
using System.Drawing;
using System.Configuration;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Security.Principal;

using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;

using Krista.FM.Common;
using Krista.FM.Common.RegistryUtils;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.Common.Forms
{
	/// <summary>
	/// Summary description for FormLogon.
	/// </summary>
	public class frmLogon : System.Windows.Forms.Form
	{
        private UltraLabel laServerName;
        private UltraLabel laSchemeName;
        private UltraLabel laLogin;
        private UltraLabel laPassword;
		private UltraButton btnOK;
        private UltraButton btnCancel;
        private Label laLogo;
        private CheckBox cbWindowsAuth;
        private System.Windows.Forms.Timer CheckShemeTimer;
        private IContainer components;
        private UltraTextEditor edLogin;
        private UltraTextEditor edPassword;
        private ComboBox edServerName;
        private UltraTextEditor cbScheme;
        private static Mutex mutexCheckSheme = new Mutex();
	    private static string currentServerName;


		public frmLogon()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogon));
            this.btnOK = new UltraButton();
            this.btnCancel = new UltraButton();
            this.laServerName = new UltraLabel();
            this.laSchemeName = new UltraLabel();
            this.laLogin = new UltraLabel();
            this.laPassword = new UltraLabel();
            this.laLogo = new Label();
            this.cbWindowsAuth = new CheckBox();
            this.CheckShemeTimer = new System.Windows.Forms.Timer(this.components);
            this.edLogin = new UltraTextEditor();
            this.edPassword = new UltraTextEditor();
            this.edServerName = new System.Windows.Forms.ComboBox();
            this.cbScheme = new UltraTextEditor();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(93, 201);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(106, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Подключиться";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(205, 201);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Нет";
            // 
            // laServerName
            // 
            this.laServerName.Location = new System.Drawing.Point(15, 71);
            this.laServerName.Name = "laServerName";
            this.laServerName.Size = new System.Drawing.Size(73, 21);
            this.laServerName.TabIndex = 2;
            this.laServerName.Text = "Сервер";
            this.laServerName.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            // 
            // laSchemeName
            // 
            this.laSchemeName.Location = new System.Drawing.Point(12, 98);
            this.laSchemeName.Name = "laSchemeName";
            this.laSchemeName.Size = new System.Drawing.Size(75, 20);
            this.laSchemeName.TabIndex = 3;
            this.laSchemeName.Text = "Схема";
		    this.laSchemeName.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            // 
            // laLogin
            // 
            this.laLogin.Location = new System.Drawing.Point(12, 146);
            this.laLogin.Name = "laLogin";
            this.laLogin.Size = new System.Drawing.Size(76, 20);
            this.laLogin.TabIndex = 4;
            this.laLogin.Text = "Логин";
            this.laLogin.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            // 
            // laPassword
            // 
            this.laPassword.Location = new System.Drawing.Point(12, 171);
            this.laPassword.Name = "laPassword";
            this.laPassword.Size = new System.Drawing.Size(76, 20);
            this.laPassword.TabIndex = 5;
            this.laPassword.Text = "Пароль";
            this.laPassword.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            // 
            // laLogo
            // 
            this.laLogo.BackColor = System.Drawing.Color.Transparent;
            this.laLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.laLogo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.laLogo.Image = ((System.Drawing.Image)(resources.GetObject("laLogo.Image")));
            this.laLogo.Location = new System.Drawing.Point(0, 0);
            this.laLogo.Margin = new System.Windows.Forms.Padding(0);
            this.laLogo.Name = "laLogo";
            this.laLogo.Size = new System.Drawing.Size(320, 60);
            this.laLogo.TabIndex = 12;
            this.laLogo.Text = "   Задайте параметры  подключения к системе";
		    this.laLogo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.laLogo.UseCompatibleTextRendering = true;
            // 
            // cbWindowsAuth
            // 
            this.cbWindowsAuth.Location = new System.Drawing.Point(94, 121);
            this.cbWindowsAuth.Name = "cbWindowsAuth";
            this.cbWindowsAuth.Size = new System.Drawing.Size(213, 22);
            this.cbWindowsAuth.TabIndex = 15;
            this.cbWindowsAuth.Text = "Аутентификация Windows";
            //this.cbWindowsAuth.UseVisualStyleBackColor = true;
            this.cbWindowsAuth.CheckedChanged += new System.EventHandler(this.cbWindowsAuth_CheckedChanged);
            // 
            // CheckShemeTimer
            // 
            this.CheckShemeTimer.Interval = 3000;
            this.CheckShemeTimer.Tick += new System.EventHandler(this.CheckShemeTimer_Tick);
            // 
            // edLogin
            // 
            this.edLogin.Location = new System.Drawing.Point(93, 146);
            this.edLogin.Name = "edLogin";
            this.edLogin.Size = new System.Drawing.Size(217, 20);
            this.edLogin.TabIndex = 21;
            // 
            // edPassword
            // 
            this.edPassword.Location = new System.Drawing.Point(93, 171);
            this.edPassword.Name = "edPassword";
            this.edPassword.PasswordChar = '*';
            this.edPassword.Size = new System.Drawing.Size(217, 20);
            this.edPassword.TabIndex = 22;
            // 
            // edServerName
            // 
            this.edServerName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.edServerName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.edServerName.FormattingEnabled = true;
            this.edServerName.Location = new System.Drawing.Point(93, 71);
            this.edServerName.Name = "edServerName";
            this.edServerName.Size = new System.Drawing.Size(217, 21);
            this.edServerName.TabIndex = 23;
            this.edServerName.TextChanged += new System.EventHandler(this.edServerName_TextChanged);
            // 
            // cbScheme
            // 
            this.cbScheme.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cbScheme.ForeColor = System.Drawing.Color.Black;
            this.cbScheme.Location = new System.Drawing.Point(93, 98);
            this.cbScheme.Name = "cbScheme";
            this.cbScheme.ReadOnly = true;
            this.cbScheme.Size = new System.Drawing.Size(217, 20);
            this.cbScheme.TabIndex = 24;
            // 
            // frmLogon
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(320, 231);
            this.Controls.Add(this.cbScheme);
            this.Controls.Add(this.edServerName);
            this.Controls.Add(this.edPassword);
            this.Controls.Add(this.edLogin);
            this.Controls.Add(this.cbWindowsAuth);
            this.Controls.Add(this.laLogo);
            this.Controls.Add(this.laPassword);
            this.Controls.Add(this.laLogin);
            this.Controls.Add(this.laSchemeName);
            this.Controls.Add(this.laServerName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogon";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Вход в систему";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmLogon_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmLogon_KeyDown);
            this.Load += new System.EventHandler(this.frmLogon_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private const string LOGIN_KEY = "Login=";
        
        private AuthenticationType AuthType
        {
            get
            {
                if (cbWindowsAuth.Checked)
                    return AuthenticationType.atWindows;
                else
                    return AuthenticationType.adPwdSHA512;
            }
            set
            {
                bool isWindowsAuth = value == AuthenticationType.atWindows;
                cbWindowsAuth.Checked = isWindowsAuth;
                edLogin.Enabled = !isWindowsAuth;
                edPassword.Enabled = !isWindowsAuth;
                if (isWindowsAuth)
                {
                    string login = WindowsIdentity.GetCurrent().Name;
                    // проверяем ключик золотой
                    string logAs = String.Empty;
                    if ((CommandLineUtils.ParameterPresent(LOGIN_KEY, out logAs)) && (!String.IsNullOrEmpty(logAs)))
                    {
                        login = logAs;
                    }
                    edLogin.Text = login;
                    edPassword.Text = String.Empty;
                }
            }
        }

        #region Загрузка/сохранение параметров в реестре
        private void LoadSettingsFromRegistry()
		{
			Utils RegUtils = new Utils(GetType(), true);
			edServerName.Text = string.IsNullOrEmpty(currentServerName) ? RegUtils.GetKeyValue("edServerName") : currentServerName;
			cbScheme.Text = RegUtils.GetKeyValue("edSchemeName");
			edLogin.Text = RegUtils.GetKeyValue("edLogin");
            string port = System.Configuration.ConfigurationManager.AppSettings["ServerPort"];
            string authTypeStr = RegUtils.GetKeyValue("authType");
            if (!String.IsNullOrEmpty(authTypeStr))
            {
                AuthType = (AuthenticationType)Convert.ToInt32(authTypeStr);
            }
            else
            {
                AuthType = AuthenticationType.atWindows;
            }
            GetAddressList();
		}

		private void SaveSettingsToRegistry()
		{
			Utils RegUtils = new Utils(GetType(), true);
			RegUtils.SetKeyValue("edServerName", MakeServerName(edServerName.Text));
            RegUtils.SetKeyValue("edSchemeName", cbScheme.Text);
		    RegUtils.SetKeyValue("edLogin", edLogin.Text);
            RegUtils.SetKeyValue("authType", ((int)AuthType).ToString());
            RegUtils.SetKeyValue("addressList", MakeAddressList(RegUtils.GetKeyValue("addressList")));
        }
        #endregion

        public static bool ShowLogonForm(ref string serverName, ref string schemeName,
            ref string login, ref string password, ref IScheme proxyScheme, ref bool differentVersionMode, ref AuthenticationType authType, ref string connectionErrorMessage)
		{
            frmLogon tmpForm = new frmLogon();
			tmpForm.LoadSettingsFromRegistry();

			IServer proxyServer = null;
            differentVersionMode = false;
            authType = AuthenticationType.atUndefined;

            DialogResult dialogResult = tmpForm.ShowDialog();
            switch (dialogResult)
            {
                case DialogResult.OK:
                    {
                        currentServerName = tmpForm.edServerName.Text;
                        Utils RegUtils = new Utils(tmpForm.GetType(), true);
                        RegUtils.SetKeyValue("currentServerName", tmpForm.cbScheme.Text);
                        authType = tmpForm.AuthType;
                        login = tmpForm.edLogin.Text;

                        if (tmpForm.MakeServerName(tmpForm.edServerName.Text) != String.Empty)
                        {   
                            proxyServer = Krista.FM.Client.Common.ClientConnectionHelper.ConnectToServer(
                                tmpForm.MakeServerName(tmpForm.edServerName.Text),
                                ConfigurationManager.AppSettings["ServerPort"],
                                ConfigurationManager.AppSettings["ServerAddress"],
                                login
                            );
                        }

                        try
                        {
                            if (proxyServer != null)
                            {
                                string errStr = String.Empty;
                                ClientSession clientSession = ClientSession.CreateSession(SessionClientType.WindowsNetClient);
                                // проверка версий общей сборки
                                string clientServerLibraryVersion = String.Empty;
                                if (!AppVersionControl.IgnoreVersionsModeOn())
                                    clientServerLibraryVersion = AppVersionControl.GetServerLibraryVersion();
                                // ***
                                proxyServer.Connect(tmpForm.cbScheme.Text, out proxyScheme, tmpForm.AuthType, 
                                    login, PwdHelper.GetPasswordHash(tmpForm.edPassword.Text), ref errStr, clientServerLibraryVersion);

                                if (!String.IsNullOrEmpty(errStr))
                                {
                                    if (errStr.Contains("Обнаружено различие версий общей сборки"))
                                    {
                                        differentVersionMode = true;
                                        connectionErrorMessage = errStr.Replace("Подключение невозможно", "Если вы хотите обновиться на новую версию, нажмите ОК. Если нет, то нажмите Отмена.");
                                        proxyServer.Connect(tmpForm.cbScheme.Text, out proxyScheme, tmpForm.AuthType,
                                            login, PwdHelper.GetPasswordHash(tmpForm.edPassword.Text), ref errStr);
                                    }

                                    if (!String.IsNullOrEmpty(errStr))
                                    {
                                        MessageBox.Show(errStr, "Ошибка подключения", MessageBoxButtons.OK,
                                                        MessageBoxIcon.Error);
                                        return true;
                                    }
                                }

                                clientSession.SessionManager = proxyScheme.SessionManager;
                                tmpForm.SaveSettingsToRegistry();
                                if (proxyScheme.MultiServerMode)
                                {
                                    MessageBox.Show(
                                        "Сервер работает в режиме MultiServerMode (запущено несколько серверов). Некоторые возможности, такие как автоматический расчет кубов и авто-обновление схемы, будут запрещены.", 
                                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                                // Для того, чтобы из задачи узнать тип аутентификации пользователя,
                                // сохраним его в контексте вызова
                                LogicalCallContextData lccd = LogicalCallContextData.GetContext();
                                lccd["AuthType"] = authType;

                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        return true;
                    }
                case DialogResult.Ignore:
                    {
                        differentVersionMode = true;
                        return true;
                    }
                default:
                    return false;
            }
		}

        private void frmLogon_Load(object sender, EventArgs e)
        {
            this.SetCbSchemeText("Определяется...", Color.Black);

            // Убрано перед выпуском версии
            //Infragistics.Win.AppStyling.StyleManager.Load(@".\Styles\Office 2007 (голубая).isl");
        }
				
		/// <summary>
        /// Проверяет, доступна ли схема.
        /// </summary>
        private void CheckSheme(object serverName)
        {
            //mutexCheckSheme.WaitOne();
            IServer proxyServer = null;
            if (MakeServerName(serverName.ToString()) != String.Empty)
            {
                try
                {
                    proxyServer = Krista.FM.Client.Common.ClientConnectionHelper.ConnectToServer(MakeServerName(serverName.ToString()));
                    if (proxyServer != null)
                    {
                        foreach (string item in proxyServer.SchemeList)
                        {
                            this.SetCbSchemeText(item, Color.Black);
                        }
						if ((threadCheckSheme != null) && (threadCheckSheme == Thread.CurrentThread))
						{
							SetBtnOkEnable(true);
						}
                    }
                    else
                    {
						if ((threadCheckSheme != null) && (threadCheckSheme == Thread.CurrentThread))
						{
							this.SetCbSchemeText("Сервер не доступен", Color.Red);
							SetBtnOkEnable(false);
						}
                    }
                }
                catch (UriFormatException)
                {
                    SetCbSchemeText("Ошибка при вводе адреса", Color.Red);
					if ((threadCheckSheme != null) && (threadCheckSheme == Thread.CurrentThread))
					{
						SetBtnOkEnable(false);
					}
                }
            }
            //mutexCheckSheme.ReleaseMutex();
        }

        delegate void SetCbSchemeTextCallback(string text, Color colorText);

        private void SetCbSchemeText(string text, Color colorText)
        {
			try
			{
				if (this.cbScheme.InvokeRequired)
				{
					SetCbSchemeTextCallback d = new SetCbSchemeTextCallback(SetCbSchemeText);
					this.Invoke(d, new object[] { text, colorText });
				}
				else
				{
					this.cbScheme.ForeColor = colorText;
					this.cbScheme.Text = text;
				}
			}
			catch (Exception e)
			{
			}
        }

		delegate void SetBtnOkEnableCallback(Boolean enable);

	    private void SetBtnOkEnable(Boolean enable)
        {
			try
			{
				if (this.btnOK.InvokeRequired)
				{
					SetBtnOkEnableCallback d = new SetBtnOkEnableCallback(SetBtnOkEnable);
					this.Invoke(d, enable);
				}
				else
					btnOK.Enabled = enable;
				
			}
			catch(Exception e)
			{
			}
        }

        bool shift = false;
        bool ctrl = false;
        bool alt = false;

        private void frmLogon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                ctrl = true;

            if (e.Shift)
                shift = true;

            if (e.Alt)
                alt = true;

            if (ctrl && shift && alt)
            {
                btnCancel.DialogResult = DialogResult.Ignore;
                btnOK.Text = "[ Подключиться ]";
            }
        }

        private void frmLogon_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                ctrl = false;

            if (e.KeyCode == Keys.ShiftKey)
                shift = false;

            if (e.KeyValue == 18)
                alt = false;

            if (!ctrl || !shift || !alt)
            {
                btnCancel.DialogResult = DialogResult.Cancel;
                btnOK.Text = "Подключиться";
            }
        }

        private void cbWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWindowsAuth.Checked)
                AuthType = AuthenticationType.atWindows;
            else 
                AuthType = AuthenticationType.adPwdSHA512;
        }

		//Поток отвечающий за проверку схемы
		Thread threadCheckSheme;

        /// <summary>
        /// По истечении таумаута проверяет доступность схемы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckShemeTimer_Tick(object sender, EventArgs e)
        {
            threadCheckSheme = new Thread(new ParameterizedThreadStart(CheckSheme));
            threadCheckSheme.Start(edServerName.Text);
            CheckShemeTimer.Stop();
        }     

        /// <summary>
        /// При изменении имени схемы перезапускает таймер.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edServerName_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = true;
            this.SetCbSchemeText("Определяется...", Color.Black);
			
			//Если уже запущен поток проверки схемы прерываем его.
			if (threadCheckSheme != null)
			{				
				threadCheckSheme.Abort();
				threadCheckSheme = null;
			}

			CheckShemeTimer.Stop();
			CheckShemeTimer.Start();
        }

        /// <summary>
        /// Формируем строку адресов для записи в реестр.
        /// </summary>
        /// <param name="addressChain">Адреса, уже имеющиеся в реестре.</param>
        /// <returns>Строка для записи</returns>
        private string MakeAddressList(string addressChain)
        {
            // берем текущий адрес: сервер и порт (если есть).
            string serverName = MakeServerName(edServerName.Text);
            // Если схема определилась, то дописываем ее.
            if (!string.IsNullOrEmpty(cbScheme.Text) 
                    && cbScheme.Text != "Сервер не доступен" 
                    && cbScheme.Text != "Ошибка при вводе адреса"
                    && cbScheme.Text != "Определяется...")
            {
                serverName += " (" + cbScheme.Text + ")";
            }
            // берем список из реестра и проверяем вхождение в него текущего.
            string[] addressList = addressChain.Split(';');
            for (int i = 0; i < addressList.Length; i++)
            {
                // Если текущий уже есть списке, но без схемы, а у нас он со схемой, то заменяем его
                Regex regExp = new Regex(string.Format("{0} (.*)", addressList[i]));
                if (regExp.Match(serverName).Success)
                {
                    addressList[i] = serverName;
                    return MakeAddressChain(addressList);
                }
                // Если с списке текущий со схемой, а у нас без, или он входит точно так же, не меняем ничего.
                regExp = new Regex(string.Format("{0} (.*)", serverName));
                if (regExp.Match(addressList[i]).Success || String.Equals(serverName, addressList[i]))
                {
                    return MakeAddressChain(addressList);
                }
            }
            // Если его нет никак, то добавляем.
            return string.Format("{0}{1};", addressChain, serverName);
        }

        private string MakeAddressChain(string[] addressList)
        {
            string addressChain = string.Empty;
            // Т.к. последний символ в цепочке адресов ";", то последний элемент в массиве пустая строка.
            // Не учитываем ее.
            for (int i = 0; i < addressList.Length - 1; i++)
            {
                addressChain += string.Format("{0};", addressList[i]);
            }
            return addressChain;
        }

        /// <summary>
        /// Получаем из реестра адреса и заполняем список.
        /// </summary>
        private void GetAddressList()
        {
            Utils RegUtils = new Utils(GetType(), true);
            string addressChain = RegUtils.GetKeyValue("addressList");
            string[] addressList = addressChain.Split(';');
            foreach (string item in addressList)
            {
                if (!string.IsNullOrEmpty(item))
                edServerName.Items.Add(item);
            }            
        }

        /// <summary>
        /// Вырезает имя сервера и проверяет правильность порта.
        /// </summary>
        /// <param name="inStr">Строка, из которй вырезаем.</param>
        /// <returns>Имя сервера, с портом, если он есть и правильный.</returns>
        private string MakeServerName(string inStr)
        {
            inStr = inStr.Trim();
            try
            {
            // Отрезаем имя сервера.
            string[] serverName = inStr.Split(':');
            // Отрезаем номер порта.
            string[] portNumber = serverName[1].Split();
                // Проверям, является ли порт числом
                bool digit = true;
                foreach (char item in portNumber[0])
                {
                    if (!char.IsDigit(item))
                    {
                        digit = false;
                    }
                }
                // Если порт число, то возвращаем сервер с портом.
                if (digit && portNumber[0].Length <= 4 && !String.IsNullOrEmpty(portNumber[0]) && !serverName[0].Contains(" "))
                {
                    return serverName[0] + ":" + portNumber[0];
                }
                else // Если порт некорректный, меняем лейбл и возвращаем пустую строку.
                {
                    SetCbSchemeText("Ошибка при вводе адреса", Color.Red);
                    return String.Empty;
                }
            }
            catch (IndexOutOfRangeException) // Если попали сюда, то номер порта не введен.
            {
                string[] serverName = inStr.Split();
                // Возвращаем только имя сервера.
                return serverName[0];
            }
        }

	}
}
