using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace VisaManager
{
    public partial class PreviewVisa : Window
    {
        public PreviewVisa()
        {
            InitializeComponent();
            LoadVisaList();
        }

        private void LoadVisaList()
        {
            VisaListBox.Items.Clear();

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                string query = "SELECT Name FROM Visa ORDER BY Name ASC";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    VisaListBox.Items.Add(reader["Name"].ToString());
                }

                conn.Close();
            }
        }

        private void VisaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VisaListBox.SelectedItem != null)
            {
                string selectedVisaName = VisaListBox.SelectedItem.ToString();
                DetailVisa detailWindow = new DetailVisa(selectedVisaName);
                bool? result = detailWindow.ShowDialog();

                if (detailWindow.VisaWasModified)
                {
                    LoadVisaList();
                }

            }
        }
    }
}
