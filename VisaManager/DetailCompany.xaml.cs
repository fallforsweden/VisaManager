using System;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for DetailCompany.xaml
    /// </summary>
    public partial class DetailCompany : Window
    {

        private string currentCompanyName;

        public DetailCompany(string name)
        {
            InitializeComponent();
            currentCompanyName = name;
            LoadCompanyDetails(name);
        }

        private void LoadCompanyDetails(string name)
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Company WHERE Name = @name", conn);
                cmd.Parameters.AddWithValue("@name", name);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Show all the details - assign to labels or TextBlocks
                    NameLabel.Text = reader["Name"].ToString();
                    ContactLabel.Text = reader["Contact"].ToString();
                    EmailLabel.Text = reader["Email"].ToString();
                    // You can also handle paths to display documents/images
                }
                    AktaLink.Tag = reader["Akta"].ToString();
                    SKLink.Tag = reader["SK"].ToString();
                    NIBLink.Tag = reader["NIB"].ToString();
                    NPWPLink.Tag = reader["NPWP"].ToString();
                    Doc1Link.Tag = reader["Doc1"].ToString();
                    Doc2Link.Tag = reader["Doc2"].ToString();
                    Doc3Link.Tag = reader["Doc3"].ToString();
                    Doc4Link.Tag = reader["Doc4"].ToString();
                    Doc5Link.Tag = reader["Doc5"].ToString();
                    reader.Close();
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

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("UPDATE Company SET Name = @name, Contact = @contact, Email = @email WHERE Name = @originalName", conn);
                cmd.Parameters.AddWithValue("@name", NameLabel.Text);
                cmd.Parameters.AddWithValue("@contact", ContactLabel.Text);
                cmd.Parameters.AddWithValue("@email", EmailLabel.Text);
                cmd.Parameters.AddWithValue("@originalName", currentCompanyName);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Company updated!");
            currentCompanyName = NameLabel.Text; // In case name was changed
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this company?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand("DELETE FROM Company WHERE Name = @name", conn);
                    cmd.Parameters.AddWithValue("@name", currentCompanyName);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Company deleted.");
                this.Close(); // Peace out ✌️
            }
        }
    }
    }
