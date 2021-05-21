using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class ProcessMemoryModel
    {
        private string _pName;
        private int _pID;
        private long _pCommit;
        private long _pMem;

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

        public long PCommit
        {
            get { return _pCommit; }
            set
            {
                _pCommit = value;
            }
        }

        public long PMem
        {
            get { return _pMem; }
            set
            {
                _pMem = value;
            }
        }

    }
}
