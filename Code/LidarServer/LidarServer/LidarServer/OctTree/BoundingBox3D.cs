using System;

namespace LidarServer
{
    public class BoundingBox3D
    {
        Vector3 _min;
        Vector3 _max;

        public Vector3 Min => new Vector3(Math.Min(_min.X, _max.X),
                                          Math.Min(_min.Y, _max.Y),
                                          Math.Min(_min.Z, _max.Z));
        public Vector3 Max => new Vector3(Math.Max(_min.X, _max.X),
                                         Math.Max(_min.Y, _max.Y),
                                         Math.Max(_min.Z, _max.Z));

        public BoundingBox3D(Vector3 v1, Vector3 v2)
        {
            double minX = Math.Min(v1.X, v2.X);
            double maxX = minX == v1.X ? v2.X : v1.X;
            double minY = Math.Min(v1.Y, v2.Y);
            double maxY = minY == v1.Y ? v2.Y : v1.Y;
            double minZ = Math.Min(v1.Z, v2.Z);
            double maxZ = minZ == v1.Z ? v2.Z : v1.Z;

            _min = new Vector3(minX, minY, minZ);
            _max = new Vector3(maxX, maxY, maxZ);
        }

        public bool Contains(Vector3 point)
        {
            return (_min.X <= point.X && _max.X >= point.X
                 && _min.Y <= point.Y && _max.Y >= point.Y
                 && _min.Z <= point.Z && _max.Z >= point.Z); 
        }

        public override string ToString()
        {
            // returns w,h,d-x,y,z
            Vector3 center = MathUtils.Average(Min, Max);
            return ("{ h: " + Math.Abs(Max.X - Min.X) + ",w:" +
                    Math.Abs(Max.Y - Min.Y) + ",d:" +
                    Math.Abs(Max.Z - Min.Z) + ",x:" +
                    center.X + ",y:" + center.Y + ",z:" + center.Z + "}"); 
		}
	}
}
