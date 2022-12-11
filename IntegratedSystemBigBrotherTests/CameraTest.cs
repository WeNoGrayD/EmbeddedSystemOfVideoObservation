using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegratedSystemBigBrother;
using System.Linq;
using System.IO;

namespace IntegratedSystemBigBrotherTests
{
    [TestClass]
    public class CameraTest
    {
        Camera cam;

        [TestInitialize]
        public void InitCamera()
        {
        }

        [TestMethod]
        public async Task SimpleTest()
        {
            /*
            using (StreamWriter log = new StreamWriter(new FileStream(@"../../TestLog/CameraTest/SimpleTest.txt", FileMode.OpenOrCreate)))
            {
                cam = new Camera();
                cam.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(5), "Иван Петрович");
                cam.AddStandardSituationToSchedule(TimeSpan.FromSeconds(2));
                cam.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(4));
                cam.AddStandardSituationToSchedule(TimeSpan.FromSeconds(2));
                cam.AddEmployeeDepartureToSchedule(TimeSpan.FromSeconds(5), "Иван Петрович");
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                CameraDataPackage cdp = cam.SendPackage();
                log.WriteLine(cdp.TimeStamp.ToString() + ": " + cdp.GetMessage());
                await Task.Delay(TimeSpan.FromSeconds(5));
                cdp = cam.SendPackage();
                log.WriteLine(cdp.TimeStamp.ToString() + ": " + cdp.GetMessage());
                await Task.Delay(TimeSpan.FromSeconds(2));
                cdp = cam.SendPackage();
                log.WriteLine(cdp.TimeStamp.ToString() + ": " + cdp.GetMessage());
                await Task.Delay(TimeSpan.FromSeconds(4));
                cdp = cam.SendPackage();
                log.WriteLine(cdp.TimeStamp.ToString() + ": " + cdp.GetMessage());
                await Task.Delay(TimeSpan.FromSeconds(2));
                cdp = cam.SendPackage();
                log.WriteLine(cdp.TimeStamp.ToString() + ": " + cdp.GetMessage());
                await Task.Delay(TimeSpan.FromSeconds(5));
                cdp = cam.SendPackage();
                log.WriteLine(cdp.TimeStamp.ToString() + ": " + cdp.GetMessage());
            }
            */
        }
    }
}
