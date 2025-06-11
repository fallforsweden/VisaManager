using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace VisaManager
{
    public partial class EditVisaControl : UserControl
    {
        private readonly string _originalVisaName;

        public EditVisaControl(string visaName)
        {
            InitializeComponent();
            _originalVisaName = visaName;
            LoadVisaData();
        }

        private void LoadVisaData()
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Visa WHERE Name = @name", conn);
                cmd.Parameters.AddWithValue("@name", _originalVisaName);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        VisaNameTextBox.Text = reader["Name"].ToString();
                        RequirementTextBox.Text = reader["Requirement"].ToString();
                        ExpireDaysTextBox.Text = reader["ExpireDate"].ToString();
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = VisaNameTextBox.Text.Trim();
            string requirement = RequirementTextBox.Text.Trim();
            string expireDate = ExpireDaysTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(expireDate))
            {
                MessageBox.Show("Please fill in all required fields");
                return;
            }

            try
            {
                using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                    conn.Open();

                    // Update visa
                    var cmd = new SQLiteCommand(
                        "UPDATE Visa SET Name = @newName, Requirement = @requirement, ExpireDate = @expireDate " +
                        "WHERE Name = @originalName", conn);

                    cmd.Parameters.AddWithValue("@newName", newName);
                    cmd.Parameters.AddWithValue("@requirement", requirement);
                    cmd.Parameters.AddWithValue("@expireDate", expireDate);
                    cmd.Parameters.AddWithValue("@originalName", _originalVisaName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Visa updated successfully!");
                        NavigateToDetail(newName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating visa: {ex.Message}");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this visa? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                    {
                        conn.Open();
                        var cmd = new SQLiteCommand("DELETE FROM Visa WHERE Name = @name", conn);
                        cmd.Parameters.AddWithValue("@name", _originalVisaName);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Visa deleted successfully");
                        NavigateBackToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting visa: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDetail(_originalVisaName);
        }

        private void NavigateToDetail(string visaName)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateTo(new DetailVisaControl(visaName));
            }
        }

        private void NavigateBackToList()
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateTo(new PreviewVisaControl());
            }
        }
    }
}