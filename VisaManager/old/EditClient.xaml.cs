using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisaManager
{
    public partial class EditClient : Window
    {
        private string originalClientName;
        private string selectedPassportPath = "";
        public string UpdatedClientName { get; private set; }

        public EditClient(string clientName)
        {
            InitializeComponent();
            originalClientName = clientName;

            LoadVisaTypes();
            LoadCountryList();
            LoadClientData();
            LoadCompanies();
        }

        private void LoadVisaTypes()
        {
            VisaComboBox.Items.Clear();
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name FROM Visa", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    VisaComboBox.Items.Add(reader["Name"].ToString());
                }
                reader.Close();
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

        private void LoadCountryList()
        {
            var countryList = System.Globalization.CultureInfo
                .GetCultures(System.Globalization.CultureTypes.SpecificCultures);
            HashSet<string> countries = new();
            foreach (var culture in countryList)
            {
                var region = new System.Globalization.RegionInfo(culture.LCID);
                countries.Add(region.EnglishName);
            }
            CountryComboBox.ItemsSource = new List<string>(countries);
        }

        private void LoadClientData()
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Clients WHERE Name = @name", conn);
                cmd.Parameters.AddWithValue("@name", originalClientName);

                using (var reader = cmd.ExecuteReader()) // FIXED
                {
                    if (reader.Read())
                    {
                        NameTextBox.Text = reader["Name"].ToString();
                        PassportNumberTextBox.Text = reader["PassportNo"].ToString();
                        EmailTextBox.Text = reader["Email"].ToString();
                        VisaComboBox.SelectedItem = reader["VisaType"].ToString();
                        ExpireDatePicker.SelectedDate = DateTime.Parse(reader["ExpireDate"].ToString());
                        CountryComboBox.SelectedItem = reader["CountryOrigin"].ToString();

                        selectedPassportPath = reader["PassportPath"].ToString();
                        if (File.Exists(selectedPassportPath))
                        {
                            if (selectedPassportPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                // Show a PDF icon instead
                                PassportImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/pdf_icon.png"));
                            }
                            else
                            {
                                try
                                {
                                    PassportImage.Source = new BitmapImage(new Uri(selectedPassportPath));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error loading image: " + ex.Message);
                                    PassportImage.Source = null;
                                }
                            }
                        }

                        UpdateVisaDetails(reader["VisaType"].ToString());
                    }
                    reader.Close();
                }
            }
        }

        private void UpdateVisaDetails(string visaName)
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Requirement, ExpireDate FROM Visa WHERE Name = @name", conn);
                cmd.Parameters.AddWithValue("@name", visaName);

                using (var reader = cmd.ExecuteReader()) // FIXED
                {
                    if (reader.Read())
                    {
                        VisaRequirementText.Text = reader["Requirement"].ToString();
                        VisaValidDaysText.Text = reader["ExpireDate"].ToString() + " days";
                    }
                    reader.Close();
                }
            }
        }

        private void VisaComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (VisaComboBox.SelectedItem != null)
            {
                UpdateVisaDetails(VisaComboBox.SelectedItem.ToString());
            }
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image or PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                selectedPassportPath = dlg.FileName;
                if (File.Exists(selectedPassportPath) && !selectedPassportPath.EndsWith(".pdf"))
                {
                    PassportImage.Source = new BitmapImage(new Uri(selectedPassportPath));
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("UPDATE Clients SET Name = @name, PassportNo = @passport, Email = @email, VisaType = @visa, ExpireDate = @expire, CountryOrigin = @country, PassportPath = @path WHERE Name = @original", conn);
                cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                cmd.Parameters.AddWithValue("@passport", PassportNumberTextBox.Text);
                cmd.Parameters.AddWithValue("@email", EmailTextBox.Text);
                cmd.Parameters.AddWithValue("@visa", VisaComboBox.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@expire", ExpireDatePicker.SelectedDate?.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@country", CountryComboBox.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@path", selectedPassportPath);
                cmd.Parameters.AddWithValue("@original", originalClientName);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Client updated successfully!");
            UpdatedClientName = NameTextBox.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
