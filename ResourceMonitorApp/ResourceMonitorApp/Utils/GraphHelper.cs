using LiveCharts;
using LiveCharts.Configurations;
using ResourceMonitorApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitorApp.Utils
{
    public class GraphHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 프로퍼티
        private double _axisMax;
        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }

        private double _axisMin;
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public ChartValues<UsagePlotModel> ChartValues { get; set; }
        public ChartValues<UsagePlotModel> DiskReadRate { get; set; }
        public ChartValues<UsagePlotModel> DiskWriteRate { get; set; }
        public ChartValues<UsagePlotModel> NetworkSendRate { get; set; }
        public ChartValues<UsagePlotModel> NetworkReceiveRate { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        #endregion

        #region 메소드

        public void SettingGraph()
        {
            var mapper = Mappers.Xy<UsagePlotModel>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);
                        Charting.For<UsagePlotModel>(mapper);

            ChartValues = new ChartValues<UsagePlotModel>();
            DiskReadRate = new ChartValues<UsagePlotModel>();
            DiskWriteRate = new ChartValues<UsagePlotModel>();
            NetworkSendRate = new ChartValues<UsagePlotModel>();
            NetworkReceiveRate = new ChartValues<UsagePlotModel>();

            DateTimeFormatter = value => new DateTime((long)value).ToString("ss");

            AxisStep = TimeSpan.FromSeconds(60).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);

        }

        public void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(0).Ticks;
            AxisMin = now.Ticks - TimeSpan.FromSeconds(60).Ticks;
        }

        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
