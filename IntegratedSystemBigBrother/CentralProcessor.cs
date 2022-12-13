﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace IntegratedSystemBigBrother
{
    public class CentralProcessor :
        ICameraDataPackageVisitor,
        INotifyPropertyChanged
    {
        private const string NONSELECTED_CAMERA_STR = "<не выбрано>";

        private ObservableDictionary<string, PeripheralProcessor> _network;
        public ObservableDictionary<string, PeripheralProcessor> Network
        {
            get { return _network; }
            set
            {
                //if (_network != null && !value.SequenceEqual(_network))
                    _network = value;
                OnPropertyChanged(nameof(Network));
            }
        }

        public bool IsInObservingMode { get; private set; }

        private ObservableCollection<CameraDataPackage> _eventLog;
        public ObservableCollection<CameraDataPackage> EventLog
        {
            get { return _eventLog; }
            set
            {
                //if (_eventLog != null && !value.SequenceEqual(_eventLog))
                    _eventLog = value;
                OnPropertyChanged(nameof(Network));
            }
        }

        private Action CameraSelectionByUserHandler;

        public event Action<Camera> CameraSelected;
        public event Action BigBrotherClosedEye;

        public CentralProcessor()
        {
            Network = new ObservableDictionary<string, PeripheralProcessor>();
            Network.Add(NONSELECTED_CAMERA_STR, null);
            EventLog = new ObservableCollection<CameraDataPackage>();

            CameraSelected += (cam) => IsInObservingMode = true;
        }

        public async Task SurveyNetwork()
        {
            await Task.FromResult(0);

            while (true)
            {
                SurveyUser();
                foreach (PeripheralProcessor pProc in Network.Values.Where(pp => pp != null))
                    SurveyPeripheral(pProc);
            }

            return;

            void  SurveyUser()
            {
                if (CameraSelectionByUserHandler != null)
                {
                    CameraSelectionByUserHandler();
                    CameraSelectionByUserHandler = null;
                }
            }

            void SurveyPeripheral(PeripheralProcessor pProc)
            {
                CameraDataPackage cdp = pProc.SendPackage();
                cdp.Accept(this);
            }
        }

        public void Visit(CameraStandardSituationDataPackage package)
        {
            ;
        }

        public void Visit(CameraEmployeeArrivalDataPackage package)
        {
            if (EventLog.Count > 10)
                return;
            if (package.IsFirstInSeries)
            {
                EventLog.Add(package);
            }
        }

        public void Visit(CameraEmployeeDepartureDataPackage package)
        {
            if (package.IsFirstInSeries)
            {
                EventLog.Add(package);
            }
        }

        public void Visit(CameraOutsiderOnObjectDataPackage package)
        {
            if (package.IsFirstInSeries)
            {
                EventLog.Add(package);
                if (!IsInObservingMode)
                {
                    CameraSelected?.Invoke(Network[package.CameraName].AgregatedCamera);
                }
            }
        }

        private void CameraSelectionByUser(string cameraName)
        {
            if (cameraName == NONSELECTED_CAMERA_STR)
            {
                IsInObservingMode = false;
                BigBrotherClosedEye();
            }
            else
            {
                CameraSelected?.Invoke(Network[cameraName].AgregatedCamera);
            }
        }

        public void OnCameraSelected(string cameraName)
        {
            CameraSelectionByUserHandler += () => CameraSelectionByUser(cameraName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
