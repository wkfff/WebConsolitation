using System;
using System.Windows.Forms;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.SchedulerServerUpdater
{
    public partial class SchedulerSettingsForm : Form
    {
        private readonly NotifyIcon notifyIcon;
        private readonly ContextMenuStrip contextMenuStrip;
        private readonly ToolStripMenuItem miShow;
        private readonly ToolStripMenuItem miHide;

        public SchedulerSettingsForm()
        {
            InitializeComponent();

            notifyIcon = new NotifyIcon(components);
            contextMenuStrip = new ContextMenuStrip(components);
            miShow = new ToolStripMenuItem();
            miHide = new ToolStripMenuItem();

            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Icon = Properties.Resources.updateicon;
            notifyIcon.Text = "АиП - Автоматическое обновление";
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;

            contextMenuStrip.Items.AddRange(new ToolStripItem[] {
            miShow,
            miHide});
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new System.Drawing.Size(125, 48);
            // 
            // miShow
            // 
            miShow.Name = "miShow";
            miShow.Size = new System.Drawing.Size(124, 22);
            miShow.Text = "Показать";
            miShow.Click += toolStripMenuItem1_Click;
            // 
            // miHide
            // 
            miHide.Name = "miHide";
            miHide.Size = new System.Drawing.Size(124, 22);
            miHide.Text = "Скрыть";
            miHide.Click += miHide_Click;

            Timer timer = new Timer { Interval = 1000 };
            timer.Tick += timer_Tick;
            timer.Start();
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
            ShowInTaskbar = false;
            Hide();
            miShow.Enabled = true;
            miHide.Enabled = false;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            toolStripMenuItem1_Click(sender, e);
        }

        #endregion


        void timer_Tick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(UpdateManager.Instance.LastUpdate))
            {
                notifyIcon.Text = UpdateManager.Instance.LastUpdate.Length > 64
                                      ? UpdateManager.Instance.LastUpdate.Substring(0, 64)
                                      : UpdateManager.Instance.LastUpdate;
            }

        }
    }
}
