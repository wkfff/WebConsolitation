using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinToolbars;

using Itenso.Configuration;
using Krista.FM.Client.Components;
using Krista.FM.Common;
using Krista.FM.Common.RegistryUtils;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.Workplace.Services;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.Workplace
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
    public partial class Workplace : Form, IWorkbench
	{
        private System.ComponentModel.IContainer components;

        private IScheme activeScheme;

        /// <summary>
        /// Режим работы с различными версиями клиента и сервера.
        /// </summary>
        private bool differentVersionsMode;

        /// <summary>
        /// Текст ошибки при подключение к серверу, при которой продолжаем работу.
        /// </summary>
	    private string connectionErrorMessage;

        private Progress _Progress;
        private Operation _Operation;
        
		private IModalClsManager _ClsManager;
	    private ITimedMessageManager timedMessageManager;
		
		private List<BaseNavigationCtrl> addins;
	    private IWorkplaceLayout layout;

        #region - UI Controls -

	    private PaneDescriptor explorerPane;

		internal PaneDescriptor ExplorerPane
		{
			get { return explorerPane; }
		}

        #endregion - UI Controls -

        #region - Конструктор / Деструктор - 

        /// <summary>
		/// Конструктор класса
		/// </summary>
		public Workplace()
		{
            explorerPane = new PaneDescriptor(typeof(ExplorerPane), "Область навигации", "ExplorerPaneIcon");

			// Используется для сохранения настроек формы
			FormSettings formSettings = new FormSettings(this);
		}

	    private UltraExplorerBarEx NavigationControl
	    {
            get { return (UltraExplorerBarEx)explorerPane.PadContent.Control; }
	    }

		/// <summary>
		/// Очистка ресурсов, вызывается при выгрузке
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				Trace.TraceVerbose("Workplace.Dispose");
				Trace.Indent();

                if (DockManagerService.Control != null)
                {
                    DockManagerService.Control.Dispose();
                }

                // для всех объектов просмотра вызываем метод очистки ресурсов
				for (int i = 0; i <= Addins.Count - 1; i++)
				{
                    BaseNavigationCtrl ViewObj = (BaseNavigationCtrl)Addins[i];
                    try
                    {
                        ViewObj.Dispose();
                    }
                    catch { }
                    
                    // TODO: Сделать выгрузку объектов просмотра
                    /*BaseViewObj ViewObj = (BaseViewObj)addins[i];
					try
					{
						ViewObj.InternalFinalize();
					}
					catch { }*/
				}

				// очищаем их список
				Addins.Clear();

				try
				{
					if (ActiveScheme != null)
						DisconnectServer();
                }
				catch
				{
				}

				// если был создан прогресс - уничтожаем
				if (_Progress != null)
				{
					_Progress.ReleaseThread();
					_Progress = null;
				}
				// если был создан объект для индикации операций - уничтожаем
				if (_Operation != null)
				{
					_Operation.ReleaseThread();
					_Operation = null;
				}

				if (_ClsManager != null)
				{
					_ClsManager.Clear();
					_ClsManager = null;
				}

                if (components != null) components.Dispose();

                UpdateFrameworkLibraryFactory.InvokeMethod("Dispose");

				Trace.Unindent();
			}
			base.Dispose( disposing );
        }

        #endregion - Конструктор / Деструктор -

        #region - Реализация интерфейса IWorkplace -

        private void WriteConnectEvent(IScheme scheme, bool connect)
        {
            if (scheme == null)
                return;
            IUsersOperationProtocol protocol = null;
            try
            {
                protocol = (IUsersOperationProtocol)scheme.GetProtocol("Workplace.exe");
                string connectedState = "Подключение";
                UsersOperationEventKind kind = UsersOperationEventKind.uoeUserConnectToScheme;
                if (!connect)
                {
                    connectedState = "Отключение";
                    kind = UsersOperationEventKind.uoeUserDisconnectFromScheme;
                }
                string msgText = String.Format(
                    "{0} {1}.{2}",
                    connectedState,
                    scheme.Server.Machine,
                    scheme.Name);
                protocol.WriteEventIntoUsersOperationProtocol(kind, msgText, SystemInformation.ComputerName);
            }
            catch
            {
                // сервер отвалился, ничего зопесать не получится
            }
            finally
            {
                if (protocol != null)
                    protocol.Dispose();
            }
        }

        public bool IsDeveloperMode
        {
            get { return ClientSession.IsDeveloper; }
        }

		private void SetActiveScheme(IScheme scheme)
		{
			if (scheme != null)
			{
				StatusBarService.SetServerName(String.Format("Сервер: {0}:{1}", scheme.Server.Machine, scheme.Server.GetConfigurationParameter("ServerPort")));
				StatusBarService.SetSchemeName(String.Format("Схема: {0}", scheme.Name));
				StatusBarService.SetUserName(String.Format("Пользователь: {0}", scheme.UsersManager.GetCurrentUserName()));

				this.Text = scheme.Server.GetConfigurationParameter("ProductName");

				if (IsDeveloperMode)
					this.Text = this.Text + " [Режим разработчика]";

				if (Convert.ToBoolean(LogicalCallContextData.GetContext()["IgnoreVersions"]))
					this.Text += " [Без контроля версий]";

				WriteConnectEvent(scheme, true);
			}
			activeScheme = scheme;
			UnhandledExceptionHandler.Scheme = scheme;
		}

		public IScheme ActiveScheme 
		{
			[DebuggerStepThrough]
			get { return activeScheme; }
		}

		private void DisconnectServer()
		{
			Trace.TraceInformation("Отключение от сервера");
			Trace.Indent();
			
			StatusBarService.SetServerName("Сервер: ...");
			StatusBarService.SetSchemeName("Схема: ...");
			StatusBarService.SetUserName("Пользователь: ...");

			WriteConnectEvent(activeScheme, false);

			ActiveScheme.SessionManager.Sessions[ClientSession.SessionId].Dispose();

			activeScheme = null;

			ClientSession.Instance.Close();

			Trace.Unindent();
		}

		public Progress ProgressObj
		{
			[DebuggerStepThrough]
			get
			{
                if (_Progress == null)
                    _Progress = new Progress();
				return _Progress;
			}
		}

		public Operation OperationObj
		{
			[DebuggerStepThrough]
			get
			{
                if (_Operation == null)
                    _Operation = new Operation();
			    return _Operation;
			}
		}

		public UltraToolbarsManager MainToolbar 
		{
			get
			{
				return ToolbarService.Control;
			}
		}

        public UltraStatusBar MainStatusBar
        {
            get
            {
                return (UltraStatusBar)StatusBarService.Control;
            }
        }


        public void SetAdministrationNavigation(IAdministrationNavigation iAdminNavi)
        {
            if (this.iAdminNavi == null)
                this.iAdminNavi = iAdminNavi;
        }
        IAdministrationNavigation iAdminNavi = null;

        IProtocolNavigation iProtocolNavi = null;

        IBaseClsNavigation iClsNavi = null;

        
		public IInplaceProtocolView ProtocolsInplacer 
		{
			get
			{
                if (iProtocolNavi == null)
                    iProtocolNavi = ((IServiceProvider)this).GetService(typeof(IProtocolNavigation)) as IProtocolNavigation;
                if (iProtocolNavi == null)
                    return null;
                return iProtocolNavi.ProtocolsInplacer();
			}
		}
        
        private const string clsAssemblyName = "Krista.FM.Client.ViewObjects.AssociatedCLSUI";
        private const string clsManagerTypeName = "Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls.ModalClsManager";

        public IModalClsManager ClsManager
        {
            get
            {
                if (_ClsManager == null)
                {
                    Assembly clsAssembly = null;
                    try
                    {
                        clsAssembly = Assembly.Load(clsAssemblyName);
                        if (clsAssembly != null)
                        {
                            Type clsManagerType =
                                clsAssembly.GetType(clsManagerTypeName);
                            object clsManager = Activator.CreateInstance(clsManagerType, this);
                            if (clsManager != null)
                                _ClsManager = (IModalClsManager) clsManager;
                        }
                    }
                    catch
                    {
                        // тут надо вставить обработку ошибки. 
                        // пока ничего не будет, так как сборка и тип всегда находятся на своем месте
                    }
                }
                return _ClsManager;
            }
        }

        private const string messagesAssemblyName = "Krista.FM.Client.ViewObjects.MessagesUI";
        private const string messagesclsManagerTypeName = "Krista.FM.Client.ViewObjects.MessagesUI.TimedMessageManager";

	    public ITimedMessageManager TimedMessageManager
	    {
	        get
	        {
                if (timedMessageManager == null)
                {
                    Assembly clsAssembly = null;
                    try
                    {
                        clsAssembly = Assembly.Load(messagesAssemblyName);
                        if (clsAssembly != null)
                        {
                            Type clsManagerType =
                                clsAssembly.GetType(messagesclsManagerTypeName);
                            object timedManager = Activator.CreateInstance(clsManagerType,
                                                                         new SchemeMessageFactory(activeScheme,
                                                                                                  activeScheme.
                                                                                                      UsersManager.
                                                                                                      GetCurrentUserID()));
                            if (timedManager != null)
                            {
                                timedMessageManager = (ITimedMessageManager) timedManager;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                return timedMessageManager;
	        }
	    }
        
        public IInplaceClsView GetClsView(IEntity cls)
        {
            if (iClsNavi == null)
                iClsNavi = ((IServiceProvider)this).GetService(typeof(IBaseClsNavigation)) as IBaseClsNavigation;
            return iClsNavi.GetClsView(cls);
        }
        
        public IInplaceTasksPermissionsView GetTasksPermissions()
        {
            if (iAdminNavi == null)
                iAdminNavi = ((IServiceProvider)this).GetService(typeof(IAdministrationNavigation)) as IAdministrationNavigation;
            return iAdminNavi != null ? iAdminNavi.GetTasksPermissions() : null;
        }
        
		public string ViewObjectCaption
		{
			set { }
		}

        public FormWindowState WorkplaceState
        {
			[DebuggerStepThrough]
			get { return this.WindowState; }
			[DebuggerStepThrough]
			set { this.WindowState = value; }
        }

	    public IWin32Window WindowHandle
	    {
            get
            {
                return (IWin32Window)(Control)this;
            }
	    }

        #endregion

        public bool InitializeWorkplace()
        {
			addins = new List<BaseNavigationCtrl>();

            IsMdiContainer = true;

            InitializeComponent();

            ToolbarService.Attach(this);

            NavigationControl.ActiveGroupChanging += new Infragistics.Win.UltraWinExplorerBar.ActiveGroupChangingEventHandler(this.NavigationControl_ActiveGroupChanging);
            NavigationControl.ActiveGroupChanged += new Infragistics.Win.UltraWinExplorerBar.ActiveGroupChangedEventHandler(this.NavigationControl_ActiveGroupChanged);

			ControlSettings navigationControlSettings = new ControlSettings(NavigationControl);
			navigationControlSettings.Settings.Add(new PropertySetting(NavigationControl, "GroupSpacing"));
        	NavigationControl.SaveSettings = true;
        	NavigationControl.SaveSettingsFormat = SaveSettingsFormat.Xml;

            #region - Load -
            //Infragistics.Win.AppStyling.StyleManager.Load("d:\\New StyleSet.isl");

            NavigationControl.Groups.Clear();
            this.Refresh();

            if (!ConnectToScheme() && !differentVersionsMode)
            {
                this.Close();
                return false;
            }
            else
            {
                this.Text = String.Format("{0} - {1}", this.ActiveScheme.Name, this.Text);

                OperationObj.Text = "Загрузка элементов интерфейса";
                OperationObj.StartOperation();
            	Trace.TraceInformation("Загрузка элементов интерфейса...");
                try
                {
                    this.SuspendLayout();
                    WorkplaceAddinLoader.LoadViewObjectsEx(this, Addins, NavigationControl);   
                }
                finally
                {
					Trace.TraceInformation("Загрузка элементов интерфейса завершена");
					OperationObj.StopOperation();
                    this.ResumeLayout();
                    // создаем список для сохранения панелей статусбара при переходе между объектами просмотра
                    statusBarPanels = new Dictionary<string, object[]>();
                    // выводим окно на передний пдан
                    this.TopMost = true;
                    this.TopMost = false;
                    // и обновляем
                    //this.Refresh();
                    
                    ComponentCustomizer.CustomizeInfragisticsControls(this);
                    ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
                    // в зависимости от типа аутентификации делаем доступной кнопку смены пароля
					ToolbarService.Control.Tools["ChangePasswordCommand"].SharedProps.Visible = _authType == AuthenticationType.adPwdSHA512;

                    //Infragistics.Win.AppStyling.StyleManager.Load(this.StyleSheet);
                }
            }
            #endregion - Load -

            InitializeColorSchema();
            ToolbarService.Control.Tools["ColorSchemeList"].ToolClick += new ToolClickEventHandler(ColorSchemeList_ToolClick);
            ToolbarService.Control.Tools["ColorSchemeList"].ToolClick += new ToolClickEventHandler(ColorSchemeList_ToolClick);

            if (differentVersionsMode && !String.IsNullOrEmpty(connectionErrorMessage))
            {
                if (MessageBox.Show(connectionErrorMessage, "Предупреждение",
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    Close();
                    return false;
                }
            }
            
            return true;
        }

	    #region Windows Form Designer generated code
        /// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Workplace));
            
            this.SuspendLayout();

            // 
            // Workplace
            // 
            this.ClientSize = new System.Drawing.Size(931, 573);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Workplace";
            this.Text = "Мониторинг, анализ, прогноз и планирование";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Workplace_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Workplace_FormClosing);
			this.Closing += new CancelEventHandler(Workplace_Closing);
            this.Load += new System.EventHandler(this.Workplace_Load);
            this.ResumeLayout(false);
		}

		#endregion

        #region - Переключение объектов просмотра - 

        public IWorkplaceLayout WorkplaceLayout
        {
            get { return layout; }
            set
            {
                if (layout != null)
                {
                    layout.ActiveWorkplaceWindowChanged -= OnActiveWindowChanged;
                    layout.Detach();
                }
                value.Attach(this);
                layout = value;
                layout.ActiveWorkplaceWindowChanged += OnActiveWindowChanged;
            }
        }

        public DockableControlPane GetWorkplaceWindow(string key)
        {
            DockableControlPane viewPane = DockManagerService.AttachControl(key);
            return viewPane;
        }

        /// <summary>
		/// Подключение объекта просмотра
		/// </summary>
		/// <param name="ViewObj">Объект просмотра</param>
        private void AttachNavigationObject(BaseNavigationCtrl ViewObj, UltraExplorerBarContainerControl navContainer)
		{
            ViewObj.Parent = navContainer;
            ViewObj.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        private void DetachViewObject(BaseNavigationCtrl ViewObj)
		{
			ViewObj.Parent = null;
			ViewObj.Dock = System.Windows.Forms.DockStyle.None;
		}

        // TODO: Переделать получение активного объекта из активного Mdi окна
        private BaseNavigationCtrl GetActiveViewObject()
		{
			BaseNavigationCtrl CurViewObj = null;
			
            if (NavigationControl.Groups.ExplorerBar.ActiveGroup == null) 
                return null;
			
            string curExplorerBarCaption = NavigationControl.Groups.ExplorerBar.ActiveGroup.Text;
			for (int i = 0; i <= Addins.Count - 1; i++)
			{
                CurViewObj = (BaseNavigationCtrl)Addins[i];
				if (String.Compare(curExplorerBarCaption, CurViewObj.Caption) == 0) 
                    break;
			}
			
            return CurViewObj;
		}

        public event ActiveGroupChangedEventHandler ActiveGroupChanged;

		private void NavigationControl_ActiveGroupChanged(object sender, Infragistics.Win.UltraWinExplorerBar.GroupEventArgs e)
		{
            UltraExplorerBar exb = (UltraExplorerBar)sender;

            BaseNavigationCtrl activeViewObject = GetActiveViewObject();
            
            if (activeViewObject == null)
                return;
            
            // если объект еще не проинициализирован - делаем это
            if ((activeViewObject.Initialized == false) && (ActiveScheme != null || differentVersionsMode))
            {
                // инициализируем объект просмотра
                activeViewObject.Initialize();
                //activeViewObject.Initialize();

                activeViewObject.ActiveItemChanged += new BaseNavigationCtrl.ActiveItemChangedEventHandler(activeViewObject_ActiveItemChanged);

                // вставляем во все гриды свои хэндлеры
                //ComponentCustomizer.EnumControls(activeViewObject.ViewCtrl, new checkProcDelegate(this.CustomizeGridEventsHandlers));
                ComponentCustomizer.EnumControls(activeViewObject, new checkProcDelegate(this.CustomizeGridEventsHandlers));

                if (ActiveGroupChanged != null)
                    ActiveGroupChanged(sender, e);
            }
            
            // оповещаем объект о том что он активизирован
            //activeViewObject.Activate(true);

            //e.Group.ExplorerBar.BeginUpdate();
            AttachNavigationObject(activeViewObject, e.Group.Container);
            //e.Group.ExplorerBar.EndUpdate();

             if (exb.SelectedGroup != e.Group)
                    exb.SelectedGroup = e.Group;

            UIElement elem = e.Group.UIElement;
            // возвратим статусбар каким его дсделали
            LoadStatusBarState(e.Group.Key);
            //if (elem != null)
                //elem.Invalidate(true);
		}

        private void activeViewObject_ActiveItemChanged(BaseNavigationCtrl sender, BaseViewContentPane viewObject)
        {
            if (ViewContentCollection.Contains(viewObject))
            {
                int indx = ViewContentCollection.IndexOf(viewObject);
                if (ViewContentCollection[indx].WorkplaceWindow != null)
                    ViewContentCollection[indx].WorkplaceWindow.SelectWindow();
            }
            else
            {
                this.ShowView(viewObject);
            }
        }

		private void NavigationControl_ActiveGroupChanging(object sender, Infragistics.Win.UltraWinExplorerBar.CancelableGroupEventArgs e)
		{
            BaseNavigationCtrl activeViewObject = GetActiveViewObject();

            if (activeViewObject != null)
			{
                // сохраняем состояние статусбара
                SaveStatusBarState();

                DetachViewObject(activeViewObject);
			}
        }

        /// <summary>
        /// Переход на другой интерфейс просмотра
        /// </summary>
        /// <param name="uiModuleName"></param>
        /// <param name="moduleParams"></param>
        public void SwitchTo(string uiModuleName, params object[] moduleParams)
        {
            BaseNavigationCtrl CurViewObj = null;
            for (int i = 0; i <= Addins.Count - 1; i++)
            {
                CurViewObj = (BaseNavigationCtrl)Addins[i];
                if (uiModuleName.ToUpper() == CurViewObj.Caption.ToUpper())
                    break;
            }

            if (CurViewObj != null)
            {
                //if (NavigationControl.Groups[CurViewObj.ToString()]. )
                // не нашел свойства, которое бы отвечало за то, виден ли заголовок группы или нет
                try
                {
                    NavigationControl.ActiveGroup = NavigationControl.Groups[CurViewObj.ToString()];
                }
                catch{}
                CurViewObj.SetActive(moduleParams);
                //if (DockManagerService.Control.ControlPanes.Exists("ViewObjectControlPane"))
                //DockManagerService.Control.ControlPanes["ViewObjectControlPane"].Text = CurViewObj.Caption;
            }
        }

        #endregion - Переключение объектов просмотра -

        #region - ПЕРЕДЕЛАТЬ!!! сохранение восстановление состояния статусбара -

        private string currentModuleName;

        private Dictionary<string, object[]> statusBarPanels; 

        private void LoadStatusBarState(string moduleName)
        {
            if (statusBarPanels.ContainsKey(moduleName))
            {
                object[] panels;
                statusBarPanels.TryGetValue(moduleName, out panels);
                ((UltraStatusBar)StatusBarService.Control).Panels.Clear();
                foreach (UltraStatusPanel panel in panels)
                {
                    ((UltraStatusBar)StatusBarService.Control).Panels.Add(panel);
                }
            }
            currentModuleName = moduleName;
        }

        private void SaveStatusBarState()
        {
            if (statusBarPanels.ContainsKey(currentModuleName))
                statusBarPanels.Remove(currentModuleName);
            object[] panels = new object[((UltraStatusBar)StatusBarService.Control).Panels.Count];
            ((UltraStatusBar)StatusBarService.Control).Panels.CopyTo(panels, 0);
            statusBarPanels.Add(currentModuleName, panels);
        }

        #endregion

        #region - Обработчики для гридов -

        // Обработчик для всех гридов объектов просмотра. Устанавливает указатель количества записей и текущей записи
		private void AfterGridRowActivate(object sender, EventArgs e)
		{
            try
            {
                UltraGrid ug = (UltraGrid)sender;
                int recNo = 0;
                if (ug.ActiveRow != null) recNo = ug.ActiveRow.VisibleIndex + 1;
                string text = String.Format("Запись: {0} из {1}", recNo, ug.Rows.FilteredInRowCount);
                StatusBarService.SetRowPosition(text);
            }
            catch
            {
                // переделать на инвок
            }
		}

		private void GridLeave(object sender, EventArgs e)
		{
            try
            {
                StatusBarService.SetRowPosition("Запись: ...");
            }
            catch
            {
                // переделать на инвок
            }
        }

        public void CustomizeGridEventsHandlers(Component cmp)
        {
            if (cmp.GetType().Name.ToString() == "UltraGrid")
            {
                UltraGrid ug = (UltraGrid)cmp;
                ug.AfterRowActivate += AfterGridRowActivate;
                ug.Enter += AfterGridRowActivate;
                ug.Leave += GridLeave;
            }
        }

        #endregion - Обработчики для гридов -

        #region - Обработчики событий главной формы -

		protected override void OnLoad(EventArgs e)
		{
			// Эту операцию необходимо выполнить до срабатывания всех остальных обработчиков
			WorkplaceLayout.ShowPad(explorerPane);
			
			base.OnLoad(e);
		}

        private void Workplace_Load(object sender, System.EventArgs e)
		{
			Trace.TraceVerbose("Workplace.Workplace_Load");

            // сохраняем группы созданные согласно настроенным правам
            UltraExplorerBarGroupsCollection collection = new UltraExplorerBarGroupsCollection(NavigationControl);
            foreach (UltraExplorerBarGroup ultraExplorerBarGroup in NavigationControl.Groups)
            {
                collection.Add(ultraExplorerBarGroup);
            }

			NavigationControl.LoadComponentSettings();

            // восстанавливаем видимость групп согласно правам, а не пользовательским настройкам
            NavigationControl.Groups.Clear();
            foreach (UltraExplorerBarGroup ultraExplorerBarGroup in collection)
            {
                NavigationControl.Groups.Add(ultraExplorerBarGroup);
            }
            try
			{
				// Первую видимую группу надо сделать активной
				UltraExplorerBarGroup firstGroup = null;
                foreach (UltraExplorerBarGroup item in NavigationControl.Groups)
				{
                    if (item.Visible)
					{
						firstGroup = item;
                        break;
					}
				}

				if (firstGroup != null)
					NavigationControl.ActiveGroup = firstGroup;
			}
			catch (Exception exception)
			{
				Trace.TraceError(Krista.Diagnostics.KristaDiagnostics.ExpandException(exception));
				// сюда попадем если шустрый пользователь успел таки куда нибудь переключится
				// вероятность очень мала
				// или если первый загружаемый объект вызвал ошибку доступа - в этом случае передаем исключение дальше
				if (exception is Krista.FM.ServerLibrary.PermissionException)
					throw;
            }

            timedMessageManager = TimedMessageManager;
            timedMessageManager.OnReciveMessages += TimedMessageManager_OnReciveMessages;
            timedMessageManager.Activate();

            // Запуск процесса обновления
            UpdateFrameworkLibraryFactory.SetPropertyValue("Scheme", activeScheme);
            UpdateFrameworkLibraryFactory.InvokeMethod("InitializeNotifyIconForm");

            if (differentVersionsMode && !String.IsNullOrEmpty(connectionErrorMessage))
            {
                Text += " [Режим обновления клиента]";
                MessageBox.Show(
                    "Workplace запущен в аварийном режиме. Пожалуйста, дождитесь, пока будет получена новая версия, и установите ее.",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
		}

        private delegate bool IsFocusedDelegate();
        private bool IsFocused()
        {
            return Focused;
        }

        private delegate IntPtr IntPtrDelegate();
        private IntPtr GetIntPtr()
        {
            return Handle;
        }

        private void TimedMessageManager_OnReciveMessages(object sender, EventArgs e)
        {
            if (timedMessageManager.NewMessagesCount > 0)
            {
                StatusBarService.SetMessagesPanelVisible(true);
                StatusBarService.SetNewMessagesCount(String.Format("Новых сообщений: {0}", timedMessageManager.NewMessagesCount.ToString()));

                if (timedMessageManager.NewImportanceMessages > 0)
                {
                    IsFocusedDelegate isFocusedDelegate = IsFocused;
                    IntPtrDelegate intPtrDelegate = GetIntPtr;
                    if (!(bool)Invoke(isFocusedDelegate))
                    {
                        FlashWindow.Flash((IntPtr)Invoke(intPtrDelegate));
                    }

                    if (NavigationControl.Groups["Krista.FM.Client.ViewObjects.MessagesUI.MessageManagementNavigation"] != null)
                    {
                        NavigationControl.BlinkUltraExplorerBarGroup(
                            NavigationControl.Groups[
                                "Krista.FM.Client.ViewObjects.MessagesUI.MessageManagementNavigation"]);
                    }
                }
            }
            else
            {
                StatusBarService.SetMessagesPanelVisible(false);
            }
        }

        private void Workplace_FormClosed(object sender, FormClosedEventArgs e)
        {
			Trace.TraceVerbose("Workplace.Workplace_FormClosed");
		}

		private void Workplace_Closing(object sender, CancelEventArgs e)
		{
			Trace.TraceVerbose("Workplace.Workplace_Closing");
			NavigationControl.SaveComponentSettings();
		}

		private void Workplace_FormClosing(object sender, FormClosingEventArgs e)
        {
			Trace.TraceVerbose("Workplace.Workplace_FormClosing");

			// Если закрытие уже отменено, то ничено не делаем
			if (e.Cancel)
				return;

			// проверяем только для закрытия пользователем
			if (e.CloseReason != CloseReason.UserClosing)
				return;

			// TODO: Сделать проверку на возможность выгрузки объекта просмотра
            // эта проверка выполняется в другой части окна, а именно в обработчике события OnClosing
            /*
			foreach (BaseNavigationCtrl vo in Addins)
			{
				// если хотя бы один объект просмотра не хочет выгружаться - не закрываем приложение
				if (!vo.CanUnload)
				{
					closeAll = false;
					e.Cancel = true;
					break;
				}
			}*/
		}

        #endregion - Обработчики событий главной формы -

        #region - Стилевые схемы -

        /// <summary>
        ///  Инициализация цветовых схем программы
        /// a) Поиск стилевых таблиц и настройка списка схем в Application menu
        /// b) Установка текущей схемы программы
        /// </summary>
        private void InitializeColorSchema()
        {
            try
            {
                //формируем список. У элемента ключем идет полный путь, кэпшэном имя файла без расширения            
                ListTool list = (ListTool)ToolbarService.Control.Tools["ColorSchemeList"];
                string defPath = GetDefaultStyleSheetPath();
                string[] files = Directory.GetFiles(@".\Styles\", "*.isl", SearchOption.TopDirectoryOnly);
                try
                {
                    //ToolbarsManager.IsMayHook = true;
                    foreach (string fileName in files)
                    {
                        list.ListToolItems.Add(Path.GetFullPath(fileName),
                            Path.GetFileNameWithoutExtension(fileName),
                            ((Path.GetFullPath(fileName) == defPath) || (defPath == "")));
                    }
                }
                finally
                {
                    //ToolbarsManager.IsMayHook = false;
                }

                this.StyleSheet = defPath;
            }
            catch
            {
#warning Неполадки со стилями, пока обрабатываются глухо
            }
        }

        private string GetDefaultStyleSheetPath()
        {
            //сперва попытаемся прочитать из реестра (при первом запуске его может не быть)
            Utils RegUtils = new Utils(GetType(), true);
            string defaultPath = RegUtils.GetKeyValue("StyleSheetPath");
            if (File.Exists(defaultPath))
            {
                return defaultPath;
            }

            //теперь попробуем  "голубой оффис" по имени
            string blueOfficeSchemePath = @".\Styles\Office 2007 (голубая).isl";
            if (File.Exists(blueOfficeSchemePath))
            {
                return Path.GetFullPath(blueOfficeSchemePath);
            }

            return string.Empty;
        }

        /// <summary>
        /// Имя стилевой таблицы приложения (файл isl)
        /// Храниться полный путь к стилевой таблице
        /// </summary>
        string styleSheet;
        public string StyleSheet
        {
            get { return styleSheet; }
            set
            {
                if (File.Exists(value))
                {
                    //устанавливаем поле
                    styleSheet = value;

                    //меняем стиль приложения
                    //Infragistics.Win.AppStyling.StyleManager.Load(value);
                    //AdjustWindowViewByStylesheet(value);

                    //Некоторые галереи панели инструментов меняются динамически, в зависимости 
                    //от выбранной цветовой схемы, как раз сейчас их поменяем
                    //this.ToolbarsManager.Load(value);

                    //запоминаем в реестре
                    Utils RegUtils = new Utils(GetType(), true);
                    RegUtils.SetKeyValue("StyleSheetPath", value);

                    //dockPanelControl.StyleSheet = styleSheet;
                }
                else
                {
#warning Выдать сообщение, что файл схемы не найден (его могли грохнуть враги, после запуска программы)
                }
            }
        }

        void ColorSchemeList_ToolClick(object sender, ToolClickEventArgs e)
        {
            ListToolItem item = ((ListTool)e.Tool).SelectedItem;
            if (item != null)
            {
                this.StyleSheet = item.Key;
            }
        }

        #endregion - Стилевые схемы -

        private AuthenticationType _authType = AuthenticationType.atUndefined;

        public bool ConnectToScheme()
        {
            string ServerName = "";
            string SchemeName = "";
            string Login = "";
            string Password = "";
            IScheme ProxyScheme = null;

            // Настройка среды .NET Remoting
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.FriendlyName + ".config", false);

            while (true)
            {
            	Trace.TraceInformation("Запрос параметров подключения к серверу");

                // показываем окно с диалогом подключения к схеме
                if (frmLogon.ShowLogonForm(ref ServerName, ref SchemeName, ref Login, ref Password,
                    ref ProxyScheme, ref differentVersionsMode, ref _authType, ref connectionErrorMessage))
                {
                    if (ProxyScheme != null || differentVersionsMode)
                    {
						Trace.TraceInformation("Подключение успешно");
						
						// если успешно подключились - создаем воркплайс
                        SetActiveScheme(new SMO.SmoScheme(((ISMOSerializable)ProxyScheme).GetSMOObjectData()));
                        //ActiveScheme = ProxyScheme;
                        // ... и выходим из цикла
                        break;
                    }
                    else
                    {
						Trace.TraceError("Не удалось подключиться к схеме");
						
						// если нет - предлагаем поключится заново
                        if (MessageBox.Show("Не удалось подключиться к схеме. Повторить?", "Ошибка подключения",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) break;
                    }
                }
                // если пользователь не захотел подключаться - выходим из цикла
                else break;
            }
            // в текущей конфигурации без схемы мы работать не можем
            return (ProxyScheme != null);
        }

        public void ChangePasswordAdm(int userID)
        {
            FormChangePassword.ChangePassword(this.ActiveScheme, true, userID);
        }

        /// <summary>
        /// обработчик событий при выключении компа или завершении сеанса пользователя в виндах
        /// </summary>
        /// <param name="m"></param>
        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x16 || m.Msg == 0x11)// WM_ENDSESSION WM_QUERYENDSESSION
            {
                Application.Exit();
            }
            base.WndProc(ref m);
        }

        public int WndHandle
        {
			[DebuggerStepThrough]
			get { return (int)this.Handle; }
        }

		internal List<BaseNavigationCtrl> Addins
		{
			[DebuggerStepThrough]
			get { return addins; }
		}

		#region IServiceProvider Members
        
        object IServiceProvider.GetService(Type serviceType)
        {
            foreach (object item in Addins)
            {
                Type[] types = item.GetType().GetInterfaces();
                foreach (Type type in types)
                {
                    if (serviceType.Equals(type))
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        
        #endregion
	}
}
