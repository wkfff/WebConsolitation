using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using Microsoft.AnalysisServices;

using Krista.FM.Client.OLAPStructures.DSOEnums;
using Krista.FM.Client.OLAPResources;

namespace Krista.FM.Client.OLAPStructures
{
	class DimensionInfo2000 : DimensionInfoBase
	{
		protected DatabaseInfoBase databaseInfo;
		protected TableInfo olapView;
        private const string xPathFromClause = "./property[@name='FromClause']";
        private const string xPathJoinClause = "./property[@name='JoinClause']";
		private const string xPathSourceTableFilter = "./property[@name='SourceTableFilter']";        

		private DimensionMDXName2000 mdxName;
		private string viewName = string.Empty;
		private string sourceTableFilter = string.Empty;

		protected CubeInfoBase cubeInfo = null;
		protected string name;

		protected Stack<CalculationInfo> calculations = new Stack<CalculationInfo>();

		public DimensionInfo2000(DatabaseInfoBase _databaseInfo, XPathNavigator _dimensionNavigator,
			CubeInfoBase _cubeInfo)
			: base(_dimensionNavigator)
		{
			databaseInfo = _databaseInfo;
			cubeInfo = _cubeInfo;
			ReadDimension();
		}

		public DimensionMDXName2000 MDXName
		{
			get
			{
				if (mdxName != null)
					return mdxName;
				else
				{
					if (!name.Equals(idInfo.Name))
						mdxName = new DimensionMDXName2000(name, idInfo.Name);
					return mdxName;
				}
			}
		}

		protected override void ReadDimension()
		{	
			ReadName();
			//CreateKeyAttribute();
			base.ReadDimension();
			//CreateKeyAttribute();
			InitKeyColumnForKeyAttribute();
			ReadOLAPView();
			GetUsedTables();

            // дополнительные аннтации для обновления DataSourceView
            CreateDsvNameAnnotation();
		}

		protected void ReadName()
		{
			name = navigator.GetAttribute("name", "");
		}

		private Guid GetGuidByName(string dimensionSafeName)
		{
			for (int i = 0; i < databaseInfo.Dimensions.Count; i++)
			{
				if (databaseInfo.Dimensions[i].Name.Equals(dimensionSafeName,
					StringComparison.OrdinalIgnoreCase))
				{
					return databaseInfo.Dimensions[i].IdentificationInfo.GUID;
				}
			}
			return Guid.NewGuid();
		}

		protected override void ReadIdentificationInfo()
		{
            //idInfo = OLAPUtils.ReadIdentificationInfo(
            //    name, ((DatabaseInfo2000)databaseInfo).GUIDsNavigator,
            //    ".//SharedDimensions/SharedDimension[Name=\"{0}\"]");
            //if (idInfo.GUID.Equals(Guid.Empty))
            //    idInfo.GUID = GetGuidByName(Name);            
            
            name = navigator.GetAttribute("name", "");            
            if (name.IndexOf("^") >= 0)
            {
                name = name.Substring(name.IndexOf("^") + 1);
            }
            string safeName = OLAPUtils.GetSafeName(name);
            idInfo = new IdentificationInfo(safeName, OLAPUtils2000.ReadID(navigator, "ID", Guid.NewGuid()));
		}

		protected override void ReadDescription()
		{
			description = navigator.GetAttribute("Description", "");
		}

		protected override void ReadDimensionType()
		{
			if (this.Name.StartsWith("Период", StringComparison.OrdinalIgnoreCase))
			{
				dimensionType = Microsoft.AnalysisServices.DimensionType.Time;
			}
			else
			{
				dimensionType = Microsoft.AnalysisServices.DimensionType.Regular;
			}
		}

		protected override void ReadAllMemberName()
		{

		}

		//protected void ReadSafeTableName()
		//{	
		//    if (attributes.Count > 0)			
		//        safeTableName = attributes[0].KeyColumns[0].TableName;
		//    if (KeyAttribute.KeyColumns.Count == 0)
		//        KeyAttribute.KeyColumns.Add(new ColumnInfo(safeTableName, "ID"));
		//    if (KeyAttribute.NameColumn == null)
		//        KeyAttribute.NameColumn = new ColumnInfo(safeTableName, "ID");
		//    //else
		//    //    if (tableRelations.Count > 0)
		//    //        safeTableName = tableRelations[0].ForeignKey.TableName;
		//}

		protected void InitKeyColumnForKeyAttribute()
		{
			string tableName = string.Empty;
			for (int i = 0; i < attributes.Count; i++)
			{
				if (attributes[i].Usage != AttributeUsage.Key)
				{
					tableName = attributes[i].KeyColumns[0].TableName;
					break;
				}
			}

			if (KeyAttribute.KeyColumns.Count == 0)
				KeyAttribute.KeyColumns.Add(new ColumnInfo(tableName, "ID"));
			if (KeyAttribute.NameColumn == null)
				KeyAttribute.NameColumn = new ColumnInfo(tableName, "ID");
			//else
			//    if (tableRelations.Count > 0)
			//        safeTableName = tableRelations[0].ForeignKey.TableName;
		}

		private void ReplaceAttributesTableName()
		{
			for (int j = 0; j < KeyAttribute.KeyColumns.Count; j++)
				KeyAttribute.KeyColumns[j].UpdateTableInfo(olapView);
			KeyAttribute.NameColumn.UpdateTableInfo(olapView);
			for (int i = 0; i < attributes.Count; i++)
			{
				for (int j = 0; j < attributes[i].KeyColumns.Count; j++)
					attributes[i].KeyColumns[j].UpdateTableInfo(olapView);
				attributes[i].NameColumn.UpdateTableInfo(olapView);
			}
		}

		protected void ClearRollUp()
		{
			for (int i = 0; i < Attributes.Count; i++)
			{
				if (Attributes[i].CustomRollUpInfo != null)
				{
					Attributes[i].CustomRollUpInfo = null;
				}
			}
		}

