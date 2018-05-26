using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Proje_filmolog.Other;
using System;
using System.Windows.Media.Animation;
using Proje_filmolog.Login;

namespace Proje_filmolog.Admin
{
    /// <summary>
    /// AdminWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class FilmEditWin : Window
    {
        public FilmEditWin()
        {
            InitializeComponent();
            SearchBox.Focus();
        }

        private DoubleAnimation explorer;
        private int txtLenght = 0;

        FilmComponents filmAddComponent = new FilmComponents();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 99; i++)
                PageCbox.Items.Add(i);
            filmAddComponent.AddfilmsListView(StockList, 0);
        }

        private void PageCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilmComponents.Page = PageCbox.SelectedIndex - 1;
            if (PageCbox.SelectedIndex>0)
                GetButton.Focus();
        }

        private void GetButton_Click(object sender, RoutedEventArgs e)
        {
            FilmListView.Items.Clear();
            FilmComponents.ListOfFilm.Clear();
            filmAddComponent.GetFilmsFromWeb(FilmListView);
        }

        private void FilmListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FilmListView.SelectedIndex > -1)
            {
                filmAddComponent.AddFilmToDB(FilmComponents.ListOfFilm, FilmListView.SelectedIndex);
                StockList.Items.Clear();
                filmAddComponent.AddfilmsListView(StockList, 0);
            }
            else
                MessageBox.Show("Eklenecek Filmi Seçin!!");
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            InfoWin win = new InfoWin();
            win.Show();
            Close();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                explorer = new DoubleAnimation()
                {
                    From = 500,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(333)
                };
            }
            else
            {
                explorer = new DoubleAnimation()
                {
                    From = 0,
                    To = 500,
                    Duration = TimeSpan.FromMilliseconds(333)
                };
            }
            if (SearchBox.IsKeyboardFocusWithin && ((SearchBox.Text.Length == 1 && (txtLenght - 1 == 0 || txtLenght == 0)) || string.IsNullOrEmpty(SearchBox.Text)))
                Animator();
            txtLenght = SearchBox.Text.Length;
            SearchList.Items.Clear();
            if (filmAddComponent.FindFilmFromDB(SearchBox.Text.Trim(), SearchList))
            {
                NotFoundTxt.Visibility = Visibility.Collapsed;
            }
            else
                NotFoundTxt.Visibility = Visibility.Visible;
        }

        private void Animator()
        {
            Storyboard board = new Storyboard();
            board.Children.Add(explorer);
            Storyboard.SetTarget(explorer, SearchList);
            Storyboard.SetTargetProperty(explorer, new PropertyPath(WidthProperty));
            board.Begin();
        }

        private void StockList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewSelectedItemDelete();
        }

        private void UserEditBtn_Click(object sender, RoutedEventArgs e)
        {
            UserEditWin win = new UserEditWin();
            win.Show();
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Oturumu kapatmak istiyor musunuz?", "", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                LoginWin win = new LoginWin();
                win.Show();
                Close();
            }
        }

        private void FilmListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
            {
                filmAddComponent.AddToDB(FilmComponents.ListOfFilm, FilmListView.SelectedItems);
                StockList.Items.Clear();
                filmAddComponent.AddfilmsListView(StockList, 0);
            }
            else if (Key.Delete == e.Key)
            {
                FilmListView.Items.Remove(FilmListView.SelectedItems);
            }
        }

        private void StockList_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                ListViewSelectedItemDelete();
            }
        }

        private void ListViewSelectedItemDelete()
        {
            if (StockList.SelectedIndex > -1)
            {
                filmAddComponent.DeleteFilmFromDB(StockList);
                StockList.Items.RemoveAt(StockList.SelectedIndex);
                StockList.Items.Clear();
                filmAddComponent.AddfilmsListView(StockList, 0);
            }
            else if (SearchList.SelectedIndex > -1)
            {
                filmAddComponent.DeleteFilmFromDB(SearchList);
                SearchList.Items.RemoveAt(SearchList.SelectedIndex);
                StockList.Items.Clear();
                SearchList.Items.Clear();
                filmAddComponent.AddfilmsListView(StockList, 0);
                SearchBox.Focus();
            }
            else
                MessageBox.Show("Silinecek Filmi Seçin!!");
        }
    }
}
