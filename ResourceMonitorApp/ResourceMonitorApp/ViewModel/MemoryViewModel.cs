using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Configurations;
using ResourceMonitorApp.Model;
using ResourceMonitorApp.Utils;

namespace ResourceMonitorApp.ViewModel
{
    public class MemoryViewModel : BaseViewModel
    {
        delegate void TimerEventFiredDelegate();

        #region 변수 및 프로퍼티 정의
        private double _maxMemory;
        private double _memoryUsage;
        private Timer _timer;
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

        private MemoryStateModel _memoryState;
        public MemoryStateModel MemoryState
        {
            get { return _memoryState; }
            set
            {
                _memoryState = value;
            }
        }

        public double MaxMemory
        {
            get { return _maxMemory; }
            set
            {
                _maxMemory = value;
                OnPropertyChanged("MaxMemory");
            }
        }

        private ObservableCollection<ProcessMemoryModel> _memoryList;

        public ObservableCollection<ProcessMemoryModel> MemoryList
        {
            get { return _memoryList; }
            set
            {
                _memoryList = value;
                OnPropertyChanged("MemoryList");
            }
        }

        #endregion

        #region 생성자
        public MemoryViewModel()
        {
            PHelper = new PerformanceHelper();
            GHelper = new GraphHelper();
            MemoryState = new MemoryStateModel();
            MemoryList = new ObservableCollection<ProcessMemoryModel>();
            SearchText = "";

            GHelper.SettingGraph();
            MaxMemory = Math.Round(PHelper.GetTotalMemory());

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
            MemoryList = PHelper.GetMemoryList(SearchText);

            _memoryUsage = _maxMemory - PHelper.GetCurrentAvailableMemory();
        }

        private void DrawData()
        {
            DrawMemoryGraph();
            DrawMemoryDetails();
        }

        private void DrawMemoryGraph()
        {
            var now = DateTime.Now;

            GHelper.ChartValues.Add(new UsagePlotModel
            {
                DateTime = now,
                Value = _memoryUsage
            });

            GHelper.SetAxisLimits(now);

            if (GHelper.ChartValues.Count > 60) GHelper.ChartValues.RemoveAt(0);
        }

        private void DrawMemoryDetails()
        {
            MemoryState.MemoryUsage = _memoryUsage.ToString() + "GB";
            MemoryState.MemoryAvailable = PHelper.GetCurrentAvailableMemory().ToString() + "GB";
            MemoryState.MemoryCommitted = PHelper.GetCommittedMemory().ToString() + "/" + PHelper.GetCommitLimitMemory().ToString() + "GB";
            MemoryState.MemoryCached = PHelper.GetCachedMemory() + "GB";
            MemoryState.MemoryPagingPool = PHelper.GetMemoryPagingPool().ToString() + "MB";
            MemoryState.MemoryNonPagingPool = PHelper.GetMemoryNonPagingPool().ToString() + "MB";
        }
        #endregion
    }
}