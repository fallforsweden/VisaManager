using CommunityToolkit.Mvvm.Input;
using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace VisaManager
{
    public partial class NotificationControl : UserControl
    {
        public class ExpiringClient
        {
            public string Name { get; set; }
            public string PassportNo { get; set; }
            public string VisaType { get; set; }
            public string ExpireDate { get; set; }
            public int DaysLeft { get; set; }
        }



        public NotificationControl()
        {
            InitializeComponent();
            LoadExpiringClients();
        }


        private void LoadExpiringClients()
        {
            using (var conn = new SQLiteConnection("Data Source=Database/mydata.sqlite;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Name, PassportNo, VisaType, ExpireDate " +
                    "FROM Clients WHERE ExpireDate IS NOT NULL", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var expireDateStr = reader["ExpireDate"].ToString();
                        if (DateTime.TryParse(expireDateStr, out DateTime expireDate))
                        {
                            var daysLeft = (expireDate - DateTime.Now).Days;
                            if (daysLeft <= 30) // Show clients expiring within 30 days
                            {
                                ExpiringClientsDataGrid.Items.Add(new ExpiringClient
                                {
                                    Name = reader["Name"].ToString(),
                                    PassportNo = reader["PassportNo"].ToString(),
                                    VisaType = reader["VisaType"].ToString(),
                                    ExpireDate = expireDate.ToString("yyyy-MM-dd"),
                                    DaysLeft = daysLeft
                                });
                            }
                        }
                    }
                }
            }
        }

      
    }
}