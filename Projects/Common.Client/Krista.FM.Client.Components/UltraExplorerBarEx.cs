using System;
using System.Drawing;
using System.Threading;
using Infragistics.Win;
using Infragistics.Win.UltraWinExplorerBar;

namespace Krista.FM.Client.Components
{
    /// <summary>
    /// Расширенный UltraExplorerBarEx
    /// </summary>
    public class UltraExplorerBarEx : UltraExplorerBar
    {
        private Thread objThreadBlinkTab;
        private delegate void BlinkUltraExplorerBarGroupDelegate(UltraExplorerBarGroup ultraExplorerBarGroup);

        private bool IsThreadWorking
        {
            get
            {
                return objThreadBlinkTab != null && objThreadBlinkTab.IsAlive;
            }
        }

        /// <summary>
        /// Вызывает мерцание группы UltraExplorerBar, если она неактивна и если 
        /// используем стиль OutlookNavigationPane.
        /// Если будем использовать какой-то другой стиль, то для него надо будет подобрать другие цвета мерцания.
        /// </summary>
        /// <param name="barGroup">Группа, для которой вызываем мерцание</param>
        public void BlinkUltraExplorerBarGroup(UltraExplorerBarGroup barGroup)
        {
            if (!IsThreadWorking)
            {
                if (Style == UltraExplorerBarStyle.OutlookNavigationPane)
                {
                    objThreadBlinkTab = new Thread(InnerBlinkUltraExplorerBarGroup);
                    objThreadBlinkTab.IsBackground = true;
                    objThreadBlinkTab.Start(barGroup);
                }
            }
        }

        /// <summary>
        /// Мерцаем, пока группа не станет активной.
        /// </summary>
        /// <param name="barGroupObj">UltraExplorerBarGroup</param>
        private void InnerBlinkUltraExplorerBarGroup(object barGroupObj)
        {
            UltraExplorerBarGroup barGroup = barGroupObj as UltraExplorerBarGroup;

            if (barGroup != null)
            {
                try
                {
                    Monitor.Enter(barGroupObj);
                    {
                        bool blnIsOriginal = true;

                        while (!barGroup.Selected)
                        {
                            try
                            {
                                if (blnIsOriginal)
                                {
                                    BlinkUltraExplorerBarGroupDelegate buebgd = ChangeTabColor;
                                    Invoke(buebgd, barGroup);
                                    blnIsOriginal = false;
                                    Thread.Sleep(500);
                                }
                                else
                                {
                                    blnIsOriginal = true;
                                    BlinkUltraExplorerBarGroupDelegate buebgd = RestoreTabColor;
                                    Invoke(buebgd, barGroup);
                                    Thread.Sleep(500);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                    Monitor.Exit(barGroupObj);
                    BlinkUltraExplorerBarGroupDelegate buebgd = RestoreTabColor;
                    Invoke(buebgd, barGroup);
                }
            }
        }

        private void ChangeTabColor(UltraExplorerBarGroup ultraExplorerBarGroup)
        {
            ultraExplorerBarGroup.Settings.AppearancesLarge.HeaderAppearance.BackColor =
                                        Office2003Colors.OutlookNavPanePressedOverflowButtonGradientDark;
            ultraExplorerBarGroup.Settings.AppearancesSmall.NavigationOverflowButtonAppearance.BackColor =
                                        Office2003Colors.OutlookNavPanePressedOverflowButtonGradientDark;
            ultraExplorerBarGroup.Settings.AppearancesLarge.HeaderAppearance.BackColor2 =
                                        Office2003Colors.OutlookNavPanePressedOverflowButtonGradientLight;
            ultraExplorerBarGroup.Settings.AppearancesSmall.NavigationOverflowButtonAppearance.BackColor2 =
                                        Office2003Colors.OutlookNavPanePressedOverflowButtonGradientLight;
        }

        private void RestoreTabColor(UltraExplorerBarGroup ultraExplorerBarGroup)
        {
            ultraExplorerBarGroup.Settings.AppearancesLarge.HeaderAppearance.BackColor = Color.Empty;
            ultraExplorerBarGroup.Settings.AppearancesLarge.HeaderAppearance.BackColor2 = Color.Empty;
            ultraExplorerBarGroup.Settings.AppearancesSmall.NavigationOverflowButtonAppearance.BackColor = Color.Empty;
            ultraExplorerBarGroup.Settings.AppearancesSmall.NavigationOverflowButtonAppearance.BackColor2 = Color.Empty;
        }
    }
}
