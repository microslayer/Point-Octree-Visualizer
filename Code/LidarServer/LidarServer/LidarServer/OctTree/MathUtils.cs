using System;

namespace LidarServer
{
    public class MathUtils
    {
        public static double Min(double a, double b, double c)
        {
            return Math.Min(Math.Min(a, b), c);
        }

        public static double Max(double a, double b, double c)
        {
            return Math.Max(Math.Max(a, b), c);
        }

		internal static Vector3 Average(Vector3 min, Vector3 max)
		{
            return new Vector3((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2); 
		}
	}
}
