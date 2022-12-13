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
    public class PeripheralProcessor : IEquatable<PeripheralProcessor>
    {
        public readonly Camera AgregatedCamera;

        public readonly string CameraName;
        
        public PeripheralProcessor(Camera camera, string cameraName)
        {
            AgregatedCamera = camera;
            CameraName = cameraName;
        }

        public CameraDataPackage SendPackage()
        {
            CameraDataPackage package = AgregatedCamera.SendPackage();
            package.CameraName = this.CameraName;
            return package;
        }

        public bool Equals(PeripheralProcessor other)
        {
            return this.CameraName == other.CameraName;
        }
    }
}
