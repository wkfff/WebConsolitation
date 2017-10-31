using System;
using System.Configuration;
using System.Drawing;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Update.Framework;
using Krista.FM.Update.Framework.FeedReaders;
using System.Windows.Forms;
using Quartz.Impl;
using Quartz;
using Resources = Krista.FM.Update.SchedulerServerUpdater.Properties.Resources;

namespace Krista.FM.Update.SchedulerServerUpdater
{
    public partial class ServerUpdaterForm : Form
    {
        private ISchedulerFactory schedFact;
        private JobDetail jobDetail;

        private string feedPath;

        public ServerUpdaterForm()
        {
            if (!ReadConfiguration())
                Environment.Exit(0);

            InitializeComponent();

            // UpdateManager initialization
            UpdateManager updManager = UpdateManager.Instance;
            updManager.UpdateFeedReader = new XmlFeedReader();
            updManager.TempFolder =
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                       "NAppUpdateWinFormsSample\\Updates");
            updManager.IsServerMode = true;

            ultraStatusBar.Text = "Обновления не было";

            this.ShowInTaskbar = false;

            InitializeScheduler();

            updManager.OnCkeckUpdates += updManager_OnCkeckUpdates;
            updManager.OnPrepareUpdates += updManager_OnPrepareUpdates;
            updManager.OnAplayUpdates += updManager_OnAplayUpdates;
            
            UpdateJob.StartCheckUpdates(feedPath);
        }

        private bool ReadConfiguration()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap { ExeConfigFilename = "Krista.FM.Update.SchedulerServerUpdater.exe.config" };

            Configuration config =
               ConfigurationManager.OpenMappedExeConfiguration(fileMap,
               ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.Count < 2)
            {
                MessageBox.Show("Не указан один или несколько параметров в Krista.FM.SchedulerServerUpdater.exe.config",
                                            "Ошибка в Krista.FM.SchedulerServerUpdater.exe.config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateManager.Instance.Logger.Error("Не указан один или несколько параметров в Krista.FM.SchedulerServerUpdater.exe.config");
                return false;
            }

            foreach (KeyValueConfigurationElement appSetting in config.AppSettings.Settings)
            {
                switch (appSetting.Key)
                {
                    case "FeedPath":
                        feedPath = appSetting.Value;
                        continue;
                    case "BaseUri":
                        Uri uri;

                        if (Uri.TryCreate(appSetting.Value, UriKind.Absolute, out uri))
                            UpdateManager.Instance.BaseUrl = uri.OriginalString;
                        else
                        {
                            MessageBox.Show("Проверьте правильность параметра BaseUri в Krista.FM.SchedulerServerUpdater.exe.config",
                                            "Ошибка в Krista.FM.SchedulerServerUpdater.exe.config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        continue;
                    default:
                        UpdateManager.Instance.Logger.Error(string.Format("Не обработанный параметр в Krista.FM.SchedulerServerUpdater.exe.config: {0}",
                                                                          appSetting.Key));
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Настройка расписания обновления
        /// </summary>
        private void InitializeScheduler()
        {
            // TODO позволить настраивать расписание обновления
            schedFact = new StdSchedulerFactory();

            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            JobListener listener = new JobListener();
            listener.Name = "UpdateListener";

            sched.AddJobListener(listener);
             
            // задача обновления
            jobDetail = new JobDetail("updateJob", null, typeof(UpdateJob));
            jobDetail.JobDataMap["statusBar"] = ultraStatusBar;
            jobDetail.JobDataMap["image"] = pictureBox;
            jobDetail.JobDataMap["label"] = labelMessage;
            jobDetail.AddJobListener("UpdateListener");

            // расписание обновления
            // ежедневно в 0:00
            Trigger trigger = TriggerUtils.MakeDailyTrigger(0, 0);
            trigger.StartTimeUtc = TriggerUtils.GetEvenHourDate(DateTime.UtcNow);
            trigger.Name = "dailyTriggerUpdate";
            sched.ScheduleJob(jobDetail, trigger);

            // Тестовое расписание - ежеминутное для проверки работоспособности
            // TODO: убрать!!
            Trigger minTrigger = TriggerUtils.MakeMinutelyTrigger(1);
            minTrigger.StartTimeUtc = TriggerUtils.GetEvenMinuteDate(DateTime.UtcNow);
            minTrigger.Name = "minutelyTriggerUpdate";
            minTrigger.JobName = "updateJob";
            sched.ScheduleJob(minTrigger);
            
        }

        #region Работа с треем

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Включаем отображения приложения на панели задач при запуске
            ShowInTaskbar = true;
            //Показываем форму
            Show();
            WindowState = FormWindowState.Normal;
            //Отключаем доступность пункта меню mnuShow
            miShow.Enabled = false;
            //Включаем доступность пункта меню mnuHide
            miHide.Enabled = true;
            //Переменной Hidden устанавливаем значение false
        }

        private void miHide_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Hide();
            miShow.Enabled = true;
            miHide.Enabled = false;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            toolStripMenuItem1_Click(sender, e);
        }

        #endregion

        #region event handler

        private void btnCheck_Click(object sender, EventArgs e)
        {

            UpdateJob.StartCheckUpdates(feedPath);
        }
        
        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "Scheduler":
                    SchedulerSettingsForm form = new SchedulerSettingsForm();
                    form.Visible = false;
                    form.Activate();
                    break;
                case "Exit":
                    Close();
                    break;
            }
        }

        private void ServerUpdaterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateManager.Instance.CleanUp();

            foreach (IScheduler allScheduler in schedFact.AllSchedulers)
            {
                allScheduler.Shutdown();
            }
        }

