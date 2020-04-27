﻿using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.ImageFunctions;

namespace DirectXInput
{
    public partial class WindowOverlay : Window
    {
        //Window Initialize
        public WindowOverlay() { InitializeComponent(); }

        //Window Variables
        public static IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get application interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Update the window style
                UpdateWindowStyle();

                //Update the window and text position
                UpdateWindowPosition();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
            }
            catch { }
        }

        //Update the window position on resolution change
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Update the window and text position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Update the window style
        void UpdateWindowStyle()
        {
            try
            {
                //Set the window style
                IntPtr UpdatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, UpdatedStyle);

                //Set the window style ex
                IntPtr UpdatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, UpdatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                Debug.WriteLine("The window style has been updated.");
            }
            catch { }
        }

        //Update the window and text position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                System.Windows.Forms.Screen targetScreen = GetActiveScreen();

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Set the window size
                    this.Width = targetScreen.Bounds.Width;
                    this.Height = targetScreen.Bounds.Height;

                    //Move the window top left
                    this.Left = targetScreen.Bounds.Left;
                    this.Top = targetScreen.Bounds.Top;
                });
            }
            catch { }
        }

        //Get the current active screen
        public System.Windows.Forms.Screen GetActiveScreen()
        {
            try
            {
                //Get default monitor
                int MonitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);

                //Get the target screen
                if (MonitorNumber > 0)
                {
                    try
                    {
                        return System.Windows.Forms.Screen.AllScreens[MonitorNumber];
                    }
                    catch
                    {
                        return System.Windows.Forms.Screen.PrimaryScreen;
                    }
                }
            }
            catch { }
            return System.Windows.Forms.Screen.PrimaryScreen;
        }

        //Show the status overlay
        public void Overlay_Show_Status(string IconName, string Message)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + IconName + ".png" }, IntPtr.Zero, -1, 0);
                    grid_Message_Status_Text.Text = Message;
                    stackpanel_Message_Status.Visibility = Visibility.Visible;
                });

                vDispatcherTimer.Stop();
                vDispatcherTimer.Interval = TimeSpan.FromSeconds(3);
                vDispatcherTimer.Tick += delegate
                {
                    stackpanel_Message_Status.Visibility = Visibility.Collapsed;
                    vDispatcherTimer.Stop();
                };
                vDispatcherTimer.Start();
            }
            catch { }
        }
    }
}