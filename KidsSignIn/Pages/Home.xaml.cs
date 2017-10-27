using FirstFloor.ModernUI.Windows.Controls;
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

namespace KidsSignIn.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }

        private void NewcomerButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new NewcomerDialogue { Title = "Newcomer details" };

            dlg.ShowDialog();

            var addNewcomer = dlg.DialogResult.HasValue ? (bool)dlg.DialogResult : false;
            var newcomerDialogueViewModel = (NewcomerDialogueViewModel)dlg.DataContext;

            if (addNewcomer 
                && !string.IsNullOrEmpty(newcomerDialogueViewModel.First)
                && !string.IsNullOrEmpty(newcomerDialogueViewModel.Last))
            {
                ((HomeViewModel)DataContext).AddNewcomer(
                    newcomerDialogueViewModel.First, 
                    newcomerDialogueViewModel.Last,
                    newcomerDialogueViewModel.MedicalFlag);
            }
        }

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (.csv)|*.csv";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                ((HomeViewModel)DataContext).DataFilename = dlg.FileName;
                ((HomeViewModel)DataContext).Children = new CsvService().LoadData(dlg.FileName);
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            ((HomeViewModel)DataContext).SignIn();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = ModernDialog.ShowMessage("Are you sure you want to clear this record?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) ((HomeViewModel)DataContext).ClearSignIn();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var result = ModernDialog.ShowMessage("Are you sure you want to change this record?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) ((HomeViewModel)DataContext).ChangeSignIn();
        }
    }
}
