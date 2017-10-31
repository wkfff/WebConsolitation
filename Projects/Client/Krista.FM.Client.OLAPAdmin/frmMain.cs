using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.OLAPAdmin.Properties;
using Krista.FM.Client.OLAPResources;
using Krista.FM.Client.OLAPStructures;
using Krista.FM.Client.TreeLogger;
using Krista.FM.Common;
using Krista.FM.Common.Services;
using Microsoft.AnalysisServices;
using Resources=Krista.FM.Client.OLAPAdmin.Properties.Resources;

namespace Krista.FM.Client.OLAPAdmin
{
    public partial class frmMain : Form
    {	
		ServerManager manager;
		
		internal Log log = new Log("Восстановление многомерной базы");
		
		delegate void AddDatabaseDelegate();

		private delegate Database CreateFullDatabaseDelegate(DatabaseInfoBase databaseInfo);
        
		public frmMain()
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeUltraTabControl(tabCtrlServers);

            ResourceService.InitializeService(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            Krista.FM.Client.Common.Resources.Loader.Initialize();
        }        
        
        
        private void toolStripBtnConnect_Click()
        {
			manager.CurrentServerWrapper.Connect();
        }

		private void frmMain_Load(object sender, EventArgs e)
		{
			Width = 1024;
			Height = 768;
			CenterToScreen();
			tabCtrlServers.ImageList = OLAPResources.OLAPResources.ImageListSmall;
			manager = new ServerManager(tabCtrlServers);
			manager.AddServers();
		    if (tabCtrlServers.Tabs.Count > 0) tabCtrlServers.SelectedTab = tabCtrlServers.Tabs[0];
		    tabCtrlServers_Selected();

            UpdateFrameworkLibraryFactory.InvokeMethod("InitializeNotifyIconForm");
		}
        
		private void toolStripBtnDelete_Click()
		{
			if (Messanger.Question(string.Format(
				Resources.msgDeleteServer, manager.CurrentServer.Name)) == DialogResult.Yes)
			{
				manager.DeleteCurrentServer();
			}			
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			manager.DisconnectFromServers();
			Settings.Default.Save();
		}

		private void toolStripBtnRefresh_Click()
		{
			manager.CurrentServerWrapper.Disconnect();
			manager.CurrentServerWrapper.Connect();			
		}		
		
		public void ShowLog()
		{
			if (log != null) { log.ShowLog(this); }
		}

		private void tabCtrlServers_Selected()
		{
		    if (manager.CurrentServerWrapper != null)
		        if (manager.CurrentServerWrapper.ServerPage != null)
		            switch (manager.CurrentServerWrapper.ServerPage.ServerType)
		            {
		                case ServerType.Server:
		                    break;
		                case ServerType.PackageRepository:
		                    break;
		                case ServerType.VSSRepository:
		                    break;
		                default:
		                    break;
		            }
		}

		private void toolStripBtnGenerate_Click()
		{
			GenerateNewHelper.Generate(this);			
		}

		private void toolStripBtnSettings_Click()
		{
			SettingsHelper.Edit(this);
		}

        /// <summary>
        /// Обработчик главного меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {

            switch(e.Tool.Key)
            {
                case "CloseApp":
                    {
                        Close();
                        break;
                    }
                case "Regsrv":
                    {
                        //System.Configuration.ConfigurationManager.AppSettings[<Имя ключа>]
                        //и добавить в референсы сборку System.Configuration.
                        string serverName = ((TextBoxTool)e.Tool).Text.Trim();

                        if (serverName.Length > 0)
                        {
                            manager.AddServer(serverName).Connect();
                            SaveLoadSettings.SaveServer(serverName);
                        }
                        else
                        {
                            Messanger.Error(Resources.msgEnterServerName);
                        }			
                        break;
                    }
                case "Connect":
                    {
                        Operation operation = new Operation();

                        try
                        {
                            operation.Text = "Подключение к серверу";
                            operation.StartOperation();
                            if (manager.CurrentServerWrapper != null) manager.CurrentServerWrapper.Connect();
                            break;
                        }
                        finally
                        {
                            operation.StopOperation();
                            operation.ReleaseThread();
                        }
                    }
                case "RefreshSvr":
                    {
                        manager.CurrentServerWrapper.Disconnect();
                        manager.CurrentServerWrapper.Connect();	
                        break;
                    }
                case "DeleteSvr":
                    {
                        if (Messanger.Question(string.Format(Resources.msgDeleteServer, manager.CurrentServer.Name)) == DialogResult.Yes)
                        {
                            manager.DeleteCurrentServer();
                        }
                        break;
                    }
                case "GenNew":
                    {
                        GenerateNewHelper.Generate(this);
                        break;
                    }
                case "Customize":
                    {
                        SettingsHelper.Edit(this);
                        break;
                    }
                case "DatabasesArea":
                    {
                        if (!((StateButtonTool)e.Tool).Checked)
                        {
                            manager.CurrentServerWrapper.ServerPage.SplitContainer.Panel1Collapsed = true;
                        }
                        else
                        {
                            manager.CurrentServerWrapper.ServerPage.SplitContainer.Panel1Collapsed = false;
                        }
                        break;
                    }
                case "ObjectsArea":
                    {
                        if (!((StateButtonTool)e.Tool).Checked)
                        {
                            manager.CurrentServerWrapper.ServerPage.SplitContainerbababasesPatch.Panel1Collapsed = true;
                        }
                        else
                        {
                            manager.CurrentServerWrapper.ServerPage.SplitContainerbababasesPatch.Panel1Collapsed = false;
                        }
                        break;
                    }
                case "PatchesArea":
                    {
                        if (!((StateButtonTool)e.Tool).Checked)
                        {
                            manager.CurrentServerWrapper.ServerPage.SplitContainerbababasesPatch.Panel2Collapsed = true;
                        }
                        else
                        {
                            manager.CurrentServerWrapper.ServerPage.SplitContainerbababasesPatch.Panel2Collapsed = false;
                        }
                        break;
                    }
                case "About":
                    {
                        // временная версия О программе
                        MessageBox.Show("Утилита для администрирования многомерных баз", "О программе",
                                        MessageBoxButtons.OK, MessageBoxIcon.None);
                        break;
                    }
            }
        }

    }	
}