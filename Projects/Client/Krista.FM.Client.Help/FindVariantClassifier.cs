using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Krista.FM.Client.Common.Forms;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Help
{
    public class FindVariantClassifier
    {
        private IScheme scheme;
        private Operation operation; 

        private StreamWriter writer;

        /// <summary>
        /// Файл для сохраниния найденных ситуаций
        /// </summary>
        private const string path = "data.txt";

        private const string variants = "variants.txt";

        public FindVariantClassifier(IScheme scheme)
        {
            operation = new Operation();
            try
            {
                operation.Text = "Поиск вариантов классификаторов";
                operation.StartOperation();

                Stream stream = File.Open(variants, FileMode.OpenOrCreate);
                writer = new StreamWriter(stream);

                this.scheme = scheme;

                FindVariants(scheme.RootPackage);

                writer.Close();
                stream.Close();
            }
            finally
            {
                operation.StopOperation();
                operation.ReleaseThread();
            }
        }

        private void FindVariants(IPackage package)
        {
            foreach (IPackage value in package.Packages.Values)
            {
                FindVariants(value);
            }

            foreach (IEntityAssociation entityAssociation in package.Associations.Values)
            {
                if (entityAssociation.RoleData.ClassType == ClassTypes.clsFactData && entityAssociation.RoleBridge.ClassType == ClassTypes.clsBridgeClassifier)
                {
                    int i = 1;
                    foreach (IEntityAssociation association in entityAssociation.RoleBridge.Associated.Values)
                    {
                        if (association.FullName != entityAssociation.FullName)
                        {
                            bool containsKey = false;
                            foreach (IEntityAssociation value in association.RoleData.Associated.Values)
                                if (value.RoleData.FullName == entityAssociation.RoleData.FullName)
                                    containsKey = true;

                            if (containsKey)
                            {
                                writer.Write(String.Format("  Вариант {0} : {1}", i, association.RoleData.FullCaption));
                                i++;
                            }
                        }
                    }
                    writer.Write(String.Format("  Ассоциации: {0}", entityAssociation.FullName));
                    writer.WriteLine();
                    //writer.WriteLine(String.Format("{0} _______ {1} _______{2}", entityAssociation.FullCaption, entityAssociation.FullName, entityAssociation.ObjectKey));
                }
            }
        }
    }
}
