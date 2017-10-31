// -- FILE ------------------------------------------------------------------
// name       : FieldSetting.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System;
using System.Reflection;

namespace Itenso.Configuration
{

	// ------------------------------------------------------------------------
	public class FieldSetting : ValueSettingBase
	{

		// ----------------------------------------------------------------------
		public FieldSetting( object component, FieldInfo fieldInfo ) :
			this( fieldInfo.Name, component, fieldInfo )
		{
		} // FieldSetting

		// ----------------------------------------------------------------------
		public FieldSetting( string name, object component, FieldInfo fieldInfo ) :
			this( name, component, fieldInfo, null )
		{
		} // FieldSetting

		// ----------------------------------------------------------------------
		public FieldSetting( string name, object component, FieldInfo fieldInfo, object defaultValue ) :
			base( name, defaultValue )
		{
			if ( component == null )
			{
				throw new ArgumentNullException( "component" );
			}
			if ( fieldInfo == null )
			{
				throw new ArgumentNullException( "fieldInfo" );
			}

			this.component = component;
			this.fieldInfo = fieldInfo;
		} // FieldSetting

		// ----------------------------------------------------------------------
		public FieldSetting( object component, string fieldName ) :
			this( fieldName, component, fieldName )
		{
		} // FieldSetting
		
		// ----------------------------------------------------------------------
		public FieldSetting( string name, object component, string fieldName ) :
			this( name, component, fieldName, null )
		{
		} // FieldSetting

		// ----------------------------------------------------------------------
		public FieldSetting( string name, object component, string fieldName, object defaultValue ) :
			base( name, defaultValue )
		{
			if ( component == null )
			{
				throw new ArgumentNullException( "component" );
			}
			if ( string.IsNullOrEmpty( fieldName ) )
			{
				throw new ArgumentNullException( "fieldName" );
			}

			this.component = component;
			this.fieldInfo = component.GetType().GetField( fieldName,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
			if ( fieldInfo == null )
			{
				throw new ArgumentException( "missing setting field: " + fieldName );
			}
			if ( fieldInfo.IsInitOnly ) // readonly field
			{
				throw new ArgumentException( "setting field '" + fieldName + "' is readonly" );
			}
		} // FieldSetting

		// ----------------------------------------------------------------------
		public FieldInfo FieldInfo
		{
			get { return this.fieldInfo; }
		} // FieldInfo

		// ----------------------------------------------------------------------
		public string FieldName
		{
			get { return this.fieldInfo.Name; }
		} // FieldName

		// ----------------------------------------------------------------------
		public object Component
		{
			get { return this.component; }
		} // Component

		// ----------------------------------------------------------------------
		public override object OriginalValue
		{
			get { return LoadValue( Name, this.fieldInfo.FieldType, SerializeAs, DefaultValue ); }
		} // OriginalValue

		// ----------------------------------------------------------------------
		public override object Value
		{
			get { return this.fieldInfo.GetValue( this.component ); }
			set { this.fieldInfo.SetValue( this.component, value ); }
		} // Value

		// ----------------------------------------------------------------------
		public override void Load()
		{
			try
			{
				object originalValue = OriginalValue;
				if ( originalValue == null && LoadUndefinedValue == false )
				{
					return;
				}
				Value = originalValue;
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
				object value = Value;
				if ( value == null && SaveUndefinedValue == false )
				{
					return;
				}
				SaveValue( Name, this.fieldInfo.FieldType, SerializeAs, value, DefaultValue );
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
		// members
		private readonly object component;
		private readonly FieldInfo fieldInfo;

	} // class FieldSetting

} // namespace Itenso.Configuration
// -- EOF -------------------------------------------------------------------
