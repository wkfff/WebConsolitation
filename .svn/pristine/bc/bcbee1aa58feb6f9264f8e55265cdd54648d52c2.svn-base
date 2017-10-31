using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

using Krista.FM.Common.RegistryUtils;

namespace Krista.FM.Client.Common
{   
    /// <summary>
    /// Сохранение и загрузка параметров формы из реестра
    /// </summary>
    public static class DefaultFormState
    {
        /// <summary>
        ///  Сохраняет состояние формы в реестре
        /// </summary>
        /// <param name="form">Непосредственно форма</param>
        public static void Save(Form form)
        {
            try
            {
                RegistryKey rk = Utils.BuildRegistryKey(Registry.CurrentUser, form.GetType().FullName);

                rk.SetValue("FormBorderStyle", (int)form.FormBorderStyle);
                rk.SetValue("WindowState", (int)form.WindowState);
                if (form.WindowState == FormWindowState.Normal)
                {
                    rk.SetValue("Top", form.Location.X);
                    rk.SetValue("Left", form.Location.Y);
                    rk.SetValue("Height", form.Height);
                    rk.SetValue("Width", form.Width);
                }
            }
            catch { /* Глушим все ошибки */ }
        }

        /// <summary>
        /// Загружает состояние формы из реестра
        /// </summary>
        /// <param name="form">Непосредственно форма</param>
        public static void Load(Form form)
        {
            bool formVisible = form.Visible;
            try
            {
                RegistryKey rk = Utils.BuildRegistryKey(Registry.CurrentUser, form.GetType().FullName);

                if (rk.ValueCount == 0)
                    return;


                form.StartPosition = FormStartPosition.Manual;

                form.Visible = false;
                if (rk.GetValue("FormBorderStyle") != null)
                    form.FormBorderStyle = (FormBorderStyle)(int)rk.GetValue("FormBorderStyle");
                form.WindowState = (FormWindowState)(int)rk.GetValue("WindowState");

                Point p = new Point();
                p.X = (int)rk.GetValue("Top");
                p.Y = (int)rk.GetValue("Left");
                form.Location = p;

                form.Height = (int)rk.GetValue("Height");
                form.Width = (int)rk.GetValue("Width");
            }
            catch { /* Глушим все ошибки */ }
            finally
            {
                form.Visible = formVisible;
            }
        }
    }
}
