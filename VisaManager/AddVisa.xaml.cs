using System.Windows;
using System.Data.SQLite;
namespace VisaManager
{
    /// <summary>
    /// Interaction logic for AddVisa.xaml
    /// </summary>
    public partial class AddVisa : Window
    {
        public AddVisa()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string visaName = VisaNameTextBox.Text.Trim();
            string requirement = RequirementTextBox.Text.Trim();
            string expireDate = ExpireDaysTextBox.Text.Trim();

            if (string.IsNullOrEmpty(visaName) || string.IsNullOrEmpty(expireDate) || !int.TryParse(expireDate, out int daysValid))
            {
                MessageBox.Show("Visa name and a valid number of days (1–1000) are required.");
                return;
            }
 
                using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                conn.Open();
                    using (var cmd = new SQLiteCommand("INSERT INTO Visa (Name, Requirement, ExpireDate) VALUES (@name, @requirement, @expireDate)", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", visaName);
                        cmd.Parameters.AddWithValue("@requirement", requirement);
                        cmd.Parameters.AddWithValue("@expireDate", expireDate);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Visa added successfully!");
                this.Close();
        }
    }
}
