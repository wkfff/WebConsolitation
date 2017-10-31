using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.XPath;
using System.IO;
using System.Xml;
using System.Diagnostics;


namespace PackageMaker2000
{
	/// <summary>
	/// Программа предназначена для создания двушаговых обновлений:
    /// на первом шаге из многомерной базы удалются объекты, предназначеные для замены,
    /// на втором шаге в многомерную базу добавляются новые версии обновленных объектов.
	/// </summary>
    public partial class frmMain : Form
	{
		XPathDocument fmmdAll;
		XPathNavigator fmmdNavigator;
		List<string> cubes = new List<string>();
		List<string> usedDimensions;
		List<string> usedOnlyForThis;
        string virtualCubePrefix = "_";

		public frmMain()
		{
			InitializeComponent();
		}

		private void btnOpenFile_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				fmmdAll = new XPathDocument(openFileDialog.FileName);
				fmmdNavigator = fmmdAll.CreateNavigator();
				XPathNodeIterator cubesNodes = fmmdNavigator.Select(".//Cubes/Cube");				
				while (cubesNodes.MoveNext())
				{
					cubes.Add(cubesNodes.Current.GetAttribute("name", ""));
				}				
				TwoLists.Init(cubes, "Кубы в пакете", "Кубы для замены");
				//btnCreatePatch.Enabled = true;
			}
		}

		private XmlWriterSettings GetWriterSettings()
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = Encoding.UTF8;
			xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
			xmlWriterSettings.Indent = true;
            xmlWriterSettings.IndentChars = Environment.NewLine;
			xmlWriterSettings.NewLineChars = Environment.NewLine;
			xmlWriterSettings.CloseOutput = true;
			return xmlWriterSettings;
		}

		private bool UsedOnlyForThis(List<string> cubes, string dimName)
		{
			XPathNodeIterator cubesNodes = fmmdNavigator.Select(
				string.Format("//Cube[.//CubeDimension/@name=\"{0}\"]", dimName));
			while (cubesNodes.MoveNext())
			{
				if (!cubes.Contains(cubesNodes.Current.GetAttribute("name", "")))
				{
					return false;
				}
			}
			return true;
		}

		private List<string> GetUsedOnly(List<string> cubes, List<string> usedDims)
		{
			List<string> usedOnly = new List<string>();
			int i = 0;
			while (i < usedDims.Count)
			{
				if (UsedOnlyForThis(cubes, usedDims[i]))
				{
					usedOnly.Add(usedDims[i]);
					usedDims.RemoveAt(i);					
				}
				else
				{
					i += 1;
				}				
			}
			return usedOnly;
		}

		private List<string> GetDimensionsList(List<string> cubes)
		{
			List<string> usedDims = new List<string>();
			for (int i = 0; i < cubes.Count; i++)
			{
				XPathNodeIterator dimsNodes = fmmdNavigator.Select(
					string.Format("//Cube[@name=\"{0}\"]//CubeDimension", cubes[i]));
				while (dimsNodes.MoveNext())
				{
					string dimName = dimsNodes.Current.GetAttribute("name", "");
					//if (!usedDims.Contains(dimName) && UsedOnlyForThis(cubes, dimName))
					if (!usedDims.Contains(dimName))
					{ 
						usedDims.Add(dimName);
					}
				}
			}
			return usedDims;
		}

		private XPathNavigator GetSharedDimension(string dimName)
		{
			return fmmdNavigator.SelectSingleNode(
				string.Format("//DatabaseDimension[@name = \"{0}\"]", dimName));
		}

        private int GetSubClass(string cubeName)
        {   
            return fmmdNavigator.SelectSingleNode(
                string.Format("//Cube[@name=\"{0}\"]/@SubClassType", cubeName)).ValueAsInt;
        }

        private void WriteCubeRemove(XmlWriter writer, string cubeName, int subClassType)
        {
            writer.WriteStartElement("Cube");
            writer.WriteAttributeString("method", "Remove");
            writer.WriteAttributeString("name", cubeName);
            writer.WriteAttributeString("ClassType", "9");
            writer.WriteAttributeString("SubClassType", subClassType.ToString());
            writer.WriteEndElement();//Cube;
        }

		private void WriteFmmdAllDelete(string dirName)
		{
			XmlWriter fmmdAllWriter = XmlWriter.Create(dirName + "\\FMMD_All.xml",
				GetWriterSettings());
			fmmdAllWriter.WriteStartDocument();
			try
			{
                fmmdAllWriter.WriteRaw(Environment.NewLine + "<XMLDSOConverter version=\"1.0\">" + Environment.NewLine);
				fmmdAllWriter.WriteStartElement("Databases");

                fmmdAllWriter.WriteRaw(Environment.NewLine + "<Database method=\"None\" name=\"/*#DatabaseName#*/\" ClassType=\"2\" SubClassType=\"0\" Description=\"\" OlapMode=\"0\" LastUpdated=\"12:00:00 AM\" IsVisible=\"true\">" + Environment.NewLine);

				fmmdAllWriter.WriteStartElement("Cubes");

                int subClassType = 0;

                //Сначала удаляем виртуальные кубы.
                for (int i = 0; i < cubes.Count; i++)
				{
                    subClassType = GetSubClass(cubes[i]);
                    if (subClassType == 1)
                    {
                        WriteCubeRemove(fmmdAllWriter, cubes[i], subClassType);
                        //Устанавливаем признак того, что куб виртуальный.
                        cubes[i] = virtualCubePrefix + cubes[i];
                    }                    
				}

                //Затем простые.
                for (int i = 0; i < cubes.Count; i++)
                {
                    if (cubes[i].StartsWith(virtualCubePrefix))
                    {
                        cubes[i] = cubes[i].Substring(virtualCubePrefix.Length, cubes[i].Length - virtualCubePrefix.Length);
                    }
                    else
                    {
                        WriteCubeRemove(fmmdAllWriter, cubes[i], GetSubClass(cubes[i]));
                    }
                }

				fmmdAllWriter.WriteEndElement();//Cubes

				//fmmdAllWriter.WriteEndElement();//Database
                fmmdAllWriter.WriteRaw(Environment.NewLine + "</Database>" + Environment.NewLine);

				fmmdAllWriter.WriteEndElement();//Databases

				//fmmdAllWriter.WriteEndElement();//XMLDSOConverter
                fmmdAllWriter.WriteRaw(Environment.NewLine + "</XMLDSOConverter>");
			}
			finally
			{
				fmmdAllWriter.WriteEndDocument();
				fmmdAllWriter.Flush();
				fmmdAllWriter.Close();
			}
		}

		private void WriteFmmdAllAdd(string dirName)
		{
			XmlWriter fmmdAllWriter = XmlWriter.Create(dirName + "\\FMMD_All.xml",
				GetWriterSettings());
			fmmdAllWriter.WriteStartDocument();
			try
			{
                fmmdAllWriter.WriteRaw(Environment.NewLine + "<XMLDSOConverter version=\"1.0\">" + Environment.NewLine);
				fmmdAllWriter.WriteStartElement("Databases");

                fmmdAllWriter.WriteRaw(Environment.NewLine + "<Database method=\"None\" name=\"/*#DatabaseName#*/\" ClassType=\"2\" SubClassType=\"0\" Description=\"\" OlapMode=\"0\" LastUpdated=\"12:00:00 AM\" IsVisible=\"true\">" + Environment.NewLine);

				fmmdAllWriter.WriteStartElement("DatabaseDimensions");
				for (int i = 0; i < usedDimensions.Count; i++)
				{	
					fmmdAllWriter.WriteNode(fmmdNavigator.SelectSingleNode(
						string.Format(".//DatabaseDimensions/DatabaseDimension[@name=\"{0}\"]",
						usedDimensions[i])), true);
				}
				fmmdAllWriter.WriteEndElement();//DatabaseDimensions

				fmmdAllWriter.WriteStartElement("Cubes");
				for (int i = 0; i < cubes.Count; i++)
				{
					fmmdAllWriter.WriteNode(fmmdNavigator.SelectSingleNode(
						string.Format(".//Cubes/Cube[@name=\"{0}\"]", cubes[i])), true);
				}
				fmmdAllWriter.WriteEndElement();//Cubes

				//fmmdAllWriter.WriteEndElement();//Database
                fmmdAllWriter.WriteRaw(Environment.NewLine + "</Database>" + Environment.NewLine);

				fmmdAllWriter.WriteEndElement();//Databases

				//fmmdAllWriter.WriteEndElement();//XMLDSOConverter
                fmmdAllWriter.WriteRaw(Environment.NewLine + "</XMLDSOConverter>");
			}
			finally
			{
				fmmdAllWriter.WriteEndDocument();
				fmmdAllWriter.Flush();
				fmmdAllWriter.Close();
			}
		}

		private void btnCreatePatch_Click(object sender, EventArgs e)
		{
			//usedDimensions = GetDimensionsList(TwoLists.rightList.Items);
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
				DirectoryInfo dirInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
				DirectoryInfo stepOneDir = dirInfo.CreateSubdirectory("Шаг_1");
				DirectoryInfo stepTwoDir = dirInfo.CreateSubdirectory("Шаг_2");

				WriteFmmdAllDelete(stepOneDir.FullName);
				WriteFmmdAllAdd(stepTwoDir.FullName);

                MessageBox.Show("Записал в каталог \"" + dirInfo.FullName + "\"");
                Process.Start("explorer", string.Format("\"{0}\"", dirInfo.FullName));                
			}
		}

		private void UpdateCubesByDims(List<string> dims)
		{
			List<string> extra = new List<string>();
			for (int i = 0; i < dims.Count; i++)
			{
				if (!usedOnlyForThis.Contains(dims[i]))
				{
					XPathNodeIterator extraNodes = fmmdNavigator.Select(
						string.Format(".//Cube[.//CubeDimension/@name=\"{0}\"]", dims[i]));
					while (extraNodes.MoveNext())
					{
						string cubeName = extraNodes.Current.GetAttribute("name", "");
						if (!cubes.Contains(cubeName)) { cubes.Add(cubeName); }
					}
				}
			}
		}

		private void btnSelectDims_Click(object sender, EventArgs e)
		{
			usedDimensions = GetDimensionsList(TwoLists.rightList.Items);
			usedOnlyForThis = GetUsedOnly(TwoLists.rightList.Items, usedDimensions);			
			
			TwoLists.Init(new List<string>(usedOnlyForThis),
				"Измерения используемые в других кубах",
				"Измерения используемые только в этих кубах");
			TwoLists.leftList.Init(usedDimensions, "Измерения используемые в других кубах");
		}

		private void btnShowAllCubes_Click(object sender, EventArgs e)
		{
			usedDimensions = TwoLists.rightList.Items;
			UpdateCubesByDims(usedDimensions);
			TwoLists.Init(cubes,
				"",
				"Кубы которые будут пропатчены");
		}

		private void btnSetUpdateMetod_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				XmlDocument step2FMMDALL = new XmlDocument();
				step2FMMDALL.Load(openFileDialog.FileName);
				
				XPathNavigator navToUpdate = step2FMMDALL.CreateNavigator();
				XPathNodeIterator dimNodes = navToUpdate.Select(".//DatabaseDimension");
				while (dimNodes.MoveNext())
				{
					dimNodes.Current.SelectSingleNode("./@method").SetValue("Replace");
				}
				step2FMMDALL.Save(openFileDialog.FileName);
			}
            MessageBox.Show("Успешно заменил в измерения метод \"Add\" на метод \"Replace\"!");
		}
	}    
}