using KidsSignIn.Service;
using log4net;

namespace KidsSignIn.Pages.Settings
{
    public class PrintingViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PrintingViewModel));

        private string labelFile = Properties.Settings.Default.LabelFile;
        private int    copies    = Properties.Settings.Default.NumberOfLabels;

        #region Properties

        public int Copies
        {
            get { return copies; }
            set
            {
                if (copies == value) return;
                copies = value;
            }
        }

        public string LabelFile 
        {
            get { return labelFile; }
            set
            {
                if (labelFile == value) return;
                labelFile = value;
                NotifyPropertyChanged("LabelFile");
            }
        }

        public string Printer { get; set; }

        public bool CanPrint 
        {
            get { return !(PrintService.Printers == null || PrintService.Printers.Count == 0); }
        }

        public bool PrintSundayDate { get; set; }

        #endregion

        #region Constructors

        public PrintingViewModel()
        {
            var printers = PrintService.Printers;
            if (printers != null && printers.Count > 0) Printer = printers[0].Name;
            else Printer = "No active Dymo LabelPrinter found";
        }

        #endregion

        #region Public methods

        public void Save()
        {
            Properties.Settings.Default.NumberOfLabels = copies;
            Properties.Settings.Default.LabelFile      = labelFile;
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}
