using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for AddCompany.xaml
    /// </summary>
    public partial class AddCompany : Window
    {
        public AddCompany()
        {
            InitializeComponent();
        }

        private void UploadAkta_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                AktaPathText.Text = dlg.FileName;
            }
        }

        private void SK_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                SKPathText.Text = dlg.FileName;
            }
        }

        private void NIB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                NIBPathText.Text = dlg.FileName;
            }
        }

        private void NPWP_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                NPWPPathText.Text = dlg.FileName;
            }
        }

        private void Doc1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                Doc1PathText.Text = dlg.FileName;
            }
        }

        private void Doc2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                Doc2PathText.Text = dlg.FileName;
            }
        }

        private void Doc3_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                Doc3PathText.Text = dlg.FileName;
            }
        }

        private void Doc4_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                Doc4PathText.Text = dlg.FileName;
            }
        }

        private void Doc5_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf";
            if (dlg.ShowDialog() == true)
            {
                Doc5PathText.Text = dlg.FileName;
            }
        }

        private string CopyFile(string sourcePath, string label)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath)) return "";
            string folder = @"C:\VisaManager";
            Directory.CreateDirectory(folder);
            string destFileName = $"{label}_{Path.GetFileName(sourcePath)}";
            string destPath = Path.Combine(folder, destFileName);
            File.Copy(sourcePath, destPath, true);
            return destPath;
        }


        private void SaveCompany_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string contact = ContactTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();


            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contact) ||
                string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("INSERT INTO Company (Name, Contact, Email, Akta, SK, NIB, NPWP, Doc1, Doc2, Doc3, Doc4, Doc5) VALUES (@name, @contact, @email, @akta, @sk, @nib, @npwp, @doc1, @doc2, @doc3, @doc4, @doc5)", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@contact", contact);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@akta", CopyFile(AktaPathText.Text, "Akta"));
                cmd.Parameters.AddWithValue("@sk", CopyFile(SKPathText.Text, "SK"));
                cmd.Parameters.AddWithValue("@nib", CopyFile(NIBPathText.Text, "NIB"));
                cmd.Parameters.AddWithValue("@npwp", CopyFile(NPWPPathText.Text, "NPWP"));
                cmd.Parameters.AddWithValue("@doc1", CopyFile(Doc1PathText.Text, "Doc1"));
                cmd.Parameters.AddWithValue("@doc2", CopyFile(Doc2PathText.Text, "Doc2"));
                cmd.Parameters.AddWithValue("@doc3", CopyFile(Doc3PathText.Text, "Doc3"));
                cmd.Parameters.AddWithValue("@doc4", CopyFile(Doc4PathText.Text, "Doc4"));
                cmd.Parameters.AddWithValue("@doc5", CopyFile(Doc5PathText.Text, "Doc5"));

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Company saved successfully!");
            this.Close();
        }
    }
}
