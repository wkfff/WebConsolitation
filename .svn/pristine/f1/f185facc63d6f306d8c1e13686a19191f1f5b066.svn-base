using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace PackageMaker2000
{
	public class IdentificationInfo
	{
		string safeName;
		Guid guid;
		int hash;

		private void Init(string _safeName, Guid _guid, int _hash)
		{
			safeName = _safeName;
			guid = _guid;
			hash = _hash;
		}


		private IdentificationInfo()
		{ }

		public IdentificationInfo(string _safeName, string _guid)
		{
			Init(_safeName, new Guid(_guid), _guid.GetHashCode()); 			
		}

		public IdentificationInfo(string _safeName, Guid _guid)
		{
			Init(_safeName, _guid, _guid.ToString().GetHashCode());			
		}

		public IdentificationInfo(string _safeName, Guid _guid, int _hash)
		{
			Init(_safeName, _guid, _hash);
		}

		public string SafeName
		{
			get { return safeName; }
		}

		public Guid GUID
		{
			get { return guid; }
			set { guid = value; }
		}

		public int Hash
		{
			get { return hash; }
		}

		public Guid ID
		{
			get { return GUID; }
		}

		public string StringID
		{
			get
			{
				if (ID.Equals(Guid.Empty))
					return string.Empty;
				else
					return ID.ToString();
			}
		}

		public static IdentificationInfo ReadFromXML(XPathNavigator objectNavigator)
		{
			string name = objectNavigator.GetAttribute("name", "");
			string id = objectNavigator.GetAttribute("id", "");
			return new IdentificationInfo(name, id);
		}
	}

	public class IdentificationList : List<IdentificationInfo>
	{
		public bool Contains(string id)
		{	
			for (int i = 0; i < base.Count; i++)
			{
				if (base[i].StringID.Equals(id, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		//public IdentificationInfo IdInfoByID(string id)
		//{			
		//    return new IdentificationInfo(this.Values[this.IndexOfKey(id)], id);
		//}
	}
}
