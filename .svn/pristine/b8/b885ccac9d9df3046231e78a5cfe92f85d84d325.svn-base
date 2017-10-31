using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using Infragistics.Win.UltraWinTree;

namespace Krista.FM.Client.OLAPAdmin
{
    /// <summary>
    /// Класс для работы со скриптом
    /// </summary>
    public class PatchManager
    {
        XmlDocument mainDoc = null;

        private Dictionary<string, BaseAMOObject> dictionary = new Dictionary<string, BaseAMOObject>();

        private XmlNamespaceManager xmlns = null;

        /// <summary>
        /// Активный документ со скриптом
        /// </summary>
        /// <param name="mainDoc"></param>
        /// <param name="xmlns">Пространство имен</param>
        public PatchManager(XmlDocument mainDoc, XmlNamespaceManager xmlns)
        {
            this.mainDoc = mainDoc;
            this.xmlns = xmlns;
        }

        public Dictionary<string, BaseAMOObject> Dictionary
        {
            get { return dictionary; }
            set { dictionary = value; }
        }

        public Dictionary<string, BaseAMOObject> Analysis()
        {
            dictionary.Clear();

            //коллекция альтеров
            XmlNodeList alterList = mainDoc.SelectNodes("as:Batch//as:Alter", xmlns);
            FindObjects(alterList);

            // коллекция объектов для создания
            XmlNodeList createList = mainDoc.SelectNodes("as:Batch//as:Create", xmlns);
            FindObjects(createList);

            // коллекция объектов для удаления
            XmlNodeList deleteList = mainDoc.SelectNodes("as:Batch//as:Delete", xmlns);
            FindDelObjects(deleteList);

            return dictionary;
        }

