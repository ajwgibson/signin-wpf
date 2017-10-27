using KidsSignIn.Model;
using KidsSignIn.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace KidsSignIn.Pages
{
    class HomeViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HomeViewModel));

        #region Fields

        private string filterValue;
        private List<Child> children;
        private Child selectedChild;
        private Room selectedRoom;
        private PrintService printService = PrintService.Instance;
        private string dataFilename;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of children to display.
        /// </summary>
        public List<Child> Children
        {
            get 
            { 
                if (!string.IsNullOrEmpty(filterValue))
                {
                    var filter = filterValue.Trim().ToLower();
                    var parts = filter.Split(' ');
                    if (parts.Length == 2)
                    {
                        // Special case - assume user typed first name and last name
                        var first = parts[0];
                        var last = parts[1];

                        return (
                            from c in children
                            where
                                c.First.ToLower().Contains(first)
                                && c.Last.ToLower().Contains(last)
                            orderby c.Last, c.First
                            select c
                        ).ToList<Child>();
                    }
                    else
                    {
                        return (
                            from c in children
                            where 
                                c.First.ToLower().Contains(filter)
                                || c.Last.ToLower().Contains(filter)
                            orderby c.Last, c.First
                            select c
                        ).ToList<Child>();
                    }
                }

                return children == null ? null : children.OrderBy(c => c.Last).ToList<Child>(); 
            }
            set
            {
                children = value;
                NotifyChanges();
            } 
        }

        /// <summary>
        /// Gets or sets the current filter value.
        /// </summary>
        public string FilterValue 
        { 
            get { return filterValue; }
            set
            {
                if (filterValue == value) return;

                SelectedChild = null;

                filterValue = value;
                NotifyChanges();
            }
        }

        /// <summary>
        /// Gets or sets the selected child.
        /// </summary>
        public Child SelectedChild 
        {
            get { return selectedChild;  }
            set
            {
                if (selectedChild == value) return;
                selectedChild = value;

                Rooms.ToList<Room>().ForEach(delegate(Room room) { room.IsSelected = false; });
                SelectedRoom = null;

                if (selectedChild != null && !string.IsNullOrEmpty(selectedChild.Room))
                {
                    SelectedRoom = Rooms.FirstOrDefault(r => r.Title == selectedChild.Room);
                }

                NotifyChanges();
            } 
        }

        /// <summary>
        /// Gets the visibility of the selected child.
        /// </summary>
        public Visibility SelectedChildVisibility 
        {
            get 
            {
                if (selectedChild != null) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the visibility of the statistics panel.
        /// </summary>
        public Visibility StatisticsVisibility
        {
            get
            {
                if (selectedChild == null) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the full list of rooms.
        /// </summary>
        public Collection<Room> Rooms { get; private set; }

        /// <summary>
        /// Gets the list of available rooms.
        /// </summary>
        public List<Room> AvailableRooms
        {
            get { return Rooms.Where(r => r.IsAvailable).ToList(); }
        }

        /// <summary>
        /// Gets the selected room.
        /// </summary>
        public Room SelectedRoom
        {
            get { return selectedRoom; }
            set
            {
                if (selectedRoom == value) return;
                selectedRoom = value;
                NotifyChanges();
            }
        }

        /// <summary>
        /// Is the sign in button enabled?
        /// </summary>
        public Boolean SignInEnabled
        {
            get { return SelectedRoom != null; }
        }

        /// <summary>
        /// Is the sign in button visible?
        /// </summary>
        public Visibility SignInButtonVisibility
        {
            get
            {
                if (SelectedRoom == null) return Visibility.Visible;
                if (SelectedRoom != null && SelectedChild != null && SelectedChild.Room == null) return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Is the clear button visible?
        /// </summary>
        public Visibility ClearButtonVisibility
        {
            get
            {
                if (SelectedChild != null
                        && SelectedRoom != null
                        && SelectedRoom.Title == SelectedChild.Room)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Is the change button visible?
        /// </summary>
        public Visibility ChangeButtonVisibility
        {
            get
            {
                if (SelectedChild != null
                        && SelectedRoom != null
                        && SelectedChild.Room != null
                        && (SelectedRoom.Title != SelectedChild.Room))
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets a flag to indicate if the newcomer button should be enabled.
        /// </summary>
        public bool NewcomerButtonEnabled
        {
            get { return children != null && children.Count > 0; }
        }

        /// <summary>
        /// Gets or sets the data file name
        /// </summary>
        public string DataFilename
        {
            get { return dataFilename; }
            set
            {
                if (dataFilename == value) return;
                dataFilename = value;
                NotifyPropertyChanged("DataFilename");
            }
        }

        /// <summary>
        /// Gets the visibility of the update contact details warning.
        /// </summary>
        public Visibility UpdateContactDetailsVisibility
        {
            get
            {
                if (selectedChild != null && selectedChild.UpdateRequired) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the count of signed in children.
        /// </summary>
        public int SignInTotal
        {
            get
            {
                if (children == null) return 0;
                return children.Where(c => c.SignedInAt != null).Count();
            }
        }

        /// <summary>
        /// Gets the visibility of the statistics table.
        /// </summary>
        public Visibility StatisticsTableVisibility
        {
            get
            {
                if (SignInTotal > 0) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the breakdown of room counts.
        /// </summary>
        public Dictionary<string, int> Statistics
        {
            get
            {
                Dictionary<string, int> stats = new Dictionary<string, int>();
                if (children != null)
                {
                    stats = children
                                .Where(c => c.SignedInAt != null)
                                .OrderBy(c => c.Room).GroupBy(c => c.Room)
                                .ToDictionary(g => g.Key, g => g.Count());
                }
                return stats;
            }
        }

        /// <summary>
        /// Gets the visibility of the medical warning.
        /// </summary>
        public Visibility MedicalWarningVisibility
        {
            get
            {
                if (selectedChild != null && selectedChild.MedicalFlag) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        #endregion

        #region Constructor

        public HomeViewModel()
        {
            Rooms = new Collection<Room>();
            XmlSerializer xs = new XmlSerializer(typeof(Collection<Room>));
            using (var reader = new StreamReader(@"Rooms.xml"))
            {
                var rooms = (Collection<Room>)xs.Deserialize(reader);
                foreach (var room in rooms) Rooms.Add(room);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Normal sign in.
        /// </summary>
        public void SignIn()
        {
            var id = Properties.Settings.Default.NextId;
            var laptopLabel = Properties.Settings.Default.LaptopLabel;
            var label = string.Format("{0}{1:D2}", laptopLabel, id);

            Properties.Settings.Default.NextId += 1;
            Properties.Settings.Default.Save();

            logger.InfoFormat(
                "Signed in {0} to {1} with label {2}",
                selectedChild.Fullname,
                SelectedRoom.Title,
                label);

            selectedChild.Room = SelectedRoom.Title;
            selectedChild.RoomLabel = SelectedRoom.ShowOnLabel ? SelectedRoom.Title : "";
            selectedChild.SignedInAt = DateTime.Now;
            selectedChild.Label = label;

            printService.Print(selectedChild);

            SaveChanges();
            NotifyChanges();
        }

        /// <summary>
        /// Change a sign in record
        /// </summary>
        public void ChangeSignIn()
        {
            logger.InfoFormat(
                "Changed sign in details for {0} ({1}) from {2} to {3}",
                selectedChild.Fullname,
                selectedChild.Label,
                SelectedChild.Room,
                SelectedRoom.Title);

            selectedChild.Room = SelectedRoom.Title;
            selectedChild.RoomLabel = SelectedRoom.ShowOnLabel ? SelectedRoom.Title : "";
            selectedChild.SignedInAt = DateTime.Now;

            printService.Print(selectedChild);

            SaveChanges();
            NotifyChanges();
        }

        /// <summary>
        /// Clear a sign in record
        /// </summary>
        public void ClearSignIn()
        {
            logger.InfoFormat(
                "Cleared details for {0} ({1}), was signed into {2}",
                selectedChild.Fullname,
                selectedChild.Label,
                SelectedChild.Room);

            selectedChild.Room = null;
            selectedChild.RoomLabel = null;
            selectedChild.SignedInAt = null;
            selectedChild.Label = null;
            selectedRoom = null;

            SaveChanges();
            NotifyChanges();
        }

        /// <summary>
        /// Adds a new child to the list and selects them to allow immediate sign in
        /// </summary>
        public void AddNewcomer(string first, string last, bool medicalFlag)
        {
            children.Add(new Child
                {
                    First = first,
                    Last = last,
                    MedicalFlag = medicalFlag,
                    IsNewcomer = true,
                });

            new CsvService().SaveData(DataFilename, children);

            if (FilterValue == last) NotifyPropertyChanged("Children");
            else FilterValue = last;

            SelectedChild = children.FirstOrDefault(c => c.First == first && c.Last == last && c.IsNewcomer);
        }

        #endregion

        #region Private methods

        private void SaveChanges()
        {
            new CsvService().SaveData(DataFilename, children);
        }

        private void NotifyChanges()
        {
            NotifyPropertyChanged("SelectedChild");
            NotifyPropertyChanged("SelectedChildVisibility");
            NotifyPropertyChanged("StatisticsVisibility");
            NotifyPropertyChanged("UpdateContactDetailsVisibility");
            NotifyPropertyChanged("SelectedRoom");
            NotifyPropertyChanged("SignInEnabled");
            NotifyPropertyChanged("SignInButtonVisibility");
            NotifyPropertyChanged("ClearButtonVisibility");
            NotifyPropertyChanged("ChangeButtonVisibility");
            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("SignInTotal");
            NotifyPropertyChanged("Statistics");
            NotifyPropertyChanged("StatisticsTableVisibility");
            NotifyPropertyChanged("NewcomerButtonEnabled");
            NotifyPropertyChanged("FilterValue");
            NotifyPropertyChanged("MedicalWarningVisibility");
        }

        #endregion
    }
}
