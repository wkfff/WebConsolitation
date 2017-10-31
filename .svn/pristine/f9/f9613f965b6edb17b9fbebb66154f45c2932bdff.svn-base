using System;
using System.Collections.Generic;
using Krista.FM.ServerLibrary.Validations;


namespace Krista.FM.Common.Validations.Messages
{
	[Serializable]
	public class ValidationMessages : List<IValidatorMessageHolder>, IValidatorMessageHolder
    {
		#region IValidatorMessageHolder Members

		public bool HasError
		{
			get 
			{
				foreach (IValidatorMessageHolder vm in this)
				{
					if (vm is ValidationMessage)
					{
						if (((ValidationMessage)vm).HasError)
							return true;
					}
					if (vm is ValidationMessages)
						if (((ValidationMessages)vm).HasError)
							return true;
				}
				return false;
			}
		}

		#endregion
	}
}