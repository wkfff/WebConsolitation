using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.Commands
{
    /// <summary>
    /// Команда генерации C# кода доменных объектов.
    /// </summary>
    public class GenerateCodeCommand
    {
        private List<string> generatedEntities = new List<string>();

        /// <summary>
        /// Генерация C# кода доменных объектов.
        /// </summary>
        /// <param name="entities">Список сущностей для генерации.</param>
        public void Execute(IList<IEntity> entities)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "DomainCode");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var entity in entities)
            {
                GenerateEntityCode(path, entity);
            }
        }

        private void GenerateEntityCode(string path, IEntity entity)
        {
            string entityName = GetClassName(entity);
            if (generatedEntities.Contains(entityName))
            {
                return;
            }

            generatedEntities.Add(entityName);

            var packagePath = CreatePackageDirectory(path, entity.ParentPackage);
            var filePath = Path.Combine(packagePath, entityName + ".cs");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;")
                .AppendLine()
                .AppendLine("namespace Krista.FM.Domain")
                .AppendLine("{");

            sb.AppendFormat("\tpublic class {0} : {1}", entityName, GetBaseEntityName(entity))
                .AppendLine()
                .AppendLine("\t{")
                .AppendFormat("\t\tpublic static readonly string Key = \"{0}\";", entity.ObjectKey)
                .AppendLine()
                .AppendLine();

            foreach (var attribute in entity.Attributes.Values)
            {
                if (attribute.Name == "ID")
                    continue;

                sb.AppendFormat("\t\tpublic virtual {0} {1} {{ get; set; }}", 
                        GetFieldDataType(attribute), attribute.Name)
                    .AppendLine();

                if (attribute.Class == DataAttributeClassTypes.Reference)
                {
                    GenerateEntityCode(path,
                        ((IEntityAssociation)attribute.OwnerObject).RoleBridge);
                }
            }

            sb.AppendLine("\t}");

            sb.AppendLine("}");

            using (var stream = File.Create(filePath))
            {
                StreamWriter tw = new StreamWriter(stream);
                tw.Write(sb.ToString());
                tw.Close();
            } 
        }

        private static string CreatePackageDirectory(string basePath, IPackage package)
        {
            if (package.ParentPackage != null)
            {
                var parentDirectory = CreatePackageDirectory(basePath, package.ParentPackage);
                var path = Path.Combine(parentDirectory, package.Name);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            return basePath;
        }

        private static string GetClassName(IEntity entity)
        {
            var namePaths = entity.FullDBName.Split(new [] {'_'});
            return String.Format("{0}_{1}_{2}", 
                                 namePaths[0].ToUpper(), namePaths[1], namePaths[2]);
        }

        private static string GetBaseEntityName(IEntity entity)
        {
            string baseEntityName;
            switch (entity.ClassType)
            {
                case ClassTypes.clsFactData:
                    baseEntityName = "FactTable";
                    break;
                case ClassTypes.clsBridgeClassifier:
                case ClassTypes.clsDataClassifier:
                case ClassTypes.clsFixedClassifier:
                    baseEntityName = "ClassifierTable";
                    break;
                default:
                    baseEntityName = "DomainObject";
                    break;
            }
            return baseEntityName;
        }

        private static string GetFieldDataType(IDataAttribute attribute)
        {
            if (attribute.Class == DataAttributeClassTypes.Reference)
            {
                return GetClassName(((IEntityAssociation)attribute.OwnerObject).RoleBridge);
            }

            string type;

            switch (attribute.Type)
            {
                case DataAttributeTypes.dtString:
                    type = "string";
                    break;
                case DataAttributeTypes.dtInteger:
                    type = "int";
                    break;
                case DataAttributeTypes.dtDouble:
                    type = "Decimal";
                    break;
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    type = "DateTime";
                    break;
                case DataAttributeTypes.dtChar:
                    type = "char";
                    break;
                case DataAttributeTypes.dtBoolean:
                    type = "bool";
                    break;
                case DataAttributeTypes.dtBLOB:
                    type = "byte[]";
                    break;
                default:
                    type = "object";
                    break;
            }

            if ((type != "string") && (type != "byte[]") && attribute.IsNullable)
            {
                type += '?';
            }

            return type;
        }
    }
}
