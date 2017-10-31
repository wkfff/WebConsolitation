using Krista.FM.Client.OLAPStructures.Editors;
using Microsoft.AnalysisServices;
using System.Xml;
using System.Xml.XPath;
using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Design;
using Krista.FM.Client.OLAPResources;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.OLAPStructures
{
	public class ControlBlock : ObjectInfo
	{
		protected UpdateMode updateMode = UpdateMode.CreateOrReplace;
		protected bool readFromFile = true;

		public ControlBlock()
		{ }

		public ControlBlock(UpdateMode _updateMode)
		{
			updateMode = _updateMode;
		}

		public ControlBlock(UpdateMode _updateMode, bool _readFromFile)
		{
			updateMode = _updateMode;
			readFromFile = _readFromFile;
		}

		[Browsable(false), CategoryAttribute("Разное"),
		DisplayName("Режим обновления"),
		DescriptionAttribute("Режим обновления."),
		ReadOnlyAttribute(false)]
		public UpdateMode UpdateMode
		{
			get { return updateMode; }
			set { updateMode = value; }
		}

		[Browsable(false)]
		public bool ReadFromFile
		{
			get { return readFromFile; }
			set { readFromFile = value; }
		}		

		protected override void WriteToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("controlblock");
			xmlWriter.WriteAttributeString("updatemode", updateMode.ToString());
			xmlWriter.WriteAttributeString("readfromfile", readFromFile.ToString());
			xmlWriter.WriteEndElement();//controlblock
		}

		public static ControlBlock ReadFromXML(XPathNavigator navigator)
		{
			ControlBlock controlBlock = new ControlBlock();
			controlBlock.readFromFile = Convert.ToBoolean(navigator.GetAttribute("readfromfile", ""));
			controlBlock.updateMode = (UpdateMode)Enum.Parse(typeof(UpdateMode),
				navigator.GetAttribute("updatemode", ""));
			return controlBlock;
		}

		public static ControlBlock ReadFromAnnotations(AnnotationCollection annotations)
		{
			Annotation annotation = annotations.Find("controlblock");
			if (annotation != null)
				return ControlBlock.ReadFromXML(annotation.Value.CreateNavigator());
			return null;
		}
	
		[Browsable(false)]
		public override string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
			}
		}

		[Browsable(false)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}
	}

	public class DatabaseControlBlock : ControlBlock
	{
		protected FileInfo fileInfo;
		//protected XPathNavigator navigator;

		protected DualAccessDictionary dimsHeaders;
		protected DualAccessDictionary cubesHeaders;
        protected DualAccessDictionary dsHeaders;

		protected PackageFormat packageFormat = PackageFormat.EmptyPack;

		protected bool createDimensionsRelations = true;
		protected bool createCubesRelations = false;

		protected string excludedDimensions = string.Empty;
		protected string excludedCubes = string.Empty;

		protected IVSSFacade vss;		

		protected void Init(string fileName, IVSSFacade _vss)
		{
			vss = _vss;
			FileName = fileName;
			navigator = new XPathDocument(fileName).CreateNavigator().
				SelectSingleNode(".//controlblock");
			readFromFile = Convert.ToBoolean(navigator.GetAttribute("readfromfile", ""));
			updateMode = (UpdateMode)Enum.Parse(typeof(UpdateMode),
				navigator.GetAttribute("updatemode", ""));

			createDimensionsRelations =
				Convert.ToBoolean(navigator.GetAttribute("dimensionsrelations", ""));
			createCubesRelations =
				Convert.ToBoolean(navigator.GetAttribute("cubesrelations", ""));

			dimsHeaders = new DualAccessDictionary();
			cubesHeaders = new DualAccessDictionary();
            dsHeaders = new DualAccessDictionary();

			ReadHeaders(OlapHeaderType.dimension, dimsHeaders);
			ReadHeaders(OlapHeaderType.cube, cubesHeaders);
            ReadHeaders(OlapHeaderType.dataSource, dsHeaders);
		}

		public DatabaseControlBlock()
			: base()
		{ }

		public DatabaseControlBlock(UpdateMode _updateMode)
			: base(_updateMode)
		{ }

		public DatabaseControlBlock(UpdateMode _updateMode, bool _readFromFile)
			: base(_updateMode, _readFromFile)
		{ }

		public DatabaseControlBlock(string fileName, IVSSFacade vss)
			: base()
		{
			Init(fileName, vss);
		}

		public DatabaseControlBlock(string fileName)
			: base()
		{
			Init(fileName, null);
		}

		public DatabaseControlBlock(Database DB)
			: base()
		{
			dimsHeaders = new DualAccessDictionary();
			cubesHeaders = new DualAccessDictionary();
            dsHeaders = new DualAccessDictionary();

			GetDimensionHeadersFromDB(DB);
			GetCubeHeadersFromDB(DB);
		    GetDataSourcesFromDB(DB);
		}

		public DatabaseControlBlock(DatabaseControlBlock source, List<object> dimsNames,
			List<object> cubesNames, List<object> excludedDimsNames)
			: base()
		{
			updateMode = source.UpdateMode; ;
			readFromFile = source.readFromFile;

			fileInfo = source.fileInfo;
			navigator = source.navigator;
			packageFormat = source.packageFormat;

			dimsHeaders = source.Dimensions.GetFiltredHeaders(dimsNames);
			cubesHeaders = source.Cubes.GetFiltredHeaders(cubesNames);
			if (excludedDimsNames != null)
				SetExcludedDimensions(source.Dimensions.GetFiltredHeaders(excludedDimsNames));
		}

		public DatabaseControlBlock(DatabaseControlBlock source)
			: base()
		{
			updateMode = source.UpdateMode; ;
			readFromFile = source.readFromFile;

			fileInfo = source.fileInfo;
			navigator = source.navigator;
			packageFormat = source.packageFormat;

			dimsHeaders = source.Dimensions;
			cubesHeaders = source.Cubes;			
		}

		private void GetDimensionHeadersFromDB(Database DB)
		{
			for (int i = 0; i < DB.Dimensions.Count; i++)
				dimsHeaders.AddHeader(new DimensionHeader(DB.Dimensions[i]));
		}

		private void GetCubeHeadersFromDB(Database DB)
		{
			for (int i = 0; i < DB.Cubes.Count; i++)
				cubesHeaders.AddHeader(new CubeHeader(DB.Cubes[i]));
		}	
	
        private void GetDataSourcesFromDB(Database DB)
        {
            for (int i = 0; i < DB.DataSources.Count; i++)
                dsHeaders.AddHeader(new DataSourceHeader(DB.DataSources[i]));
        }

		new public static DatabaseControlBlock ReadFromXML(XPathNavigator navigator)
		{
			return new DatabaseControlBlock();
		}

		private void ReadHeaders(OlapHeaderType headerType, DualAccessDictionary headers)
		{
			headers.Clear();
			if (readFromFile) { ReadHeadersFromFile(headerType, headers); }
			else { ReadHeadersFromPackage(headerType, headers); }
		}

		private static ControlBlock NewControlBlock(OlapHeaderType headerType)
		{
			if (headerType == OlapHeaderType.cube)
				return new CubeControlBlock(UpdateMode.CreateOrReplace, true);
			else
				return new DimensionControlBlock(UpdateMode.CreateOrReplace, true);
		}

		private static ControlBlock ReadControlBlock(XPathNavigator navigator, OlapHeaderType headerType)
		{
			if (headerType == OlapHeaderType.cube)
				return CubeControlBlock.ReadFromXML(navigator);
			else
				return DimensionControlBlock.ReadFromXML(navigator);
		}

		private void ReadHeadersFromFile(OlapHeaderType headerType, DualAccessDictionary headers)
		{
			string dirName = DirName(headerType);
			string[] files = OLAPUtils.GetFilesByMask(dirName, "*.fm" + headerType.ToString());

			ControlBlock controlBlock = NewControlBlock(headerType);
			for (int i = 0; i < files.Length; i++)
			{
				try
				{
					if (!string.IsNullOrEmpty(files[i]))
					{
						AddHeaders(headerType, controlBlock, headers, new XPathDocument(
							files[i]).CreateNavigator().SelectSingleNode(".//" + headerType.ToString()), files[i]);
					}					
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine(
						string.Format("Ошибка при добавлении объекта {0}: ", files[i]), e.Message);
				}
			}
		}		

		private void AddHeaders(OlapHeaderType headerType, ControlBlock controlBlock,
			DualAccessDictionary headers, XPathNavigator navigator, string filePath)
		{
			string id =
				navigator.SelectSingleNode("//" + headerType.ToString()).GetAttribute("id", "");
			if (!headers.ContainsKey(id))
			{
				OLAPObjectHeader header = null;				
				if (headerType == OlapHeaderType.cube)
				{
					header = new CubeHeader(navigator, controlBlock as CubeControlBlock);					
				}
				else
				{
					header = new DimensionHeader(navigator, controlBlock as DimensionControlBlock);
				}
				headers.AddHeader(header);
				if (vss != null && !string.IsNullOrEmpty(filePath))
				{
					OLAPResources.Utils.SetVssProperties(filePath, header.VSSInfo, vss);
				}
			}
		}

		private static bool CheckID(OlapHeaderType headerType, string fileName, string idToCheck)
		{
			XPathNavigator navigator = new XPathDocument(fileName).
				CreateNavigator().SelectSingleNode("//" + headerType.ToString());
			string id = navigator.GetAttribute("id", "");
			return id.Equals(idToCheck, StringComparison.OrdinalIgnoreCase);
		}

		private XPathNavigator GetNavigatorFromFile(OlapHeaderType headerType, string id, string name)
		{
			string dirName = DirName(headerType);
			//Пытаемся прочитать файл с именем объекта
			string[] files = OLAPUtils.GetFilesByMask(dirName, name + ".fm" + headerType.ToString());
			if (files.Length > 0 && CheckID(headerType, files[0], id))
			{
				XPathNavigator nav = new XPathDocument(files[0]).CreateNavigator();
				nav.MoveToFirstChild();
				return nav;
			}

			//Если не нашли файл с именем объекта,
			//то читаем все файлы в каталоге и начинаем сканировать их ID...
			files = OLAPUtils.GetFilesByMask(dirName, "*.fm" + headerType.ToString());
			for (int i = 0; i < files.Length; i++)
			{
				if (CheckID(headerType, files[i], id))
				{
					XPathNavigator nav = new XPathDocument(files[i]).CreateNavigator();
					nav.MoveToFirstChild();
					return nav;					
				}
			}
			return null;
		}

		private void ReadHeadersFromPackage(OlapHeaderType headerType, DualAccessDictionary headers)
		{
			XPathNavigator objNavigator;
			OLAPObjectHeader header;
			ControlBlock controlBlock;
			XPathNodeIterator nodes = navigator.Select(
					string.Format(".//declaration//{0}headers//{0}header", headerType.ToString()));
			while (nodes.MoveNext())
			{
				string id = nodes.Current.SelectSingleNode(
					".//" + headerType.ToString()).GetAttribute("id", "");
				if (headers.ContainsKey(id)) { continue; }

				controlBlock =
					ReadControlBlock(nodes.Current.SelectSingleNode(".//controlblock"), headerType);

				string name = nodes.Current.SelectSingleNode(
					".//" + headerType.ToString()).GetAttribute("name", "");

				if (controlBlock.ReadFromFile)
					objNavigator = GetNavigatorFromFile(headerType, id, name);
				else
					objNavigator = navigator.SelectSingleNode(
						string.Format(".//content//{0}s//{0}[@id=\"{1}\"]", headerType.ToString(), id));

				if (headerType == OlapHeaderType.cube)
					header = new CubeHeader(objNavigator, controlBlock as CubeControlBlock);
				else
					header =
						new DimensionHeader(objNavigator, controlBlock as DimensionControlBlock);

				headers.AddHeader(header);
			}
		}

		public List<object> GetUsedDimensions(List<object> cubesHeaders)
		{
			List<object> usedDimensions = new List<object>();			
			for (int i = 0; i < cubesHeaders.Count; i++)
			{	
				CubeHeader cubeHeader = cubesHeaders[i] as CubeHeader;
				if (cubeHeader == null) { continue; }				
				for (int j = 0; j < cubeHeader.CubeInfo.Dimensions.Count; j++)
				{
					string dimName = Dimensions.TryGetNameByID(cubeHeader.CubeInfo.Dimensions[j].DimensionID);
					if (!string.IsNullOrEmpty(dimName) && !usedDimensions.Contains(Dimensions[dimName]))
					{
						usedDimensions.Add(Dimensions[dimName]);
					}
				}
			}			
			return usedDimensions;
		}

		private void SaveEmptyDeclaration(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("declaration");
			xmlWriter.WriteEndElement();//declaration
		}

		private void SaveEmptyContent(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("content");
			xmlWriter.WriteEndElement();//content
		}

		public void SaveDatabaseContentToFiles()
		{
			DirectoryInfo dimensionsDir = Directory.CreateDirectory(DimensionsDirName);
			DirectoryInfo cubesDir = Directory.CreateDirectory(CubesDirName);
			XmlWriterSettings writerSettings = OLAPUtils.GetXmlWriterSettings(false);

			foreach (KeyValuePair<string, OLAPObjectHeader> item in dimsHeaders.ItemsByName)
				item.Value.SaveToFile(DimensionsDirName, writerSettings);
				//item.Value.SaveToFile(dimensionsDir.Name, writerSettings);

			foreach (KeyValuePair<string, OLAPObjectHeader> item in cubesHeaders.ItemsByName)
				item.Value.SaveToFile(CubesDirName, writerSettings);
				//item.Value.SaveToFile(cubesDir.Name, writerSettings);
		}

		private void SaveDatabaseDeclaration(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("declaration");

			xmlWriter.WriteStartElement("dimensionheaders");
			foreach (KeyValuePair<string, OLAPObjectHeader> item in dimsHeaders.ItemsByName)
				item.Value.SaveHeader(xmlWriter);
			xmlWriter.WriteEndElement();//dimensionheaders

			xmlWriter.WriteStartElement("cubeheaders");
			foreach (KeyValuePair<string, OLAPObjectHeader> item in cubesHeaders.ItemsByName)
				item.Value.SaveHeader(xmlWriter);
			xmlWriter.WriteEndElement();//cubesheaders

			xmlWriter.WriteEndElement();//declaration
		}

		private void SaveDatabaseContent(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("content");

			xmlWriter.WriteStartElement("dimensions");
			foreach (KeyValuePair<string, OLAPObjectHeader> item in dimsHeaders.ItemsByName)
				item.Value.SaveContent(xmlWriter);
			xmlWriter.WriteEndElement();//dimensions

			xmlWriter.WriteStartElement("cubes");
			foreach (KeyValuePair<string, OLAPObjectHeader> item in cubesHeaders.ItemsByName)
				item.Value.SaveContent(xmlWriter);
			xmlWriter.WriteEndElement();//cubes

			xmlWriter.WriteEndElement();//content
		}

		protected override void WriteToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("controlblock");
			xmlWriter.WriteAttributeString("updatemode", updateMode.ToString());
			xmlWriter.WriteAttributeString("readfromfile", readFromFile.ToString());
			xmlWriter.WriteAttributeString("dimensionsrelations", createDimensionsRelations.ToString());
			xmlWriter.WriteAttributeString("cubesrelations", createCubesRelations.ToString());
			switch (packageFormat)
			{
				case PackageFormat.EmptyPack:
					SaveEmptyDeclaration(xmlWriter);
					SaveEmptyContent(xmlWriter);
					SaveDatabaseContentToFiles();
					break;
				case PackageFormat.ConfigPack:
					SaveDatabaseDeclaration(xmlWriter);
					SaveEmptyContent(xmlWriter);
					break;
				case PackageFormat.FullPack:
					SaveDatabaseDeclaration(xmlWriter);
					SaveDatabaseContent(xmlWriter);
					break;
				default:
					break;
			}
			xmlWriter.WriteEndElement();//controlblock
		}

		public override object Clone()
		{
			DatabaseControlBlock clonedObject = (DatabaseControlBlock)base.Clone();
			if (this.dimsHeaders != null)
			{
				clonedObject.dimsHeaders = (DualAccessDictionary)this.dimsHeaders.Clone();
			}
			if (this.cubesHeaders != null)
			{
				clonedObject.cubesHeaders = (DualAccessDictionary)this.cubesHeaders.Clone();
			}
			if (this.fileInfo != null)
			{
				clonedObject.fileInfo = new FileInfo(this.fileInfo.FullName);
			}
			return clonedObject;
		}

		[BrowsableAttribute(true), CategoryAttribute("Имена файлов и каталогов"),
		DisplayName("Имя файла"),
		DefaultValueAttribute("Рабочая многомерная база"),
		DescriptionAttribute("Имя файла, в который будет сохранен пакет."),
		EditorAttribute(typeof(PackageFileNameEditor), typeof(UITypeEditor)),
		EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor)),
		ReadOnlyAttribute(false)]
		public string FileName
		{
			get
			{
				if (fileInfo != null) { return fileInfo.FullName; }
				return string.Empty;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					FileInfo newFileInfo = new FileInfo(value);
					PackageFormat format = packageFormat;
					if (!string.IsNullOrEmpty(newFileInfo.Extension))
					{
						try
						{
							//пытаемся получить формат пакета из расширения
							//при этом не забываем отсекать точку
							format = (PackageFormat)Enum.Parse(
								typeof(PackageFormat), newFileInfo.Extension.Substring(1), true);
							PackageFormat = format;
						}
						catch (Exception)
						{
							format = packageFormat;
						}
						value = value.Substring(0, value.LastIndexOf("."));
					}
					fileInfo = new FileInfo(value + "." + format.ToString().ToLowerInvariant());
				}
			}
		}

		[BrowsableAttribute(true), CategoryAttribute("Разное"),
		DisplayName("Формат пакета"),
		DefaultValueAttribute(PackageFormat.EmptyPack),
		DescriptionAttribute("Формат пакета."),
		ReadOnlyAttribute(false)]
		public PackageFormat PackageFormat
		{
			get { return packageFormat; }
			set
			{
				packageFormat = value;
				if (fileInfo != null)
				{					
					fileInfo = new FileInfo(FileName.Remove(FileName.IndexOf(fileInfo.Extension)) +
						"." + packageFormat.ToString().ToLowerInvariant());
				}
				switch (packageFormat)
				{	
					case PackageFormat.EmptyPack:
						readFromFile = true;
						Cubes.SetReadFromFile(true);
						Dimensions.SetReadFromFile(true);
						break;
					case PackageFormat.ConfigPack:
						readFromFile = false;
						Cubes.SetReadFromFile(true);
						Dimensions.SetReadFromFile(true);
						break;
					case PackageFormat.FullPack:
						readFromFile = false;
						Cubes.SetReadFromFile(false);
						Dimensions.SetReadFromFile(false);
						break;
					default:
						break;
				}				
			}
		}

		[BrowsableAttribute(true), CategoryAttribute("Имена файлов и каталогов"),
		DisplayName("Каталог измерений"),
		DescriptionAttribute("Каталог, в который будут сохранены измерения."),
		ReadOnlyAttribute(true)]
		public string DimensionsDirName
		{
			get
			{
				if (fileInfo == null) { return "Не задан файл пакета"; }
				else
					switch (packageFormat)
					{
						case PackageFormat.FullPack:
							return fileInfo.FullName;
						case PackageFormat.Package2000:
						case PackageFormat.EmptyPack:
						case PackageFormat.ConfigPack:
						default:
							return fileInfo.DirectoryName + "\\Dimensions";
					}
			}
		}

		[BrowsableAttribute(true), CategoryAttribute("Имена файлов и каталогов"),
		DisplayName("Каталог кубов"),
		DescriptionAttribute("Каталог, в который будут сохранены кубы."),
		ReadOnlyAttribute(true)]
		public string CubesDirName
		{
			get
			{
				if (fileInfo == null) { return "Не задан файл пакета"; }
				else
					switch (packageFormat)
					{
						case PackageFormat.FullPack:
							return fileInfo.FullName;
						case PackageFormat.Package2000:
						case PackageFormat.EmptyPack:
						case PackageFormat.ConfigPack:
						default:
							return fileInfo.DirectoryName + "\\Cubes";
					}
			}
		}

		public string DirName(OlapHeaderType headerType)
		{
			if (headerType == OlapHeaderType.cube) { return CubesDirName; }
			else { return DimensionsDirName; }
		}

		public void SetExcludedDimensions(DualAccessDictionary excludedDims)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> item in cubesHeaders.ItemsByName)
			{
				(item.Value as CubeHeader).SetExcludedDimensions(excludedDims);
			}
		}

		public void SetExcludedDimensions(List<object> excludedDimHeaders)
		{	
			foreach (KeyValuePair<string, OLAPObjectHeader> item in cubesHeaders.ItemsByName)
			{
				(item.Value as CubeHeader).SetExcludedDimensions(excludedDimHeaders);
			}
		}

		[Browsable(false)]
		public DualAccessDictionary Dimensions
		{
			get { return dimsHeaders; }
		}

		[Browsable(false)]
		public DualAccessDictionary Cubes
		{
			get { return cubesHeaders; }
		}

        [Browsable(false)]
	    public DualAccessDictionary DataSources
	    {
            get { return dsHeaders; }
	    }

		[Browsable(false)]
		public FileInfo FileInfo
		{
			get { return fileInfo; }
		}
		
		[Browsable(true), Category("Отношения между таблицами"),
		DisplayName("Создавать для измерений"),
		Description("Создавать отношения для измерений на уровне представления данных (DataSourceView)."),
		ReadOnly(true)]
		public bool CreateDimensionsRelations
		{
			get { return createDimensionsRelations; }
			set { createDimensionsRelations = value; }
		}

		[Browsable(true), Category("Отношения между таблицами"),
		DisplayName("Создавать для кубов"),
		Description("Создавать отношения для кубов на уровне представления данных (DataSourceView). В четыре раза увелививает время восстановления многомерной базы.")]
		public bool CreateCubesRelations
		{
			get { return createCubesRelations; }
			set { createCubesRelations = value; }
		}		
	}

    public class DataSourceControlBlock : ControlBlock
    {
        private readonly DataSourceProperty dataSourceProperty;

        public  DataSourceControlBlock (NamedComponent namedComponent)
        {
            this.namedComponent = namedComponent;
            dataSourceProperty = new DataSourceProperty((DataSource)namedComponent);
        }

        public DataSourceProperty DataSourceProperty
        {
            get { return dataSourceProperty; }
        }

        public DataSourceControlBlock()
			: base()
        {
        }

		public DataSourceControlBlock(UpdateMode _updateMode)
			: base(_updateMode)
		{ }

        public DataSourceControlBlock(UpdateMode _updateMode, bool _readFromFile)
			: base(_updateMode, _readFromFile)
		{ }

    }

    [Editor(typeof(ConnectionStringEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Подключение в БД")]
    [Category("Настройки подключения")]
    [Description("Если при создании базы данных был изменен пароль пользователя dv, то необходимо настроить подключение к БД. Это подключение будет использоваться при применении обновлений на многомерную базу.")]
    public class DataSourceProperty
    {
        private readonly DataSource dataSource;
        public DataSourceProperty(DataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        [Browsable(false)]
        public DataSource DataSourceComponent
        {
            get { return (DataSource)dataSource; }
        }

        public override string ToString()
        {
            return dataSource.ConnectionString;
        }
    }

	public class DimensionControlBlock : ControlBlock
	{
		public DimensionControlBlock()
			: base()
		{ }

		public DimensionControlBlock(UpdateMode _updateMode)
			: base(_updateMode)
		{ }

		public DimensionControlBlock(UpdateMode _updateMode, bool _readFromFile)
			: base(_updateMode, _readFromFile)
		{ }

		new public static DimensionControlBlock ReadFromXML(XPathNavigator navigator)
		{
			DimensionControlBlock controlBlock = new DimensionControlBlock();
			controlBlock.readFromFile = Convert.ToBoolean(navigator.GetAttribute("readfromfile", ""));
			controlBlock.updateMode = (UpdateMode)Enum.Parse(typeof(UpdateMode),
				navigator.GetAttribute("updatemode", ""));
			return controlBlock;
		}

		new public static ControlBlock ReadFromAnnotations(AnnotationCollection annotations)
		{
			Annotation annotation = annotations.Find("controlblock");
			if (annotation != null)
				return DimensionControlBlock.ReadFromXML(annotation.Value.CreateNavigator());
			return null;
		}
	}

	public class CubeControlBlock : ControlBlock
	{
		//так как список исключаемых измерений задается для всего пакета,
		//то в этом списке могут быть измерения, которых по факту в кубе и так нет
		protected IdentificationList excludedDimensions = new IdentificationList();

		public CubeControlBlock()
			: base()
		{ }

		public CubeControlBlock(UpdateMode _updateMode)
			: base(_updateMode)
		{ }

		public CubeControlBlock(UpdateMode _updateMode, bool _readFromFile)
			: base(_updateMode, _readFromFile)
		{ }

		public CubeControlBlock(
			UpdateMode _updateMode, bool _readFromFile, IdentificationList _excludedDimensions)
			: base(_updateMode, _readFromFile)
		{
			excludedDimensions = _excludedDimensions;
		}

		public IdentificationList ExcludedDimensions
		{
			get { return excludedDimensions; }
		}

		protected override void WriteToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("controlblock");
			xmlWriter.WriteAttributeString("updatemode", updateMode.ToString());
			xmlWriter.WriteAttributeString("readfromfile", readFromFile.ToString());
			ExcludedDimensions.ToXML(xmlWriter, "excludeddimensions", "excludeddimension");
			xmlWriter.WriteEndElement();//controlblock
		}

		new public static CubeControlBlock ReadFromXML(XPathNavigator navigator)
		{
			CubeControlBlock controlBlock = new CubeControlBlock();
			controlBlock.readFromFile = Convert.ToBoolean(navigator.GetAttribute("readfromfile", ""));
			controlBlock.updateMode = (UpdateMode)Enum.Parse(typeof(UpdateMode),
				navigator.GetAttribute("updatemode", ""));
			controlBlock.excludedDimensions = IdentificationList.ReadFromXML(
				navigator.SelectSingleNode("excludeddimensions"), "excludeddimension");
			return controlBlock;
		}

		new public static CubeControlBlock ReadFromAnnotations(AnnotationCollection annotations)
		{
			Annotation annotation = annotations.Find("controlblock");
			if (annotation != null)
				return CubeControlBlock.ReadFromXML(annotation.Value.CreateNavigator());
			return null;
		}
	}
}