using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace VisaManager
{
    public partial class CompanyClientsControl : UserControl
    {
        private readonly string _companyName;

        public CompanyClientsControl(string companyName)
        {
            InitializeComponent();
            _companyName = companyName;
            Loaded += CompanyClientsControl_Loaded;
        }

        private void CompanyClientsControl_Loaded(object sender, RoutedEventArgs e)
        {
            HeaderText.Text = $"Clients from Company: {_companyName}";
            LoadClients();
        }

        private void LoadClients()
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Name, PassportNo, Email, VisaType, ExpireDate " +
                    "FROM Clients WHERE Company = @companyName ORDER BY Name", conn);
                cmd.Parameters.AddWithValue("@companyName", _companyName);

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
                            VisaType = reader["VisaType"].ToString(),
                            ExpireDate = reader["ExpireDate"].ToString()
                        });
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateTo(new PreviewClientsControl());
            }
        }
    }
}