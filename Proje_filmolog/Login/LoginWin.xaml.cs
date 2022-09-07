using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;
using Proje_filmolog.Admin;
using Proje_filmolog.Other;
using Proje_filmolog.Users;

namespace Proje_filmolog.Login
{
    /// <summary>
    ///     Interaction logic for LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
        private readonly SQLiteConnection connection = new SQLiteConnection("data source = filmolog.db; Version=3;");

        public LoginWin()
        {
            InitializeComponent();
            UserNameTbox.Focus();
        }

        private void LoginController()
        {
            var user = UserNameTbox.Text.Trim();
            var password = PassWordTbox.Password;
            var isAdmin = false;
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                var cmd = new SQLiteCommand("select * from Users where userName = @uname and isActive=@isActive",
                    connection);
                cmd.Parameters.AddWithValue("@uname", user);
                cmd.Parameters.AddWithValue("@isActive", true);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cmd.Dispose();
                        if (password.Equals(reader["password"].ToString()))
                        {
                            Temp.ActiveUser = user;
                            isAdmin = Convert.ToBoolean(reader["isAdmin"]);
                            reader.Close();
                            if (isAdmin)
                            {
                                var win = new FilmEditWin();
                                Temp.AccType = 1;
                                win.Show();
                            }
                            else
                            {
                                var win = new UsersWin();
                                Temp.AccType = 0;
                                win.Show();
                            }

                            connection.Close();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Şifre Yanlış");
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Kullanıcı Bulunamıyor Yada Kullanıcı Erişimi Engellenmiş Olabilir!!\nKullanıcı Adını kontrol Edip Tekrar Deneyin.\nBaşaramazsanız Bir Yönetici ile İletişime Geçin..",
                            "HATA", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No: 003\nHatan mesajı:" + ex);
            }

            UserNameTbox.Clear();
            PassWordTbox.Clear();
            UserNameTbox.Focus();
        }

        private void GRİD_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PassWordTbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
                LoginController();
        }

        /// <summary>
        ///     oturum açılır
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login(object sender, RoutedEventArgs e)
        {
            LoginController();
        }

        /// <summary>
        ///     hesabı olamayan kullanıcılar hesap oluşturma ekranına gider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sign(object sender, RoutedEventArgs e)
        {
            var win = new SignWin();
            win.Show();
            Close();
        }
    }
}