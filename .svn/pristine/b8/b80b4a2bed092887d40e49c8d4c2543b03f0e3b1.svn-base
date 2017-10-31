using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace Krista.FM.Client.Common
{
    public delegate void checkProcDelegate(Component cmp);
    
    public struct ComponentCustomizer
    {
        public static void EnumControls(Control parentCtrl, checkProcDelegate checkProc)
        {
            if ((parentCtrl == null) || (parentCtrl.Controls == null)) return;
            foreach (Control childCtrl in parentCtrl.Controls)
            {
                Krista.FM.Client.Components.UltraGridEx ug = childCtrl as Krista.FM.Client.Components.UltraGridEx;
                if (ug != null)
                {
                    ;
                }
                checkProc(childCtrl);
                EnumControls(childCtrl, checkProc);
            }
        }

        public static void EnumComponents(System.ComponentModel.IContainer container, checkProcDelegate checkProc)
        {
            if ((container == null) || (container.Components == null)) return;
            foreach (Component childCmp in container.Components)
                checkProc(childCmp);
        }

        public static void CustomizeInfragisticsControls(Control Ctrl)
        {
            Ctrl.SuspendLayout();
            try
            {
                checkProcDelegate checkProc = new checkProcDelegate(
                    InfragisticComponentsCustomize.CustomizeInfragisticsControl);
                EnumControls(Ctrl, checkProc);
            }
            finally
            {
                Ctrl.ResumeLayout();
            }
        }

        public static void CustomizeInfragisticsComponents(System.ComponentModel.Container container)
        {
            CustomizeInfragisticsComponents((IContainer)container);
        }

        public static void CustomizeInfragisticsComponents(System.ComponentModel.IContainer container)
        {
            if (container == null) return;
            checkProcDelegate checkProc = new checkProcDelegate(
                InfragisticComponentsCustomize.CustomizeInfragisticsControl);
            EnumComponents(container, checkProc);
        }

    }

}