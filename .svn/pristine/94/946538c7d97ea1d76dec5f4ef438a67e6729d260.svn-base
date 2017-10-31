using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Diagnostics;

namespace PackageCleaner2000
{
	public partial class frmMain : Form
	{
		protected FileInfo fmmdAllFile;		
        XPathDocument fmmdAll;
		XPathNavigator fmmdNavigator;
		List<object> cubes = new List<object>();
		List<object> usedDimensions;
		List<object> unUsedDimensions;
        private const string addString = "Add";
        private const string replaceString = "Replace";
        private const string noneString = "None";

		public frmMain()
		{
			InitializeComponent();
		}

		private void SetButtonsState(bool btnOpenState, bool btnAcceptCubesState,
            bool btnAcceptDimsState, bool btnAcceptPackageState)
		{
			btnOpenFMMD.Enabled = btnOpenState;
			btnAcceptCubes.Enabled = btnAcceptCubesState;
			btnAcceptDimensions.Enabled = btnAcceptDimsState;
            btnAcceptPackage.Enabled = btnAcceptPackageState;

            radioButtonCubeAdd.Enabled = btnAcceptCubesState;
            radioButtonCubeReplace.Enabled = btnAcceptCubesState;

            radioButtonDimensionAdd.Enabled = btnAcceptDimsState;
            radioButtonDimensionReplace.Enabled = btnAcceptDimsState;

            radioButtonPackageNew.Enabled = btnAcceptPackageState;
            radioButtonPackageReplace.Enabled = btnAcceptPackageState;
		}

		private void btnOpenFMMD_Click(object sender, EventArgs e)
		{
			if (openFileDlgMain.ShowDialog(this) == DialogResult.OK)
			{
				fmmdAllFile = new FileInfo(openFileDlgMain.FileName);
				fmmdAll = new XPathDocument(openFileDlgMain.FileName);
				fmmdNavigator = fmmdAll.CreateNavigator();
				XPathNodeIterator cubesNodes = fmmdNavigator.Select(".//Cubes/Cube");
				while (cubesNodes.MoveNext())
				{
					cubes.Add(cubesNodes.Current.GetAttribute("name", ""));
				}
				TwoLists.Init(cubes, string.Empty, "Переместите ненужные кубы в левый список",
					"Ненужные кубы", "Нужные кубы", null, null);
				SetButtonsState(false, true, false, false);
			}
		}			

		private List<object> GetUsedDimensions(List<object> cubes)
		{
			List<object> usedDims = new List<object>();
			for (int i = 0; i < cubes.Count; i++)
			{
				XPathNodeIterator dimsNodes = fmmdNavigator.Select(
					string.Format("//Cube[@name=\"{0}\"]//CubeDimension", cubes[i]));
				while (dimsNodes.MoveNext())
				{
					string dimName = dimsNodes.Current.GetAttribute("name", "");
					if (!usedDims.Contains(dimName))
					{
						usedDims.Add(dimName);
					}
				}
			}
			return usedDims;
		}

		private List<object> GetUnUsedDimensions(List<object> usedDims)
		{
			List<object> unUsedDims = new List<object>();
			XPathNodeIterator nodes = fmmdNavigator.Select(".//DatabaseDimensions/DatabaseDimension");
			while (nodes.MoveNext())
			{
				string dimName = nodes.Current.GetAttribute("name", "");
				if (!usedDims.Contains(dimName))
				{
					unUsedDims.Add(dimName);
				}				
			}
			return unUsedDims;
		}	

		private void btnAcceptCubes_Click(object sender, EventArgs e)
		{
			usedDimensions = GetUsedDimensions(TwoLists.rightList.Items);
			unUsedDimensions = GetUnUsedDimensions(usedDimensions);

            //TwoLists.Init(new List<object>(usedDimensions),
            //    string.Empty, "Переместите ненужные кубы в левый список",
            //    "Измерения используемые в других кубах",
            //    "Измерения используемые в этих кубах", null, null);
            TwoLists.Init(usedDimensions,
                string.Empty, "Переместите ненужные кубы в левый список",
                "Измерения используемые в других кубах",
                "Измерения используемые в этих кубах", null, null);
			TwoLists.leftList.Init(
				unUsedDimensions, "Измерения используемые в других кубах", null, null, null);
			SetButtonsState(false, false, true, false);
		}

		private XmlWriterSettings GetWriterSettings()
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = Encoding.Default;			
			xmlWriterSettings.IndentChars = "\t";
			xmlWriterSettings.NewLineChars = Environment.NewLine;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
			xmlWriterSettings.NewLineOnAttributes = false;
			xmlWriterSettings.CloseOutput = true;
			return xmlWriterSettings;
		}

