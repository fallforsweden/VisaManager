using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for DetailClient.xaml
    /// </summary>
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
                var reader = cmd.ExecuteReader();

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
                        PassportImage.Source = new BitmapImage(new Uri(passportPath));
                    }
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

    }
}
