using System;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core.Gui
{
    public class DefaultControlFactory : IControlFactory
    {
        private ControlBuilder controlBuilder;

        internal ControlBuilder ControlBuilder
        {
            get { return controlBuilder ?? ControlBuilder.Current; }
            set { controlBuilder = value; }
        }

        public Control CreateControl(Type controlType)
        {
            IUnityContainer container = Resolver.Get<IUnityContainer>();
            return container.Resolve(controlType) as Control;
        }
    }
}
