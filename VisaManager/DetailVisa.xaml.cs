using System;
using System.Data.SQLite;
using System.Windows;

namespace VisaManager
{
    public partial class DetailVisa : Window
    {
        private string _originalVisaName;

        public DetailVisa(string visaName)
        {
            InitializeComponent();
            _originalVisaName = visaName;
            LoadVisaDetails();
        }
        public bool VisaWasModified { get; private set; } = false;

        private void LoadVisaDetails()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=mydata.sqlite;Version=3;"))
            {
                conn.Open();
                string query = "SELECT * FROM Visa WHERE Name = @name";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", _originalVisaName);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            VisaNameTextBox.Text = reader["Name"].ToString();
                            RequirementTextBox.Text = reader["Requirement"].ToString();
                            ExpireDaysTextBox.Text = reader["ExpireDate"].ToString();
                        }
                    }
                }
                conn.Close(); // <- Not strictly needed, but just to be explicit
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            VisaNameTextBox.IsReadOnly = false;
            RequirementTextBox.IsReadOnly = false;
            ExpireDaysTextBox.IsReadOnly = false;
            SaveButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Collapsed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = VisaNameTextBox.Text.Trim();
            string requirement = RequirementTextBox.Text.Trim();
            string expireDays = ExpireDaysTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(expireDays) || !int.TryParse(expireDays, out _))
            {
                MessageBox.Show("Please enter valid values.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=mydata.sqlite;Version=3;"))
            {
                conn.Open();
                string query = "UPDATE Visa SET Name = @newName, Requirement = @requirement, ExpireDate = @expireDays WHERE Name = @originalName";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@newName", newName);
                cmd.Parameters.AddWithValue("@requirement", requirement);
                cmd.Parameters.AddWithValue("@expireDays", expireDays);
                cmd.Parameters.AddWithValue("@originalName", _originalVisaName);
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            _originalVisaName = newName;
            MessageBox.Show("Visa updated!");

            VisaNameTextBox.IsReadOnly = true;
            RequirementTextBox.IsReadOnly = true;
            ExpireDaysTextBox.IsReadOnly = true;
            SaveButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Visible;
            VisaWasModified = true;

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this visa?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    string query = "DELETE FROM Visa WHERE Name = @name";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", _originalVisaName);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Visa deleted.");
                VisaWasModified = true;
                this.Close();
            }
        }


    }
}
