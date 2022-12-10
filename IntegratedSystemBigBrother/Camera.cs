using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedSystemBigBrother
{
    /// <summary>
    /// Основная часть класса камеры.
    /// </summary>
    public partial class Camera
    {
        public delegate Task CameraScheduledBehaviorAsyncDelegate(
            TimeSpan duration, 
            params object[] additionParams);

        private List<Func<Task>> _behaviorSchedule;
        
        public Func<CameraDataPackage> SendPackage;

        //public Func<Bitmap> MakeSnapshot;

        //public Image btmp;

        public Camera()
        {
            SendPackage = () => new CameraStandardSituationDataPackage(DateTime.Now);
            _behaviorSchedule = new List<Func<Task>>();
            Task.Run((Action)ScheduleLoop);
        }

        public Camera(List<Func<Task>> behaviorSchedule) : this()
        {
            _behaviorSchedule = behaviorSchedule;
        }

        public async void ScheduleLoop()
        {
            while(true)
            {
                foreach (Func<Task> behavior in _behaviorSchedule)
                {
                    await behavior();
                }
            }
        }

        public void AddStandardSituationToSchedule(TimeSpan duration)
        {
            _behaviorSchedule.Add(async () => await SetSendStandardSituationDataPackageBehavior(duration));
        }

        public void AddEmployeeArrivalToSchedule(TimeSpan duration, string employeeName)
        {
            _behaviorSchedule.Add(async () => await SetSendEmployeeArrivalDataPackageBehavior(duration, employeeName));
        }

        public void AddEmployeeDepartureToSchedule(TimeSpan duration, string employeeName)
        {
            _behaviorSchedule.Add(async () => await SetSendEmployeeDepartureDataPackageBehavior(duration, employeeName));
        }

        public void AddOutsiderOnObjectToSchedule(TimeSpan duration)
        {
            _behaviorSchedule.Add(async () => await SetSendOutsiderOnObjectDataPackageBehavior(duration));
        }
    }
}
