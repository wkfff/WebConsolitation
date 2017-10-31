using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharpSvn;

namespace Krista.FM.Utils.Common
{
    /// <summary>
    /// Статические методы, используемые при слиянии
    /// </summary>
    internal class JoinHelperClass
    {
        internal static bool CheckDirectories(string sourcePackage)
        {
            if (!Directory.Exists(string.Format("{0}\\Cubes", sourcePackage)))
                return false;

            if (!Directory.Exists(string.Format("{0}\\DatabaseDimensions", sourcePackage)))
                return false;

            return true;
        }

        internal static string GetXmlPath(string basePath, XElement xElement)
        {
            IEnumerable<XElement> parentPackage =
                (from el in xElement.Ancestors("Package") orderby el.Nodes().Count() descending select el);

            string xmlPath = parentPackage.Aggregate(basePath,
                                                     (current, element) =>
                                                     current + string.Format("{0}\\", element.Attribute("name").Value));

            return xmlPath + xElement.Attribute("name").Value;
        }

        internal static bool CheckPartition(XElement databaseCube)
        {
            return databaseCube.Descendants("Partition").Count() > 1
                       ? true
                       : false;
        }

        internal static void ClearAttributes(string pathDirectory)
        {
            if (Directory.Exists(pathDirectory))
            {
                string[] subDirs = Directory.GetDirectories(pathDirectory);
                foreach (string dir in subDirs)
                    ClearAttributes(dir);
                string[] files = files = Directory.GetFiles(pathDirectory);
                foreach (string file in files)
                    File.SetAttributes(file, FileAttributes.Normal);
            }
        }

        internal static void AddCubesFromXmlPackage(List<string> cubes, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            IEnumerable<XElement> cs = (from el in doc.Descendants("Cube") select el);
            foreach (var xElement in cs)
            {
                if (!cubes.Contains(xElement.Attribute("name").Value))
                    cubes.Add(xElement.Attribute("name").Value);
            }
        }

        internal static string GetCubeRevision(string cubeFullName)
        {
            try
            {
                string revision = null;
                using (SvnClient client = new SvnClient())
                {

                    SvnTarget target = SvnTarget.FromString(cubeFullName);

                    if (!String.IsNullOrEmpty(client.GetUriFromWorkingCopy(cubeFullName).ToString()))
                    {
                        SvnInfoEventArgs info;
                        client.GetInfo(target, out info);
                        revision = info.LastChangeRevision.ToString();
                    }

                }
                return revision;
            }
            catch (Exception e)
            {
                throw new JoinException(string.Format("При получении ревизии возникло исключение: {0}", e.Message));
            }
        }
    }
}