        private string InitCubeMethod()
        {
            if (radioButtonCubeAdd.Checked)
            {
                return addString;
            }
            else
            {
                return replaceString;
            }
        }

        private string InitDimensionMethod()
        {
            if (radioButtonDimensionAdd.Checked)
            {
                return addString;
            }
            else
            {
                return replaceString;
            }
        }

        private void CorrectDimensionAndCubesMethod(string dirName)
        {
            string cubeMethod = InitCubeMethod();
            string dimensionMethod = InitDimensionMethod();
            string fmmdAllFileName = dirName + "\\FMMD_All.xml";
            XmlDocument newFmmdAll = new XmlDocument();
            newFmmdAll.Load(fmmdAllFileName);
            XPathNavigator navigator = newFmmdAll.CreateNavigator();
            XPathNodeIterator items = navigator.Select(".//DatabaseDimensions/DatabaseDimension");
            while (items.MoveNext())
            {
                items.Current.SelectSingleNode("@method").SetValue(dimensionMethod);
            }

            items = navigator.Select(".//Cubes/Cube");
            while (items.MoveNext())
            {
                items.Current.SelectSingleNode("@method").SetValue(cubeMethod);
            }
            newFmmdAll.Save(fmmdAllFileName);
        }

		private void WriteFmmdAllAdd(string dirName, string databaseMethod)
		{
            XPathNavigator elementNavigator;
            XmlWriter writer = XmlWriter.Create(dirName + "\\FMMD_All.xml", GetWriterSettings());
			writer.WriteStartDocument();
			try
			{
				writer.WriteStartElement("XMLDSOConverter");
				writer.WriteAttributeString("version", "1.0");

				writer.WriteStartElement("Databases");

				writer.WriteStartElement("Database");                
                writer.WriteAttributeString("method", databaseMethod);
				writer.WriteAttributeString("name", "/*#DatabaseName#*/");
				writer.WriteAttributeString("ClassType", "2");
				writer.WriteAttributeString("SubClassType", "0");
				writer.WriteAttributeString("Description", "");
				writer.WriteAttributeString("OlapMode", "0");
				writer.WriteAttributeString("LastUpdated", "12:00:00 AM");
				writer.WriteAttributeString("IsVisible", "true");

				writer.WriteStartElement("DatabaseDimensions");
				for (int i = 0; i < usedDimensions.Count; i++)
				{
                    elementNavigator = fmmdNavigator.SelectSingleNode(
                        string.Format(".//DatabaseDimensions/DatabaseDimension[@name=\"{0}\"]",
                        usedDimensions[i]));                    
                    writer.WriteNode(elementNavigator, true);                    
				}
				writer.WriteEndElement();//DatabaseDimensions

				writer.WriteStartElement("Cubes");
				for (int i = 0; i < cubes.Count; i++)
				{
                    elementNavigator = fmmdNavigator.SelectSingleNode(
                        string.Format(".//Cubes/Cube[@name=\"{0}\"]", cubes[i]));
                    writer.WriteNode(elementNavigator, true);
				}
				writer.WriteEndElement();//Cubes

				writer.WriteEndElement();//Database				

				writer.WriteEndElement();//Databases

				writer.WriteEndElement();//XMLDSOConverter				
			}
			finally
			{
				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
			}
		}

