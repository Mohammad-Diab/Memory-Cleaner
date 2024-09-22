using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic.Devices;

namespace Memory_cleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<Bitmap> imagesList = new List<Bitmap>();
        ComputerInfo computerInfo = new ComputerInfo();
        public bool isStop = false;
        public bool isRunning = false;
        public bool finished = false;
        public ulong initialFree = 0;
        int timeToFinish = 600;

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            clean_btn.IsEnabled = false;
            stop_btn.IsEnabled = true;
            progess.Value = 0;
            initialFree = GetFreeMemory();
            timeToFinish = 600;
            isStop = false;
            finished = false;
            isRunning = true;
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted; ;
            bw.RunWorkerAsync();

            Task progress = new Task(() =>
            {
                while (bw.IsBusy && !finished)
                {
                    bw.ReportProgress(0);
                    Thread.Sleep(50);
                }
            });
            progress.Start();
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ReadRamStatistic();
            progess.Value = 100;
            clean_btn.IsEnabled = true;
            stop_btn.IsEnabled = false;
            ulong free = GetFreeMemory();
            if (finished)
            {
                MessageBox.Show(SizeSuffix(free > initialFree ? free - initialFree : 0) + " is Freed\nThank you for using this App.", "Memory Cleaner");
            }
        }

        private void ReadRamStatistic()
        {
            long usage = Process.GetCurrentProcess().PrivateMemorySize64;
            ulong total = computerInfo.TotalPhysicalMemory;
            ulong free = GetFreeMemory();
            totalMem.Text = SizeSuffix(total);
            usedMemByApp.Text = SizeSuffix((ulong)usage);
            freeMem.Text = SizeSuffix(free);

            if (isRunning && initialFree > 0)
            {
                progess.Value = 95.0 * ((total - free + 0.0) / total);
                if ((total - free + 0.0) / total > 0.985)
                {
                    timeToFinish--;
                }

                if (timeToFinish == 0)
                {
                    finished = true;
                }
            }

        }

        private ulong GetFreeMemory()
        {
            ulong free = 0;
            ManagementObjectSearcher search = new ManagementObjectSearcher("root\\CIMV2", "Select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OPeratingSystem");
            foreach (ManagementObject x in search.Get())
            {
                ulong totalMemory = (ulong)x["TotalVisibleMemorySize"];
                free = (ulong)x["FreePhysicalMemory"];
            }
            return free * 1024;
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                MessageBox.Show(e.UserState.ToString());
            }
            else
            {
                ReadRamStatistic();
            }
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            imagesList.Clear();
            GC.Collect();
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;
            Bitmap source;
            MemoryStream ms = null;
            try
            {
                using (source = new Bitmap((int)screenWidth, (int)screenHeight))
                {
                    using (Graphics g = Graphics.FromImage(source))
                    {
                        g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, source.Size);

                        ms = new MemoryStream();
                        source.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        ms.Position = 0;

                        imagesList.Add(source);
                    }
                }
                while (!isStop && !finished)
                {
                    ms.Position = 0;
                    Bitmap bmp = new Bitmap(ms);
                    imagesList.Add(bmp);
                }
            }
            catch (Exception ex)
            {
                ms?.Dispose();
                ((BackgroundWorker)sender).ReportProgress(-1, ex.Message);
            }
            ms?.Dispose();
            foreach (Image image in imagesList)
            {
                image.Dispose();
            }
            imagesList.Clear();
            GC.Collect();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            isStop = true;
            isRunning = false;
            stop_btn.IsEnabled = false;
        }

        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(ulong value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            int mag = (int)Math.Log(value, 1024);

            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadRamStatistic();
        }

        private void Run_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/mohammaddiab0/");
        }
    }
}
