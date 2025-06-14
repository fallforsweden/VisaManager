using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace VisaManager
{
    public partial class EditClientControl : UserControl
    {
        private string clientName;
        public string UpdatedClientName { get; private set; }
        // Original paths from DB
        private string originalPassportPath;
        private string originalPasPhotoPath;
        private string originalRekeningPath;
        private string originalNpwpPath;
        private string originalKtpPath;
        private string originalPermohonanPath;

        // Temporary (staged) new paths
        private string stagedPassportPath;
        private string stagedPasPhotoPath;
        private string stagedRekeningPath;
        private string stagedNpwpPath;
        private string stagedKtpPath;
        private string stagedPermohonanPath;


        public EditClientControl(string name)
        {
            InitializeComponent();
            clientName = name;
            LoadClientData();
            LoadCountries();
            LoadCompanies();
            LoadVisaTypes();
        }

        private void LoadClientData()
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
                        NameTextBox.Text = reader["Name"].ToString();
                        PassportNumberTextBox.Text = reader["PassportNo"].ToString();
                        EmailTextBox.Text = reader["Email"].ToString();
                        VisaTypeComboBox.SelectedItem = reader["VisaType"].ToString();

                        if (DateTime.TryParse(reader["ExpireDate"].ToString(), out var expireDate))
                        {
                            ExpireDatePicker.SelectedDate = expireDate;
                        }

                        CountryComboBox.SelectedItem = reader["CountryOrigin"].ToString();
                        CompanyComboBox.SelectedItem = reader["Company"].ToString();
                        originalPassportPath = reader["Passport"].ToString();
                        originalPasPhotoPath = reader["PasPhoto"].ToString();
                        originalRekeningPath = reader["Rekening"].ToString();
                        originalNpwpPath = reader["NPWP"].ToString();
                        originalKtpPath = reader["KTP"].ToString();
                        originalPermohonanPath = reader["Permohonan"].ToString();
                        SetViewButtonState(PassportButton, stagedPassportPath);
                        SetViewButtonState(PasPhotoButton, stagedPasPhotoPath);
                        SetViewButtonState(RekeningButton, stagedRekeningPath);
                        SetViewButtonState(NPWPButton, stagedNpwpPath);
                        SetViewButtonState(KTPButton, stagedKtpPath);
                        SetViewButtonState(PermohonanButton, stagedPermohonanPath);

                    }
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
                    CompanyComboBox.Items.Add(reader["Name"].ToString());
                }
            }
        }

        private void LoadVisaTypes()
        {
            VisaTypeComboBox.Items.Clear();
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name FROM Visa", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    VisaTypeComboBox.Items.Add(reader["Name"].ToString());
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportNumberTextBox.Text) ||
                VisaTypeComboBox.SelectedItem == null ||
                ExpireDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please fill in all required fields");
                return;
            }
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisaManager");
            Directory.CreateDirectory(folder);

            string SaveDocument(string original, string staged, string label)
            {
                if (string.IsNullOrEmpty(staged)) return original; // staged is empty, fallback to original
                if (original == staged) return original; // nothing changed

                try
                {
                    string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisaManager");
                    Directory.CreateDirectory(folder);

                    string destFileName = $"{label}_{Path.GetFileName(staged)}";
                    string destPath = Path.Combine(folder, destFileName);
                    File.Copy(staged, destPath, true);
                    return destPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to copy file: {ex.Message}");
                    return original;
                }
            }


            string finalPassportPath = SaveDocument(originalPassportPath, stagedPassportPath, "Passport");
            string finalPasPhotoPath = SaveDocument(originalPasPhotoPath, stagedPasPhotoPath, "PasPhoto");
            string finalRekeningPath = SaveDocument(originalRekeningPath, stagedRekeningPath, "Rekening");
            string finalNpwpPath = SaveDocument(originalNpwpPath, stagedNpwpPath, "NPWP");
            string finalKtpPath = SaveDocument(originalKtpPath, stagedKtpPath, "KTP");
            string finalPermohonanPath = SaveDocument(originalPermohonanPath, stagedPermohonanPath, "Permohonan");

            // Update database
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
            "UPDATE Clients SET Name = @name, PassportNo = @passport, Email = @email, " +
            "VisaType = @visa, ExpireDate = @expire, CountryOrigin = @country, Company = @company, " +
            "Passport = @passportPath, PasPhoto = @pasPhotoPath, Rekening = @rekeningPath, " +
            "NPWP = @npwpPath, KTP = @ktpPath, Permohonan = @permohonanPath " +
            "WHERE Name = @originalName", conn);

                cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                cmd.Parameters.AddWithValue("@passport", PassportNumberTextBox.Text);
                cmd.Parameters.AddWithValue("@email", EmailTextBox.Text);
                cmd.Parameters.AddWithValue("@visa", VisaTypeComboBox.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@expire", ExpireDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@country", CountryComboBox.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@company", CompanyComboBox.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@originalName", clientName);
                cmd.Parameters.AddWithValue("@passportPath", finalPassportPath);
                cmd.Parameters.AddWithValue("@pasPhotoPath", finalPasPhotoPath);
                cmd.Parameters.AddWithValue("@rekeningPath", finalRekeningPath);
                cmd.Parameters.AddWithValue("@npwpPath", finalNpwpPath);
                cmd.Parameters.AddWithValue("@ktpPath", finalKtpPath);
                cmd.Parameters.AddWithValue("@permohonanPath", finalPermohonanPath);


                cmd.ExecuteNonQuery();
            }

            UpdatedClientName = NameTextBox.Text;
            MessageBox.Show("Client updated successfully!");

            // Navigate to DetailClientsControl
            var parent = Parent as Panel;
            parent?.Children.Remove(this);
            parent?.Children.Add(new DetailClientsControl(UpdatedClientName));

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Confirm if user wants to discard changes
            var result = MessageBox.Show("Discard all changes?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (result == MessageBoxResult.Yes)
            {
                var detailControl = new DetailClientsControl(clientName);
                mainWindow.NavigateTo(detailControl);
            }
        }

        private string CopyFile(string sourcePath, string label)
        {
            if (string.IsNullOrWhiteSpace(sourcePath)) return string.Empty;

            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "client_doc");
            Directory.CreateDirectory(folder);
            string destFileName = $"{label}_{Path.GetFileName(sourcePath)}";
            string destPath = Path.Combine(folder, destFileName);

            try
            {
                File.Copy(sourcePath, destPath, true);
                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy file: {ex.Message}");
                return string.Empty;
            }
        }

        private void ReplaceDocument(ref string currentPath, string documentType)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                currentPath = CopyFile(dlg.FileName, documentType);
                MessageBox.Show($"{documentType} document updated successfully!");
            }
        }

        private string SelectFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf"
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }


        // Document replacement handlers
        private void ReplacePassport_Click(object sender, RoutedEventArgs e)
        {
            var selectedPath = SelectFile();
            if (selectedPath != null)
            {
                stagedPassportPath = selectedPath;
                SetViewButtonState(PassportButton, stagedPassportPath);
                
            }
        }

        private void ReplacePasPhoto_Click(object sender, RoutedEventArgs e)
        {
            var selectedPath = SelectFile();
            if (selectedPath != null)
            {
                stagedPasPhotoPath = selectedPath;
                SetViewButtonState(PasPhotoButton, stagedPasPhotoPath);
                
            }
        }

        private void ReplaceRekening_Click(object sender, RoutedEventArgs e)
        {
            var selectedPath = SelectFile();
            if (selectedPath != null)
            {
                stagedRekeningPath = selectedPath;
                SetViewButtonState(RekeningButton, stagedRekeningPath);
                
            }
        }

        private void ReplaceNPWP_Click(object sender, RoutedEventArgs e)
        {
            var selectedPath = SelectFile();
            if (selectedPath != null)
            {
                stagedNpwpPath = selectedPath;
                SetViewButtonState(NPWPButton, stagedNpwpPath);
                
            }
        }

        private void ReplaceKTP_Click(object sender, RoutedEventArgs e)
        {
            var selectedPath = SelectFile();
            if (selectedPath != null)
            {
                stagedKtpPath = selectedPath;
                SetViewButtonState(KTPButton, stagedKtpPath);
                
            }
        }

        private void ReplacePermohonan_Click(object sender, RoutedEventArgs e)
        {
            var selectedPath = SelectFile();
            if (selectedPath != null)
            {
                stagedPermohonanPath = selectedPath;
                SetViewButtonState(PermohonanButton, stagedPermohonanPath);
                
            }
        }


        private void Passport_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
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

        private void PasPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
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

        private void Rekening_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
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

        private void NPWP_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
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

        private void KTP_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
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

        private void Permohonan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
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


        private void SetViewButtonState(System.Windows.Controls.Button button, string filePath)
        {
            button.Tag = filePath;
            bool exists = File.Exists(filePath);
            button.IsEnabled = exists;
            button.ToolTip = exists ? null : "File not found";
        }

    }
}