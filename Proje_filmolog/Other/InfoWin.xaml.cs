using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Proje_filmolog.Admin;
using Proje_filmolog.Users;

namespace Proje_filmolog.Other
{
    /// <summary>
    ///     Interaction logic for InfoWin.xaml
    /// </summary>
    public partial class InfoWin : Window
    {
        private const string Adress = "data source = filmolog.db;Read Only=False;Version=3;";

        public InfoWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var con = new SQLiteConnection(Adress))
            using (var cmd = new SQLiteCommand("SELECT * FROM Users WHERE userName=@uName", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@uName", Temp.ActiveUser);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        UserNameTxt.Content = reader.GetString(0);
                        RealNameTbox.Text = reader.GetString(2);
                        TelNoTbox.Text = reader.GetInt64(3).ToString();
                        AccTypeTbox.Content = reader.GetByte(4) == 1 ? "Admin" : "User";
                        reader.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sorunla karşılaşıldı", "OPPPS", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                con.Close();
                cmd.Dispose();
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PassWordTbox.Password) && !string.IsNullOrEmpty(PassWord2Tbox.Password) &&
                !string.IsNullOrEmpty(TelNoTbox.Text.Trim()) && !string.IsNullOrEmpty(RealNameTbox.Text.Trim()))
            {
                var con = new SQLiteConnection(Adress);
                if (con.State == ConnectionState.Open) con.Close();
                con.Open();
                using (var cmd = new SQLiteCommand(
                           "UPDATE Users SET password=@password,realName=@realName,telNo=@telNo WHERE userName=@userName",
                           con))
                {
                    cmd.Parameters.AddWithValue("@userName", Temp.ActiveUser);
                    cmd.Parameters.AddWithValue("@password", PassWord2Tbox.Password);
                    cmd.Parameters.AddWithValue("@realName", RealNameTbox.Text);
                    cmd.Parameters.AddWithValue("@telNo", TelNoTbox.Text);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Değişiklikler Kaydedildi", "TAMAM", MessageBoxButton.OK, MessageBoxImage.Information);
                con.Close();
            }
            else
            {
                MessageBox.Show("Gerekli Alanlar Boş olduğu İçin Değişiklikler Kaydedilmedi!!", "DİKKAT",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (Temp.AccType == 1)
            {
                var win = new FilmEditWin();
                win.Show();
            }
            else
            {
                var win = new UsersWin();
                win.Show();
            }

            Close();
        }

        private void TelNoTbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}