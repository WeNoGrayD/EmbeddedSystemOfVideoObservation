using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace IntegratedSystemBigBrother
{
    public abstract class CameraListener : ICameraDataPackageVisitor, IDisposable
    {
        protected Camera _listenedCamera;

        public readonly string CameraName;

        protected static bool _stopListenCamera;

        protected CameraDataPackage.EventDescription _observedEvent;

        public CameraListener(PeripheralProcessor listenedPPU)
        {
            _stopListenCamera = false;
            _listenedCamera = listenedPPU.AgregatedCamera;
            CameraName = listenedPPU.CameraName;
        }

        public void Listen()
        {
            while (!_stopListenCamera)
            {
                OnListenCamera();
            }
        }

        protected virtual void OnListenCamera()
        {
            _listenedCamera.SendPackage().Accept(this); // Делается в обход периферийного процессора.
            Task.Delay(TimeSpan.FromSeconds(1));
        }

        public void Dispose()
        {
            _stopListenCamera = true;
        }

        public virtual void Visit(CameraStandardSituationDataPackage package)
        {
            if (package.MessageImportance != _observedEvent)
            {
                _observedEvent = package.MessageImportance;
                ISBBViewModel.ClearChildrenFromScreenDispatched();
                ISBBViewModel.ShowCorridor(_listenedCamera);
            }
        }

        public virtual void Visit(CameraEmployeeArrivalDataPackage package)
        {
            if (package.MessageImportance != _observedEvent)
            {
                _observedEvent = package.MessageImportance;
                ISBBViewModel.ClearChildrenFromScreenDispatched();
                ISBBViewModel.ShowCorridor(_listenedCamera);
                ISBBViewModel.ShowEmployee(_listenedCamera);
            }
        }

        public virtual void Visit(CameraEmployeeDepartureDataPackage package)
        {
            if (package.MessageImportance != _observedEvent)
            {
                _observedEvent = package.MessageImportance;
                ISBBViewModel.ClearChildrenFromScreenDispatched();
                ISBBViewModel.ShowCorridor(_listenedCamera);
                ISBBViewModel.ShowEmployee(_listenedCamera);
            }
        }

        public virtual void Visit(CameraOutsiderOnObjectDataPackage package)
        {
            if (package.MessageImportance != _observedEvent)
            {
                _observedEvent = package.MessageImportance;
                ISBBViewModel.ClearChildrenFromScreenDispatched();
                ISBBViewModel.ShowCorridor(_listenedCamera);
                ISBBViewModel.ShowOutsider(_listenedCamera);
            }
        }
    }

    public class CameraOnScreenListener : CameraListener
    {
        public CameraOnScreenListener(PeripheralProcessor listenedPPU) 
            : base(listenedPPU)
        { }
    }

    public class CameraBackgroundListener : CameraListener
    {
        private ReadOnlyCollection<Camera> _network;

        public CameraBackgroundListener(CentralProcessor cpu)
            : base(null)
        {
            cpu.PropertyChanged += OnNotObservingMode;
            _network = new ReadOnlyCollection<Camera>(cpu.Network.Values.Select(ppu => ppu.AgregatedCamera).ToList());
        }

        public void OnNotObservingMode(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CentralProcessor.IsInObservingMode))
            {
                if (!((CentralProcessor)sender).IsInObservingMode)
                {
                    _stopListenCamera = false;
                    Task.Run((Action)Listen);
                }
                else
                {
                    _stopListenCamera = true;
                }
            }
        }

        protected override void OnListenCamera()
        {
            foreach (Camera camera in _network)
            {
                _listenedCamera = camera;
                base.OnListenCamera();
            }
        }

        public override void Visit(CameraStandardSituationDataPackage package)
        { ; }

        public override void Visit(CameraEmployeeArrivalDataPackage package)
        { ; }

        public override void Visit(CameraEmployeeDepartureDataPackage package)
        { ; }
    }
}
