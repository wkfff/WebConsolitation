using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
using System.IO;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.OLAPResources
{
    public class VSSInfo
    {
        public Krista.FM.ServerLibrary.VSSFileStatus VSSState =
            Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
        public string VSSSpec = string.Empty;
        public string VSSCheckUser = string.Empty;
        public bool NewFile = false;

    }	

	public enum VSSState
	{
		unknown = -1,
		checkedOut = 0,
		notPresent = 1,
		pinned = 2,
	}

	public delegate void ToXMLDelegate(XmlWriter xmlWriter);

	public static class Utils
	{
		public const string defValueDelimiter = "=";
		public const string defPairDelimiter = "; ";

		public static string DictionaryToString(Dictionary<string, string> connectionParams,
			string valueDelimiter, string pairDelimiter)
		{
			StringBuilder returnValue = new StringBuilder();
			foreach (KeyValuePair<string, string> item in connectionParams)
			{
				returnValue.Append(item.Key + valueDelimiter + item.Value + pairDelimiter);
			}
			return returnValue.ToString(0, returnValue.Length - pairDelimiter.Length);
		}

		public static string DictionaryToString(Dictionary<string, string> connectionParams)
		{
			return DictionaryToString(connectionParams, defValueDelimiter, defPairDelimiter);			
		}

		public static Dictionary<string, string> StringToDictionary(string connectionString,
			string valueDelimiter, string pairDelimiter)
		{
			Dictionary<string, string> returnValue = new Dictionary<string, string>();
			try
			{
				string[] connectionArray = connectionString.Split(
					new string[] { pairDelimiter }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < connectionArray.Length; i++)
				{
					string[] parameter = connectionArray[i].Split(
						new string[] { valueDelimiter }, StringSplitOptions.RemoveEmptyEntries);
					if (parameter.Length == 2 && !string.IsNullOrEmpty(parameter[0]))
					{
						returnValue.Add(parameter[0].Trim(), parameter[1].Trim());
					}
				}
			}
			catch
			{ }
			return returnValue;
		}

		public static Dictionary<string, string> StringToDictionary(string connectionString)
		{
			return StringToDictionary(connectionString, defValueDelimiter, defPairDelimiter.Trim());
		}

		public static void InsertControl(Control Parent, Control Child)
		{
			Child.Parent = Parent;
			Child.Dock = DockStyle.Fill;
		}

		public static XmlWriterSettings GetXmlWriterSettings(bool omitXmlDeclaration)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = Encoding.UTF8;
			xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "\t";
			xmlWriterSettings.NewLineChars = "\r\n";
			xmlWriterSettings.NewLineOnAttributes = true;

			xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
			return xmlWriterSettings;
		}

		public static string[] GetFilesByMask(string directoryName, string mask)
		{
			return Directory.GetFiles(directoryName, mask, SearchOption.AllDirectories);
		}

		public static Image GetResourceImage16(string name)
		{
			return OLAPResources.ImageListSmall.Images[name];
		}

		public static Image GetResourceImage32(string name)
		{
			return OLAPResources.ImageListMain.Images[name];
		}

		public static string PackageRepositoryName
		{
			get { return "Пакеты"; }
		}

		public static string VSSRepositoryName
		{
			get { return "VSS"; }
		}

		public static string VSSRepositoryINI
		{
			get { return @"\\hanut\finmon\SourceFM\srcsafe.ini"; }
			//get { return VSSRepositoryTestINI; }
		}		

		public static string VSSRepositoryRoot
		{
			//get { return "$/dotNet/Repository/OLAP2005"; }
			get { return "$/"; }
		}

		public static string VSSRepositoryRootLocal
		{	
			get { return @"X:\"; }
			//get { return VSSRepositoryTestRootLocal; }
		}

		public static string VSSRepositoryTestINI
		{
			get { return @"X:\TestVSS\srcsafe.ini"; }
		}

		public static string VSSRepositoryTestRootLocal
		{			
			get { return @"T:\"; }
		}

		public static string VSSRepositoryWorkingFolder
		{
			get { return @"dotNet\Repository\OLAP2005"; }
		}

		public static string TempDir
		{
			get { return string.Format(@"{0}\OLAPAdmin", Path.GetTempPath()); }
		}		

		public static string PathToVSSPath(string filePath)
		{
			//return "$/" + filePath.Substring(3).Replace(@"\", @"/");
			return filePath.Substring(3).Replace(@"\", @"/");
		}

		public static void SetVssProperties(string fileName, VSSInfo vssInfo, IVSSFacade vss)
		{
			vssInfo.VSSSpec = PathToVSSPath(fileName);
			vssInfo.NewFile = !vss.Find(vssInfo.VSSSpec);
			if (!vssInfo.NewFile)
			{
				vssInfo.VSSState = vss.IsCheckedOut(vssInfo.VSSSpec);
				if (vssInfo.VSSState == Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_CHECKEDOUT)
				{
					vssInfo.VSSCheckUser = vss.GetCheckOutUser(vssInfo.VSSSpec);
				}
			}
		}

		//public static VSSState GetVssState(VSSItem item)
		//{			
		//    if (item == null)
		//    {
		//        return VSSState. notPresent;
		//    }
		//    else
		//    {
		//        if (item.IsCheckedOut > 0)
		//        {
		//            return VSSState.checkedOut;
		//        }
		//        else
		//        {
		//            return VSSState.unknown;
		//        }
		//    }
		//}

		public static DialogResult OpenDialog(string initialDirectory)
		{
			OLAPResources.OpenFileDialog.InitialDirectory = initialDirectory;
			return OLAPResources.OpenFileDialog.ShowDialog();
		}

		public static string OpenDialogFileName
		{
			get { return OLAPResources.OpenFileDialog.FileName; }
		}

		public static XmlNode ToXmlNode(ToXMLDelegate toXML)
		{
			StringBuilder strBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(strBuilder, GetXmlWriterSettings(true));
			try
			{
				xmlWriter.WriteStartDocument();
				toXML(xmlWriter);
				xmlWriter.WriteEndDocument();
			}
			finally
			{
				xmlWriter.Flush();
				xmlWriter.Close();
			}
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(strBuilder.ToString());
			return doc.FirstChild;
		}		
	}
}