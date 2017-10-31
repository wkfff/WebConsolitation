using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class SampleControl : CustomTreeNodeControl<object>
    {
        public SampleControl(string name, string text, object controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(name, text, controlObject, parent, imageIndex)
        {
        }
    }
}
