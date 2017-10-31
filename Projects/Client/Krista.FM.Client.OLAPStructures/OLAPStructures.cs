using System;
using System.Windows.Forms;
using Infragistics.Win.IGControls;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.OLAPStructures.Properties;
using Microsoft.AnalysisServices;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Data;
using System.Xml;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Drawing.Design;
using Krista.FM.Client.OLAPResources;
using Binding=Microsoft.AnalysisServices.Binding;

namespace Krista.FM.Client.OLAPStructures
{
	public enum Language
	{
		English = 0,
		Russian = 1049,
	}

    /// <summary>
    /// Смешанные типы таблиц из хранилища и представления источника данных.
    /// </summary>
	public enum TableType
	{
		table = 0,
		view = 1,		
        //namedQuery = 2,
	}

	public class TableComparer : IComparer<TableInfo>
	{
		public int Compare(TableInfo x, TableInfo y)
		{
			if (x == null && y == null) { return 0; }
			if (x == null && y != null) { return -1; }
			if (x != null && y == null) { return 1; }

			return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
		}
	}

    /// <summary>
    /// Хранит словарь структур TableInfo. Ключами служат имена таблиц.
    /// </summary>
    public sealed class TableDictionary
    {   
        //Словарь, где ключом служит имя таблицы.
        private SortedDictionary<string, TableInfo> tableDictionaryByName = new SortedDictionary<string, TableInfo>();
        
        /// <summary>
        /// Если в словаре еще нет TableInfo с таким же запросом - TableInfo в словарь.
        /// </summary>
        /// <param name="table">TableInfo, которую необходимо добавить.</param>
        public bool AddTable(TableInfo table)
        {            
            if (table.TableType == TableType.view)
            {                
                //Если у нас NamedQuery, то добавляем и таблицы, на которой оно построено, хотя это и не обязательно...
                foreach (string tableName in table.TableNameDictionary.Keys)
                {                    
                    if (!tableDictionaryByName.ContainsKey(tableName))
                    {
                        InternalAddTable(new TableInfo(tableName));
                    }
                }
            }
            return InternalAddTable(table);
        }

        /// <summary>
        /// Если в словаре еще нет TableInfo с таким же запросом - добавляет TableInfo в словарь.
        /// </summary>
        /// <param name="columnInfo">Столбец, TableInfo которого необходимо добавить.</param>
        public bool AddTable(ColumnInfo columnInfo)
        {            
            return AddTable(columnInfo.Table);
        }

        /// <summary>
        /// Если TableInfo НЕ содержится хотя бы в одном из словарей, то добавляет TableInfo в оба словаря.
        /// </summary>
        /// <param name="tableInfo">TableInfo, которую надо добавить.</param>
        /// <returns>True, если добавили.</returns>
        private bool InternalAddTable(TableInfo tableInfo)
        {            
            if (tableDictionaryByName.ContainsKey(tableInfo.Name) &&
                String.IsNullOrEmpty(tableInfo.Expression))
            {
                return false;
            }

            tableDictionaryByName.Add(tableInfo.Name + tableInfo.ExpressionName, tableInfo);
            return true;
        }

        /// <summary>
        /// Удаляет все таблицы из словаря.
        /// </summary>
        public void Clear()
        {            
            tableDictionaryByName.Clear();            
        }        

        /// <summary>
        /// Возвращает true, если список таблиц содержит только одну таблицу, причем ее имя совпадает с искомым.
        /// </summary>
        /// <param name="tableName">Имя искомой таблицы.</param>
        /// <returns>True, если список таблиц содержит только одну таблицу, причем ее имя совпадает с искомым.</returns>
        public bool IsOnlyTable(string tableName)
        {
            return (tableDictionaryByName.Count == 1) & tableDictionaryByName.ContainsKey(tableName);
        }

        public IEnumerable<TableInfo> Tables
        {
            get { return this.tableDictionaryByName.Values; }
        }

        /// <summary>
        /// Количество таблиц.
        /// </summary>
        public int Count
        {
            get { return tableDictionaryByName.Count; }
        }
    }

	public class AnnotationInfo
	{
		public XmlNode Value;
		public AnnotationVisibility Visibility = AnnotationVisibility.SchemaRowset;

		private void Init(string nodeName, string nodeValue, AnnotationVisibility nodeVisibility)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(string.Format("<{0}>{1}</{0}>", nodeName, nodeValue));
			Value = doc.FirstChild;
			Visibility = nodeVisibility;
		}		

		public AnnotationInfo(XmlNode node, AnnotationVisibility nodeVisibility)
		{
			Value = node;
			Visibility = nodeVisibility;
		}

		public AnnotationInfo(string nodeName, string nodeValue, AnnotationVisibility nodeVisibility)
		{
			Init(nodeName, nodeValue, nodeVisibility);
		}

		public AnnotationInfo(string nodeName, string nodeValue)
		{
			Init(nodeName, nodeValue, AnnotationVisibility.SchemaRowset);
		}

