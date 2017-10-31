using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common.Exceptions;

namespace Krista.FM.Client.Help
{
    public class SuppliersSave:CommonSave
    {
        public SuppliersSave(HelpManager manager)
            : base(manager)
        {
            CreateTransform();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        public override void SaveToHtml(System.Xml.XmlNode nodeBase)
        {
            try
            {
                XmlReader reader = XmlReader.Create(String.Format(@"{0}\output.xml", HelpManager.result));

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Auto;
                XmlWriter writer =
                    XmlWriter.Create(
                        String.Format(
                        @"{0}\{1}\suppliers.html", AppDomain.CurrentDomain.BaseDirectory, HelpManager.result), settings);

                Transform(reader, writer);

                writer.Close();
                reader.Close();
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentNullException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        protected override void CreateTransform()
        {
            base.CreateTransform();

            xslt.Load(String.Format(@"{0}\Suppliers.xsl", HelpManager.profile));
        }
    }
}