		private bool NeedToDeletePartitions(string cubeName)
		{
			if (cubeName.Equals("ФО_АС Бюджет_План доходов", StringComparison.OrdinalIgnoreCase) ||
				cubeName.Equals("ФО_АС Бюджет_Факт доходов", StringComparison.OrdinalIgnoreCase) ||
				cubeName.Equals("УФК_Свод ФУ", StringComparison.OrdinalIgnoreCase) ||
				cubeName.Equals("УФК_Форма 16", StringComparison.OrdinalIgnoreCase) ||
				cubeName.Equals("ФНС_28н_без расщепления", StringComparison.OrdinalIgnoreCase) ||
				cubeName.Equals("ФНС_28н_с расщеплением", StringComparison.OrdinalIgnoreCase) ||
				cubeName.Equals("ФО_МесОтч_Доходы", StringComparison.OrdinalIgnoreCase) ||
				cubeName.Equals("ФО_АС Бюджет_КазнИсп_Финансирование", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private void WriteDeletePartitions(string dirName)
		{
			XmlWriter writer = XmlWriter.Create(dirName + "\\FMMD_Delete_Partitions.xml",
				GetWriterSettings());
			writer.WriteStartDocument();
			try
			{
				writer.WriteStartElement("XMLDSOConverter");
				writer.WriteAttributeString("version", "1.0");
				
				writer.WriteStartElement("Databases");

				writer.WriteStartElement("Database");                
				writer.WriteAttributeString("method", "none");
				writer.WriteAttributeString("name", "/*#DatabaseName#*/");
				writer.WriteAttributeString("ClassType", "2");
				writer.WriteAttributeString("SubClassType", "0");				

				writer.WriteStartElement("Cubes");
				for (int i = 0; i < cubes.Count; i++)
				{
					if (NeedToDeletePartitions(cubes[i].ToString()))
					{
						writer.WriteStartElement("Cube");

						writer.WriteAttributeString("method", "none");
						writer.WriteAttributeString("name", cubes[i].ToString());
						writer.WriteAttributeString("ClassType", "9");
						writer.WriteAttributeString("SubClassType", "0");

						writer.WriteStartElement("Partitions");

						writer.WriteStartElement("Partition");

						writer.WriteAttributeString("method", "Remove");
						writer.WriteAttributeString("name", cubes[i].ToString());
						writer.WriteAttributeString("ClassType", "19");
						writer.WriteAttributeString("SubClassType", "0");

						writer.WriteEndElement();//Partition

						writer.WriteEndElement();//Partitions

						writer.WriteEndElement();//Cube
					}					
				}
				writer.WriteEndElement();//Cubes

				writer.WriteEndElement();//Database				

				writer.WriteEndElement();//Databases

				writer.WriteEndElement();//XMLDSOConverter				
			}
			finally
			{
				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
			}
		}

		private void CopyPacketFile(string sourceDirName, string destDirName, string fileName)
		{
			File.Copy(string.Format(@"{0}\{1}", sourceDirName, fileName),
				string.Format(@"{0}\{1}", destDirName, fileName));
		}

		private void CopyPacketFiles(string sourceDirName, string destDirName)
		{
			try
			{   
                CopyPacketFile(sourceDirName, destDirName, "FMMD_Datasource.xml");
                CopyPacketFile(sourceDirName, destDirName,
                    Path.GetFileName(Directory.GetFiles(sourceDirName, "FMMD_Macros_*.xml")[0]));
                CopyPacketFile(sourceDirName, destDirName,
                    Path.GetFileName(Directory.GetFiles(sourceDirName, "FMMD_Package_*.xml")[0]));
                CopyPacketFile(sourceDirName, destDirName, "FMMD_Replaces_Oracle.xml");                				
			}
			catch (Exception e)
			{
                Trace.WriteLine(e.Message);
            }
		}

        private void CopyAndCorrectDataSourceFile(string sourceDirName, string destDirName)
        {
            try
            {
                string datasourceFileName = destDirName + @"\FMMD_Datasource.xml";
                CopyPacketFile(sourceDirName, destDirName, "FMMD_Datasource.xml");

                XmlDocument newDatasource = new XmlDocument();
                newDatasource.Load(datasourceFileName);
                XPathNavigator navigator = newDatasource.CreateNavigator();
                navigator.SelectSingleNode(".//Databases/Database/@method").SetValue(noneString);
                navigator.SelectSingleNode(".//Databases/Database/DataSources/DataSource/@method").SetValue(noneString);
                newDatasource.Save(datasourceFileName);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }

		private void btnAcceptDimensions_Click(object sender, EventArgs e)
		{	
			SetButtonsState(false, false, false, true);
		}

        private void frmMain_Shown(object sender, EventArgs e)
        {
            SetButtonsState(true, false, false, false);
        }

        private void btnAcceptPackage_Click(object sender, EventArgs e)
        {
            string sourceDirName = Path.GetDirectoryName(openFileDlgMain.FileName);
            string destDirName = sourceDirName + @"\OLAP_" + DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            Directory.CreateDirectory(destDirName);
                        
            if (radioButtonPackageNew.Checked)
            {
                WriteFmmdAllAdd(destDirName, addString);                
                CopyPacketFiles(sourceDirName, destDirName);
            }
            else
            {
                WriteFmmdAllAdd(destDirName, noneString);                
                CopyAndCorrectDataSourceFile(sourceDirName, destDirName);
            }
            CorrectDimensionAndCubesMethod(destDirName);
            WriteDeletePartitions(destDirName);


            //WriteFmmdAllAdd(destDirName, databaseMethod);
            //CorrectDimensionAndCubesMethod(destDirName);
            //WriteDeletePartitions(destDirName);            
            //CopyPacketFiles(sourceDirName, destDirName);
            MessageBox.Show("Создал новую конфигурацию в каталоге \"" + destDirName + "\"",
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetButtonsState(true, false, false, false);

            Process.Start("explorer", string.Format("\"{0}\"", destDirName));
        }
	}
}