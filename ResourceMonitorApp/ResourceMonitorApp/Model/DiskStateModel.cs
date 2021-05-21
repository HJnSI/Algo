using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class DiskStateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _DiskReadRate;
        private string _DiskWriteRate;

        public string DiskReadRate
        {
            get
            {
                return _DiskReadRate;
            }
            set
            {
                _DiskReadRate = value;
                OnPropertyChanged("DiskReadRate");
            }
        }

        public string DiskWriteRate
        {
            get
            {
                return _DiskWriteRate;
            }
            set
            {
                _DiskWriteRate = value;
                OnPropertyChanged("DiskWriteRate");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
