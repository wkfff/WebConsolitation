using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Dundas.Maps.WinControl;
using Dundas.Utilities;
using Microsoft.Win32;
using Path=System.IO.Path;
using Microsoft.Win32;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    public partial class TunerForm : Form
    {
        private string mapRepositoryPath = "";
        private string templateName = "";
        public bool showExtendedSettings = false;


        public string MapRepositoryPath
        {
            get { return mapRepositoryPath; }
            set
            {
                mapRepositoryPath = value;
                Consts.mapRepositoryPath = value;
                SaveRegSettings();
            }
        }

        public string TemplateName
        {
            get { return templateName; }
            set
            {
                if (templateName == value)
                {
                    return;
                }
                templateName = value;
                LoadMapTemplate(MapRepositoryPath + value);
                /*
                foreach (Shape sh in this.map.Shapes)
                {
                    sh.TextVisibility = TextVisibility.Shown;
                }*/

                LoadPreset();
            }
        }


        public TunerForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            LoadRegSettings();
            propertyGrid.SelectedObject = new MapBrowseAdapter(this, this.map);
        }

        public void LoadMapTemplate(string mapDirectory)
        {
            map.Layers.Clear();
            map.Groups.Clear();
            map.Shapes.Clear();
            map.Serializer.Reset();
            if (!Directory.Exists(mapDirectory))
            {
                MessageBox.Show("Шаблон \"" + mapDirectory + "\" не найден.");
                return;
            }


            string[] files = Directory.GetFiles(mapDirectory, "*.emt", SearchOption.TopDirectoryOnly);


            
            //DefineLayerOrder(ref files);

            foreach (string fileName in files)
            {
                this.map.Serializer.Reset();
                this.map.Serializer.SerializableContent = "*.*";
                this.map.Serializer.Content = SerializationContent.All;

                map.Serializer.Load(fileName);
                //AddLayer(fileName);
            }
            
            ClearNullShapeNames();


            int i = 0;
            while (i < map.Layers.Count)
            {
                Layer layer = map.Layers[i];
                layer.Tag = true;
                if (!LayerHasShapes(layer))
                {
                    map.Layers.Remove(layer);
                }
                else
                {
                    i++;
                }
            }
        }

        private void ClearNullShapeNames()
        {
            this.map.ShapeFields.Clear();
            foreach (Shape sh in map.Shapes)
            {
                if (sh.Name.Contains("Shape") || sh.Name.Contains("(no data)"))
                {
                    sh.Text = "";
                }
            }
        }


        /// <summary>
        /// установить очередь загрузки слоев
        /// </summary>
        /// <param name="layerFiles"></param>
        private void DefineLayerOrder(ref string[] layerFiles)
        {
            //загружаем слой с городами в последнюю очередь
            for (int k = 0; k < layerFiles.Length; k++)
            {
                string layerName = Path.GetFileNameWithoutExtension(layerFiles[k]);
                if (layerName.ToUpper() == "ГОРОДА")
                {
                    string lastLayerName = layerFiles[layerFiles.Length - 1];
                    layerFiles[layerFiles.Length - 1] = layerFiles[k];
                    layerFiles[k] = lastLayerName;
                    break;
                }
            }

        }


        #region Слои

        private void AddLayer(string layerFileName)
        {
            string layerName = Path.GetFileNameWithoutExtension(layerFileName);
            int oldShapesCount = map.Shapes.Count;

            bool objectsHasNames = ObjectsHasNames(layerFileName);
            map.LoadFromShapeFile(layerFileName, objectsHasNames ? "NAME" : "", true);
            Layer layer = map.Layers.Add(layerName);
            layer.Tag = true;

            Group group = map.Groups.Add(layerName);
            group.Text = "";

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                map.Shapes[i].Layer = layerName;
                map.Shapes[i].ParentGroup = layerName;
            }

            RestoreLayerForShapes(layerName);
        }

        //имя слоя для объектов может сбиться, если загружаем слои с одинаковыми объектами... воcстановим его 
        private void RestoreLayerForShapes(string layerName)
        {
            foreach (Shape sh in map.Shapes)
            {
                if (sh.Layer == "(none)")
                {
                    sh.Layer = layerName;
                    sh.ParentGroup = layerName;
                }
            }
        }

        private bool LayerHasShapes(Layer layer)
        {
            foreach (Shape sh in map.Shapes)
            {
                if (sh.Layer == layer.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ObjectsHasNames(string layerFileName)
        {
            string dataBase = Path.GetDirectoryName(layerFileName);
            string tableName = Path.GetFileNameWithoutExtension(layerFileName);

            if (!File.Exists(dataBase + "\\" + tableName + ".dbf"))
            {
                // FormException.ShowErrorForm(new Exception(string.Format("Не найдены данные для слоя \"{0}\".", tableName)),
                //             ErrorFormButtons.WithoutTerminate);
                return false;
            }
            DataTable table = ShapeFileReader.ReadDBF(dataBase + "\\" + tableName + ".dbf");
            if (table == null)
            {
                return false;
            }
            return table.Columns.Contains("NAME");
        }

        #endregion

        private string GetSettingsFileName()
        {
            int beginPos = this.TemplateName.LastIndexOf("\\");
            return this.TemplateName.Substring(beginPos + 1);
        }

        public void SaveSettings()
        {

            string fileName = String.Format("{0}{1}\\{2}.xml", this.MapRepositoryPath, this.TemplateName, GetSettingsFileName());
            FileStream stream = null;
            stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            XmlSerializer xmlFormat = new XmlSerializer(typeof(XmlNode));
            xmlFormat.Serialize(stream, GetPresetXml());
            stream.Close();
        }

        private void LoadPreset()
        {
            string fileName = String.Format("{0}{1}\\{2}.xml", this.MapRepositoryPath, this.TemplateName, GetSettingsFileName());
            if (!File.Exists(fileName))
                return;

            FileStream stream = null;
            XmlNode presetNode = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlSerializer xmlFormat = new XmlSerializer(typeof(XmlNode));
                presetNode = (XmlNode)xmlFormat.Deserialize(stream);
                stream.Close();
            }
            catch(Exception e) 
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        


            if (presetNode == null)
                return;

            string reportElementPreset = presetNode.FirstChild.Value;

            if (reportElementPreset != string.Empty)
            {
                using (StringReader strReader = new StringReader(reportElementPreset))
                {
                    this.map.Serializer.Content = SerializationContent.All;
                    this.map.Serializer.ResetWhenLoading = false;
                    this.map.Serializer.Load(strReader);
                }
            }
        }


        public static XmlNode AddChildNode(XmlNode xmlRootNode, string tagName, params string[][] attributes)
        {
            XmlNode xmlNode = xmlRootNode.OwnerDocument.CreateElement(tagName);
            if (attributes != null)
            {
                foreach (string[] attribute in attributes)
                {
                    XmlAttribute xmlAttribute = xmlRootNode.OwnerDocument.CreateAttribute(attribute[0]);
                    xmlAttribute.Value = attribute[1];
                    xmlNode.Attributes.Append(xmlAttribute);
                }

            }
            xmlRootNode.AppendChild(xmlNode);
            return xmlNode;
        }


        private XmlNode GetPresetXml()
        {
            XmlDocument dom = new XmlDocument();
            XmlNode presetNode = dom.CreateElement("Presets");
            using (StringWriter strWriter = new StringWriter())
            {
                this.map.Serializer.SerializableContent = "*.*";
                this.map.Serializer.NonSerializableContent = "Shape.EncodedShapeData,*.Zoom,ViewCenter.*";
                this.map.Serializer.Save(strWriter);
                AppendCDataSection(presetNode, strWriter.ToString());
            }
            return presetNode;
        }

        public static void AppendCDataSection(XmlNode xmlParentNode, string value)
        {
            XmlCDataSection cdata = xmlParentNode.OwnerDocument.CreateCDataSection(value);
            xmlParentNode.AppendChild(cdata);
        }


        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void map_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.P))
            {
                this.showExtendedSettings = !this.showExtendedSettings;
                this.propertyGrid.Refresh();
            }

        }

   
        private void LoadRegSettings()
        {
            RegistryKey readKey = Registry.CurrentUser.OpenSubKey("software\\krista\\FM\\Client\\MDXExpert\\MapTuner");
            if (readKey == null)
            {
                return;
            }
            MapRepositoryPath = (string)readKey.GetValue("MapRepositoryPath", "");
            readKey.Close(); 
        }

        private void SaveRegSettings()
        {
            RegistryKey saveKey = Registry.CurrentUser.CreateSubKey("software\\krista\\FM\\Client\\MDXExpert\\MapTuner");
            saveKey.SetValue("MapRepositoryPath", MapRepositoryPath);
            saveKey.Close(); 

        }

    }
}