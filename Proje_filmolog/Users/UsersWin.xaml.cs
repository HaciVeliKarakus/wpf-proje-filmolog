using System;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Proje_filmolog.Login;
using Proje_filmolog.Other;

namespace Proje_filmolog.Users
{
    /// <summary>
    ///     Interaction logic for UsersWin.xaml
    /// </summary>
    public partial class UsersWin : Window
    {
        private readonly SQLiteConnection connection =
            new SQLiteConnection("data source = filmolog.db; Busy Timeout=10000;Version=3;");

        private string _choosenFilm;
        private int _flmİsGood = -1;

        /// <summary>
        ///     kullanımda olan listi tutar
        ///     1:tüm liste
        ///     0:favori listesi
        /// </summary>
        private int ActiveList;

        private ArrayList filmDataList = new ArrayList();
        private ArrayList myFavoriteDataList = new ArrayList();

        public UsersWin()
        {
            InitializeComponent();
            LoadListViewData();
        }

        private void UsersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var ilk_film = Convert.ToString(FilmListView.Items.GetItemAt(0));
            Get_film_info(ilk_film);
        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            _choosenFilm = Convert.ToString(FilmListView.SelectedItem);
            Get_film_info(_choosenFilm);
            FilmHakkinda.Background = Brushes.White;
            TbYorum.Background = Brushes.White;
        }

        private void AllFilmButton_Click(object sender, RoutedEventArgs e)
        {
            ActiveList = 1;
            LoadListViewData();
        }

        private void FavFilmButton_Click(object sender, RoutedEventArgs e)
        {
            ActiveList = 2;
            LoadFavoriteListViewData();
            //String ilk_film = Convert.ToString(FilmListView.Items.GetItemAt(1));
            //Get_film_info(ilk_film);
        }

        private void LoadListViewData()
        {
            var itemList = new ArrayList();
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                var command = new SQLiteCommand("Select * from film", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                    itemList.Add(reader["name"].ToString());
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:userwindow001\nHata mesajı:\n" + ex);
            }
            finally
            {
                connection.Close();
            }

            FilmListView.ItemsSource = itemList;
        }

        private void LoadFavoriteListViewData()
        {
            var itemsList = new ArrayList();
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                var command = new SQLiteCommand("Select * from comment where userName=@uName", connection);
                command.Parameters.AddWithValue("@uName", Temp.ActiveUser);
                var reader = command.ExecuteReader();
                while (reader.Read())
                    if (Convert.ToBoolean(reader["isGood"]).Equals(true))
                        itemsList.Add(reader["filName"].ToString());
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:userwindow002\nHata mesajı:\n" + ex);
            }
            finally
            {
                connection.Close();
            }

            FilmListView.ItemsSource = itemsList;
        }

        private void Btn_gonder_Click(object sender, RoutedEventArgs e)
        {
            if (connection.State == ConnectionState.Closed) connection.Open();
            if (!TbYorum.Text.Trim().Equals(""))
            {
                var cmd = new SQLiteCommand(
                    "insert into comment (userName,filName,remark,isGood) values (@u_name,@flm_name,@flm_remark,@flm_isGood)",
                    connection);
                cmd.Parameters.AddWithValue("@u_name", Temp.ActiveUser);
                cmd.Parameters.AddWithValue("@flm_name", FilmAdi.Content);
                cmd.Parameters.AddWithValue("@flm_remark", TbYorum.Text);
                cmd.Parameters.AddWithValue("@flm_isGood", _flmİsGood);
                try
                {
                    if (connection.State == ConnectionState.Closed) connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    connection.Close();
                    TbYorum.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata No:userwindow002\nHata mesajı:\n" + ex);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Bir Yorum Yazın!!");
            }
        }

        private void Get_film_info(string filmAd)
        {
            TbYorum.Clear();
            ListViewYorumlar.Items.Clear();
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                var command = new SQLiteCommand("Select * from film", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                    if (reader["name"].Equals(filmAd))
                    {
                        FilmAdi.Content = reader["name"].ToString();
                        Yonetmen.Content = reader["dir"].ToString();
                        Oyuncular.Content = reader["staring"].ToString();
                        Puan.Content = reader["rate"].ToString();
                        FilmHakkinda.Text = reader["info"].ToString();
                    }

                reader.Close();
                var command1 = new SQLiteCommand("Select * from comment", connection);
                var reader1 = command1.ExecuteReader();
                while (reader1.Read())
                {
                    if (reader1["userName"].Equals(Temp.ActiveUser) && reader1["filName"].Equals(filmAd))
                    {
                        TbYorum.Text = reader1["remark"].ToString();
                        Temp.IsGood = Convert.ToInt32(reader1["isGood"]);
                    }

                    if (reader1["filName"].Equals(filmAd))
                        ListViewYorumlar.Items.Add(reader1["userName"] + ":\n  " + reader1["remark"]);
                }

                reader1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:userwindow004\nHata mesajı:\n" + ex);
            }
            finally
            {
                connection.Close();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var list = new ArrayList();
            switch (ActiveList)
            {
                case 0:
                case 1:
                    LoadListViewData();
                    break;
                case 2:
                    LoadFavoriteListViewData();
                    break;
            }

            NotFoundLabel.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                for (var i = FilmListView.Items.Count - 1; i >= 0; i--)
                {
                    var item = FilmListView.Items[i];
                    if (item.ToString().ToLower().Contains(SearchBox.Text.Trim().ToLower())) list.Add(item.ToString());
                }

                //if (connection.State == ConnectionState.Closed) connection.Open();
                //SQLiteCommand command = new SQLiteCommand("Select * from film where name=@name",connection);
                //command.Parameters.AddWithValue("@name", SearchBox.Text.Trim());
                //SQLiteDataReader reader = command.ExecuteReader();
                //while (reader.Read())
                //{
                //    liste.Add(reader["name"].ToString());
                //}
                //reader.Close();
                //connection.Close();
                FilmListView.ItemsSource = list;
                NotFoundLabel.Visibility = list.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                if (ActiveList == 0 || ActiveList == 1) LoadListViewData();
                else if (ActiveList == 2) LoadFavoriteListViewData();
                NotFoundLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void LogOut(object sender, RoutedEventArgs e)
        {
            var win = new LoginWin();
            win.Show();
            Close();
        }

        private void Good_Click(object sender, RoutedEventArgs e)
        {
            if (connection.State == ConnectionState.Closed) connection.Open();
            _flmİsGood = ((Button)sender).Content.Equals("J") ? 1 : 0;
            if (_flmİsGood == 1)
                TbYorum.Background = FilmHakkinda.Background = Brushes.LightGreen;
            else
                TbYorum.Background = FilmHakkinda.Background = Brushes.LightPink;
            if (Temp.IsGood != _flmİsGood)
            {
                var command = new SQLiteCommand(
                    "update comment set isGood=@isGood where userName=@uName and filName=@fName",
                    connection);
                command.Parameters.AddWithValue("@fName", FilmAdi.Content);
                command.Parameters.AddWithValue("@uName", Temp.ActiveUser);
                command.Parameters.AddWithValue("@isGood", _flmİsGood);
                command.ExecuteNonQuery();
            }

            connection.Close();
            Get_film_info(_choosenFilm);
            LoadFavoriteListViewData();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            var win = new InfoWin();
            win.Show();
            Close();
        }
    }
}