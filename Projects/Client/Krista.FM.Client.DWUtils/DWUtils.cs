using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using Microsoft.AnalysisServices;
using System.Xml;
using System.IO;
using System.Windows.Forms.Design;

using Krista.FM.Client.OLAPStructures;
using System.ComponentModel;
using Krista.FM.Client.OLAPResources;
using System.Drawing.Design;

namespace Krista.FM.Client.DWUtils
{
	public enum NameCompareMethod
	{
		exact = 0,
		subString = 1,
		soft = 2,
	}

	public class GeneratorSettings
	{
		protected string packageFileName = string.Empty;
		protected string semanticsFileName = @"X:\dotNet\Repository\_Подопытная\Semantics.xml";
		protected string packageDir = string.Empty;
		protected string newObjectsDir = string.Empty;
		protected NameCompareMethod nameCompareMethod = NameCompareMethod.soft;

		[Editor(typeof(PackageFileNameEditor), typeof(UITypeEditor)),
		Browsable(true), Category("Файлы и каталоги"),
		DisplayName("Файл многомерной базы"), ReadOnly(false),
		Description("Те объекты многомерной базы, которые есть в этом файле считаются уже созданными и соответственно генерироваться не будут.")]
		public string PackageFileName
		{
			get { return packageFileName; }
			set { packageFileName = value; }
		}