		public AnnotationInfo(Annotation ann)
		{
            try
            {
                Value = ann.Value;
                Visibility = ann.Visibility;

                if (Value != null)
                {
                    if (Value.Attributes != null)
                    {
                        XmlAttribute xmlns = Value.Attributes["xmlns"];
                        if (xmlns != null)
                        {
                            Value.Attributes.Remove(xmlns);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("При создании аннотации произошла ошибка" + e.Message);
            }
		}

		public string Name
		{
			get { return Value.Name; }
		}

		//public static AnnotationInfo ReadFromXML(XPathNavigator annotationNavigator)
		//{
		//    AnnotationInfo annotationInfo = new AnnotationInfo(annotationNavigator.Name,
		//        annotationNavigator.InnerXml, AnnotationVisibility.SchemaRowset);
		//    return annotationInfo;
		//}

		public static AnnotationInfo ReadFromXML(XPathNavigator annotationNavigator)
		{
			string name = annotationNavigator.SelectSingleNode("./name").Value;
			AnnotationVisibility visibility = (AnnotationVisibility)Enum.Parse(
				typeof(AnnotationVisibility), annotationNavigator.SelectSingleNode("./visibility").Value);
			XmlDocument doc = new XmlDocument();
			doc.Load(annotationNavigator.SelectSingleNode("./value").ReadSubtree());
			XmlNode value = doc.FirstChild.FirstChild;
			return new AnnotationInfo(value, visibility);
		}
	}


	public class AttrRelationshipInfo
	{
		private string attributeID;
		private Cardinality cardinality = Cardinality.Many;
		private RelationshipType relationshipType = RelationshipType.Flexible;
		private bool visible = true;
	    private string name;

		public AttrRelationshipInfo(string _attributeID)
		{
			attributeID = _attributeID;
		}

		public AttrRelationshipInfo(string _attributeID,
			Cardinality _cardinality, RelationshipType _relationshipType, bool _visible)
		{
			attributeID = _attributeID;
			cardinality = _cardinality;
			relationshipType = _relationshipType;
			visible = _visible;
		}

        public AttrRelationshipInfo(string _name, string _attributeID,
            Cardinality _cardinality, RelationshipType _relationshipType, bool _visible)
            : this(_attributeID, _cardinality, _relationshipType, _visible)
        {
            name = _name;
        }

		public string AttributeID
		{
			get { return attributeID; }
		}

		public Cardinality Cardinality
		{
			get { return cardinality; }
		}

		public RelationshipType RelationshipType
		{
			get { return relationshipType; }
		}

		public bool Visible
		{
			get { return visible; }
		}

	    public string Name
	    {
	        get { return name; }
	    }

		public static AttrRelationshipInfo ReadFromXML(XPathNavigator relationNavigator)
		{
			string attributeID = relationNavigator.GetAttribute("attributeid", "");
			Cardinality cardinality = (Cardinality)Enum.Parse(typeof(Cardinality),
				relationNavigator.GetAttribute("cardinality", ""), true);
			RelationshipType relationshipType = (RelationshipType)Enum.Parse(
				typeof(RelationshipType),
				relationNavigator.GetAttribute("relationshiptype", ""), true);
			bool visible = Convert.ToBoolean(relationNavigator.GetAttribute("visible", ""));
			return new AttrRelationshipInfo(attributeID, cardinality, relationshipType, visible);
		}
	}


	public class AttributeInfo
	{
		private string name;
		public string Description;

		public string AttributeHierarchyDisplayFolder = "";
		public bool AttributeHierarchyEnabled = true;
		public OptimizationType AttributeHierarchyOptimizedState = OptimizationType.FullyOptimized;
		public bool AttributeHierarchyOrdered = true;
		public bool AttributeHierarchyVisible = false;
		public string DefaultMember = string.Empty;
		public RollUpInfo CustomRollUpInfo;
		public bool IsAggregatable = true;
		private List<ColumnInfo> keyColumns = new List<ColumnInfo>();
		public bool MemberNamesUnique = false;
		public MembersWithData MembersWithData = MembersWithData.NonLeafDataVisible;
		public string MembersWithDataCaption = "(* ДАННЫЕ)";
		public ColumnInfo NameColumn;
		public string NamingTemplate;
		public OrderBy OrderBy = OrderBy.Name;
		public string OrderByAttributeID = "";
		public RootIfValue RootMemberIf = RootIfValue.ParentIsBlankSelfOrMissing;
		public AttributeInfo ParentAttr;
		public AttributeType Type = AttributeType.Regular;
		public AttributeUsage Usage = AttributeUsage.Regular;

		private List<AttrRelationshipInfo> attrRelationShips = new List<AttrRelationshipInfo>();

		public AttributeInfo(string _name)
		{
			name = _name;
		}

		public AttributeInfo(DimensionAttribute attr, DataSourceView dsv)
		{
			name = attr.Name;
			Description = attr.Description;

			AttributeHierarchyDisplayFolder = attr.AttributeHierarchyDisplayFolder;
			AttributeHierarchyEnabled = attr.AttributeHierarchyEnabled;
			AttributeHierarchyOrdered = attr.AttributeHierarchyOrdered;
			AttributeHierarchyVisible = attr.AttributeHierarchyVisible;

			if (attr.CustomRollupColumn != null)
			{
				string[] names = OLAPUtils.GetNamesFromBinding(attr.CustomRollupColumn.Source);
				DataTable dataTable = dsv.Schema.Tables[names[0]];
				string expression = dataTable.
					Columns[names[1]].ExtendedProperties["ComputedColumnExpression"].ToString();
				CustomRollUpInfo = new RollUpInfo("", "", LevelName, expression);
			}

			IsAggregatable = attr.IsAggregatable;
			MemberNamesUnique = attr.MemberNamesUnique;
			MembersWithData = attr.MembersWithData;
			MembersWithDataCaption = attr.MembersWithDataCaption;
			NamingTemplate = attr.NamingTemplate;
			OrderBy = attr.OrderBy;
			OrderByAttributeID = attr.OrderByAttributeID;
			RootMemberIf = attr.RootMemberIf;
			Type = attr.Type;
			Usage = attr.Usage;
			for (int i = 0; i < attr.KeyColumns.Count; i++)
				KeyColumns.Add(new ColumnInfo(dsv, attr.KeyColumns[i].Source));

			NameColumn = new ColumnInfo(dsv, attr.NameColumn.Source);

			for (int i = 0; i < attr.AttributeRelationships.Count; i++)
			{
				AttrRelationShips.Add(new AttrRelationshipInfo(
					attr.AttributeRelationships[i].AttributeID,
					attr.AttributeRelationships[i].Cardinality,
					attr.AttributeRelationships[i].RelationshipType,
					attr.AttributeRelationships[i].Visible));
			}
		}

		public string SafeName
		{
			get { return OLAPUtils.GetSafeName(name); }
			set { name = value; }
		}

		public string LevelName
		{
			get { return SafeName.Replace("уровень_", ""); }
		}

		public List<ColumnInfo> KeyColumns
		{
			get { return keyColumns; }
		}

		public List<AttrRelationshipInfo> AttrRelationShips
		{
			get { return attrRelationShips; }
		}

		public void AddRelationShip(AttributeInfo attrInfo)
		{
			attrRelationShips.Add(new AttrRelationshipInfo(attrInfo.SafeName));
			attrInfo.ParentAttr = this;
		}

		public static AttributeInfo ReadFromXML(XPathNavigator attributeNavigator)
		{
			AttributeInfo attrInfo = new AttributeInfo(attributeNavigator.GetAttribute("name", ""));
			attrInfo.Description = attributeNavigator.GetAttribute("description", "");

			attrInfo.AttributeHierarchyDisplayFolder = attributeNavigator.GetAttribute(
				"attributehierarchydisplayfolder", "");
			attrInfo.AttributeHierarchyEnabled = Convert.ToBoolean(attributeNavigator.GetAttribute(
				"attributehierarchyenabled", ""));
			attrInfo.AttributeHierarchyOrdered = Convert.ToBoolean(attributeNavigator.GetAttribute(
				"attributehierarchyordered", ""));
			attrInfo.AttributeHierarchyVisible = Convert.ToBoolean(attributeNavigator.GetAttribute(
				"attributehierarchyvisible", ""));
			string customRollUp = attributeNavigator.GetAttribute("customrollup", "");
			if (customRollUp != String.Empty)
			{
				attrInfo.CustomRollUpInfo =
					new RollUpInfo("", "", attrInfo.LevelName, customRollUp);
			}
			attrInfo.IsAggregatable = Convert.ToBoolean(attributeNavigator.GetAttribute(
				"isaggregatable", ""));
			attrInfo.MemberNamesUnique = Convert.ToBoolean(attributeNavigator.GetAttribute(
				"membernamesunique", ""));
			attrInfo.MembersWithData = (MembersWithData)Enum.Parse(typeof(MembersWithData),
				attributeNavigator.GetAttribute("memberswithdata", ""), true);
			attrInfo.MembersWithDataCaption = attributeNavigator.GetAttribute(
				"memberswithdatacaption", "");
			attrInfo.NamingTemplate = attributeNavigator.GetAttribute("namingtemplate", "");
			attrInfo.OrderBy = (OrderBy)Enum.Parse(typeof(OrderBy),
				attributeNavigator.GetAttribute("orderby", ""), true);
			attrInfo.OrderByAttributeID = attributeNavigator.GetAttribute("orderbyattributeid", "");
			attrInfo.RootMemberIf = (RootIfValue)Enum.Parse(typeof(RootIfValue),
				attributeNavigator.GetAttribute("rootmemberif", ""), true);
			attrInfo.Type = (AttributeType)Enum.Parse(typeof(AttributeType),
				attributeNavigator.GetAttribute("type", ""), true);
			attrInfo.Usage = (AttributeUsage)Enum.Parse(typeof(AttributeUsage),
				attributeNavigator.GetAttribute("usage", ""), true);
			XPathNodeIterator keyColumns = attributeNavigator.Select(".//keycolumns//columninfo");
			while (keyColumns.MoveNext())
				attrInfo.KeyColumns.Add(ColumnInfo.ReadFromXML(keyColumns.Current));
			attrInfo.NameColumn = ColumnInfo.ReadFromXML(
				attributeNavigator.SelectSingleNode(".//namecolumn//columninfo"));
			XPathNodeIterator relations = attributeNavigator.Select(
				".//relationships//relationship");
			while (relations.MoveNext())
				attrInfo.AttrRelationShips.Add(AttrRelationshipInfo.ReadFromXML(relations.Current));
			return attrInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("attribute");

			xmlWriter.WriteAttributeString("name", SafeName);
			xmlWriter.WriteAttributeString("description", Description);
			xmlWriter.WriteAttributeString(
				"attributehierarchydisplayfolder", AttributeHierarchyDisplayFolder);
			xmlWriter.WriteAttributeString(
				"attributehierarchyenabled", AttributeHierarchyEnabled.ToString());
			xmlWriter.WriteAttributeString(
				"attributehierarchyordered", AttributeHierarchyOrdered.ToString());
			xmlWriter.WriteAttributeString(
				"attributehierarchyvisible", AttributeHierarchyVisible.ToString());
			xmlWriter.WriteAttributeString("defaultmember", DefaultMember);

			if (CustomRollUpInfo != null) { CustomRollUpInfo.ToXml(xmlWriter); }

			xmlWriter.WriteAttributeString("isaggregatable", IsAggregatable.ToString());
			xmlWriter.WriteAttributeString("membernamesunique", MemberNamesUnique.ToString());
			xmlWriter.WriteAttributeString("memberswithdata", MembersWithData.ToString());
			xmlWriter.WriteAttributeString("memberswithdatacaption", MembersWithDataCaption);
			xmlWriter.WriteAttributeString("namingtemplate", NamingTemplate);
			xmlWriter.WriteAttributeString("orderby", OrderBy.ToString());
			xmlWriter.WriteAttributeString("orderbyattributeid", OrderByAttributeID);
			xmlWriter.WriteAttributeString("rootmemberif", RootMemberIf.ToString());
			xmlWriter.WriteAttributeString("type", Type.ToString());
			xmlWriter.WriteAttributeString("usage", Usage.ToString());

			xmlWriter.WriteStartElement("keycolumns");
			for (int i = 0; i < KeyColumns.Count; i++)
				KeyColumns[i].ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//keycolumns

			NameColumn.ToXML(xmlWriter, "namecolumn");

			xmlWriter.WriteStartElement("relationships");
			for (int i = 0; i < AttrRelationShips.Count; i++)
			{
				xmlWriter.WriteStartElement("relationship");
				xmlWriter.WriteAttributeString("attributeid", AttrRelationShips[i].AttributeID);
				xmlWriter.WriteAttributeString("cardinality",
					AttrRelationShips[i].Cardinality.ToString());
				xmlWriter.WriteAttributeString("relationshiptype",
					AttrRelationShips[i].RelationshipType.ToString());
				xmlWriter.WriteAttributeString("visible", AttrRelationShips[i].Visible.ToString());
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();//relationships

			xmlWriter.WriteEndElement();//attribute
		}
	}


	public class ColumnInfo
	{
		private TableInfo tableInfo;
		private string columnName;

		private void Init(string _tableName, string _columnName)
		{
			tableInfo = new TableInfo(_tableName.ToUpper());
			columnName = _columnName.ToUpper();
		}

        private void Init(string _tableName, string _columnName, string expression)
        {
            tableInfo = new TableInfo(_tableName.ToUpper(), expression);
            columnName = _columnName.ToUpper();
        }

		public ColumnInfo(TableInfo _tableInfo, string _columnName)
		{
			tableInfo = _tableInfo;
			columnName = _columnName.ToUpper();
		}

		public ColumnInfo(string _tableName, string _columnName)
		{
			Init(_tableName, _columnName);
		}

		public ColumnInfo(string tableDotColumn)
		{
		    string expression = string.Empty;

            if (tableDotColumn.ToLower().StartsWith("mod"))
		    {
		        expression = tableDotColumn.Replace("\"", "");
                expression = expression.Replace("DV.", "");
		        tableDotColumn = tableDotColumn.Split(new char[] {'(', ','})[1];
		    }

		    tableDotColumn = tableDotColumn.Replace("\"", "");
			tableDotColumn = tableDotColumn.Replace("DV.", "");
			tableDotColumn = tableDotColumn.Replace("[", "");
			tableDotColumn = tableDotColumn.Replace("]", "");

			string[] dot = { "." };
			string[] names = tableDotColumn.Split(dot, StringSplitOptions.RemoveEmptyEntries);

		    Init(names[0], names[1], expression);
		}

		public ColumnInfo(DataSourceView dsv, Binding source)
		{
			string[] names = OLAPUtils.GetNamesFromBinding(source);
			string tableName = names[0];
			columnName = names[1];

			DataTable dataTable = dsv.Schema.Tables[tableName];
			tableInfo = new TableInfo(tableName, GetExtendedProperty(dataTable, "Description"),
				GetExtendedProperty(dataTable, "QueryDefinition"),
                (TableType)Enum.Parse(typeof(TableType), GetExtendedProperty(dataTable, "TableType"), true));
            //tableInfo = new TableInfo(GetExtendedProperty(dataTable, "BaseTableName"),
            //    GetExtendedProperty(dataTable, "Description"), tableName, string.Empty,
            //    GetExtendedProperty(dataTable, "QueryDefinition"));
		}

		private static string GetExtendedProperty(DataTable dataTable, string propName)
		{
			if (dataTable.ExtendedProperties.ContainsKey(propName))
				return dataTable.ExtendedProperties[propName].ToString();
			return string.Empty;
		}

		public TableInfo Table
		{
			get { return tableInfo; }
		}

		public string TableName
		{
			get { return tableInfo.Name; }
		}

		public string ColumnName
		{
			get { return columnName; }
			set { columnName = value; }
		}

		public void UpdateTableInfo(TableInfo newTable)
		{			
            //if (tableInfo.BaseTableName.Equals(
            //    newTable.BaseTableName, StringComparison.OrdinalIgnoreCase))
            //    tableInfo = newTable;
            if (tableInfo.BasedOnTable(newTable))
            {
                tableInfo = newTable;
            }
		}

		public static ColumnInfo ReadFromXML(XPathNavigator columnNavigator)
		{
			if (columnNavigator == null) { return null; }
			string columnName = columnNavigator.GetAttribute("columnName", "");
			TableInfo tableInfo = TableInfo.ReadFromXML(
				columnNavigator.SelectSingleNode(".//tableinfo"));
			return new ColumnInfo(tableInfo, columnName);
		}
        
        /// <summary>
        /// Возвращает строку виду "ИмяТаблицы.ИмяПоля";
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TableName.ToUpperInvariant() + "." + ColumnName.ToUpperInvariant();
        }

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("columninfo");
			xmlWriter.WriteAttributeString("columnName", ColumnName);
			tableInfo.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//columninfo
		}

		public void ToXML(XmlWriter xmlWriter, string tagName)
		{
			xmlWriter.WriteStartElement(tagName);
			ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//tagName
		}
	}


	public class DataSourceInfo
	{
		public string Name;
		public string Description;
		public Dictionary<string, string> ConnectionParams = new Dictionary<string, string>();
		//public string ConnectionString = "Provider=OraOLEDB.Oracle.1; " +
		//    "Persist Security Info=True; Data Source=DV; User ID=DV; Password=DV";		

		public UpdateMode UpdateMode;

		private void Init()
		{
			Name = "DV";
			UpdateMode = UpdateMode.CreateOrReplace;
			ConnectionString = "Provider=OraOLEDB.Oracle.1; " + 
				"Persist Security Info=True; Data Source=DV; User ID=DV; Password=DV";
		}

		public DataSourceInfo()
		{
			Init();
		}

		public DataSourceInfo(string _connectionString)
		{
			Init();
			ConnectionString = _connectionString;
		}		

		public string ConnectionString
		{
			get { return OLAPResources.Utils.DictionaryToString(ConnectionParams); }
			set
			{
				ConnectionParams.Clear();
				ConnectionParams = OLAPResources.Utils.StringToDictionary(value);
			}
		}		
	}


	public class DataSourceViewInfo
	{
		public string Name;
		public string Description;
		public string DataSourceName;
		public UpdateMode UpdateMode = UpdateMode.CreateOrReplace;
		private List<RelationInfo> tableRelations = new List<RelationInfo>();
		private TableDictionary usedTables = new TableDictionary();

		public DataSourceViewInfo()
		{
			Name = "DV";
		}

		public DataSourceViewInfo(DataSourceView dsv)
		{
			Name = dsv.Name;
		}

		public List<RelationInfo> TableRelations
		{
			get { return tableRelations; }
		}

		public TableDictionary UsedTables
		{
			get { return usedTables; }
		}
	}


	public class HierarchyInfo
	{
		public string Description;
		public string AllMemberName = string.Empty;
		public bool MemberNamesUnique = true;
		public bool AllowDuplicateNames = false;
		public string DisplayFolder = string.Empty;
		public List<LevelInfo> Levels;

		public bool ParentChild = false;
		//public AttributeInfo ParentAttribute;
		private IdentificationInfo identificationInfo;

		public HierarchyInfo(IdentificationInfo _identificationInfo)
		{
			identificationInfo = _identificationInfo;
		}

		public HierarchyInfo(Hierarchy hierarchy)
		{
			identificationInfo = new IdentificationInfo(hierarchy.Name, hierarchy.ID);
			Description = hierarchy.Description;
			AllMemberName = hierarchy.AllMemberName;
			MemberNamesUnique = hierarchy.MemberNamesUnique;
			AllowDuplicateNames = hierarchy.AllowDuplicateNames;
			DisplayFolder = hierarchy.DisplayFolder;

			Levels = new List<LevelInfo>();
			for (int i = 0; i < hierarchy.Levels.Count; i++)
				Levels.Add(new LevelInfo(hierarchy.Levels[i]));

		}

		public string Name
		{
			get { return identificationInfo.Name; }
		}

		public string ID
		{
			get { return identificationInfo.ID; }
		}

		public bool AllLevel
		{
			get { return AllMemberName != string.Empty; }
		}

		public string GetLevelNameByAttribute(string attrID)
		{
			if (Levels == null) { return string.Empty; }
			for (int i = 0; i < Levels.Count; i++)
			{
				if (Levels[i].SourceAttributeID.Equals(attrID, StringComparison.OrdinalIgnoreCase))
				{
					return Levels[i].Name;
				}
			}
			return string.Empty;
		}

		public static HierarchyInfo ReadFromXML(XPathNavigator hierarchyNavigator)
		{
			IdentificationInfo idInfo = IdentificationInfo.ReadFromXML(hierarchyNavigator);
			HierarchyInfo hierarchyInfo = new HierarchyInfo(idInfo);
			hierarchyInfo.AllMemberName = hierarchyNavigator.GetAttribute("allmembername", "");
			hierarchyInfo.AllowDuplicateNames = Convert.ToBoolean(
				hierarchyNavigator.GetAttribute("allowduplicatenames", ""));
			hierarchyInfo.Description = hierarchyNavigator.GetAttribute("description", "");
			hierarchyInfo.DisplayFolder = hierarchyNavigator.GetAttribute("displayfolder", "");
			hierarchyInfo.MemberNamesUnique = Convert.ToBoolean(
				hierarchyNavigator.GetAttribute("membernamesunique", ""));
			XPathNodeIterator levelsNodes = hierarchyNavigator.Select(".//levels//level");
			if (levelsNodes.Count > 0)
			{
				hierarchyInfo.Levels = new List<LevelInfo>(levelsNodes.Count);
				while (levelsNodes.MoveNext())
					hierarchyInfo.Levels.Add(LevelInfo.ReadFromXML(levelsNodes.Current));
			}
			return hierarchyInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("hierarchy");
			identificationInfo.ToXML(xmlWriter);
			xmlWriter.WriteAttributeString("allmembername", AllMemberName);
			xmlWriter.WriteAttributeString("allowduplicatenames", AllowDuplicateNames.ToString());
			xmlWriter.WriteAttributeString("description", Description);
			xmlWriter.WriteAttributeString("displayfolder", DisplayFolder);
			xmlWriter.WriteAttributeString("membernamesunique", MemberNamesUnique.ToString());

			xmlWriter.WriteStartElement("levels");
			for (int i = 0; i < Levels.Count; i++)
			{
				Levels[i].ToXML(xmlWriter);
			}
			xmlWriter.WriteEndElement();//levels
			xmlWriter.WriteEndElement();//hierarchy
		}
	}


	public class LevelInfo
	{
		private string name;
		//public string Caption;
		public string Description;
		public HideIfValue HideMemberIf = HideIfValue.Never;
		public string SourceAttributeID;

		public bool IsAll()
		{
			return name == "(All)";
		}

		public LevelInfo(string _name)
		{
			name = _name;
		}

		public LevelInfo(Level level)
		{
			name = level.Name;
			Description = level.Description;
			HideMemberIf = level.HideMemberIf;
			SourceAttributeID = level.SourceAttributeID;
		}

		public string Name
		{
			get { return name; }
		}

		public static LevelInfo ReadFromXML(XPathNavigator levelNavigator)
		{
			LevelInfo levelInfo = new LevelInfo(levelNavigator.GetAttribute("name", ""));
			levelInfo.Description = levelNavigator.GetAttribute("description", "");
			levelInfo.HideMemberIf = (HideIfValue)Enum.Parse(typeof(HideIfValue),
				levelNavigator.GetAttribute("hidememberif", ""), true);
			levelInfo.SourceAttributeID = levelNavigator.GetAttribute("sourceattributeid", "");
			return levelInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("level");
			xmlWriter.WriteAttributeString("name", name);
			xmlWriter.WriteAttributeString("description", Description);
			xmlWriter.WriteAttributeString("hidememberif", HideMemberIf.ToString());
			xmlWriter.WriteAttributeString("sourceattributeid", SourceAttributeID);
			xmlWriter.WriteEndElement();//level
		}
	}

	public class RelationComparer : IComparer<RelationInfo>
	{
		public int Compare(RelationInfo x, RelationInfo y)
		{
			if (x == null && y == null) { return 0; }
			if (x == null && y != null) { return -1; }
			if (x != null && y == null) { return 1; }

			//int retval = x.Name.Length.CompareTo(y.Name.Length);
			//if (retval != 0) { return retval; }

			//// If the strings are of equal length, sort them with ordinary string comparison.			
			//return x.Name.CompareTo(y.Name);

			return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
		}
	}



	public class RelationInfo
	{
		public ColumnInfo ForeignKey;
		public ColumnInfo PrimaryKey;

		public RelationInfo(ColumnInfo _ForeignKey, ColumnInfo _PrimaryKey)
		{
			ForeignKey = _ForeignKey;
			PrimaryKey = _PrimaryKey;
		}

		public RelationInfo(string tableName, string primaryKeyName, string foreignKeyName)
		{
			ForeignKey = new ColumnInfo(tableName, foreignKeyName);
			PrimaryKey = new ColumnInfo(tableName, primaryKeyName);
		}

		public string Name
		{
			get
			{
				if (ForeignKey != null)
				{
					return string.Format("FK_{0}_{1}_{2}",
						ForeignKey.TableName, ForeignKey.ColumnName, PrimaryKey.TableName);
				}
				else
					return "Без имени (не задана таблица с внешними ключами)";
			}
		}

        /// <summary>
        /// Проверяет, содержит ли отношение искомую таблицу.
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
		public bool ContainsTable(TableInfo tableInfo)
		{
            return PrimaryKey.Table.BasedOnTable(tableInfo) || ForeignKey.Table.BasedOnTable(tableInfo);			
		}
	}


	public class RollUpInfo
	{
		private string cubeName;
		private string hierarchy;
		private string attribute;
		private string expression;

		public RollUpInfo(string _cubeName, string _hierarchy, string _attribute, string _expression)
		{
			cubeName = _cubeName;
			hierarchy = _hierarchy;
			attribute = _attribute;
			expression = _expression;
		}

		public string Name
		{
			get
			{
				if (cubeName == string.Empty)
					return String.Format("CR_{0}_{1}", "Shared", attribute);
				else
					return String.Format("CR_{0}_{1}", cubeName, attribute);
			}
		}

		public string Hierarchy
		{
			get { return hierarchy; }
		}

		public string Level
		{
			get { return attribute; }
		}

		public string Expression
		{
			get { return expression; }
			set { expression = value; }
		}

		public string SafeExpression
		{
			get
			{
				if (expression.Length > 0)
				{
					string safeExpression = expression;
					if (expression[0] != '\'')
						safeExpression = '\'' + safeExpression;
					if (expression[expression.Length - 1] != '\'')
						safeExpression = safeExpression + '\'';
					return safeExpression;
				}
				else
					return "\'\'";
			}
		}

		public string SafeComputedColumnName
		{
			get { return OLAPUtils.GetSafeName(Name); }
		}

		public void ToXml(XmlWriter xmlWriter)
		{
			xmlWriter.WriteAttributeString("customrollup", expression);
		}
	}


	public class MeasureInfo
	{
		public AggregationFunction AggregateFunction = AggregationFunction.Sum;
		public MeasureDataType DataType = MeasureDataType.Double;
		public string Description = string.Empty;
		public string DisplayFolder = string.Empty;
		public string FormatString = string.Empty;
		public string MeasureExpression = string.Empty;
		public bool Visible = true;
		public ColumnInfo SourceColumn;
		private IdentificationInfo identificationInfo;

		public MeasureInfo(string name, Guid guid)
		{
			identificationInfo = new IdentificationInfo(name, guid);
		}

		public MeasureInfo(IdentificationInfo _identificationInfo)
		{
			identificationInfo = _identificationInfo;
		}

		public MeasureInfo(Measure measure)
		{
			identificationInfo = new IdentificationInfo(measure.Name, measure.ID);
			AggregateFunction = measure.AggregateFunction;
			DataType = measure.DataType;
			Description = measure.Description;
			DisplayFolder = measure.DisplayFolder;
			FormatString = measure.FormatString;
			MeasureExpression = measure.MeasureExpression;
			Visible = measure.Visible;
			if (!measure.IsLinked)
				SourceColumn = new ColumnInfo(measure.ParentCube.DataSourceView, measure.Source.Source);
		}

		public string Name
		{
			get { return identificationInfo.Name; }
		}

		public string ID
		{
			get { return identificationInfo.ID; }
		}

		public Guid GUID
		{
			get { return identificationInfo.GUID; }
			set { identificationInfo.GUID = value; }
		}

		public bool IsLinked
		{
			get { return SourceColumn == null; }
		}

		public static MeasureInfo ReadFromXML(XPathNavigator measureNavigator)
		{
			MeasureInfo measureInfo = new MeasureInfo(
				IdentificationInfo.ReadFromXML(measureNavigator));
			string aggrFunction = measureNavigator.GetAttribute("aggregatefunction", "");
			if (aggrFunction != string.Empty)
			{
				measureInfo.AggregateFunction = (AggregationFunction)Enum.Parse(
					typeof(AggregationFunction),
					measureNavigator.GetAttribute("aggregatefunction", ""), true);
			}
			string dataType = measureNavigator.GetAttribute("datatype", "");
			if (dataType != string.Empty)
			{
				measureInfo.DataType = (MeasureDataType)Enum.Parse(typeof(MeasureDataType),
					measureNavigator.GetAttribute("datatype", ""), true);
			}
			measureInfo.Description = measureNavigator.GetAttribute("description", "");
			measureInfo.DisplayFolder = measureNavigator.GetAttribute("displayfolder", "");
			measureInfo.FormatString = measureNavigator.GetAttribute("formatstring", "");
			measureInfo.MeasureExpression = measureNavigator.GetAttribute("measureexpression", "");
			measureInfo.Visible = Convert.ToBoolean(measureNavigator.GetAttribute("visible", ""));
			measureInfo.SourceColumn = ColumnInfo.ReadFromXML(
				measureNavigator.SelectSingleNode(".//columninfo"));
			return measureInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("measure");
			identificationInfo.ToXML(xmlWriter);
			if (!IsLinked)
			{
				xmlWriter.WriteAttributeString("aggregatefunction", AggregateFunction.ToString());
				xmlWriter.WriteAttributeString("datatype", DataType.ToString());
				xmlWriter.WriteAttributeString("description", Description);
				xmlWriter.WriteAttributeString("displayfolder", DisplayFolder);
				xmlWriter.WriteAttributeString("formatstring", FormatString);
				xmlWriter.WriteAttributeString("measureexpression", MeasureExpression);
			}
			xmlWriter.WriteAttributeString("visible", Visible.ToString());

			if (!IsLinked) { SourceColumn.ToXML(xmlWriter); }

			xmlWriter.WriteEndElement();//measure
		}
	}


	public enum MeasureGroupDimensionType
	{
		RegularDimension = 0,
		ReferenceDimension = 1,
		DegenerateDimension = 2, //Это измерение на основе таблицы фактов куба.
	}


	public class MeasureGroupDimensionInfo
	{
		public IdentificationInfo CubeDimIdentification;
		public Cardinality Cardinality = Cardinality.Many;
		public MeasureGroupDimensionType DimensionType;
		public List<ColumnInfo> ReferenceToThis = new List<ColumnInfo>();
		public string IntermediateGranularityAttrID;
		public IdentificationInfo IntermediateDimIdentification;

		public MeasureGroupDimensionInfo()
		{
		}

		public MeasureGroupDimensionInfo(CubeDimensionInfoBase cubeDimension)
		{
			CubeDimIdentification = new IdentificationInfo(cubeDimension.Name, cubeDimension.ID);
		}

		private void InitAttributes(MeasureGroupAttributeCollection attributes,
			Cardinality cardinality, DataSourceView dsv)
		{
			Cardinality = cardinality;
			for (int i = 0; i < attributes.Count; i++)
				if (attributes[i].Type == MeasureGroupAttributeType.Granularity)
				{
					for (int j = 0; j < attributes[i].KeyColumns.Count; j++)
						ReferenceToThis.Add(new ColumnInfo(dsv, attributes[i].KeyColumns[j].Source));
					break;
				}
		}

		internal MeasureGroupDimensionInfo(MeasureGroupDimension dim)
		{
			CubeDimIdentification = new IdentificationInfo(dim.CubeDimension.Name, dim.CubeDimensionID);
			if (dim is DegenerateMeasureGroupDimension)
			{
				DegenerateMeasureGroupDimension degDim = dim as DegenerateMeasureGroupDimension;
				InitAttributes(
					degDim.Attributes, degDim.Cardinality, dim.ParentCube.DataSourceView);
				DimensionType = MeasureGroupDimensionType.DegenerateDimension;
			}
			else
				if (dim is ReferenceMeasureGroupDimension)
				{
					ReferenceMeasureGroupDimension refDim = dim as ReferenceMeasureGroupDimension;
					InitAttributes(
						refDim.Attributes, refDim.Cardinality, dim.ParentCube.DataSourceView);
					IntermediateDimIdentification =
						new IdentificationInfo(
						refDim.IntermediateCubeDimension.Name, refDim.IntermediateCubeDimensionID);
					IntermediateGranularityAttrID = refDim.IntermediateGranularityAttributeID;
					DimensionType = MeasureGroupDimensionType.ReferenceDimension;
				}
				else
					if (dim is RegularMeasureGroupDimension)
					{
						RegularMeasureGroupDimension regDim = dim as RegularMeasureGroupDimension;
						InitAttributes(
							regDim.Attributes, regDim.Cardinality, dim.ParentCube.DataSourceView);
						DimensionType = MeasureGroupDimensionType.RegularDimension;
					}
		}

		private void MeasureGroupDimensionAttributesToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteAttributeString("cardinality", Cardinality.ToString());
			for (int i = 0; i < ReferenceToThis.Count; i++)
				ReferenceToThis[i].ToXML(xmlWriter);
		}

		public static MeasureGroupDimensionInfo ReadFromXML(XPathNavigator dimensionNavigator)
		{
			MeasureGroupDimensionInfo mgDimension = new MeasureGroupDimensionInfo();
			mgDimension.DimensionType = (MeasureGroupDimensionType)
				Enum.Parse(typeof(MeasureGroupDimensionType), dimensionNavigator.Name, true);
			mgDimension.CubeDimIdentification =
				IdentificationInfo.ReadFromXML(dimensionNavigator);

			XPathNodeIterator keyColumns = dimensionNavigator.Select(".//columninfo");
			while (keyColumns.MoveNext())
				mgDimension.ReferenceToThis.Add(ColumnInfo.ReadFromXML(keyColumns.Current));

			XPathNavigator intermediateDim = dimensionNavigator.SelectSingleNode(
				".//intermediatedimension");
			if (intermediateDim != null)
			{
				mgDimension.IntermediateDimIdentification =
					IdentificationInfo.ReadFromXML(intermediateDim);
				mgDimension.IntermediateGranularityAttrID =
					intermediateDim.GetAttribute("attributeid", "");
			}

			return mgDimension;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			switch (DimensionType)
			{
				case MeasureGroupDimensionType.RegularDimension:
					xmlWriter.WriteStartElement("regulardimension");
					CubeDimIdentification.ToXML(xmlWriter);
					MeasureGroupDimensionAttributesToXML(xmlWriter);
					xmlWriter.WriteEndElement();//dimension
					break;
				case MeasureGroupDimensionType.ReferenceDimension:
					xmlWriter.WriteStartElement("referencedimension");
					CubeDimIdentification.ToXML(xmlWriter);
					MeasureGroupDimensionAttributesToXML(xmlWriter);

					xmlWriter.WriteStartElement("intermediatedimension");
					IntermediateDimIdentification.ToXML(xmlWriter);
					xmlWriter.WriteAttributeString("attributeid", IntermediateGranularityAttrID);
					xmlWriter.WriteEndElement();//intermediatedimension

					xmlWriter.WriteEndElement();//dimension
					break;
				case MeasureGroupDimensionType.DegenerateDimension:
					xmlWriter.WriteStartElement("degeneratedimension");
					CubeDimIdentification.ToXML(xmlWriter);
					MeasureGroupDimensionAttributesToXML(xmlWriter);
					xmlWriter.WriteEndElement();//dimension
					break;
				default:
					break;
			}
		}
	}

