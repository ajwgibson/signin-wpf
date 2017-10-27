namespace KidsSignIn.Pages.Settings
{
    public class GeneralViewModel : ViewModelBase
    {
        private int    nextId       = Properties.Settings.Default.NextId;
        private string laptopLabel  = Properties.Settings.Default.LaptopLabel;
        private string organisation = Properties.Settings.Default.Organisation;

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

        public string Organisation
        {
            get { return organisation; }
            set { organisation = value; }
        }

        public void Refresh()
        {
            NextId       = Properties.Settings.Default.NextId;
            LaptopLabel  = Properties.Settings.Default.LaptopLabel;
            Organisation = Properties.Settings.Default.Organisation;
            NotifyPropertyChanged("NextId");
            NotifyPropertyChanged("LaptopLabel");
            NotifyPropertyChanged("Organisation");
        }

        public void Save()
        {
            Properties.Settings.Default.NextId       = nextId;
            Properties.Settings.Default.LaptopLabel  = laptopLabel;
            Properties.Settings.Default.Organisation = organisation;
            Properties.Settings.Default.Save();
        }
    }
}
