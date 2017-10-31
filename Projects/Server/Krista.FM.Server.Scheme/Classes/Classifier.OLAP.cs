using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Krista.FM.Server.Scheme.Services.OLAP;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    partial class Classifier
    {
        private string GetOlapDBName(params string[] parameters)
        {
            string result = "\"DV\"";
            foreach (string item in parameters)
                result += String.Format(".\"{0}\"", item.ToUpper());
            return result;
        }

        protected DSO.SubClassTypes GetOlapSubClassTypes()
        {
            switch (this.Levels.HierarchyType)
            {
                case HierarchyType.Regular:
                    return DSO.SubClassTypes.sbclsRegular;
                case HierarchyType.ParentChild:
                    return DSO.SubClassTypes.sbclsParentChild;
                default:
                    throw new Exception("Неизвестный тип иерархии.");
            }
        }

        private short GetOlapDataType(DataAttributeTypes type)
        {
            switch (type)
            {
                case DataAttributeTypes.dtBoolean:
                case DataAttributeTypes.dtInteger:
                    return 131;
                case DataAttributeTypes.dtDouble:
                    return 5;
                case DataAttributeTypes.dtChar:
                case DataAttributeTypes.dtString:
                    return 129;
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    return 133;
                default:
                    throw new Exception("Неизвестрый тип данных.");
            }
        }

        private void SetOlapAllLevel(DSO.Dimension dsoDimension, DimensionLevel level)
        {
            DSO.Level dsoLevel = (DSO.Level)dsoDimension.Levels.AddNew("(All)", DSO.SubClassTypes.sbclsRegular);
            dsoLevel.LevelType = DSO.LevelTypes.levAll;
            dsoLevel.MemberKeyColumn = level.Name;
        }

        private void SetOlapMemberProperty(DSO.Level dsoLevel, string name, string sourceColumn, short type, short size)
        {
            DSO.MemberProperty dsoMemberProperty = (DSO.MemberProperty)dsoLevel.MemberProperties.AddNew(name, DSO.SubClassTypes.sbclsRegular);
            dsoMemberProperty.SourceColumn = GetOlapDBName(FullDBName, sourceColumn);
            dsoMemberProperty.set_ColumnType(ref type);
            dsoMemberProperty.set_ColumnSize(ref size);
        }

        private void SetOlapMemberProperties(DSO.Level dsoLevel)
        {
            //DSO.MemberProperty dsoMemberProperty;

            // Добавлять все поля кроме наименования как member properties по порядку, причем PKID делать последним
            foreach (DataAttribute attr in Attributes.Values)
            {
                if (attr.Name == DataAttribute.IDColumnName ||
                    attr.Name == DataAttribute.SourceIDColumnName ||
                    attr.Name == DataAttribute.PumpIDColumnName ||
                    attr.Name == DataAttribute.TaskIDColumnName ||
                    attr.Name == "Name" ||
                    attr.Name == DataAttribute.ParentIDColumnName ||
                    attr.Class == DataAttributeClassTypes.Reference)
                    continue;

                if (attr.Kind == DataAttributeKindTypes.Sys || attr.Kind == DataAttributeKindTypes.Serviced)
                    continue;

                string memberCaption = attr.Caption.Replace('.', '_');
                memberCaption = attr.Caption.Replace(',', '_');
                memberCaption = attr.Caption.Replace(';', '_');
                memberCaption = attr.Caption.Replace(':', '_');
                memberCaption = attr.Caption.Replace('(', '_');
                memberCaption = attr.Caption.Replace(')', '_');
                memberCaption = attr.Caption.Replace('-', '_');
                memberCaption = attr.Caption.Replace('+', '_');
                memberCaption = attr.Caption.Replace('=', '_');
                memberCaption = attr.Caption.Replace('?', '_');
                memberCaption = attr.Caption.Replace('"', '_');
                SetOlapMemberProperty(dsoLevel, memberCaption, attr.Name, GetOlapDataType(attr.Type), 10);
            }

            SetOlapMemberProperty(dsoLevel, "PKID", "ID", 131, 10);
        }

        private string GetOlapParametersJoinString()
        {
            string result = String.Format("CASE {0} WHEN 0 THEN {1} || ' ' || {2} ",
                GetOlapDBName("DataSources", "KindsOfParams"),
                GetOlapDBName("DataSources", "Name"),
                GetOlapDBName("DataSources", "Year"));
            result += String.Format(" WHEN 1 THEN cast({0} as varchar(4)) ",
                GetOlapDBName("DataSources", "Year"));
            result += String.Format(" WHEN 2 THEN {0} || ' ' || {1} ",
                GetOlapDBName("DataSources", "Year"),
                GetOlapDBName("DataSources", "Month"));
            result += String.Format(" WHEN 3 THEN {0} || ' ' || {1} || ' ' || {2} ",
                GetOlapDBName("DataSources", "Year"),
                GetOlapDBName("DataSources", "Month"),
                GetOlapDBName("DataSources", "Variant"));
            result += String.Format(" WHEN 4 THEN {0} || ' ' || {1} ",
                GetOlapDBName("DataSources", "Year"),
                GetOlapDBName("DataSources", "Variant"));
            result += String.Format(" WHEN 5 THEN {0} || ' ' || {1} END ",
                GetOlapDBName("DataSources", "Year"),
                GetOlapDBName("DataSources", "Quarter"));
            result += String.Format(" WHEN 6 THEN {0} || ' ' || {1} END ",
                GetOlapDBName("DataSources", "Year"),
                GetOlapDBName("DataSources", "Territory"));
            return result;
        }

        private void SetOlapSourceRegularLevel(DSO.Dimension dsoDimension, DimensionLevel level)
        {
            DSO.Level dsoLevel = (DSO.Level)dsoDimension.Levels.AddNew("Источник", DSO.SubClassTypes.sbclsRegular);
            dsoLevel.MemberKeyColumn = GetOlapDBName(FullDBName, "SourceID");
            dsoLevel.MemberNameColumn = GetOlapDBName(FullDBName, "DataSourceName");
            dsoLevel.EstimatedSize = 1;

            // Добавляем member properties с именем «ID источника» по полю "SOURCEID".
            DSO.MemberProperty dsoMemberProperty = (DSO.MemberProperty)dsoLevel.MemberProperties.AddNew("ID источника", DSO.SubClassTypes.sbclsRegular);
            dsoMemberProperty.SourceColumn = GetOlapDBName(FullDBName, "SourceID");
            short columnType = 131;
            dsoMemberProperty.set_ColumnType(ref columnType);
            short columnSize = 10;
            dsoMemberProperty.set_ColumnSize(ref columnSize);
        }

        private void SetOlapRegularLevel(DSO.Dimension dsoDimension, DimensionLevel level)
        {
            DSO.Level dsoLevel = (DSO.Level)dsoDimension.Levels.AddNew(level.Name, DSO.SubClassTypes.sbclsRegular);
            dsoLevel.MemberKeyColumn = GetOlapDBName(FullDBName, level.MemberKey.Name);
            dsoLevel.MemberNameColumn = GetOlapDBName(FullDBName, level.MemberName.Name);
            if (level.MemberKey.Name == "CodeStr")
            {
                dsoLevel.ColumnType = 200;
                dsoLevel.ColumnSize = 20;
            }
            dsoLevel.EstimatedSize = 2;

            SetOlapMemberProperties(dsoLevel);
        }

        private void SetOlapRegularHierarchy(DSO.Dimension dsoDimension)
        {
            int levelNo = 0;
            foreach (DimensionLevel level in this.Levels.Values)
            {
                if (level.LevelType == LevelTypes.All)
                    SetOlapAllLevel(dsoDimension, level);
                else
                {
                    if ((levelNo == 0 || levelNo == 1) && ClassType == ClassTypes.clsDataClassifier && IsDivided)
                    {
                        SetOlapSourceRegularLevel(dsoDimension, level);
                        levelNo++;
                    }
                    SetOlapRegularLevel(dsoDimension, level);
                }
                levelNo++;
            }
        }

        private void SetOlapParentChildLevel(DSO.Dimension dsoDimension, DimensionLevel level)
        {
            DSO.Level dsoLevel = (DSO.Level)dsoDimension.Levels.AddNew(level.Name, DSO.SubClassTypes.sbclsParentChild);
            dsoLevel.MemberKeyColumn = GetOlapDBName(FullDBName, level.MemberKey.Name);
            dsoLevel.MemberNameColumn = GetOlapDBName(FullDBName, level.MemberName.Name);

            if (ClassType == ClassTypes.clsDataClassifier)
            {
                string sourceLevelName = String.Empty;
                if (IsDivided)
                {
                    dsoLevel.ParentKeyColumn = GetOlapDBName(FullDBName, DataAttribute.CubeParentIDColumnName);
                    sourceLevelName = "Источник; ";
                }
                else
                    dsoLevel.ParentKeyColumn = GetOlapDBName(FullDBName, DataAttribute.ParentIDColumnName);

                if (level.LevelNamingTemplate == null || level.LevelNamingTemplate == String.Empty)
                    dsoLevel.LevelNamingTemplate = sourceLevelName + "Уровень *";
                else
                    dsoLevel.LevelNamingTemplate = sourceLevelName + level.LevelNamingTemplate;
            }
            else
            {
                dsoLevel.ParentKeyColumn = GetOlapDBName(FullDBName, level.ParentKey.Name);
                dsoLevel.LevelNamingTemplate = level.LevelNamingTemplate;
            }

            dsoLevel.EstimatedSize = 1;

            SetOlapMemberProperties(dsoLevel);
        }

        private void SetOlapRegularParentChild(DSO.Dimension dsoDimension)
        {
            dsoDimension.MembersWithData = DSO.MembersWithDataValues.dataforNonLeafMembersVisible;
            dsoDimension.DataMemberCaptionTemplate = "(* ДАННЫЕ)";

            foreach (DimensionLevel level in this.Levels.Values)
            {
                if (level.LevelType == LevelTypes.All)
                    SetOlapAllLevel(dsoDimension, level);
                else
                    SetOlapParentChildLevel(dsoDimension, level);
            }
        }

        private DSO.DataSource GetOlapDataSource()
        {
            DSO.Database db = (DSO.Database)((SchemeMDStore)SchemeClass.Instance.SchemeMDStore).OlapDatabase.DatabaseObject;
            if (!db.DataSources.Find("dv"))
                throw new Exception("Нет источника dv.");
            else
                return (DSO.DataSource)db.DataSources.Item("dv");

        }

        public void SetOlapDimensionCustomProperties(DSO.Dimension dsoDimension)
        {
            dsoDimension.CustomProperties.Add(this.FullName, "FullName", VBA.VbVarType.vbString);
            dsoDimension.CustomProperties.Add(this.ObjectKey, "ObjectKey", VBA.VbVarType.vbString);
            //dsoDimension.CustomProperties.Add(this.Caption, "Caption", VBA.VbVarType.vbString);
            //dsoDimension.CustomProperties.Add(this.Description, "Description", VBA.VbVarType.vbString);
        }

        public void SetOlapProperties(DSO.Dimension dsoDimension)
        {
            dsoDimension.DataSource = GetOlapDataSource();
            dsoDimension.Description = this.Description;
            dsoDimension.FromClause = GetOlapDBName(FullDBName);
            if (ClassType == ClassTypes.clsDataClassifier || ClassType == ClassTypes.clsBridgeClassifier)
                dsoDimension.AllowSiblingsWithSameName = false;

            SetOlapDimensionCustomProperties(dsoDimension);
        }

        public void SetOlapHierarchy(DSO.Dimension dsoDimension)
        {
            if (GetOlapSubClassTypes() == DSO.SubClassTypes.sbclsRegular)
                SetOlapRegularHierarchy(dsoDimension);
            else if (GetOlapSubClassTypes() == DSO.SubClassTypes.sbclsParentChild)
                SetOlapRegularParentChild(dsoDimension);
        }

        protected virtual void InternalCreateOlapObject(DSO.Database db)
        {
            DSO.Dimension dim;
            try
            {
                if (!db.Dimensions.Find(GetOlapCaption()))
                    dim = (DSO.Dimension)db.Dimensions.AddNew(GetOlapCaption(), GetOlapSubClassTypes());
                else
                {
                    db.Dimensions.Remove(GetOlapCaption());
                    dim = (DSO.Dimension)db.Dimensions.AddNew(GetOlapCaption(), GetOlapSubClassTypes());
                }

                SetOlapProperties(dim);
                SetOlapHierarchy(dim);

                dim.Update();
            }
            finally
            {
                dim = null;
            }
        }

        public virtual void CreateOlapObject()
        {
            DSO.Database db = null;
            if (!SchemeMDStore.Instance.IsAS2005())
            {
                DSO.Server server = (DSO.Server)((SchemeMDStore)SchemeClass.Instance.SchemeMDStore).OlapDatabase.ServerObject;
                db = (DSO.Database)server.MDStores.Item("DV");
                try
                {
                    InternalCreateOlapObject(db);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    throw new Exception(e.Message, e);
                }
                finally
                {
                    db = null;
                }
            }
        }

        protected virtual void CreateOlapObjects()
        {
        }
    }
}
