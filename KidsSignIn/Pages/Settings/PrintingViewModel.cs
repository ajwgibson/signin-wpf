using KidsSignIn.Service;
using FirstFloor.ModernUI.Presentation;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KidsSignIn.Pages.Settings
{
    public class PrintingViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PrintingViewModel));

        private string labelFile;

        #region Properties

        public int Copies { get; set; }

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
            Copies = 3;
            LabelFile = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + Path.DirectorySeparatorChar + @"Labels\VineyardStars.label";
            var printers = PrintService.Printers;
            if (printers != null && printers.Count > 0) Printer = printers[0].Name;
            else Printer = "No active Dymo LabelPrinter found";
            PrintSundayDate = false;
        }

        #endregion
    }
}
