using System;
using System.Collections.Generic;
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
            var visaList = new List<Visa>();

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name, Requirement, ExpireDate FROM Visa", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    visaList.Add(new Visa
                    {
                        Name = reader["Name"].ToString(),
                        Requirement = reader["Requirement"].ToString(),
                        ExpireDate = reader["ExpireDate"].ToString()
                    });
                }
            }

            VisaDataGrid.ItemsSource = visaList;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox)?.Text;
            LoadVisaList(searchText);
        }

        private void VisaListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisaDataGrid.SelectedItem is Visa selectedVisa)
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

        private void VisaName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Visa visa)
            {
                var detailControl = new DetailVisaControl(visa.Name);
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateTo(detailControl);
            }
        }

        private void ViewClients_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Visa visa)
            {
                Console.WriteLine($"Navigating to clients view for visa: {visa.Name}"); // Debug
                var clientsControl = new VisaClientsControl(visa.Name);
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateTo(clientsControl);
            }
        }

        private void EditVisa_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Visa visa)
            {
                var editControl = new EditVisaControl(visa.Name);
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateTo(editControl);
            }
        }
    }
}