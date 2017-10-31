using System;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.Workplace.Services
{
	public class ComboButtonTool
	{
		private ComboBoxTool comboboxtool;
		private String key;
		private ButtonTool buttontool;
		
		public ComboBoxTool Comboboxtool
		{
			get { return comboboxtool; }
		}

		public ButtonTool Buttontool
		{
			get { return buttontool; }
		}

		public string Key
		{
			get { return key; }
		}

		public ComboButtonTool(String key)
		{
			this.key = key;
			comboboxtool = new ComboBoxTool(key);
			buttontool = new ButtonTool(String.Format("button_{0}", key));
			comboboxtool.Tag = buttontool;
			buttontool.Tag = comboboxtool;
			
		//	comboboxtool.ToolValueChanged += new ToolEventHandler(comboboxtool_ToolValueChanged);
		}

		
		///не здесь!
		/*void comboboxtool_ToolValueChanged(object sender, ToolEventArgs e)
		{
			if ((comboboxtool.Value == null) || (comboboxtool.Value.ToString() == ""))
				buttontool.SharedProps.Enabled = false;

			else
			{
				String s = comboboxtool.Value.ToString().TrimEnd('%');
				Double d;
				try
				{
					d = Convert.ToDouble(s) / 100;
					buttontool.SharedProps.Enabled = true;
				}
				catch
				{
					buttontool.SharedProps.Enabled = false;
				}
			}
		}*/
	}

	public static class CommandService
	{
		/// <summary>
		/// Связывает элемент управления ctrl с командой cmd.
		/// </summary>
		public static void AttachCommandToControl(AbstractCommand cmd, Control ctrl)
		{
			ctrl.Tag = cmd;
			ctrl.Click += OnCommandToolClick;
		}

		/// <summary>
		/// Обработчик срабатывания команды.
		/// </summary>
		private static void OnCommandToolClick(object sender, EventArgs e)
		{
			// Обычная кнопка на форме или панели
			if (sender is Control && ((Control)sender).Tag != null)
			{
				((AbstractCommand)((Control)sender).Tag).Run();
			}

			// Компонент тулбара или меню
			if (sender is ToolBase && ((ToolBase)sender).SharedProps.Tag != null)
			{
				((AbstractCommand)((ToolBase)sender).SharedProps.Tag).Run();
			}
		}

		public static ButtonTool AttachToolbarTool(AbstractCommand command, UltraToolbar toolbar, string parentToolName)
		{
			UltraToolbarsManager utm = toolbar.ToolbarsManager;

			if (utm.Tools.Exists(command.Key))
				return (ButtonTool)utm.Tools[command.Key];

			ButtonTool tool = new ButtonTool(command.Key);
			tool.SharedProps.Caption = command.Caption;
			tool.SharedProps.Shortcut = command.Shortcut;
			tool.SharedProps.Tag = command;
			Icon icon = ResourceService.GetIcon(command.IconKey);
			if (icon != null)
			{
				tool.CustomizedImage = new Icon(icon, 16, 16).ToBitmap();
			}
			utm.Tools.Add(tool);
			if (!String.IsNullOrEmpty(parentToolName) && utm.Tools.Exists(parentToolName))
				((PopupMenuTool)utm.Tools[parentToolName]).Tools.AddTool(tool.Key);
			else
				toolbar.Tools.AddTool(tool.Key);

			tool.ToolClick += OnCommandToolClick;
			return tool;
		}

		public static ComboButtonTool AttachToolbarComboButtonTool(AbstractCommand command, UltraToolbar toolbar)
		{
			UltraToolbarsManager utm = toolbar.ToolbarsManager;

			if (utm.Tools.Exists(command.Key))
				return (ComboButtonTool)(utm.Tools[command.Key].Tag);
			
			ComboButtonTool tool = new ComboButtonTool(command.Key);

			tool.Comboboxtool.SharedProps.Caption = command.Caption;
			tool.Buttontool.SharedProps.Caption = command.Caption;
			tool.Buttontool.SharedProps.Shortcut = command.Shortcut;
			tool.Buttontool.SharedProps.Tag = command;
			Icon icon = ResourceService.GetIcon(command.IconKey);
			if (icon != null)
			{
				tool.Buttontool.CustomizedImage = new Icon(icon, 16, 16).ToBitmap();
			}
			utm.Tools.Add(tool.Comboboxtool);
			utm.Tools.Add(tool.Buttontool);
			tool.Comboboxtool.Tag = tool;
			//tool.Buttontool.Tag = ;

			toolbar.Tools.AddTool(tool.Comboboxtool.Key);
			toolbar.Tools.AddTool(tool.Buttontool.Key);
			
			tool.Buttontool.ToolClick += OnCommandToolClick;
			return tool;
		}

        public static T AttachToolbarTool<T>(AbstractCommand command, UltraToolbar toolbar) where T : ToolBase
        {
            UltraToolbarsManager utm = toolbar.ToolbarsManager;

            if (utm.Tools.Exists(command.Key))
                return (T)utm.Tools[command.Key].Tag;

            var tool = (T)Activator.CreateInstance(typeof(T), command.Key);
            tool.SharedProps.Caption = command.Caption;
            tool.SharedProps.Shortcut = command.Shortcut;
            tool.SharedProps.Tag = command;
            Icon icon = ResourceService.GetIcon(command.IconKey);
            if (icon != null)
            {
                tool.CustomizedImage = new Icon(icon, 16, 16).ToBitmap();
            }
            utm.Tools.Add(tool);
            tool.Tag = tool;
            toolbar.Tools.AddTool(tool.Key);
            tool.ToolClick += OnCommandToolClick;
            return tool;
        }

		public static ComboButtonTool GetComboButtonByKey(UltraToolbarsManager utm, String key)
		{
			if (utm.Tools.Exists(key))
				return (ComboButtonTool)(utm.Tools[key].Tag);
			else return null; 
		}

		public static StateButtonTool AttachToolbarCheckedTool(AbstractCommand command, UltraToolbar toolbar, string parentToolName)
		{
			UltraToolbarsManager utm = toolbar.ToolbarsManager;

			if (utm.Tools.Exists(command.Key))
				return (StateButtonTool)utm.Tools[command.Key];

			StateButtonTool tool = new StateButtonTool(command.Key);
			tool.SharedProps.Caption = command.Caption;
			tool.SharedProps.Shortcut = command.Shortcut;
			tool.SharedProps.Tag = command;
			Icon icon = ResourceService.GetIcon(command.IconKey);
			if (icon != null)
			{
				tool.CustomizedImage = new Icon(icon, 16, 16).ToBitmap();
			}
			utm.Tools.Add(tool);
			if (!String.IsNullOrEmpty(parentToolName) && utm.Tools.Exists(parentToolName))
				((PopupMenuTool)utm.Tools[parentToolName]).Tools.AddTool(tool.Key);
			else
				toolbar.Tools.AddTool(tool.Key);

			tool.ToolClick += OnCommandToolClick;
			return tool;
		}

		public static ButtonTool AttachToolbarTool(AbstractCommand command, UltraToolbar toolbar)
		{
			return AttachToolbarTool(command, toolbar, String.Empty);
		}
	}
}
