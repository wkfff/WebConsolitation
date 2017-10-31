using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Krista.FM.Common.OfficeHelpers
{
    public abstract class OfficeApplication : DisposableObject, IOfficeApplication
    {
        private readonly object officeApp;

        protected OfficeApplication(object officeApp)
        {
            this.officeApp = officeApp;
        }

        public object OfficeApp
        {
            get { return officeApp; }
        }

        /// <summary>
        /// Создание документа по шаблону.
        /// </summary>
        public abstract OfficeDocument CreateAsTemplate(string templatePath);

        public abstract object CreateEmptyDocument(string fileName);

        public abstract object LoadFile(string fileName, bool openReadOnly);


        /// <summary>
        /// Абстрактный метод проверки соответствия расширения файла типу приложения.
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns></returns>
        protected abstract bool CheckFileExt(string fileName);

        /// <summary>
        /// Проверка имени файла на существование и принадлежность к объекту данного типа (по расширению).
        /// Используется для проверки параметров функций
        /// </summary>
        /// <param name="fileName">имя файла</param>
        private void CheckFile(string fileName)
        {
            // проверяем принадлежность
            if (!CheckFileExt(fileName))
                throw new Exception("Файл '" + fileName + "' имеет неверный формат");
            // проверяем наличие файла
            if (!File.Exists(fileName))
                throw new Exception("Файл '" + fileName + "' не существует");
        }

        public object OpenFile(string fileName, bool openReadOnly, bool show)
        {
            // проверяем корректность файла
            CheckFile(fileName);
            
            // загружаем файл
            LoadFile(fileName, openReadOnly);
            
            // показываем Excel если необходимо
            Visible = show;

            return this;
        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        public abstract void SaveChanges(object docObj, string fileName);

        public abstract string GetExtension();

        /// <summary>
        /// Достает из реестра ProgId и возвращает цифру версии
        /// </summary>
        public abstract int GetVersionNumber();


        #region Выполнение макросов

        public object RunMacros(string macrosName)
        {
            object[] paramArray = new object[31];
            paramArray[0] = macrosName;
            for (int i = 1; i <= 30; i++)
            {
                paramArray[i] = Type.Missing;
            }
            try
            {
                return ReflectionHelper.CallMethod(OfficeApp, "Run", paramArray);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        
        public object RunMacros(string macrosName, object rs)
        {
            object[] paramArray = new object[31];
            paramArray[0] = macrosName;
            paramArray[1] = rs;
            for (int i = 2; i <= 30; i++)
            {
                paramArray[i] = Type.Missing;
            }

            return ReflectionHelper.CallMethod(OfficeApp, "Run", paramArray);
        }

        #endregion Выполнение макросов

        #region Свойства офисного приложения

        /// <summary>
        /// Видимость офисного приложения.
        /// </summary>
        public bool Visible
        {
            get { return (bool)ReflectionHelper.GetProperty(OfficeApp, "Visible", null); }
            set { ReflectionHelper.SetProperty(OfficeApp, "Visible", value); }
        }

        /// <summary>
        /// Запретить/разрешить обновление окна.
        /// </summary>
        public bool ScreenUpdating
        {
            get { return (bool)ReflectionHelper.GetProperty(OfficeApp, "ScreenUpdating", null); }
            set { ReflectionHelper.SetProperty(OfficeApp, "ScreenUpdating", value); }
        }

        public bool Interactive
        {
            get { return (bool)ReflectionHelper.GetProperty(OfficeApp, "Interactive", null); }
            set { ReflectionHelper.SetProperty(OfficeApp, "Interactive", value); }
        }

        public virtual bool DisplayAlerts
        {
            get { return (bool)ReflectionHelper.GetProperty(OfficeApp, "DisplayAlerts", null); }
            set { ReflectionHelper.SetProperty(OfficeApp, "DisplayAlerts", value); }
        }

        #endregion Свойства офисного приложения

        #region Методы офисного приложения

        public abstract void Activate();

        public virtual void Deactivate()
        {
        }

        public virtual void Quit()
        {
            ReflectionHelper.CallMethod(OfficeApp, "Quit");
        }

        #endregion Методы офисного приложения

        #region Деструктор

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if ((OfficeApp != null) && (Marshal.IsComObject(OfficeApp)))
                {
                    Marshal.ReleaseComObject(OfficeApp);
                    GC.GetTotalMemory(true);
                }
            }

            base.Dispose(disposing);
        }

        #endregion Деструктор

        #region методы, импортированные из внешних COM библиотек

        [DllImport("user32.dll")]
        protected static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("User32.dll")]
        protected static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        protected static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        protected static extern bool IsIconic(IntPtr hWnd);

// ReSharper disable InconsistentNaming
        protected const int SW_RESTORE = 9;
        protected const int SW_MAXIMIZE = 3;
// ReSharper restore InconsistentNaming

        #endregion
    }
}
