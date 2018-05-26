using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Proje_filmolog.Other
{
    /// <summary>
    /// Interaction logic for BAR.xaml
    /// </summary>
    public partial class BAR : UserControl
    {
        public BAR() => InitializeComponent();

        private void ExitWin(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("EMİN MİSİNİZ ?", "KAPAT", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                Environment.Exit(0);
            }
        }

        private void MinWin(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            win.WindowState = WindowState.Minimized;
        }

        private void MoveWin(object sender, MouseButtonEventArgs e)
        {
            Window win = Window.GetWindow(this);
            win.DragMove();
        }

        private void GRİD_Loaded(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            Title.Content = win.Title;
        }
    }
}
