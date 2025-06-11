using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for PreviewCompanyControl.xaml
    /// </summary>
    public partial class PreviewCompanyControl : UserControl
    {
        public PreviewCompanyControl()
        {
            InitializeComponent();
            LoadCompany();
        }
        public class CompanyInfo
        {
            public int Id { get; set; } // Add this property
            public string Name { get; set; }
            public string Contact { get; set; }
            public string Email { get; set; }
            public string AktaPath { get; set; }
            public string SKPath { get; set; }
            public string NIBPath { get; set; }
            public string NPWPPath { get; set; }
            public string Doc1Path { get; set; }
            public string Doc2Path { get; set; }
            public string Doc3Path { get; set; }
            public string Doc4Path { get; set; }
            public string Doc5Path { get; set; }
        }

        private List<Company> companyList = new();


        private void LoadCompany()
        {
            companyList.Clear();

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Id, Name, Contact, Email, Akta, SK, NIB, NPWP, Doc1, Doc2, Doc3, Doc4, Doc5 FROM Company", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var company = new Company
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Contact = reader["Contact"].ToString(),
                        Email = reader["Email"].ToString(),
                        AktaPath = reader["Akta"].ToString(),
                        SKPath = reader["SK"].ToString(),
                        NIBPath = reader["NIB"].ToString(),
                        NPWPPath = reader["NPWP"].ToString(),
                        Doc1Path = reader["Doc1"].ToString(),
                        Doc2Path = reader["Doc2"].ToString(),
                        Doc3Path = reader["Doc3"].ToString(),
                        Doc4Path = reader["Doc4"].ToString(),
                        Doc5Path = reader["Doc5"].ToString()
                    };

                    companyList.Add(company);
                }

                CompanyDataGrid.ItemsSource = null;
                CompanyDataGrid.ItemsSource = companyList;
            }
        }

       

        private void CompanyName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Company company)
            {
                var detailControl = new DetailCompanyControl(company.Name);
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateTo(detailControl);

                if (detailControl.CompanyWasModified)
                {
                    LoadCompany();
                }
            }
        }
    }
}
