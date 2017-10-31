using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Krista.FM.Update.Framework.Forms;

namespace Krista.FM.Update.Framework.Controls
{
    public partial class NotifyIconControl : UserControl
    {
        #region Fields

        private IUpdateManager manager;
        private UpdateProcessState state = UpdateProcessState.NotChecked;
        private bool asRemote = true;
        private PatchListForm patchListForm;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem checkUpdates;
        private ToolStripMenuItem patchListMenu;
        private ToolStripMenuItem receivedPatchListMenu;
        private ToolStripMenuItem remind1;
        private ToolStripMenuItem remind3;
        private ToolStripMenuItem noremind;
        private ToolStripMenuItem openLogsFolder;
        private ToolStripMenuItem exit;
        private ToolStripMenuItem applayUpdates;
        private ToolStripMenuItem options;
        private Timer remindTimer;
        private string baloonHeader;
        private bool showbaloon;
        private bool notifyErrorShow = true;
        private INotifierClient client;

        #endregion

        #region Properties

        public IUpdateManager Manager
        {
            get { return manager; }
            set
            {
                manager = value;

                if (value != null)
                {
                    notifyIcon.Icon = GetIcon();
                    AttachClient();
                }
            }
        }

        public bool AsRemote
        {
            get { return asRemote; }
            set { asRemote = value; }
        }

        private void CorrectMenu()
        {
            try
            {
                if (Manager != null && Manager.KristaApplication == KristaApp.OfficeAddIn)
                {
                    exit.Visible = false;
                }

                receivedPatchListMenu.Visible = (Manager == null) ? true : Manager.IsServerMode;
            }
            catch (RemotingException)
            {
                NotifyErrorNotConnected();
            }
        }

        public NotifyIcon NotifyIcon
        {
            get { return notifyIcon; }
            set { notifyIcon = value; }
        }

        #endregion

        #region Constructor

        public NotifyIconControl()
        {
            InitializeComponent();
            InitializeContextMenu();
        }

        #endregion

        private void AttachClient()
        {
            if (Manager == null)
                return;

            if (asRemote)
            {
                client = new RemoteClientWrapper();
                ((RemoteClientWrapper)client).ReceiveNewStateEvent += Client_ReceiveNewStateEvent;
                Manager.AttachClient(client);
            }
        }


        private void Client_ReceiveNewStateEvent(object sender, UpdateProcessStateArgs e)
        {
            ReceiveNewState(e.State);
        }

        private void InitializeContextMenu()
        {
            remindTimer = new Timer() {Interval = 1000*60*30};
            remindTimer.Tick += t_Tick;

            notifyIcon = new NotifyIcon(components);
            notifyIcon.BalloonTipClicked += NotifyIconBalloonTipClicked;
            notifyIcon.MouseClick += new MouseEventHandler(NotifyIconMouseClick);
            contextMenuStrip = new ContextMenuStrip(components);
            checkUpdates = new ToolStripMenuItem();
            patchListMenu = new ToolStripMenuItem();
            receivedPatchListMenu = new ToolStripMenuItem();
            remind1 = new ToolStripMenuItem();
            remind3 = new ToolStripMenuItem();
            noremind = new ToolStripMenuItem();
            exit = new ToolStripMenuItem();
            applayUpdates = new ToolStripMenuItem();
            openLogsFolder = new ToolStripMenuItem();
            options = new ToolStripMenuItem();

            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Text = "АиП - Автоматическое обновление";
            notifyIcon.Visible = true;

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                                                {
                                                    receivedPatchListMenu, patchListMenu, checkUpdates, applayUpdates, openLogsFolder,
                                                    remind1, remind3, noremind,
                                                    new ToolStripSeparator(), options, exit
                                                });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(125, 48);
            //
            //  receivedPatchListMenu
            //
            receivedPatchListMenu.Name = "receivedPatchListMenu";
            receivedPatchListMenu.Size = new Size(124, 22);
            receivedPatchListMenu.Text = "Список полученных обновлений";
            receivedPatchListMenu.Click += ReceivedPatchListMenu_Click;
            receivedPatchListMenu.Image = Resources.list_received;
            //
            //  patchListMenu
            //
            patchListMenu.Name = "patchListMenu";
            patchListMenu.Size = new Size(124, 22);
            patchListMenu.Text = "Список установленных обновлений";
            patchListMenu.Click += patchListMenu_Click;
            patchListMenu.Image = Resources.list;
            //
            //
            //
            checkUpdates.Name = "checkUpdates";
            checkUpdates.Size = new Size(124, 22);
            checkUpdates.Text = "Проверить обновления";
            checkUpdates.Click += check_Click;
            checkUpdates.Image = Resources.checkUpdates;


