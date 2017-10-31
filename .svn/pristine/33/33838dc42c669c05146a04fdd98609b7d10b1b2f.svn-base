using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Krista.FM.ServerLibrary;
using System.Diagnostics;
using System.Collections;
using SourceSafeTypeLib;

namespace Krista.FM.Client.OLAPResources
{
    /// <summary>
    /// Расширенный интерфейс для работы с VSS
    /// </summary>
	public interface IVSSFacade2 : IVSSFacade
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
		string GetCheckOutUser(string local);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		string GetUserName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		string GetDatabaseName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		string GetINI();

        /// <summary>
        /// Метод берет версия файла из VSS на диск без блокирования файла
        /// </summary>
        /// <param name="local">локальный путь файла</param>
        /// <param name="path"> Полный путь к выкачаному файлу</param>
	    void Get(string local, string  path);

        /// <summary>
        /// 
        /// </summary>
		void Refresh();
	}

	public class VSSUtils : IVSSFacade2
	{
		private string baseLocalPath = String.Empty;
		private VSSDatabase vssDatabase;
		private IVSSItem vssBaseFolder;

		private string internalPassword = string.Empty;
		private string internalbaseWorkingProject = string.Empty;

		~VSSUtils()
		{
			Close();
		}

	    public void Get(string  local, string path)
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

        private static IVSSItem GetChild(IVSSItem vssItem, string local)
        {
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
        }

		private void CheckOpen()
		{
			if (vssDatabase == null)
				throw new Exception("База SourceSafe не открыта.");
			if (vssBaseFolder == null)
				throw new Exception("рабочий проект не открыт.");
		}

		private IVSSItem Add(string local)
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
				catch (System.Runtime.InteropServices.COMException)
				{
					item = currertProject.Add(parts[0], "", 0);
					currertProject = item;
				}
			}

			item = currertProject.Add(String.Format("{0}\\{1}", baseLocalPath, local.Replace('/', '\\')),
				String.Format("User: {0}", GetUserName()), 0);

			return item;
		}		

		private string GetLocalPath(string local)
		{
			return String.Format("{0}\\{1}", baseLocalPath, local.Replace('/', '\\'));
		}
		
		#region IVSSFacade Members
		
		public void Open(string srcSafeIni,
			string username, string password, string baseWorkingProject, string localPath)
		{
			try
			{
				this.baseLocalPath = localPath;
				this.internalPassword = password;
				this.internalbaseWorkingProject = baseWorkingProject;
				String[] parts = username.Split('\\');
				string user = parts[parts.Length - 1];

				vssDatabase = new VSSDatabase();
				vssDatabase.Open(srcSafeIni, user, password);
				vssBaseFolder = vssDatabase.get_VSSItem(baseWorkingProject, false);
			}
			catch (Exception e)
			{	
				Trace.WriteLine(String.Format("{0} Open: {1}", DateTime.Now, e.Message), this.GetType().Name);
				throw new Exception(e.Message, e);
			}
		}

		public void Close()
		{
			try 
			{
				if (vssDatabase != null)
				{
					vssDatabase = null;
					vssBaseFolder = null;
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(String.Format("{0} Close: {1}", DateTime.Now, e.Message), this.GetType().Name);
				throw new Exception(e.Message, e);
			}
		}

		public void Checkin(string local, string comments)
		{
			try
			{
				CheckOpen();
				IVSSItem vssItem = null;
				try
				{
					vssItem = GetChild(vssBaseFolder, local);
				}
				catch (System.Runtime.InteropServices.COMException)
				{
					Add(local);
					Trace.WriteLine(
						String.Format("Add \"{0}\" at {1}", local, DateTime.Now), this.GetType().Name);
					return;
				}

				if (vssItem != null)
				{
					vssItem.Checkin(String.Format(
						"Пользователь: {0}\n-----Комментарий------\n{1}", GetUserName(), comments),
						GetLocalPath(local), 0);
					Trace.WriteLine(String.Format(
						"Checkin \"{0}\" at {1}", local, DateTime.Now), this.GetType().Name);
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(String.Format(
					"{0} Checkin: {1}", DateTime.Now, e.Message), this.GetType().Name);
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
					 comments,
					GetLocalPath(local), 0);
				Trace.WriteLine(String.Format("Checkout \"{0}\" at {1}", local, DateTime.Now),
					this.GetType().Name);
			}
			catch (Exception e)
			{
				Trace.WriteLine(String.Format("{0} Checkout: {1}", DateTime.Now, e.Message),
					this.GetType().Name);
				throw new Exception(e.Message, e);
			}
		}

		public void UndoCheckout(string local)
		{
			try
			{
				CheckOpen();
                IVSSItem vssItem = GetChild(vssBaseFolder, local);
				if ((Krista.FM.ServerLibrary.VSSFileStatus)vssItem.IsCheckedOut ==
					Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
				{
					vssItem.UndoCheckout(GetLocalPath(local), 0);
					Trace.WriteLine(String.Format(
						"UndoCheckout \"{0}\" at {1}", local, DateTime.Now), this.GetType().Name);
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(String.Format("{0} UndoCheckout: {1}", DateTime.Now, e.Message),
					this.GetType().Name);
				throw new Exception(e.Message, e);
			}
		}

		public Krista.FM.ServerLibrary.VSSFileStatus IsCheckedOut(string local)
		{
			try
			{
				CheckOpen();
                IVSSItem vssItem = GetChild(vssBaseFolder, local);
				if ((VSSItemType)vssItem.Type == VSSItemType.VSSITEM_FILE)
					return (Krista.FM.ServerLibrary.VSSFileStatus)vssItem.IsCheckedOut;
				else
					return Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
			}
			catch (Exception e)
			{
				Trace.WriteLine(String.Format(
					"{0} IsCheckedOut: {1}", DateTime.Now, e.Message), this.GetType().Name);
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
				catch (System.Runtime.InteropServices.COMException)
				{
					return false;
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(String.Format("{0} Find: {1}", DateTime.Now, e.Message), this.GetType().Name);
				throw new Exception(e.Message, e);
			}
		}

		#endregion

		#region IVSSFacade2 Members

		public string GetCheckOutUser(string local)
		{
			try
			{
				CheckOpen();
                IVSSItem vssItem = GetChild(vssBaseFolder, local);
				if ((VSSItemType)vssItem.Type == VSSItemType.VSSITEM_FILE &&
					(Krista.FM.ServerLibrary.VSSFileStatus)vssItem.IsCheckedOut ==
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
				Trace.WriteLine(String.Format("{0} Получаю имя пользователя, извлекшего файл {1}: {2}",
					DateTime.Now, local, e.Message), this.GetType().Name);
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
			catch(Exception e)
			{
				Trace.WriteLine(string.Format(
					"{0} Получаю имя пользователя: {1}", DateTime.Now, e.Message), this.GetType().Name);
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
				Trace.WriteLine(string.Format(
					"{0} Получаю имя базы данных: {1}", DateTime.Now, e.Message), this.GetType().Name);
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
				Trace.WriteLine(string.Format(
					"{0} Получаю имя ini-файла: {1}", DateTime.Now, e.Message), this.GetType().Name);
				throw new Exception(e.Message, e);
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


	public class VSSInfo
	{
		public Krista.FM.ServerLibrary.VSSFileStatus VSSState =
			Krista.FM.ServerLibrary.VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
		public string VSSSpec = string.Empty;
		public string VSSCheckUser = string.Empty;
		public bool NewFile = false;

	}	
}