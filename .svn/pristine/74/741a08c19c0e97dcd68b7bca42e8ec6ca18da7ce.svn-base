using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinStatusBar;
using Krista.FM.Update.Framework;
using Quartz;

namespace Krista.FM.Update.SchedulerServerUpdater
{
    /// <summary>
    /// Задача обновления
    /// </summary>
    public class UpdateJob : IJob
    {
        private static string _feed;

        public void Execute(JobExecutionContext context)
        {
            if (!String.IsNullOrEmpty(_feed))
                StartCheckUpdates(_feed);
        }

        public static void StartCheckUpdates(string feed)
        {
            _feed = feed;
            UpdateManager.Instance.StartUpdates(feed);
        }
    }

    public class JobListener : IJobListener
    {
        public delegate void SetStatusDelegate(string message, UltraStatusBar statusBar);
        public delegate void SetImageDelegate(Bitmap image, PictureBox pictureBox);
        public delegate void SetLabelDelegate(string message, Label label);

        public void JobToBeExecuted(JobExecutionContext context)
        {
            UltraStatusBar statusBar = context.JobDetail.JobDataMap["statusBar"] as UltraStatusBar;
            if (statusBar != null)
            {
                if (statusBar.InvokeRequired)
                {
                    SetStatusDelegate del = SetStatus;
                    statusBar.Invoke(del, "Задача будет выполнена", statusBar);
                }
                else
                {
                    statusBar.Text = "Задача будет выполнена";
                }
            }

            PictureBox pictureBox = context.JobDetail.JobDataMap["image"] as PictureBox;
            if (pictureBox != null)
            {
                if (pictureBox.InvokeRequired)
                {
                    SetImageDelegate del = SetImage;
                    pictureBox.Invoke(del, Properties.Resources.loading28, pictureBox);
                }
                else
                {
                    pictureBox.Image = Properties.Resources.loading28;
                }
            }
        }

        private static void SetStatus(string message, UltraStatusBar statusBar)
        {
            statusBar.Text = message;
        }

        private static void SetImage(Bitmap image, PictureBox pictureBox)
        {
            pictureBox.Image = image;
        }

        public void JobExecutionVetoed(JobExecutionContext context)
        {
        }

        public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException)
        {
            UltraStatusBar statusBar = context.JobDetail.JobDataMap["statusBar"] as UltraStatusBar;
            PictureBox pictureBox = context.JobDetail.JobDataMap["image"] as PictureBox;
            if (!Convert.ToBoolean(context.JobDetail.JobDataMap["result"]))
            {
                if (statusBar != null)
                {
                    if (statusBar.InvokeRequired)
                    {
                        SetStatusDelegate del = SetStatus;
                        statusBar.Invoke(del, String.Format("В процессе обновления были ошибки. См. предыдущие сообщения в логе!"), statusBar);
                    }
                    else
                    {
                        statusBar.Text = "В процессе обновления были ошибки. См. предыдущие сообщения в логе!";
                    }
                }

                if (pictureBox != null)
                {
                    if (pictureBox.InvokeRequired)
                    {
                        SetImageDelegate del = SetImage;
                        pictureBox.Invoke(del, Properties.Resources.update_available, pictureBox);
                    }
                    else
                    {
                        pictureBox.Image = Properties.Resources.update_available;
                    }
                }
            }
            if (statusBar != null)
            {
                if (statusBar.InvokeRequired)
                {
                    SetStatusDelegate del = SetStatus;
                    statusBar.Invoke(del, String.Format("Последнее обновление было в {0}", DateTime.Now), statusBar);
                }
                else
                {
                    statusBar.Text = String.Format("Последнее обновление было в {0}", DateTime.Now);
                }
            }

            if (pictureBox != null)
            {
                if (pictureBox.InvokeRequired)
                {
                    SetImageDelegate del = SetImage;
                    pictureBox.Invoke(del, Properties.Resources.update_uptodate, pictureBox);
                }
                else
                {
                    pictureBox.Image = Properties.Resources.update_uptodate;
                }
            }
        }

        public string Name { get; set; }

    }
}
