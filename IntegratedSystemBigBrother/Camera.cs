using System;
using System.Collections.Generic;
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
        public delegate Task CameraScheduledBehaviorAsyncDelegate(
            TimeSpan duration, 
            params object[] additionParams);

        private List<Func<Task>> _behaviorSchedule;
        
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

        public Camera()
        {
            DrawCorridor();
            SendPackage = () => new CameraStandardSituationDataPackage(DateTime.Now, IsFirstPackageInSeries);
            _behaviorSchedule = new List<Func<Task>>();
            Task.Run((Action)ScheduleLoop);
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

        public void AddStandardSituationToSchedule(TimeSpan duration)
        {
            _behaviorSchedule.Add(async () =>
            {
                await SetSendStandardSituationDataPackageBehavior(duration);
            });
        }

        public void AddEmployeeArrivalToSchedule(TimeSpan duration, string employeeName)
        {
            _behaviorSchedule.Add(async () =>
            {
                await SetSendEmployeeArrivalDataPackageBehavior(duration, employeeName);
            });
        }

        public void AddEmployeeDepartureToSchedule(TimeSpan duration, string employeeName)
        {
            _behaviorSchedule.Add(async () =>
            {
                await SetSendEmployeeDepartureDataPackageBehavior(duration, employeeName);
            });
        }

        public void AddOutsiderOnObjectToSchedule(TimeSpan duration)
        {
            _behaviorSchedule.Add(async () =>
            {
                await SetSendOutsiderOnObjectDataPackageBehavior(duration);
            });
        }
    }
}
