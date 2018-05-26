using Proje_filmolog.Login;
using Proje_filmolog.Other;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Proje_filmolog.Users
{
    /// <summary>
    /// Interaction logic for UsersWin.xaml
    /// </summary>
    public partial class UsersWin : Window
    {
        public UsersWin()
        {
            InitializeComponent();
            LoadListViewData();
        }
        int flm_isGood = -1;
        ArrayList filmDataList = new ArrayList();
        ArrayList myFavoriteDataList = new ArrayList();
        /// <summary>
        /// kullanımda olan listi tutar
        /// 1:tüm liste
        /// 0:favori listesi
        /// </summary>
        int ActiveList = 0;
        private SQLiteConnection connection = new SQLiteConnection("data source = filmolog.db; Busy Timeout=10000;Version=3;");

        private void UsersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            String ilk_film = Convert.ToString(FilmListView.Items.GetItemAt(0));
            Get_film_info(ilk_film);
        }
        String secilen_film;
        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            secilen_film = Convert.ToString(FilmListView.SelectedItem);
            Get_film_info(secilen_film);
            film_hakkinda.Background = Brushes.White;
            tb_yorum.Background = Brushes.White;
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
            ArrayList itemsList = new ArrayList();
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from film", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                    itemsList.Add(reader["name"].ToString());
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:userwindow001\nHata mesajı:\n" + ex.ToString());
            }
            finally { connection.Close(); }
            FilmListView.ItemsSource = itemsList;
        }

        private void LoadFavoriteListViewData()
        {
            ArrayList itemsList = new ArrayList();
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from comment where userName=@uName", connection);
                command.Parameters.AddWithValue("@uName", TEMP.active_User);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    if (Convert.ToBoolean(reader["isGood"]).Equals(true))
                        itemsList.Add(reader["filName"].ToString());
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:userwindow002\nHata mesajı:\n" + ex.ToString());
            }
            finally { connection.Close(); }
            FilmListView.ItemsSource = itemsList;
        }

        private void Btn_gonder_Click(object sender, RoutedEventArgs e)
        {
            if (connection.State == ConnectionState.Closed) connection.Open();
            if (!tb_yorum.Text.Trim().Equals(""))
            {
                SQLiteCommand cmd = new SQLiteCommand("insert into comment (userName,filName,remark,isGood) values (@u_name,@flm_name,@flm_remark,@flm_isGood)", connection);
                cmd.Parameters.AddWithValue("@u_name", TEMP.active_User);
                cmd.Parameters.AddWithValue("@flm_name", film_adi.Content);
                cmd.Parameters.AddWithValue("@flm_remark", tb_yorum.Text);
                cmd.Parameters.AddWithValue("@flm_isGood", flm_isGood);
                try
                {
                    if (connection.State == ConnectionState.Closed) connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    connection.Close();
                    tb_yorum.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata No:userwindow002\nHata mesajı:\n" + ex.ToString());
                }
                finally { connection.Close(); }
            }
            else
                MessageBox.Show("Bir Yorum Yazın!!");
        }

        private void Get_film_info(String film_ad)
        {
            tb_yorum.Clear();
            listView_yorumlar.Items.Clear();
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from film", connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["name"].Equals(film_ad))
                    {
                        film_adi.Content = reader["name"].ToString();
                        yonetmen.Content = reader["dir"].ToString();
                        oyuncular.Content = reader["staring"].ToString();
                        puan.Content = reader["rate"].ToString();
                        film_hakkinda.Text = reader["info"].ToString();
                    }
                }
                reader.Close();
                SQLiteCommand command1 = new SQLiteCommand("Select * from comment", connection);
                SQLiteDataReader reader1 = command1.ExecuteReader();
                while (reader1.Read())
                {
                    if (reader1["userName"].Equals(TEMP.active_User) && (reader1["filName"]).Equals(film_ad))
                    {
                        tb_yorum.Text = reader1["remark"].ToString();
                        TEMP.isGood = Convert.ToInt32(reader1["isGood"]);
                    }
                    if (reader1["filName"].Equals(film_ad))
                    {
                        listView_yorumlar.Items.Add(reader1["userName"].ToString() + ":\n  " + reader1["remark"].ToString());
                    }
                }
                reader1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:userwindow004\nHata mesajı:\n" + ex.ToString());
            }
            finally { connection.Close(); }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ArrayList liste = new ArrayList();
            if (ActiveList == 0 || ActiveList == 1) LoadListViewData();
            else if (ActiveList == 2) LoadFavoriteListViewData();
            NotFoundLabel.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                for (int i = FilmListView.Items.Count - 1; i >= 0; i--)
                {
                    var item = FilmListView.Items[i];
                    if (item.ToString().ToLower().Contains(SearchBox.Text.Trim().ToLower()))
                    {
                        liste.Add(item.ToString());
                    }
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
                FilmListView.ItemsSource = liste;
                NotFoundLabel.Visibility = (liste.Count > 0) ? Visibility.Collapsed : Visibility.Visible;
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
            LoginWin win = new LoginWin();
            win.Show(); Close();
        }

        private void Good_Click(object sender, RoutedEventArgs e)
        {
            if (connection.State == ConnectionState.Closed) connection.Open();
            flm_isGood = ((sender as Button).Content.Equals("J")) ? 1 : 0;
            if (flm_isGood == 1)
            {
                tb_yorum.Background = film_hakkinda.Background = Brushes.LightGreen;
            }
            else
            {
                tb_yorum.Background = film_hakkinda.Background = Brushes.LightPink;
            }
            if (TEMP.isGood != flm_isGood)
            {
                SQLiteCommand command = new SQLiteCommand("update comment set isGood=@isGood where userName=@uName and filName=@fName", connection);
                command.Parameters.AddWithValue("@fName", film_adi.Content);
                command.Parameters.AddWithValue("@uName", TEMP.active_User);
                command.Parameters.AddWithValue("@isGood", flm_isGood);
                command.ExecuteNonQuery();
            }
            connection.Close();
            Get_film_info(secilen_film);
            LoadFavoriteListViewData();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            InfoWin win = new InfoWin();
            win.Show();
            Close();
        }
    }
}