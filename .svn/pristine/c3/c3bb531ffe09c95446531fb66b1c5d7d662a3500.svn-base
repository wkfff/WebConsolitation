using System;
using System.Linq;
using System.Xml.Linq;

namespace Krista.FM.Utils.Common
{
    /// <summary>
    /// Статические медоды классы SplitOperation
    /// </summary>
    public class SplitHelperClass
    {
        internal static string CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = AppDomain.CurrentDomain.BaseDirectory;
            return path;
        }

        #region Валидация

        internal static void Validate(XElement element, bool throwError)
        {
            switch (element.Name.ToString())
            {
                case "DatabaseDimension":
                    ValidateDatabaseDimension(element, throwError);
                    break;
                case "Cube":
                    ValidateCube(element, throwError);
                    break;
            }
        }

        private static void ValidateCube(XElement element, bool throwError)
        {
            XElement xElement = element.Descendants("CustomProperties").Where(el => el.Parent.Name != "DataSource").First();

            if (xElement != null)
            {
                AddID(element, xElement);

                if (xElement.Elements().Where(el => el.Attribute("name").Value == "MeasureGroupID").Count() == 0)
                {
                    XElement el =
                        XElement.Parse(String.Format(@"<Property name=""MeasureGroupID"" datatype=""8""><![CDATA[{0}]]></Property>",
                                                     Guid.NewGuid()));
                    xElement.Add(el);
                    Trace.TraceInformation(String.Format("У куба {0} в CustomProperties добавлен элемент MeasureGroupID", element.Attribute("name").Value));
                }

                if (element.Attribute("SubClassType").Value == "0")
                {
                    CheckObjectKeyAndFullName(element, xElement, throwError);
                }
            }
            foreach (var descendant in element.Descendants("Partition"))
            {
                XElement xElementCP;
                if (descendant.Descendants("CustomProperties").Where(el => el.Parent.Name != "DataSource").Count() == 0)
                {
                    xElementCP = new XElement("CustomProperties");
                    descendant.Add(xElementCP);
                }
                else
                {
                    xElementCP = descendant.Descendants("CustomProperties").Where(el => el.Parent.Name != "DataSource").First();
                }

                if (xElementCP != null)
                {
                    AddID(element, xElementCP);

                    if (xElementCP.Elements().Where(el => el.Attribute("name").Value == "ObjectKey").Count() == 0)
                    {
                        if (throwError)
                            throw new JoinException(String.Format("У объекта {0} в CustomProperties не задан ObjectKey",
                                          descendant.Attribute("name").Value));

                        Trace.TraceWapning(
                            String.Format("У партиции {0} в кубе {1} в CustomProperties не задан ObjectKey",
                                          descendant.Attribute("name").Value, element.Attribute("name").Value));
                    }   
                }
            }
        }

        private static void ValidateDatabaseDimension(XElement element, bool throwError)
        {
            try
            {
                XElement xElement = element.Descendants("CustomProperties").Where(el => el.Parent.Name != "DataSource").First();

                if (xElement != null)
                {
                    AddID(element, xElement);

                    if (xElement.Elements().Where(el => el.Attribute("name").Value == "HierarchyID").Count() == 0)
                    {
                        XElement el =
                            XElement.Parse(String.Format(@"<Property name=""HierarchyID"" datatype=""8""><![CDATA[{0}]]></Property>",
                                                         Guid.NewGuid()));
                        xElement.Add(el);
                        Trace.TraceInformation(String.Format("У измерения {0} в CustomProperties добавлен элемент HierarchyID", element.Attribute("name").Value));
                    }

                    CheckObjectKeyAndFullName(element, xElement, throwError);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static void AddID(XElement element, XElement xElement)
        {
            if (xElement.Elements().Where(el => el.Attribute("name").Value == "ID").Count() == 0)
            {
                XElement el =
                    XElement.Parse(String.Format(@"<Property name=""ID"" datatype=""8""><![CDATA[{0}]]></Property>",
                                                 Guid.NewGuid()));
                xElement.Add(el);
                Trace.TraceInformation(String.Format("У объекта {0} в CustomProperties добавлен элемент ID", element.Attribute("name").Value));
            }
        }

        private static void CheckObjectKeyAndFullName(XElement element, XElement xElement, bool throwError)
        {
            if (xElement.Elements().Where(el => el.Attribute("name").Value == "FullName").Count() == 0)
            {
                if (throwError)
                    throw new JoinException(String.Format("У объекта {0} в CustomProperties не задан FullName",
                                  element.Attribute("name").Value));
                Trace.TraceWapning(
                    String.Format("У объекта {0} в CustomProperties не задан FullName",
                                  element.Attribute("name").Value));
            }

            if (xElement.Elements().Where(el => el.Attribute("name").Value == "ObjectKey").Count() == 0)
            {
                if (throwError)
                    throw new JoinException(String.Format("У объекта {0} в CustomProperties не задан ObjectKey",
                                  element.Attribute("name").Value));
                Trace.TraceWapning(
                    String.Format("У объекта {0} в CustomProperties не задан ObjectKey",
                                  element.Attribute("name").Value));
            }
        }

        #endregion
    }
}
