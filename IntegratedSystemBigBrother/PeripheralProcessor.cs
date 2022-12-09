using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedSystemBigBrother
{
    /// <summary>
    /// Класс, отвечающий за работу с камерой.
    /// </summary>
    public class PeripheralProcessor
    {
        private Camera _camera;

        public readonly string CameraName;
        
        public PeripheralProcessor(Camera camera, string cameraName)
        {
            _camera = camera;
            CameraName = cameraName;
        }

        public CameraDataPackage SendPackage()
        {
            CameraDataPackage package = _camera.SendPackage();
            package.CameraName = this.CameraName;
            return package;
        }
    }
}
