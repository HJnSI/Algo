using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class CPUStateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _CPUUsage;
        private string _ProcessCount;
        private string _ThreadCount;
        private string _HandleCount;
        private string _CPURunddingTime;

        public string CPUUsage
        {
            get { return _CPUUsage; }
            set
            {
                _CPUUsage = value;
                OnPropertyChanged("CPUUsage");
            }
        }

        public string ProcessCount
        {
            get { return _ProcessCount; }
            set
            {
                _ProcessCount = value;
                OnPropertyChanged("ProcessCount");
            }
        }

        public string ThreadCount
        {
            get { return _ThreadCount; }
            set
            {
                _ThreadCount = value;
                OnPropertyChanged("ThreadCount");
            }
        }

        public string HandleCount
        {
            get { return _HandleCount; }
            set
            {
                _HandleCount = value;
                OnPropertyChanged("HandleCount");
            }
        }
        public string CPURunningTime
        {
            get { return _CPURunddingTime; }
            set
            {
                _CPURunddingTime = value;
                OnPropertyChanged("CPURunningTime");
            }
        }

        public CPUStateModel()
        {
            CPUUsage = "0%";
            ProcessCount = "0";
            ThreadCount = "0";
            HandleCount = "0";
            CPURunningTime = "00:00:00:00";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            string res = string.Format("\n########## CPU Information ##########\nCPU Usage : {0} \nProcess Count : {1} \nThread Count : {2} \nHandle Count : {3} \nRunning Time : {4}", CPUUsage, ProcessCount, ThreadCount, HandleCount, CPURunningTime);
            return res;
        }
    }
}
