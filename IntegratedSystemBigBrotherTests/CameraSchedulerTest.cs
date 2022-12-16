using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegratedSystemBigBrother;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IntegratedSystemBigBrotherTests
{
    [TestClass]
    public class CameraSchedulerTest
    {
        private class CameraBehaviorLogger : ICameraDataPackageVisitor, IDisposable
        {
            public readonly StreamWriter Log;

            public CameraBehaviorLogger(StreamWriter log, CameraScheduler scheduler)
            {
                Log = log;

                scheduler.CameraBehaviorStart += OnCameraBehaviorStart;
                //scheduler.CameraBehaviorEnd += OnCameraBehaviorEnd;
            }

            public void Dispose()
            {
                Log.Dispose();
            }

            private async Task OnCameraBehaviorStart(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
            {
                await Task.Run(() => ppu.SendPackage().Accept(this));
            }

            /*
            private async Task OnCameraBehaviorEnd(PeripheralProcessor ppu, CameraBehaviorRecord)
            {

            }
            */

            public void LogPackage(CameraDataPackage cdp)
            {
                Log.WriteLine($"{cdp.CameraName}/{cdp.TimeStamp.ToString()} : {cdp.GetMessage()}");
            }

            public void Visit(CameraStandardSituationDataPackage cdp)
            {
                LogPackage(cdp);
            }

            public void Visit(CameraEmployeeArrivalDataPackage cdp)
            {
                LogPackage(cdp);
            }

            public void Visit(CameraEmployeeDepartureDataPackage cdp)
            {
                LogPackage(cdp);
            }

            public void Visit(CameraOutsiderOnObjectDataPackage cdp)
            {
                LogPackage(cdp);
            }
        }

        [TestMethod]
        public async Task SimpleTest()
        {
            CentralProcessor cpu = new CentralProcessor();

            CameraScheduler scheduler = new CameraScheduler(cpu);

            Camera cam1 = new CameraWatchingFromLeftCornerWall(),
                   cam2 = new CameraWatchingFromWallCentre(),
                   cam3 = new CameraWatchingFromRightCornerWall(),
                   cam4 = new CameraWatchingFromWallCentre();

            cpu.Network.Add("Камера 1", new PeripheralProcessor(cam1, "Камера 1"));
            cpu.Network.Add("Камера 2", new PeripheralProcessor(cam2, "Камера 2"));
            cpu.Network.Add("Камера 3", new PeripheralProcessor(cam3, "Камера 3"));
            cpu.Network.Add("Камера 4", new PeripheralProcessor(cam4, "Камера 4"));

            cam1.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(3), "Иван Петрович");
            cam1.AddEmployeeDepartureToSchedule(TimeSpan.FromSeconds(5), "Семён Семёныч");
            cam1.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(4));

            cam2.AddEmployeeDepartureToSchedule(TimeSpan.FromSeconds(4), "Олег Егорыч");
            cam2.AddStandardSituationToSchedule(TimeSpan.FromSeconds(3));
            cam2.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(3), "Вадим Вадимыч");

            cam3.AddStandardSituationToSchedule(TimeSpan.FromSeconds(4));
            cam3.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(4), "Потап Ефимыч");
            cam3.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(6));

            StreamWriter log = new StreamWriter(new FileStream(@"../../TestLog/CameraSchedulerTest/SimpleTest.txt", FileMode.OpenOrCreate), System.Text.Encoding.UTF8);
            CameraBehaviorLogger logger = new CameraBehaviorLogger(log, scheduler);

            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                tokenSource.CancelAfter(TimeSpan.FromSeconds(600));
                await Task.Run(() => scheduler.RunScheduleAsync(tokenSource.Token));
            }
            logger.Dispose();
        }

        private async Task ControlScheduleRunning(
            CameraBehaviorLogger logger,
            CancellationTokenSource tokenSource)
        {
            DateTime start = DateTime.Now;
            TimeSpan limit = TimeSpan.FromSeconds(60);
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                if (DateTime.Now - start > limit)
                    break;
            }
            tokenSource.Cancel();
            logger.Dispose();
        }
    }
}
