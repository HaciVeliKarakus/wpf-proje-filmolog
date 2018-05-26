using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Proje_filmolog.Admin
{
    /// <summary>
    /// Interaction logic for UserEditWin.xaml
    /// </summary>
    public partial class UserEditWin : Window
    {
        public UserEditWin() => InitializeComponent();

        FilmComponents filmAddComponent = new FilmComponents();
        static private bool activite;

        private void Window_Loaded(object sender, RoutedEventArgs e) => filmAddComponent.AddUsersListViev_inDB(userListView);

        private void UserListView_doubleClik(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            remark_ListView.Items.Clear();
            activite = filmAddComponent.showUser(userListView, lbl_realName, lbl_UserName, lbl_TelNo);
            filmAddComponent.showRemark(remark_ListView, lbl_UserName.Content.ToString());
            if (activite)
                Banner.Content = "BANLA";
            else
                Banner.Content = "BANI KALDIR";
        }

        private void UserSearchBox_textChange(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            userListView.Items.Clear();
            if (string.IsNullOrEmpty(UserSearchBox.Text.Trim()))
                filmAddComponent.AddUsersListViev_inDB(userListView);
            else
                filmAddComponent.UserFinder(userListView, UserSearchBox.Text.Trim());
        }

        private void Banner_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection("data source = filmolog.db; Version=3;"))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand cmd_update = new SQLiteCommand("update Users set isActive=@actv where userName=@uname", connection))
                    {
                        try
                        {
                            cmd_update.Parameters.AddWithValue("@uname", lbl_UserName.Content.ToString());
                            if (activite)
                            {
                                cmd_update.Parameters.AddWithValue("@actv", false);
                                activite = false;
                            }
                            else
                            {
                                cmd_update.Parameters.AddWithValue("@actv", true);
                                activite = true;
                            }
                            cmd_update.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata No:16\n" + ex);
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Hata No:17\n" + ex.Message);
                }
                finally { connection.Close(); }
            }
            activite = filmAddComponent.showUser(userListView, lbl_realName, lbl_UserName, lbl_TelNo);
            if (activite)
                Banner.Content = "BANLA";
            else
                Banner.Content = "BANI KALDIR";
        }

        private void Films_Click(object sender, RoutedEventArgs e)
        {
            FilmEditWin win = new FilmEditWin();
            win.Show();
            Close();
        }
    }
}