        /// <summary>
        /// Поиск объектов для удаления
        /// </summary>
        /// <param name="list"></param>
        private void FindDelObjects(XmlNodeList list)
        {
            foreach (XmlNode node in list)
            {
                if (node.SelectSingleNode("as:Object/as:CubeID", xmlns) != null)
                {
                    InitializeDelCube(node);
                }
                else if (node.SelectSingleNode("as:Object/as:DimensionID", xmlns) != null)
                {
                    InitializeDelDimension(node);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alterList"></param>
        private void FindObjects(XmlNodeList alterList)
        {
            foreach (XmlNode node in alterList)
                if (node.SelectSingleNode("as:ObjectDefinition/as:Database", xmlns) != null)
                    InitializeDatabase(node);
            foreach (XmlNode node in alterList)  
                if (node.SelectSingleNode("as:ObjectDefinition/as:DataSourceView", xmlns) != null)
                    InitializeDSV(node);
            foreach (XmlNode node in alterList)
                if (node.SelectSingleNode("as:ObjectDefinition/as:Dimension", xmlns) != null)
                    InitializeDimension(node);
            foreach (XmlNode node in alterList)
                if (node.SelectSingleNode("as:ObjectDefinition/as:Cube", xmlns) != null)
                    InitializeCube(node);
        }

        private void InitializeDatabase(XmlNode node)
        {
            AMODatabase database = new AMODatabase();
            if (node.SelectSingleNode("as:ObjectDefinition/as:Database/as:ID", xmlns) != null)
            {
                database.ID = node.SelectSingleNode("as:ObjectDefinition/as:Database/as:ID", xmlns).InnerText;
            }
            if (node.SelectSingleNode("as:ObjectDefinition/as:Database/as:Name", xmlns) != null)
            {
                database.Name = node.SelectSingleNode("as:ObjectDefinition/as:Database/as:Name", xmlns).InnerText;
            }
            XmlNodeList annotationList = node.SelectNodes("as:ObjectDefinition/as:Database/as:Annotations/as:Annotation", xmlns);

            foreach (XmlNode xmlNode in annotationList)
            {
                if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "Версия")
                {
                        database.Version = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                }
            }

            dictionary.Add(database.Name, database);
        }

        private void InitializeDSV(XmlNode node)
        {
            AMODSV dsv = new AMODSV();
            if (node.SelectSingleNode("as:Object/as:DatabaseID", xmlns) != null)
            {
                dsv.Database = node.SelectSingleNode("as:Object/as:DatabaseID", xmlns).InnerText;
                dsv.ID = node.SelectSingleNode("as:Object/as:DataSourceViewID", xmlns).InnerText;
            }
            dsv.Name = node.SelectSingleNode("as:ObjectDefinition/as:DataSourceView/as:Name", xmlns).InnerText;
            dsv.Method = node.Name;

            dictionary.Add(dsv.Name, dsv);
        }

        /// <summary>
        /// Инициализация измерения для удаления
        /// </summary>
        /// <param name="node"></param>
        private void InitializeDelDimension(XmlNode node)
        {
            AMODimension dimension = new AMODimension();
            dimension.Database = node.SelectSingleNode("as:Object/as:DatabaseID", xmlns).InnerText;
            dimension.ID = node.SelectSingleNode("as:Object/as:DimensionID", xmlns).InnerText;
            dimension.Method = node.Name;

            dictionary.Add(string.IsNullOrEmpty(dimension.Name) ? dimension.ID : dimension.Name, dimension);
        }

        private void InitializeDelCube(XmlNode node)
        {
            AMOCube cube = new AMOCube();
            cube.Database = node.SelectSingleNode("as:Object/as:DatabaseID", xmlns).InnerText;
            cube.ID = node.SelectSingleNode("as:Object/as:CubeID", xmlns).InnerText;
            cube.Method = node.Name;
            dictionary.Add(string.IsNullOrEmpty(cube.Name) ? cube.ID : cube.Name, cube);
        }

        private void InitializeCube(XmlNode node)
        {
            try
            {
                AMOCube cube = new AMOCube();
                if (node.SelectSingleNode("as:Object/as:DatabaseID", xmlns) != null)
                {
                    cube.Database = node.SelectSingleNode("as:Object/as:DatabaseID", xmlns).InnerText;
                    cube.ID = node.SelectSingleNode("as:Object/as:CubeID", xmlns).InnerText;
                }
                if (node.SelectSingleNode("as:ParentObject//as:DatabaseID", xmlns) != null)
                {
                    cube.Database = node.SelectSingleNode("as:ParentObject/as:DatabaseID", xmlns).InnerText;
                }
                cube.Name = node.SelectSingleNode("as:ObjectDefinition/as:Cube/as:Name", xmlns).InnerText;
                if (node.SelectSingleNode("as:ObjectDefinition/as:Cube/as:Description", xmlns) != null)
                    cube.Description = node.SelectSingleNode("as:ObjectDefinition/as:Cube/as:Description", xmlns).InnerText;
                cube.Method = node.Name;

                XmlNodeList annotationList = node.SelectNodes("as:ObjectDefinition/as:Cube/as:Annotations/as:Annotation", xmlns);
                foreach (XmlNode xmlNode in annotationList)
                {
                    if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "dsvNameAnnotation")
                    {
                        cube.DsvName = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                    }
                    if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "FullName")
                    {
                        cube.FullName = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                    }
                    if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "joinAnnotation")
                    {
                        cube.JoinClause = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                    }
                }

                dictionary.Add(cube.Name, cube);
            }
            catch
            {
                
            }
        }

