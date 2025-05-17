using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using static VisaManager.PreviewCompany;

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
            LoadCompanies();
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

        private void LoadCompanies()
        {
            CompanyComboBox.Items.Clear();

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name FROM Company", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string companyName = reader["Name"].ToString();
                    CompanyComboBox.Items.Add(companyName);
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
                "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua and Barbuda", "Argentina", "Armenia", "Australia", "Austria",
                "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bhutan",
                "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cabo Verde", "Cambodia",
                "Cameroon", "Canada", "Central African Republic", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo (Congo-Brazzaville)", "Costa Rica",
                "Croatia", "Cuba", "Cyprus", "Czechia", "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador",
                "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini", "Ethiopia", "Fiji", "Finland", "France",
                "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guinea-Bissau",
                "Guyana", "Haiti", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland",
                "Israel", "Italy", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan",
                "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Madagascar",
                "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico", "Micronesia",
                "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal",
                "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "North Macedonia", "Norway", "Oman", "Pakistan",
                "Palau", "Palestine State", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Qatar",
                "Romania", "Russia", "Rwanda", "Saint Kitts and Nevis", "Saint Lucia", "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia",
                "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa",
                "South Korea", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Taiwan",
                "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan",
                "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States of America", "Uruguay", "Uzbekistan", "Vanuatu", "Vatican City",
                "Venezuela", "Vietnam", "Yemen", "Zambia", "Zimbabwe"
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
            string company = CompanyComboBox.SelectedItem?.ToString();
            DateTime? expireDate = ExpireDatePicker.SelectedDate;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(passportNo) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(visaType) ||
                expireDate == null || string.IsNullOrEmpty(country) || string.IsNullOrEmpty(passportFilePath))
            {
                MessageBox.Show("Please fill in all fields and upload a passport file.");
                return;
            }

            string folder = @"C:\VisaManager";
            Directory.CreateDirectory(folder);
            string newFileName = $"{passportNo}_{Path.GetFileName(passportFilePath)}";
            string destPath = Path.Combine(folder, newFileName);
            File.Copy(passportFilePath, destPath, true);

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("INSERT INTO Clients (Name, PassportNo, Email, VisaType, ExpireDate, CountryOrigin, Company, PassportFile) VALUES (@name, @passport, @email, @visa, @expire, @country, @company, @path)", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@passport", passportNo);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@visa", visaType);
                cmd.Parameters.AddWithValue("@expire", expireDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@country", country);
                cmd.Parameters.AddWithValue("@company", company);
                cmd.Parameters.AddWithValue("@path", destPath);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Client saved successfully!");
            this.Close();
        }
    }
}