		[Editor(typeof(XmlFileNameEditor), typeof(UITypeEditor)),
		Browsable(true), Category("Файлы и каталоги"),
		DisplayName("Файл семантики"), ReadOnly(false),
		DefaultValue(@"X:\dotNet\Repository\_Подопытная\Semantics.xml"),
		Description("Файл с семантикой.")]
		public string SemanticsFileName
		{
			get { return semanticsFileName; }
			set { semanticsFileName = value; }
		}

		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor)),
		Browsable(true), Category("Файлы и каталоги"),
		DisplayName("Каталог пакетов"), ReadOnly(false),
		DefaultValue(@"X:\dotNet\Repository\_Подопытная\Packages"),
		Description("Каталог, в котором лежат пакеты хранилища. На основании содержимого этих пакетов генерируются объекты многомерной базы.")]
		public string PackageDir
		{
			get
			{
				if (string.IsNullOrEmpty(packageDir))
				{
					return Path.GetDirectoryName(semanticsFileName) + @"\Packages";
				}
				return packageDir;
				
			}
			set { packageDir = value; }
		}

		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor)),
		Browsable(true), Category("Файлы и каталоги"),
		DisplayName("Каталог для создания папки с новыми объектами"), ReadOnly(false),
		DefaultValue(@"X:\dotNet\Repository"),
		Description("В данном каталоге будет создана папка \"OLAP2005 ГГГГ_ММ_ДД_ччммсс\", где ГГГГ - год, ММ - месяц, ДД - день, чч - часы, мм - минуты, сс - секунды.")]
		public string NewObjectsDir
		{
			get
			{
				if (string.IsNullOrEmpty(newObjectsDir))
				{
					return Path.GetPathRoot(Path.GetDirectoryName(semanticsFileName)) + @"dotNet\Repository";
				}
				return newObjectsDir;
			}
			set { newObjectsDir = value; }
		}

		[Browsable(true), Category("Разное"),
		DisplayName("Метод сравнения имен объектов многомерной базы и хранилища"), ReadOnly(false),
		DefaultValue(NameCompareMethod.soft),
	  Description("У нас нередки случаи, когда один и тот же объект по разному называется в хранилище и в OLAP-базе. Например, \"КД_МесОтч2004\" в хранилище и \"КД_МесОтч_2004\" в OLAP-базе. Метод \"exact\" тупо сравнивает две строки на полное совпадение. Метод \"soft\" сначала удаляет все разделители и только потом сравнивает имена. Однако многие имена настолько разные, что сравнить их нет никакой возможности - поэтому при любом методе сравнения генерируются несколько десятков объектов, которые на самом деле уже существуют...")]
		public NameCompareMethod NameCompareMethod
		{
			get { return nameCompareMethod; }
			set { nameCompareMethod = value; }
		}
	}

	public class OLAPGenerator
	{
		public GeneratorSettings GeneratorSettings = new GeneratorSettings();
		
		public DatabaseHeader dbHeader;
		public Dictionary<string, string> semantics = new Dictionary<string, string>();
		public List<DimensionInfoBase> allDims = new List<DimensionInfoBase>();		

		public void RefreshSemantics(string fileName)
		{
			XPathNavigator navigator = new XPathDocument(fileName).CreateNavigator();
			XPathNodeIterator nodes =
				navigator.Select(".//ServerConfiguration//Semantics//Semantic");
			semantics.Clear();
			while (nodes.MoveNext())
			{
				semantics.Add(nodes.Current.GetAttribute("name", ""),
					nodes.Current.GetAttribute("caption", ""));
			}
		}

		public string GetSemanticCaption(string semanticKey)
		{
			string caption;
			if (!semanticKey.Equals("FX", StringComparison.OrdinalIgnoreCase) &&
				semantics.TryGetValue(semanticKey, out caption))
			{
				return caption + "_";
			}
			return string.Empty;
		}

		public List<DimensionInfoBase> GenerateDimensions(string fileName)
		{
			XPathNavigator navigator = new XPathDocument(fileName).CreateNavigator();
			return GenerateDimensions(navigator.SelectSingleNode(".//Classes"));
		}

		public List<DimensionInfoBase> GenerateDimensions(XPathNavigator classesNav)
		{
			List<DimensionInfoBase> newDims = new List<DimensionInfoBase>();
			XPathNodeIterator classes = classesNav.SelectChildren(XPathNodeType.Element);
			while (classes.MoveNext())
			{
				if (classes.Current.Name.Equals("FixedCls", StringComparison.OrdinalIgnoreCase) ||
					classes.Current.Name.Equals("DataCls", StringComparison.OrdinalIgnoreCase) ||
					classes.Current.Name.Equals("BridgeCls", StringComparison.OrdinalIgnoreCase))
				{
					DimensionInfo2005 newDimInfo = GenerateDimension(classes.Current);
					if (newDimInfo != null) { newDims.Add(newDimInfo); }
				}
			}
			return newDims;
		}

		private void CreateDataSourceLevel(HierarchyInfo hierInfo, string tableName)
		{
			string sourceName = "Источник";
			AttributeInfo levAttr = new AttributeInfo(sourceName);
			levAttr.KeyColumns.Add(new ColumnInfo(tableName, "sourceid"));
			levAttr.NameColumn = new ColumnInfo(tableName, "datasourcename");
			LevelInfo levInfo = new LevelInfo(sourceName);
			levInfo.SourceAttributeID = sourceName;
			hierInfo.Levels.Insert(0, levInfo);

			AttributeInfo memberAttr = new AttributeInfo("ID источника");
			memberAttr.KeyColumns.Add(new ColumnInfo(tableName, "sourceid"));
			memberAttr.NameColumn = new ColumnInfo(tableName, "sourceid");
			levAttr.AddRelationShip(memberAttr);
		}

		public string CreateTableName(string prefix, string semantic, string name)
		{
			return string.Format("{0}_{1}_{2}", prefix, semantic, name).ToUpper();
		}

		public string GetTableName(XPathNavigator navigator, bool prefixByClassAttr,
			out string fullName, out string prefix, out string semantic, out string name)
		{
			if (prefixByClassAttr) { prefix = navigator.GetAttribute("class", ""); }
			else
			{
				if (navigator.Name.Equals("FixedCls", StringComparison.OrdinalIgnoreCase))
					prefix = "fx";
				else
					if (navigator.Name.Equals("DataCls", StringComparison.OrdinalIgnoreCase))
						prefix = "d";
					else
						if (navigator.Name.Equals("BridgeCls", StringComparison.OrdinalIgnoreCase))
							prefix = "b";
						else
							prefix = "f";
			}
			semantic = navigator.GetAttribute("semantic", "");
			name = navigator.GetAttribute("name", "");

			fullName = string.Format("{0}.{1}.{2}", prefix.ToLower(), semantic, name);

			if (prefix != "f" && navigator.GetAttribute("dataSourceParameter", "") != string.Empty)
			{
				prefix += "v";
			}
			return CreateTableName(prefix, semantic, name);
		}

		public string GetTableName(
			XPathNavigator navigator, bool prefixByClassAttr, out string fullName)
		{
			string prefix;
			string semantic;
			string name;
			return GetTableName(
				navigator, prefixByClassAttr, out fullName, out prefix, out semantic, out name);
		}

		public string GetTableName(XPathNavigator navigator,
			bool prefixByClassAttr, out string prefix, out string semantic, out string name)
		{
			string fullName;
			return GetTableName(
				navigator, prefixByClassAttr, out fullName, out prefix, out semantic, out name);
		}

		private bool ItemFound(DualAccessDictionary items, string itemToSearch)
		{
			switch (GeneratorSettings.NameCompareMethod)
			{
				case NameCompareMethod.exact:
					return ItemFoundExact(items, itemToSearch);
				case NameCompareMethod.subString:
					return ItemFoundBySubstring(items, itemToSearch);
				default:
				case NameCompareMethod.soft:
					return ItemFoundBySoftCompare(items, itemToSearch);				
			}
		}

		private bool ItemFoundExact(DualAccessDictionary items, string itemToSearch)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> item in items)
			{
				if (itemToSearch.Equals(item.Value.Name, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private bool ItemFoundBySubstring(DualAccessDictionary items, string itemToSearch)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> item in items)
			{
				if (itemToSearch.StartsWith(item.Value.Name, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private bool ItemFoundBySoftCompare(DualAccessDictionary items, string itemToSearch)
		{
			foreach (KeyValuePair<string, OLAPObjectHeader> item in items)
			{
				string itemName = item.Value.Name.Trim().Replace(" ", string.Empty).Replace("_", string.Empty);
				itemToSearch = itemToSearch.Trim().Replace(" ", string.Empty).Replace("_", string.Empty);
				if (itemToSearch.StartsWith(itemName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public DimensionInfo2005 GenerateDimension(XPathNavigator navigator)
		{
			try
			{
				IdentificationInfo idInfo = ReadIdentificationInfo(navigator);
				if (!ItemFound(dbHeader.ControlBlock.Dimensions, idInfo.Name))
				{
					DimensionInfo2005 dimInfo = new DimensionInfo2005();
					dimInfo.IdentificationInfo = idInfo;
					dimInfo.Description = ReadDescription(navigator);
					dimInfo.DimensionType = ReadDimensionType(navigator);
					dimInfo.AllMemberName = ReadAllMemberName(navigator);

					string fullName;
					string tableName = GetTableName(navigator, false, out fullName);
					ReadRegularHierarchy(navigator, dimInfo, tableName);
					ReadParentChildHierarchy(navigator, dimInfo, tableName);
					ReadAnnotations(dimInfo.Annotations, fullName, navigator);
					return dimInfo;
				}
				return null;
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.Write(e.Message);
				return null;
			}
		}

		private IdentificationInfo ReadIdentificationInfo(XPathNavigator navigator)
		{
			string semanticKey = navigator.GetAttribute("semantic", "");
			string name = GetSemanticCaption(semanticKey) + navigator.GetAttribute("caption", "");
			return new IdentificationInfo(name, Guid.NewGuid());
		}

		private string ReadDescription(XPathNavigator navigator)
		{
			return navigator.GetAttribute("description", "");
		}

		private DimensionType ReadDimensionType(XPathNavigator navigator)
		{
			return DimensionType.Regular;
		}

		private string ReadAllMemberName(XPathNavigator navigator)
		{
			if (navigator.SelectSingleNode(".//Hierarchy//Regular//Level[@all]") != null)
			{
				return navigator.SelectSingleNode(
					".//Hierarchy//Regular//Level[@all]").GetAttribute("all", "");
			}
			if (navigator.SelectSingleNode(".//Hierarchy//ParentChild[@allLevelName]") != null)
			{
				return navigator.SelectSingleNode(
					".//Hierarchy//ParentChild[@allLevelName]").GetAttribute("allLevelName", "");
			}
			return string.Empty;
		}

		private void ReadAnnotations(
			Dictionary<string, AnnotationInfo> annotations, string fullName, XPathNavigator navigator)
		{
			annotations.Add("FullName",
				new AnnotationInfo("FullName", fullName, AnnotationVisibility.SchemaRowset));

			annotations.Add("SubClass", new AnnotationInfo("SubClass",
				navigator.GetAttribute("takeMethod", ""), AnnotationVisibility.SchemaRowset));
		}

		private AttributeInfo ReadLevelAttribute(XPathNavigator navigator,
			string tableName)
		{
			string keyFieldName = navigator.GetAttribute("memberKey", "");
			string nameFieldName = navigator.GetAttribute("memberName", "");
			string name = navigator.GetAttribute("name", "");
			AttributeUsage usage = AttributeUsage.Regular;
			if (keyFieldName.Equals("ID", StringComparison.OrdinalIgnoreCase))
			{
				name = keyFieldName;
				usage = AttributeUsage.Key;
			}
			AttributeInfo attrInfo = new AttributeInfo(name);
			attrInfo.KeyColumns.Add(new ColumnInfo(tableName, keyFieldName));
			attrInfo.NameColumn = new ColumnInfo(tableName, nameFieldName);
			attrInfo.Usage = usage;
			return attrInfo;
		}

		private AttributeInfo CreateKeyAttribute(string tableName)
		{
			AttributeInfo attrInfo = new AttributeInfo("ID");
			attrInfo.KeyColumns.Add(new ColumnInfo(tableName, "ID"));
			attrInfo.NameColumn = new ColumnInfo(tableName, "ID");
			attrInfo.Usage = AttributeUsage.Key;
			return attrInfo;
		}

		private AttributeInfo ReadMemberAttribute(XPathNavigator navigator,
			string tableName)
		{
			string keyFieldName = navigator.GetAttribute("name", "");
			string name = navigator.GetAttribute("caption", "");
			AttributeInfo attrInfo = new AttributeInfo(name);
			attrInfo.KeyColumns.Add(new ColumnInfo(tableName, keyFieldName));
			attrInfo.NameColumn = new ColumnInfo(tableName, keyFieldName);
			return attrInfo;
		}

		private AttributeInfo GetAttributeByName(DimensionInfo2005 dimInfo, string attrName)
		{
			for (int i = 0; i < dimInfo.Attributes.Count; i++)
			{
				if (dimInfo.Attributes[i].SafeName.Equals(
					attrName, StringComparison.OrdinalIgnoreCase))
				{
					return dimInfo.Attributes[i];
				}
			}
			return null;
		}

		private void ReadMemberAttributes(XPathNavigator navigator,
			DimensionInfo2005 dimInfo, string tableName, AttributeInfo parentAttr)
		{
			XPathNodeIterator nodes = navigator.Select(".//Attributes//Attribute");
			AttributeInfo attrInfo;
			while (nodes.MoveNext())
			{
				attrInfo = ReadMemberAttribute(nodes.Current, tableName);
				parentAttr.AddRelationShip(attrInfo);
				dimInfo.Attributes.Add(attrInfo);
			}
			attrInfo = new AttributeInfo("PKID");
			attrInfo.KeyColumns.Add(new ColumnInfo(tableName, "ID"));
			attrInfo.NameColumn = new ColumnInfo(tableName, "ID");
			attrInfo.AttributeHierarchyEnabled = false;
			parentAttr.AddRelationShip(attrInfo);
			dimInfo.Attributes.Add(attrInfo);
		}

		private void ReadRegularHierarchy(
			XPathNavigator navigator, DimensionInfo2005 dimInfo, string tableName)
		{
			XPathNavigator hierNav = navigator.SelectSingleNode(".//Hierarchy//Regular");
			if (hierNav != null)
			{
				HierarchyInfo hierInfo = 
					new HierarchyInfo((IdentificationInfo)dimInfo.IdentificationInfo.Clone());
				hierInfo.AllMemberName = dimInfo.AllMemberName;
				hierInfo.Description = dimInfo.Description;
				hierInfo.ParentChild = false;
				AttributeInfo levAttr;
				LevelInfo levInfo;
				XPathNodeIterator levNodes = hierNav.Select(".//Level");
				while (levNodes.MoveNext())
				{
					levAttr = ReadLevelAttribute(levNodes.Current, tableName);
					dimInfo.Attributes.Add(levAttr);
					levInfo = new LevelInfo(levNodes.Current.GetAttribute("name", ""));
					levInfo.SourceAttributeID = levAttr.SafeName;
					hierInfo.Levels = new List<LevelInfo>();
					hierInfo.Levels.Add(levInfo);
				}
				ReadMemberAttributes(navigator, dimInfo, tableName, GetAttributeByName(
					dimInfo, hierInfo.Levels[hierInfo.Levels.Count - 1].SourceAttributeID));

				if (navigator.GetAttribute("dataSourceParameter", "") != string.Empty)
				{
					CreateDataSourceLevel(hierInfo, tableName);
				}
				dimInfo.Hierarchies.Add(hierInfo);
			}
		}

		private void ReadParentChildHierarchy(
			XPathNavigator navigator, DimensionInfo2005 dimInfo, string tableName)
		{
			XPathNavigator hierNav = navigator.SelectSingleNode(".//Hierarchy//ParentChild");
			if (hierNav != null)
			{
				AttributeInfo levAttr = ReadLevelAttribute(hierNav, tableName);
				dimInfo.Attributes.Add(levAttr);

				levAttr = new AttributeInfo(dimInfo.Name);
				if (navigator.GetAttribute("dataSourceParameter", "") != string.Empty)
				{
					levAttr.KeyColumns.Add(new ColumnInfo(tableName, "cubeParentID"));
					levAttr.NamingTemplate =
						"Источник;" + hierNav.GetAttribute("levelNamingTemplate", "");
				}
				else
				{
					levAttr.KeyColumns.Add(
						new ColumnInfo(tableName, hierNav.GetAttribute("parentKey", "")));
					levAttr.NamingTemplate = hierNav.GetAttribute("levelNamingTemplate", "");
				}
				levAttr.NameColumn = levAttr.KeyColumns[0];
				levAttr.Usage = AttributeUsage.Parent;
				dimInfo.Attributes.Add(levAttr);
				if (dimInfo.KeyAttribute == null)
					dimInfo.KeyAttribute = CreateKeyAttribute(tableName);

				ReadMemberAttributes(navigator, dimInfo, tableName, dimInfo.KeyAttribute);
			}
		}

		public XmlWriterSettings GetXmlWriterSettings(bool omitXmlDeclaration)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = Encoding.UTF8;
			xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "\t";
			xmlWriterSettings.NewLineChars = "\r\n";
			xmlWriterSettings.NewLineOnAttributes = true;
			//xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlWriterSettings.CloseOutput = false;
			xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
			return xmlWriterSettings;
		}

		private XPathNavigator GetNavigatorFromStrBuilder(StringBuilder strBuilder)
		{
			//// Create two different encodings.
			//Encoding utf8 = Encoding.UTF8;
			//Encoding unicode = Encoding.Unicode;

			//// Convert the string into a byte[].
			//byte[] unicodeBytes = unicode.GetBytes(strBuilder.ToString());

			//// Perform the conversion from one encoding to the other.
			//byte[] utf8Bytes = Encoding.Convert(unicode, utf8, unicodeBytes);

			//// Convert the new byte[] into a char[] and then into a string.
			//// This is a slightly different approach to converting to illustrate
			//// the use of GetCharCount/GetChars.
			//char[] utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
			//utf8.GetChars(utf8Bytes, 0,utf8Bytes.Length, utf8Chars, 0);			

			XmlDocument doc = new XmlDocument();
			//doc.LoadXml(new string(utf8Chars));
			doc.LoadXml(strBuilder.ToString());
			if (doc.ChildNodes.Count == 1) { return doc.FirstChild.CreateNavigator(); }
			else { return doc.ChildNodes[1].CreateNavigator(); }
		}

		private void InitWriter(out StringBuilder strings, out XmlWriter writer)
		{
			strings = new StringBuilder(2048);
			writer = XmlWriter.Create(strings, GetXmlWriterSettings(false));
			writer.WriteStartDocument();
		}

		private XPathNavigator CloseWriter(StringBuilder strings, XmlWriter writer)
		{
			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();
			return GetNavigatorFromStrBuilder(strings);
		}

		private void ReadGlobalNavigators(
			out XPathNavigator classesNav, out XPathNavigator associationsNav)
		{
			StringBuilder classesStrings;
			XmlWriter classesWriter;

			StringBuilder associationsStrings;
			XmlWriter associationsWriter;

			InitWriter(out classesStrings, out classesWriter);
			InitWriter(out associationsStrings, out associationsWriter);
			try
			{
				classesWriter.WriteStartElement("Classes");
				associationsWriter.WriteStartElement("Associations");
				DirectoryInfo packDir = new DirectoryInfo(GeneratorSettings.PackageDir);
				FileInfo[] files = packDir.GetFiles("*.xml", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++)
				{
					XPathNavigator currentNav =
						new XPathDocument(files[i].FullName).CreateNavigator();

					XPathNodeIterator nodes = currentNav.Select("//Classes/*");
					while (nodes.MoveNext())
					{
						classesWriter.WriteNode(nodes.Current, true);
					}

					nodes = currentNav.Select("//Associations/*");
					while (nodes.MoveNext())
					{
						associationsWriter.WriteNode(nodes.Current, true);
					}
				}
				classesWriter.WriteEndElement();//Classes
				associationsWriter.WriteEndElement();//Associations				
			}
			finally
			{
				classesNav = CloseWriter(classesStrings, classesWriter);
				associationsNav = CloseWriter(associationsStrings, associationsWriter);
			}
		}

		public List<CubeInfoBase> GenerateCubes(
			XPathNavigator classesNav, XPathNavigator associationsNav)
		{
			List<CubeInfoBase> cubes = new List<CubeInfoBase>();
			XPathNodeIterator classes = classesNav.SelectChildren(XPathNodeType.Element);
			while (classes.MoveNext())
			{
				if (classes.Current.Name.Equals("DataTable", StringComparison.OrdinalIgnoreCase))
				{
					CubeInfo2005 cubeInfo = GenerateCube(classes.Current, associationsNav);
					if (cubeInfo != null) { cubes.Add(cubeInfo); }
				}
			}
			return cubes;
		}

		private List<DimensionInfoBase> GetSharedDimensions(
			string prefix, string semantic, string name)
		{
			//Предполагаем представление и ищем измерения
			string tableName = CreateTableName(prefix + "V", semantic, name);
			List<DimensionInfoBase> sharedDims =
				GetSharedDimensionByTableName(allDims, tableName);

			//Если по представлению ничего не нашли, то ищем по таблице			
			if (sharedDims.Count == 0)
			{
				tableName = CreateTableName(prefix, semantic, name);
				sharedDims =
					GetSharedDimensionByTableName(allDims, tableName);
			}
			return sharedDims;
		}

		private void ProcessBridgeReferences(CubeDimensionInfo2005 cubeDimInfo,
			XPathNavigator associationsNav,
			string dimPrefix, string dimSemantic, string dimName)
		{
			string bridgePrefix;
			string bridgeSemantic;
			string bridgeName;

			string dimTableName = CreateTableName(dimPrefix, dimSemantic, dimName);

			XPathNodeIterator dimsNodes = associationsNav.Select(string.Format(
				"./Data2Bridge[RoleData[@class=\"{0}\" and @semantic=\"{1}\" and @name=\"{2}\"]]",
				dimPrefix, dimSemantic, dimName));

			while (dimsNodes.MoveNext())
			{
				GetTableName(dimsNodes.Current.SelectSingleNode("./RoleBridge"), true,
					out bridgePrefix, out bridgeSemantic, out bridgeName);

				List<DimensionInfoBase> sharedDims =
					GetSharedDimensions(bridgePrefix, bridgeSemantic, bridgeName);

				for (int i = 0; i < sharedDims.Count; i++)
				{
					CubeDimensionInfo2005 cubeDim =
						GetCubeDimension(cubeDimInfo.CubeInfo, sharedDims[i]);

					if (!MeasureGroupDimensionExist(
						cubeDimInfo.CubeInfo.MeasureGroups[0].MGDimensions, cubeDim))
					{
						MeasureGroupDimensionInfo mgDimInfo = new MeasureGroupDimensionInfo(cubeDim);
						mgDimInfo.DimensionType = MeasureGroupDimensionType.ReferenceDimension;
						mgDimInfo.IntermediateDimIdentification = cubeDimInfo.IdentificationInfo;
						mgDimInfo.IntermediateGranularityAttrID =
							dimsNodes.Current.GetAttribute("name", "");
						mgDimInfo.ReferenceToThis.Add(new ColumnInfo(
							dimTableName + "." + dimsNodes.Current.GetAttribute("name", "")));
						cubeDimInfo.CubeInfo.MeasureGroups[0].MGDimensions.Add(mgDimInfo);
					}
				}
			}
		}

		private CubeDimensionInfo2005 GetCubeDimension(
			CubeInfoBase cubeInfo, DimensionInfoBase sharedDim)
		{
			for (int i = 0; i < cubeInfo.Dimensions.Count; i++)
			{
				if (cubeInfo.Dimensions[i].ID.Equals(sharedDim.ID))
					return cubeInfo.Dimensions[i] as CubeDimensionInfo2005;
			}
			CubeDimensionInfo2005 cubeDim = new CubeDimensionInfo2005(cubeInfo, sharedDim);
			cubeInfo.Dimensions.Add(cubeDim);
			return cubeDim;
		}

		private bool MeasureGroupDimensionExist(
			List<MeasureGroupDimensionInfo> dimensions, CubeDimensionInfoBase cubeDim)
		{
			for (int i = 0; i < dimensions.Count; i++)
			{
				if (dimensions[i].CubeDimIdentification.ID.Equals(cubeDim.ID)) { return true; }
			}
			return false;
		}

		private void ReadMeasureGroupDimensions(
			CubeInfo2005 cubeInfo, MeasureGroupInfo mgInfo,
			XPathNavigator associationsNav, string prefix, string semantic, string name)
		{
			string dimPrefix;
			string dimSemantic;
			string dimName;

			string cubeTableName = CreateTableName(prefix, semantic, name);

			XPathNodeIterator dimsNodes = associationsNav.Select(string.Format(
				"./Reference[RoleData[@class=\"{0}\" and @semantic=\"{1}\" and @name=\"{2}\"]]",
				prefix, semantic, name));
			while (dimsNodes.MoveNext())
			{
				GetTableName(dimsNodes.Current.SelectSingleNode("./RoleBridge"), true,
					out dimPrefix, out dimSemantic, out dimName);

				List<DimensionInfoBase> sharedDims =
					GetSharedDimensions(dimPrefix, dimSemantic, dimName);
				for (int i = 0; i < sharedDims.Count; i++)
				{
					CubeDimensionInfo2005 cubeDim = GetCubeDimension(cubeInfo, sharedDims[i]);

					if (!MeasureGroupDimensionExist(mgInfo.MGDimensions, cubeDim))
					{
						MeasureGroupDimensionInfo mgDimInfo = new MeasureGroupDimensionInfo(cubeDim);
						mgDimInfo.DimensionType = MeasureGroupDimensionType.RegularDimension;
						mgDimInfo.ReferenceToThis.Add(new ColumnInfo(
							cubeTableName + "." + dimsNodes.Current.GetAttribute("name", "")));
						mgInfo.MGDimensions.Add(mgDimInfo);

						ProcessBridgeReferences(
							cubeDim, associationsNav, dimPrefix, dimSemantic, dimName);
					}
				}
			}
		}

		private void ReadMeasureGroups(CubeInfo2005 cubeInfo, XPathNavigator navigator,
			XPathNavigator associationsNav, string fullName, string prefix, string semantic, string name)
		{
			MeasureGroupInfo mgInfo = new MeasureGroupInfo(cubeInfo.Name, Guid.NewGuid());
			cubeInfo.MeasureGroups.Add(mgInfo);
			ReadMeasureGroupDimensions(cubeInfo, mgInfo, associationsNav, prefix, semantic, name);
			ReadMeasures(mgInfo, navigator, CreateTableName(prefix, semantic, name));
			ReadAnnotations(mgInfo.Annotations, fullName, navigator);
		}

		private void ReadMeasures(
			MeasureGroupInfo mgInfo, XPathNavigator navigator, string cubeTableName)
		{
			XPathNodeIterator measures = navigator.Select(".//Attributes//Attribute");
			while (measures.MoveNext())
			{
				MeasureInfo measureInfo =
					new MeasureInfo(measures.Current.GetAttribute("caption", ""), Guid.NewGuid());
				measureInfo.SourceColumn = new ColumnInfo(
					cubeTableName + "." + measures.Current.GetAttribute("name", ""));
				mgInfo.Measures.Add(measureInfo);
			}
		}

		private List<DimensionInfoBase> GetSharedDimensionByTableName(
			List<DimensionInfoBase> dimensions, string tableName)
		{
			List<DimensionInfoBase> foundDims = new List<DimensionInfoBase>();
			for (int i = 0; i < dimensions.Count; i++)
			{
				if (dimensions[i].KeyAttribute != null &&
					dimensions[i].KeyAttribute.KeyColumns[0].TableName.Equals(
					tableName, StringComparison.CurrentCultureIgnoreCase))
				{
					foundDims.Add(dimensions[i]);
				}
			}
			return foundDims;
		}		

		private string ReadCubeName(XPathNavigator navigator)
		{
			string cubeName = navigator.GetAttribute("shortCaption", "");
			if (string.IsNullOrEmpty(cubeName))
			{
				return navigator.GetAttribute("caption", "");
			}
			return cubeName.Split(
				new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
		}

		private void ProcessDataSources(CubeInfo2005 cubeInfo, XPathNavigator navigator,
			string prefix, string semantic, string name)
		{
			if (!string.IsNullOrEmpty(navigator.GetAttribute("dataSourceParameter", "")))
			{
				DimensionHeader dataSourceDimHeader =
					dbHeader.ControlBlock.Dimensions["Источники данных"] as DimensionHeader;
				if (dataSourceDimHeader != null)
				{
					CubeDimensionInfo2005 cubeDim =
						new CubeDimensionInfo2005(cubeInfo, dataSourceDimHeader.DimensionInfo);
					cubeInfo.Dimensions.Add(cubeDim);

					string cubeTableName = CreateTableName(prefix, semantic, name);
					MeasureGroupDimensionInfo mgDimInfo = new MeasureGroupDimensionInfo(cubeDim);
					mgDimInfo.DimensionType = MeasureGroupDimensionType.RegularDimension;
					mgDimInfo.ReferenceToThis.Add(new ColumnInfo(cubeTableName + ".SourceID"));
					cubeInfo.MeasureGroups[0].MGDimensions.Add(mgDimInfo);
				}
			}
		}

		private void GeneratePartition(CubeInfo2005 cubeInfo)
		{
			PartitionInfo partitionInfo = new PartitionInfo();
			partitionInfo.identificationInfo = new IdentificationInfo(cubeInfo.Name, cubeInfo.Name);
			partitionInfo.AggregationPrefix =
				cubeInfo.Name + "_" + partitionInfo.identificationInfo.Name;
			partitionInfo.ProcessingMode = ProcessingMode.Regular;
			partitionInfo.ProcessingPriority = 0;
			partitionInfo.Slice = "";
			partitionInfo.Source = new TableInfo(
				cubeInfo.MeasureGroups[0].Measures[0].SourceColumn.TableName, "");
			partitionInfo.StorageMode = StorageMode.Molap;
			cubeInfo.MeasureGroups[0].Partitions.Add(partitionInfo);
		}

		private CubeInfo2005 GenerateCube(
			XPathNavigator navigator, XPathNavigator associationsNav)
		{
			try
			{
				string cubeName = ReadCubeName(navigator);				
				if (!ItemFound(dbHeader.ControlBlock.Cubes, cubeName))				
				{
					string prefix;
					string semantic;
					string name;
					string fullName;

					GetTableName(navigator, false, out fullName, out prefix, out semantic, out name);

					CubeInfo2005 cubeInfo = new CubeInfo2005();
					cubeInfo.IdentificationInfo = new IdentificationInfo(cubeName, Guid.NewGuid());
					cubeInfo.Description = ReadDescription(navigator);

					cubeInfo.MDXScripts.Add(new MDXScriptInfo());
					cubeInfo.MDXScripts[0].Calculations.Add(
						new CalculationInfo(string.Empty, "CALCULATE;", string.Empty));

					ReadMeasureGroups(
						cubeInfo, navigator, associationsNav, fullName, prefix, semantic, name);
					ProcessDataSources(cubeInfo, navigator, prefix, semantic, name);

					GeneratePartition(cubeInfo);
					return cubeInfo;
				}
				return null;
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.Write(e.Message);
				return null;
			}
		}		

		public void ReadGlobalPackage(GeneratorSettings generatorSettings,
			out List<DimensionInfoBase> newDims, out List<CubeInfoBase> newCubes)
		{
			if (generatorSettings != null)
			{
				GeneratorSettings = generatorSettings;
			}			
			dbHeader = new DatabaseHeader(generatorSettings.PackageFileName);
			XPathNavigator classesNav;
			XPathNavigator associationsNav;
			ReadGlobalNavigators(out classesNav, out associationsNav);
			RefreshSemantics(GeneratorSettings.SemanticsFileName);

			newDims = GenerateDimensions(classesNav);
			allDims.AddRange(dbHeader.DatabaseInfo.Dimensions);
			allDims.AddRange(newDims);

			newCubes = GenerateCubes(classesNav, associationsNav);
		}
	}
}