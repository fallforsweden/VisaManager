using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisaManager
{
    public partial class DetailClient : Window
    {
        private string clientName;


        public DetailClient(string name)
        {
            InitializeComponent();
            clientName = name;
            LoadClientDetails();
        }

        private void LoadClientDetails()
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Clients WHERE Name = @name", conn);
                cmd.Parameters.AddWithValue("@name", clientName);
                using (var reader = cmd.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        NameText.Text = reader["Name"].ToString();
                        EmailText.Text = reader["Email"].ToString();
                        VisaTypeText.Text = reader["VisaType"].ToString();
                        ExpireDateText.Text = reader["ExpireDate"].ToString();
                        PassportNumberText.Text = reader["PassportNo"].ToString();
                        CountryText.Text = reader["CountryOrigin"].ToString();

                        string passportPath = reader["PassportPath"].ToString();
                        if (File.Exists(passportPath))
                        {
                            if (passportPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                // Load a default PDF icon thumbnail
                                PassportImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/pdf_icon.png"));
                            }
                            else
                            {
                                PassportImage.Source = new BitmapImage(new Uri(passportPath));
                            }
                        }
                    }
                    reader.Close();
                }
                }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditClient editWindow = new EditClient(clientName);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                // In case the name has changed, reload the new name
                clientName = editWindow.UpdatedClientName;
                LoadClientDetails();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this client?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var conn = new SQLiteConnection("Data Source=mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand("DELETE FROM Clients WHERE Name = @name", conn);
                    cmd.Parameters.AddWithValue("@name", clientName);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Client deleted successfully!");
                this.Close(); // Close the detail window
            }
        }

        private void PassportImage_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PassportImage.Source is BitmapImage bmp && bmp.UriSource != null)
            {
                string filePath = bmp.UriSource.LocalPath;

                if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true // 👈 opens with default app
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to open PDF: " + ex.Message);
                    }
                }
                else
                {
                    ImageViewer viewer = new ImageViewer(filePath);
                    viewer.Owner = this;
                    viewer.ShowDialog();
                }
            }
        }




    }
}
