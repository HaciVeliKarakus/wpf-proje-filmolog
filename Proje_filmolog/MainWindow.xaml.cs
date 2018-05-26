using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Proje_filmolog
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DispatcherTimer timer = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(12),
            };
            timer.Tick += new EventHandler(TickTock);
            timer.Start();
        }

        private void TickTock(object sender, EventArgs e)
        {
            timer.Stop();
            Hide();
            Login.LoginWin win = new Login.LoginWin();
            win.ShowDialog();
        }

        private void StringAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            rota.Angle = 0;
            HeaderText.FontWeight = FontWeights.ExtraBold;
            HeaderText.FontSize = 170;
            HeaderText.Content = "FİLMOLOG";
        }
    }
}
