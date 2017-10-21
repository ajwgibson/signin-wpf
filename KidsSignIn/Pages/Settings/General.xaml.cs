using System.Windows;
using System.Windows.Controls;

namespace KidsSignIn.Pages.Settings
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl
    {
        public General()
        {
            InitializeComponent();
            DataContext = new GeneralViewModel();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ((GeneralViewModel)DataContext).Save();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ((GeneralViewModel)DataContext).Refresh();
        }
    }
}
