﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using HtmlAgilityPack;

namespace Proje_filmolog.Admin
{
    internal class FilmComponents
    {
        private const string MainURL = "https://www.sinemalar.com/en-iyi-filmler";
        private const string MainPATH = "//*[@id='container']/div[2]/div[3]/div[2]/article[";
        private const string NAME = "]/div/div[4]/h3/a";
        private const string DIRECTOR = "]/div/div[4]/p[1]/a";
        private const string STARRING = "]/div/div[4]/p[2]";
        private const string RATE = "]/div/div[2]/div";
        private const string INFO = "]/div/div[4]/p[3]/text()";
        private const string RANK = "]/div/div[4]/div";

        private const int COLUMN_COUNTER = 7;
        public static List<PropertyFilms> ListOfFilm = new List<PropertyFilms>();
        public static int Page;

        private static readonly SQLiteConnection connection =
            new SQLiteConnection("data source = filmolog.db; Version=3;");

        private readonly string connectStrg = "data source = filmolog.db; Version=3;";

        private SQLiteCommand cmd_insert =
            new SQLiteCommand(
                "insert into film(name,dir,staring,rate,info,rank) values(@name,@dir,@staring,@rate,@info,@rank)",
                connection);

        private SQLiteCommand cmd_select = new SQLiteCommand("select name from film", connection);
        private HtmlDocument doc;

        private int filmCounter;
        private string html;
        private short Idx;

        private string[] listItems;

        private PropertyFilms propertyFilms;

        private void GetValueWithPath(string xpath)
        {
            try
            {
                if (Idx == 2)
                    try
                    {
                        var pureStr = doc.DocumentNode.SelectSingleNode(xpath).InnerText.Split(',');
                        listItems[Idx] = pureStr[1].TrimStart();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        listItems[Idx] = " - ";
                    }
                else
                    listItems[Idx] = doc.DocumentNode.SelectSingleNode(xpath).InnerText;

                Idx++;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("NullReferenceException");
            }
        }

        /// <summary>
        ///     Siteden film bilgileri çekilir.
        /// </summary>
        /// <param name="lv">Doldurulacak listview</param>
        public void GetFilmsFromWeb(ListView lv)
        {
            if (Page > -1)
            {
                var client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                try
                {
                    html = client.DownloadString(MainURL + Page);
                }
                catch (WebException ex)
                {
                    MessageBox.Show("İnternet bağlantısını kontrol ediniz !" + ex.Message);
                    return;
                }

                doc = new HtmlDocument();
                doc.LoadHtml(html);
                try
                {
                    for (var i = 1; i < 26; i++)
                    {
                        listItems = new string[COLUMN_COUNTER];
                        Idx = 0;

                        GetValueWithPath(MainPATH + i + NAME);
                        GetValueWithPath(MainPATH + i + DIRECTOR);
                        GetValueWithPath(MainPATH + i + STARRING);
                        GetValueWithPath(MainPATH + i + RATE);
                        GetValueWithPath(MainPATH + i + INFO);
                        GetValueWithPath(MainPATH + i + RANK);


                        lv.Items.Add(new { rank = listItems[5], name = listItems[0], rate = listItems[3] });
                        if (!listItems[0].Equals(null)) propertyFilms._name = listItems[0];
                        else continue;
                        if (!listItems[1].Equals(null)) propertyFilms._director = listItems[1];
                        else propertyFilms._director = "-";
                        if (!listItems[2].Equals(null)) propertyFilms._starring = listItems[2];
                        else propertyFilms._starring = "-";
                        if (!listItems[3].Equals(null)) propertyFilms._rate = listItems[3];
                        else propertyFilms._rate = "-";
                        if (!listItems[4].Equals(null)) propertyFilms._info = listItems[4];
                        else propertyFilms._info = "-";
                        if (!listItems[5].Equals(null)) propertyFilms._rank = listItems[5];
                        else propertyFilms._rank = "-";
                        ListOfFilm.Add(propertyFilms);
                    }
                }
                catch (UriFormatException)
                {
                    MessageBox.Show("Hata No:03\n" + "UriFormatException");
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Hata No:04\n" + "ArgumentNullException");
                }
            }
        }

