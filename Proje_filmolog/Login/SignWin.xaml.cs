using System;
using System.Data;
using System.Data.SQLite;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Proje_filmolog.Login
{
    /// <summary>
    /// Interaction logic for SigninWin.xaml
    /// </summary>
    public partial class SignWin : Window
    {
        public SignWin() => InitializeComponent();

        private SQLiteConnection connection = new SQLiteConnection("data source = filmolog.db; Version=3;");

        private void TBOX_TextChanged(object sender, TextChangedEventArgs e) => TextController(sender);

        private void TextController(object sender)
        {
            TextBox tbox = sender as TextBox;
            var item = FindName(tbox.Name + "Hint") as TextBlock;
            switch (tbox.Text.Length)
            {
                case 0:
                    if (item.Visibility == Visibility.Collapsed)
                        item.Visibility = Visibility.Visible;
                    else
                        item.Visibility = Visibility.Collapsed;
                    break;
                default:
                    item.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox tbox = sender as PasswordBox;
            var item = FindName(tbox.Name + "Hint") as TextBlock;
            switch (tbox.Password.Length)
            {
                case 0:
                    if (item.Visibility == Visibility.Collapsed)
                    {
                        item.Visibility = Visibility.Visible;
                    }
                    else
                        item.Visibility = Visibility.Collapsed;
                    break;
                default:
                    item.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void GRİD_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

        private void TelNoTbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            LoginWin win = new LoginWin();
            win.Show();Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (!PasswordBox.Password.Trim().Equals("") && !Password2Box.Password.Trim().Equals("") &&
                !RealNameTbox.Text.Trim().Equals("") && !UserNameTbox.Text.Trim().Equals("") && !TelNoTbox.Text.Trim().Equals(""))
            {
                try
                {
                    SQLiteCommand cmd_isJoined = new SQLiteCommand("select *from Users where userName=@uName", connection);
                    if (connection.State == ConnectionState.Closed) connection.Open();

                    cmd_isJoined.Parameters.AddWithValue("@uName", UserNameTbox.Text.Trim());
                    SQLiteDataReader reader = cmd_isJoined.ExecuteReader();

                    if (!reader.Read())
                    {
                        try
                        {
                            SQLiteCommand cmd_insert = new SQLiteCommand("insert into Users (userName,password,realName,telNo,isAdmin) values (@v_userName,@v_password,@v_realName,@v_telNo,@v_isAdmin)", connection);
                            cmd_insert.Parameters.AddWithValue("@v_userName", UserNameTbox.Text.Trim());
                            cmd_insert.Parameters.AddWithValue("@v_password", Password2Box.Password);
                            cmd_insert.Parameters.AddWithValue("@v_realName", RealNameTbox.Text);
                            cmd_insert.Parameters.AddWithValue("@v_telNo", Convert.ToInt64(TelNoTbox.Text));
                            cmd_insert.Parameters.AddWithValue("@v_isAdmin", AdminKeyTbox.Text.Equals("adminkey") ? true : false);
                            cmd_insert.ExecuteNonQuery();
                            connection.Close();

                            LoginWin win = new LoginWin();
                            win.Show();
                            Close();
                        }
                        catch (Exception ex_)
                        {
                            MessageBox.Show("Kayıtta hata..Hata No:001\nHata mesajı:\n" + ex_.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Kaydı Mevcut ya da Kullanıcı Adı önceden Alınmış Olabilir.\nKullanıcı Adını Değiştirip Tekrar Deneyin.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata No:002\nHata mesajı:\n" + ex.ToString());
                }
                finally { connection.Close(); }
            }
            else
            {
                if (!PasswordBox.Password.Equals(Password2Box.Password))
                    MessageBox.Show("Şifreler Eşleşmiyor!!");
                else
                    MessageBox.Show("Lütfen Eksik Kısımları Doldurun!!");
            }
        }
    }
}