	public class MeasureGroupBindingInfo
	{
		public string CubeID = string.Empty;
		public string DataSourceID = ".";
		public string Filter = string.Empty;
		public string MeasureGroupID = string.Empty;
		public TimeSpan RefreshInterval = TimeSpan.Parse("00:00:01");
		public RefreshPolicy RefreshPolicyValue = RefreshPolicy.ByQuery;

		public MeasureGroupBindingInfo()
		{
		}

		public MeasureGroupBindingInfo(string _cubeID, string _measureGroupID)
		{
			CubeID = _cubeID;
			MeasureGroupID = _measureGroupID;
		}

		public MeasureGroupBindingInfo(MeasureGroupBinding binding)
		{
			CubeID = binding.CubeID;
			DataSourceID = binding.DataSourceID;
			Filter = binding.Filter;
			MeasureGroupID = binding.MeasureGroupID;
			RefreshInterval = binding.RefreshInterval;
			RefreshPolicyValue = binding.RefreshPolicy;
		}

		public static MeasureGroupBindingInfo ReadFromXML(XPathNavigator mgBindingNavigator)
		{
			if (mgBindingNavigator == null) { return null; }

			MeasureGroupBindingInfo mgBindingInfo = new MeasureGroupBindingInfo();
			mgBindingInfo.CubeID = mgBindingNavigator.GetAttribute("cubeid", "");
			mgBindingInfo.Filter = mgBindingNavigator.GetAttribute("filter", "");
			mgBindingInfo.MeasureGroupID = mgBindingNavigator.GetAttribute("measuregroupid", "");
			mgBindingInfo.RefreshInterval = TimeSpan.Parse(
				mgBindingNavigator.GetAttribute("refreshinterval", ""));
			mgBindingInfo.RefreshPolicyValue = (RefreshPolicy)Enum.Parse(typeof(RefreshPolicy),
				mgBindingNavigator.GetAttribute("refreshpolicy", ""), true);
			return mgBindingInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("source");
			xmlWriter.WriteAttributeString("cubeid", CubeID);
			xmlWriter.WriteAttributeString("filter", Filter);
			xmlWriter.WriteAttributeString("measuregroupid", MeasureGroupID);
			xmlWriter.WriteAttributeString("refreshinterval", RefreshInterval.ToString());
			xmlWriter.WriteAttributeString("refreshpolicy", RefreshPolicyValue.ToString());
			xmlWriter.WriteEndElement();//source
		}
	}


	public class MeasureGroupInfo
	{
		public string AggregationPrefix = string.Empty;
		public DataAggregationMode DataAggregation = DataAggregationMode.DataAndCacheAggregatable;
		public string Description = string.Empty;
		public bool IgnoreUnrelatedDimensions = true;
		public ProcessingMode ProcessingMode = ProcessingMode.LazyAggregations;
		public int ProcessingPriority = 0;
		public List<PartitionInfo> Partitions = new List<PartitionInfo>();
		public MeasureGroupBindingInfo Source = null;
		public string StorageLocation = string.Empty;
		public StorageMode StorageMode = StorageMode.Molap;
		public MeasureGroupType Type = MeasureGroupType.Regular;

		public List<MeasureInfo> Measures = new List<MeasureInfo>();
		public Dictionary<string, AnnotationInfo> Annotations = new Dictionary<string, AnnotationInfo>();
		public List<MeasureGroupDimensionInfo> MGDimensions = new List<MeasureGroupDimensionInfo>();
		public IdentificationInfo identificationInfo;

		private MeasureGroupInfo()
		{ }

		public MeasureGroupInfo(string _name, Guid _guid)
		{
			identificationInfo = new IdentificationInfo(_name, _guid);
		}

        public MeasureGroupInfo(string _name, string _guid)
        {
            identificationInfo = new IdentificationInfo(_name, _guid);
        }

		public MeasureGroupInfo(IdentificationInfo _identificationInfo)
		{
			identificationInfo = _identificationInfo;
		}

		public MeasureGroupInfo(MeasureGroup measureGroup)
		{
			identificationInfo = new IdentificationInfo(measureGroup.Name, measureGroup.ID);
			AggregationPrefix = measureGroup.AggregationPrefix;
			DataAggregation = measureGroup.DataAggregation;
			Description = measureGroup.Description;
			IgnoreUnrelatedDimensions = measureGroup.IgnoreUnrelatedDimensions;
			ProcessingMode = measureGroup.ProcessingMode;
			ProcessingPriority = measureGroup.ProcessingPriority;

			for (int i = 0; i < measureGroup.Partitions.Count; i++)
				Partitions.Add(new PartitionInfo(measureGroup.Partitions[i]));

			//Source есть только у прилинкованных групп мер
			if (measureGroup.Source != null)
				Source = new MeasureGroupBindingInfo(measureGroup.Source);

			StorageLocation = measureGroup.StorageLocation;
			StorageMode = measureGroup.StorageMode;
			Type = measureGroup.Type;

			for (int i = 0; i < measureGroup.Measures.Count; i++)
				Measures.Add(new MeasureInfo(measureGroup.Measures[i]));

			for (int i = 0; i < measureGroup.Annotations.Count; i++)
                Annotations.Add(measureGroup.Annotations[i].Name, new AnnotationInfo(measureGroup.Annotations[i]));

			for (int i = 0; i < measureGroup.Dimensions.Count; i++)
				MGDimensions.Add(new MeasureGroupDimensionInfo(measureGroup.Dimensions[i]));
		}

		public string Name
		{
			get { return identificationInfo.Name; }
		}

		public bool IsLinked
		{
			get { return Source != null; }
		}

		public string ID
		{
			get { return identificationInfo.ID; }
		}

		public static MeasureGroupInfo ReadFromXML(XPathNavigator mgNavigator,
			IdentificationList excludedDims)
		{
			MeasureGroupInfo mgInfo =
				new MeasureGroupInfo(IdentificationInfo.ReadFromXML(mgNavigator));
			mgInfo.Source = MeasureGroupBindingInfo.ReadFromXML(
				mgNavigator.SelectSingleNode("source"));
			if (mgInfo.Source == null)
			{
				mgInfo.AggregationPrefix = mgNavigator.GetAttribute("aggregationprefix", "");
				mgInfo.DataAggregation = (DataAggregationMode)Enum.Parse(typeof(DataAggregationMode),
					mgNavigator.GetAttribute("dataaggregation", ""), true);
				mgInfo.Description = mgNavigator.GetAttribute("description", "");

				mgInfo.ProcessingMode = (ProcessingMode)Enum.Parse(typeof(ProcessingMode),
				mgNavigator.GetAttribute("processingmode", ""), true);
				mgInfo.ProcessingPriority = Convert.ToInt32(
					mgNavigator.GetAttribute("processingpriority", ""));
				mgInfo.StorageLocation = mgNavigator.GetAttribute("storagelocation", "");
				mgInfo.StorageMode = (StorageMode)Enum.Parse(typeof(StorageMode),
					mgNavigator.GetAttribute("storagemode", ""), true);
				mgInfo.Type = (MeasureGroupType)Enum.Parse(typeof(MeasureGroupType),
					mgNavigator.GetAttribute("type", ""), true);
			}
			mgInfo.IgnoreUnrelatedDimensions = Convert.ToBoolean(
				mgNavigator.GetAttribute("ignoreunrelateddimensions", ""));

			XPathNodeIterator measuresNodes = mgNavigator.Select(".//measures//measure");
			while (measuresNodes.MoveNext())
				mgInfo.Measures.Add(MeasureInfo.ReadFromXML(measuresNodes.Current));

			XPathNodeIterator dimNodes = mgNavigator.Select(".//dimensions//regulardimension");
			while (dimNodes.MoveNext())
				if (!excludedDims.Contains(dimNodes.Current.GetAttribute("id", "")))
				{
					mgInfo.MGDimensions.Add(
						MeasureGroupDimensionInfo.ReadFromXML(dimNodes.Current));
				}

			dimNodes = mgNavigator.Select(".//dimensions//degeneratedimension");
			while (dimNodes.MoveNext())
				if (!excludedDims.Contains(dimNodes.Current.GetAttribute("id", "")))
				{
					mgInfo.MGDimensions.Add(
						MeasureGroupDimensionInfo.ReadFromXML(dimNodes.Current));
				}

			dimNodes = mgNavigator.Select(".//dimensions//referencedimension");
			while (dimNodes.MoveNext())
				if (!excludedDims.Contains(dimNodes.Current.GetAttribute("id", "")))
				{
					mgInfo.MGDimensions.Add(
						MeasureGroupDimensionInfo.ReadFromXML(dimNodes.Current));
				}

			XPathNodeIterator partNodes = mgNavigator.Select(".//partitions//partition");
			while (partNodes.MoveNext())
			{
				mgInfo.Partitions.Add(PartitionInfo.ReadFromXML(partNodes.Current));
			}

			XPathNodeIterator annNodes = mgNavigator.Select(".//annotations//annotation");
			while (annNodes.MoveNext())
			{
				mgInfo.Annotations.Add(AnnotationInfo.ReadFromXML(annNodes.Current).Name, AnnotationInfo.ReadFromXML(annNodes.Current));
			}

			return mgInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("measuregroup");
			identificationInfo.ToXML(xmlWriter);
			if (!IsLinked)
			{
				xmlWriter.WriteAttributeString("aggregationprefix", AggregationPrefix);
				xmlWriter.WriteAttributeString("dataaggregation", DataAggregation.ToString());
				xmlWriter.WriteAttributeString("description", Description);
			}
			xmlWriter.WriteAttributeString("ignoreunrelateddimensions", IgnoreUnrelatedDimensions.ToString());
			if (!IsLinked)
			{
				xmlWriter.WriteAttributeString("processingmode", ProcessingMode.ToString());
				xmlWriter.WriteAttributeString("processingpriority", ProcessingPriority.ToString());
				xmlWriter.WriteAttributeString("storagelocation", StorageLocation);
				xmlWriter.WriteAttributeString("storagemode", StorageMode.ToString());
				xmlWriter.WriteAttributeString("type", Type.ToString());
			}
			else
				Source.ToXML(xmlWriter);

			xmlWriter.WriteStartElement("measures");
			for (int i = 0; i < Measures.Count; i++)
				Measures[i].ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//measures

			xmlWriter.WriteStartElement("dimensions");
			for (int i = 0; i < MGDimensions.Count; i++)
				MGDimensions[i].ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//dimensions

			xmlWriter.WriteStartElement("partitions");
			for (int i = 0; i < Partitions.Count; i++)
				Partitions[i].ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//partitions

			OLAPUtils.AnnotationsToXML(xmlWriter, Annotations);

			xmlWriter.WriteEndElement();//measuregroup
		}
	}


