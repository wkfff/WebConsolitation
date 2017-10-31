using System;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.ViewObjects.BaseViewObject
{
	public abstract class BaseViewContentPane : AbstractViewContent, IPersistenceSupport
    {
		#region IPersistenceSupport Members

		public void SavePersistence()
		{
			if (Control is IPersistenceSupport)
			{
				((IPersistenceSupport)Control).SavePersistence();
			}

			if (SaveState != null)
			{
				SaveState(this, new EventArgs());
			}
		}

		public void LoadPersistence()
		{
			if (Control is IPersistenceSupport)
			{
				((IPersistenceSupport)Control).LoadPersistence();
			}

			if (LoadState != null)
			{
				LoadState(this, new EventArgs());
			}
		}

		public event EventHandler LoadState;

		public event EventHandler SaveState;

		#endregion
	}
}
