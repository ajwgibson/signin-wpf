using LINQtoCSV;
using System;

namespace KidsSignIn.Model
{
    public class Child
    {
        [CsvColumn]
        public int Id { get; set; }

        [CsvColumn]
        public string First { get; set; }

        [CsvColumn]
        public string Last { get; set; }

        [CsvColumn]
        public string Room { get; set; }

        [CsvColumn]
        public DateTime? SignedInAt { get; set; }

        [CsvColumn]
        public string Label { get; set; }

        [CsvColumn]
        public bool IsNewcomer { get; set; }

        [CsvColumn]
        public bool UpdateRequired { get; set; }

        [CsvColumn]
        public bool MedicalFlag { get; set; }



        #region Properties not saved to or loaded from CSV

        public string Fullname 
        {
            get { return string.Format("{0} {1}", First, Last); }
        }

        public string RoomLabel { get; set; }

        #endregion
    }
}
