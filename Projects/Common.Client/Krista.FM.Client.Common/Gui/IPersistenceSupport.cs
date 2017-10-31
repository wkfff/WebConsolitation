using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.Common.Gui
{
	/// <summary>
	/// Поддержка сохранения состояния.
	/// </summary>
	public interface IPersistenceSupport
	{
		void SavePersistence();
		void LoadPersistence();

		event EventHandler LoadState;
		event EventHandler SaveState;
	}
}
