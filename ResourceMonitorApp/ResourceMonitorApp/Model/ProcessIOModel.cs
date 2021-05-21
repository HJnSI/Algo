using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class ProcessIOModel
    {

        private string _pName;
        private int _pID;
        private double _pReadSpeed;
        private double _pWriteSpeed;

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

        public double PReadSpeed
        {
            get { return _pReadSpeed; }
            set
            {
                _pReadSpeed = value;
            }
        }

        public double PWriteSpeed
        {
            get { return _pWriteSpeed; }
            set
            {
                _pWriteSpeed = value;
            }
        }

    }
}
