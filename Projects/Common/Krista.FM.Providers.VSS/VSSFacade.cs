using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using VSSFileStatus = SourceSafeTypeLib.VSSFileStatus;
#if SS80
using Microsoft.VisualStudio.SourceSafe.Interop;
#else
using SourceSafeTypeLib;
#endif

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Providers.VSS
{
    /// <summary>
    /// Предоставляет доступ к файлам в Visual Source Safe.
    /// </summary>
    public class VSSFacade : IVSSFacade
    {
        private string baseLocalPath = String.Empty;
        private VSSDatabase vssDatabase;
        private IVSSItem vssBaseFolder;

        private string internalPassword = string.Empty;
        private string internalbaseWorkingProject = string.Empty;

        ~VSSFacade()
        {
            Close();
        }

        private string GetLocalPath(string local)
        {
            return String.Format("{0}\\{1}", baseLocalPath, local.Replace('/', '\\'));
        }

        private void CheckOpen()
        {
            if (vssDatabase == null)
                throw new Exception("База SourceSafe не открыта.");
            if (vssBaseFolder == null)
                throw new Exception("рабочий проект не открыт.");
        }

        private static IVSSItem GetChild(IVSSItem vssItem, string local)
        {
#if SS80
            return vssItem.get_Child(local, false);
#else
            string[] parts = local.Split('/');
            IVSSItem currertProject = vssItem;
            for (int i = 0; i < parts.Length; i++)
            {
                foreach (IVSSItem item in currertProject.get_Items(false))
                {
                    if (item.Name == parts[i])
                    {
                        currertProject = item;
                        if (i == parts.Length - 1)
                            return item;
                        break;
                    }
                }
            }
            throw new COMException(String.Format("Элемент \"{0}\" не найден.", local));
#endif
        }

        private IVSSItem Add(string local)
        {
            try
            {
                string[] parts = local.Split('/');
                IVSSItem currertProject = vssBaseFolder;
                IVSSItem item;
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    try
                    {
                        item = GetChild(currertProject, parts[i]);
                        currertProject = item;
                    }
                    catch (COMException)
                    {
                        item = currertProject.Add(parts[0], "", 0);
                        currertProject = item;
                    }
                }

                string localPath = String.Format("{0}\\{1}", baseLocalPath, local.Replace('/', '\\'));
                Trace.TraceVerbose("Добавляем объект в хранилище: {0}", localPath);
                item = currertProject.Add(localPath, String.Format("User: {0}", ClientAuthentication.UserName), 0);

                return item;
            }
            catch (Exception e)
            {
                Trace.TraceError("{0}", e.ToString());
                throw;
            }
        }

        #region IVSSFacade Members

        public void Open(string srcSafeIni, string username, string password, string baseWorkingProject, string localPath)
        {
            //Debug.WriteLine(String.Format("{0} Open: {1} {2} {3}", DateTime.Now, srcSafeIni, username, baseWorkingProject), "VSSFacade");
            try
            {
                baseLocalPath = localPath;
                String[] parts = username.Split('\\');
                string user = parts[parts.Length - 1];
                internalbaseWorkingProject = baseWorkingProject;

                vssDatabase = new VSSDatabase();
                vssDatabase.Open(srcSafeIni, user, password);
                vssBaseFolder = vssDatabase.get_VSSItem(baseWorkingProject, false);
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Open: {1}", DateTime.Now, e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public void Close()
        {
            try
            {
                if (vssDatabase != null)
                {
#if SS80
                    vssDatabase.Close();
#endif
                    vssDatabase = null;
                    vssBaseFolder = null;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Close: {1}", DateTime.Now, e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public void Checkin(string local, string comments)
        {
            try
            {
                CheckOpen();

                Trace.TraceInformation("Checkin \"{0}\" at {1}", local, DateTime.Now);

                IVSSItem vssItem;
                try
                {
                    vssItem = GetChild(vssBaseFolder, local);
                }
                catch (COMException)
                {
                    Add(local);
                    Trace.TraceInformation("Add \"{0}\" at {1}", local, DateTime.Now);
                    return;
                }

                if (vssItem != null)
                {
                    try
                    {
                        vssItem.Checkin(
                            String.Format("Пользователь: {0}\n-----Комментарий------\n{1}",
                                          ClientAuthentication.UserName, comments),
                            GetLocalPath(local), 0);
                    }
                    catch (AccessViolationException ex)
                    {
                        // временно глушим эту ошибку, выводя сообщение в лог
                        Trace.TraceError("{0} Checkin: {1}", DateTime.Now, ex.Message);

                        if (ex.Source != "Interop.SourceSafeTypeLib")
                            throw new Exception(ex.Message, ex);
                    }
                    DumpComments(comments);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Checkin: {1}", DateTime.Now, e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public void Checkout(string local, string comments)
        {
            try
            {
                CheckOpen();
                IVSSItem vssItem = GetChild(vssBaseFolder, local);
                vssItem.Checkout(
                    String.Format("Пользователь: {0}\n-----Комментарий------\n{1}", ClientAuthentication.UserName, comments), 
                    GetLocalPath(local), 0);
                Trace.TraceInformation("Checkout \"{0}\" at {1}", local, DateTime.Now);
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Checkout: {1}", DateTime.Now, e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public void UndoCheckout(string local)
        {
            try
            {
                CheckOpen();
                IVSSItem vssItem = GetChild(vssBaseFolder, local);
                if ((VSSFileStatus)vssItem.IsCheckedOut == VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
                {
                    vssItem.UndoCheckout(GetLocalPath(local), 0);
                    Trace.TraceInformation("UndoCheckout \"{0}\" at {1}", local, DateTime.Now);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} UndoCheckout: {1}", DateTime.Now, e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public ServerLibrary.VSSFileStatus IsCheckedOut(string local)
        {
            try
            {
                CheckOpen();
                IVSSItem vssItem = GetChild(vssBaseFolder, local);
                if ((VSSItemType)vssItem.Type == VSSItemType.VSSITEM_FILE)
                {
                    switch ((VSSFileStatus)vssItem.IsCheckedOut)
                    {
#if SS80
                        case Microsoft.VisualStudio.SourceSafe.Interop.VSSFileStatus.VSSFILE_NOTCHECKEDOUT: return Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
                        case Microsoft.VisualStudio.SourceSafe.Interop.VSSFileStatus.VSSFILE_CHECKEDOUT: return Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_CHECKEDOUT;
                        case Microsoft.VisualStudio.SourceSafe.Interop.VSSFileStatus.VSSFILE_CHECKEDOUT_ME: return Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_CHECKEDOUT_ME;
#else
                        case VSSFileStatus.VSSFILE_NOTCHECKEDOUT: return ServerLibrary.VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
                        case VSSFileStatus.VSSFILE_CHECKEDOUT: return ServerLibrary.VSSFileStatus.VSSFILE_CHECKEDOUT;
                        case VSSFileStatus.VSSFILE_CHECKEDOUT_ME: return ServerLibrary.VSSFileStatus.VSSFILE_CHECKEDOUT_ME;
#endif
                        default: throw new Exception(String.Format("Неизвестный VSSFileStatus = {0}", vssItem.IsCheckedOut));
                    }
                }
                else
                    return ServerLibrary.VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} IsCheckedOut: {1}", DateTime.Now, e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public bool Find(string local)
        {
            try
            {
                CheckOpen();

                try
                {
                    GetChild(vssBaseFolder, local);
                    return true;
                }
                catch (COMException)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Find: {1}", DateTime.Now, e.Message);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        private static void DumpComments(string text)
        {
            try
            {
                using (TextWriter tw = new StreamWriter("CommentsList.txt", true, Encoding.Default))
                {
                    tw.WriteLine(text);
                }
            }
            catch { ; }
        }

        #region IVSSFacade Members


        public string GetCheckOutUser(string local)
        {
            try
            {
                CheckOpen();
                IVSSItem vssItem = GetChild(vssBaseFolder, local);
                if ((VSSItemType)vssItem.Type == VSSItemType.VSSITEM_FILE &&
                    (ServerLibrary.VSSFileStatus)vssItem.IsCheckedOut ==
                    Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_CHECKEDOUT)
                {
                    IEnumerator checkOuts = vssItem.Checkouts.GetEnumerator();
                    checkOuts.MoveNext();
                    IVSSCheckout checkOut = (IVSSCheckout)checkOuts.Current;
                    return checkOut.Username;
                }
                return null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(String.Format("{0} Получаю имя пользователя, извлекшего файл {1}: {2}", DateTime.Now, local, e.Message), GetType().Name);
                throw new Exception(e.Message, e);
            }
        }

        public string GetUserName()
        {
            try
            {
                if (vssDatabase != null)
                {
                    return vssDatabase.Username;
                }
                else
                    return string.Empty;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("{0} Получаю имя пользователя: {1}", DateTime.Now, e.Message), GetType().Name);
                throw new Exception(e.Message, e);
            }
        }

        public string GetDatabaseName()
        {
            try
            {
                if (vssDatabase != null)
                {
                    return vssDatabase.DatabaseName;
                }
                else
                    return string.Empty;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(string.Format(
                    "{0} Получаю имя базы данных: {1}", DateTime.Now, e.Message), GetType().Name);
                throw new Exception(e.Message, e);
            }
        }

        public string GetINI()
        {
            try
            {
                if (vssDatabase != null)
                {
                    return vssDatabase.SrcSafeIni;
                }
                else
                    return string.Empty;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(string.Format(
                    "{0} Получаю имя ini-файла: {1}", DateTime.Now, e.Message), GetType().Name);
                throw new Exception(e.Message, e);
            }
        }

        public void Get(string local, string path)
        {
            CheckOpen();

            IVSSItem vssItem = GetChild(vssBaseFolder, local);

            if (vssItem != null)
            {
                // Get a file into a specified folder.
                string testFile = path;
                vssItem.Get(ref testFile, 0);
            }
        }

        public void Refresh()
        {
            if (vssDatabase != null)
            {
                Open(GetINI(), GetUserName(), internalPassword, internalbaseWorkingProject, baseLocalPath);
            }		
        }

        #endregion
    }
}