	public class CalculationInfo
	{
		private string name;
		private string safeName;

		private string description;
		private string expression;

		private CalculationInfo()
		{ }

		public CalculationInfo(string _name, string _expression, string _description)
		{
			name = _name;
			expression = _expression;
			description = _description;
			safeName = OLAPUtils.GetSafeName(_name);
		}

		public string SafeName
		{
			get { return safeName; }
		}

		public string Expression
		{
			get { return expression; }
			set { expression = value; }
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteElementString("command", expression);
		}
	}


	public class MDXScriptInfo
	{
		public IdentificationInfo IdentificationInfo = new IdentificationInfo("Default", "Default");
		public List<CalculationInfo> Calculations = new List<CalculationInfo>();
		public string Description;

		public MDXScriptInfo()
		{ }

		public MDXScriptInfo(string name, string id)
		{
			IdentificationInfo = new IdentificationInfo(name, id);
		}

		public MDXScriptInfo(IdentificationInfo _identificationInfo)
		{
			IdentificationInfo = _identificationInfo;
		}

		public MDXScriptInfo(MdxScript mdxScript)
		{
			IdentificationInfo = new IdentificationInfo(mdxScript.Name, mdxScript.ID);
			for (int i = 0; i < mdxScript.Commands.Count; i++)
			{
				Calculations.Add(
					new CalculationInfo(string.Empty, mdxScript.Commands[i].Text, string.Empty));
			}
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("mdxscript");

			xmlWriter.WriteAttributeString("id", IdentificationInfo.ID);
			xmlWriter.WriteAttributeString("name", IdentificationInfo.Name);
			xmlWriter.WriteAttributeString("description", Description);

			xmlWriter.WriteStartElement("commands");
			for (int i = 0; i < Calculations.Count; i++)
				Calculations[i].ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//commands

			xmlWriter.WriteEndElement();//mdxscript
		}

		public string Name
		{
			get { return IdentificationInfo.Name; }
		}

		public string ID
		{
			get { return IdentificationInfo.ID; }
		}
	}

    /// <summary>
    /// Общее описание куба
    /// </summary>
	public abstract class CubeInfoBase : VersionedObjectInfo
	{
		protected string description;
		protected List<CubeDimensionInfoBase> dimensions = new List<CubeDimensionInfoBase>();
		protected List<MeasureGroupInfo> measureGroups = new List<MeasureGroupInfo>();
		protected List<MeasureGroupInfo> linkedMeasureGroups = new List<MeasureGroupInfo>();
		protected List<MDXScriptInfo> mdxScripts = new List<MDXScriptInfo>();
		protected Dictionary<string, AnnotationInfo> annotations = new Dictionary<string, AnnotationInfo>();

        protected TableDictionary measureGroupTables = new TableDictionary();		

		protected abstract void ReadDescription();
		protected abstract void ReadCubeDimensions();
		protected abstract void ReadMeasureGroups();
		protected abstract void ReadIdentificationInfo();
		protected abstract void ReadCalculations();		

		protected void Init(Cube cube)
		{
			idInfo = new IdentificationInfo(cube.Name, cube.ID);
			description = cube.Description;
			versions = OLAPUtils.VersionsFromAnnotations(cube.Annotations);

			for (int i = 0; i < cube.Dimensions.Count; i++)
				dimensions.Add(new CubeDimensionInfoBase(cube.Dimensions[i]));

			for (int i = 0; i < cube.MeasureGroups.Count; i++)
			{
				if (cube.MeasureGroups[i].IsLinked)
					linkedMeasureGroups.Add(new MeasureGroupInfo(cube.MeasureGroups[i]));
				else
					measureGroups.Add(new MeasureGroupInfo(cube.MeasureGroups[i]));
			}

			for (int i = 0; i < cube.MdxScripts.Count; i++)
				mdxScripts.Add(new MDXScriptInfo(cube.MdxScripts[i]));


			for (int i = 0; i < cube.Annotations.Count; i++)
				Annotations.Add(cube.Annotations[i].Name, new AnnotationInfo(cube.Annotations[i]));
		}

		public CubeInfoBase()
			: base()
		{ }

		public CubeInfoBase(XPathNavigator _cubeNavigator)
			: base(_cubeNavigator, new CubeControlBlock())
		{ }

		public CubeInfoBase(XPathNavigator _cubeNavigator, CubeControlBlock _cubeControlBlock)
			: base(_cubeNavigator, _cubeControlBlock)
		{ }

		public CubeInfoBase(Cube cube)
			: base(cube, CubeControlBlock.ReadFromAnnotations(cube.Annotations))
		{
			Init(cube);
		}

		public CubeInfoBase(Cube cube, CubeControlBlock _cubeControlBlock)
			: base(cube, _cubeControlBlock)
		{
			Init(cube);
		}

		protected virtual void ReadCube()
		{
			ReadDescription();
			ReadCalculations();
			ReadCubeDimensions();
			ReadMeasureGroups();
			GetMeasureGroupTables();			
		}

		protected void GetMeasureGroupTables()
		{
			for (int i = 0; i < measureGroups.Count; i++)
			{
				for (int j = 0; j < measureGroups[i].Measures.Count; j++)
					if (measureGroups[i].Measures[j].SourceColumn != null)
						measureGroupTables.AddTable(measureGroups[i].Measures[j].SourceColumn.Table);
			}
		}

		[Browsable(false)]
		new public CubeControlBlock ControlBlock
		{
			get { return controlBlock as CubeControlBlock; }
			set { controlBlock = value; }
		}

		[Browsable(true), Category("Разное"), DisplayName("Описание"),
		Description("Описание куба."), MergableProperty(false)]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		[Browsable(false)]
		public IdentificationList ExcludedDimensions
		{
			get { return ControlBlock.ExcludedDimensions; }
		}

		[Browsable(false)]
		public Dictionary<string, AnnotationInfo> Annotations
		{
			get { return annotations; }
		}

		[Browsable(false)]
		public List<CubeDimensionInfoBase> Dimensions
		{
			get { return dimensions; }
		}

		[Browsable(false)]
		public List<MeasureGroupInfo> MeasureGroups
		{
			get { return measureGroups; }
		}

		[Browsable(false)]
		public List<MeasureGroupInfo> LinkedMeasureGroups
		{
			get { return linkedMeasureGroups; }
		}

		[Browsable(false)]
		public List<CalculationInfo> Calculations
		{
			get
			{
				if (mdxScripts.Count > 0) { return mdxScripts[0].Calculations; }
				return null;
			}
		}

		[Browsable(false)]
		public List<MDXScriptInfo> MDXScripts
		{
			get { return mdxScripts; }
		}

		[Browsable(false)]
        public TableDictionary MeasureGroupTables
		{
			get
			{
				if (measureGroupTables.Count == 0) { GetMeasureGroupTables(); }
				return measureGroupTables;
			}
		}

		[Browsable(true), Category("Управляющая информация"), DisplayName("Читать из файла"),
		Description("Определяет, где искать объект - в файле или в пакете."), ReadOnly(true)]
		public bool ReadFromFile
		{
			get { return controlBlock.ReadFromFile; }
			set { controlBlock.ReadFromFile = value; }
		}

		[Browsable(true), Category("Управляющая информация"), DisplayName("Режим обновления"),
		Description("Режим обновления куба.")]
		public UpdateMode UpdateMode
		{
			get { return controlBlock.UpdateMode; }
			set { controlBlock.UpdateMode = value; }
		}		

		protected override void WriteToXML(XmlWriter xmlWriter)
		{
			//if (xmlWriter.Settings.ConformanceLevel != ConformanceLevel.Fragment)
			//    xmlWriter.WriteStartDocument();
			try
			{
				//Microsoft.AnalysisServices.Utils.Serialize(xmlWriter, cube, false);
				//xmlWriter.WriteStartElement("cubestructure");
				//OLAPUtils.VersionToXML(xmlWriter, new Version(1, 0, 0, 0));
				xmlWriter.WriteStartElement("cube");
				idInfo.ToXML(xmlWriter);
				xmlWriter.WriteAttributeString("description", Description);				

				xmlWriter.WriteStartElement("cubedimensions");
				for (int i = 0; i < Dimensions.Count; i++)
					Dimensions[i].ToXML(xmlWriter);
				xmlWriter.WriteEndElement(); //cubedimensions

				xmlWriter.WriteStartElement("measuregroups");
				for (int i = 0; i < MeasureGroups.Count; i++)
					MeasureGroups[i].ToXML(xmlWriter);
				for (int i = 0; i < LinkedMeasureGroups.Count; i++)
					LinkedMeasureGroups[i].ToXML(xmlWriter);
				xmlWriter.WriteEndElement();//measuregroups

				xmlWriter.WriteStartElement("mdxscripts");
				for (int i = 0; i < mdxScripts.Count; i++)
					mdxScripts[i].ToXML(xmlWriter);
				xmlWriter.WriteEndElement();//mdxscripts

				OLAPUtils.AnnotationsToXML(xmlWriter, Annotations, versions);

				xmlWriter.WriteEndElement();//cube
				//xmlWriter.WriteEndElement();//cubestructure
			}
			finally
			{
				//if (xmlWriter.Settings.ConformanceLevel != ConformanceLevel.Fragment)
				//    xmlWriter.WriteEndDocument();
				//xmlWriter.Close();
			}
			//return xmlWriter.ToString();
		}

		public XPathNavigator ToNavigator(XmlWriterSettings writerSettings)
		{
			StringBuilder strBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(strBuilder, writerSettings);
			try
			{
				ToXML(xmlWriter);
			}
			finally
			{
				xmlWriter.Flush();
				xmlWriter.Close();
			}
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(strBuilder.ToString());
			return doc.CreateNavigator();
		}
	}


	public abstract class DatabaseInfoBase : VersionedObjectInfo
	{
		protected string description;
		protected DataSourceInfo dataSource;
		protected DataSourceViewInfo dataSourceView;
		protected List<CubeInfoBase> cubes = new List<CubeInfoBase>();
		protected List<CubeInfoBase> virtualCubes = new List<CubeInfoBase>();
		protected List<DimensionInfoBase> dimensions = new List<DimensionInfoBase>();
        /// <summary>
        /// Аннотации
        /// </summary>
		protected Dictionary<string, AnnotationInfo> annotations = new Dictionary<string, AnnotationInfo>();

		public Microsoft.AnalysisServices.Server server;
		public Database DB;
		public Krista.FM.Client.TreeLogger.Log log;

	    /// <summary>
	    /// Список измерений, построенных на поле таблицы. Создаем отдельно отношение,
	    /// в случае когда в кубе используется как измерение, построенное на таблице,
	    /// так и измерение, построенное на поле этой таблицы
	    /// </summary>
	    public static string[] fieldDimensions = new string[]
	                                                 {
                                                         "Источники данных",
	                                                     "Вариант__ГодОтч", "Вариант__МесОтч", "Вариант__Земельный кадастр",
	                                                     "Период__ГодУнив", "Период__МесяцУнив", "Период__ДеньУнив"
	                                                 };

		protected abstract void ReadIdentificationInfo();
		protected abstract void ReadDescription();
		protected abstract void ReadDataSource();
		protected abstract void ReadCubes();
		protected abstract void ReadDimensions();
        /// <summary>
        /// Добавляет пользовательские функции к иформации о базе данных
        /// </summary>
	    protected abstract void ReadAnnotations();

		protected void InitAnnotation()
		{
			annotations.Add("fmadatabase",
				new AnnotationInfo("fmadatabase", "true", AnnotationVisibility.SchemaRowset));
		}

		protected void Init(Database DB, ControlBlock _controlBlock)
		{
			idInfo = new IdentificationInfo(DB.Name, DB.ID);
			description = DB.Description;
			dataSource = new DataSourceInfo(DB.DataSources[0].ConnectionString + ";Password=DV");
			dataSourceView = new DataSourceViewInfo(DB.DataSourceViews[0]);
			controlBlock = _controlBlock;
			versions = OLAPUtils.VersionsFromAnnotations(DB.Annotations);
		}

		public DatabaseInfoBase(XPathNavigator _databaseNavigator)
			: base(_databaseNavigator, new DatabaseControlBlock())
		{
			InitAnnotation();
		}

		public DatabaseInfoBase(
			XPathNavigator _databaseNavigator, DatabaseControlBlock _databaseControlBlock)
			: base(_databaseNavigator, _databaseControlBlock)
		{
			InitAnnotation();
		}

		public DatabaseInfoBase(Database DB, ControlBlock _databaseControlBlock)
			: base(DB, _databaseControlBlock)
		{
			InitAnnotation();
			Init(DB, _databaseControlBlock);
		}

		protected virtual void ReadDatabase()
		{
		    try
		    {
                ReadAnnotations();
                ReadIdentificationInfo();
                ReadDescription();
                ReadDataSource();
                ReadDataSourceView();
                ReadDimensions();
                ReadCubes();
                AddReferenceAttributesToDimensions();
                ReadTableRelations();
                //AddReferenceAttributesToDimensions();
                GetUsedTables();
		    }
		    catch (Exception e)
		    {
		        FormException.ShowErrorForm(e);
		    }
            
		}

		protected virtual void ReadDataSourceView()
		{
		    try
		    {
                dataSourceView = new DataSourceViewInfo();
                dataSourceView.DataSourceName = dataSource.Name;
		    }
		    catch (Exception e)
		    {
                throw new Exception(String.Format("При обработке представления данных возникла ошибка: {0}", e));
            }
		}

		protected virtual List<RelationInfo> ReadMGDimensionTableRelations(
			MeasureGroupDimensionInfo MGDimension)
		{
			List<RelationInfo> relations = new List<RelationInfo>();
			for (int i = 0; i < MGDimension.ReferenceToThis.Count; i++)
			{				
				DimensionInfoBase sharedDim = GetSharedDimensionByID(
					MGDimension.CubeDimIdentification.ID);
				ColumnInfo primaryKey = sharedDim.KeyAttribute.KeyColumns[0];
				relations.Add(new RelationInfo(MGDimension.ReferenceToThis[i], primaryKey));
			}
			return relations;
		}

		protected virtual void AddTableRelationsToList(List<RelationInfo> sortedRelations,
			List<RelationInfo> relationsToInsert, RelationComparer relationComparer)
		{
			for (int i = 0; i < relationsToInsert.Count; i++)
			{
				int index = sortedRelations.BinarySearch(relationsToInsert[i], relationComparer);
				if (index < 0) { sortedRelations.Insert(~index, relationsToInsert[i]); }
			}
		}

		protected virtual void ReadDimensionsTableRelations(RelationComparer relationComparer)
		{
			for (int i = 0; i < dimensions.Count; i++)
				AddTableRelationsToList(
					dataSourceView.TableRelations, dimensions[i].TableRelations, relationComparer);
		}

		protected virtual void ReadCubesTableRelations(RelationComparer relationComparer)
		{
			for (int i = 0; i < cubes.Count; i++)
			{
				for (int j = 0; j < cubes[i].MeasureGroups.Count; j++)
				{
					for (int k = 0; k < cubes[i].MeasureGroups[j].MGDimensions.Count; k++)
					{
                        string name = cubes[i].MeasureGroups[j].MGDimensions[k].CubeDimIdentification.Name;

                        if (IsFieldDimension(name))
                        {
                            AddTableRelationsToList(dataSourceView.TableRelations,
                                ReadMGDimensionTableRelations(cubes[i].MeasureGroups[j].MGDimensions[k]),
                            relationComparer);
                        }
						if (cubes[i].MeasureGroups[j].MGDimensions[k].DimensionType ==
                            MeasureGroupDimensionType.ReferenceDimension)
						{
							AddTableRelationsToList(dataSourceView.TableRelations,
								ReadMGDimensionTableRelations(cubes[i].MeasureGroups[j].MGDimensions[k]),
							relationComparer);
						}
					}
				}
			}
		}

