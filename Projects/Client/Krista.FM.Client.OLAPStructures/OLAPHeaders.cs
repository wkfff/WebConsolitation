using System;
using System.Collections.Generic;
using Infragistics.Win.IGControls;
using Microsoft.AnalysisServices;
using System.Xml.XPath;
using System.ComponentModel;
using System.Xml;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Collections;
using Krista.FM.Client.OLAPResources;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.OLAPStructures
{
	public enum OlapHeaderType
	{
		unknown = -1,
		cube = 0,
		dimension = 1,
		database = 2,
        /// <summary>
        /// Источник данных
        /// </summary>
        dataSource = 3,
        /// <summary>
        /// Представление данных
        /// </summary>
        dataSourceView = 4
	}


	public class OLAPObjectHeader : ICloneable
	{
		protected IdentificationInfo idInfo;

		//Заголовок будем создавать либо из навигатора
		protected XPathNavigator navigator;
		//либо из объекта многомерной базы
		protected NamedComponent namedComponent;

		protected ObjectInfo objectInfo;
		protected OlapHeaderType headerType;
		protected ControlBlock controlBlock;

		protected VSSInfo vssInfo = new VSSInfo();

		public virtual void InitObjectInfo()
		{
			if (objectInfo == null)
			{
				try
				{

					if (navigator != null)
					{
						switch (headerType)
						{
							case OlapHeaderType.cube:
								objectInfo = new CubeInfo2005(navigator, ControlBlock as CubeControlBlock);
								break;
							case OlapHeaderType.dimension:
								objectInfo =
									new DimensionInfo2005(navigator, ControlBlock as DimensionControlBlock);
								break;
							case OlapHeaderType.database:
								objectInfo =
									new DatabaseInfo2005(navigator, ControlBlock as DatabaseControlBlock);
								break;
							default:
								break;
						}
					}
					else
					{
						switch (headerType)
						{
							case OlapHeaderType.cube:
								objectInfo = new CubeInfo2005(
									namedComponent as Cube, ControlBlock as CubeControlBlock);
								break;
							case OlapHeaderType.dimension:
								objectInfo = new DimensionInfo2005(
									namedComponent as Dimension, ControlBlock as DimensionControlBlock);
								break;
							case OlapHeaderType.database:
								objectInfo = new DatabaseInfo2005(
									namedComponent as Database, ControlBlock as DatabaseControlBlock);
								break;
							default:
								break;
						}
					}
				    //InitContextMenu();
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine(e.Message);
				}
			}
		}

        /// <summary>
        /// Инициализация контекстного меню объекта заголовка
        /// </summary>
        /*
	    private void InitContextMenu()
	    {
            objectInfo.contextMenu.MenuItems.Clear();

            Type t = objectInfo.GetType();
            string currentDeclaringType = String.Empty;
	        foreach (System.Reflection.MethodInfo methodInfo in t.GetMethods())
            {
                if (String.IsNullOrEmpty(currentDeclaringType))
                    currentDeclaringType = methodInfo.DeclaringType.FullName;

                foreach (object customAttribute in methodInfo.GetCustomAttributes(typeof(MenuActionAttribute), true))
                {
                    MenuActionAttribute attr = (MenuActionAttribute)customAttribute;
                    string caption = attr.Caption;
                    if (String.IsNullOrEmpty(caption))
                        caption = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;

                    IGMenuItem menuItem = new IGMenuItem(caption, new EventHandler(this.OnContextMenuItemClick));
                    menuItem.Name = methodInfo.Name;
                    menuItem.Tag = objectInfo;

                    objectInfo.contextMenu.MenuItems.Add(menuItem);
                    break;
                }
            }
	    }
         */ 

	    private void OnContextMenuItemClick(object sender, EventArgs e)
	    {
            IGMenuItem menuItem = sender as IGMenuItem;

            ObjectInfo objInfo = menuItem != null ? menuItem.Tag as ObjectInfo : null;
            if (objInfo != null)
            {
                try
                {
                    objInfo.GetType().InvokeMember(menuItem.Name, System.Reflection.BindingFlags.InvokeMethod, null, objInfo, null);
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp.InnerException);
                }
            }
	    }


	    private OlapHeaderType GetHeaderType()
		{
			if (navigator != null)
			{
				if (navigator.Name.Equals("database", StringComparison.OrdinalIgnoreCase))
					return OlapHeaderType.database;
				if (navigator.Name.Equals("cube", StringComparison.OrdinalIgnoreCase))
					return OlapHeaderType.cube;
				if (navigator.Name.Equals("dimension", StringComparison.OrdinalIgnoreCase))
					return OlapHeaderType.dimension;
			}
			else
			{
				if (namedComponent is Database) return OlapHeaderType.database;
				if (namedComponent is Cube) return OlapHeaderType.cube;
				if (namedComponent is Dimension) return OlapHeaderType.dimension;
			}
			return OlapHeaderType.unknown;
		}

		protected void Init(XPathNavigator objNavigator, ControlBlock _controlBlock)
		{
			navigator = objNavigator;
			namedComponent = null;
			controlBlock = _controlBlock;
			headerType = GetHeaderType();
			idInfo = IdentificationInfo.ReadFromXML(navigator);
		}

		protected void Init(NamedComponent _namedComponent, ControlBlock _controlBlock)
		{
			idInfo = new IdentificationInfo(_namedComponent.Name, _namedComponent.ID);
			navigator = null;
			namedComponent = _namedComponent;
			headerType = GetHeaderType();
			if (_controlBlock != null) { controlBlock = _controlBlock; }
			else { controlBlock = NewControlBlock(headerType); }
		}

		protected ControlBlock NewControlBlock(OlapHeaderType headerType)
		{
			switch (headerType)
			{
				case OlapHeaderType.cube:
					return new CubeControlBlock();
				case OlapHeaderType.dimension:
					return new DimensionControlBlock();
				case OlapHeaderType.database:
					return new DatabaseControlBlock();
                case OlapHeaderType.dataSource:
			        return new DataSourceControlBlock();
				default:
					return new DimensionControlBlock();
			}
		}

		public OLAPObjectHeader(XPathNavigator _navigator, ControlBlock _controlBlock)
		{
			Init(_navigator, _controlBlock);
		}

		public OLAPObjectHeader(string _fileName, ControlBlock _controlBlock)
		{
			Init(new XPathDocument(_fileName).CreateNavigator(), _controlBlock);
		}

		public OLAPObjectHeader(NamedComponent _namedComponent)
		{
			Init(_namedComponent, null);
		}

		public OLAPObjectHeader(NamedComponent _namedComponent, ControlBlock _controlBlock)
		{
			Init(_namedComponent, _controlBlock);
		}

		public OLAPObjectHeader(OLAPObjectHeader source, ControlBlock _controlBlock)
		{
			if (source.ObjectInfo != null)
			{
				idInfo = (IdentificationInfo)source.ObjectInfo.IdentificationInfo.Clone();
			}
			else
			{
				idInfo = (IdentificationInfo)source.idInfo.Clone();
			}
			navigator = source.navigator;
			namedComponent = source.namedComponent;
			//objectInfo = source.objectInfo;
			headerType = source.headerType;
			controlBlock = _controlBlock;
		}

		public OLAPObjectHeader(ObjectInfo _objectInfo)
		{
			objectInfo = _objectInfo;
			navigator = objectInfo.Navigator;
			namedComponent = objectInfo.NamedComponent;
			headerType = GetHeaderType();
			controlBlock = objectInfo.ControlBlock;
		}		

		[Browsable(false)]
		public OlapHeaderType HeaderType
		{
			get { return headerType; }
		}

		public virtual void SaveHeader(XmlWriter xmlWriter)
		{ }

		public virtual void SaveContent(XmlWriter xmlWriter)
		{
			InitObjectInfo();
			objectInfo.ToXML(xmlWriter);			
		}

		protected virtual string GetFileName(string directoryName)
		{
			string fileName = directoryName + "\\" + Name;
			if (headerType == OlapHeaderType.cube) { return fileName + ".fmcube"; }
			return fileName + ".fmdimension";
		}

		public virtual void SaveToFile(string directoryName, XmlWriterSettings writerSettings)
		{
			XmlWriter xmlWriter = XmlWriter.Create(GetFileName(directoryName), writerSettings);
			xmlWriter.WriteStartDocument();
			try
			{
				InitObjectInfo();
                if (objectInfo != null)
                {
                    objectInfo.ToXML(xmlWriter);
                    xmlWriter.WriteEndDocument();
                }
			}
			finally
			{
				xmlWriter.Flush();
				xmlWriter.Close();
			}
		}

		[Browsable(false)]
		public NamedComponent NamedComponent
		{
			get { return namedComponent; }
		}

		[Browsable(false)]
		public XPathNavigator Navigator
		{
			get { return navigator; }
			set { navigator = value; }
		}

		public override string ToString()
		{
			return idInfo.Name;
		}

		[Browsable(false)]
		public ControlBlock ControlBlock
		{
			get { return controlBlock; }
		}

		[Browsable(false)]
		public virtual bool CanInitObjectInfo
		{
			get { return true; }
		}

		[Browsable(true)]
		public ObjectInfo ObjectInfo
		{
			get
			{
				if (CanInitObjectInfo && objectInfo == null) { InitObjectInfo(); }
				return objectInfo;
			}
		}

		[Browsable(true)]
		public VersionedObjectInfo VersionedObjectInfo
		{
			get
			{
				if (ObjectInfo != null && ObjectInfo is VersionedObjectInfo)
				{
					return ObjectInfo as VersionedObjectInfo;
				}
				else
				{
					return null;
				}
			}
		}

		[Browsable(true)]
		public virtual bool ObjectInfoInitialized
		{
			get
			{
				if (CanInitObjectInfo)
				{
					return objectInfo != null;					
				}
				return false;				
			}
		}		

		[BrowsableAttribute(true), CategoryAttribute("Идентификаторы"),
		DisplayName("Идентификатор объекта"),
			//DefaultValueAttribute("Анализ и планирование"),
		DescriptionAttribute("Идентификатор, под которым будет восстановлен объект."),
		ReadOnlyAttribute(false)]
		public string ID
		{
			get { return idInfo.ID; }
		}

		[BrowsableAttribute(true), CategoryAttribute("Идентификаторы"),
		DisplayName("Имя объекта"),
			//DefaultValueAttribute("Анализ и планирование"),
		DescriptionAttribute("Имя, под которым будет восстановен объект."),
		ReadOnlyAttribute(false)]
		public string Name
		{
			get { return idInfo.Name; }
			set { idInfo.Name = value; }
		}

		[BrowsableAttribute(false), CategoryAttribute("Управляющая информация"),
		DisplayName("Читать из файла"),
		DescriptionAttribute("Для многомерной базы определяет откуда читать кубы и измерения."),
		ReadOnlyAttribute(false)]
		public bool ReadFromFile
		{
			get { return controlBlock.ReadFromFile; }
		}

		[BrowsableAttribute(false), CategoryAttribute("Управляющая информация"),
		DisplayName("Режим обновления"),
		DescriptionAttribute("Режим обновления объекта в многомерной базе."),
		ReadOnlyAttribute(false)]
		public UpdateMode UpdateMode
		{
			get { return controlBlock.UpdateMode; }
			set { controlBlock.UpdateMode = value; }
		}		

		public VSSInfo VSSInfo
		{
			get { return vssInfo; }			
		}		

		#region ICloneable Members

		public virtual object Clone()
		{
			OLAPObjectHeader clonedObject = (OLAPObjectHeader)this.MemberwiseClone();
			if (this.idInfo != null)
			{
				clonedObject.idInfo = (IdentificationInfo)this.idInfo.Clone();
			}
			if (this.navigator != null)
			{
				clonedObject.navigator = (XPathNavigator)this.navigator.Clone();
			}
			if (this.controlBlock != null)
			{
				clonedObject.controlBlock = (ControlBlock)this.controlBlock.Clone();
			}			
			return clonedObject;
		}

		#endregion
	}

	public class CubeHeader : OLAPObjectHeader
	{
		public CubeHeader(string _fileName)
			: base(_fileName, new CubeControlBlock())
		{ }

		public CubeHeader(XPathNavigator _navigator)
			: base(_navigator, new CubeControlBlock())
		{ }

		public CubeHeader(XPathNavigator _navigator, CubeControlBlock _cubeControlBlock)
			: base(_navigator, _cubeControlBlock)
		{ }

		public CubeHeader(NamedComponent _namedComponent)
			: base(_namedComponent)
		{ }

		public override void SaveHeader(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("cubeheader");
			xmlWriter.WriteStartElement("cube");
			idInfo.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//cube

			ControlBlock.ToXML(xmlWriter);

			xmlWriter.WriteEndElement();//cubeheader
		}

		protected override string GetFileName(string directoryName)
		{
			return directoryName + "\\" + Name + ".fmcube";
		}		
		
		public void SetExcludedDimensions(DualAccessDictionary items)
		{
			ExcludedDimensions.Clear();
			foreach (KeyValuePair<string, OLAPObjectHeader> item in items.ItemsByName)
				ExcludedDimensions.Add(new IdentificationInfo(item.Value.Name, item.Value.ID));
		}

		public void SetExcludedDimensions(List<object> items)
		{
			ExcludedDimensions.Clear();
			for (int i = 0; i < items.Count; i++)
			{	
				ExcludedDimensions.Add(new IdentificationInfo(
					(items[i] as OLAPObjectHeader).Name, (items[i] as OLAPObjectHeader).ID));
			}
		}		

		public IdentificationList ExcludedDimensions
		{
			get { return ControlBlock.ExcludedDimensions; }
		}

		[Browsable(false)]
		public CubeInfo2005 CubeInfo
		{
			get { return ObjectInfo as CubeInfo2005; }
		}

		[Browsable(false)]
		new public CubeControlBlock ControlBlock
		{
			get { return controlBlock as CubeControlBlock; }
		}
	}

	public class DimensionHeader : OLAPObjectHeader
	{
		public DimensionHeader(XPathNavigator _navigator)
			: base(_navigator, new DimensionControlBlock())
		{ }

		public DimensionHeader(XPathNavigator _navigator, DimensionControlBlock _dimensionControlBlock)
			: base(_navigator, _dimensionControlBlock)
		{ }

		public DimensionHeader(NamedComponent _namedComponent)
			: base(_namedComponent)
		{ }

		public DimensionHeader(DimensionInfoBase dimInfo)
			: base(dimInfo)
		{ }

		public override void SaveHeader(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("dimensionheader");
			xmlWriter.WriteStartElement("dimension");
			idInfo.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//dimension

			ControlBlock.ToXML(xmlWriter);

			xmlWriter.WriteEndElement();//dimensionheader
		}

		protected override string GetFileName(string directoryName)
		{
			return directoryName + "\\" + Name + ".fmdimension";
		}

		public DimensionInfo2005 DimensionInfo
		{
			get { return ObjectInfo as DimensionInfo2005; }
		}
	}

    public class DataSourceHeader : OLAPObjectHeader
    {
        public DataSourceHeader(XPathNavigator _navigator)
            : base (_navigator, new DataSourceControlBlock())
        {
        }

        public DataSourceHeader(XPathNavigator _navigator, ControlBlock _controlBlock) 
            : base(_navigator, _controlBlock)
        {
        }

        public DataSourceHeader(string _fileName, ControlBlock _controlBlock) 
            : base(_fileName, _controlBlock)
        {
        }

        public DataSourceHeader(NamedComponent _namedComponent) 
            : base(_namedComponent, new DataSourceControlBlock(_namedComponent))
        {
        }

        public DataSourceHeader(NamedComponent _namedComponent, ControlBlock _controlBlock) : base(_namedComponent, _controlBlock)
        {
        }

        public DataSourceHeader(OLAPObjectHeader source, ControlBlock _controlBlock) : base(source, _controlBlock)
        {
        }

        public DataSourceHeader(ObjectInfo _objectInfo) 
            : base(_objectInfo)
        {
        }
    }

	[DefaultProperty("FileName")]
	public class DatabaseHeader : OLAPObjectHeader
	{
		[Browsable(false)]
		new public DatabaseControlBlock ControlBlock
		{
			get { return base.ControlBlock as DatabaseControlBlock; }
		}

		public DatabaseHeader(XPathNavigator _navigator)
			: base(_navigator, new CubeControlBlock())
		{ }

		public DatabaseHeader(XPathNavigator _navigator, DatabaseControlBlock _databaseControlBlock)
			: base(_navigator, _databaseControlBlock)
		{ }

		public DatabaseHeader(Database DB)
			: base(DB, new DatabaseControlBlock(DB))
		{ }

		public DatabaseHeader(Database DB, DatabaseControlBlock _databaseControlBlock)
			: base(DB, _databaseControlBlock)
		{ }

		public DatabaseHeader(string fileName)
			: base(new XPathDocument(fileName).CreateNavigator().SelectSingleNode(".//database"),
			new DatabaseControlBlock(fileName))
		{ }

		public DatabaseHeader(string fileName, IVSSFacade vss)
			: base(new XPathDocument(fileName).CreateNavigator().SelectSingleNode(".//database"),
			new DatabaseControlBlock(fileName, vss))
		{ }

		private void SetExcludedDimensions(DualAccessDictionary excludedDims)
		{
			ControlBlock.SetExcludedDimensions(excludedDims);
		}

		public DatabaseHeader(DatabaseHeader source, List<object> dimHeaders,
			List<object> cubeHeaders, List<object> excludedDimHeaders)
			: base(source,
			new DatabaseControlBlock(source.ControlBlock, dimHeaders, cubeHeaders, excludedDimHeaders))
		{
			DatabaseInfo.IdentificationInfo =
				(IdentificationInfo)source.DatabaseInfo.IdentificationInfo.Clone();
			DatabaseInfo.ConnectionString = source.DatabaseInfo.ConnectionString;
			DatabaseInfo.Description = source.Description;
		}

		//public DatabaseHeader(DatabaseHeader source)
		//    : base(source, new DatabaseControlBlock(source.ControlBlock))
		//{	
		//    //DatabaseInfo.IdentificationInfo = source.DatabaseInfo.IdentificationInfo.Clone();
		//    //DatabaseInfo.ConnectionString = source.DatabaseInfo.ConnectionString;
		//}

		public void SaveToFile()
		{
			XmlWriter xmlWriter = XmlWriter.Create(
				ControlBlock.FileInfo.FullName, OLAPUtils.GetXmlWriterSettings(false));
			xmlWriter.WriteStartDocument();
			try
			{
				xmlWriter.WriteStartElement("databaseheader");
				DatabaseInfo.ToXML(xmlWriter);
				ControlBlock.ToXML(xmlWriter);

				xmlWriter.WriteEndElement();//databaseheader
			}
			finally
			{
				xmlWriter.WriteEndDocument();
				xmlWriter.Close();
			}
		}

		public void SaveToFile(bool withHeader)
		{
			if (withHeader)
			{
				SaveToFile();
			}
			else
			{
				ControlBlock.SaveDatabaseContentToFiles();
			}
		}

		private ControlBlock ReadControlBlock(XPathNavigator navigator, OlapHeaderType headerType)
		{
			switch (headerType)
			{
				case OlapHeaderType.cube:
					return CubeControlBlock.ReadFromXML(navigator);
				case OlapHeaderType.dimension:
					return DimensionControlBlock.ReadFromXML(navigator);
				default:
					return CubeControlBlock.ReadFromXML(navigator);
			}
		}

		public void ReadOLAPObjects()
		{ }

		public override object Clone()
		{
			DatabaseHeader clonedObject = (DatabaseHeader)base.Clone();
			if (this.objectInfo != null)
			{
				clonedObject.objectInfo = (DatabaseInfo2005)this.objectInfo.Clone();
			}
			return clonedObject;
		}

		[Browsable(false)]
		public override bool CanInitObjectInfo
		{
			get
			{
				if (NamedComponent == null)
				{
					return true;
				}
				/*else
				{
					bool FMADatabase = false;
					if (NamedComponent.Annotations.Contains("fmadatabase"))
					{
						FMADatabase = Convert.ToBoolean(
							NamedComponent.Annotations["fmadatabase"].Value.InnerText);
					}
					return FMADatabase;
				}*/
			    return true;
			}
		}

		[BrowsableAttribute(true), CategoryAttribute("Разное"),
		DisplayName("Исходная база данных"),
		ReadOnlyAttribute(true)]
		new public Database NamedComponent
		{
			get { return namedComponent as Database; }
		}

		[BrowsableAttribute(true), CategoryAttribute("Разное"),
		DisplayName("Описание базы данных"),
		DefaultValueAttribute("Рабочая многомерная база"),
		DescriptionAttribute("Описание многомерной базы данных."),
		ReadOnlyAttribute(false)]
		public string Description
		{
			get { return DatabaseInfo.Description; }
			set { DatabaseInfo.Description = value; }
		}		

		[TypeConverter(typeof(ParamsConverter)),
		Editor(typeof(DictionaryTextEditor), typeof(UITypeEditor)),
	    BrowsableAttribute(true), CategoryAttribute("Хранилище"),
	    DisplayName("Строка подключения к хранилищу"),	    
	    ReadOnlyAttribute(false)]
		public Dictionary<string, string> ConnectionParams
		{
			get { return DatabaseInfo.DataSource.ConnectionParams; }
			set { DatabaseInfo.DataSource.ConnectionParams = value; }
		}
		
		[Browsable(false)]
		public DatabaseInfo2005 DatabaseInfo
		{
			get { return ObjectInfo as DatabaseInfo2005; }
		}

		public override bool ObjectInfoInitialized
		{
			get
			{	
				InitObjectInfo();
				return base.ObjectInfoInitialized;
			}
		}
	}	

	public class DualAccessDictionary: IList, ICloneable
	{
		protected SortedDictionary<string, OLAPObjectHeader> itemsByName;
		protected Dictionary<string, string> namesByID;
		protected List<OLAPObjectHeader> items;
		protected IList itemsList;

		public DualAccessDictionary()
		{
			itemsByName = new SortedDictionary<string, OLAPObjectHeader>();
			namesByID = new Dictionary<string, string>();
			items = new List<OLAPObjectHeader>();
			itemsList = (IList)items;
		}

		//public DualAccessDictionary(Dictionary<string, OLAPObjectHeader> _itemsByID)
		//{
		//    itemsByName = new SortedDictionary<string, OLAPObjectHeader>();
		//    namesByID = new Dictionary<string, string>();
		//    foreach (KeyValuePair<string, OLAPObjectHeader> item in _itemsByID)
		//        AddHeader(item.Value);
		//}

		//public DualAccessDictionary(SortedDictionary<string, OLAPObjectHeader> _itemsByName,
		//    Dictionary<string, string> _namesByID)
		//{
		//    itemsByName = _itemsByName;
		//    namesByID = _namesByID;
		//}

		public void AddHeader(OLAPObjectHeader header)
		{
			itemsByName.Add(header.Name, header);
			namesByID.Add(header.ID, header.Name);
		}

		public void AddHeader(object header)
		{
			if (header is OLAPObjectHeader)
			{
				AddHeader((OLAPObjectHeader) header);
			}			
		}

		public void Clear()
		{
			itemsByName.Clear();
			namesByID.Clear();
		}

		public bool ContainsKey(string id)
		{
			return namesByID.ContainsKey(id);
		}

		public bool ContainsName(string name)
		{
			return itemsByName.ContainsKey(name);
		}

		public bool SubstringOfName(string name)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> item in itemsByName)
			{
				if (name.StartsWith(item.Value.Name, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool SoftCompare(string name)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> item in itemsByName)
			{
				string itemName = item.Value.Name.Trim().Replace(" ", string.Empty).Replace("_", string.Empty);
				name = name.Trim().Replace(" ", string.Empty).Replace("_", string.Empty);
				if (name.StartsWith(itemName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public string TryGetNameByID(string id)
		{
			string dimName;
			namesByID.TryGetValue(id, out dimName);
			return dimName;
		}

		public string GetNameByID(string id)
		{
			return namesByID[id];
		}

		public OLAPObjectHeader this[string name]
		{
			get { return itemsByName[name]; }
			set { itemsByName[name] = value; }
		}		

		public List<object> GetItems()
		{	
			List<object> headers = new List<object>();
			IEnumerator headersEnumerator = itemsByName.Values.GetEnumerator();
			while (headersEnumerator.MoveNext())
				headers.Add(headersEnumerator.Current);
			return headers;
		}

		public DualAccessDictionary GetFiltredHeaders(List<object> names)
		{
			DualAccessDictionary headers = new DualAccessDictionary();
			for (int i = 0; i < names.Count; i++)
				headers.AddHeader(this[names[i].ToString()]);
			return headers;
		}

		public DualAccessDictionary GetFiltredHeaders(ListBox.SelectedObjectCollection names)
		{
			DualAccessDictionary headers = new DualAccessDictionary();
			for (int i = 0; i < names.Count; i++)
				headers.AddHeader(this[names[i].ToString()]);
			return headers;
		}

		public List<object> GetFiltredHeaders(Krista.FM.ServerLibrary.VSSFileStatus getWithThis,
			bool includeNewFiles)
		{
			List<object> filtredItems = new List<object>();
			foreach (KeyValuePair<string, OLAPObjectHeader> item in itemsByName)
			{
				if (includeNewFiles && item.Value.VSSInfo.NewFile)
				{
					filtredItems.Add(item.Value);
				}
				else
				{
					if (item.Value.VSSInfo.VSSState == getWithThis)
					{
						filtredItems.Add(item.Value);
					}
				}
			}
			return filtredItems;
		}


		public SortedDictionary<string, OLAPObjectHeader> ItemsByName
		{
			get { return itemsByName; }
		}

		public Dictionary<string, string> NamesByID
		{
			get { return namesByID; }
		}

		public void SetReadFromFile(bool readFromFile)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> header in itemsByName)
			{
				header.Value.ControlBlock.ReadFromFile = readFromFile;
			}
		}

		public void ResetItems()
		{
			itemsByName.Clear();
			namesByID.Clear();
		}

		public void RefreshItems(List<object> items)
		{
			ResetItems();
			for (int i = 0; i < items.Count; i++)
			{
				AddHeader(items[i]);
			}
		}

		#region IList Members

		public int Add(object value)
		{
			return itemsList.Add(value);
		}

		public bool Contains(object value)
		{
			return itemsList.Contains(value);
		}

		public int IndexOf(object value)
		{	
			return itemsList.IndexOf(value);			
		}

		public void Insert(int index, object value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsFixedSize
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool IsReadOnly
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public void Remove(object value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RemoveAt(int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public object this[int index]
		{
			get
			{	
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			((ICollection)itemsByName).CopyTo(array, index);
		}

		public int Count
		{
			get { return ((ICollection)itemsByName).Count; }
		}

		public bool IsSynchronized
		{
			get { return ((ICollection)itemsByName).IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return ((ICollection)itemsByName).SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return itemsByName.GetEnumerator();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			DualAccessDictionary clonedObject = (DualAccessDictionary)this.MemberwiseClone();
			clonedObject.itemsByName = new SortedDictionary<string, OLAPObjectHeader>(this.itemsByName);
			clonedObject.namesByID = new Dictionary<string, string>(this.namesByID);
			return clonedObject;
		}

		#endregion
	}
}