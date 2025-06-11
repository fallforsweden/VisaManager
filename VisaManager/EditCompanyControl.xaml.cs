using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using System.Data.SQLite;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Data;

namespace VisaManager
{
    public partial class EditCompanyControl : UserControl
    {
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ReplaceDocumentCommand { get; }

        private Company _currentCompany;

        public EditCompanyControl(Company company)
        {
            InitializeComponent();
            _currentCompany = company ?? throw new ArgumentNullException(nameof(company));

            // Initialize commands
            SaveCommand = new RelayCommand(SaveCompany);
            CancelCommand = new RelayCommand(CancelEditing);
            ReplaceDocumentCommand = new RelayCommand<string>(ReplaceDocument);

            // Set data context to enable command binding
            DataContext = this;

            // Load company data
            LoadCompanyData();
        }

        private void LoadCompanyData()
        {
            // Basic info
            NameTextBox.Text = _currentCompany.Name;
            ContactTextBox.Text = _currentCompany.Contact;
            EmailTextBox.Text = _currentCompany.Email;

            // Documents - initialize view buttons
            InitializeDocumentButton(AktaLink, _currentCompany.AktaPath);
            InitializeDocumentButton(SKLink, _currentCompany.SKPath);
            InitializeDocumentButton(NIBLink, _currentCompany.NIBPath);
            InitializeDocumentButton(NPWPLink, _currentCompany.NPWPPath);
            InitializeDocumentButton(Doc1Link, _currentCompany.Doc1Path);
            InitializeDocumentButton(Doc2Link, _currentCompany.Doc2Path);
            InitializeDocumentButton(Doc3Link, _currentCompany.Doc3Path);
            InitializeDocumentButton(Doc4Link, _currentCompany.Doc4Path);
            InitializeDocumentButton(Doc5Link, _currentCompany.Doc5Path);
        }

        private void InitializeDocumentButton(Button button, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                button.Content = Path.GetFileName(filePath);
                button.Tag = filePath; // Store full path in Tag
            }
        }

        private void SaveCompany()
        {
            // 🌟 1. Validate name
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Company name is required.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 🌟 2. Prepare company data
            var updatedCompany = new Company
            {
                Id = _currentCompany.Id,
                Name = NameTextBox.Text.Trim(),
                Contact = ContactTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                AktaPath = AktaLink.Tag as string,
                SKPath = SKLink.Tag as string,
                NIBPath = NIBLink.Tag as string,
                NPWPPath = NPWPLink.Tag as string,
                Doc1Path = Doc1Link.Tag as string,
                Doc2Path = Doc2Link.Tag as string,
                Doc3Path = Doc3Link.Tag as string,
                Doc4Path = Doc4Link.Tag as string,
                Doc5Path = Doc5Link.Tag as string
            };

            try
            {
                using var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;");
                conn.Open();

                using var cmd = new SQLiteCommand(@"
            UPDATE Company SET
                Name = @Name,
                Contact = @Contact,
                Email = @Email,
                Akta = @Akta,
                SK = @SK,
                NIB = @NIB,
                NPWP = @NPWP,
                Doc1 = @Doc1,
                Doc2 = @Doc2,
                Doc3 = @Doc3,
                Doc4 = @Doc4,
                Doc5 = @Doc5
            WHERE Id = @Id;", conn);

                // 🌟 3. Secure parameter binding
                cmd.Parameters.AddWithValue("@Name", updatedCompany.Name);
                cmd.Parameters.AddWithValue("@Contact", updatedCompany.Contact);
                cmd.Parameters.AddWithValue("@Email", updatedCompany.Email);
                cmd.Parameters.AddWithValue("@Akta", updatedCompany.AktaPath ?? string.Empty);
                cmd.Parameters.AddWithValue("@SK", updatedCompany.SKPath ?? string.Empty);
                cmd.Parameters.AddWithValue("@NIB", updatedCompany.NIBPath ?? string.Empty);
                cmd.Parameters.AddWithValue("@NPWP", updatedCompany.NPWPPath ?? string.Empty);
                cmd.Parameters.AddWithValue("@Doc1", updatedCompany.Doc1Path ?? string.Empty);
                cmd.Parameters.AddWithValue("@Doc2", updatedCompany.Doc2Path ?? string.Empty);
                cmd.Parameters.AddWithValue("@Doc3", updatedCompany.Doc3Path ?? string.Empty);
                cmd.Parameters.AddWithValue("@Doc4", updatedCompany.Doc4Path ?? string.Empty);
                cmd.Parameters.AddWithValue("@Doc5", updatedCompany.Doc5Path ?? string.Empty);
                cmd.Parameters.AddWithValue("@Id", updatedCompany.Id);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    // Navigate back to detail view
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    if (mainWindow != null)
                    {
                        var detailControl = new DetailCompanyControl(updatedCompany.Name);
                        mainWindow.NavigateTo(detailControl);
                    }

                    MessageBox.Show("Company saved successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No changes saved. Company may not exist.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went error:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelEditing()
        {
            // Confirm if user wants to discard changes
            var result = MessageBox.Show("Discard all changes?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (result == MessageBoxResult.Yes)
            {
                var detailControl = new DetailCompanyControl(_currentCompany.Name);
                mainWindow.NavigateTo(detailControl);
            }
        }

        private void ReplaceDocument(string documentType)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = $"Replace {documentType}",
                Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var fileName = Path.GetFileName(filePath);

                // Update the appropriate button based on document type
                switch (documentType)
                {
                    case "Akta":
                        AktaLink.Content = fileName;
                        AktaLink.Tag = filePath;
                        break;
                    case "SK":
                        SKLink.Content = fileName;
                        SKLink.Tag = filePath;
                        break;
                    case "NIB":
                        NIBLink.Content = fileName;
                        NIBLink.Tag = filePath;
                        break;
                    case "NPWP":
                        NPWPLink.Content = fileName;
                        NPWPLink.Tag = filePath;
                        break;
                    case "Doc1":
                        Doc1Link.Content = fileName;
                        Doc1Link.Tag = filePath;
                        break;
                    case "Doc2":
                        Doc2Link.Content = fileName;
                        Doc2Link.Tag = filePath;
                        break;
                    case "Doc3":
                        Doc3Link.Content = fileName;
                        Doc3Link.Tag = filePath;
                        break;
                    case "Doc4":
                        Doc4Link.Content = fileName;
                        Doc4Link.Tag = filePath;
                        break;
                    case "Doc5":
                        Doc5Link.Content = fileName;
                        Doc5Link.Tag = filePath;
                        break;
                }
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string filePath && !string.IsNullOrEmpty(filePath))
            {
                try
                {
                    // Open the file with default application
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No document available", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Example Company class - replace with your actual implementation
    public class Company
    {
        public int Id { get; set; } // Add this property
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string AktaPath { get; set; }
        public string SKPath { get; set; }
        public string NIBPath { get; set; }
        public string NPWPPath { get; set; }
        public string Doc1Path { get; set; }
        public string Doc2Path { get; set; }
        public string Doc3Path { get; set; }
        public string Doc4Path { get; set; }
        public string Doc5Path { get; set; }
    }
}