        private void AddFieldToDictionary(Dictionary<string, string> fieldDictionary, string field)
        {   
            if (!fieldDictionary.ContainsKey(field))
            {
                fieldDictionary.Add(field, field);
            }
        }

        protected Dictionary<string, string> GetFieldDictionary()
        {
            Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();            
            foreach (AttributeInfo attributeInfo in Attributes)
            {
                foreach (ColumnInfo columnInfo in attributeInfo.KeyColumns)
                {   
                    AddFieldToDictionary(fieldDictionary, columnInfo.ToString());
                }                
                AddFieldToDictionary(fieldDictionary, attributeInfo.NameColumn.ToString());
            }
            return fieldDictionary;
        }

		protected void ReadOLAPView()
		{			
            if (navigator.SelectSingleNode(xPathSourceTableFilter) != null)
            {
                string fromClause = navigator.SelectSingleNode(xPathFromClause).Value;
                string joinClause = navigator.SelectSingleNode(xPathJoinClause).Value;
                string sourceFilter = navigator.SelectSingleNode(xPathSourceTableFilter).Value;
                
                if (!string.IsNullOrEmpty(sourceFilter))
                {
                    olapView = OLAPUtils.GetViewInfo(
                        this.Name, fromClause, joinClause, sourceFilter, GetFieldDictionary());
                }                
            }
			if (olapView != null)
			{
				if (cubeInfo == null)
				{
					ClearRollUp();
				}
				ReplaceAttributesTableName();
				OLAPUtils2000.ReplaceRelationsTableName(tableRelations, olapView);				
			}
		}

		public TableInfo OlapView
		{
			get { return olapView; }
		}

		protected override void ReadTableRelations()
		{
			tableRelations = OLAPUtils2000.ReadRelations(navigator);
		}

		//protected override void ReadKeyAttribute()
		//{
		//    keyAttribute = new AttributeInfo("ID");
		//    keyAttribute.Usage = AttributeUsage.Key;            
		//    keyAttribute.Description = "Ключевой аттрибут измерения";
		//    keyAttribute.AttributeHierarchyVisible = false;
		//    keyAttribute.OrderBy = OrderBy.Key;
		//}

		protected override void ReadAttributes()
		{
			//AttributeInfo keyAttribute = new AttributeInfo("ID");
			//keyAttribute.Usage = AttributeUsage.Key;
			//keyAttribute.Description = "Ключевой аттрибут измерения";
			//keyAttribute.AttributeHierarchyVisible = false;
			//keyAttribute.OrderBy = OrderBy.Key;
			//KeyAttribute = keyAttribute;
		}		

		protected override void ReadAnnotations()
		{
			OLAPUtils2000.ReadCustomProperties(navigator, annotations);
			OLAPUtils2000.CreateVersionAnnotations(versions, annotations);
		}

        /// <summary>
        /// Создает в аннотации имя объекта в DataSourceView и QueryDefinituion, если он есть
        /// </summary>
	    private void CreateDsvNameAnnotation()
	    {
            if(olapView != null)
            {
                AnnotationInfo dsvNameAnnotation = new AnnotationInfo("dsvNameAnnotation", olapView.Name);
                annotations.Add("dsvNameAnnotation", dsvNameAnnotation);

                AnnotationInfo dsvQueryDefinitionAnnotation = new AnnotationInfo("dsvQueryDefinition",
                                                                                 String.Format("<![CDATA[{0}]]>",
                                                                                               olapView.QueryDefinition));
                annotations.Add("dsvQueryDefinition", dsvQueryDefinitionAnnotation);

                return;
            }

	        string fromClause = string.Empty;
            if (navigator.SelectSingleNode(xPathSourceTableFilter) != null)
                fromClause = navigator.SelectSingleNode(xPathFromClause).Value;

            if (!String.IsNullOrEmpty(fromClause))
            {
                fromClause =
                    fromClause.Replace("\"DV\".", string.Empty).Replace("DV.", string.Empty).Replace("\"",string.Empty).Trim();
                AnnotationInfo dsvNameAnnotation = new AnnotationInfo("dsvNameAnnotation", fromClause);
                annotations.Add("DsvSourceName", dsvNameAnnotation);
            }

            string joinClause = string.Empty;
            if (navigator.SelectSingleNode(xPathJoinClause) != null)
                joinClause = navigator.SelectSingleNode(xPathJoinClause).Value;

            if (!String.IsNullOrEmpty(joinClause))
            {
                AnnotationInfo joinAnnotation = new AnnotationInfo("joinAnnotation", joinClause);
                annotations.Add("JoinClause", joinAnnotation);
            }
	    }

	    private bool ReadBoolFromXML(string attributeName, bool defaultValue)
		{
			string strBoolean = navigator.GetAttribute(attributeName, "");
			if (strBoolean != string.Empty)
				return Convert.ToBoolean(strBoolean);
			else
				return defaultValue;

		}

		protected override void ReadHierarchies()
		{
            string hierarchyID = Guid.NewGuid().ToString();
            if (annotations.ContainsKey("HierarchyID"))
                hierarchyID = annotations["HierarchyID"].Value.InnerText;

            HierarchyInfo hier = new HierarchyInfo(new IdentificationInfo(
            OLAPUtils2000.GetHierarchyName(this.Name), hierarchyID));
            hier.Description = this.Description;
            hier.AllowDuplicateNames = ReadBoolFromXML("AllowSiblingsWithSameName", false);
            hier.MemberNamesUnique = ReadBoolFromXML("AreMemberNamesUnique", false);
            GetLevels(ref hier);
            if (KeyAttribute == null) { CreateKeyAttribute(); }
            if (hier.Levels.Count > 0) { hierarchies.Add(hier); }
        }

