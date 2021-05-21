using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using ResourceMonitorApp.Model;
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
using ResourceMonitorApp.Utils;
using System.Windows.Data;
using System.Windows.Input;
using ResourceMonitorApp.Command;
using System.IO;
using System.Data;

namespace ResourceMonitorApp.ViewModel
{
    public class CPUViewModel : BaseViewModel
    {
        delegate void TimerEventFiredDelegate();

        #region 변수 및 프로퍼티 정의
        public ICommand LogCommand { get; set; }

        private double _cpuUsage = 0;
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

        private CPUStateModel _cpuState;
        public CPUStateModel CPUState
        {
            get { return _cpuState; }
            set
            {
                _cpuState = value;
            }
        }

        private ObservableCollection<ProcessCPUModel> _cpuList;
        public ObservableCollection<ProcessCPUModel> CPUList
        {
            get { return _cpuList; }
            set
            {
                _cpuList = value;
                OnPropertyChanged("CPUList");
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
        public CPUViewModel()
        {
            PHelper = new PerformanceHelper();
            GHelper = new GraphHelper();
            CPUState = new CPUStateModel();
            CPUList = new ObservableCollection<ProcessCPUModel>();

            LogCommand = new RelayCommand<Object>(LogWrite);

            GHelper.SettingGraph();

            _timer = new Timer(CallBack);
            _timer.Change(0, 1000);

        }
        #endregion

        #region 메소드
        private void LogWrite(Object obj)
        {
            string curTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string DirPath = Environment.CurrentDirectory + @"\Log";
            string FilePath = DirPath + "\\Log_CPU_" + curTime + ".log";

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);

                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        sw.WriteLine(CPUState.ToString().Replace("\n", string.Format("\n[{0}] ", curTime)));
                        DataTable dt = new DataTable();
                        dt.Columns.AddRange(new DataColumn[]
                        {
                            new DataColumn { ColumnName = "Name", Caption = "이름", DataType = typeof(string) },
                            new DataColumn { ColumnName = "PID", Caption = "프로세스ID", DataType = typeof(int) },
                            new DataColumn { ColumnName = "Thread", Caption = "스레드", DataType = typeof(int) },
                            new DataColumn { ColumnName = "CPU", Caption = "CPU", DataType = typeof(int) }
                        });

                        foreach (ProcessCPUModel pcm in CPUList)
                        {
                            dt.Rows.Add(new object[] { pcm.PName, pcm.PID, pcm.PThread, pcm.PCPU });
                        }
                        sw.WriteLine(string.Join(Environment.NewLine,dt.Rows.OfType<DataRow>().Select(x => string.Join("\t\t", x.ItemArray))));

                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        sw.WriteLine(CPUState.ToString().Replace("\n", string.Format("\n[{0}] ", curTime)));
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            };
        }

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
            CPUList = PHelper.GetCPUList(SearchText);
            _cpuUsage = PHelper.GetCurrentCpuUsage();
        }

        private void DrawData()
        {
            DrawCPUGraph();
            DrawCPUDetails();
        }

        private void DrawCPUGraph()
        {
            var now = DateTime.Now;
            GHelper.ChartValues.Add(new UsagePlotModel
            {
                DateTime = now,
                Value = _cpuUsage
            });

            GHelper.SetAxisLimits(now);

            if (GHelper.ChartValues.Count > 60) GHelper.ChartValues.RemoveAt(0);
        }

        private void DrawCPUDetails()
        {
            CPUState.CPUUsage = _cpuUsage + "%";
            CPUState.ProcessCount = PHelper.GetProcessCount().ToString();
            CPUState.ThreadCount = PHelper.GetThreadCount().ToString();
            CPUState.HandleCount = PHelper.GetHandleCount().ToString();
            TimeSpan ts = TimeSpan.FromSeconds(PHelper.GetCurrentCpuRunningime());
            CPUState.CPURunningTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", (int)ts.TotalDays, (int)ts.Hours, (int)ts.Minutes, (int)ts.Seconds);
        }
        #endregion
    }

}
