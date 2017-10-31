using System;
using System.Threading;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    /// <summary>
    /// Инкапсулирует методы фонофой инициализации схемы.
    /// </summary>
    internal class BackgroundInitializeTasks
    {
        /// <summary>
        /// Запускает фоновую пост-инициализацию объектов схемы.
        /// </summary>
        internal static void PostInitializeSchemeObjects()
        {
            Thread thread = new Thread(new ThreadStart(PostInitializeSchemeObjectsEntryPoint));
            thread.Start();
        }

        internal static void PostInitializeSchemeObjectsEntryPoint()
        {
            Thread.CurrentThread.Name = "PostInitializeSchemeObjects";
            Trace.TraceVerbose("Запущена фоновая инициализация объектов.");

            SchemeClass scheme = SchemeClass.Instance;
            SchemeClass.MutexSchemeAutoUpdate.WaitOne();
            
            try
            {
                foreach (Classes.Entity item in scheme.Classifiers.Values)
                    item.PostInitialize();
                foreach (Classes.Entity item in scheme.FactTables.Values)
                    item.PostInitialize();
                foreach (Classes.EntityAssociation item in scheme.Associations.Values)
                    item.PostInitialize();

                scheme.InitializeConversionTables();

                if (scheme.NeedUpdateScheme)
                {
                    SchemeDWH.Instance.DB.ExecQuery("update DatabaseVersions set NeedUpdate = 0 where ID = (select max(ID) from DatabaseVersions)", QueryResultTypes.Scalar);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Ошибка в PostInitialize: {0}", e.Message));
            }
            finally
            {
                SchemeClass.MutexSchemeAutoUpdate.ReleaseMutex();
            }

            Trace.TraceVerbose("Завершена фоновая инициализация объектов.");
        }
    }
}
