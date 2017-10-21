namespace KidsSignIn.Pages.Settings
{
    public class GeneralViewModel : ViewModelBase
    {
        private int    nextId      = Properties.Settings.Default.NextId;
        private string laptopLabel = Properties.Settings.Default.LaptopLabel;

        public int NextId 
        {
            get { return nextId; }
            set { nextId = value; }
        }

        public string LaptopLabel
        {
            get { return laptopLabel; }
            set { laptopLabel = value; }
        }

        public void Refresh()
        {
            NextId      = Properties.Settings.Default.NextId;
            LaptopLabel = Properties.Settings.Default.LaptopLabel;
            NotifyPropertyChanged("NextId");
            NotifyPropertyChanged("LaptopLabel");
        }

        public void Save()
        {
            Properties.Settings.Default.NextId      = nextId;
            Properties.Settings.Default.LaptopLabel = laptopLabel;
            Properties.Settings.Default.Save();
        }
    }
}
