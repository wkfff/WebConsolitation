// -- FILE ------------------------------------------------------------------
// name       : FormSettings.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace Itenso.Configuration
{

	// ------------------------------------------------------------------------
	public class FormSettings : ApplicationSettings
	{

		// ----------------------------------------------------------------------
		public FormSettings( Form form ) :
			this( form, form.GetType().Name )
		{
		} // FormSettings

		// ----------------------------------------------------------------------
		public FormSettings( Form form, string settingsKey ) :
			base( settingsKey )
		{
			if ( form == null )
			{
				throw new ArgumentNullException( "form" );
			}

			this.form = form;
			UseLocation = true;
			UseSize = true;
			UseWindowState = true;
			SaveOnClose = true;

			// settings 
			this.topSetting = CreateSetting( "Window.Top", "Top" );
			this.leftSetting = CreateSetting( "Window.Left", "Left" );
			this.widthSetting = CreateSetting( "Window.Width", "Width" );
			this.heightSetting = CreateSetting( "Window.Height", "Height" );
			this.stateSetting = CreateSetting( "Window.WindowState", "WindowState" );

			// subscribe to parent form's events
			this.form.Load += new EventHandler( FormLoad );
			this.form.Closing += new CancelEventHandler( FormClosing );
		} // FormSettings

		// ----------------------------------------------------------------------
		public Form Form
		{
			get { return this.form; }
		} // Form

		// ----------------------------------------------------------------------
		public ISetting TopSetting
		{
			get { return this.topSetting; }
		} // TopSetting

		// ----------------------------------------------------------------------
		public ISetting LeftSetting
		{
			get { return this.leftSetting; }
		} // LeftSetting

		// ----------------------------------------------------------------------
		public ISetting WidthSetting
		{
			get { return this.widthSetting; }
		} // WidthSetting

		// ----------------------------------------------------------------------
		public ISetting HeightSetting
		{
			get { return this.heightSetting; }
		} // HeightSetting

		// ----------------------------------------------------------------------
		public ISetting StateSetting
		{
			get { return this.stateSetting; }
		} // StateSetting

		// ----------------------------------------------------------------------
		public DialogResult? SaveCondition
		{
			get { return this.saveCondition; }
			set { this.saveCondition = value; }
		} // SaveCondition

		// ----------------------------------------------------------------------
		public bool UseLocation { get; set; }

		// ----------------------------------------------------------------------
		public bool UseSize { get; set; }

		// ----------------------------------------------------------------------
		public bool UseWindowState { get; set; }

		// ----------------------------------------------------------------------
		public bool AllowMinimized { get; set; }

		// ----------------------------------------------------------------------
		public bool SaveOnClose { get; set; }

		// ----------------------------------------------------------------------
		public override void Save()
		{
			if ( this.saveCondition.HasValue && this.saveCondition.Value != this.form.DialogResult )
			{
				return;
			}
			base.Save();
		} // Save

		// ----------------------------------------------------------------------
		private void FormLoad( object sender, EventArgs e )
		{
			if ( UseLocation )
			{
				Settings.Add( this.topSetting );
				Settings.Add( this.leftSetting );
			}
			if ( UseSize )
			{
				Settings.Add( this.widthSetting );
				Settings.Add( this.heightSetting );
			}
			if ( UseWindowState )
			{
				Settings.Add( this.stateSetting );
			}

			Load();
		} // FormLoad

		// ----------------------------------------------------------------------
		private void FormClosing( object sender, CancelEventArgs e )
		{
			if ( !this.SaveOnClose )
			{
				return;
			}
			Save();
		} // FormClosing

		// ----------------------------------------------------------------------
		private PropertySetting CreateSetting( string name, string propertyName )
		{
			PropertySetting propertySetting = new PropertySetting( name, this.form, propertyName );
			propertySetting.ValueSaving += new SettingValueCancelEventHandler( ValueSaving );
			return propertySetting;
		} // CreateSetting

		// ----------------------------------------------------------------------
		private void ValueSaving( object sender, SettingValueCancelEventArgs e )
		{
			if ( AllowMinimized == false && this.form.WindowState == FormWindowState.Minimized )
			{
				e.Cancel = true;
			}
		} // ValueSaving

		// ----------------------------------------------------------------------
		// members
		private readonly Form form;
		private readonly PropertySetting topSetting;
		private readonly PropertySetting leftSetting;
		private readonly PropertySetting widthSetting;
		private readonly PropertySetting heightSetting;
		private readonly PropertySetting stateSetting;
		private DialogResult? saveCondition;
	} // class FormSettings

} // namespace Itenso.Configuration
// -- EOF -------------------------------------------------------------------