        private bool IsFieldDimension(string name)
        {
            foreach (string fieldDimension in fieldDimensions)
            {
                if (name.Equals(fieldDimension, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

		protected virtual void ReadTableRelations()
		{
		    try
		    {
                RelationComparer relationComparer = new RelationComparer();
                dataSourceView.TableRelations.Sort(relationComparer);
                if (ControlBlock.CreateDimensionsRelations)
                {
                    ReadDimensionsTableRelations(relationComparer);
                }
                //if (ControlBlock.CreateCubesRelations)
                //Создаем отношения только для ReferenceDimension. Они необходимы, когда 
                //такие измерения подключаются через другие ReferenceDimension.
                if (true)
                {
                    ReadCubesTableRelations(relationComparer);
                }
		    }
		    catch (Exception e)
		    {
                throw new Exception(String.Format("При обработке отношений возникла ошибка: {0}", e));
		    }
			
		}

		protected void AddReferenceAttributesToDimension(MeasureGroupDimensionInfo dimInfo)
		{
			if (dimInfo.DimensionType == MeasureGroupDimensionType.ReferenceDimension)
			{
				DimensionInfoBase intermediateDimInfo = GetSharedDimensionByID(
					dimInfo.IntermediateDimIdentification.ID);
				//to-do Проверить почему получаем null
				if (intermediateDimInfo != null)
				{
					bool attributeExist = false;
					for (int l = 0; l < intermediateDimInfo.Attributes.Count; l++)
					{
						if (intermediateDimInfo.Attributes[l].SafeName.Equals(
							dimInfo.IntermediateGranularityAttrID,
							StringComparison.OrdinalIgnoreCase))
						{
							if (!intermediateDimInfo.Attributes[l].Description.
								Contains(dimInfo.CubeDimIdentification.Name))
							{
								intermediateDimInfo.Attributes[l].Description += string.Format(", '{0}'",
									dimInfo.CubeDimIdentification.Name);
							}
							attributeExist = true;
							break;
						}
					}
					if (!attributeExist)
					{
						ColumnInfo sourceColumn = dimInfo.ReferenceToThis[0];
						AttributeInfo attr = intermediateDimInfo.AddMemberAttribute(
							dimInfo.IntermediateGranularityAttrID,
							string.Format("Cсылка на измерение '{0}'",
							dimInfo.CubeDimIdentification.Name), false,
							sourceColumn, sourceColumn, AttributeType.Utility);
					}
				}
			}
		}

		protected void AddReferenceAttributesToDimensions()
		{
		    try
		    {
                for (int i = 0; i < cubes.Count; i++)
                {
                    for (int j = 0; j < cubes[i].MeasureGroups.Count; j++)
                    {
                        for (int k = 0; k < cubes[i].MeasureGroups[j].MGDimensions.Count; k++)
                        {
                            AddReferenceAttributesToDimension(cubes[i].MeasureGroups[j].MGDimensions[k]);
                        }
                    }
                }
		    }
		    catch (Exception e)
		    {

                throw new Exception(e.ToString());
		    }
			
		}

		protected void GetUsedTables()
		{
			for (int i = 0; i < dimensions.Count; i++)
			{
                foreach (TableInfo tableInfo in dimensions[i].UsedTables.Tables)
                {
                    dataSourceView.UsedTables.AddTable(tableInfo);
                }                
			}
			for (int i = 0; i < cubes.Count; i++)
			{
                foreach (TableInfo tableInfo in cubes[i].MeasureGroupTables.Tables)
                {
                    dataSourceView.UsedTables.AddTable(tableInfo);
                }
			}
		}

		public void AddCubeInfo(CubeInfoBase cubeInfo)
		{
			bool cubeExist = false;
			for (int i = 0; i < cubes.Count; i++)
			{
				if (cubes[i].Name.Equals(cubeInfo.Name, StringComparison.OrdinalIgnoreCase))
				{
					cubes[i].ControlBlock = cubeInfo.ControlBlock;
					cubeExist = true;
					break;
				}
			}
			if (!cubeExist) { cubes.Add(cubeInfo); }
		}

		[Browsable(false)]
		public Dictionary<string, AnnotationInfo> Annotations
		{
			get { return annotations; }
		}

		[Browsable(false), Category("Разное"), DisplayName("Строка подключения к хранилищу"),
		Description("Строка подключения к хранилищу.")]
		public string ConnectionString
		{
			get { return dataSource.ConnectionString; }
			set { dataSource.ConnectionString = value; }
		}

		[TypeConverter(typeof(ParamsConverter)),
		Editor(typeof(DictionaryTextEditor), typeof(UITypeEditor)),
		BrowsableAttribute(true), CategoryAttribute("Хранилище"),
		DisplayName("Строка подключения к хранилищу"),
		ReadOnlyAttribute(false)]
		public Dictionary<string, string> ConnectionParams
		{
			get { return dataSource.ConnectionParams; }
			set { dataSource.ConnectionParams = value; }
		}

		[Browsable(true), Category("Разное"), DisplayName("Описание"),
		Description("Описание многомерной базы.")]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		[Browsable(false)]
		public DataSourceInfo DataSource
		{
			get { return dataSource; }
		}

		[Browsable(false)]
		public DataSourceViewInfo DataSourceView
		{
			get { return dataSourceView; }
		}

		[Browsable(false)]
		public List<CubeInfoBase> Cubes
		{
			get { return cubes; }
		}

		[Browsable(false)]
		public List<CubeInfoBase> VirtualCubes
		{
			get { return virtualCubes; }
		}

		[Browsable(false)]
		public List<DimensionInfoBase> Dimensions
		{
			get { return dimensions; }
		}

		[Browsable(false)]
		public UpdateMode UpdateMode
		{
			get { return ControlBlock.UpdateMode; }
			set { ControlBlock.UpdateMode = value; }
		}

		public DimensionInfoBase GetSharedDimensionByName(string dimensionSafeName)
		{
			DimensionInfoBase sharedDimension = null;
			for (int i = 0; i < this.dimensions.Count; i++)
			{
				if (this.dimensions[i].Name.Equals(dimensionSafeName,
					StringComparison.CurrentCultureIgnoreCase))
				{
					sharedDimension = this.dimensions[i];
					break;
				}
			}
			return sharedDimension;
		}

		public DimensionInfoBase GetSharedDimensionByID(string stringID)
		{
			DimensionInfoBase sharedDimension = null;
			for (int i = 0; i < this.dimensions.Count; i++)
			{
				if (this.dimensions[i].ID.Equals(stringID,
					StringComparison.CurrentCultureIgnoreCase))
				{
					sharedDimension = this.dimensions[i];
					break;
				}
			}
			return sharedDimension;
		}

		public DimensionInfoBase GetSharedDimensionByID(Guid id)
		{
			DimensionInfoBase sharedDimension = null;
			for (int i = 0; i < this.dimensions.Count; i++)
			{
				if (this.dimensions[i].ID.Equals(id))
				{
					sharedDimension = this.dimensions[i];
					break;
				}
			}
			return sharedDimension;
		}

		[Browsable(false)]
        public TableDictionary UsedTables
		{
			get { return dataSourceView.UsedTables; }
		}

		[BrowsableAttribute(false)]
		new public DatabaseControlBlock ControlBlock
		{
			get { return controlBlock as DatabaseControlBlock; }
		}

		protected override void WriteToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("database");
			idInfo.ToXML(xmlWriter);
			xmlWriter.WriteAttributeString("description", description);
			xmlWriter.WriteAttributeString("connectionstring", dataSource.ConnectionString);
			OLAPUtils.AnnotationsToXML(xmlWriter, Annotations, versions);
			xmlWriter.WriteEndElement();//database
		}
	}

	public abstract class DimensionInfoBase : VersionedObjectInfo
	{
		protected string description;
		protected string allMemberName;		
		protected List<AttributeInfo> attributes = new List<AttributeInfo>();
		protected Dictionary<string, AnnotationInfo> annotations = new Dictionary<string, AnnotationInfo>();
		protected List<HierarchyInfo> hierarchies = new List<HierarchyInfo>();
		protected List<RelationInfo> tableRelations = new List<RelationInfo>();
		protected DimensionType dimensionType = DimensionType.Regular;
        protected TableDictionary usedTables = new TableDictionary();
		protected DimensionStorageMode storageMode = DimensionStorageMode.Molap;

		protected abstract void ReadIdentificationInfo();
		protected abstract void ReadDescription();
		protected abstract void ReadDimensionType();
		protected abstract void ReadAllMemberName();		
		protected abstract void ReadAttributes();
		protected abstract void ReadAnnotations();
		protected abstract void ReadHierarchies();
		protected abstract void ReadTableRelations();
	    protected abstract void ChangeDateUNV();

		public Dimension Dimension = null;

		protected void Init(Dimension dim)
		{
			idInfo = new IdentificationInfo(dim.Name, dim.ID);
			description = dim.Description;
			allMemberName = dim.AttributeAllMemberName;			
			for (int i = 0; i < dim.Attributes.Count; i++)
				attributes.Add(new AttributeInfo(dim.Attributes[i], dim.DataSourceView));

			for (int i = 0; i < dim.Annotations.Count; i++)
                annotations.Add(dim.Annotations[i].Name, new AnnotationInfo(dim.Annotations[i]));

			for (int i = 0; i < dim.Hierarchies.Count; i++)
				hierarchies.Add(new HierarchyInfo(dim.Hierarchies[i]));

			dimensionType = dim.Type;
			versions = OLAPUtils.VersionsFromAnnotations(dim.Annotations);			
			storageMode = dim.StorageMode;
		}

		protected DimensionInfoBase()
			: base()
		{ }

		public DimensionInfoBase(XPathNavigator _dimensionNavigator)
			: base(_dimensionNavigator, new DimensionControlBlock())
		{ }

		public DimensionInfoBase(
			XPathNavigator _dimensionNavigator, DimensionControlBlock _dimensionControlBlock)
			: base(_dimensionNavigator, _dimensionControlBlock)
		{ }

		public DimensionInfoBase(Dimension dim)
			: base(dim, DimensionControlBlock.ReadFromAnnotations(dim.Annotations))
		{
			Init(dim);
		}

		public DimensionInfoBase(Dimension dim, DimensionControlBlock _dimensionControlBlock)
			: base(dim, _dimensionControlBlock)
		{
			Init(dim);
		}		

		protected virtual void ReadDimension()
		{
			ReadIdentificationInfo();
			ReadDescription();
			ReadDimensionType();
			ReadAllMemberName();
			ReadAnnotations();

			#region Не менять порядок выражений внутри блока!!!
			//ReadKeyAttribute();
			ReadAttributes();
			ReadTableRelations();//
			ReadHierarchies();

            // Редактирование измерения Период.Период
		    ChangeDateUNV();
			#endregion
		}

	   

	    public virtual AttributeInfo AddMemberAttribute(string name, string description,
			bool isVisible, ColumnInfo sourceColumn, ColumnInfo keyColumn, AttributeType attrType)
		{
			AttributeInfo attr = new AttributeInfo(name);
			attr.Description = description;
			attr.AttributeHierarchyVisible = isVisible;
			attr.KeyColumns.Add(keyColumn);
			attr.NameColumn = sourceColumn;
			attr.Type = attrType;
			attr.Usage = AttributeUsage.Regular;
			this.attributes.Add(attr);
			return attr;
		}

		public virtual AttributeInfo AddMemberAttribute(string name, string description,
			bool isVisible, ColumnInfo sourceColumn, AttributeType attrType)
		{
			return AddMemberAttribute(
				name, description, isVisible, sourceColumn, sourceColumn, attrType);
		}

		protected virtual void GetUsedTables()
		{
            usedTables.Clear();
            for (int i = 0; i < attributes.Count; i++)
			{
                for (int j = 0; j < attributes[i].KeyColumns.Count; j++)
                {
                    usedTables.AddTable(attributes[i].KeyColumns[j]);
                }
                if (attributes[i].NameColumn != null)
                {
                    usedTables.AddTable(attributes[i].NameColumn);                    
                }
			}
			for (int i = 0; i < tableRelations.Count; i++)
			{
				usedTables.AddTable(tableRelations[i].ForeignKey);
				usedTables.AddTable(tableRelations[i].PrimaryKey);
			}
		}


		[Browsable(true), Category("Разное"), DisplayName("Наименование таблицы"),
		Description("Наименование таблицы ключевого аттрибута измерения."), MergableProperty(false)]
		public string TableName
		{
			get
			{
				if (KeyAttribute != null && KeyAttribute.KeyColumns.Count > 0)
				{
					return KeyAttribute.KeyColumns[0].TableName;
				}
				else
				{
					return string.Empty;
				}
			}
		}

		[Browsable(true), Category("Разное"), DisplayName("Описание"),
		Description("Описание измерения."), MergableProperty(false)]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		[Browsable(true), Category("Разное"), DisplayName("Тип измерения"),
		Description("Тип измерения (обычное, ссылочное или основанное на таблице фактов)."),
		MergableProperty(false)]
		public DimensionType DimensionType
		{
			get { return dimensionType; }
			set { dimensionType = value; }
		}

		[Browsable(true), Category("Разное"), DisplayName("Наименование уровня ALL"),
		Description("Наименование уровня ALL по умолчанию."), MergableProperty(false)]
		public string AllMemberName
		{
			get { return allMemberName; }
			set { allMemberName = value; }
		}

		[Browsable(false)]
		public List<HierarchyInfo> Hierarchies
		{
			get { return hierarchies; }
		}

		[Browsable(false), Category("Разное"), DisplayName("Ключевой аттрибут"),
		Description("Ключевой аттрибут измерения."), MergableProperty(false)]
		public AttributeInfo KeyAttribute
		{
			get
			{
				for (int i = 0; i < Attributes.Count; i++)
				{
					if (Attributes[i].Usage == AttributeUsage.Key) { return Attributes[i]; }
				}
				return null;
			}
			set
			{
				for (int i = 0; i < Attributes.Count; i++)
				{
					if (Attributes[i].Usage == AttributeUsage.Key)
					{
						Attributes.Remove(Attributes[i]);
						break;
					}
				}
				value.Usage = AttributeUsage.Key;
				Attributes.Add(value);
			}
		}

		[Browsable(false)]
		public List<AttributeInfo> Attributes
		{
			get { return attributes; }
		}

		[Browsable(false)]
		public Dictionary<string, AnnotationInfo> Annotations
		{
			get { return annotations; }
		}

		[Browsable(false)]
		public List<RelationInfo> TableRelations
		{
			get { return tableRelations; }
		}

		[Browsable(false), Category("Разное"), DisplayName("Режим хранения"),
		Description("Режим хранения измерения."), MergableProperty(false)]
		public DimensionStorageMode StorageMode
		{
			get { return storageMode; }
		}

		[Browsable(false)]
		new public DimensionControlBlock ControlBlock
		{
			get { return controlBlock as DimensionControlBlock; }
		}

		//public Version VersionInfo
		//{
		//    get { return versionInfo; }
		//}
		
		[Browsable(true), Category("Управляющая информация"), DisplayName("Читать из файла"),
		Description("Определяет, где искать объект - в файле или в пакете."), ReadOnly(true)]
		public bool ReadFromFile
		{
			get { return controlBlock.ReadFromFile; }
			set { controlBlock.ReadFromFile = value; }
		}

		[Browsable(true), Category("Управляющая информация"), DisplayName("Режим обновления"),
		Description("Режим обновления измерения.")]
		public UpdateMode UpdateMode
		{
			get { return controlBlock.UpdateMode; }
			set { controlBlock.UpdateMode = value; }
		}

		[Browsable(false)]
        public TableDictionary UsedTables
		{
			get { return usedTables; }
		}

		protected override void WriteToXML(XmlWriter xmlWriter)
		{	
			try
			{
				xmlWriter.WriteStartElement("dimension");
				idInfo.ToXML(xmlWriter);
				xmlWriter.WriteAttributeString("description", Description);
				xmlWriter.WriteAttributeString("type", dimensionType.ToString());
				xmlWriter.WriteAttributeString("attributeallmembername", AllMemberName);
				xmlWriter.WriteAttributeString("dimensiontype", DimensionType.ToString());
				xmlWriter.WriteAttributeString("storagemode", StorageMode.ToString());

				xmlWriter.WriteStartElement("attributes");
				for (int i = 0; i < Attributes.Count; i++)
					if (Attributes[i].Type != AttributeType.Utility)
					{
						Attributes[i].ToXML(xmlWriter);
					}
				xmlWriter.WriteEndElement();//attributes

				xmlWriter.WriteStartElement("hierarchies");
				for (int i = 0; i < Hierarchies.Count; i++)
					Hierarchies[i].ToXML(xmlWriter);
				xmlWriter.WriteEndElement();

				OLAPUtils.AnnotationsToXML(xmlWriter, Annotations, versions);				

				xmlWriter.WriteEndElement();//dimension				
			}
			finally
			{
				//if (xmlWriter.Settings.ConformanceLevel != ConformanceLevel.Fragment)
				//{
				//    xmlWriter.WriteEndDocument();
				//    //xmlWriter.Close();
				//}
			}
		}

		public XPathNavigator ToNavigator(XmlWriterSettings writerSettings)
		{
			StringBuilder strBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(strBuilder, writerSettings);
			try
			{
				ToXML(xmlWriter);
			}
			finally
			{
				xmlWriter.Flush();
				xmlWriter.Close();
			}
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(strBuilder.ToString());
			return doc.CreateNavigator();
		}
	}

	public class CubeAttributeInfo
	{
		public AggregationUsage AggregationUsage = AggregationUsage.Default;
		public bool AttributeHierarchyEnabled = true;
		public OptimizationType AttributeHierarchyOptimizedState = OptimizationType.FullyOptimized;
		public bool AttributeHierarchyVisible = true;
		public string AttributeID = string.Empty;

		public CubeAttributeInfo()
		{ }

		public CubeAttributeInfo(AttributeInfo attrInfo)
		{
			AttributeHierarchyEnabled = attrInfo.AttributeHierarchyEnabled;
			AttributeHierarchyOptimizedState = attrInfo.AttributeHierarchyOptimizedState;
			AttributeHierarchyVisible = attrInfo.AttributeHierarchyVisible;
			AttributeID = attrInfo.SafeName;
		}

		public CubeAttributeInfo(AggregationUsage _aggregationUsage, bool _attributeHierarchyEnabled,
			OptimizationType _attributeHierarchyOptimizedState, bool _attributeHierarchyVisible,
			string _attributeID)
		{
			AggregationUsage = _aggregationUsage;
			AttributeHierarchyEnabled = _attributeHierarchyEnabled;
			AttributeHierarchyOptimizedState = _attributeHierarchyOptimizedState;
			AttributeHierarchyVisible = _attributeHierarchyVisible;
			AttributeID = _attributeID;
		}

		public CubeAttributeInfo(CubeAttribute cubeAttr)
		{
			AggregationUsage = cubeAttr.AggregationUsage;
			AttributeHierarchyEnabled = cubeAttr.AttributeHierarchyEnabled;
			AttributeHierarchyOptimizedState = cubeAttr.AttributeHierarchyOptimizedState;
			AttributeHierarchyVisible = cubeAttr.AttributeHierarchyVisible;
			AttributeID = cubeAttr.AttributeID;
		}

		public static CubeAttributeInfo ReadFromXML(XPathNavigator attrNavigator)
		{
			CubeAttributeInfo cubeAttrInfo = new CubeAttributeInfo();
			cubeAttrInfo.AggregationUsage = (AggregationUsage)Enum.Parse(typeof(AggregationUsage),
				attrNavigator.GetAttribute("aggregationusage", ""), true);
			cubeAttrInfo.AttributeHierarchyEnabled = Convert.ToBoolean(
				attrNavigator.GetAttribute("attributehierarchyenabled", ""));
			cubeAttrInfo.AttributeHierarchyOptimizedState = (OptimizationType)Enum.Parse(
				typeof(OptimizationType),
				attrNavigator.GetAttribute("attributehierarchyoptimizedstate", ""), true);
			cubeAttrInfo.AttributeHierarchyVisible = Convert.ToBoolean(
				attrNavigator.GetAttribute("attributehierarchyvisible", ""));
			//cubeAttrInfo.AttributeID = new Guid(attrNavigator.GetAttribute("attributeid", ""));
			cubeAttrInfo.AttributeID = attrNavigator.GetAttribute("attributeid", "");
			return cubeAttrInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("attribute");
			xmlWriter.WriteAttributeString("aggregationusage", AggregationUsage.ToString());
			xmlWriter.WriteAttributeString("attributehierarchyenabled", AttributeHierarchyEnabled.ToString());
			xmlWriter.WriteAttributeString("attributehierarchyoptimizedstate",
				AttributeHierarchyOptimizedState.ToString());
			xmlWriter.WriteAttributeString("attributehierarchyvisible", AttributeHierarchyVisible.ToString());
			xmlWriter.WriteAttributeString("attributeid", AttributeID);
			xmlWriter.WriteEndElement(); //attribute
		}
	}

	public class CubeHierarchyInfo
	{
		public bool Enabled = true;
		public string HierarchyID = string.Empty;
		public OptimizationType OptimizedState = OptimizationType.FullyOptimized;
		public bool Visible = true;

		public CubeHierarchyInfo()
		{ }

		public CubeHierarchyInfo(string _hierarchyID)
		{
			HierarchyID = _hierarchyID;
		}

		public CubeHierarchyInfo(bool _enabled,
			string _hierarchyID, OptimizationType _optimizationState, bool _visible)
		{
			Enabled = _enabled;
			HierarchyID = _hierarchyID;
			OptimizedState = _optimizationState;
			Visible = _visible;
		}

		public CubeHierarchyInfo(CubeHierarchy cubeHier)
		{
			Enabled = cubeHier.Enabled;
			HierarchyID = cubeHier.HierarchyID;
			OptimizedState = cubeHier.OptimizedState;
			Visible = cubeHier.Visible;
		}

		public static CubeHierarchyInfo ReadFromXML(XPathNavigator hierNavigator)
		{
			CubeHierarchyInfo cubeHierarchyInfo = new CubeHierarchyInfo();
			cubeHierarchyInfo.Enabled = Convert.ToBoolean(hierNavigator.GetAttribute("enabled", ""));
			cubeHierarchyInfo.HierarchyID = hierNavigator.GetAttribute("hierarchyid", "");
			cubeHierarchyInfo.OptimizedState = (OptimizationType)Enum.Parse(
				typeof(OptimizationType), hierNavigator.GetAttribute("optimizedstate", ""), true);
			cubeHierarchyInfo.Visible = Convert.ToBoolean(hierNavigator.GetAttribute("visible", ""));
			return cubeHierarchyInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("hierarchy");
			xmlWriter.WriteAttributeString("enabled", Enabled.ToString());
			xmlWriter.WriteAttributeString("hierarchyid", HierarchyID.ToString());
			xmlWriter.WriteAttributeString("optimizedstate", OptimizedState.ToString());
			xmlWriter.WriteAttributeString("visible", Visible.ToString());
			xmlWriter.WriteEndElement(); //hierarchy
		}
	}

	[XmlRootAttribute("CubeDimension", IsNullable = false)]
	public class CubeDimensionInfoBase
	{
		protected CubeInfoBase cubeInfo = null;
		protected XPathNavigator dimensionNavigator = null;

		protected IdentificationInfo identificationInfo;
		protected Guid dimensionID = Guid.Empty;
		protected AggregationUsage allMemberAggregationUsage = AggregationUsage.Default;
		protected string description = "";
		protected HierarchyUniqueNameStyle hierarchyUniqueNameStyle =
			HierarchyUniqueNameStyle.IncludeDimensionName;
		protected MemberUniqueNameStyle memberUniqueNameStyle = MemberUniqueNameStyle.Native;
		protected bool visible = true;

		protected List<CubeAttributeInfo> attributes = new List<CubeAttributeInfo>();
		protected List<CubeHierarchyInfo> hierarchies = new List<CubeHierarchyInfo>();

		protected virtual void ReadIdentificationInfo()
		{ }

		protected virtual void ReadDescription()
		{ }

		protected virtual void ReadAttributes()
		{ }

		protected virtual void ReadHierarchies()
		{ }

		public string ExceptionMessage = null;

		public CubeDimensionInfoBase(CubeInfoBase _cubeInfo, DimensionInfoBase _dimInfo)
		{
			cubeInfo = _cubeInfo;
			IdentificationInfo = new IdentificationInfo(_dimInfo.Name, _dimInfo.ID);
			for (int i = 0; i < _dimInfo.Attributes.Count; i++)
			{
				Attributes.Add(new CubeAttributeInfo(_dimInfo.Attributes[i]));
			}
			for (int i = 0; i < _dimInfo.Hierarchies.Count; i++)
			{
				Hierarchies.Add(new CubeHierarchyInfo(_dimInfo.Hierarchies[i].ID));
			}
		}

		public CubeDimensionInfoBase(CubeInfoBase _cubeInfo)
		{
			cubeInfo = _cubeInfo;
		}

		public CubeDimensionInfoBase(XPathNavigator _dimensionNavigator, CubeInfoBase _cubeInfo)
		{
			dimensionNavigator = _dimensionNavigator;
			cubeInfo = _cubeInfo;
			ReadDimension();
		}

		public CubeDimensionInfoBase(CubeDimension cubeDim)
		{
			identificationInfo = new IdentificationInfo(cubeDim.Name, cubeDim.ID);
			dimensionID = new Guid(cubeDim.DimensionID);
			allMemberAggregationUsage = cubeDim.AllMemberAggregationUsage;
			description = cubeDim.Description;
			hierarchyUniqueNameStyle = cubeDim.HierarchyUniqueNameStyle;
			memberUniqueNameStyle = cubeDim.MemberUniqueNameStyle;
			visible = cubeDim.Visible;

			for (int i = 0; i < cubeDim.Attributes.Count; i++)
				attributes.Add(new CubeAttributeInfo(cubeDim.Attributes[i]));

			for (int i = 0; i < cubeDim.Hierarchies.Count; i++)
				hierarchies.Add(new CubeHierarchyInfo(cubeDim.Hierarchies[i]));
		}

		protected virtual void ReadDimension()
		{
			ReadIdentificationInfo();
			ReadDescription();
			ReadAttributes();
			ReadHierarchies();
		}

		[XmlElement(ElementName = "name")]
		public string Name
		{
			get { return identificationInfo.Name; }
		}

		public string ID
		{
			get { return identificationInfo.ID; }
		}

		public string Description
		{
			get { return description; }
		}

		public IdentificationInfo IdentificationInfo
		{
			get { return identificationInfo; }
			set { identificationInfo = value; }
		}

		public string DimensionID
		{
			get { return ID; }
		}

		public CubeInfoBase CubeInfo
		{
			get { return cubeInfo; }
		}

		public List<CubeHierarchyInfo> Hierarchies
		{
			get { return hierarchies; }
		}

		public List<CubeAttributeInfo> Attributes
		{
			get { return attributes; }
		}		

		public AggregationUsage AllMemberAggregationUsage
		{
			get { return allMemberAggregationUsage; }
		}

		public HierarchyUniqueNameStyle HierarchyUniqueNameStyle
		{
			get { return hierarchyUniqueNameStyle; }
		}

		public MemberUniqueNameStyle MemberUniqueNameStyle
		{
			get { return memberUniqueNameStyle; }
		}

		public bool Visible
		{
			get { return visible; }
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("cubedimension");

			identificationInfo.ToXML(xmlWriter);
			xmlWriter.WriteAttributeString("allmemberaggregationusage", AllMemberAggregationUsage.ToString());
			xmlWriter.WriteAttributeString("description", Description);
			xmlWriter.WriteAttributeString("hierarchyuniquenamestyle", HierarchyUniqueNameStyle.ToString());
			xmlWriter.WriteAttributeString("memberuniquenamestyle", MemberUniqueNameStyle.ToString());
			xmlWriter.WriteAttributeString("visible", Visible.ToString());

			xmlWriter.WriteStartElement("hierarchies");
			for (int i = 0; i < Hierarchies.Count; i++)
				Hierarchies[i].ToXML(xmlWriter);
			xmlWriter.WriteEndElement(); //hierarchies

			xmlWriter.WriteStartElement("attributes");
			for (int i = 0; i < Attributes.Count; i++)
				Attributes[i].ToXML(xmlWriter);
			xmlWriter.WriteEndElement(); //attributes

			xmlWriter.WriteEndElement(); //cubedimension
		}
	}


	public static class OLAPUtils
	{
		//Максимальная длина имени именованого запроса.
        public static readonly int MaxNamedQueryNameLength = 51;

        public static IdentificationInfo ReadIdentificationInfo(
			string name, XPathNavigator guidsNavigator, string XPathGuidNode)
		{
			try
			{
				string safeName = GetSafeName(name);
				Guid guid = Guid.Empty;
				int hash = 0;

				if (guidsNavigator != null)
				{
					XPathNavigator guidNavigator = guidsNavigator.SelectSingleNode(
						String.Format(XPathGuidNode, safeName));
					if (guidNavigator != null)
					{
						guid = new Guid(guidNavigator.SelectSingleNode("GUID").Value);
						hash = guidNavigator.SelectSingleNode("Hash").ValueAsInt;
					}
				}
				return new IdentificationInfo(safeName, guid, hash);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static string SemanticDelimiter
		{
			get { return "__"; }
		}

		public static string GetSafeName(string name)
		{
			string[,] replaceRules = { { ".", SemanticDelimiter }, { "(", "" }, { ")", "" }, { "%", "процент" } };
			for (int i = 0; i < replaceRules.GetLength(0); i++)
				name = name.Replace(replaceRules[i, 0], replaceRules[i, 1]);

			if (name.IndexOf("^") >= 0)
			{
				string[] nameArray = name.Split(new string[] { "^" },
					StringSplitOptions.RemoveEmptyEntries);
				name = nameArray[nameArray.Length - 1];
			}
			return name;
		}

		//protected static string NameWithoutSemantic(string dimensionSafeName)
		//{
		//    return dimensionSafeName.Substring(
		//        dimensionSafeName.IndexOf(OLAPUtils.SemanticDelimiter) +
		//        OLAPUtils.SemanticDelimiter.Length);
		//}

        /// <summary>
        /// Удаляет имя схемы (DV) и двойные кавычки из строки.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string RemoveDV(string sourceString)
        {
            if (!string.IsNullOrEmpty(sourceString))
            {
                return sourceString.Replace("\"DV\".", string.Empty).Replace("DV.", string.Empty).Replace("\"", string.Empty).Trim();
            }
            return sourceString;
        }

        /// <summary>
        /// Получает список полей из строки. Предполагается, что имена таблицы и поля отделены точкой.
        /// </summary>
        /// <param name="sourceString">Исходная строка в которой будем искать поля.</param>
        /// <param name="splitters">Массив разделителей, которые отделяют разные поля в исходной строке.</param>
        /// <param name="columnDictionary">Словарь уже имеющихся полей.</param>
        /// <returns>Количество добавленных полей.</returns>
        public static int GetColumns(string sourceString, string[] delimiters, Dictionary<string, string> columnDictionary)
        {
            int columnsAdded = 0;
            if (!string.IsNullOrEmpty(sourceString))
            {
                sourceString = RemoveDV(sourceString).ToUpperInvariant();
                string[] sourceStringSplitted = sourceString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sourceStringSplitted.Length; i++)
                {
                    string mayBeField = sourceStringSplitted[i].Trim();                    
                    if (mayBeField.IndexOf(".") > -1 && !columnDictionary.ContainsKey(mayBeField))
                    {
                        columnDictionary.Add(mayBeField, mayBeField);
                        columnsAdded++;
                    }                    
                }
            }
            return columnsAdded;
        }

        /// <summary>
        /// Получает имена таблиц из строки и заносит их в словарь.
        /// </summary>
        /// <param name="fromClause">Строка с именами таблиц, разделенными запятой.</param>
        /// <returns>Словарь с именами таблиц.</returns>
        public static Dictionary<string, string> GetAliasedTables(string fromClause)
        {
            Dictionary<string, string> aliasedTables = new Dictionary<string, string>();
            string[] tableNameArray = fromClause.Split(new char[] { ',' });
            for (int i = 0; i < tableNameArray.Length; i++)
            {
                aliasedTables.Add(tableNameArray[i], "t" + (i + 1).ToString());
            }
            return aliasedTables;
        }

        /// <summary>
        /// Строит SQL запрос.
        /// </summary>        
        /// <param name="fromClause"></param>
        /// <param name="joinClause"></param>
        /// <param name="sourceFilter"></param>
        /// <param name="columnDictionary"></param>
        /// <returns>StringBuilder в котором будет сформированый запрос.</returns>
        public static StringBuilder BuildSQLQuery(string fromClause, string joinClause,
            string sourceFilter, Dictionary<string, string> columnDictionary)
        {
            Dictionary<string, string> aliasedTables = GetAliasedTables(fromClause);

            //БЛОК SELECT
            StringBuilder queryStringBuilder = new StringBuilder("SELECT");
            string delimiter = " ";
            foreach (string fieldString in columnDictionary.Keys)
            {
                queryStringBuilder.Append(delimiter + fieldString);
                if (delimiter == " ")
                {
                    delimiter = ", ";
                }
            }            

            //БЛОК FROM
            queryStringBuilder.Append(" FROM " + fromClause);

            //БЛОК WHERE
            if (!string.IsNullOrEmpty(sourceFilter))
            {
                queryStringBuilder.Append(" WHERE " + "(" + sourceFilter + ")");
                if (!string.IsNullOrEmpty(joinClause))
                {
                    queryStringBuilder.Append(" AND " + "(" + joinClause + ")");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(joinClause))
                {
                    queryStringBuilder.Append(" WHERE " + "(" + joinClause + ")");
                }
            }

            ////Заменяем имена таблиц из алиасами
            //foreach (KeyValuePair<string, string> aliasedTable in aliasedTables)
            //{
            //    //Заменяем "ИмяТаблицы.ИмяПоля" на "Алиас.ИмяПоля"
            //    queryStringBuilder = queryStringBuilder.Replace(aliasedTable.Key + ".", aliasedTable.Value + ".");
                
            //    //Заменяем "ИмяТаблицы" на "ИмяТаблицы Алиас"
            //    queryStringBuilder = queryStringBuilder.Replace(aliasedTable.Key + ",", aliasedTable.Key + " " + aliasedTable.Value + ",");
            //    queryStringBuilder = queryStringBuilder.Replace(aliasedTable.Key + " ", aliasedTable.Key + " " + aliasedTable.Value + " ");
            //}

            return queryStringBuilder;
        }

		/// <summary>
		/// Конвертирует данные об имени таблицы и фильтровом условии в данные об именованном запросе (NamedQuery)
        /// в слое представления данных (DataSourceView).
        /// Имя именованном запроса генерируется как "OLAP_" + Имя таблицы + Хэш(Имя измерения).
		/// </summary>
		/// <param name="dimensionSafeName">Имя измерения. Используется при генерации имени именованного запроса.</param>
		/// <param name="tableName">Имя таблицы</param>
		/// <param name="filter">Фильтровое условие.</param>
		/// <returns>Класс TableInfo, заполненный данными об именованном представлении.</returns>
        public static TableInfo GetViewInfo(
            string dimensionSafeName, string fromClause, string joinClause, string sourceFilter, Dictionary<string, string> fieldDictionary)
        {   
            //Избавляемся от имени источника, двойных кавычек и точки.
            fromClause = RemoveDV(fromClause).ToUpperInvariant();            
            joinClause = RemoveDV(joinClause).ToUpperInvariant();            
            
            //sourceFilter нельзя приводить к верхнему регистру, т.к. там могут быть строковые значения полей!!!
            sourceFilter = RemoveDV(sourceFilter);
                        
            //Получаем список полей из соединения (joinClause)
            GetColumns(joinClause, new string[] { "=", "(", ")" }, fieldDictionary);
            //Получаем список полей из фильтра (sourceFilter)
            GetColumns(sourceFilter.ToUpperInvariant(), new string[] { " OR", " AND" , " IN", "=", "<>", "<", ">", "(", ")", " ", ","}, fieldDictionary);

            StringBuilder queryStringBuilder = BuildSQLQuery(fromClause, joinClause, sourceFilter, fieldDictionary);

            //Строим имя именованного запроса.
            int hashCode = dimensionSafeName.GetHashCode();
            string namedQueryName = string.Concat("OLAP__", fromClause.Replace(",", "__"), "__", hashCode.ToString());
            //string namedQueryName = string.Concat("OLAP_", fromClause.Replace(",", "__"), "_", dimensionSafeName);            
            if (namedQueryName.Length > MaxNamedQueryNameLength)
            {
                //При обрезании имени могут получаться дубликаты!
                namedQueryName = namedQueryName.Substring(0, MaxNamedQueryNameLength);
            }

            return new TableInfo(namedQueryName, string.Empty, queryStringBuilder.ToString());
		}

        //public static TableInfo GetViewInfo(string dimensionSafeName, string filter)
        //{
        //    switch (dimensionSafeName)
        //    {
        //        case "Вариант__МесОтч":
        //            return new TableInfo("DATASOURCES", "",
        //                "OLAP_DATASOURCES_MONTHREPORT",
        //                "SUPPLIERCODE = 'ФО' AND DATACODE = 2 AND DATANAME = 'Ежемесячный отчет'", "");
        //        case "Период__Дата принятия":
        //        case "Период__День_с остатками":
        //            return new TableInfo("FX_DATE_YEARDAY", "",
        //                "OLAP_FX_DATE_YEARDAY_WITHBLNC",
        //                "DATEDAY <> 32", "");
        //        case "Период__День":
        //            return new TableInfo("FX_DATE_YEARDAY", "",
        //                "OLAP_FX_DATE_YEARDAY",
        //                "DATEDAY <> 0 and DATEDAY <> 32", "");
        //        case "Период__День_с закл об":
        //        case "Период__День_ФО":
        //        case "Период__День_ФК":
        //            return new TableInfo("FX_DATE_YEARDAY", "",
        //                "OLAP_FX_DATE_YEARDAY_CLSNGTURN",
        //                "DATEDAY <> 0", "");
        //        case "Период_Месяц":
        //            return new TableInfo("FX_DATE_YEARMONTH", "",
        //                "OLAP_FX_DATE_YEARMONTH",
        //                "(not DATEMONTH like 'Квартал%') and (ID <> 19980000) and " +
        //                "(ID <> 19990000) and (ID <> 20000000) and (ID <> 20010000) and " +
        //                "(ID <> 20020000) and (ID <> 20030000) and (ID <> 20040000) and " +
        //                "(ID <> 20050000) and (ID <> 20060000) and (ID <> 20070000) and " +
        //                "(ID <> 20080000) and (ID <> 20090000) and (ID <> 20100000)", "");
        //        case "Период__Год Квартал Месяц":
        //            return new TableInfo("FX_DATE_YEARDAYUNV", "",
        //                "OLAP_FX_DATE_YEARDAYUNV",
        //                "DATEDAYID < -1", "");
        //        default:
        //            return null;
        //    }
        //}

		//public static Version ReadVersionFromXML(XPathNavigator versionNavigator)
		//{
		//    int major = Convert.ToInt32(versionNavigator.GetAttribute("major", ""));
		//    int minor = Convert.ToInt32(versionNavigator.GetAttribute("minor", ""));
		//    int build = Convert.ToInt32(versionNavigator.GetAttribute("build", ""));
		//    int revision = Convert.ToInt32(versionNavigator.GetAttribute("revision", ""));
		//    return new Version(major, minor, build, revision);
		//}

		public static string[] GetNamesFromBinding(Binding source)
		{
			return source.ToString().Split(new string[] { "." },
				StringSplitOptions.RemoveEmptyEntries);
		}

		//public static void VersionToXML(XmlWriter xmlWriter, Version version)
		//{
		//    xmlWriter.WriteStartElement("version");
		//    xmlWriter.WriteAttributeString("major", version.Major.ToString());
		//    xmlWriter.WriteAttributeString("minor", version.Minor.ToString());
		//    xmlWriter.WriteAttributeString("build", version.Build.ToString());
		//    xmlWriter.WriteAttributeString("revision", version.Revision.ToString());
		//    xmlWriter.WriteEndElement();
		//}

		public static XmlWriterSettings GetXmlWriterSettings()
		{
			return GetXmlWriterSettings(true);
		}

		public static XmlWriterSettings GetXmlWriterSettings(bool omitXmlDeclaration)
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

		//public static XmlNode VersionToNode(Version version)
		//{
		//    StringBuilder strBuilder = new StringBuilder();
		//    XmlWriter xmlWriter = XmlWriter.Create(strBuilder, GetXmlWriterSettings());
		//    try
		//    {
		//        xmlWriter.WriteStartDocument();
		//        VersionToXML(xmlWriter, version);
		//        xmlWriter.WriteEndDocument();
		//    }
		//    finally
		//    {
		//        xmlWriter.Flush();
		//        xmlWriter.Close();
		//    }
		//    XmlDocument doc = new XmlDocument();
		//    doc.LoadXml(strBuilder.ToString());
		//    return doc.SelectSingleNode("version");
		//}		

		public static Versions VersionsFromAnnotations(AnnotationCollection annotations)
		{	
			for (int i = 0; i < annotations.Count; i++)
			{
				if (annotations[i].Name.Equals("versions", StringComparison.OrdinalIgnoreCase))
				{	
					return Versions.ReadFromXML(annotations[i].Value.CreateNavigator());
				}
			}
			return null;
		}

		public static Versions VersionsFromAnnotations(Dictionary<string, AnnotationInfo> annotations)
		{
            if (annotations.ContainsKey("versions"))
                return Versions.ReadFromXML(annotations["versions"].Value.CreateNavigator());
			
			return null;
		}

		//public static Version VersionFromAnnotations(AnnotationCollection annotations)
		//{
		//    int major = 1;
		//    int minor = 0;
		//    int build = 0;
		//    int revision = 0;
		//    for (int i = 0; i < annotations.Count; i++)
		//    {
		//        if (annotations[i].Name.Equals("version", StringComparison.OrdinalIgnoreCase))
		//        {
		//            major = Convert.ToInt32(annotations[i].Value.SelectSingleNode("@major").Value);
		//            if (major < 0) { major = 1; }
		//            minor = Convert.ToInt32(annotations[i].Value.SelectSingleNode("@minor").Value);
		//            if (minor < 0) { minor = 1; }
		//            build = Convert.ToInt32(annotations[i].Value.SelectSingleNode("@build").Value);
		//            if (build < 0) { build = 1; }
		//            revision = Convert.ToInt32(annotations[i].Value.SelectSingleNode("@revision").Value);
		//            if (revision < 0) { revision = 1; }
		//        }
		//    }
		//    return new Version(major, minor, build, revision);
		//}		

		public static void AnnotationToXML(XmlWriter xmlWriter,
			string name, AnnotationVisibility visibility, XmlNode value)
		{
			xmlWriter.WriteStartElement("annotation");
			xmlWriter.WriteElementString("name", name);
			xmlWriter.WriteElementString("visibility", visibility.ToString());
			xmlWriter.WriteStartElement("value");
			value.WriteTo(xmlWriter);
			xmlWriter.WriteEndElement();//value
			xmlWriter.WriteEndElement();//annotation
		}

		public static void AnnotationsToXML(XmlWriter xmlWriter, AnnotationCollection annotations)
		{
			xmlWriter.WriteStartElement("annotations");
			for (int i = 0; i < annotations.Count; i++)
			{
				if (annotations[i].Visibility == AnnotationVisibility.SchemaRowset)
					AnnotationToXML(xmlWriter,
						annotations[i].Name, annotations[i].Visibility, annotations[i].Value);
			}
			xmlWriter.WriteEndElement();//annotations
		}

		public static void AnnotationsToXML(XmlWriter xmlWriter, Dictionary<string, AnnotationInfo> annotations)
		{
			xmlWriter.WriteStartElement("annotations");

			foreach(AnnotationInfo info in annotations.Values)
			{
                if (info.Visibility == AnnotationVisibility.SchemaRowset)
					AnnotationToXML(xmlWriter,
                        info.Name, info.Visibility, info.Value);
			}
			xmlWriter.WriteEndElement();//annotations
		}

		public static void AnnotationsToXML
			(XmlWriter xmlWriter, Dictionary<string, AnnotationInfo> annotations, Versions versions)
		{			
			xmlWriter.WriteStartElement("annotations");
			foreach(AnnotationInfo info in annotations.Values)
			{
                if (info.Visibility == AnnotationVisibility.SchemaRowset &&
                    !info.Name.Equals("versions", StringComparison.OrdinalIgnoreCase))
					AnnotationToXML(xmlWriter,
                        info.Name, info.Visibility, info.Value);
			}
			if (versions != null)
			{
				xmlWriter.WriteStartElement("annotation");
				xmlWriter.WriteElementString("name", "versions");
				xmlWriter.WriteElementString("visibility", AnnotationVisibility.None.ToString());

				xmlWriter.WriteStartElement("value");
				versions.WriteToXML(xmlWriter);
				xmlWriter.WriteEndElement();//value

				xmlWriter.WriteEndElement();//annotation
			}
			xmlWriter.WriteEndElement();//annotations
			
		}

		public static string[] GetFilesByMask(string directoryName, string mask)
		{
			return Directory.GetFiles(directoryName, mask, SearchOption.AllDirectories);
		}

		/// <summary>
        /// Возвращает список измерений, использующих таблицу с заданным именем.
		/// </summary>
		/// <param name="dimensions">Список измерений, в котором будем искать.</param>
		/// <param name="tableName">Имя таблицы, которую будем искать.</param>
        /// <returns>Список измерений, использующих таблицу с заданным именем.</returns>
        public static List<ObjectInfo> GetDimensionsByTableName(
			List<DimensionInfoBase> dimensions, string tableName)
		{
            List<ObjectInfo> found = new List<ObjectInfo>();
			for (int i = 0; i < dimensions.Count; i++)
			{
				if (dimensions[i].TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
				{
					found.Add(dimensions[i]);
				}
			}
			return found;
		}

        public static List<ObjectInfo> GetCubesByTableName(List<CubeInfoBase> cubes, string tableName)
		{
            List<ObjectInfo> found = new List<ObjectInfo>();
			for (int i = 0; i < cubes.Count; i++)
			{
				if (cubes[i].MeasureGroupTables.IsOnlyTable(tableName))
				{
					found.Add(cubes[i]);
				}
			}
            return found;
		}

		public static bool IsParentChild(DimensionInfoBase dim)
		{
			return dim.Hierarchies == null || dim.Hierarchies.Count == 0;
		}
	}

	
    /// <summary>
    /// Содержит информацию о таблице: имя, тип (таблица, представление, именованный запрос), SQL-запрос, описание.
    /// </summary>
    public class TableInfo
	{   
		private string _name;
        private TableType _tableType;
		private string _queryDefinition;
		private string _description;
        private string _expression = String.Empty;
        private string _expressionName;
        
        //Имена используемых таблиц. Должны задаваться при задании запроса.

        public static string GetQueryDefinition(string tableName, string filterCondition)
        {
            string query = GetQueryDefinition(tableName);
            if (!string.IsNullOrEmpty(filterCondition))
                return query + " WHERE " + filterCondition;
            return query;
        }

        public static string GetQueryDefinition(string tableName)
        {
            return string.Format("SELECT * FROM {0}", tableName);
        }

        public static string CheckQueryDefinition(string tableName, string queryDefinition)
        {
            if (string.IsNullOrEmpty(queryDefinition))
            {
                return GetQueryDefinition(tableName);
            }
            else
                return queryDefinition;
        }

        private void SetQueryDefinition(string queryDefinition)
        {            
            _queryDefinition = queryDefinition;
            SetTableNameDictionary();
        }

        private void InitTableInfo()
        { }

        private TableInfo()
        {
            TableNameDictionary = new Dictionary<string, string>(1);
        }

        /// <summary>
        /// Создает TableInfo с типом "table". Генерируется запрос вида "SELECT * FROM name".
        /// </summary>
        /// <param name="name">Имя таблицы.</param>
        public TableInfo(string name)
        {
            TableNameDictionary = new Dictionary<string, string>(1);
            _name = name;
            _tableType = TableType.table;
            SetQueryDefinition(CheckQueryDefinition(name, string.Empty));
        }

        public TableInfo(string name, string expression)
            : this(name)
        {
            Expression = expression;
        }

      /*  /// <summary>
        /// Создает TableInfo с типом "table". Генерируется запрос вида "SELECT * FROM name".
        /// </summary>
        /// <param name="name">Имя таблицы.</param>
        /// <param name="description">Описание таблицы.</param>
        public TableInfo(string name, string description) : this(name)
        {
            _description = description;
        }*/
        
        /// <summary>
        /// Создает TableInfo с типом "namedQuery".
        /// </summary>
        /// <param name="name">Имя таблицы.</param>
        /// <param name="description">Описание таблицы.</param>
        /// <param name="queryDefinition">SQL-запрос.</param>
        public TableInfo(string name, string description, string queryDefinition)
        {
            TableNameDictionary = new Dictionary<string, string>(1);
            _name = name;
            _tableType = TableType.view;
            SetQueryDefinition(CheckQueryDefinition(name, queryDefinition));
            _description = description;
        }

        /// <summary>
        /// Создает TableInfo с типом, указанным в параметрах.
        /// </summary>
        /// <param name="name">Имя таблицы.</param>
        /// <param name="description">Описание таблицы.</param>
        /// <param name="queryDefinition">SQL-запрос.</param>
        /// <param name="tableType">Тип таблицы.</param>
        public TableInfo(string name, string description, string queryDefinition, TableType tableType)
        {
            TableNameDictionary = new Dictionary<string, string>(1);
            _name = name;
            _tableType = tableType;
            SetQueryDefinition(CheckQueryDefinition(name, queryDefinition));
            _description = description;
        }

        /// <summary>
        /// Извлекает имена таблиц из SQL запроса и заполняет ими словарь tableNameList.
        /// </summary>
        private void SetTableNameDictionary()
        {
            TableNameDictionary.Clear();
            
            if (!string.IsNullOrEmpty(_queryDefinition))
            {
                int fromIndex = _queryDefinition.IndexOf("from", StringComparison.OrdinalIgnoreCase);
                int whereIndex = _queryDefinition.IndexOf("where", StringComparison.OrdinalIgnoreCase);
                string names;
                if (fromIndex > -1)
                {
                    fromIndex += 5; //Увеличиваем на длину слова "from" + один символ на пробел.
                    if (whereIndex > -1)
                    {
                        names = _queryDefinition.Substring(fromIndex, whereIndex - fromIndex).Trim();
                    }
                    else
                    {
                        names = _queryDefinition.Substring(fromIndex).Trim();
                    }
                    //Получаем имена таблиц и их алиасы.
                    string[] nameAndAliasArray = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    //Разделяем имена таблиц и алиасов, приводим к верхнему регистру и заносим их в словарь.
                    foreach (string nameAndAlias in nameAndAliasArray)
                    {
                        string[] nameArray = nameAndAlias.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameArray.Length == 1)
                        {
                            //Алиаса нет, только имя.
                            TableNameDictionary.Add(nameArray[0].Trim().ToUpperInvariant(), string.Empty);
                        }
                        else
                            if (nameArray.Length == 2)
                            {
                                //Есть и алиас, и имя.
                                TableNameDictionary.Add(nameArray[0].Trim().ToUpperInvariant(), nameArray[1].Trim().ToUpperInvariant());
                            }
                    }
                }
            }
            ////Заносим имя самой TableInfo в список используемых имен.
            //if (!string.IsNullOrEmpty(_name) && !tableNameDictionary.ContainsKey(_name.ToUpperInvariant()))
            //{
            //    tableNameDictionary.Add(_name, string.Empty);
            //}
        }

        /// <summary>
        /// Возвращает true, если искомое имя таблицы содержится в SQL-запросе или совпадает с именем самой TableInfo.
        /// </summary>
        /// <param name="tableName">Искомое имя таблицы.</param>
        /// <returns>True, если искомое имя таблицы содержится в SQL-запросе или совпадает с именем самой TableInfo.</returns>
        public bool BasedOnTable(string tableName)
        {
            return this.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase) | (TableNameDictionary.ContainsKey(tableName.ToUpperInvariant()));
        }

        /// <summary>
        /// Возвращает true, если в SQL-запросе содержится хотя бы одно имя таблицы из проверяемой TableInfo.
        /// </summary>
        /// <param name="other">Проверяемая TableInfo.</param>
        /// <returns>True, если искомое имя таблицы содержится в SQL-запросе или совпадает с именем самой TableInfo.</returns>
        public bool BasedOnTable(TableInfo other)
        {
            //Проверяем на совпадение с именем самой TableInfo.
            bool tableFound = this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
            //Не совпадает - ищем среди используемых таблиц.
            if (!tableFound)
            {
                foreach (string name in other.TableNameDictionary.Keys)
                {
                    if (BasedOnTable(name))
                    {
                        tableFound = true;
                        break;
                    }
                }
            }            
            return tableFound;
        }

        /// <summary>        
        /// Список имен таблиц, используемых в запросе.
        /// </summary>
        public Dictionary<string, string> TableNameDictionary { get; set; }

        /// <summary>
		/// Имя таблицы или представления.
        /// В случае таблицы совпадает с именем, содержащимся в TableNameDictionary.
        /// В случае представления может отличаться от имен в TableNameDictionary.
		/// </summary>
        public string Name
		{
			get { return _name; }			
		}        

		public string QueryDefinition
		{
			get { return _queryDefinition; }
            set { _queryDefinition = value; }
		}        

        public TableType TableType
        {
            get { return _tableType; }
        }

        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                if (String.IsNullOrEmpty(_expressionName) && !String.IsNullOrEmpty(value))
                    _expressionName = "MOD_" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                _expression = value;
            }
        }

        public string ExpressionName
        {
            get { return _expressionName; }
            set { _expressionName = value; }
        }

        public static TableInfo ReadFromXML(XPathNavigator tableNavigator)
		{            
            string name = tableNavigator.GetAttribute("name", "");
            TableType tableType = 
                (TableType)Enum.Parse(typeof(TableType), tableNavigator.GetAttribute("tabletype", ""), true);
            string queryDefinition = tableNavigator.GetAttribute("querydefinition", "");
            string description = tableNavigator.GetAttribute("description", "");
            return new TableInfo(name, description, queryDefinition, tableType);
		}

		public void ToXML(XmlWriter xmlWriter)
		{            
            xmlWriter.WriteStartElement("tableinfo");            
            xmlWriter.WriteAttributeString("name", this._name);
            xmlWriter.WriteAttributeString("tabletype", this._tableType.ToString());            
            xmlWriter.WriteAttributeString("querydefinition", this._queryDefinition);
            xmlWriter.WriteAttributeString("description", this._description);
            xmlWriter.WriteEndElement();//tableinfo
		}
	}

