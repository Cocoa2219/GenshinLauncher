using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using ProgressBar = System.Windows.Controls.ProgressBar;
using FontFamily = System.Windows.Media.FontFamily;

namespace GenshinLauncher;

public partial class MainWindow : Window
{
    private bool _isDownloading;
    private bool _isPaused;
    private bool _isDownloaded;

    private float _downloadProgress;
    private long _downloadSize;
    private long _downloadedSize;

    private readonly List<BitmapImage> _splashImages = [];
    private readonly List<Button> _centerButtons = [];
    private readonly List<Button> _buttons = [];
    private readonly List<Image> _images = [];
    
    private NotifyIcon _notifyIcon;

    public MainWindow()
    {
        InitializeComponent();

        _notifyIcon = new NotifyIcon();
        _notifyIcon.Icon = new Icon("C:\\Users\\PC\\Documents\\GenshinLauncher\\GenshinLauncher\\Images\\FurinaTray.ico");
        _notifyIcon.Visible = true;
        _notifyIcon.Text = "원신 런쳐 - 원신은 역시 푸리나";
        _notifyIcon.BalloonTipText = "원신 런쳐가 트레이 아이콘으로 이동되었습니다.";
        _notifyIcon.BalloonTipTitle = "원신 런쳐 - 원신은 역시 푸리나";
        _notifyIcon.Click +=
            delegate(object sender, EventArgs args)
            {
                if (((System.Windows.Forms.MouseEventArgs)args).Button == MouseButtons.Right) return;

                Show();
                WindowState = WindowState.Normal;
            };

        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

        _notifyIcon.ContextMenuStrip.Items.Add("종료", null, (sender, args) => { Close(); });
        

        LoadImages();

        LoadNotifications();

        InitializeAutoSwipeTimer();
    }

    private void LoadNotifications()
    {
        var dir = "C:\\Users\\PC\\Documents\\GenshinLauncher\\GenshinLauncher\\Notifications";

        var events = File.ReadAllText($"{dir}\\Events.json", Encoding.UTF8);
        var notifications = File.ReadAllText($"{dir}\\Notifications.json", Encoding.UTF8);

        var eventsJson = JsonSerializer.Deserialize<List<NotificateItem>>(events);
        var notificationsJson = JsonSerializer.Deserialize<List<NotificateItem>>(notifications);

        if (eventsJson == null || notificationsJson == null) return;

        foreach (var item in eventsJson)
        {
            // Create Button
            Button button = new Button();
            button.Template = (ControlTemplate)Application.Current.Resources["NoMouseOverButtonTemplate"];
            button.Cursor = Cursors.Hand;
            button.Background = Brushes.Transparent;
            button.Foreground = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);
            button.Click += (sender, args) =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = item.Url,
                    UseShellExecute = true
                };

