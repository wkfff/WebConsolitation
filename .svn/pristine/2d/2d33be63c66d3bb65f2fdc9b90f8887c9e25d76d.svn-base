using System;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;

namespace iMonotoringWM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            ScreenOrientation customScreenOrientation = SystemSettings.ScreenOrientation;
            try
            {
                //По умолчанию будет вертикальная ориентация
                SystemSettings.ScreenOrientation = ScreenOrientation.Angle0;
                OpenNETCF.Windows.Forms.Application2.Run(new MainForm());
                //Application.Run(new MainForm());
            }
            finally
            {
                //Вернем пользователькую ориентацию экрана
                if (SystemSettings.ScreenOrientation != customScreenOrientation)
                    SystemSettings.ScreenOrientation = customScreenOrientation;
            }
        }
    }
}