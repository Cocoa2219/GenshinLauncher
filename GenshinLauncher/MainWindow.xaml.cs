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

namespace GenshinLauncher
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

        public void mouse_MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btn_Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btn_Close(object sender, RoutedEventArgs e) {
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
            var psi = new ProcessStartInfo
            {
                FileName = "https://namu.wiki/w/붕괴: 스타레일",
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            switch (GridStartingSettings.Visibility)
            {
                case Visibility.Visible:
                    GridStartingSettings.Visibility = Visibility.Hidden;
                    break;
                case Visibility.Hidden:
                    GridStartingSettings.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}