                Process.Start(psi);
            };

            // Create Grid
            Grid grid = new Grid();
            grid.Width = 392;
            grid.Height = 33;
            grid.Margin = new Thickness(0);

            // Create TextBlock 1
            TextBlock textBlock1 = new TextBlock();
            textBlock1.Text = item.Text;
            textBlock1.FontSize = 15;
            textBlock1.FontFamily = new FontFamily("NanumGothic");
            textBlock1.Foreground = Brushes.White;
            textBlock1.FontWeight = FontWeights.SemiBold;
            textBlock1.VerticalAlignment = VerticalAlignment.Center;
            textBlock1.MouseEnter += Textbox_FadeInColor;
            textBlock1.MouseLeave += Textbox_FadeOutColor;

            // Create TextBlock 2
            TextBlock textBlock2 = new TextBlock();
            textBlock2.Text = item.Day;
            textBlock2.FontSize = 15;
            textBlock2.FontFamily = new FontFamily("NanumGothic");
            textBlock2.Foreground = new SolidColorBrush(Color.FromArgb(255, 154, 154, 154)); // #FF9A9A9A in ARGB
            textBlock2.FontWeight = FontWeights.SemiBold;
            textBlock2.VerticalAlignment = VerticalAlignment.Center;
            textBlock2.HorizontalAlignment = HorizontalAlignment.Right;

            // Add TextBlocks to Grid
            grid.Children.Add(textBlock1);
            grid.Children.Add(textBlock2);

            // Add Grid to Button
            button.Content = grid;

            // Add Button to StackPanel
            StackEvents.Children.Add(button);
        }

        foreach (var notificateItem in notificationsJson)
        {
            // Create Button
            Button button = new Button();
            button.Template = (ControlTemplate)Application.Current.Resources["NoMouseOverButtonTemplate"];
            button.Cursor = Cursors.Hand;
            button.Background = Brushes.Transparent;
            button.Foreground = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);
            button.Click += (sender, args) =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = notificateItem.Url,
                    UseShellExecute = true
                };

                Process.Start(psi);
            };

            // Create Grid
            Grid grid = new Grid();
            grid.Width = 392;
            grid.Height = 33;
            grid.Margin = new Thickness(0);

            // Create TextBlock 1
            TextBlock textBlock1 = new TextBlock();
            textBlock1.Text = notificateItem.Text;
            textBlock1.FontSize = 15;
            textBlock1.FontFamily = new FontFamily("NanumGothic");
            textBlock1.Foreground = Brushes.White;
            textBlock1.FontWeight = FontWeights.SemiBold;
            textBlock1.VerticalAlignment = VerticalAlignment.Center;
            textBlock1.MouseEnter += Textbox_FadeInColor;
            textBlock1.MouseLeave += Textbox_FadeOutColor;

            // Create TextBlock 2
            TextBlock textBlock2 = new TextBlock();
            textBlock2.Text = notificateItem.Day;
            textBlock2.FontSize = 15;
            textBlock2.FontFamily = new FontFamily("NanumGothic");
            textBlock2.Foreground = new SolidColorBrush(Color.FromArgb(255, 154, 154, 154)); // #FF9A9A9A in ARGB
            textBlock2.FontWeight = FontWeights.SemiBold;
            textBlock2.VerticalAlignment = VerticalAlignment.Center;
            textBlock2.HorizontalAlignment = HorizontalAlignment.Right;

            // Add TextBlocks to Grid
            grid.Children.Add(textBlock1);
            grid.Children.Add(textBlock2);

            // Add Grid to Button
            button.Content = grid;

            // Add Button to StackPanel
            StackNotifications.Children.Add(button);
        }
    }

    private DispatcherTimer _autoSwipeTimer;

    private void InitializeAutoSwipeTimer()
    {
        _autoSwipeTimer = new DispatcherTimer();
        _autoSwipeTimer.Interval = TimeSpan.FromSeconds(10);
        _autoSwipeTimer.Tick += AutoSwipeTimer_Tick;
        _autoSwipeTimer.Start();
    }

    private void AutoSwipeTimer_Tick(object sender, EventArgs e)
    {
        MoveToNextImg();
    }

    private void ResetAutoSwipeTimer()
    {
        _autoSwipeTimer.Stop();
        _autoSwipeTimer.Start();
    }


    private void LoadImages()
    {
        var dir = "C:\\Users\\PC\\Documents\\GenshinLauncher\\GenshinLauncher\\SplashImages";

        var files = Directory.GetFiles(dir, "*.json");

        var idx = 0;

        GridLeft.Opacity = 0;
        GridRight.Opacity = 0;
        GridCenterButtons.Opacity = 0;

        GridLeft.Visibility = Visibility.Hidden;
        GridRight.Visibility = Visibility.Hidden;
        GridCenterButtons.Visibility = Visibility.Hidden;

        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var splashImage = JsonSerializer.Deserialize<SplashImage>(json);

            if (splashImage == null) continue;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(splashImage.Path);
            bitmap.EndInit();

            _splashImages.Add(bitmap);

            var btn = new Button
            {
                Width = 432,
                Height = 200,
                Background = Brushes.Transparent,
                Template = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"),
                BorderThickness = new Thickness(0)
            };

            var img = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Fill
            };

            _images.Add(img);
            btn.Content = img;
            btn.Cursor = Cursors.Hand;

            btn.Click += (sender, args) =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = splashImage.Url,
                    UseShellExecute = true
                };

                Process.Start(psi);
            };

            _buttons.Add(btn);
            CanvasImages.Children.Add(btn);

            var button = new Button();
            button.Width = 20;
            button.Height = 20;
            button.Template = (ControlTemplate)FindResource("NoMouseOverButtonTemplate");
            button.Background = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);
            button.Cursor = Cursors.Hand;
            button.Click += (sender, args) => { BtnCenter_Click(sender, args); };

            var ellipse = new Ellipse();
            ellipse.Width = 15;
            ellipse.Height = 15;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.White;
            ellipse.Fill = Brushes.White;

            if (idx == 0)
                _currentEllipse = ellipse;

            button.Content = ellipse;

            if (idx != 0) ellipse.Fill = Brushes.Transparent;

            StackPanelButtons.Children.Add(button);

            _centerButtons.Add(button);

            Canvas.SetLeft(btn, 432 * idx);
            Canvas.SetTop(btn, 0);

            idx++;
        }
    }

    public void mouse_MoveWindow(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void btn_Minimize(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private bool _toastShown;

    private void btn_Close(object sender, RoutedEventArgs e)
    {
        if (_closeSettings == -1)
        {
            GridClose.Visibility = Visibility.Visible;
        }
        else
        {
            if (_closeSettings == 1)
            {
                Close();
            }
            else
            {
                if (!_toastShown)
                {
                    _notifyIcon.ShowBalloonTip(5);

                    _toastShown = true;
                }

                Hide();
            }
        }
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
        if (_isDownloading) return;

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
        if (_isDownloading) return;

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
            FileName = "https://act.hoyoverse.com/ys/event/e20210601blue_post/vert.html?page_sn=3719b7221f4c4efc&mhy_presentation_style=fullscreen#/update",
            UseShellExecute = true
        };

        Process.Start(psi);
    }

    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        if (_isDownloading) return;

        if (!_isDownloaded)
            Download();
        else
            StartGame();
    }

    private void StartGame()
    {
        if (!File.Exists("C:\\Program Files\\Genshin Impact\\Genshin Impact Game\\GenshinImpact.exe"))
        {
            MessageBox.Show("원래 실행이 되어야 하는데... 원신 없어요? 쩝", "흐음...", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var processStartInfo =
            new ProcessStartInfo("C:\\Program Files\\Genshin Impact\\Genshin Impact Game\\GenshinImpact.exe");

        processStartInfo.UseShellExecute = true;
        processStartInfo.Verb = "runas";

        Process.Start(processStartInfo);
    }

    private void Download()
    {
        GridDownload.Visibility = Visibility.Visible;
        ProgressDownload.Value = 0f;
        _isDownloading = true;
        _downloadSize = Random.Shared.NextInt64(5000000000, 20000000000);
        var bc = new BrushConverter();
        BtnStart.Background = bc.ConvertFrom("#FFBA8F00") as Brush;
        BtnSettings.Background = bc.ConvertFrom("#FFBA8F00") as Brush;
        BtnStart.Cursor = Cursors.Arrow;
        BtnSettings.Cursor = Cursors.Arrow;

        ResumeDownload();
    }

    private async void ResumeDownload()
    {
        while (_isDownloading) await DownloadTask();
    }

    private async Task DownloadTask()
    {
        var waitRandom = Random.Shared.Next(500, 800);

        await Task.Delay(waitRandom);

        if (!_isPaused && _isDownloading)
        {
            var random = Random.Shared.NextInt64(110000000, 120000000);
            _downloadedSize += random;
            _downloadProgress = _downloadedSize / (float)_downloadSize * 100f;

            if (_downloadProgress >= 100f)
            {
                _isDownloading = false;
                _isDownloaded = true;

                GridDownload.Visibility = Visibility.Hidden;

                var bc = new BrushConverter();

                BtnStart.Background = bc.ConvertFrom("#FFFFC70A") as Brush;
                BtnSettings.Background = bc.ConvertFrom("#FFFFC70A") as Brush;
                BtnStart.Cursor = Cursors.Hand;

                LabelStart.Content = "게임 실행";
            }

            SetPercent(ProgressDownload, _downloadProgress);
            LabelDownloadPercentage.Content =
                $"다운로드 중... | {_downloadProgress:0.##}% ({FormatBytes(_downloadedSize)} / {FormatBytes(_downloadSize)})";
            LabelDownloadSpeed.Content = $"다운로드 속도 | {FormatBytes(random)}/s";
            TextBlockETA.Text =
                $@"예상 시간 | {TimeSpan.FromSeconds((_downloadSize - _downloadedSize) / (float)random):hh\:mm\:ss}";
        }
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        if (_isDownloading) return;

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

        InvalidateMeasure();
    }

    private void BtnSettings_MouseEnter(object sender, MouseEventArgs e)
    {
        if (_isDownloading) return;

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
        if (_isDownloading) return;

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
        var btn = e.Source as Button;

        if (btn != null && (btn.Name == "BtnSettings" || !btn.Name.StartsWith("BtnStartingSettings")))
        {
        }
        else
        {
            GridStartingSettings.Visibility = Visibility.Hidden;
        }
    }


    private void BtnPause_Click(object sender, RoutedEventArgs e)
    {
        if (_isPaused)
        {
            _isPaused = false;

            LabelDownloadSpeed.Content = "다운로드 속도 | 0 B/s";

            GridPaused.Visibility = Visibility.Visible;
            GridResume.Visibility = Visibility.Hidden;
        }
        else
        {
            _isPaused = true;

            LabelDownloadSpeed.Content = "다운로드 속도 | 일시 중지됨";

            GridPaused.Visibility = Visibility.Hidden;
            GridResume.Visibility = Visibility.Visible;
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
        int i;
        double dblSByte = bytes;
        for (i = 0; i < Suffix.Length && bytes >= 1000; i++, bytes /= 1000) dblSByte = bytes / 1024.0;

        return $"{dblSByte:0.##} {Suffix[i]}";
    }

    private void BtnPause_MouseEnter(object sender, MouseEventArgs e)
    {
        var btn = sender as Button;
        var bc = new BrushConverter();

        if (btn != null)
        {
            btn.Background = bc.ConvertFrom("#b2ffffff") as Brush;
            btn.BorderBrush = bc.ConvertFrom("#b2ffffff") as Brush;
        }
    }

    private void BtnPause_MouseLeave(object sender, MouseEventArgs e)
    {
        var btn = sender as Button;
        var bc = new BrushConverter();

        if (btn != null)
        {
            btn.Background = bc.ConvertFrom("#4cffffff") as Brush;
            btn.BorderBrush = bc.ConvertFrom("#4cffffff") as Brush;
        }
    }

    private void BtnCloseClose_Click(object sender, RoutedEventArgs e)
    {
        GridClose.Visibility = Visibility.Hidden;
    }

    private int _closeSettings = -1;

    private void BtnCloseApp_Click(object sender, RoutedEventArgs e)
    {
        _closeSettings = 1;

        EllipseClose.Visibility = Visibility.Visible;
        EllipseMinimize.Visibility = Visibility.Hidden;
    }

    private void BtnMinimizeToTray_Click(object sender, RoutedEventArgs e)
    {
        _closeSettings = 0;

        EllipseClose.Visibility = Visibility.Hidden;
        EllipseMinimize.Visibility = Visibility.Visible;
    }

    private void LabelClose_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _closeSettings = 1;

        EllipseClose.Visibility = Visibility.Visible;
        EllipseMinimize.Visibility = Visibility.Hidden;
    }

    private void LabelMinimize_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _closeSettings = 0;

        EllipseClose.Visibility = Visibility.Hidden;
        EllipseMinimize.Visibility = Visibility.Visible;
    }

    private void BtnCloseOK_MouseEnter(object sender, MouseEventArgs e)
    {
        var btn = sender as Button;
        var bc = new BrushConverter();

        if (btn != null)
        {
            btn.Background = bc.ConvertFrom("#FF795A5A") as Brush;
            btn.BorderBrush = bc.ConvertFrom("#FF795A5A") as Brush;
        }
    }

    private void BtnCloseOK_MouseLeave(object sender, MouseEventArgs e)
    {
        var btn = sender as Button;
        var bc = new BrushConverter();

        if (btn != null)
        {
            btn.Background = bc.ConvertFrom("#FF3A2A2A") as Brush;
            btn.BorderBrush = bc.ConvertFrom("#FF3A2A2A") as Brush;
        }
    }

    private void BtnCloseOK_Click(object sender, RoutedEventArgs e)
    {
        if (_closeSettings == 1)
        {
            Close();
        }
        else
        {
            if (!_toastShown)
            {
                _notifyIcon.ShowBalloonTip(5);

                _toastShown = true;
            }

            Hide();
            GridClose.Visibility = Visibility.Hidden;
            if (_closeSettings == -1) _closeSettings = 0;
        }
    }

    private void BtnMinimizeToTray_MouseEnter(object sender, MouseEventArgs e)
    {
        var bc = new BrushConverter();

        EllipseMinimizeBorder.Stroke = bc.ConvertFrom("#FF696969") as Brush;
    }

    private void BtnMinimizeToTray_MouseLeave(object sender, MouseEventArgs e)
    {
        var bc = new BrushConverter();

        EllipseMinimizeBorder.Stroke = bc.ConvertFrom("#FFC7C7C7") as Brush;
    }

    private void BtnCloseApp_MouseEnter(object sender, MouseEventArgs e)
    {
        var bc = new BrushConverter();

        EllipseCloseBorder.Stroke = bc.ConvertFrom("#FF696969") as Brush;
    }

    private void BtnCloseApp_MouseLeave(object sender, MouseEventArgs e)
    {
        var bc = new BrushConverter();

        EllipseCloseBorder.Stroke = bc.ConvertFrom("#FFC7C7C7") as Brush;
    }

    private int _selectedTab;

    private void BtnInfo_MouseEnter(object sender, MouseEventArgs e)
    {
        if (_selectedTab == 1) return;
        BtnInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        ColorAnimation animation;
        animation = new ColorAnimation();
        animation.From = (Color)ColorConverter.ConvertFromString("#ffffff");
        animation.To = (Color)ColorConverter.ConvertFromString("#DCC9A6");
        animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
        BtnInfo.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }

    private void BtnInfo_MouseLeave(object sender, MouseEventArgs e)
    {
        if (_selectedTab == 1) return;
        ColorAnimation animation;
        animation = new ColorAnimation();
        animation.From = (Color)ColorConverter.ConvertFromString("#DCC9A6");
        animation.To = (Color)ColorConverter.ConvertFromString("#ffffff");
        animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
        BtnInfo.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }

    private void BtnEvent_MouseEnter(object sender, MouseEventArgs e)
    {
        if (_selectedTab == 0) return;
        BtnEvent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        ColorAnimation animation;
        animation = new ColorAnimation();
        animation.From = (Color)ColorConverter.ConvertFromString("#ffffff");
        animation.To = (Color)ColorConverter.ConvertFromString("#DCC9A6");
        animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
        BtnEvent.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }

    private void BtnEvent_MouseLeave(object sender, MouseEventArgs e)
    {
        if (_selectedTab == 0) return;
        ColorAnimation animation;
        animation = new ColorAnimation();
        animation.From = (Color)ColorConverter.ConvertFromString("#DCC9A6");
        animation.To = (Color)ColorConverter.ConvertFromString("#ffffff");
        animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
        BtnEvent.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }

    private void BtnEvent_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedTab == 0) return;

        _selectedTab = 0;

        RectInfo.Visibility = Visibility.Hidden;
        RectEvent.Visibility = Visibility.Visible;

        BtnEvent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCC9A6"));
        BtnInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));

        BorderEvent.Visibility = Visibility.Visible;
        BorderInfo.Visibility = Visibility.Hidden;
    }

    private void BtnInfo_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedTab == 1) return;

        _selectedTab = 1;

        RectInfo.Visibility = Visibility.Visible;
        RectEvent.Visibility = Visibility.Hidden;

        BtnInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCC9A6"));
        BtnEvent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));

        BorderInfo.Visibility = Visibility.Visible;
        BorderEvent.Visibility = Visibility.Hidden;
    }

    private void Textbox_FadeOutColor(object sender, RoutedEventArgs e)
    {
        var tb = (TextBlock)sender;
        tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCC9A6"));
        ColorAnimation animation;
        animation = new ColorAnimation();
        animation.From = (Color)ColorConverter.ConvertFromString("#DCC9A6");
        animation.To = (Color)ColorConverter.ConvertFromString("#ffffff");
        animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
        tb.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }

    private void Textbox_FadeInColor(object sender, RoutedEventArgs e)
    {
        var tb = (TextBlock)sender;
        tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        ColorAnimation animation;
        animation = new ColorAnimation();
        animation.From = (Color)ColorConverter.ConvertFromString("#ffffff");
        animation.To = (Color)ColorConverter.ConvertFromString("#DCC9A6");
        animation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
        tb.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }

    private void BtnCancel_MouseEnter(object sender, MouseEventArgs e)
    {
        var btn = sender as Button;
        var bc = new BrushConverter();

        if (btn != null)
        {
            btn.Background = bc.ConvertFrom("#b2ffffff") as Brush;
            btn.BorderBrush = bc.ConvertFrom("#b2ffffff") as Brush;
        }
    }

    private void BtnCancel_MouseLeave(object sender, MouseEventArgs e)
    {
        var btn = sender as Button;
        var bc = new BrushConverter();

        if (btn != null)
        {
            btn.Background = bc.ConvertFrom("#4cffffff") as Brush;
            btn.BorderBrush = bc.ConvertFrom("#4cffffff") as Brush;
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        _isDownloading = false;
        _isPaused = false;
        _isDownloaded = false;

        _downloadProgress = 0f;
        _downloadSize = 0;
        _downloadedSize = 0;

        GridDownload.Visibility = Visibility.Hidden;

        var bc = new BrushConverter();

        BtnStart.Background = bc.ConvertFrom("#FFFFC70A") as Brush;
        BtnStart.Cursor = Cursors.Hand;
    }

    private int _currentImageIdx;
    private Ellipse _currentEllipse;

    private void MoveToNextImg()
    {
        MoveToImg(_currentImageIdx + 1);
    }

    private void BtnCenter_Click(object sender, EventArgs e)
    {
        var idx = _centerButtons.IndexOf((Button)sender);

        MoveToImg(idx);
    }

    private void MoveToPrevImg()
    {
        MoveToImg(_currentImageIdx - 1);
    }

    private bool _isSwiping;

    private void MoveToImg(int idx)
    {
        if (_isSwiping) return;

        var originalIdx = _currentImageIdx;

        if (idx < 0 || idx >= _splashImages.Count) idx = idx < 0 ? _splashImages.Count - 1 : 0;

        _currentImageIdx = idx;

        var idxDiff = _currentImageIdx - originalIdx;

        _currentEllipse.Fill = Brushes.Transparent;

        var img = _splashImages[_currentImageIdx];

        _currentEllipse = (Ellipse)_centerButtons[_currentImageIdx].Content;

        _currentEllipse.Fill = Brushes.White;

        var storyboard = new Storyboard();

        foreach (var button in _buttons)
        {
            var newLeft = Canvas.GetLeft(button) - 432 * idxDiff;

            var doubleAnimation = new DoubleAnimation
            {
                To = newLeft,
                Duration = TimeSpan.FromSeconds(0.4)
            };

            doubleAnimation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseInOut
            };

            Storyboard.SetTarget(doubleAnimation, button);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(Canvas.LeftProperty));

            storyboard.Children.Add(doubleAnimation);
        }

        storyboard.Completed += (sender, args) => { _isSwiping = false; };

        storyboard.Begin();
        _isSwiping = true;

        ResetAutoSwipeTimer();
    }

    public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null) yield return (T)Enumerable.Empty<T>();
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var ithChild = VisualTreeHelper.GetChild(depObj, i);
            if (ithChild == null) continue;
            if (ithChild is T t) yield return t;
            foreach (var childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
        }
    }

    private void GridImages_MouseEnter(object sender, MouseEventArgs e)
    {
        GridLeft.Visibility = Visibility.Visible;
        GridRight.Visibility = Visibility.Visible;
        GridCenterButtons.Visibility = Visibility.Visible;

        var fadeInAnimation = new DoubleAnimation();
        fadeInAnimation.From = 0.0;
        fadeInAnimation.To = 1.0;
        fadeInAnimation.Duration = TimeSpan.FromSeconds(0.2);

        Storyboard.SetTarget(fadeInAnimation, GridLeft);
        Storyboard.SetTarget(fadeInAnimation, GridRight);
        Storyboard.SetTarget(fadeInAnimation, GridCenterButtons);

        Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));

        GridLeft.BeginAnimation(OpacityProperty, fadeInAnimation);
        GridRight.BeginAnimation(OpacityProperty, fadeInAnimation);
        GridCenterButtons.BeginAnimation(OpacityProperty, fadeInAnimation);
    }

    private void GridImages_MouseLeave(object sender, MouseEventArgs e)
    {
        var fadeOutAnimation = new DoubleAnimation();
        fadeOutAnimation.From = 1.0;
        fadeOutAnimation.To = 0.0;
        fadeOutAnimation.Duration = TimeSpan.FromSeconds(0.2);

        Storyboard.SetTarget(fadeOutAnimation, GridLeft);
        Storyboard.SetTarget(fadeOutAnimation, GridRight);
        Storyboard.SetTarget(fadeOutAnimation, GridCenterButtons);

        Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

        GridLeft.BeginAnimation(OpacityProperty, fadeOutAnimation);
        GridRight.BeginAnimation(OpacityProperty, fadeOutAnimation);
        GridCenterButtons.BeginAnimation(OpacityProperty, fadeOutAnimation);

        fadeOutAnimation.Completed += FadeOutStoryboard_Completed;
    }

    private void FadeOutStoryboard_Completed(object? sender, EventArgs e)
    {
        GridLeft.Visibility = Visibility.Hidden;
        GridRight.Visibility = Visibility.Hidden;
        GridCenterButtons.Visibility = Visibility.Hidden;
    }

    private void BtnLeft_MouseEnter(object sender, MouseEventArgs e)
    {
        BtnLeft.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
    }

    private void BtnLeft_MouseLeave(object sender, MouseEventArgs e)
    {
        BtnLeft.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
    }

    private void BtnRight_MouseEnter(object sender, MouseEventArgs e)
    {
        BtnRight.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
    }

    private void BtnRight_MouseLeave(object sender, MouseEventArgs e)
    {
        BtnRight.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
    }

    private void BtnLeft_Click(object sender, RoutedEventArgs e)
    {
        MoveToPrevImg();
    }

    private void BtnRight_Click(object sender, RoutedEventArgs e)
    {
        MoveToNextImg();
    }

    public static void SetPercent(ProgressBar progressBar, double percentage)
    {
        DoubleAnimation animation = new DoubleAnimation(percentage, TimeSpan.FromSeconds(0.5));
        animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
        progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
    }

    private void BtnStartSettings1_Click(object sender, RoutedEventArgs e)
    {
        GridStartingSettings.Visibility = Visibility.Hidden;
        if (!Directory.Exists("C:\\Program Files\\Genshin Impact\\Genshin Impact game"))
        {
            MessageBox.Show("원래 실행이 되어야 하는데... 원신 없어요? 쩝", "흐음...", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Process.Start("explorer.exe", @"C:\Program Files\Genshin Impact\Genshin Impact game");
    }

    private void BtnStartSettings3_Click(object sender, RoutedEventArgs e)
    {
        GridStartingSettings.Visibility = Visibility.Hidden;

        if (!_isDownloaded) return;

        _isDownloaded = false;
        _isDownloading = false;
        _isPaused = false;
        _downloadProgress = 0f;
        _downloadSize = 0;
        _downloadedSize = 0;

        LabelStart.Content = "다운로드";
    }
}