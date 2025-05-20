using System.Windows;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace VisaManager
{
    public partial class AddVisaControl : UserControl
    {
        public AddVisaControl()
        {
            InitializeComponent();
            Loaded += (s, e) => VisaNameTextBox.Focus();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string visaName = VisaNameTextBox.Text.Trim();
            string requirement = RequirementTextBox.Text.Trim();
            string expireDate = ExpireDaysTextBox.Text.Trim();

            if (string.IsNullOrEmpty(visaName) || string.IsNullOrEmpty(expireDate) || !int.TryParse(expireDate, out int daysValid))
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            try
            {
                using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Visa (Name, Requirement, ExpireDate) VALUES (@name, @requirement, @expireDate)", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", visaName);
                        cmd.Parameters.AddWithValue("@requirement", requirement);
                        cmd.Parameters.AddWithValue("@expireDate", expireDate);
                        await Task.Run(() => cmd.ExecuteNonQuery());
                    }
                    MessageBox.Show("Visa saved successfully!");
                }


                // Clear fields
                VisaNameTextBox.Text = "";
                RequirementTextBox.Text = "";
                ExpireDaysTextBox.Text = "";
            }
            catch (Exception ex)
            {
                await DialogHost.Show(new ErrorDialog("Database Error",
                    $"Failed to save visa: {ex.Message}"),
                    "RootDialog");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Return to previous view or clear fields
            VisaNameTextBox.Text = "";
            RequirementTextBox.Text = "";
            ExpireDaysTextBox.Text = "";
        }

        // Modern keyboard support
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                SaveButton_Click(sender, e);
                e.Handled = true;
            }
        }



        private static readonly Regex _numericRegex = new Regex("^[0-9]+$");

        private void ExpireDaysTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_numericRegex.IsMatch(e.Text);
        }

        // Optional: block pasting non-numeric content
        private void ExpireDaysTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // No spacebar, bro
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V)
            {
                // This handles Ctrl+V
                var clipboardText = Clipboard.GetText();
                if (!_numericRegex.IsMatch(clipboardText))
                {
                    e.Handled = true;
                }
            }
        }

       
    }

    // Modern error dialog (create as separate UserControl)
    public class ErrorDialog : UserControl
    {
        public ErrorDialog(string title, string message) { /* Implementation */ }
    }
}