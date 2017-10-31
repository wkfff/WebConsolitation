using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Drawing.Design;

namespace Krista.FM.Client.OLAPResources
{
	public class VersionComparer : Comparer<Version>
	{
		public override int Compare(Version x, Version y)
		{
			//так как нам необходимо сортировать версии по убыванию, то возвращаем "-".
			return -x.CompareTo(y);
		}
	}

	public enum VersionPart
	{
		Major = 0,
		Minor = 1,
		Build = 2,
		Revision = 3,
	}

	[TypeConverter(typeof(VersionsConverter))]
	public class Versions
	{	
		protected SortedList<Version, DetailedVersionInfo> versionsList =
			new SortedList<Version, DetailedVersionInfo>(new VersionComparer());

		public void IncrementVersion(VersionPart versionPart,
			string _author, string _comment, DateTime _dateTime)
		{
			Version lastVersion;
			DetailedVersionInfo lastDetailedVersion = GetLastVersion();
			if (lastDetailedVersion != null)
			{
				lastVersion = lastDetailedVersion.Version;
			}
			else
			{
				lastVersion = new Version(1, 0, 0 ,0);
			}
			
			Version newVersion;
			switch (versionPart)
			{
				case VersionPart.Major:
					newVersion = new Version(
						lastVersion.Major + 1, 0, 0, 0);
					break;
				case VersionPart.Minor:
					newVersion = new Version(
						lastVersion.Major, lastVersion.Minor + 1, 0, 0);
					break;
				case VersionPart.Build:
					newVersion = new Version(
						lastVersion.Major, lastVersion.Minor, lastVersion.Build + 1, 0);
					break;
				case VersionPart.Revision:
				default:
					newVersion = new Version(
						lastVersion.Major, lastVersion.Minor, lastVersion.Build, lastVersion.Revision + 1);
					break;				
			}
			AddVersion(newVersion, _author, _comment, _dateTime);
		}

		public void AddVersion(DetailedVersionInfo detailVersionInfo)
		{
			versionsList.Add(detailVersionInfo.Version, detailVersionInfo);
		}

		public void AddVersion(int major, int minor, int build, int revision,
			string _author, string _comment, DateTime _dateTime)
		{
			AddVersion(new DetailedVersionInfo(
				major, minor, build, revision, _author, _comment, _dateTime));
		}

		public void AddVersion(Version _version, string _author, string _comment, DateTime _dateTime)
		{
			AddVersion(new DetailedVersionInfo(_version, _author, _comment, _dateTime));
		}

		public DetailedVersionInfo GetVersion(string version)
		{
			string[] versionArray =
				version.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			int major = 0;
			int minor = 0;
			int build = 0;
			int revision = 0;

			if (versionArray.Length > 0)
			{
				if (versionArray.Length > 0) { major = Convert.ToInt32(versionArray[0]); }
				if (versionArray.Length > 1) { minor = Convert.ToInt32(versionArray[1]); }
				if (versionArray.Length > 2) { build = Convert.ToInt32(versionArray[2]); }
				if (versionArray.Length > 3) { revision = Convert.ToInt32(versionArray[3]); }
				return GetVersion(major, minor, build, revision);
			}
			return null;
		}

		public DetailedVersionInfo GetVersion(int major, int minor, int build, int revision)
		{
			Version version = new Version(major, minor, build, revision);
			return GetVersion(version);
		}

		public DetailedVersionInfo GetVersion(Version version)
		{
			try
			{
				return versionsList[version];
			}
			catch
			{
				return null;
			}			
		}

		public DetailedVersionInfo GetLastVersion()
		{
			if (versionsList.Count > 0)
			{
				return versionsList.Values[0];
			}
			else
			{
				return null;
			}			
		}

		public SortedList<Version, DetailedVersionInfo> VersionsList
		{
			get { return versionsList; }
		}

		public DetailedVersionInfo this[int index]
		{
			get { return versionsList.Values[index]; }
			set { versionsList.Values[index] = value; }
		}

		public virtual void WriteToXML(XmlWriter xmlWriter)
		{
			try
			{
				xmlWriter.WriteStartElement("versions");
				for (int i = 0; i < versionsList.Count; i++)
				{
					versionsList.Values[i].WriteToXML(xmlWriter);
				}
				xmlWriter.WriteEndElement();//versions
			}
			finally
			{ }
		}

