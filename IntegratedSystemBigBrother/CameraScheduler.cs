using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;

namespace IntegratedSystemBigBrother
{
    public class CameraScheduler
    {
        private CentralProcessor _cpu;

        private Dictionary<string, LinkedList<CameraBehaviorRecord>> _schedule;

        public event Func<PeripheralProcessor, CameraBehaviorRecord, Task> CameraBehaviorStart;

        public event Func<PeripheralProcessor, CameraBehaviorRecord, Task> CameraBehaviorEnd;

        public readonly object CameraBehaviorEventLockingObject = 0; 

        public CameraScheduler(CentralProcessor cpu)
        {
            _cpu = cpu;
            _schedule = new Dictionary<string, LinkedList<CameraBehaviorRecord>>();
            _cpu.Network.CollectionChanged += AddCameraToSchedule;
            CameraBehaviorStart += ManageCameraBehavior;
        }

        private async Task ManageCameraBehavior(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
        {
            if (!behaviorRecord.IsRunning)
            {
                await behaviorRecord.Behavior();
            }
        }

        private void AddCameraToSchedule(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            Dictionary<string, PeripheralProcessor> newCameras = 
                e.NewItems.Cast<KeyValueTuple<string, PeripheralProcessor>>()
                          .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (string cameraName in newCameras.Keys)
            {
                _schedule.Add(cameraName, new LinkedList<CameraBehaviorRecord>());
                SubscribeOnCameraBehaviorUpdating(cameraName, newCameras[cameraName]);
            }
        }

        private void SubscribeOnCameraBehaviorUpdating(string cameraName, PeripheralProcessor ppu)
        {
            ppu.AgregatedCamera.BehaviorSchedule.CollectionChanged +=
                (ccsender, cce) =>
                {
                    if (cce.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (CameraBehaviorRecord newBehaviorRecord in cce.NewItems)
                            AddBehaviorToCameraSchedule(cameraName, newBehaviorRecord);
                    }
                };
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

        public async Task RunScheduleAsync(CancellationToken token)
        {
            TimeSpan wastedTime,
                     delayTime = TimeSpan.FromSeconds(1);

            ClearEmptyCameraSchedules();
            while (true)
            {
                if (token.IsCancellationRequested) return;
                RunUrgentBehaviors();
                wastedTime = RetrieveClosiestBehaviorTimeToStart();
                await WaitFor(delayTime);
                ShiftSchedule(delayTime);
            }

            return;
        }

        private void ClearEmptyCameraSchedules()
        {
            Dictionary<string, LinkedList<CameraBehaviorRecord>> nonEmptyCameraSchedules = 
                _schedule.Where(record => record.Value.Count > 0)
                         .ToDictionary();
            _schedule.Clear();
            _schedule = nonEmptyCameraSchedules;
        }

        private void RunUrgentBehaviors()
        {
            LinkedList<CameraBehaviorRecord> cameraSchedule;
            CameraBehaviorRecord behaviorRecord;
            foreach (string cameraName in _schedule.Keys)
            {
                cameraSchedule = _schedule[cameraName];
                behaviorRecord = cameraSchedule.First();
                lock (CameraBehaviorEventLockingObject)
                {
                    CameraBehaviorStart?.Invoke(_cpu.Network[cameraName], behaviorRecord);
                }
                if (!behaviorRecord.IsRunning)
                {
                    cameraSchedule.RemoveFirst();
                    behaviorRecord = behaviorRecord.Run();
                    cameraSchedule.AddFirst(behaviorRecord);
                }
            }
        }

        private TimeSpan RetrieveClosiestBehaviorTimeToStart()
        {
            Dictionary<string, CameraBehaviorRecord> nextBehaviors =
                _schedule.Keys.ToDictionary(cameraName => cameraName, cameraName => _schedule[cameraName].First());
            double closiestTimeToEnd = nextBehaviors.Min(record => record.Value.TimeToEnd.TotalMilliseconds);
            return TimeSpan.FromMilliseconds(closiestTimeToEnd);
        }

        private async Task WaitFor(TimeSpan wastedTime)
        {
            await Task.Delay(wastedTime);
        }

        private void ShiftSchedule(TimeSpan wastedTime)
        {
            CameraBehaviorRecord behaviorRecord;
            foreach (string cameraName in _schedule.Keys)
            {
                behaviorRecord = _schedule[cameraName].First();
                if (behaviorRecord.TimeToEnd - wastedTime == TimeSpan.Zero)
                    ShiftBehaviorRecordWithZeroTimeToEnd(cameraName);
                else
                    ShiftBehaviorRecordWithNonZeroTimeToEnd(cameraName);
            }

            return;

            void ShiftBehaviorRecordWithNonZeroTimeToEnd(string cameraName)
            {
                _schedule[cameraName].RemoveFirst();
                behaviorRecord = behaviorRecord.UpdateTimeToEnd(wastedTime);
                _schedule[cameraName].AddFirst(behaviorRecord);
            }

            void ShiftBehaviorRecordWithZeroTimeToEnd(string cameraName)
            {
                lock (CameraBehaviorEventLockingObject)
                {
                    CameraBehaviorEnd?.Invoke(_cpu.Network[cameraName], behaviorRecord);
                }
                _schedule[cameraName].RemoveFirst();
                behaviorRecord = behaviorRecord.Restart();
                _schedule[cameraName].AddLast(behaviorRecord);
            }
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
                this.Duration);
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
