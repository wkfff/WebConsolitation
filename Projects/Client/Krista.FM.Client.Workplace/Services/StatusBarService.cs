using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinStatusBar;

namespace Krista.FM.Client.Workplace.Services
{
    public static class StatusBarService
    {
        private const string ServerPanelName = "Server";
        private const string SchemePanelName = "Scheme";
        private const string UserPanelName = "User";
        private const string RowsPanelName = "Rows";
        private const string MessagesPanelName = "Messages";

        private static Bitmap[] messageAnimationIcons;
        private static Timer messageAnimationTimer;

        private static UltraStatusBar statusBar;

        internal static void Initialize()
        {
            statusBar = new UltraStatusBar();

            statusBar.BorderStylePanel = Infragistics.Win.UIElementBorderStyle.Solid;
            statusBar.Location = new System.Drawing.Point(0, 550);
            statusBar.Name = "sbStatus";

            UltraStatusPanel ultraStatusPanel1 = new UltraStatusPanel();
            ultraStatusPanel1.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;

            ultraStatusPanel1.Text = "Сервер: ...";
            ultraStatusPanel1.Key = ServerPanelName;
            ultraStatusPanel1.WrapText = Infragistics.Win.DefaultableBoolean.False;
            
            UltraStatusPanel ultraStatusPanel2 = new UltraStatusPanel();
            ultraStatusPanel2.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            ultraStatusPanel2.Text = "Схема: ...";
            ultraStatusPanel2.Key = SchemePanelName;
            ultraStatusPanel2.WrapText = Infragistics.Win.DefaultableBoolean.False;

            UltraStatusPanel ultraStatusPanel3 = new UltraStatusPanel();
            ultraStatusPanel3.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            ultraStatusPanel3.Text = "Пользователь: ...";
            ultraStatusPanel3.Key = UserPanelName;
            ultraStatusPanel3.WrapText = Infragistics.Win.DefaultableBoolean.False;

            UltraStatusPanel ultraStatusPanel4 = new UltraStatusPanel();
            ultraStatusPanel4.Key = RowsPanelName;
            ultraStatusPanel4.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            ultraStatusPanel4.Text = "Запись: ...";

            UltraStatusPanel ultraStatusPanel5 = new UltraStatusPanel();
            ultraStatusPanel5.Key = MessagesPanelName;
            ultraStatusPanel5.SizingMode = PanelSizingMode.Automatic;
            ultraStatusPanel5.Text = "Новых сообщений:...";
            ultraStatusPanel5.Appearance.Image = Bitmaps.message;
            ultraStatusPanel5.Visible = false;

            statusBar.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[] {
                ultraStatusPanel1,
                ultraStatusPanel2,
                ultraStatusPanel3,
                ultraStatusPanel4,
                ultraStatusPanel5});

            statusBar.ResizeStyle = Infragistics.Win.UltraWinStatusBar.ResizeStyle.Deferred;
            statusBar.Size = new System.Drawing.Size(931, 23);
            statusBar.TabIndex = 6;
            statusBar.ViewStyle = Infragistics.Win.UltraWinStatusBar.ViewStyle.VisualStudio2005;
            statusBar.WrapText = false;

            statusDelegateText = SetTextToStatusBar;
            statusDelegateVisible = SetVisibleToStatusBar;

            Bitmap[] bitmaps = new[]
                                   {
                                       Bitmaps.message,
                                       Bitmaps.message_accept
                                   };
            SetAnimationClip(bitmaps);
            messageAnimationTimer = new Timer();
            messageAnimationTimer.Tick += messageAnimationTimer_Tick;
        }

        public static Control Control
        {
            get
            {
                System.Diagnostics.Debug.Assert(statusBar != null);
                return statusBar;
            }
        }

        public static bool Visible
        {
            get
            {
                System.Diagnostics.Debug.Assert(statusBar != null);
                return statusBar.Visible;
            }
            set
            {
                System.Diagnostics.Debug.Assert(statusBar != null);
                statusBar.Visible = value;
            }
        }

        public static void SetServerName(string text)
        {
            statusBar.Panels[ServerPanelName].Text = text;
        }

        public static void SetSchemeName(string text)
        {
            statusBar.Panels[SchemePanelName].Text = text;
        }

        public static void SetUserName(string text)
        {
            statusBar.Panels[UserPanelName].Text = text;
        }

        #region messages

        public static void SetNewMessagesCount(string text)
        {
            if (statusBar.IsHandleCreated)
            {
                statusBar.Invoke(statusDelegateText, new object[] {statusBar, text, MessagesPanelName});
            }
        }

        public static void SetMessagesPanelVisible(bool visible)
        {
            if (statusBar.IsHandleCreated)
            {
                statusBar.Invoke(statusDelegateVisible, new object[] { statusBar, visible, MessagesPanelName });
            }
        }

        #endregion

        private delegate void SetStatusPanelText(UltraStatusBar statusBar, string text, string panelName);
        private delegate void SetStatusPanelVisible(UltraStatusBar statusBar, bool visible, string panelName);

        private static SetStatusPanelText statusDelegateText;
        private static SetStatusPanelVisible statusDelegateVisible;

        private static void SetTextToStatusBar(UltraStatusBar ultraStatusBar, string text, string panelName)
        {
            ultraStatusBar.Panels[panelName].Text = text;
        }

        private static void SetVisibleToStatusBar(UltraStatusBar ultraStatusBar, bool visible, string panelName)
        {
            ultraStatusBar.Panels[panelName].Visible = visible;
            if (visible) StartAnimation(300); else StopAnimation();
        }

        public static void SetRowPosition(string text)
        {
            statusBar.Invoke(statusDelegateText, new object[] { statusBar, text, RowsPanelName });
        }

        #region message panel work

        private static void SetAnimationClip(Bitmap[] bitmaps)
        {
            messageAnimationIcons = new Bitmap[bitmaps.Length];
            for (int i = 0; i < bitmaps.Length; i++)
            {
                messageAnimationIcons[i] = bitmaps[i];
            }
        }

        private static void SetAnimationClip(Bitmap bitmapStrip)
        {
            messageAnimationIcons = new Bitmap[bitmapStrip.Width/16];
            for (int i = 0; i < messageAnimationIcons.Length; i++)
            {
                Rectangle rect = new Rectangle(i*16, 0, 16, 16);
                Bitmap bmp = bitmapStrip.Clone(rect, bitmapStrip.PixelFormat);
                messageAnimationIcons[i] = bmp;
            }
        }

        private static void StartAnimation(int interval)
        {
            if (messageAnimationIcons == null)
            {
                return;
            }

            messageAnimationTimer.Interval = interval;
            messageAnimationTimer.Start();
        }

        private static void StopAnimation()
        {
            messageAnimationTimer.Stop();
        }

        private static int messageCurrentIndex = 0;
        private static void messageAnimationTimer_Tick(object sender, EventArgs e)
        {
            if (messageCurrentIndex < messageAnimationIcons.Length)
            {
                statusBar.Panels["Messages"].Appearance.Image = messageAnimationIcons[messageCurrentIndex];
                messageCurrentIndex++;
            }
            else
            {
                messageCurrentIndex = 0;
            }
        }

        #endregion

    }
}
