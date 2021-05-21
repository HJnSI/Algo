using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class MemoryStateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _memoryUsage;
        private string _memoryAvailable;
        private string _memoryCommitted;
        private string _memoryCached;
        private string _memoryPagingPool;
        private string _memoryNonPagingPool;

        public string MemoryUsage
        {
            get { return _memoryUsage; }
            set
            {
                _memoryUsage = value;
                OnPropertyChanged("MemoryUsage");
            }
        }

        public string MemoryAvailable
        {
            get { return _memoryAvailable; }
            set
            {
                _memoryAvailable = value;
                OnPropertyChanged("MemoryAvailable");
            }
        }

        public string MemoryCommitted
        {
            get
            {
                return _memoryCommitted;
            }
            set
            {
                _memoryCommitted = value;
                OnPropertyChanged("MemoryCommitted");
            }
        }

        public string MemoryCached
        {
            get
            {
                return _memoryCached;
            }
            set
            {
                _memoryCached = value;
                OnPropertyChanged("MemoryCached");
            }
        }

        public string MemoryPagingPool
        {
            get
            {
                return _memoryPagingPool;
            }
            set
            {
                _memoryPagingPool = value;
                OnPropertyChanged("MemoryPagingPool");
            }
        }
        public string MemoryNonPagingPool
        {
            get
            {
                return _memoryNonPagingPool;
            }
            set
            {
                _memoryNonPagingPool = value;
                OnPropertyChanged("MemoryNonPagingPool");
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