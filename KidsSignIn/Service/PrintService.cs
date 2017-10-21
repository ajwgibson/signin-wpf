using KidsSignIn.Extensions;
using KidsSignIn.Model;
using DYMO.Label.Framework;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KidsSignIn.Service
{
    public class PrintService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PrintService));

        private static PrintService service;

        private PrintService() 
        {
            Printer = Framework.GetLabelWriterPrinters().FirstOrDefault(p => p.IsConnected == true);
            Label = Framework.Open(@"Labels\VineyardStars.label");
            Copies = 3;
        }

        public static PrintService Instance
        {
            get
            {
                if (service == null) service = new PrintService();
                return service;
            }
        }

        public IPrinter Printer { get; private set; }
        public ILabel Label { get; private set; }
        public int Copies { get; private set; }
        public bool PrintForSunday { get; private set; }

        public static List<IPrinter> Printers
        {
            get
            {
                return Framework.GetLabelWriterPrinters()
                    .Where(p => p.IsConnected == true)
                    .OrderBy(p => p.Name)
                    .ToList();
            }
        }

        public void Configure(string printer, string labelFile, int copies, bool printForSunday)
        {
            logger.DebugFormat("Configure: Printer={0}, Copies={1}, Label={2}", printer, copies, labelFile);

            Printer = Printers.FirstOrDefault(p => p.Name == printer);
            Label = Framework.Open(labelFile);
            Copies = copies;
            PrintForSunday = printForSunday;
        }

        public bool PrintingAvailable 
        {
            get { return Printer != null && Label != null; }
        }

        public void Print(Child child)
        {
            DoPrint(child, Copies);
        }

        public void Print(Child child, int copies = 1)
        {
            DoPrint(child, copies);
        }

        private void DoPrint(Child child, int copies)
        {
            if (PrintingAvailable)
            {
                var regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
                var first = Regex.Replace(child.First, regex, "").Trim();
                var last  = Regex.Replace(child.Last, regex, "").Trim();

                logger.DebugFormat(
                    "Printing {0} labels for {1} [{2}] using printer {3}",
                    Copies,
                    child.Fullname,
                    child.Label,
                    Printer.Name);

                Label.SetObjectText("NAME", string.Format("{0}\r\n{1}", first, last));
                Label.SetObjectText("ENVIRONMENT", child.RoomLabel);
                Label.SetObjectText("NUMBER", child.Label);

                var signedInAt = child.SignedInAt.HasValue ? child.SignedInAt.Value : DateTime.Now;
                if (PrintForSunday && !(signedInAt.DayOfWeek == DayOfWeek.Sunday)) signedInAt = signedInAt.Next(DayOfWeek.Sunday);

                Label.SetObjectText("DATE", signedInAt.ToString("d MMMM yyyy"));

                if (child.IsNewcomer)
                {
                    Label.SetObjectText("FLAGS", "** NEW **");
                }
                else
                {
                    Label.SetObjectText("FLAGS", String.Empty);
                }
                
                Label.Print(Printer, new LabelWriterPrintParams() { Copies = copies });
            }
        }

    }
}
