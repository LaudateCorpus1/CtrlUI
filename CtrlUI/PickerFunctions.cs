﻿using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.ProcessClasses;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the File Picker Popup
        async Task Popup_Show_FilePicker(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                //Play the popup opening sound
                if (!vFilePickerOpen)
                {
                    PlayInterfaceSound(vInterfaceSoundVolume, "PopupOpen", false);
                }

                //Save previous focus element
                Popup_PreviousFocusSave(vFilePickerElementFocus, previousFocus);

                //Reset file picker variables
                vFilePickerCompleted = false;
                vFilePickerCancelled = false;
                vFilePickerResult = null;
                vFilePickerOpen = true;

                //Set file picker header texts
                grid_Popup_FilePicker_txt_Title.Text = vFilePickerTitle;
                grid_Popup_FilePicker_txt_Description.Text = vFilePickerDescription;

                //Change the list picker item style
                if (vFilePickerShowRoms)
                {
                    lb_FilePicker.Style = Application.Current.Resources["ListBoxWrapPanel"] as Style;
                    lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemRom"] as DataTemplate;
                    grid_Popup_Filepicker_Row1.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    lb_FilePicker.Style = Application.Current.Resources["ListBoxVertical"] as Style;
                    lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemString"] as DataTemplate;
                    grid_Popup_Filepicker_Row1.HorizontalAlignment = HorizontalAlignment.Stretch;
                }

                //Show the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_FilePicker, true, true, 0.10);
                AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 0.02, true, false, 0.10); }
                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                //if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 0.02, true, false, 0.10); }
                if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                //Get and update the current index and path
                vFilePickerCurrentPath = targetPath;
                if (storeIndex)
                {
                    FilePicker_AddIndexHistory(lb_FilePicker.SelectedIndex);
                }

                //Clear the current file picker list
                List_FilePicker.Clear();

                //Get and set all strings to list
                if (targetPath == "String")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Hide file and folder availability
                    grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    //Convert all the strings from array
                    foreach (DataBindString stringPicker in vFilePickerStrings)
                    {
                        try
                        {
                            DataBindFile dataBindFile = new DataBindFile() { Type = "File", Name = stringPicker.Name, PathFile = stringPicker.PathFile, ImageBitmap = stringPicker.ImageBitmap };
                            await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFile, false, false);
                        }
                        catch { }
                    }
                }
                //Get and list all the disk drives
                else if (targetPath == "PC")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Hide file and folder availability
                    grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    BitmapImage imageFolder = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, -1);

                    //Add my documents and pictures folder
                    DataBindFile dataBindFilePictures = new DataBindFile() { Type = "PreDirectory", Name = "My Pictures", ImageBitmap = imageFolder, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePictures, false, false);
                    DataBindFile dataBindFileDocuments = new DataBindFile() { Type = "PreDirectory", Name = "My Documents", ImageBitmap = imageFolder, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDocuments, false, false);

                    //Check and add the previous path
                    if (!string.IsNullOrWhiteSpace(vFilePickerPreviousPath))
                    {
                        DataBindFile dataBindFilePreviousPath = new DataBindFile() { Type = "PreDirectory", Name = "Previous Path", NameSub = "(" + vFilePickerPreviousPath + ")", ImageBitmap = imageFolder, PathFile = vFilePickerPreviousPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePreviousPath, false, false);
                    }

                    //Add launch without a file option
                    if (vFilePickerShowNoFile)
                    {
                        string fileDescription = "Launch application without a file";
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileWithoutFile = new DataBindFile() { Type = "File", Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutFile, false, false);
                    }

                    //Add all disk drives that are connected
                    DriveInfo[] diskDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo disk in diskDrives)
                    {
                        try
                        {
                            //Skip network drive depending on the setting
                            if (disk.DriveType == DriveType.Network && Convert.ToBoolean(ConfigurationManager.AppSettings["HideNetworkDrives"]))
                            {
                                continue;
                            }

                            //Check if the disk is currently connected
                            if (disk.IsReady)
                            {
                                //Get the current disk size
                                string freeSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalFreeSpace);
                                string usedSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalSize);
                                string diskSpace = freeSpace + "/" + usedSpace;

                                DataBindFile dataBindFileDisk = new DataBindFile() { Type = "Directory", Name = disk.Name, NameSub = disk.VolumeLabel, NameDetail = diskSpace, ImageBitmap = imageFolder, PathFile = disk.Name };
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDisk, false, false);
                            }
                        }
                        catch { }
                    }

                    //Add Json file locations
                    foreach (FileLocation Locations in vCtrlLocationsFile)
                    {
                        try
                        {
                            if (Directory.Exists(Locations.Path))
                            {
                                DataBindFile dataBindFileLocation = new DataBindFile() { Type = "Directory", Name = Locations.Path, NameSub = Locations.Name, ImageBitmap = imageFolder, PathFile = Locations.Path };
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileLocation, false, false);
                            }
                        }
                        catch { }
                    }
                }
                //Get and list all the UWP applications
                else if (targetPath == "UWP")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Hide file and folder availability
                    grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;

                    //Enable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    //Add uwp applications to the filepicker list
                    async Task TaskAction()
                    {
                        try
                        {
                            await ListLoadAllUwpApplications(lb_FilePicker, List_FilePicker);
                        }
                        catch { }
                    }
                    await AVActions.TaskStartAsync(TaskAction, null);
                }
                else
                {
                    //Clean the target path string
                    targetPath = Path.GetFullPath(targetPath);

                    //Add the Go up directory to the list
                    if (Path.GetPathRoot(targetPath) != targetPath)
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + targetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = Path.GetDirectoryName(targetPath) };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                    else
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + targetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = "PC" };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }

                    //Add launch emulator options
                    if (vFilePickerShowRoms)
                    {
                        string fileDescription = "Launch the emulator without a rom loaded";
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileWithoutRom = new DataBindFile() { Type = "File", Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutRom, false, false);

                        string romDescription = "Launch the emulator with this folder as rom";
                        BitmapImage romImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileFolderRom = new DataBindFile() { Type = "File", Name = romDescription, Description = romDescription + ".", ImageBitmap = romImage, PathFile = targetPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolderRom, false, false);
                    }

                    //Enable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Visible;

                    //Get all the directories from target directory
                    if (vFilePickerShowDirectories)
                    {
                        try
                        {
                            //Get all the files or folders
                            DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                            DirectoryInfo[] directoryPaths = null;
                            if (vFilePickerSortByName)
                            {
                                directoryPaths = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                            }
                            else
                            {
                                directoryPaths = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                            }

                            //Fill the file picker listbox with directories
                            BitmapImage folderImage = null;
                            string folderDescription = string.Empty;
                            foreach (DirectoryInfo listDirectory in directoryPaths)
                            {
                                try
                                {
                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        //Get folder name
                                        string folderName = listDirectory.Name.ToLower();

                                        //Get description names
                                        string folderRomsTxt = "Assets\\Roms\\" + folderName + ".txt";
                                        string folderImageTxt = Path.Combine(listDirectory.FullName, folderName + ".txt");
                                        folderDescription = FileToString(new string[] { folderRomsTxt, folderImageTxt });

                                        //Get image names
                                        string folderRomsJpg = "Assets\\Roms\\" + folderName + ".jpg";
                                        string folderRomsPng = "Assets\\Roms\\" + folderName + ".png";
                                        string folderImageJpg = Path.Combine(listDirectory.FullName, folderName + ".jpg");
                                        string folderImagePng = Path.Combine(listDirectory.FullName, folderName + ".png");

                                        folderImage = FileToBitmapImage(new string[] { folderRomsPng, folderImagePng, folderRomsJpg, folderImageJpg, "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, 180);
                                    }
                                    else
                                    {
                                        folderImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, -1);
                                    }

                                    //Get the folder size
                                    //string folderSize = AVFunctions.ConvertBytesSizeToString(GetDirectorySize(listDirectory));

                                    //Add folder to the list
                                    bool systemFileFolder = listDirectory.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listDirectory.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"])))
                                    {
                                        DataBindFile dataBindFileFolder = new DataBindFile() { Type = "Directory", Name = listDirectory.Name, Description = folderDescription, DateModified = listDirectory.LastWriteTime, ImageBitmap = folderImage, PathFile = listDirectory.FullName };
                                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }

                    //Get all the files from target directory
                    if (vFilePickerShowFiles)
                    {
                        try
                        {
                            //Disable selection button in the list
                            grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                            //Get all the files or folders
                            DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                            FileInfo[] directoryPathsFiles = null;
                            if (vFilePickerSortByName)
                            {
                                directoryPathsFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                            }
                            else
                            {
                                directoryPathsFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                            }

                            string[] imageFilter = new string[] { "jpg", "png" };
                            string[] descriptionFilter = new string[] { "txt" };
                            FileInfo[] romImagesDirectory = new FileInfo[] { };
                            FileInfo[] romDescriptionsDirectory = new FileInfo[] { };

                            //Filter rom images and descriptions
                            if (vFilePickerShowRoms)
                            {
                                DirectoryInfo directoryInfoRoms = new DirectoryInfo("Assets\\Roms");
                                FileInfo[] directoryPathsRoms = directoryInfoRoms.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(z => z.Name).ToArray();

                                FileInfo[] RomsImages = directoryPathsRoms.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                FileInfo[] FilesImages = directoryPathsFiles.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                romImagesDirectory = FilesImages.Concat(RomsImages).OrderByDescending(s => s.Name.Length).ToArray();

                                FileInfo[] RomsDescriptions = directoryPathsRoms.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                FileInfo[] FilesDescriptions = directoryPathsFiles.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                romDescriptionsDirectory = FilesDescriptions.Concat(RomsDescriptions).OrderByDescending(s => s.Name.Length).ToArray();
                            }

                            //Filter files in and out
                            if (vFilePickerFilterIn.Any())
                            {
                                directoryPathsFiles = directoryPathsFiles.Where(file => vFilePickerFilterIn.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }
                            if (vFilePickerFilterOut.Any())
                            {
                                directoryPathsFiles = directoryPathsFiles.Where(file => !vFilePickerFilterOut.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }

                            //Fill the file picker listbox with files based on filter
                            BitmapImage fileImage = null;
                            string fileDescription = string.Empty;
                            foreach (FileInfo listFile in directoryPathsFiles)
                            {
                                try
                                {
                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        //Get rom file names
                                        string romFoundPathImage = string.Empty;
                                        string romFoundPathDescription = string.Empty;
                                        string romNameWithoutExtension = Path.GetFileNameWithoutExtension(listFile.Name).Replace(" ", string.Empty).ToLower();

                                        //Check if rom directory has image
                                        foreach (FileInfo FoundRom in romImagesDirectory)
                                        {
                                            try
                                            {
                                                if (romNameWithoutExtension.Contains(Path.GetFileNameWithoutExtension(FoundRom.Name.Replace(" ", string.Empty).ToLower())))
                                                {
                                                    romFoundPathImage = FoundRom.FullName;
                                                    break;
                                                }
                                            }
                                            catch { }
                                        }

                                        //Check if rom directory has description
                                        foreach (FileInfo FoundRom in romDescriptionsDirectory)
                                        {
                                            try
                                            {
                                                if (romNameWithoutExtension.Contains(Path.GetFileNameWithoutExtension(FoundRom.Name.Replace(" ", string.Empty).ToLower())))
                                                {
                                                    romFoundPathDescription = FoundRom.FullName;
                                                    break;
                                                }
                                            }
                                            catch { }
                                        }

                                        fileDescription = FileToString(new string[] { romFoundPathDescription });
                                        fileImage = FileToBitmapImage(new string[] { romFoundPathImage, "Rom" }, IntPtr.Zero, 180);
                                    }
                                    else
                                    {
                                        fileDescription = string.Empty;
                                        string listFileFullNameLower = listFile.FullName.ToLower();
                                        if (listFileFullNameLower.EndsWith(".jpg") || listFileFullNameLower.EndsWith(".png") || listFileFullNameLower.EndsWith(".gif"))
                                        {
                                            fileImage = FileToBitmapImage(new string[] { listFile.FullName }, IntPtr.Zero, 50);
                                        }
                                        else if (listFileFullNameLower.EndsWith(".exe"))
                                        {
                                            fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                        }
                                        else if (listFileFullNameLower.EndsWith(".bat"))
                                        {
                                            fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/FileBat.png" }, IntPtr.Zero, -1);
                                        }
                                        else
                                        {
                                            fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/File.png" }, IntPtr.Zero, -1);
                                        }
                                    }

                                    //Get the file size
                                    string fileSize = AVFunctions.ConvertBytesSizeToString(listFile.Length);

                                    //Add file to the list
                                    bool systemFileFolder = listFile.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listFile.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"])))
                                    {
                                        DataBindFile dataBindFileFile = new DataBindFile() { Type = "File", Name = listFile.Name, NameDetail = fileSize, Description = fileDescription, DateModified = listFile.LastWriteTime, ImageBitmap = fileImage, PathFile = listFile.FullName };
                                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFile, false, false);
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        //Enable selection button in the list
                        grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Visible;
                    }

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();
                }

                //Focus on the file picker listbox
                await ListboxFocus(lb_FilePicker, false, false, targetIndex);
            }
            catch { }
        }

        //Check if there are files or folders
        void FilePicker_CheckFilesAndFoldersCount()
        {
            try
            {
                int totalFileCount = List_FilePicker.Count - 1; //Filter out GoUp
                if (totalFileCount > 0)
                {
                    grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;
                    Debug.WriteLine("There are files and folders in the list.");
                }
                else
                {
                    grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Visible;
                    Debug.WriteLine("None files and folders in the list.");
                }
            }
            catch { }
        }

        //Store the picker navigation index
        void FilePicker_AddIndexHistory(int PreviousIndex)
        {
            try
            {
                Debug.WriteLine("Adding navigation history: " + PreviousIndex);
                vFilePickerNavigateIndexes.Add(PreviousIndex);
            }
            catch { }
        }

        //Close file picker popup
        async Task Popup_Close_FilePicker(bool IsCompleted, bool CurrentFolder)
        {
            try
            {
                PlayInterfaceSound(vInterfaceSoundVolume, "PopupClose", false);

                //Reset and update popup variables
                vFilePickerOpen = false;
                if (IsCompleted)
                {
                    vFilePickerCompleted = true;
                    if (CurrentFolder)
                    {
                        DataBindFile targetPath = new DataBindFile();
                        targetPath.PathFile = vFilePickerCurrentPath;
                        vFilePickerResult = targetPath;
                    }
                    else
                    {
                        vFilePickerResult = (DataBindFile)lb_FilePicker.SelectedItem;
                    }
                }
                else
                {
                    vFilePickerCancelled = true;
                }

                //Store the current picker path
                if (vFilePickerCurrentPath.Contains(":"))
                {
                    Debug.WriteLine("Closed the picker on: " + vFilePickerCurrentPath);
                    vFilePickerPreviousPath = vFilePickerCurrentPath;
                }

                //Clear the current file picker list
                vFilePickerNavigateIndexes.Clear();
                List_FilePicker.Clear();

                //Hide the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_FilePicker, false, false, 0.10);

                if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                else if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 1, true, true, 0.10); }
                else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 1, true, true, 0.10); }
                else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                while (grid_Popup_FilePicker.Visibility == Visibility.Visible) { await Task.Delay(10); }

                //Focus on the previous focus element
                await Popup_PreviousFocusForce(vFilePickerElementFocus);
            }
            catch { }
        }

        //Show string based picker
        async void Button_ShowStringPicker(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);
                string ButtonName = ButtonSender.Name;

                //Change the manage application category
                if (ButtonName == "btn_Manage_AddAppCategory")
                {
                    //Check if the application is UWP
                    bool UwpApplication = sp_AddAppExePath.Visibility == Visibility.Collapsed;

                    //Add application type categories
                    vFilePickerStrings.Clear();

                    BitmapImage imageGame = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Game.png" }, IntPtr.Zero, -1);
                    DataBindString stringGame = new DataBindString() { Name = "Game", PathFile = "Game", ImageBitmap = imageGame };
                    vFilePickerStrings.Add(stringGame);

                    BitmapImage imageApp = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                    DataBindString stringApp = new DataBindString() { Name = "App & Media", PathFile = "App", ImageBitmap = imageApp };
                    vFilePickerStrings.Add(stringApp);

                    if (!UwpApplication)
                    {
                        BitmapImage imageEmulator = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1);
                        DataBindString stringEmulator = new DataBindString() { Name = "Emulator", PathFile = "Emulator", ImageBitmap = imageEmulator };
                        vFilePickerStrings.Add(stringEmulator);
                    }

                    vFilePickerFilterIn = new List<string>();
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Application Category";
                    vFilePickerDescription = "Please select a new application category:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = false;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("String", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    image_Manage_AddAppCategory.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + vFilePickerResult.PathFile + ".png" }, IntPtr.Zero, -1);
                    textblock_Manage_AddAppCategory.Text = vFilePickerResult.Name;
                    btn_Manage_AddAppCategory.Tag = vFilePickerResult.PathFile;

                    if (UwpApplication || vFilePickerResult.PathFile != "Emulator")
                    {
                        sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        sp_AddAppPathRoms.Visibility = Visibility.Visible;
                    }

                    if (UwpApplication || vFilePickerResult.PathFile != "App")
                    {
                        checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        checkbox_AddLaunchFilePicker.Visibility = Visibility.Visible;
                    }
                }
                //Change the quick launch app
                else if (ButtonName == "btn_Settings_AppQuickLaunch")
                {
                    //Add all apps to the string list
                    vFilePickerStrings.Clear();

                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false))
                    {
                        DataBindString stringApp = new DataBindString() { Name = dataBindApp.Name, ImageBitmap = dataBindApp.ImageBitmap };
                        vFilePickerStrings.Add(stringApp);
                    }

                    vFilePickerFilterIn = new List<string>();
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Quick Launch Application";
                    vFilePickerDescription = "Please select a new quick launch application:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = false;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("String", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    btn_Settings_AppQuickLaunch.Content = "Change quick launch app: " + vFilePickerResult.Name;

                    //Set previous quick launch application to false
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.QuickLaunch))
                    {
                        dataBindApp.QuickLaunch = false;
                    }

                    //Set new quick launch application to true
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.Name.ToLower() == vFilePickerResult.Name.ToLower()))
                    {
                        dataBindApp.QuickLaunch = true;
                    }

                    //Save changes to Json file
                    JsonSaveApplications();
                }
            }
            catch { }
        }

        //Show file based picker
        async void Button_ShowFilePicker(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);
                string ButtonName = ButtonSender.Name;

                //Add and Edit application page
                if (ButtonName == "btn_AddAppLogo")
                {
                    //Check if there is an application name set
                    if (string.IsNullOrWhiteSpace(tb_AddAppName.Text) || tb_AddAppName.Text == "Select application executable file first")
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                        return;
                    }

                    vFilePickerFilterIn = new List<string> { "jpg", "png" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Application Image";
                    vFilePickerDescription = "Please select a new application image:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Load and copy the new application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { vFilePickerResult.PathFile }, IntPtr.Zero, 120);
                    img_AddAppLogo.Source = applicationImage;
                    if (vEditAppDataBind != null) { vEditAppDataBind.ImageBitmap = applicationImage; }
                    File.Copy(vFilePickerResult.PathFile, "Assets\\Apps\\" + tb_AddAppName.Text + ".png", true);
                }
                else if (ButtonName == "btn_AddAppExePath")
                {
                    vFilePickerFilterIn = new List<string> { "exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Application Executable";
                    vFilePickerDescription = "Please select an application executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Set fullpath to exe path textbox
                    tb_AddAppExePath.Text = vFilePickerResult.PathFile;
                    tb_AddAppExePath.IsEnabled = true;

                    //Set application launch path to textbox
                    tb_AddAppPathLaunch.Text = Path.GetDirectoryName(vFilePickerResult.PathFile);
                    tb_AddAppPathLaunch.IsEnabled = true;
                    btn_AddAppPathLaunch.IsEnabled = true;

                    //Set application name to textbox
                    tb_AddAppName.Text = vFilePickerResult.Name.Replace(".exe", "");
                    tb_AddAppName.IsEnabled = true;

                    //Set application image to image preview
                    img_AddAppLogo.Source = FileToBitmapImage(new string[] { tb_AddAppName.Text, vFilePickerResult.PathFile }, IntPtr.Zero, 120);
                    btn_AddAppLogo.IsEnabled = true;
                    btn_Manage_ResetAppLogo.IsEnabled = true;
                }
                else if (ButtonName == "btn_AddAppPathLaunch")
                {
                    vFilePickerFilterIn = new List<string>();
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Launch Folder";
                    vFilePickerDescription = "Please select the launch folder:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    tb_AddAppPathLaunch.Text = vFilePickerResult.PathFile;
                    tb_AddAppPathLaunch.IsEnabled = true;
                    btn_AddAppPathLaunch.IsEnabled = true;
                }
                else if (ButtonName == "btn_AddAppPathRoms")
                {
                    vFilePickerFilterIn = new List<string>();
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Rom Folder";
                    vFilePickerDescription = "Please select the rom folder:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    tb_AddAppPathRoms.Text = vFilePickerResult.PathFile;
                    tb_AddAppPathRoms.IsEnabled = true;
                }

                //Settings Background Changer
                else if (ButtonName == "btn_Settings_ChangeBackground")
                {
                    vFilePickerFilterIn = new List<string> { "jpg", "png" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Background Image";
                    vFilePickerDescription = "Please select a new background image:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    File.Copy(vFilePickerResult.PathFile, "Assets\\Background.png", true);
                    cb_SettingsDesktopBackground.IsChecked = false;
                    SettingSave("DesktopBackground", "False");
                    UpdateBackgroundImage();
                }

                //First launch quick setup
                else if (ButtonName == "grid_Popup_Welcome_button_Steam")
                {
                    vFilePickerFilterIn = new List<string> { "steam.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Steam";
                    vFilePickerDescription = "Please select the Steam executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Steam", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), Argument = "-bigpicture", QuickLaunch = true };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Origin")
                {
                    vFilePickerFilterIn = new List<string> { "origin.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Origin";
                    vFilePickerDescription = "Please select the Origin executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Origin", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Uplay")
                {
                    vFilePickerFilterIn = new List<string> { "upc.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Uplay";
                    vFilePickerDescription = "Please select the Uplay executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Uplay", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_GoG")
                {
                    vFilePickerFilterIn = new List<string> { "galaxyclient.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "GoG";
                    vFilePickerDescription = "Please select the GoG executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "GoG", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Battle")
                {
                    vFilePickerFilterIn = new List<string> { "battle.net.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Battle.net";
                    vFilePickerDescription = "Please select the Battle.net executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Battle.net", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_PS4Remote")
                {
                    vFilePickerFilterIn = new List<string> { "RemotePlay.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "PS4 Remote Play";
                    vFilePickerDescription = "Please select the PS4 Remote Play executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Remote Play", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Kodi")
                {
                    vFilePickerFilterIn = new List<string> { "kodi.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Kodi";
                    vFilePickerDescription = "Please select the Kodi executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Kodi", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Spotify")
                {
                    vFilePickerFilterIn = new List<string> { "spotify.exe" };
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Spotify";
                    vFilePickerDescription = "Please select the Spotify executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Spotify", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
            }
            catch { }
        }
    }
}