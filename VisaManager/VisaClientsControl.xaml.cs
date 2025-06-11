using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace VisaManager
{
    public partial class VisaClientsControl : UserControl
    {
        private readonly string _visaName;

        public VisaClientsControl(string visaName)
        {
            InitializeComponent();
            _visaName = visaName;
            Loaded += VisaClientsControl_Loaded;
        }

        private void VisaClientsControl_Loaded(object sender, RoutedEventArgs e)
        {
            HeaderText.Text = $"Clients with Visa: {_visaName}";
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                Console.WriteLine($"Loading clients for visa: {_visaName}"); // Debug output

                using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand(
                        "SELECT Name, PassportNo, Email, ExpireDate, CountryOrigin, Company " +
                        "FROM Clients WHERE VisaType = @visaName ORDER BY Name", conn);
                    cmd.Parameters.AddWithValue("@visaName", _visaName);

                    // Debug: Print the actual SQL being executed
                    Console.WriteLine($"Executing query: {cmd.CommandText}");
                    Console.WriteLine($"With parameter: {_visaName}");

                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            count++;
                            ClientsDataGrid.Items.Add(new
                            {
                                Name = reader["Name"].ToString(),
                                PassportNo = reader["PassportNo"].ToString(),
                                Email = reader["Email"].ToString(),
                                ExpireDate = reader["ExpireDate"].ToString(),
                                CountryOrigin = reader["CountryOrigin"].ToString(),
                                Company = reader["Company"].ToString()
                            });
                        }
                        Console.WriteLine($"Found {count} clients"); // Debug output
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading clients: {ex.Message}"); // Debug output
                MessageBox.Show($"Error loading clients: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the parent window and navigate back
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateTo(new PreviewVisaControl());
            }
        }
    }
}