// -- FILE ------------------------------------------------------------------
// name       : SettingCollectorCollection.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System;
using System.Collections;

namespace Itenso.Configuration
{

	// ------------------------------------------------------------------------
	public sealed class SettingCollectorCollection : IEnumerable
	{

		// ----------------------------------------------------------------------
		public event SettingCollectorCancelEventHandler CollectingSetting;

		// ----------------------------------------------------------------------
		public SettingCollectorCollection( ApplicationSettings applicationSettings )
		{
			if ( applicationSettings == null )
			{
				throw new ArgumentNullException( "applicationSettings" );
			}

			this.applicationSettings = applicationSettings;
		} // SettingCollectorCollection

		// ----------------------------------------------------------------------
		public ApplicationSettings ApplicationSettings
		{
			get { return this.applicationSettings; }
		} // ApplicationSettings

		// ----------------------------------------------------------------------
		public int Count
		{
			get { return this.settingCollectors.Count; }
		} // Count

		// ----------------------------------------------------------------------
		public IEnumerator GetEnumerator()
		{
			return this.settingCollectors.GetEnumerator();
		} // GetEnumerator

		// ----------------------------------------------------------------------
		public void Add( ISettingCollector setting )
		{
			if ( setting == null )
			{
				throw new ArgumentNullException( "setting" );
			}
			setting.ApplicationSettings = this.applicationSettings;
			setting.CollectingSetting += new SettingCollectorCancelEventHandler( SettingCollectingSetting );
			this.settingCollectors.Add( setting );
		} // Add

		// ----------------------------------------------------------------------
		public void Remove( ISettingCollector setting )
		{
			if ( setting == null )
			{
				throw new ArgumentNullException( "setting" );
			}
			setting.CollectingSetting -= new SettingCollectorCancelEventHandler( SettingCollectingSetting );
			this.settingCollectors.Remove( setting );
		} // Remove

		// ----------------------------------------------------------------------
		public void Clear()
		{
			foreach ( ISettingCollector settingCollector in this.settingCollectors )
			{
				Remove( settingCollector );
			}
		} // Clear

		// ----------------------------------------------------------------------
		public void Collect()
		{
			foreach ( ISettingCollector settingCollector in this.settingCollectors )
			{
				settingCollector.Collect();
			}
		} // Collect

		// ----------------------------------------------------------------------
		private void SettingCollectingSetting( object sender, SettingCollectorCancelEventArgs e )
		{
			if ( CollectingSetting != null )
			{
				CollectingSetting( this, e );
			}
		} // SettingCollectingSetting

		// ----------------------------------------------------------------------
		// members
		private readonly ArrayList settingCollectors = new ArrayList();
		private readonly ApplicationSettings applicationSettings;

	} // class SettingCollectorCollection

} // namespace Itenso.Configuration
// -- EOF -------------------------------------------------------------------
