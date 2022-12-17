using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace IntegratedSystemBigBrother
{
    public abstract class CameraListener : ICameraDataPackageVisitor, IDisposable
    {
        protected Camera _listenedCamera;

        public readonly string CameraName;

        protected static bool _stopListenCamera;

        protected CameraDataPackage.EventDescription _observedEvent;

        protected static bool _showActor;

        protected static CameraScheduler _scheduler = ISBBViewModel.Scheduler;

        public CameraListener(PeripheralProcessor listenedPPU)
        {
            _stopListenCamera = false;
            _listenedCamera = listenedPPU.AgregatedCamera;
            CameraName = listenedPPU.CameraName;
        }

        public async Task Listen()
        {
            OnStartListenCamera();
            while (!_stopListenCamera)
            {
                await OnListenCamera();
            }
        }

        protected virtual void OnStartListenCamera()
        {
            lock (_scheduler.CameraBehaviorEventLockingObject)
            {
                //_scheduler.CameraBehaviorStart += StartObserve;
                _scheduler.CameraBehaviorEnd += StopObserve;
            }
        }

        protected async Task OnListenCamera()
        {
            CameraDataPackage.EventDescription prevObservedEvent = _observedEvent;
            lock (_listenedCamera.StateLockerObject)
            {
                _listenedCamera.SendPackage().Accept(this); // Делается в обход периферийного процессора.
            }
            if (_observedEvent != prevObservedEvent && _showActor)
            {
                lock (_scheduler.CameraBehaviorEventLockingObject)
                {
                    _scheduler.CameraBehaviorStart += ShowActorAndStartAnimation;
                    _scheduler.CameraBehaviorEnd += HideActor;
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        protected virtual void OnStopListenCamera()
        {
            lock (ISBBViewModel.Scheduler.CameraBehaviorEventLockingObject)
            {
                //_scheduler.CameraBehaviorStart -= StartObserve;
                _scheduler.CameraBehaviorEnd -= StopObserve;
            }
        }

        public async Task StartObserve(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
        {
            if (ppu.CameraName != CameraName) return;
            //DispatchScreen(() => AddChildOnScreen(ppu.AgregatedCamera.Actor, ""));
            if (behaviorRecord.IsRunning)
            {
                Storyboard animation = _listenedCamera.Animation;
                TimeSpan seekTime = behaviorRecord.Duration - behaviorRecord.TimeToEnd;
                ISBBViewModel.DispatchAnimation(animation, () => animation.Seek(seekTime));
            }
            _listenedCamera.StartObserve();
        }

        /// <summary>
        /// Одноразовый обработчик события запуска поведения камеры.
        /// Должен изъять себя из списка обработчиков события при срабатывании.
        /// Добавляет актёра на экран.
        /// </summary>
        /// <param name="ppu"></param>
        /// <param name="behaviorRecord"></param>
        /// <returns></returns>
        public async Task ShowActorAndStartAnimation(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
        {
            lock (_scheduler.CameraBehaviorEventLockingObject)
            {
                _scheduler.CameraBehaviorStart -= ShowActorAndStartAnimation;
            }
            ISBBViewModel.AddActorOnScreen(_listenedCamera.Actor, "actor");
            await StartObserve(ppu, behaviorRecord);
        }

        /// <summary>
        /// Одноразовый обработчик события окончания поведения камеры.
        /// Должен изъять себя из списка обработчиков события при срабатывании.
        /// Скрывает актёра с экрана.
        /// </summary>
        /// <param name="ppu"></param>
        /// <param name="behaviorRecord"></param>
        /// <returns></returns>
        public async Task HideActor(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
        {
            lock (_scheduler.CameraBehaviorEventLockingObject)
            {
                _scheduler.CameraBehaviorEnd -= HideActor;
                _scheduler.CameraBehaviorEnd -= StopObserve;
            }
            ISBBViewModel.RemoveActorFromScreen(_listenedCamera.Actor, "actor");
        }

        public async Task StopObserve(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
        {
            _listenedCamera.StopObserve();
        }

        public void Dispose()
        {
            _stopListenCamera = true;
            OnStopListenCamera();
        }

        public virtual void Visit(CameraStandardSituationDataPackage package)
        {
            if (package.MessageImportance != _observedEvent)
            {
                _observedEvent = package.MessageImportance;
                ISBBViewModel.ClearChildrenFromScreenDispatched();
                ISBBViewModel.ShowCorridor(_listenedCamera);
                _showActor = false;
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
                _showActor = true;
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
                _showActor = true;
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
                _showActor = true;
            }
        }
    }

    public class CameraOnScreenListener : CameraListener
    {
        public CameraOnScreenListener(PeripheralProcessor listenedPPU) 
            : base(listenedPPU)
        { }
    }

    /*
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
    */
}
