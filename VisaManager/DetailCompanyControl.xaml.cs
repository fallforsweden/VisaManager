using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for DetailCompanyControl.xaml
    /// </summary>
    public partial class DetailCompanyControl : UserControl
    {

        public bool CompanyWasModified { get; private set; } = false;
        private string CompanyName;


        public DetailCompanyControl(string name)
        {
            InitializeComponent();
            CompanyName = name;
            LoadCompanyDetails();
        }



        private void LoadCompanyDetails()
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Company WHERE Name = @name", conn);
                cmd.Parameters.AddWithValue("@name", CompanyName);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    NameLabel.Text = reader["Name"].ToString();
                    ContactLabel.Text = reader["Contact"].ToString();
                    EmailLabel.Text = reader["Email"].ToString();
                    AktaLink.Tag = reader["Akta"].ToString();
                    AktaLink.Visibility = File.Exists(AktaLink.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    SKLink.Tag = reader["SK"].ToString();
                    SKLink.Visibility = File.Exists(SKLink.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    NIBLink.Tag = reader["NIB"].ToString();
                    NIBLink.Visibility = File.Exists(NIBLink.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    NPWPLink.Tag = reader["NPWP"].ToString();
                    NPWPLink.Visibility = File.Exists(NPWPLink.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    Doc1Link.Tag = reader["Doc1"].ToString();
                    Doc1Link.Visibility = File.Exists(Doc1Link.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    Doc2Link.Tag = reader["Doc2"].ToString();
                    Doc2Link.Visibility = File.Exists(Doc2Link.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    Doc3Link.Tag = reader["Doc3"].ToString();
                    Doc3Link.Visibility = File.Exists(Doc3Link.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    Doc4Link.Tag = reader["Doc4"].ToString();
                    Doc4Link.Visibility = File.Exists(Doc4Link.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    Doc5Link.Tag = reader["Doc5"].ToString();
                    Doc5Link.Visibility = File.Exists(Doc5Link.Tag?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    ShowDocument(AktaLink, AktaPlaceholder, reader["Akta"].ToString());
                    ShowDocument(SKLink, SKPlaceholder, reader["SK"].ToString());
                    ShowDocument(NIBLink, NIBPlaceholder, reader["NIB"].ToString());
                    ShowDocument(NPWPLink, NPWPPlaceholder, reader["NPWP"].ToString());
                    ShowDocument(Doc1Link, Doc1Placeholder, reader["Doc1"].ToString());
                    ShowDocument(Doc2Link, Doc2Placeholder, reader["Doc2"].ToString());
                    ShowDocument(Doc3Link, Doc3Placeholder, reader["Doc3"].ToString());
                    ShowDocument(Doc4Link, Doc4Placeholder, reader["Doc4"].ToString());
                    ShowDocument(Doc5Link, Doc5Placeholder, reader["Doc5"].ToString());

                }
                reader.Close();
            }
        }

        private void ShowDocument(Button button, TextBlock placeholder, string filePath)
        {
            button.Tag = filePath;

            if (File.Exists(filePath))
            {
                button.Visibility = Visibility.Visible;
                placeholder.Visibility = Visibility.Collapsed;

                // Animate opacity
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                button.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
            else
            {
                button.Visibility = Visibility.Collapsed;
                placeholder.Visibility = Visibility.Visible;
            }
        }



        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string filePath)
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

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the current company details
            var company = new Company
            {
                Name = NameLabel.Text,
                Contact = ContactLabel.Text,
                Email = EmailLabel.Text,
                AktaPath = AktaLink.Tag?.ToString(),
                SKPath = SKLink.Tag?.ToString(),
                NIBPath = NIBLink.Tag?.ToString(),
                NPWPPath = NPWPLink.Tag?.ToString(),
                Doc1Path = Doc1Link.Tag?.ToString(),
                Doc2Path = Doc2Link.Tag?.ToString(),
                Doc3Path = Doc3Link.Tag?.ToString(),
                Doc4Path = Doc4Link.Tag?.ToString(),
                Doc5Path = Doc5Link.Tag?.ToString()
            };

            // Get the ID from database
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Id FROM Company WHERE Name = @name", conn);
                cmd.Parameters.AddWithValue("@name", company.Name);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    company.Id = Convert.ToInt32(result);
                }
            }

            // Navigate to edit control
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var editControl = new EditCompanyControl(company);
                mainWindow.NavigateTo(editControl);
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this company?",
                "Confirm Delete",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // Delete from database
                using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand("DELETE FROM Company WHERE Name = @name", conn);
                    cmd.Parameters.AddWithValue("@name", CompanyName);
                    cmd.ExecuteNonQuery();
                }

                // Get the parent window (MainWindow)
                var mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null)
                {
                    // Navigate back to PreviewCompanyControl
                    mainWindow.ShowContent(new PreviewCompanyControl());

                    // Optional: Show confirmation snackbar
                    mainWindow.ShowSnackbar("Company deleted successfully");
                }

                CompanyWasModified = true;
            }
        }
    }
}
