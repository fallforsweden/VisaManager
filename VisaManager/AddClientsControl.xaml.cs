using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

using static VisaManager.PreviewCompany;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for AddClientsControl.xaml
    /// </summary>
    public partial class AddClientsControl : UserControl
    {
        public AddClientsControl()
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

        private void upload_passport(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                PassportPathText.Text = dlg.FileName;
            }
        }

        private void upload_pasphoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                PasPhotoPathText.Text = dlg.FileName;
            }
        }

        private void upload_rekening(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                RekeningPathText.Text = dlg.FileName;
            }
        }

        private void upload_ktp(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                KTPPathText.Text = dlg.FileName;
            }
        }

        private void upload_permohonan(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                PermohonanPathText.Text = dlg.FileName;
            }
        }

        private void upload_npwp(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                NPWPPathText.Text = dlg.FileName;
            }
        }

        private string CopyFile(string sourcePath, string label)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath)) return "";
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "client_doc");
            Directory.CreateDirectory(folder);
            string destFileName = $"{label}_{Path.GetFileName(sourcePath)}";
            string destPath = Path.Combine(folder, destFileName);
            File.Copy(sourcePath, destPath, true);
            return destPath;
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
                expireDate == null || string.IsNullOrEmpty(country))
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("INSERT INTO Clients (Name, PassportNo, Email, VisaType, ExpireDate, CountryOrigin, Company, Passport, PasPhoto, Rekening, KTP, Permohonan, NPWP) VALUES (@name, @noPassport, @email, @visa, @expire, @country, @company, @passport, @pasPhoto, @rekening, @ktp, @permohonan, @npwp)", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@noPassport", passportNo);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@visa", visaType);
                cmd.Parameters.AddWithValue("@expire", expireDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@country", country);
                cmd.Parameters.AddWithValue("@company", company);
                cmd.Parameters.AddWithValue("@passport", CopyFile(PassportPathText.Text, "Passport"));
                cmd.Parameters.AddWithValue("@pasPhoto", CopyFile(PasPhotoPathText.Text, "PasPhoto"));
                cmd.Parameters.AddWithValue("@rekening", CopyFile(RekeningPathText.Text, "Rekening"));
                cmd.Parameters.AddWithValue("@ktp", CopyFile(KTPPathText.Text, "KTP"));
                cmd.Parameters.AddWithValue("@permohonan", CopyFile(PermohonanPathText.Text, "Permohonan"));
                cmd.Parameters.AddWithValue("@npwp", CopyFile(NPWPPathText.Text, "NPWP"));
                cmd.ExecuteNonQuery();
            }

            

            // Get the parent window (MainWindow)
            var mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Navigate back to PreviewCompanyControl
                mainWindow.ShowContent(new PreviewClientsControl());

                // Optional: Show confirmation snackbar
                MessageBox.Show("Client saved successfully!");
            }
        }
    }
}
