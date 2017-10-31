using System;
using Itenso.Configuration;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.Common.Configuration
{
	public class PersistenceSupportComponentSettings : ApplicationSettings
	{
		private readonly IPersistenceSupport control;

		public PersistenceSupportComponentSettings(IPersistenceSupport control)
			: this( control, control.GetType().Name )
		{
			
		}

		public PersistenceSupportComponentSettings(IPersistenceSupport control, string settingsKey)
			: base( settingsKey )
		{
			if ( control == null )
			{
				throw new ArgumentNullException( "control" );
			}

			this.control = control;
			this.control.LoadState += ControlHandleLoadState;
			this.control.SaveState += ControlHandleSaveState;
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
