using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class ProcessCPUModel
    {
        private string _pName;
        private int _pID;
        private int _pThread;
        private int _pCPU;

        public string PName
        {
            get { return _pName; }
            set
            {
                _pName = value;
            }
        }
        public int PID
        {
            get { return _pID; }
            set
            {
                _pID = value;
            }
        }

        public int PThread
        {
            get { return _pThread; }
            set
            {
                _pThread = value;
            }
        }

        public int PCPU
        {
            get { return _pCPU; }
            set
            {
                _pCPU = value;
            }
        }
    }

}
