﻿using ArnoldVinkCode;
using AVForms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Display a certain grid page
        void ShowGridPage(FrameworkElement elementTarget)
        {
            try
            {
                grid_Connection.Visibility = Visibility.Collapsed;
                grid_Controller.Visibility = Visibility.Collapsed;
                grid_Keyboard.Visibility = Visibility.Collapsed;
                grid_Keypad.Visibility = Visibility.Collapsed;
                grid_Settings.Visibility = Visibility.Collapsed;
                grid_Shortcuts.Visibility = Visibility.Collapsed;
                grid_Help.Visibility = Visibility.Collapsed;
                elementTarget.Visibility = Visibility.Visible;
            }
            catch { }
        }

        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                //Make sure the correct window style is set
                StateChanged += CheckWindowStateAndStyle;

                //Main menu functions
                lb_Menu.PreviewKeyUp += lb_Menu_KeyPressUp;
                lb_Menu.PreviewMouseUp += lb_Menu_MousePressUp;

                //Connection functions
                button_Controller0.Click += Button_Controller0_Click;
                button_Controller1.Click += Button_Controller1_Click;
                button_Controller2.Click += Button_Controller2_Click;
                button_Controller3.Click += Button_Controller3_Click;
                btn_SearchNewControllers.Click += Btn_SearchNewControllers_Click;
                Btn_IgnoreController.Click += Btn_IgnoreController_Click;
                Btn_AllowIgnoredControllers.Click += Btn_AllowIgnoredControllers_Click;
                btn_DisconnectController.Click += Btn_DisconnectController_Click;
                btn_DisconnectControllerAll.Click += Btn_DisconnectControllerAll_Click;
                btn_RemoveController.Click += Btn_RemoveController_Click;
                btn_DebugInformation.Click += Btn_CopyControllerDebugInfo_Click;
                btn_CheckControllers.Click += btn_CheckControllers_Click;
                btn_CheckDeviceManager.Click += btn_CheckDeviceManager_Click;

                //Controller functions
                btn_RumbleTestLight.Click += Btn_TestRumble_Click;
                btn_RumbleTestHeavy.Click += Btn_TestRumble_Click;

                //Save controller settings
                cb_ControllerFakeGuideButton.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.FakeGuideButton = cb_ControllerFakeGuideButton.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerUseButtonTriggers.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.UseButtonTriggers = cb_ControllerUseButtonTriggers.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerDPadFourWayMovement.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.DPadFourWayMovement = cb_ControllerDPadFourWayMovement.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipMovement.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipMovement = cb_ControllerThumbFlipMovement.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipAxesLeft = cb_ControllerThumbFlipAxesLeft.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipAxesRight = cb_ControllerThumbFlipAxesRight.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbReverseAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbReverseAxesLeft = cb_ControllerThumbReverseAxesLeft.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbReverseAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbReverseAxesRight = cb_ControllerThumbReverseAxesRight.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                slider_ControllerRumbleStrength.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerRumbleStrength.Text = "Rumble strength: " + slider_ControllerRumbleStrength.Value.ToString("0") + "%";
                        activeController.Details.Profile.RumbleStrength = Convert.ToInt32(slider_ControllerRumbleStrength.Value);
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                slider_ControllerLedBrightness.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = GetActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerLedBrightness.Text = "Led brightness: " + slider_ControllerLedBrightness.Value.ToString("0") + "%";
                        activeController.Details.Profile.LedBrightness = Convert.ToInt32(slider_ControllerLedBrightness.Value);
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");

                        //Update the controller led
                        SendXRumbleData(activeController, true, false, false);
                    }
                };

                //Controller button mapping functions
                btn_SetA.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetA.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetB.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetB.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetX.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetX.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetY.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetY.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetShoulderLeft.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetShoulderLeft.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetShoulderRight.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetShoulderRight.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetThumbLeft.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetThumbLeft.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetThumbRight.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetThumbRight.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetBack.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetBack.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetGuide.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetGuide.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetStart.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetStart.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetTriggerLeft.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetTriggerLeft.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetTriggerRight.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetTriggerRight.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;

                //Keypad button mapping functions
                btn_SetPadThumbLeftLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftLeft.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeftUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftUp.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeftRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftRight.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeftDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftDown.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightLeft.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightUp.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightRight.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightDown.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadA.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadA.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadB.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadB.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadX.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadX.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadY.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadY.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadShoulderLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadShoulderLeft.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadTriggerLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadTriggerLeft.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeft.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadShoulderRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadShoulderRight.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadTriggerRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadTriggerRight.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRight.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadBack.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadBack.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadStart.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadStart.MouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_Settings_KeypadProcessProfile_Add.Click += Btn_Settings_KeypadProcessProfile_Add_Click;
                btn_Settings_KeypadProcessProfile_Remove.Click += Btn_Settings_KeypadProcessProfile_Remove_Click;
                combobox_KeypadProcessProfile.SelectionChanged += Combobox_KeypadProcessProfile_SelectionChanged;

                //Settings functions
                btn_Settings_InstallDrivers.Click += btn_Settings_InstallDrivers_Click;

                //Help functions
                btn_Help_ProjectWebsite.Click += btn_Help_ProjectWebsite_Click;
                btn_Help_OpenDonation.Click += btn_Help_OpenDonation_Click;
            }
            catch { }
        }

        //Update the current window status
        void UpdateWindowStatus()
        {
            try
            {
                vProcessCtrlUI = GetProcessByNameOrTitle("CtrlUI", false);
                vProcessFpsOverlayer = GetProcessByNameOrTitle("FpsOverlayer", false);
                vProcessForeground = GetProcessMultiFromWindowHandle(GetForegroundWindow());

                //Check if CtrlUI is currently activated
                if (vProcessCtrlUI != null && vProcessCtrlUI.Id == vProcessForeground.Identifier) { vProcessCtrlUIActivated = true; } else { vProcessCtrlUIActivated = false; }

                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Maximized) { vAppMaximized = true; } else { vAppMaximized = false; }
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Id == vProcessForeground.Identifier)
                        {
                            AppWindowActivated();
                        }
                        else
                        {
                            AppWindowDeactivated();
                        }
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Application window activated event
        void AppWindowActivated()
        {
            try
            {
                if (!vAppActivated)
                {
                    vAppActivated = true;
                    Debug.WriteLine("Activated the application.");

                    //Enable application window
                    AppWindowEnable();
                }
            }
            catch { }
        }

        //Application window deactivated event
        void AppWindowDeactivated()
        {
            try
            {
                if (vAppActivated)
                {
                    vAppActivated = false;
                    Debug.WriteLine("Deactivated the application.");

                    //Disable application window
                    AppWindowDisable("Application window is not activated.");
                }
            }
            catch { }
        }

        //Enable application window
        void AppWindowEnable()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Enable the application window
                    grid_WindowActive.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Disable application window
        void AppWindowDisable(string windowText)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Update window status message
                    grid_WindowActiveText.Text = windowText;

                    //Disable the application window
                    grid_WindowActive.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }

        //Install the required drivers message popup
        async Task Message_InstallDrivers()
        {
            try
            {
                int messageResult = await AVMessageBox.MessageBoxPopup(this, "Drivers not installed", "Welcome to DirectXInput, it seems like you have not yet installed the required drivers to use this application, please make sure that you have installed the required drivers.\n\nDirectXInput will be closed during the installation of the required drivers.\n\nIf you just installed the drivers and this message shows up restart your PC.", "Install the drivers", "Close application", "", "");
                if (messageResult == 1)
                {
                    if (!CheckRunningProcessByNameOrTitle("DriverInstaller", false))
                    {
                        await ProcessLauncherWin32Async("DriverInstaller.exe", "", "", false, false);
                        await Application_Exit();
                    }
                }
                else
                {
                    await Application_Exit();
                }
            }
            catch { }
        }
    }
}