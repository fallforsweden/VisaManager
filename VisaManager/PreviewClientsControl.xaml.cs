using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;



namespace VisaManager
{
    /// <summary>
    /// Interaction logic for PreviewClientsControl.xaml
    /// </summary>
    public partial class PreviewClientsControl : UserControl
    {
        public class ClientInfo
        {
            public string Name { get; set; }
            public string ExpireDate { get; set; }
            public string Country { get; set; }
        }

        public List<ClientInfo> ExpiringClients { get; private set; } = new();

        public PreviewClientsControl()
        {
            InitializeComponent();
            LoadClients();
        }

        private List<ClientInfo> clientList = new();


        private void LoadClients()
        {
            clientList.Clear();
            ExpiringClients.Clear();

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name, ExpireDate, CountryOrigin FROM Clients", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var client = new ClientInfo
                    {
                        Name = reader["Name"].ToString(),
                        ExpireDate = reader["ExpireDate"].ToString(),
                        Country = reader["CountryOrigin"].ToString()
                    };
                    clientList.Add(client);

                    if (DateTime.TryParseExact(client.ExpireDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime expireDate))
                    {
                        int daysLeft = (expireDate - DateTime.Now).Days;
                        if (daysLeft <= 30)
                        {
                            ExpiringClients.Add(client);
                        }
                    }
                }

                ClientsDataGrid.ItemsSource = clientList;
            }
        }

        private void ClientsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var client = (ClientInfo)e.Row.Item;

            if (DateTime.TryParseExact(client.ExpireDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expireDate))
            {
                int daysLeft = (expireDate - DateTime.Now).Days;

                if (daysLeft <= 30)
                {
                    e.Row.Background = Brushes.LightCoral; // 🔴
                }
                else if (daysLeft <= 59)
                {
                    e.Row.Background = Brushes.LightGoldenrodYellow; // 🟡
                }
                else
                {
                    e.Row.Background = Brushes.LightGreen; // 🟢
                }
            }
        }

        private async void Name_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is ClientInfo client)
            {
                var detailControl = new DetailClientsControl(client.Name);
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateTo(detailControl);

                if (detailControl.ClientWasModified)
                {
                    await Task.Delay(300); // Small delay for smooth UI
                    LoadClients();
                }
            }
        }


        public class ExpiryStatusConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string dateStr && DateTime.TryParse(dateStr, out DateTime expireDate))
                {
                    int daysLeft = (expireDate - DateTime.Now).Days;
                    return daysLeft switch
                    {
                        < 30 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")), // Red
                        < 60 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107")), // Yellow
                        _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"))     // Green
                    };
                }
                return Brushes.Transparent;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        // DaysLeftConverter.cs
        public class DaysLeftConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string dateStr && DateTime.TryParse(dateStr, out DateTime expireDate))
                {
                    int daysLeft = (expireDate - DateTime.Now).Days;
                    return daysLeft switch
                    {
                        < 0 => "Expired",
                        < 30 => $"{daysLeft} days (Urgent)",
                        < 60 => $"{daysLeft} days (Warning)",
                        _ => $"{daysLeft} days"
                    };
                }
                return "N/A";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private bool isFiltered = false;

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isFiltered)
            {
                // Show only expiring
                ClientsDataGrid.ItemsSource = ExpiringClients;
                FilterButton.Content = "Show All";
                isFiltered = true;
            }
            else
            {
                // Show all
                ClientsDataGrid.ItemsSource = clientList;
                FilterButton.Content = "Show Expiring";
                isFiltered = false;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                // Show all if empty
                ClientsDataGrid.ItemsSource = isFiltered ? ExpiringClients : clientList;
                return;
            }

            var sourceList = isFiltered ? ExpiringClients : clientList;

            var filtered = sourceList.FindAll(client =>
                client.Name.ToLower().Contains(query) ||
                client.Country.ToLower().Contains(query));

            ClientsDataGrid.ItemsSource = filtered;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchButton_Click(sender, null); // Just reuse that logic
        }


    }
}