		public static Versions ReadFromXML(XPathNavigator navigator)
		{
			try
			{
				Versions versions = new Versions();
				XPathNodeIterator nodes = navigator.Select(".//detailedversioninfo");
				while (nodes.MoveNext())
				{
					versions.AddVersion(DetailedVersionInfo.ReadFromXML(nodes.Current));
				}				
				return versions;
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				return null;
			}
		}
	}
	
	[TypeConverter(typeof(DetailedVersionConverter))]
	public class DetailedVersionInfo
	{
		protected Version innerVersion;
		protected string author = string.Empty;
		protected string comment = string.Empty;
		protected DateTime dateTime;

		protected void Init(int major, int minor, int build, int revision,
			string _author, string _comment, DateTime _dateTime)
		{
			Init(new Version(major, minor, build, revision), _author, _comment, _dateTime);
		}

		protected void Init(Version _version, string _author, string _comment, DateTime _dateTime)
		{
			innerVersion = _version;
			author = _author;
			comment = _comment;
			dateTime = _dateTime;
		}

		public DetailedVersionInfo(Version _version, string _author, string _comment)
		{
			Init(_version, _author, _comment, DateTime.Now);
		}

		public DetailedVersionInfo(Version _version, string _author, string _comment, DateTime _dateTime)
		{
			Init(_version, _author, _comment, _dateTime);
		}

		public DetailedVersionInfo(int major, int minor, int build, int revision,
			string _author, string _comment)
		{
			Init(major, minor, build, revision, _author, _comment, DateTime.Now);
		}

		public DetailedVersionInfo(int major, int minor, int build, int revision,
			string _author, string _comment, DateTime _dateTime)
		{
			Init(major, minor, build, revision, _author, _comment, _dateTime);
		}

		public virtual void WriteToXML(XmlWriter xmlWriter)
		{
			try
			{	
				xmlWriter.WriteStartElement("detailedversioninfo");
				xmlWriter.WriteAttributeString("author", this.author);
				xmlWriter.WriteAttributeString("datetime", this.dateTime.ToString());
				VersionUtils.VersionToXML(xmlWriter, this.Version);
				xmlWriter.WriteElementString("comment", this.comment);

				xmlWriter.WriteEndElement();//detailedversioninfo
			}
			finally
			{ }
		}

		public static DetailedVersionInfo ReadFromXML(XPathNavigator navigator)
		{
			try
			{
				string author = navigator.GetAttribute("author", string.Empty);
				DateTime dateTime = Convert.ToDateTime(navigator.GetAttribute("datetime", string.Empty));
				Version version = VersionUtils.ReadVersionFromXML(navigator.SelectSingleNode("version"));
				string comment = navigator.SelectSingleNode("comment").Value;
				return new DetailedVersionInfo(version, author, comment, dateTime);
			}
			catch
			{
				return null;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}   ({1} - {2})",
				innerVersion.ToString(), dateTime.ToString("yyyy.MM.dd HH:mm"), author);
		}

		[DisplayName("Версия")]
		public Version Version
		{
			get { return innerVersion; }
		}

		[DisplayName("Автор")]
		public string Author
		{
			get { return author; }
		}

		[DisplayName("Комментарий"), Editor(typeof(TextEditor), typeof(UITypeEditor))]
		public string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		[DisplayName("Время создания")]
		public DateTime DateTime
		{
			get { return dateTime; }
		}
	}

	public static class VersionUtils
	{
		public static Version ReadVersionFromXML(XPathNavigator navigator)
		{
			int major = Convert.ToInt32(navigator.GetAttribute("major", ""));
			int minor = Convert.ToInt32(navigator.GetAttribute("minor", ""));
			int build = Convert.ToInt32(navigator.GetAttribute("build", ""));
			int revision = Convert.ToInt32(navigator.GetAttribute("revision", ""));
			return new Version(major, minor, build, revision);
		}

		public static void VersionToXML(XmlWriter xmlWriter, Version version)
		{
			xmlWriter.WriteStartElement("version");
			xmlWriter.WriteAttributeString("major", version.Major.ToString());
			xmlWriter.WriteAttributeString("minor", version.Minor.ToString());
			xmlWriter.WriteAttributeString("build", version.Build.ToString());
			xmlWriter.WriteAttributeString("revision", version.Revision.ToString());
			xmlWriter.WriteEndElement();
		}
	}
}