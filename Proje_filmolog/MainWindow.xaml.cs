using System;
using System.Windows;
using System.Windows.Threading;
using Proje_filmolog.Login;

namespace Proje_filmolog
{
    /// <summary>
    ///     MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(12)
            };
            timer.Tick += TickTock;
            timer.Start();
        }

        private void TickTock(object sender, EventArgs e)
        {
            timer.Stop();
            Hide();
            var win = new LoginWin();
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