	public class IdentificationList : List<IdentificationInfo>
	{
		public bool Contains(string id)
		{
			for (int i = 0; i < Count; i++)
			{
				if (base[i].ID.Equals(id, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		public void ToXML(XmlWriter xmlWriter, string tagName, string itemTagName)
		{
			xmlWriter.WriteStartElement(tagName);
			for (int i = 0; i < Count; i++)
			{
				xmlWriter.WriteStartElement(itemTagName);
				xmlWriter.WriteAttributeString("id", base[i].ID);
				xmlWriter.WriteAttributeString("name", base[i].Name);
				xmlWriter.WriteEndElement();//itemTagName
			}
			xmlWriter.WriteEndElement();//tagName
		}

		public static IdentificationList ReadFromXML(XPathNavigator navigator, string itemTagName)
		{
			IdentificationList idList = new IdentificationList();
			XPathNodeIterator nodes = navigator.Select("//" + itemTagName);
			while (nodes.MoveNext())
			{
				string id = nodes.Current.GetAttribute("id", "");
				string name = nodes.Current.GetAttribute("name", "");
				idList.Add(new IdentificationInfo(name, id));
			}
			return idList;
		}
	}

	public class IdentificationInfo : ICloneable
	{
		string name;
		string id;
		int hash;

		private IdentificationInfo()
		{ }

		public IdentificationInfo(string _name, string _id)
		{
			name = _name;
			id = _id;
			hash = id.GetHashCode();
		}

		public IdentificationInfo(string _name, Guid _id)
		{
			name = _name;
			id = _id.ToString();
			hash = id.GetHashCode();
		}

		public IdentificationInfo(string _name, Guid _id, int _hash)
		{
			name = _name;
			id = _id.ToString();
			hash = _hash;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Guid GUID
		{
			get
			{
				try
				{
					return new Guid(id);
				}
				catch (Exception)
				{

					return Guid.Empty;
				}
			}
			set
			{
				id = value.ToString();
			}
		}

		public int Hash
		{
			get { return hash; }
		}

		public string ID
		{
			get { return id; }
			set { id = value; }
		}		

		public static IdentificationInfo ReadFromXML(XPathNavigator objectNavigator)
		{
			string name = objectNavigator.GetAttribute("name", "");
			string id = objectNavigator.GetAttribute("id", "");
			return new IdentificationInfo(name, id);
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteAttributeString("id", ID);
			xmlWriter.WriteAttributeString("name", Name);
		}

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();			
		}

		#endregion
	}

	public enum PackageFormat
	{
		[BrowsableAttribute(false)]
		Package2000 = 0,
		[DescriptionAttribute("Пустой пакет (emptypack)")]
		EmptyPack = 1,
		[DescriptionAttribute("Конфигурационный пакет (configpack)")]
		ConfigPack = 2,
		[DescriptionAttribute("Автономный пакет (fullpack)")]
		FullPack = 3,
	}


	public class PartitionInfo
	{
		public string AggregationPrefix = string.Empty;
		public string Description = string.Empty;
		public IdentificationInfo identificationInfo;
		public ProcessingMode ProcessingMode = ProcessingMode.Regular;
		public int ProcessingPriority = 0;
		public string Slice = string.Empty;
		public TableInfo Source;
		public string StorageLocation = string.Empty;
		public StorageMode StorageMode = StorageMode.Molap;
        protected Dictionary<string, AnnotationInfo> annotations;

		public PartitionInfo()
		{
            annotations = new Dictionary<string, AnnotationInfo>();
        }

		public PartitionInfo(Partition part)
            : this()
		{
			identificationInfo = new IdentificationInfo(part.Name, part.ID);
			AggregationPrefix = part.AggregationPrefix;
			Description = part.Description;
			ProcessingMode = part.ProcessingMode;
			ProcessingPriority = part.ProcessingPriority;
			Slice = part.Slice;

			if (part.Source is TableBinding)
			{
				TableBinding tb = part.Source as TableBinding;
				Source = new TableInfo(tb.DbTableName, part.Description);
			}
			else
			{
				QueryBinding qb = part.Source as QueryBinding;
				Source = new TableInfo(string.Empty, part.Description, qb.QueryDefinition, TableType.view);
                //Source = new TableInfo(string.Empty, part.Description, string.Empty, string.Empty,
                //    qb.QueryDefinition);
			}

			StorageLocation = part.StorageLocation;
			StorageMode = part.StorageMode;
		}

        [Browsable(false)]
        public Dictionary<string, AnnotationInfo> Annotations
        {
            get { return annotations; }
        }

        public static PartitionInfo ReadFromXML(XPathNavigator partNavigator)
		{
			PartitionInfo partInfo = new PartitionInfo();
			partInfo.identificationInfo = IdentificationInfo.ReadFromXML(partNavigator);
			partInfo.AggregationPrefix = partNavigator.GetAttribute("description", "");
			partInfo.ProcessingMode = (ProcessingMode)Enum.Parse(typeof(ProcessingMode),
				partNavigator.GetAttribute("processingmode", ""));
			partInfo.ProcessingPriority =
				Convert.ToInt32(partNavigator.GetAttribute("processingpriority", ""));
			partInfo.Slice = partNavigator.GetAttribute("slice", "");
			partInfo.Source = TableInfo.ReadFromXML(partNavigator.SelectSingleNode(".//tableinfo"));
			partInfo.StorageLocation = partNavigator.GetAttribute("storagelocation", "");
			partInfo.StorageMode = (StorageMode)Enum.Parse(typeof(StorageMode),
				partNavigator.GetAttribute("storagemode", ""));
			return partInfo;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("partition");
			xmlWriter.WriteAttributeString("id", identificationInfo.ID);
			xmlWriter.WriteAttributeString("name", identificationInfo.Name);
			xmlWriter.WriteAttributeString("aggregationprefix", AggregationPrefix);
			xmlWriter.WriteAttributeString("description", Description);
			xmlWriter.WriteAttributeString("processingmode", ProcessingMode.ToString());
			xmlWriter.WriteAttributeString("processingpriority", ProcessingPriority.ToString());
			xmlWriter.WriteAttributeString("slice", Slice);
			xmlWriter.WriteAttributeString("storagelocation", StorageLocation);
			xmlWriter.WriteAttributeString("storagemode", StorageMode.ToString());
			Source.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();//partition
		}
	}	

	public class ObjectInfo : ICloneable
	{
		protected IdentificationInfo idInfo;
		protected XPathNavigator navigator;
		protected NamedComponent namedComponent;
		protected ControlBlock controlBlock;

        /// <summary>
        /// Контекстное меню объекта
        /// </summary>
        //public IGContextMenu contextMenu = new IGContextMenu();

		public Exception BlockRestoreReason;

		public ObjectInfo()
		{}

		public ObjectInfo(XPathNavigator _navigator, ControlBlock _controlBlock)
		{
			navigator = _navigator;
			controlBlock = _controlBlock;
		}
		
		public ObjectInfo(NamedComponent _namedComponent, ControlBlock _controlBlock)
		{
			namedComponent = _namedComponent;
			controlBlock = _controlBlock;
		}

		[Browsable(true), Category("Идентификация"), DisplayName("Наименование"),
		Description("Наименование объекта."), MergableProperty(false)]
		public virtual string Name
		{
			get
			{
				if (idInfo == null)
				{
					return string.Empty;
				}
				else
				{
					return idInfo.Name;
				}

			}
			set
			{
				if (idInfo == null)
				{
					idInfo = new IdentificationInfo(value, string.Empty);
				}
				else
				{
					idInfo.Name = value;
				}
			}
		}

		[Browsable(true), Category("Идентификация"), DisplayName("Идентификатор"),
		Description("Идентификатор объекта."), MergableProperty(false)]
		public virtual string ID
		{
			get
			{
				if (idInfo == null)
				{
					return string.Empty;
				}
				else
				{
					return idInfo.ID;
				}

			}
			set
			{
				if (idInfo == null)
				{
					idInfo = new IdentificationInfo(string.Empty, value);
				}
				else
				{
					idInfo.ID = value;
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

		[Browsable(false)]
		public XPathNavigator Navigator
		{
			get { return navigator; }
			set { navigator = value; }
		}

		[Browsable(false)]
		public NamedComponent NamedComponent
		{
			get { return namedComponent; }
		}

		[Browsable(false)]
		public virtual IdentificationInfo IdentificationInfo
		{
			get { return idInfo; }
			set { idInfo = value; }
		}

		[Browsable(false)]
		public virtual ControlBlock ControlBlock
		{
			get { return controlBlock; }
			set { controlBlock = value; }
		}		

		protected virtual void WriteToXML(XmlWriter xmlWriter)
		{ }

		public void ToXML(XmlWriter xmlWriter)
		{
			//--- Свойства объекта могут измениться с тех пор, как объект был проинициализирован из xml.
			//Поэтому сохранять нафигатор некорректно - он уже устарел.

			//if (navigator != null) { xmlWriter.WriteNode(navigator, true); }
			//else WriteToXML(xmlWriter);
			//---

			WriteToXML(xmlWriter);
		}

		public virtual XmlNode ToXMLNode()
		{
			StringBuilder strBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(strBuilder, OLAPUtils.GetXmlWriterSettings());
			try
			{
				xmlWriter.WriteStartDocument();
				ToXML(xmlWriter);
				xmlWriter.WriteEndDocument();
			}
			finally
			{
				xmlWriter.Flush();
				xmlWriter.Close();
			}
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(strBuilder.ToString());
			return doc.FirstChild;
		}

	    public virtual void InitializeContextMenu()
        {}

		#region ICloneable Members

		public virtual object Clone()
		{
			ObjectInfo clonedObject = (ObjectInfo)this.MemberwiseClone();
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

	public class VersionedObjectInfo : ObjectInfo
	{
        
		protected Versions versions = new Versions();

		public VersionedObjectInfo() : base()
		{ }

		public VersionedObjectInfo(NamedComponent _namedComponent, ControlBlock _controlBlock)
			: base(_namedComponent, _controlBlock)
		{ }

		public VersionedObjectInfo(XPathNavigator _navigator, ControlBlock _controlBlock)
			: base(_navigator, _controlBlock)
		{ }

		[Browsable(false)]
		public virtual Versions Versions
		{
			get { return versions; }
			set { versions = value; }
		}

		public DetailedVersionInfo GetLastVersion()
		{
			return versions.GetLastVersion();
		}		

		public virtual void InitVersionInfo()
		{ }

        [MenuAction("AlterScript", ImageIndex = 1)]
        public void AlterScript()
        {
            ICommand command = new AlterScriptCommand(namedComponent);
            command.Execute();
        }

        [MenuAction("CreateScript", ImageIndex = 0)]
        public void CreateScript()
        {
            ICommand command = new CreateScriptCommand(namedComponent);
            command.Execute();
        }

        [MenuAction("DeleteScript", ImageIndex = 2)]
        public void DeleteScript()
        {
            ICommand command = new DeleteScriptCommand(namedComponent);
            command.Execute();
        }
	}


	public static class PackageFormatResolver
	{
		public static PackageFormat GetPackageFormat(XPathNavigator packageNavigator)
		{
			if (packageNavigator.SelectSingleNode("XMLDSOConverter") != null)
				return PackageFormat.Package2000;
			else
				return PackageFormat.EmptyPack;
		}
	}
}