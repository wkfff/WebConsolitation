using System;
using System.Collections.Generic;
using System.Text;
using Itenso.Configuration;
using Krista.FM.Client.Components;
using System.Configuration;
using System.IO;

namespace Krista.FM.Client.Common.Configuration
{
	public class UltraGridExSetting : Setting
	{
		private readonly UltraGridEx grid;
		private readonly string name;

		public UltraGridExSetting(string name, UltraGridEx grid)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (grid == null)
			{
				throw new ArgumentNullException("grid");
			}

			this.name = name;
			this.grid = grid;
		}

		public override bool HasChanged
		{
			get { return true; }
		}

		public override void Load()
		{
			object value = LoadValue("DisplayLayout", typeof (byte[]), SettingsSerializeAs.Binary);
			if (value != null)
			{
				grid.ugData.DisplayLayout.LoadFromXml(new MemoryStream((byte[]) value));
			}
		}

		public override void Save()
		{
            using (MemoryStream stream = new MemoryStream())
            {
                grid.ugData.DisplayLayout.SaveAsXml(stream);
                SaveValue("DisplayLayout", typeof(byte[]), SettingsSerializeAs.Binary, stream.GetBuffer());
            }
		}

		public override string ToString()
		{
			return String.Concat(name, " (UltraGridEx)");
		}
	}
}
