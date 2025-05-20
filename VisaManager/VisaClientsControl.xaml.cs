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
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Name, PassportNo, Email, ExpireDate, CountryOrigin, Company " +
                    "FROM Clients WHERE VisaType = @visaName ORDER BY Name", conn);
                cmd.Parameters.AddWithValue("@visaName", _visaName);

                using (var reader = cmd.ExecuteReader())
                {
                    ClientsDataGrid.Items.Clear();
                    while (reader.Read())
                    {
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
                }
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