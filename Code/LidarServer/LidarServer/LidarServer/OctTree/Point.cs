using System;

namespace LidarServer
{
    public class Point
    {
        public Vector3 Pos { get; set; }
        public double Intensity { get; set; }

        public Point(double x, double y, double z, double intensity = 0)
        {
            Pos = new Vector3(x, y, z);
            this.Intensity = intensity;
        }
    }
}
