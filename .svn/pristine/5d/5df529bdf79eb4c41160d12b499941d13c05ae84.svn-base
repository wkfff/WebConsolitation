using System;

namespace Krista.FM.RIA.Core.Gui
{
    public class ControlBuilder
    {
        private static readonly ControlBuilder instance = new ControlBuilder();
        private Func<IControlFactory> factoryThunk;

        static ControlBuilder()
        {
            instance = new ControlBuilder();
        }

        public ControlBuilder()
        {
            DefaultControlFactory controlFactory = new DefaultControlFactory();
            controlFactory.ControlBuilder = this;
            SetControlFactory(controlFactory);
        }

        public static ControlBuilder Current
        {
            get { return instance; }
        }

        public IControlFactory GetControllerFactory()
        {
            return factoryThunk();
        }

        public void SetControlFactory(IControlFactory controlFactory)
        {
            if (controlFactory == null)
            {
                throw new ArgumentNullException("controlFactory");
            }

            factoryThunk = delegate
            {
                return controlFactory;
            };
        }
    }
}
