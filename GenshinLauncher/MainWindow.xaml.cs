using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinRT;

namespace GenshinLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDownloading = false;
        private bool _isPaused = false;
        private bool _isDownloaded = false;

        private float _downloadProgress = 0f;
        private long _downloadSize;
        private long _downloadedSize;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void mouse_MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btn_Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void btn_ResetColor(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Background = null;
                btn.BorderBrush = null;
            }
        }

        private void BtnMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            if (btn != null)
            {
                btn.Background = bc.ConvertFrom("#ff4c4c4c") as Brush;
                btn.BorderBrush = bc.ConvertFrom("#ff4c4c4c") as Brush;
            }
        }

        private void BtnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            if (btn != null)
            {
                btn.Background = bc.ConvertFrom("#ffff6347") as Brush;
                btn.BorderBrush = bc.ConvertFrom("#ffff6347") as Brush;
            }
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;

            if (btn != null)
            {
                btn.Background = Brushes.Transparent;
                btn.BorderBrush = Brushes.Transparent;
            }
        }

        private void BtnStart_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            if (btn != null)
            {
                btn.Background = bc.ConvertFrom("#FFFFDB60") as Brush;
                btn.BorderBrush = bc.ConvertFrom("#FFFFDB60") as Brush;
            }
        }

        private void BtnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            if (btn != null)
            {
                btn.Background = bc.ConvertFrom("#FFFFC70A") as Brush;
                btn.BorderBrush = bc.ConvertFrom("#FFFFC70A") as Brush;
            }
        }

        private void BtnIssues_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://www.youtube.com/watch?v=9ixmwNCbo1c",
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            // var psi = new ProcessStartInfo
            // {
            //     FileName = "https://namu.wiki/w/붕괴: 스타레일",
            //     UseShellExecute = true
            // };
            //
            // Process.Start(psi);

            if (!_isDownloaded)
            {
                Download();
            }
        }

        private void Download()
        {
            GridDownload.Visibility = Visibility.Visible;
            ProgressDownload.Value = 0f;
            _isDownloading = true;
            _downloadSize = Random.Shared.NextInt64(5000000000, 20000000000);
            var bc = new BrushConverter();
            BtnStart.Background = bc.ConvertFrom("#FFBA8F00") as Brush;
            BtnStart.IsEnabled = false;

            ResumeDownload();
        }

        private async void ResumeDownload()
        {
            while (_isDownloading)
            {
                await DownloadTask();
            }
        }

        private async Task DownloadTask()
        {
            var random = Random.Shared.NextInt64(8000000, 120000000);
            _downloadedSize += random;
            _downloadProgress = (float)_downloadedSize / (float)_downloadSize * 100f;

            if (_downloadProgress >= 100f)
            {
                _isDownloading = false;
                _isDownloaded = true;
            }

            ProgressDownload.Value = _downloadProgress;
            LabelDownloadPercentage.Content = $"다운로드 중... {_downloadProgress:0.##}% ({FormatBytes(_downloadedSize)} / {FormatBytes(_downloadSize)})";
            LabelDownloadSpeed.Content = $"다운로드 속도: {FormatBytes(random)}/s";

            await Task.Delay(1000);
        }

        private void PauseDownload()
        {

        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            switch (GridStartingSettings.Visibility)
            {
                case Visibility.Visible:
                    GridStartingSettings.Visibility = Visibility.Hidden;
                    GridStartingSettingsHint.Visibility = Visibility.Visible;
                    break;
                case Visibility.Hidden:
                    GridStartingSettingsHint.Visibility = Visibility.Hidden;
                    GridStartingSettings.Visibility = Visibility.Visible;
                    break;
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Content of window may be black in case of SizeToContent is set. 
            // This eliminates the problem. 
            // Do not use InvalidateVisual because it may implicitly break your markup.
            InvalidateMeasure();
        }

        private void BtnSettings_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            if (GridStartingSettings.Visibility != Visibility.Visible)
                GridStartingSettingsHint.Visibility = Visibility.Visible;

            if (btn != null)
            {
                btn.Background = bc.ConvertFrom("#FFFFDB60") as Brush;
                btn.BorderBrush = bc.ConvertFrom("#FFFFDB60") as Brush;
            }
        }

        private void BtnSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            GridStartingSettingsHint.Visibility = Visibility.Hidden;

            if (btn != null)
            {
                btn.Background = bc.ConvertFrom("#FFFFC70A") as Brush;
                btn.BorderBrush = bc.ConvertFrom("#FFFFC70A") as Brush;
            }
        }

        private void BtnStartSettings_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            if (btn != null)
            {
                btn.Background = bc.ConvertFrom("#FFDDDDDD") as Brush;
                btn.BorderBrush = bc.ConvertFrom("#FFDDDDDD") as Brush;
            }
        }

        private void BtnStartSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var bc = new BrushConverter();

            if (btn != null)
            {
                btn.Background = Brushes.Transparent;
                btn.BorderBrush = Brushes.Transparent;
            }
        }

        private void btn_Screenshot(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", @"C:\Users");
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GridStartingSettings.Visibility = Visibility.Hidden;
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {

        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return $"{dblSByte:0.##} {Suffix[i]}";
        }
    }
}