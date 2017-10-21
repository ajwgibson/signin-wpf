using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidsSignIn
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            var eventToFire = PropertyChanged;
            if (eventToFire != null)
                eventToFire(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void FireChangeEvents()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(""));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
