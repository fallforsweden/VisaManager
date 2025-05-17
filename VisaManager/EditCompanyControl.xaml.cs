using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using CommunityToolkit.Mvvm.Input;

namespace VisaManager
{
    public partial class EditCompanyControl : UserControl
    {
        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ReplaceDocumentCommand { get; }

        // Current company being edited
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
            // Validate inputs
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Company name is required", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update company object
            _currentCompany.Name = NameTextBox.Text.Trim();
            _currentCompany.Contact = ContactTextBox.Text.Trim();
            _currentCompany.Email = EmailTextBox.Text.Trim();

            // Update document paths from buttons' Tag properties
            _currentCompany.AktaPath = AktaLink.Tag as string;
            _currentCompany.SKPath = SKLink.Tag as string;
            _currentCompany.NIBPath = NIBLink.Tag as string;
            _currentCompany.NPWPPath = NPWPLink.Tag as string;
            _currentCompany.Doc1Path = Doc1Link.Tag as string;
            _currentCompany.Doc2Path = Doc2Link.Tag as string;
            _currentCompany.Doc3Path = Doc3Link.Tag as string;
            _currentCompany.Doc4Path = Doc4Link.Tag as string;
            _currentCompany.Doc5Path = Doc5Link.Tag as string;

            try
            {
                // Save to database or service
                // CompanyService.SaveCompany(_currentCompany);

                MessageBox.Show("Company saved successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Navigate back (implementation depends on your navigation system)
                // MainWindow.NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving company: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEditing()
        {
            // Confirm if user wants to discard changes
            var result = MessageBox.Show("Discard all changes?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Navigate back (implementation depends on your navigation system)
                // MainWindow.NavigateBack();
            }
        }

        private void ReplaceDocument(string documentType)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = $"Replace {documentType}",
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
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