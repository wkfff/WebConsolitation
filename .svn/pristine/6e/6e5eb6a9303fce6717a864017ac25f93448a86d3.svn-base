using System;

namespace Krista.FM.RIA.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ControllerSessionStateAttribute : Attribute 
    {
        public ControllerSessionStateAttribute(ControllerSessionState mode) 
        {
            Mode = mode;
        }

        public ControllerSessionState Mode { get; private set; }
    }
}
