using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VisaManager
{
    public partial class PreviewClients : Window
    {
        public class ClientInfo
        {
            public string Name { get; set; }
            public string ExpireDate { get; set; }
            public string Country { get; set; }
        }

        private List<ClientInfo> clientList = new();

        public PreviewClients()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            clientList.Clear();

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name, ExpireDate, CountryOrigin FROM Clients", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clientList.Add(new ClientInfo
                    {
                        Name = reader["Name"].ToString(),
                        ExpireDate = reader["ExpireDate"].ToString(),
                        Country = reader["CountryOrigin"].ToString()
                    });

                }
            }

            ClientsDataGrid.ItemsSource = clientList;
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

        private void Name_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is ClientInfo client)
            {
                DetailClient detailWindow = new DetailClient(client.Name);
                detailWindow.ShowDialog();
            }
        }

    }
}
