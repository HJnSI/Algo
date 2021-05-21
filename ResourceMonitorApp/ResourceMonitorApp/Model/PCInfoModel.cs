using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Model
{
    public class PCInfoModel
    {
        private string _pcName;
        private string _pcProperty;
        private string _pcDescription;

        public string PcName
        {
            get { return _pcName; }
            set
            {
                _pcName = value;
            }
        }
        public string PcProperty
        {
            get { return _pcProperty; }
            set
            {
                _pcProperty = value;
            }
        }
        public string PcDescription
        {
            get { return _pcDescription; }
            set
            {
                _pcDescription = value;
            }
        }
    }
}
