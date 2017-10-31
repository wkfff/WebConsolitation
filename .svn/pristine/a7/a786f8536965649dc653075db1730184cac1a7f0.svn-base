using System;
using System.Collections.Generic;
using System.Text;
using Itenso.Configuration;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.Common.Configuration
{
	public class ViewSettings : ApplicationSettings
	{
		private readonly IPersistenceSupport view;

		public ViewSettings(IPersistenceSupport view)
			: this(view, view.GetType().Name)
		{
		}

		public ViewSettings(IPersistenceSupport view, string settingsKey)
			: base( settingsKey )
		{
			if (view == null)
			{
				throw new ArgumentNullException("view");
			}

			this.view = view;

			// subscribe to parent form's events
			this.view.LoadState += ControlHandleLoadState;
			this.view.SaveState += ControlHandleSaveState;
			SaveOnClose = true;
		}

		private void ControlHandleSaveState(object sender, EventArgs e)
		{
			if (SaveOnClose == false)
			{
				return;
			}
			Save();
		}

		private void ControlHandleLoadState(object sender, EventArgs e)
		{
			Load();
		}

		private bool saveOnClose;
		public bool SaveOnClose
		{
			get { return saveOnClose; }
			set { saveOnClose = value; }
		}
	}
}
