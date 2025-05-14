using System.Data.SQLite;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace VisaManager
{
    public partial class PreviewVisaControl : UserControl
    {
        public PreviewVisaControl()
        {
            InitializeComponent();
            LoadVisaList();
        }

        private void LoadVisaList(string searchTerm = null)
        {
            VisaListBox.Items.Clear();

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                string query = "SELECT Name, Requirement, ExpireDate FROM Visa";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " WHERE LOWER(Name) LIKE @searchTerm";
                }

                query += " ORDER BY Name ASC";

                SQLiteCommand cmd = new SQLiteCommand(query, conn);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm.ToLower()}%");
                }

                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    VisaListBox.Items.Add(new
                    {
                        Text = reader["Name"].ToString(),
                        FullData = reader // Store the full data if needed
                    });
                }
            }
        }

        private void ShowVisaDetails(string visaName)
        {
            // Create and show the detail control
            var detailControl = new DetailVisaControl(visaName);
            
            // Assuming you have a container in your main window for details
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Clear previous content and add new detail control
                mainWindow.ContentPanel.Children.Clear();
                mainWindow.ContentPanel.Children.Add(detailControl);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox)?.Text;
            LoadVisaList(searchText);
        }

        private async void AddNewVisa_Click(object sender, RoutedEventArgs e)
        {
            var addDialog = new AddVisaDialog();
            var result = await DialogHost.Show(addDialog, "RootDialog");

            if (result is bool visaAdded && visaAdded)
            {
                await Task.Delay(300); // Small delay for smooth UI update
                LoadVisaList();

                // Show success snackbar
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.ShowSnackbar("Visa added successfully!");
            }
        }

        private void VisaListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisaListBox.SelectedItem != null && VisaListBox.SelectedItem is TextBlock textBlock)
            {
                ShowVisaDetails(textBlock.Text);
            }
        }

    }
}