using System;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.HelpGenerator
{
    /// <summary>
    /// Создание урезанного описания многомерки
    /// </summary>
    public class CutFMMD_All
    {
        /// <summary>
        /// !!Точка входа!! 
        /// </summary>
        /// <param name="fmmd_all"></param>
        public static void CutDatabase(ref XmlDocument fmmd_all, CutFMMD_AllForm form)
        {
            // удаляем лишние измерения
            CutDatabaseDimensions(ref fmmd_all, form);
            // удаляем лишние кубы
            CutCube(ref fmmd_all, form);
        }

        /// <summary>
        /// Проверка измерений
        /// </summary>
        /// <param name="fmmd_all"></param>
        private static void CutDatabaseDimensions(ref XmlDocument fmmd_all, CutFMMD_AllForm form)
        {
            form.RichTextBoxCut.Text += "\nОбработка измерений...";
            XmlNode root = fmmd_all.SelectSingleNode("//XMLDSOConverter/Databases/Database/DatabaseDimensions");
            XmlNodeList list =
                fmmd_all.SelectNodes("//XMLDSOConverter/Databases/Database/DatabaseDimensions/DatabaseDimension");
            foreach (XmlNode dimension in list)
            {
                if (dimension.SelectSingleNode("CustomProperties/Property[@name = 'ObjectKey']") != null)
                {
                    string  fullName =
                        dimension.SelectSingleNode("CustomProperties/Property[@name = 'ObjectKey']").InnerText;

                    if(!String.IsNullOrEmpty(fullName))
                    {
                        IEntity obj = SchemeEditor.SchemeEditor.Instance.Scheme.RootPackage.FindEntityByName(fullName);
                        if(obj == null)
                        {
                            form.RichTextBoxCut.Text += "\n\tУдаление измерения " + dimension.Attributes["name"];
                            root.RemoveChild(dimension);

                            // Необходимо это измерение также почикать во всех кубах, где оно используется
                            XmlNodeList cubeslist = fmmd_all.SelectNodes("//XMLDSOConverter/Databases/Database/Cubes/Cube");
                            foreach (XmlNode cube in cubeslist)
                            {
                                XmlNode cubeDimensions = cube.SelectSingleNode("CubeDimensions");
                                foreach (XmlNode cubeDimension in cube.SelectNodes("CubeDimensions/CubeDimension"))
                                {
                                    if (string.Compare(dimension.Attributes["name"].InnerText, cubeDimension.Attributes["name"].InnerText) == 0)
                                    {
                                        form.RichTextBoxCut.Text += "\n\t\tУдаление измерения " + dimension.Attributes["name"] + " из куба " + cube.Attributes["name"].InnerText;
                                        cubeDimensions.RemoveChild(cubeDimension);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Проверка кубов
        /// </summary>
        /// <param name="fmmd_all"></param>
        private static void CutCube(ref XmlDocument fmmd_all, CutFMMD_AllForm form)
        {
            form.RichTextBoxCut.Text += "\nОбработка кубов...";
            XmlNode root = fmmd_all.SelectSingleNode("//XMLDSOConverter/Databases/Database/Cubes");
            XmlNodeList list =
                fmmd_all.SelectNodes("//XMLDSOConverter/Databases/Database/Cubes/Cube");
            foreach (XmlNode cube in list)
            {
                if (cube.SelectSingleNode("CustomProperties/Property[@name = 'ObjectKey']") != null)
                {
                    string fullName =
                        cube.SelectSingleNode("CustomProperties/Property[@name = 'ObjectKey']").InnerText;

                    if (!String.IsNullOrEmpty(fullName))
                    {
                        IEntity obj = SchemeEditor.SchemeEditor.Instance.Scheme.RootPackage.FindEntityByName(fullName);
                        if (obj == null)
                        {
                            form.RichTextBoxCut.Text += "\n\tУдаление куба " + cube.Attributes["name"];
                            root.RemoveChild(cube);
                        }
                    }
                }
            }
        }
    }
}