        private void InitializeDimension(XmlNode node)
        {
            AMODimension dimension = new AMODimension();
            if (node.SelectSingleNode("as:Object/as:DatabaseID", xmlns) != null)
            {
                dimension.Database = node.SelectSingleNode("as:Object/as:DatabaseID", xmlns).InnerText;
                dimension.ID = node.SelectSingleNode("as:Object/as:DimensionID", xmlns).InnerText;
            }
            if (node.SelectSingleNode("as:ParentObject//as:DatabaseID", xmlns) != null)
            {
                dimension.Database = node.SelectSingleNode("as:ParentObject/as:DatabaseID", xmlns).InnerText;
            }
            dimension.Name = node.SelectSingleNode("as:ObjectDefinition/as:Dimension/as:Name", xmlns).InnerText;
            if (node.SelectSingleNode("as:ObjectDefinition/as:Dimension/as:Description", xmlns) != null)
                dimension.Description = node.SelectSingleNode("as:ObjectDefinition/as:Dimension/as:Description", xmlns).InnerText;
            dimension.Method = node.Name;

            XmlNodeList annotationList = node.SelectNodes("as:ObjectDefinition/as:Dimension/as:Annotations/as:Annotation", xmlns);
            foreach (XmlNode xmlNode in annotationList)
            {
                if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "dsvNameAnnotation")
                {
                    dimension.DsvName = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                }
                if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "dsvQueryDefinition")
                {
                    dimension.QueryDefinition = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                }
                if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "FullName")
                {
                    dimension.FullName = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                }
                if (xmlNode.SelectSingleNode("as:Name", xmlns).InnerText == "joinAnnotation")
                {
                    dimension.JoinClause = xmlNode.SelectSingleNode("as:Value", xmlns).InnerText;
                }
            }

            dictionary.Add(dimension.Name, dimension);
        }

        public BaseAMOObject FindByName(string name)
        {
            if (dictionary.ContainsKey(name))
                return dictionary[name];

            return null;
        }
    }

    /// <summary>
    /// Базовый узел дерева
    /// </summary>
    public class BaseTreeNode : UltraTreeNode
    {
        /// <summary>
        /// Свойства объекта
        /// </summary>
        private SMO smoObject;

        public BaseTreeNode(SMO smoObject)
        {
            this.smoObject = smoObject;
        }

        /// <summary>
        /// Свойства объекта
        /// </summary>
        public SMO SmoObject
        {
            get { return smoObject; }
            set { smoObject = value; }
        }
    }

    /// <summary>
    /// Базовый класс для объектов в скрипте (объект дерева)
    /// </summary>
    public class MajorObjForScript<T> : BaseTreeNode
    {
        /// <summary>
        /// Управляющий объект
        /// </summary>
        private T controlOblect;

        public MajorObjForScript(T controlObject, SMO smoObject)
            : base (smoObject)
        {
            this.controlOblect = controlObject;
        }

        public T ControlOblect
        {
            get { return controlOblect; }
            set { controlOblect = value; }
        }
    }

    
    public class CubeForScript : MajorObjForScript<AMOCube>
    {
        public CubeForScript(AMOCube cube, SMOCube smoObject)
            : base(cube, smoObject)
        {
            this.Text = string.IsNullOrEmpty(cube.Name) ? cube.ID : cube.Name;
        }
    }

    public class DimensionObjectForScript : MajorObjForScript<AMODimension>
    {
        public DimensionObjectForScript(AMODimension dimension, SMODimension smoObject)
            : base(dimension, smoObject)
        {
            this.Text = string.IsNullOrEmpty(dimension.Name) ? dimension.ID : dimension.Name;
        }
    }

    public class DSVObjectForScript : MajorObjForScript<AMODSV>
    {
        public DSVObjectForScript(AMODSV dsv, SMODSV smoObject)
            : base(dsv, smoObject)
        {
            this.Text = string.IsNullOrEmpty(dsv.Name) ? dsv.ID : dsv.Name;
        }
    }

    public class DatabaseObjectForScript : MajorObjForScript<AMODatabase>
    {
        public DatabaseObjectForScript(AMODatabase database, SMODatabase smoObject)
            : base(database, smoObject)
        {
            this.Text = string.IsNullOrEmpty(database.Name) ? database.ID : database.Name;
        }
    }
}
