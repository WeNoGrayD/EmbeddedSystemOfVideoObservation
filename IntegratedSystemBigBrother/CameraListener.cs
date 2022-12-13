using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedSystemBigBrother
{
    public class CameraListener : ICameraDataPackageVisitor, IDisposable
    {
        private static Camera _listenedCamera;

        private static bool _stopListenCamera;

        public CameraListener(Camera listenedCamera)
        {
            _stopListenCamera = false;
            _listenedCamera = listenedCamera;
        }

        public void ListenCamera()
        {
            while (!_stopListenCamera)
            {
                _listenedCamera.SendPackage().Accept(this); // Делается в обход периферийного процессора.
                Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public void Dispose()
        {
            _stopListenCamera = true;
        }

        public void Visit(CameraStandardSituationDataPackage package)
        {
            ;
        }

        public void Visit(CameraEmployeeArrivalDataPackage package)
        {
            ISBBViewModel.ShowEmployee();
        }

        public void Visit(CameraEmployeeDepartureDataPackage package)
        {
            ISBBViewModel.ShowEmployee();
        }

        public void Visit(CameraOutsiderOnObjectDataPackage package)
        {
            ISBBViewModel.ShowOutsider();
        }
    }
}
