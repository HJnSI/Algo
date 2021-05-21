using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class NetworkStateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _NetworkSendRate;
        private string _NetworkReceiveRate;

        public string NetworkSendRate
        {
            get
            {
                return _NetworkSendRate;
            }
            set
            {
                _NetworkSendRate = value;
                OnPropertyChanged("NetworkSendRate");
            }
        }

        public string NetworkReceiveRate
        {
            get
            {
                return _NetworkReceiveRate;
            }
            set
            {
                _NetworkReceiveRate = value;
                OnPropertyChanged("NetworkReceiveRate");
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
