using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    /// <summary>
    /// UITypedEditor used to display a list of types for the <see cref="CalcSettings.TreatAsType"/> property of a <see cref="Infragistics.Win.UltraWinCalcManager.CalcSettings"/>.
    /// </summary>
    /// <remarks>
    /// <p class="body">This class provides a dropdown list of types for the <see cref="CalcSettings.TreatAsType"/> property. This is not neccessarily a complete list of all possible types that are valid for the property, but it is a list of the most common types.</p>
    /// </remarks>
    public sealed class EnumTypeEditor : Infragistics.Win.Design.DataTypeUITypeEditor
    {
        /// <summary>
        /// Array of types the ui type editor should display in the drop down.
        /// </summary>
        protected override Type[] Types
        {
            get
            {
                return new Type[] {
                                    typeof(Server.PayPeriodicity),
								  };
            }
        }
    }
}
