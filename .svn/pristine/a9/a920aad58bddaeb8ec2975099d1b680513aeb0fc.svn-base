// -- FILE ------------------------------------------------------------------
// name       : ApplicationSettings.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Configuration;

namespace Itenso.Configuration
{

	// ------------------------------------------------------------------------
	public class ApplicationSettings : ApplicationSettingsBase
	{

		// ----------------------------------------------------------------------
		public event SettingValueCancelEventHandler SettingSaving;
		public event SettingValueCancelEventHandler SettingLoading;
		public event SettingCollectorCancelEventHandler CollectingSetting;

		// ----------------------------------------------------------------------
		public const string UpgradeSettingsKey = "UpgradeSettings";

		// ----------------------------------------------------------------------
		public ApplicationSettings( object obj ) :
			this( obj.GetType().Name, obj )
		{
		} // ApplicationSettings

		// ----------------------------------------------------------------------
		public ApplicationSettings( string settingsKey ) :
			this( settingsKey, null )
		{
		} // ApplicationSettings

		// ----------------------------------------------------------------------
		public ApplicationSettings( string settingsKey, object obj ) :
			base( settingsKey )
		{
			this.settings = new SettingCollection( this );

			// provider
			this.defaultProvider = new LocalFileSettingsProvider();
			this.defaultProvider.Initialize( "LocalFileSettingsProvider", null );
			base.Providers.Add( this.defaultProvider );

			// upgrade
			this.upgradeSettings = new ValueSetting( UpgradeSettingsKey, typeof( bool ), true );
			UseAutoUpgrade = true;

			if ( obj != null )
			{
				Settings.AddAll( obj );
			}
		} // ApplicationSettings

		// ----------------------------------------------------------------------
		public SettingsProvider DefaultProvider
		{
			get { return this.defaultProvider; }
		} // DefaultProvider

		// ----------------------------------------------------------------------
		public SettingCollection Settings
		{
			get { return this.settings; }
		} // Settings

		// ----------------------------------------------------------------------
		public SettingCollectorCollection SettingCollectors
		{
			get 
			{
				if ( this.settingCollectors == null )
				{
					this.settingCollectors = new SettingCollectorCollection( this );
					this.settingCollectors.CollectingSetting += new SettingCollectorCancelEventHandler( SettingCollectorsCollectingSetting );
				}
				return this.settingCollectors; 
			}
		} // SettingCollectors

		// ----------------------------------------------------------------------
		public bool UseAutoUpgrade
		{
			get { return this.settings.Contains( this.upgradeSettings ); }
// ReSharper disable ValueParameterNotUsed
			set 
// ReSharper restore ValueParameterNotUsed
			{
				if ( UseAutoUpgrade )
				{
					return;
				}
				this.settings.Add( this.upgradeSettings );
			}
		} // UseAutoUpgrade

		// ----------------------------------------------------------------------
		public static string UserConfigurationFilePath
		{
			get 
			{
				System.Configuration.Configuration config =
					ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.PerUserRoamingAndLocal );
				return config.FilePath; 
			}
		} // UserConfigurationFilePath

		// ----------------------------------------------------------------------
		public void Load()
		{
			Load( true );
		} // Load

		// ----------------------------------------------------------------------
		public virtual void Load( bool upgrade )
		{
			CollectSettings();
			LoadSettings();
			if ( upgrade )
			{
				if ( (bool)upgradeSettings.Value )
				{
					Upgrade();
					this.upgradeSettings.Value = false;
				}
				LoadSettings();
			}
		} // Load

		// ----------------------------------------------------------------------
		public new virtual void Reload()
		{
			base.Reload();
			LoadSettings();
		} // Reload

		// ----------------------------------------------------------------------
		public new virtual void Reset()
		{
			base.Reset();
			LoadSettings();
		} // Reload

		// ----------------------------------------------------------------------
		public override void Save()
		{
			SaveSettings();
			base.Save();
		} // Save

		// ----------------------------------------------------------------------
		private void CollectSettings()
		{
			if ( this.settingCollectors == null )
			{
				return; // no colectors present
			}
			this.settingCollectors.Collect();
		} // CollectSettings

		// ----------------------------------------------------------------------
		private void LoadSettings()
		{
			foreach ( ISetting userSetting in this.settings )
			{
				if ( SettingLoading != null )
				{
					userSetting.ValueLoading += new SettingValueCancelEventHandler( UserSettingLoading );
				}
				userSetting.Load();
				if ( SettingLoading != null )
				{
					userSetting.ValueLoading -= new SettingValueCancelEventHandler( UserSettingLoading );
				}
			}
		} // LoadSettings

		// ----------------------------------------------------------------------
		private void SaveSettings()
		{
			foreach ( ISetting userSetting in this.settings )
			{
				if ( SettingSaving != null )
				{
					userSetting.ValueSaving += new SettingValueCancelEventHandler( UserSettingSaving );
				}
				userSetting.Save();
				if ( SettingSaving != null )
				{
					userSetting.ValueSaving -= new SettingValueCancelEventHandler( UserSettingSaving );
				}
			}
		} // SaveSettings
		
		// ----------------------------------------------------------------------
		protected virtual void OnCollectingSetting( SettingCollectorCancelEventArgs e )
		{
			if ( CollectingSetting != null )
			{
				CollectingSetting( this, e );
			}
		} // OnCollectingSetting

		// ----------------------------------------------------------------------
		private void UserSettingSaving( object sender, SettingValueCancelEventArgs e )
		{
			if ( SettingSaving != null )
			{
				SettingSaving( sender, e );
			}
		} // UserSettingSaving

		// ----------------------------------------------------------------------
		private void UserSettingLoading( object sender, SettingValueCancelEventArgs e )
		{
			if ( SettingLoading != null )
			{
				SettingLoading( sender, e );
			}
		} // UserSettingLoading

		// ----------------------------------------------------------------------
		private void SettingCollectorsCollectingSetting( object sender, SettingCollectorCancelEventArgs e )
		{
			OnCollectingSetting( e );
		} // SettingCollectorsCollectingSetting

		// ----------------------------------------------------------------------
		// members
		private readonly LocalFileSettingsProvider defaultProvider = new LocalFileSettingsProvider();
		private readonly SettingCollection settings;
		private SettingCollectorCollection settingCollectors;
		private readonly ValueSetting upgradeSettings;

	} // class ApplicationSettings

} // namespace Itenso.Configuration
// -- EOF -------------------------------------------------------------------