        /// <summary>
        /// Манипуляции с измерением Период.Период
        /// </summary>
        protected override void ChangeDateUNV()
        {
            #region Манипуляции с измерением Период.Период

            if (name == "Период.Период")
            {
                // Удаление атрибута Orderbydefault

                List<AttributeInfo> remoteAtttribute = new List<AttributeInfo>();
                foreach (AttributeInfo info in attributes)
                {
                    if (info.OrderByAttributeID == "Orderbydefault")
                    {
                        info.OrderBy = OrderBy.Key;
                        info.OrderByAttributeID = String.Empty;
                    }

                    List<AttrRelationshipInfo> attrRelationShip = new List<AttrRelationshipInfo>();
                    foreach (AttrRelationshipInfo ship in info.AttrRelationShips)
                    {
                        if (ship.AttributeID == "Orderbydefault")
                            attrRelationShip.Add(ship);
                    }

                    foreach (AttrRelationshipInfo ship in attrRelationShip)
                        info.AttrRelationShips.Remove(ship);

                    if (info.LevelName == "Orderbydefault")
                        remoteAtttribute.Add(info);
                }

                foreach (AttributeInfo info in remoteAtttribute)
                    attributes.Remove(info);

                // добавленеи новых атрбутов

                AddMemberAttribute("DATEYEARID", "Идентификатор года", false,
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEYEARID"),
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEYEARID"), AttributeType.Regular);

                AddMemberAttribute("DATEHALFYEARID", "Идентификатор полугодия", false,
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEHALFYEARID"),
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEHALFYEARID"), AttributeType.Regular);

                AddMemberAttribute("DATEQUARTERID", "Идентификатор квартала", false,
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEQUARTERID"),
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEQUARTERID"), AttributeType.Regular);

                AddMemberAttribute("DATEMONTHID", "Идентификатор месяца", false,
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEMONTHID"),
                   new ColumnInfo("FX_DATE_YEARDAYUNV.DATEMONTHID"), AttributeType.Regular);

                // добавление связей
                foreach (AttributeInfo attributeInfo in attributes)
                {
                    switch (attributeInfo.LevelName)
                    {
                        case "Год":
                            attributeInfo.AttrRelationShips.Clear();
                            attributeInfo.OrderByAttributeID = "DATEYEARID";
                            attributeInfo.AttrRelationShips.Add(new AttrRelationshipInfo("PKID", "DATEYEARID", Cardinality.Many,
                                                                                         RelationshipType.Flexible, true));
                            break;
                        case "Полугодие":
                            attributeInfo.AttrRelationShips.Clear();
                            attributeInfo.OrderByAttributeID = "DATEHALFYEARID";
                            attributeInfo.AttrRelationShips.Add(new AttrRelationshipInfo("PKID", "DATEHALFYEARID", Cardinality.Many,
                                                                                         RelationshipType.Flexible, true));
                            break;
                        case "Квартал":
                            attributeInfo.AttrRelationShips.Clear();
                            attributeInfo.OrderByAttributeID = "DATEQUARTERID";
                            attributeInfo.AttrRelationShips.Add(new AttrRelationshipInfo("PKID", "DATEQUARTERID", Cardinality.Many,
                                                                                         RelationshipType.Flexible, true));
                            break;
                        case "Месяц":
                            attributeInfo.AttrRelationShips.Clear();
                            attributeInfo.OrderByAttributeID = "DATEMONTHID";
                            attributeInfo.AttrRelationShips.Add(new AttrRelationshipInfo("PKID", "DATEMONTHID", Cardinality.Many,
                                                                                         RelationshipType.Flexible, true));
                            break;
                        case "PKID":
                            attributeInfo.KeyColumns.Clear();
                            ColumnInfo columnInfo = new ColumnInfo("FX_DATE_YEARDAYUNV.ID");
                            attributeInfo.KeyColumns.Add(columnInfo);
                            attributeInfo.NameColumn = columnInfo;

                            break;
                    }
                }
            }

            #endregion Манипуляции с измерением Период.Период
        }

		//private ColumnInfo GetKeyColumnForUnits(ColumnInfo sourceColumn)
		//{
		//    if (!this.TableName.Equals(sourceColumn.Table.TableName, StringComparison.OrdinalIgnoreCase) &&
		//        (sourceColumn.Table.TableName.Equals("DV_UNITS_OKEI", StringComparison.OrdinalIgnoreCase) ||
		//        sourceColumn.Table.TableName.Equals("DV_UNITS_OKEI_BRIDGE",
		//        StringComparison.OrdinalIgnoreCase)))
		//    {
		//        return new ColumnInfo(this.TableName, "REFUNITS");
		//    }
		//    return null;
		//}

		private ColumnInfo GetKeyColumnBySourceColumn(ColumnInfo sourceColumn)
		{	
			if (string.IsNullOrEmpty(this.TableName) ||
				this.TableName.Equals(
				sourceColumn.Table.Name, StringComparison.OrdinalIgnoreCase))
			{
				return sourceColumn;
			}

			for (int i = 0; i < tableRelations.Count; i++)
			{
				if (tableRelations[i].ForeignKey.TableName.Equals(
					sourceColumn.TableName, StringComparison.OrdinalIgnoreCase))
				{
					return tableRelations[i].PrimaryKey;
				}
				else
				{
					if (tableRelations[i].PrimaryKey.TableName.Equals(
						sourceColumn.TableName, StringComparison.OrdinalIgnoreCase))
					{
						return tableRelations[i].ForeignKey;
					}
				}				
			}
			return sourceColumn;
		}

		//private ColumnInfo GetKeyColumnBySourceColumn(ColumnInfo sourceColumn)
		//{			
		//    if (this.Name.Equals("Бюджет_МО", StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.ColumnName == "DESIGNATION")
		//            return new ColumnInfo("D_BUDGET_REG9", "REFOKEI");
		//    }
		//    //if (this.Name.Equals("ЖКХ_Бюджет", StringComparison.OrdinalIgnoreCase))
		//    //{
		//    //    if (sourceColumn.ColumnName == "DESIGNATION")
		//    //        return new ColumnInfo("D_GKH_BUDGET", "REFOKEI");
		//    //}
		//    //if (this.Name.Equals("ЖКХ_Льготы", StringComparison.OrdinalIgnoreCase))
		//    //{
		//    //    if (sourceColumn.ColumnName == "DESIGNATION")
		//    //        return new ColumnInfo("D_GKH_BONUS", "REFOKEI");
		//    //}
		//    //if (this.Name.Equals("ЖКХ_Подготовка к зиме",
		//    //    StringComparison.OrdinalIgnoreCase))
		//    //{
		//    //    if (sourceColumn.ColumnName == "DESIGNATION")
		//    //        return new ColumnInfo("D_GKH_WINTER", "REFOKEI");
		//    //}
		//    //if (this.Name.Equals("ЖКХ_Структура ЖКУ", StringComparison.OrdinalIgnoreCase))
		//    //{
		//    //    if (sourceColumn.ColumnName == "DESIGNATION")
		//    //        return new ColumnInfo("D_GKH_GKUSTRUCT", "REFOKEI");
		//    //}
		//    //if (this.Name.Equals("ЖКХ_Тарифы", StringComparison.OrdinalIgnoreCase))
		//    //{
		//    //    if (sourceColumn.ColumnName == "DESIGNATION")
		//    //        return new ColumnInfo("D_GKH_RATE", "REFOKEI");
		//    //}
		//    //if (this.Name.Equals("ЖКХ_Финансы", StringComparison.OrdinalIgnoreCase))
		//    //{
		//    //    if (sourceColumn.ColumnName == "DESIGNATION")
		//    //        return new ColumnInfo("D_GKH_FINANCE", "REFOKEI");
		//    //}
		//    if (this.Name.Equals("ЖКХ_Характеристики ЖФ",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        //if (sourceColumn.ColumnName == "DESIGNATION")
		//        //    return new ColumnInfo("D_GKH_HOMEATTRIBUTES", "REFOKEI");
		//        if (sourceColumn.TableName == "D_ACCOMPLISHMENT_REGION" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_GKH_HOMEATTRIBUTES", "REFACCOMPLISHMENT");
		//        }
		//        if (sourceColumn.TableName == "D_HAVINGSFORM_OKFC" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_GKH_HOMEATTRIBUTES", "REFHAVINGSFORM");
		//        }
		//        if (sourceColumn.TableName == "D_BUILDCOND_REGION" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_GKH_HOMEATTRIBUTES", "REFCONDITION");
		//        }
		//        if (sourceColumn.TableName == "D_GKH_HOMETYPE" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_GKH_HOMEATTRIBUTES", "REFHOMETYPE");
		//        }
		//        if (sourceColumn.TableName == "D_GKH_REASONLEAVE" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_GKH_HOMEATTRIBUTES", "REFREASONLEAVE");
		//        }
		//    }
		//    if (this.Name.Equals("ЖКХ_ХарактКомунХоз",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        //if (sourceColumn.ColumnName == "DESIGNATION")
		//        //    return new ColumnInfo("D_GKH_COMMUNALSERVICES", "REFOKEI");
		//        if (sourceColumn.TableName == "D_GKH_COMMUNALNET" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_GKH_COMMUNALSERVICES", "REFCOMMUNALNET");
		//        }
		//        if (sourceColumn.TableName == "D_BUILDCOND_REGION" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_GKH_COMMUNALSERVICES", "REFCONDITION");
		//        }
		//    }
		//    if (this.Name.Equals("КИФ_АС Бюджет 2005",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "FX_KIF_DIRECTION" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_KIF_BUDGET2005", "REFDIRECTION");
		//        }
		//        if (sourceColumn.TableName == "FX_KIF_CLSASPECT" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_KIF_BUDGET2005", "REFCLSASPECT");
		//        }
		//    }
		//    if (this.Name.Equals("КИФ_Сопоставимый", StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "FV_KIF_DIRECTION_BRIDGE" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("BV_KIF_BRIDGE", "REFDIRECTION");
		//        }
		//        if (sourceColumn.TableName == "FV_KIF_CLSASPECT_BRIDGE" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("BV_KIF_BRIDGE", "REFCLSASPECT");
		//        }
		//    }
		//    if (this.Name.Equals("МФ РФ_Индикаторы БК и КУ",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.ColumnName == "DESIGNATION")
		//            return new ColumnInfo("DV_MFRF_INDICATORBK", "REFOKEI");
		//        if (sourceColumn.TableName == "FX_FX_VERIFICATIONCONDITIONS" &&
		//            sourceColumn.ColumnName == "VALUE")
		//        {
		//            return new ColumnInfo("DV_MFRF_INDICATORBK", "REFVERIFICATIONCONDITIONS");
		//        }
		//    }
		//    if (this.Name.Equals("МФ РФ_ИНП", StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "D_MFRF_MARKS" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_MFRF_INP", "REFMARKS");
		//        }
		//        if (sourceColumn.TableName == "D_BRANCH_REGION" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_MFRF_INP", "REFBRANCH");
		//        }
		//    }
		//    if (this.Name.Equals("МФ РФ_Показатели", StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "DV_UNITS_OKEI_BRIDGE" &&
		//            sourceColumn.ColumnName == "DESIGNATION")
		//        {
		//            return new ColumnInfo("D_MFRF_MARKS", "REFOKEI");
		//        }
		//    }
		//    if (this.Name.Equals("МФ РФ_Распределение доходов",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "D_MFRF_MARKS" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("DV_MFRF_DISTRIBINCOME", "REFMARKS");
		//        }
		//        if (sourceColumn.TableName == "D_INCOME_REGION" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("DV_MFRF_DISTRIBINCOME", "REFINCOMEREGION");
		//        }
		//    }
		//    if (this.Name.Equals("МФ РФ_Сопост показатели",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.ColumnName == "DESIGNATION")
		//            return new ColumnInfo("B_MFRF_MARKS", "REFUNITSOKEI");
		//    }
		//    if (this.Name.Equals("МФ РФ_Сопоставимый",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "DV_UNITS_OKEI_BRIDGE" &&
		//            sourceColumn.ColumnName == "DESIGNATION")
		//        {
		//            return new ColumnInfo("B_MFRF_INDICATORBKBRIDGE", "REFOKEI");
		//        }
		//        if (sourceColumn.TableName == "FV_FX_VERIFICATIONCONDITIONS" &&
		//            sourceColumn.ColumnName == "DESIGNATION")
		//        {
		//            return new ColumnInfo("B_MFRF_INDICATORBKBRIDGE",
		//                "REFVERIFICATIONCONDITIONS");
		//        }
		//    }
		//    if (this.Name.Equals("Показатели_131ФЗ", StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "DV_UNITS_OKEI" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_MARKS_FZ131", "REFOKEI");
		//        }
		//    }
		//    if (this.Name.Equals("Показатели_Паспорт региона",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "DV_UNITS_OKEI" &&
		//            sourceColumn.ColumnName == "SHORT_NAME")
		//        {
		//            return new ColumnInfo("D_MARKS_REG3RGNPASSPORT", "REFOKEI");
		//        }
		//        if (sourceColumn.ColumnName == "DESIGNATION")
		//            return new ColumnInfo("D_MARKS_REG3RGNPASSPORT", "REFOKEI");
		//        if (sourceColumn.TableName == "FX_MARKS_REG3PERIOD" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_MARKS_REG3RGNPASSPORT", "REFPROVIDEPERIOD");
		//        }
		//    }
		//    if (this.Name.Equals("Показатели_Паспорт региона сопост без иерархии",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.ColumnName == "DESIGNATION")
		//            return new ColumnInfo("B_MARKS_REG3RGNPASSPORT", "REFUNITSOKEI");
		//    }
		//    if (this.Name.Equals("Показатели_Паспорт региона сопоставимый",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "DV_UNITS_OKEI" &&
		//            sourceColumn.ColumnName == "SHORT_NAME")
		//        {
		//            return new ColumnInfo("B_MARKS_REG3RGNPASSPORT", "REFUNITSOKEI");
		//        }
		//        if (sourceColumn.ColumnName == "DESIGNATION")
		//            return new ColumnInfo("B_MARKS_REG3RGNPASSPORT", "REFUNITSOKEI");
		//        if (sourceColumn.TableName == "FX_MARKS_REG3PERIOD" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("B_MARKS_REG3RGNPASSPORT", "REFPRVDPERIOD");
		//        }
		//    }
		//    if (this.Name.Equals("Показатели_Фонды",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.ColumnName == "DESIGNATION")
		//        {
		//            return new ColumnInfo("DV_MARKS_FO9FUND", "REFOKEI");
		//        }
		//    }
		//    if (this.Name.Equals("Производство_Производители",
		//        StringComparison.OrdinalIgnoreCase))
		//    {
		//        //if (sourceColumn.ColumnName == "DESIGNATION")
		//        //{
		//        //    return new ColumnInfo("D_PRODUCTION_PRODUCER", "REFOKEI");
		//        //}
		//        if (sourceColumn.TableName == "D_ACTIVITYSTATUS_OKVED" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_PRODUCTION_PRODUCER", "REFACTIVITYSTATUS");
		//        }
		//        if (sourceColumn.TableName == "D_HAVINGSFORM_OKFC" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_PRODUCTION_PRODUCER", "REFHAVINGSFORM");
		//        }
		//        if (sourceColumn.TableName == "D_LEGALFORM_OKOPF" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_PRODUCTION_PRODUCER", "REFLEGALFORM");
		//        }
		//        if (sourceColumn.TableName == "FX_FX_OBJECTSIZE" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_PRODUCTION_PRODUCER", "REFOBJECTSIZE");
		//        }
		//        if (sourceColumn.TableName == "FX_FX_FINRESULT" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_PRODUCTION_PRODUCER", "REFFINRESULT");
		//        }
		//    }
		//    if (this.Name.Equals("Территории_РФ", StringComparison.OrdinalIgnoreCase))
		//    {
		//        if (sourceColumn.TableName == "FX_FX_TERRITORIALPARTITIONTYPE" &&
		//            sourceColumn.ColumnName == "NAME")
		//        {
		//            return new ColumnInfo("D_TERRITORY_RF", "REFTERRITORIALPARTTYPE");
		//        }
		//        if (sourceColumn.TableName == "FX_FX_TERRITORIALPARTITIONTYPE" &&
		//            sourceColumn.ColumnName == "FULLNAME")
		//        {
		//            return new ColumnInfo("D_TERRITORY_RF", "REFTERRITORIALPARTTYPE");
		//        }
		//    }
		//    ColumnInfo keyColumn = GetKeyColumnForUnits(sourceColumn);
		//    if (keyColumn != null) { return keyColumn; }
		//    return sourceColumn;
		//}

		internal AttributeInfo AddMemberAttribute(string name, string description,
			bool isVisible, ColumnInfo sourceColumn, ColumnInfo keyColumn, ref AttributeInfo parentAttr)
		{
			AttributeInfo attr = AddMemberAttribute(name, description, isVisible,
				sourceColumn, keyColumn, AttributeType.Regular);
			attr.ParentAttr = parentAttr;
			
			//По согласованию с Петровым Васей помечаем Member Properties специальным типом.
			attr.Type = AttributeType.Person;

			//это значение переписывается в GetLevels
			attr.AttributeHierarchyEnabled = parentAttr.IsAggregatable;

			attr.AttributeHierarchyVisible = false;
			parentAttr.AttrRelationShips.Add(new AttrRelationshipInfo(attr.SafeName));
			return attr;
		}


		public void AddMemberAttributes(XPathNavigator levelNode, ref AttributeInfo parentAttr)
		{
			if (levelNode.SelectSingleNode(".//MemberPropertys") != null)
			{
				XPathNodeIterator attrNodes =
					levelNode.Select(".//MemberPropertys//MemberProperty");

				if (attrNodes != null && attrNodes.Count > 0)
				{
					ColumnInfo sourceColumn;
					while (attrNodes.MoveNext())
					{
						sourceColumn = new ColumnInfo(attrNodes.Current.SelectSingleNode(
							".//property[@name='SourceColumn']").Value);
						AddMemberAttribute(
							attrNodes.Current.GetAttribute("name", ""),
							attrNodes.Current.GetAttribute("Description", ""),
							attrNodes.Current.SelectSingleNode("@IsVisible").ValueAsBoolean,
							sourceColumn, GetKeyColumnBySourceColumn(sourceColumn),
							ref parentAttr);
					}
				}
			}
		}

		private AttributeType GetAttributeType(string name)
		{
			if (name.IndexOf("Год", StringComparison.OrdinalIgnoreCase) > 0)
			{
				return AttributeType.Years;
			}
			if (name.IndexOf("Полугодие", StringComparison.OrdinalIgnoreCase) > 0)
			{
				return AttributeType.HalfYears;
			}
			if (name.IndexOf("Квартал", StringComparison.OrdinalIgnoreCase) > 0)
			{
				return AttributeType.Quarters;
			}
			if (name.IndexOf("Месяц", StringComparison.OrdinalIgnoreCase) > 0)
			{
				return AttributeType.Months;
			}
			if (name.IndexOf("День", StringComparison.OrdinalIgnoreCase) > 0)
			{
				return AttributeType.Days;
			}
			return AttributeType.Regular;
		}

		private AttributeInfo AddLevelAttribute(string name, ColumnInfo keyColumn,
			ColumnInfo nameColumn, AttributeUsage attributeUsage, OrderBy orderBy,
			string orderByAttributeID, string namingTemplate, string defaultMember)
		{
			AttributeInfo attr = new AttributeInfo(name);
			attr.KeyColumns.Add(keyColumn);
			attr.NameColumn = nameColumn;
			attr.AttributeHierarchyEnabled = true;
			attr.DefaultMember = defaultMember;
			attr.IsAggregatable = true;
			attr.OrderBy = orderBy;
			attr.OrderByAttributeID = orderByAttributeID;
			attr.NamingTemplate = namingTemplate;
			attr.Type = GetAttributeType(name);
			attr.Usage = attributeUsage;
			if (attributeUsage == AttributeUsage.Parent)
				tableRelations.Add(new RelationInfo(keyColumn.TableName, "ID", keyColumn.ColumnName));
			//if (attributeUsage != AttributeUsage.Key)
			this.attributes.Add(attr);
			return attr;
		}

		private OrderBy GetOrderBy(XPathNavigator levelNavigator)
		{
			string strOrderBy = levelNavigator.GetAttribute("Ordering", "");
			if (strOrderBy != string.Empty)
			{
				DSOEnums.Ordering ordering = (DSOEnums.Ordering)Convert.ToInt32(strOrderBy);
				switch (ordering)
				{
					case DSOEnums.Ordering.orderKey:
						return OrderBy.Key;
					case DSOEnums.Ordering.orderName:
						return OrderBy.Name;
					case DSOEnums.Ordering.orderMemberProperty:
						return OrderBy.AttributeKey;
					default:
						return OrderBy.Key;
				}
			}
			else
				return OrderBy.Key;
		}

		private XPathNodeIterator GetLevelNodes(XPathNavigator dimNavigator)
		{
			XPathNodeIterator levelNodes = null;
			if (dimNavigator.SelectSingleNode(".//DatabaseLevels") != null)
				levelNodes = dimNavigator.Select(".//DatabaseLevels//DatabaseLevel");
			else
				levelNodes = dimNavigator.Select(".//CubeLevels//CubeLevel");
			return levelNodes;
		}

		private CalculationInfo CreateCalculation(HierarchyInfo hier,
			AttributeInfo levAttr, string rollUpExpression)
		{
			if (cubeInfo != null)
			{
                //bool parentChild =
                //    OLAPUtils.IsParentChild(databaseInfo.GetSharedDimensionByID(ID));
                bool parentChild = OLAPUtils.IsParentChild(this);
				if (parentChild)
					return new CalculationInfo(
						cubeInfo.Name + hier.Name + levAttr.LevelName,
						String.Format("SCOPE([{0}].[{1}].members);\r\nthis = {2};\r\nEND SCOPE;",
						this.Name, hier.Name, rollUpExpression), "");
				else
					return new CalculationInfo(
						cubeInfo.Name + hier.Name + levAttr.SafeName,
						String.Format("SCOPE([{0}].[{1}].[{2}].members);\r\nthis = {3};\r\nEND SCOPE;",
						this.Name, hier.Name, hier.GetLevelNameByAttribute(levAttr.SafeName),
						rollUpExpression), "");
			}
			else
			{				
				levAttr.CustomRollUpInfo =
					new RollUpInfo("", hier.Name, levAttr.LevelName, rollUpExpression);
				return null;
			}

		}

		private string SafeGetNodeStrValue(XPathNavigator parentNodeNavigator, string childNodeName)
		{
			if (parentNodeNavigator.SelectSingleNode(childNodeName) != null)
				return parentNodeNavigator.SelectSingleNode(childNodeName).Value;
			else
				return String.Empty;
		}

		private ColumnInfo SafeGetNodeColValue(
			XPathNavigator parentNodeNavigator, string childNodeName)
		{
			if (parentNodeNavigator.SelectSingleNode(childNodeName) != null)
				return new ColumnInfo(parentNodeNavigator.SelectSingleNode(childNodeName).Value);
			else
				return null;
		}

		private void CreateKeyAttribute()
		{
			AttributeInfo keyAttribute = new AttributeInfo("ID");
			keyAttribute.Usage = AttributeUsage.Key;
			keyAttribute.Description = "Ключевой аттрибут измерения";
			keyAttribute.AttributeHierarchyVisible = false;
			keyAttribute.OrderBy = OrderBy.Key;

			KeyAttribute = keyAttribute;
		}

		private RootIfValue GetRootIf(XPathNavigator navigator)
		{
			DSOEnums.RootIfValues intRootIf =
				(RootIfValues)(int)Convert.ToInt32(navigator.GetAttribute("RootMemberIf", ""));
			switch (intRootIf)
			{
				case RootIfValues.rootifParentIsBlankOrSelfOrMissing:
				default:
					return RootIfValue.ParentIsBlankSelfOrMissing;
				case RootIfValues.rootifParentIsBlank:
					return RootIfValue.ParentIsBlank;
				case RootIfValues.rootifParentIsSelf:
					return RootIfValue.ParentIsSelf;
				case RootIfValues.rootifParentIsMissing:
					return RootIfValue.ParentIsMissing;				
			}			
		}

		private static string CorrectDefaultMember(string attrName, string defaultMember)
		{
			int i = defaultMember.IndexOf("&");
			if (i >= 0)
			{
				return string.Format("[{0}].{1}", attrName, defaultMember.Substring(i));
			}
			{
				return defaultMember;
			}
		}

		private ColumnInfo GetNameColumn(XPathNavigator levelNavigator)
		{
			ColumnInfo nameColumn =
				SafeGetNodeColValue(levelNavigator, ".//property[@name='MemberNameColumn']");
			/*if (nameColumn != null && nameColumn.ColumnName.StartsWith("Short_Name", StringComparison.OrdinalIgnoreCase))
			{
				nameColumn.ColumnName = "Name";
			}*/
			return nameColumn;
		}

		private void AddLevel(ref HierarchyInfo hier,
			XPathNavigator levelNavigator, string levelName)
		{
			ColumnInfo parentColumn =
				SafeGetNodeColValue(levelNavigator, ".//property[@name='ParentKeyColumn']");
			ColumnInfo keyColumn =
				SafeGetNodeColValue(levelNavigator, ".//property[@name='MemberKeyColumn']");
			ColumnInfo nameColumn = GetNameColumn(levelNavigator);				
			string namingTemplate = levelNavigator.GetAttribute("LevelNamingTemplate", "");
			OrderBy orderBy = GetOrderBy(levelNavigator);
			string orderByAttributeID = levelNavigator.GetAttribute("OrderingMemberProperty", "");
			string defaultMember = navigator.GetAttribute("DefaultMember", "");

			AttributeInfo levAttr;
			if ((parentColumn == null))
			{
				string attrName;
				AttributeUsage usage;
				if (keyColumn.ColumnName.Equals("ID", StringComparison.OrdinalIgnoreCase))
				{
					attrName = "ID";
					usage = AttributeUsage.Key;
				}
				else
				{
					attrName = "уровень_" + levelName;
					usage = AttributeUsage.Regular;
				}
				if (!string.IsNullOrEmpty(defaultMember))
				{
					defaultMember = CorrectDefaultMember(attrName, defaultMember);
				}
				levAttr = AddLevelAttribute(attrName, keyColumn, nameColumn, usage, orderBy,
					orderByAttributeID, namingTemplate, defaultMember);

				LevelInfo lev = new LevelInfo(levelName);
				lev.SourceAttributeID = levAttr.SafeName;
				lev.Description = levelNavigator.GetAttribute("Description", "");
				hier.Levels.Add(lev);				
			}
			else
			{
				//TODO Проверить в какой аттрибут на самом деле должен записываться defaultMember
				hier.ParentChild = true;
				levAttr = AddLevelAttribute(
					OLAPUtils2000.GetHierarchyName(this.Name), parentColumn, parentColumn,
					AttributeUsage.Parent, OrderBy.Key, string.Empty,
					namingTemplate, defaultMember);
				levAttr.IsAggregatable = hier.AllLevel;
				levAttr.RootMemberIf = GetRootIf(levelNavigator);

				KeyAttribute = AddLevelAttribute("ID", keyColumn,
					nameColumn, AttributeUsage.Key, orderBy, orderByAttributeID,
					string.Empty, defaultMember);
				KeyAttribute.AttrRelationShips.Add(new AttrRelationshipInfo(levAttr.SafeName));
				levAttr = KeyAttribute; //чтобы навесить CustomRollUp                
			}

			string rollUpExpression = SafeGetNodeStrValue(
				levelNavigator, ".//property[@name='CustomRollUpExpression']");
			if (rollUpExpression.Length > 0)
			{
				CalculationInfo calcInfo = CreateCalculation(hier, levAttr, rollUpExpression);
				if (calcInfo != null)
				{
					//Т.к. считаться сначала должен Custom RollUp нижнего уровня,
					//затем - более высого, затем - еще более высокого,
					//а считываем мы в обратном порядке, то заталкиваем CustomRollup в стэк.
					//if (this.cubeInfo != null && (
					//    this.cubeInfo.Name.StartsWith("СТАТ_", StringComparison.OrdinalIgnoreCase) ||
					//    this.cubeInfo.Name.StartsWith("РЕГИОН_Паспорт региона", StringComparison.OrdinalIgnoreCase)))
					//{
					//    ReplaceStatCalculation(calcInfo);
					//}
					//else
					//{
					//    this.calculations.Push(calcInfo);
					//}
					this.calculations.Push(calcInfo);
				}
			}

			AddMemberAttributes(levelNavigator, ref levAttr);			
		}

		private void ReplaceStatCalculation(CalculationInfo calcInfo)
		{
			if (calcInfo.Expression.StartsWith(
				"SCOPE([Период__Год Квартал Месяц].[Период__Год Квартал Месяц].members);",
				StringComparison.OrdinalIgnoreCase))
			{
				List<CalculationInfo> newCalculation = ReadStatCustomCalculations();
				for (int i = 0; i < newCalculation.Count; i++)
				{
					this.Calculations.Push(newCalculation[i]);
				}
			}			
		}

		private List<CalculationInfo> ReadStatCustomCalculations()
		{
			List<CalculationInfo> newCalculations = new List<CalculationInfo>();
			CalculationInfo calculationInfo = new CalculationInfo(
			"Период значение последний не пустой",
			@"
			CREATE CELL CALCULATION CURRENTCUBE.[Период значение последний не пустой]
FOR '(
(
DESCENDANTS(
[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов],
4,
BEFORE),
Except(
[Показатели__Тип значения].[Показатели__Тип значения].members,
{[Показатели__Тип значения].[Показатели__Тип значения].[Показатель].[Факт за период],
[Показатели__Тип значения].[Показатели__Тип значения].[Показатель].[Процент исполнения за период],
[Показатели__Тип значения].[Показатели__Тип значения].[Отклонение за период]})
)
)'
AS
CalculationPassValue(
IIF (not IsEmpty([Период__Год Квартал Месяц].[Период__Год Квартал Месяц].CurrentMember.DataMember),
    ([Период__Год Квартал Месяц].[Период__Год Квартал Месяц].CurrentMember.DataMember, Measures.CurrentMember),
    Tail(
        Filter([Период__Год Квартал Месяц].[Период__Год Квартал Месяц].CurrentMember.Children,
        not (IsEmpty(Measures.CurrentMember) or Measures.CurrentMember = 0)),
        1).Item(0)
    ), 0),
DESCRIPTION = '';", string.Empty);
			newCalculations.Add(calculationInfo);

			calculationInfo = new CalculationInfo(
			"Период значение сумма",
			@"CREATE CELL CALCULATION CURRENTCUBE.[Период значение сумма]
FOR '(
(
DESCENDANTS(
[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов],
4,
BEFORE),
{[Показатели__Тип значения].[Показатели__Тип значения].[Показатель].[Факт за период],
[Показатели__Тип значения].[Показатели__Тип значения].[Показатель].[Процент исполнения за период],
[Показатели__Тип значения].[Показатели__Тип значения].[Отклонение за период]})
)'
AS
CalculationPassValue(
IIF (not IsEmpty([Период__Год Квартал Месяц].[Период__Год Квартал Месяц].CurrentMember.DataMember),
    ([Период__Год Квартал Месяц].[Период__Год Квартал Месяц].CurrentMember.DataMember, Measures.CurrentMember),
    [Период__Год Квартал Месяц].[Период__Год Квартал Месяц].CurrentMember), 0),
DESCRIPTION = '';", string.Empty);
			newCalculations.Add(calculationInfo);
			return newCalculations;
		}

		private AttributeInfo GetAttributeByName(string attrName)
		{
			for (int i = 0; i < attributes.Count; i++)
				if (attributes[i].SafeName == attrName)
					return attributes[i];
			return null;
		}		

		private void GetLevels(ref HierarchyInfo hier)
		{
			XPathNodeIterator levelNodes = GetLevelNodes(navigator);
			if (levelNodes != null && levelNodes.Count > 0)
			{
				hier.Levels = new List<LevelInfo>();				
				while (levelNodes.MoveNext())
				{
					string levelName = levelNodes.Current.GetAttribute("name", "");
					if (levelName == "(All)")
					{
						hier.AllMemberName = levelNodes.Current.
							SelectSingleNode(".//property[@name='MemberKeyColumn']").Value;
						this.allMemberName = hier.AllMemberName;
					}
					else
					{
						AddLevel(ref hier, levelNodes.Current, levelName);
					}
				}
				if (hier.Levels.Count > 0)
				{
					AttributeInfo topAttribute = GetAttributeByName(hier.Levels[0].SourceAttributeID);
					topAttribute.IsAggregatable = hier.AllLevel;
					for (int i = 0; i < topAttribute.AttrRelationShips.Count; i++)
					{
						GetAttributeByName(topAttribute.AttrRelationShips[i].AttributeID).
							AttributeHierarchyEnabled = topAttribute.IsAggregatable;
					}
				}				
			}
		}

		public Stack<CalculationInfo> Calculations
		{
			get { return calculations; }
		}
	}

	public class DimensionMDXName2000
	{
		private string bracketedName;
		private string bracketedName2;
		private string newName;

		public DimensionMDXName2000(string _originalName, string _newName)
		{
			string[] delimiter = { "." };
			string[] nameParts = _originalName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
			bracketedName = string.Format("[{0}].[{1}]", nameParts[0], nameParts[1]);
			bracketedName2 = string.Format("[{0}.{1}]", nameParts[0], nameParts[1]);
			//safeName = String.Format("[{0}]", _safeName);
			newName = string.Format("[{0}].[{1}]", _newName, _newName);
		}

		public string BracketedName
		{
			get { return bracketedName; }
		}

		public string BracketedName2
		{
			get { return bracketedName2; }
		}

		public string NewName
		{
			get { return newName; }
		}
	}

	class CubeDimensionInfo2000 : CubeDimensionInfoBase
	{
		private CubeDimensionInfo2000(XPathNavigator _dimensionNavigator, CubeInfoBase _cubeInfo)
			: base(_dimensionNavigator, _cubeInfo)
		{

		}

		public CubeDimensionInfo2000(CubeInfoBase _cubeInfo, DimensionInfo2000 _dimension)
			: base(null, _cubeInfo)
		{
			identificationInfo = new IdentificationInfo(_dimension.Name, _dimension.ID);
			dimensionID = _dimension.IdentificationInfo.GUID;
		}

		protected override void ReadAttributes()
		{
		}

		protected override void ReadDescription()
		{
		}

		protected override void ReadDimension()
		{
			base.ReadDimension();
		}

		protected override void ReadHierarchies()
		{
		}

		protected override void ReadIdentificationInfo()
		{
		}
	}
}