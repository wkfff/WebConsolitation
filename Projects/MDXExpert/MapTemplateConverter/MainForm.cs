using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using Dundas.Utilities;

namespace Krista.FM.Client.MDXExpert.MapTemplateConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            map.Viewport.EnablePanning = true;
            map.Viewport.OptimizeForPanning = true;
        }

        private string SelectFolder()
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            // Показываем надпись в наверху диалога.
            d.Description = "Выберите папку";
            // Выбираем первоначальную папку.
            d.SelectedPath = @"C:\";
            // Показываем диалог.
            if (d.ShowDialog() == DialogResult.OK)
            {
                // Изменяем залоговок окна на выбранную папку.
                return d.SelectedPath;
            }
            return "";
        }

        private void btSelectSourceDir_Click(object sender, EventArgs e)
        {
            tbSourceDir.Text = SelectFolder();
        }

        private void btSelectDestinationDir_Click(object sender, EventArgs e)
        {
            tbDestinationDir.Text = SelectFolder();
        }

        private void btConvert_Click(object sender, EventArgs e)
        {
            if(!Directory.Exists(tbSourceDir.Text))
                return;

            if (!Directory.Exists(tbDestinationDir.Text))
            {
                Directory.CreateDirectory(tbDestinationDir.Text);
            }

            LoadMapFolders(tbSourceDir.Text);
          

        }

        private bool IsTemplate(string path)
        {
            bool result = false;
            try
            {
                if (path != string.Empty)
                    result = (Directory.GetFiles(path, "*.shp", SearchOption.TopDirectoryOnly).Length > 0);
            }
            catch
            {
            }
            return result;
        }

        private void LoadMapFolders(string root)
        {
            this.tbResultLog.Text = String.Empty;
            string[] directories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);

            try
            {
                foreach (string directory in directories)
                {
                    if (IsTemplate(directory))
                    {
                        LoadMapTemplate(directory);
                        foreach (Shape sh in this.map.Shapes)
                        {
                            sh.TextVisibility = TextVisibility.Shown;
                        }
                        this.map.Serializer.Content = SerializationContent.All;
                        this.map.Serializer.SerializableContent = "*.*";
                        string newMapDirectory = directory.Replace(tbSourceDir.Text, tbDestinationDir.Text);
                        string templateName = directory.Substring(directory.LastIndexOf("\\"));
                        Directory.CreateDirectory(newMapDirectory);
                        File.Create(newMapDirectory + "\\" + templateName + ".emt").Close();
                        this.map.Serializer.Save(newMapDirectory + "\\" + templateName + ".emt");
                        this.tbResultLog.AppendText("Шаблон \"" + directory + "\" переконвертирован." + Environment.NewLine);
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            this.tbResultLog.AppendText("*** Конвертирование завершено.");

        }


        public void LoadMapTemplate(string mapDirectory)
        {
            map.Layers.Clear();
            map.Groups.Clear();
            map.Shapes.Clear();
            if (!Directory.Exists(mapDirectory))
            {
                MessageBox.Show("Шаблон \"" + mapDirectory + "\" не найден.");
                return;
            }


            string[] files = Directory.GetFiles(mapDirectory, "*.shp", SearchOption.TopDirectoryOnly);

            DefineLayerOrder(ref files);

            foreach (string fileName in files)
            {
                AddLayer(fileName);
            }
            //ClearNullShapeNames();


            int i = 0;
            while (i < map.Layers.Count)
            {
                Layer layer = map.Layers[i];
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
                string layerName = System.IO.Path.GetFileNameWithoutExtension(layerFiles[k]);
                if (layerName.ToUpper() == "ГОРОДА")
                {
                    string lastLayerName = layerFiles[layerFiles.Length - 1];
                    layerFiles[layerFiles.Length - 1] = layerFiles[k];
                    layerFiles[k] = lastLayerName;
                    break;
                }
            }

        }
        
        private void AddLayer(string layerFileName)
        {
            string layerName = System.IO.Path.GetFileNameWithoutExtension(layerFileName);
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
            string dataBase = System.IO.Path.GetDirectoryName(layerFileName);
            string tableName = System.IO.Path.GetFileNameWithoutExtension(layerFileName);

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


    }
}