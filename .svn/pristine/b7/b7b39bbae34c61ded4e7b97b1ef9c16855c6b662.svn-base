// -- FILE ------------------------------------------------------------------
// name       : DataGridViewSetting.cs
// created    : Jani Giannoudis - 2008.05.09
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Configuration;

namespace Itenso.Configuration
{

	// ------------------------------------------------------------------------
	public class DataGridViewSetting : Setting
	{

		// ----------------------------------------------------------------------
		public DataGridViewSetting( DataGridView dataGridView ) :
			this( dataGridView.Name, dataGridView )
		{
		} // DataGridViewSetting

		// ----------------------------------------------------------------------
		public DataGridViewSetting( string name, DataGridView dataGridView )
		{
			if ( string.IsNullOrEmpty( name ) )
			{
				throw new ArgumentNullException( "name" );
			}
			if ( dataGridView == null )
			{
				throw new ArgumentNullException( "dataGridView" );
			}

			this.name = name;
			this.dataGridView = dataGridView;
			UseVisibility = true;
			UseWidth = true;
			UseDisplayIndex = true;
		} // DataGridViewSetting

		// ----------------------------------------------------------------------
		public string Name
		{
			get { return this.name; }
		} // Name

		// ----------------------------------------------------------------------
		public DataGridView DataGridView
		{
			get { return this.dataGridView; }
		} // DataGridView

		// ----------------------------------------------------------------------
		public bool UseVisibility { get; set; }

		// ----------------------------------------------------------------------
		public bool UseWidth { get; set; }

		// ----------------------------------------------------------------------
		public bool UseDisplayIndex { get; set; }

		// ----------------------------------------------------------------------
		public override bool HasChanged
		{
			get
			{
				DataGridViewColumnSetting[] originalColumnSettings = OriginalColumnSettings;
				DataGridViewColumnSetting[] columnSettings = ColumnSettings;
				if ( originalColumnSettings == null || columnSettings == null ||
					originalColumnSettings == columnSettings )
				{
					return false;
				}

				if ( originalColumnSettings.Length != columnSettings.Length )
				{
					return true;
				}

				for ( int i = 0; i < originalColumnSettings.Length; i++ )
				{
					if ( !originalColumnSettings[ i ].Equals( columnSettings[ i ] ) )
					{
						return true;
					}
				}

				return false;
			}
		} // HasChanged

		// ----------------------------------------------------------------------
		private DataGridViewColumnSetting[] OriginalColumnSettings
		{
			get 
			{
				return LoadValue( 
					Name, 
					typeof( DataGridViewColumnSetting[] ),
					SettingsSerializeAs.Binary, 
					null ) as DataGridViewColumnSetting[];
			}
		} // OriginalColumnSettings

		// ----------------------------------------------------------------------
		private DataGridViewColumnSetting[] ColumnSettings
		{
			get
			{
				if ( this.dataGridView.Columns.Count == 0 )
				{
					return null;
				}

				List<DataGridViewColumnSetting> columns =
					new List<DataGridViewColumnSetting>( this.dataGridView.Columns.Count );
				foreach ( DataGridViewColumn dataGridViewColumn in this.dataGridView.Columns )
				{
					columns.Add( new DataGridViewColumnSetting( dataGridViewColumn ) );
				}
				return columns.ToArray();
			}
		} // ColumnSettings

		// ----------------------------------------------------------------------
		public override void Load()
		{
			try
			{
				DataGridViewColumnSetting[] columnSettings = OriginalColumnSettings;
				if ( columnSettings == null || columnSettings.Length == 0 )
				{
					return;
				}

				foreach ( DataGridViewColumnSetting columnSetting in columnSettings )
				{
					DataGridViewColumn dataGridViewColumn = this.dataGridView.Columns[ columnSetting.Name ];
					if ( dataGridViewColumn == null )
					{
						continue;
					}

					if ( this.UseVisibility )
					{
						dataGridViewColumn.Visible = columnSetting.Visible;
					}
					if ( this.UseWidth )
					{
						dataGridViewColumn.Width = columnSetting.Width;
					}
					if ( this.UseDisplayIndex )
					{
						dataGridViewColumn.DisplayIndex = columnSetting.DisplayIndex;
					}
				}
			}
			catch
			{
				if ( ThrowOnErrorLoading )
				{
					throw;
				}
			}
		} // Load

		// ----------------------------------------------------------------------
		public override void Save()
		{
			try
			{
				DataGridViewColumnSetting[] columnSettings = ColumnSettings;
				if ( columnSettings == null )
				{
					return;
				}

				SaveValue( 
					Name,
					typeof( DataGridViewColumnSetting[] ), 
					SettingsSerializeAs.Binary,
					columnSettings,
					null );
			}
			catch
			{
				if ( ThrowOnErrorSaving )
				{
					throw;
				}
			}
		} // Save

		// ----------------------------------------------------------------------
		public override string ToString()
		{
			return string.Concat( name, " (DataGridView)" );
		} // ToString

		// ----------------------------------------------------------------------
		// members
		private readonly DataGridView dataGridView;
		private readonly string name;
	} // class DataGridViewSetting

} // namespace Itenso.Configuration
// -- EOF -------------------------------------------------------------------
