using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ResourceMonitorApp.Model;
using ResourceMonitorApp.Utils;

namespace ResourceMonitorApp.ViewModel
{
    class PCInfoViewModel : BaseViewModel
    {
        #region 변수 및 프로퍼티

        private PerformanceHelper _pHelper;
        private ObservableCollection<PCInfoModel> _infolist;

        public PerformanceHelper PHelper
        {
            get
            {
                return _pHelper;
            }
            set
            {
                _pHelper = value;
            }
        }

        public ObservableCollection<PCInfoModel> InfoList
        {
            get { return _infolist; }
            set
            {
                _infolist = value;
                OnPropertyChanged("InfoList");
            }
        }

        private ObservableCollection<string> _comboItems;
        public ObservableCollection<string> ComboItems
        {
            get { return _comboItems; }
            set
            {
                _comboItems = value;
            }
        }

        private string _selectedItem;

        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                UpdateListView("Win32_" + value);
                //OnPropertyChanged("SelectedItem");
            }
        }
        #endregion

        #region 생성자
        public PCInfoViewModel()
        {
            PHelper = new PerformanceHelper();
            this.InfoList = new ObservableCollection<PCInfoModel>();
            this.ComboItems = new ObservableCollection<string> {
                "OperatingSystem", "DiskDrive", "LogicalDisk","Processor", "DesktopMonitor", "MemoryDevice","PhysicalMemory", "ComputerSystem"};
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(InfoList);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("PcName");
            view.GroupDescriptions.Add(groupDescription);
            SelectedItem = "OperatingSystem";
        }
        #endregion

        #region 메소드 구현

        public void UpdateListView(string value)
        {
            InfoList = PHelper.GetPCInfoList(value);
        }

        #endregion
    }
}
