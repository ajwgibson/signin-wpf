using KidsSignIn.Model;
using KidsSignIn.Service;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KidsSignIn.Pages.Settings
{
    /// <summary>
    /// Interaction logic for Printing.xaml
    /// </summary>
    public partial class Printing : UserControl
    {
        public Printing()
        {
            InitializeComponent();
            DataContext = new PrintingViewModel();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = ((PrintingViewModel)DataContext);
            context.Save();
            PrintService.Instance.Configure(context.Printer, context.LabelFile, context.Copies, context.PrintSundayDate);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".label";
            dlg.Filter = "Dymo label files (.label)|*.label";
            dlg.FileName = ((PrintingViewModel)DataContext).LabelFile;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                ((PrintingViewModel)DataContext).LabelFile = dlg.FileName;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            PrintService.Instance.Print(new Child { First = "Test", Last = "Person", RoomLabel = "A room", IsNewcomer = true }, 1);
        }
    }
}
