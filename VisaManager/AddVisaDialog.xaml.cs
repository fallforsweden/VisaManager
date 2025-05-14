using MaterialDesignThemes.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisaManager
{
    /// <summary>
    /// Interaction logic for AddVisaDialog.xaml
    /// </summary>
    public partial class AddVisaDialog : UserControl
    {
        public bool VisaAdded { get; set; } = false;

        public AddVisaDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Your save logic here...
            VisaAdded = true; // Set to true when successfully added
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            VisaAdded = false;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
