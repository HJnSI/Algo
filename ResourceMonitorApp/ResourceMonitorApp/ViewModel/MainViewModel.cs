using ResourceMonitorApp.Command;
using ResourceMonitorApp.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ResourceMonitorApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand MinCommand { get; set; }
        public ICommand MaxCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ICommand PauseTimerCommand { get; set; }

        private WindowState _curWindowState;
        public WindowState CurWindowState
        {
            get{ return _curWindowState;}
            set{
                _curWindowState = value;
                OnPropertyChanged("CurWindowState");
            }
        }

        public MainViewModel()
        {
            MinCommand = new RelayCommand<Object>(MinWindow);
            MaxCommand = new RelayCommand<Object>(MaxWindow);
            CloseCommand = new RelayCommand<Window>(CloseWindow);

            PauseTimerCommand = new RelayCommand<Object>(PauseTimer);
        }
        private void PauseTimer(Object obj)
        {

        }

        private void MinWindow(Object obj)
        {
            CurWindowState = WindowState.Minimized;
        }

        private void MaxWindow(Object obj)
        {
            if(CurWindowState == WindowState.Maximized)
            {
                CurWindowState = WindowState.Normal;
            }
            else
            {
                CurWindowState = WindowState.Maximized;
            }
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

    }
}
