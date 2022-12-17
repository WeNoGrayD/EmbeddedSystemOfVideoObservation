using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace IntegratedSystemBigBrother
{
    /// <summary>
    /// Основная часть абстрактного класса камеры.
    /// </summary>
    public abstract partial class Camera
    {
        public virtual string TypeKey { get { return null; } }

        public delegate Task CameraScheduledBehaviorAsyncDelegate(
            TimeSpan duration, 
            params object[] additionParams);

        private List<Func<Task>> _behaviorSchedule;
        public readonly ObservableCollection<CameraBehaviorRecord> BehaviorSchedule;

        public Func<CameraDataPackage> SendPackage;

        private bool _isFirstPackageInSeries;
        public bool IsFirstPackageInSeries
        {
            get
            {
                bool saved = _isFirstPackageInSeries;
                _isFirstPackageInSeries = false;
                return saved;
            }
            set
            {
                _isFirstPackageInSeries = value;
            }
        }

        public Path Corridor { get; protected set; }

        public Path Actor { get; set; }

        public Storyboard Animation { get; protected set; }

        public readonly object StateLockerObject = 1; 

        public Camera()
        {
            DrawCorridor();
            SendPackage = () => new CameraStandardSituationDataPackage(DateTime.Now, IsFirstPackageInSeries);
            _behaviorSchedule = new List<Func<Task>>();
            BehaviorSchedule = new ObservableCollection<CameraBehaviorRecord>();
            //Task.Run((Action)ScheduleLoop);
        }

        public Camera(List<Func<Task>> behaviorSchedule) : this()
        {
            _behaviorSchedule = behaviorSchedule;
        }

        public void ScheduleLoop()
        {
            while(true)
            {
                foreach (Func<Task> behavior in _behaviorSchedule)
                {
                    Animation?.Stop();
                    IsFirstPackageInSeries = true;
                    behavior().Wait();
                }
            }
        }

        public void StartObserve()
        {
            if (Animation != null)
                Actor.Dispatch(() => Actor.BeginStoryboard(Animation));
        }

        public void StopObserve()
        {
            if (Animation != null)
                Animation.Dispatch((Action)Animation.Stop);
        }

        protected void AddBehaviorToSchedule(Func<Task> behavior, TimeSpan duration)
        {
            _behaviorSchedule.Add(behavior);
            CameraBehaviorRecord behaviorRecord = new CameraBehaviorRecord(behavior, duration);
            BehaviorSchedule.Add(behaviorRecord);
        }

        public void AddStandardSituationToSchedule(TimeSpan duration)
        {
            Func<Task> behavior = async () =>
            {
                await SetSendStandardSituationDataPackageBehavior(duration);
            };
            AddBehaviorToSchedule(behavior, duration);
        }

        public void AddEmployeeArrivalToSchedule(TimeSpan duration, string employeeName)
        {
            Func<Task> behavior = async () =>
            {
                await SetSendEmployeeArrivalDataPackageBehavior(duration, employeeName);
            };
            AddBehaviorToSchedule(behavior, duration);
        }

        public void AddEmployeeDepartureToSchedule(TimeSpan duration, string employeeName)
        {
            Func<Task> behavior = async () =>
            {
                await SetSendEmployeeDepartureDataPackageBehavior(duration, employeeName);
            };
            AddBehaviorToSchedule(behavior, duration);
        }

        public void AddOutsiderOnObjectToSchedule(TimeSpan duration)
        {
            Func<Task> behavior = async () =>
            {
                await SetSendOutsiderOnObjectDataPackageBehavior(duration);
            };
            AddBehaviorToSchedule(behavior, duration);
        }
    }
}
