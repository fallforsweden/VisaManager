using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml;

namespace VisaManager
{
    public partial class DetailClient : Window
    {
        private string clientName;
        public bool ClientWasModified { get; private set; } = false;

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
                        PassportNumberText.Text = reader["PassportNo"].ToString();
                        EmailText.Text = reader["Email"].ToString();
                        VisaTypeText.Text = reader["VisaType"].ToString();
                        ExpireDateText.Text = reader["ExpireDate"].ToString();
                        CountryText.Text = reader["CountryOrigin"].ToString();
                        CompanyText.Text = reader["Company"].ToString();
                        PassportLink.Tag = reader["Passport"].ToString();
                        PasPhotoLink.Tag = reader["PasPhoto"].ToString();
                        RekeningLink.Tag = reader["Rekening"].ToString();
                        KTPLink.Tag = reader["KTP"].ToString();
                        PermohonanLink.Tag = reader["Permohonan"].ToString();
                        NPWPLink.Tag = reader["NPWP"].ToString();
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
                clientName = editWindow.UpdatedClientName;
                LoadClientDetails();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this client?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand("DELETE FROM Clients WHERE Name = @name", conn);
                    cmd.Parameters.AddWithValue("@name", clientName);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Client deleted successfully!");
                ClientWasModified = true;
                this.Close(); // Close the detail window
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link && link.Tag is string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true // open with default app
                        });
                    }
                    else
                    {
                        MessageBox.Show("File not found: " + filePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open file: " + ex.Message);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




    }
}
