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
                btn.Background = null;
                btn.BorderBrush = null;
            }
        }
    }
}