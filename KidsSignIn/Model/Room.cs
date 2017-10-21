using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KidsSignIn.Model
{
    public class Room 
    {
        public string Colour { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public bool ShowOnLabel { get; set; }

        [XmlIgnore]
        public bool IsSelected { get; set; }

        [XmlIgnore]
        public string DisplayName 
        { 
            get 
            {
                return string.Format("{0}, {1} Room", Title, Colour);
            }
        }
    }
}
