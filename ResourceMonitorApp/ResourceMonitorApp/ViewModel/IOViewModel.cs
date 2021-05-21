using ResourceMonitorApp.Model;
using ResourceMonitorApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ResourceMonitorApp.ViewModel
{
    public class IOViewModel : BaseViewModel
    {
        delegate void TimerEventFiredDelegate();

        #region 변수 및 프로퍼티 정의
        private Timer _timer;
        private double _diskReadRate, _diskWriteRate, _networkSendRate, _networkReceiveRate;

        private double _maxDiskRate;
        public double MaxDiskRate
        {
            get { return _maxDiskRate; }
            set
            {
                _maxDiskRate = value;
                OnPropertyChanged("MaxDiskRate");
            }
        }

        private double _maxNetworkRate;
        public double MaxNetworkRate
        {
            get { return _maxNetworkRate; }
            set
            {
                _maxNetworkRate = value;
                OnPropertyChanged("MaxNetworkRate");
            }
        }


        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        private PerformanceHelper _pHelper;
        public PerformanceHelper PHelper
        {
            get { return _pHelper; }
            set { _pHelper = value; }
        }
        private GraphHelper _gHelper;

        public GraphHelper GHelper
        {
            get { return _gHelper; }
            set { _gHelper = value; }
        }

        private DiskStateModel _diskState;
        public DiskStateModel DiskState
        {
            get { return _diskState; }
            set
            {
                _diskState = value;
            }
        }

        private NetworkStateModel _networkState;
        public NetworkStateModel NetworkState
        {
            get { return _networkState; }
            set
            {
                _networkState = value;
            }
        }

        private ObservableCollection<ProcessIOModel> _IOList;
        public ObservableCollection<ProcessIOModel> IOList
        {
            get { return _IOList; }
            set
            {
                _IOList = value;
                OnPropertyChanged("IOList");
            }
        }


        private ProcessCPUModel _selectedProcess;
        public ProcessCPUModel SelectedProcess
        {
            get
            {
                return _selectedProcess;
            }
            set
            {
                _selectedProcess = value;
                OnPropertyChanged("SelectedProcess");
            }
        }

        #endregion

        #region 생성자
        public IOViewModel()
        {
            PHelper = new PerformanceHelper();
            GHelper = new GraphHelper();
            DiskState = new DiskStateModel();
            NetworkState = new NetworkStateModel();
            IOList = new ObservableCollection<ProcessIOModel>();

            SearchText = "";
            MaxDiskRate = 500;
            MaxNetworkRate = 100;
            GHelper.SettingGraph();

            _timer = new Timer(CallBack);
            _timer.Change(0, 1000);

        }
        #endregion

        #region 메소드
        async void CallBack(Object state)
        {
            await Task.Run(() =>
            {
                GetData();

                // UI Thread 핸들링
                Application.Current.Dispatcher.BeginInvoke(new TimerEventFiredDelegate(DrawData));
            });
        }

        private void GetData()
        {
            IOList = PHelper.GetIOList(SearchText);
            _diskReadRate = PHelper.GetDiskReadRate();
            _diskWriteRate = PHelper.GetDiskWriteRate();
            _networkSendRate = PHelper.GetNetworkSendRate();
            _networkReceiveRate = PHelper.GetNetworkReceiveRate();
        }

        private void DrawData()
        {
            DrawDiskGraph();
            DrawNetworkGraph();
            DrawDetails();
        }

        private void DrawDiskGraph()
        {
            var now = DateTime.Now;
            GHelper.DiskReadRate.Add(new UsagePlotModel
            {
                DateTime = now,
                Value = _diskReadRate
            });

            GHelper.DiskWriteRate.Add(new UsagePlotModel
            {
                DateTime = now,
                Value = _diskWriteRate
            });

            if (MaxDiskRate < _diskReadRate || MaxDiskRate < _diskWriteRate)
            {   
                MaxDiskRate = GetMaxY(_diskReadRate, _diskWriteRate);
            }

            GHelper.SetAxisLimits(now);

            bool flag = false;
            if (GHelper.DiskReadRate.Count > 60)
            {
                if (GHelper.DiskReadRate.First().Value == MaxDiskRate || GHelper.DiskWriteRate.First().Value == MaxDiskRate)
                {
                    flag = true;
                }

                GHelper.DiskReadRate.RemoveAt(0);
                GHelper.DiskWriteRate.RemoveAt(0);
                if (flag)
                {
                    MaxDiskRate = GetMaxY(GHelper.DiskReadRate.Max(i => i.Value), GHelper.DiskWriteRate.Max(i => i.Value));
                }
            }
        }

        private void DrawNetworkGraph()
        {
            var now = DateTime.Now;
            GHelper.NetworkSendRate.Add(new UsagePlotModel
            {
                DateTime = now,
                Value = _networkSendRate
            });

            GHelper.NetworkReceiveRate.Add(new UsagePlotModel
            {
                DateTime = now,
                Value = _networkReceiveRate
            });

            if (MaxNetworkRate < _networkSendRate || MaxNetworkRate < _networkReceiveRate)
            {
                MaxNetworkRate = GetMaxY(_networkSendRate, _networkReceiveRate);
            }

            GHelper.SetAxisLimits(now);

            bool flag = false;
            if (GHelper.NetworkSendRate.Count > 60)
            {
                if (GHelper.NetworkSendRate.First().Value == MaxNetworkRate || GHelper.NetworkReceiveRate.First().Value == MaxNetworkRate)
                {
                    flag = true;
                }

                GHelper.NetworkSendRate.RemoveAt(0);
                GHelper.NetworkReceiveRate.RemoveAt(0);
                if (flag)
                {
                    MaxNetworkRate = GetMaxY(GHelper.NetworkSendRate.Max(i => i.Value), GHelper.NetworkReceiveRate.Max(i => i.Value));
                }
            }
        }

        private void DrawDetails()
        {
            DiskState.DiskReadRate = _diskReadRate.ToString() + "KB/s";
            DiskState.DiskWriteRate = _diskWriteRate.ToString() + "KB/s";

            NetworkState.NetworkSendRate = _networkSendRate.ToString() + "Kbps";
            NetworkState.NetworkReceiveRate = _networkReceiveRate.ToString() + "Kbps";
        }

        private double GetMaxY(double left, double right)
        {
            return left > right ? left : right;
        }
        #endregion
    }
}
