using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTabControl;
using Microsoft.AnalysisServices;
using System.Xml.XPath;
using System.Xml;
using System.IO;
using System.Drawing;

using Krista.FM.Client.OLAPStructures;
using Krista.FM.Client.OLAPResources;
using Krista.FM.Client.OLAPAdmin.Properties;
using System.Configuration;

namespace Krista.FM.Client.OLAPAdmin
{
	class ServerManager
	{	
		public const int firstServerPageIndex = 1;

		public static List<Microsoft.AnalysisServices.Server> servers =
			new List<Microsoft.AnalysisServices.Server>();

		private UltraTabControl tabServers;


        public ServerManager(UltraTabControl _tabServers)
		{
			tabServers = _tabServers;			
			//new ServerControlsWrapper(tabServers, new ctrlServerPage(ServerType.VSSRepository));
			//new ServerControlsWrapper(tabServers, new ctrlServerPage(ServerType.PackageRepository));
		}

        private UltraTabControl TabServers
		{
			get { return tabServers; }
		}

		private Microsoft.AnalysisServices.Server ServerByName(string serverName, bool createIfnotFound)
		{			
			for (int i = 0; i < servers.Count; i++)
			{
				//Сравниваем по ConnectionString, т.к. Server.Name автоматически
				//подменяет "localhost" на имя комьютера в сети.
				if (servers[i].ConnectionString == serverName) { return servers[i]; }
			}
			if (createIfnotFound)
			{
				Microsoft.AnalysisServices.Server server =
					new Microsoft.AnalysisServices.Server();
				server.Name = GetServerName(serverName);
				servers.Add(server);
				return server;
			}
			return null;
		}

        /// <summary>
        /// Получение строкового имени сервеа по указанному (вместо имени может быть IP адрес компьютера)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
	    private string GetServerName(string name)
	    {
	        if (name.LastIndexOf('.') != -1)
	        {
	            string[] ip = name.Split('/');
                if (ip[0] != null && ip[1] != null)
                {
                    IPHostEntry entry = Dns.GetHostEntry(IPAddress.Parse(ip[0]));
                    return String.Format("{0}\\{1}", entry.HostName.Split('.')[0], ip[1]);
                }
	        }

            return name;
	    }

	    public Microsoft.AnalysisServices.Server ServerByName(string serverName)
		{
			return ServerByName(serverName, false);
		}

		private ServerControlsWrapper GetServerWrapper(string name)
		{
		    if (TabServers.Tabs.Exists(name))
		    {
		        int index = TabServers.Tabs[name].Index;
		        if (index >= 0)
		        {
		            TabServers.SelectedTab = TabServers.Tabs[index];
		            return (ServerControlsWrapper)TabServers.Tabs[index].Tag;
		        }
		    }
		    return null;
		}

		public ServerControlsWrapper PackageRepository
		{
			get
			{ 
				return (ServerControlsWrapper)TabServers.
                    Tabs[OLAPResources.Utils.PackageRepositoryName].Tag;
			}
		}

		public void AddPackage(DatabaseHeader package)
		{
			PackageRepository.ServerPage.AddDatabase(package);
		}

		public void AddDatabase(Database DB)
		{	
			CurrentServerWrapper.ServerPage.AddDatabase(DB);
		}		
		
		public ServerControlsWrapper AddServer(string serverName)
		{
			Microsoft.AnalysisServices.Server server = ServerByName(serverName, true);
			ServerControlsWrapper wrapper = GetServerWrapper(serverName);
			if (wrapper == null)
				wrapper = new ServerControlsWrapper(TabServers, new ctrlServerPage(server));
			else
				wrapper.Refresh(new ctrlServerPage(server));
			return wrapper;
		}		

		public void AddServers()
		{
			string[] servers = SaveLoadSettings.ReadServers();
			for (int i = 0; i < servers.Length; i++)
				AddServer(servers[i]);
		}		

		public ServerControlsWrapper CurrentServerWrapper
		{
			get 
            {
                if (tabServers.SelectedTab != null)
                    return (ServerControlsWrapper)tabServers.SelectedTab.Tag;
                return null;
            }
		}

		public Microsoft.AnalysisServices.Server CurrentServer
		{
			get { return CurrentServerWrapper.Server; }
		}

		public DatabaseHeader CurrentPackage
		{
			get { return PackageRepository.ServerPage.DatabaseHeader; }
		}

		public void DeleteCurrentServer()
		{
			CurrentServerWrapper.Disconnect();
			SaveLoadSettings.DeleteServer(CurrentServerWrapper.ServerPage.ID);
            tabServers.Tabs.Remove(tabServers.Tabs[CurrentServerWrapper.ServerPage.ID]);
		}

		public void DisconnectFromServers()
		{
			for (int i = 0; i < servers.Count; i++)
				if (servers[i].Connected) { servers[i].Disconnect(true); }
		}		
	}

	public enum ServerType
	{
		Server = 0,
		PackageRepository = 1,
		VSSRepository = 2,
	}	

	internal class ServerControlsWrapper
	{
		UltraTab tabPage;

