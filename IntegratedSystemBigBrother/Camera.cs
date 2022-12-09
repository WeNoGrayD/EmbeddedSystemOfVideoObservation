using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace IntegratedSystemBigBrother
{
    public class Camera
    {
        public delegate Task CameraScheduledBehaviorAsyncDelegate(
            Camera cam, 
            TimeSpan duration, 
            params object[] additionParams);

        private List<Func<Task>> _behaviorSchedule;
        
        public Func<CameraDataPackage> SendPackage;

        public Func<Bitmap> MakeSnapshot;

        public Image

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
            _behaviorSchedule.Add(async () => await SendStandardSituationDataPackage(this, duration));
        }

        public void AddEmployeeArrivalToSchedule(TimeSpan duration, string employeeName)
        {
            _behaviorSchedule.Add(async () => await SendEmployeeArrivalDataPackage(this, duration, employeeName));
        }

        public void AddEmployeeDepartureToSchedule(TimeSpan duration, string employeeName)
        {
            _behaviorSchedule.Add(async () => await SendEmployeeDepartureDataPackage(this, duration, employeeName));
        }

        public void AddOutsiderOnObjectToSchedule(TimeSpan duration)
        {
            _behaviorSchedule.Add(async () => await SendOutsiderOnObjectDataPackage(this, duration));
        }

        private async Task SendStandardSituationDataPackage(
            Camera cam, 
            TimeSpan duration,
            params object[] additionParams)
        {
            SendPackage = () => new CameraStandardSituationDataPackage(DateTime.Now);
            await Task.Delay(duration);
            return;
        }

        private async Task SendEmployeeArrivalDataPackage(
            Camera cam, 
            TimeSpan duration,
            params object[] additionParams)
        {
            string employeeName = (string)additionParams[0];
            cam.SendPackage = () => new CameraEmployeeArrivalDataPackage(DateTime.Now, employeeName);
            await Task.Delay(duration);
            return;
        }

        private async Task SendEmployeeDepartureDataPackage(
            Camera cam,
            TimeSpan duration,
            params object[] additionParams)
        {
            string employeeName = (string)additionParams[0];
            cam.SendPackage = () => new CameraEmployeeDepartureDataPackage(DateTime.Now, employeeName);
            await Task.Delay(duration);
            return;
        }

        private async Task SendOutsiderOnObjectDataPackage(
            Camera cam,
            TimeSpan duration,
            params object[] additionParams)
        {
            cam.SendPackage = () => new CameraOutsiderOnObjectDataPackage(DateTime.Now);
            await Task.Delay(duration);
            return;
        }
    }
}
