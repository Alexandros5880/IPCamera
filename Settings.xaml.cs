﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Threading;
using ComboBox = System.Windows.Controls.ComboBox;

namespace IPCamera
{
    public partial class Settings : Window
    {
        List<Users> users;
        public List<Users> Users
        {
            get
            {
                return this.users;
            }
            set
            {
                this.users = value;
            }
        }


        public Settings()
        {

            InitializeComponent();

            this.Update_settings_page();

            // Fill The Users in The Users DataGrid
            this.FillUsers();

            // Fill the TextBoxes With the Data
            sms_account_ssid.Text = MainWindow.twilioAccountSID;
            sms_account_token.Text = MainWindow.twilioAccountToken;
            sms_account_phone.Text = MainWindow.twilioNumber;
        }

        // Progress Bar Event Method
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

        protected override void OnClosed(EventArgs e)
        {
            MainWindow.settings_oppened = false;
            Console.WriteLine("settings_oppened: " + Convert.ToString(MainWindow.settings_oppened));
            this.Close();
        }



        private void Button_pictures_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result.ToString().Equals("OK"))
                {
                    if(dialog.SelectedPath != "")
                    {
                        Camera.pictures_dir = dialog.SelectedPath;
                        txtEditor_pictures.Text = Camera.pictures_dir;
                    }
                }
            }
        }



        private void Button_videos_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result.ToString().Equals("OK"))
                {
                    if (dialog.SelectedPath != "")
                    {
                        Camera.videos_dir = dialog.SelectedPath;
                        txtEditor_videos.Text = Camera.videos_dir;
                    }
                }
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            progressBarPageOne.Visibility = Visibility.Visible;
            progressBarTracking.Visibility = Visibility.Visible;
            this.ApplyFirstPageSettings();
            progressBarPageOne.Visibility = Visibility.Hidden;
            progressBarTracking.Visibility = Visibility.Hidden;
        }

        private async void ApplyFirstPageSettings()
        {
            // ProgressBar Object
            UpdateProgressBarDelegate updateProgressBaDelegate = new UpdateProgressBarDelegate(progressBarPageOne.SetValue);
            UpdateProgressBarDelegate updateProgressBaDelegateTow = new UpdateProgressBarDelegate(progressBarTracking.SetValue);
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(20) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(20) });

            MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
            String query;
            MySqlCommand cmd;
            int result;

            // Save Paths
            if (txtEditor_pictures.Text != "" && txtEditor_videos.Text != "")
            {
                // Clear DataBase
                query = "DELETE FROM FilesDirs";
                cmd = new MySqlCommand(query, cn);
                cn.Open();
                result = await cmd.ExecuteNonQueryAsync();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Save to DataBase Pictures
                query = $"INSERT INTO FilesDirs (id, Name, Path) VALUES (@id,@name,@path)";
                cmd = new MySqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@id", 1);
                cmd.Parameters.AddWithValue("@name", "Pictures");
                cmd.Parameters.AddWithValue("@path", txtEditor_pictures.Text);
                cn.Open();
                result = await cmd.ExecuteNonQueryAsync();
                // Check Error
                if (result < 0)
                {
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                }
            }
            cn.Close();
            query = $"INSERT INTO FilesDirs (id, Name, Path) VALUES (@id,@name,@path)";
            cmd = new MySqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@id", 2);
            cmd.Parameters.AddWithValue("@name", "Videos");
            cmd.Parameters.AddWithValue("@path", txtEditor_videos.Text);
            cn.Open();
            result = await cmd.ExecuteNonQueryAsync();
            // Check Error
            if (result < 0)
            {
                System.Windows.MessageBox.Show("Error inserting data into Database!");
            }
            cn.Close();

            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(40) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(40) });
            
            // Save URLS
            List<Cameras> cams = new List<Cameras>(8);
            try
            {
                if (url_1.Text.Length > 0 && name_1.Text.Length > 0 &&
                    name_1.Text.Length > 0 && password_1.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_1.Text, name_1.Text, username_1.Text, password_1.Password, fps_1.Text, camera1_esp32.IsChecked.Value));
                }
                if (url_2.Text.Length > 0 && name_2.Text.Length > 0 &&
                    name_2.Text.Length > 0 && password_2.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_2.Text, name_2.Text, username_2.Text, password_2.Password, fps_2.Text, camera2_esp32.IsChecked.Value));
                }
                if (url_3.Text.Length > 0 && name_3.Text.Length > 0 &&
                    name_3.Text.Length > 0 && password_3.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_3.Text, name_3.Text, username_3.Text, password_3.Password, fps_3.Text, camera3_esp32.IsChecked.Value));
                }
                if (url_4.Text.Length > 0 && name_4.Text.Length > 0 &&
                    name_4.Text.Length > 0 && password_4.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_4.Text, name_4.Text, username_4.Text, password_4.Password, fps_4.Text, camera4_esp32.IsChecked.Value));
                }
                if (url_5.Text.Length > 0 && name_5.Text.Length > 0 &&
                    name_5.Text.Length > 0 && password_5.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_5.Text, name_5.Text, username_5.Text, password_5.Password, fps_5.Text, camera5_esp32.IsChecked.Value));
                }
                if (url_6.Text.Length > 0 && name_6.Text.Length > 0 &&
                    name_6.Text.Length > 0 && password_6.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_6.Text, name_6.Text, username_6.Text, password_6.Password, fps_6.Text, camera6_esp32.IsChecked.Value));
                }
                if (url_7.Text.Length > 0 && name_7.Text.Length > 0 &&
                    name_7.Text.Length > 0 && password_7.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_7.Text, name_7.Text, username_7.Text, password_7.Password, fps_7.Text, camera7_esp32.IsChecked.Value));
                }
                if (url_8.Text.Length > 0 && name_8.Text.Length > 0 &&
                    name_8.Text.Length > 0 && password_8.Password.Length > 0)
                {
                    cams.Add(new Cameras(url_8.Text, name_8.Text, username_8.Text, password_8.Password, fps_8.Text, camera8_esp32.IsChecked.Value));
                }
            }
            catch (System.ArgumentException ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nParamnAME:{ex.ParamName}\n{ex.Message}");
            }

            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(60) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(60) });
            
            int urls_num = cams.Count;
            // If urls.Count > 0
            if (urls_num > 0)
            {
                // Clear Database
                cn = new MySqlConnection(App.DB_connection_string);
                cmd = new MySqlCommand
                {
                    CommandText = "DELETE FROM MyCameras ",
                    Connection = cn
                };
                cn.Open();
                await cmd.ExecuteNonQueryAsync();
                cn.Close();
                foreach (Cameras d in cams)
                {
                    Guid guid = Guid.NewGuid();
                    String my_id = guid.ToString();
                    // Save Data To Database
                    query = $"INSERT INTO MyCameras (id,urls,name,username,password,fps,isEsp32 ) VALUES (@id,@urls,@name,@username,@password,@fps,@isESP)";
                    cmd = new MySqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", my_id);
                    cmd.Parameters.AddWithValue("@urls", d.url);
                    cmd.Parameters.AddWithValue("@name", d.name);
                    cmd.Parameters.AddWithValue("@username", d.username);
                    cmd.Parameters.AddWithValue("@password", d.password);
                    cmd.Parameters.AddWithValue("@fps", d.fps);
                    cmd.Parameters.AddWithValue("@isESP", d.isEsp32);
                    cn.Open();
                    result = await cmd.ExecuteNonQueryAsync();
                    // Check Error
                    if (result < 0)
                    {
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    cn.Close();
                }
            }
            else
            {
                // Clear Database
                cn = new MySqlConnection(App.DB_connection_string);
                cmd = new MySqlCommand
                {
                    CommandText = "DELETE FROM MyCameras ",
                    Connection = cn
                };
                cn.Open();
                await cmd.ExecuteNonQueryAsync();
                cn.Close();
            }

            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(80) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(80) });
            
            // Save Email Sender And Password
            if ((!email_send_textbox.Text.Equals(MainWindow.email_send)) ||
                    (!pass_send_textbox.Password.Equals(MainWindow.pass_send)))
            {
                Console.WriteLine(pass_send_textbox.Password);
                // If email is an valid email
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email_send_textbox.Text);
                    if (addr.Address == email_send_textbox.Text)
                    {
                        // Delete From Table The Last
                        cn = new MySqlConnection(App.DB_connection_string);
                        query = $"DELETE FROM EmailSender";
                        cmd = new MySqlCommand(query, cn);
                        cn.Open();
                        result = await cmd.ExecuteNonQueryAsync();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                        // Save Data To Database
                        query = $"INSERT INTO EmailSender (Email,Pass) VALUES (@email,@pass)";
                        cmd = new MySqlCommand(query, cn);
                        cmd.Parameters.AddWithValue("@email", email_send_textbox.Text);
                        cmd.Parameters.AddWithValue("@pass", pass_send_textbox.Password);
                        cn.Open();
                        result = await cmd.ExecuteNonQueryAsync();
                        // Check Error
                        if (result < 0)
                        {
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        }
                    }
                    else
                    {
                        if (!email_send_textbox.Text.Equals(""))
                        {
                            System.Windows.MessageBox.Show("Not Valid Email!");
                        }

                    }
                }
                catch (Exception ex)
                {
                    if (!email_send_textbox.Text.Equals(""))
                    {
                        System.Windows.MessageBox.Show("Not Valid Email!");
                    }
                    else
                    {
                        Console.WriteLine($"Source:{ex.Source}\n\n{ex.Message}");
                    }
                }
            }

            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(100) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(100) });
            
            // Save SMS sid, token, phone
            if (!sms_account_ssid.Text.Equals(MainWindow.twilioAccountSID) ||
                !sms_account_token.Text.Equals(MainWindow.twilioAccountToken) ||
                !sms_account_phone.Text.Equals(MainWindow.twilioNumber))
            {
                // Delete From Table The Last
                query = $"DELETE FROM SMS";
                cmd = new MySqlCommand(query, cn);
                cn.Open();
                result = await cmd.ExecuteNonQueryAsync();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Save Data To Database
                query = $"INSERT INTO SMS (AccountSID,AccountTOKEN,Phone) VALUES (@sid,@token,@phone)";
                cmd = new MySqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@sid", sms_account_ssid.Text);
                cmd.Parameters.AddWithValue("@token", sms_account_token.Text);
                cmd.Parameters.AddWithValue("@phone", sms_account_phone.Text);
                cn.Open();
                result = await cmd.ExecuteNonQueryAsync();
                // Check Error
                if (result < 0)
                {
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                }      
            }

            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(120) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(120) });

            // Update History Files Length
            query = $"UPDATE FilesFormats SET history_time='{MainWindow.video_recording_history_length}'";
            cmd = new MySqlCommand(query, cn);
            cn.Open();
            result = await cmd.ExecuteNonQueryAsync();
            if (result < 0)
            {
                System.Windows.MessageBox.Show("Error inserting data into Database!");
            }
            cn.Close();

            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(140) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(140) });


            // Ask to Restart The Application
            MessageBoxResult res = System.Windows.MessageBox.Show("Restart ?", "Question", (MessageBoxButton)MessageBoxButtons.OKCancel);
            if (res.ToString() == "OK")
            {
                // Close Settings Window
                this.Close();
                // Restart App Application
                MainWindow.RestartApp();
            }
        }


        private void Update_settings_page()
        {
            // Update files paths
            txtEditor_pictures.Text = Camera.pictures_dir;
            txtEditor_videos.Text = Camera.videos_dir;
            // Update Files Formats
            avi_checkbox.IsChecked = Camera.avi_format;
            mp4_checkbox.IsChecked = Camera.mp4_format;
            // Update Recording History Time
            recordingTime_ComboBox.SelectedIndex = MainWindow.video_recording_history_length-1;

            // Feel the urls
            if (Camera.count > 0)
            {
                if (MainWindow.cameras[0].url != "" && MainWindow.cameras[0].name != "")
                {
                    url_1.Text = MainWindow.cameras[0].url;
                    name_1.Text = MainWindow.cameras[0].name;
                    username_1.Text = MainWindow.cameras[0].Username;
                    password_1.Password = MainWindow.cameras[0].Password;
                    fps_1.Text = MainWindow.cameras[0].Framerate.ToString();
                    camera1_esp32.IsChecked = MainWindow.cameras[0].isEsp32;
                }
            }
            if (Camera.count > 1)
            {
                if (MainWindow.cameras[1].url != "" && MainWindow.cameras[1].name != "")
                {
                    url_2.Text = MainWindow.cameras[1].url;
                    name_2.Text = MainWindow.cameras[1].name;
                    username_2.Text = MainWindow.cameras[1].Username;
                    password_2.Password = MainWindow.cameras[1].Password;
                    fps_2.Text = MainWindow.cameras[1].Framerate.ToString();
                    camera2_esp32.IsChecked = MainWindow.cameras[1].isEsp32;
                }
            }
            if (Camera.count > 2)
            {
                if (MainWindow.cameras[2].url != "" && MainWindow.cameras[2].name != "")
                {
                    url_3.Text = MainWindow.cameras[2].url;
                    name_3.Text = MainWindow.cameras[2].name;
                    username_3.Text = MainWindow.cameras[2].Username;
                    password_3.Password = MainWindow.cameras[2].Password;
                    fps_3.Text = MainWindow.cameras[2].Framerate.ToString();
                    camera3_esp32.IsChecked = MainWindow.cameras[2].isEsp32;
                }
            }
            if (Camera.count > 3)
            {
                if (MainWindow.cameras[3].url != "" && MainWindow.cameras[3].name != "")
                {
                    url_4.Text = MainWindow.cameras[3].url;
                    name_4.Text = MainWindow.cameras[3].name;
                    username_4.Text = MainWindow.cameras[3].Username;
                    password_4.Password = MainWindow.cameras[3].Password;
                    fps_4.Text = MainWindow.cameras[3].Framerate.ToString();
                    camera4_esp32.IsChecked = MainWindow.cameras[3].isEsp32;
                }
            }
            if (Camera.count > 4)
            {
                if (MainWindow.cameras[4].url != "" && MainWindow.cameras[4].name != "")
                {
                    url_5.Text = MainWindow.cameras[4].url;
                    name_5.Text = MainWindow.cameras[4].name;
                    username_5.Text = MainWindow.cameras[4].Username;
                    password_5.Password = MainWindow.cameras[4].Password;
                    fps_5.Text = MainWindow.cameras[4].Framerate.ToString();
                    camera5_esp32.IsChecked = MainWindow.cameras[4].isEsp32;
                }
            }
            if (Camera.count > 5)
            {
                if (MainWindow.cameras[5].url != "" && MainWindow.cameras[5].name != "")
                {
                    url_6.Text = MainWindow.cameras[5].url;
                    name_6.Text = MainWindow.cameras[5].name;
                    username_6.Text = MainWindow.cameras[5].Username;
                    password_6.Password = MainWindow.cameras[5].Password;
                    fps_6.Text = MainWindow.cameras[5].Framerate.ToString();
                    camera6_esp32.IsChecked = MainWindow.cameras[5].isEsp32;
                }
            }
            if (Camera.count > 6)
            {
                if (MainWindow.cameras[6].url != "" && MainWindow.cameras[6].name != "")
                {
                    url_7.Text = MainWindow.cameras[6].url;
                    name_7.Text = MainWindow.cameras[6].name;
                    username_7.Text = MainWindow.cameras[6].Username;
                    password_7.Password = MainWindow.cameras[6].Password;
                    fps_7.Text = MainWindow.cameras[6].Framerate.ToString();
                    camera7_esp32.IsChecked = MainWindow.cameras[6].isEsp32;
                }
            }
            if (Camera.count > 7)
            {
                if (MainWindow.cameras[7].url != "" && MainWindow.cameras[7].name != "")
                {
                    url_8.Text = MainWindow.cameras[7].url;
                    name_8.Text = MainWindow.cameras[7].name;
                    username_8.Text = MainWindow.cameras[7].Username;
                    password_8.Password = MainWindow.cameras[7].Password;
                    fps_8.Text = MainWindow.cameras[7].Framerate.ToString();
                    camera8_esp32.IsChecked = MainWindow.cameras[7].isEsp32;
                }
            }
            // Update Email Sender And Pasword
            email_send_textbox.Text = MainWindow.email_send;
            pass_send_textbox.Password = MainWindow.pass_send;
            // Update Robotic . CameraSelector cameras
            camera_selector.Items.Add("Select a camera");
            camera_selector.SelectedIndex = camera_selector.Items.IndexOf("Select a camera");
            foreach (Camera cam in MainWindow.cameras)
            {
                camera_selector.Items.Add(cam.name);
            }
        }


        // Fill Users Table With Users
        public void FillUsers()
        {
            // Save list with users before
            this.Users = new List<Users>(MainWindow.myUsers);
            // Add the List To DataGrid
            users_grid.ItemsSource = this.Users;
            // Make Id Column No Editable
            users_grid.AutoGeneratingColumn += (object sender, DataGridAutoGeneratingColumnEventArgs e) =>
            {
                if (e.Column.Header.ToString() == "Id")
                {
                    e.Column.IsReadOnly = true; // Makes the column as read only
                    e.Column.Width = 33;
                }
                if (e.Column.Header.ToString() == "FirstName")
                {
                    e.Column.MinWidth = 111;
                }
                if (e.Column.Header.ToString() == "LastName")
                {
                    e.Column.MinWidth = 111;
                }
                if (e.Column.Header.ToString() == "Email")
                {
                    e.Column.MinWidth = 311;
                }
                if (e.Column.Header.ToString() == "Password")
                {
                    e.Column.MinWidth = 200;
                    e.Cancel = true;
                }
                if (e.Column.Header.ToString() == "Licences")
                {
                    e.Column.IsReadOnly = true; // Makes the column as read only
                }
            };
        }

        // Users Apply Button
        private void U_Apply_Click(object sender, RoutedEventArgs e)
        {
            progressBarPageUsers.Visibility = Visibility.Visible;
            this.UserApply();
            progressBarPageUsers.Visibility = Visibility.Hidden;
        }

        private async void UserApply()
        {
            // ProgressBar Object
            UpdateProgressBarDelegate updateProgressBaDelegate = new UpdateProgressBarDelegate(progressBarPageUsers.SetValue);
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(70) });

            // Commit Changes to the List with users
            users_grid.CommitEdit();
            // Chech If Delete a users
            if (users.Count > MainWindow.myUsers.Count)
            {
                Console.WriteLine("DELETE OK");
                foreach (Users u in users)
                {
                    if (!MainWindow.myUsers.Contains(u))
                    {
                        // Delete This User From DB
                        MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                        String query = $"DELETE FROM Users WHERE Id='{u.Id}'";
                        MySqlCommand cmd = new MySqlCommand(query, cn);
                        cn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                }
            }
            else // Update Users On DB
            {
                int counter = 0;
                foreach (Users u in MainWindow.myUsers)
                {
                    Users old_user = users[counter];
                    // If A record changeds updated
                    if ((old_user.Firstname.Equals(u.Firstname)) ||
                            (old_user.Lastname.Equals(u.Lastname)) ||
                            (old_user.Email.Equals(u.Email)) ||
                            (old_user.Phone.Equals(u.Phone)))
                    {
                        Console.WriteLine("UPDATE OK");
                        //Console.WriteLine($"ID: {u.Id}  FName: {u.Firstname}  LName: {u.Lastname}  Email: {u.Email}  Phone: {u.Phone}");
                        // Update DataBase with this user
                        MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                        String query = $"UPDATE Users SET FirstName='{u.Firstname}', " +
                                                        $"LastName='{u.Lastname}', Email='{u.Email}', " +
                                                        $"Phone='{u.Phone}' WHERE Id='{u.Id}'";
                        MySqlCommand cmd = new MySqlCommand(query, cn);
                        cn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                        counter++;
                    }
                }
            }
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(110) });
            Thread.Sleep(1000);
            // Refresch Users Table
            this.Close();
            new Settings().Show();
        }

        // Users Add Button
        private void U_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String fname = FirstName.Text;
                String lname = LastName.Text;
                String email = Email.Text;
                String phone = Phone.Text;
                String selection = new_user_licenses.SelectedValue.ToString();
                if (selection.Contains("Admin"))
                {
                    selection = "Admin";
                }
                else if (selection.Contains("Employee"))
                {
                    selection = "Employee";
                }
                String password = Password.Password;
                String repeat_pass = Repeat_Pass.Password;
                if (password.Equals(repeat_pass))
                {
                    // Insert to DB First to create an Id and then update MainWindow.myUsers
                    String query = $"INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)" +
                                                            $" VALUES (@fname, @lname, @email, @phone, @licences, @pass)";
                    using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
                    {
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@fname", fname);
                            command.Parameters.AddWithValue("@lname", lname);
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@phone", phone);
                            command.Parameters.AddWithValue("@licences", selection);
                            command.Parameters.AddWithValue("@pass", password);
                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            // Check Error
                            if (result < 0)
                                System.Windows.MessageBox.Show("Error inserting data into Database!");
                            connection.Close();
                        }
                        //connection.Close();
                        // Get The New User User From DB And Add Him To MainWindow.myUsers
                        query = $"SELECT Id, FirstName, LastName, Email, Phone, Licences, Password FROM Users " +
                                                    $"WHERE FirstName=@fname AND LastName=@lname AND Email=@email AND Phone=@phone AND Password=@pass";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@fname", fname);
                            command.Parameters.AddWithValue("@lname", lname);
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@phone", phone);
                            command.Parameters.AddWithValue("@pass", password);
                            connection.Open();
                            MySqlDataReader dataReader = command.ExecuteReader();
                            while (dataReader.Read())
                            {
                                int id = (int)dataReader["Id"];
                                String fname2 = dataReader["FirstName"].ToString().Trim();
                                String lname2 = dataReader["LastName"].ToString().Trim();
                                String email2 = dataReader["Email"].ToString().Trim();
                                String phone2 = dataReader["Phone"].ToString().Trim();
                                String licences = dataReader["Licences"].ToString().Trim();
                                String pass = dataReader["Password"].ToString().Trim();
                                // Create The Usres Objects
                                Users user = new Users(id, fname2, lname2, email2, phone2, licences, pass);
                                MainWindow.myUsers.Add(user);
                            }
                        }
                        connection.Close();
                    }
                    Console.WriteLine("ADD OK");
                    // Refresch Users Table
                    this.Close();
                    new Settings().Show();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Rong Password.");
                }
            }
            catch(MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                {
                    System.Windows.MessageBox.Show("This User Exists");
                }
            }
        }

        // Files format checkboxes
        private void AVI_chencked(object sender, EventArgs e)
        {
            Camera.avi_format = true;
            Camera.mp4_format = false;
            mp4_checkbox.IsChecked = false;
            try
            {
                // Delete Data From DB
                MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                String query = $"DELETE FROM FilesFormats";
                MySqlCommand cmd = new MySqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 1);
                        command.Parameters.AddWithValue("@mp4", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }
        private void AVI_unchencked(object sender, EventArgs e)
        {
            Camera.avi_format = false;
            try
            {
                // Delete Data From DB
                MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                String query = $"DELETE FROM FilesFormats";
                MySqlCommand cmd = new MySqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }

        private void MP4_chencked(object sender, EventArgs e)
        {
            Camera.mp4_format = true;
            Camera.avi_format = false;
            avi_checkbox.IsChecked = false;
            try
            {
                // Delete Data From DB
                MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                String query = $"DELETE FROM FilesFormats";
                MySqlCommand cmd = new MySqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 1);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }
        private void MP4_unchencked(object sender, EventArgs e)
        {
            Camera.mp4_format = false;
            try
            {
                // Delete Data From DB
                MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                String query = $"DELETE FROM FilesFormats";
                MySqlCommand cmd = new MySqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }


        // ComboBox Selected Recording Time Changed
        private void ComboBox_Selected_Recording_Time_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem typeItem = (ComboBoxItem)recordingTime_ComboBox.SelectedItem;
                if (typeItem.Content != null)
                {
                    string value = typeItem.Content.ToString();
                    switch (value)
                    {
                        case "1 Month":
                            MainWindow.video_recording_history_length = 1;
                            Console.WriteLine("1 Month");
                            break;
                        case "2 Month":
                            MainWindow.video_recording_history_length = 2;
                            Console.WriteLine("2 Month");
                            break;
                        case "3 Month":
                            MainWindow.video_recording_history_length = 3;
                            Console.WriteLine("3 Month");
                            break;
                        case "4 Month":
                            MainWindow.video_recording_history_length = 4;
                            Console.WriteLine("4 Month");
                            break;
                        case "5 Month":
                            MainWindow.video_recording_history_length = 5;
                            Console.WriteLine("5 Month");
                            break;
                        case "6 Month":
                            MainWindow.video_recording_history_length = 6;
                            Console.WriteLine("6 Month");
                            break;
                        case "7 Month":
                            MainWindow.video_recording_history_length = 7;
                            Console.WriteLine("7 Month");
                            break;
                        case "8 Month":
                            MainWindow.video_recording_history_length = 8;
                            Console.WriteLine("8 Month");
                            break;
                        case "9 Month":
                            MainWindow.video_recording_history_length = 9;
                            Console.WriteLine("9 Month");
                            break;
                        case "10 Month":
                            MainWindow.video_recording_history_length = 10;
                            Console.WriteLine("10 Month");
                            break;
                        case "11 Month":
                            MainWindow.video_recording_history_length = 11;
                            Console.WriteLine("11 Month");
                            break;
                        case "12 Month":
                            MainWindow.video_recording_history_length = 12;
                            Console.WriteLine("12 Month");
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\n\n\nException: {ex.Message}");
            }
        }

        // When select a tab
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // SMS Hyper Link Func
        private void Hyperlink_RequestNavigate(object sender,
                                       System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        // Whene Robotic Select Camera Combo Box change
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String cam_name = camera_selector.SelectedItem.ToString();
            if (!cam_name.Equals("Select a camera"))
            {
                Console.WriteLine($"Selected Camera: {cam_name}");
                foreach (Camera cam in MainWindow.cameras)
                {
                    if (cam.name.Equals(cam_name))
                    {
                        up_text.Text = cam.up_req;
                        down_text.Text = cam.down_req;
                        right_text.Text = cam.right_req;
                        left_text.Text = cam.left_req;
                    }
                }
            }
            else
            {
                Console.WriteLine("No Camera Selected");
                up_text.Text = "";
                down_text.Text = "";
                right_text.Text = "";
                left_text.Text = "";
            }
        }

        // Save the cameras remote controll settings
        private void Apply_get_req_Click(object sender, RoutedEventArgs e)
        {
            progressBarPageRobotic.Visibility = Visibility.Visible;
            this.ApplyRobotics();
            progressBarPageRobotic.Visibility = Visibility.Hidden;
        }

        private async void ApplyRobotics()
        {
            // ProgressBar Object
            UpdateProgressBarDelegate updateProgressBaDelegate = new UpdateProgressBarDelegate(progressBarPageRobotic.SetValue);
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(50) });

            // Save UP, DOWN, RIGHT, LEFT Buttons
            if (CheckURL(up_text.Text) && CheckURL(down_text.Text) &&
                    CheckURL(right_text.Text) && CheckURL(left_text.Text))
            {
                String cam_name = camera_selector.SelectedItem.ToString();
                if (!cam_name.Equals("Select a camera"))
                {
                    Console.WriteLine("Update DATABASE");
                    // Update Data To Database
                    MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                    String query = "UPDATE MyCameras SET Up_req=@up, Down_req=@down, Left_req=@left, Right_req=@right WHERE name=@cam_name";
                    MySqlCommand cmd = new MySqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@up", up_text.Text);
                    cmd.Parameters.AddWithValue("@down", down_text.Text);
                    cmd.Parameters.AddWithValue("@left", left_text.Text);
                    cmd.Parameters.AddWithValue("@right", right_text.Text);
                    cmd.Parameters.AddWithValue("@cam_name", cam_name);
                    cn.Open();
                    int result = await cmd.ExecuteNonQueryAsync();
                    cn.Close();
                    if (result < 0)
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    else
                    {
                        // Ask to Restart The Application
                        MessageBoxResult res = System.Windows.MessageBox.Show("Restart ?", "Question", (MessageBoxButton)MessageBoxButtons.OKCancel);
                        if (res.ToString() == "OK")
                        {
                            // Close Settings Window
                            this.Close();
                            // Restart App Application
                            MainWindow.RestartApp();
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Select a camera.");
                }
                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(110) });
            }
            else
            {
                System.Windows.MessageBox.Show("Setup cameras http/https urls");
            }
        }


        // Check if the texts is a valis urls
        private static bool CheckURL(String url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

    }



    public class Cameras
    {
        public String url;
        public String name;
        public String username;
        public String password;
        public int fps;
        public bool isEsp32 = false;
        public Cameras(String u, String n, String un, String p, String fps, bool isEsp)
        {
            this.url = u;
            this.name = n;
            this.username = un;
            this.password = p;
            this.fps = Int16.Parse(fps);
            this.isEsp32 = isEsp;
        }
        ~Cameras()
        {

        }
    }

}
