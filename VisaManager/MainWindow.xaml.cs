using System;
using System.Windows;
using System.Data.SQLite;
using System.IO;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using System.Text;
using System.Windows.Media;
using System.Collections.Generic;
using System.Globalization;
using static MaterialDesignThemes.Wpf.Theme;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class ClientInfo
        {
            public string Name { get; set; }
            public string ExpireDate { get; set; }
            public string Country { get; set; }
        }

        public class NavigationItem
        {
            public string Title { get; set; }
            public PackIconKind Icon { get; set; }
            public bool IsExpanded { get; set; }
            public ICommand Command { get; set; }
            public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();
        }

        public ObservableCollection<NavigationItem> NavigationItems { get; } = new ObservableCollection<NavigationItem>();

        public List<ClientInfo> ExpiringClients { get; private set; } = new();
        public SnackbarMessageQueue MessageQueue { get; }

        public MainWindow()
        {
            InitializeComponent();
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            MySnackbar.MessageQueue = MessageQueue;
            StartWelcomeAnimation();


            string dbPath = "Database/mydata.sqlite";

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();

                string visaTable = @"
                CREATE TABLE IF NOT EXISTS Visa (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Requirement TEXT,
                    ExpireDate TEXT
                );";

                string clientsTable = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    PassportNo TEXT,
                    Email TEXT,
                    VisaType TEXT,
                    ExpireDate TEXT,
                    Company TEXT,
                    CountryOrigin TEXT,
                    Passport TEXT,
                    PasPhoto TEXT, 
                    Rekening TEXT,
                    KTP TEXT,
                    Permohonan TEXT,
                    NPWP TEXT,
                    FOREIGN KEY (VisaType) REFERENCES Visa(Name)
                    FOREIGN KEY (Company) REFERENCES Company(Name)
                );";

                string companyTable = @"
                CREATE TABLE IF NOT EXISTS Company (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Contact TEXT,
                    Email TEXT,
                    Akta TEXT,
	                SK TEXT,
	                NIB TEXT,
	                NPWP TEXT,
	                Doc1 TEXT,
	                Doc2 TEXT,
	                Doc3 TEXT,
	                Doc4 TEXT,
	                Doc5 TEXT
	                );";

                // Create both tables
                SQLiteCommand cmd1 = new SQLiteCommand(visaTable, conn);
                cmd1.ExecuteNonQuery();

                SQLiteCommand cmd2 = new SQLiteCommand(clientsTable, conn);
                cmd2.ExecuteNonQuery();

                SQLiteCommand cmd3 = new SQLiteCommand(companyTable, conn);
                cmd2.ExecuteNonQuery();

                conn.Close();
            }

            // Initialize navigation items
            NavigationItems.Add(new NavigationItem
            {
                Title = "Visa",
                Icon = PackIconKind.Passport,
                IsExpanded = false,
                Children =
        {
            new NavigationItem
            {
                Title = "Add Visa",
                Icon = PackIconKind.Plus,
                Command = new RelayCommand(() => ShowContent(new AddVisaControl()))
            },
            new NavigationItem
            {
                Title = "Manage",
                Icon = PackIconKind.FormatListBulleted,
                Command = new RelayCommand(() => ShowContent(new PreviewVisaControl()))
            }
        }
            });

            NavigationItems.Add(new NavigationItem
            {
                Title = "Client",
                Icon = PackIconKind.Account,
                IsExpanded = false,
                Children =
        {
            new NavigationItem
            {
                Title = "Add Client",
                Icon = PackIconKind.Plus,
                Command = new RelayCommand(() => ShowContent(new AddClientsControl()))
            },
            new NavigationItem
            {
                Title = "Manage",
                Icon = PackIconKind.FormatListBulleted,
                Command = new RelayCommand(() => ShowContent(new PreviewClientsControl()))
            }
        }
            });

            NavigationItems.Add(new NavigationItem
            {
                Title = "Company",
                Icon = PackIconKind.OfficeBuilding,
                IsExpanded = false,
                Children =
        {
            new NavigationItem
            {
                Title = "Add Company",
                Icon = PackIconKind.Plus,
                Command = new RelayCommand(() => ShowContent(new AddCompanyControl()))
            },
            new NavigationItem
            {
                Title = "Manage",
                Icon = PackIconKind.FormatListBulleted,
                Command = new RelayCommand(() => ShowContent(new PreviewCompanyControl()))
            }
        }
            });

            DataContext = this;

        }


        private async void StartWelcomeAnimation()
        {
            await Task.Delay(1500); // Initial delay

           

            await Task.Delay(500); // Pause for logo effect

            // 2. "Pull tab" stretch effect
            var tabAnimation = new DoubleAnimation
            {
                From = 1,
                To = 1.5,
                Duration = TimeSpan.FromMilliseconds(200),
                AutoReverse = true,
                EasingFunction = new SineEase()
            };
            PullTabScale.BeginAnimation(ScaleTransform.ScaleYProperty, tabAnimation);

            await Task.Delay(200);

            // 3. Curtain rise with variable speed
            var curtainAnimation = new DoubleAnimation
            {
                From = 0,
                To = -ActualHeight,
                Duration = TimeSpan.FromSeconds(1.2),
                EasingFunction = new PowerEase
                {
                    Power = 3,
                    EasingMode = EasingMode.EaseOut
                }
            };

            curtainAnimation.Completed += (s, e) =>
            {
                WelcomeCurtain.Visibility = Visibility.Collapsed;
            };

            CurtainTransform.BeginAnimation(TranslateTransform.YProperty, curtainAnimation);

            // 4. Simultaneous fade-to-transparent
            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.8),
                BeginTime = TimeSpan.FromSeconds(0.4) // Delayed start
            };
            WelcomeCurtain.BeginAnimation(OpacityProperty, fadeAnimation);
        }

        public async void ShowContent(UserControl control)
        {
            // Fade out current content
            var fadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            ContentPanel.BeginAnimation(OpacityProperty, fadeOut);

            await Task.Delay(200);

            // Clear and add new content
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(control);

            // Fade in new content
            var fadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));
            ContentPanel.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void AddVisa_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(new AddVisaControl());
        }

        private void OpenVisaList_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(new PreviewVisaControl());
        }

        private void OpenAddClient(object sender, RoutedEventArgs e)
        {
            ShowContent(new AddClientsControl());
        }
        private void OpenPreviewClient(object sender, RoutedEventArgs e)
        {
            ShowContent(new PreviewClientsControl());
        }

        private void OpenAddCompany(object sender, RoutedEventArgs e)
        {
            ShowContent(new AddCompanyControl());
        }

        private void OpenPreviewCompany(object sender, RoutedEventArgs e)
        {
            ShowContent(new PreviewCompanyControl());
        }


        public void ShowSnackbar(string message)
        {
            MessageQueue.Enqueue(message);
        }

        public void NavigateTo(UserControl control)
        {
            ContentPanel.Children.Clear(); ContentPanel.Children.Add(control);

        }

        private void AppTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Show notification control instead of refreshing
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new NotificationControl());

            // Optional: Show snackbar notification
            ShowSnackbar("Showing clients with expiring visas");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        public void NavigateToClientDetail(string passportNo)
        {
            ShowContent(new DetailClientsControl(passportNo));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }


    }

}