        private ctrlWorkingPatch workingPatch;

        public ServerControlsWrapper(UltraTabControl tabServers, ctrlServerPage _serverPage)
		{		
			tabServers.Tabs.Add(_serverPage.ID, _serverPage.ID);
            tabPage = tabServers.Tabs[tabServers.Tabs.Count - 1];
			tabPage.Tag = this;
			//tabPage.Tab.Appearance.I = "Undefined";
			OLAPResources.Utils.InsertControl(tabPage.TabPage, _serverPage);
			_serverPage.Name = "ctrlServerPage";
			Refresh(_serverPage);
		}

        void item_Click(object sender, EventArgs e)
        {
            ((UltraTabControl)tabPage.TabControl).Tabs.Remove(tabPage);
        }


		private void CreateTabDatabases()
		{
            UltraTabControl tabDatabases = new UltraTabControl();
			OLAPResources.Utils.InsertControl(tabPage.TabPage, tabDatabases);
			//tabDatabases.Multiline = true;
			tabDatabases.Name = "tabDatabases";			
		}		

		public void Refresh(ctrlServerPage _serverPage)
		{
			switch (_serverPage.ServerType)
			{
				case ServerType.Server:
					break;
				case ServerType.PackageRepository:
					break;
				default:
					break;
			}
		}		

		public Microsoft.AnalysisServices.Server Server
		{
			get
			{
				if (ServerPage != null) { return ServerPage.Server; }
				else return null;
			}
		}
		
		public ctrlServerPage ServerPage
		{
			get
			{
				return (ctrlServerPage)tabPage.TabPage.Controls["ctrlServerPage"];
			}
		}		

	    public ctrlWorkingPatch WorkingPath
	    {
	        get
	        {
                if (workingPatch != null)
                    return workingPatch;

	            workingPatch = ServerPage.CtrlWorkingPatch;
                return workingPatch; 
	        }
	    }

		public void Connect()
		{
			switch (ServerPage.ServerType)
			{
				case ServerType.Server:
					ConnectToServer();
					break;
				case ServerType.PackageRepository:
					break;
				case ServerType.VSSRepository:
					ConnectToVSS();
					break;
				default:
					break;
			}
		}

		public void ConnectToVSS()
		{
			//to-do сохранять имя пользователя в настройках
			//string[] loginInfo = OLAPResources.OLAPResources.OpenLoginForm(
			//    Environment.UserName, OLAPResources.Utils.VSSRepositoryINI, frmMain.ActiveForm);
			
			string[] loginInfo = OLAPResources.OLAPResources.OpenLoginForm(
				Environment.UserName, Settings.Default.VSSINI, frmMain.ActiveForm);
			
			if (loginInfo != null)
			{
				try
				{
                    tabPage.Appearance.Image = Properties.Resources.activserver;

					ServerPage.VSSDB.Open(loginInfo[0], loginInfo[1], loginInfo[2],
						OLAPResources.Utils.VSSRepositoryRoot, OLAPResources.Utils.VSSRepositoryRootLocal);
					//tabPage.ImageKey = "OK";
					ServerPage.ConnectToVSS();
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine(e.Message);
					Messanger.Error(e.Message);
					ConnectToVSS();
				}			
			}			
		}

		public void ConnectToServer()
		{
			if (!Server.Connected)
			{
				//tabPage.ImageKey = "Warning";
				tabPage.TabPage.Refresh();
				try
				{
					Server.Connect(ServerPage.ID);
				    tabPage.Appearance.Image = Properties.Resources.activserver;
					ServerPage.ConnectToServer(Server);

				    WorkingPath.Server = Server;
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine(e.Message);
					Messanger.Error(e.Message);
					//tabPage.ImageKey = "Error";
				}
			}
		}

		public void Disconnect()
		{
			switch (ServerPage.ServerType)
			{
				case ServerType.Server:
					DisconnectFromServer();
					break;
				case ServerType.PackageRepository:
					break;
				case ServerType.VSSRepository:
					DisconnectFromVSS();
					break;
				default:
					break;
			}
		}

		public void DisconnectFromVSS()
		{
			if (ServerPage.VSSDB != null)
			{
				ServerPage.VSSDB.Close();
				//tabPage.ImageKey = "Undefined";
			}
		}

		public void DisconnectFromServer()
		{
			if (Server != null && Server.Connected)
			{
				//tabPage.ImageKey = "Warning";
				tabPage.TabPage.Refresh();
				Server.Disconnect(true);
				//tabPage.ImageKey = "Undefined";
			}
		}

		public void DeleteDatabase()
		{	
			if (ServerPage.DatabaseHeader != null && ServerPage.DatabaseHeader.NamedComponent != null)
			{
				Database DB = ServerPage.DatabaseHeader.NamedComponent;				
				DB.Drop(DropOptions.IgnoreFailures);
				Server.Databases.Remove(DB);
				Server.Update(UpdateOptions.Default, UpdateMode.Default);
				DB.Dispose();
				ServerPage.RefreshDatabases();
			}			
		}
	}
}