            applayUpdates.Name = "applayUpdates";
            applayUpdates.Size = new Size(124, 22);
            applayUpdates.Text = "Установить обновления";
            applayUpdates.Click += ApplayClick;
            applayUpdates.Visible = false;
            applayUpdates.Image = Resources.install;
            //
            // remind
            //
            remind1.Name = "remind1";
            remind1.Size = new Size(124, 22);
            remind1.Text = "Напомнить через 1 час";
            remind1.Click += Remind1Click;
            remind1.Visible = false;

            remind3.Name = "remind3";
            remind3.Size = new Size(124, 22);
            remind3.Text = "Напомнить через 3 часа";
            remind3.Click += Remind1Click;
            remind3.Visible = false;

            noremind.Name = "noremind";
            noremind.Size = new Size(124, 22);
            noremind.Text = "Напомнить при перезапуске клиента";
            noremind.Click += Remind1Click;
            noremind.Visible = false;

            openLogsFolder.Name = "openLogsFolder";
            openLogsFolder.Size = new Size(124, 22);
            openLogsFolder.Text = "Открыть папку с логами обновления";
            openLogsFolder.Click += OpenLogsFolderClick;
            openLogsFolder.Visible = true;

            options.Name = "options";
            options.Size = new Size(124, 22);
            options.Text = "Настройка...";
            options.Click += OptionsClick;
            options.Visible = true;
            options.Image = Resources.options;
            //
            // exit
            //
            exit.Name = "exitListMenu";
            exit.Size = new Size(124, 22);
            exit.Text = "Выход";
            exit.Click += ExitClick;
        }

