using System;
using System.Collections.Generic;
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

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    string query = "INSERT INTO Visa (Name, Requirement, ExpireDate) VALUES (@name, @requirement, @expireDate)";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", visaName);
                    cmd.Parameters.AddWithValue("@requirement", requirement);
                    cmd.Parameters.AddWithValue("@expireDate", expireDate);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Visa added successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
