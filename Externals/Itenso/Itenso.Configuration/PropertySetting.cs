// -- FILE ------------------------------------------------------------------
// name       : PropertySetting.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System;
using System.Reflection;

namespace Itenso.Configuration
{

	// ------------------------------------------------------------------------
	public class PropertySetting : ValueSettingBase
	{

		// ----------------------------------------------------------------------
		public PropertySetting( object component, PropertyInfo propertyInfo ) :
			this( propertyInfo.Name, component, propertyInfo )
		{
		} // PropertySetting

		// ----------------------------------------------------------------------
		public PropertySetting( string name, object component, PropertyInfo propertyInfo ) :
			this( name, component, propertyInfo, null )
		{
		} // PropertySetting

		// ----------------------------------------------------------------------
		public PropertySetting( string name, object component, PropertyInfo propertyInfo, object defaultValue ) :
			base( name, defaultValue )
		{
			if ( component == null )
			{
				throw new ArgumentNullException( "component" );
			}
			if ( propertyInfo == null )
			{
				throw new ArgumentNullException( "propertyInfo" );
			}

			this.component = component;
			this.propertyInfo = propertyInfo;
		} // PropertySetting

		// ----------------------------------------------------------------------
		public PropertySetting( object component, string propertyName ) :
			this( propertyName, component, propertyName )
		{
		} // PropertySetting

		// ----------------------------------------------------------------------
		public PropertySetting( string name, object component, string propertyName ) :
			this( name, component, propertyName, null )
		{
		} // PropertySetting

		// ----------------------------------------------------------------------
		public PropertySetting( string name, object component, string propertyName, object defaultValue ) :
			base( name, defaultValue )
		{
			if ( component == null )
			{
				throw new ArgumentNullException( "component" );
			}
			if ( string.IsNullOrEmpty( propertyName ) )
			{
				throw new ArgumentNullException( "propertyName" );
			}

			this.component = component;
			this.propertyInfo = component.GetType().GetProperty( propertyName, 
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
			if ( propertyInfo == null )
			{
				throw new ArgumentException( "missing setting property: " + propertyName );
			}
			if ( !propertyInfo.CanRead ) // no get; accessor
			{
				throw new ArgumentException( "setting property '" + propertyName + "' must be readable" );
			}
			if ( !propertyInfo.CanWrite ) // no set; accessor
			{
				throw new ArgumentException( "setting property '" + propertyName + "' must be writeable" );
			}
		} // PropertySetting

		// ----------------------------------------------------------------------
		public PropertyInfo PropertyInfo
		{
			get { return this.propertyInfo; }
		} // PropertyInfo

		// ----------------------------------------------------------------------
		public string PropertyName
		{
			get { return this.propertyInfo.Name; }
		} // PropertyName

		// ----------------------------------------------------------------------
		public object Component
		{
			get { return this.component; }
		} // Component

		// ----------------------------------------------------------------------
		public override object OriginalValue
		{
			get { return LoadValue( Name, this.propertyInfo.PropertyType, SerializeAs, DefaultValue ); }
		} // OriginalValue

		// ----------------------------------------------------------------------
		public override object Value
		{
			get { return this.propertyInfo.GetValue( this.component, null ); }
			set { this.propertyInfo.SetValue( this.component, value, null ); }
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
				SaveValue( Name, this.propertyInfo.PropertyType, SerializeAs, value, DefaultValue );
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
		private readonly PropertyInfo propertyInfo;

	} // class PropertySetting

} // namespace Itenso.Configuration
// -- EOF -------------------------------------------------------------------
