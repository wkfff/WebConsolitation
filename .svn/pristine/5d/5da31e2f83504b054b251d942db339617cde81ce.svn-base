using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.OLAPResources
{
	public static class Messanger
	{
		public static DialogResult Message(
			string msgText, MessageBoxIcon MBIcon, MessageBoxButtons buttons)
		{
			return MessageBox.Show(msgText, Application.ProductName, buttons, MBIcon);
		}

		public static DialogResult Message(string msgText, MessageBoxIcon MBIcon)
		{
			return Message(msgText, MBIcon, MessageBoxButtons.OK);
		}

		public static DialogResult Error(string msgText)
		{
			return Message(msgText, MessageBoxIcon.Error);
		}

		public static DialogResult Warning(string msgText)
		{
			return Message(msgText, MessageBoxIcon.Warning);
		}

		public static DialogResult Info(string msgText)
		{
			return Message(msgText, MessageBoxIcon.Information);
		}

		public static DialogResult Question(string msgText)
		{
			return Message(msgText, MessageBoxIcon.Question, MessageBoxButtons.YesNo);
		}
	}	
}
