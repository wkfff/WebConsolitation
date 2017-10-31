using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;

namespace Krista.FM.Update.Framework.Conditions.SchemeConditions
{
    [Serializable]
    public class SchemeDependedCondition : UpdateCondition
    {
        protected object instance;

        public SchemeDependedCondition(bool local)
        {
        }

        public SchemeDependedCondition()
        {
            if(Process.GetCurrentProcess().MainModule.FileName.Contains("Krista.FM.Update.PatchMakerConsole"))
                return;

            var path = Path.Combine(Path.GetDirectoryName(UpdateManager.Instance.ApplicationPath),
                String.Format("Krista.FM.Update.SchemeAdapter.dll"));

            ObjectHandle handle = null;
            try
            {
                handle = Activator.CreateInstanceFrom(
                    path,
                    "Krista.FM.Update.SchemeAdapter.SchemeAdapter",
                    true, BindingFlags.CreateInstance, null,
                    null, null, null, null);
            }
            catch (FileNotFoundException e)
            {
                Trace.TraceError(string.Format("Невозможно загрузить сборку Krista.FM.Update.SchemeAdapter.dll: {0}", e.Message));
            }

            if (handle != null)
            {
                instance = handle.Unwrap();
            }
        }
    }
}