        public void AddToDB(List<PropertyFilms> listOfFilm, IList items)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectStrg))
                {
                    connection.Open();
                    for (var i = 0; i < items.Count; i++)
                        using (var cmd = new SQLiteCommand(
                                   "insert into film(name,dir,staring,rate,info,rank) values(@name,@dir,@staring,@rate,@info,@rank)",
                                   connection))
                        {
                            using (var cmd_varmi = new SQLiteCommand("select *from film where name=@name", connection))
                            {
                                cmd_varmi.Parameters.AddWithValue("@name", ListOfFilm[i]._name);
                                using (var reader = cmd_varmi.ExecuteReader())
                                {
                                    if (!reader.Read())
                                        try
                                        {
                                            cmd.Parameters.AddWithValue("@name", listOfFilm[i]._name);
                                            cmd.Parameters.AddWithValue("@dir", listOfFilm[i]._director);
                                            cmd.Parameters.AddWithValue("@staring", listOfFilm[i]._starring);
                                            cmd.Parameters.AddWithValue("@rate", listOfFilm[i]._rate);
                                            cmd.Parameters.AddWithValue("@info", listOfFilm[i]._info);
                                            cmd.Parameters.AddWithValue("@rank", listOfFilm[i]._rank);
                                            var ex = cmd.ExecuteNonQuery();
                                            if (ex > 0)
                                                MessageBox.Show("kayıt tamam..");
                                            else
                                                MessageBox.Show("kayıt başarısız..");
                                        }
                                        catch (SQLiteException)
                                        {
                                            MessageBox.Show("Hata No:07\n" +
                                                            "Filmi kaydedemedik Kusura Bakmayın!!\nTekrar Deneyebilirsiniz İsterseniz");
                                        }
                                    else
                                        MessageBox.Show("Fİlm Zaten Mevcut", "Dikkat", MessageBoxButton.OK,
                                            MessageBoxImage.Warning);
                                }
                            }
                        }
                    //MessageBox.Show("Kayıtlar başarılı");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:08\n" + ex);
            }
        }

        public void AddfilmsListView(ListView FilmListView, int i)
        {
            using (var connection = new SQLiteConnection(connectStrg))
            {
                using (var cmd = new SQLiteCommand("select rank,name from film", connection))
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open) connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                FilmListView.Items.Add(new { rank = reader["rank"], name = reader["name"] });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata No:06\n" + ex.Message);
                    }
                }
            }
        }

        public void DeleteFilmFromDB(ListView lv)
        {
            try
            {
                using (var con = new SQLiteConnection(connectStrg))
                {
                    for (var item = 0; item < lv.SelectedItems.Count; item++)
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        dynamic selectedItem = lv.SelectedItems[item];
                        using (var cmd = new SQLiteCommand("delete from film where rank=@_rank", con))
                        {
                            cmd.Parameters.AddWithValue("@_rank", selectedItem.rank);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Filmler Listeden Kaldırıldı..");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No: 09\n" + ex);
            }
        }

        public void AddFilmToDB(List<PropertyFilms> listOfFilm, int i)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectStrg))
                {
                    connection.Open();
                    using (var cmd_insert =
                           new SQLiteCommand(
                               "insert into film(name,dir,staring,rate,info,rank) values(@name,@dir,@staring,@rate,@info,@rank)",
                               connection))
                    {
                        using (var cmd_varmi = new SQLiteCommand("select *from film where name=@name", connection))
                        {
                            cmd_varmi.Parameters.AddWithValue("@name", listOfFilm[i]._name);
                            using (var reader = cmd_varmi.ExecuteReader())
                            {
                                if (!reader.Read())
                                    try
                                    {
                                        cmd_insert.Parameters.AddWithValue("@name", listOfFilm[i]._name);
                                        cmd_insert.Parameters.AddWithValue("@dir", listOfFilm[i]._director);
                                        cmd_insert.Parameters.AddWithValue("@staring", listOfFilm[i]._starring);
                                        cmd_insert.Parameters.AddWithValue("@rate", listOfFilm[i]._rate);
                                        cmd_insert.Parameters.AddWithValue("@info", listOfFilm[i]._info);
                                        cmd_insert.Parameters.AddWithValue("@rank", listOfFilm[i]._rank);
                                        var ex = cmd_insert.ExecuteNonQuery();
                                        if (ex > 0)
                                            MessageBox.Show("kayıt tamam..");
                                        else
                                            MessageBox.Show("kayıt başarısız..");
                                    }
                                    catch (SQLiteException)
                                    {
                                        MessageBox.Show("Hata No:07\n" +
                                                        "Filmi kaydedemedik Kusura Bakmayın!!\nTekrar Deneyebilirsiniz İsterseniz");
                                    }
                                else
                                    MessageBox.Show("Fİlm Zaten Mevcut", "Dikkat", MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:08\n" + ex);
            }
        }

        public bool FindFilmFromDB(string film_find, ListView lv)
        {
            using (var connection = new SQLiteConnection(connectStrg))
            {
                try
                {
                    if (connection.State == ConnectionState.Closed) connection.Open();
                    using (var cmd_findFilm =
                           new SQLiteCommand("select *from film where name like '%" + film_find + "%' ", connection))
                    {
                        filmCounter = 0;
                        var reader = cmd_findFilm.ExecuteReader();
                        while (reader.Read())
                        {
                            lv.Items.Add(new { rank = reader["rank"], name = reader["name"] });
                            filmCounter++;
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata No:10\n" + ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return filmCounter == 0 ? false : true;
        }

        public void AddUsersListViev_inDB(ListView listView)
        {
            using (var connection = new SQLiteConnection(connectStrg))
            {
                try
                {
                    connection.Open();
                    using (var cmd_selectuser =
                           new SQLiteCommand("select *from Users  userName where isAdmin=0", connection))
                    {
                        try
                        {
                            using (var reader = cmd_selectuser.ExecuteReader())
                            {
                                while (reader.Read()) listView.Items.Add(new { uname = reader["userName"] });
                                reader.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata No:11\n" + ex.Message);
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Hata No:12\n" + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        public void UserFinder(ListView listView, string findUser)
        {
            using (var connection = new SQLiteConnection(connectStrg))
            {
                using (var cmd_selectuser =
                       new SQLiteCommand("select *from Users where userName  like '%" + findUser + "%' and isAdmin=0",
                           connection))
                {
                    try
                    {
                        connection.Open();
                        using (var reader = cmd_selectuser.ExecuteReader())
                        {
                            while (reader.Read()) listView.Items.Add(new { uname = reader["userName"] });
                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata No:13\n" + ex);
                    }
                }
            }
        }

        public void showRemark(ListView listView, string uName)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectStrg))
                {
                    connection.Open();
                    using (var cmd_selectRemark =
                           new SQLiteCommand("select *from comment where userName=@uName", connection))
                    {
                        cmd_selectRemark.Parameters.AddWithValue("@uName", uName);
                        using (var reader = cmd_selectRemark.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ig = "-";
                                if (Convert.ToBoolean(reader["isGood"]))
                                    ig = "+";
                                listView.Items.Add(new
                                    { usersFilm = reader["filName"], remark = reader["remark"], isgood = ig });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:15\n" + ex);
            }
        }

        public bool showUser(ListView userListView, Label name, Label username, Label telno)
        {
            var act = true;
            using (var connection = new SQLiteConnection(connectStrg))
            {
                using (var cmd_selecteduser = new SQLiteCommand("select *from Users where userName=@uName", connection))
                {
                    try
                    {
                        if (userListView.SelectedIndex > -1)
                        {
                            dynamic selectedind = userListView.SelectedItems[0];
                            string x = selectedind.uname;
                            cmd_selecteduser.Parameters.AddWithValue("@uName", x);
                            connection.Open();
                            using (var reader = cmd_selecteduser.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    name.Content = reader["realName"].ToString();
                                    username.Content = reader["userName"].ToString();
                                    telno.Content = reader["telNo"].ToString();
                                    act = Convert.ToBoolean(reader["isActive"]);
                                }

                                reader.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("bos bastın");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata No:14\n" + ex);
                    }

                    connection.Close();
                }
            }

            return act;
        }

        public struct PropertyFilms
        {
            public string _name;
            public string _director;
            public string _starring;
            public string _rate;
            public string _info;
            public string _rank;
        }
    }
}