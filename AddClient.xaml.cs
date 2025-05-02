using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace VisaManager
{
    public partial class AddClient : Window
    {
        private string passportFilePath = "";

        public AddClient()
        {
            InitializeComponent();
            LoadVisaTypes();
            LoadCountries();
        }

        private class VisaInfo
        {
            public string Requirement { get; set; }
            public int ValidDays { get; set; }
        }

        private Dictionary<string, VisaInfo> visaData = new();

        private void LoadVisaTypes()
        {
            VisaTypeComboBox.Items.Clear();
            visaData.Clear();

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name, Requirement, ExpireDate FROM Visa", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    string requirement = reader["Requirement"].ToString();
                    int validDays = Convert.ToInt32(reader["ExpireDate"]);
                    visaData[name] = new VisaInfo { Requirement = requirement, ValidDays = validDays };
                    VisaTypeComboBox.Items.Add(name);
                }
            }
        }


        private void VisaTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (VisaTypeComboBox.SelectedItem != null)
            {
                string selectedVisa = VisaTypeComboBox.SelectedItem.ToString();
                if (visaData.TryGetValue(selectedVisa, out VisaInfo info))
                {
                    VisaRequirementText.Text = info.Requirement;
                    VisaDaysText.Text = $"{info.ValidDays} days";
                }
                else
                {
                    VisaRequirementText.Text = "";
                    VisaDaysText.Text = "";
                }
            }
        }



        private void LoadCountries()
        {
            List<string> countries = new List<string>
            {
                "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", // ← shortened
                "Argentina", "Australia", "Austria", "Bangladesh", "Belgium",
                "Brazil", "Canada", "China", "Denmark", "Egypt", "France", "Germany",
                "India", "Indonesia", "Italy", "Japan", "Mexico", "Netherlands", "Nigeria",
                "Norway", "Pakistan", "Russia", "Saudi Arabia", "South Africa",
                "Spain", "Sweden", "Thailand", "Turkey", "Ukraine", "United Kingdom", "United States", "Vietnam", "Zimbabwe"
            };

            countries.Sort();
            CountryComboBox.ItemsSource = countries;
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                passportFilePath = dlg.FileName;
                PassportPathText.Text = passportFilePath;
            }
        }

        private void SaveClient_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string passportNo = PassportNoTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string visaType = VisaTypeComboBox.SelectedItem?.ToString();
            string country = CountryComboBox.SelectedItem?.ToString();
            DateTime? expireDate = ExpireDatePicker.SelectedDate;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(passportNo) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(visaType) ||
                expireDate == null || string.IsNullOrEmpty(country) || string.IsNullOrEmpty(passportFilePath))
            {
                MessageBox.Show("Please fill in all fields and upload a passport file.");
                return;
            }

            // Save file to local folder
            string folder = @"C:\VisaManager";
            Directory.CreateDirectory(folder);
            string newFileName = $"{passportNo}_{Path.GetFileName(passportFilePath)}";
            string destPath = Path.Combine(folder, newFileName);
            File.Copy(passportFilePath, destPath, true);

            // Save to database (file path only)
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("INSERT INTO Clients (Name, PassportNo, Email, VisaType, ExpireDate, CountryOrigin, PassportPath) VALUES (@name, @passport, @email, @visa, @expire, @country, @path)", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@passport", passportNo);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@visa", visaType);
                cmd.Parameters.AddWithValue("@expire", expireDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@country", country);
                cmd.Parameters.AddWithValue("@path", destPath);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Client saved successfully!");
            this.Close();
        }
    }
}
