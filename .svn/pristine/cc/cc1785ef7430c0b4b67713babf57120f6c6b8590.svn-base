using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using Krista.FM.Client.OLAPResources;
using Krista.FM.Client.DWUtils;
using Krista.FM.Client.OLAPStructures;


namespace Krista.FM.Client.OLAPAdmin
{
	internal static class GenerateNewHelper
	{
		private static frmPropGrid CreateSettingsForm(GeneratorSettings generatorSettings)
		{
			frmPropGrid frmNew = new frmPropGrid(FormClosingHandler,
				"Установите параметры генерация новых объектов многомерной базы");
			frmNew.propGrid.SelectedObject = generatorSettings;			
			return frmNew;
		}

		public static void Generate(IWin32Window owner)
		{
			GeneratorSettings generatorSettings = new GeneratorSettings();
			frmPropGrid frmNew = CreateSettingsForm(generatorSettings);
			if (frmNew.ShowDialog(owner) == DialogResult.OK)
			{
				List<DimensionInfoBase> dims;
				List<CubeInfoBase> cubes;

				DWUtils.OLAPGenerator dwUtils = new DWUtils.OLAPGenerator();
				dwUtils.ReadGlobalPackage(generatorSettings, out dims, out cubes);
				string newObjects = "\\OLAP2005 " + DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
				string dimensionsDirName = generatorSettings.NewObjectsDir + newObjects + "\\Dimensions";
				string cubesDirName = generatorSettings.NewObjectsDir + newObjects + "\\Cubes";

				DirectoryInfo dirInfo = Directory.CreateDirectory(dimensionsDirName);
				XmlWriterSettings writerSettings =
					Krista.FM.Client.OLAPResources.Utils.GetXmlWriterSettings(false);
				for (int i = 0; i < dims.Count; i++)
				{
					XmlWriter writer = XmlWriter.Create(dirInfo.FullName + "\\" +
						dims[i].Name + ".fmdimension", writerSettings);
					dims[i].ToXML(writer);
					writer.Flush();
					writer.Close();
				}

				dirInfo = Directory.CreateDirectory(cubesDirName);
				for (int i = 0; i < cubes.Count; i++)
				{
					XmlWriter writer = XmlWriter.Create(dirInfo.FullName + "\\" +
						cubes[i].Name + ".fmcube", writerSettings);
					cubes[i].ToXML(writer);
					writer.Flush();
					writer.Close();
				}
			}			
		}

		private static void FormClosingHandler(object sender, FormClosingEventArgs e)
		{
			if (sender is frmPropGrid)
			{
				frmPropGrid frm = sender as frmPropGrid;
				if (frm.DialogResult == DialogResult.OK)
				{
					DWUtils.GeneratorSettings dwSettings =
						(DWUtils.GeneratorSettings)frm.propGrid.SelectedObject;
					if (string.IsNullOrEmpty(dwSettings.NewObjectsDir) ||
						string.IsNullOrEmpty(dwSettings.PackageDir) ||
						string.IsNullOrEmpty(dwSettings.PackageFileName) ||
						string.IsNullOrEmpty(dwSettings.SemanticsFileName))
					{
						Messanger.Error("Вы задали не все параметры для генерации!");
						e.Cancel = true;
					}
				}
			}			
		}
	}
}
