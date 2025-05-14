using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace VisaManager
{
    public partial class DetailVisaControl : UserControl
    {
        private string visaName;

        public DetailVisaControl(string name)
        {
            InitializeComponent();
            visaName = name;
            LoadVisaDetails();
        }

        private void LoadVisaDetails()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                string query = "SELECT * FROM Visa WHERE Name = @name";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", visaName);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        VisaNameText.Text = reader["Name"].ToString();
                        RequirementsText.Text = reader["Requirement"].ToString();
                        ExpireDateText.Text = reader["ExpireDate"].ToString();
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove this control from its parent
            var parent = Parent as Panel;
            parent?.Children.Remove(this);
        }
    }
}