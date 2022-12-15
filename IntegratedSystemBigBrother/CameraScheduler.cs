using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;

namespace IntegratedSystemBigBrother
{
    public class CameraScheduler
    {
        private Dictionary<string, LinkedList<CameraBehaviorRecord>> _schedule;

        public CameraScheduler(CentralProcessor cpu)
        {
            cpu.Network.CollectionChanged += AddCameraToSchedule;
        }

        private async Task RunSchedule()
        {
            (string CameraName, CameraBehaviorRecord BehaviorRecord) closiest;

            ClearEmptyCameraSchedules();
            while (true)
            {
                RunUrgentBehaviors();
                closiest = SelectClosiestBehavior();
                await WaitFor(closiest.BehaviorRecord);
                ShiftSchedule(closiest);
            }
        }

        private void ClearEmptyCameraSchedules()
        {
            _schedule = _schedule.Where(record => record.Value.Count > 0)
                .ToDictionary(record => record.Key, record => record.Value);
        }

        private void RunUrgentBehaviors()
        {
            CameraBehaviorRecord behaviorRecord;
            foreach (LinkedList<CameraBehaviorRecord> cameraSchedule in _schedule.Values)
            {
                behaviorRecord = cameraSchedule.First();
                if (!behaviorRecord.IsRunning)
                {
                    cameraSchedule.RemoveFirst();
                    behaviorRecord = behaviorRecord.Run();
                    cameraSchedule.AddFirst(behaviorRecord);
                }
            }
        }

        private (string CameraName, CameraBehaviorRecord BehaviorRecord) SelectClosiestBehavior()
        {
            Dictionary<string, CameraBehaviorRecord> nextBehaviors =
                _schedule.Keys.ToDictionary(cameraName => cameraName, cameraName => _schedule[cameraName].First());
            double closiestTimeToEnd = nextBehaviors.Min(record => record.Value.TimeToEnd.TotalMilliseconds);
            KeyValuePair<string, CameraBehaviorRecord> closiest = 
                nextBehaviors.First(record => record.Value.TimeToEnd.TotalMilliseconds == closiestTimeToEnd);
            return (closiest.Key, closiest.Value);
        }

        private async Task WaitFor(CameraBehaviorRecord behaviorRecord)
        {
            await Task.Delay(behaviorRecord.TimeToEnd);
        }

        private void ShiftSchedule((string CameraName, CameraBehaviorRecord BehaviorRecord) closiest)
        {
            TimeSpan wastedTime = closiest.BehaviorRecord.TimeToEnd;
            CameraBehaviorRecord behaviorRecord;
            foreach (string cameraName in _schedule.Keys.Except(new[] { closiest.CameraName }))
            {
                behaviorRecord = _schedule[cameraName].First();
                if (behaviorRecord.TimeToEnd - wastedTime == TimeSpan.Zero)
                    ShiftBehaviorRecordWithZeroTimeToEnd(cameraName);
                else
                    ShiftBehaviorRecordWithNonZeroTimeToEnd(cameraName);
            }
            behaviorRecord = _schedule[closiest.CameraName].First();
            ShiftBehaviorRecordWithZeroTimeToEnd(closiest.CameraName);

            return;

            void ShiftBehaviorRecordWithNonZeroTimeToEnd(string cameraName)
            {
                _schedule[cameraName].RemoveFirst();
                behaviorRecord = behaviorRecord.UpdateTimeToEnd(wastedTime);
                _schedule[cameraName].AddFirst(behaviorRecord);
            }

            void ShiftBehaviorRecordWithZeroTimeToEnd(string cameraName)
            {
                _schedule[cameraName].RemoveFirst();
                behaviorRecord = behaviorRecord.Restart();
                _schedule[cameraName].AddLast(behaviorRecord);
            }
        }

        private void AddCameraToSchedule(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            IEnumerable<string> newCameras = (IEnumerable<string>)e.NewItems;
            foreach(string cameraName in newCameras)
            {
                _schedule.Add(cameraName, new LinkedList<CameraBehaviorRecord>());
            }
        }

        public void AddBehaviorToCameraSchedule(
            PeripheralProcessor ppu, 
            CameraBehaviorRecord newBehavior)
        {
            AddBehaviorToCameraSchedule(ppu, newBehavior);
        }

        public void AddBehaviorToCameraSchedule(string cameraName, CameraBehaviorRecord newBehavior)
        {
            _schedule[cameraName].AddLast(newBehavior);
        }
    }

    public struct CameraBehaviorRecord
    {
        public readonly Func<Task> Behavior;

        public readonly TimeSpan Duration;

        public readonly TimeSpan TimeToEnd;

        public readonly bool IsRunning;

        public CameraBehaviorRecord(Func<Task> behavior, TimeSpan duration)
            : this()
        {
            Behavior = behavior;
            Duration = duration;
            TimeToEnd = duration;
            IsRunning = false;
        }

        public CameraBehaviorRecord(Func<Task> behavior, TimeSpan duration, TimeSpan timeToEnd)
            : this()
        {
            Behavior = behavior;
            Duration = duration;
            TimeToEnd = timeToEnd;
            IsRunning = false;
        }

        public CameraBehaviorRecord(Func<Task> behavior, TimeSpan duration, TimeSpan timeToEnd, bool isRunning)
            : this()
        {
            Behavior = behavior;
            Duration = duration;
            TimeToEnd = timeToEnd;
            IsRunning = isRunning;
        }

        public CameraBehaviorRecord Run()
        {
            return new CameraBehaviorRecord(
                this.Behavior,
                this.Duration,
                this.TimeToEnd,
                true);
        }

        public CameraBehaviorRecord Restart()
        {
            return new CameraBehaviorRecord(
                this.Behavior,
                this.Duration,
                this.TimeToEnd);
        }

        public CameraBehaviorRecord UpdateTimeToEnd(TimeSpan wastedTime)
        {
            return new CameraBehaviorRecord(
                this.Behavior,
                this.Duration,
                this.TimeToEnd - wastedTime,
                this.IsRunning);
        }
    }
}
