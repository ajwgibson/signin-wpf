using KidsSignIn.Extensions;
using KidsSignIn.Model;
using DYMO.Label.Framework;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KidsSignIn.Properties;

namespace KidsSignIn.Service
{
    public class PrintService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PrintService));

        private static PrintService service;

        private PrintService() 
        {
            Printer = Framework.GetLabelWriterPrinters().FirstOrDefault(p => p.IsConnected == true);
            Copies  = Properties.Settings.Default.NumberOfLabels;
            LabelFile = Settings.Default.LabelFile;
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
        public string LabelFile { get; private set; }
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

            LabelFile = labelFile;

            Printer = Printers.FirstOrDefault(p => p.Name == printer);
            Copies = copies;
            PrintForSunday = printForSunday;
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
            ILabel label = Framework.Open(LabelFile);

            if (Printer != null && label != null)
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

                label.SetObjectText("NAME", string.Format("{0}\r\n{1}", first, last));
                label.SetObjectText("ORGANISATION", Settings.Default.Organisation);
                label.SetObjectText("ENVIRONMENT", child.RoomLabel);
                label.SetObjectText("NUMBER", child.Label);

                var signedInAt = child.SignedInAt.HasValue ? child.SignedInAt.Value : DateTime.Now;
                if (PrintForSunday && !(signedInAt.DayOfWeek == DayOfWeek.Sunday)) signedInAt = signedInAt.Next(DayOfWeek.Sunday);

                label.SetObjectText("DATE", signedInAt.ToString("d MMMM yyyy"));

                if (child.IsNewcomer)
                {
                    label.SetObjectText("FLAGS", "** NEW **");
                }
                else
                {
                    label.SetObjectText("FLAGS", String.Empty);
                }

                if (!child.MedicalFlag)
                {
                    label.SetObjectText("MEDICAL", String.Empty);
                }

                label.Print(Printer, new LabelWriterPrintParams() { Copies = copies });
            }
        }

    }
}
