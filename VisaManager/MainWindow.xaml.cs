using System;
using System.Windows;
using System.Data.SQLite;
using System.IO;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string dbPath = "Database/mydata.sqlite";

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();

                string visaTable = @"
                CREATE TABLE IF NOT EXISTS Visa (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Requirement TEXT,
                    ExpireDate TEXT
                );";

                string clientsTable = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    PassportNo TEXT,
                    Email TEXT,
                    VisaType TEXT,
                    ExpireDate TEXT,
                    PassportFile TEXT,
                    CountryOrigin TEXT,
                    PassportPath TEXT,
                    FOREIGN KEY (VisaType) REFERENCES Visa(Name)
                );";

                string companyTable = @"
                CREATE TABLE IF NOT EXISTS Company (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Contact TEXT,
                    Email TEXT,
                    Akta TEXT,
	                SK TEXT,
	                NIB TEXT,
	                NPWP TEXT,
	                Doc1 TEXT,
	                Doc2 TEXT,
	                Doc3 TEXT,
	                Doc4 TEXT,
	                Doc5 TEXT
	                );";

                // Create both tables
                SQLiteCommand cmd1 = new SQLiteCommand(visaTable, conn);
                cmd1.ExecuteNonQuery();

                SQLiteCommand cmd2 = new SQLiteCommand(clientsTable, conn);
                cmd2.ExecuteNonQuery();

                SQLiteCommand cmd3 = new SQLiteCommand(companyTable, conn);
                cmd2.ExecuteNonQuery();

                conn.Close();
            }

        }

        // open window

        private void PreviewCompanies_Click(object sender, RoutedEventArgs e)
        {
            var preview = new AddVisa();
            preview.ShowDialog();
        }

        private void OpenVisaList_Click(object sender, RoutedEventArgs e)
        {
            PreviewVisa preview = new PreviewVisa();
            preview.ShowDialog();
        }

        private void OpenAddClient(object sender, RoutedEventArgs e)
        {
            AddClient preview = new AddClient();
            preview.ShowDialog();
        }
        private void OpenPreviewClient(object sender, RoutedEventArgs e)
        {
            PreviewClients preview = new PreviewClients();
            preview.ShowDialog();
        }

        private void OpenAddCompany(object sender, RoutedEventArgs e)
        {
            AddCompany preview = new AddCompany();
            preview.ShowDialog();
        }

        private void OpenPreviewCompany(object sender, RoutedEventArgs e)
        {
            PreviewCompany preview = new PreviewCompany();
            preview.ShowDialog();
        }
    }
}
