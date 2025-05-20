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
        public class Visa
        {
            public string Name { get; set; }
            public string Requirement { get; set; }
            public string ExpireDate { get; set; }
        }

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
                    VisaListBox.Items.Add(new Visa
                    {
                        Name = reader["Name"].ToString(),
                        Requirement = reader["Requirement"].ToString(),
                        ExpireDate = reader["ExpireDate"].ToString()
                    });
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox)?.Text;
            LoadVisaList(searchText);
        }

        private void VisaListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisaListBox.SelectedItem is Visa selectedVisa)
            {
                ShowVisaDetails(selectedVisa.Name);
            }
        }

        private void VisaName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is Visa visa)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateTo(new VisaClientsControl(visa.Name));
            }
        }

        private void ShowVisaDetails(string visaName)
        {
            var detailControl = new DetailVisaControl(visaName);
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.ContentPanel.Children.Clear();
            mainWindow?.ContentPanel.Children.Add(detailControl);
        }

        private async void AddNewVisa_Click(object sender, RoutedEventArgs e)
        {
            var addDialog = new AddVisaDialog();
            var result = await DialogHost.Show(addDialog, "RootDialog");

            if (result is bool visaAdded && visaAdded)
            {
                await Task.Delay(300);
                LoadVisaList();

                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.ShowSnackbar("Visa added successfully!");
            }
        }
    }
}