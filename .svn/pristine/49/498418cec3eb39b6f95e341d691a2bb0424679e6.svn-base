using System;

using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.RegistryUtils;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;


namespace Krista.FM.Client.MDXExpert
{
	/// <summary>
	/// Менеджер последних используемых шаблонов
	/// </summary>
	public class MRUManager
	{
        #region Members

        private int maxNumberOfFiles = 10;      
        private int maxDisplayLength = 40;      
        private ArrayList mruList;              
        private const string regEntryName = "file";
	    private ListTool listTool;

        #endregion

        #region Windows API

        // BOOL PathCompactPathEx(          
        //    LPTSTR pszOut,
        //    LPCTSTR pszSrc,
        //    UINT cchMax,
        //    DWORD dwFlags
        //    );

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]    
        private static extern bool PathCompactPathEx(
            StringBuilder pszOut, 
            string pszPath,
            int cchMax, 
            int reserved); 
        
        #endregion

		public MRUManager()
		{
            mruList = new ArrayList();
		}

        /// <summary>
        /// Максимальная длинна имени файла в списке
        /// </summary>
        public int MaxDisplayNameLength
        {
            set
            {
                maxDisplayLength = value;

                if ( maxDisplayLength < 10 )
                    maxDisplayLength = 10;
            }

            get
            {
                return maxDisplayLength;
            }
        }

        /// <summary>
        /// Максимальная длинна списка
        /// </summary>
        public int MaxMRULength
        {
            set
            {
                maxNumberOfFiles = value;

                if ( maxNumberOfFiles < 1 )
                    maxNumberOfFiles = 1;

                if ( mruList.Count > maxNumberOfFiles )
                    mruList.RemoveRange(maxNumberOfFiles - 1, mruList.Count - maxNumberOfFiles);
            }

            get
            {
                return maxNumberOfFiles;
            }
        }


        public void Initialize(ListTool list)
        {
            this.listTool = list;
            InitMenuList();
        }

        /// <summary>
        /// Инициализация списка в меню главной формы
        /// </summary>
        private void InitMenuList()
        {
            this.listTool.ListToolItems.Clear();
            IEnumerator myEnumerator = mruList.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                this.listTool.ListToolItems.Add(Path.GetFullPath((string)myEnumerator.Current),
                                       GetDisplayName((string)myEnumerator.Current));
            }

        }

	    /// <summary>
        /// Добавление файла в список
        /// </summary>
        /// <param name="file">File Name</param>
        public void Add(string file)
        {
            Remove(file);

            // если массив имеет максимальную длинну, удаляем последний элемент
            if ( mruList.Count == maxNumberOfFiles )
                mruList.RemoveAt(maxNumberOfFiles - 1);

            // вставляем новое имя файла в начало списка
            mruList.Insert(0, file);
            InitMenuList();
        }

        /// <summary>
        /// Удаление файла из списка
        /// </summary>
        /// <param name="file">File Name</param>
        public void Remove(string file)
        {
            int i = 0;

            IEnumerator myEnumerator = mruList.GetEnumerator();

            while ( myEnumerator.MoveNext() )
            {
                if ( (string)myEnumerator.Current == file )
                {
                    mruList.RemoveAt(i);
                    InitMenuList();
                    return;
                }
                i++;
            }
        }


        public void SaveMRU()
        {
            int i, n;

            try
            {
 
                RegistryKey key = Utils.BuildRegistryKey(Registry.CurrentUser, GetType().FullName);
                if ( key != null )
                {
                    n = mruList.Count;

                    for ( i = 0; i < maxNumberOfFiles; i++ )
                    {
                        key.DeleteValue(regEntryName + i.ToString(), false);
                    }

                    for ( i = 0; i < n; i++ )
                    {
                        key.SetValue(regEntryName + i.ToString(), mruList[i]);
                    }
                }

            }
            catch 
            {
            }
        }


        public void LoadMRU()
        {
            string sKey, s;

            try
            {
                mruList.Clear();

                RegistryKey key = Utils.BuildRegistryKey(Registry.CurrentUser, GetType().FullName);

                if ( key != null )
                {
                    for ( int i = 0; i < maxNumberOfFiles; i++ )
                    {
                        sKey = regEntryName + i.ToString();

                        s = (string)key.GetValue(sKey, "");

                        if ( s.Length == 0 )
                            break;

                        mruList.Add(s);
                    }
                }
            }
            catch 
            {
            }
        }

        /// <summary>
        /// Имя шаблона, которое будет показываться в списке
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private string GetDisplayName(string fullName)
        {
            return GetShortDisplayName(fullName, maxDisplayLength);
        }

        /// <summary>
        /// Короткое имя шаблона
        /// </summary>
        /// <param name="longName"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        private string GetShortDisplayName(string longName, int maxLen)
        {
            StringBuilder pszOut = new StringBuilder(maxLen + maxLen + 2);  // for safety

            if ( PathCompactPathEx(pszOut, longName, maxLen, 0) )
            {
                return pszOut.ToString();
            }
            else
            {
                return longName;
            }
        }

	}
}