        void NotifyIconMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CorrectMenu();
            }
        }

        private void OptionsClick(object sender, EventArgs e)
        {
            ConfigEditorForm form = new ConfigEditorForm(Manager);
            form.ShowDialog();
        }

        private void ApplayClick(object sender, EventArgs e)
        {
            AplayUpdates();
        }

        private static void OpenLogsFolderClick(object sender, EventArgs e)
        {
            OpenLogsFolder();
        }

        private void NotifyIconBalloonTipClicked(object sender, EventArgs e)
        {
            if (manager != null)
            {
                switch (state)
                {
                    case UpdateProcessState.Prepared:
                        {
                            AplayUpdates();
                            break;
                        }
                    case UpdateProcessState.Error:
                        {
                            OpenLogsFolder();
                            break;
                        }
                }

                showbaloon = false;
            }
        }

        private void AplayUpdates()
        {
            IList<IUpdatePatch> patchs = new List<IUpdatePatch>();
            foreach (
                var updateFeed in
                    manager.Feeds.Values.Where(updateFeed => !updateFeed.IsBase).Where(i => i.Patches.Count() > 0))
            {
                foreach (IUpdatePatch updatePatch in updateFeed.UpdatesToApply)
                {
                    patchs.Add(updatePatch);
                }
            }

            if (patchs.Count > 0)
            {
                if (patchListForm != null)
                {
                    return;
                }

                manager.ApplyUpdates();
            }
        }

        private static void OpenLogsFolder()
        {
            string myDocspath = Path.Combine(Path.GetDirectoryName(UpdateManager.GetProcessModule().FileName),
                                             "UpdateLog");
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            var prc = new Process();
            prc.StartInfo.FileName = windir + @"\explorer.exe";
            prc.StartInfo.Arguments = myDocspath;
            prc.Start();
        }

        private void ExitClick(object sender, EventArgs e)
        {
            try
            {
                if (manager.KristaApplication == KristaApp.OfficeAddIn)
                {
                    CloseCurrentProcess(Process.GetCurrentProcess().ProcessName);
                    return;
                }
            }
            catch {}
            finally
            {
                Environment.Exit(0);
            }
        }

        internal static void CloseCurrentProcess(string processName)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.StartsWith(processName))
                {
                    try
                    {
                        clsProcess.Kill();
                    }
                    catch (Win32Exception e)
                    {
                        System.Diagnostics.Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                    catch (NotSupportedException e)
                    {
                        System.Diagnostics.Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                    catch (InvalidOperationException e)
                    {
                        System.Diagnostics.Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                    catch (SystemException e)
                    {
                        System.Diagnostics.Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                }
            }
        }

        private Icon GetIcon()
        {
            if (Manager == null)
            {
                baloonHeader = "Криста Сервер Обновления";
                return Resources.updateicon;
            }

            switch (manager.KristaApplication)
            {
                case KristaApp.Updater:
                    baloonHeader = "Криста Сервер Обновления";
                    return Resources.updateicon;
                case KristaApp.SchemeDesigner:
                    baloonHeader = "Криста Дизайнер Схем";
                    return Resources.schemeDesigner;
                case KristaApp.Workplace:
                    baloonHeader = "Криста WorkPlace";
                    return Resources.workplace;
                case KristaApp.OlapAdmin:
                    baloonHeader = "Криста OlapAdmin";
                    return Resources.olapAdmin;
                case KristaApp.OfficeAddIn:
                    baloonHeader = "Криста Office Add-in";
                    return Resources.OfficeAddIn;
                case KristaApp.MDXExpert:
                    baloonHeader = "Криста MDXExpert";
                    return Resources.MDXExpert;
                default:
                    baloonHeader = "Неизвестное приложение";
                    return Resources.updateicon;
            }
        }

        private void Remind1Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                if (remindTimer.Enabled)
                    remindTimer.Stop();

                switch (item.Name)
                {
                    case "remind1":
                        remindTimer.Interval = 3600000;
                        remindTimer.Start();
                        break;
                    case "remind3":
                        remindTimer.Interval = 10800000;
                        remindTimer.Start();
                        break;
                }
            }
        }

        private void ReceivedPatchListMenu_Click(object sender, EventArgs e)
        {
            if (!CheckRemotingConnection())
            {
                return;
            }

            if (Manager != null)
            {
                if (patchListForm != null)
                    return;

                patchListForm = new PatchListForm(manager.GetReceivedPatchList(), false,
                                                  manager.IsServerMode, true);
                patchListForm.Text = "Список полученных обновлений";
                DialogResult result = patchListForm.ShowDialog(this);

                if (result == DialogResult.Abort)
                {
                    SaveFileDialog sfdExportToxcel = new SaveFileDialog();

                    ExportToExcel exp = new ExportToExcel();
                    sfdExportToxcel.FileName = String.Format("{0} - Список установленных обновлений.xls",
                                                             manager.KristaApplication);
                    sfdExportToxcel.Filter = "Файлы Excel (*.xls)|*.xls|Все файлы (*.*)|*.*";
                    sfdExportToxcel.FilterIndex = 1;
                    sfdExportToxcel.RestoreDirectory = true;

                    if (sfdExportToxcel.ShowDialog() == DialogResult.OK)
                    {
                        string path = sfdExportToxcel.FileName;
                        exp.DataGridView2Excel(patchListForm.PatchListControl.DataGridView, path, "PatchesSheet");
                    }

                    DisposePatchForm();
                }
                else if (result == DialogResult.Cancel)
                {
                    DisposePatchForm();
                }
            }
        }

        private void patchListMenu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckRemotingConnection()) return;

                if (Manager != null)
                {
                    if (patchListForm != null)
                        return;

                    patchListForm = new PatchListForm(Manager.GetPatchList(), false, Manager.IsServerMode, false);
                    DialogResult result = patchListForm.ShowDialog(this);

                    if (result == DialogResult.Ignore)
                    {
                        if (patchListForm.PatchListControl.DataGridView.SelectedRows.Count == 0)
                        {
                            MessageBox.Show("Не выбрано ни одного патча для отката", "Нет выбранного патча",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                            DisposePatchForm();
                            return;
                        }

                        UpdateItemView itemView =
                            patchListForm.PatchListControl.DataGridView.SelectedRows[0].Tag as UpdateItemView;
                        if (itemView == null)
                        {
                            DisposePatchForm();
                            return;
                        }

                        if (
                            MessageBox.Show("Вы действительно хотите откатить выбранное обновление?", "Откат обновлений",
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            patchListForm.PatchListControl.Patches.Remove(itemView.Patch);

                            itemView.Patch.RollbackAsync(null);
                        }

                        DisposePatchForm();
                    }
                    else if (result == DialogResult.Abort)
                    {
                        SaveFileDialog sfdExportToxcel = new SaveFileDialog();

                        ExportToExcel exp = new ExportToExcel();
                        sfdExportToxcel.FileName = String.Format("{0} - Список установленных обновлений.xls",
                                                                 manager.KristaApplication);
                        sfdExportToxcel.Filter = "Файлы Excel (*.xls)|*.xls|Все файлы (*.*)|*.*";
                        sfdExportToxcel.FilterIndex = 1;
                        sfdExportToxcel.RestoreDirectory = true;

                        if (sfdExportToxcel.ShowDialog() == DialogResult.OK)
                        {
                            string path = sfdExportToxcel.FileName;
                            exp.DataGridView2Excel(patchListForm.PatchListControl.DataGridView, path, "PatchesSheet");
                        }

                        DisposePatchForm();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        DisposePatchForm();
                    }
                }
            }
            catch (RemotingException)
            {
                NotifyErrorNotConnected();
            }
        }

        private void DisposePatchForm()
        {
            patchListForm.Dispose();
            patchListForm = null;
        }

        public bool Connect()
        {
            try
            {
                client = new RemoteClientWrapper();
                manager = UpdateManagerFactory.CreateUpdateManager(true);
                ((RemoteClientWrapper)client).ReceiveNewStateEvent += NotifyIconControl_ReceiveNewStateEvent;
                manager.AttachClient(client);
                notifyErrorShow = true;
                EnableDisableContextMenu(true);
                return true;
            }
            catch (FrameworkRemotingException e)
            {
                using (var writer =
                    new StreamWriter(string.Format("{0}\\CrashNotify.txt",
                                                   Path.GetDirectoryName(UpdateManager.GetProcessModule().FileName))))
                {
                    writer.Write(e.InnerException);
                    NotifyErrorNotConnected();
                    return false;
                }
            }
        }

        private void NotifyIconControl_ReceiveNewStateEvent(object sender, UpdateProcessStateArgs e)
        {
            ReceiveNewState(e.State);
        }

        private void NotifyErrorNotConnected()
        {
            if (notifyErrorShow)
            {
                notifyIcon.Text = "Не запущена служба автоматического обновления";
                notifyIcon.BalloonTipTitle = "Криста Сервер Обновления";
                notifyIcon.BalloonTipText = "Не запущена служба автоматического обновления";
                notifyIcon.ShowBalloonTip(10);
                notifyErrorShow = false;
                EnableDisableContextMenu(false);
                Manager = null;
                CleanUp();
            }
        }

        private void EnableDisableContextMenu(bool enable)
        {
            checkUpdates.Enabled = enable;
            patchListMenu.Enabled = enable;
            receivedPatchListMenu.Enabled = enable;
            openLogsFolder.Enabled = enable;
            options.Enabled = enable;
        }

        private void check_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckRemotingConnection()) return;

                CleanUp();
                manager.StartUpdates();
                showbaloon = true;
            }
            catch (RemotingException)
            {
                NotifyErrorNotConnected();
            }
        }

        private bool CheckRemotingConnection()
        {
            if (Manager == null)
            {
                if (!Connect())
                {
                    return false;
                }
            }

            return true;
        }

        private void t_Tick(object sender, EventArgs e)
        {
            string text = "Обновления готовы к установке. Установить сейчас?";
            if (state == UpdateProcessState.Prepared)
            {
                notifyIcon.Text = text;
                notifyIcon.BalloonTipText = text;
                notifyIcon.ShowBalloonTip(10);
            }
        }

        private void CleanUp()
        {
            state = UpdateProcessState.NotChecked;

            remind1.Visible = false;
            remind3.Visible = false;
            noremind.Visible = false;
            applayUpdates.Visible = false;

            if (remindTimer.Enabled)
                remindTimer.Stop();
        }

        public void ReceiveNewState(UpdateProcessState updateProcessState)
        {
            try
            {
                bool baloon = true;
                if (!CheckRemotingConnection()) 
                    return;

                checkUpdates.Enabled =
                    !(updateProcessState == UpdateProcessState.Checked ||
                      updateProcessState == UpdateProcessState.Prepared);

                if (state == updateProcessState)
                    return;

                state = updateProcessState;

                string text = String.Empty;

                switch (updateProcessState)
                {
                    case UpdateProcessState.LastVersion:
                        text = "У вас последняя версия приложения " + DateTime.Now;
                        baloon = false;
                        break;
                    case UpdateProcessState.Prepared:
                        if (!manager.AutoUpdateMode)
                        {
                            text = "Обновления готовы к установке. Установить сейчас?";
                            remind1.Visible = true;
                            remind3.Visible = true;
                            noremind.Visible = true;
                            applayUpdates.Visible = true;

                            remindTimer.Start();
                        }
                        break;

                    case UpdateProcessState.AppliedSuccessfully:
                        text = String.Format("Получены новые обновления {0}", DateTime.Now);
                        break;
                    case UpdateProcessState.Error:
                        text = String.Format("Обновление завершено с ошибками. См. логи");
                        break;
                    case UpdateProcessState.Warning:
                        text = String.Format("Обновление завершено успешно с предупреждениями. См. логи");
                        break;
                    case UpdateProcessState.WaitRestart:
                        remind1.Visible = false;
                        remind3.Visible = false;
                        noremind.Visible = false;
                        applayUpdates.Visible = false;
                        checkUpdates.Enabled = false;
                        patchListMenu.Enabled = false;
                        break;
                }

                if (String.IsNullOrEmpty(text))
                    return;

                notifyIcon.Text = text;
                notifyIcon.BalloonTipTitle = baloonHeader;
                notifyIcon.BalloonTipText = text;
                if (showbaloon || baloon)
                {
                    notifyIcon.ShowBalloonTip(10);
                    showbaloon = false;
                }
            }
            catch (RemotingException)
            {
                NotifyErrorNotConnected();
            }
        }
    }
}
