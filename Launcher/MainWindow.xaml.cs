using Configuration;
using System;
using Systems;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Path = System.IO.Path;
using System.Windows.Media.Imaging;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IConfiguration Configuration
        {
            get
            {
                if (Factory.Plugin != null)
                {
                    return Factory.Config.Merge(Factory.Plugin.Config.Merge(Factory.UserConfig));
                }
                else
                {
                    return Factory.Config.Merge(Factory.UserConfig);
                }

            }
        }

        private CancellationTokenSource _cancelSource;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            DisplayMessage("Initializing!");

            if (File.Exists("favico.ico"))
            {
                Uri iconUri = new(@"favico.ico", UriKind.RelativeOrAbsolute);
                //Icon = BitmapFrame.Create(iconUri);
                Icon = new BitmapImage(iconUri);
            }
            else if (Factory.Plugin.IconBytes.Length > 0)
            {
                Icon = LoadImage(Factory.Plugin.IconBytes);
            }

            this.TitleBlock.Text = Configuration.Title;
            this.Width = Configuration.Width;
            this.Height = Configuration.Height;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _cancelSource = new CancellationTokenSource();
            await ApplyFixes(_cancelSource.Token);
        }

        private async Task ApplyFixes(CancellationToken cancelToken = default)
        {

            Dictionary<string, string> TmpFolderToCopy = new();

            try
            {
                double CurrentProgress = 0d;
                double ProgressIncrease = 0d;
                TimeSpan ts = TimeSpan.FromMilliseconds(250);
                if (!File.Exists(Configuration.Executable))
                {
                    //MessageBox.Show("Application not found!");
                    throw new Exception("Application not found!");

                }

                DisplayMessage("Closing running Launcher!");
                await CloseRunningLauncher();
                CurrentProgress = (ProgressIncrease += 5d);

                FixProgress.SetPercent(CurrentProgress, ts);

                if (Factory.Plugin != null)
                {
                    var PreProgress = new Progress<FixProgress>(preProgress =>
                    {
                        ProgressIncrease = preProgress.Progress / 4;
                        FixProgress.SetPercent(CurrentProgress + ProgressIncrease, ts);
                        Status.Text = preProgress.StatusMessage;
                    });
                    await Factory.Plugin.Fix.Pre(PreProgress, cancelToken);

                    RestoreRegistry(Factory.Plugin.RegistryContent);

                    CurrentProgress += ProgressIncrease;
                }
                //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//

                var copyProgress = new Progress<FixProgress>(cProgress => {
                    ProgressIncrease = cProgress.Progress / 5;
                    FixProgress.SetPercent(CurrentProgress + ProgressIncrease, ts);
                    DisplayMessage($"Copying {cProgress.StatusMessage}");
                });

                string portableDataPath = Path.GetFullPath(Configuration.DataFolder);
                SecureDirectory(portableDataPath);

                TmpFolderToCopy = Configuration.DataPaths.Simplify(portableDataPath);

                await CopyFolders(await GatherFoldersToCopy(TmpFolderToCopy), copyProgress, cancelToken);
                CurrentProgress += ProgressIncrease;

                int ctr = 0;
                foreach (KeyValuePair<string, string> kvp in TmpFolderToCopy)
                {
                    DisplayMessage($"Creating link for {kvp.Key}");
                    SystemFunctions.CreateSymLink(kvp.Key, kvp.Value);
                    ctr++;
                    ProgressIncrease = ((ctr / TmpFolderToCopy.Count) * 100) / 5;
                    FixProgress.SetPercent(CurrentProgress + ProgressIncrease, ts);
                }
                CurrentProgress += ProgressIncrease;

                DisplayMessage($"Updating the registry!");

                string regPath = Path.Combine(portableDataPath, "Registries");
                //SecureDirectory(regPath);
                DirectoryInfo regFolder = new(regPath);
                if (regFolder.Exists)
                {
                    FileInfo[] regFiles = regFolder.GetFiles("*.preg");
                    if (regFiles.Length > 0)
                    {
                        RestoreRegistries(regFiles);
                    }
                }

                //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

                if (Factory.Plugin != null)
                {
                    MessageBox.Show(Systems.Registry.Helpers.ValueFormat(Factory.Plugin.RegistryContent));
                    RestoreRegistry(Factory.Plugin.RegistryContent);

                    var PostProgress = new Progress<FixProgress>(postProgress =>
                    {
                        ProgressIncrease = postProgress.Progress / 4;
                        FixProgress.SetPercent(CurrentProgress + ProgressIncrease, ts);
                        Status.Text = postProgress.StatusMessage;
                    });
                    await Factory.Plugin.Fix.Post(PostProgress, cancelToken);
                }

                DisplayMessage("Done!");
                await Task.Delay(300, cancelToken);

                FixProgress.SetPercent(100, TimeSpan.FromMilliseconds(10));
                DisplayMessage("Starting Epic Games Launcher!");

                this.Visibility = Visibility.Hidden;
                await Task.Delay(10, cancelToken);
                //if(Factory.Plugin.Fix.ExecuteApplication())
                string executableFullPath = Path.GetFullPath(Configuration.Executable); //new FileInfo(Configuration.Executable).FullName;
                try
                {
                    Factory.Plugin.Fix.ExecuteApplication(executableFullPath, Environment.GetCommandLineArgs());
                }
                catch (NotImplementedException)
                {
                    LaunchApplication(executableFullPath, Environment.GetCommandLineArgs());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (Configuration.ClearSymlinks && TmpFolderToCopy.Count > 0)
                {
                    foreach(KeyValuePair<string, string> kvp in TmpFolderToCopy)
                    {
                        SystemFunctions.DeleteSymLink(kvp.Key);
                    }
                }
            }
            this.Close();
        }

        private static void RestoreRegistries(FileInfo[] files)
        {
            foreach (FileInfo RegFile in files)
            {
                if (!RegFile.Exists)
                    throw new Exception(string.Format("File Does not exist!\nFile: \"{0}\"", RegFile.FullName));
                if (RegFile.Extension.ToLower() == "reg")
                    throw new Exception(string.Format("Unsupported File type.!\nFile: \"{0}\"", RegFile.FullName));

                RestoreRegistry(File.ReadAllText(RegFile.FullName));
            }
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private static void RestoreRegistry(string Content)
        {
            if (!string.IsNullOrWhiteSpace(Content))
            {
                string tmp = Systems.Registry.Helpers.ValueFormat(Content);
                //MessageBox.Show(tmp);
                Systems.Registry.Restore.RestoreRegistry(tmp);
            }
        }

        private static void SecureDirectory(string Path)
        {
            if (!Directory.Exists(Path))
            { Directory.CreateDirectory(Path); }
        }

        private static async Task CloseRunningLauncher()
        {
            Process[] tmpPrcs = Process.GetProcessesByName("EpicGamesLauncher");
            foreach (Process tp in tmpPrcs)
            {
                SystemFunctions.KillProcessAndChildren(tp.Id);
                await Task.Delay(10);
            }
        }

        private static void LaunchApplication(string AppExec, string[] args)
        {
            Process Exec = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = AppExec,
                    WorkingDirectory = Path.GetDirectoryName(AppExec),
                    Arguments = string.Join(" ", args)
                },
            };
            Exec.Start();
            Exec.WaitForExit();
        }

        private static bool WillCopyFolder(string sourcePath, string targetPath)
        {
            bool output = false;

            if (!Directory.Exists(targetPath) ||
                SystemFunctions.IsDirectoryEmpty(targetPath) ||
                SystemFunctions.GetFolderInfo(new DirectoryInfo(targetPath)).Size <= 0)
            {
                output = WillMakeSymLink(sourcePath, targetPath);
            }

            return output;
        }

        private static bool WillMakeSymLink(string sourcePath, string targetPath)
        {
            bool output = false;
            string link = string.Empty;

            try { link = SystemFunctions.GetFinalPathName(sourcePath); }
            catch (Exception) { }

            if (!string.IsNullOrEmpty(link) && link != targetPath && !SystemFunctions.IsDirectoryEmpty(sourcePath))
            {
                output = true;
            }
            return output;
        }

        private static async Task<Dictionary<string, string>> GatherFoldersToCopy(Dictionary<string, string> dataPaths)
        {
            Dictionary<string, string> TmpFolderToCopy = new();
            foreach (KeyValuePair<string, string> path in dataPaths)
            {
                if (WillCopyFolder(path.Key, path.Value))
                {
                    TmpFolderToCopy.Add(path.Key, path.Value);
                }
                await Task.Delay(10);
            }

            return TmpFolderToCopy;
        }

        private static async Task CopyFolders(Dictionary<string, string> Folders, IProgress<FixProgress> progress = null, CancellationToken cancelToken = default)
        {
            if (Folders.Count > 0)
            {
                int ctr = 0;
                foreach (KeyValuePair<string, string> kvp in Folders)
                {
                    ctr++;
                    FixProgress copyProgress = new();
                    var FPI = new Progress<FolderCopyProgressInfo>(prog => {
                        copyProgress.Progress = ((prog.Progress * ctr) / (100 * Folders.Count)) * 100;
                        copyProgress.StatusMessage = prog.FileName;
                        progress?.Report(copyProgress);
                    });

                    await Copy.Folder(new DirectoryInfo(kvp.Key), new DirectoryInfo(kvp.Value), FPI, cancelToken);
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        { if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); } }

        private void DisplayMessage(string message)
        { Status.Text = message; }

        private async void FixProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ProgressLabel.Text = $"{e.NewValue:0.00}%";
            if (e.NewValue == 100d)
            {
                await Task.Delay(750);
                ProgressLabel.Text = string.Empty;
                FixProgress.Value = 0d;
                FixProgress.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _cancelSource.Cancel();
        }
    }
}
