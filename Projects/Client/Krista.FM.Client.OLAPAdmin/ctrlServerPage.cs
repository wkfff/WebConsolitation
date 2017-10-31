using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml.XPath;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.OLAPAdmin.Properties;
using Krista.FM.Client.OLAPResources;
using Krista.FM.Client.OLAPStructures;
using Krista.FM.Client.TreeLogger;
using Krista.FM.Providers.VSS;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices;
using CommandType=Krista.FM.Client.OLAPStructures.CommandType;
using Utils=Krista.FM.Client.OLAPResources.Utils;
//using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.OLAPAdmin
{
	public partial class ctrlServerPage : UserControl
	{
		protected Microsoft.AnalysisServices.Server server;
		protected Log log = new Log("Восстановление многомерной базы");
		protected ServerType serverType = ServerType.Server;		
		protected IVSSFacade vss;

		protected delegate void AddDatabaseDelegate(Database DB);
		protected delegate Database CreateFullDatabaseDelegate(DatabaseInfoBase databaseInfo);

		protected void Init(ServerType _serverType)
		{
			InitializeComponent();
			serverType = _serverType;			
			ctrlDatabases.Init(null, "Нет подключения.", ValueChangedHandler, null, null);
			InitDatabasesButtons();
			CubesExplorer.Init(null, "Кубы", null, null, null);
			DimsExplorer.Init(null, "Измерения", null, null, null);
            DSExplorer.Init(null, "Источники данных", null, null, null);
		}

		public ctrlServerPage(ServerType _serverType)
		{
			Init(_serverType);
		}

		public ctrlServerPage(Microsoft.AnalysisServices.Server _server)
		{
			server = _server;
			Init(ServerType.Server);

            ctrlWorkingPatch.AddObjectsEvent += new EventHandler(ctrlWorkingPatch_AddObjectsEvent);
		}

        void ctrlWorkingPatch_AddObjectsEvent(object sender, EventArgs e)
        {
            if (ctrlDatabase.TabCtrlDB.SelectedTab.Index == 0)
            {
                foreach (object o in ctrlDatabase.CubesExplorer.ObjectList.SelectedItems)
                {
                    if (o is OLAPObjectHeader)
                    {
                        DataRow row = ctrlWorkingPatch.Table.NewRow();
                        row[1] = ((OLAPObjectHeader) o).NamedComponent.Name;
                        row[2] = "Куб";
                        row[3] = UpdateType.Create;
                        ctrlWorkingPatch.Table.Rows.Add(row);

                        ctrlWorkingPatch.ObjForScriptCollection.Add(((OLAPObjectHeader) o).NamedComponent.Name,
                                                                    new ObjectForScript(CommandType.alter,
                                                                                        ((MajorObject)
                                                                                         ((OLAPObjectHeader) o).
                                                                                             NamedComponent),
                                                                                        row[2].ToString()));

                        ctrlWorkingPatch.Table.AcceptChanges();
                    }
                }
            }

            if (ctrlDatabase.TabCtrlDB.SelectedTab.Index == 1)
            {
                foreach (object o in ctrlDatabase.DimsExplorer.ObjectList.SelectedItems)
                {
                    if (o is OLAPObjectHeader)
                    {
                        DataRow row = ctrlWorkingPatch.Table.NewRow();
                        row[1] = ((OLAPObjectHeader) o).NamedComponent.Name;
                        row[2] = "Измерение";
                        row[3] = UpdateType.Create;
                        ctrlWorkingPatch.Table.Rows.Add(row);

                        ctrlWorkingPatch.ObjForScriptCollection.Add(((OLAPObjectHeader) o).NamedComponent.Name,
                                                                    new ObjectForScript(CommandType.alter,
                                                                                        ((MajorObject)
                                                                                         ((OLAPObjectHeader) o).
                                                                                             NamedComponent),
                                                                                        row[2].ToString()));

                        ctrlWorkingPatch.Table.AcceptChanges();
                    }
                }
            }

            ctrlWorkingPatch.SaveChanges();
           ctrlWorkingPatch.RefreshScriptObject();
        }

		public ServerType ServerType
		{
			get { return serverType; }
		}

		public string ID
		{	
			get
			{
				switch (serverType)
				{
					case ServerType.Server:
						if (server != null)
						{
							return server.ID;
						}
						else
						{
							return string.Empty;
						}						
					case ServerType.PackageRepository:
						return Utils.PackageRepositoryName;
					case ServerType.VSSRepository:
						return Utils.VSSRepositoryName;
					default:
						return string.Empty;						
				}				
			}
		}		

		public Microsoft.AnalysisServices.Server Server
		{
			get
			{
				if (ServerType == ServerType.Server) { return server; }
				return null;
			}
		}

		public IVSSFacade VSSDB
		{
			get
			{
				if (ServerType == ServerType.VSSRepository)
				{
					if (vss == null)
					{
						vss = new VSSFacade();
					}

					return vss;
				}
				return null;
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public void InitDatabasesButtons()
		{	
			switch (serverType)
			{
				case ServerType.Server:
					InitServerButtons();
					break;
				case ServerType.PackageRepository:
					InitPackageRepositoryButtons();
					break;
				case ServerType.VSSRepository:
					InitVSSRepositoryButtons();
					break;
				default:
					break;
			}
		}

		public void InitServerButtons()
		{	
			ctrlDatabases.AddButton("Восстановить", "restore",
				Utils.GetResourceImage32("OK"), new ToolClickEventHandler(RestoreDatabase_Click));
			ctrlDatabases.AddButton("Сохранить", "save",
				Utils.GetResourceImage32("OK"), new ToolClickEventHandler(DBToPackage_Click));
			ctrlDatabases.AddSeparator();
			ctrlDatabases.AddButton("Удалить", "delete",
                Utils.GetResourceImage32("Error"), new ToolClickEventHandler(DeleteDatabase_Click));
			SetButtonsState(ctrlDatabases, false, new object[] { "restore", "save", "delete" });
		}

		public void InitPackageRepositoryButtons()
		{	
			ctrlDatabases.AddButton("Открыть", "restore",
                Utils.GetResourceImage32("OK"), new ToolClickEventHandler(RestoreDatabase_Click));
			ctrlDatabases.AddButton("Сохранить", "save",
                Utils.GetResourceImage32("OK"), new ToolClickEventHandler(DBToPackage_Click));
			ctrlDatabases.AddSeparator();
			ctrlDatabases.AddButton("Удалить", "delete",
                Utils.GetResourceImage32("Error"), new ToolClickEventHandler(DeleteDatabase_Click));
			SetButtonsState(ctrlDatabases, false, new object[] { "save", "delete" });
		}

		private void InitVSSDatabaseButtons(ctrlObjectExplorer explorer)
		{
			explorer.AddButton("Создать новый", "add",
                Utils.GetResourceImage32("OK"), new ToolClickEventHandler(AddNewDatabaseHeader_Click));
			explorer.AddSeparator();
			SetButtonsState(explorer, false, new object[] { "restore", "save", "add" });
		}

		public void InitVSSRepositoryButtons()
		{	
			ctrlDatabases.AddButton("Извлечь", "restore",
                Utils.GetResourceImage32("OK"), new ToolClickEventHandler(CheckOutSeveralObjects_Click));
			ctrlDatabases.AddButton("Выложить", "save",
                Utils.GetResourceImage32("OK"), new ToolClickEventHandler(CheckInSeveralObjects_Click));
			//ctrlDatabases.AddSeparator();
			//ctrlDatabases.AddButton("Удалить", "delete",
			//    OLAPResources.Utils.GetResourceImage32("Error"), new EventHandler(DeleteDatabase_Click));
			ctrlDatabases.AddSeparator();
			ctrlDatabases.AddButton("Создать новый", "add",
                Utils.GetResourceImage32("OK"), new ToolClickEventHandler(AddNewDatabaseHeader_Click));
			SetButtonsState(ctrlDatabases, false, new object[] { "restore", "save" , "add"});

			InitVSSDatabaseButtons(ctrlDatabase.CubesExplorer);
			InitVSSDatabaseButtons(ctrlDatabase.DimsExplorer);
		}

        private void RestoreDatabase_Click(object sender, ToolClickEventArgs e)
		{
			if (Utils.OpenDialog(Settings.Default.FilePathPackages) == DialogResult.OK)
			{
				if (ServerType == ServerType.Server)
				{
					SetButtonsState(ctrlDatabases, false, new object[] { "restore", "save", "delete" });
					DatabaseInfoBase databaseInfo = GetDatabaseInfo(
						server, Utils.OpenDialogFileName);
					CreateFullDatabaseDelegate createFullDatabaseDelegate =
						new CreateFullDatabaseDelegate(
						SQLServerUtils.SQLServerUtils.CreateFullDatabase);
					AsyncCallback callBack = new AsyncCallback(ProcessCreateFullDatabaseResult);
					IAsyncResult aResult = createFullDatabaseDelegate.BeginInvoke(databaseInfo,
						callBack, @databaseInfo);
					log.ShowLog(this);					
				}
				else
				{
					AddDatabase(new DatabaseHeader(Utils.OpenDialogFileName));
				}
			}
		}

		private void CheckOutHeaders(List<object> headers)
		{
			for (int i = 0; i < headers.Count; i++)
			{
				OLAPObjectHeader header = (OLAPObjectHeader)headers[i];
				if (header.VSSInfo.VSSState == VSSFileStatus.VSSFILE_NOTCHECKEDOUT)
				{
					vss.Checkout(header.VSSInfo.VSSSpec, string.Empty);
					header.VSSInfo.VSSState = VSSFileStatus.VSSFILE_CHECKEDOUT_ME;
				}
			}
		}

		private List<object> FilterHeadersByVSSState(
			VSSFileStatus getWithThis, List<object> items)
		{
			List<object> filtredItems = new List<object>();
			for (int i = 0; i < items.Count; i++)
			{
				OLAPObjectHeader header = (OLAPObjectHeader)items[i];
				if (header.VSSInfo.VSSState == getWithThis)
				{
					filtredItems.Add(header);
				}
			}
			return filtredItems;
		}

        private void CheckOutSeveralObjects_Click(object sender, ToolClickEventArgs e)
		{	
			DatabaseHeader newHeader = (DatabaseHeader)DatabaseHeader.Clone();
			newHeader.ControlBlock.Cubes.RefreshItems(newHeader.ControlBlock.Cubes.GetFiltredHeaders(
				VSSFileStatus.VSSFILE_NOTCHECKEDOUT, false));
			newHeader.ControlBlock.Dimensions.RefreshItems(newHeader.ControlBlock.Dimensions.GetFiltredHeaders(
				VSSFileStatus.VSSFILE_NOTCHECKEDOUT, false));
			WizardSelectOLAPObjects wizard = new WizardSelectOLAPObjects(this, newHeader);
			if (wizard.Execute())
			{
				CheckOutHeaders(wizard.CubesPage.RightList);
				CheckOutHeaders(wizard.DimensionsPage.RightList);
			}
		}

		private void CheckInHeaders(List<object> headers)
		{
			for (int i = 0; i < headers.Count; i++)
			{
				OLAPObjectHeader header = (OLAPObjectHeader)headers[i];
				if (header.VSSInfo.NewFile ||
					header.VSSInfo.VSSState == VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
				{
					string comment = string.Empty;
					if (header.VersionedObjectInfo != null && header.VersionedObjectInfo.GetLastVersion() != null)
					{
						comment = header.VersionedObjectInfo.GetLastVersion().Comment;
					}
					vss.Checkin(header.VSSInfo.VSSSpec, comment);
					header.VSSInfo.VSSState = VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
					header.VSSInfo.NewFile = false;					
				}
			}
		}

		private void IncrementVersion(DualAccessDictionary items)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> item in items)
			{
				if (item.Value.VersionedObjectInfo != null)
				{
					if (item.Value.VersionedObjectInfo.Versions == null)
					{
						item.Value.VersionedObjectInfo.Versions = new Versions();
					}
					item.Value.VersionedObjectInfo.Versions.IncrementVersion(
						VersionPart.Revision, vss.GetUserName(), string.Empty, DateTime.Now);
				}
			}
		}

        private void CheckInSeveralObjects_Click(object sender, ToolClickEventArgs e)
		{
			DatabaseHeader newHeader = (DatabaseHeader)DatabaseHeader.Clone();			
			newHeader.ControlBlock.Cubes.RefreshItems(newHeader.ControlBlock.Cubes.GetFiltredHeaders(
				VSSFileStatus.VSSFILE_CHECKEDOUT_ME, true));
			newHeader.ControlBlock.Dimensions.RefreshItems(newHeader.ControlBlock.Dimensions.GetFiltredHeaders(
				VSSFileStatus.VSSFILE_CHECKEDOUT_ME, true));
			
			//Добавляем новую версию к каждому объекту
			IncrementVersion(newHeader.ControlBlock.Cubes);
			IncrementVersion(newHeader.ControlBlock.Dimensions);

			WizardSelectOLAPObjectsAndProperties wizard = new WizardSelectOLAPObjectsAndProperties(this, newHeader);
			if (wizard.Execute())
			{
				//Сохраняем объекты, т.к. у них появилась информация о новой версии
				DatabaseHeader newHeaderToSave = (DatabaseHeader)newHeader.Clone();
				newHeaderToSave.ControlBlock.Cubes.RefreshItems(wizard.CubesPage.RightList);
				newHeaderToSave.ControlBlock.Dimensions.RefreshItems(wizard.DimensionsPage.RightList);
				newHeaderToSave.SaveToFile(false);
				//И только сохраненные объкты уже выкладываем в VSS
				CheckInHeaders(wizard.CubesPage.RightList);
				CheckInHeaders(wizard.DimensionsPage.RightList);
			}
		}

		private void AddNewDatabaseHeader_Click(object sender, ToolClickEventArgs e)
		{	
			DatabaseHeader newHeader = SaveDatabaseHeaderToFile((DatabaseHeader)DatabaseHeader.Clone());
			if (newHeader != null)
			{	
				vss.Checkin(Utils.PathToVSSPath(newHeader.ControlBlock.FileName), string.Empty);
				vss.Refresh();
				ConnectToVSS();
			}
		}		

		private void ProcessCreateFullDatabaseResult(IAsyncResult result)
		{
			SetButtonsState(ctrlDatabases, true, new object[] { "restore", "save", "delete" });
			if (result != null)
			{
				DatabaseInfoBase databaseInfo = (DatabaseInfoBase)result.AsyncState;
				Database DB = databaseInfo.DB;
				if (DB != null)
					this.Invoke(new AddDatabaseDelegate(AddDatabase), DB);					
			}
		}

		private DatabaseInfoBase GetDatabaseInfo(
			Microsoft.AnalysisServices.Server server, string packageFileName)
		{
			XPathNavigator packageNavigator
				= new XPathDocument(packageFileName).CreateNavigator();
			PackageFormat packageFormat =
				PackageFormatResolver.GetPackageFormat(packageNavigator);
			DatabaseInfoBase databaseInfo = null;
			switch (packageFormat)
			{
				case PackageFormat.Package2000:
					databaseInfo = new DatabaseInfo2000(packageNavigator);
					break;
				case PackageFormat.EmptyPack:
				case PackageFormat.ConfigPack:
				case PackageFormat.FullPack:
					DatabaseHeader sourceHeader = new DatabaseHeader(packageFileName);
					WizardSelectOLAPObjectsAndProperties wizard =
						new WizardSelectOLAPObjectsAndProperties(this, sourceHeader);
					wizard.Execute();
					DatabaseHeader resultHeader = new DatabaseHeader(sourceHeader,
						wizard.DimensionsPage.RightList, wizard.CubesPage.RightList,
						wizard.DimensionsPage.LeftList);
					databaseInfo = resultHeader.DatabaseInfo;					
					break;
				default:
					break;
			}
			databaseInfo.server = server;
			databaseInfo.log = log;
			return databaseInfo;
		}

		private DatabaseHeader SaveDatabaseHeaderToFile(DatabaseHeader newHeader)
		{
			newHeader.Navigator = null;
			newHeader.DatabaseInfo.Navigator = null;
			newHeader.ControlBlock.Navigator = null;
			WizardSelectOLAPObjectsAndProperties wizard =
				new WizardSelectOLAPObjectsAndProperties(this, newHeader);
			if (wizard.Execute())
			{
				newHeader.ControlBlock.Cubes.RefreshItems(wizard.CubesPage.RightList);
				newHeader.ControlBlock.Dimensions.RefreshItems(wizard.DimensionsPage.RightList);
				newHeader.ControlBlock.SetExcludedDimensions(wizard.DimensionsPage.LeftList);				
				newHeader.SaveToFile();
				return newHeader;
			}
			return null;
		}

        private void DBToPackage_Click(object sender, ToolClickEventArgs e)
		{
			SaveDatabaseHeaderToFile((DatabaseHeader)DatabaseHeader.Clone());			
		}

        private void DeleteDatabase_Click(object sender, ToolClickEventArgs e)
		{
			if (DatabaseHeader != null && DatabaseHeader.NamedComponent != null)
			{
				if (Messanger.Question(
					string.Format("Вы уверены, что хотите удалить \"{0}\"?",
					DatabaseHeader.NamedComponent.Name)) == DialogResult.Yes)
				{
					Database DB = DatabaseHeader.NamedComponent;
					DB.Drop(DropOptions.IgnoreFailures);
					server.Databases.Remove(DB);
					server.Update(UpdateOptions.Default, UpdateMode.Default);
					DB.Dispose();
					RefreshDatabases();
				}				
			}			
		}

		protected void SetButtonsState(ctrlObjectExplorer explorer, bool enabled, object[] names)
		{
			explorer.InvokeSetButtonState(enabled, names);
		}

		public void ConnectToServer(Microsoft.AnalysisServices.Server _server)
		{
			server = _server;
			RefreshServerDatabases(server.Databases);
			SetButtonsState(ctrlDatabases, true, new object[] { "restore" });
		}

		public void ConnectToVSS()
		{
			if (vss == null)
			{
				vss = new VSSFacade();
			}
			RefreshVSSDatabases(Utils.VSSRepositoryRootLocal +
				Utils.VSSRepositoryWorkingFolder);
			ctrlDatabases.Caption = string.Format(
				"{0} ({1}): {2}", vss.GetDatabaseName(), vss.GetINI(), vss.GetUserName());
			SetButtonsState(ctrlDatabases, true, new object[] { "restore" });
		}


	    public SplitContainer SplitContainer
	    {
	        get { return splitContainer; }
	    }


	    public SplitContainer SplitContainerbababasesPatch
	    {
	        get { return splitContainerbababasesPatch; }
	    }

	    private bool IsPacketExtension(string fileExtension)
		{
			return fileExtension.Equals(".emptypack", StringComparison.OrdinalIgnoreCase) ||
				fileExtension.Equals(".configpack", StringComparison.OrdinalIgnoreCase) ||
				fileExtension.Equals(".fullpack", StringComparison.OrdinalIgnoreCase);
			
		}		

        /*
		public void GetItems(VSSItem itemsRoot)
		{
			IEnumerator items = itemsRoot.get_Items(true).GetEnumerator();
			string itemPath = string.Empty;
			while (items.MoveNext())
			{
				VSSItem item = (VSSItem)items.Current;
				if ((VSSItemType)item.Type == VSSItemType.VSSITEM_FILE && !(item.IsCheckedOut > 0))
				{
					item.Get(ref itemPath, 0);
				}
			}
		}	
         * */
		
		public void RefreshVSSDatabases(string rootDir)
		{	
			List<object> headers = new List<object>();
			string[] items = Directory.GetFiles(rootDir, "*.*pack");
			for (int i = 0; i < items.Length; i++)
			{
				DatabaseHeader header = new DatabaseHeader(items[i], vss);
				Utils.SetVssProperties(items[i], header.VSSInfo, vss);				
				headers.Add(header);
			}
			ctrlDatabases.objectList.RefreshItems(headers);
			if (headers.Count == 0)
			{
				ctrlDatabase.CubesExplorer.objectList.RefreshItems(null);
				ctrlDatabase.DimsExplorer.objectList.RefreshItems(null);
			}
		}

		public void RefreshServerDatabases(DatabaseCollection databases)
		{
			List<object> items = new List<object>(databases.Count);
			for (int i = 0; i < databases.Count; i++)
			{
				DatabaseHeader header = new DatabaseHeader(databases[i]);
				if (header.CanInitObjectInfo) { items.Add(header); }				
			}

			ctrlDatabases.objectList.RefreshItems(items);
			if (databases.Count == 0)			
			{
				ctrlDatabase.CubesExplorer.objectList.RefreshItems(null);
				ctrlDatabase.DimsExplorer.objectList.RefreshItems(null);	
		        ctrlDatabase.DSExplorer.objectList.RefreshItems(null);
			}
		}

		public void RefreshDatabases()
		{
			if (server != null) { RefreshServerDatabases(server.Databases); }			
		}

		public void AddDatabase(DatabaseHeader _header, bool updateList)
		{
			ctrlDatabases.objectList.AddItem(_header, updateList);			
		}

		public void AddDatabase(Database DB)
		{	
			DatabaseHeader header = (DatabaseHeader)ctrlDatabases.objectList.GetByName(DB.Name);
			if (header != null) { Refresh(header); }
			else { AddDatabase(new DatabaseHeader(DB), true); }
		}

		public void AddDatabase(DatabaseHeader _header)
		{
			DatabaseHeader header =
				(DatabaseHeader)ctrlDatabases.objectList.GetByName(_header.Name);
			if (header != null) { Refresh(header); }
			else { AddDatabase(_header, true); }
		}

		public void Refresh(DatabaseHeader header)
		{
			AddCubes(header);
			AddDimensions(header);
		    AddDataSources(header);
		}

	    private void AddDataSources(DatabaseHeader header)
	    {
	        AddObjects(header, "dataSource", DSExplorer);
	    }

	    public void AddCubes(DatabaseHeader header)
		{			
			AddObjects(header, "cube", CubesExplorer);
		}

		public void AddDimensions(DatabaseHeader header)
		{			
			AddObjects(header, "dimension", DimsExplorer);			
		}

		private static void AddObjects(DatabaseHeader header, string objTypeName,
			ctrlObjectExplorer objectExplorer)
		{
			if (header != null)
			{
				if (objTypeName.Equals("cube", StringComparison.OrdinalIgnoreCase))
				{
					objectExplorer.RefreshItems(header.ControlBlock.Cubes.GetItems());
				}
				else if (objTypeName.Equals("dimension", StringComparison.OrdinalIgnoreCase))
				{
					objectExplorer.RefreshItems(header.ControlBlock.Dimensions.GetItems());
				}
                else if (objTypeName.Equals("dataSource", StringComparison.OrdinalIgnoreCase))
				{
                    objectExplorer.RefreshItems(header.ControlBlock.DataSources.GetItems());
				}
			}
			else
			{
				objectExplorer.RefreshItems(null);				
			}
		}

		private void ValueChangedHandler(object sender, EventArgs e)			
		{
			ListBox.SelectedObjectCollection items = ((ListBox)sender).SelectedItems;
			if (items.Count > 0) { Refresh(items[0] as DatabaseHeader); }
			else { Refresh(null); }

            if (ctrlDatabases.UltraToolbarsManager.Tools.Count > 0)
			{
				bool databaseSelected = ctrlDatabases.objectList.SelectedItem != null;
				SetButtonsState(
					ctrlDatabases, databaseSelected, new string[] { "save", "delete", "add", "generate" });
			}			
		}		

		private void ItemsCollectionChangedHandler(object sender, EventArgs e)
		{
			ListBox listBox = sender as ListBox;
			for (int i = 0; i < listBox.Items.Count; i++)
			{				
				if (listBox.Items[i] is ToolStripButton &&
					listBox.Items[i].ToString().
					Equals("save", StringComparison.OrdinalIgnoreCase))
				{
					(listBox.Items[i] as ToolStripButton).Enabled = listBox.Items.Count > 0;
				}
			}
		}

		public DatabaseHeader DatabaseHeader
		{
			get { return ctrlDatabases.objectList.SelectedItem as DatabaseHeader; }
		}

		public ctrlObjectExplorer CubesExplorer
		{
			get { return ctrlDatabase.CubesExplorer; }
		}

		public ctrlObjectExplorer DimsExplorer
		{
			get { return ctrlDatabase.DimsExplorer; }
		}

	    public ctrlObjectExplorer DSExplorer
	    {
            get { return ctrlDatabase.DSExplorer; }
	    }

        /// <summary>
        /// Контрол для работы с патчами
        /// </summary>
	    public ctrlWorkingPatch CtrlWorkingPatch
	    {
	        get { return ctrlWorkingPatch; }
	    }
	}
}
