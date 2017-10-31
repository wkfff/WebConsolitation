using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;

namespace Krista.FM.Client.OLAPResources
{
	public class StringDescriptor : PropertyDescriptor
	{
		private string name;

		public StringDescriptor(string _name)
			: base(_name, null)
		{
			name = _name;
		}

		public override string Name
		{
			get { return name; }
		}

		public override object GetValue(object component)
		{
			string value;
			((Dictionary<string, string>)component).TryGetValue(name, out value);
			return value;
		}

		public override void SetValue(object component, object value)
		{
			((Dictionary<string, string>)component)[name] = (string)value;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return typeof(Dictionary<string, string>); }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return typeof(string); }
		}

		public override void ResetValue(object component)
		{
			//propValue = string.Empty;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}

	public class ParamsConverter : ExpandableObjectConverter
	{
		public override PropertyDescriptorCollection GetProperties(
			ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			List<StringDescriptor> propList = new List<StringDescriptor>();
			foreach (KeyValuePair<string, string> item in (Dictionary<string, string>)value)
			{
				propList.Add(new StringDescriptor(item.Key));
			}
			StringDescriptor[] propStrings = new StringDescriptor[propList.Count];
			propList.CopyTo(propStrings);

			return new PropertyDescriptorCollection(propStrings, true);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
		{
			if (t == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, t);
		}

		public override object ConvertFrom(
			ITypeDescriptorContext context, CultureInfo info, object value)
		{
			return Utils.StringToDictionary((string)value);
		}

		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destType)
		{
			return Utils.DictionaryToString((Dictionary<string, string>)value);
		}
	}

	public class DetailedVersionDescriptor : PropertyDescriptor
	{
		private string name;
		private string version;

		/// <summary>
		/// Вычленяет из строки вида "1.0.0.0   (2007.02.19 - tartushkin)" номер версии
		/// (в данном случае - "1.0.0.0")
		/// </summary>
		/// <param name="name">имя свойства</param>
		/// <returns>строковое представление версии</returns>
		private string GetVersionFromName(string name)
		{
			int bracketIndex = name.IndexOf("(");
			if (bracketIndex > 0)
			{
				return name.Substring(0, bracketIndex).Trim();
			}
			else
			{
				return name;
			}
		}

		public DetailedVersionDescriptor(string _name, Attribute[] attrs)
			: base(_name, attrs)
		{
			name = _name;
			version = GetVersionFromName(name);
		}

		public override string Name
		{
			get { return name; }
		}

		public override object GetValue(object component)
		{
			DetailedVersionInfo detailedVersion = ((Versions)component).GetVersion(version);
			return detailedVersion;
		}

		public override void SetValue(object component, object value)
		{
			DetailedVersionInfo detailedVersion = ((Versions)component).GetVersion(version);
			if (detailedVersion != null)
			{
				detailedVersion.Comment = (string)value;
			}
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return typeof(Versions); }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return typeof(DetailedVersionInfo); }			
		}

		public override void ResetValue(object component)
		{
			//propValue = string.Empty;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}

	public class VersionsConverter : ExpandableObjectConverter
	{
		public override PropertyDescriptorCollection GetProperties(
			ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			List<DetailedVersionDescriptor> propList = new List<DetailedVersionDescriptor>();
			Versions versions = (Versions)value;
			
			for (int i = 0; i < versions.VersionsList.Count; i++)
			{
				Attribute[] attrs = new Attribute[1] { 
					new EditorAttribute(typeof(DetailedVersionInfoTextEditor), typeof(UITypeEditor))
					};
				//propList.Add(
				//    new DetailedVersionDescriptor(versions.VersionsList.Values[i].Version.ToString(), attrs));
				propList.Add(
				    new DetailedVersionDescriptor(versions.VersionsList.Values[i].ToString(), attrs));
			}
			DetailedVersionDescriptor[] propStrings = new DetailedVersionDescriptor[propList.Count];
			propList.CopyTo(propStrings);

			return new PropertyDescriptorCollection(propStrings, true);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
		{
			if (t == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, t);
		}

		public override object ConvertFrom(
			ITypeDescriptorContext context, CultureInfo info, object value)
		{	
			return value;
		}

		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destType)
		{			
			return value;
		}
	}	

	public class DetailedVersionConverter : ExpandableObjectConverter
	{
		protected DetailedVersionInfo GetDetailedVersionInfo(ITypeDescriptorContext context, object value)
		{
			//return ((Versions)context.Instance).GetVersion(context.PropertyDescriptor.Name);
			return (DetailedVersionInfo)value;
		}

		public override PropertyDescriptorCollection GetProperties(
			ITypeDescriptorContext context, object value, Attribute[] attributes)
		{	
			return TypeDescriptor.GetProperties(typeof(DetailedVersionInfo));
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
		{
			if (t == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, t);
		}

		public override object ConvertFrom(
			ITypeDescriptorContext context, CultureInfo info, object value)
		{			
			return value;
		}

		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destType)
		{
			DetailedVersionInfo detailedVersionInfo = GetDetailedVersionInfo(context, value);
			if (detailedVersionInfo != null)
			{
				return detailedVersionInfo.Comment;
			}
			else
			{
				return string.Empty;
			}
		}
	}

	public class TextEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		protected Button CreateButton(Panel panel, string text, DialogResult dialogResult, int left)
		{
			Button button = new Button();
			button.Parent = panel;
			button.Width = 80;
			button.Height = 36;
			button.Left = left;
			button.Top = 2;
			button.Anchor = AnchorStyles.Right;
			button.DialogResult = dialogResult;
			button.Text = text;
			return button;
		}

		protected virtual string GetText(object value)
		{
			return (string)value;
		}

		protected virtual object SetText(string text)
		{
			return text;
		}

		public override object EditValue(
			ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Form textForm = new Form();
			textForm.Text = context.PropertyDescriptor.DisplayName;
			textForm.Width = 640;
			textForm.Height = 480;
			textForm.ControlBox = true;
			textForm.StartPosition = FormStartPosition.CenterParent;

			Panel panel = new Panel();
			panel.Width = textForm.ClientSize.Width;
			panel.Height = 39;
			panel.Parent = textForm;
			panel.Dock = DockStyle.Bottom;

			Button okBtn = CreateButton(
				panel, "ОК", DialogResult.OK, panel.ClientSize.Width - 80 * 2);
			Button cancelBtn = CreateButton(
				panel, "Отмена", DialogResult.Cancel, panel.ClientSize.Width - 80 * 1);
			textForm.AcceptButton = okBtn;
			textForm.CancelButton = cancelBtn;

			TextBox textBox = new TextBox();
			textBox.Parent = textForm;
			textBox.Dock = DockStyle.Fill;
			textBox.Multiline = true;
			textBox.WordWrap = true;
			textBox.Text = GetText(value);

			
			textBox.ReadOnly =
				((ReadOnlyAttribute)context.PropertyDescriptor.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly;
			okBtn.Enabled = !textBox.ReadOnly;

			IWindowsFormsEditorService edSvc =
				(IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if (edSvc.ShowDialog(textForm) == DialogResult.OK)
			{
				return SetText(textBox.Text);				
			}
			return base.EditValue(context, provider, value);
		}
	}

	public class DictionaryTextEditor : TextEditor
	{
		protected override string GetText(object value)
		{
			return Utils.DictionaryToString((Dictionary<string, string>)value);
		}

		protected override object SetText(string text)
		{	
			return Utils.StringToDictionary(text);
		}
	}

	public class DetailedVersionInfoTextEditor : TextEditor
	{
		protected override string GetText(object value)
		{
			return ((DetailedVersionInfo)value).Comment;
		}

		protected override object SetText(string text)
		{
			return text;
		}
	}

	public class PackageFileNameEditor : FileNameEditor
	{
		public PackageFileNameEditor()
		{ }

		protected override void InitializeDialog(OpenFileDialog openFileDialog)
		{
			base.InitializeDialog(openFileDialog);
			openFileDialog.Title = "Выберите файл";
			openFileDialog.AddExtension = false;
			openFileDialog.DefaultExt = "emptypack";
			openFileDialog.Filter = "Файлы XML (*.xml)|*.xml|" +
				"Пустые пакеты (*.emptypack)|*.emptypack|" +
				"Конфигурационные пакеты (*.configpack)|*.configpack|" +
				"Автономные пакеты (*.fullpack)|*.fullpack|" +
				"Все файлы (*.*)|*.*";
			openFileDialog.FilterIndex = 2;
			openFileDialog.CheckFileExists = false;
		}
	}

	public class XmlFileNameEditor : FileNameEditor
	{
		public XmlFileNameEditor()
		{ }

		protected override void InitializeDialog(OpenFileDialog openFileDialog)
		{
			base.InitializeDialog(openFileDialog);
			openFileDialog.Title = "Выберите файл";
			openFileDialog.AddExtension = false;
			openFileDialog.DefaultExt = "xml";
			openFileDialog.Filter = "Файлы XML (*.xml)|*.xml|Все файлы (*.*)|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.CheckFileExists = true;
		}
	}

	public class IniFileNameEditor : FileNameEditor
	{
		public IniFileNameEditor()
		{ }

		protected override void InitializeDialog(OpenFileDialog openFileDialog)
		{
			base.InitializeDialog(openFileDialog);
			openFileDialog.Title = "Выберите файл";
			openFileDialog.AddExtension = false;
			openFileDialog.DefaultExt = "ini";
			openFileDialog.Filter = "Файлы ini (*.ini)|*.ini|Все файлы (*.*)|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.CheckFileExists = true;
		}
	}

	//public class FolderNameEditor2 : FolderNameEditor
	//{
	//    override 
	//}
}