        #region temp

        public delegate void SetStatusDelegate(string message, UltraStatusBar statusBar);
        public delegate void SetImageDelegate(Bitmap image, PictureBox pictureBox);
        public delegate void SetLabelDelegate(string message, Label label);

        #region helper

        private void SetLabelText(string message)
        {
            if (labelMessage != null)
            {
                if (labelMessage.InvokeRequired)
                {
                    SetLabelDelegate del = SetLabel;
                    labelMessage.Invoke(del, message, labelMessage);
                }
                else
                {
                    labelMessage.Text = message;
                }
            }
        }

        private void SetPictureBox(Bitmap picture)
        {
            if (pictureBox != null)
            {
                if (pictureBox.InvokeRequired)
                {
                    SetImageDelegate del = SetImage;
                    pictureBox.Invoke(del, picture, pictureBox);
                }
                else
                {
                    pictureBox.Image = picture;
                }
            }
        }

        private void SetStatusBar(string message)
        {
            if (ultraStatusBar != null)
            {
                if (ultraStatusBar.InvokeRequired)
                {
                    SetStatusDelegate del = SetStatus;
                    ultraStatusBar.Invoke(del, message, ultraStatusBar);
                }
                else
                {
                    ultraStatusBar.Text = message;
                }
            }
        }

        #endregion

        private static void SetStatus(string message, UltraStatusBar statusBar)
        {
            statusBar.Text = message;
        }

        private static void SetImage(Bitmap image, PictureBox pictureBox)
        {
            pictureBox.Image = image;
        }

        private static void SetLabel(string message, Label label)
        {
            label.Text = message;
        }

        #endregion

        void updManager_OnPrepareUpdates(bool succeeded)
        {
            if (!succeeded)
            {
                SetStatusBar("В процессе подготовки задач возникли критические ошибки. См. предыдущие сообщения в логе!");
            }
        }

        void updManager_OnCkeckUpdates(int count)
        {
            if (count == 0)
            {
                SetStatusBar(String.Format("Последнее обновление было в {0}", DateTime.Now));
                SetPictureBox(Resources.update_uptodate);
                SetLabelText("Вы используете самую последнюю версию приложения!");
            }
        }

        void updManager_OnAplayUpdates(bool obj)
        {
           if (obj)
           {
               SetStatusBar("В процессе обновления были ошибки. См. предыдущие сообщения в логе!");
               SetPictureBox(Resources.update_available);
               jobDetail.JobDataMap["result"] = false;
               return;
                
           }

           SetStatusBar(String.Format("Последнее обновление было в {0}", DateTime.Now));
           SetPictureBox(Resources.update_uptodate);
           SetLabelText("Обновления загружены!");
           jobDetail.JobDataMap["result"] = true;
        }

        #endregion

    }
}
