using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	/// <summary>
	/// Список закачек данных.
	/// </summary>
	public class DataPumpsListUI : BaseViewObj
	{
		public DataPumpsListUI(string key)
			: base(key)
		{
			Caption = "Закачки данных";	
		}

		public override Icon Icon
		{
			get { return Icon.FromHandle(Properties.Resources.pump_DataPump_16.GetHicon()); }
		}

		protected override void SetViewCtrl()
		{
			fViewCtrl = new DataPumpsListView();
			fViewCtrl.Text = Caption;
		}
	}
}
