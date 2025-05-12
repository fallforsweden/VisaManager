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
using System.Windows.Shapes;
using static VisaManager.PreviewClients;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for PreviewCompany.xaml
    /// </summary>
    public partial class PreviewCompany : Window
    {
        public PreviewCompany()
        {
            InitializeComponent();
            LoadCompany();
        }

        public class CompanyInfo
        {
            public string Name { get; set; }
            public string Contact { get; set; }
            public string Email { get; set; }
        }

        private List<CompanyInfo> companyList = new();

        private void LoadCompany()
        {
            companyList.Clear();

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name, Contact, Email FROM Company", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    companyList.Add(new CompanyInfo
                    {
                        Name = reader["Name"].ToString(),
                        Contact = reader["Contact"].ToString(),
                        Email = reader["Email"].ToString()
                    });
                }

                CompanyDataGrid.ItemsSource = null;
                CompanyDataGrid.ItemsSource = companyList;
            }
        }

        private void CompanyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CompanyDataGrid.SelectedItem is CompanyInfo selectedCompany)
            {
                var detailWindow = new DetailCompany(selectedCompany.Name); // We'll pass the name!
                detailWindow.ShowDialog();
            }
        }

    }
}
