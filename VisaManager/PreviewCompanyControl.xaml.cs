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
                DetailCompany detailWindow = new DetailCompany(selectedCompany.Name);
                bool? result = detailWindow.ShowDialog();

                if (detailWindow.CompanyWasModified)
                {
                    LoadCompany();
                }
            }
        }

        private async void CompanyName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CompanyInfo company)
            {
                var detailControl = new DetailCompanyControl(company.Name);
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateTo(detailControl);

                if (detailControl.CompanyWasModified)
                {
                    await Task.Delay(300); // Small delay for smooth UI
                    LoadCompany();
                }
            }
        }
    }
}
