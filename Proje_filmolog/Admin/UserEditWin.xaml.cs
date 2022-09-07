using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Proje_filmolog.Admin
{
    /// <summary>
    ///     Interaction logic for UserEditWin.xaml
    /// </summary>
    public partial class UserEditWin : Window
    {
        private static bool activite;

        private readonly FilmComponents filmAddComponent = new FilmComponents();

        public UserEditWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            filmAddComponent.AddUsersListViev_inDB(userListView);
        }

        private void UserListView_doubleClik(object sender, MouseButtonEventArgs e)
        {
            remark_ListView.Items.Clear();
            activite = filmAddComponent.showUser(userListView, lbl_realName, lbl_UserName, lbl_TelNo);
            filmAddComponent.showRemark(remark_ListView, lbl_UserName.Content.ToString());
            if (activite)
                Banner.Content = "BANLA";
            else
                Banner.Content = "BANI KALDIR";
        }

        private void UserSearchBox_textChange(object sender, TextChangedEventArgs e)
        {
            userListView.Items.Clear();
            if (string.IsNullOrEmpty(UserSearchBox.Text.Trim()))
                filmAddComponent.AddUsersListViev_inDB(userListView);
            else
                filmAddComponent.UserFinder(userListView, UserSearchBox.Text.Trim());
        }

        private void Banner_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new SQLiteConnection("data source = filmolog.db; Version=3;"))
            {
                try
                {
                    connection.Open();
                    using (var cmd_update = new SQLiteCommand("update Users set isActive=@actv where userName=@uname",
                               connection))
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
                finally
                {
                    connection.Close();
                }
            }

            activite = filmAddComponent.showUser(userListView, lbl_realName, lbl_UserName, lbl_TelNo);
            if (activite)
                Banner.Content = "BANLA";
            else
                Banner.Content = "BANI KALDIR";
        }

        private void Films_Click(object sender, RoutedEventArgs e)
        {
            var win = new FilmEditWin();
            win.Show();
            Close();
        }
    }
}