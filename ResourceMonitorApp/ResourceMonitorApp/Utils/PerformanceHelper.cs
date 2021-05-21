using ResourceMonitorApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ResourceMonitorApp.Utils
{
    public class PerformanceHelper
    {
        #region 변수 및 프로퍼티 정의
        private ManagementObjectSearcher _oWMI;
        private PerformanceCounter _cpuUsageCounter;
        private PerformanceCounter _cpuRunningTimeCounter;
        private PerformanceCounter _countCounter;
        private PerformanceCounter _memoryCounter;
        private PerformanceCounter _diskReadCounter, _diskWriteCounter;
        private PerformanceCounter _networkSendCounter;
        private PerformanceCounter _networkReceiveCounter;

        #endregion

        public PerformanceHelper()
        {
            _cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuRunningTimeCounter = new PerformanceCounter("System", "System Up Time");
            _diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
            _diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
            _networkSendCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", GetCurrentNetwork());
            _networkReceiveCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", GetCurrentNetwork());
        }

        #region 메소드

        #region CPU
        public double GetCurrentCpuUsage()
        {
            return Math.Round(_cpuUsageCounter.NextValue());
        }

        public int GetProcessCount()
        {
            // System Interrupt, Idle, _Total 제외한 개수 리턴
            return Process.GetProcesses().Count() - 3;
        }

        public int GetThreadCount()
        {
            _countCounter = new PerformanceCounter("System", "Threads");

            return (int)_countCounter.NextValue();
        }

        public int GetHandleCount()
        {
            _countCounter = new PerformanceCounter("Process", "Handle Count", "_Total");

            return (int)_countCounter.NextValue();
        }

        public double GetCurrentCpuRunningime()
        {
            return _cpuRunningTimeCounter.NextValue();
        }

        public ObservableCollection<ProcessCPUModel> GetCPUList(string searchText)
        {
            ObservableCollection<ProcessCPUModel> CPUList = new ObservableCollection<ProcessCPUModel>();
            string wmiStr = string.Format("select * from Win32_PerfFormattedData_PerfProc_Process where Name != '_Total' and Name != 'Idle' and Name LIKE '%{0}%'", searchText);

            _oWMI = new ManagementObjectSearcher(new SelectQuery(wmiStr));

            try
            {
                foreach (ManagementObject oItem in _oWMI.Get())
                {
                    CPUList.Add(new ProcessCPUModel()
                    {
                        PThread = Convert.ToInt32(oItem.GetPropertyValue("ThreadCount")),
                        PCPU = Convert.ToInt32(oItem.GetPropertyValue("PercentProcessorTime")),
                        PName = oItem.GetPropertyValue("Name").ToString(),
                        PID = Convert.ToInt32(oItem.GetPropertyValue("IDProcess"))
                    });
                }
            }
            catch (Exception) { }

            return new ObservableCollection<ProcessCPUModel>(CPUList.OrderByDescending(i => i.PCPU));
        }

        #endregion

        #region 메모리
        public double GetCurrentAvailableMemory()
        {
            _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            return Math.Round(_memoryCounter.NextValue() / 1024, 1);
        }

        public double GetTotalMemory()
        {
            double totalMemory = 0;

            _oWMI = new ManagementObjectSearcher(new SelectQuery("select * from Win32_OperatingSystem"));

            foreach (ManagementObject o in _oWMI.Get())
            {
                totalMemory = Math.Round(Convert.ToDouble(o.GetPropertyValue("TotalVisibleMemorySize")) / (1024 * 1024), 1);
            }

            return totalMemory;
        }

        // GB
        public double GetCommittedMemory()
        {
            _memoryCounter = new PerformanceCounter("Memory", "Committed Bytes");

            return Math.Round(_memoryCounter.NextValue() / (1024 * 1024 * 1024), 1);
        }

        // GB
        public double GetCommitLimitMemory()
        {
            _memoryCounter = new PerformanceCounter("Memory", "Commit Limit");

            return Math.Round(_memoryCounter.NextValue() / (1024 * 1024 * 1024), 1);
        }

        // ?
        public double GetCachedMemory()
        {
            _memoryCounter = new PerformanceCounter("Memory", "Cache Bytes");
            return 0;
            //            return Math.Round(_memoryCounter.NextValue() / (1024 * 1024), 1);
        }

        // MB
        public double GetMemoryPagingPool()
        {
            _memoryCounter = new PerformanceCounter("Memory", "Pool Paged Bytes");

            return Math.Round(_memoryCounter.NextValue() / (1024 * 1024), 1);
        }

        // MB
        public double GetMemoryNonPagingPool()
        {
            _memoryCounter = new PerformanceCounter("Memory", "Pool NonPaged Bytes");

            return Math.Round(_memoryCounter.NextValue() / (1024 * 1024), 1);
        }

        public ObservableCollection<ProcessMemoryModel> GetMemoryList(string searchText)
        {
            ObservableCollection<ProcessMemoryModel> MemoryList = new ObservableCollection<ProcessMemoryModel>();

            try
            {
                Process[] pList = Process.GetProcesses();

                foreach (Process p in pList)
                {
                    MemoryList.Add(new ProcessMemoryModel()
                    {
                        PName = p.ProcessName,
                        PID = p.Id,
                        PCommit = p.PrivateMemorySize64 / 1024,
                        PMem = p.WorkingSet64 / 1024
                    });
                }
            }
            catch (Exception) { }

            return new ObservableCollection<ProcessMemoryModel>(MemoryList.Where(i => i.PName.Contains(searchText)).OrderBy(i => i.PName));
        }

        #endregion

        #region Disk & Network

        public double GetDiskReadRate()
        {
            return Math.Round(_diskReadCounter.NextValue() / 1000, 1);
        }

        public double GetDiskWriteRate()
        {
            return Math.Round(_diskWriteCounter.NextValue() / 1000, 1);
        }

        public string GetCurrentNetwork()
        {
            NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();
            var activeAdapter = networks.First(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback
                                && x.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                                && x.OperationalStatus == OperationalStatus.Up
                                && x.Name.StartsWith("vEthernet") == false);

            return activeAdapter.Description.Replace("(", "[").Replace(")", "]");
        }

        public double GetNetworkSendRate()
        {
            return Math.Round(Convert.ToDouble(_networkSendCounter.NextValue().ToString()) / 125);
        }

        public double GetNetworkReceiveRate()
        {
            return Math.Round(Convert.ToDouble(_networkReceiveCounter.NextValue().ToString()) / 125);
        }

        public ObservableCollection<ProcessIOModel> GetIOList(string searchText)
        {
            ObservableCollection<ProcessIOModel> IOList = new ObservableCollection<ProcessIOModel>();
            string wmiStr = string.Format("select * from Win32_PerfFormattedData_PerfProc_Process where Name != '_Total' and Name != 'Idle' and Name LIKE '%{0}%'", searchText);

            _oWMI = new ManagementObjectSearcher(new SelectQuery(wmiStr));

            try
            {
                foreach (ManagementObject oItem in _oWMI.Get())
                {
                    if ((oItem.GetPropertyValue("IOReadBytesPerSec").ToString() != "0") || (oItem.GetPropertyValue("IOWriteBytesPerSec").ToString() != "0")) {
                        IOList.Add(new ProcessIOModel()
                        {
                            PName = oItem.GetPropertyValue("Name").ToString(),
                            PID = Convert.ToInt32(oItem.GetPropertyValue("IDProcess")),
                            PReadSpeed = Math.Round(Convert.ToDouble(oItem.GetPropertyValue("IOReadBytesPerSec")) / 1000, 2),
                            PWriteSpeed = Math.Round(Convert.ToDouble(oItem.GetPropertyValue("IOWriteBytesPerSec")) / 1000, 2)
                        });
                    }
                }

            }
            catch (Exception) { }

            return new ObservableCollection<ProcessIOModel>(IOList.OrderBy(i => i.PName));
        }


        public ObservableCollection<PCInfoModel> GetPCInfoList(string param)
        {
            ObservableCollection<PCInfoModel> InfoList = new ObservableCollection<PCInfoModel>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(InfoList);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("PcName");
            view.GroupDescriptions.Add(groupDescription);

            _oWMI = new ManagementObjectSearcher(new SelectQuery(param));

            try
            {
                foreach (ManagementObject oItem in _oWMI.Get())
                {
                    foreach (PropertyData pd in oItem.Properties)
                    {
                        InfoList.Add(new PCInfoModel()
                        {
                            PcName = oItem.GetPropertyValue("Caption") as string,
                            PcProperty = pd.Name,
                            PcDescription = pd.Value == null ? pd.Value as string : pd.Value.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return InfoList;
        }
        #endregion

        #endregion
    }
}